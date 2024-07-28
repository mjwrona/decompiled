// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.WebApi.TFSAnalyticsHttpClient
// Assembly: Microsoft.TeamFoundation.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 055A63EC-DEB5-4484-8793-E8DBCC9AC203
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Analytics.WebApi
{
  [ResourceArea("{7765C886-D562-4D12-A581-BB47C80434E1}")]
  [ClientCancellationTimeout(10)]
  [ClientCircuitBreakerSettings(5, 80)]
  public class TFSAnalyticsHttpClient : VssHttpClientBase
  {
    public TFSAnalyticsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TFSAnalyticsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TFSAnalyticsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TFSAnalyticsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TFSAnalyticsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task UpdateAnalyticsStateAsync(
      AnalyticsState state,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TFSAnalyticsHttpClient analyticsHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c7e05427-3711-440d-91f7-59ecdc9cd6e2");
      HttpContent httpContent = (HttpContent) new ObjectContent<AnalyticsState>(state, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TFSAnalyticsHttpClient analyticsHttpClient2 = analyticsHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await analyticsHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
