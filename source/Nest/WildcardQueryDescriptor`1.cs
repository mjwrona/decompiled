// Decompiled with JetBrains decompiler
// Type: Nest.WildcardQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class WildcardQueryDescriptor<T> : 
    TermQueryDescriptorBase<WildcardQueryDescriptor<T>, IWildcardQuery, T>,
    IWildcardQuery,
    ITermQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    MultiTermQueryRewrite IWildcardQuery.Rewrite { get; set; }

    string IWildcardQuery.Wildcard { get; set; }

    protected override bool Conditionless => WildcardQuery.IsConditionless((IWildcardQuery) this);

    public WildcardQueryDescriptor<T> Rewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IWildcardQuery, MultiTermQueryRewrite>) ((a, v) => a.Rewrite = v));

    public WildcardQueryDescriptor<T> Wildcard(string value) => this.Assign<string>(value, (Action<IWildcardQuery, string>) ((a, v) => a.Wildcard = v));
  }
}
