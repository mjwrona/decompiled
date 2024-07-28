// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement.FeatureManagementHttpClient
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement
{
  public class FeatureManagementHttpClient : VssHttpClientBase
  {
    public FeatureManagementHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FeatureManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FeatureManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FeatureManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FeatureManagementHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<ContributedFeature> GetFeatureAsync(
      string featureId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ContributedFeature>(new HttpMethod("GET"), new Guid("c4209f25-7a27-41dd-9f04-06080c7b6afd"), (object) new
      {
        featureId = featureId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ContributedFeature>> GetFeaturesAsync(
      string targetContributionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c4209f25-7a27-41dd-9f04-06080c7b6afd");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (targetContributionId != null)
        keyValuePairList.Add(nameof (targetContributionId), targetContributionId);
      return this.SendAsync<List<ContributedFeature>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ContributedFeatureState> GetFeatureStateAsync(
      string featureId,
      string userScope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ContributedFeatureState>(new HttpMethod("GET"), new Guid("98911314-3f9b-4eaf-80e8-83900d8e85d9"), (object) new
      {
        featureId = featureId,
        userScope = userScope
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ContributedFeatureState> SetFeatureStateAsync(
      ContributedFeatureState feature,
      string featureId,
      string userScope,
      string reason = null,
      string reasonCode = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("98911314-3f9b-4eaf-80e8-83900d8e85d9");
      object obj1 = (object) new
      {
        featureId = featureId,
        userScope = userScope
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ContributedFeatureState>(feature, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (reason != null)
        collection.Add(nameof (reason), reason);
      if (reasonCode != null)
        collection.Add(nameof (reasonCode), reasonCode);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ContributedFeatureState>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<ContributedFeatureState> GetFeatureStateForScopeAsync(
      string featureId,
      string userScope,
      string scopeName,
      string scopeValue,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ContributedFeatureState>(new HttpMethod("GET"), new Guid("dd291e43-aa9f-4cee-8465-a93c78e414a4"), (object) new
      {
        featureId = featureId,
        userScope = userScope,
        scopeName = scopeName,
        scopeValue = scopeValue
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ContributedFeatureState> SetFeatureStateForScopeAsync(
      ContributedFeatureState feature,
      string featureId,
      string userScope,
      string scopeName,
      string scopeValue,
      string reason = null,
      string reasonCode = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("dd291e43-aa9f-4cee-8465-a93c78e414a4");
      object obj1 = (object) new
      {
        featureId = featureId,
        userScope = userScope,
        scopeName = scopeName,
        scopeValue = scopeValue
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ContributedFeatureState>(feature, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (reason != null)
        collection.Add(nameof (reason), reason);
      if (reasonCode != null)
        collection.Add(nameof (reasonCode), reasonCode);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ContributedFeatureState>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<ContributedFeatureStateQuery> QueryFeatureStatesAsync(
      ContributedFeatureStateQuery query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2b4486ad-122b-400c-ae65-17b6672c1f9d");
      HttpContent httpContent = (HttpContent) new ObjectContent<ContributedFeatureStateQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ContributedFeatureStateQuery>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ContributedFeatureStateQuery> QueryFeatureStatesForDefaultScopeAsync(
      ContributedFeatureStateQuery query,
      string userScope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3f810f28-03e2-4239-b0bc-788add3005e5");
      object obj1 = (object) new{ userScope = userScope };
      HttpContent httpContent = (HttpContent) new ObjectContent<ContributedFeatureStateQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ContributedFeatureStateQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ContributedFeatureStateQuery> QueryFeatureStatesForNamedScopeAsync(
      ContributedFeatureStateQuery query,
      string userScope,
      string scopeName,
      string scopeValue,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f29e997b-c2da-4d15-8380-765788a1a74c");
      object obj1 = (object) new
      {
        userScope = userScope,
        scopeName = scopeName,
        scopeValue = scopeValue
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ContributedFeatureStateQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ContributedFeatureStateQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
