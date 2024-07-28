// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ReparentCollectionHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [ResourceArea("{45A344C2-967D-4353-953D-DDA8B88ECA08}")]
  public class ReparentCollectionHttpClient : VssHttpClientBase
  {
    public ReparentCollectionHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ReparentCollectionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ReparentCollectionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ReparentCollectionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ReparentCollectionHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task CancelRequestAsync(
      Guid requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("7ce8bc16-d4d6-4d42-9ad1-d9f57fa82591"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<ServicingOrchestrationRequestStatus> GetRequestStatusAsync(
      Guid requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ServicingOrchestrationRequestStatus>(new HttpMethod("GET"), new Guid("7ce8bc16-d4d6-4d42-9ad1-d9f57fa82591"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task QueueRequestAsync(
      FrameworkReparentCollectionRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReparentCollectionHttpClient collectionHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("7ce8bc16-d4d6-4d42-9ad1-d9f57fa82591");
      HttpContent httpContent = (HttpContent) new ObjectContent<FrameworkReparentCollectionRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ReparentCollectionHttpClient collectionHttpClient2 = collectionHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await collectionHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task ValidateRequestAsync(
      FrameworkReparentCollectionRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReparentCollectionHttpClient collectionHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7ce8bc16-d4d6-4d42-9ad1-d9f57fa82591");
      HttpContent httpContent = (HttpContent) new ObjectContent<FrameworkReparentCollectionRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ReparentCollectionHttpClient collectionHttpClient2 = collectionHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await collectionHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
