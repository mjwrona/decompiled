// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber
{
  internal sealed class SensitiveDataScrubber : ISensitiveDataScrubber
  {
    private static readonly RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;
    private readonly Regex allPatternsRegexes;
    private static readonly Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.SubstringMatch[] substringMatches = new Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.SubstringMatch[3]
    {
      new Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.SubstringMatch("publickeytoken=", "token="),
      new Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.SubstringMatch((string) null, "password="),
      new Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.SubstringMatch((string) null, "blob.core.windows.net", new Regex("((blob.core.windows.net)*(sv=))", Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.Options))
    };
    private static readonly List<string> additionalPatterns = new List<string>()
    {
      "([A-Z0-9_]|\\.|%2E){1,50}(@|_AT_|%40)([A-Z0-9_]{1,50}(\\.|_|%2E))+(COM|ORG|GOV|EDU)",
      "([A-Z]:[^\\<\\>\\:\"\\|\\?\\*]+Users)|(\\/Users/)"
    };

    public SensitiveDataScrubber() => this.allPatternsRegexes = new Regex(string.Join("|", (IEnumerable<string>) Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.additionalPatterns), Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.Options);

    public bool ContainsSensitiveData(string propertyValue, bool scrubAllTypesOfPersonalData)
    {
      if (string.IsNullOrWhiteSpace(propertyValue))
        return false;
      foreach (Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.SubstringMatch substringMatch in Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.substringMatches)
      {
        bool flag = Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.IsMatch(propertyValue, in substringMatch);
        if (flag && substringMatch.FalsePositive != null)
          flag = Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.IsMatch(Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.RemoveSubstring(propertyValue, substringMatch.FalsePositive), in substringMatch);
        if (flag)
          return true;
      }
      return scrubAllTypesOfPersonalData && this.allPatternsRegexes.IsMatch(propertyValue);
    }

    private static bool IsMatch(
      string value,
      in Microsoft.VisualStudio.Telemetry.SensitiveDataScrubber.SensitiveDataScrubber.SubstringMatch substringMatch)
    {
      string pattern = substringMatch.Pattern;
      if (pattern.Length > value.Length)
        return false;
      bool flag = value.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;
      if (flag && substringMatch.FallbackRegex != null)
        flag = substringMatch.FallbackRegex.IsMatch(value);
      return flag;
    }

    private static string RemoveSubstring(string value, string substring) => string.IsNullOrEmpty(value) || string.IsNullOrEmpty(substring) ? value : value.ToLowerInvariant().Replace(substring.ToLowerInvariant(), string.Empty);

    private readonly struct SubstringMatch
    {
      public readonly string FalsePositive;
      public readonly string Pattern;
      public readonly Regex FallbackRegex;

      public SubstringMatch(string falsePositive, string pattern, Regex fallbackRegEx = null)
      {
        this.FalsePositive = falsePositive;
        this.Pattern = pattern;
        this.FallbackRegex = fallbackRegEx;
      }
    }
  }
}
