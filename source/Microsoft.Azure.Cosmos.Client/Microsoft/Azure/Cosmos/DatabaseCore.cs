// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.DatabaseCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class DatabaseCore : DatabaseInternal
  {
    protected DatabaseCore(CosmosClientContext clientContext, string databaseId)
    {
      this.Id = databaseId;
      this.ClientContext = clientContext;
      this.LinkUri = clientContext.CreateLink((string) null, "dbs", databaseId);
    }

    public override string Id { get; }

    public override CosmosClient Client => this.ClientContext.Client;

    internal override string LinkUri { get; }

    internal override CosmosClientContext ClientContext { get; }

    public async Task<DatabaseResponse> ReadAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      ResponseMessage responseMessage = await databaseCore.ReadStreamAsync(requestOptions, trace, cancellationToken);
      return databaseCore.ClientContext.ResponseFactory.CreateDatabaseResponse((Database) databaseCore, responseMessage);
    }

    public async Task<DatabaseResponse> DeleteAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      ResponseMessage responseMessage = await databaseCore.DeleteStreamAsync(requestOptions, trace, cancellationToken);
      return databaseCore.ClientContext.ResponseFactory.CreateDatabaseResponse((Database) databaseCore, responseMessage);
    }

    public async Task<int?> ReadThroughputAsync(ITrace trace, CancellationToken cancellationToken) => (int?) (await this.ReadThroughputIfExistsAsync((RequestOptions) null, cancellationToken)).Resource?.Throughput;

    public async Task<ThroughputResponse> ReadThroughputAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      string ridAsync = await databaseCore.GetRIDAsync(cancellationToken);
      return await new CosmosOffers(databaseCore.ClientContext).ReadThroughputAsync(ridAsync, requestOptions, cancellationToken);
    }

    internal override async Task<ThroughputResponse> ReadThroughputIfExistsAsync(
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      string ridAsync = await databaseCore.GetRIDAsync(cancellationToken);
      return await new CosmosOffers(databaseCore.ClientContext).ReadThroughputIfExistsAsync(ridAsync, requestOptions, cancellationToken);
    }

    public async Task<ThroughputResponse> ReplaceThroughputAsync(
      int throughput,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      string ridAsync = await databaseCore.GetRIDAsync(cancellationToken);
      return await new CosmosOffers(databaseCore.ClientContext).ReplaceThroughputAsync(ridAsync, throughput, requestOptions, cancellationToken);
    }

    internal override async Task<ThroughputResponse> ReplaceThroughputIfExistsAsync(
      int throughput,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      string ridAsync = await databaseCore.GetRIDAsync(cancellationToken);
      return await new CosmosOffers(databaseCore.ClientContext).ReplaceThroughputIfExistsAsync(ridAsync, throughput, requestOptions, cancellationToken);
    }

    public Task<ResponseMessage> CreateContainerStreamAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (containerProperties == null)
        throw new ArgumentNullException(nameof (containerProperties));
      this.ValidateContainerProperties(containerProperties);
      return this.ProcessCollectionCreateAsync(this.ClientContext.SerializerCore.ToStream<ContainerProperties>(containerProperties), throughputProperties, requestOptions, trace, cancellationToken);
    }

    public async Task<ContainerResponse> CreateContainerAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DatabaseCore databaseCore = this;
      if (containerProperties == null)
        throw new ArgumentNullException(nameof (containerProperties));
      databaseCore.ValidateContainerProperties(containerProperties);
      ResponseMessage async = await databaseCore.ProcessCollectionCreateAsync(databaseCore.ClientContext.SerializerCore.ToStream<ContainerProperties>(containerProperties), throughputProperties, requestOptions, trace, cancellationToken);
      return databaseCore.ClientContext.ResponseFactory.CreateContainerResponse(databaseCore.GetContainer(containerProperties.Id), async);
    }

    public async Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      ContainerProperties containerProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      if (containerProperties == null)
        throw new ArgumentNullException(nameof (containerProperties));
      databaseCore.ValidateContainerProperties(containerProperties);
      double totalRequestCharge = 0.0;
      ContainerCore container = (ContainerCore) databaseCore.GetContainer(containerProperties.Id);
      ContainerCore containerCore1 = container;
      RequestOptions requestOptions1 = requestOptions;
      ITrace trace1 = trace;
      RequestOptions requestOptions2 = requestOptions1;
      CancellationToken cancellationToken1 = cancellationToken;
      using (ResponseMessage responseMessage = await containerCore1.ReadContainerStreamAsync(trace1, requestOptions2, cancellationToken1))
      {
        totalRequestCharge = responseMessage.Headers.RequestCharge;
        if (responseMessage.StatusCode != HttpStatusCode.NotFound)
        {
          ContainerResponse containerResponse = databaseCore.ClientContext.ResponseFactory.CreateContainerResponse((Container) container, responseMessage);
          if (containerProperties.PartitionKey.Kind != PartitionKind.MultiHash && !containerResponse.Resource.PartitionKeyPath.Equals(containerProperties.PartitionKeyPath))
            throw new ArgumentException(string.Format(ClientResources.PartitionKeyPathConflict, (object) containerProperties.PartitionKeyPath, (object) containerProperties.Id, (object) containerResponse.Resource.PartitionKeyPath), "PartitionKey");
          return containerResponse;
        }
      }
      databaseCore.ValidateContainerProperties(containerProperties);
      using (ResponseMessage containerStreamAsync = await databaseCore.CreateContainerStreamAsync(containerProperties, throughputProperties, requestOptions, trace, cancellationToken))
      {
        totalRequestCharge += containerStreamAsync.Headers.RequestCharge;
        containerStreamAsync.Headers.RequestCharge = totalRequestCharge;
        if (containerStreamAsync.StatusCode != HttpStatusCode.Conflict)
          return databaseCore.ClientContext.ResponseFactory.CreateContainerResponse((Container) container, containerStreamAsync);
      }
      ContainerCore containerCore2 = container;
      RequestOptions requestOptions3 = requestOptions;
      ITrace trace2 = trace;
      RequestOptions requestOptions4 = requestOptions3;
      CancellationToken cancellationToken2 = cancellationToken;
      using (ResponseMessage responseMessage = await containerCore2.ReadContainerStreamAsync(trace2, requestOptions4, cancellationToken2))
      {
        totalRequestCharge += responseMessage.Headers.RequestCharge;
        responseMessage.Headers.RequestCharge = totalRequestCharge;
        return databaseCore.ClientContext.ResponseFactory.CreateContainerResponse((Container) container, responseMessage);
      }
    }

    public async Task<ThroughputResponse> ReplaceThroughputAsync(
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DatabaseCore databaseCore = this;
      string ridAsync = await databaseCore.GetRIDAsync(cancellationToken);
      return await new CosmosOffers(databaseCore.ClientContext).ReplaceThroughputPropertiesAsync(ridAsync, throughputProperties, requestOptions, cancellationToken);
    }

    internal override async Task<ThroughputResponse> ReplaceThroughputPropertiesIfExistsAsync(
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      string ridAsync = await databaseCore.GetRIDAsync(cancellationToken);
      return await new CosmosOffers(databaseCore.ClientContext).ReplaceThroughputPropertiesIfExistsAsync(ridAsync, throughputProperties, requestOptions, cancellationToken);
    }

    public Task<ResponseMessage> ReadStreamAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessResourceOperationStreamAsync((Stream) null, OperationType.Read, this.LinkUri, ResourceType.Database, requestOptions, trace, cancellationToken);
    }

    public Task<ResponseMessage> DeleteStreamAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessResourceOperationStreamAsync((Stream) null, OperationType.Delete, this.LinkUri, ResourceType.Database, requestOptions, trace, cancellationToken);
    }

    public async Task<ContainerResponse> CreateContainerAsync(
      ContainerProperties containerProperties,
      int? throughput,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      if (containerProperties == null)
        throw new ArgumentNullException(nameof (containerProperties));
      databaseCore.ValidateContainerProperties(containerProperties);
      ResponseMessage async = await databaseCore.ProcessCollectionCreateAsync(databaseCore.ClientContext.SerializerCore.ToStream<ContainerProperties>(containerProperties), throughput, requestOptions, trace, cancellationToken);
      return databaseCore.ClientContext.ResponseFactory.CreateContainerResponse(databaseCore.GetContainer(containerProperties.Id), async);
    }

    public Task<ContainerResponse> CreateContainerAsync(
      string id,
      string partitionKeyPath,
      int? throughput,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      if (string.IsNullOrEmpty(partitionKeyPath))
        throw new ArgumentNullException(nameof (partitionKeyPath));
      return this.CreateContainerAsync(new ContainerProperties(id, partitionKeyPath), throughput, requestOptions, trace, cancellationToken);
    }

    public Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      ContainerProperties containerProperties,
      int? throughput,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (containerProperties == null)
        throw new ArgumentNullException(nameof (containerProperties));
      return this.CreateContainerIfNotExistsAsync(containerProperties, ThroughputProperties.CreateManualThroughput(throughput), requestOptions, trace, cancellationToken);
    }

    public Task<ContainerResponse> CreateContainerIfNotExistsAsync(
      string id,
      string partitionKeyPath,
      int? throughput,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      if (string.IsNullOrEmpty(partitionKeyPath))
        throw new ArgumentNullException(nameof (partitionKeyPath));
      return this.CreateContainerIfNotExistsAsync(new ContainerProperties(id, partitionKeyPath), throughput, requestOptions, trace, cancellationToken);
    }

    public override Container GetContainer(string id) => !string.IsNullOrEmpty(id) ? (Container) new ContainerInlineCore(this.ClientContext, (DatabaseInternal) this, id) : throw new ArgumentNullException(nameof (id));

    public Task<ResponseMessage> CreateContainerStreamAsync(
      ContainerProperties containerProperties,
      int? throughput,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (containerProperties == null)
        throw new ArgumentNullException(nameof (containerProperties));
      this.ValidateContainerProperties(containerProperties);
      return this.ProcessCollectionCreateAsync(this.ClientContext.SerializerCore.ToStream<ContainerProperties>(containerProperties), throughput, requestOptions, trace, cancellationToken);
    }

    public async Task<UserResponse> CreateUserAsync(
      string id,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      UserProperties userProperties = !string.IsNullOrEmpty(id) ? new UserProperties(id) : throw new ArgumentNullException(nameof (id));
      ResponseMessage userStreamAsync = await databaseCore.CreateUserStreamAsync(userProperties, requestOptions, trace, cancellationToken);
      UserResponse userResponse = databaseCore.ClientContext.ResponseFactory.CreateUserResponse(databaseCore.GetUser(userProperties.Id), userStreamAsync);
      userProperties = (UserProperties) null;
      return userResponse;
    }

    public override User GetUser(string id) => !string.IsNullOrEmpty(id) ? (User) new UserInlineCore(this.ClientContext, (DatabaseInternal) this, id) : throw new ArgumentNullException(nameof (id));

    public Task<ResponseMessage> CreateUserStreamAsync(
      UserProperties userProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (userProperties == null)
        throw new ArgumentNullException(nameof (userProperties));
      this.ClientContext.ValidateResource(userProperties.Id);
      return this.ProcessUserCreateAsync(this.ClientContext.SerializerCore.ToStream<UserProperties>(userProperties), requestOptions, trace, cancellationToken);
    }

    public async Task<UserResponse> UpsertUserAsync(
      string id,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      DatabaseCore databaseCore = this;
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      databaseCore.ClientContext.ValidateResource(id);
      ResponseMessage responseMessage = await databaseCore.ProcessUserUpsertAsync(databaseCore.ClientContext.SerializerCore.ToStream<UserProperties>(new UserProperties(id)), requestOptions, trace, cancellationToken);
      return databaseCore.ClientContext.ResponseFactory.CreateUserResponse(databaseCore.GetUser(id), responseMessage);
    }

    public override FeedIterator GetContainerQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetContainerQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetContainerQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator GetContainerQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorCore(this.ClientContext, this.LinkUri, ResourceType.Collection, queryDefinition, continuationToken, requestOptions, (ContainerInternal) null, this.Id);
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (!(this.GetContainerQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.Collection)));
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (!(this.GetUserQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.User)));
    }

    public override FeedIterator GetUserQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorCore(this.ClientContext, this.LinkUri, ResourceType.User, queryDefinition, continuationToken, requestOptions, (ContainerInternal) null, this.Id);
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetUserQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator GetUserQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetUserQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override ContainerBuilder DefineContainer(string name, string partitionKeyPath) => new ContainerBuilder((Database) this, name, partitionKeyPath);

    public override ClientEncryptionKey GetClientEncryptionKey(string id) => !string.IsNullOrEmpty(id) ? (ClientEncryptionKey) new ClientEncryptionKeyInlineCore(this.ClientContext, (DatabaseInternal) this, id) : throw new ArgumentNullException(nameof (id));

    public override FeedIterator<ClientEncryptionKeyProperties> GetClientEncryptionKeyQueryIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (!(this.GetClientEncryptionKeyQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected FeedIteratorInternal.");
      return (FeedIterator<ClientEncryptionKeyProperties>) new FeedIteratorCore<ClientEncryptionKeyProperties>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<ClientEncryptionKeyProperties>>) (responseMessage => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<ClientEncryptionKeyProperties>(responseMessage, ResourceType.ClientEncryptionKey)));
    }

    private FeedIterator GetClientEncryptionKeyQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorCore(this.ClientContext, this.LinkUri, ResourceType.ClientEncryptionKey, queryDefinition, continuationToken, requestOptions, (ContainerInternal) null, this.Id);
    }

    public async Task<ClientEncryptionKeyResponse> CreateClientEncryptionKeyAsync(
      ITrace trace,
      ClientEncryptionKeyProperties clientEncryptionKeyProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DatabaseCore databaseCore = this;
      Stream stream = databaseCore.ClientContext.SerializerCore.ToStream<ClientEncryptionKeyProperties>(clientEncryptionKeyProperties);
      ResponseMessage encryptionKeyStreamAsync = await databaseCore.CreateClientEncryptionKeyStreamAsync(trace, stream, requestOptions, cancellationToken);
      return databaseCore.ClientContext.ResponseFactory.CreateClientEncryptionKeyResponse(databaseCore.GetClientEncryptionKey(clientEncryptionKeyProperties.Id), encryptionKeyStreamAsync);
    }

    private void ValidateContainerProperties(ContainerProperties containerProperties)
    {
      containerProperties.ValidateRequiredProperties();
      this.ClientContext.ValidateResource(containerProperties.Id);
    }

    private Task<ResponseMessage> ProcessCollectionCreateAsync(
      Stream streamPayload,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosClientContext clientContext = this.ClientContext;
      string linkUri = this.LinkUri;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      Action<RequestMessage> requestEnricher = (Action<RequestMessage>) (httpRequestMessage => httpRequestMessage.AddThroughputPropertiesHeader(throughputProperties));
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.Collection, OperationType.Create, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, requestEnricher, trace1, cancellationToken1);
    }

    private Task<ResponseMessage> ProcessCollectionCreateAsync(
      Stream streamPayload,
      int? throughput,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosClientContext clientContext = this.ClientContext;
      string linkUri = this.LinkUri;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      Action<RequestMessage> requestEnricher = (Action<RequestMessage>) (httpRequestMessage => httpRequestMessage.AddThroughputHeader(throughput));
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.Collection, OperationType.Create, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, requestEnricher, trace1, cancellationToken1);
    }

    private Task<ResponseMessage> ProcessUserCreateAsync(
      Stream streamPayload,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosClientContext clientContext = this.ClientContext;
      string linkUri = this.LinkUri;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.User, OperationType.Create, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, (Action<RequestMessage>) null, trace1, cancellationToken1);
    }

    private Task<ResponseMessage> ProcessUserUpsertAsync(
      Stream streamPayload,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosClientContext clientContext = this.ClientContext;
      string linkUri = this.LinkUri;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.User, OperationType.Upsert, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, (Action<RequestMessage>) null, trace1, cancellationToken1);
    }

    internal override async Task<string> GetRIDAsync(CancellationToken cancellationToken) => (await this.ReadAsync(cancellationToken: cancellationToken))?.Resource?.ResourceId;

    private Task<ResponseMessage> CreateClientEncryptionKeyStreamAsync(
      ITrace trace,
      Stream streamPayload,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (streamPayload == null)
        throw new ArgumentNullException(nameof (streamPayload));
      CosmosClientContext clientContext = this.ClientContext;
      string linkUri = this.LinkUri;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.ClientEncryptionKey, OperationType.Create, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, (Action<RequestMessage>) null, trace1, cancellationToken1);
    }

    private Task<ResponseMessage> ProcessResourceOperationStreamAsync(
      Stream streamPayload,
      OperationType operationType,
      string linkUri,
      ResourceType resourceType,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosClientContext clientContext = this.ClientContext;
      string resourceUri = linkUri;
      int num1 = (int) resourceType;
      int num2 = (int) operationType;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(resourceUri, (ResourceType) num1, (OperationType) num2, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, (Action<RequestMessage>) null, trace1, cancellationToken1);
    }
  }
}
