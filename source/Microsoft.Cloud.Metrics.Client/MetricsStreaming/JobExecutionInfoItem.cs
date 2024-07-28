// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.JobExecutionInfoItem
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public class JobExecutionInfoItem
  {
    public string JobId { get; set; }

    public int RuleVersion { get; set; }

    [JsonConverter(typeof (StringEnumConverter))]
    public JobExecutionStatus Status { get; set; }

    public string Stamp { get; set; }

    public string StampColor { get; set; }

    public DateTime? PublishCheckpointTimeUtc { get; set; }
  }
}
