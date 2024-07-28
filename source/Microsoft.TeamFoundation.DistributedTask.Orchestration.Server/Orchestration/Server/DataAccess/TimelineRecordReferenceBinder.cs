// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TimelineRecordReferenceBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TimelineRecordReferenceBinder : ObjectBinder<TimelineRecordReference>
  {
    private SqlColumnBinder m_recordId = new SqlColumnBinder("RecordId");
    private SqlColumnBinder m_state = new SqlColumnBinder("State");
    private SqlColumnBinder m_identifier = new SqlColumnBinder("Identifier");

    protected override TimelineRecordReference Bind() => new TimelineRecordReference()
    {
      Id = this.m_recordId.GetGuid((IDataReader) this.Reader),
      State = new TimelineRecordState?((TimelineRecordState) this.m_state.GetByte((IDataReader) this.Reader)),
      Identifier = this.m_identifier.GetString((IDataReader) this.Reader, true)
    };
  }
}
