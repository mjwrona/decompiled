// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactTraceabilityComponent
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class ArtifactTraceabilityComponent : DeploymentSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[8]
    {
      (IComponentCreator) new ComponentCreator<ArtifactTraceabilityComponent>(1),
      (IComponentCreator) new ComponentCreator<ArtifactTraceabilityComponent2>(2),
      (IComponentCreator) new ComponentCreator<ArtifactTraceabilityComponent3>(3),
      (IComponentCreator) new ComponentCreator<ArtifactTraceabilityComponent4>(4),
      (IComponentCreator) new ComponentCreator<ArtifactTraceabilityComponent5>(5),
      (IComponentCreator) new ComponentCreator<ArtifactTraceabilityComponent6>(6),
      (IComponentCreator) new ComponentCreator<ArtifactTraceabilityComponent7>(7),
      (IComponentCreator) new ComponentCreator<ArtifactTraceabilityComponent8>(8)
    }, "DeploymentArtifactTraceability", "Deployment");
    private static readonly SqlMetaData[] typ_ArtifactVersionRepoInfoTableV1 = new SqlMetaData[7]
    {
      new SqlMetaData("SubArtifactVersionId", SqlDbType.Int),
      new SqlMetaData("RepoType", SqlDbType.TinyInt),
      new SqlMetaData("RepoId", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("RepoName", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Branch", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("CommitId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Properties", SqlDbType.NVarChar, 2048L)
    };
    private static readonly SqlMetaData[] typ_SubArtifactNameTableV1 = new SqlMetaData[2]
    {
      new SqlMetaData("SubArtifactVersionId", SqlDbType.Int),
      new SqlMetaData("SubArtifactName", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_ArtifactTypeTableV1 = new SqlMetaData[1]
    {
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_ArtifactAliasTableV1 = new SqlMetaData[1]
    {
      new SqlMetaData("ArtifactAlias", SqlDbType.NVarChar, 256L)
    };

    public ArtifactTraceabilityComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual IList<ArtifactVersionRepoInfo> GetArtifactVersionRepoInfoForJob(
      Guid projectId,
      int pipelineRunId,
      string jobId)
    {
      Guid result;
      if (!Guid.TryParse(jobId, out result) || !(result != Guid.Empty))
        throw new InvalidCastException(DeploymentResources.InvalidJobIdColumnType((object) jobId));
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetArtifactVersionRepoInfoForJob)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactVersionRepoInfoForJob");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindGuid("@jobId", result);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ArtifactVersionRepoInfo>((ObjectBinder<ArtifactVersionRepoInfo>) this.GetArtifactVersionRepoInfoBinder());
          return (IList<ArtifactVersionRepoInfo>) resultCollection.GetCurrent<ArtifactVersionRepoInfo>().Items;
        }
      }
    }

    public virtual int GetArtifactsCountForPipeline(
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
      this.BindInt("@filterByArtifactType", 1);
      this.BindArtifactTypeTable("@artifactTypes", artifactCategories1 ?? (IList<string>) Enumerable.Empty<string>().ToList<string>());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder numArtifactsCount = new SqlColumnBinder("ArtifactsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => numArtifactsCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    public virtual IList<ArtifactVersion> GetArtifactTraceabilityDataForPipelineOrJob(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      string jobId,
      IList<string> aliases,
      bool includeSourceDetails = false,
      IList<ArtifactCategory> artifactCategories = null)
    {
      Guid result;
      if (!Guid.TryParse(jobId, out result))
        throw new InvalidCastException(DeploymentResources.InvalidJobIdColumnType((object) jobId));
      IList<string> artifactCategories1 = this.GetArtifactTypesFromArtifactCategories(artifactCategories);
      if (result == Guid.Empty)
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactTraceabilityForPipelineRunId");
      }
      else
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactTraceabilityForJob");
        this.BindGuid("@jobId", result);
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
      this.BindInt("@filterByArtifactType", 1);
      this.BindArtifactTypeTable("@artifactTypes", artifactCategories1 ?? (IList<string>) Enumerable.Empty<string>().ToList<string>());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ArtifactTraceabilityDataRow>((ObjectBinder<ArtifactTraceabilityDataRow>) new ArtifactTraceabilityDataRowBinder());
        return this.GetArtifactVersionsFromTraceabilityRowData((IList<ArtifactTraceabilityDataRow>) resultCollection.GetCurrent<ArtifactTraceabilityDataRow>().Items, includeSourceDetails);
      }
    }

    internal virtual void GetArtifactNameAndRepoInfoForPipelineRunId(
      Guid projectId,
      string pipelineRunId,
      out IList<SubArtifactDataRow> subArtifactNameList,
      out IList<ArtifactVersionRepoInfo> repoInfoList)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetArtifactNameAndRepoInfoForPipelineRunId)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetArtifactNameAndRepoInfoForPipelineRunId");
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

    public virtual void AddArtifactTraceabilityForRepositoryOrContainerResource(
      ArtifactTraceabilityData data)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForRepositoryOrContainerResource)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabiltiyForRepositoryResource");
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
        this.BindString("@subArtifactName", data.ArtifactName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        if (data.Resource != null)
        {
          this.BindByte("@repoType", (byte) ArtifactTraceabilityHelper.GetRepositoryType(data.Resource.Type));
          this.BindString("@repoId", data.Resource.Id, 1024, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
          this.BindString("@repoName", data.Resource.Name, 1024, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
          this.BindString("@branch", data.Resource.Branch, 1024, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
          this.BindString("@commitId", data.Resource.Version, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        }
        ArtifactTraceabilityResourceInfo resource = data.Resource;
        if ((resource != null ? (resource.Properties.Count > 0 ? 1 : 0) : 0) != 0)
          this.BindString("@artifactVersionRepoProperties", JsonUtility.ToString((object) data.Resource?.Properties), 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (data.PipelineDefinitionId != ArtifactTraceabilityConstants.IncorrectId && data.PipelineRunId != ArtifactTraceabilityConstants.IncorrectId && !string.IsNullOrWhiteSpace(data.ArtifactAlias))
        {
          this.BindInt("@addPipelineRunInfo", 1);
          this.BindInt("@pipelineDefinitionId", data.PipelineDefinitionId);
          this.BindInt("@pipelineRunId", data.PipelineRunId);
          this.BindString("@artifactAlias", data.ArtifactAlias, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          if (!string.IsNullOrWhiteSpace(data.JobId) && !string.IsNullOrWhiteSpace(data.JobName))
          {
            Guid result;
            if (!Guid.TryParse(data.JobId, out result) || !(result != Guid.Empty))
              throw new InvalidCastException(DeploymentResources.InvalidJobIdColumnType((object) data.JobId));
            this.BindInt("@addJobData", 1);
            this.BindGuid("@jobId", result);
            this.BindString("@jobName", data.JobName, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          }
          else
          {
            this.BindInt("@addJobData", 0);
            this.BindGuid("@jobId", Guid.Empty);
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
          this.BindGuid("@jobId", Guid.Empty);
          this.BindString("@jobName", (string) null, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        }
        this.ExecuteNonQuery();
      }
    }

    public virtual void AddArtifactTraceabilityForPipelineResource(
      ArtifactTraceabilityData data,
      IList<SubArtifactDataRow> subArtifactNameList,
      IList<ArtifactVersionRepoInfo> repoInfoList)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForPipelineResource)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabiltiyForPipelineResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(data.ProjectId));
        this.BindString("@artifactType", data.ArtifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@uniqueResourceIdentifier", data.UniqueResourceIdentifier, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        if (data != null && data.ArtifactConnectionData?.Id.HasValue)
          this.BindGuid("@connectionId", data.ArtifactConnectionData.Id);
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

    public virtual DownloadTaskTraceabilityError AddArtifactTraceabilityForDownloadTask(
      ArtifactTraceabilityData data)
    {
      Guid result;
      if (!Guid.TryParse(data.JobId, out result) || !(result != Guid.Empty))
        throw new InvalidCastException(DeploymentResources.InvalidJobIdColumnType((object) result));
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForDownloadTask)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabilityForDownloadTask");
        this.BindInt("@dataspaceId", this.GetDataspaceId(data.ProjectId));
        this.BindString("@subArtifactName", data.ArtifactName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@pipelineDefinitionId", data.PipelineDefinitionId);
        this.BindInt("@pipelineRunId", data.PipelineRunId);
        this.BindString("@artifactAlias", data.ArtifactAlias, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@jobId", result);
        this.BindString("@jobName", data.JobName, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
        return DownloadTaskTraceabilityError.NoError;
      }
    }

    public virtual void AddArtifactTraceabilityForPublishedArtifact(
      ArtifactTraceabilityData data,
      IList<ArtifactVersionRepoInfo> repoInfoList)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForPublishedArtifact)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabiltiyForPublishedArtifact");
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

    public virtual IList<CDPipelineRunData> GetCDPipelinesRunsDataForCIPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      int paginationLimit,
      int continuationToken)
    {
      return (IList<CDPipelineRunData>) Enumerable.Empty<CDPipelineRunData>().ToList<CDPipelineRunData>();
    }

    public virtual IList<CDPipelineRunData> GetPipelineRunsUsingExistingPipelineRun(
      Guid projectId,
      int pipelineDefinitionId,
      string existingPipelineURI,
      int existingPipelineRunId)
    {
      return (IList<CDPipelineRunData>) Enumerable.Empty<CDPipelineRunData>().ToList<CDPipelineRunData>();
    }

    internal virtual ArtifactVersionRepoInfoBinder GetArtifactVersionRepoInfoBinder() => new ArtifactVersionRepoInfoBinder();

    internal virtual DownloadTaskTraceabilityErrorBinder GetDownloadTaskTraceabilityErrorBinder() => new DownloadTaskTraceabilityErrorBinder();

    protected IList<string> GetArtifactTypesFromArtifactCategories(
      IList<ArtifactCategory> artifactCategories)
    {
      HashSet<string> source = new HashSet<string>();
      if (artifactCategories != null)
      {
        foreach (ArtifactCategory artifactCategory1 in (IEnumerable<ArtifactCategory>) artifactCategories)
        {
          IList<string> artifactCategory2 = this.GetArtifactTypesFromArtifactCategory(artifactCategory1);
          if (artifactCategory2 != null)
            source.UnionWith((IEnumerable<string>) artifactCategory2);
        }
      }
      if (source.Count == 0)
      {
        IList<string> artifactCategory = this.GetArtifactTypesFromArtifactCategory(ArtifactCategory.All);
        if (artifactCategory != null)
          source.UnionWith((IEnumerable<string>) artifactCategory);
      }
      return (IList<string>) source.ToList<string>();
    }

    private IList<string> GetArtifactTypesFromArtifactCategory(ArtifactCategory artifactCategory)
    {
      IList<string> artifactCategory1 = (IList<string>) null;
      if (artifactCategory != ArtifactCategory.All && artifactCategory != ArtifactCategory.None)
        ArtifactTraceabilityHelper.ArtifactCategoryToArtifactTypeMap.TryGetValue(Enum.GetName(typeof (ArtifactCategory), (object) artifactCategory), out artifactCategory1);
      else if (artifactCategory == ArtifactCategory.All)
        return (IList<string>) ArtifactTraceabilityHelper.ArtifactTypeToArtifactCategoryMap.Keys.ToList<string>();
      return artifactCategory1;
    }

    internal IList<ArtifactVersion> GetArtifactVersionsFromTraceabilityRowData(
      IList<ArtifactTraceabilityDataRow> traceabilityDataRows,
      bool includeSourceDetails)
    {
      HashSet<ArtifactVersion> source = new HashSet<ArtifactVersion>();
      if (traceabilityDataRows != null)
      {
        foreach (ArtifactTraceabilityDataRow traceabilityDataRow in (IEnumerable<ArtifactTraceabilityDataRow>) traceabilityDataRows)
        {
          bool flag = false;
          ArtifactVersion equalValue = new ArtifactVersion(traceabilityDataRow.ArtifactType, traceabilityDataRow.ArtifactAlias, traceabilityDataRow.ArtifactVersionId);
          ArtifactVersion actualValue;
          if (source.TryGetValue(equalValue, out actualValue))
          {
            flag = true;
            equalValue = actualValue;
          }
          SubArtifactVersion subArtifactVersion = new SubArtifactVersion(traceabilityDataRow.ArtifactName);
          if (equalValue.SubArtifactVersions.Contains(subArtifactVersion))
            subArtifactVersion = equalValue.SubArtifactVersions[equalValue.SubArtifactVersions.IndexOf(subArtifactVersion)];
          else
            equalValue.SubArtifactVersions.Add(subArtifactVersion);
          if (includeSourceDetails)
          {
            Guid result1 = Guid.Empty;
            Guid result2 = Guid.Empty;
            if (traceabilityDataRow.RepositoryProperties != null)
            {
              if (traceabilityDataRow.RepositoryProperties.ContainsKey(ArtifactTraceabilityPropertyKeys.ConnectionId))
                Guid.TryParse(traceabilityDataRow.RepositoryProperties[ArtifactTraceabilityPropertyKeys.ConnectionId], out result1);
              if (traceabilityDataRow.RepositoryProperties.ContainsKey(ArtifactTraceabilityPropertyKeys.ProjectId))
                Guid.TryParse(traceabilityDataRow.RepositoryProperties[ArtifactTraceabilityPropertyKeys.ProjectId], out result2);
            }
            ArtifactSourceVersion artifactSourceVersion = new ArtifactSourceVersion()
            {
              RepositoryType = Enum.GetName(typeof (RepositoryType), (object) traceabilityDataRow.RepositoryType),
              RepositoryId = traceabilityDataRow.RepositoryId,
              RepositoryName = traceabilityDataRow.RepositoryName,
              SourceBranch = traceabilityDataRow.Branch,
              Version = traceabilityDataRow.CommitId,
              Properties = traceabilityDataRow.RepositoryProperties,
              ServiceConnectionId = result1
            };
            subArtifactVersion.ArtifactSourceVersions.Add(artifactSourceVersion);
          }
          if (!flag)
          {
            equalValue.ArtifactCategory = !ArtifactTraceabilityHelper.ArtifactTypeToArtifactCategoryMap.ContainsKey(traceabilityDataRow.ArtifactType) ? ArtifactCategory.Build : ArtifactTraceabilityHelper.GetArtifactCategory(ArtifactTraceabilityHelper.ArtifactTypeToArtifactCategoryMap[traceabilityDataRow.ArtifactType]);
            equalValue.UniqueResourceIdentifier = traceabilityDataRow.UniqueResourceIdentifier;
            equalValue.ConnectionId = traceabilityDataRow.EndpointId ?? Guid.Empty;
            equalValue.ArtifactVersionName = traceabilityDataRow.ArtifactVersionName;
            equalValue.ArtifactVersionProperties = traceabilityDataRow.ArtifactVersionProperties;
            source.Add(equalValue);
          }
        }
      }
      return (IList<ArtifactVersion>) source.ToList<ArtifactVersion>();
    }

    protected SqlParameter BindArtifactVersionRepoInfoTable(
      string parameterName,
      IList<ArtifactVersionRepoInfo> rows)
    {
      return this.BindTable(parameterName, "Deployment.typ_ArtifactVersionRepoInfoTableV1", rows.Select<ArtifactVersionRepoInfo, SqlDataRecord>((System.Func<ArtifactVersionRepoInfo, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ArtifactTraceabilityComponent.typ_ArtifactVersionRepoInfoTableV1);
        int ordinal = 0;
        sqlDataRecord.SetInt32(ordinal, row.SubArtifactVersionId);
        int num1;
        sqlDataRecord.SetByte(num1 = ordinal + 1, row.RepoType);
        int num2;
        if (!string.IsNullOrWhiteSpace(row.RepoId))
          sqlDataRecord.SetString(num2 = num1 + 1, row.RepoId);
        else
          sqlDataRecord.SetString(num2 = num1 + 1, string.Empty);
        int num3;
        if (!string.IsNullOrWhiteSpace(row.RepoName))
          sqlDataRecord.SetString(num3 = num2 + 1, row.RepoName);
        else
          sqlDataRecord.SetString(num3 = num2 + 1, string.Empty);
        int num4;
        if (!string.IsNullOrWhiteSpace(row.Branch))
          sqlDataRecord.SetString(num4 = num3 + 1, row.Branch);
        else
          sqlDataRecord.SetString(num4 = num3 + 1, string.Empty);
        int num5;
        if (!string.IsNullOrWhiteSpace(row.CommitId))
          sqlDataRecord.SetString(num5 = num4 + 1, row.CommitId);
        else
          sqlDataRecord.SetString(num5 = num4 + 1, string.Empty);
        int num6;
        if (!string.IsNullOrWhiteSpace(row.Properties))
          sqlDataRecord.SetString(num6 = num5 + 1, row.Properties);
        else
          sqlDataRecord.SetDBNull(num6 = num5 + 1);
        return sqlDataRecord;
      })));
    }

    protected SqlParameter BindSubArtifactVersionTable(
      string parameterName,
      IList<SubArtifactDataRow> rows)
    {
      return this.BindTable(parameterName, "Deployment.typ_SubArtifactNameTableV1", rows.Select<SubArtifactDataRow, SqlDataRecord>((System.Func<SubArtifactDataRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ArtifactTraceabilityComponent.typ_SubArtifactNameTableV1);
        int ordinal1 = 0;
        sqlDataRecord.SetInt32(ordinal1, row.SubArtifactVersionId);
        int ordinal2;
        int num = ordinal2 = ordinal1 + 1;
        string subArtifactName = row.SubArtifactName;
        sqlDataRecord.SetString(ordinal2, subArtifactName);
        return sqlDataRecord;
      })));
    }

    protected SqlParameter BindArtifactTypeTable(string parameterName, IList<string> artifactTypes) => this.BindTable(parameterName, "Deployment.typ_ArtifactTypeTableV1", artifactTypes.Select<string, SqlDataRecord>((System.Func<string, SqlDataRecord>) (artifactType =>
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(ArtifactTraceabilityComponent.typ_ArtifactTypeTableV1);
      sqlDataRecord.SetString(0, artifactType);
      return sqlDataRecord;
    })));

    protected SqlParameter BindArtifactAliasTable(
      string parameterName,
      IList<string> artifactAliases)
    {
      return this.BindTable(parameterName, "Deployment.typ_ArtifactAliasTableV1", artifactAliases.Select<string, SqlDataRecord>((System.Func<string, SqlDataRecord>) (artifactAlias =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ArtifactTraceabilityComponent.typ_ArtifactAliasTableV1);
        sqlDataRecord.SetString(0, artifactAlias);
        return sqlDataRecord;
      })));
    }
  }
}
