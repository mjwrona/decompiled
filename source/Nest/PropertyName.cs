// Decompiled with JetBrains decompiler
// Type: Nest.PropertyName
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Nest
{
  [JsonFormatter(typeof (PropertyNameFormatter))]
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class PropertyName : IEquatable<PropertyName>, IUrlParameter
  {
    private readonly object _comparisonValue;
    private readonly Type _type;

    public PropertyName(string name)
    {
      this.Name = name;
      this._comparisonValue = (object) name;
    }

    public PropertyName(Expression expression)
    {
      this.Expression = expression;
      Type type;
      bool cachable;
      this._comparisonValue = expression.ComparisonValueFromExpression(out type, out cachable);
      this.CacheableExpression = cachable;
      this._type = type;
    }

    public PropertyName(PropertyInfo property)
    {
      this.Property = property;
      this._comparisonValue = (object) property;
      this._type = property.DeclaringType;
    }

    public bool CacheableExpression { get; }

    public Expression Expression { get; }

    public string Name { get; }

    public PropertyInfo Property { get; }

    internal string DebugDisplay => (this.Expression?.ToString() ?? this.PropertyDebug ?? this.Name) + (this._type == (Type) null ? "" : " typeof: " + this._type.Name);

    public override string ToString() => this.DebugDisplay;

    private string PropertyDebug => !(this.Property == (PropertyInfo) null) ? "PropertyInfo: " + this.Property.Name : (string) null;

    private static int TypeHashCode { get; } = typeof (PropertyName).GetHashCode();

    public bool Equals(PropertyName other) => this.EqualsMarker(other);

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => settings is IConnectionSettingsValues connectionSettingsValues ? connectionSettingsValues.Inferrer.PropertyName(this) : throw new ArgumentNullException(nameof (settings), "Can not resolve PropertyName if no IConnectionSettingsValues is provided");

    public static implicit operator PropertyName(string name) => !name.IsNullOrEmpty() ? new PropertyName(name) : (PropertyName) null;

    public static implicit operator PropertyName(Expression expression) => expression != null ? new PropertyName(expression) : (PropertyName) null;

    public static implicit operator PropertyName(PropertyInfo property) => !(property == (PropertyInfo) null) ? new PropertyName(property) : (PropertyName) null;

    public override int GetHashCode()
    {
      int num1 = PropertyName.TypeHashCode * 397;
      object comparisonValue = this._comparisonValue;
      int hashCode1 = comparisonValue != null ? comparisonValue.GetHashCode() : 0;
      int num2 = (num1 ^ hashCode1) * 397;
      Type type = this._type;
      int hashCode2 = (object) type != null ? type.GetHashCode() : 0;
      return num2 ^ hashCode2;
    }

    public override bool Equals(object obj)
    {
      if (obj is string other1)
        return this.EqualsString(other1);
      PropertyName other2 = obj as PropertyName;
      return (object) other2 != null && this.EqualsMarker(other2);
    }

    private bool EqualsString(string other) => !other.IsNullOrEmpty() && other == this.Name;

    public bool EqualsMarker(PropertyName other) => !(this._type != (Type) null) ? other != (PropertyName) null && this._comparisonValue.Equals(other._comparisonValue) : other != (PropertyName) null && this._type == other._type && this._comparisonValue.Equals(other._comparisonValue);

    public static bool operator ==(PropertyName left, PropertyName right) => object.Equals((object) left, (object) right);

    public static bool operator !=(PropertyName left, PropertyName right) => !object.Equals((object) left, (object) right);
  }
}
