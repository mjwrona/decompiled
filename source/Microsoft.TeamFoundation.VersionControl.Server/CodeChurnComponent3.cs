// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CodeChurnComponent3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CodeChurnComponent3 : CodeChurnComponent2
  {
    private static readonly SqlMetaData[] typ_VersionedItemId2 = new SqlMetaData[3]
    {
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ItemId", SqlDbType.Int),
      new SqlMetaData("VersionFrom", SqlDbType.Int)
    };

    protected override SqlParameter BindArtifactSpecToVersionedItemIdTable(
      string parameterName,
      IEnumerable<ArtifactSpec> rows)
    {
      rows = rows ?? Enumerable.Empty<ArtifactSpec>();
      System.Func<ArtifactSpec, SqlDataRecord> selector = (System.Func<ArtifactSpec, SqlDataRecord>) (item =>
      {
        SqlDataRecord versionedItemIdTable = new SqlDataRecord(CodeChurnComponent3.typ_VersionedItemId2);
        int num = (int) item.Id[0] << 24 | (int) item.Id[1] << 16 | (int) item.Id[2] << 8 | (int) item.Id[3];
        versionedItemIdTable.SetInt32(0, this.GetDataspaceIdDebug(item.DataspaceIdentifier, (string) null));
        versionedItemIdTable.SetInt32(1, num);
        versionedItemIdTable.SetInt32(2, item.Version);
        return versionedItemIdTable;
      });
      return this.BindTable(parameterName, "typ_VersionedItemId2", rows.Select<ArtifactSpec, SqlDataRecord>(selector));
    }

    public override ResultCollection GetFailedItemPairs(
      IEnumerable<ArtifactSpec> failedItemPairSpecs)
    {
      this.PrepareStoredProcedure("prc_QueryChurnFailedItemPairs", 3600);
      this.BindArtifactSpecToVersionedItemIdTable("@itemPairs", failedItemPairSpecs);
      ResultCollection failedItemPairs = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      failedItemPairs.AddBinder<ItemPair>((ObjectBinder<ItemPair>) new ItemPairColumns3((VersionControlSqlResourceComponent) this));
      return failedItemPairs;
    }

    public override ResultCollection GetChurnItemPairs(
      int changeSetId,
      string lastServerItem,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_QueryChurnItemPairs", 3600);
      this.BindInt("@changeSetId", changeSetId);
      if (lastServerItem != null)
        this.BindString("@lastServerItem", DBPath.ServerToDatabasePath(this.ConvertToPathWithProjectGuid(lastServerItem)), -1, false, SqlDbType.NVarChar);
      ResultCollection churnItemPairs = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      churnItemPairs.AddBinder<ItemPair>((ObjectBinder<ItemPair>) new ItemPairColumns3((VersionControlSqlResourceComponent) this));
      return churnItemPairs;
    }
  }
}
