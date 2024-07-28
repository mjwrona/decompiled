// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ReaderValidationUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using System;
using System.Text;

namespace Microsoft.OData
{
  internal static class ReaderValidationUtils
  {
    internal static void ValidateMessageReaderSettings(
      ODataMessageReaderSettings messageReaderSettings,
      bool readingResponse)
    {
      if (messageReaderSettings.BaseUri != (Uri) null && !messageReaderSettings.BaseUri.IsAbsoluteUri)
        throw new ODataException(Strings.ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute((object) UriUtils.UriToString(messageReaderSettings.BaseUri)));
    }

    internal static void ValidateEntityReferenceLink(ODataEntityReferenceLink link)
    {
      if (link.Url == (Uri) null)
        throw new ODataException(Strings.ReaderValidationUtils_EntityReferenceLinkMissingUri);
    }

    internal static void ValidateStreamReferenceProperty(
      IODataStreamReferenceInfo streamInfo,
      string propertyName,
      IEdmStructuredType structuredType,
      IEdmProperty streamEdmProperty,
      bool throwOnUndeclaredLinkProperty)
    {
      ValidationUtils.ValidateStreamPropertyInfo(streamInfo, streamEdmProperty, propertyName);
      if (structuredType != null && structuredType.IsOpen && streamEdmProperty == null & throwOnUndeclaredLinkProperty)
        throw new ODataException(Strings.ValidationUtils_OpenStreamProperty((object) propertyName));
    }

    internal static void ValidateNullValue(
      IEdmTypeReference expectedTypeReference,
      bool enablePrimitiveTypeConversion,
      bool validateNullValue,
      string propertyName,
      bool? isDynamicProperty)
    {
      if (expectedTypeReference == null || !enablePrimitiveTypeConversion && expectedTypeReference.TypeKind() == EdmTypeKind.Primitive)
        return;
      ReaderValidationUtils.ValidateNullValueAllowed(expectedTypeReference, validateNullValue, propertyName, isDynamicProperty);
    }

    internal static IEdmProperty ValidatePropertyDefined(
      string propertyName,
      IEdmStructuredType owningStructuredType,
      bool throwOnUndeclaredPropertyForNonOpenType)
    {
      if (owningStructuredType == null)
        return (IEdmProperty) null;
      IEdmProperty property = owningStructuredType.FindProperty(propertyName);
      if (property == null && !owningStructuredType.IsOpen && throwOnUndeclaredPropertyForNonOpenType)
        throw new ODataException(Strings.ValidationUtils_PropertyDoesNotExistOnType((object) propertyName, (object) owningStructuredType.FullTypeName()));
      return property;
    }

    internal static ODataException GetPrimitiveTypeConversionException(
      IEdmPrimitiveTypeReference targetTypeReference,
      Exception innerException,
      string stringValue)
    {
      return new ODataException(Strings.ReaderValidationUtils_CannotConvertPrimitiveValue((object) stringValue, (object) targetTypeReference.FullName()), innerException);
    }

    internal static IEdmType ResolvePayloadTypeName(
      IEdmModel model,
      IEdmTypeReference expectedTypeReference,
      string payloadTypeName,
      EdmTypeKind expectedTypeKind,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      out EdmTypeKind payloadTypeKind)
    {
      switch (payloadTypeName)
      {
        case null:
          payloadTypeKind = EdmTypeKind.None;
          return (IEdmType) null;
        case "":
          payloadTypeKind = expectedTypeKind;
          return (IEdmType) null;
        default:
          IEdmType edmType = MetadataUtils.ResolveTypeNameForRead(model, expectedTypeReference == null ? (IEdmType) null : expectedTypeReference.Definition, payloadTypeName, clientCustomTypeResolver, out payloadTypeKind);
          if (payloadTypeKind == EdmTypeKind.None)
            payloadTypeKind = expectedTypeKind;
          return edmType;
      }
    }

