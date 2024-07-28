// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting.SettingIndexAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting
{
  public static class SettingIndexAttributes
  {
    private const string StandardTokenizer = "standard";
    private const string AsciiFoldingFilter = "asciifolding";
    private const string LightEnglishStemmerFilter = "lightEnglishStemmer";
    private const string MinimalEnglishStemmerFilter = "minimalEnglishStemmer";
    private const string LowercaseFilter = "lowercase";
    private const string FullTextAnalyzer = "fullTextAnalyzer";
    private static readonly string[] s_fullTextTokenFilters = new string[3]
    {
      "asciifolding",
      "lowercase",
      "lightEnglishStemmer"
    };

    public static IndexSettings GetSettingIndexSettings(
      int numPrimaries,
      int numReplicas,
      string refreshInterval)
    {
      IndexSettings settingIndexSettings = new IndexSettings();
      settingIndexSettings.RefreshInterval = (Time) refreshInterval;
      settingIndexSettings.NumberOfReplicas = new int?(numReplicas);
      settingIndexSettings.NumberOfShards = new int?(numPrimaries);
      Analysis analysis = new Analysis();
      Analyzers analyzers = new Analyzers();
      analyzers["fullTextAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) SettingIndexAttributes.s_fullTextTokenFilters
      };
      analysis.Analyzers = (IAnalyzers) analyzers;
      TokenFilters tokenFilters = new TokenFilters();
      tokenFilters["lightEnglishStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "light_english"
      };
      tokenFilters["minimalEnglishStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "minimal_english"
      };
      analysis.TokenFilters = (ITokenFilters) tokenFilters;
      settingIndexSettings.Analysis = (IAnalysis) analysis;
      return settingIndexSettings;
    }

    public static ITypeMapping GetSettingIndexMappings(int indexVersion = 0)
    {
      TypeMapping settingIndexMappings = new TypeMapping();
      settingIndexMappings.Meta = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["version"] = (object) indexVersion
      };
      settingIndexMappings.DateDetection = new bool?(false);
      Properties properties = new Properties();
      PropertyName key = (PropertyName) "item";
      KeywordProperty keywordProperty = new KeywordProperty();
      keywordProperty.Name = (PropertyName) "item";
      keywordProperty.Index = new bool?(false);
      properties[key] = (IProperty) keywordProperty;
      properties[(PropertyName) "contractType"] = (IProperty) new KeywordProperty();
      properties[(PropertyName) "indexedTimeStamp"] = (IProperty) new DateProperty()
      {
        Format = "epoch_second"
      };
      properties[(PropertyName) "title"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Positions)
      };
      properties[(PropertyName) "description"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Positions)
      };
      properties[(PropertyName) "routeId"] = (IProperty) new KeywordProperty();
      properties[(PropertyName) "routeParameterMapping"] = (IProperty) new KeywordProperty();
      properties[(PropertyName) "scope"] = (IProperty) new KeywordProperty();
      properties[(PropertyName) "tags"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Positions)
      };
      properties[(PropertyName) "icon"] = (IProperty) new KeywordProperty();
      settingIndexMappings.Properties = (IProperties) properties;
      return (ITypeMapping) settingIndexMappings;
    }

    internal static class FieldNameSuffix
    {
      public const string Raw = "raw";
      public const string Lower = "lower";
      public const string Stemmed = "stemmed";
      public const string CaseChangeAnalyzed = "casechangeanalyzed";
    }
  }
}
