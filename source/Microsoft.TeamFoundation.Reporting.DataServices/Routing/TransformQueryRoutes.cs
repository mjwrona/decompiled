// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Routing.TransformQueryRoutes
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Routing
{
  public class TransformQueryRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Project, ReportingLocationIds.TransformQueryV2, "Reporting", "TransformQuery", "{area}/{resource}/{scope}", VssRestApiVersion.v1_0, defaults: (object) new
      {
        scope = FeatureProviderScopes.WorkItemQueries
      }, routeName: "TransformQuery");
    }
  }
}
