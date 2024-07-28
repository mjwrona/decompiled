// Decompiled with JetBrains decompiler
// Type: Nest.SuggestDictionaryFormatter`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class SuggestDictionaryFormatter<T> : 
    IJsonFormatter<ISuggestDictionary<T>>,
    IJsonFormatter
    where T : class
  {
    public ISuggestDictionary<T> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return (ISuggestDictionary<T>) new SuggestDictionary<T>((IReadOnlyDictionary<string, ISuggest<T>[]>) formatterResolver.GetFormatter<Dictionary<string, ISuggest<T>[]>>().Deserialize(ref reader, formatterResolver));
    }

    public void Serialize(
      ref JsonWriter writer,
      ISuggestDictionary<T> value,
      IJsonFormatterResolver formatterResolver)
    {
      new VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, ISuggest<T>[]>().Serialize(ref writer, (IReadOnlyDictionary<string, ISuggest<T>[]>) value, formatterResolver);
    }
  }
}
