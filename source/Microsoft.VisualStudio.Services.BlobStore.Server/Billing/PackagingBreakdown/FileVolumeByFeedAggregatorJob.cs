// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown.FileVolumeByFeedAggregatorJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown
{
  public class FileVolumeByFeedAggregatorJob : VssAsyncJobExtension
  {
    public static readonly Guid JobId = Guid.Parse("090d6dfc-5d73-456e-8359-4320e387cdbe");

    public override Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      IDomainId defaultDomainId = WellKnownDomainIds.DefaultDomainId;
      FilePackagingStorageBreakdownService service = requestContext.GetService<FilePackagingStorageBreakdownService>();
      (string runId, int totalPartitions) = service.GetRunResult(requestContext, defaultDomainId);
      if (string.IsNullOrEmpty(runId))
        return Task.FromResult<VssJobResult>(new VssJobResult(TeamFoundationJobExecutionResult.Failed, "No existing results were found."));
      List<FileVolumeByFeedResult> list = service.GetPartitionResults(requestContext, defaultDomainId).ToList<FileVolumeByFeedResult>();
      FileVolumeByFeedAggregatedResult dataContractObject = new FileVolumeByFeedAggregatedResult();
      foreach (FileVolumeByFeedResult volumeByFeedResult in list)
      {
        foreach (KeyValuePair<string, ulong> keyValuePair in volumeByFeedResult.LogicalSizeByFeed)
        {
          long num = (long) dataContractObject.LogicalSizeByFeed.AddOrUpdate<string, ulong>(keyValuePair.Key, keyValuePair.Value, (Func<ulong, ulong, ulong>) ((existing, newValue) => existing + newValue));
        }
      }
      dataContractObject.RunId = runId;
      dataContractObject.TotalPartitions = totalPartitions;
      dataContractObject.TotalSucceededPartitions = list.Count;
      dataContractObject.LogicalSizeByFeed = UsageInfoServiceExtensions.GetTopLogicalSizeByFeed(dataContractObject.LogicalSizeByFeed);
      return Task.FromResult<VssJobResult>(new VssJobResult(dataContractObject.TotalSucceededPartitions == dataContractObject.TotalPartitions ? TeamFoundationJobExecutionResult.Succeeded : TeamFoundationJobExecutionResult.PartiallySucceeded, JsonSerializer.Serialize<FileVolumeByFeedAggregatedResult>(dataContractObject)));
    }
  }
}
