// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component51
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
  internal class Build2Component51 : Build2Component50
  {
    public override async Task<UpdateBuildsResult> ResetBuildStateAsync(
      Guid projectId,
      int buildId,
      Guid requestedBy)
    {
      Build2Component51 build2Component51 = this;
      build2Component51.TraceEnter(0, nameof (ResetBuildStateAsync));
      build2Component51.PrepareStoredProcedure("Build.prc_ResetBuildState");
      build2Component51.BindInt("@dataspaceId", build2Component51.GetDataspaceId(projectId));
      build2Component51.BindInt("@buildId", buildId);
      build2Component51.BindGuid("@requestedBy", requestedBy);
      UpdateBuildsResult result = new UpdateBuildsResult();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component51.ExecuteReaderAsync(), build2Component51.ProcedureName, build2Component51.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(build2Component51.GetBuildDataBinder());
        resultCollection.AddBinder<BuildData>(build2Component51.GetBuildDataBinder());
        resultCollection.AddBinder<BuildDefinition>(build2Component51.GetBuildDefinitionBinder());
        resultCollection.AddBinder<BuildTagData>(build2Component51.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(build2Component51.GetBuildOrchestrationDataBinder());
        result.OldBuilds = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
        resultCollection.NextResult();
        result.NewBuilds = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
        Dictionary<int, BuildData> dictionary = result.NewBuilds.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
        resultCollection.NextResult();
        result.DefinitionsById = (IDictionary<int, BuildDefinition>) resultCollection.GetCurrent<BuildDefinition>().Items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (d => d.Id));
        resultCollection.NextResult();
        foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
        {
          BuildData buildData;
          if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
            buildData.Tags.Add(buildTagData.Tag);
        }
        resultCollection.NextResult();
        foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
        {
          BuildData buildData;
          if (dictionary.TryGetValue(orchestrationData.BuildId, out buildData))
          {
            int? orchestrationType = orchestrationData.Plan.OrchestrationType;
            int num = 1;
            if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
              buildData.OrchestrationPlan = orchestrationData.Plan;
            buildData.Plans.Add(orchestrationData.Plan);
          }
        }
      }
      build2Component51.TraceLeave(0, nameof (ResetBuildStateAsync));
      UpdateBuildsResult updateBuildsResult = result;
      result = new UpdateBuildsResult();
      return updateBuildsResult;
    }
  }
}
