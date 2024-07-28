// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosResponseFactoryInternal
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Azure.Documents;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class CosmosResponseFactoryInternal : CosmosResponseFactory
  {
    public abstract FeedResponse<T> CreateChangeFeedUserTypeResponse<T>(
      ResponseMessage responseMessage);

    public abstract FeedResponse<T> CreateQueryFeedUserTypeResponse<T>(
      ResponseMessage responseMessage);

    public abstract FeedResponse<T> CreateQueryFeedResponse<T>(
      ResponseMessage responseMessage,
      ResourceType resourceType);

    public abstract ContainerResponse CreateContainerResponse(
      Container container,
      ResponseMessage responseMessage);

    public abstract UserResponse CreateUserResponse(User user, ResponseMessage responseMessage);

    public abstract PermissionResponse CreatePermissionResponse(
      Permission permission,
      ResponseMessage responseMessage);

    public abstract ClientEncryptionKeyResponse CreateClientEncryptionKeyResponse(
      ClientEncryptionKey clientEncryptionKey,
      ResponseMessage responseMessage);

    public abstract DatabaseResponse CreateDatabaseResponse(
      Database database,
      ResponseMessage responseMessage);

    public abstract ThroughputResponse CreateThroughputResponse(ResponseMessage responseMessage);

    public abstract StoredProcedureResponse CreateStoredProcedureResponse(
      ResponseMessage responseMessage);

    public abstract TriggerResponse CreateTriggerResponse(ResponseMessage responseMessage);

    public abstract UserDefinedFunctionResponse CreateUserDefinedFunctionResponse(
      ResponseMessage responseMessage);
  }
}
