// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component84
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component84 : Build2Component83
  {
    public override IEnumerable<BuildDefinitionBranch> UpdateDefinitionBranches(
      Guid projectId,
      int definitionId,
      int definitionVersion,
      IEnumerable<BuildDefinitionBranch> branches,
      int maxConcurrentBuildsPerBranch,
      bool ignoreSourceIdCheck)
    {
      using (this.TraceScope(method: nameof (UpdateDefinitionBranches)))
      {
        this.PrepareStoredProcedure("Build.prc_UpdateDefinitionBranches");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@definitionId", definitionId);
        this.BindInt("@definitionVersion", definitionVersion);
        this.BindBuildDefinitionBranchTable3("@branches", branches);
        this.BindInt("@maxConcurrentBuildsPerBranch", maxConcurrentBuildsPerBranch);
        this.BindBoolean("@ignoreSourceIdCheck", ignoreSourceIdCheck);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildDefinitionBranch>(this.GetBuildDefinitionBranchBinder());
          return (IEnumerable<BuildDefinitionBranch>) resultCollection.GetCurrent<BuildDefinitionBranch>().Items;
        }
      }
    }

    public override void DeleteCheckEvents(
      CheckEventStatus status,
      DateTime minCreatedTime,
      int? batchSize)
    {
      using (this.TraceScope(method: nameof (DeleteCheckEvents)))
      {
        this.PrepareStoredProcedure("Build.prc_DeleteCheckEvents");
        this.BindByte("@status", (byte) status);
        this.BindDateTime2("@minCreatedTime", minCreatedTime);
        this.BindNullableInt("@batchSize", batchSize);
        this.ExecuteNonQuery();
      }
    }
  }
}
