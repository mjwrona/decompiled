// Decompiled with JetBrains decompiler
// Type: Nest.IScheduleTriggerEvent
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (ScheduleTriggerEvent))]
  public interface IScheduleTriggerEvent : ITriggerEvent
  {
    [DataMember(Name = "scheduled_time")]
    Union<DateTimeOffset, string> ScheduledTime { get; set; }

    [DataMember(Name = "triggered_time")]
    Union<DateTimeOffset, string> TriggeredTime { get; set; }
  }
}
