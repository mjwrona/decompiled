// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CsmResourceProviderHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ResourceArea("2900E97E-7BBD-4D87-95EE-BE54611B6184")]
  public abstract class CsmResourceProviderHttpClientBase : VssHttpClientBase
  {
    public CsmResourceProviderHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CsmResourceProviderHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CsmResourceProviderHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CsmResourceProviderHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CsmResourceProviderHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<AccountResource> Accounts_CreateOrUpdateAsync(
      AccountResourceRequest requestData,
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("58fa3a85-af20-408d-b46d-6d369408e3da");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        resourceName = resourceName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AccountResourceRequest>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccountResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task Accounts_DeleteAsync(
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("58fa3a85-af20-408d-b46d-6d369408e3da"), (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        resourceName = resourceName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<AccountResource> Accounts_GetAsync(
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<AccountResource>(new HttpMethod("GET"), new Guid("58fa3a85-af20-408d-b46d-6d369408e3da"), (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        resourceName = resourceName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<AccountResource> Accounts_UpdateAsync(
      AccountTagRequest requestData,
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("58fa3a85-af20-408d-b46d-6d369408e3da");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        resourceName = resourceName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AccountTagRequest>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccountResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<AccountResourceListResult> Accounts_ListByResourceGroupAsync(
      Guid subscriptionId,
      string resourceGroupName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<AccountResourceListResult>(new HttpMethod("GET"), new Guid("955956a7-fbeb-48e6-9d78-c60f3f84bae9"), (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<OperationListResult> Operations_ListAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OperationListResult>(new HttpMethod("GET"), new Guid("14917175-ecbe-453b-b436-50430219eba9"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ExtensionResource> Extensions_CreateAsync(
      ExtensionResourceRequest requestData,
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("8df1cb68-197e-4baf-8ce2-c96021879971");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        accountResourceName = accountResourceName,
        extensionResourceName = extensionResourceName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionResourceRequest>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task Extensions_DeleteAsync(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("8df1cb68-197e-4baf-8ce2-c96021879971"), (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        accountResourceName = accountResourceName,
        extensionResourceName = extensionResourceName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<ExtensionResource> Extensions_GetAsync(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ExtensionResource>(new HttpMethod("GET"), new Guid("8df1cb68-197e-4baf-8ce2-c96021879971"), (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        accountResourceName = accountResourceName,
        extensionResourceName = extensionResourceName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ExtensionResource> Extensions_UpdateAsync(
      ExtensionResourceRequest requestData,
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("8df1cb68-197e-4baf-8ce2-c96021879971");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        accountResourceName = accountResourceName,
        extensionResourceName = extensionResourceName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionResourceRequest>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ExtensionResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ExtensionResourceListResult> Extensions_ListByAccountAsync(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ExtensionResourceListResult>(new HttpMethod("GET"), new Guid("e14787ab-fbd5-4064-a75d-0603c9ed66a8"), (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        accountResourceName = accountResourceName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CheckNameAvailabilityResult> Accounts_CheckNameAvailabilityAsync(
      CheckNameAvailabilityParameter request,
      Guid subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7dbae6e1-993e-4ac9-b20d-6a39eee4028b");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CheckNameAvailabilityParameter>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CheckNameAvailabilityResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task MoveResourcesAsync(
      ResourcesMoveRequest resourcesMoveRequest,
      Guid subscriptionId,
      string resourceGroupName,
      string operationName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CsmResourceProviderHttpClientBase providerHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8d9245ee-19a2-45b2-be3e-03234122298e");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        operationName = operationName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourcesMoveRequest>(resourcesMoveRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CsmResourceProviderHttpClientBase providerHttpClientBase2 = providerHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await providerHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task HandleNotificationAsync(
      CsmSubscriptionRequest requestData,
      Guid subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CsmResourceProviderHttpClientBase providerHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("a7f5be2f-9af8-4cc2-863f-d07377b2c079");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CsmSubscriptionRequest>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CsmResourceProviderHttpClientBase providerHttpClientBase2 = providerHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await providerHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<CsmSubscriptionResourceListResult> SubscriptionResources_ListAsync(
      Guid subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CsmSubscriptionResourceListResult>(new HttpMethod("GET"), new Guid("8a066194-3817-4e76-9bbc-2a1446fa0fc5"), (object) new
      {
        subscriptionId = subscriptionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
