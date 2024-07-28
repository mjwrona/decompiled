// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDefinitionWorkspaceTable2
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
  internal sealed class BuildDefinitionWorkspaceTable2 : 
    TeamFoundationTableValueParameter<WorkspaceMapping>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[5]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, 400L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, 260L),
      new SqlMetaData("MappingType", SqlDbType.TinyInt),
      new SqlMetaData("Depth", SqlDbType.Int)
    };

    public BuildDefinitionWorkspaceTable2(IEnumerable<WorkspaceMapping> rows)
      : base(rows, "typ_BuildDefinitionWorkspaceTableV2", BuildDefinitionWorkspaceTable2.s_metadata)
    {
    }

    public override void SetRecord(WorkspaceMapping row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.DefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetString(1, DBHelper.VersionControlPathToDBPath(row.ServerItem));
      if (!string.IsNullOrEmpty(row.LocalItem))
        record.SetString(2, DBHelper.LocalPathToDBPath(row.LocalItem));
      else
        record.SetDBNull(2);
      record.SetByte(3, (byte) row.MappingType);
      record.SetInt32(4, row.Depth);
    }
  }
}
