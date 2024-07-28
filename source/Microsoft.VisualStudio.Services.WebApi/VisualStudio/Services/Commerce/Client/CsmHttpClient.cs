// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.CsmHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Client
{
  public class CsmHttpClient : CsmHttpClientBase
  {
    public CsmHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CsmHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CsmHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CsmHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CsmHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<AccountResource> Accounts_CreateAsync(
      AccountResourceRequest requestData,
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      Microsoft.VisualStudio.Services.Identity.Identity requestor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.Accounts_CreateAsync(requestData, subscriptionId, resourceGroupName, resourceName, requestor.GetProperty<string>("Account", string.Empty), requestor.GetProperty<string>("Domain", string.Empty), userState, cancellationToken);
    }

    public virtual Task<AccountResource> Accounts_CreateAsync(
      AccountResourceRequest requestData,
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      string principalName,
      string domain,
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
      httpContent.Headers.Add("x-ms-client-principal-name", principalName);
      httpContent.Headers.Add("x-ms-client-tenant-id", domain);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("4.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccountResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Task<AccountResource> Accounts_CreateOrUpdateAsync(
      AccountResourceRequest requestData,
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException("Only the overload of Accounts_CreateAsync that accepts a requestor identity is supported.");
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
  }
}
