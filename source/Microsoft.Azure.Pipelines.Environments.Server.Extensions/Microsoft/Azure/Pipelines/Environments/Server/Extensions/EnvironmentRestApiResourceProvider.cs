// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Extensions.EnvironmentRestApiResourceProvider
// Assembly: Microsoft.Azure.Pipelines.Environments.Server.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3EFE35-87EB-4897-9A22-A06F71604275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Environments.Server.Extensions.dll

using Microsoft.Azure.Pipelines.Environments.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Environments.Server.Extensions
{
  public sealed class EnvironmentRestApiResourceProvider : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      areas.RegisterArea("environments", "0A833654-DF2A-437E-8253-FE6B63B82035");
      routes.MapResourceRoute(TfsApiResourceScope.Project, EnvironmentResourceIds.EnvironmentsLocationId, "environments", "environments", "pipelines/{resource}/{environmentId}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, defaults: (object) new
      {
        environmentId = RouteParameter.Optional
      });
      routes.MapResourceRoute(TfsApiResourceScope.Project, EnvironmentResourceIds.DeploymentExecutionHistoryLocationId, "environments", "environmentdeploymentrecords", "pipelines/environments/{environmentId}/{resource}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, constraints: (object) new
      {
        environmentId = "[0-9]*"
      });
      HttpRouteCollection routes1 = routes;
      Guid resourcesLocationId = EnvironmentResourceIds.KubernetesResourcesLocationId;
      object obj1 = (object) new
      {
        environmentId = "[0-9]*",
        resourceId = "[0-9]*"
      };
      var defaults1 = new
      {
        resourceId = RouteParameter.Optional
      };
      object constraints1 = obj1;
      VssRestApiVersion? deprecatedAtVersion1 = new VssRestApiVersion?();
      routes1.MapResourceRoute(TfsApiResourceScope.Project, resourcesLocationId, "environments", "kubernetes", "pipelines/environments/{environmentId}/providers/{resource}/{resourceId}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, 2, (object) defaults1, constraints1, deprecatedAtVersion: deprecatedAtVersion1);
      HttpRouteCollection routes2 = routes;
      Guid groupsLocationId = EnvironmentResourceIds.VirtualMachineGroupsLocationId;
      object obj2 = (object) new
      {
        environmentId = "[0-9]*",
        resourceId = "[0-9]*"
      };
      var defaults2 = new
      {
        resourceId = RouteParameter.Optional
      };
      object constraints2 = obj2;
      VssRestApiVersion? deprecatedAtVersion2 = new VssRestApiVersion?();
      routes2.MapResourceRoute(TfsApiResourceScope.Project, groupsLocationId, "environments", "virtualmachinegroups", "pipelines/environments/{environmentId}/providers/{resource}/{resourceId}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, defaults: (object) defaults2, constraints: constraints2, deprecatedAtVersion: deprecatedAtVersion2);
      routes.MapResourceRoute(TfsApiResourceScope.Project, EnvironmentResourceIds.EnvironmentAccessToken, "environments", "environmentaccesstoken", "pipelines/{area}/{resource}/{environmentId}", VssRestApiVersion.v6_0, VssRestApiReleaseState.Released, constraints: (object) new
      {
        environmentId = "[0-9]*"
      });
      routes.MapResourceRoute(TfsApiResourceScope.Project, EnvironmentResourceIds.VirtualMachinesLocationId, "environments", "virtualmachines", "pipelines/environments/{environmentId}/providers/virtualmachinegroups/{resourceId}/{resource}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, constraints: (object) new
      {
        environmentId = "[0-9]*",
        resourceId = "[0-9]*"
      });
      HttpRouteCollection routes3 = routes;
      Guid resourceLocationId = EnvironmentResourceIds.VirtualMachinesResourceLocationId;
      object obj3 = (object) new
      {
        environmentId = "[0-9]*",
        resourceId = "[0-9]*"
      };
      var defaults3 = new
      {
        resourceId = RouteParameter.Optional
      };
      object constraints3 = obj3;
      VssRestApiVersion? deprecatedAtVersion3 = new VssRestApiVersion?();
      routes3.MapResourceRoute(TfsApiResourceScope.Project, resourceLocationId, "environments", "vmresource", "pipelines/environments/{environmentId}/providers/virtualmachines/{resourceId}", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, defaults: (object) defaults3, constraints: constraints3, deprecatedAtVersion: deprecatedAtVersion3);
      routes.MapResourceRoute(TfsApiResourceScope.Project, EnvironmentResourceIds.EnvironmentPoolLocationId, "environments", "pool", "pipelines/environments/{environmentId}/providers/virtualmachines/pool", VssRestApiVersion.v5_2, VssRestApiReleaseState.Released, constraints: (object) new
      {
        environmentId = "[0-9]*"
      });
    }
  }
}
