// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceComponent6
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkspaceComponent6 : WorkspaceComponent5
  {
    private readonly SqlMetaData[] typ_Mapping2 = new SqlMetaData[4]
    {
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MappingType", SqlDbType.Bit),
      new SqlMetaData("Depth", SqlDbType.TinyInt)
    };

    protected override SqlParameter BindMappingTable(
      string parameterName,
      IEnumerable<Mapping> rows)
    {
      rows = rows ?? Enumerable.Empty<Mapping>();
      System.Func<Mapping, SqlDataRecord> selector = (System.Func<Mapping, SqlDataRecord>) (mapping =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_Mapping2);
        sqlDataRecord.SetString(0, DBPath.ServerToDatabasePath(mapping.ServerItem));
        if (mapping is WorkingFolder workingFolder2 && workingFolder2.Type == WorkingFolderType.Map && workingFolder2.LocalItem != null)
          sqlDataRecord.SetString(1, DBPath.LocalToDatabasePath(workingFolder2.LocalItem));
        else
          sqlDataRecord.SetDBNull(1);
        sqlDataRecord.SetBoolean(2, mapping.Type != WorkingFolderType.Cloak);
        sqlDataRecord.SetByte(3, (byte) mapping.Depth);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Mapping2", rows.Select<Mapping, SqlDataRecord>(selector));
    }
  }
}
