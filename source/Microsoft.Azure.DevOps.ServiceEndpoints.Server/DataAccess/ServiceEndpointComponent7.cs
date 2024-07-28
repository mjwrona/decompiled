// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent7
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent7 : ServiceEndpointComponent6
  {
    private static readonly SqlMetaData[] typ_ServiceEndpointTable = new SqlMetaData[10]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Type", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Url", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AuthorizationScheme", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IsReady", SqlDbType.Bit),
      new SqlMetaData("OperationStatus", SqlDbType.NVarChar, -1L)
    };

    public ServiceEndpointComponent7() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    internal override List<ServiceEndpoint> GetServiceEndpoints(
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
        this.BindInt("@dataspaceId", scopeId.Equals(Guid.Empty) ? 0 : this.GetDataspaceId(scopeId, true));
        this.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindStringTable("@authSchemeTable", authSchemes != null ? authSchemes.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null);
        this.BindGuidTable("@endpointIdsTable", endpointIds != null ? endpointIds.Distinct<Guid>() : (IEnumerable<Guid>) null);
        this.BindBoolean("@includeFailed", includeFailed);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) this.GetServiceEndpointBinder());
          return resultCollection.GetCurrent<ServiceEndpoint>().Items;
        }
      }
    }

    internal override async Task<List<ServiceEndpoint>> GetServiceEndpointsAsync(
      Guid scopeId,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed)
    {
      ServiceEndpointComponent7 component1 = this;
      List<ServiceEndpoint> items;
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) component1, nameof (GetServiceEndpointsAsync)))
      {
        component1.PrepareStoredProcedure("Task.prc_GetServiceEndpoints");
        component1.BindInt("@dataspaceId", scopeId.Equals(Guid.Empty) ? 0 : component1.GetDataspaceId(scopeId, true));
        component1.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        ServiceEndpointComponent7 component2 = component1;
        IEnumerable<string> source1 = authSchemes;
        IEnumerable<string> rows1 = source1 != null ? source1.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null;
        component2.BindStringTable("@authSchemeTable", rows1);
        ServiceEndpointComponent7 component3 = component1;
        IEnumerable<Guid> source2 = endpointIds;
        IEnumerable<Guid> rows2 = source2 != null ? source2.Distinct<Guid>() : (IEnumerable<Guid>) null;
        component3.BindGuidTable("@endpointIdsTable", rows2);
        component1.BindBoolean("@includeFailed", includeFailed);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component1.ExecuteReaderAsync(), component1.ProcedureName, component1.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) component1.GetServiceEndpointBinder());
          items = resultCollection.GetCurrent<ServiceEndpoint>().Items;
        }
      }
      return items;
    }

    internal override void UpdateServiceEndpoints(
      Guid scopeId,
      List<ServiceEndpoint> serviceEndpoints,
      bool disallowDuplicateEndpointName)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (UpdateServiceEndpoints)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateServiceEndpoints");
        this.BindDataspaceId(scopeId);
        this.BindServiceEndpointTable("@endpoints", (IEnumerable<ServiceEndpoint>) serviceEndpoints);
        this.ExecuteNonQuery();
      }
    }

    protected virtual SqlParameter BindServiceEndpointTable(
      string parameterName,
      IEnumerable<ServiceEndpoint> rows)
    {
      return this.BindTable(parameterName, "Task.typ_ServiceEndpointTable", (rows ?? Enumerable.Empty<ServiceEndpoint>()).Select<ServiceEndpoint, SqlDataRecord>(new System.Func<ServiceEndpoint, SqlDataRecord>(this.ConvertToServiceEndpointTableSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertToServiceEndpointTableSqlDataRecord(ServiceEndpoint row)
    {
      SqlDataRecord record = new SqlDataRecord(ServiceEndpointComponent7.typ_ServiceEndpointTable);
      record.SetGuid(0, row.Id);
      record.SetString(1, row.Name);
      record.SetNullableString(2, row.Type);
      record.SetNullableString(3, row.Url == (Uri) null ? (string) null : row.Url?.AbsoluteUri);
      record.SetNullableString(4, row.Description);
      record.SetGuid(5, row.CreatedBy != null ? new Guid(row.CreatedBy.Id) : Guid.Empty);
      record.SetString(6, row.Authorization.Scheme);
      record.SetString(7, JsonUtility.ToString((object) row.Data));
      record.SetBoolean(8, row.IsReady);
      record.SetNullableString(9, JsonUtility.ToString<JToken>((IList<JToken>) row.OperationStatus));
      return record;
    }
  }
}
