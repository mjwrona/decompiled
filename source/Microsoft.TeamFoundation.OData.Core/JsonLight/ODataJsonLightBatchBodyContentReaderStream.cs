// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightBatchBodyContentReaderStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightBatchBodyContentReaderStream : MemoryStream
  {
    private IODataStreamListener listener;
    private string cachedBodyContent;

    internal ODataJsonLightBatchBodyContentReaderStream(IODataStreamListener listener) => this.listener = listener;

    internal bool PopulateBodyContent(IJsonReader jsonReader, string contentTypeHeader)
    {
      bool flag = false;
      ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType? nullable = ODataJsonLightBatchBodyContentReaderStream.DetectBatchPayloadBodyContentType(jsonReader, contentTypeHeader);
      if (!nullable.HasValue)
      {
        this.cachedBodyContent = jsonReader.ReadStringValue();
      }
      else
      {
        if (nullable.HasValue)
        {
          switch (nullable.GetValueOrDefault())
          {
            case ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType.Json:
              this.WriteJsonContent(jsonReader);
              break;
            case ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType.Textual:
              this.WriteBytes(Encoding.UTF8.GetBytes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
              {
                (object) jsonReader.ReadStringValue()
              })));
              break;
            case ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType.Binary:
              this.WriteBinaryContent(jsonReader.ReadStringValue());
              break;
            default:
              goto label_7;
          }
          flag = true;
          goto label_9;
        }
label_7:
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "unknown / undefined type, new type that needs to be supported: {0}? ", new object[1]
        {
          (object) nullable
        }));
      }
label_9:
      return flag;
    }

    internal void PopulateCachedBodyContent(string contentTypeHeader)
    {
      ODataMediaType mediaType = ODataJsonLightBatchBodyContentReaderStream.GetMediaType(contentTypeHeader);
      if (mediaType != null && mediaType.Type.Equals("text"))
        this.WriteBytes(Encoding.UTF8.GetBytes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
        {
          (object) this.cachedBodyContent
        })));
      else
        this.WriteBinaryContent(this.cachedBodyContent);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.listener != null)
      {
        this.listener.StreamDisposed();
        this.listener = (IODataStreamListener) null;
      }
      base.Dispose(disposing);
    }

    private static ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType? DetectBatchPayloadBodyContentType(
      IJsonReader jsonReader,
      string contentTypeHeader)
    {
      ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType? nullable = new ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType?();
      ODataMediaType mediaType = ODataJsonLightBatchBodyContentReaderStream.GetMediaType(contentTypeHeader);
      if (jsonReader.NodeType == JsonNodeType.StartObject)
        nullable = new ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType?(ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType.Json);
      else if (jsonReader.NodeType == JsonNodeType.PrimitiveValue && (mediaType != null || contentTypeHeader != null))
        nullable = mediaType == null || !mediaType.Type.Equals("text") ? new ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType?(ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType.Binary) : new ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType?(ODataJsonLightBatchBodyContentReaderStream.BatchPayloadBodyContentType.Textual);
      return nullable;
    }

    private static ODataMediaType GetMediaType(string contentTypeHeader)
    {
      if (string.IsNullOrEmpty(contentTypeHeader))
        return (ODataMediaType) null;
      contentTypeHeader = contentTypeHeader.Trim();
      int length1 = contentTypeHeader.IndexOf(';');
      string str = length1 != -1 ? contentTypeHeader.Substring(0, length1) : contentTypeHeader;
      int length2 = str.IndexOf('/');
      string subType = (string) null;
      string type;
      if (length2 != -1)
      {
        type = str.Substring(0, length2);
        subType = str.Substring(length2 + 1);
      }
      else
        type = str;
      return new ODataMediaType(type, subType);
    }

    private void WriteJsonContent(IJsonReader reader)
    {
      IJsonWriter jsonWriter = (IJsonWriter) new JsonWriter((TextWriter) new StreamWriter((Stream) this), true);
      ODataJsonLightBatchBodyContentReaderStream.WriteCurrentJsonObject(reader, jsonWriter);
      this.Flush();
      this.Position = 0L;
    }

    private void WriteBinaryContent(string encodedContent) => this.WriteBytes(Convert.FromBase64String(encodedContent.Replace('-', '+').Replace('_', '/')));

    private void WriteBytes(byte[] bytes)
    {
      this.Write(bytes, 0, bytes.Length);
      this.Flush();
      this.Position = 0L;
    }

    private static void WriteCurrentJsonObject(IJsonReader reader, IJsonWriter jsonWriter)
    {
      Stack<JsonNodeType> jsonNodeTypeStack = new Stack<JsonNodeType>();
      do
      {
        switch (reader.NodeType)
        {
          case JsonNodeType.StartObject:
            jsonNodeTypeStack.Push(reader.NodeType);
            jsonWriter.StartObjectScope();
            break;
          case JsonNodeType.EndObject:
            int num1 = (int) jsonNodeTypeStack.Pop();
            jsonWriter.EndObjectScope();
            break;
          case JsonNodeType.StartArray:
            jsonNodeTypeStack.Push(reader.NodeType);
            jsonWriter.StartArrayScope();
            break;
          case JsonNodeType.EndArray:
            int num2 = (int) jsonNodeTypeStack.Pop();
            jsonWriter.EndArrayScope();
            break;
          case JsonNodeType.Property:
            jsonWriter.WriteName(reader.Value.ToString());
            break;
          case JsonNodeType.PrimitiveValue:
            if (reader.Value != null)
            {
              jsonWriter.WritePrimitiveValue(reader.Value);
              break;
            }
            jsonWriter.WriteValue((string) null);
            break;
          default:
            throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected reader.NodeType: {0}.", new object[1]
            {
              (object) reader.NodeType
            }));
        }
        int num3 = (int) reader.ReadNext();
      }
      while (jsonNodeTypeStack.Count != 0);
      jsonWriter.Flush();
    }

    private enum BatchPayloadBodyContentType
    {
      Json,
      Textual,
      Binary,
    }
  }
}
