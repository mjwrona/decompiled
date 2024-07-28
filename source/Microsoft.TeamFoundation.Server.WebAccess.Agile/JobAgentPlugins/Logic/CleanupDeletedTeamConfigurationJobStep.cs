// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic.CleanupDeletedTeamConfigurationJobStep
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic
{
  public class CleanupDeletedTeamConfigurationJobStep : BaseJobStep
  {
    public JobStepResult Execute(IVssRequestContext requestContext)
    {
      JobStepResult jobStepResult = new JobStepResult(nameof (CleanupDeletedTeamConfigurationJobStep));
      jobStepResult.StartTimer();
      string str1 = "Unknown";
      string str2 = "Unknown";
      try
      {
        if (requestContext.ServiceHost != null && requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
        {
          str1 = requestContext.ServiceHost.InstanceId.ToString();
          str2 = requestContext.ServiceHost.Name;
        }
        using (this.Trace(requestContext, 290946, 290947, nameof (Execute)))
          requestContext.GetService<TeamConfigurationService>().CleanupDeletedTeamSettings(requestContext);
        jobStepResult.Message = "Completed Successfully on collection " + str1 + "(" + str2 + ").";
        jobStepResult.ExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290004, TraceLevel.Warning, "Agile", TfsTraceLayers.Framework, ex);
        jobStepResult.Message = string.Format("Agile artifact cleanup job failed on collection {0}({1}) with exception: {2}", (object) str1, (object) str2, (object) ex);
        jobStepResult.ExecutionResult = TeamFoundationJobExecutionResult.Failed;
      }
      finally
      {
        jobStepResult.StopTimer();
      }
      return jobStepResult;
    }
  }
}
