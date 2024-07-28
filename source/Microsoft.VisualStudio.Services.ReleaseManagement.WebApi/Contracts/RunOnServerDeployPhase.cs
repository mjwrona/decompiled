// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.RunOnServerDeployPhase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class RunOnServerDeployPhase : DeployPhase
  {
    private ServerDeploymentInput deploymentInput;

    public RunOnServerDeployPhase()
      : base(DeployPhaseTypes.RunOnServer)
    {
    }

    [DataMember]
    public ServerDeploymentInput DeploymentInput
    {
      get
      {
        if (this.deploymentInput == null)
          this.deploymentInput = new ServerDeploymentInput();
        return this.deploymentInput;
      }
      set => this.deploymentInput = value;
    }

    public override BaseDeploymentInput GetDeploymentInput() => (BaseDeploymentInput) this.DeploymentInput;

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.DeploymentInput?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
