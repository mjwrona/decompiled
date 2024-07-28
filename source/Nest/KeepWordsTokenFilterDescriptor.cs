// Decompiled with JetBrains decompiler
// Type: Nest.KeepWordsTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class KeepWordsTokenFilterDescriptor : 
    TokenFilterDescriptorBase<KeepWordsTokenFilterDescriptor, IKeepWordsTokenFilter>,
    IKeepWordsTokenFilter,
    ITokenFilter
  {
    protected override string Type => "keep";

    IEnumerable<string> IKeepWordsTokenFilter.KeepWords { get; set; }

    bool? IKeepWordsTokenFilter.KeepWordsCase { get; set; }

    string IKeepWordsTokenFilter.KeepWordsPath { get; set; }

    public KeepWordsTokenFilterDescriptor KeepWordsCase(bool? keepCase = true) => this.Assign<bool?>(keepCase, (Action<IKeepWordsTokenFilter, bool?>) ((a, v) => a.KeepWordsCase = v));

    public KeepWordsTokenFilterDescriptor KeepWordsPath(string path) => this.Assign<string>(path, (Action<IKeepWordsTokenFilter, string>) ((a, v) => a.KeepWordsPath = v));

    public KeepWordsTokenFilterDescriptor KeepWords(IEnumerable<string> keepWords) => this.Assign<IEnumerable<string>>(keepWords, (Action<IKeepWordsTokenFilter, IEnumerable<string>>) ((a, v) => a.KeepWords = v));

    public KeepWordsTokenFilterDescriptor KeepWords(params string[] keepWords) => this.Assign<string[]>(keepWords, (Action<IKeepWordsTokenFilter, string[]>) ((a, v) => a.KeepWords = (IEnumerable<string>) v));
  }
}
