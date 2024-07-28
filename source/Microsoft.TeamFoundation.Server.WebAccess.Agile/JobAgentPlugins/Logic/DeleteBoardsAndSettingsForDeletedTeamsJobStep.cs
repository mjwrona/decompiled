// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic.DeleteBoardsAndSettingsForDeletedTeamsJobStep
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.JobAgentPlugins.Logic
{
  public class DeleteBoardsAndSettingsForDeletedTeamsJobStep : BaseJobStep
  {
    public JobStepResult Execute(IVssRequestContext requestContext)
    {
      JobStepResult jobStepResult = new JobStepResult(nameof (DeleteBoardsAndSettingsForDeletedTeamsJobStep));
      jobStepResult.StartTimer();
      string str1 = "Unknown";
      string str2 = "Unknown";
      IList<TeamFoundationIdentity> source = (IList<TeamFoundationIdentity>) null;
      try
      {
        if (requestContext.ServiceHost != null && requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
        {
          str1 = requestContext.ServiceHost.InstanceId.ToString();
          str2 = requestContext.ServiceHost.Name;
        }
        TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
        using (this.Trace(requestContext, 290766, 290767, nameof (Execute)))
          source = service.ListAllApplicationGroups(requestContext, new Guid[1]
          {
            requestContext.ServiceHost.InstanceId
          }, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
          {
            TeamConstants.TeamPropertyName
          }, true);
        HashSet<string> projectUriHashSet = requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed).Select<Microsoft.TeamFoundation.Core.WebApi.ProjectInfo, string>((Func<Microsoft.TeamFoundation.Core.WebApi.ProjectInfo, string>) (p => p.Uri)).ToHashSet<string>();
        Guid[] array = source.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (g => projectUriHashSet.Contains(g.GetAttribute("Domain", (string) null)))).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (t => !t.IsActive)).Select<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (t => t.TeamFoundationId)).ToArray<Guid>();
        if (((IEnumerable<Guid>) array).Any<Guid>())
        {
          using (this.Trace(requestContext, 290768, 290769, nameof (Execute)))
            requestContext.GetService<TeamConfigurationService>().DeleteTeamSettings(requestContext.Elevate(), (IEnumerable<Guid>) array);
          using (this.Trace(requestContext, 290770, 290771, nameof (Execute)))
            requestContext.GetService<BoardService>().DeleteTeamBoards(requestContext, (IEnumerable<Guid>) array);
        }
        jobStepResult.Message = "Completed Successfully on collection " + str1 + "(" + str2 + ").";
        jobStepResult.ExecutionResult = TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290004, TraceLevel.Warning, "Agile", TfsTraceLayers.Framework, ex);
        jobStepResult.Message = "Agile artifact cleanup job failed on collection " + str1 + "(" + str2 + ") with exception: " + ex.ToString();
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
