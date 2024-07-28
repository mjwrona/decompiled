// Decompiled with JetBrains decompiler
// Type: Nest.PagerDutyContextDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PagerDutyContextDescriptor : 
    DescriptorBase<PagerDutyContextDescriptor, IPagerDutyContext>,
    IPagerDutyContext
  {
    public PagerDutyContextDescriptor(PagerDutyContextType type) => this.Self.Type = type;

    string IPagerDutyContext.Href { get; set; }

    string IPagerDutyContext.Src { get; set; }

    PagerDutyContextType IPagerDutyContext.Type { get; set; }

    public PagerDutyContextDescriptor Href(string href) => this.Assign<string>(href, (Action<IPagerDutyContext, string>) ((a, v) => a.Href = v));

    public PagerDutyContextDescriptor Src(string src) => this.Assign<string>(src, (Action<IPagerDutyContext, string>) ((a, v) => a.Src = v));
  }
}
