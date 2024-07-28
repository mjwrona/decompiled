// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EdmTypeBuilder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  internal class EdmTypeBuilder
  {
    private readonly List<IEdmTypeConfiguration> _configurations;
    private readonly Dictionary<Type, IEdmType> _types = new Dictionary<Type, IEdmType>();
    private readonly Dictionary<PropertyInfo, IEdmProperty> _properties = new Dictionary<PropertyInfo, IEdmProperty>();
    private readonly Dictionary<IEdmProperty, QueryableRestrictions> _propertiesRestrictions = new Dictionary<IEdmProperty, QueryableRestrictions>();
    private readonly Dictionary<IEdmProperty, ModelBoundQuerySettings> _propertiesQuerySettings = new Dictionary<IEdmProperty, ModelBoundQuerySettings>();
    private readonly Dictionary<IEdmStructuredType, ModelBoundQuerySettings> _structuredTypeQuerySettings = new Dictionary<IEdmStructuredType, ModelBoundQuerySettings>();
    private readonly Dictionary<Enum, IEdmEnumMember> _members = new Dictionary<Enum, IEdmEnumMember>();
    private readonly Dictionary<IEdmStructuredType, PropertyInfo> _openTypes = new Dictionary<IEdmStructuredType, PropertyInfo>();

    internal EdmTypeBuilder(IEnumerable<IEdmTypeConfiguration> configurations) => this._configurations = configurations.ToList<IEdmTypeConfiguration>();

    private Dictionary<Type, IEdmType> GetEdmTypes()
    {
      this._types.Clear();
      this._properties.Clear();
      this._members.Clear();
      this._openTypes.Clear();
      foreach (IEdmTypeConfiguration configuration in this._configurations)
        this.CreateEdmTypeHeader(configuration);
      foreach (IEdmTypeConfiguration configuration in this._configurations)
        this.CreateEdmTypeBody(configuration);
      foreach (StructuralTypeConfiguration config in this._configurations.OfType<StructuralTypeConfiguration>())
        this.CreateNavigationProperty(config);
      return this._types;
    }

    private void CreateEdmTypeHeader(IEdmTypeConfiguration config)
    {
      IEdmType edmType = this.GetEdmType(config.ClrType);
      if (edmType == null)
      {
        if (config.Kind == EdmTypeKind.Complex)
        {
          ComplexTypeConfiguration typeConfiguration = (ComplexTypeConfiguration) config;
          IEdmComplexType baseType = (IEdmComplexType) null;
          if (typeConfiguration.BaseType != null)
          {
            this.CreateEdmTypeHeader((IEdmTypeConfiguration) typeConfiguration.BaseType);
            baseType = this.GetEdmType(typeConfiguration.BaseType.ClrType) as IEdmComplexType;
          }
          EdmComplexType key = new EdmComplexType(config.Namespace, config.Name, baseType, typeConfiguration.IsAbstract.GetValueOrDefault(), typeConfiguration.IsOpen);
          this._types.Add(config.ClrType, (IEdmType) key);
          if (typeConfiguration.IsOpen)
            this._openTypes.Add((IEdmStructuredType) key, typeConfiguration.DynamicPropertyDictionary);
          edmType = (IEdmType) key;
        }
        else if (config.Kind == EdmTypeKind.Entity)
        {
          EntityTypeConfiguration typeConfiguration = config as EntityTypeConfiguration;
          IEdmEntityType baseType = (IEdmEntityType) null;
          if (typeConfiguration.BaseType != null)
          {
            this.CreateEdmTypeHeader((IEdmTypeConfiguration) typeConfiguration.BaseType);
            baseType = this.GetEdmType(typeConfiguration.BaseType.ClrType) as IEdmEntityType;
          }
          EdmEntityType key = new EdmEntityType(config.Namespace, config.Name, baseType, typeConfiguration.IsAbstract.GetValueOrDefault(), typeConfiguration.IsOpen, typeConfiguration.HasStream);
          this._types.Add(config.ClrType, (IEdmType) key);
          if (typeConfiguration.IsOpen)
            this._openTypes.Add((IEdmStructuredType) key, typeConfiguration.DynamicPropertyDictionary);
          edmType = (IEdmType) key;
        }
        else
        {
          EnumTypeConfiguration typeConfiguration = config as EnumTypeConfiguration;
          this._types.Add(typeConfiguration.ClrType, (IEdmType) new EdmEnumType(typeConfiguration.Namespace, typeConfiguration.Name, EdmTypeBuilder.GetTypeKind(typeConfiguration.UnderlyingType), typeConfiguration.IsFlags));
        }
      }
      IEdmStructuredType key1 = edmType as IEdmStructuredType;
      StructuralTypeConfiguration typeConfiguration1 = config as StructuralTypeConfiguration;
      if (key1 == null || typeConfiguration1 == null || this._structuredTypeQuerySettings.ContainsKey(key1) || typeConfiguration1.QueryConfiguration.ModelBoundQuerySettings == null)
        return;
      this._structuredTypeQuerySettings.Add(key1, typeConfiguration1.QueryConfiguration.ModelBoundQuerySettings);
    }

    private void CreateEdmTypeBody(IEdmTypeConfiguration config)
    {
      IEdmType edmType = this.GetEdmType(config.ClrType);
      if (edmType.TypeKind == EdmTypeKind.Complex)
        this.CreateComplexTypeBody((EdmComplexType) edmType, (ComplexTypeConfiguration) config);
      else if (edmType.TypeKind == EdmTypeKind.Entity)
        this.CreateEntityTypeBody((EdmEntityType) edmType, (EntityTypeConfiguration) config);
      else
        this.CreateEnumTypeBody((EdmEnumType) edmType, (EnumTypeConfiguration) config);
    }

    private static IEdmTypeReference AddPrecisionConfigInPrimitiveTypeReference(
      PrecisionPropertyConfiguration precisionProperty,
      IEdmTypeReference primitiveTypeReference)
    {
      return primitiveTypeReference is EdmTemporalTypeReference && precisionProperty.Precision.HasValue ? (IEdmTypeReference) new EdmTemporalTypeReference((IEdmPrimitiveType) primitiveTypeReference.Definition, primitiveTypeReference.IsNullable, precisionProperty.Precision) : primitiveTypeReference;
    }

    private static IEdmTypeReference AddLengthConfigInPrimitiveTypeReference(
      LengthPropertyConfiguration lengthProperty,
      IEdmTypeReference primitiveTypeReference)
    {
      if (lengthProperty.MaxLength.HasValue)
      {
        switch (primitiveTypeReference)
        {
          case EdmStringTypeReference _:
            return (IEdmTypeReference) new EdmStringTypeReference((IEdmPrimitiveType) primitiveTypeReference.Definition, primitiveTypeReference.IsNullable, false, lengthProperty.MaxLength, new bool?(true));
          case EdmBinaryTypeReference _:
            return (IEdmTypeReference) new EdmBinaryTypeReference((IEdmPrimitiveType) primitiveTypeReference.Definition, primitiveTypeReference.IsNullable, false, lengthProperty.MaxLength);
        }
      }
      return primitiveTypeReference;
    }

    private void CreateStructuralTypeBody(
      EdmStructuredType type,
      StructuralTypeConfiguration config)
    {
      foreach (PropertyConfiguration property in config.Properties)
      {
        IEdmProperty key = (IEdmProperty) null;
        switch (property.Kind)
        {
          case PropertyKind.Primitive:
            PrimitivePropertyConfiguration propertyConfiguration1 = (PrimitivePropertyConfiguration) property;
            EdmPrimitiveTypeKind primitiveTypeKind = (EdmPrimitiveTypeKind) ((int) propertyConfiguration1.TargetEdmTypeKind ?? (int) EdmTypeBuilder.GetTypeKind(propertyConfiguration1.PropertyInfo.PropertyType));
            IEdmTypeReference edmTypeReference = (IEdmTypeReference) EdmCoreModel.Instance.GetPrimitive(primitiveTypeKind, propertyConfiguration1.OptionalProperty);
            if (primitiveTypeKind == EdmPrimitiveTypeKind.Decimal)
            {
              DecimalPropertyConfiguration propertyConfiguration2 = propertyConfiguration1 as DecimalPropertyConfiguration;
              int? nullable = propertyConfiguration2.Precision;
              if (!nullable.HasValue)
              {
                nullable = propertyConfiguration2.Scale;
                if (!nullable.HasValue)
                  goto label_11;
              }
              IEdmPrimitiveType definition = (IEdmPrimitiveType) edmTypeReference.Definition;
              int num = edmTypeReference.IsNullable ? 1 : 0;
              int? precision = propertyConfiguration2.Precision;
              nullable = propertyConfiguration2.Scale;
              int? scale = nullable.HasValue ? propertyConfiguration2.Scale : new int?(0);
              edmTypeReference = (IEdmTypeReference) new EdmDecimalTypeReference(definition, num != 0, precision, scale);
            }
            else if (EdmLibHelpers.HasPrecision(primitiveTypeKind))
              edmTypeReference = EdmTypeBuilder.AddPrecisionConfigInPrimitiveTypeReference(propertyConfiguration1 as PrecisionPropertyConfiguration, edmTypeReference);
            else if (EdmLibHelpers.HasLength(primitiveTypeKind))
              edmTypeReference = EdmTypeBuilder.AddLengthConfigInPrimitiveTypeReference(propertyConfiguration1 as LengthPropertyConfiguration, edmTypeReference);
label_11:
            key = (IEdmProperty) type.AddStructuralProperty(propertyConfiguration1.Name, edmTypeReference, propertyConfiguration1.DefaultValueString);
            break;
          case PropertyKind.Complex:
            ComplexPropertyConfiguration propertyConfiguration3 = property as ComplexPropertyConfiguration;
            IEdmComplexType edmType = this.GetEdmType(propertyConfiguration3.RelatedClrType) as IEdmComplexType;
            key = (IEdmProperty) type.AddStructuralProperty(propertyConfiguration3.Name, (IEdmTypeReference) new EdmComplexTypeReference(edmType, propertyConfiguration3.OptionalProperty));
            break;
          case PropertyKind.Collection:
            key = this.CreateStructuralTypeCollectionPropertyBody(type, (CollectionPropertyConfiguration) property);
            break;
          case PropertyKind.Enum:
            key = this.CreateStructuralTypeEnumPropertyBody(type, (EnumPropertyConfiguration) property);
            break;
        }
        if (key != null)
        {
          if (property.PropertyInfo != (PropertyInfo) null)
            this._properties[property.PropertyInfo] = key;
          if (property.IsRestricted)
            this._propertiesRestrictions[key] = new QueryableRestrictions(property);
          if (property.QueryConfiguration.ModelBoundQuerySettings != null)
            this._propertiesQuerySettings.Add(key, property.QueryConfiguration.ModelBoundQuerySettings);
        }
      }
    }

    private IEdmProperty CreateStructuralTypeCollectionPropertyBody(
      EdmStructuredType type,
      CollectionPropertyConfiguration collectionProperty)
    {
      Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(collectionProperty.ElementType);
      IEdmTypeReference elementType;
      if (TypeHelper.IsEnum(underlyingTypeOrSelf))
      {
        elementType = (IEdmTypeReference) new EdmEnumTypeReference((IEdmEnumType) (this.GetEdmType(underlyingTypeOrSelf) ?? throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EnumTypeDoesNotExist, (object) underlyingTypeOrSelf.Name)), collectionProperty.ElementType != underlyingTypeOrSelf);
      }
      else
      {
        IEdmType edmType = this.GetEdmType(collectionProperty.ElementType);
        elementType = edmType == null ? (IEdmTypeReference) EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(collectionProperty.ElementType) : (IEdmTypeReference) new EdmComplexTypeReference(edmType as IEdmComplexType, collectionProperty.OptionalProperty);
      }
      return (IEdmProperty) type.AddStructuralProperty(collectionProperty.Name, (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(elementType)));
    }

    private IEdmProperty CreateStructuralTypeEnumPropertyBody(
      EdmStructuredType type,
      EnumPropertyConfiguration enumProperty)
    {
      Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(enumProperty.RelatedClrType);
      IEdmTypeReference type1 = (IEdmTypeReference) new EdmEnumTypeReference((IEdmEnumType) (this.GetEdmType(underlyingTypeOrSelf) ?? throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EnumTypeDoesNotExist, (object) underlyingTypeOrSelf.Name)), enumProperty.OptionalProperty);
      return (IEdmProperty) type.AddStructuralProperty(enumProperty.Name, type1, enumProperty.DefaultValueString);
    }

    private void CreateComplexTypeBody(EdmComplexType type, ComplexTypeConfiguration config) => this.CreateStructuralTypeBody((EdmStructuredType) type, (StructuralTypeConfiguration) config);

    private void CreateEntityTypeBody(EdmEntityType type, EntityTypeConfiguration config)
    {
      this.CreateStructuralTypeBody((EdmStructuredType) type, (StructuralTypeConfiguration) config);
      IEnumerable<IEdmStructuralProperty> keyProperties = ((IEnumerable<PropertyConfiguration>) config.Keys).Concat<PropertyConfiguration>((IEnumerable<PropertyConfiguration>) config.EnumKeys).OrderBy<PropertyConfiguration, int>((Func<PropertyConfiguration, int>) (p => p.Order)).ThenBy<PropertyConfiguration, string>((Func<PropertyConfiguration, string>) (p => p.Name)).Select<PropertyConfiguration, IEdmStructuralProperty>((Func<PropertyConfiguration, IEdmStructuralProperty>) (p => type.DeclaredProperties.OfType<IEdmStructuralProperty>().First<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (dp => dp.Name == p.Name))));
      type.AddKeys(keyProperties);
    }

    private void CreateNavigationProperty(StructuralTypeConfiguration config)
    {
      EdmStructuredType edmType = (EdmStructuredType) this.GetEdmType(config.ClrType);
      foreach (NavigationPropertyConfiguration navigationProperty1 in config.NavigationProperties)
      {
        NavigationPropertyConfiguration navProp = navigationProperty1;
        Func<NavigationPropertyConfiguration, EdmNavigationPropertyInfo> func = (Func<NavigationPropertyConfiguration, EdmNavigationPropertyInfo>) (nav =>
        {
          EdmNavigationPropertyInfo navigationProperty2 = new EdmNavigationPropertyInfo()
          {
            Name = nav.Name,
            TargetMultiplicity = nav.Multiplicity,
            Target = this.GetEdmType(nav.RelatedClrType) as IEdmEntityType,
            ContainsTarget = nav.ContainsTarget,
            OnDelete = nav.OnDeleteAction
          };
          if (nav.PrincipalProperties.Any<PropertyInfo>())
            navigationProperty2.PrincipalProperties = (IEnumerable<IEdmStructuralProperty>) this.GetDeclaringPropertyInfo(nav.PrincipalProperties);
          if (nav.DependentProperties.Any<PropertyInfo>())
            navigationProperty2.DependentProperties = (IEnumerable<IEdmStructuralProperty>) this.GetDeclaringPropertyInfo(nav.DependentProperties);
          return navigationProperty2;
        });
        EdmNavigationPropertyInfo navInfo = func(navProp);
        Dictionary<IEdmProperty, NavigationPropertyConfiguration> dictionary = new Dictionary<IEdmProperty, NavigationPropertyConfiguration>();
        if (edmType is EdmEntityType edmEntityType && navProp.Partner != null)
        {
          EdmNavigationProperty key1 = edmEntityType.AddBidirectionalNavigation(navInfo, func(navProp.Partner));
          IEdmProperty key2 = (navInfo.Target as EdmEntityType).Properties().Single<IEdmProperty>((Func<IEdmProperty, bool>) (p => p.Name == navProp.Partner.Name));
          dictionary.Add((IEdmProperty) key1, navProp);
          dictionary.Add(key2, navProp.Partner);
        }
        else if (!(config.ModelBuilder.GetTypeConfigurationOrNull(navProp.RelatedClrType) as StructuralTypeConfiguration).NavigationProperties.Any<NavigationPropertyConfiguration>((Func<NavigationPropertyConfiguration, bool>) (p => p.Partner != null && p.Partner.Name == navInfo.Name)))
        {
          EdmNavigationProperty key = edmType.AddUnidirectionalNavigation(navInfo);
          dictionary.Add((IEdmProperty) key, navProp);
        }
        foreach (KeyValuePair<IEdmProperty, NavigationPropertyConfiguration> keyValuePair in dictionary)
        {
          IEdmProperty key = keyValuePair.Key;
          NavigationPropertyConfiguration propertyConfiguration = keyValuePair.Value;
          if (propertyConfiguration.PropertyInfo != (PropertyInfo) null)
            this._properties[propertyConfiguration.PropertyInfo] = key;
          if (propertyConfiguration.IsRestricted)
            this._propertiesRestrictions[key] = new QueryableRestrictions((PropertyConfiguration) propertyConfiguration);
          if (propertyConfiguration.QueryConfiguration.ModelBoundQuerySettings != null)
            this._propertiesQuerySettings.Add(key, propertyConfiguration.QueryConfiguration.ModelBoundQuerySettings);
        }
      }
    }

    private IList<IEdmStructuralProperty> GetDeclaringPropertyInfo(
      IEnumerable<PropertyInfo> propertyInfos)
    {
      IList<IEdmProperty> source = (IList<IEdmProperty>) new List<IEdmProperty>();
      foreach (PropertyInfo propertyInfo in propertyInfos)
      {
        IEdmProperty edmProperty;
        if (this._properties.TryGetValue(propertyInfo, out edmProperty))
        {
          source.Add(edmProperty);
        }
        else
        {
          for (Type baseType = TypeHelper.GetBaseType(TypeHelper.GetReflectedType((MemberInfo) propertyInfo)); baseType != (Type) null; baseType = TypeHelper.GetBaseType(baseType))
          {
            if (this._properties.TryGetValue(baseType.GetProperty(propertyInfo.Name), out edmProperty))
            {
              source.Add(edmProperty);
              break;
            }
          }
        }
      }
      return (IList<IEdmStructuralProperty>) source.OfType<IEdmStructuralProperty>().ToList<IEdmStructuralProperty>();
    }

    private void CreateEnumTypeBody(EdmEnumType type, EnumTypeConfiguration config)
    {
      foreach (EnumMemberConfiguration member1 in config.Members)
      {
        long int64;
        try
        {
          int64 = Convert.ToInt64((object) member1.MemberInfo, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch
        {
          throw Microsoft.AspNet.OData.Common.Error.Argument("value", SRResources.EnumValueCannotBeLong, (object) Enum.GetName(member1.MemberInfo.GetType(), (object) member1.MemberInfo));
        }
        EdmEnumMember member2 = new EdmEnumMember((IEdmEnumType) type, member1.Name, (IEdmEnumMemberValue) new EdmEnumMemberValue(int64));
        type.AddMember((IEdmEnumMember) member2);
        this._members[member1.MemberInfo] = (IEdmEnumMember) member2;
      }
    }

    private IEdmType GetEdmType(Type clrType)
    {
      IEdmType edmType;
      this._types.TryGetValue(clrType, out edmType);
      return edmType;
    }

    public static EdmTypeMap GetTypesAndProperties(IEnumerable<IEdmTypeConfiguration> configurations)
    {
      EdmTypeBuilder edmTypeBuilder = configurations != null ? new EdmTypeBuilder(configurations) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configurations));
      return new EdmTypeMap(edmTypeBuilder.GetEdmTypes(), edmTypeBuilder._properties, edmTypeBuilder._propertiesRestrictions, edmTypeBuilder._propertiesQuerySettings, edmTypeBuilder._structuredTypeQuerySettings, edmTypeBuilder._members, edmTypeBuilder._openTypes);
    }

    public static EdmPrimitiveTypeKind GetTypeKind(Type clrType) => (EdmLibHelpers.GetEdmPrimitiveTypeOrNull(clrType) ?? throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (clrType), SRResources.MustBePrimitiveType, (object) clrType.FullName)).PrimitiveKind;
  }
}
