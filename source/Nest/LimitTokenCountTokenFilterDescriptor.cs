// Decompiled with JetBrains decompiler
// Type: Nest.LimitTokenCountTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LimitTokenCountTokenFilterDescriptor : 
    TokenFilterDescriptorBase<LimitTokenCountTokenFilterDescriptor, ILimitTokenCountTokenFilter>,
    ILimitTokenCountTokenFilter,
    ITokenFilter
  {
    protected override string Type => "limit";

    bool? ILimitTokenCountTokenFilter.ConsumeAllTokens { get; set; }

    int? ILimitTokenCountTokenFilter.MaxTokenCount { get; set; }

    public LimitTokenCountTokenFilterDescriptor ConsumeAllToken(bool? consumeAllTokens = true) => this.Assign<bool?>(consumeAllTokens, (Action<ILimitTokenCountTokenFilter, bool?>) ((a, v) => a.ConsumeAllTokens = v));

    public LimitTokenCountTokenFilterDescriptor MaxTokenCount(int? maxTokenCount) => this.Assign<int?>(maxTokenCount, (Action<ILimitTokenCountTokenFilter, int?>) ((a, v) => a.MaxTokenCount = v));
  }
}
