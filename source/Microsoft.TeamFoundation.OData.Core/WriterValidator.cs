// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.WriterValidator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal class WriterValidator : IWriterValidator
  {
    private readonly ODataMessageWriterSettings settings;

    internal WriterValidator(ODataMessageWriterSettings settings) => this.settings = settings;

    public IDuplicatePropertyNameChecker CreateDuplicatePropertyNameChecker() => !this.settings.ThrowOnDuplicatePropertyNames ? (IDuplicatePropertyNameChecker) new NullDuplicatePropertyNameChecker() : (IDuplicatePropertyNameChecker) new DuplicatePropertyNameChecker();

    public virtual void ValidateResourceInNestedResourceInfo(
      IEdmStructuredType resourceType,
      IEdmStructuredType parentNavigationPropertyType)
    {
      if (!this.settings.ThrowIfTypeConflictsWithMetadata)
        return;
      WriterValidationUtils.ValidateNestedResource(resourceType, parentNavigationPropertyType);
    }

    public virtual void ValidateNestedResourceInfoHasCardinality(
      ODataNestedResourceInfo nestedResourceInfo)
    {
      WriterValidationUtils.ValidateNestedResourceInfoHasCardinality(nestedResourceInfo);
    }

    public virtual void ValidateIsExpectedPrimitiveType(
      object value,
      IEdmPrimitiveTypeReference valuePrimitiveTypeReference,
      IEdmTypeReference expectedTypeReference)
    {
      if (!this.settings.ThrowIfTypeConflictsWithMetadata)
        return;
      ValidationUtils.ValidateIsExpectedPrimitiveType(value, valuePrimitiveTypeReference, expectedTypeReference);
    }

    public virtual void ValidateTypeReference(
      IEdmTypeReference typeReferenceFromMetadata,
      IEdmTypeReference typeReferenceFromValue)
    {
      if (!this.settings.ThrowIfTypeConflictsWithMetadata)
        return;
      if (typeReferenceFromValue.IsODataPrimitiveTypeKind())
        ValidationUtils.ValidateMetadataPrimitiveType(typeReferenceFromMetadata, typeReferenceFromValue);
      else if (typeReferenceFromMetadata.IsEntity())
        ValidationUtils.ValidateEntityTypeIsAssignable((IEdmEntityTypeReference) typeReferenceFromMetadata, (IEdmEntityTypeReference) typeReferenceFromValue);
      else if (typeReferenceFromMetadata.IsComplex())
        ValidationUtils.ValidateComplexTypeIsAssignable(typeReferenceFromMetadata.Definition as IEdmComplexType, typeReferenceFromValue.Definition as IEdmComplexType);
      else if (typeReferenceFromMetadata.IsCollection())
      {
        if (!typeReferenceFromMetadata.Definition.IsElementTypeEquivalentTo(typeReferenceFromValue.Definition))
          throw new ODataException(Strings.ValidationUtils_IncompatibleType((object) typeReferenceFromValue.FullName(), (object) typeReferenceFromMetadata.FullName()));
      }
      else if (typeReferenceFromMetadata.FullName() != typeReferenceFromValue.FullName())
        throw new ODataException(Strings.ValidationUtils_IncompatibleType((object) typeReferenceFromValue.FullName(), (object) typeReferenceFromMetadata.FullName()));
    }

    public virtual void ValidateTypeKind(
      EdmTypeKind actualTypeKind,
      EdmTypeKind expectedTypeKind,
      bool? expectStructuredType,
      IEdmType edmType)
    {
      if (!this.settings.ThrowIfTypeConflictsWithMetadata)
        return;
      ValidationUtils.ValidateTypeKind(actualTypeKind, expectedTypeKind, expectStructuredType, edmType == null ? (string) null : edmType.FullTypeName());
    }

    public virtual void ValidateMetadataResource(
      ODataResourceBase resource,
      IEdmEntityType resourceType)
    {
      ValidationUtils.ValidateMediaResource(resource, resourceType);
    }

    public void ValidateNullPropertyValue(
      IEdmTypeReference expectedPropertyTypeReference,
      string propertyName,
      bool isTopLevel,
      IEdmModel model)
    {
      if (this.settings.ThrowIfTypeConflictsWithMetadata)
        WriterValidationUtils.ValidateNullPropertyValue(expectedPropertyTypeReference, propertyName, model);
      if (isTopLevel)
      {
        ODataVersion? nullable = this.settings.LibraryCompatibility < ODataLibraryCompatibility.Version7 ? this.settings.Version : throw new ODataException(Strings.ODataMessageWriter_CannotWriteTopLevelNull);
        ODataVersion odataVersion = ODataVersion.V401;
        if (!(nullable.GetValueOrDefault() >= odataVersion & nullable.HasValue))
          ;
      }
    }

    public void ValidateNullCollectionItem(IEdmTypeReference expectedItemType)
    {
      if (!this.settings.ThrowIfTypeConflictsWithMetadata)
        return;
      ValidationUtils.ValidateNullCollectionItem(expectedItemType);
    }

    public IEdmProperty ValidatePropertyDefined(
      string propertyName,
      IEdmStructuredType owningStructuredType)
    {
      return WriterValidationUtils.ValidatePropertyDefined(propertyName, owningStructuredType, this.settings.ThrowOnUndeclaredPropertyForNonOpenType);
    }

    public IEdmNavigationProperty ValidateNestedResourceInfo(
      ODataNestedResourceInfo nestedResourceInfo,
      IEdmStructuredType declaringStructuredType,
      ODataPayloadKind? expandedPayloadKind)
    {
      return WriterValidationUtils.ValidateNestedResourceInfo(nestedResourceInfo, declaringStructuredType, expandedPayloadKind, this.settings.ThrowOnUndeclaredPropertyForNonOpenType);
    }

    public void ValidateDerivedTypeConstraint(
      IEdmStructuredType resourceType,
      IEdmStructuredType metadataType,
      IEnumerable<string> derivedTypeConstraints,
      string itemKind,
      string itemName)
    {
      WriterValidationUtils.ValidateDerivedTypeConstraint(resourceType, metadataType, derivedTypeConstraints, itemKind, itemName);
    }
  }
}
