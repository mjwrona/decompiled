// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PurgedBuildsResults
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class PurgedBuildsResults
  {
    public PurgedBuildsResults()
    {
      this.PurgedArtifacts = (IReadOnlyCollection<BuildArtifact>) Array.Empty<BuildArtifact>();
      this.PurgedBuilds = (IReadOnlyCollection<BuildData>) Array.Empty<BuildData>();
    }

    public PurgedBuildsResults(
      IReadOnlyCollection<BuildArtifact> purgedArtifacts,
      IReadOnlyCollection<BuildData> purgedBuilds)
    {
      this.PurgedArtifacts = purgedArtifacts;
      this.PurgedBuilds = purgedBuilds;
    }

    public PurgedBuildsResults(ResultCollection rc)
    {
      this.PurgedArtifacts = (IReadOnlyCollection<BuildArtifact>) rc.GetCurrent<BuildArtifact>().Items.GetUniqueArtifacts().AsReadOnly();
      rc.NextResult();
      this.PurgedBuilds = (IReadOnlyCollection<BuildData>) rc.GetCurrent<BuildData>().Items;
      if (!rc.TryNextResult())
        return;
      Dictionary<int, BuildData> dictionary = this.PurgedBuilds.ToDictionary<BuildData, int>((Func<BuildData, int>) (b => b.Id));
      foreach (BuildOrchestrationData orchestrationData in rc.GetCurrent<BuildOrchestrationData>())
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

    public IReadOnlyCollection<BuildArtifact> PurgedArtifacts { get; }

    public IReadOnlyCollection<BuildData> PurgedBuilds { get; }
  }
}
