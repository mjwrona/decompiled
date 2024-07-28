// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmTypeSemantics
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public static class EdmTypeSemantics
  {
    public static bool IsCollection(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.Collection;
    }

    public static bool IsEntity(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.Entity;
    }

    public static bool IsPath(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.Path;
    }

    public static bool IsEntityReference(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.EntityReference;
    }

    public static bool IsComplex(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.Complex;
    }

    public static bool IsUntyped(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.Untyped;
    }

    public static bool IsEnum(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.Enum;
    }

    public static bool IsTypeDefinition(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.TypeDefinition;
    }

    public static bool IsStructured(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      switch (type.TypeKind())
      {
        case EdmTypeKind.Entity:
        case EdmTypeKind.Complex:
          return true;
        default:
          return false;
      }
    }

    public static bool IsStructured(this EdmTypeKind typeKind) => typeKind == EdmTypeKind.Entity || typeKind == EdmTypeKind.Complex;

    public static bool IsPrimitive(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.TypeKind() == EdmTypeKind.Primitive;
    }

    public static bool IsBinary(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Binary;
    }

    public static bool IsBoolean(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Boolean;
    }

    public static bool IsTemporal(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.Definition.IsTemporal();
    }

    public static bool IsTemporal(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type is IEdmPrimitiveType edmPrimitiveType && edmPrimitiveType.PrimitiveKind.IsTemporal();
    }

    public static bool IsTemporal(this EdmPrimitiveTypeKind typeKind) => typeKind == EdmPrimitiveTypeKind.DateTimeOffset || typeKind == EdmPrimitiveTypeKind.Duration || typeKind == EdmPrimitiveTypeKind.TimeOfDay;

    public static bool IsDuration(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Duration;
    }

    public static bool IsDate(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Date;
    }

    public static bool IsDateTimeOffset(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.DateTimeOffset;
    }

    public static bool IsDecimal(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.Definition.IsDecimal();
    }

    public static bool IsDecimal(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type is IEdmPrimitiveType edmPrimitiveType && edmPrimitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Decimal;
    }

    public static bool IsFloating(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      switch (type.PrimitiveKind())
      {
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Single:
          return true;
        default:
          return false;
      }
    }

    public static bool IsSingle(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Single;
    }

    public static bool IsTimeOfDay(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.TimeOfDay;
    }

    public static bool IsDouble(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Double;
    }

    public static bool IsGuid(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Guid;
    }

    public static bool IsSignedIntegral(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      switch (type.PrimitiveKind())
      {
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
          return true;
        default:
          return false;
      }
    }

    public static bool IsSByte(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.SByte;
    }

    public static bool IsInt16(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int16;
    }

    public static bool IsInt32(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int32;
    }

    public static bool IsInt64(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int64;
    }

    public static bool IsIntegral(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      switch (type.PrimitiveKind())
      {
        case EdmPrimitiveTypeKind.Byte:
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
          return true;
        default:
          return false;
      }
    }

    public static bool IsIntegral(this EdmPrimitiveTypeKind primitiveTypeKind)
    {
      switch (primitiveTypeKind)
      {
        case EdmPrimitiveTypeKind.Byte:
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
          return true;
        default:
          return false;
      }
    }

    public static bool IsByte(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.PrimitiveKind() == EdmPrimitiveTypeKind.Byte;
    }

    public static bool IsString(this IEdmTypeReference type) => type.Definition.IsString();

    public static bool IsString(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type is IEdmPrimitiveType edmPrimitiveType && edmPrimitiveType.PrimitiveKind == EdmPrimitiveTypeKind.String;
    }

    public static bool IsUntyped(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type.TypeKind == EdmTypeKind.Untyped;
    }

    public static bool IsBinary(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type is IEdmPrimitiveType edmPrimitiveType && edmPrimitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Binary;
    }

    public static bool IsStream(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.Definition.IsStream();
    }

    public static bool IsStream(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type is IEdmPrimitiveType edmPrimitiveType && edmPrimitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Stream;
    }

    public static bool IsSpatial(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.Definition.IsSpatial();
    }

    public static bool IsSpatial(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type is IEdmPrimitiveType edmPrimitiveType && edmPrimitiveType.PrimitiveKind.IsSpatial();
    }

    public static bool IsSpatial(this EdmPrimitiveTypeKind typeKind) => (uint) (typeKind - 16) <= 15U;

    public static bool IsGeography(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type is IEdmPrimitiveType edmPrimitiveType && edmPrimitiveType.PrimitiveKind.IsGeography();
    }

    public static bool IsGeography(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.Definition.IsGeography();
    }

    public static bool IsGeography(this EdmPrimitiveTypeKind typeKind)
    {
      switch (typeKind)
      {
        case EdmPrimitiveTypeKind.Geography:
        case EdmPrimitiveTypeKind.GeographyPoint:
        case EdmPrimitiveTypeKind.GeographyLineString:
        case EdmPrimitiveTypeKind.GeographyPolygon:
        case EdmPrimitiveTypeKind.GeographyCollection:
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
          return true;
        default:
          return false;
      }
    }

    public static bool IsGeometry(this IEdmType type)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(type, nameof (type));
      return type is IEdmPrimitiveType edmPrimitiveType && edmPrimitiveType.PrimitiveKind.IsGeometry();
    }

    public static bool IsGeometry(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return type.Definition.IsGeometry();
    }

    public static bool IsGeometry(this EdmPrimitiveTypeKind typeKind)
    {
      switch (typeKind)
      {
        case EdmPrimitiveTypeKind.Geometry:
        case EdmPrimitiveTypeKind.GeometryPoint:
        case EdmPrimitiveTypeKind.GeometryLineString:
        case EdmPrimitiveTypeKind.GeometryPolygon:
        case EdmPrimitiveTypeKind.GeometryCollection:
        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
        case EdmPrimitiveTypeKind.GeometryMultiLineString:
        case EdmPrimitiveTypeKind.GeometryMultiPoint:
          return true;
        default:
          return false;
      }
    }

    public static IEdmPrimitiveTypeReference AsPrimitive(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmPrimitiveTypeReference primitiveTypeReference)
        return primitiveTypeReference;
      IEdmType definition1 = type.Definition;
      if (definition1.TypeKind == EdmTypeKind.Primitive)
      {
        if (definition1 is IEdmPrimitiveType definition2)
        {
          switch (definition2.PrimitiveKind)
          {
            case EdmPrimitiveTypeKind.Binary:
              return (IEdmPrimitiveTypeReference) type.AsBinary();
            case EdmPrimitiveTypeKind.Boolean:
            case EdmPrimitiveTypeKind.Byte:
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Guid:
            case EdmPrimitiveTypeKind.Int16:
            case EdmPrimitiveTypeKind.Int32:
            case EdmPrimitiveTypeKind.Int64:
            case EdmPrimitiveTypeKind.SByte:
            case EdmPrimitiveTypeKind.Single:
            case EdmPrimitiveTypeKind.Stream:
            case EdmPrimitiveTypeKind.Date:
            case EdmPrimitiveTypeKind.PrimitiveType:
              return (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference(definition2, type.IsNullable);
            case EdmPrimitiveTypeKind.DateTimeOffset:
            case EdmPrimitiveTypeKind.Duration:
            case EdmPrimitiveTypeKind.TimeOfDay:
              return (IEdmPrimitiveTypeReference) type.AsTemporal();
            case EdmPrimitiveTypeKind.Decimal:
              return (IEdmPrimitiveTypeReference) type.AsDecimal();
            case EdmPrimitiveTypeKind.String:
              return (IEdmPrimitiveTypeReference) type.AsString();
            case EdmPrimitiveTypeKind.Geography:
            case EdmPrimitiveTypeKind.GeographyPoint:
            case EdmPrimitiveTypeKind.GeographyLineString:
            case EdmPrimitiveTypeKind.GeographyPolygon:
            case EdmPrimitiveTypeKind.GeographyCollection:
            case EdmPrimitiveTypeKind.GeographyMultiPolygon:
            case EdmPrimitiveTypeKind.GeographyMultiLineString:
            case EdmPrimitiveTypeKind.GeographyMultiPoint:
            case EdmPrimitiveTypeKind.Geometry:
            case EdmPrimitiveTypeKind.GeometryPoint:
            case EdmPrimitiveTypeKind.GeometryLineString:
            case EdmPrimitiveTypeKind.GeometryPolygon:
            case EdmPrimitiveTypeKind.GeometryCollection:
            case EdmPrimitiveTypeKind.GeometryMultiPolygon:
            case EdmPrimitiveTypeKind.GeometryMultiLineString:
            case EdmPrimitiveTypeKind.GeometryMultiPoint:
              return (IEdmPrimitiveTypeReference) type.AsSpatial();
          }
        }
      }
      else if (definition1.TypeKind == EdmTypeKind.TypeDefinition)
      {
        IEdmPrimitiveType definition3 = definition1.UnderlyingType();
        if (!(type is IEdmTypeDefinitionReference definitionReference))
          return (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference(definition3, type.IsNullable);
        switch (definition3.PrimitiveKind)
        {
          case EdmPrimitiveTypeKind.Binary:
            return (IEdmPrimitiveTypeReference) new EdmBinaryTypeReference(definition3, definitionReference.IsNullable, definitionReference.IsUnbounded, definitionReference.MaxLength);
          case EdmPrimitiveTypeKind.DateTimeOffset:
          case EdmPrimitiveTypeKind.Duration:
          case EdmPrimitiveTypeKind.TimeOfDay:
            return (IEdmPrimitiveTypeReference) new EdmTemporalTypeReference(definition3, definitionReference.IsNullable, definitionReference.Precision);
          case EdmPrimitiveTypeKind.Decimal:
            return (IEdmPrimitiveTypeReference) new EdmDecimalTypeReference(definition3, definitionReference.IsNullable, definitionReference.Precision, definitionReference.Scale);
          case EdmPrimitiveTypeKind.String:
            return (IEdmPrimitiveTypeReference) new EdmStringTypeReference(definition3, definitionReference.IsNullable, definitionReference.IsUnbounded, definitionReference.MaxLength, definitionReference.IsUnicode);
          case EdmPrimitiveTypeKind.Geography:
          case EdmPrimitiveTypeKind.GeographyPoint:
          case EdmPrimitiveTypeKind.GeographyLineString:
          case EdmPrimitiveTypeKind.GeographyPolygon:
          case EdmPrimitiveTypeKind.GeographyCollection:
          case EdmPrimitiveTypeKind.GeographyMultiPolygon:
          case EdmPrimitiveTypeKind.GeographyMultiLineString:
          case EdmPrimitiveTypeKind.GeographyMultiPoint:
          case EdmPrimitiveTypeKind.Geometry:
          case EdmPrimitiveTypeKind.GeometryPoint:
          case EdmPrimitiveTypeKind.GeometryLineString:
          case EdmPrimitiveTypeKind.GeometryPolygon:
          case EdmPrimitiveTypeKind.GeometryCollection:
          case EdmPrimitiveTypeKind.GeometryMultiPolygon:
          case EdmPrimitiveTypeKind.GeometryMultiLineString:
          case EdmPrimitiveTypeKind.GeometryMultiPoint:
            return (IEdmPrimitiveTypeReference) new EdmSpatialTypeReference(definition3, definitionReference.IsNullable, definitionReference.SpatialReferenceIdentifier);
          default:
            return (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference(definition3, definitionReference.IsNullable);
        }
      }
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Primitive"));
      return (IEdmPrimitiveTypeReference) new BadPrimitiveTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static IEdmCollectionTypeReference AsCollection(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmCollectionTypeReference collectionTypeReference)
        return collectionTypeReference;
      IEdmType definition = type.Definition;
      if (definition.TypeKind == EdmTypeKind.Collection)
        return (IEdmCollectionTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) definition);
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), type.FullName(), "Collection"));
      return (IEdmCollectionTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new BadCollectionType((IEnumerable<EdmError>) errors));
    }

    public static IEdmStructuredTypeReference AsStructured(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmStructuredTypeReference structuredTypeReference)
        return structuredTypeReference;
      switch (type.TypeKind())
      {
        case EdmTypeKind.Entity:
          return (IEdmStructuredTypeReference) type.AsEntity();
        case EdmTypeKind.Complex:
          return (IEdmStructuredTypeReference) type.AsComplex();
        default:
          string str = type.FullName();
          List<EdmError> errors = new List<EdmError>(type.TypeErrors());
          if (errors.Count == 0)
            errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Structured"));
          return (IEdmStructuredTypeReference) new BadEntityTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
      }
    }

    public static IEdmEnumTypeReference AsEnum(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmEnumTypeReference enumTypeReference)
        return enumTypeReference;
      IEdmType definition = type.Definition;
      if (definition.TypeKind == EdmTypeKind.Enum)
        return (IEdmEnumTypeReference) new EdmEnumTypeReference((IEdmEnumType) definition, type.IsNullable);
      string str = type.FullName();
      return (IEdmEnumTypeReference) new EdmEnumTypeReference((IEdmEnumType) new BadEnumType(str, EdmTypeSemantics.ConversionError(type.Location(), str, "Enum")), type.IsNullable);
    }

    public static IEdmTypeDefinitionReference AsTypeDefinition(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmTypeDefinitionReference definitionReference)
        return definitionReference;
      IEdmType definition = type.Definition;
      if (definition.TypeKind == EdmTypeKind.TypeDefinition)
        return (IEdmTypeDefinitionReference) new EdmTypeDefinitionReference((IEdmTypeDefinition) definition, type.IsNullable);
      string str = type.FullName();
      return (IEdmTypeDefinitionReference) new EdmTypeDefinitionReference((IEdmTypeDefinition) new BadTypeDefinition(str, EdmTypeSemantics.ConversionError(type.Location(), str, "TypeDefinition")), type.IsNullable);
    }

    public static IEdmEntityTypeReference AsEntity(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmEntityTypeReference entityTypeReference)
        return entityTypeReference;
      IEdmType definition = type.Definition;
      if (definition.TypeKind == EdmTypeKind.Entity)
        return (IEdmEntityTypeReference) new EdmEntityTypeReference((IEdmEntityType) definition, type.IsNullable);
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Entity"));
      return (IEdmEntityTypeReference) new BadEntityTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static IEdmEntityReferenceTypeReference AsEntityReference(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmEntityReferenceTypeReference referenceTypeReference)
        return referenceTypeReference;
      IEdmType definition = type.Definition;
      if (definition.TypeKind == EdmTypeKind.EntityReference)
        return (IEdmEntityReferenceTypeReference) new EdmEntityReferenceTypeReference((IEdmEntityReferenceType) definition, type.IsNullable);
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), type.FullName(), "EntityReference"));
      return (IEdmEntityReferenceTypeReference) new EdmEntityReferenceTypeReference((IEdmEntityReferenceType) new BadEntityReferenceType((IEnumerable<EdmError>) errors), type.IsNullable);
    }

    public static IEdmComplexTypeReference AsComplex(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmComplexTypeReference complexTypeReference)
        return complexTypeReference;
      IEdmType definition = type.Definition;
      if (definition.TypeKind == EdmTypeKind.Complex)
        return (IEdmComplexTypeReference) new EdmComplexTypeReference((IEdmComplexType) definition, type.IsNullable);
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Complex"));
      return (IEdmComplexTypeReference) new BadComplexTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static IEdmPathTypeReference AsPath(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmPathTypeReference pathTypeReference)
        return pathTypeReference;
      IEdmType definition = type.Definition;
      if (definition.TypeKind == EdmTypeKind.Path)
        return (IEdmPathTypeReference) new EdmPathTypeReference((IEdmPathType) definition, type.IsNullable);
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Path"));
      return (IEdmPathTypeReference) new BadPathTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static IEdmSpatialTypeReference AsSpatial(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmSpatialTypeReference spatialTypeReference)
        return spatialTypeReference;
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Spatial"));
      return (IEdmSpatialTypeReference) new BadSpatialTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static IEdmTemporalTypeReference AsTemporal(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmTemporalTypeReference temporalTypeReference)
        return temporalTypeReference;
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Temporal"));
      return (IEdmTemporalTypeReference) new BadTemporalTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static IEdmDecimalTypeReference AsDecimal(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmDecimalTypeReference decimalTypeReference)
        return decimalTypeReference;
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Decimal"));
      return (IEdmDecimalTypeReference) new BadDecimalTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static IEdmStringTypeReference AsString(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmStringTypeReference stringTypeReference)
        return stringTypeReference;
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "String"));
      return (IEdmStringTypeReference) new BadStringTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static IEdmBinaryTypeReference AsBinary(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (type is IEdmBinaryTypeReference binaryTypeReference)
        return binaryTypeReference;
      string str = type.FullName();
      List<EdmError> errors = new List<EdmError>(type.Errors());
      if (errors.Count == 0)
        errors.AddRange(EdmTypeSemantics.ConversionError(type.Location(), str, "Binary"));
      return (IEdmBinaryTypeReference) new BadBinaryTypeReference(str, type.IsNullable, (IEnumerable<EdmError>) errors);
    }

    public static EdmPrimitiveTypeKind PrimitiveKind(this IEdmTypeReference type)
    {
      if (type == null)
        return EdmPrimitiveTypeKind.None;
      IEdmType definition = type.Definition;
      return definition.TypeKind != EdmTypeKind.Primitive ? EdmPrimitiveTypeKind.None : ((IEdmPrimitiveType) definition).PrimitiveKind;
    }

    public static bool InheritsFrom(
      this IEdmStructuredType type,
      IEdmStructuredType potentialBaseType)
    {
      do
      {
        type = type.BaseType;
        if (type != null && type.IsEquivalentTo((IEdmType) potentialBaseType))
          return true;
      }
      while (type != null);
      return false;
    }

    public static bool IsOrInheritsFrom(this IEdmType thisType, IEdmType otherType)
    {
      if (thisType == null || otherType == null)
        return false;
      if (thisType.IsEquivalentTo(otherType))
        return true;
      EdmTypeKind typeKind = thisType.TypeKind;
      return typeKind == otherType.TypeKind && (typeKind == EdmTypeKind.Entity || typeKind == EdmTypeKind.Complex) && ((IEdmStructuredType) thisType).InheritsFrom((IEdmStructuredType) otherType);
    }

    public static bool IsOnSameTypeHierarchyLineWith(this IEdmType thisType, IEdmType otherType) => thisType.IsOrInheritsFrom(otherType) || otherType.IsOrInheritsFrom(thisType);

    public static IEdmType AsActualType(this IEdmType type) => (IEdmType) type.UnderlyingType() ?? type;

    internal static IEdmPrimitiveTypeReference GetPrimitiveTypeReference(
      this IEdmPrimitiveType type,
      bool isNullable)
    {
      switch (type.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.Binary:
          return (IEdmPrimitiveTypeReference) new EdmBinaryTypeReference(type, isNullable);
        case EdmPrimitiveTypeKind.Boolean:
        case EdmPrimitiveTypeKind.Byte:
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Guid:
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
        case EdmPrimitiveTypeKind.Single:
        case EdmPrimitiveTypeKind.Stream:
        case EdmPrimitiveTypeKind.Date:
        case EdmPrimitiveTypeKind.PrimitiveType:
          return (IEdmPrimitiveTypeReference) new EdmPrimitiveTypeReference(type, isNullable);
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Duration:
        case EdmPrimitiveTypeKind.TimeOfDay:
          return (IEdmPrimitiveTypeReference) new EdmTemporalTypeReference(type, isNullable);
        case EdmPrimitiveTypeKind.Decimal:
          return (IEdmPrimitiveTypeReference) new EdmDecimalTypeReference(type, isNullable);
        case EdmPrimitiveTypeKind.String:
          return (IEdmPrimitiveTypeReference) new EdmStringTypeReference(type, isNullable);
        case EdmPrimitiveTypeKind.Geography:
        case EdmPrimitiveTypeKind.GeographyPoint:
        case EdmPrimitiveTypeKind.GeographyLineString:
        case EdmPrimitiveTypeKind.GeographyPolygon:
        case EdmPrimitiveTypeKind.GeographyCollection:
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
        case EdmPrimitiveTypeKind.Geometry:
        case EdmPrimitiveTypeKind.GeometryPoint:
        case EdmPrimitiveTypeKind.GeometryLineString:
        case EdmPrimitiveTypeKind.GeometryPolygon:
        case EdmPrimitiveTypeKind.GeometryCollection:
        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
        case EdmPrimitiveTypeKind.GeometryMultiLineString:
        case EdmPrimitiveTypeKind.GeometryMultiPoint:
          return (IEdmPrimitiveTypeReference) new EdmSpatialTypeReference(type, isNullable);
        default:
          throw new InvalidOperationException(Strings.EdmPrimitive_UnexpectedKind);
      }
    }

    internal static IEdmTypeReference GetTypeReference(this IEdmType type, bool isNullable)
    {
      switch (type)
      {
        case IEdmPrimitiveType type1:
          return (IEdmTypeReference) type1.GetPrimitiveTypeReference(isNullable);
        case IEdmComplexType complexType:
          return (IEdmTypeReference) new EdmComplexTypeReference(complexType, isNullable);
        case IEdmEntityType entityType:
          return (IEdmTypeReference) new EdmEntityTypeReference(entityType, isNullable);
        case IEdmEnumType enumType:
          return (IEdmTypeReference) new EdmEnumTypeReference(enumType, isNullable);
        case IEdmPathType definition:
          return (IEdmTypeReference) new EdmPathTypeReference(definition, isNullable);
        default:
          throw new InvalidOperationException(Strings.EdmType_UnexpectedEdmType);
      }
    }

    internal static IEdmPrimitiveType UnderlyingType(this IEdmType type) => type == null || type.TypeKind != EdmTypeKind.TypeDefinition ? (IEdmPrimitiveType) null : ((IEdmTypeDefinition) type).UnderlyingType;

    internal static IEdmPrimitiveType UnderlyingType(this IEdmTypeReference type) => type == null ? (IEdmPrimitiveType) null : type.Definition.UnderlyingType();

    internal static IEdmTypeReference AsActualTypeReference(this IEdmTypeReference type) => type == null || type.TypeKind() != EdmTypeKind.TypeDefinition ? type : (IEdmTypeReference) type.AsPrimitive();

    internal static bool CanSpecifyMaxLength(this IEdmPrimitiveType type)
    {
      switch (type.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.Binary:
        case EdmPrimitiveTypeKind.String:
        case EdmPrimitiveTypeKind.Stream:
          return true;
        default:
          return false;
      }
    }

    private static IEnumerable<EdmError> ConversionError(
      EdmLocation location,
      string typeName,
      string typeKindName)
    {
      return (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(location, EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, Strings.TypeSemantics_CouldNotConvertTypeReference((object) (typeName ?? "UnnamedType"), (object) typeKindName))
      };
    }
  }
}
