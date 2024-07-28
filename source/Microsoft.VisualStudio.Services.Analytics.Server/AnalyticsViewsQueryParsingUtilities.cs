// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsQueryParsingUtilities
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsViewsQueryParsingUtilities
  {
    private static readonly Regex SelectedProperties = new Regex("^(.*&)?\\$select=(?<properties>[^&]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string[] GetSelectedProperties(string query)
    {
      Match match = AnalyticsViewsQueryParsingUtilities.SelectedProperties.Match(query);
      if (!match.Success)
        return Array.Empty<string>();
      return match.Groups["properties"].Value.Split(',');
    }

    public static string ReplaceSelectedProperties(string query, IEnumerable<string> properties) => AnalyticsViewsQueryParsingUtilities.SelectedProperties.Replace(query, (MatchEvaluator) (m => string.Format("{0}$select={1}", (object) m.Groups[1], (object) string.Join(",", properties))));

    public static bool IsPropertySelected(string query, string property) => Regex.IsMatch(query, "\\$select=([^&]+,)?" + property + "(,.*|&.*)?$");
  }
}
