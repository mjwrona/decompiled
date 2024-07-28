// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsExporter.JobDefinitionDetailsEntityResponse
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Cloud.Metrics.Client.MetricsExporter
{
  public class JobDefinitionDetailsEntityResponse
  {
    [JsonProperty("jobDetailsEntityId")]
    public string JobDetailsEntityId { get; set; }

    [JsonProperty("customerJobId")]
    public string CustomerJobId { get; set; }

    [JsonProperty("isActive")]
    public bool? IsActive { get; set; }

    [JsonProperty("batchQueryPartitionKey")]
    public string BatchQueryPartitionKey { get; set; }

    [JsonProperty("dataSourceMdmAccountInfo")]
    public string DataSourceMdmAccountInfo { get; set; }

    [JsonProperty("dataSourceMdmNamespace")]
    public string DataSourceMdmNamespace { get; set; }

    [JsonProperty("logsAccount")]
    public string LogsAccount { get; set; }

    [JsonProperty("logsVersion")]
    public string LogsVersion { get; set; }

    [JsonProperty("logsEvent")]
    public string LogsEvent { get; set; }

    [JsonProperty("logsErrorEvent")]
    public string LogsErrorEvent { get; set; }

    [JsonProperty("logsNamespace")]
    public string LogsNamespace { get; set; }

    [JsonProperty("logsRegion")]
    public string LogsRegion { get; set; }

    [JsonProperty("statusName")]
    public string StatusName { get; set; }

    [JsonProperty("lastEndTimeUtc")]
    public DateTime? LastEndTimeUtc { get; set; }

    [JsonProperty("lastStartTimeUtc")]
    public DateTime? LastStartTimeUtc { get; set; }

    [JsonProperty("lastStatusChangeUtc")]
    public DateTime? LastStatusChangeUtc { get; set; }

    [JsonProperty("nextExecutionTimeUtc")]
    public DateTime? NextExecutionTimeUtc { get; set; }

    [JsonProperty("observationWindowSeconds")]
    public int? ObservationWindowSeconds { get; set; }

    [JsonProperty("query")]
    public string Query { get; set; }

    [JsonProperty("maxExecutionDelaySeconds")]
    public int? MaxExecutionDelaySeconds { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("expirationTime")]
    public DateTime? ExpirationTime { get; set; }

    [JsonProperty("ownerEmail")]
    public string OwnerEmail { get; set; }

    [JsonProperty("failedCount")]
    public int? FailedCount { get; set; }
  }
}
