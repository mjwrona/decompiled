// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WebPlatform.Server.TfsProjectHelpers
// Assembly: Microsoft.TeamFoundation.WebPlatform.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BDF91478-A3ED-4B5B-AA51-9473C7AE6182
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WebPlatform.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WebPlatform.Utils;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.WebPlatform.Server
{
  public static class TfsProjectHelpers
  {
    public static readonly string s_AvoidProjectInformationDisclosure = "VisualStudio.WebPlatform.TfsProjectHelpers.AvoidProjectInformationDisclosure";

    public static ProjectInfo GetProjectFromName(
      IVssRequestContext tfsRequestContext,
      string projectName,
      IProjectAccessChecker projectAccessChecker = null,
      bool includeProjectHistory = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ProjectInfo project = tfsRequestContext.GetService<IProjectService>().GetProject(tfsRequestContext.Elevate(), projectName, includeProjectHistory);
      TfsProjectHelpers.CheckProjectAccess(tfsRequestContext, project, projectAccessChecker);
      return project.State == ProjectState.WellFormed ? project : throw new ProjectDoesNotExistWithNameException(projectName);
    }

    public static ProjectInfo GetProjectFromId(
      IVssRequestContext tfsRequestContext,
      Guid projectId,
      IProjectAccessChecker projectAccessChecker = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ProjectInfo project = tfsRequestContext.GetService<IProjectService>().GetProject(tfsRequestContext.Elevate(), projectId);
      TfsProjectHelpers.CheckProjectAccess(tfsRequestContext, project, projectAccessChecker);
      return project.State == ProjectState.WellFormed ? project : throw new ProjectDoesNotExistException(projectId.ToString());
    }

    public static ProjectInfo GetProject(
      IVssRequestContext tfsRequestContext,
      Uri projectUri,
      IProjectAccessChecker projectAccessChecker = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri));
      ProjectInfo project = tfsRequestContext.GetService<IProjectService>().GetProject(tfsRequestContext.Elevate(), ProjectInfo.GetProjectId(projectUri.ToString()));
      TfsProjectHelpers.CheckProjectAccess(tfsRequestContext, project, projectAccessChecker);
      return project.State == ProjectState.WellFormed ? project : throw new ProjectDoesNotExistException(projectUri.ToString());
    }

    public static ProjectInfo GetProjectFromRequestName(
      IVssRequestContext requestContext,
      string requestProjectName,
      IProjectAccessChecker accessChecker = null)
    {
      ProjectInfo projectFromRequestName;
      try
      {
        Guid result;
        if (Guid.TryParseExact(requestProjectName, "D", out result))
        {
          try
          {
            projectFromRequestName = TfsProjectHelpers.GetProjectFromId(requestContext, result, accessChecker);
          }
          catch (ProjectDoesNotExistException ex)
          {
            projectFromRequestName = TfsProjectHelpers.GetProjectFromName(requestContext, requestProjectName, accessChecker);
          }
        }
        else
          projectFromRequestName = TfsProjectHelpers.GetProjectFromName(requestContext, requestProjectName, accessChecker, true);
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        throw new HttpException(404, ex.Message, (Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        if (TfsProjectHelpers.IsAvoidProjectInformationDisclosureFeaturedEnabled(requestContext) && !OrganizationTakeoverHelpers.CanCurrentUserTakeOverOrg(requestContext))
        {
          Policy<bool> policy = requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext, "Policy.AllowRequestAccessToken", true);
          if (policy != null && !policy.EffectiveValue)
          {
            requestContext.TraceAlways(90002101, TraceLevel.Info, nameof (TfsProjectHelpers), nameof (GetProjectFromRequestName), "UnauthorizedAccessException returns 404");
            ProjectDoesNotExistWithNameException innerException = new ProjectDoesNotExistWithNameException(requestProjectName);
            throw new HttpException(404, innerException.Message, (Exception) innerException);
          }
        }
        requestContext.TraceAlways(90002102, TraceLevel.Info, nameof (TfsProjectHelpers), nameof (GetProjectFromRequestName), "UnauthorizedAccessException returns 401");
        throw new HttpException(401, ex.Message, (Exception) ex);
      }
      catch (Exception ex)
      {
        if (string.Equals(ex.GetType().Name, "ProjectDoesNotExistWithNameException", StringComparison.OrdinalIgnoreCase))
          throw new HttpException(404, ex.Message, ex);
        throw;
      }
      return projectFromRequestName;
    }

    public static void PopulateProcessTemplateProperties(
      this ProjectInfo projectInfo,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(projectInfo, nameof (projectInfo));
      projectInfo.PopulateProperties(requestContext, ProcessTemplateIdPropertyNames.CurrentProcessTemplateId, ProcessTemplateIdPropertyNames.ProcessTemplateType);
    }

    private static void CheckProjectAccess(
      IVssRequestContext tfsRequestContext,
      ProjectInfo project,
      IProjectAccessChecker projectAccessChecker)
    {
      if (projectAccessChecker == null)
        projectAccessChecker = (IProjectAccessChecker) new ProjectAccessChecker();
      projectAccessChecker.CheckProjectAccess(tfsRequestContext, project);
    }

    public static bool IsAvoidProjectInformationDisclosureFeaturedEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled(TfsProjectHelpers.s_AvoidProjectInformationDisclosure);
    }
  }
}
