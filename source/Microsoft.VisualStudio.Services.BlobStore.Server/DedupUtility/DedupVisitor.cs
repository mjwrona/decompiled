// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility.DedupVisitor
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Storage;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility
{
  public class DedupVisitor
  {
    private const string c_fixedHeader = "(Dedup Visiting)";
    private const string c_perfHeader = "(perf)";
    private readonly string m_header;
    protected readonly VssRequestPump.SecuredDomainProcessor processor;
    private readonly IDedupInfoRetriever m_retriever;
    private readonly bool m_reentrant;
    protected bool m_diag;
    private readonly Action<TraceLevel, string> log;
    protected bool m_skipMissing;
    private bool m_topDownCollectPathData;
    private long m_visitCount;
    private DedupTraversalConfig config;
    private IDedupProcessor m_procBlock;
    private ActionBlock<DedupLink> m_queueBlock;
    private Tracker m_treeDicoveryTracker;
    private Stack<DedupInfoLink> m_dedupInfoLinkStack;
    private IBottomUpConcurrencyEvaluator m_cevaluator;
    private const int MaxRetrievalRetries = 3;
    private const int PauseBetweenRetrievalRetryInMs = 500;

    protected IDomainId domainId => this.processor.SecuredDomainRequest.DomainId;

    public bool IsAborting => this.m_procBlock != null && this.m_procBlock.IsAborting;

    public ulong DedupContentSize { get; private set; }

    public void Abort() => this.m_procBlock?.Abort();

    public DedupVisitor(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupInfoRetriever retriever,
      DedupTraversalConfig config,
      IBottomUpConcurrencyEvaluator cevaluator,
      bool reentrant,
      string header,
      Action<TraceLevel, string> log)
    {
      this.processor = processor;
      this.m_retriever = retriever;
      this.m_reentrant = reentrant;
      this.m_cevaluator = cevaluator;
      this.log = log;
      this.m_header = string.IsNullOrEmpty(header) ? "(Dedup Visiting)" : "(Dedup Visiting) " + header;
      this.Initialize(config);
    }

    private void Initialize(DedupTraversalConfig config)
    {
      this.config = config;
      this.m_diag = config.EnableDiagnostics;
      this.m_skipMissing = config.IgnoreMissingNode;
      this.m_topDownCollectPathData = !config.TopDownSkipCollectingPathData;
      if (config.PerformBottomUpTraversal == BottomUpTraversalOption.Disabled)
        return;
      this.m_dedupInfoLinkStack = new Stack<DedupInfoLink>();
    }

    private IDedupProcessor GetDedupProcessorOnce(
      VssRequestPump.Processor processor,
      DedupTraversalConfig config,
      Func<DedupLink, Task> processAsync,
      bool processInline)
    {
      if (this.m_procBlock == null)
      {
        lock (this)
        {
          if (this.m_procBlock == null)
            return this.m_procBlock = (IDedupProcessor) new DedupProcessor(processor, (DedupProcessingConfig) config, processAsync, this.log, processInline);
        }
      }
      if (this.m_reentrant)
        return this.m_procBlock;
      throw new InvalidOperationException("DedupDispatcher is not reentrant. VisitNodesAsync() can only be called once.");
    }

    protected async Task<long> VisitNodesAsync(
      DedupIdentifier dedupId,
      Func<DedupLink, Task> processAsync)
    {
      IDedupProcessor block = (IDedupProcessor) null;
      bool processInline = false;
      try
      {
        if (this.m_diag)
          Interlocked.Increment(ref this.m_visitCount);
        BottomUpTraversalOption bottomUpTraversal = this.config.PerformBottomUpTraversal;
        if (bottomUpTraversal != BottomUpTraversalOption.Disabled)
        {
          block = this.GetDedupProcessorOnce((VssRequestPump.Processor) this.processor, this.config, processAsync, processInline = true);
          await this.VisitDedupsBottomUpAsync(this.processor, block, dedupId, processAsync, bottomUpTraversal == BottomUpTraversalOption.ParallelizedWithoutPathInfo, bottomUpTraversal == BottomUpTraversalOption.SerializedWithPathInfo).ConfigureAwait(false);
        }
        else if (this.config.DispatchingParallelism == 1)
        {
          block = this.GetDedupProcessorOnce((VssRequestPump.Processor) this.processor, this.config, processAsync, processInline = false);
          ConfiguredTaskAwaitable configuredTaskAwaitable = block.PostAsync(DedupLink.Create((DedupIdentifier) null, dedupId)).ConfigureAwait(false);
          await configuredTaskAwaitable;
          configuredTaskAwaitable = this.VisitDedupsTopDownAsync(this.processor, block, dedupId).ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
        else
        {
          block = this.GetDedupProcessorOnce((VssRequestPump.Processor) this.processor, this.config, processAsync, processInline = false);
          await this.VisitDedupsInParallelAsync(this.processor, block, dedupId).ConfigureAwait(false);
        }
        this.config.Profiler.Increment(ProfilingCategory.Man_RootCount);
      }
      finally
      {
        if (block != null && !processInline)
          await block.CompleteAsync().ConfigureAwait(false);
      }
      long visitCount = this.m_visitCount;
      block = (IDedupProcessor) null;
      return visitCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<IDedupInfo> GetDedupInfoAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      IDedupInfo info = (IDedupInfo) null;
      int retries = 0;
      while (retries < 3)
      {
        info = await this.m_retriever.GetDedupInfoAsync(processor, dedupId).ConfigureAwait(false);
        if (info.Status == HealthStatus.Undetermined)
        {
          ++retries;
          await Task.Delay(500 * retries);
        }
        else
          break;
      }
      IDedupInfo dedupInfoAsync = info;
      info = (IDedupInfo) null;
      return dedupInfoAsync;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<DedupNode?> GetDedupNodeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      NodeDedupIdentifier nodeId)
    {
      DedupNode? nullable = new DedupNode?();
      int retries = 0;
      DedupNode? dedupNodeAsync;
      while (true)
      {
        int num;
        do
        {
          try
          {
            dedupNodeAsync = await this.m_retriever.GetDedupNodeAsync(processor, nodeId).ConfigureAwait(false);
            goto label_12;
          }
          catch (StorageException ex)
          {
            num = 1;
          }
        }
        while (num != 1);
        ++retries;
        if (retries >= 3)
        {
          if ((object) ex is Exception source)
            ExceptionDispatchInfo.Capture(source).Throw();
          else
            break;
        }
        await Task.Delay(500 * retries);
      }
      throw (object) ex;
label_12:
      return dedupNodeAsync;
    }

    private async Task VisitDedupsTopDownAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupProcessor block,
      DedupIdentifier dedupId)
    {
      try
      {
        processor.CancellationToken.ThrowIfCancellationRequested();
        if (!ChunkerHelper.IsNode(dedupId.AlgorithmId))
          return;
        DedupNode? nullable = await this.GetDedupNodeAsync(processor, dedupId.CastToNodeDedupIdentifier()).ConfigureAwait(false);
        if (!nullable.HasValue)
        {
          if (!this.m_skipMissing)
          {
            string message = this.m_header + " Cannot retrieve dedup node for " + dedupId.ValueString + ". The node is missing.";
            this.log(TraceLevel.Error, message);
            throw new InvalidOperationException(message);
          }
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          List<DedupIdentifier> list = nullable.Value.ChildNodes.Select<DedupNode, DedupIdentifier>(DedupVisitor.\u003C\u003EO.\u003C0\u003E__Create ?? (DedupVisitor.\u003C\u003EO.\u003C0\u003E__Create = new Func<DedupNode, DedupIdentifier>(DedupIdentifier.Create))).ToList<DedupIdentifier>();
          if (block.IsAborting)
            return;
          foreach (DedupIdentifier child in list)
          {
            if (this.m_diag)
              Interlocked.Increment(ref this.m_visitCount);
            await block.PostAsync(DedupLink.Create(dedupId, child));
          }
          foreach (DedupIdentifier dedupId1 in list)
            await this.VisitDedupsTopDownAsync(processor, block, dedupId1);
          list = (List<DedupIdentifier>) null;
        }
      }
      catch (Exception ex)
      {
        this.Abort();
        throw;
      }
    }

    private async Task VisitDedupsBottomUpAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupProcessor block,
      DedupIdentifier dedupId,
      Func<DedupLink, Task> processAsync,
      bool parallelizeChunkProcessing,
      bool preprocessWithPathToRoot)
    {
      BottomUpDedupProcessorFactory factory = new BottomUpDedupProcessorFactory((VssRequestPump.Processor) processor, this.config, !parallelizeChunkProcessing, processAsync, this.log);
      IBottomUpDedupProcessor cproc = factory.CreateProcessor(this.m_cevaluator);
      if (cproc.Parallelism > 1)
        this.log(TraceLevel.Verbose, string.Format("{0} {1} Started bottom-up traversing with dedup processing parallelism = {2}.", (object) this.m_header, (object) "(perf)", (object) cproc.Parallelism));
      try
      {
        LinkedList<IDedupInfo> list = preprocessWithPathToRoot ? new LinkedList<IDedupInfo>() : (LinkedList<IDedupInfo>) null;
        NodeDedupIdentifier rootId = dedupId.CastToNodeDedupIdentifier();
        this.DedupContentSize = (await this.GetDedupNodeAsync(processor, rootId).ConfigureAwait(false)).Value.TransitiveContentBytes;
        DedupInfoLink andPushDedupsAsync = await this.GetAndPushDedupsAsync((IDedupInfo) null, (DedupIdentifier) rootId, list);
        if (andPushDedupsAsync != DedupInfoLink.Null)
        {
          list?.AddFirst(andPushDedupsAsync.Parent);
          cproc = await this.VisitNodesBottomUpInternalAsync(processor, factory, cproc, andPushDedupsAsync.Child, list);
        }
        await cproc.WaitForCompletionAsync();
        if (cproc.Parallelism > 1)
          this.log(TraceLevel.Verbose, string.Format("{0} {1} Decreased dedup processing parallelism (node-only) during bottom-up traversing: {2} => 1", (object) this.m_header, (object) "(perf)", (object) cproc.Parallelism));
        while (this.m_dedupInfoLinkStack.Count > 0)
        {
          if (block.IsAborting)
          {
            factory = (BottomUpDedupProcessorFactory) null;
            cproc = (IBottomUpDedupProcessor) null;
            return;
          }
          processor.CancellationToken.ThrowIfCancellationRequested();
          DedupLink dedupLink = this.m_dedupInfoLinkStack.Pop().ToDedupLink();
          if (this.m_diag)
            Interlocked.Increment(ref this.m_visitCount);
          await block.RunAsync(dedupLink);
        }
        list = (LinkedList<IDedupInfo>) null;
        rootId = (NodeDedupIdentifier) null;
        factory = (BottomUpDedupProcessorFactory) null;
        cproc = (IBottomUpDedupProcessor) null;
      }
      catch (Exception ex)
      {
        this.Abort();
        throw;
      }
    }

    private async Task<IBottomUpDedupProcessor> VisitNodesBottomUpInternalAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      BottomUpDedupProcessorFactory factory,
      IBottomUpDedupProcessor cproc,
      IDedupInfo dedupInfo,
      LinkedList<IDedupInfo> list)
    {
      processor.CancellationToken.ThrowIfCancellationRequested();
      DedupIdentifier dedupId = dedupInfo.Identifier;
      DedupNode? nullable = await this.GetDedupNodeAsync(processor, dedupId.CastToNodeDedupIdentifier()).ConfigureAwait(false);
      if (!nullable.HasValue)
      {
        if (this.m_skipMissing)
          return cproc;
        string message = this.m_header + " Cannot retrieve dedup node for " + dedupId.ValueString + ". The node is missing.";
        this.log(TraceLevel.Error, message);
        throw new InvalidOperationException(message);
      }
      foreach (DedupNode node in nullable.Value.ChildNodes.ToList<DedupNode>())
      {
        DedupIdentifier dedupIdentifier = node.Type == DedupNode.NodeType.InnerNode ? (DedupIdentifier) node.GetNodeIdentifier() : (DedupIdentifier) node.GetChunkIdentifier();
        if (ChunkerHelper.IsChunk(dedupIdentifier.AlgorithmId))
        {
          if (list == null)
          {
            if (await cproc.SendAsync(DedupLink.Create(dedupInfo?.Identifier, dedupIdentifier)))
            {
              await cproc.WaitForCompletionAsync();
              int parallelism1 = cproc.Parallelism;
              cproc = factory.CreateProcessor(this.m_cevaluator);
              int parallelism2 = cproc.Parallelism;
              if (parallelism2 > parallelism1)
                this.log(TraceLevel.Verbose, string.Format("{0} {1} Increased dedup processing parallelism (chunk-only) during bottom-up traversing: {2} => {3}", (object) this.m_header, (object) "(perf)", (object) parallelism1, (object) parallelism2));
            }
          }
          else
          {
            DedupInfoLink andPushDedupsAsync1 = await this.GetAndPushDedupsAsync(dedupInfo, dedupIdentifier, list);
          }
        }
        else
        {
          DedupInfoLink andPushDedupsAsync2 = await this.GetAndPushDedupsAsync(dedupInfo, dedupIdentifier, list);
          if (andPushDedupsAsync2 != DedupInfoLink.Null)
          {
            list?.AddFirst(andPushDedupsAsync2.Parent);
            try
            {
              IDedupInfo child = andPushDedupsAsync2.Child;
              cproc = await this.VisitNodesBottomUpInternalAsync(processor, factory, cproc, child, list);
            }
            finally
            {
              list?.RemoveFirst();
            }
          }
        }
      }
      return cproc;
    }

    protected virtual bool PreprocessDedupLinkTopDownAheadOfBottomUpTraversal(DedupInfoLink link) => false;

    protected virtual Task<IDedupInfo> GetDedupInfoAheadOfBottomUpTraversal(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier id)
    {
      return this.GetDedupInfoAsync(processor, id);
    }

    private async Task<DedupInfoLink> GetAndPushDedupsAsync(
      IDedupInfo parentDedupInfo,
      DedupIdentifier cid,
      LinkedList<IDedupInfo> list)
    {
      DedupInfoLink link = DedupInfoLink.Create(parentDedupInfo, await this.GetDedupInfoAheadOfBottomUpTraversal(this.processor, cid), (ICollection<IDedupInfo>) list);
      if (this.PreprocessDedupLinkTopDownAheadOfBottomUpTraversal(link))
        return DedupInfoLink.Null;
      this.m_dedupInfoLinkStack.Push(link);
      return link;
    }

    private async Task VisitDedupsInParallelAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupProcessor block,
      DedupIdentifier rootId)
    {
      DedupVisitor dedupVisitor1 = this;
      dedupVisitor1.m_treeDicoveryTracker = new Tracker();
      DedupVisitor dedupVisitor2 = dedupVisitor1;
      Func<DedupLink, Task> action = (Func<DedupLink, Task>) (async link =>
      {
        try
        {
          await this.VisitDedupsInParallelInternalAsync(processor, block, link, link.Parent == (DedupIdentifier) null);
          this.m_treeDicoveryTracker.Decrement();
        }
        catch (Exception ex)
        {
          this.Abort();
          throw;
        }
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = dedupVisitor1.config.DispatchingParallelism;
      dataflowBlockOptions.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions.BoundedCapacity = dedupVisitor1.config.DispatchingAndProcessingCapacity;
      ActionBlock<DedupLink> actionBlock = NonSwallowingActionBlock.Create<DedupLink>(action, dataflowBlockOptions);
      dedupVisitor2.m_queueBlock = actionBlock;
      dedupVisitor1.m_queueBlock.Post(DedupLink.Create((DedupIdentifier) null, rootId));
      Task task1 = dedupVisitor1.m_treeDicoveryTracker.Completion.ContinueWith((Action<Task>) (ante => this.m_queueBlock.Complete()));
      Task t2 = dedupVisitor1.m_queueBlock.Completion;
      Task task2 = await Task.WhenAny(task1, t2);
      await t2;
      t2 = (Task) null;
    }

    private async Task VisitDedupsInParallelInternalAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupProcessor block,
      DedupLink link,
      bool isRootNode)
    {
      processor.CancellationToken.ThrowIfCancellationRequested();
      DedupIdentifier dedupId = link.Child;
      object pdata = await this.PreprocessDedupLinkBeforeTopDownTraversalAsync(link, link.PathData, true);
      if (pdata is bool flag)
      {
        if (flag)
        {
          dedupId = (DedupIdentifier) null;
          pdata = (object) null;
        }
        else
        {
          string message = this.m_header + " The preprocessing for " + dedupId.ValueString + " returned false. This value is not allowed.";
          this.log(TraceLevel.Error, message);
          throw new InvalidOperationException(message);
        }
      }
      else
      {
        link.PathData = this.m_topDownCollectPathData ? pdata : (object) null;
        await block.PostAsync(link);
        if (!ChunkerHelper.IsNode(dedupId.AlgorithmId))
        {
          dedupId = (DedupIdentifier) null;
          pdata = (object) null;
        }
        else
        {
          DedupNode? nullable = await this.GetDedupNodeAsync(processor, dedupId.CastToNodeDedupIdentifier()).ConfigureAwait(false);
          if (!nullable.HasValue)
          {
            if (this.m_skipMissing)
            {
              dedupId = (DedupIdentifier) null;
              pdata = (object) null;
            }
            else
            {
              this.m_treeDicoveryTracker.Abort();
              string message = this.m_header + " Cannot retrieve dedup node for " + dedupId.ValueString + ". The node is missing.";
              this.log(TraceLevel.Error, message);
              throw new InvalidOperationException(message);
            }
          }
          else
          {
            if (isRootNode)
              this.DedupContentSize = nullable.Value.TransitiveContentBytes;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            List<DedupIdentifier> list = nullable.Value.ChildNodes.Select<DedupNode, DedupIdentifier>(DedupVisitor.\u003C\u003EO.\u003C0\u003E__Create ?? (DedupVisitor.\u003C\u003EO.\u003C0\u003E__Create = new Func<DedupNode, DedupIdentifier>(DedupIdentifier.Create))).ToList<DedupIdentifier>();
            if (block.IsAborting)
            {
              this.m_treeDicoveryTracker.Abort();
              dedupId = (DedupIdentifier) null;
              pdata = (object) null;
            }
            else
            {
              foreach (DedupIdentifier child in list)
              {
                if (this.m_diag)
                  Interlocked.Increment(ref this.m_visitCount);
                this.m_treeDicoveryTracker.Increment();
                DedupLink link1 = DedupLink.Create(dedupId, child);
                if (this.m_topDownCollectPathData)
                  link1.PathData = pdata;
                if (!this.m_queueBlock.Post(link1))
                {
                  this.m_treeDicoveryTracker.Decrement();
                  await this.VisitDedupsInParallelInternalAsync(processor, block, link1, false).ConfigureAwait(false);
                }
              }
              dedupId = (DedupIdentifier) null;
              pdata = (object) null;
            }
          }
        }
      }
    }

    protected virtual Task<object> PreprocessDedupLinkBeforeTopDownTraversalAsync(
      DedupLink link,
      object pathData,
      bool checkContinuation)
    {
      return Task.FromResult<object>((object) null);
    }
  }
}
