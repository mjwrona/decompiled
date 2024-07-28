// Decompiled with JetBrains decompiler
// Type: Nest.RescoringDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class RescoringDescriptor<T> : 
    DescriptorPromiseBase<RescoringDescriptor<T>, IList<IRescore>>
    where T : class
  {
    public RescoringDescriptor()
      : base((IList<IRescore>) new List<IRescore>())
    {
    }

    public RescoringDescriptor<T> Rescore(Func<RescoreDescriptor<T>, IRescore> selector) => this.AddRescore(selector != null ? selector(new RescoreDescriptor<T>()) : (IRescore) null);

    private RescoringDescriptor<T> AddRescore(IRescore rescore) => rescore != null ? this.Assign<IRescore>(rescore, (Action<IList<IRescore>, IRescore>) ((a, v) => a.Add(v))) : this;
  }
}
