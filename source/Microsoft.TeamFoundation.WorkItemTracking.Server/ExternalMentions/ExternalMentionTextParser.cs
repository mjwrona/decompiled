// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalMentions.ExternalMentionTextParser
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalMentions
{
  internal static class ExternalMentionTextParser
  {
    internal static readonly HashSet<string> WorkItemResolveVerbs = new HashSet<string>((IEnumerable<string>) new string[3]
    {
      "Fix",
      "Fixes",
      "Fixed"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly Regex WorkItemAbFormat = new Regex("(?:\\b|_)(?:([a-z]+?)[ :] ?)?(\\[?)AB#(\\d+)(?:\\b|_)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1.0));
    private static readonly Regex WorkItemNonLinkFormat = new Regex("(?(?<=_)|\\b)(AB#(\\d+))(?!([^\\]\\[]*?\\]\\(.*?\\)))(?=(\\b|_))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1.0));
    private static readonly string markdownMentionFormat = "[AB#{0}]({1})";

    internal static IDictionary<int, WorkItemMention> ParseWorkItemMentions(
      string message,
      bool allowResolveVerb,
      DateTime? resolvedDate = null)
    {
      Dictionary<int, WorkItemMention> workItemMentions = new Dictionary<int, WorkItemMention>();
      if (!string.IsNullOrEmpty(message))
      {
        MatchCollection matchCollection = ExternalMentionTextParser.WorkItemAbFormat.Matches(message);
        for (int i = 0; i < matchCollection.Count; ++i)
        {
          Match match = matchCollection[i];
          int result;
          if (match.Success && match.Groups.Count > 1 && int.TryParse(match.Groups[3].Value, out result) && result > 0)
          {
            string str = (match.Groups[1].Value ?? "").Trim();
            bool flag = allowResolveVerb && ExternalMentionTextParser.WorkItemResolveVerbs.Contains(str);
            WorkItemMention workItemMention1 = new WorkItemMention()
            {
              WorkItemId = result,
              ShouldResolve = flag,
              ResolvedDate = resolvedDate
            };
            WorkItemMention workItemMention2;
            if (workItemMentions.TryGetValue(result, out workItemMention2))
            {
              if (flag && !workItemMention2.ShouldResolve)
                workItemMentions[result] = workItemMention1;
            }
            else
              workItemMentions[result] = workItemMention1;
          }
        }
      }
      return (IDictionary<int, WorkItemMention>) workItemMentions;
    }

    internal static string ReplaceWorkItemsWithHyperlinks(
      string description,
      Dictionary<int, string> replacements)
    {
      StringBuilder stringBuilder = new StringBuilder(description);
      MatchCollection matchCollection = ExternalMentionTextParser.WorkItemNonLinkFormat.Matches(description);
      for (int i = matchCollection.Count - 1; i >= 0; --i)
      {
        Match match = matchCollection[i];
        int result;
        if (match.Success && int.TryParse(match.Groups[1].Value, out result) && replacements.ContainsKey(result))
          stringBuilder.Replace(match.Value, replacements[result], match.Index, match.Length);
      }
      return stringBuilder.ToString();
    }

    internal static string GetMarkdownMention(int workItemId, string link) => string.Format(ExternalMentionTextParser.markdownMentionFormat, (object) workItemId, (object) link);
  }
}
