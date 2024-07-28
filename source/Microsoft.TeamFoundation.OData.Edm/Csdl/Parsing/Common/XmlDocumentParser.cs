// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlDocumentParser
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal abstract class XmlDocumentParser
  {
    private readonly string docPath;
    private readonly Stack<XmlDocumentParser.ElementScope> currentBranch = new Stack<XmlDocumentParser.ElementScope>();
    private XmlReader reader;
    private IXmlLineInfo xmlLineInfo;
    private List<EdmError> errors;
    private StringBuilder currentText;
    private CsdlLocation currentTextLocation;
    private XmlDocumentParser.ElementScope currentScope;

    protected XmlDocumentParser(XmlReader underlyingReader, string documentPath)
    {
      this.reader = underlyingReader;
      this.docPath = documentPath;
      this.errors = new List<EdmError>();
    }

    internal string DocumentPath => this.docPath;

    internal string DocumentNamespace { get; private set; }

    internal Version DocumentVersion { get; private set; }

    internal CsdlLocation DocumentElementLocation { get; private set; }

    internal bool HasErrors { get; private set; }

    internal XmlElementValue Result { get; private set; }

    internal CsdlLocation Location
    {
      get
      {
        int number = 0;
        int position = 0;
        if (this.xmlLineInfo != null && this.xmlLineInfo.HasLineInfo())
        {
          number = this.xmlLineInfo.LineNumber;
          position = this.xmlLineInfo.LinePosition;
        }
        return new CsdlLocation(this.DocumentPath, number, position);
      }
    }

    internal IEnumerable<EdmError> Errors => (IEnumerable<EdmError>) this.errors;

    private bool IsTextNode
    {
      get
      {
        switch (this.reader.NodeType)
        {
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
          case XmlNodeType.SignificantWhitespace:
            return true;
          default:
            return false;
        }
      }
    }

    internal void ParseDocumentElement()
    {
      this.reader = this.InitializeReader(this.reader);
      this.xmlLineInfo = this.reader as IXmlLineInfo;
      if (this.reader.NodeType != XmlNodeType.Element)
      {
        while (this.reader.Read() && this.reader.NodeType != XmlNodeType.Element)
          ;
      }
      if (this.reader.EOF)
      {
        this.ReportEmptyFile();
      }
      else
      {
        this.DocumentNamespace = this.reader.NamespaceURI;
        Version version;
        string[] expectedNamespaces;
        if (this.TryGetDocumentVersion(this.DocumentNamespace, out version, out expectedNamespaces))
        {
          this.DocumentVersion = version;
          this.DocumentElementLocation = this.Location;
          bool isEmptyElement = this.reader.IsEmptyElement;
          XmlElementInfo xmlElementInfo = this.ReadElement(this.reader.LocalName, this.DocumentElementLocation);
          XmlElementParser parser;
          if (!this.TryGetRootElementParser(this.DocumentVersion, xmlElementInfo, out parser))
          {
            this.ReportUnexpectedRootElement(xmlElementInfo.Location, xmlElementInfo.Name, this.DocumentNamespace);
          }
          else
          {
            this.BeginElement(parser, xmlElementInfo);
            if (isEmptyElement)
              this.EndElement();
            else
              this.Parse();
          }
        }
        else
          this.ReportUnexpectedRootNamespace(this.reader.LocalName, this.DocumentNamespace, expectedNamespaces);
      }
    }

    protected void ReportError(
      CsdlLocation errorLocation,
      EdmErrorCode errorCode,
      string errorMessage)
    {
      this.errors.Add(new EdmError((EdmLocation) errorLocation, errorCode, errorMessage));
      this.HasErrors = true;
    }

    protected abstract XmlReader InitializeReader(XmlReader inputReader);

    protected abstract bool TryGetDocumentVersion(
      string xmlNamespaceName,
      out Version version,
      out string[] expectedNamespaces);

    protected abstract bool TryGetRootElementParser(
      Version artifactVersion,
      XmlElementInfo rootElement,
      out XmlElementParser parser);

    protected virtual bool IsOwnedNamespace(string namespaceName) => this.DocumentNamespace.EqualsOrdinal(namespaceName);

    protected virtual XmlElementParser<TResult> Element<TResult>(
      string elementName,
      Func<XmlElementInfo, XmlElementValueCollection, TResult> parserFunc,
      params XmlElementParser[] childParsers)
    {
      return XmlElementParser.Create<TResult>(elementName, parserFunc, (IEnumerable<XmlElementParser>) childParsers, (IEnumerable<XmlElementParser>) null);
    }

    private void Parse()
    {
      while (this.currentBranch.Count > 0 && this.reader.Read())
        this.ProcessNode();
      if (this.reader.EOF)
        return;
      this.reader.Read();
    }

    private void EndElement()
    {
      XmlDocumentParser.ElementScope elementScope = this.currentBranch.Pop();
      this.currentScope = this.currentBranch.Count > 0 ? this.currentBranch.Peek() : (XmlDocumentParser.ElementScope) null;
      XmlElementValue xmlElementValue1 = elementScope.Parser.Parse(elementScope.Element, elementScope.ChildValues);
      if (xmlElementValue1 != null)
      {
        if (this.currentScope != null)
          this.currentScope.AddChildValue(xmlElementValue1);
        else
          this.Result = xmlElementValue1;
      }
      foreach (XmlAttributeInfo xmlAttributeInfo in elementScope.Element.Attributes.Unused)
        this.ReportUnexpectedAttribute(xmlAttributeInfo.Location, xmlAttributeInfo.Name);
      IEnumerable<XmlElementValue> source1 = elementScope.ChildValues.Where<XmlElementValue>((Func<XmlElementValue, bool>) (v => v.IsText));
      IEnumerable<XmlElementValue> source2 = source1.Where<XmlElementValue>((Func<XmlElementValue, bool>) (t => !t.IsUsed));
      if (source2.Any<XmlElementValue>())
      {
        XmlTextValue xmlTextValue = source2.Count<XmlElementValue>() != source1.Count<XmlElementValue>() ? (XmlTextValue) source2.First<XmlElementValue>() : (XmlTextValue) source1.First<XmlElementValue>();
        this.ReportTextNotAllowed(xmlTextValue.Location, xmlTextValue.Value);
      }
      foreach (XmlElementValue xmlElementValue2 in elementScope.ChildValues.Where<XmlElementValue>((Func<XmlElementValue, bool>) (v => !v.IsText && !v.IsUsed)))
        this.ReportUnusedElement(xmlElementValue2.Location, xmlElementValue2.Name);
    }

    private void BeginElement(XmlElementParser elementParser, XmlElementInfo element)
    {
      XmlDocumentParser.ElementScope elementScope = new XmlDocumentParser.ElementScope(elementParser, element);
      this.currentBranch.Push(elementScope);
      this.currentScope = elementScope;
    }

    private void ProcessNode()
    {
      if (this.IsTextNode)
      {
        if (this.currentText == null)
        {
          this.currentText = new StringBuilder();
          this.currentTextLocation = this.Location;
        }
        this.currentText.Append(this.reader.Value);
      }
      else
      {
        if (this.currentText != null)
        {
          string textValue = this.currentText.ToString();
          CsdlLocation currentTextLocation = this.currentTextLocation;
          this.currentText = (StringBuilder) null;
          this.currentTextLocation = (CsdlLocation) null;
          if (!EdmUtil.IsNullOrWhiteSpaceInternal(textValue) && !string.IsNullOrEmpty(textValue))
            this.currentScope.AddChildValue((XmlElementValue) new XmlTextValue(currentTextLocation, textValue));
        }
        switch (this.reader.NodeType)
        {
          case XmlNodeType.Element:
            this.ProcessElement();
            break;
          case XmlNodeType.EntityReference:
          case XmlNodeType.DocumentType:
            this.reader.Skip();
            break;
          case XmlNodeType.ProcessingInstruction:
            break;
          case XmlNodeType.Comment:
            break;
          case XmlNodeType.Notation:
            break;
          case XmlNodeType.Whitespace:
            break;
          case XmlNodeType.EndElement:
            this.EndElement();
            break;
          case XmlNodeType.XmlDeclaration:
            break;
          default:
            this.ReportUnexpectedNodeType(this.reader.NodeType);
            this.reader.Skip();
            break;
        }
      }
    }

    private void ProcessElement()
    {
      bool isEmptyElement = this.reader.IsEmptyElement;
      string namespaceUri = this.reader.NamespaceURI;
      string localName = this.reader.LocalName;
      if (namespaceUri == this.DocumentNamespace)
      {
        XmlElementParser elementParser;
        if (!this.currentScope.Parser.TryGetChildElementParser(localName, out elementParser))
        {
          if (localName != "Annotation")
            this.ReportUnexpectedElement(this.Location, this.reader.Name);
          if (isEmptyElement)
            return;
          int depth = this.reader.Depth;
          do
          {
            this.reader.Read();
          }
          while (this.reader.Depth > depth);
        }
        else
        {
          XmlElementInfo element = this.ReadElement(localName, this.Location);
          this.BeginElement(elementParser, element);
          if (!isEmptyElement)
            return;
          this.EndElement();
        }
      }
      else if (string.IsNullOrEmpty(namespaceUri) || this.IsOwnedNamespace(namespaceUri))
      {
        if (localName != "Annotation")
          this.ReportUnexpectedElement(this.Location, this.reader.Name);
        this.reader.Skip();
      }
      else
      {
        XmlReader xmlReader = this.reader.ReadSubtree();
        int content = (int) xmlReader.MoveToContent();
        string str = xmlReader.ReadOuterXml();
        this.currentScope.Element.AddAnnotation(new XmlAnnotationInfo(this.Location, namespaceUri, localName, str, false));
      }
    }

    private XmlElementInfo ReadElement(string elementName, CsdlLocation elementLocation)
    {
      List<XmlAttributeInfo> attributes = (List<XmlAttributeInfo>) null;
      List<XmlAnnotationInfo> annotations = (List<XmlAnnotationInfo>) null;
      for (bool flag = this.reader.MoveToFirstAttribute(); flag; flag = this.reader.MoveToNextAttribute())
      {
        string namespaceUri = this.reader.NamespaceURI;
        if (string.IsNullOrEmpty(namespaceUri) || namespaceUri.EqualsOrdinal(this.DocumentNamespace))
        {
          if (attributes == null)
            attributes = new List<XmlAttributeInfo>();
          attributes.Add(new XmlAttributeInfo(this.reader.LocalName, this.reader.Value, this.Location));
        }
        else if (this.IsOwnedNamespace(namespaceUri))
        {
          this.ReportUnexpectedAttribute(this.Location, this.reader.Name);
        }
        else
        {
          if (annotations == null)
            annotations = new List<XmlAnnotationInfo>();
          annotations.Add(new XmlAnnotationInfo(this.Location, this.reader.NamespaceURI, this.reader.LocalName, this.reader.Value, true));
        }
      }
      return new XmlElementInfo(elementName, elementLocation, (IList<XmlAttributeInfo>) attributes, annotations);
    }

    private void ReportEmptyFile() => this.ReportError(this.Location, EdmErrorCode.EmptyFile, this.DocumentPath == null ? Strings.XmlParser_EmptySchemaTextReader : Strings.XmlParser_EmptyFile((object) this.DocumentPath));

    private void ReportUnexpectedRootNamespace(
      string elementName,
      string namespaceUri,
      string[] expectedNamespaces)
    {
      string str = string.Join(", ", expectedNamespaces);
      this.ReportError(this.Location, EdmErrorCode.UnexpectedXmlElement, string.IsNullOrEmpty(namespaceUri) ? Strings.XmlParser_UnexpectedRootElementNoNamespace((object) str) : Strings.XmlParser_UnexpectedRootElementWrongNamespace((object) namespaceUri, (object) str));
    }

    private void ReportUnexpectedRootElement(
      CsdlLocation elementLocation,
      string elementName,
      string expectedNamespace)
    {
      this.ReportError(elementLocation, EdmErrorCode.UnexpectedXmlElement, Strings.XmlParser_UnexpectedRootElement((object) elementName, (object) "Schema"));
    }

    private void ReportUnexpectedAttribute(CsdlLocation errorLocation, string attributeName) => this.ReportError(errorLocation, EdmErrorCode.UnexpectedXmlAttribute, Strings.XmlParser_UnexpectedAttribute((object) attributeName));

    private void ReportUnexpectedNodeType(XmlNodeType nodeType) => this.ReportError(this.Location, EdmErrorCode.UnexpectedXmlNodeType, Strings.XmlParser_UnexpectedNodeType((object) nodeType));

    private void ReportUnexpectedElement(CsdlLocation errorLocation, string elementName)
    {
      if (!(elementName != "Annotation"))
        return;
      this.ReportError(errorLocation, EdmErrorCode.UnexpectedXmlElement, Strings.XmlParser_UnexpectedElement((object) elementName));
    }

    private void ReportUnusedElement(CsdlLocation errorLocation, string elementName) => this.ReportError(errorLocation, EdmErrorCode.UnexpectedXmlElement, Strings.XmlParser_UnusedElement((object) elementName));

    private void ReportTextNotAllowed(CsdlLocation errorLocation, string textValue) => this.ReportError(errorLocation, EdmErrorCode.TextNotAllowed, Strings.XmlParser_TextNotAllowed((object) textValue));

    private class ElementScope
    {
      private static readonly IList<XmlElementValue> EmptyValues = (IList<XmlElementValue>) new ReadOnlyCollection<XmlElementValue>((IList<XmlElementValue>) new XmlElementValue[0]);
      private List<XmlElementValue> childValues;

      internal ElementScope(XmlElementParser parser, XmlElementInfo element)
      {
        this.Parser = parser;
        this.Element = element;
      }

      internal XmlElementParser Parser { get; private set; }

      internal XmlElementInfo Element { get; private set; }

      internal IList<XmlElementValue> ChildValues => (IList<XmlElementValue>) this.childValues ?? XmlDocumentParser.ElementScope.EmptyValues;

      internal void AddChildValue(XmlElementValue value)
      {
        if (this.childValues == null)
          this.childValues = new List<XmlElementValue>();
        this.childValues.Add(value);
      }
    }
  }
}
