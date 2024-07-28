// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemDataRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemDataRecordBinder : ObjectBinder<WorkItemDataRecord>
  {
    private SqlColumnBinder m_authorizedDateColumn = new SqlColumnBinder("AuthorizedDate");
    private SqlColumnBinder m_revisedDateColumn = new SqlColumnBinder("RevisedDate");
    private SqlColumnBinder m_valueColumn = new SqlColumnBinder("Value");
    private SqlColumnBinder m_idColumn = new SqlColumnBinder("Id");

    protected override WorkItemDataRecord Bind()
    {
      WorkItemDataRecord workItemDataRecord = new WorkItemDataRecord();
      workItemDataRecord.AuthorizedDate = this.m_authorizedDateColumn.GetDateTime(this.BaseReader);
      workItemDataRecord.RevisedDate = this.m_revisedDateColumn.GetDateTime(this.BaseReader);
      workItemDataRecord.Id = this.m_idColumn.GetInt32(this.BaseReader);
      workItemDataRecord.Value = this.m_valueColumn.GetString(this.BaseReader, true);
      return workItemDataRecord;
    }
  }
}
