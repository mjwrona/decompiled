// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients.ReleaseHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients
{
  [ResourceArea("efc2f575-36ef-48e9-b672-0c6fb4a48ac5")]
  public abstract class ReleaseHttpClientBase : ReleaseCompatHttpClientBase
  {
    public ReleaseHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ReleaseHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ReleaseHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ReleaseHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ReleaseHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AgentArtifactDefinition>> GetAgentArtifactDefinitionsAsync(
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AgentArtifactDefinition>>(new HttpMethod("GET"), new Guid("f2571c27-bf50-4938-b396-32d109ddef26"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AgentArtifactDefinition>> GetAgentArtifactDefinitionsAsync(
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AgentArtifactDefinition>>(new HttpMethod("GET"), new Guid("f2571c27-bf50-4938-b396-32d109ddef26"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ReleaseApproval>> GetApprovalsAsync(
      string project,
      string assignedToFilter = null,
      ApprovalStatus? statusFilter = null,
      IEnumerable<int> releaseIdsFilter = null,
      ApprovalType? typeFilter = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseQueryOrder? queryOrder = null,
      bool? includeMyGroupApprovals = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b47c6458-e73b-47cb-a770-4df1e8813a91");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (assignedToFilter != null)
        keyValuePairList.Add(nameof (assignedToFilter), assignedToFilter);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (releaseIdsFilter != null && releaseIdsFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdsFilter), string.Join<int>(",", releaseIdsFilter));
      if (typeFilter.HasValue)
        keyValuePairList.Add(nameof (typeFilter), typeFilter.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (includeMyGroupApprovals.HasValue)
        keyValuePairList.Add(nameof (includeMyGroupApprovals), includeMyGroupApprovals.Value.ToString());
      return this.SendAsync<List<ReleaseApproval>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ReleaseApproval>> GetApprovalsAsync(
      Guid project,
      string assignedToFilter = null,
      ApprovalStatus? statusFilter = null,
      IEnumerable<int> releaseIdsFilter = null,
      ApprovalType? typeFilter = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseQueryOrder? queryOrder = null,
      bool? includeMyGroupApprovals = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b47c6458-e73b-47cb-a770-4df1e8813a91");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (assignedToFilter != null)
        keyValuePairList.Add(nameof (assignedToFilter), assignedToFilter);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (releaseIdsFilter != null && releaseIdsFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdsFilter), string.Join<int>(",", releaseIdsFilter));
      if (typeFilter.HasValue)
        keyValuePairList.Add(nameof (typeFilter), typeFilter.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (includeMyGroupApprovals.HasValue)
        keyValuePairList.Add(nameof (includeMyGroupApprovals), includeMyGroupApprovals.Value.ToString());
      return this.SendAsync<List<ReleaseApproval>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseApproval> GetApprovalHistoryAsync(
      string project,
      int approvalStepId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ReleaseApproval>(new HttpMethod("GET"), new Guid("250c7158-852e-4130-a00f-a0cce9b72d05"), (object) new
      {
        project = project,
        approvalStepId = approvalStepId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseApproval> GetApprovalHistoryAsync(
      Guid project,
      int approvalStepId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ReleaseApproval>(new HttpMethod("GET"), new Guid("250c7158-852e-4130-a00f-a0cce9b72d05"), (object) new
      {
        project = project,
        approvalStepId = approvalStepId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseApproval> GetApprovalAsync(
      string project,
      int approvalId,
      bool? includeHistory = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9328e074-59fb-465a-89d9-b09c82ee5109");
      object routeValues = (object) new
      {
        project = project,
        approvalId = approvalId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeHistory.HasValue)
        keyValuePairList.Add(nameof (includeHistory), includeHistory.Value.ToString());
      return this.SendAsync<ReleaseApproval>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseApproval> GetApprovalAsync(
      Guid project,
      int approvalId,
      bool? includeHistory = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9328e074-59fb-465a-89d9-b09c82ee5109");
      object routeValues = (object) new
      {
        project = project,
        approvalId = approvalId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeHistory.HasValue)
        keyValuePairList.Add(nameof (includeHistory), includeHistory.Value.ToString());
      return this.SendAsync<ReleaseApproval>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReleaseApproval> UpdateReleaseApprovalAsync(
      ReleaseApproval approval,
      string project,
      int approvalId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9328e074-59fb-465a-89d9-b09c82ee5109");
      object obj1 = (object) new
      {
        project = project,
        approvalId = approvalId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseApproval>(approval, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseApproval>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReleaseApproval> UpdateReleaseApprovalAsync(
      ReleaseApproval approval,
      Guid project,
      int approvalId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9328e074-59fb-465a-89d9-b09c82ee5109");
      object obj1 = (object) new
      {
        project = project,
        approvalId = approvalId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseApproval>(approval, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseApproval>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseApproval>> UpdateReleaseApprovalsAsync(
      IEnumerable<ReleaseApproval> approvals,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c957584a-82aa-4131-8222-6d47f78bfa7a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ReleaseApproval>>(approvals, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ReleaseApproval>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseApproval>> UpdateReleaseApprovalsAsync(
      IEnumerable<ReleaseApproval> approvals,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c957584a-82aa-4131-8222-6d47f78bfa7a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ReleaseApproval>>(approvals, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<ReleaseApproval>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete("GetTaskAttachmentContent API is deprecated. Use GetReleaseTaskAttachmentContent API instead.")]
    public virtual async Task<Stream> GetTaskAttachmentContentAsync(
      string project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c4071f6d-3697-46ca-858e-8b10ff09e52f");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [Obsolete("GetTaskAttachmentContent API is deprecated. Use GetReleaseTaskAttachmentContent API instead.")]
    public virtual async Task<Stream> GetTaskAttachmentContentAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c4071f6d-3697-46ca-858e-8b10ff09e52f");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetReleaseTaskAttachmentContentAsync(
      string project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("60b86efb-7b8c-4853-8f9f-aa142b77b479");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetReleaseTaskAttachmentContentAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("60b86efb-7b8c-4853-8f9f-aa142b77b479");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [Obsolete("GetTaskAttachments API is deprecated. Use GetReleaseTaskAttachments API instead.")]
    public virtual Task<List<ReleaseTaskAttachment>> GetTaskAttachmentsAsync(
      string project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseTaskAttachment>>(new HttpMethod("GET"), new Guid("214111ee-2415-4df2-8ed2-74417f7d61f9"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete("GetTaskAttachments API is deprecated. Use GetReleaseTaskAttachments API instead.")]
    public virtual Task<List<ReleaseTaskAttachment>> GetTaskAttachmentsAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseTaskAttachment>>(new HttpMethod("GET"), new Guid("214111ee-2415-4df2-8ed2-74417f7d61f9"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ReleaseTaskAttachment>> GetReleaseTaskAttachmentsAsync(
      string project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid planId,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseTaskAttachment>>(new HttpMethod("GET"), new Guid("a4d06688-0dfa-4895-82a5-f43ec9452306"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        planId = planId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ReleaseTaskAttachment>> GetReleaseTaskAttachmentsAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid planId,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseTaskAttachment>>(new HttpMethod("GET"), new Guid("a4d06688-0dfa-4895-82a5-f43ec9452306"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        planId = planId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AutoTriggerIssue>> GetAutoTriggerIssuesAsync(
      string project,
      string artifactType,
      string sourceId,
      string artifactVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c1a68497-69da-40fb-9423-cab19cfeeca9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (sourceId), sourceId);
      keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      return this.SendAsync<List<AutoTriggerIssue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AutoTriggerIssue>> GetAutoTriggerIssuesAsync(
      Guid project,
      string artifactType,
      string sourceId,
      string artifactVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c1a68497-69da-40fb-9423-cab19cfeeca9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (sourceId), sourceId);
      keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      return this.SendAsync<List<AutoTriggerIssue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AutoTriggerIssue>> GetAutoTriggerIssuesAsync(
      string artifactType,
      string sourceId,
      string artifactVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c1a68497-69da-40fb-9423-cab19cfeeca9");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (sourceId), sourceId);
      keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      return this.SendAsync<List<AutoTriggerIssue>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<string> GetDeploymentBadgeAsync(
      Guid projectId,
      int releaseDefinitionId,
      int environmentId,
      string branchName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("GET"), new Guid("1a60a35d-b8c9-45fb-bf67-da0829711147"), (object) new
      {
        projectId = projectId,
        releaseDefinitionId = releaseDefinitionId,
        environmentId = environmentId,
        branchName = branchName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Change>> GetReleaseChangesAsync(
      string project,
      int releaseId,
      int? baseReleaseId = null,
      int? top = null,
      string artifactAlias = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8dcf9fe9-ca37-4113-8ee1-37928e98407c");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (baseReleaseId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = baseReleaseId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (baseReleaseId), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (artifactAlias != null)
        keyValuePairList.Add(nameof (artifactAlias), artifactAlias);
      return this.SendAsync<List<Change>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Change>> GetReleaseChangesAsync(
      Guid project,
      int releaseId,
      int? baseReleaseId = null,
      int? top = null,
      string artifactAlias = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8dcf9fe9-ca37-4113-8ee1-37928e98407c");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (baseReleaseId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = baseReleaseId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (baseReleaseId), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (artifactAlias != null)
        keyValuePairList.Add(nameof (artifactAlias), artifactAlias);
      return this.SendAsync<List<Change>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DefinitionEnvironmentReference>> GetDefinitionEnvironmentsAsync(
      string project,
      Guid? taskGroupId = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("12b5d21a-f54c-430e-a8c1-7515d196890e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (taskGroupId.HasValue)
        keyValuePairList.Add(nameof (taskGroupId), taskGroupId.Value.ToString());
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<List<DefinitionEnvironmentReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<DefinitionEnvironmentReference>> GetDefinitionEnvironmentsAsync(
      Guid project,
      Guid? taskGroupId = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("12b5d21a-f54c-430e-a8c1-7515d196890e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (taskGroupId.HasValue)
        keyValuePairList.Add(nameof (taskGroupId), taskGroupId.Value.ToString());
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<List<DefinitionEnvironmentReference>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReleaseDefinition> CreateReleaseDefinitionAsync(
      ReleaseDefinition releaseDefinition,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseDefinition>(releaseDefinition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReleaseDefinition> CreateReleaseDefinitionAsync(
      ReleaseDefinition releaseDefinition,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseDefinition>(releaseDefinition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteReleaseDefinitionAsync(
      string project,
      int definitionId,
      string comment = null,
      bool? forceDelete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (forceDelete.HasValue)
        keyValuePairList.Add(nameof (forceDelete), forceDelete.Value.ToString());
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteReleaseDefinitionAsync(
      Guid project,
      int definitionId,
      string comment = null,
      bool? forceDelete = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      if (forceDelete.HasValue)
        keyValuePairList.Add(nameof (forceDelete), forceDelete.Value.ToString());
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<ReleaseDefinition> GetReleaseDefinitionAsync(
      string project,
      int definitionId,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<ReleaseDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReleaseDefinition> GetReleaseDefinitionAsync(
      Guid project,
      int definitionId,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<ReleaseDefinition>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetReleaseDefinitionRevisionAsync(
      string project,
      int definitionId,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (revision), revision.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetReleaseDefinitionRevisionAsync(
      Guid project,
      int definitionId,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (revision), revision.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      string project,
      string searchText = null,
      ReleaseDefinitionExpands? expand = null,
      string artifactType = null,
      string artifactSourceId = null,
      int? top = null,
      string continuationToken = null,
      ReleaseDefinitionQueryOrder? queryOrder = null,
      string path = null,
      bool? isExactNameMatch = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<string> definitionIdFilter = null,
      bool? isDeleted = null,
      bool? searchTextContainsFolderName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactType != null)
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (artifactSourceId != null)
        keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      bool flag;
      if (isExactNameMatch.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isExactNameMatch.Value;
        string str = flag.ToString();
        collection.Add(nameof (isExactNameMatch), str);
      }
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (definitionIdFilter != null && definitionIdFilter.Any<string>())
        keyValuePairList.Add(nameof (definitionIdFilter), string.Join(",", definitionIdFilter));
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      if (searchTextContainsFolderName.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = searchTextContainsFolderName.Value;
        string str = flag.ToString();
        collection.Add(nameof (searchTextContainsFolderName), str);
      }
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      Guid project,
      string searchText = null,
      ReleaseDefinitionExpands? expand = null,
      string artifactType = null,
      string artifactSourceId = null,
      int? top = null,
      string continuationToken = null,
      ReleaseDefinitionQueryOrder? queryOrder = null,
      string path = null,
      bool? isExactNameMatch = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<string> definitionIdFilter = null,
      bool? isDeleted = null,
      bool? searchTextContainsFolderName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactType != null)
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (artifactSourceId != null)
        keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      bool flag;
      if (isExactNameMatch.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isExactNameMatch.Value;
        string str = flag.ToString();
        collection.Add(nameof (isExactNameMatch), str);
      }
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (definitionIdFilter != null && definitionIdFilter.Any<string>())
        keyValuePairList.Add(nameof (definitionIdFilter), string.Join(",", definitionIdFilter));
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      if (searchTextContainsFolderName.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = searchTextContainsFolderName.Value;
        string str = flag.ToString();
        collection.Add(nameof (searchTextContainsFolderName), str);
      }
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinition> UndeleteReleaseDefinitionAsync(
      ReleaseDefinitionUndeleteParameter releaseDefinitionUndeleteParameter,
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseDefinitionUndeleteParameter>(releaseDefinitionUndeleteParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinition> UndeleteReleaseDefinitionAsync(
      ReleaseDefinitionUndeleteParameter releaseDefinitionUndeleteParameter,
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object obj1 = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseDefinitionUndeleteParameter>(releaseDefinitionUndeleteParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReleaseDefinition> UpdateReleaseDefinitionAsync(
      ReleaseDefinition releaseDefinition,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseDefinition>(releaseDefinition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReleaseDefinition> UpdateReleaseDefinitionAsync(
      ReleaseDefinition releaseDefinition,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseDefinition>(releaseDefinition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseDefinition>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<Deployment>> GetDeploymentsAsync(
      string project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string createdBy = null,
      DateTime? minModifiedTime = null,
      DateTime? maxModifiedTime = null,
      DeploymentStatus? deploymentStatus = null,
      DeploymentOperationStatus? operationStatus = null,
      bool? latestAttemptsOnly = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      string createdFor = null,
      DateTime? minStartedTime = null,
      DateTime? maxStartedTime = null,
      string sourceBranch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b005ef73-cddc-448e-9ba2-5193bf36b19f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (createdBy != null)
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (minModifiedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minModifiedTime), minModifiedTime.Value);
      if (maxModifiedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxModifiedTime), maxModifiedTime.Value);
      if (deploymentStatus.HasValue)
        keyValuePairList.Add(nameof (deploymentStatus), deploymentStatus.Value.ToString());
      if (operationStatus.HasValue)
        keyValuePairList.Add(nameof (operationStatus), operationStatus.Value.ToString());
      if (latestAttemptsOnly.HasValue)
        keyValuePairList.Add(nameof (latestAttemptsOnly), latestAttemptsOnly.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (createdFor != null)
        keyValuePairList.Add(nameof (createdFor), createdFor);
      if (minStartedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minStartedTime), minStartedTime.Value);
      if (maxStartedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxStartedTime), maxStartedTime.Value);
      if (sourceBranch != null)
        keyValuePairList.Add(nameof (sourceBranch), sourceBranch);
      return this.SendAsync<List<Deployment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Deployment>> GetDeploymentsAsync(
      Guid project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string createdBy = null,
      DateTime? minModifiedTime = null,
      DateTime? maxModifiedTime = null,
      DeploymentStatus? deploymentStatus = null,
      DeploymentOperationStatus? operationStatus = null,
      bool? latestAttemptsOnly = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      string createdFor = null,
      DateTime? minStartedTime = null,
      DateTime? maxStartedTime = null,
      string sourceBranch = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b005ef73-cddc-448e-9ba2-5193bf36b19f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (createdBy != null)
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (minModifiedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minModifiedTime), minModifiedTime.Value);
      if (maxModifiedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxModifiedTime), maxModifiedTime.Value);
      if (deploymentStatus.HasValue)
        keyValuePairList.Add(nameof (deploymentStatus), deploymentStatus.Value.ToString());
      if (operationStatus.HasValue)
        keyValuePairList.Add(nameof (operationStatus), operationStatus.Value.ToString());
      if (latestAttemptsOnly.HasValue)
        keyValuePairList.Add(nameof (latestAttemptsOnly), latestAttemptsOnly.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (createdFor != null)
        keyValuePairList.Add(nameof (createdFor), createdFor);
      if (minStartedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minStartedTime), minStartedTime.Value);
      if (maxStartedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxStartedTime), maxStartedTime.Value);
      if (sourceBranch != null)
        keyValuePairList.Add(nameof (sourceBranch), sourceBranch);
      return this.SendAsync<List<Deployment>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Deployment>> GetDeploymentsForMultipleEnvironmentsAsync(
      DeploymentQueryParameters queryParameters,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b005ef73-cddc-448e-9ba2-5193bf36b19f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentQueryParameters>(queryParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Deployment>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Deployment>> GetDeploymentsForMultipleEnvironmentsAsync(
      DeploymentQueryParameters queryParameters,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b005ef73-cddc-448e-9ba2-5193bf36b19f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentQueryParameters>(queryParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Deployment>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReleaseEnvironment> GetReleaseEnvironmentAsync(
      string project,
      int releaseId,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ReleaseEnvironment>(new HttpMethod("GET"), new Guid("a7e426b1-03dc-48af-9dfe-c98bac612dcb"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 7), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReleaseEnvironment> GetReleaseEnvironmentAsync(
      Guid project,
      int releaseId,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ReleaseEnvironment>(new HttpMethod("GET"), new Guid("a7e426b1-03dc-48af-9dfe-c98bac612dcb"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 7), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReleaseEnvironment> GetReleaseEnvironmentAsync(
      string project,
      int releaseId,
      int environmentId,
      ReleaseEnvironmentExpands expand,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a7e426b1-03dc-48af-9dfe-c98bac612dcb");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$expand", expand.ToString());
      return this.SendAsync<ReleaseEnvironment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReleaseEnvironment> GetReleaseEnvironmentAsync(
      Guid project,
      int releaseId,
      int environmentId,
      ReleaseEnvironmentExpands expand,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a7e426b1-03dc-48af-9dfe-c98bac612dcb");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$expand", expand.ToString());
      return this.SendAsync<ReleaseEnvironment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 7), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ReleaseEnvironment> UpdateReleaseEnvironmentAsync(
      ReleaseEnvironmentUpdateMetadata environmentUpdateData,
      string project,
      int releaseId,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a7e426b1-03dc-48af-9dfe-c98bac612dcb");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseEnvironmentUpdateMetadata>(environmentUpdateData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseEnvironment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReleaseEnvironment> UpdateReleaseEnvironmentAsync(
      ReleaseEnvironmentUpdateMetadata environmentUpdateData,
      Guid project,
      int releaseId,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a7e426b1-03dc-48af-9dfe-c98bac612dcb");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseEnvironmentUpdateMetadata>(environmentUpdateData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 7);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseEnvironment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinitionEnvironmentTemplate> CreateDefinitionEnvironmentTemplateAsync(
      ReleaseDefinitionEnvironmentTemplate template,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseDefinitionEnvironmentTemplate>(template, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseDefinitionEnvironmentTemplate>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinitionEnvironmentTemplate> CreateDefinitionEnvironmentTemplateAsync(
      ReleaseDefinitionEnvironmentTemplate template,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseDefinitionEnvironmentTemplate>(template, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseDefinitionEnvironmentTemplate>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteDefinitionEnvironmentTemplateAsync(
      string project,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (templateId), templateId.ToString());
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteDefinitionEnvironmentTemplateAsync(
      Guid project,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (templateId), templateId.ToString());
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinitionEnvironmentTemplate> GetDefinitionEnvironmentTemplateAsync(
      string project,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (templateId), templateId.ToString());
      return this.SendAsync<ReleaseDefinitionEnvironmentTemplate>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinitionEnvironmentTemplate> GetDefinitionEnvironmentTemplateAsync(
      Guid project,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (templateId), templateId.ToString());
      return this.SendAsync<ReleaseDefinitionEnvironmentTemplate>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseDefinitionEnvironmentTemplate>> ListDefinitionEnvironmentTemplatesAsync(
      string project,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      return this.SendAsync<List<ReleaseDefinitionEnvironmentTemplate>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseDefinitionEnvironmentTemplate>> ListDefinitionEnvironmentTemplatesAsync(
      Guid project,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      return this.SendAsync<List<ReleaseDefinitionEnvironmentTemplate>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinitionEnvironmentTemplate> UndeleteReleaseDefinitionEnvironmentTemplateAsync(
      string project,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (templateId), templateId.ToString());
      return this.SendAsync<ReleaseDefinitionEnvironmentTemplate>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinitionEnvironmentTemplate> UndeleteReleaseDefinitionEnvironmentTemplateAsync(
      Guid project,
      Guid templateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("6b03b696-824e-4479-8eb2-6644a51aba89");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (templateId), templateId.ToString());
      return this.SendAsync<ReleaseDefinitionEnvironmentTemplate>(method, locationId, routeValues, new ApiResourceVersion(7.2, 4), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FavoriteItem>> CreateFavoritesAsync(
      IEnumerable<FavoriteItem> favoriteItems,
      string project,
      string scope,
      string identityId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("938f7222-9acb-48fe-b8a3-4eda04597171");
      object obj1 = (object) new
      {
        project = project,
        scope = scope
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<FavoriteItem>>(favoriteItems, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (identityId != null)
        collection.Add(nameof (identityId), identityId);
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
      return this.SendAsync<List<FavoriteItem>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FavoriteItem>> CreateFavoritesAsync(
      IEnumerable<FavoriteItem> favoriteItems,
      Guid project,
      string scope,
      string identityId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("938f7222-9acb-48fe-b8a3-4eda04597171");
      object obj1 = (object) new
      {
        project = project,
        scope = scope
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<FavoriteItem>>(favoriteItems, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (identityId != null)
        collection.Add(nameof (identityId), identityId);
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
      return this.SendAsync<List<FavoriteItem>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteFavoritesAsync(
      string project,
      string scope,
      string identityId = null,
      string favoriteItemIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("938f7222-9acb-48fe-b8a3-4eda04597171");
      object routeValues = (object) new
      {
        project = project,
        scope = scope
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (identityId != null)
        keyValuePairList.Add(nameof (identityId), identityId);
      if (favoriteItemIds != null)
        keyValuePairList.Add(nameof (favoriteItemIds), favoriteItemIds);
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteFavoritesAsync(
      Guid project,
      string scope,
      string identityId = null,
      string favoriteItemIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("938f7222-9acb-48fe-b8a3-4eda04597171");
      object routeValues = (object) new
      {
        project = project,
        scope = scope
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (identityId != null)
        keyValuePairList.Add(nameof (identityId), identityId);
      if (favoriteItemIds != null)
        keyValuePairList.Add(nameof (favoriteItemIds), favoriteItemIds);
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FavoriteItem>> GetFavoritesAsync(
      string project,
      string scope,
      string identityId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("938f7222-9acb-48fe-b8a3-4eda04597171");
      object routeValues = (object) new
      {
        project = project,
        scope = scope
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (identityId != null)
        keyValuePairList.Add(nameof (identityId), identityId);
      return this.SendAsync<List<FavoriteItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FavoriteItem>> GetFavoritesAsync(
      Guid project,
      string scope,
      string identityId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("938f7222-9acb-48fe-b8a3-4eda04597171");
      object routeValues = (object) new
      {
        project = project,
        scope = scope
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (identityId != null)
        keyValuePairList.Add(nameof (identityId), identityId);
      return this.SendAsync<List<FavoriteItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetFlightAssignmentsAsync(
      string flightName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("409d301f-3046-46f3-beb9-4357fbce0a8c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (flightName != null)
        keyValuePairList.Add(nameof (flightName), flightName);
      return this.SendAsync<List<string>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Folder> CreateFolderAsync(
      Folder folder,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Folder> CreateFolderAsync(
      Folder folder,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete("This method is no longer supported. Use CreateFolder with folder parameter API.")]
    public virtual Task<Folder> CreateFolderAsync(
      Folder folder,
      string project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea");
      object obj1 = (object) new
      {
        project = project,
        path = path
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete("This method is no longer supported. Use CreateFolder with folder parameter API.")]
    public virtual Task<Folder> CreateFolderAsync(
      Folder folder,
      Guid project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea");
      object obj1 = (object) new
      {
        project = project,
        path = path
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteFolderAsync(
      string project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea"), (object) new
      {
        project = project,
        path = path
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFolderAsync(
      Guid project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea"), (object) new
      {
        project = project,
        path = path
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<Folder>> GetFoldersAsync(
      string project,
      string path = null,
      FolderPathQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea");
      object routeValues = (object) new
      {
        project = project,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<Folder>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Folder>> GetFoldersAsync(
      Guid project,
      string path = null,
      FolderPathQueryOrder? queryOrder = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea");
      object routeValues = (object) new
      {
        project = project,
        path = path
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      return this.SendAsync<List<Folder>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Folder> UpdateFolderAsync(
      Folder folder,
      string project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea");
      object obj1 = (object) new
      {
        project = project,
        path = path
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Folder> UpdateFolderAsync(
      Folder folder,
      Guid project,
      string path,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("f7ddf76d-ce0c-4d68-94ff-becaec5d9dea");
      object obj1 = (object) new
      {
        project = project,
        path = path
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Folder>(folder, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Folder>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReleaseGates> UpdateGatesAsync(
      GateUpdateMetadata gateUpdateMetadata,
      string project,
      int gateStepId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2666a539-2001-4f80-bcc7-0379956749d4");
      object obj1 = (object) new
      {
        project = project,
        gateStepId = gateStepId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GateUpdateMetadata>(gateUpdateMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseGates>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ReleaseGates> UpdateGatesAsync(
      GateUpdateMetadata gateUpdateMetadata,
      Guid project,
      int gateStepId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2666a539-2001-4f80-bcc7-0379956749d4");
      object obj1 = (object) new
      {
        project = project,
        gateStepId = gateStepId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<GateUpdateMetadata>(gateUpdateMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseGates>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseRevision>> GetReleaseHistoryAsync(
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseRevision>>(new HttpMethod("GET"), new Guid("23f461c8-629a-4144-a076-3054fa5f268a"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseRevision>> GetReleaseHistoryAsync(
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseRevision>>(new HttpMethod("GET"), new Guid("23f461c8-629a-4144-a076-3054fa5f268a"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<InputValuesQuery> GetInputValuesAsync(
      InputValuesQuery query,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("71dd499b-317d-45ea-9134-140ea1932b5e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<InputValuesQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<InputValuesQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<InputValuesQuery> GetInputValuesAsync(
      InputValuesQuery query,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("71dd499b-317d-45ea-9134-140ea1932b5e");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<InputValuesQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<InputValuesQuery>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AutoTriggerIssue>> GetIssuesAsync(
      string project,
      int buildId,
      string sourceId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cd42261a-f5c6-41c8-9259-f078989b9f25");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (sourceId != null)
        keyValuePairList.Add(nameof (sourceId), sourceId);
      return this.SendAsync<List<AutoTriggerIssue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<AutoTriggerIssue>> GetIssuesAsync(
      Guid project,
      int buildId,
      string sourceId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cd42261a-f5c6-41c8-9259-f078989b9f25");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (sourceId != null)
        keyValuePairList.Add(nameof (sourceId), sourceId);
      return this.SendAsync<List<AutoTriggerIssue>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetGateLogAsync(
      string project,
      int releaseId,
      int environmentId,
      int gateId,
      int taskId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dec7ca5a-7f7f-4797-8bf1-8efc0dc93b28");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        gateId = gateId,
        taskId = taskId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetGateLogAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int gateId,
      int taskId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dec7ca5a-7f7f-4797-8bf1-8efc0dc93b28");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        gateId = gateId,
        taskId = taskId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetLogsAsync(
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c37fbab5-214b-48e4-a55b-cb6b4f6e4038");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetLogsAsync(
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c37fbab5-214b-48e4-a55b-cb6b4f6e4038");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetLogAsync(
      string project,
      int releaseId,
      int environmentId,
      int taskId,
      int? attemptId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e71ba1ed-c0a4-4a28-a61f-2dd5f68cf3fd");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        taskId = taskId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (attemptId.HasValue)
        keyValuePairList.Add(nameof (attemptId), attemptId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetLogAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int taskId,
      int? attemptId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e71ba1ed-c0a4-4a28-a61f-2dd5f68cf3fd");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        taskId = taskId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (attemptId.HasValue)
        keyValuePairList.Add(nameof (attemptId), attemptId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetTaskLog2Async(
      string project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      int taskId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2577e6c3-6999-4400-bc69-fe1d837755fe");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineId,
        taskId = taskId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetTaskLog2Async(
      Guid project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      int taskId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2577e6c3-6999-4400-bc69-fe1d837755fe");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineId,
        taskId = taskId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTaskLogAsync(
      string project,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int taskId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("17c91af7-09fd-4256-bff1-c24ee4f73bc0");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        releaseDeployPhaseId = releaseDeployPhaseId,
        taskId = taskId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetTaskLogAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int taskId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("17c91af7-09fd-4256-bff1-c24ee4f73bc0");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        releaseDeployPhaseId = releaseDeployPhaseId,
        taskId = taskId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startLine.HasValue)
        keyValuePairList.Add(nameof (startLine), startLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<ManualIntervention> GetManualInterventionAsync(
      string project,
      int releaseId,
      int manualInterventionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ManualIntervention>(new HttpMethod("GET"), new Guid("616c46e4-f370-4456-adaa-fbaf79c7b79e"), (object) new
      {
        project = project,
        releaseId = releaseId,
        manualInterventionId = manualInterventionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ManualIntervention> GetManualInterventionAsync(
      Guid project,
      int releaseId,
      int manualInterventionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ManualIntervention>(new HttpMethod("GET"), new Guid("616c46e4-f370-4456-adaa-fbaf79c7b79e"), (object) new
      {
        project = project,
        releaseId = releaseId,
        manualInterventionId = manualInterventionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ManualIntervention>> GetManualInterventionsAsync(
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ManualIntervention>>(new HttpMethod("GET"), new Guid("616c46e4-f370-4456-adaa-fbaf79c7b79e"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ManualIntervention>> GetManualInterventionsAsync(
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ManualIntervention>>(new HttpMethod("GET"), new Guid("616c46e4-f370-4456-adaa-fbaf79c7b79e"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ManualIntervention> UpdateManualInterventionAsync(
      ManualInterventionUpdateMetadata manualInterventionUpdateMetadata,
      string project,
      int releaseId,
      int manualInterventionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("616c46e4-f370-4456-adaa-fbaf79c7b79e");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId,
        manualInterventionId = manualInterventionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ManualInterventionUpdateMetadata>(manualInterventionUpdateMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ManualIntervention>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<ManualIntervention> UpdateManualInterventionAsync(
      ManualInterventionUpdateMetadata manualInterventionUpdateMetadata,
      Guid project,
      int releaseId,
      int manualInterventionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("616c46e4-f370-4456-adaa-fbaf79c7b79e");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId,
        manualInterventionId = manualInterventionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ManualInterventionUpdateMetadata>(manualInterventionUpdateMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ManualIntervention>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Metric>> GetMetricsAsync(
      string project,
      DateTime? minMetricsTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cd1502bb-3c73-4e11-80a6-d11308dceae5");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      return this.SendAsync<List<Metric>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Metric>> GetMetricsAsync(
      Guid project,
      DateTime? minMetricsTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cd1502bb-3c73-4e11-80a6-d11308dceae5");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      return this.SendAsync<List<Metric>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<OrgPipelineReleaseSettings> GetOrgPipelineReleaseSettingsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OrgPipelineReleaseSettings>(new HttpMethod("GET"), new Guid("d156c759-ca4e-492b-90d4-db03971796ea"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<OrgPipelineReleaseSettings> UpdateOrgPipelineReleaseSettingsAsync(
      OrgPipelineReleaseSettingsUpdateParameters newSettings,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d156c759-ca4e-492b-90d4-db03971796ea");
      HttpContent httpContent = (HttpContent) new ObjectContent<OrgPipelineReleaseSettingsUpdateParameters>(newSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<OrgPipelineReleaseSettings>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ProjectPipelineReleaseSettings> GetPipelineReleaseSettingsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProjectPipelineReleaseSettings>(new HttpMethod("GET"), new Guid("e816b9f4-f9fe-46ba-bdcc-a9af6abf3144"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ProjectPipelineReleaseSettings> GetPipelineReleaseSettingsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProjectPipelineReleaseSettings>(new HttpMethod("GET"), new Guid("e816b9f4-f9fe-46ba-bdcc-a9af6abf3144"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ProjectPipelineReleaseSettings> UpdatePipelineReleaseSettingsAsync(
      ProjectPipelineReleaseSettingsUpdateParameters newSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e816b9f4-f9fe-46ba-bdcc-a9af6abf3144");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ProjectPipelineReleaseSettingsUpdateParameters>(newSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProjectPipelineReleaseSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ProjectPipelineReleaseSettings> UpdatePipelineReleaseSettingsAsync(
      ProjectPipelineReleaseSettingsUpdateParameters newSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e816b9f4-f9fe-46ba-bdcc-a9af6abf3144");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ProjectPipelineReleaseSettingsUpdateParameters>(newSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProjectPipelineReleaseSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ProjectReference>> GetReleaseProjectsAsync(
      string artifactType,
      string artifactSourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("917ace4a-79d1-45a7-987c-7be4db4268fa");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      return this.SendAsync<List<ProjectReference>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Release>> GetReleasesAsync(
      string project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string searchText = null,
      string createdBy = null,
      ReleaseStatus? statusFilter = null,
      int? environmentStatusFilter = null,
      DateTime? minCreatedTime = null,
      DateTime? maxCreatedTime = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseExpands? expand = null,
      string artifactTypeId = null,
      string sourceId = null,
      string artifactVersionId = null,
      string sourceBranchFilter = null,
      bool? isDeleted = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<int> releaseIdFilter = null,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (createdBy != null)
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactTypeId != null)
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (sourceId != null)
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (artifactVersionId != null)
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (sourceBranchFilter != null)
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      return this.SendAsync<List<Release>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Release>> GetReleasesAsync(
      Guid project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string searchText = null,
      string createdBy = null,
      ReleaseStatus? statusFilter = null,
      int? environmentStatusFilter = null,
      DateTime? minCreatedTime = null,
      DateTime? maxCreatedTime = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseExpands? expand = null,
      string artifactTypeId = null,
      string sourceId = null,
      string artifactVersionId = null,
      string sourceBranchFilter = null,
      bool? isDeleted = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<int> releaseIdFilter = null,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (createdBy != null)
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactTypeId != null)
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (sourceId != null)
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (artifactVersionId != null)
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (sourceBranchFilter != null)
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      return this.SendAsync<List<Release>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Release>> GetReleasesAsync(
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string searchText = null,
      string createdBy = null,
      ReleaseStatus? statusFilter = null,
      int? environmentStatusFilter = null,
      DateTime? minCreatedTime = null,
      DateTime? maxCreatedTime = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseExpands? expand = null,
      string artifactTypeId = null,
      string sourceId = null,
      string artifactVersionId = null,
      string sourceBranchFilter = null,
      bool? isDeleted = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<int> releaseIdFilter = null,
      string path = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (createdBy != null)
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactTypeId != null)
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (sourceId != null)
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (artifactVersionId != null)
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (sourceBranchFilter != null)
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      return this.SendAsync<List<Release>>(method, locationId, version: new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Release> CreateReleaseAsync(
      ReleaseStartMetadata releaseStartMetadata,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseStartMetadata>(releaseStartMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 8);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Release>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Release> CreateReleaseAsync(
      ReleaseStartMetadata releaseStartMetadata,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseStartMetadata>(releaseStartMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 8);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Release>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteReleaseAsync(
      string project,
      int releaseId,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteReleaseAsync(
      Guid project,
      int releaseId,
      string comment = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (comment != null)
        keyValuePairList.Add(nameof (comment), comment);
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Release> GetReleaseAsync(
      string project,
      int releaseId,
      ApprovalFilters? approvalFilters = null,
      IEnumerable<string> propertyFilters = null,
      SingleReleaseExpands? expand = null,
      int? topGateRecords = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (approvalFilters.HasValue)
        keyValuePairList.Add(nameof (approvalFilters), approvalFilters.Value.ToString());
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (topGateRecords.HasValue)
        keyValuePairList.Add("$topGateRecords", topGateRecords.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<Release>(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Release> GetReleaseAsync(
      Guid project,
      int releaseId,
      ApprovalFilters? approvalFilters = null,
      IEnumerable<string> propertyFilters = null,
      SingleReleaseExpands? expand = null,
      int? topGateRecords = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (approvalFilters.HasValue)
        keyValuePairList.Add(nameof (approvalFilters), approvalFilters.Value.ToString());
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (topGateRecords.HasValue)
        keyValuePairList.Add("$topGateRecords", topGateRecords.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<Release>(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinitionSummary> GetReleaseDefinitionSummaryAsync(
      string project,
      int definitionId,
      int releaseCount,
      bool? includeArtifact = null,
      IEnumerable<int> definitionEnvironmentIdsFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (definitionId), definitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseCount), releaseCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeArtifact.HasValue)
        keyValuePairList.Add(nameof (includeArtifact), includeArtifact.Value.ToString());
      if (definitionEnvironmentIdsFilter != null && definitionEnvironmentIdsFilter.Any<int>())
        keyValuePairList.Add(nameof (definitionEnvironmentIdsFilter), string.Join<int>(",", definitionEnvironmentIdsFilter));
      return this.SendAsync<ReleaseDefinitionSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinitionSummary> GetReleaseDefinitionSummaryAsync(
      Guid project,
      int definitionId,
      int releaseCount,
      bool? includeArtifact = null,
      IEnumerable<int> definitionEnvironmentIdsFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (definitionId), definitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseCount), releaseCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeArtifact.HasValue)
        keyValuePairList.Add(nameof (includeArtifact), includeArtifact.Value.ToString());
      if (definitionEnvironmentIdsFilter != null && definitionEnvironmentIdsFilter.Any<int>())
        keyValuePairList.Add(nameof (definitionEnvironmentIdsFilter), string.Join<int>(",", definitionEnvironmentIdsFilter));
      return this.SendAsync<ReleaseDefinitionSummary>(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetReleaseRevisionAsync(
      string project,
      int releaseId,
      int definitionSnapshotRevision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (definitionSnapshotRevision), definitionSnapshotRevision.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.8"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetReleaseRevisionAsync(
      Guid project,
      int releaseId,
      int definitionSnapshotRevision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (definitionSnapshotRevision), definitionSnapshotRevision.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.8"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UndeleteReleaseAsync(
      string project,
      int releaseId,
      string comment,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (comment), comment);
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UndeleteReleaseAsync(
      Guid project,
      int releaseId,
      string comment,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (comment), comment);
      using (await releaseHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Release> UpdateReleaseAsync(
      Release release,
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Release>(release, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 8);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Release>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Release> UpdateReleaseAsync(
      Release release,
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Release>(release, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 8);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Release>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Release> UpdateReleaseResourceAsync(
      ReleaseUpdateMetadata releaseUpdateMetadata,
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseUpdateMetadata>(releaseUpdateMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 8);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Release>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Release> UpdateReleaseResourceAsync(
      ReleaseUpdateMetadata releaseUpdateMetadata,
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseUpdateMetadata>(releaseUpdateMetadata, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 8);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Release>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseSettings> GetReleaseSettingsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ReleaseSettings>(new HttpMethod("GET"), new Guid("c63c3718-7cfd-41e0-b89b-81c1ca143437"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseSettings> GetReleaseSettingsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ReleaseSettings>(new HttpMethod("GET"), new Guid("c63c3718-7cfd-41e0-b89b-81c1ca143437"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseSettings> UpdateReleaseSettingsAsync(
      ReleaseSettings releaseSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c63c3718-7cfd-41e0-b89b-81c1ca143437");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseSettings>(releaseSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseSettings> UpdateReleaseSettingsAsync(
      ReleaseSettings releaseSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("c63c3718-7cfd-41e0-b89b-81c1ca143437");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ReleaseSettings>(releaseSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ReleaseSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task<Stream> GetDefinitionRevisionAsync(
      string project,
      int definitionId,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("258b82e0-9d41-43f3-86d6-fef14ddd44bc");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId,
        revision = revision
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.4"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetDefinitionRevisionAsync(
      Guid project,
      int definitionId,
      int revision,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("258b82e0-9d41-43f3-86d6-fef14ddd44bc");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId,
        revision = revision
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await releaseHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.4"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await releaseHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<ReleaseDefinitionRevision>> GetReleaseDefinitionHistoryAsync(
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseDefinitionRevision>>(new HttpMethod("GET"), new Guid("258b82e0-9d41-43f3-86d6-fef14ddd44bc"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 4), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ReleaseDefinitionRevision>> GetReleaseDefinitionHistoryAsync(
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseDefinitionRevision>>(new HttpMethod("GET"), new Guid("258b82e0-9d41-43f3-86d6-fef14ddd44bc"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 4), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SummaryMailSection>> GetSummaryMailSectionsAsync(
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SummaryMailSection>>(new HttpMethod("GET"), new Guid("224e92b2-8d13-4c14-b120-13d877c516f8"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<SummaryMailSection>> GetSummaryMailSectionsAsync(
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SummaryMailSection>>(new HttpMethod("GET"), new Guid("224e92b2-8d13-4c14-b120-13d877c516f8"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SendSummaryMailAsync(
      MailMessage mailMessage,
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("224e92b2-8d13-4c14-b120-13d877c516f8");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MailMessage>(mailMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ReleaseHttpClientBase releaseHttpClientBase2 = releaseHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await releaseHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SendSummaryMailAsync(
      MailMessage mailMessage,
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseHttpClientBase releaseHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("224e92b2-8d13-4c14-b120-13d877c516f8");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MailMessage>(mailMessage, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ReleaseHttpClientBase releaseHttpClientBase2 = releaseHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await releaseHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetSourceBranchesAsync(
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("0e5def23-78b3-461f-8198-1558f25041c8"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetSourceBranchesAsync(
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("0e5def23-78b3-461f-8198-1558f25041c8"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> AddDefinitionTagAsync(
      string project,
      int releaseDefinitionId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("PATCH"), new Guid("3d21b4c8-c32e-45b2-a7cb-770a369012f4"), (object) new
      {
        project = project,
        releaseDefinitionId = releaseDefinitionId,
        tag = tag
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> AddDefinitionTagAsync(
      Guid project,
      int releaseDefinitionId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("PATCH"), new Guid("3d21b4c8-c32e-45b2-a7cb-770a369012f4"), (object) new
      {
        project = project,
        releaseDefinitionId = releaseDefinitionId,
        tag = tag
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> AddDefinitionTagsAsync(
      IEnumerable<string> tags,
      string project,
      int releaseDefinitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3d21b4c8-c32e-45b2-a7cb-770a369012f4");
      object obj1 = (object) new
      {
        project = project,
        releaseDefinitionId = releaseDefinitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(tags, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> AddDefinitionTagsAsync(
      IEnumerable<string> tags,
      Guid project,
      int releaseDefinitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("3d21b4c8-c32e-45b2-a7cb-770a369012f4");
      object obj1 = (object) new
      {
        project = project,
        releaseDefinitionId = releaseDefinitionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(tags, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> DeleteDefinitionTagAsync(
      string project,
      int releaseDefinitionId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("3d21b4c8-c32e-45b2-a7cb-770a369012f4"), (object) new
      {
        project = project,
        releaseDefinitionId = releaseDefinitionId,
        tag = tag
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> DeleteDefinitionTagAsync(
      Guid project,
      int releaseDefinitionId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("3d21b4c8-c32e-45b2-a7cb-770a369012f4"), (object) new
      {
        project = project,
        releaseDefinitionId = releaseDefinitionId,
        tag = tag
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetDefinitionTagsAsync(
      string project,
      int releaseDefinitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("3d21b4c8-c32e-45b2-a7cb-770a369012f4"), (object) new
      {
        project = project,
        releaseDefinitionId = releaseDefinitionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetDefinitionTagsAsync(
      Guid project,
      int releaseDefinitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("3d21b4c8-c32e-45b2-a7cb-770a369012f4"), (object) new
      {
        project = project,
        releaseDefinitionId = releaseDefinitionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> AddReleaseTagAsync(
      string project,
      int releaseId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("PATCH"), new Guid("c5b602b6-d1b3-4363-8a51-94384f78068f"), (object) new
      {
        project = project,
        releaseId = releaseId,
        tag = tag
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> AddReleaseTagAsync(
      Guid project,
      int releaseId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("PATCH"), new Guid("c5b602b6-d1b3-4363-8a51-94384f78068f"), (object) new
      {
        project = project,
        releaseId = releaseId,
        tag = tag
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> AddReleaseTagsAsync(
      IEnumerable<string> tags,
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c5b602b6-d1b3-4363-8a51-94384f78068f");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(tags, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> AddReleaseTagsAsync(
      IEnumerable<string> tags,
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c5b602b6-d1b3-4363-8a51-94384f78068f");
      object obj1 = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(tags, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<string>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> DeleteReleaseTagAsync(
      string project,
      int releaseId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("c5b602b6-d1b3-4363-8a51-94384f78068f"), (object) new
      {
        project = project,
        releaseId = releaseId,
        tag = tag
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> DeleteReleaseTagAsync(
      Guid project,
      int releaseId,
      string tag,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("DELETE"), new Guid("c5b602b6-d1b3-4363-8a51-94384f78068f"), (object) new
      {
        project = project,
        releaseId = releaseId,
        tag = tag
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetReleaseTagsAsync(
      string project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("c5b602b6-d1b3-4363-8a51-94384f78068f"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetReleaseTagsAsync(
      Guid project,
      int releaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("c5b602b6-d1b3-4363-8a51-94384f78068f"), (object) new
      {
        project = project,
        releaseId = releaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetTagsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("86cee25a-68ba-4ba3-9171-8ad6ffc6df93"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetTagsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("86cee25a-68ba-4ba3-9171-8ad6ffc6df93"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseTask>> GetTasksForTaskGroupAsync(
      string project,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseTask>>(new HttpMethod("GET"), new Guid("4259191d-4b0a-4409-9fb3-09f22ab9bc47"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        releaseDeployPhaseId = releaseDeployPhaseId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseTask>> GetTasksForTaskGroupAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseTask>>(new HttpMethod("GET"), new Guid("4259191d-4b0a-4409-9fb3-09f22ab9bc47"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        releaseDeployPhaseId = releaseDeployPhaseId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseTask>> GetTasks2Async(
      string project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseTask>>(new HttpMethod("GET"), new Guid("4259291d-4b0a-4409-9fb3-04f22ab9bc47"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseTask>> GetTasks2Async(
      Guid project,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseTask>>(new HttpMethod("GET"), new Guid("4259291d-4b0a-4409-9fb3-04f22ab9bc47"), (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseTask>> GetTasksAsync(
      string project,
      int releaseId,
      int environmentId,
      int? attemptId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("36b276e0-3c70-4320-a63c-1a2e1466a0d1");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (attemptId.HasValue)
        keyValuePairList.Add(nameof (attemptId), attemptId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ReleaseTask>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseTask>> GetTasksAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int? attemptId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("36b276e0-3c70-4320-a63c-1a2e1466a0d1");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (attemptId.HasValue)
        keyValuePairList.Add(nameof (attemptId), attemptId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ReleaseTask>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ArtifactTypeDefinition>> GetArtifactTypeDefinitionsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ArtifactTypeDefinition>>(new HttpMethod("GET"), new Guid("8efc2a3c-1fc8-4f6d-9822-75e98cecb48f"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ArtifactTypeDefinition>> GetArtifactTypeDefinitionsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ArtifactTypeDefinition>>(new HttpMethod("GET"), new Guid("8efc2a3c-1fc8-4f6d-9822-75e98cecb48f"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ArtifactVersionQueryResult> GetArtifactVersionsAsync(
      string project,
      int releaseDefinitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("30fc787e-a9e0-4a07-9fbc-3e903aa051d2");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseDefinitionId), releaseDefinitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<ArtifactVersionQueryResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ArtifactVersionQueryResult> GetArtifactVersionsAsync(
      Guid project,
      int releaseDefinitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("30fc787e-a9e0-4a07-9fbc-3e903aa051d2");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseDefinitionId), releaseDefinitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<ArtifactVersionQueryResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ArtifactVersionQueryResult> GetArtifactVersionsForSourcesAsync(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("30fc787e-a9e0-4a07-9fbc-3e903aa051d2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>>(artifacts, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ArtifactVersionQueryResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ArtifactVersionQueryResult> GetArtifactVersionsForSourcesAsync(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("30fc787e-a9e0-4a07-9fbc-3e903aa051d2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>>(artifacts, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ArtifactVersionQueryResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseWorkItemRef>> GetReleaseWorkItemsRefsAsync(
      string project,
      int releaseId,
      int? baseReleaseId = null,
      int? top = null,
      string artifactAlias = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4f165cc0-875c-4768-b148-f12f78769fab");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (baseReleaseId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = baseReleaseId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (baseReleaseId), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (artifactAlias != null)
        keyValuePairList.Add(nameof (artifactAlias), artifactAlias);
      return this.SendAsync<List<ReleaseWorkItemRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseWorkItemRef>> GetReleaseWorkItemsRefsAsync(
      Guid project,
      int releaseId,
      int? baseReleaseId = null,
      int? top = null,
      string artifactAlias = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4f165cc0-875c-4768-b148-f12f78769fab");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (baseReleaseId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = baseReleaseId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (baseReleaseId), str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (artifactAlias != null)
        keyValuePairList.Add(nameof (artifactAlias), artifactAlias);
      return this.SendAsync<List<ReleaseWorkItemRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
