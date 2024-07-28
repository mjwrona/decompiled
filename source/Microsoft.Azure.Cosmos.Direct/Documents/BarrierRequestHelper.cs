// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BarrierRequestHelper
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
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
      barrierLsnRequest.Headers["x-ms-date"] = Rfc1123DateTimeCache.UtcNow();
      long num;
      if (targetLsn.HasValue && targetLsn.Value > 0L)
      {
        INameValueCollection headers = barrierLsnRequest.Headers;
        num = targetLsn.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        headers["x-ms-target-lsn"] = str;
      }
      if (targetGlobalCommittedLsn.HasValue && targetGlobalCommittedLsn.Value > 0L)
      {
        INameValueCollection headers = barrierLsnRequest.Headers;
        num = targetGlobalCommittedLsn.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        headers["x-ms-target-global-committed-lsn"] = str;
      }
      switch (authorizationTokenType)
      {
        case AuthorizationTokenType.PrimaryMasterKey:
        case AuthorizationTokenType.PrimaryReadonlyMasterKey:
        case AuthorizationTokenType.SecondaryMasterKey:
        case AuthorizationTokenType.SecondaryReadonlyMasterKey:
          INameValueCollection nameValueCollection = barrierLsnRequest.Headers;
          nameValueCollection["authorization"] = (await authorizationTokenProvider.GetUserAuthorizationAsync(barrierLsnRequest.ResourceAddress, flag ? PathsHelper.GetResourcePath(ResourceType.Collection) : PathsHelper.GetResourcePath(ResourceType.Database), "HEAD", barrierLsnRequest.Headers, authorizationTokenType)).Item1;
          nameValueCollection = (INameValueCollection) null;
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
        case AuthorizationTokenType.AadToken:
          barrierLsnRequest.Headers["authorization"] = request.Headers["authorization"];
          break;
        default:
          DefaultTrace.TraceCritical(string.Format("Unknown authorization token kind [{0}] for read request", (object) authorizationTokenType));
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
      DocumentServiceRequest async = barrierLsnRequest;
      barrierLsnRequest = (DocumentServiceRequest) null;
      return async;
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
        case ResourceType.PartitionedSystemDocument:
        case ResourceType.SystemDocument:
          return true;
        case ResourceType.PartitionKeyRange:
          return false;
        default:
          return false;
      }
    }
  }
}
