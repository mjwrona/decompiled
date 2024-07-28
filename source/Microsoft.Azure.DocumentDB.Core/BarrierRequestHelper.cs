// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BarrierRequestHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal static class BarrierRequestHelper
  {
    public static async Task<DocumentServiceRequest> CreateAsync(
      DocumentServiceRequest request,
      IAuthorizationTokenProvider authorizationTokenProvider,
      long? targetLsn,
      long? targetGlobalCommittedLsn)
    {
      bool flag = BarrierRequestHelper.IsCollectionHeadBarrierRequest(request.ResourceType, request.OperationType);
      if (request.ServiceIdentity != null && request.ServiceIdentity.IsMasterService)
        flag = false;
      if (request.RequestAuthorizationTokenType == AuthorizationTokenType.Invalid)
        DefaultTrace.TraceCritical("AuthorizationTokenType not set for the read request");
      AuthorizationTokenType authorizationTokenType = request.RequestAuthorizationTokenType;
      DocumentServiceRequest barrierLsnRequest = (DocumentServiceRequest) null;
      barrierLsnRequest = flag ? (!request.IsNameBased ? DocumentServiceRequest.Create(OperationType.Head, ResourceId.Parse(request.ResourceId).DocumentCollectionId.ToString(), ResourceType.Collection, (Stream) null, authorizationTokenType) : DocumentServiceRequest.CreateFromName(OperationType.Head, PathsHelper.GetCollectionPath(request.ResourceAddress), ResourceType.Collection, authorizationTokenType)) : DocumentServiceRequest.Create(OperationType.HeadFeed, (string) null, ResourceType.Database, authorizationTokenType);
      barrierLsnRequest.Headers["x-ms-date"] = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
      if (targetLsn.HasValue && targetLsn.Value > 0L)
        barrierLsnRequest.Headers["x-ms-target-lsn"] = targetLsn.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (targetGlobalCommittedLsn.HasValue && targetGlobalCommittedLsn.Value > 0L)
        barrierLsnRequest.Headers["x-ms-target-global-committed-lsn"] = targetGlobalCommittedLsn.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      switch (authorizationTokenType)
      {
        case AuthorizationTokenType.PrimaryMasterKey:
        case AuthorizationTokenType.PrimaryReadonlyMasterKey:
        case AuthorizationTokenType.SecondaryMasterKey:
        case AuthorizationTokenType.SecondaryReadonlyMasterKey:
          barrierLsnRequest.Headers["authorization"] = authorizationTokenProvider.GetUserAuthorizationToken(barrierLsnRequest.ResourceAddress, flag ? PathsHelper.GetResourcePath(ResourceType.Collection) : PathsHelper.GetResourcePath(ResourceType.Database), "HEAD", barrierLsnRequest.Headers, authorizationTokenType, out string _);
          break;
        case AuthorizationTokenType.SystemReadOnly:
        case AuthorizationTokenType.SystemReadWrite:
        case AuthorizationTokenType.SystemAll:
          if (request.RequestContext.TargetIdentity == null)
          {
            DefaultTrace.TraceCritical("TargetIdentity is needed to create the ReadBarrier request");
            throw new InternalServerErrorException(RMResources.InternalServerError);
          }
          await authorizationTokenProvider.AddSystemAuthorizationHeaderAsync(barrierLsnRequest, request.RequestContext.TargetIdentity.FederationId, "HEAD", (string) null);
          break;
        case AuthorizationTokenType.ResourceToken:
          barrierLsnRequest.Headers["authorization"] = request.Headers["authorization"];
          break;
        default:
          DefaultTrace.TraceCritical("Unknown authorization token kind for read request");
          throw new InternalServerErrorException(RMResources.InternalServerError);
      }
      barrierLsnRequest.RequestContext = request.RequestContext.Clone();
      if (request.ServiceIdentity != null)
        barrierLsnRequest.RouteTo(request.ServiceIdentity);
      if (request.PartitionKeyRangeIdentity != null)
        barrierLsnRequest.RouteTo(request.PartitionKeyRangeIdentity);
      if (request.Headers["x-ms-documentdb-partitionkey"] != null)
        barrierLsnRequest.Headers["x-ms-documentdb-partitionkey"] = request.Headers["x-ms-documentdb-partitionkey"];
      if (request.Headers["x-ms-documentdb-collection-rid"] != null)
        barrierLsnRequest.Headers["x-ms-documentdb-collection-rid"] = request.Headers["x-ms-documentdb-collection-rid"];
      if (request.Properties != null && request.Properties.ContainsKey("x-ms-effective-partition-key-string"))
      {
        if (barrierLsnRequest.Properties == null)
          barrierLsnRequest.Properties = (IDictionary<string, object>) new Dictionary<string, object>();
        barrierLsnRequest.Properties["x-ms-effective-partition-key-string"] = request.Properties["x-ms-effective-partition-key-string"];
      }
      return barrierLsnRequest;
    }

    internal static bool IsCollectionHeadBarrierRequest(
      ResourceType resourceType,
      OperationType operationType)
    {
      switch (resourceType)
      {
        case ResourceType.Collection:
          return operationType != OperationType.ReadFeed && operationType != OperationType.Query && operationType != OperationType.SqlQuery;
        case ResourceType.Document:
        case ResourceType.Attachment:
        case ResourceType.Conflict:
        case ResourceType.StoredProcedure:
        case ResourceType.Trigger:
        case ResourceType.UserDefinedFunction:
          return true;
        case ResourceType.PartitionKeyRange:
          return operationType == OperationType.GetSplitPoint || operationType == OperationType.AbortSplit || operationType == OperationType.GetSplitPoints;
        default:
          return false;
      }
    }
  }
}
