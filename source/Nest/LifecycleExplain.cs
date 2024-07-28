// Decompiled with JetBrains decompiler
// Type: Nest.LifecycleExplain
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class LifecycleExplain
  {
    [DataMember(Name = "action")]
    public string Action { get; internal set; }

    [DataMember(Name = "action_time_millis")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset ActionTime { get; internal set; }

    [DataMember(Name = "is_auto_retryable_error")]
    public bool? IsAutoRetryableError { get; internal set; }

    [DataMember(Name = "failed_step")]
    public string FailedStep { get; internal set; }

    [DataMember(Name = "failed_step_retry_count")]
    public int? FailedStepRetryCount { get; internal set; }

    [DataMember(Name = "index")]
    public IndexName Index { get; internal set; }

    [DataMember(Name = "lifecycle_date_millis")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset LifecycleDate { get; internal set; }

    [DataMember(Name = "managed")]
    public bool Managed { get; internal set; }

    [DataMember(Name = "phase")]
    public string Phase { get; internal set; }

    [DataMember(Name = "phase_time_millis")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset PhaseTime { get; internal set; }

    [DataMember(Name = "policy")]
    public string Policy { get; internal set; }

    [DataMember(Name = "step")]
    public string Step { get; internal set; }

    [DataMember(Name = "step_info")]
    public IReadOnlyDictionary<string, object> StepInfo { get; internal set; } = EmptyReadOnly<string, object>.Dictionary;

    [DataMember(Name = "step_time_millis")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset StepTime { get; internal set; }

    [DataMember(Name = "age")]
    public Time Age { get; internal set; }
  }
}
