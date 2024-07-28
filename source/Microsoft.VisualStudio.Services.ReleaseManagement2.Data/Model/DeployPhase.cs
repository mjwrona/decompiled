// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DeployPhase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class DeployPhase : ForeignKeyModelBase
  {
    public int ReleaseDefinitionId { get; set; }

    public int DefinitionEnvironmentId { get; set; }

    public int Rank { get; set; }

    public DeployPhaseTypes PhaseType { get; set; }

    public string Name { get; set; }

    public string RefName { get; set; }

    public string Workflow { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "needed for deserialization as it is not data contract")]
    public JObject DeploymentInput { get; set; }

    public DeployPhase DeepClone() => new DeployPhase()
    {
      ReleaseDefinitionId = this.ReleaseDefinitionId,
      DefinitionEnvironmentId = this.DefinitionEnvironmentId,
      Rank = this.Rank,
      PhaseType = this.PhaseType,
      Name = this.Name,
      RefName = this.RefName,
      Workflow = this.Workflow,
      DeploymentInput = this.DeploymentInput
    };

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
  }
}
