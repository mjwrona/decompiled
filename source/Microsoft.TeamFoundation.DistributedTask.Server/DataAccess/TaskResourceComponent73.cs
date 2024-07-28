// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent73
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent73 : TaskResourceComponent72
  {
    public override ResourceLockRequest QueueResourceLockRequest(ResourceLockRequest request) => throw new NotSupportedException();

    public override ResourceLockRequest UpdateResourceLockRequest(
      long requestId,
      ResourceLockStatus status,
      DateTime? finishTime = null)
    {
      throw new NotSupportedException();
    }

    public override List<ResourceLockRequest> FreeResourceLocks(
      Guid planId,
      string nodeName,
      int nodeAttempt)
    {
      throw new NotSupportedException();
    }

    public override void CleanupResourceLockTable(int daysToKeep) => throw new NotSupportedException();

    public override List<AgentRequestData> GetAgentRequestDataFromDate(
      int batchSize,
      DateTime? fromTime)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestDataFromDate)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequestDataFromDate");
        this.BindInt("@batchSize", batchSize);
        this.BindNullableDateTime2("@fromTime", fromTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AgentRequestData>(this.GetAgentRequestDataBinder());
          return resultCollection.GetCurrent<AgentRequestData>().Items;
        }
      }
    }

    protected override ObjectBinder<AgentRequestData> GetAgentRequestDataBinder() => (ObjectBinder<AgentRequestData>) new AgentRequestDataBinder2();
  }
}
