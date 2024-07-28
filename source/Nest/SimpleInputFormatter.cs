// Decompiled with JetBrains decompiler
// Type: Nest.SimpleInputFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class SimpleInputFormatter : IJsonFormatter<ISimpleInput>, IJsonFormatter
  {
    public ISimpleInput Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) => reader.GetCurrentJsonToken() != JsonToken.BeginObject ? (ISimpleInput) null : (ISimpleInput) new SimpleInput(formatterResolver.GetFormatter<IDictionary<string, object>>().Deserialize(ref reader, formatterResolver));

    public void Serialize(
      ref JsonWriter writer,
      ISimpleInput value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.Payload == null)
        return;
      formatterResolver.GetFormatter<IDictionary<string, object>>().Serialize(ref writer, value.Payload, formatterResolver);
    }
  }
}
