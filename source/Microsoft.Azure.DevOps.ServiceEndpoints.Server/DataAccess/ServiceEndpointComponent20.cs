// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.ServiceEndpointComponent20
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  internal class ServiceEndpointComponent20 : ServiceEndpointComponent19
  {
    internal override IList<ServiceEndpointExecutionRecord> GetServiceEndpointExecutionHistory(
      Guid scopeId,
      Guid endpointId,
      int maxCount,
      long continuationToken)
    {
      using (new ServiceEndpointsSqlComponentBase.SqlMethodScope((ServiceEndpointsSqlComponentBase) this, nameof (GetServiceEndpointExecutionHistory)))
      {
        this.PrepareStoredProcedure("Task.prc_GetServiceEndpointExecutionHistory");
        this.BindGuid("@endpointId", endpointId);
        this.BindDataspaceId(scopeId);
        this.BindInt("@maxCount", maxCount);
        this.BindLong("@continuationToken", continuationToken);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ServiceEndpointExecutionRecord>((ObjectBinder<ServiceEndpointExecutionRecord>) new ServiceEndpointExecutionRecordBinder());
          return (IList<ServiceEndpointExecutionRecord>) resultCollection.GetCurrent<ServiceEndpointExecutionRecord>().Items;
        }
      }
    }
  }
}
