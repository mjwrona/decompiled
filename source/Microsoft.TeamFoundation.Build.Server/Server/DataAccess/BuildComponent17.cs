// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent17
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent17 : BuildComponent15
  {
    public BuildComponent17()
    {
      this.ServiceVersion = ServiceVersion.V17;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override List<ProcessTemplate> QueryProcessTemplatesByPath(
      IVssRequestContext requestContext,
      string teamProject,
      ICollection<string> paths,
      bool includeDeleted,
      bool recursive)
    {
      this.TraceEnter(0, nameof (QueryProcessTemplatesByPath));
      this.PrepareStoredProcedure("prc_QueryBuildProcessTemplatesByPath");
      this.BindStringTable("@pathsTable", (IEnumerable<string>) paths);
      this.BindTeamProjectDataspace(requestContext, "@dataspaceId", teamProject, true);
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindBoolean("@recursive", recursive);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
        this.TraceLeave(0, nameof (QueryProcessTemplatesByPath));
        return resultCollection.GetCurrent<ProcessTemplate>().Items;
      }
    }
  }
}
