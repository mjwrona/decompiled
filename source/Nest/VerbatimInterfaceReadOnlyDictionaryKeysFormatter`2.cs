// Decompiled with JetBrains decompiler
// Type: Nest.VerbatimInterfaceReadOnlyDictionaryKeysFormatter`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class VerbatimInterfaceReadOnlyDictionaryKeysFormatter<TKey, TValue> : 
    VerbatimDictionaryKeysBaseFormatter<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>
  {
    public override IReadOnlyDictionary<TKey, TValue> Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return (IReadOnlyDictionary<TKey, TValue>) formatterResolver.GetFormatter<Dictionary<TKey, TValue>>().Deserialize(ref reader, formatterResolver);
    }
  }
}
