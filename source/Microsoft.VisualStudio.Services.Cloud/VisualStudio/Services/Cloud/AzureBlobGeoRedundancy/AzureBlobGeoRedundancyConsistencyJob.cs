// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyConsistencyJob
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyConsistencyJob : ITeamFoundationJobExtension
  {
    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      AzureBlobGeoRedundancyConsistencyJobData consistencyJobData = TeamFoundationSerializationUtility.Deserialize<AzureBlobGeoRedundancyConsistencyJobData>(jobDefinition.Data);
      GeoRedundantStorageAccountSettings storageAccount = new GeoRedundantStorageAccountSettings()
      {
        DrawerName = consistencyJobData.DrawerName,
        PrimaryLookupKey = consistencyJobData.PrimaryLookupKey,
        SecondaryLookupKey = consistencyJobData.SecondaryLookupKey
      };
      AzureBlobGeoRedundancyConsistencySettings settings = new AzureBlobGeoRedundancyConsistencySettings()
      {
        Repair = consistencyJobData.Repair,
        CheckContentMD5 = consistencyJobData.CheckContentMD5,
        SyncSource = consistencyJobData.SyncSource,
        ContainerNames = consistencyJobData.ContainerNames,
        QueueCopies = consistencyJobData.QueueCopies,
        ExcludeBlobsBefore = consistencyJobData.ExcludeBlobsBefore,
        CleanupTarget = consistencyJobData.CleanupTarget,
        CleanupMinimumAge = consistencyJobData.CleanupMinimumAge
      };
      try
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        BlobRepairStats blobRepairStats = requestContext.GetService<IAzureBlobGeoRedundancyConsistencyService>().CheckConsistency(requestContext, storageAccount, settings, consistencyJobData.Verbose ? AzureBlobGeoRedundancyConsistencyJob.\u003C\u003EO.\u003C0\u003E__TraceRawAlwaysOn ?? (AzureBlobGeoRedundancyConsistencyJob.\u003C\u003EO.\u003C0\u003E__TraceRawAlwaysOn = new ThreadSafeTraceMethod(TeamFoundationTracingService.TraceRawAlwaysOn)) : AzureBlobGeoRedundancyConsistencyJob.\u003C\u003EO.\u003C1\u003E__TraceRaw ?? (AzureBlobGeoRedundancyConsistencyJob.\u003C\u003EO.\u003C1\u003E__TraceRaw = new ThreadSafeTraceMethod(TeamFoundationTracingService.TraceRaw)));
        resultMessage = "Successfully checked all containers! " + blobRepairStats.ToString();
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex) when (AzureBlobGeoRedundancyConsistencyJob.IsCancelledException(ex))
      {
        resultMessage = "Job was cancelled!";
        return TeamFoundationJobExecutionResult.PartiallySucceeded;
      }
    }

    private static bool IsCancelledException(Exception e)
    {
      switch (e)
      {
        case TaskCanceledException _:
        case OperationCanceledException _:
          return true;
        case AggregateException aggregateException:
          return aggregateException.InnerExceptions.Any<Exception>((Func<Exception, bool>) (ie => ie is TaskCanceledException || ie is OperationCanceledException));
        default:
          return false;
      }
    }
  }
}
