// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component94
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component94 : Build2Component93
  {
    public override IEnumerable<BuildTagFilter> GetFilterBuildTags(
      Guid projectId,
      int? definitionId)
    {
      this.TraceEnter(0, nameof (GetFilterBuildTags));
      this.PrepareStoredProcedure("Build.prc_GetFilterBuildTags");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindNullableInt("@definitionId", definitionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildTagFilter>(this.GetBuildTagFilterBinder());
        List<BuildTagFilter> items = resultCollection.GetCurrent<BuildTagFilter>().Items;
        this.TraceLeave(0, nameof (GetFilterBuildTags));
        return (IEnumerable<BuildTagFilter>) items;
      }
    }

    public override int UpdatePermissionsToEditQueueVars(int batchSize)
    {
      this.PrepareStoredProcedure("Build.prc_SetPermissionsToEditQueueVariables");
      this.BindInt("@batchSize", batchSize);
      int editQueueVars;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
        editQueueVars = resultCollection.GetCurrent<int>().Items[0];
      }
      this.TraceLeave(0, nameof (UpdatePermissionsToEditQueueVars));
      return editQueueVars;
    }
  }
}
