// Decompiled with JetBrains decompiler
// Type: Nest.Infer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nest
{
  public static class Infer
  {
    public static readonly Nest.Indices AllIndices = Nest.Indices.All;

    public static IndexName Index(IndexName index) => index;

    public static IndexName Index<T>() => (IndexName) typeof (T);

    public static IndexName Index<T>(string clusterName) => IndexName.From<T>(clusterName);

    public static Nest.Indices Indices<T>() => (Nest.Indices) typeof (T);

    public static Nest.Indices Indices(params IndexName[] indices) => (Nest.Indices) indices;

    public static Nest.Indices Indices(IEnumerable<IndexName> indices) => (Nest.Indices) indices.ToArray<IndexName>();

    public static RelationName Relation(string type) => (RelationName) type;

    public static RelationName Relation(Type type) => (RelationName) type;

    public static RelationName Relation<T>() => (RelationName) typeof (T);

    public static Routing Route<T>(T instance) where T : class => Routing.From<T>(instance);

    public static Nest.Names Names(params string[] names) => (Nest.Names) string.Join(",", names);

    public static Nest.Names Names(IEnumerable<string> names) => (Nest.Names) string.Join(",", names);

    public static Nest.Id Id<T>(T document) where T : class => Nest.Id.From<T>(document);

    public static Nest.Fields Fields<T>(params Expression<Func<T, object>>[] fields) where T : class => new Nest.Fields(((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Nest.Field>((Func<Expression<Func<T, object>>, Nest.Field>) (f => new Nest.Field((Expression) f))));

    public static Nest.Fields Fields(params string[] fields) => new Nest.Fields(((IEnumerable<string>) fields).Select<string, Nest.Field>((Func<string, Nest.Field>) (f => new Nest.Field(f))));

    public static Nest.Fields Fields(params PropertyInfo[] properties) => new Nest.Fields(((IEnumerable<PropertyInfo>) properties).Select<PropertyInfo, Nest.Field>((Func<PropertyInfo, Nest.Field>) (f => new Nest.Field(f))));

    public static Nest.Field Field<T, TValue>(
      Expression<Func<T, TValue>> path,
      double? boost = null,
      string format = null)
      where T : class
    {
      return new Nest.Field((Expression) path, boost, format);
    }

    public static Nest.Field Field<T>(
      Expression<Func<T, object>> path,
      double? boost = null,
      string format = null)
      where T : class
    {
      return new Nest.Field((Expression) path, boost, format);
    }

    public static Nest.Field Field(string field, double? boost = null, string format = null) => new Nest.Field(field, boost, format);

    public static Nest.Field Field(PropertyInfo property, double? boost = null, string format = null) => new Nest.Field(property, boost, format);

    public static PropertyName Property(string property) => (PropertyName) property;

    public static PropertyName Property<T, TValue>(Expression<Func<T, TValue>> path) where T : class => (PropertyName) (Expression) path;

    public static PropertyName Property<T>(Expression<Func<T, object>> path) where T : class => (PropertyName) (Expression) path;
  }
}
