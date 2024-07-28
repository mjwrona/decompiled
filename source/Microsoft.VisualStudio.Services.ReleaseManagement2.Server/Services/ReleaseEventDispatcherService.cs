// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseEventDispatcherService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.AspNet.SignalR;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Hub;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events;
using Microsoft.VisualStudio.Services.SignalR;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseEventDispatcherService : 
    ReleaseManagement2ServiceBase,
    IReleaseEventDispatcherService,
    IVssFrameworkService
  {
    private IHubContext<IReleaseHubClient> hubContext;

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.hubContext = GlobalHost.ConnectionManager.GetHubContext<ReleaseHub, IReleaseHubClient>();
    }

    public Task WatchReleaseDefinitionReleases(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      string clientId)
    {
      string releasesGroupName = ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, projectId, definitionId);
      return this.hubContext.Groups.AddTrackedConnection(context, "ReleaseHub", releasesGroupName, clientId);
    }

    public Task StopWatchingReleaseDefinitionReleases(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      string clientId)
    {
      string releasesGroupName = ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, projectId, definitionId);
      return this.hubContext.Groups.RemoveTrackedConnection(context, "ReleaseHub", releasesGroupName, clientId);
    }

    public Task WatchRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      string clientId)
    {
      string releaseGroupName = ReleaseEventDispatcherService.GetReleaseGroupName(context, projectId, releaseId);
      return this.hubContext.Groups.AddTrackedConnection(context, "ReleaseHub", releaseGroupName, clientId);
    }

    public Task StopWatchingRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      string clientId)
    {
      string releaseGroupName = ReleaseEventDispatcherService.GetReleaseGroupName(context, projectId, releaseId);
      return this.hubContext.Groups.RemoveTrackedConnection(context, "ReleaseHub", releaseGroupName, clientId);
    }

    public Task WatchReleaseJobLogs(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      Guid jobId,
      string clientId)
    {
      string timelineRecordGroupName = ReleaseEventDispatcherService.GetTimelineRecordGroupName(context, projectId, releaseId, jobId);
      return this.hubContext.Groups.AddTrackedConnection(context, "ReleaseHub", timelineRecordGroupName, clientId);
    }

    public Task WatchCollection(IVssRequestContext context, string clientId)
    {
      string collectionGroupName = ReleaseEventDispatcherService.GetCollectionGroupName(context);
      return this.hubContext.Groups.AddTrackedConnection(context, "ReleaseHub", collectionGroupName, clientId);
    }

    public Task Disconnect(
      IVssRequestContext requestContext,
      string clientId,
      bool isClientTimeout)
    {
      return this.hubContext.RemoveTrackedConnection<IReleaseHubClient>(requestContext, "ReleaseHub", clientId, isClientTimeout);
    }

    public virtual void SendReleaseCreatedEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId)
    {
      if (definitionId <= 0 || releaseId <= 0 || projectId.Equals(Guid.Empty))
        return;
      string definitionReleasesGroupName = ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, projectId, definitionId);
      ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetHubClient(context, definitionReleasesGroupName).OnReleaseCreated(context, projectId, definitionId, releaseId)));
    }

    public virtual void SendReleaseUpdatedEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId)
    {
      if (definitionId <= 0 || releaseId <= 0 || projectId.Equals(Guid.Empty))
        return;
      var traceObject = new
      {
        projectId = projectId,
        releaseId = releaseId
      };
      context.TraceDataConditionally(1999994, TraceLevel.Verbose, "ReleaseManagementService", "ReleaseEventDispatcher", Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseUpdatedPublishMessage, (Func<object>) (() => (object) traceObject), nameof (SendReleaseUpdatedEvent));
      Action<string> action = (Action<string>) (groupName => ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetHubClient(context, groupName).OnReleaseUpdated2(context, projectId, definitionId, releaseId))));
      action(ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, projectId, definitionId));
      action(ReleaseEventDispatcherService.GetReleaseGroupName(context, projectId, releaseId));
    }

    public virtual void SendReleaseEnvironmentUpdatedEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId)
    {
      if (definitionId <= 0 || releaseId <= 0 || releaseEnvironmentId <= 0 || projectId.Equals(Guid.Empty))
        return;
      Action<string> action = (Action<string>) (groupName => ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetHubClient(context, groupName).OnReleaseEnvironmentUpdated(context, projectId, definitionId, releaseId, releaseEnvironmentId))));
      action(ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, projectId, definitionId));
      action(ReleaseEventDispatcherService.GetReleaseGroupName(context, projectId, releaseId));
    }

    public virtual void SendRealTimeEvent(
      IVssRequestContext context,
      ReleaseEnvironmentStatusUpdatedEvent notificationEvent)
    {
      Action<string> action = (Action<string>) (groupName => ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetHubClient(context, groupName).OnReleaseEnvironmentStatusUpdated(context, notificationEvent.ProjectId, notificationEvent.DefinitionId, notificationEvent.ReleaseId, notificationEvent.EnvironmentId, notificationEvent.EnvironmentStatus, notificationEvent.LatestDeploymentStatus, notificationEvent.LatestDeploymentOperationStatus))));
      action(ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, notificationEvent.ProjectId, notificationEvent.DefinitionId));
      action(ReleaseEventDispatcherService.GetReleaseGroupName(context, notificationEvent.ProjectId, notificationEvent.ReleaseId));
    }

    public virtual void SendDeployJobStartedEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId)
    {
      if (definitionId <= 0 || releaseId <= 0 || definitionEnvironmentId <= 0 || releaseEnvironmentId <= 0 || projectId.Equals(Guid.Empty))
        return;
      string definitionReleasesGroupName = ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, projectId, definitionId);
      ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetHubClient(context, definitionReleasesGroupName).OnDeployJobStartedEvent(context, projectId, definitionId, releaseId, definitionEnvironmentId, releaseEnvironmentId)));
      string releaseGroupName = ReleaseEventDispatcherService.GetReleaseGroupName(context, projectId, releaseId);
      ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetHubClient(context, releaseGroupName).OnReleaseEnvironmentUpdated(context, projectId, definitionId, releaseId, releaseEnvironmentId)));
    }

    public virtual void SendRealTimeEvent(
      IVssRequestContext context,
      ReleaseTasksUpdatedEvent notificationEvent)
    {
      if (notificationEvent == null || !notificationEvent.Tasks.Any<ReleaseTask>())
        return;
      SignalROperation timelineRecords = new SignalROperation();
      timelineRecords.Operations.AddRange(notificationEvent.Tasks.Select<ReleaseTask, SignalRObject>((Func<ReleaseTask, SignalRObject>) (t => new SignalRObject()
      {
        Identifier = t.TimelineRecordId.ToString()
      })));
      string groupName = ReleaseEventDispatcherService.GetReleaseGroupName(context, notificationEvent.ProjectId, notificationEvent.ReleaseId);
      ReleaseEventDispatcherService.SafeExecute(context, (Action) (() =>
      {
        int releaseDeployPhaseId = notificationEvent.ReleaseDeployPhaseId;
        this.GetHubClient(context, groupName).OnReleaseTasksUpdated(context, notificationEvent.ProjectId, notificationEvent.ReleaseId, notificationEvent.EnvironmentId, releaseDeployPhaseId, notificationEvent.ReleaseStepId, notificationEvent.Job.TimelineRecordId, notificationEvent.PlanId, timelineRecords);
      }));
    }

    public virtual void SendRealTimeEvent(
      IVssRequestContext context,
      ReleaseTaskLogUpdatedEvent notificationEvent)
    {
      ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetReleaseHubClient(context, notificationEvent).OnReleaseTaskLogUpdated(context, notificationEvent)));
    }

    public virtual void SendRealTimeEvent(
      IVssRequestContext context,
      TaskOrchestrationPlanGroupsStartedEvent notificationEvent)
    {
      ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetReleaseHubClient(context).OnPlanGroupsStarted(context, notificationEvent)));
    }

    public virtual void SendReleaseApprovalPendingEvent(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int releaseId,
      int releaseEnvironmentId,
      int approvalId,
      Guid approverId)
    {
      string definitionReleasesGroupName = ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, projectId, definitionId);
      ReleaseEventDispatcherService.SafeExecute(context, (Action) (() => this.GetHubClient(context, definitionReleasesGroupName).OnReleaseApprovalPending(context, projectId, definitionId, releaseId, releaseEnvironmentId, approvalId, approverId)));
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "we need to safely execute this and log exception")]
    private static void SafeExecute(IVssRequestContext context, Action action)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        context.TraceException(1999999, "ReleaseManagementService", "signalR", ex);
      }
    }

    private static string GetCollectionGroupName(IVssRequestContext requestContext) => requestContext.ServiceHost.CollectionServiceHost.InstanceId.ToString();

    private static string GetReleaseGroupName(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) projectId, (object) releaseId);
    }

    private static string GetDefinitionReleasesGroupName(
      IVssRequestContext context,
      Guid projectId,
      int definitionId)
    {
      return ReleaseEventDispatcherService.GetDefinitionGroupName(context, "definition_releases_", projectId, definitionId);
    }

    private static string GetDefinitionGroupName(
      IVssRequestContext requestContext,
      string namePrefix,
      Guid projectId,
      int definitionId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}_{2}_{3}", (object) namePrefix, (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) projectId, (object) definitionId);
    }

    private static string GetTimelineRecordGroupName(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      Guid jobId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}_{3}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) projectId, (object) releaseId, (object) jobId);
    }

    private IReleaseHubClient GetReleaseHubClient(
      IVssRequestContext context,
      ReleaseTaskLogUpdatedEvent notificationEvent)
    {
      if (notificationEvent == null)
        throw new ArgumentNullException(nameof (notificationEvent));
      string timelineRecordGroupName = ReleaseEventDispatcherService.GetTimelineRecordGroupName(context, notificationEvent.ProjectId, notificationEvent.ReleaseId, notificationEvent.TimelineRecordId);
      return this.GetHubClient(context, timelineRecordGroupName);
    }

    private object GetReleaseHubClient(
      IVssRequestContext context,
      ReleaseEnvironmentStatusUpdatedEvent notificationEvent)
    {
      if (notificationEvent == null)
        throw new ArgumentNullException(nameof (notificationEvent));
      string releasesGroupName = ReleaseEventDispatcherService.GetDefinitionReleasesGroupName(context, notificationEvent.ProjectId, notificationEvent.DefinitionId);
      return (object) this.GetHubClient(context, releasesGroupName);
    }

    private IReleaseHubClient GetHubClient(IVssRequestContext context, string groupName)
    {
      // ISSUE: reference to a compiler-generated field
      if (ReleaseEventDispatcherService.\u003C\u003Eo__26.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ReleaseEventDispatcherService.\u003C\u003Eo__26.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IReleaseHubClient>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IReleaseHubClient), typeof (ReleaseEventDispatcherService)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ReleaseEventDispatcherService.\u003C\u003Eo__26.\u003C\u003Ep__0.Target((CallSite) ReleaseEventDispatcherService.\u003C\u003Eo__26.\u003C\u003Ep__0, this.hubContext.Clients.TrackedGroup<IReleaseHubClient>(context, "ReleaseHub", groupName));
    }

    private IReleaseHubClient GetReleaseHubClient(IVssRequestContext context)
    {
      string collectionGroupName = ReleaseEventDispatcherService.GetCollectionGroupName(context);
      // ISSUE: reference to a compiler-generated field
      if (ReleaseEventDispatcherService.\u003C\u003Eo__27.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ReleaseEventDispatcherService.\u003C\u003Eo__27.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, IReleaseHubClient>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IReleaseHubClient), typeof (ReleaseEventDispatcherService)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ReleaseEventDispatcherService.\u003C\u003Eo__27.\u003C\u003Ep__0.Target((CallSite) ReleaseEventDispatcherService.\u003C\u003Eo__27.\u003C\u003Ep__0, this.hubContext.Clients.TrackedGroup<IReleaseHubClient>(context, "ReleaseHub", collectionGroupName));
    }
  }
}
