// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupRestoringVisitor
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class DedupRestoringVisitor : DedupVisitor
  {
    private readonly Func<TraceLevel, string, Task> log;

    public DedupRestoringVisitor(
      VssRequestPump.SecuredDomainProcessor processor,
      IDedupInfoRetriever retriever,
      IDedupRestorer restorer,
      DedupTraversalConfig config,
      Func<TraceLevel, string, Task> log)
    {
      VssRequestPump.SecuredDomainProcessor processor1 = processor;
      DedupInfoRestoringRetriever retriever1 = new DedupInfoRestoringRetriever(retriever, restorer);
      DedupTraversalConfig config1 = config;
      FixedConcurrencyEvaluator cevaluator = new FixedConcurrencyEvaluator();
      cevaluator.Concurrency = 1;
      Action<TraceLevel, string> log1 = (Action<TraceLevel, string>) (async (tl, message) => await log(tl, message).ConfigureAwait(false));
      // ISSUE: explicit constructor call
      base.\u002Ector(processor1, (IDedupInfoRetriever) retriever1, config1, (IBottomUpConcurrencyEvaluator) cevaluator, false, (string) null, log1);
      this.log = log;
    }

    public async Task<long> RestoreAsync(DedupIdentifier dedupId)
    {
      DedupRestoringVisitor restoringVisitor = this;
      // ISSUE: reference to a compiler-generated method
      return await restoringVisitor.VisitNodesAsync(dedupId, new Func<DedupLink, Task>(restoringVisitor.\u003CRestoreAsync\u003Eb__2_0));
    }

    private async Task ProcessAsync(DedupLink tuple)
    {
      DedupRestoringVisitor restoringVisitor = this;
      try
      {
        IDedupInfo dedupInfo = await restoringVisitor.GetDedupInfoAsync(restoringVisitor.processor, tuple.Child).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        await restoringVisitor.log(TraceLevel.Error, string.Format("[Dedup] Validation failed for {0}. Error: {1}", (object) tuple.Child, (object) ex.Message)).ConfigureAwait(false);
      }
    }
  }
}
