// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DeployPhaseTasksValidatorHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class DeployPhaseTasksValidatorHelper
  {
    public static void ValidateTasks(
      this IList<DeployPhase> deployPhases,
      string environmentName,
      IList<TaskDefinition> taskDefinitions)
    {
      if (deployPhases == null)
        throw new ArgumentNullException(nameof (deployPhases));
      if (taskDefinitions == null)
        throw new ArgumentNullException(nameof (taskDefinitions));
      foreach (DeployPhase deployPhase in (IEnumerable<DeployPhase>) deployPhases)
      {
        string runsOnValue = deployPhase.PhaseType.ToRunsOnValue();
        deployPhase.WorkflowTasks.ValidateTasks(taskDefinitions, environmentName, deployPhase.Name, runsOnValue);
      }
    }

    public static void ValidateTasks(
      this IEnumerable<WorkflowTask> workflowTasks,
      IList<TaskDefinition> taskDefinitions,
      string environmentName,
      string deployPhaseName,
      string taskShouldRunOn)
    {
      if (workflowTasks == null)
        throw new ArgumentNullException(nameof (workflowTasks));
      TaskDefinitionResolver definitionResolver = new TaskDefinitionResolver(taskDefinitions);
      List<string> values1 = new List<string>();
      List<string> values2 = new List<string>();
      foreach (WorkflowTask workflowTask in workflowTasks)
      {
        TaskDefinition taskDefinition = (TaskDefinition) null;
        if (!definitionResolver.TryResolveTaskReference(workflowTask.TaskId, workflowTask.Version, out taskDefinition) || taskDefinition == null)
          values1.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) workflowTask.Name, (object) workflowTask.Version));
        if (taskDefinition != null && !DeployPhaseTasksValidatorHelper.DoesTaskContainCompatibleRunsOn(taskDefinition, taskShouldRunOn))
          values2.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) workflowTask.Name, (object) workflowTask.Version));
      }
      StringBuilder stringBuilder = new StringBuilder();
      if (values1.Count > 0)
      {
        string str = string.Join(", ", (IEnumerable<string>) values1);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidTasksInDeployPhase, (object) str, (object) deployPhaseName, (object) environmentName);
      }
      if (values2.Count > 0)
      {
        string str = string.Join(", ", (IEnumerable<string>) values2);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.IncompatibleTasksInDeployPhase, (object) str, (object) deployPhaseName, (object) environmentName, (object) taskShouldRunOn);
      }
      if (!string.IsNullOrEmpty(stringBuilder.ToString()))
        throw new InvalidRequestException(stringBuilder.ToString());
    }

    private static bool DoesTaskContainCompatibleRunsOn(TaskDefinition task, string taskShouldRunOn)
    {
      if (task.RunsOn.Contains<string>(taskShouldRunOn, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        return true;
      return task.RunsOn.Contains<string>("MachineGroup", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && taskShouldRunOn.Equals("DeploymentGroup", StringComparison.OrdinalIgnoreCase);
    }
  }
}
