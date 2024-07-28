// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent8
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent8 : FileComponent7
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
      this.PrepareStoredProcedure("prc_SaveFile", 3600);
      this.BindInt("@fileId", (int) fileId);
      this.BindByte("@ownerId", (byte) ownerId);
      this.BindString("@fileName", fileName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
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
      this.BindInt("@relatedFileId", 0);
      this.BindLong("@offsetFrom", offsetFrom);
      this.BindLong("@offsetTo", offsetFrom + (long) contentBlockLength);
      this.BindBinary("@contentBlock", contentBlock, contentBlockLength, SqlDbType.VarBinary);
      this.BindBoolean("@overwrite", overwrite);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<SaveFileResult>((ObjectBinder<SaveFileResult>) new FileComponent.SaveFileColumns());
      return resultCollection.GetCurrent<SaveFileResult>().Items[0];
    }

    protected class QueryMD5ContextColumns : ObjectBinder<QueryMD5ContextResult>
    {
      protected SqlColumnBinder MD5Context = new SqlColumnBinder(nameof (MD5Context));
      protected SqlColumnBinder MD5ByteCount = new SqlColumnBinder(nameof (MD5ByteCount));

      protected override QueryMD5ContextResult Bind()
      {
        byte[] bytes = this.MD5Context.GetBytes((IDataReader) this.Reader, true);
        long int64 = this.MD5ByteCount.GetInt64((IDataReader) this.Reader, -1L);
        return bytes == null ? (QueryMD5ContextResult) null : new QueryMD5ContextResult(bytes, int64);
      }
    }
  }
}
