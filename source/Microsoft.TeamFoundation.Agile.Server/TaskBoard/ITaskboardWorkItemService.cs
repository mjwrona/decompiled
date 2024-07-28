// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.ITaskboardWorkItemService
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  [DefaultServiceImplementation(typeof (TaskboardWorkItemService))]
  public interface ITaskboardWorkItemService : IVssFrameworkService
  {
    IReadOnlyCollection<TaskboardWorkItemColumn> GetWorkItemColumns(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      Guid iterationId);

    IReadOnlyCollection<TaskboardWorkItemColumn> GetWorkItemColumns(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      Dictionary<int, (string wit, string state)> workItemIdToTypeStateMap,
      TaskboardColumns columns);

    void UpdateWorkItemColumn(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      Guid iterationId,
      int workItemId,
      string newColumn);
  }
}
