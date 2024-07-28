// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Routing.BacklogConfigurationRouting
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Work.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Routing
{
  internal class BacklogConfigurationRouting : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      VssRestApiVersion initialVersion = VssRestApiVersion.v2_0;
      VssRestApiReleaseState releaseState = VssRestApiReleaseState.Released;
      areas.RegisterArea("work", "1D4F49F9-02B9-4E26-B826-2CDB6195F2A9");
      routes.MapResourceRoute(TfsApiResourceScope.Project | TfsApiResourceScope.Team, WorkWebConstants.BacklogConfigurationLocationId, "work", "backlogconfiguration", "{area}/{resource}", initialVersion, releaseState, routeName: "Work.BacklogConfiguration");
    }
  }
}
