// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.WorkflowTaskExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class WorkflowTaskExtensions
  {
    public static string ToJsonString(this IEnumerable<WorkflowTask> tasks) => tasks != null ? JsonConvert.SerializeObject((object) tasks) : (string) null;

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    public static List<WorkflowTask> ToWorkflowTaskList(this string jsonObject)
    {
      if (string.IsNullOrEmpty(jsonObject))
        return (List<WorkflowTask>) null;
      List<WorkflowTask> workflowTasks = JsonConvert.DeserializeObject<List<WorkflowTask>>(jsonObject);
      workflowTasks.SanitizeTaskRefNames();
      workflowTasks.TrimDuplicateRefNames();
      return workflowTasks;
    }

    public static void TrimDuplicateRefNames(this IEnumerable<WorkflowTask> workflowTasks)
    {
      if (workflowTasks == null)
        return;
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkflowTask workflowTask in workflowTasks)
      {
        if (workflowTask != null && !string.IsNullOrEmpty(workflowTask.RefName))
        {
          if (stringSet.Contains(workflowTask.RefName))
            workflowTask.RefName = string.Empty;
          else
            stringSet.Add(workflowTask.RefName);
        }
      }
    }

    public static void EnsureNoDuplicateRefNames(this IEnumerable<WorkflowTask> workflowTasks)
    {
      if (workflowTasks == null)
        return;
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkflowTask workflowTask in workflowTasks)
      {
        if (workflowTask != null && !string.IsNullOrEmpty(workflowTask.RefName))
        {
          if (stringSet.Contains(workflowTask.RefName))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DuplicateRefNameUsedInPhase, (object) workflowTask.RefName)).Expected("ReleaseManagementService");
          stringSet.Add(workflowTask.RefName);
        }
      }
    }

    public static void SanitizeTaskRefNames(this IEnumerable<WorkflowTask> workflowTasks)
    {
      if (workflowTasks == null)
        return;
      foreach (WorkflowTask workflowTask in workflowTasks)
      {
        if (workflowTask != null)
          workflowTask.RefName = NameValidation.Sanitize(workflowTask.RefName);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Will have to do double conversions otherwise.")]
    public static List<WorkflowTask> DeepClone(this IEnumerable<WorkflowTask> tasks)
    {
      List<WorkflowTask> workflowTaskList = new List<WorkflowTask>();
      if (tasks != null && tasks.Count<WorkflowTask>() > 0)
      {
        foreach (WorkflowTask task in tasks)
          workflowTaskList.Add(new WorkflowTask(task));
      }
      return workflowTaskList;
    }

    public static bool IsMetaTask(this WorkflowTask task) => task != null && task.DefinitionType != null && task.DefinitionType.Equals("metaTask", StringComparison.OrdinalIgnoreCase);
  }
}
