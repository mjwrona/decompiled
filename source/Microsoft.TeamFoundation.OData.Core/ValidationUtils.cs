// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ValidationUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData
{
  internal static class ValidationUtils
  {
    internal static readonly char[] InvalidCharactersInPropertyNames = new char[3]
    {
      ':',
      '.',
      '@'
    };
    private const int MaxBoundaryLength = 70;

    internal static void ValidateOpenPropertyValue(string propertyName, object value)
    {
    }

    internal static void ValidateValueTypeKind(EdmTypeKind typeKind, string typeName)
    {
      if (typeKind != EdmTypeKind.Primitive && typeKind != EdmTypeKind.Enum && typeKind != EdmTypeKind.Collection && typeKind != EdmTypeKind.Untyped)
        throw new ODataException(Strings.ValidationUtils_IncorrectValueTypeKind((object) typeName, (object) typeKind.ToString()));
    }

    internal static string ValidateCollectionTypeName(string collectionTypeName) => EdmLibraryExtensions.GetCollectionItemTypeName(collectionTypeName) ?? throw new ODataException(Strings.ValidationUtils_InvalidCollectionTypeName((object) collectionTypeName));

    internal static void ValidateEntityTypeIsAssignable(
      IEdmEntityTypeReference expectedEntityTypeReference,
      IEdmEntityTypeReference payloadEntityTypeReference)
    {
      if (!EdmLibraryExtensions.IsAssignableFrom(expectedEntityTypeReference.EntityDefinition(), (IEdmStructuredType) payloadEntityTypeReference.EntityDefinition()))
        throw new ODataException(Strings.ValidationUtils_ResourceTypeNotAssignableToExpectedType((object) payloadEntityTypeReference.FullName(), (object) expectedEntityTypeReference.FullName()));
    }

    internal static void ValidateComplexTypeIsAssignable(
      IEdmComplexType expectedComplexType,
      IEdmComplexType payloadComplexType)
    {
      if (!EdmLibraryExtensions.IsAssignableFrom(expectedComplexType, (IEdmStructuredType) payloadComplexType))
        throw new ODataException(Strings.ValidationUtils_IncompatibleType((object) payloadComplexType.FullTypeName(), (object) expectedComplexType.FullTypeName()));
    }

    internal static IEdmCollectionTypeReference ValidateCollectionType(
      IEdmTypeReference typeReference)
    {
      IEdmCollectionTypeReference collectionTypeReference = typeReference.AsCollectionOrNull();
      return collectionTypeReference == null || typeReference.IsNonEntityCollectionType() ? collectionTypeReference : throw new ODataException(Strings.ValidationUtils_InvalidCollectionTypeReference((object) typeReference.TypeKind()));
    }

    internal static void ValidateCollectionItem(object item, bool isNullable)
    {
      if (!isNullable && item == null)
        throw new ODataException(Strings.ValidationUtils_NonNullableCollectionElementsMustNotBeNull);
      switch (item)
      {
        case ODataCollectionValue _:
          throw new ODataException(Strings.ValidationUtils_NestedCollectionsAreNotSupported);
        case ODataStreamReferenceValue _:
          throw new ODataException(Strings.ValidationUtils_StreamReferenceValuesNotSupportedInCollections);
      }
    }

    internal static void ValidateNullCollectionItem(IEdmTypeReference expectedItemType)
    {
      if (expectedItemType != null && expectedItemType.IsODataPrimitiveTypeKind() && !expectedItemType.IsNullable)
        throw new ODataException(Strings.ValidationUtils_NullCollectionItemForNonNullableType((object) expectedItemType.FullName()));
    }

    internal static void ValidateStreamPropertyInfo(
      IODataStreamReferenceInfo streamInfo,
      IEdmProperty edmProperty,
      string propertyName)
    {
      if (edmProperty != null && !edmProperty.Type.IsStream())
        throw new ODataException(Strings.ValidationUtils_MismatchPropertyKindForStreamProperty((object) propertyName));
    }

    internal static void IncreaseAndValidateRecursionDepth(ref int recursionDepth, int maxDepth)
    {
      ++recursionDepth;
      if (recursionDepth > maxDepth)
        throw new ODataException(Strings.ValidationUtils_RecursionDepthLimitReached((object) maxDepth));
    }

    internal static void ValidateOperationNotNull(ODataOperation operation, bool isAction)
    {
      if (operation == null)
        throw new ODataException(Strings.ValidationUtils_EnumerableContainsANullItem(isAction ? (object) "ODataResource.Actions" : (object) "ODataResource.Functions"));
    }

    internal static void ValidateOperationMetadataNotNull(ODataOperation operation)
    {
      if (operation.Metadata == (Uri) null)
        throw new ODataException(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata((object) operation.GetType().Name));
    }

    internal static void ValidateOperationTargetNotNull(ODataOperation operation)
    {
      if (operation.Target == (Uri) null)
        throw new ODataException(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyTarget((object) operation.GetType().Name));
    }

    internal static void ValidateMediaResource(
      ODataResourceBase resource,
      IEdmEntityType resourceType)
    {
      if (resourceType == null)
        return;
      if (resource.MediaResource == null)
      {
        if (resourceType.HasStream)
          throw new ODataException(Strings.ValidationUtils_ResourceWithoutMediaResourceAndMLEType((object) resourceType.FullTypeName()));
      }
      else if (!resourceType.HasStream)
        throw new ODataException(Strings.ValidationUtils_ResourceWithMediaResourceAndNonMLEType((object) resourceType.FullTypeName()));
    }

    internal static void ValidateIsExpectedPrimitiveType(
      object value,
      IEdmTypeReference expectedTypeReference)
    {
      IEdmPrimitiveTypeReference primitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());
      ValidationUtils.ValidateIsExpectedPrimitiveType(value, primitiveTypeReference, expectedTypeReference);
    }

    internal static void ValidateIsExpectedPrimitiveType(
      object value,
      IEdmPrimitiveTypeReference valuePrimitiveTypeReference,
      IEdmTypeReference expectedTypeReference)
    {
      if (valuePrimitiveTypeReference == null)
        throw new ODataException(Strings.ValidationUtils_UnsupportedPrimitiveType((object) value.GetType().FullName));
      if (!expectedTypeReference.IsODataPrimitiveTypeKind() && !expectedTypeReference.IsODataTypeDefinitionTypeKind())
        throw new ODataException(Strings.ValidationUtils_NonPrimitiveTypeForPrimitiveValue((object) expectedTypeReference.FullName()));
      ValidationUtils.ValidateMetadataPrimitiveType(expectedTypeReference, (IEdmTypeReference) valuePrimitiveTypeReference);
    }

    internal static void ValidateMetadataPrimitiveType(
      IEdmTypeReference expectedTypeReference,
      IEdmTypeReference typeReferenceFromValue)
    {
      IEdmType definition1 = expectedTypeReference.Definition;
      IEdmPrimitiveType definition2 = (IEdmPrimitiveType) typeReferenceFromValue.Definition;
      bool flag1 = expectedTypeReference.IsNullable == typeReferenceFromValue.IsNullable || expectedTypeReference.IsNullable && !typeReferenceFromValue.IsNullable || !typeReferenceFromValue.IsODataValueType();
      bool flag2 = definition1.IsAssignableFrom((IEdmType) definition2);
      if (!flag1 || !flag2)
        throw new ODataException(Strings.ValidationUtils_IncompatiblePrimitiveItemType((object) typeReferenceFromValue.FullName(), (object) typeReferenceFromValue.IsNullable, (object) expectedTypeReference.FullName(), (object) expectedTypeReference.IsNullable));
    }

    internal static void ValidateServiceDocumentElement(
      ODataServiceDocumentElement serviceDocumentElement,
      ODataFormat format)
    {
      if (serviceDocumentElement == null)
        throw new ODataException(Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
      if (serviceDocumentElement.Url == (Uri) null)
        throw new ODataException(Strings.ValidationUtils_ResourceMustSpecifyUrl);
      if (format == ODataFormat.Json && string.IsNullOrEmpty(serviceDocumentElement.Name))
        throw new ODataException(Strings.ValidationUtils_ResourceMustSpecifyName((object) serviceDocumentElement.Url.ToString()));
    }

    internal static void ValidateServiceDocumentElementUrl(string serviceDocumentUrl)
    {
      if (serviceDocumentUrl == null)
        throw new ODataException(Strings.ValidationUtils_ServiceDocumentElementUrlMustNotBeNull);
    }

    internal static void ValidateTypeKind(
      EdmTypeKind actualTypeKind,
      EdmTypeKind expectedTypeKind,
      bool? expectStructuredType,
      string typeName)
    {
      if (expectStructuredType.HasValue && expectStructuredType.Value && (expectedTypeKind.IsStructured() || expectedTypeKind == EdmTypeKind.None) && actualTypeKind.IsStructured() || expectedTypeKind == actualTypeKind)
        return;
      if (typeName == null)
        throw new ODataException(Strings.ValidationUtils_IncorrectTypeKindNoTypeName((object) actualTypeKind.ToString(), (object) expectedTypeKind.ToString()));
      if ((actualTypeKind != EdmTypeKind.TypeDefinition || expectedTypeKind != EdmTypeKind.Primitive) && (actualTypeKind != EdmTypeKind.Primitive || expectedTypeKind != EdmTypeKind.TypeDefinition) && (actualTypeKind != EdmTypeKind.Primitive || expectedTypeKind != EdmTypeKind.None))
        throw new ODataException(Strings.ValidationUtils_IncorrectTypeKind((object) typeName, (object) expectedTypeKind.ToString(), (object) actualTypeKind.ToString()));
    }

    internal static void ValidateBoundaryString(string boundary)
    {
      if (boundary == null || boundary.Length == 0 || boundary.Length > 70)
        throw new ODataException(Strings.ValidationUtils_InvalidBatchBoundaryDelimiterLength((object) boundary, (object) 70));
    }

    internal static bool IsValidPropertyName(string propertyName) => propertyName.IndexOfAny(ValidationUtils.InvalidCharactersInPropertyNames) < 0;

    internal static void ValidatePropertyName(string propertyName)
    {
      if (!ValidationUtils.IsValidPropertyName(propertyName))
      {
        string p1 = string.Join(", ", ((IEnumerable<char>) ValidationUtils.InvalidCharactersInPropertyNames).Select<char, string>((Func<char, string>) (c => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", new object[1]
        {
          (object) c
        }))).ToArray<string>());
        throw new ODataException(Strings.ValidationUtils_PropertiesMustNotContainReservedChars((object) propertyName, (object) p1));
      }
    }
  }
}
