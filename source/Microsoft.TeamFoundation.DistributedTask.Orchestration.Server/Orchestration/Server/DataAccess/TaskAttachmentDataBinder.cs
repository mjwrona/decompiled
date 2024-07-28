// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskAttachmentDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskAttachmentDataBinder : ObjectBinder<TaskAttachmentData>
  {
    private SqlColumnBinder m_timelineId = new SqlColumnBinder("TimelineId");
    private SqlColumnBinder m_recordId = new SqlColumnBinder("RecordId");
    private SqlColumnBinder m_type = new SqlColumnBinder("Type");
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_path = new SqlColumnBinder("Path");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_lastChangedOn = new SqlColumnBinder("LastChangedOn");
    private SqlColumnBinder m_lastChangedBy = new SqlColumnBinder("LastChangedBy");

    protected override TaskAttachmentData Bind()
    {
      TaskAttachmentData taskAttachmentData = new TaskAttachmentData();
      taskAttachmentData.TimelineId = this.m_timelineId.GetGuid((IDataReader) this.Reader, false);
      taskAttachmentData.RecordId = this.m_recordId.GetGuid((IDataReader) this.Reader, false);
      taskAttachmentData.Type = this.m_type.GetString((IDataReader) this.Reader, false);
      taskAttachmentData.Name = this.m_name.GetString((IDataReader) this.Reader, false);
      taskAttachmentData.Path = this.m_path.GetString((IDataReader) this.Reader, false);
      taskAttachmentData.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      taskAttachmentData.LastChangedOn = this.m_lastChangedOn.GetDateTime((IDataReader) this.Reader);
      taskAttachmentData.LastChangedBy = this.m_lastChangedBy.GetGuid((IDataReader) this.Reader, false);
      return taskAttachmentData;
    }
  }
}
