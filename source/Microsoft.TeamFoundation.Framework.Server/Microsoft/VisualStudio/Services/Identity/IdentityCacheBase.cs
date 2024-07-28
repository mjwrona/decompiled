// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityCacheBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityCacheBase : IIdentityCache
  {
    protected ConcurrentDictionary<Guid, IdentityCache> m_identityCaches;
    protected int m_cacheSize;
    protected IIdentityCacheWarmer m_cacheWarmer;
    protected IdentityDomain m_rootDomain;
    protected SequenceContextCache m_sequenceContextCache;
    internal static TimeSpan m_sequenceContextCacheTTL = TimeSpan.FromHours(24.0);
    internal Func<DateTimeOffset> m_sequenceContextCreationTime = (Func<DateTimeOffset>) (() => DateTimeOffset.UtcNow);
    private object m_sequenceContextCacheLock = new object();
    private const string s_area = "IdentityService";
    private const string s_layer = "IdentityCacheBase";

    public IdentityCacheBase(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      int cacheSize,
      IIdentityCacheWarmer cacheWarmer = null)
    {
      this.m_cacheSize = cacheSize;
      this.m_identityCaches = new ConcurrentDictionary<Guid, IdentityCache>();
      this.m_cacheWarmer = cacheWarmer;
      this.m_rootDomain = hostDomain.IsMaster ? hostDomain : hostDomain.Parent;
      this.AddDomain(requestContext, hostDomain);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor descriptor,
      QueryMembership queryMembership)
    {
      try
      {
        requestContext.TraceEnter(4440011, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
        IdentityCache identityCache = this.GetIdentityCache(hostDomain);
        this.WarmCacheIfNecessary(requestContext, identityCache, hostDomain);
        Microsoft.VisualStudio.Services.Identity.Identity result = identityCache.ReadIdentity(requestContext, descriptor, new QueryMembership?(queryMembership));
        if (requestContext.IsTracing(80992, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)))
        {
          string callStack = requestContext.IsTracing(80993, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)) ? EnvironmentWrapper.ToReadableStackTrace().ToString() : string.Empty;
          IdentityTracing.TraceIdentityCacheRead(IdentityTracing.TargetStoreType.L1Cache, hostDomain, descriptor?.Identifier, queryMembership, result == null ? IdentityTracing.CacheResult.Miss : IdentityTracing.CacheResult.Hit, callStack);
        }
        ImsCacheUtils.Trace(requestContext, 4440015, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: ReadIdentityByDescriptor[Domain: {0}, Descriptor: {1}, QueryMembership: {2}]\nResponse: [Identity: {3}]", (object) hostDomain, (object) EuiiUtility.MaskEmail(descriptor.ToString()), (object) queryMembership, (object) result?.GetProperty<Guid>("CUID", result.Id))));
        return IdentityCacheHelper.IsValidCacheData(requestContext, result, 1754844) ? result : (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        requestContext.TraceLeave(4440019, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SubjectDescriptor subjectDescriptor,
      QueryMembership queryMembership)
    {
      if (subjectDescriptor == new SubjectDescriptor())
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      try
      {
        requestContext.TraceEnter(119226, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
        IdentityCache identityCache = this.GetIdentityCache(hostDomain);
        this.WarmCacheIfNecessary(requestContext, identityCache, hostDomain);
        Microsoft.VisualStudio.Services.Identity.Identity result = identityCache.ReadIdentity(requestContext, subjectDescriptor, new QueryMembership?(queryMembership));
        if (requestContext.IsTracing(436397, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)))
        {
          string callStack = requestContext.IsTracing(194144, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)) ? EnvironmentWrapper.ToReadableStackTrace().ToString() : string.Empty;
          IdentityTracing.TraceIdentityCacheRead(IdentityTracing.TargetStoreType.L1Cache, hostDomain, subjectDescriptor.Identifier, queryMembership, result == null ? IdentityTracing.CacheResult.Miss : IdentityTracing.CacheResult.Hit, callStack);
        }
        ImsCacheUtils.Trace(requestContext, 83232, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: ReadIdentityBySubjectDescriptor[Domain: {0}, Descriptor: {1}, QueryMembership: {2}]\nResponse: [Identity: {3}]", (object) hostDomain, (object) subjectDescriptor, (object) queryMembership, (object) result?.GetProperty<Guid>("CUID", result.Id))));
        return IdentityCacheHelper.IsValidCacheData(requestContext, result, 1754844) ? result : (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        requestContext.TraceLeave(180689, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SocialDescriptor socialDescriptor,
      QueryMembership queryMembership)
    {
      if (socialDescriptor == new SocialDescriptor())
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      try
      {
        requestContext.TraceEnter(119226, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
        IdentityCache identityCache = this.GetIdentityCache(hostDomain);
        this.WarmCacheIfNecessary(requestContext, identityCache, hostDomain);
        Microsoft.VisualStudio.Services.Identity.Identity result = identityCache.ReadIdentity(requestContext, socialDescriptor, new QueryMembership?(QueryMembership.None));
        if (requestContext.IsTracing(436397, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)))
        {
          string callStack = requestContext.IsTracing(194144, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)) ? EnvironmentWrapper.ToReadableStackTrace().ToString() : string.Empty;
          IdentityTracing.TraceIdentityCacheRead(IdentityTracing.TargetStoreType.L1Cache, hostDomain, socialDescriptor.Identifier, QueryMembership.None, result == null ? IdentityTracing.CacheResult.Miss : IdentityTracing.CacheResult.Hit, callStack);
        }
        ImsCacheUtils.Trace(requestContext, 83232, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: ReadIdentityBySubjectDescriptor[Domain: {0}, Descriptor: {1}, QueryMembership: {2}]\nResponse: [Identity: {3}]", (object) hostDomain, (object) socialDescriptor, (object) queryMembership, (object) result?.GetProperty<Guid>("CUID", result.Id))));
        return IdentityCacheHelper.IsValidCacheData(requestContext, result, 1754844) ? result : (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        requestContext.TraceLeave(180689, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid identityId,
      QueryMembership queryMembership)
    {
      try
      {
        requestContext.TraceEnter(4440021, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
        IdentityCache identityCache = this.GetIdentityCache(hostDomain);
        this.WarmCacheIfNecessary(requestContext, identityCache, hostDomain);
        Microsoft.VisualStudio.Services.Identity.Identity result = identityCache.ReadIdentity(requestContext, identityId, new QueryMembership?(queryMembership));
        if (requestContext.IsTracing(80992, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)))
        {
          string callStack = requestContext.IsTracing(80993, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)) ? EnvironmentWrapper.ToReadableStackTrace().ToString() : string.Empty;
          IdentityTracing.TraceIdentityCacheRead(IdentityTracing.TargetStoreType.L1Cache, hostDomain, identityId, queryMembership, result == null ? IdentityTracing.CacheResult.Miss : IdentityTracing.CacheResult.Hit, callStack);
        }
        ImsCacheUtils.Trace(requestContext, 4440025, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: ReadIdentityById[Domain: {0}, IdentityId: {1}, QueryMembership: {2}]\nResponse: [Identity CUID: {3}]", (object) hostDomain, (object) identityId, (object) queryMembership, (object) result?.GetProperty<Guid>("CUID", result.Id))));
        return IdentityCacheHelper.IsValidCacheData(requestContext, result, 1754845) ? result : (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        requestContext.TraceLeave(4440029, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership)
    {
      try
      {
        requestContext.TraceEnter(4440031, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
        IdentityCache identityCache = this.GetIdentityCache(hostDomain);
        this.WarmCacheIfNecessary(requestContext, identityCache, hostDomain);
        Microsoft.VisualStudio.Services.Identity.Identity result = identityCache.ReadIdentity(requestContext, searchFactor, factorValue, new QueryMembership?(queryMembership));
        if (requestContext.IsTracing(80992, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)))
        {
          string callStack = requestContext.IsTracing(80993, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)) ? EnvironmentWrapper.ToReadableStackTrace().ToString() : string.Empty;
          IdentityTracing.TraceIdentityCacheRead(IdentityTracing.TargetStoreType.L1Cache, hostDomain, searchFactor, factorValue, queryMembership, result == null ? IdentityTracing.CacheResult.Miss : IdentityTracing.CacheResult.Hit, callStack);
        }
        ImsCacheUtils.Trace(requestContext, 4440035, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: ReadIdentityBySearch[Domain: {0}, SearchValue: {1}, QueryMembership: {2}]\nResponse: [Identity CUID: {3}]", (object) hostDomain, (object) searchFactor, (object) queryMembership, (object) result?.GetProperty<Guid>("CUID", result.Id))));
        return IdentityCacheHelper.IsValidCacheData(requestContext, result, 1754846) ? result : (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        requestContext.TraceLeave(4440039, "IdentityService", nameof (IdentityCacheBase), nameof (ReadIdentity));
      }
    }

    public void UpdateIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        requestContext.TraceEnter(4440041, "IdentityService", nameof (IdentityCacheBase), nameof (UpdateIdentity));
        if (!IdentityCacheHelper.IsValidCacheData(requestContext, identity, 1754847))
          return;
        this.GetIdentityCache(hostDomain).Add(requestContext, identity, queryMembership);
        if (requestContext.IsTracing(80992, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)))
        {
          string callStack = requestContext.IsTracing(80993, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)) ? EnvironmentWrapper.ToReadableStackTrace().ToString() : string.Empty;
          IdentityTracing.TraceIdentityCacheUpdate(IdentityTracing.TargetStoreType.L1Cache, hostDomain, identity.Id, queryMembership, callStack);
        }
        ImsCacheUtils.Trace(requestContext, 4440045, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: UpdateIdentity[Domain: {0}, QueryMembership: {1}, Identity CUID: {2}]", (object) hostDomain, (object) queryMembership, (object) identity?.GetProperty<Guid>("CUID", identity.Id))));
      }
      finally
      {
        requestContext.TraceLeave(4440049, "IdentityService", nameof (IdentityCacheBase), nameof (UpdateIdentity));
      }
    }

    public bool? IsMember(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor,
      out long cacheStamp,
      out IdentityMembershipInfo membershipInfo)
    {
      return this.GetIdentityCache(hostDomain).IsMember(requestContext, groupDescriptor, memberDescriptor, out cacheStamp, out membershipInfo);
    }

    public void UpdateParentMemberships(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        requestContext.TraceEnter(4440061, "IdentityService", nameof (IdentityCacheBase), nameof (UpdateParentMemberships));
        this.GetIdentityCache(hostDomain).SetParentMemberships(requestContext, identity);
        ImsCacheUtils.Trace(requestContext, 4440065, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() =>
        {
          IdentityDomain identityDomain = hostDomain;
          // ISSUE: variable of a boxed type
          __Boxed<Guid?> property = (ValueType) identity?.GetProperty<Guid>("CUID", identity.Id);
          Microsoft.VisualStudio.Services.Identity.Identity identity1 = identity;
          string str = identity1 != null ? identity1.MemberOf.Serialize<ICollection<IdentityDescriptor>>() : (string) null;
          return string.Format("Request: UpdateParentMemberships[Domain: {0}, Identity CUID: {1}, Parents: {2}]", (object) identityDomain, (object) property, (object) str);
        }));
      }
      finally
      {
        requestContext.TraceLeave(4440069, "IdentityService", nameof (IdentityCacheBase), nameof (UpdateParentMemberships));
      }
    }

    public bool CompareAndSwapParentMemberships(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      long cacheStamp)
    {
      try
      {
        requestContext.TraceEnter(4440091, "IdentityService", nameof (IdentityCacheBase), nameof (CompareAndSwapParentMemberships));
        int num = this.GetIdentityCache(hostDomain).CompareAndSwapParentMemberships(requestContext, identity, cacheStamp) ? 1 : 0;
        ImsCacheUtils.Trace(requestContext, 4440095, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: CompareAndSwapParentMemberships[Domain: {0}, Identity CUID: {1}, Parents: {2}]", (object) hostDomain, (object) identity?.GetProperty<Guid>("CUID", identity.Id), (object) identity?.MemberOf)));
        return num != 0;
      }
      finally
      {
        requestContext.TraceLeave(4440099, "IdentityService", nameof (IdentityCacheBase), nameof (CompareAndSwapParentMemberships));
      }
    }

    public Guid GetOrAddScopeParent(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      Guid parentScopeId)
    {
      try
      {
        requestContext.TraceEnter(4440071, "IdentityService", nameof (IdentityCacheBase), nameof (GetOrAddScopeParent));
        Guid result = this.GetIdentityCache(hostDomain).GetOrAddScopeParent(scopeId, parentScopeId);
        ImsCacheUtils.Trace(requestContext, 4440075, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: GetOrAddScopeParent[Domain: {0}, ScopeId: {1}, ParentScopeId: {2}]\nResponse: [result: {3}]", (object) hostDomain, (object) scopeId, (object) parentScopeId, (object) result)));
        return result;
      }
      finally
      {
        requestContext.TraceLeave(4440079, "IdentityService", nameof (IdentityCacheBase), nameof (GetOrAddScopeParent));
      }
    }

    public bool TryReadScopeParent(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      out Guid parentScopeId)
    {
      try
      {
        requestContext.TraceEnter(4440081, "IdentityService", nameof (IdentityCacheBase), nameof (TryReadScopeParent));
        bool result = this.GetIdentityCache(hostDomain).TryReadScopeParent(scopeId, out parentScopeId);
        Guid parentScopeIdForTracing = parentScopeId;
        ImsCacheUtils.Trace(requestContext, 4440085, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: TryReadScopeParent[Domain: {0}, ScopeId: {1}]\nResponse: [Result: {2}, ParentScopeId: {3}]", (object) hostDomain, (object) scopeId, (object) result, (object) parentScopeIdForTracing)));
        return result;
      }
      finally
      {
        requestContext.TraceLeave(4440089, "IdentityService", nameof (IdentityCacheBase), nameof (TryReadScopeParent));
      }
    }

    public bool TryGetScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      out IdentityScope scope)
    {
      try
      {
        requestContext.TraceEnter(4440131, "IdentityService", nameof (IdentityCacheBase), nameof (TryGetScope));
        scope = (IdentityScope) null;
        IdentityCache identityCache = this.GetIdentityCache(hostDomain);
        if (identityCache == null)
          return false;
        if (identityCache.TryGetScope(scopeId, out scope))
        {
          if (scope == null)
            return false;
          IdentityScope result = scope;
          ImsCacheUtils.Trace(requestContext, 4440135, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("(Cache Hit) Request: GetScopeById[scopeId: {0}]\nResponse: [Results: {1}]", (object) scopeId, (object) result.Serialize<IdentityScope>())));
          return true;
        }
        ImsCacheUtils.Trace(requestContext, 4440136, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("(Cache Miss) Request: GetScopeById[scopeId: {0}]\nResponse: null", (object) scopeId)));
        return false;
      }
      finally
      {
        requestContext.TraceLeave(4440139, "IdentityService", nameof (IdentityCacheBase), nameof (TryGetScope));
      }
    }

    public bool AddScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      IdentityScope scope)
    {
      try
      {
        requestContext.TraceEnter(4440141, "IdentityService", nameof (IdentityCacheBase), nameof (AddScope));
        IdentityCache identityCache = this.GetIdentityCache(hostDomain);
        if (identityCache == null)
          return false;
        bool result = identityCache.AddScope(scopeId, scope);
        ImsCacheUtils.Trace(requestContext, 4440145, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), 4, (Func<string>) (() => string.Format("Request: UpdateScopeByIdCache[Domain: {0}, ScopeId: {1}]\nResponse: [result: {2}]", (object) hostDomain, (object) scopeId, (object) result)));
        return result;
      }
      finally
      {
        requestContext.TraceLeave(4440149, "IdentityService", nameof (IdentityCacheBase), nameof (AddScope));
      }
    }

    public bool ProcessChanges(
      IVssRequestContext requestContext,
      ICollection<Guid> descriptorChangeIds,
      ICollection<Guid> identityChangeIds,
      ICollection<Guid> groupChangeIds,
      ICollection<MembershipChangeInfo> membershipChanges,
      ICollection<Guid> groupScopeChangeIds,
      SequenceContext sequenceContext)
    {
      requestContext.TraceEnter(899101, "IdentityService", nameof (IdentityCacheBase), nameof (ProcessChanges));
      requestContext.TraceConditionally(899102, TraceLevel.Verbose, "IdentityService", nameof (IdentityCacheBase), (Func<string>) (() => string.Format("Process Changes input parameters , descriptorChangeIds : {0} ,  identityChangeIds : {1}, groupChangeIds : {2} , membershipChanges : {3}, scopeUpdateIds : {4}", (object) descriptorChangeIds.ToQuotedStringListOrNullStringLiteral<Guid>(), (object) identityChangeIds.ToQuotedStringListOrNullStringLiteral<Guid>(), (object) groupChangeIds.ToQuotedStringListOrNullStringLiteral<Guid>(), (object) membershipChanges.ToQuotedStringListOrNullStringLiteral<MembershipChangeInfo>(), (object) groupChangeIds.ToQuotedStringListOrNullStringLiteral<Guid>())));
      string callStack = requestContext.IsTracing(80993, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)) ? EnvironmentWrapper.ToReadableStackTrace().ToString() : string.Empty;
      IEnumerable<Guid> guids = IdentityCacheBase.Concat((IEnumerable<Guid>) identityChangeIds, (IEnumerable<Guid>) groupChangeIds);
      bool flag = false;
      foreach (IdentityCache identityCache in this.m_identityCaches.Select<KeyValuePair<Guid, IdentityCache>, IdentityCache>((Func<KeyValuePair<Guid, IdentityCache>, IdentityCache>) (x => x.Value)).ToArray<IdentityCache>())
      {
        flag = identityCache.Update(requestContext, (IEnumerable<Guid>) descriptorChangeIds, guids, (IEnumerable<MembershipChangeInfo>) membershipChanges, (IEnumerable<Guid>) groupScopeChangeIds);
        if (flag)
          this.IncrementCacheInvalidationPerfCounters();
        if (requestContext.IsTracing(80992, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)))
        {
          if (descriptorChangeIds != null)
            IdentityTracing.TraceIdentityCacheInvalidationByDescriptorChange(IdentityTracing.TargetStoreType.L1Cache, identityCache.HostDomain, (IEnumerable<Guid>) descriptorChangeIds, flag ? IdentityTracing.CacheResult.Invalidated : IdentityTracing.CacheResult.NotApplicable, callStack);
          if (guids != null)
            IdentityTracing.TraceIdentityCacheInvalidationByIdentityChange(IdentityTracing.TargetStoreType.L1Cache, identityCache.HostDomain, guids, flag ? IdentityTracing.CacheResult.Invalidated : IdentityTracing.CacheResult.NotApplicable, callStack);
        }
      }
      if (sequenceContext != null && !requestContext.IsFeatureEnabled("VisualStudio.Services.IdentityCacheBase.CompareAndSwapSequenceContextIfGreater.Disable"))
        this.CompareAndSwapSequenceContextIfGreater(sequenceContext);
      requestContext.TraceLeave(899109, "IdentityService", nameof (IdentityCacheBase), nameof (ProcessChanges));
      return flag;
    }

    public void Clear(IVssRequestContext requestContext, IdentityDomain hostDomain)
    {
      IdentityCache identityCache;
      if (!this.m_identityCaches.TryGetValue(hostDomain.DomainId, out identityCache))
        return;
      identityCache.Clear(requestContext);
      if (!requestContext.IsTracing(80988, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)))
        return;
      string callStack = requestContext.IsTracing(80989, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase)) ? EnvironmentWrapper.ToReadableStackTrace().ToString() : string.Empty;
      IdentityTracing.TraceIdentityCacheClear(IdentityTracing.TargetStoreType.L1Cache, identityCache.HostDomain, IdentityTracing.IdentityTraceEventSize.Single, callStack);
    }

    public void Sweep(IVssRequestContext requestContext)
    {
      IdentityCache identityCache;
      if (!this.m_identityCaches.TryGetValue(requestContext.To(TeamFoundationHostType.Deployment).ServiceHost.InstanceId, out identityCache))
        return;
      identityCache.Sweep(requestContext);
    }

    public void Clear(IVssRequestContext requestContext)
    {
      bool flag1 = requestContext.IsTracing(80997, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase));
      bool flag2 = requestContext.IsTracing(80998, TraceLevel.Info, "IdentityService", nameof (IdentityCacheBase));
      if (flag1 & flag2)
        IdentityTracing.TraceIdentityCacheClear(IdentityTracing.TargetStoreType.L1Cache, (IdentityDomain) null, IdentityTracing.IdentityTraceEventSize.Bulk, EnvironmentWrapper.ToReadableStackTrace().ToString());
      foreach (IdentityCache identityCache in this.m_identityCaches.Select<KeyValuePair<Guid, IdentityCache>, IdentityCache>((Func<KeyValuePair<Guid, IdentityCache>, IdentityCache>) (x => x.Value)).ToArray<IdentityCache>())
      {
        identityCache.Clear(requestContext);
        if (flag1)
          IdentityTracing.TraceIdentityCacheClear(IdentityTracing.TargetStoreType.L1Cache, identityCache.HostDomain, IdentityTracing.IdentityTraceEventSize.Bulk);
        this.IncrementCacheInvalidationPerfCounters();
      }
    }

    public virtual void Unload(IVssRequestContext requestContext)
    {
      foreach (IdentityCache identityCache in this.m_identityCaches.Values.ToArray<IdentityCache>())
        identityCache.Clear(requestContext);
    }

    public virtual void AddDomain(IVssRequestContext requestContext, IdentityDomain hostDomain)
    {
      if (this.m_identityCaches.TryGetValue(hostDomain.DomainId, out IdentityCache _))
        return;
      IdentityCache identityCache = new IdentityCache(requestContext, hostDomain, this.m_cacheSize);
      this.m_identityCaches.TryAdd(hostDomain.DomainId, identityCache);
    }

    protected IdentityCache GetIdentityCache(IdentityDomain hostDomain)
    {
      IdentityCache identityCache;
      this.m_identityCaches.TryGetValue(hostDomain.DomainId, out identityCache);
      return identityCache;
    }

    public bool TryGetSequenceContext(out SequenceContext sequenceContext)
    {
      SequenceContextCache sequenceContextCache = this.m_sequenceContextCache;
      sequenceContext = (SequenceContext) null;
      if (this.IsNullOrExpired(sequenceContextCache))
        return false;
      sequenceContext = sequenceContextCache.SequenceContext;
      return true;
    }

    public SequenceContext CompareAndSwapSequenceContextIfGreater(SequenceContext sequenceContext)
    {
      if (sequenceContext.IsUnspecified)
        return this.m_sequenceContextCache?.SequenceContext;
      SequenceContextCache cache = this.m_sequenceContextCache;
      if (!this.IsNullOrExpired(cache))
        return cache.CompareAndSwapSequenceContextIfGreater(sequenceContext);
      lock (this.m_sequenceContextCacheLock)
      {
        cache = this.m_sequenceContextCache;
        if (this.IsNullOrExpired(cache))
        {
          cache = new SequenceContextCache(sequenceContext, this.m_sequenceContextCreationTime());
          this.m_sequenceContextCache = cache;
        }
      }
      return cache.CompareAndSwapSequenceContextIfGreater(sequenceContext);
    }

    private bool IsNullOrExpired(SequenceContextCache cache) => cache == null || cache.IsNullOrExpired(IdentityCacheBase.m_sequenceContextCacheTTL);

    internal void WarmCacheIfNecessary(
      IVssRequestContext requestContext,
      IdentityCache cache,
      IdentityDomain domain)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.m_cacheWarmer?.WarmCache(requestContext, cache, domain);
    }

    private static IEnumerable<Guid> Concat(IEnumerable<Guid> left, IEnumerable<Guid> right) => left == null || right == null ? (right != null ? right : left) : left.Concat<Guid>(right);

    protected void IncrementCacheInvalidationPerfCounters()
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_invalidations").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.Perf_Ims_invalidations_persec").Increment();
    }

    public void InvalidateSequenceContext(IVssRequestContext requestContext) => this.m_sequenceContextCache = (SequenceContextCache) null;
  }
}
