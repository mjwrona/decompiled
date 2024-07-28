// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitConflictComponent
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitConflictComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<GitConflictComponent>(1370)
    }, "GitConflict");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1200002,
        new SqlExceptionFactory(typeof (GenericDatabaseUpdateErrorException))
      },
      {
        1200022,
        new SqlExceptionFactory(typeof (GenericErrorException))
      },
      {
        1200009,
        new SqlExceptionFactory(typeof (GitRepositoryNotFoundException))
      },
      {
        1200014,
        new SqlExceptionFactory(typeof (GitPullRequestNotFoundException))
      },
      {
        1200015,
        new SqlExceptionFactory(typeof (GitPullRequestNotEditableException))
      }
    };
    private static readonly SqlMetaData[] typ_GitConflict = new SqlMetaData[23]
    {
      new SqlMetaData("ConflictSourceId", SqlDbType.Int),
      new SqlMetaData("ConflictSourceType", SqlDbType.TinyInt),
      new SqlMetaData("ConflictId", SqlDbType.Int),
      new SqlMetaData("MergeBaseCommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("MergeSourceCommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("MergeTargetCommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("ConflictType", SqlDbType.TinyInt),
      new SqlMetaData("ConflictPath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("SourcePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("TargetPath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("BaseObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("BaseObjectIdForTarget", SqlDbType.Binary, 20L),
      new SqlMetaData("SourceObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("TargetObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("ResolutionStatus", SqlDbType.TinyInt),
      new SqlMetaData("ResolutionError", SqlDbType.TinyInt),
      new SqlMetaData("ResolutionAction", SqlDbType.TinyInt),
      new SqlMetaData("ResolutionMergeType", SqlDbType.TinyInt),
      new SqlMetaData("ResolutionPath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ResolutionObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("ResolvedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ResolvedDate", SqlDbType.DateTime2),
      new SqlMetaData("ResolutionAuthor", SqlDbType.UniqueIdentifier)
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) GitConflictComponent.s_sqlExceptionFactories;

    public GitConflictComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public virtual GitConflict UpdateGitConflictResolution(RepoKey repoKey, GitConflict conflict)
    {
      this.PrepareStoredProcedure("prc_UpdateGitConflictResolution");
      this.BindDataspace((RepoScope) repoKey);
      this.BindInt("@conflictSourceId", conflict.ConflictSourceId);
      this.BindByte("@conflictSourceType", (byte) conflict.ConflictSourceType);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@conflictId", conflict.ConflictId);
      this.BindByte("@resolutionStatus", (byte) conflict.ResolutionStatus);
      this.BindByte("@resolutionError", (byte) conflict.ResolutionError);
      this.BindByte("@resolutionAction", conflict.ResolutionAction);
      this.BindByte("@resolutionMergeType", conflict.ResolutionMergeType);
      this.BindString("@resolutionPath", conflict.ResolutionPath?.ToString(), int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      Sha1Id resolutionObjectId = conflict.ResolutionObjectId;
      if (!resolutionObjectId.IsEmpty)
      {
        resolutionObjectId = conflict.ResolutionObjectId;
        this.BindBinary("@resolutionObjectId", resolutionObjectId.ToByteArray(), 20, SqlDbType.Binary);
      }
      else
        this.BindNullValue("@resolutionObjectId", SqlDbType.Binary);
      this.BindNullableGuid("@resolvedBy", conflict.ResolvedBy);
      this.BindDateTime2("@resolvedDate", conflict.ResolvedDate);
      this.BindNullableGuid("@resolutionAuthor", conflict.ResolutionAuthor);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitConflict>((ObjectBinder<GitConflict>) new GitConflictComponent.GitConflictBinder());
        conflict = resultCollection.GetCurrent<GitConflict>().First<GitConflict>();
        return conflict;
      }
    }

    public virtual List<GitConflict> QueryGitConflicts(
      RepoKey repoKey,
      GitConflictSourceType conflictSourceType,
      int conflictSourceId,
      int top,
      int skip,
      bool includeObsolete,
      bool excludeResolved,
      bool onlyResolved,
      int minConflictId = 0)
    {
      this.PrepareStoredProcedure("prc_QueryGitConflicts");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@conflictSourceId", conflictSourceId);
      this.BindByte("@conflictSourceType", (byte) conflictSourceType);
      this.BindBoolean("@includeObsolete", includeObsolete);
      this.BindBoolean("@excludeResolved", excludeResolved);
      this.BindBoolean("@onlyResolved", onlyResolved);
      this.BindInt("@top", top);
      this.BindInt("@skip", skip);
      if (minConflictId != 0)
        this.BindInt("@minConflictId", minConflictId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitConflict>((ObjectBinder<GitConflict>) new GitConflictComponent.GitConflictBinder());
        return resultCollection.GetCurrent<GitConflict>().Items;
      }
    }

    public virtual GitConflict GetGitConflictById(
      RepoKey repoKey,
      GitConflictSourceType conflictSourceType,
      int conflictSourceId,
      int conflictId)
    {
      this.PrepareStoredProcedure("prc_GetGitConflictById");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@conflictSourceId", conflictSourceId);
      this.BindByte("@conflictSourceType", (byte) conflictSourceType);
      this.BindInt("@conflictId", conflictId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitConflict>((ObjectBinder<GitConflict>) new GitConflictComponent.GitConflictBinder());
        return resultCollection.GetCurrent<GitConflict>().FirstOrDefault<GitConflict>();
      }
    }

    public void UpdateGitPullRequestMergeStatusAndConflicts(
      RepoKey repoKey,
      int pullRequestId,
      PullRequestAsyncStatus mergeStatus,
      Sha1Id mergeSourceCommitId,
      Sha1Id mergeTargetCommitId,
      Sha1Id mergeCommitId,
      PullRequestMergeFailureType mergeFailureType,
      string mergeFailureMessage,
      IEnumerable<GitConflict> conflicts)
    {
      if (mergeSourceCommitId.IsEmpty)
        throw new ArgumentException("EmptySha1IdNotAllowed", nameof (mergeSourceCommitId));
      if (mergeSourceCommitId.IsEmpty)
        throw new ArgumentException("EmptySha1IdNotAllowed", nameof (mergeSourceCommitId));
      if (mergeStatus != PullRequestAsyncStatus.Succeeded != mergeCommitId.IsEmpty)
        throw new InvalidOperationException();
      this.PrepareStoredProcedure("prc_UpdateGitPullRequestMergeStatusAndConflicts");
      this.BindDataspace((RepoScope) repoKey);
      this.BindGuid("@repositoryId", repoKey.RepoId);
      this.BindInt("@pullRequestId", pullRequestId);
      this.BindByte("@mergeStatus", (byte) mergeStatus);
      this.BindBinary("@mergeSourceCommitId", mergeSourceCommitId.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBinary("@mergeTargetCommitId", mergeTargetCommitId.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBinary("@mergeCommitId", mergeCommitId.ToByteArray(), 20, SqlDbType.Binary);
      this.BindShort("@mergeFailureType", (short) mergeFailureType);
      this.BindString("@mergeFailureMessage", mergeFailureMessage, 4000, true, SqlDbType.NVarChar);
      this.BindGitPullRequestConflictTable("@conflicts", conflicts);
      this.ExecuteNonQuery();
    }

    private SqlParameter BindGitPullRequestConflictTable(
      string parameterName,
      IEnumerable<GitConflict> rows)
    {
      System.Func<GitConflict, SqlDataRecord> selector = (System.Func<GitConflict, SqlDataRecord>) (row =>
      {
        ArgumentUtility.CheckForNull<NormalizedGitPath>(row.ConflictPath, "ConflictPath");
        SqlDataRecord record = new SqlDataRecord(GitConflictComponent.typ_GitConflict);
        record.SetInt32(0, row.ConflictSourceId);
        record.SetByte(1, (byte) 0);
        record.SetInt32(2, row.ConflictId);
        record.SetBytes(3, 0L, row.MergeBaseCommitId.ToByteArray(), 0, 20);
        record.SetBytes(4, 0L, row.MergeSourceCommitId.ToByteArray(), 0, 20);
        record.SetBytes(5, 0L, row.MergeTargetCommitId.ToByteArray(), 0, 20);
        record.SetByte(6, (byte) row.ConflictType);
        record.SetString(7, row.ConflictPath.ToString(), BindStringBehavior.Unchanged);
        record.SetString(8, row.SourcePath?.ToString(), BindStringBehavior.Unchanged);
        record.SetString(9, row.TargetPath?.ToString(), BindStringBehavior.Unchanged);
        GitConflictComponent.SetNullableSha1Id(record, 10, row.BaseObjectId);
        GitConflictComponent.SetNullableSha1Id(record, 11, row.BaseObjectIdForTarget);
        GitConflictComponent.SetNullableSha1Id(record, 12, row.SourceObjectId);
        GitConflictComponent.SetNullableSha1Id(record, 13, row.TargetObjectId);
        record.SetByte(14, (byte) row.ResolutionStatus);
        record.SetByte(15, (byte) row.ResolutionError);
        record.SetByte(16, row.ResolutionAction);
        record.SetByte(17, row.ResolutionMergeType);
        record.SetString(18, row.ResolutionPath?.ToString(), BindStringBehavior.Unchanged);
        GitConflictComponent.SetNullableSha1Id(record, 19, row.ResolutionObjectId);
        record.SetNullableGuid(20, row.ResolvedBy);
        record.SetDateTime(21, row.ResolvedDate);
        record.SetNullableGuid(22, row.ResolutionAuthor);
        return record;
      });
      return this.BindTable(parameterName, "typ_GitConflict", rows.Select<GitConflict, SqlDataRecord>(selector));
    }

    private static void SetNullableSha1Id(SqlDataRecord record, int ordinal, Sha1Id value)
    {
      if (value.IsEmpty)
        record.SetDBNull(ordinal);
      else
        record.SetBytes(ordinal, 0L, value.ToByteArray(), 0, 20);
    }

    private void BindDataspace(RepoScope scopeToBind) => this.BindInt("@dataspaceId", this.GetDataspaceId(scopeToBind.ProjectId, "Git"));

    private class GitConflictBinder : ObjectBinder<GitConflict>
    {
      internal SqlColumnBinder pullRequestId = new SqlColumnBinder("PullRequestId");
      internal SqlColumnBinder conflictSourceId = new SqlColumnBinder("ConflictSourceId");
      internal SqlColumnBinder conflictSourceType = new SqlColumnBinder("ConflictSourceType");
      internal SqlColumnBinder conflictId = new SqlColumnBinder("ConflictId");
      internal SqlColumnBinder mergeBaseCommitId = new SqlColumnBinder("MergeBaseCommitId");
      internal SqlColumnBinder mergeSourceCommitId = new SqlColumnBinder("MergeSourceCommitId");
      internal SqlColumnBinder mergeTargetCommitId = new SqlColumnBinder("MergeTargetCommitId");
      internal SqlColumnBinder conflictType = new SqlColumnBinder("ConflictType");
      internal SqlColumnBinder conflictPath = new SqlColumnBinder("ConflictPath");
      internal SqlColumnBinder sourcePath = new SqlColumnBinder("SourcePath");
      internal SqlColumnBinder targetPath = new SqlColumnBinder("TargetPath");
      internal SqlColumnBinder baseObjectId = new SqlColumnBinder("BaseObjectId");
      internal SqlColumnBinder baseObjectIdForTarget = new SqlColumnBinder("BaseObjectIdForTarget");
      internal SqlColumnBinder sourceObjectId = new SqlColumnBinder("SourceObjectId");
      internal SqlColumnBinder targetObjectId = new SqlColumnBinder("TargetObjectId");
      internal SqlColumnBinder resolutionStatus = new SqlColumnBinder("ResolutionStatus");
      internal SqlColumnBinder resolutionError = new SqlColumnBinder("ResolutionError");
      internal SqlColumnBinder resolutionAction = new SqlColumnBinder("ResolutionAction");
      internal SqlColumnBinder resolutionMergeType = new SqlColumnBinder("ResolutionMergeType");
      internal SqlColumnBinder resolutionPath = new SqlColumnBinder("ResolutionPath");
      internal SqlColumnBinder resolutionObjectId = new SqlColumnBinder("ResolutionObjectId");
      internal SqlColumnBinder resolvedBy = new SqlColumnBinder("ResolvedBy");
      internal SqlColumnBinder resolvedDate = new SqlColumnBinder("ResolvedDate");
      internal SqlColumnBinder resolutionAuthor = new SqlColumnBinder("ResolutionAuthor");

      protected override GitConflict Bind()
      {
        string path1 = this.sourcePath.GetString((IDataReader) this.Reader, true);
        string path2 = this.targetPath.GetString((IDataReader) this.Reader, true);
        string path3 = this.resolutionPath.GetString((IDataReader) this.Reader, true);
        int num1 = this.conflictSourceType.ColumnExists((IDataReader) this.Reader) ? (int) this.conflictSourceType.GetByte((IDataReader) this.Reader) : 0;
        int conflictSourceId = this.conflictSourceId.ColumnExists((IDataReader) this.Reader) ? this.conflictSourceId.GetInt32((IDataReader) this.Reader) : this.pullRequestId.GetInt32((IDataReader) this.Reader);
        int int32 = this.conflictId.GetInt32((IDataReader) this.Reader);
        Sha1Id sha1Id1 = this.mergeBaseCommitId.GetSha1Id((IDataReader) this.Reader);
        Sha1Id sha1Id2 = this.mergeSourceCommitId.GetSha1Id((IDataReader) this.Reader);
        Sha1Id sha1Id3 = this.mergeTargetCommitId.GetSha1Id((IDataReader) this.Reader);
        int conflictType = (int) this.conflictType.GetByte((IDataReader) this.Reader);
        NormalizedGitPath conflictPath = new NormalizedGitPath(this.conflictPath.GetString((IDataReader) this.Reader, false));
        NormalizedGitPath sourcePath = path1 != null ? new NormalizedGitPath(path1) : (NormalizedGitPath) null;
        NormalizedGitPath targetPath = path2 != null ? new NormalizedGitPath(path2) : (NormalizedGitPath) null;
        Sha1Id? nullableSha1Id = this.baseObjectId.GetNullableSha1Id((IDataReader) this.Reader);
        Sha1Id baseObjectId = nullableSha1Id ?? Sha1Id.Empty;
        nullableSha1Id = this.baseObjectIdForTarget.GetNullableSha1Id((IDataReader) this.Reader);
        Sha1Id baseObjectIdForTarget = nullableSha1Id ?? Sha1Id.Empty;
        nullableSha1Id = this.sourceObjectId.GetNullableSha1Id((IDataReader) this.Reader);
        Sha1Id sourceObjectId = nullableSha1Id ?? Sha1Id.Empty;
        nullableSha1Id = this.targetObjectId.GetNullableSha1Id((IDataReader) this.Reader);
        Sha1Id targetObjectId = nullableSha1Id ?? Sha1Id.Empty;
        int num2 = (int) this.resolutionStatus.GetByte((IDataReader) this.Reader);
        int resolutionError = (int) this.resolutionError.GetByte((IDataReader) this.Reader);
        int resolutionAction = (int) this.resolutionAction.GetByte((IDataReader) this.Reader);
        int resolutionMergeType = (int) this.resolutionMergeType.GetByte((IDataReader) this.Reader);
        NormalizedGitPath resolutionPath = path3 != null ? new NormalizedGitPath(path3) : (NormalizedGitPath) null;
        nullableSha1Id = this.resolutionObjectId.GetNullableSha1Id((IDataReader) this.Reader);
        Sha1Id resolutionObjectId = nullableSha1Id ?? Sha1Id.Empty;
        Guid guid1 = this.resolvedBy.GetGuid((IDataReader) this.Reader, true);
        DateTime dateTime = this.resolvedDate.GetDateTime((IDataReader) this.Reader);
        Guid guid2 = this.resolutionAuthor.GetGuid((IDataReader) this.Reader, true);
        return new GitConflict((GitConflictSourceType) num1, conflictSourceId, int32, sha1Id1, sha1Id2, sha1Id3, (GitConflictType) conflictType, conflictPath, sourcePath, targetPath, baseObjectId, baseObjectIdForTarget, sourceObjectId, targetObjectId, (GitResolutionStatus) num2, (GitResolutionError) resolutionError, (byte) resolutionAction, (byte) resolutionMergeType, resolutionPath, resolutionObjectId, guid1, dateTime, guid2);
      }
    }
  }
}
