// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainBlobBatchHandler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class MultiDomainBlobBatchHandler
  {
    private IVssRequestContext requestContext;
    private HttpRequestMessage requestMessage;
    private IDomainId domainId;
    private TraceInfo traceInfo;
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "BlobsBatch"
    };

    public MultiDomainBlobBatchHandler(
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

    public async Task<HttpResponseMessage> GetDownloadUrisAsync(string expiryTime = null, bool allowEdge = false)
    {
      string requestString;
      using (this.requestContext.RequestTimer.CreateTimeToFirstPageExclusionBlock())
        requestString = await this.requestMessage.Content.ReadAsStringAsync().ConfigureAwait(true);
      IEnumerable<BlobIdentifier> blobIdentifiers;
      try
      {
        blobIdentifiers = JsonSerializer.Deserialize<BlobBatch>(requestString).Blobs.Select<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob, BlobIdentifier>((Func<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob, BlobIdentifier>) (id => BlobIdentifier.Deserialize(id.Id))).Distinct<BlobIdentifier>();
      }
      catch
      {
        throw new ArgumentException(Resources.IncorrectRequestBoby());
      }
      int total = !blobIdentifiers.Any<BlobIdentifier>((Func<BlobIdentifier, bool>) (b => b.GetBlobDedupLevel() != 0)) ? blobIdentifiers.Count<BlobIdentifier>() : throw new ArgumentException("Only file-level blob IDs can be passed into the file-level blob service route. " + string.Format("One of the invalid IDs: {0}", (object) blobIdentifiers.First<BlobIdentifier>((Func<BlobIdentifier, bool>) (b => b.GetBlobDedupLevel() != 0))));
      if (total == 0)
        this.requestContext.Trace(this.traceInfo.TracePointBase + 990, TraceLevel.Info, this.traceData.Area, this.traceData.Layer, "BlobBatchController.GetDownloadUrisAsync: the request deserialized into 0 blob ids. The request is '" + requestString + "'.");
      DateTimeOffset? expiryTime1 = new DateTimeOffset?();
      if (!string.IsNullOrEmpty(expiryTime))
        expiryTime1 = new DateTimeOffset?(DateTimeOffset.Parse(expiryTime).ToUniversalTime());
      bool supportedClient = !MultiDomainBlobUrlHandler.ClientHasBugsWithAzureFrontDoorDownloads(this.requestMessage);
      if (allowEdge && !supportedClient)
        allowEdge = false;
      if (this.traceInfo.ShouldTraceMethodDetail)
        this.requestContext.TraceAlways(this.traceInfo.TracePointBase + 20, TraceLevel.Info, this.traceInfo.TraceArea, this.traceInfo.ControllerName, "ExpiryTime={0},AllowEdge={1},BlobIdCount={2}", (object) expiryTime, (object) allowEdge, (object) total);
      IDictionary<BlobIdentifier, PreauthenticatedUri> source = await this.requestContext.GetService<IBlobStore>().GetDownloadUrisAsync(this.requestContext, this.domainId, blobIdentifiers, allowEdge ? EdgeCache.Allowed : EdgeCache.NotAllowed, expiryTime1).ConfigureAwait(true);
      EdgeType edgeType = source.Select<KeyValuePair<BlobIdentifier, PreauthenticatedUri>, EdgeType>((Func<KeyValuePair<BlobIdentifier, PreauthenticatedUri>, EdgeType>) (kvp => kvp.Value.EdgeType)).Distinct<EdgeType>().Count<EdgeType>() == 1 ? source.First<KeyValuePair<BlobIdentifier, PreauthenticatedUri>>().Value.EdgeType : EdgeType.Unknown;
      List<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob> list = source.Select<KeyValuePair<BlobIdentifier, PreauthenticatedUri>, Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob>((Func<KeyValuePair<BlobIdentifier, PreauthenticatedUri>, Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob>) (kvp => new Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob(kvp.Key)
      {
        Url = kvp.Value.NotNullUri.AbsoluteUri
      })).ToList<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob>();
      if (list.Count != total)
        this.requestContext.Trace(this.traceInfo.TracePointBase + 990, TraceLevel.Warning, this.traceData.Area, this.traceData.Layer, string.Format("BlobBatchController.GetDownloadUrisAsync: the service generated {0} blob URIs, but {1} is expected. The request is '{2}'.", (object) list.Count, (object) total, (object) requestString));
      BlobBatch dataContractObject = new BlobBatch()
      {
        Blobs = list
      };
      HttpResponseMessage downloadUrisAsync = MultiDomainBlobUrlHandler.AddEdgeCachingHeader(new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = JsonSerializer.SerializeToContent<BlobBatch>(dataContractObject)
      }, edgeType, supportedClient);
      requestString = (string) null;
      return downloadUrisAsync;
    }

    public async Task<BlobMappings> GetSASTokens() => new BlobMappings(await this.requestContext.GetService<IBlobStore>().GetSasUrisAsync(this.requestContext, this.domainId));
  }
}
