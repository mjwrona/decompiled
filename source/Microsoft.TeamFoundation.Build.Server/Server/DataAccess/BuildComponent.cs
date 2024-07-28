// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent : BuildSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[17]
    {
      (IComponentCreator) new ComponentCreator<BuildComponent>(1),
      (IComponentCreator) new ComponentCreator<BuildComponent2>(2),
      (IComponentCreator) new ComponentCreator<BuildComponent3>(3),
      (IComponentCreator) new ComponentCreator<BuildComponent4>(4),
      (IComponentCreator) new ComponentCreator<BuildComponent5>(5),
      (IComponentCreator) new ComponentCreator<BuildComponent6>(6),
      (IComponentCreator) new ComponentCreator<BuildComponent7>(7),
      (IComponentCreator) new ComponentCreator<BuildComponent8>(8),
      (IComponentCreator) new ComponentCreator<BuildComponent9>(9),
      (IComponentCreator) new ComponentCreator<BuildComponent10>(10),
      (IComponentCreator) new ComponentCreator<BuildComponent11>(11),
      (IComponentCreator) new ComponentCreator<BuildComponent12>(12),
      (IComponentCreator) new ComponentCreator<BuildComponent13>(13),
      (IComponentCreator) new ComponentCreator<BuildComponent14>(14),
      (IComponentCreator) new ComponentCreator<BuildComponent15>(15),
      (IComponentCreator) new ComponentCreator<BuildComponent15>(16),
      (IComponentCreator) new ComponentCreator<BuildComponent17>(17)
    }, "Build", "Build");

    protected override string TraceArea => "Build";

    internal virtual BuildDetail AddBuild(
      string buildControllerUri,
      string buildDefinitionUri,
      Guid projectId,
      string buildNumber,
      string dropLocation,
      DateTime startTime,
      string sourceGetVersion,
      string buildQuality,
      Microsoft.TeamFoundation.Build.Server.BuildStatus buildStatus,
      TeamFoundationIdentity requestedBy,
      TeamFoundationIdentity requestedFor)
    {
      this.TraceEnter(0, nameof (AddBuild));
      this.PrepareStoredProcedure("prc_AddBuild");
      this.BindItemUriToInt32("@controllerId", buildControllerUri);
      this.BindItemUriToInt32("@definitionId", buildDefinitionUri);
      this.BindItemPath("@buildNumber", buildNumber, true);
      this.BindUncPath("@dropLocation", dropLocation, true);
      this.BindUtcDateTime("@startTime", startTime);
      this.BindString("@sourceGetVersion", sourceGetVersion, 325, false, SqlDbType.NVarChar);
      this.BindString("@buildQuality", buildQuality, 256, true, SqlDbType.NVarChar);
      this.BindByte("@buildStatus", (byte) buildStatus);
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindIdentity("@requestedFor", requestedFor);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
        ObjectBinder<BuildDetail> current = resultCollection.GetCurrent<BuildDetail>();
        current.MoveNext();
        this.TraceLeave(0, nameof (AddBuild));
        return current.Current;
      }
    }

    internal virtual ResultCollection AddBuildDefinitions(
      ICollection<BuildDefinition> definitions,
      TeamFoundationIdentity requestedBy,
      Guid writerId,
      bool allowMissingController)
    {
      this.TraceEnter(0, nameof (AddBuildDefinitions));
      this.PrepareStoredProcedure("prc_AddBuildDefinitions");
      int num = 0;
      List<Schedule> rows1 = new List<Schedule>();
      List<RetentionPolicy> rows2 = new List<RetentionPolicy>();
      List<WorkspaceMapping> rows3 = new List<WorkspaceMapping>();
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>();
      foreach (BuildDefinition definition1 in (IEnumerable<BuildDefinition>) definitions)
      {
        BuildDefinition definition = definition1;
        definition.Uri = DBHelper.CreateArtifactUri("Definition", ++num);
        this.Trace(0, TraceLevel.Info, "Definition uri '{0}' created", (object) definition.Uri);
        definition.Schedules.ForEach((Action<Schedule>) (x => x.DefinitionUri = definition.Uri));
        definition.RetentionPolicies.ForEach((Action<RetentionPolicy>) (x => x.DefinitionUri = definition.Uri));
        definition.WorkspaceTemplate.Mappings.ForEach((Action<WorkspaceMapping>) (x => x.DefinitionUri = definition.Uri));
        rows1.AddRange((IEnumerable<Schedule>) definition.Schedules);
        rows2.AddRange((IEnumerable<RetentionPolicy>) definition.RetentionPolicies);
        rows3.AddRange((IEnumerable<WorkspaceMapping>) definition.WorkspaceTemplate.Mappings);
        properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateEmptyArtifactSpec(BuildPropertyKinds.BuildDefinition, definition.Id), (IEnumerable<PropertyValue>) definition.Properties));
      }
      this.BindTable<BuildDefinition>("@buildDefinitionTable", (TeamFoundationTableValueParameter<BuildDefinition>) new BuildDefinitionTable(definitions));
      this.BindTable<RetentionPolicy>("@retentionPolicyTable", (TeamFoundationTableValueParameter<RetentionPolicy>) new BuildDefinitionRetentionPolicyTable((ICollection<RetentionPolicy>) rows2));
      this.BindTable<Schedule>("@scheduleTable", (TeamFoundationTableValueParameter<Schedule>) new BuildDefinitionScheduleTable((ICollection<Schedule>) rows1));
      this.BindTable<WorkspaceMapping>("@workspaceTable", (TeamFoundationTableValueParameter<WorkspaceMapping>) new BuildDefinitionWorkspaceTable((ICollection<WorkspaceMapping>) rows3));
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildDefinition);
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindGuid("@writerId", writerId);
      this.BindBoolean("@allowMissingController", allowMissingController);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      this.TraceLeave(0, nameof (AddBuildDefinitions));
      return resultCollection;
    }

    internal virtual void AddBuildQualities(TeamProject teamProject, IList<string> qualities)
    {
      this.TraceEnter(0, nameof (AddBuildQualities));
      this.PrepareStoredProcedure("prc_AddBuildQualities");
      this.BindUri("@teamProject", teamProject.Uri, false);
      this.BindTable<string>("@qualityTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) qualities.Select<string, string>((System.Func<string, string>) (x => BuildQuality.TryConvertBuildQualityToResId(x))).ToArray<string>(), false, 256));
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (AddBuildQualities));
    }

    internal virtual void DeleteBuildQualities(TeamProject teamProject, IList<string> qualities)
    {
      this.TraceEnter(0, nameof (DeleteBuildQualities));
      this.PrepareStoredProcedure("prc_DeleteBuildQualities");
      this.BindUri("@teamProject", teamProject.Uri, false);
      this.BindTable<string>("@qualityTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) qualities.Select<string, string>((System.Func<string, string>) (x => BuildQuality.TryConvertBuildQualityToResId(x))).ToArray<string>(), false, 256));
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteBuildQualities));
    }

    internal virtual ResultCollection GetBuildQualities(TeamProject teamProject)
    {
      this.TraceEnter(0, nameof (GetBuildQualities));
      this.PrepareStoredProcedure("prc_GetBuildQualities");
      if (teamProject != null)
        this.BindUri("@teamProject", teamProject.Uri, false);
      else
        this.BindNullValue("@teamProject", SqlDbType.VarChar);
      ResultCollection buildQualities = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      buildQualities.AddBinder<string>((ObjectBinder<string>) new BuildQualityBinder());
      this.TraceLeave(0, nameof (GetBuildQualities));
      return buildQualities;
    }

    internal virtual ResultCollection AddProcessTemplates(
      ICollection<ProcessTemplate> processTemplates)
    {
      this.TraceEnter(0, nameof (AddProcessTemplates));
      this.PrepareStoredProcedure("prc_AddBuildProcessTemplates");
      this.BindTable<ProcessTemplate>("@processTemplateTable", (TeamFoundationTableValueParameter<ProcessTemplate>) new BuildProcessTemplateTable(processTemplates, true));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (AddProcessTemplates));
      return resultCollection;
    }

    internal virtual void DeleteBuildDefinitions(
      IEnumerable<BuildDefinition> definitions,
      Guid writerId,
      ArtifactKind definitionArtifactKind)
    {
      this.TraceEnter(0, nameof (DeleteBuildDefinitions));
      this.PrepareStoredProcedure("prc_DeleteBuildDefinitions", 3600);
      this.BindTable<int>("@definitionIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table(definitions.Select<BuildDefinition, string>((System.Func<BuildDefinition, string>) (x => x.Uri))));
      this.BindGuid("@writerId", writerId);
      this.BindInt("@compactKindId", definitionArtifactKind.CompactKindId);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteBuildDefinitions));
    }

    internal virtual ResultCollection DeleteBuilds(
      IEnumerable<BuildDetail> builds,
      DeleteOptions options,
      TeamFoundationIdentity deletedBy)
    {
      this.TraceEnter(0, nameof (DeleteBuilds));
      this.PrepareStoredProcedure("prc_DeleteBuilds", 3600);
      this.BindTable<string>("@buildUriTable", (TeamFoundationTableValueParameter<string>) BuildSqlResourceComponent.UrisToStringTable(builds.Select<BuildDetail, string>((System.Func<BuildDetail, string>) (x => x.Uri))));
      this.BindInt("@options", (int) options);
      this.BindIdentity("@deletedBy", deletedBy);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<KeyValuePair<DeleteOptions, string>>((ObjectBinder<KeyValuePair<DeleteOptions, string>>) new BuildDeletionOptionsBinder());
      resultCollection.AddBinder<SymbolStoreData>((ObjectBinder<SymbolStoreData>) new SymbolStoreDataBinder());
      this.TraceLeave(0, nameof (DeleteBuilds));
      return resultCollection;
    }

    internal virtual void DestroyBuilds(IEnumerable<BuildDetail> builds)
    {
      this.TraceEnter(0, nameof (DestroyBuilds));
      this.PrepareStoredProcedure("prc_DestroyBuilds", 3600);
      this.BindTable<string>("@buildUriTable", (TeamFoundationTableValueParameter<string>) BuildSqlResourceComponent.UrisToStringTable(builds.Select<BuildDetail, string>((System.Func<BuildDetail, string>) (x => x.Uri))));
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DestroyBuilds));
    }

    internal virtual List<string> DestroyDeletedBuilds() => new List<string>();

    internal virtual IEnumerable<string> DeleteTeamProject(
      string projectUri,
      Guid writerId,
      ArtifactKind definitionArtifactKind)
    {
      this.TraceEnter(0, nameof (DeleteTeamProject));
      this.PrepareStoredProcedure("prc_DeleteTeamProject");
      this.BindUri("@teamProject", projectUri, false);
      this.BindGuid("@writerId", writerId);
      this.BindInt("@compactKindId", definitionArtifactKind.CompactKindId);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder binder = new SqlColumnBinder("DefinitionId");
      List<string> stringList = new List<string>();
      while (reader.Read())
        stringList.Add(binder.GetArtifactUriFromInt32(reader, "Definition", false));
      this.TraceLeave(0, nameof (DeleteTeamProject));
      return (IEnumerable<string>) stringList;
    }

    internal virtual void DeleteProcessTemplates(ICollection<ProcessTemplate> processTemplates)
    {
      this.TraceEnter(0, nameof (DeleteProcessTemplates));
      this.PrepareStoredProcedure("prc_DeleteBuildProcessTemplates");
      this.BindTable<int>("@processTemplateIdTable", (TeamFoundationTableValueParameter<int>) new Int32Table(processTemplates.Select<ProcessTemplate, int>((System.Func<ProcessTemplate, int>) (x => x.Id))));
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteProcessTemplates));
    }

    internal virtual ResultCollection QueryBuilds(
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
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuilds));
      return resultCollection;
    }

    internal virtual ResultCollection QueryChangedBuilds(
      IVssRequestContext requestContext,
      ICollection<string> teamProjects,
      DateTime minChangedTime,
      Microsoft.TeamFoundation.Build.Server.BuildStatus statusFilter,
      int batchSize = 2147483647)
    {
      this.TraceEnter(0, nameof (QueryChangedBuilds));
      this.PrepareStoredProcedure("prc_QueryChangedBuilds");
      this.BindTable<string>("@teamProjectTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) teamProjects));
      this.BindUtcDateTime("@minChangedTime", minChangedTime);
      this.BindNullableInt("@statusFilter", (int) statusFilter, 63);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryChangedBuilds));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildsByUri(
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
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildsByUri));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildDefinitions(
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
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildDefinitions));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildDefinitionsByUri(
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
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildDefinitionsByUri));
      return resultCollection;
    }

    internal virtual ResultCollection QueryProcessTemplates(
      IVssRequestContext requestContext,
      string teamProject,
      IEnumerable<ProcessTemplateType> templateTypes,
      bool includeDeleted)
    {
      this.TraceEnter(0, nameof (QueryProcessTemplates));
      this.PrepareStoredProcedure("prc_QueryBuildProcessTemplates");
      this.BindTable<int>("@templateTypeTable", (TeamFoundationTableValueParameter<int>) new Int32Table((IEnumerable<int>) templateTypes.Cast<int>().ToArray<int>()));
      this.BindTeamProject(requestContext, "@teamProject", BuildCommonUtil.IsStar(teamProject) ? (string) null : teamProject, true);
      this.BindBoolean("@includeDeleted", includeDeleted);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryProcessTemplates));
      return resultCollection;
    }

    internal virtual ResultCollection QueryProcessTemplatesById(ICollection<int> templateIds)
    {
      this.TraceEnter(0, nameof (QueryProcessTemplatesById));
      this.PrepareStoredProcedure("prc_QueryBuildProcessTemplatesById");
      this.BindTable<int>("@processTemplateIdTable", (TeamFoundationTableValueParameter<int>) new Int32Table((IEnumerable<int>) templateIds));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryProcessTemplatesById));
      return resultCollection;
    }

    internal virtual List<ProcessTemplate> QueryProcessTemplatesByPath(
      IVssRequestContext requestContext,
      string teamProject,
      ICollection<string> paths,
      bool includeDeleted,
      bool recursive)
    {
      return new List<ProcessTemplate>();
    }

    internal virtual ResultCollection UpdateBuildDefinitions(
      IList<BuildDefinition> definitions,
      TeamFoundationIdentity requestedBy,
      Guid writerId,
      bool isServicingRequest)
    {
      this.TraceEnter(0, nameof (UpdateBuildDefinitions));
      this.PrepareStoredProcedure("prc_UpdateBuildDefinitions");
      List<Schedule> rows1 = new List<Schedule>();
      List<RetentionPolicy> rows2 = new List<RetentionPolicy>();
      List<WorkspaceMapping> rows3 = new List<WorkspaceMapping>();
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>(definitions.Count);
      foreach (BuildDefinition definition1 in (IEnumerable<BuildDefinition>) definitions)
      {
        BuildDefinition definition = definition1;
        definition.Schedules.ForEach((Action<Schedule>) (x => x.DefinitionUri = definition.Uri));
        definition.RetentionPolicies.ForEach((Action<RetentionPolicy>) (x => x.DefinitionUri = definition.Uri));
        rows1.AddRange((IEnumerable<Schedule>) definition.Schedules);
        rows2.AddRange((IEnumerable<RetentionPolicy>) definition.RetentionPolicies);
        if (definition.WorkspaceTemplate != null)
        {
          definition.WorkspaceTemplate.Mappings.ForEach((Action<WorkspaceMapping>) (x => x.DefinitionUri = definition.Uri));
          rows3.AddRange((IEnumerable<WorkspaceMapping>) definition.WorkspaceTemplate.Mappings);
        }
        properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateArtifactSpec(definition.Uri), (IEnumerable<PropertyValue>) definition.Properties));
      }
      this.BindTable<BuildDefinition>("@buildDefinitionTable", (TeamFoundationTableValueParameter<BuildDefinition>) new BuildDefinitionTable((ICollection<BuildDefinition>) definitions));
      this.BindTable<RetentionPolicy>("@retentionPolicyTable", (TeamFoundationTableValueParameter<RetentionPolicy>) new BuildDefinitionRetentionPolicyTable((ICollection<RetentionPolicy>) rows2));
      this.BindTable<Schedule>("@scheduleTable", (TeamFoundationTableValueParameter<Schedule>) new BuildDefinitionScheduleTable((ICollection<Schedule>) rows1));
      this.BindTable<WorkspaceMapping>("@workspaceTable", (TeamFoundationTableValueParameter<WorkspaceMapping>) new BuildDefinitionWorkspaceTable((ICollection<WorkspaceMapping>) rows3));
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildDefinition);
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindGuid("@writerId", writerId);
      this.BindBoolean("@isServicingRequest", isServicingRequest);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildDefinitions));
      return resultCollection;
    }

    internal virtual ResultCollection UpdateBuilds(
      ICollection<BuildUpdateOptions> options,
      TeamFoundationIdentity requestedBy,
      Guid writerId)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("prc_UpdateBuilds");
      this.BindTable<BuildUpdateOptions>("@buildUpdateTable", (TeamFoundationTableValueParameter<BuildUpdateOptions>) new BuildUpdateTable(options));
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindGuid("@writerId", writerId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildUpdateOptions>((ObjectBinder<BuildUpdateOptions>) new BuildUpdateOptionsBinder());
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      resultCollection.AddBinder<SharedResourceData>((ObjectBinder<SharedResourceData>) new SharedResourceDataBinder());
      this.TraceLeave(0, nameof (UpdateBuilds));
      return resultCollection;
    }

    internal virtual ResultCollection UpdateProcessTemplates(
      ICollection<ProcessTemplate> processTemplates,
      bool throwIfNotExists)
    {
      this.TraceEnter(0, nameof (UpdateProcessTemplates));
      this.PrepareStoredProcedure("prc_UpdateBuildProcessTemplates");
      this.BindTable<ProcessTemplate>("@processTemplateTable", (TeamFoundationTableValueParameter<ProcessTemplate>) new BuildProcessTemplateTable(processTemplates, false));
      this.BindBoolean("@throwIfNotExists", throwIfNotExists);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (UpdateProcessTemplates));
      return resultCollection;
    }

    internal ResultCollection QueryBuildServerProperties()
    {
      this.TraceEnter(0, nameof (QueryBuildServerProperties));
      this.PrepareStoredProcedure("prc_QueryBuildServerProperties");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<GatedCheckInKey>((ObjectBinder<GatedCheckInKey>) new GatedCheckInKeyBinder());
      this.TraceLeave(0, nameof (QueryBuildServerProperties));
      return resultCollection;
    }

    internal ResultCollection UpdateBuildServerProperties(
      byte[] key,
      byte[] iv,
      int blockSize,
      bool replace)
    {
      this.TraceEnter(0, nameof (UpdateBuildServerProperties));
      this.PrepareStoredProcedure("prc_UpdateBuildServerProperties");
      this.BindBinary("@key", key, SqlDbType.VarBinary);
      this.BindBinary("@iv", iv, SqlDbType.VarBinary);
      this.BindInt("@blockSize", blockSize);
      this.BindBoolean("@replace", replace);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<GatedCheckInKey>((ObjectBinder<GatedCheckInKey>) new GatedCheckInKeyBinder());
      this.TraceLeave(0, nameof (UpdateBuildServerProperties));
      return resultCollection;
    }

    internal virtual List<BuildDefinition> QueryDefinitionsWithNewBuilds(DateTime minFinishTime) => throw new NotImplementedException();

    internal virtual ResultCollection EvaluateRetentionPolicy(BuildDefinition definition)
    {
      this.TraceEnter(0, nameof (EvaluateRetentionPolicy));
      this.PrepareStoredProcedure("prc_EvaluateRetentionPolicy");
      this.BindItemUriToInt32("@definitionId", definition.Uri);
      ResultCollection retentionPolicy = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      retentionPolicy.AddBinder<KeyValuePair<DeleteOptions, string>>((ObjectBinder<KeyValuePair<DeleteOptions, string>>) new BuildDeletionOptionsBinder());
      this.TraceLeave(0, nameof (EvaluateRetentionPolicy));
      return retentionPolicy;
    }

    internal virtual ResultCollection ProcessChangeset(
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
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (ProcessChangeset));
      return resultCollection;
    }

    internal virtual IEnumerable<string> GetBuildGroups()
    {
      this.TraceEnter(0, nameof (GetBuildGroups));
      this.PrepareStoredProcedure("prc_GetBuildGroups");
      SqlDataReader sqlDataReader = this.ExecuteReader();
      List<string> buildGroups = new List<string>();
      while (sqlDataReader.Read())
        buildGroups.Add(sqlDataReader.GetString(0));
      this.TraceLeave(0, nameof (GetBuildGroups));
      return (IEnumerable<string>) buildGroups;
    }

    public virtual List<BuildServiceHost> GetDisconnectedBuildServiceHosts(
      TimeSpan disconnectTimeout)
    {
      return new List<BuildServiceHost>();
    }
  }
}
