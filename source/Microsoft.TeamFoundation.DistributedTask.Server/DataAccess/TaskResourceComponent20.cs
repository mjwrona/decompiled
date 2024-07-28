// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent20
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent20 : TaskResourceComponent19
  {
    public override CreateAgentResult AddAgent(int poolId, TaskAgent agent, bool createEnabled = true)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgent)))
      {
        this.PrepareStoredProcedure("Task.prc_AddAgent");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@version", agent.Version, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        if (agent.Authorization != null)
          this.BindBinary("@publicKey", JsonUtility.Serialize((object) agent.Authorization.PublicKey, false), 512, SqlDbType.VarBinary);
        else
          this.BindNullValue("@publicKey", SqlDbType.VarBinary);
        if (agent.SystemCapabilities.Count > 0)
        {
          agent.SystemCapabilities.Remove(PipelineConstants.AgentName);
          agent.SystemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        CreateAgentResult createAgentResult = new CreateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
          createAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
          resultCollection.NextResult();
          createAgentResult.Agent = resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>().PopulateImplicitCapabilities();
          foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities)
            createAgentResult.Agent.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
          foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
            createAgentResult.Agent.UserCapabilities.Add(userCapability.Key, userCapability.Value);
          return createAgentResult;
        }
      }
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
        this.BindGuid("@createdBy", createdBy);
        this.BindBoolean("@isHosted", isHosted);
        this.BindBoolean("@autoProvision", autoProvision);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
        }
      }
    }

    public override DeleteAgentPoolResult DeleteAgentPool(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAgentPool");
        this.BindInt("@poolId", poolId);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentPoolResult deleteAgentPoolResult = new DeleteAgentPoolResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder2());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          deleteAgentPoolResult.DeletedAgents = (IList<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.DeletedSessions = (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return deleteAgentPoolResult;
        }
      }
    }

    public override TaskAgentPoolData GetAgentPool(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPool");
        this.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
        }
      }
    }

    public override IEnumerable<TaskAgentPoolData> GetAgentPools(
      string poolName,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPools)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPools");
        this.BindString("@poolName", poolName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return (IEnumerable<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
        }
      }
    }

    public override async Task<IList<TaskAgentPoolData>> GetAgentPoolsWithCapabilityChangesAsync()
    {
      TaskResourceComponent20 component = this;
      IList<TaskAgentPoolData> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentPoolsWithCapabilityChangesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentPoolsWithCapabilityChanges");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(component.CreateTaskAgentPoolBinder());
          items = (IList<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
        }
      }
      return items;
    }

    public override UpdateAgentResult UpdateAgent(
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgent)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgent");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agent.Id);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@version", agent.Version, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableBoolean("@enabled", agent.Enabled);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        if (agent.Authorization != null)
          this.BindBinary("@publicKey", JsonUtility.Serialize((object) agent.Authorization.PublicKey, false), 512, SqlDbType.VarBinary);
        else
          this.BindNullValue("@publicKey", SqlDbType.VarBinary);
        this.BindByte("@capabilityUpdate", (byte) capabilityUpdate);
        if (agent.SystemCapabilities.Count > 0)
        {
          agent.SystemCapabilities.Remove(PipelineConstants.AgentName);
          agent.SystemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        if ((capabilityUpdate & TaskAgentCapabilityType.System) == TaskAgentCapabilityType.System)
          this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        else
          this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        if ((capabilityUpdate & TaskAgentCapabilityType.User) == TaskAgentCapabilityType.User)
          this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        else
          this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        UpdateAgentResult updateAgentResult = new UpdateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
          updateAgentResult.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          updateAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
          resultCollection.NextResult();
          updateAgentResult.Agent = resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>().PopulateImplicitCapabilities();
          foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities)
            updateAgentResult.Agent.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
          foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
            updateAgentResult.Agent.UserCapabilities.Add(userCapability.Key, userCapability.Value);
          return updateAgentResult;
        }
      }
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
        this.BindNullableGuid("@groupScopeId", groupScopeId);
        this.BindNullableGuid("@administratorsGroupId", administratorsGroupId);
        this.BindNullableGuid("@serviceAccountsGroupId", serviceAccountsGroupId);
        this.BindNullableGuid("@serviceIdentityId", serviceIdentityId);
        this.BindNullableBoolean("@isHosted", isHosted);
        this.BindNullableBoolean("@autoProvision", autoProvision);
        this.BindNullableBoolean("@provisioned", provisioned);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          UpdateAgentPoolResult updateAgentPoolResult = new UpdateAgentPoolResult();
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          updateAgentPoolResult.UpdatedPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return updateAgentPoolResult;
        }
      }
    }

    protected virtual ObjectBinder<TaskAgentPoolData> CreateTaskAgentPoolBinder() => (ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder6(this.RequestContext);
  }
}
