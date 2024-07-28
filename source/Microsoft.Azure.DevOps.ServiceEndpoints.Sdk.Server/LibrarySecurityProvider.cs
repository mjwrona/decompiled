// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.LibrarySecurityProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public abstract class LibrarySecurityProvider
  {
    public static readonly char NamespaceSeparator = '/';
    public static readonly Guid LibraryNamespaceId = new Guid("B7E84409-6553-448A-BBB2-AF228E07CBEB");
    public static readonly string Library = nameof (Library);
    public static readonly string Collection = nameof (Collection);
    public static readonly string ServiceEndpointRoleScopeId = "distributedtask.serviceendpointrole";
    public static readonly string ServiceEndpointCollectionRoleScopeId = "distributedtask.collection.serviceendpointrole";
    public static readonly string ServiceEndpointProjectRoleScopeId = "distributedtask.project.serviceendpointrole";
    private const string c_LibraryPermissionsInitializedKey = "/Service/DistributedTask/Settings/Library/PermissionsInitialized/";

    public virtual void CheckAndInitializeLibraryPermissions(
      IVssRequestContext requestContext,
      Guid? projectId)
    {
      if (this.IsLibraryPermissionsInitialized(requestContext, projectId))
        return;
      this.InitializeLibraryPermissions(requestContext, projectId.Value);
    }

    public virtual void CheckCreatePermissions(
      IVssRequestContext requestContext,
      Guid? projectId,
      string itemType,
      bool checkBothProjectAndCollectionScope = false)
    {
      this.CheckAndInitializeLibraryPermissions(requestContext, projectId);
      if (this.HasPermissions(requestContext, projectId, (string) null, 2, checkBothProjectAndCollectionScope: checkBothProjectAndCollectionScope))
        return;
      this.CheckPermissions(requestContext, projectId, string.Empty, 4, false, this.GetErrorMessage(itemType), checkBothProjectAndCollectionScope);
    }

    public virtual void CheckPermissions(
      IVssRequestContext requestContext,
      Guid? projectId,
      string itemId,
      int permissions,
      bool alwaysAllowAdministrators,
      string errorMessage,
      bool checkBothProjectAndCollectionScope = false)
    {
      if (!this.HasPermissions(requestContext, projectId, itemId, permissions, alwaysAllowAdministrators, checkBothProjectAndCollectionScope))
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        throw new AccessCheckException(userIdentity.Descriptor, userIdentity.DisplayName, itemId, permissions, LibrarySecurityProvider.LibraryNamespaceId, errorMessage);
      }
    }

    public virtual bool HasPermissions(
      IVssRequestContext requestContext,
      Guid? projectId,
      string itemId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false,
      bool checkBothProjectAndCollectionScope = false)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId);
      bool flag = securityNamespace.HasPermission(requestContext, this.GetToken(projectId, itemId), requiredPermissions, alwaysAllowAdministrators);
      if (!flag & checkBothProjectAndCollectionScope)
        flag = securityNamespace.HasPermission(requestContext, this.GetToken(new Guid?(), itemId), requiredPermissions, alwaysAllowAdministrators);
      return flag;
    }

    public virtual void RemoveAccessControlLists(
      IVssRequestContext requestContext,
      Guid projectId,
      string itemId)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId).RemoveAccessControlLists(requestContext, (IEnumerable<string>) new string[1]
      {
        this.GetToken(new Guid?(projectId), itemId)
      }, true);
    }

    public virtual void RemoveCollectionLevelAccessControlLists(
      IVssRequestContext requestContext,
      string itemId)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId).RemoveAccessControlLists(requestContext, (IEnumerable<string>) new string[1]
      {
        this.GetToken(new Guid?(), itemId)
      }, true);
    }

    public void AddLibraryItemCreatorAsItemAdministrator(
      IVssRequestContext requestContext,
      Guid? projectId,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      string itemId)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId);
      List<IAccessControlEntry> aces = new List<IAccessControlEntry>()
      {
        this.CreateLibraryAdminPermission(userIdentity.Descriptor)
      };
      Microsoft.TeamFoundation.Framework.Server.AccessControlList accessControlList = new Microsoft.TeamFoundation.Framework.Server.AccessControlList(this.GetToken(projectId, itemId), true, (IEnumerable<IAccessControlEntry>) aces);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      securityNamespace.SetAccessControlLists(requestContext1, (IEnumerable<IAccessControlList>) new List<Microsoft.TeamFoundation.Framework.Server.AccessControlList>()
      {
        accessControlList
      });
    }

    public void AddLibraryItemCreatorAsCollectionAdmin(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      string itemId)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId);
      IList<IAccessControlEntry> collectionAdminRoleAce = this.GetCollectionAdminRoleAce(userIdentity);
      if (collectionAdminRoleAce.IsNullOrEmpty<IAccessControlEntry>())
        return;
      Microsoft.TeamFoundation.Framework.Server.AccessControlList accessControlList = new Microsoft.TeamFoundation.Framework.Server.AccessControlList(this.GetToken(new Guid?(), itemId), true, (IEnumerable<IAccessControlEntry>) collectionAdminRoleAce);
      securityNamespace.SetAccessControlLists(requestContext.Elevate(), (IEnumerable<IAccessControlList>) new List<Microsoft.TeamFoundation.Framework.Server.AccessControlList>()
      {
        accessControlList
      });
    }

    public void AddVariableGroupAdminAtCollectionLevel(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      string itemId)
    {
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId);
      string token1 = this.GetToken(new Guid?(), itemId);
      Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry1 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identityDescriptor, 19, 0);
      IVssRequestContext requestContext1 = requestContext;
      string token2 = token1;
      Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry2 = accessControlEntry1;
      securityNamespace.SetAccessControlEntry(requestContext1, token2, (IAccessControlEntry) accessControlEntry2, true);
    }

    public void CheckFrameworkReadPermissions(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);

    protected abstract string GetTokenSuffix(string itemId);

    protected virtual IList<IAccessControlEntry> GetCollectionAdminRoleAce(Microsoft.VisualStudio.Services.Identity.Identity userIdentity) => (IList<IAccessControlEntry>) new List<IAccessControlEntry>();

    private string GetErrorMessage(string itemType) => !(itemType == ServiceEndpointSdkResources.OAuthConfiguration()) ? ServiceEndpointSdkResources.LibraryItemAccessDeniedForCreate((object) itemType) : ServiceEndpointSdkResources.OAuthConfigurationAccessDeniedForCreate();

    private string GetToken(Guid? projectId, string itemToken) => LibrarySecurityProvider.GetRootToken(projectId) + (object) LibrarySecurityProvider.NamespaceSeparator + this.GetTokenSuffix(itemToken);

    public static string GetRootToken(Guid? projectId) => !projectId.HasValue ? LibrarySecurityProvider.Library + (object) LibrarySecurityProvider.NamespaceSeparator + LibrarySecurityProvider.Collection : LibrarySecurityProvider.Library + (object) LibrarySecurityProvider.NamespaceSeparator + (object) projectId;

    public bool IsLibraryPermissionsInitialized(IVssRequestContext requestContext, Guid? projectId)
    {
      if (!projectId.HasValue)
        return true;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      if (service.GetValue<bool>(vssRequestContext, (RegistryQuery) this.GetPermissionInitializedKey(projectId.Value), false))
        return true;
      IAccessControlList accessControlList = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, LibrarySecurityProvider.LibraryNamespaceId).QueryAccessControlList(vssRequestContext, LibrarySecurityProvider.GetRootToken(projectId), (IEnumerable<IdentityDescriptor>) null, false);
      if (accessControlList == null || accessControlList.AccessControlEntries == null || accessControlList.AccessControlEntries.Count<IAccessControlEntry>() <= 1)
        return false;
      service.SetValue<bool>(vssRequestContext, this.GetPermissionInitializedKey(projectId.Value), true);
      return true;
    }

    private string GetPermissionInitializedKey(Guid projectId) => "/Service/DistributedTask/Settings/Library/PermissionsInitialized/" + (object) projectId;

    private void InitializeLibraryPermissions(IVssRequestContext requestContext, Guid projectId)
    {
      List<IAccessControlList> accessControlListList1 = new List<IAccessControlList>();
      List<IAccessControlEntry> aces = new List<IAccessControlEntry>();
      this.AddProjectDefaultGroupPermissions(requestContext, (IList<IAccessControlEntry>) aces, projectId);
      if (aces.Count <= 0)
        return;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, LibrarySecurityProvider.LibraryNamespaceId);
      accessControlListList1.Add((IAccessControlList) new Microsoft.TeamFoundation.Framework.Server.AccessControlList(LibrarySecurityProvider.GetRootToken(new Guid?(projectId)), true, (IEnumerable<IAccessControlEntry>) aces));
      IVssRequestContext requestContext1 = requestContext.Elevate();
      List<IAccessControlList> accessControlListList2 = accessControlListList1;
      securityNamespace.SetAccessControlLists(requestContext1, (IEnumerable<IAccessControlList>) accessControlListList2);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<IVssRegistryService>().SetValue<bool>(vssRequestContext, this.GetPermissionInitializedKey(projectId), true);
    }

    private void AddProjectDefaultGroupPermissions(
      IVssRequestContext requestContext,
      IList<IAccessControlEntry> aces,
      Guid projectId)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = service.ListGroups(requestContext1, new Guid[1]
      {
        projectId
      }, false, (IEnumerable<string>) null);
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source2 = source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => group.GetProperty<string>("Account", string.Empty).Equals(ServiceEndpointSdkResources.ContributorGroup(), StringComparison.OrdinalIgnoreCase)));
      if (!(source2 is IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList))
        identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source2.ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
        aces.Add((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identity.Descriptor, 5, 0));
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in source1.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group =>
      {
        string property = group.GetProperty<string>("Account", string.Empty);
        return property.Equals(ServiceEndpointSdkResources.ProjectReleaseAdminAccountName(), StringComparison.OrdinalIgnoreCase) || property.Equals(ServiceEndpointSdkResources.ProjectBuildAdminAccountName(), StringComparison.OrdinalIgnoreCase) || property.Equals(ServiceEndpointSdkResources.ProjectReleaseManagerGroupName(), StringComparison.OrdinalIgnoreCase);
      })))
        aces.Add(this.CreateLibraryAdminPermission(identity.Descriptor));
      try
      {
        IdentityScope scope = service.GetScope(requestContext1, projectId);
        if (scope != null)
          aces.Add(this.CreateLibraryAdminPermission(scope.Administrators));
      }
      catch (GroupScopeDoesNotExistException ex)
      {
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = service.GetGroups(requestContext1, projectId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.EveryoneGroup
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity1 == null)
        return;
      aces.Add((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identity1.Descriptor, 1, 0));
    }

    private IAccessControlEntry CreateLibraryAdminPermission(IdentityDescriptor descriptor) => (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor, 23, 0);
  }
}
