// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.Client.OrganizationPolicyHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Organization.Client
{
  public class OrganizationPolicyHttpClient : VssHttpClientBase
  {
    public OrganizationPolicyHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public OrganizationPolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public OrganizationPolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public OrganizationPolicyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public OrganizationPolicyHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<Policy> GetPolicyAsync(
      string policyName,
      string defaultValue,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d0ab077b-1b97-4f78-984c-cfe2d248fc79");
      object routeValues = (object) new
      {
        policyName = policyName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (defaultValue), defaultValue);
      return this.SendAsync<Policy>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdatePolicyAsync(
      JsonPatchDocument patchDocument,
      string policyName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OrganizationPolicyHttpClient policyHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d0ab077b-1b97-4f78-984c-cfe2d248fc79");
      object obj1 = (object) new{ policyName = policyName };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      OrganizationPolicyHttpClient policyHttpClient2 = policyHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await policyHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<Dictionary<string, Policy>> GetPoliciesAsync(
      IEnumerable<string> policyNames,
      IEnumerable<string> defaultValues,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7ef423e0-59d8-4c00-b951-7143b18bd97b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str1 = (string) null;
      if (policyNames != null)
        str1 = string.Join(",", policyNames);
      keyValuePairList.Add(nameof (policyNames), str1);
      string str2 = (string) null;
      if (defaultValues != null)
        str2 = string.Join(",", defaultValues);
      keyValuePairList.Add(nameof (defaultValues), str2);
      return this.SendAsync<Dictionary<string, Policy>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PolicyInfo> GetPolicyInformationAsync(
      string policyName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PolicyInfo>(new HttpMethod("GET"), new Guid("222af71b-7280-4a95-80e4-dcb0deeac834"), (object) new
      {
        policyName = policyName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Dictionary<string, PolicyInfo>> GetPolicyInformationsAsync(
      IEnumerable<string> policyNames = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("222af71b-7280-4a95-80e4-dcb0deeac834");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (policyNames != null && policyNames.Any<string>())
        keyValuePairList.Add(nameof (policyNames), string.Join(",", policyNames));
      return this.SendAsync<Dictionary<string, PolicyInfo>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
