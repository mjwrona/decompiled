// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.WorkItemIndexAnalyzers
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public abstract class WorkItemIndexAnalyzers
  {
    public const string LightStemmedFullTextAnalyzer = "lightStemmedFullTextAnalyzer";
    public const string MinimallyStemmedFullTextAnalyzer = "minimallyStemmedFullTextAnalyzer";
    public const string UnstemmedFullTextAnalyzer = "unstemmedFullTextAnalyzer";
    public const string OnlyLowerCaseAnalyzer = "onlyLowerCaseAnalyzer";
    public const string PathAnalyzer = "pathAnalyzer";
    public const string TagAnalyzer = "tagAnalyzer";
    protected const string StandardTokenizer = "standard";
    protected const string KeywordTokenizer = "keyword";
    protected const string PathTokenizer = "pathTokenizer";
    protected const string AsciiFoldingFilter = "asciifolding";
    protected const string LowercaseFilter = "lowercase";
    protected const string FullTextWordDelimiterFilter = "fullTextWordDelimiter";
    protected const string LanguageStopWords = "languageStopWords";
    protected const string LanguageStemmer = "languageStemmer";
    protected const string LightLanguageStemmer = "lightLanguageStemmer";
    protected const string MinimalLanguageStemmer = "minimalLanguageStemmer";

    public IDictionary<string, IAnalyzer> AnalyzersMap { get; private set; }

    public ITokenFilters TokenFilters { get; private set; }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected WorkItemIndexAnalyzers() => this.PopulateAnalyzers();

    private void PopulateAnalyzers()
    {
      WorkItemIndexAnalyzers.WorkItemIndexAnalysis analyzers = this.GetAnalyzers();
      this.AnalyzersMap = (IDictionary<string, IAnalyzer>) new Dictionary<string, IAnalyzer>()
      {
        ["onlyLowerCaseAnalyzer"] = (analyzers.OnlyLowerCaseAnalyzer ?? this.DefaultOnlyLowerCaseAnalyzer),
        ["lightStemmedFullTextAnalyzer"] = analyzers.LightStemmedFullTextAnalyzer,
        ["minimallyStemmedFullTextAnalyzer"] = analyzers.MinimallyStemmedFullTextAnalyzer,
        ["unstemmedFullTextAnalyzer"] = (analyzers.UnstemmedFullTextAnalyzer ?? this.DefaultUnstemmedFullTextAnalyzer),
        ["pathAnalyzer"] = (analyzers.PathAnalyzer ?? this.DefaultPathAnalyzer),
        ["tagAnalyzer"] = (analyzers.TagAnalyzer ?? this.DefaultTagAnalyzer)
      };
      this.TokenFilters = analyzers.TokenFilters;
    }

    protected abstract WorkItemIndexAnalyzers.WorkItemIndexAnalysis GetAnalyzers();

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
        "asciifolding",
        "lowercase"
      }
    };

    protected IAnalyzer DefaultPathAnalyzer => (IAnalyzer) new CustomAnalyzer()
    {
      Tokenizer = "pathTokenizer",
      Filter = (IEnumerable<string>) new string[1]
      {
        "lowercase"
      }
    };

    protected IAnalyzer DefaultTagAnalyzer => (IAnalyzer) new PatternAnalyzer()
    {
      Pattern = "[;,] ",
      Lowercase = new bool?(true)
    };

    protected class WorkItemIndexAnalysis
    {
      public IAnalyzer LightStemmedFullTextAnalyzer { get; set; }

      public IAnalyzer MinimallyStemmedFullTextAnalyzer { get; set; }

      public IAnalyzer UnstemmedFullTextAnalyzer { get; set; }

      public IAnalyzer OnlyLowerCaseAnalyzer { get; set; }

      public IAnalyzer PathAnalyzer { get; set; }

      public IAnalyzer TagAnalyzer { get; set; }

      public ITokenFilters TokenFilters { get; set; }
    }
  }
}
