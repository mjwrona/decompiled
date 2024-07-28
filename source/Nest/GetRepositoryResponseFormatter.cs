// Decompiled with JetBrains decompiler
// Type: Nest.GetRepositoryResponseFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class GetRepositoryResponseFormatter : 
    IJsonFormatter<GetRepositoryResponse>,
    IJsonFormatter
  {
    public GetRepositoryResponse Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      GetRepositoryResponse repositoryResponse1 = new GetRepositoryResponse();
      Dictionary<string, ISnapshotRepository> dictionary = new Dictionary<string, ISnapshotRepository>();
      int count1 = 0;
      while (reader.ReadIsInObject(ref count1))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (ResponseFormatterHelpers.ServerErrorFields.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              if (reader.GetCurrentJsonToken() == JsonToken.String)
              {
                GetRepositoryResponse repositoryResponse2 = repositoryResponse1;
                Error error = new Error();
                error.Reason = reader.ReadString();
                repositoryResponse2.Error = error;
                continue;
              }
              IJsonFormatter<Error> formatter = formatterResolver.GetFormatter<Error>();
              repositoryResponse1.Error = formatter.Deserialize(ref reader, formatterResolver);
              continue;
            case 1:
              if (reader.GetCurrentJsonToken() == JsonToken.Number)
              {
                repositoryResponse1.StatusCode = new int?(reader.ReadInt32());
                continue;
              }
              reader.ReadNextBlock();
              continue;
            default:
              continue;
          }
        }
        else
        {
          string key = segment.Utf8String();
          ArraySegment<byte> arraySegment = reader.ReadNextBlockSegment();
          Elasticsearch.Net.Utf8Json.JsonReader reader1 = new Elasticsearch.Net.Utf8Json.JsonReader(arraySegment.Array, arraySegment.Offset);
          int count2 = 0;
          string str = (string) null;
          ArraySegment<byte> settings = new ArraySegment<byte>();
          while (reader1.ReadIsInObject(ref count2))
          {
            switch (reader1.ReadPropertyName())
            {
              case "type":
                str = reader1.ReadString();
                continue;
              case "settings":
                settings = reader1.ReadNextBlockSegment();
                continue;
              default:
                reader1.ReadNextBlock();
                continue;
            }
          }
          switch (str)
          {
            case "fs":
              FileSystemRepository repository1 = this.GetRepository<FileSystemRepository, FileSystemRepositorySettings>(settings, formatterResolver);
              dictionary.Add(key, (ISnapshotRepository) repository1);
              continue;
            case "url":
              ReadOnlyUrlRepository repository2 = this.GetRepository<ReadOnlyUrlRepository, ReadOnlyUrlRepositorySettings>(settings, formatterResolver);
              dictionary.Add(key, (ISnapshotRepository) repository2);
              continue;
            case "azure":
              AzureRepository repository3 = this.GetRepository<AzureRepository, AzureRepositorySettings>(settings, formatterResolver);
              dictionary.Add(key, (ISnapshotRepository) repository3);
              continue;
            case "s3":
              S3Repository repository4 = this.GetRepository<S3Repository, S3RepositorySettings>(settings, formatterResolver);
              dictionary.Add(key, (ISnapshotRepository) repository4);
              continue;
            case "hdfs":
              HdfsRepository repository5 = this.GetRepository<HdfsRepository, HdfsRepositorySettings>(settings, formatterResolver);
              dictionary.Add(key, (ISnapshotRepository) repository5);
              continue;
            case "source":
              reader1.ResetOffset();
              ISourceOnlyRepository sourceOnlyRepository = formatterResolver.GetFormatter<ISourceOnlyRepository>().Deserialize(ref reader1, formatterResolver);
              dictionary.Add(key, (ISnapshotRepository) sourceOnlyRepository);
              continue;
            default:
              continue;
          }
        }
      }
      repositoryResponse1.Repositories = (IReadOnlyDictionary<string, ISnapshotRepository>) dictionary;
      return repositoryResponse1;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      GetRepositoryResponse value,
      IJsonFormatterResolver formatterResolver)
    {
      DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<GetRepositoryResponse>().Serialize(ref writer, value, formatterResolver);
    }

    private TRepository GetRepository<TRepository, TSettings>(
      ArraySegment<byte> settings,
      IJsonFormatterResolver formatterResolver)
      where TRepository : ISnapshotRepository
      where TSettings : IRepositorySettings
    {
      if (settings == new ArraySegment<byte>())
        return typeof (TRepository).CreateInstance<TRepository>();
      IJsonFormatter<TSettings> formatter = formatterResolver.GetFormatter<TSettings>();
      Elasticsearch.Net.Utf8Json.JsonReader reader = new Elasticsearch.Net.Utf8Json.JsonReader(settings.Array, settings.Offset);
      return typeof (TRepository).CreateInstance<TRepository>((object) formatter.Deserialize(ref reader, formatterResolver));
    }
  }
}
