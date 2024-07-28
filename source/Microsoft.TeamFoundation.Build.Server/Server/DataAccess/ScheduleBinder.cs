// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.ScheduleBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class ScheduleBinder : BuildObjectBinder<Schedule>
  {
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder time = new SqlColumnBinder("ScheduleTime");
    private SqlColumnBinder weekday1 = new SqlColumnBinder("Weekday1");
    private SqlColumnBinder weekday2 = new SqlColumnBinder("Weekday2");
    private SqlColumnBinder weekday3 = new SqlColumnBinder("Weekday3");
    private SqlColumnBinder weekday4 = new SqlColumnBinder("Weekday4");
    private SqlColumnBinder weekday5 = new SqlColumnBinder("Weekday5");
    private SqlColumnBinder weekday6 = new SqlColumnBinder("Weekday6");
    private SqlColumnBinder weekday7 = new SqlColumnBinder("Weekday7");
    private SqlColumnBinder timeZoneId = new SqlColumnBinder("TimeZoneId");
    private SqlColumnBinder lastModified = new SqlColumnBinder("LastModified");

    protected override Schedule Bind()
    {
      Schedule schedule = new Schedule();
      schedule.DefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false);
      schedule.UtcStartTime = this.time.GetInt32((IDataReader) this.Reader, 0);
      schedule.UtcDaysToBuild = (ScheduleDays) ((this.weekday1.GetBoolean((IDataReader) this.Reader, false) ? 64 : 0) | (this.weekday2.GetBoolean((IDataReader) this.Reader, false) ? 1 : 0) | (this.weekday3.GetBoolean((IDataReader) this.Reader, false) ? 2 : 0) | (this.weekday4.GetBoolean((IDataReader) this.Reader, false) ? 4 : 0) | (this.weekday5.GetBoolean((IDataReader) this.Reader, false) ? 8 : 0) | (this.weekday6.GetBoolean((IDataReader) this.Reader, false) ? 16 : 0) | (this.weekday7.GetBoolean((IDataReader) this.Reader, false) ? 32 : 0));
      schedule.TimeZoneId = this.timeZoneId.GetString((IDataReader) this.Reader, false);
      schedule.LastModifiedUtc = this.lastModified.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      schedule.AdjustUtcStartTimeForDST(DateTime.UtcNow);
      return schedule;
    }
  }
}
