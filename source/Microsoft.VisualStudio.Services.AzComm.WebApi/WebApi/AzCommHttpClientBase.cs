// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.WebApi.AzCommHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using Microsoft.VisualStudio.Services.AzComm.Rest.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.AzComm.WebApi
{
  [ResourceArea("ED1325FD-71E8-4623-89F3-485951654312")]
  public abstract class AzCommHttpClientBase : VssHttpClientBase
  {
    public AzCommHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AzCommHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AzCommHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AzCommHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AzCommHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task SetupDataImportAsync(
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("aa3c0e29-5221-4ec6-9146-846585fb3bee"), (object) new
      {
        organizationId = organizationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<AzureSubscriptionDetailsGetResponse> GetAzureSubscriptionDetailsAsync(
      Guid subscriptionId,
      Guid organizationId,
      Guid tenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dcdf0dac-a564-478e-953e-83960cc0c2a6");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (organizationId), organizationId.ToString());
      keyValuePairList.Add(nameof (tenantId), tenantId.ToString());
      return this.SendAsync<AzureSubscriptionDetailsGetResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<AzureSubscriptionsGetResponse> GetAzureSubscriptionsForUserAsync(
      AzureSubscriptionsGetRequest payload,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dcdf0dac-a564-478e-953e-83960cc0c2a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (payload), (object) payload);
      return this.SendAsync<AzureSubscriptionsGetResponse>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BillingGetResponse> GetBillingInfoAsync(
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BillingGetResponse>(new HttpMethod("GET"), new Guid("01a173e9-49d7-4117-ab7f-903781216b69"), (object) new
      {
        organizationId = organizationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task RemoveBillingAsync(
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("01a173e9-49d7-4117-ab7f-903781216b69"), (object) new
      {
        organizationId = organizationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task SetupBillingAsync(
      BillingSetupRequest payload,
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommHttpClientBase commHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("01a173e9-49d7-4117-ab7f-903781216b69");
      object obj1 = (object) new
      {
        organizationId = organizationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BillingSetupRequest>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommHttpClientBase commHttpClientBase2 = commHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await commHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateBillingModeAsync(
      UpdateBillingModeRequest payload,
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommHttpClientBase commHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("01a173e9-49d7-4117-ab7f-903781216b69");
      object obj1 = (object) new
      {
        organizationId = organizationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateBillingModeRequest>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommHttpClientBase commHttpClientBase2 = commHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await commHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<DefaultLicenseTypeGetResponse> GetDefaultLicenseTypeAsync(
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DefaultLicenseTypeGetResponse>(new HttpMethod("GET"), new Guid("e4c08577-7df9-4489-9075-8d4f4e601109"), (object) new
      {
        organizationId = organizationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateDefaultLicenseTypeAsync(
      DefaultLicenseTypeUpdateRequest payload,
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommHttpClientBase commHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("e4c08577-7df9-4489-9075-8d4f4e601109");
      object obj1 = (object) new
      {
        organizationId = organizationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DefaultLicenseTypeUpdateRequest>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommHttpClientBase commHttpClientBase2 = commHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await commHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<MeterResourceGetResponse>> GetAllMeterResourcesAsync(
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<MeterResourceGetResponse>>(new HttpMethod("GET"), new Guid("bc59054b-0637-45e3-b38a-21f99c9e9041"), (object) new
      {
        organizationId = organizationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<MeterResourceUpdateResponse> UpdateMeterResourceAsync(
      MeterResourceUpdateRequest payload,
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bc59054b-0637-45e3-b38a-21f99c9e9041");
      object obj1 = (object) new
      {
        organizationId = organizationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MeterResourceUpdateRequest>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<MeterResourceUpdateResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<MeterUsageGetResponse> GetMeterUsageAsync(
      MeterUsageGetRequest payload,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3349fb1d-bea3-4af5-80b5-c996ec13db42");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (payload), (object) payload);
      return this.SendAsync<MeterUsageGetResponse>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task ReportUsageAsync(
      MeterUsageReportRequest payload,
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommHttpClientBase commHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3349fb1d-bea3-4af5-80b5-c996ec13db42");
      HttpContent httpContent = (HttpContent) new ObjectContent<MeterUsageReportRequest>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (organizationId), organizationId.ToString());
      AzCommHttpClientBase commHttpClientBase2 = commHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await commHttpClientBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<MeterUsage2GetResponse>> GetAllMeterUsagesAsync(
      Guid organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<MeterUsage2GetResponse>>(new HttpMethod("GET"), new Guid("1f553305-7e51-4278-837f-0c208c4a98fc"), (object) new
      {
        organizationId = organizationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SubscriptionGetResponse> GetSubscriptionInformationAsync(
      Guid subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<SubscriptionGetResponse>(new HttpMethod("GET"), new Guid("ea7ceb20-4788-41ce-a9e2-fffa8a742d68"), (object) new
      {
        subscriptionId = subscriptionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SubscriptionMeterResourceGetResponse>> GetAllSubscriptionMeterResourcesAsync(
      Guid subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SubscriptionMeterResourceGetResponse>>(new HttpMethod("GET"), new Guid("f1983f56-5f08-41b6-a309-d29e994be8d3"), (object) new
      {
        subscriptionId = subscriptionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateSubscriptionMeterResourceAsync(
      SubscriptionFreeQuantityRequest payload,
      Guid subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzCommHttpClientBase commHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("f1983f56-5f08-41b6-a309-d29e994be8d3");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SubscriptionFreeQuantityRequest>(payload, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AzCommHttpClientBase commHttpClientBase2 = commHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await commHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
