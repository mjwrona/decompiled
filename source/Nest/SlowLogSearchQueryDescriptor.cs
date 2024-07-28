// Decompiled with JetBrains decompiler
// Type: Nest.SlowLogSearchQueryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SlowLogSearchQueryDescriptor : 
    DescriptorBase<SlowLogSearchQueryDescriptor, ISlowLogSearchQuery>,
    ISlowLogSearchQuery
  {
    Time ISlowLogSearchQuery.ThresholdDebug { get; set; }

    Time ISlowLogSearchQuery.ThresholdInfo { get; set; }

    Time ISlowLogSearchQuery.ThresholdTrace { get; set; }

    Time ISlowLogSearchQuery.ThresholdWarn { get; set; }

    public SlowLogSearchQueryDescriptor ThresholdDebug(Time time) => this.Assign<Time>(time, (Action<ISlowLogSearchQuery, Time>) ((a, v) => a.ThresholdDebug = v));

    public SlowLogSearchQueryDescriptor ThresholdInfo(Time time) => this.Assign<Time>(time, (Action<ISlowLogSearchQuery, Time>) ((a, v) => a.ThresholdInfo = v));

    public SlowLogSearchQueryDescriptor ThresholdTrace(Time time) => this.Assign<Time>(time, (Action<ISlowLogSearchQuery, Time>) ((a, v) => a.ThresholdTrace = v));

    public SlowLogSearchQueryDescriptor ThresholdWarn(Time time) => this.Assign<Time>(time, (Action<ISlowLogSearchQuery, Time>) ((a, v) => a.ThresholdWarn = v));
  }
}
