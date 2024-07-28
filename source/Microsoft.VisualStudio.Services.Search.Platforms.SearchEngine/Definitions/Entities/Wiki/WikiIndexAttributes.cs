// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.WikiIndexAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public static class WikiIndexAttributes
  {
    private const int PositionIncrementGapValue = 100;
    private const string WikiIndexRoutingAllocationConstant = "index.routing.allocation.include.entity.wiki";
    private const string ContentAnalyzer = "contentAnalyzer";
    private const string ContentTokenizer = "contentTokenizer";
    private const string CustomTokenFilter = "customtokenfilter";

    public static IndexSettings GetWikiIndexSettings(
      ExecutionContext executionContext,
      int numPrimaries,
      int numReplicas,
      string refreshInterval,
      int localeId = 0)
    {
      WikiIndexAnalyzers wikiIndexAnalyzers = WikiIndexAnalyzersProvider.GetWikiIndexAnalyzers(localeId);
      IndexSettings indexSettings1 = new IndexSettings();
      indexSettings1.RefreshInterval = (Time) refreshInterval;
      indexSettings1.NumberOfReplicas = new int?(numReplicas);
      indexSettings1.NumberOfShards = new int?(numPrimaries);
      indexSettings1.Analysis = (IAnalysis) new Analysis()
      {
        Analyzers = (IAnalyzers) new Analyzers(wikiIndexAnalyzers.AnalyzersMap),
        TokenFilters = wikiIndexAnalyzers.TokenFilters,
        Tokenizers = (ITokenizers) new Tokenizers()
      };
      IndexSettings indexSettings2 = indexSettings1;
      WikiIndexAttributes.AddPatternTokenizerForNon_EnglishCharactesrs(indexSettings2, executionContext.RequestContext);
      if (!executionContext.RequestContext.ExecutionEnvironment.IsDevFabricDeployment && !executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        indexSettings2.Add("index.routing.allocation.include.entity.wiki", (object) true);
      return indexSettings2;
    }

    private static void AddPatternTokenizerForNon_EnglishCharactesrs(
      IndexSettings indexSettings,
      IVssRequestContext requestContext)
    {
      indexSettings.Analysis.Analyzers.Add("contentAnalyzer", (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "contentTokenizer",
        Filter = (IEnumerable<string>) new string[1]
        {
          "customtokenfilter"
        }
      });
      indexSettings.Analysis.Tokenizers.Add("contentTokenizer", (ITokenizer) new PatternTokenizer()
      {
        Pattern = "(\\w+)|([^\\w\\s]?)",
        Group = new int?(0)
      });
      indexSettings.Analysis.TokenFilters.Add("customtokenfilter", (ITokenFilter) new LengthTokenFilter()
      {
        Min = new int?(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MinValueForLengthTokenFilter", 1)),
        Max = new int?(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MinValueForLengthTokenFilter", 20000))
      });
    }

    public static TypeMapping GetWikiIndexMappings(int indexVersion = 0) => WikiIndexAttributes.GetWikiTypeMapping(indexVersion);

    private static TypeMapping GetWikiTypeMapping(int indexVersion)
    {
      TypeMapping wikiTypeMapping = new TypeMapping();
      wikiTypeMapping.Meta = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["version"] = (object) indexVersion
      };
      Properties properties1 = new Properties();
      properties1[(PropertyName) "documentId"] = (IProperty) new KeywordProperty();
      PropertyName key1 = (PropertyName) "item";
      KeywordProperty keywordProperty = new KeywordProperty();
      keywordProperty.Name = (PropertyName) "item";
      keywordProperty.Index = new bool?(false);
      properties1[key1] = (IProperty) keywordProperty;
      properties1[(PropertyName) "contractType"] = (IProperty) new KeywordProperty();
      PropertyName key2 = (PropertyName) "fileNames";
      TextProperty textProperty1 = new TextProperty();
      textProperty1.Analyzer = "stemmedFullTextAnalyzer";
      textProperty1.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty1.PositionIncrementGap = new int?(100);
      Properties properties2 = new Properties();
      properties2[(PropertyName) "unstemmed"] = (IProperty) new TextProperty()
      {
        Analyzer = "unstemmedFullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties2[(PropertyName) "lower"] = (IProperty) new TextProperty()
      {
        Analyzer = "lowercaseWhitespaceAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties2[(PropertyName) "pattern"] = (IProperty) new TextProperty()
      {
        Analyzer = "contentAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      textProperty1.Fields = (IProperties) properties2;
      properties1[key2] = (IProperty) textProperty1;
      PropertyName key3 = (PropertyName) "content";
      TextProperty textProperty2 = new TextProperty();
      textProperty2.Analyzer = "stemmedFullTextAnalyzer";
      textProperty2.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty2.PositionIncrementGap = new int?(100);
      Properties properties3 = new Properties();
      properties3[(PropertyName) "unstemmed"] = (IProperty) new TextProperty()
      {
        Analyzer = "unstemmedFullTextAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      properties3[(PropertyName) "pattern"] = (IProperty) new TextProperty()
      {
        Analyzer = "contentAnalyzer",
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100)
      };
      textProperty2.Fields = (IProperties) properties3;
      properties1[key3] = (IProperty) textProperty2;
      properties1[(PropertyName) "contentMetadata"] = (IProperty) new KeywordProperty();
      PropertyName key4 = (PropertyName) "contentLinks";
      TextProperty textProperty3 = new TextProperty();
      textProperty3.Analyzer = "lightStemmedFullTextAnalyzer";
      textProperty3.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty3.PositionIncrementGap = new int?(100);
      Properties properties4 = new Properties();
      properties4[(PropertyName) "lower"] = (IProperty) new TextProperty()
      {
        Analyzer = "onlyLowerCaseAnalyzer"
      };
      textProperty3.Fields = (IProperties) properties4;
      properties1[key4] = (IProperty) textProperty3;
      properties1[(PropertyName) "filePath"] = (IProperty) new TextProperty()
      {
        Analyzer = "onlyLowerCaseAnalyzer"
      };
      PropertyName key5 = (PropertyName) "collectionName";
      TextProperty textProperty4 = new TextProperty();
      textProperty4.Analyzer = "onlyLowerCaseAnalyzer";
      textProperty4.Norms = new bool?(false);
      Properties properties5 = new Properties();
      properties5[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      properties5[(PropertyName) "search"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "unstemmedFullTextAnalyzer"
      };
      properties5[(PropertyName) "searchwithcamelcasedelimiter"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "unstemmedFullTextWithCamelCaseAnalyzer"
      };
      textProperty4.Fields = (IProperties) properties5;
      properties1[key5] = (IProperty) textProperty4;
      properties1[(PropertyName) "collectionUrl"] = (IProperty) new KeywordProperty();
      PropertyName key6 = (PropertyName) "projectName";
      TextProperty textProperty5 = new TextProperty();
      textProperty5.Analyzer = "onlyLowerCaseAnalyzer";
      textProperty5.Norms = new bool?(false);
      Properties properties6 = new Properties();
      properties6[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      properties6[(PropertyName) "search"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "unstemmedFullTextAnalyzer"
      };
      properties6[(PropertyName) "searchwithcamelcasedelimiter"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "unstemmedFullTextWithCamelCaseAnalyzer"
      };
      textProperty5.Fields = (IProperties) properties6;
      properties1[key6] = (IProperty) textProperty5;
      PropertyName key7 = (PropertyName) "wikiName";
      TextProperty textProperty6 = new TextProperty();
      textProperty6.Analyzer = "onlyLowerCaseAnalyzer";
      textProperty6.Norms = new bool?(false);
      Properties properties7 = new Properties();
      properties7[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      properties7[(PropertyName) "search"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        Analyzer = "unstemmedFullTextAnalyzer"
      };
      textProperty6.Fields = (IProperties) properties7;
      properties1[key7] = (IProperty) textProperty6;
      properties1[(PropertyName) "projectVisibility"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "repoName"] = (IProperty) new TextProperty()
      {
        Analyzer = "onlyLowerCaseAnalyzer"
      };
      PropertyName key8 = (PropertyName) "branchName";
      TextProperty textProperty7 = new TextProperty();
      textProperty7.Analyzer = "onlyLowerCaseAnalyzer";
      textProperty7.Norms = new bool?(false);
      Properties properties8 = new Properties();
      properties8[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      textProperty7.Fields = (IProperties) properties8;
      properties1[key8] = (IProperty) textProperty7;
      PropertyName key9 = (PropertyName) "tags";
      TextProperty textProperty8 = new TextProperty();
      textProperty8.Analyzer = "unstemmedFullTextAnalyzer";
      textProperty8.IndexOptions = new IndexOptions?(IndexOptions.Offsets);
      textProperty8.PositionIncrementGap = new int?(100);
      Properties properties9 = new Properties();
      properties9[(PropertyName) "raw"] = (IProperty) new KeywordProperty();
      properties9[(PropertyName) "lower"] = (IProperty) new TextProperty()
      {
        Analyzer = "onlyLowerCaseAnalyzer"
      };
      properties9[(PropertyName) "searchwithcamelcasedelimiter"] = (IProperty) new TextProperty()
      {
        IndexOptions = new IndexOptions?(IndexOptions.Offsets),
        PositionIncrementGap = new int?(100),
        Analyzer = "unstemmedFullTextWithCamelCaseAnalyzer"
      };
      textProperty8.Fields = (IProperties) properties9;
      properties1[key9] = (IProperty) textProperty8;
      properties1[(PropertyName) "isDefaultBranch"] = (IProperty) new BooleanProperty();
      properties1[(PropertyName) "fileExtension"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "fileExtensionId"] = (IProperty) new NumberProperty(NumberType.Float);
      properties1[(PropertyName) "collectionId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "organizationId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "projectId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "repositoryId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "wikiId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "mappedPath"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "contentId"] = (IProperty) new KeywordProperty();
      properties1[(PropertyName) "lastUpdated"] = (IProperty) new DateProperty()
      {
        Format = "strict_date_optional_time"
      };
      properties1[(PropertyName) "indexedTimeStamp"] = (IProperty) new DateProperty()
      {
        Format = "epoch_second"
      };
      wikiTypeMapping.Properties = (IProperties) properties1;
      return wikiTypeMapping;
    }
  }
}
