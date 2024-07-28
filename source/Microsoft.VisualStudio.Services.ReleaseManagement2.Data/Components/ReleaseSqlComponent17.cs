// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent17
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
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
  public class ReleaseSqlComponent17 : ReleaseSqlComponent16
  {
    public override Deployment CreateAndQueueDeployment(
      Guid projectId,
      int releaseDefinitionId,
      int releaseId,
      int definitionEnvironmentId,
      int releaseEnvironmentId,
      Guid requestedBy,
      Guid requestedFor,
      DeploymentReason reason,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_QueueDeployment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindGuid(nameof (requestedBy), requestedBy);
      this.BindByte(nameof (reason), (byte) reason);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
        return resultCollection.GetCurrent<Deployment>().Items.SingleOrDefault<Deployment>();
      }
    }

    public override Release RejectReleaseEnvironment(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      Guid changedBy,
      string comment)
    {
      this.PrepareStoredProcedure("Release.prc_RejectReleaseEnvironment", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (attempt), attempt);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      return this.GetReleaseObject(projectId);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override Release UpdateEnvironmentAndDeploymentStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      Guid changedBy,
      ReleaseEnvironmentStatus environmentStatus,
      DeploymentStatus deploymentStatus,
      ReleaseEnvironmentStatusChangeDetails changeDetails,
      DeploymentOperationStatus operationStatus = DeploymentOperationStatus.Undefined)
    {
      this.PrepareStoredProcedure("Release.prc_OnDeploymentCompleted", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (attempt), attempt);
      this.BindByte(nameof (environmentStatus), (byte) environmentStatus);
      this.BindByte(nameof (deploymentStatus), (byte) deploymentStatus);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindChangeDetails(changeDetails);
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
        List<Deployment> deploymentList = new List<Deployment>();
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
          resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
          resultCollection.NextResult();
          deploymentList = resultCollection.GetCurrent<Deployment>().Items;
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
          ReleaseSqlComponent15.StitchReleaseObject(release, source1.Where<ReleaseEnvironment>((System.Func<ReleaseEnvironment, bool>) (env => env.ReleaseId == release.Id)), source2.Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (step => step.ReleaseId == release.Id)), source3.Where<PipelineArtifactSource>((System.Func<PipelineArtifactSource, bool>) (source => source.ReleaseId == release.Id)), (IEnumerable<ReleaseDeployPhase>) deployPhases, (IEnumerable<Deployment>) deploymentList, (IEnumerable<ManualIntervention>) null, (IEnumerable<ReleaseTagData>) null);
        }
        return (IEnumerable<Release>) items;
      }
    }

    public override Deployment UpdateDeploymentOperationStatus(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int attempt,
      DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateDeploymentOperationStatus", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (attempt), attempt);
      this.BindInt(nameof (operationStatus), (int) operationStatus);
      this.BindGuid(nameof (changedBy), changedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
        return resultCollection.GetCurrent<Deployment>().Items.SingleOrDefault<Deployment>();
      }
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
      this.BindGuid(nameof (changedBy), changedBy);
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
      this.BindInt(nameof (attempt), attempt);
      this.BindGuid("changedby", changedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        return resultCollection.GetCurrent<ReleaseDeployPhase>().Items.FirstOrDefault<ReleaseDeployPhase>();
      }
    }

    public override ReleaseDeployPhase UpdateReleaseDeployPhase(
      Guid projectId,
      ReleaseDeployPhase releaseDeployPhase,
      DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      if (releaseDeployPhase == null)
        throw new ArgumentNullException(nameof (releaseDeployPhase));
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseDeployPhase", projectId);
      this.BindInt("releaseId", releaseDeployPhase.ReleaseId);
      this.BindInt("releaseEnvironmentId", releaseDeployPhase.ReleaseEnvironmentId);
      this.BindInt("releaseDeployPhaseId", releaseDeployPhase.Id);
      this.BindByte("status", (byte) releaseDeployPhase.Status);
      this.BindNullableGuid("runPlanId", releaseDeployPhase.RunPlanId);
      this.BindString("logs", releaseDeployPhase.Logs, 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("deploymentAttempt", releaseDeployPhase.Attempt);
      this.BindInt(nameof (operationStatus), (int) operationStatus);
      this.BindGuid("changedby", changedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        return resultCollection.GetCurrent<ReleaseDeployPhase>().Items.FirstOrDefault<ReleaseDeployPhase>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Too heavy to be a property")]
    protected override Release GetReleaseObject(Guid projectId)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Release>((ObjectBinder<Release>) this.GetReleaseBinder(projectId));
        resultCollection.AddBinder<ReleaseEnvironment>((ObjectBinder<ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        Release release = resultCollection.GetCurrent<Release>().FirstOrDefault<Release>();
        resultCollection.NextResult();
        List<ReleaseEnvironment> items1 = resultCollection.GetCurrent<ReleaseEnvironment>().Items;
        resultCollection.NextResult();
        List<ReleaseDeployPhase> items2 = resultCollection.GetCurrent<ReleaseDeployPhase>().Items;
        resultCollection.NextResult();
        List<Deployment> items3 = resultCollection.GetCurrent<Deployment>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items4 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<PipelineArtifactSource> items5 = resultCollection.GetCurrent<PipelineArtifactSource>().Items;
        if (release != null)
          ReleaseSqlComponent15.StitchReleaseObject(release, (IEnumerable<ReleaseEnvironment>) items1, (IEnumerable<ReleaseEnvironmentStep>) items4, (IEnumerable<PipelineArtifactSource>) items5, (IEnumerable<ReleaseDeployPhase>) items2, (IEnumerable<Deployment>) items3, (IEnumerable<ManualIntervention>) null, (IEnumerable<ReleaseTagData>) null);
        return release;
      }
    }
  }
}
