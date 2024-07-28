// Decompiled with JetBrains decompiler
// Type: Nest.ElisionTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ElisionTokenFilterDescriptor : 
    TokenFilterDescriptorBase<ElisionTokenFilterDescriptor, IElisionTokenFilter>,
    IElisionTokenFilter,
    ITokenFilter
  {
    protected override string Type => "elision";

    IEnumerable<string> IElisionTokenFilter.Articles { get; set; }

    bool? IElisionTokenFilter.ArticlesCase { get; set; }

    public ElisionTokenFilterDescriptor Articles(IEnumerable<string> articles) => this.Assign<IEnumerable<string>>(articles, (Action<IElisionTokenFilter, IEnumerable<string>>) ((a, v) => a.Articles = v));

    public ElisionTokenFilterDescriptor Articles(params string[] articles) => this.Assign<string[]>(articles, (Action<IElisionTokenFilter, string[]>) ((a, v) => a.Articles = (IEnumerable<string>) v));

    public ElisionTokenFilterDescriptor ArticlesCase(bool? articlesCase = true) => this.Assign<bool?>(articlesCase, (Action<IElisionTokenFilter, bool?>) ((a, v) => a.ArticlesCase = v));
  }
}
