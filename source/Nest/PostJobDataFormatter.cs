// Decompiled with JetBrains decompiler
// Type: Nest.PostJobDataFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class PostJobDataFormatter : IJsonFormatter<IPostJobDataRequest>, IJsonFormatter
  {
    private const byte Newline = 10;

    public IPostJobDataRequest Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }

    public void Serialize(
      ref JsonWriter writer,
      IPostJobDataRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.Data == null)
      {
        writer.WriteNull();
      }
      else
      {
        IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
        IElasticsearchSerializer sourceSerializer = connectionSettings.SourceSerializer;
        foreach (object obj in value.Data)
          writer.WriteSerialized<object>(obj, sourceSerializer, (IConnectionConfigurationValues) connectionSettings);
      }
    }
  }
}
