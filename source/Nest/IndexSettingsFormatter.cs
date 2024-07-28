// Decompiled with JetBrains decompiler
// Type: Nest.IndexSettingsFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class IndexSettingsFormatter : IJsonFormatter<IIndexSettings>, IJsonFormatter
  {
    private static readonly DynamicIndexSettingsFormatter DynamicIndexSettingsFormatter = new DynamicIndexSettingsFormatter();

    public void Serialize(
      ref JsonWriter writer,
      IIndexSettings value,
      IJsonFormatterResolver formatterResolver)
    {
      IndexSettingsFormatter.DynamicIndexSettingsFormatter.Serialize(ref writer, (IDynamicIndexSettings) value, formatterResolver);
    }

    public IIndexSettings Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return (IIndexSettings) IndexSettingsFormatter.DynamicIndexSettingsFormatter.Deserialize(ref reader, formatterResolver);
    }
  }
}
