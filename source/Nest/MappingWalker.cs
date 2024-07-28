// Decompiled with JetBrains decompiler
// Type: Nest.MappingWalker
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class MappingWalker
  {
    private readonly IMappingVisitor _visitor;

    public MappingWalker(IMappingVisitor visitor)
    {
      visitor.ThrowIfNull<IMappingVisitor>(nameof (visitor));
      this._visitor = visitor;
    }

    public void Accept(GetMappingResponse response)
    {
      if (response?.Indices == null)
        return;
      foreach (KeyValuePair<IndexName, IndexMappings> index in (IEnumerable<KeyValuePair<IndexName, IndexMappings>>) response.Indices)
      {
        if (index.Value?.Mappings != null)
          this.Accept((ITypeMapping) index.Value.Mappings);
      }
    }

    public void Accept(ITypeMapping mapping)
    {
      if (mapping == null)
        return;
      this._visitor.Visit(mapping);
      this.Accept(mapping.Properties);
    }

    private static void Visit<TProperty>(IProperty prop, Action<TProperty> act) where TProperty : class, IProperty
    {
      if (!(prop is TProperty property))
        return;
      act(property);
    }

    public void Accept(IProperties properties)
    {
      if (properties == null)
        return;
      foreach (KeyValuePair<PropertyName, IProperty> property in (IEnumerable<KeyValuePair<PropertyName, IProperty>>) properties)
      {
        IProperty prop = property.Value;
        FieldType? nullable = prop.Type.ToEnum<FieldType>();
        if (nullable.HasValue)
        {
          switch (nullable.GetValueOrDefault())
          {
            case FieldType.GeoPoint:
              MappingWalker.Visit<IGeoPointProperty>(prop, (Action<IGeoPointProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.GeoShape:
              MappingWalker.Visit<IGeoShapeProperty>(prop, (Action<IGeoShapeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Ip:
              MappingWalker.Visit<IIpProperty>(prop, (Action<IIpProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Binary:
              MappingWalker.Visit<IBinaryProperty>(prop, (Action<IBinaryProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Keyword:
              MappingWalker.Visit<IKeywordProperty>(prop, (Action<IKeywordProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Text:
              MappingWalker.Visit<ITextProperty>(prop, (Action<ITextProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.SearchAsYouType:
              MappingWalker.Visit<ISearchAsYouTypeProperty>(prop, (Action<ISearchAsYouTypeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Date:
              MappingWalker.Visit<IDateProperty>(prop, (Action<IDateProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.DateNanos:
              MappingWalker.Visit<IDateNanosProperty>(prop, (Action<IDateNanosProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Boolean:
              MappingWalker.Visit<IBooleanProperty>(prop, (Action<IBooleanProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Completion:
              MappingWalker.Visit<ICompletionProperty>(prop, (Action<ICompletionProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Nested:
              MappingWalker.Visit<INestedProperty>(prop, (Action<INestedProperty>) (t =>
              {
                this._visitor.Visit(t);
                ++this._visitor.Depth;
                this.Accept(t.Properties);
                --this._visitor.Depth;
              }));
              continue;
            case FieldType.Object:
              MappingWalker.Visit<IObjectProperty>(prop, (Action<IObjectProperty>) (t =>
              {
                this._visitor.Visit(t);
                ++this._visitor.Depth;
                this.Accept(t.Properties);
                --this._visitor.Depth;
              }));
              continue;
            case FieldType.Murmur3Hash:
              MappingWalker.Visit<IMurmur3HashProperty>(prop, (Action<IMurmur3HashProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.TokenCount:
              MappingWalker.Visit<ITokenCountProperty>(prop, (Action<ITokenCountProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Percolator:
              MappingWalker.Visit<IPercolatorProperty>(prop, (Action<IPercolatorProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.Integer:
            case FieldType.Long:
            case FieldType.Short:
            case FieldType.Byte:
            case FieldType.Float:
            case FieldType.HalfFloat:
            case FieldType.ScaledFloat:
            case FieldType.Double:
              MappingWalker.Visit<INumberProperty>(prop, (Action<INumberProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.IntegerRange:
              MappingWalker.Visit<IIntegerRangeProperty>(prop, (Action<IIntegerRangeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.FloatRange:
              MappingWalker.Visit<IFloatRangeProperty>(prop, (Action<IFloatRangeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.LongRange:
              MappingWalker.Visit<ILongRangeProperty>(prop, (Action<ILongRangeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.DoubleRange:
              MappingWalker.Visit<IDoubleRangeProperty>(prop, (Action<IDoubleRangeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.DateRange:
              MappingWalker.Visit<IDateRangeProperty>(prop, (Action<IDateRangeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.IpRange:
              MappingWalker.Visit<IIpRangeProperty>(prop, (Action<IIpRangeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Join:
              MappingWalker.Visit<IJoinProperty>(prop, (Action<IJoinProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.RankFeature:
              MappingWalker.Visit<IRankFeatureProperty>(prop, (Action<IRankFeatureProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.RankFeatures:
              MappingWalker.Visit<IRankFeaturesProperty>(prop, (Action<IRankFeaturesProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.Flattened:
              MappingWalker.Visit<IFlattenedProperty>(prop, (Action<IFlattenedProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.Shape:
              MappingWalker.Visit<IShapeProperty>(prop, (Action<IShapeProperty>) (t =>
              {
                this._visitor.Visit(t);
                this.Accept(t.Fields);
              }));
              continue;
            case FieldType.Histogram:
              MappingWalker.Visit<IHistogramProperty>(prop, (Action<IHistogramProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.ConstantKeyword:
              MappingWalker.Visit<IConstantKeywordProperty>(prop, (Action<IConstantKeywordProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.Point:
              MappingWalker.Visit<IPointProperty>(prop, (Action<IPointProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.Version:
              MappingWalker.Visit<IVersionProperty>(prop, (Action<IVersionProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.DenseVector:
              MappingWalker.Visit<IDenseVectorProperty>(prop, (Action<IDenseVectorProperty>) (t => this._visitor.Visit(t)));
              continue;
            case FieldType.MatchOnlyText:
              MappingWalker.Visit<IMatchOnlyTextProperty>(prop, (Action<IMatchOnlyTextProperty>) (t => this._visitor.Visit(t)));
              continue;
            default:
              continue;
          }
        }
      }
    }
  }
}
