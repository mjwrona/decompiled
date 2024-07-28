// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component92
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build2.Server.DataAccess;
using Microsoft.TeamFoundation.Build2.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component92 : Build2Component91
  {
    public override IList<PoisonedBuild> GetPoisonedBuilds(int batchSize)
    {
      this.TraceEnter(0, nameof (GetPoisonedBuilds));
      this.PrepareStoredProcedure("Build.prc_GetPoisonedBuilds");
      this.BindInt("@batchSize", batchSize);
      List<PoisonedBuild> poisonedBuilds = new List<PoisonedBuild>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PoisonedBuild>(this.GetPoisonedBuildBinder());
        poisonedBuilds = resultCollection.GetCurrent<PoisonedBuild>().Items;
      }
      this.TraceLeave(0, nameof (GetPoisonedBuilds));
      return (IList<PoisonedBuild>) poisonedBuilds;
    }

    protected virtual ObjectBinder<PoisonedBuild> GetPoisonedBuildBinder() => (ObjectBinder<PoisonedBuild>) new PoisonedBuildBinder();
  }
}
