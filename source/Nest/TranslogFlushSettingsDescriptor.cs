// Decompiled with JetBrains decompiler
// Type: Nest.TranslogFlushSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TranslogFlushSettingsDescriptor : 
    DescriptorBase<TranslogFlushSettingsDescriptor, ITranslogFlushSettings>,
    ITranslogFlushSettings
  {
    Time ITranslogFlushSettings.Interval { get; set; }

    Time ITranslogFlushSettings.ThresholdPeriod { get; set; }

    string ITranslogFlushSettings.ThresholdSize { get; set; }

    public TranslogFlushSettingsDescriptor ThresholdSize(string size) => this.Assign<string>(size, (Action<ITranslogFlushSettings, string>) ((a, v) => a.ThresholdSize = v));

    public TranslogFlushSettingsDescriptor ThresholdPeriod(Time time) => this.Assign<Time>(time, (Action<ITranslogFlushSettings, Time>) ((a, v) => a.ThresholdPeriod = v));

    public TranslogFlushSettingsDescriptor Interval(Time time) => this.Assign<Time>(time, (Action<ITranslogFlushSettings, Time>) ((a, v) => a.Interval = v));
  }
}
