// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent1690
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent1690 : FileComponent1680
  {
    public override void DeleteOrphanPendingUploadFiles(
      IEnumerable<KeyValuePair<int, int>> filesToDelete,
      bool reuseFileIdSecondaryRange)
    {
      this.PrepareStoredProcedure("prc_DeleteOrphanPendingUploadFiles");
      this.BindKeyValuePairInt32Int32Table("@filesToDelete", filesToDelete);
      this.BindBoolean("@reuseFileIdSecondaryRange", reuseFileIdSecondaryRange);
      this.ExecuteNonQuery();
    }

    public override void DeleteUnusedFiles(
      int retentionPeriodInDays,
      bool reuseFileIdSecondaryRange,
      int chunkSize = 100)
    {
      this.PrepareStoredProcedure("prc_DeleteUnusedFiles", 0);
      this.BindInt("@retentionPeriodInDays", -retentionPeriodInDays);
      this.BindInt("@chunkSize", chunkSize);
      this.BindBoolean("@reuseFileIdSecondaryRange", reuseFileIdSecondaryRange);
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
        this.BindBoolean("@reuseFileIdSecondaryRange", reuseFileIdSecondaryRange);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<SaveFileResult>((ObjectBinder<SaveFileResult>) new FileComponent.SaveFileColumns2());
        return resultCollection.GetCurrent<SaveFileResult>().Items[0];
      }
      finally
      {
        this.DataspaceRlsEnabled = dataspaceRlsEnabled;
      }
    }
  }
}
