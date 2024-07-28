// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocalLocationDataProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class LocalLocationDataProvider : LocationDataProvider
  {
    protected Guid m_hostId;
    protected Guid m_instanceType;
    private static readonly string s_area = "LocationService";
    private static readonly string s_layer = nameof (LocalLocationDataProvider);
    private static readonly SemaphoreSlim s_loadSemaphore = new SemaphoreSlim(50);

    internal LocalLocationDataProvider()
    {
    }

    public LocalLocationDataProvider(
      IVssRequestContext requestContext,
      ILocationDataCache<string> locationCache)
      : base(requestContext, locationCache)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_instanceType = vssRequestContext.GetService<IVssRegistryService>().GetValue<Guid>(vssRequestContext, (RegistryQuery) ConfigurationConstants.InstanceType, false, new Guid());
      this.m_hostId = requestContext.ServiceHost.InstanceId;
    }

    public override Guid HostId => this.m_hostId;

    public override Guid InstanceType => this.m_instanceType;

    protected override string CacheKey => string.Empty;

    protected override Guid DeploymentId => Guid.Empty;

    public override void SaveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitionsIncoming)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForNull<IEnumerable<ServiceDefinition>>(serviceDefinitionsIncoming, "serviceDefinitions");
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      List<ServiceDefinition> serviceDefinitions = new List<ServiceDefinition>(serviceDefinitionsIncoming);
      if (!serviceDefinitions.Any<ServiceDefinition>())
        return;
      service.PublishDecisionPoint(requestContext, (object) serviceDefinitions);
      foreach (ServiceDefinition definition in serviceDefinitions)
        this.ValidateServiceDefinitionToSave(requestContext, definition);
      LocalLocationDataProvider.TraceDefinitions(requestContext, (IEnumerable<ServiceDefinition>) serviceDefinitions);
      bool definitionsChanged = false;
      this.LocationCache.Update(requestContext, this.CacheKey, (Action) (() =>
      {
        using (LocationComponent component = requestContext.CreateComponent<LocationComponent>())
          definitionsChanged = component.SaveServiceDefinitions((IEnumerable<ServiceDefinition>) serviceDefinitions);
        definitionsChanged |= LocationPropertyHelper.UpdateServiceDefinitionProperties(requestContext, (IEnumerable<ServiceDefinition>) serviceDefinitions);
      }));
      if (!definitionsChanged)
        return;
      try
      {
        service.SyncPublishNotification(requestContext, (object) serviceDefinitions);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(279839817, LocalLocationDataProvider.s_area, LocalLocationDataProvider.s_layer, ex);
      }
    }

    public override void RemoveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      this.RemoveServiceDefinitions(requestContext, serviceDefinitions, false);
    }

    public void RemoveServiceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions,
      bool requireFullMatch)
    {
      this.ValidateRequestContext(requestContext);
      this.LocationCache.Update(requestContext, this.CacheKey, (Action) (() =>
      {
        using (LocationComponent component = requestContext.CreateComponent<LocationComponent>())
          component.RemoveServiceDefinitions(serviceDefinitions, requireFullMatch);
      }));
    }

    public override ServiceDefinition FindServiceDefinitionWithFaultIn(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier,
      bool previewFaultIn)
    {
      ServiceDefinition serviceDefinition = this.FindServiceDefinition(requestContext, serviceType, identifier);
      if (serviceDefinition == null && this.CanFaultInDefinition(requestContext, serviceType, identifier))
      {
        Guid identifierForFaultIn = this.GetIdentifierForFaultIn(requestContext, serviceType, identifier);
        using (requestContext.AllowAnonymousOrPublicUserWrites())
          serviceDefinition = this.TryFaultInDefinition(requestContext, identifierForFaultIn, previewFaultIn);
        if (serviceDefinition != null)
          serviceDefinition = this.PostProcessAndCloneServiceDefinition(requestContext, serviceDefinition, this.GetLocationData(requestContext).WebAppRelativeDirectory);
      }
      if (serviceDefinition == null)
        requestContext.TraceAlways(72008, TraceLevel.Info, LocalLocationDataProvider.s_area, LocalLocationDataProvider.s_layer, "Cannot find service definition for identifier {0}", (object) identifier);
      return serviceDefinition;
    }

    protected Guid GetIdentifierForFaultIn(
      IVssRequestContext requestContext,
      string serviceType,
      Guid identifier)
    {
      ServiceDefinition serviceDefinition = this.GetInheritedLocationData(requestContext).FindServiceDefinition(serviceType, identifier);
      Guid identifierForFaultIn = identifier;
      if (serviceDefinition != null && this.CanFaultInDefinition(requestContext, serviceDefinition.ParentServiceType, serviceDefinition.ParentIdentifier))
        identifierForFaultIn = serviceDefinition.ParentIdentifier;
      return identifierForFaultIn;
    }

    private ServiceDefinition TryFaultInDefinition(
      IVssRequestContext requestContext,
      Guid identifier,
      bool previewFaultIn)
    {
      try
      {
        ServiceDefinition instanceAllocation = InstanceAllocationHelper.ComputeInstanceAllocation(requestContext, identifier);
        if (instanceAllocation != null)
        {
          if (!previewFaultIn)
            this.SaveServiceDefinitions(requestContext, (IEnumerable<ServiceDefinition>) new ServiceDefinition[1]
            {
              instanceAllocation
            });
          return instanceAllocation;
        }
      }
      catch (CannotChangeParentDefinitionException ex)
      {
      }
      return (ServiceDefinition) null;
    }

    public override AccessMapping ConfigureAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping,
      bool makeDefault,
      bool allowOverlapping)
    {
      this.ValidateRequestContext(requestContext);
      if (!string.IsNullOrEmpty(accessMapping.AccessPoint))
      {
        try
        {
          Uri uri = new Uri(accessMapping.AccessPoint);
        }
        catch (UriFormatException ex)
        {
          throw new InvalidAccessPointException(FrameworkResources.AccessPointMustBeAValidURI((object) accessMapping.AccessPoint));
        }
        string pattern = !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.ApplicationLocationServiceRelativePath : TFCommonUtil.CombinePaths(this.GetWebApplicationRelativeDirectory(requestContext), Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.CollectionLocationServiceRelativePath);
        if (VssStringComparer.Url.EndsWith(accessMapping.AccessPoint, pattern))
          accessMapping.AccessPoint = accessMapping.AccessPoint.Substring(0, accessMapping.AccessPoint.Length - pattern.Length);
      }
      this.LocationCache.Update(requestContext, this.CacheKey, (Action) (() =>
      {
        using (LocationComponent component = requestContext.CreateComponent<LocationComponent>())
          component.ConfigureAccessMapping(accessMapping, makeDefault, allowOverlapping);
      }));
      return accessMapping;
    }

    public override void SetDefaultAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      this.ValidateRequestContext(requestContext);
      AccessMapping configuredAccessMapping;
      this.GetLocationData(requestContext).AccessMappings.TryGetValue(accessMapping.Moniker, out configuredAccessMapping);
      if (configuredAccessMapping == null)
        throw new AccessMappingNotRegisteredException(accessMapping.Moniker);
      this.LocationCache.Update(requestContext, this.CacheKey, (Action) (() =>
      {
        using (LocationComponent component = requestContext.CreateComponent<LocationComponent>())
          component.ConfigureAccessMapping(configuredAccessMapping, true, true);
      }));
    }

    public override AccessMapping DetermineAccessMapping(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      if (requestContext.Items.ContainsKey(RequestContextItemsKeys.ClientAccessMapping))
      {
        AccessMapping accessMapping;
        requestContext.TryGetItem<AccessMapping>(RequestContextItemsKeys.ClientAccessMapping, out accessMapping);
        return LocationDataProvider.Clone(accessMapping);
      }
      AccessMapping accessMappingInternal;
      try
      {
        requestContext.Items[RequestContextItemsKeys.ClientAccessMapping] = (object) null;
        accessMappingInternal = this.DetermineAccessMappingInternal(requestContext);
      }
      finally
      {
        requestContext.Items.Remove(RequestContextItemsKeys.ClientAccessMapping);
      }
      requestContext.Items[RequestContextItemsKeys.ClientAccessMapping] = (object) accessMappingInternal;
      return LocationDataProvider.Clone(accessMappingInternal);
    }

    private AccessMapping DetermineAccessMappingInternal(IVssRequestContext requestContext)
    {
      AccessMapping accessMappingInternal = (AccessMapping) null;
      LocationData locationData = this.GetLocationData(requestContext);
      Uri uri = requestContext.RequestUri();
      if (uri == (Uri) null)
        accessMappingInternal = LocationDataProvider.Clone(locationData.DefaultAccessMapping);
      else if (requestContext.ServiceHost.InstanceId == requestContext.RootContext.ServiceHost.InstanceId)
      {
        string str1 = (string) null;
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Location.IgnoreExternalClientAccessMapping"))
        {
          if (requestContext.RootContext.Items.TryGetValue<string>(RequestContextItemsKeys.ClientAccessMappingMonikers, out str1))
          {
            string str2 = str1;
            char[] chArray = new char[1]{ ';' };
            foreach (string key in str2.Split(chArray))
            {
              AccessMapping accessMapping;
              if (locationData.AccessMappings.TryGetValue(key, out accessMapping))
              {
                accessMappingInternal = accessMapping.Clone();
                break;
              }
            }
          }
          if (accessMappingInternal == null)
            requestContext.Trace(72007, TraceLevel.Info, LocalLocationDataProvider.s_area, LocalLocationDataProvider.s_layer, "Cannot determine client mapping from {0}", (object) str1);
        }
        if (accessMappingInternal == null)
        {
          string virtualDirectory = LocationServiceHelper.ComputeVirtualDirectory(LocationServiceHelper.ComputeWebApplicationRelativeDirectory(requestContext));
          foreach (AccessMapping accessMapping in (IEnumerable<AccessMapping>) locationData.AccessMappings.Values.OrderBy<AccessMapping, int>((Func<AccessMapping, int>) (x => x.Moniker == locationData.DefaultAccessMapping.Moniker ? 1 : 0)))
          {
            if (VssStringComparer.Hostname.Equals(new Uri(accessMapping.AccessPoint).Host, uri.Host) && VssStringComparer.ServerUrl.Equals(accessMapping.VirtualDirectory, virtualDirectory))
            {
              accessMappingInternal = accessMapping.Clone();
              break;
            }
          }
          if (accessMappingInternal == null)
          {
            string absoluteUri = new UriBuilder(uri)
            {
              Path = (string.IsNullOrEmpty(virtualDirectory) ? requestContext.VirtualPath() : UrlHostResolutionService.ApplicationVirtualPath),
              Query = ((string) null),
              Fragment = ((string) null)
            }.Uri.AbsoluteUri;
            accessMappingInternal = new AccessMapping(absoluteUri, absoluteUri, absoluteUri, Guid.Empty, virtualDirectory);
          }
        }
      }
      else
      {
        IVssRequestContext rootContext = requestContext.RootContext;
        AccessMapping accessMapping1 = rootContext.GetService<ILocationService>().DetermineAccessMapping(rootContext);
        AccessMapping accessMapping2;
        if (accessMapping1 != null && locationData.AccessMappings.TryGetValue(accessMapping1.Moniker, out accessMapping2))
          accessMappingInternal = accessMapping2.Clone();
        if (accessMappingInternal == null && requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          string absoluteUri = new UriBuilder(uri)
          {
            Path = UrlHostResolutionService.ApplicationVirtualPath,
            Query = ((string) null),
            Fragment = ((string) null)
          }.Uri.AbsoluteUri;
          accessMappingInternal = new AccessMapping(absoluteUri, absoluteUri, absoluteUri, Guid.Empty, string.Empty);
        }
        if (accessMappingInternal == null)
          accessMappingInternal = LocationDataProvider.Clone(locationData.DefaultAccessMapping);
      }
      return accessMappingInternal;
    }

    public override void RemoveAccessMapping(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      this.ValidateRequestContext(requestContext);
      this.LocationCache.Update(requestContext, this.CacheKey, (Action) (() =>
      {
        using (LocationComponent component = requestContext.CreateComponent<LocationComponent>())
          component.RemoveAccessMappings((IEnumerable<AccessMapping>) new AccessMapping[1]
          {
            accessMapping
          });
      }));
    }

    internal static void LoadLocationDataFromDatabase(
      IVssRequestContext requestContext,
      out List<ServiceDefinition> serviceDefinitions,
      out List<AccessMapping> accessMappings,
      out List<LocationMappingData> locationMappings,
      out string defaultAccessMappingMoniker,
      out long databaseLastChangeId)
    {
      databaseLastChangeId = -1L;
      using (LocationComponent component = requestContext.CreateComponent<LocationComponent>())
      {
        ResultCollection resultCollection = component.QueryServiceData();
        serviceDefinitions = resultCollection.GetCurrent<ServiceDefinition>().Items;
        resultCollection.NextResult();
        locationMappings = resultCollection.GetCurrent<LocationMappingData>().Items;
        resultCollection.NextResult();
        accessMappings = resultCollection.GetCurrent<AccessMapping>().Items;
        resultCollection.NextResult();
        defaultAccessMappingMoniker = resultCollection.GetCurrent<string>().Items.FirstOrDefault<string>();
        resultCollection.NextResult();
        ObjectBinder<long> current = resultCollection.GetCurrent<long>();
        if (current.Items.Count <= 0)
          return;
        databaseLastChangeId = current.Items[0];
      }
    }

    protected internal override LocationData FetchLocationData(IVssRequestContext requestContext)
    {
      List<ServiceDefinition> serviceDefinitions;
      List<AccessMapping> accessMappings1;
      List<LocationMappingData> locationMappings;
      string defaultAccessMappingMoniker;
      long databaseLastChangeId;
      LocalLocationDataProvider.LoadLocationDataFromDatabase(requestContext, out serviceDefinitions, out accessMappings1, out locationMappings, out defaultAccessMappingMoniker, out databaseLastChangeId);
      List<ServiceDefinition> list1 = serviceDefinitions.Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (x => x.InheritLevel == InheritLevel.None)).ToList<ServiceDefinition>();
      List<LocationMappingData> list2 = locationMappings.Where<LocationMappingData>((Func<LocationMappingData, bool>) (x => !x.Location.StartsWith("~/"))).ToList<LocationMappingData>();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        LocationPropertyHelper.FetchServiceDefinitionProperties(requestContext, list1, (IEnumerable<string>) new List<string>()
        {
          "*"
        });
      string webAppRelativeDirectory = (string) null;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IHostUriData uriData = vssRequestContext.GetService<IInternalUrlHostResolutionService>().ResolveUriData(vssRequestContext, requestContext.ServiceHost.InstanceId);
        if (uriData != null)
          webAppRelativeDirectory = uriData.ComputeWebApplicationRelativeDirectory();
      }
      AccessMapping defaultAccessMapping1;
      AccessMapping publicAccessMapping1;
      AccessMapping serverAccessMapping1;
      Dictionary<string, AccessMapping> accessMappings2 = this.GetAccessMappings(requestContext, (IEnumerable<AccessMapping>) accessMappings1, defaultAccessMappingMoniker, webAppRelativeDirectory, Guid.Empty, out defaultAccessMapping1, out publicAccessMapping1, out serverAccessMapping1);
      LocationServiceHelper.SetLocationMappings(list1, list2);
      AccessMapping defaultAccessMapping2 = defaultAccessMapping1;
      AccessMapping publicAccessMapping2 = publicAccessMapping1;
      AccessMapping serverAccessMapping2 = serverAccessMapping1;
      List<ServiceDefinition> serviceDefinitionList = list1;
      string webApplicationRelativeDirectory = webAppRelativeDirectory;
      DateTime maxValue = DateTime.MaxValue;
      long lastChangeId = databaseLastChangeId;
      Guid hostId = this.m_hostId;
      Guid instanceType = this.m_instanceType;
      Guid instanceId = requestContext.ServiceHost.DeploymentServiceHost.InstanceId;
      return new LocationData(accessMappings2, defaultAccessMapping2, publicAccessMapping2, serverAccessMapping2, (IEnumerable<ServiceDefinition>) serviceDefinitionList, webApplicationRelativeDirectory, maxValue, lastChangeId, hostId, instanceType, instanceId);
    }

    protected internal override LocationData GetInheritedLocationData(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<InheritedLocationDataService>().GetData(vssRequestContext, Guid.Empty, TeamFoundationHostTypeHelper.NormalizeHostType(requestContext.ServiceHost.HostType));
    }

    protected override LocationData PostprocessLocationData(
      IVssRequestContext requestContext,
      LocationData locationData)
    {
      Dictionary<string, AccessMapping> accessMappings = new Dictionary<string, AccessMapping>((IDictionary<string, AccessMapping>) locationData.AccessMappings);
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      using (this.AcquireSqlSemaphore(requestContext))
      {
        foreach (IAccessMappingProvider extension in (IEnumerable<IAccessMappingProvider>) vssRequestContext1.GetExtensions<IAccessMappingProvider>(ExtensionLifetime.Service))
          extension.AddAccessMappings(requestContext, accessMappings, locationData.WebAppRelativeDirectory);
      }
      AccessMapping defaultAccessMapping = locationData.DefaultAccessMapping;
      AccessMapping publicAccessMapping;
      if (!accessMappings.TryGetValue(AccessMappingConstants.PublicAccessMappingMoniker, out publicAccessMapping))
        publicAccessMapping = defaultAccessMapping;
      AccessMapping serverAccessMapping;
      if (!accessMappings.TryGetValue(AccessMappingConstants.ServerAccessMappingMoniker, out serverAccessMapping))
        serverAccessMapping = publicAccessMapping;
      if (defaultAccessMapping == null)
        defaultAccessMapping = publicAccessMapping;
      TeamFoundationExecutionEnvironment executionEnvironment;
      if (this.InstanceType != ServiceInstanceTypes.SPS)
      {
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (executionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          ServiceDefinition serviceDefinition = locationData.FindServiceDefinition("LocationService2", ServiceInstanceTypes.SPS);
          if (serviceDefinition != null)
            LocationServiceHelper.EnsureSpsInstanceServiceDefinitionExists(vssRequestContext1, serviceDefinition.ParentIdentifier, (string) null);
          requestContext.Items[RequestContextItemsKeys.SpsDefinition] = (object) serviceDefinition;
        }
      }
      LocationData processedLocationData = new LocationData(accessMappings, defaultAccessMapping, publicAccessMapping, serverAccessMapping, (IEnumerable<ServiceDefinition>) null, locationData.WebAppRelativeDirectory, locationData.CacheExpirationDate, locationData.LastChangeId, locationData.HostId, locationData.InstanceType, locationData.DeploymentId);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (executionEnvironment.IsHostedDeployment)
        {
          HashSet<ServiceDefinition> visitedDefinitions = new HashSet<ServiceDefinition>();
          using (IEnumerator<ServiceDefinition> enumerator = locationData.GetAllServiceDefinitions().GetEnumerator())
          {
            while (enumerator.MoveNext())
              LocalLocationDataProvider.ProcessDefinitionInDependencyOrder(enumerator.Current, locationData, visitedDefinitions, (Action<ServiceDefinition>) (sd => processedLocationData.AddServiceDefinition(this.PostProcessAndCloneServiceDefinition(requestContext, sd, locationData.WebAppRelativeDirectory, (Func<string, Guid, ServiceDefinition>) ((st, id) => processedLocationData.FindServiceDefinition(st, id))))));
            goto label_32;
          }
        }
      }
      processedLocationData.AddServiceDefinitions((IEnumerable<ServiceDefinition>) this.PostProcessAndCloneServiceDefinitions(requestContext, locationData.GetAllServiceDefinitions(), locationData.WebAppRelativeDirectory, (Func<string, Guid, ServiceDefinition>) ((st, id) => (ServiceDefinition) null)));
