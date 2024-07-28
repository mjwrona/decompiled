// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ServerStoreModel
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ServerStoreModel : IStoreModelExtension, IStoreModel, IDisposable
  {
    private readonly StoreClient storeClient;
    private EventHandler<SendingRequestEventArgs> sendingRequest;
    private readonly EventHandler<ReceivedResponseEventArgs> receivedResponse;

    public ServerStoreModel(StoreClient storeClient) => this.storeClient = storeClient;

    public ServerStoreModel(
      StoreClient storeClient,
      EventHandler<SendingRequestEventArgs> sendingRequest,
      EventHandler<ReceivedResponseEventArgs> receivedResponse)
      : this(storeClient)
    {
      this.sendingRequest = sendingRequest;
      this.receivedResponse = receivedResponse;
    }

    public uint? DefaultReplicaIndex { get; set; }

    public string LastReadAddress
    {
      get => this.storeClient.LastReadAddress;
      set => this.storeClient.LastReadAddress = value;
    }

    public bool ForceAddressRefresh
    {
      get => this.storeClient.ForceAddressRefresh;
      set => this.storeClient.ForceAddressRefresh = value;
    }

    public Task<DocumentServiceResponse> ProcessMessageAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.DefaultReplicaIndex.HasValue)
        request.DefaultReplicaIndex = this.DefaultReplicaIndex;
      string header = request.Headers["x-ms-consistency-level"];
      request.RequestContext.OriginalRequestConsistencyLevel = new ConsistencyLevel?();
      if (!string.IsNullOrEmpty(header))
      {
        ConsistencyLevel result;
        if (!Enum.TryParse<ConsistencyLevel>(header, out result))
          throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) header, (object) "x-ms-consistency-level"));
        request.RequestContext.OriginalRequestConsistencyLevel = new ConsistencyLevel?(result);
      }
      if (ReplicatedResourceClient.IsMasterResource(request.ResourceType))
        request.Headers["x-ms-consistency-level"] = ConsistencyLevel.Strong.ToString();
      EventHandler<SendingRequestEventArgs> sendingRequest = this.sendingRequest;
      if (sendingRequest != null)
        sendingRequest((object) this, new SendingRequestEventArgs(request));
      return this.receivedResponse != null ? this.ProcessMessageWithReceivedResponseDelegateAsync(request, cancellationToken) : this.storeClient.ProcessMessageAsync(request, cancellationToken);
    }

    public async Task OpenConnectionsToAllReplicasAsync(
      string databaseName,
      string containerLinkUri,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await this.storeClient.OpenConnectionsToAllReplicasAsync(databaseName, containerLinkUri, cancellationToken);
    }

    private async Task<DocumentServiceResponse> ProcessMessageWithReceivedResponseDelegateAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ServerStoreModel sender = this;
      DocumentServiceResponse response = await sender.storeClient.ProcessMessageAsync(request, cancellationToken);
      EventHandler<ReceivedResponseEventArgs> receivedResponse = sender.receivedResponse;
      if (receivedResponse != null)
        receivedResponse((object) sender, new ReceivedResponseEventArgs(request, response));
      return response;
    }

    public void Dispose()
    {
    }
  }
}
