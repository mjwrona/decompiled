// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Routing.TaskboardColumnRoute
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Routing
{
  public class TaskboardColumnRoute : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Team, new Guid("C6815DBE-8E7E-4FFE-9A79-E83EE712AA92"), "work", "taskboardColumns", "{area}/{resource}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, routeName: "Agile.TaskboardColumns");
      routes.MapResourceRoute(TfsApiResourceScope.Team, new Guid("1BE23C36-8872-4ABC-B57D-402CD6C669D9"), "work", "taskboardWorkItems", "{area}/{resource}/{iterationId}/{workItemId}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, defaults: (object) new
      {
        workItemId = RouteParameter.Optional
      }, routeName: "Agile.TaskboardWorkItems");
    }
  }
}
