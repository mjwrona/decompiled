// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.StructuralTypeConfiguration`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public abstract class StructuralTypeConfiguration<TStructuralType> where TStructuralType : class
  {
    private StructuralTypeConfiguration _configuration;

    protected StructuralTypeConfiguration(StructuralTypeConfiguration configuration) => this._configuration = configuration != null ? configuration : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));

    public IEnumerable<PropertyConfiguration> Properties => this._configuration.Properties;

    public string FullName => this._configuration.FullName;

    public string Namespace
    {
      get => this._configuration.Namespace;
      set => this._configuration.Namespace = value;
    }

    public string Name
    {
      get => this._configuration.Name;
      set => this._configuration.Name = value;
    }

    public bool IsOpen => this._configuration.IsOpen;

    internal StructuralTypeConfiguration Configuration => this._configuration;

    public virtual void Ignore<TProperty>(
      Expression<Func<TStructuralType, TProperty>> propertyExpression)
    {
      this._configuration.RemoveProperty(PropertySelectorVisitor.GetSelectedProperty((Expression) propertyExpression));
    }

    public LengthPropertyConfiguration Property(
      Expression<Func<TStructuralType, string>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, true) as LengthPropertyConfiguration;
    }

    public LengthPropertyConfiguration Property(
      Expression<Func<TStructuralType, byte[]>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, true) as LengthPropertyConfiguration;
    }

    public PrimitivePropertyConfiguration Property(
      Expression<Func<TStructuralType, Stream>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, true);
    }

    public DecimalPropertyConfiguration Property(
      Expression<Func<TStructuralType, Decimal?>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, true) as DecimalPropertyConfiguration;
    }

    public DecimalPropertyConfiguration Property(
      Expression<Func<TStructuralType, Decimal>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, false) as DecimalPropertyConfiguration;
    }

    public PrecisionPropertyConfiguration Property(
      Expression<Func<TStructuralType, TimeOfDay?>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, true) as PrecisionPropertyConfiguration;
    }

    public PrecisionPropertyConfiguration Property(
      Expression<Func<TStructuralType, TimeOfDay>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, false) as PrecisionPropertyConfiguration;
    }

    public PrecisionPropertyConfiguration Property(
      Expression<Func<TStructuralType, TimeSpan?>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, true) as PrecisionPropertyConfiguration;
    }

    public PrecisionPropertyConfiguration Property(
      Expression<Func<TStructuralType, TimeSpan>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, false) as PrecisionPropertyConfiguration;
    }

    public PrecisionPropertyConfiguration Property(
      Expression<Func<TStructuralType, DateTimeOffset?>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, true) as PrecisionPropertyConfiguration;
    }

    public PrecisionPropertyConfiguration Property(
      Expression<Func<TStructuralType, DateTimeOffset>> propertyExpression)
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, false) as PrecisionPropertyConfiguration;
    }

    public PrimitivePropertyConfiguration Property<T>(
      Expression<Func<TStructuralType, T?>> propertyExpression)
      where T : struct
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, true);
    }

    public PrimitivePropertyConfiguration Property<T>(
      Expression<Func<TStructuralType, T>> propertyExpression)
      where T : struct
    {
      return this.GetPrimitivePropertyConfiguration((Expression) propertyExpression, false);
    }

    public EnumPropertyConfiguration EnumProperty<T>(
      Expression<Func<TStructuralType, T?>> propertyExpression)
      where T : struct
    {
      return this.GetEnumPropertyConfiguration((Expression) propertyExpression, true);
    }

    public EnumPropertyConfiguration EnumProperty<T>(
      Expression<Func<TStructuralType, T>> propertyExpression)
      where T : struct
    {
      return this.GetEnumPropertyConfiguration((Expression) propertyExpression, false);
    }

    public ComplexPropertyConfiguration ComplexProperty<TComplexType>(
      Expression<Func<TStructuralType, TComplexType>> propertyExpression)
    {
      return this.GetComplexPropertyConfiguration((Expression) propertyExpression);
    }

    public CollectionPropertyConfiguration CollectionProperty<TElementType>(
      Expression<Func<TStructuralType, IEnumerable<TElementType>>> propertyExpression)
    {
      return this.GetCollectionPropertyConfiguration((Expression) propertyExpression);
    }

    public void HasDynamicProperties(
      Expression<Func<TStructuralType, IDictionary<string, object>>> propertyExpression)
    {
      this._configuration.AddDynamicPropertyDictionary(PropertySelectorVisitor.GetSelectedProperty((Expression) propertyExpression));
    }

    public NavigationPropertyConfiguration HasMany<TTargetEntity>(
      Expression<Func<TStructuralType, IEnumerable<TTargetEntity>>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      return this.GetOrCreateNavigationProperty((Expression) navigationPropertyExpression, EdmMultiplicity.Many);
    }

    public NavigationPropertyConfiguration HasOptional<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      return this.GetOrCreateNavigationProperty((Expression) navigationPropertyExpression, EdmMultiplicity.ZeroOrOne);
    }

    public NavigationPropertyConfiguration HasOptional<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression,
      Expression<Func<TStructuralType, TTargetEntity, bool>> referentialConstraintExpression)
      where TTargetEntity : class
    {
      return this.HasNavigationProperty<TTargetEntity>(navigationPropertyExpression, referentialConstraintExpression, EdmMultiplicity.ZeroOrOne, (Expression) null);
    }

    public NavigationPropertyConfiguration HasOptional<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression,
      Expression<Func<TStructuralType, TTargetEntity, bool>> referentialConstraintExpression,
      Expression<Func<TTargetEntity, IEnumerable<TStructuralType>>> partnerExpression)
      where TTargetEntity : class
    {
      return this.HasNavigationProperty<TTargetEntity>(navigationPropertyExpression, referentialConstraintExpression, EdmMultiplicity.ZeroOrOne, (Expression) partnerExpression);
    }

    public NavigationPropertyConfiguration HasOptional<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression,
      Expression<Func<TStructuralType, TTargetEntity, bool>> referentialConstraintExpression,
      Expression<Func<TTargetEntity, TStructuralType>> partnerExpression)
      where TTargetEntity : class
    {
      return this.HasNavigationProperty<TTargetEntity>(navigationPropertyExpression, referentialConstraintExpression, EdmMultiplicity.ZeroOrOne, (Expression) partnerExpression);
    }

    public NavigationPropertyConfiguration HasRequired<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      return this.GetOrCreateNavigationProperty((Expression) navigationPropertyExpression, EdmMultiplicity.One);
    }

    public NavigationPropertyConfiguration HasRequired<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression,
      Expression<Func<TStructuralType, TTargetEntity, bool>> referentialConstraintExpression,
      Expression<Func<TTargetEntity, IEnumerable<TStructuralType>>> partnerExpression)
      where TTargetEntity : class
    {
      return this.HasNavigationProperty<TTargetEntity>(navigationPropertyExpression, referentialConstraintExpression, EdmMultiplicity.One, (Expression) partnerExpression);
    }

    public NavigationPropertyConfiguration HasRequired<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression,
      Expression<Func<TStructuralType, TTargetEntity, bool>> referentialConstraintExpression)
      where TTargetEntity : class
    {
      return this.HasNavigationProperty<TTargetEntity>(navigationPropertyExpression, referentialConstraintExpression, EdmMultiplicity.One, (Expression) null);
    }

    public NavigationPropertyConfiguration HasRequired<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression,
      Expression<Func<TStructuralType, TTargetEntity, bool>> referentialConstraintExpression,
      Expression<Func<TTargetEntity, TStructuralType>> partnerExpression)
      where TTargetEntity : class
    {
      return this.HasNavigationProperty<TTargetEntity>(navigationPropertyExpression, referentialConstraintExpression, EdmMultiplicity.One, (Expression) partnerExpression);
    }

    private NavigationPropertyConfiguration HasNavigationProperty<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression,
      Expression<Func<TStructuralType, TTargetEntity, bool>> referentialConstraintExpression,
      EdmMultiplicity multiplicity,
      Expression partnerProperty)
      where TTargetEntity : class
    {
      NavigationPropertyConfiguration navigationProperty = this.GetOrCreateNavigationProperty((Expression) navigationPropertyExpression, multiplicity);
      foreach (KeyValuePair<PropertyInfo, PropertyInfo> constraint in (IEnumerable<KeyValuePair<PropertyInfo, PropertyInfo>>) PropertyPairSelectorVisitor.GetSelectedProperty((Expression) referentialConstraintExpression))
        navigationProperty.HasConstraint(constraint);
      if (partnerProperty != null)
      {
        PropertyInfo partnerPropertyInfo = PropertySelectorVisitor.GetSelectedProperty(partnerProperty);
        if (typeof (IEnumerable).IsAssignableFrom(partnerPropertyInfo.PropertyType))
          this._configuration.ModelBuilder.EntityType<TTargetEntity>().HasMany<TStructuralType>((Expression<Func<TTargetEntity, IEnumerable<TStructuralType>>>) partnerProperty);
        else
          this._configuration.ModelBuilder.EntityType<TTargetEntity>().HasRequired<TStructuralType>((Expression<Func<TTargetEntity, TStructuralType>>) partnerProperty);
        NavigationPropertyConfiguration propertyConfiguration = this._configuration.ModelBuilder.EntityType<TTargetEntity>().Properties.First<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (p => p.Name == partnerPropertyInfo.Name)) as NavigationPropertyConfiguration;
        navigationProperty.Partner = propertyConfiguration;
      }
      return navigationProperty;
    }

    public NavigationPropertyConfiguration ContainsMany<TTargetEntity>(
      Expression<Func<TStructuralType, IEnumerable<TTargetEntity>>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      return this.GetOrCreateContainedNavigationProperty((Expression) navigationPropertyExpression, EdmMultiplicity.Many);
    }

    public NavigationPropertyConfiguration ContainsOptional<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      return this.GetOrCreateContainedNavigationProperty((Expression) navigationPropertyExpression, EdmMultiplicity.ZeroOrOne);
    }

    public NavigationPropertyConfiguration ContainsRequired<TTargetEntity>(
      Expression<Func<TStructuralType, TTargetEntity>> navigationPropertyExpression)
      where TTargetEntity : class
    {
      return this.GetOrCreateContainedNavigationProperty((Expression) navigationPropertyExpression, EdmMultiplicity.One);
    }

    public StructuralTypeConfiguration<TStructuralType> Count()
    {
      this._configuration.QueryConfiguration.SetCount(true);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Count(QueryOptionSetting setting)
    {
      this._configuration.QueryConfiguration.SetCount(setting == QueryOptionSetting.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> OrderBy(
      QueryOptionSetting setting,
      params string[] properties)
    {
      this._configuration.QueryConfiguration.SetOrderBy((IEnumerable<string>) properties, setting == QueryOptionSetting.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> OrderBy(params string[] properties)
    {
      this._configuration.QueryConfiguration.SetOrderBy((IEnumerable<string>) properties, true);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> OrderBy(QueryOptionSetting setting)
    {
      this._configuration.QueryConfiguration.SetOrderBy((IEnumerable<string>) null, setting == QueryOptionSetting.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> OrderBy()
    {
      this._configuration.QueryConfiguration.SetOrderBy((IEnumerable<string>) null, true);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Filter(
      QueryOptionSetting setting,
      params string[] properties)
    {
      this._configuration.QueryConfiguration.SetFilter((IEnumerable<string>) properties, setting == QueryOptionSetting.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Filter(params string[] properties)
    {
      this._configuration.QueryConfiguration.SetFilter((IEnumerable<string>) properties, true);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Filter(QueryOptionSetting setting)
    {
      this._configuration.QueryConfiguration.SetFilter((IEnumerable<string>) null, setting == QueryOptionSetting.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Filter()
    {
      this._configuration.QueryConfiguration.SetFilter((IEnumerable<string>) null, true);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Select(
      SelectExpandType selectType,
      params string[] properties)
    {
      this._configuration.QueryConfiguration.SetSelect((IEnumerable<string>) properties, selectType);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Select(params string[] properties)
    {
      this._configuration.QueryConfiguration.SetSelect((IEnumerable<string>) properties, SelectExpandType.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Select(SelectExpandType selectType)
    {
      this._configuration.QueryConfiguration.SetSelect((IEnumerable<string>) null, selectType);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Select()
    {
      this._configuration.QueryConfiguration.SetSelect((IEnumerable<string>) null, SelectExpandType.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Page(int? maxTopValue, int? pageSizeValue)
    {
      this._configuration.QueryConfiguration.SetMaxTop(maxTopValue);
      this._configuration.QueryConfiguration.SetPageSize(pageSizeValue);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Page()
    {
      this._configuration.QueryConfiguration.SetMaxTop(new int?());
      this._configuration.QueryConfiguration.SetPageSize(new int?());
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Expand(
      int maxDepth,
      SelectExpandType expandType,
      params string[] properties)
    {
      this._configuration.QueryConfiguration.SetExpand((IEnumerable<string>) properties, new int?(maxDepth), expandType);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Expand(params string[] properties)
    {
      this._configuration.QueryConfiguration.SetExpand((IEnumerable<string>) properties, new int?(), SelectExpandType.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Expand(
      int maxDepth,
      params string[] properties)
    {
      this._configuration.QueryConfiguration.SetExpand((IEnumerable<string>) properties, new int?(maxDepth), SelectExpandType.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Expand(
      SelectExpandType expandType,
      params string[] properties)
    {
      this._configuration.QueryConfiguration.SetExpand((IEnumerable<string>) properties, new int?(), expandType);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Expand(
      SelectExpandType expandType,
      int maxDepth)
    {
      this._configuration.QueryConfiguration.SetExpand((IEnumerable<string>) null, new int?(maxDepth), expandType);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Expand(int maxDepth)
    {
      this._configuration.QueryConfiguration.SetExpand((IEnumerable<string>) null, new int?(maxDepth), SelectExpandType.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Expand(SelectExpandType expandType)
    {
      this._configuration.QueryConfiguration.SetExpand((IEnumerable<string>) null, new int?(), expandType);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    public StructuralTypeConfiguration<TStructuralType> Expand()
    {
      this._configuration.QueryConfiguration.SetExpand((IEnumerable<string>) null, new int?(), SelectExpandType.Allowed);
      this._configuration.AddedExplicitly = true;
      return this;
    }

    internal NavigationPropertyConfiguration GetOrCreateNavigationProperty(
      Expression navigationPropertyExpression,
      EdmMultiplicity multiplicity)
    {
      return this._configuration.AddNavigationProperty(PropertySelectorVisitor.GetSelectedProperty(navigationPropertyExpression), multiplicity);
    }

    internal NavigationPropertyConfiguration GetOrCreateContainedNavigationProperty(
      Expression navigationPropertyExpression,
      EdmMultiplicity multiplicity)
    {
      return this._configuration.AddContainedNavigationProperty(PropertySelectorVisitor.GetSelectedProperty(navigationPropertyExpression), multiplicity);
    }

    private PrimitivePropertyConfiguration GetPrimitivePropertyConfiguration(
      Expression propertyExpression,
      bool optional)
    {
      PrimitivePropertyConfiguration propertyConfiguration = this._configuration.AddProperty(PropertySelectorVisitor.GetSelectedProperty(propertyExpression));
      if (optional)
        propertyConfiguration.IsOptional();
      return propertyConfiguration;
    }

    private EnumPropertyConfiguration GetEnumPropertyConfiguration(
      Expression propertyExpression,
      bool optional)
    {
      EnumPropertyConfiguration propertyConfiguration = this._configuration.AddEnumProperty(PropertySelectorVisitor.GetSelectedProperty(propertyExpression));
      if (optional)
        propertyConfiguration.IsOptional();
      return propertyConfiguration;
    }

    private ComplexPropertyConfiguration GetComplexPropertyConfiguration(
      Expression propertyExpression,
      bool optional = false)
    {
      ComplexPropertyConfiguration propertyConfiguration = this._configuration.AddComplexProperty(PropertySelectorVisitor.GetSelectedProperty(propertyExpression));
      if (optional)
        propertyConfiguration.IsOptional();
      else
        propertyConfiguration.IsRequired();
      return propertyConfiguration;
    }

    private CollectionPropertyConfiguration GetCollectionPropertyConfiguration(
      Expression propertyExpression,
      bool optional = false)
    {
      CollectionPropertyConfiguration propertyConfiguration = this._configuration.AddCollectionProperty(PropertySelectorVisitor.GetSelectedProperty(propertyExpression));
      if (optional)
        propertyConfiguration.IsOptional();
      else
        propertyConfiguration.IsRequired();
      return propertyConfiguration;
    }
  }
}
