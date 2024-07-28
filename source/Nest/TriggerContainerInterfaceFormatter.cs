// Decompiled with JetBrains decompiler
// Type: Nest.TriggerContainerInterfaceFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;

namespace Nest
{
  internal class TriggerContainerInterfaceFormatter : 
    IJsonFormatter<ITriggerContainer>,
    IJsonFormatter
  {
    public ITriggerContainer Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return (ITriggerContainer) formatterResolver.GetFormatter<TriggerContainer>().Deserialize(ref reader, formatterResolver);
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      ITriggerContainer value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<ITriggerContainer>().Serialize(ref writer, value, formatterResolver);
    }
  }
}
