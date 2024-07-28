// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FeatureAvailability.WebApi.FeatureAvailabilityHttpClient
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

namespace Microsoft.VisualStudio.Services.FeatureAvailability.WebApi
{
  public class FeatureAvailabilityHttpClient : VssHttpClientBase
  {
    public FeatureAvailabilityHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FeatureAvailabilityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FeatureAvailabilityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FeatureAvailabilityHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FeatureAvailabilityHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<List<FeatureFlag>> GetAllFeatureFlagsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FeatureFlag>>(new HttpMethod("GET"), new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<FeatureFlag>> GetAllFeatureFlagsAsync(
      string userEmail,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userEmail), userEmail);
      return this.SendAsync<List<FeatureFlag>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<FeatureFlag> GetFeatureFlagByNameAsync(
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeatureFlag>(new HttpMethod("GET"), new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c"), (object) new
      {
        name = name
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<FeatureFlag> GetFeatureFlagByNameAsync(
      string name,
      bool checkFeatureExists,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c");
      object routeValues = (object) new{ name = name };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (checkFeatureExists), checkFeatureExists.ToString());
      return this.SendAsync<FeatureFlag>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<FeatureFlag> GetFeatureFlagByNameAndUserEmailAsync(
      string name,
      string userEmail,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c");
      object routeValues = (object) new{ name = name };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userEmail), userEmail);
      return this.SendAsync<FeatureFlag>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<FeatureFlag> GetFeatureFlagByNameAndUserEmailAsync(
      string name,
      string userEmail,
      bool checkFeatureExists,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c");
      object routeValues = (object) new{ name = name };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userEmail), userEmail);
      keyValuePairList.Add(nameof (checkFeatureExists), checkFeatureExists.ToString());
      return this.SendAsync<FeatureFlag>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<FeatureFlag> GetFeatureFlagByNameAndUserIdAsync(
      string name,
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c");
      object routeValues = (object) new{ name = name };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userId), userId.ToString());
      return this.SendAsync<FeatureFlag>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<FeatureFlag> GetFeatureFlagByNameAndUserIdAsync(
      string name,
      Guid userId,
      bool checkFeatureExists,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c");
      object routeValues = (object) new{ name = name };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (userId), userId.ToString());
      keyValuePairList.Add(nameof (checkFeatureExists), checkFeatureExists.ToString());
      return this.SendAsync<FeatureFlag>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<FeatureFlag> UpdateFeatureFlagAsync(
      FeatureFlagPatch state,
      string name,
      bool? checkFeatureExists = null,
      bool? setAtApplicationLevelAlso = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c");
      object obj1 = (object) new{ name = name };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeatureFlagPatch>(state, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (checkFeatureExists.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = checkFeatureExists.Value;
        string str = flag.ToString();
        collection.Add(nameof (checkFeatureExists), str);
      }
      if (setAtApplicationLevelAlso.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = setAtApplicationLevelAlso.Value;
        string str = flag.ToString();
        collection.Add(nameof (setAtApplicationLevelAlso), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeatureFlag>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<FeatureFlag> UpdateFeatureFlagAsync(
      FeatureFlagPatch state,
      string name,
      string userEmail,
      bool? checkFeatureExists = null,
      bool? setAtApplicationLevelAlso = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3e2b80f8-9e6f-441e-8393-005610692d9c");
      object obj1 = (object) new{ name = name };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeatureFlagPatch>(state, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      collection1.Add(nameof (userEmail), userEmail);
      bool flag;
      if (checkFeatureExists.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        flag = checkFeatureExists.Value;
        string str = flag.ToString();
        collection2.Add(nameof (checkFeatureExists), str);
      }
      if (setAtApplicationLevelAlso.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        flag = setAtApplicationLevelAlso.Value;
        string str = flag.ToString();
        collection3.Add(nameof (setAtApplicationLevelAlso), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeatureFlag>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }
  }
}
