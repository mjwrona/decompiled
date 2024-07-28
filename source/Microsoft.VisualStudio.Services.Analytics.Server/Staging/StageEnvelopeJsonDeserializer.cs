// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Staging.StageEnvelopeJsonDeserializer
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Analytics.Staging
{
  public class StageEnvelopeJsonDeserializer
  {
    private const string c_traceLayer = "StageEnvelopeDeserializer";

    public StageEnvelope Deserialize(
      IVssRequestContext requestContext,
      Stream stream,
      IDictionary<int, SqlMetaData[]> schemas,
      SqlMetaData[] extendedSchema = null)
    {
      JsonTextReader jsonTextReader = new JsonTextReader((TextReader) new StreamReader(stream));
      jsonTextReader.CloseInput = false;
      using (JsonTextReader reader = jsonTextReader)
      {
        StageEnvelope stageEnvelope = this.ReadEnvelope(requestContext, reader, schemas, extendedSchema);
        if (reader.TokenType != JsonToken.None)
          throw this.CreateException(reader, AnalyticsResources.UNEXPECTED_TOKEN((object) reader.TokenType));
        return stageEnvelope;
      }
    }

    private StageEnvelope ReadEnvelope(
      IVssRequestContext requestContext,
      JsonTextReader reader,
      IDictionary<int, SqlMetaData[]> schemas,
      SqlMetaData[] extendedSchema)
    {
      StageEnvelope envelope = new StageEnvelope();
      int? nullable = new int?();
      reader.Read();
      this.ParseToken(reader, JsonToken.StartObject);
      while (reader.TokenType != JsonToken.EndObject)
      {
        string propertyName = this.GetPropertyName(reader);
        if (propertyName.Equals("contentVersion", StringComparison.InvariantCultureIgnoreCase))
          envelope.ContentVersion = this.ParsePropertyInt32(reader, "contentVersion");
        else if (propertyName.Equals("stageVersion", StringComparison.InvariantCultureIgnoreCase))
        {
          envelope.ProviderStageVersion = this.ParsePropertyInt32(reader, "stageVersion");
          nullable = new int?(envelope.ProviderStageVersion);
        }
        else if (propertyName.Equals("fromWatermark", StringComparison.InvariantCultureIgnoreCase))
          envelope.FromWatermark = this.ParsePropertyString(reader, "fromWatermark");
        else if (propertyName.Equals("toWatermark", StringComparison.InvariantCultureIgnoreCase))
          envelope.ToWatermark = this.ParsePropertyString(reader, "toWatermark");
        else if (propertyName.Equals("isCurrent", StringComparison.InvariantCultureIgnoreCase))
          envelope.IsCurrent = new bool?(this.ParsePropertyBoolean(reader, "isCurrent"));
        else if (propertyName.Equals("syncDate", StringComparison.InvariantCultureIgnoreCase))
          envelope.SyncDate = new DateTime?(this.ParsePropertyDateTime(reader, "syncDate"));
        else if (propertyName.Equals("replace", StringComparison.InvariantCultureIgnoreCase))
          envelope.Replace = new bool?(this.ParsePropertyBoolean(reader, "replace"));
        else if (propertyName.Equals("keysOnly", StringComparison.InvariantCultureIgnoreCase))
        {
          envelope.KeysOnly = new bool?(this.ParsePropertyBoolean(reader, "keysOnly"));
        }
        else
        {
          if (!propertyName.Equals("records", StringComparison.InvariantCultureIgnoreCase))
            throw this.CreateException(reader, AnalyticsResources.UNSUPPORTED_PROPERTY((object) propertyName));
          if (!nullable.HasValue)
            throw new InvalidOperationException("No StageVersion supplied");
          envelope.StageVersion = schemas.ContainsKey(nullable.Value) ? nullable.Value : schemas.Keys.Max();
          SqlMetaData[] schema = schemas[envelope.StageVersion];
          this.ParseRecords(requestContext, reader, "records", envelope, schema, extendedSchema);
        }
      }
      this.ParseToken(reader, JsonToken.EndObject);
      return envelope;
    }

    private Dictionary<string, int> CreateOrdinalMap(SqlMetaData[] schema)
    {
      Dictionary<string, int> ordinalMap = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      for (int index = 0; index < schema.Length; ++index)
        ordinalMap.Add(schema[index].Name, index);
      return ordinalMap;
    }

    private void ParseRecords(
      IVssRequestContext requestContext,
      JsonTextReader reader,
      string name,
      StageEnvelope envelope,
      SqlMetaData[] schema,
      SqlMetaData[] extendedSchema)
    {
      envelope.Records = (ICollection<SqlDataRecord>) new List<SqlDataRecord>();
      envelope.ExtendedFields = (ICollection<SqlDataRecord>) new List<SqlDataRecord>();
      this.CheckPropertyName(reader, name, false);
      Dictionary<string, int> dictionary = (Dictionary<string, int>) null;
      Dictionary<string, int> ordinalMap1 = this.CreateOrdinalMap(schema);
      Dictionary<string, int> ordinalMap2 = extendedSchema != null ? this.CreateOrdinalMap(extendedSchema) : (Dictionary<string, int>) null;
      int num = 0;
      reader.Read();
      this.ParseToken(reader, JsonToken.StartArray);
      while (reader.TokenType == JsonToken.StartObject)
      {
        List<string> failedProperties;
        this.ParseRecord(requestContext, reader, num++, envelope.Records, envelope.ExtendedFields, schema, extendedSchema, ordinalMap1, ordinalMap2, out failedProperties);
        if (failedProperties != null && failedProperties.Count > 0)
        {
          dictionary = new Dictionary<string, int>();
          foreach (string key1 in failedProperties)
            dictionary.AddOrUpdate<string, int>(key1, 1, (Func<string, int, int>) ((key, val) => val + 1));
          envelope.PropertyParseErrorCounts = dictionary;
        }
      }
      envelope.PropertyParseErrorCounts = dictionary;
      this.ParseToken(reader, JsonToken.EndArray);
    }

    private void ParseRecord(
      IVssRequestContext requestContext,
      JsonTextReader reader,
      int recordNumber,
      ICollection<SqlDataRecord> records,
      ICollection<SqlDataRecord> extendedFields,
      SqlMetaData[] schema,
      SqlMetaData[] extendedSchema,
      Dictionary<string, int> ordinalMap,
      Dictionary<string, int> extendedOrdinalMap,
      out List<string> failedProperties)
    {
      this.ParseToken(reader, JsonToken.StartObject);
      failedProperties = (List<string>) null;
      SqlDataRecord record = new SqlDataRecord(schema);
      records.Add(record);
      int ordinal1;
      if (ordinalMap.TryGetValue("AxRecordNumber", out ordinal1))
        record.SetInt32(ordinal1, recordNumber);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      while (reader.TokenType == JsonToken.PropertyName)
      {
        string str = (string) reader.Value;
        int ordinal2 = 0;
        if (!stringSet.Add(str))
          throw this.CreateException(reader, AnalyticsResources.PROPERTY_ALREADY_DEFINED((object) str));
        if (ordinalMap.TryGetValue(str, out ordinal2))
        {
          try
          {
            this.ReadField(reader, schema, record, ordinal2, str, requestContext);
          }
          catch (JsonReaderException ex) when (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          {
            requestContext.TraceAlways(12010003, TraceLevel.Warning, "AnalyticsStaging", "StageEnvelopeDeserializer", AnalyticsResources.STAGE_ENVELOPE_PARSE_FIELD_IGNORED((object) str, (object) ex.Message));
            reader.Read();
            if (failedProperties == null)
              failedProperties = new List<string>();
            failedProperties.Add(str);
          }
        }
        else
          this.ReadExtendedField(reader, extendedFields, extendedSchema, extendedOrdinalMap, str, recordNumber);
      }
      this.ParseToken(reader, JsonToken.EndObject);
    }

    private void ReadField(
      JsonTextReader reader,
      SqlMetaData[] schema,
      SqlDataRecord record,
      int ordinal,
      string propertyName,
      IVssRequestContext requestContext)
    {
      Guid result = new Guid();
      switch (schema[ordinal].SqlDbType)
      {
        case SqlDbType.BigInt:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this.ReadPropertyValue<long>(reader, ordinal, new Action<int, long>(record.SetInt64), StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C1\u003E__ToInt64 ?? (StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C1\u003E__ToInt64 = new Func<object, IFormatProvider, long>(Convert.ToInt64)), JsonToken.Integer);
          break;
        case SqlDbType.Bit:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this.ReadPropertyValue<bool>(reader, ordinal, new Action<int, bool>(record.SetBoolean), StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C2\u003E__ToBoolean ?? (StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C2\u003E__ToBoolean = new Func<object, IFormatProvider, bool>(Convert.ToBoolean)), JsonToken.Boolean);
          break;
        case SqlDbType.DateTime:
          this.ReadPropertyNullableValue<DateTime>(reader, ordinal, new Action<int, DateTime>(record.SetDateTime), new Func<DateTime?>(((JsonReader) reader).ReadAsDateTime));
          break;
        case SqlDbType.Decimal:
          this.ReadPropertyNullableValue<Decimal>(reader, ordinal, new Action<int, Decimal>(record.SetDecimal), new Func<Decimal?>(((JsonReader) reader).ReadAsDecimal));
          break;
        case SqlDbType.Float:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this.ReadPropertyValue<double>(reader, ordinal, new Action<int, double>(record.SetDouble), StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C5\u003E__ToDouble ?? (StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C5\u003E__ToDouble = new Func<object, IFormatProvider, double>(Convert.ToDouble)), JsonToken.Float, JsonToken.Integer);
          break;
        case SqlDbType.Int:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this.ReadPropertyValue<int>(reader, ordinal, new Action<int, int>(record.SetInt32), StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Func<object, IFormatProvider, int>(Convert.ToInt32)), JsonToken.Integer);
          break;
        case SqlDbType.NVarChar:
          reader.DateParseHandling = DateParseHandling.None;
          reader.Read();
          string str = reader.TokenType != JsonToken.StartObject ? (reader.TokenType != JsonToken.StartArray ? reader.Value?.ToString() : JArray.Load((JsonReader) reader).ToString()) : JObject.Load((JsonReader) reader).ToString();
          if (str != null)
          {
            record.SetString(ordinal, str);
            break;
          }
          break;
        case SqlDbType.UniqueIdentifier:
          string input = reader.ReadAsString();
          if (input != null)
          {
            if (!Guid.TryParse(input, out result))
              throw this.CreateException(reader, AnalyticsResources.ERROR_READING_GUID((object) input));
            record.SetGuid(ordinal, result);
            break;
          }
          break;
        case SqlDbType.SmallInt:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this.ReadPropertyValue<short>(reader, ordinal, new Action<int, short>(record.SetInt16), StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C4\u003E__ToInt16 ?? (StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C4\u003E__ToInt16 = new Func<object, IFormatProvider, short>(Convert.ToInt16)), JsonToken.Bytes, JsonToken.Integer);
          break;
        case SqlDbType.TinyInt:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this.ReadPropertyValue<byte>(reader, ordinal, new Action<int, byte>(record.SetByte), StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C3\u003E__ToByte ?? (StageEnvelopeJsonDeserializer.\u003C\u003EO.\u003C3\u003E__ToByte = new Func<object, IFormatProvider, byte>(Convert.ToByte)), JsonToken.Bytes, JsonToken.Integer);
          break;
        case SqlDbType.Xml:
          this.ReadXmlPropertyValue(reader, propertyName, ordinal, record, requestContext);
          break;
        case SqlDbType.DateTimeOffset:
          this.ReadPropertyNullableValue<DateTimeOffset>(reader, ordinal, new Action<int, DateTimeOffset>(record.SetDateTimeOffset), new Func<DateTimeOffset?>(((JsonReader) reader).ReadAsDateTimeOffset));
          break;
        default:
          throw this.CreateException(reader, AnalyticsResources.UNSUPPORTED_TYPE_FOR_CUSTOM_JSON_SERIALIZER((object) schema[ordinal].SqlDbType));
      }
      reader.Read();
    }

    private void ReadExtendedField(
      JsonTextReader reader,
      ICollection<SqlDataRecord> extendedFields,
      SqlMetaData[] schema,
      Dictionary<string, int> ordinalMap,
      string propertyName,
      int recordNumber)
    {
      reader.DateParseHandling = DateParseHandling.DateTimeOffset;
      reader.Read();
      if (reader.TokenType != JsonToken.Null && schema != null)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(schema);
        int ordinal = 0;
        extendedFields.Add(sqlDataRecord);
        if (ordinalMap.TryGetValue("AxRecordNumber", out ordinal))
          sqlDataRecord.SetInt32(ordinal, recordNumber);
        if (ordinalMap.TryGetValue("FieldName", out ordinal))
          sqlDataRecord.SetString(ordinal, propertyName);
        switch (reader.TokenType)
        {
          case JsonToken.StartObject:
            JObject jobject = JObject.Load((JsonReader) reader);
            if (ordinalMap.TryGetValue("ValueObject", out ordinal))
            {
              sqlDataRecord.SetString(ordinal, jobject.ToString());
              break;
            }
            break;
          case JsonToken.StartArray:
            JArray jarray = JArray.Load((JsonReader) reader);
            if (ordinalMap.TryGetValue("ValueObject", out ordinal))
            {
              sqlDataRecord.SetString(ordinal, jarray.ToString());
              break;
            }
            break;
          case JsonToken.Integer:
            if (ordinalMap.TryGetValue("ValueInt", out ordinal))
            {
              sqlDataRecord.SetInt64(ordinal, Convert.ToInt64(reader.Value));
              break;
            }
            break;
          case JsonToken.Float:
            if (ordinalMap.TryGetValue("ValueFloat", out ordinal))
            {
              sqlDataRecord.SetDouble(ordinal, Convert.ToDouble(reader.Value));
              break;
            }
            break;
          case JsonToken.String:
            string input = (string) reader.Value;
            Guid result = new Guid();
            if (Guid.TryParseExact(input, "d", out result))
            {
              if (ordinalMap.TryGetValue("ValueGuid", out ordinal))
              {
                sqlDataRecord.SetGuid(ordinal, result);
                break;
              }
              break;
            }
            if (ordinalMap.TryGetValue("ValueString", out ordinal))
            {
              sqlDataRecord.SetString(ordinal, input);
              break;
            }
            break;
          case JsonToken.Boolean:
            if (ordinalMap.TryGetValue("ValueInt", out ordinal))
            {
              sqlDataRecord.SetInt64(ordinal, (long) Convert.ToBoolean(reader.Value));
              break;
            }
            break;
          case JsonToken.Date:
            if (ordinalMap.TryGetValue("ValueDatetime", out ordinal))
            {
              if (reader.ValueType == typeof (DateTimeOffset))
              {
                sqlDataRecord.SetDateTimeOffset(ordinal, (DateTimeOffset) reader.Value);
                break;
              }
              sqlDataRecord.SetDateTimeOffset(ordinal, new DateTimeOffset(Convert.ToDateTime(reader.Value).ToUniversalTime()));
              break;
            }
            break;
          default:
            throw this.CreateException(reader, AnalyticsResources.UNEXPECTED_TOKEN((object) reader.TokenType));
        }
      }
      reader.Read();
    }

    private void ReadPropertyValue<T>(
      JsonTextReader reader,
      int ordinal,
      Action<int, T> set,
      Func<object, IFormatProvider, T> convert,
      JsonToken tokenType)
    {
      reader.Read();
      if (reader.TokenType == tokenType)
        set(ordinal, convert(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture));
      else if (reader.TokenType != JsonToken.Null)
        throw this.CreateException(reader, AnalyticsResources.UNEXPECTED_TOKEN((object) reader.TokenType));
    }

    private void ReadPropertyValue<T>(
      JsonTextReader reader,
      int ordinal,
      Action<int, T> set,
      Func<object, IFormatProvider, T> convert,
      JsonToken tokenType1,
      JsonToken tokenType2)
    {
      reader.Read();
      if (reader.TokenType == tokenType1 || reader.TokenType == tokenType2)
        set(ordinal, convert(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture));
      else if (reader.TokenType != JsonToken.Null)
        throw this.CreateException(reader, AnalyticsResources.UNEXPECTED_TOKEN((object) reader.TokenType));
    }

    private void ReadPropertyNullableValue<T>(
      JsonTextReader reader,
      int ordinal,
      Action<int, T> set,
      Func<T?> read)
      where T : struct
    {
      T? nullable = read();
      if (!nullable.HasValue)
        return;
      set(ordinal, nullable.Value);
    }

    private void ReadXmlPropertyValue(
      JsonTextReader reader,
      string propertyName,
      int ordinal,
      SqlDataRecord record,
      IVssRequestContext requestContext)
    {
      reader.Read();
      if (reader.TokenType == JsonToken.StartArray)
      {
        XmlDocument node = JsonConvert.DeserializeXmlNode(new JObject((object) new JProperty("Item", (object) JArray.Load((JsonReader) reader))).ToString(), propertyName);
        if (requestContext.IsFeatureEnabled("Analytics.Stage.SpecialCharacter"))
        {
          Regex regex = new Regex("&#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|[19][0-9A-F]|7F|8[0-46-9A-F]|0?[1-8BCEF])", RegexOptions.IgnoreCase);
          if (regex.IsMatch(node.InnerXml))
            node.InnerXml = regex.Replace(node.InnerXml, string.Empty);
        }
        using (XmlNodeReader xmlNodeReader = new XmlNodeReader((XmlNode) node))
        {
          SqlXml sqlXml = new SqlXml((XmlReader) xmlNodeReader);
          record.SetSqlXml(ordinal, sqlXml);
        }
      }
      else
      {
        if (reader.TokenType != JsonToken.StartObject)
          throw this.CreateException(reader, AnalyticsResources.UNEXPECTED_TOKEN((object) reader.TokenType));
        using (XmlNodeReader xmlNodeReader = new XmlNodeReader((XmlNode) JsonConvert.DeserializeXmlNode(JObject.Load((JsonReader) reader).ToString(), propertyName)))
        {
          SqlXml sqlXml = new SqlXml((XmlReader) xmlNodeReader);
          record.SetSqlXml(ordinal, sqlXml);
        }
      }
    }

    private string ParsePropertyString(JsonTextReader reader, string name)
    {
      this.CheckPropertyName(reader, name, false);
      string propertyString = reader.ReadAsString();
      reader.Read();
      return propertyString;
    }

    private bool ParsePropertyBoolean(
      JsonTextReader reader,
      string name,
      bool optional = true,
      bool defaultValue = false)
    {
      if (!this.CheckPropertyName(reader, name, optional))
        return defaultValue;
      reader.Read();
      return Convert.ToBoolean(this.ParseToken(reader, JsonToken.Boolean));
    }

    private DateTime ParsePropertyDateTime(JsonTextReader reader, string name)
    {
      this.CheckPropertyName(reader, name, false);
      reader.Read();
      return Convert.ToDateTime(this.ParseToken(reader, JsonToken.Date)).ToUniversalTime();
    }

    private int ParsePropertyInt32(
      JsonTextReader reader,
      string name,
      bool optional = true,
      int defaultValue = 0)
    {
      if (!this.CheckPropertyName(reader, name, optional))
        return defaultValue;
      reader.Read();
      return Convert.ToInt32(this.ParseToken(reader, JsonToken.Integer));
    }

    private string GetPropertyName(JsonTextReader reader) => reader.TokenType == JsonToken.PropertyName ? reader.Value as string : throw this.CreateException(reader, AnalyticsResources.UNEXPECTED_TOKEN((object) reader.TokenType));

    private bool CheckPropertyName(JsonTextReader reader, string name, bool optional)
    {
      string propertyName = this.GetPropertyName(reader);
      if (propertyName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        return true;
      if (optional)
        return false;
      throw this.CreateException(reader, AnalyticsResources.UNEXPECTED_TOKEN((object) propertyName));
    }

    public object ParseToken(JsonTextReader reader, JsonToken tokenType)
    {
      if (reader.TokenType != tokenType)
        throw this.CreateException(reader, AnalyticsResources.UNEXPECTED_TOKEN((object) reader.TokenType));
      object token = reader.Value;
      reader.Read();
      return token;
    }

    private JsonReaderException CreateException(JsonTextReader reader, string message) => new JsonReaderException(message + Environment.NewLine + AnalyticsResources.PATH() + ": " + reader.Path + string.Format("{0}{1}: {2}", (object) Environment.NewLine, (object) AnalyticsResources.LINE_NUMBER(), (object) reader.LineNumber) + string.Format("{0}{1}: {2}", (object) Environment.NewLine, (object) AnalyticsResources.LINE_POSITION(), (object) reader.LinePosition));
  }
}
