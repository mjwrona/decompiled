// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageMetricsTransactionHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Azure;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class StorageMetricsTransactionHelper
  {
    private const 
    #nullable disable
    string Ingress = "Ingress";
    private const string Egress = "Egress";
    private const string SuccessResponseType = "Success";
    private const string ClientOtherErrorResponseType = "ClientOtherError";
    private const string ClientThrottlingErrorResponseType = "ClientThrottlingError";
    private const string ServerBusyErrorResponseType = "ServerBusyError";
    private const string AuthorizationErrorResponseType = "AuthorizationError";
    private const string NetworkErrorResponseType = "NetworkError";
    private const string ClientTimeoutErrorResponseType = "ClientTimeoutError";
    private const string ServerTimeoutErrorResponseType = "ServerTimeoutError";
    private const string ServerOtherErrorResponseType = "ServerOtherError";
    private const string transactionsKey = "transactions";
    private const string responseTypeKey = "responsetype";
    public const string AzureStorageResourceType = "Microsoft.Storage/storageAccounts";
    private const string c_ExclusionRegistryPath = "/Diagnostics/Hosting/StorageAnalytics/ExclusionList";
    private const string c_AdditionalDrawerNamesRegistryPath = "/Diagnostics/Hosting/StorageAnalytics/AdditionalDrawerNamesList";

    public List<AzureMonitorStorageMetrics> CreateMetricsRowsAggregateByApiName(
      Dictionary<string, MetricTimeSeriesElement> timeSeriesElements)
    {
      // ISSUE: unable to decompile the method.
    }

    public async Task<Dictionary<string, MetricTimeSeriesElement>> GetApiMetricsByResponseType(
      MetricsQueryClient metricsClient,
      GenericResource storageResource,
      QueryTimeRange queryTimeRange)
    {
      Dictionary<string, MetricTimeSeriesElement> apiMetricsByResponseTypes = new Dictionary<string, MetricTimeSeriesElement>();
      MetricsQueryOptions metricsQueryOptions = new MetricsQueryOptions()
      {
        TimeRange = new QueryTimeRange?(queryTimeRange),
        Filter = "ResponseType eq '*' and ApiName eq '*'"
      };
      foreach (MetricTimeSeriesElement timeSeriesElement in ((NullableResponse<MetricsQueryResult>) await metricsClient.QueryResourceAsync(((ArmResource) storageResource).Id.ToString(), (IEnumerable<string>) new string[1]
      {
        "transactions"
      }, metricsQueryOptions, new CancellationToken())).Value.Metrics.SelectMany<MetricResult, MetricTimeSeriesElement>((Func<MetricResult, IEnumerable<MetricTimeSeriesElement>>) (m => (IEnumerable<MetricTimeSeriesElement>) m.TimeSeries)))
      {
        string str1;
        string str2;
        if (timeSeriesElement.Metadata.TryGetValue("responsetype", out str1) && timeSeriesElement.Metadata.TryGetValue("apiname", out str2))
          apiMetricsByResponseTypes[str2 + "/" + str1] = timeSeriesElement;
      }
      Dictionary<string, MetricTimeSeriesElement> metricsByResponseType = apiMetricsByResponseTypes;
      apiMetricsByResponseTypes = (Dictionary<string, MetricTimeSeriesElement>) null;
      return metricsByResponseType;
    }

    public MetricsQueryClient CreateMetricsClient(
      IVssRequestContext requestContext,
      StorageMetricsTransactionHelper.AuthenticationType authType)
    {
      string subscriptionTenantId = StorageMetricsTransactionHelper.GetSubscriptionTenantId(requestContext);
      switch (authType)
      {
        case StorageMetricsTransactionHelper.AuthenticationType.UseForAzureMonitorAccess:
          return MetricsQueryClientFactory.UsingManagedIdentity().GetMetricsQueryClient((string) null);
        case StorageMetricsTransactionHelper.AuthenticationType.SpecifyServicePrincipal:
          return MetricsQueryClientFactory.UsingServicePrincipal(AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalClientId"), AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint")).GetMetricsQueryClient(subscriptionTenantId);
        case StorageMetricsTransactionHelper.AuthenticationType.FactoryDefault:
          return MetricsQueryClientFactory.Default.GetMetricsQueryClient(subscriptionTenantId, AadSupportedResources.AzureResourceManager);
        default:
          throw new InvalidOperationException("Invalid authentication type");
      }
    }

    public ArmClient CreateManagementClient(
      IVssRequestContext requestContext,
      StorageMetricsTransactionHelper.AuthenticationType authType)
    {
      string subscriptionTenantId = StorageMetricsTransactionHelper.GetSubscriptionTenantId(requestContext);
      switch (authType)
      {
        case StorageMetricsTransactionHelper.AuthenticationType.UseForAzureMonitorAccess:
          return ArmClientFactory.ManagedIdentity((string) null).GetArmClient((string) null);
        case StorageMetricsTransactionHelper.AuthenticationType.SpecifyServicePrincipal:
          return ArmClientFactory.ServicePrincipal(AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalClientId"), AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint")).GetArmClient(subscriptionTenantId);
        case StorageMetricsTransactionHelper.AuthenticationType.FactoryDefault:
          return ArmClientFactory.Default.GetArmClient(subscriptionTenantId, AadSupportedResources.AzureResourceManager);
        default:
          throw new InvalidOperationException("Invalid authentication type");
      }
    }

    public async Task<(StorageMetricsTransactionHelper.AuthenticationType? authType, HashSet<GenericResource> genericResources)> GetStorageAccounts(
      IVssRequestContext requestContext,
      string AzureSubscriptionId)
    {
      foreach (StorageMetricsTransactionHelper.AuthenticationType authType in System.Enum.GetValues(typeof (StorageMetricsTransactionHelper.AuthenticationType)))
      {
        try
        {
          if (authType == StorageMetricsTransactionHelper.AuthenticationType.UseForAzureMonitorAccess)
          {
            if (!requestContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.UseForAzureMonitorAccess"))
              continue;
          }
          AsyncPageable<GenericResource> genericResourcesAsync = this.CreateManagementClient(requestContext, authType).GetSubscriptionResource(SubscriptionResource.CreateResourceIdentifier(AzureSubscriptionId)).GetGenericResourcesAsync("resourceType eq 'Microsoft.Storage/storageAccounts'", (string) null, new int?(), new CancellationToken());
          HashSet<GenericResource> storageResources = new HashSet<GenericResource>((IEqualityComparer<GenericResource>) new AzureMonitorHelper.GenericResourceComparer());
          await foreach (GenericResource genericResource in genericResourcesAsync)
            storageResources.Add(genericResource);
          return (new StorageMetricsTransactionHelper.AuthenticationType?(authType), storageResources);
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, string.Format("Unable to use AuthenticationType {0} due to error {1}", (object) authType, (object) ex.Message));
        }
      }
      requestContext.TraceAlways(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, "All authentication types failed.");
      return (new StorageMetricsTransactionHelper.AuthenticationType?(), (HashSet<GenericResource>) null);
    }

    private static string GetSubscriptionTenantId(IVssRequestContext requestContext)
    {
      string configurationSetting = AzureRoleUtil.GetOverridableConfigurationSetting(ServicingTokenConstants.AzureSubscriptionId);
      return !requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.ResourceManagement.TenantIdFromSubscription") ? AzureRoleUtil.GetOverridableConfigurationSetting(ServicingTokenConstants.ResourceManagerAadTenantId) : AadTenantHelper.WithManagedIdentityClient(Guid.Parse(configurationSetting), (ITFLogger) null).GetSingleTenantId().ToString();
    }

    public string GetStorageCluster(
      IVssRequestContext requestContext,
      Uri blobEndpoint,
      string storageAccountName)
    {
      try
      {
        IPHostEntry entry;
        if (AzureDnsUtils.IsValidDnsEntry(blobEndpoint.Host, out entry))
        {
          try
          {
            return entry.HostName.Split('.')[1];
          }
          catch (IndexOutOfRangeException ex)
          {
            requestContext.TraceInfo(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, "Error parsing hostName " + blobEndpoint.Host + " when processing storage account " + storageAccountName);
            return string.Empty;
          }
        }
        else
        {
          requestContext.TraceInfo(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, "Error resolving host " + blobEndpoint.Host + " when processing storage account " + storageAccountName);
          return string.Empty;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceInfo(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, "Error resolving host " + blobEndpoint.Host + " when processing storage account " + storageAccountName + ". " + ex.ToReadableStackTrace());
        return string.Empty;
      }
    }

    public async Task<Dictionary<string, Uri>> GetStorageAccountEndpointMappings(
      IVssRequestContext requestContext)
    {
      Dictionary<string, Uri> mappedAccounts = new Dictionary<string, Uri>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      HashSet<string> accountExclusion = new HashSet<string>((IEnumerable<string>) service.GetValue<string>(requestContext, (RegistryQuery) "/Diagnostics/Hosting/StorageAnalytics/ExclusionList", string.Empty).Split(','), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        FrameworkServerConstants.ConfigurationSecretsDrawerName,
        FrameworkServerConstants.StorageConnectionsDrawerName
      };
      string str = service.GetValue<string>(requestContext, (RegistryQuery) "/Diagnostics/Hosting/StorageAnalytics/AdditionalDrawerNamesList", string.Empty);
      stringSet.UnionWith((IEnumerable<string>) str.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries));
      ITeamFoundationStrongBoxService strongBox = requestContext.GetService<ITeamFoundationStrongBoxService>();
      foreach (string name in stringSet)
      {
        Guid drawerId = strongBox.UnlockDrawer(requestContext, name, false);
        if (!(drawerId == Guid.Empty))
        {
          foreach (StrongBoxItemInfo drawerContent in strongBox.GetDrawerContents(requestContext, drawerId))
          {
            try
            {
              if (!drawerContent.LookupKey.EndsWith("-previous"))
              {
                string connectionString = strongBox.GetString(requestContext, drawerContent);
                if (!connectionString.Equals(FrameworkServerConstants.DevStorageConnectionString, StringComparison.OrdinalIgnoreCase))
                {
                  CloudStorageAccount storageAccount;
                  if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
                  {
                    if (!mappedAccounts.Keys.Contains<string>(storageAccount.Credentials.AccountName) && !accountExclusion.Contains(drawerContent.LookupKey))
                    {
                      if (await this.CheckClientAsync(requestContext, storageAccount))
                      {
                        requestContext.TraceInfo(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, "Successfully retrieved blobEndpoint Uri for storage account " + storageAccount.Credentials.AccountName);
                        mappedAccounts.Add(storageAccount.Credentials.AccountName, storageAccount.BlobEndpoint);
                      }
                      else
                        requestContext.TraceInfo(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, "Failed retrieved blobEndpoint Uri for storage account " + storageAccount.Credentials.AccountName);
                    }
                    storageAccount = (CloudStorageAccount) null;
                  }
                }
              }
            }
            catch (RequestCanceledException ex)
            {
              throw;
            }
            catch
            {
            }
          }
        }
      }
      Dictionary<string, Uri> endpointMappings = mappedAccounts;
      mappedAccounts = (Dictionary<string, Uri>) null;
      accountExclusion = (HashSet<string>) null;
      strongBox = (ITeamFoundationStrongBoxService) null;
      return endpointMappings;
    }

    protected virtual async Task<bool> CheckClientAsync(
      IVssRequestContext requestContext,
      CloudStorageAccount storageAccount)
    {
      try
      {
        (await new CloudStorageAccountWrapper(storageAccount).CreateCloudBlobClient().ListContainersSegmentedAsync("aaa", ContainerListingDetails.None, new int?(1), (BlobContinuationToken) null, cancellationToken: requestContext.CancellationToken)).Results.Count<CloudBlobContainer>();
        return true;
      }
      catch (StorageException ex) when (string.Equals(ex.RequestInformation.ErrorCode, "AuthenticationFailed", StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }
      catch
      {
        return false;
      }
    }

    public enum AuthenticationType
    {
      UseForAzureMonitorAccess,
      SpecifyServicePrincipal,
      FactoryDefault,
    }
  }
}
