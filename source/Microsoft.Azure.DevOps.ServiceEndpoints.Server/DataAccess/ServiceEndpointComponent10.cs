// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent10
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
  internal class ServiceEndpointComponent10 : ServiceEndpointComponent9
  {
    internal override async Task<List<ServiceEndpoint>> GetServiceEndpointsAsync(
      Guid scopeId,
      string type,
      IEnumerable<string> authSchemes,
      IEnumerable<string> endpointNames,
      string owner,
      bool includeFailed)
    {
      ServiceEndpointComponent10 component1 = this;
      List<ServiceEndpoint> items;
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) component1, nameof (GetServiceEndpointsAsync)))
      {
        component1.PrepareStoredProcedure("Task.prc_GetServiceEndpointsByName");
        component1.BindInt("@dataspaceId", scopeId.Equals(Guid.Empty) ? 0 : component1.GetDataspaceId(scopeId, true));
        component1.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        ServiceEndpointComponent10 component2 = component1;
        IEnumerable<string> source1 = authSchemes;
        IEnumerable<string> rows1 = source1 != null ? source1.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null;
        component2.BindStringTable("@authSchemeTable", rows1);
        ServiceEndpointComponent10 component3 = component1;
        IEnumerable<string> source2 = endpointNames;
        IEnumerable<string> rows2 = source2 != null ? source2.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null;
        component3.BindStringTable("@endpointNamesTable", rows2);
        component1.BindBoolean("@includeFailed", includeFailed);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component1.ExecuteReaderAsync(), component1.ProcedureName, component1.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpoint>((ObjectBinder<ServiceEndpoint>) component1.GetServiceEndpointBinder());
          items = resultCollection.GetCurrent<ServiceEndpoint>().Items;
        }
      }
      return items;
    }
  }
}
