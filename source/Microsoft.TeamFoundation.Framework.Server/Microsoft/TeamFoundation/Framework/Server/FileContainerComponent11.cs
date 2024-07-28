// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent11 : FileContainerComponent10
  {
    private static readonly SqlMetaData[] typ_ContainerItemTable3 = new SqlMetaData[10]
    {
      new SqlMetaData("Path", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ItemType", SqlDbType.TinyInt),
      new SqlMetaData("FileLength", SqlDbType.BigInt),
      new SqlMetaData("FileHash", SqlDbType.Binary, 16L),
      new SqlMetaData("FileEncoding", SqlDbType.Int),
      new SqlMetaData("FileType", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("FileId", SqlDbType.BigInt),
      new SqlMetaData("ContentId", SqlDbType.VarBinary, 128L),
      new SqlMetaData("ArtifactId", SqlDbType.BigInt)
    };

    public override void UpdateItemStatus(
      long containerId,
      string path,
      int fileId,
      ContainerItemStatus status,
      Guid? dataspaceIdentifier,
      long fileLength = -1,
      byte[] contentId = null,
      long? artifactId = null)
    {
      this.TraceEnter(0, nameof (UpdateItemStatus));
      if (!string.IsNullOrEmpty(path))
      {
        string databasePath = DBPath.UserToDatabasePath(path, true, true);
        this.PrepareStoredProcedure("prc_UpdateContainerItemStatus");
        this.BindLong("@containerId", containerId);
        this.BindString("@path", databasePath, -1, false, SqlDbType.NVarChar);
        this.BindInt("@fileId", fileId);
        this.BindByte("@status", (byte) status);
        this.BindGuid("@createdBy", this.Author);
        if (contentId != null)
          this.BindBinary("@contentId", contentId, SqlDbType.Binary);
        if (fileLength > -1L)
          this.BindLong("@fileLength", fileLength);
        if (artifactId.HasValue)
          this.BindLong("@artifactId", artifactId.Value);
        this.BindDataspace(dataspaceIdentifier);
        this.ExecuteNonQuery();
      }
      this.TraceLeave(0, nameof (UpdateItemStatus));
    }

    public override ContainerItemBlobReference AddBlobReference(
      string artifactHash,
      BlobCompressionType compressionType,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (AddBlobReference));
      this.PrepareStoredProcedure("prc_AddBlobReference");
      this.BindDataspace(dataspaceIdentifier);
      this.BindString("@artifactHash", artifactHash, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindByte("@compressionType", (byte) compressionType);
      ContainerItemBlobReference itemBlobReference;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerItemBlobReference>((ObjectBinder<ContainerItemBlobReference>) this.GetContainerItemBlobReferenceBinder());
        itemBlobReference = resultCollection.GetCurrent<ContainerItemBlobReference>().FirstOrDefault<ContainerItemBlobReference>();
      }
      this.TraceLeave(0, nameof (AddBlobReference));
      return itemBlobReference;
    }

    public override ContainerItemBlobReference GetBlobReference(
      long artifactId,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (GetBlobReference));
      this.PrepareStoredProcedure("prc_GetBlobReference");
      this.BindDataspace(dataspaceIdentifier);
      this.BindLong("@artifactId", artifactId);
      ContainerItemBlobReference blobReference;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerItemBlobReference>((ObjectBinder<ContainerItemBlobReference>) this.GetContainerItemBlobReferenceBinder());
        blobReference = resultCollection.GetCurrent<ContainerItemBlobReference>().FirstOrDefault<ContainerItemBlobReference>();
      }
      this.TraceLeave(0, nameof (GetBlobReference));
      return blobReference;
    }

    internal override ContainerItemBinder GetFileContainerItemBinder() => (ContainerItemBinder) new ContainerItemBinder3((TeamFoundationSqlResourceComponent) this);

    internal override FileContainerCleanupStatsBinder GetFileContainerCleanupStatsBinder() => (FileContainerCleanupStatsBinder) new FileContainerCleanupStatsBinder2();

    protected override SqlParameter BindContainerItemTable(
      string parameterName,
      IEnumerable<FileContainerItem> rows)
    {
      rows = rows ?? Enumerable.Empty<FileContainerItem>();
      return this.BindTable(parameterName, "typ_ContainerItemTable3", this.BindContainerItemRows(rows));
    }

    private IEnumerable<SqlDataRecord> BindContainerItemRows(IEnumerable<FileContainerItem> rows)
    {
      foreach (FileContainerItem row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FileContainerComponent11.typ_ContainerItemTable3);
        sqlDataRecord.SetString(0, DBPath.UserToDatabasePath(row.Path, true, true));
        sqlDataRecord.SetByte(1, (byte) row.ItemType);
        switch (row.ItemType)
        {
          case ContainerItemType.Folder:
            sqlDataRecord.SetDBNull(2);
            sqlDataRecord.SetDBNull(3);
            sqlDataRecord.SetDBNull(4);
            sqlDataRecord.SetDBNull(5);
            sqlDataRecord.SetDBNull(6);
            sqlDataRecord.SetDBNull(7);
            sqlDataRecord.SetDBNull(8);
            sqlDataRecord.SetDBNull(9);
            break;
          case ContainerItemType.File:
            sqlDataRecord.SetInt64(2, row.FileLength);
            if (row.FileHash != null)
              sqlDataRecord.SetBytes(3, 0L, row.FileHash, 0, row.FileHash.Length);
            else
              sqlDataRecord.SetDBNull(3);
            sqlDataRecord.SetInt32(4, row.FileEncoding);
            sqlDataRecord.SetInt32(5, row.FileType);
            if (row.Status > (ContainerItemStatus) 0)
              sqlDataRecord.SetByte(6, (byte) row.Status);
            else
              sqlDataRecord.SetDBNull(6);
            if (row.FileId > 0)
              sqlDataRecord.SetInt64(7, (long) row.FileId);
            else
              sqlDataRecord.SetDBNull(7);
            if (row.ContentId != null)
              sqlDataRecord.SetBytes(8, 0L, row.ContentId, 0, row.ContentId.Length);
            else
              sqlDataRecord.SetDBNull(8);
            if (row.ArtifactId.HasValue)
            {
              sqlDataRecord.SetInt64(9, row.ArtifactId.Value);
              break;
            }
            sqlDataRecord.SetDBNull(9);
            break;
          default:
            throw new ArgumentException("ItemType");
        }
        yield return sqlDataRecord;
      }
    }
  }
}
