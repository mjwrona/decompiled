// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TimelineRecordVariableBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TimelineRecordVariableBinder : ObjectBinder<TimelineRecordVariableData>
  {
    private SqlColumnBinder m_recordId = new SqlColumnBinder("RecordId");
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_isSecret = new SqlColumnBinder("IsSecret");
    private SqlColumnBinder m_value = new SqlColumnBinder("Value");

    protected override TimelineRecordVariableData Bind() => new TimelineRecordVariableData()
    {
      RecordId = this.m_recordId.GetGuid((IDataReader) this.Reader),
      Name = this.m_name.GetString((IDataReader) this.Reader, false),
      IsSecret = this.m_isSecret.GetBoolean((IDataReader) this.Reader),
      Value = this.m_value.GetString((IDataReader) this.Reader, true)
    };
  }
}
