// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskStep
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TaskStep : JobStep
  {
    [DataMember(Name = "Environment", EmitDefaultValue = false)]
    private IDictionary<string, string> m_environment;
    [DataMember(Name = "Inputs", EmitDefaultValue = false)]
    private IDictionary<string, string> m_inputs;

    [JsonConstructor]
    public TaskStep()
    {
    }

    public TaskStep(TaskInstance legacyTaskInstance)
    {
      this.ContinueOnError = legacyTaskInstance.ContinueOnError;
      this.DisplayName = legacyTaskInstance.DisplayName;
      this.Enabled = legacyTaskInstance.Enabled;
      this.Id = legacyTaskInstance.InstanceId;
      this.Name = legacyTaskInstance.RefName;
      this.TimeoutInMinutes = legacyTaskInstance.TimeoutInMinutes;
      this.RetryCountOnTaskFailure = legacyTaskInstance.RetryCountOnTaskFailure;
      this.Reference = new TaskStepDefinitionReference()
      {
        Id = legacyTaskInstance.Id,
        Name = legacyTaskInstance.Name,
        Version = legacyTaskInstance.Version
      };
      if (!string.IsNullOrEmpty(legacyTaskInstance.Condition))
        this.Condition = legacyTaskInstance.Condition;
      else if (legacyTaskInstance.AlwaysRun)
        this.Condition = "succeededOrFailed()";
      else
        this.Condition = "succeeded()";
      foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) legacyTaskInstance.Inputs)
        this.Inputs[input.Key] = input.Value;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) legacyTaskInstance.Environment)
        this.Environment[keyValuePair.Key] = keyValuePair.Value;
    }

    private TaskStep(TaskStep taskToClone)
      : base((JobStep) taskToClone)
    {
      this.Reference = taskToClone.Reference?.Clone();
      this.IsServerOwned = taskToClone.IsServerOwned;
      IDictionary<string, string> environment = taskToClone.m_environment;
      if ((environment != null ? (environment.Count > 0 ? 1 : 0) : 0) != 0)
        this.m_environment = (IDictionary<string, string>) new Dictionary<string, string>(taskToClone.m_environment, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IDictionary<string, string> inputs = taskToClone.m_inputs;
      if ((inputs != null ? (inputs.Count > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_inputs = (IDictionary<string, string>) new Dictionary<string, string>(taskToClone.m_inputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public override StepType Type => StepType.Task;

    [DataMember]
    public TaskStepDefinitionReference Reference { get; set; }

    [DataMember]
    public bool? IsServerOwned { get; set; }

    public IDictionary<string, string> Environment
    {
      get
      {
        if (this.m_environment == null)
          this.m_environment = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_environment;
      }
    }

    public IDictionary<string, string> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_inputs;
      }
    }

    public override Step Clone() => (Step) new TaskStep(this);

    internal TaskInstance ToLegacyTaskInstance()
    {
      TaskInstance taskInstance = new TaskInstance();
      taskInstance.AlwaysRun = string.Equals(this.Condition ?? string.Empty, "succeededOrFailed()", StringComparison.Ordinal);
      taskInstance.Condition = this.Condition;
      taskInstance.ContinueOnError = this.ContinueOnError;
      taskInstance.DisplayName = this.DisplayName;
      taskInstance.Enabled = this.Enabled;
      taskInstance.InstanceId = this.Id;
      taskInstance.RefName = this.Name;
      taskInstance.TimeoutInMinutes = this.TimeoutInMinutes;
      taskInstance.RetryCountOnTaskFailure = this.RetryCountOnTaskFailure;
      taskInstance.Id = this.Reference.Id;
      taskInstance.Name = this.Reference.Name;
      taskInstance.Version = this.Reference.Version;
      TaskInstance legacyTaskInstance = taskInstance;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) this.Environment)
        legacyTaskInstance.Environment[keyValuePair.Key] = keyValuePair.Value;
      foreach (KeyValuePair<string, string> input in (IEnumerable<KeyValuePair<string, string>>) this.Inputs)
        legacyTaskInstance.Inputs[input.Key] = input.Value;
      return legacyTaskInstance;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, string> environment = this.m_environment;
      if ((environment != null ? (environment.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_environment = (IDictionary<string, string>) null;
      IDictionary<string, string> inputs = this.m_inputs;
      if ((inputs != null ? (inputs.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_inputs = (IDictionary<string, string>) null;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.m_environment != null)
        this.m_environment = (IDictionary<string, string>) new Dictionary<string, string>(this.m_environment, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.m_inputs == null)
        return;
      this.m_inputs = (IDictionary<string, string>) new Dictionary<string, string>(this.m_inputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
