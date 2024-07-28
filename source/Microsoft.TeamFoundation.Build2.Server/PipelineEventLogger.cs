// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelineEventLogger
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ServiceHooks.Sdk.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class PipelineEventLogger
  {
    public static void IgnoreEvent(
      IVssRequestContext requestContext,
      IExternalGitEvent gitEvent,
      string reason)
    {
      PipelineEventLogger.Log(requestContext, PipelineEventType.IgnoreEvent, gitEvent, new PipelineEventLogger.LogProperties().WithGitEvent(gitEvent).WithIgnoreReason(reason));
    }

    public static void BuildStatus(
      IVssRequestContext requestContext,
      IReadOnlyBuildData buildData,
      string buildStatus,
      string statusCode,
      string statusResult)
    {
      string pipelineEventId;
      if (PipelineEventLogger.TryGetPipelineEventIdFromBuildData(buildData, out pipelineEventId))
        PipelineEventLogger.LogWithoutEvent(requestContext, PipelineEventType.BuildStatus, pipelineEventId, new PipelineEventLogger.LogProperties().WithBuild(buildData).WithBuildStatus(buildStatus).WithStatusCode(statusCode).WithStatusResult(statusResult));
      else
        PipelineEventLogger.PipelineEventIdNotFoundForBuild(requestContext, buildData.Id);
    }

    public static void CheckRun(
      IVssRequestContext requestContext,
      IReadOnlyBuildData buildData,
      string runId,
      string checkName,
      string checkRun,
      string statusCode,
      string result,
      string conclusion)
    {
      string pipelineEventId;
      if (PipelineEventLogger.TryGetPipelineEventIdFromBuildData(buildData, out pipelineEventId))
        PipelineEventLogger.LogWithoutEvent(requestContext, PipelineEventType.BuildStatus, pipelineEventId, new PipelineEventLogger.LogProperties().WithBuild(buildData).WithCheckName(checkName).WithCheckRun(checkRun).WithConclusion(conclusion).WithRunId(runId).WithStatusCode(statusCode).WithStatusResult(result));
      else
        PipelineEventLogger.PipelineEventIdNotFoundForBuild(requestContext, buildData.Id);
    }

    public static void PullRequestBuildsQueued(
      IVssRequestContext requestContext,
      ExternalGitPullRequest pullRequest,
      IEnumerable<BuildData> builds = null)
    {
      if (pullRequest == null)
        return;
      PipelineEventLogger.BuildsQueued(requestContext, pullRequest.PipelineEventId, (PipelineEventLogger.ReadOnlyLogProperties) new PipelineEventLogger.LogProperties().WithGitEvent((IExternalGitEvent) pullRequest).WithTriggerType("pull_request"), builds);
    }

    public static void CIBuildsQueued(
      IVssRequestContext requestContext,
      ExternalGitPush push,
      IEnumerable<BuildData> builds = null)
    {
      if (push == null)
        return;
      PipelineEventLogger.BuildsQueued(requestContext, push.PipelineEventId, (PipelineEventLogger.ReadOnlyLogProperties) new PipelineEventLogger.LogProperties().WithGitEvent((IExternalGitEvent) push).WithTriggerType(nameof (push)), builds);
    }

    public static void RetryBuild(
      IVssRequestContext requestContext,
      IExternalGitEvent gitEvent,
      int buildId,
      string buildStatus)
    {
      if (gitEvent != null && !string.IsNullOrWhiteSpace(gitEvent.PipelineEventId))
        PipelineEventLogger.Log(requestContext, PipelineEventType.BuildStatus, gitEvent, new PipelineEventLogger.LogProperties().WithBuildId(buildId).WithBuildResult(PipelineEventConstants.Retry).WithBuildStatus(buildStatus).WithGitEvent(gitEvent));
      else
        PipelineEventLogger.PipelineEventIdNotFoundForBuild(requestContext, buildId);
    }

    private static void BuildsQueued(
      IVssRequestContext requestContext,
      string pipelineEventId,
      PipelineEventLogger.ReadOnlyLogProperties logProperties,
      IEnumerable<BuildData> builds)
    {
      BuildData[] buildDataArray = (builds != null ? builds.ToArray<BuildData>() : (BuildData[]) null) ?? Array.Empty<BuildData>();
      if (buildDataArray.Length != 0)
      {
        foreach (BuildData buildData in buildDataArray)
        {
          string pipelineEventId1 = pipelineEventId;
          string pipelineEventId2;
          if (string.IsNullOrWhiteSpace(pipelineEventId1) && PipelineEventLogger.TryGetPipelineEventIdFromBuildData((IReadOnlyBuildData) buildData, out pipelineEventId2))
            pipelineEventId1 = pipelineEventId2;
          if (!string.IsNullOrWhiteSpace(pipelineEventId1))
            PipelineEventLogger.LogWithoutEvent(requestContext, PipelineEventType.BuildStatus, pipelineEventId1, new PipelineEventLogger.LogProperties(logProperties).WithBuild((IReadOnlyBuildData) buildData));
          else
            requestContext.TraceInfo(12030215, PipelineEventProperties.PipelineEventsLayer, "Pipeline event ID was not found when queuing build '{0}'", (object) buildData.Id.ToString());
        }
      }
      else
        PipelineEventLogger.LogWithoutEvent(requestContext, PipelineEventType.NoMatchingPipelines, pipelineEventId, new PipelineEventLogger.LogProperties());
    }

    public static void EventReceivedBySubscriber(
      IVssRequestContext requestContext,
      IExternalGitEvent gitEvent)
    {
      PipelineEventLogger.Log(requestContext, PipelineEventType.EventReceivedBySubscriber, gitEvent, new PipelineEventLogger.LogProperties().WithGitEvent(gitEvent));
    }

    public static void LogEvents(
      IVssRequestContext requestContext,
      IEnumerable<IExternalGitEvent> events)
    {
      foreach (IExternalGitEvent gitEvent in events)
        PipelineEventLogger.Log(requestContext, PipelineEventType.HandleEvent, gitEvent, new PipelineEventLogger.LogProperties().WithGitEvent(gitEvent));
    }

    public static void Log(
      IVssRequestContext requestContext,
      PipelineEventType type,
      IExternalGitEvent gitEvent,
      IDictionary<string, string> properties = null)
    {
      PipelineExternalEventLogger.Log(requestContext, type, gitEvent?.PipelineEventId, properties);
    }

    private static void Log(
      IVssRequestContext requestContext,
      PipelineEventType type,
      IExternalGitEvent gitEvent,
      PipelineEventLogger.LogProperties logProperties)
    {
      PipelineExternalEventLogger.Log(requestContext, type, gitEvent?.PipelineEventId, (IDictionary<string, string>) logProperties.ToProperties());
    }

    public static void LogWithoutEvent(
      IVssRequestContext requestContext,
      PipelineEventType type,
      string pipelineEventId,
      IDictionary<string, string> properties = null)
    {
      PipelineExternalEventLogger.Log(requestContext, type, pipelineEventId, properties);
    }

    private static void LogWithoutEvent(
      IVssRequestContext requestContext,
      PipelineEventType type,
      string pipelineEventId,
      PipelineEventLogger.LogProperties logProperties)
    {
      PipelineExternalEventLogger.Log(requestContext, type, pipelineEventId, (IDictionary<string, string>) logProperties.ToProperties());
    }

    public static void LogException(
      IVssRequestContext requestContext,
      IExternalGitEvent gitEvent,
      Exception e)
    {
      PipelineEventLogger.Log(requestContext, PipelineEventType.ExceptionThrown, gitEvent, new PipelineEventLogger.LogProperties().WithException(e).WithGitEvent(gitEvent));
    }

    public static void LogBuildException(
      IVssRequestContext requestContext,
      IReadOnlyBuildData buildData,
      Exception exception)
    {
      string pipelineEventId;
      if (PipelineEventLogger.TryGetPipelineEventIdFromBuildData(buildData, out pipelineEventId))
        PipelineEventLogger.LogWithoutEvent(requestContext, PipelineEventType.ExceptionThrown, pipelineEventId, new PipelineEventLogger.LogProperties().WithBuild(buildData).WithException(exception));
      else
        PipelineEventLogger.PipelineEventIdNotFoundForBuild(requestContext, buildData.Id);
    }

    public static string GetPipelineEventId(
      IVssRequestContext requestContext,
      IDictionary<string, string> headers = null)
    {
      string str = (string) null;
      return headers == null || !headers.TryGetValue("X-VSS-E2EID", out str) ? PipelineExternalEventLogger.GetPipelineEventId(requestContext) : str;
    }

    private static void PipelineEventIdNotFoundForBuild(
      IVssRequestContext requestContext,
      int buildId)
    {
      requestContext.TraceInfo(12030214, PipelineEventProperties.PipelineEventsLayer, "Pipeline event ID was not found for build '{0}'", (object) buildId);
    }

    private static bool TryGetPipelineEventIdFromBuildData(
      IReadOnlyBuildData buildData,
      out string pipelineEventId)
    {
      object obj;
      if (buildData?.Properties != null && buildData.Properties.TryGetValue(BuildProperties.PipelineEventId, out obj) && obj != null)
      {
        pipelineEventId = obj.ToString();
        return true;
      }
      pipelineEventId = string.Empty;
      return false;
    }

    private static bool IsValidSha(string sha) => !string.IsNullOrWhiteSpace(sha) && sha.Length == 40;

    private abstract class ReadOnlyLogProperties
    {
      protected Dictionary<string, string> m_properties;

      protected ReadOnlyLogProperties() => this.m_properties = new Dictionary<string, string>();

      protected ReadOnlyLogProperties(PipelineEventLogger.ReadOnlyLogProperties clone) => this.m_properties = new Dictionary<string, string>((IDictionary<string, string>) clone.m_properties);

      public Dictionary<string, string> ToProperties() => this.m_properties;
    }

    private class LogProperties : PipelineEventLogger.ReadOnlyLogProperties
    {
      public LogProperties()
      {
      }

      public LogProperties(PipelineEventLogger.ReadOnlyLogProperties clone)
        : base(clone)
      {
      }

      public PipelineEventLogger.LogProperties WithGitEvent(IExternalGitEvent gitEvent)
      {
        switch (gitEvent)
        {
          case ExternalGitPullRequest externalGitPullRequest:
            this.m_properties[PipelineEventProperties.Sha] = externalGitPullRequest.MergeCommitSha;
            this.m_properties[PipelineEventProperties.PullRequestNumber] = externalGitPullRequest.Number;
            break;
          case ExternalGitPush externalGitPush:
            this.m_properties[PipelineEventProperties.Sha] = externalGitPush.AfterSha;
            break;
          case ExternalPullRequestCommentEvent requestCommentEvent:
            if (requestCommentEvent.PullRequest != null)
            {
              this.m_properties[PipelineEventProperties.Sha] = requestCommentEvent.PullRequest.MergeCommitSha;
              this.m_properties[PipelineEventProperties.PullRequestNumber] = requestCommentEvent.PullRequest.Number;
              break;
            }
            break;
        }
        return this;
      }

      public PipelineEventLogger.LogProperties WithBuild(IReadOnlyBuildData buildData)
      {
        this.m_properties[PipelineEventProperties.BuildId] = buildData.Id.ToString();
        this.m_properties[PipelineEventProperties.BuildResult] = buildData.Result.ToString();
        this.m_properties[PipelineEventProperties.BuildStatus] = buildData.Status.ToString();
        this.m_properties[PipelineEventProperties.DefinitionName] = buildData.Definition?.Name;
        if (PipelineEventLogger.IsValidSha(buildData.SourceVersion))
          this.m_properties[PipelineEventProperties.Sha] = buildData.SourceVersion;
        return this;
      }

      public PipelineEventLogger.LogProperties WithBuildId(int buildId)
      {
        this.m_properties[PipelineEventProperties.BuildId] = buildId.ToString();
        return this;
      }

      public PipelineEventLogger.LogProperties WithBuildResult(string buildResult)
      {
        this.m_properties[PipelineEventProperties.BuildResult] = buildResult;
        return this;
      }

      public PipelineEventLogger.LogProperties WithBuildStatus(string buildStatus)
      {
        this.m_properties[PipelineEventProperties.BuildStatus] = buildStatus;
        return this;
      }

      public PipelineEventLogger.LogProperties WithCheckName(string checkName)
      {
        this.m_properties[PipelineEventProperties.CheckName] = checkName;
        return this;
      }

      public PipelineEventLogger.LogProperties WithCheckRun(string checkRun)
      {
        this.m_properties[PipelineEventProperties.CheckRun] = checkRun;
        return this;
      }

      public PipelineEventLogger.LogProperties WithConclusion(string conclusion)
      {
        this.m_properties[PipelineEventProperties.Conclusion] = conclusion;
        return this;
      }

      public PipelineEventLogger.LogProperties WithDefinitionName(string definitionName)
      {
        this.m_properties[PipelineEventProperties.DefinitionName] = definitionName;
        return this;
      }

      public PipelineEventLogger.LogProperties WithException(Exception exception)
      {
        this.m_properties[PipelineEventProperties.Exception] = exception.ToString();
        return this;
      }

      public PipelineEventLogger.LogProperties WithIgnoreReason(string reason)
      {
        this.m_properties[PipelineEventProperties.IgnoreReason] = reason;
        return this;
      }

      public PipelineEventLogger.LogProperties WithRunId(string runId)
      {
        this.m_properties[PipelineEventProperties.RunId] = runId;
        return this;
      }

      public PipelineEventLogger.LogProperties WithStatusResult(string statusResult)
      {
        this.m_properties[PipelineEventProperties.StatusResult] = statusResult;
        return this;
      }

      public PipelineEventLogger.LogProperties WithStatusCode(string statusCode)
      {
        this.m_properties[PipelineEventProperties.StatusCode] = statusCode;
        return this;
      }

      public PipelineEventLogger.LogProperties WithTriggerType(string triggerType)
      {
        this.m_properties[PipelineEventProperties.TriggerType] = triggerType;
        return this;
      }
    }
  }
}
