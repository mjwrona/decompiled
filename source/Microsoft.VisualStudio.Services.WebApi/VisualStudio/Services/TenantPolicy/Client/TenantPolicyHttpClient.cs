// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.Client.TenantPolicyHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.TenantPolicy.Client
{
  [ResourceArea("207403F3-B4CE-459A-B6CD-8042D9D309F0")]
  public class TenantPolicyHttpClient : VssHttpClientBase
  {
    public TenantPolicyHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TenantPolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TenantPolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TenantPolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TenantPolicyHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Policy> GetPolicyAsync(
      string policyName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Policy>(new HttpMethod("GET"), new Guid("9fe0af3a-57a2-4e65-af9b-2c16f0c4c068"), (object) new
      {
        policyName = policyName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task SetPolicyAsync(
      Policy policy,
      string policyName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TenantPolicyHttpClient policyHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9fe0af3a-57a2-4e65-af9b-2c16f0c4c068");
      object obj1 = (object) new{ policyName = policyName };
      HttpContent httpContent = (HttpContent) new ObjectContent<Policy>(policy, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TenantPolicyHttpClient policyHttpClient2 = policyHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await policyHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<Policy> GetPolicyForTenantAsync(
      string policyName,
      Guid tenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Policy>(new HttpMethod("GET"), new Guid("bedf0ada-f7b4-49d4-a38e-2a70f02af09e"), (object) new
      {
        policyName = policyName,
        tenantId = tenantId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PolicyInfo> GetPolicyInfoAsync(
      string policyName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyInfo>(new HttpMethod("GET"), new Guid("656cce1d-f9ce-4125-95c3-d3e419f79390"), (object) new
      {
        policyName = policyName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
