// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.TenantPolicyPermissionValidator
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public class TenantPolicyPermissionValidator : ITenantPolicyPermissionExtension
  {
    private const string c_area = "TenantPolicy";
    private const string c_layer = "TenantPolicyPermissionValidator";
    internal const string UseGetUserRolesAndGroupsFeatureName = "VisualStudio.Services.TenantPolicy.UseGetUserRolesAndGroupsForPermissionValidation";

    public virtual bool HasPermission(
      IVssRequestContext context,
      string policyName,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      TenantPolicyPermissionType permissionType = TenantPolicyPermissionType.Write)
    {
      Guid tenantId = AadIdentityHelper.ExtractTenantId((IReadOnlyVssIdentity) identity);
      return this.HasPermission(context, policyName, identity, identity.GetAadObjectId(), tenantId, permissionType);
    }

    public virtual bool HasPermission(
      IVssRequestContext context,
      string policyName,
      Guid identityObjectId,
      Guid targetTenantId,
      TenantPolicyPermissionType permissionType = TenantPolicyPermissionType.Write)
    {
      return this.HasPermission(context, policyName, (Microsoft.VisualStudio.Services.Identity.Identity) null, identityObjectId, targetTenantId, permissionType);
    }

    public bool HasPermission(
      IVssRequestContext context,
      string policyName,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid identityObjectId,
      Guid targetTenantId,
      TenantPolicyPermissionType permissionType = TenantPolicyPermissionType.Write)
    {
      if (!PolicyNames.KnownTenantPolicyNames.Contains<string>(policyName) || !context.ExecutionEnvironment.IsHostedDeployment || targetTenantId == Guid.Empty || !context.ServiceHost.Is(TeamFoundationHostType.Deployment) && !context.IsOrganizationAadBacked())
        return false;
      if (!TenantPolicyRolloutHelper.IsInPreview(context, targetTenantId))
      {
        context.Trace(8524011, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), string.Format("TenantPolicy is not in preview for tenant {0}, request user oid is {1}", (object) targetTenantId, (object) identityObjectId));
        return false;
      }
      context.Trace(8524012, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), string.Format("TenantPolicy is in preview for tenant {0}, request user oid is {1}", (object) targetTenantId, (object) identityObjectId));
      if (!TenantPolicyRolloutHelper.IsPolicyFeatureAvailable(context, policyName, targetTenantId))
      {
        context.Trace(8524013, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), string.Format("TenantPolicy policy {0} is not available for tenant {1}, request user oid is {2}", (object) policyName, (object) targetTenantId, (object) identityObjectId));
        return false;
      }
      if (!this.IsPolicyAvailableForMicrosoftTenant(policyName) && context.IsMicrosoftTenant(targetTenantId))
        return false;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = identity ?? context.GetUserIdentity();
      if (context.IsFeatureEnabled("VisualStudio.Services.TenantPolicy.RestrictTenantPolicyAPIToTenantMembers") && targetTenantId != AadIdentityHelper.ExtractTenantId((IReadOnlyVssIdentity) userIdentity))
      {
        context.Trace(8524014, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), string.Format("User (oid: {0}) is not a member of {1}", (object) identityObjectId, (object) targetTenantId));
        return false;
      }
      return permissionType == TenantPolicyPermissionType.Read || this.HasAdminRolePermission(context, identityObjectId, targetTenantId);
    }

    private bool HasAdminRolePermission(
      IVssRequestContext context,
      Guid identityObjectId,
      Guid targetTenantId)
    {
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Deployment).Elevate();
      try
      {
        IEnumerable<AadDirectoryRole> adminDirectoryRoles = this.GetAdminDirectoryRoles(context1, targetTenantId);
        return this.IsMember(context1, identityObjectId, adminDirectoryRoles, targetTenantId);
      }
      catch (Exception ex)
      {
        context.TraceException(8524035, TraceLevel.Error, "TenantPolicy", nameof (TenantPolicyPermissionValidator), ex);
        return false;
      }
    }

    private bool IsPolicyAvailableForMicrosoftTenant(string policyName)
    {
      switch (policyName)
      {
        case "TenantPolicy.OrganizationCreationRestriction":
          return false;
        default:
          return true;
      }
    }

    private IEnumerable<AadDirectoryRole> GetAdminDirectoryRoles(
      IVssRequestContext context,
      Guid targetTenantId)
    {
      GetDirectoryRolesRequest directoryRolesRequest = new GetDirectoryRolesRequest();
      directoryRolesRequest.ToTenant = targetTenantId.ToString();
      GetDirectoryRolesRequest request = directoryRolesRequest;
      GetDirectoryRolesResponse directoryRoles = context.GetService<AadService>().GetDirectoryRoles(context, request);
      HashSet<string> adminRoleTemplateObjectIds = TenantPolicyPermissionValidator.ReadAdminRoleTemplateIds(context);
      return directoryRoles.DirectoryRoles.Where<AadDirectoryRole>((Func<AadDirectoryRole, bool>) (x => adminRoleTemplateObjectIds.Contains(x.RoleTemplateId)));
    }

    private static HashSet<string> ReadAdminRoleTemplateIds(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      string str = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/TenantRoleTemplateIds", "e3973bdf-4987-49ae-837a-ba8e231c7286");
      context.Trace(8524025, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), "The AdminRoleTemplateIds string is " + str + ".");
      if (string.IsNullOrEmpty(str))
      {
        context.TraceAlways(8524026, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), "Empty AdminRoleTemplateIds found");
        return new HashSet<string>();
      }
      try
      {
        return ((IEnumerable<string>) str.Split(',')).ToHashSet<string>();
      }
      catch (Exception ex)
      {
        context.TraceException(8524027, TraceLevel.Error, "TenantPolicy", nameof (TenantPolicyPermissionValidator), ex);
        return new HashSet<string>();
      }
    }

    private bool IsMember(
      IVssRequestContext context,
      Guid identityObjectId,
      IEnumerable<AadDirectoryRole> adminDirectoryRoles,
      Guid targetTenantId)
    {
      if (adminDirectoryRoles.Count<AadDirectoryRole>() == 0)
      {
        context.TraceAlways(8524020, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), "No activated Azure admin role found in the tenant, fail the permission check.");
        return false;
      }
      if (context.IsFeatureEnabled("VisualStudio.Services.TenantPolicy.UseGetUserRolesAndGroupsForPermissionValidation"))
        return this.IsUserInAdminRoles(context, identityObjectId, adminDirectoryRoles, targetTenantId);
      AadService service = context.GetService<AadService>();
      foreach (AadDirectoryRole adminDirectoryRole in adminDirectoryRoles)
      {
        GetDirectoryRoleMembersRequest roleMembersRequest = new GetDirectoryRoleMembersRequest();
        roleMembersRequest.DirectoryRoleObjectId = adminDirectoryRole.ObjectId;
        roleMembersRequest.ToTenant = targetTenantId.ToString();
        GetDirectoryRoleMembersRequest request = roleMembersRequest;
        ISet<AadObject> members = service.GetDirectoryRoleMembers(context, request).Members;
        if (members.Any<AadObject>((Func<AadObject, bool>) (m => m.ObjectId == identityObjectId)))
        {
          context.Trace(8524030, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), string.Format("The userObjectId {0} is member of {1}. ", (object) identityObjectId, (object) adminDirectoryRole.RoleTemplateId) + "Members are " + string.Join<Guid>(",", (IEnumerable<Guid>) members.Select<AadObject, Guid>((Func<AadObject, Guid>) (m => m.ObjectId)).ToArray<Guid>()));
          return true;
        }
      }
      context.Trace(8524031, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), string.Format("The userObjectId {0} is not member of any admin roles", (object) identityObjectId));
      return false;
    }

    private bool IsUserInAdminRoles(
      IVssRequestContext context,
      Guid identityObjectId,
      IEnumerable<AadDirectoryRole> adminDirectoryRoles,
      Guid targetTenantId)
    {
      ISet<Guid> userRolesAndGroups = this.GetUserRolesAndGroups(context, identityObjectId, targetTenantId);
      foreach (AadDirectoryRole adminDirectoryRole in adminDirectoryRoles)
      {
        if (userRolesAndGroups.Contains(adminDirectoryRole.ObjectId))
        {
          context.Trace(8524032, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), string.Format("The userObjectId {0} is member of {1}. ", (object) identityObjectId, (object) adminDirectoryRole.RoleTemplateId));
          return true;
        }
      }
      context.Trace(8524033, TraceLevel.Info, "TenantPolicy", nameof (TenantPolicyPermissionValidator), string.Format("The userObjectId {0} is not member of any admin roles", (object) identityObjectId));
      return false;
    }

    private ISet<Guid> GetUserRolesAndGroups(
      IVssRequestContext context,
      Guid identityObjectId,
      Guid targetTenantId)
    {
      GetUserRolesAndGroupsRequest andGroupsRequest = new GetUserRolesAndGroupsRequest();
      andGroupsRequest.UserObjectId = identityObjectId;
      andGroupsRequest.ToTenant = targetTenantId.ToString();
      GetUserRolesAndGroupsRequest request = andGroupsRequest;
      return context.GetService<AadService>().GetUserRolesAndGroups(context, request).Members;
    }
  }
}
