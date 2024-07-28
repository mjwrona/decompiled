// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityPermissionHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityPermissionHelper
  {
    private static string c_area = "IMS";
    private static string c_layer = "IdentityScopeHelper";
    private const string c_groupMembershipTokenPrefix = "GroupMemberships/";
    private const string c_allowOnlyJITServicePrincipalToManageMembership = "VisualStudio.Services.Identity.AllowOnlyJitServicePrincipalToManageMembership";
    private const string c_preventMembershipUpdatesOnEnterpriseServiceAccountsGroup = "VisualStudio.Services.Identity.PreventMembershipUpdatesOnEnterpriseServiceAccountsGroup";

    internal static void CheckUpdateMembershipPermissionsOnJITManagedOrganizations(
      IVssRequestContext context,
      SubjectDescriptor groupDescriptor)
    {
      if (!context.ExecutionEnvironment.IsHostedDeployment || !context.IsFeatureEnabled("VisualStudio.Services.Identity.AllowOnlyJitServicePrincipalToManageMembership") || !context.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection) || !IdentityPermissionHelper.IsAdminGroup(context, groupDescriptor))
        return;
      context.TraceEnter(15270090, IdentityPermissionHelper.c_area, IdentityPermissionHelper.c_layer, nameof (CheckUpdateMembershipPermissionsOnJITManagedOrganizations));
      context.TraceDataConditionally(15270091, TraceLevel.Verbose, IdentityPermissionHelper.c_area, IdentityPermissionHelper.c_layer, "Received input parameters", (Func<object>) (() => (object) new
      {
        groupDescriptor = groupDescriptor
      }), nameof (CheckUpdateMembershipPermissionsOnJITManagedOrganizations));
      try
      {
        string token = "GroupMemberships/" + (string) groupDescriptor;
        context.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(context, SecurityNamespaceIds.SystemGraphNamespaceId).CheckPermission(context, token, 4, false);
      }
      finally
      {
        context.TraceLeave(15270092, IdentityPermissionHelper.c_area, IdentityPermissionHelper.c_layer, nameof (CheckUpdateMembershipPermissionsOnJITManagedOrganizations));
      }
    }

    internal static void CheckUpdateMembershipPermissionsOnEnterpriseServiceAccountsGroup(
      IVssRequestContext context,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor)
    {
      if (context.ExecutionEnvironment.IsHostedDeployment && context.IsFeatureEnabled("VisualStudio.Services.Identity.PreventMembershipUpdatesOnEnterpriseServiceAccountsGroup") && IdentityPermissionHelper.IsEnterpriseServiceAccountsGroup(context.To(TeamFoundationHostType.Application), groupDescriptor))
      {
        context.TraceDataConditionally(15270094, TraceLevel.Verbose, IdentityPermissionHelper.c_area, IdentityPermissionHelper.c_layer, string.Format("Blocking attempt to modify membership of Enterprise Service Accounts group: {0} for memberDescriptor: {1}", (object) groupDescriptor, (object) memberDescriptor), methodName: nameof (CheckUpdateMembershipPermissionsOnEnterpriseServiceAccountsGroup));
        throw new AccessCheckException(string.Format("Requesting user is not allowed modify memberships of Enterprise Service Accounts group: {0}.", (object) groupDescriptor));
      }
    }

    private static bool IsAdminGroup(IVssRequestContext context, SubjectDescriptor descriptor) => IdentityDomain.MapToWellKnownIdentifier(context.ServiceHost.InstanceId, descriptor).Equals(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup.ToSubjectDescriptor(context));

    private static bool IsEnterpriseServiceAccountsGroup(
      IVssRequestContext context,
      SubjectDescriptor descriptor)
    {
      return IdentityDomain.MapToWellKnownIdentifier(context.ServiceHost.InstanceId, descriptor).Equals(GroupWellKnownIdentityDescriptors.ServiceUsersGroup.ToSubjectDescriptor(context));
    }
  }
}