    internal static IEdmTypeReference ResolvePayloadTypeNameAndComputeTargetType(
      EdmTypeKind expectedTypeKind,
      bool? expectStructuredType,
      IEdmType defaultPrimitivePayloadType,
      IEdmTypeReference expectedTypeReference,
      string payloadTypeName,
      IEdmModel model,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool throwIfTypeConflictsWithMetadata,
      bool enablePrimitiveTypeConversion,
      Func<EdmTypeKind> typeKindFromPayloadFunc,
      out EdmTypeKind targetTypeKind,
      out ODataTypeAnnotation typeAnnotation)
    {
      typeAnnotation = (ODataTypeAnnotation) null;
      EdmTypeKind payloadTypeKind;
      IEdmType payloadType = ReaderValidationUtils.ResolvePayloadTypeName(model, expectedTypeReference, payloadTypeName, EdmTypeKind.Complex, clientCustomTypeResolver, out payloadTypeKind);
      bool? nullable1 = expectStructuredType;
      bool flag1 = true;
      bool forResource = nullable1.GetValueOrDefault() == flag1 & nullable1.HasValue || !expectStructuredType.HasValue && payloadTypeKind.IsStructured();
      targetTypeKind = ReaderValidationUtils.ComputeTargetTypeKind(expectedTypeReference, forResource, payloadTypeName, payloadTypeKind, clientCustomTypeResolver, throwIfTypeConflictsWithMetadata, enablePrimitiveTypeConversion, typeKindFromPayloadFunc);
      IEdmTypeReference targetTypeReference;
      if (targetTypeKind == EdmTypeKind.Primitive)
      {
        targetTypeReference = ReaderValidationUtils.ResolveAndValidatePrimitiveTargetType(expectedTypeReference, payloadTypeKind, payloadType, payloadTypeName, defaultPrimitivePayloadType, model, clientCustomTypeResolver, enablePrimitiveTypeConversion, throwIfTypeConflictsWithMetadata);
      }
      else
      {
        targetTypeReference = ReaderValidationUtils.ResolveAndValidateNonPrimitiveTargetType(targetTypeKind, expectedTypeReference, payloadTypeKind, payloadType, payloadTypeName, model, clientCustomTypeResolver, throwIfTypeConflictsWithMetadata);
        if (targetTypeReference != null)
          typeAnnotation = ReaderValidationUtils.CreateODataTypeAnnotation(payloadTypeName, payloadType, targetTypeReference);
      }
      if (expectedTypeKind == EdmTypeKind.None)
      {
        if (targetTypeKind != EdmTypeKind.Untyped)
        {
          bool? nullable2 = expectStructuredType;
          bool flag2 = true;
          if (!(nullable2.GetValueOrDefault() == flag2 & nullable2.HasValue))
            goto label_9;
        }
        else
          goto label_9;
      }
      if (targetTypeReference != null)
        ValidationUtils.ValidateTypeKind(targetTypeKind, expectedTypeKind, new bool?(forResource), payloadTypeName);
label_9:
      return targetTypeReference;
    }

    internal static IEdmTypeReference ResolveAndValidatePrimitiveTargetType(
      IEdmTypeReference expectedTypeReference,
      EdmTypeKind payloadTypeKind,
      IEdmType payloadType,
      string payloadTypeName,
      IEdmType defaultPayloadType,
      IEdmModel model,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool enablePrimitiveTypeConversion,
      bool throwIfTypeConflictsWithMetadata)
    {
      bool flag = clientCustomTypeResolver != null && payloadType != null;
      if (payloadTypeKind != EdmTypeKind.None && !enablePrimitiveTypeConversion | throwIfTypeConflictsWithMetadata)
        ValidationUtils.ValidateTypeKind(payloadTypeKind, EdmTypeKind.Primitive, new bool?(), payloadTypeName);
      if (!model.IsUserModel() || expectedTypeReference == null | flag || !enablePrimitiveTypeConversion)
        return MetadataUtils.GetNullablePayloadTypeReference(payloadType ?? defaultPayloadType);
      if (!throwIfTypeConflictsWithMetadata || payloadType == null)
        return expectedTypeReference;
      if (!MetadataUtilsCommon.CanConvertPrimitiveTypeTo((SingleValueNode) null, (IEdmPrimitiveType) payloadType.AsActualType(), (IEdmPrimitiveType) expectedTypeReference.Definition))
        throw new ODataException(Strings.ValidationUtils_IncompatibleType((object) payloadTypeName, (object) expectedTypeReference.FullName()));
      return expectedTypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.PrimitiveType ? payloadType.ToTypeReference(expectedTypeReference.IsNullable) : expectedTypeReference;
    }

