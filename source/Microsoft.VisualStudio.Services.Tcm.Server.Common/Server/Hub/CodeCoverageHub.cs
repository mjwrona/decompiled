// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Hub.CodeCoverageHub
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server.Hub
{
  [CLSCompliant(false)]
  public class CodeCoverageHub : VssHub
  {
    public override async Task OnConnected()
    {
      CodeCoverageHub codeCoverageHub = this;
      // ISSUE: reference to a compiler-generated method
      await codeCoverageHub.\u003C\u003En__0();
      // ISSUE: explicit non-virtual call
      __nonvirtual (codeCoverageHub.VssRequestContext).TraceVerbose("SignalRTraceLayer", "CodeCoverage hub is connected");
    }

    public override async Task OnDisconnected(bool stopCalled)
    {
      CodeCoverageHub codeCoverageHub = this;
      // ISSUE: reference to a compiler-generated method
      await codeCoverageHub.\u003C\u003En__1(stopCalled);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      await __nonvirtual (codeCoverageHub.VssRequestContext).GetService<ICodeCoverageHubDispatcher>().OnDisconnected(__nonvirtual (codeCoverageHub.VssRequestContext), __nonvirtual (codeCoverageHub.Context)?.ConnectionId, stopCalled);
      // ISSUE: explicit non-virtual call
      __nonvirtual (codeCoverageHub.VssRequestContext).TraceVerbose("SignalRTraceLayer", "CodeCoverage hub is disconnected");
    }

    public void UnWatchBuild(int buildId) => this.VssRequestContext.GetService<IBuildCodeCoverageHubDispatcher>().UnWatch(this.VssRequestContext, buildId, this.Context?.ConnectionId);

    public void WatchBuild(int buildId) => this.VssRequestContext.GetService<IBuildCodeCoverageHubDispatcher>().Watch(this.VssRequestContext, buildId, this.Context?.ConnectionId);
  }
}
