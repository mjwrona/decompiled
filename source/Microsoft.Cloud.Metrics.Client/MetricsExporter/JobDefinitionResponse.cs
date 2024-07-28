// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsExporter.JobDefinitionResponse
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.MetricsExporter
{
  public sealed class JobDefinitionResponse
  {
    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("errorMessage")]
    public IReadOnlyList<string> ErrorMessage { get; set; }

    [JsonProperty("jobId")]
    public string JobId { get; set; }

    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    [JsonProperty("jobName")]
    public string JobName { get; set; }

    [JsonProperty("dataSourceInfo")]
    public DataSourceInfo DataSourceInfo { get; set; }

    [JsonProperty("genevaLogsInfo")]
    public GenevaLogsInfo GenevaLogsInfo { get; set; }

    [JsonProperty("query")]
    public string Query { get; set; }

    [JsonProperty("partitionKey")]
    public string PartitionKey { get; set; }

    [JsonProperty("maxExecutionDelay")]
    public TimeSpan MaxExecutionDelay { get; set; }

    [JsonProperty("expirationTime")]
    public DateTime ExpirationTime { get; set; }

    [JsonProperty("status")]
    [JsonConverter(typeof (StringEnumConverter))]
    public JobStatus Status { get; set; }

    [JsonProperty("lastExecutionEndTime")]
    public DateTime LastExecutionEndTime { get; set; }

    [JsonProperty("lastExecutionStartTime")]
    public DateTime LastExecutionStartTime { get; set; }

    [JsonProperty("observationWindow")]
    public TimeSpan ObservationWindow { get; set; }

    [JsonProperty("ownerEmail")]
    public string OwnerEmail { get; set; }

    [JsonProperty("nextExecutionTime")]
    public DateTime NextExecutionTime { get; set; }
  }
}
