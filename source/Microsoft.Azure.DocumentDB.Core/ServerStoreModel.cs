// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ServerStoreModel
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class ServerStoreModel : IStoreModel, IDisposable
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

    public async Task<DocumentServiceResponse> ProcessMessageAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ServerStoreModel sender = this;
      if (sender.DefaultReplicaIndex.HasValue)
        request.DefaultReplicaIndex = sender.DefaultReplicaIndex;
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
      EventHandler<SendingRequestEventArgs> sendingRequest = sender.sendingRequest;
      if (sendingRequest != null)
        sendingRequest((object) sender, new SendingRequestEventArgs(request));
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
