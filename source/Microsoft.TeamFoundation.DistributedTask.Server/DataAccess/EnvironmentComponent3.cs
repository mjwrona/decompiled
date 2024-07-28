// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentComponent3
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
  internal class EnvironmentComponent3 : EnvironmentComponent2
  {
    public override EnvironmentInstance GetEnvironmentById(
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
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          EnvironmentInstance environmentById = this.ReadEnvironment(rc);
          if (environmentById != null && includeResourceReferences)
          {
            ((List<EnvironmentResourceReference>) environmentById.Resources).AddRange((IEnumerable<EnvironmentResourceReference>) this.ReadResources(rc));
            Dictionary<int, List<string>> resourceIdToTags = this.ReadTags(rc);
            this.MapTagsToResources(environmentById.Resources, resourceIdToTags);
          }
          return environmentById;
        }
      }
    }

    private EnvironmentInstance ReadEnvironment(ResultCollection rc)
    {
      rc.AddBinder<EnvironmentInstance>((ObjectBinder<EnvironmentInstance>) this.GetEnvironmentBinder());
      return rc.GetCurrent<EnvironmentInstance>().FirstOrDefault<EnvironmentInstance>();
    }

    private List<EnvironmentResourceReference> ReadResources(ResultCollection rc)
    {
      rc.NextResult();
      rc.AddBinder<EnvironmentResourceReference>((ObjectBinder<EnvironmentResourceReference>) new EnvironmentResourceBinder());
      return rc.GetCurrent<EnvironmentResourceReference>().Items;
    }

    private Dictionary<int, List<string>> ReadTags(ResultCollection rc)
    {
      rc.NextResult();
      rc.AddBinder<EnvironmentResourceTag>((ObjectBinder<EnvironmentResourceTag>) new EnvironmentResourceTagBinder());
      return rc.GetCurrent<EnvironmentResourceTag>().Items.GroupBy<EnvironmentResourceTag, int>((System.Func<EnvironmentResourceTag, int>) (tag => tag.ResourceId)).ToDictionary<IGrouping<int, EnvironmentResourceTag>, int, List<string>>((System.Func<IGrouping<int, EnvironmentResourceTag>, int>) (group => group.Key), (System.Func<IGrouping<int, EnvironmentResourceTag>, List<string>>) (group => group.Select<EnvironmentResourceTag, string>((System.Func<EnvironmentResourceTag, string>) (tag => tag.Tag)).ToList<string>()));
    }

    private void MapTagsToResources(
      IList<EnvironmentResourceReference> resources,
      Dictionary<int, List<string>> resourceIdToTags)
    {
      foreach (EnvironmentResourceReference resource in (IEnumerable<EnvironmentResourceReference>) resources)
      {
        List<string> stringList;
        resourceIdToTags.TryGetValue(resource.Id, out stringList);
        resource.Tags = (IList<string>) stringList;
      }
    }

    public override EnvironmentObject GetEnvironmentResourceWithFilters(
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
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          EnvironmentObject environmentObject = (this.ReadEnvironment(rc) ?? throw new EnvironmentNotFoundException(TaskResources.EnvironmentNotFound((object) id))).ToEnvironmentObject();
          if (includeResourceReferences)
          {
            foreach (EnvironmentResourceReference readResource in this.ReadResources(rc))
              environmentObject.Resources.Add(readResource.ToResourceReferenceObject());
            if (includeLastCompletedRequest)
            {
              List<DeploymentExecutionRecordObject> mappings = this.ReadDeploymentExecutionRecords(rc);
              if (environmentObject.Resources != null)
              {
                IDictionary<int, IList<DeploymentExecutionRecordObject>> mappingsPerResource = this.GetDeploymentRecordsMappingsPerResource((IList<DeploymentExecutionRecordObject>) mappings);
                foreach (EnvironmentResourceReferenceObject resource in (IEnumerable<EnvironmentResourceReferenceObject>) environmentObject.Resources)
                {
                  if (mappingsPerResource.ContainsKey(resource.Id))
                    ((List<DeploymentExecutionRecordObject>) resource.DeploymentRecords).AddRange((IEnumerable<DeploymentExecutionRecordObject>) mappingsPerResource[resource.Id]);
                }
              }
            }
            Dictionary<int, List<string>> resourceIdToTags = this.ReadTags(rc);
            this.MapTagsToResources(environmentObject.Resources, resourceIdToTags);
          }
          return environmentObject;
        }
      }
    }

    private List<DeploymentExecutionRecordObject> ReadDeploymentExecutionRecords(ResultCollection rc)
    {
      rc.NextResult();
      rc.AddBinder<DeploymentExecutionRecordObject>((ObjectBinder<DeploymentExecutionRecordObject>) new DeploymentExecutionRecordObjectBinder((EnvironmentComponent) this));
      return rc.GetCurrent<DeploymentExecutionRecordObject>().Items;
    }

    private void MapTagsToResources(
      IList<EnvironmentResourceReferenceObject> resources,
      Dictionary<int, List<string>> resourceIdToTags)
    {
      foreach (EnvironmentResourceReferenceObject resource in (IEnumerable<EnvironmentResourceReferenceObject>) resources)
      {
        List<string> stringList;
        resourceIdToTags.TryGetValue(resource.Id, out stringList);
        resource.Tags = (IList<string>) (stringList ?? new List<string>());
      }
    }

    public override int? GetEnvironmentPoolId(int environmentId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetEnvironmentPoolId)))
      {
        this.PrepareStoredProcedure("Task.prc_GetEnvironmentPoolId");
        this.BindInt("@environmentId", environmentId);
        int? environmentPoolId = new int?();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          try
          {
            environmentPoolId = new int?(resultCollection.GetCurrent<int>().First<int>());
          }
          catch (InvalidOperationException ex)
          {
          }
          return environmentPoolId;
        }
      }
    }

    public override int RegisterEnvironmentPool(int environmentId, int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (RegisterEnvironmentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_RegisterEnvironmentPool");
        this.BindInt("@environmentId", environmentId);
        this.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          poolId = resultCollection.GetCurrent<int>().FirstOrDefault<int>();
          return poolId;
        }
      }
    }

    protected override EnvironmentBinder GetEnvironmentBinder() => (EnvironmentBinder) new EnvironmentBinder2((EnvironmentComponent) this);
  }
}
