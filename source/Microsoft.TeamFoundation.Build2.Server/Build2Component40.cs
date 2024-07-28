// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component40
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component40 : Build2Component39
  {
    public override void DeleteDefinitions(
      Guid projectId,
      IEnumerable<int> definitionIds,
      Guid requestedBy)
    {
      this.DeleteDefinitions(projectId, definitionIds, requestedBy, out List<BuildData> _);
    }

    public override void DeleteDefinitions(
      Guid projectId,
      IEnumerable<int> definitionIds,
      Guid requestedBy,
      out List<BuildData> buildsToCancel)
    {
      this.TraceEnter(0, nameof (DeleteDefinitions));
      this.PrepareStoredProcedure("Build.prc_DeleteDefinitions");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt32Table("@definitionIdTable", definitionIds);
      this.BindGuid("@requestedBy", requestedBy);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        buildsToCancel = resultCollection.GetCurrent<BuildData>().Items;
        this.TraceLeave(0, nameof (DeleteDefinitions));
      }
    }
  }
}
