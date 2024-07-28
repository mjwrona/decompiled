// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component48
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
  internal class Build2Component48 : Build2Component47
  {
    public override async Task<List<BuildDefinition>> GetDefinitionsWithSchedulesAsync(
      Guid projectId)
    {
      Build2Component48 build2Component48 = this;
      build2Component48.TraceEnter(0, nameof (GetDefinitionsWithSchedulesAsync));
      build2Component48.PrepareStoredProcedure("Build.prc_GetDefinitionsWithSchedules");
      build2Component48.BindInt("@dataspaceId", build2Component48.GetDataspaceId(projectId));
      List<BuildDefinition> withSchedulesAsync;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component48.ExecuteReaderAsync(), build2Component48.ProcedureName, build2Component48.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(build2Component48.GetBuildDefinitionBinder());
        List<BuildDefinition> items = resultCollection.GetCurrent<BuildDefinition>().Items;
        build2Component48.TraceLeave(0, nameof (GetDefinitionsWithSchedulesAsync));
        withSchedulesAsync = items;
      }
      return withSchedulesAsync;
    }
  }
}
