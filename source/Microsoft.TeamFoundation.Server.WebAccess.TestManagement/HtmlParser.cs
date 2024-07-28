// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.HtmlParser
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class HtmlParser
  {
    internal const string XhtmlNamespace = "http://www.w3.org/1999/xhtml";
    private HtmlLexicalAnalyzer _htmlLexicalAnalyzer;
    private XmlDocument _document;
    private Stack<XmlElement> _openedElements;
    private Stack<XmlElement> _pendingInlineElements;

    private HtmlParser(string inputString)
    {
      this._document = new XmlDocument();
      this._openedElements = new Stack<XmlElement>();
      this._pendingInlineElements = new Stack<XmlElement>();
      this._htmlLexicalAnalyzer = new HtmlLexicalAnalyzer(inputString);
      this._htmlLexicalAnalyzer.GetNextContentToken();
    }

    internal static XmlElement ParseHtml(string htmlString) => new HtmlParser(htmlString).ParseHtmlContent();

    private XmlElement ParseHtmlContent()
    {
      XmlElement htmlElement = this._document.CreateElement("html", "http://www.w3.org/1999/xhtml");
      this.OpenStructuringElement(htmlElement);
      while (this._htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF)
      {
        if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.OpeningTagStart)
        {
          this._htmlLexicalAnalyzer.GetNextTagToken();
          if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
          {
            string lowerInvariant = this._htmlLexicalAnalyzer.NextToken.ToLowerInvariant();
            this._htmlLexicalAnalyzer.GetNextTagToken();
            XmlElement element = this._document.CreateElement(lowerInvariant, "http://www.w3.org/1999/xhtml");
            this.ParseAttributes(element);
            if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.EmptyTagEnd || HtmlSchema.IsEmptyElement(lowerInvariant))
              this.AddEmptyElement(element);
            else if (HtmlSchema.IsInlineElement(lowerInvariant))
              this.OpenInlineElement(element);
            else if (HtmlSchema.IsBlockElement(lowerInvariant) || HtmlSchema.IsKnownOpenableElement(lowerInvariant))
              this.OpenStructuringElement(element);
          }
        }
        else if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.ClosingTagStart)
        {
          this._htmlLexicalAnalyzer.GetNextTagToken();
          if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
          {
            string lowerInvariant = this._htmlLexicalAnalyzer.NextToken.ToLowerInvariant();
            this._htmlLexicalAnalyzer.GetNextTagToken();
            this.CloseElement(lowerInvariant);
          }
        }
        else if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Text)
          this.AddTextContent(this._htmlLexicalAnalyzer.NextToken);
        else if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Comment)
          this.AddComment(this._htmlLexicalAnalyzer.NextToken);
        this._htmlLexicalAnalyzer.GetNextContentToken();
      }
      if (htmlElement.FirstChild is XmlElement && htmlElement.FirstChild == htmlElement.LastChild && string.Equals(htmlElement.FirstChild.LocalName, "html", StringComparison.OrdinalIgnoreCase))
        htmlElement = (XmlElement) htmlElement.FirstChild;
      return htmlElement;
    }

    private XmlElement CreateElementCopy(XmlElement htmlElement)
    {
      XmlElement element = this._document.CreateElement(htmlElement.LocalName, "http://www.w3.org/1999/xhtml");
      for (int i = 0; i < htmlElement.Attributes.Count; ++i)
      {
        XmlAttribute attribute = htmlElement.Attributes[i];
        element.SetAttribute(attribute.Name, attribute.Value);
      }
      return element;
    }

    private void AddEmptyElement(XmlElement htmlEmptyElement) => this._openedElements.Peek().AppendChild((XmlNode) htmlEmptyElement);

    private void OpenInlineElement(XmlElement htmlInlineElement) => this._pendingInlineElements.Push(htmlInlineElement);

    private void OpenStructuringElement(XmlElement htmlElement)
    {
      if (HtmlSchema.IsBlockElement(htmlElement.LocalName))
      {
        while (this._openedElements.Count > 0 && HtmlSchema.IsInlineElement(this._openedElements.Peek().LocalName))
          this._pendingInlineElements.Push(this.CreateElementCopy(this._openedElements.Pop()));
      }
      if (this._openedElements.Count > 0)
      {
        XmlElement xmlElement = this._openedElements.Peek();
        if (HtmlSchema.ClosesOnNextElementStart(xmlElement.LocalName, htmlElement.LocalName))
        {
          this._openedElements.Pop();
          xmlElement = this._openedElements.Count > 0 ? this._openedElements.Peek() : (XmlElement) null;
        }
        xmlElement?.AppendChild((XmlNode) htmlElement);
      }
      this._openedElements.Push(htmlElement);
    }

    private bool IsElementOpened(string htmlElementName)
    {
      foreach (XmlNode openedElement in this._openedElements)
      {
        if (string.Equals(openedElement.LocalName, htmlElementName, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private void CloseElement(string htmlElementName)
    {
      if (this._pendingInlineElements.Count > 0 && string.Equals(this._pendingInlineElements.Peek().LocalName, htmlElementName, StringComparison.OrdinalIgnoreCase))
      {
        XmlElement newChild = this._pendingInlineElements.Pop();
        this._openedElements.Peek().AppendChild((XmlNode) newChild);
      }
      else
      {
        if (!this.IsElementOpened(htmlElementName))
          return;
        while (this._openedElements.Count > 1)
        {
          XmlElement htmlElement = this._openedElements.Pop();
          if (string.Equals(htmlElement.LocalName, htmlElementName, StringComparison.OrdinalIgnoreCase))
            break;
          if (HtmlSchema.IsInlineElement(htmlElement.LocalName))
            this._pendingInlineElements.Push(this.CreateElementCopy(htmlElement));
        }
      }
    }

    private void AddTextContent(string textContent)
    {
      this.OpenPendingInlineElements();
      this._openedElements.Peek().AppendChild((XmlNode) this._document.CreateTextNode(textContent));
    }

    private void AddComment(string comment)
    {
      this.OpenPendingInlineElements();
      this._openedElements.Peek().AppendChild((XmlNode) this._document.CreateComment(comment));
    }

    private void OpenPendingInlineElements()
    {
      if (this._pendingInlineElements.Count <= 0)
        return;
      XmlElement newChild = this._pendingInlineElements.Pop();
      this.OpenPendingInlineElements();
      this._openedElements.Peek().AppendChild((XmlNode) newChild);
      this._openedElements.Push(newChild);
    }

    private void ParseAttributes(XmlElement xmlElement)
    {
      while (this._htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF && this._htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.TagEnd && this._htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EmptyTagEnd)
      {
        if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
        {
          string nextToken1 = this._htmlLexicalAnalyzer.NextToken;
          this._htmlLexicalAnalyzer.GetNextEqualSignToken();
          this._htmlLexicalAnalyzer.GetNextAtomToken();
          string nextToken2 = this._htmlLexicalAnalyzer.NextToken;
          xmlElement.SetAttribute(nextToken1, nextToken2);
        }
        this._htmlLexicalAnalyzer.GetNextTagToken();
      }
    }
  }
}
