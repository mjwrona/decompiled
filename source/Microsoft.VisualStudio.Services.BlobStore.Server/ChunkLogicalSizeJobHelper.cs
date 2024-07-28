// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ChunkLogicalSizeJobHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Storage;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.DataExport;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ChunkLogicalSizeJobHelper
  {
    public async Task PerformChunkLogicalSizeAccounting(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IAdminDedupStore dedupStore,
      IDedupLogicalDataJobResult jobResult,
      JobParameters jobParameters,
      string prefix,
      int CpuThreshold,
      bool CPUThrottlingDisabled,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      List<IDedupMetadataEntryProcessor> enabledChunkExporters = ReferenceExporterPluginRegistration.GetEnabledChunkExporters(requestContext, jobParameters).ToList<IDedupMetadataEntryProcessor>();
      await requestContext.PumpFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, domainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
      {
        using (IConcurrentIterator<IEnumerable<DedupMetadataEntry>> rootPages = dedupStore.GetRootPages(processor, new DedupMetadataPageRetrievalOption(prefix, new DateTimeOffset?(), new DateTimeOffset?(), ResultArrangement.AllUnordered, 0, StateFilter.Active), tracer))
          await rootPages.ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(processor.CancellationToken, (Func<IEnumerable<DedupMetadataEntry>, Task>) (async page =>
          {
            List<DedupMetadataEntry> dedupMetadataEntries = page.ToList<DedupMetadataEntry>();
            if (!CPUThrottlingDisabled)
            {
              int num = await CpuThrottleHelper.Instance.Yield(CpuThreshold, processor.CancellationToken).ConfigureAwait(false);
              jobResult.TotalThrottleDuration += TimeSpan.FromSeconds((double) num);
            }
            jobResult.TotalRootsDiscovered += (ulong) dedupMetadataEntries.LongCount<DedupMetadataEntry>();
            DedupLogicalDataJobResult jobResult2 = jobResult as DedupLogicalDataJobResult;
            await this.PerformChunkLogicalSizeAccounting(processor, dedupStore, jobResult2, dedupMetadataEntries, (IEnumerable<IDedupMetadataEntryProcessor>) enabledChunkExporters, tracer);
            dedupMetadataEntries = (List<DedupMetadataEntry>) null;
          })).ConfigureAwait(false);
      }));
      try
      {
        using (List<IDedupMetadataEntryProcessor>.Enumerator enumerator = enabledChunkExporters.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Complete();
        }
      }
      catch (Exception ex)
      {
        tracer.TraceException(ex);
      }
    }

    private async Task PerformChunkLogicalSizeAccounting(
      VssRequestPump.SecuredDomainProcessor processor,
      IAdminDedupStore dedupStore,
      DedupLogicalDataJobResult jobResult,
      List<DedupMetadataEntry> dedupMetadataEntries,
      IEnumerable<IDedupMetadataEntryProcessor> dedupMetadataEntryProcessors,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      foreach (DedupMetadataEntry dedupMetadataEntry1 in dedupMetadataEntries.Where<DedupMetadataEntry>((Func<DedupMetadataEntry, bool>) (entry => entry != null)))
      {
        DedupMetadataEntry dedupMetadataEntry = dedupMetadataEntry1;
        if (dedupMetadataEntry.IsSoftDeleted)
        {
          ++jobResult.TotalRootsSoftDeleted;
        }
        else
        {
          if (ChunkerHelper.IsChunk(dedupMetadataEntry.DedupId.AlgorithmId))
          {
            ++jobResult.TotalChunkRoots;
            tracer.TraceInfo(string.Format("Skipping root {0} since it is a chunk.", (object) dedupMetadataEntry.DedupId));
          }
          else
          {
            ulong size = 0;
            long? size1 = dedupMetadataEntry.Size;
            if (size1.HasValue)
            {
              size1 = dedupMetadataEntry.Size;
              size = (ulong) size1.Value;
            }
            else
            {
              NodeDedupIdentifier dedupNodeId = dedupMetadataEntry.DedupId.CastToNodeDedupIdentifier();
              DedupNode? nullable = await AsyncHttpRetryHelper.InvokeAsync<DedupNode?>((Func<Task<DedupNode?>>) (async () => await dedupStore.GetDedupNodeAsync(processor, dedupNodeId).ConfigureAwait(false)), 3, (IAppTraceSource) NoopAppTraceSource.Instance, (Func<Exception, bool>) (e =>
              {
                bool flag;
                switch (e)
                {
                  case ExpandedTableStorageException _:
                  case ExpandedStorageException _:
                    flag = true;
                    break;
                  default:
                    flag = false;
                    break;
                }
                return flag && ((StorageException) e).RequestInformation.HttpStatusCode == 306;
              }), processor.CancellationToken, false, nameof (PerformChunkLogicalSizeAccounting));
              if (!nullable.HasValue)
                tracer.TraceAlways("Blob backing the node was not found: " + string.Format("NodeId: {0}, ", (object) dedupNodeId) + string.Format("BlobId: {0}, ", (object) dedupMetadataEntry.DedupId.ToBlobIdentifier()) + "RefId: " + dedupMetadataEntry.ReferenceId + ", Scope: " + dedupMetadataEntry.Scope + ", DomainId: " + processor.SecuredDomainRequest.Serialize<ISecuredDomainRequest>());
              else
                size = nullable.Value.TransitiveContentBytes;
            }
            if (dedupMetadataEntry.ReferenceId.StartsWith("feed"))
            {
              string[] source = dedupMetadataEntry.ReferenceId.Split('/');
              if (((IEnumerable<string>) source).Count<string>() >= 2 && source[0].Equals("feed", StringComparison.OrdinalIgnoreCase))
              {
                long num = (long) jobResult.LogicalSizeByFeed.AddOrUpdate(source[1], size, (Func<string, ulong, ulong>) ((key, existing) => existing + size));
              }
            }
            ArtifactScopeType scopeTypeFromScopeId = ArtifactScopeHelper.GetScopeTypeFromScopeId(dedupMetadataEntry.Scope);
            long num1 = (long) jobResult.SizeByScope.AddOrUpdate(scopeTypeFromScopeId, size, (Func<ArtifactScopeType, ulong, ulong>) ((key, cur) => cur + size));
            if (scopeTypeFromScopeId == ArtifactScopeType.PipelineArtifact || scopeTypeFromScopeId == ArtifactScopeType.PipelineCache || scopeTypeFromScopeId == ArtifactScopeType.BuildArtifacts || scopeTypeFromScopeId == ArtifactScopeType.BuildLogs)
              jobResult.TotalBytesOutOfScope += size;
            else
              jobResult.TotalBytes += size;
          }
          try
          {
            foreach (IDedupMetadataEntryProcessor metadataEntryProcessor in dedupMetadataEntryProcessors)
              metadataEntryProcessor.VisitEntry(dedupMetadataEntry);
          }
          catch (Exception ex)
          {
          }
          ++jobResult.TotalRootsEvaluated;
          dedupMetadataEntry = (DedupMetadataEntry) null;
        }
      }
    }
  }
}
