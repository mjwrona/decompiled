// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Hub.CodeCoverageHubDispatcher
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.AspNet.SignalR;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server.Hub
{
  [CLSCompliant(false)]
  public class CodeCoverageHubDispatcher : 
    ICodeCoverageHubDispatcher,
    IVssFrameworkService,
    IBuildCodeCoverageHubDispatcher
  {
    private IHubContext<ICodeCoverageHubClient> m_hubContext;
    private static string layer = nameof (CodeCoverageHubDispatcher);

    void IBuildCodeCoverageHubDispatcher.SendCoverageStatsChanged(
      IVssRequestContext requestContext,
      int buildId)
    {
      Action invoker = (Action) (() =>
      {
        requestContext.TraceInfo(CodeCoverageHubDispatcher.layer, "Sending Code Coverage SignalR event for build {0}", (object) buildId);
        this.GetBuildClientGroup(requestContext, buildId).codeCoverageStatsChangedForBuild(requestContext, buildId);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("BuildId", (double) buildId);
        this.PublishTelemetry(requestContext, properties);
      });
      this.SendEvent(requestContext, invoker);
    }

    void IBuildCodeCoverageHubDispatcher.HandleCoverageStatsChange(
      TestManagementRequestContext tcmRequestContext,
      Guid projectId,
      int buildId)
    {
      throw new NotImplementedException();
    }

    void IBuildCodeCoverageHubDispatcher.DeleteNotification(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      throw new NotImplementedException();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<CodeCoverageHub, ICodeCoverageHubClient>();
      systemRequestContext.TraceVerbose("SignalRTraceLayer", "CodeCoverage hub service start");
    }

    async Task IBuildCodeCoverageHubDispatcher.UnWatch(
      IVssRequestContext requestContext,
      int buildId,
      string clientId)
    {
      await this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "CodeCoverageHub", CodeCoverageHubDispatcher.GetBuildGroupName(requestContext, buildId), clientId);
    }

    async Task IBuildCodeCoverageHubDispatcher.Watch(
      IVssRequestContext requestContext,
      int buildId,
      string clientId)
    {
      await this.m_hubContext.Groups.AddTrackedConnection(requestContext, "CodeCoverageHub", CodeCoverageHubDispatcher.GetBuildGroupName(requestContext, buildId), clientId);
    }

    bool IBuildCodeCoverageHubDispatcher.AnyClient(IVssRequestContext requestContext, int buildId)
    {
      VssSignalRHubGroup group = requestContext.GetService<IVssSignalRHubGroupService>().GetGroup(requestContext, "CodeCoverageHub", CodeCoverageHubDispatcher.GetBuildGroupName(requestContext, buildId));
      if (group == null)
        return false;
      int? count = group.Connections?.Count;
      int num = 0;
      return count.GetValueOrDefault() > num & count.HasValue;
    }

    public Task OnDisconnected(
      IVssRequestContext requestContext,
      string connectionId,
      bool stopCalled)
    {
      return this.m_hubContext.RemoveTrackedConnection<ICodeCoverageHubClient>(requestContext, "CodeCoverageHub", connectionId, !stopCalled);
    }

    private static string GetBuildGroupName(IVssRequestContext requestContext, int buildId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BUILD_{0}_{1}", (object) requestContext.ServiceHost.InstanceId, (object) buildId);

    private ICodeCoverageHubClient GetBuildClientGroup(
      IVssRequestContext requestContext,
      int buildId)
    {
      // ISSUE: reference to a compiler-generated field
      if (CodeCoverageHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        CodeCoverageHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, ICodeCoverageHubClient>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ICodeCoverageHubClient), typeof (CodeCoverageHubDispatcher)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return CodeCoverageHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) CodeCoverageHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<ICodeCoverageHubClient>(requestContext, "CodeCoverageHub", CodeCoverageHubDispatcher.GetBuildGroupName(requestContext, buildId)));
    }

    protected void SendEvent(IVssRequestContext requestContext, Action invoker)
    {
      try
      {
        invoker();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(CodeCoverageHubDispatcher.layer, ex);
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
        requestContext.TraceException(CodeCoverageHubDispatcher.layer, ex);
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
