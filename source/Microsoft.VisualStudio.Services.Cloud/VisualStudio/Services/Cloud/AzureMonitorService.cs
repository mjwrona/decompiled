// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureMonitorService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Monitor;
using Azure.ResourceManager.Monitor.Models;
using Azure.ResourceManager.Redis;
using Azure.ResourceManager.Resources;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AzureMonitorService : IAzureMonitorService, IVssFrameworkService
  {
    private const string c_area = "AzureMonitorService";
    private const string c_layer = "AzureMonitorService";
    private SubscriptionResource m_subscription;
    private ArmClient m_armClient;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      if (systemRequestContext.ExecutionEnvironment.IsDevFabricDeployment)
      {
        systemRequestContext.Trace(10028002, TraceLevel.Error, nameof (AzureMonitorService), nameof (AzureMonitorService), "The Azure monitor service is not supported in DevFabric");
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      }
      this.Initialize(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void Initialize(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      this.CreateManagementClient(systemRequestContext);
      IVssRequestContext requestContext = systemRequestContext;
      RegistrySettingsChangedCallback callback = new RegistrySettingsChangedCallback(this.OnFeatureFlagChanged);
      string[] strArray = new string[1]
      {
        string.Format(FeatureAvailbilityRegistryConstants.FeatureStateRegistryFormat, (object) "AzureDevOps.Services.ManagedIdentity.UseForAzureMonitorAccess")
      };
      service.RegisterNotification(requestContext, callback, strArray);
    }

    internal void OnFeatureFlagChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceAlways(15117107, TraceLevel.Info, nameof (AzureMonitorService), nameof (AzureMonitorService), "Recreating management client since the secret to use has changed.");
      this.CreateManagementClient(requestContext);
    }

    private void CreateManagementClient(IVssRequestContext requestContext)
    {
      string configurationSetting = AzureRoleUtil.GetOverridableConfigurationSetting(ServicingTokenConstants.AzureSubscriptionId);
      if (requestContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.UseForAzureMonitorAccess"))
      {
        this.m_armClient = ArmClientFactory.ManagedIdentity((string) null).GetArmClient((string) null);
      }
      else
      {
        string str = !requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.ResourceManagement.TenantIdFromSubscription") ? AzureRoleUtil.GetOverridableConfigurationSetting(ServicingTokenConstants.ResourceManagerAadTenantId) : AadTenantHelper.WithManagedIdentityClient(Guid.Parse(configurationSetting), (ITFLogger) null).GetSingleTenantId().ToString();
        this.m_armClient = ArmClientFactory.ServicePrincipal(AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalClientId"), AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint")).GetArmClient(str);
      }
      this.m_subscription = this.m_armClient.GetSubscriptionResource(SubscriptionResource.CreateResourceIdentifier(configurationSetting));
    }

    public virtual HashSet<GenericResource> GetAzureResources(
      IVssRequestContext requestContext,
      string resourceType)
    {
      HashSet<GenericResource> azureResources = new HashSet<GenericResource>((IEnumerable<GenericResource>) this.m_subscription.GetGenericResources("resourceType eq '" + resourceType + "'", (string) null, new int?(), new CancellationToken()), (IEqualityComparer<GenericResource>) new AzureMonitorHelper.GenericResourceComparer());
      requestContext.TraceAlways(15117108, TraceLevel.Info, nameof (AzureMonitorService), nameof (AzureMonitorService), string.Format("Found {0} object(s). ResourceType: {1}", (object) azureResources.Count, (object) resourceType));
      return azureResources;
    }

    public RedisResource[] GetRedisResources(
      IVssRequestContext requestContext,
      string resourceGroupName)
    {
      RedisResource[] array = ((IEnumerable<RedisResource>) RedisExtensions.GetAllRedis(this.m_armClient.GetResourceGroupResource(ResourceGroupResource.CreateResourceIdentifier(((ArmResource) this.m_subscription).Id.SubscriptionId, resourceGroupName)))).ToArray<RedisResource>();
      requestContext.TraceAlways(15117109, TraceLevel.Info, nameof (AzureMonitorService), nameof (AzureMonitorService), string.Format("Found {0} Redis caches in the {1} resource group.", (object) array.Length, (object) resourceGroupName));
      return array;
    }

    public virtual MonitorMetric[] GetAzureResourceMetricData(
      IVssRequestContext requestContext,
      string resourceId,
      string metricNames,
      string aggregation,
      DateTime startTime,
      DateTime endTime,
      TimeSpan interval)
    {
      if (startTime.CompareTo(endTime) == 0)
      {
        requestContext.Trace(10028005, TraceLevel.Error, nameof (AzureMonitorService), nameof (AzureMonitorService), "The starttime and endtime provided to the Azure Monitor service are the same.");
        throw new AzureMonitorInvalidTimeRangeException("The starttime and endtime provided are the same.");
      }
      ArmClient armClient = this.m_armClient;
      ResourceIdentifier resourceIdentifier = new ResourceIdentifier(resourceId);
      ArmResourceGetMonitorMetricsOptions monitorMetricsOptions = new ArmResourceGetMonitorMetricsOptions();
      monitorMetricsOptions.Timespan = string.Format("{0:o}/{1:o}", (object) startTime, (object) endTime);
      monitorMetricsOptions.Interval = new TimeSpan?(interval);
      monitorMetricsOptions.Metricnames = metricNames;
      monitorMetricsOptions.Aggregation = aggregation;
      monitorMetricsOptions.ResultType = new MonitorResultType?((MonitorResultType) 0);
      CancellationToken cancellationToken = new CancellationToken();
      return ((IEnumerable<MonitorMetric>) MonitorExtensions.GetMonitorMetrics(armClient, resourceIdentifier, monitorMetricsOptions, cancellationToken)).ToArray<MonitorMetric>();
    }
  }
}
