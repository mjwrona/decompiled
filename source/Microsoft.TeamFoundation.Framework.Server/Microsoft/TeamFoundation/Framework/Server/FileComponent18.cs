// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent18
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent18 : FileComponent17
  {
    public override Guid StoreDelta(
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      ArraySegment<byte> content)
    {
      this.PrepareStoredProcedure("prc_StoreDelta");
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
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
      this.BindInt("@newFileId", (int) newFileId.FileId);
      this.BindInt("@newDataspaceId", this.GetDataspaceId(newFileId));
      this.BindInt("@oldFileId", (int) oldFileId.FileId);
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
      this.BindInt("@lastFileId", (int) lastFileId);
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime2("@startDate", startDate);
      this.BindDateTime2("@endDate", endDate);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      SqlColumnBinder FileId = new SqlColumnBinder("FileId");
      SqlColumnBinder DataspaceId = new SqlColumnBinder("DataspaceId");
      resultCollection.AddBinder<FileIdentifier>((ObjectBinder<FileIdentifier>) new SimpleObjectBinder<FileIdentifier>((System.Func<IDataReader, FileIdentifier>) (reader => this.GetFileIdentifier((long) FileId.GetInt32(reader), DataspaceId.GetInt32(reader)))));
      return resultCollection;
    }

    public override void RevertDelta(
      Guid resourceId,
      long compressedLength,
      RemoteStoreId remoteStoreId)
    {
      this.PrepareStoredProcedure("prc_RevertDeltaFromUploadedFile");
      this.BindGuid("@resourceId", resourceId);
      this.BindLong("@compressedLength", compressedLength);
      this.BindByte("@remoteStoreId", (byte) remoteStoreId);
      this.ExecuteNonQuery();
    }
  }
}
