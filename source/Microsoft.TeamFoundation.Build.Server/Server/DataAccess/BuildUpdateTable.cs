// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildUpdateTable
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  [Obsolete]
  internal sealed class BuildUpdateTable : TeamFoundationTableValueParameter<BuildUpdateOptions>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[12]
    {
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Fields", SqlDbType.Int),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 260L),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 260L),
      new SqlMetaData("LabelName", SqlDbType.NVarChar, 326L),
      new SqlMetaData("LogLocation", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Quality", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("CompilationStatus", SqlDbType.TinyInt),
      new SqlMetaData("TestStatus", SqlDbType.TinyInt),
      new SqlMetaData("KeepForever", SqlDbType.Bit),
      new SqlMetaData("SourceGetVersion", SqlDbType.NVarChar, 325L)
    };

    public BuildUpdateTable(ICollection<BuildUpdateOptions> rows)
      : base((IEnumerable<BuildUpdateOptions>) rows, "typ_BuildUpdateTable", BuildUpdateTable.s_metadata)
    {
    }

    public override void SetRecord(BuildUpdateOptions row, SqlDataRecord record)
    {
      record.SetString(0, DBHelper.ExtractDbId(row.Uri));
      record.SetInt32(1, (int) row.Fields);
      if ((row.Fields & BuildUpdate.BuildNumber) == BuildUpdate.BuildNumber)
        record.SetString(2, DBHelper.ServerPathToDBPath(row.BuildNumber));
      else
        record.SetDBNull(2);
      if ((row.Fields & BuildUpdate.DropLocation) == BuildUpdate.DropLocation)
        record.SetString(3, row.DropLocation);
      else
        record.SetDBNull(3);
      if ((row.Fields & BuildUpdate.LabelName) == BuildUpdate.LabelName)
        record.SetString(4, row.LabelName);
      else
        record.SetDBNull(4);
      if ((row.Fields & BuildUpdate.LogLocation) == BuildUpdate.LogLocation)
        record.SetString(5, row.LogLocation);
      else
        record.SetDBNull(5);
      if ((row.Fields & BuildUpdate.Quality) == BuildUpdate.Quality)
        record.SetString(6, BuildQuality.TryConvertBuildQualityToResId(row.Quality));
      else
        record.SetDBNull(6);
      if ((row.Fields & BuildUpdate.Status) == BuildUpdate.Status)
        record.SetByte(7, (byte) row.Status);
      else
        record.SetDBNull(7);
      if ((row.Fields & BuildUpdate.CompilationStatus) == BuildUpdate.CompilationStatus)
        record.SetByte(8, (byte) row.CompilationStatus);
      else
        record.SetDBNull(8);
      if ((row.Fields & BuildUpdate.TestStatus) == BuildUpdate.TestStatus)
        record.SetByte(9, (byte) row.TestStatus);
      else
        record.SetDBNull(9);
      if ((row.Fields & BuildUpdate.KeepForever) == BuildUpdate.KeepForever)
        record.SetBoolean(10, row.KeepForever);
      else
        record.SetDBNull(10);
      if ((row.Fields & BuildUpdate.SourceGetVersion) == BuildUpdate.SourceGetVersion)
        record.SetString(11, row.SourceGetVersion);
      else
        record.SetDBNull(11);
    }
  }
}
