// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.HtmlToMarkdownConverter
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using HtmlAgilityPack;
using System.Web;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  internal class HtmlToMarkdownConverter : HtmlConverterBase
  {
    protected override string ConvertListItem(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      string str = string.Empty;
      if (indentLevel > 1)
        str = new string(' ', 4 * (indentLevel - 1));
      return string.Format("\r\n{2}{0} {1}", node.ParentNode.Name == "ol" ? (object) "1." : (object) "+", (object) innerContent, (object) str);
    }

    protected override string ConvertStrong(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.IsNullOrWhiteSpace(innerContent) ? innerContent : string.Format("**{0}**", (object) innerContent.Replace("\r\n", "**\r\n**"));
    }

    protected override string ConvertItalic(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.IsNullOrWhiteSpace(innerContent) ? innerContent : string.Format("_{0}_", (object) innerContent.Replace("\r\n", "_\r\n_"));
    }

    protected override string ConvertLink(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("[{1}]({0})", node.Attributes.Contains("href") ? (object) HttpUtility.HtmlDecode(node.Attributes["href"].Value) : (object) string.Empty, (object) innerContent);
    }

    protected override string ConvertImage(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("![]({0})", node.Attributes.Contains("src") ? (object) node.Attributes["src"].Value : (object) string.Empty);
    }

    protected override string ConvertCodeBlock(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("```\r\n{0}\r\n```", (object) innerContent);
    }

    protected override string ConvertHeader1(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n\r\n# {0}\r\n\r\n", (object) innerContent);
    }

    protected override string ConvertHeader2(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n\r\n## {0}\r\n\r\n", (object) innerContent);
    }

    protected override string ConvertHeader3(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n\r\n### {0}\r\n\r\n", (object) innerContent);
    }

    protected override string ConvertHeader4(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n\r\n#### {0}\r\n\r\n", (object) innerContent);
    }

    protected override string ConvertHeader5(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n\r\n##### {0}\r\n\r\n", (object) innerContent);
    }

    protected override string ConvertHeader6(
      HtmlNode node,
      string innerContent,
      int indentLevel = 0,
      int orderNumber = 0)
    {
      return string.Format("\r\n\r\n###### {0}\r\n\r\n", (object) innerContent);
    }
  }
}
