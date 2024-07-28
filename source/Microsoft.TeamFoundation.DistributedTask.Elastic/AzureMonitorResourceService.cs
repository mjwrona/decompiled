// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.AzureMonitorResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Azure;
using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class AzureMonitorResourceService : 
    ResourceServiceBase<IAzureMonitorClient>,
    IAzureMonitorResourceService,
    IVssFrameworkService
  {
    public async Task DeleteAutoscaleSettingAsync(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope,
      AutoscaleSettingResource autoscaleSetting)
    {
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointId, nameof (serviceEndpointId));
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointScope, nameof (serviceEndpointScope));
      await this.Client(requestContext, serviceEndpointId, serviceEndpointScope).DeleteAutoscaleSetting(requestContext.ActivityId, autoscaleSetting);
    }

    public async Task<IEnumerable<AutoscaleSettingResource>> ListAutoscaleSettingsInSubscriptionAsync(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope)
    {
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointId, nameof (serviceEndpointId));
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointScope, nameof (serviceEndpointScope));
      ServiceEndpoint serviceEndpointAsync = await requestContext.GetService<IServiceEndpointService2>().GetServiceEndpointAsync(requestContext, serviceEndpointScope, serviceEndpointId);
      if (serviceEndpointAsync == null)
        throw new ServiceEndpointDoesNotExistException(serviceEndpointId, serviceEndpointScope);
      if (!serviceEndpointAsync.Data.ContainsKey("subscriptionId") || string.IsNullOrEmpty(serviceEndpointAsync.Data["subscriptionId"]))
        throw new InvalidServiceEndpointException("Service endpoint " + serviceEndpointAsync.Name + " does not contain an Azure Subscription Id");
      IAzureMonitorClient azureMonitorClient = this.Client(requestContext, serviceEndpointId, serviceEndpointScope);
      Guid guid = new Guid(serviceEndpointAsync.Data["subscriptionId"]);
      Guid activityId = requestContext.ActivityId;
      Guid subscriptionId = guid;
      CancellationToken cancellationToken = new CancellationToken();
      return (IEnumerable<AutoscaleSettingResource>) (await azureMonitorClient.ListAutoscaleSettings(activityId, subscriptionId, cancellationToken)).ToArray<AutoscaleSettingResource>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.DisableTracing(systemRequestContext);

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.EnableTracing(systemRequestContext);

    protected override void ClientFactory(
      IVssRequestContext requestContext,
      out IAzureMonitorClient client)
    {
      string apiVersion = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/AzureMonitorClientApiVersion", AzureMonitorClient.DefaultApiVersion);
      bool validateHttpClientQueryParams = requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolValidateAzureHttpClientApiVersionQueryParam");
      client = (IAzureMonitorClient) new AzureMonitorClient(ResourceServiceBase<IAzureMonitorClient>.GetAzureResourceManagerUrl(), this.GetMessageHandler(requestContext, AzureHttpClientBase.DefaultAuthenticationUrl), false, new TimeSpan(), apiVersion, validateHttpClientQueryParams);
    }

    private IAzureMonitorClient Client(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope)
    {
      return this.Client(requestContext, serviceEndpointId, serviceEndpointScope, nameof (AzureMonitorResourceService), "IAzureMonitorClient");
    }
  }
}
