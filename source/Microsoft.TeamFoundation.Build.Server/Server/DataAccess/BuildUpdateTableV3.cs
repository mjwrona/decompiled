// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildUpdateTableV3
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
  internal sealed class BuildUpdateTableV3 : TeamFoundationTableValueParameter<BuildUpdateOptions>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[14]
    {
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Fields", SqlDbType.Int),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 260L),
      new SqlMetaData("DropLocationRoot", SqlDbType.NVarChar, 400L),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("LabelName", SqlDbType.NVarChar, 326L),
      new SqlMetaData("LogLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("ContainerId", SqlDbType.BigInt),
      new SqlMetaData("Quality", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("CompilationStatus", SqlDbType.TinyInt),
      new SqlMetaData("TestStatus", SqlDbType.TinyInt),
      new SqlMetaData("KeepForever", SqlDbType.Bit),
      new SqlMetaData("SourceGetVersion", SqlDbType.NVarChar, 325L)
    };

    public BuildUpdateTableV3(IEnumerable<BuildUpdateOptions> rows)
      : base(rows, "typ_BuildUpdateTableV3", BuildUpdateTableV3.s_metadata)
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
      if ((row.Fields & BuildUpdate.DropLocationRoot) == BuildUpdate.DropLocationRoot)
        record.SetString(3, row.DropLocationRoot);
      else
        record.SetDBNull(3);
      if ((row.Fields & BuildUpdate.DropLocation) == BuildUpdate.DropLocation)
        record.SetString(4, row.DropLocation);
      else
        record.SetDBNull(4);
      if ((row.Fields & BuildUpdate.LabelName) == BuildUpdate.LabelName)
        record.SetString(5, row.LabelName);
      else
        record.SetDBNull(5);
      if ((row.Fields & BuildUpdate.LogLocation) == BuildUpdate.LogLocation)
        record.SetString(6, row.LogLocation);
      else
        record.SetDBNull(6);
      if ((row.Fields & BuildUpdate.ContainerId) == BuildUpdate.ContainerId)
      {
        long? containerId = row.ContainerId;
        long num1 = 0;
        if (containerId.GetValueOrDefault() > num1 & containerId.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          containerId = row.ContainerId;
          long num2 = containerId.Value;
          sqlDataRecord.SetInt64(7, num2);
          goto label_19;
        }
      }
      record.SetDBNull(7);
label_19:
      if ((row.Fields & BuildUpdate.Quality) == BuildUpdate.Quality)
        record.SetString(8, BuildQuality.TryConvertBuildQualityToResId(row.Quality));
      else
        record.SetDBNull(8);
      if ((row.Fields & BuildUpdate.Status) == BuildUpdate.Status)
        record.SetByte(9, (byte) row.Status);
      else
        record.SetDBNull(9);
      if ((row.Fields & BuildUpdate.CompilationStatus) == BuildUpdate.CompilationStatus)
        record.SetByte(10, (byte) row.CompilationStatus);
      else
        record.SetDBNull(10);
      if ((row.Fields & BuildUpdate.TestStatus) == BuildUpdate.TestStatus)
        record.SetByte(11, (byte) row.TestStatus);
      else
        record.SetDBNull(11);
      if ((row.Fields & BuildUpdate.KeepForever) == BuildUpdate.KeepForever)
        record.SetBoolean(12, row.KeepForever);
      else
        record.SetDBNull(12);
      if ((row.Fields & BuildUpdate.SourceGetVersion) == BuildUpdate.SourceGetVersion)
        record.SetString(13, row.SourceGetVersion);
      else
        record.SetDBNull(13);
    }
  }
}
