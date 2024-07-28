// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainBlob2Controller
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
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "domains", ResourceName = "blobs", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [SetActivityLogAnonymousIdentifier]
  public sealed class MultiDomainBlob2Controller : BlobControllerBase
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "MultiDomainBlobController"
    };
    private const int ThrottlingThresholdForAddBlockBlob = 87;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BlobExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5704011)]
    public async Task<HttpResponseMessage> GetBlobByIdentifierAsync(string domainId, string blobId)
    {
      MultiDomainBlob2Controller domainBlob2Controller = this;
      return await new MultiDomainBlobHandler(domainBlob2Controller.TfsRequestContext, domainBlob2Controller.Request, DomainIdFactory.Create(domainId)).GetBlobByIdentifierAsync(blobId);
    }

    [HttpPost]
    [ControllerMethodTraceFilter(5704031)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "MultiDomainAddBlockBlobBlock", 87, -1, 5, -1)]
    public async Task<HttpResponseMessage> AddBlockBlobBlockAsync(
      string domainId,
      string blobId,
      string blockHash)
    {
      MultiDomainBlob2Controller domainBlob2Controller = this;
      return await new MultiDomainBlobHandler(domainBlob2Controller.TfsRequestContext, domainBlob2Controller.Request, DomainIdFactory.Create(domainId)).AddBlockBlobBlockAsync(blobId, blockHash);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5704041)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "MultiDomainAddSingleBlockBlob", 87, -1, 5, -1)]
    public async Task<HttpResponseMessage> AddBlobAsync(
      string domainId,
      string blobId,
      string referenceId,
      string referenceScope)
    {
      MultiDomainBlob2Controller domainBlob2Controller = this;
      return await new MultiDomainBlobHandler(domainBlob2Controller.TfsRequestContext, domainBlob2Controller.Request, DomainIdFactory.Create(domainId)).AddSingleBlockBlobAsync(BlobIdentifier.Deserialize(blobId), new BlobReference(referenceId, referenceScope)).ConfigureAwait(true);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(5704051)]
    [CpuThrottlingActionFilter("/Configuration/BlobStore", "MultiDomainAddSingleBlockBlob", 87, -1, 5, -1)]
    public async Task<HttpResponseMessage> AddBlobWithExpirationAsync(
      string domainId,
      string blobId,
      string keepUntil)
    {
      MultiDomainBlob2Controller domainBlob2Controller = this;
      return await new MultiDomainBlobHandler(domainBlob2Controller.TfsRequestContext, domainBlob2Controller.Request, DomainIdFactory.Create(domainId)).AddSingleBlockBlobAsync(BlobIdentifier.Deserialize(blobId), new BlobReference(KeepUntilBlobReference.ParseDate(keepUntil))).ConfigureAwait(true);
    }
  }
}
