// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.SwedishAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class SwedishAnalyzers : WikiIndexAnalyzers
  {
    protected override WikiIndexAnalyzers.WikiIndexAnalysis GetAnalyzers()
    {
      WikiIndexAnalyzers.WikiIndexAnalysis analyzers = new WikiIndexAnalyzers.WikiIndexAnalysis();
      WikiIndexAnalyzers.WikiIndexAnalysis wikiIndexAnalysis = analyzers;
      Nest.TokenFilters tokenFilters = new Nest.TokenFilters();
      tokenFilters["languageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "swedish"
      };
      tokenFilters["languageStopWords"] = (ITokenFilter) new StopTokenFilter()
      {
        StopWords = new StopWords("_swedish_")
      };
      wikiIndexAnalysis.TokenFilters = (ITokenFilters) tokenFilters;
      analyzers.LightStemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[3]
        {
          "lowercase",
          "languageStopWords",
          "languageStemmer"
        }
      };
      analyzers.StemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[3]
        {
          "lowercase",
          "languageStopWords",
          "languageStemmer"
        }
      };
      return analyzers;
    }
  }
}
