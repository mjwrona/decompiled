// Decompiled with JetBrains decompiler
// Type: Nest.IPropertiesDescriptor`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public interface IPropertiesDescriptor<T, out TReturnType>
    where T : class
    where TReturnType : class
  {
    TReturnType Scalar(
      Expression<Func<T, int>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, int?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<int>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<int?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, float>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, float?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<float>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<float?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, sbyte>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, sbyte?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<sbyte>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<sbyte?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, short>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, short?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<short>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<short?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, byte>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, byte?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<byte>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<byte?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, long>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, long?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<long>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<long?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, uint>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, uint?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<uint>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<uint?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, TimeSpan>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, TimeSpan?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<TimeSpan>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<TimeSpan?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Decimal>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Decimal?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<Decimal>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<Decimal?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, ulong>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, ulong?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<ulong>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<ulong?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, double>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, double?>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<double>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<double?>>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Enum>> field,
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, DateTime>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, DateTime?>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<DateTime>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<DateTime?>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, DateTimeOffset>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, DateTimeOffset?>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<DateTimeOffset>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<DateTimeOffset?>>> field,
      Func<DatePropertyDescriptor<T>, IDateProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, bool>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, bool?>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<bool>>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<bool?>>> field,
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, char>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, char?>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<char>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<char?>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Guid>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Guid?>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<Guid>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<Guid?>>> field,
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, string>> field,
      Func<TextPropertyDescriptor<T>, ITextProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IEnumerable<string>>> field,
      Func<TextPropertyDescriptor<T>, ITextProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Nest.DateRange>> field,
      Func<DateRangePropertyDescriptor<T>, IDateRangeProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Nest.DoubleRange>> field,
      Func<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Nest.LongRange>> field,
      Func<LongRangePropertyDescriptor<T>, ILongRangeProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Nest.IntegerRange>> field,
      Func<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, Nest.FloatRange>> field,
      Func<FloatRangePropertyDescriptor<T>, IFloatRangeProperty> selector = null);

    TReturnType Scalar(
      Expression<Func<T, IpAddressRange>> field,
      Func<IpRangePropertyDescriptor<T>, IIpRangeProperty> selector = null);

    TReturnType Text(
      Func<TextPropertyDescriptor<T>, ITextProperty> selector);

    TReturnType Keyword(
      Func<KeywordPropertyDescriptor<T>, IKeywordProperty> selector);

    TReturnType Number(
      Func<NumberPropertyDescriptor<T>, INumberProperty> selector);

    TReturnType TokenCount(
      Func<TokenCountPropertyDescriptor<T>, ITokenCountProperty> selector);

    TReturnType Date(
      Func<DatePropertyDescriptor<T>, IDateProperty> selector);

    TReturnType DateNanos(
      Func<DateNanosPropertyDescriptor<T>, IDateNanosProperty> selector);

    TReturnType Boolean(
      Func<BooleanPropertyDescriptor<T>, IBooleanProperty> selector);

    TReturnType Binary(
      Func<BinaryPropertyDescriptor<T>, IBinaryProperty> selector);

    TReturnType Object<TChild>(
      Func<ObjectTypeDescriptor<T, TChild>, IObjectProperty> selector)
      where TChild : class;

    TReturnType Nested<TChild>(
      Func<NestedPropertyDescriptor<T, TChild>, INestedProperty> selector)
      where TChild : class;

    TReturnType Ip(
      Func<IpPropertyDescriptor<T>, IIpProperty> selector);

    TReturnType GeoPoint(
      Func<GeoPointPropertyDescriptor<T>, IGeoPointProperty> selector);

    TReturnType GeoShape(
      Func<GeoShapePropertyDescriptor<T>, IGeoShapeProperty> selector);

    TReturnType Shape(
      Func<ShapePropertyDescriptor<T>, IShapeProperty> selector);

    TReturnType Point(
      Func<PointPropertyDescriptor<T>, IPointProperty> selector);

    TReturnType Completion(
      Func<CompletionPropertyDescriptor<T>, ICompletionProperty> selector);

    TReturnType Murmur3Hash(
      Func<Murmur3HashPropertyDescriptor<T>, IMurmur3HashProperty> selector);

    TReturnType Percolator(
      Func<PercolatorPropertyDescriptor<T>, IPercolatorProperty> selector);

    TReturnType DateRange(
      Func<DateRangePropertyDescriptor<T>, IDateRangeProperty> selector);

    TReturnType DoubleRange(
      Func<DoubleRangePropertyDescriptor<T>, IDoubleRangeProperty> selector);

    TReturnType FloatRange(
      Func<FloatRangePropertyDescriptor<T>, IFloatRangeProperty> selector);

    TReturnType IntegerRange(
      Func<IntegerRangePropertyDescriptor<T>, IIntegerRangeProperty> selector);

    TReturnType LongRange(
      Func<LongRangePropertyDescriptor<T>, ILongRangeProperty> selector);

    TReturnType IpRange(
      Func<IpRangePropertyDescriptor<T>, IIpRangeProperty> selector);

    TReturnType Join(
      Func<JoinPropertyDescriptor<T>, IJoinProperty> selector);

    TReturnType Histogram(
      Func<HistogramPropertyDescriptor<T>, IHistogramProperty> selector);

    TReturnType FieldAlias(
      Func<FieldAliasPropertyDescriptor<T>, IFieldAliasProperty> selector);

    TReturnType RankFeature(
      Func<RankFeaturePropertyDescriptor<T>, IRankFeatureProperty> selector);

    TReturnType RankFeatures(
      Func<RankFeaturesPropertyDescriptor<T>, IRankFeaturesProperty> selector);

    TReturnType Flattened(
      Func<FlattenedPropertyDescriptor<T>, IFlattenedProperty> selector);

    TReturnType SearchAsYouType(
      Func<SearchAsYouTypePropertyDescriptor<T>, ISearchAsYouTypeProperty> selector);

    TReturnType ConstantKeyword(
      Func<ConstantKeywordPropertyDescriptor<T>, IConstantKeywordProperty> selector);

    TReturnType Wildcard(
      Func<WildcardPropertyDescriptor<T>, IWildcardProperty> selector);

    TReturnType Version(
      Func<VersionPropertyDescriptor<T>, IVersionProperty> selector);

    TReturnType DenseVector(
      Func<DenseVectorPropertyDescriptor<T>, IDenseVectorProperty> selector);

    TReturnType MatchOnlyText(
      Func<MatchOnlyTextPropertyDescriptor<T>, IMatchOnlyTextProperty> selector);
  }
}
