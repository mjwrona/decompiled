// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VirtualMachineGroupComponent
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
  internal class VirtualMachineGroupComponent : 
    TaskSqlComponentBase,
    IEnvironmentResourceComponent<VirtualMachineGroup>,
    IDisposable
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<VirtualMachineGroupComponent>(1),
      (IComponentCreator) new ComponentCreator<VirtualMachineGroupComponent2>(2)
    }, "DistributedTaskVirtualMachineGroup", "DistributedTask");

    public VirtualMachineGroupComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual VirtualMachineGroup AddEnvironmentResource(
      Guid projectId,
      VirtualMachineGroup resource)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_AddVirtualMachineGroup");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", resource.EnvironmentReference.Id);
        this.BindString("@name", resource.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@poolId", resource.PoolId);
        this.BindGuid("@createdBy", new Guid(resource.CreatedBy.Id));
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual List<VirtualMachineGroup> GetEnvironmentResources(
      Guid projectId,
      int environmentId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentResources)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVirtualMachineGroups");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VirtualMachineGroup>((ObjectBinder<VirtualMachineGroup>) new VirtualMachineGroupBinder());
          return resultCollection.GetCurrent<VirtualMachineGroup>().Items;
        }
      }
    }

    public virtual VirtualMachineGroup GetEnvironmentResource(
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVirtualMachineGroup");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@resourceId", resourceId);
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual IList<VirtualMachineGroup> GetEnvironmentResourcesById(
      Guid projectId,
      int environmentId,
      IEnumerable<int> resourceIds)
    {
      return (IList<VirtualMachineGroup>) new List<VirtualMachineGroup>();
    }

    public virtual IList<VirtualMachineGroup> GetVirtualMachineGroupsByPoolIds(
      IEnumerable<int> poolIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetVirtualMachineGroupsByPoolIds)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVirtualMachineGroupsByPoolIds");
        this.BindInt32Table("@poolIds", poolIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VirtualMachineGroup>((ObjectBinder<VirtualMachineGroup>) new VirtualMachineGroupBinder());
          return (IList<VirtualMachineGroup>) resultCollection.GetCurrent<VirtualMachineGroup>().ToArray<VirtualMachineGroup>();
        }
      }
    }

    public virtual VirtualMachineGroup UpdateEnvironmentResource(
      Guid projectId,
      VirtualMachineGroup resource)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateVirtualMachineGroup");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", resource.EnvironmentReference.Id);
        this.BindInt("@resourceId", resource.Id);
        this.BindString("@name", resource.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(resource.LastModifiedBy.Id));
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual VirtualMachineGroup DeleteEnvironmentResource(
      Guid projectId,
      int environmentId,
      int resourceId,
      Guid deletedBy)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteVirtualMachineGroup");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@resourceId", resourceId);
        this.BindGuid("@deletedBy", deletedBy);
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual IList<VirtualMachineGroup> DeleteEnvironmentResources(
      Guid projectId,
      int environmentId,
      Guid deletedBy)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteEnvironmentResources)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAllVirtualMachineGroups");
        this.BindInt("@environmentId", environmentId);
        this.BindGuid("@deletedBy", deletedBy);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VirtualMachineGroup>((ObjectBinder<VirtualMachineGroup>) new VirtualMachineGroupBinder());
          return (IList<VirtualMachineGroup>) resultCollection.GetCurrent<VirtualMachineGroup>().Items;
        }
      }
    }

    public GetVirtualMachinesResult GetVirtualMachines(
      Guid projectId,
      int environmentId,
      int resourceId,
      IEnumerable<string> tagFilters)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetVirtualMachines)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVirtualMachines");
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@resourceId", resourceId);
        this.BindStringTable("@tagFilters", tagFilters != null ? tagFilters.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        GetVirtualMachinesResult virtualMachines = new GetVirtualMachinesResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VirtualMachineData>((ObjectBinder<VirtualMachineData>) new VirtualMachineDataBinder());
          resultCollection.AddBinder<VirtualMachineTag>((ObjectBinder<VirtualMachineTag>) new VirtualMachineTagBinder());
          resultCollection.AddBinder<VirtualMachineGroup>((ObjectBinder<VirtualMachineGroup>) new VirtualMachineGroupBinder());
          List<VirtualMachineData> items = resultCollection.GetCurrent<VirtualMachineData>().Items;
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<VirtualMachineTag>().Items.ToLookup<VirtualMachineTag, int, string>((System.Func<VirtualMachineTag, int>) (t => t.VMAgentId), (System.Func<VirtualMachineTag, string>) (t => t.Tag));
          virtualMachines.VirtualMachines = this.CreateVirtualMachinesWithTags((IEnumerable<VirtualMachineData>) items, lookup, environmentId, resourceId);
          resultCollection.NextResult();
          virtualMachines.VirtualMachineGroup = resultCollection.GetCurrent<VirtualMachineGroup>().FirstOrDefault<VirtualMachineGroup>((System.Func<VirtualMachineGroup, bool>) (sg => sg.Id == resourceId));
        }
        return virtualMachines;
      }
    }

    public virtual UpdateVirtualMachinesResult UpdateVirtualMachines(
      Guid projectId,
      int environmentId,
      int resourceId,
      IEnumerable<VirtualMachine> virtualMachines)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateVirtualMachines)))
      {
        IList<int> machineIdsForTagDeletion = (IList<int>) null;
        UpdateVirtualMachinesResult virtualMachinesResult = new UpdateVirtualMachinesResult();
        IList<KeyValuePair<int, string>> machinesTagsTable = this.GetVirtualMachinesTagsTable(virtualMachines, out machineIdsForTagDeletion);
        this.PrepareStoredProcedure("Task.prc_UpdateVirtualMachines");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@resourceId", resourceId);
        this.BindKeyValuePairInt32StringTable("@tagsTable", (IEnumerable<KeyValuePair<int, string>>) machinesTagsTable);
        this.BindUniqueInt32Table("@agentIdsForTagDeletion", (IEnumerable<int>) machineIdsForTagDeletion);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<VirtualMachineGroup>((ObjectBinder<VirtualMachineGroup>) new VirtualMachineGroupBinder());
          resultCollection.AddBinder<VirtualMachineData>((ObjectBinder<VirtualMachineData>) new VirtualMachineDataBinder());
          resultCollection.AddBinder<VirtualMachineTag>((ObjectBinder<VirtualMachineTag>) new VirtualMachineTagBinder());
          virtualMachinesResult.VirtualMachineGroup = resultCollection.GetCurrent<VirtualMachineGroup>().FirstOrDefault<VirtualMachineGroup>((System.Func<VirtualMachineGroup, bool>) (sg => sg.Id == resourceId));
          resultCollection.NextResult();
          List<VirtualMachineData> items = resultCollection.GetCurrent<VirtualMachineData>().Items;
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<VirtualMachineTag>().Items.ToLookup<VirtualMachineTag, int, string>((System.Func<VirtualMachineTag, int>) (t => t.VMAgentId), (System.Func<VirtualMachineTag, string>) (t => t.Tag));
          virtualMachinesResult.VirtualMachines = this.CreateVirtualMachinesWithTags((IEnumerable<VirtualMachineData>) items, lookup, environmentId, resourceId);
        }
        return virtualMachinesResult;
      }
    }

    protected virtual IEnumerable<VirtualMachine> CreateVirtualMachinesWithTags(
      IEnumerable<VirtualMachineData> mappings,
      ILookup<int, string> tagsWithMappingIdLookup,
      int environmentId,
      int resourceId)
    {
      return mappings.Where<VirtualMachineData>((System.Func<VirtualMachineData, bool>) (m => m.EnvironmentId == environmentId && m.ResourceId == resourceId)).Select<VirtualMachineData, VirtualMachine>((System.Func<VirtualMachineData, VirtualMachine>) (m =>
      {
        return new VirtualMachine()
        {
          Id = m.AgentId,
          Agent = new TaskAgent() { Id = m.AgentId },
          Tags = (IList<string>) tagsWithMappingIdLookup[m.VMAgentId].ToList<string>()
        };
      }));
    }

    protected virtual IList<KeyValuePair<int, string>> GetVirtualMachinesTagsTable(
      IEnumerable<VirtualMachine> virtualMachines,
      out IList<int> machineIdsForTagDeletion)
    {
      List<KeyValuePair<int, string>> machinesTagsTable = new List<KeyValuePair<int, string>>();
      machineIdsForTagDeletion = (IList<int>) new List<int>();
      if (virtualMachines != null)
      {
        foreach (VirtualMachine virtualMachine in virtualMachines)
        {
          VirtualMachine vm = virtualMachine;
          if (vm != null && vm.Tags != null)
          {
            if (vm.Tags.Count > 0)
              machinesTagsTable.AddRange(vm.Tags.Select<string, KeyValuePair<int, string>>((System.Func<string, KeyValuePair<int, string>>) (t => new KeyValuePair<int, string>(vm.Id, t))));
            else
              machineIdsForTagDeletion.Add(vm.Id);
          }
        }
      }
      return (IList<KeyValuePair<int, string>>) machinesTagsTable;
    }

    protected VirtualMachineGroup ExecuteStoredProcedure()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<VirtualMachineGroup>((ObjectBinder<VirtualMachineGroup>) new VirtualMachineGroupBinder());
        return resultCollection.GetCurrent<VirtualMachineGroup>().Items.FirstOrDefault<VirtualMachineGroup>();
      }
    }
  }
}
