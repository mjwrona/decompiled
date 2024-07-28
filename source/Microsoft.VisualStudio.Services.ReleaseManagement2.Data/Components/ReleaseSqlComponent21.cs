// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent21
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent21 : ReleaseSqlComponent20
  {
    public override ReleaseLogContainers GetReleaseLogContainers(
      Guid projectId,
      int releaseId,
      bool skipIsDeletedCheck)
    {
      List<ReleaseDeployPhaseRef> source = new List<ReleaseDeployPhaseRef>();
      this.PrepareStoredProcedure("Release.prc_GetReleaseRunPlanIdRefs", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindBoolean(nameof (skipIsDeletedCheck), skipIsDeletedCheck);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhaseRef>((ObjectBinder<ReleaseDeployPhaseRef>) this.GetReleaseRunPlanIdRefBinder());
        source = resultCollection.GetCurrent<ReleaseDeployPhaseRef>().Items;
      }
      return new ReleaseLogContainers()
      {
        DeployPhases = (IList<ReleaseDeployPhaseRef>) source.Where<ReleaseDeployPhaseRef>((System.Func<ReleaseDeployPhaseRef, bool>) (e => e.PlanId != Guid.Empty)).ToList<ReleaseDeployPhaseRef>()
      };
    }

    public override Release AddRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.EnsureReleaseNameIsNotEmpty(release);
      this.PrepareStoredProcedure("Release.prc_Release_Add", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindString("releaseName", release.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("description", release.Description, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindByte("releaseStatus", (byte) release.Status);
      this.BindInt("releaseDefinitionId", release.ReleaseDefinitionId);
      this.BindString("definitionJson", ReleaseDefinitionSnapshotUtility.ToJson(release.DefinitionSnapshot), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("variables", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.ReplaceSecretVariablesWithNull(release.Variables)), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("variableGroups", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) VariableGroupUtility.ClearSecrets(release.VariableGroups)), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("createdBy", release.CreatedBy);
      this.BindGuid("modifiedBy", release.ModifiedBy);
      this.BindByte("reason", (byte) release.Reason);
      this.BindString("releaseNameFormat", release.ReleaseNameFormat, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    public override Release UpdateDraftRelease(Guid projectId, Release release, string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      this.PrepareStoredProcedure("Release.prc_Release_UpdateDraft", projectId);
      IEnumerable<PipelineArtifactSource> releaseArtifactSources = this.GetReleaseArtifactSources((IEnumerable<ArtifactSource>) release.LinkedArtifacts);
      this.BindInt("releaseId", release.Id);
      this.BindString("releaseName", release.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("description", release.Description, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("definitionJson", ReleaseDefinitionSnapshotUtility.ToJson(release.DefinitionSnapshot), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("variables", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.VariablesUtility.ReplaceSecretVariablesWithNull(release.Variables)), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("variableGroups", Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) VariableGroupUtility.ClearSecrets(release.VariableGroups)), -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("modifiedBy", release.ModifiedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    public override ReleaseDeployPhase AddReleaseDeployPhase(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int rank,
      int attempt,
      DeployPhaseTypes phaseType,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_AddReleaseDeployPhase", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt("deployPhaseRank", rank);
      this.BindByte("deployPhaseType", (byte) phaseType);
      this.BindInt(nameof (attempt), attempt);
      this.BindGuid("changedby", changedBy);
      this.BindBoolean("shareOutputVariables", false);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        return resultCollection.GetCurrent<ReleaseDeployPhase>().Items.FirstOrDefault<ReleaseDeployPhase>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected override ReleaseBinder GetReleaseBinder(Guid projectId) => (ReleaseBinder) new ReleaseBinder2(this.RequestContext, (ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual ReleaseRunPlanIdRefBinder GetReleaseRunPlanIdRefBinder() => (ReleaseRunPlanIdRefBinder) new ReleaseRunPlanIdRefBinder2();
  }
}
