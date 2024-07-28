// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.MetadataUtilsCommon
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Metadata
{
  internal static class MetadataUtilsCommon
  {
    internal static bool IsODataPrimitiveTypeKind(this IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(typeReference, nameof (typeReference));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(typeReference.Definition, "typeReference.Definition");
      return typeReference.Definition.IsODataPrimitiveTypeKind();
    }

    internal static bool IsODataPrimitiveTypeKind(this IEdmType type)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(type, nameof (type));
      return type.TypeKind == EdmTypeKind.Primitive && !type.IsStream();
    }

    internal static bool IsODataComplexTypeKind(this IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(typeReference, nameof (typeReference));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(typeReference.Definition, "typeReference.Definition");
      return typeReference.Definition.IsODataComplexTypeKind();
    }

    internal static bool IsODataComplexTypeKind(this IEdmType type)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(type, nameof (type));
      return type.TypeKind == EdmTypeKind.Complex;
    }

    internal static bool IsODataEnumTypeKind(this IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(typeReference, nameof (typeReference));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(typeReference.Definition, "typeReference.Definition");
      return typeReference.Definition.IsODataEnumTypeKind();
    }

    internal static bool IsODataEnumTypeKind(this IEdmType type)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(type, nameof (type));
      return type.TypeKind == EdmTypeKind.Enum;
    }

    internal static bool IsODataTypeDefinitionTypeKind(this IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(typeReference, nameof (typeReference));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(typeReference.Definition, "typeReference.Definition");
      return typeReference.Definition.IsODataTypeDefinitionTypeKind();
    }

    internal static bool IsODataTypeDefinitionTypeKind(this IEdmType type)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(type, nameof (type));
      return type.TypeKind == EdmTypeKind.TypeDefinition;
    }

    internal static bool IsODataEntityTypeKind(this IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(typeReference, nameof (typeReference));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(typeReference.Definition, "typeReference.Definition");
      return typeReference.Definition.IsODataEntityTypeKind();
    }

    internal static bool IsODataEntityTypeKind(this IEdmType type)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(type, nameof (type));
      return type.TypeKind == EdmTypeKind.Entity;
    }

    internal static bool IsODataValueType(this IEdmTypeReference typeReference)
    {
      IEdmPrimitiveTypeReference type = typeReference.AsPrimitiveOrNull();
      if (type == null)
        return false;
      switch (ExtensionMethods.PrimitiveKind(type))
      {
        case EdmPrimitiveTypeKind.Boolean:
        case EdmPrimitiveTypeKind.Byte:
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Decimal:
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Guid:
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
        case EdmPrimitiveTypeKind.Single:
        case EdmPrimitiveTypeKind.Duration:
          return true;
        default:
          return false;
      }
    }

    internal static bool IsNonEntityCollectionType(this IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(typeReference, nameof (typeReference));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(typeReference.Definition, "typeReference.Definition");
      return typeReference.Definition.IsNonEntityCollectionType();
    }

    internal static bool IsEntityCollectionType(this IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(typeReference, nameof (typeReference));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(typeReference.Definition, "typeReference.Definition");
      return typeReference.Definition.IsEntityCollectionType();
    }

    internal static bool IsStructuredCollectionType(this IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(typeReference, nameof (typeReference));
      ExceptionUtils.CheckArgumentNotNull<IEdmType>(typeReference.Definition, "typeReference.Definition");
      return typeReference.Definition.IsStructuredCollectionType();
    }

    internal static bool IsNonEntityCollectionType(this IEdmType type) => type is IEdmCollectionType edmCollectionType && (edmCollectionType.ElementType == null || edmCollectionType.ElementType.TypeKind() != EdmTypeKind.Entity);

    internal static bool IsEntityCollectionType(this IEdmType type) => type is IEdmCollectionType edmCollectionType && (edmCollectionType.ElementType == null || edmCollectionType.ElementType.TypeKind() == EdmTypeKind.Entity);

    internal static bool IsStructuredCollectionType(this IEdmType type) => type is IEdmCollectionType edmCollectionType && (edmCollectionType.ElementType == null || edmCollectionType.ElementType.TypeKind() == EdmTypeKind.Entity || edmCollectionType.ElementType.TypeKind() == EdmTypeKind.Complex);

    internal static bool IsEntityOrEntityCollectionType(this IEdmType edmType) => edmType.IsEntityOrEntityCollectionType(out IEdmEntityType _);

    internal static bool IsEntityOrEntityCollectionType(
      this IEdmType edmType,
      out IEdmEntityType entityType)
    {
      if (edmType.TypeKind == EdmTypeKind.Entity)
      {
        entityType = (IEdmEntityType) edmType;
        return true;
      }
      if (edmType.TypeKind != EdmTypeKind.Collection)
      {
        entityType = (IEdmEntityType) null;
        return false;
      }
      entityType = ((IEdmCollectionType) edmType).ElementType.Definition as IEdmEntityType;
      return entityType != null;
    }

    internal static bool IsStructuredOrStructuredCollectionType(this IEdmType edmType) => edmType.IsStructuredOrStructuredCollectionType(out IEdmStructuredType _);

    internal static bool IsStructuredOrStructuredCollectionType(
      this IEdmType edmType,
      out IEdmStructuredType structuredType)
    {
      if (edmType.TypeKind.IsStructured())
      {
        structuredType = (IEdmStructuredType) edmType;
        return true;
      }
      if (edmType.TypeKind != EdmTypeKind.Collection)
      {
        structuredType = (IEdmStructuredType) null;
        return false;
      }
      structuredType = ((IEdmCollectionType) edmType).ElementType.Definition as IEdmStructuredType;
      return structuredType != null;
    }

    internal static IEdmPrimitiveTypeReference AsPrimitiveOrNull(
      this IEdmTypeReference typeReference)
    {
      if (typeReference == null)
        return (IEdmPrimitiveTypeReference) null;
      return typeReference.TypeKind() != EdmTypeKind.Primitive && typeReference.TypeKind() != EdmTypeKind.TypeDefinition ? (IEdmPrimitiveTypeReference) null : typeReference.AsPrimitive();
    }

    internal static IEdmEntityTypeReference AsEntityOrNull(this IEdmTypeReference typeReference)
    {
      if (typeReference == null)
        return (IEdmEntityTypeReference) null;
      return typeReference.TypeKind() != EdmTypeKind.Entity ? (IEdmEntityTypeReference) null : typeReference.AsEntity();
    }

    internal static IEdmStructuredTypeReference AsStructuredOrNull(
      this IEdmTypeReference typeReference)
    {
      if (typeReference == null)
        return (IEdmStructuredTypeReference) null;
      return !typeReference.IsStructured() ? (IEdmStructuredTypeReference) null : typeReference.AsStructured();
    }

    internal static bool CanConvertPrimitiveTypeTo(
      SingleValueNode sourceNodeOrNull,
      IEdmPrimitiveType sourcePrimitiveType,
      IEdmPrimitiveType targetPrimitiveType)
    {
      EdmPrimitiveTypeKind primitiveKind1 = sourcePrimitiveType.PrimitiveKind;
      EdmPrimitiveTypeKind primitiveKind2 = targetPrimitiveType.PrimitiveKind;
      if (primitiveKind2 == EdmPrimitiveTypeKind.PrimitiveType)
        return true;
      switch (primitiveKind1)
      {
        case EdmPrimitiveTypeKind.Byte:
          switch (primitiveKind2)
          {
            case EdmPrimitiveTypeKind.Byte:
            case EdmPrimitiveTypeKind.Decimal:
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Int16:
            case EdmPrimitiveTypeKind.Int32:
            case EdmPrimitiveTypeKind.Int64:
            case EdmPrimitiveTypeKind.Single:
              return true;
          }
          break;
        case EdmPrimitiveTypeKind.Double:
          if (primitiveKind2 == EdmPrimitiveTypeKind.Decimal)
            return MetadataUtilsCommon.TryGetConstantNodePrimitiveLDMF(sourceNodeOrNull, out object _);
          if (primitiveKind2 == EdmPrimitiveTypeKind.Double)
            return true;
          break;
        case EdmPrimitiveTypeKind.Int16:
          switch (primitiveKind2)
          {
            case EdmPrimitiveTypeKind.Decimal:
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Int16:
            case EdmPrimitiveTypeKind.Int32:
            case EdmPrimitiveTypeKind.Int64:
            case EdmPrimitiveTypeKind.Single:
              return true;
          }
          break;
        case EdmPrimitiveTypeKind.Int32:
          switch (primitiveKind2)
          {
            case EdmPrimitiveTypeKind.Decimal:
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Int32:
            case EdmPrimitiveTypeKind.Int64:
            case EdmPrimitiveTypeKind.Single:
              return true;
          }
          break;
        case EdmPrimitiveTypeKind.Int64:
          switch (primitiveKind2)
          {
            case EdmPrimitiveTypeKind.Decimal:
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Int64:
            case EdmPrimitiveTypeKind.Single:
              return true;
          }
          break;
        case EdmPrimitiveTypeKind.SByte:
          switch (primitiveKind2)
          {
            case EdmPrimitiveTypeKind.Decimal:
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Int16:
            case EdmPrimitiveTypeKind.Int32:
            case EdmPrimitiveTypeKind.Int64:
            case EdmPrimitiveTypeKind.SByte:
            case EdmPrimitiveTypeKind.Single:
              return true;
          }
          break;
        case EdmPrimitiveTypeKind.Single:
          switch (primitiveKind2)
          {
            case EdmPrimitiveTypeKind.Decimal:
              return MetadataUtilsCommon.TryGetConstantNodePrimitiveLDMF(sourceNodeOrNull, out object _);
            case EdmPrimitiveTypeKind.Double:
            case EdmPrimitiveTypeKind.Single:
              return true;
          }
          break;
        case EdmPrimitiveTypeKind.Date:
          if (primitiveKind2 == EdmPrimitiveTypeKind.DateTimeOffset || primitiveKind2 == EdmPrimitiveTypeKind.Date)
            return true;
          break;
        default:
          return primitiveKind1 == primitiveKind2 || EdmLibraryExtensions.IsAssignableFrom(targetPrimitiveType, sourcePrimitiveType);
      }
      return false;
    }

    internal static bool TryGetConstantNodePrimitiveLDMF(
      SingleValueNode sourceNodeOrNull,
      out object primitiveValue)
    {
      primitiveValue = (object) null;
      if (!(sourceNodeOrNull is ConstantNode constantNode) || !(constantNode.TypeReference.AsPrimitiveOrNull().Definition is IEdmPrimitiveType definition))
        return false;
      switch (definition.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.Decimal:
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.Single:
          primitiveValue = constantNode.Value;
          return true;
        default:
          return false;
      }
    }
  }
}
