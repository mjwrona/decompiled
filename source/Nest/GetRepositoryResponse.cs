// Decompiled with JetBrains decompiler
// Type: Nest.GetRepositoryResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  [JsonFormatter(typeof (GetRepositoryResponseFormatter))]
  public class GetRepositoryResponse : ResponseBase
  {
    public IReadOnlyDictionary<string, ISnapshotRepository> Repositories { get; internal set; } = EmptyReadOnly<string, ISnapshotRepository>.Dictionary;

    public AzureRepository Azure(string name) => this.Get<AzureRepository>(name);

    public FileSystemRepository FileSystem(string name) => this.Get<FileSystemRepository>(name);

    public HdfsRepository Hdfs(string name) => this.Get<HdfsRepository>(name);

    public ReadOnlyUrlRepository ReadOnlyUrl(string name) => this.Get<ReadOnlyUrlRepository>(name);

    public S3Repository S3(string name) => this.Get<S3Repository>(name);

    private TRepository Get<TRepository>(string name) where TRepository : class, ISnapshotRepository
    {
      if (this.Repositories == null)
        return default (TRepository);
      ISnapshotRepository snapshotRepository;
      return !this.Repositories.TryGetValue(name, out snapshotRepository) ? default (TRepository) : snapshotRepository as TRepository;
    }
  }
}
