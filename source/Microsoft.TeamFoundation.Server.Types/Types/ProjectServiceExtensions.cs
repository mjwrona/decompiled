// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectServiceExtensions
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Microsoft.TeamFoundation.Server.Types
{
  public static class ProjectServiceExtensions
  {
    private static readonly string s_area = "Project";
    private static readonly string s_layer = nameof (ProjectServiceExtensions);

    public static void CheckProjectExists(
      this IProjectService projectService,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      projectService.GetProject(requestContext.Elevate(), projectId);
    }

    public static bool HasProjectPermission(
      this IProjectService service,
      IVssRequestContext requestContext,
      string projectUri,
      int permission,
      bool alwaysAllowAdministrators = true)
    {
      return service.DetermineProjectPermission(requestContext, projectUri, permission, alwaysAllowAdministrators: alwaysAllowAdministrators);
    }

    public static void CheckProjectPermission(
      this IProjectService service,
      IVssRequestContext requestContext,
      string projectUri,
      int permission,
      bool alwaysAllowAdministrators = true)
    {
      service.DetermineProjectPermission(requestContext, projectUri, permission, true, alwaysAllowAdministrators);
    }

    internal static bool DetermineProjectPermission(
      this IProjectService service,
      IVssRequestContext requestContext,
      string projectUri,
      int permission,
      bool throwOnNoAccess = false,
      bool alwaysAllowAdministrators = true)
    {
      requestContext.TraceEnter(5500220, ProjectServiceExtensions.s_area, ProjectServiceExtensions.s_layer, nameof (DetermineProjectPermission));
      try
      {
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
        if (securityNamespace == null)
          throw new ConfigurationErrorsException("Could not find the team project security namespace. Please register a remote team project security namespace pointer for this service host.");
        string securityToken = service.GetSecurityToken(requestContext, projectUri);
        bool projectPermission;
        if (throwOnNoAccess)
        {
          try
          {
            securityNamespace.CheckPermission(requestContext, securityToken, permission, alwaysAllowAdministrators);
            projectPermission = true;
          }
          catch (Exception ex) when (
          {
            // ISSUE: unable to correctly present filter
            int num;
            switch (ex)
            {
              case AccessCheckException _:
              case UnauthorizedAccessException _:
                num = ProjectServiceExtensions.ShouldThrowDoesNotExistException(requestContext, permission) ? 1 : 0;
                break;
              default:
                num = 0;
                break;
            }
            if ((uint) num > 0U)
            {
              SuccessfulFiltering;
            }
            else
              throw;
          }
          )
          {
            throw new ProjectDoesNotExistException(projectUri);
          }
        }
        else
          projectPermission = securityNamespace.HasPermission(requestContext, securityToken, permission, alwaysAllowAdministrators);
        return projectPermission;
      }
      finally
      {
        requestContext.TraceLeave(5500221, ProjectServiceExtensions.s_area, ProjectServiceExtensions.s_layer, nameof (DetermineProjectPermission));
      }
    }

    public static IEnumerable<ProjectProperty> GetProjectProperties(
      this IProjectService projectService,
      IVssRequestContext requestContext,
      Guid projectId,
      params string[] projectPropertyFilters)
    {
      return projectService.GetProjectProperties(requestContext, projectId, (IEnumerable<string>) projectPropertyFilters);
    }

    public static IEnumerable<ProjectProperties> GetProjectsProperties(
      this IProjectService projectService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      params string[] projectPropertyFilters)
    {
      return projectService.GetProjectsProperties(requestContext, projectIds, (IEnumerable<string>) projectPropertyFilters);
    }

    public static void SetProjectProperties(
      this IProjectService projectService,
      IVssRequestContext requestContext,
      Guid projectId,
      params ProjectProperty[] projectProperties)
    {
      projectService.SetProjectProperties(requestContext, projectId, (IEnumerable<ProjectProperty>) projectProperties);
    }

    private static bool ShouldThrowDoesNotExistException(
      IVssRequestContext requestContext,
      int permission)
    {
      return permission == TeamProjectPermissions.GenericRead;
    }
  }
}
