// Decompiled with JetBrains decompiler
// Type: Nest.ChainInputFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class ChainInputFormatter : IJsonFormatter<IChainInput>, IJsonFormatter
  {
    public IChainInput Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (IChainInput) null;
      }
      reader.ReadNext();
      reader.ReadNext();
      reader.ReadNext();
      int count = 0;
      Dictionary<string, InputContainer> inputs = new Dictionary<string, InputContainer>();
      IJsonFormatter<InputContainer> formatter = formatterResolver.GetFormatter<InputContainer>();
      while (reader.ReadIsInArray(ref count))
      {
        reader.ReadNext();
        string key = reader.ReadPropertyName();
        InputContainer inputContainer = formatter.Deserialize(ref reader, formatterResolver);
        reader.ReadNext();
        inputs.Add(key, inputContainer);
      }
      reader.ReadNext();
      return (IChainInput) new ChainInput((IDictionary<string, InputContainer>) inputs);
    }

    public void Serialize(
      ref JsonWriter writer,
      IChainInput value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.Inputs == null)
        return;
      writer.WriteBeginObject();
      writer.WritePropertyName("inputs");
      writer.WriteBeginArray();
      int num = 0;
      IJsonFormatter<IInputContainer> formatter = formatterResolver.GetFormatter<IInputContainer>();
      foreach (KeyValuePair<string, InputContainer> input in (IEnumerable<KeyValuePair<string, InputContainer>>) value.Inputs)
      {
        if (num > 0)
          writer.WriteValueSeparator();
        writer.WriteBeginObject();
        writer.WritePropertyName(input.Key);
        formatter.Serialize(ref writer, (IInputContainer) input.Value, formatterResolver);
        writer.WriteEndObject();
        ++num;
      }
      writer.WriteEndArray();
      writer.WriteEndObject();
    }
  }
}
