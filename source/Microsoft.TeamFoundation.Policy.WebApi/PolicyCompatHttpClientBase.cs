// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.WebApi.PolicyCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.Policy.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E2CB80F-05BD-43A4-BD5A-A4654EDC6268
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Policy.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Policy.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class PolicyCompatHttpClientBase : VssHttpClientBase
  {
    public PolicyCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PolicyCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PolicyCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PolicyCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PolicyCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<List<PolicyConfiguration>> GetPolicyConfigurationsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<PolicyConfiguration>>(new HttpMethod("GET"), new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121"), (object) new
      {
        project = project
      }, new ApiResourceVersion("4.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<PolicyConfiguration>> GetPolicyConfigurationsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<PolicyConfiguration>>(new HttpMethod("GET"), new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121"), (object) new
      {
        project = project
      }, new ApiResourceVersion("4.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public Task<List<PolicyConfiguration>> GetPolicyConfigurationsAsync(
      string project,
      string scope,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(scope))
        keyValuePairList.Add(nameof (scope), scope);
      return this.SendAsync<List<PolicyConfiguration>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public Task<List<PolicyConfiguration>> GetPolicyConfigurationsAsync(
      Guid project,
      string scope,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(scope))
        keyValuePairList.Add(nameof (scope), scope);
      return this.SendAsync<List<PolicyConfiguration>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PolicyConfiguration> CreatePolicyConfigurationAsync(
      PolicyConfiguration configuration,
      string project,
      int? configurationId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object obj1 = (object) new
      {
        project = project,
        configurationId = configurationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PolicyConfiguration>(configuration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PolicyConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<PolicyConfiguration> CreatePolicyConfigurationAsync(
      PolicyConfiguration configuration,
      Guid project,
      int? configurationId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object obj1 = (object) new
      {
        project = project,
        configurationId = configurationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PolicyConfiguration>(configuration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PolicyConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
