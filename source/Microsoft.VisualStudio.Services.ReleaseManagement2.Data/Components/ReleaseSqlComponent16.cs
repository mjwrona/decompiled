// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent16
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
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
  public class ReleaseSqlComponent16 : ReleaseSqlComponent15
  {
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
      this.BindGuid("modifiedBy", release.ModifiedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override IEnumerable<Release> ListReleases(
      Guid projectId,
      string namePattern,
      int releaseDefinitionId,
      int definitionEnvironmentId,
      ReleaseStatus statusFilter,
      ReleaseEnvironmentStatus environmentStatusFilter,
      IEnumerable<Guid> createdByIds,
      DateTime? minCreatedTime,
      DateTime? maxCreatedTime,
      DateTime? maxModifiedTime,
      int maxReleases,
      ReleaseQueryOrder releaseQueryOrder,
      int releaseContinuationToken,
      bool includeEnvironments,
      bool includeArtifacts,
      bool includeApprovals,
      bool includeManualInterventions,
      string artifactTypeId,
      string sourceId,
      string artifactVersionId,
      string sourceBranchFilter,
      bool isDeleted,
      bool includeDeletedReleaseDefinitions,
      bool includeVariables = false,
      bool includeTags = false,
      IEnumerable<string> tagFilter = null,
      IEnumerable<int> releaseIds = null,
      string path = null)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleases", projectId);
      this.BindString(nameof (namePattern), namePattern, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (definitionEnvironmentId), definitionEnvironmentId);
      this.BindByte(nameof (statusFilter), (byte) statusFilter);
      this.BindEnvironmentStatusFilter(environmentStatusFilter);
      this.BindGuidTable("createdByIdTable", createdByIds);
      this.BindNullableDateTime(nameof (minCreatedTime), minCreatedTime);
      this.BindNullableDateTime(nameof (maxCreatedTime), maxCreatedTime);
      this.BindMaxModifiedTime(nameof (maxModifiedTime), maxModifiedTime);
      this.BindInt(nameof (maxReleases), maxReleases);
      this.BindByte("queryOrder", (byte) releaseQueryOrder);
      this.BindInt(nameof (releaseContinuationToken), releaseContinuationToken);
      this.BindBoolean(nameof (includeEnvironments), includeEnvironments);
      this.BindBoolean(nameof (includeArtifacts), includeArtifacts);
      this.BindBoolean(nameof (includeApprovals), includeApprovals);
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("SourceId", sourceId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (artifactVersionId), artifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString(nameof (sourceBranchFilter), sourceBranchFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindIsDeleted(nameof (isDeleted), isDeleted);
      this.BindFolderPath(path);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        List<ReleaseEnvironment> source1 = new List<ReleaseEnvironment>();
        List<ReleaseDeployPhase> deployPhases = new List<ReleaseDeployPhase>();
        List<ReleaseEnvironmentStep> source2 = new List<ReleaseEnvironmentStep>();
        List<PipelineArtifactSource> source3 = new List<PipelineArtifactSource>();
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        List<Release> items = resultCollection.GetCurrent<Release>().Items;
        if (includeEnvironments)
        {
          resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
          resultCollection.NextResult();
          source1 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
          resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
          resultCollection.NextResult();
          deployPhases = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
        }
        if (includeApprovals)
        {
          resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
          resultCollection.NextResult();
          source2 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        }
        if (includeArtifacts)
        {
          resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
          resultCollection.NextResult();
          source3 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        }
        foreach (Release release1 in items)
        {
          Release release = release1;
          ReleaseSqlComponent15.StitchReleaseObject(release, source1.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == release.Id)), source2.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == release.Id)), source3.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == release.Id)), (IEnumerable<ReleaseDeployPhase>) deployPhases, (IEnumerable<Deployment>) null, (IEnumerable<ManualIntervention>) null, (IEnumerable<ReleaseTagData>) null);
        }
        return (IEnumerable<Release>) items;
      }
    }

    public override IEnumerable<string> GetSourceBranches(Guid projectId, int releaseDefinitionId)
    {
      this.PrepareStoredProcedure("Release.prc_QueryArtifactSourceBranches", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new SourceBranchBinder());
        return (IEnumerable<string>) resultCollection.GetCurrent<string>().Items;
      }
    }

    public override IEnumerable<PipelineArtifactSource> GetReleaseArtifactSources(
      Guid projectId,
      IEnumerable<int> releaseIds,
      string artifactTypeId)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseArtifactSource", projectId);
      this.BindInt32Table(nameof (releaseIds), releaseIds);
      this.BindString(nameof (artifactTypeId), artifactTypeId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        return (IEnumerable<PipelineArtifactSource>) resultCollection.GetCurrent<PipelineArtifactSource>().Items;
      }
    }

    protected override void BindChangeDetails(
      ReleaseEnvironmentStatusChangeDetails changeDetails)
    {
      this.BindInt("changeDetailsId", changeDetails == null ? 701001 : (int) changeDetails.Id);
    }
  }
}
