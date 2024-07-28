// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformIdentityCache
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class PlatformIdentityCache : IdentityCacheBase, IPlatformIdentityCache, IIdentityCache
  {
    private const string s_area = "IdentityService";
    private const string s_layer = "PlatformIdentityCache";

    public PlatformIdentityCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      int cacheSize,
      IIdentityCacheWarmer cacheWarmer = null)
      : base(requestContext, hostDomain, cacheSize, cacheWarmer)
    {
    }

    public override void AddDomain(IVssRequestContext requestContext, IdentityDomain hostDomain)
    {
      if (this.m_identityCaches.TryGetValue(hostDomain.DomainId, out IdentityCache _))
        return;
      IdentityCache identityCache = new IdentityCache(requestContext, hostDomain, this.m_cacheSize, (IScopeMapper) new ScopeIdMapper(requestContext), (IIdMapper) new IdentityIdMapper(requestContext, hostDomain.IsMaster));
      this.m_identityCaches.TryAdd(hostDomain.DomainId, identityCache);
    }

    public IIdMapper GetIdMapper(IVssRequestContext requestContext, IdentityDomain hostDomain) => this.GetIdentityCache(hostDomain).IdMapper;

    public IScopeMapper GetScopeMapper(IVssRequestContext requestContext, IdentityDomain hostDomain) => this.GetIdentityCache(hostDomain).ScopeMapper;

    public void OnIdentityIdTranslationChanged(
      IVssRequestContext requestContext,
      IdentityIdTranslationChangeData identityIdTranslationChangeData)
    {
      if (identityIdTranslationChangeData == null)
        return;
      requestContext.Trace(80034, TraceLevel.Info, "IdentityService", nameof (PlatformIdentityCache), "Identity Id translation cache cleared");
      requestContext.GetService<IdentityIdTranslationService>().InvalidateIdTranslationCache(requestContext, identityIdTranslationChangeData);
    }

    public bool UpdateScopeAncestorIds(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      HashSet<Guid> ancestorScopeIds)
    {
      try
      {
        requestContext.TraceEnter(4440091, "IdentityService", nameof (PlatformIdentityCache), nameof (UpdateScopeAncestorIds));
        bool result = this.GetIdentityCache(hostDomain).UpdateScopeAncestorIds(scopeId, ancestorScopeIds);
        ImsCacheUtils.Trace(requestContext, 4440095, TraceLevel.Verbose, "IdentityService", nameof (PlatformIdentityCache), 4, (Func<string>) (() => string.Format("Request: UpdateScopeAncestorIds[Domain: {0}, ScopeId: {1}, AncestorIds: {2}]\nResponse: [result: {3}]", (object) hostDomain, (object) scopeId, (object) ancestorScopeIds.Serialize<HashSet<Guid>>(), (object) result)));
        return result;
      }
      finally
      {
        requestContext.TraceLeave(4440099, "IdentityService", nameof (PlatformIdentityCache), nameof (UpdateScopeAncestorIds));
      }
    }

    public bool TryReadScopeAncestorIds(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      out HashSet<Guid> ancestorScopeIds)
    {
      try
      {
        requestContext.TraceEnter(4440101, "IdentityService", nameof (PlatformIdentityCache), nameof (TryReadScopeAncestorIds));
        bool result = this.GetIdentityCache(hostDomain).TryReadScopeAncestorIds(scopeId, out ancestorScopeIds);
        HashSet<Guid> ancestorIdsForTracing = ancestorScopeIds;
        ImsCacheUtils.Trace(requestContext, 4440105, TraceLevel.Verbose, "IdentityService", nameof (PlatformIdentityCache), 4, (Func<string>) (() => string.Format("Request: TryReadScopeAncestorIds[Domain: {0}, ScopeId: {1}]\nResponse: [Result: {2}, AncestorIds: {3}]", (object) hostDomain, (object) scopeId, (object) result, (object) ancestorIdsForTracing.Serialize<HashSet<Guid>>())));
        return result;
      }
      finally
      {
        requestContext.TraceLeave(4440109, "IdentityService", nameof (PlatformIdentityCache), nameof (TryReadScopeAncestorIds));
      }
    }

    IdentityCache IPlatformIdentityCache.GetIdentityCacheByDomain(IdentityDomain hostDomain) => this.GetIdentityCache(hostDomain);

    public IDictionary<IdentityDescriptor, bool> HasAadGroups(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> aadGroupDescriptors)
    {
      try
      {
        requestContext.TraceEnter(4440111, "IdentityService", nameof (PlatformIdentityCache), nameof (HasAadGroups));
        IdentityCache identityCache = this.GetIdentityCache(this.m_rootDomain);
        this.WarmAadGroupsCacheIfNecessary(requestContext, identityCache, this.m_rootDomain);
        IDictionary<IdentityDescriptor, bool> results = identityCache.HasAadGroups(requestContext, aadGroupDescriptors);
        ImsCacheUtils.Trace(requestContext, 4440115, TraceLevel.Verbose, "IdentityService", nameof (PlatformIdentityCache), 4, (Func<string>) (() => "Request: HasAadGroups[Descriptors: " + aadGroupDescriptors.Serialize<IList<IdentityDescriptor>>() + "]\nResponse: [Results: " + results.Serialize<IDictionary<IdentityDescriptor, bool>>() + "]"));
        return results;
      }
      finally
      {
        requestContext.TraceLeave(4440119, "IdentityService", nameof (PlatformIdentityCache), nameof (HasAadGroups));
      }
    }

    public IDictionary<IdentityDescriptor, IdentityMembershipInfo> ReadIdentityMembershipInfo(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      SequenceContext minSequenceContext)
    {
      try
      {
        requestContext.TraceEnter(4440220, "IdentityService", nameof (PlatformIdentityCache), nameof (ReadIdentityMembershipInfo));
        ArgumentUtility.CheckForNull<IList<IdentityDescriptor>>(descriptors, nameof (descriptors));
        SequenceContext currentSequenceContext = (SequenceContext) null;
        if (minSequenceContext == null || !this.TryGetSequenceContext(out currentSequenceContext) || currentSequenceContext == null || currentSequenceContext.GroupSequenceId < minSequenceContext.GroupSequenceId)
        {
          requestContext.TraceConditionally(4440222, TraceLevel.Verbose, "IdentityService", nameof (PlatformIdentityCache), (Func<string>) (() => "Returning nulls, minSequenceContext:" + (minSequenceContext?.ToString() ?? "null") + ", currentSequenceContext:" + (currentSequenceContext?.ToString() ?? "null")));
          return (IDictionary<IdentityDescriptor, IdentityMembershipInfo>) descriptors.ToDedupedDictionary<IdentityDescriptor, IdentityDescriptor, IdentityMembershipInfo>((Func<IdentityDescriptor, IdentityDescriptor>) (x => x), (Func<IdentityDescriptor, IdentityMembershipInfo>) (x => (IdentityMembershipInfo) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        }
        IDictionary<IdentityDescriptor, IdentityMembershipInfo> identityMembershipInfoMap = this.GetIdentityCache(hostDomain).ReadIdentityMembershipInfo(requestContext, descriptors);
        ImsCacheUtils.Trace(requestContext, 4440225, TraceLevel.Verbose, "IdentityService", nameof (PlatformIdentityCache), 4, (Func<string>) (() => "Request: ReadIdentityMembershipInfo[Descriptors: " + descriptors.Serialize<IList<IdentityDescriptor>>() + "]\nResponse: [Results: " + identityMembershipInfoMap.Serialize<IDictionary<IdentityDescriptor, IdentityMembershipInfo>>() + "]"));
        return identityMembershipInfoMap;
      }
      finally
      {
        requestContext.TraceLeave(4440229, "IdentityService", nameof (PlatformIdentityCache), nameof (ReadIdentityMembershipInfo));
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SocialDescriptor socialDescriptor)
    {
      try
      {
        requestContext.TraceEnter(824607, "IdentityService", nameof (PlatformIdentityCache), nameof (ReadIdentity));
        if (socialDescriptor == new SocialDescriptor())
          return (Microsoft.VisualStudio.Services.Identity.Identity) null;
        IdentityCache identityCache = this.GetIdentityCache(hostDomain);
        this.WarmCacheIfNecessary(requestContext, identityCache, hostDomain);
        Microsoft.VisualStudio.Services.Identity.Identity result = identityCache.ReadIdentity(requestContext, socialDescriptor, new QueryMembership?(QueryMembership.None));
        if (requestContext.IsTracing(436397, TraceLevel.Info, "IdentityService", nameof (PlatformIdentityCache)))
        {
          string callStack = requestContext.IsTracing(194144, TraceLevel.Info, "IdentityService", nameof (PlatformIdentityCache)) ? Environment.StackTrace : string.Empty;
          IdentityTracing.TraceIdentityCacheRead(IdentityTracing.TargetStoreType.L1Cache, hostDomain, socialDescriptor.ToString(), QueryMembership.None, result == null ? IdentityTracing.CacheResult.Miss : IdentityTracing.CacheResult.Hit, callStack);
        }
        ImsCacheUtils.Trace(requestContext, 436397, TraceLevel.Verbose, "IdentityService", nameof (PlatformIdentityCache), 4, (Func<string>) (() => string.Format("Request: ReadIdentityBySocialDescriptor[Domain: {0}, Descriptor: {1}, QueryMembership: {2}]\nResponse: [Identity: {3}]", (object) hostDomain, (object) socialDescriptor, (object) QueryMembership.None, (object) result?.GetProperty<Guid>("CUID", result.Id))));
        return IdentityCacheHelper.IsValidCacheData(requestContext, result, 1754844) ? result : (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      finally
      {
        requestContext.TraceLeave(824607, "IdentityService", nameof (PlatformIdentityCache), nameof (ReadIdentity));
      }
    }

    private void WarmAadGroupsCacheIfNecessary(
      IVssRequestContext requestContext,
      IdentityCache cache,
      IdentityDomain domain)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.m_cacheWarmer?.WarmAadGroupsCache(requestContext, cache, domain);
    }
  }
}
