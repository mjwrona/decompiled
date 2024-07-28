// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhaseSnapshot
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class DeployPhaseSnapshot
  {
    public string Name { get; set; }

    public string RefName { get; set; }

    public int Rank { get; set; }

    public DeployPhaseTypes PhaseType { get; set; }

    [Obsolete("This is cleaned up from client contract. Not removing from server model as json snapshot of older releases will have this property")]
    public ControlOptions ControlOptions { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need it for deserialization from Json")]
    public IList<WorkflowTask> Workflow { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need it for deserialization from Json")]
    public JObject DeploymentInput { get; set; }

    public DeployPhaseSnapshot() => this.Workflow = (IList<WorkflowTask>) new List<WorkflowTask>();

    public DeployPhaseSnapshot DeepClone()
    {
      DeployPhaseSnapshot deployPhaseSnapshot = new DeployPhaseSnapshot()
      {
        Name = this.Name,
        RefName = this.RefName,
        Rank = this.Rank,
        PhaseType = this.PhaseType
      };
      if (this.Workflow != null)
        deployPhaseSnapshot.Workflow = (IList<WorkflowTask>) new List<WorkflowTask>((IEnumerable<WorkflowTask>) this.Workflow);
      if (this.DeploymentInput != null)
        deployPhaseSnapshot.DeploymentInput = (JObject) this.DeploymentInput.DeepClone();
      return deployPhaseSnapshot;
    }

    public BaseDeploymentInput GetDeploymentInput()
    {
      switch (this.PhaseType)
      {
        case DeployPhaseTypes.AgentBasedDeployment:
          return (BaseDeploymentInput) this.DeploymentInput.ToObject<AgentDeploymentInput>();
        case DeployPhaseTypes.RunOnServer:
          return (BaseDeploymentInput) this.DeploymentInput.ToObject<ServerDeploymentInput>();
        case DeployPhaseTypes.MachineGroupBasedDeployment:
          return (BaseDeploymentInput) this.DeploymentInput.ToObject<MachineGroupDeploymentInput>();
        case DeployPhaseTypes.DeploymentGates:
          return (BaseDeploymentInput) this.DeploymentInput.ToObject<GatesDeploymentInput>();
        default:
          throw new NotSupportedException();
      }
    }

    public T GetDeploymentInput<T>() where T : BaseDeploymentInput => this.GetDeploymentInput() as T;
  }
}
