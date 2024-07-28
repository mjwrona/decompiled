// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats.StorageLogStatsJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats
{
  public class StorageLogStatsJob : VssAsyncJobExtension
  {
    public override Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      DateTimeOffset formattable = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(2.0));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = service.GetValue(requestContext, (RegistryQuery) ServiceRegistryConstants.StorageLogsStatsJobFilterStringPath, true, string.Empty);
      string input1 = service.GetValue(requestContext, (RegistryQuery) ServiceRegistryConstants.StorageLogsStatsJobStartTimePath, true, formattable.ToStringSafe());
      string input2 = service.GetValue(requestContext, (RegistryQuery) ServiceRegistryConstants.StorageLogsStatsJobEndTimePath, true, formattable.ToStringSafe());
      DateTimeOffset result;
      DateTimeOffset.TryParse(input1, (IFormatProvider) null, DateTimeStyles.AssumeUniversal, out result);
      DateTimeOffset dateTimeOffset;
      ref DateTimeOffset local = ref dateTimeOffset;
      DateTimeOffset.TryParse(input2, (IFormatProvider) null, DateTimeStyles.AssumeUniversal, out local);
      StorageLogStatsProcessor logStatsProcessor = new StorageLogStatsProcessor(requestContext);
      int num = int.Parse(service.GetValue(requestContext, (RegistryQuery) ServiceRegistryConstants.StorageLogStatsJobModePath, true, "0"));
      try
      {
        StorageLogStatsJob.JobInfo jobInfo = new StorageLogStatsJob.JobInfo()
        {
          startTime = result,
          endTime = dateTimeOffset,
          filterString = str,
          processor = logStatsProcessor
        };
        switch (num)
        {
          case 0:
          case 1:
            return this.AggregateEgress(requestContext, jobInfo);
          default:
            return this.AggregateLatency(requestContext, jobInfo);
        }
      }
      catch (Exception ex)
      {
        return Task.FromResult<VssJobResult>(new VssJobResult(TeamFoundationJobExecutionResult.Failed, string.Format("Error: Exception {0}", (object) ex)));
      }
    }

    private Task<VssJobResult> AggregateLatency(
      IVssRequestContext requestContext,
      StorageLogStatsJob.JobInfo jobInfo)
    {
      return Task.FromResult<VssJobResult>(new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonSerializer.Serialize<AggregatedStorageLogStatsResult>(jobInfo.processor.AggregateStorageLogTransactions(requestContext, (IStorageAccountAdapter) new StorageAccountAdapter(), jobInfo.startTime, jobInfo.endTime, jobInfo.filterString))));
    }

    private Task<VssJobResult> AggregateEgress(
      IVssRequestContext requestContext,
      StorageLogStatsJob.JobInfo jobInfo)
    {
      return Task.FromResult<VssJobResult>(new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonSerializer.Serialize<Dictionary<Guid, long?>>(jobInfo.processor.AggregateEgressByHostId(requestContext, (IStorageAccountAdapter) new StorageAccountAdapter(), jobInfo.startTime, jobInfo.endTime, jobInfo.filterString))));
    }

    private class JobInfo
    {
      public DateTimeOffset startTime;
      public DateTimeOffset endTime;
      public string filterString;
      public StorageLogStatsProcessor processor;
    }
  }
}
