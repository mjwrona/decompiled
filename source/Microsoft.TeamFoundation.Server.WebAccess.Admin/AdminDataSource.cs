// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminDataSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class AdminDataSource
  {
    public static object GetJobProgress(IVssRequestContext tfsRequestContext, Guid jobId)
    {
      ArgumentUtility.CheckForEmptyGuid(jobId, nameof (jobId));
      ProcessUpdateProgressModel updateProgressModel = tfsRequestContext.GetService<IProcessAdminService>().MonitorUpdateProgress(tfsRequestContext, jobId);
      ProcessAdminService.ProcessUpdateProgressState updateProgressState = ProcessAdminService.ProcessUpdateProgressState.Created;
      object jobProgress;
      if (updateProgressModel != null)
      {
        if (updateProgressModel.IsSuccessful)
          updateProgressState = ProcessAdminService.ProcessUpdateProgressState.Success;
        else if (!updateProgressModel.IsSuccessful && updateProgressModel.RemainingRetries > 0)
          updateProgressState = ProcessAdminService.ProcessUpdateProgressState.InProgress;
        else if (!updateProgressModel.IsSuccessful && updateProgressModel.RemainingRetries == 0)
          updateProgressState = ProcessAdminService.ProcessUpdateProgressState.Failure;
        int num1 = updateProgressModel.TotalProjectsCount <= 0 ? 0 : updateProgressModel.ProcessedProjectsCount * 100 / updateProgressModel.TotalProjectsCount;
        int num2 = (int) updateProgressState;
        string str;
        switch (updateProgressState)
        {
          case ProcessAdminService.ProcessUpdateProgressState.InProgress:
            str = string.Format(AdminResources.PromoteProcessStarted, (object) updateProgressModel.ProcessedProjectsCount, (object) updateProgressModel.TotalProjectsCount);
            break;
          case ProcessAdminService.ProcessUpdateProgressState.Failure:
            str = AdminDataSource.GetJobResultMessage(tfsRequestContext, jobId);
            break;
          default:
            str = string.Empty;
            break;
        }
        jobProgress = (object) new
        {
          PercentComplete = num1,
          State = (ProcessAdminService.ProcessUpdateProgressState) num2,
          ProgressText = str
        };
      }
      else
        jobProgress = (object) new
        {
          PercentComplete = 0,
          State = ProcessAdminService.ProcessUpdateProgressState.Failure,
          ProgressText = string.Empty
        };
      return jobProgress;
    }

    public static string GetProcessFieldUsages(
      IVssRequestContext tfsRequestContext,
      Guid processTypeId)
    {
      return tfsRequestContext.GetService<IProcessAdminService>().GetProcessFieldUsages(tfsRequestContext, processTypeId).ToJson();
    }

    private static string GetJobResultMessage(IVssRequestContext requestContext, Guid jobId) => requestContext.GetService<ITeamFoundationJobService>().QueryLatestJobHistory(requestContext, jobId).ResultMessage;
  }
}
