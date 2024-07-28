// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.EntityTranslator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Interop.Common.Schema.Edm;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal static class EntityTranslator
  {
    internal static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
    {
      ContractResolver = (IContractResolver) new DefaultContractResolver(),
      NullValueHandling = NullValueHandling.Ignore,
      DateParseHandling = DateParseHandling.None
    };
    internal static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(EntityTranslator.JsonSerializerSettings);

    internal static Document GetDocumentFromEntityProperties(
      IDictionary<string, EntityProperty> properties,
      string partitionKey,
      string rowKey,
      bool removeSystemGeneratedProperties)
    {
      if (properties == null)
        throw new ArgumentException("Entity properties should not be null.");
      if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
        throw new ArgumentException("Partition/row key should not be null");
      if (removeSystemGeneratedProperties)
      {
        if (properties.ContainsKey("Timestamp"))
          properties.Remove("Timestamp");
        if (properties.ContainsKey("Etag"))
          properties.Remove("Etag");
      }
      using (StringWriter writer = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (ITableEntityWriter tableEntityWriter = (ITableEntityWriter) new TableEntityWriter((TextWriter) writer))
        {
          tableEntityWriter.Start();
          foreach (KeyValuePair<string, EntityProperty> property in (IEnumerable<KeyValuePair<string, EntityProperty>>) properties)
          {
            if (!property.Value.IsNull)
            {
              tableEntityWriter.WriteName(property.Key);
              switch (property.Value.PropertyType)
              {
                case EdmType.String:
                  tableEntityWriter.WriteString(property.Value.StringValue);
                  continue;
                case EdmType.Binary:
                  tableEntityWriter.WriteBinary(property.Value.BinaryValue);
                  continue;
                case EdmType.Boolean:
                  tableEntityWriter.WriteBoolean(property.Value.BooleanValue);
                  continue;
                case EdmType.DateTime:
                  tableEntityWriter.WriteDateTime(property.Value.DateTime);
                  continue;
                case EdmType.Double:
                  tableEntityWriter.WriteDouble(property.Value.DoubleValue);
                  continue;
                case EdmType.Guid:
                  tableEntityWriter.WriteGuid(property.Value.GuidValue);
                  continue;
                case EdmType.Int32:
                  tableEntityWriter.WriteInt32(property.Value.Int32Value);
                  continue;
                case EdmType.Int64:
                  tableEntityWriter.WriteInt64(property.Value.Int64Value);
                  continue;
                default:
                  throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unexpected Edm type '{0}'", (object) (int) property.Value.PropertyType));
              }
            }
          }
          tableEntityWriter.End();
        }
        Document entityProperties = EntityTranslator.DeserializeObject<Document>(writer.ToString());
        entityProperties.Id = rowKey;
        entityProperties.SetPropertyValue("$pk", (object) partitionKey);
        entityProperties.SetPropertyValue("$id", (object) rowKey);
        return entityProperties;
      }
    }

    internal static Document GetDocumentWithPartitionAndRowKey(string partitionKey, string rowKey)
    {
      Document partitionAndRowKey = new Document();
      partitionAndRowKey.Id = rowKey;
      partitionAndRowKey.SetPropertyValue("$pk", (object) partitionKey);
      return partitionAndRowKey;
    }

    internal static void GetEntityPropertiesFromDocument(
      Document document,
      IList<string> selectColumns,
      out string partitionKey,
      out string rowKey,
      out string eTag,
      out DateTimeOffset timestamp,
      out IDictionary<string, EntityProperty> entityProperties)
    {
      partitionKey = document.GetPropertyValue<string>("$pk");
      rowKey = document.GetPropertyValue<string>("$id");
      eTag = document.ETag;
      timestamp = (DateTimeOffset) document.Timestamp;
      entityProperties = EntityTranslator.GetEntityPropertiesFromDocument(document, selectColumns);
    }

    internal static IDictionary<string, EntityProperty> GetEntityPropertiesFromDocument(
      Document document,
      IList<string> selectColumns)
    {
      string serializedDocument = EntityTranslator.SerializeObject((object) document);
      IDictionary<string, EntityProperty> entityProperties;
      EdmConverter.GetPropertiesFromDocument(selectColumns, serializedDocument, out entityProperties, out IDictionary<string, EntityProperty> _);
      return entityProperties;
    }

    internal static string GetPropertyName(string propertyName, bool enableTimestampQuery = false)
    {
      if (string.Equals(propertyName, "RowKey", StringComparison.Ordinal))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}['{1}']", (object) EdmSchemaMapping.EntityName, (object) "$id");
      if (string.Equals(propertyName, "PartitionKey", StringComparison.Ordinal))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}['{1}']", (object) EdmSchemaMapping.EntityName, (object) "$pk");
      return string.Equals(propertyName, "Timestamp", StringComparison.Ordinal) & enableTimestampQuery ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}['{1}']", (object) EdmSchemaMapping.EntityName, (object) "_ts") : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}['{1}']['$v']", (object) EdmSchemaMapping.EntityName, (object) propertyName);
    }

    internal static string SerializeObject(object value)
    {
      StringWriter stringWriter = new StringWriter(new StringBuilder(256), (IFormatProvider) CultureInfo.InvariantCulture);
      using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) stringWriter))
      {
        jsonTextWriter.Formatting = EntityTranslator.JsonSerializer.Formatting;
        EntityTranslator.JsonSerializer.Serialize((JsonWriter) jsonTextWriter, value, (Type) null);
      }
      return stringWriter.ToString();
    }

    internal static TObject DeserializeObject<TObject>(string value)
    {
      using (JsonTextReader reader = new JsonTextReader((TextReader) new StringReader(value)))
      {
        reader.DateParseHandling = DateParseHandling.None;
        return (TObject) EntityTranslator.JsonSerializer.Deserialize((JsonReader) reader, typeof (TObject));
      }
    }
  }
}
