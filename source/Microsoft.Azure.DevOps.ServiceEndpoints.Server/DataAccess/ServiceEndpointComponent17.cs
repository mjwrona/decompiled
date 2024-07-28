// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent17
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent17 : ServiceEndpointComponent16
  {
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
        EndpointOwner result;
        Enum.TryParse<EndpointOwner>(owner, true, out result);
        this.BindInt("@owner", (int) result);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) this.GetServiceEndpointBinder());
          return resultCollection.GetCurrent<ServiceEndpoint>().Items;
        }
      }
    }

    internal override Task<List<ServiceEndpoint>> GetServiceEndpointsAsync(
      Guid scopeId,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<Guid> endpointIds,
      string owner,
      bool includeFailed)
    {
      return Task.FromResult<List<ServiceEndpoint>>(this.GetServiceEndpoints(scopeId, type, authSchemes, endpointIds, owner, includeFailed));
    }

    internal override async Task<List<ServiceEndpoint>> GetServiceEndpointsAsync(
      Guid scopeId,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<string> endpointNames,
      string owner,
      bool includeFailed)
    {
      ServiceEndpointComponent17 component1 = this;
      List<ServiceEndpoint> items;
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) component1, nameof (GetServiceEndpointsAsync)))
      {
        component1.PrepareStoredProcedure("Task.prc_GetServiceEndpointsByName");
        component1.BindInt("@dataspaceId", scopeId.Equals(Guid.Empty) ? 0 : component1.GetDataspaceId(scopeId, true));
        component1.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        ServiceEndpointComponent17 component2 = component1;
        IEnumerable<string> source1 = authSchemes;
        IEnumerable<string> rows1 = source1 != null ? source1.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null;
        component2.BindStringTable("@authSchemeTable", rows1);
        ServiceEndpointComponent17 component3 = component1;
        IEnumerable<string> source2 = endpointNames;
        IEnumerable<string> rows2 = source2 != null ? source2.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null;
        component3.BindStringTable("@endpointNamesTable", rows2);
        component1.BindBoolean("@includeFailed", includeFailed);
        EndpointOwner result;
        Enum.TryParse<EndpointOwner>(owner, true, out result);
        component1.BindInt("@owner", (int) result);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component1.ExecuteReaderAsync(), component1.ProcedureName, component1.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) component1.GetServiceEndpointBinder());
          items = resultCollection.GetCurrent<ServiceEndpoint>().Items;
        }
      }
      return items;
    }

    internal override async Task<List<ServiceEndpoint>> QueryServiceEndpointsAsync(
      Guid scopeId,
      string searchText,
      IEnumerable<Guid> createdBy,
      int top,
      string owner,
      string continuationToken = null)
    {
      ServiceEndpointComponent17 component = this;
      List<ServiceEndpoint> items;
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) component, nameof (QueryServiceEndpointsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_QueryServiceEndpoints");
        component.BindInt("@dataspaceId", scopeId.Equals(Guid.Empty) ? 0 : component.GetDataspaceId(scopeId, true));
        component.BindString("@searchText", searchText, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindGuidTable("@createdByIdTable", createdBy);
        EndpointOwner result;
        Enum.TryParse<EndpointOwner>(owner, true, out result);
        component.BindInt("@owner", (int) result);
        component.BindInt("@top", top);
        component.BindString("@continuationToken", continuationToken, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) component.GetServiceEndpointBinder());
          items = resultCollection.GetCurrent<ServiceEndpoint>().Items;
        }
      }
      return items;
    }
  }
}
