// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskLogPageBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskLogPageBinder : ObjectBinder<TaskLogPage>
  {
    private SqlColumnBinder m_logId = new SqlColumnBinder("LogId");
    private SqlColumnBinder m_pageId = new SqlColumnBinder("PageId");
    private SqlColumnBinder m_startLine = new SqlColumnBinder("StartLine");
    private SqlColumnBinder m_endLine = new SqlColumnBinder("EndLine");
    private SqlColumnBinder m_state = new SqlColumnBinder("State");

    protected override TaskLogPage Bind() => new TaskLogPage()
    {
      LogId = this.m_logId.GetInt32((IDataReader) this.Reader),
      PageId = this.m_pageId.GetInt32((IDataReader) this.Reader),
      StartLine = this.m_startLine.GetInt64((IDataReader) this.Reader),
      EndLine = this.m_endLine.GetInt64((IDataReader) this.Reader),
      State = (TaskLogPageState) this.m_state.GetInt32((IDataReader) this.Reader)
    };
  }
}
