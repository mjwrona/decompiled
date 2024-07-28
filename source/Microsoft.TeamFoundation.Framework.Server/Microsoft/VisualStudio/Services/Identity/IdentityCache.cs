// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityCache : VssCacheBase
  {
    private readonly ILockName m_cacheLockName;
    private readonly VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>[] m_descriptorCache;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>[] m_subjectDescriptorCache;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SocialDescriptor>[] m_socialDescriptorCache;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>[] m_idCache;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>[] m_accountNameCache;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>[] m_localGroupNameCache;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>[] m_aliasCache;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>[] m_memberReferences;
    private readonly VssMemoryCacheList<IdentityDescriptor, bool> m_hasAadGroupsCache;
    private readonly VssMemoryCacheList<IdentityDescriptor, IdentityMembershipInfo> m_isMemberCacheByDescriptor;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, IdentityMembershipInfo, Guid> m_isMemberCacheById;
    private readonly IVssMemoryCacheGrouping<IdentityDescriptor, IdentityMembershipInfo, IdentityDescriptor> m_isMemberCacheByParent;
    private readonly VssMemoryCacheList<Guid, Guid> m_scopeParentIdCache;
    private readonly VssMemoryCacheList<Guid, IdentityScope> m_scopeCacheById;
    private readonly VssMemoryCacheList<Guid, HashSet<Guid>> m_scopeAncestorIdsCache;
    private readonly IdentityDomain m_hostDomain;
    private readonly bool m_cacheEvictionEnabled;
    private static readonly VssCacheExpiryProvider<IdentityDescriptor, IdentityMembershipInfo> s_isMemberCacheExpiryProvider = new VssCacheExpiryProvider<IdentityDescriptor, IdentityMembershipInfo>(Capture.Create<TimeSpan>(TimeSpan.FromHours(24.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
    private static readonly VssCacheExpiryProvider<Guid, IdentityScope> s_scopeCacheByIdExpiryProvider = new VssCacheExpiryProvider<Guid, IdentityScope>(Capture.Create<TimeSpan>(TimeSpan.FromHours(24.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
    private static readonly VssCacheExpiryProvider<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> s_MsaIdentityDescriptorExpiryProvider = new VssCacheExpiryProvider<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry), Capture.Create<TimeSpan>(TimeSpan.FromHours(8.0)));
    private const int c_maxMembershipQuery = 1;
    private const int c_DescriptorCacheExpiryIntervalInHours = 8;
    private const string c_UseVssMemoryCacheGroupingForMembershipCache = "VisualStudio.Services.IdentityCache.Membership.UseVssMemoryCacheGrouping";
    private const string c_DontApplyNonOrgLevelMembershipChangesToOrgScopeCache = "VisualStudio.Services.IdentityCache.DontApplyNonOrgLevelMembershipChangesToOrgScopeCache";
    private const string c_UseDefaultTTLForMsaIdentityInDescriptorCache = "VisualStudio.Services.IdentityCache.UseDefaultTTLForMsaIdentitiesInDescriptorCache";
    private const string c_shouldAddInvalidatedIdentityMembershipInfoEvenWhenKeyIsMissing = "VisualStudio.Services.IdentityCache.AddInvalidatedIdentityMembershipInfoEvenWhenKeyIsMissing";
    private const string c_UseExpiryInNonDeploymentDescriptorCache = "VisualStudio.Services.IdentityCache.UseExpiryInNonDeploymentDescriptorCache";
    private readonly bool m_shouldAddInvalidatedIdentityMembershipInfoEvenWhenKeyIsMissing;
    private const string s_area = "IdentityService";
    private const string s_layer = "IdentityCache";

    public IdentityCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      int cacheSize,
      IScopeMapper scopeMapper = null,
      IIdMapper idMapper = null)
    {
      this.ScopeMapper = scopeMapper;
      this.IdMapper = idMapper;
      this.m_descriptorCache = new VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>[2];
      this.m_idCache = new IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>[2];
      this.m_subjectDescriptorCache = new IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>[2];
      this.m_socialDescriptorCache = new IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SocialDescriptor>[2];
      this.m_accountNameCache = new IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>[2];
      this.m_localGroupNameCache = new IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>[2];
      this.m_aliasCache = new IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>[2];
      this.m_memberReferences = new IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>[2];
      this.m_isMemberCacheByDescriptor = new VssMemoryCacheList<IdentityDescriptor, IdentityMembershipInfo>((IVssCachePerformanceProvider) this, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance, cacheSize, IdentityCache.s_isMemberCacheExpiryProvider);
      this.m_shouldAddInvalidatedIdentityMembershipInfoEvenWhenKeyIsMissing = requestContext.IsFeatureEnabled("VisualStudio.Services.IdentityCache.AddInvalidatedIdentityMembershipInfoEvenWhenKeyIsMissing");
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.IdentityCache.Membership.UseVssMemoryCacheGrouping"))
      {
        this.m_isMemberCacheById = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, IdentityMembershipInfo, Guid>(requestContext, this.m_isMemberCacheByDescriptor, (Func<IdentityDescriptor, IdentityMembershipInfo, IEnumerable<Guid>>) ((K, V) =>
        {
          if (V.IsInvalid())
            return (IEnumerable<Guid>) null;
          return (IEnumerable<Guid>) new Guid[1]
          {
            V.IdentityId
          };
        }));
        this.m_isMemberCacheByParent = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, IdentityMembershipInfo, IdentityDescriptor>(requestContext, this.m_isMemberCacheByDescriptor, (Func<IdentityDescriptor, IdentityMembershipInfo, IEnumerable<IdentityDescriptor>>) ((K, V) => !V.IsInvalid() ? (IEnumerable<IdentityDescriptor>) V.Parents : (IEnumerable<IdentityDescriptor>) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      }
      else
      {
        this.m_isMemberCacheById = (IVssMemoryCacheGrouping<IdentityDescriptor, IdentityMembershipInfo, Guid>) new LegacyVssMemoryCacheGrouping<IdentityDescriptor, IdentityMembershipInfo, Guid>(this.m_isMemberCacheByDescriptor, (Func<IdentityDescriptor, IdentityMembershipInfo, IEnumerable<Guid>>) ((K, V) =>
        {
          if (V.IsInvalid())
            return (IEnumerable<Guid>) null;
          return (IEnumerable<Guid>) new Guid[1]
          {
            V.IdentityId
          };
        }));
        this.m_isMemberCacheByParent = (IVssMemoryCacheGrouping<IdentityDescriptor, IdentityMembershipInfo, IdentityDescriptor>) new LegacyVssMemoryCacheGrouping<IdentityDescriptor, IdentityMembershipInfo, IdentityDescriptor>(this.m_isMemberCacheByDescriptor, (Func<IdentityDescriptor, IdentityMembershipInfo, IEnumerable<IdentityDescriptor>>) ((K, V) => !V.IsInvalid() ? (IEnumerable<IdentityDescriptor>) V.Parents : (IEnumerable<IdentityDescriptor>) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      }
      this.m_hasAadGroupsCache = new VssMemoryCacheList<IdentityDescriptor, bool>((IVssCachePerformanceProvider) this, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      this.m_scopeParentIdCache = new VssMemoryCacheList<Guid, Guid>((IVssCachePerformanceProvider) this);
      this.m_scopeAncestorIdsCache = new VssMemoryCacheList<Guid, HashSet<Guid>>((IVssCachePerformanceProvider) this);
      this.m_scopeCacheById = new VssMemoryCacheList<Guid, IdentityScope>((IVssCachePerformanceProvider) this, IdentityCache.s_scopeCacheByIdExpiryProvider);
      this.m_cacheLockName = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}.{1}.{2}", (object) nameof (IdentityCache), (object) nameof (m_cacheLockName), (object) hostDomain.DomainId));
      this.m_hostDomain = hostDomain;
      this.LastWarmedTime = DateTime.MinValue;
      this.IsWarm = false;
      VssCacheExpiryProvider<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> expiryProvider = (VssCacheExpiryProvider<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) null;
      this.m_cacheEvictionEnabled = false;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        this.m_cacheEvictionEnabled = service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Integration/Settings/IdentityEvictionEnabled", false);
        if (this.m_cacheEvictionEnabled)
        {
          int num = Math.Max(1, service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Integration/Settings/IdentityInactivityIntervalInHours", 8));
          expiryProvider = new VssCacheExpiryProvider<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry), Capture.Create<TimeSpan>(TimeSpan.FromHours((double) num)));
        }
      }
      else
      {
        this.m_cacheEvictionEnabled = requestContext.IsImsFeatureEnabled("VisualStudio.Services.IdentityCache.UseExpiryInNonDeploymentDescriptorCache");
        if (this.m_cacheEvictionEnabled)
        {
          int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Integration/Settings/IdentityExpiryIntervalInHours", true, 8);
          requestContext.Trace(898011, TraceLevel.Info, "IdentityService", nameof (IdentityCache), "Expiration set to {0} hours", (object) num);
          expiryProvider = new VssCacheExpiryProvider<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>(Capture.Create<TimeSpan>(TimeSpan.FromHours((double) num)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
        }
      }
      for (int index = 0; index <= 1; ++index)
      {
        this.m_descriptorCache[index] = !this.m_cacheEvictionEnabled ? new VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((IVssCachePerformanceProvider) this, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance, cacheSize) : new VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((IVssCachePerformanceProvider) this, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance, cacheSize, expiryProvider);
        this.m_idCache[index] = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>(requestContext, this.m_descriptorCache[index], (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<Guid>>) ((K, V) => (IEnumerable<Guid>) new Guid[1]
        {
          V.Id
        }), groupingBehavior: VssMemoryCacheGroupingBehavior.Replace);
        this.m_subjectDescriptorCache[index] = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>(requestContext, this.m_descriptorCache[index], (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<SubjectDescriptor>>) ((K, V) => (IEnumerable<SubjectDescriptor>) new SubjectDescriptor[1]
        {
          V.SubjectDescriptor
        }), groupingBehavior: VssMemoryCacheGroupingBehavior.Replace);
        this.m_socialDescriptorCache[index] = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SocialDescriptor>(requestContext, this.m_descriptorCache[index], (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<SocialDescriptor>>) ((K, V) => (IEnumerable<SocialDescriptor>) new SocialDescriptor[1]
        {
          V.SocialDescriptor
        }), groupingBehavior: VssMemoryCacheGroupingBehavior.Replace);
        this.m_accountNameCache[index] = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>(requestContext, this.m_descriptorCache[index], (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<string>>) ((K, V) => (IEnumerable<string>) new string[1]
        {
          IdentityHelper.GetUniqueName(V)
        }), (IEqualityComparer<string>) VssStringComparer.DomainName, VssMemoryCacheGroupingBehavior.Replace);
        this.m_localGroupNameCache[index] = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>(requestContext, this.m_descriptorCache[index], (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<string>>) ((K, V) => (IEnumerable<string>) new string[1]
        {
          IdentityHelper.GetUniqueName(V)
        }), (IEqualityComparer<string>) VssStringComparer.DomainName, VssMemoryCacheGroupingBehavior.Replace);
        this.m_aliasCache[index] = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>(requestContext, this.m_descriptorCache[index], (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<string>>) ((K, V) =>
        {
          string property = V.GetProperty<string>("Alias", (string) null);
          if (string.IsNullOrEmpty(property))
            return (IEnumerable<string>) null;
          return (IEnumerable<string>) new string[1]
          {
            property
          };
        }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, VssMemoryCacheGroupingBehavior.Replace);
        VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> cache = this.m_descriptorCache[index];
        if (index == 0)
          cache = new VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((IVssCachePerformanceProvider) this, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance, 0);
        this.m_memberReferences[index] = VssMemoryCacheGroupingFactory.Create<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>(requestContext, cache, (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<Guid>>) ((K, V) => (IEnumerable<Guid>) V.MemberIds));
      }
    }

    public Guid GetOrAddScopeParent(Guid scopeId, Guid parentId)
    {
      this.m_scopeParentIdCache.Add(scopeId, parentId, false);
      return parentId;
    }

    public bool TryReadScopeParent(Guid scopeId, out Guid parentId) => this.m_scopeParentIdCache.TryGetValue(scopeId, out parentId);

    public bool UpdateScopeAncestorIds(Guid scopeId, HashSet<Guid> ancestorIds) => this.m_scopeAncestorIdsCache.Add(scopeId, ancestorIds, true);

    public bool TryGetScope(Guid scopeId, out IdentityScope scope)
    {
      scope = (IdentityScope) null;
      IdentityScope identityScope;
      if (!this.m_scopeCacheById.TryGetValue(scopeId, out identityScope) || identityScope == null)
        return false;
      scope = identityScope.Clone();
      return true;
    }

    public bool AddScope(Guid scopeId, IdentityScope scope) => scope != null && this.m_scopeCacheById.Add(scopeId, scope.Clone(), true);

    public bool TryReadScopeAncestorIds(Guid scopeId, out HashSet<Guid> ancestorIds) => this.m_scopeAncestorIdsCache.TryGetValue(scopeId, out ancestorIds);

    public void Add(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      QueryMembership queryMembership)
    {
      if (identity == null || !identity.IsActive && identity.IsContainer)
        return;
      bool flag1 = requestContext.IsImsFeatureEnabled("VisualStudio.Services.IdentityCache.UseDefaultTTLForMsaIdentitiesInDescriptorCache");
      bool flag2 = !requestContext.IsImsFeatureEnabled("VisualStudio.Services.IdentityCache.UseExpiryInNonDeploymentDescriptorCache");
      using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
      {
        if (queryMembership > QueryMembership.Direct)
          return;
        identity = identity.Clone();
        if (this.m_cacheEvictionEnabled & flag2 & flag1 && identity.IsMsaIdentity())
          this.m_descriptorCache[(int) queryMembership].Add(identity.Descriptor, identity, true, IdentityCache.s_MsaIdentityDescriptorExpiryProvider);
        else
          this.m_descriptorCache[(int) queryMembership][identity.Descriptor] = identity;
      }
    }

    public void SetParentMemberships(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IdentityMembershipInfo parentMembershipInfo = IdentityMembershipInfoUtils.CreateParentMembershipInfo(identity);
      VssCacheExpiryProvider<IdentityDescriptor, IdentityMembershipInfo> expiryProvider = (VssCacheExpiryProvider<IdentityDescriptor, IdentityMembershipInfo>) null;
      DateTime dateTime;
      if (identity.Properties.TryGetValue<DateTime>("CacheMaxAge", out dateTime))
      {
        long cacheTTLInTicks = dateTime.Ticks - DateTime.UtcNow.Ticks;
        if (cacheTTLInTicks <= 0L)
          return;
        expiryProvider = IdentityCache.CreateCacheExpiryProvider(cacheTTLInTicks);
      }
      using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
      {
        if (expiryProvider == null)
          this.m_isMemberCacheByDescriptor[identity.Descriptor] = parentMembershipInfo;
        else
          this.m_isMemberCacheByDescriptor.Add(identity.Descriptor, parentMembershipInfo, true, expiryProvider);
      }
    }

    public bool CompareAndSwapParentMemberships(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      long cacheStamp)
    {
      VssCacheExpiryProvider<IdentityDescriptor, IdentityMembershipInfo> expiryProvider = (VssCacheExpiryProvider<IdentityDescriptor, IdentityMembershipInfo>) null;
      DateTime dateTime;
      if (identity.Properties.TryGetValue<DateTime>("CacheMaxAge", out dateTime))
      {
        long cacheTTLInTicks = dateTime.Ticks - DateTime.UtcNow.Ticks;
        if (cacheTTLInTicks <= 0L)
          return false;
        expiryProvider = IdentityCache.CreateCacheExpiryProvider(cacheTTLInTicks);
      }
      using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
      {
        IdentityMembershipInfo membershipInfo;
        this.m_isMemberCacheByDescriptor.TryGetValue(identity.Descriptor, out membershipInfo);
        requestContext.Trace(898012, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "CompareAndSwapParentMemberships input parameters , identity.Id = {0}, identity.Descriptor.IdentityType = {1}, cacheStamp = {2},  currentMembershipInfo.CacheStamp = {3}", (object) identity.Id, (object) identity.Descriptor?.IdentityType, (object) cacheStamp, (object) membershipInfo.GetCacheStamp());
        if (membershipInfo.GetCacheStamp() != cacheStamp)
          return false;
        IdentityMembershipInfo parentMembershipInfo = IdentityMembershipInfoUtils.CreateParentMembershipInfo(identity);
        if (expiryProvider == null)
          this.m_isMemberCacheByDescriptor[identity.Descriptor] = parentMembershipInfo;
        else
          this.m_isMemberCacheByDescriptor.Add(identity.Descriptor, parentMembershipInfo, true, expiryProvider);
        return true;
      }
    }

    private bool RemoveParents(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      Guid identityId)
    {
      requestContext.TraceEnter(899021, "IdentityService", nameof (IdentityCache), nameof (RemoveParents));
      requestContext.Trace(899022, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "RemoveParents input parameters , m_hostDomain = {0}, descriptor.IdentityType = {1},  identityId = {2}", (object) this.m_hostDomain, (object) descriptor?.IdentityType, (object) identityId);
      using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
      {
        bool flag = false;
        if (descriptor != (IdentityDescriptor) null)
          flag |= this.InvalidateIsMemberCacheByDescriptor(requestContext, descriptor);
        IEnumerable<IdentityDescriptor> keys;
        if (!identityId.Equals(Guid.Empty) && this.m_isMemberCacheById.TryGetKeys(identityId, out keys))
        {
          foreach (IdentityDescriptor descriptor1 in keys)
            flag |= this.InvalidateIsMemberCacheByDescriptor(requestContext, descriptor1);
        }
        requestContext.TraceLeave(899029, "IdentityService", nameof (IdentityCache), nameof (RemoveParents));
        return flag;
      }
    }

    public bool Remove(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      QueryMembership? queryMembership = null)
    {
      requestContext.TraceEnter(899031, "IdentityService", nameof (IdentityCache), nameof (Remove));
      requestContext.TraceConditionally(899032, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), (Func<string>) (() => string.Format("Remove input parameters , m_hostDomain = {0}, descriptor = {1}, queryMembership = {2}", (object) this.m_hostDomain, (object) SecurityServiceHelpers.DescriptorToString(requestContext, descriptor), (object) queryMembership)));
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentity(requestContext, descriptor, queryMembership);
      if (identity != null)
        return this.Remove(requestContext, identity, queryMembership);
      if (queryMembership.HasValue)
      {
        QueryMembership? nullable1 = queryMembership;
        QueryMembership queryMembership1 = QueryMembership.Expanded;
        if (!(nullable1.GetValueOrDefault() == queryMembership1 & nullable1.HasValue))
        {
          QueryMembership? nullable2 = queryMembership;
          QueryMembership queryMembership2 = QueryMembership.ExpandedUp;
          if (!(nullable2.GetValueOrDefault() == queryMembership2 & nullable2.HasValue))
          {
            QueryMembership? nullable3 = queryMembership;
            QueryMembership queryMembership3 = QueryMembership.ExpandedDown;
            if (!(nullable3.GetValueOrDefault() == queryMembership3 & nullable3.HasValue))
            {
              requestContext.TraceLeave(899039, "IdentityService", nameof (IdentityCache), nameof (Remove));
              return false;
            }
          }
        }
      }
      return this.RemoveParents(requestContext, descriptor, Guid.Empty);
    }

    public bool Remove(
      IVssRequestContext requestContext,
      Guid identityId,
      QueryMembership? queryMembership = null)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentity(requestContext, identityId, queryMembership);
      requestContext.TraceEnter(899041, "IdentityService", nameof (IdentityCache), nameof (Remove));
      requestContext.Trace(899042, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "Remove input parameters , m_hostDomain = {0}, identityId = {1}, queryMembership = {2}, identity CUID = {3}", (object) this.m_hostDomain, (object) identityId, (object) queryMembership, (object) identity?.GetProperty<Guid>("CUID", Guid.Empty));
      if (identity != null)
        return this.Remove(requestContext, identity, queryMembership);
      if (queryMembership.HasValue)
      {
        QueryMembership? nullable1 = queryMembership;
        QueryMembership queryMembership1 = QueryMembership.Expanded;
        if (!(nullable1.GetValueOrDefault() == queryMembership1 & nullable1.HasValue))
        {
          QueryMembership? nullable2 = queryMembership;
          QueryMembership queryMembership2 = QueryMembership.ExpandedUp;
          if (!(nullable2.GetValueOrDefault() == queryMembership2 & nullable2.HasValue))
          {
            QueryMembership? nullable3 = queryMembership;
            QueryMembership queryMembership3 = QueryMembership.ExpandedDown;
            if (!(nullable3.GetValueOrDefault() == queryMembership3 & nullable3.HasValue))
            {
              requestContext.TraceLeave(899049, "IdentityService", nameof (IdentityCache), nameof (Remove));
              return false;
            }
          }
        }
      }
      return this.RemoveParents(requestContext, (IdentityDescriptor) null, identityId);
    }

    public bool Remove(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      QueryMembership? queryMembership = null)
    {
      requestContext.TraceEnter(899051, "IdentityService", nameof (IdentityCache), nameof (Remove));
      requestContext.Trace(899052, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "Remove input parameters , m_hostDomain = {0}, identity CUID = {1}, queryMembership = {2}", (object) this.m_hostDomain, (object) identity?.GetProperty<Guid>("CUID", identity.Id), (object) queryMembership);
      try
      {
        using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
        {
          bool flag1 = false;
          if (queryMembership.HasValue)
          {
            QueryMembership? nullable = queryMembership;
            QueryMembership queryMembership1 = QueryMembership.Expanded;
            if (!(nullable.GetValueOrDefault() == queryMembership1 & nullable.HasValue))
            {
              nullable = queryMembership;
              QueryMembership queryMembership2 = QueryMembership.ExpandedUp;
              if (!(nullable.GetValueOrDefault() == queryMembership2 & nullable.HasValue))
              {
                nullable = queryMembership;
                QueryMembership queryMembership3 = QueryMembership.ExpandedDown;
                if (!(nullable.GetValueOrDefault() == queryMembership3 & nullable.HasValue))
                  goto label_7;
              }
            }
          }
          requestContext.Trace(899023, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "Calling InvalidateIsMemberCacheByDescriptor with m_hostDomain = {0}, identity.Id = {1}, identity.Descriptor.IdentityType = {2}", (object) this.m_hostDomain, (object) identity.Id, (object) identity.Descriptor?.IdentityType);
          flag1 |= this.InvalidateIsMemberCacheByDescriptor(requestContext, identity.Descriptor);
label_7:
          for (int index = 0; index <= 1; ++index)
          {
            if (!queryMembership.HasValue || queryMembership.Value == (QueryMembership) index)
            {
              bool flag2 = this.m_descriptorCache[index].Remove(identity.Descriptor);
              flag1 |= flag2;
              requestContext.Trace(899024, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "Removed identity for m_hostDomain = {0}, identity.Id = {1}, identity.Descriptor.IdentityType = {2}, isRemoved = {3}", (object) this.m_hostDomain, (object) identity.Id, (object) identity.Descriptor?.IdentityType, (object) flag2);
            }
          }
          return flag1;
        }
      }
      finally
      {
        this.OnIdentityEvicted(new IdentityRemovedEventArgs()
        {
          IdentityId = identity.Id,
          IdentityDomain = this.m_hostDomain
        });
        requestContext.TraceLeave(899059, "IdentityService", nameof (IdentityCache), nameof (Remove));
      }
    }

    public void Sweep(IVssRequestContext requestContext)
    {
      bool flag = requestContext.IsTracing(4440016, TraceLevel.Info, "IdentityService", nameof (IdentityCache));
      for (int index = 0; index <= 1; ++index)
      {
        if (flag)
        {
          int count1 = this.m_descriptorCache[index].Count;
          int num = this.m_descriptorCache[index].Sweep();
          int count2 = this.m_descriptorCache[index].Count;
          requestContext.Trace(4440016, TraceLevel.Info, "IdentityService", nameof (IdentityCache), "Sweep Cache called, with {0} items in the cache. {1} items sweeped and remaining number of cached items are {2}", (object) count1, (object) num, (object) count2);
        }
        else
          this.m_descriptorCache[index].Sweep();
      }
    }

    public void Clear(IVssRequestContext requestContext)
    {
      using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
      {
        for (int index = 0; index <= 1; ++index)
          this.m_descriptorCache[index].Clear();
        this.m_isMemberCacheByDescriptor.Clear();
        this.m_scopeParentIdCache.Clear();
        this.m_scopeAncestorIdsCache.Clear();
        this.m_scopeCacheById.Clear();
        this.m_hasAadGroupsCache.Clear();
      }
      this.ScopeMapper?.ClearCache();
      this.IdMapper?.ClearCaches();
      requestContext.GetExtensions<IIdentityIdTranslatorExtension>(ExtensionLifetime.Service).FirstOrDefault<IIdentityIdTranslatorExtension>()?.ClearCaches(requestContext);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      QueryMembership? queryMembership = null)
    {
      if (!queryMembership.HasValue || queryMembership.Value <= QueryMembership.Direct)
      {
        if (queryMembership.HasValue)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (this.m_descriptorCache[(int) queryMembership.Value].TryGetValue(descriptor, out identity))
            return identity.Clone();
        }
        else
        {
          for (int index = 0; index <= 1; ++index)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (this.m_descriptorCache[index].TryGetValue(descriptor, out identity))
              return identity.Clone();
          }
        }
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      QueryMembership? queryMembership = null)
    {
      if (!queryMembership.HasValue || queryMembership.Value <= QueryMembership.Direct)
      {
        if (queryMembership.HasValue)
        {
          IEnumerable<IdentityDescriptor> keys;
          IdentityDescriptor key;
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (this.m_subjectDescriptorCache[(int) queryMembership.Value].TryGetKeys(subjectDescriptor, out keys) && (key = keys.SingleOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null && this.m_descriptorCache[(int) queryMembership.Value].TryGetValue(key, out identity))
            return identity.Clone();
        }
        else
        {
          for (int index = 0; index <= 1; ++index)
          {
            IEnumerable<IdentityDescriptor> keys;
            IdentityDescriptor key;
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (this.m_subjectDescriptorCache[index].TryGetKeys(subjectDescriptor, out keys) && (key = keys.SingleOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null && this.m_descriptorCache[index].TryGetValue(key, out identity))
              return identity.Clone();
          }
        }
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      SocialDescriptor socialDescriptor,
      QueryMembership? queryMembership = null)
    {
      if (!queryMembership.HasValue || queryMembership.Value <= QueryMembership.Direct)
      {
        if (queryMembership.HasValue)
        {
          IEnumerable<IdentityDescriptor> keys;
          IdentityDescriptor key;
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (this.m_socialDescriptorCache[(int) queryMembership.Value].TryGetKeys(socialDescriptor, out keys) && (key = keys.SingleOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null && this.m_descriptorCache[(int) queryMembership.Value].TryGetValue(key, out identity))
            return identity.Clone();
        }
        else
        {
          for (int index = 0; index <= 1; ++index)
          {
            IEnumerable<IdentityDescriptor> keys;
            IdentityDescriptor key;
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (this.m_socialDescriptorCache[index].TryGetKeys(socialDescriptor, out keys) && (key = keys.SingleOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null && this.m_descriptorCache[index].TryGetValue(key, out identity))
              return identity.Clone();
          }
        }
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      Guid identityId,
      QueryMembership? queryMembership = null)
    {
      if (!queryMembership.HasValue || queryMembership.Value <= QueryMembership.Direct)
      {
        if (queryMembership.HasValue)
        {
          IEnumerable<IdentityDescriptor> keys;
          IdentityDescriptor key;
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          if (this.m_idCache[(int) queryMembership.Value].TryGetKeys(identityId, out keys) && (key = keys.SingleOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null && this.m_descriptorCache[(int) queryMembership.Value].TryGetValue(key, out identity))
            return identity.Clone();
        }
        else
        {
          for (int index = 0; index <= 1; ++index)
          {
            IEnumerable<IdentityDescriptor> keys;
            IdentityDescriptor key;
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (this.m_idCache[index].TryGetKeys(identityId, out keys) && (key = keys.SingleOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null && this.m_descriptorCache[index].TryGetValue(key, out identity))
              return identity.Clone();
          }
        }
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership? queryMembership = null)
    {
      if (!queryMembership.HasValue || queryMembership.Value <= QueryMembership.Direct)
      {
        IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>[] memoryCacheGroupingArray;
        switch (searchFactor)
        {
          case IdentitySearchFilter.AccountName:
            memoryCacheGroupingArray = this.m_accountNameCache;
            break;
          case IdentitySearchFilter.Alias:
            memoryCacheGroupingArray = this.m_aliasCache;
            break;
          case IdentitySearchFilter.LocalGroupName:
            memoryCacheGroupingArray = this.m_localGroupNameCache;
            break;
          default:
            memoryCacheGroupingArray = (IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>[]) null;
            break;
        }
        if (memoryCacheGroupingArray != null)
        {
          if (queryMembership.HasValue)
          {
            IEnumerable<IdentityDescriptor> keys;
            IdentityDescriptor key;
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (memoryCacheGroupingArray[(int) queryMembership.Value].TryGetKeys(factorValue, out keys) && (key = keys.SingleOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null && this.m_descriptorCache[(int) queryMembership.Value].TryGetValue(key, out identity))
              return identity.Clone();
          }
          else
          {
            for (int index = 0; index <= 1; ++index)
            {
              IEnumerable<IdentityDescriptor> keys;
              IdentityDescriptor key;
              Microsoft.VisualStudio.Services.Identity.Identity identity;
              if (memoryCacheGroupingArray[index].TryGetKeys(factorValue, out keys) && (key = keys.SingleOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null && this.m_descriptorCache[index].TryGetValue(key, out identity))
                return identity.Clone();
            }
          }
        }
      }
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public IDictionary<IdentityDescriptor, bool> HasAadGroups(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors)
    {
      if (aadGroupDescriptors.IsNullOrEmpty<IdentityDescriptor>())
        return (IDictionary<IdentityDescriptor, bool>) new Dictionary<IdentityDescriptor, bool>();
      Dictionary<IdentityDescriptor, bool> dictionary = new Dictionary<IdentityDescriptor, bool>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (IdentityDescriptor aadGroupDescriptor in (IEnumerable<IdentityDescriptor>) aadGroupDescriptors)
      {
        bool flag = this.m_hasAadGroupsCache.TryGetValue(aadGroupDescriptor, out bool _);
        dictionary[aadGroupDescriptor] = flag;
      }
      return (IDictionary<IdentityDescriptor, bool>) dictionary;
    }

    public IDictionary<IdentityDescriptor, IdentityMembershipInfo> ReadIdentityMembershipInfo(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> descriptors)
    {
      if (descriptors.IsNullOrEmpty<IdentityDescriptor>())
        return (IDictionary<IdentityDescriptor, IdentityMembershipInfo>) new Dictionary<IdentityDescriptor, IdentityMembershipInfo>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      Dictionary<IdentityDescriptor, IdentityMembershipInfo> dedupedDictionary = descriptors.ToDedupedDictionary<IdentityDescriptor, IdentityDescriptor, IdentityMembershipInfo>((Func<IdentityDescriptor, IdentityDescriptor>) (x => x), (Func<IdentityDescriptor, IdentityMembershipInfo>) (x => (IdentityMembershipInfo) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      foreach (IdentityDescriptor descriptor in (IEnumerable<IdentityDescriptor>) descriptors)
      {
        IdentityMembershipInfo identityMembershipInfo;
        if (this.m_isMemberCacheByDescriptor.TryGetValue(descriptor, out identityMembershipInfo) && identityMembershipInfo != null)
          dedupedDictionary[descriptor] = identityMembershipInfo.Clone();
      }
      return (IDictionary<IdentityDescriptor, IdentityMembershipInfo>) dedupedDictionary;
    }

    public void AddAadGroups(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors)
    {
      if (aadGroupDescriptors.IsNullOrEmpty<IdentityDescriptor>())
        return;
      using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
      {
        foreach (IdentityDescriptor aadGroupDescriptor in (IEnumerable<IdentityDescriptor>) aadGroupDescriptors)
          this.m_hasAadGroupsCache[aadGroupDescriptor] = true;
      }
    }

    public bool? IsMember(
      IVssRequestContext requestContext,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor,
      out long cacheStamp,
      out IdentityMembershipInfo parentMemberships)
    {
      bool flag1 = requestContext.IsTracingSecurityEvaluation(80099, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache));
      bool flag2 = this.m_isMemberCacheByDescriptor.TryGetValue(memberDescriptor, out parentMemberships) && parentMemberships != null;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (flag1)
      {
        empty1 = SecurityServiceHelpers.DescriptorToString(requestContext, groupDescriptor);
        empty2 = SecurityServiceHelpers.DescriptorToString(requestContext, memberDescriptor);
        IdentityMembershipInfo identityMembershipInfo = parentMemberships;
        string str1 = EuiiUtility.MaskEmail((identityMembershipInfo != null ? identityMembershipInfo.Serialize<IdentityMembershipInfo>() : (string) null) ?? "null");
        IVssRequestContext requestContext1 = requestContext;
        string str2 = string.Format("[IdentityCache.IsMember] groupDescriptor:{0}, memberDescriptor:{1}, identityDomain:{2}, ", (object) empty1, (object) empty2, (object) this.HostDomain.DomainId);
        string str3 = string.Format("cacheHit:{0}, parentMemberships:{1}, ", (object) flag2, (object) str1);
        IdentityMembershipInfo membershipInfo = parentMemberships;
        string str4 = (membershipInfo != null ? membershipInfo.IsInvalid().ToString() : (string) null) ?? "null";
        string format = str2 + str3 + "parentMemberships.IsInvalid:" + str4;
        object[] objArray = Array.Empty<object>();
        requestContext1.TraceSecurityEvaluation(80100, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), format, objArray);
      }
      cacheStamp = parentMemberships.GetCacheStamp();
      bool flag3;
      if (requestContext.RootContext.Items.TryGetValue<bool>(RequestContextItemsKeys.IgnoreMembershipCache, out flag3) & flag3)
      {
        if (flag1)
          requestContext.TraceSecurityEvaluation(899067, TraceLevel.Info, "IdentityService", nameof (IdentityCache), string.Format("Returning null result with cachestamp: {0}, for member: {1} in group: {2}, since we are being requested to ignore membership cache.", (object) cacheStamp, (object) empty2, (object) empty1));
        return new bool?();
      }
      return flag2 && !parentMemberships.IsInvalid() ? new bool?(parentMemberships.Parents != null && parentMemberships.Parents.Contains(groupDescriptor)) : new bool?();
    }

    public bool Update(
      IVssRequestContext requestContext,
      IEnumerable<Guid> descriptorChanges,
      IEnumerable<Guid> identityChanges,
      IEnumerable<MembershipChangeInfo> membershipChanges,
      IEnumerable<Guid> scopeChangeIds)
    {
      requestContext.TraceEnter(899061, "IdentityService", nameof (IdentityCache), nameof (Update));
      requestContext.TraceConditionally(899062, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), (Func<string>) (() => string.Format("Update input parameters , m_hostDomain = {0}, descriptorChanges = {1}, identityChanges = {2}, membershipChanges = {3}, scopeUpdates = {4}", (object) this.m_hostDomain, (object) descriptorChanges.ToQuotedStringListOrNullStringLiteral<Guid>(), (object) identityChanges.ToQuotedStringListOrNullStringLiteral<Guid>(), (object) membershipChanges.ToQuotedStringListOrNullStringLiteral<MembershipChangeInfo>(), (object) scopeChangeIds.ToQuotedStringListOrNullStringLiteral<Guid>())));
      bool flag1 = false;
      if (descriptorChanges != null || identityChanges != null || membershipChanges != null || scopeChangeIds != null)
      {
        using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
        {
          if (descriptorChanges != null)
          {
            foreach (Guid descriptorChange in descriptorChanges)
            {
              flag1 |= this.Remove(requestContext, descriptorChange);
              for (int index = 0; index <= 1; ++index)
              {
                IEnumerable<IdentityDescriptor> keys;
                if (this.m_memberReferences[index].TryGetKeys(descriptorChange, out keys))
                {
                  foreach (IdentityDescriptor descriptor in keys)
                    flag1 |= this.Remove(requestContext, descriptor, new QueryMembership?(QueryMembership.Direct));
                }
              }
            }
          }
          if (identityChanges != null)
          {
            foreach (Guid identityChange in identityChanges)
              flag1 |= this.Remove(requestContext, identityChange);
          }
          if (membershipChanges != null)
          {
            bool flag2 = IdentityCacheHelper.IsSkipMembershipChangeEnabled(requestContext);
            StringBuilder stringBuilder = (StringBuilder) null;
            foreach (MembershipChangeInfo membershipChange1 in membershipChanges)
            {
              MembershipChangeInfo membershipChange = membershipChange1;
              requestContext.Trace(899063, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "Processing membership change, on m_hostDomain = {0}, and membershipChange = {1}", (object) this.m_hostDomain, (object) membershipChange);
              if (this.IsChangeApplicable(requestContext, membershipChange))
              {
                if (membershipChange.ContainerDescriptor != (IdentityDescriptor) null)
                  flag1 |= this.Remove(requestContext, membershipChange.ContainerDescriptor, new QueryMembership?(QueryMembership.Direct));
                else
                  flag1 |= this.Remove(requestContext, membershipChange.ContainerId, new QueryMembership?(QueryMembership.Direct));
                if (membershipChange.MemberId != Guid.Empty)
                  flag1 |= this.Remove(requestContext, membershipChange.MemberId);
                else
                  flag1 |= this.Remove(requestContext, membershipChange.MemberDescriptor);
                if (!membershipChange.IsMemberGroup && membershipChange.MemberDescriptor != (IdentityDescriptor) null)
                {
                  flag1 |= this.RemoveParents(requestContext, membershipChange.MemberDescriptor, membershipChange.MemberId);
                  if (membershipChange.InvalidateStrongly)
                    flag1 |= this.InvalidateIsMemberCacheByDescriptor(requestContext, membershipChange.MemberDescriptor, membershipChange.InvalidateStrongly);
                }
                if (membershipChange.IsMemberGroup)
                {
                  int num = !flag2 ? 0 : (IdentityCacheHelper.ShouldSkipMembershipChange(requestContext, membershipChange) ? 1 : 0);
                  if (num != 0)
                  {
                    if (stringBuilder == null)
                      stringBuilder = new StringBuilder(string.Format("Skipping membership change: {0}.", (object) membershipChange));
                    else
                      stringBuilder.AppendLine(string.Format("Skipping membership change: {0}.", (object) membershipChange));
                  }
                  IEnumerable<IdentityDescriptor> identityDescriptors;
                  bool keysExist = this.m_isMemberCacheByParent.TryGetKeys(membershipChange.MemberDescriptor, out identityDescriptors);
                  requestContext.TraceConditionally(899064, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), (Func<string>) (() => "membershipChange.MemberDescriptor: " + SecurityServiceHelpers.DescriptorToString(requestContext, membershipChange.MemberDescriptor) + ", matchingIdentityDescriptorsCount:" + (keysExist ? identityDescriptors.Count<IdentityDescriptor>().ToString() : "0")));
                  if (num == 0 & keysExist)
                  {
                    foreach (IdentityDescriptor descriptor in identityDescriptors)
                      flag1 |= this.InvalidateIsMemberCacheByDescriptor(requestContext, descriptor);
                  }
                }
                if (this.IsAadGroupsCacheWarm && membershipChange.IsMemberGroup && AadIdentityHelper.IsAadGroup(membershipChange.MemberDescriptor))
                {
                  requestContext.Trace(899065, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "For m_hostDomain = {0}, Adding AAD Group= {1}", (object) this.m_hostDomain, (object) membershipChange.MemberId);
                  this.AddAadGroups(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
                  {
                    membershipChange.MemberDescriptor
                  });
                }
              }
            }
            if (stringBuilder != null)
              requestContext.TraceAlways(899081, TraceLevel.Info, "IdentityService", nameof (IdentityCache), stringBuilder.ToString());
          }
          if (scopeChangeIds != null)
          {
            foreach (Guid scopeChangeId in scopeChangeIds)
              flag1 |= this.RemoveScope(requestContext, scopeChangeId);
          }
        }
      }
      requestContext.TraceLeave(899069, "IdentityService", nameof (IdentityCache), nameof (Update));
      return flag1;
    }

    private bool RemoveScope(IVssRequestContext requestContext, Guid scopeId)
    {
      requestContext.TraceConditionally(899066, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), (Func<string>) (() => string.Format("For m_hostDomian = {0}, scope invalidation is called for scopeId = {1}", (object) this.m_hostDomain, (object) scopeId)));
      return this.m_scopeCacheById.Remove(scopeId);
    }

    public bool Update(
      IVssRequestContext requestContext,
      IEnumerable<IdentityDescriptor> identityChanges)
    {
      bool updated = false;
      requestContext.TraceEnter(899071, "IdentityService", nameof (IdentityCache), nameof (Update));
      requestContext.TraceConditionally(899072, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), (Func<string>) (() =>
      {
        IEnumerable<IdentityDescriptor> source = identityChanges;
        return string.Format("Update input parameters , m_hostDomain = {0}, identityChanges = {1}", (object) this.m_hostDomain, source != null ? (object) source.Select<IdentityDescriptor, string>((Func<IdentityDescriptor, string>) (descriptor => SecurityServiceHelpers.DescriptorToString(requestContext, descriptor))).ToQuotedStringList<string>() : (object) (string) null);
      }));
      if (identityChanges != null)
      {
        using (this.AcquireWriterLock(requestContext, this.m_cacheLockName))
        {
          foreach (IdentityDescriptor identityChange in identityChanges)
          {
            IdentityDescriptor descriptor = identityChange;
            updated |= this.Remove(requestContext, descriptor);
            requestContext.TraceConditionally(899073, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), (Func<string>) (() => string.Format("descriptor = {0}, removed = {1}", (object) SecurityServiceHelpers.DescriptorToString(requestContext, descriptor), (object) updated)));
          }
        }
      }
      requestContext.TraceLeave(899079, "IdentityService", nameof (IdentityCache), nameof (Update));
      return updated;
    }

    internal IEnumerable<int> GetCacheCounts(IVssRequestContext requestContext)
    {
      List<int> cacheCounts = new List<int>();
      cacheCounts.AddRange(((IEnumerable<VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>) this.m_descriptorCache).Select<VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, int>((Func<VssMemoryCacheList<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, int>) (cache => cache.Count)));
      cacheCounts.AddRange(((IEnumerable<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>>) this.m_idCache).Select<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>, int>((Func<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, Guid>, int>) (cache => cache.Count)));
      cacheCounts.AddRange(((IEnumerable<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>>) this.m_subjectDescriptorCache).Select<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>, int>((Func<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>, int>) (cache => cache.Count)));
      cacheCounts.AddRange(((IEnumerable<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SocialDescriptor>>) this.m_socialDescriptorCache).Select<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SocialDescriptor>, int>((Func<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, SocialDescriptor>, int>) (cache => cache.Count)));
      cacheCounts.AddRange(((IEnumerable<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>>) this.m_accountNameCache).Select<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>, int>((Func<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>, int>) (cache => cache.Count)));
      cacheCounts.AddRange(((IEnumerable<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>>) this.m_localGroupNameCache).Select<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>, int>((Func<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>, int>) (cache => cache.Count)));
      cacheCounts.AddRange(((IEnumerable<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>>) this.m_aliasCache).Select<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>, int>((Func<IVssMemoryCacheGrouping<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity, string>, int>) (cache => cache.Count)));
      cacheCounts.Add(this.m_isMemberCacheByDescriptor.Count);
      cacheCounts.Add(this.m_isMemberCacheById.Count);
      cacheCounts.Add(this.m_isMemberCacheByParent.Count);
      cacheCounts.Add(this.m_scopeParentIdCache.Count);
      cacheCounts.Add(this.m_scopeAncestorIdsCache.Count);
      cacheCounts.Add(this.m_scopeCacheById.Count);
      return (IEnumerable<int>) cacheCounts;
    }

    private bool InvalidateIsMemberCacheByDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool invalidateStrongly = false)
    {
      return requestContext.ExecutionEnvironment.IsHostedDeployment && !this.m_shouldAddInvalidatedIdentityMembershipInfoEvenWhenKeyIsMissing && requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS ? this.InvalidateIsMemberCacheByDescriptorOnlyIfMatchingKeyExistsAndIsValid(requestContext, descriptor, invalidateStrongly) : this.InvalidateIsMemberCacheByDescriptorByAddingNewInvalidatedMembershipInfo(requestContext, descriptor, invalidateStrongly);
    }

    private bool InvalidateIsMemberCacheByDescriptorByAddingNewInvalidatedMembershipInfo(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool invalidateStrongly)
    {
      requestContext.TraceEnter(899091, "IdentityService", nameof (IdentityCache), nameof (InvalidateIsMemberCacheByDescriptorByAddingNewInvalidatedMembershipInfo));
      IdentityMembershipInfo membershipInfo;
      bool flag = this.m_isMemberCacheByDescriptor.TryGetValue(descriptor, out membershipInfo) && !membershipInfo.IsInvalid();
      requestContext.Trace(899092, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "InvalidateIsMemberCacheByDescriptorByAddingNewInvalidatedMembershipInfo input parameters , m_hostDomain = {0} , descriptor.IdentityType = {1} , output parameters parentMemberships = {2} , hasValidValue = {3}", (object) this.m_hostDomain, (object) descriptor?.IdentityType, (object) membershipInfo, (object) flag);
      this.m_isMemberCacheByDescriptor[descriptor] = IdentityMembershipInfoUtils.CreateInvalidatedMembershipInfo(invalidateStrongly);
      requestContext.TraceLeave(899099, "IdentityService", nameof (IdentityCache), nameof (InvalidateIsMemberCacheByDescriptorByAddingNewInvalidatedMembershipInfo));
      return flag;
    }

    private bool InvalidateIsMemberCacheByDescriptorOnlyIfMatchingKeyExistsAndIsValid(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool invalidateStrongly = false)
    {
      requestContext.TraceEnter(899101, "IdentityService", nameof (IdentityCache), nameof (InvalidateIsMemberCacheByDescriptorOnlyIfMatchingKeyExistsAndIsValid));
      IdentityMembershipInfo membershipInfo;
      bool flag = this.m_isMemberCacheByDescriptor.TryGetValue(descriptor, out membershipInfo) && !membershipInfo.IsInvalid();
      requestContext.Trace(899102, TraceLevel.Verbose, "IdentityService", nameof (IdentityCache), "InvalidateIsMemberCacheByDescriptorOnlyIfMatchingKeyExistsAndIsValid input parameters , m_hostDomain = {0} , descriptor.IdentityType = {1} , output parameters parentMemberships = {2} , hasValidValue = {3}", (object) this.m_hostDomain, (object) descriptor?.IdentityType, (object) membershipInfo, (object) flag);
      if (invalidateStrongly)
        this.m_isMemberCacheByDescriptor[descriptor] = IdentityMembershipInfo.StronglyInvalidatedIdentityMembershipInfo;
      else if (flag)
        this.m_isMemberCacheByDescriptor[descriptor] = IdentityMembershipInfo.InvalidatedIdentityMembershipInfo;
      requestContext.TraceLeave(899103, "IdentityService", nameof (IdentityCache), nameof (InvalidateIsMemberCacheByDescriptorOnlyIfMatchingKeyExistsAndIsValid));
      return flag;
    }

    private bool IsChangeApplicable(
      IVssRequestContext context,
      MembershipChangeInfo membershipChange)
    {
      if (!context.ExecutionEnvironment.IsHostedDeployment || membershipChange.ContainerScopeId == Guid.Empty || membershipChange.ContainerAncestorScopeIds == null || membershipChange.ContainerScopeId == this.m_hostDomain.DomainId)
        return true;
      if (this.m_hostDomain.HostLevel == TeamFoundationHostType.Application && membershipChange.IsMemberGroup && context.IsFeatureEnabled("VisualStudio.Services.IdentityCache.DontApplyNonOrgLevelMembershipChangesToOrgScopeCache"))
        return false;
      if (membershipChange.ContainerAncestorScopeIds.Contains(this.m_hostDomain.DomainId))
        return true;
      if (this.m_hostDomain.HostLevel == TeamFoundationHostType.ProjectCollection && this.m_hostDomain.Parent != null)
      {
        IdentityDomain parent = this.m_hostDomain.Parent;
        if (membershipChange.ContainerScopeId == parent.DomainId)
          return true;
      }
      return false;
    }

    private static VssCacheExpiryProvider<IdentityDescriptor, IdentityMembershipInfo> CreateCacheExpiryProvider(
      long cacheTTLInTicks)
    {
      Capture<TimeSpan> capture = Capture.Create<TimeSpan>(TimeSpan.FromTicks(cacheTTLInTicks));
      return new VssCacheExpiryProvider<IdentityDescriptor, IdentityMembershipInfo>(capture, capture);
    }

    private IDisposable AcquireWriterLock(IVssRequestContext requestContext, ILockName lockName) => (IDisposable) requestContext.AcquireWriterLock(lockName);

    internal DateTime LastWarmedTime { get; set; }

    internal bool IsWarm { get; set; }

    internal DateTime LastWarmedTimeForAadGroupsCache { get; set; }

    internal bool IsAadGroupsCacheWarm { get; set; }

    internal IScopeMapper ScopeMapper { get; }

    internal IIdMapper IdMapper { get; }

    internal IdentityDomain HostDomain => this.m_hostDomain;

    public event IdentityEvictedHandler IdentityEvicted;

    protected virtual void OnIdentityEvicted(IdentityRemovedEventArgs args)
    {
      IdentityEvictedHandler identityEvicted = this.IdentityEvicted;
      if (identityEvicted == null)
        return;
      identityEvicted((object) this, args);
    }
  }
}
