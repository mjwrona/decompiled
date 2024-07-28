// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.CompatibilityComponent4
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal class CompatibilityComponent4 : CompatibilityComponent3
  {
    public CompatibilityComponent4()
    {
      this.ServiceVersion = ServiceVersion.V14;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection QueryBuilds(
      BuildDetailSpec2010 spec,
      QueryOptions2010 options)
    {
      this.TraceEnter(0, nameof (QueryBuilds));
      this.PrepareStoredProcedure("prc_QueryBuilds2010");
      if (spec.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
      {
        this.BindTeamProjectDataspace(this.RequestContext, "@dataspaceId", spec.TeamProject, true);
        this.BindItemPath("@definitionPath", spec.GroupPath, false);
      }
      else
        this.BindUrisToInt32Table("@definitionUriTable", (IEnumerable<string>) spec.DefinitionFilter);
      this.BindItemPath("@buildNumber", spec.BuildNumber, false);
      this.BindUtcDateTime("@minFinishTime", spec.MinFinishTime);
      this.BindUtcDateTime("@maxFinishTime", spec.MaxFinishTime);
      this.BindUtcDateTime("@minChangedTime", spec.MinChangedTime);
      this.BindNullableInt("@reasonFilter", (int) spec.Reason, (int) byte.MaxValue);
      this.BindNullableInt("@statusFilter", (int) spec.Status, 63);
      this.BindString("@qualityFilter", BuildQuality.TryConvertBuildQualityToResId(spec.Quality), 256, true, SqlDbType.NVarChar);
      this.BindInt("@queryOptions", (int) options);
      this.BindInt("@queryOrder", (int) spec.QueryOrder);
      this.BindIdentityFilter("@requestedFor", spec.RequestedFor, spec.RequestedForIdentity);
      this.BindInt("@maxBuildsPerDefinition", spec.MaxBuildsPerDefinition);
      this.BindInt("@queryDeletedOption", (int) spec.QueryDeletedOption);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition2010>((ObjectBinder<BuildDefinition2010>) new BuildDefinition2010Binder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy2010>((ObjectBinder<RetentionPolicy2010>) new RetentionPolicy2010Binder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule2010>((ObjectBinder<Schedule2010>) new Schedule2010Binder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate2010>((ObjectBinder<ProcessTemplate2010>) new ProcessTemplate2010Binder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate2010>((ObjectBinder<WorkspaceTemplate2010>) new WorkspaceTemplate2010Binder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping2010>((ObjectBinder<WorkspaceMapping2010>) new WorkspaceMapping2010Binder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail2010>((ObjectBinder<BuildDetail2010>) new BuildDetail2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController2010>((ObjectBinder<BuildController2010>) new BuildController2010Binder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent2010>((ObjectBinder<BuildAgent2010>) new BuildAgent2010Binder());
      resultCollection.AddBinder<BuildServiceHost2010>((ObjectBinder<BuildServiceHost2010>) new BuildServiceHost2010Binder());
      this.TraceLeave(0, nameof (QueryBuilds));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildsByUri(
      IList<string> uris,
      QueryOptions2010 options,
      QueryDeletedOption2010 queryDeletedOption)
    {
      this.TraceEnter(0, nameof (QueryBuildsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildsByUri2010");
      this.BindQueryBuildsByUriTable("@buildUriTable", (IEnumerable<string>) uris);
      this.BindInt("@queryOptions", (int) options);
      this.BindInt("@queryDeletedOption", (int) queryDeletedOption);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition2010>((ObjectBinder<BuildDefinition2010>) new BuildDefinition2010Binder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy2010>((ObjectBinder<RetentionPolicy2010>) new RetentionPolicy2010Binder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule2010>((ObjectBinder<Schedule2010>) new Schedule2010Binder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate2010>((ObjectBinder<ProcessTemplate2010>) new ProcessTemplate2010Binder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate2010>((ObjectBinder<WorkspaceTemplate2010>) new WorkspaceTemplate2010Binder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping2010>((ObjectBinder<WorkspaceMapping2010>) new WorkspaceMapping2010Binder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail2010>((ObjectBinder<BuildDetail2010>) new BuildDetail2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController2010>((ObjectBinder<BuildController2010>) new BuildController2010Binder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent2010>((ObjectBinder<BuildAgent2010>) new BuildAgent2010Binder());
      resultCollection.AddBinder<BuildServiceHost2010>((ObjectBinder<BuildServiceHost2010>) new BuildServiceHost2010Binder());
      this.TraceLeave(0, nameof (QueryBuildsByUri));
      return resultCollection;
    }
  }
}
