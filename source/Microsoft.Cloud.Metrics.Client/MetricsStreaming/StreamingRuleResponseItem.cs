// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.StreamingRuleResponseItem
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public class StreamingRuleResponseItem
  {
    public string Id { get; set; }

    public int SchemaVersion { get; set; }

    public string RuleNamespace { get; set; }

    public string RuleName { get; set; }

    public string ServiceTreeId { get; set; }

    public int RuleVersion { get; set; }

    public string MdmAccountInfo { get; set; }

    public string HomeStampOverride { get; set; }

    public string ContactEmail { get; set; }

    public string MetricNamespace { get; set; }

    public string QueryStatement { get; set; }

    public int StabilizationPeriodSecs { get; set; }

    public Dictionary<string, string> QueryParameters { get; set; }

    public string SinkInfo { get; set; }

    public Dictionary<string, string> CustomProperties { get; set; }

    public DateTime EndTimeUTC { get; set; }

    public DateTime LastUpdatedUTC { get; set; }

    public bool IsDisabled { get; set; }

    public string StreamingBloomfilterId { get; set; }

    public string Etag { get; set; }

    public StreamingRuleRequest ConvertToRequest() => new StreamingRuleRequest()
    {
      RuleNamespace = this.RuleNamespace,
      RuleName = this.RuleName,
      ServiceTreeId = this.ServiceTreeId,
      ContactEmail = this.ContactEmail,
      MdmAccountInfo = this.MdmAccountInfo,
      HomeStampOverride = this.HomeStampOverride,
      MetricNamespace = this.MetricNamespace,
      QueryStatement = this.QueryStatement,
      QueryParameters = this.QueryParameters,
      StabilizationPeriodSecs = this.StabilizationPeriodSecs,
      SinkInfo = this.SinkInfo,
      EndTimeUTC = this.EndTimeUTC,
      CustomProperties = this.CustomProperties,
      IsDisabled = this.IsDisabled,
      Etag = this.Etag,
      StreamingBloomfilterId = this.StreamingBloomfilterId
    };
  }
}
