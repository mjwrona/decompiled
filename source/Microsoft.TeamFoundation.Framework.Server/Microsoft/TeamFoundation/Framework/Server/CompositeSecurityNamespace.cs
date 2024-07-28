// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CompositeSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class CompositeSecurityNamespace : 
    IVssSecurityNamespace,
    IMutableSecurityNamespace,
    IQueryableSecurityNamespace,
    IQueryableSecurityNamespaceInternal
  {
    protected readonly SecurityNamespaceDescription m_description;
    protected readonly string m_namespaceIdString;
    protected readonly bool m_isHierarchical;
    protected readonly ISecurityNamespaceExtension m_namespaceExtension;
    protected readonly Guid m_serviceHostInstanceId;
    private IdentityService m_identityService;
    private const string c_area = "Security";
    private const string c_layer = "CompositeSecurityNamespace";

    protected CompositeSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description,
      ISecurityNamespaceExtension namespaceExtension)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
      ArgumentUtility.CheckForNull<ISecurityNamespaceExtension>(namespaceExtension, nameof (namespaceExtension));
      this.m_description = description;
      this.m_isHierarchical = description.NamespaceStructure == SecurityNamespaceStructure.Hierarchical;
      this.m_namespaceIdString = this.m_description.NamespaceId.ToString();
      this.m_namespaceExtension = namespaceExtension;
      this.m_serviceHostInstanceId = requestContext.ServiceHost.InstanceId;
    }

    public Guid NamespaceId => this.m_description.NamespaceId;

    public SecurityNamespaceDescription Description => this.m_description;

    public ISecurityNamespaceExtension NamespaceExtension => this.m_namespaceExtension;

    public void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      string token,
      int requestedPermissions,
      EvaluationPrincipal failingPrincipal = null)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (failingPrincipal != null)
      {
        Guid result;
        if (StringComparer.OrdinalIgnoreCase.Equals(failingPrincipal.PrimaryDescriptor.IdentityType, "System:License") && Guid.TryParse(failingPrincipal.PrimaryDescriptor.Identifier, out result))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          SecuritySubjectEntry securitySubjectEntry = vssRequestContext.GetService<IVssSecuritySubjectService>().GetSecuritySubjectEntry(vssRequestContext, result);
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = new Microsoft.VisualStudio.Services.Identity.Identity();
          identity2.Id = result;
          identity2.Descriptor = failingPrincipal.PrimaryDescriptor;
          identity2.CustomDisplayName = securitySubjectEntry.Description;
          identity2.ProviderDisplayName = securitySubjectEntry.Description;
          identity1 = identity2;
        }
        else
        {
          try
          {
            identity1 = requestContext.ReadIdentity(failingPrincipal.PrimaryDescriptor);
          }
          catch (IdentityNotFoundException ex)
          {
          }
        }
      }
      if (identity1 != null)
      {
        this.NamespaceExtension.ThrowAccessDeniedException(requestContext, (IVssSecurityNamespace) this, identity1, token, requestedPermissions);
      }
      else
      {
        IdentityDescriptor descriptor = requestContext.UserContext;
        descriptor = (IdentityDescriptor) null == descriptor ? new IdentityDescriptor("Microsoft.VisualStudio.Services.Identity.UnknownIdentity", "Unauthenticated Request") : throw new AccessCheckException(descriptor, token, requestedPermissions, this.Description.NamespaceId, TFCommonResources.AccessCheckExceptionTokenFormat((object) requestContext.GetUserId().ToString(), (object) token, (object) string.Join(", ", this.Description.GetLocalizedActions(requestedPermissions))));
      }
    }

    public abstract void OnDataChanged(IVssRequestContext requestContext);

    protected static void RelayOnDataChanged(
      IVssRequestContext requestContext,
      CompositeSecurityNamespace.CompositeAclStores aclStores)
    {
      foreach (IInvalidatableAclStore invalidatableStore in aclStores.InvalidatableStores)
        invalidatableStore.NotifyChanged(requestContext, (TokenStoreSequenceId) -3L, true);
    }

    public virtual bool PollForRequestLocalInvalidation(IVssRequestContext requestContext)
    {
      bool flag = false;
      foreach (IInvalidatableAclStore invalidatableStore in this.GetAclStores(requestContext).InvalidatableStores)
        flag |= invalidatableStore.PollForRequestLocalInvalidation(requestContext);
      return flag;
    }

    public virtual IQueryableAclStore GetQueryableAclStore(
      IVssRequestContext requestContext,
      Guid aclStoreId)
    {
      return this.GetAclStores(requestContext).QueryableStores.SingleOrDefault<IQueryableAclStore>((Func<IQueryableAclStore, bool>) (s => s.AclStoreId == aclStoreId));
    }

    public IEnumerable<IAccessControlEntry> SetAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IAccessControlEntry> accessControlEntries,
      bool merge,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      return this.GetAclStores(requestContext).MutableStore.SetAccessControlEntries(requestContext, token, accessControlEntries, merge, throwOnInvalidIdentity, rootNewIdentities);
    }

    public void SetAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> accessControlLists,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      this.GetAclStores(requestContext).MutableStore.SetAccessControlLists(requestContext, accessControlLists, throwOnInvalidIdentity, rootNewIdentities);
    }

    public void SetInheritFlag(IVssRequestContext requestContext, string token, bool inherit) => this.GetAclStores(requestContext).MutableStore.SetInheritFlag(requestContext, token, inherit);

    public bool RemoveAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      return this.GetAclStores(requestContext).MutableStore.RemoveAccessControlEntries(requestContext, token, descriptors);
    }

    public bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      bool recurse)
    {
      return this.GetAclStores(requestContext).MutableStore.RemoveAccessControlLists(requestContext, tokens, recurse);
    }

    public void RenameToken(
      IVssRequestContext requestContext,
      string existingToken,
      string newToken,
      bool copy)
    {
      this.GetAclStores(requestContext).MutableStore.RenameToken(requestContext, existingToken, newToken, copy);
    }

    public void RenameTokens(
      IVssRequestContext requestContext,
      IEnumerable<TokenRename> renameTokens)
    {
      this.GetAclStores(requestContext).MutableStore.RenameTokens(requestContext, renameTokens);
    }

    public IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse)
    {
      requestContext.TraceEnter(56200, "Security", nameof (CompositeSecurityNamespace), nameof (QueryAccessControlLists));
      try
      {
        ((IQueryableSecurityNamespaceInternal) this).CheckRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token, true);
        evaluationPrincipals = this.MapToWellKnownIdentifiers(requestContext, evaluationPrincipals);
        IEnumerable<QueriedAccessControlList> preliminaryAccessControlLists = this.GetAclStores(requestContext).PrimaryQueryableStore.QueryAccessControlLists(requestContext, token, evaluationPrincipals, includeExtendedInfo, recurse);
        return this.m_namespaceExtension.QueryPermissions(requestContext, (IVssSecurityNamespace) this, token, evaluationPrincipals, includeExtendedInfo, recurse, preliminaryAccessControlLists);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56208, "Security", nameof (CompositeSecurityNamespace), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56209, "Security", nameof (CompositeSecurityNamespace), nameof (QueryAccessControlLists));
      }
    }

    void IQueryableSecurityNamespaceInternal.CheckRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHostInstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.SecurityNamespaceRequestContextHostMessage((object) this.Description.DisplayName, (object) this.m_serviceHostInstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    void IQueryableSecurityNamespaceInternal.QueryEffectivePermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      out int effectiveAllow,
      out int effectiveDeny,
      int bitsToConsider)
    {
      effectiveAllow = 0;
      effectiveDeny = 0;
      bool flag = requestContext.IsTracingSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace));
      if (flag)
        requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "QueryEffectivePermissions: Starting to composite effective permissions for token {0}.", (object) token);
      foreach (IQueryableAclStore queryableStore in this.GetAclStores(requestContext).QueryableStores)
      {
        int effectiveAllow1;
        int effectiveDeny1;
        queryableStore.QueryEffectivePermissions(requestContext, token, evaluationPrincipal, out effectiveAllow1, out effectiveDeny1, bitsToConsider);
        if (flag)
          requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "QueryEffectivePermissions: Effective permissions in ACL store {0} for token {1}: allow {2}, deny {3}.", (object) queryableStore.AclStoreId.ToString(), (object) token, (object) effectiveAllow1, (object) effectiveDeny1);
        effectiveAllow |= effectiveAllow1 & ~effectiveDeny;
        effectiveDeny |= effectiveDeny1 & ~effectiveAllow;
        if (flag)
          requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "QueryEffectivePermissions: Current composite effective permissions for token {0}: allow {1}, deny {2}.", (object) token, (object) effectiveAllow, (object) effectiveDeny);
        bitsToConsider &= ~(effectiveAllow | effectiveDeny);
        if (bitsToConsider == 0)
          break;
      }
      if (!flag)
        return;
      requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "QueryEffectivePermissions: Final composite effective permissions for token {0}: allow {1}, deny {2}.", (object) token, (object) effectiveAllow, (object) effectiveDeny);
    }

    private bool? PrincipalHasPermissionCommon(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool alwaysAllowAdministrators,
      Func<int, int, int, bool?> permissionCheckLogic)
    {
      requestContext.TraceEnter(56000, "Security", nameof (CompositeSecurityNamespace), nameof (PrincipalHasPermissionCommon));
      try
      {
        ((IQueryableSecurityNamespaceInternal) this).CheckRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token);
        if (requestContext.IsSystemContext)
        {
          requestContext.Trace(56002, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "SecNS {0}/{1}: HasPerm returning true for system request context checking token {2}", (object) this.Description.DisplayName, (object) this.m_namespaceIdString, (object) token);
          return new bool?(true);
        }
        int effectiveAllow;
        int effectiveDeny;
        ((IQueryableSecurityNamespaceInternal) this).QueryEffectivePermissions(requestContext, token, principal, out effectiveAllow, out effectiveDeny, requestedPermissions);
        bool? nullable = permissionCheckLogic(effectiveAllow, effectiveDeny, requestedPermissions);
        if (((!nullable.HasValue ? 1 : (!nullable.Value ? 1 : 0)) & (alwaysAllowAdministrators ? 1 : 0)) != 0 && principal.MembershipDescriptor != (IdentityDescriptor) null && SecurityEvaluator.IsMemberInternal(requestContext, requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>(), this.GetIdentityService(requestContext), GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, principal.MembershipDescriptor, false, this.m_description.NamespaceId, requestedPermissions, token, token))
          nullable = new bool?(true);
        bool valueOrDefault = nullable.GetValueOrDefault();
        bool flag = this.NamespaceExtension.HasPermission(requestContext, (IVssSecurityNamespace) this, principal, token, requestedPermissions, effectiveAllow, effectiveDeny, valueOrDefault);
        if (nullable.HasValue || valueOrDefault != flag)
          nullable = new bool?(flag);
        if (flag && !valueOrDefault && requestContext.IsTracing(56008, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace)))
          requestContext.Trace(56008, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "SecNS {0}/{1}: PrincipalHasPermission: Namespace extension changed result to true for {2}, token {3}.", (object) this.Description.DisplayName, (object) this.m_namespaceIdString, (object) SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, principal), (object) token);
        if (requestContext.IsTracing(56003, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace)))
          requestContext.Trace(56003, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "SecNS {0}/{1}: PrincipalHasPermission returning {2} for {3}, token {4}", (object) this.Description.DisplayName, (object) this.m_namespaceIdString, !nullable.HasValue ? (object) "NotSet" : (nullable.Value ? (object) bool.TrueString : (object) bool.FalseString), (object) SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, principal), (object) token);
        return nullable;
      }
      finally
      {
        requestContext.TraceLeave(56001, "Security", nameof (CompositeSecurityNamespace), nameof (PrincipalHasPermissionCommon));
      }
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermission(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions1,
      bool alwaysAllowAdministrators)
    {
      return this.PrincipalHasPermissionCommon(requestContext, principal, token, requestedPermissions1, alwaysAllowAdministrators, (Func<int, int, int, bool?>) ((effectiveAllow, effectiveDeny, requestedPermissions2) => new bool?((effectiveAllow & requestedPermissions2) == requestedPermissions2))).Value;
    }

    bool? IQueryableSecurityNamespaceInternal.GetPrincipalPermissionState(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions1,
      bool alwaysAllowAdministrators)
    {
      return this.PrincipalHasPermissionCommon(requestContext, principal, token, requestedPermissions1, alwaysAllowAdministrators, (Func<int, int, int, bool?>) ((effectiveAllow, effectiveDeny, requestedPermissions2) => effectiveAllow == 0 && effectiveDeny == 0 ? new bool?() : new bool?((effectiveAllow & requestedPermissions2) == requestedPermissions2)));
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermissionForAllChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators)
    {
      requestContext.TraceEnter(56100, "Security", nameof (CompositeSecurityNamespace), "PrincipalHasPermissionForAllChildren");
      try
      {
        ((IQueryableSecurityNamespaceInternal) this).CheckRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token);
        if (requestContext.IsSystemContext)
          return true;
        Func<IQueryableAclStore, int, EvaluationPrincipal, bool> func = (Func<IQueryableAclStore, int, EvaluationPrincipal, bool>) ((aclStore, bitsToCheck, principalToCheck) =>
        {
          bool flag1 = resultIfNoChildrenFound;
          foreach (EnumeratedPermission queryChildPermission in aclStore.QueryChildPermissions(requestContext, token, principalToCheck, bitsToCheck))
          {
            bool flag2 = !queryChildPermission.Inherit ? (queryChildPermission.Allow & bitsToCheck) == bitsToCheck : (queryChildPermission.Deny & bitsToCheck) == 0;
            if (!(flag1 = flag2))
              break;
          }
          return flag1;
        });
        bool flag3 = resultIfNoChildrenFound;
        using (IEnumerator<IQueryableAclStore> enumerator = this.GetAclStores(requestContext).QueryableStores.GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            int num1 = 0;
            do
            {
              IQueryableAclStore current;
              int num2;
              do
              {
                current = enumerator.Current;
                num2 = requestedPermissions & ~num1;
                if (enumerator.MoveNext())
                {
                  int effectiveAllow;
                  current.QueryEffectivePermissions(requestContext, token, principal, out effectiveAllow, out int _);
                  num2 &= effectiveAllow;
                }
              }
              while (num2 == 0);
              num1 |= num2;
              flag3 = func(current, num2, principal);
              if (!flag3)
                break;
            }
            while (num1 != requestedPermissions);
          }
        }
        if (!flag3 & alwaysAllowAdministrators && principal.MembershipDescriptor != (IdentityDescriptor) null && SecurityEvaluator.IsMemberInternal(requestContext, requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>(), this.GetIdentityService(requestContext), GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, principal.MembershipDescriptor, false, this.m_description.NamespaceId, requestedPermissions, token, token))
          flag3 = true;
        bool preliminaryDecision = flag3;
        bool flag4 = this.NamespaceExtension.HasPermissionForAllChildren(requestContext, (IVssSecurityNamespace) this, principal, token, requestedPermissions, preliminaryDecision);
        if (flag4 && !preliminaryDecision && requestContext.IsTracing(56008, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace)))
          requestContext.Trace(56008, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "SecNS {0}/{1}: PrincipalHasPermissionForAllChildren: Namespace extension changed result to true for {2}, token {3}.", (object) this.Description.DisplayName, (object) this.m_namespaceIdString, (object) SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, principal), (object) token);
        return flag4;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56108, "Security", nameof (CompositeSecurityNamespace), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56109, "Security", nameof (CompositeSecurityNamespace), "PrincipalHasPermissionForAllChildren");
      }
    }

    bool IQueryableSecurityNamespaceInternal.PrincipalHasPermissionForAnyChildren(
      IVssRequestContext requestContext,
      EvaluationPrincipal principal,
      string token,
      int requestedPermissions,
      bool resultIfNoChildrenFound,
      bool alwaysAllowAdministrators)
    {
      requestContext.TraceEnter(56120, "Security", nameof (CompositeSecurityNamespace), "PrincipalHasPermissionForAnyChildren");
      try
      {
        ((IQueryableSecurityNamespaceInternal) this).CheckRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token);
        if (requestContext.IsSystemContext)
          return true;
        bool flag1 = resultIfNoChildrenFound;
        foreach (IQueryableAclStore queryableStore in this.GetAclStores(requestContext).QueryableStores)
        {
          foreach (EnumeratedPermission queryChildPermission in queryableStore.QueryChildPermissions(requestContext, token, principal, requestedPermissions))
          {
            if (flag1 = (queryChildPermission.Allow & requestedPermissions) == requestedPermissions)
              return flag1;
          }
        }
        if (alwaysAllowAdministrators && principal.MembershipDescriptor != (IdentityDescriptor) null && SecurityEvaluator.IsMemberInternal(requestContext, requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>(), this.GetIdentityService(requestContext), GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, principal.MembershipDescriptor, false, this.m_description.NamespaceId, requestedPermissions, token, token))
          flag1 = true;
        bool preliminaryDecision = flag1;
        bool flag2 = this.NamespaceExtension.HasPermissionForAnyChildren(requestContext, (IVssSecurityNamespace) this, principal, token, requestedPermissions, preliminaryDecision);
        if (flag2 && !preliminaryDecision && requestContext.IsTracing(56008, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace)))
          requestContext.Trace(56008, TraceLevel.Verbose, "Security", nameof (CompositeSecurityNamespace), "SecNS {0}/{1}: PrincipalHasPermissionForAnyChildren: Namespace extension changed result to true for {2}, token {3}.", (object) this.Description.DisplayName, (object) this.m_namespaceIdString, (object) SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, principal), (object) token);
        return flag2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56128, "Security", nameof (CompositeSecurityNamespace), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56129, "Security", nameof (CompositeSecurityNamespace), "PrincipalHasPermissionForAnyChildren");
      }
    }

    private IEnumerable<EvaluationPrincipal> MapToWellKnownIdentifiers(
      IVssRequestContext requestContext,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals)
    {
      if (evaluationPrincipals == null)
        return (IEnumerable<EvaluationPrincipal>) null;
      IdentityMapper mapper = this.GetIdentityService(requestContext).IdentityMapper;
      return evaluationPrincipals.Select<EvaluationPrincipal, EvaluationPrincipal>((Func<EvaluationPrincipal, EvaluationPrincipal>) (s => s.ToWellKnownEvaluationPrincipal(mapper)));
    }

    protected abstract CompositeSecurityNamespace.CompositeAclStores GetAclStores(
      IVssRequestContext requestContext);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected IdentityService GetIdentityService(IVssRequestContext requestContext)
    {
      IdentityService identityService = this.m_identityService;
      if (identityService == null)
      {
        identityService = requestContext.GetService<IdentityService>();
        this.m_identityService = identityService;
      }
      return identityService;
    }

    protected class QuerySecurityDataResult : IQuerySecurityDataResult
    {
      private readonly Guid m_aclStoreId;
      private readonly long m_oldSequenceId;
      private readonly TokenStoreSequenceId m_newSequenceId;
      private readonly IEnumerable<BackingStoreAccessControlEntry> m_accessControlEntries;
      private readonly IEnumerable<string> m_noInheritTokens;

      public QuerySecurityDataResult(
        Guid aclStoreId,
        long oldSequenceId,
        TokenStoreSequenceId newSequenceId,
        IEnumerable<BackingStoreAccessControlEntry> accessControlEntries,
        IEnumerable<string> noInheritTokens)
      {
        this.m_aclStoreId = aclStoreId;
        this.m_oldSequenceId = oldSequenceId;
        this.m_newSequenceId = newSequenceId;
        this.m_accessControlEntries = accessControlEntries;
        this.m_noInheritTokens = noInheritTokens;
      }

      public Guid AclStoreId => this.m_aclStoreId;

      public long OldSequenceId => this.m_oldSequenceId;

      public TokenStoreSequenceId NewSequenceId => this.m_newSequenceId;

      public IEnumerable<BackingStoreAccessControlEntry> AccessControlEntries => this.m_accessControlEntries;

      public IEnumerable<string> NoInheritTokens => this.m_noInheritTokens;
    }

    protected class CompositeAclStores
    {
      public readonly IEnumerable<IQueryableAclStore> QueryableStores;
      public readonly IQueryableAclStore PrimaryQueryableStore;
      public readonly IMutableAclStore MutableStore;
      public readonly IEnumerable<IInvalidatableAclStore> InvalidatableStores;

      public CompositeAclStores(
        IEnumerable<IQueryableAclStore> queryableStores,
        IQueryableAclStore primaryQueryableStore,
        IMutableAclStore mutableStore)
      {
        this.QueryableStores = queryableStores;
        this.PrimaryQueryableStore = primaryQueryableStore;
        this.MutableStore = mutableStore;
        this.InvalidatableStores = (IEnumerable<IInvalidatableAclStore>) new List<IInvalidatableAclStore>(((IEnumerable<ISecurityAclStore>) queryableStores).Concat<ISecurityAclStore>((IEnumerable<ISecurityAclStore>) new ISecurityAclStore[2]
        {
          (ISecurityAclStore) primaryQueryableStore,
          (ISecurityAclStore) mutableStore
        }).OfType<IInvalidatableAclStore>().Distinct<IInvalidatableAclStore>());
      }
    }
  }
}
