// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard.ITaskboardHubDispatcher
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Taskboard
{
  [DefaultServiceImplementation(typeof (TaskboardHubDispatcher))]
  public interface ITaskboardHubDispatcher : IVssFrameworkService
  {
    Task WatchTaskboard(IVssRequestContext requestContext, Guid teamId, string connectionId);

    Task UnwatchTaskboard(IVssRequestContext requestContext, Guid teamId, string connectionId);

    void NotifyWorkItemChanged(
      IVssRequestContext requestContext,
      WorkItemChangedEvent workItemChangedEvent);

    void NotifyWorkItemColumnChanged(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardWorkItemColumnChangedEvent workItemColumnChangedEvent);

    void NotifyColumnOptionsChanged(IVssRequestContext requestContext, Guid projectId, Guid teamId);

    void NotifyTeamSettingsChanged(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Guid> teamIds);

    void NotifyTaskboardCardSettingsChanged(IVssRequestContext requestContext, Guid teamId);
  }
}
