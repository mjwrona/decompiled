// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationSubscriptionSecurityUtils
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationSubscriptionSecurityUtils
  {
    internal static GroupAdminCheck PerformGroupAdminCheck = new GroupAdminCheck(NotificationSubscriptionSecurityUtils.IsGroupAdminInternal);
    internal static GroupAdminBulkCheck PerformGroupAdminBulkCheck = new GroupAdminBulkCheck(NotificationSubscriptionSecurityUtils.GetBulkGroupAdminPermissionsInternal);
    private static readonly string s_subscriberToken = "$SUBSCRIBER:";
    private static readonly string s_subscriberTokenFormatString = "$SUBSCRIBER:{0}";

    internal static bool HasPermissionsNoGroupAdminCheck(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriptionIdentity,
      int requestedPermission)
    {
      return NotificationSubscriptionSecurityUtils.HasPermissionsWorker(requestContext, subscriptionIdentity, requestedPermission, true, out bool _, out bool _);
    }

    internal static bool HasPermissions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriptionIdentity,
      int requestedPermission)
    {
      return NotificationSubscriptionSecurityUtils.HasPermissionsWorker(requestContext, subscriptionIdentity, requestedPermission, false, out bool _, out bool _);
    }

    internal static bool HasPermissions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriptionIdentity,
      int requestedPermission,
      out bool isGroupAdmin,
      out bool hasDoneGroupAdminCheck)
    {
      return NotificationSubscriptionSecurityUtils.HasPermissionsWorker(requestContext, subscriptionIdentity, requestedPermission, false, out isGroupAdmin, out hasDoneGroupAdminCheck);
    }

    private static bool HasPermissionsWorker(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriptionIdentity,
      int requestedPermission,
      bool skipGroupAdminCheck,
      out bool isGroupAdmin,
      out bool hasDoneGroupAdminCheck)
    {
      isGroupAdmin = false;
      hasDoneGroupAdminCheck = false;
      if (requestContext.IsSystemContext)
        return true;
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(subscriptionIdentity, nameof (subscriptionIdentity));
      Guid id = subscriptionIdentity.Id;
      Guid userId = requestContext.GetUserId();
      if (id.Equals(userId))
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.EventSubscriberNamespaceId);
      string tokenFromSubscriber = NotificationSubscriptionSecurityUtils.GetTokenFromSubscriber(id);
      IVssRequestContext requestContext1 = requestContext;
      string token = tokenFromSubscriber;
      int requestedPermissions = requestedPermission;
      bool flag = securityNamespace.HasPermission(requestContext1, token, requestedPermissions);
      if (!flag && !skipGroupAdminCheck)
      {
        flag = NotificationSubscriptionSecurityUtils.IsGroupAdmin(requestContext, subscriptionIdentity);
        hasDoneGroupAdminCheck = true;
        isGroupAdmin = flag;
      }
      return flag;
    }

    internal static bool CallerHasAdminPermissions(
      IVssRequestContext requestContext,
      int requestedPermission)
    {
      return requestContext.IsSystemContext || requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.EventSubscriberNamespaceId).HasPermission(requestContext, NotificationSubscriptionSecurityUtils.s_subscriberToken, requestedPermission, false);
    }

    internal static void SetSubscriberAccessControlEntry(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriber)
    {
      if (!subscriber.IsContainer)
        return;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.EventSubscriberNamespaceId);
      if (securityNamespace == null)
        return;
      NotificationSubscriptionSecurityUtils.SetSubscriptionAccessControlEntry(requestContext, subscriber, securityNamespace);
    }

    internal static void SetSubscriptionAccessControlEntry(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity subscriber,
      IVssSecurityNamespace eventSecurity)
    {
      string tokenFromSubscriber = NotificationSubscriptionSecurityUtils.GetTokenFromSubscriber(subscriber.Id);
      eventSecurity.SetAccessControlEntry(requestContext, tokenFromSubscriber, (IAccessControlEntry) new AccessControlEntry(subscriber.Descriptor, 1, 0), false, rootNewIdentities: false);
    }

    private static bool IsGroupAdmin(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity groupId) => groupId.IsContainer && NotificationSubscriptionSecurityUtils.PerformGroupAdminCheck(requestContext, groupId);

    private static bool IsGroupAdminInternal(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity groupId)
    {
      string securityToken = NotificationSubscriptionSecurityUtils.CreateSecurityToken(groupId);
      Guid identitiesNamespaceId = FrameworkSecurity.IdentitiesNamespaceId;
      return !requestContext.ExecutionEnvironment.IsHostedDeployment || !(requestContext.ServiceInstanceType() != ServiceInstanceTypes.TFS) ? requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).HasPermission(requestContext, securityToken, 8) : requestContext.GetClient<SecurityHttpClient>(ServiceInstanceTypes.TFS).HasPermissionAsync(identitiesNamespaceId, securityToken, 8, true, cancellationToken: requestContext.CancellationToken).SyncResult<bool>();
    }

    internal static List<bool> GetBulkGroupAdminPermissions(
      IVssRequestContext requestContext,
      List<Microsoft.VisualStudio.Services.Identity.Identity> groupIds)
    {
      return NotificationSubscriptionSecurityUtils.PerformGroupAdminBulkCheck(requestContext, groupIds);
    }

    private static List<bool> GetBulkGroupAdminPermissionsInternal(
      IVssRequestContext requestContext,
      List<Microsoft.VisualStudio.Services.Identity.Identity> groupIds)
    {
      string[] tokens = new string[groupIds.Count];
      for (int index = 0; index < groupIds.Count; ++index)
        tokens[index] = NotificationSubscriptionSecurityUtils.CreateSecurityToken(groupIds[index]);
      Guid sps = ServiceInstanceTypes.SPS;
      Guid identitiesNamespaceId = FrameworkSecurity.IdentitiesNamespaceId;
      List<bool> permissionsInternal;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceInstanceType() != ServiceInstanceTypes.TFS)
      {
        permissionsInternal = requestContext.To(TeamFoundationHostType.Application).GetClient<SecurityHttpClient>(sps).HasPermissionsAsync(identitiesNamespaceId, (IEnumerable<string>) tokens, 8, true, cancellationToken: requestContext.CancellationToken).SyncResult<List<bool>>();
      }
      else
      {
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId);
        bool[] collection = new bool[groupIds.Count];
        for (int index = 0; index < groupIds.Count; ++index)
          collection[index] = securityNamespace.HasPermission(requestContext, tokens[index], 8);
        permissionsInternal = new List<bool>((IEnumerable<bool>) collection);
      }
      return permissionsInternal;
    }

    internal static string CreateSecurityToken(Microsoft.VisualStudio.Services.Identity.Identity group)
    {
      object obj = (object) null;
      group.TryGetProperty("LocalScopeId", out obj);
      return string.Format("{0}{1}{2}", (object) (obj?.ToString() ?? string.Empty), (object) FrameworkSecurity.IdentitySecurityPathSeparator, (object) group.Id.ToString());
    }

    internal static string GetTokenFromSubscriber(Guid subscriberId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, NotificationSubscriptionSecurityUtils.s_subscriberTokenFormatString, (object) subscriberId.ToString());

    internal static string GetRootToken() => NotificationSubscriptionSecurityUtils.s_subscriberToken;
  }
}
