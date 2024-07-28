// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskHttpClient
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public sealed class TaskHttpClient : TaskHttpClientBase
  {
    private readonly ApiResourceVersion m_currentApiVersion = new ApiResourceVersion(2.0, 1);

    public TaskHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TaskHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TaskHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TaskHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TaskHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task AppendTimelineRecordFeedAsync(
      Guid scopeIdentifier,
      string planType,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      IEnumerable<string> lines,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      return this.AppendTimelineRecordFeedAsync(scopeIdentifier, planType, planId, timelineId, recordId, new TimelineRecordFeedLinesWrapper(Guid.Empty, (IList<string>) lines.ToList<string>()), userState, cancellationToken);
    }

    public Task AppendTimelineRecordFeedAsync(
      Guid scopeIdentifier,
      string planType,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      Guid stepId,
      IList<string> lines,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      return this.AppendTimelineRecordFeedAsync(scopeIdentifier, planType, planId, timelineId, recordId, new TimelineRecordFeedLinesWrapper(stepId, lines), userState, cancellationToken);
    }

    public Task AppendTimelineRecordFeedAsync(
      Guid scopeIdentifier,
      string planType,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      Guid stepId,
      IList<string> lines,
      long startLine,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      return this.AppendTimelineRecordFeedAsync(scopeIdentifier, planType, planId, timelineId, recordId, new TimelineRecordFeedLinesWrapper(stepId, lines, startLine), userState, cancellationToken);
    }

    public async Task RaisePlanEventAsync<T>(
      Guid scopeIdentifier,
      string planType,
      Guid planId,
      T eventData,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
      where T : JobEvent
    {
      TaskHttpClient taskHttpClient1 = this;
      var data = new
      {
        scopeIdentifier = scopeIdentifier,
        hubName = planType,
        planId = planId
      };
      TaskHttpClient taskHttpClient2 = taskHttpClient1;
      T obj = eventData;
      Guid planEvents = TaskResourceIds.PlanEvents;
      var routeValues = data;
      ApiResourceVersion currentApiVersion = taskHttpClient1.m_currentApiVersion;
      CancellationToken cancellationToken1 = cancellationToken;
      object userState1 = userState;
      CancellationToken cancellationToken2 = cancellationToken1;
      HttpResponseMessage httpResponseMessage = await taskHttpClient2.PostAsync<T>(obj, planEvents, (object) routeValues, currentApiVersion, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
    }

    public Task<List<TimelineRecord>> UpdateTimelineRecordsAsync(
      Guid scopeIdentifier,
      string planType,
      Guid planId,
      Guid timelineId,
      IEnumerable<TimelineRecord> records,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      return this.UpdateRecordsAsync(scopeIdentifier, planType, planId, timelineId, new VssJsonCollectionWrapper<IEnumerable<TimelineRecord>>((IEnumerable) records), userState, cancellationToken);
    }

    public Task<TaskAgentJob> GetAgentRequestJobAsync(
      Guid scopeIdentifier,
      string planType,
      string orchestrationId,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      return this.GetJobInstanceAsync(scopeIdentifier, planType, orchestrationId, userState, cancellationToken);
    }
  }
}
