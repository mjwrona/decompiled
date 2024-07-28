// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.KubernetesResourceComponent5
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
  internal class KubernetesResourceComponent5 : KubernetesResourceComponent4
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
        this.BindStringTable("@tags", (IEnumerable<string>) resource.Tags.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>());
        return this.ExecuteStoredProcedure();
      }
    }

    public override IList<KubernetesResource> GetEnvironmentResourcesById(
      Guid projectId,
      int environmentId,
      IEnumerable<int> resourceIds)
    {
      this.PrepareStoredProcedure("Task.prc_GetKubernetesResourcesById");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
      this.BindInt("@environmentId", environmentId);
      this.BindUniqueInt32Table("@resourceIds", resourceIds);
      return this.ExecuteBatchStoredProcedure();
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
        this.BindStringTable("@tags", (IEnumerable<string>) resource.Tags.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>());
        return this.ExecuteStoredProcedure();
      }
    }

    public override IList<KubernetesResource> DeleteEnvironmentResources(
      Guid projectId,
      int environmentId,
      Guid deletedBy)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteEnvironmentResources)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAllKubernetesResources");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindGuid("@deletedBy", deletedBy);
        return this.ExecuteBatchStoredProcedure();
      }
    }

    protected override KubernetesResource ExecuteStoredProcedure()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<KubernetesResource>((ObjectBinder<KubernetesResource>) new KubernetesResourceBinder());
        resultCollection.AddBinder<EnvironmentResourceTag>((ObjectBinder<EnvironmentResourceTag>) new EnvironmentResourceTagBinder());
        KubernetesResource kubernetesResource = resultCollection.GetCurrent<KubernetesResource>().Items.FirstOrDefault<KubernetesResource>();
        if (kubernetesResource == null)
          return (KubernetesResource) null;
        resultCollection.NextResult();
        List<string> list = resultCollection.GetCurrent<EnvironmentResourceTag>().Items.Select<EnvironmentResourceTag, string>((System.Func<EnvironmentResourceTag, string>) (x => x.Tag)).Distinct<string>().Where<string>((System.Func<string, bool>) (t => !string.IsNullOrWhiteSpace(t))).ToList<string>();
        kubernetesResource.Tags = (IList<string>) list;
        return kubernetesResource;
      }
    }

    protected IList<KubernetesResource> ExecuteBatchStoredProcedure()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<KubernetesResource>((ObjectBinder<KubernetesResource>) new KubernetesResourceBinder());
        resultCollection.AddBinder<EnvironmentResourceTag>((ObjectBinder<EnvironmentResourceTag>) new EnvironmentResourceTagBinder());
        List<KubernetesResource> items = resultCollection.GetCurrent<KubernetesResource>().Items;
        resultCollection.NextResult();
        EnvironmentResourceTag[] array = resultCollection.GetCurrent<EnvironmentResourceTag>().ToArray<EnvironmentResourceTag>();
        foreach (KubernetesResource kubernetesResource1 in items)
        {
          KubernetesResource kubernetesResource = kubernetesResource1;
          List<string> stringList = new List<string>();
          stringList.AddRange(((IEnumerable<EnvironmentResourceTag>) array).Where<EnvironmentResourceTag>((System.Func<EnvironmentResourceTag, bool>) (x => x.ResourceId == kubernetesResource.Id)).Select<EnvironmentResourceTag, string>((System.Func<EnvironmentResourceTag, string>) (x => x.Tag)));
          kubernetesResource.Tags = (IList<string>) stringList;
        }
        return (IList<KubernetesResource>) items;
      }
    }
  }
}
