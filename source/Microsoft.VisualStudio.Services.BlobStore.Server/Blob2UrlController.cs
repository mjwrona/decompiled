// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Blob2UrlController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

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
  [VersionedApiControllerCustomName(Area = "blob", ResourceName = "url", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [SetActivityLogAnonymousIdentifier]
  public sealed class Blob2UrlController : BlobControllerBase
  {
    private const int TracepointBlob2Base = 5704000;
    private const int TracepointGetBlobUrlByIdentifierAsync = 5704020;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BlobExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5704020)]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob), null, null)]
    public async Task<HttpResponseMessage> GetBlobUrlByIdentifierAsync(
      string blobId,
      string filename = null,
      string contentType = null,
      string expiryTime = null,
      bool allowEdge = false)
    {
      Blob2UrlController blob2UrlController = this;
      TraceInfo traceInfo = new TraceInfo(5704000, blob2UrlController.ShouldTraceMethodDetail(nameof (GetBlobUrlByIdentifierAsync)), blob2UrlController.ControllerContext.ControllerDescriptor.ControllerName, blob2UrlController.TraceArea);
      return await new MultiDomainBlobUrlHandler(blob2UrlController.TfsRequestContext, WellKnownDomainIds.DefaultDomainId, blob2UrlController.Request, traceInfo).GetBlobUrlByIdentifierAsync(blobId, filename, contentType, expiryTime, allowEdge);
    }
  }
}
