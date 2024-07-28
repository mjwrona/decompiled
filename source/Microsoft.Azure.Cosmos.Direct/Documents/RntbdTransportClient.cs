// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdTransportClient
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class RntbdTransportClient : TransportClient
  {
    private readonly ConnectionPoolManager rntbdConnectionManager;

    public RntbdTransportClient(
      int requestTimeout,
      int maxConcurrentConnectionOpenRequests,
      UserAgentContainer userAgent = null,
      string overrideHostNameInCertificate = null,
      int openTimeoutInSeconds = 0,
      int idleTimeoutInSeconds = 100,
      int timerPoolGranularityInSeconds = 0)
    {
      this.rntbdConnectionManager = new ConnectionPoolManager((IConnectionDispenser) new RntbdConnectionDispenser(requestTimeout, overrideHostNameInCertificate, openTimeoutInSeconds, idleTimeoutInSeconds, timerPoolGranularityInSeconds, userAgent), maxConcurrentConnectionOpenRequests);
    }

    public override void Dispose()
    {
      base.Dispose();
      this.rntbdConnectionManager?.Dispose();
    }

    internal override Task<StoreResponse> InvokeStoreAsync(
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request)
    {
      return this.InvokeStoreAsync(new TransportAddressUri(physicalAddress), resourceOperation, request);
    }

    internal override async Task<StoreResponse> InvokeStoreAsync(
      TransportAddressUri physicalAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request)
    {
      Guid activityId = System.Diagnostics.Trace.CorrelationManager.ActivityId;
      if (!request.IsBodySeekableClonableAndCountable)
        throw new InternalServerErrorException();
      IConnection connection;
      try
      {
        connection = await this.rntbdConnectionManager.GetOpenConnection(activityId, physicalAddress.Uri);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("GetOpenConnection failed: RID: {0}, ResourceType {1}, Op: {2}, Address: {3}, Exception: {4}", (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) physicalAddress, (object) ex);
        throw;
      }
      StoreResponse storeResponse1;
      try
      {
        storeResponse1 = await connection.RequestAsync(request, physicalAddress.Uri, resourceOperation, activityId);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("RequestAsync failed: RID: {0}, ResourceType {1}, Op: {2}, Address: {3}, Exception: {4}", (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) physicalAddress, (object) ex);
        connection.Close();
        throw;
      }
      this.rntbdConnectionManager.ReturnToPool(connection);
      TransportClient.ThrowServerException(request.ResourceAddress, storeResponse1, physicalAddress.Uri, activityId, request);
      StoreResponse storeResponse2 = storeResponse1;
      connection = (IConnection) null;
      return storeResponse2;
    }
  }
}
