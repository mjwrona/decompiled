// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainDedupUrlsHandler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class MultiDomainDedupUrlsHandler
  {
    private IVssRequestContext requestContext;
    private HttpRequestMessage requestMessage;
    private IDomainId domainId;

    public MultiDomainDedupUrlsHandler(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      IDomainId domainId)
    {
      this.requestContext = requestContext;
      this.requestMessage = requestMessage;
      this.domainId = domainId;
    }

    public async Task<HttpResponseMessage> GetUrlsBatchAsync(bool allowEdge)
    {
      if (!this.requestContext.AllowMultiDomainOperations(this.domainId))
        throw new FeatureDisabledException("Multi-Domain");
      string json;
      using (this.requestContext.RequestTimer.CreateTimeToFirstPageExclusionBlock())
        json = await this.requestMessage.Content.ReadAsStringAsync();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      HashSet<DedupIdentifier> dedupIdentifierSet = new HashSet<DedupIdentifier>(((IEnumerable<string>) JsonSerializer.Deserialize<string[]>(json)).Select<string, DedupIdentifier>(MultiDomainDedupUrlsHandler.\u003C\u003EO.\u003C0\u003E__Create ?? (MultiDomainDedupUrlsHandler.\u003C\u003EO.\u003C0\u003E__Create = new Func<string, DedupIdentifier>(DedupIdentifier.Create))));
      IDedupStore service = this.requestContext.GetService<IDedupStore>();
      EdgeCache edgeCache = allowEdge ? EdgeCache.Allowed : EdgeCache.NotAllowed;
      IVssRequestContext requestContext = this.requestContext;
      IDomainId domainId = this.domainId;
      HashSet<DedupIdentifier> dedupIds = dedupIdentifierSet;
      int num = (int) edgeCache;
      Dictionary<string, string> dictionary = (await service.GetUris(requestContext, domainId, (ISet<DedupIdentifier>) dedupIds, (EdgeCache) num).ConfigureAwait(true)).ToDictionary<KeyValuePair<DedupIdentifier, PreauthenticatedUri>, string, string>((Func<KeyValuePair<DedupIdentifier, PreauthenticatedUri>, string>) (kvp => kvp.Key.ValueString), (Func<KeyValuePair<DedupIdentifier, PreauthenticatedUri>, string>) (kvp => kvp.Value.NotNullUri.AbsoluteUri));
      return new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<Dictionary<string, string>>(dictionary),
        StatusCode = HttpStatusCode.OK
      };
    }

    public async Task<HttpResponseMessage> GetDownloadInfoAsync(string dedupId, bool includeChunks)
    {
      if (!this.requestContext.AllowMultiDomainOperations(this.domainId))
        throw new FeatureDisabledException("Multi-Domain");
      DedupIdentifier dedupId1 = DedupIdentifier.Create(dedupId);
      DedupDownloadInfo dataContractObject = await this.requestContext.GetService<IDedupStore>().GetDownloadInfoAsync(this.requestContext, this.domainId, dedupId1, includeChunks).ConfigureAwait(true);
      return new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<DedupDownloadInfo>(dataContractObject),
        StatusCode = HttpStatusCode.OK
      };
    }
  }
}
