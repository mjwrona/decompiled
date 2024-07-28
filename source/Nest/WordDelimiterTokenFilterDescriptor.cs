// Decompiled with JetBrains decompiler
// Type: Nest.WordDelimiterTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class WordDelimiterTokenFilterDescriptor : 
    TokenFilterDescriptorBase<WordDelimiterTokenFilterDescriptor, IWordDelimiterTokenFilter>,
    IWordDelimiterTokenFilter,
    ITokenFilter
  {
    protected override string Type => "word_delimiter";

    bool? IWordDelimiterTokenFilter.CatenateAll { get; set; }

    bool? IWordDelimiterTokenFilter.CatenateNumbers { get; set; }

    bool? IWordDelimiterTokenFilter.CatenateWords { get; set; }

    bool? IWordDelimiterTokenFilter.GenerateNumberParts { get; set; }

    bool? IWordDelimiterTokenFilter.GenerateWordParts { get; set; }

    bool? IWordDelimiterTokenFilter.PreserveOriginal { get; set; }

    IEnumerable<string> IWordDelimiterTokenFilter.ProtectedWords { get; set; }

    string IWordDelimiterTokenFilter.ProtectedWordsPath { get; set; }

    bool? IWordDelimiterTokenFilter.SplitOnCaseChange { get; set; }

    bool? IWordDelimiterTokenFilter.SplitOnNumerics { get; set; }

    bool? IWordDelimiterTokenFilter.StemEnglishPossessive { get; set; }

    IEnumerable<string> IWordDelimiterTokenFilter.TypeTable { get; set; }

    string IWordDelimiterTokenFilter.TypeTablePath { get; set; }

    public WordDelimiterTokenFilterDescriptor GenerateWordParts(bool? generateWordParts = true) => this.Assign<bool?>(generateWordParts, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.GenerateWordParts = v));

    public WordDelimiterTokenFilterDescriptor GenerateNumberParts(bool? generateNumberParts = true) => this.Assign<bool?>(generateNumberParts, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.GenerateNumberParts = v));

    public WordDelimiterTokenFilterDescriptor CatenateWords(bool? catenateWords = true) => this.Assign<bool?>(catenateWords, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.CatenateWords = v));

    public WordDelimiterTokenFilterDescriptor CatenateNumbers(bool? catenateNumbers = true) => this.Assign<bool?>(catenateNumbers, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.CatenateNumbers = v));

    public WordDelimiterTokenFilterDescriptor CatenateAll(bool? catenateAll = true) => this.Assign<bool?>(catenateAll, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.CatenateAll = v));

    public WordDelimiterTokenFilterDescriptor SplitOnCaseChange(bool? split = true) => this.Assign<bool?>(split, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.SplitOnCaseChange = v));

    public WordDelimiterTokenFilterDescriptor SplitOnNumerics(bool? split = true) => this.Assign<bool?>(split, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.SplitOnNumerics = v));

    public WordDelimiterTokenFilterDescriptor PreserveOriginal(bool? preserve = true) => this.Assign<bool?>(preserve, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.PreserveOriginal = v));

    public WordDelimiterTokenFilterDescriptor StemEnglishPossessive(bool? stem = true) => this.Assign<bool?>(stem, (Action<IWordDelimiterTokenFilter, bool?>) ((a, v) => a.StemEnglishPossessive = v));

    public WordDelimiterTokenFilterDescriptor ProtectedWords(IEnumerable<string> protectedWords) => this.Assign<IEnumerable<string>>(protectedWords, (Action<IWordDelimiterTokenFilter, IEnumerable<string>>) ((a, v) => a.ProtectedWords = v));

    public WordDelimiterTokenFilterDescriptor ProtectedWords(params string[] protectedWords) => this.Assign<string[]>(protectedWords, (Action<IWordDelimiterTokenFilter, string[]>) ((a, v) => a.ProtectedWords = (IEnumerable<string>) v));

    public WordDelimiterTokenFilterDescriptor ProtectedWordsPath(string path) => this.Assign<string>(path, (Action<IWordDelimiterTokenFilter, string>) ((a, v) => a.ProtectedWordsPath = v));

    public WordDelimiterTokenFilterDescriptor TypeTable(IEnumerable<string> typeTable) => this.Assign<IEnumerable<string>>(typeTable, (Action<IWordDelimiterTokenFilter, IEnumerable<string>>) ((a, v) => a.TypeTable = v));

    public WordDelimiterTokenFilterDescriptor TypeTable(params string[] typeTable) => this.Assign<string[]>(typeTable, (Action<IWordDelimiterTokenFilter, string[]>) ((a, v) => a.TypeTable = (IEnumerable<string>) v));

    public WordDelimiterTokenFilterDescriptor TypeTablePath(string path) => this.Assign<string>(path, (Action<IWordDelimiterTokenFilter, string>) ((a, v) => a.TypeTablePath = v));
  }
}
