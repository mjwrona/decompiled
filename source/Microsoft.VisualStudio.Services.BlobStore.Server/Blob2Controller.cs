// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Blob2Controller
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "blob", ResourceName = "blobs", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [SetActivityLogAnonymousIdentifier]
  public sealed class Blob2Controller : BlobControllerBase
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Controller"
    };
    private const int ThrottlingThresholdForAddBlockBlob = 87;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BlobExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5704010)]
    public async Task<HttpResponseMessage> GetBlobByIdentifierAsync(string blobId)
    {
      Blob2Controller blob2Controller = this;
      return await new MultiDomainBlobHandler(blob2Controller.TfsRequestContext, blob2Controller.Request, WellKnownDomainIds.DefaultDomainId).GetBlobByIdentifierAsync(blobId);
    }

    [HttpPost]
    [ControllerMethodTraceFilter(5704030)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "AddBlockBlobBlock", 87, -1, 5, -1)]
    public async Task<HttpResponseMessage> AddBlockBlobBlockAsync(string blobId, string blockHash)
    {
      Blob2Controller blob2Controller = this;
      return await new MultiDomainBlobHandler(blob2Controller.TfsRequestContext, blob2Controller.Request, WellKnownDomainIds.DefaultDomainId).AddBlockBlobBlockAsync(blobId, blockHash);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5704040)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "AddSingleBlockBlob", 87, -1, 5, -1)]
    public async Task<HttpResponseMessage> AddBlobAsync(
      string blobId,
      string referenceId,
      string referenceScope)
    {
      Blob2Controller blob2Controller = this;
      return await new MultiDomainBlobHandler(blob2Controller.TfsRequestContext, blob2Controller.Request, WellKnownDomainIds.DefaultDomainId).AddSingleBlockBlobAsync(BlobIdentifier.Deserialize(blobId), new BlobReference(referenceId, referenceScope)).ConfigureAwait(true);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5704050)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "AddSingleBlockBlob", 87, -1, 5, -1)]
    public async Task<HttpResponseMessage> AddBlobWithExpirationAsync(
      string blobId,
      string keepUntil)
    {
      Blob2Controller blob2Controller = this;
      return await new MultiDomainBlobHandler(blob2Controller.TfsRequestContext, blob2Controller.Request, WellKnownDomainIds.DefaultDomainId).AddSingleBlockBlobAsync(BlobIdentifier.Deserialize(blobId), new BlobReference(KeepUntilBlobReference.ParseDate(keepUntil))).ConfigureAwait(true);
    }
  }
}
