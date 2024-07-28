// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent9 : FileComponent8
  {
    protected override sealed void CommitFile(
      long fileId,
      Guid dataspaceIdentifier,
      byte[] hashValue,
      long fileLength)
    {
      this.PrepareStoredProcedure("prc_CommitFile", 3600);
      this.BindInt("@fileId", (int) fileId);
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
        this.BindInt("@fileId", (int) fileId);
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
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<SaveFileResult>((ObjectBinder<SaveFileResult>) new FileComponent.SaveFileColumns());
        return resultCollection.GetCurrent<SaveFileResult>().Items[0];
      }
      finally
      {
        this.DataspaceRlsEnabled = dataspaceRlsEnabled;
      }
    }

    internal override TeamFoundationFileSet RetrievePendingFile(long fileId)
    {
      this.PrepareStoredProcedure("prc_RetrievePendingFile", 3600);
      this.BindInt("@fileId", (int) fileId);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      return new TeamFoundationFileSet(result);
    }

    internal override TeamFoundationFileSet RetrieveFile(
      FileIdentifier fileIdentifier,
      bool includeContent,
      bool failOnDelete)
    {
      int fileId = (int) fileIdentifier.FileId;
      if (fileId == 0)
        throw new FileIdNotFoundException((long) fileId);
      this.PrepareStoredProcedure("prc_RetrieveFile", 3600);
      this.BindInt("@fileId", fileId);
      this.BindBoolean("@includeContent", includeContent);
      this.BindBoolean("@failOnDelete", failOnDelete);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      return new TeamFoundationFileSet(result);
    }

    public override void HardDeleteFiles(IEnumerable<Guid> filesToDelete)
    {
      this.PrepareStoredProcedure("prc_HardDeleteFiles", 3600);
      this.BindGuidTable("@filesToDelete", filesToDelete);
      this.ExecuteNonQuery();
    }

    public override List<Guid> QueryUnusedFiles(int maxNumberOfFiles, int retentionPeriodInDays)
    {
      this.PrepareStoredProcedure("prc_QueryUnusedFiles", 3600);
      this.BindInt("@chunkSize", maxNumberOfFiles);
      this.BindInt("@retentionPeriodInDays", retentionPeriodInDays);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new FileComponent9.QueryUnusedFilesColumns());
      return resultCollection.GetCurrent<Guid>().Items;
    }

    public override void CreateDummyFile()
    {
      this.PrepareStoredProcedure("prc_CreateDummyFile");
      this.ExecuteNonQuery();
    }

    public override void SeedFileTable()
    {
      this.PrepareSqlBatch("\r\n                IF EXISTS (\r\n                    SELECT  *\r\n                    FROM    sys.tables\r\n                    WHERE   name = 'tbl_FileReference'\r\n                )\r\n                BEGIN\r\n                    DECLARE @newSeed        INT\r\n\r\n                    SELECT  @newSeed = MAX(FileId)\r\n                    FROM    tbl_FileReference\r\n                    WHERE   PartitionId > 0\r\n\r\n                    IF (@newSeed IS NOT NULL)\r\n                    BEGIN\r\n                        DECLARE @currentSeed INT\r\n\r\n                        SELECT  @currentSeed = IDENT_CURRENT('dbo.tbl_FilePendingUpload')\r\n\r\n                        -- Insert and then delete a 'dummy' record in tbl_FilePendingUpload with IDENTITY_INSERT using the new seed\r\n                        -- The identity column seed is always the largest value ever previously inserted (with IDENTITY_INSERT or normally)\r\n                        IF (@currentSeed < @newSeed)\r\n                        BEGIN\r\n                            BEGIN TRAN\r\n\r\n                            SET IDENTITY_INSERT tbl_FilePendingUpload ON\r\n\r\n                            INSERT  tbl_FilePendingUpload\r\n                                    (PartitionId, FileId, DataspaceId, OwnerId, CreationDate, ContentType, CompressionType, RemoteStoreId, HashValue, FileLength, CompressedLength)\r\n                            VALUES  (0, @newSeed, 0, 255, GETUTCDATE(), 1, 1, 0, 0x, 0, 0)\r\n\r\n                            SET IDENTITY_INSERT tbl_FilePendingUpload OFF\r\n\r\n                            DELETE  tbl_FilePendingUpload\r\n                            WHERE   PartitionId = 0\r\n                                    AND FileId = @newSeed\r\n                                    AND DataspaceId = 0\r\n\r\n                            COMMIT TRAN\r\n                        END\r\n                    END\r\n                END".Length, false);
      this.AddStatement("\r\n                IF EXISTS (\r\n                    SELECT  *\r\n                    FROM    sys.tables\r\n                    WHERE   name = 'tbl_FileReference'\r\n                )\r\n                BEGIN\r\n                    DECLARE @newSeed        INT\r\n\r\n                    SELECT  @newSeed = MAX(FileId)\r\n                    FROM    tbl_FileReference\r\n                    WHERE   PartitionId > 0\r\n\r\n                    IF (@newSeed IS NOT NULL)\r\n                    BEGIN\r\n                        DECLARE @currentSeed INT\r\n\r\n                        SELECT  @currentSeed = IDENT_CURRENT('dbo.tbl_FilePendingUpload')\r\n\r\n                        -- Insert and then delete a 'dummy' record in tbl_FilePendingUpload with IDENTITY_INSERT using the new seed\r\n                        -- The identity column seed is always the largest value ever previously inserted (with IDENTITY_INSERT or normally)\r\n                        IF (@currentSeed < @newSeed)\r\n                        BEGIN\r\n                            BEGIN TRAN\r\n\r\n                            SET IDENTITY_INSERT tbl_FilePendingUpload ON\r\n\r\n                            INSERT  tbl_FilePendingUpload\r\n                                    (PartitionId, FileId, DataspaceId, OwnerId, CreationDate, ContentType, CompressionType, RemoteStoreId, HashValue, FileLength, CompressedLength)\r\n                            VALUES  (0, @newSeed, 0, 255, GETUTCDATE(), 1, 1, 0, 0x, 0, 0)\r\n\r\n                            SET IDENTITY_INSERT tbl_FilePendingUpload OFF\r\n\r\n                            DELETE  tbl_FilePendingUpload\r\n                            WHERE   PartitionId = 0\r\n                                    AND FileId = @newSeed\r\n                                    AND DataspaceId = 0\r\n\r\n                            COMMIT TRAN\r\n                        END\r\n                    END\r\n                END");
      this.ExecuteNonQuery();
    }

    protected override FileBinder CreateFileBinder() => (FileBinder) new FileBinder2((FileComponent) this);

    protected override FileComponent.FileStatisticsColumns CreateFileStatisticsColumns() => (FileComponent.FileStatisticsColumns) new FileComponent9.FileStatisticsColumns2((FileComponent) this);

    protected class QueryUnusedFilesColumns : ObjectBinder<Guid>
    {
      protected SqlColumnBinder ResourceId = new SqlColumnBinder(nameof (ResourceId));

      protected override Guid Bind() => this.ResourceId.GetGuid((IDataReader) this.Reader);
    }

    protected class FileStatisticsColumns2 : FileComponent.FileStatisticsColumns
    {
      protected SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      protected SqlColumnBinder ResourceId = new SqlColumnBinder(nameof (ResourceId));
      protected SqlColumnBinder ContentId = new SqlColumnBinder(nameof (ContentId));
      private FileComponent m_fileComponent;

      public FileStatisticsColumns2(FileComponent fileComponent) => this.m_fileComponent = fileComponent;

      protected override FileStatistics Bind()
      {
        FileStatistics fileStatistics = base.Bind();
        fileStatistics.DataspaceIdentifier = this.m_fileComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32((IDataReader) this.Reader));
        fileStatistics.ResourceId = this.ResourceId.GetGuid((IDataReader) this.Reader);
        fileStatistics.ContentId = this.ContentId.GetBytes((IDataReader) this.Reader, true);
        return fileStatistics;
      }
    }
  }
}
