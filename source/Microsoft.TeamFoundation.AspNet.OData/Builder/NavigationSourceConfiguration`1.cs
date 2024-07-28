// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.NavigationSourceConfiguration`1
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
  public abstract class NavigationSourceConfiguration<TEntityType> where TEntityType : class
  {
    private readonly NavigationSourceConfiguration _configuration;
    private readonly EntityTypeConfiguration<TEntityType> _entityType;
    private readonly ODataModelBuilder _modelBuilder;
    private readonly BindingPathConfiguration<TEntityType> _binding;

    internal NavigationSourceConfiguration(
      ODataModelBuilder modelBuilder,
      NavigationSourceConfiguration configuration)
    {
      if (modelBuilder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelBuilder));
      this._configuration = configuration != null ? configuration : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      this._modelBuilder = modelBuilder;
      this._entityType = new EntityTypeConfiguration<TEntityType>(modelBuilder, this._configuration.EntityType);
      this._binding = new BindingPathConfiguration<TEntityType>(modelBuilder, (StructuralTypeConfiguration<TEntityType>) this._entityType, this._configuration);
    }

    public EntityTypeConfiguration<TEntityType> EntityType => this._entityType;

    internal NavigationSourceConfiguration Configuration => this._configuration;

    public BindingPathConfiguration<TEntityType> Binding => this._binding;

    public NavigationPropertyBindingConfiguration HasManyBinding<TTargetType, TDerivedEntityType>(
      Expression<Func<TDerivedEntityType, IEnumerable<TTargetType>>> navigationExpression,
      string entitySetName)
      where TTargetType : class
      where TDerivedEntityType : class, TEntityType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(entitySetName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (entitySetName));
      NavigationPropertyConfiguration navigationConfiguration = this._modelBuilder.EntityType<TDerivedEntityType>().DerivesFrom<TEntityType>().HasMany<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        TypeHelper.AsMemberInfo(typeof (TDerivedEntityType)),
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      return this.Configuration.AddBinding(navigationConfiguration, this._modelBuilder.EntitySet<TTargetType>(entitySetName)._configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasManyBinding<TTargetType>(
      Expression<Func<TEntityType, IEnumerable<TTargetType>>> navigationExpression,
      string entitySetName)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(entitySetName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (entitySetName));
      return this.Configuration.AddBinding(this.EntityType.HasMany<TTargetType>(navigationExpression), this._modelBuilder.EntitySet<TTargetType>(entitySetName)._configuration);
    }

    public NavigationPropertyBindingConfiguration HasManyBinding<TTargetType>(
      Expression<Func<TEntityType, IEnumerable<TTargetType>>> navigationExpression,
      NavigationSourceConfiguration<TTargetType> targetEntitySet)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (targetEntitySet == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (targetEntitySet));
      return this.Configuration.AddBinding(this.EntityType.HasMany<TTargetType>(navigationExpression), targetEntitySet.Configuration);
    }

    public NavigationPropertyBindingConfiguration HasManyBinding<TTargetType, TDerivedEntityType>(
      Expression<Func<TDerivedEntityType, IEnumerable<TTargetType>>> navigationExpression,
      NavigationSourceConfiguration<TTargetType> targetEntitySet)
      where TTargetType : class
      where TDerivedEntityType : class, TEntityType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (targetEntitySet == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = this._modelBuilder.EntityType<TDerivedEntityType>().DerivesFrom<TEntityType>().HasMany<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        TypeHelper.AsMemberInfo(typeof (TDerivedEntityType)),
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      return this.Configuration.AddBinding(navigationConfiguration, targetEntitySet.Configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasRequiredBinding<TTargetType>(
      Expression<Func<TEntityType, TTargetType>> navigationExpression,
      string entitySetName)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(entitySetName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (entitySetName));
      return this.Configuration.AddBinding(this.EntityType.HasRequired<TTargetType>(navigationExpression), this._modelBuilder.EntitySet<TTargetType>(entitySetName).Configuration);
    }

    public NavigationPropertyBindingConfiguration HasRequiredBinding<TTargetType, TDerivedEntityType>(
      Expression<Func<TDerivedEntityType, TTargetType>> navigationExpression,
      string entitySetName)
      where TTargetType : class
      where TDerivedEntityType : class, TEntityType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(entitySetName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (entitySetName));
      NavigationPropertyConfiguration navigationConfiguration = this._modelBuilder.EntityType<TDerivedEntityType>().DerivesFrom<TEntityType>().HasRequired<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        TypeHelper.AsMemberInfo(typeof (TDerivedEntityType)),
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      return this.Configuration.AddBinding(navigationConfiguration, this._modelBuilder.EntitySet<TTargetType>(entitySetName).Configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasRequiredBinding<TTargetType>(
      Expression<Func<TEntityType, TTargetType>> navigationExpression,
      NavigationSourceConfiguration<TTargetType> targetEntitySet)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (targetEntitySet == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (targetEntitySet));
      return this.Configuration.AddBinding(this.EntityType.HasRequired<TTargetType>(navigationExpression), targetEntitySet.Configuration);
    }

    public NavigationPropertyBindingConfiguration HasRequiredBinding<TTargetType, TDerivedEntityType>(
      Expression<Func<TDerivedEntityType, TTargetType>> navigationExpression,
      NavigationSourceConfiguration<TTargetType> targetEntitySet)
      where TTargetType : class
      where TDerivedEntityType : class, TEntityType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (targetEntitySet == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = this._modelBuilder.EntityType<TDerivedEntityType>().DerivesFrom<TEntityType>().HasRequired<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        TypeHelper.AsMemberInfo(typeof (TDerivedEntityType)),
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      return this.Configuration.AddBinding(navigationConfiguration, targetEntitySet.Configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasOptionalBinding<TTargetType>(
      Expression<Func<TEntityType, TTargetType>> navigationExpression,
      string entitySetName)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(entitySetName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (entitySetName));
      return this.Configuration.AddBinding(this.EntityType.HasOptional<TTargetType>(navigationExpression), this._modelBuilder.EntitySet<TTargetType>(entitySetName).Configuration);
    }

    public NavigationPropertyBindingConfiguration HasOptionalBinding<TTargetType, TDerivedEntityType>(
      Expression<Func<TDerivedEntityType, TTargetType>> navigationExpression,
      string entitySetName)
      where TTargetType : class
      where TDerivedEntityType : class, TEntityType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(entitySetName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (entitySetName));
      NavigationPropertyConfiguration navigationConfiguration = this._modelBuilder.EntityType<TDerivedEntityType>().DerivesFrom<TEntityType>().HasOptional<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        TypeHelper.AsMemberInfo(typeof (TDerivedEntityType)),
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      return this.Configuration.AddBinding(navigationConfiguration, this._modelBuilder.EntitySet<TTargetType>(entitySetName).Configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasOptionalBinding<TTargetType>(
      Expression<Func<TEntityType, TTargetType>> navigationExpression,
      NavigationSourceConfiguration<TTargetType> targetEntitySet)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (targetEntitySet == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (targetEntitySet));
      return this.Configuration.AddBinding(this.EntityType.HasOptional<TTargetType>(navigationExpression), targetEntitySet.Configuration);
    }

    public NavigationPropertyBindingConfiguration HasOptionalBinding<TTargetType, TDerivedEntityType>(
      Expression<Func<TDerivedEntityType, TTargetType>> navigationExpression,
      NavigationSourceConfiguration<TTargetType> targetEntitySet)
      where TTargetType : class
      where TDerivedEntityType : class, TEntityType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (targetEntitySet == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (targetEntitySet));
      NavigationPropertyConfiguration navigationConfiguration = this._modelBuilder.EntityType<TDerivedEntityType>().DerivesFrom<TEntityType>().HasOptional<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        TypeHelper.AsMemberInfo(typeof (TDerivedEntityType)),
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      return this.Configuration.AddBinding(navigationConfiguration, targetEntitySet.Configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasSingletonBinding<TTargetType>(
      Expression<Func<TEntityType, TTargetType>> navigationExpression,
      string singletonName)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(singletonName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (singletonName));
      return this.Configuration.AddBinding(this.EntityType.HasRequired<TTargetType>(navigationExpression), this._modelBuilder.Singleton<TTargetType>(singletonName).Configuration);
    }

    public NavigationPropertyBindingConfiguration HasSingletonBinding<TTargetType, TDerivedEntityType>(
      Expression<Func<TDerivedEntityType, TTargetType>> navigationExpression,
      string singletonName)
      where TTargetType : class
      where TDerivedEntityType : class, TEntityType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (string.IsNullOrEmpty(singletonName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (singletonName));
      NavigationPropertyConfiguration navigationConfiguration = this._modelBuilder.EntityType<TDerivedEntityType>().DerivesFrom<TEntityType>().HasRequired<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        TypeHelper.AsMemberInfo(typeof (TDerivedEntityType)),
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      return this.Configuration.AddBinding(navigationConfiguration, this._modelBuilder.Singleton<TTargetType>(singletonName).Configuration, bindingPath);
    }

    public NavigationPropertyBindingConfiguration HasSingletonBinding<TTargetType>(
      Expression<Func<TEntityType, TTargetType>> navigationExpression,
      NavigationSourceConfiguration<TTargetType> targetSingleton)
      where TTargetType : class
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (targetSingleton == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (targetSingleton));
      return this.Configuration.AddBinding(this.EntityType.HasRequired<TTargetType>(navigationExpression), targetSingleton.Configuration);
    }

    public NavigationPropertyBindingConfiguration HasSingletonBinding<TTargetType, TDerivedEntityType>(
      Expression<Func<TDerivedEntityType, TTargetType>> navigationExpression,
      NavigationSourceConfiguration<TTargetType> targetSingleton)
      where TTargetType : class
      where TDerivedEntityType : class, TEntityType
    {
      if (navigationExpression == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationExpression));
      if (targetSingleton == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (targetSingleton));
      NavigationPropertyConfiguration navigationConfiguration = this._modelBuilder.EntityType<TDerivedEntityType>().DerivesFrom<TEntityType>().HasRequired<TTargetType>(navigationExpression);
      IList<MemberInfo> bindingPath = (IList<MemberInfo>) new List<MemberInfo>()
      {
        TypeHelper.AsMemberInfo(typeof (TDerivedEntityType)),
        (MemberInfo) navigationConfiguration.PropertyInfo
      };
      return this.Configuration.AddBinding(navigationConfiguration, targetSingleton.Configuration, bindingPath);
    }

    public void HasEditLink(
      Func<ResourceContext<TEntityType>, Uri> editLinkFactory,
      bool followsConventions)
    {
      if (editLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (editLinkFactory));
      this._configuration.HasEditLink(new SelfLinkBuilder<Uri>((Func<ResourceContext, Uri>) (context => editLinkFactory(NavigationSourceConfiguration<TEntityType>.UpCastEntityContext(context))), followsConventions));
    }

    public void HasReadLink(
      Func<ResourceContext<TEntityType>, Uri> readLinkFactory,
      bool followsConventions)
    {
      if (readLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readLinkFactory));
      this._configuration.HasReadLink(new SelfLinkBuilder<Uri>((Func<ResourceContext, Uri>) (context => readLinkFactory(NavigationSourceConfiguration<TEntityType>.UpCastEntityContext(context))), followsConventions));
    }

    public void HasIdLink(
      Func<ResourceContext<TEntityType>, Uri> idLinkFactory,
      bool followsConventions)
    {
      if (idLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (idLinkFactory));
      this._configuration.HasIdLink(new SelfLinkBuilder<Uri>((Func<ResourceContext, Uri>) (context => idLinkFactory(NavigationSourceConfiguration<TEntityType>.UpCastEntityContext(context))), followsConventions));
    }

    public void HasNavigationPropertyLink(
      NavigationPropertyConfiguration navigationProperty,
      Func<ResourceContext<TEntityType>, IEdmNavigationProperty, Uri> navigationLinkFactory,
      bool followsConventions)
    {
      if (navigationProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationProperty));
      if (navigationLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationLinkFactory));
      this._configuration.HasNavigationPropertyLink(navigationProperty, new NavigationLinkBuilder((Func<ResourceContext, IEdmNavigationProperty, Uri>) ((context, property) => navigationLinkFactory(NavigationSourceConfiguration<TEntityType>.UpCastEntityContext(context), property)), followsConventions));
    }

    public void HasNavigationPropertiesLink(
      IEnumerable<NavigationPropertyConfiguration> navigationProperties,
      Func<ResourceContext<TEntityType>, IEdmNavigationProperty, Uri> navigationLinkFactory,
      bool followsConventions)
    {
      if (navigationProperties == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationProperties));
      if (navigationLinkFactory == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationLinkFactory));
      this._configuration.HasNavigationPropertiesLink(navigationProperties, new NavigationLinkBuilder((Func<ResourceContext, IEdmNavigationProperty, Uri>) ((entity, property) => navigationLinkFactory(NavigationSourceConfiguration<TEntityType>.UpCastEntityContext(entity), property)), followsConventions));
    }

    public IEnumerable<NavigationPropertyBindingConfiguration> FindBindings(string propertyName) => this._configuration.FindBindings(propertyName);

    public IEnumerable<NavigationPropertyBindingConfiguration> FindBinding(
      NavigationPropertyConfiguration navigationConfiguration)
    {
      return this._configuration.FindBinding(navigationConfiguration);
    }

    public NavigationPropertyBindingConfiguration FindBinding(
      NavigationPropertyConfiguration navigationConfiguration,
      IList<MemberInfo> bindingPath)
    {
      return this._configuration.FindBinding(navigationConfiguration, bindingPath);
    }

    private static ResourceContext<TEntityType> UpCastEntityContext(ResourceContext context)
    {
      ResourceContext<TEntityType> resourceContext = new ResourceContext<TEntityType>();
      resourceContext.SerializerContext = context.SerializerContext;
      resourceContext.EdmObject = context.EdmObject;
      resourceContext.StructuredType = context.StructuredType;
      return resourceContext;
    }
  }
}
