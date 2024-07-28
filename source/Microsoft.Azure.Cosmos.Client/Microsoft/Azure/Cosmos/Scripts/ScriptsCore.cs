// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Scripts.ScriptsCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Scripts
{
  internal abstract class ScriptsCore : Microsoft.Azure.Cosmos.Scripts.Scripts
  {
    protected readonly ContainerInternal container;

    internal ScriptsCore(ContainerInternal container, CosmosClientContext clientContext)
    {
      this.container = container;
      this.ClientContext = clientContext;
    }

    protected CosmosClientContext ClientContext { get; }

    public Task<StoredProcedureResponse> CreateStoredProcedureAsync(
      StoredProcedureProperties storedProcedureProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessScriptsCreateOperationAsync<StoredProcedureResponse>(this.container.LinkUri, ResourceType.StoredProcedure, OperationType.Create, this.ClientContext.SerializerCore.ToStream<StoredProcedureProperties>(storedProcedureProperties), requestOptions, new Func<ResponseMessage, StoredProcedureResponse>(this.ClientContext.ResponseFactory.CreateStoredProcedureResponse), trace, cancellationToken);
    }

    public override FeedIterator GetStoredProcedureQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetStoredProcedureQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator<T> GetStoredProcedureQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetStoredProcedureQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator GetStoredProcedureQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorCore(this.ClientContext, this.container.LinkUri, ResourceType.StoredProcedure, queryDefinition, continuationToken, requestOptions, this.container);
    }

    public override FeedIterator<T> GetStoredProcedureQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (!(this.GetStoredProcedureQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected a FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.StoredProcedure)));
    }

    public Task<StoredProcedureResponse> ReadStoredProcedureAsync(
      string id,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      return this.ProcessStoredProcedureOperationAsync(id, OperationType.Read, (Stream) null, requestOptions, trace, cancellationToken);
    }

    public Task<StoredProcedureResponse> ReplaceStoredProcedureAsync(
      StoredProcedureProperties storedProcedureProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessStoredProcedureOperationAsync(storedProcedureProperties.Id, OperationType.Replace, this.ClientContext.SerializerCore.ToStream<StoredProcedureProperties>(storedProcedureProperties), requestOptions, trace, cancellationToken);
    }

    public Task<StoredProcedureResponse> DeleteStoredProcedureAsync(
      string id,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      return this.ProcessStoredProcedureOperationAsync(id, OperationType.Delete, (Stream) null, requestOptions, trace, cancellationToken);
    }

    public async Task<StoredProcedureExecuteResponse<TOutput>> ExecuteStoredProcedureAsync<TOutput>(
      string storedProcedureId,
      Microsoft.Azure.Cosmos.PartitionKey partitionKey,
      object[] parameters,
      StoredProcedureRequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ClientContext.ResponseFactory.CreateStoredProcedureExecuteResponse<TOutput>(await this.ExecuteStoredProcedureStreamAsync(storedProcedureId, partitionKey, parameters, requestOptions, trace, cancellationToken));
    }

    public Task<ResponseMessage> ExecuteStoredProcedureStreamAsync(
      string storedProcedureId,
      Microsoft.Azure.Cosmos.PartitionKey partitionKey,
      object[] parameters,
      StoredProcedureRequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      Stream stream = (Stream) null;
      if (parameters != null)
        stream = this.ClientContext.SerializerCore.ToStream<object[]>(parameters);
      string storedProcedureId1 = storedProcedureId;
      Microsoft.Azure.Cosmos.PartitionKey partitionKey1 = partitionKey;
      Stream streamPayload = stream;
      Microsoft.Azure.Cosmos.PartitionKey partitionKey2 = partitionKey1;
      StoredProcedureRequestOptions requestOptions1 = requestOptions;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.ExecuteStoredProcedureStreamAsync(storedProcedureId1, streamPayload, partitionKey2, requestOptions1, trace1, cancellationToken1);
    }

    public Task<ResponseMessage> ExecuteStoredProcedureStreamAsync(
      string storedProcedureId,
      Stream streamPayload,
      Microsoft.Azure.Cosmos.PartitionKey partitionKey,
      StoredProcedureRequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(storedProcedureId))
        throw new ArgumentNullException(nameof (storedProcedureId));
      ContainerInternal.ValidatePartitionKey((object) partitionKey, (RequestOptions) requestOptions);
      return this.ProcessStreamOperationAsync(this.ClientContext.CreateLink(this.container.LinkUri, "sprocs", storedProcedureId), ResourceType.StoredProcedure, OperationType.ExecuteJavaScript, new Microsoft.Azure.Cosmos.PartitionKey?(partitionKey), streamPayload, (RequestOptions) requestOptions, trace, cancellationToken);
    }

    public Task<TriggerResponse> CreateTriggerAsync(
      TriggerProperties triggerProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (triggerProperties == null)
        throw new ArgumentNullException(nameof (triggerProperties));
      if (string.IsNullOrEmpty(triggerProperties.Id))
        throw new ArgumentNullException("Id");
      if (string.IsNullOrEmpty(triggerProperties.Body))
        throw new ArgumentNullException("Body");
      return this.ProcessScriptsCreateOperationAsync<TriggerResponse>(this.container.LinkUri, ResourceType.Trigger, OperationType.Create, this.ClientContext.SerializerCore.ToStream<TriggerProperties>(triggerProperties), requestOptions, new Func<ResponseMessage, TriggerResponse>(this.ClientContext.ResponseFactory.CreateTriggerResponse), trace, cancellationToken);
    }

    public override FeedIterator GetTriggerQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetTriggerQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator<T> GetTriggerQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetTriggerQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator GetTriggerQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorCore(this.ClientContext, this.container.LinkUri, ResourceType.Trigger, queryDefinition, continuationToken, requestOptions, this.container);
    }

    public override FeedIterator<T> GetTriggerQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (!(this.GetTriggerQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected a FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.Trigger)));
    }

    public Task<TriggerResponse> ReadTriggerAsync(
      string id,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      return this.ProcessTriggerOperationAsync(id, OperationType.Read, (Stream) null, requestOptions, trace, cancellationToken);
    }

    public Task<TriggerResponse> ReplaceTriggerAsync(
      TriggerProperties triggerProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (triggerProperties == null)
        throw new ArgumentNullException(nameof (triggerProperties));
      if (string.IsNullOrEmpty(triggerProperties.Id))
        throw new ArgumentNullException("Id");
      if (string.IsNullOrEmpty(triggerProperties.Body))
        throw new ArgumentNullException("Body");
      return this.ProcessTriggerOperationAsync(triggerProperties.Id, OperationType.Replace, this.ClientContext.SerializerCore.ToStream<TriggerProperties>(triggerProperties), requestOptions, trace, cancellationToken);
    }

    public Task<TriggerResponse> DeleteTriggerAsync(
      string id,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      return this.ProcessTriggerOperationAsync(id, OperationType.Delete, (Stream) null, requestOptions, trace, cancellationToken);
    }

    public Task<UserDefinedFunctionResponse> CreateUserDefinedFunctionAsync(
      UserDefinedFunctionProperties userDefinedFunctionProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (userDefinedFunctionProperties == null)
        throw new ArgumentNullException(nameof (userDefinedFunctionProperties));
      if (string.IsNullOrEmpty(userDefinedFunctionProperties.Id))
        throw new ArgumentNullException("Id");
      if (string.IsNullOrEmpty(userDefinedFunctionProperties.Body))
        throw new ArgumentNullException("Body");
      return this.ProcessScriptsCreateOperationAsync<UserDefinedFunctionResponse>(this.container.LinkUri, ResourceType.UserDefinedFunction, OperationType.Create, this.ClientContext.SerializerCore.ToStream<UserDefinedFunctionProperties>(userDefinedFunctionProperties), requestOptions, new Func<ResponseMessage, UserDefinedFunctionResponse>(this.ClientContext.ResponseFactory.CreateUserDefinedFunctionResponse), trace, cancellationToken);
    }

    public override FeedIterator GetUserDefinedFunctionQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetUserDefinedFunctionQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator<T> GetUserDefinedFunctionQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetUserDefinedFunctionQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator GetUserDefinedFunctionQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorCore(this.ClientContext, this.container.LinkUri, ResourceType.UserDefinedFunction, queryDefinition, continuationToken, requestOptions, this.container);
    }

    public override FeedIterator<T> GetUserDefinedFunctionQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (!(this.GetUserDefinedFunctionQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected a FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.UserDefinedFunction)));
    }

    public Task<UserDefinedFunctionResponse> ReadUserDefinedFunctionAsync(
      string id,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      return this.ProcessUserDefinedFunctionOperationAsync(id, OperationType.Read, (Stream) null, requestOptions, trace, cancellationToken);
    }

    public Task<UserDefinedFunctionResponse> ReplaceUserDefinedFunctionAsync(
      UserDefinedFunctionProperties userDefinedFunctionProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (userDefinedFunctionProperties == null)
        throw new ArgumentNullException(nameof (userDefinedFunctionProperties));
      if (string.IsNullOrEmpty(userDefinedFunctionProperties.Id))
        throw new ArgumentNullException("Id");
      if (string.IsNullOrEmpty(userDefinedFunctionProperties.Body))
        throw new ArgumentNullException("Body");
      return this.ProcessUserDefinedFunctionOperationAsync(userDefinedFunctionProperties.Id, OperationType.Replace, this.ClientContext.SerializerCore.ToStream<UserDefinedFunctionProperties>(userDefinedFunctionProperties), requestOptions, trace, cancellationToken);
    }

    public Task<UserDefinedFunctionResponse> DeleteUserDefinedFunctionAsync(
      string id,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      return this.ProcessUserDefinedFunctionOperationAsync(id, OperationType.Delete, (Stream) null, requestOptions, trace, cancellationToken);
    }

    private async Task<StoredProcedureResponse> ProcessStoredProcedureOperationAsync(
      string id,
      OperationType operationType,
      Stream streamPayload,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ScriptsCore scriptsCore1 = this;
      string link = scriptsCore1.ClientContext.CreateLink(scriptsCore1.container.LinkUri, "sprocs", id);
      ScriptsCore scriptsCore2 = scriptsCore1;
      string resourceUri = link;
      int num = (int) operationType;
      RequestOptions requestOptions1 = requestOptions;
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey = new Microsoft.Azure.Cosmos.PartitionKey?();
      Stream streamPayload1 = streamPayload;
      RequestOptions requestOptions2 = requestOptions1;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      ResponseMessage responseMessage = await scriptsCore2.ProcessStreamOperationAsync(resourceUri, ResourceType.StoredProcedure, (OperationType) num, partitionKey, streamPayload1, requestOptions2, trace1, cancellationToken1);
      return scriptsCore1.ClientContext.ResponseFactory.CreateStoredProcedureResponse(responseMessage);
    }

    private async Task<TriggerResponse> ProcessTriggerOperationAsync(
      string id,
      OperationType operationType,
      Stream streamPayload,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ScriptsCore scriptsCore1 = this;
      string link = scriptsCore1.ClientContext.CreateLink(scriptsCore1.container.LinkUri, "triggers", id);
      ScriptsCore scriptsCore2 = scriptsCore1;
      string resourceUri = link;
      int num = (int) operationType;
      RequestOptions requestOptions1 = requestOptions;
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey = new Microsoft.Azure.Cosmos.PartitionKey?();
      Stream streamPayload1 = streamPayload;
      RequestOptions requestOptions2 = requestOptions1;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      ResponseMessage responseMessage = await scriptsCore2.ProcessStreamOperationAsync(resourceUri, ResourceType.Trigger, (OperationType) num, partitionKey, streamPayload1, requestOptions2, trace1, cancellationToken1);
      return scriptsCore1.ClientContext.ResponseFactory.CreateTriggerResponse(responseMessage);
    }

    private Task<ResponseMessage> ProcessStreamOperationAsync(
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      Stream streamPayload,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ClientContext.ProcessResourceOperationStreamAsync(resourceUri, resourceType, operationType, requestOptions, this.container, partitionKey.HasValue ? (FeedRange) new FeedRangePartitionKey(partitionKey.Value) : (FeedRange) null, streamPayload, (Action<RequestMessage>) null, trace, cancellationToken);
    }

    private async Task<T> ProcessScriptsCreateOperationAsync<T>(
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      Stream streamPayload,
      RequestOptions requestOptions,
      Func<ResponseMessage, T> responseFunc,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      string resourceUri1 = resourceUri;
      int num1 = (int) resourceType;
      int num2 = (int) operationType;
      RequestOptions requestOptions1 = requestOptions;
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey = new Microsoft.Azure.Cosmos.PartitionKey?();
      Stream streamPayload1 = streamPayload;
      RequestOptions requestOptions2 = requestOptions1;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return responseFunc(await this.ProcessStreamOperationAsync(resourceUri1, (ResourceType) num1, (OperationType) num2, partitionKey, streamPayload1, requestOptions2, trace1, cancellationToken1));
    }

    private async Task<UserDefinedFunctionResponse> ProcessUserDefinedFunctionOperationAsync(
      string id,
      OperationType operationType,
      Stream streamPayload,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ScriptsCore scriptsCore1 = this;
      string link = scriptsCore1.ClientContext.CreateLink(scriptsCore1.container.LinkUri, "udfs", id);
      ScriptsCore scriptsCore2 = scriptsCore1;
      string resourceUri = link;
      int num = (int) operationType;
      RequestOptions requestOptions1 = requestOptions;
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey = new Microsoft.Azure.Cosmos.PartitionKey?();
      Stream streamPayload1 = streamPayload;
      RequestOptions requestOptions2 = requestOptions1;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      ResponseMessage responseMessage = await scriptsCore2.ProcessStreamOperationAsync(resourceUri, ResourceType.UserDefinedFunction, (OperationType) num, partitionKey, streamPayload1, requestOptions2, trace1, cancellationToken1);
      return scriptsCore1.ClientContext.ResponseFactory.CreateUserDefinedFunctionResponse(responseMessage);
    }
  }
}
