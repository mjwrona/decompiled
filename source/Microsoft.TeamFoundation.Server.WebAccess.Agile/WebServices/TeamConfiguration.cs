// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.TeamConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class TeamConfiguration
  {
    internal TeamConfiguration()
    {
    }

    internal TeamConfiguration(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ITeamSettings teamSettings,
      string teamField,
      bool isDefaultTeam)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<ITeamSettings>(teamSettings, nameof (teamSettings));
      this.ProjectUri = Microsoft.TeamFoundation.Core.WebApi.ProjectInfo.GetProjectUri(team.ProjectId);
      this.TeamId = team.Id;
      this.TeamName = team.Name;
      this.IsDefaultTeam = isDefaultTeam;
      this.TeamSettings = new TeamSettings(requestContext, teamSettings, teamField);
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ProjectUri { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid TeamId { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string TeamName { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public TeamSettings TeamSettings { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool IsDefaultTeam { get; set; }
  }
}
