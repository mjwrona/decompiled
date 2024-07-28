// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostAcquisition.OrganizationAcquisitionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.HostAcquisition
{
  internal class OrganizationAcquisitionService : 
    IOrganizationAcquisitionService,
    IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private OrganizationAcquisitionService.RegionsCache m_regionsCache;
    private TimeSpan m_regionsCacheRefreshTaskExecutionInterval;
    private TimeSpan m_regionsCacheTimeToLive;
    private TimeSpan m_regionsCacheSoonToExpireThreshold;
    private static readonly Random s_random = new Random();
    private static readonly TimeSpan s_defaultRegionsCacheRefreshTaskExecutionInterval = TimeSpan.FromMinutes(1.0);
    private static readonly TimeSpan s_defaultRegionsCacheTimeToLive = TimeSpan.FromMinutes(10.0);
    private static readonly TimeSpan s_defaultRegionsCacheSoonToExpireThreshold = TimeSpan.FromMinutes(8.0);
    private const string c_root = "/Service/HostAcquisition/RegionsCache";
    private const string c_regionsCacheSettingsRootFilter = "/Service/HostAcquisition/RegionsCache/...";
    internal const string c_regionsCacheRefreshTaskExecutionInternalPath = "/Service/HostAcquisition/RegionsCache/RefreshTaskExecutionInternal";
    internal const string c_regionsCacheTimeToLivePath = "/Service/HostAcquisition/RegionsCache/TimeToLive";
    internal const string c_regionsCacheSoonToExpireThresholdPath = "/Service/HostAcquisition/RegionsCache/SoonToExpireThreshold";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "HostAcquisition", nameof (OrganizationAcquisitionService));
    private const string c_area = "HostAcquisition";
    private const string c_layer = "OrganizationAcquisitionService";

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      OrganizationAcquisitionService.ValidateRequestContext(context);
      context.GetService<IVssRegistryService>().RegisterNotification(context, new RegistrySettingsChangedCallback(this.OnRegionsCacheSettingsChanged), "/Service/HostAcquisition/RegionsCache/...");
      this.LoadRegionsCacheSettings(context);
      context.GetService<ITeamFoundationTaskService>().AddTask(context, new TeamFoundationTask(new TeamFoundationTaskCallback(this.RefreshRegionsCacheIfNearExpiry), (object) null, (int) this.m_regionsCacheRefreshTaskExecutionInterval.TotalMilliseconds));
      this.m_regionsCache = new OrganizationAcquisitionService.RegionsCache((IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) null, DateTime.MinValue);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
      context.GetService<ITeamFoundationTaskService>().RemoveTask(context, new TeamFoundationTaskCallback(this.RefreshRegionsCacheIfNearExpiry));
      context.GetService<IVssRegistryService>().UnregisterNotification(context, new RegistrySettingsChangedCallback(this.OnRegionsCacheSettingsChanged));
    }

    [Obsolete("Please use CreateCollection for most scenarios.")]
    public Microsoft.VisualStudio.Services.Organization.Organization CreateOrganization(
      IVssRequestContext context,
      OrganizationCreationContext creationContext)
    {
      OrganizationAcquisitionService.ValidateRequestContext(context);
      ArgumentValidator.ValidateCreateContext(creationContext);
      using (OrganizationAcquisitionService.s_tracer.TraceTimedAction(context, 688006, 5000, nameof (CreateOrganization)))
      {
        try
        {
          Guid onPreferredRegion = this.GetSpsInstanceBasedOnPreferredRegion(context, creationContext.PreferredRegion);
          return OrganizationAcquisitionService.CreateDeploymentClient<OrganizationHttpClient>(context.To(TeamFoundationHostType.Deployment).Elevate(), onPreferredRegion).CreateOrganizationAsync(creationContext.ToClient()).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>().ToServer();
        }
        catch (Microsoft.VisualStudio.Services.Organization.RegionNotAvailableException ex)
        {
          context.Trace(7781025, TraceLevel.Error, "HostAcquisition", nameof (OrganizationAcquisitionService), string.Format("Received RegionNotAvailableException for region: {0}, going to invalidate the regions cache... Exception: {1}", (object) creationContext.PreferredRegion, (object) ex));
          this.InvalidateRegionsCache();
          throw;
        }
      }
    }

    private static void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
    }

    private void LoadRegionsCacheSettings(IVssRequestContext context)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      this.m_regionsCacheRefreshTaskExecutionInterval = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Service/HostAcquisition/RegionsCache/RefreshTaskExecutionInternal", OrganizationAcquisitionService.s_defaultRegionsCacheRefreshTaskExecutionInterval);
      this.m_regionsCacheTimeToLive = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Service/HostAcquisition/RegionsCache/TimeToLive", OrganizationAcquisitionService.s_defaultRegionsCacheTimeToLive);
      this.m_regionsCacheSoonToExpireThreshold = service.GetValue<TimeSpan>(context, (RegistryQuery) "/Service/HostAcquisition/RegionsCache/SoonToExpireThreshold", OrganizationAcquisitionService.s_defaultRegionsCacheSoonToExpireThreshold);
    }

    private void OnRegionsCacheSettingsChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      this.LoadRegionsCacheSettings(context);
      context.GetService<ITeamFoundationTaskService>().AddTask(context, new TeamFoundationTask(new TeamFoundationTaskCallback(this.RefreshRegionsCacheIfNearExpiry), (object) null, (int) this.m_regionsCacheRefreshTaskExecutionInterval.TotalMilliseconds));
      context.TraceAlways(7781004, TraceLevel.Info, "HostAcquisition", nameof (OrganizationAcquisitionService), string.Format("Changed {0} task to run every {1} minutes.", (object) "RefreshRegionsCacheIfNearExpiry", (object) this.m_regionsCacheRefreshTaskExecutionInterval.TotalMinutes));
    }

    private void RefreshRegionsCacheIfNearExpiry(IVssRequestContext context, object taskArgs)
    {
      DateTime lastRefreshed = this.m_regionsCache.LastRefreshed;
      if (DateTime.UtcNow - lastRefreshed > this.m_regionsCacheSoonToExpireThreshold)
      {
        context.Trace(7781007, TraceLevel.Info, "HostAcquisition", nameof (OrganizationAcquisitionService), string.Format("Regions cache was last refreshed at {0} and is about to expire or already expired, refreshing it...", (object) lastRefreshed));
        IList<Microsoft.VisualStudio.Services.Organization.Client.Region> regionsFromSource = OrganizationAcquisitionService.GetRegionsFromSource(context);
        this.SetRegionsCache(context, regionsFromSource);
      }
      else
        context.Trace(7781007, TraceLevel.Info, "HostAcquisition", nameof (OrganizationAcquisitionService), "Regions cache is not neary expiry, do nothing.");
    }

    private static T CreateDeploymentClient<T>(
      IVssRequestContext systemRequestContext,
      Guid spsDeploymentHostId)
      where T : class, IVssHttpClient
    {
      ICreateClient clientProvider = systemRequestContext.ClientProvider as ICreateClient;
      PartitionContainer partitionContainer = systemRequestContext.GetService<IPartitioningService>().GetPartitionContainer(systemRequestContext, spsDeploymentHostId);
      IVssRequestContext requestContext = systemRequestContext;
      Uri baseUri = new Uri(partitionContainer.Address);
      Guid targetServicePrincipal = new Guid();
      return clientProvider.CreateClient<T>(requestContext, baseUri, "OrganizationHelper", (ApiResourceLocationCollection) null, targetServicePrincipal: targetServicePrincipal);
    }

    private Guid GetSpsInstanceBasedOnPreferredRegion(
      IVssRequestContext context,
      string preferredRegion)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Organization.Client.Region> matchingRegions = this.GetRegionsInternal(context).Where<Microsoft.VisualStudio.Services.Organization.Client.Region>((Func<Microsoft.VisualStudio.Services.Organization.Client.Region, bool>) (x => x != null && StringComparer.OrdinalIgnoreCase.Equals(x.Name, preferredRegion)));
      Guid pickedSpsInstanceId = !matchingRegions.IsNullOrEmpty<Microsoft.VisualStudio.Services.Organization.Client.Region>() ? OrganizationAcquisitionService.PickSpsInstanceBasedOnAvailableHostsCount(context, (IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) matchingRegions.ToList<Microsoft.VisualStudio.Services.Organization.Client.Region>()) : throw new RegionNotAvailableException("Region: " + preferredRegion + " is not available right now, please try again later or pick a different region.");
      if (pickedSpsInstanceId == Guid.Empty)
        throw new RegionNotAvailableException("Region: " + preferredRegion + " is not available right now, please try again later or pick a different region.");
      context.TraceConditionally(7781040, TraceLevel.Info, "HostAcquisition", nameof (OrganizationAcquisitionService), (Func<string>) (() => string.Format("{0}: {1}, picked SPS instance: {2}.", (object) preferredRegion, (object) matchingRegions.Serialize<IEnumerable<Microsoft.VisualStudio.Services.Organization.Client.Region>>(), (object) pickedSpsInstanceId)));
      return pickedSpsInstanceId;
    }

    private IList<Microsoft.VisualStudio.Services.Organization.Client.Region> GetRegionsInternal(
      IVssRequestContext context)
    {
      IList<Microsoft.VisualStudio.Services.Organization.Client.Region> regions;
      if (!this.TryGetCachedRegions(context, out regions))
      {
        context.TraceAlways(7781003, TraceLevel.Warning, "HostAcquisition", nameof (OrganizationAcquisitionService), "Regions cache miss.");
        regions = OrganizationAcquisitionService.GetRegionsFromSource(context);
        this.SetRegionsCache(context, regions);
      }
      if (regions.IsNullOrEmpty<Microsoft.VisualStudio.Services.Organization.Client.Region>())
        return (IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) new List<Microsoft.VisualStudio.Services.Organization.Client.Region>();
      if (!context.ExecutionEnvironment.IsDevFabricDeployment)
        regions = (IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) regions.Where<Microsoft.VisualStudio.Services.Organization.Client.Region>((Func<Microsoft.VisualStudio.Services.Organization.Client.Region, bool>) (x => x != null && x.AvailableHostsCount > 0)).ToList<Microsoft.VisualStudio.Services.Organization.Client.Region>();
      return (IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) regions.Where<Microsoft.VisualStudio.Services.Organization.Client.Region>((Func<Microsoft.VisualStudio.Services.Organization.Client.Region, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Organization.Client.Region>();
    }

    private bool TryGetCachedRegions(IVssRequestContext context, out IList<Microsoft.VisualStudio.Services.Organization.Client.Region> regions)
    {
      OrganizationAcquisitionService.RegionsCache regionsCacheSnapshot = this.m_regionsCache.Clone();
      context.TraceConditionally(7781005, TraceLevel.Info, "HostAcquisition", nameof (OrganizationAcquisitionService), (Func<string>) (() => string.Format("Regions cache snapshot: {0}.", (object) regionsCacheSnapshot)));
      if (regionsCacheSnapshot.IsCacheExpired(this.m_regionsCacheTimeToLive))
      {
        regions = (IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) null;
        return false;
      }
      regions = regionsCacheSnapshot.Regions;
      return true;
    }

    private static IList<Microsoft.VisualStudio.Services.Organization.Client.Region> GetRegionsFromSource(
      IVssRequestContext context)
    {
      IVssRequestContext context1 = context.Elevate();
      IEnumerable<Microsoft.VisualStudio.Services.Organization.Client.Region> regions = context1.GetService<IOrganizationCatalogService>().GetRegions(context1, true);
      List<Microsoft.VisualStudio.Services.Organization.Client.Region> latestRegions = regions != null ? regions.ToList<Microsoft.VisualStudio.Services.Organization.Client.Region>() : (List<Microsoft.VisualStudio.Services.Organization.Client.Region>) null;
      context.TraceConditionally(7781001, TraceLevel.Info, "HostAcquisition", nameof (OrganizationAcquisitionService), (Func<string>) (() => "Latest regions: " + latestRegions.Serialize<List<Microsoft.VisualStudio.Services.Organization.Client.Region>>() + "."));
      return (IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) latestRegions;
    }

    private void SetRegionsCache(IVssRequestContext context, IList<Microsoft.VisualStudio.Services.Organization.Client.Region> regionsToCache)
    {
      if (regionsToCache.IsNullOrEmpty<Microsoft.VisualStudio.Services.Organization.Client.Region>())
        context.Trace(7781006, TraceLevel.Error, "HostAcquisition", nameof (OrganizationAcquisitionService), "Trying to cache a null/empty list of regions. Won`t update the cache.");
      else
        this.m_regionsCache = new OrganizationAcquisitionService.RegionsCache(regionsToCache, DateTime.UtcNow);
    }

    private void InvalidateRegionsCache() => this.m_regionsCache.LastRefreshed = DateTime.MinValue;

    internal static Guid PickSpsInstanceBasedOnAvailableHostsCount(
      IVssRequestContext context,
      IList<Microsoft.VisualStudio.Services.Organization.Client.Region> regions)
    {
      int maxValue = regions.Sum<Microsoft.VisualStudio.Services.Organization.Client.Region>((Func<Microsoft.VisualStudio.Services.Organization.Client.Region, int>) (x => x.AvailableHostsCount));
      if (maxValue <= 0)
      {
        context.TraceConditionally(7781041, TraceLevel.Info, "HostAcquisition", nameof (OrganizationAcquisitionService), (Func<string>) (() => "Found no available hosts in regions list: " + regions.Serialize<IList<Microsoft.VisualStudio.Services.Organization.Client.Region>>() + "."));
        return context.ExecutionEnvironment.IsDevFabricDeployment ? regions.ElementAt<Microsoft.VisualStudio.Services.Organization.Client.Region>(OrganizationAcquisitionService.s_random.Next(0, regions.Count)).ServiceInstanceId : Guid.Empty;
      }
      int num1 = OrganizationAcquisitionService.s_random.Next(0, maxValue) + 1;
      int num2 = 0;
      foreach (Microsoft.VisualStudio.Services.Organization.Client.Region region in (IEnumerable<Microsoft.VisualStudio.Services.Organization.Client.Region>) regions)
      {
        num2 += region.AvailableHostsCount;
        if (num1 <= num2)
          return region.ServiceInstanceId;
      }
      return Guid.Empty;
    }

    private class RegionsCache
    {
      public RegionsCache(IList<Microsoft.VisualStudio.Services.Organization.Client.Region> regions, DateTime timestamp)
      {
        this.Regions = regions;
        this.LastRefreshed = timestamp;
      }

      public IList<Microsoft.VisualStudio.Services.Organization.Client.Region> Regions { get; }

      public DateTime LastRefreshed { get; set; }

      public override string ToString()
      {
        IList<Microsoft.VisualStudio.Services.Organization.Client.Region> regions = this.Regions;
        return "Regions: " + (regions != null ? regions.Serialize<IList<Microsoft.VisualStudio.Services.Organization.Client.Region>>() : (string) null) + ", TimeStamp: " + this.LastRefreshed.Serialize<DateTime>();
      }

      public bool IsCacheExpired(TimeSpan timeToLive) => DateTime.UtcNow - this.LastRefreshed > timeToLive;

      public OrganizationAcquisitionService.RegionsCache Clone()
      {
        IList<Microsoft.VisualStudio.Services.Organization.Client.Region> regions = this.Regions;
        return new OrganizationAcquisitionService.RegionsCache(regions != null ? (IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) regions.Select<Microsoft.VisualStudio.Services.Organization.Client.Region, Microsoft.VisualStudio.Services.Organization.Client.Region>((Func<Microsoft.VisualStudio.Services.Organization.Client.Region, Microsoft.VisualStudio.Services.Organization.Client.Region>) (x =>
        {
          if (x == null)
            return (Microsoft.VisualStudio.Services.Organization.Client.Region) null;
          return new Microsoft.VisualStudio.Services.Organization.Client.Region()
          {
            Name = x.Name,
            DisplayName = x.DisplayName,
            NameInAzure = x.NameInAzure,
            IsDefault = x.IsDefault,
            ServiceInstanceId = x.ServiceInstanceId,
            AvailableHostsCount = x.AvailableHostsCount,
            RegionStatus = x.RegionStatus
          };
        })).ToList<Microsoft.VisualStudio.Services.Organization.Client.Region>() : (IList<Microsoft.VisualStudio.Services.Organization.Client.Region>) null, this.LastRefreshed);
      }
    }
  }
}
