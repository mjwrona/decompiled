// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.CleanupJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  public class CleanupJob : ITeamFoundationJobExtension
  {
    private const int MaxRetries = 3;

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(requestContext, (RegistryQuery) OnPremConstants.RegistryPathCleanupJobTimeBudgetInMin, 30);
      if (num1 <= 0)
      {
        resultMessage = "BlobStore cleanup disabled. (Time budget <= 0 min)";
        return TeamFoundationJobExecutionResult.Blocked;
      }
      int batchCount = service.GetValue<int>(requestContext, (RegistryQuery) OnPremConstants.RegistryPathCleanupJobBatchSize, 1000);
      if (batchCount < 100)
        batchCount = 100;
      else if (batchCount > 2000)
        batchCount = 2000;
      int num2 = 3;
      Stopwatch stopwatch = Stopwatch.StartNew();
      bool flag = true;
      SqlBlobProvider sqlBlobProvider = new SqlBlobProvider(BlobStoreProviderConstants.BlobStoreSuffix);
      while (flag)
      {
        try
        {
          if (stopwatch.Elapsed <= TimeSpan.FromMinutes((double) num1))
          {
            flag = sqlBlobProvider.Cleanup(requestContext, batchCount);
            num2 = 3;
          }
          else
          {
            resultMessage = "BlobStore cleanup partially done.";
            return TeamFoundationJobExecutionResult.Succeeded;
          }
        }
        catch (Exception ex)
        {
          --num2;
          if (num2 < 0)
          {
            resultMessage = "BlobStore cleanup failed: " + ex.Message;
            return TeamFoundationJobExecutionResult.Failed;
          }
        }
      }
      resultMessage = "BlobStore cleanup done.";
      return TeamFoundationJobExecutionResult.Succeeded;
    }
  }
}
