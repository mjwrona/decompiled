// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent2 : BuildComponent
  {
    public BuildComponent2() => this.ServiceVersion = ServiceVersion.V2;

    internal override ResultCollection QueryBuilds(
      IVssRequestContext requestContext,
      BuildDetailSpec spec,
      QueryOptions options)
    {
      this.TraceEnter(0, nameof (QueryBuilds));
      this.PrepareStoredProcedure("prc_QueryBuilds");
      if (spec.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
      {
        this.BindTeamProject(requestContext, "@teamProject", BuildCommonUtil.IsStar(spec.TeamProject) ? (string) null : spec.TeamProject, true);
        this.BindItemPath("@definitionPath", spec.Path, false);
      }
      else
        this.BindTable<int>("@definitionIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToDistinctInt32Table((IEnumerable<string>) spec.DefinitionFilter));
      this.BindItemPath("@buildNumber", spec.BuildNumber, false);
      this.BindUtcDateTime("@minFinishTime", spec.MinFinishTime);
      this.BindUtcDateTime("@maxFinishTime", spec.MaxFinishTime);
      this.BindUtcDateTime("@minChangedTime", spec.MinChangedTime);
      this.BindNullableInt("@reasonFilter", (int) spec.Reason, 511);
      this.BindNullableInt("@statusFilter", (int) spec.Status, 63);
      this.BindString("@qualityFilter", BuildQuality.TryConvertBuildQualityToResId(spec.Quality), 256, true, SqlDbType.NVarChar);
      this.BindInt("@queryOptions", (int) options);
      this.BindInt("@queryOrder", (int) spec.QueryOrder);
      this.BindIdentityFilter("@requestedFor", spec.RequestedFor, spec.RequestedForIdentity);
      this.BindInt("@maxBuildsPerDefinition", spec.MaxBuildsPerDefinition);
      this.BindInt("@queryDeletedOption", (int) spec.QueryDeletedOption);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuilds));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildsByUri(
      IList<string> uris,
      QueryOptions options,
      QueryDeletedOption queryDeletedOption)
    {
      this.TraceEnter(0, nameof (QueryBuildsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildsByUri");
      this.BindTable<KeyValuePair<int, string>>("@buildUriTable", (TeamFoundationTableValueParameter<KeyValuePair<int, string>>) BuildSqlResourceComponent.UrisToOrderedStringTable((IEnumerable<string>) uris));
      this.BindInt("@queryOptions", (int) options);
      this.BindInt("@queryDeletedOption", (int) queryDeletedOption);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildsByUri));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildDefinitions(
      IVssRequestContext requestContext,
      string teamProject,
      string definitionPath,
      DefinitionTriggerType continuousIntegrationType,
      QueryOptions options,
      bool strict)
    {
      this.TraceEnter(0, nameof (QueryBuildDefinitions));
      this.PrepareStoredProcedure("prc_QueryBuildDefinitions");
      this.BindTeamProject(requestContext, "@teamProject", teamProject, true);
      this.BindItemPath("@definitionPath", definitionPath, false);
      this.BindByte("@continuousIntegrationType", (byte) continuousIntegrationType);
      this.BindByte("@queryOptions", (byte) options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildDefinitions));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildDefinitionsByUri(
      IEnumerable<string> uris,
      QueryOptions options)
    {
      this.TraceEnter(0, nameof (QueryBuildDefinitionsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildDefinitionsByUri");
      this.BindTable<int>("@definitionIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table(uris));
      this.BindByte("@queryOptions", (byte) options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildDefinitionsByUri));
      return resultCollection;
    }

    internal override ResultCollection ProcessChangeset(
      int changesetId,
      string changesetVersion,
      TeamFoundationIdentity changesetOwner,
      TeamFoundationIdentity buildOwnerName,
      ICollection<BuildDefinition> affectedDefinitions)
    {
      this.TraceEnter(0, nameof (ProcessChangeset));
      this.PrepareStoredProcedure("prc_ProcessChangeset");
      this.BindInt("@changesetId", changesetId);
      this.BindString("@changesetVersion", changesetVersion, 325, false, SqlDbType.NVarChar);
      this.BindIdentity("@changesetOwner", changesetOwner);
      this.BindIdentity("@buildOwner", buildOwnerName);
      this.BindTable<BuildDefinition>("@affectedDefinitionTable", (TeamFoundationTableValueParameter<BuildDefinition>) new AffectedBuildDefinitionTable(affectedDefinitions));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (ProcessChangeset));
      return resultCollection;
    }
  }
}
