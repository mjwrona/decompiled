// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.KubernetesResourceComponent
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
  internal class KubernetesResourceComponent : 
    TaskSqlComponentBase,
    IEnvironmentResourceComponent<KubernetesResource>,
    IDisposable
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<KubernetesResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<KubernetesResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<KubernetesResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<KubernetesResourceComponent4>(4),
      (IComponentCreator) new ComponentCreator<KubernetesResourceComponent5>(5)
    }, "DistributedTaskKubernetesResource", "DistributedTask");

    public KubernetesResourceComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual KubernetesResource AddEnvironmentResource(
      Guid projectId,
      KubernetesResource resource)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_AddKubernetesResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", resource.EnvironmentReference.Id);
        this.BindString("@name", resource.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@namespace", resource.Namespace, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@serviceEndpointId", resource.ServiceEndpointId);
        this.BindGuid("@createdBy", new Guid(resource.CreatedBy.Id));
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual KubernetesResource GetEnvironmentResource(
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_GetKubernetesResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@resourceId", resourceId);
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual IList<KubernetesResource> GetEnvironmentResourcesById(
      Guid projectId,
      int environmentId,
      IEnumerable<int> resourceIds)
    {
      return (IList<KubernetesResource>) new List<KubernetesResource>();
    }

    public virtual KubernetesResource UpdateEnvironmentResource(
      Guid projectId,
      KubernetesResource resource)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateKubernetesResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", resource.EnvironmentReference.Id);
        this.BindInt("@resourceId", resource.Id);
        this.BindString("@name", resource.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@namespace", resource.Namespace, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(resource.LastModifiedBy.Id));
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual KubernetesResource DeleteEnvironmentResource(
      Guid projectId,
      int environmentId,
      int resourceId,
      Guid deletedBy)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteEnvironmentResource)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteKubernetesResource");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@resourceId", resourceId);
        this.BindGuid("@deletedBy", deletedBy);
        return this.ExecuteStoredProcedure();
      }
    }

    public virtual IList<KubernetesResource> DeleteEnvironmentResources(
      Guid projectId,
      int environmentId,
      Guid deletedBy)
    {
      return (IList<KubernetesResource>) new List<KubernetesResource>();
    }

    protected virtual KubernetesResource ExecuteStoredProcedure()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<KubernetesResource>((ObjectBinder<KubernetesResource>) new KubernetesResourceBinder());
        return resultCollection.GetCurrent<KubernetesResource>().Items.FirstOrDefault<KubernetesResource>();
      }
    }
  }
}
