// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Hub.TestHubDispatcher
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.AspNet.SignalR;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server.Hub
{
  [CLSCompliant(false)]
  public class TestHubDispatcher : 
    ITestHubDispatcher,
    IVssFrameworkService,
    IBuildTestHubDispatcher,
    IReleaseTestHubDispatcher
  {
    private IHubContext<ITestHubClient> m_hubContext;
    private static string layer = nameof (TestHubDispatcher);

    void IBuildTestHubDispatcher.HandleTestRunStatsChange(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int buildId)
    {
      IVssRequestContext requestContext = tcmRequestContext.RequestContext;
      if (!((IBuildTestHubDispatcher) this).AnyClient(requestContext, buildId))
        return;
      requestContext.TraceInfo(TestHubDispatcher.layer, string.Format("Storing test run stat change notification for build {0} in DB", (object) buildId));
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
        managementDatabase.AddOrUpdateBuildInProgressTestSignal(projectId, buildId);
      this.QueueDelayedJob(tcmRequestContext);
    }

    void IReleaseTestHubDispatcher.HandleTestRunStatsChange(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int releaseId,
      int environmentId)
    {
      IVssRequestContext requestContext = tcmRequestContext.RequestContext;
      if (!((IReleaseTestHubDispatcher) this).AnyClient(requestContext, releaseId, environmentId))
        return;
      requestContext.TraceInfo(TestHubDispatcher.layer, string.Format("Storing test run stat change notification for release({0}) and enviroment({1}) in DB", (object) releaseId, (object) environmentId));
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
        managementDatabase.AddOrUpdateReleaseInProgressTestSignal(projectId, releaseId, environmentId);
      this.QueueDelayedJob(tcmRequestContext);
    }

    void IBuildTestHubDispatcher.SendTestRunStatsChanged(
      IVssRequestContext requestContext,
      int buildId)
    {
      Action invoker = (Action) (() =>
      {
        requestContext.TraceInfo(TestHubDispatcher.layer, "Sending SignalR event for build {0}", (object) buildId);
        this.GetBuildClientGroup(requestContext, buildId).testRunStatsChangedForBuild(requestContext, buildId);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("BuildId", (double) buildId);
        this.PublishTelemetry(requestContext, properties);
      });
      this.SendEvent(requestContext, invoker);
    }

    void IReleaseTestHubDispatcher.SendTestRunStatsChanged(
      IVssRequestContext requestContext,
      int releaseId,
      int environmentId)
    {
      Action invoker = (Action) (() =>
      {
        requestContext.TraceInfo(TestHubDispatcher.layer, "Sending SignalR event for release {0} & environment {1}", (object) releaseId, (object) environmentId);
        this.GetReleaseClientGroup(requestContext, releaseId, environmentId).testRunStatsChangedForRelease(requestContext, releaseId, environmentId);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("ReleaseId", (double) releaseId);
        this.PublishTelemetry(requestContext, properties);
      });
      this.SendEvent(requestContext, invoker);
    }

    void IBuildTestHubDispatcher.DeleteNotification(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
        managementDatabase.DeleteBuildInProgressTestSignal(projectId, buildId);
    }

    void IReleaseTestHubDispatcher.DeleteNotification(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId)
    {
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
        managementDatabase.DeleteReleaseInProgressTestSignal(projectId, releaseId, environmentId);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<TestHub, ITestHubClient>();

    async Task IBuildTestHubDispatcher.UnWatch(
      IVssRequestContext requestContext,
      int buildId,
      string clientId)
    {
      await this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "TestHub", TestHubDispatcher.GetBuildGroupName(requestContext, buildId), clientId);
    }

    async Task IReleaseTestHubDispatcher.UnWatch(
      IVssRequestContext requestContext,
      int releaseId,
      int environmentId,
      string clientId)
    {
      await this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "TestHub", TestHubDispatcher.GetReleaseGroupName(requestContext, releaseId, environmentId), clientId);
    }

    async Task IBuildTestHubDispatcher.Watch(
      IVssRequestContext requestContext,
      int buildId,
      string clientId)
    {
      await this.m_hubContext.Groups.AddTrackedConnection(requestContext, "TestHub", TestHubDispatcher.GetBuildGroupName(requestContext, buildId), clientId);
    }

    async Task IReleaseTestHubDispatcher.Watch(
      IVssRequestContext requestContext,
      int releaseId,
      int environmentId,
      string clientId)
    {
      await this.m_hubContext.Groups.AddTrackedConnection(requestContext, "TestHub", TestHubDispatcher.GetReleaseGroupName(requestContext, releaseId, environmentId), clientId);
    }

    bool IBuildTestHubDispatcher.AnyClient(IVssRequestContext requestContext, int buildId)
    {
      VssSignalRHubGroup group = requestContext.GetService<IVssSignalRHubGroupService>().GetGroup(requestContext, "TestHub", TestHubDispatcher.GetBuildGroupName(requestContext, buildId));
      if (group == null)
        return false;
      int? count = group.Connections?.Count;
      int num = 0;
      return count.GetValueOrDefault() > num & count.HasValue;
    }

    bool IReleaseTestHubDispatcher.AnyClient(
      IVssRequestContext requestContext,
      int releaseId,
      int environmentId)
    {
      VssSignalRHubGroup group = requestContext.GetService<IVssSignalRHubGroupService>().GetGroup(requestContext, "TestHub", TestHubDispatcher.GetReleaseGroupName(requestContext, releaseId, environmentId));
      if (group == null)
        return false;
      int? count = group.Connections?.Count;
      int num = 0;
      return count.GetValueOrDefault() > num & count.HasValue;
    }

    private int GetTestResultNotificationProcessorJobDelayInSec(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestResultNotificationProcessorJobDelayInSec", 10);

    public Task OnDisconnected(
      IVssRequestContext requestContext,
      string connectionId,
      bool stopCalled)
    {
      return this.m_hubContext.RemoveTrackedConnection<ITestHubClient>(requestContext, "TestHub", connectionId, !stopCalled);
    }

    private static string GetBuildGroupName(IVssRequestContext requestContext, int buildId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BUILD_{0}_{1}", (object) requestContext.ServiceHost.InstanceId, (object) buildId);

    private static string GetReleaseGroupName(
      IVssRequestContext requestContext,
      int releaseId,
      int environmentId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RELEASE_{0}_{1}_ENV_{2}", (object) requestContext.ServiceHost.InstanceId, (object) releaseId, (object) environmentId);
    }

    private void QueueDelayedJob(TestManagementRequestContext tcmRequestContext)
    {
      IVssRequestContext requestContext1 = tcmRequestContext.RequestContext;
      ITeamFoundationJobService service = requestContext1.GetService<ITeamFoundationJobService>();
      int processorJobDelayInSec = this.GetTestResultNotificationProcessorJobDelayInSec(requestContext1);
      Guid jobMapping = tcmRequestContext.JobMappings["TestManagement.Jobs.SignalProcessorJob"];
      IVssRequestContext requestContext2 = requestContext1;
      Guid[] jobIds = new Guid[1]{ jobMapping };
      int maxDelaySeconds = processorJobDelayInSec;
      int num = service.QueueDelayedJobs(requestContext2, (IEnumerable<Guid>) jobIds, maxDelaySeconds);
      requestContext1.TraceInfo(TestHubDispatcher.layer, string.Format("{0} Attempts to schedule a TestRunSingalsProcessor Job", (object) num));
    }

    private ITestHubClient GetBuildClientGroup(IVssRequestContext requestContext, int buildId)
    {
      // ISSUE: reference to a compiler-generated field
      if (TestHubDispatcher.\u003C\u003Eo__19.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TestHubDispatcher.\u003C\u003Eo__19.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, ITestHubClient>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ITestHubClient), typeof (TestHubDispatcher)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return TestHubDispatcher.\u003C\u003Eo__19.\u003C\u003Ep__0.Target((CallSite) TestHubDispatcher.\u003C\u003Eo__19.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<ITestHubClient>(requestContext, "TestHub", TestHubDispatcher.GetBuildGroupName(requestContext, buildId)));
    }

    private ITestHubClient GetReleaseClientGroup(
      IVssRequestContext requestContext,
      int releaseId,
      int environmentId)
    {
      // ISSUE: reference to a compiler-generated field
      if (TestHubDispatcher.\u003C\u003Eo__20.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TestHubDispatcher.\u003C\u003Eo__20.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, ITestHubClient>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ITestHubClient), typeof (TestHubDispatcher)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return TestHubDispatcher.\u003C\u003Eo__20.\u003C\u003Ep__0.Target((CallSite) TestHubDispatcher.\u003C\u003Eo__20.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<ITestHubClient>(requestContext, "TestHub", TestHubDispatcher.GetReleaseGroupName(requestContext, releaseId, environmentId)));
    }

    protected void SendEvent(IVssRequestContext requestContext, Action invoker)
    {
      try
      {
        invoker();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(TestHubDispatcher.layer, ex);
      }
    }

    protected async Task SendEventAsync(IVssRequestContext requestContext, Func<Task> invoker)
    {
      try
      {
        await invoker();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(TestHubDispatcher.layer, ex);
      }
    }

    protected void PublishTelemetry(
      IVssRequestContext requestContext,
      CustomerIntelligenceData properties)
    {
      if (properties.GetData().Count <= 0)
        return;
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "TestManagement", "SignalR", properties);
    }
  }
}
