// Decompiled with JetBrains decompiler
// Type: Nest.UpdateIndexSettingsRequestFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class UpdateIndexSettingsRequestFormatter : 
    IJsonFormatter<IUpdateIndexSettingsRequest>,
    IJsonFormatter
  {
    private static readonly DynamicIndexSettingsFormatter DynamicIndexSettingsFormatter = new DynamicIndexSettingsFormatter();

    public IUpdateIndexSettingsRequest Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      IDynamicIndexSettings dynamicIndexSettings = UpdateIndexSettingsRequestFormatter.DynamicIndexSettingsFormatter.Deserialize(ref reader, formatterResolver);
      return (IUpdateIndexSettingsRequest) new UpdateIndexSettingsRequest()
      {
        IndexSettings = dynamicIndexSettings
      };
    }

    public void Serialize(
      ref JsonWriter writer,
      IUpdateIndexSettingsRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
        writer.WriteNull();
      else
        UpdateIndexSettingsRequestFormatter.DynamicIndexSettingsFormatter.Serialize(ref writer, value.IndexSettings, formatterResolver);
    }
  }
}
