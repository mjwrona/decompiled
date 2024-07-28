// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent5
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

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent5 : BuildComponent4
  {
    public BuildComponent5()
    {
      this.ServiceVersion = ServiceVersion.V5;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
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
      this.BindTable<BuildDefinition>("@affectedDefinitionTable", (TeamFoundationTableValueParameter<BuildDefinition>) new AffectedBuildDefinitionTable2((IEnumerable<BuildDefinition>) affectedDefinitions));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (ProcessChangeset));
      return resultCollection;
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
      this.BindTable<BuildDefinition>("@buildDefinitionTable", (TeamFoundationTableValueParameter<BuildDefinition>) new BuildDefinitionTable2((IEnumerable<BuildDefinition>) definitions));
      this.BindTable<RetentionPolicy>("@retentionPolicyTable", (TeamFoundationTableValueParameter<RetentionPolicy>) new BuildDefinitionRetentionPolicyTable((ICollection<RetentionPolicy>) rows2));
      this.BindTable<Schedule>("@scheduleTable", (TeamFoundationTableValueParameter<Schedule>) new BuildDefinitionScheduleTable((ICollection<Schedule>) rows1));
      this.BindTable<WorkspaceMapping>("@workspaceTable", (TeamFoundationTableValueParameter<WorkspaceMapping>) new BuildDefinitionWorkspaceTable2((IEnumerable<WorkspaceMapping>) rows3));
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
      this.BindTable<BuildDefinition>("@buildDefinitionTable", (TeamFoundationTableValueParameter<BuildDefinition>) new BuildDefinitionTable2((IEnumerable<BuildDefinition>) definitions));
      this.BindTable<RetentionPolicy>("@retentionPolicyTable", (TeamFoundationTableValueParameter<RetentionPolicy>) new BuildDefinitionRetentionPolicyTable((ICollection<RetentionPolicy>) rows2));
      this.BindTable<Schedule>("@scheduleTable", (TeamFoundationTableValueParameter<Schedule>) new BuildDefinitionScheduleTable((ICollection<Schedule>) rows1));
      this.BindTable<WorkspaceMapping>("@workspaceTable", (TeamFoundationTableValueParameter<WorkspaceMapping>) new BuildDefinitionWorkspaceTable2((IEnumerable<WorkspaceMapping>) rows3));
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

    internal override ResultCollection AddProcessTemplates(
      ICollection<ProcessTemplate> processTemplates)
    {
      this.TraceEnter(0, nameof (AddProcessTemplates));
      this.PrepareStoredProcedure("prc_AddBuildProcessTemplates");
      this.BindTable<ProcessTemplate>("@processTemplateTable", (TeamFoundationTableValueParameter<ProcessTemplate>) new BuildProcessTemplateTable3((IEnumerable<ProcessTemplate>) processTemplates, true));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (AddProcessTemplates));
      return resultCollection;
    }

    internal override ResultCollection UpdateProcessTemplates(
      ICollection<ProcessTemplate> processTemplates,
      bool throwIfNotExists)
    {
      this.TraceEnter(0, nameof (UpdateProcessTemplates));
      this.PrepareStoredProcedure("prc_UpdateBuildProcessTemplates");
      this.BindTable<ProcessTemplate>("@processTemplateTable", (TeamFoundationTableValueParameter<ProcessTemplate>) new BuildProcessTemplateTable3((IEnumerable<ProcessTemplate>) processTemplates, false));
      this.BindBoolean("@throwIfNotExists", throwIfNotExists);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (UpdateProcessTemplates));
      return resultCollection;
    }

    internal override ResultCollection UpdateBuilds(
      ICollection<BuildUpdateOptions> options,
      TeamFoundationIdentity requestedBy,
      Guid writerId)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("prc_UpdateBuilds");
      this.BindTable<BuildUpdateOptions>("@buildUpdateTable", (TeamFoundationTableValueParameter<BuildUpdateOptions>) new BuildUpdateTableV3((IEnumerable<BuildUpdateOptions>) options));
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindGuid("@writerId", writerId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildUpdateOptions>((ObjectBinder<BuildUpdateOptions>) new BuildUpdateOptionsBinder());
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      resultCollection.AddBinder<SharedResourceData>((ObjectBinder<SharedResourceData>) new SharedResourceDataBinder());
      this.TraceLeave(0, nameof (UpdateBuilds));
      return resultCollection;
    }
  }
}
