// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.StreamingRuleRequest
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public sealed class StreamingRuleRequest
  {
    [JsonProperty("ruleNamespace")]
    public string RuleNamespace { get; set; }

    [JsonProperty("ruleName")]
    public string RuleName { get; set; }

    [JsonProperty("serviceTreeId")]
    public string ServiceTreeId { get; set; }

    [JsonProperty(PropertyName = "mdmAccountInfo")]
    public string MdmAccountInfo { get; set; }

    [JsonProperty(PropertyName = "homeStampOverride")]
    public string HomeStampOverride { get; set; }

    [JsonProperty(PropertyName = "contactEmail")]
    public string ContactEmail { get; set; }

    [JsonProperty(PropertyName = "metricNamespace")]
    public string MetricNamespace { get; set; }

    [JsonProperty("queryStatement")]
    public string QueryStatement { get; set; }

    [JsonProperty("sinkInfo")]
    public string SinkInfo { get; set; }

    [JsonProperty("stabilizationPeriodSecs")]
    public int StabilizationPeriodSecs { get; set; }

    [JsonProperty("queryParameters")]
    public Dictionary<string, string> QueryParameters { get; set; }

    [JsonProperty("customProperties")]
    public Dictionary<string, string> CustomProperties { get; set; }

    [JsonProperty("endTimeUTC")]
    public DateTime EndTimeUTC { get; set; }

    [JsonProperty("isDisabled")]
    public bool IsDisabled { get; set; }

    [JsonProperty("streamingBloomfilterId")]
    public string StreamingBloomfilterId { get; set; }

    [JsonProperty("etag")]
    public string Etag { get; set; }
  }
}