    internal static IEdmTypeReference ResolveAndValidateNonPrimitiveTargetType(
      EdmTypeKind expectedTypeKind,
      IEdmTypeReference expectedTypeReference,
      EdmTypeKind payloadTypeKind,
      IEdmType payloadType,
      string payloadTypeName,
      IEdmModel model,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool throwIfTypeConflictsWithMetadata)
    {
      bool flag = clientCustomTypeResolver != null && payloadType != null;
      if (!flag && model.IsUserModel() && expectedTypeReference == null | throwIfTypeConflictsWithMetadata)
        ReaderValidationUtils.VerifyPayloadTypeDefined(payloadTypeName, payloadType);
      if (payloadTypeKind != EdmTypeKind.None && (throwIfTypeConflictsWithMetadata || expectedTypeReference == null))
        ValidationUtils.ValidateTypeKind(payloadTypeKind, expectedTypeKind, new bool?(), payloadTypeName);
      if (!model.IsUserModel())
        return (IEdmTypeReference) null;
      if (expectedTypeReference == null | flag)
        return ReaderValidationUtils.ResolveAndValidateTargetTypeWithNoExpectedType(expectedTypeKind, payloadType);
      return !throwIfTypeConflictsWithMetadata ? ReaderValidationUtils.ResolveAndValidateTargetTypeStrictValidationDisabled(expectedTypeKind, expectedTypeReference, payloadType) : ReaderValidationUtils.ResolveAndValidateTargetTypeStrictValidationEnabled(expectedTypeKind, expectedTypeReference, payloadType);
    }

    internal static void ValidateEncodingSupportedInBatch(Encoding encoding)
    {
      if (string.CompareOrdinal(Encoding.UTF8.WebName, encoding.WebName) != 0)
        throw new ODataException(Strings.ODataBatchReaderStream_MultiByteEncodingsNotSupported((object) encoding.WebName));
    }

    internal static void ValidateEncodingSupportedInAsync(Encoding encoding)
    {
      if (string.CompareOrdinal(Encoding.UTF8.WebName, encoding.WebName) != 0)
        throw new ODataException(Strings.ODataAsyncReader_MultiByteEncodingsNotSupported((object) encoding.WebName));
    }

    internal static void ValidateResourceSetOrResourceContextUri(
      ODataJsonLightContextUriParseResult contextUriParseResult,
      ODataReaderCore.Scope scope,
      bool updateScope)
    {
      if (contextUriParseResult.EdmType is IEdmCollectionType)
      {
        ReaderValidationUtils.ValidateResourceSetContextUri(contextUriParseResult, scope, updateScope);
      }
      else
      {
        if (scope.NavigationSource == null)
        {
          if (updateScope)
            scope.NavigationSource = contextUriParseResult.NavigationSource;
        }
        else if (contextUriParseResult.NavigationSource != null && string.CompareOrdinal(scope.NavigationSource.FullNavigationSourceName(), contextUriParseResult.NavigationSource.FullNavigationSourceName()) != 0)
          throw new ODataException(Strings.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet((object) UriUtils.UriToString(contextUriParseResult.ContextUri), (object) contextUriParseResult.NavigationSource.FullNavigationSourceName(), (object) scope.NavigationSource.FullNavigationSourceName()));
        IEdmStructuredType edmType = (IEdmStructuredType) contextUriParseResult.EdmType;
        if (edmType == null)
          return;
        if (scope.ResourceType == null)
        {
          if (!updateScope)
            return;
          scope.ResourceTypeReference = (IEdmTypeReference) edmType.ToTypeReference(true).AsStructured();
        }
        else if (scope.ResourceType.IsAssignableFrom((IEdmType) edmType))
        {
          if (!updateScope)
            return;
          scope.ResourceTypeReference = (IEdmTypeReference) edmType.ToTypeReference(true).AsStructured();
        }
        else if (!edmType.IsAssignableFrom(scope.ResourceType))
          throw new ODataException(Strings.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntityType((object) UriUtils.UriToString(contextUriParseResult.ContextUri), (object) contextUriParseResult.EdmType.FullTypeName(), (object) scope.ResourceType.FullTypeName()));
      }
    }

