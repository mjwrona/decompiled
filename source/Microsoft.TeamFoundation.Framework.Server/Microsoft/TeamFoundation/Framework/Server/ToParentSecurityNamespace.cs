// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ToParentSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ToParentSecurityNamespace : 
    IVssSecurityNamespace,
    IMutableSecurityNamespace,
    IQueryableSecurityNamespace,
    IQueryableSecurityNamespaceInternal,
    IRemoteSecurityNamespace
  {
    private readonly IVssSecurityNamespace m_securityNamespace;
    private readonly Guid m_serviceHostId;

    public ToParentSecurityNamespace(IVssSecurityNamespace securityNamespace, Guid serviceHostId)
    {
      ArgumentUtility.CheckForNull<IVssSecurityNamespace>(securityNamespace, nameof (securityNamespace));
      ArgumentUtility.CheckForEmptyGuid(serviceHostId, nameof (serviceHostId));
      this.m_securityNamespace = securityNamespace;
      this.m_serviceHostId = serviceHostId;
    }

    public SecurityNamespaceDescription Description => this.m_securityNamespace.Description;

    public ISecurityNamespaceExtension NamespaceExtension => this.m_securityNamespace.NamespaceExtension;

    public void OnDataChanged(IVssRequestContext requestContext) => this.m_securityNamespace.OnDataChanged(this.TranslateRequestContext(requestContext));

    public bool PollForRequestLocalInvalidation(IVssRequestContext requestContext) => this.m_securityNamespace.PollForRequestLocalInvalidation(requestContext);

    public void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      EvaluationPrincipal failingPrincipal = null)
    {
      this.m_securityNamespace.ThrowAccessDeniedException(this.TranslateRequestContext(requestContext), token, requestedPermissions, failingPrincipal);
    }

    public IQueryableAclStore GetQueryableAclStore(
      IVssRequestContext requestContext,
      Guid aclStoreId)
    {
      IQueryableAclStore aclStore = this.m_securityNamespace.GetQueryableAclStore(this.TranslateRequestContext(requestContext), aclStoreId);
      if (aclStore != null)
        aclStore = (IQueryableAclStore) new ToParentSecurityNamespace.ToParentQueryableAclStore(aclStore, this.m_serviceHostId);
      return aclStore;
    }

    public Guid ServiceOwner => SecurityServiceConstants.ToParentServiceOwner;

    public bool IsCacheable => false;

    public IEnumerable<IAccessControlEntry> SetAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IAccessControlEntry> accessControlEntries,
      bool merge,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      IEnumerable<IAccessControlEntry> aces = this.m_securityNamespace.SetAccessControlEntries(vssRequestContext, token, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateAccessControlEntries(requestContext, vssRequestContext, accessControlEntries), merge, throwOnInvalidIdentity, rootNewIdentities);
      return ToParentSecurityNamespace.ToParentTranslationHelper.TranslateAccessControlEntries(vssRequestContext, requestContext, aces);
    }

    public void SetAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> accessControlLists,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      this.m_securityNamespace.SetAccessControlLists(vssRequestContext, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateAccessControlLists(requestContext, vssRequestContext, accessControlLists), throwOnInvalidIdentity, rootNewIdentities);
    }

    public void SetInheritFlag(IVssRequestContext requestContext, string token, bool inherit) => this.m_securityNamespace.SetInheritFlag(this.TranslateRequestContext(requestContext), token, inherit);

    public bool RemoveAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      return this.m_securityNamespace.RemoveAccessControlEntries(vssRequestContext, token, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateIdentityDescriptors(requestContext, vssRequestContext, descriptors));
    }

    public bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      bool recurse)
    {
      return this.m_securityNamespace.RemoveAccessControlLists(this.TranslateRequestContext(requestContext), tokens, recurse);
    }

    public void RenameToken(
      IVssRequestContext requestContext,
      string existingToken,
      string newToken,
      bool copy)
    {
      this.m_securityNamespace.RenameToken(this.TranslateRequestContext(requestContext), existingToken, newToken, copy);
    }

    public void RenameTokens(
      IVssRequestContext requestContext,
      IEnumerable<TokenRename> renameTokens)
    {
      this.m_securityNamespace.RenameTokens(this.TranslateRequestContext(requestContext), renameTokens);
    }

    public IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      IEnumerable<QueriedAccessControlList> qacls = this.m_securityNamespace.QueryAccessControlLists(vssRequestContext, token, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipals(requestContext, vssRequestContext, evaluationPrincipals), includeExtendedInfo, recurse);
      return ToParentSecurityNamespace.ToParentTranslationHelper.TranslateQueriedAccessControlLists(vssRequestContext, requestContext, qacls);
    }

    void IQueryableSecurityNamespaceInternal.CheckRequestContext(IVssRequestContext requestContext) => this.m_securityNamespace.QueryableSecurityNamespaceInternal().CheckRequestContext(this.TranslateRequestContext(requestContext));

    void IQueryableSecurityNamespaceInternal.QueryEffectivePermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      out int effectiveAllow,
      out int effectiveDeny,
      int bitsToConsider)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      this.m_securityNamespace.QueryableSecurityNamespaceInternal().QueryEffectivePermissions(vssRequestContext, token, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipal(requestContext, vssRequestContext, evaluationPrincipal), out effectiveAllow, out effectiveDeny, bitsToConsider);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermission(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermission(vssRequestContext, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipal(requestContext, vssRequestContext, principal), token, requestedPermissions, alwaysAllowAdministrators);
    }

    bool? IQueryableSecurityNamespaceInternal.GetPrincipalPermissionState(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().GetPrincipalPermissionState(vssRequestContext, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipal(requestContext, vssRequestContext, principal), token, requestedPermissions, alwaysAllowAdministrators);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermissionForAllChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermissionForAllChildren(vssRequestContext, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipal(requestContext, vssRequestContext, principal), token, requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators);
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermissionForAnyChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators)
    {
      IVssRequestContext vssRequestContext = this.TranslateRequestContext(requestContext);
      return this.m_securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermissionForAnyChildren(vssRequestContext, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipal(requestContext, vssRequestContext, principal), token, requestedPermissions, resultIfNoChildrenFound, alwaysAllowAdministrators);
    }

    private IVssRequestContext TranslateRequestContext(IVssRequestContext requestContext) => ToParentSecurityNamespace.ToParentTranslationHelper.TranslateRequestContext(requestContext, this.m_serviceHostId);

    private class ToParentQueryableAclStore : IQueryableAclStore, ISecurityAclStore
    {
      private readonly IQueryableAclStore m_aclStore;
      private readonly Guid m_serviceHostId;

      public ToParentQueryableAclStore(IQueryableAclStore aclStore, Guid serviceHostId)
      {
        this.m_aclStore = aclStore;
        this.m_serviceHostId = serviceHostId;
      }

      public Guid AclStoreId => this.m_aclStore.AclStoreId;

      public IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<EvaluationPrincipal> evaluationPrincipals,
        bool includeExtendedInfo,
        bool recurse)
      {
        IVssRequestContext vssRequestContext = ToParentSecurityNamespace.ToParentTranslationHelper.TranslateRequestContext(requestContext, this.m_serviceHostId);
        IEnumerable<QueriedAccessControlList> qacls = this.m_aclStore.QueryAccessControlLists(vssRequestContext, token, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipals(requestContext, vssRequestContext, evaluationPrincipals), includeExtendedInfo, recurse);
        return ToParentSecurityNamespace.ToParentTranslationHelper.TranslateQueriedAccessControlLists(vssRequestContext, requestContext, qacls);
      }

      public IEnumerable<EnumeratedPermission> QueryChildPermissions(
        IVssRequestContext requestContext,
        string token,
        EvaluationPrincipal evaluationPrincipal,
        int bitsToConsider = -1)
      {
        IVssRequestContext vssRequestContext = ToParentSecurityNamespace.ToParentTranslationHelper.TranslateRequestContext(requestContext, this.m_serviceHostId);
        return this.m_aclStore.QueryChildPermissions(vssRequestContext, token, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipal(requestContext, vssRequestContext, evaluationPrincipal), bitsToConsider);
      }

      public void QueryEffectivePermissions(
        IVssRequestContext requestContext,
        string token,
        EvaluationPrincipal evaluationPrincipal,
        out int effectiveAllow,
        out int effectiveDeny,
        int bitsToConsider = -1)
      {
        IVssRequestContext vssRequestContext = ToParentSecurityNamespace.ToParentTranslationHelper.TranslateRequestContext(requestContext, this.m_serviceHostId);
        this.m_aclStore.QueryEffectivePermissions(vssRequestContext, token, ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipal(requestContext, vssRequestContext, evaluationPrincipal), out effectiveAllow, out effectiveDeny, bitsToConsider);
      }

      public int QueryAclCount(IVssRequestContext requestContext) => this.m_aclStore.QueryAclCount(ToParentSecurityNamespace.ToParentTranslationHelper.TranslateRequestContext(requestContext, this.m_serviceHostId));
    }

    private static class ToParentTranslationHelper
    {
      public static IVssRequestContext TranslateRequestContext(
        IVssRequestContext requestContext,
        Guid targetServiceHostId)
      {
        if (requestContext.ServiceHost.InstanceId == targetServiceHostId)
          throw new InvalidOperationException();
        IVssRequestContext context = requestContext;
        while (!context.GetService<IdentityService>().IdentityServiceInternal().Domain.IsMaster)
        {
          context = context.To(TeamFoundationHostType.Parent);
          if (context == null)
            throw new HostDoesNotExistException(targetServiceHostId);
          if (!(context.ServiceHost.InstanceId != targetServiceHostId))
            return context;
        }
        throw new InvalidRemoteSecurityNamespaceException(FrameworkResources.ToParentCrossesIsMasterBoundary());
      }

      public static IdentityDescriptor TranslateIdentityDescriptor(
        IVssRequestContext sourceContext,
        IVssRequestContext targetContext,
        IdentityDescriptor descriptor)
      {
        IdentityMapper identityMapper1 = sourceContext.GetService<IdentityService>().IdentityMapper;
        IdentityMapper identityMapper2 = targetContext.GetService<IdentityService>().IdentityMapper;
        return (IdentityDescriptor) null != descriptor ? identityMapper2.MapToWellKnownIdentifier(identityMapper1.MapFromWellKnownIdentifier(descriptor)) : (IdentityDescriptor) null;
      }

      public static IEnumerable<IdentityDescriptor> TranslateIdentityDescriptors(
        IVssRequestContext sourceContext,
        IVssRequestContext targetContext,
        IEnumerable<IdentityDescriptor> descriptors)
      {
        IdentityMapper sourceMapper = sourceContext.GetService<IdentityService>().IdentityMapper;
        IdentityMapper targetMapper = targetContext.GetService<IdentityService>().IdentityMapper;
        return descriptors != null ? descriptors.Select<IdentityDescriptor, IdentityDescriptor>((Func<IdentityDescriptor, IdentityDescriptor>) (s => targetMapper.MapToWellKnownIdentifier(sourceMapper.MapFromWellKnownIdentifier(s)))) : (IEnumerable<IdentityDescriptor>) null;
      }

      public static EvaluationPrincipal TranslateEvaluationPrincipal(
        IVssRequestContext sourceContext,
        IVssRequestContext targetContext,
        EvaluationPrincipal evaluationPrincipal)
      {
        return evaluationPrincipal != null ? new EvaluationPrincipal(ToParentSecurityNamespace.ToParentTranslationHelper.TranslateIdentityDescriptor(sourceContext, targetContext, evaluationPrincipal.PrimaryDescriptor), evaluationPrincipal.MembershipDescriptor == (IdentityDescriptor) null ? (IdentityDescriptor) null : ToParentSecurityNamespace.ToParentTranslationHelper.TranslateIdentityDescriptor(sourceContext, targetContext, evaluationPrincipal.MembershipDescriptor), (IEnumerable<IdentityDescriptor>) evaluationPrincipal.RoleDescriptors) : (EvaluationPrincipal) null;
      }

      public static IEnumerable<EvaluationPrincipal> TranslateEvaluationPrincipals(
        IVssRequestContext sourceContext,
        IVssRequestContext targetContext,
        IEnumerable<EvaluationPrincipal> evaluationPrincipals)
      {
        return evaluationPrincipals != null ? evaluationPrincipals.Select<EvaluationPrincipal, EvaluationPrincipal>((Func<EvaluationPrincipal, EvaluationPrincipal>) (s => ToParentSecurityNamespace.ToParentTranslationHelper.TranslateEvaluationPrincipal(sourceContext, targetContext, s))) : (IEnumerable<EvaluationPrincipal>) null;
      }

      public static IEnumerable<IAccessControlEntry> TranslateAccessControlEntries(
        IVssRequestContext sourceContext,
        IVssRequestContext targetContext,
        IEnumerable<IAccessControlEntry> aces)
      {
        IdentityMapper sourceMapper = sourceContext.GetService<IdentityService>().IdentityMapper;
        IdentityMapper targetMapper = targetContext.GetService<IdentityService>().IdentityMapper;
        return aces != null ? aces.Select<IAccessControlEntry, IAccessControlEntry>((Func<IAccessControlEntry, IAccessControlEntry>) (ace =>
        {
          IdentityDescriptor wellKnownIdentifier = targetMapper.MapToWellKnownIdentifier(sourceMapper.MapFromWellKnownIdentifier(ace.Descriptor));
          if ((object) wellKnownIdentifier != (object) ace.Descriptor)
          {
            ace = ace.Clone();
            ace.Descriptor = wellKnownIdentifier;
          }
          return ace;
        })) : (IEnumerable<IAccessControlEntry>) null;
      }

      public static IEnumerable<IAccessControlList> TranslateAccessControlLists(
        IVssRequestContext sourceContext,
        IVssRequestContext targetContext,
        IEnumerable<IAccessControlList> acls)
      {
        return acls != null ? (IEnumerable<IAccessControlList>) acls.Select<IAccessControlList, AccessControlList>((Func<IAccessControlList, AccessControlList>) (acl =>
        {
          AccessControlList acl1 = new AccessControlList(acl.Token, acl.InheritPermissions);
          acl1.SetAccessControlEntries(ToParentSecurityNamespace.ToParentTranslationHelper.TranslateAccessControlEntries(sourceContext, targetContext, acl.AccessControlEntries), false);
          return acl1;
        })) : (IEnumerable<IAccessControlList>) null;
      }

      public static IEnumerable<QueriedAccessControlEntry> TranslateQueriedAccessControlEntries(
        IVssRequestContext sourceContext,
        IVssRequestContext targetContext,
        IEnumerable<QueriedAccessControlEntry> qaces)
      {
        return qaces != null ? qaces.Select<QueriedAccessControlEntry, QueriedAccessControlEntry>((Func<QueriedAccessControlEntry, QueriedAccessControlEntry>) (s => new QueriedAccessControlEntry(ToParentSecurityNamespace.ToParentTranslationHelper.TranslateIdentityDescriptor(sourceContext, targetContext, s.IdentityDescriptor), s.Allow, s.Deny, s.InheritedAllow, s.InheritedDeny, s.EffectiveAllow, s.EffectiveDeny))) : (IEnumerable<QueriedAccessControlEntry>) null;
      }

      public static IEnumerable<QueriedAccessControlList> TranslateQueriedAccessControlLists(
        IVssRequestContext sourceContext,
        IVssRequestContext targetContext,
        IEnumerable<QueriedAccessControlList> qacls)
      {
        return qacls != null ? qacls.Select<QueriedAccessControlList, QueriedAccessControlList>((Func<QueriedAccessControlList, QueriedAccessControlList>) (s => new QueriedAccessControlList(s.Token, s.Inherit, (IList<QueriedAccessControlEntry>) ToParentSecurityNamespace.ToParentTranslationHelper.TranslateQueriedAccessControlEntries(sourceContext, targetContext, (IEnumerable<QueriedAccessControlEntry>) s.AccessControlEntries).ToList<QueriedAccessControlEntry>()))) : (IEnumerable<QueriedAccessControlList>) null;
      }
    }
  }
}
