// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent21
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent21 : ServiceEndpointComponent20
  {
    protected static readonly SqlMetaData[] typ_ServiceEndpointProjectReference = new SqlMetaData[3]
    {
      new SqlMetaData("@dataspaceId", SqlDbType.Int),
      new SqlMetaData("@name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("@description", SqlDbType.NVarChar, 1024L)
    };

    internal override void AddServiceEndpoint(
      Guid scopeId,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
      List<Guid> guidList;
      if (!(scopeId != Guid.Empty))
      {
        guidList = new List<Guid>();
      }
      else
      {
        guidList = new List<Guid>();
        guidList.Add(scopeId);
      }
      List<Guid> scopeIds = guidList;
      if (scopeId != Guid.Empty)
        serviceEndpoint.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) new List<ServiceEndpointProjectReference>()
        {
          new ServiceEndpointProjectReference()
          {
            Description = serviceEndpoint.Description,
            Name = serviceEndpoint.Name,
            ProjectReference = new ProjectReference()
            {
              Id = scopeId
            }
          }
        };
      this.AddServiceEndpoint(scopeIds, serviceEndpoint, disallowDuplicateEndpointName);
    }

    internal override void AddServiceEndpoint(
      List<Guid> scopeIds,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (AddServiceEndpoint)))
      {
        Guid projectId = scopeIds.Count == 1 ? scopeIds[0] : new Guid();
        string actionId = projectId != new Guid() ? ServiceConnectionAuditConstants.ServiceConnectionCreated : ServiceConnectionAuditConstants.ServiceConnectionCreatedForMultipleProjects;
        List<string> excludeParameters = new List<string>()
        {
          "@authorizationParameters"
        };
        if (projectId == new Guid())
          excludeParameters.Add("@serviceEndpointProjectReferences");
        this.PrepareForAuditingAction(actionId, projectId: projectId, excludeParameters: (IEnumerable<string>) excludeParameters);
        this.PrepareStoredProcedure("Task.prc_AddServiceEndpoint");
        Guid parameterValue = serviceEndpoint.RemoveAuthConfigurationIfRequired();
        this.BindGuid("@id", serviceEndpoint.Id);
        this.BindString("@name", serviceEndpoint.Name.Trim(), 512, false, SqlDbType.NVarChar);
        this.BindString("@type", serviceEndpoint.Type.Trim(), 128, false, SqlDbType.NVarChar);
        if (serviceEndpoint.Authorization.Scheme.Equals("OAuth2", StringComparison.OrdinalIgnoreCase))
        {
          serviceEndpoint.Url = (Uri) null;
          this.BindString("@url", (string) null, int.MaxValue, true, SqlDbType.NVarChar);
        }
        else
          this.BindString("@url", serviceEndpoint.Url.OriginalString, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@description", serviceEndpoint.Description, int.MaxValue, true, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(serviceEndpoint.CreatedBy.Id));
        this.BindString("@authorizationScheme", serviceEndpoint.Authorization.Scheme, 128, false, SqlDbType.NVarChar);
        this.BindString("@authorizationParameters", JsonUtility.ToString((object) serviceEndpoint.Authorization.Parameters), int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@data", JsonUtility.ToString((object) serviceEndpoint.Data), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindBoolean("@isReady", serviceEndpoint.IsReady);
        this.BindString("@operationStatus", JsonUtility.ToString<JToken>((IList<JToken>) serviceEndpoint.OperationStatus), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindNullableGuid("@configurationId", parameterValue);
        EndpointOwner result;
        Enum.TryParse<EndpointOwner>(serviceEndpoint.Owner, true, out result);
        this.BindInt("@owner", (int) result);
        this.BindDisallowDuplicateEndpointName(disallowDuplicateEndpointName);
        IList<ServiceEndpointProjectReference> projectReferences = serviceEndpoint.ServiceEndpointProjectReferences;
        IEnumerable<SqlDataRecord> rows;
        if (projectReferences == null)
        {
          rows = (IEnumerable<SqlDataRecord>) null;
        }
        else
        {
          IEnumerable<ServiceEndpointProjectReference> source = projectReferences.Where<ServiceEndpointProjectReference>((System.Func<ServiceEndpointProjectReference, bool>) (x => x?.ProjectReference != null));
          rows = source != null ? source.Select<ServiceEndpointProjectReference, SqlDataRecord>(new System.Func<ServiceEndpointProjectReference, SqlDataRecord>(this.ConvertToSqlDataRecord)) : (IEnumerable<SqlDataRecord>) null;
        }
        this.BindTable("@serviceEndpointProjectReferences", "Task.typ_ServiceEndpointProjectReferenceTable", rows, false);
        this.ExecuteNonQuery();
      }
    }

    internal override ServiceEndpoint DeleteServiceEndpoint(Guid id, Guid scopeId)
    {
      List<Guid> guidList;
      if (!(scopeId != Guid.Empty))
      {
        guidList = new List<Guid>();
      }
      else
      {
        guidList = new List<Guid>();
        guidList.Add(scopeId);
      }
      List<Guid> scopeIds = guidList;
      return this.DeleteServiceEndpoint(id, scopeIds);
    }

    internal override ServiceEndpoint DeleteServiceEndpoint(Guid id, List<Guid> scopeIds)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (DeleteServiceEndpoint)))
      {
        Guid projectId = scopeIds.Count == 1 ? scopeIds[0] : new Guid();
        this.PrepareForAuditingAction(projectId != new Guid() ? ServiceConnectionAuditConstants.ServiceConnectionDeleted : ServiceConnectionAuditConstants.ServiceConnectionDeletedFromMultipleProjects, projectId: projectId);
        this.PrepareStoredProcedure("Task.prc_DeleteServiceEndpoint");
        List<int> rows = new List<int>();
        foreach (Guid scopeId in scopeIds)
          rows.Add(this.GetDataspaceId(scopeId, true));
        this.BindInt32Table("@dataspaceIds", (IEnumerable<int>) rows);
        this.BindGuid("@id", id);
        this.DataspaceRlsEnabled = false;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) this.GetServiceEndpointBinder());
          return resultCollection.GetCurrent<ServiceEndpoint>().Items.FirstOrDefault<ServiceEndpoint>();
        }
      }
    }

    internal override async Task<List<ServiceEndpointProjectReferenceResult>> QueryServiceEndpointSharedProjectsAsync(
      IEnumerable<Guid> endpointIds,
      Guid project)
    {
      ServiceEndpointComponent21 component = this;
      List<ServiceEndpointProjectReferenceResult> items;
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) component, nameof (QueryServiceEndpointSharedProjectsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetServiceEndpointSharedProjects");
        component.BindGuidTable("@endpointIds", endpointIds);
        component.BindInt("@forDataspaceId", project.Equals(Guid.Empty) ? 0 : component.GetDataspaceId(project));
        component.DataspaceRlsEnabled = false;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpointProjectReferenceResult>((ObjectBinder<ServiceEndpointProjectReferenceResult>) new ServiceEndpointProjectReferenceBinder2((ServiceEndpointComponent) component));
          items = resultCollection.GetCurrent<ServiceEndpointProjectReferenceResult>().Items;
        }
      }
      return items;
    }

    protected SqlDataRecord ConvertToSqlDataRecord(
      ServiceEndpointProjectReference serviceEndpointProjectReferences)
    {
      SqlDataRecord record = new SqlDataRecord(ServiceEndpointComponent21.typ_ServiceEndpointProjectReference);
      record.SetInt32(0, this.GetDataspaceId(serviceEndpointProjectReferences.ProjectReference.Id, true));
      record.SetString(1, serviceEndpointProjectReferences.Name?.Trim() ?? "");
      record.SetNullableString(2, serviceEndpointProjectReferences.Description);
      return record;
    }

    protected void StitchServiceEndpointObject(
      ServiceEndpoint endpoint,
      List<ServiceEndpointProjectReference> serviceEndpointProjectReferences)
    {
      if (endpoint == null)
        return;
      endpoint.ServiceEndpointProjectReferences = (IList<ServiceEndpointProjectReference>) serviceEndpointProjectReferences;
      if (serviceEndpointProjectReferences.Count <= 1)
        return;
      endpoint.IsShared = true;
    }
  }
}
