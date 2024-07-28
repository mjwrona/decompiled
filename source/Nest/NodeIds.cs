// Decompiled with JetBrains decompiler
// Type: Nest.NodeIds
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class NodeIds : IEquatable<NodeIds>, IUrlParameter
  {
    public NodeIds(IEnumerable<string> nodeIds)
    {
      this.Value = nodeIds != null ? (IList<string>) nodeIds.ToList<string>() : (IList<string>) null;
      if (!this.Value.HasAny<string>())
        throw new ArgumentException("can not create NodeIds on an empty enumerable of ", nameof (nodeIds));
    }

    internal IList<string> Value { get; }

    private string DebugDisplay => ((IUrlParameter) this).GetString((IConnectionConfigurationValues) null);

    public override string ToString() => this.DebugDisplay;

    public bool Equals(NodeIds other) => NodeIds.EqualsAllIds((ICollection<string>) this.Value, (ICollection<string>) other.Value);

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => string.Join(",", (IEnumerable<string>) this.Value);

    public static NodeIds Parse(string nodeIds)
    {
      string[] split;
      return !nodeIds.IsNullOrEmptyCommaSeparatedList(out split) ? new NodeIds((IEnumerable<string>) split) : (NodeIds) null;
    }

    public static implicit operator NodeIds(string nodes) => NodeIds.Parse(nodes);

    public static implicit operator NodeIds(string[] nodes) => !((IEnumerable<string>) nodes).IsEmpty<string>() ? new NodeIds((IEnumerable<string>) nodes) : (NodeIds) null;

    public static bool operator ==(NodeIds left, NodeIds right) => object.Equals((object) left, (object) right);

    public static bool operator !=(NodeIds left, NodeIds right) => !object.Equals((object) left, (object) right);

    private static bool EqualsAllIds(ICollection<string> thisIds, ICollection<string> otherIds)
    {
      if (thisIds == null && otherIds == null)
        return true;
      return thisIds != null && otherIds != null && thisIds.Count == otherIds.Count && thisIds.Count == otherIds.Count && !thisIds.Except<string>((IEnumerable<string>) otherIds).Any<string>();
    }

    public override bool Equals(object obj)
    {
      if (obj is string nodeIds)
        return this.Equals(NodeIds.Parse(nodeIds));
      NodeIds other = obj as NodeIds;
      return (object) other != null && this.Equals(other);
    }

    public override int GetHashCode() => this.Value.GetHashCode();
  }
}
