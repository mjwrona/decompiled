// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.WikiIndexAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public abstract class WikiIndexAnalyzers
  {
    public const string LightStemmedFullTextAnalyzer = "lightStemmedFullTextAnalyzer";
    public const string StemmedFullTextAnalyzer = "stemmedFullTextAnalyzer";
    public const string UnstemmedFullTextAnalyzer = "unstemmedFullTextAnalyzer";
    public const string UnstemmedFullTextWithCamelCaseAnalyzer = "unstemmedFullTextWithCamelCaseAnalyzer";
    public const string OnlyLowerCaseAnalyzer = "onlyLowerCaseAnalyzer";
    public const string LowerCaseWhitespaceAnalyzer = "lowercaseWhitespaceAnalyzer";
    protected const string StandardTokenizer = "standard";
    protected const string KeywordTokenizer = "keyword";
    protected const string WhitespaceTokenizer = "whitespace";
    protected const string AsciiFoldingFilter = "asciifolding";
    protected const string LowercaseFilter = "lowercase";
    protected const string LanguageStopWords = "languageStopWords";
    protected const string LanguageStemmer = "languageStemmer";
    protected const string LightLanguageStemmer = "lightLanguageStemmer";

    public IDictionary<string, IAnalyzer> AnalyzersMap { get; private set; }

    public ITokenFilters TokenFilters { get; private set; }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected WikiIndexAnalyzers() => this.PopulateAnalyzers();

    private void PopulateAnalyzers()
    {
      WikiIndexAnalyzers.WikiIndexAnalysis analyzers = this.GetAnalyzers();
      this.AnalyzersMap = (IDictionary<string, IAnalyzer>) new Dictionary<string, IAnalyzer>()
      {
        ["onlyLowerCaseAnalyzer"] = (analyzers.OnlyLowerCaseAnalyzer ?? this.DefaultOnlyLowerCaseAnalyzer),
        ["lightStemmedFullTextAnalyzer"] = analyzers.LightStemmedFullTextAnalyzer,
        ["stemmedFullTextAnalyzer"] = analyzers.StemmedFullTextAnalyzer,
        ["unstemmedFullTextAnalyzer"] = (analyzers.UnstemmedFullTextAnalyzer ?? this.DefaultUnstemmedFullTextAnalyzer),
        ["unstemmedFullTextWithCamelCaseAnalyzer"] = (analyzers.UnstemmedFullTextWithCamelCaseAnalyzer ?? this.DefaultUnstemmedFullTextAnalyzer),
        ["lowercaseWhitespaceAnalyzer"] = (analyzers.LowerCaseWhitespaceAnalyzer ?? this.DefaultLowerCaseWhitespaceAnalyzer)
      };
      this.TokenFilters = analyzers.TokenFilters;
    }

    protected abstract WikiIndexAnalyzers.WikiIndexAnalysis GetAnalyzers();

    protected IAnalyzer DefaultOnlyLowerCaseAnalyzer => (IAnalyzer) new CustomAnalyzer()
    {
      Tokenizer = "keyword",
      Filter = (IEnumerable<string>) new string[1]
      {
        "lowercase"
      }
    };

    protected IAnalyzer DefaultUnstemmedFullTextAnalyzer => (IAnalyzer) new CustomAnalyzer()
    {
      Tokenizer = "standard",
      Filter = (IEnumerable<string>) new string[2]
      {
        "lowercase",
        "languageStopWords"
      }
    };

    protected IAnalyzer DefaultLowerCaseWhitespaceAnalyzer => (IAnalyzer) new CustomAnalyzer()
    {
      Tokenizer = "whitespace",
      Filter = (IEnumerable<string>) new string[1]
      {
        "lowercase"
      }
    };

    protected class WikiIndexAnalysis
    {
      public IAnalyzer LightStemmedFullTextAnalyzer { get; set; }

      public IAnalyzer StemmedFullTextAnalyzer { get; set; }

      public IAnalyzer UnstemmedFullTextAnalyzer { get; set; }

      public IAnalyzer UnstemmedFullTextWithCamelCaseAnalyzer { get; set; }

      public IAnalyzer OnlyLowerCaseAnalyzer { get; set; }

      public IAnalyzer LowerCaseWhitespaceAnalyzer { get; set; }

      public ITokenFilters TokenFilters { get; set; }
    }
  }
}
