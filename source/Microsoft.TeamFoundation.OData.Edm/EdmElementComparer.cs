// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmElementComparer
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm
{
  public static class EdmElementComparer
  {
    public static bool IsEquivalentTo(this IEdmType thisType, IEdmType otherType)
    {
      if (thisType == otherType)
        return true;
      if (thisType == null || otherType == null)
        return false;
      thisType = thisType.AsActualType();
      otherType = otherType.AsActualType();
      if (thisType.TypeKind != otherType.TypeKind)
        return false;
      switch (thisType.TypeKind)
      {
        case EdmTypeKind.None:
          return otherType.TypeKind == EdmTypeKind.None;
        case EdmTypeKind.Primitive:
          return EdmElementComparer.IsEquivalentTo((IEdmPrimitiveType) thisType, (IEdmPrimitiveType) otherType);
        case EdmTypeKind.Entity:
        case EdmTypeKind.Complex:
          return EdmElementComparer.IsEquivalentTo((IEdmSchemaType) thisType, (IEdmSchemaType) otherType);
        case EdmTypeKind.Collection:
          return EdmElementComparer.IsEquivalentTo((IEdmCollectionType) thisType, (IEdmCollectionType) otherType);
        case EdmTypeKind.EntityReference:
          return EdmElementComparer.IsEquivalentTo((IEdmEntityReferenceType) thisType, (IEdmEntityReferenceType) otherType);
        case EdmTypeKind.Enum:
          return EdmElementComparer.IsEquivalentTo((IEdmEnumType) thisType, (IEdmEnumType) otherType);
        default:
          throw new InvalidOperationException(Strings.UnknownEnumVal_TypeKind((object) thisType.TypeKind));
      }
    }

    public static bool IsEquivalentTo(this IEdmTypeReference thisType, IEdmTypeReference otherType)
    {
      if (thisType == otherType)
        return true;
      if (thisType == null || otherType == null)
        return false;
      thisType = thisType.AsActualTypeReference();
      otherType = otherType.AsActualTypeReference();
      EdmTypeKind edmTypeKind = thisType.TypeKind();
      if (edmTypeKind != otherType.TypeKind())
        return false;
      if (edmTypeKind == EdmTypeKind.Primitive)
        return EdmElementComparer.IsEquivalentTo((IEdmPrimitiveTypeReference) thisType, (IEdmPrimitiveTypeReference) otherType);
      return thisType.IsNullable == otherType.IsNullable && thisType.Definition.IsEquivalentTo(otherType.Definition);
    }

    private static bool IsEquivalentTo(this IEdmPrimitiveType thisType, IEdmPrimitiveType otherType) => thisType.PrimitiveKind == otherType.PrimitiveKind && thisType.FullName() == otherType.FullName();

    private static bool IsEquivalentTo(this IEdmEnumType thisType, IEdmEnumType otherType) => thisType.FullName() == otherType.FullName() && EdmElementComparer.IsEquivalentTo(thisType.UnderlyingType, otherType.UnderlyingType) && thisType.IsFlags == otherType.IsFlags;

    private static bool IsEquivalentTo(this IEdmSchemaType thisType, IEdmSchemaType otherType) => thisType == otherType;

    private static bool IsEquivalentTo(
      this IEdmCollectionType thisType,
      IEdmCollectionType otherType)
    {
      return thisType.ElementType.IsEquivalentTo(otherType.ElementType);
    }

    private static bool IsEquivalentTo(
      this IEdmEntityReferenceType thisType,
      IEdmEntityReferenceType otherType)
    {
      return EdmElementComparer.IsEquivalentTo(thisType.EntityType, (IEdmSchemaType) otherType.EntityType);
    }

    private static bool IsEquivalentTo(
      this IEdmPrimitiveTypeReference thisType,
      IEdmPrimitiveTypeReference otherType)
    {
      EdmPrimitiveTypeKind typeKind = ExtensionMethods.PrimitiveKind(thisType);
      if (typeKind != ExtensionMethods.PrimitiveKind(otherType))
        return false;
      switch (typeKind)
      {
        case EdmPrimitiveTypeKind.Binary:
          return EdmElementComparer.IsEquivalentTo(thisType as IEdmBinaryTypeReference, otherType as IEdmBinaryTypeReference);
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Duration:
          return EdmElementComparer.IsEquivalentTo(thisType as IEdmTemporalTypeReference, otherType as IEdmTemporalTypeReference);
        case EdmPrimitiveTypeKind.Decimal:
          return EdmElementComparer.IsEquivalentTo(thisType as IEdmDecimalTypeReference, otherType as IEdmDecimalTypeReference);
        case EdmPrimitiveTypeKind.String:
          return EdmElementComparer.IsEquivalentTo(thisType as IEdmStringTypeReference, otherType as IEdmStringTypeReference);
        default:
          if (typeKind.IsSpatial())
            return EdmElementComparer.IsEquivalentTo(thisType as IEdmSpatialTypeReference, otherType as IEdmSpatialTypeReference);
          return thisType.IsNullable == otherType.IsNullable && thisType.Definition.IsEquivalentTo(otherType.Definition);
      }
    }

    private static bool IsEquivalentTo(
      this IEdmBinaryTypeReference thisType,
      IEdmBinaryTypeReference otherType)
    {
      if (thisType == null || otherType == null || thisType.IsNullable != otherType.IsNullable || thisType.IsUnbounded != otherType.IsUnbounded)
        return false;
      int? maxLength1 = thisType.MaxLength;
      int? maxLength2 = otherType.MaxLength;
      return maxLength1.GetValueOrDefault() == maxLength2.GetValueOrDefault() & maxLength1.HasValue == maxLength2.HasValue;
    }

    private static bool IsEquivalentTo(
      this IEdmDecimalTypeReference thisType,
      IEdmDecimalTypeReference otherType)
    {
      if (thisType != null && otherType != null && thisType.IsNullable == otherType.IsNullable)
      {
        int? nullable1 = thisType.Precision;
        int? nullable2 = otherType.Precision;
        if (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue)
        {
          nullable2 = thisType.Scale;
          nullable1 = otherType.Scale;
          return nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() & nullable2.HasValue == nullable1.HasValue;
        }
      }
      return false;
    }

    private static bool IsEquivalentTo(
      this IEdmTemporalTypeReference thisType,
      IEdmTemporalTypeReference otherType)
    {
      if (thisType == null || otherType == null || thisType.TypeKind() != otherType.TypeKind() || thisType.IsNullable != otherType.IsNullable)
        return false;
      int? precision1 = thisType.Precision;
      int? precision2 = otherType.Precision;
      return precision1.GetValueOrDefault() == precision2.GetValueOrDefault() & precision1.HasValue == precision2.HasValue;
    }

    private static bool IsEquivalentTo(
      this IEdmStringTypeReference thisType,
      IEdmStringTypeReference otherType)
    {
      if (thisType != null && otherType != null && thisType.IsNullable == otherType.IsNullable && thisType.IsUnbounded == otherType.IsUnbounded)
      {
        int? maxLength1 = thisType.MaxLength;
        int? maxLength2 = otherType.MaxLength;
        if (maxLength1.GetValueOrDefault() == maxLength2.GetValueOrDefault() & maxLength1.HasValue == maxLength2.HasValue)
        {
          bool? isUnicode1 = thisType.IsUnicode;
          bool? isUnicode2 = otherType.IsUnicode;
          return isUnicode1.GetValueOrDefault() == isUnicode2.GetValueOrDefault() & isUnicode1.HasValue == isUnicode2.HasValue;
        }
      }
      return false;
    }

    private static bool IsEquivalentTo(
      this IEdmSpatialTypeReference thisType,
      IEdmSpatialTypeReference otherType)
    {
      if (thisType == null || otherType == null || thisType.IsNullable != otherType.IsNullable)
        return false;
      int? referenceIdentifier1 = thisType.SpatialReferenceIdentifier;
      int? referenceIdentifier2 = otherType.SpatialReferenceIdentifier;
      return referenceIdentifier1.GetValueOrDefault() == referenceIdentifier2.GetValueOrDefault() & referenceIdentifier1.HasValue == referenceIdentifier2.HasValue;
    }
  }
}
