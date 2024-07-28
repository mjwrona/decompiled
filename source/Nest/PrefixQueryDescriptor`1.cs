// Decompiled with JetBrains decompiler
// Type: Nest.PrefixQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PrefixQueryDescriptor<T> : 
    TermQueryDescriptorBase<PrefixQueryDescriptor<T>, IPrefixQuery, T>,
    IPrefixQuery,
    ITermQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    MultiTermQueryRewrite IPrefixQuery.Rewrite { get; set; }

    public PrefixQueryDescriptor<T> Rewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IPrefixQuery, MultiTermQueryRewrite>) ((a, v) => a.Rewrite = v));
  }
}
