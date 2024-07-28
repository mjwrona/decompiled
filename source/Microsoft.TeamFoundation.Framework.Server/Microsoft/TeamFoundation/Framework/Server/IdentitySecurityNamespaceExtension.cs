// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentitySecurityNamespaceExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class IdentitySecurityNamespaceExtension : DefaultSecurityNamespaceExtension
  {
    private const string c_useSecurityNamespaceExtensionToComputePermissions = "VisualStudio.Services.Identity.UseSecurityNamespaceExtensionToComputePermissions";
    private static readonly char[] IdentitySecurityTokenDelimiter = new char[1]
    {
      '\\'
    };
    private const string c_area = "IdentityService";
    private const int c_servicePrincipalPermissionsOnOwnedGroupScope = 14;

    public override bool HasPermission(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      int effectiveAllows,
      int effectiveDenys,
      bool preliminaryDecision)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSecurityNamespaceExtensionToComputePermissions"))
      {
        bool computedDecision = preliminaryDecision;
        this.ComputePermissionsWithWellKnownGroupDefaults(requestContext, token, principal.PrimaryDescriptor, effectiveAllows, effectiveDenys, requestedPermissions, out int _, out computedDecision, preliminaryDecision);
        if (computedDecision != preliminaryDecision)
          requestContext.TraceConditionally(80452, TraceLevel.Verbose, "IdentityService", nameof (IdentitySecurityNamespaceExtension), (Func<string>) (() => string.Format("IdentitySecurityNamespaceExtension changed decision (from {0} to {1}) for {2} with token {3}", (object) preliminaryDecision, (object) computedDecision, (object) principal.PrimaryDescriptor, (object) token)));
        preliminaryDecision = computedDecision;
      }
      if (!preliminaryDecision && token != null && !token.StartsWith("$", StringComparison.Ordinal))
      {
        Guid spId;
        Guid scopeId;
        if (IdentitySecurityNamespaceExtension.IsDeploymentAndHosted(requestContext) && IdentitySecurityNamespaceExtension.CanGrantRequestedPermissions(requestedPermissions, effectiveAllows, effectiveDenys, 14) && ServicePrincipals.IsServicePrincipal(requestContext, principal.PrimaryDescriptor, false, out spId) && IdentitySecurityNamespaceExtension.TryParseScopeId(token, out scopeId))
        {
          ServiceDefinition serviceDefinition = !(requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS) ? requestContext.GetService<ILocationService>().FindServiceDefinition(requestContext, "LocationService2", scopeId) : throw new InvalidOperationException("Expected to be running on SPS since this is an IMS permission check at the deployment level on a hosted service.");
          if (serviceDefinition == null && scopeId != requestContext.ServiceInstanceId())
          {
            requestContext.Trace(80451, TraceLevel.Verbose, "IdentityService", nameof (IdentitySecurityNamespaceExtension), "Service principal {0} requested {1} on group scope {2}: granted because service definition does not yet exist", (object) spId, (object) (requestedPermissions & 14), (object) scopeId);
            return true;
          }
          if (serviceDefinition != null && StringComparer.Ordinal.Equals(serviceDefinition.ParentServiceType, "VsService") && InstanceManagementHelper.ServicePrincipalFromServiceInstance(serviceDefinition.ParentIdentifier) == spId)
          {
            requestContext.Trace(80451, TraceLevel.Verbose, "IdentityService", nameof (IdentitySecurityNamespaceExtension), "Service principal {0} requested {1} on group scope {2}: granted because service definition is owned by same service principal", (object) spId, (object) (requestedPermissions & 14), (object) scopeId);
            return true;
          }
        }
        if (1 == requestedPermissions)
        {
          IdentityScope identityScope = IdentitySecurityNamespaceExtension.TryGetIdentityScope(requestContext, token);
          if (identityScope != null && requestContext.GetService<IdentityService>().IsMember(requestContext, identityScope.Administrators, principal.PrimaryDescriptor))
          {
            requestContext.Trace(80451, TraceLevel.Verbose, "IdentityService", nameof (IdentitySecurityNamespaceExtension), "Identity descriptor {0} requested {1} on group scope {2}: granted because identity is a member of scope Administrators group", (object) principal.PrimaryDescriptor, (object) 1, (object) identityScope.Id);
            return true;
          }
        }
      }
      return preliminaryDecision;
    }

    public override int QueryEffectivePermissions(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string token,
      EvaluationPrincipal principal,
      int preliminaryEffectivePermissions)
    {
      int effectiveAllow = preliminaryEffectivePermissions;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSecurityNamespaceExtensionToComputePermissions"))
        this.ComputePermissionsWithWellKnownGroupDefaults(requestContext, token, principal.PrimaryDescriptor, preliminaryEffectivePermissions, 0, preliminaryEffectivePermissions, out effectiveAllow, out bool _);
      return effectiveAllow;
    }

    public override string HandleIncomingToken(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string securityToken)
    {
      if (securityToken != null && securityToken.StartsWith("vstfs:///", StringComparison.OrdinalIgnoreCase))
      {
        int num = securityToken.LastIndexOf('/');
        if (num > 0 && num < securityToken.Length - 1)
          return securityToken.Substring(num + 1);
      }
      return securityToken;
    }

    public override void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string token,
      int requestedPermissions)
    {
      if (IdentitySecurityNamespaceExtension.TryGetIdentityScope(requestContext, token) != null)
      {
        List<string> values = new List<string>();
        if ((requestedPermissions & 1) == 1)
          values.Add(FrameworkResources.PermissionIdentityRead());
        if ((requestedPermissions & 2) == 2)
          values.Add(FrameworkResources.PermissionIdentityWrite());
        if ((requestedPermissions & 4) == 4)
          values.Add(FrameworkResources.PermissionIdentityDelete());
        if ((requestedPermissions & 8) == 8)
          values.Add(FrameworkResources.PermissionIdentityManageMembership());
        if ((requestedPermissions & 16) == 16)
          values.Add(FrameworkResources.PermissionIdentityCreateScope());
        throw new AccessCheckException(identity.Descriptor, identity.DisplayName, token, requestedPermissions, FrameworkSecurity.IdentitiesNamespaceId, FrameworkResources.AccessCheckExceptionPrivilegeFormatWithDetails((object) identity.Id.ToString(), (object) FrameworkResources.IdentityNamespaceName(), (object) string.Join(", ", (IEnumerable<string>) values), (object) token, (object) requestedPermissions));
      }
      base.ThrowAccessDeniedException(requestContext, securityNamespace, identity, token, requestedPermissions);
    }

    public void ComputePermissionsWithWellKnownGroupDefaults(
      IVssRequestContext requestContext,
      string token,
      IdentityDescriptor descriptor,
      int allowBits,
      int denyBits,
      int requestedPermissions,
      out int effectiveAllow,
      out bool computedDecision,
      bool preliminaryDecision = false)
    {
      effectiveAllow = allowBits;
      computedDecision = preliminaryDecision;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Dictionary<GroupScopeType, Dictionary<string, ScopeWellKnownGroupPermissionEntry>> permissionEntries = vssRequestContext.GetService<IIdentitySecurityNamespaceSettingsService>().GetScopeWellKnownGroupPermissionEntries(vssRequestContext);
      Guid scopeId1;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || permissionEntries.IsNullOrEmpty<KeyValuePair<GroupScopeType, Dictionary<string, ScopeWellKnownGroupPermissionEntry>>>() || !this.TryGetScopeId(requestContext, token, out scopeId1))
        return;
      IdentityService service = requestContext.GetService<IdentityService>();
      IdentityScope identityScope1 = IdentitySecurityNamespaceExtension.TryGetIdentityScope(requestContext, scopeId1.ToString());
      if (identityScope1 == null)
        return;
      Guid scopeId2 = scopeId1;
      if ((identityScope1.ScopeType == GroupScopeType.TeamProject || identityScope1.ScopeType == GroupScopeType.Generic) && identityScope1.ParentId != Guid.Empty)
      {
        if (identityScope1.ScopeType == GroupScopeType.Generic)
        {
          IdentityScope identityScope2 = IdentitySecurityNamespaceExtension.TryGetIdentityScope(requestContext, identityScope1.ParentId.ToString());
          if (identityScope2 != null && identityScope2.ScopeType == GroupScopeType.TeamProject)
            scopeId2 = identityScope2.ParentId;
        }
        else
          scopeId2 = identityScope1.ParentId;
        Dictionary<string, ScopeWellKnownGroupPermissionEntry> dictionary = permissionEntries[identityScope1.ScopeType];
        foreach (string key in dictionary.Keys)
        {
          IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(key);
          IdentityDescriptor groupDescriptor = IdentityDomain.MapFromWellKnownIdentifier(identityScope1.Id, foundationDescriptor);
          if (service.IsMember(requestContext, groupDescriptor, descriptor))
          {
            denyBits |= dictionary[key].DenyBits & ~denyBits;
            allowBits |= dictionary[key].AllowBits & ~denyBits;
          }
        }
      }
      Dictionary<string, ScopeWellKnownGroupPermissionEntry> dictionary1 = permissionEntries[GroupScopeType.ServiceHost];
      foreach (string key in dictionary1.Keys)
      {
        IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(key);
        IdentityDescriptor groupDescriptor = IdentityDomain.MapFromWellKnownIdentifier(scopeId2, foundationDescriptor);
        if (service.IsMember(requestContext, groupDescriptor, descriptor))
        {
          denyBits |= dictionary1[key].DenyBits & ~denyBits;
          allowBits |= dictionary1[key].AllowBits & ~denyBits;
        }
      }
      effectiveAllow = allowBits;
      computedDecision = (allowBits & requestedPermissions) == requestedPermissions;
      bool computedDecisionTraceMessage = computedDecision;
      requestContext.TraceConditionally(80453, TraceLevel.Verbose, "IdentityService", nameof (IdentitySecurityNamespaceExtension), (Func<string>) (() => string.Format("Computed: permissions as {0}, allowBits as {1} denyBits as {2}; inputs: preliminaryDecision as {3} for token {4} with IdentityDescriptor {5}.", (object) computedDecisionTraceMessage, (object) allowBits, (object) denyBits, (object) preliminaryDecision, (object) token, (object) descriptor)));
    }

    private bool TryGetScopeId(IVssRequestContext requestContext, string token, out Guid scopeId)
    {
      string[] strArray = token.Split(IdentitySecurityNamespaceExtension.IdentitySecurityTokenDelimiter, StringSplitOptions.RemoveEmptyEntries);
      scopeId = new Guid();
      if (strArray.Length <= 2 && IdentitySecurityNamespaceExtension.TryParseScopeId(strArray[0], out scopeId))
        return true;
      requestContext.TraceConditionally(1040073, TraceLevel.Verbose, "IdentityService", nameof (IdentitySecurityNamespaceExtension), (Func<string>) (() => "Cannot parse scopeId from token " + token));
      return false;
    }

    private static IdentityScope TryGetIdentityScope(
      IVssRequestContext requestContext,
      string token)
    {
      IdentityScope identityScope = (IdentityScope) null;
      Guid result;
      if (token.Length >= 36 && Guid.TryParse(token.Substring(0, 36), out result))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        try
        {
          identityScope = service.GetScope(requestContext.Elevate(), result);
        }
        catch (GroupScopeDoesNotExistException ex)
        {
        }
      }
      return identityScope;
    }

    private static bool IsDeploymentAndHosted(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);

    private static bool CanGrantRequestedPermissions(
      int totalRequestedPermissions,
      int alreadyAllowedBits,
      int alreadyDeniedBits,
      int newPotentiallyAllowedBits)
    {
      int num = totalRequestedPermissions & ~alreadyAllowedBits;
      return num != 0 && (alreadyDeniedBits & newPotentiallyAllowedBits) == 0 && num == (num & newPotentiallyAllowedBits);
    }

    private static bool TryParseScopeId(string token, out Guid scopeId)
    {
      scopeId = Guid.Empty;
      return token != null && token.Length >= 36 && Guid.TryParse(token.Substring(0, 36), out scopeId);
    }
  }
}
