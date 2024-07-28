// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionPoliciesService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class ExtensionPoliciesService : 
    VssBaseService,
    IExtensionPoliciesService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public UserExtensionPolicy GetPolicies(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      bool flag = this.HasManagePermission(requestContext);
      return new UserExtensionPolicy()
      {
        UserId = userIdentity.Id.ToString(),
        DisplayName = userIdentity.DisplayName,
        Permissions = new ExtensionPolicy()
        {
          Install = flag ? ExtensionPolicyFlags.All : ExtensionPolicyFlags.None,
          Request = ExtensionPolicyFlags.All
        }
      };
    }

    public void CheckManagePermission(IVssRequestContext requestContext) => this.CheckPermissionInExtensionManagement(requestContext, 2);

    public bool HasManagePermission(IVssRequestContext requestContext) => this.CheckPermissionInExtensionManagement(requestContext, 2, false);

    public bool HasAdminPermission(IVssRequestContext requestContext) => this.CheckPermissionInExtensionManagement(requestContext, 4, false, true);

    private bool CheckPermissionInExtensionManagement(
      IVssRequestContext requestContext,
      int permission,
      bool throwIfNotChecked = true,
      bool alwaysAllowAdministrators = false)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ExtensionManagementSecurity.ExtensionManagementNamespaceId);
      string empty = string.Empty;
      if (!throwIfNotChecked)
        return securityNamespace.HasPermission(requestContext, empty, permission, alwaysAllowAdministrators);
      securityNamespace.CheckPermission(requestContext, empty, permission, alwaysAllowAdministrators);
      return true;
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> GetExtensionManagers(
      IVssRequestContext requestContext,
      int maxResults)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> extensionManagers = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (maxResults > 0)
      {
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ISecuredTeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ExtensionManagementSecurity.ExtensionManagementNamespaceId);
        IAccessControlList accessControlList = securityNamespace.QueryAccessControlLists(requestContext, string.Empty, false, false).FirstOrDefault<IAccessControlList>();
        if (accessControlList != null)
        {
          IList<IdentityDescriptor> list = (IList<IdentityDescriptor>) accessControlList.AccessControlEntries.Where<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (ace => (ace.Allow & 2) != 0)).Select<IAccessControlEntry, IdentityDescriptor>((Func<IAccessControlEntry, IdentityDescriptor>) (ace => ace.Descriptor)).ToList<IdentityDescriptor>();
          foreach (Microsoft.VisualStudio.Services.Identity.Identity expandedActiveMember in this.GetExpandedActiveMembers(requestContext, list))
          {
            if ((securityNamespace.QueryEffectivePermissions(requestContext, string.Empty, (EvaluationPrincipal) expandedActiveMember.Descriptor) & 2) != 0)
            {
              extensionManagers.Add(expandedActiveMember);
              if (extensionManagers.Count >= maxResults)
                break;
            }
          }
        }
      }
      return extensionManagers;
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetExpandedActiveMembers(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors)
    {
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary1 = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
      Dictionary<string, IdentityDescriptor> dictionary2 = new Dictionary<string, IdentityDescriptor>();
      IdentityService service = requestContext.GetService<IdentityService>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(requestContext, descriptors, QueryMembership.Expanded, (IEnumerable<string>) null))
      {
        if (readIdentity != null && readIdentity.IsActive)
        {
          if (readIdentity.IsContainer)
          {
            foreach (IdentityDescriptor member in (IEnumerable<IdentityDescriptor>) readIdentity.Members)
              dictionary2[member.Identifier] = member;
          }
          else
            dictionary1[readIdentity.Id] = readIdentity;
        }
      }
      if (dictionary2.Count > 0)
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) dictionary2.Values.ToList<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null))
        {
          if (readIdentity != null && readIdentity.IsActive && !readIdentity.IsContainer && !IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) readIdentity))
            dictionary1[readIdentity.Id] = readIdentity;
        }
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) dictionary1.Values;
    }
  }
}
