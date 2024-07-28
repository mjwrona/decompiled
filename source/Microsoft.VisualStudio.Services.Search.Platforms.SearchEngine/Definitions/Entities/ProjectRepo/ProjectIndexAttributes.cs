// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.ProjectIndexAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Nest;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public static class ProjectIndexAttributes
  {
    private const string StandardTokenizer = "standard";
    private const string KeywordTokenizer = "keyword";
    private const string WhitespaceTokenizer = "whitespace";
    private const string AsciiFoldingFilter = "asciifolding";
    private const string FullTextWordDelimiterFilter = "fullTextWordDelimiter";
    private const string FullTextwithCamelCaseWordDelimiterFilter = "advancedFullTextWordDelimiter";
    private const string ShingleFilter = "shingleFilter";
    private const string LightEnglishStemmerFilter = "lightEnglishStemmer";
    private const string MinimalEnglishStemmerFilter = "minimalEnglishStemmer";
    private const string LowercaseFilter = "lowercase";
    private const string EnglishStemmerFilter = "englishStemmer";
    private const string FullTextAnalyzer = "fullTextAnalyzer";
    private const string UnstemmedFullTextAnalyzer = "unstemmedFullTextAnalyzer";
    private const string UnstemmedFullTextWithCamelCaseShingleAnalyzer = "unstemmedFullTextWithCamelCaseShingleAnalyzer";
    private const string OnlyLowerCaseAnalyzer = "onlyLowerCaseAnalyzer";
    private const string ProjectIndexRoutingAllocationConstant = "index.routing.allocation.include.entity.projectrepo";
    private static readonly string[] s_fullTextTokenFilters = new string[4]
    {
      "asciifolding",
      "fullTextWordDelimiter",
      "lowercase",
      "englishStemmer"
    };
    private static readonly string[] s_unstemmedFullTextTokenFilters = new string[3]
    {
      "asciifolding",
      "fullTextWordDelimiter",
      "lowercase"
    };
    private static readonly string[] s_unstemmedFullTextwithCamelCaseShingleTokenFilters = new string[4]
    {
      "asciifolding",
      "advancedFullTextWordDelimiter",
      "shingleFilter",
      "lowercase"
    };
    private const int PositionIncrementGapValue = 100;

    public static IndexSettings GetProjectIndexSettings(
      IVssRequestContext requestContext,
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
        Filter = (IEnumerable<string>) ProjectIndexAttributes.s_fullTextTokenFilters
      };
      analyzers1["unstemmedFullTextAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) ProjectIndexAttributes.s_unstemmedFullTextTokenFilters
      };
      analyzers1["unstemmedFullTextWithCamelCaseShingleAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "whitespace",
        Filter = (IEnumerable<string>) ProjectIndexAttributes.s_unstemmedFullTextwithCamelCaseShingleTokenFilters
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
      tokenFilters["shingleFilter"] = (ITokenFilter) new ShingleTokenFilter()
      {
        TokenSeparator = string.Empty,
        MaxShingleSize = new int?(4),
        MinShingleSize = new int?(2)
      };
      tokenFilters["englishStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "english"
      };
      analysis3.TokenFilters = (ITokenFilters) tokenFilters;
      Analysis analysis4 = analysis1;
      indexSettings2.Analysis = (IAnalysis) analysis4;
      IndexSettings projectIndexSettings = indexSettings1;
      if (!requestContext.ExecutionEnvironment.IsDevFabricDeployment && !requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        projectIndexSettings.Add("index.routing.allocation.include.entity.projectrepo", (object) true);
      return projectIndexSettings;
    }

    public static ITypeMapping GetProjectIndexMappings(int indexVersion = 0)
    {
      TypeMapping projectIndexMappings = new TypeMapping();
      projectIndexMappings.Meta = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["version"] = (object) indexVersion
      };
      projectIndexMappings.DateDetection = new bool?(false);
      projectIndexMappings.RoutingField = (IRoutingField) new RoutingField()
      {
        Required = new bool?(true)
      };
      Properties properties1 = new Properties();
      properties1[(PropertyName) "documentId"] = (IProperty) new KeywordProperty();
      PropertyName key1 = (PropertyName) "item";
      KeywordProperty keywordProperty = new KeywordProperty();
      keywordProperty.Name = (PropertyName) "item";
      keywordProperty.Index = new bool?(false);
      properties1[key1] = (IProperty) keywordProperty;
      properties1[(PropertyName) "joinField"] = (IProperty) new JoinProperty()
      {
        Relations = (IRelations) new Relations()
        {
          {
            (RelationName) "ProjectContract",
            (RelationName) "RepositoryContract",
            Array.Empty<RelationName>()
          }
        }
      };
      properties1[(PropertyName) "contractType"] = (IProperty) new KeywordProperty();
      PropertyName key2 = (PropertyName) "name";
      TextProperty textProperty1 = new TextProperty();
      textProperty1.Analyzer = "unstemmedFullTextAnalyzer";
      textProperty1.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty1.PositionIncrementGap = new int?(100);
      Properties properties2 = new Properties();
      properties2[(PropertyName) "casechangeanalyzed"] = (IProperty) new TextProperty()
      {
        Analyzer = "unstemmedFullTextWithCamelCaseShingleAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties2[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      textProperty1.Fields = (IProperties) properties2;
      properties1[key2] = (IProperty) textProperty1;
      PropertyName key3 = (PropertyName) "languages";
      TextProperty textProperty2 = new TextProperty();
      textProperty2.PositionIncrementGap = new int?(100);
      textProperty2.Analyzer = "onlyLowerCaseAnalyzer";
      textProperty2.Norms = new bool?(false);
      Properties properties3 = new Properties();
      properties3[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      properties3[(PropertyName) "analysed"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100),
        Analyzer = "unstemmedFullTextAnalyzer"
      };
      textProperty2.Fields = (IProperties) properties3;
      properties1[key3] = (IProperty) textProperty2;
      PropertyName key4 = (PropertyName) "activityStats";
      ObjectProperty objectProperty = new ObjectProperty();
      objectProperty.Enabled = new bool?(false);
      Properties properties4 = new Properties();
      properties4[(PropertyName) "activityDate"] = (IProperty) new DateProperty()
      {
        Format = "strict_date_optional_time"
      };
      properties4[(PropertyName) "activityValue"] = (IProperty) new NumberProperty(NumberType.Integer);
      objectProperty.Properties = (IProperties) properties4;
      properties1[key4] = (IProperty) objectProperty;
      properties1[(PropertyName) "activityCount1day"] = (IProperty) new NumberProperty(NumberType.Integer);
      properties1[(PropertyName) "activityCount7day"] = (IProperty) new NumberProperty(NumberType.Integer);
      properties1[(PropertyName) "activityCount30days"] = (IProperty) new NumberProperty(NumberType.Integer);
      properties1[(PropertyName) "visibility"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "url"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "collectionId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "collectionNameOriginal"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "collectionName"] = (IProperty) new KeywordProperty();
      PropertyName key5 = (PropertyName) "collectionNameAnalyzed";
      TextProperty textProperty3 = new TextProperty();
      textProperty3.Analyzer = "unstemmedFullTextAnalyzer";
      textProperty3.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty3.PositionIncrementGap = new int?(100);
      Properties properties5 = new Properties();
      properties5[(PropertyName) "casechangeanalyzed"] = (IProperty) new TextProperty()
      {
        Analyzer = "unstemmedFullTextWithCamelCaseShingleAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      textProperty3.Fields = (IProperties) properties5;
      properties1[key5] = (IProperty) textProperty3;
      properties1[(PropertyName) "organizationId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "organizationNameOriginal"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "organizationName"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "indexedTimeStamp"] = (IProperty) new DateProperty()
      {
        Format = "epoch_second"
      };
      properties1[(PropertyName) "description"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties1[(PropertyName) "descriptionMetadata"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "likesCount"] = (IProperty) new NumberProperty(NumberType.Integer);
      PropertyName key6 = (PropertyName) "tags";
      TextProperty textProperty4 = new TextProperty();
      textProperty4.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty4.PositionIncrementGap = new int?(100);
      textProperty4.Analyzer = "unstemmedFullTextAnalyzer";
      Properties properties6 = new Properties();
      properties6[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      properties6[(PropertyName) "lower"] = (IProperty) new TextProperty()
      {
        PositionIncrementGap = new int?(100),
        Analyzer = "onlyLowerCaseAnalyzer",
        Norms = new bool?(false)
      };
      properties6[(PropertyName) "casechangeanalyzed"] = (IProperty) new TextProperty()
      {
        Analyzer = "unstemmedFullTextWithCamelCaseShingleAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      textProperty4.Fields = (IProperties) properties6;
      properties1[key6] = (IProperty) textProperty4;
      properties1[(PropertyName) "TrendFactor1Day"] = (IProperty) new NumberProperty(NumberType.Integer);
      properties1[(PropertyName) "TrendFactor7Days"] = (IProperty) new NumberProperty(NumberType.Integer);
      properties1[(PropertyName) "TrendFactor30Days"] = (IProperty) new NumberProperty(NumberType.Integer);
      properties1[(PropertyName) "lastUpdated"] = (IProperty) new DateProperty()
      {
        Format = "strict_date_optional_time"
      };
      properties1[(PropertyName) "imageUrl"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "parentDocumentId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "readme"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties1[(PropertyName) "readmeMetadata"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "readmeLinks"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties1[(PropertyName) "readmeFilePath"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "forks"] = (IProperty) new NumberProperty(NumberType.Integer);
      PropertyName key7 = (PropertyName) "projectName";
      TextProperty textProperty5 = new TextProperty();
      textProperty5.Analyzer = "unstemmedFullTextAnalyzer";
      textProperty5.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty5.PositionIncrementGap = new int?(100);
      Properties properties7 = new Properties();
      properties7[(PropertyName) "casechangeanalyzed"] = (IProperty) new TextProperty()
      {
        Analyzer = "unstemmedFullTextWithCamelCaseShingleAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      textProperty5.Fields = (IProperties) properties7;
      properties1[key7] = (IProperty) textProperty5;
      properties1[(PropertyName) "versionControl"] = (IProperty) new KeywordProperty();
      projectIndexMappings.Properties = (IProperties) properties1;
      return (ITypeMapping) projectIndexMappings;
    }

    internal static class FieldNameSuffix
    {
      public const string Raw = "raw";
      public const string Lower = "lower";
      public const string Stemmed = "stemmed";
      public const string Analyzed = "analysed";
      public const string CaseChangeAnalyzed = "casechangeanalyzed";
    }
  }
}
