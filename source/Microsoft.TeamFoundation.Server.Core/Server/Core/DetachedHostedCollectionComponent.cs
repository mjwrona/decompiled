// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.DetachedHostedCollectionComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class DetachedHostedCollectionComponent : TeamFoundationSqlResourceComponent
  {
    private const string c_changeIdentityType = "\r\nUPDATE  tbl_Identity\r\nSET     TypeId = dbo.func_GetIdentityTypeIdFromName(@identityType)\r\nWHERE   Sid = @sid\r\n        AND PartitionId = @partitionId";

    internal List<string> GetAccountNamesForClaimsIdentities()
    {
      List<string> claimsIdentities = new List<string>();
      string sqlStatement = "select AccountName from tbl_identity where TypeId = 5 or TypeId = 6";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          claimsIdentities.Add(DetachedHostedCollectionComponent.AccountNameColumn.AccountName.GetString((IDataReader) reader, false));
      }
      return claimsIdentities;
    }

    internal void ChangeIdentityType(
      IdentityDescriptor descriptor,
      string identityType,
      int partitionId)
    {
      this.PrepareSqlBatch("\r\nUPDATE  tbl_Identity\r\nSET     TypeId = dbo.func_GetIdentityTypeIdFromName(@identityType)\r\nWHERE   Sid = @sid\r\n        AND PartitionId = @partitionId".Length);
      this.AddStatement("\r\nUPDATE  tbl_Identity\r\nSET     TypeId = dbo.func_GetIdentityTypeIdFromName(@identityType)\r\nWHERE   Sid = @sid\r\n        AND PartitionId = @partitionId");
      this.BindInt("@partitionId", partitionId);
      this.BindString("@identityType", identityType, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@sid", descriptor.Identifier, 256, true, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    private static class AccountNameColumn
    {
      internal static SqlColumnBinder AccountName = new SqlColumnBinder(nameof (AccountName));
    }
  }
}
