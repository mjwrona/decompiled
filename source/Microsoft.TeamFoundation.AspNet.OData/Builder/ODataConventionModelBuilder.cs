// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ODataConventionModelBuilder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Builder.Conventions;
using Microsoft.AspNet.OData.Builder.Conventions.Attributes;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Microsoft.AspNet.OData.Builder
{
  public class ODataConventionModelBuilder : ODataModelBuilder
  {
    private static readonly List<IConvention> _conventions = new List<IConvention>()
    {
      (IConvention) new AbstractTypeDiscoveryConvention(),
      (IConvention) new DataContractAttributeEdmTypeConvention(),
      (IConvention) new NotMappedAttributeConvention(),
      (IConvention) new DataMemberAttributeEdmPropertyConvention(),
      (IConvention) new RequiredAttributeEdmPropertyConvention(),
      (IConvention) new DefaultValueAttributeEdmPropertyConvention(),
      (IConvention) new ConcurrencyCheckAttributeEdmPropertyConvention(),
      (IConvention) new TimestampAttributeEdmPropertyConvention(),
      (IConvention) new ColumnAttributeEdmPropertyConvention(),
      (IConvention) new KeyAttributeEdmPropertyConvention(),
      (IConvention) new EntityKeyConvention(),
      (IConvention) new ComplexTypeAttributeConvention(),
      (IConvention) new IgnoreDataMemberAttributeEdmPropertyConvention(),
      (IConvention) new NotFilterableAttributeEdmPropertyConvention(),
      (IConvention) new NonFilterableAttributeEdmPropertyConvention(),
      (IConvention) new NotSortableAttributeEdmPropertyConvention(),
      (IConvention) new UnsortableAttributeEdmPropertyConvention(),
      (IConvention) new NotNavigableAttributeEdmPropertyConvention(),
      (IConvention) new NotExpandableAttributeEdmPropertyConvention(),
      (IConvention) new NotCountableAttributeEdmPropertyConvention(),
      (IConvention) new MediaTypeAttributeConvention(),
      (IConvention) new AutoExpandAttributeEdmPropertyConvention(),
      (IConvention) new AutoExpandAttributeEdmTypeConvention(),
      (IConvention) new MaxLengthAttributeEdmPropertyConvention(),
      (IConvention) new PageAttributeEdmPropertyConvention(),
      (IConvention) new PageAttributeEdmTypeConvention(),
      (IConvention) new ExpandAttributeEdmPropertyConvention(),
      (IConvention) new ExpandAttributeEdmTypeConvention(),
      (IConvention) new CountAttributeEdmPropertyConvention(),
      (IConvention) new CountAttributeEdmTypeConvention(),
      (IConvention) new OrderByAttributeEdmTypeConvention(),
      (IConvention) new FilterAttributeEdmTypeConvention(),
      (IConvention) new OrderByAttributeEdmPropertyConvention(),
      (IConvention) new FilterAttributeEdmPropertyConvention(),
      (IConvention) new SelectAttributeEdmTypeConvention(),
      (IConvention) new SelectAttributeEdmPropertyConvention(),
      (IConvention) new SelfLinksGenerationConvention(),
      (IConvention) new NavigationLinksGenerationConvention(),
      (IConvention) new AssociationSetDiscoveryConvention(),
      (IConvention) new ActionLinkGenerationConvention(),
      (IConvention) new FunctionLinkGenerationConvention()
    };
    private HashSet<StructuralTypeConfiguration> _mappedTypes;
    private HashSet<NavigationSourceConfiguration> _configuredNavigationSources;
    private HashSet<Type> _ignoredTypes;
    private IEnumerable<StructuralTypeConfiguration> _explicitlyAddedTypes;
    private bool _isModelBeingBuilt;
    private bool _isQueryCompositionMode;
    private Lazy<IDictionary<Type, List<Type>>> _allTypesWithDerivedTypeMapping;

    public ODataConventionModelBuilder()
      : this(WebApiAssembliesResolver.Default)
    {
    }

    public ODataConventionModelBuilder(HttpConfiguration configuration)
      : this(configuration, false)
    {
    }

    public ODataConventionModelBuilder(HttpConfiguration configuration, bool isQueryCompositionMode)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      this.Initialize((IWebApiAssembliesResolver) new WebApiAssembliesResolver(configuration.Services.GetAssembliesResolver()), isQueryCompositionMode);
    }

    internal ODataConventionModelBuilder(IWebApiAssembliesResolver resolver)
      : this(resolver, false)
    {
    }

    internal ODataConventionModelBuilder(
      IWebApiAssembliesResolver resolver,
      bool isQueryCompositionMode)
    {
      if (resolver == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resolver));
      this.Initialize(resolver, isQueryCompositionMode);
    }

    public bool ModelAliasingEnabled { get; set; }

    public System.Action<ODataConventionModelBuilder> OnModelCreating { get; set; }

    internal void Initialize(
      IWebApiAssembliesResolver assembliesResolver,
      bool isQueryCompositionMode)
    {
      this._isQueryCompositionMode = isQueryCompositionMode;
      this._configuredNavigationSources = new HashSet<NavigationSourceConfiguration>();
      this._mappedTypes = new HashSet<StructuralTypeConfiguration>();
      this._ignoredTypes = new HashSet<Type>();
      this.ModelAliasingEnabled = true;
      this._allTypesWithDerivedTypeMapping = new Lazy<IDictionary<Type, List<Type>>>((Func<IDictionary<Type, List<Type>>>) (() => (IDictionary<Type, List<Type>>) ODataConventionModelBuilder.BuildDerivedTypesMapping(assembliesResolver)), false);
    }

    public ODataConventionModelBuilder Ignore<T>()
    {
      this._ignoredTypes.Add(typeof (T));
      return this;
    }

    public ODataConventionModelBuilder Ignore(params Type[] types)
    {
      foreach (Type type in types)
        this._ignoredTypes.Add(type);
      return this;
    }

    public override EntityTypeConfiguration AddEntityType(Type type)
    {
      EntityTypeConfiguration edmType = base.AddEntityType(type);
      if (this._isModelBeingBuilt)
        this.MapType((StructuralTypeConfiguration) edmType);
      return edmType;
    }

    public override ComplexTypeConfiguration AddComplexType(Type type)
    {
      ComplexTypeConfiguration edmType = base.AddComplexType(type);
      if (this._isModelBeingBuilt)
        this.MapType((StructuralTypeConfiguration) edmType);
      return edmType;
    }

    public override EntitySetConfiguration AddEntitySet(
      string name,
      EntityTypeConfiguration entityType)
    {
      EntitySetConfiguration setConfiguration = base.AddEntitySet(name, entityType);
      if (this._isModelBeingBuilt)
        this.ApplyNavigationSourceConventions((NavigationSourceConfiguration) setConfiguration);
      return setConfiguration;
    }

    public override SingletonConfiguration AddSingleton(
      string name,
      EntityTypeConfiguration entityType)
    {
      SingletonConfiguration singletonConfiguration = base.AddSingleton(name, entityType);
      if (this._isModelBeingBuilt)
        this.ApplyNavigationSourceConventions((NavigationSourceConfiguration) singletonConfiguration);
      return singletonConfiguration;
    }

    public override EnumTypeConfiguration AddEnumType(Type type)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (!TypeHelper.IsEnum(type))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (type), SRResources.TypeCannotBeEnum, (object) type.FullName);
      EnumTypeConfiguration enumTypeConfiguration = this.EnumTypes.SingleOrDefault<EnumTypeConfiguration>((Func<EnumTypeConfiguration, bool>) (e => e.ClrType == type));
      if (enumTypeConfiguration == null)
      {
        enumTypeConfiguration = base.AddEnumType(type);
        foreach (object obj in Enum.GetValues(type))
        {
          object member = obj;
          bool flag = enumTypeConfiguration.Members.Any<EnumMemberConfiguration>((Func<EnumMemberConfiguration, bool>) (m => m.Name.Equals(member.ToString())));
          enumTypeConfiguration.AddMember((Enum) member).AddedExplicitly = flag;
        }
        this.ApplyEnumTypeConventions(enumTypeConfiguration);
      }
      return enumTypeConfiguration;
    }

    public override IEdmModel GetEdmModel()
    {
      if (this._isModelBeingBuilt)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.GetEdmModelCalledMoreThanOnce);
      this._explicitlyAddedTypes = (IEnumerable<StructuralTypeConfiguration>) new List<StructuralTypeConfiguration>(this.StructuralTypes);
      this._isModelBeingBuilt = true;
      this.MapTypes();
      this.DiscoverInheritanceRelationships();
      if (!this._isQueryCompositionMode)
        this.RediscoverComplexTypes();
      this.PruneUnreachableTypes();
      foreach (NavigationSourceConfiguration navigationSourceConfiguration in (IEnumerable<NavigationSourceConfiguration>) new List<NavigationSourceConfiguration>(this.NavigationSources))
        this.ApplyNavigationSourceConventions(navigationSourceConfiguration);
      foreach (OperationConfiguration operation in this.Operations)
        this.ApplyOperationConventions(operation);
      if (this.OnModelCreating != null)
        this.OnModelCreating(this);
      return base.GetEdmModel();
    }

    internal bool IsIgnoredType(Type type) => this._ignoredTypes.Contains(type);

    internal void DiscoverInheritanceRelationships()
    {
      Dictionary<Type, EntityTypeConfiguration> dictionary1 = this.StructuralTypes.OfType<EntityTypeConfiguration>().ToDictionary<EntityTypeConfiguration, Type>((Func<EntityTypeConfiguration, Type>) (e => e.ClrType));
      foreach (EntityTypeConfiguration derivedStructrualType in this.StructuralTypes.OfType<EntityTypeConfiguration>().Where<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => !e.BaseTypeConfigured)))
      {
        for (Type baseType = TypeHelper.GetBaseType(derivedStructrualType.ClrType); baseType != (Type) null; baseType = TypeHelper.GetBaseType(baseType))
        {
          EntityTypeConfiguration typeConfiguration;
          if (dictionary1.TryGetValue(baseType, out typeConfiguration))
          {
            this.RemoveBaseTypeProperties((StructuralTypeConfiguration) derivedStructrualType, (StructuralTypeConfiguration) typeConfiguration);
            if (this._isQueryCompositionMode)
            {
              foreach (PrimitivePropertyConfiguration keyProperty in derivedStructrualType.Keys.ToArray<PrimitivePropertyConfiguration>())
                derivedStructrualType.RemoveKey(keyProperty);
              foreach (EnumPropertyConfiguration enumKeyProperty in derivedStructrualType.EnumKeys.ToArray<EnumPropertyConfiguration>())
                derivedStructrualType.RemoveKey(enumKeyProperty);
            }
            derivedStructrualType.DerivesFrom(typeConfiguration);
            break;
          }
        }
      }
      Dictionary<Type, ComplexTypeConfiguration> dictionary2 = this.StructuralTypes.OfType<ComplexTypeConfiguration>().ToDictionary<ComplexTypeConfiguration, Type>((Func<ComplexTypeConfiguration, Type>) (e => e.ClrType));
      foreach (ComplexTypeConfiguration derivedStructrualType in this.StructuralTypes.OfType<ComplexTypeConfiguration>().Where<ComplexTypeConfiguration>((Func<ComplexTypeConfiguration, bool>) (e => !e.BaseTypeConfigured)))
      {
        for (Type baseType = TypeHelper.GetBaseType(derivedStructrualType.ClrType); baseType != (Type) null; baseType = TypeHelper.GetBaseType(baseType))
        {
          ComplexTypeConfiguration typeConfiguration;
          if (dictionary2.TryGetValue(baseType, out typeConfiguration))
          {
            this.RemoveBaseTypeProperties((StructuralTypeConfiguration) derivedStructrualType, (StructuralTypeConfiguration) typeConfiguration);
            derivedStructrualType.DerivesFrom(typeConfiguration);
            break;
          }
        }
      }
    }

    internal void RemoveBaseTypeProperties(
      StructuralTypeConfiguration derivedStructrualType,
      StructuralTypeConfiguration baseStructuralType)
    {
      IEnumerable<StructuralTypeConfiguration> typeConfigurations = ((IEnumerable<StructuralTypeConfiguration>) new StructuralTypeConfiguration[1]
      {
        derivedStructrualType
      }).Concat<StructuralTypeConfiguration>(this.DerivedTypes(derivedStructrualType));
      foreach (PropertyConfiguration propertyConfiguration1 in baseStructuralType.Properties.Concat<PropertyConfiguration>(baseStructuralType.DerivedProperties()))
      {
        PropertyConfiguration property = propertyConfiguration1;
        foreach (StructuralTypeConfiguration typeConfiguration in typeConfigurations)
        {
          PropertyConfiguration propertyConfiguration2 = typeConfiguration.Properties.SingleOrDefault<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (p => p.PropertyInfo.Name == property.PropertyInfo.Name));
          if (propertyConfiguration2 != null)
            typeConfiguration.RemoveProperty(propertyConfiguration2.PropertyInfo);
        }
      }
      foreach (PropertyInfo ignoredProperty1 in baseStructuralType.IgnoredProperties())
      {
        PropertyInfo ignoredProperty = ignoredProperty1;
        foreach (StructuralTypeConfiguration typeConfiguration in typeConfigurations)
        {
          PropertyConfiguration propertyConfiguration = typeConfiguration.Properties.SingleOrDefault<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (p => p.PropertyInfo.Name == ignoredProperty.Name));
          if (propertyConfiguration != null)
            typeConfiguration.RemoveProperty(propertyConfiguration.PropertyInfo);
        }
      }
    }

    private void RediscoverComplexTypes()
    {
      this.ReconfigureEntityTypesAsComplexType(this.StructuralTypes.Except<StructuralTypeConfiguration>(this._explicitlyAddedTypes).OfType<EntityTypeConfiguration>().Where<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (entity => !entity.Keys().Any<PropertyConfiguration>())).ToArray<EntityTypeConfiguration>());
      this.DiscoverInheritanceRelationships();
    }

    private void ReconfigureEntityTypesAsComplexType(
      EntityTypeConfiguration[] misconfiguredEntityTypes)
    {
      IList<EntityTypeConfiguration> list1 = (IList<EntityTypeConfiguration>) this.StructuralTypes.OfType<EntityTypeConfiguration>().Where<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (entity => entity.Keys().Any<PropertyConfiguration>())).Concat<EntityTypeConfiguration>(this._explicitlyAddedTypes.OfType<EntityTypeConfiguration>()).Except<EntityTypeConfiguration>((IEnumerable<EntityTypeConfiguration>) misconfiguredEntityTypes).ToList<EntityTypeConfiguration>();
      HashSet<EntityTypeConfiguration> typeConfigurationSet = new HashSet<EntityTypeConfiguration>();
      foreach (EntityTypeConfiguration misconfiguredEntityType in misconfiguredEntityTypes)
      {
        if (!typeConfigurationSet.Contains(misconfiguredEntityType))
        {
          IEnumerable<EntityTypeConfiguration> basedTypes = misconfiguredEntityType.BaseTypes().OfType<EntityTypeConfiguration>();
          if (list1.Any<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => basedTypes.Any<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (a => a.ClrType == e.ClrType)))))
          {
            typeConfigurationSet.Add(misconfiguredEntityType);
          }
          else
          {
            IList<EntityTypeConfiguration> list2 = (IList<EntityTypeConfiguration>) EdmTypeConfigurationExtensions.DerivedTypes(this, misconfiguredEntityType).Concat<EntityTypeConfiguration>((IEnumerable<EntityTypeConfiguration>) new EntityTypeConfiguration[1]
            {
              misconfiguredEntityType
            }).OfType<EntityTypeConfiguration>().ToList<EntityTypeConfiguration>();
            foreach (EntityTypeConfiguration typeConfiguration in (IEnumerable<EntityTypeConfiguration>) list2)
            {
              EntityTypeConfiguration subEnityType = typeConfiguration;
              if (list1.Any<EntityTypeConfiguration>((Func<EntityTypeConfiguration, bool>) (e => e.ClrType == subEnityType.ClrType)))
                throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotReconfigEntityTypeAsComplexType, (object) misconfiguredEntityType.ClrType.FullName, (object) subEnityType.ClrType.FullName);
              this.RemoveStructuralType(subEnityType.ClrType);
            }
            this.AddComplexType(misconfiguredEntityType.ClrType);
            foreach (EntityTypeConfiguration typeConfiguration in (IEnumerable<EntityTypeConfiguration>) list2)
            {
              EntityTypeConfiguration subEnityType = typeConfiguration;
              typeConfigurationSet.Add(subEnityType);
              foreach (StructuralTypeConfiguration edmTypeConfiguration in (IEnumerable<StructuralTypeConfiguration>) this.StructuralTypes.ToList<StructuralTypeConfiguration>())
              {
                foreach (NavigationPropertyConfiguration propertyConfiguration in edmTypeConfiguration.NavigationProperties.Where<NavigationPropertyConfiguration>((Func<NavigationPropertyConfiguration, bool>) (navigationProperty => navigationProperty.RelatedClrType == subEnityType.ClrType)).ToArray<NavigationPropertyConfiguration>())
                {
                  string name = propertyConfiguration.Name;
                  edmTypeConfiguration.RemoveProperty(propertyConfiguration.PropertyInfo);
                  PropertyConfiguration property = propertyConfiguration.Multiplicity != EdmMultiplicity.Many ? (PropertyConfiguration) edmTypeConfiguration.AddComplexProperty(propertyConfiguration.PropertyInfo) : (PropertyConfiguration) edmTypeConfiguration.AddCollectionProperty(propertyConfiguration.PropertyInfo);
                  property.AddedExplicitly = false;
                  this.ReapplyPropertyConvention(property, edmTypeConfiguration);
                  property.Name = name;
                }
              }
            }
          }
        }
      }
    }

    private void MapTypes()
    {
      foreach (StructuralTypeConfiguration explicitlyAddedType in this._explicitlyAddedTypes)
        this.MapType(explicitlyAddedType);
      this.ApplyForeignKeyConventions();
    }

    private void ApplyForeignKeyConventions()
    {
      ForeignKeyAttributeConvention attributeConvention1 = new ForeignKeyAttributeConvention();
      ForeignKeyDiscoveryConvention discoveryConvention = new ForeignKeyDiscoveryConvention();
      ActionOnDeleteAttributeConvention attributeConvention2 = new ActionOnDeleteAttributeConvention();
      foreach (EntityTypeConfiguration typeConfiguration in this.StructuralTypes.OfType<EntityTypeConfiguration>())
      {
        foreach (PropertyConfiguration property in typeConfiguration.Properties)
        {
          attributeConvention1.Apply(property, (StructuralTypeConfiguration) typeConfiguration, this);
          discoveryConvention.Apply(property, (StructuralTypeConfiguration) typeConfiguration, this);
          attributeConvention2.Apply(property, (StructuralTypeConfiguration) typeConfiguration, this);
        }
      }
    }

    private void MapType(StructuralTypeConfiguration edmType)
    {
      if (this._mappedTypes.Contains(edmType))
        return;
      this._mappedTypes.Add(edmType);
      this.MapStructuralType(edmType);
      this.ApplyTypeAndPropertyConventions(edmType);
    }

    private void MapStructuralType(StructuralTypeConfiguration structuralType)
    {
      foreach (PropertyInfo property1 in ConventionsHelpers.GetProperties(structuralType, this._isQueryCompositionMode))
      {
        PropertyInfo property = property1;
        bool isCollection;
        PropertyKind propertyType = this.GetPropertyType(property, out isCollection, out IEdmTypeConfiguration _);
        switch (propertyType)
        {
          case PropertyKind.Primitive:
          case PropertyKind.Complex:
          case PropertyKind.Enum:
            this.MapStructuralProperty(structuralType, property, propertyType, isCollection);
            continue;
          case PropertyKind.Dynamic:
            structuralType.AddDynamicPropertyDictionary(property);
            continue;
          default:
            if (structuralType.NavigationProperties.All<NavigationPropertyConfiguration>((Func<NavigationPropertyConfiguration, bool>) (p => p.Name != property.Name)))
            {
              NavigationPropertyConfiguration propertyConfiguration = isCollection ? structuralType.AddNavigationProperty(property, EdmMultiplicity.Many) : structuralType.AddNavigationProperty(property, EdmMultiplicity.ZeroOrOne);
              if (property.GetCustomAttribute<ContainedAttribute>() != null)
                propertyConfiguration.Contained();
              propertyConfiguration.AddedExplicitly = false;
              continue;
            }
            continue;
        }
      }
      this.MapDerivedTypes(structuralType);
    }

    internal void MapDerivedTypes(StructuralTypeConfiguration structuralType)
    {
      HashSet<Type> typeSet = new HashSet<Type>();
      Queue<StructuralTypeConfiguration> typeConfigurationQueue = new Queue<StructuralTypeConfiguration>();
      typeConfigurationQueue.Enqueue(structuralType);
      while (typeConfigurationQueue.Count != 0)
      {
        StructuralTypeConfiguration typeConfiguration1 = typeConfigurationQueue.Dequeue();
        typeSet.Add(typeConfiguration1.ClrType);
        List<Type> typeList;
        if (this._allTypesWithDerivedTypeMapping.Value.TryGetValue(typeConfiguration1.ClrType, out typeList))
        {
          foreach (Type type in typeList)
          {
            if (!typeSet.Contains(type) && !this.IsIgnoredType(type))
            {
              StructuralTypeConfiguration typeConfiguration2 = typeConfiguration1.Kind != EdmTypeKind.Entity ? (StructuralTypeConfiguration) this.AddComplexType(type) : (StructuralTypeConfiguration) this.AddEntityType(type);
              typeConfigurationQueue.Enqueue(typeConfiguration2);
            }
          }
        }
      }
    }

    private void MapStructuralProperty(
      StructuralTypeConfiguration type,
      PropertyInfo property,
      PropertyKind propertyKind,
      bool isCollection)
    {
      bool flag = type.HasProperty(property.Name);
      PropertyConfiguration propertyConfiguration;
      if (!isCollection)
      {
        switch (propertyKind)
        {
          case PropertyKind.Primitive:
            propertyConfiguration = (PropertyConfiguration) type.AddProperty(property);
            break;
          case PropertyKind.Enum:
            this.AddEnumType(TypeHelper.GetUnderlyingTypeOrSelf(property.PropertyType));
            propertyConfiguration = (PropertyConfiguration) type.AddEnumProperty(property);
            break;
          default:
            propertyConfiguration = (PropertyConfiguration) type.AddComplexProperty(property);
            break;
        }
      }
      else
      {
        int num = this._isQueryCompositionMode ? 1 : 0;
        if (property.PropertyType.IsGenericType())
        {
          Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(((IEnumerable<Type>) property.PropertyType.GetGenericArguments()).First<Type>());
          if (TypeHelper.IsEnum(underlyingTypeOrSelf))
            this.AddEnumType(underlyingTypeOrSelf);
        }
        else
        {
          Type elementType;
          if (TypeHelper.IsCollection(property.PropertyType, out elementType))
          {
            Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(elementType);
            if (TypeHelper.IsEnum(underlyingTypeOrSelf))
              this.AddEnumType(underlyingTypeOrSelf);
          }
        }
        propertyConfiguration = (PropertyConfiguration) type.AddCollectionProperty(property);
      }
      propertyConfiguration.AddedExplicitly = flag;
    }

    private PropertyKind GetPropertyType(
      PropertyInfo property,
      out bool isCollection,
      out IEdmTypeConfiguration mappedType)
    {
      if (typeof (IDictionary<string, object>).IsAssignableFrom(property.PropertyType))
      {
        mappedType = (IEdmTypeConfiguration) null;
        isCollection = false;
        return PropertyKind.Dynamic;
      }
      PropertyKind propertyKind;
      if (this.TryGetPropertyTypeKind(property.PropertyType, out mappedType, out propertyKind))
      {
        isCollection = false;
        return propertyKind;
      }
      Type elementType;
      if (TypeHelper.IsCollection(property.PropertyType, out elementType))
      {
        isCollection = true;
        return this.TryGetPropertyTypeKind(elementType, out mappedType, out propertyKind) ? propertyKind : PropertyKind.Navigation;
      }
      isCollection = false;
      return PropertyKind.Navigation;
    }

    private bool TryGetPropertyTypeKind(
      Type propertyType,
      out IEdmTypeConfiguration mappedType,
      out PropertyKind propertyKind)
    {
      if (EdmLibHelpers.GetEdmPrimitiveTypeOrNull(propertyType) != null)
      {
        mappedType = (IEdmTypeConfiguration) null;
        propertyKind = PropertyKind.Primitive;
        return true;
      }
      mappedType = this.GetStructuralTypeOrNull(propertyType);
      if (mappedType != null)
      {
        propertyKind = !(mappedType is ComplexTypeConfiguration) ? (!(mappedType is EnumTypeConfiguration) ? PropertyKind.Navigation : PropertyKind.Enum) : PropertyKind.Complex;
        return true;
      }
      for (Type baseType = TypeHelper.GetBaseType(propertyType); baseType != (Type) null && baseType != typeof (object); baseType = TypeHelper.GetBaseType(baseType))
      {
        IEdmTypeConfiguration structuralTypeOrNull = this.GetStructuralTypeOrNull(baseType);
        if (structuralTypeOrNull != null && structuralTypeOrNull is ComplexTypeConfiguration)
        {
          propertyKind = PropertyKind.Complex;
          return true;
        }
      }
      PropertyKind propertyKind1 = PropertyKind.Navigation;
      if (this.InferEdmTypeFromDerivedTypes(propertyType, ref propertyKind1))
      {
        if (propertyKind1 == PropertyKind.Complex)
          this.ReconfigInferedEntityTypeAsComplexType(propertyType);
        propertyKind = propertyKind1;
        return true;
      }
      if (TypeHelper.IsEnum(propertyType))
      {
        propertyKind = PropertyKind.Enum;
        return true;
      }
      propertyKind = PropertyKind.Navigation;
      return false;
    }

    internal void ReconfigInferedEntityTypeAsComplexType(Type propertyType)
    {
      HashSet<Type> typeSet = new HashSet<Type>();
      Queue<Type> typeQueue = new Queue<Type>();
      typeQueue.Enqueue(propertyType);
      IList<EntityTypeConfiguration> source = (IList<EntityTypeConfiguration>) new List<EntityTypeConfiguration>();
      while (typeQueue.Count != 0)
      {
        Type key = typeQueue.Dequeue();
        typeSet.Add(key);
        List<Type> typeList;
        if (this._allTypesWithDerivedTypeMapping.Value.TryGetValue(key, out typeList))
        {
          foreach (Type type in typeList)
          {
            Type derivedType = type;
            if (!typeSet.Contains(derivedType))
            {
              StructuralTypeConfiguration typeConfiguration = this.StructuralTypes.Except<StructuralTypeConfiguration>(this._explicitlyAddedTypes).FirstOrDefault<StructuralTypeConfiguration>((Func<StructuralTypeConfiguration, bool>) (c => c.ClrType == derivedType));
              if (typeConfiguration != null && typeConfiguration.Kind == EdmTypeKind.Entity)
                source.Add((EntityTypeConfiguration) typeConfiguration);
              typeQueue.Enqueue(derivedType);
            }
          }
        }
      }
      if (!source.Any<EntityTypeConfiguration>())
        return;
      this.ReconfigureEntityTypesAsComplexType(source.ToArray<EntityTypeConfiguration>());
    }

    internal bool InferEdmTypeFromDerivedTypes(Type propertyType, ref PropertyKind propertyKind)
    {
      HashSet<Type> typeSet = new HashSet<Type>();
      Queue<Type> typeQueue = new Queue<Type>();
      typeQueue.Enqueue(propertyType);
      IList<StructuralTypeConfiguration> source = (IList<StructuralTypeConfiguration>) new List<StructuralTypeConfiguration>();
      while (typeQueue.Count != 0)
      {
        Type key = typeQueue.Dequeue();
        typeSet.Add(key);
        List<Type> typeList;
        if (this._allTypesWithDerivedTypeMapping.Value.TryGetValue(key, out typeList))
        {
          foreach (Type type in typeList)
          {
            Type derivedType = type;
            if (!typeSet.Contains(derivedType))
            {
              StructuralTypeConfiguration typeConfiguration = this._explicitlyAddedTypes.FirstOrDefault<StructuralTypeConfiguration>((Func<StructuralTypeConfiguration, bool>) (c => c.ClrType == derivedType));
              if (typeConfiguration != null)
                source.Add(typeConfiguration);
              typeQueue.Enqueue(derivedType);
            }
          }
        }
      }
      if (!source.Any<StructuralTypeConfiguration>())
        return false;
      IEnumerable<EntityTypeConfiguration> list1 = (IEnumerable<EntityTypeConfiguration>) source.OfType<EntityTypeConfiguration>().ToList<EntityTypeConfiguration>();
      IEnumerable<ComplexTypeConfiguration> list2 = (IEnumerable<ComplexTypeConfiguration>) source.OfType<ComplexTypeConfiguration>().ToList<ComplexTypeConfiguration>();
      if (!list1.Any<EntityTypeConfiguration>())
      {
        propertyKind = PropertyKind.Complex;
        return true;
      }
      if (!list2.Any<ComplexTypeConfiguration>())
      {
        propertyKind = PropertyKind.Navigation;
        return true;
      }
      throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.CannotInferEdmType, (object) propertyType.FullName, (object) string.Join(",", list1.Select<EntityTypeConfiguration, string>((Func<EntityTypeConfiguration, string>) (e => e.ClrType.FullName))), (object) string.Join(",", list2.Select<ComplexTypeConfiguration, string>((Func<ComplexTypeConfiguration, string>) (e => e.ClrType.FullName))));
    }

    private void PruneUnreachableTypes()
    {
      Queue<StructuralTypeConfiguration> typeConfigurationQueue = new Queue<StructuralTypeConfiguration>(this._explicitlyAddedTypes);
      HashSet<StructuralTypeConfiguration> source = new HashSet<StructuralTypeConfiguration>();
      while (typeConfigurationQueue.Count != 0)
      {
        StructuralTypeConfiguration typeConfiguration1 = typeConfigurationQueue.Dequeue();
        foreach (PropertyConfiguration propertyConfiguration in typeConfiguration1.Properties.Where<PropertyConfiguration>((Func<PropertyConfiguration, bool>) (property => property.Kind != 0)))
        {
          if (propertyConfiguration.Kind != PropertyKind.Collection || EdmLibHelpers.GetEdmPrimitiveTypeOrNull((propertyConfiguration as CollectionPropertyConfiguration).ElementType) == null)
          {
            IEdmTypeConfiguration structuralTypeOrNull = this.GetStructuralTypeOrNull(propertyConfiguration.RelatedClrType);
            if (structuralTypeOrNull is StructuralTypeConfiguration typeConfiguration2 && !((IEnumerable<IEdmTypeConfiguration>) source).Contains<IEdmTypeConfiguration>(structuralTypeOrNull))
              typeConfigurationQueue.Enqueue(typeConfiguration2);
          }
        }
        if (typeConfiguration1.Kind == EdmTypeKind.Entity)
        {
          EntityTypeConfiguration entity = (EntityTypeConfiguration) typeConfiguration1;
          if (entity.BaseType != null && !source.Contains((StructuralTypeConfiguration) entity.BaseType))
            typeConfigurationQueue.Enqueue((StructuralTypeConfiguration) entity.BaseType);
          foreach (EntityTypeConfiguration derivedType in EdmTypeConfigurationExtensions.DerivedTypes(this, entity))
          {
            if (!source.Contains((StructuralTypeConfiguration) derivedType))
              typeConfigurationQueue.Enqueue((StructuralTypeConfiguration) derivedType);
          }
        }
        else if (typeConfiguration1.Kind == EdmTypeKind.Complex)
        {
          ComplexTypeConfiguration complex = (ComplexTypeConfiguration) typeConfiguration1;
          if (complex.BaseType != null && !source.Contains((StructuralTypeConfiguration) complex.BaseType))
            typeConfigurationQueue.Enqueue((StructuralTypeConfiguration) complex.BaseType);
          foreach (ComplexTypeConfiguration derivedType in EdmTypeConfigurationExtensions.DerivedTypes(this, complex))
          {
            if (!source.Contains((StructuralTypeConfiguration) derivedType))
              typeConfigurationQueue.Enqueue((StructuralTypeConfiguration) derivedType);
          }
        }
        source.Add(typeConfiguration1);
      }
      foreach (StructuralTypeConfiguration typeConfiguration in this.StructuralTypes.ToArray<StructuralTypeConfiguration>())
      {
        if (!source.Contains(typeConfiguration))
          this.RemoveStructuralType(typeConfiguration.ClrType);
      }
    }

    private void ApplyTypeAndPropertyConventions(StructuralTypeConfiguration edmTypeConfiguration)
    {
      foreach (IConvention convention in ODataConventionModelBuilder._conventions)
      {
        if (convention is IEdmTypeConvention edmTypeConvention)
          edmTypeConvention.Apply((IEdmTypeConfiguration) edmTypeConfiguration, this);
        if (convention is IEdmPropertyConvention propertyConvention)
          this.ApplyPropertyConvention(propertyConvention, edmTypeConfiguration);
      }
    }

    private void ApplyEnumTypeConventions(EnumTypeConfiguration enumTypeConfiguration) => new DataContractAttributeEnumTypeConvention().Apply(enumTypeConfiguration, this);

    private void ApplyNavigationSourceConventions(
      NavigationSourceConfiguration navigationSourceConfiguration)
    {
      if (this._configuredNavigationSources.Contains(navigationSourceConfiguration))
        return;
      this._configuredNavigationSources.Add(navigationSourceConfiguration);
      foreach (INavigationSourceConvention sourceConvention in ODataConventionModelBuilder._conventions.OfType<INavigationSourceConvention>())
        sourceConvention?.Apply(navigationSourceConfiguration, (ODataModelBuilder) this);
    }

    private void ApplyOperationConventions(OperationConfiguration operation)
    {
      foreach (IOperationConvention operationConvention in ODataConventionModelBuilder._conventions.OfType<IOperationConvention>())
        operationConvention.Apply(operation, (ODataModelBuilder) this);
    }

    private IEdmTypeConfiguration GetStructuralTypeOrNull(Type clrType)
    {
      IEdmTypeConfiguration structuralTypeOrNull = (IEdmTypeConfiguration) this.StructuralTypes.SingleOrDefault<StructuralTypeConfiguration>((Func<StructuralTypeConfiguration, bool>) (edmType => edmType.ClrType == clrType));
      if (structuralTypeOrNull == null)
      {
        Type type = TypeHelper.GetUnderlyingTypeOrSelf(clrType);
        structuralTypeOrNull = (IEdmTypeConfiguration) this.EnumTypes.SingleOrDefault<EnumTypeConfiguration>((Func<EnumTypeConfiguration, bool>) (edmType => edmType.ClrType == type));
      }
      return structuralTypeOrNull;
    }

    private void ApplyPropertyConvention(
      IEdmPropertyConvention propertyConvention,
      StructuralTypeConfiguration edmTypeConfiguration)
    {
      foreach (PropertyConfiguration edmProperty in edmTypeConfiguration.Properties.ToArray<PropertyConfiguration>())
        propertyConvention.Apply(edmProperty, edmTypeConfiguration, this);
    }

    private void ReapplyPropertyConvention(
      PropertyConfiguration property,
      StructuralTypeConfiguration edmTypeConfiguration)
    {
      foreach (IEdmPropertyConvention propertyConvention in ODataConventionModelBuilder._conventions.OfType<IEdmPropertyConvention>())
        propertyConvention.Apply(property, edmTypeConfiguration, this);
    }

    private static Dictionary<Type, List<Type>> BuildDerivedTypesMapping(
      IWebApiAssembliesResolver assemblyResolver)
    {
      IEnumerable<Type> source = TypeHelper.GetLoadedTypes(assemblyResolver).Where<Type>((Func<Type, bool>) (t => TypeHelper.IsVisible(t) && TypeHelper.IsClass(t) && t != typeof (object)));
      Dictionary<Type, List<Type>> dictionary = source.Distinct<Type>().ToDictionary<Type, Type, List<Type>>((Func<Type, Type>) (k => k), (Func<Type, List<Type>>) (k => new List<Type>()));
      foreach (Type clrType in source)
      {
        List<Type> typeList;
        if (TypeHelper.GetBaseType(clrType) != (Type) null && dictionary.TryGetValue(TypeHelper.GetBaseType(clrType), out typeList))
          typeList.Add(clrType);
      }
      return dictionary;
    }

    public override void ValidateModel(IEdmModel model)
    {
      if (this._isQueryCompositionMode)
        return;
      base.ValidateModel(model);
    }
  }
}
