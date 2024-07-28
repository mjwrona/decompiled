// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent10
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent10 : BuildComponent9
  {
    public BuildComponent10()
    {
      this.ServiceVersion = ServiceVersion.V10;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public override List<BuildServiceHost> GetDisconnectedBuildServiceHosts(
      TimeSpan disconnectTimeout)
    {
      this.TraceEnter(0, "GetOfflineBuildServiceHosts");
      this.PrepareStoredProcedure("prc_GetDisconnectedBuildServiceHosts");
      this.BindInt("@disconnectTimeout", (int) disconnectTimeout.TotalSeconds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
        this.TraceLeave(0, "GetOfflineBuildServiceHosts");
        return resultCollection.GetCurrent<BuildServiceHost>().Items;
      }
    }
  }
}
