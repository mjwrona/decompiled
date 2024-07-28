// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentBasedDeployPhase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class AgentBasedDeployPhase : DeployPhase
  {
    public AgentBasedDeployPhase()
      : base(DeployPhaseTypes.AgentBasedDeployment)
    {
      this.DeploymentInput = new AgentDeploymentInput();
    }

    [DataMember(EmitDefaultValue = false)]
    public AgentDeploymentInput DeploymentInput { get; set; }

    public override bool Equals(DeployPhase other) => other is AgentBasedDeployPhase basedDeployPhase && (this.DeploymentInput == null || basedDeployPhase.DeploymentInput != null) && (this.DeploymentInput != null || basedDeployPhase.DeploymentInput == null) && (this.DeploymentInput == null || basedDeployPhase.DeploymentInput == null || this.DeploymentInput.Equals((BaseDeploymentInput) basedDeployPhase.DeploymentInput)) && base.Equals(other);

    public override BaseDeploymentInput GetDeploymentInput() => (BaseDeploymentInput) this.DeploymentInput;

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.DeploymentInput?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
