// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CronScheduleBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class CronScheduleBinder : BuildObjectBinder<CronSchedule>
  {
    private SqlColumnBinder m_scheduleId = new SqlColumnBinder("ScheduleId");
    private SqlColumnBinder m_scheduleName = new SqlColumnBinder("ScheduleName");
    private SqlColumnBinder m_scheduleType = new SqlColumnBinder("ScheduleType");
    private SqlColumnBinder m_branchName = new SqlColumnBinder("BranchDetails");
    private SqlColumnBinder m_cronExpression = new SqlColumnBinder("ScheduleDetails");
    private SqlColumnBinder m_scheduleJobId = new SqlColumnBinder("ScheduleJobId");
    private SqlColumnBinder m_scheduleOnlyWithChanges = new SqlColumnBinder("ScheduleOnlyWithChanges");
    private SqlColumnBinder m_batch = new SqlColumnBinder("Batch");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private IVssRequestContext m_requestContext;
    private const string TraceLayer = "CronScheduleBinder";

    public CronScheduleBinder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
      this.m_requestContext = requestContext;
    }

    protected override CronSchedule Bind()
    {
      try
      {
        return new CronSchedule(this.m_scheduleId.GetInt32((IDataReader) this.Reader), this.m_scheduleName.GetString((IDataReader) this.Reader, true), this.m_branchName.GetString((IDataReader) this.Reader, false), this.m_cronExpression.GetString((IDataReader) this.Reader, false), this.m_scheduleJobId.GetGuid((IDataReader) this.Reader), this.m_scheduleOnlyWithChanges.GetBoolean((IDataReader) this.Reader), this.m_batch.GetBoolean((IDataReader) this.Reader), this.m_definitionId.GetInt32((IDataReader) this.Reader));
      }
      catch (JsonReaderException ex)
      {
        this.m_requestContext.TraceException(12030366, "Build2", nameof (CronScheduleBinder), (Exception) ex);
        throw new BuildScheduleDataCorruptedException("Cron schedule data is corrupted.");
      }
    }
  }
}
