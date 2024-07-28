// Decompiled with JetBrains decompiler
// Type: Nest.SourceFormatter`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.IO;

namespace Nest
{
  internal class SourceFormatter<T> : IJsonFormatter<T>, IJsonFormatter
  {
    public virtual SerializationFormatting? ForceFormatting { get; }

    private static bool AttemptFastPath(
      IElasticsearchSerializer serializer,
      out IJsonFormatterResolver formatter)
    {
      formatter = (IJsonFormatterResolver) null;
      return serializer is IInternalSerializer internalSerializer && internalSerializer.TryGetJsonFormatter(out formatter);
    }

    public T Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
      IElasticsearchSerializer sourceSerializer = connectionSettings.SourceSerializer;
      IJsonFormatterResolver formatter;
      if (SourceFormatter<T>.AttemptFastPath(sourceSerializer, out formatter))
        return formatter.GetFormatter<T>().Deserialize(ref reader, formatter);
      ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
      using (MemoryStream memoryStream = connectionSettings.MemoryStreamFactory.Create(arraySegment.Array, arraySegment.Offset, arraySegment.Count))
        return sourceSerializer.Deserialize<T>((Stream) memoryStream);
    }

    public virtual void Serialize(
      ref JsonWriter writer,
      T value,
      IJsonFormatterResolver formatterResolver)
    {
      IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
      IElasticsearchSerializer sourceSerializer = connectionSettings.SourceSerializer;
      IJsonFormatterResolver formatter;
      if (SourceFormatter<T>.AttemptFastPath(sourceSerializer, out formatter))
      {
        formatter.GetFormatter<T>().Serialize(ref writer, value, formatter);
      }
      else
      {
        SerializationFormatting valueOrDefault = this.ForceFormatting.GetValueOrDefault();
        writer.WriteSerialized<T>(value, sourceSerializer, (IConnectionConfigurationValues) connectionSettings, valueOrDefault);
      }
    }
  }
}
