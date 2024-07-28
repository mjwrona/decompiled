// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.JsonMinimalMetadataTypeNameOracle
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.JsonLight
{
  internal sealed class JsonMinimalMetadataTypeNameOracle : JsonLightTypeNameOracle
  {
    internal override string GetResourceSetTypeNameForWriting(
      string expectedResourceTypeName,
      ODataResourceSet resourceSet,
      bool isUndeclared)
    {
      if (resourceSet.TypeAnnotation != null)
        return resourceSet.TypeAnnotation.TypeName;
      return (expectedResourceTypeName == null ? (string) null : EdmLibraryExtensions.GetCollectionTypeName(expectedResourceTypeName)) != resourceSet.TypeName | isUndeclared ? resourceSet.TypeName : (string) null;
    }

    internal override string GetResourceTypeNameForWriting(
      string expectedTypeName,
      ODataResourceBase resource,
      bool isUndeclared = false)
    {
      if (resource.TypeAnnotation != null)
        return resource.TypeAnnotation.TypeName;
      string typeName = resource.TypeName;
      return expectedTypeName != typeName | isUndeclared ? typeName : (string) null;
    }

    internal override string GetValueTypeNameForWriting(
      ODataValue value,
      IEdmTypeReference typeReferenceFromMetadata,
      IEdmTypeReference typeReferenceFromValue,
      bool isOpenProperty)
    {
      string typeNameForWriting = (string) null;
      string propertyName;
      if (TypeNameOracle.TryGetTypeNameFromAnnotation(value, out propertyName))
        return propertyName;
      if (typeReferenceFromValue != null)
      {
        typeNameForWriting = typeReferenceFromValue.FullName();
        if (typeReferenceFromMetadata != null && typeReferenceFromMetadata.Definition.AsActualType().FullTypeName() != typeNameForWriting)
          return typeNameForWriting;
        if (typeReferenceFromValue.IsPrimitive() && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue) value, typeReferenceFromValue.AsPrimitive()))
          return (string) null;
        if (typeReferenceFromMetadata == null && typeReferenceFromValue.IsStructured() && (typeReferenceFromValue as IEdmStructuredTypeReference).StructuredDefinition().BaseType != null)
          return typeNameForWriting;
      }
      return !isOpenProperty ? (string) null : typeNameForWriting ?? TypeNameOracle.GetTypeNameFromValue((object) value);
    }

    internal override string GetValueTypeNameForWriting(
      ODataValue value,
      PropertySerializationInfo propertyInfo,
      bool isOpenProperty)
    {
      string typeNameForWriting = (string) null;
      PropertyValueTypeInfo valueType = propertyInfo.ValueType;
      PropertyMetadataTypeInfo metadataType = propertyInfo.MetadataType;
      string propertyName;
      if (TypeNameOracle.TryGetTypeNameFromAnnotation(value, out propertyName))
        return propertyName;
      if (valueType.TypeReference != null)
      {
        typeNameForWriting = valueType.FullName;
        if (metadataType.TypeReference != null && metadataType.FullName != typeNameForWriting)
          return typeNameForWriting;
        if (valueType.IsPrimitive && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue) value, valueType.PrimitiveTypeKind))
          return (string) null;
        if (metadataType.TypeReference == null && valueType.IsComplex && (valueType.TypeReference as IEdmComplexTypeReference).ComplexDefinition().BaseType != null)
          return typeNameForWriting;
      }
      return !isOpenProperty ? (string) null : typeNameForWriting ?? TypeNameOracle.GetTypeNameFromValue((object) value);
    }
  }
}
