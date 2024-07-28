// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.AzureVirtualMachineClient
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Azure
{
  public class AzureVirtualMachineClient : 
    AzureHttpClientBase,
    IAzureVirtualMachineClient,
    ICanExpire
  {
    private const string c_virtualMachinePath = "providers/Microsoft.Compute/virtualMachines";
    private const string c_vmssPath = "providers/Microsoft.Compute/virtualMachineScaleSets";
    private const string c_vmInstanceView = "instanceview";
    private const string c_vmReimage = "reimage";
    public static string DefaultApiVersion = "2022-08-01";

    public AzureVirtualMachineClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler,
      TimeSpan timeToLiveInCache,
      string apiVersion,
      bool validateHttpClientQueryParams)
      : base(baseUrl, pipeline, disposeHandler, timeToLiveInCache, validateHttpClientQueryParams)
    {
      this.ApiVersion = apiVersion ?? AzureVirtualMachineClient.DefaultApiVersion;
    }

    public async Task<IEnumerable<VirtualMachineScaleSetVM>> ListVMInstancesAsync(
      Guid activityId,
      Guid subscriptionId,
      string scalesetAzureId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineClient virtualMachineClient = this;
      string url = virtualMachineClient.GetResourceUrlBuilder(scalesetAzureId).Add("providers/Microsoft.Compute/virtualMachines").ToUrl();
      string expandParameters = "&$filter='virtualMachineScaleSet/id' eq '" + scalesetAzureId + "'";
      return await virtualMachineClient.GetPaginatedWithTracingAsync<VirtualMachineScaleSetVM>(url, expandParameters, activityId, AzureHttpClientBase.BuildRequestParameters((nameof (subscriptionId), (object) subscriptionId)), cancellationToken, nameof (ListVMInstancesAsync));
    }

    public async Task<VirtualMachineScaleSetVMInstanceView> GetInstanceViewAsync(
      Guid activityId,
      string azureId,
      string instanceId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineClient virtualMachineClient = this;
      string url = virtualMachineClient.GetResourceUrlBuilder(azureId).Add("providers/Microsoft.Compute/virtualMachines").Add(instanceId).Add("instanceview").ToUrl();
      return await virtualMachineClient.GetAsync<VirtualMachineScaleSetVMInstanceView>(url, activityId, cancellationToken: cancellationToken);
    }

    public async Task DeleteInstancesAsync(
      Guid activityId,
      string azureId,
      string instanceId,
      bool forceDelete = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineClient virtualMachineClient = this;
      string url1 = virtualMachineClient.GetResourceUrlBuilder(azureId).Add("providers/Microsoft.Compute/virtualMachines").Add(instanceId).ToUrl();
      string url2 = virtualMachineClient.AddApiVersion(url1);
      if (forceDelete)
        url2 += "&forceDeletion=true";
      AzureOperationResponse operationResponse = await virtualMachineClient.DeleteAsync(url2, activityId, cancellationToken: cancellationToken);
    }

    public async Task ReimageInstancesAsync(
      Guid activityId,
      string azureId,
      string instanceId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineClient virtualMachineClient = this;
      string url = virtualMachineClient.GetResourceUrlBuilder(azureId).Add("providers/Microsoft.Compute/virtualMachines").Add(instanceId).Add("reimage").ToUrl();
      await virtualMachineClient.PostAsync(url, activityId, cancellationToken: cancellationToken);
    }

    private AzureHttpClientBase.UrlPathBuilder GetResourceUrlBuilder(string azureId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(azureId, nameof (azureId));
      string pathComponent = ((IEnumerable<string>) azureId.Split(new string[1]
      {
        "providers/Microsoft.Compute/virtualMachineScaleSets"
      }, StringSplitOptions.RemoveEmptyEntries)).FirstOrDefault<string>();
      return !string.IsNullOrEmpty(pathComponent) ? new AzureHttpClientBase.UrlPathBuilder().Add(pathComponent) : throw new ArgumentException("Azure URL is invalid.", "resourceUrl");
    }

    protected override string ApiVersion { get; set; }

    bool ICanExpire.get_IsExpired() => this.IsExpired;
  }
}
