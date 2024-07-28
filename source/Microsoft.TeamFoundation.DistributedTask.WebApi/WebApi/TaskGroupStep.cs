// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskGroupStep
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskGroupStep
  {
    [DataMember(Name = "Environment", EmitDefaultValue = false)]
    private Dictionary<string, string> m_environment;
    private string m_displayName;

    public TaskGroupStep()
    {
    }

    private TaskGroupStep(TaskGroupStep taskGroupStep)
    {
      this.DisplayName = taskGroupStep.DisplayName;
      this.AlwaysRun = taskGroupStep.AlwaysRun;
      this.ContinueOnError = taskGroupStep.ContinueOnError;
      this.Enabled = taskGroupStep.Enabled;
      this.TimeoutInMinutes = taskGroupStep.TimeoutInMinutes;
      this.RetryCountOnTaskFailure = taskGroupStep.RetryCountOnTaskFailure;
      this.Inputs = (IDictionary<string, string>) new Dictionary<string, string>(taskGroupStep.Inputs);
      if (taskGroupStep.m_environment != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in taskGroupStep.m_environment)
          this.Environment[keyValuePair.Key] = keyValuePair.Value;
      }
      this.Task = taskGroupStep.Task.Clone();
    }

    [DataMember]
    public string DisplayName
    {
      get
      {
        if (this.m_displayName == null)
          this.m_displayName = string.Empty;
        return this.m_displayName;
      }
      set => this.m_displayName = value;
    }

    [DataMember]
    public bool AlwaysRun { get; set; }

    [DataMember]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Condition { get; set; }

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public int TimeoutInMinutes { get; set; }

    [DataMember]
    public int RetryCountOnTaskFailure { get; set; }

    [DataMember]
    public IDictionary<string, string> Inputs { get; set; }

    public IDictionary<string, string> Environment
    {
      get
      {
        if (this.m_environment == null)
          this.m_environment = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        return (IDictionary<string, string>) this.m_environment;
      }
    }

    [DataMember]
    public TaskDefinitionReference Task { get; set; }

    public static bool EqualsAndOldTaskInputsAreSubsetOfNewTaskInputs(
      TaskGroupStep oldTaskGroupStep,
      TaskGroupStep newTaskGroupStep)
    {
      return oldTaskGroupStep.DisplayName.Equals(newTaskGroupStep.DisplayName) && oldTaskGroupStep.AlwaysRun == newTaskGroupStep.AlwaysRun && oldTaskGroupStep.Enabled == newTaskGroupStep.Enabled && oldTaskGroupStep.ContinueOnError == newTaskGroupStep.ContinueOnError && oldTaskGroupStep.Task.Equals((object) newTaskGroupStep.Task) && oldTaskGroupStep.Inputs != null && newTaskGroupStep.Inputs != null && oldTaskGroupStep.Inputs.Keys.All<string>((Func<string, bool>) (key => newTaskGroupStep.Inputs.ContainsKey(key) && string.Equals(newTaskGroupStep.Inputs[key], oldTaskGroupStep.Inputs[key], StringComparison.OrdinalIgnoreCase))) && oldTaskGroupStep.Environment != null && oldTaskGroupStep.Environment.Keys.All<string>((Func<string, bool>) (key => newTaskGroupStep.Environment.ContainsKey(key) && string.Equals(newTaskGroupStep.Environment[key], oldTaskGroupStep.Environment[key], StringComparison.OrdinalIgnoreCase)));
    }

    internal TaskGroupStep Clone() => new TaskGroupStep(this);
  }
}
