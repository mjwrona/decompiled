// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsAccessService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsViewsAccessService : IAnalyticsViewsAccessService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void CheckReadViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId)
    {
      AnalyticsViewsAccessService.CheckPermission(requestContext, 1, visibility, projectId, new Guid?(viewId));
    }

    public void CheckEditViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId)
    {
      AnalyticsViewsAccessService.CheckPermission(requestContext, 2, visibility, projectId, new Guid?(viewId));
    }

    public void CheckDeleteViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId)
    {
      AnalyticsViewsAccessService.CheckPermission(requestContext, 4, visibility, projectId, new Guid?(viewId));
    }

    public void CheckCreateViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId)
    {
      if (visibility != AnalyticsViewVisibility.Shared)
        return;
      AnalyticsViewsAccessService.CheckPermission(requestContext, 2, AnalyticsViewVisibility.Shared, projectId);
    }

    public void CheckExecViewsPermission(IVssRequestContext requestContext, Guid projectId) => AnalyticsViewsAccessService.GetSecurityNamespace(requestContext).CheckPermission(requestContext, string.Format("${0}{1}", (object) AnalyticsViewsSecurityNamespace.NamespaceSeparator, (object) projectId), 8);

    public void SetViewOwnerPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId)
    {
      this.SetPermission(requestContext, AnalyticsViewsSecurityNamespace.DefaultViewOwnerAllowPermissions, AnalyticsViewsSecurityNamespace.DefaultViewOwnerDenyPermissions, requestContext.GetUserIdentity().Descriptor, visibility, projectId, viewId);
    }

    public bool HasReadViewPermission(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId)
    {
      return AnalyticsViewsAccessService.HasPermission(requestContext, 1, visibility, projectId, new Guid?(viewId));
    }

    public int GetEffectivePermissions(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid? viewId)
    {
      IVssSecurityNamespace securityNamespace = AnalyticsViewsAccessService.GetSecurityNamespace(requestContext);
      string securityToken = AnalyticsViewsAccessService.GetSecurityToken(requestContext, visibility, projectId, viewId);
      IVssRequestContext requestContext1 = requestContext;
      string token = securityToken;
      EvaluationPrincipal descriptor = (EvaluationPrincipal) requestContext.GetUserIdentity().Descriptor;
      return securityNamespace.QueryEffectivePermissions(requestContext1, token, descriptor);
    }

    public void RemovePermissionsForVisibility(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid? viewId)
    {
      IVssSecurityNamespace securityNamespace = AnalyticsViewsAccessService.GetSecurityNamespace(requestContext);
      string securityToken = AnalyticsViewsAccessService.GetSecurityToken(requestContext, visibility, projectId, viewId);
      IVssRequestContext requestContext1 = requestContext;
      string[] tokens = new string[1]{ securityToken };
      securityNamespace.RemoveAccessControlLists(requestContext1, (IEnumerable<string>) tokens, false);
    }

    private void SetPermission(
      IVssRequestContext requestContext,
      int allowPermission,
      int denyPermission,
      IdentityDescriptor identity,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid viewId)
    {
      AnalyticsViewsAccessService.GetSecurityNamespace(requestContext).SetPermissions(requestContext, AnalyticsViewsSecurityNamespace.GetSecurityToken(visibility, projectId, viewId), identity, allowPermission, denyPermission, false);
    }

    private static void CheckPermission(
      IVssRequestContext requestContext,
      int permission,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid? viewId = null)
    {
      AnalyticsViewsAccessService.GetSecurityNamespace(requestContext).CheckPermission(requestContext, AnalyticsViewsAccessService.GetSecurityToken(requestContext, visibility, projectId, viewId), permission, false);
    }

    private static bool HasPermission(
      IVssRequestContext requestContext,
      int permission,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid? viewId = null)
    {
      return AnalyticsViewsAccessService.GetSecurityNamespace(requestContext).HasPermission(requestContext, AnalyticsViewsAccessService.GetSecurityToken(requestContext, visibility, projectId, viewId), permission, false);
    }

    private static string GetSecurityToken(
      IVssRequestContext requestContext,
      AnalyticsViewVisibility visibility,
      Guid projectId,
      Guid? viewId = null)
    {
      return !viewId.HasValue ? AnalyticsViewsSecurityNamespace.GetSecurityToken(visibility, projectId) : AnalyticsViewsSecurityNamespace.GetSecurityToken(visibility, projectId, viewId.Value);
    }

    private static IVssSecurityNamespace GetSecurityNamespace(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AnalyticsViewsSecurityNamespace.Id);
  }
}
