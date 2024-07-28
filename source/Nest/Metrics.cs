// Decompiled with JetBrains decompiler
// Type: Nest.Metrics
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class Metrics : IEquatable<Metrics>, IUrlParameter
  {
    internal Metrics(IndicesStatsMetric metric) => this.Value = (Enum) metric;

    internal Metrics(NodesStatsMetric metric) => this.Value = (Enum) metric;

    internal Metrics(NodesInfoMetric metric) => this.Value = (Enum) metric;

    internal Metrics(ClusterStateMetric metric) => this.Value = (Enum) metric;

    internal Metrics(WatcherStatsMetric metric) => this.Value = (Enum) metric;

    internal Metrics(NodesUsageMetric metric) => this.Value = (Enum) metric;

    internal Enum Value { get; }

    public bool Equals(Metrics other) => this.Value.Equals((object) other.Value);

    public string GetString(IConnectionConfigurationValues settings) => this.Value.GetStringValue();

    public static implicit operator Metrics(IndicesStatsMetric metric) => new Metrics(metric);

    public static implicit operator Metrics(NodesStatsMetric metric) => new Metrics(metric);

    public static implicit operator Metrics(NodesInfoMetric metric) => new Metrics(metric);

    public static implicit operator Metrics(ClusterStateMetric metric) => new Metrics(metric);

    public static implicit operator Metrics(WatcherStatsMetric metric) => new Metrics(metric);

    public static implicit operator Metrics(NodesUsageMetric metric) => new Metrics(metric);

    public bool Equals(Enum other) => this.Value.Equals((object) other);

    public override bool Equals(object obj)
    {
      if (obj is Enum other)
        return this.Equals(other);
      Metrics metrics = obj as Metrics;
      return (object) metrics != null && this.Equals(metrics.Value);
    }

    public override int GetHashCode() => this.Value == null ? 0 : this.Value.GetHashCode();

    public static bool operator ==(Metrics left, Metrics right) => object.Equals((object) left, (object) right);

    public static bool operator !=(Metrics left, Metrics right) => !object.Equals((object) left, (object) right);
  }
}
