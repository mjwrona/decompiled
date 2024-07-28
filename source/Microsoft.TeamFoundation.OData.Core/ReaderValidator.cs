// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ReaderValidator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData
{
  internal class ReaderValidator : IReaderValidator
  {
    private readonly ODataMessageReaderSettings settings;

    internal ReaderValidator(ODataMessageReaderSettings settings) => this.settings = settings;

    public virtual void ValidateMediaResource(
      ODataResourceBase resource,
      IEdmEntityType resourceType)
    {
      ValidationUtils.ValidateMediaResource(resource, resourceType);
    }

    public PropertyAndAnnotationCollector CreatePropertyAndAnnotationCollector() => new PropertyAndAnnotationCollector(this.settings.ThrowOnDuplicatePropertyNames);

    public void ValidateNullValue(
      IEdmTypeReference expectedTypeReference,
      bool validateNullValue,
      string propertyName,
      bool? isDynamicProperty)
    {
      if (!this.settings.ThrowIfTypeConflictsWithMetadata)
        return;
      ReaderValidationUtils.ValidateNullValue(expectedTypeReference, this.settings.EnablePrimitiveTypeConversion, validateNullValue, propertyName, isDynamicProperty);
    }

    public IEdmTypeReference ResolvePayloadTypeNameAndComputeTargetType(
      EdmTypeKind expectedTypeKind,
      bool? expectStructuredType,
      IEdmType defaultPrimitivePayloadType,
      IEdmTypeReference expectedTypeReference,
      string payloadTypeName,
      IEdmModel model,
      Func<EdmTypeKind> typeKindFromPayloadFunc,
      out EdmTypeKind targetTypeKind,
      out ODataTypeAnnotation typeAnnotation)
    {
      return ReaderValidationUtils.ResolvePayloadTypeNameAndComputeTargetType(expectedTypeKind, expectStructuredType, defaultPrimitivePayloadType, expectedTypeReference, payloadTypeName, model, this.settings.ClientCustomTypeResolver, this.settings.ThrowIfTypeConflictsWithMetadata, this.settings.EnablePrimitiveTypeConversion, typeKindFromPayloadFunc, out targetTypeKind, out typeAnnotation);
    }

    public IEdmProperty ValidatePropertyDefined(
      string propertyName,
      IEdmStructuredType owningStructuredType)
    {
      return ReaderValidationUtils.ValidatePropertyDefined(propertyName, owningStructuredType, this.settings.ThrowOnUndeclaredPropertyForNonOpenType);
    }

    public void ValidateStreamReferenceProperty(
      IODataStreamReferenceInfo streamInfo,
      string propertyName,
      IEdmStructuredType structuredType,
      IEdmProperty streamEdmProperty)
    {
      ReaderValidationUtils.ValidateStreamReferenceProperty(streamInfo, propertyName, structuredType, streamEdmProperty, this.settings.ThrowOnUndeclaredPropertyForNonOpenType);
    }
  }
}
