// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainBlobUrlHandler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class MultiDomainBlobUrlHandler
  {
    private IVssRequestContext requestContext;
    private HttpRequestMessage requestMessage;
    private IDomainId domainId;
    private TraceInfo traceInfo;
    private static readonly HashSet<string> BrokenRangeAfdClients = new HashSet<string>((IEnumerable<string>) new string[8]
    {
      "15.116",
      "15.117",
      "15.118",
      "15.119",
      "15.120",
      "15.121",
      "15.122",
      "15.123"
    });

    public MultiDomainBlobUrlHandler(
      IVssRequestContext requestContext,
      IDomainId domainId,
      HttpRequestMessage requestMessage,
      TraceInfo traceInfo)
    {
      this.requestContext = requestContext.AllowMultiDomainOperations(domainId) ? requestContext : throw new FeatureDisabledException("Multi-Domain");
      this.requestMessage = requestMessage;
      this.domainId = domainId;
      this.traceInfo = traceInfo;
    }

    internal static bool ClientHasBugsWithAzureFrontDoorDownloads(HttpRequestMessage request) => request.Headers.UserAgent != null && request.Headers.UserAgent.Any<ProductInfoHeaderValue>((Func<ProductInfoHeaderValue, bool>) (userAgent => userAgent?.Product?.Name == "VSServices" && MultiDomainBlobUrlHandler.BrokenRangeAfdClients.Contains(userAgent.Product?.Version?.Substring(0, 6))));

    internal static HttpResponseMessage AddEdgeCachingHeader(
      HttpResponseMessage response,
      EdgeType edgeType,
      bool supportedClient)
    {
      response.Headers.Add("X-TFS-EdgeCaching", edgeType.ToString());
      response.Headers.Add("X-TFS-EdgeCachingSupportedClient", supportedClient.ToString());
      return response;
    }

    public async Task<HttpResponseMessage> GetBlobUrlByIdentifierAsync(
      string blobId,
      string filename = null,
      string contentType = null,
      string expiryTime = null,
      bool allowEdge = false)
    {
      if (this.traceInfo.ShouldTraceMethodDetail)
        this.requestContext.TraceAlways(this.traceInfo.TracePointBase + 20, TraceLevel.Info, this.traceInfo.TraceArea, this.traceInfo.ControllerName, "BlobId={0},ContentType={1},ExpiryTime={2},AllowEdge={3}", (object) blobId, (object) contentType, (object) expiryTime, (object) allowEdge);
      IBlobStore service = this.requestContext.GetService<IBlobStore>();
      DateTimeOffset? expiryTime1 = new DateTimeOffset?();
      if (!string.IsNullOrEmpty(expiryTime))
        expiryTime1 = new DateTimeOffset?(DateTimeOffset.ParseExact(expiryTime, KeepUntilBlobReference.KeepUntilFormat, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
      bool supportedClient = !MultiDomainBlobUrlHandler.ClientHasBugsWithAzureFrontDoorDownloads(this.requestMessage);
      if (allowEdge && !supportedClient)
        allowEdge = false;
      BlobIdentifier blobIdentifier = BlobIdentifier.Deserialize(blobId);
      if (blobIdentifier.GetBlobDedupLevel() != BlobDedupLevel.FileLevel)
        throw new InvalidBlobTypeException(Resources.FileLevelBlobIdRequired());
      IVssRequestContext requestContext = this.requestContext;
      IDomainId domainId = this.domainId;
      BlobIdWithHeaders blobId1 = new BlobIdWithHeaders(blobIdentifier, allowEdge ? EdgeCache.Allowed : EdgeCache.NotAllowed, filename, contentType, expiryTime1);
      PreauthenticatedUri preauthenticatedUri = await service.GetDownloadUriAsync(requestContext, domainId, blobId1).ConfigureAwait(true);
      return MultiDomainBlobUrlHandler.AddEdgeCachingHeader(new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob>(new Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob(blobId)
        {
          Url = preauthenticatedUri.NotNullUri.AbsoluteUri
        }),
        StatusCode = HttpStatusCode.OK
      }, preauthenticatedUri.EdgeType, supportedClient);
    }
  }
}
