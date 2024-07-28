// Decompiled with JetBrains decompiler
// Type: Nest.PhraseSuggestCollateDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PhraseSuggestCollateDescriptor<T> : 
    DescriptorBase<PhraseSuggestCollateDescriptor<T>, IPhraseSuggestCollate>,
    IPhraseSuggestCollate
    where T : class
  {
    IDictionary<string, object> IPhraseSuggestCollate.Params { get; set; }

    bool? IPhraseSuggestCollate.Prune { get; set; }

    IPhraseSuggestCollateQuery IPhraseSuggestCollate.Query { get; set; }

    public PhraseSuggestCollateDescriptor<T> Query(
      Func<PhraseSuggestCollateQueryDescriptor, IPhraseSuggestCollateQuery> selector)
    {
      return this.Assign<Func<PhraseSuggestCollateQueryDescriptor, IPhraseSuggestCollateQuery>>(selector, (Action<IPhraseSuggestCollate, Func<PhraseSuggestCollateQueryDescriptor, IPhraseSuggestCollateQuery>>) ((a, v) => a.Query = v != null ? v(new PhraseSuggestCollateQueryDescriptor()) : (IPhraseSuggestCollateQuery) null));
    }

    public PhraseSuggestCollateDescriptor<T> Prune(bool? prune = true) => this.Assign<bool?>(prune, (Action<IPhraseSuggestCollate, bool?>) ((a, v) => a.Prune = v));

    public PhraseSuggestCollateDescriptor<T> Params(IDictionary<string, object> paramsDictionary) => this.Assign<IDictionary<string, object>>(paramsDictionary, (Action<IPhraseSuggestCollate, IDictionary<string, object>>) ((a, v) => a.Params = v));

    public PhraseSuggestCollateDescriptor<T> Params(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsDictionary)
    {
      return this.Assign<FluentDictionary<string, object>>(paramsDictionary(new FluentDictionary<string, object>()), (Action<IPhraseSuggestCollate, FluentDictionary<string, object>>) ((a, v) => a.Params = (IDictionary<string, object>) v));
    }
  }
}
