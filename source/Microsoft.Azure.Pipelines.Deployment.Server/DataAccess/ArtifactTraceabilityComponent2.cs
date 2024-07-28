// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactTraceabilityComponent2
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class ArtifactTraceabilityComponent2 : ArtifactTraceabilityComponent
  {
    public override IList<ArtifactVersionRepoInfo> GetArtifactVersionRepoInfoForJob(
      Guid projectId,
      int pipelineRunId,
      string jobId)
    {
      if (string.IsNullOrWhiteSpace(jobId))
        return (IList<ArtifactVersionRepoInfo>) Enumerable.Empty<ArtifactVersionRepoInfo>().ToList<ArtifactVersionRepoInfo>();
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetArtifactVersionRepoInfoForJob)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactVersionRepoInfoForJob");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@pipelineRunId", pipelineRunId);
        this.BindString("@jobId", jobId, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ArtifactVersionRepoInfo>((ObjectBinder<ArtifactVersionRepoInfo>) this.GetArtifactVersionRepoInfoBinder());
          return (IList<ArtifactVersionRepoInfo>) resultCollection.GetCurrent<ArtifactVersionRepoInfo>().Items;
        }
      }
    }

    public override int GetArtifactsCountForPipeline(
      Guid projectId,
      int pipelineRunId,
      IList<string> aliases,
      IList<ArtifactCategory> artifactCategories = null)
    {
      IList<string> artifactCategories1 = this.GetArtifactTypesFromArtifactCategories(artifactCategories);
      this.PrepareStoredProcedure("Deployment.prc_GetArtifactTraceabilityCountForPipelineRunId");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@pipelineRunId", pipelineRunId);
      if (aliases == null || aliases.Count == 0)
      {
        this.BindInt("@filterByArtifactAlias", 0);
        this.BindArtifactAliasTable("@artifactAliases", (IList<string>) Enumerable.Empty<string>().ToList<string>());
      }
      else
      {
        this.BindInt("@filterByArtifactAlias", 1);
        this.BindArtifactAliasTable("@artifactAliases", aliases);
      }
      this.BindArtifactTypeTable("@artifactTypes", artifactCategories1 ?? (IList<string>) Enumerable.Empty<string>().ToList<string>());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder numArtifactsCount = new SqlColumnBinder("ArtifactsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => numArtifactsCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    public override IList<ArtifactVersion> GetArtifactTraceabilityDataForPipelineOrJob(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      string jobId,
      IList<string> aliases,
      bool includeSourceDetails = false,
      IList<ArtifactCategory> artifactCategories = null)
    {
      IList<string> artifactCategories1 = this.GetArtifactTypesFromArtifactCategories(artifactCategories);
      if (string.IsNullOrWhiteSpace(jobId))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactTraceabilityForPipelineRunId");
      }
      else
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactTraceabilityForJob");
        this.BindString("@jobId", jobId, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      }
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@pipelineRunId", pipelineRunId);
      if (aliases == null || aliases.Count == 0)
      {
        this.BindInt("@filterByArtifactAlias", 0);
        this.BindArtifactAliasTable("@artifactAliases", (IList<string>) Enumerable.Empty<string>().ToList<string>());
      }
      else
      {
        this.BindInt("@filterByArtifactAlias", 1);
        this.BindArtifactAliasTable("@artifactAliases", aliases);
      }
      this.BindInt("@includeSourceDetails", includeSourceDetails ? 1 : 0);
      this.BindArtifactTypeTable("@artifactTypes", artifactCategories1 ?? (IList<string>) Enumerable.Empty<string>().ToList<string>());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ArtifactTraceabilityDataRow>((ObjectBinder<ArtifactTraceabilityDataRow>) new ArtifactTraceabilityDataRowBinder());
        return this.GetArtifactVersionsFromTraceabilityRowData((IList<ArtifactTraceabilityDataRow>) resultCollection.GetCurrent<ArtifactTraceabilityDataRow>().Items, includeSourceDetails);
      }
    }

    public override void AddArtifactTraceabilityForRepositoryOrContainerResource(
      ArtifactTraceabilityData data)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForRepositoryOrContainerResource)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabilityForRepositoryResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(data.ProjectId));
        this.BindString("@artifactType", data.ArtifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@uniqueResourceIdentifier", data.UniqueResourceIdentifier, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        if (data != null && data.ArtifactConnectionData?.Id.HasValue)
          this.BindGuid("@connectionId", data.ArtifactConnectionData.Id);
        this.BindString("@versionId", data.ArtifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@versionName", data.ArtifactVersionName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (data.ArtifactVersionProperties.Count > 0)
          this.BindString("@artifactVersionProperties", JsonUtility.ToString((object) data.ArtifactVersionProperties), 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@subArtifactName", data.ArtifactName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        if (data.Resource != null)
        {
          this.BindInt("@addRepoData", 1);
          this.BindByte("@repoType", (byte) ArtifactTraceabilityHelper.GetRepositoryType(data.Resource.Type));
          this.BindString("@repoId", data.Resource.Id, 1024, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
          this.BindString("@repoName", data.Resource.Name, 1024, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
          this.BindString("@branch", data.Resource.Branch, 1024, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
          this.BindString("@commitId", data.Resource.Version, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
          if (data.Resource.Properties.Count > 0)
            this.BindString("@artifactVersionRepoProperties", JsonUtility.ToString((object) data.Resource?.Properties), 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        }
        else
          this.BindInt("@addRepoData", 0);
        if (data.PipelineDefinitionId != ArtifactTraceabilityConstants.IncorrectId && data.PipelineRunId != ArtifactTraceabilityConstants.IncorrectId && !string.IsNullOrWhiteSpace(data.ArtifactAlias))
        {
          this.BindInt("@addPipelineRunInfo", 1);
          this.BindInt("@pipelineDefinitionId", data.PipelineDefinitionId);
          this.BindInt("@pipelineRunId", data.PipelineRunId);
          this.BindString("@artifactAlias", data.ArtifactAlias, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          if (!string.IsNullOrWhiteSpace(data.JobId) && !string.IsNullOrWhiteSpace(data.JobName))
          {
            this.BindInt("@addJobData", 1);
            this.BindString("@jobId", data.JobId, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
            this.BindString("@jobName", data.JobName, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          }
          else
          {
            this.BindInt("@addJobData", 0);
            this.BindString("@jobId", (string) null, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
            this.BindString("@jobName", (string) null, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          }
        }
        else
        {
          this.BindInt("@addPipelineRunInfo", 0);
          this.BindInt("@pipelineDefinitionId", ArtifactTraceabilityConstants.IncorrectId);
          this.BindInt("@pipelineRunId", ArtifactTraceabilityConstants.IncorrectId);
          this.BindString("@artifactAlias", (string) null, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          this.BindInt("@addJobData", 0);
          this.BindString("@jobId", (string) null, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          this.BindString("@jobName", (string) null, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        }
        this.ExecuteNonQuery();
      }
    }

    public override void AddArtifactTraceabilityForPipelineResource(
      ArtifactTraceabilityData data,
      IList<SubArtifactDataRow> subArtifactNameList,
      IList<ArtifactVersionRepoInfo> repoInfoList)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForPipelineResource)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabilityForPipelineResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(data.ProjectId));
        this.BindString("@artifactType", data.ArtifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@uniqueResourceIdentifier", data.UniqueResourceIdentifier, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        if (data.ArtifactConnectionData != null)
        {
          Guid id = data.ArtifactConnectionData.Id;
          this.BindGuid("@connectionId", data.ArtifactConnectionData.Id);
        }
        this.BindString("@versionId", data.ArtifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@versionName", data.ArtifactVersionName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (data.ArtifactVersionProperties.Count > 0)
          this.BindString("@artifactVersionProperties", JsonUtility.ToString((object) data.ArtifactVersionProperties), 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindSubArtifactVersionTable("@subArtifactNames", subArtifactNameList);
        this.BindArtifactVersionRepoInfoTable("@repoInfo", repoInfoList);
        this.BindInt("@pipelineDefinitionId", data.PipelineDefinitionId);
        this.BindInt("@pipelineRunId", data.PipelineRunId);
        this.BindString("@artifactAlias", data.ArtifactAlias, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public override DownloadTaskTraceabilityError AddArtifactTraceabilityForDownloadTask(
      ArtifactTraceabilityData data)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForDownloadTask)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabilityForDownloadTask");
        this.BindInt("@dataspaceId", this.GetDataspaceId(data.ProjectId));
        this.BindInt("@isSelfArtifact", data.IsSelfArtifact ? 1 : 0);
        this.BindString("@subArtifactName", data.ArtifactName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@pipelineDefinitionId", data.PipelineDefinitionId);
        this.BindInt("@pipelineRunId", data.PipelineRunId);
        this.BindString("@artifactAlias", data.ArtifactAlias, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@jobId", data.JobId, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@jobName", data.JobName, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      return DownloadTaskTraceabilityError.NoError;
    }

    public override void AddArtifactTraceabilityForPublishedArtifact(
      ArtifactTraceabilityData data,
      IList<ArtifactVersionRepoInfo> repoInfoList)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForPublishedArtifact)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabilityForPublishedArtifact");
        this.BindInt("@dataspaceId", this.GetDataspaceId(data.ProjectId));
        this.BindString("@artifactType", data.ArtifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@uniqueResourceIdentifier", data.UniqueResourceIdentifier, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@versionId", data.ArtifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@versionName", data.ArtifactVersionName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (data.ArtifactVersionProperties.Count > 0)
          this.BindString("@artifactVersionProperties", JsonUtility.ToString((object) data.ArtifactVersionProperties), 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@subArtifactName", data.ArtifactName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindArtifactVersionRepoInfoTable("@repoInfo", repoInfoList ?? (IList<ArtifactVersionRepoInfo>) Enumerable.Empty<ArtifactVersionRepoInfo>().ToList<ArtifactVersionRepoInfo>());
        this.ExecuteNonQuery();
      }
    }
  }
}
