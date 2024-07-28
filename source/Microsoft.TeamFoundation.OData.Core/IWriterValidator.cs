// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.IWriterValidator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal interface IWriterValidator
  {
    IDuplicatePropertyNameChecker CreateDuplicatePropertyNameChecker();

    void ValidateResourceInNestedResourceInfo(
      IEdmStructuredType resourceType,
      IEdmStructuredType parentNavigationPropertyType);

    void ValidateNestedResourceInfoHasCardinality(ODataNestedResourceInfo nestedResourceInfo);

    void ValidateIsExpectedPrimitiveType(
      object value,
      IEdmPrimitiveTypeReference valuePrimitiveTypeReference,
      IEdmTypeReference expectedTypeReference);

    void ValidateTypeReference(
      IEdmTypeReference typeReferenceFromMetadata,
      IEdmTypeReference typeReferenceFromValue);

    void ValidateTypeKind(
      EdmTypeKind actualTypeKind,
      EdmTypeKind expectedTypeKind,
      bool? expectStructuredType,
      IEdmType edmType);

    void ValidateMetadataResource(ODataResourceBase resource, IEdmEntityType resourceType);

    void ValidateNullPropertyValue(
      IEdmTypeReference expectedPropertyTypeReference,
      string propertyName,
      bool isTopLevel,
      IEdmModel model);

    void ValidateNullCollectionItem(IEdmTypeReference expectedItemType);

    IEdmProperty ValidatePropertyDefined(
      string propertyName,
      IEdmStructuredType owningStructuredType);

    IEdmNavigationProperty ValidateNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmStructuredType declaringStructuredType,
      ODataPayloadKind? expandedPayloadKind);

    void ValidateDerivedTypeConstraint(
      IEdmStructuredType resourceType,
      IEdmStructuredType metadataType,
      IEnumerable<string> derivedTypeConstraints,
      string itemKind,
      string itemName);
  }
}
