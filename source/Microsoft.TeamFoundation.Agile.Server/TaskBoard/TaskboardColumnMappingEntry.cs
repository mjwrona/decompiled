// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumnMappingEntry
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  public class TaskboardColumnMappingEntry
  {
    public string WorkItemType { get; set; }

    public string State { get; set; }

    public TaskboardColumnMapping Convert() => new TaskboardColumnMapping(this.WorkItemType, this.State);

    public static List<TaskboardColumnMappingEntry> Convert(
      IReadOnlyCollection<ITaskboardColumnMapping> mappings)
    {
      return mappings.Select<ITaskboardColumnMapping, TaskboardColumnMappingEntry>((Func<ITaskboardColumnMapping, TaskboardColumnMappingEntry>) (m => new TaskboardColumnMappingEntry()
      {
        WorkItemType = m.WorkItemType,
        State = m.State
      })).ToList<TaskboardColumnMappingEntry>();
    }
  }
}
