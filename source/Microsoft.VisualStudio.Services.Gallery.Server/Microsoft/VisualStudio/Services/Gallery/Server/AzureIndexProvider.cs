// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzureIndexProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class AzureIndexProvider
  {
    private AzureSearchConverter m_azureSearchConverter;
    private AzureSearchSDKWrapper m_azureSearchSDKWrapper;
    private static readonly string s_Slash = "/";
    private static readonly string s_Dash = "-";
    private static readonly string s_UnderScore = "_";
    private static readonly string s_RoundBracketOpening = "\\(";
    private static readonly string s_RoundBracketClosing = "\\)";
    private static readonly string s_Space = " ";
    private const string s_area = "gallery";
    private const string s_layer = "AzureIndexProvider";
    private readonly int maxSupportedMinorVersionIncrement;
    private string searchIndexName;
    private readonly bool enableSortByInstallCountIsOn;
    private readonly IReadOnlyCollection<string> supportedProductArchitectures;

    public AzureIndexProvider(
      IVssRequestContext requestContext,
      string serviceName,
      string indexName,
      string key,
      AzureClientMode mode)
    {
      this.searchIndexName = indexName;
      this.m_azureSearchSDKWrapper = new AzureSearchSDKWrapper(serviceName, indexName, key, mode);
      this.enableSortByInstallCountIsOn = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSortByInstallCount");
      this.maxSupportedMinorVersionIncrement = AzureSearchVsIdeSupport.GetMaxSupportedMinorVersionIncrement(requestContext);
      this.supportedProductArchitectures = AzureSearchVsIdeSupport.GetSupportedProductArchitectures(requestContext);
      this.m_azureSearchConverter = new AzureSearchConverter(AzureSearchVsIdeSupport.GetMaxSupportedMajorVersion(requestContext), this.maxSupportedMinorVersionIncrement, AzureSearchVsIdeSupport.GetSupportedMajorVersions(requestContext), this.supportedProductArchitectures);
    }

    internal AzureIndexProvider()
    {
    }

    internal AzureIndexProvider(
      AzureSearchConverter azureSearchConverter,
      AzureSearchSDKWrapper azureSearchSDKWrapper)
    {
      this.m_azureSearchConverter = azureSearchConverter;
      this.m_azureSearchSDKWrapper = azureSearchSDKWrapper;
    }

    internal AzureIndexProvider(
      string indexName,
      AzureSearchConverter azureSearchConverter,
      AzureSearchSDKWrapper azureSearchSDKWrapper)
    {
      this.searchIndexName = indexName;
      this.m_azureSearchConverter = azureSearchConverter;
      this.m_azureSearchSDKWrapper = azureSearchSDKWrapper;
    }

    public virtual void CreateIndex(AzureSearchIndexDefinitionConstants indexConstants) => this.m_azureSearchSDKWrapper.CreateIndex(this.GetExtensionIndexDefinition(indexConstants));

    public virtual void CreateOrUpdateIndex(AzureSearchIndexDefinitionConstants indexConstants) => this.m_azureSearchSDKWrapper.CreateOrUpdateIndex(this.GetExtensionIndexDefinition(indexConstants));

    public virtual void CreateVSCodeIndex(AzureSearchIndexDefinitionConstants indexConstants) => this.m_azureSearchSDKWrapper.CreateIndex(this.GetVSCodeExtensionIndexDefinition(indexConstants));

    public virtual void CreateOrUpdateVSCodeIndex(AzureSearchIndexDefinitionConstants indexConstants) => this.m_azureSearchSDKWrapper.CreateOrUpdateIndex(this.GetVSCodeExtensionIndexDefinition(indexConstants));

    public virtual void UploadSynonymMap(string synonymMapName, string synonymMapValue = "")
    {
      if (string.IsNullOrWhiteSpace(synonymMapName) || string.IsNullOrWhiteSpace(synonymMapValue))
        throw new NotSupportedException("Either the synonymMapName or synonmMapValue is empty");
      if (this.m_azureSearchSDKWrapper.GetSynonymMap(synonymMapName) == null)
      {
        this.m_azureSearchSDKWrapper.UploadSynonymMap(synonymMapName, synonymMapValue);
        Index indexDefinition = this.m_azureSearchSDKWrapper.GetIndexDefinition();
        this.AssignSynonymMapToSearchableFields(indexDefinition, synonymMapName);
        this.m_azureSearchSDKWrapper.CreateOrUpdateIndex(indexDefinition);
      }
      else
        this.m_azureSearchSDKWrapper.UploadSynonymMap(synonymMapName, synonymMapValue);
    }

    public virtual void RemoveSynonymMap(string synonymMapName)
    {
      if (string.IsNullOrWhiteSpace(synonymMapName))
        throw new NotSupportedException("Input synonymMapName is empty");
      this.m_azureSearchSDKWrapper.RemoveSynonymMap(synonymMapName);
      Index indexDefinition = this.m_azureSearchSDKWrapper.GetIndexDefinition();
      this.RemoveSynonymMapingFromSearchableFields(indexDefinition, synonymMapName);
      this.m_azureSearchSDKWrapper.CreateOrUpdateIndex(indexDefinition);
    }

    public virtual void DeleteIndex() => this.m_azureSearchSDKWrapper.DeleteIndex();

    public virtual void PopulateIndex(
      List<PublishedExtension> extensionsList,
      bool useNewIndexDefinition = false,
      bool useProductArchitectureInfo = false,
      bool isPlatformSpecificExtensionsForVSCodeEnabled = false,
      bool usePublisherDomainInfo = false)
    {
      List<AzureIndexDocument> indexObject = this.m_azureSearchConverter.ConvertExtensionObjectToIndexObject((IList<PublishedExtension>) extensionsList, useNewIndexDefinition, useProductArchitectureInfo, isPlatformSpecificExtensionsForVSCodeEnabled, usePublisherDomainInfo);
      if (!indexObject.IsNullOrEmpty<AzureIndexDocument>())
        this.m_azureSearchSDKWrapper.PopulateIndex(indexObject);
      else
        TeamFoundationTracingService.TraceRaw(12060105, TraceLevel.Info, "gallery", nameof (AzureIndexProvider), "No extensions found to index.");
    }

    public virtual void DeleteEntries(List<PublishedExtension> extensionsList)
    {
      List<AzureIndexDocument> indexDocumentList = new List<AzureIndexDocument>();
      foreach (PublishedExtension extensions in extensionsList)
        indexDocumentList.Add(new AzureIndexDocument()
        {
          ExtensionId = extensions.ExtensionId.ToString()
        });
      this.m_azureSearchSDKWrapper.DeleteEntries(indexDocumentList);
    }

    private Index GetExtensionIndexDefinition(AzureSearchIndexDefinitionConstants indexConstants)
    {
      TextWeights textWeights = new TextWeights()
      {
        Weights = (IDictionary<string, double>) new Dictionary<string, double>()
      };
      textWeights.Weights.Add("ExtensionDisplayName", (double) indexConstants.ExtensionDisplayNameWeight);
      textWeights.Weights.Add("ExtensionDisplayNameForPrefixMatch", (double) indexConstants.ExtensionDisplayNameForPrefixMatchWeight);
      textWeights.Weights.Add("ShortDescription", (double) indexConstants.ShortDescriptionWeight);
      textWeights.Weights.Add("PublisherDisplayName", (double) indexConstants.PublisherDisplayNameWeight);
      textWeights.Weights.Add("ShortDescriptionForPrefixMatch", (double) indexConstants.ShortDescriptionForPrefixMatchWeight);
      textWeights.Weights.Add("SearchableTags", (double) indexConstants.TagsWeight);
      textWeights.Weights.Add("ExtensionFullyQualifiedNameForExactMatch", (double) indexConstants.ExtensionFullyQualifiedNameExactMatchWeight);
      textWeights.Weights.Add("PublisherNameForExactMatch", (double) indexConstants.PublisherNameExactMatchWeight);
      textWeights.Weights.Add("ExtensionNameForExactMatch", (double) indexConstants.ExtensionNameExactMatchWeight);
      textWeights.Weights.Add("ExtensionDisplayNameForExactMatch", (double) indexConstants.ExtensionDisplayNameExactMatchWeight);
      MagnitudeScoringFunction magnitudeScoringFunction1;
      if (this.enableSortByInstallCountIsOn)
      {
        MagnitudeScoringFunction magnitudeScoringFunction2 = new MagnitudeScoringFunction();
        ((ScoringFunction) magnitudeScoringFunction2).FieldName = indexConstants.InstallCountScoringFunctionFieldName;
        ((ScoringFunction) magnitudeScoringFunction2).Boost = (double) indexConstants.InstallCountScoringFunctionBoost;
        ((ScoringFunction) magnitudeScoringFunction2).Interpolation = new ScoringFunctionInterpolation?((ScoringFunctionInterpolation) 0);
        magnitudeScoringFunction2.Parameters = new MagnitudeScoringParameters()
        {
          BoostingRangeEnd = (double) indexConstants.InstallCountScoringFunctionBoostingRangeEnd,
          BoostingRangeStart = (double) indexConstants.InstallCountScoringFunctionBoostingRangeStart,
          ShouldBoostBeyondRangeByConstant = new bool?(true)
        };
        magnitudeScoringFunction1 = magnitudeScoringFunction2;
      }
      else
      {
        MagnitudeScoringFunction magnitudeScoringFunction3 = new MagnitudeScoringFunction();
        ((ScoringFunction) magnitudeScoringFunction3).FieldName = indexConstants.DownloadCountScoringFunctionFieldName;
        ((ScoringFunction) magnitudeScoringFunction3).Boost = (double) indexConstants.DownloadCountScoringFunctionBoost;
        ((ScoringFunction) magnitudeScoringFunction3).Interpolation = new ScoringFunctionInterpolation?((ScoringFunctionInterpolation) 0);
        magnitudeScoringFunction3.Parameters = new MagnitudeScoringParameters()
        {
          BoostingRangeEnd = (double) indexConstants.DownloadCountScoringFunctionBoostingRangeEnd,
          BoostingRangeStart = (double) indexConstants.DownloadCountScoringFunctionBoostingRangeStart,
          ShouldBoostBeyondRangeByConstant = new bool?(true)
        };
        magnitudeScoringFunction1 = magnitudeScoringFunction3;
      }
      Index extensionIndexDefinition = new Index();
      extensionIndexDefinition.Name = this.searchIndexName;
      extensionIndexDefinition.Fields = FieldBuilder.BuildForType<AzureIndexDocument>();
      Index index1 = extensionIndexDefinition;
      CustomAnalyzer[] customAnalyzerArray = new CustomAnalyzer[4];
      CustomAnalyzer customAnalyzer1 = new CustomAnalyzer();
      ((Analyzer) customAnalyzer1).Name = indexConstants.DefaultIndexAnalyzerName;
      customAnalyzer1.CharFilters = (IList<CharFilterName>) new CharFilterName[5]
      {
        CharFilterName.Create(indexConstants.SlashFilterName),
        CharFilterName.Create(indexConstants.DashFilterName),
        CharFilterName.Create(indexConstants.UnderScoreFilterName),
        CharFilterName.Create(indexConstants.RoundBracketOpeningFilterName),
        CharFilterName.Create(indexConstants.RoundBracketClosingFilterName)
      };
      customAnalyzer1.Tokenizer = TokenizerName.Whitespace;
      customAnalyzer1.TokenFilters = (IList<TokenFilterName>) new TokenFilterName[2]
      {
        TokenFilterName.Lowercase,
        TokenFilterName.Trim
      };
      customAnalyzerArray[0] = customAnalyzer1;
      CustomAnalyzer customAnalyzer2 = new CustomAnalyzer();
      ((Analyzer) customAnalyzer2).Name = indexConstants.PrefixAnalyzerName;
      customAnalyzer2.Tokenizer = TokenizerName.Whitespace;
      customAnalyzer2.TokenFilters = (IList<TokenFilterName>) new TokenFilterName[6]
      {
        TokenFilterName.Lowercase,
        TokenFilterName.Stopwords,
        TokenFilterName.Create(indexConstants.PrefixCreatingTokenFilterName),
        TokenFilterName.Trim,
        TokenFilterName.Create(indexConstants.CatenateNumberFilterTokenFilterName),
        TokenFilterName.Unique
      };
      customAnalyzerArray[1] = customAnalyzer2;
      CustomAnalyzer customAnalyzer3 = new CustomAnalyzer();
      ((Analyzer) customAnalyzer3).Name = indexConstants.SearchTermAnalyzerName;
      customAnalyzer3.Tokenizer = TokenizerName.Whitespace;
      customAnalyzer3.TokenFilters = (IList<TokenFilterName>) new TokenFilterName[1]
      {
        TokenFilterName.Lowercase
      };
      customAnalyzerArray[2] = customAnalyzer3;
      CustomAnalyzer customAnalyzer4 = new CustomAnalyzer();
      ((Analyzer) customAnalyzer4).Name = indexConstants.KeywordIndexAnalyzer;
      customAnalyzer4.Tokenizer = TokenizerName.Keyword;
      customAnalyzer4.TokenFilters = (IList<TokenFilterName>) new TokenFilterName[2]
      {
        TokenFilterName.Lowercase,
        TokenFilterName.Trim
      };
      customAnalyzerArray[3] = customAnalyzer4;
      index1.Analyzers = (IList<Analyzer>) customAnalyzerArray;
      Index index2 = extensionIndexDefinition;
      List<TokenFilter> tokenFilterList = new List<TokenFilter>();
      EdgeNGramTokenFilterV2 ngramTokenFilterV2 = new EdgeNGramTokenFilterV2();
      ((TokenFilter) ngramTokenFilterV2).Name = indexConstants.PrefixCreatingTokenFilterName;
      ngramTokenFilterV2.MinGram = new int?(1);
      ngramTokenFilterV2.MaxGram = new int?(50);
      tokenFilterList.Add((TokenFilter) ngramTokenFilterV2);
      WordDelimiterTokenFilter delimiterTokenFilter = new WordDelimiterTokenFilter();
      ((TokenFilter) delimiterTokenFilter).Name = indexConstants.CatenateNumberFilterTokenFilterName;
      delimiterTokenFilter.CatenateNumbers = new bool?(true);
      delimiterTokenFilter.CatenateWords = new bool?(true);
      delimiterTokenFilter.CatenateAll = new bool?(true);
      delimiterTokenFilter.PreserveOriginal = new bool?(true);
      tokenFilterList.Add((TokenFilter) delimiterTokenFilter);
      StemmerTokenFilter stemmerTokenFilter = new StemmerTokenFilter();
      ((TokenFilter) stemmerTokenFilter).Name = indexConstants.EnglishStemmerTokenFilterName;
      stemmerTokenFilter.Language = (StemmerTokenFilterLanguage) 10;
      tokenFilterList.Add((TokenFilter) stemmerTokenFilter);
      index2.TokenFilters = (IList<TokenFilter>) tokenFilterList;
      extensionIndexDefinition.CharFilters = (IList<CharFilter>) new List<CharFilter>()
      {
        (CharFilter) new PatternReplaceCharFilter(indexConstants.SlashFilterName, AzureIndexProvider.s_Slash, AzureIndexProvider.s_Space),
        (CharFilter) new PatternReplaceCharFilter(indexConstants.DashFilterName, AzureIndexProvider.s_Dash, AzureIndexProvider.s_Space),
        (CharFilter) new PatternReplaceCharFilter(indexConstants.UnderScoreFilterName, AzureIndexProvider.s_UnderScore, AzureIndexProvider.s_Space),
        (CharFilter) new PatternReplaceCharFilter(indexConstants.RoundBracketOpeningFilterName, AzureIndexProvider.s_RoundBracketOpening, AzureIndexProvider.s_Space),
        (CharFilter) new PatternReplaceCharFilter(indexConstants.RoundBracketClosingFilterName, AzureIndexProvider.s_RoundBracketClosing, AzureIndexProvider.s_Space)
      };
      Index index3 = extensionIndexDefinition;
      List<ScoringProfile> scoringProfileList1 = new List<ScoringProfile>();
      List<ScoringProfile> scoringProfileList2 = scoringProfileList1;
      ScoringProfile scoringProfile1 = new ScoringProfile();
      scoringProfile1.Name = indexConstants.ScoringProfileName;
      scoringProfile1.TextWeights = textWeights;
      ScoringProfile scoringProfile2 = scoringProfile1;
      MagnitudeScoringFunction[] magnitudeScoringFunctionArray = new MagnitudeScoringFunction[2]
      {
        magnitudeScoringFunction1,
        null
      };
      MagnitudeScoringFunction magnitudeScoringFunction4 = new MagnitudeScoringFunction();
      ((ScoringFunction) magnitudeScoringFunction4).FieldName = indexConstants.WeightedRatingScoringFunctionFieldName;
      ((ScoringFunction) magnitudeScoringFunction4).Boost = (double) indexConstants.WeightedRatingScoringFunctionBoost;
      ((ScoringFunction) magnitudeScoringFunction4).Interpolation = new ScoringFunctionInterpolation?((ScoringFunctionInterpolation) 2);
      magnitudeScoringFunction4.Parameters = new MagnitudeScoringParameters()
      {
        BoostingRangeEnd = (double) indexConstants.WeightedRatingScoringFunctionBoostingRangeEnd,
        BoostingRangeStart = (double) indexConstants.WeightedRatingScoringFunctionBoostingRangeStart
      };
      magnitudeScoringFunctionArray[1] = magnitudeScoringFunction4;
      scoringProfile2.Functions = (IList<ScoringFunction>) magnitudeScoringFunctionArray;
      scoringProfile1.FunctionAggregation = new ScoringFunctionAggregation?((ScoringFunctionAggregation) 0);
      ScoringProfile scoringProfile3 = scoringProfile1;
      scoringProfileList2.Add(scoringProfile3);
      List<ScoringProfile> scoringProfileList3 = scoringProfileList1;
      index3.ScoringProfiles = (IList<ScoringProfile>) scoringProfileList3;
      extensionIndexDefinition.DefaultScoringProfile = indexConstants.ScoringProfileName;
      return extensionIndexDefinition;
    }

    private Index GetVSCodeExtensionIndexDefinition(
      AzureSearchIndexDefinitionConstants indexConstants)
    {
      TextWeights textWeights = new TextWeights()
      {
        Weights = (IDictionary<string, double>) new Dictionary<string, double>()
      };
      textWeights.Weights.Add("ExtensionDisplayName", (double) indexConstants.ExtensionDisplayNameWeight);
      textWeights.Weights.Add("ExtensionDisplayNameForPrefixMatch", (double) indexConstants.ExtensionDisplayNameForPrefixMatchWeight);
      textWeights.Weights.Add("ShortDescription", (double) indexConstants.ShortDescriptionWeight);
      textWeights.Weights.Add("PublisherDisplayName", (double) indexConstants.PublisherDisplayNameWeight);
      textWeights.Weights.Add("ShortDescriptionForPrefixMatch", (double) indexConstants.ShortDescriptionForPrefixMatchWeight);
      textWeights.Weights.Add("SearchableTags", (double) indexConstants.TagsWeight);
      textWeights.Weights.Add("ExtensionFullyQualifiedNameForExactMatch", (double) indexConstants.ExtensionFullyQualifiedNameExactMatchWeight);
      textWeights.Weights.Add("PublisherNameForExactMatch", (double) indexConstants.PublisherNameExactMatchWeight);
      textWeights.Weights.Add("ExtensionNameForExactMatch", (double) indexConstants.ExtensionNameExactMatchWeight);
      textWeights.Weights.Add("ExtensionDisplayNameForExactMatch", (double) indexConstants.ExtensionDisplayNameExactMatchWeight);
      MagnitudeScoringFunction magnitudeScoringFunction1 = new MagnitudeScoringFunction();
      ((ScoringFunction) magnitudeScoringFunction1).FieldName = indexConstants.InstallCountScoringFunctionFieldName;
      ((ScoringFunction) magnitudeScoringFunction1).Boost = (double) indexConstants.InstallCountScoringFunctionBoost;
      ((ScoringFunction) magnitudeScoringFunction1).Interpolation = new ScoringFunctionInterpolation?((ScoringFunctionInterpolation) 0);
      magnitudeScoringFunction1.Parameters = new MagnitudeScoringParameters()
      {
        BoostingRangeEnd = (double) indexConstants.InstallCountScoringFunctionBoostingRangeEnd,
        BoostingRangeStart = (double) indexConstants.InstallCountScoringFunctionBoostingRangeStart,
        ShouldBoostBeyondRangeByConstant = new bool?(true)
      };
      MagnitudeScoringFunction magnitudeScoringFunction2 = magnitudeScoringFunction1;
      Index extensionIndexDefinition = new Index();
      extensionIndexDefinition.Name = this.searchIndexName;
      extensionIndexDefinition.Fields = FieldBuilder.BuildForType<VSCodeIndexDocument>();
      Index index1 = extensionIndexDefinition;
      CustomAnalyzer[] customAnalyzerArray = new CustomAnalyzer[4];
      CustomAnalyzer customAnalyzer1 = new CustomAnalyzer();
      ((Analyzer) customAnalyzer1).Name = indexConstants.DefaultIndexAnalyzerName;
      customAnalyzer1.CharFilters = (IList<CharFilterName>) new CharFilterName[5]
      {
        CharFilterName.Create(indexConstants.SlashFilterName),
        CharFilterName.Create(indexConstants.DashFilterName),
        CharFilterName.Create(indexConstants.UnderScoreFilterName),
        CharFilterName.Create(indexConstants.RoundBracketOpeningFilterName),
        CharFilterName.Create(indexConstants.RoundBracketClosingFilterName)
      };
      customAnalyzer1.Tokenizer = TokenizerName.Whitespace;
      customAnalyzer1.TokenFilters = (IList<TokenFilterName>) new TokenFilterName[3]
      {
        TokenFilterName.Lowercase,
        TokenFilterName.Trim,
        TokenFilterName.Create(indexConstants.EnglishStemmerTokenFilterName)
      };
      customAnalyzerArray[0] = customAnalyzer1;
      CustomAnalyzer customAnalyzer2 = new CustomAnalyzer();
      ((Analyzer) customAnalyzer2).Name = indexConstants.PrefixAnalyzerName;
      customAnalyzer2.Tokenizer = TokenizerName.Whitespace;
      customAnalyzer2.TokenFilters = (IList<TokenFilterName>) new TokenFilterName[6]
      {
        TokenFilterName.Lowercase,
        TokenFilterName.Stopwords,
        TokenFilterName.Create(indexConstants.PrefixCreatingTokenFilterName),
        TokenFilterName.Trim,
        TokenFilterName.Create(indexConstants.CatenateNumberFilterTokenFilterName),
        TokenFilterName.Unique
      };
      customAnalyzerArray[1] = customAnalyzer2;
      CustomAnalyzer customAnalyzer3 = new CustomAnalyzer();
      ((Analyzer) customAnalyzer3).Name = indexConstants.SearchTermAnalyzerName;
      customAnalyzer3.Tokenizer = TokenizerName.Whitespace;
      customAnalyzer3.TokenFilters = (IList<TokenFilterName>) new TokenFilterName[2]
      {
        TokenFilterName.Lowercase,
        TokenFilterName.Create(indexConstants.EnglishStemmerTokenFilterName)
      };
      customAnalyzerArray[2] = customAnalyzer3;
      CustomAnalyzer customAnalyzer4 = new CustomAnalyzer();
      ((Analyzer) customAnalyzer4).Name = indexConstants.KeywordIndexAnalyzer;
      customAnalyzer4.Tokenizer = TokenizerName.Keyword;
      customAnalyzer4.TokenFilters = (IList<TokenFilterName>) new TokenFilterName[3]
      {
        TokenFilterName.Lowercase,
        TokenFilterName.Trim,
        TokenFilterName.Create(indexConstants.EnglishStemmerTokenFilterName)
      };
      customAnalyzerArray[3] = customAnalyzer4;
      index1.Analyzers = (IList<Analyzer>) customAnalyzerArray;
      Index index2 = extensionIndexDefinition;
      List<TokenFilter> tokenFilterList = new List<TokenFilter>();
      EdgeNGramTokenFilterV2 ngramTokenFilterV2 = new EdgeNGramTokenFilterV2();
      ((TokenFilter) ngramTokenFilterV2).Name = indexConstants.PrefixCreatingTokenFilterName;
      ngramTokenFilterV2.MinGram = new int?(1);
      ngramTokenFilterV2.MaxGram = new int?(50);
      tokenFilterList.Add((TokenFilter) ngramTokenFilterV2);
      WordDelimiterTokenFilter delimiterTokenFilter = new WordDelimiterTokenFilter();
      ((TokenFilter) delimiterTokenFilter).Name = indexConstants.CatenateNumberFilterTokenFilterName;
      delimiterTokenFilter.CatenateNumbers = new bool?(true);
      delimiterTokenFilter.CatenateWords = new bool?(true);
      delimiterTokenFilter.CatenateAll = new bool?(true);
      delimiterTokenFilter.PreserveOriginal = new bool?(true);
      tokenFilterList.Add((TokenFilter) delimiterTokenFilter);
      StemmerTokenFilter stemmerTokenFilter = new StemmerTokenFilter();
      ((TokenFilter) stemmerTokenFilter).Name = indexConstants.EnglishStemmerTokenFilterName;
      stemmerTokenFilter.Language = (StemmerTokenFilterLanguage) 10;
      tokenFilterList.Add((TokenFilter) stemmerTokenFilter);
      index2.TokenFilters = (IList<TokenFilter>) tokenFilterList;
      extensionIndexDefinition.CharFilters = (IList<CharFilter>) new List<CharFilter>()
      {
        (CharFilter) new PatternReplaceCharFilter(indexConstants.SlashFilterName, AzureIndexProvider.s_Slash, AzureIndexProvider.s_Space),
        (CharFilter) new PatternReplaceCharFilter(indexConstants.DashFilterName, AzureIndexProvider.s_Dash, AzureIndexProvider.s_Space),
        (CharFilter) new PatternReplaceCharFilter(indexConstants.UnderScoreFilterName, AzureIndexProvider.s_UnderScore, AzureIndexProvider.s_Space),
        (CharFilter) new PatternReplaceCharFilter(indexConstants.RoundBracketOpeningFilterName, AzureIndexProvider.s_RoundBracketOpening, AzureIndexProvider.s_Space),
        (CharFilter) new PatternReplaceCharFilter(indexConstants.RoundBracketClosingFilterName, AzureIndexProvider.s_RoundBracketClosing, AzureIndexProvider.s_Space)
      };
      Index index3 = extensionIndexDefinition;
      List<ScoringProfile> scoringProfileList1 = new List<ScoringProfile>();
      List<ScoringProfile> scoringProfileList2 = scoringProfileList1;
      ScoringProfile scoringProfile1 = new ScoringProfile();
      scoringProfile1.Name = indexConstants.ScoringProfileName;
      scoringProfile1.TextWeights = textWeights;
      ScoringProfile scoringProfile2 = scoringProfile1;
      MagnitudeScoringFunction[] magnitudeScoringFunctionArray = new MagnitudeScoringFunction[2]
      {
        magnitudeScoringFunction2,
        null
      };
      MagnitudeScoringFunction magnitudeScoringFunction3 = new MagnitudeScoringFunction();
      ((ScoringFunction) magnitudeScoringFunction3).FieldName = indexConstants.WeightedRatingScoringFunctionFieldName;
      ((ScoringFunction) magnitudeScoringFunction3).Boost = (double) indexConstants.WeightedRatingScoringFunctionBoost;
      ((ScoringFunction) magnitudeScoringFunction3).Interpolation = new ScoringFunctionInterpolation?((ScoringFunctionInterpolation) 2);
      magnitudeScoringFunction3.Parameters = new MagnitudeScoringParameters()
      {
        BoostingRangeEnd = (double) indexConstants.WeightedRatingScoringFunctionBoostingRangeEnd,
        BoostingRangeStart = (double) indexConstants.WeightedRatingScoringFunctionBoostingRangeStart
      };
      magnitudeScoringFunctionArray[1] = magnitudeScoringFunction3;
      scoringProfile2.Functions = (IList<ScoringFunction>) magnitudeScoringFunctionArray;
      scoringProfile1.FunctionAggregation = new ScoringFunctionAggregation?((ScoringFunctionAggregation) 0);
      ScoringProfile scoringProfile3 = scoringProfile1;
      scoringProfileList2.Add(scoringProfile3);
      List<ScoringProfile> scoringProfileList3 = scoringProfileList1;
      index3.ScoringProfiles = (IList<ScoringProfile>) scoringProfileList3;
      extensionIndexDefinition.DefaultScoringProfile = indexConstants.ScoringProfileName;
      return extensionIndexDefinition;
    }

    private List<string> GetSearchableFields() => new List<string>()
    {
      "ExtensionDisplayName",
      "ExtensionDisplayNameForExactMatch",
      "ExtensionDisplayNameForPrefixMatch",
      "ExtensionName",
      "ExtensionNameForExactMatch",
      "ExtensionFullyQualifiedNameForExactMatch",
      "PublisherDisplayName",
      "PublisherDisplayNameForExactMatch",
      "PublisherDisplayNameForPrefixMatch",
      "ShortDescription",
      "ShortDescriptionForPrefixMatch",
      "SearchableTags"
    };

    private void AssignSynonymMapToSearchableFields(Index indexDefinition, string synonymMapName)
    {
      foreach (string searchableField1 in this.GetSearchableFields())
      {
        string searchableField = searchableField1;
        if (indexDefinition.Fields.FirstOrDefault<Field>((Func<Field, bool>) (f => f.Name.Equals(searchableField))) != null)
          indexDefinition.Fields.First<Field>((Func<Field, bool>) (f => f.Name.Equals(searchableField))).SynonymMaps = new string[1]
          {
            synonymMapName
          };
      }
    }

    private void RemoveSynonymMapingFromSearchableFields(
      Index indexDefinition,
      string synonymMapToBeRemoved)
    {
      foreach (string searchableField1 in this.GetSearchableFields())
      {
        string searchableField = searchableField1;
        if (indexDefinition.Fields.FirstOrDefault<Field>((Func<Field, bool>) (f => f.Name.Equals(searchableField))) != null)
          indexDefinition.Fields.First<Field>((Func<Field, bool>) (f => f.Name.Equals(searchableField))).SynonymMaps = (string[]) null;
      }
    }

    public ExtensionQueryResult Search(
      ExtensionSearchParams extSearchParams,
      ExtensionQueryFlags queryFlags,
      bool useNewIndexDefinition = false,
      bool enableSortByInstallCountUI = false,
      bool useProductArchitectureInfo = false,
      bool includeInstallationTargetWithAndWithoutProductArchitecture = false,
      bool enableFilterOnTagsWithSearchText = false)
    {
      ExtensionQueryResult extensionQueryResult = new ExtensionQueryResult();
      SearchParameters searchParameters1 = new SearchParameters();
      SearchParameters searchParameters2 = new SearchParameters();
      string searchText = string.Empty;
      List<string> values1 = new List<string>();
      List<string> values2 = new List<string>();
      List<string> values3 = new List<string>();
      List<string> values4 = new List<string>();
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      List<string> values5 = new List<string>();
      List<string> source1 = new List<string>();
      List<string> values6 = new List<string>();
      List<string> stringList3 = new List<string>();
      List<string> source2 = new List<string>();
      List<string> productArchitectures = new List<string>();
      List<string> values7 = new List<string>();
      Version filterVersion = (Version) null;
      Version version = (Version) null;
      Version filterVersionRangeEnd = (Version) null;
      bool flag1 = false;
      bool flag2 = false;
      int result1 = 0;
      int result2 = 0;
      int result3 = 0;
      bool flag3 = false;
      SearchParameters searchParameters3 = new SearchParameters();
      searchParameters1.Facets = (IList<string>) new List<string>();
      if (extSearchParams.MetadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeResultCount))
        searchParameters1.IncludeTotalResultCount = true;
      if (extSearchParams.MetadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludePreCategoryFilterCategories) || extSearchParams.MetadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeResultSetCategories))
        searchParameters1.Facets.Add("Categories,count:55");
      try
      {
        for (int index = 0; index < extSearchParams.CriteriaList.Count; ++index)
        {
          SearchCriteria criteria = extSearchParams.CriteriaList[index];
          switch (criteria.FilterType)
          {
            case SearchFilterType.SearchPhrase:
              string enumerable = criteria.FilterValue;
              if (!enumerable.IsNullOrEmpty<char>() && !enumerable.StartsWith("\"") && !enumerable.EndsWith("\""))
                enumerable = "\"" + enumerable + "\"";
              values5.Add(enumerable);
              break;
            case SearchFilterType.SearchWord:
              values5.Add(criteria.FilterValue);
              break;
            case SearchFilterType.InstallationTarget:
              if (criteria.OperatorType == SearchFilterOperatorType.Or)
              {
                values4.Add(criteria.FilterValue);
                break;
              }
              values3.Add(criteria.FilterValue);
              break;
            case SearchFilterType.Category:
              if (criteria.OperatorType == SearchFilterOperatorType.Or)
              {
                values2.Add(criteria.FilterValue);
                break;
              }
              values1.Add(criteria.FilterValue);
              break;
            case SearchFilterType.Tag:
              stringList1.Add(criteria.FilterValue);
              break;
            case SearchFilterType.ExcludeExtensionsWithFlags:
              if (!int.TryParse(criteria.FilterValue, out result2))
                throw new InvalidExtensionQueryException("Exception while converting input exclude flags - " + criteria.FilterValue);
              break;
            case SearchFilterType.Name:
              if (criteria.FilterValue.StartsWith("\""))
                stringList2.Add("ExtensionDisplayName");
              else
                stringList2.Add("ExtensionDisplayNameForPrefixMatch");
              values5.Add(criteria.FilterValue);
              break;
            case SearchFilterType.Publisher:
              if (criteria.FilterValue.StartsWith("\""))
                stringList2.Add("PublisherDisplayName");
              else
                stringList2.Add("PublisherDisplayNameForPrefixMatch");
              values5.Add(criteria.FilterValue);
              break;
            case SearchFilterType.TagName:
              stringList2.Add("SearchableTags");
              values5.Add(criteria.FilterValue);
              break;
            case SearchFilterType.IncludeExtensionsWithFlags:
              if (!int.TryParse(criteria.FilterValue, out result1))
                throw new InvalidExtensionQueryException("Exception while converting input include flags - " + criteria.FilterValue);
              break;
            case SearchFilterType.Lcid:
              values6.Add(criteria.FilterValue);
              break;
            case SearchFilterType.Metadata:
              switch (criteria.OperatorType)
              {
                case SearchFilterOperatorType.Equal:
                  stringList3.Add(criteria.FilterValue);
                  continue;
                case SearchFilterOperatorType.NotEqual:
                  source2.Add(criteria.FilterValue);
                  continue;
                default:
                  continue;
              }
            case SearchFilterType.InstallationTargetVersion:
              filterVersion = this.GetParsedVersion(criteria.FilterValue);
              flag1 = true;
              break;
            case SearchFilterType.InstallationTargetVersionRange:
              string[] source3 = criteria.FilterValue.Split(new string[1]
              {
                "-"
              }, StringSplitOptions.RemoveEmptyEntries);
              if (((IEnumerable<string>) source3).Count<string>() != 2)
              {
                TeamFoundationTracingService.TraceRaw(12060106, TraceLevel.Info, "gallery", nameof (AzureIndexProvider), "Invalid installation target version range specified. Received value: " + criteria.FilterValue);
                break;
              }
              version = this.GetParsedVersion(source3[0]);
              filterVersionRangeEnd = this.GetParsedVersion(source3[1]);
              flag2 = true;
              break;
            case SearchFilterType.IncludePrivateExtensions:
              flag3 = true;
              break;
            case SearchFilterType.ExactPublisherDisplayName:
              values5.Add(criteria.FilterValue);
              stringList2.Add("PublisherDisplayNameForExactMatch");
              break;
            case SearchFilterType.IncludeExtensionsWithPublisherFlags:
              if (!int.TryParse(criteria.FilterValue, out result3))
                throw new InvalidExtensionQueryException("Exception while converting input publisher include flags - " + criteria.FilterValue);
              break;
            case SearchFilterType.OrganizationSharedWith:
              source1.Add(criteria.FilterValue);
              break;
            case SearchFilterType.ProductArchitecture:
              productArchitectures.Add(criteria.FilterValue);
              break;
            case SearchFilterType.TargetPlatform:
              values7.Add(criteria.FilterValue);
              break;
          }
        }
        string sortBy;
        string sortOrder;
        this.GetValidSortByTypeAndSortOrder(extSearchParams.SortBy, extSearchParams.SortOrder, enableSortByInstallCountUI, out sortBy, out sortOrder);
        if (!string.IsNullOrEmpty(sortBy))
          searchParameters1.OrderBy = (IList<string>) new List<string>()
          {
            string.Format("{0} {1}", (object) sortBy, (object) sortOrder)
          };
        if (values1.Count > 0)
        {
          string filterText = string.Format("Categories/any(t:search.in(t, '{0}', ','))", (object) string.Join(",", (IEnumerable<string>) values1));
          this.SetSearchParametersFilter(filterText, "and", searchParameters1);
          this.SetSearchParametersFilter(filterText, "and", searchParameters3);
        }
        if (values2.Count > 0)
        {
          string filterText = string.Format("Categories/any(t:search.in(t, '{0}', ','))", (object) string.Join(",", (IEnumerable<string>) values2));
          this.SetSearchParametersFilter(filterText, "and", searchParameters1);
          this.SetSearchParametersFilter(filterText, "and", searchParameters3);
        }
        if (values7.Count > 0)
        {
          string filterText = string.Format("TargetPlatforms/any(t:search.in(t, '{0}'))", (object) string.Join(",", (IEnumerable<string>) values7));
          this.SetSearchParametersFilter(filterText, "and", searchParameters1);
          this.SetSearchParametersFilter(filterText, "and", searchParameters2);
        }
        if (productArchitectures.IsNullOrEmpty<string>())
          productArchitectures.AddRange((IEnumerable<string>) (this.supportedProductArchitectures ?? AzureSearchVsIdeSupport.SupportedProductArchitecturesDefaultValues));
        if (flag1)
        {
          List<string> stringList4 = values3.Count > 0 ? values3 : values4;
          string format = "InstallationTargetList/any(t:search.in(t, '{0}'))";
          string filterText;
          if (useProductArchitectureInfo)
          {
            List<string> installationTargetVersionWithProductArchitecture = new List<string>();
            stringList4.ForEach((Action<string>) (target => productArchitectures.ForEach((Action<string>) (productArchitecture => this.AddInstallationTargetVersionWithProductArchitectureEntry(target, filterVersion.Major, filterVersion.Minor, productArchitecture, installationTargetVersionWithProductArchitecture, includeInstallationTargetWithAndWithoutProductArchitecture)))));
            filterText = string.Format(format, (object) string.Join(",", (IEnumerable<string>) installationTargetVersionWithProductArchitecture));
          }
          else
          {
            List<string> stringList5 = new List<string>();
            stringList5.AddRange(stringList4.Select<string, string>((Func<string, string>) (x => string.Format("{0}_{1}.{2}", (object) x, (object) filterVersion.Major, (object) filterVersion.Minor))));
            this.AddInstallationTargetVersionWithDefaultProductArchitecture(stringList4, filterVersion.Major, filterVersion.Minor, stringList5, includeInstallationTargetWithAndWithoutProductArchitecture);
            filterText = string.Format(format, (object) string.Join(",", (IEnumerable<string>) stringList5));
          }
          this.SetSearchParametersFilter(filterText, "and", searchParameters1);
          this.SetSearchParametersFilter(filterText, "and", searchParameters2);
        }
        else if (flag2)
        {
          List<string> stringList6 = values3.Count > 0 ? values3 : values4;
          List<string> values8 = new List<string>();
          for (int majorVersion = version.Major; majorVersion < filterVersionRangeEnd.Major; majorVersion++)
          {
            for (int minorVersion = majorVersion == version.Major ? version.Minor : 0; minorVersion < this.maxSupportedMinorVersionIncrement; minorVersion++)
            {
              if (useProductArchitectureInfo)
              {
                List<string> installationTargetVersionWithProductArchitecture = new List<string>();
                stringList6.ForEach((Action<string>) (target => productArchitectures.ForEach((Action<string>) (productArchitecture => this.AddInstallationTargetVersionWithProductArchitectureEntry(target, majorVersion, minorVersion, productArchitecture, installationTargetVersionWithProductArchitecture, includeInstallationTargetWithAndWithoutProductArchitecture)))));
                values8.AddRange((IEnumerable<string>) installationTargetVersionWithProductArchitecture);
              }
              else
              {
                List<string> stringList7 = new List<string>();
                stringList7.AddRange(stringList6.Select<string, string>((Func<string, string>) (x => string.Format("{0}_{1}.{2}", (object) x, (object) majorVersion, (object) minorVersion))));
                this.AddInstallationTargetVersionWithDefaultProductArchitecture(stringList6, majorVersion, minorVersion, stringList7, includeInstallationTargetWithAndWithoutProductArchitecture);
                values8.AddRange((IEnumerable<string>) stringList7);
              }
            }
          }
          for (int minorVersion = 0; minorVersion <= filterVersionRangeEnd.Minor; minorVersion++)
          {
            if (useProductArchitectureInfo)
            {
              List<string> installationTargetVersionWithProductArchitecture = new List<string>();
              stringList6.ForEach((Action<string>) (target => productArchitectures.ForEach((Action<string>) (productArchitecture => this.AddInstallationTargetVersionWithProductArchitectureEntry(target, filterVersionRangeEnd.Major, minorVersion, productArchitecture, installationTargetVersionWithProductArchitecture, includeInstallationTargetWithAndWithoutProductArchitecture)))));
              values8.AddRange((IEnumerable<string>) installationTargetVersionWithProductArchitecture);
            }
            else
            {
              List<string> stringList8 = new List<string>();
              stringList8.AddRange(stringList6.Select<string, string>((Func<string, string>) (x => string.Format("{0}_{1}.{2}", (object) x, (object) filterVersionRangeEnd.Major, (object) minorVersion))));
              this.AddInstallationTargetVersionWithDefaultProductArchitecture(stringList6, filterVersionRangeEnd.Major, minorVersion, stringList8, includeInstallationTargetWithAndWithoutProductArchitecture);
              values8.AddRange((IEnumerable<string>) stringList8);
            }
          }
          string filterText = "InstallationTargetList/any(t:search.in(t, '" + string.Join(",", (IEnumerable<string>) values8) + "'))";
          this.SetSearchParametersFilter(filterText, "and", searchParameters1);
          this.SetSearchParametersFilter(filterText, "and", searchParameters2);
        }
        else
        {
          if (values3.Count > 0)
          {
            string filterText = string.Format("InstallationTargetList/any(t:search.in(t, '{0}'))", (object) string.Join(",", (IEnumerable<string>) values3));
            this.SetSearchParametersFilter(filterText, "and", searchParameters1);
            this.SetSearchParametersFilter(filterText, "and", searchParameters2);
            this.SetSearchParametersFilter(filterText, "and", searchParameters3);
          }
          if (values4.Count > 0)
          {
            string filterText = string.Format("InstallationTargetList/any(t:search.in(t, '{0}'))", (object) string.Join(",", (IEnumerable<string>) values4));
            this.SetSearchParametersFilter(filterText, "and", searchParameters1);
            this.SetSearchParametersFilter(filterText, "and", searchParameters2);
            this.SetSearchParametersFilter(filterText, "and", searchParameters3);
          }
        }
        if (enableFilterOnTagsWithSearchText && stringList1.Any<string>())
        {
          string filterText = string.Format("SearchableTags/any(t:search.in(t, '{0}'))", (object) string.Join(",", (IEnumerable<string>) stringList1));
          this.SetSearchParametersFilter(filterText, "and", searchParameters1);
          this.SetSearchParametersFilter(filterText, "and", searchParameters2);
          this.SetSearchParametersFilter(filterText, "and", searchParameters3);
        }
        if (values6.Count > 0)
        {
          string filterText = "Lcids/any(t:search.in(t, '" + string.Join(",", (IEnumerable<string>) values6) + "'))";
          this.SetSearchParametersFilter(filterText, "and", searchParameters1);
          this.SetSearchParametersFilter(filterText, "and", searchParameters2);
          this.SetSearchParametersFilter(filterText, "and", searchParameters3);
        }
        if (stringList3.Count > 0)
        {
          foreach (string str in stringList3)
          {
            string filterText = "SearchableMetadata/any(t:search.in(t, '" + str.ToLower() + "'))";
            this.SetSearchParametersFilter(filterText, "and", searchParameters1);
            this.SetSearchParametersFilter(filterText, "and", searchParameters2);
          }
        }
        if (source2.Count > 0)
        {
          string filterText = string.Format("SearchableMetadata/all(t: not search.in(t, '{0}'))", (object) string.Join(",", (IEnumerable<string>) source2.Select<string, string>((Func<string, string>) (x => x.ToLower())).ToList<string>()));
          this.SetSearchParametersFilter(filterText, "and", searchParameters1);
          this.SetSearchParametersFilter(filterText, "and", searchParameters2);
        }
        searchParameters1.Top = new int?(extSearchParams.PageSize);
        searchParameters1.Skip = new int?((extSearchParams.PageNumber - 1) * extSearchParams.PageSize);
        if (stringList2.Count > 0)
        {
          searchParameters1.SearchFields = stringList2.Count <= 1 ? (IList<string>) stringList2 : throw new InvalidExtensionQueryException("Only one filter for search field is allowed");
          searchParameters2.SearchFields = (IList<string>) stringList2;
          searchParameters3.SearchFields = (IList<string>) stringList2;
        }
        searchText = string.Join(" ", (IEnumerable<string>) values5);
        int num = result1 & result2;
        if (num != 0)
          result1 &= ~num;
        if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.ExcludeNonValidated))
          result1 |= 4;
        string str1 = string.Empty;
        if (!flag3)
          result1 |= 256;
        else if (source1.Count > 0)
        {
          string flagFilter = this.GetFlagFilter("EnterpriseSharedWithIds/any(t:search.in(t, '" + string.Join(",", source1.Select<string, string>((Func<string, string>) (x => x.ToUpper()))) + "')) and ExtensionFlags/all(t: not search.in(t, 'Public'))", "and", str1);
          if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.ExcludeNonValidated))
            flagFilter = this.GetFlagFilter("ExtensionFlags/any(t: search.in(t, 'Validated'))", "and", flagFilter);
          str1 = "(" + flagFilter + ")";
          if ((result2 & 256) == 0)
            str1 = "(" + this.GetFlagFilter("ExtensionFlags/any(t: search.in(t, 'Public'))", "or", str1) + ")";
        }
        string str2 = string.Empty;
        if (result1 != 0)
        {
          foreach (PublishedExtensionFlags publishedExtensionFlags in Enum.GetValues(typeof (PublishedExtensionFlags)))
          {
            if (((PublishedExtensionFlags) result1 & publishedExtensionFlags) > PublishedExtensionFlags.None)
              str2 = this.GetFlagFilter(string.Format("ExtensionFlags/any(t: search.in(t, '{0}'))", (object) publishedExtensionFlags.ToString()), "and", str2);
          }
        }
        if (result2 != 0)
          str2 = this.GetFlagFilter(string.Format("ExtensionFlags/all(t: not search.in(t, '{0}'))", (object) Enum.Format(typeof (PublishedExtensionFlags), (object) result2, "G")), "and", str2);
        if (!string.IsNullOrEmpty(str1))
          str2 = this.GetFlagFilter(str1, "and", str2);
        this.SetSearchParametersFilter(str2, "and", searchParameters1);
        this.SetSearchParametersFilter(str2, "and", searchParameters2);
        this.SetSearchParametersFilter(str2, "and", searchParameters3);
        if (result3 != 0)
        {
          foreach (PublisherFlags publisherFlags in Enum.GetValues(typeof (PublisherFlags)))
          {
            if (publisherFlags != PublisherFlags.ServiceFlags && ((PublisherFlags) result3 & publisherFlags) > PublisherFlags.None)
            {
              string filterText = string.Format("PublisherFlags/any(t: search.in(t, '{0}'))", (object) publisherFlags.ToString());
              this.SetSearchParametersFilter(filterText, "and", searchParameters1);
              this.SetSearchParametersFilter(filterText, "and", searchParameters2);
              this.SetSearchParametersFilter(filterText, "and", searchParameters3);
            }
          }
        }
        searchParameters1.Select = (IList<string>) this.GetDefaultSelectFields();
        if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeCategoryAndTags))
        {
          searchParameters1.Select.Add("Categories");
          searchParameters1.Select.Add("Tags");
        }
        if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeInstallationTargets))
          searchParameters1.Select.Add("InstallationTargets");
        if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedAccounts) || queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedOrganizations))
          searchParameters1.Select.Add("SharedWith");
        if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeStatistics))
          searchParameters1.Select.Add("Statistics");
        if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeMetadata))
          searchParameters1.Select.Add("Metadata");
        if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeLcids))
          searchParameters1.Select.Add("Lcids");
        if (extSearchParams.MetadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeResultSetProjectType))
          searchParameters1.Facets.Add("ProjectType,count:55");
        if (extSearchParams.MetadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeTargetPlatforms))
          searchParameters1.Facets.Add(string.Format("{0},count:{1}", (object) "TargetPlatforms", (object) 30));
        DocumentSearchResult<AzureIndexDocument> searchResults = this.m_azureSearchSDKWrapper.Search(searchText, searchParameters1);
        DocumentSearchResult<AzureIndexDocument> searchResultsForCategoryMetadata = (DocumentSearchResult<AzureIndexDocument>) null;
        DocumentSearchResult<AzureIndexDocument> searchResultForTargetPlatformMetadata = (DocumentSearchResult<AzureIndexDocument>) null;
        if (((DocumentSearchResultBase<SearchResult<AzureIndexDocument>, AzureIndexDocument>) searchResults).ContinuationToken != null)
          TeamFoundationTracingService.TraceRaw(12060106, TraceLevel.Error, "gallery", nameof (AzureIndexProvider), string.Format("[Warning] Received a continuation token from Azure search. Azure Search can't return all the requested results in a single Search response.  Check caller, query and page size. \nSearch Text: {0} \nSearch Filter: {1} \nPageSize: {2} \nPageNumber: {3}", (object) searchText, string.IsNullOrEmpty(searchParameters1.Filter) ? (object) "" : (object) searchParameters1.Filter, (object) extSearchParams.PageSize, (object) extSearchParams.PageNumber));
        if (values1.Count > 0 && extSearchParams.MetadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludePreCategoryFilterCategories))
        {
          searchParameters2.Select = (IList<string>) new List<string>()
          {
            "ExtensionId"
          };
          searchParameters2.Top = new int?(0);
          searchParameters2.Facets = (IList<string>) new List<string>()
          {
            "Categories,count:55"
          };
          searchResultsForCategoryMetadata = this.m_azureSearchSDKWrapper.Search(searchText, searchParameters2);
        }
        if (values7.Count > 0 && extSearchParams.MetadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeTargetPlatforms))
        {
          searchParameters3.Select = (IList<string>) new List<string>()
          {
            "ExtensionId"
          };
          searchParameters3.Top = new int?(0);
          searchParameters3.Facets = (IList<string>) new List<string>()
          {
            string.Format("{0},count:{1}", (object) "TargetPlatforms", (object) 30)
          };
          searchResultForTargetPlatformMetadata = this.m_azureSearchSDKWrapper.Search(searchText, searchParameters3);
        }
        return this.m_azureSearchConverter.ConvertSearchResultToExtensionQueryResult((object) searchResults, queryFlags, extSearchParams.MetadataFlags, (object) searchResultsForCategoryMetadata, useNewIndexDefinition, (object) searchResultForTargetPlatformMetadata);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRaw(12060106, TraceLevel.Error, "gallery", nameof (AzureIndexProvider), "Search Text: " + searchText + " \nSearch Filter: " + (string.IsNullOrEmpty(searchParameters1.Filter) ? "" : searchParameters1.Filter) + " \nException: " + ex.Message);
        throw;
      }
    }

    private List<string> GetDefaultSelectFields() => new List<string>()
    {
      "DeploymentType",
      "ExtensionDisplayName",
      "ExtensionId",
      "ExtensionName",
      "ExtensionFlags",
      "LastUpdated",
      "PublishedDate",
      "Publisher",
      "ReleasedDate",
      "ShortDescription",
      "ValidatedVersions"
    };

    private void GetValidSortByTypeAndSortOrder(
      int sortByType,
      int sortOrderType,
      bool enableSortByInstallCountUI,
      out string sortBy,
      out string sortOrder)
    {
      Dictionary<SortByType, Tuple<string, string>> dictionary = new Dictionary<SortByType, Tuple<string, string>>()
      {
        {
          SortByType.Author,
          new Tuple<string, string>("PublisherDisplayNameForPrefixMatch", AzureSearchConstants.AscendingSortOrder)
        },
        {
          SortByType.AverageRating,
          new Tuple<string, string>("WeightedRating", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.InstallCount,
          new Tuple<string, string>("DownloadCount", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.LastUpdatedDate,
          new Tuple<string, string>("LastUpdated", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.PublishedDate,
          new Tuple<string, string>("PublishedDate", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.Publisher,
          new Tuple<string, string>("PublisherDisplayNameForPrefixMatch", AzureSearchConstants.AscendingSortOrder)
        },
        {
          SortByType.ReleaseDate,
          new Tuple<string, string>("ReleasedDate", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.Relevance,
          new Tuple<string, string>("", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.Title,
          new Tuple<string, string>("ExtensionDisplayNameForPrefixMatch", AzureSearchConstants.AscendingSortOrder)
        },
        {
          SortByType.TrendingDaily,
          new Tuple<string, string>("TrendingScore", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.TrendingMonthly,
          new Tuple<string, string>("TrendingScore", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.TrendingWeekly,
          new Tuple<string, string>("TrendingScore", AzureSearchConstants.DescendingSortOrder)
        },
        {
          SortByType.WeightedRating,
          new Tuple<string, string>("WeightedRating", AzureSearchConstants.DescendingSortOrder)
        }
      };
      if (enableSortByInstallCountUI)
        dictionary[SortByType.InstallCount] = new Tuple<string, string>("InstallCount", AzureSearchConstants.DescendingSortOrder);
      sortBy = "";
      sortOrder = AzureSearchConstants.DescendingSortOrder;
      if (!Enum.IsDefined(typeof (SortByType), (object) sortByType))
        return;
      Tuple<string, string> tuple = dictionary[(SortByType) sortByType];
      sortBy = tuple.Item1;
      ref string local = ref sortOrder;
      string str;
      switch (sortOrderType)
      {
        case 1:
          str = AzureSearchConstants.AscendingSortOrder;
          break;
        case 2:
          str = AzureSearchConstants.DescendingSortOrder;
          break;
        default:
          str = tuple.Item2;
          break;
      }
      local = str;
    }

    private void SetSearchParametersFilter(
      string filterText,
      string operatorType,
      SearchParameters searchParameters)
    {
      if (!string.IsNullOrEmpty(searchParameters.Filter))
      {
        SearchParameters searchParameters1 = searchParameters;
        searchParameters1.Filter = searchParameters1.Filter + " " + operatorType + " " + filterText;
      }
      else
        searchParameters.Filter = filterText;
    }

    private string GetFlagFilter(string filterText, string operatorType, string existingFilter)
    {
      string flagFilter;
      if (!string.IsNullOrEmpty(existingFilter))
        flagFilter = existingFilter + " " + operatorType + " " + filterText;
      else
        flagFilter = filterText;
      return flagFilter;
    }

    private Version GetParsedVersion(string inputStringVersion)
    {
      Version result1 = (Version) null;
      if (!Version.TryParse(inputStringVersion, out result1))
      {
        int result2;
        if (!int.TryParse(inputStringVersion, out result2))
        {
          TeamFoundationTracingService.TraceRaw(12060106, TraceLevel.Info, "gallery", nameof (AzureIndexProvider), "Invalid installation target version detected. Received value: " + inputStringVersion);
          return result1;
        }
        result1 = new Version(result2, 0);
      }
      return result1;
    }

    private void AddInstallationTargetVersionWithProductArchitectureEntry(
      string target,
      int majorVersion,
      int minorVersion,
      string productArchitecture,
      List<string> installationTargetVersionWithProductArchitecture,
      bool includeInstallationTargetWithoutProductArchitecture)
    {
      if (productArchitecture == "x86")
      {
        installationTargetVersionWithProductArchitecture.Add(string.Format("{0}_{1}.{2}_{3}", (object) target, (object) majorVersion, (object) minorVersion, (object) productArchitecture));
        if (!includeInstallationTargetWithoutProductArchitecture || majorVersion >= 17)
          return;
        installationTargetVersionWithProductArchitecture.Add(string.Format("{0}_{1}.{2}", (object) target, (object) majorVersion, (object) minorVersion));
      }
      else
      {
        if (majorVersion < 17)
          return;
        installationTargetVersionWithProductArchitecture.Add(string.Format("{0}_{1}.{2}_{3}", (object) target, (object) majorVersion, (object) minorVersion, (object) productArchitecture));
      }
    }

    private void AddInstallationTargetVersionWithDefaultProductArchitecture(
      List<string> installationTargetsToUse,
      int majorVersion,
      int minorVersion,
      List<string> installationTargetVersionWithoutProductArchitecture,
      bool includeInstallationTargetWithAndWithoutProductArchitecture)
    {
      if (!includeInstallationTargetWithAndWithoutProductArchitecture)
        return;
      installationTargetVersionWithoutProductArchitecture.AddRange(installationTargetsToUse.Select<string, string>((Func<string, string>) (x => string.Format("{0}_{1}.{2}_{3}", (object) x, (object) majorVersion, (object) minorVersion, (object) "x86"))));
    }
  }
}
