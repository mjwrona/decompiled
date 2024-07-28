// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.CheckSuiteResult
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal static class CheckSuiteResult
  {
    public static CheckSuite GetCheckSuiteResponse(
      Guid checkSuiteId,
      List<CheckRun> checkRuns,
      CheckSuiteContext suiteContext)
    {
      DateTime? nullable = new DateTime?();
      if (suiteContext == null)
        suiteContext = new CheckSuiteContext();
      string message;
      CheckRunStatus checkSuiteStatus = CheckSuiteResult.GetCheckSuiteStatus(checkRuns, out message);
      if (CheckSuiteResult.isCheckSuiteCompleted(checkSuiteStatus))
        nullable = CheckSuiteResult.GetCheckSuiteCompletedDate(checkRuns, checkSuiteStatus);
      CheckSuite checkSuiteResponse = new CheckSuite();
      checkSuiteResponse.Status = checkSuiteStatus;
      checkSuiteResponse.Id = checkSuiteId;
      checkSuiteResponse.Message = message;
      checkSuiteResponse.CheckRuns = checkRuns;
      checkSuiteResponse.Context = suiteContext.Context;
      checkSuiteResponse.Resources = suiteContext.Resources;
      checkSuiteResponse.CompletedDate = nullable;
      return checkSuiteResponse;
    }

    public static CheckRunStatus GetCheckSuiteStatus(List<CheckRun> checkRuns, out string message)
    {
      message = string.Empty;
      if (checkRuns == null || checkRuns.Count == 0)
        return CheckRunStatus.Approved;
      CheckRunStatus mergedStatus = CheckRunStatus.None;
      foreach (CheckRun checkRun in checkRuns)
        mergedStatus |= checkRun.Status;
      return CheckSuiteResult.GetOverallCheckSuiteStatus(mergedStatus);
    }

    private static CheckRunStatus GetOverallCheckSuiteStatus(CheckRunStatus mergedStatus)
    {
      if ((mergedStatus & CheckRunStatus.Rejected) != CheckRunStatus.None)
        return CheckRunStatus.Rejected;
      if ((mergedStatus & CheckRunStatus.TimedOut) != CheckRunStatus.None)
        return CheckRunStatus.TimedOut;
      if ((mergedStatus & CheckRunStatus.Canceled) != CheckRunStatus.None)
        return CheckRunStatus.Canceled;
      if ((mergedStatus & CheckRunStatus.Running) != CheckRunStatus.None)
        return CheckRunStatus.Running;
      CheckRunStatus checkRunStatus1 = CheckRunStatus.Approved | CheckRunStatus.Bypassed;
      if ((mergedStatus & ~checkRunStatus1) == CheckRunStatus.None)
        return CheckRunStatus.Approved;
      if (mergedStatus == CheckRunStatus.Queued)
        return mergedStatus;
      CheckRunStatus checkRunStatus2 = CheckRunStatus.Approved | CheckRunStatus.Bypassed | CheckRunStatus.Deferred;
      return (mergedStatus & checkRunStatus2) == mergedStatus ? CheckRunStatus.Deferred : CheckRunStatus.Running;
    }

    public static bool isCheckSuiteCompleted(CheckRunStatus status) => (status & CheckRunStatus.Completed) == status;

    private static DateTime? GetCheckSuiteCompletedDate(
      List<CheckRun> checkRuns,
      CheckRunStatus suiteStatus)
    {
      List<CheckRun> list = checkRuns != null ? checkRuns.Where<CheckRun>((Func<CheckRun, bool>) (record => record.Status == suiteStatus && record.CompletedDate.HasValue)).OrderBy<CheckRun, DateTime?>((Func<CheckRun, DateTime?>) (record => record.CompletedDate)).ToList<CheckRun>() : (List<CheckRun>) null;
      DateTime? suiteCompletedDate = new DateTime?();
      if (suiteStatus == CheckRunStatus.Approved)
        suiteCompletedDate = (list != null ? list.LastOrDefault<CheckRun>() : (CheckRun) null) != null ? list.LastOrDefault<CheckRun>().CompletedDate : new DateTime?(DateTime.UtcNow);
      else if (suiteStatus == (CheckRunStatus.Failed & suiteStatus))
        suiteCompletedDate = (list != null ? list.FirstOrDefault<CheckRun>() : (CheckRun) null) != null ? list.FirstOrDefault<CheckRun>().CompletedDate : new DateTime?(DateTime.UtcNow);
      return suiteCompletedDate;
    }
  }
}
