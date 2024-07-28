// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildQueueComponent6
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildQueueComponent6 : BuildQueueComponent5
  {
    public BuildQueueComponent6()
    {
      this.ServiceVersion = ServiceVersion.V6;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection UpdateBuilds(
      IList<QueuedBuildUpdateOptions> updates,
      bool resetQueueTime)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("prc_UpdateQueuedBuilds");
      this.BindTable<QueuedBuildUpdateOptions>("@buildUpdateTable", (TeamFoundationTableValueParameter<QueuedBuildUpdateOptions>) new QueuedBuildUpdateTable((ICollection<QueuedBuildUpdateOptions>) updates));
      this.BindBoolean("@resetQueueTime", resetQueueTime);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuilds));
      return resultCollection;
    }
  }
}
