// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.BlobStoreComponent
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  internal class BlobStoreComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<BlobStoreComponent>(1),
      (IComponentCreator) new ComponentCreator<BlobStoreComponent2>(2)
    }, "BlobStore");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static string s_area = nameof (BlobStoreComponent);
    private static SqlMetaData[] typ_BlobBlockList = new SqlMetaData[1]
    {
      new SqlMetaData("BlockHash", SqlDbType.Binary, 32L)
    };

    internal int ContainerId { private get; set; }

    static BlobStoreComponent() => BlobStoreComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public BlobStoreComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) BlobStoreComponent.s_sqlExceptionFactories;

    protected override string TraceArea => BlobStoreComponent.s_area;

    public virtual bool AddBlock(
      BlobIdentifier bid,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash,
      int fileId,
      long fileLength)
    {
      this.PrepareStoredProcedure("BlobStore.prc_AddBlock");
      this.BindBlobId(bid);
      this.BindBlockHash(blockHash);
      this.BindInt(nameof (fileId), fileId);
      this.BindLong(nameof (fileLength), fileLength);
      SqlParameter sqlParameter = this.BindOutputBoolean("isAddedOrUpdated");
      this.ExecuteNonQuery();
      return (bool) sqlParameter.Value;
    }

    public virtual bool PutBlockList(
      BlobIdentifier bid,
      IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash> blockHashes,
      ref string etag)
    {
      this.PrepareStoredProcedure("BlobStore.prc_PutBlockList");
      this.BindBlobId(bid);
      this.BindBlobBlockList(blockHashes);
      SqlParameter sqlParameter1 = this.BindOutputBoolean("matched");
      SqlParameter sqlParameter2 = this.BindOutputETag(true, etag);
      this.ExecuteNonQuery();
      etag = DBNull.Value == sqlParameter2.Value ? (string) null : (string) sqlParameter2.Value;
      return (bool) sqlParameter1.Value;
    }

    public virtual List<SqlBlockInfo> GetBlockList(
      BlobIdentifier bid,
      out string etag,
      out bool isSealed)
    {
      this.PrepareStoredProcedure("BlobStore.prc_GetBlockList");
      this.BindBlobId(bid);
      SqlParameter sqlParameter1 = this.BindOutputBoolean("sealed");
      SqlParameter sqlParameter2 = this.BindOutputETag();
      List<SqlBlockInfo> sqlBlockInfo;
      try
      {
        sqlBlockInfo = this.GetSQLBlockInfo("BlobStore.prc_GetBlockList");
      }
      catch (SqlException ex)
      {
        throw;
      }
      etag = DBNull.Value == sqlParameter2.Value ? (string) null : (string) sqlParameter2.Value;
      isSealed = (bool) sqlParameter1.Value;
      return sqlBlockInfo;
    }

    public virtual List<SqlBlockInfo> GetBlobBlocks(
      BlobIdentifier bid,
      out string etag,
      out bool isSealed)
    {
      this.PrepareStoredProcedure("BlobStore.prc_GetBlobBlocks");
      this.BindBlobId(bid);
      SqlParameter sqlParameter1 = this.BindOutputBoolean("sealed");
      SqlParameter sqlParameter2 = this.BindOutputETag();
      List<SqlBlockInfo> sqlBlockInfo;
      try
      {
        sqlBlockInfo = this.GetSQLBlockInfo("BlobStore.prc_GetBlobBlocks");
      }
      catch (SqlException ex)
      {
        throw;
      }
      etag = DBNull.Value == sqlParameter2.Value ? (string) null : (string) sqlParameter2.Value;
      isSealed = (bool) sqlParameter1.Value;
      return sqlBlockInfo;
    }

    public virtual List<SqlBlockInfo> DeleteBlob(
      BlobIdentifier bid,
      ref string etag,
      out bool matched)
    {
      this.PrepareStoredProcedure("BlobStore.prc_DeleteBlob");
      this.BindBlobId(bid);
      SqlParameter sqlParameter1 = this.BindOutputBoolean(nameof (matched));
      SqlParameter sqlParameter2 = this.BindOutputETag(true, etag);
      List<SqlBlockInfo> sqlBlockInfo;
      try
      {
        sqlBlockInfo = this.GetSQLBlockInfo("BlobStore.prc_DeleteBlob");
      }
      catch (SqlException ex)
      {
        throw;
      }
      etag = DBNull.Value == sqlParameter2.Value ? (string) null : (string) sqlParameter2.Value;
      matched = (bool) sqlParameter1.Value;
      return sqlBlockInfo;
    }

    protected override SqlCommand PrepareStoredProcedure(string storedProcedure)
    {
      SqlCommand sqlCommand = base.PrepareStoredProcedure(storedProcedure);
      this.BindInt("@containerId", this.ContainerId);
      return sqlCommand;
    }

    protected SqlParameter BindBlobBlockList(
      IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash> blockHashes,
      string parameterName = "allBlocks")
    {
      blockHashes = blockHashes ?? Enumerable.Empty<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash>();
      return this.BindTable(parameterName, "BlobStore.typ_BlobBlockList", blockHashes.Select<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, SqlDataRecord>((System.Func<Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash, SqlDataRecord>) (blockHash =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BlobStoreComponent.typ_BlobBlockList);
        byte[] hashBytes = blockHash.HashBytes;
        sqlDataRecord.SetBytes(0, 0L, hashBytes, 0, hashBytes.Length);
        return sqlDataRecord;
      })));
    }

    protected void BindBlobId(BlobIdentifier bid) => this.BindBinary("blobId", bid.Bytes, SqlDbType.Binary);

    protected void BindBlockHash(Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash blockHash) => this.BindBinary(nameof (blockHash), blockHash.HashBytes, SqlDbType.Binary);

    protected SqlParameter BindOutputBoolean(string pname)
    {
      SqlParameter sqlParameter = this.BindBoolean(pname, false);
      sqlParameter.Direction = ParameterDirection.Output;
      return sqlParameter;
    }

    protected SqlParameter BindOutputETag(bool bidirectional = false, string etagValue = null)
    {
      SqlParameter sqlParameter = this.BindString("etag", etagValue, 50, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      sqlParameter.Direction = bidirectional ? ParameterDirection.InputOutput : ParameterDirection.Output;
      if (etagValue == null)
        sqlParameter.Size = 50;
      return sqlParameter;
    }

    private List<SqlBlockInfo> GetSQLBlockInfo(string sprocName)
    {
      this.RetriesRemaining = 5;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), sprocName, this.RequestContext))
      {
        resultCollection.AddBinder<SqlBlockInfo>((ObjectBinder<SqlBlockInfo>) new BlockInfoColumns());
        return resultCollection.GetCurrent<SqlBlockInfo>().Items;
      }
    }
  }
}
