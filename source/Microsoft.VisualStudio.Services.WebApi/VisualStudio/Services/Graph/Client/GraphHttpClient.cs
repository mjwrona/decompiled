// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [ExcludeFromCodeCoverage]
  [ResourceArea("BB1E7EC9-E901-4B68-999A-DE7012B920F8")]
  [ClientCircuitBreakerSettings(20, 80, MaxConcurrentRequests = 55)]
  public class GraphHttpClient : GraphCompatHttpClientBase
  {
    public GraphHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public GraphHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public GraphHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public GraphHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public GraphHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task DeleteAvatarAsync(
      string subjectDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("801eaf9c-0585-4be8-9cdb-b0efa074de91"), (object) new
      {
        subjectDescriptor = subjectDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Avatar> GetAvatarAsync(
      string subjectDescriptor,
      AvatarSize? size = null,
      string format = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("801eaf9c-0585-4be8-9cdb-b0efa074de91");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (size.HasValue)
        keyValuePairList.Add(nameof (size), size.Value.ToString());
      if (format != null)
        keyValuePairList.Add(nameof (format), format);
      return this.SendAsync<Avatar>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task SetAvatarAsync(
      Avatar avatar,
      string subjectDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphHttpClient graphHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("801eaf9c-0585-4be8-9cdb-b0efa074de91");
      object obj1 = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Avatar>(avatar, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GraphHttpClient graphHttpClient2 = graphHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await graphHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<GraphCachePolicies> GetCachePoliciesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphCachePolicies>(new HttpMethod("GET"), new Guid("beb83272-b415-48e8-ac1e-a9b805760739"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<GraphDescriptorResult> GetDescriptorAsync(
      Guid storageKey,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphDescriptorResult>(new HttpMethod("GET"), new Guid("048aee0a-7072-4cde-ab73-7af77b1e0b4e"), (object) new
      {
        storageKey = storageKey
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<GraphFederatedProviderData> GetFederatedProviderDataAsync(
      SubjectDescriptor subjectDescriptor,
      string providerName,
      long? versionHint = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5dcd28d6-632d-477f-ac6b-398ea9fc2f71");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerName), providerName);
      if (versionHint.HasValue)
        keyValuePairList.Add(nameof (versionHint), versionHint.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<GraphFederatedProviderData>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<GraphGroup> CreateGroupAsync(
      GraphGroupCreationContext creationContext,
      string scopeDescriptor = null,
      IEnumerable<SubjectDescriptor> groupDescriptors = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ebbe6af8-0b91-4c13-8cf1-777c14858188");
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphGroupCreationContext>(creationContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (scopeDescriptor != null)
        collection.Add(nameof (scopeDescriptor), scopeDescriptor);
      if (groupDescriptors != null && groupDescriptors.Any<SubjectDescriptor>())
        collection.Add(nameof (groupDescriptors), string.Join<SubjectDescriptor>(",", groupDescriptors));
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
      return this.SendAsync<GraphGroup>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteGroupAsync(
      string groupDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("ebbe6af8-0b91-4c13-8cf1-777c14858188"), (object) new
      {
        groupDescriptor = groupDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<GraphGroup> GetGroupAsync(
      string groupDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphGroup>(new HttpMethod("GET"), new Guid("ebbe6af8-0b91-4c13-8cf1-777c14858188"), (object) new
      {
        groupDescriptor = groupDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<PagedGraphGroups> ListGroupsAsync(
      string scopeDescriptor = null,
      IEnumerable<string> subjectTypes = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphHttpClient graphHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ebbe6af8-0b91-4c13-8cf1-777c14858188");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopeDescriptor != null)
        keyValuePairList.Add(nameof (scopeDescriptor), scopeDescriptor);
      if (subjectTypes != null && subjectTypes.Any<string>())
        keyValuePairList.Add(nameof (subjectTypes), string.Join(",", subjectTypes));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      PagedGraphGroups pagedGraphGroups1;
      using (HttpRequestMessage requestMessage = await graphHttpClient.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        PagedGraphGroups returnObject = new PagedGraphGroups();
        using (HttpResponseMessage response = await graphHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = graphHttpClient.GetHeaderValue(response, "X-MS-ContinuationToken");
          PagedGraphGroups pagedGraphGroups = returnObject;
          pagedGraphGroups.GraphGroups = (IEnumerable<GraphGroup>) await graphHttpClient.ReadContentAsAsync<List<GraphGroup>>(response, cancellationToken).ConfigureAwait(false);
          pagedGraphGroups = (PagedGraphGroups) null;
        }
        pagedGraphGroups1 = returnObject;
      }
      return pagedGraphGroups1;
    }

    public Task<GraphGroup> UpdateGroupAsync(
      string groupDescriptor,
      JsonPatchDocument patchDocument,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ebbe6af8-0b91-4c13-8cf1-777c14858188");
      object obj1 = (object) new
      {
        groupDescriptor = groupDescriptor
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GraphGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Guid> TranslateAsync(
      string masterId = null,
      string localId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9cacc4da-06e3-474a-a1fa-604dd34a2fa2");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (masterId != null)
        keyValuePairList.Add(nameof (masterId), masterId);
      if (localId != null)
        keyValuePairList.Add(nameof (localId), localId);
      return this.SendAsync<Guid>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<IReadOnlyDictionary<SubjectDescriptor, GraphMember>> LookupMembersAsync(
      GraphSubjectLookup memberLookup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3d74d524-ae3d-4d24-a9a7-f8a5cf82347a");
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphSubjectLookup>(memberLookup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IReadOnlyDictionary<SubjectDescriptor, GraphMember>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<PagedGraphMembers> ListMembersAsync(
      string continuationToken = null,
      IEnumerable<string> subjectTypes = null,
      IEnumerable<string> subjectKinds = null,
      IEnumerable<string> metaTypes = null,
      string scopeDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphHttpClient graphHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8b9ecdb2-b752-485a-8418-cc15cf12ee07");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (subjectTypes != null && subjectTypes.Any<string>())
        keyValuePairList.Add(nameof (subjectTypes), string.Join(",", subjectTypes));
      if (subjectKinds != null && subjectKinds.Any<string>())
        keyValuePairList.Add(nameof (subjectKinds), string.Join(",", subjectKinds));
      if (metaTypes != null && metaTypes.Any<string>())
        keyValuePairList.Add(nameof (metaTypes), string.Join(",", metaTypes));
      if (scopeDescriptor != null)
        keyValuePairList.Add(nameof (scopeDescriptor), scopeDescriptor);
      PagedGraphMembers pagedGraphMembers1;
      using (HttpRequestMessage requestMessage = await graphHttpClient.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        PagedGraphMembers returnObject = new PagedGraphMembers();
        using (HttpResponseMessage response = await graphHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = graphHttpClient.GetHeaderValue(response, "X-MS-ContinuationToken");
          PagedGraphMembers pagedGraphMembers = returnObject;
          pagedGraphMembers.GraphMembers = (IEnumerable<GraphMember>) await graphHttpClient.ReadContentAsAsync<List<GraphMember>>(response, cancellationToken).ConfigureAwait(false);
          pagedGraphMembers = (PagedGraphMembers) null;
        }
        pagedGraphMembers1 = returnObject;
      }
      return pagedGraphMembers1;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<GraphMember> GetMemberByDescriptorAsync(
      string memberDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphMember>(new HttpMethod("GET"), new Guid("b9af63a7-5db6-4af8-aae7-387f775ea9c6"), (object) new
      {
        memberDescriptor = memberDescriptor
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<GraphMembership> AddMembershipAsync(
      string subjectDescriptor,
      string containerDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphMembership>(new HttpMethod("PUT"), new Guid("3fd2e6ca-fb30-443a-b579-95b19ed0934c"), (object) new
      {
        subjectDescriptor = subjectDescriptor,
        containerDescriptor = containerDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<bool> CheckMembershipExistenceAsync(
      string subjectDescriptor,
      string containerDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphHttpClient graphHttpClient = this;
      HttpMethod method = new HttpMethod("HEAD");
      Guid locationId = new Guid("3fd2e6ca-fb30-443a-b579-95b19ed0934c");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor,
        containerDescriptor = containerDescriptor
      };
      try
      {
        HttpResponseMessage httpResponseMessage = await graphHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (VssServiceResponseException ex)
      {
        if (ex.HttpStatusCode == HttpStatusCode.NotFound)
          return false;
        throw;
      }
    }

    public Task<GraphMembership> GetMembershipAsync(
      string subjectDescriptor,
      string containerDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphMembership>(new HttpMethod("GET"), new Guid("3fd2e6ca-fb30-443a-b579-95b19ed0934c"), (object) new
      {
        subjectDescriptor = subjectDescriptor,
        containerDescriptor = containerDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemoveMembershipAsync(
      string subjectDescriptor,
      string containerDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("3fd2e6ca-fb30-443a-b579-95b19ed0934c"), (object) new
      {
        subjectDescriptor = subjectDescriptor,
        containerDescriptor = containerDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<List<GraphMembership>> ListMembershipsAsync(
      string subjectDescriptor,
      GraphTraversalDirection? direction = null,
      int? depth = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e34b6394-6b30-4435-94a9-409a5eef3e31");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (direction.HasValue)
        keyValuePairList.Add(nameof (direction), direction.Value.ToString());
      if (depth.HasValue)
        keyValuePairList.Add(nameof (depth), depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<GraphMembership>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<GraphMembershipState> GetMembershipStateAsync(
      string subjectDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphMembershipState>(new HttpMethod("GET"), new Guid("1ffe5c94-1144-4191-907b-d0211cad36a8"), (object) new
      {
        subjectDescriptor = subjectDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>> LookupMembershipTraversalsAsync(
      GraphSubjectLookup membershipTraversalLookup,
      GraphTraversalDirection? direction = null,
      int? depth = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("5d59d874-746f-4f9b-9459-0e571f1ded8c");
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphSubjectLookup>(membershipTraversalLookup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (direction.HasValue)
        collection.Add(nameof (direction), direction.Value.ToString());
      if (depth.HasValue)
        collection.Add(nameof (depth), depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
      return this.SendAsync<IReadOnlyDictionary<SubjectDescriptor, GraphMembershipTraversal>>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<GraphMembershipTraversal> TraverseMembershipsAsync(
      string subjectDescriptor,
      GraphTraversalDirection? direction = null,
      int? depth = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5d59d874-746f-4f9b-9459-0e571f1ded8c");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (direction.HasValue)
        keyValuePairList.Add(nameof (direction), direction.Value.ToString());
      if (depth.HasValue)
        keyValuePairList.Add(nameof (depth), depth.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<GraphMembershipTraversal>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<GraphProviderInfo> GetProviderInfoAsync(
      string userDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphProviderInfo>(new HttpMethod("GET"), new Guid("1e377995-6fa2-4588-bd64-930186abdcfa"), (object) new
      {
        userDescriptor = userDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RequestAccessAsync(
      JToken jsondocument,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphHttpClient graphHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8d54bf92-8c99-47f2-9972-b21341f1722e");
      HttpContent httpContent = (HttpContent) new ObjectContent<JToken>(jsondocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      GraphHttpClient graphHttpClient2 = graphHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await graphHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<ResolveDisconnectedUsersResponse> ResolveAsync(
      IdentityMappings mappings,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2f0cae3a-74a3-4c40-a13b-974889698e6b");
      HttpContent httpContent = (HttpContent) new ObjectContent<IdentityMappings>(mappings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ResolveDisconnectedUsersResponse>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<GraphScope> CreateScopeAsync(
      GraphScopeCreationContext creationContext,
      string scopeDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("21b5fea7-2513-41d0-af78-b8cdb0f328bb");
      object obj1 = (object) new
      {
        scopeDescriptor = scopeDescriptor
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphScopeCreationContext>(creationContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GraphScope>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task DeleteScopeAsync(
      string scopeDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("21b5fea7-2513-41d0-af78-b8cdb0f328bb"), (object) new
      {
        scopeDescriptor = scopeDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<GraphScope> GetScopeAsync(
      string scopeDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphScope>(new HttpMethod("GET"), new Guid("21b5fea7-2513-41d0-af78-b8cdb0f328bb"), (object) new
      {
        scopeDescriptor = scopeDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task UpdateScopeAsync(
      string scopeDescriptor,
      JsonPatchDocument patchDocument,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphHttpClient graphHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("21b5fea7-2513-41d0-af78-b8cdb0f328bb");
      object obj1 = (object) new
      {
        scopeDescriptor = scopeDescriptor
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      GraphHttpClient graphHttpClient2 = graphHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await graphHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<GraphServicePrincipal> CreateServicePrincipalAsync(
      GraphServicePrincipalCreationContext creationContext,
      IEnumerable<SubjectDescriptor> groupDescriptors = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e1dbb0ae-49cb-4532-95a1-86cd89cfcab4");
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphServicePrincipalCreationContext>(creationContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (groupDescriptors != null && groupDescriptors.Any<SubjectDescriptor>())
        collection.Add(nameof (groupDescriptors), string.Join<SubjectDescriptor>(",", groupDescriptors));
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
      return this.SendAsync<GraphServicePrincipal>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteServicePrincipalAsync(
      string servicePrincipalDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e1dbb0ae-49cb-4532-95a1-86cd89cfcab4"), (object) new
      {
        servicePrincipalDescriptor = servicePrincipalDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<GraphServicePrincipal> GetServicePrincipalAsync(
      string servicePrincipalDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphServicePrincipal>(new HttpMethod("GET"), new Guid("e1dbb0ae-49cb-4532-95a1-86cd89cfcab4"), (object) new
      {
        servicePrincipalDescriptor = servicePrincipalDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<PagedGraphServicePrincipals> ListServicePrincipalsAsync(
      string continuationToken = null,
      string scopeDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphHttpClient graphHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e1dbb0ae-49cb-4532-95a1-86cd89cfcab4");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (scopeDescriptor != null)
        keyValuePairList.Add(nameof (scopeDescriptor), scopeDescriptor);
      PagedGraphServicePrincipals servicePrincipals1;
      using (HttpRequestMessage requestMessage = await graphHttpClient.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        PagedGraphServicePrincipals returnObject = new PagedGraphServicePrincipals();
        using (HttpResponseMessage response = await graphHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = graphHttpClient.GetHeaderValue(response, "X-MS-ContinuationToken");
          PagedGraphServicePrincipals servicePrincipals = returnObject;
          servicePrincipals.GraphServicePrincipals = (IEnumerable<GraphServicePrincipal>) await graphHttpClient.ReadContentAsAsync<List<GraphServicePrincipal>>(response, cancellationToken).ConfigureAwait(false);
          servicePrincipals = (PagedGraphServicePrincipals) null;
        }
        servicePrincipals1 = returnObject;
      }
      return servicePrincipals1;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<GraphServicePrincipal> UpdateServicePrincipalAsync(
      GraphServicePrincipalUpdateContext updateContext,
      string servicePrincipalDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e1dbb0ae-49cb-4532-95a1-86cd89cfcab4");
      object obj1 = (object) new
      {
        servicePrincipalDescriptor = servicePrincipalDescriptor
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphServicePrincipalUpdateContext>(updateContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GraphServicePrincipal>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<GraphStorageKeyResult> GetStorageKeyAsync(
      string subjectDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphStorageKeyResult>(new HttpMethod("GET"), new Guid("eb85f8cc-f0f6-4264-a5b1-ffe2e4d4801f"), (object) new
      {
        subjectDescriptor = subjectDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<IReadOnlyDictionary<SubjectDescriptor, GraphSubject>> LookupSubjectsAsync(
      GraphSubjectLookup subjectLookup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4dd4d168-11f2-48c4-83e8-756fa0de027c");
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphSubjectLookup>(subjectLookup, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IReadOnlyDictionary<SubjectDescriptor, GraphSubject>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<GraphSubject>> QuerySubjectsAsync(
      GraphSubjectQuery subjectQuery,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("05942c89-006a-48ce-bb79-baeb8abf99c6");
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphSubjectQuery>(subjectQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GraphSubject>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<GraphSubject> GetSubjectAsync(
      string subjectDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphSubject>(new HttpMethod("GET"), new Guid("1d44a2ac-4f8a-459e-83c2-1c92626fb9c6"), (object) new
      {
        subjectDescriptor = subjectDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<GraphUser> CreateUserAsync(
      GraphUserCreationContext creationContext,
      IEnumerable<SubjectDescriptor> groupDescriptors = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("005e26ec-6b77-4e4f-a986-b3827bf241f5");
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphUserCreationContext>(creationContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (groupDescriptors != null && groupDescriptors.Any<SubjectDescriptor>())
        collection.Add(nameof (groupDescriptors), string.Join<SubjectDescriptor>(",", groupDescriptors));
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
      return this.SendAsync<GraphUser>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteUserAsync(
      string userDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("005e26ec-6b77-4e4f-a986-b3827bf241f5"), (object) new
      {
        userDescriptor = userDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<GraphUser> GetUserAsync(
      string userDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<GraphUser>(new HttpMethod("GET"), new Guid("005e26ec-6b77-4e4f-a986-b3827bf241f5"), (object) new
      {
        userDescriptor = userDescriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<PagedGraphUsers> ListUsersAsync(
      IEnumerable<string> subjectTypes = null,
      string continuationToken = null,
      string scopeDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphHttpClient graphHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("005e26ec-6b77-4e4f-a986-b3827bf241f5");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (subjectTypes != null && subjectTypes.Any<string>())
        keyValuePairList.Add(nameof (subjectTypes), string.Join(",", subjectTypes));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (scopeDescriptor != null)
        keyValuePairList.Add(nameof (scopeDescriptor), scopeDescriptor);
      PagedGraphUsers pagedGraphUsers1;
      using (HttpRequestMessage requestMessage = await graphHttpClient.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        PagedGraphUsers returnObject = new PagedGraphUsers();
        using (HttpResponseMessage response = await graphHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = graphHttpClient.GetHeaderValue(response, "X-MS-ContinuationToken");
          PagedGraphUsers pagedGraphUsers = returnObject;
          pagedGraphUsers.GraphUsers = (IEnumerable<GraphUser>) await graphHttpClient.ReadContentAsAsync<List<GraphUser>>(response, cancellationToken).ConfigureAwait(false);
          pagedGraphUsers = (PagedGraphUsers) null;
        }
        pagedGraphUsers1 = returnObject;
      }
      return pagedGraphUsers1;
    }

    public Task<GraphUser> UpdateUserAsync(
      GraphUserUpdateContext updateContext,
      string userDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("005e26ec-6b77-4e4f-a986-b3827bf241f5");
      object obj1 = (object) new
      {
        userDescriptor = userDescriptor
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GraphUserUpdateContext>(updateContext, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<GraphUser>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
