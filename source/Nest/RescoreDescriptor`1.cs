// Decompiled with JetBrains decompiler
// Type: Nest.RescoreDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RescoreDescriptor<T> : DescriptorBase<RescoreDescriptor<T>, IRescore>, IRescore where T : class
  {
    IRescoreQuery IRescore.Query { get; set; }

    int? IRescore.WindowSize { get; set; }

    public virtual RescoreDescriptor<T> RescoreQuery(
      Func<RescoreQueryDescriptor<T>, IRescoreQuery> rescoreQuerySelector)
    {
      return this.Assign<Func<RescoreQueryDescriptor<T>, IRescoreQuery>>(rescoreQuerySelector, (Action<IRescore, Func<RescoreQueryDescriptor<T>, IRescoreQuery>>) ((a, v) => a.Query = v != null ? v(new RescoreQueryDescriptor<T>()) : (IRescoreQuery) null));
    }

    public virtual RescoreDescriptor<T> WindowSize(int? windowSize) => this.Assign<int?>(windowSize, (Action<IRescore, int?>) ((a, v) => a.WindowSize = v));
  }
}
