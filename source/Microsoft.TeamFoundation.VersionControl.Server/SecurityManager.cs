// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.SecurityManager
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal sealed class SecurityManager
  {
    public const int AllWorkspacePermissions = 15;

    public SecurityManager(IVssRequestContext requestContext) => this.IdentityService = requestContext.GetService<Microsoft.VisualStudio.Services.Identity.IdentityService>().IdentityServiceInternal();

    internal Microsoft.VisualStudio.Services.Identity.Identity FindIdentity(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(700105, TraceArea.Identities, TraceLayer.BusinessLogic, nameof (FindIdentity));
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        if (userIdentity == null)
        {
          requestContext.Trace(700106, TraceLevel.Verbose, TraceArea.Identities, TraceLayer.BusinessLogic, "ReadRequestIdentity has no match");
          throw new IdentityNotFoundException("IdentityNotFoundException", !(requestContext.UserContext != (IdentityDescriptor) null) || string.IsNullOrEmpty(requestContext.UserContext.Identifier) ? "<unknown>" : requestContext.UserContext.Identifier);
        }
        requestContext.Trace(700107, TraceLevel.Verbose, TraceArea.Identities, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (FindIdentity), (object) userIdentity);
        return userIdentity;
      }
      finally
      {
        requestContext.TraceLeave(700108, TraceArea.Identities, TraceLayer.BusinessLogic, nameof (FindIdentity));
      }
    }

    public void FindIdentityNames(
      IVssRequestContext requestContext,
      Guid vsid,
      out string identityName,
      out string displayName)
    {
      requestContext.Trace(700116, TraceLevel.Verbose, TraceArea.Identities, TraceLayer.BusinessLogic, "Entering {0}: {1}", (object) nameof (FindIdentityNames), (object) vsid);
      Microsoft.VisualStudio.Services.Identity.Identity identity = TfvcIdentityHelper.FindIdentity(requestContext, vsid, false);
      displayName = identity.DisplayName;
      string uniqueName = IdentityHelper.GetUniqueName(identity);
      identityName = string.IsNullOrEmpty(uniqueName) ? identity.Id.ToString() : uniqueName;
      requestContext.Trace(700118, TraceLevel.Verbose, TraceArea.Identities, TraceLayer.BusinessLogic, "Leaving {0}: {1} {2}", (object) nameof (FindIdentityNames), (object) identityName, (object) displayName);
    }

    public string FindIdentityDisplayName(IVssRequestContext requestContext, Guid vsid)
    {
      string displayName;
      this.FindIdentityNames(requestContext, vsid, out string _, out displayName);
      return displayName;
    }

    public void CheckGSSGlobalPermission(
      VersionControlRequestContext versionControlRequestContext,
      string actionName,
      int permissionRequired)
    {
      versionControlRequestContext.RequestContext.Trace(700125, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) nameof (CheckGSSGlobalPermission), (object) actionName, (object) permissionRequired);
      if (!this.HasGSSGlobalPermission(versionControlRequestContext, permissionRequired))
      {
        versionControlRequestContext.RequestContext.Trace(700126, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Permission Denied!");
        throw new ResourceAccessException(versionControlRequestContext.RequestContext.GetUserId().ToString(), actionName);
      }
      versionControlRequestContext.RequestContext.Trace(700127, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (CheckGSSGlobalPermission), (object) "Permission Granted");
    }

    public void CheckGSSProjectPermission(
      VersionControlRequestContext versionControlRequestContext,
      string projectUri,
      string actionName,
      int permissionRequired,
      bool alwaysAllowAdmins = true)
    {
      versionControlRequestContext.RequestContext.Trace(700128, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2} {3}", (object) nameof (CheckGSSProjectPermission), (object) projectUri, (object) actionName, (object) permissionRequired);
      if (!this.HasGSSProjectPermission(versionControlRequestContext, projectUri, permissionRequired, alwaysAllowAdmins))
      {
        versionControlRequestContext.RequestContext.Trace(700129, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Permission Denied!");
        throw new ResourceAccessException(versionControlRequestContext.RequestContext.GetUserId().ToString(), actionName);
      }
      versionControlRequestContext.RequestContext.Trace(700130, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (CheckGSSProjectPermission), (object) "Permission Granted");
    }

    public bool HasGSSGlobalPermission(
      VersionControlRequestContext versionControlRequestContext,
      int permissionRequired)
    {
      versionControlRequestContext.RequestContext.Trace(700131, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1}", (object) nameof (HasGSSGlobalPermission), (object) permissionRequired);
      bool flag = versionControlRequestContext.GetAuthorizationGlobalSecurity().HasPermission(versionControlRequestContext.RequestContext, FrameworkSecurity.TeamProjectCollectionNamespaceToken, permissionRequired);
      versionControlRequestContext.RequestContext.Trace(700132, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (HasGSSGlobalPermission), (object) flag);
      return flag;
    }

    public bool HasGSSProjectPermission(
      VersionControlRequestContext versionControlRequestContext,
      string projectUri,
      int permissionRequired,
      bool alwaysAllowAdmins = true)
    {
      versionControlRequestContext.RequestContext.Trace(700133, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) nameof (HasGSSProjectPermission), (object) projectUri, (object) permissionRequired);
      IVssSecurityNamespace authorizationProjectSecurity = versionControlRequestContext.GetAuthorizationProjectSecurity();
      bool flag = authorizationProjectSecurity.HasPermission(versionControlRequestContext.RequestContext, authorizationProjectSecurity.NamespaceExtension.HandleIncomingToken(versionControlRequestContext.RequestContext, authorizationProjectSecurity, projectUri), permissionRequired, alwaysAllowAdmins);
      versionControlRequestContext.RequestContext.Trace(700134, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (HasGSSProjectPermission), (object) flag);
      return flag;
    }

    public void CheckGlobalPermission(
      VersionControlRequestContext versionControlRequestContext,
      GlobalPermissions permissionRequired)
    {
      versionControlRequestContext.RequestContext.Trace(700135, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1}", (object) nameof (CheckGlobalPermission), (object) permissionRequired);
      versionControlRequestContext.GetPrivilegeSecurity().CheckPermission(versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, (int) permissionRequired);
      versionControlRequestContext.RequestContext.Trace(700136, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (CheckGlobalPermission), (object) "Permission Granted");
    }

    public bool HasGlobalPermission(
      VersionControlRequestContext versionControlRequestContext,
      GlobalPermissions permissionRequired)
    {
      versionControlRequestContext.RequestContext.Trace(700137, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1}", (object) nameof (HasGlobalPermission), (object) permissionRequired);
      bool flag = versionControlRequestContext.GetPrivilegeSecurity().HasPermission(versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, (int) permissionRequired);
      versionControlRequestContext.RequestContext.Trace(700138, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (HasGlobalPermission), (object) flag);
      return flag;
    }

    public bool HasItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string itemPath)
    {
      return this.HasItemPermission(versionControlRequestContext, permissionRequired, itemPath, false);
    }

    public bool HasItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string itemPath,
      bool alwaysAllowAdmins)
    {
      return this.HasItemPermission(versionControlRequestContext, permissionRequired, ItemPathPair.FromServerItem(itemPath), alwaysAllowAdmins);
    }

    public bool HasItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair)
    {
      return this.HasItemPermission(versionControlRequestContext, permissionRequired, itemPathPair, false);
    }

    public bool HasItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair,
      bool alwaysAllowAdmins)
    {
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      bool flag = securityNamespace.HasPermission(versionControlRequestContext.RequestContext, token, (int) permissionRequired, alwaysAllowAdmins);
      versionControlRequestContext.RequestContext.Trace(700095, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "HasItemPermission({0},{1},{2}) = {3}", (object) permissionRequired, (object) token, (object) alwaysAllowAdmins, (object) flag);
      return flag;
    }

    public bool HasItemPermissionExpect(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair,
      bool alwaysAllowAdmins,
      bool expectedResult)
    {
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      return securityNamespace.HasPermissionExpect(versionControlRequestContext.RequestContext, token, (int) permissionRequired, expectedResult, alwaysAllowAdmins);
    }

    public bool HasItemPermissionForAnyChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string itemPath)
    {
      return this.HasItemPermissionForAnyChildren(versionControlRequestContext, permissionRequired, itemPath, false, false);
    }

    public bool HasItemPermissionForAnyChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string itemPath,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdmins)
    {
      return this.HasItemPermissionForAnyChildren(versionControlRequestContext, permissionRequired, ItemPathPair.FromServerItem(itemPath), resultIfNoChildrenFound, alwaysAllowAdmins);
    }

    public bool HasItemPermissionForAnyChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair)
    {
      return this.HasItemPermissionForAnyChildren(versionControlRequestContext, permissionRequired, itemPathPair, false, false);
    }

    public bool HasItemPermissionForAnyChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdmins)
    {
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      return securityNamespace.HasPermissionForAnyChildren(versionControlRequestContext.RequestContext, token, (int) permissionRequired, resultIfNoChildrenFound, alwaysAllowAdmins);
    }

    public bool HasItemPermissionForAllChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string itemPath)
    {
      return this.HasItemPermissionForAllChildren(versionControlRequestContext, permissionRequired, itemPath, true, false);
    }

    public bool HasItemPermissionForAllChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string itemPath,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdmins)
    {
      return this.HasItemPermissionForAllChildren(versionControlRequestContext, permissionRequired, ItemPathPair.FromServerItem(itemPath), resultIfNoChildrenFound, alwaysAllowAdmins);
    }

    public bool HasItemPermissionForAllChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair)
    {
      return this.HasItemPermissionForAllChildren(versionControlRequestContext, permissionRequired, itemPathPair, true, false);
    }

    public bool HasItemPermissionForAllChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdmins)
    {
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      return securityNamespace.HasPermissionForAllChildren(versionControlRequestContext.RequestContext, token, (int) permissionRequired, resultIfNoChildrenFound, alwaysAllowAdmins);
    }

    public void CheckItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string itemPath)
    {
      this.CheckItemPermission(versionControlRequestContext, permissionRequired, itemPath, false);
    }

    public void CheckItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string itemPath,
      bool alwaysAllowAdmins)
    {
      this.CheckItemPermission(versionControlRequestContext, permissionRequired, ItemPathPair.FromServerItem(itemPath), alwaysAllowAdmins);
    }

    public void CheckItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair)
    {
      this.CheckItemPermission(versionControlRequestContext, permissionRequired, itemPathPair, false);
    }

    public void CheckItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair,
      bool alwaysAllowAdmins)
    {
      versionControlRequestContext.RequestContext.Trace(700139, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2} {3} {4}", (object) nameof (CheckItemPermission), (object) permissionRequired, (object) itemPathPair.ProjectNamePath, (object) itemPathPair.ProjectGuidPath, (object) alwaysAllowAdmins);
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      if ((permissionRequired & VersionedItemPermissions.Read) == VersionedItemPermissions.Read && (permissionRequired & ~VersionedItemPermissions.Read) != VersionedItemPermissions.None && !securityNamespace.HasPermission(versionControlRequestContext.RequestContext, token, (int) permissionRequired, alwaysAllowAdmins))
      {
        versionControlRequestContext.RequestContext.Trace(700140, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Checking for read permission first.");
        securityNamespace.CheckPermission(versionControlRequestContext.RequestContext, token, 1, alwaysAllowAdmins);
        versionControlRequestContext.RequestContext.Trace(700141, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Read check succeeded, now checking the rest of the permissions.");
        securityNamespace.CheckPermission(versionControlRequestContext.RequestContext, token, (int) (permissionRequired & ~VersionedItemPermissions.Read), alwaysAllowAdmins);
      }
      else
      {
        versionControlRequestContext.RequestContext.Trace(700142, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Checking all permissions together.");
        securityNamespace.CheckPermission(versionControlRequestContext.RequestContext, token, (int) permissionRequired, alwaysAllowAdmins);
      }
      versionControlRequestContext.RequestContext.Trace(700143, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (CheckItemPermission), (object) "Permission Granted");
    }

    public void CheckWorkspacePermission(
      VersionControlRequestContext versionControlRequestContext,
      int permissionRequired,
      Workspace workspace)
    {
      versionControlRequestContext.RequestContext.Trace(700144, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) nameof (CheckWorkspacePermission), (object) permissionRequired, (object) workspace.SecurityToken);
      if (IdentityDescriptorComparer.Instance.Equals(workspace.Owner.Descriptor, versionControlRequestContext.RequestContext.UserContext))
      {
        versionControlRequestContext.RequestContext.Trace(700144, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (CheckWorkspacePermission), (object) "User is owner of workspace. Permission Granted.");
      }
      else
      {
        if (versionControlRequestContext.RequestContext.IsTracing(700144, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic))
          versionControlRequestContext.RequestContext.Trace(700144, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "User '{0}', '{1}' is not workspace owner '{2}', '{3}'", (object) versionControlRequestContext.RequestContext.UserContext, (object) versionControlRequestContext.RequestContext.GetUserIdentity().Id, (object) workspace.Owner.Descriptor, (object) workspace.Owner.Id);
        versionControlRequestContext.GetWorkspaceSecurity().CheckPermission(versionControlRequestContext.RequestContext, workspace.SecurityToken, permissionRequired);
        versionControlRequestContext.RequestContext.Trace(700144, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (CheckWorkspacePermission), (object) "Permission Granted.");
      }
    }

    public void CheckItemPermissionForAllChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      string serverItem)
    {
      this.CheckItemPermissionForAllChildren(versionControlRequestContext, permissionRequired, ItemPathPair.FromServerItem(serverItem));
    }

    public void CheckItemPermissionForAllChildren(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions permissionRequired,
      ItemPathPair itemPathPair)
    {
      versionControlRequestContext.RequestContext.Trace(700147, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2} {3}", (object) nameof (CheckItemPermissionForAllChildren), (object) permissionRequired, (object) itemPathPair.ProjectNamePath, (object) itemPathPair.ProjectGuidPath);
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      if (!securityNamespace.HasPermissionForAllChildren(versionControlRequestContext.RequestContext, token, (int) permissionRequired, alwaysAllowAdministrators: false))
        throw new ResourceAccessException(versionControlRequestContext.RequestContext.GetUserId().ToString(), permissionRequired.ToString(), VersionControlPath.Combine(itemPathPair.ProjectNamePath, "*"));
      versionControlRequestContext.RequestContext.Trace(700147, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (CheckItemPermissionForAllChildren), (object) "Permission Granted.");
    }

    public bool HasWorkspacePermission(
      VersionControlRequestContext versionControlRequestContext,
      int permissionRequired,
      Workspace workspace)
    {
      versionControlRequestContext.RequestContext.Trace(700149, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) nameof (HasWorkspacePermission), (object) permissionRequired, (object) workspace.Id);
      if (IdentityDescriptorComparer.Instance.Equals(workspace.Owner.Descriptor, versionControlRequestContext.RequestContext.UserContext))
      {
        versionControlRequestContext.RequestContext.Trace(700150, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (HasWorkspacePermission), (object) "Caller is owner of workspace.");
        return true;
      }
      bool flag = versionControlRequestContext.GetWorkspaceSecurity().HasPermission(versionControlRequestContext.RequestContext, workspace.SecurityToken, permissionRequired);
      versionControlRequestContext.RequestContext.Trace(700151, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (HasWorkspacePermission), (object) flag);
      return flag;
    }

    public List<string> QueryEffectiveGlobalPermissions(
      VersionControlRequestContext versionControlRequestContext,
      IdentityDescriptor user,
      string identityName)
    {
      versionControlRequestContext.RequestContext.Trace(700152, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) nameof (QueryEffectiveGlobalPermissions), (object) user, (object) identityName);
      Microsoft.VisualStudio.Services.Identity.Identity identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, identityName);
      int bits = versionControlRequestContext.GetPrivilegeSecurity().QueryEffectivePermissions(versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, (EvaluationPrincipal) identity.Descriptor);
      versionControlRequestContext.RequestContext.Trace(700153, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1} {2}", (object) nameof (QueryEffectiveGlobalPermissions), (object) identity, (object) bits);
      return TFCommonUtil.TranslateEnum(typeof (GlobalPermissions), bits);
    }

    public List<string> QueryEffectiveItemPermissions(
      VersionControlRequestContext versionControlRequestContext,
      string serverItem,
      string identityName)
    {
      return this.QueryEffectiveItemPermissions(versionControlRequestContext, ItemPathPair.FromServerItem(serverItem), identityName);
    }

    public List<string> QueryEffectiveItemPermissions(
      VersionControlRequestContext versionControlRequestContext,
      ItemPathPair itemPathPair,
      string identityName)
    {
      versionControlRequestContext.RequestContext.Trace(700154, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Entering {0}: {1} {2} {3}", (object) nameof (QueryEffectiveItemPermissions), (object) itemPathPair.ProjectNamePath, (object) itemPathPair.ProjectGuidPath, (object) identityName);
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, itemPathPair, true);
      Microsoft.VisualStudio.Services.Identity.Identity identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, identityName);
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      int bits = securityNamespace.QueryEffectivePermissions(versionControlRequestContext.RequestContext, token, (EvaluationPrincipal) identity.Descriptor);
      versionControlRequestContext.RequestContext.Trace(700155, TraceLevel.Verbose, TraceArea.Security, TraceLayer.BusinessLogic, "Leaving {0}: {1} {2}", (object) nameof (QueryEffectiveItemPermissions), (object) identity, (object) bits);
      return TFCommonUtil.TranslateEnum(typeof (VersionedItemPermissions), bits);
    }

    private List<ItemSecurity> QueryItemPermissions(
      VersionControlRequestContext versionControlRequestContext,
      ArrayList specs,
      Microsoft.VisualStudio.Services.Identity.Identity[] identities,
      Workspace workspace)
    {
      List<ItemSecurity> itemSecurityList = new List<ItemSecurity>(specs.Count);
      List<IdentityDescriptor> descriptors = (List<IdentityDescriptor>) null;
      if (identities != null && identities.Length != 0)
      {
        descriptors = new List<IdentityDescriptor>();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
          descriptors.Add(identity.Descriptor);
      }
      foreach (ItemSpec spec in specs)
      {
        ItemPathPair serverItem = spec.toServerItem(versionControlRequestContext.RequestContext, workspace);
        foreach (IAccessControlList accessControlList in versionControlRequestContext.GetRepositorySecurity().QueryAccessControlLists(versionControlRequestContext.RequestContext, serverItem.ProjectGuidPath ?? serverItem.ProjectNamePath, (IEnumerable<IdentityDescriptor>) descriptors, true, spec.RecursionType == RecursionType.Full))
        {
          if (versionControlRequestContext.GetRepositorySecurity().HasPermission(versionControlRequestContext.RequestContext, accessControlList.Token, 1))
          {
            ItemSecurity itemSecurity = new ItemSecurity()
            {
              ServerItem = accessControlList.Token.Equals("$") ? "$/" : accessControlList.Token
            };
            itemSecurity.Writable = versionControlRequestContext.GetRepositorySecurity().HasPermission(versionControlRequestContext.RequestContext, itemSecurity.ServerItem, 1024);
            itemSecurity.Inherit = accessControlList.InheritPermissions;
            List<AccessEntry> accessEntryList = new List<AccessEntry>();
            foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
            {
              AccessEntry accessEntry = new AccessEntry(accessControlEntry.Allow, accessControlEntry.Deny, accessControlEntry.InheritedAllow, accessControlEntry.InheritedDeny, IdentityHelper.GetUniqueName(IdentityHelper.GetIdentityFromList(this.IdentityService.ReadIdentities(versionControlRequestContext.RequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
              {
                accessControlEntry.Descriptor
              }, QueryMembership.None, (IEnumerable<string>) null), accessControlEntry.Descriptor.Identifier) ?? throw new IdentityNotFoundException("IdentityNotFoundException", versionControlRequestContext.RequestContext.AuthenticatedUserName)));
              accessEntry.Allow = TFCommonUtil.TranslateEnum(typeof (VersionedItemPermissions), accessEntry.allowBits);
              accessEntry.Deny = TFCommonUtil.TranslateEnum(typeof (VersionedItemPermissions), accessEntry.denyBits);
              accessEntry.AllowInherited = TFCommonUtil.TranslateEnum(typeof (VersionedItemPermissions), accessEntry.allowInheritedBits);
              accessEntry.DenyInherited = TFCommonUtil.TranslateEnum(typeof (VersionedItemPermissions), accessEntry.denyInheritedBits);
              accessEntryList.Add(accessEntry);
            }
            itemSecurity.Entries = accessEntryList.ToArray();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            Array.Sort<AccessEntry>(itemSecurity.Entries, SecurityManager.\u003C\u003EO.\u003C0\u003E__CompareIdentity ?? (SecurityManager.\u003C\u003EO.\u003C0\u003E__CompareIdentity = new Comparison<AccessEntry>(AccessEntry.CompareIdentity)));
            itemSecurityList.Add(itemSecurity);
          }
        }
      }
      return itemSecurityList;
    }

    internal void UpdateGlobalPermission(
      VersionControlRequestContext versionControlRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      GlobalPermissions allow,
      GlobalPermissions deny,
      GlobalPermissions remove)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckGlobalPermission(versionControlRequestContext, GlobalPermissions.AdminConfiguration);
      if (remove != (GlobalPermissions) 0)
        versionControlRequestContext.GetPrivilegeSecurity().RemovePermissions(versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, identity.Descriptor, (int) remove);
      if (allow == (GlobalPermissions) 0 && deny == (GlobalPermissions) 0)
        return;
      versionControlRequestContext.GetPrivilegeSecurity().SetPermissions(versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, identity.Descriptor, (int) allow, (int) deny, true);
    }

    public void UpdateItemInheritFlag(
      VersionControlRequestContext versionControlRequestContext,
      string serverItem,
      bool inheritFlag)
    {
      this.UpdateItemInheritFlag(versionControlRequestContext, ItemPathPair.FromServerItem(serverItem), inheritFlag);
    }

    public void UpdateItemInheritFlag(
      VersionControlRequestContext versionControlRequestContext,
      ItemPathPair itemPathPair,
      bool inheritFlag)
    {
      if (VersionControlPath.IsRootFolder(itemPathPair.ProjectNamePath))
        throw new RootInheritanceException();
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, itemPathPair, true);
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      securityNamespace.SetInheritFlag(versionControlRequestContext.RequestContext, token, inheritFlag);
    }

    internal GlobalSecurity QueryGlobalPermissions(
      VersionControlRequestContext versionControlRequestContext,
      string[] identityNames)
    {
      GlobalSecurity globalSecurity = new GlobalSecurity();
      globalSecurity.Writable = versionControlRequestContext.GetPrivilegeSecurity().HasPermission(versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, 32);
      List<AccessEntry> accessEntryList = new List<AccessEntry>();
      List<IdentityDescriptor> descriptors = (List<IdentityDescriptor>) null;
      if (identityNames != null && identityNames.Length != 0)
      {
        descriptors = new List<IdentityDescriptor>();
        foreach (string identityName in identityNames)
        {
          try
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, identityName);
            descriptors.Add(identity.Descriptor);
          }
          catch (AuthorizationException ex)
          {
            versionControlRequestContext.RequestContext.TraceException(700156, TraceLevel.Info, TraceArea.Identities, TraceLayer.BusinessLogic, (Exception) ex);
          }
        }
        if (descriptors.Count == 0)
        {
          globalSecurity.Entries = accessEntryList;
          return globalSecurity;
        }
      }
      Dictionary<IdentityDescriptor, AccessEntry> dictionary = new Dictionary<IdentityDescriptor, AccessEntry>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (IAccessControlEntry accessControlEntry in versionControlRequestContext.GetPrivilegeSecurity().QueryAccessControlList(versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, (IEnumerable<IdentityDescriptor>) descriptors, true).AccessControlEntries)
      {
        AccessEntry accessEntry = new AccessEntry();
        Microsoft.VisualStudio.Services.Identity.Identity identityFromList = IdentityHelper.GetIdentityFromList(this.IdentityService.ReadIdentities(versionControlRequestContext.RequestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
        {
          accessControlEntry.Descriptor
        }, QueryMembership.None, (IEnumerable<string>) null), accessControlEntry.Descriptor.Identifier);
        accessEntry.IdentityName = identityFromList != null ? IdentityHelper.GetDomainUserName(identityFromList) : throw new IdentityNotFoundException("IdentityNotFoundException", versionControlRequestContext.RequestContext.AuthenticatedUserName);
        accessEntry.DisplayName = identityFromList.DisplayName;
        accessEntry.allowBits = accessControlEntry.Allow;
        accessEntry.denyBits = accessControlEntry.Deny;
        dictionary[accessControlEntry.Descriptor] = accessEntry;
        accessEntryList.Add(accessEntry);
      }
      foreach (AccessEntry accessEntry in accessEntryList)
      {
        accessEntry.Allow = TFCommonUtil.TranslateEnum(typeof (GlobalPermissions), accessEntry.allowBits);
        accessEntry.Deny = TFCommonUtil.TranslateEnum(typeof (GlobalPermissions), accessEntry.denyBits);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      accessEntryList.Sort(SecurityManager.\u003C\u003EO.\u003C0\u003E__CompareIdentity ?? (SecurityManager.\u003C\u003EO.\u003C0\u003E__CompareIdentity = new Comparison<AccessEntry>(AccessEntry.CompareIdentity)));
      globalSecurity.Entries = accessEntryList;
      return globalSecurity;
    }

    internal List<ItemSecurity> QueryItemPermissions(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      ItemSpec[] itemSpecs,
      string[] identityNames,
      List<Failure> failures)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] identities = (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
      if (identityNames != null)
      {
        identities = new Microsoft.VisualStudio.Services.Identity.Identity[identityNames.Length];
        for (int index = 0; index < identityNames.Length; ++index)
          identities[index] = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, identityNames[index]);
      }
      ArrayList specs = new ArrayList(itemSpecs.Length);
      foreach (ItemSpec itemSpec in itemSpecs)
      {
        try
        {
          if (itemSpec.isWildcard)
            failures.Add(new Failure((Exception) new WildcardNotAllowedException("WildcardNotAllowedException", new object[1]
            {
              (object) itemSpec.Item
            })));
          else
            specs.Add((object) itemSpec);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (ApplicationException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700157, TraceLevel.Info, TraceArea.Identities, TraceLayer.BusinessLogic, (Exception) ex);
          failures.Add(new Failure((Exception) ex, itemSpec.Item, RequestType.None));
        }
      }
      return this.QueryItemPermissions(versionControlRequestContext, specs, identities, workspace);
    }

    internal List<PermissionChange> UpdateGlobalSecurity(
      VersionControlRequestContext versionControlRequestContext,
      PermissionChange[] changes,
      List<Failure> failures)
    {
      List<PermissionChange> permissionChangeList = new List<PermissionChange>();
      foreach (PermissionChange change in changes)
      {
        GlobalPermissions allow = (GlobalPermissions) VersionControlUtil.TranslatePermission(typeof (GlobalPermissions), change.Allow, 62);
        GlobalPermissions deny = (GlobalPermissions) VersionControlUtil.TranslatePermission(typeof (GlobalPermissions), change.Deny, 62);
        GlobalPermissions remove = (GlobalPermissions) VersionControlUtil.TranslatePermission(typeof (GlobalPermissions), change.Remove, 62);
        try
        {
          if (change.m_identity == null)
          {
            change.m_identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, change.IdentityName);
            if (!change.m_identity.IsActive && (allow != (GlobalPermissions) 0 || deny != (GlobalPermissions) 0))
              throw new IdentityNotFoundException("IdentityNotFoundException", change.IdentityName);
          }
          this.UpdateGlobalPermission(versionControlRequestContext, change.m_identity, allow, deny, remove);
          permissionChangeList.Add(change);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700158, TraceArea.Permissions, TraceLayer.BusinessLogic, ex);
          failures.Add(new Failure(ex));
        }
      }
      return permissionChangeList;
    }

    internal List<SecurityChange> UpdateItemSecurity(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      SecurityChange[] changes,
      List<Failure> failures)
    {
      List<SecurityChange> securityChangeList = new List<SecurityChange>(changes.Length);
      foreach (SecurityChange change in changes)
      {
        try
        {
          ItemPathPair serverItem = new ItemSpec(change.Item, RecursionType.None).toServerItem(versionControlRequestContext.RequestContext, workspace);
          this.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, serverItem, true);
          switch (change)
          {
            case InheritanceChange _:
              InheritanceChange inheritanceChange = (InheritanceChange) change;
              this.UpdateItemInheritFlag(versionControlRequestContext, serverItem, inheritanceChange.Inherit);
              break;
            case PermissionChange _:
              PermissionChange permissionChange = (PermissionChange) change;
              VersionedItemPermissions allow = (VersionedItemPermissions) VersionControlUtil.TranslatePermission(typeof (VersionedItemPermissions), permissionChange.Allow, 15871);
              VersionedItemPermissions deny = (VersionedItemPermissions) VersionControlUtil.TranslatePermission(typeof (VersionedItemPermissions), permissionChange.Deny, 15871);
              VersionedItemPermissions remove = (VersionedItemPermissions) VersionControlUtil.TranslatePermission(typeof (VersionedItemPermissions), permissionChange.Remove, 15871);
              if (allow == VersionedItemPermissions.None && allow == deny && allow == remove)
                throw new ArgumentException(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get("EmptyPermissionsOrInheritanceError"));
              if (permissionChange.m_identity == null)
              {
                permissionChange.m_identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, permissionChange.IdentityName);
                if (!permissionChange.m_identity.IsActive && (allow != VersionedItemPermissions.None || deny != VersionedItemPermissions.None))
                  throw new IdentityNotFoundException("IdentityNotFoundException", permissionChange.IdentityName);
              }
              this.UpdateItemPermission(versionControlRequestContext, serverItem, permissionChange.m_identity, allow, deny, remove);
              break;
            default:
              continue;
          }
          securityChangeList.Add(change);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700159, TraceArea.Permissions, TraceLayer.BusinessLogic, ex);
          failures.Add(new Failure(ex, change.Item, RequestType.None));
        }
      }
      return securityChangeList;
    }

    internal void UpdateItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      string serverItem,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      VersionedItemPermissions allow,
      VersionedItemPermissions deny,
      VersionedItemPermissions remove)
    {
      this.UpdateItemPermission(versionControlRequestContext, ItemPathPair.FromServerItem(serverItem), identity, allow, deny, remove);
    }

    internal void UpdateItemPermission(
      VersionControlRequestContext versionControlRequestContext,
      ItemPathPair itemPathPair,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      VersionedItemPermissions allow,
      VersionedItemPermissions deny,
      VersionedItemPermissions remove)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, itemPathPair, true);
      string token;
      IVssSecurityNamespace securityNamespace;
      if (itemPathPair.ProjectGuidPath != null)
      {
        token = itemPathPair.ProjectGuidPath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity2();
      }
      else
      {
        token = itemPathPair.ProjectNamePath;
        securityNamespace = versionControlRequestContext.GetRepositorySecurity();
      }
      if (remove != VersionedItemPermissions.None)
        securityNamespace.RemovePermissions(versionControlRequestContext.RequestContext, token, identity.Descriptor, (int) remove);
      if (allow == VersionedItemPermissions.None && deny == VersionedItemPermissions.None)
        return;
      securityNamespace.SetPermissions(versionControlRequestContext.RequestContext, token, identity.Descriptor, (int) allow, (int) deny, true);
    }

    internal IIdentityServiceInternal IdentityService { get; set; }
  }
}
