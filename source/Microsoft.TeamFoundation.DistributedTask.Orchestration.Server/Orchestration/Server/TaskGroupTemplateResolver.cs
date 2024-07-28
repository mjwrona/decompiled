// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskGroupTemplateResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class TaskGroupTemplateResolver : ITaskTemplateResolver
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;

    public TaskGroupTemplateResolver(IVssRequestContext requestContext, Guid projectId)
    {
      this.m_projectId = projectId;
      this.m_requestContext = requestContext;
    }

    public bool CanResolve(TaskTemplateReference template) => template.Id != Guid.Empty;

    public IList<TaskStep> ResolveTasks(TaskTemplateStep template)
    {
      TaskGroup taskGroup = this.m_requestContext.GetService<IDistributedTaskPoolService>().GetTaskGroup(this.m_requestContext, this.m_projectId, template.Reference.Id, template.Reference.Version, new bool?(true));
      if (taskGroup == null || taskGroup.Disabled)
      {
        string str1 = taskGroup == null ? "TaskGroup {0} with name {1} and version {2} is missing" : "TaskGroup {0} with name {1} and version {2} is disabled";
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        string format = str1;
        // ISSUE: variable of a boxed type
        __Boxed<Guid> id = (ValueType) template.Id;
        string name = template.Name;
        TaskVersion version = taskGroup?.Version;
        string str2 = (object) version != null ? (string) version : template.Reference.Version;
        throw new MetaTaskDefinitionNotFoundException(string.Format((IFormatProvider) invariantCulture, format, (object) id, (object) name, (object) str2));
      }
      return TaskGroupTemplateResolver.ExpandTasks(template, taskGroup);
    }

    private static IList<TaskStep> ExpandTasks(TaskTemplateStep templateStep, TaskGroup taskGroup)
    {
      IDictionary<string, string> parameters = TaskGroupTemplateResolver.GetParameters(templateStep, taskGroup);
      return (IList<TaskStep>) taskGroup.Tasks.Select<TaskGroupStep, TaskStep>((Func<TaskGroupStep, TaskStep>) (groupTask =>
      {
        TaskStep taskStep = new TaskStep()
        {
          ContinueOnError = groupTask.ContinueOnError,
          Enabled = groupTask.Enabled,
          TimeoutInMinutes = groupTask.TimeoutInMinutes,
          RetryCountOnTaskFailure = groupTask.RetryCountOnTaskFailure,
          DisplayName = groupTask.DisplayName,
          Reference = new TaskStepDefinitionReference()
          {
            Id = groupTask.Task.Id,
            Version = groupTask.Task.VersionSpec
          }
        };
        if (groupTask.AlwaysRun)
          taskStep.Condition = "always()";
        taskStep.DisplayName = VariableUtility.ExpandVariables(taskStep.DisplayName, parameters, false);
        taskStep.Enabled &= templateStep.Enabled;
        taskStep.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) groupTask.Inputs.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, string>) (x => VariableUtility.ExpandVariables(x.Value, parameters, false)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
        taskStep.Environment.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) groupTask.Environment.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, string>) (x => VariableUtility.ExpandVariables(x.Value, parameters, false)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(groupTask.Condition))
          taskStep.Condition = VariableUtility.ExpandConditionVariables(groupTask.Condition, parameters, false);
        return taskStep;
      })).ToList<TaskStep>();
    }

    private static IDictionary<string, string> GetParameters(
      TaskTemplateStep templateStep,
      TaskGroup taskGroup)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      Dictionary<string, string> dictionary = taskGroup.Inputs.ToDictionary<TaskInputDefinition, string, string>((Func<TaskInputDefinition, string>) (x => x.Name), (Func<TaskInputDefinition, string>) (x => x.DefaultValue), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) templateStep.Parameters)
      {
        string str = parameter.Value;
        if (string.IsNullOrEmpty(str) && dictionary.ContainsKey(parameter.Key))
          str = dictionary[parameter.Key];
        parameters.Add(parameter.Key, str);
      }
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        if (!templateStep.Parameters.ContainsKey(keyValuePair.Key) && !string.IsNullOrEmpty(keyValuePair.Value))
          parameters.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return (IDictionary<string, string>) parameters;
    }
  }
}
