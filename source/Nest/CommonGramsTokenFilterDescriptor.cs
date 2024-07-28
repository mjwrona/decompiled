// Decompiled with JetBrains decompiler
// Type: Nest.CommonGramsTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class CommonGramsTokenFilterDescriptor : 
    TokenFilterDescriptorBase<CommonGramsTokenFilterDescriptor, ICommonGramsTokenFilter>,
    ICommonGramsTokenFilter,
    ITokenFilter
  {
    protected override string Type => "common_grams";

    IEnumerable<string> ICommonGramsTokenFilter.CommonWords { get; set; }

    string ICommonGramsTokenFilter.CommonWordsPath { get; set; }

    bool? ICommonGramsTokenFilter.IgnoreCase { get; set; }

    bool? ICommonGramsTokenFilter.QueryMode { get; set; }

    public CommonGramsTokenFilterDescriptor QueryMode(bool? queryMode = true) => this.Assign<bool?>(queryMode, (Action<ICommonGramsTokenFilter, bool?>) ((a, v) => a.QueryMode = v));

    public CommonGramsTokenFilterDescriptor IgnoreCase(bool? ignoreCase = true) => this.Assign<bool?>(ignoreCase, (Action<ICommonGramsTokenFilter, bool?>) ((a, v) => a.IgnoreCase = v));

    public CommonGramsTokenFilterDescriptor CommonWordsPath(string path) => this.Assign<string>(path, (Action<ICommonGramsTokenFilter, string>) ((a, v) => a.CommonWordsPath = v));

    public CommonGramsTokenFilterDescriptor CommonWords(IEnumerable<string> commonWords) => this.Assign<IEnumerable<string>>(commonWords, (Action<ICommonGramsTokenFilter, IEnumerable<string>>) ((a, v) => a.CommonWords = v));

    public CommonGramsTokenFilterDescriptor CommonWords(params string[] commonWords) => this.Assign<string[]>(commonWords, (Action<ICommonGramsTokenFilter, string[]>) ((a, v) => a.CommonWords = (IEnumerable<string>) v));
  }
}
