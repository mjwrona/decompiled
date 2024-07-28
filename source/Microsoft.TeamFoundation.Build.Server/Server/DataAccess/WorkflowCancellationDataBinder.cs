// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.WorkflowCancellationDataBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class WorkflowCancellationDataBinder : BuildObjectBinder<WorkflowCancellationData>
  {
    private SqlColumnBinder agentDisplayNameColumn = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder reservationIdColumn = new SqlColumnBinder("ReservationId");
    private SqlColumnBinder reservedAgentIdColumn = new SqlColumnBinder("ReservedAgentId");
    private SqlColumnBinder messageQueueUrlColumn = new SqlColumnBinder("MessageQueueUrl");
    private SqlColumnBinder endpointUrlColumn = new SqlColumnBinder("EndpointUrl");

    protected override WorkflowCancellationData Bind() => new WorkflowCancellationData()
    {
      AgentDisplayName = this.agentDisplayNameColumn.GetBuildItem(this.Reader, false),
      ReservationId = this.reservationIdColumn.GetInt32((IDataReader) this.Reader),
      ReservedAgentId = this.reservedAgentIdColumn.GetInt32((IDataReader) this.Reader),
      MessageQueueUrl = DBHelper.DBUrlToServerUrl(this.messageQueueUrlColumn.GetString((IDataReader) this.Reader, false)),
      EndpointUrl = this.endpointUrlColumn.ColumnExists((IDataReader) this.Reader) ? DBHelper.DBUrlToServerUrl(this.endpointUrlColumn.GetString((IDataReader) this.Reader, false)) : (string) null
    };
  }
}
