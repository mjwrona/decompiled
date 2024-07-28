// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Routing.WorkItemPickListRoutes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.Server.Core;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Routing
{
  public class WorkItemPickListRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.SpecificPickList, "processDefinitions", "lists", "work/{area}/{resource}/{listId}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, constraints: (object) new
      {
        listId = new GuidRouteConstraint()
      }, routeName: "processDefinitionsspecificPickList");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessDefinitionPickLists, "processDefinitions", "lists", "work/{area}/{resource}", VssRestApiVersion.v2_1, VssRestApiReleaseState.Released, routeName: "processDefinitionslists");
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessPickList, "processes", "lists", "work/{area}/{resource}/{listId}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released, defaults: (object) new
      {
        listId = RouteParameter.Optional
      }, routeName: "processeslists");
    }
  }
}
