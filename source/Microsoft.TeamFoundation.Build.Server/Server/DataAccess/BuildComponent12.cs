// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent12
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent12 : BuildComponent11
  {
    public BuildComponent12()
    {
      this.ServiceVersion = ServiceVersion.V12;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection QueryChangedBuilds(
      IVssRequestContext requestContext,
      ICollection<string> teamProjects,
      DateTime minChangedTime,
      BuildStatus statusFilter,
      int batchSize = 2147483647)
    {
      this.TraceEnter(0, nameof (QueryChangedBuilds));
      this.PrepareStoredProcedure("prc_QueryChangedBuilds");
      this.BindTable<string>("@teamProjectTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) teamProjects));
      this.BindUtcDateTime("@minChangedTime", minChangedTime);
      this.BindNullableInt("@statusFilter", (int) statusFilter, 63);
      this.BindInt("@batchSize", batchSize);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryChangedBuilds));
      return resultCollection;
    }
  }
}
