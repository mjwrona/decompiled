// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [KnownType(typeof (AgentBasedDeployPhase))]
  [KnownType(typeof (MachineGroupBasedDeployPhase))]
  [KnownType(typeof (RunOnServerDeployPhase))]
  [KnownType(typeof (GatesDeployPhase))]
  [JsonConverter(typeof (DeployPhaseJsonConverter))]
  [DataContract]
  public abstract class DeployPhase : ReleaseManagementSecuredObject, IEquatable<DeployPhase>
  {
    protected DeployPhase(DeployPhaseTypes phaseType)
    {
      this.PhaseType = phaseType;
      this.WorkflowTasks = (IList<WorkflowTask>) new List<WorkflowTask>();
    }

    [DataMember]
    public int Rank { get; set; }

    [DataMember]
    public DeployPhaseTypes PhaseType { get; protected set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string RefName { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<WorkflowTask> WorkflowTasks { get; set; }

    public abstract BaseDeploymentInput GetDeploymentInput();

    public virtual bool Equals(DeployPhase other)
    {
      if (other == null || this.Rank != other.Rank || this.PhaseType != other.PhaseType || !string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase) || !string.Equals(this.RefName, other.RefName, StringComparison.OrdinalIgnoreCase))
        return false;
      BaseDeploymentInput deploymentInput1 = this.GetDeploymentInput();
      BaseDeploymentInput deploymentInput2 = other.GetDeploymentInput();
      return (deploymentInput1 == null || deploymentInput2 != null) && (deploymentInput1 != null || deploymentInput2 == null) && (deploymentInput1 == null || deploymentInput2 == null || deploymentInput1.Equals(deploymentInput2)) && this.AreWorkflowTasksEqual(other, false);
    }

    public bool AreWorkflowTasksEqual(
      DeployPhase other,
      bool ignoreTaskInputsTaskNameAndControlOptions)
    {
      if (this.WorkflowTasks == null && other.WorkflowTasks != null || this.WorkflowTasks != null && other.WorkflowTasks == null)
        return false;
      if (this.WorkflowTasks == null || other.WorkflowTasks == null)
        return true;
      return this.WorkflowTasks.Count == other.WorkflowTasks.Count && !this.WorkflowTasks.Where<WorkflowTask>((Func<WorkflowTask, int, bool>) ((t, i) => !t.Equals((object) other.WorkflowTasks[i], ignoreTaskInputsTaskNameAndControlOptions))).Any<WorkflowTask>();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<WorkflowTask> workflowTasks = this.WorkflowTasks;
      if (workflowTasks == null)
        return;
      workflowTasks.ForEach<WorkflowTask>((Action<WorkflowTask>) (i => i?.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
