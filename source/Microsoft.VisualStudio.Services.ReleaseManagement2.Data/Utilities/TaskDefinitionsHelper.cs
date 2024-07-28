// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.TaskDefinitionsHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class TaskDefinitionsHelper
  {
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "List is fine here.")]
    public static List<TaskDefinition> GetAllTaskDefinitions(
      IVssRequestContext context,
      Guid projectId,
      bool includeTaskGroups)
    {
      List<TaskDefinition> allTaskDefinitions = new List<TaskDefinition>();
      IDistributedTaskPoolService service = context.GetService<IDistributedTaskPoolService>();
      IList<TaskDefinition> taskDefinitions = service.GetTaskDefinitions(context);
      if (taskDefinitions != null)
        allTaskDefinitions.AddRange((IEnumerable<TaskDefinition>) taskDefinitions);
      if (includeTaskGroups)
      {
        IList<TaskGroup> taskGroups = service.GetTaskGroups(context, projectId);
        if (taskGroups != null)
          allTaskDefinitions.AddRange((IEnumerable<TaskDefinition>) taskGroups);
      }
      return allTaskDefinitions;
    }

    public static IList<TaskDefinition> GetTaskDefinitionsByIds(
      IVssRequestContext context,
      Guid projectId,
      bool includeTaskGroups,
      IList<Guid> taskIds)
    {
      return taskIds != null && taskIds.Count > 0 ? (IList<TaskDefinition>) TaskDefinitionsHelper.GetAllTaskDefinitions(context, projectId, includeTaskGroups).FindAll((Predicate<TaskDefinition>) (x => taskIds.Contains(x.Id))) : (IList<TaskDefinition>) new List<TaskDefinition>();
    }
  }
}
