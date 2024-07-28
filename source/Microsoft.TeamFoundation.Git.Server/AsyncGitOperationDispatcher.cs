// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.AsyncGitOperationDispatcher
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.AspNet.SignalR;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class AsyncGitOperationDispatcher : IAsyncGitOperationDispatcher, IVssFrameworkService
  {
    private IHubContext<IAsyncGitOperationClient> hubContext;
    private const string c_signalrFeatureFlagName = "AzureTfs.SignalR";

    public void SendProgressNotification(
      IVssRequestContext requestContext,
      AsyncGitOperationNotification notification)
    {
      Action eventReporter = (Action) (() => this.hubContext.Clients.Group(this.GetGroupName(requestContext, notification.OperationId)).ReportProgress(requestContext, notification));
      this.TryReportEvent(requestContext, eventReporter);
    }

    public void SendCompletionNotification(
      IVssRequestContext requestContext,
      AsyncGitOperationNotification notification)
    {
      Action eventReporter = (Action) (() => this.hubContext.Clients.Group(this.GetGroupName(requestContext, notification.OperationId)).ReportCompletion(requestContext, notification));
      this.TryReportEvent(requestContext, eventReporter);
    }

    public void SendFailureNotification(
      IVssRequestContext requestContext,
      AsyncGitOperationNotification notification)
    {
      Action eventReporter = (Action) (() => this.hubContext.Clients.Group(this.GetGroupName(requestContext, notification.OperationId)).ReportFailure(requestContext, notification));
      this.TryReportEvent(requestContext, eventReporter);
    }

    public void SendTimeoutNotification(
      IVssRequestContext requestContext,
      AsyncGitOperationNotification notification)
    {
      Action eventReporter = (Action) (() => this.hubContext.Clients.Group(this.GetGroupName(requestContext, notification.OperationId)).ReportTimeout(requestContext, notification));
      this.TryReportEvent(requestContext, eventReporter);
    }

    public Task Unsubscribe(
      IVssRequestContext requestContext,
      int operationId,
      string connectionId)
    {
      return this.hubContext.Groups.Remove(connectionId, this.GetGroupName(requestContext, operationId));
    }

    public Task Subscribe(IVssRequestContext requestContext, int operationId, string connectionId) => this.hubContext.Groups.Add(connectionId, this.GetGroupName(requestContext, operationId));

    private string GetGroupName(IVssRequestContext requestContext, int operationId) => string.Format("{0}_AsyncGitOperation_{1}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) operationId);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.hubContext = (IHubContext<IAsyncGitOperationClient>) null;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.hubContext = GlobalHost.ConnectionManager.GetHubContext<AsyncGitOperationHub, IAsyncGitOperationClient>();

    private void TryReportEvent(IVssRequestContext requestContext, Action eventReporter)
    {
      if (!requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "AzureTfs.SignalR"))
        return;
      try
      {
        eventReporter();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1013578, this.GetType().Namespace, this.GetType().Name, ex);
      }
    }
  }
}
