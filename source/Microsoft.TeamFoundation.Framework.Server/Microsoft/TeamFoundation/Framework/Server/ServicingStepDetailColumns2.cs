// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepDetailColumns2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingStepDetailColumns2 : ObjectBinder<ServicingStepDetail>
  {
    private SqlColumnBinder m_queueTimeColumn = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_detailIdColumn = new SqlColumnBinder("DetailId");
    private SqlColumnBinder m_detailTimeColumn = new SqlColumnBinder("DetailTime");
    private SqlColumnBinder m_operationNameColumn = new SqlColumnBinder("OperationName");
    private SqlColumnBinder m_groupNameColumn = new SqlColumnBinder("GroupName");
    private SqlColumnBinder m_stepNameColumn = new SqlColumnBinder("StepName");
    private SqlColumnBinder m_stateColumn = new SqlColumnBinder("State");
    private SqlColumnBinder m_entryKindColumn = new SqlColumnBinder("EntryKind");
    private SqlColumnBinder m_messageColumn = new SqlColumnBinder("Message");

    protected override ServicingStepDetail Bind()
    {
      SqlDataReader reader = this.Reader;
      byte num = this.m_stateColumn.GetByte((IDataReader) reader, byte.MaxValue);
      ServicingStepDetail servicingStepDetail;
      if (num == byte.MaxValue)
        servicingStepDetail = (ServicingStepDetail) new ServicingStepLogEntry()
        {
          EntryKindDataTransfer = (int) this.m_entryKindColumn.GetByte((IDataReader) reader),
          Message = this.m_messageColumn.GetString((IDataReader) reader, true)
        };
      else
        servicingStepDetail = (ServicingStepDetail) new ServicingStepStateChange()
        {
          StepStateDataTransfer = (int) num
        };
      servicingStepDetail.QueueTime = this.m_queueTimeColumn.GetDateTime((IDataReader) reader);
      servicingStepDetail.DetailId = this.m_detailIdColumn.GetInt64((IDataReader) reader);
      servicingStepDetail.DetailTime = this.m_detailTimeColumn.GetDateTime((IDataReader) reader);
      servicingStepDetail.ServicingOperation = this.m_operationNameColumn.GetString((IDataReader) reader, false);
      servicingStepDetail.ServicingStepGroupId = this.m_groupNameColumn.GetString((IDataReader) reader, false);
      servicingStepDetail.ServicingStepId = this.m_stepNameColumn.GetString((IDataReader) reader, false);
      return servicingStepDetail;
    }
  }
}
