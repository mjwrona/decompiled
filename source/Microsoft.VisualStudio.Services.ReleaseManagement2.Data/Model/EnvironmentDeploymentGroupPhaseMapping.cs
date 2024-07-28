// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentDeploymentGroupPhaseMapping
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class EnvironmentDeploymentGroupPhaseMapping
  {
    public EnvironmentDeploymentGroupPhaseMapping()
    {
    }

    public EnvironmentDeploymentGroupPhaseMapping(
      int releaseDefinitionId,
      int environmentId,
      int deploymentGroupId,
      string tags)
    {
      this.ReleaseDefinitionId = releaseDefinitionId;
      this.EnvironmentId = environmentId;
      this.DeploymentGroupId = deploymentGroupId;
      this.Tags = tags;
    }

    public int ReleaseDefinitionId { get; set; }

    public int EnvironmentId { get; set; }

    public int DeploymentGroupId { get; set; }

    public string Tags { get; set; }
  }
}
