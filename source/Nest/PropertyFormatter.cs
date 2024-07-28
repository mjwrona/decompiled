// Decompiled with JetBrains decompiler
// Type: Nest.PropertyFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class PropertyFormatter : IJsonFormatter<IProperty>, IJsonFormatter
  {
    private static readonly AutomataDictionary AutomataDictionary = new AutomataDictionary()
    {
      {
        "type",
        0
      },
      {
        "properties",
        1
      }
    };

    public IProperty Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      JsonReader reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      int count = 0;
      string str = (string) null;
      FieldType fieldType = FieldType.None;
      while (reader1.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader1.ReadPropertyNameSegmentRaw();
        int num;
        if (PropertyFormatter.AutomataDictionary.TryGetValue(bytes, out num))
        {
          if (num != 0)
          {
            if (num == 1)
            {
              if (fieldType == FieldType.None)
                fieldType = FieldType.Object;
              reader1.ReadNextBlock();
            }
          }
          else
          {
            str = reader1.ReadString();
            fieldType = str.ToEnum<FieldType>().GetValueOrDefault(fieldType);
          }
          if (num == 0)
            break;
        }
        else
          reader1.ReadNextBlock();
      }
      reader1 = new JsonReader(arraySegment.Array, arraySegment.Offset);
      switch (fieldType)
      {
        case FieldType.None:
          return (IProperty) PropertyFormatter.Deserialize<ObjectProperty>(ref reader1, formatterResolver);
        case FieldType.GeoPoint:
          return (IProperty) PropertyFormatter.Deserialize<GeoPointProperty>(ref reader1, formatterResolver);
        case FieldType.GeoShape:
          return (IProperty) PropertyFormatter.Deserialize<GeoShapeProperty>(ref reader1, formatterResolver);
        case FieldType.Ip:
          return (IProperty) PropertyFormatter.Deserialize<IpProperty>(ref reader1, formatterResolver);
        case FieldType.Binary:
          return (IProperty) PropertyFormatter.Deserialize<BinaryProperty>(ref reader1, formatterResolver);
        case FieldType.Keyword:
          return (IProperty) PropertyFormatter.Deserialize<KeywordProperty>(ref reader1, formatterResolver);
        case FieldType.Text:
          return (IProperty) PropertyFormatter.Deserialize<TextProperty>(ref reader1, formatterResolver);
        case FieldType.SearchAsYouType:
          return (IProperty) PropertyFormatter.Deserialize<SearchAsYouTypeProperty>(ref reader1, formatterResolver);
        case FieldType.Date:
          return (IProperty) PropertyFormatter.Deserialize<DateProperty>(ref reader1, formatterResolver);
        case FieldType.DateNanos:
          return (IProperty) PropertyFormatter.Deserialize<DateNanosProperty>(ref reader1, formatterResolver);
        case FieldType.Boolean:
          return (IProperty) PropertyFormatter.Deserialize<BooleanProperty>(ref reader1, formatterResolver);
        case FieldType.Completion:
          return (IProperty) PropertyFormatter.Deserialize<CompletionProperty>(ref reader1, formatterResolver);
        case FieldType.Nested:
          return (IProperty) PropertyFormatter.Deserialize<NestedProperty>(ref reader1, formatterResolver);
        case FieldType.Object:
          return (IProperty) PropertyFormatter.Deserialize<ObjectProperty>(ref reader1, formatterResolver);
        case FieldType.Murmur3Hash:
          return (IProperty) PropertyFormatter.Deserialize<Murmur3HashProperty>(ref reader1, formatterResolver);
        case FieldType.TokenCount:
          return (IProperty) PropertyFormatter.Deserialize<TokenCountProperty>(ref reader1, formatterResolver);
        case FieldType.Percolator:
          return (IProperty) PropertyFormatter.Deserialize<PercolatorProperty>(ref reader1, formatterResolver);
        case FieldType.Integer:
        case FieldType.Long:
        case FieldType.UnsignedLong:
        case FieldType.Short:
        case FieldType.Byte:
        case FieldType.Float:
        case FieldType.HalfFloat:
        case FieldType.ScaledFloat:
        case FieldType.Double:
          NumberProperty numberProperty = PropertyFormatter.Deserialize<NumberProperty>(ref reader1, formatterResolver);
          ((IProperty) numberProperty).Type = str;
          return (IProperty) numberProperty;
        case FieldType.IntegerRange:
          return (IProperty) PropertyFormatter.Deserialize<IntegerRangeProperty>(ref reader1, formatterResolver);
        case FieldType.FloatRange:
          return (IProperty) PropertyFormatter.Deserialize<FloatRangeProperty>(ref reader1, formatterResolver);
        case FieldType.LongRange:
          return (IProperty) PropertyFormatter.Deserialize<LongRangeProperty>(ref reader1, formatterResolver);
        case FieldType.DoubleRange:
          return (IProperty) PropertyFormatter.Deserialize<DoubleRangeProperty>(ref reader1, formatterResolver);
        case FieldType.DateRange:
          return (IProperty) PropertyFormatter.Deserialize<DateRangeProperty>(ref reader1, formatterResolver);
        case FieldType.IpRange:
          return (IProperty) PropertyFormatter.Deserialize<IpRangeProperty>(ref reader1, formatterResolver);
        case FieldType.Alias:
          return (IProperty) PropertyFormatter.Deserialize<FieldAliasProperty>(ref reader1, formatterResolver);
        case FieldType.Join:
          return (IProperty) PropertyFormatter.Deserialize<JoinProperty>(ref reader1, formatterResolver);
        case FieldType.RankFeature:
          return (IProperty) PropertyFormatter.Deserialize<RankFeatureProperty>(ref reader1, formatterResolver);
        case FieldType.RankFeatures:
          return (IProperty) PropertyFormatter.Deserialize<RankFeaturesProperty>(ref reader1, formatterResolver);
        case FieldType.Flattened:
          return (IProperty) PropertyFormatter.Deserialize<FlattenedProperty>(ref reader1, formatterResolver);
        case FieldType.Shape:
          return (IProperty) PropertyFormatter.Deserialize<ShapeProperty>(ref reader1, formatterResolver);
        case FieldType.Histogram:
          return (IProperty) PropertyFormatter.Deserialize<HistogramProperty>(ref reader1, formatterResolver);
        case FieldType.ConstantKeyword:
          return (IProperty) PropertyFormatter.Deserialize<ConstantKeywordProperty>(ref reader1, formatterResolver);
        case FieldType.Wildcard:
          return (IProperty) PropertyFormatter.Deserialize<WildcardProperty>(ref reader1, formatterResolver);
        case FieldType.Point:
          return (IProperty) PropertyFormatter.Deserialize<PointProperty>(ref reader1, formatterResolver);
        case FieldType.Version:
          return (IProperty) PropertyFormatter.Deserialize<VersionProperty>(ref reader1, formatterResolver);
        case FieldType.DenseVector:
          return (IProperty) PropertyFormatter.Deserialize<DenseVectorProperty>(ref reader1, formatterResolver);
        case FieldType.MatchOnlyText:
          return (IProperty) PropertyFormatter.Deserialize<MatchOnlyTextProperty>(ref reader1, formatterResolver);
        default:
          throw new ArgumentOutOfRangeException("type", (object) fieldType, "mapping property converter does not know this value");
      }
    }

    public void Serialize(
      ref JsonWriter writer,
      IProperty value,
      IJsonFormatterResolver formatterResolver)
    {
      switch (value)
      {
        case null:
          writer.WriteNull();
          break;
        case ITextProperty textProperty:
          PropertyFormatter.Serialize<ITextProperty>(ref writer, textProperty, formatterResolver);
          break;
        case IKeywordProperty keywordProperty:
          PropertyFormatter.Serialize<IKeywordProperty>(ref writer, keywordProperty, formatterResolver);
          break;
        case INumberProperty numberProperty:
          PropertyFormatter.Serialize<INumberProperty>(ref writer, numberProperty, formatterResolver);
          break;
        case IDateProperty dateProperty:
          PropertyFormatter.Serialize<IDateProperty>(ref writer, dateProperty, formatterResolver);
          break;
        case IBooleanProperty booleanProperty:
          PropertyFormatter.Serialize<IBooleanProperty>(ref writer, booleanProperty, formatterResolver);
          break;
        case INestedProperty nestedProperty:
          PropertyFormatter.Serialize<INestedProperty>(ref writer, nestedProperty, formatterResolver);
          break;
        case IObjectProperty objectProperty:
          PropertyFormatter.Serialize<IObjectProperty>(ref writer, objectProperty, formatterResolver);
          break;
        case ISearchAsYouTypeProperty asYouTypeProperty:
          PropertyFormatter.Serialize<ISearchAsYouTypeProperty>(ref writer, asYouTypeProperty, formatterResolver);
          break;
        case IDateNanosProperty dateNanosProperty:
          PropertyFormatter.Serialize<IDateNanosProperty>(ref writer, dateNanosProperty, formatterResolver);
          break;
        case IBinaryProperty binaryProperty:
          PropertyFormatter.Serialize<IBinaryProperty>(ref writer, binaryProperty, formatterResolver);
          break;
        case IIpProperty ipProperty:
          PropertyFormatter.Serialize<IIpProperty>(ref writer, ipProperty, formatterResolver);
          break;
        case IGeoPointProperty geoPointProperty:
          PropertyFormatter.Serialize<IGeoPointProperty>(ref writer, geoPointProperty, formatterResolver);
          break;
        case IGeoShapeProperty geoShapeProperty:
          PropertyFormatter.Serialize<IGeoShapeProperty>(ref writer, geoShapeProperty, formatterResolver);
          break;
        case IShapeProperty shapeProperty:
          PropertyFormatter.Serialize<IShapeProperty>(ref writer, shapeProperty, formatterResolver);
          break;
        case IPointProperty pointProperty:
          PropertyFormatter.Serialize<IPointProperty>(ref writer, pointProperty, formatterResolver);
          break;
        case ICompletionProperty completionProperty:
          PropertyFormatter.Serialize<ICompletionProperty>(ref writer, completionProperty, formatterResolver);
          break;
        case ITokenCountProperty tokenCountProperty:
          PropertyFormatter.Serialize<ITokenCountProperty>(ref writer, tokenCountProperty, formatterResolver);
          break;
        case IMurmur3HashProperty murmur3HashProperty:
          PropertyFormatter.Serialize<IMurmur3HashProperty>(ref writer, murmur3HashProperty, formatterResolver);
          break;
        case IPercolatorProperty percolatorProperty:
          PropertyFormatter.Serialize<IPercolatorProperty>(ref writer, percolatorProperty, formatterResolver);
          break;
        case IDateRangeProperty dateRangeProperty:
          PropertyFormatter.Serialize<IDateRangeProperty>(ref writer, dateRangeProperty, formatterResolver);
          break;
        case IDoubleRangeProperty doubleRangeProperty:
          PropertyFormatter.Serialize<IDoubleRangeProperty>(ref writer, doubleRangeProperty, formatterResolver);
          break;
        case IFloatRangeProperty floatRangeProperty:
          PropertyFormatter.Serialize<IFloatRangeProperty>(ref writer, floatRangeProperty, formatterResolver);
          break;
        case IIntegerRangeProperty integerRangeProperty:
          PropertyFormatter.Serialize<IIntegerRangeProperty>(ref writer, integerRangeProperty, formatterResolver);
          break;
        case ILongRangeProperty longRangeProperty:
          PropertyFormatter.Serialize<ILongRangeProperty>(ref writer, longRangeProperty, formatterResolver);
          break;
        case IIpRangeProperty ipRangeProperty:
          PropertyFormatter.Serialize<IIpRangeProperty>(ref writer, ipRangeProperty, formatterResolver);
          break;
        case IJoinProperty joinProperty:
          PropertyFormatter.Serialize<IJoinProperty>(ref writer, joinProperty, formatterResolver);
          break;
        case IFieldAliasProperty fieldAliasProperty:
          PropertyFormatter.Serialize<IFieldAliasProperty>(ref writer, fieldAliasProperty, formatterResolver);
          break;
        case IRankFeatureProperty rankFeatureProperty:
          PropertyFormatter.Serialize<IRankFeatureProperty>(ref writer, rankFeatureProperty, formatterResolver);
          break;
        case IRankFeaturesProperty featuresProperty:
          PropertyFormatter.Serialize<IRankFeaturesProperty>(ref writer, featuresProperty, formatterResolver);
          break;
        case IFlattenedProperty flattenedProperty:
          PropertyFormatter.Serialize<IFlattenedProperty>(ref writer, flattenedProperty, formatterResolver);
          break;
        case IHistogramProperty histogramProperty:
          PropertyFormatter.Serialize<IHistogramProperty>(ref writer, histogramProperty, formatterResolver);
          break;
        case IConstantKeywordProperty constantKeywordProperty:
          PropertyFormatter.Serialize<IConstantKeywordProperty>(ref writer, constantKeywordProperty, formatterResolver);
          break;
        case IWildcardProperty wildcardProperty:
          PropertyFormatter.Serialize<IWildcardProperty>(ref writer, wildcardProperty, formatterResolver);
          break;
        case IGenericProperty genericProperty:
          PropertyFormatter.Serialize<IGenericProperty>(ref writer, genericProperty, formatterResolver);
          break;
        case IVersionProperty versionProperty:
          PropertyFormatter.Serialize<IVersionProperty>(ref writer, versionProperty, formatterResolver);
          break;
        case IDenseVectorProperty denseVectorProperty:
          PropertyFormatter.Serialize<IDenseVectorProperty>(ref writer, denseVectorProperty, formatterResolver);
          break;
        case IMatchOnlyTextProperty onlyTextProperty:
          PropertyFormatter.Serialize<IMatchOnlyTextProperty>(ref writer, onlyTextProperty, formatterResolver);
          break;
        default:
          formatterResolver.GetFormatter<object>().Serialize(ref writer, (object) value, formatterResolver);
          break;
      }
    }

    private static void Serialize<TProperty>(
      ref JsonWriter writer,
      TProperty value,
      IJsonFormatterResolver formatterResolver)
      where TProperty : class, IProperty
    {
      formatterResolver.GetFormatter<TProperty>().Serialize(ref writer, value, formatterResolver);
    }

    private static TProperty Deserialize<TProperty>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TProperty : IProperty
    {
      return formatterResolver.GetFormatter<TProperty>().Deserialize(ref reader, formatterResolver);
    }
  }
}
