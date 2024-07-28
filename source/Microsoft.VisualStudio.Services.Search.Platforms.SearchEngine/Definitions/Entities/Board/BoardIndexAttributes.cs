// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board.BoardIndexAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board
{
  public static class BoardIndexAttributes
  {
    private const string StandardTokenizer = "standard";
    private const string AsciiFoldingFilter = "asciifolding";
    private const string LightEnglishStemmerFilter = "lightEnglishStemmer";
    private const string MinimalEnglishStemmerFilter = "minimalEnglishStemmer";
    private const string LowercaseFilter = "lowercase";
    private const string FullTextAnalyzer = "fullTextAnalyzer";
    private const string BoardIndexRoutingAllocationConstant = "index.routing.allocation.include.entity.board";
    private static readonly string[] s_fullTextTokenFilters = new string[3]
    {
      "asciifolding",
      "lowercase",
      "lightEnglishStemmer"
    };
    private const int PositionIncrementGapValue = 100;

    public static IndexSettings GetBoardIndexSettings(
      ExecutionContext executionContext,
      int numPrimaries,
      int numReplicas,
      string refreshInterval)
    {
      IndexSettings indexSettings = new IndexSettings();
      indexSettings.RefreshInterval = (Time) refreshInterval;
      indexSettings.NumberOfReplicas = new int?(numReplicas);
      indexSettings.NumberOfShards = new int?(numPrimaries);
      Analysis analysis = new Analysis();
      Analyzers analyzers = new Analyzers();
      analyzers["fullTextAnalyzer"] = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) BoardIndexAttributes.s_fullTextTokenFilters
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
      indexSettings.Analysis = (IAnalysis) analysis;
      IndexSettings boardIndexSettings = indexSettings;
      if (!executionContext.RequestContext.ExecutionEnvironment.IsDevFabricDeployment && !executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        boardIndexSettings.Add("index.routing.allocation.include.entity.board", (object) true);
      return boardIndexSettings;
    }

    public static ITypeMapping GetBoardIndexMappings(int indexVersion = 0)
    {
      TypeMapping boardIndexMappings = new TypeMapping();
      boardIndexMappings.Meta = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["version"] = (object) indexVersion
      };
      boardIndexMappings.DateDetection = new bool?(false);
      Properties properties1 = new Properties();
      properties1[(PropertyName) "documentId"] = (IProperty) new KeywordProperty();
      PropertyName key1 = (PropertyName) "item";
      KeywordProperty keywordProperty = new KeywordProperty();
      keywordProperty.Name = (PropertyName) "item";
      keywordProperty.Index = new bool?(false);
      properties1[key1] = (IProperty) keywordProperty;
      properties1[(PropertyName) "contractType"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "indexedTimeStamp"] = (IProperty) new DateProperty()
      {
        Format = "epoch_second"
      };
      properties1[(PropertyName) "collectionId"] = (IProperty) new KeywordProperty();
      PropertyName key2 = (PropertyName) "collectionName";
      TextProperty textProperty = new TextProperty();
      textProperty.Analyzer = "fullTextAnalyzer";
      textProperty.Norms = new bool?(false);
      Properties properties2 = new Properties();
      properties2[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      textProperty.Fields = (IProperties) properties2;
      properties1[key2] = (IProperty) textProperty;
      properties1[(PropertyName) "projectId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "projectName"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties1[(PropertyName) "teamId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "teamName"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties1[(PropertyName) "description"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties1[(PropertyName) "boardType"] = (IProperty) new TextProperty()
      {
        Analyzer = "fullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      boardIndexMappings.Properties = (IProperties) properties1;
      return (ITypeMapping) boardIndexMappings;
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
