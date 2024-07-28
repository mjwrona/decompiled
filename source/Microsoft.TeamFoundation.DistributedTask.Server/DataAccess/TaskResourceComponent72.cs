// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent72
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent72 : TaskResourceComponent71
  {
    public override ResourceLockRequest QueueResourceLockRequest(ResourceLockRequest request)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (QueueResourceLockRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_QueueResourceLockRequest");
        this.BindGuid("@planId", request.PlanId);
        this.BindString("@resourceId", request.ResourceId, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindString("@resourceType", request.ResourceType, 64, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindString("@nodeName", request.NodeName, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindInt("@nodeAttempt", request.NodeAttempt);
        this.BindGuid("@projectId", request.ProjectId);
        this.BindGuid("@checkRunId", request.CheckRunId);
        this.BindInt("@definitionId", request.DefinitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>((ObjectBinder<ResourceLockRequest>) new ResourceLockRequestBinder());
          return resultCollection.GetCurrent<ResourceLockRequest>().FirstOrDefault<ResourceLockRequest>();
        }
      }
    }

    public override ResourceLockRequest UpdateResourceLockRequest(
      long requestId,
      ResourceLockStatus status,
      DateTime? finishTime = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateResourceLockRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateResourceLockRequest");
        this.BindLong("@requestId", requestId);
        this.BindInt("@status", (int) (byte) status);
        if (finishTime.HasValue)
          this.BindDateTime2("@finishTime", finishTime.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>((ObjectBinder<ResourceLockRequest>) new ResourceLockRequestBinder());
          return resultCollection.GetCurrent<ResourceLockRequest>().FirstOrDefault<ResourceLockRequest>();
        }
      }
    }

    public override List<ResourceLockRequest> FreeResourceLocks(
      Guid planId,
      string nodeName,
      int nodeAttempt)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (FreeResourceLocks)))
      {
        this.PrepareStoredProcedure("Task.prc_FreeResourceLockRequests");
        this.BindGuid("@planId", planId);
        this.BindString("@nodeName", nodeName, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@nodeAttempt", nodeAttempt);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>((ObjectBinder<ResourceLockRequest>) new ResourceLockRequestBinder());
          return resultCollection.GetCurrent<ResourceLockRequest>().Items;
        }
      }
    }

    public override void CleanupResourceLockTable(int daysToKeep)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CleanupResourceLockTable)))
      {
        this.PrepareStoredProcedure("Task.prc_CleanupResourceLockRequests");
        this.BindInt("@daysToKeep", daysToKeep);
        this.ExecuteNonQuery();
      }
    }
  }
}
