// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumn
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  public class TaskboardColumn : ITaskboardColumn
  {
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public int Order { get; private set; }

    public DateTime ChangeDate { get; private set; }

    public IReadOnlyCollection<ITaskboardColumnMapping> Mappings { get; private set; }

    public TaskboardColumn(
      Guid id,
      string name,
      int order,
      DateTime changeDate,
      IReadOnlyCollection<TaskboardColumnMapping> mappings)
    {
      this.Id = id;
      this.Name = name;
      this.Order = order;
      this.Mappings = (IReadOnlyCollection<ITaskboardColumnMapping>) mappings;
      this.ChangeDate = changeDate;
    }

    public Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumn Convert() => new Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumn()
    {
      Id = this.Id,
      Name = this.Name,
      Order = this.Order,
      Mappings = (IReadOnlyCollection<ITaskboardColumnMapping>) this.Mappings.Select<ITaskboardColumnMapping, Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumnMapping>((Func<ITaskboardColumnMapping, Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumnMapping>) (m => (m as TaskboardColumnMapping).Convert())).ToList<Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard.TaskboardColumnMapping>()
    };
  }
}
