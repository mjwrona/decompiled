// Decompiled with JetBrains decompiler
// Type: Nest.CreateRepositoryFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  internal class CreateRepositoryFormatter : IJsonFormatter<ICreateRepositoryRequest>, IJsonFormatter
  {
    public ICreateRepositoryRequest Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotSupportedException();
    }

    public void Serialize(
      ref JsonWriter writer,
      ICreateRepositoryRequest value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value?.Repository == null)
      {
        writer.WriteBeginObject();
        writer.WriteEndObject();
      }
      else
      {
        switch (value.Repository.Type)
        {
          case "s3":
            CreateRepositoryFormatter.Serialize<IS3Repository>(ref writer, value.Repository, formatterResolver);
            break;
          case "azure":
            CreateRepositoryFormatter.Serialize<IAzureRepository>(ref writer, value.Repository, formatterResolver);
            break;
          case "url":
            CreateRepositoryFormatter.Serialize<IReadOnlyUrlRepository>(ref writer, value.Repository, formatterResolver);
            break;
          case "hdfs":
            CreateRepositoryFormatter.Serialize<IHdfsRepository>(ref writer, value.Repository, formatterResolver);
            break;
          case "fs":
            CreateRepositoryFormatter.Serialize<IFileSystemRepository>(ref writer, value.Repository, formatterResolver);
            break;
          case "source":
            CreateRepositoryFormatter.Serialize<ISourceOnlyRepository>(ref writer, value.Repository, formatterResolver);
            break;
          default:
            CreateRepositoryFormatter.Serialize<ISnapshotRepository>(ref writer, value.Repository, formatterResolver);
            break;
        }
      }
    }

    private static void Serialize<TRepository>(
      ref JsonWriter writer,
      ISnapshotRepository value,
      IJsonFormatterResolver formatterResolver)
      where TRepository : class, ISnapshotRepository
    {
      formatterResolver.GetFormatter<TRepository>().Serialize(ref writer, value as TRepository, formatterResolver);
    }
  }
}
