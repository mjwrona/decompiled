// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdTransportClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
      if (this.rntbdConnectionManager == null)
        return;
      this.rntbdConnectionManager.Dispose();
    }

    internal override async Task<StoreResponse> InvokeStoreAsync(
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request)
    {
      Guid activityId = Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId;
      if (!request.IsBodySeekableClonableAndCountable)
        throw new InternalServerErrorException();
      IConnection connection;
      try
      {
        connection = await this.rntbdConnectionManager.GetOpenConnection(activityId, physicalAddress);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("GetOpenConnection failed: RID: {0}, ResourceType {1}, Op: {2}, Address: {3}, Exception: {4}", (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) physicalAddress, (object) ex);
        throw;
      }
      StoreResponse storeResponse;
      try
      {
        storeResponse = await connection.RequestAsync(request, physicalAddress, resourceOperation, activityId);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("RequestAsync failed: RID: {0}, ResourceType {1}, Op: {2}, Address: {3}, Exception: {4}", (object) request.ResourceAddress, (object) request.ResourceType, (object) resourceOperation, (object) physicalAddress, (object) ex);
        connection.Close();
        throw;
      }
      this.rntbdConnectionManager.ReturnToPool(connection);
      TransportClient.ThrowServerException(request.ResourceAddress, storeResponse, physicalAddress, activityId, request);
      return storeResponse;
    }
  }
}
