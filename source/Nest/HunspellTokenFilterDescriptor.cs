// Decompiled with JetBrains decompiler
// Type: Nest.HunspellTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HunspellTokenFilterDescriptor : 
    TokenFilterDescriptorBase<HunspellTokenFilterDescriptor, IHunspellTokenFilter>,
    IHunspellTokenFilter,
    ITokenFilter
  {
    protected override string Type => "hunspell";

    bool? IHunspellTokenFilter.Dedup { get; set; }

    string IHunspellTokenFilter.Dictionary { get; set; }

    string IHunspellTokenFilter.Locale { get; set; }

    bool? IHunspellTokenFilter.LongestOnly { get; set; }

    public HunspellTokenFilterDescriptor LongestOnly(bool? longestOnly = true) => this.Assign<bool?>(longestOnly, (Action<IHunspellTokenFilter, bool?>) ((a, v) => a.LongestOnly = v));

    public HunspellTokenFilterDescriptor Dedup(bool? dedup = true) => this.Assign<bool?>(dedup, (Action<IHunspellTokenFilter, bool?>) ((a, v) => a.Dedup = v));

    public HunspellTokenFilterDescriptor Locale(string locale) => this.Assign<string>(locale, (Action<IHunspellTokenFilter, string>) ((a, v) => a.Locale = v));

    public HunspellTokenFilterDescriptor Dictionary(string dictionary) => this.Assign<string>(dictionary, (Action<IHunspellTokenFilter, string>) ((a, v) => a.Dictionary = v));
  }
}
