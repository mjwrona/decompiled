// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CsmHttpClientBase
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
  [ResourceArea("B3705FD5-DC18-47FC-BB2F-7B0F19A70822")]
  public abstract class CsmHttpClientBase : VssHttpClientBase
  {
    public CsmHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CsmHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CsmHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CsmHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CsmHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
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
      Guid guid = new Guid("5745408e-6e9e-49c7-92bf-62932c8df69d");
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("5745408e-6e9e-49c7-92bf-62932c8df69d"), (object) new
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
      return this.SendAsync<AccountResource>(new HttpMethod("GET"), new Guid("5745408e-6e9e-49c7-92bf-62932c8df69d"), (object) new
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
      Guid guid = new Guid("5745408e-6e9e-49c7-92bf-62932c8df69d");
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
      return this.SendAsync<AccountResourceListResult>(new HttpMethod("GET"), new Guid("73d8b171-a2a0-4ac6-ba0b-ef762098e5ec"), (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<OperationListResult> Operations_ListAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OperationListResult>(new HttpMethod("GET"), new Guid("454d976b-812e-4947-bc4e-c2c23160317e"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
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
      Guid guid = new Guid("9cb405cb-4a72-4a50-ab6d-be1da1726c33");
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9cb405cb-4a72-4a50-ab6d-be1da1726c33"), (object) new
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
      return this.SendAsync<ExtensionResource>(new HttpMethod("GET"), new Guid("9cb405cb-4a72-4a50-ab6d-be1da1726c33"), (object) new
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
      Guid guid = new Guid("9cb405cb-4a72-4a50-ab6d-be1da1726c33");
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
      return this.SendAsync<ExtensionResourceListResult>(new HttpMethod("GET"), new Guid("a509d9a8-d23f-4e0f-a69f-ad52b248943b"), (object) new
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
      Guid guid = new Guid("031d6b9b-a0d4-4b46-97c5-9ddaca1aa5cd");
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
      CsmHttpClientBase csmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9e0fa51b-9d61-4899-a5a1-e1f0f5e75bc0");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        operationName = operationName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourcesMoveRequest>(resourcesMoveRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CsmHttpClientBase csmHttpClientBase2 = csmHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await csmHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task HandleNotificationAsync(
      CsmSubscriptionRequest requestData,
      Guid subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CsmHttpClientBase csmHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("97bc4c4d-ce2e-4ca3-87cc-2bd07aeee500");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CsmSubscriptionRequest>(requestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CsmHttpClientBase csmHttpClientBase2 = csmHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await csmHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<CsmSubscriptionResourceListResult> SubscriptionResources_ListAsync(
      Guid subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CsmSubscriptionResourceListResult>(new HttpMethod("GET"), new Guid("f34be62f-f215-4bda-8b57-9e8a7a5fd66a"), (object) new
      {
        subscriptionId = subscriptionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
