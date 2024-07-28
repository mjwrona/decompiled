// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.SecurityRoleMappingService
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  public class SecurityRoleMappingService : ISecurityRoleMappingService, IVssFrameworkService
  {
    private IDisposableReadOnlyList<ISecurityRoleScope> m_roleScopes;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_roleScopes = VssExtensionManagementService.GetExtensionsRaw<ISecurityRoleScope>(systemRequestContext.ServiceHost.PlugInDirectory);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_roleScopes == null)
        return;
      this.m_roleScopes.Dispose();
      this.m_roleScopes = (IDisposableReadOnlyList<ISecurityRoleScope>) null;
    }

    public List<SecurityRole> GetRoles(IVssRequestContext requestContext, string scopeId) => this.GetScope(requestContext, scopeId).GetRoles();

    public List<RoleAssignment> GetRoleAssignments(
      IVssRequestContext requestContext,
      string resourceId,
      string scopeId)
    {
      ISecurityRoleScope scope = this.GetScope(requestContext, scopeId);
      RoleAssignmentSecurityContext assignmentSecurityContext = new RoleAssignmentSecurityContext(requestContext, scope, resourceId);
      IVssRequestContext scopeContext = assignmentSecurityContext.ScopeContext;
      assignmentSecurityContext.CheckPermission(scope.GetReadPermission());
      IVssRequestContext requestContext1 = scopeContext.Elevate();
      IAccessControlList accessControlList = assignmentSecurityContext.SecurityNamespace.QueryAccessControlLists(requestContext1, assignmentSecurityContext.SecurityToken, true, false).FirstOrDefault<IAccessControlList>();
      IdentityService service = scopeContext.GetService<IdentityService>();
      List<RoleAssignment> roleAssignments = new List<RoleAssignment>();
      if (accessControlList == null)
        return roleAssignments;
      List<IdentityDescriptor> list1 = accessControlList.AccessControlEntries.Select<IAccessControlEntry, IdentityDescriptor>((Func<IAccessControlEntry, IdentityDescriptor>) (ace => ace.Descriptor)).ToList<IdentityDescriptor>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = service.ReadIdentities(scopeContext, (IList<IdentityDescriptor>) list1, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => id != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      IdentityMapper identityMapper = new IdentityMapper(scopeContext.ServiceHost.InstanceId);
      foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
      {
        IAccessControlEntry ace = accessControlEntry;
        Microsoft.VisualStudio.Services.Identity.Identity identity = list2.Find((Predicate<Microsoft.VisualStudio.Services.Identity.Identity>) (id => IdentityDescriptorComparer.Instance.Equals(id.Descriptor, identityMapper.MapFromWellKnownIdentifier(ace.Descriptor))));
        if (identity != null && !scope.ShouldExcludeFromRoleAssignmentListing(scopeContext, identity))
        {
          SecurityRole role1 = scope.GetRole(ace.Allow);
          SecurityRole role2 = scope.GetRole(ace.EffectiveAllow);
          SecurityRole y = role2;
          RoleAccess roleAccess = SecurityRole.Equals(role1, y) ? RoleAccess.Assigned : RoleAccess.Inherited;
          SecurityRole securityRole = role2;
          if (securityRole != null)
            roleAssignments.Add(new RoleAssignment()
            {
              Identity = new IdentityRef()
              {
                Id = identity.Id.ToString(),
                DisplayName = identity.DisplayName,
                UniqueName = identity.ProviderDisplayName
              },
              Role = securityRole,
              Access = roleAccess
            });
        }
      }
      return roleAssignments;
    }

    public List<RoleAssignment> SetRoleAssignments(
      IVssRequestContext requestContext,
      List<UserRoleAssignmentRef> userRoles,
      string resourceId,
      string scopeId,
      bool limitToCallerIdentityDomain = false)
    {
      ISecurityRoleScope scope = this.GetScope(requestContext, scopeId);
      RoleAssignmentSecurityContext assignmentSecurityContext = new RoleAssignmentSecurityContext(requestContext, scope, resourceId);
      IVssRequestContext scopeContext = assignmentSecurityContext.ScopeContext;
      assignmentSecurityContext.CheckPermission(scope.GetWritePermission());
      IVssRequestContext requestContext1 = scopeContext.Elevate();
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      Dictionary<IdentityDescriptor, RoleAssignment> newRoles = new Dictionary<IdentityDescriptor, RoleAssignment>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      if (userRoles != null && userRoles.Any<UserRoleAssignmentRef>())
      {
        foreach (UserRoleAssignmentRef userRole1 in userRoles)
        {
          UserRoleAssignmentRef userRole = userRole1;
          SecurityRole securityRole = scope.GetRoles().Where<SecurityRole>((Func<SecurityRole, bool>) (scopeRole => string.Equals(scopeRole.Name, userRole.RoleName))).FirstOrDefault<SecurityRole>();
          if (securityRole == null)
            throw new RoleNotFoundException(userRole.RoleName);
          IdentityService service = requestContext.GetService<IdentityService>();
          Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          bool flag = false;
          if (userRole.UserId != Guid.Empty)
          {
            List<Guid> identityIds = new List<Guid>()
            {
              userRole.UserId
            };
            identity = service.ReadIdentities(requestContext, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && identity == null && !limitToCallerIdentityDomain)
            {
              IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
              identity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            }
          }
          else if (!string.IsNullOrEmpty(userRole.UniqueName))
          {
            List<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(requestContext, IdentitySearchFilter.MailAddress, userRole.UniqueName, QueryMembership.None, (IEnumerable<string>) null).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
            if (source.Count > 0)
              flag = true;
            if (source.Count > 1)
            {
              List<Microsoft.VisualStudio.Services.Identity.Identity> list = source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (candidate =>
              {
                string property = candidate.GetProperty<string>("Domain", "Windows Live ID");
                return !string.IsNullOrEmpty(property) && !string.Equals(property, "Windows Live ID", StringComparison.OrdinalIgnoreCase);
              })).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
              if (list.Count > 0)
                source = list;
            }
            if (source.Count > 1 | limitToCallerIdentityDomain)
            {
              string authorizedUserDomain = requestContext.GetUserIdentity().GetProperty<string>("Domain", (string) null);
              if (authorizedUserDomain != null)
              {
                List<Microsoft.VisualStudio.Services.Identity.Identity> list = source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (candidate =>
                {
                  string property = candidate.GetProperty<string>("Domain", (string) null);
                  return property != null && string.Equals(authorizedUserDomain, property, StringComparison.OrdinalIgnoreCase);
                })).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
                if (list.Count > 0 | limitToCallerIdentityDomain)
                  source = list;
              }
            }
            identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          }
          if (limitToCallerIdentityDomain && identity == null)
          {
            if (flag)
              throw new IdentityNotFoundInCurrentDirectoryException();
            throw new IdentityNotFoundException();
          }
          Guid userId = scopeContext.GetUserId(true);
          if (identity == null || userId != Guid.Empty && userId.Equals(identity.Id))
          {
            if (identity != null && userRoles.Count == 1)
              throw new InvalidOperationException(SecurityRolesResources.CannotModifySelfRole());
          }
          else
          {
            IdentityDescriptor descriptor = identity.Descriptor;
            accessControlEntryList.Add(new AccessControlEntry(descriptor, securityRole.AllowPermissions, securityRole.DenyPermissions));
            newRoles.Add(descriptor, new RoleAssignment()
            {
              Identity = new IdentityRef()
              {
                Id = identity.Id.ToString(),
                DisplayName = identity.DisplayName,
                UniqueName = identity.ProviderDisplayName
              },
              Role = securityRole,
              Access = RoleAccess.Assigned
            });
          }
        }
      }
      List<IAccessControlEntry> list1 = assignmentSecurityContext.SecurityNamespace.SetAccessControlEntries(requestContext1, assignmentSecurityContext.SecurityToken, (IEnumerable<IAccessControlEntry>) accessControlEntryList, false).ToList<IAccessControlEntry>();
      List<RoleAssignment> successfulAssignments = new List<RoleAssignment>();
      IdentityMapper identityMapper = new IdentityMapper(scopeContext.ServiceHost.InstanceId);
      Action<IAccessControlEntry> action = (Action<IAccessControlEntry>) (ace =>
      {
        RoleAssignment roleAssignment;
        if (!newRoles.TryGetValue(identityMapper.MapFromWellKnownIdentifier(ace.Descriptor), out roleAssignment))
          return;
        successfulAssignments.Add(roleAssignment);
      });
      list1.ForEach(action);
      scope.CompleteSetRoleAssignments(scopeContext, successfulAssignments, resourceId);
      return successfulAssignments;
    }

    public bool RemoveRoleAssignments(
      IVssRequestContext requestContext,
      List<Guid> identityIds,
      string resourceId,
      string scopeId)
    {
      ISecurityRoleScope scope = this.GetScope(requestContext, scopeId);
      RoleAssignmentSecurityContext assignmentSecurityContext = new RoleAssignmentSecurityContext(requestContext, scope, resourceId);
      IVssRequestContext scopeContext = assignmentSecurityContext.ScopeContext;
      assignmentSecurityContext.CheckPermission(scope.GetWritePermission());
      IVssRequestContext requestContext1 = scopeContext.Elevate();
      Dictionary<Guid, IdentityDescriptor> identityDescriptors = SecurityRoleMappingService.GetIdentityDescriptors(requestContext, (IList<Guid>) identityIds);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && identityDescriptors.Count < identityIds.Count)
      {
        HashSet<Guid> source = new HashSet<Guid>((IEnumerable<Guid>) identityIds);
        foreach (Guid key in identityDescriptors.Keys)
          source.Remove(key);
        IVssRequestContext requestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
        identityDescriptors.AddRange<KeyValuePair<Guid, IdentityDescriptor>, Dictionary<Guid, IdentityDescriptor>>((IEnumerable<KeyValuePair<Guid, IdentityDescriptor>>) SecurityRoleMappingService.GetIdentityDescriptors(requestContext2, (IList<Guid>) source.ToList<Guid>()));
      }
      int num = assignmentSecurityContext.SecurityNamespace.RemoveAccessControlEntries(requestContext1, assignmentSecurityContext.SecurityToken, (IEnumerable<IdentityDescriptor>) identityDescriptors.Values) ? 1 : 0;
      if (num == 0)
        return num != 0;
      scope.CompleteRemoveRoleAssignments(scopeContext, identityIds, resourceId);
      return num != 0;
    }

    public void ChangeInheritance(
      IVssRequestContext requestContext,
      string scopeId,
      string resourceId,
      bool inheritPermissions)
    {
      ISecurityRoleScope scope = this.GetScope(requestContext, scopeId);
      RoleAssignmentSecurityContext assignmentSecurityContext = new RoleAssignmentSecurityContext(requestContext, scope, resourceId);
      IVssRequestContext scopeContext = assignmentSecurityContext.ScopeContext;
      assignmentSecurityContext.CheckPermission(scope.GetWritePermission());
      assignmentSecurityContext.ChangeInheritance(inheritPermissions);
    }

    private static Dictionary<Guid, IdentityDescriptor> GetIdentityDescriptors(
      IVssRequestContext requestContext,
      IList<Guid> identityIds)
    {
      Dictionary<Guid, IdentityDescriptor> identityDescriptors = new Dictionary<Guid, IdentityDescriptor>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) requestContext.GetService<IdentityService>().ReadIdentities(requestContext, identityIds, QueryMembership.None, (IEnumerable<string>) null))
      {
        if (readIdentity != null)
          identityDescriptors[readIdentity.Id] = readIdentity.Descriptor;
      }
      return identityDescriptors;
    }

    internal virtual ISecurityRoleScope GetScope(IVssRequestContext requestContext, string scopeId)
    {
      if (this.m_roleScopes != null)
      {
        foreach (ISecurityRoleScope roleScope in (IEnumerable<ISecurityRoleScope>) this.m_roleScopes)
        {
          if (roleScope.GetScopeId().Equals(scopeId))
            return roleScope;
        }
      }
      throw new ScopeNotFoundException(scopeId);
    }
  }
}
