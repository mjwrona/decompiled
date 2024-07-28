// Decompiled with JetBrains decompiler
// Type: Nest.PagerDutyContextsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PagerDutyContextsDescriptor : 
    DescriptorPromiseBase<PagerDutyContextsDescriptor, IList<IPagerDutyContext>>
  {
    public PagerDutyContextsDescriptor()
      : base((IList<IPagerDutyContext>) new List<IPagerDutyContext>())
    {
    }

    public PagerDutyContextsDescriptor Context(
      PagerDutyContextType type,
      Func<PagerDutyContextDescriptor, IPagerDutyContext> selector)
    {
      return this.Assign<IPagerDutyContext>(selector != null ? selector(new PagerDutyContextDescriptor(type)) : (IPagerDutyContext) null, (Action<IList<IPagerDutyContext>, IPagerDutyContext>) ((a, v) => a.AddIfNotNull<IPagerDutyContext>(v)));
    }
  }
}
