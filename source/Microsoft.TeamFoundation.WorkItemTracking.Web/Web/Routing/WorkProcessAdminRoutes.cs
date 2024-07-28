// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Routing.WorkProcessAdminRoutes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Routing
{
  public class WorkProcessAdminRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Collection, WorkItemTrackingLocationIds.ProcessAdminBehaviors, "processAdmin", "behaviors", "work/{area}/{processId}/{resource}/{behaviorid}", VssRestApiVersion.v3_0, VssRestApiReleaseState.Released, defaults: (object) new
      {
        behaviorId = RouteParameter.Optional
      }, constraints: (object) new
      {
        processId = new GuidRouteConstraint()
      }, routeName: "processAdminbehaviors");
      HttpRouteCollection routes1 = routes;
      Guid processImportExport = WorkItemTrackingLocationIds.ProcessImportExport;
      object obj = (object) new
      {
        id = new GuidRouteConstraint()
      };
      var defaults = new{ id = Guid.Empty };
      object constraints = obj;
      VssRestApiVersion? deprecatedAtVersion = new VssRestApiVersion?();
      routes1.MapResourceRoute(TfsApiResourceScope.Collection, processImportExport, "processAdmin", "processes", "work/{area}/{resource}/{action}/{id}", VssRestApiVersion.v2_2, VssRestApiReleaseState.Released, defaults: (object) defaults, constraints: constraints, routeName: "processAdminId", deprecatedAtVersion: deprecatedAtVersion);
    }
  }
}
