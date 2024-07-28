// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Azure.AzureMonitorClient
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
  public class AzureMonitorClient : AzureHttpClientBase, IAzureMonitorClient, ICanExpire
  {
    private const string c_autoscalesettingsPath = "providers/microsoft.insights/autoscalesettings";
    public static string DefaultApiVersion = "2015-04-01";

    public AzureMonitorClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler,
      TimeSpan timeToLiveInCache,
      string apiVersion,
      bool validateHttpClientQueryParams)
      : base(baseUrl, pipeline, disposeHandler, timeToLiveInCache, validateHttpClientQueryParams)
    {
      this.ApiVersion = apiVersion ?? AzureMonitorClient.DefaultApiVersion;
    }

    public async Task DeleteAutoscaleSetting(
      Guid activityId,
      AutoscaleSettingResource autoscaleSettingResource,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureMonitorClient azureMonitorClient = this;
      ArgumentUtility.CheckForNull<AutoscaleSettingResource>(autoscaleSettingResource, nameof (autoscaleSettingResource));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(autoscaleSettingResource.Id, "Id");
      string url = AzureMonitorClient.GetAutoscaleSettingUrlBuilder(autoscaleSettingResource.Id).ToUrl();
      AzureOperationResponse operationResponse = await azureMonitorClient.DeleteWithTracingAsync(url, activityId, AzureHttpClientBase.BuildRequestParameters(("Id", (object) autoscaleSettingResource.Id)), cancellationToken, nameof (DeleteAutoscaleSetting));
    }

    public async Task<IEnumerable<AutoscaleSettingResource>> ListAutoscaleSettings(
      Guid activityId,
      Guid subscriptionId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureMonitorClient azureMonitorClient = this;
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      string url = AzureHttpClientBase.GetSubscriptionUrlBuilder(subscriptionId).Add("providers/microsoft.insights/autoscalesettings").ToUrl();
      return await azureMonitorClient.GetPaginatedWithTracingAsync<AutoscaleSettingResource>(url, (string) null, activityId, AzureHttpClientBase.BuildRequestParameters((nameof (subscriptionId), (object) subscriptionId)), cancellationToken, nameof (ListAutoscaleSettings)).ConfigureAwait(false);
    }

    public async Task<AutoscaleSettingResource> UpdateAutoscaleSetting(
      Guid activityId,
      AutoscaleSettingResource autoscaleSettingResource,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureMonitorClient azureMonitorClient = this;
      ArgumentUtility.CheckForNull<AutoscaleSettingResource>(autoscaleSettingResource, nameof (autoscaleSettingResource));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(autoscaleSettingResource.Id, "Id");
      string url = AzureMonitorClient.GetAutoscaleSettingUrlBuilder(autoscaleSettingResource.Id).ToUrl();
      return await azureMonitorClient.PatchWithTracingAsync<AutoscaleSettingResource, AutoscaleSettingResource>(url, autoscaleSettingResource, activityId, AzureHttpClientBase.BuildRequestParameters(("Id", (object) autoscaleSettingResource.Id)), cancellationToken, nameof (UpdateAutoscaleSetting));
    }

    protected static AzureHttpClientBase.UrlPathBuilder GetAutoscaleSettingUrlBuilder(string azureId) => new AzureHttpClientBase.UrlPathBuilder().Add(azureId);

    protected override string ApiVersion { get; set; }

    bool ICanExpire.get_IsExpired() => this.IsExpired;
  }
}
