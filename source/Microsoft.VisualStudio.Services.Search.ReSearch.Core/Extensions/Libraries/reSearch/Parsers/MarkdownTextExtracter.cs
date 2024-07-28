// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.MarkdownTextExtracter
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using HtmlAgilityPack;
using Markdig;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  public class MarkdownTextExtracter
  {
    private readonly string m_html;
    [StaticSafe("Grandfathered")]
    private static int s_urlLengthLimit = 8000;
    [StaticSafe("Grandfathered")]
    private static Func<HtmlNode, string> s_urlExtractor = (Func<HtmlNode, string>) (node =>
    {
      string name = MarkdownTextExtracter.s_htmlTagsToUrlAttribute[node.Name];
      return node.Attributes.Contains(name) ? node.Attributes[name].Value : "";
    });
    [StaticSafe("Grandfathered")]
    private static IDictionary<string, string> s_htmlTagsToUrlAttribute = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "a",
        "href"
      },
      {
        "img",
        "src"
      }
    };

    internal MarkdownTextExtracter(string markDownContent) => this.m_html = Markdown.ToHtml(markDownContent);

    internal string GetTextFromMarkdown() => MarkdownTextExtracter.ExtractTextFromHtml(this.m_html);

    internal IEnumerable<string> GetLinksFromMarkDown() => MarkdownTextExtracter.ExtractLinkFromHtml(this.m_html);

    private static IEnumerable<string> ExtractLinkFromHtml(string html)
    {
      HtmlDocument htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(html);
      return htmlDocument.DocumentNode.Descendants().Where<HtmlNode>((Func<HtmlNode, bool>) (n => n.NodeType == HtmlNodeType.Element)).Where<HtmlNode>((Func<HtmlNode, bool>) (n => MarkdownTextExtracter.s_htmlTagsToUrlAttribute.ContainsKey(n.Name))).Select<HtmlNode, string>((Func<HtmlNode, string>) (n => MarkdownTextExtracter.s_urlExtractor(n))).Where<string>((Func<string, bool>) (url => MarkdownTextExtracter.isValidUrl(url)));
    }

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
    public static string GetEncodedUrl(string url)
    {
      if (string.IsNullOrEmpty(url))
        return url;
      List<string> list = MarkdownTextExtracter.ExtractLinkFromHtml(Markdown.ToHtml(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[]({0})", (object) url))).ToList<string>();
      return list != null && list.Count<string>() == 1 ? list[0] : (string) null;
    }

    private static string ExtractTextFromHtml(string html)
    {
      HtmlDocument htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(html);
      string text = htmlDocument.DocumentNode.InnerText;
      if (!string.IsNullOrEmpty(text))
        text = HtmlEntity.DeEntitize(text).Trim('\r', '\n', ' ');
      return text;
    }

    private static bool isValidUrl(string url) => !string.IsNullOrEmpty(url) && !url.ToLowerInvariant().StartsWith("data:", StringComparison.Ordinal) && url.Length <= MarkdownTextExtracter.s_urlLengthLimit;
  }
}
