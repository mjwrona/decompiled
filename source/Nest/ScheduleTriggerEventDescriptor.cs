// Decompiled with JetBrains decompiler
// Type: Nest.ScheduleTriggerEventDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ScheduleTriggerEventDescriptor : 
    DescriptorBase<ScheduleTriggerEventDescriptor, IScheduleTriggerEvent>,
    IScheduleTriggerEvent,
    ITriggerEvent
  {
    Union<DateTimeOffset, string> IScheduleTriggerEvent.ScheduledTime { get; set; }

    Union<DateTimeOffset, string> IScheduleTriggerEvent.TriggeredTime { get; set; }

    public ScheduleTriggerEventDescriptor TriggeredTime(DateTimeOffset? triggeredTime) => this.Assign<DateTimeOffset?>(triggeredTime, (Action<IScheduleTriggerEvent, DateTimeOffset?>) ((a, v) =>
    {
      IScheduleTriggerEvent scheduleTriggerEvent = a;
      DateTimeOffset? nullable = v;
      Union<DateTimeOffset, string> valueOrDefault = nullable.HasValue ? (Union<DateTimeOffset, string>) nullable.GetValueOrDefault() : (Union<DateTimeOffset, string>) null;
      scheduleTriggerEvent.TriggeredTime = valueOrDefault;
    }));

    public ScheduleTriggerEventDescriptor TriggeredTime(string triggeredTime) => this.Assign<string>(triggeredTime, (Action<IScheduleTriggerEvent, string>) ((a, v) => a.TriggeredTime = (Union<DateTimeOffset, string>) v));

    public ScheduleTriggerEventDescriptor ScheduledTime(DateTimeOffset? scheduledTime) => this.Assign<DateTimeOffset?>(scheduledTime, (Action<IScheduleTriggerEvent, DateTimeOffset?>) ((a, v) =>
    {
      IScheduleTriggerEvent scheduleTriggerEvent = a;
      DateTimeOffset? nullable = v;
      Union<DateTimeOffset, string> valueOrDefault = nullable.HasValue ? (Union<DateTimeOffset, string>) nullable.GetValueOrDefault() : (Union<DateTimeOffset, string>) null;
      scheduleTriggerEvent.ScheduledTime = valueOrDefault;
    }));

    public ScheduleTriggerEventDescriptor ScheduledTime(string scheduledTime) => this.Assign<string>(scheduledTime, (Action<IScheduleTriggerEvent, string>) ((a, v) => a.ScheduledTime = (Union<DateTimeOffset, string>) v));
  }
}
