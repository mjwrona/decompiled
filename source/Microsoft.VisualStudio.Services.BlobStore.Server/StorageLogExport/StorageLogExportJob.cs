// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogExport.StorageLogExportJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogExport
{
  public class StorageLogExportJob : VssAsyncJobExtension
  {
    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      DateTimeOffset dateTimeOffset = utcNow.Subtract(TimeSpan.FromHours(1.0));
      StorageLogExportJobDefinition exportJobDefinition = TeamFoundationSerializationUtility.Deserialize<StorageLogExportJobDefinition>(jobDefinition.Data);
      DateTimeOffset? jobDataTime = this.ParseJobDataTime(exportJobDefinition.StartTime);
      DateTimeOffset startTime = jobDataTime ?? dateTimeOffset;
      jobDataTime = this.ParseJobDataTime(exportJobDefinition.EndTime);
      DateTimeOffset endTime = jobDataTime ?? utcNow;
      string filePath = exportJobDefinition.FilePath;
      string filterString = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) ServiceRegistryConstants.StorageLogsStatsJobFilterStringPath, true, string.Empty);
      try
      {
        await new StorageLogExporter().ExportLogsAsync(requestContext, startTime, endTime, filePath, filterString);
        return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, "Success");
      }
      catch (Exception ex)
      {
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, ex.Message);
      }
    }

    private DateTimeOffset? ParseJobDataTime(string timeStringToConvert)
    {
      DateTimeOffset result;
      DateTimeOffset.TryParse(timeStringToConvert, (IFormatProvider) null, DateTimeStyles.AssumeUniversal, out result);
      return result.EqualsExact(DateTimeOffset.MinValue) ? new DateTimeOffset?() : new DateTimeOffset?(result);
    }
  }
}
