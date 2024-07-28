// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ResourceUsage.Server.ResourceUsageRoutes
// Assembly: Microsoft.TeamFoundation.ResourceUsage.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55568389-A340-4F60-8DD1-887E0E3F1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ResourceUsage.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Web.Http;

namespace Microsoft.TeamFoundation.ResourceUsage.Server
{
  public class ResourceUsageRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      areas.RegisterArea("ResourceUsage", ResourceUsageConstants.AreaId);
      routes.MapResourceRoute(TfsApiResourceScope.Collection, LocationIds.TeamProjectCollectionLimitLocationId, "ResourceUsage", "TeamProjectCollection", "{area}", VssRestApiVersion.v7_1, routeName: "TeamProjectCollection", deprecatedAtVersion: new VssRestApiVersion?(VssRestApiVersion.v7_1));
      routes.MapResourceRoute(TfsApiResourceScope.Project, LocationIds.ProjectLimitLocationId, "ResourceUsage", "Project", "{area}", VssRestApiVersion.v7_1, routeName: "Project", deprecatedAtVersion: new VssRestApiVersion?(VssRestApiVersion.v7_1));
    }
  }
}
