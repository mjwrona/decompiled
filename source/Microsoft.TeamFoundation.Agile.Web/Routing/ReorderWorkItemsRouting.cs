// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Routing.ReorderWorkItemsRouting
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Work.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Routing
{
  public class ReorderWorkItemsRouting : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Team, ReorderWorkItemsConstants.WorkItemsOrderBacklogLocationId, "work", "workitemsorder", "{area}/{resource}", VssRestApiVersion.v5_1, VssRestApiReleaseState.Released, routeName: "Work.ReorderBacklogWorkItems");
      routes.MapResourceRoute(TfsApiResourceScope.Team, ReorderWorkItemsConstants.WorkItemsOrderIterationLocationId, "work", "workitemsorder", "{area}/iterations/{iterationId}/{resource}", VssRestApiVersion.v5_1, VssRestApiReleaseState.Released, routeName: "Work.ReorderIterationWorkItems");
    }
  }
}
