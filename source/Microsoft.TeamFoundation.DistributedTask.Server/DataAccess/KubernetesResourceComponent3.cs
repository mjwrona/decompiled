// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.KubernetesResourceComponent3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class KubernetesResourceComponent3 : KubernetesResourceComponent2
  {
    public override KubernetesResource AddEnvironmentResource(
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
        this.BindString("@clusterName", resource.ClusterName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@serviceEndpointId", resource.ServiceEndpointId);
        this.BindGuid("@createdBy", new Guid(resource.CreatedBy.Id));
        return this.ExecuteStoredProcedure();
      }
    }

    public override KubernetesResource UpdateEnvironmentResource(
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
        this.BindString("@clusterName", resource.ClusterName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(resource.LastModifiedBy.Id));
        return this.ExecuteStoredProcedure();
      }
    }
  }
}
