// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.JsonWriterExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.OData.Json
{
  internal static class JsonWriterExtensions
  {
    internal static void WriteJsonObjectValue(
      this IJsonWriter jsonWriter,
      IDictionary<string, object> jsonObjectValue,
      Action<IJsonWriter> injectPropertyAction)
    {
      jsonWriter.StartObjectScope();
      if (injectPropertyAction != null)
        injectPropertyAction(jsonWriter);
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) jsonObjectValue)
      {
        jsonWriter.WriteName(keyValuePair.Key);
        jsonWriter.WriteJsonValue(keyValuePair.Value);
      }
      jsonWriter.EndObjectScope();
    }

    internal static void WritePrimitiveValue(this IJsonWriter jsonWriter, object value)
    {
      switch (value)
      {
        case bool flag:
          jsonWriter.WriteValue(flag);
          break;
        case byte num1:
          jsonWriter.WriteValue(num1);
          break;
        case Decimal num2:
          jsonWriter.WriteValue(num2);
          break;
        case double num3:
          jsonWriter.WriteValue(num3);
          break;
        case short num4:
          jsonWriter.WriteValue(num4);
          break;
        case int num5:
          jsonWriter.WriteValue(num5);
          break;
        case long num6:
          jsonWriter.WriteValue(num6);
          break;
        case sbyte num7:
          jsonWriter.WriteValue(num7);
          break;
        case float num8:
          jsonWriter.WriteValue(num8);
          break;
        case string str:
          jsonWriter.WriteValue(str);
          break;
        case byte[] numArray:
          jsonWriter.WriteValue(numArray);
          break;
        case DateTimeOffset dateTimeOffset:
          jsonWriter.WriteValue(dateTimeOffset);
          break;
        case Guid guid:
          jsonWriter.WriteValue(guid);
          break;
        case TimeSpan timeSpan:
          jsonWriter.WriteValue(timeSpan);
          break;
        case Date date:
          jsonWriter.WriteValue(date);
          break;
        case TimeOfDay timeOfDay:
          jsonWriter.WriteValue(timeOfDay);
          break;
        default:
          throw new ODataException(Microsoft.OData.Strings.ODataJsonWriter_UnsupportedValueType((object) value.GetType().FullName));
      }
    }

    internal static void WriteODataValue(this IJsonWriter jsonWriter, ODataValue odataValue)
    {
      switch (odataValue)
      {
        case null:
        case ODataNullValue _:
          jsonWriter.WriteValue((string) null);
          break;
        default:
          object obj1 = odataValue.FromODataValue();
          if (EdmLibraryExtensions.IsPrimitiveType(obj1.GetType()))
          {
            jsonWriter.WritePrimitiveValue(obj1);
            break;
          }
          switch (odataValue)
          {
            case ODataResourceValue odataResourceValue:
              jsonWriter.StartObjectScope();
              foreach (ODataProperty property in odataResourceValue.Properties)
              {
                jsonWriter.WriteName(property.Name);
                jsonWriter.WriteODataValue(property.ODataValue);
              }
              jsonWriter.EndObjectScope();
              return;
            case ODataCollectionValue odataCollectionValue:
              jsonWriter.StartArrayScope();
              foreach (object obj2 in odataCollectionValue.Items)
              {
                ODataValue odataValue1 = obj2 as ODataValue;
                if (obj2 == null)
                  throw new ODataException(Microsoft.OData.Strings.ODataJsonWriter_UnsupportedValueInCollection);
                jsonWriter.WriteODataValue(odataValue1);
              }
              jsonWriter.EndArrayScope();
              return;
            default:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonWriter_UnsupportedValueType((object) odataValue.GetType().FullName));
          }
      }
    }

    private static void WriteJsonArrayValue(this IJsonWriter jsonWriter, IEnumerable arrayValue)
    {
      jsonWriter.StartArrayScope();
      foreach (object propertyValue in arrayValue)
        jsonWriter.WriteJsonValue(propertyValue);
      jsonWriter.EndArrayScope();
    }

    private static void WriteJsonValue(this IJsonWriter jsonWriter, object propertyValue)
    {
      if (propertyValue == null)
        jsonWriter.WriteValue((string) null);
      else if (EdmLibraryExtensions.IsPrimitiveType(propertyValue.GetType()))
        jsonWriter.WritePrimitiveValue(propertyValue);
      else if (propertyValue is IDictionary<string, object> jsonObjectValue)
      {
        jsonWriter.WriteJsonObjectValue(jsonObjectValue, (Action<IJsonWriter>) null);
      }
      else
      {
        IEnumerable arrayValue = propertyValue as IEnumerable;
        jsonWriter.WriteJsonArrayValue(arrayValue);
      }
    }
  }
}
