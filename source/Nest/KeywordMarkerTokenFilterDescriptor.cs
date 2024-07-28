// Decompiled with JetBrains decompiler
// Type: Nest.KeywordMarkerTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class KeywordMarkerTokenFilterDescriptor : 
    TokenFilterDescriptorBase<KeywordMarkerTokenFilterDescriptor, IKeywordMarkerTokenFilter>,
    IKeywordMarkerTokenFilter,
    ITokenFilter
  {
    protected override string Type => "keyword_marker";

    bool? IKeywordMarkerTokenFilter.IgnoreCase { get; set; }

    IEnumerable<string> IKeywordMarkerTokenFilter.Keywords { get; set; }

    string IKeywordMarkerTokenFilter.KeywordsPath { get; set; }

    string IKeywordMarkerTokenFilter.KeywordsPattern { get; set; }

    public KeywordMarkerTokenFilterDescriptor IgnoreCase(bool? ignoreCase = true) => this.Assign<bool?>(ignoreCase, (Action<IKeywordMarkerTokenFilter, bool?>) ((a, v) => a.IgnoreCase = v));

    public KeywordMarkerTokenFilterDescriptor KeywordsPath(string path) => this.Assign<string>(path, (Action<IKeywordMarkerTokenFilter, string>) ((a, v) => a.KeywordsPath = v));

    public KeywordMarkerTokenFilterDescriptor KeywordsPattern(string pattern) => this.Assign<string>(pattern, (Action<IKeywordMarkerTokenFilter, string>) ((a, v) => a.KeywordsPattern = v));

    public KeywordMarkerTokenFilterDescriptor Keywords(IEnumerable<string> keywords) => this.Assign<IEnumerable<string>>(keywords, (Action<IKeywordMarkerTokenFilter, IEnumerable<string>>) ((a, v) => a.Keywords = v));

    public KeywordMarkerTokenFilterDescriptor Keywords(params string[] keywords) => this.Assign<string[]>(keywords, (Action<IKeywordMarkerTokenFilter, string[]>) ((a, v) => a.Keywords = (IEnumerable<string>) v));
  }
}
