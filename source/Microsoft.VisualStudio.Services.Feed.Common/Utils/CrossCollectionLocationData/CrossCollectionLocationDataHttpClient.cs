// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.Utils.CrossCollectionLocationData.CrossCollectionLocationDataHttpClient
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.Utils.CrossCollectionLocationData
{
  public class CrossCollectionLocationDataHttpClient : VssHttpClientBase
  {
    public CrossCollectionLocationDataHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CrossCollectionLocationDataHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CrossCollectionLocationDataHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CrossCollectionLocationDataHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CrossCollectionLocationDataHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<IEnumerable<ApiResourceLocation>> GetResourceLocationsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CrossCollectionLocationDataHttpClient locationDataHttpClient = this;
      IEnumerable<ApiResourceLocation> resourceLocationsAsync;
      // ISSUE: explicit non-virtual call
      using (HttpRequestMessage optionsRequest = new HttpRequestMessage(HttpMethod.Options, VssHttpUriUtility.ConcatUri(__nonvirtual (locationDataHttpClient.BaseAddress), "_apis/")))
        resourceLocationsAsync = await locationDataHttpClient.SendAsync<IEnumerable<ApiResourceLocation>>(optionsRequest, userState, cancellationToken).ConfigureAwait(false);
      return resourceLocationsAsync;
    }
  }
}
