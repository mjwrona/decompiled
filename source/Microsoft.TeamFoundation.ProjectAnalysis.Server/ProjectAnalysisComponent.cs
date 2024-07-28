// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.ProjectAnalysisComponent
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public class ProjectAnalysisComponent : ProjectAnalysisComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ProjectAnalysisComponent>(1),
      (IComponentCreator) new ComponentCreator<ProjectAnalysisComponent2>(2),
      (IComponentCreator) new ComponentCreator<ProjectAnalysisComponent3>(3)
    }, "ProjectAnalysis");
    private const string c_serviceName = "ProjectAnalysis";

    public virtual List<LanguageMetadataRecord> GetLanguageMetadata(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_GetLanguageMetadata");
      this.BindDataspaceId(projectId);
      this.BindProjectId(projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<LanguageMetadataRecord>((ObjectBinder<LanguageMetadataRecord>) this.GetBinder());
        return resultCollection.GetCurrent<LanguageMetadataRecord>().Items;
      }
    }

    public virtual void CreateLanguageMetadata(LanguageMetadataRecord record)
    {
      this.PrepareStoredProcedure("prc_CreateLanguageMetadata");
      this.BindDataspaceId(record.ProjectId);
      this.BindProjectId(record.ProjectId);
      this.BindByte("@repositoryType", (byte) record.RepositoryType);
      this.BindGuid("@repositoryId", record.RepositoryId);
      this.BindInt("@fileCount", record.FileCount);
      this.BindNullableInt("@changeSetId", record.ChangesetId);
      Sha1Id? commitId = record.CommitId;
      ref Sha1Id? local = ref commitId;
      this.BindBinary("@commitId", local.HasValue ? local.GetValueOrDefault().ToByteArray() : (byte[]) null, 20, SqlDbType.Binary);
      this.BindString("@branch", record.Branch, 400, false, SqlDbType.NVarChar);
      this.BindString("@languageBreakdown", record.LanguageBreakdown.Serialize<List<LanguageStatistics>>(true), int.MaxValue, false, SqlDbType.NVarChar);
      this.BindByte("@resultPhase", (byte) record.ResultPhase);
      this.BindRecordVersion(record);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateLanguageMetadata(LanguageMetadataRecord record)
    {
      this.PrepareStoredProcedure("prc_UpdateLanguageMetadata");
      this.BindDataspaceId(record.ProjectId);
      this.BindProjectId(record.ProjectId);
      this.BindByte("@repositoryType", (byte) record.RepositoryType);
      this.BindGuid("@repositoryId", record.RepositoryId);
      this.BindInt("@fileCount", record.FileCount);
      this.BindNullableInt("@changeSetId", record.ChangesetId);
      Sha1Id? commitId = record.CommitId;
      ref Sha1Id? local = ref commitId;
      this.BindBinary("@commitId", local.HasValue ? local.GetValueOrDefault().ToByteArray() : (byte[]) null, 20, SqlDbType.Binary);
      this.BindString("@branch", record.Branch, 400, false, SqlDbType.NVarChar);
      this.BindString("@languageBreakdown", record.LanguageBreakdown.Serialize<List<LanguageStatistics>>(true), int.MaxValue, false, SqlDbType.NVarChar);
      this.BindByte("@resultPhase", (byte) record.ResultPhase);
      this.BindRecordVersion(record);
      this.ExecuteNonQuery();
    }

    public virtual LanguageMetadataRecord GetRepositoryLanguageMetadata(
      Guid projectId,
      RepositoryType repositoryType,
      Guid repositoryId)
    {
      this.PrepareStoredProcedure("prc_GetRepoLanguageMetadata");
      this.BindDataspaceId(projectId);
      this.BindProjectId(projectId);
      this.BindByte("@repositoryType", (byte) repositoryType);
      this.BindGuid("@repositoryId", repositoryId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<LanguageMetadataRecord>((ObjectBinder<LanguageMetadataRecord>) this.GetBinder());
        return resultCollection.GetCurrent<LanguageMetadataRecord>().FirstOrDefault<LanguageMetadataRecord>();
      }
    }

    protected virtual void BindRecordVersion(LanguageMetadataRecord record)
    {
    }

    protected virtual void BindDataspaceId(Guid projectId)
    {
    }

    protected virtual void BindProjectId(Guid projectId) => this.BindGuid("@projectId", projectId);

    internal virtual LanguageMetadataRecordBinder GetBinder() => new LanguageMetadataRecordBinder();
  }
}
