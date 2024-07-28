// Decompiled with JetBrains decompiler
// Type: Nest.Indices
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nest
{
  [JsonFormatter(typeof (IndicesMultiSyntaxFormatter))]
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class Indices : Union<Indices.AllIndicesMarker, Indices.ManyIndices>, IUrlParameter
  {
    internal Indices(Indices.AllIndicesMarker all)
      : base(all)
    {
    }

    internal Indices(Indices.ManyIndices indices)
      : base(indices)
    {
    }

    internal Indices(IEnumerable<IndexName> indices)
      : base(new Indices.ManyIndices(indices))
    {
    }

    public static Indices All { get; } = new Indices(new Indices.AllIndicesMarker());

    public static Indices AllIndices { get; } = Indices.All;

    private string DebugDisplay => this.Match<string>((Func<Indices.AllIndicesMarker, string>) (all => "_all"), (Func<Indices.ManyIndices, string>) (types => string.Format("Count: {0} [", (object) types.Indices.Count) + string.Join(",", types.Indices.Select<IndexName, string>((Func<IndexName, int, string>) ((t, i) => string.Format("({0}: {1})", (object) (i + 1), (object) t.DebugDisplay)))) + "]"));

    public override string ToString() => this.DebugDisplay;

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => this.Match<string>((Func<Indices.AllIndicesMarker, string>) (all => "_all"), (Func<Indices.ManyIndices, string>) (many =>
    {
      Inferrer infer = settings is IConnectionSettingsValues connectionSettingsValues2 ? connectionSettingsValues2.Inferrer : throw new Exception("Tried to pass index names on querysting but it could not be resolved because no nest settings are available");
      return string.Join(",", many.Indices.Select<IndexName, string>((Func<IndexName, string>) (i => infer.IndexName(i))).Distinct<string>());
    }));

    public static IndexName Index(string index) => (IndexName) index;

    public static IndexName Index(IndexName index) => index;

    public static IndexName Index<T>() => (IndexName) typeof (T);

    public static Indices.ManyIndices Index(IEnumerable<IndexName> indices) => new Indices.ManyIndices(indices);

    public static Indices.ManyIndices Index(params IndexName[] indices) => new Indices.ManyIndices((IEnumerable<IndexName>) indices);

    public static Indices.ManyIndices Index(IEnumerable<string> indices) => new Indices.ManyIndices(indices);

    public static Indices.ManyIndices Index(params string[] indices) => new Indices.ManyIndices((IEnumerable<string>) indices);

    public static Indices Parse(string indicesString)
    {
      string[] split;
      if (indicesString.IsNullOrEmptyCommaSeparatedList(out split))
        return (Indices) null;
      return !((IEnumerable<string>) split).Contains<string>("_all") ? (Indices) Indices.Index(((IEnumerable<string>) split).Select<string, IndexName>((Func<string, IndexName>) (i => (IndexName) i))) : Indices.All;
    }

    public static implicit operator Indices(string indicesString) => Indices.Parse(indicesString);

    public static implicit operator Indices(Indices.ManyIndices many) => many != null ? new Indices(many) : (Indices) null;

    public static implicit operator Indices(string[] many) => (Indices) (((IEnumerable<string>) many).IsEmpty<string>() ? (Indices.ManyIndices) null : new Indices.ManyIndices((IEnumerable<string>) many));

    public static implicit operator Indices(IndexName[] many) => (Indices) (((IEnumerable<IndexName>) many).IsEmpty<IndexName>() ? (Indices.ManyIndices) null : new Indices.ManyIndices((IEnumerable<IndexName>) many));

    public static implicit operator Indices(IndexName index)
    {
      Indices.ManyIndices manyIndices;
      if (!(index == (IndexName) null))
        manyIndices = new Indices.ManyIndices((IEnumerable<IndexName>) new IndexName[1]
        {
          index
        });
      else
        manyIndices = (Indices.ManyIndices) null;
      return (Indices) manyIndices;
    }

    public static implicit operator Indices(Type type)
    {
      Indices.ManyIndices manyIndices;
      if (!(type == (Type) null))
        manyIndices = new Indices.ManyIndices((IEnumerable<IndexName>) new IndexName[1]
        {
          (IndexName) type
        });
      else
        manyIndices = (Indices.ManyIndices) null;
      return (Indices) manyIndices;
    }

    public static bool operator ==(Indices left, Indices right) => object.Equals((object) left, (object) right);

    public static bool operator !=(Indices left, Indices right) => !object.Equals((object) left, (object) right);

    public override bool Equals(object obj)
    {
      Indices other = obj as Indices;
      return (object) other != null && this.Match<bool>((Func<Indices.AllIndicesMarker, bool>) (all => other.Match<bool>((Func<Indices.AllIndicesMarker, bool>) (a => true), (Func<Indices.ManyIndices, bool>) (m => false))), (Func<Indices.ManyIndices, bool>) (many => other.Match<bool>((Func<Indices.AllIndicesMarker, bool>) (a => false), (Func<Indices.ManyIndices, bool>) (m => Indices.EqualsAllIndices(m.Indices, many.Indices)))));
    }

    private static bool EqualsAllIndices(
      IReadOnlyList<IndexName> thisIndices,
      IReadOnlyList<IndexName> otherIndices)
    {
      if (thisIndices == null && otherIndices == null)
        return true;
      return thisIndices != null && otherIndices != null && thisIndices.Count == otherIndices.Count && !thisIndices.Except<IndexName>((IEnumerable<IndexName>) otherIndices).Any<IndexName>();
    }

    public override int GetHashCode() => this.Match<int>((Func<Indices.AllIndicesMarker, int>) (all => "_all".GetHashCode()), (Func<Indices.ManyIndices, int>) (many => string.Concat<IndexName>((IEnumerable<IndexName>) many.Indices.OrderBy<IndexName, string>((Func<IndexName, string>) (i => i.ToString()))).GetHashCode()));

    public class AllIndicesMarker
    {
      internal AllIndicesMarker()
      {
      }
    }

    public class ManyIndices
    {
      private readonly List<IndexName> _indices = new List<IndexName>();

      internal ManyIndices(IEnumerable<IndexName> indices)
      {
        indices.ThrowIfEmpty<IndexName>(nameof (indices));
        this._indices.AddRange(indices);
      }

      internal ManyIndices(IEnumerable<string> indices)
      {
        indices.ThrowIfEmpty<string>(nameof (indices));
        this._indices.AddRange(indices.Select<string, IndexName>((Func<string, IndexName>) (s => (IndexName) s)));
      }

      public IReadOnlyList<IndexName> Indices => (IReadOnlyList<IndexName>) this._indices;

      public Indices.ManyIndices And<T>()
      {
        this._indices.Add((IndexName) typeof (T));
        return this;
      }
    }
  }
}
