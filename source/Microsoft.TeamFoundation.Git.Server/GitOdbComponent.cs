// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitOdbComponent
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly, null)]
  internal sealed class GitOdbComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly SqlMetaData[] GitOdb_typ_CommitChangeSummary = new SqlMetaData[4]
    {
      new SqlMetaData("CommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("NumAdds", SqlDbType.Int),
      new SqlMetaData("NumEdits", SqlDbType.Int),
      new SqlMetaData("NumDeletes", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] GitOdb_typ_CommitMetadata2 = new SqlMetaData[7]
    {
      new SqlMetaData("CommitId", SqlDbType.Binary, 20L),
      new SqlMetaData("ShortComment", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Committer", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Author", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CommitTime", SqlDbType.DateTime),
      new SqlMetaData("AuthorTime", SqlDbType.DateTime),
      new SqlMetaData("IsCommentTruncated", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] GitOdb_typ_KnownFileUpdate = new SqlMetaData[3]
    {
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ActionDate", SqlDbType.DateTime2),
      new SqlMetaData("Type", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] GitOdb_typ_LfsObject = new SqlMetaData[2]
    {
      new SqlMetaData("ObjectId", SqlDbType.Binary, 32L),
      new SqlMetaData("Size", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] GitOdb_typ_ObjectIdLengthTable = new SqlMetaData[2]
    {
      new SqlMetaData("ObjectId", SqlDbType.Binary, 20L),
      new SqlMetaData("Length", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] GitOdb_typ_ObjectIdTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.Binary, 20L)
    };
    private static readonly SqlMetaData[] GitOdb_typ_Sha256ObjectId = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.Binary, 32L)
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<GitOdbComponent>(1590),
      (IComponentCreator) new ComponentCreator<GitOdbComponent>(1610),
      (IComponentCreator) new ComponentCreator<GitOdbComponent>(1700)
    }, "GitOdb");
    private OdbId m_odbId;
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    private const string c_dataspaceCategory = "GitOdb";

    private SqlParameter BindCommitChangeSummaryTable(
      string parameterName,
      IEnumerable<GitCommitChangeSummary> rows)
    {
      rows = rows ?? Enumerable.Empty<GitCommitChangeSummary>();
      System.Func<GitCommitChangeSummary, SqlDataRecord> selector = (System.Func<GitCommitChangeSummary, SqlDataRecord>) (change =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitOdbComponent.GitOdb_typ_CommitChangeSummary);
        sqlDataRecord.SetBytes(0, 0L, change.CommitId.ToByteArray(), 0, 20);
        sqlDataRecord.SetInt32(1, change.ChangeCounts.Adds);
        sqlDataRecord.SetInt32(2, change.ChangeCounts.Edits);
        sqlDataRecord.SetInt32(3, change.ChangeCounts.Deletes);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "GitOdb.typ_CommitChangeSummary", rows.Select<GitCommitChangeSummary, SqlDataRecord>(selector));
    }

    private SqlParameter BindCommitMetadataTable2(
      string parameterName,
      IEnumerable<CommitMetadataUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<CommitMetadataUpdate>();
      System.Func<CommitMetadataUpdate, SqlDataRecord> selector = (System.Func<CommitMetadataUpdate, SqlDataRecord>) (commit =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitOdbComponent.GitOdb_typ_CommitMetadata2);
        sqlDataRecord.SetBytes(0, 0L, commit.Metadata.CommitId.ToByteArray(), 0, 20);
        sqlDataRecord.SetString(1, commit.Metadata.ShortComment);
        sqlDataRecord.SetString(2, commit.Metadata.Committer);
        sqlDataRecord.SetString(3, commit.Metadata.Author);
        sqlDataRecord.SetDateTime(4, commit.Metadata.CommitTime);
        sqlDataRecord.SetDateTime(5, commit.Metadata.AuthorTime);
        sqlDataRecord.SetBoolean(6, commit.Metadata.ShortCommentIsTruncated);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "GitOdb.typ_CommitMetadata2", rows.Select<CommitMetadataUpdate, SqlDataRecord>(selector));
    }

    private SqlParameter BindLfsObjectTable(string parameterName, IEnumerable<GitLfsObject> rows)
    {
      rows = rows ?? Enumerable.Empty<GitLfsObject>();
      System.Func<GitLfsObject, SqlDataRecord> selector = (System.Func<GitLfsObject, SqlDataRecord>) (lfs =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitOdbComponent.GitOdb_typ_LfsObject);
        sqlDataRecord.SetBytes(0, 0L, lfs.ObjectId.ToByteArray(), 0, 32);
        sqlDataRecord.SetInt64(1, lfs.Size);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "GitOdb.typ_LfsObject", rows.Select<GitLfsObject, SqlDataRecord>(selector));
    }

    private SqlParameter BindKnownFileCreateTable(
      string parameterName,
      IEnumerable<KeyValuePair<string, KnownFile>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<string, KnownFile>>();
      System.Func<KeyValuePair<string, KnownFile>, SqlDataRecord> selector = (System.Func<KeyValuePair<string, KnownFile>, SqlDataRecord>) (newFile =>
      {
        SqlDataRecord table = new SqlDataRecord(GitOdbComponent.GitOdb_typ_KnownFileUpdate);
        table.SetString(0, newFile.Key);
        table.SetDateTime(1, newFile.Value.CreatedDate);
        table.SetByte(2, (byte) newFile.Value.Type);
        return table;
      });
      return this.BindTable(parameterName, "GitOdb.typ_KnownFileUpdate", rows.Select<KeyValuePair<string, KnownFile>, SqlDataRecord>(selector));
    }

    private SqlParameter BindObjectIdTable(string parameterName, IEnumerable<Sha1Id> rows)
    {
      rows = rows ?? Enumerable.Empty<Sha1Id>();
      System.Func<Sha1Id, SqlDataRecord> selector = (System.Func<Sha1Id, SqlDataRecord>) (hash =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitOdbComponent.GitOdb_typ_ObjectIdTable);
        sqlDataRecord.SetBytes(0, 0L, hash.ToByteArray(), 0, 20);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "GitOdb.typ_ObjectIdTable", rows.Select<Sha1Id, SqlDataRecord>(selector));
    }

    private SqlParameter BindObjectIdLengthTable(
      string parameterName,
      IEnumerable<ObjectIdAndSize> rows)
    {
      rows = rows ?? Enumerable.Empty<ObjectIdAndSize>();
      System.Func<ObjectIdAndSize, SqlDataRecord> selector = (System.Func<ObjectIdAndSize, SqlDataRecord>) (objectSize =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitOdbComponent.GitOdb_typ_ObjectIdLengthTable);
        sqlDataRecord.SetBytes(0, 0L, objectSize.Id.ToByteArray(), 0, 20);
        sqlDataRecord.SetInt64(1, objectSize.Size);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "GitOdb.typ_ObjectIdLengthTable", rows.Select<ObjectIdAndSize, SqlDataRecord>(selector));
    }

    private SqlParameter BindSha256ObjectId(string parameterName, IEnumerable<Sha256Id> rows)
    {
      rows = rows ?? Enumerable.Empty<Sha256Id>();
      System.Func<Sha256Id, SqlDataRecord> selector = (System.Func<Sha256Id, SqlDataRecord>) (hash =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GitOdbComponent.GitOdb_typ_Sha256ObjectId);
        sqlDataRecord.SetBytes(0, 0L, hash.ToByteArray(), 0, 32);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "GitOdb.typ_Sha256ObjectId", rows.Select<Sha256Id, SqlDataRecord>(selector));
    }

    static GitOdbComponent() => GitOdbComponent.s_sqlExceptionFactories.Add(1200026, new SqlExceptionFactory(typeof (GitInvalidParentSpecified)));

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) GitOdbComponent.s_sqlExceptionFactories;

    public GitOdbComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.m_odbId = new OdbId(dataspaceIdentifier);
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public void CreateFork(MinimalGlobalRepoKey repoKey)
    {
      this.PrepareStoredProcedure("prc_CreateFork");
      this.BindGuid("@collectionId", repoKey.CollectionId);
      this.BindGuid("@repositoryId", repoKey.RepositoryId);
      this.ExecuteNonQuery();
    }

    public void CreateForkRelationship(MinimalGlobalRepoKey source, MinimalGlobalRepoKey target)
    {
      this.PrepareStoredProcedure("prc_CreateForkRelationship");
      this.BindGuid("@sourceCollectionId", source.CollectionId);
      this.BindGuid("@sourceRepositoryId", source.RepositoryId);
      this.BindGuid("@targetCollectionId", target.CollectionId);
      this.BindGuid("@targetRepositoryId", target.RepositoryId);
      this.ExecuteNonQuery();
    }

    public void DeleteCommits(IEnumerable<Sha1Id> commits)
    {
      this.EnforceMinimalVersion(1590);
      this.PrepareStoredProcedure("prc_DeleteCommits");
      this.BindObjectIdTable("@commits", commits);
      this.ExecuteNonQuery();
    }

    public void DeleteFork(MinimalGlobalRepoKey toDelete)
    {
      this.PrepareStoredProcedure("prc_DeleteFork");
      this.BindGuid("@collectionId", toDelete.CollectionId);
      this.BindGuid("@repositoryId", toDelete.RepositoryId);
      this.ExecuteNonQuery();
    }

    public void DeleteForkRelationships(MinimalGlobalRepoKey toDelete)
    {
      this.PrepareStoredProcedure("prc_DeleteForkRelationships");
      this.BindGuid("@collectionId", toDelete.CollectionId);
      this.BindGuid("@repositoryId", toDelete.RepositoryId);
      this.ExecuteNonQuery();
    }

    public void DeleteKnownFiles(IEnumerable<string> deletes)
    {
      this.PrepareStoredProcedure("prc_DeleteKnownFiles");
      this.BindStringTable("@deletes", deletes);
      this.ExecuteNonQuery();
    }

    public void DeleteOdb()
    {
      this.PrepareStoredProcedure("prc_DeleteOdb", 0);
      this.ExecuteNonQuery();
    }

    public List<Sha1Id> GetAllCommits(Sha1Id? afterCommitId = null)
    {
      this.EnforceMinimalVersion(1590);
      this.PrepareStoredProcedure("prc_GetCommits");
      if (afterCommitId.HasValue)
        this.BindBinary("@afterCommitId", afterCommitId.ToByteArray(), 20, SqlDbType.Binary);
      return this.GetCommitIdResult();
    }

    public MinimalGlobalRepoKey? GetParent(MinimalGlobalRepoKey globalKey)
    {
      this.PrepareStoredProcedure("prc_GetParent");
      this.BindGuid("@collectionId", globalKey.CollectionId);
      this.BindGuid("@repositoryId", globalKey.RepositoryId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<MinimalGlobalRepoKey>((ObjectBinder<MinimalGlobalRepoKey>) new GitOdbComponent.MinimalGlobalRepoKeyBinder());
        List<MinimalGlobalRepoKey> items = resultCollection.GetCurrent<MinimalGlobalRepoKey>().Items;
        return items.Count == 0 ? new MinimalGlobalRepoKey?() : new MinimalGlobalRepoKey?(items.First<MinimalGlobalRepoKey>());
      }
    }

    public List<string> QueryCommitAuthors()
    {
      this.PrepareStoredProcedure("prc_QueryCommitAuthors");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder fullName = new SqlColumnBinder("FullName");
        resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => fullName.GetString(reader, false))));
        return resultCollection.GetCurrent<string>().Items;
      }
    }

    public IReadOnlyList<MinimalGlobalRepoKey> QueryChildren(
      MinimalGlobalRepoKey globalKey,
      Guid inCollectionId)
    {
      this.PrepareStoredProcedure("prc_QueryChildren");
      this.BindGuid("@collectionId", globalKey.CollectionId);
      this.BindGuid("@repositoryId", globalKey.RepositoryId);
      if (inCollectionId == this.RequestContext.ServiceHost.InstanceId)
        this.BindGuid("@inCollectionId", Guid.Empty);
      else
        this.BindGuid("@inCollectionId", inCollectionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<MinimalGlobalRepoKey>((ObjectBinder<MinimalGlobalRepoKey>) new GitOdbComponent.MinimalGlobalRepoKeyBinder());
        return (IReadOnlyList<MinimalGlobalRepoKey>) resultCollection.GetCurrent<MinimalGlobalRepoKey>().Items;
      }
    }

    public List<Sha1Id> QueryCommits(
      RepoKey repoKey,
      string author,
      string committer,
      DateTime? fromDate,
      DateTime? toDate,
      Sha1Id fromCommitId,
      Sha1Id toCommitId,
      int? skip,
      int? take,
      bool orderByCommitTimeAscending = false)
    {
      this.PrepareStoredProcedure("prc_QueryCommits");
      if ((RepoScope) repoKey != (RepoScope) null)
      {
        this.BindDataspaceIdForProject(repoKey.ProjectId);
        this.BindGuid("@repositoryId", repoKey.RepoId);
      }
      this.BindString("@author", author, -1, true, SqlDbType.NVarChar);
      this.BindString("@committer", committer, -1, true, SqlDbType.NVarChar);
      this.BindBinary("@fromCommitId", fromCommitId.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBinary("@toCommitId", toCommitId.ToByteArray(), 20, SqlDbType.Binary);
      if (fromDate.HasValue)
        this.BindDateTime("@fromDate", fromDate.Value, true);
      if (toDate.HasValue)
        this.BindDateTime("@toDate", toDate.Value, true);
      if (skip.HasValue)
        this.BindInt("@skip", skip.Value);
      if (take.HasValue)
        this.BindInt("@take", take.Value);
      if (this.Version >= 1610)
        this.BindBoolean("@orderByCommitTimeAscending", orderByCommitTimeAscending);
      return this.GetCommitIdResult();
    }

    public List<MinimalGlobalRepoKey> QueryForksByOdb()
    {
      this.PrepareStoredProcedure("prc_QueryForksByOdb");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<MinimalGlobalRepoKey>((ObjectBinder<MinimalGlobalRepoKey>) new GitOdbComponent.MinimalGlobalRepoKeyBinder());
        return resultCollection.GetCurrent<MinimalGlobalRepoKey>().Items;
      }
    }

    public List<Sha1Id> QueryMissingCommits(IEnumerable<Sha1Id> commitIds)
    {
      this.PrepareStoredProcedure("prc_QueryMissingCommits");
      this.BindObjectIdTable("@commitIds", commitIds);
      return this.GetCommitIdResult();
    }

    public List<Sha1Id> QueryUncalculatedCommitMetadata(int? top = null)
    {
      this.PrepareStoredProcedure("prc_QueryUncalculatedCommitMetadata", 3600);
      if (top.HasValue)
        this.BindInt("@top", top.Value);
      return this.GetCommitIdResult();
    }

    public IDictionary<Sha1Id, GitCommitChangeSummary> ReadCommitChangeCounts(
      IEnumerable<Sha1Id> commitIds)
    {
      this.PrepareStoredProcedure("prc_ReadCommitChangeCounts");
      this.BindObjectIdTable("@commits", commitIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitCommitChangeSummary>((ObjectBinder<GitCommitChangeSummary>) new GitOdbComponent.GitCommitChangeSummaryBinder());
        return (IDictionary<Sha1Id, GitCommitChangeSummary>) resultCollection.GetCurrent<GitCommitChangeSummary>().Items.ToDictionary<GitCommitChangeSummary, Sha1Id, GitCommitChangeSummary>((System.Func<GitCommitChangeSummary, Sha1Id>) (s => s.CommitId), (System.Func<GitCommitChangeSummary, GitCommitChangeSummary>) (s => s));
      }
    }

    public ResultCollection ReadCommitsById(IEnumerable<Sha1Id> commitIds)
    {
      this.PrepareStoredProcedure("prc_ReadCommitsById");
      this.BindObjectIdTable("@commitIds", commitIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TfsGitCommitMetadata>((ObjectBinder<TfsGitCommitMetadata>) new GitOdbComponent.GitCommitMetadataBinder());
      return resultCollection;
    }

    public IReadOnlyDictionary<string, KnownFile> ReadKnownFiles()
    {
      this.PrepareStoredProcedure("prc_ReadKnownFiles");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<KeyValuePair<string, KnownFile>>((ObjectBinder<KeyValuePair<string, KnownFile>>) new GitOdbComponent.KnownFileBinder());
        return (IReadOnlyDictionary<string, KnownFile>) resultCollection.GetCurrent<KeyValuePair<string, KnownFile>>().ToDictionary<KeyValuePair<string, KnownFile>, string, KnownFile>((System.Func<KeyValuePair<string, KnownFile>, string>) (pair => pair.Key), (System.Func<KeyValuePair<string, KnownFile>, KnownFile>) (pair => pair.Value));
      }
    }

    public DateTime? ReadDateTimeOfLastKnownFile(KnownFileType knownFileType)
    {
      this.PrepareStoredProcedure("prc_ReadDateTimeOfLastKnownFile");
      this.BindByte("@knownFileType", (byte) knownFileType);
      object obj = this.ExecuteScalar();
      switch (obj)
      {
        case DBNull _:
        case null:
          return new DateTime?();
        default:
          return (DateTime?) obj;
      }
    }

    public IReadOnlyDictionary<string, KnownFileIntervalData> ReadKnownFilesInterval(
      DateTime? minCreatedDate,
      int? minIntervals)
    {
      this.PrepareStoredProcedure("prc_ReadKnownFilesIntervals");
      this.BindNullableDateTime2("@minCreatedDate", minCreatedDate);
      this.BindNullableInt("@minIntervals", minIntervals);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<KeyValuePair<string, KnownFileIntervalData>>((ObjectBinder<KeyValuePair<string, KnownFileIntervalData>>) new GitOdbComponent.KnownFileIntervalBinder());
        return (IReadOnlyDictionary<string, KnownFileIntervalData>) resultCollection.GetCurrent<KeyValuePair<string, KnownFileIntervalData>>().ToDictionary<KeyValuePair<string, KnownFileIntervalData>, string, KnownFileIntervalData>((System.Func<KeyValuePair<string, KnownFileIntervalData>, string>) (pair => pair.Key), (System.Func<KeyValuePair<string, KnownFileIntervalData>, KnownFileIntervalData>) (pair => pair.Value));
      }
    }

    public IReadOnlyList<GitLfsObject> ReadAllLfsObjects()
    {
      this.PrepareStoredProcedure("prc_ReadAllLfsObjects");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitLfsObject>((ObjectBinder<GitLfsObject>) new GitOdbComponent.GitLfsObjectBinder());
        return (IReadOnlyList<GitLfsObject>) resultCollection.GetCurrent<GitLfsObject>().Items;
      }
    }

    public IReadOnlyList<GitLfsObject> ReadLfsObjects(IEnumerable<Sha256Id> objectIds)
    {
      this.PrepareStoredProcedure("prc_ReadLfsObjects");
      this.BindSha256ObjectId("@objectIds", objectIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitLfsObject>((ObjectBinder<GitLfsObject>) new GitOdbComponent.GitLfsObjectBinder());
        return (IReadOnlyList<GitLfsObject>) resultCollection.GetCurrent<GitLfsObject>().Items;
      }
    }

    public IReadOnlyList<ObjectIdAndSize> ReadObjectLengths(IEnumerable<Sha1Id> objectIds)
    {
      this.PrepareStoredProcedure("prc_ReadObjectLengths");
      this.BindObjectIdTable("@objectIds", objectIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ObjectIdAndSize>((ObjectBinder<ObjectIdAndSize>) new GitOdbComponent.GitObjectSizeBinder());
        return (IReadOnlyList<ObjectIdAndSize>) resultCollection.GetCurrent<ObjectIdAndSize>().Items;
      }
    }

    public Sha1Id? ReadPointer(OdbPointerType pointerType)
    {
      this.PrepareStoredProcedure("prc_ReadPointer");
      this.BindByte("@pointerType", (byte) pointerType);
      object sha1IdBytes = this.ExecuteScalar();
      switch (sha1IdBytes)
      {
        case DBNull _:
        case null:
          return new Sha1Id?();
        default:
          return new Sha1Id?(new Sha1Id((byte[]) sha1IdBytes));
      }
    }

    public void UpdateCommitMetadata(IEnumerable<CommitMetadataUpdate> commits)
    {
      this.PrepareStoredProcedure("prc_UpdateCommitMetadata", 600);
      this.BindCommitMetadataTable2("@metadata", commits);
      this.BindCommitChangeSummaryTable("@changes", commits.Select<CommitMetadataUpdate, GitCommitChangeSummary>((System.Func<CommitMetadataUpdate, GitCommitChangeSummary>) (x => x.ToChangeSummary())));
      this.ExecuteNonQuery();
    }

    public Sha1Id? UpdatePointer(
      OdbPointerType pointerType,
      Sha1Id? oldPointerId,
      Sha1Id? newPointerId)
    {
      this.PrepareStoredProcedure("prc_UpdatePointer");
      this.BindByte("@pointerType", (byte) pointerType);
      this.BindBinary("@oldPointerId", oldPointerId.ToByteArray(), 20, SqlDbType.Binary);
      this.BindBinary("@newPointerId", newPointerId.ToByteArray(), 20, SqlDbType.Binary);
      object sha1IdBytes = this.ExecuteScalar();
      switch (sha1IdBytes)
      {
        case DBNull _:
        case null:
          return new Sha1Id?();
        default:
          return new Sha1Id?(new Sha1Id((byte[]) sha1IdBytes));
      }
    }

    public List<Sha1Id> WriteCommits(
      IEnumerable<Sha1Id> newCommits,
      IEnumerable<Sha1Id> commitParents)
    {
      this.PrepareStoredProcedure("prc_WriteCommits", 900);
      this.BindObjectIdTable("@commitsIn", newCommits);
      this.BindObjectIdTable("@commitParentsIn", commitParents);
      return this.GetCommitIdResult();
    }

    public void WriteKnownFiles(IReadOnlyDictionary<string, KnownFile> creates)
    {
      this.PrepareStoredProcedure("prc_WriteKnownFiles");
      this.BindKnownFileCreateTable("@creates", (IEnumerable<KeyValuePair<string, KnownFile>>) creates);
      this.ExecuteNonQuery();
    }

    public void WriteKnownFilesIntervals(ISet<string> increments, ISet<string> resets)
    {
      this.PrepareStoredProcedure("prc_WriteKnownFilesIntervals");
      this.BindStringTable("@increments", (IEnumerable<string>) increments);
      this.BindStringTable("@resets", (IEnumerable<string>) resets);
      this.ExecuteNonQuery();
    }

    public IReadOnlyList<GitLfsObject> WriteLfsObjects(IEnumerable<GitLfsObject> toWrite)
    {
      this.PrepareStoredProcedure("prc_WriteLfsObjects");
      this.BindLfsObjectTable("@inserts", toWrite);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GitLfsObject>((ObjectBinder<GitLfsObject>) new GitOdbComponent.GitLfsObjectBinder());
        return (IReadOnlyList<GitLfsObject>) resultCollection.GetCurrent<GitLfsObject>().Items;
      }
    }

    public void WriteObjectLengths(IEnumerable<ObjectIdAndSize> objectSizeMap)
    {
      this.PrepareStoredProcedure("prc_WriteObjectLengths");
      this.BindObjectIdLengthTable("@objectLengths", objectSizeMap);
      this.ExecuteNonQuery();
    }

    public void WritePolicyMigrationObject(
      Guid collectionId,
      Guid sourceRepoId,
      Guid targetRepoId,
      int sourcePolicyId,
      int sourcePolicyRevision,
      int targetPolicyId)
    {
      this.PrepareStoredProcedure("prc_WritePolicyMigrationObject");
      this.BindGuid("@collectionId", collectionId);
      this.BindGuid("@sourceRepositoryId", sourceRepoId);
      this.BindGuid("@targetRepositoryId", targetRepoId);
      this.BindInt("@sourcePolicyId", sourcePolicyId);
      this.BindInt("@sourcePolicyRevision", sourcePolicyRevision);
      this.BindInt("@targetPolicyId", targetPolicyId);
      this.ExecuteNonQuery();
    }

    protected override SqlCommand PrepareStoredProcedure(string sprocName)
    {
      SqlCommand sqlCommand = base.PrepareStoredProcedure("GitOdb." + sprocName);
      this.BindDataspaceId(this.m_odbId.Value);
      return sqlCommand;
    }

    protected override SqlCommand PrepareStoredProcedure(string sprocName, int timeoutSec)
    {
      SqlCommand sqlCommand = base.PrepareStoredProcedure("GitOdb." + sprocName, timeoutSec);
      this.BindDataspaceId(this.m_odbId.Value);
      return sqlCommand;
    }

    private void BindDataspaceId(Guid objectDbId)
    {
      IVssRequestContext requestContext = this.RequestContext;
      using (requestContext != null ? requestContext.AllowCrossDataspaceAccess() : (IDisposable) null)
        this.BindInt("@dataspaceId", this.GetDataspaceId(objectDbId, "GitOdb"));
    }

    private void BindDataspaceIdForProject(Guid projectId)
    {
      IVssRequestContext requestContext = this.RequestContext;
      using (requestContext != null ? requestContext.AllowCrossDataspaceAccess() : (IDisposable) null)
        this.BindInt("@gitDataspaceId", this.GetDataspaceId(projectId, "Git"));
    }

    private List<Sha1Id> GetCommitIdResult()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder commitIdBinder = new SqlColumnBinder("CommitId");
        resultCollection.AddBinder<Sha1Id>((ObjectBinder<Sha1Id>) new SimpleObjectBinder<Sha1Id>((System.Func<IDataReader, Sha1Id>) (reader => commitIdBinder.GetSha1Id(reader))));
        return resultCollection.GetCurrent<Sha1Id>().Items;
      }
    }

    private void EnforceMinimalVersion(int version)
    {
      if (this.Version < version)
        throw new ServiceVersionNotSupportedException(GitOdbComponent.ComponentFactory.ServiceName, this.Version, version);
    }

    private class GitCommitChangeSummaryBinder : ObjectBinder<GitCommitChangeSummary>
    {
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder m_status = new SqlColumnBinder("Status");
      private SqlColumnBinder m_numAdds = new SqlColumnBinder("NumAdds");
      private SqlColumnBinder m_numEdits = new SqlColumnBinder("NumEdits");
      private SqlColumnBinder m_numDeletes = new SqlColumnBinder("NumDeletes");

      protected override GitCommitChangeSummary Bind()
      {
        GitCommitMetadataStatus status = (GitCommitMetadataStatus) this.m_status.GetByte((IDataReader) this.Reader, (byte) 0);
        ChangeCounts changeCounts = status == GitCommitMetadataStatus.ChangesCalculated ? new ChangeCounts(this.m_numAdds.GetInt32((IDataReader) this.Reader), this.m_numEdits.GetInt32((IDataReader) this.Reader), this.m_numDeletes.GetInt32((IDataReader) this.Reader)) : (ChangeCounts) null;
        return new GitCommitChangeSummary(this.m_commitId.GetSha1Id((IDataReader) this.Reader), status, changeCounts);
      }
    }

    private class GitCommitMetadataBinder : ObjectBinder<TfsGitCommitMetadata>
    {
      private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
      private SqlColumnBinder m_comment = new SqlColumnBinder("Comment");
      private SqlColumnBinder m_committer = new SqlColumnBinder("Committer");
      private SqlColumnBinder m_author = new SqlColumnBinder("Author");
      private SqlColumnBinder m_commitTime = new SqlColumnBinder("CommitTime");
      private SqlColumnBinder m_authorTime = new SqlColumnBinder("AuthorTime");
      private SqlColumnBinder m_isCommentTruncated = new SqlColumnBinder("IsCommentTruncated");

      protected override TfsGitCommitMetadata Bind() => new TfsGitCommitMetadata(this.m_commitId.GetSha1Id((IDataReader) this.Reader), this.m_comment.GetString((IDataReader) this.Reader, true), this.m_committer.GetString((IDataReader) this.Reader, true), this.m_author.GetString((IDataReader) this.Reader, true), this.m_commitTime.GetDateTime((IDataReader) this.Reader), this.m_authorTime.GetDateTime((IDataReader) this.Reader), this.m_isCommentTruncated.GetBoolean((IDataReader) this.Reader));
    }

    private class GitLfsObjectBinder : ObjectBinder<GitLfsObject>
    {
      private SqlColumnBinder m_objectId = new SqlColumnBinder("ObjectId");
      private SqlColumnBinder m_objectIndex = new SqlColumnBinder("ObjectIndex");
      private SqlColumnBinder m_size = new SqlColumnBinder("Size");

      protected override GitLfsObject Bind()
      {
        Sha256Id sha256Id = this.m_objectId.GetSha256Id((IDataReader) this.Reader);
        int int32 = this.m_objectIndex.GetInt32((IDataReader) this.Reader);
        long int64 = this.m_size.GetInt64((IDataReader) this.Reader);
        int objectIndex = int32;
        long size = int64;
        return new GitLfsObject(sha256Id, objectIndex, size);
      }
    }

    private class GitObjectSizeBinder : ObjectBinder<ObjectIdAndSize>
    {
      private SqlColumnBinder m_objectId = new SqlColumnBinder("ObjectId");
      private SqlColumnBinder m_length = new SqlColumnBinder("Length");

      protected override ObjectIdAndSize Bind() => new ObjectIdAndSize(this.m_objectId.GetSha1Id((IDataReader) this.Reader), this.m_length.GetInt64((IDataReader) this.Reader));
    }

    private class KnownFileBinder : ObjectBinder<KeyValuePair<string, KnownFile>>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_type = new SqlColumnBinder("Type");
      private SqlColumnBinder m_createdDate = new SqlColumnBinder("CreatedDate");

      protected override KeyValuePair<string, KnownFile> Bind() => new KeyValuePair<string, KnownFile>(this.m_name.GetString((IDataReader) this.Reader, false), new KnownFile((KnownFileType) this.m_type.GetByte((IDataReader) this.Reader), this.m_createdDate.GetDateTime((IDataReader) this.Reader)));
    }

    private class KnownFileIntervalBinder : ObjectBinder<KeyValuePair<string, KnownFileIntervalData>>
    {
      private SqlColumnBinder m_name = new SqlColumnBinder("Name");
      private SqlColumnBinder m_type = new SqlColumnBinder("Type");
      private SqlColumnBinder m_intervals = new SqlColumnBinder("IntervalsFileNotInUse");

      protected override KeyValuePair<string, KnownFileIntervalData> Bind() => new KeyValuePair<string, KnownFileIntervalData>(this.m_name.GetString((IDataReader) this.Reader, false), new KnownFileIntervalData((KnownFileType) this.m_type.GetByte((IDataReader) this.Reader), this.m_intervals.GetInt32((IDataReader) this.Reader)));
    }

    private class MinimalGlobalRepoKeyBinder : ObjectBinder<MinimalGlobalRepoKey>
    {
      private SqlColumnBinder m_collectionId = new SqlColumnBinder("CollectionId");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");

      protected override MinimalGlobalRepoKey Bind() => new MinimalGlobalRepoKey(this.m_collectionId.GetGuid((IDataReader) this.Reader), this.m_repositoryId.GetGuid((IDataReader) this.Reader));
    }
  }
}
