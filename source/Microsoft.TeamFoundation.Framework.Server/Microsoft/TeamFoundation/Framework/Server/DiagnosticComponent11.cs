// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent11 : DiagnosticComponent10
  {
    public override List<ResourceSemaphoreInfo> QueryResourceSemaphores()
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryResourceSemaphores");
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, "DIAGNOSTIC.prc_QueryResourceSemaphores", this.RequestContext))
        {
          resultCollection.AddBinder<ResourceSemaphoreInfo>((ObjectBinder<ResourceSemaphoreInfo>) new ResourceSemaphoreInfoBinder());
          return resultCollection.GetCurrent<ResourceSemaphoreInfo>().Items;
        }
      }
    }

    public override List<QueryOptimizerMemoryGatewaysInfo> QueryOptimizerMemoryGateways()
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryOptimizerMemoryGateways");
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, "DIAGNOSTIC.prc_QueryOptimizerMemoryGateways", this.RequestContext))
        {
          resultCollection.AddBinder<QueryOptimizerMemoryGatewaysInfo>((ObjectBinder<QueryOptimizerMemoryGatewaysInfo>) new QueryOptimizerMemoryGatewaysInfoBinder());
          return resultCollection.GetCurrent<QueryOptimizerMemoryGatewaysInfo>().Items;
        }
      }
    }

    public override List<PerformanceCounterInfo> QueryPerformanceCounters()
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryPerformanceCounters");
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, "DIAGNOSTIC.prc_QueryPerformanceCounters", this.RequestContext))
        {
          resultCollection.AddBinder<PerformanceCounterInfo>((ObjectBinder<PerformanceCounterInfo>) new PerformanceCounterInfoBinder());
          return resultCollection.GetCurrent<PerformanceCounterInfo>().Items;
        }
      }
    }
  }
}
