// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.DataAccess.PipelineResourceAuthorizationComponent
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Authorization.Server.DataAccess
{
  internal class PipelineResourceAuthorizationComponent : AuthorizationSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PipelineResourceAuthorizationComponent>(1)
    }, "PipelineResourceAuthorization", "Build");
    protected static readonly SqlMetaData[] typ_AuthorizedResourceTable = new SqlMetaData[2]
    {
      new SqlMetaData("ResourceType", SqlDbType.Int),
      new SqlMetaData("ResourceId", SqlDbType.NVarChar, 100L)
    };
    protected static readonly SqlMetaData[] typ_ScopedDefinitionTable = new SqlMetaData[3]
    {
      new SqlMetaData("ResourceType", SqlDbType.Int),
      new SqlMetaData("ResourceId", SqlDbType.NVarChar, 100L),
      new SqlMetaData("DefinitionId", SqlDbType.Int)
    };

    public PipelineResourceAuthorizationComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected virtual ObjectBinder<PipelineResourceReference> GetResourceReferenceBinder() => (ObjectBinder<PipelineResourceReference>) new PipelineResourceReferenceBinder(this.RequestContext);

    public virtual async Task<PipelineProcessResources> GetAuthorizedResourcesAsync(
      Guid projectId,
      int? definitionId,
      ResourceType? resourceType,
      string resourceId)
    {
      PipelineResourceAuthorizationComponent authorizationComponent1 = this;
      authorizationComponent1.TraceEnter(0, nameof (GetAuthorizedResourcesAsync));
      authorizationComponent1.PrepareStoredProcedure("PipelinePolicy.prc_GetAuthorizedResources");
      authorizationComponent1.BindInt("@dataspaceId", authorizationComponent1.GetDataspaceId(projectId, "Build"));
      authorizationComponent1.BindNullableInt("@definitionId", definitionId);
      PipelineResourceAuthorizationComponent authorizationComponent2 = authorizationComponent1;
      ResourceType? nullable = resourceType;
      int? parameterValue = nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?();
      authorizationComponent2.BindNullableInt("@resourceType", parameterValue);
      authorizationComponent1.BindString("@resourceId", resourceId, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      PipelineProcessResources authorizedResourcesAsync;
      using (ResultCollection rc = new ResultCollection((IDataReader) await authorizationComponent1.ExecuteReaderAsync(), authorizationComponent1.ProcedureName, authorizationComponent1.RequestContext))
      {
        rc.AddBinder<PipelineResourceReference>(authorizationComponent1.GetResourceReferenceBinder());
        PipelineProcessResources authorizedResources = authorizationComponent1.GetAuthorizedResources(rc);
        authorizationComponent1.TraceLeave(0, nameof (GetAuthorizedResourcesAsync));
        authorizedResourcesAsync = authorizedResources;
      }
      return authorizedResourcesAsync;
    }

    public virtual async Task<PipelineProcessResources> UpdateAuthorizedResourcesAsync(
      Guid projectId,
      int? definitionId,
      Guid authorizedBy,
      PipelineProcessResources resources)
    {
      PipelineResourceAuthorizationComponent authorizationComponent = this;
      authorizationComponent.TraceEnter(0, nameof (UpdateAuthorizedResourcesAsync));
      authorizationComponent.PrepareStoredProcedure("PipelinePolicy.prc_UpdateAuthorizedResources");
      authorizationComponent.BindInt("@dataspaceId", authorizationComponent.GetDataspaceId(projectId, "Build"));
      authorizationComponent.BindNullableInt("@definitionId", definitionId);
      authorizationComponent.BindGuid("@authorizedBy", authorizedBy);
      authorizationComponent.BindAuthorizedResourcesTable("@resourcesToAuthorize", resources.GetAuthorizedResources());
      authorizationComponent.BindAuthorizedResourcesTable("@resourcesToUnauthorize", resources.GetUnauthorizedResources());
      PipelineProcessResources processResources;
      using (ResultCollection rc = new ResultCollection((IDataReader) await authorizationComponent.ExecuteReaderAsync(), authorizationComponent.ProcedureName, authorizationComponent.RequestContext))
      {
        rc.AddBinder<PipelineResourceReference>(authorizationComponent.GetResourceReferenceBinder());
        PipelineProcessResources authorizedResources = authorizationComponent.GetAuthorizedResources(rc);
        authorizationComponent.TraceLeave(0, nameof (UpdateAuthorizedResourcesAsync));
        processResources = authorizedResources;
      }
      return processResources;
    }

    public virtual void RemoveDefinitionSpecificAuthorization(Guid projectId, int definitionId)
    {
      this.TraceEnter(0, nameof (RemoveDefinitionSpecificAuthorization));
      this.PrepareStoredProcedure("PipelinePolicy.prc_RemoveAuthorizedResourcesByDefinition");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "Build"));
      this.BindInt("@definitionId", definitionId);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (RemoveDefinitionSpecificAuthorization));
    }

    public virtual async Task<List<ResourcePipelinePermissions>> GetPipelinePermissionsForResources(
      Guid projectId,
      List<Resource> resources)
    {
      PipelineResourceAuthorizationComponent authorizationComponent = this;
      authorizationComponent.TraceEnter(0, nameof (GetPipelinePermissionsForResources));
      authorizationComponent.PrepareStoredProcedure("PipelinePolicy.prc_GetAuthorizedDefinitionsForResources");
      authorizationComponent.BindInt("@dataspaceId", authorizationComponent.GetDataspaceId(projectId, "Build"));
      List<PipelineResourceReference> queriedResources = new List<PipelineResourceReference>();
      resources.ForEach((Action<Resource>) (resource => queriedResources.Add(new PipelineResourceReference()
      {
        Type = resource.Type,
        Id = resource.Id
      })));
      authorizationComponent.BindScopedDefinitionTable("@resources", queriedResources);
      List<ResourcePipelinePermissions> permissionsForResources;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await authorizationComponent.ExecuteReaderAsync(), authorizationComponent.ProcedureName, authorizationComponent.RequestContext))
      {
        resultCollection.AddBinder<PipelineResourceReference>(authorizationComponent.GetResourceReferenceBinder());
        List<ResourcePipelinePermissions> pipelinePermissionsList = authorizationComponent.GetResourcePipelinePermissionsList(resultCollection.GetCurrent<PipelineResourceReference>());
        authorizationComponent.TraceLeave(0, nameof (GetPipelinePermissionsForResources));
        permissionsForResources = pipelinePermissionsList;
      }
      return permissionsForResources;
    }

    public virtual async Task<ResourcePipelinePermissions> UpdatePipelinePermisionsForResource(
      Guid projectId,
      string resourceType,
      string resourceId,
      Guid authorizedBy,
      ResourcePipelinePermissions resourcePipelinePermissions)
    {
      resourcePipelinePermissions.Resource = new Resource()
      {
        Type = resourceType,
        Id = resourceId
      };
      List<ResourcePipelinePermissions> resourcePipelinePermissionsList = new List<ResourcePipelinePermissions>()
      {
        resourcePipelinePermissions
      };
      List<ResourcePipelinePermissions> source = await this.UpdatePipelinePermisionsForResources(projectId, authorizedBy, resourcePipelinePermissionsList);
      ResourcePipelinePermissions pipelinePermissions;
      if (!source.Any<ResourcePipelinePermissions>())
        pipelinePermissions = new ResourcePipelinePermissions()
        {
          Resource = new Resource()
          {
            Type = resourceType,
            Id = resourceId
          }
        };
      else
        pipelinePermissions = source.FirstOrDefault<ResourcePipelinePermissions>();
      return pipelinePermissions;
    }

    public virtual async Task DeletePipelinePermissionsForResource(
      Guid projectId,
      string resourceType,
      string resourceId)
    {
      PipelineResourceAuthorizationComponent authorizationComponent = this;
      authorizationComponent.TraceEnter(0, nameof (DeletePipelinePermissionsForResource));
      authorizationComponent.PrepareStoredProcedure("PipelinePolicy.prc_UnauthorizeAllDefinitionsForResource");
      authorizationComponent.BindInt("@dataspaceId", authorizationComponent.GetDataspaceId(projectId, "Build"));
      authorizationComponent.BindInt("@resourceType", (int) authorizationComponent.GetIntTypeValue(resourceType));
      authorizationComponent.BindString("@resourceId", resourceId, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      int num = await authorizationComponent.ExecuteNonQueryAsync();
      authorizationComponent.TraceLeave(0, nameof (DeletePipelinePermissionsForResource));
    }

    public virtual async Task<List<ResourcePipelinePermissions>> UpdatePipelinePermisionsForResources(
      Guid projectId,
      Guid authorizedBy,
      List<ResourcePipelinePermissions> resourcePipelinePermissionsList)
    {
      PipelineResourceAuthorizationComponent authorizationComponent = this;
      authorizationComponent.TraceEnter(0, nameof (UpdatePipelinePermisionsForResources));
      authorizationComponent.PrepareStoredProcedure("PipelinePolicy.prc_UpdateAuthorizedDefinitionsForResources");
      authorizationComponent.BindInt("@dataspaceId", authorizationComponent.GetDataspaceId(projectId, "Build"));
      authorizationComponent.BindGuid("@authorizedBy", authorizedBy);
      authorizationComponent.BindResources(resourcePipelinePermissionsList);
      List<ResourcePipelinePermissions> pipelinePermissionsList1;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await authorizationComponent.ExecuteReaderAsync(), authorizationComponent.ProcedureName, authorizationComponent.RequestContext))
      {
        resultCollection.AddBinder<PipelineResourceReference>(authorizationComponent.GetResourceReferenceBinder());
        List<ResourcePipelinePermissions> pipelinePermissionsList2 = authorizationComponent.GetResourcePipelinePermissionsList(resultCollection.GetCurrent<PipelineResourceReference>());
        authorizationComponent.TraceLeave(0, nameof (UpdatePipelinePermisionsForResources));
        pipelinePermissionsList1 = pipelinePermissionsList2;
      }
      return pipelinePermissionsList1;
    }

    private void BindResources(
      List<ResourcePipelinePermissions> resourcePipelinePermissionsList)
    {
      List<PipelineResourceReference> resourcesToAuthorize = new List<PipelineResourceReference>();
      List<PipelineResourceReference> resourcesToUnauthorize = new List<PipelineResourceReference>();
      resourcePipelinePermissionsList.ForEach((Action<ResourcePipelinePermissions>) (resourcePipelinePermissions =>
      {
        Resource resource = resourcePipelinePermissions.Resource;
        resourcePipelinePermissions.Pipelines.ForEach((Action<PipelinePermission>) (pipeline =>
        {
          PipelineResourceReference resourceReference = new PipelineResourceReference()
          {
            Type = resource.Type,
            Id = resource.Id,
            DefinitionId = new int?(pipeline.Id)
          };
          if (pipeline.Authorized)
            resourcesToAuthorize.Add(resourceReference);
          else
            resourcesToUnauthorize.Add(resourceReference);
        }));
        if (resourcePipelinePermissions.AllPipelines == null)
          return;
        PipelineResourceReference resourceReference1 = new PipelineResourceReference()
        {
          Type = resource.Type,
          Id = resource.Id,
          DefinitionId = new int?()
        };
        if (resourcePipelinePermissions.AllPipelines.Authorized)
          resourcesToAuthorize.Add(resourceReference1);
        else
          resourcesToUnauthorize.Add(resourceReference1);
      }));
      resourcesToAuthorize = resourcesToAuthorize.Distinct<PipelineResourceReference>().ToList<PipelineResourceReference>();
      resourcesToUnauthorize = resourcesToUnauthorize.Distinct<PipelineResourceReference>().ToList<PipelineResourceReference>();
      this.BindScopedDefinitionTable("@resourcesToAuthorize", resourcesToAuthorize);
      this.BindScopedDefinitionTable("@resourcesToUnauthorize", resourcesToUnauthorize);
    }

    protected void BindAuthorizedResourcesTable(
      string parameterName,
      PipelineProcessResources resources)
    {
      resources = resources ?? new PipelineProcessResources();
      IEnumerable<KeyValuePair<int, string>> source = resources.Resources.Select<PipelineResourceReference, KeyValuePair<int, string>>((System.Func<PipelineResourceReference, KeyValuePair<int, string>>) (e => new KeyValuePair<int, string>((int) this.GetIntTypeValue(e.Type), e.Id.ToString())));
      this.BindTable(parameterName, "PipelinePolicy.typ_AuthorizedResourceTable", source.Select<KeyValuePair<int, string>, SqlDataRecord>(new System.Func<KeyValuePair<int, string>, SqlDataRecord>(rowBinder)));

      static SqlDataRecord rowBinder(KeyValuePair<int, string> resource)
      {
        SqlDataRecord record = new SqlDataRecord(PipelineResourceAuthorizationComponent.typ_AuthorizedResourceTable);
        record.SetInt32(0, resource.Key);
        record.SetString(1, resource.Value, BindStringBehavior.Unchanged);
        return record;
      }
    }

    protected void BindScopedDefinitionTable(
      string parameterName,
      List<PipelineResourceReference> resourceReferences)
    {
      resourceReferences = resourceReferences ?? new List<PipelineResourceReference>();
      // ISSUE: method pointer
      this.BindTable(parameterName, "PipelinePolicy.typ_ScopedDefinitionTable", resourceReferences.Select<PipelineResourceReference, SqlDataRecord>(new System.Func<PipelineResourceReference, SqlDataRecord>((object) this, __methodptr(\u003CBindScopedDefinitionTable\u003Eg__rowBinder\u007C14_0))));
    }

    private ResourceType GetIntTypeValue(string Type)
    {
      ResourceType result = (ResourceType) 0;
      ResourceTypeNames.TryParse(Type, out result);
      return result;
    }

    private PipelineProcessResources GetAuthorizedResources(ResultCollection rc)
    {
      PipelineProcessResources authorizedResources = new PipelineProcessResources();
      foreach (PipelineResourceReference resourceReference in rc.GetCurrent<PipelineResourceReference>())
      {
        if (resourceReference != null)
          authorizedResources.Add(resourceReference);
      }
      return authorizedResources;
    }

    private List<ResourcePipelinePermissions> GetResourcePipelinePermissionsList(
      ObjectBinder<PipelineResourceReference> references)
    {
      Dictionary<string, ResourcePipelinePermissions> dictionary = new Dictionary<string, ResourcePipelinePermissions>();
      foreach (PipelineResourceReference reference in references)
      {
        PipelineResourceReference resourceReference = reference;
        string key = resourceReference.Type + "-" + resourceReference.Id;
        ResourcePipelinePermissions pipelinePermissions;
        if (!dictionary.TryGetValue(key, out pipelinePermissions))
        {
          pipelinePermissions = new ResourcePipelinePermissions()
          {
            Resource = new Resource()
            {
              Type = resourceReference.Type,
              Id = resourceReference.Id
            }
          };
          dictionary.Add(key, pipelinePermissions);
        }
        if (!resourceReference.DefinitionId.HasValue)
        {
          if (pipelinePermissions.AllPipelines == null)
            pipelinePermissions.AllPipelines = new Permission()
            {
              Authorized = true,
              AuthorizedBy = new IdentityRef()
              {
                Id = resourceReference.AuthorizedBy.ToString()
              },
              AuthorizedOn = resourceReference.AuthorizedOn
            };
        }
        else if (!pipelinePermissions.Pipelines.Exists((Predicate<PipelinePermission>) (pipeline =>
        {
          int id = pipeline.Id;
          int? definitionId = resourceReference.DefinitionId;
          int valueOrDefault = definitionId.GetValueOrDefault();
          return id == valueOrDefault & definitionId.HasValue;
        })))
          pipelinePermissions.Pipelines.Add(this.ConvertResourceReferenceToPipelinePermission(resourceReference));
      }
      return dictionary.Values.ToList<ResourcePipelinePermissions>();
    }

    private PipelinePermission ConvertResourceReferenceToPipelinePermission(
      PipelineResourceReference resourceReference)
    {
      PipelinePermission pipelinePermission = new PipelinePermission();
      pipelinePermission.Id = resourceReference.DefinitionId.Value;
      pipelinePermission.Authorized = resourceReference.Authorized;
      pipelinePermission.AuthorizedBy = new IdentityRef()
      {
        Id = resourceReference.AuthorizedBy.ToString()
      };
      pipelinePermission.AuthorizedOn = resourceReference.AuthorizedOn;
      return pipelinePermission;
    }
  }
}
