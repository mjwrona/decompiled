// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.RepositoryShimSecurityNamespace
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class RepositoryShimSecurityNamespace : 
    IVssSecurityNamespace,
    IMutableSecurityNamespace,
    IQueryableSecurityNamespace,
    IQueryableSecurityNamespaceInternal
  {
    private readonly SecurityNamespaceDescription m_description;
    private readonly IVssSecurityNamespace m_securityNamespace;
    private const string c_name = "VersionControlItems";

    public RepositoryShimSecurityNamespace(IVssSecurityNamespace securityNamespace)
    {
      ArgumentUtility.CheckForNull<IVssSecurityNamespace>(securityNamespace, nameof (securityNamespace));
      SecurityNamespaceDescription namespaceDescription = new SecurityNamespaceDescription();
      namespaceDescription.NamespaceId = SecurityConstants.RepositorySecurityNamespaceGuid;
      namespaceDescription.Name = "VersionControlItems";
      namespaceDescription.DisplayName = "VersionControlItems";
      namespaceDescription.DataspaceCategory = securityNamespace.Description.DataspaceCategory;
      namespaceDescription.SeparatorValue = securityNamespace.Description.SeparatorValue;
      namespaceDescription.ElementLength = securityNamespace.Description.ElementLength;
      namespaceDescription.NamespaceStructure = securityNamespace.Description.NamespaceStructure;
      namespaceDescription.ReadPermission = securityNamespace.Description.ReadPermission;
      namespaceDescription.WritePermission = securityNamespace.Description.WritePermission;
      namespaceDescription.Actions.AddRange((IEnumerable<ActionDefinition>) securityNamespace.Description.Actions);
      namespaceDescription.ExtensionType = securityNamespace.Description.ExtensionType;
      namespaceDescription.IsRemotable = true;
      this.m_description = namespaceDescription;
      this.m_securityNamespace = securityNamespace;
    }

    public SecurityNamespaceDescription Description => this.m_description;

    public ISecurityNamespaceExtension NamespaceExtension => this.m_securityNamespace.NamespaceExtension;

    public void OnDataChanged(IVssRequestContext requestContext) => this.m_securityNamespace.OnDataChanged(requestContext);

    public bool PollForRequestLocalInvalidation(IVssRequestContext requestContext) => this.m_securityNamespace.PollForRequestLocalInvalidation(requestContext);

    public void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      EvaluationPrincipal failingPrincipal = null)
    {
      this.m_securityNamespace.ThrowAccessDeniedException(requestContext, token, requestedPermissions, failingPrincipal);
    }

    public IQueryableAclStore GetQueryableAclStore(
      IVssRequestContext requestContext,
      Guid aclStoreId)
    {
      IQueryableAclStore aclStore = this.m_securityNamespace.GetQueryableAclStore(requestContext, aclStoreId);
      if (aclStore != null)
        aclStore = (IQueryableAclStore) new RepositoryShimSecurityNamespace.ShimQueryableAclStore(aclStore);
      return aclStore;
    }

    public IEnumerable<IAccessControlEntry> SetAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IAccessControlEntry> accessControlEntries,
      bool merge,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      return this.m_securityNamespace.SetAccessControlEntries(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), accessControlEntries, merge, throwOnInvalidIdentity, rootNewIdentities);
    }

    public void SetAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> accessControlLists,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      this.m_securityNamespace.SetAccessControlLists(requestContext, this.TranslateIncomingAccessControlLists(requestContext, accessControlLists), throwOnInvalidIdentity, rootNewIdentities);
    }

    public void SetInheritFlag(IVssRequestContext requestContext, string token, bool inherit) => this.m_securityNamespace.SetInheritFlag(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), inherit);

    public bool RemoveAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      return this.m_securityNamespace.RemoveAccessControlEntries(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), descriptors);
    }

    public bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      bool recurse)
    {
      return this.m_securityNamespace.RemoveAccessControlLists(requestContext, this.TranslateIncomingTokens(requestContext, tokens), recurse);
    }

    public void RenameToken(
      IVssRequestContext requestContext,
      string existingToken,
      string newToken,
      bool copy)
    {
      this.m_securityNamespace.RenameToken(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, existingToken), RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, newToken), copy);
    }

    public void RenameTokens(
      IVssRequestContext requestContext,
      IEnumerable<TokenRename> renameTokens)
    {
      this.m_securityNamespace.RenameTokens(requestContext, this.TranslateIncomingTokenRenames(requestContext, renameTokens));
    }

    public IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse)
    {
      IEnumerable<QueriedAccessControlList> acls = this.m_securityNamespace.QueryAccessControlLists(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), evaluationPrincipals, includeExtendedInfo, recurse);
      return RepositoryShimSecurityNamespace.TranslateOutgoingAccessControlLists(requestContext, acls);
    }

    void IQueryableSecurityNamespaceInternal.CheckRequestContext(IVssRequestContext requestContext) => this.m_securityNamespace.QueryableSecurityNamespaceInternal().CheckRequestContext(requestContext);

    void IQueryableSecurityNamespaceInternal.QueryEffectivePermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      out int effectiveAllow,
      out int effectiveDeny,
      int bitsToConsider)
    {
      this.m_securityNamespace.QueryableSecurityNamespaceInternal().QueryEffectivePermissions(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), evaluationPrincipal, out effectiveAllow, out effectiveDeny, bitsToConsider);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermission(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermission(requestContext, principal, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), requestedPermissions, alwaysAllowAdministrators);
    }

    bool? IQueryableSecurityNamespaceInternal.GetPrincipalPermissionState(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().GetPrincipalPermissionState(requestContext, principal, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), requestedPermissions, alwaysAllowAdministrators);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermissionForAllChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators)
    {
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermissionForAllChildren(requestContext, principal, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermissionForAnyChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators)
    {
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermissionForAnyChildren(requestContext, principal, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators);
    }

    private static string TranslateIncomingToken(IVssRequestContext requestContext, string token)
    {
      string convertedPath;
      ProjectUtility.TryConvertToPathWithProjectId(requestContext, token, out convertedPath, out Guid _, out string _);
      return convertedPath;
    }

    private static string TranslateOutgoingToken(IVssRequestContext requestContext, string token)
    {
      string convertedPath;
      ProjectUtility.TryConvertToPathWithProjectName(requestContext, token, out convertedPath, out Guid _, out string _);
      return convertedPath;
    }

    private IEnumerable<string> TranslateIncomingTokens(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens)
    {
      return tokens != null ? tokens.Select<string, string>((Func<string, string>) (token => RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token))) : (IEnumerable<string>) null;
    }

    private TokenRename TranslateIncomingTokenRename(
      IVssRequestContext requestContext,
      TokenRename renameToken)
    {
      return new TokenRename(RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, renameToken.OldToken), RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, renameToken.NewToken), renameToken.Copy, renameToken.Recurse);
    }

    private IEnumerable<TokenRename> TranslateIncomingTokenRenames(
      IVssRequestContext requestContext,
      IEnumerable<TokenRename> renameTokens)
    {
      return renameTokens != null ? renameTokens.Select<TokenRename, TokenRename>((Func<TokenRename, TokenRename>) (renameToken => this.TranslateIncomingTokenRename(requestContext, renameToken))) : (IEnumerable<TokenRename>) null;
    }

    private IEnumerable<IAccessControlList> TranslateIncomingAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> acls)
    {
      return acls != null ? acls.Select<IAccessControlList, IAccessControlList>((Func<IAccessControlList, IAccessControlList>) (acl =>
      {
        IAccessControlList accessControlList = (IAccessControlList) acl.Clone();
        accessControlList.Token = RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, acl.Token);
        return accessControlList;
      })) : (IEnumerable<IAccessControlList>) null;
    }

    private static IEnumerable<QueriedAccessControlList> TranslateOutgoingAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<QueriedAccessControlList> acls)
    {
      return acls != null ? acls.Select<QueriedAccessControlList, QueriedAccessControlList>((Func<QueriedAccessControlList, QueriedAccessControlList>) (acl => new QueriedAccessControlList(RepositoryShimSecurityNamespace.TranslateOutgoingToken(requestContext, acl.Token), acl.Inherit, acl.AccessControlEntries))) : (IEnumerable<QueriedAccessControlList>) null;
    }

    private static IEnumerable<EnumeratedPermission> TranslateOutgoingEnumeratedPermissions(
      IVssRequestContext requestContext,
      IEnumerable<EnumeratedPermission> enumeratedPermissions)
    {
      return enumeratedPermissions != null ? enumeratedPermissions.Select<EnumeratedPermission, EnumeratedPermission>((Func<EnumeratedPermission, EnumeratedPermission>) (p => new EnumeratedPermission(RepositoryShimSecurityNamespace.TranslateOutgoingToken(requestContext, p.Token), p.Inherit, p.Allow, p.Deny))) : (IEnumerable<EnumeratedPermission>) null;
    }

    private class ShimQueryableAclStore : IQueryableAclStore, ISecurityAclStore
    {
      private readonly IQueryableAclStore m_aclStore;

      public ShimQueryableAclStore(IQueryableAclStore aclStore) => this.m_aclStore = aclStore;

      public Guid AclStoreId => this.m_aclStore.AclStoreId;

      public IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<EvaluationPrincipal> evaluationPrincipals,
        bool includeExtendedInfo,
        bool recurse)
      {
        IEnumerable<QueriedAccessControlList> acls = this.m_aclStore.QueryAccessControlLists(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), evaluationPrincipals, includeExtendedInfo, recurse);
        return RepositoryShimSecurityNamespace.TranslateOutgoingAccessControlLists(requestContext, acls);
      }

      public IEnumerable<EnumeratedPermission> QueryChildPermissions(
        IVssRequestContext requestContext,
        string token,
        EvaluationPrincipal evaluationPrincipal,
        int bitsToConsider = -1)
      {
        IEnumerable<EnumeratedPermission> enumeratedPermissions = this.m_aclStore.QueryChildPermissions(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), evaluationPrincipal, bitsToConsider);
        return RepositoryShimSecurityNamespace.TranslateOutgoingEnumeratedPermissions(requestContext, enumeratedPermissions);
      }

      public void QueryEffectivePermissions(
        IVssRequestContext requestContext,
        string token,
        EvaluationPrincipal evaluationPrincipal,
        out int effectiveAllow,
        out int effectiveDeny,
        int bitsToConsider = -1)
      {
        this.m_aclStore.QueryEffectivePermissions(requestContext, RepositoryShimSecurityNamespace.TranslateIncomingToken(requestContext, token), evaluationPrincipal, out effectiveAllow, out effectiveDeny, bitsToConsider);
      }

      public int QueryAclCount(IVssRequestContext requestContext) => this.m_aclStore.QueryAclCount(requestContext);
    }
  }
}
