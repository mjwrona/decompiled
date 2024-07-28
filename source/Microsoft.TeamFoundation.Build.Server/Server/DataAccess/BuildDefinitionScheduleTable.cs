// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDefinitionScheduleTable
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
  internal sealed class BuildDefinitionScheduleTable : TeamFoundationTableValueParameter<Schedule>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[10]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("StartTime", SqlDbType.Int),
      new SqlMetaData("Weekday1", SqlDbType.Bit),
      new SqlMetaData("Weekday2", SqlDbType.Bit),
      new SqlMetaData("Weekday3", SqlDbType.Bit),
      new SqlMetaData("Weekday4", SqlDbType.Bit),
      new SqlMetaData("Weekday5", SqlDbType.Bit),
      new SqlMetaData("Weekday6", SqlDbType.Bit),
      new SqlMetaData("Weekday7", SqlDbType.Bit),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 32L)
    };

    public BuildDefinitionScheduleTable(ICollection<Schedule> rows)
      : base((IEnumerable<Schedule>) rows, "typ_BuildDefinitionScheduleTable", BuildDefinitionScheduleTable.s_metadata)
    {
    }

    public override void SetRecord(Schedule row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.DefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetInt32(1, row.UtcStartTime);
      record.SetBoolean(2, (row.UtcDaysToBuild & ScheduleDays.Sunday) == ScheduleDays.Sunday);
      record.SetBoolean(3, (row.UtcDaysToBuild & ScheduleDays.Monday) == ScheduleDays.Monday);
      record.SetBoolean(4, (row.UtcDaysToBuild & ScheduleDays.Tuesday) == ScheduleDays.Tuesday);
      record.SetBoolean(5, (row.UtcDaysToBuild & ScheduleDays.Wednesday) == ScheduleDays.Wednesday);
      record.SetBoolean(6, (row.UtcDaysToBuild & ScheduleDays.Thursday) == ScheduleDays.Thursday);
      record.SetBoolean(7, (row.UtcDaysToBuild & ScheduleDays.Friday) == ScheduleDays.Friday);
      record.SetBoolean(8, (row.UtcDaysToBuild & ScheduleDays.Saturday) == ScheduleDays.Saturday);
      record.SetString(9, row.TimeZoneId);
    }
  }
}
