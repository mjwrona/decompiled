// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent5
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent5 : DeploymentSqlComponent4
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> ListDeployments(
      Guid projectId,
      int releaseDefinitionId,
      int releaseDefinitionEnvironmentId,
      int releaseId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      IList<Guid> createdByIds,
      bool latestAttemptsOnly,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder queryOrder,
      int continuationToken,
      bool isDeleted,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime,
      IList<Guid> createdForIds,
      int maxDeployments,
      string branchName,
      DateTime? minStartedTime,
      DateTime? maxStartedTime,
      string artifactTypeId = "",
      string sourceId = "",
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason releaseReason = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.None)
    {
      this.PrepareStoredProcedure("Release.prc_QueryDeployments", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (releaseDefinitionEnvironmentId), releaseDefinitionEnvironmentId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindNullableInt(nameof (deploymentStatus), new int?((int) deploymentStatus));
      this.BindNullableInt(nameof (operationStatus), new int?((int) operationStatus));
      this.BindGuidTable("createdByIdsTable", (IEnumerable<Guid>) createdByIds);
      this.BindNullableBoolean(nameof (latestAttemptsOnly), new bool?(latestAttemptsOnly));
      this.BindByte(nameof (queryOrder), (byte) queryOrder);
      this.BindNullableInt("deploymentContinuationToken", new int?(continuationToken));
      this.BindMaxDeployments(maxDeployments);
      this.BindNullableBoolean(nameof (isDeleted), new bool?(isDeleted));
      this.BindModifiedTime(minModifiedTime, maxModifiedTime);
      this.BindCreatedForIds(createdForIds);
      return this.GetDeployments();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetDeploymentsForMultipleEnvironments(
      Guid projectId,
      IList<DefinitionEnvironmentReference> environments,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      string artifactSourceId,
      string artifactTypeId,
      string sourceBranch,
      IList<string> artifactVersions,
      int deploymentsPerEnvironment,
      bool isDeleted,
      DeploymentExpands deploymentExpands = DeploymentExpands.All)
    {
      this.PrepareStoredProcedure("Release.prc_QueryDeploymentsForMultipleEnvironments", projectId);
      this.BindDeploymentsEnvironmentsFilterTable("releaseDefinitionAndEnvironmentIds", (IEnumerable<DefinitionEnvironmentReference>) environments);
      this.BindNullableInt(nameof (deploymentStatus), new int?((int) deploymentStatus));
      this.BindNullableInt(nameof (operationStatus), new int?((int) operationStatus));
      this.BindString(nameof (artifactSourceId), artifactSourceId, 256, true, SqlDbType.NVarChar);
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, true, SqlDbType.NVarChar);
      this.BindSourceBranch(sourceBranch);
      this.BindStringTable("artifactVersionsFilter", (IEnumerable<string>) artifactVersions, true, 256);
      this.BindDeploymentsPerEnvironment(deploymentsPerEnvironment);
      this.BindNullableBoolean(nameof (isDeleted), new bool?(isDeleted));
      this.BindDeploymentExpands(deploymentExpands);
      return this.GetDeployments(this.ShouldIncludeApprovals(deploymentExpands), this.ShouldIncludeArtifacts(deploymentExpands));
    }

    protected virtual bool ShouldIncludeApprovals(DeploymentExpands deploymentExpands) => true;

    protected virtual bool ShouldIncludeArtifacts(DeploymentExpands deploymentExpands) => true;

    protected virtual void BindDeploymentExpands(DeploymentExpands deploymentExpands)
    {
    }

    protected virtual void BindSourceBranch(string sourceBranch)
    {
    }

    protected virtual void BindDeploymentsPerEnvironment(int deploymentsPerEnvironment)
    {
    }
  }
}
