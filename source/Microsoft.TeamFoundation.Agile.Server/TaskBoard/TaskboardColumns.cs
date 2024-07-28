// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumns
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  public class TaskboardColumns
  {
    public WebApiTeam Team { get; private set; }

    public IReadOnlyCollection<TaskboardColumn> Columns { get; private set; }

    public bool IsCustomized { get; private set; }

    public bool IsValidMapping { get; private set; }

    public Exception ValidationException { get; private set; }

    public TaskboardColumns(
      WebApiTeam team,
      IEnumerable<TaskboardColumn> columns,
      bool isCustomized,
      bool isValidMapping,
      Exception validationException)
    {
      this.Team = team;
      this.Columns = (IReadOnlyCollection<TaskboardColumn>) columns.OrderBy<TaskboardColumn, int>((Func<TaskboardColumn, int>) (c => c.Order)).ToList<TaskboardColumn>();
      this.IsCustomized = isCustomized;
      this.IsValidMapping = isValidMapping;
      this.ValidationException = validationException;
    }

    public Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumns Convert() => new Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumns()
    {
      Columns = (IReadOnlyCollection<Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumn>) this.Columns.Select<TaskboardColumn, Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumn>((Func<TaskboardColumn, Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumn>) (c => c.Convert())).ToList<Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumn>(),
      IsCustomized = this.IsCustomized,
      IsValid = this.IsValidMapping,
      ValidationMesssage = this.ValidationException?.Message
    };
  }
}
