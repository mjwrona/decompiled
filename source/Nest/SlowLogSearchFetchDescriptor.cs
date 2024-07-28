// Decompiled with JetBrains decompiler
// Type: Nest.SlowLogSearchFetchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SlowLogSearchFetchDescriptor : 
    DescriptorBase<SlowLogSearchFetchDescriptor, ISlowLogSearchFetch>,
    ISlowLogSearchFetch
  {
    Time ISlowLogSearchFetch.ThresholdDebug { get; set; }

    Time ISlowLogSearchFetch.ThresholdInfo { get; set; }

    Time ISlowLogSearchFetch.ThresholdTrace { get; set; }

    Time ISlowLogSearchFetch.ThresholdWarn { get; set; }

    public SlowLogSearchFetchDescriptor ThresholdDebug(Time time) => this.Assign<Time>(time, (Action<ISlowLogSearchFetch, Time>) ((a, v) => a.ThresholdDebug = v));

    public SlowLogSearchFetchDescriptor ThresholdInfo(Time time) => this.Assign<Time>(time, (Action<ISlowLogSearchFetch, Time>) ((a, v) => a.ThresholdInfo = v));

    public SlowLogSearchFetchDescriptor ThresholdTrace(Time time) => this.Assign<Time>(time, (Action<ISlowLogSearchFetch, Time>) ((a, v) => a.ThresholdTrace = v));

    public SlowLogSearchFetchDescriptor ThresholdWarn(Time time) => this.Assign<Time>(time, (Action<ISlowLogSearchFetch, Time>) ((a, v) => a.ThresholdWarn = v));
  }
}
