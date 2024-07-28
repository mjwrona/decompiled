// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.BlobStoreComponent2
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  internal class BlobStoreComponent2 : BlobStoreComponent
  {
    private static SqlMetaData[] typ_BlobBlocks = new SqlMetaData[2]
    {
      new SqlMetaData("BlobId", SqlDbType.Binary, 33L),
      new SqlMetaData("BlockHash", SqlDbType.Binary, 32L)
    };

    public virtual List<SqlBlobBlockInfo> GetUnusedBlocks(
      out DateTime cutoffDateTime,
      int maxReturn)
    {
      this.PrepareStoredProcedure("BlobStore.prc_GetUnusedBlocks");
      this.BindInt(nameof (maxReturn), maxReturn);
      SqlParameter sqlParameter = this.BindDateTime("cutoffDatetime", DateTime.MinValue);
      sqlParameter.Direction = ParameterDirection.Output;
      List<SqlBlobBlockInfo> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "BlobStore.prc_GetUnusedBlocks", this.RequestContext))
      {
        resultCollection.AddBinder<SqlBlobBlockInfo>((ObjectBinder<SqlBlobBlockInfo>) new BlobBlockInfoColumns());
        items = resultCollection.GetCurrent<SqlBlobBlockInfo>().Items;
      }
      cutoffDateTime = (DateTime) sqlParameter.Value;
      return items;
    }

    public virtual void DeleteUnusedBlocks(
      IEnumerable<SqlBlobBlockInfo> pdBlocks,
      DateTime cutoffDateTime)
    {
      this.PrepareStoredProcedure("BlobStore.prc_DeleteUnusedBlocks");
      this.BindBlobBlocks(pdBlocks);
      this.BindDateTime("cutoffDatetime", cutoffDateTime);
      this.ExecuteNonQuery();
    }

    protected SqlParameter BindBlobBlocks(
      IEnumerable<SqlBlobBlockInfo> pdBlocks,
      string parameterName = "pdBlocks")
    {
      pdBlocks = pdBlocks ?? Enumerable.Empty<SqlBlobBlockInfo>();
      return this.BindTable(parameterName, "BlobStore.typ_BlobBlocks", pdBlocks.Select<SqlBlobBlockInfo, SqlDataRecord>((System.Func<SqlBlobBlockInfo, SqlDataRecord>) (bbi =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BlobStoreComponent2.typ_BlobBlocks);
        byte[] bytes = bbi.BlobId.Bytes;
        sqlDataRecord.SetBytes(0, 0L, bytes, 0, bytes.Length);
        byte[] hashBytes = bbi.BlockHash.HashBytes;
        sqlDataRecord.SetBytes(1, 0L, hashBytes, 0, hashBytes.Length);
        return sqlDataRecord;
      })));
    }
  }
}
