// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildAgentTable
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  [Obsolete]
  internal sealed class BuildAgentTable : TeamFoundationTableValueParameter<BuildAgent>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[9]
    {
      new SqlMetaData("AgentId", SqlDbType.Int),
      new SqlMetaData("ServiceHostId", SqlDbType.Int),
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildDirectory", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Enabled", SqlDbType.Bit),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L)
    };

    public BuildAgentTable(ICollection<BuildAgent> rows)
      : base((IEnumerable<BuildAgent>) rows, "typ_BuildAgentTable", BuildAgentTable.s_metadata)
    {
    }

    public override void SetRecord(BuildAgent row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.Uri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetInt32(1, int.Parse(DBHelper.ExtractDbId(row.ServiceHostUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetInt32(2, int.Parse(DBHelper.ExtractDbId(row.ControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetString(3, DBHelper.ServerPathToDBPath(row.Name));
      record.SetString(4, row.BuildDirectory);
      record.SetByte(5, (byte) row.Status);
      record.SetString(6, row.StatusMessage ?? string.Empty);
      record.SetBoolean(7, row.Enabled);
      if (!string.IsNullOrEmpty(row.Description))
        record.SetString(8, row.Description);
      else
        record.SetDBNull(8);
    }
  }
}
