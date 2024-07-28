// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent12
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent12 : FileComponent11
  {
    public FileIdentifier GetFileIdentifier(long fileId, int dataspaceId)
    {
      Dataspace dataspace = this.GetDataspace(dataspaceId);
      return new FileIdentifier(fileId, dataspace.DataspaceIdentifier, TeamFoundationFileService.GetOwnerIdFromCategory(dataspace.DataspaceCategory));
    }

    public FileIdentifier GetFileIdentifier(long fileId, int dataspaceId, OwnerId ownerId) => new FileIdentifier(fileId, this.GetDataspaceIdentifier(dataspaceId), ownerId);

    public override void SaveDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      byte[] content,
      int count,
      RemoteStoreId remoteStoreId)
    {
      this.PrepareStoredProcedure("prc_SaveDelta", 3600);
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.BindByte("@remoteStoreId", (byte) remoteStoreId);
      this.BindBinary("@contentBlock", content, count, SqlDbType.VarBinary);
      this.ExecuteNonQuery();
    }

    public List<Tuple<FileIdentifier, FileIdentifier, int>> QueryPendingDeltas2(
      int maxNumberOfPendingDeltas)
    {
      this.PrepareStoredProcedure("prc_QueryPendingDeltas", 3600);
      this.BindInt("@numberOfDeltas", maxNumberOfPendingDeltas);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<FileIdentifier, FileIdentifier, int>>((ObjectBinder<Tuple<FileIdentifier, FileIdentifier, int>>) new FileComponent12.QueryPendingDeltasColumns2(this));
      return resultCollection.GetCurrent<Tuple<FileIdentifier, FileIdentifier, int>>().Items;
    }

    public override int DecrementDeltaRetryCount(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      int maxDeltaChainLength)
    {
      this.PrepareStoredProcedure("prc_SetDeltaRetryCount");
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.BindInt("@maxDeltaChainLength", maxDeltaChainLength);
      object obj = this.ExecuteScalar();
      return obj != null ? (int) obj : 0;
    }

    public override void DeletePendingDelta(FileIdentifier newFileId, FileIdentifier oldFileId)
    {
      this.PrepareStoredProcedure("prc_DeletePendingDelta", 3600);
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
      this.BindInt("@oldDataspaceId", this.GetDataspaceId(oldFileId));
      this.ExecuteNonQuery();
    }

    public override void CreatePendingDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      int retryCount)
    {
      this.PrepareStoredProcedure("prc_CreatePendingDelta", 3600);
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
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
      this.BindInt("@fileId", (int) fileId.FileId);
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
      this.BindInt("@fileId", (int) fileId.FileId);
      this.BindInt("@dataspaceId", this.GetDataspaceId(fileId));
      this.BindBoolean("@includeContent", includeContent);
      this.BindBoolean("@failOnDelete", failOnDelete);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>(binder);
      result.AddBinder<TeamFoundationFile>(binder);
      return new TeamFoundationFileSet(result);
    }

    protected class QueryPendingDeltasColumns2 : 
      ObjectBinder<Tuple<FileIdentifier, FileIdentifier, int>>
    {
      private FileComponent12 m_fileComponent;
      protected SqlColumnBinder NewFileId = new SqlColumnBinder(nameof (NewFileId));
      protected SqlColumnBinder NewDataspaceId = new SqlColumnBinder(nameof (NewDataspaceId));
      protected SqlColumnBinder OldFileId = new SqlColumnBinder(nameof (OldFileId));
      protected SqlColumnBinder OldDataspaceId = new SqlColumnBinder(nameof (OldDataspaceId));
      protected SqlColumnBinder RetryCount = new SqlColumnBinder(nameof (RetryCount));

      public QueryPendingDeltasColumns2(FileComponent12 fileComponent) => this.m_fileComponent = fileComponent;

      protected override Tuple<FileIdentifier, FileIdentifier, int> Bind() => new Tuple<FileIdentifier, FileIdentifier, int>(this.m_fileComponent.GetFileIdentifier((long) this.NewFileId.GetInt32((IDataReader) this.Reader), this.NewDataspaceId.GetInt32((IDataReader) this.Reader)), this.m_fileComponent.GetFileIdentifier((long) this.OldFileId.GetInt32((IDataReader) this.Reader), this.OldDataspaceId.GetInt32((IDataReader) this.Reader)), this.RetryCount.GetInt32((IDataReader) this.Reader));
    }
  }
}