label_32:
      executionEnvironment = requestContext.ExecutionEnvironment;
      if ((executionEnvironment.IsHostedDeployment ? (requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application) ? 1 : 0) : (requestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? 1 : 0)) != 0)
      {
        foreach (TeamFoundationServiceHostProperties child in vssRequestContext1.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext1, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren).Children)
        {
          executionEnvironment = requestContext.ExecutionEnvironment;
          AccessMapping accessMapping;
          if (executionEnvironment.IsHostedDeployment)
          {
            accessMapping = LocationServiceHelper.GetHostPublicAccessMapping(requestContext, child.Id);
            if (accessMapping == null)
            {
              accessMapping = LocationServiceHelper.GetHostGuidAccessMapping(requestContext, child.Id);
              accessMapping.Moniker = AccessMappingConstants.PublicAccessMappingMoniker;
              accessMapping.DisplayName = TFCommonResources.PublicAccessMappingDisplayName();
            }
          }
          else
          {
            accessMapping = publicAccessMapping.Clone();
            accessMapping.VirtualDirectory = child.Name;
          }
          ServiceDefinition serviceDefinition1 = new ServiceDefinition("LocationService2", child.Id, TFCommonResources.LocationService(), TFCommonUtil.CombinePaths("/" + accessMapping.VirtualDirectory, "/"), Microsoft.VisualStudio.Services.Location.RelativeToSetting.WebApplication, TFCommonResources.FrameworkLocationServiceDescription(), "Framework");
          executionEnvironment = requestContext.ExecutionEnvironment;
          if (executionEnvironment.IsHostedDeployment)
          {
            serviceDefinition1.RelativePath = (string) null;
            serviceDefinition1.RelativeToSetting = Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified;
            serviceDefinition1.LocationMappings.Add(accessMapping.ToLocationMapping());
            serviceDefinition1.LocationMappings.Add(LocationServiceHelper.GetHostGuidLocationMapping(requestContext, child.Id));
          }
          serviceDefinition1.SetProperty("Microsoft.TeamFoundation.Location.CollectionName", (object) child.Name);
          processedLocationData.AddServiceDefinition(serviceDefinition1);
          if (this.InstanceType == Guid.Empty || this.InstanceType == ServiceInstanceTypes.TFS)
          {
            ServiceDefinition serviceDefinition2 = new ServiceDefinition("LocationService", child.Id, TFCommonResources.LocationService(), TFCommonUtil.CombinePaths("/" + accessMapping.VirtualDirectory, Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.CollectionLocationServiceRelativePath), Microsoft.VisualStudio.Services.Location.RelativeToSetting.WebApplication, TFCommonResources.FrameworkLocationServiceDescription(), "Framework");
            executionEnvironment = requestContext.ExecutionEnvironment;
            if (executionEnvironment.IsHostedDeployment)
            {
              serviceDefinition2.RelativePath = (string) null;
              serviceDefinition2.RelativeToSetting = Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified;
              LocationMapping locationMapping = accessMapping.ToLocationMapping();
              locationMapping.Location += Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.CollectionLocationServiceRelativePath;
              serviceDefinition2.LocationMappings.Add(locationMapping);
            }
            processedLocationData.AddServiceDefinition(serviceDefinition2);
          }
        }
      }
      else if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        ServiceDefinition serviceDefinition3 = new ServiceDefinition("LocationService2", Microsoft.VisualStudio.Services.Location.LocationServiceConstants.ApplicationIdentifier, TFCommonResources.LocationService(), "/", Microsoft.VisualStudio.Services.Location.RelativeToSetting.WebApplication, TFCommonResources.FrameworkLocationServiceDescription(), "Framework");
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (executionEnvironment.IsHostedDeployment)
        {
          IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Application);
          string locationServiceUrl = vssRequestContext2.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext2, ServiceInstanceTypes.SPS, AccessMappingConstants.HostGuidAccessMappingMoniker);
          serviceDefinition3.RelativePath = (string) null;
          serviceDefinition3.RelativeToSetting = Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified;
          serviceDefinition3.LocationMappings.Add(new LocationMapping(AccessMappingConstants.HostGuidAccessMappingMoniker, locationServiceUrl));
          if (this.InstanceType == Guid.Empty || this.InstanceType == ServiceInstanceTypes.TFS)
          {
            ServiceDefinition serviceDefinition4 = new ServiceDefinition("LocationService", requestContext.ServiceHost.InstanceId, TFCommonResources.LocationService(), Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants.CollectionLocationServiceRelativePath, Microsoft.VisualStudio.Services.Location.RelativeToSetting.Context, TFCommonResources.FrameworkLocationServiceDescription(), "Framework");
            processedLocationData.AddServiceDefinition(serviceDefinition4);
          }
          Guid instanceId = requestContext.ServiceHost.InstanceId;
          string displayName = TFCommonResources.LocationService();
          string description = TFCommonResources.FrameworkLocationServiceDescription();
          List<LocationMapping> locationMappings = new List<LocationMapping>();
          locationMappings.Add(new LocationMapping(AccessMappingConstants.PublicAccessMappingMoniker, publicAccessMapping.AccessPoint));
          Guid serviceOwner = new Guid();
          ServiceDefinition serviceDefinition5 = new ServiceDefinition("LocationService2", instanceId, displayName, (string) null, Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified, description, "Framework", locationMappings, serviceOwner);
          serviceDefinition5.LocationMappings.Add(LocationServiceHelper.GetHostGuidLocationMapping(requestContext, requestContext.ServiceHost.InstanceId));
          serviceDefinition5.SetProperty("Microsoft.TeamFoundation.Location.CollectionName", (object) requestContext.ServiceHost.Name);
          processedLocationData.AddServiceDefinition(serviceDefinition5);
        }
        processedLocationData.AddServiceDefinition(serviceDefinition3);
      }
      ServiceDefinition serviceDefinition6 = new ServiceDefinition("LocationService2", Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier, TFCommonResources.LocationService(), "/", Microsoft.VisualStudio.Services.Location.RelativeToSetting.Context, TFCommonResources.FrameworkLocationServiceDescription(), "Framework");
      processedLocationData.AddServiceDefinition(serviceDefinition6);
      return processedLocationData;
    }

    private static void ProcessDefinitionInDependencyOrder(
      ServiceDefinition serviceDefinition,
      LocationData locationData,
      HashSet<ServiceDefinition> visitedDefinitions,
      Action<ServiceDefinition> processAction)
    {
      if (visitedDefinitions.Contains(serviceDefinition))
        return;
      visitedDefinitions.Add(serviceDefinition);
      if (serviceDefinition.ParentServiceType != null)
      {
        ServiceDefinition serviceDefinition1 = locationData.FindServiceDefinition(serviceDefinition.ParentServiceType, serviceDefinition.ParentIdentifier);
        if (serviceDefinition1 != null)
          LocalLocationDataProvider.ProcessDefinitionInDependencyOrder(serviceDefinition1, locationData, visitedDefinitions, processAction);
      }
      processAction(serviceDefinition);
    }

    private static void TraceDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> definitions)
    {
      requestContext.TraceConditionally(72009, TraceLevel.Info, LocalLocationDataProvider.s_area, LocalLocationDataProvider.s_layer, (Func<string>) (() =>
      {
        StringBuilder stringBuilder = new StringBuilder("Definitions: ");
        foreach (ServiceDefinition definition in definitions)
          stringBuilder.AppendFormat("Identifier={0} ParentIdentifier={1},", (object) definition.Identifier, (object) definition.ParentIdentifier);
        return stringBuilder.ToString();
      }));
    }

    private void ValidateServiceDefinitionToSave(
      IVssRequestContext requestContext,
      ServiceDefinition definition)
    {
      ArgumentUtility.CheckForNull<ServiceDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckStringForNullOrEmpty(definition.DisplayName, "definition.DisplayName");
      ArgumentUtility.CheckStringForNullOrEmpty(definition.ServiceType, "definition.ServiceType");
      ArgumentUtility.CheckStringForInvalidCharacters(definition.ServiceType, "definition.ServiceType");
      ArgumentUtility.CheckStringForInvalidSqlEscapeCharacters(definition.ServiceType, "definition.ServiceType");
      ArgumentUtility.CheckForEmptyGuid(definition.Identifier, "definition.Identifier");
      if (definition.RelativePath != null && definition.RelativeToSetting == Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified)
        throw new Microsoft.TeamFoundation.Framework.Server.InvalidServiceDefinitionException(TFCommonResources.InvalidFullyQualifiedServiceDefinition((object) definition.RelativeToSetting.ToString()));
      if (definition.RelativePath == null && definition.RelativeToSetting != Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified)
        throw new Microsoft.TeamFoundation.Framework.Server.InvalidServiceDefinitionException(TFCommonResources.InvalidRelativeServiceDefinition((object) definition.RelativeToSetting.ToString()));
      if (definition.InheritLevel != InheritLevel.None && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new Microsoft.TeamFoundation.Framework.Server.InvalidServiceDefinitionException(TFCommonResources.InvalidInheritLevelServiceDefinition((object) definition.InheritLevel.ToString()));
      if (definition.LocationMappings.Any<LocationMapping>() && requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new Microsoft.TeamFoundation.Framework.Server.InvalidServiceDefinitionException(TFCommonResources.InvalidLocationMappingsServiceDefinition());
      if (definition.RelativeToSetting == Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified && definition.ParentIdentifier == Guid.Empty && definition.LocationMappings.Count == 0)
        throw new Microsoft.TeamFoundation.Framework.Server.InvalidServiceDefinitionException(TFCommonResources.ServiceDefinitionWithNoLocations((object) definition.ServiceType));
      definition.Description = definition.Description == null ? string.Empty : definition.Description;
      definition.ToolId = definition.ToolId == null ? string.Empty : definition.ToolId;
      if (definition.InheritLevel == InheritLevel.None && (definition.ParentIdentifier != Guid.Empty || !string.IsNullOrEmpty(definition.ParentServiceType)) && this.ResolveParentIdentifier(requestContext, definition) == null)
        throw new ParentDefinitionNotFoundException(definition.ServiceType, definition.Identifier, definition.ParentServiceType, definition.ParentIdentifier);
      if (!definition.HasModifiedProperties || definition.Properties.Count <= 0)
        return;
      requestContext.CheckDeploymentRequestContext();
    }

    protected IDisposable AcquireSqlSemaphore(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Location.UseStaticLocalLoadSemaphore"))
        return (IDisposable) null;
      if (LocalLocationDataProvider.s_loadSemaphore.Wait(5000, requestContext.CancellationToken))
        return (IDisposable) new LocationDataProvider.SemaphoreReference(LocalLocationDataProvider.s_loadSemaphore);
      requestContext.Trace(12665669, TraceLevel.Error, LocalLocationDataProvider.s_area, LocalLocationDataProvider.s_layer, "Waited more than 5 seconds to acquire the static local load semaphore.");
      return (IDisposable) null;
    }
  }
}
