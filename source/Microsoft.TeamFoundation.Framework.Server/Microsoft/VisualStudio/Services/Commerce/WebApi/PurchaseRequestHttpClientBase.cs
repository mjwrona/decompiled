// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.PurchaseRequestHttpClientBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  [ResourceArea("9D439667-F8CF-4991-89A9-95CA6A763327")]
  public abstract class PurchaseRequestHttpClientBase : VssHttpClientBase
  {
    public PurchaseRequestHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PurchaseRequestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PurchaseRequestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PurchaseRequestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PurchaseRequestHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task CreatePurchaseRequestAsync(
      PurchaseRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PurchaseRequestHttpClientBase requestHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("6f905b2d-292a-4d30-b38a-2d254eab06b7");
      HttpContent httpContent = (HttpContent) new ObjectContent<PurchaseRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PurchaseRequestHttpClientBase requestHttpClientBase2 = requestHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await requestHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePurchaseRequestAsync(
      PurchaseRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PurchaseRequestHttpClientBase requestHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6f905b2d-292a-4d30-b38a-2d254eab06b7");
      HttpContent httpContent = (HttpContent) new ObjectContent<PurchaseRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PurchaseRequestHttpClientBase requestHttpClientBase2 = requestHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await requestHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
