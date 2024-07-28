// Decompiled with JetBrains decompiler
// Type: Nest.IndexMetrics
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class IndexMetrics : IEquatable<IndexMetrics>, IUrlParameter
  {
    private readonly NodesStatsIndexMetric _enumValue;

    internal IndexMetrics(NodesStatsIndexMetric metric) => this._enumValue = metric;

    internal Enum Value => (Enum) this._enumValue;

    public bool Equals(IndexMetrics other) => this.Value.Equals((object) other.Value);

    public string GetString(IConnectionConfigurationValues settings) => this._enumValue.GetStringValue();

    public static implicit operator IndexMetrics(NodesStatsIndexMetric metric) => new IndexMetrics(metric);

    public bool Equals(Enum other) => this.Value.Equals((object) other);

    public override bool Equals(object obj)
    {
      if (obj is Enum other)
        return this.Equals(other);
      IndexMetrics indexMetrics = obj as IndexMetrics;
      return (object) indexMetrics != null && this.Equals(indexMetrics.Value);
    }

    public override int GetHashCode() => this._enumValue.GetHashCode();

    public static bool operator ==(IndexMetrics left, IndexMetrics right) => object.Equals((object) left, (object) right);

    public static bool operator !=(IndexMetrics left, IndexMetrics right) => !object.Equals((object) left, (object) right);
  }
}
