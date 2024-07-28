// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.UserCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class UserCore : User
  {
    internal UserCore(CosmosClientContext clientContext, DatabaseInternal database, string userId)
    {
      this.Id = userId;
      this.ClientContext = clientContext;
      this.LinkUri = clientContext.CreateLink(database.LinkUri, "users", userId);
      this.Database = (Database) database;
    }

    public override string Id { get; }

    public Database Database { get; }

    internal virtual string LinkUri { get; }

    internal virtual CosmosClientContext ClientContext { get; }

    public async Task<UserResponse> ReadAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      UserCore userCore = this;
      ResponseMessage responseMessage = await userCore.ReadStreamAsync(requestOptions, trace, cancellationToken);
      return userCore.ClientContext.ResponseFactory.CreateUserResponse((User) userCore, responseMessage);
    }

    public Task<ResponseMessage> ReadStreamAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessStreamAsync((Stream) null, OperationType.Read, requestOptions, trace, cancellationToken);
    }

    public async Task<UserResponse> ReplaceAsync(
      UserProperties userProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      UserCore userCore = this;
      if (userProperties == null)
        throw new ArgumentNullException(nameof (userProperties));
      userCore.ClientContext.ValidateResource(userProperties.Id);
      ResponseMessage responseMessage = await userCore.ReplaceStreamInternalAsync(userCore.ClientContext.SerializerCore.ToStream<UserProperties>(userProperties), requestOptions, trace, cancellationToken);
      return userCore.ClientContext.ResponseFactory.CreateUserResponse((User) userCore, responseMessage);
    }

    public Task<ResponseMessage> ReplaceStreamAsync(
      UserProperties userProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (userProperties == null)
        throw new ArgumentNullException(nameof (userProperties));
      this.ClientContext.ValidateResource(userProperties.Id);
      return this.ReplaceStreamInternalAsync(this.ClientContext.SerializerCore.ToStream<UserProperties>(userProperties), requestOptions, trace, cancellationToken);
    }

    public async Task<UserResponse> DeleteAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      UserCore userCore = this;
      ResponseMessage responseMessage = await userCore.DeleteStreamAsync(requestOptions, trace, cancellationToken);
      return userCore.ClientContext.ResponseFactory.CreateUserResponse((User) userCore, responseMessage);
    }

    public Task<ResponseMessage> DeleteStreamAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessStreamAsync((Stream) null, OperationType.Delete, requestOptions, trace, cancellationToken);
    }

    public override Permission GetPermission(string id) => !string.IsNullOrEmpty(id) ? (Permission) new PermissionInlineCore(this.ClientContext, this, id) : throw new ArgumentNullException(nameof (id));

    public async Task<PermissionResponse> CreatePermissionAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      UserCore userCore = this;
      if (permissionProperties == null)
        throw new ArgumentNullException(nameof (permissionProperties));
      userCore.ClientContext.ValidateResource(permissionProperties.Id);
      ResponseMessage streamInternalAsync = await userCore.CreatePermissionStreamInternalAsync(userCore.ClientContext.SerializerCore.ToStream<PermissionProperties>(permissionProperties), tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
      return userCore.ClientContext.ResponseFactory.CreatePermissionResponse(userCore.GetPermission(permissionProperties.Id), streamInternalAsync);
    }

    public Task<ResponseMessage> CreatePermissionStreamAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (permissionProperties == null)
        throw new ArgumentNullException(nameof (permissionProperties));
      this.ClientContext.ValidateResource(permissionProperties.Id);
      return this.CreatePermissionStreamInternalAsync(this.ClientContext.SerializerCore.ToStream<PermissionProperties>(permissionProperties), tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
    }

    public async Task<PermissionResponse> UpsertPermissionAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      UserCore userCore = this;
      if (permissionProperties == null)
        throw new ArgumentNullException(nameof (permissionProperties));
      userCore.ClientContext.ValidateResource(permissionProperties.Id);
      ResponseMessage responseMessage = await userCore.UpsertPermissionStreamInternalAsync(userCore.ClientContext.SerializerCore.ToStream<PermissionProperties>(permissionProperties), tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
      return userCore.ClientContext.ResponseFactory.CreatePermissionResponse(userCore.GetPermission(permissionProperties.Id), responseMessage);
    }

    public override FeedIterator<T> GetPermissionQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (!(this.GetPermissionQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected a FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.Permission)));
    }

    public FeedIterator GetPermissionQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorCore(this.ClientContext, this.LinkUri, ResourceType.Permission, queryDefinition, continuationToken, requestOptions, (ContainerInternal) null, this.Database.Id);
    }

    public override FeedIterator<T> GetPermissionQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetPermissionQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public FeedIterator GetPermissionQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetPermissionQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    internal Task<ResponseMessage> ProcessPermissionCreateAsync(
      Stream streamPayload,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosClientContext clientContext = this.ClientContext;
      string linkUri = this.LinkUri;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      Action<RequestMessage> requestEnricher = (Action<RequestMessage>) (requestMessage =>
      {
        if (!tokenExpiryInSeconds.HasValue)
          return;
        requestMessage.Headers.Add("x-ms-documentdb-expiry-seconds", tokenExpiryInSeconds.Value.ToString());
      });
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.Permission, OperationType.Create, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, requestEnricher, trace1, cancellationToken1);
    }

    internal Task<ResponseMessage> ProcessPermissionUpsertAsync(
      Stream streamPayload,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosClientContext clientContext = this.ClientContext;
      string linkUri = this.LinkUri;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      Action<RequestMessage> requestEnricher = (Action<RequestMessage>) (requestMessage =>
      {
        if (!tokenExpiryInSeconds.HasValue)
          return;
        requestMessage.Headers.Add("x-ms-documentdb-expiry-seconds", tokenExpiryInSeconds.Value.ToString());
      });
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.Permission, OperationType.Upsert, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, requestEnricher, trace1, cancellationToken1);
    }

    private Task<ResponseMessage> ReplaceStreamInternalAsync(
      Stream streamPayload,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessStreamAsync(streamPayload, OperationType.Replace, requestOptions, trace, cancellationToken);
    }

    private Task<ResponseMessage> ProcessStreamAsync(
      Stream streamPayload,
      OperationType operationType,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessResourceOperationStreamAsync(streamPayload, operationType, this.LinkUri, ResourceType.User, requestOptions, trace, cancellationToken);
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

    private Task<ResponseMessage> CreatePermissionStreamInternalAsync(
      Stream streamPayload,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessPermissionCreateAsync(streamPayload, tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
    }

    private Task<ResponseMessage> UpsertPermissionStreamInternalAsync(
      Stream streamPayload,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessPermissionUpsertAsync(streamPayload, tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
    }
  }
}
