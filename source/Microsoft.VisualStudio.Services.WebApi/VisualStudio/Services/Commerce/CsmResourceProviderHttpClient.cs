// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CsmResourceProviderHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CsmResourceProviderHttpClient : CsmResourceProviderHttpClientBase
  {
    public CsmResourceProviderHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CsmResourceProviderHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CsmResourceProviderHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CsmResourceProviderHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public CsmResourceProviderHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public override Task<AccountResource> Accounts_CreateOrUpdateAsync(
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
      httpContent.Headers.Add("x-ms-client-principal-name", requestData.AccountName);
      httpContent.Headers.Add("x-ms-client-tenant-id", requestData.Upn);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccountResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<AccountResource> Accounts_CreateAsync(
      AccountResourceRequest requestData,
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      string upn,
      string domain,
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
      httpContent.Headers.Add("x-ms-client-principal-name", upn);
      httpContent.Headers.Add("x-ms-client-tenant-id", domain);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccountResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public AccountResource Accounts_Get(
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      try
      {
        return this.Accounts_GetAsync(subscriptionId, resourceGroupName, resourceName, userState, cancellationToken).SyncResult<AccountResource>();
      }
      catch (VssServiceResponseException ex)
      {
        if (ex.HttpStatusCode == HttpStatusCode.NotFound)
          return (AccountResource) null;
        throw;
      }
    }

    public virtual async Task MoveResourcesAsync(
      ResourcesMoveRequest resourcesMoveRequest,
      Guid subscriptionId,
      string resourceGroupName,
      string operationName,
      string upn,
      string domain,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CsmResourceProviderHttpClient providerHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8d9245ee-19a2-45b2-be3e-03234122298e");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId,
        resourceGroupName = resourceGroupName,
        operationName = operationName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ResourcesMoveRequest>(resourcesMoveRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      httpContent.Headers.Add("x-ms-client-principal-name", upn);
      httpContent.Headers.Add("x-ms-client-tenant-id", domain);
      CsmResourceProviderHttpClient providerHttpClient2 = providerHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await providerHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
