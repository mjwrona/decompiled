// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.CompositeMetricConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class CompositeMetricConfiguration : 
    ICompositeMetricConfiguration,
    IMetricConfiguration
  {
    private readonly List<CompositeMetricSource> metricSources;
    private readonly List<CompositeExpression> compositeExpressions;

    [JsonConstructor]
    internal CompositeMetricConfiguration(
      string metricNamespace,
      string name,
      DateTime lastUpdatedTime,
      string lastUpdatedBy,
      uint version,
      bool treatMissingSeriesAsZeroes,
      string description,
      IEnumerable<CompositeMetricSource> metricSources,
      IEnumerable<CompositeExpression> compositeExpressions)
    {
      this.MetricNamespace = metricNamespace;
      this.Name = name;
      this.LastUpdatedTime = lastUpdatedTime;
      this.LastUpdatedBy = lastUpdatedBy;
      this.Version = version;
      this.TreatMissingSeriesAsZeroes = treatMissingSeriesAsZeroes;
      this.Description = description;
      this.metricSources = metricSources.ToList<CompositeMetricSource>();
      this.compositeExpressions = compositeExpressions.ToList<CompositeExpression>();
    }

    public string MetricNamespace { get; }

    public string Name { get; }

    public DateTime LastUpdatedTime { get; }

    public string LastUpdatedBy { get; }

    public uint Version { get; }

    public string Description { get; }

    public IEnumerable<CompositeMetricSource> MetricSources => (IEnumerable<CompositeMetricSource>) this.metricSources;

    public IEnumerable<CompositeExpression> CompositeExpressions => (IEnumerable<CompositeExpression>) this.compositeExpressions;

    public bool TreatMissingSeriesAsZeroes { get; set; }

    public static CompositeMetricConfiguration CreateCompositeMetricConfiguration(
      string metricNamespace,
      string metric,
      IEnumerable<CompositeMetricSource> metricSources,
      IEnumerable<CompositeExpression> expressions,
      bool treatMissingSeriesAsZeroes,
      string description = "")
    {
      if (string.IsNullOrWhiteSpace(metricNamespace))
        throw new ArgumentNullException(nameof (metricNamespace));
      if (string.IsNullOrWhiteSpace(metric))
        throw new ArgumentNullException(nameof (metric));
      if (metricSources == null)
        throw new ArgumentNullException(nameof (metricSources));
      if (expressions == null)
        throw new ArgumentNullException(nameof (expressions));
      if (description.Length > 1024)
        throw new ArgumentOutOfRangeException(nameof (description), string.Format("The metric description cannot be greater than {0} characters.", (object) 1024));
      return new CompositeMetricConfiguration(metricNamespace, metric, DateTime.MinValue, string.Empty, 0U, treatMissingSeriesAsZeroes, description, metricSources, expressions);
    }

    public void AddMetricSource(CompositeMetricSource metricSource)
    {
      if (metricSource == null)
        throw new ArgumentNullException(nameof (metricSource));
      if (this.metricSources.Any<CompositeMetricSource>((Func<CompositeMetricSource, bool>) (x => string.Equals(x.DisplayName, metricSource.DisplayName, StringComparison.OrdinalIgnoreCase))))
        throw new ConfigurationValidationException("Cannot add metric sources with duplicate names.", ValidationType.DuplicateMetricSource);
      this.metricSources.Add(metricSource);
    }

    public void RemoveMetricSource(string metricSourceName) => this.metricSources.RemoveAll((Predicate<CompositeMetricSource>) (x => string.Equals(x.DisplayName, metricSourceName, StringComparison.OrdinalIgnoreCase)));

    public void AddExpression(CompositeExpression expression)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof (expression));
      if (this.compositeExpressions.Any<CompositeExpression>((Func<CompositeExpression, bool>) (x => string.Equals(x.Name, expression.Name, StringComparison.OrdinalIgnoreCase))))
        throw new ConfigurationValidationException("Cannot add composite expressions with duplicate names.", ValidationType.DuplicateSamplingType);
      this.compositeExpressions.Add(expression);
    }

    public void RemoveExpression(string expressionName) => this.compositeExpressions.RemoveAll((Predicate<CompositeExpression>) (x => string.Equals(x.Name, expressionName, StringComparison.OrdinalIgnoreCase)));
  }
}
