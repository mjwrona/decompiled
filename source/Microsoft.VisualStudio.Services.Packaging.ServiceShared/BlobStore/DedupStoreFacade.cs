// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore.DedupStoreFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore
{
  public class DedupStoreFacade : IPackagingDedupStore
  {
    private readonly IVssRequestContext requestContext;
    private static readonly HttpClient httpClient = new HttpClient();

    public DedupStoreFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public async Task TouchRootAsync(DedupIdentifier dedupId)
    {
      IDedupStore store = this.requestContext.GetService<IDedupStore>();
      DateTime dateTime = DateTime.UtcNow;
      dateTime = dateTime.Date;
      KeepUntilBlobReference keepUntilReference = new KeepUntilBlobReference(dateTime.AddDays(3.0));
      Stack<NodeDedupIdentifier> stack = new Stack<NodeDedupIdentifier>();
      stack.Push(dedupId.CastToNodeDedupIdentifier());
      while (stack.Count > 0)
      {
        NodeDedupIdentifier nodeId = stack.Pop();
        (await store.TryKeepUntilReferenceNodeAsync(this.requestContext, WellKnownDomainIds.OriginalDomainId, nodeId, keepUntilReference, (SummaryKeepUntilReceipt) null).ConfigureAwait(true)).Match((Action<DedupNodeNotFound>) (notFound =>
        {
          throw new DedupNotFoundException("Package is missing content.");
        }), (Action<DedupNodeChildrenNotEnoughKeepUntil>) (notEnoughKeepUntil =>
        {
          stack.Push(nodeId);
          foreach (DedupIdentifier dedupIdentifier in notEnoughKeepUntil.InsufficientKeepUntil)
            stack.Push(dedupIdentifier.CastToNodeDedupIdentifier());
        }), (Action<DedupNodeUpdated>) (updated => { }));
      }
      store = (IDedupStore) null;
    }

    public Task PutRootAsync(DedupIdentifier dedupId, IdBlobReference rootRef) => this.requestContext.GetService<IDedupStore>().PutRootAsync(this.requestContext, WellKnownDomainIds.OriginalDomainId, dedupId, rootRef);

    public Task DeleteRootAsync(DedupIdentifier dedupId, IdBlobReference rootRef) => this.requestContext.GetService<IDedupStore>().DeleteRootAsync(this.requestContext, WellKnownDomainIds.OriginalDomainId, dedupId, rootRef);

    public async Task<long> GetNodeAsync(NodeDedupIdentifier parsedDedupId) => (long) DedupNode.Deserialize((await this.requestContext.GetService<IDedupStore>().GetNodeAsync(this.requestContext, WellKnownDomainIds.DefaultDomainId, parsedDedupId) ?? throw new DedupNotFoundException("Dedup does not exist: " + parsedDedupId.ValueString)).Uncompressed.CreateCopy<byte>()).TransitiveContentBytes;

    public async Task<(Manifest Manifest, ulong Size)> DownloadManifestAsync(DedupIdentifier dedupId)
    {
      DedupDownloadInfo downloadInfoAsync = await this.requestContext.GetService<IDedupStore>().GetDownloadInfoAsync(this.requestContext, WellKnownDomainIds.OriginalDomainId, dedupId, true);
      if (downloadInfoAsync == null)
        throw new FileNotFoundException();
      IEnumerable<Uri> uris1;
      if (downloadInfoAsync.Chunks != null)
        uris1 = (IEnumerable<Uri>) ((IEnumerable<ChunkDedupDownloadInfo>) downloadInfoAsync.Chunks).Select<ChunkDedupDownloadInfo, Uri>((Func<ChunkDedupDownloadInfo, Uri>) (c => c.Url)).ToArray<Uri>();
      else
        uris1 = (IEnumerable<Uri>) new Uri[1]
        {
          downloadInfoAsync.Url
        };
      IEnumerable<Uri> uris2 = uris1;
      ulong size = (ulong) downloadInfoAsync.Size;
      (Manifest, ulong) valueTuple;
      using (Stream transitiveContent = (Stream) Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.BlobStitcher.GetTransitiveContent(downloadInfoAsync.Size, uris2, DedupStoreFacade.httpClient, (IAppTraceSource) NoopAppTraceSource.Instance, this.requestContext.CancellationToken))
        valueTuple = (JsonSerializer.Deserialize<Manifest>(transitiveContent), size);
      return valueTuple;
    }

    public bool IsContentValid(
      HashSet<DedupNode> proofNodes,
      DedupIdentifier rootId,
      IDictionary<DedupIdentifier, ulong> fileEntries)
    {
      HashSet<DedupNode> proofNodes1 = proofNodes;
      HashSet<NodeDedupIdentifier> validatedRoots = new HashSet<NodeDedupIdentifier>();
      validatedRoots.Add(rootId.CastToNodeDedupIdentifier());
      IDictionary<DedupIdentifier, ulong> idsToValidate = fileEntries;
      return !ProofHelper.DetermineUnvalidatedIds((ISet<DedupNode>) proofNodes1, (ISet<NodeDedupIdentifier>) validatedRoots, idsToValidate).Any<DedupIdentifier>();
    }
  }
}
