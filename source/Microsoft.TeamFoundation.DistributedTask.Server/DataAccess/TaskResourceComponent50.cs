// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent50
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent50 : TaskResourceComponent49
  {
    protected override ObjectBinder<TaskAgentPoolData> CreateTaskAgentPoolBinder() => (ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder10(this.RequestContext);

    protected override ObjectBinder<TaskAgentCloud> CreateTaskAgentCloudBinder() => (ObjectBinder<TaskAgentCloud>) new TaskAgentCloudBinder2();

    protected override ObjectBinder<TaskAgentCloudRequest> CreateTaskAgentCloudRequestBinder() => (ObjectBinder<TaskAgentCloudRequest>) new TaskAgentCloudRequestBinder2();

    public override async Task<TaskAgentCloud> AddAgentCloudAsync(
      Guid id,
      string name,
      string type,
      string getAgentDefinitionEndpoint,
      string acquireAgentEndpoint,
      string getAgentRequestStatusEndpoint,
      string releaseAgentEndpoint,
      string getAccountParallelismEndpoint,
      int? maxParallelism,
      int? acquisitionTimeout,
      bool internalAgentCloud = false)
    {
      TaskResourceComponent50 component = this;
      TaskAgentCloud taskAgentCloud;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddAgentCloudAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddAgentCloud");
        component.BindString("@name", name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentDefinitionEndpoint", getAgentDefinitionEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@acquireAgentEndpoint", acquireAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentRequestStatusEndpoint", getAgentRequestStatusEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@releaseAgentEndpoint", releaseAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          taskAgentCloud = resultCollection.GetCurrent<TaskAgentCloud>().First<TaskAgentCloud>();
        }
      }
      return taskAgentCloud;
    }

    public override TaskAgentPoolData AddAgentPool(
      string name,
      Guid createdBy,
      bool isHosted = false,
      bool autoProvision = false,
      bool? autoSize = true,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation,
      int? poolMetadataFileId = null,
      Guid? ownerId = null,
      int? agentCloudId = null,
      int? targetSize = null,
      bool? isLegacy = null,
      TaskAgentPoolOptions options = TaskAgentPoolOptions.None)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_AddAgentPool");
        this.BindString("@name", name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindByte("@poolType", (byte) poolType);
        this.BindGuid("@createdBy", createdBy);
        this.BindBoolean("@isHosted", isHosted);
        this.BindBoolean("@autoProvision", autoProvision);
        this.BindBoolean("@autoSize", !autoSize.HasValue || autoSize.Value);
        this.BindNullableInt("@poolMetadataFileId", poolMetadataFileId);
        this.BindNullableGuid("@ownerId", ownerId);
        this.BindNullableInt("@agentCloudId", agentCloudId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
        }
      }
    }

    public override async Task<TaskAgentCloud> DeleteAgentCloudAsync(int agentCloudId)
    {
      TaskResourceComponent50 component = this;
      TaskAgentCloud taskAgentCloud;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (DeleteAgentCloudAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_DeleteAgentCloud");
        component.BindNullableInt("@agentCloudId", new int?(agentCloudId));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          taskAgentCloud = resultCollection.GetCurrent<TaskAgentCloud>().Items.FirstOrDefault<TaskAgentCloud>();
        }
      }
      return taskAgentCloud;
    }

    public override UpdateAgentPoolResult UpdateAgentPool(
      int poolId,
      string name = null,
      Guid? createdBy = null,
      Guid? groupScopeId = null,
      Guid? administratorsGroupId = null,
      Guid? serviceAccountsGroupId = null,
      Guid? serviceIdentityId = null,
      bool? isHosted = null,
      bool? autoProvision = null,
      bool? autoSize = null,
      bool? provisioned = null,
      bool removePoolMetadata = false,
      int? poolMetadataFileId = null,
      Guid? ownerId = null,
      int? agentCloudId = null,
      bool removeAgentCloud = false,
      int? targetSize = null,
      bool? isLegacy = null,
      bool? autoUpdate = null,
      TaskAgentPoolOptions? options = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgentPool");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", name, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableGuid("@createdBy", createdBy);
        this.BindNullableGuid("@serviceIdentityId", serviceIdentityId);
        this.BindNullableBoolean("@isHosted", isHosted);
        this.BindNullableBoolean("@autoProvision", autoProvision);
        this.BindNullableBoolean("@autoSize", autoSize);
        this.BindBoolean("@removePoolMetadata", removePoolMetadata);
        this.BindNullableInt("@poolMetadataFileId", poolMetadataFileId);
        this.BindNullableGuid("@ownerId", ownerId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          UpdateAgentPoolResult updateAgentPoolResult = new UpdateAgentPoolResult();
          updateAgentPoolResult.PreviousPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          updateAgentPoolResult.UpdatedPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return updateAgentPoolResult;
        }
      }
    }
  }
}
