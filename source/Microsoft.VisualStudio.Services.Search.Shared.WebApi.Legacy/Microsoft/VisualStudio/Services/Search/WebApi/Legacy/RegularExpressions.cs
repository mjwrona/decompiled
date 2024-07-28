// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.RegularExpressions
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  public static class RegularExpressions
  {
    public static readonly Regex WhitespaceRegex = new Regex("\\s+", RegexOptions.Compiled);
    public static readonly Regex WildcardRegex = new Regex("[\\*\\?]+", RegexOptions.Compiled);
    public static readonly Regex SlashTrimRegex = new Regex("(\\s*\\\\\\s*)+", RegexOptions.Compiled);
    public static readonly Regex PrefixWildcardRegex = new Regex("^\\s*[\\*\\?]+", RegexOptions.Compiled);
    public static readonly Regex PostfixWildcardRegex = new Regex("[\\*\\?]+\\s*$", RegexOptions.Compiled);
    public static readonly Regex SupportedSubStringRegex = new Regex("^\\s*[\\*]+[\\w]{3,}[\\*]+\\s*$", RegexOptions.Compiled);
    public static readonly Regex SubStringRegex = new Regex("^\\s*[\\*]+[\\w]+[\\*]+\\s*$", RegexOptions.Compiled);
    public static readonly Regex SpecialCharsRegex = new Regex("[^A-Za-z0-9_\\?\\*]", RegexOptions.Compiled);
    public static readonly Regex WildcardOnlyRegex = new Regex("^[\\s\\*\\?]*$", RegexOptions.Compiled);
    public static readonly Regex SpecialCharRegexForAdvancedQuery = new Regex("[^A-Za-z0-9_\\?\\*()\\[\\]:]", RegexOptions.Compiled);
    public static readonly Regex SpecialCharRegexWithoutWildcardAndSpace = new Regex("[^A-Za-z0-9_\\?\\*\\s]", RegexOptions.Compiled);
    public static readonly Regex SpecialCharRegexForPhraseSuggesterQuery = new Regex("[\\*\\?\\\\\\#\\/]+", RegexOptions.Compiled);
    public static readonly Regex BooleanOperatorRegex = new Regex("AND|OR|NOT", RegexOptions.Compiled);
    public static readonly Regex ProximityOperandRegex = new Regex("^[\\w]+$", RegexOptions.Compiled);
    public static readonly Regex MatchAllRegex = new Regex("^[\\s\\*]+$", RegexOptions.Compiled);
    public static readonly Regex PrefixInfixSuffixWildcardRegex = new Regex("^\\s*[\\*]+[^\\*]+[\\*]+[^\\*]+[\\*]+\\s*$", RegexOptions.Compiled);
    public static readonly Regex SubstringTooShortRegex = new Regex("^\\s*[\\*]+[\\w]{1,2}[\\*]+\\s*$", RegexOptions.Compiled);
    public static readonly Regex QuestionMarkWildcardRegex = new Regex("^\\s*[\\?]+[\\w]+[\\?]+\\s*$", RegexOptions.Compiled);
    public static readonly Regex MixedWildcardRegex = new Regex("^\\s*[\\?]+[\\w]+[\\*]+\\s*$|^\\s*[\\*]+[\\w]+[\\?]+\\s*$", RegexOptions.Compiled);
  }
}
