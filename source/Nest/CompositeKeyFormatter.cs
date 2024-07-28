// Decompiled with JetBrains decompiler
// Type: Nest.CompositeKeyFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class CompositeKeyFormatter : IJsonFormatter<CompositeKey>, IJsonFormatter
  {
    private static readonly VerbatimInterfaceReadOnlyDictionaryKeysPreservingNullFormatter<string, object> DictionaryFormatter = new VerbatimInterfaceReadOnlyDictionaryKeysPreservingNullFormatter<string, object>();

    public void Serialize(
      ref JsonWriter writer,
      CompositeKey value,
      IJsonFormatterResolver formatterResolver)
    {
      CompositeKeyFormatter.DictionaryFormatter.Serialize(ref writer, (IReadOnlyDictionary<string, object>) value, formatterResolver);
    }

    public CompositeKey Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => reader.ReadIsNull() ? (CompositeKey) null : new CompositeKey(CompositeKeyFormatter.DictionaryFormatter.Deserialize(ref reader, formatterResolver));
  }
}
