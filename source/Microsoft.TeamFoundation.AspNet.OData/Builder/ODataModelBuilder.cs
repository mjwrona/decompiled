// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ODataModelBuilder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  public class ODataModelBuilder
  {
    private const string DefaultNamespace = "Default";
    private static readonly Version _defaultDataServiceVersion = EdmConstants.EdmVersion4;
    private static readonly Version _defaultMaxDataServiceVersion = EdmConstants.EdmVersion4;
    private Dictionary<Type, EnumTypeConfiguration> _enumTypes = new Dictionary<Type, EnumTypeConfiguration>();
    private Dictionary<Type, StructuralTypeConfiguration> _structuralTypes = new Dictionary<Type, StructuralTypeConfiguration>();
    private Dictionary<string, NavigationSourceConfiguration> _navigationSources = new Dictionary<string, NavigationSourceConfiguration>();
    private Dictionary<Type, PrimitiveTypeConfiguration> _primitiveTypes = new Dictionary<Type, PrimitiveTypeConfiguration>();
    private List<OperationConfiguration> _operations = new List<OperationConfiguration>();
    private Version _dataServiceVersion;
    private Version _maxDataServiceVersion;
    private string _namespace;

    public ODataModelBuilder()
    {
      this._namespace = "Default";
      this.ContainerName = "Container";
      this.DataServiceVersion = ODataModelBuilder._defaultDataServiceVersion;
      this.MaxDataServiceVersion = ODataModelBuilder._defaultMaxDataServiceVersion;
      this.BindingOptions = NavigationPropertyBindingOption.None;
      this.HasAssignedNamespace = false;
    }

    public string Namespace
    {
      get => this._namespace;
      set
      {
        if (string.IsNullOrEmpty(value))
        {
          this.HasAssignedNamespace = false;
          this._namespace = "Default";
        }
        else
        {
          this.HasAssignedNamespace = true;
          this._namespace = value;
        }
      }
    }

    public string ContainerName { get; set; }

    public Version DataServiceVersion
    {
      get => this._dataServiceVersion;
      set => this._dataServiceVersion = !(value == (Version) null) ? value : throw Microsoft.AspNet.OData.Common.Error.PropertyNull();
    }

    public Version MaxDataServiceVersion
    {
      get => this._maxDataServiceVersion;
      set => this._maxDataServiceVersion = !(value == (Version) null) ? value : throw Microsoft.AspNet.OData.Common.Error.PropertyNull();
    }

    public virtual IEnumerable<EntitySetConfiguration> EntitySets => this._navigationSources.Values.OfType<EntitySetConfiguration>();

    public virtual IEnumerable<StructuralTypeConfiguration> StructuralTypes => (IEnumerable<StructuralTypeConfiguration>) this._structuralTypes.Values;

    public virtual IEnumerable<EnumTypeConfiguration> EnumTypes => (IEnumerable<EnumTypeConfiguration>) this._enumTypes.Values;

    public virtual IEnumerable<SingletonConfiguration> Singletons => this._navigationSources.Values.OfType<SingletonConfiguration>();

    public virtual IEnumerable<NavigationSourceConfiguration> NavigationSources => (IEnumerable<NavigationSourceConfiguration>) this._navigationSources.Values;

    public virtual IEnumerable<OperationConfiguration> Operations => (IEnumerable<OperationConfiguration>) this._operations;

    public NavigationPropertyBindingOption BindingOptions { get; set; }

    internal bool HasAssignedNamespace { get; private set; }

    public EntityTypeConfiguration<TEntityType> EntityType<TEntityType>() where TEntityType : class => new EntityTypeConfiguration<TEntityType>(this, this.AddEntityType(typeof (TEntityType)));

    public ComplexTypeConfiguration<TComplexType> ComplexType<TComplexType>() where TComplexType : class => new ComplexTypeConfiguration<TComplexType>(this, this.AddComplexType(typeof (TComplexType)));

    public EntitySetConfiguration<TEntityType> EntitySet<TEntityType>(string name) where TEntityType : class
    {
      EntityTypeConfiguration entityType = this.AddEntityType(typeof (TEntityType));
      return new EntitySetConfiguration<TEntityType>(this, this.AddEntitySet(name, entityType));
    }

    public EnumTypeConfiguration<TEnumType> EnumType<TEnumType>() => new EnumTypeConfiguration<TEnumType>(this.AddEnumType(typeof (TEnumType)));

    public SingletonConfiguration<TEntityType> Singleton<TEntityType>(string name) where TEntityType : class
    {
      EntityTypeConfiguration entityType = this.AddEntityType(typeof (TEntityType));
      return new SingletonConfiguration<TEntityType>(this, this.AddSingleton(name, entityType));
    }

    public virtual ActionConfiguration Action(string name)
    {
      ActionConfiguration actionConfiguration = new ActionConfiguration(this, name);
      this._operations.Add((OperationConfiguration) actionConfiguration);
      return actionConfiguration;
    }

    public virtual FunctionConfiguration Function(string name)
    {
      FunctionConfiguration functionConfiguration = new FunctionConfiguration(this, name);
      this._operations.Add((OperationConfiguration) functionConfiguration);
      return functionConfiguration;
    }

    public virtual EntityTypeConfiguration AddEntityType(Type type)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (!this._structuralTypes.ContainsKey(type))
      {
        EntityTypeConfiguration typeConfiguration = new EntityTypeConfiguration(this, type);
        this._structuralTypes.Add(type, (StructuralTypeConfiguration) typeConfiguration);
        return typeConfiguration;
      }
      if (!(this._structuralTypes[type] is EntityTypeConfiguration structuralType) || structuralType.ClrType != type)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (type), SRResources.TypeCannotBeEntityWasComplex, (object) type.FullName);
      return structuralType;
    }

    public virtual ComplexTypeConfiguration AddComplexType(Type type)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (!this._structuralTypes.ContainsKey(type))
      {
        ComplexTypeConfiguration typeConfiguration = new ComplexTypeConfiguration(this, type);
        this._structuralTypes.Add(type, (StructuralTypeConfiguration) typeConfiguration);
        return typeConfiguration;
      }
      if (!(this._structuralTypes[type] is ComplexTypeConfiguration structuralType) || structuralType.ClrType != type)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (type), SRResources.TypeCannotBeComplexWasEntity, (object) type.FullName);
      return structuralType;
    }

    public virtual EnumTypeConfiguration AddEnumType(Type type)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (!TypeHelper.IsEnum(type))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (type), SRResources.TypeCannotBeEnum, (object) type.FullName);
      if (!this._enumTypes.ContainsKey(type))
      {
        EnumTypeConfiguration typeConfiguration = new EnumTypeConfiguration(this, type);
        this._enumTypes.Add(type, typeConfiguration);
        return typeConfiguration;
      }
      EnumTypeConfiguration enumType = this._enumTypes[type];
      if (enumType.ClrType != type)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (type), SRResources.TypeCannotBeEnum, (object) type.FullName);
      return enumType;
    }

    public virtual void AddOperation(OperationConfiguration operation) => this._operations.Add(operation);

    public virtual EntitySetConfiguration AddEntitySet(
      string name,
      EntityTypeConfiguration entityType)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (name));
      if (entityType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entityType));
      if (name.Contains("."))
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.InvalidEntitySetName, (object) name);
      if (this._navigationSources.ContainsKey(name))
      {
        if (!(this._navigationSources[name] is EntitySetConfiguration setConfiguration))
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (name), SRResources.EntitySetNameAlreadyConfiguredAsSingleton, (object) name);
        if (setConfiguration.EntityType != entityType)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (entityType), SRResources.EntitySetAlreadyConfiguredDifferentEntityType, (object) setConfiguration.Name, (object) setConfiguration.EntityType.Name);
      }
      else
      {
        setConfiguration = new EntitySetConfiguration(this, entityType, name);
        this._navigationSources[name] = (NavigationSourceConfiguration) setConfiguration;
      }
      return setConfiguration;
    }

    public virtual SingletonConfiguration AddSingleton(
      string name,
      EntityTypeConfiguration entityType)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (name));
      if (entityType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entityType));
      if (name.Contains("."))
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.InvalidSingletonName, (object) name);
      if (this._navigationSources.ContainsKey(name))
      {
        if (!(this._navigationSources[name] is SingletonConfiguration singletonConfiguration))
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (name), SRResources.SingletonNameAlreadyConfiguredAsEntitySet, (object) name);
        if (singletonConfiguration.EntityType != entityType)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (entityType), SRResources.SingletonAlreadyConfiguredDifferentEntityType, (object) singletonConfiguration.Name, (object) singletonConfiguration.EntityType.Name);
      }
      else
      {
        singletonConfiguration = new SingletonConfiguration(this, entityType, name);
        this._navigationSources[name] = (NavigationSourceConfiguration) singletonConfiguration;
      }
      return singletonConfiguration;
    }

    public virtual bool RemoveStructuralType(Type type) => !(type == (Type) null) ? this._structuralTypes.Remove(type) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));

    public virtual bool RemoveEnumType(Type type) => !(type == (Type) null) ? this._enumTypes.Remove(type) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));

    public virtual bool RemoveEntitySet(string name)
    {
      if (name == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (name));
      return this._navigationSources.ContainsKey(name) && this._navigationSources[name] is EntitySetConfiguration && this._navigationSources.Remove(name);
    }

    public virtual bool RemoveSingleton(string name)
    {
      if (name == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (name));
      return this._navigationSources.ContainsKey(name) && this._navigationSources[name] is SingletonConfiguration && this._navigationSources.Remove(name);
    }

    public virtual bool RemoveOperation(string name)
    {
      if (name == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (name));
      OperationConfiguration[] array = this._operations.Where<OperationConfiguration>((Func<OperationConfiguration, bool>) (p => p.Name == name)).ToArray<OperationConfiguration>();
      switch (((IEnumerable<OperationConfiguration>) array).Count<OperationConfiguration>())
      {
        case 0:
          return false;
        case 1:
          return this.RemoveOperation(array[0]);
        default:
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.MoreThanOneOperationFound, (object) name);
      }
    }

    public virtual bool RemoveOperation(OperationConfiguration operation) => operation != null ? this._operations.Remove(operation) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (operation));

    public IEdmTypeConfiguration GetTypeConfigurationOrNull(Type type)
    {
      if (this._primitiveTypes.ContainsKey(type))
        return (IEdmTypeConfiguration) this._primitiveTypes[type];
      IEdmPrimitiveType primitiveTypeOrNull = EdmLibHelpers.GetEdmPrimitiveTypeOrNull(type);
      if (primitiveTypeOrNull != null)
      {
        PrimitiveTypeConfiguration configurationOrNull = new PrimitiveTypeConfiguration(this, primitiveTypeOrNull, type);
        this._primitiveTypes[type] = configurationOrNull;
        return (IEdmTypeConfiguration) configurationOrNull;
      }
      if (this._structuralTypes.ContainsKey(type))
        return (IEdmTypeConfiguration) this._structuralTypes[type];
      return this._enumTypes.ContainsKey(type) ? (IEdmTypeConfiguration) this._enumTypes[type] : (IEdmTypeConfiguration) null;
    }

    public virtual IEdmModel GetEdmModel()
    {
      IEdmModel model = EdmModelHelperMethods.BuildEdmModel(this);
      this.ValidateModel(model);
      return model;
    }

    public virtual void ValidateModel(IEdmModel model)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      foreach (IEdmEntitySet navigationSource in model.EntityContainer.Elements.OfType<IEdmEntitySet>())
      {
        if (!navigationSource.EntityType().Key().Any<IEdmStructuralProperty>())
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EntitySetTypeHasNoKeys, (object) navigationSource.Name, (object) navigationSource.EntityType().FullName());
      }
      foreach (IEdmStructuredType type in model.SchemaElementsAcrossModels().OfType<IEdmStructuredType>())
      {
        foreach (IEdmNavigationProperty navigationProperty in type.DeclaredNavigationProperties())
        {
          if (navigationProperty.TargetMultiplicity() == EdmMultiplicity.Many)
          {
            IEdmEntityType entityType = navigationProperty.ToEntityType();
            if (!entityType.Key().Any<IEdmStructuralProperty>())
              throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CollectionNavigationPropertyEntityTypeDoesntHaveKeyDefined, (object) entityType.FullTypeName(), (object) navigationProperty.Name, (object) type.FullTypeName());
          }
        }
      }
    }
  }
}
