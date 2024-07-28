// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.Server.SecurityRestApiResourceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Security.Server
{
  public sealed class SecurityRestApiResourceProvider : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      HttpRouteCollection routes1 = routes;
      Guid securityPermissions = LocationResourceIds.SecurityPermissions;
      object obj = (object) new{ permissions = "[0-9]*" };
      Version version1 = VssRestApiVersion.v1_0.ToVersion();
      Version version2 = VssRestApiVersion.v1_0.ToVersion();
      object constraints = obj;
      routes1.MapLegacyResourceRoute(TeamFoundationHostType.All, securityPermissions, "Security", "Permissions", "{resource}/{securityNamespaceId}/{permissions}", version1, defaultApiVersion: version2, maxResourceVersion: 2, constraints: constraints);
      routes.MapLegacyResourceRoute(TeamFoundationHostType.All, LocationResourceIds.SecurityAccessControlEntries, "Security", "AccessControlEntries", "{resource}/{securityNamespaceId}", VssRestApiVersion.v1_0.ToVersion(), defaultApiVersion: VssRestApiVersion.v1_0.ToVersion());
      routes.MapLegacyResourceRoute(TeamFoundationHostType.All, LocationResourceIds.SecurityAccessControlLists, "Security", "AccessControlLists", "{resource}/{securityNamespaceId}", VssRestApiVersion.v1_0.ToVersion(), defaultApiVersion: VssRestApiVersion.v1_0.ToVersion());
      routes.MapLegacyResourceRoute(TeamFoundationHostType.All, LocationResourceIds.SecurityNamespaces, "Security", "SecurityNamespaces", "{resource}/{securityNamespaceId}", VssRestApiVersion.v1_0.ToVersion(), defaultApiVersion: VssRestApiVersion.v1_0.ToVersion(), defaults: (object) new
      {
        securityNamespaceId = "00000000-0000-0000-0000-000000000000"
      });
      routes.MapResourceRoute(TeamFoundationHostType.All, LocationResourceIds.SecurityPermissionEvaluationBatch, "Security", "PermissionEvaluationBatch", "{area}/{resource}", VssRestApiVersion.v3_0, VssRestApiReleaseState.Released);
    }
  }
}
