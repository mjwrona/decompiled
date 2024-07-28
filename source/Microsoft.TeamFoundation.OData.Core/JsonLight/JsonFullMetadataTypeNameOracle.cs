// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.JsonFullMetadataTypeNameOracle
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;

namespace Microsoft.OData.JsonLight
{
  internal sealed class JsonFullMetadataTypeNameOracle : JsonLightTypeNameOracle
  {
    internal override string GetResourceSetTypeNameForWriting(
      string expectedResourceTypeName,
      ODataResourceSet resourceSet,
      bool isUndeclared)
    {
      return resourceSet.TypeAnnotation != null ? resourceSet.TypeAnnotation.TypeName : resourceSet.TypeName;
    }

    internal override string GetResourceTypeNameForWriting(
      string expectedTypeName,
      ODataResourceBase resource,
      bool isUndeclared = false)
    {
      return resource.TypeAnnotation != null ? resource.TypeAnnotation.TypeName : resource.TypeName;
    }

    internal override string GetValueTypeNameForWriting(
      ODataValue value,
      IEdmTypeReference typeReferenceFromMetadata,
      IEdmTypeReference typeReferenceFromValue,
      bool isOpenProperty)
    {
      string propertyName;
      if (TypeNameOracle.TryGetTypeNameFromAnnotation(value, out propertyName))
        return propertyName;
      return typeReferenceFromValue != null && typeReferenceFromValue.IsPrimitive() && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue) value, typeReferenceFromValue.AsPrimitive()) ? (string) null : TypeNameOracle.GetTypeNameFromValue((object) value);
    }

    internal override string GetValueTypeNameForWriting(
      ODataValue value,
      PropertySerializationInfo propertyInfo,
      bool isOpenProperty)
    {
      PropertyValueTypeInfo valueType = propertyInfo.ValueType;
      string propertyName;
      if (TypeNameOracle.TryGetTypeNameFromAnnotation(value, out propertyName))
        return propertyName;
      return valueType.TypeReference != null && valueType.IsPrimitive && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue) value, valueType.PrimitiveTypeKind) ? (string) null : TypeNameOracle.GetTypeNameFromValue((object) value);
    }
  }
}
