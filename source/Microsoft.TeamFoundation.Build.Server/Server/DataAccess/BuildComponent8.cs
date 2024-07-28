// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent8
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent8 : BuildComponent7
  {
    public BuildComponent8()
    {
      this.ServiceVersion = ServiceVersion.V8;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override List<string> DestroyDeletedBuilds()
    {
      this.TraceEnter(0, nameof (DestroyDeletedBuilds));
      this.PrepareStoredProcedure("prc_DestroyDeletedBuilds");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new StringBinder());
        this.TraceLeave(0, nameof (DestroyDeletedBuilds));
        return resultCollection.GetCurrent<string>().Items;
      }
    }
  }
}
