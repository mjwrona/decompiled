// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.WikiCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class WikiCompatHttpClientBase : VssHttpClientBase
  {
    public WikiCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WikiCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WikiCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WikiCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WikiCompatHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<Microsoft.TeamFoundation.Wiki.WebApi.Wiki> CreateWikiAsync(
      WikiCreateParameters wikiCreateParams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiCreateParameters>(wikiCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("4.1-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Microsoft.TeamFoundation.Wiki.WebApi.Wiki> CreateWikiAsync(
      WikiCreateParameters wikiCreateParams,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiCreateParameters>(wikiCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("4.1-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Microsoft.TeamFoundation.Wiki.WebApi.Wiki> CreateWikiAsync(
      WikiCreateParameters wikiCreateParams,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<WikiCreateParameters>(wikiCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("4.1-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>> GetWikisAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), version: new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>> GetWikisAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>> GetWikisAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Microsoft.TeamFoundation.Wiki.WebApi.Wiki>>(new HttpMethod("GET"), new Guid("288d122c-dbd4-451d-aa5f-7dbbba070728"), (object) new
      {
        project = project
      }, new ApiResourceVersion("4.1-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
