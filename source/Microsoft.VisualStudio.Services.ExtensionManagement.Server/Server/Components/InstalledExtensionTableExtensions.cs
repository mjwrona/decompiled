// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.InstalledExtensionTableExtensions
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal static class InstalledExtensionTableExtensions
  {
    private static SqlMetaData[] typ_InstalledExtensionTable = new SqlMetaData[5]
    {
      new SqlMetaData("PublisherName", SqlDbType.VarChar, 100L),
      new SqlMetaData("ExtensionName", SqlDbType.VarChar, 100L),
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("Flags", SqlDbType.Int),
      new SqlMetaData("LastVersionCheck", SqlDbType.DateTime)
    };

    public static SqlParameter BindInstalledExtensionTable(
      this InstalledExtensionComponent component,
      string parameterName,
      IEnumerable<ExtensionState> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionState>();
      System.Func<ExtensionState, SqlDataRecord> selector = (System.Func<ExtensionState, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(InstalledExtensionTableExtensions.typ_InstalledExtensionTable);
        sqlDataRecord.SetString(0, row.PublisherName);
        sqlDataRecord.SetString(1, row.ExtensionName);
        if (string.IsNullOrEmpty(row.Version))
          sqlDataRecord.SetDBNull(2);
        else
          sqlDataRecord.SetString(2, row.Version);
        sqlDataRecord.SetInt32(3, (int) row.Flags);
        sqlDataRecord.SetDateTime(4, row.LastVersionCheck.HasValue ? row.LastVersionCheck.Value : DateTime.UtcNow);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "Extension.typ_InstalledExtensionTable", rows.Select<ExtensionState, SqlDataRecord>(selector));
    }
  }
}
