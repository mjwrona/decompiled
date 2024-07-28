// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupBlobControllers.MultiDomainDedupBlobController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher;
using Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher;
using Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupBlobControllers
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "blobs", ResourceVersion = 1)]
  [FeatureEnabled("Blobstore.Features.MultiDomainOperations")]
  [FeatureEnabled("Blobstore.Features.ServerDedup")]
  [RequestContentTypeRestriction(AllowStream = true)]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  [Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup.ServerSideDedup]
  [CpuThrottlingActionFilter("/Configuration/BlobStore", "DedupSessionsController", 87, -1, 5, -1)]
  public class MultiDomainDedupBlobController : BlobControllerBase
  {
    private static readonly HttpClient _chunkHttpClient = new HttpClient()
    {
      Timeout = TimeSpan.FromSeconds(5.0)
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DedupExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5708100)]
    public async Task<IHttpActionResult> GetFileStreamAsync(
      string domainId,
      string nodeId,
      string filename = null)
    {
      MultiDomainDedupBlobController dedupBlobController = this;
      if (string.IsNullOrWhiteSpace(domainId))
        return (IHttpActionResult) dedupBlobController.BadRequest("domainId was null or empty.");
      if (string.IsNullOrWhiteSpace(nodeId))
        return (IHttpActionResult) dedupBlobController.BadRequest("nodeId was null or empty.");
      DedupIdentifier nodeId1 = DedupIdentifier.Deserialize(nodeId);
      IDomainId domainId1 = DomainIdFactory.Create(domainId);
      IHttpActionResult fileStreamAsync;
      if (dedupBlobController.TfsRequestContext.IsFeatureEnabled("Blobstore.Features.EnableContentStitcher"))
        fileStreamAsync = await dedupBlobController.StitchViaContentStitcher(domainId1, nodeId1, filename);
      else
        fileStreamAsync = await dedupBlobController.StitchContentLegacy(domainId1, nodeId1, filename);
      return fileStreamAsync;
    }

    [HttpPost]
    [ControllerMethodTraceFilter(5708120)]
    public async Task<IHttpActionResult> PostFileStreamAsync(string domainId)
    {
      MultiDomainDedupBlobController dedupBlobController = this;
      if (string.IsNullOrWhiteSpace(domainId))
        return (IHttpActionResult) dedupBlobController.BadRequest("domainId was null or empty.");
      using (Stream file = await dedupBlobController.Request.Content.ReadAsStreamAsync())
      {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        UploadResult uploadResult = await dedupBlobController.TfsRequestContext.GetService<IServerDeduplicationService>().ChunkAndUploadAsync(dedupBlobController.TfsRequestContext, DomainIdFactory.Create(domainId), file, cancellationTokenSource.Token);
        HttpResponseMessage response = new HttpResponseMessage();
        Uri baseUri = dedupBlobController.TfsRequestContext.RequestUri();
        response.Headers.Location = new Uri(baseUri, baseUri.AbsolutePath + "/" + uploadResult.rootId.ValueString + baseUri.Query);
        response.StatusCode = uploadResult.IsNewRoot ? HttpStatusCode.Created : HttpStatusCode.SeeOther;
        return (IHttpActionResult) dedupBlobController.ResponseMessage(response);
      }
    }

    private async Task<IHttpActionResult> StitchContentLegacy(
      IDomainId domainId,
      DedupIdentifier nodeId,
      string filename = null)
    {
      MultiDomainDedupBlobController dedupBlobController = this;
      IVssRegistryService service = dedupBlobController.TfsRequestContext.GetService<IVssRegistryService>();
      Stream streamAsync = await new DedupContentStreamFactory((IDedupContentProvider) new DedupContentDownloader(dedupBlobController.TfsRequestContext, MultiDomainDedupBlobController._chunkHttpClient), service.GetValue<int>(dedupBlobController.TfsRequestContext, (RegistryQuery) "/Configuration/BlobStore/StitchingBoundedCapacity", true, 20), service.GetValue<int>(dedupBlobController.TfsRequestContext, (RegistryQuery) "/Configuration/BlobStore/StitchingParallelism", true, 5)).GetStreamAsync(domainId, nodeId, dedupBlobController.TfsRequestContext.CancellationToken);
      dedupBlobController.TfsRequestContext.RequestTimer.SetTimeToFirstPageEnd();
      StreamContent streamContent = streamAsync != null ? new StreamContent(streamAsync) : throw new DedupNotFoundException("Dedup does not exist: " + nodeId?.ToString());
      streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      if (!string.IsNullOrWhiteSpace(filename))
        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
          FileName = filename
        };
      return (IHttpActionResult) dedupBlobController.ResponseMessage(new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) streamContent
      });
    }

    private async Task<IHttpActionResult> StitchViaContentStitcher(
      IDomainId domainId,
      DedupIdentifier nodeId,
      string filename = null)
    {
      MultiDomainDedupBlobController dedupBlobController = this;
      IContentStitcherService service = dedupBlobController.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<IContentStitcherService>();
      dedupBlobController.TfsRequestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", "GetFileStreamAsync", "Requesting redirect from ContentStitcher.");
      IDomainId domainId1 = domainId;
      DedupIdentifier nodeId1 = nodeId;
      IVssRequestContext tfsRequestContext = dedupBlobController.TfsRequestContext;
      string fileName = filename;
      HttpResponseMessage httpResponseMessage = await service.GetSignedUriAsync(domainId1, nodeId1, tfsRequestContext, fileName).ConfigureAwait(true);
      if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
      {
        dedupBlobController.TfsRequestContext.TraceAlways(5708101, TraceLevel.Info, "ContentStitcher", "GetFileStreamAsync", "Response from ContentStitcher was successful. Request will be redirected.");
        string location = httpResponseMessage.Content.ReadAsStringAsync().Result.Replace("\"", "");
        return (IHttpActionResult) dedupBlobController.Redirect(location);
      }
      dedupBlobController.TfsRequestContext.TraceAlways(5708101, TraceLevel.Error, "ContentStitcher", "GetFileStreamAsync", "Response from ContentStitcher was not received or was unsuccessful. Check logs for failure details. Reason: " + httpResponseMessage?.ReasonPhrase + ".");
      return (IHttpActionResult) dedupBlobController.InternalServerError();
    }
  }
}
