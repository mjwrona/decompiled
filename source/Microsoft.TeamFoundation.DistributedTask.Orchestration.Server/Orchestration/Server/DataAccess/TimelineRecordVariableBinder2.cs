// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TimelineRecordVariableBinder2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TimelineRecordVariableBinder2 : TimelineRecordVariableBinder
  {
    private SqlColumnBinder m_timelineId = new SqlColumnBinder("TimelineId");

    protected override TimelineRecordVariableData Bind()
    {
      TimelineRecordVariableData recordVariableData = base.Bind();
      if (this.m_timelineId.ColumnExists((IDataReader) this.Reader))
        recordVariableData.TimelineId = this.m_timelineId.GetNullableGuid((IDataReader) this.Reader);
      return recordVariableData;
    }
  }
}
