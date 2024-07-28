// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityEvaluator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityEvaluator
  {
    private readonly Guid m_namespaceId;
    private readonly string m_namespaceIdString;
    private readonly Guid m_aclStoreId;
    private readonly string m_aclStoreIdString;
    private readonly TokenStoreSequenceId m_sequenceId;
    private readonly ITokenStore<AccessControlListSlim> m_tokenStore;
    private readonly IIdentityServiceInternal m_identityService;
    private readonly SecuritySettingsService m_settingsService;
    private readonly SecurityEvaluator.CacheEntry[] m_cache = new SecurityEvaluator.CacheEntry[2];
    private static readonly AccessControlEntrySlim[] s_emptyAceList = Array.Empty<AccessControlEntrySlim>();
    private static readonly Guid s_l2AdminServicePrincipal = new Guid("DEADBEEF-0000-8888-8000-000000000000");
    private const int c_cacheSize = 2;
    private const string c_area = "Security";
    private const string c_layer = "SecurityEvaluator";
    private const string s_callIsMemberWithRootContextFeatureFlag = "VisualStudio.Services.Security.ToParentSecurityNamespace.CallIsMemberWithRootContext";
    private static readonly Guid s_licensingSecurityNamespaceId = new Guid("453E2DB3-2E81-474F-874D-3BF51027F2EE");
    private static readonly Guid s_userEntitlementSecurityNamespaceId = new Guid("FC17B2CF-7451-4887-A0DB-6AE4BBE9B586");

    public SecurityEvaluator(
      Guid namespaceId,
      Guid aclStoreId,
      TokenStoreSequenceId sequenceId,
      ITokenStore<AccessControlListSlim> tokenStore,
      IIdentityServiceInternal identityService,
      SecuritySettingsService settingsService)
    {
      this.m_namespaceId = namespaceId;
      this.m_namespaceIdString = namespaceId.ToString();
      this.m_aclStoreId = aclStoreId;
      this.m_aclStoreIdString = aclStoreId.ToString();
      this.m_sequenceId = sequenceId;
      this.m_tokenStore = tokenStore;
      this.m_identityService = identityService;
      this.m_settingsService = settingsService;
    }

    public ITokenStore<AccessControlListSlim> GetAclStore() => this.m_tokenStore;

    public TokenStoreSequenceId SequenceId => this.m_sequenceId;

    public IEnumerable<EnumeratedPermission> EnumChildPermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      int bitsToConsider = -1)
    {
      requestContext.TraceEnter(56809, "Security", nameof (SecurityEvaluator), nameof (EnumChildPermissions));
      try
      {
        bool isTracing = requestContext.IsTracingSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator));
        string evaluationPrincipalString = (string) null;
        if (isTracing)
          evaluationPrincipalString = SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, evaluationPrincipal);
        foreach (AccessControlListSlim accessControlListSlim in this.m_tokenStore.EnumSubTree(token, false))
        {
          IEnumerable<AccessControlEntrySlim> accessControlEntries = accessControlListSlim.AccessControlEntries;
          int effectiveAllow;
          int effectiveDeny;
          this.EvaluatePermissions(requestContext, evaluationPrincipal, accessControlListSlim.Token, token, accessControlEntries, evaluationPrincipalString, isTracing, out effectiveAllow, out effectiveDeny, bitsToConsider);
          if (isTracing)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EnumChildPermissions: For token {0}, allow is {1} and deny is {2}", (object) EuiiUtility.MaskEmail(accessControlListSlim.Token ?? "<null>"), (object) effectiveAllow, (object) effectiveDeny);
          yield return new EnumeratedPermission(accessControlListSlim.Token, accessControlListSlim.InheritPermissions, effectiveAllow, effectiveDeny);
        }
        evaluationPrincipalString = (string) null;
      }
      finally
      {
        requestContext.TraceLeave(56810, "Security", nameof (SecurityEvaluator), nameof (EnumChildPermissions));
      }
    }

    public SecurityEvaluator.EvaluationResult QueryEffectivePermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      int bitsToConsider = -1)
    {
      requestContext.TraceEnter(56800, "Security", nameof (SecurityEvaluator), nameof (QueryEffectivePermissions));
      try
      {
        bool isTracing = requestContext.IsTracingSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator));
        string descriptorString = (string) null;
        if (isTracing)
          descriptorString = SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, evaluationPrincipal);
        bool principalHasStatelessAces = this.DoesPrincipalHaveStatelessAces(evaluationPrincipal);
        if (!principalHasStatelessAces && this.m_tokenStore.Count == 0)
        {
          if (isTracing)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "QueryEffectivePermissions: Token store is empty; returning empty EvaluationResult. Sequence ID for ACL store is {0}", (object) this.m_sequenceId);
          return new SecurityEvaluator.EvaluationResult();
        }
        string str = isTracing ? EuiiUtility.MaskEmail(token ?? "<null>") : string.Empty;
        for (int index = 0; index < 2; ++index)
        {
          SecurityEvaluator.CacheEntry cacheEntry = this.m_cache[index];
          if (cacheEntry.EvaluationPrincipal != null && cacheEntry.EvaluationPrincipal.Equals((object) evaluationPrincipal) && (bitsToConsider & ~cacheEntry.ValidBits) == 0 && this.m_tokenStore.IsSubItem(token, cacheEntry.Token))
          {
            if (cacheEntry.IdentityChangeId != this.m_identityService.GetCurrentChangeId())
            {
              this.m_cache[index] = new SecurityEvaluator.CacheEntry();
            }
            else
            {
              if (isTracing)
                requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "QueryEffectivePermissions: Cache hit for token {0} and evaluation principal {1} in namespace {2} at sequence ID {3}", (object) str, (object) descriptorString, (object) this.m_namespaceIdString, (object) this.m_sequenceId);
              SecurityEvaluator.EvaluationResult evaluationResult = cacheEntry.EvaluationResult;
              evaluationResult.Allow &= bitsToConsider;
              evaluationResult.Deny &= bitsToConsider;
              return evaluationResult;
            }
          }
        }
        if (isTracing)
          requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "QueryEffectivePermissions: Cache miss for token {0} and evaluation principal {1} in namespace {2} at sequence ID {3}", (object) str, (object) descriptorString, (object) this.m_namespaceIdString, (object) this.m_sequenceId);
        SecurityEvaluator.CacheEntry cacheEntry1 = new SecurityEvaluator.CacheEntry();
        cacheEntry1.IdentityChangeId = this.m_identityService.GetCurrentChangeId();
        bool noChildrenBelow;
        string noChildrenBelowToken;
        SecurityEvaluator.EvaluationResult effectivePermissions = this.ComputeEffectivePermissions(requestContext, token, evaluationPrincipal, principalHasStatelessAces, isTracing, descriptorString, out noChildrenBelow, out noChildrenBelowToken, bitsToConsider);
        if (noChildrenBelow)
        {
          cacheEntry1.Token = noChildrenBelowToken;
          cacheEntry1.EvaluationPrincipal = evaluationPrincipal;
          cacheEntry1.EvaluationResult = effectivePermissions;
          cacheEntry1.ValidBits = bitsToConsider;
          for (int index = 1; index >= 1; --index)
            this.m_cache[index] = this.m_cache[index - 1];
          this.m_cache[0] = cacheEntry1;
          if (isTracing)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "QueryEffectivePermissions: Cached result for token {0} (and all children), evaluation principal {1}, identity change ID {2}, effective allow {3}, effective deny {4}", (object) EuiiUtility.MaskEmail(cacheEntry1.Token ?? "<null>"), (object) descriptorString, (object) cacheEntry1.IdentityChangeId, (object) cacheEntry1.EvaluationResult.Allow, (object) cacheEntry1.EvaluationResult.Deny);
        }
        return effectivePermissions;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56802, "Security", nameof (SecurityEvaluator), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56801, "Security", nameof (SecurityEvaluator), nameof (QueryEffectivePermissions));
      }
    }

    public SecurityEvaluator.EvaluationResult ComputeEffectivePermissionsForQueryWithExtendedInfo(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal)
    {
      bool isTracing = requestContext.IsTracingSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator));
      string descriptorString = (string) null;
      if (isTracing)
        descriptorString = SecurityServiceHelpers.EvaluationPrincipalToString(requestContext, evaluationPrincipal);
      return this.ComputeEffectivePermissions(requestContext, token, evaluationPrincipal, this.DoesPrincipalHaveStatelessAces(evaluationPrincipal), isTracing, descriptorString, out bool _, out string _, forQueryWithExtendedInfo: true);
    }

    private bool DoesPrincipalHaveStatelessAces(EvaluationPrincipal evaluationPrincipal) => evaluationPrincipal.RoleDescriptors.Any<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (s => s.IsSystemAccessControlType() && s.Identifier.StartsWith(this.m_namespaceIdString, StringComparison.OrdinalIgnoreCase)));

    private SecurityEvaluator.EvaluationResult ComputeEffectivePermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      bool principalHasStatelessAces,
      bool isTracing,
      string descriptorString,
      out bool noChildrenBelow,
      out string noChildrenBelowToken,
      int bitsToConsider = -1,
      bool forQueryWithExtendedInfo = false)
    {
      noChildrenBelow = false;
      noChildrenBelowToken = (string) null;
      try
      {
        SecurityEvaluator.EvaluationResult result = new SecurityEvaluator.EvaluationResult();
        string str1 = isTracing ? EuiiUtility.MaskEmail(token ?? "<null>") : string.Empty;
        if (isTracing)
        {
          requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: Starting to evaluate effective permissions for token {0} and descriptor {1} in ACL store {2}.", (object) str1, (object) descriptorString, (object) this.m_aclStoreIdString);
          if (-1 != bitsToConsider)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: This evaluation is bitmask-constrained. Bits to consider: {0}.", (object) bitsToConsider);
        }
        bool firstParent = true;
        bool tempNoChildrenBelow = false;
        string tempNoChildrenBelowToken = (string) null;
        this.m_tokenStore.EnumAndEvaluateParents(token, principalHasStatelessAces, (Func<string, AccessControlListSlim, string, bool, bool>) ((nodeToken, nodeAcl, nodeNoChildrenBelow, isExactMatch) =>
        {
          IEnumerable<AccessControlEntrySlim> permissions = (IEnumerable<AccessControlEntrySlim>) SecurityEvaluator.s_emptyAceList;
          bool flag = true;
          int num1 = 0;
          if (nodeAcl != null)
          {
            permissions = nodeAcl.AccessControlEntries;
            flag = nodeAcl.InheritPermissions;
            num1 = nodeAcl.Count;
          }
          string str2 = isTracing ? EuiiUtility.MaskEmail(token ?? "<null>") : string.Empty;
          if (isTracing)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: ACL for token {0} has {1} ACEs", (object) str2, (object) num1);
          if (firstParent && !principalHasStatelessAces && (nodeToken == null || nodeNoChildrenBelow != null))
          {
            tempNoChildrenBelow = true;
            tempNoChildrenBelowToken = nodeNoChildrenBelow;
          }
          firstParent = false;
          int num2;
          if (principalHasStatelessAces)
          {
            num2 = -1;
          }
          else
          {
            num2 = 0;
            foreach (AccessControlEntrySlim controlEntrySlim in permissions)
              num2 |= controlEntrySlim.Allow | controlEntrySlim.Deny;
            if (isTracing)
              requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: Possible bits across all ACEs in the token: {0}. Bits already set: {1}.", (object) num2, (object) (result.Allow | result.Deny));
          }
          int bitsToConsider1 = num2 & bitsToConsider & ~(result.Allow | result.Deny);
          if (bitsToConsider1 != 0)
          {
            int effectiveAllow;
            int effectiveDeny;
            this.EvaluatePermissions(requestContext, evaluationPrincipal, nodeToken, token, permissions, descriptorString, isTracing, out effectiveAllow, out effectiveDeny, bitsToConsider1, forQueryWithExtendedInfo);
            if (isTracing)
              requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: For token {0}, allow is {1} and deny is {2}", (object) str2, (object) effectiveAllow, (object) effectiveDeny);
            result.Allow |= effectiveAllow & ~result.Deny;
            result.Deny |= effectiveDeny & ~result.Allow;
            if (isTracing)
              requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: Effective allow is now {0}, effective deny is now {1}", (object) result.Allow, (object) result.Deny);
          }
          else if (isTracing)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: For token {0}, no bits can be added to allow or deny; skipping evaluation", (object) str2);
          if (forQueryWithExtendedInfo && !isExactMatch)
          {
            int num3 = 0;
            int num4 = 0;
            if (nodeAcl != null)
            {
              AccessControlEntrySlim controlEntrySlim = nodeAcl.QueryAccessControlEntry(evaluationPrincipal.PrimaryDescriptor);
              num3 = controlEntrySlim.Allow;
              num4 = controlEntrySlim.Deny;
            }
            if (isTracing)
              requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: For token {0}, specific allow is {1} and specific deny is {2}", (object) str2, (object) num3, (object) num4);
            result.InheritedAllow |= num3 & ~result.InheritedDeny;
            result.InheritedDeny |= num4 & ~result.InheritedAllow;
            if (isTracing)
              requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: Inherited allow is now {0}, inherited deny is now {1}", (object) result.InheritedAllow, (object) result.InheritedDeny);
          }
          if (flag)
            return true;
          if (isTracing)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: Ending evaluation because ACL for token {0} has inheritance disabled", (object) str2);
          return false;
        }));
        noChildrenBelow = tempNoChildrenBelow;
        noChildrenBelowToken = tempNoChildrenBelowToken;
        if (isTracing)
        {
          if (forQueryWithExtendedInfo)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: Final result for token {0} and descriptor {1}: effective allow {2}, effective deny {3}, inherited allow {4}, inherited deny {5}", (object) str1, (object) descriptorString, (object) result.Allow, (object) result.Deny, (object) result.InheritedAllow, (object) result.InheritedDeny);
          else
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "ComputeEffectivePermissions: Final result for token {0} and descriptor {1}: effective allow {2}, effective deny {3}", (object) str1, (object) descriptorString, (object) result.Allow, (object) result.Deny);
        }
        return result;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56807, "Security", nameof (SecurityEvaluator), ex);
        throw;
      }
    }

    private void EvaluatePermissions(
      IVssRequestContext requestContext,
      EvaluationPrincipal evaluationPrincipal,
      string aceToken,
      string evalToken,
      IEnumerable<AccessControlEntrySlim> permissions,
      string principalString,
      bool isTracing,
      out int effectiveAllow,
      out int effectiveDeny,
      int bitsToConsider = -1,
      bool forQueryWithExtendedInfo = false)
    {
      effectiveAllow = 0;
      effectiveDeny = 0;
      if (isTracing && -1 != bitsToConsider)
        requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: This evaluation is bitmask-constrained. Bits to consider: {0}.", (object) bitsToConsider);
      foreach (IdentityDescriptor roleDescriptor in (IEnumerable<IdentityDescriptor>) evaluationPrincipal.RoleDescriptors)
      {
        string str1 = (string) null;
        if (isTracing)
        {
          str1 = SecurityServiceHelpers.DescriptorToString(requestContext, roleDescriptor);
          requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Evaluating role descriptor {0}", (object) str1);
        }
        int descriptorAllow;
        int descriptorDeny;
        if (roleDescriptor.IsSystemAccessControlType() && roleDescriptor.Identifier.StartsWith(this.m_namespaceIdString, StringComparison.OrdinalIgnoreCase) && SecurityEvaluator.AccessControlDescriptorMatchesToken(roleDescriptor.Identifier, aceToken, out descriptorAllow, out descriptorDeny))
        {
          effectiveAllow |= descriptorAllow & bitsToConsider;
          effectiveDeny |= descriptorDeny & bitsToConsider;
          if (isTracing)
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Access control role descriptor matched the token; applying the stateless ACE. Allow bits now {1}, deny bits now {2}.", (object) principalString, (object) effectiveAllow, (object) effectiveDeny);
        }
        foreach (AccessControlEntrySlim permission in permissions)
        {
          bool flag = false;
          string str2 = (string) null;
          if (isTracing)
          {
            str2 = SecurityServiceHelpers.DescriptorToString(requestContext, permission.Descriptor);
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: ACE for descriptor {0} has allow {1} and deny {2}", (object) str2, (object) permission.Allow, (object) permission.Deny);
          }
          int num1 = permission.Allow & bitsToConsider & ~effectiveAllow;
          int num2 = permission.Deny & bitsToConsider & ~effectiveDeny;
          if (num1 == 0 && num2 == 0)
          {
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: All allow and deny bits from this ACE are already present in the result or are not considerable; skipping");
          }
          else
          {
            if (IdentityDescriptorComparer.Instance.Equals(permission.Descriptor, roleDescriptor))
            {
              flag = true;
              if (isTracing)
                requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Role descriptor {0} matches ACE descriptor {1}; this ACE will be applied", (object) str1, (object) str2);
            }
            else if (isTracing)
              requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Role descriptor {0} does not match ACE descriptor {1}; moving to next evaluation role", (object) str1, (object) str2);
            if (flag)
            {
              effectiveAllow |= permission.Allow & bitsToConsider;
              effectiveDeny |= permission.Deny & bitsToConsider;
              if (isTracing)
                requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: For evaluation principal {0}, allow bits now {1}, deny bits now {2}", (object) principalString, (object) effectiveAllow, (object) effectiveDeny);
            }
          }
        }
      }
      if ((IdentityDescriptor) null != evaluationPrincipal.MembershipDescriptor)
      {
        string str3 = (string) null;
        if (isTracing)
        {
          str3 = SecurityServiceHelpers.DescriptorToString(requestContext, evaluationPrincipal.MembershipDescriptor);
          requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Evaluating membership descriptor {0}", (object) str3);
        }
        foreach (AccessControlEntrySlim permission in permissions)
        {
          bool flag = false;
          string str4 = (string) null;
          if (isTracing)
          {
            str4 = SecurityServiceHelpers.DescriptorToString(requestContext, permission.Descriptor);
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: ACE for descriptor {0} has allow {1} and deny {2}", (object) str4, (object) permission.Allow, (object) permission.Deny);
          }
          int requestedPermissions = permission.Allow & bitsToConsider & ~effectiveAllow;
          int num = permission.Deny & bitsToConsider & ~effectiveDeny;
          if (requestedPermissions == 0 && num == 0)
          {
            requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: All allow and deny bits from this ACE are already present in the result or are not considerable; skipping");
          }
          else
          {
            if (IdentityDescriptorComparer.Instance.Equals(permission.Descriptor, evaluationPrincipal.MembershipDescriptor))
            {
              flag = true;
              if (isTracing)
                requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Membership descriptor {0} matches ACE descriptor {1}; this ACE will be applied", (object) str3, (object) str4);
            }
            else if (this.IsMemberInternal(requestContext, permission.Descriptor, evaluationPrincipal.MembershipDescriptor, forQueryWithExtendedInfo, requestedPermissions, aceToken, evalToken))
            {
              flag = true;
              if (isTracing)
                requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Membership descriptor {0} is a member of ACE descriptor {1}; this ACE will be applied", (object) str3, (object) str4);
            }
            else if (isTracing)
              requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Membership descriptor {0} does not match ACE descriptor {1}; the ACE will *not* be applied", (object) str3, (object) str4);
            if (flag)
            {
              effectiveAllow |= permission.Allow & bitsToConsider;
              effectiveDeny |= permission.Deny & bitsToConsider;
              if (isTracing)
                requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: For evaluation principal {0}, allow bits now {1}, deny bits now {2}", (object) principalString, (object) effectiveAllow, (object) effectiveDeny);
            }
          }
        }
      }
      effectiveAllow &= ~effectiveDeny;
      if (!isTracing)
        return;
      requestContext.TraceSecurityEvaluation(56808, TraceLevel.Verbose, "Security", nameof (SecurityEvaluator), "EvaluatePermissions: Final result for principal {0} is allow of {1} and deny of {2}", (object) principalString, (object) effectiveAllow, (object) effectiveDeny);
    }

    private static bool AccessControlDescriptorMatchesToken(
      string descriptorIdentifier,
      string tokenToMatch,
      out int descriptorAllow,
      out int descriptorDeny)
    {
      descriptorAllow = 0;
      descriptorDeny = 0;
      if (descriptorIdentifier.Length < 41 || descriptorIdentifier[36] != ';')
        return false;
      int num1 = descriptorIdentifier.IndexOf(';', 38);
      if (num1 < 38)
        return false;
      int num2 = descriptorIdentifier.IndexOf(';', num1 + 1);
      if (num2 < 40 || !StringComparer.OrdinalIgnoreCase.Equals(tokenToMatch, descriptorIdentifier.Substring(num2 + 1)))
        return false;
      if (!int.TryParse(descriptorIdentifier.Substring(37, num1 - 37), out descriptorAllow))
      {
        descriptorAllow = 0;
        return false;
      }
      if (int.TryParse(descriptorIdentifier.Substring(num1 + 1, num2 - num1 - 1), out descriptorDeny))
        return true;
      descriptorDeny = 0;
      return false;
    }

    private bool IsMemberInternal(
      IVssRequestContext requestContext,
      IdentityDescriptor group,
      IdentityDescriptor member,
      bool forQueryWithExtendedInfo,
      int requestedPermissions,
      string aceToken,
      string evalToken)
    {
      return SecurityEvaluator.IsMemberInternal(requestContext, this.m_settingsService, (IdentityService) this.m_identityService, group, member, forQueryWithExtendedInfo, this.m_namespaceId, requestedPermissions, aceToken, evalToken);
    }

    internal static bool IsMemberInternal(
      IVssRequestContext requestContext,
      SecuritySettingsService settingsService,
      IdentityService identityService,
      IdentityDescriptor group,
      IdentityDescriptor member,
      bool forQueryWithExtendedInfo,
      Guid namespaceId,
      int requestedPermissions,
      string aceToken,
      string evalToken)
    {
      bool flag1 = !group.IdentityType.StartsWith("System:", StringComparison.Ordinal) && !member.IdentityType.StartsWith("System:", StringComparison.Ordinal);
      if (flag1 && ServicePrincipals.IsServicePrincipal(requestContext, member))
        flag1 = false;
      bool flag2;
      if (flag1)
      {
        if (SecurityEvaluator.ShouldCallIsMemberWithRootContext(requestContext, namespaceId))
        {
          requestContext.TraceSecurityEvaluation(56901, TraceLevel.Info, "Security", nameof (SecurityEvaluator), string.Format("Calling IsMember with the root context. Namespace: {0}, Group: {1}, Member: {2}, AceToken: {3}, EvalToken: {4}, RequestedPermission: {5}.", (object) namespaceId, (object) group, (object) member, (object) aceToken, (object) evalToken, (object) requestedPermissions));
          IVssRequestContext vssRequestContext = requestContext.RootContext;
          if (requestContext.IsSystemContext)
          {
            requestContext.TraceSecurityEvaluation(56902, TraceLevel.Warning, "Security", nameof (SecurityEvaluator), "The original context is a system context, elevating the IsMember context as well. This may have a performance hit.");
            vssRequestContext = vssRequestContext.Elevate();
          }
          flag2 = vssRequestContext.GetService<IdentityService>().IsMember(vssRequestContext, group, member);
        }
        else
          flag2 = identityService.IsMember(requestContext, group, member);
      }
      else
      {
        flag2 = false;
        if (group.IsSystemServicePrincipalType() && string.Equals(group.Identifier, "*", StringComparison.Ordinal))
          flag2 = ServicePrincipals.IsServicePrincipal(requestContext, member);
        Guid spId;
        if (group.IsTeamFoundationType() && group.Identifier.EndsWith("-0-0-0-0-1", StringComparison.Ordinal) && ServicePrincipals.IsServicePrincipal(requestContext, member, false, out spId) && spId == SecurityEvaluator.s_l2AdminServicePrincipal)
          flag2 = true;
      }
      return flag2;
    }

    private static bool ShouldCallIsMemberWithRootContext(
      IVssRequestContext requestContext,
      Guid namespaceId)
    {
      return requestContext?.RootContext != null && !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application) && !(requestContext.ServiceHost.InstanceId == requestContext.RootContext.ServiceHost.InstanceId) && (!(namespaceId != FrameworkSecurity.IdentitiesNamespaceId) || !(namespaceId != OrganizationSecurity.NamespaceId) || !(namespaceId != SecurityEvaluator.s_licensingSecurityNamespaceId) || !(namespaceId != SecurityEvaluator.s_userEntitlementSecurityNamespaceId)) && requestContext.IsFeatureEnabled("VisualStudio.Services.Security.ToParentSecurityNamespace.CallIsMemberWithRootContext");
    }

    internal static long CacheHitCount => 0;

    private struct CacheEntry
    {
      public string Token;
      public EvaluationPrincipal EvaluationPrincipal;
      public int IdentityChangeId;
      public SecurityEvaluator.EvaluationResult EvaluationResult;
      public int ValidBits;
    }

    public struct EvaluationResult
    {
      public int Allow;
      public int Deny;
      public int InheritedAllow;
      public int InheritedDeny;
    }
  }
}
