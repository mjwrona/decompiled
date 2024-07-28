// Decompiled with JetBrains decompiler
// Type: Nest.DynamicResponseFormatter`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class DynamicResponseFormatter<TResponse> : IJsonFormatter<TResponse>, IJsonFormatter where TResponse : ResponseBase, IDynamicResponse, new()
  {
    public TResponse Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      TResponse response = new TResponse();
      IJsonFormatter<string> formatter1 = formatterResolver.GetFormatter<string>();
      IJsonFormatter<object> formatter2 = formatterResolver.GetFormatter<object>();
      Dictionary<string, object> values = new Dictionary<string, object>();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (ResponseFormatterHelpers.ServerErrorFields.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              if (reader.GetCurrentJsonToken() == JsonToken.String)
              {
                // ISSUE: variable of a boxed type
                __Boxed<TResponse> local = (object) response;
                Error error = new Error();
                error.Reason = reader.ReadString();
                local.Error = error;
                continue;
              }
              IJsonFormatter<Error> formatter3 = formatterResolver.GetFormatter<Error>();
              response.Error = formatter3.Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              if (reader.GetCurrentJsonToken() == JsonToken.Number)
              {
                response.StatusCode = new int?(reader.ReadInt32());
                continue;
              }
              reader.ReadNextBlock();
              continue;
            default:
              continue;
          }
        }
        else
        {
          JsonReader reader1 = new JsonReader(bytes.Array, bytes.Offset - 1);
          string key = formatter1.Deserialize(ref reader1, formatterResolver);
          object obj = formatter2.Deserialize(ref reader, formatterResolver);
          values.Add(key, obj);
        }
      }
      response.BackingDictionary = DynamicDictionary.Create((IDictionary<string, object>) values);
      return response;
    }

    public void Serialize(
      ref JsonWriter writer,
      TResponse value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }
  }
}
