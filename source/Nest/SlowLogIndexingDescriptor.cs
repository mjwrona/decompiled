// Decompiled with JetBrains decompiler
// Type: Nest.SlowLogIndexingDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SlowLogIndexingDescriptor : 
    DescriptorBase<SlowLogIndexingDescriptor, ISlowLogIndexing>,
    ISlowLogIndexing
  {
    Nest.LogLevel? ISlowLogIndexing.LogLevel { get; set; }

    int? ISlowLogIndexing.Source { get; set; }

    Time ISlowLogIndexing.ThresholdDebug { get; set; }

    Time ISlowLogIndexing.ThresholdInfo { get; set; }

    Time ISlowLogIndexing.ThresholdTrace { get; set; }

    Time ISlowLogIndexing.ThresholdWarn { get; set; }

    public SlowLogIndexingDescriptor LogLevel(Nest.LogLevel? level) => this.Assign<Nest.LogLevel?>(level, (Action<ISlowLogIndexing, Nest.LogLevel?>) ((a, v) => a.LogLevel = v));

    public SlowLogIndexingDescriptor Source(int? source) => this.Assign<int?>(source, (Action<ISlowLogIndexing, int?>) ((a, v) => a.Source = v));

    public SlowLogIndexingDescriptor ThresholdDebug(Time time) => this.Assign<Time>(time, (Action<ISlowLogIndexing, Time>) ((a, v) => a.ThresholdDebug = v));

    public SlowLogIndexingDescriptor ThresholdInfo(Time time) => this.Assign<Time>(time, (Action<ISlowLogIndexing, Time>) ((a, v) => a.ThresholdInfo = v));

    public SlowLogIndexingDescriptor ThresholdTrace(Time time) => this.Assign<Time>(time, (Action<ISlowLogIndexing, Time>) ((a, v) => a.ThresholdTrace = v));

    public SlowLogIndexingDescriptor ThresholdWarn(Time time) => this.Assign<Time>(time, (Action<ISlowLogIndexing, Time>) ((a, v) => a.ThresholdWarn = v));
  }
}
