// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  public class ServiceEndpointComponent : ServiceEndpointsSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[26]
    {
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent2>(2),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent3>(3),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent4>(4),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent5>(5),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent6>(6),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent7>(7),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent8>(8),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent9>(9),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent10>(10),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent11>(11),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent12>(12),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent13>(13),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent14>(14),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent15>(15),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent16>(16),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent17>(17),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent18>(18),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent19>(19),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent20>(20),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent21>(21),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent22>(22),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent23>(23),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent24>(24),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent25>(25),
      (IComponentCreator) new ComponentCreator<ServiceEndpointComponent26>(26)
    }, "DistributedTaskServiceEndpoint", "DistributedTask");
    protected const string DistributedTaskNamespaceCategory = "DistributedTask";

    public ServiceEndpointComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    internal virtual void AddServiceEndpoint(
      Guid scopeId,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (AddServiceEndpoint)))
      {
        this.PrepareStoredProcedure("Task.prc_AddServiceEndpoint");
        this.BindDataspaceId(scopeId);
        this.BindGuid("@id", serviceEndpoint.Id);
        this.BindString("@name", serviceEndpoint.Name.Trim(), 512, false, SqlDbType.NVarChar);
        this.BindString("@type", serviceEndpoint.Type.Trim(), 128, false, SqlDbType.NVarChar);
        this.BindString("@url", serviceEndpoint.Url.ToString().Trim(), int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@description", serviceEndpoint.Description.Trim(), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(serviceEndpoint.CreatedBy.Id));
        this.BindString("@authorizationScheme", serviceEndpoint.Authorization.Scheme.Trim(), 128, false, SqlDbType.NVarChar);
        this.BindString("@data", JsonUtility.ToString((object) serviceEndpoint.Data), int.MaxValue, true, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    internal virtual void AddServiceEndpoint(
      List<Guid> scopeIds,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
      this.AddServiceEndpoint(scopeIds.First<Guid>(), serviceEndpoint, disallowDuplicateEndpointName);
    }

    internal virtual ServiceEndpoint DeleteServiceEndpoint(Guid id, Guid scopeId)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (DeleteServiceEndpoint)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteServiceEndpoint");
        this.BindGuid("@id", id);
        this.BindDataspaceId(scopeId);
        int num = (bool) this.ExecuteScalar() ? 1 : 0;
        ServiceEndpoint serviceEndpoint = (ServiceEndpoint) null;
        if (num != 0)
          serviceEndpoint = new ServiceEndpoint()
          {
            Id = id
          };
        return serviceEndpoint;
      }
    }

    internal virtual ServiceEndpoint DeleteServiceEndpoint(Guid id, List<Guid> scopeIds) => this.DeleteServiceEndpoint(id, scopeIds.First<Guid>());

    internal ServiceEndpoint GetServiceEndpoint(Guid id, Guid scopeId)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (GetServiceEndpoint)))
      {
        this.PrepareStoredProcedure("Task.prc_GetServiceEndpoint");
        this.BindGuid("@id", id);
        this.BindInt("@dataspaceId", scopeId.Equals(Guid.Empty) ? 0 : this.GetDataspaceId(scopeId, true));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) this.GetServiceEndpointBinder());
          return resultCollection.GetCurrent<ServiceEndpoint>().Items.FirstOrDefault<ServiceEndpoint>();
        }
      }
    }

    internal virtual IList<ServiceEndpointOAuthConfigurationReference> GetConfigurationIdsOfServiceEndpoints(
      Guid scopeId,
      IList<Guid> serviceEndpointIds)
    {
      return (IList<ServiceEndpointOAuthConfigurationReference>) new List<ServiceEndpointOAuthConfigurationReference>();
    }

    internal virtual async Task<ServiceEndpoint> GetServiceEndpointAsync(Guid id, Guid scopeId)
    {
      ServiceEndpointComponent component = this;
      ServiceEndpoint serviceEndpointAsync;
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) component, nameof (GetServiceEndpointAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetServiceEndpoint");
        component.BindGuid("@id", id);
        component.BindInt("@dataspaceId", scopeId.Equals(Guid.Empty) ? 0 : component.GetDataspaceId(scopeId, true));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) component.GetServiceEndpointBinder());
          serviceEndpointAsync = resultCollection.GetCurrent<ServiceEndpoint>().Items.FirstOrDefault<ServiceEndpoint>();
        }
      }
      return serviceEndpointAsync;
    }

    internal virtual List<ServiceEndpoint> GetServiceEndpoints(
      Guid scopeId,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (GetServiceEndpoints)))
      {
        this.PrepareStoredProcedure("Task.prc_GetServiceEndpoints");
        this.BindDataspaceId(scopeId);
        this.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) this.GetServiceEndpointBinder());
          return resultCollection.GetCurrent<ServiceEndpoint>().Items;
        }
      }
    }

    internal virtual Task<List<ServiceEndpoint>> GetServiceEndpointsAsync(
      Guid scopeId,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed)
    {
      return Task.FromResult<List<ServiceEndpoint>>(this.GetServiceEndpoints(scopeId, type, authSchemes, endpointIds, owner, includeFailed));
    }

    internal virtual async Task<List<ServiceEndpoint>> GetServiceEndpointsAsync(
      Guid scopeId,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<string> endpointNames,
      string owner,
      bool includeFailed)
    {
      HashSet<string> endpointNameSet = new HashSet<string>(endpointNames, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<ServiceEndpoint> serviceEndpointsAsync = await this.GetServiceEndpointsAsync(scopeId, type, authSchemes, (IEnumerable<Guid>) Array.Empty<Guid>(), owner, includeFailed);
      return endpointNameSet.Count != 0 ? serviceEndpointsAsync.Where<ServiceEndpoint>((System.Func<ServiceEndpoint, bool>) (x => endpointNameSet.Contains(x.Name))).ToList<ServiceEndpoint>() : serviceEndpointsAsync;
    }

    internal virtual void UpdateServiceEndpoint(
      Guid scopeId,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
    }

    internal virtual void UpdateServiceEndpoint(
      List<Guid> scopeIds,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
    }

    internal virtual void UpdateServiceEndpoints(
      Guid scopeId,
      List<ServiceEndpoint> serviceEndpoints,
      bool disallowDuplicateEndpointName)
    {
    }

    internal virtual IList<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord> AddServiceEndpointExecutionHistory(
      Guid scopeId,
      ServiceEndpointExecutionRecordsInput input)
    {
      throw new ServiceVersionNotSupportedException();
    }

    internal virtual IList<Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExecutionRecord> GetServiceEndpointExecutionHistory(
      Guid scopeId,
      Guid endpointId,
      int maxCount,
      long continuationToken)
    {
      throw new ServiceVersionNotSupportedException();
    }

    internal virtual IList<ServiceEndpointOAuthConfigurationReference> GetServiceEndpointsByConfigurationId(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      throw new ServiceVersionNotSupportedException();
    }

    internal virtual void MarkOAuth2EndpointsAsDirty(
      IVssRequestContext requestContext,
      Guid serviceEndpointDataspaceId,
      IList<Guid> endpointsToBeMarkedDirty,
      string operationStatus)
    {
      throw new ServiceVersionNotSupportedException();
    }

    internal virtual void ShareServiceEndpoint(Guid endpointId, Guid fromProject, Guid withProject)
    {
    }

    internal virtual void ShareServiceEndpoint(
      Guid endpointId,
      List<ServiceEndpointProjectReference> serviceEndpointProjectReferences)
    {
    }

    internal virtual async Task<List<ServiceEndpointProjectReferenceResult>> QueryServiceEndpointSharedProjectsAsync(
      IEnumerable<Guid> endpointIds,
      Guid project)
    {
      return await Task.FromResult<List<ServiceEndpointProjectReferenceResult>>(new List<ServiceEndpointProjectReferenceResult>());
    }

    internal virtual void UpdateServiceEndpointDuplicateName(
      Guid projectId,
      List<KeyValuePair<Guid, string>> serviceEndPointNames,
      int batchSize)
    {
      throw new ServiceVersionNotSupportedException();
    }

    internal virtual IList<ServiceEndpointPartial> QueryAllServiceEndpoints(Guid projectId) => throw new ServiceVersionNotSupportedException();

    internal virtual Task<List<ServiceEndpoint>> QueryServiceEndpointsAsync(
      Guid scopeId,
      string searchText,
      IEnumerable<Guid> createdBy,
      int top,
      string owner,
      string continuationToken = null)
    {
      throw new ServiceVersionNotSupportedException();
    }

    internal virtual void BindDisallowDuplicateEndpointName(bool disallowDuplicateEndpointName)
    {
    }

    public virtual ServiceEndpointBinder GetServiceEndpointBinder() => new ServiceEndpointBinder();

    public virtual void DeleteTeamProject(Guid projectId)
    {
    }

    public virtual ServiceEndpointPartialBinder GetServiceEndpointPartialBinder() => new ServiceEndpointPartialBinder();
  }
}
