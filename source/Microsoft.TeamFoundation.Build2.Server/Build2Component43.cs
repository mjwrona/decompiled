// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component43
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component43 : Build2Component42
  {
    public override BuildDefinition RestoreDefinition(
      Guid projectId,
      int definitionId,
      Guid authorId,
      string comment = null)
    {
      this.TraceEnter(0, nameof (RestoreDefinition));
      this.PrepareStoredProcedure("Build.prc_RestoreDefinition");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.BindGuid("@requestedBy", authorId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        BuildDefinition buildDefinition = resultCollection.GetCurrent<BuildDefinition>().SingleOrDefault<BuildDefinition>();
        this.TraceLeave(0, nameof (RestoreDefinition));
        return buildDefinition;
      }
    }

    public override BuildDefinition GetDeletedDefinition(Guid projectId, int definitionId)
    {
      this.TraceEnter(0, nameof (GetDeletedDefinition));
      this.PrepareStoredProcedure("Build.prc_GetDeletedDefinition");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        BuildDefinition deletedDefinition = resultCollection.GetCurrent<BuildDefinition>().SingleOrDefault<BuildDefinition>();
        this.TraceLeave(0, nameof (GetDeletedDefinition));
        return deletedDefinition;
      }
    }

    public override List<BuildDefinition> GetRecentlyBuiltDefinitions(
      Guid projectId,
      int top,
      bool includeQueuedBuilds)
    {
      this.TraceEnter(0, nameof (GetRecentlyBuiltDefinitions));
      this.PrepareStoredProcedure("Build.prc_GetRecentlyBuiltDefinitions");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@top", top);
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
  }
}
