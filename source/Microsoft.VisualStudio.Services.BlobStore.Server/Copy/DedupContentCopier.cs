// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Copy.DedupContentCopier
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Copy
{
  public sealed class DedupContentCopier
  {
    private const int MaxParallelism = 192;
    private const int MaxHttpRetries = 10;
    private readonly TimeSpan defaultKeepUntilDuration = TimeSpan.FromDays(2.0);
    private readonly SemaphoreSlim copyParallelismSemaphore = new SemaphoreSlim(192, 192);
    private readonly ConcurrentDictionary<DedupIdentifier, KeepUntilReceipt> sessionReceipts = new ConcurrentDictionary<DedupIdentifier, KeepUntilReceipt>();
    private readonly Guid copyFromHostId;
    private readonly IDomainId copyFromDomainId;
    private readonly IDomainId copyToDomainId;
    private IDedupStoreHttpClient downloader;
    private IDedupStoreHttpClient uploader;
    private IDedupStore dedupStoreService;
    private readonly KeepUntilBlobReference keepUntil;
    private bool isCopyJAEnabled;
    private IAppTraceSource traceSource = (IAppTraceSource) NoopAppTraceSource.Instance;
    public const string TraceLayer = "Copy";
    private readonly TraceData TraceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Copy"
    };

    private DedupContentCopier(
      Guid copyFromHostId,
      IDomainId copyFromDomainId,
      IDomainId copyToDomainId)
    {
      this.copyFromHostId = copyFromHostId;
      this.copyFromDomainId = copyFromDomainId;
      this.copyToDomainId = copyToDomainId;
      this.keepUntil = new KeepUntilBlobReference(DateTime.UtcNow.Add(this.defaultKeepUntilDuration));
    }

    internal DedupContentCopier(
      Guid copyFromHostId,
      IDomainId copyFromDomainId,
      IDomainId copyToDomainId,
      IDedupStoreHttpClient downloader,
      IDedupStoreHttpClient uploader)
      : this(copyFromHostId, copyFromDomainId, copyToDomainId)
    {
      this.downloader = downloader;
      this.uploader = uploader;
    }

    internal DedupContentCopier(
      Guid copyFromHostId,
      IDomainId copyFromDomainId,
      IDomainId copyToDomainId,
      IDedupStoreHttpClient downloader,
      IDedupStore dedupStore)
      : this(copyFromHostId, copyFromDomainId, copyToDomainId)
    {
      this.downloader = downloader;
      this.dedupStoreService = dedupStore;
      this.isCopyJAEnabled = true;
    }

    private void InitializeClients(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Uri hostUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, this.copyFromHostId, Microsoft.VisualStudio.Services.Content.Common.ServiceInstanceTypes.BlobStore);
      if (hostUri == (Uri) null)
        throw new ServiceHostDoesNotExistException(this.copyFromHostId);
      this.downloader = (IDedupStoreHttpClient) new DomainHttpClientWrapper(this.copyFromDomainId, (IDomainDedupStoreHttpClient) ((ICreateClient) requestContext.ClientProvider).CreateClient<DomainDedupStoreHttpClient>(requestContext, hostUri, "Empty", (ApiResourceLocationCollection) null));
      this.isCopyJAEnabled = requestContext.IsFeatureEnabled("BlobStore.Features.EnableCopyViaJobAgent");
      this.dedupStoreService = requestContext.GetService<IDedupStore>();
      this.uploader = (IDedupStoreHttpClient) new DomainHttpClientWrapper(this.copyToDomainId, (IDomainDedupStoreHttpClient) requestContext.GetClient<DomainDedupStoreHttpClient>());
    }

    public static DedupContentCopier Create(
      IVssRequestContext requestContext,
      Guid copyFromHostId,
      IDomainId copyFromDomainId,
      IDomainId copyToDomainId)
    {
      DedupContentCopier dedupContentCopier = new DedupContentCopier(copyFromHostId, copyFromDomainId, copyToDomainId);
      dedupContentCopier.InitializeClients(requestContext);
      return dedupContentCopier;
    }

    public async Task<Dictionary<DedupIdentifier, KeepUntilReceipt>> CopyAsync(
      IVssRequestContext requestContext,
      ISet<DedupIdentifier> dedupIds,
      CancellationToken cancellationToken)
    {
      Dictionary<DedupIdentifier, KeepUntilReceipt> receipts = new Dictionary<DedupIdentifier, KeepUntilReceipt>();
      foreach (DedupIdentifier dedupId1 in (IEnumerable<DedupIdentifier>) dedupIds)
      {
        DedupIdentifier dedupId = dedupId1;
        using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.TraceData, 5701920, nameof (CopyAsync)))
        {
          using (requestContext.AcquireExemptionLock())
          {
            if (ChunkerHelper.IsChunk(dedupId.AlgorithmId))
            {
              ChunkDedupIdentifier chunkDedupIdentifier = (ChunkDedupIdentifier) dedupId;
              receipts.Add(dedupId, (await requestContext.PumpOrInlineFromAsync<(DedupIdentifier, KeepUntilReceipt)>((Func<VssRequestPump.Processor, Task<(DedupIdentifier, KeepUntilReceipt)>>) (async processor => await this.CopyChunkAsync(processor, chunkDedupIdentifier, tracer, cancellationToken)), true)).Item2);
            }
            else
              receipts.Add(dedupId, (await requestContext.PumpOrInlineFromAsync<(DedupIdentifier, KeepUntilReceipt)>((Func<VssRequestPump.Processor, Task<(DedupIdentifier, KeepUntilReceipt)>>) (async processor => await this.CopyNodeRecursiveAsync(processor, (NodeDedupIdentifier) dedupId, tracer, cancellationToken)), true)).Item2);
          }
        }
      }
      Dictionary<DedupIdentifier, KeepUntilReceipt> dictionary = receipts;
      receipts = (Dictionary<DedupIdentifier, KeepUntilReceipt>) null;
      return dictionary;
    }

    private async Task<(DedupIdentifier DedupId, KeepUntilReceipt Receipt)> CopyNodeRecursiveAsync(
      VssRequestPump.Processor processor,
      NodeDedupIdentifier dedupId,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      CancellationToken cancellationToken)
    {
      DedupCompressedBuffer nodeContent = await this.GetNodeAsync(dedupId.CastToNodeDedupIdentifier(), cancellationToken).ConfigureAwait(false);
      Func<DedupNodeUpdated, KeepUntilReceipt> func;
      (NodeDedupIdentifier, KeepUntilReceipt) valueTuple = await (await this.PutNodeAndKeepUntilReferenceAsync(processor, dedupId, nodeContent, (SummaryKeepUntilReceipt) null, cancellationToken)).Match<Task<(NodeDedupIdentifier, KeepUntilReceipt)>>((Func<DedupNodeChildrenNeedAction, Task<(NodeDedupIdentifier, KeepUntilReceipt)>>) (async needAction =>
      {
        HashSet<DedupIdentifier> hashSet = ((IEnumerable<DedupIdentifier>) needAction.Missing).Union<DedupIdentifier>((IEnumerable<DedupIdentifier>) needAction.InsufficientKeepUntil).ToHashSet<DedupIdentifier>();
        Dictionary<DedupIdentifier, KeepUntilReceipt> dictionary = ((IEnumerable<(DedupIdentifier, KeepUntilReceipt)>) await Task.WhenAll<(DedupIdentifier, KeepUntilReceipt)>((IEnumerable<Task<(DedupIdentifier, KeepUntilReceipt)>>) this.CreateChildrenUploadTasks(processor, hashSet, tracer, cancellationToken)).ConfigureAwait(false)).ToDictionary<(DedupIdentifier, KeepUntilReceipt), DedupIdentifier, KeepUntilReceipt>((Func<(DedupIdentifier, KeepUntilReceipt), DedupIdentifier>) (t => t.Item1), (Func<(DedupIdentifier, KeepUntilReceipt), KeepUntilReceipt>) (t => t.Item2));
        SummaryKeepUntilReceipt summaryKeepUntilReceipt = this.CollectChildrenReceipts(DedupNode.Deserialize(nodeContent.Uncompressed).ChildNodes, dictionary, needAction.Receipts);
        KeepUntilReceipt keepUntilReceipt = (await this.PutNodeAndKeepUntilReferenceAsync(processor, dedupId, nodeContent, summaryKeepUntilReceipt, cancellationToken)).Match<KeepUntilReceipt>((Func<DedupNodeChildrenNeedAction, KeepUntilReceipt>) (actionNeeded =>
        {
          throw new InvalidOperationException("Unexpected 2nd put node to require additional action.");
        }), func ?? (func = (Func<DedupNodeUpdated, KeepUntilReceipt>) (updated => updated.Receipts[(DedupIdentifier) dedupId])));
        this.sessionReceipts.TryAdd((DedupIdentifier) dedupId, keepUntilReceipt);
        return (dedupId, keepUntilReceipt);
      }), (Func<DedupNodeUpdated, Task<(NodeDedupIdentifier, KeepUntilReceipt)>>) (updated =>
      {
        KeepUntilReceipt receipt = updated.Receipts[(DedupIdentifier) dedupId];
        this.sessionReceipts.TryAdd((DedupIdentifier) dedupId, receipt);
        return Task.FromResult<(NodeDedupIdentifier, KeepUntilReceipt)>((dedupId, receipt));
      }));
      return ((DedupIdentifier) valueTuple.Item1, valueTuple.Item2);
    }

    private SummaryKeepUntilReceipt CollectChildrenReceipts(
      IReadOnlyList<DedupNode> childNodes,
      Dictionary<DedupIdentifier, KeepUntilReceipt> newlyUploadedReceipts,
      Dictionary<DedupIdentifier, KeepUntilReceipt> existingReceipts)
    {
      bool flag = true;
      KeepUntilReceipt[] keepUntilReceiptArray = new KeepUntilReceipt[childNodes.Count];
      for (int index = 0; index < childNodes.Count; ++index)
      {
        DedupIdentifier key = DedupIdentifier.Create(childNodes[index]);
        if (newlyUploadedReceipts.ContainsKey(key))
          keepUntilReceiptArray[index] = newlyUploadedReceipts[key];
        else if (existingReceipts.ContainsKey(key))
        {
          keepUntilReceiptArray[index] = existingReceipts[key];
        }
        else
        {
          KeepUntilReceipt keepUntilReceipt;
          if (this.sessionReceipts.TryGetValue(key, out keepUntilReceipt))
          {
            keepUntilReceiptArray[index] = keepUntilReceipt;
          }
          else
          {
            keepUntilReceiptArray[index] = (KeepUntilReceipt) null;
            flag = false;
          }
        }
      }
      return !flag ? (SummaryKeepUntilReceipt) null : new SummaryKeepUntilReceipt(keepUntilReceiptArray);
    }

    private List<Task<(DedupIdentifier, KeepUntilReceipt)>> CreateChildrenUploadTasks(
      VssRequestPump.Processor processor,
      HashSet<DedupIdentifier> nodesToCopy,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      CancellationToken cancellationToken)
    {
      List<Task<(DedupIdentifier, KeepUntilReceipt)>> childrenUploadTasks = new List<Task<(DedupIdentifier, KeepUntilReceipt)>>();
      foreach (DedupIdentifier dedupIdentifier in nodesToCopy)
      {
        if (!this.sessionReceipts.ContainsKey(dedupIdentifier))
        {
          if (ChunkerHelper.IsChunk(dedupIdentifier.AlgorithmId))
            childrenUploadTasks.Add(this.CopyChunkAsync(processor, (ChunkDedupIdentifier) dedupIdentifier, tracer, cancellationToken));
          else
            childrenUploadTasks.Add(this.CopyNodeRecursiveAsync(processor, (NodeDedupIdentifier) dedupIdentifier, tracer, cancellationToken));
        }
      }
      return childrenUploadTasks;
    }

    private async Task<(DedupIdentifier DedupId, KeepUntilReceipt Receipt)> CopyChunkAsync(
      VssRequestPump.Processor processor,
      ChunkDedupIdentifier dedupId,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      CancellationToken cancellationToken)
    {
      DedupCompressedBuffer chunk = await this.GetChunkAsync(dedupId.CastToChunkDedupIdentifier(), cancellationToken).ConfigureAwait(false);
      KeepUntilReceipt keepUntilReceipt = await this.PutChunkAndKeepUntilReferenceAsync(processor, dedupId, chunk, cancellationToken);
      this.sessionReceipts.TryAdd((DedupIdentifier) dedupId, keepUntilReceipt);
      return ((DedupIdentifier) dedupId, keepUntilReceipt);
    }

    private async Task<KeepUntilReceipt> PutChunkAndKeepUntilReferenceAsync(
      VssRequestPump.Processor processor,
      ChunkDedupIdentifier dedupId,
      DedupCompressedBuffer chunk,
      CancellationToken cancellationToken)
    {
      KeepUntilReceipt keepUntilReceipt1;
      using (await SemaphoreSlimToken.Wait(this.copyParallelismSemaphore).ConfigureAwait(false))
      {
        KeepUntilReceipt keepUntilReceipt2;
        if (this.isCopyJAEnabled)
          keepUntilReceipt2 = await processor.ExecuteAsyncWorkAsync<KeepUntilReceipt>((Func<IVssRequestContext, Task<KeepUntilReceipt>>) (requestContext => AsyncHttpRetryHelper<KeepUntilReceipt>.InvokeAsync((Func<Task<KeepUntilReceipt>>) (() => this.dedupStoreService.PutChunkAndKeepUntilReferenceAsync(requestContext, this.copyToDomainId, dedupId, chunk, this.keepUntil)), 10, this.traceSource, (Func<Exception, bool>) null, cancellationToken, true, "PutNodeAndKeepUntilReferenceAsync", new TimeSpan?(TimeSpan.FromSeconds(10.0)), new TimeSpan?(TimeSpan.FromSeconds(180.0)))));
        else
          keepUntilReceipt2 = await AsyncHttpRetryHelper<KeepUntilReceipt>.InvokeAsync((Func<Task<KeepUntilReceipt>>) (() => this.uploader.PutChunkAndKeepUntilReferenceAsync(dedupId, chunk, this.keepUntil, cancellationToken)), 10, this.traceSource, (Func<Exception, bool>) null, cancellationToken, false, nameof (PutChunkAndKeepUntilReferenceAsync), new TimeSpan?(TimeSpan.FromSeconds(10.0)), new TimeSpan?(TimeSpan.FromSeconds(180.0)));
        keepUntilReceipt1 = keepUntilReceipt2;
      }
      return keepUntilReceipt1;
    }

    private async Task<PutNodeResponse> PutNodeAndKeepUntilReferenceAsync(
      VssRequestPump.Processor processor,
      NodeDedupIdentifier dedupId,
      DedupCompressedBuffer nodeContent,
      SummaryKeepUntilReceipt summaryKeepUntilReceipt,
      CancellationToken cancellationToken)
    {
      PutNodeResponse putNodeResponse1;
      using (await SemaphoreSlimToken.Wait(this.copyParallelismSemaphore).ConfigureAwait(false))
      {
        PutNodeResponse putNodeResponse2;
        if (this.isCopyJAEnabled)
          putNodeResponse2 = await processor.ExecuteAsyncWorkAsync<PutNodeResponse>((Func<IVssRequestContext, Task<PutNodeResponse>>) (requestContext => AsyncHttpRetryHelper<PutNodeResponse>.InvokeAsync((Func<Task<PutNodeResponse>>) (() => this.dedupStoreService.PutNodeAndKeepUntilReferenceAsync(requestContext, this.copyToDomainId, dedupId, nodeContent, this.keepUntil, summaryKeepUntilReceipt)), 10, this.traceSource, (Func<Exception, bool>) null, cancellationToken, true, nameof (PutNodeAndKeepUntilReferenceAsync), new TimeSpan?(TimeSpan.FromSeconds(10.0)), new TimeSpan?(TimeSpan.FromSeconds(180.0)))));
        else
          putNodeResponse2 = await AsyncHttpRetryHelper<PutNodeResponse>.InvokeAsync((Func<Task<PutNodeResponse>>) (() => this.uploader.PutNodeAndKeepUntilReferenceAsync(dedupId, nodeContent, this.keepUntil, summaryKeepUntilReceipt, cancellationToken)), 10, this.traceSource, (Func<Exception, bool>) null, cancellationToken, false, nameof (PutNodeAndKeepUntilReferenceAsync), new TimeSpan?(TimeSpan.FromSeconds(10.0)), new TimeSpan?(TimeSpan.FromSeconds(180.0)));
        putNodeResponse1 = putNodeResponse2;
      }
      return putNodeResponse1;
    }

    private async Task<DedupCompressedBuffer> GetChunkAsync(
      ChunkDedupIdentifier dedupId,
      CancellationToken cancellationToken)
    {
      MaybeCached<DedupCompressedBuffer> maybeCached = await AsyncHttpRetryHelper<MaybeCached<DedupCompressedBuffer>>.InvokeAsync((Func<Task<MaybeCached<DedupCompressedBuffer>>>) (() => this.downloader.GetChunkAsync(dedupId, true, cancellationToken)), 10, this.traceSource, (Func<Exception, bool>) null, cancellationToken, false, nameof (GetChunkAsync), new TimeSpan?(TimeSpan.FromSeconds(10.0)), new TimeSpan?(TimeSpan.FromSeconds(180.0)));
      return maybeCached.Value != null ? maybeCached.Value : throw new DedupNotFoundException("Dedup chunk with id: " + dedupId.ValueString + " does not exist.");
    }

    private async Task<DedupCompressedBuffer> GetNodeAsync(
      NodeDedupIdentifier dedupId,
      CancellationToken cancellationToken)
    {
      MaybeCached<DedupCompressedBuffer> maybeCached = await AsyncHttpRetryHelper<MaybeCached<DedupCompressedBuffer>>.InvokeAsync((Func<Task<MaybeCached<DedupCompressedBuffer>>>) (() => this.downloader.GetNodeAsync(dedupId, true, cancellationToken)), 10, this.traceSource, (Func<Exception, bool>) null, cancellationToken, false, nameof (GetNodeAsync), new TimeSpan?(TimeSpan.FromSeconds(10.0)), new TimeSpan?(TimeSpan.FromSeconds(180.0)));
      return maybeCached.Value != null ? maybeCached.Value : throw new DedupNotFoundException("Dedup node with id: " + dedupId.ValueString + " does not exist.");
    }
  }
}
