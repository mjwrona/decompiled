// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.AdminEngagement.WebApi.RestApiResourceProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.Server.Core;
using System.Web.Http;

namespace Microsoft.TeamFoundation.AdminEngagement.WebApi
{
  public sealed class RestApiResourceProvider : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      areas.RegisterArea("OrganizationSettings", ResourceIds.OrganizationSettingsAreaId);
      areas.RegisterArea("ProjectSettings", ResourceIds.ProjectSettingsAreaId);
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, ResourceIds.DisconnectedUsersResourceId, "OrganizationSettings", "DisconnectedUser", "{area}/{resource}", VssRestApiVersion.v5_1);
      routes.MapResourceRoute(TfsApiResourceScope.Project, ResourceIds.ProjectSettingsResourceId, "ProjectSettings", "Project", "{area}/{resource}", VssRestApiVersion.v5_0, defaults: (object) new
      {
        id = RouteParameter.Optional
      }, routeName: "Project");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, ResourceIds.PermissionsResourceId, "OrganizationSettings", "Permissions", "{area}/{resource}/{subjectDescriptor}", VssRestApiVersion.v5_1, constraints: (object) new
      {
        subjectDescriptor = new SubjectDescriptorRouteConstraint()
      });
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, ResourceIds.PermissionTraceResourceId, "OrganizationSettings", "PermissionTrace", "{area}/{resource}/{subjectDescriptor}", VssRestApiVersion.v5_1, constraints: (object) new
      {
        subjectDescriptor = new SubjectDescriptorRouteConstraint()
      });
    }
  }
}
