// Decompiled with JetBrains decompiler
// Type: Nest.RelationName
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Diagnostics;

namespace Nest
{
  [JsonFormatter(typeof (RelationNameFormatter))]
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class RelationName : IEquatable<RelationName>, IUrlParameter
  {
    private RelationName(string type) => this.Name = type;

    private RelationName(Type type) => this.Type = type;

    public string Name { get; }

    public Type Type { get; }

    internal string DebugDisplay => !(this.Type == (Type) null) ? "RelationName for typeof: " + this.Type?.Name : this.Name;

    private static int TypeHashCode { get; } = typeof (RelationName).GetHashCode();

    public bool Equals(RelationName other) => this.EqualsMarker(other);

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => settings is IConnectionSettingsValues connectionSettingsValues ? connectionSettingsValues.Inferrer.RelationName(this) : throw new ArgumentNullException(nameof (settings), "Can not resolve RelationName if no IConnectionSettingsValues is provided");

    public static RelationName From<T>() => (RelationName) typeof (T);

    public static RelationName Create(Type type) => RelationName.GetRelationNameForType(type);

    public static RelationName Create<T>() where T : class => RelationName.GetRelationNameForType(typeof (T));

    private static RelationName GetRelationNameForType(Type type) => new RelationName(type);

    public static implicit operator RelationName(string typeName) => !typeName.IsNullOrEmpty() ? new RelationName(typeName) : (RelationName) null;

    public static implicit operator RelationName(Type type) => !(type == (Type) null) ? new RelationName(type) : (RelationName) null;

    public override int GetHashCode()
    {
      int num = RelationName.TypeHashCode * 397;
      string name = this.Name;
      int hashCode;
      if (name == null)
      {
        Type type = this.Type;
        hashCode = (object) type != null ? type.GetHashCode() : 0;
      }
      else
        hashCode = name.GetHashCode();
      return num ^ hashCode;
    }

    public static bool operator ==(RelationName left, RelationName right) => object.Equals((object) left, (object) right);

    public static bool operator !=(RelationName left, RelationName right) => !object.Equals((object) left, (object) right);

    public override bool Equals(object obj)
    {
      if (obj is string other1)
        return this.EqualsString(other1);
      RelationName other2 = obj as RelationName;
      return (object) other2 != null && this.EqualsMarker(other2);
    }

    public bool EqualsMarker(RelationName other)
    {
      if (!this.Name.IsNullOrEmpty() && other != (RelationName) null && !other.Name.IsNullOrEmpty())
        return this.EqualsString(other.Name);
      return this.Type != (Type) null && other?.Type != (Type) null && this.Type == other.Type;
    }

    private bool EqualsString(string other) => !other.IsNullOrEmpty() && other == this.Name;

    public override string ToString() => this.DebugDisplay;
  }
}
