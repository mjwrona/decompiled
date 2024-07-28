// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.CjkAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class CjkAnalyzers : WorkItemIndexAnalyzers
  {
    protected override WorkItemIndexAnalyzers.WorkItemIndexAnalysis GetAnalyzers()
    {
      CustomAnalyzer customAnalyzer = new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[4]
        {
          "cjk_width",
          "lowercase",
          "cjk_bigram",
          "languageStopWords"
        }
      };
      WorkItemIndexAnalyzers.WorkItemIndexAnalysis analyzers = new WorkItemIndexAnalyzers.WorkItemIndexAnalysis();
      Nest.TokenFilters tokenFilters = new Nest.TokenFilters();
      tokenFilters["languageStopWords"] = (ITokenFilter) new StopTokenFilter()
      {
        StopWords = new StopWords("_english_")
      };
      analyzers.TokenFilters = (ITokenFilters) tokenFilters;
      analyzers.LightStemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer;
      analyzers.MinimallyStemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer;
      analyzers.UnstemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer;
      return analyzers;
    }
  }
}
