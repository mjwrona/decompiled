// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.EdmXmlDocumentParser`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal abstract class EdmXmlDocumentParser<TResult> : XmlDocumentParser<TResult>
  {
    protected XmlElementInfo currentElement;
    private readonly Stack<XmlElementInfo> elementStack = new Stack<XmlElementInfo>();
    private HashSetInternal<string> edmNamespaces;

    internal EdmXmlDocumentParser(string artifactLocation, XmlReader reader)
      : base(reader, artifactLocation)
    {
    }

    internal abstract IEnumerable<KeyValuePair<Version, string>> SupportedVersions { get; }

    internal static XmlAttributeInfo GetOptionalAttribute(
      XmlElementInfo element,
      string attributeName)
    {
      return element.Attributes[attributeName];
    }

    internal XmlAttributeInfo GetRequiredAttribute(XmlElementInfo element, string attributeName)
    {
      XmlAttributeInfo attribute = element.Attributes[attributeName];
      if (!attribute.IsMissing)
        return attribute;
      this.ReportError(element.Location, EdmErrorCode.MissingAttribute, Strings.XmlParser_MissingAttribute((object) attributeName, (object) element.Name));
      return attribute;
    }

    protected override XmlReader InitializeReader(XmlReader reader)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        CheckCharacters = true,
        CloseInput = false,
        IgnoreWhitespace = true,
        ConformanceLevel = ConformanceLevel.Auto,
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,
        DtdProcessing = DtdProcessing.Prohibit
      };
      return XmlReader.Create(reader, settings);
    }

    protected override bool TryGetDocumentVersion(
      string xmlNamespaceName,
      out Version version,
      out string[] expectedNamespaces)
    {
      expectedNamespaces = this.SupportedVersions.Select<KeyValuePair<Version, string>, string>((Func<KeyValuePair<Version, string>, string>) (v => v.Value)).ToArray<string>();
      version = this.SupportedVersions.Where<KeyValuePair<Version, string>>((Func<KeyValuePair<Version, string>, bool>) (v => v.Value == xmlNamespaceName)).Select<KeyValuePair<Version, string>, Version>((Func<KeyValuePair<Version, string>, Version>) (v => v.Key)).FirstOrDefault<Version>();
      return version != (Version) null;
    }

    protected override bool IsOwnedNamespace(string namespaceName) => this.IsEdmNamespace(namespaceName);

    protected XmlElementParser<TItem> CsdlElement<TItem>(
      string elementName,
      Func<XmlElementInfo, XmlElementValueCollection, TItem> initializer,
      params XmlElementParser[] childParsers)
      where TItem : class
    {
      return this.Element<TItem>(elementName, (Func<XmlElementInfo, XmlElementValueCollection, TItem>) ((element, childValues) =>
      {
        this.BeginItem(element);
        TItem result = initializer(element, childValues);
        this.AnnotateItem((object) result, childValues);
        this.EndItem();
        return result;
      }), childParsers);
    }

    protected void BeginItem(XmlElementInfo element)
    {
      this.elementStack.Push(element);
      this.currentElement = element;
    }

    protected abstract void AnnotateItem(object result, XmlElementValueCollection childValues);

    protected void EndItem()
    {
      this.elementStack.Pop();
      this.currentElement = this.elementStack.Count == 0 ? (XmlElementInfo) null : this.elementStack.Peek();
    }

    protected int? OptionalInteger(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      if (optionalAttribute.IsMissing)
        return new int?();
      int? result;
      if (!EdmValueParser.TryParseInt(optionalAttribute.Value, out result))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidInteger, Strings.ValueParser_InvalidInteger((object) optionalAttribute.Value));
      return result;
    }

    protected long? OptionalLong(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      if (optionalAttribute.IsMissing)
        return new long?();
      long? result;
      if (!EdmValueParser.TryParseLong(optionalAttribute.Value, out result))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidLong, Strings.ValueParser_InvalidLong((object) optionalAttribute.Value));
      return result;
    }

    protected int? OptionalSrid(string attributeName, int defaultSrid)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      if (optionalAttribute.IsMissing)
        return new int?(defaultSrid);
      int? result;
      if (optionalAttribute.Value.EqualsOrdinalIgnoreCase("Variable"))
        result = new int?();
      else if (!EdmValueParser.TryParseInt(optionalAttribute.Value, out result))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidSrid, Strings.ValueParser_InvalidSrid((object) optionalAttribute.Value));
      return result;
    }

    protected int? OptionalScale(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      if (optionalAttribute.IsMissing)
        return new int?(0);
      int? result;
      if (optionalAttribute.Value.EqualsOrdinalIgnoreCase("Variable"))
        result = new int?();
      else if (!EdmValueParser.TryParseInt(optionalAttribute.Value, out result))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidSrid, Strings.ValueParser_InvalidScale((object) optionalAttribute.Value));
      return result;
    }

    protected int? OptionalMaxLength(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      if (optionalAttribute.IsMissing)
        return new int?();
      int? result;
      if (!EdmValueParser.TryParseInt(optionalAttribute.Value, out result))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidMaxLength, Strings.ValueParser_InvalidMaxLength((object) optionalAttribute.Value));
      return result;
    }

    protected EdmMultiplicity RequiredMultiplicity(string attributeName)
    {
      XmlAttributeInfo requiredAttribute = this.GetRequiredAttribute(this.currentElement, attributeName);
      if (!requiredAttribute.IsMissing)
      {
        switch (requiredAttribute.Value)
        {
          case "1":
            return EdmMultiplicity.One;
          case "0..1":
            return EdmMultiplicity.ZeroOrOne;
          case "*":
            return EdmMultiplicity.Many;
          default:
            this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidMultiplicity, Strings.CsdlParser_InvalidMultiplicity((object) requiredAttribute.Value));
            break;
        }
      }
      return EdmMultiplicity.One;
    }

    protected EdmOnDeleteAction RequiredOnDeleteAction(string attributeName)
    {
      XmlAttributeInfo requiredAttribute = this.GetRequiredAttribute(this.currentElement, attributeName);
      if (!requiredAttribute.IsMissing)
      {
        switch (requiredAttribute.Value)
        {
          case "None":
            return EdmOnDeleteAction.None;
          case "Cascade":
            return EdmOnDeleteAction.Cascade;
          default:
            this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidOnDelete, Strings.CsdlParser_InvalidDeleteAction((object) requiredAttribute.Value));
            break;
        }
      }
      return EdmOnDeleteAction.None;
    }

    protected bool? OptionalBoolean(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      if (optionalAttribute.IsMissing)
        return new bool?();
      bool? result;
      if (!EdmValueParser.TryParseBool(optionalAttribute.Value, out result))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidBoolean, Strings.ValueParser_InvalidBoolean((object) optionalAttribute.Value));
      return result;
    }

    protected string Optional(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      return optionalAttribute.IsMissing ? (string) null : optionalAttribute.Value;
    }

    protected string Required(string attributeName)
    {
      XmlAttributeInfo requiredAttribute = this.GetRequiredAttribute(this.currentElement, attributeName);
      return requiredAttribute.IsMissing ? string.Empty : requiredAttribute.Value;
    }

    protected string OptionalAlias(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      return !optionalAttribute.IsMissing ? this.ValidateAlias(optionalAttribute.Value) : (string) null;
    }

    protected string RequiredAlias(string attributeName)
    {
      XmlAttributeInfo requiredAttribute = this.GetRequiredAttribute(this.currentElement, attributeName);
      return !requiredAttribute.IsMissing ? this.ValidateAlias(requiredAttribute.Value) : (string) null;
    }

    protected string RequiredEntitySetPath(string attributeName)
    {
      XmlAttributeInfo requiredAttribute = this.GetRequiredAttribute(this.currentElement, attributeName);
      return !requiredAttribute.IsMissing ? this.ValidateEntitySetPath(requiredAttribute.Value) : (string) null;
    }

    protected string RequiredEnumMemberPath(string attributeName)
    {
      XmlAttributeInfo requiredAttribute = this.GetRequiredAttribute(this.currentElement, attributeName);
      return !requiredAttribute.IsMissing ? this.ValidateEnumMemberPath(requiredAttribute.Value) : (string) null;
    }

    protected string RequiredEnumMemberPath(XmlTextValue text) => this.ValidateEnumMembersPath(text != null ? text.TextValue : string.Empty);

    protected string OptionalType(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      return !optionalAttribute.IsMissing ? this.ValidateTypeName(optionalAttribute.Value) : (string) null;
    }

    protected string RequiredType(string attributeName)
    {
      XmlAttributeInfo requiredAttribute = this.GetRequiredAttribute(this.currentElement, attributeName);
      return !requiredAttribute.IsMissing ? this.ValidateTypeName(requiredAttribute.Value) : (string) null;
    }

    protected string OptionalQualifiedName(string attributeName)
    {
      XmlAttributeInfo optionalAttribute = EdmXmlDocumentParser<TResult>.GetOptionalAttribute(this.currentElement, attributeName);
      return !optionalAttribute.IsMissing ? this.ValidateQualifiedName(optionalAttribute.Value) : (string) null;
    }

    protected string RequiredQualifiedName(string attributeName)
    {
      XmlAttributeInfo requiredAttribute = this.GetRequiredAttribute(this.currentElement, attributeName);
      return !requiredAttribute.IsMissing ? this.ValidateQualifiedName(requiredAttribute.Value) : (string) null;
    }

    protected string ValidateEnumMembersPath(string path)
    {
      if (string.IsNullOrEmpty(path.Trim()))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEnumMemberPath, Strings.CsdlParser_InvalidEnumMemberPath((object) path));
      string[] array = ((IEnumerable<string>) path.Split(' ')).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))).ToArray<string>();
      string str1 = (string) null;
      foreach (string str2 in array)
      {
        char[] chArray = new char[1]{ '/' };
        string[] source = str2.Split(chArray);
        if (((IEnumerable<string>) source).Count<string>() != 2 || !EdmUtil.IsValidDottedName(source[0]) || !EdmUtil.IsValidUndottedName(source[1]))
          this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEnumMemberPath, Strings.CsdlParser_InvalidEnumMemberPath((object) path));
        if (str1 != null && source[0] != str1)
          this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEnumMemberPath, Strings.CsdlParser_InvalidEnumMemberPath((object) path));
        str1 = source[0];
      }
      return string.Join(" ", array);
    }

    private string ValidateTypeName(string name)
    {
      string[] source = name.Split('(', ')');
      string str = source[0];
      switch (str)
      {
        case "Collection":
          if (((IEnumerable<string>) source).Count<string>() == 1)
            return name;
          str = source[1];
          break;
        case "Ref":
          if (((IEnumerable<string>) source).Count<string>() == 1)
          {
            this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Strings.CsdlParser_InvalidTypeName((object) name));
            return name;
          }
          str = source[1];
          break;
      }
      if (EdmUtil.IsQualifiedName(str) || EdmCoreModel.Instance.GetPrimitiveTypeKind(str) != EdmPrimitiveTypeKind.None)
        return name;
      this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Strings.CsdlParser_InvalidTypeName((object) name));
      return name;
    }

    private string ValidateAlias(string name)
    {
      if (!EdmUtil.IsValidUndottedName(name))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidQualifiedName, Strings.CsdlParser_InvalidAlias((object) name));
      return name;
    }

    private string ValidateEntitySetPath(string path)
    {
      string[] source = path.Split('/');
      if (((IEnumerable<string>) source).Count<string>() != 2 || !EdmUtil.IsValidDottedName(source[0]) || !EdmUtil.IsValidUndottedName(source[1]))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEntitySetPath, Strings.CsdlParser_InvalidEntitySetPath((object) path));
      return path;
    }

    private string ValidateEnumMemberPath(string path)
    {
      string[] source = path.Split('/');
      if (((IEnumerable<string>) source).Count<string>() != 2 || !EdmUtil.IsValidDottedName(source[0]) || !EdmUtil.IsValidUndottedName(source[1]))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEnumMemberPath, Strings.CsdlParser_InvalidEnumMemberPath((object) path));
      return path;
    }

    private string ValidateQualifiedName(string qualifiedName)
    {
      if (!EdmUtil.IsQualifiedName(qualifiedName))
        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidQualifiedName, Strings.CsdlParser_InvalidQualifiedName((object) qualifiedName));
      return qualifiedName;
    }

    private bool IsEdmNamespace(string xmlNamespaceUri)
    {
      if (this.edmNamespaces == null)
      {
        this.edmNamespaces = new HashSetInternal<string>();
        foreach (string[] strArray in CsdlConstants.SupportedVersions.Values)
        {
          foreach (string thingToAdd in strArray)
            this.edmNamespaces.Add(thingToAdd);
        }
      }
      return this.edmNamespaces.Contains(xmlNamespaceUri);
    }
  }
}
