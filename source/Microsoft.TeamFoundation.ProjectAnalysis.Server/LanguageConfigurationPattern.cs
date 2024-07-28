// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.LanguageConfigurationPattern
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal class LanguageConfigurationPattern
  {
    private StringComparison m_comparison;
    private bool m_matchFolder;
    private bool m_isWildcard;
    private bool m_startsWithWildcard;
    private bool m_endsWithWildcard;
    private string m_textualPattern;
    private string m_pattern;
    private const string c_layer = "LanguageConfigurationPattern";

    public LanguageConfigurationPattern(string pattern, bool caseSensitivity = false)
    {
      ArgumentUtility.CheckForNull<string>(pattern, nameof (pattern));
      this.m_comparison = caseSensitivity ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
      this.m_pattern = pattern.Trim();
      this.m_matchFolder = this.m_pattern.EndsWith("/");
      this.m_isWildcard = this.m_pattern.Equals("*");
      this.m_startsWithWildcard = !this.m_isWildcard && this.m_pattern.StartsWith("*");
      this.m_textualPattern = this.m_startsWithWildcard ? this.m_pattern.Substring(1) : this.m_pattern;
      this.m_textualPattern = this.m_matchFolder ? this.m_textualPattern.Substring(0, this.m_textualPattern.Length - 1) : this.m_textualPattern;
      this.m_endsWithWildcard = this.m_textualPattern.EndsWith("*");
      this.m_textualPattern = this.m_endsWithWildcard ? this.m_textualPattern.Substring(0, this.m_textualPattern.Length - 1) : this.m_textualPattern;
    }

    public bool Matches(string filePath)
    {
      if (this.m_isWildcard)
        return true;
      if (string.IsNullOrEmpty(filePath))
        return false;
      string[] strArray;
      if (!this.m_matchFolder)
      {
        strArray = new string[1]
        {
          Path.GetFileName(filePath)
        };
      }
      else
      {
        string directoryName = Path.GetDirectoryName(filePath);
        if (directoryName == null)
          strArray = (string[]) null;
        else
          strArray = directoryName.Split('\\');
      }
      string[] source = strArray;
      if (source == null)
        return false;
      Match match = !this.m_startsWithWildcard || !this.m_endsWithWildcard ? (!this.m_startsWithWildcard ? (!this.m_endsWithWildcard ? (Match) (s => s.Equals(this.m_textualPattern, this.m_comparison)) : (Match) (s => s.StartsWith(this.m_textualPattern, this.m_comparison) && !s.Equals(this.m_textualPattern, this.m_comparison))) : (Match) (s => s.EndsWith(this.m_textualPattern, this.m_comparison) && !s.Equals(this.m_textualPattern, this.m_comparison))) : (Match) (s => s.IndexOf(this.m_textualPattern, this.m_comparison) > 0 && !s.Equals(this.m_textualPattern, this.m_comparison));
      return ((IEnumerable<string>) source).Any<string>((Func<string, bool>) (s => match(s)));
    }

    public bool TryMatches(IVssRequestContext requestContext, string filePath, bool defaultResult)
    {
      try
      {
        return this.Matches(filePath);
      }
      catch (PathTooLongException ex)
      {
        requestContext.Trace(15280025, TraceLevel.Verbose, "ProjectAnalysisService", nameof (LanguageConfigurationPattern), ex.Message);
        return defaultResult;
      }
      catch (ArgumentException ex)
      {
        requestContext.Trace(15280025, TraceLevel.Verbose, "ProjectAnalysisService", nameof (LanguageConfigurationPattern), ex.Message);
        return defaultResult;
      }
    }
  }
}
