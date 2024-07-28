// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactTraceabilityComponent6
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
  public class ArtifactTraceabilityComponent6 : ArtifactTraceabilityComponent5
  {
    public override void AddArtifactTraceabilityForRepositoryOrContainerResource(
      ArtifactTraceabilityData data)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForRepositoryOrContainerResource)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabilityForRepositoryOrContainerResource");
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
        bool flag1 = false;
        bool flag2 = false;
        if (data.PipelineDefinitionId != ArtifactTraceabilityConstants.IncorrectId && data.PipelineRunId != ArtifactTraceabilityConstants.IncorrectId && !string.IsNullOrWhiteSpace(data.ArtifactAlias))
        {
          flag1 = true;
          if (!string.IsNullOrWhiteSpace(data.JobId) && !string.IsNullOrWhiteSpace(data.JobName))
            flag2 = true;
        }
        this.BindInt("@addPipelineRunInfo", flag1 ? 1 : 0);
        this.BindInt("@pipelineDefinitionId", flag1 ? data.PipelineDefinitionId : ArtifactTraceabilityConstants.IncorrectId);
        this.BindInt("@pipelineRunId", flag1 ? data.PipelineRunId : ArtifactTraceabilityConstants.IncorrectId);
        this.BindString("@artifactAlias", flag1 ? data.ArtifactAlias : (string) null, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@addJobData", flag2 ? 1 : 0);
        this.BindString("@jobId", flag2 ? data.JobId : (string) null, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@jobName", flag2 ? data.JobName : (string) null, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public override int GetArtifactsCountForPipeline(
      Guid projectId,
      int pipelineRunId,
      IList<string> aliases,
      IList<ArtifactCategory> artifactCategories = null)
    {
      IList<string> artifactCategories1 = this.GetArtifactTypesFromArtifactCategories(artifactCategories);
      bool flag = false;
      this.PrepareStoredProcedure("Deployment.prc_GetArtifactTraceabilityCountForPipelineRun");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@pipelineRunId", pipelineRunId);
      if (aliases != null && aliases.Count > 0)
        flag = true;
      this.BindInt("@filterByArtifactAlias", flag ? 1 : 0);
      this.BindArtifactAliasTable("@artifactAliases", flag ? aliases : (IList<string>) Enumerable.Empty<string>().ToList<string>());
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
      bool flag = false;
      if (string.IsNullOrWhiteSpace(jobId))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactTraceabilityForPipelineRun");
      }
      else
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactTraceabilityForJob");
        this.BindString("@jobId", jobId, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      }
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@pipelineRunId", pipelineRunId);
      if (aliases != null && aliases.Count > 0)
        flag = true;
      this.BindInt("@filterByArtifactAlias", flag ? 1 : 0);
      this.BindArtifactAliasTable("@artifactAliases", flag ? aliases : (IList<string>) Enumerable.Empty<string>().ToList<string>());
      this.BindInt("@includeSourceDetails", includeSourceDetails ? 1 : 0);
      this.BindArtifactTypeTable("@artifactTypes", artifactCategories1 ?? (IList<string>) Enumerable.Empty<string>().ToList<string>());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ArtifactTraceabilityDataRow>((ObjectBinder<ArtifactTraceabilityDataRow>) new ArtifactTraceabilityDataRowBinder());
        return this.GetArtifactVersionsFromTraceabilityRowData((IList<ArtifactTraceabilityDataRow>) resultCollection.GetCurrent<ArtifactTraceabilityDataRow>().Items, includeSourceDetails);
      }
    }

    internal override void GetArtifactNameAndRepoInfoForPipelineRunId(
      Guid projectId,
      string pipelineRunId,
      out IList<SubArtifactDataRow> subArtifactNameList,
      out IList<ArtifactVersionRepoInfo> repoInfoList)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetArtifactNameAndRepoInfoForPipelineRunId)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactNameAndRepoInfoForPipelineRun");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindString("@pipelineRunId", pipelineRunId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<SubArtifactDataRow>((ObjectBinder<SubArtifactDataRow>) new SubArtifactVersionBinder());
          resultCollection.AddBinder<ArtifactVersionRepoInfo>((ObjectBinder<ArtifactVersionRepoInfo>) new ArtifactVersionRepoInfoBinder());
          subArtifactNameList = (IList<SubArtifactDataRow>) resultCollection.GetCurrent<SubArtifactDataRow>().Items;
          resultCollection.NextResult();
          repoInfoList = (IList<ArtifactVersionRepoInfo>) resultCollection.GetCurrent<ArtifactVersionRepoInfo>().Items;
        }
      }
    }
  }
}
