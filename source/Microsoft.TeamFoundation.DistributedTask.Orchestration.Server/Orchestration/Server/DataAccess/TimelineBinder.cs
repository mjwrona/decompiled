// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TimelineBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TimelineBinder : ObjectBinder<Timeline>
  {
    private SqlColumnBinder m_timelineId = new SqlColumnBinder("TimelineId");
    private SqlColumnBinder m_changeId = new SqlColumnBinder("ChangeId");
    private SqlColumnBinder m_lastChangedBy = new SqlColumnBinder("LastChangedBy");
    private SqlColumnBinder m_lastChangedOn = new SqlColumnBinder("LastChangedOn");

    protected override Timeline Bind()
    {
      Timeline timeline = new Timeline();
      timeline.Id = this.m_timelineId.GetGuid((IDataReader) this.Reader);
      timeline.ChangeId = this.m_changeId.GetInt32((IDataReader) this.Reader);
      timeline.LastChangedBy = this.m_lastChangedBy.GetGuid((IDataReader) this.Reader);
      timeline.LastChangedOn = this.m_lastChangedOn.GetDateTime((IDataReader) this.Reader);
      return timeline;
    }
  }
}
