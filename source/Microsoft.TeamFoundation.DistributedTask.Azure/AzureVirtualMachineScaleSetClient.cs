// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.AzureVirtualMachineScaleSetClient
// Assembly: Microsoft.TeamFoundation.DistributedTask.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C98D823D-2608-4E2C-9060-C83C236BDAA8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Azure.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Azure
{
  public class AzureVirtualMachineScaleSetClient : 
    AzureHttpClientBase,
    IAzureVirtualMachineScaleSetClient,
    ICanExpire
  {
    private const string c_vmssPath = "providers/Microsoft.Compute/virtualMachineScaleSets";
    private const string c_vmPath = "virtualMachines";
    private const string c_vmDelete = "delete";
    private const string c_vmExtensions = "extensions";
    private const string c_vmReimageAll = "reimageall";
    private const string c_vmReimage = "reimage";
    private const string c_vmUpgrade = "manualupgrade";
    private const string c_vmInstanceView = "instanceview";
    public static string DefaultApiVersion = "2020-06-01";

    public AzureVirtualMachineScaleSetClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler,
      TimeSpan timeToLiveInCache,
      string apiVersion,
      bool validateHttpClientQueryParams)
      : base(baseUrl, pipeline, disposeHandler, timeToLiveInCache, validateHttpClientQueryParams)
    {
      this.ApiVersion = apiVersion ?? AzureVirtualMachineScaleSetClient.DefaultApiVersion;
    }

    public async Task<VirtualMachineScaleSet> GetAsync(
      Guid activityId,
      string azureId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).ToUrl();
      return await machineScaleSetClient.GetWithTracingAsync<VirtualMachineScaleSet>(url, activityId, AzureHttpClientBase.BuildRequestParameters((nameof (azureId), (object) azureId)), cancellationToken, nameof (GetAsync)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<VirtualMachineScaleSet>> ListAsync(
      Guid activityId,
      Guid subscriptionId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      string url = AzureHttpClientBase.GetSubscriptionUrlBuilder(subscriptionId).Add("providers/Microsoft.Compute/virtualMachineScaleSets").ToUrl();
      return await machineScaleSetClient.GetPaginatedWithTracingAsync<VirtualMachineScaleSet>(url, (string) null, activityId, AzureHttpClientBase.BuildRequestParameters((nameof (subscriptionId), (object) subscriptionId)), cancellationToken, nameof (ListAsync)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<VirtualMachineScaleSetVM>> ListVMInstancesAsync(
      Guid activityId,
      string azureId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("virtualMachines").ToUrl();
      string expandParameters = "&" + Uri.EscapeDataString("$expand") + "=instanceView";
      return await machineScaleSetClient.GetPaginatedWithTracingAsync<VirtualMachineScaleSetVM>(url, expandParameters, activityId, AzureHttpClientBase.BuildRequestParameters((nameof (azureId), (object) azureId)), cancellationToken, nameof (ListVMInstancesAsync)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<VirtualMachineScaleSetVM>> ListVMsAsync(
      Guid activityId,
      string azureId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("virtualMachines").ToUrl();
      return await machineScaleSetClient.GetPaginatedWithTracingAsync<VirtualMachineScaleSetVM>(url, string.Empty, activityId, AzureHttpClientBase.BuildRequestParameters((nameof (azureId), (object) azureId)), cancellationToken, nameof (ListVMsAsync)).ConfigureAwait(false);
    }

    public async Task<VirtualMachineScaleSet> UpdateAsync(
      Guid activityId,
      string azureId,
      VirtualMachineScaleSet vmss,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).ToUrl();
      if (vmss?.Properties?.VirtualMachineProfile != null)
        vmss.Properties.VirtualMachineProfile = (VirtualMachineScaleSetVMProfile) null;
      return await machineScaleSetClient.PatchWithTracingAsync<VirtualMachineScaleSet, VirtualMachineScaleSet>(url, vmss, activityId, AzureHttpClientBase.BuildRequestParameters((nameof (azureId), (object) azureId)), cancellationToken, nameof (UpdateAsync));
    }

    public async Task DeleteInstancesAsync(
      Guid activityId,
      string azureId,
      MachineInstances instances,
      bool force,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient1 = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url1 = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("delete").ToUrl();
      AzureVirtualMachineScaleSetClient machineScaleSetClient2 = machineScaleSetClient1;
      string url2 = url1;
      MachineInstances machineInstances = instances;
      Guid activityId1 = activityId;
      CancellationToken cancellationToken1 = cancellationToken;
      string queryParameters = force ? "forceDeletion=true" : (string) null;
      CancellationToken cancellationToken2 = cancellationToken1;
      await machineScaleSetClient2.PostAsync<MachineInstances>(url2, machineInstances, activityId1, queryParameters: queryParameters, cancellationToken: cancellationToken2);
    }

    public async Task UpgradeInstancesAsync(
      Guid activityId,
      string azureId,
      MachineInstances instances,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("manualupgrade").ToUrl();
      await machineScaleSetClient.PostAsync<MachineInstances>(url, instances, activityId, cancellationToken: cancellationToken);
    }

    public async Task<VirtualMachineScaleSetVMInstanceView> GetInstanceViewAsync(
      Guid activityId,
      string azureId,
      string instanceId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("virtualMachines").Add(instanceId).Add("instanceview").ToUrl();
      return await machineScaleSetClient.GetAsync<VirtualMachineScaleSetVMInstanceView>(url, activityId, cancellationToken: cancellationToken);
    }

    public async Task CreateExtensionAsync(
      Guid activityId,
      string azureId,
      VirtualMachineScaleSetExtension extension,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("extensions").Add(extension.Name).ToUrl();
      VirtualMachineScaleSetExtension scaleSetExtension = await machineScaleSetClient.PutAsync<VirtualMachineScaleSetExtension, VirtualMachineScaleSetExtension>(url, extension, activityId, cancellationToken: cancellationToken);
    }

    public async Task DeleteExtensionAsync(
      Guid activityId,
      string azureId,
      string extensionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("extensions").Add(extensionName).ToUrl();
      AzureOperationResponse operationResponse = await machineScaleSetClient.DeleteAsync(url, activityId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<VirtualMachineScaleSetExtension>> ListExtensionsAsync(
      Guid activityId,
      string azureId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("extensions").ToUrl();
      return await machineScaleSetClient.GetPaginatedWithTracingAsync<VirtualMachineScaleSetExtension>(url, (string) null, activityId, AzureHttpClientBase.BuildRequestParameters((nameof (azureId), (object) azureId)), cancellationToken, nameof (ListExtensionsAsync)).ConfigureAwait(false);
    }

    public async Task ReimageAllInstancesAsync(
      Guid activityId,
      string azureId,
      MachineInstances instances,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("reimageall").ToUrl();
      await machineScaleSetClient.PostAsync<MachineInstances>(url, instances, activityId, cancellationToken: cancellationToken);
    }

    public async Task ReimageInstancesAsync(
      Guid activityId,
      string azureId,
      MachineInstances instances,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureVirtualMachineScaleSetClient machineScaleSetClient = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(azureId, nameof (azureId));
      string url = AzureVirtualMachineScaleSetClient.GetVMSSUrlBuilder(azureId).Add("reimage").ToUrl();
      await machineScaleSetClient.PostAsync<MachineInstances>(url, instances, activityId, cancellationToken: cancellationToken);
    }

    protected static AzureHttpClientBase.UrlPathBuilder GetVMSSUrlBuilder(string azureId) => new AzureHttpClientBase.UrlPathBuilder().Add(azureId);

    protected override string ApiVersion { get; set; }

    bool ICanExpire.get_IsExpired() => this.IsExpired;
  }
}
