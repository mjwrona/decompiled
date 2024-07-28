// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupLicensingRule.LicensingRuleHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.GroupLicensingRule
{
  [ResourceArea("4F9A6C65-A750-4DE3-96D3-E4BCCF3A39B0")]
  public class LicensingRuleHttpClient : VssHttpClientBase
  {
    public LicensingRuleHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public LicensingRuleHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public LicensingRuleHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public LicensingRuleHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public LicensingRuleHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<OperationReference> AddGroupLicensingRuleAsync(
      Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule licensingRule,
      RuleOption? ruleOption = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("1dae9af4-c85d-411b-b0c1-a46afaea1986");
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>(licensingRule, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (ruleOption.HasValue)
        collection.Add(nameof (ruleOption), ruleOption.Value.ToString());
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
      return this.SendAsync<OperationReference>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<OperationReference> DeleteGroupLicenseRuleAsync(
      string subjectDescriptor,
      RuleOption? ruleOption = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("1dae9af4-c85d-411b-b0c1-a46afaea1986");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (ruleOption.HasValue)
        keyValuePairList.Add(nameof (ruleOption), ruleOption.Value.ToString());
      return this.SendAsync<OperationReference>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule> GetGroupLicensingRuleAsync(
      string subjectDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>(new HttpMethod("GET"), new Guid("1dae9af4-c85d-411b-b0c1-a46afaea1986"), (object) new
      {
        subjectDescriptor = subjectDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>> GetGroupLicensingRulesAsync(
      int top,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1dae9af4-c85d-411b-b0c1-a46afaea1986");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (top), top.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (skip.HasValue)
        keyValuePairList.Add(nameof (skip), skip.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<OperationReference> UpdateGroupLicensingRuleAsync(
      GroupLicensingRuleUpdate licensingRuleUpdate,
      RuleOption? ruleOption = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1dae9af4-c85d-411b-b0c1-a46afaea1986");
      HttpContent httpContent = (HttpContent) new ObjectContent<GroupLicensingRuleUpdate>(licensingRuleUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (ruleOption.HasValue)
        collection.Add(nameof (ruleOption), ruleOption.Value.ToString());
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
      return this.SendAsync<OperationReference>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<OperationReference> ApplyGroupLicensingRulesToAllUsersAsync(
      RuleOption? ruleOption = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("14602853-288e-4711-a613-c3f27ffce285");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (ruleOption.HasValue)
        keyValuePairList.Add(nameof (ruleOption), ruleOption.Value.ToString());
      return this.SendAsync<OperationReference>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ApplicationStatus> GetApplicationStatusAsync(
      Guid? operationId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ApplicationStatus>(new HttpMethod("GET"), new Guid("8953c613-d07f-43d3-a7bd-e9b66f960839"), (object) new
      {
        operationId = operationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<string> RetrieveFileAsync(
      int fileId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c3c87024-5143-4631-94ce-cb2338b04bbc");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (fileId), fileId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<string>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>> LookupGroupLicensingRulesAsync(
      GraphSubjectLookup groupRuleLookup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6282b958-792b-4f26-b5c8-6d035e02289f");
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphSubjectLookup>(groupRuleLookup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task ApplyGroupLicensingRulesToUserAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("74a9de62-9afc-4a60-a6d9-f7c65e028619"), (object) new
      {
        userId = userId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task RemoveDirectAssignmentAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("74a9de62-9afc-4a60-a6d9-f7c65e028619"), (object) new
      {
        userId = userId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
