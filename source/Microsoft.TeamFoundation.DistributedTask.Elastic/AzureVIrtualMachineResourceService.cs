// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.AzureVIrtualMachineResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Azure;
using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class AzureVIrtualMachineResourceService : 
    ResourceServiceBase<IAzureVirtualMachineClient>,
    IAzureVirtualMachineResourceService,
    IVssFrameworkService
  {
    public async Task<IReadOnlyList<VirtualMachineScaleSetVM>> ListVMInstancesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool)
    {
      Guid serviceEndpointId = elasticPool.ServiceEndpointId;
      Guid serviceEndpointScope = elasticPool.ServiceEndpointScope;
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointId, "serviceEndpointId");
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointScope, "serviceEndpointScope");
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpointAsync = await requestContext.GetService<IServiceEndpointService2>().GetServiceEndpointAsync(requestContext, serviceEndpointScope, serviceEndpointId);
      if (serviceEndpointAsync == null)
        throw new ServiceEndpointDoesNotExistException(serviceEndpointId, serviceEndpointScope);
      if (!serviceEndpointAsync.Data.ContainsKey("subscriptionId") || string.IsNullOrEmpty(serviceEndpointAsync.Data["subscriptionId"]))
        throw new InvalidServiceEndpointException("Service endpoint " + serviceEndpointAsync.Name + " does not contain an Azure Subscription Id");
      IAzureVirtualMachineClient virtualMachineClient = this.Client(requestContext, serviceEndpointId, serviceEndpointScope);
      Guid guid = new Guid(serviceEndpointAsync.Data["subscriptionId"]);
      Guid activityId = requestContext.ActivityId;
      Guid subscriptionId = guid;
      string azureId = elasticPool.AzureId;
      CancellationToken cancellationToken = new CancellationToken();
      return (IReadOnlyList<VirtualMachineScaleSetVM>) (await virtualMachineClient.ListVMInstancesAsync(activityId, subscriptionId, azureId, cancellationToken)).ToArray<VirtualMachineScaleSetVM>();
    }

    public async Task<VirtualMachineScaleSetVMInstanceView> GetInstanceViewAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string instanceId)
    {
      Guid serviceEndpointId = elasticPool.ServiceEndpointId;
      Guid serviceEndpointScope = elasticPool.ServiceEndpointScope;
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointId, "serviceEndpointId");
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointScope, "serviceEndpointScope");
      return await this.Client(requestContext, serviceEndpointId, serviceEndpointScope).GetInstanceViewAsync(requestContext.ActivityId, elasticPool.AzureId, instanceId);
    }

    public async Task DeleteMachineAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string instanceId,
      bool forceDelete = false)
    {
      Guid serviceEndpointId = elasticPool.ServiceEndpointId;
      Guid serviceEndpointScope = elasticPool.ServiceEndpointScope;
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointId, "serviceEndpointId");
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointScope, "serviceEndpointScope");
      await this.Client(requestContext, serviceEndpointId, serviceEndpointScope).DeleteInstancesAsync(requestContext.ActivityId, elasticPool.AzureId, instanceId, forceDelete);
    }

    public async Task ReimageMachineAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string instanceId)
    {
      Guid serviceEndpointId = elasticPool.ServiceEndpointId;
      Guid serviceEndpointScope = elasticPool.ServiceEndpointScope;
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointId, "serviceEndpointId");
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointScope, "serviceEndpointScope");
      await this.Client(requestContext, serviceEndpointId, serviceEndpointScope).ReimageInstancesAsync(requestContext.ActivityId, elasticPool.AzureId, instanceId);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.DisableTracing(systemRequestContext);

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.EnableTracing(systemRequestContext);

    protected override void ClientFactory(
      IVssRequestContext requestContext,
      out IAzureVirtualMachineClient client)
    {
      string apiVersion = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/AzureVMClientApiVersion", AzureVirtualMachineClient.DefaultApiVersion);
      bool validateHttpClientQueryParams = requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolValidateAzureHttpClientApiVersionQueryParam");
      client = (IAzureVirtualMachineClient) new AzureVirtualMachineClient(ResourceServiceBase<IAzureVirtualMachineClient>.GetAzureResourceManagerUrl(), this.GetMessageHandler(requestContext, AzureHttpClientBase.DefaultAuthenticationUrl), false, new TimeSpan(), apiVersion, validateHttpClientQueryParams);
    }

    private IAzureVirtualMachineClient Client(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope)
    {
      return this.Client(requestContext, serviceEndpointId, serviceEndpointScope, nameof (AzureVIrtualMachineResourceService), "IAzureVirtualMachineClient");
    }
  }
}
