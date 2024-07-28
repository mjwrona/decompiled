// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Zeus.BlobCopyRequestHttpClientBase
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

namespace Microsoft.VisualStudio.Services.Zeus
{
  [ResourceArea("{8907fe1c-346a-455b-9ab9-dde883687231}")]
  public abstract class BlobCopyRequestHttpClientBase : VssHttpClientBase
  {
    public BlobCopyRequestHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public BlobCopyRequestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public BlobCopyRequestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public BlobCopyRequestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public BlobCopyRequestHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<HttpResponseMessage> DeleteBlobCopyRequestAsync(
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(new HttpMethod("DELETE"), new Guid("8907fe1c-346a-455b-9ab9-dde883687231"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion("2.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BlobCopyRequest> GetBlobCopyRequestAsync(
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BlobCopyRequest>(new HttpMethod("GET"), new Guid("8907fe1c-346a-455b-9ab9-dde883687231"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion("2.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BlobCopyRequest>> GetBlobCopyRequestsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BlobCopyRequest>>(new HttpMethod("GET"), new Guid("8907fe1c-346a-455b-9ab9-dde883687231"), version: new ApiResourceVersion("2.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BlobCopyRequest> QueueBlobCopyRequestAsync(
      BlobCopyRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8907fe1c-346a-455b-9ab9-dde883687231");
      HttpContent httpContent = (HttpContent) new ObjectContent<BlobCopyRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("2.0-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BlobCopyRequest>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<BlobCopyRequest> UpdateBlobCopyRequestAsync(
      BlobCopyRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("8907fe1c-346a-455b-9ab9-dde883687231");
      HttpContent httpContent = (HttpContent) new ObjectContent<BlobCopyRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("2.0-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BlobCopyRequest>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