    internal static IEdmTypeReference ValidateCollectionContextUriAndGetPayloadItemTypeReference(
      ODataJsonLightContextUriParseResult contextUriParseResult,
      IEdmTypeReference expectedItemTypeReference)
    {
      if (contextUriParseResult == null || contextUriParseResult.EdmType == null)
        return expectedItemTypeReference;
      if (contextUriParseResult.EdmType is IEdmCollectionType)
      {
        IEdmCollectionType edmType = (IEdmCollectionType) contextUriParseResult.EdmType;
        if (expectedItemTypeReference != null && !expectedItemTypeReference.IsAssignableFrom(edmType.ElementType))
          throw new ODataException(Strings.ReaderValidationUtils_ContextUriDoesNotReferTypeAssignableToExpectedType((object) UriUtils.UriToString(contextUriParseResult.ContextUri), (object) edmType.ElementType.FullName(), (object) expectedItemTypeReference.FullName()));
        return edmType.ElementType;
      }
      if (expectedItemTypeReference != null && !expectedItemTypeReference.Definition.IsAssignableFrom(contextUriParseResult.EdmType))
        throw new ODataException(Strings.ReaderValidationUtils_ContextUriDoesNotReferTypeAssignableToExpectedType((object) UriUtils.UriToString(contextUriParseResult.ContextUri), (object) contextUriParseResult.EdmType.FullTypeName(), (object) expectedItemTypeReference.Definition.FullTypeName()));
      return contextUriParseResult.EdmType.ToTypeReference(true);
    }

    internal static void ValidateOperationProperty(
      object propertyValue,
      string propertyName,
      string metadata,
      string operationsHeader)
    {
      if (propertyValue == null)
        throw new ODataException(Strings.ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull((object) propertyName, (object) metadata, (object) operationsHeader));
    }

    private static IEdmTypeReference ResolveAndValidateTargetTypeWithNoExpectedType(
      EdmTypeKind expectedTypeKind,
      IEdmType payloadType)
    {
      if (payloadType != null)
        return payloadType.ToTypeReference(true);
      if (expectedTypeKind == EdmTypeKind.Entity)
        throw new ODataException(Strings.ReaderValidationUtils_ResourceWithoutType);
      return (IEdmTypeReference) null;
    }

