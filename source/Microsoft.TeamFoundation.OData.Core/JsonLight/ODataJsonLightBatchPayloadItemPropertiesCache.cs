// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightBatchPayloadItemPropertiesCache
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OData.JsonLight
{
  internal class ODataJsonLightBatchPayloadItemPropertiesCache
  {
    internal const string PropertyNameId = "ID";
    internal const string PropertyNameAtomicityGroup = "ATOMICITYGROUP";
    internal const string PropertyNameHeaders = "HEADERS";
    internal const string PropertyNameBody = "BODY";
    internal const string PropertyNameDependsOn = "DEPENDSON";
    internal const string PropertyNameMethod = "METHOD";
    internal const string PropertyNameUrl = "URL";
    internal const string PropertyNameStatus = "STATUS";
    private IJsonReader jsonReader;
    private IODataStreamListener listener;
    private Dictionary<string, object> jsonProperties;
    private bool isStreamPopulated;

    internal ODataJsonLightBatchPayloadItemPropertiesCache(ODataJsonLightBatchReader jsonBatchReader)
    {
      this.jsonReader = (IJsonReader) jsonBatchReader.JsonLightInputContext.JsonReader;
      this.listener = (IODataStreamListener) jsonBatchReader;
      this.ScanJsonProperties();
    }

    internal object GetPropertyValue(string propertyName)
    {
      object obj;
      return this.jsonProperties != null && this.jsonProperties.TryGetValue(ODataJsonLightBatchPayloadItemPropertiesCache.Normalize(propertyName), out obj) ? obj : (object) null;
    }

    private ODataJsonLightBatchBodyContentReaderStream CreateJsonPayloadBodyContentStream(
      string contentTypeHeader)
    {
      ODataJsonLightBatchBodyContentReaderStream bodyContentStream = new ODataJsonLightBatchBodyContentReaderStream(this.listener);
      this.isStreamPopulated = bodyContentStream.PopulateBodyContent(this.jsonReader, contentTypeHeader);
      return bodyContentStream;
    }

    private static string Normalize(string propertyName) => propertyName.ToUpperInvariant();

    private void ScanJsonProperties()
    {
      this.jsonProperties = new Dictionary<string, object>();
      string contentTypeHeader = (string) null;
      ODataJsonLightBatchBodyContentReaderStream contentReaderStream = (ODataJsonLightBatchBodyContentReaderStream) null;
      try
      {
        this.jsonReader.ReadStartObject();
        while (this.jsonReader.NodeType != JsonNodeType.EndObject)
        {
          string key1 = ODataJsonLightBatchPayloadItemPropertiesCache.Normalize(this.jsonReader.ReadPropertyName());
          switch (key1)
          {
            case "ATOMICITYGROUP":
            case "ID":
            case "METHOD":
            case "URL":
              this.jsonProperties.Add(key1, (object) this.jsonReader.ReadStringValue());
              continue;
            case "BODY":
              contentReaderStream = this.CreateJsonPayloadBodyContentStream(contentTypeHeader);
              this.jsonProperties.Add(key1, (object) contentReaderStream);
              continue;
            case "DEPENDSON":
              IList<string> stringList = (IList<string>) new List<string>();
              this.jsonReader.ReadStartArray();
              while (this.jsonReader.NodeType != JsonNodeType.EndArray)
                stringList.Add(this.jsonReader.ReadStringValue());
              this.jsonReader.ReadEndArray();
              this.jsonProperties.Add(key1, (object) stringList);
              continue;
            case "HEADERS":
              ODataBatchOperationHeaders operationHeaders = new ODataBatchOperationHeaders();
              contentTypeHeader = "";
              this.jsonReader.ReadStartObject();
              while (this.jsonReader.NodeType != JsonNodeType.EndObject)
              {
                string key2 = this.jsonReader.ReadPropertyName();
                string str = this.jsonReader.ReadPrimitiveValue().ToString();
                if (key2.Equals("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                  contentTypeHeader = str;
                operationHeaders.Add(key2, str);
              }
              this.jsonReader.ReadEndObject();
              this.jsonProperties.Add(key1, (object) operationHeaders);
              if (!this.isStreamPopulated && contentReaderStream != null)
              {
                contentReaderStream.PopulateCachedBodyContent(contentTypeHeader);
                continue;
              }
              continue;
            case "STATUS":
              this.jsonProperties.Add(key1, this.jsonReader.ReadPrimitiveValue());
              continue;
            default:
              throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown property name '{0}' for message in batch", new object[1]
              {
                (object) key1
              }));
          }
        }
        this.jsonReader.ReadEndObject();
      }
      finally
      {
        this.jsonReader = (IJsonReader) null;
      }
    }
  }
}
