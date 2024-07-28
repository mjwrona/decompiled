// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityStoreBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Cache;
using Microsoft.VisualStudio.Services.Identity.SearchFilter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class IdentityStoreBase
  {
    private SequenceContextRedisCache m_sequenceContextCache;
    public const string SequenceContextCachingConfig = "SequenceContext.CachingConfig";
    private static readonly IConfigPrototype<IdentityStoreBase.SequenceContextCachingSettings> configPrototype = ConfigPrototype.Create<IdentityStoreBase.SequenceContextCachingSettings>("SequenceContext.CachingConfig", new IdentityStoreBase.SequenceContextCachingSettings());
    private const int c_defaultMegaTenantSize = 5000;
    private const string c_area = "IdentityStoreBase";
    private const string c_layer = "IdentityStoreBase";
    private const string c_sequenceIdsLayer = "SequenceIDs";
    private const string c_SearchIdentitiesLayer = "SearchIdentities";
    private const string s_featureNameStoreMegaTenantAtCollection = "VisualStudio.Services.Identity.StoreMegaTenantAtCollection";
    private const string s_featureNameProjectScopedMaterializedAadResults = "VisualStudio.Services.Identity.EnableProjectScopedMaterializedAadResults";

    internal IConfigQueryable<IdentityStoreBase.SequenceContextCachingSettings> Config { get; set; }

    public IdentityStoreBase()
    {
      this.Config = ConfigProxy.Create<IdentityStoreBase.SequenceContextCachingSettings>(IdentityStoreBase.configPrototype);
      this.m_sequenceContextCache = new SequenceContextRedisCache();
    }

    internal abstract IdentityDomain Domain { get; }

    internal abstract IIdentityCache IdentityCache { get; }

    internal void InvalidateSequenceContextInternal(IVssRequestContext requestContext)
    {
      IdentityStoreBase.SequenceContextCachingSettings contextCachingSettings = this.Config.QueryByCtx<IdentityStoreBase.SequenceContextCachingSettings>(requestContext);
      if (!contextCachingSettings.EnableInvalidation)
        return;
      IVssRequestContext projectCollection = IdentityStoreBase.GetParentContextIfProjectCollection(requestContext);
      IdentityStoreBase.InvalidateDelegate invalidateDelegate1 = new IdentityStoreBase.InvalidateDelegate(this.IdentityCache.InvalidateSequenceContext);
      IdentityStoreBase.InvalidateDelegate invalidateDelegate2 = new IdentityStoreBase.InvalidateDelegate(this.InvalidateSequenceContextRedis);
      if (contextCachingSettings.RedisAsPrimaryCache)
      {
        IdentityStoreBase.InvalidateDelegate invalidateDelegate3 = invalidateDelegate2;
        invalidateDelegate2 = invalidateDelegate1;
        invalidateDelegate1 = invalidateDelegate3;
      }
      invalidateDelegate1(projectCollection);
      projectCollection.Trace(1765004, TraceLevel.Info, nameof (IdentityStoreBase), "SequenceIDs", "Invalidated sequence context cache in primary " + (contextCachingSettings.RedisAsPrimaryCache ? "redis" : "original"));
      if (!contextCachingSettings.EnableSecondaryShadowCache)
        return;
      invalidateDelegate2(projectCollection);
      projectCollection.Trace(1765005, TraceLevel.Info, nameof (IdentityStoreBase), "SequenceIDs", "Invalidated sequence context cache in shadow " + (contextCachingSettings.RedisAsPrimaryCache ? "original" : "redis"));
    }

    private void InvalidateSequenceContextRedis(IVssRequestContext targetContext) => this.m_sequenceContextCache.Invalidate(targetContext);

    private static IVssRequestContext GetParentContextIfProjectCollection(
      IVssRequestContext requestContext)
    {
      return requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.To(TeamFoundationHostType.Parent) : requestContext;
    }

    private IdentityDomain GetParentDomainIfNotMaster(IdentityDomain hostDomain) => !hostDomain.IsMaster ? hostDomain.Parent : hostDomain;

    private SequenceContext GetOrSetRedis(
      IVssRequestContext targetContext,
      IdentityDomain targetHostDomain)
    {
      bool cacheHit = true;
      SequenceContext orSet = this.m_sequenceContextCache.GetOrSet(targetContext, (Func<SequenceContext>) (() =>
      {
        cacheHit = false;
        targetContext.Trace(1765002, TraceLevel.Info, nameof (IdentityStoreBase), "SequenceIDs", "redis cache miss, calling onCacheMiss handler");
        return this.GetSequenceLatestSequenceContextOrDefault(targetContext, targetHostDomain);
      }));
      if (cacheHit)
        targetContext.Trace(1765007, TraceLevel.Info, nameof (IdentityStoreBase), "SequenceIDs", "redis cache hit, identitySequenceId {0}; groupSequenceId {1}", (object) orSet.IdentitySequenceId, (object) orSet.GroupSequenceId);
      return orSet;
    }

    private SequenceContext GetOrSet(
      IVssRequestContext targetContext,
      IdentityDomain targetHostDomain)
    {
      SequenceContext sequenceContext;
      if (this.IdentityCache.TryGetSequenceContext(out sequenceContext))
      {
        targetContext.Trace(1765006, TraceLevel.Info, nameof (IdentityStoreBase), "SequenceIDs", "original cache hit, identitySequenceId {0}; groupSequenceId {1}", (object) sequenceContext.IdentitySequenceId, (object) sequenceContext.GroupSequenceId);
        return sequenceContext;
      }
      targetContext.Trace(1765001, TraceLevel.Info, nameof (IdentityStoreBase), "SequenceIDs", "original cache miss, calling GetLatestSequenceContext");
      return this.IdentityCache.CompareAndSwapSequenceContextIfGreater(this.GetSequenceLatestSequenceContextOrDefault(targetContext, targetHostDomain));
    }

    private SequenceContext GetSequenceLatestSequenceContextOrDefault(
      IVssRequestContext targetContext,
      IdentityDomain targetHostDomain)
    {
      try
      {
        return this.GetLatestSequenceContext(targetContext, targetHostDomain);
      }
      catch (Exception ex)
      {
        targetContext.TraceException(1765000, nameof (IdentityStoreBase), "SequenceIDs", ex);
        return SequenceContext.InitSequenceContext;
      }
    }

    internal SequenceContext GetSequenceContext(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain)
    {
      IdentityStoreBase.SequenceContextCachingSettings sequenceContextCachingSettings = this.Config.QueryByCtx<IdentityStoreBase.SequenceContextCachingSettings>(requestContext);
      IdentityDomain domainIfNotMaster = this.GetParentDomainIfNotMaster(hostDomain);
      IVssRequestContext projectCollection = IdentityStoreBase.GetParentContextIfProjectCollection(requestContext);
      IdentityStoreBase.GetOrSetDelegate getOrSetDelegate1 = new IdentityStoreBase.GetOrSetDelegate(this.GetOrSet);
      IdentityStoreBase.GetOrSetDelegate getOrSetDelegate2 = new IdentityStoreBase.GetOrSetDelegate(this.GetOrSetRedis);
      if (sequenceContextCachingSettings.RedisAsPrimaryCache)
      {
        IdentityStoreBase.GetOrSetDelegate getOrSetDelegate3 = getOrSetDelegate2;
        getOrSetDelegate2 = getOrSetDelegate1;
        getOrSetDelegate1 = getOrSetDelegate3;
      }
      SequenceContext sequenceContext = getOrSetDelegate1(projectCollection, domainIfNotMaster);
      if (sequenceContextCachingSettings.EnableSecondaryShadowCache)
      {
        SequenceContext shadowSequenceContext = getOrSetDelegate2(projectCollection, domainIfNotMaster);
        requestContext.TraceConditionally(1765008, TraceLevel.Info, nameof (IdentityStoreBase), "SequenceIDs", (Func<string>) (() => this.ToComparisonString(sequenceContextCachingSettings, sequenceContext, shadowSequenceContext)));
      }
      return sequenceContext;
    }

    private string ToComparisonString(
      IdentityStoreBase.SequenceContextCachingSettings settings,
      SequenceContext primaryContext,
      SequenceContext secondaryContext)
    {
      string str1 = "redis";
      string str2 = "original";
      if (settings.RedisAsPrimaryCache)
      {
        string str3 = str2;
        str2 = str1;
        str1 = str3;
      }
      return string.Format("Sequence Context primary {0} caching: {1}; shadow {2} caching: {3}", (object) str2, (object) primaryContext, (object) str1, (object) secondaryContext);
    }

    internal void ProcessGroupSequenceIdChange(long groupSequenceId) => this.IdentityCache.CompareAndSwapSequenceContextIfGreater(new SequenceContext(-1L, groupSequenceId, -1L));

    protected abstract SequenceContext GetLatestSequenceContext(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain);

    public abstract IdentityScope GetScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId);

    public virtual Guid GetScopeParentId(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId)
    {
      Guid parentScopeId;
      if (!this.IdentityCache.TryReadScopeParent(requestContext, hostDomain, scopeId, out parentScopeId))
      {
        IdentityScope scope = this.GetScope(requestContext, this.Domain, scopeId);
        if (scope != null)
        {
          parentScopeId = scope.ParentId;
          this.IdentityCache.GetOrAddScopeParent(requestContext, this.Domain, scopeId, parentScopeId);
        }
      }
      return parentScopeId;
    }

    internal abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesInScope(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters);

    public abstract bool IsMember(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentityDescriptor groupDescriptor,
      IdentityDescriptor memberDescriptor);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IList<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility);

    public abstract Microsoft.VisualStudio.Services.Identity.Identity ReadIdentity(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      SocialDescriptor socialDescriptor,
      bool bypassCache = false);

    public abstract IList<Microsoft.VisualStudio.Services.Identity.Identity> ListGroups(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid[] scopeIds,
      bool recurse,
      bool deleted,
      IEnumerable<string> propertyNameFilters);

    public virtual IdentitySearchResult SearchIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IdentitySearchParameters searchParameters)
    {
      requestContext.TraceEnter(1755000, nameof (IdentityStoreBase), nameof (IdentityStoreBase), nameof (SearchIdentities));
      requestContext.TraceEnter(1755100, nameof (IdentityStoreBase), nameof (SearchIdentities), nameof (SearchIdentities));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IdentityDomain>(hostDomain, nameof (hostDomain));
      ArgumentUtility.CheckForNull<IdentitySearchParameters>(searchParameters, nameof (searchParameters));
      IdentitySearchHelper.ValidateSearchIdentitiesContext(requestContext);
      requestContext.TraceConditionally(1755104, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), (Func<string>) (() => string.Format("SearchIdentities with hostDomain={0}, searchParameters:{1}", (object) hostDomain.DomainId, (object) searchParameters.Serialize<IdentitySearchParameters>())));
      PagingContext pagingContextInfo = this.GetPagingContextInfo(searchParameters);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> actualResults;
      IList<IdentityId> cachedIdentityIds;
      this.SearchIdentities(requestContext, hostDomain, pagingContextInfo, out cachedIdentityIds, out actualResults);
      IVssRequestContext requestContext1 = requestContext;
      IList<IdentityId> identityIdList = cachedIdentityIds;
      int count;
      string str1;
      if (identityIdList == null)
      {
        str1 = (string) null;
      }
      else
      {
        count = identityIdList.Count;
        str1 = count.ToString();
      }
      if (str1 == null)
        str1 = "null";
      string str2;
      if (actualResults == null)
      {
        str2 = (string) null;
      }
      else
      {
        count = actualResults.Count;
        str2 = count.ToString();
      }
      if (str2 == null)
        str2 = "null";
      requestContext1.Trace(1755108, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), "SearchIdentities returned with cachedIdentityIds={0}, actualResults={1}", (object) str1, (object) str2);
      if (cachedIdentityIds != null)
      {
        requestContext.TraceConditionally(1755110, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), (Func<string>) (() => string.Format("Matching cachedIdentityIds = {0} for scopeId:{1}", (object) cachedIdentityIds.Serialize<IList<IdentityId>>(), (object) hostDomain.DomainId)));
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.ComputeCachedSearchResult(requestContext, hostDomain, searchParameters, pagingContextInfo, cachedIdentityIds);
        requestContext.TraceConditionally(1755114, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), (Func<string>) (() => string.Format("Matching identities = {0} for scopeId:{1}", (object) identities.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id)).Serialize<IEnumerable<Guid>>(), (object) hostDomain.DomainId)));
        return new IdentitySearchResult(identities, pagingContextInfo.ToBase64());
      }
      if (actualResults != null)
      {
        requestContext.Trace(1755116, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), string.Format("ComputeOnDemandSearchResult for scopeId:{0}", (object) hostDomain.DomainId));
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.ComputeOnDemandSearchResult(requestContext, hostDomain, actualResults, searchParameters, pagingContextInfo);
        requestContext.TraceConditionally(1755118, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), (Func<string>) (() => string.Format("Matching identities = {0} for scopeId:{1}", (object) identities.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id)).Serialize<IEnumerable<Guid>>(), (object) hostDomain.DomainId)));
        return new IdentitySearchResult(identities, pagingContextInfo.ToBase64());
      }
      requestContext.Trace(1755120, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), string.Format("Returning null identities IdentitySearchResult for scopeId:{0}", (object) hostDomain.DomainId));
      return new IdentitySearchResult((IList<Microsoft.VisualStudio.Services.Identity.Identity>) null, (string) null);
    }

    private void SearchIdentities(
      IVssRequestContext requestContext,
      IdentityDomain domain,
      PagingContext pagingContext,
      out IList<IdentityId> cachedIdentityIds,
      out IList<Microsoft.VisualStudio.Services.Identity.Identity> actualResults)
    {
      requestContext.TraceEnter(1755220, nameof (IdentityStoreBase), nameof (IdentityStoreBase), nameof (SearchIdentities));
      cachedIdentityIds = (IList<IdentityId>) null;
      actualResults = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      Guid guid = pagingContext.ScopeId == new Guid() ? domain.DomainId : pagingContext.ScopeId;
      IVssRequestContext organizationContext = IdentitySearchHelper.GetOrganizationContext(requestContext);
      bool isStale;
      cachedIdentityIds = this.SearchIdentityIdsBySearchKindsInCache(organizationContext, guid, pagingContext.SearchKind, pagingContext.Query, out isStale);
      if (cachedIdentityIds != null && !isStale)
        return;
      if (IdentityStoreBase.IsMegaTenant(requestContext))
      {
        requestContext.Trace(1755222, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), string.Format("PublishImsSearchCacheExpiryEvent for scopeId:{0} since it is a mega tenant", (object) domain.DomainId));
        IdentitySearchHelper.PublishImsSearchCacheExpiryEvent(requestContext, guid, true);
      }
      else
      {
        requestContext.Trace(1755224, TraceLevel.Info, nameof (IdentityStoreBase), nameof (SearchIdentities), string.Format("RefreshSearchIdentitiesCacheV2 for scopeId:{0} since it is not a mega tenant", (object) domain.DomainId));
        actualResults = this.RefreshSearchIdentitiesCache(organizationContext, domain, guid);
        cachedIdentityIds = this.SearchIdentityIdsBySearchKindsInCache(organizationContext, guid, pagingContext.SearchKind, pagingContext.Query, out isStale);
      }
    }

    private IdentitySearchResult ListOnPremDeploymentLevelGroups(
      IVssRequestContext requestContext,
      IdentityDomain domain,
      IdentitySearchParameters searchParameters,
      PagingContext pagingContext)
    {
      if (!searchParameters.IdentityTypes.HasFlag((Enum) IdentitySearchType.Group))
        return new IdentitySearchResult((IList<Microsoft.VisualStudio.Services.Identity.Identity>) null, (string) null);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> list = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.ListGroups(requestContext, domain, new Guid[1]
      {
        domain.DomainId
      }, false, false, searchParameters.PropertyNameFilters).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsActive && x.GetProperty<string>("RestrictedVisible", (string) null) == null && !ServicePrincipals.IsServicePrincipal(requestContext, x.Descriptor) && !IdentityHelper.IsAnonymousPrincipal(x.Descriptor))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      return list != null ? new IdentitySearchResult(this.ComputeOnDemandSearchResult(requestContext, domain, list, searchParameters, pagingContext), pagingContext.ToBase64()) : new IdentitySearchResult((IList<Microsoft.VisualStudio.Services.Identity.Identity>) null, (string) null);
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ComputeCachedSearchResult(
      IVssRequestContext context,
      IdentityDomain domain,
      IdentitySearchParameters searchParameters,
      PagingContext pagingContext,
      IList<IdentityId> cachedIdentityIds)
    {
      cachedIdentityIds = IdentityStoreBase.GetPageAndUpdatePagingContext<IdentityId>(this.FilterIdentitySearchResults<IdentityId>(context, domain, searchParameters, cachedIdentityIds, (Func<IdentityId, IdentityDescriptor>) (identityId => identityId.Descriptor)), pagingContext, (Func<IdentityId, bool>) (x => (pagingContext.IdentitySearchType & IdentityStoreBase.ConvertToIdentitySearchType(x.Type)) != 0));
      return this.ReadIdentities(context, domain, (IList<IdentityDescriptor>) cachedIdentityIds.Select<IdentityId, IdentityDescriptor>((Func<IdentityId, IdentityDescriptor>) (x => x.Descriptor)).ToArray<IdentityDescriptor>(), QueryMembership.None, searchParameters.PropertyNameFilters, false);
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ComputeOnDemandSearchResult(
      IVssRequestContext requestContext,
      IdentityDomain domain,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> allIdentities,
      IdentitySearchParameters searchParameters,
      PagingContext pagingContext)
    {
      HashSet<Microsoft.VisualStudio.Services.Identity.Identity> source = new HashSet<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (searchParameters.SearchKind.HasFlag((Enum) IdentitySearchKind.DisplayNamePrefix))
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> other = allIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !string.IsNullOrWhiteSpace(x.DisplayName) && x.DisplayName.StartsWith(pagingContext.Query, StringComparison.InvariantCultureIgnoreCase)));
        source.UnionWith(other);
      }
      if (searchParameters.SearchKind.HasFlag((Enum) IdentitySearchKind.EmailPrefix))
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> other = allIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !string.IsNullOrWhiteSpace(x.Properties.GetValue<string>("Mail", string.Empty)) && x.Properties.GetValue<string>("Mail", string.Empty).StartsWith(pagingContext.Query, StringComparison.InvariantCultureIgnoreCase)));
        source.UnionWith(other);
      }
      if (searchParameters.SearchKind.HasFlag((Enum) IdentitySearchKind.AccountNamePrefix))
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> other = allIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !string.IsNullOrWhiteSpace(x.Properties.GetValue<string>("Account", string.Empty)) && x.Properties.GetValue<string>("Account", string.Empty).StartsWith(pagingContext.Query, StringComparison.InvariantCultureIgnoreCase)));
        source.UnionWith(other);
      }
      return IdentityStoreBase.GetPageAndUpdatePagingContext<Microsoft.VisualStudio.Services.Identity.Identity>(this.FilterIdentitySearchResults<Microsoft.VisualStudio.Services.Identity.Identity>(requestContext, domain, searchParameters, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source.ToList<Microsoft.VisualStudio.Services.Identity.Identity>(), (Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor)), pagingContext, (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => (pagingContext.IdentitySearchType & IdentityStoreBase.ConvertToIdentitySearchType((IReadOnlyVssIdentity) x)) != 0));
    }

    private IEnumerable<T> FilterIdentitySearchResults<T>(
      IVssRequestContext requestContext,
      IdentityDomain domain,
      IdentitySearchParameters searchParameters,
      IList<T> identities,
      Func<T, IdentityDescriptor> getIdentityDescriptor)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.DisableIdentitySearchFilters"))
        return (IEnumerable<T>) identities;
      IdentityStoreBase.IdentityStoreProvider identityStoreProvider = new IdentityStoreBase.IdentityStoreProvider(this, domain);
      return new AncestorAndIdentitySearchFilter(requestContext, (Microsoft.VisualStudio.Services.Identity.SearchFilter.IIdentityProvider) identityStoreProvider, searchParameters).FilterIdentities<T>(requestContext, (IEnumerable<T>) identities, getIdentityDescriptor);
    }

    internal IList<Microsoft.VisualStudio.Services.Identity.Identity> RefreshSearchIdentitiesCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      Guid scopeId)
    {
      requestContext.TraceEnter(1755300, nameof (IdentityStoreBase), "SearchIdentities", nameof (RefreshSearchIdentitiesCache));
      IdentitySearchHelper.ValidateSearchIdentitiesContext(requestContext);
      IVssRequestContext organizationContext = IdentitySearchHelper.GetOrganizationContext(requestContext);
      IdentityDomain organizationDomain = IdentitySearchHelper.GetOrganizationDomain(hostDomain);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1;
      if (!organizationDomain.DomainId.Equals(scopeId))
        identityList1 = this.ReadIdentitiesInScope(organizationContext, organizationDomain, scopeId, QueryMembership.None, (IEnumerable<string>) null);
      else
        identityList1 = this.ListGroups(organizationContext, organizationDomain, new Guid[1]
        {
          scopeId
        }, false, false, (IEnumerable<string>) null);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList2 = identityList1;
      requestContext.Trace(1755302, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", string.Format("IdentitiesInScope={0}, scopeId={1}, hostDomain:{2}", (object) identityList2.Count, (object) scopeId, (object) hostDomain.DomainId));
      IdentityScope scope = this.GetScope(requestContext, hostDomain, scopeId);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.EnableProjectScopedMaterializedAadResults") && scope != null && scope.ScopeType == GroupScopeType.TeamProject)
      {
        List<IdentityDescriptor> list = identityList2.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (o => o.IsContainer && AadIdentityHelper.IsAadGroup(o.Descriptor))).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (o => o.Descriptor)).ToList<IdentityDescriptor>();
        IAadTenantDetailProvider extension = organizationContext.GetExtension<IAadTenantDetailProvider>();
        requestContext.Trace(1755306, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", string.Format("aadGroups={0}, scope={1}, hostDomain:{2}", (object) list.Count, (object) scopeId, (object) hostDomain.DomainId));
        IVssRequestContext requestContext1 = organizationContext;
        List<IdentityDescriptor> aadGroupDescriptors = list;
        List<IdentityDescriptor> groupsDescendants = extension.GetGroupsDescendants(requestContext1, aadGroupDescriptors);
        if (!groupsDescendants.IsNullOrEmpty<IdentityDescriptor>())
        {
          IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.ReadIdentities(organizationContext, organizationDomain, (IList<IdentityDescriptor>) groupsDescendants, QueryMembership.None, (IEnumerable<string>) null, false).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (o => o != null));
          identityList2.AddRange<Microsoft.VisualStudio.Services.Identity.Identity, IList<Microsoft.VisualStudio.Services.Identity.Identity>>(identities);
          requestContext.Trace(1755308, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", string.Format("MaterializedAadIdentities={0}, scopeId={1}, hostDomain:{2}", (object) identities.Count<Microsoft.VisualStudio.Services.Identity.Identity>(), (object) scopeId, (object) hostDomain.DomainId));
        }
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = IdentitySearchHelper.FilterOutSystemIdentities(requestContext, identityList2);
      requestContext.Trace(1755304, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", string.Format("ActualIdentities={0}, scopeId={1}, hostDomain:{2}", (object) source.Count, (object) scopeId, (object) hostDomain.DomainId));
      try
      {
        IImsCacheService service = organizationContext.GetService<IImsCacheService>();
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        Dictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> dictionary = source.Distinct<Microsoft.VisualStudio.Services.Identity.Identity>().ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x), IdentityStoreBase.\u003C\u003EO.\u003C0\u003E__ExtractIdentityId ?? (IdentityStoreBase.\u003C\u003EO.\u003C0\u003E__ExtractIdentityId = new Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>(ImsCacheUtils.ExtractIdentityId)));
        service.CreateSearchIndexByDisplayName(organizationContext, scopeId, (IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>) dictionary);
        service.CreateSearchIndexByEmail(organizationContext, scopeId, (IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>) dictionary);
        service.CreateSearchIndexByAccountName(organizationContext, scopeId, (IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>) dictionary);
        service.CreateSearchIndexByDomainAccountName(organizationContext, scopeId, (IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>) dictionary);
        service.CreateSearchIndexByAppId(organizationContext, scopeId, (IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>) dictionary);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1755320, nameof (IdentityStoreBase), "SearchIdentities", ex);
      }
      return source;
    }

    private static IdentitySearchType ConvertToIdentitySearchType(Microsoft.VisualStudio.Services.Identity.Cache.IdentityType identityType)
    {
      IdentitySearchType identitySearchType = IdentitySearchType.None;
      if ((identityType & Microsoft.VisualStudio.Services.Identity.Cache.IdentityType.User) != Microsoft.VisualStudio.Services.Identity.Cache.IdentityType.None)
        identitySearchType = IdentitySearchType.User;
      if ((identityType & Microsoft.VisualStudio.Services.Identity.Cache.IdentityType.Group) != Microsoft.VisualStudio.Services.Identity.Cache.IdentityType.None)
        identitySearchType |= IdentitySearchType.Group;
      if ((identityType & Microsoft.VisualStudio.Services.Identity.Cache.IdentityType.ServicePrincipal) != Microsoft.VisualStudio.Services.Identity.Cache.IdentityType.None)
        identitySearchType |= IdentitySearchType.ServicePrincipal;
      return identitySearchType;
    }

    private static IdentitySearchType ConvertToIdentitySearchType(IReadOnlyVssIdentity identity) => !identity.IsContainer ? IdentitySearchType.User : IdentitySearchType.Group;

    private static IList<T> GetPageAndUpdatePagingContext<T>(
      IEnumerable<T> list,
      PagingContext pagingContext,
      Func<T, bool> checkType)
    {
      IList<T> list1;
      if (pagingContext.IdentitySearchType == IdentitySearchType.All)
      {
        list1 = (IList<T>) list.Skip<T>(pagingContext.LastRecordIndex).Take<T>(pagingContext.PageSize).ToList<T>();
        pagingContext.LastRecordIndex += list1.Count;
      }
      else
      {
        List<T> list2 = list.Skip<T>(pagingContext.LastRecordIndex).ToList<T>();
        list1 = (IList<T>) list2.Where<T>(checkType).Take<T>(pagingContext.PageSize).ToList<T>();
        if (list1.Any<T>())
        {
          int num = list2.IndexOf(list1.Last<T>());
          pagingContext.LastRecordIndex += num + 1;
        }
      }
      return list1;
    }

    private PagingContext GetPagingContextInfo(IdentitySearchParameters searchParameters)
    {
      PagingContext pagingContextInfo = searchParameters.PagingContext == null ? (PagingContext) null : PagingContext.FromBase64(searchParameters.PagingContext);
      if (pagingContextInfo != null)
      {
        pagingContextInfo.PageSize = searchParameters.PageSize > 0 ? searchParameters.PageSize : pagingContextInfo.PageSize;
        return pagingContextInfo;
      }
      if (searchParameters.PageSize <= 0)
        throw new ArgumentException("Page size cannot be 0 or less.", "searchParameters.PageSize");
      if (ArgumentUtility.IsInvalidString(searchParameters.Query, false, true))
        throw new ArgumentException(string.Format("Cannot search for {0}.", (object) searchParameters.Query), "searchParameters.Query");
      if (searchParameters.SearchKind == IdentitySearchKind.None)
        throw new ArgumentException("searchParameters.SearchKind cannot be IdentitySearchKind.None.", "searchParameters.SearchKind");
      if (searchParameters.IdentityTypes == IdentitySearchType.None)
        throw new ArgumentException("searchParameters.IdentityTypes cannot be IdentitySearchType.None.", "searchParameters.IdentityTypes");
      return new PagingContext(0, searchParameters.PageSize, searchParameters.Query, searchParameters.IdentityTypes, searchParameters.SearchKind, searchParameters.ScopeId);
    }

    private IList<IdentityId> SearchIdentityIdsBySearchKindsInCache(
      IVssRequestContext context,
      Guid domainId,
      IdentitySearchKind searchKind,
      string query,
      out bool isStale)
    {
      context.Trace(1755321, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", "displayNamePrefix={0}", (object) IdentitySearchKind.DisplayNamePrefix);
      IImsCacheService service = context.GetService<IImsCacheService>();
      HashSet<IdentityId> identityIdSet = (HashSet<IdentityId>) null;
      isStale = false;
      if ((IdentitySearchKind.DisplayNamePrefix & searchKind) == IdentitySearchKind.DisplayNamePrefix)
      {
        bool isStale1;
        IEnumerable<IdentityId> identityIds = service.SearchIdentityIdsByDisplayName(context, domainId, query, out isStale1);
        isStale |= isStale1;
        if (identityIds != null)
        {
          identityIdSet = new HashSet<IdentityId>(identityIds);
          context.Trace(1755322, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", "stale={0}, identities={1}", (object) isStale1, (object) identityIds.Count<IdentityId>());
        }
      }
      if ((IdentitySearchKind.EmailPrefix & searchKind) == IdentitySearchKind.EmailPrefix)
      {
        bool isStale2;
        IEnumerable<IdentityId> identityIds = service.SearchIdentityIdsByEmail(context, domainId, query, out isStale2);
        isStale |= isStale2;
        if (identityIds != null)
        {
          identityIdSet = identityIdSet == null ? new HashSet<IdentityId>(identityIds) : new HashSet<IdentityId>(identityIdSet.Concat<IdentityId>(identityIds));
          context.Trace(1755323, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", "stale={0}, identities={1}", (object) isStale2, (object) identityIds.Count<IdentityId>());
        }
      }
      if ((IdentitySearchKind.AccountNamePrefix & searchKind) == IdentitySearchKind.AccountNamePrefix)
      {
        bool isStale3;
        IEnumerable<IdentityId> identityIds = service.SearchIdentityIdsByAccountName(context, domainId, query, out isStale3);
        isStale |= isStale3;
        if (identityIds != null)
        {
          identityIdSet = identityIdSet == null ? new HashSet<IdentityId>(identityIds) : new HashSet<IdentityId>(identityIdSet.Concat<IdentityId>(identityIds));
          context.Trace(1755324, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", "stale={0}, identities={1}", (object) isStale3, (object) identityIds.Count<IdentityId>());
        }
      }
      if ((IdentitySearchKind.DomainAccountNamePrefix & searchKind) == IdentitySearchKind.DomainAccountNamePrefix)
      {
        bool isStale4;
        IEnumerable<IdentityId> identityIds = service.SearchIdentityIdsByDomainAccountName(context, domainId, query, out isStale4);
        isStale |= isStale4;
        if (identityIds != null)
        {
          identityIdSet = identityIdSet == null ? new HashSet<IdentityId>(identityIds) : new HashSet<IdentityId>(identityIdSet.Concat<IdentityId>(identityIds));
          context.Trace(1755325, TraceLevel.Info, nameof (IdentityStoreBase), "SearchIdentities", "stale={0}, identities={1}", (object) isStale4, (object) identityIds.Count<IdentityId>());
        }
      }
      if ((IdentitySearchKind.AppId & searchKind) == IdentitySearchKind.AppId)
      {
        bool isStale5;
        bool inputParameterError;
        IEnumerable<IdentityId> identityIds = service.SearchIdentityIdsByAppId(context, domainId, query, out isStale5, out inputParameterError);
        isStale |= isStale5;
        if (identityIds != null && !inputParameterError)
          identityIdSet = identityIdSet == null ? new HashSet<IdentityId>(identityIds) : new HashSet<IdentityId>(identityIdSet.Concat<IdentityId>(identityIds));
      }
      return identityIdSet == null ? (IList<IdentityId>) null : (IList<IdentityId>) identityIdSet.ToList<IdentityId>();
    }

    protected void UpdateMegaTenantState(IVssRequestContext requestContext, int count)
    {
      if (!requestContext.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.SearchCacheWarmup") || !IdentityStoreBase.IsSearchContext(requestContext))
        return;
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      int num = vssRequestContext1.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext1, (RegistryQuery) "/Service/Identity/Settings/IdentityMegaTenantSize", 5000);
      bool flag = IdentityStoreBase.IsMegaTenant(requestContext);
      IVssRequestContext vssRequestContext2 = requestContext;
      if (vssRequestContext1.IsFeatureEnabled("VisualStudio.Services.Identity.StoreMegaTenantAtCollection") || vssRequestContext2.IsVirtualServiceHost())
      {
        vssRequestContext2 = vssRequestContext2.RootContext;
        if (!vssRequestContext2.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          return;
      }
      IVssRegistryService service = vssRequestContext2.GetService<IVssRegistryService>();
      if (count > num)
      {
        if (flag)
          return;
        service.SetValue<bool>(vssRequestContext2, "/Service/Identity/Settings/IdentityMegaTenant", true);
      }
      else
      {
        if (!flag)
          return;
        service.SetValue<bool>(vssRequestContext2, "/Service/Identity/Settings/IdentityMegaTenant", false);
      }
    }

    internal static bool IsSearchContext(IVssRequestContext requestContext) => !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application) || !requestContext.ExecutionEnvironment.IsOnPremisesDeployment) && (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || !requestContext.ExecutionEnvironment.IsHostedDeployment);

    internal static bool IsMegaTenant(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || requestContext.ServiceHost.Is(TeamFoundationHostType.Application) && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      IVssRequestContext vssRequestContext = requestContext;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ExecutionEnvironment.IsHostedDeployment)
        vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      if (requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.Services.Identity.StoreMegaTenantAtCollection") || vssRequestContext.IsVirtualServiceHost())
      {
        vssRequestContext = vssRequestContext.RootContext;
        if (!vssRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          return false;
      }
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<bool>(vssRequestContext, (RegistryQuery) "/Service/Identity/Settings/IdentityMegaTenant", false);
    }

    internal virtual void ClearCache(IVssRequestContext requestContext) => this.IdentityCache.Clear(requestContext);

    internal virtual void SweepCache(IVssRequestContext requestContext) => this.IdentityCache.Sweep(requestContext);

    internal class SequenceContextCachingSettings
    {
      public bool EnableInvalidation { get; set; }

      public bool RedisAsPrimaryCache { get; set; }

      public bool EnableSecondaryShadowCache { get; set; }
    }

    private delegate void InvalidateDelegate(IVssRequestContext requestContext);

    private delegate SequenceContext GetOrSetDelegate(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain);

    protected enum TaskState
    {
      NotQueued,
      Queueing,
      Queued,
    }

    internal sealed class IdentityStoreProvider : Microsoft.VisualStudio.Services.Identity.SearchFilter.IIdentityProvider
    {
      private readonly IdentityStoreBase IdentityStoreBase;
      private readonly IdentityDomain IdentityDomain;

      internal IdentityStoreProvider(
        IdentityStoreBase identityStoreBase,
        IdentityDomain identityDomain)
      {
        this.IdentityStoreBase = identityStoreBase;
        this.IdentityDomain = identityDomain;
      }

      public IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
        IVssRequestContext requestContext,
        IList<IdentityDescriptor> descriptors)
      {
        return this.IdentityStoreBase.ReadIdentities(requestContext, this.IdentityDomain, descriptors, QueryMembership.None, (IEnumerable<string>) null, false);
      }

      public bool IsMember(
        IVssRequestContext requestContext,
        IdentityDescriptor groupDescriptor,
        IdentityDescriptor memberDescriptor)
      {
        return this.IdentityStoreBase.IsMember(requestContext, this.IdentityDomain, groupDescriptor, memberDescriptor);
      }
    }
  }
}
