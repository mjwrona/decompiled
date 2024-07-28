// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.DatabaseInlineCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class DatabaseInlineCore : DatabaseCore
  {
    internal DatabaseInlineCore(CosmosClientContext clientContext, string databaseId)
      : base(clientContext, databaseId)
    {
    }

    public override Task<ContainerResponse> CreateContainerAsync(
      ContainerProperties containerProperties,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (CreateContainerAsync), requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.CreateContainerAsync(containerProperties, throughput, requestOptions, trace, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response, response.Resource?.Id, this.Id)));
    }

    public override Task<ContainerResponse> CreateContainerAsync(
      string id,
      string partitionKeyPath,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (CreateContainerAsync), requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions, trace, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response, response.Resource?.Id, this.Id)));
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      ContainerProperties containerProperties,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (CreateContainerIfNotExistsAsync), requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.CreateContainerIfNotExistsAsync(containerProperties, throughput, requestOptions, trace, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response, response.Resource?.Id, this.Id)));
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      string id,
      string partitionKeyPath,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (CreateContainerIfNotExistsAsync), requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput, requestOptions, trace, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response, response.Resource?.Id, this.Id)));
    }

    public override Task<ResponseMessage> CreateContainerStreamAsync(
      ContainerProperties containerProperties,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (CreateContainerStreamAsync), requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.CreateContainerStreamAsync(containerProperties, throughput, requestOptions, trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<UserResponse> CreateUserAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserResponse>(nameof (CreateUserAsync), requestOptions, (Func<ITrace, Task<UserResponse>>) (trace => this.CreateUserAsync(id, requestOptions, trace, cancellationToken)), (Func<UserResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserProperties>((Response<UserProperties>) response)));
    }

    public override ContainerBuilder DefineContainer(string name, string partitionKeyPath) => base.DefineContainer(name, partitionKeyPath);

    public override Task<DatabaseResponse> DeleteAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<DatabaseResponse>(nameof (DeleteAsync), requestOptions, (Func<ITrace, Task<DatabaseResponse>>) (trace => this.DeleteAsync(requestOptions, trace, cancellationToken)), (Func<DatabaseResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<DatabaseProperties>((Response<DatabaseProperties>) response, databaseName: response.Resource?.Id)));
    }

    public override Task<ResponseMessage> DeleteStreamAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (DeleteStreamAsync), requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.DeleteStreamAsync(requestOptions, trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Container GetContainer(string id) => base.GetContainer(id);

    public override FeedIterator<T> GetContainerQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetContainerQueryIterator<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetContainerQueryIterator<T>(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetContainerQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetContainerQueryStreamIterator(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetContainerQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetContainerQueryStreamIterator(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override User GetUser(string id) => base.GetUser(id);

    public override FeedIterator<T> GetUserQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetUserQueryIterator<T>(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetUserQueryIterator<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override Task<DatabaseResponse> ReadAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<DatabaseResponse>(nameof (ReadAsync), requestOptions, (Func<ITrace, Task<DatabaseResponse>>) (trace => this.ReadAsync(requestOptions, trace, cancellationToken)), (Func<DatabaseResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<DatabaseProperties>((Response<DatabaseProperties>) response, databaseName: response.Resource?.Id)));
    }

    public override Task<ResponseMessage> ReadStreamAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (ReadStreamAsync), requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ReadStreamAsync(requestOptions, trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ClientContext.OperationHelperAsync<int?>(nameof (ReadThroughputAsync), (RequestOptions) null, (Func<ITrace, Task<int?>>) (trace => this.ReadThroughputAsync(trace, cancellationToken)));

    public override Task<ThroughputResponse> ReadThroughputAsync(
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ThroughputResponse>(nameof (ReadThroughputAsync), requestOptions, (Func<ITrace, Task<ThroughputResponse>>) (trace => this.ReadThroughputAsync(requestOptions, trace, cancellationToken)), (Func<ThroughputResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ThroughputProperties>((Response<ThroughputProperties>) response)));
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(
      int throughput,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ThroughputResponse>(nameof (ReplaceThroughputAsync), requestOptions, (Func<ITrace, Task<ThroughputResponse>>) (trace => this.ReplaceThroughputAsync(throughput, requestOptions, trace, cancellationToken)), (Func<ThroughputResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ThroughputProperties>((Response<ThroughputProperties>) response)));
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ThroughputResponse>(nameof (ReplaceThroughputAsync), requestOptions, (Func<ITrace, Task<ThroughputResponse>>) (trace => this.ReplaceThroughputAsync(throughputProperties, requestOptions, trace, cancellationToken)), (Func<ThroughputResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ThroughputProperties>((Response<ThroughputProperties>) response)));
    }

    public override Task<ContainerResponse> CreateContainerAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (CreateContainerAsync), requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.CreateContainerAsync(containerProperties, throughputProperties, requestOptions, trace, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response, response.Resource?.Id, this.Id)));
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (CreateContainerIfNotExistsAsync), requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties, requestOptions, trace, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response, response.Resource?.Id, this.Id)));
    }

    public override Task<ResponseMessage> CreateContainerStreamAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (CreateContainerStreamAsync), requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.CreateContainerStreamAsync(containerProperties, throughputProperties, requestOptions, trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<UserResponse> UpsertUserAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserResponse>(nameof (UpsertUserAsync), requestOptions, (Func<ITrace, Task<UserResponse>>) (trace => this.UpsertUserAsync(id, requestOptions, trace, cancellationToken)), (Func<UserResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserProperties>((Response<UserProperties>) response)));
    }

    public override ClientEncryptionKey GetClientEncryptionKey(string id) => base.GetClientEncryptionKey(id);

    public override FeedIterator<ClientEncryptionKeyProperties> GetClientEncryptionKeyQueryIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return base.GetClientEncryptionKeyQueryIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override Task<ClientEncryptionKeyResponse> CreateClientEncryptionKeyAsync(
      ClientEncryptionKeyProperties clientEncryptionKeyProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ClientEncryptionKeyResponse>(nameof (CreateClientEncryptionKeyAsync), requestOptions, (Func<ITrace, Task<ClientEncryptionKeyResponse>>) (trace => this.CreateClientEncryptionKeyAsync(trace, clientEncryptionKeyProperties, requestOptions, cancellationToken)), (Func<ClientEncryptionKeyResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ClientEncryptionKeyProperties>((Response<ClientEncryptionKeyProperties>) response)));
    }
  }
}
