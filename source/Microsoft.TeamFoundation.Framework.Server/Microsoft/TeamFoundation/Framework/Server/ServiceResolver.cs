// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceResolver
  {
    private bool m_initialized;
    private readonly ConcurrentDictionary<string, VssFrameworkServiceDescriptor> m_extensibleVirtualHostServices = new ConcurrentDictionary<string, VssFrameworkServiceDescriptor>();
    private readonly ConcurrentDictionary<string, VssFrameworkServiceDescriptor> m_extensibleServices = new ConcurrentDictionary<string, VssFrameworkServiceDescriptor>();
    private ILockName m_extensibleServicesLockName;
    private static readonly ConcurrentDictionary<string, string> s_orphanedExtensibleTypes = new ConcurrentDictionary<string, string>();
    private static int s_multipleServiceCollectionProvidersReported = 0;
    private static int s_extensibleTypeMismatchesReported = 0;
    private static int s_extensibleTypesMissingReported = 0;
    private static ParameterizedLazy<ServiceResolver.FeatureFlags, IVssRequestContext> s_featureFlags = new ParameterizedLazy<ServiceResolver.FeatureFlags, IVssRequestContext>((Func<IVssRequestContext, ServiceResolver.FeatureFlags>) (context => ServiceResolver.FeatureFlags.Create(context)));
    private const string c_ExtensibleTypesRegistryQuery = "/Configuration/ServiceProvider/ExtensibleTypes/*";
    private const string c_PlatformIdentityServiceSimpleName = "Microsoft.VisualStudio.Services.Identity.PlatformIdentityService";
    private const string c_PlatformIdentityServiceFullName = "Microsoft.VisualStudio.Services.Identity.PlatformIdentityService, Microsoft.VisualStudio.Services.Identity";
    private const string c_Area = "HostManagement";
    private const string c_Layer = "ServiceResolver";

    public ServiceResolver(IVssServiceHost serviceHost) => this.m_extensibleServicesLockName = serviceHost.CreateUniqueLockName(string.Format("{0}/extensibleTypes", (object) this.GetType()));

    public VssFrameworkServiceDescriptor GetServiceDescriptor(
      IVssRequestContext requestContext,
      Type requestedServiceType)
    {
      return ServiceResolver.IsExtensibleType(requestedServiceType) ? this.GetServiceDescriptor(requestContext, requestedServiceType, this.IsVirtualHost(requestContext)) : VssFrameworkServiceDescriptor.Create(requestedServiceType);
    }

    public void Reset() => this.m_initialized = false;

    private void Initialize(IVssRequestContext requestContext)
    {
      IVssFrameworkServiceCollection collectionFromRegistry = this.GetServiceCollectionFromRegistry(requestContext);
      using (requestContext.Lock(this.m_extensibleServicesLockName))
      {
        if (this.m_initialized)
          return;
        this.m_initialized = true;
        try
        {
          this.m_extensibleServices.Clear();
          this.m_extensibleVirtualHostServices.Clear();
          IVssFrameworkServiceCollection collectionFromBinaries = this.GetServiceCollectionFromBinaries(requestContext);
          foreach (KeyValuePair<string, VssFrameworkServiceDescriptor> serviceCollection in (IEnumerable<KeyValuePair<string, VssFrameworkServiceDescriptor>>) this.MergeServiceCollections(requestContext, collectionFromBinaries, collectionFromRegistry))
          {
            this.m_extensibleServices[serviceCollection.Key] = serviceCollection.Value;
            this.m_extensibleVirtualHostServices[serviceCollection.Key] = serviceCollection.Value;
          }
        }
        catch (Exception ex)
        {
          this.m_initialized = false;
          throw;
        }
      }
    }

    internal virtual IVssFrameworkServiceCollection GetServiceCollectionFromBinaries(
      IVssRequestContext requestContext)
    {
      if (!ServiceResolver.s_featureFlags.GetValue(requestContext).LoadExtensibleTypesFromBinaries)
        return (IVssFrameworkServiceCollection) null;
      this.GetOrCreateDefaultImplementation<IVssAssembliesResolverService>(requestContext);
      IVssExtensionManagementService defaultImplementation = this.GetOrCreateDefaultImplementation<IVssExtensionManagementService>(requestContext);
      if (defaultImplementation == null)
        throw new InvalidOperationException("Unable to find or create a default implementation of IVssExtensionManagementService");
      IVssFrameworkServiceCollectionProvider collectionProvider = (IVssFrameworkServiceCollectionProvider) null;
      try
      {
        collectionProvider = defaultImplementation.GetExtension<IVssFrameworkServiceCollectionProvider>(requestContext, throwOnError: true);
      }
      catch (Exception ex)
      {
        if (Interlocked.CompareExchange(ref ServiceResolver.s_multipleServiceCollectionProvidersReported, 1, 0) == 0)
          TeamFoundationTracingService.TraceRaw(5867840, TraceLevel.Error, "HostManagement", nameof (ServiceResolver), "More than one IVssFrameworkServiceCollectionProvider was found.  A Service should have only one implmentation of this interface.");
      }
      if (collectionProvider == null && requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        throw new InvalidOperationException("VisualStudio.Services.ExtensibleTypes.Binaries.Enable feature flag is enabled but no IVssFrameworkServiceCollectionProvider was found in plugins.");
      return collectionProvider?.Services ?? (IVssFrameworkServiceCollection) new ServiceCollection();
    }

    private T GetOrCreateDefaultImplementation<T>(IVssRequestContext requestContext) where T : class, IVssFrameworkService
    {
      T implementationInstance = default (T);
      Type type = typeof (T);
      Type serviceImplementation = this.GetDefaultServiceImplementation(requestContext, type);
      try
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          implementationInstance = (T) ServiceProvider.CreateService(requestContext, type, serviceImplementation);
          VssFrameworkServiceDescriptor serviceDescriptor = VssFrameworkServiceDescriptor.Create(type, (object) implementationInstance);
          this.m_extensibleServices[type.Name] = serviceDescriptor;
          this.m_extensibleVirtualHostServices[type.Name] = serviceDescriptor;
        }
        else
        {
          requestContext = requestContext.To(TeamFoundationHostType.Deployment);
          implementationInstance = requestContext.GetService<T>();
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(5867839, TraceLevel.Error, "HostManagement", nameof (ServiceResolver), ex, string.Format("Error getting or creating default implementation {0} for extensible type {1}.", (object) serviceImplementation, (object) type.Name));
      }
      return implementationInstance;
    }

    internal virtual IVssFrameworkServiceCollection GetServiceCollectionFromRegistry(
      IVssRequestContext requestContext)
    {
      if (!ServiceResolver.s_featureFlags.GetValue(requestContext).LoadExtensibleTypesFromRegistry)
        return (IVssFrameworkServiceCollection) null;
      IEnumerable<RegistryItem> registryItems = ServiceResolver.ReadRegistryItems(requestContext, "/Configuration/ServiceProvider/ExtensibleTypes/*");
      ServiceCollection services = new ServiceCollection();
      foreach (RegistryItem registryItem in registryItems)
      {
        string keyName = RegistryUtility.GetKeyName(registryItem.Path);
        string typeName = registryItem.Value;
        Type type = Type.GetType(typeName);
        if (type == (Type) null && typeName.StartsWith("Microsoft.VisualStudio.Services.Identity.PlatformIdentityService", StringComparison.Ordinal))
        {
          typeName = "Microsoft.VisualStudio.Services.Identity.PlatformIdentityService, Microsoft.VisualStudio.Services.Identity";
          type = Type.GetType(typeName);
        }
        if (type != (Type) null)
          services.Replace(VssFrameworkServiceDescriptor.Create(keyName, type));
        else if (ServiceResolver.s_orphanedExtensibleTypes.TryAdd(keyName, typeName))
          requestContext.Trace(16101, TraceLevel.Error, "HostManagement", nameof (ServiceResolver), "An extensible type was registered but not found: Name = " + keyName + ", Value = " + typeName);
      }
      return (IVssFrameworkServiceCollection) services;
    }

    private IVssFrameworkServiceCollection MergeServiceCollections(
      IVssRequestContext requestContext,
      IVssFrameworkServiceCollection binariesServiceCollection,
      IVssFrameworkServiceCollection registryServiceCollection)
    {
      if (binariesServiceCollection == null && registryServiceCollection == null)
        throw new InvalidOperationException("Extensible type mappings from both binaries and registry are disabled.");
      if (binariesServiceCollection == null)
        return registryServiceCollection;
      if (registryServiceCollection == null)
        return binariesServiceCollection;
      List<Tuple<VssFrameworkServiceDescriptor, VssFrameworkServiceDescriptor>> tupleList = new List<Tuple<VssFrameworkServiceDescriptor, VssFrameworkServiceDescriptor>>();
      List<VssFrameworkServiceDescriptor> serviceDescriptorList = new List<VssFrameworkServiceDescriptor>();
      foreach (KeyValuePair<string, VssFrameworkServiceDescriptor> registryService in (IEnumerable<KeyValuePair<string, VssFrameworkServiceDescriptor>>) registryServiceCollection)
      {
        VssFrameworkServiceDescriptor descriptor = registryService.Value;
        VssFrameworkServiceDescriptor serviceDescriptor;
        if (binariesServiceCollection.TryGetValue(registryService.Key, out serviceDescriptor))
        {
          if (descriptor.ImplementationType != serviceDescriptor.ImplementationType)
          {
            tupleList.Add(Tuple.Create<VssFrameworkServiceDescriptor, VssFrameworkServiceDescriptor>(descriptor, serviceDescriptor));
            binariesServiceCollection.Replace(descriptor);
          }
        }
        else
        {
          serviceDescriptorList.Add(descriptor);
          binariesServiceCollection.Add(descriptor);
        }
      }
      if (tupleList.Count > 0 && Interlocked.CompareExchange(ref ServiceResolver.s_extensibleTypeMismatchesReported, 1, 0) == 0)
      {
        StringBuilder stringBuilder = new StringBuilder("Extensible type mapping(s) defined in binaries differ from mappings defined in registry.");
        foreach (Tuple<VssFrameworkServiceDescriptor, VssFrameworkServiceDescriptor> tuple in tupleList)
          stringBuilder.AppendLine(tuple.Item1.ServiceTypeName + ": Registry => " + tuple.Item1.ImplementationType.FullName + ", Binaries => " + tuple.Item2.ImplementationType.FullName);
        if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
          throw new InvalidOperationException(stringBuilder.ToString());
        TeamFoundationTracingService.TraceRaw(5867841, TraceLevel.Error, "HostManagement", nameof (ServiceResolver), stringBuilder.ToString());
      }
      if (serviceDescriptorList.Count > 0 && Interlocked.CompareExchange(ref ServiceResolver.s_extensibleTypesMissingReported, 1, 0) == 0)
      {
        StringBuilder stringBuilder = new StringBuilder("Extensible type mapping(s) defined in registry are missing from mappings defined in binaries.");
        foreach (VssFrameworkServiceDescriptor serviceDescriptor in serviceDescriptorList)
          stringBuilder.AppendLine(serviceDescriptor.ServiceTypeName + " => " + serviceDescriptor.ImplementationType.FullName);
        TeamFoundationTracingService.TraceRaw(5867842, TraceLevel.Error, "HostManagement", nameof (ServiceResolver), stringBuilder.ToString());
      }
      return binariesServiceCollection;
    }

    private static IEnumerable<RegistryItem> ReadRegistryItems(
      IVssRequestContext requestContext,
      string registryQuery)
    {
      IEnumerable<RegistryItem> registryItems;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        registryItems = RegistryHelpers.DeploymentReadRaw(requestContext.FrameworkConnectionInfo, (RegistryQuery) registryQuery);
      }
      else
      {
        requestContext = requestContext.To(TeamFoundationHostType.Deployment);
        registryItems = requestContext.GetService<CachedRegistryService>().Read(requestContext, (RegistryQuery) registryQuery);
      }
      return registryItems;
    }

    private VssFrameworkServiceDescriptor GetServiceDescriptor(
      IVssRequestContext requestContext,
      Type requestedType,
      bool isVirtualHost)
    {
      if (!this.m_initialized)
        this.Initialize(requestContext);
      ConcurrentDictionary<string, VssFrameworkServiceDescriptor> concurrentDictionary = isVirtualHost ? this.m_extensibleVirtualHostServices : this.m_extensibleServices;
      VssFrameworkServiceDescriptor serviceDescriptor;
      if (!concurrentDictionary.TryGetValue(requestedType.Name, out serviceDescriptor))
      {
        using (requestContext.Lock(this.m_extensibleServicesLockName))
        {
          if (!concurrentDictionary.TryGetValue(requestedType.Name, out serviceDescriptor))
          {
            Type serviceImplementation = this.GetDefaultServiceImplementation(requestContext, requestedType, isVirtualHost);
            if (serviceImplementation != (Type) null)
            {
              serviceDescriptor = VssFrameworkServiceDescriptor.Create(requestedType, serviceImplementation);
              concurrentDictionary[requestedType.Name] = serviceDescriptor;
            }
          }
        }
      }
      if (serviceDescriptor == null)
        throw new ExtensibleServiceTypeNotRegisteredException(requestedType);
      if (!requestedType.IsAssignableFrom(serviceDescriptor.ImplementationType))
        throw new ExtensibleServiceTypeNotValidException(requestedType, serviceDescriptor.ImplementationType);
      return serviceDescriptor;
    }

    private Type GetDefaultServiceImplementation(
      IVssRequestContext requestContext,
      Type requestedType,
      bool isVirtualHost = false)
    {
      Type serviceImplementation = (Type) null;
      DefaultServiceImplementationAttribute[] customAttributes = (DefaultServiceImplementationAttribute[]) requestedType.GetCustomAttributes(typeof (DefaultServiceImplementationAttribute), true);
      if (customAttributes.Length != 0)
      {
        serviceImplementation = customAttributes[0].GetImplementation(isVirtualHost);
        if (serviceImplementation != (Type) null && ServiceResolver.IsExtensibleType(serviceImplementation) && requestedType.IsAssignableFrom(serviceImplementation))
          serviceImplementation = this.GetServiceDescriptor(requestContext, serviceImplementation, isVirtualHost)?.ImplementationType;
      }
      return serviceImplementation;
    }

    private static bool IsExtensibleType(Type managedType)
    {
      if (!(managedType != (Type) null))
        return false;
      return managedType.IsAbstract || managedType.IsInterface;
    }

    internal virtual bool IsVirtualHost(IVssRequestContext requestContext) => requestContext.IsVirtualServiceHost();

    private class FeatureFlags
    {
      private const string c_RegistryExtensibleTypesDisabledFeatureFlag = "VisualStudio.Services.ExtensibleTypes.Registry.Disable";
      private const string c_BinaryExtensibleTypesEnabledFeatureFlag = "VisualStudio.Services.ExtensibleTypes.Binaries.Enable";

      private FeatureFlags(
        bool loadExtensibleTypesFromRegistry,
        bool loadExtensibleTypesFromBinaries)
      {
        this.LoadExtensibleTypesFromRegistry = loadExtensibleTypesFromRegistry;
        this.LoadExtensibleTypesFromBinaries = loadExtensibleTypesFromBinaries;
      }

      public bool LoadExtensibleTypesFromRegistry { get; }

      public bool LoadExtensibleTypesFromBinaries { get; }

      public static ServiceResolver.FeatureFlags Create(IVssRequestContext requestContext) => new ServiceResolver.FeatureFlags(ServiceResolver.FeatureFlags.GetFeatureFlag(requestContext, "VisualStudio.Services.ExtensibleTypes.Registry.Disable", FeatureAvailabilityState.Off) == FeatureAvailabilityState.Off, ServiceResolver.FeatureFlags.GetFeatureFlag(requestContext, "VisualStudio.Services.ExtensibleTypes.Binaries.Enable", FeatureAvailabilityState.Off) == FeatureAvailabilityState.On);

      private static FeatureAvailabilityState GetFeatureFlag(
        IVssRequestContext requestContext,
        string featureFlagName,
        FeatureAvailabilityState defaultState)
      {
        IEnumerable<RegistryItem> source = ServiceResolver.ReadRegistryItems(requestContext, TeamFoundationFeatureAvailabilityService.GenerateServiceHostAvailabilityStateRegistryPath(featureFlagName));
        return source.Any<RegistryItem>() ? TeamFoundationFeatureAvailabilityService.FeatureAvailabilityStateFromString(source.First<RegistryItem>().Value) : defaultState;
      }
    }
  }
}
