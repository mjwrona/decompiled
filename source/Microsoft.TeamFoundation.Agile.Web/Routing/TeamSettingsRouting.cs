// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Routing.TeamSettingsRouting
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Work.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Routing
{
  public class TeamSettingsRouting : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      VssRestApiVersion initialVersion1 = VssRestApiVersion.v2_0;
      VssRestApiReleaseState releaseState = VssRestApiReleaseState.Released;
      VssRestApiVersion initialVersion2 = VssRestApiVersion.v4_1;
      areas.RegisterArea("work", "1D4F49F9-02B9-4E26-B826-2CDB6195F2A9");
      routes.MapResourceRoute(TfsApiResourceScope.Project | TfsApiResourceScope.Team, TeamSettingsApiConstants.LocationId, "work", "teamsettings", "{area}/{resource}", initialVersion1, releaseState, routeName: "Work.TeamSettings");
      routes.MapResourceRoute(TfsApiResourceScope.Project | TfsApiResourceScope.Team, TeamSettingsApiConstants.IterationsLocationId, "work", "iterations", "{area}/teamsettings/{resource}/{id}", initialVersion1, releaseState, defaults: (object) new
      {
        id = RouteParameter.Optional
      }, routeName: "Work.TeamSettings.Iterations");
      routes.MapResourceRoute(TfsApiResourceScope.Project | TfsApiResourceScope.Team, TeamSettingsApiConstants.TeamDaysOffLocationId, "work", "teamdaysoff", "{area}/teamsettings/iterations/{iterationId}/{resource}", initialVersion1, releaseState, routeName: "Work.TeamSettings.Iterations.TeamDaysOff");
      routes.MapResourceRoute(TfsApiResourceScope.Project | TfsApiResourceScope.Team, TeamSettingsApiConstants.TeamFieldValuesLocationId, "work", "teamfieldvalues", "{area}/teamsettings/{resource}", initialVersion1, releaseState, routeName: "TeamSettingsTeamFieldValues");
      routes.MapResourceRoute(TfsApiResourceScope.Project | TfsApiResourceScope.Team, TeamSettingsApiConstants.CapacityLocationId, "work", "capacities", "{area}/teamsettings/iterations/{iterationId}/{resource}/{teamMemberId}", initialVersion1, releaseState, 3, (object) new
      {
        teamMemberId = RouteParameter.Optional
      }, routeName: "Work.TeamSettings.Capacity");
      routes.MapResourceRoute(TfsApiResourceScope.Project | TfsApiResourceScope.Team, TeamSettingsApiConstants.IterationWorkItemsLocationId, "work", "workitems", "{area}/teamsettings/iterations/{iterationId}/{resource}", initialVersion2, releaseState, routeName: "Work.TeamSettings.IterationWorkItems");
    }
  }
}
