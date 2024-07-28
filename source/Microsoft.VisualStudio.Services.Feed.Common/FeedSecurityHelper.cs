// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedSecurityHelper
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public class FeedSecurityHelper
  {
    public static bool CanBypassUnderMaintenance(IVssRequestContext requestContext)
    {
      if (!ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()) || requestContext.IsImpersonating)
        return false;
      IVssSecurityNamespace feedIndexSecurity = FeedSecurityHelper.GetFeedIndexSecurity(requestContext);
      int num = 1;
      IVssRequestContext requestContext1 = requestContext;
      int requestedPermissions = num;
      return feedIndexSecurity.HasPermission(requestContext1, "$/", requestedPermissions, false);
    }

    public static bool SetFeedPermissions(
      IVssRequestContext requestContext,
      FeedCore feed,
      IEnumerable<AccessControlEntry> permissions)
    {
      return requestContext.TraceBlock<bool>(10019024, 10019025, 10019026, "Feed", "Security", (Func<bool>) (() =>
      {
        string securityToken = FeedSecurityHelper.CalculateSecurityToken(feed);
        IEnumerable<IAccessControlEntry> source = FeedSecurityHelper.GetFeedSecurity(requestContext).SetAccessControlEntries(requestContext, securityToken, (IEnumerable<IAccessControlEntry>) permissions, false);
        return source != null && source.Any<IAccessControlEntry>();
      }), nameof (SetFeedPermissions));
    }

    public static bool RemoveAllPermissionsForFeedView(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      if (feed.View == null)
        throw new ArgumentException("RemoveAllPermissionsForFeedView must be called with a FeedCore which represents a view.");
      return requestContext.TraceBlock<bool>(10019092, 10019093, 10019094, "Feed", "Security", (Func<bool>) (() =>
      {
        string securityToken = FeedSecurityHelper.CalculateSecurityToken(feed);
        return FeedSecurityHelper.GetFeedSecurity(requestContext).RemoveAccessControlLists(requestContext, (IEnumerable<string>) new string[1]
        {
          securityToken
        }, false);
      }), nameof (RemoveAllPermissionsForFeedView));
    }

    public static void CheckCreateFeedPermissions(IVssRequestContext requestContext) => requestContext.TraceBlock(10019030, 10019031, 10019032, "Feed", "Security", (Action) (() =>
    {
      if (!FeedSecurityHelper.GetFeedSecurity(requestContext).HasPermission(requestContext, "$/", 8))
        throw new FeedNeedsPermissionsException(FeedSecurityHelper.GetUserString(requestContext), Enum.GetName(typeof (FeedPermissionConstants), (object) FeedPermissionConstants.CreateFeed));
    }), nameof (CheckCreateFeedPermissions));

    private static string GetUserString(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return requestContext.GetUserId().ToString("D");
      string userString = requestContext.AuthenticatedUserName;
      if (requestContext.IsImpersonating)
        userString = requestContext.DomainUserName;
      return userString;
    }

    public static bool HasReadFeedPermissions(
      IVssRequestContext requestContext,
      FeedCore feed,
      bool checkImplicitAccess = false)
    {
      return FeedSecurityHelper.HasFeedPermission(requestContext, FeedSecurityHelper.GetFeedSecurity(requestContext), feed, 32, true, checkImplicitAccess);
    }

    public static bool HasModifyIndexPermissions(
      IVssRequestContext requestContext,
      FeedCore feed,
      bool checkImplicitAccess = false)
    {
      return FeedSecurityHelper.HasFeedPermission(requestContext, FeedSecurityHelper.GetFeedIndexSecurity(requestContext), feed, 1, false, checkImplicitAccess);
    }

    public static bool HasFeedPermissions(
      IVssRequestContext requestContext,
      FeedCore feed,
      int permissions,
      bool checkImplicitAccess = false)
    {
      return FeedSecurityHelper.HasFeedPermission(requestContext, FeedSecurityHelper.GetFeedSecurity(requestContext), feed, permissions, true, checkImplicitAccess);
    }

    public static void CheckReadFeedPermissions(
      IVssRequestContext requestContext,
      FeedCore feed,
      bool checkImplicitAccess = false)
    {
      FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.ReadPackages, 10019033, 10019034, 10019035, checkImplicitAccess);
    }

    public static void CheckAdministerFeedPermissions(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.AdministerFeed, 10019036, 10019037, 10019038);
    }

    public static void CheckEditFeedPermissions(IVssRequestContext requestContext, FeedCore feed) => FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.EditFeed, 10019039, 10019040, 10019041);

    public static void CheckDeleteFeedPermissions(IVssRequestContext requestContext, FeedCore feed) => FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.DeleteFeed, 10019042, 10019043, 10019044);

    public static void CheckAddPackagePermissions(IVssRequestContext requestContext, FeedCore feed) => FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.AddPackage, 10019045, 10019046, 10019047);

    public static void CheckUpdatePackagePermissions(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.UpdatePackage, 10019048, 10019049, 10019050);
    }

    public static void CheckDeletePackagePermissions(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.DeletePackage, 10019051, 10019052, 10019053);
    }

    public static void CheckDelistPackagePermissions(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.DelistPackage, 10019063, 10019064, 10019065);
    }

    public static void CheckAddUpstreamPackagePermissions(
      IVssRequestContext requestContext,
      FeedCore feed)
    {
      FeedSecurityHelper.CheckFeedPermissions(requestContext, feed, FeedPermissionConstants.AddUpstreamPackage, 10019104, 10019105, 10019106);
    }

    public static void CheckModifyIndexPermissions(IVssRequestContext requestContext, FeedCore feed) => FeedSecurityHelper.CheckFeedIndexPermissions(requestContext, feed, FeedIndexPermissionConstants.InternalAdmin, 10019221, 10019222, 10019223);

    public static void CheckModifyIndexPermissions(IVssRequestContext requestContext) => requestContext.TraceBlock(10019221, 10019222, 10019223, "Feed", "Security", (Action) (() =>
    {
      IVssSecurityNamespace feedIndexSecurity = FeedSecurityHelper.GetFeedIndexSecurity(requestContext);
      int num = 1;
      IVssRequestContext requestContext1 = requestContext;
      int requestedPermissions = num;
      if (!feedIndexSecurity.HasPermission(requestContext1, "$/", requestedPermissions, false))
        throw new FeedNeedsPermissionsException(FeedSecurityHelper.GetUserString(requestContext), Enum.GetName(typeof (FeedIndexPermissionConstants), (object) FeedIndexPermissionConstants.InternalAdmin));
    }), nameof (CheckModifyIndexPermissions));

    public static IVssSecurityNamespace GetFeedSecurity(IVssRequestContext requestContext) => FeedSecurityHelper.GetSecurityNamespace(requestContext, FeedConstants.FeedSecurityNamespaceId);

    public static IVssSecurityNamespace GetFeedIndexSecurity(IVssRequestContext requestContext) => FeedSecurityHelper.GetSecurityNamespace(requestContext, FeedConstants.FeedIndexSecurityNamespaceId);

    public static string CalculateSecurityToken(FeedCore feed, bool includeView = true) => FeedSecurityHelper.CalculateSecurityTokenWithSpecifiedView(feed, includeView ? feed.View : (FeedView) null);

    public static string CalculateSecurityToken(ArtifactsFeed feed, bool includeView = true)
    {
      if (!includeView)
        return FeedSecurityHelper.CalculateSecurityTokenWithSpecifiedView(feed, new Guid?());
      return feed.SelectedViewId == Guid.Empty ? FeedSecurityHelper.CalculateSecurityTokenWithSpecifiedView(feed, new Guid?()) : FeedSecurityHelper.CalculateSecurityTokenWithSpecifiedView(feed, new Guid?(feed.SelectedViewId));
    }

    public static string CalculateSecurityTokenWithSpecifiedView(FeedCore feed, FeedView view) => FeedSecurityHelper.CalculateSecurityTokenCore(feed.Project?.Id, feed.Id, view?.Id);

    public static string CalculateSecurityTokenWithSpecifiedView(ArtifactsFeed feed, Guid? viewId) => FeedSecurityHelper.CalculateSecurityTokenCore(feed.Project?.Id, feed.Id, viewId);

    private static string CalculateSecurityTokenCore(Guid? projectId, Guid feedId, Guid? viewId)
    {
      string str = projectId.HasValue ? string.Format("$/project:{0}/{1}/", (object) projectId, (object) feedId) : string.Format("$/{0}/", (object) feedId);
      return !viewId.HasValue ? str : string.Format("{0}{1}/", (object) str, (object) viewId);
    }

    public static void ThrowFeedNeedsPermissionException(
      IVssRequestContext requestContext,
      string permissionName)
    {
      throw new FeedNeedsPermissionsException(FeedSecurityHelper.GetUserString(requestContext), permissionName);
    }

    private static IVssSecurityNamespace GetSecurityNamespace(
      IVssRequestContext requestContext,
      Guid securityNamespaceId)
    {
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, securityNamespaceId) ?? throw new SecurityNamespaceNullException();
    }

    private static void CheckFeedPermissions(
      IVssRequestContext requestContext,
      FeedCore feed,
      FeedPermissionConstants permissions,
      int traceEnter,
      int traceLeave,
      int traceException,
      bool checkImplicitAccess = false)
    {
      if (requestContext.IsSystemContext)
        return;
      FeedSecurityHelper.CheckPermissions(requestContext, FeedSecurityHelper.GetFeedSecurity(requestContext), feed, Enum.GetName(typeof (FeedPermissionConstants), (object) permissions), (int) permissions, true, traceEnter, traceLeave, traceException, checkImplicitAccess);
    }

    private static void CheckFeedIndexPermissions(
      IVssRequestContext requestContext,
      FeedCore feed,
      FeedIndexPermissionConstants permissions,
      int traceEnter,
      int traceLeave,
      int traceException)
    {
      FeedSecurityHelper.CheckPermissions(requestContext, FeedSecurityHelper.GetFeedIndexSecurity(requestContext), feed, Enum.GetName(typeof (FeedIndexPermissionConstants), (object) permissions), (int) permissions, false, traceEnter, traceLeave, traceException, false);
    }

    private static void CheckPermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      FeedCore feed,
      string permissionName,
      int permissionsToCheck,
      bool alwaysAllowAdministrators,
      int traceEnter,
      int traceLeave,
      int traceException,
      bool checkImplicitAccess)
    {
      requestContext.TraceBlock(traceEnter, traceLeave, traceException, "Feed", "Security", (Action) (() =>
      {
        if (FeedSecurityHelper.HasFeedPermission(requestContext, securityNamespace, feed, permissionsToCheck, alwaysAllowAdministrators, checkImplicitAccess))
          return;
        FeedSecurityHelper.ThrowFeedNeedsPermissionException(requestContext, permissionName);
      }), nameof (CheckPermissions));
    }

    private static bool HasFeedPermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      FeedCore feed,
      int permissionsToCheck,
      bool alwaysAllowAdministrators,
      bool checkImplicitAccess)
    {
      return requestContext.TraceBlock<bool>(10019027, 10019028, 10019029, "Feed", "Security", (Func<bool>) (() =>
      {
        string securityToken = FeedSecurityHelper.CalculateSecurityToken(feed);
        try
        {
          if (!securityNamespace.HasPermission(requestContext, securityToken, permissionsToCheck, alwaysAllowAdministrators))
          {
            if ((!(checkImplicitAccess & (permissionsToCheck & permissionsToCheck - 1) == 0) ? 0 : (securityNamespace.HasPermissionForAnyChildren(requestContext, securityToken, permissionsToCheck, alwaysAllowAdministrators: alwaysAllowAdministrators) ? 1 : 0)) == 0)
            {
              if (securityNamespace.PollForRequestLocalInvalidation(requestContext))
              {
                if (securityNamespace.HasPermission(requestContext, securityToken, permissionsToCheck, alwaysAllowAdministrators))
                  goto label_8;
              }
              return false;
            }
          }
        }
        catch (VssServiceException ex) when (FeedException.IsHostShutdownResponse(ex))
        {
          throw new HostShutdownException(ex.Message);
        }
label_8:
        return true;
      }), nameof (HasFeedPermission));
    }
  }
}
