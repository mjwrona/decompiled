// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClientEncryptionKeyCore
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
  internal class ClientEncryptionKeyCore : ClientEncryptionKey
  {
    protected ClientEncryptionKeyCore()
    {
    }

    public ClientEncryptionKeyCore(
      CosmosClientContext clientContext,
      DatabaseInternal database,
      string keyId)
    {
      this.Id = keyId;
      this.ClientContext = clientContext;
      this.LinkUri = ClientEncryptionKeyCore.CreateLinkUri(clientContext, database, keyId);
      this.Database = (Database) database;
    }

    public override string Id { get; }

    public virtual Database Database { get; }

    public virtual string LinkUri { get; }

    public virtual CosmosClientContext ClientContext { get; }

    public override async Task<ClientEncryptionKeyResponse> ReadAsync(
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ReadInternalAsync(requestOptions, (ITrace) NoOpTrace.Singleton, cancellationToken);
    }

    public override async Task<ClientEncryptionKeyResponse> ReplaceAsync(
      ClientEncryptionKeyProperties clientEncryptionKeyProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ReplaceInternalAsync(clientEncryptionKeyProperties, requestOptions, (ITrace) NoOpTrace.Singleton, cancellationToken);
    }

    public static string CreateLinkUri(
      CosmosClientContext clientContext,
      DatabaseInternal database,
      string keyId)
    {
      return clientContext.CreateLink(database.LinkUri, "clientencryptionkeys", keyId);
    }

    private async Task<ClientEncryptionKeyResponse> ReadInternalAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ClientEncryptionKeyCore encryptionKeyCore = this;
      ResponseMessage responseMessage = await encryptionKeyCore.ProcessStreamAsync((Stream) null, OperationType.Read, requestOptions, trace, cancellationToken);
      return encryptionKeyCore.ClientContext.ResponseFactory.CreateClientEncryptionKeyResponse((ClientEncryptionKey) encryptionKeyCore, responseMessage);
    }

    private async Task<ClientEncryptionKeyResponse> ReplaceInternalAsync(
      ClientEncryptionKeyProperties clientEncryptionKeyProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ClientEncryptionKeyCore encryptionKeyCore = this;
      ResponseMessage responseMessage = await encryptionKeyCore.ProcessStreamAsync(encryptionKeyCore.ClientContext.SerializerCore.ToStream<ClientEncryptionKeyProperties>(clientEncryptionKeyProperties), OperationType.Replace, requestOptions, trace, cancellationToken);
      return encryptionKeyCore.ClientContext.ResponseFactory.CreateClientEncryptionKeyResponse((ClientEncryptionKey) encryptionKeyCore, responseMessage);
    }

    private Task<ResponseMessage> ProcessStreamAsync(
      Stream streamPayload,
      OperationType operationType,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CosmosClientContext clientContext = this.ClientContext;
      string linkUri = this.LinkUri;
      int num = (int) operationType;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.ClientEncryptionKey, (OperationType) num, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, (Action<RequestMessage>) null, trace1, cancellationToken1);
    }
  }
}
