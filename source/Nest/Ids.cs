// Decompiled with JetBrains decompiler
// Type: Nest.Ids
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
  public class Ids : IUrlParameter, IEquatable<Ids>
  {
    private readonly List<string> _ids;

    public Ids(IEnumerable<string> value) => this._ids = value != null ? value.ToList<string>() : (List<string>) null;

    public Ids(string value)
    {
      string[] split;
      if (value.IsNullOrEmptyCommaSeparatedList(out split))
        return;
      this._ids = ((IEnumerable<string>) split).ToList<string>();
    }

    private string DebugDisplay => ((IUrlParameter) this).GetString((IConnectionConfigurationValues) null);

    public override string ToString() => this.DebugDisplay;

    public bool Equals(Ids other)
    {
      if (other == (Ids) null)
        return false;
      if (this._ids == null && other._ids == null)
        return true;
      return this._ids != null && other._ids != null && this._ids.Count == other._ids.Count && this._ids.OrderBy<string, string>((Func<string, string>) (id => id)).SequenceEqual<string>((IEnumerable<string>) other._ids.OrderBy<string, string>((Func<string, string>) (id => id)));
    }

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => string.Join(",", (IEnumerable<string>) this._ids ?? Enumerable.Empty<string>());

    public static implicit operator Ids(string value)
    {
      string[] split;
      return !value.IsNullOrEmptyCommaSeparatedList(out split) ? new Ids((IEnumerable<string>) split) : (Ids) null;
    }

    public static implicit operator Ids(string[] value) => !((IEnumerable<string>) value).IsEmpty<string>() ? new Ids((IEnumerable<string>) value) : (Ids) null;

    public override bool Equals(object obj)
    {
      Ids other = obj as Ids;
      return (object) other != null && this.Equals(other);
    }

    public override int GetHashCode()
    {
      if (this._ids == null)
        return 0;
      int hashCode = 0;
      foreach (string str in (IEnumerable<string>) this._ids.OrderBy<string, string>((Func<string, string>) (id => id)))
        hashCode = hashCode * 17 + str.GetHashCode();
      return hashCode;
    }

    public static bool operator ==(Ids left, Ids right) => object.Equals((object) left, (object) right);

    public static bool operator !=(Ids left, Ids right) => !object.Equals((object) left, (object) right);
  }
}
