// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent1470
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent1470 : FileComponent1371
  {
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
        this.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<SaveFileResult>((ObjectBinder<SaveFileResult>) new FileComponent.SaveFileColumns());
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
      this.BindInt("@ServiceFileId", (int) sequenceNewFileId);
      this.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
      this.ExecuteNonQuery();
    }

    [Obsolete]
    public override (int, int) GetMinMaxFileId()
    {
      this.PrepareStoredProcedure("prc_GetMinMaxFileId");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<int, int>>((ObjectBinder<Tuple<int, int>>) new FileComponent1470.MinMaxBinder());
      int num1;
      int num2;
      resultCollection.GetCurrent<Tuple<int, int>>().Single<Tuple<int, int>>().Deconstruct<int, int>(out num1, out num2);
      return (num1, num2);
    }

    public override FileIdUsage GetFileIdUsage()
    {
      this.PrepareStoredProcedure("prc_GetMinMaxFileId");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<int, int>>((ObjectBinder<Tuple<int, int>>) new FileComponent1470.MinMaxBinder());
      int num1;
      int num2;
      resultCollection.GetCurrent<Tuple<int, int>>().Single<Tuple<int, int>>().Deconstruct<int, int>(out num1, out num2);
      int num3 = num1;
      int num4 = num2;
      return new FileIdUsage()
      {
        MaxNegativeId = (long) num3,
        MaxPositiveId = (long) num4
      };
    }

    internal class MinMaxBinder : ObjectBinder<Tuple<int, int>>
    {
      protected SqlColumnBinder MinFileIdColumn = new SqlColumnBinder("MinFileId");
      protected SqlColumnBinder MaxFileIdColumn = new SqlColumnBinder("MaxFileId");

      protected override Tuple<int, int> Bind() => new Tuple<int, int>(this.MinFileIdColumn.GetInt32((IDataReader) this.Reader), this.MaxFileIdColumn.GetInt32((IDataReader) this.Reader));
    }
  }
}
