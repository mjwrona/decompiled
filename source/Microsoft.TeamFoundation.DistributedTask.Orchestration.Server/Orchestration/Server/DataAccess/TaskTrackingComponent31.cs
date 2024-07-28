// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent31
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent31 : TaskTrackingComponent30
  {
    public override async Task<PlanLogTable> CreateLogTableAsync(
      Guid scopeIdentifier,
      Guid planId,
      string storageKey,
      string tableName,
      DateTime startedOn,
      DateTime expiryOn)
    {
      TaskTrackingComponent31 component = this;
      PlanLogTable logTableAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (CreateLogTableAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_CreateLogTable");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindString("@storageKey", storageKey, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindString("@tableName", tableName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindDateTime2("@startedOn", startedOn);
        component.BindDateTime2("@expiryOn", expiryOn);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<PlanLogTable>((ObjectBinder<PlanLogTable>) new PlanLogTableBinder());
          logTableAsync = resultCollection.GetCurrent<PlanLogTable>().FirstOrDefault<PlanLogTable>();
        }
      }
      return logTableAsync;
    }

    public override async Task<PlanLogTable> UpdateLogTableAsync(
      Guid scopeIdentifier,
      Guid planId,
      PlanLogTable logTable)
    {
      TaskTrackingComponent31 component = this;
      PlanLogTable planLogTable;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateLogTableAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateLogTable");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindString("@storageKey", logTable.StorageKey, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindString("@tableName", logTable.TableName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindNullableDateTime2("@startedOn", logTable.StartedOn);
        component.BindNullableDateTime2("@completedOn", logTable.CompletedOn);
        component.BindNullableDateTime2("@expiryOn", logTable.ExpiryOn);
        component.BindNullableDateTime2("@deletedOn", logTable.DeletedOn);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<PlanLogTable>((ObjectBinder<PlanLogTable>) new PlanLogTableBinder());
          planLogTable = resultCollection.GetCurrent<PlanLogTable>().FirstOrDefault<PlanLogTable>();
        }
      }
      return planLogTable;
    }

    public override async Task<PlanLogTable> GetLogTableAsync(Guid scopeIdentifier, Guid planId)
    {
      TaskTrackingComponent31 component = this;
      PlanLogTable logTableAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetLogTableAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetLogTable");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<PlanLogTable>((ObjectBinder<PlanLogTable>) new PlanLogTableBinder());
          logTableAsync = resultCollection.GetCurrent<PlanLogTable>().FirstOrDefault<PlanLogTable>();
        }
      }
      return logTableAsync;
    }

    public override async Task<IList<PlanLogTable>> GetExpiredLogTablesAsync(int maxCount)
    {
      TaskTrackingComponent31 component = this;
      IList<PlanLogTable> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetExpiredLogTablesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetExpiredLogTables");
        component.BindInt("@maxCount", maxCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<PlanLogTable>((ObjectBinder<PlanLogTable>) new PlanLogTableBinder());
          items = (IList<PlanLogTable>) resultCollection.GetCurrent<PlanLogTable>().Items;
        }
      }
      return items;
    }

    public override async Task DeleteLogTablesAsync(IList<string> tableNames)
    {
      TaskTrackingComponent31 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (DeleteLogTablesAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_DeleteLogTables");
        component.BindStringTable("@tableNames", tableNames.Distinct<string>((IEqualityComparer<string>) StringComparer.Ordinal));
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }
  }
}
