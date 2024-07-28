// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Scripts.ScriptsInlineCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Scripts
{
  internal sealed class ScriptsInlineCore : ScriptsCore
  {
    internal ScriptsInlineCore(ContainerInternal container, CosmosClientContext clientContext)
      : base(container, clientContext)
    {
    }

    public override Task<StoredProcedureResponse> CreateStoredProcedureAsync(
      StoredProcedureProperties storedProcedureProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<StoredProcedureResponse>(nameof (CreateStoredProcedureAsync), requestOptions, (Func<ITrace, Task<StoredProcedureResponse>>) (trace => this.CreateStoredProcedureAsync(storedProcedureProperties, requestOptions, trace, cancellationToken)), (Func<StoredProcedureResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<StoredProcedureProperties>((Response<StoredProcedureProperties>) response)));
    }

    public override FeedIterator<T> GetStoredProcedureQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetStoredProcedureQueryIterator<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetStoredProcedureQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetStoredProcedureQueryStreamIterator(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetStoredProcedureQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetStoredProcedureQueryIterator<T>(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetStoredProcedureQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetStoredProcedureQueryStreamIterator(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override Task<StoredProcedureResponse> ReadStoredProcedureAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<StoredProcedureResponse>(nameof (ReadStoredProcedureAsync), requestOptions, (Func<ITrace, Task<StoredProcedureResponse>>) (trace => this.ReadStoredProcedureAsync(id, requestOptions, trace, cancellationToken)), (Func<StoredProcedureResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<StoredProcedureProperties>((Response<StoredProcedureProperties>) response)));
    }

    public override Task<StoredProcedureResponse> ReplaceStoredProcedureAsync(
      StoredProcedureProperties storedProcedureProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<StoredProcedureResponse>(nameof (ReplaceStoredProcedureAsync), requestOptions, (Func<ITrace, Task<StoredProcedureResponse>>) (trace => this.ReplaceStoredProcedureAsync(storedProcedureProperties, requestOptions, trace, cancellationToken)), (Func<StoredProcedureResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<StoredProcedureProperties>((Response<StoredProcedureProperties>) response)));
    }

    public override Task<StoredProcedureResponse> DeleteStoredProcedureAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<StoredProcedureResponse>(nameof (DeleteStoredProcedureAsync), requestOptions, (Func<ITrace, Task<StoredProcedureResponse>>) (trace => this.DeleteStoredProcedureAsync(id, requestOptions, trace, cancellationToken)), (Func<StoredProcedureResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<StoredProcedureProperties>((Response<StoredProcedureProperties>) response)));
    }

    public override Task<StoredProcedureExecuteResponse<TOutput>> ExecuteStoredProcedureAsync<TOutput>(
      string storedProcedureId,
      PartitionKey partitionKey,
      object[] parameters,
      StoredProcedureRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<StoredProcedureExecuteResponse<TOutput>>(nameof (ExecuteStoredProcedureAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<StoredProcedureExecuteResponse<TOutput>>>) (trace => this.ExecuteStoredProcedureAsync<TOutput>(storedProcedureId, partitionKey, parameters, requestOptions, trace, cancellationToken)), (Func<StoredProcedureExecuteResponse<TOutput>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<TOutput>((Response<TOutput>) response)));
    }

    public override Task<ResponseMessage> ExecuteStoredProcedureStreamAsync(
      string storedProcedureId,
      PartitionKey partitionKey,
      object[] parameters,
      StoredProcedureRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (ExecuteStoredProcedureStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ExecuteStoredProcedureStreamAsync(storedProcedureId, partitionKey, parameters, requestOptions, trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ResponseMessage> ExecuteStoredProcedureStreamAsync(
      string storedProcedureId,
      Stream streamPayload,
      PartitionKey partitionKey,
      StoredProcedureRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (ExecuteStoredProcedureStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ExecuteStoredProcedureStreamAsync(storedProcedureId, streamPayload, partitionKey, requestOptions, trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<TriggerResponse> CreateTriggerAsync(
      TriggerProperties triggerProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<TriggerResponse>(nameof (CreateTriggerAsync), requestOptions, (Func<ITrace, Task<TriggerResponse>>) (trace => this.CreateTriggerAsync(triggerProperties, requestOptions, trace, cancellationToken)), (Func<TriggerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<TriggerProperties>((Response<TriggerProperties>) response)));
    }

    public override FeedIterator<T> GetTriggerQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetTriggerQueryIterator<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetTriggerQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetTriggerQueryStreamIterator(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetTriggerQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetTriggerQueryIterator<T>(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetTriggerQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetTriggerQueryStreamIterator(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override Task<TriggerResponse> ReadTriggerAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<TriggerResponse>(nameof (ReadTriggerAsync), requestOptions, (Func<ITrace, Task<TriggerResponse>>) (trace => this.ReadTriggerAsync(id, requestOptions, trace, cancellationToken)), (Func<TriggerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<TriggerProperties>((Response<TriggerProperties>) response)));
    }

    public override Task<TriggerResponse> ReplaceTriggerAsync(
      TriggerProperties triggerProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<TriggerResponse>(nameof (ReplaceTriggerAsync), requestOptions, (Func<ITrace, Task<TriggerResponse>>) (trace => this.ReplaceTriggerAsync(triggerProperties, requestOptions, trace, cancellationToken)), (Func<TriggerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<TriggerProperties>((Response<TriggerProperties>) response)));
    }

    public override Task<TriggerResponse> DeleteTriggerAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<TriggerResponse>(nameof (DeleteTriggerAsync), requestOptions, (Func<ITrace, Task<TriggerResponse>>) (trace => this.DeleteTriggerAsync(id, requestOptions, trace, cancellationToken)), (Func<TriggerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<TriggerProperties>((Response<TriggerProperties>) response)));
    }

    public override Task<UserDefinedFunctionResponse> CreateUserDefinedFunctionAsync(
      UserDefinedFunctionProperties userDefinedFunctionProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserDefinedFunctionResponse>(nameof (CreateUserDefinedFunctionAsync), requestOptions, (Func<ITrace, Task<UserDefinedFunctionResponse>>) (trace => this.CreateUserDefinedFunctionAsync(userDefinedFunctionProperties, requestOptions, trace, cancellationToken)), (Func<UserDefinedFunctionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserDefinedFunctionProperties>((Response<UserDefinedFunctionProperties>) response)));
    }

    public override FeedIterator<T> GetUserDefinedFunctionQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetUserDefinedFunctionQueryIterator<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetUserDefinedFunctionQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetUserDefinedFunctionQueryStreamIterator(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetUserDefinedFunctionQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetUserDefinedFunctionQueryIterator<T>(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetUserDefinedFunctionQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetUserDefinedFunctionQueryStreamIterator(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override Task<UserDefinedFunctionResponse> ReadUserDefinedFunctionAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserDefinedFunctionResponse>(nameof (ReadUserDefinedFunctionAsync), requestOptions, (Func<ITrace, Task<UserDefinedFunctionResponse>>) (trace => this.ReadUserDefinedFunctionAsync(id, requestOptions, trace, cancellationToken)), (Func<UserDefinedFunctionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserDefinedFunctionProperties>((Response<UserDefinedFunctionProperties>) response)));
    }

    public override Task<UserDefinedFunctionResponse> ReplaceUserDefinedFunctionAsync(
      UserDefinedFunctionProperties userDefinedFunctionProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserDefinedFunctionResponse>(nameof (ReplaceUserDefinedFunctionAsync), requestOptions, (Func<ITrace, Task<UserDefinedFunctionResponse>>) (trace => this.ReplaceUserDefinedFunctionAsync(userDefinedFunctionProperties, requestOptions, trace, cancellationToken)), (Func<UserDefinedFunctionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserDefinedFunctionProperties>((Response<UserDefinedFunctionProperties>) response)));
    }

    public override Task<UserDefinedFunctionResponse> DeleteUserDefinedFunctionAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<UserDefinedFunctionResponse>(nameof (DeleteUserDefinedFunctionAsync), requestOptions, (Func<ITrace, Task<UserDefinedFunctionResponse>>) (trace => this.DeleteUserDefinedFunctionAsync(id, requestOptions, trace, cancellationToken)), (Func<UserDefinedFunctionResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<UserDefinedFunctionProperties>((Response<UserDefinedFunctionProperties>) response)));
    }
  }
}
