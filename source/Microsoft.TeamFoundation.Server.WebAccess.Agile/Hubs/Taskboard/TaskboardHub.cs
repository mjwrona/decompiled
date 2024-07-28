// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard.TaskboardHub
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard
{
  public sealed class TaskboardHub : VssHub<ITaskboardHubClient>
  {
    public Task WatchTaskboard(Guid teamId)
    {
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      return this.VssRequestContext.GetService<ITaskboardHubDispatcher>().WatchTaskboard(this.VssRequestContext, teamId, this.Context.ConnectionId);
    }

    public Task UnwatchTaskboard(Guid teamId)
    {
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      return this.VssRequestContext.GetService<ITaskboardHubDispatcher>().UnwatchTaskboard(this.VssRequestContext, teamId, this.Context.ConnectionId);
    }
  }
}
