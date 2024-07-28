// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelComponent7
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class LabelComponent7 : LabelComponent6
  {
    private static readonly SqlMetaData[] typ_CreateRecursiveLabelInput = new SqlMetaData[3]
    {
      new SqlMetaData("Recursive", SqlDbType.Bit),
      new SqlMetaData("VersionFrom", SqlDbType.Int),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L)
    };

    protected virtual SqlParameter BindRecursiveLabelItemTable(
      string parameterName,
      IEnumerable<RecursiveLabelItem> rows)
    {
      rows = rows ?? Enumerable.Empty<RecursiveLabelItem>();
      System.Func<RecursiveLabelItem, SqlDataRecord> selector = (System.Func<RecursiveLabelItem, SqlDataRecord>) (item =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(LabelComponent7.typ_CreateRecursiveLabelInput);
        string databasePath = DBPath.ServerToDatabasePath(this.ConvertToPathWithProjectGuid(item.ServerItem));
        sqlDataRecord.SetBoolean(0, item.Active);
        sqlDataRecord.SetInt32(1, item.VersionFrom);
        sqlDataRecord.SetString(2, databasePath);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_CreateRecursiveLabelInput", rows.Select<RecursiveLabelItem, SqlDataRecord>(selector));
    }

    public override void CreateRecursiveLabel(
      string labelName,
      ItemPathPair labelScopePair,
      Guid ownerId,
      string comment,
      List<RecursiveLabelItem> items)
    {
      this.PrepareStoredProcedure("prc_CreateRecursiveLabel");
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@labelScopeDataspaceId", "@labelScope", labelScopePair, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindRecursiveLabelItemTable("@labelItems", (IEnumerable<RecursiveLabelItem>) items);
      this.ExecuteNonQuery();
    }
  }
}
