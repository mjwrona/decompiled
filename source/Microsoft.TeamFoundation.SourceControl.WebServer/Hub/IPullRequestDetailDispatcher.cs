// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Hub.IPullRequestDetailDispatcher
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Hub
{
  [DefaultServiceImplementation(typeof (PullRequestDetailDispatcher))]
  public interface IPullRequestDetailDispatcher : IVssFrameworkService
  {
    GitPullRequest Subscribe(
      IVssRequestContext requestContext,
      int pullRequestId,
      string repoId,
      string clientId);

    Task Unsubscribe(IVssRequestContext requestContext, int pullRequestId, string clientId);

    Task Disconnect(IVssRequestContext requestContext, string clientId, bool stopCalled);

    void SendRealtimeEvent(
      IVssRequestContext requestContext,
      RealTimePullRequestEvent realTimeEvent);
  }
}
