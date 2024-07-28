// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility.DedupProcessor
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility
{
  public class DedupProcessor : IDedupProcessor
  {
    private VssRequestPump.Processor processor;
    private bool m_shouldTerminate;
    private CancellationToken m_token;
    private ActionBlock<DedupLink> m_block;
    private Action<TraceLevel, string> log;
    private IOperationProfiler profiler;
    private Func<DedupLink, Task> m_processAsync;
    private bool m_processInline;

    public bool IsAborting => this.m_shouldTerminate || this.m_token.IsCancellationRequested;

    public DedupProcessor(
      VssRequestPump.Processor processor,
      DedupProcessingConfig dpconf,
      Func<DedupLink, Task> processAsync,
      Action<TraceLevel, string> log,
      bool processInline)
    {
      this.processor = processor;
      this.log = log;
      this.profiler = dpconf.Profiler;
      this.m_processInline = processInline;
      this.m_processAsync = processAsync;
      if (processInline)
        return;
      this.m_token = processor.CancellationToken;
      Func<DedupLink, Task> action = (Func<DedupLink, Task>) (async dl =>
      {
        this.profiler.Decrement(ProfilingCategory.Man_QueuedForProcessingCount);
        await this.RunAsync(dl);
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = dpconf.ProcessingParallelism;
      dataflowBlockOptions.CancellationToken = this.m_token;
      dataflowBlockOptions.BoundedCapacity = dpconf.DispatchingAndProcessingCapacity;
      this.m_block = NonSwallowingActionBlock.Create<DedupLink>(action, dataflowBlockOptions);
    }

    public async Task RunAsync(DedupLink dl)
    {
      try
      {
        this.profiler.Increment(ProfilingCategory.Agg_ProcessingCount);
        Stopwatch sw = Stopwatch.StartNew();
        await this.m_processAsync(dl).ConfigureAwait(false);
        sw.Stop();
        this.profiler.Update(ProfilingCategory.Avg_ProcessDedupLinkTime, (long) sw.Elapsed.TotalMilliseconds);
        this.profiler.Decrement(ProfilingCategory.Agg_ProcessingCount);
        sw = (Stopwatch) null;
      }
      catch (OperationCanceledException ex)
      {
        CancellationToken cancellationToken = this.processor.CancellationToken;
        throw NonSwallowingTransformBlockUtils.CreateTimeoutException(ex, "Dedup Processor", cancellationToken);
      }
    }

    public async Task PostAsync(DedupLink link)
    {
      int num = await this.m_block.SendAsync<DedupLink>(link).ConfigureAwait(false) ? 1 : 0;
      this.profiler.Increment(ProfilingCategory.Man_QueuedForProcessingCount);
      if (num != 0)
        return;
      this.m_shouldTerminate = true;
    }

    public Task CompleteAsync()
    {
      this.m_block.Complete();
      return this.m_block.Completion;
    }

    public void Abort() => this.m_shouldTerminate = true;
  }
}
