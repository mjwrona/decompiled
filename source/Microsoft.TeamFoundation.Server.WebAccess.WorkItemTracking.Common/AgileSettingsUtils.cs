// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.AgileSettingsUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class AgileSettingsUtils
  {
    public static bool DeleteSettingsForProject(
      IVssRequestContext requestContext,
      string projectUri)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      bool flag = true;
      try
      {
        requestContext.GetService<ProjectConfigurationService>().DeleteProjectSettings(requestContext, projectUri);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(240178, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
        flag = false;
      }
      try
      {
        requestContext.GetService<TeamConfigurationService>().DeleteProjectTeamSettings(requestContext, ProjectInfo.GetProjectId(projectUri));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(240188, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
        flag = false;
      }
      try
      {
        requestContext.GetService<BoardService>().DeleteProjectBoards(requestContext, projectUri);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(240312, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
        flag = false;
      }
      return flag;
    }

    public static Microsoft.TeamFoundation.Work.WebApi.BugsBehavior Convert(
      BugsBehavior bugsBehavior)
    {
      if (bugsBehavior == BugsBehavior.AsRequirements)
        return Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.AsRequirements;
      return bugsBehavior == BugsBehavior.AsTasks ? Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.AsTasks : Microsoft.TeamFoundation.Work.WebApi.BugsBehavior.Off;
    }
  }
}
