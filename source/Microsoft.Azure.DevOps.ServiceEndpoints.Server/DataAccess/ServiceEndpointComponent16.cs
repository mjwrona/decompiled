// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent16
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
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
  internal class ServiceEndpointComponent16 : ServiceEndpointComponent15
  {
    private static readonly SqlMetaData[] ServiceEndpointTableType3 = new SqlMetaData[12]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Type", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Url", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AuthorizationScheme", SqlDbType.NVarChar, 128L),
      new SqlMetaData("AuthorizationParameters", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IsReady", SqlDbType.Bit),
      new SqlMetaData("OperationStatus", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ConfigurationId", SqlDbType.UniqueIdentifier)
    };

    internal override void AddServiceEndpoint(
      Guid scopeId,
      ServiceEndpoint serviceEndpoint,
      bool disallowDuplicateEndpointName)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (AddServiceEndpoint)))
      {
        this.PrepareStoredProcedure("Task.prc_AddServiceEndpoint");
        Guid parameterValue = serviceEndpoint.RemoveAuthConfigurationIfRequired();
        this.BindInt("@dataspaceId", scopeId.Equals(Guid.Empty) ? 0 : this.GetDataspaceId(scopeId, true));
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
        this.ExecuteNonQuery();
      }
    }

    internal override void UpdateServiceEndpoints(
      Guid scopeId,
      List<ServiceEndpoint> serviceEndpoints,
      bool disallowDuplicateEndpointName)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (UpdateServiceEndpoints)))
      {
        this.PrepareForAuditingAction(ServiceConnectionAuditConstants.ServiceConnectionForProjectModified, projectId: scopeId);
        this.PrepareStoredProcedure("Task.prc_UpdateServiceEndpoints");
        this.BindDataspaceId(scopeId);
        this.BindServiceEndpointTable("@endpoints", (IEnumerable<ServiceEndpoint>) serviceEndpoints);
        this.BindDisallowDuplicateEndpointName(disallowDuplicateEndpointName);
        this.ExecuteNonQuery();
      }
    }

    protected override SqlParameter BindServiceEndpointTable(
      string parameterName,
      IEnumerable<ServiceEndpoint> rows)
    {
      return this.BindTable(parameterName, "Task.typ_ServiceEndpointTable3", (rows ?? Enumerable.Empty<ServiceEndpoint>()).Select<ServiceEndpoint, SqlDataRecord>(new System.Func<ServiceEndpoint, SqlDataRecord>(((ServiceEndpointComponent7) this).ConvertToServiceEndpointTableSqlDataRecord)));
    }

    protected override SqlDataRecord ConvertToServiceEndpointTableSqlDataRecord(ServiceEndpoint row)
    {
      Guid guid = row.RemoveAuthConfigurationIfRequired();
      SqlDataRecord record = new SqlDataRecord(ServiceEndpointComponent16.ServiceEndpointTableType3);
      record.SetGuid(0, row.Id);
      record.SetString(1, row.Name);
      record.SetNullableString(2, row.Type);
      record.SetNullableString(3, row.Url == (Uri) null ? (string) null : row.Url?.AbsoluteUri);
      record.SetNullableString(4, row.Description);
      record.SetGuid(5, row.CreatedBy != null ? new Guid(row.CreatedBy.Id) : Guid.Empty);
      record.SetString(6, row.Authorization.Scheme);
      record.SetString(7, JsonUtility.ToString((object) row.Authorization.Parameters));
      record.SetString(8, JsonUtility.ToString((object) row.Data));
      record.SetBoolean(9, row.IsReady);
      record.SetNullableString(10, JsonUtility.ToString<JToken>((IList<JToken>) row.OperationStatus));
      record.SetNullableGuid(11, guid);
      return record;
    }
  }
}
