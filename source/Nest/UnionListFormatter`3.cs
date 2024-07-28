// Decompiled with JetBrains decompiler
// Type: Nest.UnionListFormatter`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class UnionListFormatter<TCollection, TFirst, TSecond> : 
    IJsonFormatter<TCollection>,
    IJsonFormatter
    where TCollection : List<Union<TFirst, TSecond>>, new()
  {
    private static readonly UnionFormatter<TFirst, TSecond> CharFilterFormatter = new UnionFormatter<TFirst, TSecond>();

    public TCollection Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.ReadIsNull())
        return default (TCollection);
      int count = 0;
      TCollection collection = new TCollection();
      reader.ReadIsBeginArrayWithVerify();
      while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
        collection.Add(UnionListFormatter<TCollection, TFirst, TSecond>.CharFilterFormatter.Deserialize(ref reader, formatterResolver));
      return collection;
    }

    public void Serialize(
      ref JsonWriter writer,
      TCollection value,
      IJsonFormatterResolver formatterResolver)
    {
      if ((object) value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginArray();
        if (value.Count != 0)
          UnionListFormatter<TCollection, TFirst, TSecond>.CharFilterFormatter.Serialize(ref writer, value[0], formatterResolver);
        for (int index = 1; index < value.Count; ++index)
        {
          writer.WriteValueSeparator();
          UnionListFormatter<TCollection, TFirst, TSecond>.CharFilterFormatter.Serialize(ref writer, value[index], formatterResolver);
        }
        writer.WriteEndArray();
      }
    }
  }
}
