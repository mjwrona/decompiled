// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.EnglishAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class EnglishAnalyzers : WorkItemIndexAnalyzers
  {
    protected override WorkItemIndexAnalyzers.WorkItemIndexAnalysis GetAnalyzers()
    {
      ITokenFilter tokenFilter = (ITokenFilter) new WordDelimiterTokenFilter()
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
      WorkItemIndexAnalyzers.WorkItemIndexAnalysis analyzers = new WorkItemIndexAnalyzers.WorkItemIndexAnalysis();
      WorkItemIndexAnalyzers.WorkItemIndexAnalysis itemIndexAnalysis = analyzers;
      Nest.TokenFilters tokenFilters = new Nest.TokenFilters();
      tokenFilters["lightLanguageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "light_english"
      };
      tokenFilters["minimalLanguageStemmer"] = (ITokenFilter) new StemmerTokenFilter()
      {
        Language = "minimal_english"
      };
      tokenFilters["fullTextWordDelimiter"] = tokenFilter;
      itemIndexAnalysis.TokenFilters = (ITokenFilters) tokenFilters;
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
      analyzers.MinimallyStemmedFullTextAnalyzer = (IAnalyzer) new CustomAnalyzer()
      {
        Tokenizer = "standard",
        Filter = (IEnumerable<string>) new string[4]
        {
          "asciifolding",
          "fullTextWordDelimiter",
          "lowercase",
          "minimalLanguageStemmer"
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
      return analyzers;
    }
  }
}
