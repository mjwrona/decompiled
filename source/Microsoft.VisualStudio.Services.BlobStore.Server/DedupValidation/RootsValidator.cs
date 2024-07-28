// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation.RootsValidator
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation;
using Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation
{
  public class RootsValidator
  {
    private readonly VssRequestPump.Processor processor;
    private readonly IDedupInfoRetriever retriever;
    private readonly RootsValidationConfig config;
    private readonly RootsValidationResult result;
    private readonly Action<TraceLevel, string> log;
    private SafeTaskCompletionSource<int> m_tcs;
    private ActionBlock<DedupMetadataEntry> m_block;
    private bool m_shouldTerminate;
    private CancellationToken m_token;
    private int m_total;

    public bool IsAborting => this.m_shouldTerminate || this.m_token.IsCancellationRequested;

    internal RootsValidator(
      VssRequestPump.SecuredDomainProcessor processor,
      RootsValidationConfig config,
      IDedupInfoRetriever retriever,
      IDedupProcessingCache cache,
      RootsValidationResult result,
      Action<TraceLevel, string> log)
    {
      RootsValidator rootsValidator = this;
      this.processor = (VssRequestPump.Processor) processor;
      this.retriever = retriever;
      this.config = config;
      this.result = result;
      this.log = log;
      int totalRoots = 0;
      this.m_token = processor.CancellationToken;
      this.m_tcs = new SafeTaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
      Func<DedupMetadataEntry, Task> action = (Func<DedupMetadataEntry, Task>) (async entry =>
      {
        DedupValidationResult dresult;
        if (rootsValidator.IsAborting)
        {
          dresult = (DedupValidationResult) null;
        }
        else
        {
          DedupIdentifier dedupId1 = entry.DedupId;
          string scope = entry.Scope;
          string referenceId = entry.ReferenceId;
          dresult = new DedupValidationResult((DedupTraversalConfig) rootsValidator.config, referenceId + "(" + scope + ")");
          DedupValidator dedupValidator = new DedupValidator(processor, retriever, cache, dresult, (DedupTraversalConfig) rootsValidator.config);
          int seq = Interlocked.Increment(ref totalRoots);
          log(TraceLevel.Info, string.Format("[ROOT({0})] Started validation for {1}", (object) seq, (object) dresult.ExtraInfo));
          DedupIdentifier dedupId2 = dedupId1;
          Action<TraceLevel, string> log1 = rootsValidator.log;
          dresult.VisitCount = await dedupValidator.ValidateAsync(dedupId2, log1).ConfigureAwait(false);
          if (dresult.Status == ResultStatus.Success)
          {
            log(TraceLevel.Info, string.Format("[ROOT({0})] Completed validation for {1}", (object) seq, (object) dresult.ExtraInfo));
            result.RecordSuccess((ITraversalResult) dresult);
            dresult = (DedupValidationResult) null;
          }
          else
          {
            log(TraceLevel.Warning, string.Format("[ROOT({0})] Failed validation for {1}", (object) seq, (object) dresult.ExtraInfo));
            if (result.RecordFailure((ITraversalResult) dresult) < rootsValidator.config.MaxFailuresBeforeAbortion)
            {
              dresult = (DedupValidationResult) null;
            }
            else
            {
              rootsValidator.Abort();
              dresult = (DedupValidationResult) null;
            }
          }
        }
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = this.config.RootLevelParallelism;
      dataflowBlockOptions.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions.BoundedCapacity = this.config.DispatchingAndProcessingCapacity;
      this.m_block = NonSwallowingActionBlock.Create<DedupMetadataEntry>(action, dataflowBlockOptions);
    }

    public async Task PostAsync(DedupMetadataEntry entry)
    {
      ++this.m_total;
      if (await this.m_block.SendAsync<DedupMetadataEntry>(entry))
        return;
      this.Abort();
    }

    public Task CompleteAsync()
    {
      this.log(TraceLevel.Info, string.Format("[VALIDATION] Dispatched all of {0} roots for {1}", (object) this.m_total, (object) this.result.ExtraInfo));
      this.m_block.Complete();
      return (Task) Task.WhenAny(this.m_block.Completion, (Task) this.m_tcs.Task);
    }

    private void Abort()
    {
      this.log(TraceLevel.Warning, "[VALIDATION] Aborting validation for " + this.result.ExtraInfo);
      this.m_shouldTerminate = true;
      this.m_tcs.SetResult(1);
    }
  }
}
