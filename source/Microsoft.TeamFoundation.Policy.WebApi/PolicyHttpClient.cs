// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.WebApi.PolicyHttpClient
// Assembly: Microsoft.TeamFoundation.Policy.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E2CB80F-05BD-43A4-BD5A-A4654EDC6268
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Policy.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Policy.WebApi
{
  [ResourceArea("FB13A388-40DD-4A04-B530-013A739C72EF")]
  public class PolicyHttpClient : PolicyHttpClientBase
  {
    public PolicyHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PolicyHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<PolicyConfiguration> CreatePolicyConfigurationAsync(
      PolicyConfiguration configuration,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PolicyConfiguration>(configuration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PolicyConfiguration>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PolicyConfiguration>>) null);
    }

    public Task<PolicyConfiguration> CreatePolicyConfigurationAsync(
      PolicyConfiguration configuration,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PolicyConfiguration>(configuration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PolicyConfiguration>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PolicyConfiguration>>) null);
    }

    public async Task DeletePolicyConfigurationAsync(
      string project,
      int configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121"), (object) new
      {
        project = project,
        configurationId = configurationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeletePolicyConfigurationAsync(
      Guid project,
      int configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121"), (object) new
      {
        project = project,
        configurationId = configurationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<PolicyConfiguration> GetPolicyConfigurationAsync(
      string project,
      int configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyConfiguration>(new HttpMethod("GET"), new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121"), (object) new
      {
        project = project,
        configurationId = configurationId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyConfiguration>>) null);
    }

    public Task<PolicyConfiguration> GetPolicyConfigurationAsync(
      Guid project,
      int configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyConfiguration>(new HttpMethod("GET"), new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121"), (object) new
      {
        project = project,
        configurationId = configurationId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyConfiguration>>) null);
    }

    public Task<List<PolicyConfiguration>> GetPolicyConfigurationsAsync(
      string project,
      string scope = null,
      Guid? policyType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scope != null)
        keyValuePairList.Add(nameof (scope), scope);
      if (policyType.HasValue)
        keyValuePairList.Add(nameof (policyType), policyType.Value.ToString());
      return this.SendAsync<List<PolicyConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<PolicyConfiguration>>>) null);
    }

    public Task<List<PolicyConfiguration>> GetPolicyConfigurationsAsync(
      Guid project,
      string scope = null,
      Guid? policyType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dad91cbe-d183-45f8-9c6e-9c1164472121");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scope != null)
        keyValuePairList.Add(nameof (scope), scope);
      if (policyType.HasValue)
        keyValuePairList.Add(nameof (policyType), policyType.Value.ToString());
      return this.SendAsync<List<PolicyConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<PolicyConfiguration>>>) null);
    }

    public Task<PolicyConfiguration> UpdatePolicyConfigurationAsync(
      PolicyConfiguration configuration,
      string project,
      int configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
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
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PolicyConfiguration>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PolicyConfiguration>>) null);
    }

    public Task<PolicyConfiguration> UpdatePolicyConfigurationAsync(
      PolicyConfiguration configuration,
      Guid project,
      int configurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
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
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PolicyConfiguration>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<PolicyConfiguration>>) null);
    }

    public Task<PolicyEvaluationRecord> GetPolicyEvaluationAsync(
      string project,
      Guid evaluationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyEvaluationRecord>(new HttpMethod("GET"), new Guid("46aecb7a-5d2c-4647-897b-0209505a9fe4"), (object) new
      {
        project = project,
        evaluationId = evaluationId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyEvaluationRecord>>) null);
    }

    public Task<PolicyEvaluationRecord> GetPolicyEvaluationAsync(
      Guid project,
      Guid evaluationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyEvaluationRecord>(new HttpMethod("GET"), new Guid("46aecb7a-5d2c-4647-897b-0209505a9fe4"), (object) new
      {
        project = project,
        evaluationId = evaluationId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyEvaluationRecord>>) null);
    }

    public Task<PolicyEvaluationRecord> RequeuePolicyEvaluationAsync(
      string project,
      Guid evaluationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyEvaluationRecord>(new HttpMethod("PATCH"), new Guid("46aecb7a-5d2c-4647-897b-0209505a9fe4"), (object) new
      {
        project = project,
        evaluationId = evaluationId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyEvaluationRecord>>) null);
    }

    public Task<PolicyEvaluationRecord> RequeuePolicyEvaluationAsync(
      Guid project,
      Guid evaluationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyEvaluationRecord>(new HttpMethod("PATCH"), new Guid("46aecb7a-5d2c-4647-897b-0209505a9fe4"), (object) new
      {
        project = project,
        evaluationId = evaluationId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyEvaluationRecord>>) null);
    }

    public Task<List<PolicyEvaluationRecord>> GetPolicyEvaluationsAsync(
      string project,
      string artifactId,
      bool? includeNotApplicable = null,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c23ddff5-229c-4d04-a80b-0fdce9f360c8");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactId), artifactId);
      if (includeNotApplicable.HasValue)
        keyValuePairList.Add(nameof (includeNotApplicable), includeNotApplicable.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<PolicyEvaluationRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<PolicyEvaluationRecord>>>) null);
    }

    public Task<List<PolicyEvaluationRecord>> GetPolicyEvaluationsAsync(
      Guid project,
      string artifactId,
      bool? includeNotApplicable = null,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c23ddff5-229c-4d04-a80b-0fdce9f360c8");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactId), artifactId);
      if (includeNotApplicable.HasValue)
        keyValuePairList.Add(nameof (includeNotApplicable), includeNotApplicable.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<PolicyEvaluationRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<PolicyEvaluationRecord>>>) null);
    }

    public Task<PolicyConfiguration> GetPolicyConfigurationRevisionAsync(
      string project,
      int configurationId,
      int revisionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyConfiguration>(new HttpMethod("GET"), new Guid("fe1e68a2-60d3-43cb-855b-85e41ae97c95"), (object) new
      {
        project = project,
        configurationId = configurationId,
        revisionId = revisionId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyConfiguration>>) null);
    }

    public Task<PolicyConfiguration> GetPolicyConfigurationRevisionAsync(
      Guid project,
      int configurationId,
      int revisionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyConfiguration>(new HttpMethod("GET"), new Guid("fe1e68a2-60d3-43cb-855b-85e41ae97c95"), (object) new
      {
        project = project,
        configurationId = configurationId,
        revisionId = revisionId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyConfiguration>>) null);
    }

    public Task<List<PolicyConfiguration>> GetPolicyConfigurationRevisionsAsync(
      string project,
      int configurationId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fe1e68a2-60d3-43cb-855b-85e41ae97c95");
      object routeValues = (object) new
      {
        project = project,
        configurationId = configurationId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<PolicyConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<PolicyConfiguration>>>) null);
    }

    public Task<List<PolicyConfiguration>> GetPolicyConfigurationRevisionsAsync(
      Guid project,
      int configurationId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fe1e68a2-60d3-43cb-855b-85e41ae97c95");
      object routeValues = (object) new
      {
        project = project,
        configurationId = configurationId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<PolicyConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<PolicyConfiguration>>>) null);
    }

    public Task<PolicyType> GetPolicyTypeAsync(
      string project,
      Guid typeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyType>(new HttpMethod("GET"), new Guid("44096322-2d3d-466a-bb30-d1b7de69f61f"), (object) new
      {
        project = project,
        typeId = typeId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyType>>) null);
    }

    public Task<PolicyType> GetPolicyTypeAsync(
      Guid project,
      Guid typeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyType>(new HttpMethod("GET"), new Guid("44096322-2d3d-466a-bb30-d1b7de69f61f"), (object) new
      {
        project = project,
        typeId = typeId
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<PolicyType>>) null);
    }

    public Task<List<PolicyType>> GetPolicyTypesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<PolicyType>>(new HttpMethod("GET"), new Guid("44096322-2d3d-466a-bb30-d1b7de69f61f"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<PolicyType>>>) null);
    }

    public Task<List<PolicyType>> GetPolicyTypesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<PolicyType>>(new HttpMethod("GET"), new Guid("44096322-2d3d-466a-bb30-d1b7de69f61f"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<PolicyType>>>) null);
    }
  }
}
