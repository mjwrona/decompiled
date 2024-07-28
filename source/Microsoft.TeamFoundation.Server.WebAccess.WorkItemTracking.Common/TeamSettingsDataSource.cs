// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamSettingsDataSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class TeamSettingsDataSource
  {
    public static TeamWITSettingsModel TeamSettings(
      IVssRequestContext tfsRequestContext,
      ProjectInfo projectInfo)
    {
      TeamWITSettingsModel witSettingsModel;
      try
      {
        IWebTeamContext webTeamContext = tfsRequestContext.GetWebTeamContext();
        witSettingsModel = TeamWITSettingsModel.CreateTeamWITSettingsModel(tfsRequestContext, projectInfo, webTeamContext.Team, tfsRequestContext.GetCollectionTimeZone());
      }
      catch (ProjectSettingsException ex)
      {
        witSettingsModel = new TeamWITSettingsModel();
        tfsRequestContext.TraceException(290233, "WebAccess.TeamSettings", "DataSource", (Exception) ex);
      }
      catch (InvalidTeamSettingsException ex)
      {
        witSettingsModel = new TeamWITSettingsModel();
        tfsRequestContext.TraceException(290232, "WebAccess.TeamSettings", "DataSource", (Exception) ex);
      }
      return witSettingsModel;
    }
  }
}
