// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package.PackageIndexAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package
{
  public static class PackageIndexAttributes
  {
    private const string StandardTokenizer = "standard";
    private const string KeywordTokenizer = "keyword";
    private const string WhitespaceTokenizer = "whitespace";
    private const string AsciiFoldingFilter = "asciifolding";
    private const string FullTextWordDelimiterFilter = "fullTextWordDelimiter";
    private const string FullTextWithCamelCaseWordDelimiterFilter = "advancedFullTextWordDelimiter";
    private const string VersionDelimiterFilter = "versionDelimiterFilter";
    private const string PackageNameDelimiterFilter = "packageNameDelimiterFilter";
    private const string LightEnglishStemmerFilter = "lightEnglishStemmer";
    private const string MinimalEnglishStemmerFilter = "minimalEnglishStemmer";
    private const string LowercaseFilter = "lowercase";
    private const string FullTextAnalyzer = "fullTextAnalyzer";
    private const string MinimallyStemmedFullTextAnalyzer = "minimallyStemmedFullTextAnalyzer";
    private const string UnstemmedFullTextAnalyzer = "unstemmedFullTextAnalyzer";
    private const string UnstemmedFullTextWithCamelCaseAnalyzer = "unstemmedFullTextWithCamelCaseAnalyzer";
    private const string OnlyLowerCaseAnalyzer = "onlyLowerCaseAnalyzer";
    private const string VersionAnalyzer = "versionAnalyzer";
    private const string PackageNameAnalyzer = "packageNameAnalyzer";
    private const string PackageIndexRoutingAllocationConstant = "index.routing.allocation.include.entity.package";
    private static readonly string[] s_fullTextTokenFilters = new string[4]
    {
      "asciifolding",
      "fullTextWordDelimiter",
      "lowercase",
      "lightEnglishStemmer"
    };
    private static readonly string[] s_versionTokenFilters = new string[1]
    {
      "versionDelimiterFilter"
    };
    private static readonly string[] s_lightStemmedPackageNameTokenFilters = new string[4]
    {
      "asciifolding",
      "packageNameDelimiterFilter",
      "lowercase",
      "lightEnglishStemmer"
    };
    private static readonly string[] s_minimallyStemmedFullTextTokenFilters = new string[4]
    {
      "asciifolding",
      "fullTextWordDelimiter",
      "lowercase",
      "minimalEnglishStemmer"
    };
    private static readonly string[] s_unstemmedFullTextTokenFilters = new string[3]
    {
      "asciifolding",
      "fullTextWordDelimiter",
      "lowercase"
    };
    private static readonly string[] s_unstemmedFullTextwithCamelCaseTokenFilters = new string[3]
    {
      "asciifolding",
      "advancedFullTextWordDelimiter",
      "lowercase"
    };
    private const int PositionIncrementGapValue = 100;

    public static IndexSettings GetPackageIndexSettings(
      ExecutionContext executionContext,
      int numPrimaries,
      int numReplicas,
      string refreshInterval)
    {
      IndexSettings indexSettings1 = new IndexSettings();
      indexSettings1.RefreshInterval = (Time) refreshInterval;
      indexSettings1.NumberOfReplicas = new int?(numReplicas);
      indexSettings1.NumberOfShards = new int?(numPrimaries);
      IndexSettings indexSettings2 = indexSettings1;
      Analysis analysis1 = new Analysis();
      Analysis analysis2 = analysis1;
      Analyzers analyzers1 = new Analyzers();
      analyzers1["onlyLowerCaseAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "keyword",
        Filter = (IEnumerable<string>) new string[1]
        {
          "lowercase"
        }
      };
      analyzers1["fullTextAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) PackageIndexAttributes.s_fullTextTokenFilters
      };
      analyzers1["minimallyStemmedFullTextAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) PackageIndexAttributes.s_minimallyStemmedFullTextTokenFilters
      };
      analyzers1["unstemmedFullTextAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) PackageIndexAttributes.s_unstemmedFullTextTokenFilters
      };
      analyzers1["versionAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) PackageIndexAttributes.s_versionTokenFilters
      };
      analyzers1["packageNameAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) PackageIndexAttributes.s_lightStemmedPackageNameTokenFilters
      };
      analyzers1["unstemmedFullTextWithCamelCaseAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "whitespace",
        Filter = (IEnumerable<string>) PackageIndexAttributes.s_unstemmedFullTextwithCamelCaseTokenFilters
      };
      Analyzers analyzers2 = analyzers1;
      analysis2.Analyzers = (IAnalyzers) analyzers2;
      Analysis analysis3 = analysis1;
      TokenFilters tokenFilters = new TokenFilters();
      tokenFilters["lightEnglishStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "light_english"
      };
      tokenFilters["minimalEnglishStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "minimal_english"
      };
      tokenFilters["fullTextWordDelimiter"] = (ITokenFilter) new WordDelimiterTokenFilter()
      {
        GenerateWordParts = new bool?(true),
        GenerateNumberParts = new bool?(true),
        CatenateWords = new bool?(false),
        CatenateNumbers = new bool?(false),
        CatenateAll = new bool?(false),
        SplitOnCaseChange = new bool?(false),
        PreserveOriginal = new bool?(false),
        SplitOnNumerics = new bool?(false),
        StemEnglishPossessive = new bool?(true)
      };
      tokenFilters["packageNameDelimiterFilter"] = (ITokenFilter) new WordDelimiterTokenFilter()
      {
        GenerateWordParts = new bool?(true),
        GenerateNumberParts = new bool?(true),
        CatenateWords = new bool?(false),
        CatenateNumbers = new bool?(false),
        CatenateAll = new bool?(false),
        SplitOnCaseChange = new bool?(false),
        PreserveOriginal = new bool?(false),
        SplitOnNumerics = new bool?(false),
        StemEnglishPossessive = new bool?(true)
      };
      tokenFilters["advancedFullTextWordDelimiter"] = (ITokenFilter) new WordDelimiterTokenFilter()
      {
        GenerateWordParts = new bool?(true),
        GenerateNumberParts = new bool?(true),
        CatenateWords = new bool?(false),
        CatenateNumbers = new bool?(false),
        CatenateAll = new bool?(false),
        SplitOnCaseChange = new bool?(true),
        PreserveOriginal = new bool?(false),
        SplitOnNumerics = new bool?(true),
        StemEnglishPossessive = new bool?(true)
      };
      tokenFilters["versionDelimiterFilter"] = (ITokenFilter) new WordDelimiterTokenFilter()
      {
        GenerateWordParts = new bool?(true),
        GenerateNumberParts = new bool?(true),
        CatenateWords = new bool?(false),
        CatenateNumbers = new bool?(false),
        CatenateAll = new bool?(false),
        SplitOnCaseChange = new bool?(false),
        PreserveOriginal = new bool?(false),
        SplitOnNumerics = new bool?(true),
        StemEnglishPossessive = new bool?(false)
      };
      analysis3.TokenFilters = (ITokenFilters) tokenFilters;
      Analysis analysis4 = analysis1;
      indexSettings2.Analysis = (IAnalysis) analysis4;
      IndexSettings packageIndexSettings = indexSettings1;
      if (!executionContext.RequestContext.ExecutionEnvironment.IsDevFabricDeployment && !executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        packageIndexSettings.Add("index.routing.allocation.include.entity.package", (object) true);
      return packageIndexSettings;
    }

    public static ITypeMapping GetPackageIndexMappings(int indexVersion = 0)
    {
      TypeMapping packageIndexMappings = new TypeMapping();
      packageIndexMappings.Meta = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["version"] = (object) indexVersion
      };
      packageIndexMappings.DateDetection = new bool?(false);
      Properties properties1 = new Properties();
      properties1[(PropertyName) "documentId"] = (IProperty) new KeywordProperty();
      PropertyName key1 = (PropertyName) "item";
      KeywordProperty keywordProperty1 = new KeywordProperty();
      keywordProperty1.Name = (PropertyName) "item";
      keywordProperty1.Index = new bool?(false);
      properties1[key1] = (IProperty) keywordProperty1;
      properties1[(PropertyName) "contractType"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "indexedTimeStamp"] = (IProperty) new DateProperty()
      {
        Format = "epoch_second"
      };
      properties1[(PropertyName) "organizationId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "organizationName"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "organizationNameOriginal"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "collectionId"] = (IProperty) new KeywordProperty();
      PropertyName key2 = (PropertyName) "collectionName";
      TextProperty textProperty1 = new TextProperty();
      textProperty1.Analyzer = "onlyLowerCaseAnalyzer";
      textProperty1.Norms = new bool?(false);
      Properties properties2 = new Properties();
      properties2[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      textProperty1.Fields = (IProperties) properties2;
      properties1[key2] = (IProperty) textProperty1;
      properties1[(PropertyName) "collectionNameOriginal"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "collectionUrl"] = (IProperty) new KeywordProperty();
      PropertyName key3 = (PropertyName) "name";
      TextProperty textProperty2 = new TextProperty();
      textProperty2.Analyzer = "packageNameAnalyzer";
      textProperty2.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty2.PositionIncrementGap = new int?(100);
      Properties properties3 = new Properties();
      properties3[(PropertyName) "casechangeanalyzed"] = (IProperty) new TextProperty()
      {
        Analyzer = "unstemmedFullTextWithCamelCaseAnalyzer",
        PositionIncrementGap = new int?(100)
      };
      textProperty2.Fields = (IProperties) properties3;
      properties1[key3] = (IProperty) textProperty2;
      PropertyName key4 = (PropertyName) "normalizedName";
      KeywordProperty keywordProperty2 = new KeywordProperty();
      keywordProperty2.DocValues = new bool?(true);
      properties1[key4] = (IProperty) keywordProperty2;
      properties1[(PropertyName) "description"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      PropertyName key5 = (PropertyName) "protocol";
      TextProperty textProperty3 = new TextProperty();
      textProperty3.Analyzer = "unstemmedFullTextAnalyzer";
      textProperty3.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty3.PositionIncrementGap = new int?(100);
      Properties properties4 = new Properties();
      properties4[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      properties4[(PropertyName) "lower"] = (IProperty) new TextProperty()
      {
        PositionIncrementGap = new int?(100),
        Analyzer = "onlyLowerCaseAnalyzer",
        Norms = new bool?(false)
      };
      textProperty3.Fields = (IProperties) properties4;
      properties1[key5] = (IProperty) textProperty3;
      properties1[(PropertyName) "author"] = (IProperty) new TextProperty()
      {
        Analyzer = "onlyLowerCaseAnalyzer"
      };
      properties1[(PropertyName) "version"] = (IProperty) new TextProperty()
      {
        Analyzer = "versionAnalyzer"
      };
      properties1[(PropertyName) "normalizedVersion"] = (IProperty) new TextProperty()
      {
        Analyzer = "versionAnalyzer"
      };
      PropertyName key6 = (PropertyName) "sortableVersion";
      KeywordProperty keywordProperty3 = new KeywordProperty();
      keywordProperty3.DocValues = new bool?(true);
      properties1[key6] = (IProperty) keywordProperty3;
      properties1[(PropertyName) "feedId"] = (IProperty) new KeywordProperty();
      PropertyName key7 = (PropertyName) "feedName";
      TextProperty textProperty4 = new TextProperty();
      textProperty4.Analyzer = "unstemmedFullTextAnalyzer";
      textProperty4.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty4.PositionIncrementGap = new int?(100);
      Properties properties5 = new Properties();
      properties5[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      properties5[(PropertyName) "lower"] = (IProperty) new TextProperty()
      {
        PositionIncrementGap = new int?(100),
        Analyzer = "onlyLowerCaseAnalyzer",
        Norms = new bool?(false)
      };
      textProperty4.Fields = (IProperties) properties5;
      properties1[key7] = (IProperty) textProperty4;
      properties1[(PropertyName) "versionId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "packageId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "publishDate"] = (IProperty) new DateProperty()
      {
        Format = "strict_date_optional_time"
      };
      PropertyName key8 = (PropertyName) "views";
      ObjectProperty objectProperty = new ObjectProperty();
      Properties properties6 = new Properties();
      properties6[(PropertyName) "viewName"] = (IProperty) new KeywordProperty();
      properties6[(PropertyName) "viewNameOriginal"] = (IProperty) new KeywordProperty();
      properties6[(PropertyName) "viewId"] = (IProperty) new KeywordProperty();
      properties6[(PropertyName) "viewUrl"] = (IProperty) new KeywordProperty();
      objectProperty.Properties = (IProperties) properties6;
      properties1[key8] = (IProperty) objectProperty;
      properties1[(PropertyName) "tags"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "isListed"] = (IProperty) new BooleanProperty();
      packageIndexMappings.Properties = (IProperties) properties1;
      return (ITypeMapping) packageIndexMappings;
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
