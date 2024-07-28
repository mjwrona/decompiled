// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.GermanAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class GermanAnalyzers : WorkItemIndexAnalyzers
  {
    private const string GermanNormalizationFilter = "german_normalization";

    protected override WorkItemIndexAnalyzers.WorkItemIndexAnalysis GetAnalyzers()
    {
      WorkItemIndexAnalyzers.WorkItemIndexAnalysis analyzers = new WorkItemIndexAnalyzers.WorkItemIndexAnalysis();
      WorkItemIndexAnalyzers.WorkItemIndexAnalysis itemIndexAnalysis = analyzers;
      Nest.TokenFilters tokenFilters = new Nest.TokenFilters();
      tokenFilters["lightLanguageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "light_german"
      };
      tokenFilters["minimalLanguageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "minimal_german"
      };
      tokenFilters["languageStopWords"] = (ITokenFilter) new StopTokenFilter()
      {
        StopWords = new StopWords("_german_")
      };
      itemIndexAnalysis.TokenFilters = (ITokenFilters) tokenFilters;
      analyzers.LightStemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[4]
        {
          "lowercase",
          "languageStopWords",
          "german_normalization",
          "lightLanguageStemmer"
        }
      };
      analyzers.MinimallyStemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[4]
        {
          "lowercase",
          "languageStopWords",
          "german_normalization",
          "minimalLanguageStemmer"
        }
      };
      return analyzers;
    }
  }
}
