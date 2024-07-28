// Decompiled with JetBrains decompiler
// Type: Nest.DictionaryResponseFormatter`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class DictionaryResponseFormatter<TResponse, TKey, TValue> : 
    IJsonFormatter<TResponse>,
    IJsonFormatter
    where TResponse : ResponseBase, IDictionaryResponse<TKey, TValue>, new()
  {
    public TResponse Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      TResponse response = new TResponse();
      IJsonFormatter<TKey> formatter1 = formatterResolver.GetFormatter<TKey>();
      IJsonFormatter<TValue> formatter2 = formatterResolver.GetFormatter<TValue>();
      Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
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
          TKey key = formatter1.Deserialize(ref reader1, formatterResolver);
          TValue obj = formatter2.Deserialize(ref reader, formatterResolver);
          dictionary.Add(key, obj);
        }
      }
      response.BackingDictionary = (IReadOnlyDictionary<TKey, TValue>) dictionary;
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
