// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.JsonReaderExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.OData.Json
{
  internal static class JsonReaderExtensions
  {
    internal static void ReadStartObject(this IJsonReader jsonReader) => jsonReader.ReadNext(JsonNodeType.StartObject);

    internal static void ReadEndObject(this IJsonReader jsonReader) => jsonReader.ReadNext(JsonNodeType.EndObject);

    internal static void ReadStartArray(this IJsonReader jsonReader) => jsonReader.ReadNext(JsonNodeType.StartArray);

    internal static void ReadEndArray(this IJsonReader jsonReader) => jsonReader.ReadNext(JsonNodeType.EndArray);

    internal static string GetPropertyName(this IJsonReader jsonReader) => (string) jsonReader.Value;

    internal static string ReadPropertyName(this IJsonReader jsonReader)
    {
      jsonReader.ValidateNodeType(JsonNodeType.Property);
      string propertyName = jsonReader.GetPropertyName();
      int num = (int) jsonReader.ReadNext();
      return propertyName;
    }

    internal static object ReadPrimitiveValue(this IJsonReader jsonReader)
    {
      object obj = jsonReader.Value;
      jsonReader.ReadNext(JsonNodeType.PrimitiveValue);
      return obj;
    }

    internal static string ReadStringValue(this IJsonReader jsonReader)
    {
      object p0 = jsonReader.ReadPrimitiveValue();
      string str = p0 as string;
      return p0 == null || str != null ? str : throw JsonReaderExtensions.CreateException(Strings.JsonReaderExtensions_CannotReadValueAsString(p0));
    }

    internal static Uri ReadUriValue(this IJsonReader jsonReader) => UriUtils.StringToUri(jsonReader.ReadStringValue());

    internal static string ReadStringValue(this IJsonReader jsonReader, string propertyName)
    {
      object p0 = jsonReader.ReadPrimitiveValue();
      string str = p0 as string;
      return p0 == null || str != null ? str : throw JsonReaderExtensions.CreateException(Strings.JsonReaderExtensions_CannotReadPropertyValueAsString(p0, (object) propertyName));
    }

    internal static double? ReadDoubleValue(this IJsonReader jsonReader)
    {
      object p0 = jsonReader.ReadPrimitiveValue();
      double? nullable1 = p0 as double?;
      if (p0 == null || nullable1.HasValue)
        return nullable1;
      int? nullable2 = p0 as int?;
      if (nullable2.HasValue)
        return new double?((double) nullable2.Value);
      return new double?((double) (p0 as Decimal? ?? throw JsonReaderExtensions.CreateException(Strings.JsonReaderExtensions_CannotReadValueAsDouble(p0))).Value);
    }

    internal static void SkipValue(this IJsonReader jsonReader)
    {
      int num = 0;
      do
      {
        switch (jsonReader.NodeType)
        {
          case JsonNodeType.StartObject:
          case JsonNodeType.StartArray:
            ++num;
            break;
          case JsonNodeType.EndObject:
          case JsonNodeType.EndArray:
            --num;
            break;
        }
      }
      while (jsonReader.Read() && num > 0);
      if (num > 0)
        throw JsonReaderExtensions.CreateException(Strings.JsonReader_EndOfInputWithOpenScope);
    }

    internal static void SkipValue(
      this IJsonReader jsonReader,
      StringBuilder jsonRawValueStringBuilder)
    {
      using (StringWriter writer = new StringWriter(jsonRawValueStringBuilder, (IFormatProvider) CultureInfo.InvariantCulture))
      {
        JsonWriter jsonWriter = new JsonWriter((TextWriter) writer, false);
        int num = 0;
        do
        {
          switch (jsonReader.NodeType)
          {
            case JsonNodeType.StartObject:
              jsonWriter.StartObjectScope();
              ++num;
              break;
            case JsonNodeType.EndObject:
              jsonWriter.EndObjectScope();
              --num;
              break;
            case JsonNodeType.StartArray:
              jsonWriter.StartArrayScope();
              ++num;
              break;
            case JsonNodeType.EndArray:
              jsonWriter.EndArrayScope();
              --num;
              break;
            case JsonNodeType.Property:
              jsonWriter.WriteName(jsonReader.GetPropertyName());
              break;
            case JsonNodeType.PrimitiveValue:
              if (jsonReader.Value == null)
              {
                jsonWriter.WriteValue((string) null);
                break;
              }
              jsonWriter.WritePrimitiveValue(jsonReader.Value);
              break;
          }
        }
        while (jsonReader.Read() && num > 0);
        if (num > 0)
          throw JsonReaderExtensions.CreateException(Strings.JsonReader_EndOfInputWithOpenScope);
        jsonWriter.Flush();
      }
    }

    internal static ODataValue ReadAsUntypedOrNullValue(this IJsonReader jsonReader)
    {
      StringBuilder jsonRawValueStringBuilder = new StringBuilder();
      jsonReader.SkipValue(jsonRawValueStringBuilder);
      return (ODataValue) new ODataUntypedValue()
      {
        RawValue = jsonRawValueStringBuilder.ToString()
      };
    }

    internal static ODataValue ReadODataValue(this IJsonReader jsonReader)
    {
      if (jsonReader.NodeType == JsonNodeType.PrimitiveValue)
        return jsonReader.ReadPrimitiveValue().ToODataValue();
      if (jsonReader.NodeType == JsonNodeType.StartObject)
      {
        jsonReader.ReadStartObject();
        ODataResourceValue odataResourceValue = new ODataResourceValue();
        IList<ODataProperty> odataPropertyList = (IList<ODataProperty>) new List<ODataProperty>();
        while (jsonReader.NodeType != JsonNodeType.EndObject)
        {
          ODataProperty odataProperty = new ODataProperty();
          odataProperty.Name = jsonReader.ReadPropertyName();
          odataProperty.Value = (object) jsonReader.ReadODataValue();
          odataPropertyList.Add(odataProperty);
        }
        odataResourceValue.Properties = (IEnumerable<ODataProperty>) odataPropertyList;
        jsonReader.ReadEndObject();
        return (ODataValue) odataResourceValue;
      }
      if (jsonReader.NodeType != JsonNodeType.StartArray)
        return jsonReader.ReadAsUntypedOrNullValue();
      jsonReader.ReadStartArray();
      ODataCollectionValue odataCollectionValue = new ODataCollectionValue();
      IList<object> objectList = (IList<object>) new List<object>();
      while (jsonReader.NodeType != JsonNodeType.EndArray)
        objectList.Add((object) jsonReader.ReadODataValue());
      odataCollectionValue.Items = (IEnumerable<object>) objectList;
      jsonReader.ReadEndArray();
      return (ODataValue) odataCollectionValue;
    }

    internal static JsonNodeType ReadNext(this IJsonReader jsonReader)
    {
      jsonReader.Read();
      return jsonReader.NodeType;
    }

    internal static bool IsOnValueNode(this IJsonReader jsonReader)
    {
      JsonNodeType nodeType = jsonReader.NodeType;
      switch (nodeType)
      {
        case JsonNodeType.StartObject:
        case JsonNodeType.PrimitiveValue:
          return true;
        default:
          return nodeType == JsonNodeType.StartArray;
      }
    }

    [Conditional("DEBUG")]
    internal static void AssertNotBuffering(this BufferingJsonReader bufferedJsonReader)
    {
    }

    [Conditional("DEBUG")]
    internal static void AssertBuffering(this BufferingJsonReader bufferedJsonReader)
    {
    }

    internal static ODataException CreateException(string exceptionMessage) => new ODataException(exceptionMessage);

    private static void ReadNext(this IJsonReader jsonReader, JsonNodeType expectedNodeType)
    {
      jsonReader.ValidateNodeType(expectedNodeType);
      jsonReader.Read();
    }

    private static void ValidateNodeType(this IJsonReader jsonReader, JsonNodeType expectedNodeType)
    {
      if (jsonReader.NodeType != expectedNodeType)
        throw JsonReaderExtensions.CreateException(Strings.JsonReaderExtensions_UnexpectedNodeDetected((object) expectedNodeType, (object) jsonReader.NodeType));
    }
  }
}
