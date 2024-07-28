// Decompiled with JetBrains decompiler
// Type: Nest.VerbatimDictionaryKeysFormatter`4
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class VerbatimDictionaryKeysFormatter<TDictionary, TInterface, TKey, TValue> : 
    IJsonFormatter<TInterface>,
    IJsonFormatter
    where TDictionary : TInterface, IIsADictionary<TKey, TValue>
    where TInterface : IIsADictionary<TKey, TValue>
  {
    private static readonly VerbatimDictionaryInterfaceKeysFormatter<TKey, TValue> DictionaryFormatter = new VerbatimDictionaryInterfaceKeysFormatter<TKey, TValue>();

    public void Serialize(
      ref JsonWriter writer,
      TInterface value,
      IJsonFormatterResolver formatterResolver)
    {
      VerbatimDictionaryKeysFormatter<TDictionary, TInterface, TKey, TValue>.DictionaryFormatter.Serialize(ref writer, (IDictionary<TKey, TValue>) value, formatterResolver);
    }

    public TInterface Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => (TInterface) typeof (TDictionary).CreateInstance<TDictionary>((object) VerbatimDictionaryKeysFormatter<TDictionary, TInterface, TKey, TValue>.DictionaryFormatter.Deserialize(ref reader, formatterResolver));
  }
}
