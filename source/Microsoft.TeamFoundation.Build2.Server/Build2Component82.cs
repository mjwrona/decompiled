// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component82
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component82 : Build2Component81
  {
    public override async Task<ILookup<Guid, long>> GetOrphanedBuildContainersAsync(
      DateTime minDate,
      int maxContainers)
    {
      Build2Component82 build2Component82 = this;
      ILookup<Guid, long> lookup;
      using (build2Component82.TraceScope(method: nameof (GetOrphanedBuildContainersAsync)))
      {
        build2Component82.PrepareStoredProcedure("Build.prc_GetOrphanedBuildContainers", 3600);
        build2Component82.BindDateTime2("@minDate", minDate);
        build2Component82.BindInt("@maxContainers", maxContainers);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component82.ExecuteReaderAsync(), build2Component82.ProcedureName, build2Component82.RequestContext))
        {
          resultCollection.AddBinder<OrphanedBuildContainer>(build2Component82.GetOrphanedBuildContainerBinder());
          lookup = resultCollection.GetCurrent<OrphanedBuildContainer>().ToLookup<OrphanedBuildContainer, Guid, long>((System.Func<OrphanedBuildContainer, Guid>) (obc => obc.ProjectId), (System.Func<OrphanedBuildContainer, long>) (obc => obc.ContainerId));
        }
      }
      return lookup;
    }

    public override void SampleRetentionData(int retentionDays)
    {
      using (this.TraceScope(method: nameof (SampleRetentionData)))
      {
        this.PrepareStoredProcedure("Build.prc_SampleRetentionData", 3600);
        this.BindInt("@retentionDays", retentionDays);
        this.ExecuteNonQuery();
      }
    }

    public override IEnumerable<BuildRetentionSample> GetRetentionHistory(int lookbackDays)
    {
      using (this.TraceScope(method: nameof (GetRetentionHistory)))
      {
        this.PrepareStoredProcedure("Build.prc_GetRetentionHistory");
        this.BindInt("@lookbackDays", lookbackDays);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildRetentionSample>(this.GetBuildRetentionSampleBinder());
          return (IEnumerable<BuildRetentionSample>) resultCollection.GetCurrent<BuildRetentionSample>().Items;
        }
      }
    }
  }
}
