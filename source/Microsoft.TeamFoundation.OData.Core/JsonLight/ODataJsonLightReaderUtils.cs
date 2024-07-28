// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightReaderUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;

namespace Microsoft.OData.JsonLight
{
  internal static class ODataJsonLightReaderUtils
  {
    internal static bool ErrorPropertyNotFound(
      ref ODataJsonLightReaderUtils.ErrorPropertyBitMask propertiesFoundBitField,
      ODataJsonLightReaderUtils.ErrorPropertyBitMask propertyFoundBitMask)
    {
      if ((propertiesFoundBitField & propertyFoundBitMask) == propertyFoundBitMask)
        return false;
      propertiesFoundBitField |= propertyFoundBitMask;
      return true;
    }

    internal static object ConvertValue(
      object value,
      IEdmPrimitiveTypeReference primitiveTypeReference,
      ODataMessageReaderSettings messageReaderSettings,
      bool validateNullValue,
      string propertyName,
      ODataPayloadValueConverter converter)
    {
      if (value == null)
      {
        messageReaderSettings.Validator.ValidateNullValue((IEdmTypeReference) primitiveTypeReference, validateNullValue, propertyName, new bool?());
        return (object) null;
      }
      value = converter.ConvertFromPayloadValue(value, (IEdmTypeReference) primitiveTypeReference);
      return value;
    }

    internal static void EnsureInstance<T>(ref T instance) where T : class, new()
    {
      if ((object) instance != null)
        return;
      instance = new T();
    }

    internal static bool IsODataAnnotationName(string propertyName) => propertyName.StartsWith("odata.", StringComparison.Ordinal);

    internal static bool IsAnnotationProperty(string propertyName) => propertyName.IndexOf('.') >= 0;

    internal static void ValidateAnnotationValue(object propertyValue, string annotationName)
    {
      if (propertyValue == null)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightReaderUtils_AnnotationWithNullValue((object) annotationName));
    }

    internal static string GetPayloadTypeName(object payloadItem)
    {
      switch (payloadItem)
      {
        case null:
          return (string) null;
        case bool _:
          return "Edm.Boolean";
        case string _:
          return "Edm.String";
        case int _:
          return "Edm.Int32";
        case double _:
          return "Edm.Double";
        case ODataCollectionValue odataCollectionValue:
          return EdmLibraryExtensions.GetCollectionTypeFullName(odataCollectionValue.TypeName);
        case ODataResourceBase odataResourceBase:
          return odataResourceBase.TypeName;
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightReader_ReadResourceStart));
      }
    }

    [Flags]
    internal enum ErrorPropertyBitMask
    {
      None = 0,
      Error = 1,
      Code = 2,
      Message = 4,
      MessageValue = 16, // 0x00000010
      InnerError = 32, // 0x00000020
      TypeName = 64, // 0x00000040
      StackTrace = 128, // 0x00000080
      Target = 256, // 0x00000100
      Details = 512, // 0x00000200
    }
  }
}
