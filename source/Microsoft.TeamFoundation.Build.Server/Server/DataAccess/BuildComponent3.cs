// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent3
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent3 : BuildComponent2
  {
    public BuildComponent3()
    {
      this.ServiceVersion = ServiceVersion.V3;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection AddBuildDefinitions(
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
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      this.TraceLeave(0, nameof (AddBuildDefinitions));
      return resultCollection;
    }

    internal override ResultCollection AddProcessTemplates(
      ICollection<ProcessTemplate> processTemplates)
    {
      this.TraceEnter(0, nameof (AddProcessTemplates));
      this.PrepareStoredProcedure("prc_AddBuildProcessTemplates");
      this.BindTable<ProcessTemplate>("@processTemplateTable", (TeamFoundationTableValueParameter<ProcessTemplate>) new BuildProcessTemplateTable2((IEnumerable<ProcessTemplate>) processTemplates, true));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (AddProcessTemplates));
      return resultCollection;
    }

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
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
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
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
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
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
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
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildDefinitionsByUri));
      return resultCollection;
    }

    internal override ResultCollection QueryProcessTemplates(
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
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryProcessTemplates));
      return resultCollection;
    }

    internal override ResultCollection QueryProcessTemplatesById(ICollection<int> templateIds)
    {
      this.TraceEnter(0, nameof (QueryProcessTemplatesById));
      this.PrepareStoredProcedure("prc_QueryBuildProcessTemplatesById");
      this.BindTable<int>("@processTemplateIdTable", (TeamFoundationTableValueParameter<int>) new Int32Table((IEnumerable<int>) templateIds));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryProcessTemplatesById));
      return resultCollection;
    }

    internal override ResultCollection UpdateBuildDefinitions(
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
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildDefinitions));
      return resultCollection;
    }

    internal override ResultCollection UpdateProcessTemplates(
      ICollection<ProcessTemplate> processTemplates,
      bool throwIfNotExists)
    {
      this.TraceEnter(0, nameof (UpdateProcessTemplates));
      this.PrepareStoredProcedure("prc_UpdateBuildProcessTemplates");
      this.BindTable<ProcessTemplate>("@processTemplateTable", (TeamFoundationTableValueParameter<ProcessTemplate>) new BuildProcessTemplateTable2((IEnumerable<ProcessTemplate>) processTemplates, false));
      this.BindBoolean("@throwIfNotExists", throwIfNotExists);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (UpdateProcessTemplates));
      return resultCollection;
    }
  }
}
