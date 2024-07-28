// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Hub.PullRequestDetailHub
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Hub
{
  public class PullRequestDetailHub : VssHub<IPullRequestDetailClient>
  {
    public GitPullRequest Subscribe(int pullRequestId, string repositoryId) => this.VssRequestContext.GetService<IPullRequestDetailDispatcher>().Subscribe(this.VssRequestContext, pullRequestId, repositoryId, this.Context.ConnectionId);

    public Task Unsubscribe(int pullRequestId) => this.VssRequestContext.GetService<IPullRequestDetailDispatcher>().Unsubscribe(this.VssRequestContext, pullRequestId, this.Context.ConnectionId);

    public override async Task OnDisconnected(bool stopCalled)
    {
      PullRequestDetailHub requestDetailHub = this;
      // ISSUE: reference to a compiler-generated method
      await requestDetailHub.\u003C\u003En__0(stopCalled);
      // ISSUE: explicit non-virtual call
      IVssRequestContext vssRequestContext = __nonvirtual (requestDetailHub.VssRequestContext);
      // ISSUE: explicit non-virtual call
      await vssRequestContext.GetService<IPullRequestDetailDispatcher>().Disconnect(vssRequestContext, __nonvirtual (requestDetailHub.Context).ConnectionId, stopCalled);
    }
  }
}
