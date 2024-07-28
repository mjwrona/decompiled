// Decompiled with JetBrains decompiler
// Type: Nest.CronExpressionFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  internal class CronExpressionFormatter : IJsonFormatter<CronExpression>, IJsonFormatter
  {
    public CronExpression Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      return new CronExpression(reader.ReadString());
    }

    public void Serialize(
      ref JsonWriter writer,
      CronExpression value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == (CronExpression) null)
        writer.WriteNull();
      else
        writer.WriteString(value.ToString());
    }
  }
}
