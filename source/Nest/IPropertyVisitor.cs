// Decompiled with JetBrains decompiler
// Type: Nest.IPropertyVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Reflection;

namespace Nest
{
  public interface IPropertyVisitor
  {
    void Visit(
      ITextProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IKeywordProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      INumberProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IBooleanProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IDateProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IDateNanosProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IBinaryProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      INestedProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IObjectProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IGeoPointProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IGeoShapeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IShapeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IPointProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      ICompletionProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IIpProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IMurmur3HashProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      ITokenCountProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IPercolatorProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IIntegerRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IFloatRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      ILongRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IDoubleRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IDateRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IIpRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IJoinProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IRankFeatureProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IRankFeaturesProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IFlattenedProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IHistogramProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IConstantKeywordProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      ISearchAsYouTypeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IFieldAliasProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IWildcardProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IVersionProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IDenseVectorProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    void Visit(
      IMatchOnlyTextProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute);

    IProperty Visit(PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute);

    bool SkipProperty(PropertyInfo propertyInfo, ElasticsearchPropertyAttributeBase attribute);
  }
}
