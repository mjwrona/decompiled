// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Routing.AccountMyWorkRoutes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Routing
{
  public class AccountMyWorkRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, WorkItemTrackingLocationIds.AccountMyWorkDataGuid, "wit", "accountMyWork", "work/{resource}", VssRestApiVersion.v1_0, defaults: (object) new
      {
        queryOption = RouteParameter.Optional
      }, routeName: "work.accountMyWork");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, WorkItemTrackingLocationIds.WorkItemTypeColorDataGuid, "wit", "workitemTypeColor", "work/{resource}", VssRestApiVersion.v1_0, routeName: "work.workitemTypeColor");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, WorkItemTrackingLocationIds.WorkItemStateColorDataGuid, "wit", "workitemStateColor", "work/{resource}", VssRestApiVersion.v1_0, routeName: "work.workitemStateColor");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, WorkItemTrackingLocationIds.AccountMyWorkRecentActivityDataGuid, "wit", "accountMyWorkRecentActivity", "work/{resource}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, 2, routeName: "work.accountMyWorkRecentActivity");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, WorkItemTrackingLocationIds.AccountRecentMentionsDataGuid, "wit", "accountRecentMentions", "work/{resource}", VssRestApiVersion.v1_0, routeName: "work.accountRecentMentions");
    }
  }
}
