// Decompiled with JetBrains decompiler
// Type: Nest.TaskIdFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class TaskIdFormatter : 
    IJsonFormatter<TaskId>,
    IJsonFormatter,
    IObjectPropertyNameFormatter<TaskId>
  {
    public TaskId Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() == JsonToken.String)
        return new TaskId(reader.ReadString());
      reader.ReadNextBlock();
      return (TaskId) null;
    }

    public void Serialize(
      ref JsonWriter writer,
      TaskId value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (TaskId) null)
        writer.WriteNull();
      else
        writer.WriteString(value.ToString());
    }

    public TaskId DeserializeFromPropertyName(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return this.Deserialize(ref reader, formatterResolver);
    }

    public void SerializeToPropertyName(
      ref JsonWriter writer,
      TaskId value,
      IJsonFormatterResolver formatterResolver)
    {
      this.Serialize(ref writer, value, formatterResolver);
    }
  }
}
