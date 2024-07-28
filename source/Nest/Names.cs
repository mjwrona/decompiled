// Decompiled with JetBrains decompiler
// Type: Nest.Names
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
  public class Names : IEquatable<Names>, IUrlParameter
  {
    public Names(IEnumerable<string> names)
      : this(names != null ? (IEnumerable<Name>) names.Select<string, Name>((Func<string, Name>) (n => (Name) n)).ToList<Name>() : (IEnumerable<Name>) null)
    {
    }

    public Names(IEnumerable<Name> names)
    {
      this.Value = names != null ? (IList<Name>) names.ToList<Name>() : (IList<Name>) null;
      if (!this.Value.HasAny<Name>())
        throw new ArgumentException("can not create Names on an empty enumerable of ", nameof (names));
    }

    internal IList<Name> Value { get; }

    private string DebugDisplay => ((IUrlParameter) this).GetString((IConnectionConfigurationValues) null);

    public override string ToString() => this.DebugDisplay;

    public bool Equals(Names other) => Names.EqualsAllIds((ICollection<Name>) this.Value, (ICollection<Name>) other.Value);

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => string.Join(",", this.Value.Cast<IUrlParameter>().Select<IUrlParameter, string>((Func<IUrlParameter, string>) (n => n.GetString(settings))));

    public static Names Parse(string names)
    {
      string[] split;
      return !names.IsNullOrEmptyCommaSeparatedList(out split) ? new Names((IEnumerable<string>) split) : (Names) null;
    }

    public static implicit operator Names(Name name)
    {
      if (name == (Name) null)
        return (Names) null;
      return new Names((IEnumerable<Name>) new Name[1]
      {
        name
      });
    }

    public static implicit operator Names(string names) => Names.Parse(names);

    public static implicit operator Names(string[] names) => !((IEnumerable<string>) names).IsEmpty<string>() ? new Names((IEnumerable<string>) names) : (Names) null;

    public static bool operator ==(Names left, Names right) => object.Equals((object) left, (object) right);

    public static bool operator !=(Names left, Names right) => !object.Equals((object) left, (object) right);

    private static bool EqualsAllIds(ICollection<Name> thisIds, ICollection<Name> otherIds)
    {
      if (thisIds == null && otherIds == null)
        return true;
      return thisIds != null && otherIds != null && thisIds.Count == otherIds.Count && thisIds.Count == otherIds.Count && !thisIds.Except<Name>((IEnumerable<Name>) otherIds).Any<Name>();
    }

    public override bool Equals(object obj)
    {
      if (obj is string names)
        return this.Equals(Names.Parse(names));
      Names other = obj as Names;
      return (object) other != null && this.Equals(other);
    }

    public override int GetHashCode() => this.Value.GetHashCode();
  }
}
