// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.VisitsHttpClient
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [ResourceArea("D69BCC31-8EB7-42A6-B1B8-B52E91062597")]
  public class VisitsHttpClient : VssHttpClientBase
  {
    public VisitsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public VisitsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public VisitsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public VisitsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public VisitsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<List<ArtifactStats>> GetStatsAsync(
      IEnumerable<ArtifactStats> stats,
      bool? includeUpdatesSinceLastVisit = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2d358c96-88cc-42ba-9b5d-a2cb26c64972");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ArtifactStats>>(stats, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (includeUpdatesSinceLastVisit.HasValue)
        collection.Add(nameof (includeUpdatesSinceLastVisit), includeUpdatesSinceLastVisit.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ArtifactStats>>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ArtifactVisit> UpdateLastVisitAsync(
      ArtifactVisit visit,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c4bc78ab-8d09-4b62-98f2-efb1affe50f8");
      HttpContent httpContent = (HttpContent) new ObjectContent<ArtifactVisit>(visit, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ArtifactVisit>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<ArtifactVisit>> GetVisitsAsync(
      IEnumerable<ArtifactVisit> visits,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d1786677-7a19-445b-9a7a-25728f48d149");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ArtifactVisit>>(visits, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ArtifactVisit>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
