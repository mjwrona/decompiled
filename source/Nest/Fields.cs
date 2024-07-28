// Decompiled with JetBrains decompiler
// Type: Nest.Fields
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nest
{
  [JsonFormatter(typeof (FieldsFormatter))]
  [DebuggerDisplay("{DebugDisplay,nq}")]
  public class Fields : IUrlParameter, IEnumerable<Field>, IEnumerable, IEquatable<Fields>
  {
    internal readonly List<Field> ListOfFields;

    internal Fields() => this.ListOfFields = new List<Field>();

    internal Fields(IEnumerable<Field> fieldNames) => this.ListOfFields = fieldNames.ToList<Field>();

    private string DebugDisplay => string.Format("Count: {0} [", (object) this.ListOfFields.Count) + string.Join(",", this.ListOfFields.Select<Field, string>((Func<Field, int, string>) ((t, i) => string.Format("({0}: {1})", (object) (i + 1), (object) (t?.DebugDisplay ?? "NULL"))))) + "]";

    public override string ToString() => this.DebugDisplay;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<Field> GetEnumerator() => (IEnumerator<Field>) this.ListOfFields.GetEnumerator();

    public bool Equals(Fields other) => Fields.EqualsAllFields((IReadOnlyList<Field>) this.ListOfFields, (IReadOnlyList<Field>) other.ListOfFields);

    string IUrlParameter.GetString(IConnectionConfigurationValues settings) => string.Join(",", this.ListOfFields.Where<Field>((Func<Field, bool>) (f => f != (Field) null)).Select<Field, string>((Func<Field, string>) (f => ((IUrlParameter) f).GetString((IConnectionConfigurationValues) (settings as IConnectionSettingsValues ?? throw new ArgumentNullException(nameof (settings), "Can not resolve Fields if no IConnectionSettingsValues is provided"))))));

    public static implicit operator Fields(string[] fields) => !((IEnumerable<string>) fields).IsEmpty<string>() ? new Fields(((IEnumerable<string>) fields).Select<string, Field>((Func<string, Field>) (f => new Field(f)))) : (Fields) null;

    public static implicit operator Fields(string field)
    {
      string[] split;
      return !field.IsNullOrEmptyCommaSeparatedList(out split) ? new Fields(((IEnumerable<string>) split).Select<string, Field>((Func<string, Field>) (f => new Field(f)))) : (Fields) null;
    }

    public static implicit operator Fields(Expression[] fields) => !((IEnumerable<Expression>) fields).IsEmpty<Expression>() ? new Fields(((IEnumerable<Expression>) fields).Select<Expression, Field>((Func<Expression, Field>) (f => new Field(f)))) : (Fields) null;

    public static implicit operator Fields(Expression field)
    {
      if (field == null)
        return (Fields) null;
      return new Fields((IEnumerable<Field>) new Field[1]
      {
        new Field(field)
      });
    }

    public static implicit operator Fields(Field field)
    {
      if (field == (Field) null)
        return (Fields) null;
      return new Fields((IEnumerable<Field>) new Field[1]
      {
        field
      });
    }

    public static implicit operator Fields(PropertyInfo field)
    {
      if (field == (PropertyInfo) null)
        return (Fields) null;
      return new Fields((IEnumerable<Field>) new Field[1]
      {
        (Field) field
      });
    }

    public static implicit operator Fields(PropertyInfo[] fields) => !((IEnumerable<PropertyInfo>) fields).IsEmpty<PropertyInfo>() ? new Fields(((IEnumerable<PropertyInfo>) fields).Select<PropertyInfo, Field>((Func<PropertyInfo, Field>) (f => new Field(f)))) : (Fields) null;

    public static implicit operator Fields(Field[] fields) => !((IEnumerable<Field>) fields).IsEmpty<Field>() ? new Fields((IEnumerable<Field>) fields) : (Fields) null;

    public Fields And<T, TValue>(Expression<Func<T, TValue>> field, double? boost = null, string format = null) where T : class
    {
      this.ListOfFields.Add(new Field((Expression) field, boost, format));
      return this;
    }

    public Fields And(string field, double? boost = null, string format = null)
    {
      this.ListOfFields.Add(new Field(field, boost, format));
      return this;
    }

    public Fields And(PropertyInfo property, double? boost = null)
    {
      this.ListOfFields.Add(new Field(property, boost));
      return this;
    }

    public Fields And<T>(params Expression<Func<T, object>>[] fields) where T : class
    {
      this.ListOfFields.AddRange(((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (f => new Field((Expression) f))));
      return this;
    }

    public Fields And(params string[] fields)
    {
      this.ListOfFields.AddRange(((IEnumerable<string>) fields).Select<string, Field>((Func<string, Field>) (f => new Field(f))));
      return this;
    }

    public Fields And(params PropertyInfo[] properties)
    {
      this.ListOfFields.AddRange(((IEnumerable<PropertyInfo>) properties).Select<PropertyInfo, Field>((Func<PropertyInfo, Field>) (f => new Field(f))));
      return this;
    }

    public Fields And(params Field[] fields)
    {
      this.ListOfFields.AddRange((IEnumerable<Field>) fields);
      return this;
    }

    public static bool operator ==(Fields left, Fields right) => object.Equals((object) left, (object) right);

    public static bool operator !=(Fields left, Fields right) => !object.Equals((object) left, (object) right);

    public override bool Equals(object obj)
    {
      Fields other1 = obj as Fields;
      if ((object) other1 != null)
        return this.Equals(other1);
      if (obj is string other2)
        return this.Equals((Fields) other2);
      Field other3 = obj as Field;
      if ((object) other3 != null)
        return this.Equals((Fields) other3);
      switch (obj)
      {
        case Field[] other4:
          return this.Equals((Fields) other4);
        case Expression other5:
          return this.Equals((Fields) other5);
        case Expression[] other6:
          return this.Equals((Fields) other6);
        default:
          return false;
      }
    }

    private static bool EqualsAllFields(
      IReadOnlyList<Field> thisTypes,
      IReadOnlyList<Field> otherTypes)
    {
      if (thisTypes == null && otherTypes == null)
        return true;
      return thisTypes != null && otherTypes != null && thisTypes.Count == otherTypes.Count && thisTypes.Count == otherTypes.Count && !thisTypes.Except<Field>((IEnumerable<Field>) otherTypes).Any<Field>();
    }

    public override int GetHashCode() => this.ListOfFields.GetHashCode();
  }
}
