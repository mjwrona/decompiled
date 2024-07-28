// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.MarkdownTextExtracterLegacy
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using HtmlAgilityPack;
using Markdig;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  public class MarkdownTextExtracterLegacy
  {
    private readonly string m_html;

    internal MarkdownTextExtracterLegacy(string markDownContent) => this.m_html = Markdown.ToHtml(markDownContent);

    internal string GetTextFromMarkdown() => MarkdownTextExtracterLegacy.ExtractTextFromHtml(this.m_html);

    internal List<string> GetLinksFromMarkDown() => MarkdownTextExtracterLegacy.ExtractLinkFromHtml(this.m_html);

    private static List<string> ExtractLinkFromHtml(string html)
    {
      List<string> linkFromHtml = new List<string>();
      HtmlDocument htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(html);
      Stack<HtmlNode> htmlNodeStack = new Stack<HtmlNode>();
      htmlNodeStack.Push(htmlDocument.DocumentNode);
      while (htmlNodeStack.Count > 0)
      {
        HtmlNode htmlNode = htmlNodeStack.Pop();
        foreach (HtmlNode childNode in (IEnumerable<HtmlNode>) htmlNode.ChildNodes)
          htmlNodeStack.Push(childNode);
        if (htmlNode.NodeType == HtmlNodeType.Element)
        {
          HtmlAttribute htmlAttribute = (HtmlAttribute) null;
          switch (htmlNode.Name)
          {
            case "a":
              htmlAttribute = htmlNode.Attributes["href"];
              break;
            case "img":
              htmlAttribute = htmlNode.Attributes["src"];
              break;
          }
          if (htmlAttribute != null)
            linkFromHtml.Add(htmlAttribute.Value);
        }
      }
      return linkFromHtml;
    }

    private static string ExtractTextFromHtml(string html)
    {
      HtmlDocument htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(html);
      StringBuilder innerText = new StringBuilder();
      MarkdownTextExtracterLegacy.GetHtmlInnerText(htmlDocument.DocumentNode, innerText);
      return innerText.ToString().Trim('\r', '\n', ' ');
    }

    private static void GetNodeInnerText(HtmlNode node, StringBuilder innerText)
    {
      foreach (HtmlNode childNode in (IEnumerable<HtmlNode>) node.ChildNodes)
        MarkdownTextExtracterLegacy.GetHtmlInnerText(childNode, innerText);
    }

    private static void GetHtmlInnerText(HtmlNode node, StringBuilder innerText)
    {
      switch (node.NodeType)
      {
        case HtmlNodeType.Document:
          MarkdownTextExtracterLegacy.GetNodeInnerText(node, innerText);
          break;
        case HtmlNodeType.Element:
          if (node.Name == "p")
          {
            if (node.HasChildNodes)
              MarkdownTextExtracterLegacy.GetNodeInnerText(node, innerText);
            innerText.Append("\r\n");
            break;
          }
          if (!node.HasChildNodes)
            break;
          MarkdownTextExtracterLegacy.GetNodeInnerText(node, innerText);
          break;
        case HtmlNodeType.Text:
          switch (node.ParentNode.Name)
          {
            case "script":
              return;
            case "style":
              return;
            default:
              string text = ((HtmlTextNode) node).Text;
              if (HtmlNode.IsOverlappedClosingElement(text) || text.Trim().Length <= 0)
                return;
              innerText.Append(HtmlEntity.DeEntitize(text));
              return;
          }
      }
    }
  }
}
