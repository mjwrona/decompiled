// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent24
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent24 : ServiceEndpointComponent23
  {
    internal override IList<ServiceEndpointPartial> QueryAllServiceEndpoints(Guid projectId)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (QueryAllServiceEndpoints)))
      {
        this.PrepareStoredProcedure("Task.prc_QueryAllServiceEndpoints");
        this.BindDataspaceId(projectId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpointPartial>((ObjectBinder<ServiceEndpointPartial>) this.GetServiceEndpointPartialBinder());
          return (IList<ServiceEndpointPartial>) resultCollection.GetCurrent<ServiceEndpointPartial>().Items;
        }
      }
    }

    internal override void UpdateServiceEndpointDuplicateName(
      Guid projectId,
      List<KeyValuePair<Guid, string>> serviceEndPointNames,
      int batchSize)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (UpdateServiceEndpointDuplicateName)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateServiceEndpointDuplicateName");
        this.BindDataspaceId(projectId);
        this.BindInt("@batchSize", batchSize);
        this.BindKeyValuePairGuidStringTable("@endpointIdNamePairTable", (IEnumerable<KeyValuePair<Guid, string>>) serviceEndPointNames);
        this.ExecuteNonQuery();
      }
    }
  }
}
