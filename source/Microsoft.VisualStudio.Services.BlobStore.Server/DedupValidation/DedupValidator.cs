// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation.DedupValidator
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation;
using Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation
{
  public class DedupValidator : DedupVisitor
  {
    private readonly DedupValidationResult result;
    private readonly IDedupProcessingCache cache;
    private readonly int frequency;
    private long m_total;
    private int m_processed;

    internal DedupValidator(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupInfoRetriever retriever,
      IDedupProcessingCache cache,
      DedupValidationResult result,
      DedupTraversalConfig config)
      : base(processor, retriever, config, (IBottomUpConcurrencyEvaluator) new FixedConcurrencyEvaluator()
      {
        Concurrency = 1
      }, false, (string) null, (Action<TraceLevel, string>) ((lvl, msg) => { }))
    {
      this.cache = cache;
      this.result = result;
      this.frequency = config.TracingFrequency;
    }

    public async Task<long> ValidateAsync(DedupIdentifier dedupId, Action<TraceLevel, string> log)
    {
      DedupValidator dedupValidator = this;
      long length = await dedupValidator.VisitNodesAsync(dedupId, (Func<DedupLink, Task>) (async tuple => await this.ProcessAsync(tuple, log)));
      dedupValidator.result.SetTotalQueueLength(length);
      return dedupValidator.m_total;
    }

    private async Task ProcessAsync(DedupLink tuple, Action<TraceLevel, string> log)
    {
      DedupValidator dedupValidator = this;
      if (dedupValidator.IsAborting)
        return;
      try
      {
        DedupIdentifier parentDedupId = tuple.Parent;
        DedupIdentifier thisDedupId = tuple.Child;
        IDedupInfo dedupInfo = (IDedupInfo) null;
        if (parentDedupId != (DedupIdentifier) null)
        {
          dedupInfo = dedupValidator.cache.GetDedupInfo((VssRequestPump.Processor) dedupValidator.processor, parentDedupId);
          if (dedupInfo == null)
          {
            dedupInfo = await dedupValidator.GetDedupInfoAsync(dedupValidator.processor, parentDedupId).ConfigureAwait(false);
            if (dedupInfo.Status != HealthStatus.Intact)
            {
              if (dedupValidator.OnFailure(dedupInfo))
                return;
              dedupValidator.Abort();
              return;
            }
            dedupValidator.cache.SetDedupInfo((VssRequestPump.Processor) dedupValidator.processor, parentDedupId, dedupInfo);
          }
        }
        IDedupInfo info = await dedupValidator.ProcessAsync(dedupValidator.cache, dedupInfo, thisDedupId, (object) null).ConfigureAwait(false);
        if (info == null)
          dedupValidator.Abort();
        else
          dedupValidator.cache.SetDedupInfo((VssRequestPump.Processor) dedupValidator.processor, thisDedupId, info);
        parentDedupId = (DedupIdentifier) null;
        thisDedupId = (DedupIdentifier) null;
      }
      catch (Exception ex)
      {
        log(TraceLevel.Error, "[Dedup] Validation failed for " + dedupValidator.result.ExtraInfo + ". Error: " + ex.Message);
        dedupValidator.result.SetFailure(ex);
        dedupValidator.Abort();
      }
      finally
      {
        int num = (int) Interlocked.Increment(ref dedupValidator.m_total) >> dedupValidator.frequency;
        int processed = dedupValidator.m_processed;
        if (num > processed && processed == Interlocked.CompareExchange(ref dedupValidator.m_processed, num, processed))
          log(TraceLevel.Info, string.Format("[Dedup] Validated {0} nodes/chunks for {1}", (object) dedupValidator.m_total, (object) dedupValidator.result.ExtraInfo));
      }
    }

    protected async Task<IDedupInfo> GetDedupInfoAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupProcessingCache vcache,
      DedupIdentifier dedup)
    {
      DedupValidator dedupValidator = this;
      IDedupInfo dedupInfo = vcache.GetDedupInfo((VssRequestPump.Processor) processor, dedup);
      if (dedupInfo == null)
        return await dedupValidator.GetDedupInfoAsync(processor, dedup).ConfigureAwait(false);
      if (dedupValidator.m_diag)
        dedupValidator.result.IncrementDedupInfoCacheHit();
      return dedupInfo;
    }

    private async Task<IDedupInfo> ProcessAsync(
      IDedupProcessingCache vcache,
      IDedupInfo parentInfo,
      DedupIdentifier dedup,
      object pathData)
    {
      DedupValidator dedupValidator = this;
      DedupIdentifier parentId = parentInfo?.Identifier;
      if (vcache.IsParentChildValidated(parentId, dedup))
      {
        if (dedupValidator.m_diag)
          dedupValidator.result.IncrementValidationCacheHit();
        return await dedupValidator.GetDedupInfoAsync(dedupValidator.processor, vcache, dedup).ConfigureAwait(false);
      }
      IDedupInfo dedupInfo = (IDedupInfo) null;
      int retries = 2;
      while (retries > 0)
      {
        dedupInfo = await dedupValidator.GetDedupInfoAsync(dedupValidator.processor, vcache, dedup).ConfigureAwait(false);
        switch (dedupInfo.Status)
        {
          case HealthStatus.Intact:
            retries = 0;
            break;
          case HealthStatus.Absent:
          case HealthStatus.MissingMetadata:
            dedupValidator.result.SetFailure(dedupInfo);
            return (IDedupInfo) null;
          case HealthStatus.Undetermined:
            --retries;
            if (retries <= 0)
            {
              dedupValidator.result.SetFailure(dedupInfo);
              return (IDedupInfo) null;
            }
            break;
        }
        DateTime? keepUntil1 = (DateTime?) parentInfo?.KeepUntil;
        if (keepUntil1.HasValue)
        {
          DateTime dateTime = keepUntil1.Value;
          DateTime? keepUntil2 = dedupInfo.KeepUntil;
          if ((keepUntil2.HasValue ? (dateTime > keepUntil2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          {
            dedupValidator.result.SetKeepUntilViolation(parentInfo, dedupInfo);
            return (IDedupInfo) null;
          }
        }
      }
      vcache.AddValidatedParentChild(parentId, dedup);
      return dedupInfo;
    }

    public virtual bool OnFailure(IDedupInfo dedupInfo)
    {
      this.result.SetFailure(dedupInfo);
      return false;
    }
  }
}
