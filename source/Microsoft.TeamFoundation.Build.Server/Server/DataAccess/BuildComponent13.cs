// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent13
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent13 : BuildComponent12
  {
    public BuildComponent13()
    {
      this.ServiceVersion = ServiceVersion.V13;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override List<BuildDefinition> QueryDefinitionsWithNewBuilds(DateTime minFinishTime)
    {
      this.TraceEnter(0, nameof (QueryDefinitionsWithNewBuilds));
      this.PrepareStoredProcedure("prc_QueryDefinitionsWithNewBuilds");
      this.BindNullableDateTime("@minFinishTime", minFinishTime);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      List<BuildDefinition> buildDefinitionList = new List<BuildDefinition>();
      while (sqlDataReader.Read())
        new BuildDefinition().Uri = DBHelper.CreateArtifactUri("Definition", sqlDataReader.GetInt32(0));
      this.TraceLeave(0, nameof (QueryDefinitionsWithNewBuilds));
      return buildDefinitionList;
    }
  }
}
