// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.PipelineSettingsSecurityProvider
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class PipelineSettingsSecurityProvider
  {
    public static readonly Guid AdministrationNamespaceId = new Guid("302ACACA-B667-436d-A946-87133492041C");
    public static readonly Guid BuildNamespaceId = new Guid("33344D9C-FC72-4d6f-ABA5-FA317101A7E9");
    public static readonly string PrivilegesToken = "BuildPrivileges";
    public static readonly int ViewBuildResources = 1;
    public static readonly int ManageBuildResources = 2;
    public static readonly int ManagePipelinePolicies = 16;

    public static void CheckViewProjectPermission(IVssRequestContext requestContext, Guid projectId)
    {
      Action throwAccessDeniedException = (Action) (() =>
      {
        throw new UnauthorizedAccessException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.AccessDeniedMessage, (object) requestContext.GetUserIdentity().DisplayName));
      });
      PipelineSettingsSecurityProvider.CheckPermission(requestContext, PipelineSettingsSecurityProvider.BuildNamespaceId, projectId.ToString(), PipelineSettingsSecurityProvider.ViewBuildResources, true, throwAccessDeniedException);
    }

    public static bool HasEditProjectPermission(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PipelineSettingsSecurityProvider.BuildNamespaceId).HasPermission(requestContext, projectId.ToString(), PipelineSettingsSecurityProvider.ManageBuildResources);

    public static void CheckViewCollectionPermission(IVssRequestContext requestContext)
    {
      Action throwAccessDeniedException = (Action) (() =>
      {
        throw new UnauthorizedAccessException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.AccessDeniedMessage, (object) requestContext.GetUserIdentity().DisplayName));
      });
      PipelineSettingsSecurityProvider.CheckPermission(requestContext, PipelineSettingsSecurityProvider.AdministrationNamespaceId, PipelineSettingsSecurityProvider.PrivilegesToken, PipelineSettingsSecurityProvider.ViewBuildResources, true, throwAccessDeniedException);
    }

    public static bool HasManageCollectionPermission(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PipelineSettingsSecurityProvider.AdministrationNamespaceId).HasPermission(requestContext, PipelineSettingsSecurityProvider.PrivilegesToken, PipelineSettingsSecurityProvider.ManagePipelinePolicies);

    private static void CheckPermission(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators,
      Action throwAccessDeniedException)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId);
      if (securityNamespace.HasPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators) || securityNamespace.PollForRequestLocalInvalidation(requestContext) && securityNamespace.HasPermission(requestContext, token, requestedPermissions, alwaysAllowAdministrators))
        return;
      throwAccessDeniedException();
    }
  }
}
