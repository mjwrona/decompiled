// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.GitHubGitEventHandler
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ServiceHooks.Sdk.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  internal class GitHubGitEventHandler : IPipelineEventHandler
  {
    private const string c_layer = "GitHubGitEventHandler";

    public bool HandleEvent(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      string jsonPayload,
      IDictionary<string, string> headers)
    {
      string headerValue = headers.GetHeaderValue("X-GitHub-Event");
      JObject eventPayload = JObject.Parse(jsonPayload);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubGitEventHandler), "Ignoring {0} {1} event at the deployment level.", (object) "GitHub", (object) headerValue);
        return false;
      }
      JObject providerAuthentication = GitHubHelper.GetProviderAuthentication(requestContext, eventPayload);
      List<IExternalGitEvent> list = this.GetExternalGitEvents(requestContext, headerValue, eventPayload, providerAuthentication).ToList<IExternalGitEvent>();
      if (!list.Any<IExternalGitEvent>())
      {
        PipelineEventLogger.LogWithoutEvent(requestContext, PipelineEventType.NoActionableEvents, PipelineEventLogger.GetPipelineEventId(requestContext));
        return false;
      }
      PipelineEventLogger.LogEvents(requestContext, (IEnumerable<IExternalGitEvent>) list);
      BuildEventHelper.HandleEventsForAllDefinitions(requestContext, provider, providerAuthentication, list);
      return true;
    }

    internal bool IsPullRequestBuildEvent(IVssRequestContext requestContext, JObject payload)
    {
      if (!this.IsPipelineBuildEvent(requestContext, payload))
        return false;
      object obj1 = (object) payload;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target1 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p2 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target2 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p1 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "pull_request", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__0, obj1);
      object obj3 = target2((CallSite) p1, obj2, (object) null);
      if (target1((CallSite) p2, obj3))
        return false;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__4.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p4 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__4;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__3.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__3, obj1);
      string lowerInvariant = target3((CallSite) p4, obj4)?.ToLowerInvariant();
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target4 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p9 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__9;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "changes", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__5.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__5, obj1);
      object obj6;
      if (obj5 == null)
      {
        obj6 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj7 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__6.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__6, obj5);
        if (obj7 == null)
        {
          obj6 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__7 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ref", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj8 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__7.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__7, obj7);
          if (obj8 == null)
          {
            obj6 = (object) null;
          }
          else
          {
            // ISSUE: reference to a compiler-generated field
            if (GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__8 == null)
            {
              // ISSUE: reference to a compiler-generated field
              GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "from", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            obj6 = GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__8.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__2.\u003C\u003Ep__8, obj8);
          }
        }
      }
      bool flag = !string.IsNullOrEmpty(target4((CallSite) p9, obj6));
      switch (lowerInvariant)
      {
        case "opened":
        case "reopened":
        case "synchronize":
          return true;
        case "edited":
          return flag;
        default:
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "Pull request event action {0} ignored", (object) lowerInvariant);
          return false;
      }
    }

    private IEnumerable<IExternalGitEvent> GetExternalGitEvents(
      IVssRequestContext requestContext,
      string eventType,
      JObject eventPayload,
      JObject authentication)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubGitEventHandler), "{0} - github", (object) nameof (GetExternalGitEvents));
      ArgumentUtility.CheckForNull<JObject>(eventPayload, nameof (eventPayload));
      List<IExternalGitEvent> events = new List<IExternalGitEvent>();
      string pipelineEventId = PipelineEventLogger.GetPipelineEventId(requestContext);
      switch (eventType.ToLowerInvariant())
      {
        case "check_suite":
          if (this.IsRetryCheckSuiteBuildEvent(requestContext, eventPayload))
          {
            events.Add((IExternalGitEvent) this.GitHubCheckSuiteEventToExternalGitPush(requestContext, eventPayload, pipelineEventId));
            events.AddRange((IEnumerable<IExternalGitEvent>) this.GitHubCheckSuiteEventToExternalPullRequests(eventPayload, pipelineEventId));
            break;
          }
          break;
        case "check_run":
          if (this.IsCheckRunBuildEvent(requestContext, eventPayload))
          {
            events.Add((IExternalGitEvent) this.GitHubCheckRunEventToExternalGitPush(requestContext, eventPayload, pipelineEventId));
            events.AddRange((IEnumerable<IExternalGitEvent>) this.GitHubCheckRunEventToExternalPullRequests(requestContext, eventPayload, pipelineEventId));
            break;
          }
          break;
        case "pull_request":
          if (this.IsPullRequestBuildEvent(requestContext, eventPayload))
          {
            events.Add((IExternalGitEvent) PublishingUtils.GitHubPullRequestEventToExternalGitPullRequest(eventPayload, pipelineEventId));
            break;
          }
          break;
        case "push":
          events.Add((IExternalGitEvent) PublishingUtils.GitHubPushEventToExternalGitPush(eventPayload, pipelineEventId));
          break;
      }
      try
      {
        this.PublishAnalyticsNotificationsForDistinctGitEvents(requestContext, events);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (GitHubGitEventHandler), ex);
      }
      return (IEnumerable<IExternalGitEvent>) events;
    }

    private bool IsRetryCheckSuiteBuildEvent(IVssRequestContext requestContext, JObject payload)
    {
      if (!this.IsPipelineBuildEvent(requestContext, payload))
        return false;
      object obj1 = (object) payload;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target1 = GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p2 = GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target2 = GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p1 = GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__0, obj1);
      object obj3 = target2((CallSite) p1, obj2, (object) null);
      if (target1((CallSite) p2, obj3))
        return false;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__4.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p4 = GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__4;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__3.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__4.\u003C\u003Ep__3, obj1);
      string lowerInvariant = target3((CallSite) p4, obj4)?.ToLowerInvariant();
      if (lowerInvariant == "rerequested")
        return true;
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "Check suite event action {0} ignored", (object) lowerInvariant);
      return false;
    }

    private bool IsCheckRunBuildEvent(IVssRequestContext requestContext, JObject payload)
    {
      if (!this.IsPipelineBuildEvent(requestContext, payload))
        return false;
      object obj1 = (object) payload;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target1 = GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p2 = GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target2 = GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p1 = GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__0, obj1);
      object obj3 = target2((CallSite) p1, obj2, (object) null);
      if (target1((CallSite) p2, obj3))
        return false;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__4.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p4 = GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__4;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__3.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__5.\u003C\u003Ep__3, obj1);
      string lowerInvariant = target3((CallSite) p4, obj4)?.ToLowerInvariant();
      if (lowerInvariant == "requested" || lowerInvariant == "rerequested")
        return true;
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "Check run event action {0} ignored", (object) lowerInvariant);
      return false;
    }

    private bool IsPipelineBuildEvent(IVssRequestContext requestContext, JObject payload)
    {
      object obj1 = (object) payload;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target = GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p2 = GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__0, obj1);
      object obj3;
      if (obj2 == null)
      {
        obj3 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj3 = GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__1.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__6.\u003C\u003Ep__1, obj2);
      }
      if (!string.IsNullOrEmpty(target((CallSite) p2, obj3)))
        return true;
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "GitHub event without installation ID ignored");
      return false;
    }

    private ExternalGitPush GitHubCheckSuiteEventToExternalGitPush(
      IVssRequestContext requestContext,
      JObject gitHubCheckSuiteEvent,
      string pipelineEventId)
    {
      ArgumentUtility.CheckForNull<JObject>(gitHubCheckSuiteEvent, nameof (gitHubCheckSuiteEvent));
      object obj1 = (object) gitHubCheckSuiteEvent;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__2 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target1 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p2 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__2;
      Type type1 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__0, obj1);
      object obj3;
      if (obj2 == null)
      {
        obj3 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj3 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__1.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__1, obj2);
      }
      target1((CallSite) p2, type1, obj3, "gitHubCheckSuiteEvent.check_suite.id");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__5 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target2 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p5 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__5;
      Type type2 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__3.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__3, obj1);
      object obj5;
      if (obj4 == null)
      {
        obj5 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "before", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj5 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__4.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__4, obj4);
      }
      target2((CallSite) p5, type2, obj5, "gitHubCheckSuiteEvent.check_suite.before");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__8 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target3 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__8.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p8 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__8;
      Type type3 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__6.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__6, obj1);
      object obj7;
      if (obj6 == null)
      {
        obj7 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "after", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj7 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__7.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__7, obj6);
      }
      target3((CallSite) p8, type3, obj7, "gitHubCheckSuiteEvent.check_suite.after");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__11 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target4 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p11 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__11;
      Type type4 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__9.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__9, obj1);
      object obj9;
      if (obj8 == null)
      {
        obj9 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head_branch", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj9 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__10.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__10, obj8);
      }
      target4((CallSite) p11, type4, obj9, "gitHubCheckSuiteEvent.check_suite.head_branch");
      ExternalGitRepo repositoryFromGitHubEvent = this.GetRepositoryFromGitHubEvent(gitHubCheckSuiteEvent);
      ExternalGitUser senderFromGitHubEvent = this.GetSenderFromGitHubEvent(gitHubCheckSuiteEvent);
      ArgumentUtility.CheckForNull<ExternalGitRepo>(repositoryFromGitHubEvent, "gitHubEvent.repository");
      ArgumentUtility.CheckForNull<ExternalGitUser>(senderFromGitHubEvent, "gitHubEvent.sender");
      ExternalGitPush externalGitPush1 = new ExternalGitPush();
      ExternalGitPush externalGitPush2 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target5 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__14.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p14 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__14;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target6 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p13 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__12.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__12, obj1);
      object obj11 = target6((CallSite) p13, obj10);
      string str1 = target5((CallSite) p14, obj11);
      externalGitPush2.Id = str1;
      ExternalGitPush externalGitPush3 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target7 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p17 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "before", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target8 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__16.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p16 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__16;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj12 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__15.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__15, obj1);
      object obj13 = target8((CallSite) p16, obj12);
      string str2 = target7((CallSite) p17, obj13);
      externalGitPush3.BeforeSha = str2;
      ExternalGitPush externalGitPush4 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target9 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__20.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p20 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__20;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "after", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target10 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__19.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p19 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__19;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj14 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__18.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__18, obj1);
      object obj15 = target10((CallSite) p19, obj14);
      string str3 = target9((CallSite) p20, obj15);
      externalGitPush4.AfterSha = str3;
      externalGitPush1.Repo = repositoryFromGitHubEvent;
      ExternalGitPush externalGitPush5 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target11 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__23.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p23 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__23;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head_branch", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target12 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__22.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p22 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__22;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj16 = GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__21.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__21, obj1);
      object obj17 = target12((CallSite) p22, obj16);
      string str4 = target11((CallSite) p23, obj17);
      externalGitPush5.GitRef = str4;
      externalGitPush1.PushedBy = senderFromGitHubEvent;
      externalGitPush1.PipelineEventId = pipelineEventId;
      externalGitPush1.Commits = (IList<ExternalGitCommit>) new List<ExternalGitCommit>();
      return externalGitPush1;
    }

    private ExternalGitPush GitHubCheckRunEventToExternalGitPush(
      IVssRequestContext requestContext,
      JObject gitHubCheckRunEvent,
      string pipelineEventId)
    {
      ArgumentUtility.CheckForNull<JObject>(gitHubCheckRunEvent, nameof (gitHubCheckRunEvent));
      object obj1 = (object) gitHubCheckRunEvent;
      ExternalGitRepo repositoryFromGitHubEvent = this.GetRepositoryFromGitHubEvent(gitHubCheckRunEvent);
      ExternalGitUser senderFromGitHubEvent = this.GetSenderFromGitHubEvent(gitHubCheckRunEvent);
      ArgumentUtility.CheckForNull<ExternalGitRepo>(repositoryFromGitHubEvent, "gitHubEvent.repository");
      ArgumentUtility.CheckForNull<ExternalGitUser>(senderFromGitHubEvent, "gitHubEvent.sender");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__3 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target1 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p3 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__3;
      Type type1 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__0, obj1);
      object obj3;
      if (obj2 == null)
      {
        obj3 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj4 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__1.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__1, obj2);
        if (obj4 == null)
        {
          obj3 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj3 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__2.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__2, obj4);
        }
      }
      target1((CallSite) p3, type1, obj3, "gitHubCheckRunEvent.check_run.check_suite.id");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__7 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target2 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p7 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__7;
      Type type2 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__4.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__4, obj1);
      object obj6;
      if (obj5 == null)
      {
        obj6 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj7 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__5.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__5, obj5);
        if (obj7 == null)
        {
          obj6 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__6 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "before", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj6 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__6.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__6, obj7);
        }
      }
      target2((CallSite) p7, type2, obj6, "gitHubCheckRunEvent.check_run.check_suite.before");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__11 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target3 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p11 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__11;
      Type type3 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__8.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__8, obj1);
      object obj9;
      if (obj8 == null)
      {
        obj9 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj10 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__9.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__9, obj8);
        if (obj10 == null)
        {
          obj9 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__10 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "after", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj9 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__10.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__10, obj10);
        }
      }
      target3((CallSite) p11, type3, obj9, "gitHubCheckRunEvent.check_run.check_suite.after");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__15 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target4 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__15.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p15 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__15;
      Type type4 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj11 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__12.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__12, obj1);
      object obj12;
      if (obj11 == null)
      {
        obj12 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__13 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj13 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__13.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__13, obj11);
        if (obj13 == null)
        {
          obj12 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__14 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head_branch", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj12 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__14.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__14, obj13);
        }
      }
      target4((CallSite) p15, type4, obj12, "gitHubCheckRunEvent.check_run.check_suite.head_branch");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__18 = CallSite<\u003C\u003EA\u007B00049000\u007D<CallSite, GitHubGitEventHandler, IVssRequestContext, object, string, string, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "GetBuildInfoFromCheckRun", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[6]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: variable of a compiler-generated type
      \u003C\u003EA\u007B00049000\u007D<CallSite, GitHubGitEventHandler, IVssRequestContext, object, string, string, string> target5 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__18.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<\u003C\u003EA\u007B00049000\u007D<CallSite, GitHubGitEventHandler, IVssRequestContext, object, string, string, string>> p18 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__18;
      IVssRequestContext vssRequestContext = requestContext;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj14 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__16.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__16, obj1);
      object obj15;
      if (obj14 == null)
      {
        obj15 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__17 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "external_id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj15 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__17.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__17, obj14);
      }
      string str1;
      ref string local1 = ref str1;
      string str2;
      ref string local2 = ref str2;
      string str3;
      ref string local3 = ref str3;
      target5((CallSite) p18, this, vssRequestContext, obj15, ref local1, ref local2, ref local3);
      ExternalGitPush externalGitPush1 = new ExternalGitPush();
      ExternalGitPush externalGitPush2 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target6 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__22.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p22 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__22;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target7 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__21.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p21 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__21;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target8 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__20.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p20 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__20;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj16 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__19.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__19, obj1);
      object obj17 = target8((CallSite) p20, obj16);
      object obj18 = target7((CallSite) p21, obj17);
      string str4 = target6((CallSite) p22, obj18);
      externalGitPush2.Id = str4;
      ExternalGitPush externalGitPush3 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__26 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target9 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__26.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p26 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__26;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__25 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "before", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target10 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__25.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p25 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__25;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target11 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__24.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p24 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__24;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj19 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__23.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__23, obj1);
      object obj20 = target11((CallSite) p24, obj19);
      object obj21 = target10((CallSite) p25, obj20);
      string str5 = target9((CallSite) p26, obj21);
      externalGitPush3.BeforeSha = str5;
      ExternalGitPush externalGitPush4 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__30 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target12 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__30.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p30 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__30;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__29 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "after", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target13 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__29.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p29 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__29;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__28 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target14 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__28.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p28 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__28;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__27 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj22 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__27.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__27, obj1);
      object obj23 = target14((CallSite) p28, obj22);
      object obj24 = target13((CallSite) p29, obj23);
      string str6 = target12((CallSite) p30, obj24);
      externalGitPush4.AfterSha = str6;
      ExternalGitPush externalGitPush5 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__34 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target15 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__34.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p34 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__34;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__33 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head_branch", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target16 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__33.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p33 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__33;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__32 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target17 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__32.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p32 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__32;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__31 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj25 = GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__31.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__8.\u003C\u003Ep__31, obj1);
      object obj26 = target17((CallSite) p32, obj25);
      object obj27 = target16((CallSite) p33, obj26);
      string str7 = target15((CallSite) p34, obj27);
      externalGitPush5.GitRef = str7;
      externalGitPush1.Repo = repositoryFromGitHubEvent;
      externalGitPush1.PushedBy = senderFromGitHubEvent;
      externalGitPush1.Commits = (IList<ExternalGitCommit>) new List<ExternalGitCommit>();
      externalGitPush1.DefinitionToBuild = str1;
      externalGitPush1.BuildToRetry = str2;
      externalGitPush1.ProjectId = str3;
      externalGitPush1.PipelineEventId = pipelineEventId;
      return externalGitPush1;
    }

    private List<IExternalGitEvent> GitHubCheckSuiteEventToExternalPullRequests(
      JObject gitHubCheckSuiteEvent,
      string pipelineEventId)
    {
      ArgumentUtility.CheckForNull<JObject>(gitHubCheckSuiteEvent, nameof (gitHubCheckSuiteEvent));
      object obj1 = (object) gitHubCheckSuiteEvent;
      ExternalGitRepo repositoryFromGitHubEvent = this.GetRepositoryFromGitHubEvent(gitHubCheckSuiteEvent);
      ExternalGitUser senderFromGitHubEvent = this.GetSenderFromGitHubEvent(gitHubCheckSuiteEvent);
      ArgumentUtility.CheckForNull<ExternalGitRepo>(repositoryFromGitHubEvent, "gitHubEvent.repository");
      ArgumentUtility.CheckForNull<ExternalGitUser>(senderFromGitHubEvent, "gitHubEvent.sender");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__0, obj1);
      object obj3;
      if (obj2 == null)
      {
        obj3 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "pull_requests", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj3 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__1.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__1, obj2);
      }
      JArray var = obj3 as JArray;
      ArgumentUtility.CheckForNull<JArray>(var, "gitHubCheckSuiteEvent.check_suite.pull_requests");
      List<IExternalGitEvent> externalPullRequests = new List<IExternalGitEvent>(var.Count);
      for (int index = 0; index < var.Count; ++index)
      {
        object obj4 = (object) var[index];
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__3 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, Type, object, string> target1 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, Type, object, string>> p3 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__3;
        Type type1 = typeof (ArgumentUtility);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__2.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__2, obj4);
        string str1 = string.Format("gitHubCheckSuiteEvent.check_suite.pull_requests[{0}].id", (object) index);
        target1((CallSite) p3, type1, obj5, str1);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__5 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, Type, object, string> target2 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, Type, object, string>> p5 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__5;
        Type type2 = typeof (ArgumentUtility);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj6 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__4.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__4, obj4);
        string str2 = string.Format("gitHubCheckSuiteEvent.check_suite.pull_requests[{0}].number", (object) index);
        target2((CallSite) p5, type2, obj6, str2);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__7 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, Type, object, string> target3 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__7.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, Type, object, string>> p7 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__7;
        Type type3 = typeof (ArgumentUtility);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "url", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj7 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__6.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__6, obj4);
        string str3 = string.Format("gitHubCheckSuiteEvent.check_suite.pull_requests[{0}].url", (object) index);
        target3((CallSite) p7, type3, obj7, str3);
        ExternalGitPullRequest externalGitPullRequest1 = new ExternalGitPullRequest();
        ExternalGitPullRequest externalGitPullRequest2 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target4 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__9.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p9 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__9;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj8 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__8.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__8, obj4);
        string str4 = target4((CallSite) p9, obj8);
        externalGitPullRequest2.Id = str4;
        ExternalGitPullRequest externalGitPullRequest3 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__11 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target5 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__11.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p11 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__11;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj9 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__10.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__10, obj4);
        string str5 = target5((CallSite) p11, obj9);
        externalGitPullRequest3.Number = str5;
        ExternalGitPullRequest externalGitPullRequest4 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__13 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target6 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__13.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p13 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__13;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__12 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "url", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj10 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__12.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__12, obj4);
        string str6 = target6((CallSite) p13, obj10);
        externalGitPullRequest4.Url = str6;
        externalGitPullRequest1.Repo = repositoryFromGitHubEvent;
        externalGitPullRequest1.Sender = senderFromGitHubEvent;
        externalGitPullRequest1.IsMergeable = new bool?();
        ExternalGitPullRequest externalGitPullRequest5 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__21 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (GitHubGitEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target7 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__21.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p21 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__21;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__20 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target8 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__20.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p20 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__20;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__16 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target9 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__16.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p16 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__16;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__15 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target10 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__15.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p15 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__15;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__14 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj11 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__14.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__14, obj4);
        object obj12 = target10((CallSite) p15, obj11);
        object obj13 = target9((CallSite) p16, obj12);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__19 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target11 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__19.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p19 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__19;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__18 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target12 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__18.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p18 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__18;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__17 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj14 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__17.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__17, obj4);
        object obj15 = target12((CallSite) p18, obj14);
        object obj16 = target11((CallSite) p19, obj15);
        object obj17 = target8((CallSite) p20, obj13, obj16);
        int num1 = target7((CallSite) p21, obj17) ? 1 : 0;
        externalGitPullRequest5.IsFork = num1 != 0;
        externalGitPullRequest1.PipelineEventId = pipelineEventId;
        ExternalGitPullRequest externalGitPullRequest6 = externalGitPullRequest1;
        externalGitPullRequest6.Draft = false;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__24 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target13 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__24.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p24 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__24;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__23 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target14 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__23.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p23 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__23;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__22 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "draft", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj18 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__22.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__22, obj4);
        object obj19 = target14((CallSite) p23, obj18, (object) null);
        if (target13((CallSite) p24, obj19))
        {
          ExternalGitPullRequest externalGitPullRequest7 = externalGitPullRequest6;
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__26 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (GitHubGitEventHandler)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target15 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__26.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p26 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__26;
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__25 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "draft", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj20 = GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__25.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__9.\u003C\u003Ep__25, obj4);
          int num2 = target15((CallSite) p26, obj20) ? 1 : 0;
          externalGitPullRequest7.Draft = num2 != 0;
        }
        externalPullRequests.Add((IExternalGitEvent) externalGitPullRequest6);
      }
      return externalPullRequests;
    }

    private List<IExternalGitEvent> GitHubCheckRunEventToExternalPullRequests(
      IVssRequestContext requestContext,
      JObject gitHubCheckRunEvent,
      string pipelineEventId)
    {
      ArgumentUtility.CheckForNull<JObject>(gitHubCheckRunEvent, nameof (gitHubCheckRunEvent));
      object obj1 = (object) gitHubCheckRunEvent;
      ExternalGitRepo repositoryFromGitHubEvent = this.GetRepositoryFromGitHubEvent(gitHubCheckRunEvent);
      ExternalGitUser senderFromGitHubEvent = this.GetSenderFromGitHubEvent(gitHubCheckRunEvent);
      ArgumentUtility.CheckForNull<ExternalGitRepo>(repositoryFromGitHubEvent, "gitHubEvent.repository");
      ArgumentUtility.CheckForNull<ExternalGitUser>(senderFromGitHubEvent, "gitHubEvent.sender");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__0, obj1);
      object obj3;
      if (obj2 == null)
      {
        obj3 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_suite", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj4 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__1.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__1, obj2);
        if (obj4 == null)
        {
          obj3 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "pull_requests", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj3 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__2.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__2, obj4);
        }
      }
      JArray var = obj3 as JArray;
      ArgumentUtility.CheckForNull<JArray>(var, "gitHubCheckSuiteEvent.check_suite.pull_requests");
      List<IExternalGitEvent> externalPullRequests = new List<IExternalGitEvent>(var.Count);
      for (int index = 0; index < var.Count; ++index)
      {
        object obj5 = (object) var[index];
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__4 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, Type, object, string> target1 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__4.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, Type, object, string>> p4 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__4;
        Type type1 = typeof (ArgumentUtility);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj6 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__3.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__3, obj5);
        string str1 = string.Format("gitHubCheckSuiteEvent.check_run.check_suite.pull_requests[{0}].id", (object) index);
        target1((CallSite) p4, type1, obj6, str1);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__6 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, Type, object, string> target2 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__6.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, Type, object, string>> p6 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__6;
        Type type2 = typeof (ArgumentUtility);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj7 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__5.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__5, obj5);
        string str2 = string.Format("gitHubCheckSuiteEvent.check_run.check_suite.pull_requests[{0}].number", (object) index);
        target2((CallSite) p6, type2, obj7, str2);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__8 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, Type, object, string> target3 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__8.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, Type, object, string>> p8 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__8;
        Type type3 = typeof (ArgumentUtility);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "url", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj8 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__7.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__7, obj5);
        string str3 = string.Format("gitHubCheckSuiteEvent.check_run.check_suite.pull_requests[{0}].url", (object) index);
        target3((CallSite) p8, type3, obj8, str3);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__11 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__11 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Action<CallSite, Type, object, string> target4 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__11.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Action<CallSite, Type, object, string>> p11 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__11;
        Type type4 = typeof (ArgumentUtility);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj9 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__9.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__9, obj1);
        object obj10;
        if (obj9 == null)
        {
          obj10 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__10 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "external_id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj10 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__10.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__10, obj9);
        }
        target4((CallSite) p11, type4, obj10, "gitHubCheckSuiteEvent.check_run.external_id");
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__14 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__14 = CallSite<\u003C\u003EA\u007B00049000\u007D<CallSite, GitHubGitEventHandler, IVssRequestContext, object, string, string, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "GetBuildInfoFromCheckRun", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[6]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: variable of a compiler-generated type
        \u003C\u003EA\u007B00049000\u007D<CallSite, GitHubGitEventHandler, IVssRequestContext, object, string, string, string> target5 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__14.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<\u003C\u003EA\u007B00049000\u007D<CallSite, GitHubGitEventHandler, IVssRequestContext, object, string, string, string>> p14 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__14;
        IVssRequestContext vssRequestContext = requestContext;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__12 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "check_run", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj11 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__12.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__12, obj1);
        object obj12;
        if (obj11 == null)
        {
          obj12 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__13 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "external_id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj12 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__13.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__13, obj11);
        }
        string str4;
        ref string local1 = ref str4;
        string str5;
        ref string local2 = ref str5;
        string str6;
        ref string local3 = ref str6;
        target5((CallSite) p14, this, vssRequestContext, obj12, ref local1, ref local2, ref local3);
        ExternalGitPullRequest externalGitPullRequest1 = new ExternalGitPullRequest();
        ExternalGitPullRequest externalGitPullRequest2 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__16 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target6 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__16.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p16 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__16;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__15 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj13 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__15.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__15, obj5);
        string str7 = target6((CallSite) p16, obj13);
        externalGitPullRequest2.Id = str7;
        ExternalGitPullRequest externalGitPullRequest3 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__18 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target7 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__18.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p18 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__18;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__17 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj14 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__17.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__17, obj5);
        string str8 = target7((CallSite) p18, obj14);
        externalGitPullRequest3.Number = str8;
        ExternalGitPullRequest externalGitPullRequest4 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__20 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target8 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__20.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p20 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__20;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__19 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "url", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj15 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__19.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__19, obj5);
        string str9 = target8((CallSite) p20, obj15);
        externalGitPullRequest4.Url = str9;
        externalGitPullRequest1.Repo = repositoryFromGitHubEvent;
        externalGitPullRequest1.Sender = senderFromGitHubEvent;
        externalGitPullRequest1.IsMergeable = new bool?();
        ExternalGitPullRequest externalGitPullRequest5 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__28 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (GitHubGitEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target9 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__28.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p28 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__28;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__27 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target10 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__27.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p27 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__27;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__23 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target11 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__23.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p23 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__23;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__22 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target12 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__22.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p22 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__22;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__21 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj16 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__21.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__21, obj5);
        object obj17 = target12((CallSite) p22, obj16);
        object obj18 = target11((CallSite) p23, obj17);
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__26 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Id", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target13 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__26.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p26 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__26;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__25 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target14 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__25.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p25 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__25;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__24 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj19 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__24.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__24, obj5);
        object obj20 = target14((CallSite) p25, obj19);
        object obj21 = target13((CallSite) p26, obj20);
        object obj22 = target10((CallSite) p27, obj18, obj21);
        int num1 = target9((CallSite) p28, obj22) ? 1 : 0;
        externalGitPullRequest5.IsFork = num1 != 0;
        externalGitPullRequest1.DefinitionToBuild = str4;
        externalGitPullRequest1.BuildToRetry = str5;
        externalGitPullRequest1.ProjectId = str6;
        externalGitPullRequest1.PipelineEventId = pipelineEventId;
        ExternalGitPullRequest externalGitPullRequest6 = externalGitPullRequest1;
        externalGitPullRequest6.Draft = false;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__31 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target15 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__31.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p31 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__31;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__30 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target16 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__30.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p30 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__30;
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__29 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "draft", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj23 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__29.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__29, obj5);
        object obj24 = target16((CallSite) p30, obj23, (object) null);
        if (target15((CallSite) p31, obj24))
        {
          ExternalGitPullRequest externalGitPullRequest7 = externalGitPullRequest6;
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__33 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (GitHubGitEventHandler)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target17 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__33.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p33 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__33;
          // ISSUE: reference to a compiler-generated field
          if (GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__32 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "draft", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj25 = GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__32.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__32, obj5);
          int num2 = target17((CallSite) p33, obj25) ? 1 : 0;
          externalGitPullRequest7.Draft = num2 != 0;
        }
        externalPullRequests.Add((IExternalGitEvent) externalGitPullRequest6);
      }
      return externalPullRequests;
    }

    private void GetBuildInfoFromCheckRun(
      IVssRequestContext requestContext,
      object external_id,
      out string definitionId,
      out string buildId,
      out string projectId)
    {
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target = GitHubGitEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p1 = GitHubGitEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = GitHubGitEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__0, typeof (Convert), external_id);
      string str = target((CallSite) p1, obj);
      string[] strArray = str.Split('|');
      switch (strArray.Length)
      {
        case 0:
          definitionId = (string) null;
          buildId = (string) null;
          projectId = (string) null;
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "No definition or build id provided in check run event. Event ignored.");
          break;
        case 1:
          definitionId = strArray[0];
          buildId = (string) null;
          projectId = (string) null;
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "No build id provided in check run event. Defaulting to using the definition id: '" + definitionId + "'.");
          break;
        case 2:
          definitionId = strArray[0];
          buildId = (string) null;
          projectId = (string) null;
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "No project id provided in check run event. Defaulting to using the definition id: '" + definitionId + "'.");
          break;
        case 3:
          definitionId = strArray[0];
          buildId = strArray[1];
          projectId = strArray[2];
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "Found external id '" + definitionId + "|" + buildId + "|" + projectId + "' of format: 'definitionId | buildId | projectId'");
          break;
        default:
          definitionId = (string) null;
          buildId = (string) null;
          projectId = (string) null;
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubGitEventHandler), "Invalid external id '" + str + "' provided in check run event. Event ignored.");
          break;
      }
    }

    private ExternalGitRepo GetRepositoryFromGitHubEvent(JObject gitHubEvent)
    {
      ArgumentUtility.CheckForNull<JObject>(gitHubEvent, nameof (gitHubEvent));
      object obj1 = (object) gitHubEvent;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__2 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target1 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p2 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__2;
      Type type1 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__0, obj1);
      object obj3;
      if (obj2 == null)
      {
        obj3 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj3 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__1.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__1, obj2);
      }
      target1((CallSite) p2, type1, obj3, "eventPayload.repository.name");
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__5 = CallSite<Action<CallSite, Type, object, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "CheckForNull", (IEnumerable<Type>) null, typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Type, object, string> target2 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Type, object, string>> p5 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__5;
      Type type2 = typeof (ArgumentUtility);
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__3.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__3, obj1);
      object obj5;
      if (obj4 == null)
      {
        obj5 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "clone_url", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj5 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__4.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__4, obj4);
      }
      target2((CallSite) p5, type2, obj5, "eventPayload.repository.clone_url");
      ExternalGitRepo repositoryFromGitHubEvent = new ExternalGitRepo();
      ExternalGitRepo externalGitRepo1 = repositoryFromGitHubEvent;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__8.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p8 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__8;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "full_name", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target4 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p7 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__6.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__6, obj1);
      object obj7 = target4((CallSite) p7, obj6);
      string str1 = target3((CallSite) p8, obj7);
      externalGitRepo1.Id = str1;
      ExternalGitRepo externalGitRepo2 = repositoryFromGitHubEvent;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target5 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p11 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__11;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target6 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__10.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p10 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__10;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__9.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__9, obj1);
      object obj9 = target6((CallSite) p10, obj8);
      string str2 = target5((CallSite) p11, obj9);
      externalGitRepo2.Name = str2;
      ExternalGitRepo externalGitRepo3 = repositoryFromGitHubEvent;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target7 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__14.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p14 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__14;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "clone_url", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target8 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p13 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__12.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__12, obj1);
      object obj11 = target8((CallSite) p13, obj10);
      string str3 = target7((CallSite) p14, obj11);
      externalGitRepo3.Url = str3;
      ExternalGitRepo externalGitRepo4 = repositoryFromGitHubEvent;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target9 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p17 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "html_url", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target10 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__16.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p16 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__16;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj12 = GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__15.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__12.\u003C\u003Ep__15, obj1);
      object obj13 = target10((CallSite) p16, obj12);
      string str4 = target9((CallSite) p17, obj13);
      externalGitRepo4.WebUrl = str4;
      return repositoryFromGitHubEvent;
    }

    private ExternalGitUser GetSenderFromGitHubEvent(JObject gitHubEvent)
    {
      ArgumentUtility.CheckForNull<JObject>(gitHubEvent, nameof (gitHubEvent));
      object obj1 = (object) gitHubEvent;
      ExternalGitUser senderFromGitHubEvent = new ExternalGitUser();
      ExternalGitUser externalGitUser1 = senderFromGitHubEvent;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target1 = GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p2 = GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__0.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__0, obj1);
      object obj3;
      if (obj2 == null)
      {
        obj3 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj3 = GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__1.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__1, obj2);
      }
      string str1 = target1((CallSite) p2, obj3);
      externalGitUser1.Name = str1;
      ExternalGitUser externalGitUser2 = senderFromGitHubEvent;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubGitEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target2 = GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p5 = GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__3.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__3, obj1);
      object obj5;
      if (obj4 == null)
      {
        obj5 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (GitHubGitEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj5 = GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__4.Target((CallSite) GitHubGitEventHandler.\u003C\u003Eo__13.\u003C\u003Ep__4, obj4);
      }
      string str2 = target2((CallSite) p5, obj5);
      externalGitUser2.Email = str2;
      return senderFromGitHubEvent;
    }

    private void PublishAnalyticsNotificationsForDistinctGitEvents(
      IVssRequestContext requestContext,
      List<IExternalGitEvent> events)
    {
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      if (events.FirstOrDefault<IExternalGitEvent>((Func<IExternalGitEvent, bool>) (e => e is ExternalGitPush)) is ExternalAnalyticsGitPush notificationEvent1)
        service.PublishNotification(requestContext, (object) notificationEvent1);
      if (!(events.FirstOrDefault<IExternalGitEvent>((Func<IExternalGitEvent, bool>) (e => e is ExternalGitPullRequest)) is ExternalAnalyticsGitPullRequest notificationEvent2))
        return;
      service.PublishNotification(requestContext, (object) notificationEvent2);
    }
  }
}
