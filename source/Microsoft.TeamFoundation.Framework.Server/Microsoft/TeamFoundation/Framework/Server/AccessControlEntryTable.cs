// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControlEntryTable
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class AccessControlEntryTable
  {
    private static readonly SqlMetaData[] typ_AccessControlEntryTable = new SqlMetaData[6]
    {
      new SqlMetaData("IdentityType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("Identifier", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SecurityToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IndexableToken", SqlDbType.NVarChar, 350L),
      new SqlMetaData("AllowPermission", SqlDbType.Int),
      new SqlMetaData("DenyPermission", SqlDbType.Int)
    };

    public static SqlParameter BindAccessControlEntryTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<IAccessControlList> rows,
      char separator)
    {
      rows = rows ?? Enumerable.Empty<IAccessControlList>();
      return component.BindTable(parameterName, "typ_AccessControlEntryTable", AccessControlEntryTable.BindAccessControlEntryRow(rows, separator));
    }

    private static IEnumerable<SqlDataRecord> BindAccessControlEntryRow(
      IEnumerable<IAccessControlList> rows,
      char separator)
    {
      foreach (IAccessControlList acl in rows)
      {
        foreach (IAccessControlEntry accessControlEntry in acl.AccessControlEntries)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(AccessControlEntryTable.typ_AccessControlEntryTable);
          string token = PermissionTable.AddSeparator(separator, acl.Token);
          sqlDataRecord.SetString(0, accessControlEntry.Descriptor.IdentityType);
          sqlDataRecord.SetString(1, accessControlEntry.Descriptor.Identifier);
          sqlDataRecord.SetString(2, token);
          sqlDataRecord.SetString(3, PermissionTable.GetIndexableTokenFromToken(token, separator));
          sqlDataRecord.SetInt32(4, accessControlEntry.Allow);
          sqlDataRecord.SetInt32(5, accessControlEntry.Deny);
          yield return sqlDataRecord;
        }
      }
    }
  }
}
