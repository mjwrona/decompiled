// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security.ReleaseManagementSecurityExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security
{
  public static class ReleaseManagementSecurityExtensions
  {
    public const string SecurityTokenCacheKey = "releaseManagementSecurityTokenKey";
    public const string SecurityPermissionCacheKey = "releaseManagementSecurityPermissionKey";

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed")]
    public static bool HasPermission(
      this IVssRequestContext requestContext,
      string securityToken,
      ReleaseManagementSecurityPermissions permissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrEmpty(securityToken))
        throw new ArgumentNullException(nameof (securityToken));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, SecurityConstants.ReleaseManagementSecurityNamespaceId);
      bool flag = securityNamespace.HasPermission(requestContext, securityToken, (int) permissions, alwaysAllowAdministrators);
      if (!flag)
        flag = securityNamespace.PollForRequestLocalInvalidation(requestContext) && securityNamespace.HasPermission(requestContext, securityToken, (int) permissions, alwaysAllowAdministrators);
      return flag;
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed")]
    public static bool HasPermission(
      this IVssRequestContext requestContext,
      ReleaseManagementSecurityInfo securityInfo,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string securityToken = securityInfo != null ? securityInfo.GetToken() : throw new ArgumentNullException(nameof (securityInfo));
      return requestContext.HasPermission(securityToken, securityInfo.Permission, alwaysAllowAdministrators);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed")]
    public static bool HasPermission(
      this IVssRequestContext requestContext,
      Guid projectId,
      string folderPath,
      int releaseDefinitionId,
      ReleaseManagementSecurityPermissions permission,
      bool alwaysAllowAdministrators = false)
    {
      return requestContext.HasPermission(projectId, folderPath, releaseDefinitionId, -1, permission, alwaysAllowAdministrators);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed")]
    public static bool HasPermission(
      this IVssRequestContext requestContext,
      Guid projectId,
      string folderPath,
      int releaseDefinitionId,
      int environmentId,
      ReleaseManagementSecurityPermissions permission,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (requestContext.IsSystemContext)
        return true;
      ReleaseManagementSecurityInfo securityInfo = new ReleaseManagementSecurityInfo()
      {
        ProjectId = projectId,
        Path = folderPath,
        ReleaseDefinitionId = releaseDefinitionId,
        Permission = permission,
        EnvironmentId = environmentId
      };
      return requestContext.HasPermission(securityInfo, alwaysAllowAdministrators);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Need to set a default value")]
    public static bool HasUIPermission(
      this IVssRequestContext requestContext,
      ReleaseManagementUISecurityPermissions permission,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string uiPermissionToken = SecurityConstants.ReleaseManagementUIPermissionToken;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, SecurityConstants.ReleaseManagementUISecurityNamespaceId);
      bool flag = securityNamespace.HasPermission(requestContext, uiPermissionToken, (int) permission, alwaysAllowAdministrators);
      if (!flag)
        flag = securityNamespace.PollForRequestLocalInvalidation(requestContext) && securityNamespace.HasPermission(requestContext, uiPermissionToken, (int) permission, alwaysAllowAdministrators);
      return flag;
    }

    public static void SetSecuredObjects<T>(
      this IVssRequestContext requestContext,
      IEnumerable<T> results)
      where T : ReleaseManagementSecuredObject
    {
      if (results == null || !results.Any<T>())
        return;
      string securityToken;
      ReleaseManagementSecurityPermissions permission;
      ReleaseManagementSecurityExtensions.ValidateSecurityCachedItems(requestContext, out securityToken, out permission);
      foreach (T result in results)
        result.SetSecuredObject(securityToken, (int) permission);
      ReleaseManagementSecurityExtensions.ClearSecurityCachedItems(requestContext);
    }

    public static void SetSecuredObject<T>(this IVssRequestContext requestContext, T result) where T : ReleaseManagementSecuredObject
    {
      if ((object) result == null)
        return;
      string securityToken;
      ReleaseManagementSecurityPermissions permission;
      ReleaseManagementSecurityExtensions.ValidateSecurityCachedItems(requestContext, out securityToken, out permission);
      result.SetSecuredObject(securityToken, (int) permission);
      ReleaseManagementSecurityExtensions.ClearSecurityCachedItems(requestContext);
    }

    public static ISecuredObject GetSecuredObject(this IVssRequestContext requestContext)
    {
      string securityToken;
      ReleaseManagementSecurityPermissions permission;
      ReleaseManagementSecurityExtensions.ValidateSecurityCachedItems(requestContext, out securityToken, out permission);
      return (ISecuredObject) new ReleaseManagementSecuredObject(securityToken, (int) permission);
    }

    private static void ValidateSecurityCachedItems(
      IVssRequestContext requestContext,
      out string securityToken,
      out ReleaseManagementSecurityPermissions permission)
    {
      securityToken = (string) null;
      permission = ReleaseManagementSecurityPermissions.None;
      if (!requestContext.TryGetItem<string>("releaseManagementSecurityTokenKey", out securityToken) && securityToken != null)
        throw new InvalidOperationException(Resources.CannotFindSecurityToken);
      if (!requestContext.TryGetItem<ReleaseManagementSecurityPermissions>("releaseManagementSecurityPermissionKey", out permission) && permission != ReleaseManagementSecurityPermissions.None)
        throw new InvalidOperationException(Resources.CannotFindSecurityPermission);
    }

    private static void ClearSecurityCachedItems(IVssRequestContext requestContext)
    {
      if (requestContext.TryGetItem<string>("releaseManagementSecurityTokenKey", out string _))
        requestContext.Items.Remove("releaseManagementSecurityTokenKey");
      if (!requestContext.TryGetItem<ReleaseManagementSecurityPermissions>("releaseManagementSecurityPermissionKey", out ReleaseManagementSecurityPermissions _))
        return;
      requestContext.Items.Remove("releaseManagementSecurityPermissionKey");
    }
  }
}
