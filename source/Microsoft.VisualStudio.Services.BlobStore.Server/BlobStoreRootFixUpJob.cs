// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStoreRootFixUpJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class BlobStoreRootFixUpJob : VssAsyncJobExtension
  {
    private SingleLocationTracePoint JobTracePoint = new SingleLocationTracePoint(5701999, "BlobStore", "Job");
    private const string LastExecutionResultRegistryKey = "/Configuration/BlobStore/BlobStoreRootFixUpJob/LastExecutionResult";
    private const string DisabledRegistryKey = "/Configuration/BlobStore/BlobStoreRootFixUpJob/Disabled";
    private TimeSpan progressInterval = TimeSpan.FromMinutes(30.0);
    private Stopwatch progressTimer = Stopwatch.StartNew();

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      string skipReason;
      if (this.SkipExecution(requestContext, out skipReason))
        return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, skipReason);
      List<PhysicalDomainInfo> list = (await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext)).ToList<PhysicalDomainInfo>();
      AdminDedupStoreService dedupService = requestContext.GetService<AdminDedupStoreService>();
      BlobStoreRootFixUpJob.FixUpStatistics fixUpStatistics = new BlobStoreRootFixUpJob.FixUpStatistics();
      foreach (MultiDomainInfo multiDomainInfo in list)
      {
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, multiDomainInfo.DomainId);
        int num = await this.FixUpDomain(requestContext, (IAdminDedupStore) dedupService, domainRequest, fixUpStatistics) ? 1 : 0;
      }
      string message = JsonConvert.SerializeObject((object) fixUpStatistics);
      requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, "/Configuration/BlobStore/BlobStoreRootFixUpJob/LastExecutionResult", message);
      return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, message);
    }

    private bool SkipExecution(IVssRequestContext requestContext, out string skipReason)
    {
      skipReason = string.Empty;
      bool flag = false;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (service.GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/BlobStore/BlobStoreRootFixUpJob/Disabled", true, false))
      {
        skipReason = "Job is disabled.";
        flag = true;
      }
      else
      {
        string str = service.GetValue(requestContext, (RegistryQuery) "/Configuration/BlobStore/BlobStoreRootFixUpJob/LastExecutionResult", false, string.Empty);
        BlobStoreRootFixUpJob.FixUpStatistics fixUpStatistics = (BlobStoreRootFixUpJob.FixUpStatistics) null;
        try
        {
          if (!string.IsNullOrEmpty(str))
            fixUpStatistics = JsonConvert.DeserializeObject<BlobStoreRootFixUpJob.FixUpStatistics>(str);
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(this.JobTracePoint, "Exception deserializing last execution result {0}.", (object) ex);
        }
        if (fixUpStatistics != null && fixUpStatistics.TotalRootsSizeUpdated == 0L && fixUpStatistics.TotalRootsErrored == 0L)
        {
          flag = true;
          skipReason = "Execution skipped because all roots have been updated in previous run.";
        }
      }
      return flag;
    }

    private async Task<bool> FixUpDomain(
      IVssRequestContext requestContext,
      IAdminDedupStore dedupStore,
      SecuredDomainRequest domainRequest,
      BlobStoreRootFixUpJob.FixUpStatistics statistics)
    {
      bool flag;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.JobTracePoint, nameof (FixUpDomain)))
      {
        await requestContext.PumpFromAsync((ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
        {
          using (IConcurrentIterator<IEnumerable<DedupMetadataEntry>> rootPages = dedupStore.GetRootPages(processor, new DedupMetadataPageRetrievalOption((string) null, new DateTimeOffset?(), new DateTimeOffset?(), ResultArrangement.AllUnordered, 0, StateFilter.Active), tracer))
            await rootPages.ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(processor.CancellationToken, (Func<IEnumerable<DedupMetadataEntry>, Task>) (async page => await this.FixUpPage(dedupStore, statistics, processor, page))).ConfigureAwait(false);
        }));
        flag = true;
      }
      return flag;
    }

    private async Task FixUpPage(
      IAdminDedupStore dedupStore,
      BlobStoreRootFixUpJob.FixUpStatistics statistics,
      VssRequestPump.SecuredDomainProcessor processor,
      IEnumerable<DedupMetadataEntry> page)
    {
      foreach (DedupMetadataEntry entry in page)
      {
        ++statistics.TotalRootsScanned;
        try
        {
          if (await dedupStore.UpdateRootSizeAsync(processor, entry))
            ++statistics.TotalRootsSizeUpdated;
        }
        catch (Exception ex)
        {
          ++statistics.TotalRootsErrored;
          if (statistics.TotalRootsErrored < 1000L)
            await processor.TraceAlwaysAsync(this.JobTracePoint, "Unable to update size for root {0}. {1}", (object) entry.DedupId.ValueString, (object) ex);
        }
      }
      if (!(this.progressTimer.Elapsed > this.progressInterval))
        return;
      this.progressTimer.Restart();
      await processor.TraceAlwaysAsync(this.JobTracePoint, "{0}", (object) JsonConvert.SerializeObject((object) statistics));
    }

    private class FixUpStatistics
    {
      public long TotalRootsScanned { get; set; }

      public long TotalRootsSizeUpdated { get; set; }

      public long TotalRootsErrored { get; set; }
    }
  }
}
