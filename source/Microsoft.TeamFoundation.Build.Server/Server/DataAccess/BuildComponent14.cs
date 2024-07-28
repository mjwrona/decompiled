// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildComponent14
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildComponent14 : BuildComponent13
  {
    protected static SqlMetaData[] typ_BuildDefinitionTableV3 = new SqlMetaData[12]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("TriggerType", SqlDbType.TinyInt),
      new SqlMetaData("QuietPeriod", SqlDbType.Int),
      new SqlMetaData("BatchSize", SqlDbType.Int),
      new SqlMetaData("QueueStatus", SqlDbType.TinyInt),
      new SqlMetaData("ProcessTemplateId", SqlDbType.Int),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L)
    };
    protected static SqlMetaData[] typ_BuildDefinitionRetentionPolicyTableV2 = new SqlMetaData[6]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("BuildStatus", SqlDbType.Int),
      new SqlMetaData("BuildReason", SqlDbType.Int),
      new SqlMetaData("NumberToKeep", SqlDbType.Int),
      new SqlMetaData("DeleteOptions", SqlDbType.Int)
    };
    protected static SqlMetaData[] typ_BuildDefinitionScheduleTableV2 = new SqlMetaData[11]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("StartTime", SqlDbType.Int),
      new SqlMetaData("Weekday1", SqlDbType.Bit),
      new SqlMetaData("Weekday2", SqlDbType.Bit),
      new SqlMetaData("Weekday3", SqlDbType.Bit),
      new SqlMetaData("Weekday4", SqlDbType.Bit),
      new SqlMetaData("Weekday5", SqlDbType.Bit),
      new SqlMetaData("Weekday6", SqlDbType.Bit),
      new SqlMetaData("Weekday7", SqlDbType.Bit),
      new SqlMetaData("TimeZoneId", SqlDbType.NVarChar, 32L)
    };
    protected static SqlMetaData[] typ_BuildDefinitionSourceProviderTableV2 = new SqlMetaData[5]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("SourceProviderId", SqlDbType.Int),
      new SqlMetaData("SourceProvider", SqlDbType.NVarChar, 64L),
      new SqlMetaData("Fields", SqlDbType.NVarChar, -1L)
    };
    protected static SqlMetaData[] typ_BuildDefinitionWorkspaceTableV3 = new SqlMetaData[6]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, 400L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, 260L),
      new SqlMetaData("MappingType", SqlDbType.TinyInt),
      new SqlMetaData("Depth", SqlDbType.Int)
    };
    protected static SqlMetaData[] typ_BuildProcessTemplateTableV4 = new SqlMetaData[8]
    {
      new SqlMetaData("TemplateId", SqlDbType.Int),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("ServerPath", SqlDbType.NVarChar, 400L),
      new SqlMetaData("TemplateType", SqlDbType.Int),
      new SqlMetaData("SupportedReasons", SqlDbType.Int),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Parameters", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Version", SqlDbType.NVarChar, 47L)
    };
    protected static SqlMetaData[] typ_BuildIdDataspaceTable = new SqlMetaData[2]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("DataspaceId", SqlDbType.Int)
    };
    protected static SqlMetaData[] typ_BuildUriDataspaceTable = new SqlMetaData[2]
    {
      new SqlMetaData("Uri", SqlDbType.VarChar, 256L),
      new SqlMetaData("DataspaceId", SqlDbType.Int)
    };
    protected static SqlMetaData[] typ_AffectedBuildDefinitionTableV3 = new SqlMetaData[5]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("TriggerType", SqlDbType.TinyInt)
    };
    protected static SqlMetaData[] typ_BuildUpdateTableV4 = new SqlMetaData[15]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Fields", SqlDbType.Int),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 260L),
      new SqlMetaData("DropLocationRoot", SqlDbType.NVarChar, 400L),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("LabelName", SqlDbType.NVarChar, 326L),
      new SqlMetaData("LogLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("ContainerId", SqlDbType.BigInt),
      new SqlMetaData("Quality", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("CompilationStatus", SqlDbType.TinyInt),
      new SqlMetaData("TestStatus", SqlDbType.TinyInt),
      new SqlMetaData("KeepForever", SqlDbType.Bit),
      new SqlMetaData("SourceGetVersion", SqlDbType.NVarChar, 325L)
    };

    public BuildComponent14()
    {
      this.ServiceVersion = ServiceVersion.V14;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override BuildDetail AddBuild(
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
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
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
        resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
        ObjectBinder<BuildDetail> current = resultCollection.GetCurrent<BuildDetail>();
        current.MoveNext();
        this.TraceLeave(0, nameof (AddBuild));
        return current.Current;
      }
    }

    internal override ResultCollection AddBuildDefinitions(
      ICollection<BuildDefinition> definitions,
      TeamFoundationIdentity requestedBy,
      Guid writerId,
      bool allowMissingController)
    {
      this.TraceEnter(0, nameof (AddBuildDefinitions));
      this.PrepareStoredProcedure("prc_AddBuildDefinitions");
      List<Schedule> rows1 = new List<Schedule>();
      List<BuildDefinitionSourceProvider> rows2 = new List<BuildDefinitionSourceProvider>();
      List<RetentionPolicy> rows3 = new List<RetentionPolicy>();
      List<WorkspaceMapping> rows4 = new List<WorkspaceMapping>();
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>();
      int num = 0;
      foreach (BuildDefinition definition1 in (IEnumerable<BuildDefinition>) definitions)
      {
        BuildDefinition definition = definition1;
        definition.Uri = DBHelper.CreateArtifactUri("Definition", ++num);
        this.Trace(0, TraceLevel.Info, "Definition uri '{0}' created", (object) definition.Uri);
        definition.Schedules.ForEach((Action<Schedule>) (x =>
        {
          x.DefinitionUri = definition.Uri;
          x.ProjectId = definition.TeamProject.Id;
        }));
        definition.SourceProviders.ForEach((Action<BuildDefinitionSourceProvider>) (x =>
        {
          x.DefinitionUri = definition.Uri;
          x.ProjectId = definition.TeamProject.Id;
        }));
        definition.RetentionPolicies.ForEach((Action<RetentionPolicy>) (x =>
        {
          x.DefinitionUri = definition.Uri;
          x.ProjectId = definition.TeamProject.Id;
        }));
        definition.WorkspaceTemplate.Mappings.ForEach((Action<WorkspaceMapping>) (x =>
        {
          x.DefinitionUri = definition.Uri;
          x.ProjectId = definition.TeamProject.Id;
        }));
        rows1.AddRange((IEnumerable<Schedule>) definition.Schedules);
        rows2.AddRange((IEnumerable<BuildDefinitionSourceProvider>) definition.SourceProviders);
        rows3.AddRange((IEnumerable<RetentionPolicy>) definition.RetentionPolicies);
        rows4.AddRange((IEnumerable<WorkspaceMapping>) definition.WorkspaceTemplate.Mappings);
        properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateEmptyArtifactSpec(BuildPropertyKinds.BuildDefinition, definition.Id, definition.TeamProject.Id), (IEnumerable<PropertyValue>) definition.Properties));
      }
      this.BindBuildDefinitionTable("@buildDefinitionTable", (IEnumerable<BuildDefinition>) definitions);
      this.BindBuildDefinitionRetentionPolicyTable("@retentionPolicyTable", (IEnumerable<RetentionPolicy>) rows3);
      this.BindBuildDefinitionScheduleTable("@scheduleTable", (IEnumerable<Schedule>) rows1);
      this.BindBuildDefinitionSourceProviderTable("@sourceProviderTable", (IEnumerable<BuildDefinitionSourceProvider>) rows2);
      this.BindBuildDefinitionWorkspaceTable("@workspaceTable", (IEnumerable<WorkspaceMapping>) rows4);
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildDefinition);
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindGuid("@writerId", writerId);
      this.BindBoolean("@allowMissingController", allowMissingController);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (AddBuildDefinitions));
      return resultCollection;
    }

    internal override void AddBuildQualities(TeamProject teamProject, IList<string> qualities)
    {
      this.TraceEnter(0, nameof (AddBuildQualities));
      this.PrepareStoredProcedure("prc_AddBuildQualities");
      this.BindInt("@dataspaceId", this.GetDataspaceId(teamProject.Id));
      this.BindStringTable("@qualityTable", (IEnumerable<string>) qualities.Select<string, string>((System.Func<string, string>) (x => BuildQuality.TryConvertBuildQualityToResId(x))).ToArray<string>(), maxLength: 256);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (AddBuildQualities));
    }

    internal override ResultCollection AddProcessTemplates(
      ICollection<ProcessTemplate> processTemplates)
    {
      this.TraceEnter(0, nameof (AddProcessTemplates));
      this.PrepareStoredProcedure("prc_AddBuildProcessTemplates");
      this.BindBuildProcessTemplateTable("@processTemplateTable", (IEnumerable<ProcessTemplate>) processTemplates, true);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (AddProcessTemplates));
      return resultCollection;
    }

    internal override void DeleteBuildDefinitions(
      IEnumerable<BuildDefinition> definitions,
      Guid writerId,
      ArtifactKind definitionArtifactKind)
    {
      this.TraceEnter(0, nameof (DeleteBuildDefinitions));
      this.PrepareStoredProcedure("prc_DeleteBuildDefinitions", 3600);
      this.BindDefinitionIdDataspaceTable("@definitionsTable", definitions);
      this.BindGuid("@writerId", writerId);
      this.BindInt("@compactKindId", definitionArtifactKind.CompactKindId);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteBuildDefinitions));
    }

    internal override void DeleteBuildQualities(TeamProject teamProject, IList<string> qualities)
    {
      this.TraceEnter(0, nameof (DeleteBuildQualities));
      this.PrepareStoredProcedure("prc_DeleteBuildQualities");
      this.BindInt("@dataspaceId", this.GetDataspaceId(teamProject.Id));
      this.BindStringTable("@qualityTable", (IEnumerable<string>) qualities.Select<string, string>((System.Func<string, string>) (x => BuildQuality.TryConvertBuildQualityToResId(x))).ToArray<string>(), maxLength: 256);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteBuildQualities));
    }

    internal override ResultCollection DeleteBuilds(
      IEnumerable<BuildDetail> builds,
      DeleteOptions options,
      TeamFoundationIdentity deletedBy)
    {
      this.TraceEnter(0, nameof (DeleteBuilds));
      this.PrepareStoredProcedure("prc_DeleteBuilds", 3600);
      this.BindBuildUriDataspaceTable("@buildUriDataspaceTable", builds);
      this.BindInt("@options", (int) options);
      this.BindIdentity("@deletedBy", deletedBy);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<KeyValuePair<DeleteOptions, string>>((ObjectBinder<KeyValuePair<DeleteOptions, string>>) new BuildDeletionOptionsBinder());
      resultCollection.AddBinder<SymbolStoreData>((ObjectBinder<SymbolStoreData>) new SymbolStoreDataBinder());
      this.TraceLeave(0, nameof (DeleteBuilds));
      return resultCollection;
    }

    internal override void DeleteProcessTemplates(ICollection<ProcessTemplate> processTemplates)
    {
      this.TraceEnter(0, nameof (DeleteProcessTemplates));
      this.PrepareStoredProcedure("prc_DeleteBuildProcessTemplates");
      this.BindProcessTemplateIdDataspaceTable("@processTemplateTable", (IEnumerable<ProcessTemplate>) processTemplates);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteProcessTemplates));
    }

    internal override IEnumerable<string> DeleteTeamProject(
      string projectUri,
      Guid writerId,
      ArtifactKind definitionArtifactKind)
    {
      this.TraceEnter(0, nameof (DeleteTeamProject));
      this.PrepareStoredProcedure("prc_DeleteTeamProject");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Parse(LinkingUtilities.DecodeUri(projectUri).ToolSpecificId)));
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

    internal override void DestroyBuilds(IEnumerable<BuildDetail> builds)
    {
      this.TraceEnter(0, nameof (DestroyBuilds));
      this.PrepareStoredProcedure("prc_DestroyBuilds", 3600);
      this.BindBuildUriDataspaceTable("@buildUriTable", builds);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DestroyBuilds));
    }

    internal override ResultCollection EvaluateRetentionPolicy(BuildDefinition definition)
    {
      this.TraceEnter(0, nameof (EvaluateRetentionPolicy));
      this.PrepareStoredProcedure("prc_EvaluateRetentionPolicy");
      this.BindInt("@definitionId", definition.Id);
      this.BindInt("@dataspaceId", this.GetDataspaceId(definition.TeamProject.Id));
      ResultCollection retentionPolicy = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      retentionPolicy.AddBinder<KeyValuePair<DeleteOptions, string>>((ObjectBinder<KeyValuePair<DeleteOptions, string>>) new BuildDeletionOptionsBinder());
      this.TraceLeave(0, nameof (EvaluateRetentionPolicy));
      return retentionPolicy;
    }

    internal override IEnumerable<string> GetBuildGroups()
    {
      this.TraceEnter(0, nameof (GetBuildGroups));
      this.PrepareStoredProcedure("prc_GetBuildGroups");
      SqlDataReader sqlDataReader = this.ExecuteReader();
      List<string> buildGroups = new List<string>();
      while (sqlDataReader.Read())
        buildGroups.Add(LinkingUtilities.EncodeUri(new ArtifactId()
        {
          ToolSpecificId = this.GetDataspaceIdentifier(sqlDataReader.GetInt32(0)).ToString("D"),
          ArtifactType = "TeamProject",
          Tool = "Classification"
        }));
      this.TraceLeave(0, nameof (GetBuildGroups));
      return (IEnumerable<string>) buildGroups;
    }

    internal override ResultCollection GetBuildQualities(TeamProject teamProject)
    {
      this.TraceEnter(0, nameof (GetBuildQualities));
      this.PrepareStoredProcedure("prc_GetBuildQualities");
      if (teamProject != null)
        this.BindInt("@dataspaceId", this.GetDataspaceId(teamProject.Id));
      else
        this.BindNullValue("@teamProject", SqlDbType.Int);
      ResultCollection buildQualities = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      buildQualities.AddBinder<string>((ObjectBinder<string>) new BuildQualityBinder());
      this.TraceLeave(0, nameof (GetBuildQualities));
      return buildQualities;
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
      this.BindAffectedDefinitionsTable("@affectedDefinitionTable", (IEnumerable<BuildDefinition>) affectedDefinitions);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (ProcessChangeset));
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
      this.BindTeamProjectDataspace(requestContext, "@dataspaceId", teamProject, true);
      this.BindItemPath("@definitionPath", definitionPath, false);
      this.BindByte("@continuousIntegrationType", (byte) continuousIntegrationType);
      this.BindByte("@queryOptions", (byte) options);
      this.BindBoolean("@strict", strict);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
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
      this.BindUrisToInt32Table("@definitionIdTable", uris);
      this.BindByte("@queryOptions", (byte) options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildDefinitionsByUri));
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
        this.BindTeamProjectDataspace(requestContext, "@dataspaceId", spec.TeamProject, true);
        this.BindItemPath("@definitionPath", spec.Path, false);
      }
      else
        this.BindUrisToDistinctInt32Table("@definitionIdTable", (IEnumerable<string>) spec.DefinitionFilter);
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
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
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
      this.BindUrisToOrderedStringTable("@buildUriTable", (IEnumerable<string>) uris);
      this.BindInt("@queryOptions", (int) options);
      this.BindInt("@queryDeletedOption", (int) queryDeletedOption);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildsByUri));
      return resultCollection;
    }

    internal override ResultCollection QueryChangedBuilds(
      IVssRequestContext requestContext,
      ICollection<string> teamProjects,
      DateTime minChangedTime,
      Microsoft.TeamFoundation.Build.Server.BuildStatus statusFilter,
      int batchSize = 2147483647)
    {
      this.TraceEnter(0, nameof (QueryChangedBuilds));
      this.PrepareStoredProcedure("prc_QueryChangedBuilds");
      this.BindTeamProjectTableDataspace(requestContext, "@dataspaceIdtable", teamProjects);
      this.BindUtcDateTime("@minChangedTime", minChangedTime);
      this.BindNullableInt("@statusFilter", (int) statusFilter, 63);
      this.BindInt("@batchSize", batchSize);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryChangedBuilds));
      return resultCollection;
    }

    internal override List<BuildDefinition> QueryDefinitionsWithNewBuilds(DateTime minFinishTime)
    {
      this.TraceEnter(0, nameof (QueryDefinitionsWithNewBuilds));
      this.PrepareStoredProcedure("prc_QueryDefinitionsWithNewBuilds");
      this.BindNullableDateTime("@minFinishTime", minFinishTime);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      List<BuildDefinition> buildDefinitionList = new List<BuildDefinition>();
      while (sqlDataReader.Read())
        buildDefinitionList.Add(new BuildDefinition()
        {
          TeamProject = this.GetTeamProjectFromGuid(this.RequestContext, this.GetDataspaceIdentifier(sqlDataReader.GetInt32(0))),
          Uri = DBHelper.CreateArtifactUri("Definition", sqlDataReader.GetInt32(1))
        });
      this.TraceLeave(0, nameof (QueryDefinitionsWithNewBuilds));
      return buildDefinitionList;
    }

    internal override ResultCollection QueryProcessTemplates(
      IVssRequestContext requestContext,
      string teamProject,
      IEnumerable<ProcessTemplateType> templateTypes,
      bool includeDeleted)
    {
      this.TraceEnter(0, nameof (QueryProcessTemplates));
      this.PrepareStoredProcedure("prc_QueryBuildProcessTemplates");
      this.BindInt32Table("@templateTypeTable", (IEnumerable<int>) templateTypes.Cast<int>().ToArray<int>());
      this.BindTeamProjectDataspace(requestContext, "@dataspaceId", teamProject, true);
      this.BindBoolean("@includeDeleted", includeDeleted);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryProcessTemplates));
      return resultCollection;
    }

    internal override ResultCollection QueryProcessTemplatesById(ICollection<int> templateIds)
    {
      this.TraceEnter(0, nameof (QueryProcessTemplatesById));
      this.PrepareStoredProcedure("prc_QueryBuildProcessTemplatesById");
      this.BindInt32Table("@processTemplateIdTable", (IEnumerable<int>) templateIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryProcessTemplatesById));
      return resultCollection;
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
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
        this.TraceLeave(0, nameof (QueryProcessTemplatesByPath));
        return resultCollection.GetCurrent<ProcessTemplate>().Items;
      }
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
      List<BuildDefinitionSourceProvider> rows2 = new List<BuildDefinitionSourceProvider>();
      List<RetentionPolicy> rows3 = new List<RetentionPolicy>();
      List<WorkspaceMapping> rows4 = new List<WorkspaceMapping>();
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>(definitions.Count);
      foreach (BuildDefinition definition1 in (IEnumerable<BuildDefinition>) definitions)
      {
        BuildDefinition definition = definition1;
        definition.Schedules.ForEach((Action<Schedule>) (x =>
        {
          x.DefinitionUri = definition.Uri;
          x.ProjectId = definition.TeamProject.Id;
        }));
        definition.SourceProviders.ForEach((Action<BuildDefinitionSourceProvider>) (x =>
        {
          x.DefinitionUri = definition.Uri;
          x.ProjectId = definition.TeamProject.Id;
        }));
        definition.RetentionPolicies.ForEach((Action<RetentionPolicy>) (x =>
        {
          x.DefinitionUri = definition.Uri;
          x.ProjectId = definition.TeamProject.Id;
        }));
        rows1.AddRange((IEnumerable<Schedule>) definition.Schedules);
        rows2.AddRange((IEnumerable<BuildDefinitionSourceProvider>) definition.SourceProviders);
        rows3.AddRange((IEnumerable<RetentionPolicy>) definition.RetentionPolicies);
        if (definition.WorkspaceTemplate != null)
        {
          definition.WorkspaceTemplate.Mappings.ForEach((Action<WorkspaceMapping>) (x =>
          {
            x.DefinitionUri = definition.Uri;
            x.ProjectId = definition.TeamProject.Id;
          }));
          rows4.AddRange((IEnumerable<WorkspaceMapping>) definition.WorkspaceTemplate.Mappings);
        }
        properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateArtifactSpec(definition.Uri, BuildPropertyKinds.BuildDefinition, definition.TeamProject.Id), (IEnumerable<PropertyValue>) definition.Properties));
      }
      this.BindBuildDefinitionTable("@buildDefinitionTable", (IEnumerable<BuildDefinition>) definitions);
      this.BindBuildDefinitionRetentionPolicyTable("@retentionPolicyTable", (IEnumerable<RetentionPolicy>) rows3);
      this.BindBuildDefinitionScheduleTable("@scheduleTable", (IEnumerable<Schedule>) rows1);
      this.BindBuildDefinitionSourceProviderTable("@sourceProviderTable", (IEnumerable<BuildDefinitionSourceProvider>) rows2);
      this.BindBuildDefinitionWorkspaceTable("@workspaceTable", (IEnumerable<WorkspaceMapping>) rows4);
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildDefinition);
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindGuid("@writerId", writerId);
      this.BindBoolean("@isServicingRequest", isServicingRequest);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildDefinitions));
      return resultCollection;
    }

    internal override ResultCollection UpdateBuilds(
      ICollection<BuildUpdateOptions> options,
      TeamFoundationIdentity requestedBy,
      Guid writerId)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("prc_UpdateBuilds");
      this.BindBuildUpdateTable("@buildUpdateTable", (IEnumerable<BuildUpdateOptions>) options);
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindGuid("@writerId", writerId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildUpdateOptions>((ObjectBinder<BuildUpdateOptions>) new BuildUpdateOptionsBinder());
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      resultCollection.AddBinder<SharedResourceData>((ObjectBinder<SharedResourceData>) new SharedResourceDataBinder());
      this.TraceLeave(0, nameof (UpdateBuilds));
      return resultCollection;
    }

    internal override ResultCollection UpdateProcessTemplates(
      ICollection<ProcessTemplate> processTemplates,
      bool throwIfNotExists)
    {
      this.TraceEnter(0, nameof (UpdateProcessTemplates));
      this.PrepareStoredProcedure("prc_UpdateBuildProcessTemplates");
      this.BindBuildProcessTemplateTable("@processTemplateTable", (IEnumerable<ProcessTemplate>) processTemplates, false);
      this.BindBoolean("@throwIfNotExists", throwIfNotExists);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (UpdateProcessTemplates));
      return resultCollection;
    }

    protected virtual SqlParameter BindBuildDefinitionTable(
      string parameterName,
      IEnumerable<BuildDefinition> rows)
    {
      return this.BindTable(parameterName, "typ_BuildDefinitionTableV3", (rows ?? Enumerable.Empty<BuildDefinition>()).Select<BuildDefinition, SqlDataRecord>(new System.Func<BuildDefinition, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindBuildDefinitionRetentionPolicyTable(
      string parameterName,
      IEnumerable<RetentionPolicy> rows)
    {
      return this.BindTable(parameterName, "typ_BuildDefinitionRetentionPolicyTableV2", (rows ?? Enumerable.Empty<RetentionPolicy>()).Select<RetentionPolicy, SqlDataRecord>(new System.Func<RetentionPolicy, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindBuildDefinitionScheduleTable(
      string parameterName,
      IEnumerable<Schedule> rows)
    {
      return this.BindTable(parameterName, "typ_BuildDefinitionScheduleTableV2", (rows ?? Enumerable.Empty<Schedule>()).Select<Schedule, SqlDataRecord>(new System.Func<Schedule, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindBuildDefinitionSourceProviderTable(
      string parameterName,
      IEnumerable<BuildDefinitionSourceProvider> rows)
    {
      return this.BindTable(parameterName, "typ_BuildDefinitionSourceProviderTableV2", (rows ?? Enumerable.Empty<BuildDefinitionSourceProvider>()).Select<BuildDefinitionSourceProvider, SqlDataRecord>(new System.Func<BuildDefinitionSourceProvider, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindBuildDefinitionWorkspaceTable(
      string parameterName,
      IEnumerable<WorkspaceMapping> rows)
    {
      return this.BindTable(parameterName, "typ_BuildDefinitionWorkspaceTableV3", (rows ?? Enumerable.Empty<WorkspaceMapping>()).Select<WorkspaceMapping, SqlDataRecord>(new System.Func<WorkspaceMapping, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindBuildProcessTemplateTable(
      string parameterName,
      IEnumerable<ProcessTemplate> rows,
      bool isAdd)
    {
      return this.BindTable(parameterName, "typ_BuildProcessTemplateTableV4", (rows ?? Enumerable.Empty<ProcessTemplate>()).Select<ProcessTemplate, SqlDataRecord>((System.Func<ProcessTemplate, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x, isAdd))));
    }

    protected virtual SqlParameter BindDefinitionIdDataspaceTable(
      string parameterName,
      IEnumerable<BuildDefinition> rows)
    {
      return this.BindTable(parameterName, "typ_BuildIdDataspaceTable", (rows ?? Enumerable.Empty<BuildDefinition>()).Select<BuildDefinition, SqlDataRecord>((System.Func<BuildDefinition, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x.Id, x.TeamProject.Id))));
    }

    protected virtual SqlParameter BindBuildUriDataspaceTable(
      string parameterName,
      IEnumerable<BuildDetail> rows)
    {
      return this.BindTable(parameterName, "typ_BuildUriDataspaceTable", (rows ?? Enumerable.Empty<BuildDetail>()).Select<BuildDetail, SqlDataRecord>(new System.Func<BuildDetail, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindProcessTemplateIdDataspaceTable(
      string parameterName,
      IEnumerable<ProcessTemplate> rows)
    {
      return this.BindTable(parameterName, "typ_BuildIdDataspaceTable", (rows ?? Enumerable.Empty<ProcessTemplate>()).Select<ProcessTemplate, SqlDataRecord>((System.Func<ProcessTemplate, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x.Id, x.TeamProjectObj.Id))));
    }

    protected virtual SqlParameter BindAffectedDefinitionsTable(
      string parameterName,
      IEnumerable<BuildDefinition> rows)
    {
      return this.BindTable(parameterName, "typ_AffectedBuildDefinitionTableV3", (rows ?? Enumerable.Empty<BuildDefinition>()).Select<BuildDefinition, SqlDataRecord>(new System.Func<BuildDefinition, SqlDataRecord>(this.ConvertToAffectedDefinitionRecord)));
    }

    protected virtual SqlParameter BindBuildUpdateTable(
      string parameterName,
      IEnumerable<BuildUpdateOptions> rows)
    {
      return this.BindTable(parameterName, "typ_BuildUpdateTableV4", (rows ?? Enumerable.Empty<BuildUpdateOptions>()).Select<BuildUpdateOptions, SqlDataRecord>(new System.Func<BuildUpdateOptions, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(BuildDefinition row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildComponent14.typ_BuildDefinitionTableV3);
      sqlDataRecord.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.Uri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(row.TeamProject.Id));
      sqlDataRecord.SetString(2, DBHelper.ServerPathToDBPath(BuildPath.RemoveTeamProject(row.FullPath)));
      sqlDataRecord.SetInt32(3, int.Parse(DBHelper.ExtractDbId(row.BuildControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetString(4, row.DefaultDropLocation);
      sqlDataRecord.SetByte(5, (byte) row.TriggerType);
      sqlDataRecord.SetInt32(6, row.ContinuousIntegrationQuietPeriod);
      sqlDataRecord.SetInt32(7, row.BatchSize);
      sqlDataRecord.SetByte(8, (byte) row.QueueStatus);
      if (row.Process != null)
        sqlDataRecord.SetInt32(9, row.Process.Id);
      else
        sqlDataRecord.SetDBNull(9);
      if (!string.IsNullOrEmpty(row.Description))
        sqlDataRecord.SetString(10, row.Description);
      else
        sqlDataRecord.SetDBNull(10);
      if (!string.IsNullOrEmpty(row.ProcessParameters))
        sqlDataRecord.SetString(11, row.ProcessParameters);
      else
        sqlDataRecord.SetDBNull(11);
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(RetentionPolicy row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildComponent14.typ_BuildDefinitionRetentionPolicyTableV2);
      sqlDataRecord.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.DefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord.SetInt32(2, (int) row.BuildStatus);
      sqlDataRecord.SetInt32(3, (int) row.BuildReason);
      sqlDataRecord.SetInt32(4, row.NumberToKeep);
      sqlDataRecord.SetInt32(5, (int) row.DeleteOptions);
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(Schedule row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildComponent14.typ_BuildDefinitionScheduleTableV2);
      sqlDataRecord.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.DefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord.SetInt32(2, row.UtcStartTime);
      sqlDataRecord.SetBoolean(3, (row.UtcDaysToBuild & ScheduleDays.Sunday) == ScheduleDays.Sunday);
      sqlDataRecord.SetBoolean(4, (row.UtcDaysToBuild & ScheduleDays.Monday) == ScheduleDays.Monday);
      sqlDataRecord.SetBoolean(5, (row.UtcDaysToBuild & ScheduleDays.Tuesday) == ScheduleDays.Tuesday);
      sqlDataRecord.SetBoolean(6, (row.UtcDaysToBuild & ScheduleDays.Wednesday) == ScheduleDays.Wednesday);
      sqlDataRecord.SetBoolean(7, (row.UtcDaysToBuild & ScheduleDays.Thursday) == ScheduleDays.Thursday);
      sqlDataRecord.SetBoolean(8, (row.UtcDaysToBuild & ScheduleDays.Friday) == ScheduleDays.Friday);
      sqlDataRecord.SetBoolean(9, (row.UtcDaysToBuild & ScheduleDays.Saturday) == ScheduleDays.Saturday);
      sqlDataRecord.SetString(10, row.TimeZoneId);
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(BuildDefinitionSourceProvider row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildComponent14.typ_BuildDefinitionSourceProviderTableV2);
      sqlDataRecord.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.DefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord.SetInt32(2, row.Id);
      sqlDataRecord.SetString(3, row.Name);
      sqlDataRecord.SetString(4, BuildSqlColumnBinderExtensions.ToXml(row.Fields.ToArray()));
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(WorkspaceMapping row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildComponent14.typ_BuildDefinitionWorkspaceTableV3);
      sqlDataRecord.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.DefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord.SetString(2, this.VersionControlPathToDataspaceDBPath(row.ServerItem));
      if (!string.IsNullOrEmpty(row.LocalItem))
        sqlDataRecord.SetString(3, DBHelper.LocalPathToDBPath(row.LocalItem));
      else
        sqlDataRecord.SetDBNull(3);
      sqlDataRecord.SetByte(4, (byte) row.MappingType);
      sqlDataRecord.SetInt32(5, row.Depth);
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(ProcessTemplate row, bool isAdd)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildComponent14.typ_BuildProcessTemplateTableV4);
      if (isAdd)
      {
        sqlDataRecord.SetDBNull(0);
        if (row.ServerPath.StartsWith("$/"))
          sqlDataRecord.SetString(2, this.VersionControlPathToDataspaceDBPath(row.ServerPath));
        else
          sqlDataRecord.SetString(2, DBHelper.VersionControlPathToDBPath(row.ServerPath));
      }
      else
      {
        sqlDataRecord.SetInt32(0, row.Id);
        if (string.IsNullOrEmpty(row.ServerPath))
          sqlDataRecord.SetDBNull(2);
        else if (row.ServerPath.StartsWith("$/"))
          sqlDataRecord.SetString(2, this.VersionControlPathToDataspaceDBPath(row.ServerPath));
        else
          sqlDataRecord.SetString(2, DBHelper.VersionControlPathToDBPath(row.ServerPath));
      }
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(row.TeamProjectObj.Id));
      sqlDataRecord.SetInt32(3, (int) row.TemplateType);
      sqlDataRecord.SetInt32(4, (int) row.SupportedReasons);
      if (!string.IsNullOrEmpty(row.Description))
        sqlDataRecord.SetString(5, row.Description);
      else
        sqlDataRecord.SetDBNull(5);
      if (!string.IsNullOrEmpty(row.Parameters))
        sqlDataRecord.SetString(6, row.Parameters);
      else
        sqlDataRecord.SetDBNull(6);
      if (!string.IsNullOrEmpty(row.Version))
        sqlDataRecord.SetString(7, row.Version);
      else
        sqlDataRecord.SetDBNull(7);
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(int id, Guid projectId)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildComponent14.typ_BuildIdDataspaceTable);
      sqlDataRecord.SetInt32(0, id);
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(projectId));
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(BuildDetail row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildComponent14.typ_BuildUriDataspaceTable);
      sqlDataRecord.SetString(0, DBHelper.ExtractDbId(row.Uri));
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(row.Definition.TeamProject.Id));
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToAffectedDefinitionRecord(BuildDefinition row)
    {
      SqlDataRecord definitionRecord = new SqlDataRecord(BuildComponent14.typ_AffectedBuildDefinitionTableV3);
      definitionRecord.SetInt32(0, this.GetDataspaceId(row.TeamProject.Id));
      definitionRecord.SetInt32(1, row.Id);
      definitionRecord.SetInt32(2, int.Parse(DBHelper.ExtractDbId(row.BuildControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      definitionRecord.SetString(3, row.DefaultDropLocation);
      definitionRecord.SetByte(4, (byte) row.TriggerType);
      return definitionRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(BuildUpdateOptions row)
    {
      SqlDataRecord sqlDataRecord1 = new SqlDataRecord(BuildComponent14.typ_BuildUpdateTableV4);
      sqlDataRecord1.SetInt32(0, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord1.SetString(1, DBHelper.ExtractDbId(row.Uri));
      sqlDataRecord1.SetInt32(2, (int) row.Fields);
      if ((row.Fields & BuildUpdate.BuildNumber) == BuildUpdate.BuildNumber)
        sqlDataRecord1.SetString(3, DBHelper.ServerPathToDBPath(row.BuildNumber));
      else
        sqlDataRecord1.SetDBNull(3);
      if ((row.Fields & BuildUpdate.DropLocationRoot) == BuildUpdate.DropLocationRoot)
        sqlDataRecord1.SetString(4, row.DropLocationRoot);
      else
        sqlDataRecord1.SetDBNull(4);
      if ((row.Fields & BuildUpdate.DropLocation) == BuildUpdate.DropLocation)
        sqlDataRecord1.SetString(5, row.DropLocation);
      else
        sqlDataRecord1.SetDBNull(5);
      if ((row.Fields & BuildUpdate.LabelName) == BuildUpdate.LabelName)
        sqlDataRecord1.SetString(6, row.LabelName);
      else
        sqlDataRecord1.SetDBNull(6);
      if ((row.Fields & BuildUpdate.LogLocation) == BuildUpdate.LogLocation)
        sqlDataRecord1.SetString(7, row.LogLocation);
      else
        sqlDataRecord1.SetDBNull(7);
      if ((row.Fields & BuildUpdate.ContainerId) == BuildUpdate.ContainerId)
      {
        long? containerId = row.ContainerId;
        long num1 = 0;
        if (containerId.GetValueOrDefault() > num1 & containerId.HasValue)
        {
          SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
          containerId = row.ContainerId;
          long num2 = containerId.Value;
          sqlDataRecord2.SetInt64(8, num2);
          goto label_19;
        }
      }
      sqlDataRecord1.SetDBNull(8);
label_19:
      if ((row.Fields & BuildUpdate.Quality) == BuildUpdate.Quality)
        sqlDataRecord1.SetString(9, BuildQuality.TryConvertBuildQualityToResId(row.Quality));
      else
        sqlDataRecord1.SetDBNull(9);
      if ((row.Fields & BuildUpdate.Status) == BuildUpdate.Status)
        sqlDataRecord1.SetByte(10, (byte) row.Status);
      else
        sqlDataRecord1.SetDBNull(10);
      if ((row.Fields & BuildUpdate.CompilationStatus) == BuildUpdate.CompilationStatus)
        sqlDataRecord1.SetByte(11, (byte) row.CompilationStatus);
      else
        sqlDataRecord1.SetDBNull(11);
      if ((row.Fields & BuildUpdate.TestStatus) == BuildUpdate.TestStatus)
        sqlDataRecord1.SetByte(12, (byte) row.TestStatus);
      else
        sqlDataRecord1.SetDBNull(12);
      if ((row.Fields & BuildUpdate.KeepForever) == BuildUpdate.KeepForever)
        sqlDataRecord1.SetBoolean(13, row.KeepForever);
      else
        sqlDataRecord1.SetDBNull(13);
      if ((row.Fields & BuildUpdate.SourceGetVersion) == BuildUpdate.SourceGetVersion)
        sqlDataRecord1.SetString(14, row.SourceGetVersion);
      else
        sqlDataRecord1.SetDBNull(14);
      return sqlDataRecord1;
    }
  }
}
