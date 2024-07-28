// Decompiled with JetBrains decompiler
// Type: Nest.FieldValues
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  [JsonFormatter(typeof (FieldValuesFormatter))]
  public class FieldValues : IsADictionaryBase<string, LazyDocument>
  {
    public static readonly FieldValues Empty = new FieldValues();
    private static readonly HashSet<Type> NumericTypes = new HashSet<Type>()
    {
      typeof (int),
      typeof (double),
      typeof (Decimal),
      typeof (long),
      typeof (short),
      typeof (sbyte),
      typeof (byte),
      typeof (ulong),
      typeof (ushort),
      typeof (uint),
      typeof (float)
    };
    private readonly Inferrer _inferrer;

    protected FieldValues()
    {
    }

    internal FieldValues(Inferrer inferrer, IDictionary<string, LazyDocument> container)
      : base(container)
    {
      this._inferrer = inferrer;
    }

    public TValue Value<TValue>(Field field)
    {
      TValue[] source = this.ValuesOf<TValue>(field);
      return source == null ? default (TValue) : ((IEnumerable<TValue>) source).FirstOrDefault<TValue>();
    }

    public TValue ValueOf<T, TValue>(Expression<Func<T, TValue>> objectPath) where T : class
    {
      TValue[] source = this.Values<T, TValue>(objectPath);
      return source == null ? default (TValue) : ((IEnumerable<TValue>) source).FirstOrDefault<TValue>();
    }

    public TValue[] ValuesOf<TValue>(Field field) => this._inferrer == null ? new TValue[0] : this.FieldArray<TValue>(this._inferrer.Field(field));

    public TValue[] Values<T, TValue>(Expression<Func<T, TValue>> objectPath) where T : class => this._inferrer == null ? new TValue[0] : this.FieldArray<TValue>(this._inferrer.Field((Field) (Expression) objectPath));

    public static bool IsNumeric(Type myType)
    {
      HashSet<Type> numericTypes = FieldValues.NumericTypes;
      Type type = Nullable.GetUnderlyingType(myType);
      if ((object) type == null)
        type = myType;
      return numericTypes.Contains(type);
    }

    public static bool IsNullable(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);

    private TValue[] FieldArray<TValue>(string field)
    {
      LazyDocument lazyDocument;
      if (this.BackingDictionary == null || !this.BackingDictionary.TryGetValue(field, out lazyDocument))
        return (TValue[]) null;
      if (!FieldValues.IsNumeric(typeof (TValue)))
        return lazyDocument.As<TValue[]>();
      if (!FieldValues.IsNullable(typeof (TValue)))
        return ((IEnumerable<double>) lazyDocument.As<double[]>()).Select<double, TValue>((Func<double, TValue>) (d => (TValue) Convert.ChangeType((object) d, typeof (TValue)))).ToArray<TValue>();
      Type underlyingType = Nullable.GetUnderlyingType(typeof (TValue));
      return ((IEnumerable<double?>) lazyDocument.As<double?[]>()).Select<double?, TValue>((Func<double?, TValue>) (d => !d.HasValue ? default (TValue) : (TValue) Convert.ChangeType((object) d, underlyingType))).ToArray<TValue>();
    }
  }
}
