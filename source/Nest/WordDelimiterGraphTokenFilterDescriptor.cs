// Decompiled with JetBrains decompiler
// Type: Nest.WordDelimiterGraphTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class WordDelimiterGraphTokenFilterDescriptor : 
    TokenFilterDescriptorBase<WordDelimiterGraphTokenFilterDescriptor, IWordDelimiterGraphTokenFilter>,
    IWordDelimiterGraphTokenFilter,
    ITokenFilter
  {
    protected override string Type => "word_delimiter_graph";

    bool? IWordDelimiterGraphTokenFilter.AdjustOffsets { get; set; }

    bool? IWordDelimiterGraphTokenFilter.CatenateAll { get; set; }

    bool? IWordDelimiterGraphTokenFilter.CatenateNumbers { get; set; }

    bool? IWordDelimiterGraphTokenFilter.CatenateWords { get; set; }

    bool? IWordDelimiterGraphTokenFilter.GenerateNumberParts { get; set; }

    bool? IWordDelimiterGraphTokenFilter.GenerateWordParts { get; set; }

    bool? IWordDelimiterGraphTokenFilter.IgnoreKeywords { get; set; }

    bool? IWordDelimiterGraphTokenFilter.PreserveOriginal { get; set; }

    IEnumerable<string> IWordDelimiterGraphTokenFilter.ProtectedWords { get; set; }

    string IWordDelimiterGraphTokenFilter.ProtectedWordsPath { get; set; }

    bool? IWordDelimiterGraphTokenFilter.SplitOnCaseChange { get; set; }

    bool? IWordDelimiterGraphTokenFilter.SplitOnNumerics { get; set; }

    bool? IWordDelimiterGraphTokenFilter.StemEnglishPossessive { get; set; }

    IEnumerable<string> IWordDelimiterGraphTokenFilter.TypeTable { get; set; }

    string IWordDelimiterGraphTokenFilter.TypeTablePath { get; set; }

    public WordDelimiterGraphTokenFilterDescriptor GenerateWordParts(bool? generateWordParts = true) => this.Assign<bool?>(generateWordParts, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.GenerateWordParts = v));

    public WordDelimiterGraphTokenFilterDescriptor GenerateNumberParts(bool? generateNumberParts = true) => this.Assign<bool?>(generateNumberParts, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.GenerateNumberParts = v));

    public WordDelimiterGraphTokenFilterDescriptor IgnoreKeywords(bool? ignoreKeywords = true) => this.Assign<bool?>(ignoreKeywords, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.IgnoreKeywords = v));

    public WordDelimiterGraphTokenFilterDescriptor CatenateWords(bool? catenateWords = true) => this.Assign<bool?>(catenateWords, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.CatenateWords = v));

    public WordDelimiterGraphTokenFilterDescriptor CatenateNumbers(bool? catenateNumbers = true) => this.Assign<bool?>(catenateNumbers, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.CatenateNumbers = v));

    public WordDelimiterGraphTokenFilterDescriptor AdjustOffsets(bool? adjustOffsets = true) => this.Assign<bool?>(adjustOffsets, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.AdjustOffsets = v));

    public WordDelimiterGraphTokenFilterDescriptor CatenateAll(bool? catenateAll = true) => this.Assign<bool?>(catenateAll, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.CatenateAll = v));

    public WordDelimiterGraphTokenFilterDescriptor SplitOnCaseChange(bool? split = true) => this.Assign<bool?>(split, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.SplitOnCaseChange = v));

    public WordDelimiterGraphTokenFilterDescriptor SplitOnNumerics(bool? split = true) => this.Assign<bool?>(split, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.SplitOnNumerics = v));

    public WordDelimiterGraphTokenFilterDescriptor PreserveOriginal(bool? preserve = true) => this.Assign<bool?>(preserve, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.PreserveOriginal = v));

    public WordDelimiterGraphTokenFilterDescriptor StemEnglishPossessive(bool? stem = true) => this.Assign<bool?>(stem, (Action<IWordDelimiterGraphTokenFilter, bool?>) ((a, v) => a.StemEnglishPossessive = v));

    public WordDelimiterGraphTokenFilterDescriptor ProtectedWords(IEnumerable<string> protectedWords) => this.Assign<IEnumerable<string>>(protectedWords, (Action<IWordDelimiterGraphTokenFilter, IEnumerable<string>>) ((a, v) => a.ProtectedWords = v));

    public WordDelimiterGraphTokenFilterDescriptor ProtectedWords(params string[] protectedWords) => this.Assign<string[]>(protectedWords, (Action<IWordDelimiterGraphTokenFilter, string[]>) ((a, v) => a.ProtectedWords = (IEnumerable<string>) v));

    public WordDelimiterGraphTokenFilterDescriptor ProtectedWordsPath(string path) => this.Assign<string>(path, (Action<IWordDelimiterGraphTokenFilter, string>) ((a, v) => a.ProtectedWordsPath = v));

    public WordDelimiterGraphTokenFilterDescriptor TypeTable(IEnumerable<string> typeTable) => this.Assign<IEnumerable<string>>(typeTable, (Action<IWordDelimiterGraphTokenFilter, IEnumerable<string>>) ((a, v) => a.TypeTable = v));

    public WordDelimiterGraphTokenFilterDescriptor TypeTable(params string[] typeTable) => this.Assign<string[]>(typeTable, (Action<IWordDelimiterGraphTokenFilter, string[]>) ((a, v) => a.TypeTable = (IEnumerable<string>) v));

    public WordDelimiterGraphTokenFilterDescriptor TypeTablePath(string path) => this.Assign<string>(path, (Action<IWordDelimiterGraphTokenFilter, string>) ((a, v) => a.TypeTablePath = v));
  }
}
