// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.TeamSettingsTeamDaysOffApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "teamdaysoff")]
  public class TeamSettingsTeamDaysOffApiController : TeamSettingsApiControllerBase
  {
    [HttpGet]
    [ClientExample("GET__work_teamsettings_iterations__iterationId__teamdaysoff.json", "Get team's days off for an iteration", null, null)]
    public TeamSettingsDaysOff GetTeamDaysOff(Guid iterationId)
    {
      this.TfsRequestContext.TraceEnter(290131, "AgileService", "AgileService", nameof (GetTeamDaysOff));
      try
      {
        TeamSettingsDaysOff teamDaysOff = new TeamSettingsDaysOff()
        {
          DaysOff = (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.DateRange>) new List<Microsoft.TeamFoundation.Work.WebApi.DateRange>()
        };
        teamDaysOff.DaysOff = new ConversionHelper().ConvertToWebApiDateRange(this.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamIterationDaysOff(this.TfsRequestContext, this.Team, iterationId));
        string resourceUriString1 = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.TeamDaysOffLocationId, this.ProjectId, this.TeamId, (object) new
        {
          iterationId = iterationId
        });
        string resourceUriString2 = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.IterationsLocationId, this.ProjectId, this.TeamId, (object) new
        {
          id = iterationId
        });
        teamDaysOff.Links = this.GetReferenceLinks(resourceUriString1, TeamSettingsApiControllerBase.CommonUrlLink.TeamSettings | TeamSettingsApiControllerBase.CommonUrlLink.Iterations);
        teamDaysOff.Links.AddLink("teamIteration", resourceUriString2);
        teamDaysOff.Url = resourceUriString1;
        return teamDaysOff;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290132, "AgileService", "AgileService", nameof (GetTeamDaysOff));
      }
    }

    [HttpPatch]
    [ClientExample("PATCH__work_teamsettings_iterations__iterationId__teamdaysoff.json", "Set a team's days off for an iteration. Example 1", null, null)]
    [ClientExample("PATCH__work_teamsettings_iterations__iterationId__teamdaysoff2.json", "Set a team's days off for an iteration. Example 2", null, null)]
    public TeamSettingsDaysOff UpdateTeamDaysOff(
      Guid iterationId,
      [FromBody] TeamSettingsDaysOffPatch daysOffPatch)
    {
      this.TfsRequestContext.TraceEnter(290133, "AgileService", "AgileService", "PatchTeamDaysOff");
      try
      {
        ArgumentUtility.CheckForNull<TeamSettingsDaysOffPatch>(daysOffPatch, nameof (daysOffPatch));
        if (daysOffPatch.DaysOff == null)
          throw new ArgumentNullException("daysOff");
        this.TfsRequestContext.GetService<ITeamConfigurationService>().SetTeamIterationDaysOff(this.TfsRequestContext, this.ProjectId, this.Team, iterationId, new ConversionHelper().ConvertToServerDateRange(daysOffPatch.DaysOff));
        return this.GetTeamDaysOff(iterationId);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290134, "AgileService", "AgileService", "GetTeamDaysOff");
      }
    }
  }
}
