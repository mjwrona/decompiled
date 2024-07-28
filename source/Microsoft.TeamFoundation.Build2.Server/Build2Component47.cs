// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component47
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
  internal class Build2Component47 : Build2Component46
  {
    public override BuildDefinition GetRenamedDefinition(Guid projectId, string name, string path)
    {
      this.TraceEnter(0, nameof (GetRenamedDefinition));
      this.PrepareStoredProcedure("Build.prc_GetRenamedDefinition");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@oldName", DBHelper.ServerPathToDBPath(name), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@oldPath", DBHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        BuildDefinition renamedDefinition = resultCollection.GetCurrent<BuildDefinition>().SingleOrDefault<BuildDefinition>();
        this.TraceLeave(0, nameof (GetRenamedDefinition));
        return renamedDefinition;
      }
    }

    public override async Task<List<BuildDefinition>> GetDeletedDefinitionsAsync(
      Guid projectId,
      DefinitionQueryOrder queryOrder,
      int maxDefinitions)
    {
      Build2Component47 build2Component47 = this;
      build2Component47.TraceEnter(0, nameof (GetDeletedDefinitionsAsync));
      build2Component47.PrepareStoredProcedure("Build.prc_GetDeletedDefinitions");
      build2Component47.BindInt("@dataspaceId", build2Component47.GetDataspaceId(projectId));
      build2Component47.BindInt("@queryOrder", (int) queryOrder);
      build2Component47.BindInt("@maxDefinitions", maxDefinitions);
      List<BuildDefinition> definitionsAsync;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component47.ExecuteReaderAsync(), build2Component47.ProcedureName, build2Component47.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(build2Component47.GetBuildDefinitionBinder());
        List<BuildDefinition> items = resultCollection.GetCurrent<BuildDefinition>().Items;
        build2Component47.TraceLeave(0, nameof (GetDeletedDefinitionsAsync));
        definitionsAsync = items;
      }
      return definitionsAsync;
    }
  }
}
