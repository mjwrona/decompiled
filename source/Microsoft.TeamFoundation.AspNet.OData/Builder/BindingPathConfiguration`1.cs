// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.BindingPathConfiguration`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public class BindingPathConfiguration<TStructuralType> where TStructuralType : class
  {
    private readonly NavigationSourceConfiguration _navigationSource;
    private readonly StructuralTypeConfiguration<TStructuralType> _structuralType;
    private readonly ODataModelBuilder _modelBuilder;
    private readonly IList<MemberInfo> _bindingPath;

    public BindingPathConfiguration(
      ODataModelBuilder modelBuilder,
      StructuralTypeConfiguration<TStructuralType> structuralType,
      NavigationSourceConfiguration navigationSource)
      : this(modelBuilder, structuralType, navigationSource, (IList<MemberInfo>) new List<MemberInfo>())
    {
    }

    public BindingPathConfiguration(
      ODataModelBuilder modelBuilder,
      StructuralTypeConfiguration<TStructuralType> structuralType,
      NavigationSourceConfiguration navigationSource,
      IList<MemberInfo> bindingPath)
    {
      if (modelBuilder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelBuilder));
      if (structuralType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralType));
      if (navigationSource == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationSource));
      if (bindingPath == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (bindingPath));
      this._modelBuilder = modelBuilder;
      this._navigationSource = navigationSource;
      this._structuralType = structuralType;
      this._bindingPath = bindingPath;
    }

    public IList<MemberInfo> Path => this._bindingPath;

    public string BindingPath => this._bindingPath.ConvertBindingPath();

    public BindingPathConfiguration<TTargetType> HasManyPath<TTargetType>(
      Expression<Func<TStructuralType, IEnumerable<TTargetType>>> pathExpression)
      where TTargetType : class
    {
      return this.HasManyPath<TTargetType>(pathExpression, false);
    }

    public BindingPathConfiguration<TTargetType> HasManyPath<TTargetType>(
      Expression<Func<TStructuralType, IEnumerable<TTargetType>>> pathExpression,
      bool contained)
      where TTargetType : class
    {
      PropertyInfo propertyInfo = pathExpression != null ? PropertySelectorVisitor.GetSelectedProperty((Expression) pathExpression) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (pathExpression));
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add((MemberInfo) propertyInfo);
      StructuralTypeConfiguration<TTargetType> structuralType;
      if (contained)
      {
        structuralType = (StructuralTypeConfiguration<TTargetType>) this._modelBuilder.EntityType<TTargetType>();
        this._structuralType.ContainsMany<TTargetType>(pathExpression);
      }
      else
      {
        structuralType = (StructuralTypeConfiguration<TTargetType>) this._modelBuilder.ComplexType<TTargetType>();
        this._structuralType.CollectionProperty<TTargetType>(pathExpression);
      }
      return new BindingPathConfiguration<TTargetType>(this._modelBuilder, structuralType, this._navigationSource, bindingPath);
    }

    public BindingPathConfiguration<TTargetType> HasManyPath<TTargetType, TDerivedType>(
      Expression<Func<TDerivedType, IEnumerable<TTargetType>>> pathExpression)
      where TTargetType : class
      where TDerivedType : class, TStructuralType
    {
      return this.HasManyPath<TTargetType, TDerivedType>(pathExpression, false);
    }

    public BindingPathConfiguration<TTargetType> HasManyPath<TTargetType, TDerivedType>(
      Expression<Func<TDerivedType, IEnumerable<TTargetType>>> pathExpression,
      bool contained)
      where TTargetType : class
      where TDerivedType : class, TStructuralType
    {
      PropertyInfo propertyInfo = pathExpression != null ? PropertySelectorVisitor.GetSelectedProperty((Expression) pathExpression) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (pathExpression));
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add(TypeHelper.AsMemberInfo(typeof (TDerivedType)));
      bindingPath.Add((MemberInfo) propertyInfo);
      StructuralTypeConfiguration<TDerivedType> typeConfiguration = this._structuralType.Configuration.Kind != EdmTypeKind.Entity ? (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.ComplexType<TDerivedType>().DerivesFrom<TStructuralType>() : (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.EntityType<TDerivedType>().DerivesFrom<TStructuralType>();
      StructuralTypeConfiguration<TTargetType> structuralType;
      if (contained)
      {
        structuralType = (StructuralTypeConfiguration<TTargetType>) this._modelBuilder.EntityType<TTargetType>();
        typeConfiguration.ContainsMany<TTargetType>(pathExpression);
      }
      else
      {
        structuralType = (StructuralTypeConfiguration<TTargetType>) this._modelBuilder.ComplexType<TTargetType>();
        typeConfiguration.CollectionProperty<TTargetType>(pathExpression);
      }
      return new BindingPathConfiguration<TTargetType>(this._modelBuilder, structuralType, this._navigationSource, bindingPath);
    }

    public BindingPathConfiguration<TTargetType> HasSinglePath<TTargetType>(
      Expression<Func<TStructuralType, TTargetType>> pathExpression)
      where TTargetType : class
    {
      return this.HasSinglePath<TTargetType>(pathExpression, false, false);
    }

    public BindingPathConfiguration<TTargetType> HasSinglePath<TTargetType>(
      Expression<Func<TStructuralType, TTargetType>> pathExpression,
      bool required,
      bool contained)
      where TTargetType : class
    {
      PropertyInfo propertyInfo = pathExpression != null ? PropertySelectorVisitor.GetSelectedProperty((Expression) pathExpression) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (pathExpression));
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add((MemberInfo) propertyInfo);
      StructuralTypeConfiguration<TTargetType> structuralType;
      if (contained)
      {
        structuralType = (StructuralTypeConfiguration<TTargetType>) this._modelBuilder.EntityType<TTargetType>();
        if (required)
          this._structuralType.ContainsRequired<TTargetType>(pathExpression);
        else
          this._structuralType.ContainsOptional<TTargetType>(pathExpression);
      }
      else
      {
        structuralType = (StructuralTypeConfiguration<TTargetType>) this._modelBuilder.ComplexType<TTargetType>();
        this._structuralType.ComplexProperty<TTargetType>(pathExpression).OptionalProperty = !required;
      }
      return new BindingPathConfiguration<TTargetType>(this._modelBuilder, structuralType, this._navigationSource, bindingPath);
    }

    public BindingPathConfiguration<TTargetType> HasSinglePath<TTargetType, TDerivedType>(
      Expression<Func<TDerivedType, TTargetType>> pathExpression)
      where TTargetType : class
      where TDerivedType : class, TStructuralType
    {
      return this.HasSinglePath<TTargetType, TDerivedType>(pathExpression, false, false);
    }

    public BindingPathConfiguration<TTargetType> HasSinglePath<TTargetType, TDerivedType>(
      Expression<Func<TDerivedType, TTargetType>> pathExpression,
      bool required,
      bool contained)
      where TTargetType : class
      where TDerivedType : class, TStructuralType
    {
      PropertyInfo propertyInfo = pathExpression != null ? PropertySelectorVisitor.GetSelectedProperty((Expression) pathExpression) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (pathExpression));
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add(TypeHelper.AsMemberInfo(typeof (TDerivedType)));
      bindingPath.Add((MemberInfo) propertyInfo);
      StructuralTypeConfiguration<TDerivedType> typeConfiguration = this._structuralType.Configuration.Kind != EdmTypeKind.Entity ? (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.ComplexType<TDerivedType>().DerivesFrom<TStructuralType>() : (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.EntityType<TDerivedType>().DerivesFrom<TStructuralType>();
      StructuralTypeConfiguration<TTargetType> structuralType;
      if (contained)
      {
        structuralType = (StructuralTypeConfiguration<TTargetType>) this._modelBuilder.EntityType<TTargetType>();
        if (required)
          typeConfiguration.ContainsRequired<TTargetType>(pathExpression);
        else
          typeConfiguration.ContainsOptional<TTargetType>(pathExpression);
      }
      else
      {
        structuralType = (StructuralTypeConfiguration<TTargetType>) this._modelBuilder.ComplexType<TTargetType>();
        typeConfiguration.ComplexProperty<TTargetType>(pathExpression).OptionalProperty = !required;
      }
      return new BindingPathConfiguration<TTargetType>(this._modelBuilder, structuralType, this._navigationSource, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasManyBinding<TTargetType>(
      Expression<Func<TStructuralType, IEnumerable<TTargetType>>> navigationExpression,
      string targetEntitySet)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(targetEntitySet))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = this._structuralType.HasMany<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add((MemberInfo) navigationConfiguration.PropertyInfo);
      NavigationSourceConfiguration configuration = this._modelBuilder.EntitySet<TTargetType>(targetEntitySet).Configuration;
      return this._navigationSource.AddBinding(navigationConfiguration, configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasManyBinding<TTargetType, TDerivedType>(
      Expression<Func<TDerivedType, IEnumerable<TTargetType>>> navigationExpression,
      string targetEntitySet)
      where TTargetType : class
      where TDerivedType : class, TStructuralType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(targetEntitySet))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = (this._structuralType.Configuration.Kind != EdmTypeKind.Entity ? (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.ComplexType<TDerivedType>().DerivesFrom<TStructuralType>() : (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.EntityType<TDerivedType>().DerivesFrom<TStructuralType>()).HasMany<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add(TypeHelper.AsMemberInfo(typeof (TDerivedType)));
      bindingPath.Add((MemberInfo) navigationConfiguration.PropertyInfo);
      NavigationSourceConfiguration configuration = this._modelBuilder.EntitySet<TTargetType>(targetEntitySet).Configuration;
      return this._navigationSource.AddBinding(navigationConfiguration, configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasRequiredBinding<TTargetType>(
      Expression<Func<TStructuralType, TTargetType>> navigationExpression,
      string targetEntitySet)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(targetEntitySet))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = this._structuralType.HasRequired<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add((MemberInfo) navigationConfiguration.PropertyInfo);
      NavigationSourceConfiguration configuration = this._modelBuilder.EntitySet<TTargetType>(targetEntitySet).Configuration;
      return this._navigationSource.AddBinding(navigationConfiguration, configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasRequiredBinding<TTargetType, TDerivedType>(
      Expression<Func<TDerivedType, TTargetType>> navigationExpression,
      string targetEntitySet)
      where TTargetType : class
      where TDerivedType : class, TStructuralType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(targetEntitySet))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = (this._structuralType.Configuration.Kind != EdmTypeKind.Entity ? (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.ComplexType<TDerivedType>().DerivesFrom<TStructuralType>() : (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.EntityType<TDerivedType>().DerivesFrom<TStructuralType>()).HasRequired<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add(TypeHelper.AsMemberInfo(typeof (TDerivedType)));
      bindingPath.Add((MemberInfo) navigationConfiguration.PropertyInfo);
      NavigationSourceConfiguration configuration = this._modelBuilder.EntitySet<TTargetType>(targetEntitySet).Configuration;
      return this._navigationSource.AddBinding(navigationConfiguration, configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasOptionalBinding<TTargetType>(
      Expression<Func<TStructuralType, TTargetType>> navigationExpression,
      string targetEntitySet)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(targetEntitySet))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = this._structuralType.HasOptional<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add((MemberInfo) navigationConfiguration.PropertyInfo);
      NavigationSourceConfiguration configuration = this._modelBuilder.EntitySet<TTargetType>(targetEntitySet).Configuration;
      return this._navigationSource.AddBinding(navigationConfiguration, configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasOptionalBinding<TTargetType, TDerivedType>(
      Expression<Func<TDerivedType, TTargetType>> navigationExpression,
      string targetEntitySet)
      where TTargetType : class
      where TDerivedType : class, TStructuralType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(targetEntitySet))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = (this._structuralType.Configuration.Kind != EdmTypeKind.Entity ? (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.ComplexType<TDerivedType>().DerivesFrom<TStructuralType>() : (StructuralTypeConfiguration<TDerivedType>) this._modelBuilder.EntityType<TDerivedType>().DerivesFrom<TStructuralType>()).HasOptional<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>((IEnumerable<MemberInfo>) this._bindingPath);
      bindingPath.Add(TypeHelper.AsMemberInfo(typeof (TDerivedType)));
      bindingPath.Add((MemberInfo) navigationConfiguration.PropertyInfo);
      NavigationSourceConfiguration configuration = this._modelBuilder.EntitySet<TTargetType>(targetEntitySet).Configuration;
      return this._navigationSource.AddBinding(navigationConfiguration, configuration, bindingPath);
    }
  }
}
