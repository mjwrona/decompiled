// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EntityTypeConfiguration`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public class EntityTypeConfiguration<TEntityType> : StructuralTypeConfiguration<TEntityType> where TEntityType : class
  {
    private EntityTypeConfiguration _configuration;
    private EntityCollectionConfiguration<TEntityType> _collection;
    private ODataModelBuilder _modelBuilder;

    internal EntityTypeConfiguration(ODataModelBuilder modelBuilder)
      : this(modelBuilder, new EntityTypeConfiguration(modelBuilder, typeof (TEntityType)))
    {
    }

    internal EntityTypeConfiguration(
      ODataModelBuilder modelBuilder,
      EntityTypeConfiguration configuration)
      : base((StructuralTypeConfiguration) configuration)
    {
      this._modelBuilder = modelBuilder;
      this._configuration = configuration;
      this._collection = new EntityCollectionConfiguration<TEntityType>(configuration);
    }

    public EntityTypeConfiguration BaseType => this._configuration.BaseType;

    public IEnumerable<NavigationPropertyConfiguration> NavigationProperties => this._configuration.NavigationProperties;

    public EntityCollectionConfiguration<TEntityType> Collection => this._collection;

    public EntityTypeConfiguration<TEntityType> Abstract()
    {
      this._configuration.IsAbstract = new bool?(true);
      return this;
    }

    public EntityTypeConfiguration<TEntityType> MediaType()
    {
      this._configuration.HasStream = true;
      return this;
    }

    public EntityTypeConfiguration<TEntityType> DerivesFromNothing()
    {
      this._configuration.DerivesFromNothing();
      return this;
    }

    public EntityTypeConfiguration<TEntityType> DerivesFrom<TBaseType>() where TBaseType : class
    {
      this._configuration.DerivesFrom(this._modelBuilder.EntityType<TBaseType>()._configuration);
      return this;
    }

    public EntityTypeConfiguration<TEntityType> HasKey<TKey>(
      Expression<Func<TEntityType, TKey>> keyDefinitionExpression)
    {
      foreach (PropertyInfo selectedProperty in (IEnumerable<PropertyInfo>) PropertySelectorVisitor.GetSelectedProperties((Expression) keyDefinitionExpression))
        this._configuration.HasKey(selectedProperty);
      return this;
    }

    public ActionConfiguration Action(string name)
    {
      ActionConfiguration actionConfiguration = this._configuration.ModelBuilder.Action(name);
      actionConfiguration.SetBindingParameter("bindingParameter", (IEdmTypeConfiguration) this._configuration);
      return actionConfiguration;
    }

    public FunctionConfiguration Function(string name)
    {
      FunctionConfiguration functionConfiguration = this._configuration.ModelBuilder.Function(name);
      functionConfiguration.SetBindingParameter("bindingParameter", (IEdmTypeConfiguration) this._configuration);
      return functionConfiguration;
    }
  }
}
