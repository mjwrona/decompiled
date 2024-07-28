// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component54
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component54 : Build2Component53
  {
    public override BuildDefinition RestoreDefinition(
      Guid projectId,
      int definitionId,
      Guid authorId,
      string comment = null)
    {
      using (this.TraceScope(method: nameof (RestoreDefinition)))
      {
        this.PrepareStoredProcedure("Build.prc_RestoreDefinition");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@definitionId", definitionId);
        this.BindGuid("@requestedBy", authorId);
        this.BindString("@comment", comment, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
          return resultCollection.GetCurrent<BuildDefinition>().SingleOrDefault<BuildDefinition>();
        }
      }
    }
  }
}
