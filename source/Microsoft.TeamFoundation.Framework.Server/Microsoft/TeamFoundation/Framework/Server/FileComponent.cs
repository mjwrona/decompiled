// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[25]
    {
      (IComponentCreator) new ComponentCreator<FileComponent>(1, true),
      (IComponentCreator) new ComponentCreator<FileComponent2>(2),
      (IComponentCreator) new ComponentCreator<FileComponent3>(3),
      (IComponentCreator) new ComponentCreator<FileComponent4>(4),
      (IComponentCreator) new ComponentCreator<FileComponent5>(5),
      (IComponentCreator) new ComponentCreator<FileComponent6>(6),
      (IComponentCreator) new ComponentCreator<FileComponent7>(7),
      (IComponentCreator) new ComponentCreator<FileComponent8>(8),
      (IComponentCreator) new ComponentCreator<FileComponent9>(9),
      (IComponentCreator) new ComponentCreator<FileComponent10>(10),
      (IComponentCreator) new ComponentCreator<FileComponent11>(11),
      (IComponentCreator) new ComponentCreator<FileComponent12>(12),
      (IComponentCreator) new ComponentCreator<FileComponent13>(13),
      (IComponentCreator) new ComponentCreator<FileComponent14>(14),
      (IComponentCreator) new ComponentCreator<FileComponent15>(15),
      (IComponentCreator) new ComponentCreator<FileComponent16>(16),
      (IComponentCreator) new ComponentCreator<FileComponent17>(17),
      (IComponentCreator) new ComponentCreator<FileComponent18>(18),
      (IComponentCreator) new ComponentCreator<FileComponent19>(19),
      (IComponentCreator) new ComponentCreator<FileComponent1370>(1370),
      (IComponentCreator) new ComponentCreator<FileComponent1371>(1371),
      (IComponentCreator) new ComponentCreator<FileComponent1470>(1470),
      (IComponentCreator) new ComponentCreator<FileComponent1680>(1680),
      (IComponentCreator) new ComponentCreator<FileComponent1690>(1690),
      (IComponentCreator) new ComponentCreator<FileComponent1740>(1740)
    }, "File");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static FileComponent()
    {
      FileComponent.s_sqlExceptionFactories.Add(500121, new SqlExceptionFactory(typeof (FileIdNotFoundException)));
      FileComponent.s_sqlExceptionFactories.Add(800056, new SqlExceptionFactory(typeof (DuplicateFileNameException)));
      FileComponent.s_sqlExceptionFactories.Add(800057, new SqlExceptionFactory(typeof (FileAlreadyUploadedException)));
      FileComponent.s_sqlExceptionFactories.Add(800059, new SqlExceptionFactory(typeof (UnknownMigrationOwnerException)));
      FileComponent.s_sqlExceptionFactories.Add(800080, new SqlExceptionFactory(typeof (CleanupJobInProgressException)));
      FileComponent.s_sqlExceptionFactories.Add(800106, new SqlExceptionFactory(typeof (DuplicateFileIdDataspaceException)));
      FileComponent.s_sqlExceptionFactories.Add(500029, new SqlExceptionFactory(typeof (IncompleteUploadException)));
      FileComponent.s_sqlExceptionFactories.Add(500233, new SqlExceptionFactory(typeof (ResourceIdAlreadyExistsException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) FileComponent.s_sqlExceptionFactories;

    public virtual void DeleteOrphanPendingUploadFiles(
      IEnumerable<KeyValuePair<int, int>> filesToDelete,
      bool reuseFileIdSecondaryRange)
    {
    }

    public virtual List<Tuple<int, int, Guid>> QueryOrphanPendingUploadFiles(TimeSpan retentionDate) => new List<Tuple<int, int, Guid>>();

    public virtual void UpdatePendingUploadFileDate(long fileId, int dataspaceId, DateTime newDate)
    {
    }

    protected virtual void CommitFile(
      long fileId,
      Guid dataspaceIdentifier,
      byte[] hashValue,
      long fileLength)
    {
      this.PrepareStoredProcedure("prc_CommitFile");
      this.BindInt("@fileId", (int) fileId);
      this.BindLong("@fileLength", fileLength);
      this.BindInt("@relatedFileId", 0);
      this.BindLong("@maxPatchableSize", 4194304L);
      this.BindInt("@maxRetryCount", 5);
      this.ExecuteNonQuery();
    }

    public virtual void CommitSingleFile(
      long fileId,
      Guid dataspaceIdentifier,
      byte[] hashValue,
      long fileLength)
    {
      this.CommitFile(fileId, dataspaceIdentifier, hashValue, fileLength);
    }

    public virtual void DeleteUnusedFiles(
      int retentionPeriodInDays,
      bool reuseFileIdSecondaryRange,
      int chunkSize = 100)
    {
      this.PrepareStoredProcedure("prc_DeleteUnusedFiles", 0);
      this.BindInt("@retentionPeriodInDays", -retentionPeriodInDays);
      this.BindInt("@chunkSize", chunkSize);
      this.ExecuteNonQuery();
    }

    public virtual void CreatePendingDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      int retryCount)
    {
      this.PrepareStoredProcedure("prc_CreatePendingDelta");
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
      this.BindInt("@retryCount", retryCount);
      this.ExecuteNonQuery();
    }

    public virtual SaveFileResult SaveFile(
      long fileId,
      Guid dataspaceIdentifier,
      OwnerId ownerId,
      string fileName,
      byte[] clientContentId,
      byte[] computedContentId,
      CompressionType compressionType,
      RemoteStoreId remoteStoreId,
      byte[] hashValue,
      long fileLength,
      long compressedLength,
      byte[] md5Context,
      long md5ByteCount,
      long offsetFrom,
      byte[] contentBlock,
      int contentBlockLength,
      bool overwrite,
      bool useSecondaryFileIdRange,
      bool reuseFileIdSecondaryRange)
    {
      this.PrepareStoredProcedure("prc_SaveFile", 3600);
      this.BindInt("@fileId", (int) fileId);
      this.BindByte("@ownerId", (byte) ownerId);
      this.BindString("@fileName", fileName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@compressionType", (byte) compressionType);
      this.BindByte("@remoteStoreId", (byte) remoteStoreId);
      this.BindBinary("@hashValue", hashValue, 16, SqlDbType.Binary);
      this.BindLong("@fileLength", fileLength);
      this.BindLong("@compressedLength", compressedLength);
      this.BindInt("@relatedFileId", 0);
      this.BindLong("@offsetFrom", offsetFrom);
      this.BindLong("@offsetTo", offsetFrom + (long) contentBlockLength);
      this.BindBinary("@contentBlock", contentBlock, contentBlockLength, SqlDbType.VarBinary);
      this.BindBoolean("@overwrite", overwrite);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<SaveFileResult>((ObjectBinder<SaveFileResult>) new FileComponent.SaveFileColumns());
      return resultCollection.GetCurrent<SaveFileResult>().Items[0];
    }

    public TeamFoundationFile RetrieveStatistics(FileIdentifier fileId)
    {
      using (TeamFoundationFileSet foundationFileSet = this.RetrieveFile(fileId, false))
        return foundationFileSet.Metadata;
    }

    internal virtual TeamFoundationFileSet RetrievePendingFile(long fileId) => throw new NotSupportedException();

    internal virtual TeamFoundationFileSet RetrieveFile(
      FileIdentifier fileId,
      bool includeContent = true,
      bool failOnDelete = true)
    {
      if (fileId.FileId == 0L)
        throw new FileIdNotFoundException(fileId.FileId);
      this.PrepareStoredProcedure("prc_RetrieveFile", 3600);
      this.BindInt("@fileId", (int) fileId.FileId);
      this.BindBoolean("@includeContent", includeContent);
      this.BindBoolean("@failOnDelete", failOnDelete);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      return new TeamFoundationFileSet(result);
    }

    internal virtual TeamFoundationFileSet RetrieveFile(
      ObjectBinder<TeamFoundationFile> binder,
      FileIdentifier fileId,
      bool includeContent = true,
      bool failOnDelete = true)
    {
      throw new NotImplementedException();
    }

    internal virtual TeamFoundationFileSet RetrieveFile(
      Guid resourceId,
      bool includeContent = true,
      bool failOnDelete = true)
    {
      throw new NotImplementedException();
    }

    public virtual void DeleteFile(FileIdentifier fileId)
    {
      this.PrepareStoredProcedure("prc_DeleteFile");
      this.BindInt("@fileId", (int) fileId.FileId);
      this.ExecuteNonQuery();
    }

    public virtual void SoftDeleteFile(FileIdentifier fileId) => throw new NotImplementedException();

    public virtual void SaveDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      byte[] content,
      int count,
      RemoteStoreId remoteStoreId)
    {
      this.PrepareStoredProcedure("prc_SaveDelta");
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
      this.BindByte("@remoteStoreId", (byte) remoteStoreId);
      this.BindBinary("@contentBlock", content, count, SqlDbType.VarBinary);
      this.ExecuteNonQuery();
    }

    public virtual Guid StoreDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      ArraySegment<byte> content)
    {
      throw new NotImplementedException();
    }

    public virtual void SaveDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      Guid resourceId)
    {
      throw new NotImplementedException();
    }

    public virtual void RevertDelta(
      Guid resourceId,
      long compressedLength,
      RemoteStoreId remoteStoreId)
    {
      throw new NotImplementedException();
    }

    public List<Tuple<int, int, int>> QueryPendingDeltas(int maxNumberOfPendingDeltas)
    {
      this.PrepareStoredProcedure("prc_QueryPendingDeltas");
      this.BindInt("@numberOfDeltas", maxNumberOfPendingDeltas);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<int, int, int>>((ObjectBinder<Tuple<int, int, int>>) new FileComponent.QueryPendingDeltasColumns());
      return resultCollection.GetCurrent<Tuple<int, int, int>>().Items;
    }

    public virtual int DecrementDeltaRetryCount(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      int maxDeltaChainLength)
    {
      this.PrepareStoredProcedure("prc_SetDeltaRetryCount");
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
      this.BindInt("@maxDeltaChainLength", maxDeltaChainLength);
      object obj = this.ExecuteScalar();
      return obj != null ? (int) obj : 0;
    }

    public virtual void DeletePendingDelta(FileIdentifier newFileId, FileIdentifier oldFileId)
    {
      this.PrepareStoredProcedure("prc_DeletePendingDelta");
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
      this.ExecuteNonQuery();
    }

    public ResultCollection QueryFiles(OwnerId ownerId, string pattern)
    {
      this.PrepareStoredProcedure("prc_QueryFiles", 3600);
      this.BindByte("@ownerId", (byte) ownerId);
      this.BindString("@pattern", pattern, -1, true, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<FileStatistics>((ObjectBinder<FileStatistics>) this.CreateFileStatisticsColumns());
      return resultCollection;
    }

    public int GetFileIdFromFileName(OwnerId ownerId, string fileName)
    {
      this.PrepareStoredProcedure("prc_GetFileIdFromFileName");
      this.BindByte("@ownerId", (byte) ownerId);
      this.BindString("@fileName", fileName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      object obj = this.ExecuteScalar();
      return obj == null ? 0 : (int) obj;
    }

    public void DeleteNamedFiles(OwnerId ownerId, IEnumerable<string> fileNames)
    {
      this.PrepareStoredProcedure("prc_DeleteNamedFiles");
      this.BindByte("@ownerId", (byte) ownerId);
      this.BindStringTable("@fileNames", fileNames);
      this.ExecuteNonQuery();
    }

    public virtual void HardDeleteFiles(IEnumerable<Guid> filesToDelete) => throw new NotImplementedException();

    public virtual void DeleteFiles(IEnumerable<FileIdentifier> filesToDelete)
    {
      foreach (FileIdentifier fileId in filesToDelete)
        this.DeleteFile(fileId);
    }

    public virtual List<Guid> QueryUnusedFiles(int maxNumberOfFiles, int retentionPeriodInDays) => throw new NotImplementedException();

    public virtual void RenameFile(long fileId, string newFileName) => throw new NotImplementedException();

    public virtual ResultCollection QueryAllFiles(
      RemoteStoreId remoteStoreId,
      Guid lastResourceId,
      int batchSize)
    {
      throw new NotImplementedException();
    }

    internal virtual ResultCollection QueryAllFiles(
      ObjectBinder<FileStatistics> binder,
      RemoteStoreId remoteStoreId,
      Guid lastResourceId,
      int batchSize)
    {
      throw new NotImplementedException();
    }

    public virtual ResultCollection QueryAllFilesInInterval(
      long lastFileId,
      int batchSize,
      DateTime startDate,
      DateTime endDate)
    {
      throw new NotImplementedException();
    }

    public virtual long GetFullContentFileSize()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetFullContentFileSize.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      return (long) this.ExecuteScalar();
    }

    public virtual IDictionary<long, long> QueryFileSizes(
      IEnumerable<long> fileIds,
      Guid dataspaceId,
      OwnerId ownerId)
    {
      return (IDictionary<long, long>) new Dictionary<long, long>();
    }

    internal virtual List<FileComponent.FileUtilizationByOwner> GetFileUtilizationByOwner()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_GetFileUtilizationByOwner.sql");
      this.PrepareSqlBatch(resourceAsString.Length, 3600);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), nameof (GetFileUtilizationByOwner), this.RequestContext);
      resultCollection.AddBinder<FileComponent.FileUtilizationByOwner>((ObjectBinder<FileComponent.FileUtilizationByOwner>) new FileComponent.FileUtilizationByOwnerColumns());
      return resultCollection.GetCurrent<FileComponent.FileUtilizationByOwner>().Items;
    }

    public virtual void SwapFileResources(long fileA, long fileB, bool deleteB) => throw new NotImplementedException();

    public virtual void CreateDummyFile() => throw new NotImplementedException();

    public virtual void SeedFileTable()
    {
    }

    public virtual void UpdateSequenceFileId(long sequenceNewFileId, bool useSecondaryFileIdRange)
    {
    }

    public virtual int GetMaxFileId() => 1023;

    [Obsolete("Use GetFileIdUsage instead")]
    public virtual (int, int) GetMinMaxFileId() => (1023, 1023);

    public virtual FileIdUsage GetFileIdUsage() => new FileIdUsage()
    {
      MaxNegativeId = 1023,
      MaxPositiveId = 1023,
      ReusableNegativeIds = 0
    };

    protected virtual FileBinder CreateFileBinder() => new FileBinder();

    protected virtual FileComponent.FileStatisticsColumns CreateFileStatisticsColumns() => new FileComponent.FileStatisticsColumns();

    internal class FileUtilizationByOwnerColumns : ObjectBinder<FileComponent.FileUtilizationByOwner>
    {
      protected SqlColumnBinder OwnerColumn = new SqlColumnBinder("Owner");
      protected SqlColumnBinder CompressedSizeColumn = new SqlColumnBinder("CompressedSize");

      protected override FileComponent.FileUtilizationByOwner Bind() => new FileComponent.FileUtilizationByOwner()
      {
        Owner = this.OwnerColumn.GetString((IDataReader) this.Reader, false),
        CompressedSize = this.CompressedSizeColumn.GetInt64((IDataReader) this.Reader)
      };
    }

    internal class FileUtilizationByOwner
    {
      public string Owner { get; set; }

      public long CompressedSize { get; set; }
    }

    protected class SaveFileColumns : ObjectBinder<SaveFileResult>
    {
      protected SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
      protected SqlColumnBinder ResourceIdColumn = new SqlColumnBinder("ResourceId");
      protected SqlColumnBinder IsCompleteColumn = new SqlColumnBinder("IsComplete");

      protected override SaveFileResult Bind() => new SaveFileResult()
      {
        FileId = (long) this.FileIdColumn.GetInt32((IDataReader) this.Reader),
        ResourceId = this.ResourceIdColumn.GetGuid((IDataReader) this.Reader),
        IsUploadComplete = this.IsCompleteColumn.GetBoolean((IDataReader) this.Reader)
      };
    }

    protected class SaveFileColumns2 : FileComponent.SaveFileColumns
    {
      protected override SaveFileResult Bind() => new SaveFileResult()
      {
        FileId = this.FileIdColumn.GetInt64((IDataReader) this.Reader),
        ResourceId = this.ResourceIdColumn.GetGuid((IDataReader) this.Reader),
        IsUploadComplete = this.IsCompleteColumn.GetBoolean((IDataReader) this.Reader)
      };
    }

    internal class FileStatisticsColumns : ObjectBinder<FileStatistics>
    {
      protected SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
      protected SqlColumnBinder OwnerIdColumn = new SqlColumnBinder("OwnerId");
      protected SqlColumnBinder CreationDateColumn = new SqlColumnBinder("CreationDate");
      protected SqlColumnBinder FileNameColumn = new SqlColumnBinder("FileName");
      protected SqlColumnBinder ContentTypeColumn = new SqlColumnBinder("ContentType");
      protected SqlColumnBinder CompressionTypeColumn = new SqlColumnBinder("CompressionType");
      protected SqlColumnBinder HashValueColumn = new SqlColumnBinder("HashValue");
      protected SqlColumnBinder FileLengthColumn = new SqlColumnBinder("FileLength");
      protected SqlColumnBinder CompressedLengthColumn = new SqlColumnBinder("CompressedLength");

      protected override FileStatistics Bind() => new FileStatistics()
      {
        FileId = (long) this.FileIdColumn.GetInt32((IDataReader) this.Reader),
        DataspaceIdentifier = Guid.Empty,
        OwnerId = (OwnerId) this.OwnerIdColumn.GetByte((IDataReader) this.Reader),
        CreationDate = this.CreationDateColumn.GetDateTime((IDataReader) this.Reader),
        FileName = this.FileNameColumn.GetString((IDataReader) this.Reader, true),
        ContentId = (byte[]) null,
        ContentType = (ContentType) this.ContentTypeColumn.GetByte((IDataReader) this.Reader),
        CompressionType = (CompressionType) this.CompressionTypeColumn.GetByte((IDataReader) this.Reader),
        HashValue = this.HashValueColumn.GetBytes((IDataReader) this.Reader, false),
        FileLength = this.FileLengthColumn.GetInt64((IDataReader) this.Reader),
        CompressedLength = this.CompressedLengthColumn.GetInt64((IDataReader) this.Reader)
      };
    }

    protected class QueryPendingDeltasColumns : ObjectBinder<Tuple<int, int, int>>
    {
      protected SqlColumnBinder NewFileId = new SqlColumnBinder(nameof (NewFileId));
      protected SqlColumnBinder OldFileId = new SqlColumnBinder(nameof (OldFileId));
      protected SqlColumnBinder RetryCount = new SqlColumnBinder(nameof (RetryCount));

      protected override Tuple<int, int, int> Bind() => new Tuple<int, int, int>(this.NewFileId.GetInt32((IDataReader) this.Reader), this.OldFileId.GetInt32((IDataReader) this.Reader), this.RetryCount.GetInt32((IDataReader) this.Reader));
    }
  }
}
