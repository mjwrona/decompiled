// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.MarkDownTextParser
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  internal class MarkDownTextParser : IParser
  {
    private const string TemplateSyntax = "template";
    private bool m_includeTemplatePagePathLinks;
    private TextEncoding m_textEncoding;

    public MarkDownTextParser(bool includeTemplatePagePathLinks)
      : this(includeTemplatePagePathLinks, new TextEncoding())
    {
    }

    public MarkDownTextParser(bool includeTemplatePagePathLinks, TextEncoding textEncoding)
    {
      this.m_textEncoding = textEncoding;
      this.m_includeTemplatePagePathLinks = includeTemplatePagePathLinks;
    }

    public MarkDownParsedContent ParseContent(string contents)
    {
      contents = !string.IsNullOrWhiteSpace(contents) ? contents : string.Empty;
      MarkdownTextExtracter markdownTextExtracter = new MarkdownTextExtracter(contents);
      return new MarkDownParsedContent()
      {
        Content = markdownTextExtracter.GetTextFromMarkdown(),
        Links = this.AppendTemplatePathsIfRequired(contents, markdownTextExtracter.GetLinksFromMarkDown()).ToList<string>()
      };
    }

    public MarkDownParsedContent ParseContentLegacy(string contents)
    {
      MarkdownTextExtracterLegacy textExtracterLegacy = new MarkdownTextExtracterLegacy(contents);
      return new MarkDownParsedContent()
      {
        Content = textExtracterLegacy.GetTextFromMarkdown(),
        Links = this.AppendTemplatePathsIfRequired(contents, (IEnumerable<string>) textExtracterLegacy.GetLinksFromMarkDown()).ToList<string>()
      };
    }

    public IParsedContent Parse(byte[] contents) => (IParsedContent) this.ParseContent(this.m_textEncoding.GetString(contents));

    private IEnumerable<string> AppendTemplatePathsIfRequired(
      string content,
      IEnumerable<string> markdownLinks)
    {
      return this.m_includeTemplatePagePathLinks ? markdownLinks.Concat<string>(MarkDownTextParser.ExtractTemplateLinksFromText(content)) : markdownLinks;
    }

    private static IEnumerable<string> ExtractTemplateLinksFromText(string text) => new Regex(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "^\\s*:::\\s*{0}\\s+([^\\s]+)\\s*$", (object) "template"), RegexOptions.Multiline).Matches(text).Cast<Match>().Select<Match, string>((Func<Match, string>) (x => MarkdownTextExtracter.GetEncodedUrl(x.Groups[1].Value)));
  }
}