    private static IEdmTypeReference ResolveAndValidateTargetTypeStrictValidationDisabled(
      EdmTypeKind expectedTypeKind,
      IEdmTypeReference expectedTypeReference,
      IEdmType payloadType)
    {
      switch (expectedTypeKind)
      {
        case EdmTypeKind.Entity:
          if (payloadType != null && expectedTypeKind == payloadType.TypeKind && EdmLibraryExtensions.IsAssignableFrom(expectedTypeReference.AsEntity().EntityDefinition(), (IEdmStructuredType) payloadType))
            return payloadType.ToTypeReference(true);
          goto case EdmTypeKind.Enum;
        case EdmTypeKind.Complex:
          if (payloadType != null && expectedTypeKind == payloadType.TypeKind && EdmLibraryExtensions.IsAssignableFrom(expectedTypeReference.AsComplex().ComplexDefinition(), (IEdmStructuredType) payloadType))
            return payloadType.ToTypeReference(true);
          goto case EdmTypeKind.Enum;
        case EdmTypeKind.Collection:
          if (payloadType != null && expectedTypeKind == payloadType.TypeKind)
          {
            ReaderValidationUtils.VerifyCollectionComplexItemType(expectedTypeReference, payloadType);
            goto case EdmTypeKind.Enum;
          }
          else
            goto case EdmTypeKind.Enum;
        case EdmTypeKind.Enum:
        case EdmTypeKind.TypeDefinition:
        case EdmTypeKind.Untyped:
          return expectedTypeReference;
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind));
      }
    }

    private static IEdmTypeReference ResolveAndValidateTargetTypeStrictValidationEnabled(
      EdmTypeKind expectedTypeKind,
      IEdmTypeReference expectedTypeReference,
      IEdmType payloadType)
    {
      switch (expectedTypeKind)
      {
        case EdmTypeKind.Entity:
          if (payloadType != null)
          {
            IEdmTypeReference typeReference = payloadType.ToTypeReference(true);
            ValidationUtils.ValidateEntityTypeIsAssignable((IEdmEntityTypeReference) expectedTypeReference, (IEdmEntityTypeReference) typeReference);
            return typeReference;
          }
          goto case EdmTypeKind.Untyped;
        case EdmTypeKind.Complex:
          if (payloadType != null)
          {
            ReaderValidationUtils.VerifyComplexType(expectedTypeReference, payloadType, true);
            return payloadType.ToTypeReference(true);
          }
          goto case EdmTypeKind.Untyped;
        case EdmTypeKind.Collection:
          if (payloadType != null && !payloadType.IsElementTypeEquivalentTo(expectedTypeReference.Definition))
          {
            ReaderValidationUtils.VerifyCollectionComplexItemType(expectedTypeReference, payloadType);
            throw new ODataException(Strings.ValidationUtils_IncompatibleType((object) payloadType.FullTypeName(), (object) expectedTypeReference.FullName()));
          }
          goto case EdmTypeKind.Untyped;
        case EdmTypeKind.Enum:
          if (payloadType != null && string.CompareOrdinal(payloadType.FullTypeName(), expectedTypeReference.FullName()) != 0)
            throw new ODataException(Strings.ValidationUtils_IncompatibleType((object) payloadType.FullTypeName(), (object) expectedTypeReference.FullName()));
          goto case EdmTypeKind.Untyped;
        case EdmTypeKind.TypeDefinition:
          if (payloadType != null && !expectedTypeReference.Definition.IsAssignableFrom(payloadType))
            throw new ODataException(Strings.ValidationUtils_IncompatibleType((object) payloadType.FullTypeName(), (object) expectedTypeReference.FullName()));
          goto case EdmTypeKind.Untyped;
        case EdmTypeKind.Untyped:
          return expectedTypeReference;
        default:
          throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind));
      }
    }

    private static void VerifyPayloadTypeDefined(string payloadTypeName, IEdmType payloadType)
    {
      if (payloadTypeName != null && payloadType == null)
        throw new ODataException(Strings.ValidationUtils_UnrecognizedTypeName((object) payloadTypeName));
    }

    private static void VerifyComplexType(
      IEdmTypeReference expectedTypeReference,
      IEdmType payloadType,
      bool failIfNotRelated)
    {
      IEdmStructuredType edmStructuredType1 = expectedTypeReference.AsStructured().StructuredDefinition();
      IEdmStructuredType edmStructuredType2 = (IEdmStructuredType) payloadType;
      if (!EdmLibraryExtensions.IsAssignableFrom(edmStructuredType1, edmStructuredType2) && failIfNotRelated)
        throw new ODataException(Strings.ValidationUtils_IncompatibleType((object) edmStructuredType2.FullTypeName(), (object) edmStructuredType1.FullTypeName()));
    }

    private static void VerifyCollectionComplexItemType(
      IEdmTypeReference expectedTypeReference,
      IEdmType payloadType)
    {
      IEdmTypeReference collectionItemType1 = ValidationUtils.ValidateCollectionType(expectedTypeReference).GetCollectionItemType();
      if (collectionItemType1 == null || !collectionItemType1.IsODataComplexTypeKind())
        return;
      IEdmTypeReference collectionItemType2 = ValidationUtils.ValidateCollectionType(payloadType.ToTypeReference()).GetCollectionItemType();
      if (collectionItemType2 == null || !collectionItemType2.IsODataComplexTypeKind())
        return;
      ReaderValidationUtils.VerifyComplexType(collectionItemType1, collectionItemType2.Definition, false);
    }

    private static ODataTypeAnnotation CreateODataTypeAnnotation(
      string payloadTypeName,
      IEdmType payloadType,
      IEdmTypeReference targetTypeReference)
    {
      if (payloadType != null && !payloadType.IsEquivalentTo(targetTypeReference.Definition))
        return new ODataTypeAnnotation(payloadTypeName, payloadType);
      return payloadType == null ? new ODataTypeAnnotation() : (ODataTypeAnnotation) null;
    }

    private static EdmTypeKind ComputeTargetTypeKind(
      IEdmTypeReference expectedTypeReference,
      bool forResource,
      string payloadTypeName,
      EdmTypeKind payloadTypeKind,
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool throwIfTypeConflictsWithMetadata,
      bool enablePrimitiveTypeConversion,
      Func<EdmTypeKind> typeKindFromPayloadFunc)
    {
      bool flag = clientCustomTypeResolver != null && payloadTypeKind != 0;
      EdmTypeKind edmTypeKind = EdmTypeKind.None;
      if (!flag)
        edmTypeKind = ReaderUtils.GetExpectedTypeKind(expectedTypeReference, enablePrimitiveTypeConversion);
      EdmTypeKind actualTypeKind;
      if (edmTypeKind != EdmTypeKind.None)
        actualTypeKind = edmTypeKind;
      else if (payloadTypeKind != EdmTypeKind.None)
      {
        if (!forResource)
          ValidationUtils.ValidateValueTypeKind(payloadTypeKind, payloadTypeName);
        actualTypeKind = payloadTypeKind;
      }
      else
        actualTypeKind = typeKindFromPayloadFunc();
      if (ReaderValidationUtils.ShouldValidatePayloadTypeKind(clientCustomTypeResolver, throwIfTypeConflictsWithMetadata, enablePrimitiveTypeConversion, expectedTypeReference, payloadTypeKind))
        ValidationUtils.ValidateTypeKind(actualTypeKind, expectedTypeReference.TypeKind(), new bool?(), payloadTypeName);
      return actualTypeKind;
    }

    private static bool ShouldValidatePayloadTypeKind(
      Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
      bool throwIfTypeConflictsWithMetadata,
      bool enablePrimitiveTypeConversion,
      IEdmTypeReference expectedValueTypeReference,
      EdmTypeKind payloadTypeKind)
    {
      bool flag = clientCustomTypeResolver != null && payloadTypeKind != 0;
      if (expectedValueTypeReference == null)
        return false;
      if (throwIfTypeConflictsWithMetadata | flag)
        return true;
      return expectedValueTypeReference.IsODataPrimitiveTypeKind() && !enablePrimitiveTypeConversion;
    }

    private static void ValidateNullValueAllowed(
      IEdmTypeReference expectedValueTypeReference,
      bool validateNullValue,
      string propertyName,
      bool? isDynamicProperty)
    {
      if (!validateNullValue || expectedValueTypeReference == null)
        return;
      if (expectedValueTypeReference.IsODataPrimitiveTypeKind())
      {
        if (expectedValueTypeReference.IsNullable)
          return;
        ReaderValidationUtils.ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
      }
      else if (expectedValueTypeReference.IsODataEnumTypeKind())
      {
        if (expectedValueTypeReference.IsNullable)
          return;
        ReaderValidationUtils.ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
      }
      else if (expectedValueTypeReference.IsNonEntityCollectionType())
      {
        bool? nullable = isDynamicProperty;
        bool flag = true;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
          return;
        ReaderValidationUtils.ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
      }
      else if (expectedValueTypeReference.IsODataComplexTypeKind())
      {
        if (expectedValueTypeReference.AsComplex().IsNullable)
          return;
        ReaderValidationUtils.ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
      }
      else
      {
        if (!expectedValueTypeReference.IsUntyped() || expectedValueTypeReference.IsNullable)
          return;
        ReaderValidationUtils.ThrowNullValueForNonNullableTypeException(expectedValueTypeReference, propertyName);
      }
    }

    private static void ThrowNullValueForNonNullableTypeException(
      IEdmTypeReference expectedValueTypeReference,
      string propertyName)
    {
      if (string.IsNullOrEmpty(propertyName))
        throw new ODataException(Strings.ReaderValidationUtils_NullValueForNonNullableType((object) expectedValueTypeReference.FullName()));
      throw new ODataException(Strings.ReaderValidationUtils_NullNamedValueForNonNullableType((object) propertyName, (object) expectedValueTypeReference.FullName()));
    }

    private static void ValidateResourceSetContextUri(
      ODataJsonLightContextUriParseResult contextUriParseResult,
      ODataReaderCore.Scope scope,
      bool updateScope)
    {
    }
  }
}
