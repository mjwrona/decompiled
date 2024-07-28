// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WorkItemHub.WorkItemHub
// Assembly: Microsoft.Azure.Boards.WorkItemHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 749A696A-54F8-4B6F-8877-B350F1725C24
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.WorkItemHub.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System.Threading.Tasks;

namespace Microsoft.Azure.Boards.WorkItemHub
{
  public sealed class WorkItemHub : VssHub<IWorkItemHubClient>
  {
    public Task WatchWorkItem(int workItemId)
    {
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      return this.VssRequestContext.GetService<IWorkItemHubDispatcher>().WatchWorkItem(this.VssRequestContext, workItemId, this.Context.ConnectionId);
    }

    public Task UnwatchWorkItem(int workItemId)
    {
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      return this.VssRequestContext.GetService<IWorkItemHubDispatcher>().UnwatchWorkItem(this.VssRequestContext, workItemId, this.Context.ConnectionId);
    }
  }
}
