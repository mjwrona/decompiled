// Decompiled with JetBrains decompiler
// Type: Nest.IndexName
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nest
{
  [JsonFormatter(typeof (IndexNameFormatter))]
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class IndexName : IEquatable<IndexName>, IUrlParameter
  {
    private const char ClusterSeparator = ':';

    private IndexName(string index, string cluster = null)
    {
      this.Name = index;
      this.Cluster = cluster;
    }

    private IndexName(Type type, string cluster = null)
    {
      this.Type = type;
      this.Cluster = cluster;
    }

    private IndexName(string index, Type type, string cluster = null)
    {
      this.Name = index;
      this.Type = type;
      this.Cluster = cluster;
    }

    public string Cluster { get; }

    public string Name { get; }

    public Type Type { get; }

    internal string DebugDisplay => !(this.Type == (Type) null) ? "IndexName for typeof: " + this.Type?.Name : this.Name;

    private static int TypeHashCode { get; } = typeof (IndexName).GetHashCode();

    bool IEquatable<IndexName>.Equals(IndexName other) => this.EqualsMarker(other);

    public string GetString(IConnectionConfigurationValues settings) => settings is IConnectionSettingsValues connectionSettingsValues ? connectionSettingsValues.Inferrer.IndexName(this) : throw new Exception("Tried to pass index name on querystring but it could not be resolved because no nest settings are available");

    public static IndexName From<T>() => (IndexName) typeof (T);

    public static IndexName From<T>(string clusterName) => IndexName.From(typeof (T), clusterName);

    private static IndexName From(Type type, string clusterName) => new IndexName(type, clusterName);

    internal static IndexName Rebuild(string index, Type type, string clusterName = null) => new IndexName(index, type, clusterName);

    public Indices And<T>() => new Indices((IEnumerable<IndexName>) new IndexName[2]
    {
      this,
      (IndexName) typeof (T)
    });

    public Indices And<T>(string clusterName) => new Indices((IEnumerable<IndexName>) new IndexName[2]
    {
      this,
      IndexName.From(typeof (T), clusterName)
    });

    public Indices And(IndexName index) => new Indices((IEnumerable<IndexName>) new IndexName[2]
    {
      this,
      index
    });

    private static IndexName Parse(string indexName)
    {
      if (string.IsNullOrWhiteSpace(indexName))
        return (IndexName) null;
      int length = indexName.IndexOf(':');
      if (length <= -1)
        return new IndexName(indexName);
      string cluster = indexName.Substring(0, length);
      return new IndexName(indexName.Substring(length + 1), cluster);
    }

    public static implicit operator IndexName(string indexName) => IndexName.Parse(indexName);

    public static implicit operator IndexName(Type type) => !(type == (Type) null) ? new IndexName(type) : (IndexName) null;

    public override bool Equals(object obj)
    {
      if (obj is string other1)
        return this.EqualsString(other1);
      IndexName other2 = obj as IndexName;
      return (object) other2 != null && this.EqualsMarker(other2);
    }

    public override int GetHashCode()
    {
      int num1 = IndexName.TypeHashCode * 397;
      string name = this.Name;
      int hashCode1;
      if (name == null)
      {
        Type type = this.Type;
        hashCode1 = (object) type != null ? type.GetHashCode() : 0;
      }
      else
        hashCode1 = name.GetHashCode();
      int num2 = (num1 ^ hashCode1) * 397;
      string cluster = this.Cluster;
      int hashCode2 = cluster != null ? cluster.GetHashCode() : 0;
      return num2 ^ hashCode2;
    }

    public static bool operator ==(IndexName left, IndexName right) => object.Equals((object) left, (object) right);

    public static bool operator !=(IndexName left, IndexName right) => !object.Equals((object) left, (object) right);

    public override string ToString()
    {
      if (!this.Name.IsNullOrEmpty())
        return this.PrefixClusterName(this.Name);
      return !(this.Type != (Type) null) ? string.Empty : this.PrefixClusterName(this.Type.Name);
    }

    private string PrefixClusterName(string name) => IndexName.PrefixClusterName(this, name);

    private static string PrefixClusterName(IndexName index, string name) => !index.Cluster.IsNullOrEmpty() ? index.Cluster + ":" + name : name;

    private bool EqualsString(string other) => !other.IsNullOrEmpty() && other == this.PrefixClusterName(this.Name);

    private bool EqualsMarker(IndexName other)
    {
      if (other == (IndexName) null)
        return false;
      if (!this.Name.IsNullOrEmpty() && !other.Name.IsNullOrEmpty())
        return this.EqualsString(IndexName.PrefixClusterName(other, other.Name));
      return (this.Cluster.IsNullOrEmpty() && other.Cluster.IsNullOrEmpty() || !(this.Cluster != other.Cluster)) && this.Type != (Type) null && other?.Type != (Type) null && this.Type == other.Type;
    }
  }
}
