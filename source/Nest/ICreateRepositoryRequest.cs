// Decompiled with JetBrains decompiler
// Type: Nest.ICreateRepositoryRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("snapshot.create_repository.json")]
  [JsonFormatter(typeof (CreateRepositoryFormatter))]
  [InterfaceDataContract]
  public interface ICreateRepositoryRequest : IRequest<CreateRepositoryRequestParameters>, IRequest
  {
    ISnapshotRepository Repository { get; set; }

    [IgnoreDataMember]
    Name RepositoryName { get; }
  }
}
