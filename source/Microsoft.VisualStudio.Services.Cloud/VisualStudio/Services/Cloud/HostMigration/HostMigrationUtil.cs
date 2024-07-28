// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.HostMigrationUtil
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.HostMigration;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class HostMigrationUtil
  {
    public const string c_frameworkStorageName = "FileService";
    private const string c_callerFormat = "{0};{1}";
    private const int c_defaultContainerParallelism = 8;
    private const int c_defaultBlobParallelism = 8;
    private const int c_defaultSasTokenExpirationDays = 60;
    private const string SetReadOnlyModePolicyUponSettingReadOnlyModeFeatureFlagName = "VisualStudio.Services.HostMigration.SetReadOnlyModePolicyUponSettingReadOnlyMode";

    public static int DefaultSasTokenExpirationDays => 60;

    public static T CreateHttpClient<T>(
      IVssRequestContext deploymentContext,
      Guid serviceInstanceId)
      where T : class, IVssHttpClient
    {
      HostMigrationUtil.CheckRequestContext(deploymentContext);
      IInternalInstanceManagementService service = deploymentContext.GetService<IInternalInstanceManagementService>();
      ServiceInstance serviceInstance1 = service.GetServiceInstance(deploymentContext, serviceInstanceId);
      Uri baseUri;
      if (serviceInstance1 != null)
      {
        baseUri = serviceInstance1.PublicUri;
      }
      else
      {
        ServiceInstance serviceInstance2 = service.GetServiceInstance(deploymentContext, serviceInstanceId);
        if (serviceInstance2 != null)
          baseUri = serviceInstance2.PublicUri;
        else
          baseUri = new Uri((deploymentContext.GetService<IPartitioningService>().QueryPartitionContainers(deploymentContext, ServiceInstanceTypes.SPS).Where<PartitionContainer>((Func<PartitionContainer, bool>) (c => c.ContainerId == serviceInstanceId)).FirstOrDefault<PartitionContainer>() ?? throw new TeamFoundationServicingException("Could not find ServiceInstance or PartitionContainer for instance id: " + serviceInstanceId.ToString("D"))).Address);
      }
      return ((ICreateClient) deploymentContext.ClientProvider).CreateClient<T>(deploymentContext, baseUri, "HostMigration", (ApiResourceLocationCollection) null);
    }

    internal static Dictionary<string, CloudStorageAccount> LoadStorageAccounts(
      IVssRequestContext deploymentContext,
      int fileServiceStorageAccountId,
      Action<string> log = null)
    {
      Dictionary<string, CloudStorageAccount> dictionary = new Dictionary<string, CloudStorageAccount>();
      if (fileServiceStorageAccountId == -2)
        return dictionary;
      dictionary["FileService"] = !deploymentContext.GetService<IVssRegistryService>().GetValue<bool>(deploymentContext, (RegistryQuery) SwappableAzureBlobGeoRedundancyProvider.SwapPrimaryAndSecondaryRegistryKey, false) ? CloudStorageAccount.Parse(TeamFoundationFileService.GetStorageAccountConnectionString(deploymentContext, fileServiceStorageAccountId)) : throw new InvalidOperationException("Migrations are not supported when swapping the primary and secondary storage accounts during a failover");
      using (IDisposableReadOnlyList<BlobMigrationExtension> extensions = deploymentContext.GetExtensions<BlobMigrationExtension>())
      {
        foreach (BlobMigrationExtension migrationExtension in (IEnumerable<BlobMigrationExtension>) extensions)
        {
          foreach (MigrateStorageInfo storageInfo in migrationExtension.GetStorageInfos(deploymentContext, log))
            dictionary[storageInfo.Name] = CloudStorageAccount.Parse(HostMigrationUtil.GetStorageConnectionStringFromStrongbox(deploymentContext, storageInfo.Drawer, storageInfo.LookupKey));
        }
      }
      return dictionary;
    }

    internal static Dictionary<string, AzureProvider> ToAzureProviders(
      IVssRequestContext deploymentContext,
      IDictionary<string, CloudStorageAccount> accounts)
    {
      return accounts.ToDictionary<KeyValuePair<string, CloudStorageAccount>, string, AzureProvider>((Func<KeyValuePair<string, CloudStorageAccount>, string>) (o => o.Key), (Func<KeyValuePair<string, CloudStorageAccount>, AzureProvider>) (o => HostMigrationUtil.GetAzureProvider(deploymentContext, o.Value)));
    }

    public static Dictionary<string, AzureProvider> LoadBlobProviders(
      IVssRequestContext deploymentContext,
      int fileServiceStorageAccountId,
      Action<string> log = null)
    {
      return HostMigrationUtil.ToAzureProviders(deploymentContext, (IDictionary<string, CloudStorageAccount>) HostMigrationUtil.LoadStorageAccounts(deploymentContext, fileServiceStorageAccountId, log));
    }

    public static List<Tuple<StorageMigration, AzureProvider>> GetAdditionalStorageMigrations(
      IVssRequestContext deploymentContext,
      Guid hostId,
      bool shardedOnly = false,
      StorageType? storageType = null,
      Action<string> log = null)
    {
      TeamFoundationServiceHostProperties hostProperties = deploymentContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentContext, hostId) ?? new TeamFoundationServiceHostProperties(hostId, Guid.Empty, "", "", "", "", TeamFoundationServiceHostStatus.Stopped, "", ServiceHostSubStatus.None, TeamFoundationHostType.Unknown, 0, DateTime.Now, "", 0);
      List<Tuple<StorageMigration, AzureProvider>> storageMigrations = new List<Tuple<StorageMigration, AzureProvider>>();
      using (IDisposableReadOnlyList<BlobMigrationExtension> extensions = deploymentContext.GetExtensions<BlobMigrationExtension>())
      {
        foreach (BlobMigrationExtension migrationExtension in (IEnumerable<BlobMigrationExtension>) extensions)
        {
          List<MigrateStorageInfo> storageInfos = migrationExtension.GetStorageInfos(deploymentContext, log);
          List<StorageMigration> containerLists = migrationExtension.GetContainerLists(deploymentContext, hostProperties, log);
          int count = containerLists.Count;
          if (count != storageInfos.Count)
            throw new InvalidOperationException("The extension " + migrationExtension.GetType().Name + " returns a list of StorageMigration and a list of MigrateStorageInfo. But the two lists are not of the same length.");
          for (int index = 0; index < count; ++index)
          {
            MigrateStorageInfo migrateStorageInfo = storageInfos[index];
            StorageMigration storageMigration = containerLists[index];
            AzureProvider azureProvider = HostMigrationUtil.GetAzureProvider(deploymentContext, migrateStorageInfo.Drawer, migrateStorageInfo.LookupKey);
            if (!shardedOnly || storageMigration.IsSharded)
            {
              if (storageType.HasValue)
              {
                StorageType? nullable = storageType;
                StorageType storageType1 = storageMigration.StorageType;
                if (!(nullable.GetValueOrDefault() == storageType1 & nullable.HasValue))
                  continue;
              }
              storageMigrations.Add(new Tuple<StorageMigration, AzureProvider>(storageMigration, azureProvider));
            }
          }
        }
      }
      return storageMigrations;
    }

    public static Guid GetCollectionId(IVssRequestContext requestContext, Guid hostId)
    {
      Guid collectionId = Guid.Empty;
      foreach (ServiceDefinition serviceDefinition in LocationServiceHelper.GetSpsLocationClient(requestContext, hostId).GetServiceDefinitionsAsync("LocationService2").Result)
      {
        if (!string.IsNullOrEmpty(serviceDefinition.GetProperty<string>("Microsoft.TeamFoundation.Location.CollectionName", (string) null)))
        {
          collectionId = serviceDefinition.Identifier;
          break;
        }
      }
      return collectionId;
    }

    public static void GetServiceInstance(
      IVssRequestContext requestContext,
      string instanceName,
      out Guid instanceId,
      out Guid instanceType)
    {
      ServiceInstance serviceInstance = HostMigrationUtil.GetServiceInstance(requestContext, instanceName);
      instanceId = serviceInstance.InstanceId;
      instanceType = serviceInstance.InstanceType;
    }

    public static ServiceInstance GetServiceInstance(
      IVssRequestContext requestContext,
      string instanceName)
    {
      PartitionContainer container = (PartitionContainer) null;
      IEnumerable<ServiceInstance> serviceInstances = (IEnumerable<ServiceInstance>) requestContext.GetService<IInstanceManagementService>().GetServiceInstances(requestContext);
      ServiceInstance serviceInstance1 = HostMigrationUtil.MatchServiceInstanceByGuid(serviceInstances, instanceName);
      if (serviceInstance1 != null)
        return serviceInstance1;
      ServiceInstance serviceInstance2 = HostMigrationUtil.MatchServiceInstanceByHostedServiceName(serviceInstances, instanceName);
      if (serviceInstance2 != null)
        return serviceInstance2;
      ServiceInstance serviceInstance3 = HostMigrationUtil.MatchServiceInstanceByUri(serviceInstances, instanceName);
      if (serviceInstance3 != null)
        return serviceInstance3;
      ServiceInstance serviceInstance4 = HostMigrationUtil.MatchServiceInstanceByUriAuthority(serviceInstances, instanceName);
      if (serviceInstance4 != null)
        return serviceInstance4;
      ServiceInstance serviceInstance5 = HostMigrationUtil.FuzzyMatchServiceInstanceByUri(serviceInstances, instanceName);
      if (serviceInstance5 != null || serviceInstance5 != null)
        return serviceInstance5;
      IList<PartitionContainer> source1 = requestContext.GetService<IPartitioningService>().QueryPartitionContainers(requestContext, ServiceInstanceTypes.SPS);
      IEnumerable<PartitionContainer> partitionContainers = (IEnumerable<PartitionContainer>) new List<PartitionContainer>();
      Guid parsedGuidResult;
      IEnumerable<PartitionContainer> source2 = !Guid.TryParse(instanceName, out parsedGuidResult) ? source1.Where<PartitionContainer>((Func<PartitionContainer, bool>) (c => HostMigrationUtil.Matches(instanceName, c.Address) || HostMigrationUtil.Matches(instanceName, c.InternalAddress))) : source1.Where<PartitionContainer>((Func<PartitionContainer, bool>) (x => x.ContainerId == parsedGuidResult));
      if (source2.Count<PartitionContainer>() > 1)
        throw new HostInstanceAmbiguousException("Found multiple matching partition containers containing that instance name. Please use the service instance GUID instead. Matching containers:\n" + string.Join("\n", source2.Select<PartitionContainer, string>((Func<PartitionContainer, string>) (c => string.Format("{0} - {1}", (object) c.Name, (object) c.ContainerId)))));
      if (source2.Count<PartitionContainer>() == 1)
        container = source2.SingleOrDefault<PartitionContainer>();
      if (container != null)
      {
        serviceInstance5 = new ServiceInstance(container);
        return serviceInstance5;
      }
      List<string> values = new List<string>();
      values.AddRange(serviceInstances.Select<ServiceInstance, string>((Func<ServiceInstance, string>) (x => string.Format("({0} {1}) {2}", (object) "serviceInstances", (object) "AzureUri", (object) x.AzureUri))));
      values.AddRange(serviceInstances.Select<ServiceInstance, string>((Func<ServiceInstance, string>) (x => string.Format("({0} {1}) {2}", (object) "serviceInstances", (object) "PublicUri", (object) x.PublicUri))));
      values.AddRange(source1.Select<PartitionContainer, string>((Func<PartitionContainer, string>) (x => "(containers Address) " + x.Address)));
      values.AddRange(source1.Select<PartitionContainer, string>((Func<PartitionContainer, string>) (x => "(containers InternalAddress) " + x.InternalAddress)));
      throw new HostInstanceDoesNotExistException("Couldn't find the service instance " + instanceName + " checked " + string.Join(Environment.NewLine, (IEnumerable<string>) values));
    }

    private static ServiceInstance MatchServiceInstanceByGuid(
      IEnumerable<ServiceInstance> serviceInstances,
      string searchTerm)
    {
      ServiceInstance serviceInstance = (ServiceInstance) null;
      Guid parsedGuidResult;
      if (Guid.TryParse(searchTerm, out parsedGuidResult))
      {
        IEnumerable<ServiceInstance> serviceInstances1 = serviceInstances.Where<ServiceInstance>((Func<ServiceInstance, bool>) (si => si.InstanceId == parsedGuidResult));
        try
        {
          serviceInstance = serviceInstances1.DefaultIfEmpty<ServiceInstance>((ServiceInstance) null).SingleOrDefault<ServiceInstance>();
        }
        catch (InvalidOperationException ex)
        {
          throw new HostInstanceAmbiguousException("Multiple service instances matched the given GUID: " + searchTerm + ". " + HostMigrationUtil.MatchingInstancesToString(serviceInstances1));
        }
      }
      return serviceInstance;
    }

    private static ServiceInstance MatchServiceInstanceByHostedServiceName(
      IEnumerable<ServiceInstance> serviceInstances,
      string searchTerm)
    {
      IEnumerable<ServiceInstance> serviceInstances1 = serviceInstances.Where<ServiceInstance>((Func<ServiceInstance, bool>) (si => si.Name.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)));
      try
      {
        return serviceInstances1.DefaultIfEmpty<ServiceInstance>((ServiceInstance) null).SingleOrDefault<ServiceInstance>();
      }
      catch (InvalidOperationException ex)
      {
        throw new HostInstanceAmbiguousException("Multiple service instances matched the given HostedServiceName: " + searchTerm + ". " + HostMigrationUtil.MatchingInstancesToString(serviceInstances1));
      }
    }

    private static ServiceInstance MatchServiceInstanceByUri(
      IEnumerable<ServiceInstance> serviceInstances,
      string searchTerm)
    {
      ServiceInstance serviceInstance = (ServiceInstance) null;
      Uri parsedUri;
      if (Uri.TryCreate(searchTerm, UriKind.Absolute, out parsedUri))
      {
        IEnumerable<ServiceInstance> serviceInstances1 = serviceInstances.Where<ServiceInstance>((Func<ServiceInstance, bool>) (si => si.AzureUri == parsedUri || si.PublicUri == parsedUri));
        try
        {
          serviceInstance = serviceInstances1.DefaultIfEmpty<ServiceInstance>((ServiceInstance) null).SingleOrDefault<ServiceInstance>();
        }
        catch (InvalidOperationException ex)
        {
          throw new HostInstanceAmbiguousException("Multiple service instances matched the given URI authority: " + searchTerm + ". " + HostMigrationUtil.MatchingInstancesToString(serviceInstances1));
        }
      }
      return serviceInstance;
    }

    private static ServiceInstance MatchServiceInstanceByUriAuthority(
      IEnumerable<ServiceInstance> serviceInstances,
      string searchTerm)
    {
      IEnumerable<ServiceInstance> serviceInstances1 = serviceInstances.Where<ServiceInstance>((Func<ServiceInstance, bool>) (si => si.AzureUri.Authority.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) || si.PublicUri.Authority.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)));
      try
      {
        return serviceInstances1.DefaultIfEmpty<ServiceInstance>((ServiceInstance) null).SingleOrDefault<ServiceInstance>();
      }
      catch (InvalidOperationException ex)
      {
        throw new HostInstanceAmbiguousException("Multiple service instances matched the given URI authority: " + searchTerm + ". " + HostMigrationUtil.MatchingInstancesToString(serviceInstances1));
      }
    }

    private static ServiceInstance FuzzyMatchServiceInstanceByUri(
      IEnumerable<ServiceInstance> serviceInstances,
      string searchTerm)
    {
      IEnumerable<ServiceInstance> serviceInstances1 = serviceInstances.Where<ServiceInstance>((Func<ServiceInstance, bool>) (si => HostMigrationUtil.Matches(searchTerm, si.AzureUri.AbsoluteUri) || HostMigrationUtil.Matches(searchTerm, si.PublicUri.AbsoluteUri)));
      try
      {
        return serviceInstances1.DefaultIfEmpty<ServiceInstance>((ServiceInstance) null).SingleOrDefault<ServiceInstance>();
      }
      catch (InvalidOperationException ex)
      {
        throw new HostInstanceAmbiguousException("Multiple service instances matched the given URI authority: " + searchTerm + ". " + HostMigrationUtil.MatchingInstancesToString(serviceInstances1));
      }
    }

    private static bool Matches(string searchTerm, string candidate)
    {
      if (string.IsNullOrEmpty(candidate))
        return false;
      int num = candidate.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);
      int length = searchTerm.Length;
      return num >= 0 && (num + length >= candidate.Length || candidate[num + length] == '.' || candidate[num + length] == '/');
    }

    private static string MatchingInstancesToString(IEnumerable<ServiceInstance> matchingInstances) => "Matching instances:\n" + string.Join("\n", matchingInstances.Select<ServiceInstance, string>((Func<ServiceInstance, string>) (mi => string.Format("{0} - {1}", (object) mi.Name, (object) mi.InstanceId))));

    public static void VerifySasConnections(
      IVssRequestContext requestContext,
      TargetHostMigration targetMigration,
      List<StorageMigration> storageMigrations)
    {
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), "Verifying SAS Connections");
      List<StorageMigration> storageMigrations1 = new List<StorageMigration>();
      foreach (StorageMigration storageMigration in storageMigrations)
      {
        HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), "Verifying connection for: " + storageMigration.Uri);
        if (storageMigration.StorageType == StorageType.Blob)
        {
          CloudBlobContainer cloudBlobContainer = new CloudBlobContainer(new Uri(storageMigration.Uri), new StorageCredentials(storageMigration.SasToken));
          try
          {
            cloudBlobContainer.ListBlobsSegmented((string) null, true, BlobListingDetails.None, new int?(1), (BlobContinuationToken) null, (BlobRequestOptions) null, (OperationContext) null);
            try
            {
              HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), nameof (VerifySasConnections), true);
            }
            catch (Exception ex)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), "Fault was intentionally injected, this should only happen in tests!");
              throw new StorageException(new RequestResult()
              {
                HttpStatusCode = 403
              }, "Test injected error", ex);
            }
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), "...connection verified.");
          }
          catch (StorageException ex)
          {
            HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), "...connection attempt resulted an exception. Exception details: " + ex.ToString());
            if (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 403)
            {
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), "Connection attempt resulted a 403 Forbidden, will attempt to generate a new SAS token...");
              storageMigrations1.Add(storageMigration);
            }
            else if (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 404)
              HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), "Connection attempt resulted a 404 Not Found, the container does not exist.");
            else
              throw;
          }
        }
      }
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), string.Format("Found {0} containers that need new SAS tokens.", (object) storageMigrations1.Count));
      HostMigrationUtil.UpdateSasTokens(requestContext, targetMigration, storageMigrations1, (Action<string>) (x => HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (VerifySasConnections), x)));
      HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) targetMigration, nameof (HostMigrationUtil), string.Format("Generated new SAS tokens for {0} containers.", (object) storageMigrations1.Count));
    }

    public static void SetSasTokens(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      List<StorageMigration> containers,
      Action<string> log)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      List<Tuple<StrongBoxItemInfo, string>> items = new List<Tuple<StrongBoxItemInfo, string>>();
      string secretsDrawerName = FrameworkServerConstants.HostMigrationSecretsDrawerName;
      Guid drawer = service.UnlockOrCreateDrawer(requestContext, secretsDrawerName);
      foreach (StorageMigration container in containers)
      {
        string lookupKey = migrationEntry.GetLookupKey(container.Uri);
        StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo()
        {
          DrawerId = drawer,
          LookupKey = lookupKey
        };
        items.Add(Tuple.Create<StrongBoxItemInfo, string>(strongBoxItemInfo, container.SasToken));
        log("Added/Updated secret to Drawer: '" + secretsDrawerName + "', lookup key: '" + lookupKey + "'.");
      }
      if (items.Count <= 0)
        return;
      service.AddStrings(requestContext, items);
      log(string.Format("Added/Updated {0} SAS tokens in '{1}'.", (object) items.Count, (object) secretsDrawerName));
    }

    public static void SetSasToken(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      StorageMigration container,
      Action<string> log)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string secretsDrawerName = FrameworkServerConstants.HostMigrationSecretsDrawerName;
      string lookupKey = migrationEntry.GetLookupKey(container.Uri);
      Guid drawer = service.UnlockOrCreateDrawer(requestContext, secretsDrawerName);
      StrongBoxItemInfo info = new StrongBoxItemInfo()
      {
        DrawerId = drawer,
        LookupKey = lookupKey
      };
      service.AddString(requestContext, info, container.SasToken);
      log("Added secret to Drawer: '" + secretsDrawerName + "', lookup key: '" + lookupKey + "'.");
    }

    public static string GetSasToken(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      StorageMigration container,
      Action<string> log)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string secretsDrawerName = FrameworkServerConstants.HostMigrationSecretsDrawerName;
      string lookupKey = migrationEntry.GetLookupKey(container.Uri);
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, secretsDrawerName, lookupKey, false);
      if (itemInfo == null)
      {
        string message = "Each container must have a SAS token in Strongbox. Sas token at Drawer: '" + secretsDrawerName + "', lookup key: '" + lookupKey + "' was not found.";
        log(message);
        throw new InvalidOperationException(message);
      }
      string sasToken = service.GetString(requestContext, itemInfo);
      log("SAS token retrieved at Drawer: '" + secretsDrawerName + "', lookup key: '" + lookupKey + "'.");
      return sasToken;
    }

    public static long SetupContainerCopy(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      StorageMigration container,
      ConcurrentDictionary<string, object> sharedContext,
      Action<string> log,
      CancellationToken cancellationToken,
      string faultInjectionService = null,
      string faultInjectionMethod = null,
      bool useCachedBlobContinuationToken = false,
      string prefix = null)
    {
      IVssDeploymentServiceHost deploymentServiceHost = requestContext.ServiceHost.DeploymentServiceHost;
      Dictionary<string, AzureProvider> sourceProviders = HostMigrationUtil.LoadBlobProviders(requestContext, migrationEntry.StorageAccountId, log);
      long num1 = 0;
      int blobParallelism = HostMigrationUtil.GetBlobParallelism(requestContext);
      bool flag1 = HostMigrationUtil.FailIfUnexpectedBlobsOnTarget(requestContext, container.VsoArea);
      if (!container.IsSharded)
      {
        StorageCredentials credentials1 = new StorageCredentials(container.SasToken);
        CloudBlobContainer container1 = new CloudBlobContainer(new Uri(container.Uri), credentials1);
        CloudBlobContainer cloudBlobContainer = sourceProviders[container.VsoArea].GetCloudBlobContainer(requestContext, container.Id, true);
        StorageCredentials credentials2 = credentials1;
        ICloudBlobContainerWrapper containerWrapper1 = (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(container1, credentials2)
        {
          UpdateCredentialsCallback = (Func<StorageCredentials>) (() =>
          {
            using (IVssRequestContext updateRequestContext = deploymentServiceHost.CreateSystemContext())
            {
              HostMigrationUtil.UpdateSasToken(updateRequestContext, migrationEntry, container, (Action<string>) (m => HostMigrationLogger.LogInfo(updateRequestContext, (IMigrationEntry) migrationEntry, nameof (SetupContainerCopy), m)));
              return new StorageCredentials(container.SasToken);
            }
          })
        };
        ICloudBlobContainerWrapper containerWrapper2 = (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(cloudBlobContainer);
        if (faultInjectionService != null && faultInjectionMethod != null)
          HostMigrationUtil.CheckForFaultInjection(requestContext, (IMigrationEntry) migrationEntry, faultInjectionService, faultInjectionMethod);
        IVssRequestContext requestContext1 = requestContext;
        ICloudBlobContainerWrapper sourceContainer = containerWrapper1;
        ICloudBlobContainerWrapper targetContainer = containerWrapper2;
        int maxDegreeOfParallelism = blobParallelism;
        CancellationToken cancellationToken1 = cancellationToken;
        bool flag2 = useCachedBlobContinuationToken;
        bool flag3 = flag1;
        Action<string> log1 = log;
        int num2 = flag2 ? 1 : 0;
        int num3 = flag3 ? 1 : 0;
        num1 = BlobCopyUtil.QueueBlobCopy(requestContext1, sourceContainer, targetContainer, maxDegreeOfParallelism, true, cancellationToken1, log1, useCachedBlobContinuationToken: num2 != 0, failIfUnexpectedBlobsOnTarget: num3 != 0);
      }
      else
      {
        if (string.IsNullOrEmpty(prefix))
        {
          log("Copying blobs with in-container parallelism. No real-time logging is available.");
          log = (Action<string>) (msg => { });
        }
        using (IDisposableReadOnlyList<BlobMigrationExtension> extensions = requestContext.GetExtensions<BlobMigrationExtension>())
        {
          foreach (BlobMigrationExtension migrationExtension in (IEnumerable<BlobMigrationExtension>) extensions)
          {
            migrationExtension.SharedContextForShardedCopy = sharedContext;
            num1 += migrationExtension.ExecuteShardedCopy(requestContext, migrationEntry, container, prefix, sourceProviders, log);
          }
        }
      }
      using (HostMigrationComponent component = requestContext.CreateComponent<HostMigrationComponent>())
        component.UpdateContainerMigrationStatus(container, StorageMigrationStatus.Queued, string.Format("Queued {0} blobs for migration.", (object) num1));
      return num1;
    }

    public static int SetupContainerCopyForNewTargetBlobsForRollback(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      CopyNewTargetBlobsToSourceForRollbackRequest copyNewTargetBlobsToSourceForRollbackRequest,
      CancellationToken cancellationToken)
    {
      IVssDeploymentServiceHost deploymentServiceHost = requestContext.ServiceHost.DeploymentServiceHost;
      requestContext.GetService<TargetHostMigrationService>().GetMigrationEntry(requestContext, migrationEntry.MigrationId, decryptSasTokens: true);
      Dictionary<string, AzureProvider> dictionary = HostMigrationUtil.LoadBlobProviders(requestContext, migrationEntry.HostProperties.StorageAccountId);
      int blobParallelism = HostMigrationUtil.GetBlobParallelism(requestContext);
      int num = 0;
      ISasTokenRequestService service = requestContext.GetService<ISasTokenRequestService>();
      foreach (AccountSasTokenInfo providerSasToken in copyNewTargetBlobsToSourceForRollbackRequest.BlobProviderSasTokens)
      {
        AzureProvider azureProvider1;
        if (!dictionary.TryGetValue(providerSasToken.VsoArea, out azureProvider1))
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (SetupContainerCopyForNewTargetBlobsForRollback), "Unable to find SourceBlobProvider for " + providerSasToken.VsoArea + " on " + providerSasToken.AccountName);
        }
        else
        {
          string accountName = providerSasToken.AccountName;
          string tokenS2Sencrypted = providerSasToken.SasTokenS2SEncrypted;
          string sasToken = service.DecryptToken(requestContext, tokenS2Sencrypted);
          StorageCredentials storageCredentials = new StorageCredentials(sasToken);
          AzureProvider providerUsingSasToken = HostMigrationUtil.GetAzureProviderUsingSasToken(requestContext, new CloudStorageAccount(storageCredentials, accountName, (string) null, true), sasToken);
          Page<TaggedBlobItem> page = (Page<TaggedBlobItem>) null;
label_5:
          if (page == null)
          {
            page = providerUsingSasToken.FindBlobsByTags((IDictionary<string, string>) new Dictionary<string, string>()
            {
              {
                "HostId",
                migrationEntry.HostProperties.Id.ToString("N")
              }
            });
          }
          else
          {
            AzureProvider azureProvider2 = providerUsingSasToken;
            Dictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("HostId", migrationEntry.HostProperties.Id.ToString("N"));
            string continuationToken = page.ContinuationToken;
            page = azureProvider2.FindBlobsByTags((IDictionary<string, string>) tags, continuationToken: continuationToken);
          }
          if (page != null)
          {
            foreach (IGrouping<string, TaggedBlobItem> source in page.Values.GroupBy<TaggedBlobItem, string>((Func<TaggedBlobItem, string>) (blob => blob.BlobContainerName)))
            {
              try
              {
                string key = source.Key;
                List<TaggedBlobItem> list = source.ToList<TaggedBlobItem>();
                num += list.Count;
                Guid containerId = Guid.Parse(key);
                Uri containerAddress = new Uri(providerUsingSasToken.Uri, key);
                CloudBlobContainer cloudBlobContainer = azureProvider1.GetCloudBlobContainer(requestContext, containerId, true, new TimeSpan?());
                CloudBlobContainer container = new CloudBlobContainer(containerAddress, storageCredentials);
                ICloudBlobContainerWrapper sourceContainerWrapper = (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(cloudBlobContainer);
                ICloudBlobContainerWrapper targetContainerWrapper = (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(container, storageCredentials);
                BlobCopyUtil.CopyTargetBlobsToSource(requestContext, sourceContainerWrapper, targetContainerWrapper, (IEnumerable<TaggedBlobItem>) list, blobParallelism, cancellationToken);
              }
              catch (Exception ex)
              {
                HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (SetupContainerCopyForNewTargetBlobsForRollback), "Error queueing container " + source.Key + ". Msg: " + ex.Message + ", " + ex.StackTrace, ServicingStepLogEntryKind.Error);
              }
            }
          }
          if (!string.IsNullOrEmpty(page?.ContinuationToken) && !cancellationToken.IsCancellationRequested)
            goto label_5;
        }
      }
      return num;
    }

    public static TaggedBlobTransferStats MonitorContainerCopyForNewTargetBlobsForRollback(
      IVssRequestContext requestContext,
      SourceHostMigration migrationEntry,
      CopyNewTargetBlobsToSourceForRollbackRequest copyNewTargetBlobsToSourceForRollbackRequest,
      CancellationToken cancellationToken)
    {
      TaggedBlobTransferStats blobTransferStats = new TaggedBlobTransferStats();
      IVssDeploymentServiceHost deploymentServiceHost = requestContext.ServiceHost.DeploymentServiceHost;
      requestContext.GetService<TargetHostMigrationService>().GetMigrationEntry(requestContext, migrationEntry.MigrationId, decryptSasTokens: true);
      Dictionary<string, AzureProvider> dictionary = HostMigrationUtil.LoadBlobProviders(requestContext, migrationEntry.HostProperties.StorageAccountId);
      int blobParallelism = HostMigrationUtil.GetBlobParallelism(requestContext);
      int num = 0;
      ISasTokenRequestService service = requestContext.GetService<ISasTokenRequestService>();
      foreach (AccountSasTokenInfo providerSasToken in copyNewTargetBlobsToSourceForRollbackRequest.BlobProviderSasTokens)
      {
        AzureProvider azureProvider1;
        if (!dictionary.TryGetValue(providerSasToken.VsoArea, out azureProvider1))
        {
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (MonitorContainerCopyForNewTargetBlobsForRollback), "Unable to find SourceBlobProvider for " + providerSasToken.VsoArea + " on " + providerSasToken.AccountName);
        }
        else
        {
          string accountName = providerSasToken.AccountName;
          string tokenS2Sencrypted = providerSasToken.SasTokenS2SEncrypted;
          string sasToken = service.DecryptToken(requestContext, tokenS2Sencrypted);
          StorageCredentials storageCredentials = new StorageCredentials(sasToken);
          AzureProvider providerUsingSasToken = HostMigrationUtil.GetAzureProviderUsingSasToken(requestContext, new CloudStorageAccount(storageCredentials, accountName, (string) null, true), sasToken);
          HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (MonitorContainerCopyForNewTargetBlobsForRollback), "Searching " + providerSasToken.AccountName + " for tagged blobs being transferred");
          Page<TaggedBlobItem> page = (Page<TaggedBlobItem>) null;
          bool flag = true;
          do
          {
            if (page == null)
            {
              page = providerUsingSasToken.FindBlobsByTags((IDictionary<string, string>) new Dictionary<string, string>()
              {
                {
                  "HostId",
                  migrationEntry.HostProperties.Id.ToString("N")
                }
              });
            }
            else
            {
              AzureProvider azureProvider2 = providerUsingSasToken;
              Dictionary<string, string> tags = new Dictionary<string, string>();
              tags.Add("HostId", migrationEntry.HostProperties.Id.ToString("N"));
              string continuationToken = page.ContinuationToken;
              page = azureProvider2.FindBlobsByTags((IDictionary<string, string>) tags, continuationToken: continuationToken);
            }
            if (page != null)
            {
              foreach (IGrouping<string, TaggedBlobItem> source1 in page.Values.GroupBy<TaggedBlobItem, string>((Func<TaggedBlobItem, string>) (blob => blob.BlobContainerName)))
              {
                try
                {
                  string key = source1.Key;
                  List<TaggedBlobItem> list = source1.ToList<TaggedBlobItem>();
                  num += list.Count;
                  Guid containerId = Guid.Parse(key);
                  Uri containerAddress = new Uri(providerUsingSasToken.Uri, key);
                  CloudBlobContainer cloudBlobContainer = azureProvider1.GetCloudBlobContainer(requestContext, containerId, true, new TimeSpan?());
                  CloudBlobContainer container = new CloudBlobContainer(containerAddress, storageCredentials);
                  ICloudBlobContainerWrapper sourceContainerWrapper = (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(cloudBlobContainer);
                  ICloudBlobContainerWrapper targetContainerWrapper = (ICloudBlobContainerWrapper) new CloudBlobContainerWrapper(container, storageCredentials);
                  TaggedBlobTransferStats source2 = BlobCopyUtil.MonitorTargetBlobsToSource(requestContext, sourceContainerWrapper, targetContainerWrapper, (IEnumerable<TaggedBlobItem>) list, blobParallelism, cancellationToken);
                  blobTransferStats.CopiedBlobs += source2.CopiedBlobs;
                  blobTransferStats.PendingBlobs += source2.PendingBlobs;
                  blobTransferStats.NewQueuedBlobs += source2.NewQueuedBlobs;
                  blobTransferStats.CopyFailures += source2.CopyFailures;
                  if (source2.CompletedAllBlobTransfers)
                  {
                    if (source2.CopyFailures <= 0)
                      continue;
                  }
                  flag = false;
                }
                catch (Exception ex)
                {
                  HostMigrationLogger.LogInfo(requestContext, (IMigrationEntry) migrationEntry, nameof (MonitorContainerCopyForNewTargetBlobsForRollback), "Error monitoring container " + source1.Key + ". Msg: " + ex.Message + ", " + ex.StackTrace, ServicingStepLogEntryKind.Error);
                  ++blobTransferStats.CopyContainerFailures;
                }
              }
            }
          }
          while (!string.IsNullOrEmpty(page?.ContinuationToken) && !cancellationToken.IsCancellationRequested);
          blobTransferStats.CompletedAllBlobTransfers = flag;
        }
      }
      return blobTransferStats;
    }

    public static int GetContainerParallelism(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.MigrationMaxContainerParallelism, 8);

    public static int GetBlobParallelism(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.MigrationMaxBlobParallelism, 8);

    public static bool UseReadOnlyMode(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.MigrationUseReadOnlyMode, false);

    public static bool IsHostInReadOnlyMode(IVssRequestContext deploymentContext, Guid hostId) => deploymentContext.GetService<IHostReadOnlyModeService>().IsReadOnly(deploymentContext, hostId);

    public static void EnableReadOnlyMode(
      IVssRequestContext deploymentContext,
      Guid hostId,
      Action<string> log)
    {
      HostMigrationUtil.SetReadOnlyModePolicy(deploymentContext, hostId, true, log);
      IHostReadOnlyModeService service = deploymentContext.GetService<IHostReadOnlyModeService>();
      log(string.Format("Enabling read-only mode for host {0}.", (object) hostId));
      IVssRequestContext requestContext = deploymentContext;
      Guid hostId1 = hostId;
      service.UpdateReadOnlyState(requestContext, hostId1, true, true);
      log(string.Format("Host {0} is now read-only.", (object) hostId));
    }

    public static void DisableReadOnlyMode(
      IVssRequestContext deploymentContext,
      Guid hostId,
      Action<string> log)
    {
      IHostReadOnlyModeService service = deploymentContext.GetService<IHostReadOnlyModeService>();
      log(string.Format("Disabling read-only mode for host {0}.", (object) hostId));
      IVssRequestContext requestContext = deploymentContext;
      Guid hostId1 = hostId;
      service.UpdateReadOnlyState(requestContext, hostId1, false, true);
      log(string.Format("Host {0} is now writeable.", (object) hostId));
      HostMigrationUtil.SetReadOnlyModePolicy(deploymentContext, hostId, false, log);
    }

    public static bool IsServiceHostIdle(IVssRequestContext deploymentContext, Guid hostId) => deploymentContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentContext, hostId).SubStatus == ServiceHostSubStatus.Idle;

    public static void EnableIdleMode(IVssRequestContext deploymentContext, Guid hostId) => HostMigrationUtil.SetHostSubStaus(deploymentContext, hostId, ServiceHostSubStatus.Idle);

    public static void DisableIdleMode(IVssRequestContext deploymentContext, Guid hostId) => HostMigrationUtil.SetHostSubStaus(deploymentContext, hostId, ServiceHostSubStatus.None);

    public static void EnableMigratingMode(IVssRequestContext deploymentContext, Guid hostId) => HostMigrationUtil.SetHostSubStaus(deploymentContext, hostId, ServiceHostSubStatus.Migrating);

    private static void SetHostSubStaus(
      IVssRequestContext deploymentContext,
      Guid hostId,
      ServiceHostSubStatus subStatus)
    {
      ITeamFoundationHostManagementService service = deploymentContext.GetService<ITeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties hostProperties = service.QueryServiceHostProperties(deploymentContext, hostId);
      hostProperties.SubStatus = subStatus;
      service.UpdateServiceHost(deploymentContext, hostProperties);
    }

    private static void SetReadOnlyModePolicy(
      IVssRequestContext deploymentContext,
      Guid hostId,
      bool readOnlyPolicyValue,
      Action<string> log)
    {
      if (!deploymentContext.IsFeatureEnabled("VisualStudio.Services.HostMigration.SetReadOnlyModePolicyUponSettingReadOnlyMode"))
      {
        log("Not setting the Policy Policy.IsReadOnly because feature flag VisualStudio.Services.HostMigration.SetReadOnlyModePolicyUponSettingReadOnlyMode is not enabled");
      }
      else
      {
        ITeamFoundationHostManagementService service1 = deploymentContext.GetService<ITeamFoundationHostManagementService>();
        if (service1.QueryServiceHostProperties(deploymentContext, hostId).HostType != TeamFoundationHostType.Application)
          return;
        using (IVssRequestContext context = service1.BeginRequest(deploymentContext, hostId, RequestContextType.ServicingContext))
        {
          IOrganizationPolicyService service2 = context.GetService<IOrganizationPolicyService>();
          if (service2.GetPolicy<bool>(context, "Policy.IsReadOnly", false).EffectiveValue == readOnlyPolicyValue)
          {
            log(string.Format("Policy {0} is already set to desired value {1} on Host {2}", (object) "Policy.IsReadOnly", (object) readOnlyPolicyValue, (object) hostId));
          }
          else
          {
            service2.SetPolicyValue<bool>(context, "Policy.IsReadOnly", readOnlyPolicyValue);
            log(string.Format("Policy {0} is set to {1} on Host {2}", (object) "Policy.IsReadOnly", (object) readOnlyPolicyValue, (object) hostId));
          }
        }
      }
    }

    internal static void CheckPermission(
      IVssRequestContext requestContext,
      HostMigrationPermissions permission)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, HostMigrationSecurityConstants.HostMigrationNamespaceId).CheckPermission(requestContext, "AllMigrations", (int) permission, false);
    }

    internal static void CheckMigrationEnabled(
      IVssRequestContext requestContext,
      bool isStorageOnly)
    {
      HostMigrationUtil.CheckRequestContext(requestContext);
      string reason;
      if (HostMigrationUtil.IsMigrationEnabled(requestContext, isStorageOnly, out reason))
        return;
      if (string.IsNullOrEmpty(reason))
        throw new DataMigrationDisabledException();
      throw new DataMigrationDisabledException(reason);
    }

    internal static void BlockNewMigrationRequests(
      IVssRequestContext requestContext,
      Action<string> log)
    {
      if (requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
      {
        requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, FrameworkServerConstants.HostMigrateBlockNewMigrationRequests, true);
        log("New migration requests are blocked until the deployment completes.");
      }
      else
        log("This is not an SPS instance, will not block new migration requests.");
    }

    internal static void UnblockNewMigrationRequests(
      IVssRequestContext requestContext,
      Action<string> log)
    {
      requestContext.GetService<IVssRegistryService>().SetValue(requestContext, FrameworkServerConstants.HostMigrateBlockNewMigrationRequests, (object) null);
      log("New migration requests are now enabled.");
    }

    internal static bool IsMigrationEnabled(
      IVssRequestContext requestContext,
      bool isStorageOnly,
      out string reason)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery;
      if (!isStorageOnly)
      {
        IVssRegistryService registryService = service;
        IVssRequestContext requestContext1 = requestContext;
        registryQuery = (RegistryQuery) FrameworkServerConstants.HostMigrateBlockNewMigrationRequests;
        ref RegistryQuery local = ref registryQuery;
        if (registryService.GetValue<bool>(requestContext1, in local, false))
        {
          reason = "Migrations are temporarily disabled due to an ongoing deployment. Please try again later.";
          return false;
        }
      }
      if (!requestContext.GetService<IHostMigrationManagementService>().IsBackgroundMigrationEnabled(requestContext))
      {
        reason = "Migrations are temporarily disabled until databases are upgraded. Please try again later.";
        return false;
      }
      IVssRegistryService registryService1 = service;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.AccountMigrateEnabled;
      ref RegistryQuery local1 = ref registryQuery;
      if (!registryService1.GetValue<bool>(requestContext2, in local1, false))
      {
        reason = "Migrations are not enabled on this host. See registry key '" + FrameworkServerConstants.AccountMigrateEnabled;
        return false;
      }
      reason = string.Empty;
      return true;
    }

    internal static void CheckOnlineBlobCopyEnabled(IVssRequestContext requestContext)
    {
      HostMigrationUtil.CheckRequestContext(requestContext);
      if (!HostMigrationUtil.IsOnlineBlobCopyEnabled(requestContext))
        throw new OnlineBlobCopyDisabledException();
    }

    internal static bool IsOnlineBlobCopyEnabled(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) FrameworkServerConstants.OnlineBlobCopyEnabled, false);

    internal static bool FailIfUnexpectedBlobsOnTarget(
      IVssRequestContext requestContext,
      string containerArea)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) string.Format(FrameworkServerConstants.BlobCopyFailIfUnexpectedBlobsOnTargetPerAreaFormatString, (object) containerArea), false);
    }

    internal static void InvokeExtensionCallouts(
      IVssRequestContext requestContext,
      Action<string, ServicingStepLogEntryKind> log,
      string calloutName,
      Action<IHostMigrationExtension> invokeCallout,
      bool suppressExceptions)
    {
      requestContext.CheckDeploymentRequestContext();
      using (IDisposableReadOnlyList<IHostMigrationExtension> extensions = requestContext.GetExtensions<IHostMigrationExtension>())
      {
        log(string.Format("Found {0} IHostMigrationExtensions.", (object) extensions.Count), ServicingStepLogEntryKind.Informational);
        foreach (IHostMigrationExtension migrationExtension in (IEnumerable<IHostMigrationExtension>) extensions)
        {
          log("Executing " + migrationExtension.GetType().FullName + "." + calloutName, ServicingStepLogEntryKind.Informational);
          try
          {
            invokeCallout(migrationExtension);
          }
          catch (Exception ex) when (suppressExceptions)
          {
            log(migrationExtension.GetType().FullName + "." + calloutName + " threw a " + ex.GetType().Name + " exception. This is a violation of the API contract and the exception will be suppressed. " + ex.ToString(), ServicingStepLogEntryKind.Warning);
          }
        }
        log(string.Format("Finished executing {0} IHostMigrationExtensions ({1}).", (object) extensions.Count, (object) calloutName), ServicingStepLogEntryKind.Informational);
      }
    }

    internal static void CheckInstanceMappingNotLocal(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      HostInstanceMapping instanceMappingFromSps = requestContext.GetService<IInternalInstanceManagementService>().GetHostInstanceMappingFromSps(requestContext, hostId);
      if (instanceMappingFromSps != null && instanceMappingFromSps.ServiceInstance.InstanceId == requestContext.ServiceHost.InstanceId)
        throw new InvalidOperationException(string.Format("Could not delete service host {0} because InstanceManagementService is still using current instance id: {1}!", (object) hostId, (object) requestContext.ServiceHost.InstanceId));
      if (instanceMappingFromSps == null && requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS)
        throw new TeamFoundationServicingException(string.Format("Tenant mapping is missing from SPS. Not deleting the host because this state is unexpected. HostId: {0}", (object) hostId));
    }

    private static void CheckRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.IsSystemContext)
        throw new UnexpectedRequestContextTypeException(RequestContextType.ServicingContext);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException(FrameworkResources.DeploymentHostRequired());
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
    }

    internal static SqlConnectionInfoWrapper EncryptConnectionString(
      IVssRequestContext requestContext,
      ISqlConnectionInfo connectionInfo)
    {
      HostMigrationUtil.CheckRequestContext(requestContext);
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(connectionInfo, nameof (connectionInfo));
      IS2SEncryptionService service = requestContext.GetService<IS2SEncryptionService>();
      ISupportSqlCredential supportSqlCredential = (ISupportSqlCredential) connectionInfo;
      return new SqlConnectionInfoWrapper()
      {
        ConnectionString = connectionInfo.ConnectionString,
        UserId = supportSqlCredential.UserId,
        PasswordEncrypted = service.Encrypt(requestContext, supportSqlCredential.Password)
      };
    }

    internal static ISqlConnectionInfo DecryptConnection(
      IVssRequestContext requestContext,
      TargetHostMigration targetMigration)
    {
      HostMigrationUtil.CheckRequestContext(requestContext);
      ArgumentUtility.CheckForNull<TargetHostMigration>(targetMigration, nameof (targetMigration));
      SqlConnectionInfoWrapper connectionInfo = targetMigration.ConnectionInfo;
      SecureString password = requestContext.GetService<IS2SEncryptionService>().Decrypt(requestContext, connectionInfo.PasswordEncrypted);
      return SqlConnectionInfoFactory.Create(connectionInfo.ConnectionString, connectionInfo.UserId, password);
    }

    internal static void UpdateSasTokens(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      List<StorageMigration> storageMigrations,
      Action<string> log)
    {
      log(string.Format("Updating SAS tokens for migration: {0}", (object) migrationEntry.MigrationId));
      MigrationHttpClient httpClient = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, migrationEntry.SourceServiceInstanceId);
      foreach (StorageMigration storageMigration in storageMigrations)
      {
        SasTokenInfo request = new SasTokenInfo();
        request.ResourceUri = storageMigration.Uri;
        request.Permissions = SasRequestPermissions.Read | SasRequestPermissions.List;
        request.Expiration = TimeSpan.FromDays((double) HostMigrationUtil.DefaultSasTokenExpirationDays);
        if (migrationEntry.OnlineBlobCopy)
          request.Permissions |= SasRequestPermissions.Delete;
        string sasToken = httpClient.CreateSasTokenAsync(request, cancellationToken: requestContext.CancellationToken).SyncResult<SasTokenInfo>().SasToken;
        log("Retrieved a new SAS token from the source host");
        string a = requestContext.GetService<ISasTokenRequestService>().DecryptToken(requestContext, sasToken);
        if (!string.Equals(a, storageMigration.SasToken))
          storageMigration.SasToken = a;
      }
      log("Storing updated tokens in strongbox");
      HostMigrationUtil.SetSasTokens(requestContext, migrationEntry, storageMigrations, log);
    }

    public static StorageMigration UpdateSasToken(
      IVssRequestContext requestContext,
      TargetHostMigration migrationEntry,
      StorageMigration storageMigration,
      Action<string> log)
    {
      log("Updating SAS token for container: " + storageMigration.Uri);
      string sasToken = HostMigrationUtil.CreateHttpClient<MigrationHttpClient>(requestContext, migrationEntry.SourceServiceInstanceId).CreateSasTokenAsync(new SasTokenInfo()
      {
        ResourceUri = storageMigration.Uri,
        Permissions = SasRequestPermissions.Read | SasRequestPermissions.Delete | SasRequestPermissions.List,
        Expiration = TimeSpan.FromDays((double) HostMigrationUtil.DefaultSasTokenExpirationDays)
      }, cancellationToken: requestContext.CancellationToken).SyncResult<SasTokenInfo>().SasToken;
      log("Retrieved a new SAS token from the source host, storing it in StrongBox.");
      string a = requestContext.GetService<ISasTokenRequestService>().DecryptToken(requestContext, sasToken);
      if (!string.Equals(a, storageMigration.SasToken))
      {
        storageMigration.SasToken = a;
        HostMigrationUtil.SetSasToken(requestContext, migrationEntry, storageMigration, log);
      }
      return storageMigration;
    }

    private static AzureProvider GetFileServiceAzureProvider(
      IVssRequestContext deploymentContext,
      int storageAccountId)
    {
      string connectionString = TeamFoundationFileService.GetStorageAccountConnectionString(deploymentContext, storageAccountId);
      AzureProvider serviceAzureProvider = new AzureProvider();
      serviceAzureProvider.ServiceStart(deploymentContext, (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "BlobStorageConnectionStringOverride",
          connectionString
        }
      });
      return serviceAzureProvider;
    }

    public static AzureProvider GetAzureProvider(
      IVssRequestContext deploymentContext,
      string drawerName,
      string lookupKey)
    {
      string stringFromStrongbox = HostMigrationUtil.GetStorageConnectionStringFromStrongbox(deploymentContext, drawerName, lookupKey);
      Dictionary<string, string> settings = new Dictionary<string, string>();
      settings.Add("BlobStorageConnectionStringOverride", stringFromStrongbox);
      AzureProvider azureProvider = new AzureProvider();
      azureProvider.ServiceStart(deploymentContext, (IDictionary<string, string>) settings);
      return azureProvider;
    }

    internal static AzureProvider GetAzureProvider(
      IVssRequestContext deploymentContext,
      CloudStorageAccount account)
    {
      AzureProvider azureProvider = new AzureProvider();
      azureProvider.ServiceStart(deploymentContext, (IDictionary<string, string>) new Dictionary<string, string>()
      {
        ["BlobStorageConnectionStringOverride"] = account.ToString(true)
      });
      return azureProvider;
    }

    internal static AzureProvider GetAzureProviderUsingSasToken(
      IVssRequestContext deploymentContext,
      CloudStorageAccount account,
      string sasToken)
    {
      AzureProvider providerUsingSasToken = new AzureProvider();
      providerUsingSasToken.ServiceStart(deploymentContext, (IDictionary<string, string>) new Dictionary<string, string>()
      {
        ["BlobStorageUriOverride"] = account.BlobStorageUri.PrimaryUri.ToString(),
        ["BlobStorageCredentialsOverride"] = sasToken
      });
      return providerUsingSasToken;
    }

    private static string GetStorageConnectionStringFromStrongbox(
      IVssRequestContext deploymentContext,
      string drawerName,
      string lookupKey)
    {
      TeamFoundationStrongBoxService service = deploymentContext.GetService<TeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentContext, drawerName, true);
      return service.GetString(deploymentContext, drawerId, lookupKey);
    }

    internal static void InvalidateCredentials(
      ICloudBlobContainerWrapper container,
      Action<string> log)
    {
      if (!(container is CloudBlobContainerWrapper containerWrapper))
        return;
      DateTime dateTime = DateTime.UtcNow.AddDays((double) HostMigrationUtil.DefaultSasTokenExpirationDays);
      StorageCredentials newCredentials = new StorageCredentials(string.Format("sv=2015-12-11&sr=c&sig=0000000000000000000000000000000000000000000%3D&se={0}-{1}-{2}T12:00:00Z&sp=rl", (object) dateTime.Year, (object) dateTime.Month, (object) dateTime.Day));
      containerWrapper.SetCredentials(newCredentials);
      log(string.Format("Invalidated credentials on {0}", (object) container.Uri));
    }

    internal static bool GetTestRegistryFlag(
      IVssRequestContext requestContext,
      string query,
      Action<string> log)
    {
      bool testRegistryFlag = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) query, false);
      if (testRegistryFlag)
      {
        if (requestContext.ServiceHost.IsProduction)
        {
          log("Test registry setting at path '" + query + "' is set but disallowed in production.  This value should only be set in test environments.");
          testRegistryFlag = false;
        }
        else
          log("Test registry setting at path '" + query + "' is set.  This value should only be set in test environments.");
      }
      return testRegistryFlag;
    }

    internal static void CheckForFaultInjection(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry,
      string service,
      string method,
      bool clearAfterHit = false)
    {
      string message;
      if (!HostMigrationUtil.CheckInjection(requestContext, migrationEntry, service, method, clearAfterHit, out message))
        return;
      HostMigrationUtil.PerformFaultInjectionFailure(requestContext, migrationEntry, method, message);
    }

    private static bool CheckInjection(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry,
      string service,
      string method,
      bool clearAfterHit,
      out string message)
    {
      message = (string) null;
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      string b = string.Format("{0};{1}", (object) service, (object) method);
      string a = service1.GetValue(requestContext, (RegistryQuery) FrameworkServerConstants.ServicingCollectionFaultInjection, string.Empty);
      bool flag = string.Equals(a, b, StringComparison.CurrentCulture) || string.Equals(a, b + "_" + migrationEntry.HostProperties.HostType.ToString(), StringComparison.CurrentCulture);
      if (flag)
      {
        if (requestContext.ServiceHost.IsProduction)
        {
          message = "Fault injection registry key is set but disallowed in production. Key: '" + FrameworkServerConstants.ServicingCollectionFaultInjection + "'. Value: '" + a + "'.";
          HostMigrationLogger.LogInfo(requestContext, migrationEntry, method, message, ServicingStepLogEntryKind.Warning);
          flag = false;
        }
        else
        {
          message = "Fault injection registry key was set matched the current method. Intentionally cause a fault. Key: '" + FrameworkServerConstants.ServicingCollectionFaultInjection + "'. Value: '" + a + "'.";
          HostMigrationLogger.LogInfo(requestContext, migrationEntry, method, message, ServicingStepLogEntryKind.Warning);
          if (clearAfterHit)
          {
            HostMigrationLogger.LogInfo(requestContext, migrationEntry, method, "clearAfterHit was set, clearing the registry key: " + FrameworkServerConstants.ServicingCollectionFaultInjection);
            service1.SetValue(requestContext, FrameworkServerConstants.ServicingCollectionFaultInjection, (object) null);
          }
        }
      }
      return flag;
    }

    private static void PerformFaultInjectionFailure(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry,
      string operation,
      string message)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      switch (HostMigrationInjectionUtil.CheckInjection<string>(requestContext, FrameworkServerConstants.MigrationFaultInjectionAction, string.Empty).InjectionValue)
      {
        case "KillProcess":
          HostMigrationLogger.LogInfo(requestContext, migrationEntry, operation, "Exiting the current process for fault injection");
          Process.GetCurrentProcess().Kill();
          break;
        case "ClearAfterHit":
          HostMigrationLogger.LogInfo(requestContext, migrationEntry, operation, "Clearing the registry key: " + FrameworkServerConstants.ServicingCollectionFaultInjection);
          service.SetValue(requestContext, FrameworkServerConstants.ServicingCollectionFaultInjection, (object) null);
          break;
        default:
          HostMigrationLogger.LogInfo(requestContext, migrationEntry, operation, "Throwing an exception for fault injection");
          throw new TeamFoundationServicingException(message);
      }
    }

    public static void SetDatabaseIdForHostDataspacesToUnspecified(
      IVssRequestContext deploymentContext,
      ITeamFoundationHostManagementService hms,
      Guid hostId)
    {
      using (IVssRequestContext vssRequestContext = hms.BeginRequest(deploymentContext, hostId, RequestContextType.SystemContext, throwIfShutdown: false))
        vssRequestContext.GetService<IDataspaceService>().UpdateDataspaces(vssRequestContext, (string) null, new Guid?(), new int?(DatabaseManagementConstants.InvalidDatabaseId), new DataspaceState?());
    }

    public static void FixJobPriorities(
      IVssRequestContext requestContext,
      IMigrationEntry migrationEntry)
    {
      List<TeamFoundationJobQueueEntry> foundationJobQueueEntryList1 = new List<TeamFoundationJobQueueEntry>();
      List<TeamFoundationJobQueueEntry> foundationJobQueueEntryList2 = new List<TeamFoundationJobQueueEntry>();
      List<TeamFoundationJobQueueEntry> jobQueueUpdates = new List<TeamFoundationJobQueueEntry>();
      using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
      {
        List<TeamFoundationJobQueueEntry> source = component.QueryJobQueueForOneHost(migrationEntry.HostProperties.Id, (IEnumerable<Guid>) null);
        HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (FixJobPriorities), string.Format("Found {0} job(s) queued for host {1} ", (object) source.Count, (object) migrationEntry.HostProperties.Id));
        List<TeamFoundationJobQueueEntry> list = source.Where<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (x => x.Priority >= 240)).ToList<TeamFoundationJobQueueEntry>();
        HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (FixJobPriorities), string.Format("Found {0} job(s) queued for host {1} with invalid priorities, correcting and requeueing", (object) list.Count, (object) migrationEntry.HostProperties.Id));
        foreach (TeamFoundationJobQueueEntry foundationJobQueueEntry1 in list)
        {
          TeamFoundationJobQueueEntry foundationJobQueueEntry2 = new TeamFoundationJobQueueEntry();
          foundationJobQueueEntry2.JobId = foundationJobQueueEntry1.JobId;
          foundationJobQueueEntry2.JobSource = foundationJobQueueEntry1.JobSource;
          foundationJobQueueEntry2.Priority = foundationJobQueueEntry1.Priority * -1 + 256;
          TeamFoundationJobQueueEntry foundationJobQueueEntry3 = foundationJobQueueEntry2;
          requestContext.Trace(15288133, TraceLevel.Info, nameof (HostMigrationUtil), nameof (FixJobPriorities), string.Format("Setting JobId {0}  for host {1} with priority {2}", (object) foundationJobQueueEntry3.JobId, (object) foundationJobQueueEntry3.JobSource, (object) foundationJobQueueEntry3.Priority));
          jobQueueUpdates.Add(foundationJobQueueEntry3);
        }
        if (jobQueueUpdates.Count <= 0)
          return;
        HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (FixJobPriorities), "Rescheduling bad jobs with corrected entries");
        int num = component.UpdateJobQueuePriorityDirect((IEnumerable<TeamFoundationJobQueueEntry>) jobQueueUpdates);
        HostMigrationLogger.LogInfo(requestContext, migrationEntry, nameof (FixJobPriorities), string.Format("Updated priority on  {0} job(s)", (object) num));
      }
    }
  }
}
