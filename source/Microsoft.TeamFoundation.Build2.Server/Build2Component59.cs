// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component59
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component59 : Build2Component58
  {
    public override List<BuildDefinition> GetRecentlyBuiltDefinitions(
      Guid projectId,
      int top,
      bool includeQueuedBuilds)
    {
      this.TraceEnter(0, nameof (GetRecentlyBuiltDefinitions));
      this.PrepareStoredProcedure("Build.prc_GetRecentlyBuiltDefinitions");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@top", top);
      this.BindBoolean("@includeQueuedBuilds", includeQueuedBuilds);
      this.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) new List<int>());
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildData>(this.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
        this.BindLatestBuilds(rc, dictionary);
        this.TraceLeave(0, nameof (GetRecentlyBuiltDefinitions));
        return items;
      }
    }

    public override async Task<PurgedBuildsResults> PurgeArtifactsAsync(
      Guid projectId,
      int daysOld,
      int batchSize)
    {
      Build2Component59 build2Component59 = this;
      PurgedBuildsResults purgedBuildsResults;
      using (build2Component59.TraceScope(method: nameof (PurgeArtifactsAsync)))
      {
        build2Component59.PrepareStoredProcedure("Build.prc_PurgeArtifacts");
        build2Component59.BindInt("@dataspaceId", build2Component59.GetDataspaceId(projectId));
        build2Component59.BindInt("@daysOld", daysOld);
        build2Component59.BindInt("@batchSize", batchSize);
        using (ResultCollection rc = new ResultCollection((IDataReader) await build2Component59.ExecuteReaderAsync(), build2Component59.ProcedureName, build2Component59.RequestContext))
        {
          rc.AddBinder<BuildArtifact>(build2Component59.GetBuildArtifactBinder());
          rc.AddBinder<BuildData>(build2Component59.GetBuildDataBinder());
          purgedBuildsResults = new PurgedBuildsResults(rc);
        }
      }
      return purgedBuildsResults;
    }

    public override async Task<PurgedBuildsResults> PurgeBuildsAsync(
      Guid projectId,
      int daysOld,
      int batchSize,
      string branchPrefix)
    {
      Build2Component59 build2Component59 = this;
      PurgedBuildsResults purgedBuildsResults;
      using (build2Component59.TraceScope(method: nameof (PurgeBuildsAsync)))
      {
        build2Component59.PrepareStoredProcedure("Build.prc_PurgeBuilds");
        build2Component59.BindInt("@dataspaceId", build2Component59.GetDataspaceId(projectId));
        build2Component59.BindInt("@daysOld", daysOld);
        build2Component59.BindInt("@batchSize", batchSize);
        build2Component59.BindString("@branchPrefix", branchPrefix, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection rc = new ResultCollection((IDataReader) await build2Component59.ExecuteReaderAsync(), build2Component59.ProcedureName, build2Component59.RequestContext))
        {
          rc.AddBinder<BuildArtifact>(build2Component59.GetBuildArtifactBinder());
          rc.AddBinder<BuildData>(build2Component59.GetBuildDataBinder());
          purgedBuildsResults = new PurgedBuildsResults(rc);
        }
      }
      return purgedBuildsResults;
    }
  }
}
