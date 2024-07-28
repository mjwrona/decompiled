// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ResourceLockComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ResourceLockComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<ResourceLockComponent>(1),
      (IComponentCreator) new ComponentCreator<ResourceLockComponent2>(2),
      (IComponentCreator) new ComponentCreator<ResourceLockComponent3>(3),
      (IComponentCreator) new ComponentCreator<ResourceLockComponent4>(4),
      (IComponentCreator) new ComponentCreator<ResourceLockComponent5>(5)
    }, "DistributedTaskResourceLock", "DistributedTask");
    protected static SqlMetaData[] typ_ResourceTable = new SqlMetaData[2]
    {
      new SqlMetaData("ResourceId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("ResourceType", SqlDbType.NVarChar, 64L)
    };
    protected static SqlMetaData[] typ_ResourceLockRequestTable = new SqlMetaData[13]
    {
      new SqlMetaData("RequestId", SqlDbType.BigInt),
      new SqlMetaData("ResourceId", SqlDbType.NVarChar, 400L),
      new SqlMetaData("ResourceType", SqlDbType.NVarChar, 64L),
      new SqlMetaData("PlanId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NodeName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("NodeAttempt", SqlDbType.Int),
      new SqlMetaData("ProjectId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CheckRunId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("QueueTime", SqlDbType.DateTime2),
      new SqlMetaData("AssignTime", SqlDbType.DateTime2),
      new SqlMetaData("FinishTime", SqlDbType.DateTime2)
    };

    public ResourceLockComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual List<ResourceLockRequest> QueueResourceLockRequests(
      IEnumerable<ResourceLockRequest> requests)
    {
      List<ResourceLockRequest> resourceLockRequestList = new List<ResourceLockRequest>();
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (QueueResourceLockRequests)))
      {
        this.PrepareStoredProcedure("Task.prc_QueueResourceLockRequests");
        this.BindResourceLockTable("@requests", requests);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>(this.CreateResourceLockRequestBinder());
          resourceLockRequestList.AddRange((IEnumerable<ResourceLockRequest>) resultCollection.GetCurrent<ResourceLockRequest>().Items);
        }
      }
      return resourceLockRequestList;
    }

    public virtual List<ResourceLockRequest> UpdateResourceLockRequests(
      IEnumerable<ResourceLockRequest> resourceLocks)
    {
      List<ResourceLockRequest> resourceLockRequestList = new List<ResourceLockRequest>();
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateResourceLockRequests)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateResourceLockRequests");
        this.BindResourceLockTable("@requests", resourceLocks);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>(this.CreateResourceLockRequestBinder());
          resourceLockRequestList.AddRange((IEnumerable<ResourceLockRequest>) resultCollection.GetCurrent<ResourceLockRequest>().Items);
        }
      }
      return resourceLockRequestList;
    }

    public virtual async Task<List<ResourceLockRequest>> FreeResourceLocksAsync(
      Guid planId,
      string nodeName = null,
      int? nodeAttempt = null)
    {
      ResourceLockComponent component = this;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FreeResourceLocksAsync)))
      {
        if (nodeName == null || !nodeAttempt.HasValue)
          return new List<ResourceLockRequest>();
        component.PrepareStoredProcedure("Task.prc_FreeResourceLockRequests");
        component.BindGuid("@planId", planId);
        component.BindString("@nodeName", nodeName, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindInt("@nodeAttempt", nodeAttempt.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>(component.CreateResourceLockRequestBinder());
          return resultCollection.GetCurrent<ResourceLockRequest>().Items;
        }
      }
    }

    public virtual async Task<AssignResourceLockRequestsResult> GetAssignableResourceLockRequests()
    {
      ResourceLockComponent component = this;
      AssignResourceLockRequestsResult resourceLockRequests;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAssignableResourceLockRequests)))
      {
        AssignResourceLockRequestsResult result = new AssignResourceLockRequestsResult();
        component.PrepareStoredProcedure("Task.prc_GetAssignableResourceLockRequests");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>(component.CreateResourceLockRequestBinder());
          resultCollection.AddBinder<ResourceLockRequest>(component.CreateResourceLockRequestBinder());
          result.AssignedRequests.AddRange((IEnumerable<ResourceLockRequest>) resultCollection.GetCurrent<ResourceLockRequest>().Items);
          resultCollection.NextResult();
          result.CanceledRequests.AddRange((IEnumerable<ResourceLockRequest>) resultCollection.GetCurrent<ResourceLockRequest>().Items);
          resourceLockRequests = result;
        }
      }
      return resourceLockRequests;
    }

    public virtual ResourceLockRequest GetResourceLockRequestByCheckRun(
      Guid projectId,
      Guid checkRunId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetResourceLockRequestByCheckRun)))
      {
        this.PrepareStoredProcedure("Task.prc_GetResourceLockRequestByCheckRun");
        this.BindGuid("@projectId", projectId);
        this.BindGuid("@checkRunId", checkRunId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>(this.CreateResourceLockRequestBinder());
          return resultCollection.GetCurrent<ResourceLockRequest>().FirstOrDefault<ResourceLockRequest>();
        }
      }
    }

    public virtual List<ResourceLockRequest> GetActiveResourceLocks(
      IEnumerable<CheckResource> resources)
    {
      List<ResourceLockRequest> activeResourceLocks = new List<ResourceLockRequest>();
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetActiveResourceLocks)))
      {
        this.PrepareStoredProcedure("Task.prc_GetActiveResourceLocks");
        this.BindCheckResourceTable("@resources", resources);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ResourceLockRequest>(this.CreateResourceLockRequestBinder());
          activeResourceLocks.AddRange((IEnumerable<ResourceLockRequest>) resultCollection.GetCurrent<ResourceLockRequest>().Items);
        }
      }
      return activeResourceLocks;
    }

    public virtual void CleanupResourceLockTable(int daysToKeep)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CleanupResourceLockTable)))
      {
        this.PrepareStoredProcedure("Task.prc_CleanupResourceLockRequests");
        this.BindInt("@daysToKeep", daysToKeep);
        this.ExecuteNonQuery();
      }
    }

    public virtual List<ResourceLockRequest> GetResourceLockRequestsByCheckRuns(
      Guid projectId,
      IEnumerable<Guid> checkRunIds)
    {
      return new List<ResourceLockRequest>();
    }

    protected virtual SqlParameter BindCheckResourceTable(
      string parameterName,
      IEnumerable<CheckResource> rows)
    {
      return this.BindTable(parameterName, "Task.typ_ResourceTable", (rows ?? Enumerable.Empty<CheckResource>()).Select<CheckResource, SqlDataRecord>(new System.Func<CheckResource, SqlDataRecord>(this.ConvertCheckResourceToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertCheckResourceToSqlDataRecord(CheckResource row)
    {
      SqlDataRecord record = new SqlDataRecord(ResourceLockComponent.typ_ResourceTable);
      record.SetString(0, row.Id, BindStringBehavior.Unchanged);
      record.SetString(1, row.Type, BindStringBehavior.Unchanged);
      return record;
    }

    protected virtual SqlParameter BindResourceLockTable(
      string parameterName,
      IEnumerable<ResourceLockRequest> rows)
    {
      return this.BindTable(parameterName, "Task.typ_ResourceLockRequestTable", (rows ?? Enumerable.Empty<ResourceLockRequest>()).Select<ResourceLockRequest, SqlDataRecord>(new System.Func<ResourceLockRequest, SqlDataRecord>(this.ConvertLockRequestToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertLockRequestToSqlDataRecord(ResourceLockRequest row)
    {
      SqlDataRecord record = new SqlDataRecord(ResourceLockComponent.typ_ResourceLockRequestTable);
      record.SetInt64(0, row.RequestId);
      record.SetString(1, row.ResourceId, BindStringBehavior.Unchanged);
      record.SetString(2, row.ResourceType, BindStringBehavior.Unchanged);
      record.SetGuid(3, row.PlanId);
      record.SetString(4, row.NodeName, BindStringBehavior.Unchanged);
      record.SetInt32(5, row.NodeAttempt);
      record.SetGuid(6, row.ProjectId);
      record.SetGuid(7, row.CheckRunId);
      record.SetInt32(8, row.DefinitionId);
      record.SetByte(9, (byte) row.Status);
      record.SetDateTime(10, row.QueueTime);
      record.SetNullableDateTime(11, row.AssignTime);
      record.SetNullableDateTime(12, row.FinishTime);
      return record;
    }

    public virtual List<ResourceLockRequest> QueueResourceLockRequestsV2(
      IEnumerable<ResourceLockRequest> requests)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 5);
    }

    protected virtual ObjectBinder<ResourceLockRequest> CreateResourceLockRequestBinder() => (ObjectBinder<ResourceLockRequest>) new ResourceLockRequestBinder();

    private IDisposable TraceScope([CallerMemberName] string method = null) => this.RequestContext.TraceScope(this.TraceLayer, method);

    private string TraceLayer => nameof (ResourceLockComponent);
  }
}
