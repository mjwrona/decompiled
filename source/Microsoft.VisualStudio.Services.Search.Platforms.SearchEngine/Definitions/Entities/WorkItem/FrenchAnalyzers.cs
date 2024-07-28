// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.FrenchAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class FrenchAnalyzers : WorkItemIndexAnalyzers
  {
    protected override WorkItemIndexAnalyzers.WorkItemIndexAnalysis GetAnalyzers()
    {
      WorkItemIndexAnalyzers.WorkItemIndexAnalysis analyzers = new WorkItemIndexAnalyzers.WorkItemIndexAnalysis();
      WorkItemIndexAnalyzers.WorkItemIndexAnalysis itemIndexAnalysis = analyzers;
      Nest.TokenFilters tokenFilters = new Nest.TokenFilters();
      tokenFilters["lightLanguageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "light_french"
      };
      tokenFilters["minimalLanguageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "minimal_french"
      };
      tokenFilters["languageStopWords"] = (ITokenFilter) new StopTokenFilter()
      {
        StopWords = new StopWords("_french_")
      };
      itemIndexAnalysis.TokenFilters = (ITokenFilters) tokenFilters;
      analyzers.LightStemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[3]
        {
          "lowercase",
          "languageStopWords",
          "lightLanguageStemmer"
        }
      };
      analyzers.MinimallyStemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[3]
        {
          "lowercase",
          "languageStopWords",
          "minimalLanguageStemmer"
        }
      };
      return analyzers;
    }
  }
}
