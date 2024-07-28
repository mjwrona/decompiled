// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.TurkishAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class TurkishAnalyzers : WikiIndexAnalyzers
  {
    private const string ApostropheFilter = "apostrophe";
    private const string TurkishLowerCaseFilter = "turkishLowerCase";

    protected override WikiIndexAnalyzers.WikiIndexAnalysis GetAnalyzers()
    {
      CustomAnalyzer customAnalyzer1 = new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[4]
        {
          "apostrophe",
          "turkishLowerCase",
          "languageStopWords",
          "languageStemmer"
        }
      };
      CustomAnalyzer customAnalyzer2 = new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[3]
        {
          "apostrophe",
          "turkishLowerCase",
          "languageStopWords"
        }
      };
      WikiIndexAnalyzers.WikiIndexAnalysis analyzers = new WikiIndexAnalyzers.WikiIndexAnalysis();
      WikiIndexAnalyzers.WikiIndexAnalysis wikiIndexAnalysis = analyzers;
      Nest.TokenFilters tokenFilters = new Nest.TokenFilters();
      tokenFilters["languageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "turkish"
      };
      tokenFilters["languageStopWords"] = (ITokenFilter) new StopTokenFilter()
      {
        StopWords = new StopWords("_turkish_")
      };
      tokenFilters["turkishLowerCase"] = (ITokenFilter) new LowercaseTokenFilter()
      {
        Language = "turkish"
      };
      wikiIndexAnalysis.TokenFilters = (ITokenFilters) tokenFilters;
      analyzers.OnlyLowerCaseAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "keyword",
        Filter = (IEnumerable<string>) new string[1]
        {
          "turkishLowerCase"
        }
      };
      analyzers.LowerCaseWhitespaceAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "whitespace",
        Filter = (IEnumerable<string>) new string[1]
        {
          "turkishLowerCase"
        }
      };
      analyzers.LightStemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer1;
      analyzers.StemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer1;
      analyzers.UnstemmedFullTextAnalyzer = (IAnalyzer) customAnalyzer2;
      analyzers.UnstemmedFullTextWithCamelCaseAnalyzer = (IAnalyzer) customAnalyzer2;
      return analyzers;
    }
  }
}
