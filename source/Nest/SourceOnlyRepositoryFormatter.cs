// Decompiled with JetBrains decompiler
// Type: Nest.SourceOnlyRepositoryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Extensions;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using System;

namespace Nest
{
  internal class SourceOnlyRepositoryFormatter : 
    IJsonFormatter<ISourceOnlyRepository>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary Fields = new AutomataDictionary()
    {
      {
        "type",
        0
      },
      {
        "settings",
        1
      }
    };
    private static readonly byte[] DelegateType = JsonWriter.GetEncodedPropertyNameWithoutQuotation("delegate_type");

    public void Serialize(
      ref JsonWriter writer,
      ISourceOnlyRepository value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value.DelegateType.IsNullOrEmpty())
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        writer.WritePropertyName("type");
        writer.WriteString("source");
        if (value.DelegateSettings != null)
        {
          writer.WriteValueSeparator();
          writer.WritePropertyName("settings");
          writer.WriteBeginObject();
          writer.WritePropertyName("delegate_type");
          writer.WriteString(value.DelegateType);
          writer.WriteValueSeparator();
          JsonWriter writer1 = new JsonWriter();
          switch (value.DelegateType)
          {
            case "s3":
              SourceOnlyRepositoryFormatter.Serialize<IS3RepositorySettings>(ref writer1, value.DelegateSettings, formatterResolver);
              break;
            case "azure":
              SourceOnlyRepositoryFormatter.Serialize<IAzureRepositorySettings>(ref writer1, value.DelegateSettings, formatterResolver);
              break;
            case "url":
              SourceOnlyRepositoryFormatter.Serialize<IReadOnlyUrlRepositorySettings>(ref writer1, value.DelegateSettings, formatterResolver);
              break;
            case "hdfs":
              SourceOnlyRepositoryFormatter.Serialize<IHdfsRepositorySettings>(ref writer1, value.DelegateSettings, formatterResolver);
              break;
            case "fs":
              SourceOnlyRepositoryFormatter.Serialize<IFileSystemRepositorySettings>(ref writer1, value.DelegateSettings, formatterResolver);
              break;
            default:
              SourceOnlyRepositoryFormatter.Serialize<IRepositorySettings>(ref writer1, value.DelegateSettings, formatterResolver);
              break;
          }
          ArraySegment<byte> buffer = writer1.GetBuffer();
          for (int index = 1; index < buffer.Count - 1; ++index)
            writer.WriteRawUnsafe(buffer.Array[index]);
          writer.WriteEndObject();
        }
        writer.WriteEndObject();
      }
    }

    private static void Serialize<TRepositorySettings>(
      ref JsonWriter writer,
      object value,
      IJsonFormatterResolver formatterResolver)
      where TRepositorySettings : class, IRepositorySettings
    {
      formatterResolver.GetFormatter<TRepositorySettings>().Serialize(ref writer, value as TRepositorySettings, formatterResolver);
    }

    private static TRepositorySettings Deserialize<TRepositorySettings>(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TRepositorySettings : class, IRepositorySettings
    {
      return formatterResolver.GetFormatter<TRepositorySettings>().Deserialize(ref reader, formatterResolver);
    }

    public ISourceOnlyRepository Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (ISourceOnlyRepository) null;
      }
      int count1 = 0;
      ArraySegment<byte> arraySegment1 = new ArraySegment<byte>();
      while (reader.ReadIsInObject(ref count1))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (SourceOnlyRepositoryFormatter.Fields.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              reader.ReadNext();
              continue;
            case 1:
              arraySegment1 = reader.ReadNextBlockSegment();
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNextBlock();
      }
      if (arraySegment1 == new ArraySegment<byte>())
        return (ISourceOnlyRepository) null;
      JsonReader reader1 = new JsonReader(arraySegment1.Array, arraySegment1.Offset);
      string delegateType = (string) null;
      object settings = (object) null;
      int count2 = 0;
      while (reader1.ReadIsInObject(ref count2))
      {
        ArraySegment<byte> arraySegment2 = reader1.ReadPropertyNameSegmentRaw();
        if (arraySegment2.EqualsBytes(SourceOnlyRepositoryFormatter.DelegateType))
        {
          delegateType = reader1.ReadString();
          break;
        }
        reader1.ReadNextBlock();
      }
      reader1.ResetOffset();
      switch (delegateType)
      {
        case "s3":
          settings = (object) SourceOnlyRepositoryFormatter.Deserialize<S3RepositorySettings>(ref reader1, formatterResolver);
          break;
        case "azure":
          settings = (object) SourceOnlyRepositoryFormatter.Deserialize<AzureRepositorySettings>(ref reader1, formatterResolver);
          break;
        case "url":
          settings = (object) SourceOnlyRepositoryFormatter.Deserialize<ReadOnlyUrlRepositorySettings>(ref reader1, formatterResolver);
          break;
        case "hdfs":
          settings = (object) SourceOnlyRepositoryFormatter.Deserialize<HdfsRepositorySettings>(ref reader1, formatterResolver);
          break;
        case "fs":
          settings = (object) SourceOnlyRepositoryFormatter.Deserialize<FileSystemRepositorySettings>(ref reader1, formatterResolver);
          break;
      }
      return (ISourceOnlyRepository) new SourceOnlyRepository(delegateType, settings);
    }
  }
}
