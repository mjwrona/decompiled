// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricEnrichmentRuleManagement.MetricEnrichmentRule
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.MetricEnrichmentRuleManagement
{
  public sealed class MetricEnrichmentRule
  {
    public MetricEnrichmentRule(
      string stampId,
      string monitoringAccountFilter,
      string metricNamespaceFilter,
      string metricNameFilter,
      List<MetricEnrichmentRuleTransformationDefinition> transformations)
    {
      if (string.IsNullOrEmpty(stampId))
        throw new ArgumentNullException(nameof (stampId));
      if (string.IsNullOrEmpty(monitoringAccountFilter))
        throw new ArgumentNullException(nameof (monitoringAccountFilter));
      if (string.IsNullOrEmpty(metricNamespaceFilter))
        throw new ArgumentNullException(nameof (metricNamespaceFilter));
      if (string.IsNullOrEmpty(metricNameFilter))
        throw new ArgumentNullException(nameof (metricNameFilter));
      this.StampId = stampId;
      this.MonitoringAccountFilter = monitoringAccountFilter;
      this.MetricNamespaceFilter = metricNamespaceFilter;
      this.MetricNameFilter = metricNameFilter;
      this.Transformations = transformations;
    }

    public string StampId { get; private set; }

    public string MonitoringAccountFilter { get; private set; }

    public string MetricNamespaceFilter { get; private set; }

    public string MetricNameFilter { get; private set; }

    public List<MetricEnrichmentRuleTransformationDefinition> Transformations { get; private set; }

    internal string Validate()
    {
      if (string.IsNullOrEmpty(this.StampId))
        return "Stamp id cannot be null";
      if (string.IsNullOrEmpty(this.MonitoringAccountFilter))
        return "MonitoringAccountFilter cannot be null";
      if (string.IsNullOrEmpty(this.MetricNamespaceFilter))
        return "MetricNamespaceFilter cannot be null";
      if (string.IsNullOrEmpty(this.MetricNameFilter))
        return "MetricNameFilter cannot be null";
      if (this.Transformations == null || this.Transformations.Count == 0)
        return "Transformations cannot be null or empty";
      foreach (MetricEnrichmentRuleTransformationDefinition transformation in this.Transformations)
      {
        string str = transformation.Validate();
        if (!string.IsNullOrEmpty(str))
          return str;
      }
      return string.Empty;
    }
  }
}
