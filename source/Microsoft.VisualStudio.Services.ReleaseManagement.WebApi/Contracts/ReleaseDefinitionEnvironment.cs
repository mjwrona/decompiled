// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionEnvironment
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseDefinitionEnvironment : ReleaseManagementSecuredObject
  {
    private static ProcessParameters emptyProcessParameters = new ProcessParameters();
    private PropertiesCollection properties;

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Rank { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef Owner { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, ConfigurationVariableValue> Variables { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<int> VariableGroups { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionApprovals PreDeployApprovals { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionDeployStep DeployStep { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionApprovals PostDeployApprovals { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    [XmlIgnore]
    public IList<DeployPhase> DeployPhases { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int QueueId { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    [Obsolete("This property is deprecated, use EnvironmentOptions instead.")]
    public IDictionary<string, string> RunOptions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EnvironmentOptions EnvironmentOptions { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<Demand> Demands { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<Condition> Conditions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EnvironmentExecutionPolicy ExecutionPolicy { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<ReleaseSchedule> Schedules { get; set; }

    [Obsolete("Use CurrentReleaseReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference CurrentRelease
    {
      get => (ShallowReference) this.CurrentReleaseReference;
      set => this.CurrentReleaseReference = value.ToReleaseShallowReference();
    }

    [DataMember(Name = "CurrentRelease", EmitDefaultValue = false)]
    public ReleaseShallowReference CurrentReleaseReference { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public EnvironmentRetentionPolicy RetentionPolicy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProcessParameters ProcessParameters { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PropertiesCollection Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = new PropertiesCollection();
        return this.properties;
      }
      internal set => this.properties = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionGatesStep PreDeploymentGates { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseDefinitionGatesStep PostDeploymentGates { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<EnvironmentTrigger> EnvironmentTriggers { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string BadgeUrl { get; set; }

    public ReleaseDefinitionEnvironment()
    {
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.EnvironmentOptions = new EnvironmentOptions();
      this.Demands = (IList<Demand>) new List<Demand>();
      this.Conditions = (IList<Condition>) new List<Condition>();
      this.PreDeployApprovals = new ReleaseDefinitionApprovals();
      this.PostDeployApprovals = new ReleaseDefinitionApprovals();
      this.PreDeploymentGates = new ReleaseDefinitionGatesStep();
      this.PostDeploymentGates = new ReleaseDefinitionGatesStep();
      this.EnvironmentTriggers = (IList<EnvironmentTrigger>) new List<EnvironmentTrigger>();
      this.DeployPhases = (IList<DeployPhase>) new List<DeployPhase>();
      this.ExecutionPolicy = new EnvironmentExecutionPolicy();
      this.Schedules = (IList<ReleaseSchedule>) new List<ReleaseSchedule>();
      this.VariableGroups = (IList<int>) new List<int>();
    }

    public override int GetHashCode() => this.Id.GetHashCode();

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "should not hamper the performance")]
    public override bool Equals(object obj) => this.Equals(obj, false);

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "should not hamper the performance")]
    public bool Equals(object obj, bool ignoreRank)
    {
      ReleaseDefinitionEnvironment env2 = obj as ReleaseDefinitionEnvironment;
      if (env2 == null || this.Id != env2.Id || !string.Equals(this.Name, env2.Name, StringComparison.OrdinalIgnoreCase) || !string.Equals(this.Owner.Id, env2.Owner.Id, StringComparison.OrdinalIgnoreCase) || !ignoreRank && this.Rank != env2.Rank || this.ExecutionPolicy == null && env2.ExecutionPolicy != null || this.ExecutionPolicy != null && !this.ExecutionPolicy.Equals((object) env2.ExecutionPolicy) || this.Variables.Count != env2.Variables.Count || this.DeployPhases.Count != env2.DeployPhases.Count || !this.PreDeployApprovals.Equals((object) env2.PreDeployApprovals) || this.DeployStep == null && env2.DeployStep != null || this.DeployStep != null && !this.DeployStep.Equals((object) env2.DeployStep) || !this.PostDeployApprovals.Equals((object) env2.PostDeployApprovals) || !this.EnvironmentOptions.IsEqual(env2.EnvironmentOptions) || this.RetentionPolicy == null && env2.RetentionPolicy != null || this.RetentionPolicy != null && !this.RetentionPolicy.Equals((object) env2.RetentionPolicy) || this.ProcessParameters != null && !this.ProcessParameters.Equals((object) ReleaseDefinitionEnvironment.emptyProcessParameters) && env2.ProcessParameters == null || this.ProcessParameters == null && env2.ProcessParameters != null && !env2.ProcessParameters.Equals((object) ReleaseDefinitionEnvironment.emptyProcessParameters) || !this.PreDeploymentGates.Equals((object) env2.PreDeploymentGates) || !this.PostDeploymentGates.Equals((object) env2.PostDeploymentGates) || this.VariableGroups.Count != env2.VariableGroups.Count || this.VariableGroups.Any<int>((Func<int, bool>) (c1 => !env2.VariableGroups.Any<int>((Func<int, bool>) (c2 => c2.Equals(c1))))) || this.Conditions.Count != env2.Conditions.Count || this.Conditions.Any<Condition>((Func<Condition, bool>) (c1 => !env2.Conditions.Any<Condition>((Func<Condition, bool>) (c2 => c2.Equals((object) c1))))) || this.Schedules.Count != env2.Schedules.Count || this.Schedules.Any<ReleaseSchedule>((Func<ReleaseSchedule, bool>) (s1 => !env2.Schedules.Any<ReleaseSchedule>((Func<ReleaseSchedule, bool>) (s2 => s2.Equals((object) s1))))) || this.DeployPhases.Any<DeployPhase>((Func<DeployPhase, bool>) (dp1 => !env2.DeployPhases.Any<DeployPhase>((Func<DeployPhase, bool>) (dp2 => dp2.Equals(dp1))))) || !this.AreEnvironmentTriggersEqual(this.EnvironmentTriggers, env2.EnvironmentTriggers))
        return false;
      foreach (string key in (IEnumerable<string>) this.Variables.Keys)
      {
        if (!env2.Variables.ContainsKey(key) || this.Variables[key].IsSecret != env2.Variables[key].IsSecret || !string.Equals(this.Variables[key].Value, env2.Variables[key].Value, StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return this.ProcessParameters == null || env2.ProcessParameters == null || this.ProcessParameters.Equals((object) env2.ProcessParameters);
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.ProcessParameters = this.ProcessParameters?.Clone((ISecuredObject) new ReleaseManagementSecuredObject(token, requiredPermissions));
      IList<Condition> conditions = this.Conditions;
      if (conditions != null)
        conditions.ForEach<Condition>((Action<Condition>) (i => i.SetSecuredObject(token, requiredPermissions)));
      IDictionary<string, ConfigurationVariableValue> variables = this.Variables;
      if (variables != null)
        variables.ForEach<KeyValuePair<string, ConfigurationVariableValue>>((Action<KeyValuePair<string, ConfigurationVariableValue>>) (i => i.Value.SetSecuredObject(token, requiredPermissions)));
      this.PreDeployApprovals?.SetSecuredObject(token, requiredPermissions);
      this.DeployStep?.SetSecuredObject(token, requiredPermissions);
      this.PostDeployApprovals?.SetSecuredObject(token, requiredPermissions);
      IList<DeployPhase> deployPhases = this.DeployPhases;
      if (deployPhases != null)
        deployPhases.ForEach<DeployPhase>((Action<DeployPhase>) (i => i.SetSecuredObject(token, requiredPermissions)));
      this.EnvironmentOptions?.SetSecuredObject(token, requiredPermissions);
      IList<Demand> demands = this.Demands;
      if (demands != null)
        demands.ForEach<Demand>((Action<Demand>) (i => i.SetSecuredObject(token, requiredPermissions)));
      this.ExecutionPolicy?.SetSecuredObject(token, requiredPermissions);
      IList<ReleaseSchedule> schedules = this.Schedules;
      if (schedules != null)
        schedules.ForEach<ReleaseSchedule>((Action<ReleaseSchedule>) (i => i.SetSecuredObject(token, requiredPermissions)));
      this.CurrentReleaseReference?.SetSecuredObject(token, requiredPermissions);
      this.RetentionPolicy?.SetSecuredObject(token, requiredPermissions);
      this.PreDeploymentGates?.SetSecuredObject(token, requiredPermissions);
      this.PostDeploymentGates?.SetSecuredObject(token, requiredPermissions);
      IList<EnvironmentTrigger> environmentTriggers = this.EnvironmentTriggers;
      if (environmentTriggers == null)
        return;
      environmentTriggers.ForEach<EnvironmentTrigger>((Action<EnvironmentTrigger>) (i => i.SetSecuredObject(token, requiredPermissions)));
    }

    private bool AreEnvironmentTriggersEqual(
      IList<EnvironmentTrigger> envTriggers1,
      IList<EnvironmentTrigger> envTriggers2)
    {
      bool flag1 = this.isEnvironmentTriggersNullOrEmpty(envTriggers1);
      bool flag2 = this.isEnvironmentTriggersNullOrEmpty(envTriggers2);
      return flag1 & flag2 || !(flag1 ^ flag2) && envTriggers1.Count == envTriggers2.Count && !envTriggers1.Any<EnvironmentTrigger>((Func<EnvironmentTrigger, bool>) (et1 => !envTriggers2.Any<EnvironmentTrigger>((Func<EnvironmentTrigger, bool>) (et2 => et2.Equals((object) et1)))));
    }

    private bool isEnvironmentTriggersNullOrEmpty(IList<EnvironmentTrigger> triggers) => triggers == null || !triggers.Any<EnvironmentTrigger>();
  }
}
