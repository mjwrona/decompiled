// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent6
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent6 : DeploymentSqlComponent5
  {
    protected override void BindDeploymentExpands(DeploymentExpands deploymentExpands)
    {
      bool parameterValue1 = true;
      bool parameterValue2 = true;
      if (deploymentExpands != DeploymentExpands.All)
      {
        parameterValue1 = (deploymentExpands & DeploymentExpands.Approvals) == DeploymentExpands.Approvals;
        parameterValue2 = (deploymentExpands & DeploymentExpands.Artifacts) == DeploymentExpands.Artifacts;
      }
      this.BindBoolean("includeApprovals", parameterValue1);
      this.BindBoolean("includeArtifacts", parameterValue2);
    }

    protected override bool ShouldIncludeApprovals(DeploymentExpands deploymentExpands) => deploymentExpands == DeploymentExpands.All || (deploymentExpands & DeploymentExpands.Approvals) == DeploymentExpands.Approvals;

    protected override bool ShouldIncludeArtifacts(DeploymentExpands deploymentExpands) => deploymentExpands == DeploymentExpands.All || (deploymentExpands & DeploymentExpands.Artifacts) == DeploymentExpands.Artifacts;
  }
}
