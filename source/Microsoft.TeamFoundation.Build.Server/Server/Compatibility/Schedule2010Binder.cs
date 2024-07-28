// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.Schedule2010Binder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class Schedule2010Binder : BuildObjectBinder<Schedule2010>
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

    protected override Schedule2010 Bind() => new Schedule2010()
    {
      DefinitionUri = this.definitionId.GetArtifactUriFromInt32(this.Reader, "Definition", false),
      UtcStartTime = this.time.GetInt32((IDataReader) this.Reader, 0),
      UtcDaysToBuild = (ScheduleDays2010) ((this.weekday1.GetBoolean((IDataReader) this.Reader, false) ? 64 : 0) | (this.weekday2.GetBoolean((IDataReader) this.Reader, false) ? 1 : 0) | (this.weekday3.GetBoolean((IDataReader) this.Reader, false) ? 2 : 0) | (this.weekday4.GetBoolean((IDataReader) this.Reader, false) ? 4 : 0) | (this.weekday5.GetBoolean((IDataReader) this.Reader, false) ? 8 : 0) | (this.weekday6.GetBoolean((IDataReader) this.Reader, false) ? 16 : 0) | (this.weekday7.GetBoolean((IDataReader) this.Reader, false) ? 32 : 0)),
      TimeZoneId = this.timeZoneId.GetString((IDataReader) this.Reader, false)
    };
  }
}
