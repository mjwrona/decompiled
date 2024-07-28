// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildAgentUpdateTable
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
  internal sealed class BuildAgentUpdateTable : 
    TeamFoundationTableValueParameter<BuildAgentUpdateOptions>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[9]
    {
      new SqlMetaData("AgentId", SqlDbType.Int),
      new SqlMetaData("Fields", SqlDbType.Int),
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildDirectory", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Enabled", SqlDbType.Bit),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L)
    };

    public BuildAgentUpdateTable(ICollection<BuildAgentUpdateOptions> rows)
      : base((IEnumerable<BuildAgentUpdateOptions>) rows, "typ_BuildAgentUpdateTable", BuildAgentUpdateTable.s_metadata)
    {
    }

    public override void SetRecord(BuildAgentUpdateOptions row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.Uri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetInt32(1, (int) row.Fields);
      if ((row.Fields & BuildAgentUpdate.ControllerUri) == BuildAgentUpdate.ControllerUri && !string.IsNullOrEmpty(row.ControllerUri))
        record.SetInt32(2, int.Parse(DBHelper.ExtractDbId(row.ControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      else
        record.SetDBNull(2);
      if ((row.Fields & BuildAgentUpdate.Name) == BuildAgentUpdate.Name)
        record.SetString(3, DBHelper.ServerPathToDBPath(row.Name));
      else
        record.SetDBNull(3);
      if ((row.Fields & BuildAgentUpdate.BuildDirectory) == BuildAgentUpdate.BuildDirectory)
        record.SetString(4, row.BuildDirectory);
      else
        record.SetDBNull(4);
      if ((row.Fields & BuildAgentUpdate.Status) == BuildAgentUpdate.Status)
        record.SetByte(5, (byte) row.Status);
      else
        record.SetDBNull(5);
      if ((row.Fields & BuildAgentUpdate.StatusMessage) == BuildAgentUpdate.StatusMessage)
        record.SetString(6, row.StatusMessage ?? string.Empty);
      else
        record.SetDBNull(6);
      if ((row.Fields & BuildAgentUpdate.Enabled) == BuildAgentUpdate.Enabled)
        record.SetBoolean(7, row.Enabled);
      else
        record.SetDBNull(7);
      if ((row.Fields & BuildAgentUpdate.Description) == BuildAgentUpdate.Description && !string.IsNullOrEmpty(row.Description))
        record.SetString(8, row.Description);
      else
        record.SetDBNull(8);
    }
  }
}
