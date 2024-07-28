// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.CompositeMetricSource
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public class CompositeMetricSource
  {
    private string displayName;
    private string monitoringAccount;
    private string metricNamespace;
    private string metric;

    public CompositeMetricSource(
      string displayName,
      string monitoringAccount,
      string metricNamespace,
      string metric)
    {
      if (string.IsNullOrWhiteSpace(displayName))
        throw new ArgumentNullException(nameof (displayName));
      if (string.IsNullOrWhiteSpace(monitoringAccount))
        throw new ArgumentNullException(nameof (monitoringAccount));
      if (string.IsNullOrWhiteSpace(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      if (string.IsNullOrWhiteSpace(metric))
        throw new ArgumentNullException(nameof (metric));
      this.displayName = displayName;
      this.monitoringAccount = monitoringAccount;
      this.metricNamespace = metricNamespace;
      this.metric = metric;
    }

    public string DisplayName
    {
      get => this.displayName;
      set => this.displayName = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof (value));
    }

    public string MonitoringAccount
    {
      get => this.monitoringAccount;
      set => this.monitoringAccount = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof (value));
    }

    public string MetricNamespace
    {
      get => this.metricNamespace;
      set => this.metricNamespace = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof (value));
    }

    public string Metric
    {
      get => this.metric;
      set => this.metric = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof (value));
    }
  }
}
