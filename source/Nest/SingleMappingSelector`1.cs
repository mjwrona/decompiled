// Decompiled with JetBrains decompiler
// Type: Nest.SingleMappingSelector`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class SingleMappingSelector<T> : SelectorBase, IPropertiesDescriptor<T, IProperty> where T : class
  {
    public IProperty Binary(
      Func<BinaryPropertyDescriptor<T>, IBinaryProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new BinaryPropertyDescriptor<T>());
    }

    public IProperty Boolean(
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new BooleanPropertyDescriptor<T>());
    }

    public IProperty Completion(
      Func<CompletionPropertyDescriptor<T>, ICompletionProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new CompletionPropertyDescriptor<T>());
    }

    public IProperty ConstantKeyword(
      Func<ConstantKeywordPropertyDescriptor<T>, IConstantKeywordProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new ConstantKeywordPropertyDescriptor<T>());
    }

    public IProperty Date(
      Func<DatePropertyDescriptor<T>, IDateProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new DatePropertyDescriptor<T>());
    }

    public IProperty DateNanos(
      Func<DateNanosPropertyDescriptor<T>, IDateNanosProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new DateNanosPropertyDescriptor<T>());
    }

    public IProperty DateRange(
      Func<DateRangePropertyDescriptor<T>, IDateRangeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new DateRangePropertyDescriptor<T>());
    }

    public IProperty DoubleRange(
      Func<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new DoubleRangePropertyDescriptor<T>());
    }

    public IProperty FieldAlias(
      Func<FieldAliasPropertyDescriptor<T>, IFieldAliasProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new FieldAliasPropertyDescriptor<T>());
    }

    public IProperty Flattened(
      Func<FlattenedPropertyDescriptor<T>, IFlattenedProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new FlattenedPropertyDescriptor<T>());
    }

    public IProperty FloatRange(
      Func<FloatRangePropertyDescriptor<T>, IFloatRangeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new FloatRangePropertyDescriptor<T>());
    }

    public IProperty GeoPoint(
      Func<GeoPointPropertyDescriptor<T>, IGeoPointProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new GeoPointPropertyDescriptor<T>());
    }

    public IProperty GeoShape(
      Func<GeoShapePropertyDescriptor<T>, IGeoShapeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new GeoShapePropertyDescriptor<T>());
    }

    public IProperty Histogram(
      Func<HistogramPropertyDescriptor<T>, IHistogramProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new HistogramPropertyDescriptor<T>());
    }

    public IProperty IntegerRange(
      Func<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new IntegerRangePropertyDescriptor<T>());
    }

    public IProperty Ip(
      Func<IpPropertyDescriptor<T>, IIpProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new IpPropertyDescriptor<T>());
    }

    public IProperty IpRange(
      Func<IpRangePropertyDescriptor<T>, IIpRangeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new IpRangePropertyDescriptor<T>());
    }

    public IProperty Join(
      Func<JoinPropertyDescriptor<T>, IJoinProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new JoinPropertyDescriptor<T>());
    }

    public IProperty Keyword(
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new KeywordPropertyDescriptor<T>());
    }

    public IProperty LongRange(
      Func<LongRangePropertyDescriptor<T>, ILongRangeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new LongRangePropertyDescriptor<T>());
    }

    public IProperty Murmur3Hash(
      Func<Murmur3HashPropertyDescriptor<T>, IMurmur3HashProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new Murmur3HashPropertyDescriptor<T>());
    }

    public IProperty Nested<TChild>(
      Func<NestedPropertyDescriptor<T, TChild>, INestedProperty> selector)
      where TChild : class
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new NestedPropertyDescriptor<T, TChild>());
    }

    public IProperty Number(
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new NumberPropertyDescriptor<T>());
    }

    public IProperty Object<TChild>(
      Func<ObjectTypeDescriptor<T, TChild>, IObjectProperty> selector)
      where TChild : class
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new ObjectTypeDescriptor<T, TChild>());
    }

    public IProperty Percolator(
      Func<PercolatorPropertyDescriptor<T>, IPercolatorProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new PercolatorPropertyDescriptor<T>());
    }

    public IProperty Point(
      Func<PointPropertyDescriptor<T>, IPointProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new PointPropertyDescriptor<T>());
    }

    public IProperty RankFeature(
      Func<RankFeaturePropertyDescriptor<T>, IRankFeatureProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new RankFeaturePropertyDescriptor<T>());
    }

    public IProperty RankFeatures(
      Func<RankFeaturesPropertyDescriptor<T>, IRankFeaturesProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new RankFeaturesPropertyDescriptor<T>());
    }

    public IProperty SearchAsYouType(
      Func<SearchAsYouTypePropertyDescriptor<T>, ISearchAsYouTypeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new SearchAsYouTypePropertyDescriptor<T>());
    }

    public IProperty Shape(
      Func<ShapePropertyDescriptor<T>, IShapeProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new ShapePropertyDescriptor<T>());
    }

    public IProperty Text(
      Func<TextPropertyDescriptor<T>, ITextProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new TextPropertyDescriptor<T>());
    }

    public IProperty TokenCount(
      Func<TokenCountPropertyDescriptor<T>, ITokenCountProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new TokenCountPropertyDescriptor<T>());
    }

    public IProperty Version(
      Func<VersionPropertyDescriptor<T>, IVersionProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new VersionPropertyDescriptor<T>());
    }

    public IProperty Wildcard(
      Func<WildcardPropertyDescriptor<T>, IWildcardProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new WildcardPropertyDescriptor<T>());
    }

    public IProperty Generic(
      Func<GenericPropertyDescriptor<T>, IGenericProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new GenericPropertyDescriptor<T>());
    }

    public IProperty DenseVector(
      Func<DenseVectorPropertyDescriptor<T>, IDenseVectorProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new DenseVectorPropertyDescriptor<T>());
    }

    public IProperty MatchOnlyText(
      Func<MatchOnlyTextPropertyDescriptor<T>, IMatchOnlyTextProperty> selector)
    {
      return selector == null ? (IProperty) null : (IProperty) selector(new MatchOnlyTextPropertyDescriptor<T>());
    }

    public IProperty Scalar(
      Expression<Func<T, int>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<int>(field).Type(new NumberType?(NumberType.Integer)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<int>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<int>>(field).Type(new NumberType?(NumberType.Integer)));
    }

    public IProperty Scalar(
      Expression<Func<T, int?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<int?>(field).Type(new NumberType?(NumberType.Integer)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<int?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<int?>>(field).Type(new NumberType?(NumberType.Integer)));
    }

    public IProperty Scalar(
      Expression<Func<T, float>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<float>(field).Type(new NumberType?(NumberType.Float)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<float>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<float>>(field).Type(new NumberType?(NumberType.Float)));
    }

    public IProperty Scalar(
      Expression<Func<T, float?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<float?>(field).Type(new NumberType?(NumberType.Float)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<float?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<float?>>(field).Type(new NumberType?(NumberType.Float)));
    }

    public IProperty Scalar(
      Expression<Func<T, sbyte>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<sbyte>(field).Type(new NumberType?(NumberType.Byte)));
    }

    public IProperty Scalar(
      Expression<Func<T, sbyte?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<sbyte?>(field).Type(new NumberType?(NumberType.Byte)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<sbyte>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<sbyte>>(field).Type(new NumberType?(NumberType.Byte)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<sbyte?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<sbyte?>>(field).Type(new NumberType?(NumberType.Byte)));
    }

    public IProperty Scalar(
      Expression<Func<T, short>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<short>(field).Type(new NumberType?(NumberType.Short)));
    }

    public IProperty Scalar(
      Expression<Func<T, short?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<short?>(field).Type(new NumberType?(NumberType.Short)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<short>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<short>>(field).Type(new NumberType?(NumberType.Short)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<short?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<short?>>(field).Type(new NumberType?(NumberType.Short)));
    }

    public IProperty Scalar(
      Expression<Func<T, byte>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<byte>(field).Type(new NumberType?(NumberType.Short)));
    }

    public IProperty Scalar(
      Expression<Func<T, byte?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<byte?>(field).Type(new NumberType?(NumberType.Short)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<byte>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<byte>>(field).Type(new NumberType?(NumberType.Short)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<byte?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<byte?>>(field).Type(new NumberType?(NumberType.Short)));
    }

    public IProperty Scalar(
      Expression<Func<T, long>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<long>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, long?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<long?>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<long>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<long>>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<long?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<long?>>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, uint>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<uint>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, uint?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<uint?>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<uint>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<uint>>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<uint?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<uint?>>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, TimeSpan>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<TimeSpan>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, TimeSpan?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<TimeSpan?>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<TimeSpan>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<TimeSpan>>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<TimeSpan?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<TimeSpan?>>(field).Type(new NumberType?(NumberType.Long)));
    }

    public IProperty Scalar(
      Expression<Func<T, Decimal>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<Decimal>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, Decimal?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<Decimal?>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<Decimal>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<Decimal>>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<Decimal?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<Decimal?>>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, ulong>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<ulong>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, ulong?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<ulong?>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<ulong>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<ulong>>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<ulong?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<ulong?>>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, double>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<double>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, double?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<double?>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<double>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<double>>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<double?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<IEnumerable<double?>>(field).Type(new NumberType?(NumberType.Double)));
    }

    public IProperty Scalar(
      Expression<Func<T, Enum>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<NumberPropertyDescriptor<T>, INumberProperty>(new NumberPropertyDescriptor<T>().Name<Enum>(field).Type(new NumberType?(NumberType.Integer)));
    }

    public IProperty Scalar(
      Expression<Func<T, DateTime>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<DateTime>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, DateTime?>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<DateTime?>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<DateTime>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<IEnumerable<DateTime>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<DateTime?>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<IEnumerable<DateTime?>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, DateTimeOffset>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<DateTimeOffset>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, DateTimeOffset?>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<DateTimeOffset?>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<DateTimeOffset>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<IEnumerable<DateTimeOffset>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<DateTimeOffset?>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DatePropertyDescriptor<T>, IDateProperty>(new DatePropertyDescriptor<T>().Name<IEnumerable<DateTimeOffset?>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, bool>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<BooleanPropertyDescriptor<T>, IBooleanProperty>(new BooleanPropertyDescriptor<T>().Name<bool>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, bool?>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<BooleanPropertyDescriptor<T>, IBooleanProperty>(new BooleanPropertyDescriptor<T>().Name<bool?>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<bool>>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<BooleanPropertyDescriptor<T>, IBooleanProperty>(new BooleanPropertyDescriptor<T>().Name<IEnumerable<bool>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<bool?>>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<BooleanPropertyDescriptor<T>, IBooleanProperty>(new BooleanPropertyDescriptor<T>().Name<IEnumerable<bool?>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, char>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<char>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, char?>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<char?>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<char>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<IEnumerable<char>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<char?>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<IEnumerable<char?>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, Guid>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<Guid>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, Guid?>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<Guid?>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<Guid>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<IEnumerable<Guid>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<Guid?>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<KeywordPropertyDescriptor<T>, IKeywordProperty>(new KeywordPropertyDescriptor<T>().Name<IEnumerable<Guid?>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, string>> field,
      Func<TextPropertyDescriptor<T>, ITextProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<TextPropertyDescriptor<T>, ITextProperty>(new TextPropertyDescriptor<T>().Name<string>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IEnumerable<string>>> field,
      Func<TextPropertyDescriptor<T>, ITextProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<TextPropertyDescriptor<T>, ITextProperty>(new TextPropertyDescriptor<T>().Name<IEnumerable<string>>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, Nest.DateRange>> field,
      Func<DateRangePropertyDescriptor<T>, IDateRangeProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DateRangePropertyDescriptor<T>, IDateRangeProperty>(new DateRangePropertyDescriptor<T>().Name<Nest.DateRange>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, Nest.DoubleRange>> field,
      Func<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty>(new DoubleRangePropertyDescriptor<T>().Name<Nest.DoubleRange>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, Nest.LongRange>> field,
      Func<LongRangePropertyDescriptor<T>, ILongRangeProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<LongRangePropertyDescriptor<T>, ILongRangeProperty>(new LongRangePropertyDescriptor<T>().Name<Nest.LongRange>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, Nest.IntegerRange>> field,
      Func<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty>(new IntegerRangePropertyDescriptor<T>().Name<Nest.IntegerRange>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, Nest.FloatRange>> field,
      Func<FloatRangePropertyDescriptor<T>, IFloatRangeProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<FloatRangePropertyDescriptor<T>, IFloatRangeProperty>(new FloatRangePropertyDescriptor<T>().Name<Nest.FloatRange>(field));
    }

    public IProperty Scalar(
      Expression<Func<T, IpAddressRange>> field,
      Func<IpRangePropertyDescriptor<T>, IIpRangeProperty> selector = null)
    {
      return (IProperty) selector.InvokeOrDefault<IpRangePropertyDescriptor<T>, IIpRangeProperty>(new IpRangePropertyDescriptor<T>().Name<IpAddressRange>(field));
    }
  }
}
