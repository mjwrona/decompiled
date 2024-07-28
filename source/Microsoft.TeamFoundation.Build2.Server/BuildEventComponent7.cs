// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildEventComponent7
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildEventComponent7 : BuildEventComponent6
  {
    public override BuildEventResults GetBuildEvents(BuildEventStatus status, int? maxBuildEvents)
    {
      this.TraceEnter(0, nameof (GetBuildEvents));
      this.PrepareStoredProcedure("Build.prc_GetBuildEvents");
      this.BindByte("@status", (byte) status);
      this.BindNullableInt("@maxBuildEvents", maxBuildEvents);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildChangeEvent>(this.GetBuildEventBinder());
        resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
        List<BuildChangeEvent> items = resultCollection.GetCurrent<BuildChangeEvent>().Items;
        resultCollection.NextResult();
        int num = resultCollection.GetCurrent<int>().FirstOrDefault<int>();
        this.TraceLeave(0, nameof (GetBuildEvents));
        return new BuildEventResults()
        {
          BuildEvents = items,
          QueueDepth = num
        };
      }
    }
  }
}
