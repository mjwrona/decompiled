// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.ArabicAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class ArabicAnalyzers : WikiIndexAnalyzers
  {
    protected override WikiIndexAnalyzers.WikiIndexAnalysis GetAnalyzers()
    {
      CustomAnalyzer customAnalyzer1 = new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[4]
        {
          "lowercase",
          "languageStopWords",
          "arabic_normalization",
          "languageStemmer"
        }
      };
      CustomAnalyzer customAnalyzer2 = new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[3]
        {
          "lowercase",
          "languageStopWords",
          "arabic_normalization"
        }
      };
      WikiIndexAnalyzers.WikiIndexAnalysis analyzers = new WikiIndexAnalyzers.WikiIndexAnalysis();
      Nest.TokenFilters tokenFilters = new Nest.TokenFilters();
      tokenFilters["languageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "arabic"
      };
      tokenFilters["languageStopWords"] = (ITokenFilter) new StopTokenFilter()
      {
        StopWords = new StopWords("_arabic_")
      };
      analyzers.TokenFilters = (ITokenFilters) tokenFilters;
      analyzers.LightStemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer1;
      analyzers.StemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer1;
      analyzers.UnstemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer2;
      analyzers.UnstemmedFullTextWithCamelCaseAnalyzer = (IAnalyzer) customAnalyzer2;
      return analyzers;
    }
  }
}
