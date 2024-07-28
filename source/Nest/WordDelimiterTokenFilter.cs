// Decompiled with JetBrains decompiler
// Type: Nest.WordDelimiterTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class WordDelimiterTokenFilter : TokenFilterBase, IWordDelimiterTokenFilter, ITokenFilter
  {
    public WordDelimiterTokenFilter()
      : base("word_delimiter")
    {
    }

    public bool? CatenateAll { get; set; }

    public bool? CatenateNumbers { get; set; }

    public bool? CatenateWords { get; set; }

    public bool? GenerateNumberParts { get; set; }

    public bool? GenerateWordParts { get; set; }

    public bool? PreserveOriginal { get; set; }

    public IEnumerable<string> ProtectedWords { get; set; }

    public string ProtectedWordsPath { get; set; }

    public bool? SplitOnCaseChange { get; set; }

    public bool? SplitOnNumerics { get; set; }

    public bool? StemEnglishPossessive { get; set; }

    public IEnumerable<string> TypeTable { get; set; }

    public string TypeTablePath { get; set; }
  }
}
