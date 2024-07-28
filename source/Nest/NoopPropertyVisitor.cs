// Decompiled with JetBrains decompiler
// Type: Nest.NoopPropertyVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Reflection;

namespace Nest
{
  public class NoopPropertyVisitor : IPropertyVisitor
  {
    public virtual bool SkipProperty(
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
      return false;
    }

    public virtual void Visit(
      IBooleanProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IBinaryProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IObjectProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IGeoShapeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IShapeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IPointProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      ICompletionProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IMurmur3HashProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      ITokenCountProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public void Visit(
      IPercolatorProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public void Visit(
      IIntegerRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public void Visit(
      IFloatRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public void Visit(
      ILongRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public void Visit(
      IDoubleRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public void Visit(
      IDateRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public void Visit(
      IIpRangeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public void Visit(
      IJoinProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IRankFeatureProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IRankFeaturesProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IIpProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IGeoPointProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      INestedProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IDateProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IDateNanosProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      INumberProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      ITextProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IKeywordProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IFlattenedProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IHistogramProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IConstantKeywordProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      ISearchAsYouTypeProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IFieldAliasProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IWildcardProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IVersionProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IDenseVectorProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual void Visit(
      IMatchOnlyTextProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
    }

    public virtual IProperty Visit(
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
      return (IProperty) null;
    }

    public void Visit(
      IProperty type,
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
      switch (type)
      {
        case INestedProperty type1:
          this.Visit(type1, propertyInfo, attribute);
          break;
        case IObjectProperty type2:
          this.Visit(type2, propertyInfo, attribute);
          break;
        case IBinaryProperty type3:
          this.Visit(type3, propertyInfo, attribute);
          break;
        case IBooleanProperty type4:
          this.Visit(type4, propertyInfo, attribute);
          break;
        case IDateProperty type5:
          this.Visit(type5, propertyInfo, attribute);
          break;
        case IDateNanosProperty type6:
          this.Visit(type6, propertyInfo, attribute);
          break;
        case INumberProperty type7:
          this.Visit(type7, propertyInfo, attribute);
          break;
        case ITextProperty type8:
          this.Visit(type8, propertyInfo, attribute);
          break;
        case IKeywordProperty type9:
          this.Visit(type9, propertyInfo, attribute);
          break;
        case IGeoShapeProperty type10:
          this.Visit(type10, propertyInfo, attribute);
          break;
        case IShapeProperty type11:
          this.Visit(type11, propertyInfo, attribute);
          break;
        case IGeoPointProperty type12:
          this.Visit(type12, propertyInfo, attribute);
          break;
        case ICompletionProperty type13:
          this.Visit(type13, propertyInfo, attribute);
          break;
        case IIpProperty type14:
          this.Visit(type14, propertyInfo, attribute);
          break;
        case IMurmur3HashProperty type15:
          this.Visit(type15, propertyInfo, attribute);
          break;
        case ITokenCountProperty type16:
          this.Visit(type16, propertyInfo, attribute);
          break;
        case IPercolatorProperty type17:
          this.Visit(type17, propertyInfo, attribute);
          break;
        case IJoinProperty type18:
          this.Visit(type18, propertyInfo, attribute);
          break;
        case IIntegerRangeProperty type19:
          this.Visit(type19, propertyInfo, attribute);
          break;
        case ILongRangeProperty type20:
          this.Visit(type20, propertyInfo, attribute);
          break;
        case IDoubleRangeProperty type21:
          this.Visit(type21, propertyInfo, attribute);
          break;
        case IFloatRangeProperty type22:
          this.Visit(type22, propertyInfo, attribute);
          break;
        case IDateRangeProperty type23:
          this.Visit(type23, propertyInfo, attribute);
          break;
        case IIpRangeProperty type24:
          this.Visit(type24, propertyInfo, attribute);
          break;
        case IRankFeatureProperty type25:
          this.Visit(type25, propertyInfo, attribute);
          break;
        case IRankFeaturesProperty type26:
          this.Visit(type26, propertyInfo, attribute);
          break;
        case IHistogramProperty type27:
          this.Visit(type27, propertyInfo, attribute);
          break;
        case IConstantKeywordProperty type28:
          this.Visit(type28, propertyInfo, attribute);
          break;
        case IPointProperty type29:
          this.Visit(type29, propertyInfo, attribute);
          break;
        case ISearchAsYouTypeProperty type30:
          this.Visit(type30, propertyInfo, attribute);
          break;
        case IWildcardProperty type31:
          this.Visit(type31, propertyInfo, attribute);
          break;
        case IFieldAliasProperty type32:
          this.Visit(type32, propertyInfo, attribute);
          break;
        case IVersionProperty type33:
          this.Visit(type33, propertyInfo, attribute);
          break;
        case IDenseVectorProperty type34:
          this.Visit(type34, propertyInfo, attribute);
          break;
        case IMatchOnlyTextProperty type35:
          this.Visit(type35, propertyInfo, attribute);
          break;
      }
    }
  }
}
