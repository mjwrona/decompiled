// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VirtualMachineResourceComponent
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
  internal class VirtualMachineResourceComponent : 
    TaskSqlComponentBase,
    IEnvironmentResourceComponent<VirtualMachineResource>,
    IDisposable
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<VirtualMachineResourceComponent>(1)
    }, "DistributedTaskVirtualMachineResource", "DistributedTask");

    public VirtualMachineResourceComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual VirtualMachineResource AddEnvironmentResource(
      Guid projectId,
      VirtualMachineResource resource)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_AddVirtualMachineResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", resource.EnvironmentReference.Id);
        this.BindString("@name", resource.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@agentId", resource.Agent.Id);
        this.BindGuid("@createdBy", new Guid(resource.CreatedBy.Id));
        this.BindStringTable("@tags", (IEnumerable<string>) resource.Tags.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>());
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual VirtualMachineResource GetEnvironmentResource(
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVirtualMachineResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@resourceId", resourceId);
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual IList<VirtualMachineResource> GetEnvironmentResourcesById(
      Guid projectId,
      int environmentId,
      IEnumerable<int> resourceIds)
    {
      this.PrepareStoredProcedure("Task.prc_GetVirtualMachineResourcesById");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
      this.BindInt("@environmentId", environmentId);
      this.BindUniqueInt32Table("@resourceIds", resourceIds);
      return this.ExecuteBatchStoredProcedure();
    }

    public virtual VirtualMachineResource UpdateEnvironmentResource(
      Guid projectId,
      VirtualMachineResource resource)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateVirtualMachineResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", resource.EnvironmentReference.Id);
        this.BindInt("@resourceId", resource.Id);
        this.BindGuid("@modifiedBy", new Guid(resource.LastModifiedBy.Id));
        this.BindString("@name", resource.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindStringTable("@tags", (IEnumerable<string>) resource.Tags.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>());
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual VirtualMachineResource DeleteEnvironmentResource(
      Guid projectId,
      int environmentId,
      int resourceId,
      Guid deletedBy)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteVirtualMachineResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@resourceId", resourceId);
        this.BindGuid("@deletedBy", deletedBy);
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual IList<VirtualMachineResource> DeleteEnvironmentResources(
      Guid projectId,
      int environmentId,
      Guid deletedBy)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteEnvironmentResources)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAllVirtualMachineResources");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindGuid("@deletedBy", deletedBy);
        return this.ExecuteBatchStoredProcedure();
      }
    }

    public virtual IList<VirtualMachineResource> GetEnvironmentResources(
      Guid projectId,
      int environmentId,
      string resourceName,
      IList<string> tagFilters = null,
      int top = 50,
      string continuationToken = "")
    {
      top = top > 1000 ? 1000 : top;
      tagFilters = tagFilters ?? (IList<string>) new List<string>();
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentResources)))
      {
        this.PrepareStoredProcedure("Task.prc_GetVirtualMachineResources");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindString("@resourceName", resourceName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindStringTable("@tags", (IEnumerable<string>) tagFilters);
        this.BindInt("@top", top);
        this.BindString("@continuationToken", continuationToken, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        return this.ExecuteBatchStoredProcedure();
      }
    }

    protected virtual VirtualMachineResource ExecuteStoredProcedure()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<VirtualMachineResource>((ObjectBinder<VirtualMachineResource>) new VirtualMachineResourceBinder());
        resultCollection.AddBinder<EnvironmentResourceTag>((ObjectBinder<EnvironmentResourceTag>) new EnvironmentResourceTagBinder());
        VirtualMachineResource virtualMachineResource = resultCollection.GetCurrent<VirtualMachineResource>().Items.FirstOrDefault<VirtualMachineResource>();
        if (virtualMachineResource == null)
          return (VirtualMachineResource) null;
        resultCollection.NextResult();
        List<string> list = resultCollection.GetCurrent<EnvironmentResourceTag>().Items.Select<EnvironmentResourceTag, string>((System.Func<EnvironmentResourceTag, string>) (x => x.Tag)).Distinct<string>().Where<string>((System.Func<string, bool>) (t => !string.IsNullOrWhiteSpace(t))).ToList<string>();
        virtualMachineResource.Tags = (IList<string>) list;
        return virtualMachineResource;
      }
    }

    protected IList<VirtualMachineResource> ExecuteBatchStoredProcedure()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<VirtualMachineResource>((ObjectBinder<VirtualMachineResource>) new VirtualMachineResourceBinder());
        resultCollection.AddBinder<EnvironmentResourceTag>((ObjectBinder<EnvironmentResourceTag>) new EnvironmentResourceTagBinder());
        List<VirtualMachineResource> items = resultCollection.GetCurrent<VirtualMachineResource>().Items;
        if (items.Any<VirtualMachineResource>())
        {
          resultCollection.NextResult();
          EnvironmentResourceTag[] array = resultCollection.GetCurrent<EnvironmentResourceTag>().ToArray<EnvironmentResourceTag>();
          foreach (VirtualMachineResource virtualMachineResource in items)
          {
            VirtualMachineResource vmResource = virtualMachineResource;
            List<string> stringList = new List<string>();
            stringList.AddRange(((IEnumerable<EnvironmentResourceTag>) array).Where<EnvironmentResourceTag>((System.Func<EnvironmentResourceTag, bool>) (x => x.ResourceId == vmResource.Id)).Select<EnvironmentResourceTag, string>((System.Func<EnvironmentResourceTag, string>) (x => x.Tag)));
            vmResource.Tags = (IList<string>) stringList;
          }
        }
        return (IList<VirtualMachineResource>) items;
      }
    }
  }
}
