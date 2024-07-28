// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent69
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent69 : TaskResourceComponent68
  {
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
        this.BindString("@osDescription", agent.OSDescription, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@provisioningState", agent.ProvisioningState, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@accessPoint", agent.AccessPoint, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableBoolean("@enabled", agent.Enabled);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        this.BindNullableInt("@agentStatus", agent.Status != (TaskAgentStatus) 0 ? new int?((int) agent.Status) : new int?());
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
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
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

    public override async Task<UpdateAgentResult> UpdateAgentAsync(
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate)
    {
      TaskResourceComponent69 component = this;
      UpdateAgentResult updateAgentResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgent");
        component.BindInt("@poolId", poolId);
        component.BindInt("@agentId", agent.Id);
        component.BindString("@name", agent.Name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@version", agent.Version, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@osDescription", agent.OSDescription, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@provisioningState", agent.ProvisioningState, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@accessPoint", agent.AccessPoint, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindNullableBoolean("@enabled", agent.Enabled);
        component.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        component.BindNullableInt("@agentStatus", agent.Status != (TaskAgentStatus) 0 ? new int?((int) agent.Status) : new int?());
        if (agent.Authorization != null)
          component.BindBinary("@publicKey", JsonUtility.Serialize((object) agent.Authorization.PublicKey, false), 512, SqlDbType.VarBinary);
        else
          component.BindNullValue("@publicKey", SqlDbType.VarBinary);
        component.BindByte("@capabilityUpdate", (byte) capabilityUpdate);
        if (agent.SystemCapabilities.Count > 0)
        {
          agent.SystemCapabilities.Remove(PipelineConstants.AgentName);
          agent.SystemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        if ((capabilityUpdate & TaskAgentCapabilityType.System) == TaskAgentCapabilityType.System)
          component.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        else
          component.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        if ((capabilityUpdate & TaskAgentCapabilityType.User) == TaskAgentCapabilityType.User)
          component.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        else
          component.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        UpdateAgentResult result = new UpdateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(component.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          result.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          result.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
          resultCollection.NextResult();
          result.Agent = resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>().PopulateImplicitCapabilities();
          foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities)
            result.Agent.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
          foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
            result.Agent.UserCapabilities.Add(userCapability.Key, userCapability.Value);
          updateAgentResult = result;
        }
      }
      return updateAgentResult;
    }
  }
}
