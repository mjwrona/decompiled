// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildSchedule2Binder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildSchedule2Binder : BuildObjectBinder<Schedule2>
  {
    private SqlColumnBinder m_scheduleId = new SqlColumnBinder("ScheduleId");
    private SqlColumnBinder m_scheduleName = new SqlColumnBinder("ScheduleName");
    private SqlColumnBinder m_scheduleType = new SqlColumnBinder("ScheduleType");
    private SqlColumnBinder m_branchDetails = new SqlColumnBinder("BranchDetails");
    private SqlColumnBinder m_scheduleDetails = new SqlColumnBinder("ScheduleDetails");
    private SqlColumnBinder m_scheduleJobId = new SqlColumnBinder("ScheduleJobId");
    private SqlColumnBinder m_scheduleOnlyWithChanges = new SqlColumnBinder("ScheduleOnlyWithChanges");
    private SqlColumnBinder m_batch = new SqlColumnBinder("Batch");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");

    public BuildSchedule2Binder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override Schedule2 Bind() => new Schedule2()
    {
      ScheduleId = this.m_scheduleId.GetInt32((IDataReader) this.Reader),
      ScheduleName = this.m_scheduleName.GetString((IDataReader) this.Reader, true),
      ScheduleType = (ScheduleType) this.m_scheduleType.GetInt32((IDataReader) this.Reader),
      BranchDetails = JsonUtility.FromString<List<string>>(this.m_branchDetails.GetString((IDataReader) this.Reader, false)),
      ScheduleDetails = JsonUtility.FromString<DesignerScheduleDetails>(this.m_scheduleDetails.GetString((IDataReader) this.Reader, false)),
      ScheduleJobId = this.m_scheduleJobId.GetGuid((IDataReader) this.Reader),
      ScheduleOnlyWithChanges = this.m_scheduleOnlyWithChanges.GetBoolean((IDataReader) this.Reader),
      Batch = this.m_batch.GetBoolean((IDataReader) this.Reader),
      DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader)
    };
  }
}
