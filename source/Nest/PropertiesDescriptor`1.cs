// Decompiled with JetBrains decompiler
// Type: Nest.PropertiesDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class PropertiesDescriptor<T> : 
    IsADictionaryDescriptorBase<PropertiesDescriptor<T>, IProperties, PropertyName, IProperty>,
    IPropertiesDescriptor<T, PropertiesDescriptor<T>>
    where T : class
  {
    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, int>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<int>(field).Type(new NumberType?(NumberType.Integer))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<int>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<int>>(field).Type(new NumberType?(NumberType.Integer))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, int?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<int?>(field).Type(new NumberType?(NumberType.Integer))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<int?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<int?>>(field).Type(new NumberType?(NumberType.Integer))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, float>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<float>(field).Type(new NumberType?(NumberType.Float))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<float>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<float>>(field).Type(new NumberType?(NumberType.Float))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, float?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<float?>(field).Type(new NumberType?(NumberType.Float))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<float?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<float?>>(field).Type(new NumberType?(NumberType.Float))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, sbyte>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<sbyte>(field).Type(new NumberType?(NumberType.Byte))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, sbyte?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<sbyte?>(field).Type(new NumberType?(NumberType.Byte))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<sbyte>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<sbyte>>(field).Type(new NumberType?(NumberType.Byte))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<sbyte?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<sbyte?>>(field).Type(new NumberType?(NumberType.Byte))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, short>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<short>(field).Type(new NumberType?(NumberType.Short))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, short?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<short?>(field).Type(new NumberType?(NumberType.Short))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<short>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<short>>(field).Type(new NumberType?(NumberType.Short))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<short?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<short?>>(field).Type(new NumberType?(NumberType.Short))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, byte>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<byte>(field).Type(new NumberType?(NumberType.Short))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, byte?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<byte?>(field).Type(new NumberType?(NumberType.Short))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<byte>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<byte>>(field).Type(new NumberType?(NumberType.Short))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<byte?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<byte?>>(field).Type(new NumberType?(NumberType.Short))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, long>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<long>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, long?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<long?>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<long>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<long>>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<long?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<long?>>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, uint>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<uint>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, uint?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<uint?>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<uint>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<uint>>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<uint?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<uint?>>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, TimeSpan>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<TimeSpan>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, TimeSpan?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<TimeSpan?>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<TimeSpan>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<TimeSpan>>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<TimeSpan?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<TimeSpan?>>(field).Type(new NumberType?(NumberType.Long))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Decimal>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<Decimal>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Decimal?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<Decimal?>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<Decimal>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<Decimal>>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<Decimal?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<Decimal?>>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, ulong>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<ulong>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, ulong?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<ulong?>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<ulong>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<ulong>>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<ulong?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<ulong?>>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, double>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<double>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, double?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<double?>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<double>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<double>>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<double?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<double?>>(field).Type(new NumberType?(NumberType.Double))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Enum>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<Enum>(field).Type(new NumberType?(NumberType.Integer))));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, DateTime>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<DateTime>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, DateTime?>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<DateTime?>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<DateTime>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<IEnumerable<DateTime>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<DateTime?>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<IEnumerable<DateTime?>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, DateTimeOffset>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<DateTimeOffset>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, DateTimeOffset?>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<DateTimeOffset?>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<DateTimeOffset>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<IEnumerable<DateTimeOffset>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<DateTimeOffset?>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<IEnumerable<DateTimeOffset?>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, bool>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<BooleanPropertyDescriptor<T>, IBooleanProperty>(new BooleanPropertyDescriptor<T>().Name<bool>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, bool?>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<BooleanPropertyDescriptor<T>, IBooleanProperty>(new BooleanPropertyDescriptor<T>().Name<bool?>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<bool>>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<BooleanPropertyDescriptor<T>, IBooleanProperty>(new BooleanPropertyDescriptor<T>().Name<IEnumerable<bool>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<bool?>>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<BooleanPropertyDescriptor<T>, IBooleanProperty>(new BooleanPropertyDescriptor<T>().Name<IEnumerable<bool?>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, char>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<char>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, char?>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<char?>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<char>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<IEnumerable<char>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<char?>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<IEnumerable<char?>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Guid>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<Guid>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Guid?>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<Guid?>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<Guid>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<IEnumerable<Guid>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<Guid?>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<IEnumerable<Guid?>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, string>> field,
      Func<TextPropertyDescriptor<T>, ITextProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<TextPropertyDescriptor<T>, ITextProperty>(new TextPropertyDescriptor<T>().Name<string>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IEnumerable<string>>> field,
      Func<TextPropertyDescriptor<T>, ITextProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<TextPropertyDescriptor<T>, ITextProperty>(new TextPropertyDescriptor<T>().Name<IEnumerable<string>>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Nest.DateRange>> field,
      Func<DateRangePropertyDescriptor<T>, IDateRangeProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DateRangePropertyDescriptor<T>, IDateRangeProperty>(new DateRangePropertyDescriptor<T>().Name<Nest.DateRange>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Nest.DoubleRange>> field,
      Func<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty>(new DoubleRangePropertyDescriptor<T>().Name<Nest.DoubleRange>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Nest.LongRange>> field,
      Func<LongRangePropertyDescriptor<T>, ILongRangeProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<LongRangePropertyDescriptor<T>, ILongRangeProperty>(new LongRangePropertyDescriptor<T>().Name<Nest.LongRange>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Nest.IntegerRange>> field,
      Func<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty>(new IntegerRangePropertyDescriptor<T>().Name<Nest.IntegerRange>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, Nest.FloatRange>> field,
      Func<FloatRangePropertyDescriptor<T>, IFloatRangeProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<FloatRangePropertyDescriptor<T>, IFloatRangeProperty>(new FloatRangePropertyDescriptor<T>().Name<Nest.FloatRange>(field)));
    }

    public PropertiesDescriptor<T> Scalar(
      Expression<Func<T, IpAddressRange>> field,
      Func<IpRangePropertyDescriptor<T>, IIpRangeProperty> selector = null)
    {
      return this.SetProperty((IProperty) selector.InvokeOrDefault<IpRangePropertyDescriptor<T>, IIpRangeProperty>(new IpRangePropertyDescriptor<T>().Name<IpAddressRange>(field)));
    }

    public PropertiesDescriptor()
      : base((IProperties) new Properties<T>())
    {
    }

    public PropertiesDescriptor(IProperties properties)
      : base(properties ?? (IProperties) new Properties<T>())
    {
    }

    public PropertiesDescriptor<T> Binary(
      Func<BinaryPropertyDescriptor<T>, IBinaryProperty> selector)
    {
      return this.SetProperty<BinaryPropertyDescriptor<T>, IBinaryProperty>(selector);
    }

    public PropertiesDescriptor<T> Boolean(
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector)
    {
      return this.SetProperty<BooleanPropertyDescriptor<T>, IBooleanProperty>(selector);
    }

    public PropertiesDescriptor<T> Completion(
      Func<CompletionPropertyDescriptor<T>, ICompletionProperty> selector)
    {
      return this.SetProperty<CompletionPropertyDescriptor<T>, ICompletionProperty>(selector);
    }

    public PropertiesDescriptor<T> ConstantKeyword(
      Func<ConstantKeywordPropertyDescriptor<T>, IConstantKeywordProperty> selector)
    {
      return this.SetProperty<ConstantKeywordPropertyDescriptor<T>, IConstantKeywordProperty>(selector);
    }

    public PropertiesDescriptor<T> Date(
      Func<DatePropertyDescriptor<T>, IDateProperty> selector)
    {
      return this.SetProperty<DatePropertyDescriptor<T>, IDateProperty>(selector);
    }

    public PropertiesDescriptor<T> DateNanos(
      Func<DateNanosPropertyDescriptor<T>, IDateNanosProperty> selector)
    {
      return this.SetProperty<DateNanosPropertyDescriptor<T>, IDateNanosProperty>(selector);
    }

    public PropertiesDescriptor<T> DateRange(
      Func<DateRangePropertyDescriptor<T>, IDateRangeProperty> selector)
    {
      return this.SetProperty<DateRangePropertyDescriptor<T>, IDateRangeProperty>(selector);
    }

    public PropertiesDescriptor<T> DenseVector(
      Func<DenseVectorPropertyDescriptor<T>, IDenseVectorProperty> selector)
    {
      return this.SetProperty<DenseVectorPropertyDescriptor<T>, IDenseVectorProperty>(selector);
    }

    public PropertiesDescriptor<T> DoubleRange(
      Func<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty> selector)
    {
      return this.SetProperty<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty>(selector);
    }

    public PropertiesDescriptor<T> FieldAlias(
      Func<FieldAliasPropertyDescriptor<T>, IFieldAliasProperty> selector)
    {
      return this.SetProperty<FieldAliasPropertyDescriptor<T>, IFieldAliasProperty>(selector);
    }

    public PropertiesDescriptor<T> Flattened(
      Func<FlattenedPropertyDescriptor<T>, IFlattenedProperty> selector)
    {
      return this.SetProperty<FlattenedPropertyDescriptor<T>, IFlattenedProperty>(selector);
    }

    public PropertiesDescriptor<T> FloatRange(
      Func<FloatRangePropertyDescriptor<T>, IFloatRangeProperty> selector)
    {
      return this.SetProperty<FloatRangePropertyDescriptor<T>, IFloatRangeProperty>(selector);
    }

    public PropertiesDescriptor<T> GeoPoint(
      Func<GeoPointPropertyDescriptor<T>, IGeoPointProperty> selector)
    {
      return this.SetProperty<GeoPointPropertyDescriptor<T>, IGeoPointProperty>(selector);
    }

    public PropertiesDescriptor<T> GeoShape(
      Func<GeoShapePropertyDescriptor<T>, IGeoShapeProperty> selector)
    {
      return this.SetProperty<GeoShapePropertyDescriptor<T>, IGeoShapeProperty>(selector);
    }

    public PropertiesDescriptor<T> Histogram(
      Func<HistogramPropertyDescriptor<T>, IHistogramProperty> selector)
    {
      return this.SetProperty<HistogramPropertyDescriptor<T>, IHistogramProperty>(selector);
    }

    public PropertiesDescriptor<T> IntegerRange(
      Func<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty> selector)
    {
      return this.SetProperty<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty>(selector);
    }

    public PropertiesDescriptor<T> Ip(
      Func<IpPropertyDescriptor<T>, IIpProperty> selector)
    {
      return this.SetProperty<IpPropertyDescriptor<T>, IIpProperty>(selector);
    }

    public PropertiesDescriptor<T> IpRange(
      Func<IpRangePropertyDescriptor<T>, IIpRangeProperty> selector)
    {
      return this.SetProperty<IpRangePropertyDescriptor<T>, IIpRangeProperty>(selector);
    }

    public PropertiesDescriptor<T> Join(
      Func<JoinPropertyDescriptor<T>, IJoinProperty> selector)
    {
      return this.SetProperty<JoinPropertyDescriptor<T>, IJoinProperty>(selector);
    }

    public PropertiesDescriptor<T> Keyword(
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector)
    {
      return this.SetProperty<KeywordPropertyDescriptor<T>, IKeywordProperty>(selector);
    }

    public PropertiesDescriptor<T> LongRange(
      Func<LongRangePropertyDescriptor<T>, ILongRangeProperty> selector)
    {
      return this.SetProperty<LongRangePropertyDescriptor<T>, ILongRangeProperty>(selector);
    }

    public PropertiesDescriptor<T> MatchOnlyText(
      Func<MatchOnlyTextPropertyDescriptor<T>, IMatchOnlyTextProperty> selector)
    {
      return this.SetProperty<MatchOnlyTextPropertyDescriptor<T>, IMatchOnlyTextProperty>(selector);
    }

    public PropertiesDescriptor<T> Murmur3Hash(
      Func<Murmur3HashPropertyDescriptor<T>, IMurmur3HashProperty> selector)
    {
      return this.SetProperty<Murmur3HashPropertyDescriptor<T>, IMurmur3HashProperty>(selector);
    }

    public PropertiesDescriptor<T> Nested<TChild>(
      Func<NestedPropertyDescriptor<T, TChild>, INestedProperty> selector)
      where TChild : class
    {
      return this.SetProperty<NestedPropertyDescriptor<T, TChild>, INestedProperty>(selector);
    }

    public PropertiesDescriptor<T> Number(
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector)
    {
      return this.SetProperty<NumberPropertyDescriptor<T>, INumberProperty>(selector);
    }

    public PropertiesDescriptor<T> Object<TChild>(
      Func<ObjectTypeDescriptor<T, TChild>, IObjectProperty> selector)
      where TChild : class
    {
      return this.SetProperty<ObjectTypeDescriptor<T, TChild>, IObjectProperty>(selector);
    }

    public PropertiesDescriptor<T> Percolator(
      Func<PercolatorPropertyDescriptor<T>, IPercolatorProperty> selector)
    {
      return this.SetProperty<PercolatorPropertyDescriptor<T>, IPercolatorProperty>(selector);
    }

    public PropertiesDescriptor<T> Point(
      Func<PointPropertyDescriptor<T>, IPointProperty> selector)
    {
      return this.SetProperty<PointPropertyDescriptor<T>, IPointProperty>(selector);
    }

    public PropertiesDescriptor<T> RankFeature(
      Func<RankFeaturePropertyDescriptor<T>, IRankFeatureProperty> selector)
    {
      return this.SetProperty<RankFeaturePropertyDescriptor<T>, IRankFeatureProperty>(selector);
    }

    public PropertiesDescriptor<T> RankFeatures(
      Func<RankFeaturesPropertyDescriptor<T>, IRankFeaturesProperty> selector)
    {
      return this.SetProperty<RankFeaturesPropertyDescriptor<T>, IRankFeaturesProperty>(selector);
    }

    public PropertiesDescriptor<T> SearchAsYouType(
      Func<SearchAsYouTypePropertyDescriptor<T>, ISearchAsYouTypeProperty> selector)
    {
      return this.SetProperty<SearchAsYouTypePropertyDescriptor<T>, ISearchAsYouTypeProperty>(selector);
    }

    public PropertiesDescriptor<T> Shape(
      Func<ShapePropertyDescriptor<T>, IShapeProperty> selector)
    {
      return this.SetProperty<ShapePropertyDescriptor<T>, IShapeProperty>(selector);
    }

    public PropertiesDescriptor<T> Text(
      Func<TextPropertyDescriptor<T>, ITextProperty> selector)
    {
      return this.SetProperty<TextPropertyDescriptor<T>, ITextProperty>(selector);
    }

    public PropertiesDescriptor<T> TokenCount(
      Func<TokenCountPropertyDescriptor<T>, ITokenCountProperty> selector)
    {
      return this.SetProperty<TokenCountPropertyDescriptor<T>, ITokenCountProperty>(selector);
    }

    public PropertiesDescriptor<T> Version(
      Func<VersionPropertyDescriptor<T>, IVersionProperty> selector)
    {
      return this.SetProperty<VersionPropertyDescriptor<T>, IVersionProperty>(selector);
    }

    public PropertiesDescriptor<T> Wildcard(
      Func<WildcardPropertyDescriptor<T>, IWildcardProperty> selector)
    {
      return this.SetProperty<WildcardPropertyDescriptor<T>, IWildcardProperty>(selector);
    }

    public PropertiesDescriptor<T> Custom(IProperty customType) => this.SetProperty(customType);

    private PropertiesDescriptor<T> SetProperty<TDescriptor, TInterface>(
      Func<TDescriptor, TInterface> selector)
      where TDescriptor : class, TInterface, new()
      where TInterface : IProperty
    {
      selector.ThrowIfNull<Func<TDescriptor, TInterface>>(nameof (selector));
      return this.SetProperty((IProperty) selector(new TDescriptor()));
    }

    private PropertiesDescriptor<T> SetProperty(IProperty type)
    {
      type.ThrowIfNull<IProperty>(nameof (type));
      string name = type.GetType().Name;
      if (type.Name.IsConditionless())
        throw new ArgumentException("Could not get field name for " + name + " mapping");
      return this.Assign<IProperty>(type, (Action<IProperties, IProperty>) ((a, v) => a[v.Name] = v));
    }
  }
}
