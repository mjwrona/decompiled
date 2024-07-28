// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainBlobBatchController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
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
  [VersionedApiControllerCustomName(Area = "domains", ResourceName = "blobsbatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [SetActivityLogAnonymousIdentifier]
  public sealed class MultiDomainBlobBatchController : BlobControllerBase
  {
    private const int TracepointBlobBatchBase = 5705001;
    private const int TracepointGetDownloadUrisAsync = 5705021;
    private const int TracepointGetSASTokens = 5705031;
    private const int TracepointBlobBatchWarning = 5705991;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BlobExceptionMapping.ServerErrorMap;

    [HttpPost]
    [ControllerMethodTraceFilter(5705021)]
    [ClientRequestBodyIsStream]
    [ClientResponseType(typeof (BlobBatch), null, null)]
    public async Task<HttpResponseMessage> GetDownloadUrisAsync(
      string domainId,
      string expiryTime = null,
      bool allowEdge = false)
    {
      MultiDomainBlobBatchController blobBatchController = this;
      TraceInfo traceInfo = new TraceInfo(5705001, blobBatchController.ShouldTraceMethodDetail(nameof (GetDownloadUrisAsync)), blobBatchController.ControllerContext.ControllerDescriptor.ControllerName, blobBatchController.TraceArea);
      return await new MultiDomainBlobBatchHandler(blobBatchController.TfsRequestContext, DomainIdFactory.Create(domainId), blobBatchController.Request, traceInfo).GetDownloadUrisAsync(expiryTime, allowEdge);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(5705031)]
    [FeatureEnabled("BlobStore.Features.ShardMap")]
    public async Task<BlobMappings> GetSASTokens(string domainId)
    {
      MultiDomainBlobBatchController blobBatchController = this;
      TraceInfo traceInfo = new TraceInfo(5705001, blobBatchController.ShouldTraceMethodDetail(nameof (GetSASTokens)), blobBatchController.ControllerContext.ControllerDescriptor.ControllerName, blobBatchController.TraceArea);
      return await new MultiDomainBlobBatchHandler(blobBatchController.TfsRequestContext, DomainIdFactory.Create(domainId), blobBatchController.Request, traceInfo).GetSASTokens();
    }
  }
}
