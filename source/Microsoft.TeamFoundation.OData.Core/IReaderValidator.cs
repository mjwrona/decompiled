// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.IReaderValidator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData
{
  internal interface IReaderValidator
  {
    void ValidateMediaResource(ODataResourceBase resource, IEdmEntityType resourceType);

    PropertyAndAnnotationCollector CreatePropertyAndAnnotationCollector();

    void ValidateNullValue(
      IEdmTypeReference expectedTypeReference,
      bool validateNullValue,
      string propertyName,
      bool? isDynamicProperty);

    IEdmTypeReference ResolvePayloadTypeNameAndComputeTargetType(
      EdmTypeKind expectedTypeKind,
      bool? expectStructuredType,
      IEdmType defaultPrimitivePayloadType,
      IEdmTypeReference expectedTypeReference,
      string payloadTypeName,
      IEdmModel model,
      Func<EdmTypeKind> typeKindFromPayloadFunc,
      out EdmTypeKind targetTypeKind,
      out ODataTypeAnnotation typeAnnotation);

    IEdmProperty ValidatePropertyDefined(
      string propertyName,
      IEdmStructuredType owningStructuredType);

    void ValidateStreamReferenceProperty(
      IODataStreamReferenceInfo streamInfo,
      string propertyName,
      IEdmStructuredType structuredType,
      IEdmProperty streamEdmProperty);
  }
}
