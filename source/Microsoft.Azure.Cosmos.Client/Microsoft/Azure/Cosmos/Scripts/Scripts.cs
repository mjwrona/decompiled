// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Scripts.Scripts
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Scripts
{
  public abstract class Scripts
  {
    public abstract Task<StoredProcedureResponse> CreateStoredProcedureAsync(
      StoredProcedureProperties storedProcedureProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator<T> GetStoredProcedureQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetStoredProcedureQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetStoredProcedureQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetStoredProcedureQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract Task<StoredProcedureResponse> ReadStoredProcedureAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<StoredProcedureResponse> ReplaceStoredProcedureAsync(
      StoredProcedureProperties storedProcedureProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<StoredProcedureResponse> DeleteStoredProcedureAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<StoredProcedureExecuteResponse<TOutput>> ExecuteStoredProcedureAsync<TOutput>(
      string storedProcedureId,
      PartitionKey partitionKey,
      object[] parameters,
      StoredProcedureRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> ExecuteStoredProcedureStreamAsync(
      string storedProcedureId,
      PartitionKey partitionKey,
      object[] parameters,
      StoredProcedureRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> ExecuteStoredProcedureStreamAsync(
      string storedProcedureId,
      Stream streamPayload,
      PartitionKey partitionKey,
      StoredProcedureRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<TriggerResponse> CreateTriggerAsync(
      TriggerProperties triggerProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator<T> GetTriggerQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetTriggerQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetTriggerQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetTriggerQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract Task<TriggerResponse> ReadTriggerAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<TriggerResponse> ReplaceTriggerAsync(
      TriggerProperties triggerProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<TriggerResponse> DeleteTriggerAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<UserDefinedFunctionResponse> CreateUserDefinedFunctionAsync(
      UserDefinedFunctionProperties userDefinedFunctionProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator<T> GetUserDefinedFunctionQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetUserDefinedFunctionQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetUserDefinedFunctionQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetUserDefinedFunctionQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract Task<UserDefinedFunctionResponse> ReadUserDefinedFunctionAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<UserDefinedFunctionResponse> ReplaceUserDefinedFunctionAsync(
      UserDefinedFunctionProperties userDefinedFunctionProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<UserDefinedFunctionResponse> DeleteUserDefinedFunctionAsync(
      string id,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
