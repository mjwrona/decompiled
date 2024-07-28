// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent26
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent26 : ServiceEndpointComponent25
  {
    private static readonly SqlMetaData[] ServiceEndpointTableType5 = new SqlMetaData[16]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Type", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Url", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("GroupScopeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AdministratorsGroupId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ReadersGroupId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AuthorizationScheme", SqlDbType.NVarChar, 128L),
      new SqlMetaData("AuthorizationParameters", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IsReady", SqlDbType.Bit),
      new SqlMetaData("IsDisabled", SqlDbType.Bit),
      new SqlMetaData("OperationStatus", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Owner", SqlDbType.Int)
    };

    protected override SqlParameter BindServiceEndpointTable(
      string parameterName,
      IEnumerable<ServiceEndpoint> rows)
    {
      return this.BindTable(parameterName, "Task.typ_ServiceEndpointTable5", (rows ?? Enumerable.Empty<ServiceEndpoint>()).Select<ServiceEndpoint, SqlDataRecord>(new System.Func<ServiceEndpoint, SqlDataRecord>(((ServiceEndpointComponent7) this).ConvertToServiceEndpointTableSqlDataRecord)));
    }

    protected override SqlDataRecord ConvertToServiceEndpointTableSqlDataRecord(ServiceEndpoint row)
    {
      SqlDataRecord record = new SqlDataRecord(ServiceEndpointComponent26.ServiceEndpointTableType5);
      record.SetGuid(0, row.Id);
      record.SetString(1, row.Name);
      record.SetNullableString(2, row.Type);
      record.SetNullableString(3, row.Url == (Uri) null ? (string) null : row.Url?.AbsoluteUri);
      record.SetNullableString(4, row.Description);
      record.SetGuid(5, row.CreatedBy != null ? new Guid(row.CreatedBy.Id) : Guid.Empty);
      Guid groupScopeId = row.GroupScopeId;
      record.SetGuid(6, row.GroupScopeId);
      record.SetGuid(7, row.AdministratorsGroup?.Id != null ? new Guid(row.AdministratorsGroup?.Id) : Guid.Empty);
      record.SetGuid(8, row.ReadersGroup?.Id != null ? new Guid(row.ReadersGroup?.Id) : Guid.Empty);
      record.SetString(9, row.Authorization.Scheme);
      record.SetString(10, JsonUtility.ToString((object) row.Authorization.Parameters));
      record.SetString(11, JsonUtility.ToString((object) row.Data));
      record.SetBoolean(12, row.IsReady);
      record.SetBoolean(13, row.IsDisabled);
      record.SetNullableString(14, JsonUtility.ToString<JToken>((IList<JToken>) row.OperationStatus));
      EndpointOwner result;
      EnumUtility.TryParse<EndpointOwner>(row.Owner, true, out result);
      record.SetInt32(15, (int) result);
      return record;
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
        this.BindBoolean("@isDisabled", serviceEndpoint.IsDisabled);
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
