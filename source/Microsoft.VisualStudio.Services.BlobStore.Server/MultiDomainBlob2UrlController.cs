// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainBlob2UrlController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "domains", ResourceName = "url", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [SetActivityLogAnonymousIdentifier]
  public sealed class MultiDomainBlob2UrlController : BlobControllerBase
  {
    private const int TracepointBlob2Base = 5704001;
    private const int TracepointGetBlobUrlByIdentifierAsync = 5704021;

    [HttpGet]
    [ControllerMethodTraceFilter(5704021)]
    [ClientResponseType(typeof (Blob), null, null)]
    public async Task<HttpResponseMessage> GetBlobUrlByIdentifierAsync(
      string domainId,
      string blobId,
      string filename = null,
      string contentType = null,
      string expiryTime = null,
      bool allowEdge = false)
    {
      MultiDomainBlob2UrlController blob2UrlController = this;
      TraceInfo traceInfo = new TraceInfo(5704001, blob2UrlController.ShouldTraceMethodDetail(nameof (GetBlobUrlByIdentifierAsync)), blob2UrlController.ControllerContext.ControllerDescriptor.ControllerName, blob2UrlController.TraceArea);
      return await new MultiDomainBlobUrlHandler(blob2UrlController.TfsRequestContext, DomainIdFactory.Create(domainId), blob2UrlController.Request, traceInfo).GetBlobUrlByIdentifierAsync(blobId, filename, contentType, expiryTime, allowEdge);
    }
  }
}
