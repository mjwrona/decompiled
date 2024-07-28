// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskLogBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskLogBinder : ObjectBinder<TaskLog>
  {
    private SqlColumnBinder m_logId = new SqlColumnBinder("LogId");
    private SqlColumnBinder m_logPath = new SqlColumnBinder("LogPath");
    private SqlColumnBinder m_lineCount = new SqlColumnBinder("LineCount");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_lastChangedOn = new SqlColumnBinder("LastChangedOn");

    protected override TaskLog Bind()
    {
      TaskLog taskLog = new TaskLog();
      taskLog.Id = this.m_logId.GetInt32((IDataReader) this.Reader);
      taskLog.Path = this.m_logPath.GetString((IDataReader) this.Reader, false);
      taskLog.LineCount = this.m_lineCount.GetInt64((IDataReader) this.Reader);
      taskLog.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      taskLog.LastChangedOn = this.m_lastChangedOn.GetDateTime((IDataReader) this.Reader);
      return taskLog;
    }
  }
}
