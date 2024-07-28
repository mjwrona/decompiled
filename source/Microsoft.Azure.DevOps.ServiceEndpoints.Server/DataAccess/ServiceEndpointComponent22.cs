// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent22
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent22 : ServiceEndpointComponent21
  {
    internal override void UpdateServiceEndpoint(
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
      this.UpdateServiceEndpoint(scopeIds, serviceEndpoint, disallowDuplicateEndpointName);
    }

    internal override void UpdateServiceEndpoint(
      List<Guid> scopeIds,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (UpdateServiceEndpoint)))
      {
        Dictionary<string, object> auditData = new Dictionary<string, object>();
        serviceEndpoint.Data.ForEach<KeyValuePair<string, string>>((Action<KeyValuePair<string, string>>) (x => auditData.Add(x.Key, (object) x.Value)));
        if (auditData.ContainsKey("azureSpnPermissions"))
          auditData.Remove("azureSpnPermissions");
        Guid projectId = scopeIds.Count == 1 ? scopeIds[0] : new Guid();
        string actionId = projectId != new Guid() ? ServiceConnectionAuditConstants.ServiceConnectionForProjectModified : ServiceConnectionAuditConstants.ServiceConnectionModified;
        List<string> excludeParameters = new List<string>()
        {
          "@authorizationParameters"
        };
        if (projectId == new Guid())
          excludeParameters.Add("@serviceEndpointProjectReferences");
        this.PrepareForAuditingAction(actionId, auditData, projectId, excludeParameters: (IEnumerable<string>) excludeParameters);
        this.PrepareStoredProcedure("Task.prc_UpdateServiceEndpoint");
        Guid parameterValue = serviceEndpoint.RemoveAuthConfigurationIfRequired();
        this.BindGuid("@id", serviceEndpoint.Id);
        this.BindString("@name", serviceEndpoint.Name.Trim(), 512, false, SqlDbType.NVarChar);
        this.BindString("@type", serviceEndpoint.Type.Trim(), 128, false, SqlDbType.NVarChar);
        this.BindString("@description", serviceEndpoint.Description, int.MaxValue, true, SqlDbType.NVarChar);
        this.BindString("@authorizationScheme", serviceEndpoint.Authorization.Scheme, 128, false, SqlDbType.NVarChar);
        this.BindString("@authorizationParameters", JsonUtility.ToString((object) serviceEndpoint.Authorization.Parameters), int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@data", JsonUtility.ToString((object) serviceEndpoint.Data), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindBoolean("@isReady", serviceEndpoint.IsReady);
        this.BindString("@operationStatus", JsonUtility.ToString<JToken>((IList<JToken>) serviceEndpoint.OperationStatus), int.MaxValue, true, SqlDbType.NVarChar);
        this.BindNullableGuid("@configurationId", parameterValue);
        this.BindString("@url", serviceEndpoint.Url?.OriginalString, int.MaxValue, true, SqlDbType.NVarChar);
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
          rows = source != null ? source.Select<ServiceEndpointProjectReference, SqlDataRecord>(new System.Func<ServiceEndpointProjectReference, SqlDataRecord>(((ServiceEndpointComponent21) this).ConvertToSqlDataRecord)) : (IEnumerable<SqlDataRecord>) null;
        }
        this.BindTable("@serviceEndpointProjectReferences", "Task.typ_ServiceEndpointProjectReferenceTable", rows, false);
        this.ExecuteNonQuery();
      }
    }
  }
}
