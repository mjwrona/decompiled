// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.FrameworkOrganizationCatalogService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Organization
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkOrganizationCatalogService : 
    IOrganizationCatalogService,
    IVssFrameworkService
  {
    private const string AllowGettingOrgsByidFeature = "Microsoft.AzureDevOps.AllowGettingOrgsByid";
    private Guid m_serviceHostId;
    private const string c_area = "Organization";
    private const string c_layer = "FrameworkOrganizationCatalogService";
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(OrganizationPerfCounters.StandardSet, "Organization", nameof (FrameworkOrganizationCatalogService));

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public OrganizationRef CreateOrganization(
      IVssRequestContext context,
      OrganizationCreationContext creationContext)
    {
      this.ValidateRequestContext(context);
      ArgumentValidator.ValidateCreateContext(creationContext);
      using (FrameworkOrganizationCatalogService.s_tracer.TraceTimedAction(context, FrameworkOrganizationCatalogService.TracePoints.CreateOrganization.Slow, 5000, nameof (CreateOrganization)))
        return (OrganizationRef) FrameworkOrganizationCatalogService.s_tracer.TraceAction<Microsoft.VisualStudio.Services.Organization.Organization>(context, (ActionTracePoints) FrameworkOrganizationCatalogService.TracePoints.CreateOrganization, (Func<Microsoft.VisualStudio.Services.Organization.Organization>) (() =>
        {
          Microsoft.VisualStudio.Services.Organization.Client.Organization x = this.GetHttpClient(context).CreateOrganizationAsync(creationContext.ToClient()).SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>();
          FrameworkOrganizationCatalogService.SyncHost(context, x.Id);
          if (!FrameworkOrganizationCatalogService.ShouldSkipCollectionCreation(creationContext))
            FrameworkOrganizationCatalogService.SyncHost(context, FrameworkOrganizationCatalogService.GetPrimaryCollectionId(context, creationContext.PrimaryCollection.Name));
          return x.ToServer();
        }), nameof (CreateOrganization));
    }

    public IEnumerable<OrganizationRef> GetOrganizations(
      IVssRequestContext context,
      OrganizationQueryContext queryContext)
    {
      this.ValidateRequestContext(context);
      ArgumentValidator.ValidateQueryContext(queryContext);
      using (FrameworkOrganizationCatalogService.s_tracer.TraceTimedAction(context, FrameworkOrganizationCatalogService.TracePoints.GetOrganizations.Slow, 5000, nameof (GetOrganizations)))
        return (IEnumerable<OrganizationRef>) FrameworkOrganizationCatalogService.s_tracer.TraceAction<List<OrganizationRef>>(context, (ActionTracePoints) FrameworkOrganizationCatalogService.TracePoints.GetOrganizations, (Func<List<OrganizationRef>>) (() =>
        {
          List<Microsoft.VisualStudio.Services.Organization.Client.Organization> source = new List<Microsoft.VisualStudio.Services.Organization.Client.Organization>();
          if (queryContext.SearchKind != OrganizationSearchKind.ByTenantId && (queryContext.SearchKind != OrganizationSearchKind.ById || !context.IsFeatureEnabled("Microsoft.AzureDevOps.AllowGettingOrgsByid")))
            throw new OrganizationBadRequestException(string.Format("The specified search kind '{0}' is not supported.", (object) queryContext.SearchKind));
          foreach (PartitionContainer spsContainer in (IEnumerable<PartitionContainer>) FrameworkOrganizationCatalogService.GetSpsContainers(context))
          {
            List<Microsoft.VisualStudio.Services.Organization.Client.Organization> collection = FrameworkOrganizationCatalogService.GetHttpClient(context, spsContainer.Address).GetOrganizationsAsync(queryContext.SearchKind, queryContext.SearchValue, queryContext.IsActivated).SyncResult<List<Microsoft.VisualStudio.Services.Organization.Client.Organization>>();
            source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Organization.Client.Organization>) collection);
          }
          return source.Select<Microsoft.VisualStudio.Services.Organization.Client.Organization, OrganizationRef>((Func<Microsoft.VisualStudio.Services.Organization.Client.Organization, OrganizationRef>) (x => x.ToRef())).ToList<OrganizationRef>();
        }), nameof (GetOrganizations));
    }

    public Microsoft.VisualStudio.Services.Organization.Organization GetOldestOrganizationByTenant(
      IVssRequestContext context,
      Guid tenantId)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationCatalogService.s_tracer.TraceTimedAction(context, FrameworkOrganizationCatalogService.TracePoints.GetOrganizations.Slow, 5000, nameof (GetOldestOrganizationByTenant)))
        return FrameworkOrganizationCatalogService.s_tracer.TraceAction<Microsoft.VisualStudio.Services.Organization.Organization>(context, (ActionTracePoints) FrameworkOrganizationCatalogService.TracePoints.GetOrganizations, (Func<Microsoft.VisualStudio.Services.Organization.Organization>) (() =>
        {
          Microsoft.VisualStudio.Services.Organization.Organization organizationByTenant = (Microsoft.VisualStudio.Services.Organization.Organization) null;
          foreach (PartitionContainer spsContainer in (IEnumerable<PartitionContainer>) FrameworkOrganizationCatalogService.GetSpsContainers(context))
          {
            Task<Microsoft.VisualStudio.Services.Organization.Client.Organization> organizationByTenantAsync = FrameworkOrganizationCatalogService.GetHttpClient(context, spsContainer.Address).GetOldestOrganizationByTenantAsync(tenantId);
            Microsoft.VisualStudio.Services.Organization.Organization organization1;
            if (organizationByTenantAsync == null)
            {
              organization1 = (Microsoft.VisualStudio.Services.Organization.Organization) null;
            }
            else
            {
              Microsoft.VisualStudio.Services.Organization.Client.Organization x = organizationByTenantAsync.SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>();
              organization1 = x != null ? x.ToServer() : (Microsoft.VisualStudio.Services.Organization.Organization) null;
            }
            Microsoft.VisualStudio.Services.Organization.Organization organization2 = organization1;
            if (organization2 != null)
            {
              DateTime dateCreated1 = organization2.DateCreated;
              if (organizationByTenant != null)
              {
                DateTime dateCreated2 = organizationByTenant.DateCreated;
                if (!(organizationByTenant.DateCreated > organization2.DateCreated))
                  continue;
              }
              organizationByTenant = organization2;
            }
          }
          return organizationByTenant;
        }), nameof (GetOldestOrganizationByTenant));
    }

    public IEnumerable<CollectionRef> GetCollections(
      IVssRequestContext context,
      CollectionQueryContext queryContext)
    {
      this.ValidateRequestContext(context);
      ArgumentValidator.ValidateQueryContext(queryContext);
      using (FrameworkOrganizationCatalogService.s_tracer.TraceTimedAction(context, FrameworkOrganizationCatalogService.TracePoints.GetCollections.Slow, 5000, nameof (GetCollections)))
        return (IEnumerable<CollectionRef>) FrameworkOrganizationCatalogService.s_tracer.TraceAction<List<CollectionRef>>(context, (ActionTracePoints) FrameworkOrganizationCatalogService.TracePoints.GetCollections, (Func<List<CollectionRef>>) (() =>
        {
          List<Microsoft.VisualStudio.Services.Organization.Client.Collection> source = new List<Microsoft.VisualStudio.Services.Organization.Client.Collection>();
          if (queryContext.SearchKind != CollectionSearchKind.ByTenantId)
            throw new OrganizationBadRequestException(string.Format("The specified search kind '{0}' is not supported.", (object) queryContext.SearchKind));
          foreach (PartitionContainer spsContainer1 in (IEnumerable<PartitionContainer>) FrameworkOrganizationCatalogService.GetSpsContainers(context))
          {
            PartitionContainer spsContainer = spsContainer1;
            GetCollectionsCircuitBreakerSettings circuitBreakerSettings = new GetCollectionsCircuitBreakerSettings(spsContainer.Address);
            IList<Microsoft.VisualStudio.Services.Organization.Client.Collection> collectionList = new CommandService<IList<Microsoft.VisualStudio.Services.Organization.Client.Collection>>(context, CommandSetter.WithGroupKey((CommandGroupKey) circuitBreakerSettings.CommandGroupKey).AndCommandKey((CommandKey) circuitBreakerSettings.CommandKeyForGetCollections).AndCommandPropertiesDefaults(circuitBreakerSettings.CircuitBreakerSettingsForGetCollections), (Func<IList<Microsoft.VisualStudio.Services.Organization.Client.Collection>>) (() => FrameworkOrganizationCatalogService.GetCollectionsFromSpsInstance(context, spsContainer.Address, queryContext)), (Func<IList<Microsoft.VisualStudio.Services.Organization.Client.Collection>>) (() =>
            {
              context.Trace(7620042, TraceLevel.Error, "Organization", nameof (FrameworkOrganizationCatalogService), "Failed to get collections from SPS instance: " + spsContainer.Serialize<PartitionContainer>() + ".");
              return (IList<Microsoft.VisualStudio.Services.Organization.Client.Collection>) null;
            })).Execute();
            if (!collectionList.IsNullOrEmpty<Microsoft.VisualStudio.Services.Organization.Client.Collection>())
              source.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Organization.Client.Collection>) collectionList);
          }
          return source.Select<Microsoft.VisualStudio.Services.Organization.Client.Collection, CollectionRef>((Func<Microsoft.VisualStudio.Services.Organization.Client.Collection, CollectionRef>) (x => x.ToRef())).ToList<CollectionRef>();
        }), nameof (GetCollections));
    }

    public IEnumerable<Region> GetRegions(
      IVssRequestContext context,
      bool includeRegionsWithNoAvailableHosts = false)
    {
      this.ValidateRequestContext(context);
      using (FrameworkOrganizationCatalogService.s_tracer.TraceTimedAction(context, FrameworkOrganizationCatalogService.TracePoints.GetRegions.Slow, 500, nameof (GetRegions)))
        return (IEnumerable<Region>) FrameworkOrganizationCatalogService.s_tracer.TraceAction<List<Region>>(context, (ActionTracePoints) FrameworkOrganizationCatalogService.TracePoints.GetRegions, (Func<List<Region>>) (() =>
        {
          List<Region> regions = new List<Region>();
          foreach (PartitionContainer spsContainer1 in (IEnumerable<PartitionContainer>) FrameworkOrganizationCatalogService.GetSpsContainers(context))
          {
            PartitionContainer spsContainer = spsContainer1;
            PartitionContainer partitionContainer1 = spsContainer;
            bool? nullable1;
            bool? nullable2;
            if (partitionContainer1 == null)
            {
              nullable1 = new bool?();
              nullable2 = nullable1;
            }
            else
              nullable2 = new bool?(partitionContainer1.Address.IsNullOrEmpty<char>());
            nullable1 = nullable2;
            if (nullable1.Value)
            {
              IVssRequestContext requestContext = context;
              PartitionContainer partitionContainer2 = spsContainer;
              string message = "Found invalid SPS container: " + (partitionContainer2 != null ? partitionContainer2.Serialize<PartitionContainer>() : (string) null) + ".";
              requestContext.Trace(7620031, TraceLevel.Error, "Organization", nameof (FrameworkOrganizationCatalogService), message);
            }
            else
            {
              GetRegionsCircuitBreakerSettings circuitBreakerSettings = new GetRegionsCircuitBreakerSettings(spsContainer.Address);
              IList<Region> regionList = new CommandService<IList<Region>>(context, CommandSetter.WithGroupKey((CommandGroupKey) circuitBreakerSettings.CommandGroupKey).AndCommandKey((CommandKey) circuitBreakerSettings.CommandKeyForGetRegions).AndCommandPropertiesDefaults(circuitBreakerSettings.CircuitBreakerSettingsForGetRegions), (Func<IList<Region>>) (() => FrameworkOrganizationCatalogService.GetRegionsFromSpsInstance(context, spsContainer.Address, includeRegionsWithNoAvailableHosts)), (Func<IList<Region>>) (() =>
              {
                context.Trace(7620032, TraceLevel.Error, "Organization", nameof (FrameworkOrganizationCatalogService), "Failed to get regions from SPS instance: " + spsContainer.Serialize<PartitionContainer>() + ".");
                return (IList<Region>) null;
              })).Execute();
              if (!regionList.IsNullOrEmpty<Region>())
                regions.AddRange((IEnumerable<Region>) regionList);
            }
          }
          return regions;
        }), nameof (GetRegions));
    }

    public IEnumerable<Geography> GetGeographies(IVssRequestContext context)
    {
      this.ValidateRequestContext(context);
      return (IEnumerable<Geography>) FrameworkOrganizationCatalogService.s_tracer.TraceAction<List<Geography>>(context, (ActionTracePoints) FrameworkOrganizationCatalogService.TracePoints.GetGeographies, (Func<List<Geography>>) (() =>
      {
        List<Geography> geographies = new List<Geography>();
        foreach (PartitionContainer spsContainer1 in (IEnumerable<PartitionContainer>) FrameworkOrganizationCatalogService.GetSpsContainers(context))
        {
          PartitionContainer spsContainer = spsContainer1;
          if (spsContainer == null || spsContainer.Address.IsNullOrEmpty<char>())
          {
            IVssRequestContext requestContext = context;
            PartitionContainer partitionContainer = spsContainer;
            string message = "Found invalid SPS container: " + (partitionContainer != null ? partitionContainer.Serialize<PartitionContainer>() : (string) null) + ".";
            requestContext.Trace(7620031, TraceLevel.Error, "Organization", nameof (FrameworkOrganizationCatalogService), message);
          }
          else
          {
            GetGeographiesCircuitBreakerSettings circuitBreakerSettings = new GetGeographiesCircuitBreakerSettings(spsContainer.Address);
            IList<Geography> geographyList = new CommandService<IList<Geography>>(context, CommandSetter.WithGroupKey((CommandGroupKey) circuitBreakerSettings.CommandGroupKey).AndCommandKey((CommandKey) circuitBreakerSettings.CommandKeyForGetGeographies).AndCommandPropertiesDefaults(circuitBreakerSettings.CircuitBreakerSettingsForGetGeographies), (Func<IList<Geography>>) (() => FrameworkOrganizationCatalogService.GetGeographiesFromSpsInstance(context, spsContainer.Address)), (Func<IList<Geography>>) (() =>
            {
              context.Trace(7620032, TraceLevel.Error, "Organization", nameof (FrameworkOrganizationCatalogService), "Failed to get geographies from SPS instance: " + spsContainer.Serialize<PartitionContainer>() + ".");
              return (IList<Geography>) null;
            })).Execute();
            if (!geographyList.IsNullOrEmpty<Geography>())
              geographies.AddRange((IEnumerable<Geography>) geographyList);
          }
        }
        return geographies;
      }), nameof (GetGeographies));
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private OrganizationHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<OrganizationHttpClient>();

    private static OrganizationHttpClient GetHttpClient(
      IVssRequestContext requestContext,
      string baseUri)
    {
      return (requestContext.ClientProvider as ICreateClient).CreateClient<OrganizationHttpClient>(requestContext, new Uri(baseUri), "Organization", (ApiResourceLocationCollection) null);
    }

    private static void SyncHost(IVssRequestContext context, Guid hostId) => context.GetService<IHostSyncService>().EnsureHostUpdated(context, hostId);

    private static Guid GetPrimaryCollectionId(
      IVssRequestContext context,
      string primaryCollectionName)
    {
      Guid collectionId;
      if (!HostNameResolver.TryGetCollectionServiceHostId(context, primaryCollectionName, out collectionId))
        throw new OrganizationException("Failed to find primary collection named " + primaryCollectionName + " in remote store.");
      return collectionId;
    }

    private static bool ShouldSkipCollectionCreation(OrganizationCreationContext creationContext)
    {
      object obj;
      return creationContext.Data.TryGetValue("WithNoCollections", out obj) && obj is bool flag && flag;
    }

    private static IList<PartitionContainer> GetSpsContainers(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.Elevate();
      return vssRequestContext.GetService<IPartitioningService>().QueryPartitionContainers(vssRequestContext, ServiceInstanceTypes.SPS);
    }

    private static IList<Region> GetRegionsFromSpsInstance(
      IVssRequestContext context,
      string spsInstanceUri,
      bool includeRegionsWithNoAvailableHosts = false)
    {
      return (IList<Region>) FrameworkOrganizationCatalogService.GetHttpClient(context, spsInstanceUri).GetRegionsAsync(new bool?(includeRegionsWithNoAvailableHosts)).SyncResult<List<Region>>();
    }

    private static IList<Geography> GetGeographiesFromSpsInstance(
      IVssRequestContext context,
      string spsInstanceUri)
    {
      return (IList<Geography>) FrameworkOrganizationCatalogService.GetHttpClient(context, spsInstanceUri).GetGeographiesAsync().SyncResult<List<Geography>>();
    }

    private static IList<Microsoft.VisualStudio.Services.Organization.Client.Collection> GetCollectionsFromSpsInstance(
      IVssRequestContext context,
      string spsInstanceUri,
      CollectionQueryContext queryContext)
    {
      return (IList<Microsoft.VisualStudio.Services.Organization.Client.Collection>) FrameworkOrganizationCatalogService.GetHttpClient(context, spsInstanceUri).GetCollectionsAsync(queryContext.SearchKind, queryContext.SearchValue, queryContext.IncludeDeletedCollections).SyncResult<List<Microsoft.VisualStudio.Services.Organization.Client.Collection>>();
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints CreateOrganization = new TimedActionTracePoints(7620010, 7620017, 7620018, 7620019);
      internal static readonly TimedActionTracePoints GetOrganizations = new TimedActionTracePoints(7620020, 7620027, 7620028, 7620029);
      internal static readonly TimedActionTracePoints GetRegions = new TimedActionTracePoints(7620030, 7620037, 7620038, 7620039);
      internal static readonly TimedActionTracePoints GetCollections = new TimedActionTracePoints(7620040, 7620047, 7620048, 7620049);
      internal static readonly TimedActionTracePoints GetGeographies = new TimedActionTracePoints(7620050, 7620057, 7620058, 7620059);
    }
  }
}
