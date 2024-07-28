// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskHttpClientBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public abstract class TaskHttpClientBase : VssHttpClientBase
  {
    public TaskHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TaskHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TaskHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TaskHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TaskHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<List<TaskAttachment>> GetPlanAttachmentsAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskAttachment>>(new HttpMethod("GET"), new Guid("eb55e5d6-2f30-4295-b5ed-38da50b1fc52"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAttachment> CreateAttachmentAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      Stream uploadStream,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("7898f959-9cdf-4096-b29e-7f293031629e");
      object obj1 = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskAttachment>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskAttachment> CreateAttachmentFromArtifactAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      string artifactHash,
      long length,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("7898f959-9cdf-4096-b29e-7f293031629e");
      object routeValues = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactHash), artifactHash);
      keyValuePairList.Add(nameof (length), length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TaskAttachment>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAttachment> GetAttachmentAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskAttachment>(new HttpMethod("GET"), new Guid("7898f959-9cdf-4096-b29e-7f293031629e"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetAttachmentContentAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskHttpClientBase taskHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7898f959-9cdf-4096-b29e-7f293031629e");
      object routeValues = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId,
        type = type,
        name = name
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await taskHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await taskHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TaskAttachment>> GetAttachmentsAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskAttachment>>(new HttpMethod("GET"), new Guid("7898f959-9cdf-4096-b29e-7f293031629e"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId,
        type = type
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AppendTimelineRecordFeedAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      TimelineRecordFeedLinesWrapper lines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TaskHttpClientBase taskHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("858983e4-19bd-4c5e-864c-507b59b58b12");
      object obj1 = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TimelineRecordFeedLinesWrapper>(lines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TaskHttpClientBase taskHttpClientBase2 = taskHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await taskHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TimelineRecordFeedLinesWrapper> GetLinesAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      Guid stepId,
      long? endLine = null,
      int? takeCount = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("858983e4-19bd-4c5e-864c-507b59b58b12");
      object routeValues = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId,
        recordId = recordId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (stepId), stepId.ToString());
      if (endLine.HasValue)
        keyValuePairList.Add(nameof (endLine), endLine.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (takeCount.HasValue)
        keyValuePairList.Add(nameof (takeCount), takeCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<TimelineRecordFeedLinesWrapper>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskAgentJob> GetJobInstanceAsync(
      Guid scopeIdentifier,
      string hubName,
      string orchestrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskAgentJob>(new HttpMethod("GET"), new Guid("0a1efd25-abda-43bd-9629-6c7bdd2e0d60"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        orchestrationId = orchestrationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskLog> AppendLogContentAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      int logId,
      Stream uploadStream,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("46f5667d-263a-4684-91b1-dff7fdcf64e2");
      object obj1 = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        logId = logId
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskLog>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskLog> AssociateLogAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      int logId,
      string serializedBlobId,
      int lineCount,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("46f5667d-263a-4684-91b1-dff7fdcf64e2");
      object routeValues = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        logId = logId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (serializedBlobId), serializedBlobId);
      keyValuePairList.Add(nameof (lineCount), lineCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TaskLog>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskLog> CreateLogAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      TaskLog log,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("46f5667d-263a-4684-91b1-dff7fdcf64e2");
      object obj1 = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TaskLog>(log, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TaskLog>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<string>> GetLogAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("46f5667d-263a-4684-91b1-dff7fdcf64e2");
      object routeValues = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        logId = logId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      long num;
      if (startLine.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = startLine.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (startLine), str);
      }
      if (endLine.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = endLine.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (endLine), str);
      }
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TaskLog>> GetLogsAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskLog>>(new HttpMethod("GET"), new Guid("46f5667d-263a-4684-91b1-dff7fdcf64e2"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskOrchestrationPlanGroupsQueueMetrics>> GetPlanGroupsQueueMetricsAsync(
      Guid scopeIdentifier,
      string hubName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<TaskOrchestrationPlanGroupsQueueMetrics>>(new HttpMethod("GET"), new Guid("038fd4d5-cda7-44ca-92c0-935843fee1a7"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskHubOidcToken> CreateOidcTokenAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid jobId,
      IDictionary<string, string> claims,
      Guid? serviceConnectionId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("69a319f4-28c1-4bfd-93e6-ea0ff5c6f1a2");
      object obj1 = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        jobId = jobId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, string>>(claims, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (serviceConnectionId.HasValue)
        collection.Add(nameof (serviceConnectionId), serviceConnectionId.Value.ToString());
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
      return this.SendAsync<TaskHubOidcToken>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TaskOrchestrationQueuedPlanGroup>> GetQueuedPlanGroupsAsync(
      Guid scopeIdentifier,
      string hubName,
      PlanGroupStatus? statusFilter = null,
      int? count = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0dd73091-3e36-4f43-b443-1b76dd426d84");
      object routeValues = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (count.HasValue)
        keyValuePairList.Add(nameof (count), count.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TaskOrchestrationQueuedPlanGroup>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskOrchestrationQueuedPlanGroup> GetQueuedPlanGroupAsync(
      Guid scopeIdentifier,
      string hubName,
      string planGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskOrchestrationQueuedPlanGroup>(new HttpMethod("GET"), new Guid("65fd0708-bc1e-447b-a731-0587c5464e5b"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planGroup = planGroup
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TaskOrchestrationPlan> GetPlanAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskOrchestrationPlan>(new HttpMethod("GET"), new Guid("5cecd946-d704-471e-a45f-3b4064fcfaba"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TimelineRecord>> GetRecordsAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      int? changeId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8893bc5b-35b2-4be7-83cb-99e683551db4");
      object routeValues = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeId.HasValue)
        keyValuePairList.Add(nameof (changeId), changeId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TimelineRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TimelineRecord>> UpdateRecordsAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      VssJsonCollectionWrapper<IEnumerable<TimelineRecord>> records,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("8893bc5b-35b2-4be7-83cb-99e683551db4");
      object obj1 = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VssJsonCollectionWrapper<IEnumerable<TimelineRecord>>>(records, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TimelineRecord>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Timeline> CreateTimelineAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Timeline timeline,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("83597576-cc2c-453c-bea6-2882ae6a1653");
      object obj1 = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Timeline>(timeline, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Timeline>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTimelineAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("83597576-cc2c-453c-bea6-2882ae6a1653"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Timeline> GetTimelineAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      Guid timelineId,
      int? changeId = null,
      bool? includeRecords = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("83597576-cc2c-453c-bea6-2882ae6a1653");
      object routeValues = (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId,
        timelineId = timelineId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (changeId.HasValue)
        keyValuePairList.Add(nameof (changeId), changeId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (includeRecords.HasValue)
        keyValuePairList.Add(nameof (includeRecords), includeRecords.Value.ToString());
      return this.SendAsync<Timeline>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Timeline>> GetTimelinesAsync(
      Guid scopeIdentifier,
      string hubName,
      Guid planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Timeline>>(new HttpMethod("GET"), new Guid("83597576-cc2c-453c-bea6-2882ae6a1653"), (object) new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = hubName,
        planId = planId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
