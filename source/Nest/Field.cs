// Decompiled with JetBrains decompiler
// Type: Nest.Field
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Nest
{
  [JsonFormatter(typeof (FieldFormatter))]
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class Field : IEquatable<Field>, IUrlParameter
  {
    private readonly object _comparisonValue;
    private readonly Type _type;

    public Field(string name, double? boost = null, string format = null)
    {
      name.ThrowIfNullOrEmpty(nameof (name));
      double? boost1;
      this.Name = Field.ParseFieldName(name, out boost1);
      this.Boost = boost1 ?? boost;
      this.Format = format;
      this._comparisonValue = (object) this.Name;
    }

    public Field(Expression expression, double? boost = null, string format = null)
    {
      this.Expression = expression ?? throw new ArgumentNullException(nameof (expression));
      this.Boost = boost;
      this.Format = format;
      Type type;
      bool cachable;
      this._comparisonValue = expression.ComparisonValueFromExpression(out type, out cachable);
      this._type = type;
      this.CachableExpression = cachable;
    }

    public Field(PropertyInfo property, double? boost = null, string format = null)
    {
      this.Property = property ?? throw new ArgumentNullException(nameof (property));
      this.Boost = boost;
      this.Format = format;
      this._comparisonValue = (object) property;
      this._type = property.DeclaringType;
    }

    public double? Boost { get; set; }

    public string Format { get; set; }

    public bool CachableExpression { get; }

    public Expression Expression { get; }

    public string Name { get; }

    public PropertyInfo Property { get; }

    internal string DebugDisplay => (this.Expression?.ToString() ?? this.PropertyDebug ?? this.Name) + (this.Boost.HasValue ? "^" + this.Boost.Value.ToString() : string.Empty) + (!string.IsNullOrEmpty(this.Format) ? " format: " + this.Format : string.Empty) + (this._type == (Type) null ? string.Empty : " typeof: " + this._type.Name);

    public override string ToString() => this.DebugDisplay;

    private string PropertyDebug => !(this.Property == (PropertyInfo) null) ? "PropertyInfo: " + this.Property.Name : (string) null;

    public bool Equals(Field other) => !(this._type != (Type) null) ? other != (Field) null && this._comparisonValue.Equals(other._comparisonValue) : other != (Field) null && this._type == other._type && this._comparisonValue.Equals(other._comparisonValue);

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => settings is IConnectionSettingsValues connectionSettingsValues ? connectionSettingsValues.Inferrer.Field(this) : throw new ArgumentNullException(nameof (settings), "Can not resolve Field if no IConnectionSettingsValues is provided");

    public Fields And(Field field) => new Fields((IEnumerable<Field>) new Field[2]
    {
      this,
      field
    });

    public Fields And<T, TValue>(Expression<Func<T, TValue>> field, double? boost = null, string format = null) where T : class => new Fields((IEnumerable<Field>) new Field[2]
    {
      this,
      new Field((Expression) field, boost, format)
    });

    public Fields And<T>(Expression<Func<T, object>> field, double? boost = null, string format = null) where T : class => new Fields((IEnumerable<Field>) new Field[2]
    {
      this,
      new Field((Expression) field, boost, format)
    });

    public Fields And(string field, double? boost = null, string format = null) => new Fields((IEnumerable<Field>) new Field[2]
    {
      this,
      new Field(field, boost, format)
    });

    public Fields And(PropertyInfo property, double? boost = null, string format = null) => new Fields((IEnumerable<Field>) new Field[2]
    {
      this,
      new Field(property, boost, format)
    });

    private static string ParseFieldName(string name, out double? boost)
    {
      boost = new double?();
      if (name == null)
        return (string) null;
      if (name.IndexOf('^') == -1)
        return name;
      string[] strArray = name.Split(new char[1]{ '^' }, 2, StringSplitOptions.RemoveEmptyEntries);
      name = strArray[0];
      boost = new double?(double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture));
      return name;
    }

    public static implicit operator Field(string name) => !name.IsNullOrEmpty() ? new Field(name) : (Field) null;

    public static implicit operator Field(Expression expression) => expression != null ? new Field(expression) : (Field) null;

    public static implicit operator Field(PropertyInfo property) => !(property == (PropertyInfo) null) ? new Field(property) : (Field) null;

    public override int GetHashCode()
    {
      object comparisonValue = this._comparisonValue;
      int num = (comparisonValue != null ? comparisonValue.GetHashCode() : 0) * 397;
      Type type = this._type;
      int hashCode = (object) type != null ? type.GetHashCode() : 0;
      return num ^ hashCode;
    }

    public override bool Equals(object obj)
    {
      if (obj is string other1)
        return this.Equals((Field) other1);
      PropertyInfo other2 = obj as PropertyInfo;
      if ((object) other2 != null)
        return this.Equals((Field) other2);
      Field other3 = obj as Field;
      return (object) other3 != null && this.Equals(other3);
    }

    public static bool operator ==(Field x, Field y) => object.Equals((object) x, (object) y);

    public static bool operator !=(Field x, Field y) => !object.Equals((object) x, (object) y);
  }
}
