// Decompiled with JetBrains decompiler
// Type: Nest.SerializeAsSingleFormatter`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class SerializeAsSingleFormatter<T> : IJsonFormatter<IEnumerable<T>>, IJsonFormatter
  {
    public IEnumerable<T> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.BeginArray)
        return formatterResolver.GetFormatter<IEnumerable<T>>().Deserialize(ref reader, formatterResolver);
      return (IEnumerable<T>) new T[1]
      {
        formatterResolver.GetFormatter<T>().Deserialize(ref reader, formatterResolver)
      };
    }

    public void Serialize(
      ref JsonWriter writer,
      IEnumerable<T> value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        IEnumerator<T> enumerator = value.GetEnumerator();
        if (!enumerator.MoveNext())
        {
          writer.WriteNull();
        }
        else
        {
          T current = enumerator.Current;
          formatterResolver.GetFormatter<T>().Serialize(ref writer, current, formatterResolver);
        }
      }
    }
  }
}
