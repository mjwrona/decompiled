// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformIdentityCacheWarmer
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class PlatformIdentityCacheWarmer : IIdentityCacheWarmer
  {
    private readonly IIdentityReader identityReader;
    private const string cacheWarmingIntervalRegistryPath = "/Configuration/Identity/Cache/RewarmingInterval";
    private const string c_hasAadGroupsCacheWarmingIntervalRegistryPath = "/Configuration/Identity/Cache/HasAadGroups/RewarmingInterval";
    private const int c_defaultHasAadGroupsCacheWarmingIntervalInHours = 6;
    private const string s_area = "IdentityCache";
    private const string s_layer = "PlatformIdentityCacheWarmer";

    public PlatformIdentityCacheWarmer(IIdentityReader identityReader) => this.identityReader = identityReader;

    public void WarmCache(
      IVssRequestContext requestContext,
      IdentityCache cacheToWarm,
      IdentityDomain domain,
      bool rewarmCache = false)
    {
      requestContext.TraceEnter(80057, "IdentityCache", nameof (PlatformIdentityCacheWarmer), nameof (WarmCache));
      try
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PrewarmPlatformIdentityCache") || ((DateTime.UtcNow - cacheToWarm.LastWarmedTime).Hours >= requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Identity/Cache/RewarmingInterval", 6) & rewarmCache ? 1 : (!cacheToWarm.IsWarm ? 1 : 0)) == 0)
          return;
        IdentitySnapshot identitySnapshot = this.identityReader.ReadIdentitySnapshotFromDatabase(requestContext, domain.DomainId, (HashSet<Guid>) null);
        HashSet<Guid> identityIds = new HashSet<Guid>((IEnumerable<Guid>) identitySnapshot.IdentityIds);
        foreach (Microsoft.VisualStudio.Services.Identity.Identity group in identitySnapshot.Groups)
          identityIds.Add(group.Id);
        this.WarmCache(requestContext, cacheToWarm, domain, (ICollection<Guid>) identityIds);
        cacheToWarm.LastWarmedTime = DateTime.UtcNow;
        cacheToWarm.IsWarm = true;
      }
      finally
      {
        requestContext.TraceLeave(80058, "IdentityCache", nameof (PlatformIdentityCacheWarmer), nameof (WarmCache));
      }
    }

    public void WarmCache(
      IVssRequestContext requestContext,
      IdentityCache cacheToWarm,
      IdentityDomain domain,
      ICollection<Guid> identityIds)
    {
      requestContext.TraceEnter(80059, "IdentityCache", nameof (PlatformIdentityCacheWarmer), nameof (WarmCache));
      try
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in this.identityReader.ReadIdentitiesFromDatabase(requestContext, domain, (IList<IdentityDescriptor>) null, (IList<Guid>) identityIds.ToList<Guid>(), QueryMembership.Direct, QueryMembership.Direct, false, false))
        {
          if (identity != null)
          {
            cacheToWarm.Add(requestContext, identity, QueryMembership.Direct);
            cacheToWarm.Add(requestContext, identity.Clone(false), QueryMembership.None);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(80060, "IdentityCache", nameof (PlatformIdentityCacheWarmer), nameof (WarmCache));
      }
    }

    public void WarmAadGroupsCache(
      IVssRequestContext requestContext,
      IdentityCache cacheToWarm,
      IdentityDomain domain)
    {
      try
      {
        requestContext.TraceEnter(80169, "IdentityCache", nameof (PlatformIdentityCacheWarmer), nameof (WarmAadGroupsCache));
        if (PlatformIdentityCacheWarmer.IsAadCacheWarm(requestContext, cacheToWarm))
          return;
        List<IdentityDescriptor> list = this.identityReader.ListApplicationGroups(requestContext.To(TeamFoundationHostType.Application), domain, new Guid[1]
        {
          requestContext.ServiceHost.OrganizationServiceHost.InstanceId
        }, false, false, false, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && AadIdentityHelper.IsAadGroup(x.Descriptor))).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (x => x.Descriptor)).ToList<IdentityDescriptor>();
        cacheToWarm.AddAadGroups(requestContext, (IList<IdentityDescriptor>) list);
        cacheToWarm.LastWarmedTimeForAadGroupsCache = DateTime.UtcNow;
        cacheToWarm.IsAadGroupsCacheWarm = true;
      }
      finally
      {
        requestContext.TraceLeave(80170, "IdentityCache", nameof (PlatformIdentityCacheWarmer), nameof (WarmAadGroupsCache));
      }
    }

    private static bool IsAadCacheWarm(IVssRequestContext requestContext, IdentityCache cacheToWarm)
    {
      if (cacheToWarm.IsAadGroupsCacheWarm)
        return true;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Identity/Cache/HasAadGroups/RewarmingInterval", 6);
      return cacheToWarm.LastWarmedTimeForAadGroupsCache + TimeSpan.FromHours((double) num) > DateTime.UtcNow;
    }
  }
}
