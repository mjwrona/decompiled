// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.HtmlTextAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  internal static class HtmlTextAnalyzer
  {
    private static readonly Regex s_contiguousHtmlTagsRegex = new Regex("(<[^>]*>)+", RegexOptions.Compiled);
    private static readonly Regex s_contiguousNewLinesWithWhitespacesRegex = new Regex("([ ]*[\\r\\n]+[ ]*)+", RegexOptions.Compiled);
    private static readonly Regex s_contiguousNonNewLineWhitespacesRegex = new Regex("[ \\t]+", RegexOptions.Compiled);
    private static readonly Regex s_contiguousHtmlNewLineTagsRegex = new Regex("(<br/?>|</?div>|</?p>|</?ul>|</?ol>|</tr>|</li>)+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly List<string> s_listOfSpecialTags = new List<string>()
    {
      "style"
    };
    private static readonly List<Regex> s_regexObjectsForSpecialTags = HtmlTextAnalyzer.CreateRegexObjectsForSpecialTags(HtmlTextAnalyzer.s_listOfSpecialTags);

    public static string Clean(string htmlText)
    {
      htmlText = HtmlTextAnalyzer.CleanHtmlTextForSpecialTags(htmlText);
      htmlText = htmlText.Replace('\n', ' ');
      htmlText = HtmlTextAnalyzer.s_contiguousHtmlNewLineTagsRegex.Replace(htmlText, "\n");
      htmlText = htmlText.Replace("<li>", "&#8226;");
      htmlText = HtmlTextAnalyzer.s_contiguousHtmlTagsRegex.Replace(htmlText, " ");
      htmlText = WebUtility.HtmlDecode(htmlText);
      htmlText = htmlText.Replace(' ', ' ');
      htmlText = HtmlTextAnalyzer.s_contiguousNonNewLineWhitespacesRegex.Replace(htmlText, " ");
      htmlText = HtmlTextAnalyzer.s_contiguousNewLinesWithWhitespacesRegex.Replace(htmlText, "\n");
      return htmlText;
    }

    private static string GetCustomTagRegex(string tag) => "<" + tag + ">[^<>]*</" + tag + ">";

    private static string CleanHtmlTextForSpecialTags(string htmlText)
    {
      foreach (Regex objectsForSpecialTag in HtmlTextAnalyzer.s_regexObjectsForSpecialTags)
        htmlText = objectsForSpecialTag.Replace(htmlText, " ");
      return htmlText;
    }

    private static List<Regex> CreateRegexObjectsForSpecialTags(List<string> listofSpecialTags)
    {
      List<Regex> objectsForSpecialTags = new List<Regex>();
      foreach (string listofSpecialTag in listofSpecialTags)
        objectsForSpecialTags.Add(new Regex(HtmlTextAnalyzer.GetCustomTagRegex(listofSpecialTag), RegexOptions.IgnoreCase | RegexOptions.Compiled));
      return objectsForSpecialTags;
    }
  }
}
