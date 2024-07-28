// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.ODataJsonReaderCoreUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Json
{
  internal static class ODataJsonReaderCoreUtils
  {
    internal static ISpatial ReadSpatialValue(
      IJsonReader jsonReader,
      bool insideJsonObjectValue,
      ODataInputContext inputContext,
      IEdmPrimitiveTypeReference expectedValueTypeReference,
      bool validateNullValue,
      int recursionDepth,
      string propertyName)
    {
      if (!insideJsonObjectValue && ODataJsonReaderCoreUtils.TryReadNullValue(jsonReader, inputContext, (IEdmTypeReference) expectedValueTypeReference, validateNullValue, propertyName))
        return (ISpatial) null;
      ISpatial spatial = (ISpatial) null;
      if (insideJsonObjectValue || jsonReader.NodeType == JsonNodeType.StartObject)
      {
        IDictionary<string, object> source = ODataJsonReaderCoreUtils.ReadObjectValue(jsonReader, insideJsonObjectValue, inputContext, recursionDepth);
        GeoJsonObjectFormatter jsonObjectFormatter = SpatialImplementation.CurrentImplementation.CreateGeoJsonObjectFormatter();
        spatial = !expectedValueTypeReference.IsGeography() ? (ISpatial) jsonObjectFormatter.Read<Geometry>(source) : (ISpatial) jsonObjectFormatter.Read<Geography>(source);
      }
      return spatial != null ? spatial : throw new ODataException(Microsoft.OData.Strings.ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue);
    }

    internal static bool TryReadNullValue(
      IJsonReader jsonReader,
      ODataInputContext inputContext,
      IEdmTypeReference expectedTypeReference,
      bool validateNullValue,
      string propertyName,
      bool? isDynamicProperty = null)
    {
      if (jsonReader.NodeType != JsonNodeType.PrimitiveValue || jsonReader.Value != null)
        return false;
      int num = (int) jsonReader.ReadNext();
      inputContext.MessageReaderSettings.Validator.ValidateNullValue(expectedTypeReference, validateNullValue, propertyName, isDynamicProperty);
      return true;
    }

    private static IDictionary<string, object> ReadObjectValue(
      IJsonReader jsonReader,
      bool insideJsonObjectValue,
      ODataInputContext inputContext,
      int recursionDepth)
    {
      ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth);
      IDictionary<string, object> dictionary = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
      if (!insideJsonObjectValue)
      {
        int num = (int) jsonReader.ReadNext();
      }
      while (jsonReader.NodeType != JsonNodeType.EndObject)
      {
        string annotationName = jsonReader.ReadPropertyName();
        object obj;
        switch (jsonReader.NodeType)
        {
          case JsonNodeType.StartObject:
            obj = (object) ODataJsonReaderCoreUtils.ReadObjectValue(jsonReader, false, inputContext, recursionDepth);
            break;
          case JsonNodeType.StartArray:
            obj = (object) ODataJsonReaderCoreUtils.ReadArrayValue(jsonReader, inputContext, recursionDepth);
            break;
          case JsonNodeType.PrimitiveValue:
            obj = jsonReader.ReadPrimitiveValue();
            break;
          default:
            return (IDictionary<string, object>) null;
        }
        dictionary.Add(ODataAnnotationNames.RemoveAnnotationPrefix(annotationName), obj);
      }
      jsonReader.ReadEndObject();
      return dictionary;
    }

    private static IEnumerable<object> ReadArrayValue(
      IJsonReader jsonReader,
      ODataInputContext inputContext,
      int recursionDepth)
    {
      ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth);
      List<object> objectList = new List<object>();
      int num = (int) jsonReader.ReadNext();
      while (jsonReader.NodeType != JsonNodeType.EndArray)
      {
        switch (jsonReader.NodeType)
        {
          case JsonNodeType.StartObject:
            objectList.Add((object) ODataJsonReaderCoreUtils.ReadObjectValue(jsonReader, false, inputContext, recursionDepth));
            continue;
          case JsonNodeType.StartArray:
            objectList.Add((object) ODataJsonReaderCoreUtils.ReadArrayValue(jsonReader, inputContext, recursionDepth));
            continue;
          case JsonNodeType.PrimitiveValue:
            objectList.Add(jsonReader.ReadPrimitiveValue());
            continue;
          default:
            return (IEnumerable<object>) null;
        }
      }
      jsonReader.ReadEndArray();
      return (IEnumerable<object>) objectList;
    }
  }
}
