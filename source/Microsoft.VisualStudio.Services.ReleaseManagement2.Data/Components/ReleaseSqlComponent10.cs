// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent10
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
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
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "This is our versioning mechanism")]
  public class ReleaseSqlComponent10 : ReleaseSqlComponent9
  {
    protected virtual void BindReleaseEnvironments(
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      this.BindReleaseEnvironmentTable6(nameof (releaseEnvironments), releaseEnvironments);
    }

    public override Release UpdateReleaseEnvironmentStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      ReleaseEnvironmentStatus statusFrom,
      ReleaseEnvironmentStatus statusTo,
      Guid changedBy,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      string comment,
      int attempt)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseEnvironmentStatus", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindByte(nameof (statusFrom), (byte) statusFrom);
      this.BindByte(nameof (statusTo), (byte) statusTo);
      return this.GetReleaseObject(projectId);
    }

    public override Release UpdateReleaseEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      DateTime? environmentScheduledDateTime,
      Guid? stageSchedulingJobId,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseEnvironment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindNullableDateTime("scheduledDeploymentTime", environmentScheduledDateTime);
      return this.GetReleaseObject(projectId);
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
      this.BindGuid("createdBy", release.CreatedBy);
      this.BindGuid("modifiedBy", release.ModifiedBy);
      this.BindByte("reason", (byte) release.Reason);
      this.BindString("releaseNameFormat", release.ReleaseNameFormat, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override Release PatchRelease(
      Guid projectId,
      int releaseId,
      Guid modifiedBy,
      IList<ReleaseEnvironment> releaseEnvironments,
      string comment,
      ReleaseStatus? status = null,
      bool? keepForever = null)
    {
      this.PrepareStoredProcedure("Release.prc_PatchRelease", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindGuid(nameof (modifiedBy), modifiedBy);
      if (status.HasValue)
        this.BindByte("releaseStatus", (byte) status.Value);
      if (keepForever.HasValue)
        this.BindBoolean(nameof (keepForever), keepForever.Value);
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
      this.BindReleaseEnvironments((IEnumerable<ReleaseEnvironment>) release.Environments);
      this.BindReleaseArtifactSourceTable(releaseArtifactSources);
      return this.GetReleaseObject(projectId);
    }

    public override Release UpdateEnvironmentAndStepsStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      ReleaseEnvironmentStatus environmentStatus,
      ReleaseEnvironmentStepStatus statusFrom,
      ReleaseEnvironmentStepStatus statusTo)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateEnvironmentAndStepsStatus", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindByte(nameof (environmentStatus), (byte) environmentStatus);
      this.BindByte(nameof (statusFrom), (byte) statusFrom);
      this.BindByte(nameof (statusTo), (byte) statusTo);
      return this.GetReleaseObject(projectId);
    }

    protected override void BindReleaseArtifactSourceTable(
      IEnumerable<PipelineArtifactSource> releaseArtifactSources)
    {
      this.BindReleaseArtifactSourceTable5(nameof (releaseArtifactSources), releaseArtifactSources);
    }

    protected virtual void BindEnvironmentStatusFilter(
      ReleaseEnvironmentStatus environmentStatusFilter)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => (ReleaseArtifactSourceBinder) new ReleaseArtifactSourceBinder4((ReleaseManagementSqlResourceComponentBase) this);

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
      sourceId = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetSourceIdInLegacyFormat(sourceId);
      int result;
      if (!int.TryParse(sourceId, out result))
        result = 0;
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
      this.BindInt("artifactSourceId", result);
      this.BindString(nameof (artifactVersionId), artifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindIsDeleted(nameof (isDeleted), isDeleted);
      this.BindFolderPath(path);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        IList<ReleaseEnvironment> source1 = (IList<ReleaseEnvironment>) new List<ReleaseEnvironment>();
        IList<ReleaseEnvironmentStep> source2 = (IList<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
        IList<PipelineArtifactSource> source3 = (IList<PipelineArtifactSource>) new List<PipelineArtifactSource>();
        IList<ReleaseArtifactDefinition> releaseArtifactDefinitions = (IList<ReleaseArtifactDefinition>) new List<ReleaseArtifactDefinition>();
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        if (includeEnvironments)
          resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        if (includeApprovals)
          resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        if (includeArtifacts)
        {
          resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
          resultCollection.AddBinder<ReleaseArtifactDefinition>((ObjectBinder<ReleaseArtifactDefinition>) this.GetReleaseArtifactDefinitionBinder());
        }
        List<Release> items = resultCollection.GetCurrent<Release>().Items;
        if (includeEnvironments)
        {
          resultCollection.NextResult();
          source1 = (IList<ReleaseEnvironment>) resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        }
        if (includeApprovals)
        {
          resultCollection.NextResult();
          source2 = (IList<ReleaseEnvironmentStep>) resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        }
        if (includeArtifacts)
        {
          resultCollection.NextResult();
          source3 = (IList<PipelineArtifactSource>) resultCollection.GetCurrent<PipelineArtifactSource>().Items;
          resultCollection.NextResult();
          releaseArtifactDefinitions = (IList<ReleaseArtifactDefinition>) resultCollection.GetCurrent<ReleaseArtifactDefinition>().Items;
        }
        foreach (Release release in items)
        {
          int releaseId = release.Id;
          ReleaseSqlComponent.StitchReleaseObject(release, source1.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == releaseId)), source2.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == releaseId)), source3.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == releaseId)), (IEnumerable<ReleaseArtifactDefinition>) releaseArtifactDefinitions, (IEnumerable<ReleaseDeployPhase>) null);
        }
        return (IEnumerable<Release>) items;
      }
    }

    protected virtual void BindMaxModifiedTime(string parameterName, DateTime? maxModifiedTime)
    {
    }

    protected virtual void BindIsDeleted(string parameterName, bool isDeleted)
    {
    }

    protected virtual void BindFolderPath(string path)
    {
    }
  }
}
