// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainNodeHandler
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
  public class MultiDomainNodeHandler
  {
    private IVssRequestContext requestContext;
    private HttpRequestMessage requestMessage;
    private IDomainId domainId;

    public MultiDomainNodeHandler(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      IDomainId domainId)
    {
      this.requestContext = requestContext;
      this.requestMessage = requestMessage;
      this.domainId = domainId;
    }

    public async Task<HttpResponseMessage> GetNodeAsync(
      string dedupId,
      bool allowEdge = false,
      bool redirect = true)
    {
      IDedupStore dedupStore1 = this.requestContext.AllowMultiDomainOperations(this.domainId) ? this.requestContext.GetService<IDedupStore>() : throw new FeatureDisabledException("Multi-Domain");
      NodeDedupIdentifier nodeId = NodeDedupIdentifier.Parse(dedupId);
      if (redirect)
      {
        EdgeCache edgeCache = allowEdge ? EdgeCache.Allowed : EdgeCache.NotAllowed;
        IDedupStore dedupStore2 = dedupStore1;
        IVssRequestContext requestContext = this.requestContext;
        IDomainId domainId = this.domainId;
        HashSet<DedupIdentifier> dedupIds = new HashSet<DedupIdentifier>();
        dedupIds.Add((DedupIdentifier) nodeId);
        int num = (int) edgeCache;
        IDictionary<DedupIdentifier, PreauthenticatedUri> source = await dedupStore2.GetUris(requestContext, domainId, (ISet<DedupIdentifier>) dedupIds, (EdgeCache) num).ConfigureAwait(true);
        return new HttpResponseMessage()
        {
          StatusCode = HttpStatusCode.SeeOther,
          Headers = {
            Location = source.Single<KeyValuePair<DedupIdentifier, PreauthenticatedUri>>().Value.NotNullUri
          }
        };
      }
      using (DedupCompressedBuffer buffer = await dedupStore1.GetNodeAsync(this.requestContext, this.domainId, nodeId).ConfigureAwait(true))
        return buffer != null ? new HttpResponseMessage()
        {
          Content = (HttpContent) CompressionHelpers.CreatePossiblyCompressedResponseContent(this.requestMessage, buffer),
          StatusCode = HttpStatusCode.OK
        } : throw DedupNotFoundException.Create(dedupId);
    }

    public async Task<HttpResponseMessage> PutNodeAsync(string dedupId, string keepUntil)
    {
      IDedupStore service = this.requestContext.AllowMultiDomainOperations(this.domainId) ? this.requestContext.GetService<IDedupStore>() : throw new FeatureDisabledException("Multi-Domain");
      NodeDedupIdentifier parsedDedupId = NodeDedupIdentifier.Parse(dedupId);
      SummaryKeepUntilReceipt receipts = this.CreateReceiptsFromHeaders();
      PutNodeResponse putNodeResponse;
      using (DedupCompressedBuffer buffer = await CompressionHelpers.GetPossiblyCompressedBufferAsync(this.requestMessage.Content))
        putNodeResponse = await service.PutNodeAndKeepUntilReferenceAsync(this.requestContext, this.domainId, parsedDedupId, buffer, new KeepUntilBlobReference(keepUntil), receipts).ConfigureAwait(true);
      HttpResponseMessage httpResponseMessage = putNodeResponse.Match<HttpResponseMessage>((Func<DedupNodeChildrenNeedAction, HttpResponseMessage>) (needAction => new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<DedupNodeChildrenNeedAction>(needAction),
        StatusCode = HttpStatusCode.Conflict
      }), (Func<DedupNodeUpdated, HttpResponseMessage>) (added => new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<Dictionary<DedupIdentifier, KeepUntilReceipt>>(added.Receipts),
        StatusCode = HttpStatusCode.OK
      }));
      service = (IDedupStore) null;
      parsedDedupId = (NodeDedupIdentifier) null;
      receipts = (SummaryKeepUntilReceipt) null;
      return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> TryReferenceAsync(string dedupId, string keepUntil)
    {
      IDedupStore dedupStore = this.requestContext.AllowMultiDomainOperations(this.domainId) ? this.requestContext.GetService<IDedupStore>() : throw new FeatureDisabledException("Multi-Domain");
      NodeDedupIdentifier nodeDedupIdentifier = NodeDedupIdentifier.Parse(dedupId);
      SummaryKeepUntilReceipt receiptsFromHeaders = this.CreateReceiptsFromHeaders();
      IVssRequestContext requestContext = this.requestContext;
      IDomainId domainId = this.domainId;
      NodeDedupIdentifier nodeId = nodeDedupIdentifier;
      KeepUntilBlobReference keepUntil1 = new KeepUntilBlobReference(keepUntil);
      SummaryKeepUntilReceipt receipts = receiptsFromHeaders;
      return (await dedupStore.TryKeepUntilReferenceNodeAsync(requestContext, domainId, nodeId, keepUntil1, receipts).ConfigureAwait(true)).Match<HttpResponseMessage>((Func<DedupNodeNotFound, HttpResponseMessage>) (notFound =>
      {
        throw DedupNotFoundException.Create(dedupId);
      }), (Func<DedupNodeChildrenNotEnoughKeepUntil, HttpResponseMessage>) (needAction => new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<DedupNodeChildrenNotEnoughKeepUntil>(needAction),
        StatusCode = HttpStatusCode.Conflict
      }), (Func<DedupNodeUpdated, HttpResponseMessage>) (added => new HttpResponseMessage()
      {
        Content = JsonSerializer.SerializeToContent<Dictionary<DedupIdentifier, KeepUntilReceipt>>(added.Receipts),
        StatusCode = HttpStatusCode.OK
      }));
    }

    private SummaryKeepUntilReceipt CreateReceiptsFromHeaders()
    {
      IEnumerable<string> values1;
      IEnumerable<string> values2;
      if (!this.requestMessage.Headers.TryGetValues("X-MS-KeepUntils", out values1) || !this.requestMessage.Headers.TryGetValues("X-MS-Signature", out values2))
        return (SummaryKeepUntilReceipt) null;
      List<KeepUntilBlobReference?> nullableList = new List<KeepUntilBlobReference?>();
      string str = values1.Single<string>();
      char[] chArray = new char[1]{ ',' };
      foreach (string dateString in str.Split(chArray))
      {
        if (string.IsNullOrEmpty(dateString))
        {
          nullableList.Add(new KeepUntilBlobReference?());
        }
        else
        {
          try
          {
            nullableList.Add(new KeepUntilBlobReference?(new KeepUntilBlobReference(dateString)));
          }
          catch (ArgumentException ex)
          {
            throw new ArgumentException("Bad receipt header", (Exception) ex);
          }
        }
      }
      string s = values2.Single<string>();
      return new SummaryKeepUntilReceipt(nullableList.ToArray(), Convert.FromBase64String(s));
    }
  }
}
