// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.Server.Data.Model;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class EnvironmentComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<EnvironmentComponent>(1),
      (IComponentCreator) new ComponentCreator<EnvironmentComponent2>(2),
      (IComponentCreator) new ComponentCreator<EnvironmentComponent3>(3),
      (IComponentCreator) new ComponentCreator<EnvironmentComponent4>(4)
    }, "DistributedTaskEnvironment", "DistributedTask");

    public EnvironmentComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual EnvironmentInstance AddEnvironment(
      string projectEnvironmentName,
      string collectionEnvironmentName,
      Guid projectId,
      Guid createdBy,
      string description = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddEnvironment)))
      {
        this.PrepareStoredProcedure("Task.prc_AddEnvironment");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindString("@projectEnvironmentName", projectEnvironmentName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@collectionEnvironmentName", collectionEnvironmentName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@description", description, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", createdBy);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          return resultCollection.GetCurrent<EnvironmentInstance>().Items.FirstOrDefault<EnvironmentInstance>();
        }
      }
    }

    public virtual EnvironmentInstance GetEnvironmentById(
      int id,
      Guid projectId,
      bool includeResourceReferences = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentById)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentById");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", id);
        this.BindBoolean("@includeResourceReferences", includeResourceReferences);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          EnvironmentInstance environmentById = resultCollection.GetCurrent<EnvironmentInstance>().Items.FirstOrDefault<EnvironmentInstance>();
          if (environmentById != null && includeResourceReferences)
          {
            resultCollection.NextResult();
            resultCollection.AddBinder<EnvironmentResourceReference>((ObjectBinder<EnvironmentResourceReference>) new EnvironmentResourceBinder());
            ((List<EnvironmentResourceReference>) environmentById.Resources).AddRange((IEnumerable<EnvironmentResourceReference>) resultCollection.GetCurrent<EnvironmentResourceReference>().Items);
          }
          return environmentById;
        }
      }
    }

    public virtual IList<EnvironmentInstance> GetEnvironmentsByModifiedTime(
      Guid projectId,
      DateTime? lastModifiedOn,
      int batchSize)
    {
      return (IList<EnvironmentInstance>) new List<EnvironmentInstance>();
    }

    public virtual IList<EnvironmentInstance> GetEnvironments(
      Guid projectId,
      string environmentName = null,
      string lastEnvironmentName = null,
      int maxEnvironmentsCount = 50)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironments)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironments");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindString("@environmentName", environmentName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@lastEnvironmentName", lastEnvironmentName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@maxEnvironmentsCount", maxEnvironmentsCount);
        List<EnvironmentInstance> environments = new List<EnvironmentInstance>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          environments.AddRange((IEnumerable<EnvironmentInstance>) resultCollection.GetCurrent<EnvironmentInstance>().Items);
          return (IList<EnvironmentInstance>) environments;
        }
      }
    }

    public virtual IList<EnvironmentInstance> GetEnvironmentsByIds(
      Guid projectId,
      IEnumerable<int> environmentIds,
      bool includeResourceReferences = false)
    {
      throw new ServiceVersionNotSupportedException();
    }

    public virtual int? GetEnvironmentPoolId(int environmentId) => throw new ServiceVersionNotSupportedException();

    public virtual int RegisterEnvironmentPool(int environmentId, int poolId) => throw new ServiceVersionNotSupportedException();

    public virtual IList<EnvironmentObject> GetEnvironmentsWithFilters(
      Guid projectId,
      string environmentName = null,
      string continutationToken = null,
      int maxEnvironmentsCount = 50,
      bool includeLastCompletedRequest = false,
      IEnumerable<byte> environmentLastJobStatusFilters = null,
      bool includeInProgressFilter = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentsWithFilters)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentsWithFilters");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindString("@environmentName", environmentName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@continutationToken", continutationToken, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@maxEnvironmentsCount", maxEnvironmentsCount);
        this.BindBoolean("@includeLastCompletedRequest", includeLastCompletedRequest);
        this.BindTinyIntTable("@environmentLastJobStatus", environmentLastJobStatusFilters);
        this.BindBoolean("@includeInProgressFilter", includeInProgressFilter);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          List<EnvironmentInstance> environmentInstanceList = new List<EnvironmentInstance>();
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          environmentInstanceList.AddRange((IEnumerable<EnvironmentInstance>) resultCollection.GetCurrent<EnvironmentInstance>().Items);
          IList<EnvironmentObject> environmentObjectList = environmentInstanceList.ToEnvironmentObjectList();
          if (includeLastCompletedRequest)
          {
            resultCollection.NextResult();
            resultCollection.AddBinder<DeploymentExecutionRecordObject>((ObjectBinder<DeploymentExecutionRecordObject>) new DeploymentExecutionRecordObjectBinder(this));
            IDictionary<int, IList<DeploymentExecutionRecordObject>> mappingsPerEnvironment = this.GetDeploymentRecordsMappingsPerEnvironment((IList<DeploymentExecutionRecordObject>) resultCollection.GetCurrent<DeploymentExecutionRecordObject>().Items);
            foreach (EnvironmentObject environmentObject in (IEnumerable<EnvironmentObject>) environmentObjectList)
            {
              if (mappingsPerEnvironment.ContainsKey(environmentObject.Id))
                ((List<DeploymentExecutionRecordObject>) environmentObject.DeploymentRecords).AddRange((IEnumerable<DeploymentExecutionRecordObject>) mappingsPerEnvironment[environmentObject.Id]);
            }
          }
          return environmentObjectList;
        }
      }
    }

    public virtual EnvironmentObject GetEnvironmentResourceWithFilters(
      int id,
      Guid projectId,
      string resourceName = null,
      IEnumerable<byte> environmentResourceType = null,
      bool includeResourceReferences = false,
      bool includeLastCompletedRequest = false,
      IEnumerable<byte> environmentLastJobStatusFilters = null,
      bool includeInProgressFilter = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentResourceWithFilters)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentResourceWithFilters");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", id);
        this.BindBoolean("@includeResourceReferences", includeResourceReferences);
        this.BindString("@resourceName", resourceName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindTinyIntTable("@environmentResourceType", environmentResourceType);
        this.BindBoolean("@includeLastCompletedRequest", includeLastCompletedRequest);
        this.BindTinyIntTable("@environmentLastJobStatus", environmentLastJobStatusFilters);
        this.BindBoolean("@includeInProgressFilter", includeInProgressFilter);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          EnvironmentObject environmentObject = (resultCollection.GetCurrent<EnvironmentInstance>().Items.FirstOrDefault<EnvironmentInstance>() ?? throw new EnvironmentNotFoundException(TaskResources.EnvironmentNotFound((object) id))).ToEnvironmentObject();
          if (includeResourceReferences)
          {
            resultCollection.NextResult();
            resultCollection.AddBinder<EnvironmentResourceReference>((ObjectBinder<EnvironmentResourceReference>) new EnvironmentResourceBinder());
            foreach (EnvironmentResourceReference environmentResourceReference in resultCollection.GetCurrent<EnvironmentResourceReference>().Items)
              environmentObject.Resources.Add(environmentResourceReference.ToResourceReferenceObject());
            if (includeLastCompletedRequest)
            {
              resultCollection.NextResult();
              resultCollection.AddBinder<DeploymentExecutionRecordObject>((ObjectBinder<DeploymentExecutionRecordObject>) new DeploymentExecutionRecordObjectBinder(this));
              List<DeploymentExecutionRecordObject> items = resultCollection.GetCurrent<DeploymentExecutionRecordObject>().Items;
              if (environmentObject.Resources != null)
              {
                IDictionary<int, IList<DeploymentExecutionRecordObject>> mappingsPerResource = this.GetDeploymentRecordsMappingsPerResource((IList<DeploymentExecutionRecordObject>) items);
                foreach (EnvironmentResourceReferenceObject resource in (IEnumerable<EnvironmentResourceReferenceObject>) environmentObject.Resources)
                {
                  if (mappingsPerResource.ContainsKey(resource.Id))
                    ((List<DeploymentExecutionRecordObject>) resource.DeploymentRecords).AddRange((IEnumerable<DeploymentExecutionRecordObject>) mappingsPerResource[resource.Id]);
                }
              }
            }
          }
          return environmentObject;
        }
      }
    }

    public virtual EnvironmentInstance UpdateEnvironment(
      Guid projectId,
      int environmentId,
      Guid modifiedBy,
      string projectEnvironmentName = null,
      string description = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateEnvironment)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateEnvironment");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        this.BindString("@name", projectEnvironmentName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@description", description, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", modifiedBy);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          return resultCollection.GetCurrent<EnvironmentInstance>().Items.FirstOrDefault<EnvironmentInstance>();
        }
      }
    }

    public virtual EnvironmentInstance DeleteEnvironment(Guid projectId, int environmentId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteEnvironment)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteEnvironment");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@environmentId", environmentId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
          return resultCollection.GetCurrent<EnvironmentInstance>().Items.FirstOrDefault<EnvironmentInstance>();
        }
      }
    }

    public void DeleteTeamProject(Guid projectId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteTeamProject)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteProjectEnvironments");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindGuid("@projectId", projectId);
        this.BindGuid("@namespaceGuid", EnvironmentSecurityProvider.EnvironmentNamespaceId);
        this.ExecuteNonQuery();
      }
    }

    protected virtual IDictionary<int, IList<DeploymentExecutionRecordObject>> GetDeploymentRecordsMappingsPerEnvironment(
      IList<DeploymentExecutionRecordObject> mappings)
    {
      Dictionary<int, IList<DeploymentExecutionRecordObject>> mappingsPerEnvironment = new Dictionary<int, IList<DeploymentExecutionRecordObject>>();
      foreach (DeploymentExecutionRecordObject mapping in (IEnumerable<DeploymentExecutionRecordObject>) mappings)
      {
        if (!mappingsPerEnvironment.ContainsKey(mapping.EnvironmentReference.Id))
          mappingsPerEnvironment[mapping.EnvironmentReference.Id] = (IList<DeploymentExecutionRecordObject>) new List<DeploymentExecutionRecordObject>();
        mappingsPerEnvironment[mapping.EnvironmentReference.Id].Add(mapping);
      }
      return (IDictionary<int, IList<DeploymentExecutionRecordObject>>) mappingsPerEnvironment;
    }

    protected virtual IDictionary<int, IList<DeploymentExecutionRecordObject>> GetDeploymentRecordsMappingsPerResource(
      IList<DeploymentExecutionRecordObject> mappings)
    {
      Dictionary<int, IList<DeploymentExecutionRecordObject>> mappingsPerResource = new Dictionary<int, IList<DeploymentExecutionRecordObject>>();
      foreach (DeploymentExecutionRecordObject mapping in (IEnumerable<DeploymentExecutionRecordObject>) mappings)
      {
        if (!mappingsPerResource.ContainsKey(mapping.ResourceReference.Id))
          mappingsPerResource[mapping.ResourceReference.Id] = (IList<DeploymentExecutionRecordObject>) new List<DeploymentExecutionRecordObject>();
        mappingsPerResource[mapping.ResourceReference.Id].Add(mapping);
      }
      return (IDictionary<int, IList<DeploymentExecutionRecordObject>>) mappingsPerResource;
    }

    protected virtual void MapResourcesToEnvironments(
      IList<EnvironmentInstance> environments,
      IList<EnvironmentResourceData> resources)
    {
      Dictionary<int, EnvironmentInstance> dictionary = environments.ToDictionary<EnvironmentInstance, int>((System.Func<EnvironmentInstance, int>) (env => env.Id));
      foreach (EnvironmentResourceData resource in (IEnumerable<EnvironmentResourceData>) resources)
      {
        if (dictionary.ContainsKey(resource.EnvironmentId))
          dictionary[resource.EnvironmentId].Resources.Add(resource.ToResourceReference());
      }
    }

    protected virtual EnvironmentBinder GetEnvironmentBinder() => new EnvironmentBinder();
  }
}
