// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent12
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent12 : SecurityComponent11
  {
    private static readonly SqlMetaData[] typ_TokenRenameTable3 = new SqlMetaData[6]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("NewDataspaceId", SqlDbType.Int),
      new SqlMetaData("OldToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NewToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Copy", SqlDbType.Bit),
      new SqlMetaData("Recurse", SqlDbType.Bit)
    };

    protected override SqlParameter BindTokenRenameTable(
      string parameterName,
      IEnumerable<TokenRename> rows)
    {
      rows = rows ?? Enumerable.Empty<TokenRename>();
      System.Func<TokenRename, SqlDataRecord> selector = (System.Func<TokenRename, SqlDataRecord>) (rename =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SecurityComponent12.typ_TokenRenameTable3);
        int dataspaceIdForToken1 = this.GetDataspaceIdForToken(rename.OldToken);
        int dataspaceIdForToken2 = this.GetDataspaceIdForToken(rename.NewToken);
        if (dataspaceIdForToken1 != dataspaceIdForToken2)
          throw new InvalidOperationException();
        sqlDataRecord.SetInt32(0, dataspaceIdForToken1);
        sqlDataRecord.SetInt32(1, dataspaceIdForToken2);
        sqlDataRecord.SetString(2, rename.OldToken);
        sqlDataRecord.SetString(3, rename.NewToken);
        sqlDataRecord.SetBoolean(4, rename.Copy);
        sqlDataRecord.SetBoolean(5, rename.Recurse);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_TokenRenameTable3", rows.Select<TokenRename, SqlDataRecord>(selector));
    }

    public override int RenameToken(
      Guid namespaceId,
      string originalToken,
      string newToken,
      bool copy,
      char separator)
    {
      int dataspaceIdForToken1 = this.GetDataspaceIdForToken(originalToken);
      int dataspaceIdForToken2 = this.GetDataspaceIdForToken(newToken);
      if (dataspaceIdForToken1 != dataspaceIdForToken2)
        throw new InvalidOperationException();
      this.PrepareStoredProcedure("prc_RenameToken");
      this.BindServiceHostId();
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindInt("@dataspaceId", dataspaceIdForToken1);
      this.BindInt("@newDataspaceId", dataspaceIdForToken2);
      this.BindString("@originalToken", originalToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@newToken", newToken, -1, false, SqlDbType.NVarChar);
      this.BindBoolean("@copy", copy);
      this.BindString("@separator", separator.ToString((IFormatProvider) CultureInfo.InvariantCulture), 1, false, SqlDbType.NVarChar);
      this.BindGuid("@writerIdentifier", this.Author);
      return (int) this.ExecuteScalar();
    }
  }
}
