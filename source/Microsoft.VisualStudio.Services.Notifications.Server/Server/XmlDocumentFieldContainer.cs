// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.XmlDocumentFieldContainer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class XmlDocumentFieldContainer : DocumentFieldContainer
  {
    private XmlDocument m_document;
    private string m_documentString;

    public XmlDocumentFieldContainer(string documentString) => this.m_documentString = documentString;

    public XmlDocumentFieldContainer(XmlDocument document) => this.m_document = document;

    public XmlDocumentFieldContainer(XmlDocumentFieldContainer other)
    {
      this.m_document = other.m_document;
      this.m_documentString = other.m_documentString;
    }

    public override IFieldContainer GetDynamicFieldContainer(DynamicFieldContainerType type) => type != DynamicFieldContainerType.Xml ? (IFieldContainer) null : (IFieldContainer) this;

    internal override object GetFieldValueInternal(string fieldName)
    {
      if (this.GetDocument() == null)
        return (object) null;
      object fieldValueInternal = (object) null;
      string localName = this.m_document.DocumentElement.LocalName;
      XPathNavigator navigator = this.m_document.CreateNavigator();
      navigator.MoveToChild(XPathNodeType.Element);
      object obj;
      if (fieldName.Trim().IndexOf("customExpression:", StringComparison.OrdinalIgnoreCase) != -1)
      {
        try
        {
          NotificationXsltContext notificationXsltContext = new NotificationXsltContext(new NameTable());
          XPathExpression expr = XPathExpression.Compile(fieldName, (IXmlNamespaceResolver) notificationXsltContext);
          expr.SetContext((XmlNamespaceManager) notificationXsltContext);
          obj = navigator.Evaluate(expr);
        }
        catch (Exception ex)
        {
          return (object) null;
        }
      }
      else
        obj = !ArgumentUtility.HasSurrogates(fieldName) ? navigator.Evaluate(fieldName, (IXmlNamespaceResolver) new TfsNamespaceResolver()) : navigator.Evaluate(XmlDocumentFieldContainer.ConvertSurrogatesToBytes(fieldName), (IXmlNamespaceResolver) new TfsNamespaceResolver());
      if (obj is XPathNodeIterator xpathNodeIterator)
      {
        if (xpathNodeIterator.Count == 1)
        {
          if (xpathNodeIterator.MoveNext())
            fieldValueInternal = (object) xpathNodeIterator.Current.Value;
        }
        else if (xpathNodeIterator.Count > 1)
        {
          string[] strArray = new string[xpathNodeIterator.Count];
          for (int index = 0; index < xpathNodeIterator.Count; ++index)
          {
            if (xpathNodeIterator.MoveNext())
              strArray[index] = xpathNodeIterator.Current.Value;
          }
          fieldValueInternal = (object) strArray;
        }
      }
      else
        fieldValueInternal = obj;
      return fieldValueInternal;
    }

    public static string ConvertSurrogatesToBytes(string value)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < value.Length; ++index)
      {
        if (index + 1 < value.Length && char.IsSurrogatePair(value[index], value[index + 1]))
        {
          int utf32 = char.ConvertToUtf32(value[index], value[index + 1]);
          stringBuilder.Append(string.Format("&#{0};", (object) utf32));
          ++index;
        }
        else
          stringBuilder.Append(value[index]);
      }
      return stringBuilder.ToString();
    }

    public XmlDocument GetDocument()
    {
      if (this.m_document == null)
      {
        if (this.m_documentString == null)
          return (XmlDocument) null;
        this.m_document = NotificationsSerialization.LoadXml(this.m_documentString);
      }
      return this.m_document;
    }

    public override string GetDocumentString()
    {
      if (this.m_documentString == null)
      {
        if (this.m_documentString != null || this.m_document == null)
          return (string) null;
        this.m_documentString = this.m_document.OuterXml;
      }
      return this.m_documentString;
    }

    private void AddNode(string nodeName, string nodeText)
    {
      XmlNode node = this.Document.CreateNode(XmlNodeType.Element, nodeName, (string) null);
      node.InnerText = nodeText;
      this.Document.DocumentElement.AppendChild(node);
    }

    public override void AddOrUpdateNode(string nodeName, string nodeText)
    {
      XmlNodeList elementsByTagName = this.Document.GetElementsByTagName(nodeName);
      if (elementsByTagName != null && elementsByTagName.Count > 0)
        elementsByTagName.Item(0).InnerText = nodeText;
      else
        this.AddNode(nodeName, nodeText);
      this.m_documentString = (string) null;
    }

    public XmlDocument Document => this.GetDocument();
  }
}
