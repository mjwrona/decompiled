// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PermissionCore
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
  internal abstract class PermissionCore : Permission
  {
    private readonly string linkUri;

    internal PermissionCore(CosmosClientContext clientContext, UserCore user, string userId)
    {
      this.Id = userId;
      this.ClientContext = clientContext;
      this.linkUri = clientContext.CreateLink(user.LinkUri, "permissions", userId);
    }

    public override string Id { get; }

    internal CosmosClientContext ClientContext { get; }

    public async Task<PermissionResponse> DeleteAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      PermissionCore permissionCore = this;
      ResponseMessage responseMessage = await permissionCore.DeletePermissionStreamAsync(requestOptions, trace, cancellationToken);
      return permissionCore.ClientContext.ResponseFactory.CreatePermissionResponse((Permission) permissionCore, responseMessage);
    }

    public Task<ResponseMessage> DeletePermissionStreamAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessStreamAsync((Stream) null, OperationType.Delete, new int?(), requestOptions, trace, cancellationToken);
    }

    public async Task<PermissionResponse> ReadAsync(
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      PermissionCore permissionCore = this;
      ResponseMessage responseMessage = await permissionCore.ReadPermissionStreamAsync(tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
      return permissionCore.ClientContext.ResponseFactory.CreatePermissionResponse((Permission) permissionCore, responseMessage);
    }

    public Task<ResponseMessage> ReadPermissionStreamAsync(
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessStreamAsync((Stream) null, OperationType.Read, tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
    }

    public async Task<PermissionResponse> ReplaceAsync(
      PermissionProperties permissionProperties,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      PermissionCore permissionCore = this;
      if (permissionProperties == null)
        throw new ArgumentNullException(nameof (permissionProperties));
      permissionCore.ClientContext.ValidateResource(permissionProperties.Id);
      ResponseMessage responseMessage = await permissionCore.ReplaceStreamInternalAsync(permissionCore.ClientContext.SerializerCore.ToStream<PermissionProperties>(permissionProperties), tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
      return permissionCore.ClientContext.ResponseFactory.CreatePermissionResponse((Permission) permissionCore, responseMessage);
    }

    public Task<ResponseMessage> ReplacePermissionStreamAsync(
      PermissionProperties permissionProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (permissionProperties == null)
        throw new ArgumentNullException(nameof (permissionProperties));
      this.ClientContext.ValidateResource(permissionProperties.Id);
      return this.ReplaceStreamInternalAsync(this.ClientContext.SerializerCore.ToStream<PermissionProperties>(permissionProperties), new int?(), requestOptions, trace, cancellationToken);
    }

    private Task<ResponseMessage> ReplaceStreamInternalAsync(
      Stream streamPayload,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessStreamAsync(streamPayload, OperationType.Replace, tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
    }

    private Task<ResponseMessage> ProcessStreamAsync(
      Stream streamPayload,
      OperationType operationType,
      int? tokenExpiryInSeconds,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ProcessResourceOperationStreamAsync(streamPayload, operationType, this.linkUri, ResourceType.Permission, tokenExpiryInSeconds, requestOptions, trace, cancellationToken);
    }

    private Task<ResponseMessage> ProcessResourceOperationStreamAsync(
      Stream streamPayload,
      OperationType operationType,
      string linkUri,
      ResourceType resourceType,
      int? tokenExpiryInSeconds,
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
      Action<RequestMessage> requestEnricher = (Action<RequestMessage>) (requestMessage =>
      {
        if (!tokenExpiryInSeconds.HasValue)
          return;
        requestMessage.Headers.Add("x-ms-documentdb-expiry-seconds", tokenExpiryInSeconds.Value.ToString());
      });
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(resourceUri, (ResourceType) num1, (OperationType) num2, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, requestEnricher, trace1, cancellationToken1);
    }
  }
}
