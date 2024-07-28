// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.WriterValidationUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData
{
  internal static class WriterValidationUtils
  {
    internal static void ValidateMessageWriterSettings(
      ODataMessageWriterSettings messageWriterSettings,
      bool writingResponse)
    {
      if (messageWriterSettings.BaseUri != (Uri) null && !messageWriterSettings.BaseUri.IsAbsoluteUri)
        throw new ODataException(Strings.WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute((object) UriUtils.UriToString(messageWriterSettings.BaseUri)));
      if (messageWriterSettings.HasJsonPaddingFunction() && !writingResponse)
        throw new ODataException(Strings.WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage);
    }

    internal static void ValidatePropertyNotNull(ODataPropertyInfo property)
    {
      if (property == null)
        throw new ODataException(Strings.WriterValidationUtils_PropertyMustNotBeNull);
    }

    internal static void ValidatePropertyName(string propertyName)
    {
      if (string.IsNullOrEmpty(propertyName))
        throw new ODataException(Strings.WriterValidationUtils_PropertiesMustHaveNonEmptyName);
      ValidationUtils.ValidatePropertyName(propertyName);
    }

    internal static IEdmProperty ValidatePropertyDefined(
      string propertyName,
      IEdmStructuredType owningStructuredType,
      bool throwOnUndeclaredProperty)
    {
      if (owningStructuredType == null)
        return (IEdmProperty) null;
      IEdmProperty property = owningStructuredType.FindProperty(propertyName);
      if (throwOnUndeclaredProperty && !owningStructuredType.IsOpen && property == null)
        throw new ODataException(Strings.ValidationUtils_PropertyDoesNotExistOnType((object) propertyName, (object) owningStructuredType.FullTypeName()));
      return property;
    }

    internal static void ValidatePropertyDefined(
      PropertySerializationInfo propertyInfo,
      bool throwOnUndeclaredProperty)
    {
      if (propertyInfo.MetadataType.OwningType != null && throwOnUndeclaredProperty && propertyInfo.MetadataType.IsUndeclaredProperty && !propertyInfo.MetadataType.IsOpenProperty)
        throw new ODataException(Strings.ValidationUtils_PropertyDoesNotExistOnType((object) propertyInfo.PropertyName, (object) propertyInfo.MetadataType.OwningType.FullTypeName()));
    }

    internal static IEdmNavigationProperty ValidateNavigationPropertyDefined(
      string propertyName,
      IEdmStructuredType owningType,
      bool throwOnUndeclaredProperty)
    {
      if (owningType == null)
        return (IEdmNavigationProperty) null;
      IEdmProperty edmProperty = WriterValidationUtils.ValidatePropertyDefined(propertyName, owningType, throwOnUndeclaredProperty);
      if (edmProperty == null)
        return (IEdmNavigationProperty) null;
      return edmProperty.PropertyKind == EdmPropertyKind.Navigation ? (IEdmNavigationProperty) edmProperty : throw new ODataException(Strings.ValidationUtils_NavigationPropertyExpected((object) propertyName, (object) owningType.FullTypeName(), (object) edmProperty.PropertyKind.ToString()));
    }

    internal static void ValidateNestedResource(
      IEdmStructuredType resourceType,
      IEdmStructuredType parentNavigationPropertyType)
    {
      if (parentNavigationPropertyType != null && !EdmLibraryExtensions.IsAssignableFrom(parentNavigationPropertyType, resourceType))
        throw new ODataException(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType((object) resourceType.FullTypeName(), (object) parentNavigationPropertyType.FullTypeName()));
    }

    internal static void ValidateCanWriteOperation(ODataOperation operation, bool writingResponse)
    {
      if (!writingResponse)
        throw new ODataException(Strings.WriterValidationUtils_OperationInRequest((object) operation.Metadata));
    }

    internal static void ValidateResourceSetAtEnd(ODataResourceSet resourceSet, bool writingRequest)
    {
      if (resourceSet.NextPageLink != (Uri) null && writingRequest)
        throw new ODataException(Strings.WriterValidationUtils_NextPageLinkInRequest);
    }

    internal static void ValidateDeltaResourceSetAtEnd(
      ODataDeltaResourceSet resourceSet,
      bool writingRequest)
    {
      if (resourceSet.NextPageLink != (Uri) null && writingRequest)
        throw new ODataException(Strings.WriterValidationUtils_NextPageLinkInRequest);
    }

    internal static void ValidateResourceAtStart(ODataResourceBase resource) => WriterValidationUtils.ValidateResourceId(resource.Id);

    internal static void ValidateResourceAtEnd(ODataResourceBase resource) => WriterValidationUtils.ValidateResourceId(resource.Id);

    internal static void ValidateStreamReferenceValue(
      ODataStreamReferenceValue streamReference,
      bool isDefaultStream)
    {
      if (streamReference.ContentType != null && streamReference.ContentType.Length == 0)
        throw new ODataException(Strings.WriterValidationUtils_StreamReferenceValueEmptyContentType);
      if (isDefaultStream && streamReference.ReadLink == (Uri) null && streamReference.ContentType != null)
        throw new ODataException(Strings.WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink);
      if (isDefaultStream && streamReference.ReadLink != (Uri) null && streamReference.ContentType == null)
        throw new ODataException(Strings.WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType);
      if (streamReference.EditLink == (Uri) null && streamReference.ReadLink == (Uri) null && !isDefaultStream)
        throw new ODataException(Strings.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink);
      if (streamReference.EditLink == (Uri) null && streamReference.ETag != null)
        throw new ODataException(Strings.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag);
    }

    internal static void ValidateStreamPropertyInfo(
      IODataStreamReferenceInfo streamPropertyInfo,
      IEdmProperty edmProperty,
      string propertyName,
      bool writingResponse)
    {
      ValidationUtils.ValidateStreamPropertyInfo(streamPropertyInfo, edmProperty, propertyName);
      if (!writingResponse && (streamPropertyInfo != null && streamPropertyInfo.EditLink != (Uri) null || streamPropertyInfo.ReadLink != (Uri) null || streamPropertyInfo.ETag != null))
        throw new ODataException(Strings.WriterValidationUtils_StreamPropertyInRequest((object) propertyName));
    }

    internal static void ValidatePropertyDerivedTypeConstraint(
      PropertySerializationInfo propertySerializationInfo)
    {
      if (propertySerializationInfo.MetadataType.IsUndeclaredProperty)
        return;
      PropertyValueTypeInfo valueType = propertySerializationInfo.ValueType;
      if (valueType == null || valueType.TypeReference == null || propertySerializationInfo.MetadataType.TypeReference.Definition == valueType.TypeReference.Definition)
        return;
      string fullTypeName = valueType.TypeReference.FullName();
      if (propertySerializationInfo.MetadataType.DerivedTypeConstraints != null && !propertySerializationInfo.MetadataType.DerivedTypeConstraints.Any<string>((Func<string, bool>) (d => d == fullTypeName)))
        throw new ODataException(Strings.WriterValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint((object) fullTypeName, (object) "property", (object) propertySerializationInfo.PropertyName));
    }

    internal static void ValidateEntityReferenceLinkNotNull(
      ODataEntityReferenceLink entityReferenceLink)
    {
      if (entityReferenceLink == null)
        throw new ODataException(Strings.WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull);
    }

    internal static void ValidateEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
    {
      if (entityReferenceLink.Url == (Uri) null)
        throw new ODataException(Strings.WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Keeping the validation code for nested resource info multiplicity in one place.")]
    internal static IEdmNavigationProperty ValidateNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmStructuredType declaringStructuredType,
      ODataPayloadKind? expandedPayloadKind,
      bool throwOnUndeclaredProperty)
    {
      if (string.IsNullOrEmpty(nestedResourceInfo.Name))
        throw new ODataException(Strings.ValidationUtils_LinkMustSpecifyName);
      ODataPayloadKind? nullable = expandedPayloadKind;
      ODataPayloadKind odataPayloadKind1 = ODataPayloadKind.EntityReferenceLink;
      bool flag1 = nullable.GetValueOrDefault() == odataPayloadKind1 & nullable.HasValue;
      nullable = expandedPayloadKind;
      ODataPayloadKind odataPayloadKind2 = ODataPayloadKind.ResourceSet;
      bool flag2 = nullable.GetValueOrDefault() == odataPayloadKind2 & nullable.HasValue;
      Func<object, string> func = (Func<object, string>) null;
      bool? isCollection;
      if (!flag1)
      {
        isCollection = nestedResourceInfo.IsCollection;
        if (isCollection.HasValue && expandedPayloadKind.HasValue)
        {
          int num1 = flag2 ? 1 : 0;
          isCollection = nestedResourceInfo.IsCollection;
          int num2 = isCollection.Value ? 1 : 0;
          if (num1 != num2)
            func = expandedPayloadKind.Value == ODataPayloadKind.ResourceSet ? new Func<object, string>(Strings.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent) : new Func<object, string>(Strings.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent);
        }
      }
      IEdmNavigationProperty navigationProperty = (IEdmNavigationProperty) null;
      if (func == null && declaringStructuredType != null)
      {
        navigationProperty = WriterValidationUtils.ValidateNavigationPropertyDefined(nestedResourceInfo.Name, declaringStructuredType, throwOnUndeclaredProperty);
        if (navigationProperty != null)
        {
          bool flag3 = navigationProperty.Type.TypeKind() == EdmTypeKind.Collection;
          isCollection = nestedResourceInfo.IsCollection;
          if (isCollection.HasValue)
          {
            int num3 = flag3 ? 1 : 0;
            isCollection = nestedResourceInfo.IsCollection;
            int num4 = isCollection.GetValueOrDefault() ? 1 : 0;
            if (!(num3 == num4 & isCollection.HasValue))
            {
              isCollection = nestedResourceInfo.IsCollection;
              bool flag4 = false;
              if (!(isCollection.GetValueOrDefault() == flag4 & isCollection.HasValue & flag1))
                func = flag3 ? new Func<object, string>(Strings.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata) : new Func<object, string>(Strings.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata);
            }
          }
          if (!flag1 && expandedPayloadKind.HasValue && flag3 != flag2)
            func = flag3 ? new Func<object, string>(Strings.WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadata) : new Func<object, string>(Strings.WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadata);
        }
      }
      if (func != null)
      {
        string str = nestedResourceInfo.Url == (Uri) null ? "null" : UriUtils.UriToString(nestedResourceInfo.Url);
        throw new ODataException(func((object) str));
      }
      return navigationProperty;
    }

    internal static void ValidateDerivedTypeConstraint(
      IEdmStructuredType resourceType,
      IEdmStructuredType metadataType,
      IEnumerable<string> derivedTypeConstraints,
      string itemKind,
      string itemName)
    {
      if (resourceType == null || metadataType == null || derivedTypeConstraints == null || resourceType == metadataType)
        return;
      string fullTypeName = resourceType.FullTypeName();
      if (!derivedTypeConstraints.Any<string>((Func<string, bool>) (c => c == fullTypeName)))
        throw new ODataException(Strings.WriterValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint((object) fullTypeName, (object) itemKind, (object) itemName));
    }

    internal static void ValidateNestedResourceInfoHasCardinality(
      ODataNestedResourceInfo nestedResourceInfo)
    {
      if (!nestedResourceInfo.IsCollection.HasValue)
        throw new ODataException(Strings.WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection((object) nestedResourceInfo.Name));
    }

    internal static void ValidateNullPropertyValue(
      IEdmTypeReference expectedPropertyTypeReference,
      string propertyName,
      IEdmModel model)
    {
      if (expectedPropertyTypeReference == null)
        return;
      if (expectedPropertyTypeReference.IsNonEntityCollectionType())
        throw new ODataException(Strings.WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue((object) propertyName));
      if (expectedPropertyTypeReference.IsODataPrimitiveTypeKind() && !expectedPropertyTypeReference.IsNullable)
        throw new ODataException(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue((object) propertyName, (object) expectedPropertyTypeReference.FullName()));
      if (expectedPropertyTypeReference.IsODataEnumTypeKind() && !expectedPropertyTypeReference.IsNullable)
        throw new ODataException(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue((object) propertyName, (object) expectedPropertyTypeReference.FullName()));
      if (expectedPropertyTypeReference.IsStream())
        throw new ODataException(Strings.WriterValidationUtils_StreamPropertiesMustNotHaveNullValue((object) propertyName));
      if (expectedPropertyTypeReference.IsODataComplexTypeKind() && !expectedPropertyTypeReference.AsComplex().IsNullable)
        throw new ODataException(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue((object) propertyName, (object) expectedPropertyTypeReference.FullName()));
    }

    private static void ValidateResourceId(Uri id)
    {
      if (id != (Uri) null && UriUtils.UriToString(id).Length == 0)
        throw new ODataException(Strings.WriterValidationUtils_EntriesMustHaveNonEmptyId);
    }
  }
}
