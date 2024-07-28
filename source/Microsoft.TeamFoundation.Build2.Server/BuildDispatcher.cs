// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDispatcher
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.AspNet.SignalR;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Build.WebApi.Events;
using Microsoft.TeamFoundation.Build2.Server.Events;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDispatcher : IBuildDispatcher, IVssFrameworkService
  {
    private IHubContext<IBuildHubClient> m_hubContext;
    private static string s_layer = nameof (BuildDispatcher);

    public async Task WatchBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string clientId)
    {
      IBuildService buildService = requestContext.GetService<IBuildService>();
      IVssRequestContext requestContextToConsider = requestContext;
      if (requestContext.RootContext.Items.TryGetValue("RequestProject", out object _))
        requestContextToConsider = requestContext.Elevate();
      BuildData build = buildService.GetBuildById(requestContextToConsider, projectId, buildId, (IEnumerable<string>) new string[1]
      {
        "*"
      });
      if (build == null)
      {
        buildService = (IBuildService) null;
        requestContextToConsider = (IVssRequestContext) null;
        build = (BuildData) null;
      }
      else if (build.OrchestrationPlan == null)
      {
        buildService = (IBuildService) null;
        requestContextToConsider = (IVssRequestContext) null;
        build = (BuildData) null;
      }
      else
      {
        await this.m_hubContext.Groups.AddTrackedConnection(requestContext, "BuildDetailHub", this.GetBuildGroupName(requestContext, buildId), clientId);
        if (!requestContext.IsFeatureEnabled("DistributedTask.ReducePostLinesSpeed"))
        {
          buildService = (IBuildService) null;
          requestContextToConsider = (IVssRequestContext) null;
          build = (BuildData) null;
        }
        else if (build.Properties.GetValue<bool>(BuildProperties.EnableFastPostLines, false))
        {
          buildService = (IBuildService) null;
          requestContextToConsider = (IVssRequestContext) null;
          build = (BuildData) null;
        }
        else
        {
          build.Properties.Add(BuildProperties.EnableFastPostLines, (object) true);
          buildService.UpdateProperties(requestContextToConsider, build.ProjectId, build.Id, build.Properties);
          TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");
          (int SlowPostLines, int FastPostLines) linesSpeedConstants = taskHub.GetLogLinesSpeedConstants(requestContext);
          await taskHub.SendAgentJobMetadataUpdateToPlanAsync(requestContext, projectId, build.OrchestrationPlan.PlanId, new JobMetadataMessage()
          {
            PostLinesFrequencyMillis = new int?(linesSpeedConstants.FastPostLines)
          });
          buildService = (IBuildService) null;
          requestContextToConsider = (IVssRequestContext) null;
          build = (BuildData) null;
        }
      }
    }

    public async Task SyncState(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string clientId)
    {
      List<RealtimeBuildEvent> realtimeBuildEventList = new List<RealtimeBuildEvent>();
      await requestContext.GetService<IBuildServiceInternal>().SendRealtimeEventPayloadsToClient(requestContext, buildId, clientId);
    }

    internal async Task StopWatchingBuild(
      IVssRequestContext requestContext,
      int buildId,
      string clientId)
    {
      await this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "BuildDetailHub", this.GetBuildGroupName(requestContext, buildId), clientId);
    }

    internal Task Disconnect(
      IVssRequestContext requestContext,
      string connectionId,
      bool stopCalled)
    {
      return this.m_hubContext.RemoveTrackedConnection<IBuildHubClient>(requestContext, "BuildDetailHub", connectionId, !stopCalled);
    }

    public Task WatchProject(IVssRequestContext requestContext, Guid projectId, string clientId)
    {
      if (requestContext.GetService<IProjectService>().TryGetProject(requestContext, projectId, out ProjectInfo _))
        return this.m_hubContext.Groups.AddTrackedConnection(requestContext, "BuildDetailHub", this.GetProjectGroupName(requestContext, projectId), clientId);
      requestContext.TraceError(12030145, BuildDispatcher.s_layer, "WatchProject called with project id {0}, which was not found. Either it doesn't exist or the user doesn't have permission.", (object) projectId);
      return Task.CompletedTask;
    }

    public Task StopWatchingProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string clientId)
    {
      return this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "BuildDetailHub", this.GetProjectGroupName(requestContext, projectId), clientId);
    }

    public Task WatchCollection(IVssRequestContext requestContext, string clientId) => this.m_hubContext.Groups.AddTrackedConnection(requestContext, "BuildDetailHub", this.GetCollectionGroupName(requestContext), clientId);

    public Task StopWatchingCollection(IVssRequestContext requestContext, string clientId) => this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "BuildDetailHub", this.GetCollectionGroupName(requestContext), clientId);

    public void SendArtifactAdded(
      IVssRequestContext requestContext,
      string clientId,
      int buildId,
      string artifactName)
    {
      Action<ClientTraceData> invoker = (Action<ClientTraceData>) (ciData => this.GetClient(requestContext, buildId, clientId).buildArtifactAdded(requestContext, buildId, artifactName));
      this.SendEvent(requestContext, invoker, "artifactAdded", buildId);
    }

    public void SendChangesCalculated(
      IVssRequestContext requestContext,
      string clientId,
      int buildId)
    {
      Action<ClientTraceData> invoker = (Action<ClientTraceData>) (ciData => this.GetClient(requestContext, buildId, clientId).changesCalculated(requestContext, buildId));
      this.SendEvent(requestContext, invoker, "changesCalculated", buildId);
    }

    public void SendBuildUpdated(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      int definitionId,
      int buildId,
      string definitionScope)
    {
      Action<ClientTraceData> invoker1 = (Action<ClientTraceData>) (ciData => this.GetClient(requestContext, buildId, clientId).buildUpdated2(requestContext, definitionId, buildId, definitionScope));
      this.SendEvent(requestContext, invoker1, "buildUpdated", buildId);
      Action<ClientTraceData> invoker2 = (Action<ClientTraceData>) (ciData =>
      {
        object obj = this.m_hubContext.Clients.TrackedGroup<IBuildHubClient>(requestContext, "BuildDetailHub", this.GetProjectGroupName(requestContext, projectId));
        // ISSUE: reference to a compiler-generated field
        if (BuildDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "buildUpdated2", (IEnumerable<Type>) null, typeof (BuildDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[5]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        BuildDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) BuildDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0, obj, requestContext, definitionId, buildId, definitionScope);
      });
      this.SendEvent(requestContext, invoker2, "buildUpdated", buildId);
    }

    public async Task SendBuildUpdatedAsync(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      int definitionId,
      int buildId,
      string definitionScope)
    {
      Func<ClientTraceData, Task> invoker = (Func<ClientTraceData, Task>) (ciData => this.GetClient(requestContext, buildId, clientId).buildUpdated2(requestContext, definitionId, buildId, definitionScope));
      Func<ClientTraceData, Task> projectInvoker = (Func<ClientTraceData, Task>) (ciData =>
      {
        object obj1 = this.m_hubContext.Clients.TrackedGroup<IBuildHubClient>(requestContext, "BuildDetailHub", this.GetProjectGroupName(requestContext, projectId));
        // ISSUE: reference to a compiler-generated field
        if (BuildDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, Task>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Task), typeof (BuildDispatcher)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, Task> target = BuildDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, Task>> p1 = BuildDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (BuildDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IVssRequestContext, int, int, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "buildUpdated2", (IEnumerable<Type>) null, typeof (BuildDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[5]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = BuildDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) BuildDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0, obj1, requestContext, definitionId, buildId, definitionScope);
        return target((CallSite) p1, obj2);
      });
      await this.SendEventAsync(requestContext, invoker, "buildUpdated", buildId);
      await this.SendEventAsync(requestContext, projectInvoker, "buildUpdated", buildId);
      projectInvoker = (Func<ClientTraceData, Task>) null;
    }

    public void SendStagesUpdated(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      int buildId,
      Guid timelineId,
      List<TimelineRecordUpdate> stageUpdates)
    {
      Action<ClientTraceData> invoker = (Action<ClientTraceData>) (ciData =>
      {
        object obj = this.m_hubContext.Clients.TrackedGroup<IBuildHubClient>(requestContext, "BuildDetailHub", this.GetProjectGroupName(requestContext, projectId));
        // ISSUE: reference to a compiler-generated field
        if (BuildDispatcher.\u003C\u003Eo__12.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildDispatcher.\u003C\u003Eo__12.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, Guid, List<TimelineRecordUpdate>>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "stagesUpdated", (IEnumerable<Type>) null, typeof (BuildDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[5]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        BuildDispatcher.\u003C\u003Eo__12.\u003C\u003Ep__0.Target((CallSite) BuildDispatcher.\u003C\u003Eo__12.\u003C\u003Ep__0, obj, requestContext, buildId, timelineId, stageUpdates);
      });
      this.SendEvent(requestContext, invoker, "stagesUpdated", buildId);
    }

    public async Task SendStagesUpdatedAsync(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      int buildId,
      Guid timelineId,
      List<TimelineRecordUpdate> stageUpdates)
    {
      Func<ClientTraceData, Task> invoker = (Func<ClientTraceData, Task>) (ciData =>
      {
        object obj1 = this.m_hubContext.Clients.TrackedGroup<IBuildHubClient>(requestContext, "BuildDetailHub", this.GetProjectGroupName(requestContext, projectId));
        // ISSUE: reference to a compiler-generated field
        if (BuildDispatcher.\u003C\u003Eo__13.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildDispatcher.\u003C\u003Eo__13.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, Task>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Task), typeof (BuildDispatcher)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, Task> target = BuildDispatcher.\u003C\u003Eo__13.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, Task>> p1 = BuildDispatcher.\u003C\u003Eo__13.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (BuildDispatcher.\u003C\u003Eo__13.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildDispatcher.\u003C\u003Eo__13.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IVssRequestContext, int, Guid, List<TimelineRecordUpdate>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "stagesUpdated", (IEnumerable<Type>) null, typeof (BuildDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[5]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = BuildDispatcher.\u003C\u003Eo__13.\u003C\u003Ep__0.Target((CallSite) BuildDispatcher.\u003C\u003Eo__13.\u003C\u003Ep__0, obj1, requestContext, buildId, timelineId, stageUpdates);
        return target((CallSite) p1, obj2);
      });
      await this.SendEventAsync(requestContext, invoker, "stagesUpdated", buildId);
    }

    public void SendTagsAdded(IVssRequestContext requestContext, string clientId, int buildId)
    {
      Action<ClientTraceData> invoker = (Action<ClientTraceData>) (ciData => this.GetClient(requestContext, buildId, clientId).tagsAdded(requestContext, buildId));
      this.SendEvent(requestContext, invoker, "tagsAdded", buildId);
    }

    public void SendTimelineRecordsUpdated(
      IVssRequestContext requestContext,
      string clientId,
      int buildId,
      Guid planId,
      Guid timelineId,
      int changeId)
    {
      Action<ClientTraceData> invoker = (Action<ClientTraceData>) (ciData => this.GetClient(requestContext, buildId, clientId).timelineRecordsUpdated(requestContext, buildId, planId, timelineId, changeId));
      this.SendEvent(requestContext, invoker, "timelineRecordsUpdated", buildId);
    }

    public Task SendTimelineRecordsUpdatedAsync(
      IVssRequestContext requestContext,
      string clientId,
      int buildId,
      Guid planId,
      Guid timelineId,
      int changeId)
    {
      Func<ClientTraceData, Task> invoker = (Func<ClientTraceData, Task>) (async ciData => await this.GetClient(requestContext, buildId, clientId).timelineRecordsUpdated(requestContext, buildId, planId, timelineId, changeId));
      return this.SendEventAsync(requestContext, invoker, "timelineRecordsUpdated", buildId);
    }

    public void SendTaskOrchestrationPlanGroupStarted(
      IVssRequestContext requestContext,
      string clientId,
      Guid projectId,
      string planGroup)
    {
      try
      {
        this.m_hubContext.Clients.Group(this.GetCollectionGroupName(requestContext)).taskOrchestrationPlanGroupStarted(requestContext, projectId, planGroup);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030121, nameof (BuildDispatcher), ex);
      }
    }

    public void SendLegacyBuildUpdated(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.Events.BuildUpdatedEvent buildUpdatedEvent)
    {
      Action<ClientTraceData> invoker1 = (Action<ClientTraceData>) (ciData => this.GetClient(requestContext, buildUpdatedEvent.BuildId).buildUpdated(requestContext, buildUpdatedEvent));
      this.SendEvent(requestContext, invoker1, "buildUpdated", buildUpdatedEvent.BuildId);
      if (buildUpdatedEvent.Build.Project == null)
        return;
      Action<ClientTraceData> invoker2 = (Action<ClientTraceData>) (ciData =>
      {
        object obj = this.m_hubContext.Clients.TrackedGroup<IBuildHubClient>(requestContext, "BuildDetailHub", this.GetProjectGroupName(requestContext, buildUpdatedEvent.Build.Project.Id));
        // ISSUE: reference to a compiler-generated field
        if (BuildDispatcher.\u003C\u003Eo__18.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BuildDispatcher.\u003C\u003Eo__18.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, Microsoft.TeamFoundation.Build.WebApi.Events.BuildUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "buildUpdated", (IEnumerable<Type>) null, typeof (BuildDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        BuildDispatcher.\u003C\u003Eo__18.\u003C\u003Ep__0.Target((CallSite) BuildDispatcher.\u003C\u003Eo__18.\u003C\u003Ep__0, obj, requestContext, buildUpdatedEvent);
      });
      this.SendEvent(requestContext, invoker2, "buildUpdated", buildUpdatedEvent.BuildId);
    }

    public void SendLegacyLogConsoleLines(
      IVssRequestContext requestContext,
      ConsoleLogEvent consoleLogEvent)
    {
      Action<ClientTraceData> invoker = (Action<ClientTraceData>) (ciData =>
      {
        this.GetClient(requestContext, consoleLogEvent.BuildId).logConsoleLines(requestContext, consoleLogEvent);
        ciData.Add("ConsoleLogLinesCount", (object) consoleLogEvent.Lines.Count);
      });
      this.SendEvent(requestContext, invoker, "consoleLinesReceived", consoleLogEvent.BuildId);
    }

    public Task SendLegacyLogConsoleLinesAsync(
      IVssRequestContext requestContext,
      ConsoleLogEvent consoleLogEvent)
    {
      Func<ClientTraceData, Task> invoker = (Func<ClientTraceData, Task>) (async ciData =>
      {
        await this.GetClient(requestContext, consoleLogEvent.BuildId).logConsoleLines(requestContext, consoleLogEvent);
        ciData.Add("ConsoleLogLinesCount", (object) consoleLogEvent.Lines.Count);
      });
      return this.SendEventAsync(requestContext, invoker, "consoleLinesReceived", consoleLogEvent.BuildId);
    }

    private void SendEvent(
      IVssRequestContext requestContext,
      Action<ClientTraceData> invoker,
      string key,
      int buildId)
    {
      try
      {
        ClientTraceData properties = new ClientTraceData();
        properties.Add("BuildId", (object) buildId);
        requestContext.TraceInfo(BuildDispatcher.s_layer, "Sending SignalR {0} event for build {1}", (object) key, (object) buildId);
        invoker(properties);
        this.PublishTelemetry(requestContext, properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030121, nameof (BuildDispatcher), ex);
      }
    }

    private async Task SendEventAsync(
      IVssRequestContext requestContext,
      Func<ClientTraceData, Task> invoker,
      string key,
      int buildId)
    {
      try
      {
        ClientTraceData ciData = new ClientTraceData();
        ciData.Add("BuildId", (object) buildId);
        requestContext.TraceInfo(BuildDispatcher.s_layer, "Sending SignalR {0} event for build {1}", (object) key, (object) buildId);
        await invoker(ciData);
        this.PublishTelemetry(requestContext, ciData);
        ciData = (ClientTraceData) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030121, nameof (BuildDispatcher), ex);
      }
    }

    private void PublishTelemetry(IVssRequestContext requestContext, ClientTraceData properties)
    {
      if (properties.GetData().Count <= 0)
        return;
      try
      {
        requestContext.GetService<ClientTraceService>().Publish(requestContext, "Build", "Build.SignalR", properties);
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    private IBuildHubClient GetClient(
      IVssRequestContext requestContext,
      int buildId,
      string clientId = null)
    {
      if (clientId != null)
        return this.m_hubContext.Clients.Client(clientId);
      // ISSUE: reference to a compiler-generated field
      if (BuildDispatcher.\u003C\u003Eo__24.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BuildDispatcher.\u003C\u003Eo__24.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IBuildHubClient>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IBuildHubClient), typeof (BuildDispatcher)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return BuildDispatcher.\u003C\u003Eo__24.\u003C\u003Ep__0.Target((CallSite) BuildDispatcher.\u003C\u003Eo__24.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<IBuildHubClient>(requestContext, "BuildDetailHub", this.GetBuildGroupName(requestContext, buildId)));
    }

    private string GetCollectionGroupName(IVssRequestContext requestContext) => requestContext.ServiceHost.CollectionServiceHost.InstanceId.ToString();

    private string GetBuildGroupName(IVssRequestContext requestContext, int buildId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) requestContext.ServiceHost.InstanceId, (object) buildId);

    private string GetProjectGroupName(IVssRequestContext requestContext, Guid projectId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) requestContext.ServiceHost.InstanceId, (object) projectId);

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<BuildDetailHub, IBuildHubClient>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
