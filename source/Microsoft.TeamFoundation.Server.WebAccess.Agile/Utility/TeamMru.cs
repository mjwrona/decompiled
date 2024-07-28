// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.TeamMru
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  public class TeamMru
  {
    private const string Delimiter = ";";
    private const string MappingTeamMRU = "MappingTeamMRU";

    public virtual IEnumerable<WebApiTeam> GetMruTeams(
      IVssRequestContext requestContext,
      string projectUri,
      WebApiTeam team)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      List<Guid> teamIds = this.GetTeamIds(requestContext, projectUri, team);
      return !teamIds.Any<Guid>() ? Enumerable.Empty<WebApiTeam>() : (IEnumerable<WebApiTeam>) requestContext.GetService<ITeamService>().GetTeamsByGuid(requestContext, (IEnumerable<Guid>) teamIds);
    }

    public virtual void AddTeam(
      IVssRequestContext requestContext,
      string projectUri,
      WebApiTeam team,
      Guid teamId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      List<Guid> teamIds = this.GetTeamIds(requestContext, projectUri, team);
      if (teamIds.Contains(teamId))
        teamIds.Remove(teamId);
      teamIds.Insert(0, teamId);
      this.WriteMru(requestContext, projectUri, team, teamIds.Take<Guid>(5));
    }

    private List<Guid> GetTeamIds(
      IVssRequestContext requestContext,
      string projectUri,
      WebApiTeam team)
    {
      string[] strArray = this.ReadMru(requestContext, projectUri, team).Split(new string[1]
      {
        ";"
      }, StringSplitOptions.RemoveEmptyEntries);
      List<Guid> teamIds = new List<Guid>();
      foreach (string input in strArray)
      {
        Guid result;
        if (Guid.TryParse(input, out result))
          teamIds.Add(result);
      }
      return teamIds;
    }

    public virtual string ReadMru(
      IVssRequestContext requestContext,
      string projectUri,
      WebApiTeam team)
    {
      Guid projectId = new Guid(LinkingUtilities.DecodeUri(projectUri).ToolSpecificId);
      using (ISettingsProvider webSettings = WebSettings.GetWebSettings(requestContext, projectId, team, WebSettingsScope.UserAndTeam))
        return webSettings.GetSetting<string>("MappingTeamMRU", string.Empty);
    }

    public virtual void WriteMru(
      IVssRequestContext requestContext,
      string projectUri,
      WebApiTeam team,
      IEnumerable<Guid> teamIds)
    {
      Guid projectId = new Guid(LinkingUtilities.DecodeUri(projectUri).ToolSpecificId);
      string str = string.Join<Guid>(";", teamIds);
      using (ISettingsProvider webSettings = WebSettings.GetWebSettings(requestContext, projectId, team, WebSettingsScope.UserAndTeam))
        webSettings.SetSetting<string>("MappingTeamMRU", str);
    }
  }
}
