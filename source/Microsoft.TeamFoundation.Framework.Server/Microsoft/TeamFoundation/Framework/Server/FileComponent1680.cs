// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent1680
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent1680 : FileComponent1470
  {
    public override QueryMD5ContextResult QueryMD5Context(OwnerId ownerId, long fileId)
    {
      this.PrepareStoredProcedure("prc_QueryMD5Context");
      this.BindLong("@fileId", fileId);
      this.BindByte("@ownerId", (byte) ownerId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueryMD5ContextResult>((ObjectBinder<QueryMD5ContextResult>) new FileComponent8.QueryMD5ContextColumns());
      return resultCollection.GetCurrent<QueryMD5ContextResult>().Items.FirstOrDefault<QueryMD5ContextResult>();
    }

    public override void RenameFile(long fileId, string newFileName)
    {
      this.PrepareStoredProcedure("prc_RenameFile");
      this.BindLong("@fileId", fileId);
      this.BindString("@newFileName", newFileName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override void SwapFileResources(long fileA, long fileB, bool deleteB)
    {
      this.PrepareStoredProcedure("prc_SwapFileResources");
      this.BindLong("@fileA", fileA);
      this.BindLong("@fileB", fileB);
      this.BindBoolean("@deleteB", deleteB);
      this.ExecuteNonQuery();
    }

    internal override TeamFoundationFileSet RetrievePendingFile(long fileId)
    {
      this.PrepareStoredProcedure("prc_RetrievePendingFile", 3600);
      this.BindLong("@fileId", fileId);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      return new TeamFoundationFileSet(result);
    }

    public override void DeleteFile(FileIdentifier fileId)
    {
      this.PrepareStoredProcedure("prc_DeleteFile", 3600);
      this.BindLong("@fileId", fileId.FileId);
      if (!fileId.DataspaceIdentifier.HasValue)
        this.BindInt("@dataspaceId", 0);
      else
        this.BindInt("@dataspaceId", this.GetDataspaceId(fileId.DataspaceIdentifier.Value));
      this.ExecuteNonQuery();
    }

    public override void DeleteFiles(IEnumerable<FileIdentifier> filesToDelete)
    {
      this.PrepareStoredProcedure("prc_DeleteFiles", 3600);
      this.BindKeyValuePairInt64Int32Table("@filesToDelete", filesToDelete.Select<FileIdentifier, KeyValuePair<long, int>>((System.Func<FileIdentifier, KeyValuePair<long, int>>) (f => new KeyValuePair<long, int>(f.FileId, this.GetDataspaceId(f)))));
      this.ExecuteNonQuery();
    }

    public override void SaveDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      byte[] content,
      int count,
      RemoteStoreId remoteStoreId)
    {
      this.PrepareStoredProcedure("prc_SaveDelta", 3600);
      this.BindLong("@newFileId", newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindLong("@oldFileId", oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.BindByte("@remoteStoreId", (byte) remoteStoreId);
      this.BindBinary("@contentBlock", content, count, SqlDbType.VarBinary);
      this.ExecuteNonQuery();
    }

    public override int DecrementDeltaRetryCount(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      int maxDeltaChainLength)
    {
      this.PrepareStoredProcedure("prc_SetDeltaRetryCount");
      this.BindLong("@newFileId", newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindLong("@oldFileId", oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.BindInt("@maxDeltaChainLength", maxDeltaChainLength);
      object obj = this.ExecuteScalar();
      return obj != null ? (int) obj : 0;
    }

    public override void DeletePendingDelta(FileIdentifier newFileId, FileIdentifier oldFileId)
    {
      this.PrepareStoredProcedure("prc_DeletePendingDelta", 3600);
      this.BindLong("@newFileId", newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindLong("@oldFileId", oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.ExecuteNonQuery();
    }

    public override void CreatePendingDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      int retryCount)
    {
      this.PrepareStoredProcedure("prc_CreatePendingDelta", 3600);
      this.BindLong("@newFileId", newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindLong("@oldFileId", oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.BindInt("@retryCount", retryCount);
      this.ExecuteNonQuery();
    }

    internal override TeamFoundationFileSet RetrieveFile(
      FileIdentifier fileId,
      bool includeContent = true,
      bool failOnDelete = true)
    {
      if (fileId.FileId == 0L)
        throw new FileIdNotFoundException(fileId.FileId);
      this.PrepareStoredProcedure("prc_RetrieveFile", 300);
      this.BindLong("@fileId", fileId.FileId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(fileId));
      this.BindBoolean("@includeContent", includeContent);
      this.BindBoolean("@failOnDelete", failOnDelete);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      return new TeamFoundationFileSet(result);
    }

    internal override TeamFoundationFileSet RetrieveFile(
      ObjectBinder<TeamFoundationFile> binder,
      FileIdentifier fileId,
      bool includeContent = true,
      bool failOnDelete = true)
    {
      if (fileId.FileId == 0L)
        throw new FileIdNotFoundException(fileId.FileId);
      this.PrepareStoredProcedure("prc_RetrieveFile", 300);
      this.BindLong("@fileId", fileId.FileId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(fileId));
      this.BindBoolean("@includeContent", includeContent);
      this.BindBoolean("@failOnDelete", failOnDelete);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>(binder);
      result.AddBinder<TeamFoundationFile>(binder);
      return new TeamFoundationFileSet(result);
    }

    public override void UpdatePendingUploadFileDate(
      long fileId,
      int dataspaceId,
      DateTime newDate)
    {
      this.PrepareStoredProcedure("prc_UpdateFilePendingUploadDate");
      this.BindLong("@fileId", fileId);
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindDateTime2("@newDate", newDate);
      this.ExecuteNonQuery();
    }

    public override Guid StoreDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      ArraySegment<byte> content)
    {
      this.PrepareStoredProcedure("prc_StoreDelta");
      this.BindLong("@newFileId", newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindLong("@oldFileId", oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.BindBinary("@contentBlock", content.Array, content.Count, SqlDbType.VarBinary);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      SqlColumnBinder keyIdColumn = new SqlColumnBinder("ResourceId");
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => keyIdColumn.GetGuid(reader))));
      return resultCollection.GetCurrent<Guid>().Items.Single<Guid>();
    }

    public override void SaveDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      Guid resourceId)
    {
      this.PrepareStoredProcedure("prc_SaveDeltaFromResourceId");
      this.BindLong("@newFileId", newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindLong("@oldFileId", oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.BindGuid("@resourceId", resourceId);
      this.ExecuteNonQuery();
    }

    public override ResultCollection QueryAllFilesInInterval(
      long lastFileId,
      int batchSize,
      DateTime startDate,
      DateTime endDate)
    {
      this.PrepareStoredProcedure("prc_QueryAllFilesInInterval", 3600);
      this.BindLong("@lastFileId", lastFileId);
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime2("@startDate", startDate);
      this.BindDateTime2("@endDate", endDate);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      SqlColumnBinder FileId = new SqlColumnBinder("FileId");
      SqlColumnBinder DataspaceId = new SqlColumnBinder("DataspaceId");
      resultCollection.AddBinder<FileIdentifier>((ObjectBinder<FileIdentifier>) new SimpleObjectBinder<FileIdentifier>((System.Func<IDataReader, FileIdentifier>) (reader => this.GetFileIdentifier(FileId.GetInt64(reader), DataspaceId.GetInt32(reader)))));
      return resultCollection;
    }

    public override IDictionary<long, long> QueryFileSizes(
      IEnumerable<long> fileIds,
      Guid dataspaceIdentifier,
      OwnerId ownerId)
    {
      this.PrepareStoredProcedure("prc_QueryFileSizes");
      int dataspaceId = this.GetDataspaceId(dataspaceIdentifier, TeamFoundationFileService.GetCategoryFromOwnerId(ownerId));
      this.BindInt64Table("@fileIds", fileIds);
      this.BindInt("@dataspaceId", dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<long, long>>((ObjectBinder<Tuple<long, long>>) new FileComponent1680.FileSizesBinder2());
      return (IDictionary<long, long>) resultCollection.GetCurrent<Tuple<long, long>>().Items.ToDictionary<Tuple<long, long>, long, long>((System.Func<Tuple<long, long>, long>) (t => t.Item1), (System.Func<Tuple<long, long>, long>) (t => t.Item2));
    }

    public override void SoftDeleteFile(FileIdentifier fileId)
    {
      this.PrepareStoredProcedure("prc_DeleteFile", 3600);
      this.BindLong("@fileId", fileId.FileId);
      this.BindDateTime2("@DeletionDate", DateTime.Parse("9999-01-01"));
      if (!fileId.DataspaceIdentifier.HasValue)
        this.BindInt("@dataspaceId", 0);
      else
        this.BindInt("@dataspaceId", this.GetDataspaceId(fileId.DataspaceIdentifier.Value));
      this.ExecuteNonQuery();
    }

    public override void CommitSingleFile(
      long fileId,
      Guid dataspaceIdentifier,
      byte[] hashValue,
      long fileLength)
    {
      this.PrepareStoredProcedure("prc_CommitSingleFile", 3600);
      this.BindLong("@fileId", fileId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));
      this.BindBinary("@hashValue", hashValue, 16, SqlDbType.Binary);
      this.ExecuteNonQuery();
    }

    public override SaveFileResult SaveFile(
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
      bool dataspaceRlsEnabled = this.DataspaceRlsEnabled;
      try
      {
        this.DataspaceRlsEnabled = false;
        this.PrepareStoredProcedure("prc_SaveFile", 3600);
        this.BindLong("@fileId", fileId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));
        this.BindByte("@ownerId", (byte) ownerId);
        this.BindString("@fileName", fileName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindBinary("@clientContentId", clientContentId, 32, SqlDbType.Binary);
        this.BindBinary("@computedContentId", computedContentId, 32, SqlDbType.Binary);
        this.BindByte("@compressionType", (byte) compressionType);
        this.BindByte("@remoteStoreId", (byte) remoteStoreId);
        this.BindBinary("@hashValue", hashValue, 16, SqlDbType.Binary);
        this.BindLong("@fileLength", fileLength);
        this.BindLong("@compressedLength", compressedLength);
        if (md5Context != null)
        {
          this.BindBinary("@md5Context", md5Context, 104, SqlDbType.Binary);
          this.BindLong("@md5ByteCount", md5ByteCount);
        }
        this.BindLong("@offsetFrom", offsetFrom);
        this.BindLong("@offsetTo", offsetFrom + (long) contentBlockLength);
        this.BindBinary("@contentBlock", contentBlock, contentBlockLength, SqlDbType.VarBinary);
        this.BindBoolean("@overwrite", overwrite);
        this.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<SaveFileResult>((ObjectBinder<SaveFileResult>) new FileComponent.SaveFileColumns2());
        return resultCollection.GetCurrent<SaveFileResult>().Items[0];
      }
      finally
      {
        this.DataspaceRlsEnabled = dataspaceRlsEnabled;
      }
    }

    public override void UpdateSequenceFileId(long sequenceNewFileId, bool useSecondaryFileIdRange)
    {
      this.PrepareStoredProcedure("prc_UpdateFileSequence");
      this.BindLong("@ServiceFileId", sequenceNewFileId);
      this.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
      this.ExecuteNonQuery();
    }

    protected override FileBinder CreateFileBinder() => (FileBinder) new FileBinder3((FileComponent) this);

    internal class FileSizesBinder2 : ObjectBinder<Tuple<long, long>>
    {
      protected SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
      protected SqlColumnBinder FileLengthColumn = new SqlColumnBinder("FileLength");

      protected override Tuple<long, long> Bind() => new Tuple<long, long>(this.FileIdColumn.GetInt64((IDataReader) this.Reader), this.FileLengthColumn.GetInt64((IDataReader) this.Reader));
    }
  }
}
