// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Routing.ScaledAgileRouting
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Work.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Routing
{
  public class ScaledAgileRouting : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      VssRestApiVersion initialVersion = VssRestApiVersion.v3_0;
      VssRestApiReleaseState releaseState = VssRestApiReleaseState.Released;
      routes.MapResourceRoute(TfsApiResourceScope.Project, ScaledAgileApiConstants.DeliveryTimlineLocationId, "work", "deliverytimeline", "{area}/plans/{id}/{resource}", initialVersion, releaseState, defaults: (object) new
      {
        id = RouteParameter.Optional
      }, routeName: "deliverytimeline");
      routes.MapResourceRoute(TfsApiResourceScope.Project, ScaledAgileApiConstants.PlansLocationId, "work", "plans", "{area}/{resource}/{id}", initialVersion, releaseState, defaults: (object) new
      {
        id = RouteParameter.Optional
      }, routeName: "plans");
    }
  }
}
