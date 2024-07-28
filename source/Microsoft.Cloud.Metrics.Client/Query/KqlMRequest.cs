// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.KqlMRequest
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  internal sealed class KqlMRequest
  {
    public KqlMRequest()
    {
    }

    [JsonConstructor]
    public KqlMRequest(
      string monitoringAccount,
      string metricNamespace,
      string metric,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      string queryExpression)
    {
      this.MonitoringAccount = monitoringAccount;
      this.MetricNamespace = metricNamespace;
      this.Metric = metric;
      this.StartTimeUtc = startTimeUtc;
      this.EndTimeUtc = endTimeUtc;
      this.QueryExpression = queryExpression;
    }

    public string MonitoringAccount { get; private set; }

    public string MetricNamespace { get; private set; }

    public string Metric { get; private set; }

    public DateTime StartTimeUtc { get; private set; }

    public DateTime EndTimeUtc { get; private set; }

    public string QueryExpression { get; private set; }
  }
}
