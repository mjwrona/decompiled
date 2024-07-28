// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosResponseFactoryCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class CosmosResponseFactoryCore : CosmosResponseFactoryInternal
  {
    private readonly CosmosSerializerCore serializerCore;

    public CosmosResponseFactoryCore(CosmosSerializerCore jsonSerializerCore) => this.serializerCore = jsonSerializerCore;

    public override FeedResponse<T> CreateItemFeedResponse<T>(ResponseMessage responseMessage) => this.CreateQueryFeedResponseHelper<T>(responseMessage);

    public override FeedResponse<T> CreateChangeFeedUserTypeResponse<T>(
      ResponseMessage responseMessage)
    {
      return this.CreateChangeFeedResponseHelper<T>(responseMessage);
    }

    public override FeedResponse<T> CreateQueryFeedUserTypeResponse<T>(
      ResponseMessage responseMessage)
    {
      return this.CreateQueryFeedResponseHelper<T>(responseMessage);
    }

    public override FeedResponse<T> CreateQueryFeedResponse<T>(
      ResponseMessage responseMessage,
      ResourceType resourceType)
    {
      return this.CreateQueryFeedResponseHelper<T>(responseMessage);
    }

    private FeedResponse<T> CreateQueryFeedResponseHelper<T>(ResponseMessage cosmosResponseMessage)
    {
      if (cosmosResponseMessage is QueryResponse cosmosQueryResponse)
      {
        using (cosmosResponseMessage.Trace.StartChild("Query Response Serialization"))
          return (FeedResponse<T>) QueryResponse<T>.CreateResponse<T>(cosmosQueryResponse, this.serializerCore);
      }
      else
      {
        using (cosmosResponseMessage.Trace.StartChild("Feed Response Serialization"))
          return (FeedResponse<T>) ReadFeedResponse<T>.CreateResponse<T>(cosmosResponseMessage, this.serializerCore);
      }
    }

    private FeedResponse<T> CreateChangeFeedResponseHelper<T>(ResponseMessage cosmosResponseMessage)
    {
      using (cosmosResponseMessage.Trace.StartChild("ChangeFeed Response Serialization"))
        return (FeedResponse<T>) ReadFeedResponse<T>.CreateResponse<T>(cosmosResponseMessage, this.serializerCore);
    }

    public override ItemResponse<T> CreateItemResponse<T>(ResponseMessage responseMessage) => this.ProcessMessage<ItemResponse<T>>(responseMessage, (Func<ResponseMessage, ItemResponse<T>>) (cosmosResponseMessage =>
    {
      T objectpublic = this.ToObjectpublic<T>(cosmosResponseMessage);
      return new ItemResponse<T>(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
    }));

    public override ContainerResponse CreateContainerResponse(
      Container container,
      ResponseMessage responseMessage)
    {
      return this.ProcessMessage<ContainerResponse>(responseMessage, (Func<ResponseMessage, ContainerResponse>) (cosmosResponseMessage =>
      {
        ContainerProperties objectpublic = this.ToObjectpublic<ContainerProperties>(cosmosResponseMessage);
        return new ContainerResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, container, cosmosResponseMessage.Diagnostics, responseMessage.RequestMessage);
      }));
    }

    public override UserResponse CreateUserResponse(User user, ResponseMessage responseMessage) => this.ProcessMessage<UserResponse>(responseMessage, (Func<ResponseMessage, UserResponse>) (cosmosResponseMessage =>
    {
      UserProperties objectpublic = this.ToObjectpublic<UserProperties>(cosmosResponseMessage);
      return new UserResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, user, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
    }));

    public override PermissionResponse CreatePermissionResponse(
      Permission permission,
      ResponseMessage responseMessage)
    {
      return this.ProcessMessage<PermissionResponse>(responseMessage, (Func<ResponseMessage, PermissionResponse>) (cosmosResponseMessage =>
      {
        PermissionProperties objectpublic = this.ToObjectpublic<PermissionProperties>(cosmosResponseMessage);
        return new PermissionResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, permission, cosmosResponseMessage.Diagnostics, responseMessage.RequestMessage);
      }));
    }

    public override ClientEncryptionKeyResponse CreateClientEncryptionKeyResponse(
      ClientEncryptionKey clientEncryptionKey,
      ResponseMessage responseMessage)
    {
      return this.ProcessMessage<ClientEncryptionKeyResponse>(responseMessage, (Func<ResponseMessage, ClientEncryptionKeyResponse>) (cosmosResponseMessage =>
      {
        ClientEncryptionKeyProperties objectpublic = this.ToObjectpublic<ClientEncryptionKeyProperties>(cosmosResponseMessage);
        return new ClientEncryptionKeyResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, clientEncryptionKey, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
      }));
    }

    public override DatabaseResponse CreateDatabaseResponse(
      Database database,
      ResponseMessage responseMessage)
    {
      return this.ProcessMessage<DatabaseResponse>(responseMessage, (Func<ResponseMessage, DatabaseResponse>) (cosmosResponseMessage =>
      {
        DatabaseProperties objectpublic = this.ToObjectpublic<DatabaseProperties>(cosmosResponseMessage);
        return new DatabaseResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, database, cosmosResponseMessage.Diagnostics, responseMessage.RequestMessage);
      }));
    }

    public override ThroughputResponse CreateThroughputResponse(ResponseMessage responseMessage) => this.ProcessMessage<ThroughputResponse>(responseMessage, (Func<ResponseMessage, ThroughputResponse>) (cosmosResponseMessage =>
    {
      ThroughputProperties objectpublic = this.ToObjectpublic<ThroughputProperties>(cosmosResponseMessage);
      return new ThroughputResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
    }));

    public override StoredProcedureExecuteResponse<T> CreateStoredProcedureExecuteResponse<T>(
      ResponseMessage responseMessage)
    {
      return this.ProcessMessage<StoredProcedureExecuteResponse<T>>(responseMessage, (Func<ResponseMessage, StoredProcedureExecuteResponse<T>>) (cosmosResponseMessage =>
      {
        T objectpublic = this.ToObjectpublic<T>(cosmosResponseMessage);
        return new StoredProcedureExecuteResponse<T>(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
      }));
    }

    public override StoredProcedureResponse CreateStoredProcedureResponse(
      ResponseMessage responseMessage)
    {
      return this.ProcessMessage<StoredProcedureResponse>(responseMessage, (Func<ResponseMessage, StoredProcedureResponse>) (cosmosResponseMessage =>
      {
        StoredProcedureProperties objectpublic = this.ToObjectpublic<StoredProcedureProperties>(cosmosResponseMessage);
        return new StoredProcedureResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
      }));
    }

    public override TriggerResponse CreateTriggerResponse(ResponseMessage responseMessage) => this.ProcessMessage<TriggerResponse>(responseMessage, (Func<ResponseMessage, TriggerResponse>) (cosmosResponseMessage =>
    {
      TriggerProperties objectpublic = this.ToObjectpublic<TriggerProperties>(cosmosResponseMessage);
      return new TriggerResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
    }));

    public override UserDefinedFunctionResponse CreateUserDefinedFunctionResponse(
      ResponseMessage responseMessage)
    {
      return this.ProcessMessage<UserDefinedFunctionResponse>(responseMessage, (Func<ResponseMessage, UserDefinedFunctionResponse>) (cosmosResponseMessage =>
      {
        UserDefinedFunctionProperties objectpublic = this.ToObjectpublic<UserDefinedFunctionProperties>(cosmosResponseMessage);
        return new UserDefinedFunctionResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, objectpublic, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
      }));
    }

    public T ProcessMessage<T>(
      ResponseMessage responseMessage,
      Func<ResponseMessage, T> createResponse)
    {
      using (ResponseMessage responseMessage1 = responseMessage)
      {
        responseMessage1.EnsureSuccessStatusCode();
        using (responseMessage1.Trace.StartChild("Response Serialization"))
          return createResponse(responseMessage1);
      }
    }

    public T ToObjectpublic<T>(ResponseMessage responseMessage) => responseMessage.Content == null ? default (T) : this.serializerCore.FromStream<T>(responseMessage.Content);
  }
}
