// Decompiled with JetBrains decompiler
// Type: Nest.SimulatedActionsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  internal class SimulatedActionsFormatter : IJsonFormatter<SimulatedActions>, IJsonFormatter
  {
    public SimulatedActions Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return reader.GetCurrentJsonToken() == JsonToken.String ? SimulatedActions.All : SimulatedActions.Some(formatterResolver.GetFormatter<IEnumerable<string>>().Deserialize(ref reader, formatterResolver));
    }

    public void Serialize(
      ref JsonWriter writer,
      SimulatedActions value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        return;
      if (value.UseAll)
        writer.WriteString("_all");
      else
        formatterResolver.GetFormatter<IEnumerable<string>>().Serialize(ref writer, value.Actions, formatterResolver);
    }
  }
}
