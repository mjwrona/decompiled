// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TimelineAttemptBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TimelineAttemptBinder : ObjectBinder<TimelineAttemptData>
  {
    private SqlColumnBinder m_targetId = new SqlColumnBinder("TargetId");
    private SqlColumnBinder m_identifier = new SqlColumnBinder("Identifier");
    private SqlColumnBinder m_attempt = new SqlColumnBinder("Attempt");
    private SqlColumnBinder m_timelineId = new SqlColumnBinder("TimelineId");
    private SqlColumnBinder m_recordId = new SqlColumnBinder("RecordId");

    protected override TimelineAttemptData Bind() => new TimelineAttemptData()
    {
      TargetId = this.m_targetId.GetGuid((IDataReader) this.Reader),
      Attempt = new TimelineAttempt()
      {
        Identifier = this.m_identifier.GetString((IDataReader) this.Reader, false),
        Attempt = this.m_attempt.GetInt32((IDataReader) this.Reader),
        TimelineId = this.m_timelineId.GetGuid((IDataReader) this.Reader),
        RecordId = this.m_recordId.GetGuid((IDataReader) this.Reader)
      }
    };
  }
}
