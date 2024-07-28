// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component63
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component63 : Build2Component62
  {
    public override async Task<List<ShallowBuildAnalyticsData>> GetShallowBuildAnaltyticsDataByDateAsync(
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      Build2Component63 build2Component63 = this;
      List<ShallowBuildAnalyticsData> items;
      using (build2Component63.TraceScope(method: nameof (GetShallowBuildAnaltyticsDataByDateAsync)))
      {
        build2Component63.PrepareStoredProcedure("Build.prc_GetBuildIdsAndChangedOnByDate");
        build2Component63.BindInt("@dataspaceId", dataspaceId);
        build2Component63.BindInt("@batchSize", batchSize);
        build2Component63.BindNullableDateTime("@fromDate", fromDate);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component63.ExecuteReaderAsync(), build2Component63.ProcedureName, build2Component63.RequestContext))
        {
          resultCollection.AddBinder<ShallowBuildAnalyticsData>(build2Component63.GetShallowBuildAnalyticsDataBinder(build2Component63.GetDataspaceIdentifier(dataspaceId)));
          items = resultCollection.GetCurrent<ShallowBuildAnalyticsData>().Items;
        }
      }
      return items;
    }
  }
}
