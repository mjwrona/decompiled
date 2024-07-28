// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent7
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent7 : DeploymentSqlComponent6
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> QueryDeploymentsFailingSince(
      Guid projectId,
      IList<DefinitionEnvironmentReference> environments,
      string artifactTypeId,
      string artifactSourceId,
      string sourceBranch,
      DeploymentExpands deploymentExpands = DeploymentExpands.All)
    {
      this.PrepareStoredProcedure("Release.prc_QueryDeploymentsFailingSince", projectId);
      this.BindDeploymentsEnvironmentsFilterTable("releaseDefinitionAndEnvironmentIds", (IEnumerable<DefinitionEnvironmentReference>) environments);
      this.BindString(nameof (artifactSourceId), artifactSourceId, 256, true, SqlDbType.NVarChar);
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, true, SqlDbType.NVarChar);
      this.BindString(nameof (sourceBranch), sourceBranch, 400, true, SqlDbType.NVarChar);
      this.BindDeploymentExpands(deploymentExpands);
      return this.GetDeployments(this.ShouldIncludeApprovals(deploymentExpands), this.ShouldIncludeArtifacts(deploymentExpands));
    }

    protected override void BindSourceBranch(string sourceBranch) => this.BindString(nameof (sourceBranch), sourceBranch, 400, true, SqlDbType.NVarChar);

    protected override void BindDeploymentsPerEnvironment(int deploymentsPerEnvironment) => this.BindNullableInt(nameof (deploymentsPerEnvironment), new int?(deploymentsPerEnvironment));

    protected override ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => (ReleaseArtifactSourceBinder) new ReleaseArtifactSourceBinder6((ReleaseManagementSqlResourceComponentBase) this);
  }
}
