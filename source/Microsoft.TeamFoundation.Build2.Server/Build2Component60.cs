// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component60
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
  internal class Build2Component60 : Build2Component59
  {
    public override async Task<PurgedBuildsResults> PurgeArtifactsAsync(
      Guid projectId,
      int daysOld,
      int batchSize)
    {
      Build2Component60 component = this;
      PurgedBuildsResults purgedBuildsResults;
      using (component.TraceScope(method: nameof (PurgeArtifactsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_PurgeArtifacts");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@daysOld", daysOld);
        component.BindInt("@batchSize", batchSize);
        component.BindUniqueInt32Table("@excludedDefinitionIds", (IEnumerable<int>) new HashSet<int>());
        using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          rc.AddBinder<BuildArtifact>(component.GetBuildArtifactBinder());
          rc.AddBinder<BuildData>(component.GetBuildDataBinder());
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
      Build2Component60 component = this;
      PurgedBuildsResults purgedBuildsResults;
      using (component.TraceScope(method: nameof (PurgeBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_PurgeBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@daysOld", daysOld);
        component.BindInt("@batchSize", batchSize);
        component.BindString("@branchPrefix", branchPrefix, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindUniqueInt32Table("@excludedDefinitionIds", (IEnumerable<int>) new HashSet<int>());
        using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          rc.AddBinder<BuildArtifact>(component.GetBuildArtifactBinder());
          rc.AddBinder<BuildData>(component.GetBuildDataBinder());
          purgedBuildsResults = new PurgedBuildsResults(rc);
        }
      }
      return purgedBuildsResults;
    }
  }
}
