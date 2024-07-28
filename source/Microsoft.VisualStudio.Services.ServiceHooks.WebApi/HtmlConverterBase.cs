// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.HtmlConverterBase
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  internal class HtmlConverterBase
  {
    private static readonly Regex brokenCharacterMatcher = new Regex("\\&\\#\\#([0-9]+)\\;", RegexOptions.Compiled);
    private readonly Dictionary<string, HtmlConverterBase.HtmlElementConverter> m_htmlElementConverters;

    public HtmlConverterBase() => this.m_htmlElementConverters = new Dictionary<string, HtmlConverterBase.HtmlElementConverter>()
    {
      {
        "a",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertLink)
      },
      {
        "img",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertImage)
      },
      {
        "br",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertNewLine)
      },
      {
        "p",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertNewBlock)
      },
      {
        "div",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertNewBlock)
      },
      {
        "em",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertItalic)
      },
      {
        "i",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertItalic)
      },
      {
        "b",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertStrong)
      },
      {
        "strong",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertStrong)
      },
      {
        "ol",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertNewBlock)
      },
      {
        "ul",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertNewBlock)
      },
      {
        "li",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertListItem)
      },
      {
        "code",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertCodeBlock)
      },
      {
        "pre",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertCodeBlock)
      },
      {
        "blockquote",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertCodeBlock)
      },
      {
        "h1",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertHeader1)
      },
      {
        "h2",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertHeader2)
      },
      {
        "h3",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertHeader3)
      },
      {
        "h4",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertHeader4)
      },
      {
        "h5",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertHeader5)
      },
      {
        "h6",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertHeader6)
      },
      {
        "span",
        new HtmlConverterBase.HtmlElementConverter(this.ConvertStyledSpan)
      }
    };

    public string ConvertHtmlString(string html)
    {
      HtmlDocument htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(html);
      return this.ConvertNode(htmlDocument.DocumentNode);
    }

    public string ConvertHtmlDocument(HtmlDocument doc) => this.ConvertNode(doc.DocumentNode);

    private string ConvertNode(HtmlNode node, int indentLevel = 0, int orderNumber = 0)
    {
      if (node.NodeType == HtmlNodeType.Document)
        return this.GetNodeChildrenContent(node);
      if (node.NodeType == HtmlNodeType.Text)
        return this.GetTextFromTextNode((HtmlTextNode) node);
      if (node.NodeType != HtmlNodeType.Element)
        return string.Empty;
      string innerContent = node.HasChildNodes ? this.GetNodeChildrenContent(node, indentLevel) : string.Empty;
      return this.m_htmlElementConverters.ContainsKey(node.Name) ? this.m_htmlElementConverters[node.Name](node, innerContent, indentLevel, orderNumber) : innerContent;
    }

    private string GetTextFromTextNode(HtmlTextNode textNode)
    {
      string name = textNode.ParentNode.Name;
      if (name == "script" || name == "style")
        return string.Empty;
      string text = textNode.Text;
      return HtmlNode.IsOverlappedClosingElement(text) || text.Trim().Length == 0 ? string.Empty : HtmlConverterBase.UnbreakCharacterEncoding(HtmlEntity.DeEntitize(text));
    }

    private static string UnbreakCharacterEncoding(string input) => HtmlConverterBase.brokenCharacterMatcher.Replace(input, (MatchEvaluator) (match => "&#" + match.Groups[1].Value + ";"));

    private string GetNodeChildrenContent(HtmlNode node, int indentLevel = 0, int orderNumber = 0)
    {
      if (node.Name == "ul" || node.Name == "ol")
      {
        ++indentLevel;
        orderNumber = 0;
      }
      StringBuilder stringBuilder = new StringBuilder();
      foreach (HtmlNode childNode in (IEnumerable<HtmlNode>) node.ChildNodes)
      {
        if (childNode.Name == "li")
          ++orderNumber;
        stringBuilder.Append(this.ConvertNode(childNode, indentLevel, orderNumber));
      }
      return stringBuilder.ToString();
    }

    protected virtual string ConvertListItem(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    protected virtual string ConvertStrong(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return innerContent;
    }

    protected virtual string ConvertItalic(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return innerContent;
    }

    protected virtual string ConvertStyledSpan(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      if (string.IsNullOrWhiteSpace(innerContent) || !node.Attributes.Contains("style"))
        return innerContent;
      string[] source = node.Attributes["style"].Value.Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (source.Length == 0)
        return innerContent;
      string innerContent1 = innerContent;
      string str1 = ((IEnumerable<string>) source).FirstOrDefault<string>((Func<string, bool>) (x => x.Trim().StartsWith("font-weight", StringComparison.InvariantCultureIgnoreCase)));
      if (str1 != null)
      {
        if (str1.Split(':')[1].Trim() == "bold")
          innerContent1 = this.ConvertStrong(node, innerContent1);
      }
      string str2 = ((IEnumerable<string>) source).FirstOrDefault<string>((Func<string, bool>) (x => x.Trim().StartsWith("font-style", StringComparison.InvariantCultureIgnoreCase)));
      if (str2 != null)
      {
        if (str2.Split(':')[1].Trim() == "italic")
          innerContent1 = this.ConvertItalic(node, innerContent1);
      }
      return innerContent1;
    }

    protected virtual string ConvertLink(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return innerContent;
    }

    protected virtual string ConvertImage(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return innerContent;
    }

    protected virtual string ConvertNewLine(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return "\r\n";
    }

    protected virtual string ConvertNewBlock(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    protected virtual string ConvertCodeBlock(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    protected virtual string ConvertQuoteBlock(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n\r\n>{0}\r\n\r\n", (object) innerContent);
    }

    protected virtual string ConvertHeader1(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    protected virtual string ConvertHeader2(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    protected virtual string ConvertHeader3(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    protected virtual string ConvertHeader4(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    protected virtual string ConvertHeader5(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    protected virtual string ConvertHeader6(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n{0}\r\n", (object) innerContent);
    }

    private delegate string HtmlElementConverter(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0);
  }
}
