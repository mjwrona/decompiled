// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.Client.IdentityMruHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Identity.Mru.Client
{
  [ResourceArea("FC3682BE-3D6C-427A-87C8-E527B16A1D05")]
  public abstract class IdentityMruHttpClientBase : VssHttpClientBase
  {
    public IdentityMruHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public IdentityMruHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public IdentityMruHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public IdentityMruHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public IdentityMruHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<List<Guid>> GetMruIdentitiesAsync(
      string identityId,
      Guid containerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Guid>>(new HttpMethod("GET"), new Guid("15d952a1-bb4e-436c-88ca-cfe1e9ff3331"), (object) new
      {
        identityId = identityId,
        containerId = containerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task SetMruIdentitiesAsync(
      IEnumerable<Guid> identityIds,
      string identityId,
      Guid containerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityMruHttpClientBase mruHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("15d952a1-bb4e-436c-88ca-cfe1e9ff3331");
      object obj1 = (object) new
      {
        identityId = identityId,
        containerId = containerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Guid>>(identityIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      IdentityMruHttpClientBase mruHttpClientBase2 = mruHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mruHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [Obsolete]
    public virtual async Task UpdateMruIdentitiesAsync(
      MruIdentitiesUpdateData updateData,
      string identityId,
      Guid containerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IdentityMruHttpClientBase mruHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("15d952a1-bb4e-436c-88ca-cfe1e9ff3331");
      object obj1 = (object) new
      {
        identityId = identityId,
        containerId = containerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MruIdentitiesUpdateData>(updateData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      IdentityMruHttpClientBase mruHttpClientBase2 = mruHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await mruHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<Guid>> AddMruIdentitiesAndRemoveInactiveAsync(
      MruIdentitiesUpdateData updateData,
      string identityId,
      Guid containerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c46f9443-1fa7-47d4-8df5-513c0e6c8003");
      object obj1 = (object) new
      {
        action = "AddMruIdentities",
        identityId = identityId,
        containerId = containerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MruIdentitiesUpdateData>(updateData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Guid>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
