// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.WorkflowTask
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class WorkflowTask : ReleaseManagementSecuredObject
  {
    [DataMember(Name = "Environment", EmitDefaultValue = false)]
    private Dictionary<string, string> m_environment;
    private Dictionary<string, string> inputs;
    private const string NameValuePairFormat = "{0}:{1},";

    public WorkflowTask() => this.OverrideInputs = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public WorkflowTask(WorkflowTask task)
    {
      this.TaskId = task != null ? task.TaskId : throw new ArgumentNullException(nameof (task));
      this.Version = task.Version;
      this.Name = task.Name;
      this.RefName = task.RefName;
      this.Enabled = task.Enabled;
      this.AlwaysRun = task.AlwaysRun;
      this.ContinueOnError = task.ContinueOnError;
      this.DefinitionType = task.DefinitionType;
      this.TimeoutInMinutes = task.TimeoutInMinutes;
      this.Condition = task.Condition;
      this.RetryCountOnTaskFailure = task.RetryCountOnTaskFailure;
      this.CheckConfig = task.CheckConfig;
      if (task.inputs != null)
        this.inputs = new Dictionary<string, string>((IDictionary<string, string>) task.inputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (task.OverrideInputs != null)
        this.OverrideInputs = (IDictionary<string, string>) new Dictionary<string, string>(task.OverrideInputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (task.m_environment == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in task.m_environment)
        this.Environment[keyValuePair.Key] = keyValuePair.Value;
    }

    [DataMember(IsRequired = true)]
    public Guid TaskId { get; set; }

    [DataMember(IsRequired = true)]
    public string Version { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RefName { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(Name = "Inputs", EmitDefaultValue = false, Order = 2)]
    public Dictionary<string, string> Inputs
    {
      get => this.inputs ?? (this.inputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
      set => this.inputs = value;
    }

    [DataMember(EmitDefaultValue = true)]
    public bool Enabled { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool AlwaysRun { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public int TimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public int RetryCountOnTaskFailure { get; set; }

    [DataMember]
    public string DefinitionType { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> OverrideInputs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Condition { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public CheckConfigurationReference CheckConfig { get; set; }

    public IDictionary<string, string> Environment
    {
      get
      {
        if (this.m_environment == null)
          this.m_environment = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        return (IDictionary<string, string>) this.m_environment;
      }
    }

    public static bool EqualsAndOldTaskInputsSubsetNewTaskInputs(
      WorkflowTask oldWorkflowTask,
      WorkflowTask newWorkflowTask)
    {
      if (oldWorkflowTask == null || newWorkflowTask == null || oldWorkflowTask.TaskId != newWorkflowTask.TaskId || oldWorkflowTask.Version != "*" && newWorkflowTask.Version != "*" && !string.Equals(oldWorkflowTask.Version, newWorkflowTask.Version, StringComparison.OrdinalIgnoreCase) || !string.Equals(oldWorkflowTask.Name, newWorkflowTask.Name, StringComparison.OrdinalIgnoreCase) || !string.Equals(oldWorkflowTask.RefName, newWorkflowTask.RefName, StringComparison.OrdinalIgnoreCase) || oldWorkflowTask.Inputs != null && newWorkflowTask.Inputs == null || oldWorkflowTask.Inputs == null && newWorkflowTask.Inputs != null || oldWorkflowTask.Environment != null && newWorkflowTask.Environment == null || oldWorkflowTask.Environment == null && newWorkflowTask.Environment != null || oldWorkflowTask.DefinitionType != newWorkflowTask.DefinitionType || oldWorkflowTask.CheckConfig != newWorkflowTask.CheckConfig || oldWorkflowTask.Enabled != newWorkflowTask.Enabled || oldWorkflowTask.AlwaysRun != newWorkflowTask.AlwaysRun || oldWorkflowTask.ContinueOnError != newWorkflowTask.ContinueOnError || oldWorkflowTask.TimeoutInMinutes != newWorkflowTask.TimeoutInMinutes || oldWorkflowTask.RetryCountOnTaskFailure != newWorkflowTask.RetryCountOnTaskFailure || !string.Equals(oldWorkflowTask.Condition, oldWorkflowTask.Condition, StringComparison.OrdinalIgnoreCase))
        return false;
      if (oldWorkflowTask.Inputs != null && newWorkflowTask.Inputs != null)
      {
        foreach (string key in oldWorkflowTask.Inputs.Keys)
        {
          if (!newWorkflowTask.Inputs.ContainsKey(key) || !string.Equals(oldWorkflowTask.Inputs[key], newWorkflowTask.Inputs[key], StringComparison.OrdinalIgnoreCase))
            return false;
        }
      }
      if (oldWorkflowTask.Environment != null || newWorkflowTask.Environment != null)
      {
        if (oldWorkflowTask.Environment.Count != newWorkflowTask.Environment.Count)
          return false;
        foreach (string key in (IEnumerable<string>) oldWorkflowTask.Environment.Keys)
        {
          if (!newWorkflowTask.Environment.ContainsKey(key) || !string.Equals(oldWorkflowTask.Environment[key], newWorkflowTask.Environment[key], StringComparison.OrdinalIgnoreCase))
            return false;
        }
      }
      IDictionary<string, string> overrideInputs1 = oldWorkflowTask.OverrideInputs;
      IDictionary<string, string> overrideInputs2 = newWorkflowTask.OverrideInputs;
      if (overrideInputs1 == null && overrideInputs2 == null)
        return true;
      if (overrideInputs1 == null)
        return overrideInputs2.Count == 0;
      if (overrideInputs2 == null)
        return overrideInputs1.Count == 0;
      if (overrideInputs1 != null && overrideInputs2 != null)
      {
        foreach (string key in (IEnumerable<string>) overrideInputs1.Keys)
        {
          if (!overrideInputs2.ContainsKey(key) || !string.Equals(overrideInputs1[key], overrideInputs2[key], StringComparison.OrdinalIgnoreCase))
            return false;
        }
      }
      return true;
    }

    public override bool Equals(object obj) => this.Equals(obj, false);

    public bool Equals(object value, bool ignoreTaskInputsTaskNameAndControlOptions)
    {
      if (!(value is WorkflowTask workflowTask) || this.TaskId != workflowTask.TaskId || this.Version != "*" && workflowTask.Version != "*" && !string.Equals(this.Version, workflowTask.Version, StringComparison.OrdinalIgnoreCase) || this.Inputs != null && workflowTask.Inputs == null || this.Inputs == null && workflowTask.Inputs != null || this.Environment != null && workflowTask.Environment == null || this.Environment == null && workflowTask.Environment != null || this.DefinitionType != workflowTask.DefinitionType || this.CheckConfig != workflowTask.CheckConfig)
        return false;
      if (!ignoreTaskInputsTaskNameAndControlOptions)
      {
        if (this.Enabled != workflowTask.Enabled || this.AlwaysRun != workflowTask.AlwaysRun || this.ContinueOnError != workflowTask.ContinueOnError || this.TimeoutInMinutes != workflowTask.TimeoutInMinutes || this.RetryCountOnTaskFailure != workflowTask.RetryCountOnTaskFailure || !string.Equals(this.Name, workflowTask.Name, StringComparison.OrdinalIgnoreCase) || !string.Equals(this.RefName, workflowTask.RefName, StringComparison.OrdinalIgnoreCase) || !string.Equals(this.Condition, workflowTask.Condition, StringComparison.OrdinalIgnoreCase))
          return false;
        if (this.Inputs != null && workflowTask.Inputs != null)
        {
          if (this.Inputs.Count != workflowTask.Inputs.Count)
            return false;
          foreach (string key in this.Inputs.Keys)
          {
            if (!workflowTask.Inputs.ContainsKey(key) || !string.Equals(this.Inputs[key], workflowTask.Inputs[key], StringComparison.OrdinalIgnoreCase))
              return false;
          }
        }
        if (this.Environment != null || workflowTask.Environment != null)
        {
          if (this.Environment.Count != workflowTask.Environment.Count)
            return false;
          foreach (string key in (IEnumerable<string>) this.Environment.Keys)
          {
            if (!workflowTask.Environment.ContainsKey(key) || !string.Equals(this.Environment[key], workflowTask.Environment[key], StringComparison.OrdinalIgnoreCase))
              return false;
          }
        }
        if (this.OverrideInputs == null && workflowTask.OverrideInputs == null)
          return true;
        if (this.OverrideInputs == null)
          return workflowTask.OverrideInputs.Count == 0;
        if (workflowTask.OverrideInputs == null)
          return this.OverrideInputs.Count == 0;
        if (this.OverrideInputs != null && workflowTask.OverrideInputs != null)
        {
          if (this.OverrideInputs.Count != workflowTask.OverrideInputs.Count)
            return false;
          foreach (string key in (IEnumerable<string>) this.OverrideInputs.Keys)
          {
            if (!workflowTask.OverrideInputs.ContainsKey(key) || !string.Equals(this.OverrideInputs[key], workflowTask.OverrideInputs[key], StringComparison.OrdinalIgnoreCase))
              return false;
          }
        }
      }
      return true;
    }

    public override int GetHashCode() => this.ToString().GetHashCode();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "TaskId", (object) this.TaskId));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "Version", (object) this.Version));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "Name", (object) this.Name));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "RefName", (object) this.RefName));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "Enabled", (object) this.Enabled));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "AlwaysRun", (object) this.AlwaysRun));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "ContinueOnError", (object) this.ContinueOnError));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "TaskTimeout", (object) this.TimeoutInMinutes));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "RetryCountOnTaskFailure", (object) this.RetryCountOnTaskFailure));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "DefinitionType", (object) this.DefinitionType));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) "Condition", (object) this.Condition));
      if (this.Inputs != null && this.Inputs.Count > 0)
      {
        foreach (string key in this.Inputs.Keys)
          stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) key, (object) this.Inputs[key]));
      }
      if (this.OverrideInputs != null && this.OverrideInputs.Count > 0)
      {
        foreach (string key in (IEnumerable<string>) this.OverrideInputs.Keys)
          stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) key, (object) this.OverrideInputs[key]));
      }
      if (this.Environment != null && this.Environment.Count > 0)
      {
        foreach (string key in (IEnumerable<string>) this.Environment.Keys)
          stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:{1},", (object) key, (object) this.Environment[key]));
      }
      return stringBuilder.ToString();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.CheckConfig?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
