// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent68
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent68 : TaskResourceComponent67
  {
    protected static SqlMetaData[] typ_GuidGuidTable = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.UniqueIdentifier)
    };

    public override List<AgentRequestData> GetAgentRequestData(
      List<TaskTimelineRecord> timelineRecords)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestData)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequestData");
        this.BindTable("@jobIds", "typ_GuidGuidTable", timelineRecords.Select<TaskTimelineRecord, SqlDataRecord>(new System.Func<TaskTimelineRecord, SqlDataRecord>(this.ConvertToSqlDataRecord)));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AgentRequestData>(this.GetAgentRequestDataBinder());
          return resultCollection.GetCurrent<AgentRequestData>().Items;
        }
      }
    }

    protected SqlDataRecord ConvertToSqlDataRecord(TaskTimelineRecord row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(TaskResourceComponent68.typ_GuidGuidTable);
      sqlDataRecord.SetGuid(0, row.PlanGuidId);
      sqlDataRecord.SetGuid(1, row.TimelineRecordGuid);
      return sqlDataRecord;
    }

    protected virtual ObjectBinder<AgentRequestData> GetAgentRequestDataBinder() => (ObjectBinder<AgentRequestData>) new AgentRequestDataBinder();
  }
}
