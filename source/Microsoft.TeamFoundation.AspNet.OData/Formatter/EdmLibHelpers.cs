// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.EdmLibHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Microsoft.OData.UriParser;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.AspNet.OData.Formatter
{
  internal static class EdmLibHelpers
  {
    private static readonly EdmCoreModel _coreModel = EdmCoreModel.Instance;
    private static readonly Dictionary<Type, IEdmPrimitiveType> _builtInTypesMapping = ((IEnumerable<KeyValuePair<Type, IEdmPrimitiveType>>) new KeyValuePair<Type, IEdmPrimitiveType>[60]
    {
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (string), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (bool), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (bool?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (byte), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Byte)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (byte?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Byte)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Decimal), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Decimal?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (double), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Double)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (double?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Double)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Guid), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Guid)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Guid?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Guid)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (short), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int16)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (short?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int16)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (int), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (int?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (long), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (long?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (sbyte), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.SByte)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (sbyte?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.SByte)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (float), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Single)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (float?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Single)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (byte[]), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Binary)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Stream), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Stream)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Geography), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Geography)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeographyPoint), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeographyLineString), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeographyPolygon), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeographyCollection), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeographyMultiLineString), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeographyMultiPoint), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeographyMultiPolygon), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Geometry), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeometryPoint), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeometryLineString), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeometryPolygon), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeometryCollection), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeometryMultiLineString), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeometryMultiPoint), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (GeometryMultiPolygon), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (DateTimeOffset), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (DateTimeOffset?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (TimeSpan), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Duration)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (TimeSpan?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Duration)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Date), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Date)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Date?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Date)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (TimeOfDay), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (TimeOfDay?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (XElement), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (Binary), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Binary)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (ushort), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (ushort?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (uint), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (uint?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (ulong), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (ulong?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (char[]), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (char), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (char?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.String)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (DateTime), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)),
      new KeyValuePair<Type, IEdmPrimitiveType>(typeof (DateTime?), EdmLibHelpers.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset))
    }).ToDictionary<KeyValuePair<Type, IEdmPrimitiveType>, Type, IEdmPrimitiveType>((Func<KeyValuePair<Type, IEdmPrimitiveType>, Type>) (kvp => kvp.Key), (Func<KeyValuePair<Type, IEdmPrimitiveType>, IEdmPrimitiveType>) (kvp => kvp.Value));

    public static IEdmType GetEdmType(this IEdmModel edmModel, Type clrType)
    {
      if (edmModel == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmModel));
      return !(clrType == (Type) null) ? EdmLibHelpers.GetEdmType(edmModel, clrType, true) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (clrType));
    }

    private static IEdmType GetEdmType(IEdmModel edmModel, Type clrType, bool testCollections)
    {
      IEdmPrimitiveType primitiveTypeOrNull = EdmLibHelpers.GetEdmPrimitiveTypeOrNull(clrType);
      if (primitiveTypeOrNull != null)
        return (IEdmType) primitiveTypeOrNull;
      Type entityType;
      if (testCollections)
      {
        Type genericInterface = EdmLibHelpers.ExtractGenericInterface(clrType, typeof (IEnumerable<>));
        if (genericInterface != (Type) null)
        {
          Type type = genericInterface.GetGenericArguments()[0];
          if (EdmLibHelpers.IsSelectExpandWrapper(type, out entityType))
            type = entityType;
          if (EdmLibHelpers.IsComputeWrapper(type, out entityType))
            type = entityType;
          IEdmType edmType = EdmLibHelpers.GetEdmType(edmModel, type, false);
          if (edmType != null)
            return (IEdmType) new EdmCollectionType(edmType.ToEdmTypeReference(EdmLibHelpers.IsNullable(type)));
        }
      }
      if (EdmLibHelpers.IsComputeWrapper(clrType, out entityType))
        clrType = entityType;
      Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(clrType);
      if (TypeHelper.IsEnum(underlyingTypeOrSelf))
        clrType = underlyingTypeOrSelf;
      IEdmType edmType1 = edmModel.SchemaElements.OfType<IEdmType>().Select(edmType => new
      {
        EdmType = edmType,
        Annotation = edmModel.GetAnnotationValue<ClrTypeAnnotation>((IEdmElement) edmType)
      }).Where(tuple => tuple.Annotation != null && tuple.Annotation.ClrType == clrType).Select(tuple => tuple.EdmType).SingleOrDefault<IEdmType>() ?? (IEdmType) edmModel.FindType(clrType.EdmFullName());
      if (TypeHelper.GetBaseType(clrType) != (Type) null)
        edmType1 = edmType1 ?? EdmLibHelpers.GetEdmType(edmModel, TypeHelper.GetBaseType(clrType), testCollections);
      return edmType1;
    }

    public static IEdmTypeReference GetEdmTypeReference(this IEdmModel edmModel, Type clrType)
    {
      IEdmType edmType = edmModel.GetEdmType(clrType);
      if (edmType == null)
        return (IEdmTypeReference) null;
      bool isNullable = EdmLibHelpers.IsNullable(clrType);
      return edmType.ToEdmTypeReference(isNullable);
    }

    public static IEdmTypeReference ToEdmTypeReference(this IEdmType edmType, bool isNullable)
    {
      switch (edmType.TypeKind)
      {
        case EdmTypeKind.Primitive:
          return (IEdmTypeReference) EdmLibHelpers._coreModel.GetPrimitive((edmType as IEdmPrimitiveType).PrimitiveKind, isNullable);
        case EdmTypeKind.Entity:
          return (IEdmTypeReference) new EdmEntityTypeReference(edmType as IEdmEntityType, isNullable);
        case EdmTypeKind.Complex:
          return (IEdmTypeReference) new EdmComplexTypeReference(edmType as IEdmComplexType, isNullable);
        case EdmTypeKind.Collection:
          return (IEdmTypeReference) new EdmCollectionTypeReference(edmType as IEdmCollectionType);
        case EdmTypeKind.EntityReference:
          return (IEdmTypeReference) new EdmEntityReferenceTypeReference(edmType as IEdmEntityReferenceType, isNullable);
        case EdmTypeKind.Enum:
          return (IEdmTypeReference) new EdmEnumTypeReference(edmType as IEdmEnumType, isNullable);
        default:
          throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.EdmTypeNotSupported, (object) edmType.ToTraceString());
      }
    }

    public static IEdmCollectionType GetCollection(this IEdmEntityType entityType) => (IEdmCollectionType) new EdmCollectionType((IEdmTypeReference) new EdmEntityTypeReference(entityType, false));

    public static Type GetClrType(IEdmTypeReference edmTypeReference, IEdmModel edmModel) => EdmLibHelpers.GetClrType(edmTypeReference, edmModel, WebApiAssembliesResolver.Default);

    public static Type GetClrType(
      IEdmTypeReference edmTypeReference,
      IEdmModel edmModel,
      IWebApiAssembliesResolver assembliesResolver)
    {
      if (edmTypeReference == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmTypeReference));
      Type clrType1 = EdmLibHelpers._builtInTypesMapping.Where<KeyValuePair<Type, IEdmPrimitiveType>>((Func<KeyValuePair<Type, IEdmPrimitiveType>, bool>) (kvp =>
      {
        if (!edmTypeReference.Definition.IsEquivalentTo((IEdmType) kvp.Value))
          return false;
        return !edmTypeReference.IsNullable || EdmLibHelpers.IsNullable(kvp.Key);
      })).Select<KeyValuePair<Type, IEdmPrimitiveType>, Type>((Func<KeyValuePair<Type, IEdmPrimitiveType>, Type>) (kvp => kvp.Key)).FirstOrDefault<Type>();
      if (clrType1 != (Type) null)
        return clrType1;
      Type clrType2 = EdmLibHelpers.GetClrType(edmTypeReference.Definition, edmModel, assembliesResolver);
      return clrType2 != (Type) null && TypeHelper.IsEnum(clrType2) && edmTypeReference.IsNullable ? TypeHelper.ToNullable(clrType2) : clrType2;
    }

    public static Type GetClrType(IEdmType edmType, IEdmModel edmModel) => EdmLibHelpers.GetClrType(edmType, edmModel, WebApiAssembliesResolver.Default);

    public static Type GetClrType(
      IEdmType edmType,
      IEdmModel edmModel,
      IWebApiAssembliesResolver assembliesResolver)
    {
      IEdmSchemaType element = edmType as IEdmSchemaType;
      ClrTypeAnnotation annotationValue = edmModel.GetAnnotationValue<ClrTypeAnnotation>((IEdmElement) element);
      if (annotationValue != null)
        return annotationValue.ClrType;
      string edmFullName = element.FullName();
      IEnumerable<Type> matchingTypes = EdmLibHelpers.GetMatchingTypes(edmFullName, assembliesResolver);
      if (matchingTypes.Count<Type>() > 1)
        throw Microsoft.AspNet.OData.Common.Error.Argument("edmTypeReference", SRResources.MultipleMatchingClrTypesForEdmType, (object) edmFullName, (object) string.Join(",", matchingTypes.Select<Type, string>((Func<Type, string>) (type => type.AssemblyQualifiedName))));
      edmModel.SetAnnotationValue<ClrTypeAnnotation>((IEdmElement) element, new ClrTypeAnnotation(matchingTypes.SingleOrDefault<Type>()));
      return matchingTypes.SingleOrDefault<Type>();
    }

    public static bool IsNotFilterable(
      IEdmProperty edmProperty,
      IEdmProperty pathEdmProperty,
      IEdmStructuredType pathEdmStructuredType,
      IEdmModel edmModel,
      bool enableFilter)
    {
      QueryableRestrictionsAnnotation propertyRestrictions = EdmLibHelpers.GetPropertyRestrictions(edmProperty, edmModel);
      if (propertyRestrictions != null && propertyRestrictions.Restrictions.NotFilterable)
        return true;
      if (pathEdmStructuredType == null)
        pathEdmStructuredType = edmProperty.DeclaringType;
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings(pathEdmProperty, pathEdmStructuredType, edmModel);
      if (!enableFilter)
        return !boundQuerySettings.Filterable(edmProperty.Name);
      bool flag1;
      if (boundQuerySettings.FilterConfigurations.TryGetValue(edmProperty.Name, out flag1))
        return !flag1;
      bool? defaultEnableFilter = boundQuerySettings.DefaultEnableFilter;
      bool flag2 = false;
      return defaultEnableFilter.GetValueOrDefault() == flag2 & defaultEnableFilter.HasValue;
    }

    public static bool IsNotSortable(
      IEdmProperty edmProperty,
      IEdmProperty pathEdmProperty,
      IEdmStructuredType pathEdmStructuredType,
      IEdmModel edmModel,
      bool enableOrderBy)
    {
      QueryableRestrictionsAnnotation propertyRestrictions = EdmLibHelpers.GetPropertyRestrictions(edmProperty, edmModel);
      if (propertyRestrictions != null && propertyRestrictions.Restrictions.NotSortable)
        return true;
      if (pathEdmStructuredType == null)
        pathEdmStructuredType = edmProperty.DeclaringType;
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings(pathEdmProperty, pathEdmStructuredType, edmModel);
      if (!enableOrderBy)
        return !boundQuerySettings.Sortable(edmProperty.Name);
      bool flag1;
      if (boundQuerySettings.OrderByConfigurations.TryGetValue(edmProperty.Name, out flag1))
        return !flag1;
      bool? defaultEnableOrderBy = boundQuerySettings.DefaultEnableOrderBy;
      bool flag2 = false;
      return defaultEnableOrderBy.GetValueOrDefault() == flag2 & defaultEnableOrderBy.HasValue;
    }

    public static bool IsNotSelectable(
      IEdmProperty edmProperty,
      IEdmProperty pathEdmProperty,
      IEdmStructuredType pathEdmStructuredType,
      IEdmModel edmModel,
      bool enableSelect)
    {
      if (pathEdmStructuredType == null)
        pathEdmStructuredType = edmProperty.DeclaringType;
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings(pathEdmProperty, pathEdmStructuredType, edmModel);
      if (!enableSelect)
        return !boundQuerySettings.Selectable(edmProperty.Name);
      SelectExpandType selectExpandType1;
      if (boundQuerySettings.SelectConfigurations.TryGetValue(edmProperty.Name, out selectExpandType1))
        return selectExpandType1 == SelectExpandType.Disabled;
      SelectExpandType? defaultSelectType = boundQuerySettings.DefaultSelectType;
      SelectExpandType selectExpandType2 = SelectExpandType.Disabled;
      return defaultSelectType.GetValueOrDefault() == selectExpandType2 & defaultSelectType.HasValue;
    }

    public static bool IsNotNavigable(IEdmProperty edmProperty, IEdmModel edmModel)
    {
      QueryableRestrictionsAnnotation propertyRestrictions = EdmLibHelpers.GetPropertyRestrictions(edmProperty, edmModel);
      return propertyRestrictions != null && propertyRestrictions.Restrictions.NotNavigable;
    }

    public static bool IsNotExpandable(IEdmProperty edmProperty, IEdmModel edmModel)
    {
      QueryableRestrictionsAnnotation propertyRestrictions = EdmLibHelpers.GetPropertyRestrictions(edmProperty, edmModel);
      return propertyRestrictions != null && propertyRestrictions.Restrictions.NotExpandable;
    }

    public static bool IsAutoSelect(
      IEdmProperty property,
      IEdmProperty pathProperty,
      IEdmStructuredType pathStructuredType,
      IEdmModel edmModel,
      ModelBoundQuerySettings querySettings = null)
    {
      if (querySettings == null)
        querySettings = EdmLibHelpers.GetModelBoundQuerySettings(pathProperty, pathStructuredType, edmModel);
      return querySettings != null && querySettings.IsAutomaticSelect(property.Name);
    }

    public static bool IsAutoExpand(
      IEdmProperty navigationProperty,
      IEdmProperty pathProperty,
      IEdmStructuredType pathStructuredType,
      IEdmModel edmModel,
      bool isSelectPresent = false,
      ModelBoundQuerySettings querySettings = null)
    {
      QueryableRestrictionsAnnotation propertyRestrictions = EdmLibHelpers.GetPropertyRestrictions(navigationProperty, edmModel);
      if (propertyRestrictions != null && propertyRestrictions.Restrictions.AutoExpand)
        return !propertyRestrictions.Restrictions.DisableAutoExpandWhenSelectIsPresent || !isSelectPresent;
      if (querySettings == null)
        querySettings = EdmLibHelpers.GetModelBoundQuerySettings(pathProperty, pathStructuredType, edmModel);
      return querySettings != null && querySettings.IsAutomaticExpand(navigationProperty.Name);
    }

    public static IEnumerable<IEdmNavigationProperty> GetAutoExpandNavigationProperties(
      IEdmProperty pathProperty,
      IEdmStructuredType pathStructuredType,
      IEdmModel edmModel,
      bool isSelectPresent = false,
      ModelBoundQuerySettings querySettings = null)
    {
      List<IEdmNavigationProperty> navigationProperties = new List<IEdmNavigationProperty>();
      if (pathStructuredType is IEdmEntityType entityType1)
      {
        List<IEdmEntityType> edmEntityTypeList = new List<IEdmEntityType>();
        edmEntityTypeList.Add(entityType1);
        edmEntityTypeList.AddRange(EdmLibHelpers.GetAllDerivedEntityTypes(entityType1, edmModel));
        foreach (IEdmEntityType edmEntityType in edmEntityTypeList)
        {
          IEdmEntityType entityType = edmEntityType;
          IEnumerable<IEdmNavigationProperty> source = entityType == entityType1 ? entityType.NavigationProperties() : entityType.DeclaredNavigationProperties();
          if (source != null)
            navigationProperties.AddRange(source.Where<IEdmNavigationProperty>((Func<IEdmNavigationProperty, bool>) (navigationProperty => EdmLibHelpers.IsAutoExpand((IEdmProperty) navigationProperty, pathProperty, (IEdmStructuredType) entityType, edmModel, isSelectPresent, querySettings))));
        }
      }
      return (IEnumerable<IEdmNavigationProperty>) navigationProperties;
    }

    public static IEnumerable<IEdmStructuralProperty> GetAutoSelectProperties(
      IEdmProperty pathProperty,
      IEdmStructuredType pathStructuredType,
      IEdmModel edmModel,
      ModelBoundQuerySettings querySettings = null)
    {
      List<IEdmStructuralProperty> selectProperties = new List<IEdmStructuralProperty>();
      if (pathStructuredType is IEdmEntityType entityType1)
      {
        List<IEdmEntityType> edmEntityTypeList = new List<IEdmEntityType>();
        edmEntityTypeList.Add(entityType1);
        edmEntityTypeList.AddRange(EdmLibHelpers.GetAllDerivedEntityTypes(entityType1, edmModel));
        foreach (IEdmEntityType edmEntityType in edmEntityTypeList)
        {
          IEdmEntityType entityType = edmEntityType;
          IEnumerable<IEdmStructuralProperty> source = entityType == entityType1 ? entityType.StructuralProperties() : entityType.DeclaredStructuralProperties();
          if (source != null)
            selectProperties.AddRange(source.Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (property => EdmLibHelpers.IsAutoSelect((IEdmProperty) property, pathProperty, (IEdmStructuredType) entityType, edmModel, querySettings))));
        }
      }
      else if (pathStructuredType != null)
      {
        IEnumerable<IEdmStructuralProperty> source = pathStructuredType.StructuralProperties();
        if (source != null)
          selectProperties.AddRange(source.Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (property => EdmLibHelpers.IsAutoSelect((IEdmProperty) property, pathProperty, pathStructuredType, edmModel, querySettings))));
      }
      return (IEnumerable<IEdmStructuralProperty>) selectProperties;
    }

    public static bool IsTopLimitExceeded(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      IEdmModel edmModel,
      int top,
      DefaultQuerySettings defaultQuerySettings,
      out int maxTop)
    {
      maxTop = 0;
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings(property, structuredType, edmModel, defaultQuerySettings);
      if (boundQuerySettings != null)
      {
        int num = top;
        int? maxTop1 = boundQuerySettings.MaxTop;
        int valueOrDefault = maxTop1.GetValueOrDefault();
        if (num > valueOrDefault & maxTop1.HasValue)
        {
          maxTop = boundQuerySettings.MaxTop.Value;
          return true;
        }
      }
      return false;
    }

    public static bool IsNotCountable(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      IEdmModel edmModel,
      bool enableCount)
    {
      if (property != null)
      {
        QueryableRestrictionsAnnotation propertyRestrictions = EdmLibHelpers.GetPropertyRestrictions(property, edmModel);
        if (propertyRestrictions != null && propertyRestrictions.Restrictions.NotCountable)
          return true;
      }
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings(property, structuredType, edmModel);
      if (boundQuerySettings != null)
      {
        if (boundQuerySettings.Countable.HasValue || enableCount)
        {
          bool? countable = boundQuerySettings.Countable;
          bool flag = false;
          if (!(countable.GetValueOrDefault() == flag & countable.HasValue))
            goto label_7;
        }
        return true;
      }
label_7:
      return false;
    }

    public static bool IsExpandable(
      string propertyName,
      IEdmProperty property,
      IEdmStructuredType structuredType,
      IEdmModel edmModel,
      out ExpandConfiguration expandConfiguration)
    {
      expandConfiguration = (ExpandConfiguration) null;
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings(property, structuredType, edmModel);
      if (boundQuerySettings == null)
        return false;
      bool flag = boundQuerySettings.Expandable(propertyName);
      if (!boundQuerySettings.ExpandConfigurations.TryGetValue(propertyName, out expandConfiguration) & flag)
        expandConfiguration = new ExpandConfiguration()
        {
          ExpandType = boundQuerySettings.DefaultExpandType.Value,
          MaxDepth = boundQuerySettings.DefaultMaxDepth
        };
      return flag;
    }

    public static ModelBoundQuerySettings GetModelBoundQuerySettings(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      IEdmModel edmModel,
      DefaultQuerySettings defaultQuerySettings = null)
    {
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings<IEdmStructuredType>(structuredType, edmModel, defaultQuerySettings);
      return property == null ? boundQuerySettings : EdmLibHelpers.GetMergedPropertyQuerySettings(EdmLibHelpers.GetModelBoundQuerySettings<IEdmProperty>(property, edmModel, defaultQuerySettings), boundQuerySettings);
    }

    public static IEnumerable<IEdmEntityType> GetAllDerivedEntityTypes(
      IEdmEntityType entityType,
      IEdmModel edmModel)
    {
      List<IEdmEntityType> derivedEntityTypes = new List<IEdmEntityType>();
      if (entityType != null)
      {
        List<IEdmStructuredType> edmStructuredTypeList = new List<IEdmStructuredType>();
        edmStructuredTypeList.Add((IEdmStructuredType) entityType);
        while (edmStructuredTypeList.Count > 0)
        {
          IEdmStructuredType baseType = edmStructuredTypeList[0];
          derivedEntityTypes.Add(baseType as IEdmEntityType);
          IEnumerable<IEdmStructuredType> directlyDerivedTypes = edmModel.FindDirectlyDerivedTypes(baseType);
          if (directlyDerivedTypes != null)
            edmStructuredTypeList.AddRange(directlyDerivedTypes);
          edmStructuredTypeList.RemoveAt(0);
        }
      }
      derivedEntityTypes.RemoveAt(0);
      return (IEnumerable<IEdmEntityType>) derivedEntityTypes;
    }

    public static IEdmType GetElementType(IEdmTypeReference edmTypeReference) => edmTypeReference.IsCollection() ? edmTypeReference.AsCollection().ElementType().Definition : edmTypeReference.Definition;

    public static void GetPropertyAndStructuredTypeFromPath(
      IEnumerable<ODataPathSegment> segments,
      out IEdmProperty property,
      out IEdmStructuredType structuredType,
      out string name)
    {
      property = (IEdmProperty) null;
      structuredType = (IEdmStructuredType) null;
      name = string.Empty;
      string str = string.Empty;
      if (segments == null)
        return;
      foreach (ODataPathSegment odataPathSegment in segments.Reverse<ODataPathSegment>())
      {
        switch (odataPathSegment)
        {
          case NavigationPropertySegment navigationPropertySegment:
            property = (IEdmProperty) navigationPropertySegment.NavigationProperty;
            if (structuredType == null)
              structuredType = (IEdmStructuredType) navigationPropertySegment.NavigationProperty.ToEntityType();
            name = navigationPropertySegment.NavigationProperty.Name + str;
            return;
          case PropertySegment propertySegment:
            property = (IEdmProperty) propertySegment.Property;
            if (structuredType == null)
              structuredType = EdmLibHelpers.GetElementType(property.Type) as IEdmStructuredType;
            name = property.Name + str;
            return;
          case EntitySetSegment entitySetSegment:
            if (structuredType == null)
              structuredType = (IEdmStructuredType) entitySetSegment.EntitySet.EntityType();
            name = entitySetSegment.EntitySet.Name + str;
            return;
          case TypeSegment typeSegment:
            structuredType = EdmLibHelpers.GetElementType(typeSegment.EdmType.ToEdmTypeReference(false)) as IEdmStructuredType;
            str = "/" + structuredType?.ToString();
            continue;
          default:
            continue;
        }
      }
    }

    public static string GetClrPropertyName(IEdmProperty edmProperty, IEdmModel edmModel)
    {
      if (edmProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmProperty));
      if (edmModel == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmModel));
      string clrPropertyName = edmProperty.Name;
      ClrPropertyInfoAnnotation annotationValue = edmModel.GetAnnotationValue<ClrPropertyInfoAnnotation>((IEdmElement) edmProperty);
      if (annotationValue != null)
      {
        PropertyInfo clrPropertyInfo = annotationValue.ClrPropertyInfo;
        if (clrPropertyInfo != (PropertyInfo) null)
        {
          clrPropertyName = clrPropertyInfo.Name;
          if (annotationValue.PropertiesPath != null && annotationValue.PropertiesPath.Any<PropertyInfo>())
            clrPropertyName = string.Join(string.Empty, annotationValue.PropertiesPath.Select<PropertyInfo, string>((Func<PropertyInfo, string>) (p => p.Name + "\\"))) + clrPropertyName;
        }
      }
      return clrPropertyName;
    }

    public static ClrEnumMemberAnnotation GetClrEnumMemberAnnotation(
      this IEdmModel edmModel,
      IEdmEnumType enumType)
    {
      if (edmModel == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmModel));
      return edmModel.GetAnnotationValue<ClrEnumMemberAnnotation>((IEdmElement) enumType) ?? (ClrEnumMemberAnnotation) null;
    }

    public static PropertyInfo GetDynamicPropertyDictionary(
      IEdmStructuredType edmType,
      IEdmModel edmModel)
    {
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      if (edmModel == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmModel));
      return edmModel.GetAnnotationValue<DynamicPropertyDictionaryAnnotation>((IEdmElement) edmType)?.PropertyInfo;
    }

    public static bool HasLength(EdmPrimitiveTypeKind primitiveTypeKind) => primitiveTypeKind == EdmPrimitiveTypeKind.Binary || primitiveTypeKind == EdmPrimitiveTypeKind.String;

    public static bool HasPrecision(EdmPrimitiveTypeKind primitiveTypeKind) => primitiveTypeKind == EdmPrimitiveTypeKind.Decimal || primitiveTypeKind == EdmPrimitiveTypeKind.DateTimeOffset || primitiveTypeKind == EdmPrimitiveTypeKind.Duration || primitiveTypeKind == EdmPrimitiveTypeKind.TimeOfDay;

    public static IEdmPrimitiveType GetEdmPrimitiveTypeOrNull(Type clrType)
    {
      IEdmPrimitiveType edmPrimitiveType;
      return !EdmLibHelpers._builtInTypesMapping.TryGetValue(clrType, out edmPrimitiveType) ? (IEdmPrimitiveType) null : edmPrimitiveType;
    }

    public static IEdmPrimitiveTypeReference GetEdmPrimitiveTypeReferenceOrNull(Type clrType)
    {
      IEdmPrimitiveType primitiveTypeOrNull = EdmLibHelpers.GetEdmPrimitiveTypeOrNull(clrType);
      return primitiveTypeOrNull == null ? (IEdmPrimitiveTypeReference) null : EdmLibHelpers._coreModel.GetPrimitive(primitiveTypeOrNull.PrimitiveKind, EdmLibHelpers.IsNullable(clrType));
    }

    public static Type IsNonstandardEdmPrimitive(Type type, out bool isNonstandardEdmPrimitive)
    {
      IEdmPrimitiveTypeReference typeReferenceOrNull = EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(type);
      if (typeReferenceOrNull == null)
      {
        isNonstandardEdmPrimitive = false;
        return type;
      }
      Type clrType = EdmLibHelpers.GetClrType((IEdmTypeReference) typeReferenceOrNull, (IEdmModel) EdmCoreModel.Instance);
      isNonstandardEdmPrimitive = type != clrType;
      return clrType;
    }

    public static string EdmName(this Type clrType) => EdmLibHelpers.MangleClrTypeName(clrType);

    public static string EdmFullName(this Type clrType) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", new object[2]
    {
      (object) clrType.Namespace,
      (object) clrType.EdmName()
    });

    public static IEnumerable<IEdmStructuralProperty> GetConcurrencyProperties(
      this IEdmModel model,
      IEdmNavigationSource navigationSource)
    {
      ConcurrencyPropertiesAnnotation propertiesAnnotation = model.GetAnnotationValue<ConcurrencyPropertiesAnnotation>((IEdmElement) model);
      if (propertiesAnnotation == null)
      {
        propertiesAnnotation = new ConcurrencyPropertiesAnnotation();
        model.SetAnnotationValue<ConcurrencyPropertiesAnnotation>((IEdmElement) model, propertiesAnnotation);
      }
      IEnumerable<IEdmStructuralProperty> concurrencyProperties1;
      if (propertiesAnnotation.TryGetValue(navigationSource, out concurrencyProperties1))
        return concurrencyProperties1;
      IList<IEdmStructuralProperty> concurrencyProperties2 = (IList<IEdmStructuralProperty>) new List<IEdmStructuralProperty>();
      IEdmEntityType edmEntityType = navigationSource.EntityType();
      if (navigationSource is IEdmVocabularyAnnotatable element1)
      {
        IEdmVocabularyAnnotation vocabularyAnnotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(element1, CoreVocabularyModel.ConcurrencyTerm).FirstOrDefault<IEdmVocabularyAnnotation>();
        if (vocabularyAnnotation != null && vocabularyAnnotation.Value is IEdmCollectionExpression collectionExpression)
        {
          foreach (IEdmExpression element in collectionExpression.Elements)
          {
            if (element is IEdmPathExpression edmPathExpression)
            {
              string name = edmPathExpression.PathSegments.First<string>();
              if (edmEntityType.FindProperty(name) is IEdmStructuralProperty property)
                concurrencyProperties2.Add(property);
            }
          }
        }
      }
      propertiesAnnotation[navigationSource] = (IEnumerable<IEdmStructuralProperty>) concurrencyProperties2;
      return (IEnumerable<IEdmStructuralProperty>) concurrencyProperties2;
    }

    public static bool IsDynamicTypeWrapper(Type type) => type != (Type) null && typeof (DynamicTypeWrapper).IsAssignableFrom(type);

    public static bool HasDynamicTypeWrapper(Type type)
    {
      if (!(type != (Type) null))
        return false;
      if (EdmLibHelpers.IsDynamicTypeWrapper(type))
        return true;
      return type.IsGenericType && EdmLibHelpers.IsDynamicTypeWrapper(type.GetGenericArguments()[0]);
    }

    public static bool IsAggregatedTypeWrapper(Type type)
    {
      Type entityType;
      return EdmLibHelpers.IsSelectExpandWrapper(type, out entityType) || EdmLibHelpers.IsComputeWrapper(type, out entityType) ? EdmLibHelpers.IsAggregatedTypeWrapper(entityType) : EdmLibHelpers.IsDynamicTypeWrapper(type);
    }

    public static bool IsNullable(Type type) => !TypeHelper.IsValueType(type) || Nullable.GetUnderlyingType(type) != (Type) null;

    internal static IEdmTypeReference GetExpectedPayloadType(
      Type type,
      Microsoft.AspNet.OData.Routing.ODataPath path,
      IEdmModel model)
    {
      IEdmTypeReference type1 = (IEdmTypeReference) null;
      if (typeof (IEdmObject).IsAssignableFrom(type))
      {
        IEdmType edmType = path.EdmType;
        if (edmType != null)
        {
          type1 = edmType.ToEdmTypeReference(false);
          if (type1.TypeKind() == EdmTypeKind.Collection)
          {
            IEdmTypeReference type2 = type1.AsCollection().ElementType();
            if (type2.IsEntity())
              type1 = type2;
          }
        }
      }
      else
      {
        EdmLibHelpers.TryGetInnerTypeForDelta(ref type);
        type1 = model.GetEdmTypeReference(type);
      }
      return type1;
    }

    internal static bool TryGetInnerTypeForDelta(ref Type type)
    {
      if (!type.IsGenericType() || !(type.GetGenericTypeDefinition() == typeof (Delta<>)))
        return false;
      type = type.GetGenericArguments()[0];
      return true;
    }

    private static ModelBoundQuerySettings GetMergedPropertyQuerySettings(
      ModelBoundQuerySettings propertyQuerySettings,
      ModelBoundQuerySettings propertyTypeQuerySettings)
    {
      ModelBoundQuerySettings propertyQuerySettings1 = new ModelBoundQuerySettings(propertyQuerySettings);
      if (propertyTypeQuerySettings != null)
      {
        if (!propertyQuerySettings1.PageSize.HasValue)
          propertyQuerySettings1.PageSize = propertyTypeQuerySettings.PageSize;
        int? maxTop1 = propertyQuerySettings1.MaxTop;
        int num1 = 0;
        if (maxTop1.GetValueOrDefault() == num1 & maxTop1.HasValue)
        {
          int? maxTop2 = propertyTypeQuerySettings.MaxTop;
          int num2 = 0;
          if (!(maxTop2.GetValueOrDefault() == num2 & maxTop2.HasValue))
            propertyQuerySettings1.MaxTop = propertyTypeQuerySettings.MaxTop;
        }
        if (!propertyQuerySettings1.Countable.HasValue)
          propertyQuerySettings1.Countable = propertyTypeQuerySettings.Countable;
        if (propertyQuerySettings1.OrderByConfigurations.Count == 0 && !propertyQuerySettings1.DefaultEnableOrderBy.HasValue)
        {
          propertyQuerySettings1.CopyOrderByConfigurations(propertyTypeQuerySettings.OrderByConfigurations);
          propertyQuerySettings1.DefaultEnableOrderBy = propertyTypeQuerySettings.DefaultEnableOrderBy;
        }
        if (propertyQuerySettings1.FilterConfigurations.Count == 0 && !propertyQuerySettings1.DefaultEnableFilter.HasValue)
        {
          propertyQuerySettings1.CopyFilterConfigurations(propertyTypeQuerySettings.FilterConfigurations);
          propertyQuerySettings1.DefaultEnableFilter = propertyTypeQuerySettings.DefaultEnableFilter;
        }
        if (propertyQuerySettings1.SelectConfigurations.Count == 0 && !propertyQuerySettings1.DefaultSelectType.HasValue)
        {
          propertyQuerySettings1.CopySelectConfigurations(propertyTypeQuerySettings.SelectConfigurations);
          propertyQuerySettings1.DefaultSelectType = propertyTypeQuerySettings.DefaultSelectType;
        }
        if (propertyQuerySettings1.ExpandConfigurations.Count == 0 && !propertyQuerySettings1.DefaultExpandType.HasValue)
        {
          propertyQuerySettings1.CopyExpandConfigurations(propertyTypeQuerySettings.ExpandConfigurations);
          propertyQuerySettings1.DefaultExpandType = propertyTypeQuerySettings.DefaultExpandType;
          propertyQuerySettings1.DefaultMaxDepth = propertyTypeQuerySettings.DefaultMaxDepth;
        }
      }
      return propertyQuerySettings1;
    }

    internal static ModelBoundQuerySettings GetModelBoundQuerySettings<T>(
      T key,
      IEdmModel edmModel,
      DefaultQuerySettings defaultQuerySettings = null)
      where T : IEdmElement
    {
      if ((object) key == null)
        return (ModelBoundQuerySettings) null;
      ModelBoundQuerySettings boundQuerySettings = edmModel.GetAnnotationValue<ModelBoundQuerySettings>((IEdmElement) key);
      if (boundQuerySettings == null)
      {
        boundQuerySettings = new ModelBoundQuerySettings();
        if (defaultQuerySettings != null)
        {
          if (defaultQuerySettings.MaxTop.HasValue)
          {
            int? maxTop = defaultQuerySettings.MaxTop;
            int num = 0;
            if (!(maxTop.GetValueOrDefault() > num & maxTop.HasValue))
              goto label_7;
          }
          boundQuerySettings.MaxTop = defaultQuerySettings.MaxTop;
        }
      }
label_7:
      return boundQuerySettings;
    }

    private static QueryableRestrictionsAnnotation GetPropertyRestrictions(
      IEdmProperty edmProperty,
      IEdmModel edmModel)
    {
      return edmModel.GetAnnotationValue<QueryableRestrictionsAnnotation>((IEdmElement) edmProperty);
    }

    private static IEdmPrimitiveType GetPrimitiveType(EdmPrimitiveTypeKind primitiveKind) => EdmLibHelpers._coreModel.GetPrimitiveType(primitiveKind);

    private static bool IsSelectExpandWrapper(Type type, out Type entityType) => EdmLibHelpers.IsTypeWrapper(typeof (SelectExpandWrapper<>), type, out entityType);

    internal static bool IsComputeWrapper(Type type, out Type entityType) => EdmLibHelpers.IsTypeWrapper(typeof (ComputeWrapper<>), type, out entityType);

    private static bool IsTypeWrapper(Type wrappedType, Type type, out Type entityType)
    {
      if (type == (Type) null)
      {
        entityType = (Type) null;
        return false;
      }
      if (!type.IsGenericType() || !(type.GetGenericTypeDefinition() == wrappedType))
        return EdmLibHelpers.IsTypeWrapper(wrappedType, TypeHelper.GetBaseType(type), out entityType);
      entityType = type.GetGenericArguments()[0];
      return true;
    }

    private static Type ExtractGenericInterface(Type queryType, Type interfaceType)
    {
      Func<Type, bool> predicate = (Func<Type, bool>) (t => t.IsGenericType() && t.GetGenericTypeDefinition() == interfaceType);
      return !predicate(queryType) ? ((IEnumerable<Type>) queryType.GetInterfaces()).FirstOrDefault<Type>(predicate) : queryType;
    }

    private static IEnumerable<Type> GetMatchingTypes(
      string edmFullName,
      IWebApiAssembliesResolver assembliesResolver)
    {
      return TypeHelper.GetLoadedTypes(assembliesResolver).Where<Type>((Func<Type, bool>) (t => TypeHelper.IsPublic(t) && t.EdmFullName() == edmFullName));
    }

    private static string MangleClrTypeName(Type type)
    {
      if (!type.IsGenericType())
        return type.Name;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}Of{1}", new object[2]
      {
        (object) type.Name.Replace('`', '_'),
        (object) string.Join("_", ((IEnumerable<Type>) type.GetGenericArguments()).Select<Type, string>((Func<Type, string>) (t => EdmLibHelpers.MangleClrTypeName(t))))
      });
    }
  }
}
