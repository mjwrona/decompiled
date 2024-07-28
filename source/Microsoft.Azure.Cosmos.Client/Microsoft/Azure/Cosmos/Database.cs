// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Database
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Fluent;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public abstract class Database
  {
    public abstract string Id { get; }

    public abstract CosmosClient Client { get; }

    public abstract Task<DatabaseResponse> ReadAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<DatabaseResponse> DeleteAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ThroughputResponse> ReadThroughputAsync(
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ThroughputResponse> ReplaceThroughputAsync(
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ContainerResponse> CreateContainerAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> CreateContainerStreamAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ThroughputResponse> ReplaceThroughputAsync(
      int throughput,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> ReadStreamAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> DeleteStreamAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Container GetContainer(string id);

    public abstract Task<ContainerResponse> CreateContainerAsync(
      ContainerProperties containerProperties,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ContainerResponse> CreateContainerAsync(
      string id,
      string partitionKeyPath,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      ContainerProperties containerProperties,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      string id,
      string partitionKeyPath,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> CreateContainerStreamAsync(
      ContainerProperties containerProperties,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract User GetUser(string id);

    public abstract Task<UserResponse> CreateUserAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<UserResponse> UpsertUserAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator<T> GetContainerQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetContainerQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetContainerQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetContainerQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetUserQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetUserQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract ContainerBuilder DefineContainer(string name, string partitionKeyPath);

    public abstract ClientEncryptionKey GetClientEncryptionKey(string id);

    public abstract FeedIterator<ClientEncryptionKeyProperties> GetClientEncryptionKeyQueryIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract Task<ClientEncryptionKeyResponse> CreateClientEncryptionKeyAsync(
      ClientEncryptionKeyProperties clientEncryptionKeyProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
