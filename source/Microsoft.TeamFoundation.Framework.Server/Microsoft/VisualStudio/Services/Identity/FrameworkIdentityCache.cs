// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkIdentityCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class FrameworkIdentityCache : 
    IdentityCacheBase,
    IFrameworkIdentityCache,
    IIdentityCache,
    IIdentityPropertiesCache
  {
    private readonly IIdentityPropertiesCache m_propertiesCache;
    private const string s_area = "IdentityService";
    private const string s_layer = "FrameworkIdentityCache";

    public FrameworkIdentityCache(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      int cacheSize,
      int propertyCacheSize)
      : base(requestContext, hostDomain, cacheSize)
    {
      requestContext.Trace(80117, TraceLevel.Verbose, "IdentityService", nameof (FrameworkIdentityCache), "FrameworkIdentityCache CacheSize set to value: {0}", (object) this.m_cacheSize);
      this.m_propertiesCache = (IIdentityPropertiesCache) new IdentityPropertiesCache(requestContext, propertyCacheSize);
    }

    public override void AddDomain(IVssRequestContext requestContext, IdentityDomain hostDomain)
    {
      base.AddDomain(requestContext, hostDomain);
      this.m_identityCaches[hostDomain.DomainId].IdentityEvicted += new IdentityEvictedHandler(this.OnCachedIdentityEvicted);
    }

    public void FilterUnchangedIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identityToFilter)
    {
      this.m_propertiesCache.FilterUnchangedIdentityProperties(requestContext, hostDomain, propertyNameFilters, identityToFilter);
    }

    public bool EnrichIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identityToEnrich)
    {
      return this.m_propertiesCache.EnrichIdentityProperties(requestContext, hostDomain, propertyNameFilters, identityToEnrich);
    }

    public bool UpdateIdentityProperties(
      IVssRequestContext requestContext,
      IDictionary<Guid, Dictionary<string, object>> changedIdentityProperties)
    {
      return this.m_propertiesCache.UpdateIdentityProperties(requestContext, changedIdentityProperties);
    }

    public void UpdateIdentityProperties(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.m_propertiesCache.UpdateIdentityProperties(requestContext, hostDomain, queryMembership, propertyNameFilters, identity);
    }

    public void OnCachedIdentityEvicted(object sender, IdentityRemovedEventArgs args) => this.m_propertiesCache.OnCachedIdentityEvicted(sender, args);

    public IEnumerable<string> GetPrefetchedProperties() => this.m_propertiesCache.GetPrefetchedProperties();

    public bool ProcessChanges(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      ICollection<IdentityDescriptor> identityChanges)
    {
      if (identityChanges.IsNullOrEmpty<IdentityDescriptor>())
        return false;
      bool flag = false;
      foreach (IdentityCache identityCache in this.m_identityCaches.Values.ToArray<IdentityCache>())
      {
        flag = identityCache.Update(requestContext, (IEnumerable<IdentityDescriptor>) identityChanges);
        this.IncrementCacheInvalidationPerfCounters();
      }
      return flag;
    }

    public override void Unload(IVssRequestContext requestContext)
    {
      foreach (IdentityCache identityCache in this.m_identityCaches.Values.ToArray<IdentityCache>())
        identityCache.IdentityEvicted -= new IdentityEvictedHandler(this.OnCachedIdentityEvicted);
      this.m_propertiesCache.Dispose(requestContext);
      base.Unload(requestContext);
    }

    public void Dispose(IVssRequestContext requestContext) => throw new NotImplementedException();
  }
}
