// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class BuildDefinitionStep
  {
    [DataMember(Name = "Environment", EmitDefaultValue = false)]
    private Dictionary<string, string> m_environment;
    [DataMember(Name = "Inputs", EmitDefaultValue = false, Order = 2)]
    private Dictionary<string, string> m_inputs;

    public BuildDefinitionStep()
    {
    }

    private BuildDefinitionStep(BuildDefinitionStep toClone)
    {
      ArgumentUtility.CheckForNull<BuildDefinitionStep>(toClone, nameof (toClone));
      this.Enabled = toClone.Enabled;
      this.ContinueOnError = toClone.ContinueOnError;
      this.AlwaysRun = toClone.AlwaysRun;
      this.DisplayName = toClone.DisplayName;
      this.TimeoutInMinutes = toClone.TimeoutInMinutes;
      this.RetryCountOnTaskFailure = toClone.RetryCountOnTaskFailure;
      this.Condition = toClone.Condition;
      this.RefName = toClone.RefName;
      if (toClone.TaskDefinition != null)
        this.TaskDefinition = toClone.TaskDefinition.Clone();
      if (toClone.m_inputs != null)
      {
        foreach (KeyValuePair<string, string> input in toClone.m_inputs)
          this.Inputs.Add(input.Key, input.Value);
      }
      if (toClone.m_environment == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in toClone.m_environment)
        this.Environment.Add(keyValuePair.Key, keyValuePair.Value);
    }

    [DataMember(IsRequired = true, Order = 1, Name = "Task")]
    public TaskDefinitionReference TaskDefinition { get; set; }

    public IDictionary<string, string> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, string>) this.m_inputs;
      }
      set
      {
        if (value == null)
          return;
        this.m_inputs = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    [DataMember(EmitDefaultValue = true)]
    public bool Enabled { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool AlwaysRun { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public int TimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public int RetryCountOnTaskFailure { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string Condition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RefName { get; set; }

    public IDictionary<string, string> Environment
    {
      get
      {
        if (this.m_environment == null)
          this.m_environment = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        return (IDictionary<string, string>) this.m_environment;
      }
      set
      {
        if (value == null)
          return;
        this.m_environment = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.Ordinal);
      }
    }

    public BuildDefinitionStep Clone() => new BuildDefinitionStep(this);
  }
}
