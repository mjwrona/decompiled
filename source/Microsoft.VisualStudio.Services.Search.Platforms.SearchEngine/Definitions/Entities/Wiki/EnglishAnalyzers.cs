// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.EnglishAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class EnglishAnalyzers : WikiIndexAnalyzers
  {
    private const string FullTextWordDelimiterFilter = "fullTextWordDelimiter";
    private const string FullTextWithCamelCaseWordDelimiterFilter = "fullTextWithCamelCaseWordDelimiter";
    private const string ShingleFilter = "shingleFilter";

    protected override WikiIndexAnalyzers.WikiIndexAnalysis GetAnalyzers()
    {
      ITokenFilter tokenFilter1 = (ITokenFilter) new WordDelimiterTokenFilter()
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
      ITokenFilter tokenFilter2 = (ITokenFilter) new WordDelimiterTokenFilter()
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
      WikiIndexAnalyzers.WikiIndexAnalysis analyzers = new WikiIndexAnalyzers.WikiIndexAnalysis();
      WikiIndexAnalyzers.WikiIndexAnalysis wikiIndexAnalysis = analyzers;
      Nest.TokenFilters tokenFilters = new Nest.TokenFilters();
      tokenFilters["lightLanguageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "light_english"
      };
      tokenFilters["languageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "english"
      };
      tokenFilters["fullTextWordDelimiter"] = tokenFilter1;
      tokenFilters["fullTextWithCamelCaseWordDelimiter"] = tokenFilter2;
      tokenFilters["shingleFilter"] = (ITokenFilter) new ShingleTokenFilter()
      {
        TokenSeparator = string.Empty,
        MaxShingleSize = new int?(4),
        MinShingleSize = new int?(2)
      };
      wikiIndexAnalysis.TokenFilters = (ITokenFilters) tokenFilters;
      analyzers.LightStemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[4]
        {
          "asciifolding",
          "fullTextWordDelimiter",
          "lowercase",
          "lightLanguageStemmer"
        }
      };
      analyzers.StemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[4]
        {
          "asciifolding",
          "fullTextWordDelimiter",
          "lowercase",
          "languageStemmer"
        }
      };
      analyzers.UnstemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[3]
        {
          "asciifolding",
          "fullTextWordDelimiter",
          "lowercase"
        }
      };
      analyzers.UnstemmedFullTextWithCamelCaseAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "whitespace",
        Filter = (IEnumerable<string>) new string[4]
        {
          "asciifolding",
          "fullTextWithCamelCaseWordDelimiter",
          "shingleFilter",
          "lowercase"
        }
      };
      return analyzers;
    }
  }
}
