// Decompiled with JetBrains decompiler
// Type: Nest.Name
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class Name : IEquatable<Name>, IUrlParameter
  {
    public Name(string name) => this.Value = name?.Trim();

    internal string Value { get; }

    private string DebugDisplay => this.Value;

    public override string ToString() => this.DebugDisplay;

    private static int TypeHashCode { get; } = typeof (Name).GetHashCode();

    public bool Equals(Name other) => this.EqualsString(other?.Value);

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => this.Value;

    public static implicit operator Name(string name) => !name.IsNullOrEmpty() ? new Name(name) : (Name) null;

    public static bool operator ==(Name left, Name right) => object.Equals((object) left, (object) right);

    public static bool operator !=(Name left, Name right) => !object.Equals((object) left, (object) right);

    public override bool Equals(object obj)
    {
      if (obj is string other)
        return this.EqualsString(other);
      Name name = obj as Name;
      return (object) name != null && this.EqualsString(name.Value);
    }

    private bool EqualsString(string other) => !other.IsNullOrEmpty() && other.Trim() == this.Value;

    public override int GetHashCode()
    {
      int num = Name.TypeHashCode * 397;
      string str = this.Value;
      int hashCode = str != null ? str.GetHashCode() : 0;
      return num ^ hashCode;
    }
  }
}
