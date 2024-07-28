// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.KanbanBoard.KanbanBoardHub
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.KanbanBoard
{
  public sealed class KanbanBoardHub : VssHub<IKanbanBoardHubClient>
  {
    public Task WatchKanbanBoard(Guid workItemTypeExtensionId)
    {
      ArgumentUtility.CheckForEmptyGuid(workItemTypeExtensionId, nameof (workItemTypeExtensionId));
      return this.VssRequestContext.GetService<KanbanBoardHubDispatcher>().WatchKanbanBoard(this.VssRequestContext, workItemTypeExtensionId, this.Context.ConnectionId);
    }

    public Task UnwatchKanbanBoard(Guid workItemTypeExtensionId)
    {
      ArgumentUtility.CheckForEmptyGuid(workItemTypeExtensionId, nameof (workItemTypeExtensionId));
      return this.VssRequestContext.GetService<KanbanBoardHubDispatcher>().UnwatchKanbanBoard(this.VssRequestContext, workItemTypeExtensionId, this.Context.ConnectionId);
    }
  }
}
