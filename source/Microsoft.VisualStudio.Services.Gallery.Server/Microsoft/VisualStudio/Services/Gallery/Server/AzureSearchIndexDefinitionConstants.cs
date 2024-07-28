// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzureSearchIndexDefinitionConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class AzureSearchIndexDefinitionConstants
  {
    public readonly int ExtensionDisplayNameExactMatchWeight = 10000000;
    public readonly int ExtensionDisplayNameWeight = 1000000;
    public readonly int ExtensionDisplayNameForPrefixMatchWeight = 100000;
    public readonly int ShortDescriptionWeight = 10000;
    public readonly int TagsWeight = 10000;
    public readonly int ExtensionFullyQualifiedNameExactMatchWeight = 10000;
    public readonly int ExtensionNameExactMatchWeight = 10000;
    public readonly int PublisherNameExactMatchWeight = 10000;
    public readonly int PublisherDisplayNameWeight = 1000;
    public readonly int ShortDescriptionForPrefixMatchWeight = 100;
    public readonly string SlashFilterName = "SlashFilter";
    public readonly string DashFilterName = "DashFilter";
    public readonly string UnderScoreFilterName = "UnderScoreFilter";
    public readonly string RoundBracketOpeningFilterName = "RoundBracketOpeningFilter";
    public readonly string RoundBracketClosingFilterName = "RoundBracketClosingFilter";
    public readonly string DefaultIndexAnalyzerName = "DefaultIndexAnalyzer";
    public readonly string PrefixAnalyzerName = "PrefixAnalyzer";
    public readonly string SearchTermAnalyzerName = "SearchTermAnalyzer";
    public readonly string KeywordIndexAnalyzer = nameof (KeywordIndexAnalyzer);
    public readonly string PrefixCreatingTokenFilterName = "NGramPrefix";
    public readonly string EnglishStemmerTokenFilterName = "EnglishStemmer";
    public readonly string CatenateNumberFilterTokenFilterName = "CatenateNumber";
    public readonly string ScoringProfileName = "WeightedSearch";
    public readonly string DownloadCountScoringFunctionFieldName = "DownloadCount";
    public readonly string InstallCountScoringFunctionFieldName = "InstallCount";
    public readonly string WeightedRatingScoringFunctionFieldName = "WeightedRating";
    public readonly int WeightedRatingScoringFunctionBoost = 100;
    public readonly int WeightedRatingScoringFunctionBoostingRangeStart = 3;
    public readonly int WeightedRatingScoringFunctionBoostingRangeEnd = 5;

    public virtual int DownloadCountScoringFunctionBoost { get; } = 10000;

    public virtual int DownloadCountScoringFunctionBoostingRangeStart { get; } = 1000;

    public virtual int DownloadCountScoringFunctionBoostingRangeEnd { get; } = 20000000;

    public virtual int InstallCountScoringFunctionBoost { get; } = 10000;

    public virtual int InstallCountScoringFunctionBoostingRangeStart { get; } = 500;

    public virtual int InstallCountScoringFunctionBoostingRangeEnd { get; } = 5000000;
  }
}
