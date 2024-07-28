// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component45
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component45 : Build2Component44
  {
    protected override SqlParameter BindBuildUpdateTable(
      string parameterName,
      IEnumerable<BuildData> builds)
    {
      builds = builds ?? Enumerable.Empty<BuildData>();
      Func<BuildData, SqlDataRecord> selector = (Func<BuildData, SqlDataRecord>) (build =>
      {
        SqlDataRecord record1 = new SqlDataRecord(Build2Component.typ_BuildUpdateTable4);
        record1.SetInt32(0, build.Id);
        record1.SetString(1, DBHelper.ServerPathToDBPath(build.BuildNumber), BindStringBehavior.Unchanged);
        SqlDataRecord record2 = record1;
        DateTime? nullable1 = build.StartTime;
        DateTime dateTime1 = nullable1 ?? DateTime.MinValue;
        record2.SetNullableDateTime(2, dateTime1);
        SqlDataRecord record3 = record1;
        nullable1 = build.FinishTime;
        DateTime dateTime2 = nullable1 ?? DateTime.MinValue;
        record3.SetNullableDateTime(3, dateTime2);
        record1.SetString(4, build.SourceBranch, BindStringBehavior.Unchanged);
        record1.SetString(5, build.SourceVersion, BindStringBehavior.Unchanged);
        SqlDataRecord record4 = record1;
        BuildStatus? status = build.Status;
        byte? nullable2 = status.HasValue ? new byte?((byte) status.GetValueOrDefault()) : new byte?();
        record4.SetNullableByte(6, nullable2);
        SqlDataRecord record5 = record1;
        BuildResult? result = build.Result;
        byte? nullable3 = result.HasValue ? new byte?((byte) result.GetValueOrDefault()) : new byte?();
        record5.SetNullableByte(7, nullable3);
        bool? nullable4 = build.LegacyInputKeepForever;
        if (nullable4.HasValue)
        {
          SqlDataRecord sqlDataRecord = record1;
          nullable4 = build.LegacyInputKeepForever;
          int num = nullable4.Value ? 1 : 0;
          sqlDataRecord.SetBoolean(8, num != 0);
        }
        nullable4 = build.LegacyInputRetainedByRelease;
        if (nullable4.HasValue)
        {
          SqlDataRecord sqlDataRecord = record1;
          nullable4 = build.LegacyInputRetainedByRelease;
          int num = nullable4.Value ? 1 : 0;
          sqlDataRecord.SetBoolean(9, num != 0);
        }
        if (build.ValidationResults.Count > 0)
          record1.SetString(10, JsonUtility.ToString<BuildRequestValidationResult>((IList<BuildRequestValidationResult>) build.ValidationResults), BindStringBehavior.Unchanged);
        return record1;
      });
      return this.BindTable(parameterName, "Build.typ_BuildUpdateTable4", builds.Select<BuildData, SqlDataRecord>(selector));
    }
  }
}
