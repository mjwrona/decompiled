// Decompiled with JetBrains decompiler
// Type: Nest.ScheduledEvent
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ScheduledEvent
  {
    [DataMember(Name = "calendar_id")]
    public Id CalendarId { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "start_time")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? StartTime { get; set; }

    [DataMember(Name = "end_time")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? EndTime { get; set; }

    [DataMember(Name = "event_id")]
    public Id EventId { get; set; }
  }
}
