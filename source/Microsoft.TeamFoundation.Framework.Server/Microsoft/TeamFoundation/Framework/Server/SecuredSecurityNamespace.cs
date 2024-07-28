// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuredSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecuredSecurityNamespace : 
    IVssSecurityNamespace,
    IMutableSecurityNamespace,
    IQueryableSecurityNamespace,
    IQueryableSecurityNamespaceInternal
  {
    private readonly IVssSecurityNamespace m_securityNamespace;
    private static readonly IAccessControlEntry[] s_emptyAceArray = Array.Empty<IAccessControlEntry>();

    public SecuredSecurityNamespace(IVssSecurityNamespace securityNamespace) => this.m_securityNamespace = !(securityNamespace is SecuredSecurityNamespace) ? securityNamespace : throw new InvalidOperationException();

    public IVssSecurityNamespace UnsecuredNamespace => this.m_securityNamespace;

    public SecurityNamespaceDescription Description => this.m_securityNamespace.Description;

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
        aclStore = (IQueryableAclStore) new SecuredSecurityNamespace.SecuredQueryableAclStore(this.m_securityNamespace, aclStore);
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
      this.m_securityNamespace.CheckWritePermission(requestContext, token, false);
      return this.m_securityNamespace.SetAccessControlEntries(requestContext, token, accessControlEntries, merge, throwOnInvalidIdentity, rootNewIdentities);
    }

    public void SetAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> accessControlLists,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      foreach (string token in accessControlLists.Select<IAccessControlList, string>((Func<IAccessControlList, string>) (s => s.Token)))
        this.m_securityNamespace.CheckWritePermission(requestContext, token, false);
      this.m_securityNamespace.SetAccessControlLists(requestContext, accessControlLists, throwOnInvalidIdentity, rootNewIdentities);
    }

    public void SetInheritFlag(IVssRequestContext requestContext, string token, bool inherit)
    {
      this.m_securityNamespace.CheckWritePermission(requestContext, token, false);
      this.m_securityNamespace.SetInheritFlag(requestContext, token, inherit);
    }

    public bool RemoveAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      this.m_securityNamespace.CheckWritePermission(requestContext, token, false);
      return this.m_securityNamespace.RemoveAccessControlEntries(requestContext, token, descriptors);
    }

    public bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      bool recurse)
    {
      foreach (string token in tokens)
        this.m_securityNamespace.CheckWritePermission(requestContext, token, recurse);
      return this.m_securityNamespace.RemoveAccessControlLists(requestContext, tokens, recurse);
    }

    public void RenameToken(
      IVssRequestContext requestContext,
      string existingToken,
      string newToken,
      bool copy)
    {
      throw new NotImplementedException();
    }

    public void RenameTokens(
      IVssRequestContext requestContext,
      IEnumerable<TokenRename> renameTokens)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse)
    {
      if (token != null)
        this.m_securityNamespace.CheckReadPermission(requestContext, token);
      IEnumerable<QueriedAccessControlList> accessControlLists = this.m_securityNamespace.QueryAccessControlLists(requestContext, token, evaluationPrincipals, includeExtendedInfo, recurse);
      List<QueriedAccessControlList> accessControlListList = new List<QueriedAccessControlList>();
      foreach (QueriedAccessControlList accessControlList in accessControlLists)
      {
        if (this.m_securityNamespace.HasReadPermission(requestContext, accessControlList.Token))
          accessControlListList.Add(accessControlList);
      }
      return (IEnumerable<QueriedAccessControlList>) accessControlListList;
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
      this.m_securityNamespace.QueryableSecurityNamespaceInternal().QueryEffectivePermissions(requestContext, token, evaluationPrincipal, out effectiveAllow, out effectiveDeny, bitsToConsider);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermission(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermission(requestContext, principal, token, requestedPermissions, alwaysAllowAdministrators);
    }

    bool? IQueryableSecurityNamespaceInternal.GetPrincipalPermissionState(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().GetPrincipalPermissionState(requestContext, principal, token, requestedPermissions, alwaysAllowAdministrators);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermissionForAllChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators)
    {
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermissionForAllChildren(requestContext, principal, token, requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermissionForAnyChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators)
    {
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermissionForAnyChildren(requestContext, principal, token, requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators);
    }

    private class SecuredQueryableAclStore : IQueryableAclStore, ISecurityAclStore
    {
      private readonly IVssSecurityNamespace m_securityNamespace;
      private readonly IQueryableAclStore m_aclStore;

      public SecuredQueryableAclStore(
        IVssSecurityNamespace securityNamespace,
        IQueryableAclStore aclStore)
      {
        this.m_aclStore = !(aclStore is SecuredSecurityNamespace.SecuredQueryableAclStore) ? aclStore : throw new InvalidOperationException();
        this.m_securityNamespace = securityNamespace;
      }

      public Guid AclStoreId => this.m_aclStore.AclStoreId;

      public IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<EvaluationPrincipal> evaluationPrincipals,
        bool includeExtendedInfo,
        bool recurse)
      {
        if (token != null)
          this.m_securityNamespace.CheckReadPermission(requestContext, token);
        IEnumerable<QueriedAccessControlList> accessControlLists = this.m_aclStore.QueryAccessControlLists(requestContext, token, evaluationPrincipals, includeExtendedInfo, recurse);
        List<QueriedAccessControlList> accessControlListList = new List<QueriedAccessControlList>();
        foreach (QueriedAccessControlList accessControlList in accessControlLists)
        {
          if (this.m_securityNamespace.HasReadPermission(requestContext, accessControlList.Token))
            accessControlListList.Add(accessControlList);
        }
        return (IEnumerable<QueriedAccessControlList>) accessControlListList;
      }

      public IEnumerable<EnumeratedPermission> QueryChildPermissions(
        IVssRequestContext requestContext,
        string token,
        EvaluationPrincipal evaluationPrincipal,
        int bitsToConsider = -1)
      {
        if (token != null)
          this.m_securityNamespace.CheckReadPermission(requestContext, token);
        IEnumerable<EnumeratedPermission> enumeratedPermissions = this.m_aclStore.QueryChildPermissions(requestContext, token, evaluationPrincipal, bitsToConsider);
        List<EnumeratedPermission> enumeratedPermissionList = new List<EnumeratedPermission>();
        foreach (EnumeratedPermission enumeratedPermission in enumeratedPermissions)
        {
          if (this.m_securityNamespace.HasReadPermission(requestContext, enumeratedPermission.Token))
            enumeratedPermissionList.Add(enumeratedPermission);
        }
        return (IEnumerable<EnumeratedPermission>) enumeratedPermissionList;
      }

      public void QueryEffectivePermissions(
        IVssRequestContext requestContext,
        string token,
        EvaluationPrincipal evaluationPrincipal,
        out int effectiveAllow,
        out int effectiveDeny,
        int bitsToConsider = -1)
      {
        this.m_aclStore.QueryEffectivePermissions(requestContext, token, evaluationPrincipal, out effectiveAllow, out effectiveDeny, bitsToConsider);
      }

      public int QueryAclCount(IVssRequestContext requestContext) => throw new NotImplementedException();
    }
  }
}
