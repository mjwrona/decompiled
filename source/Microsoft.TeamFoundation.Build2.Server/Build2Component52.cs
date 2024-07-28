// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component52
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component52 : Build2Component51
  {
    protected static readonly SqlMetaData[] typ_BuildUpdateTable5 = new SqlMetaData[12]
    {
      new SqlMetaData("BuildId", SqlDbType.Int),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 260L),
      new SqlMetaData("StartTime", SqlDbType.DateTime2),
      new SqlMetaData("FinishTime", SqlDbType.DateTime2),
      new SqlMetaData("SourceBranch", SqlDbType.NVarChar, 400L),
      new SqlMetaData("SourceVersion", SqlDbType.NVarChar, 326L),
      new SqlMetaData("BuildStatus", SqlDbType.TinyInt),
      new SqlMetaData("BuildResult", SqlDbType.TinyInt),
      new SqlMetaData("KeepForever", SqlDbType.Bit),
      new SqlMetaData("RetainedByRelease", SqlDbType.Bit),
      new SqlMetaData("ValidationIssues", SqlDbType.NVarChar, -1L),
      new SqlMetaData("SourceVersionInfo", SqlDbType.NVarChar, 1024L)
    };
    protected static readonly SqlMetaData[] typ_AuthorizedResourceTable = new SqlMetaData[2]
    {
      new SqlMetaData("ResourceType", SqlDbType.Int),
      new SqlMetaData("ResourceId", SqlDbType.NVarChar, 100L)
    };

    public override BuildDefinition AddDefinition(
      BuildDefinition definition,
      Guid requestedBy,
      BuildProcessResources authorizedResources = null)
    {
      this.TraceEnter(0, nameof (AddDefinition));
      this.PrepareStoredProcedure("Build.prc_AddDefinition");
      string parameterValue1 = (string) null;
      string parameterValue2 = (string) null;
      string parameterValue3 = (string) null;
      if (definition.Repository != null)
      {
        parameterValue1 = definition.Repository.Type;
        parameterValue2 = definition.Repository.Id;
        parameterValue3 = definition.Repository.DefaultBranch;
      }
      this.BindInt("@dataspaceId", this.GetDataspaceId(definition.ProjectId));
      this.BindString("@definitionName", DBHelper.ServerPathToDBPath(definition.Name), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindNullableInt32("@queueId", definition.Queue != null ? new int?(definition.Queue.Id) : new int?());
      this.BindByte("@queueStatus", (byte) definition.QueueStatus);
      this.BindByte("@quality", (byte) ((int) definition.DefinitionQuality ?? 1));
      this.BindString("@repositoryType", parameterValue1, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repositoryIdentifier", parameterValue2, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@defaultBranch", parameterValue3, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@triggerTypes", (int) definition.Triggers.Aggregate<BuildTrigger, DefinitionTriggerType>(DefinitionTriggerType.None, (Func<DefinitionTriggerType, BuildTrigger, DefinitionTriggerType>) ((x, y) => x | y.TriggerType)));
      this.BindNullableInt32("@parentDefinitionId", definition.ParentDefinition != null ? new int?(definition.ParentDefinition.Id) : new int?());
      this.BindString("@options", this.ToString<List<BuildOption>>(definition.Options), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repository", JsonUtility.ToString((object) definition.Repository), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@triggers", this.ToString<List<BuildTrigger>>(definition.Triggers), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@process", JsonUtility.ToString((object) definition.Process), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      BuildProcess process = definition.Process;
      this.BindInt("@processType", process != null ? process.Type : 1);
      this.BindString("@variables", this.ToString<Dictionary<string, BuildDefinitionVariable>>(definition.Variables), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@variableGroups", this.ToString<List<VariableGroup>>(definition.VariableGroups), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@demands", this.ToString<List<Demand>>(definition.Demands), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@retentionPolicy", (string) null, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@description", definition.Description, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@buildNumberFormat", definition.BuildNumberFormat, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@jobAuthorizationScope", (byte) definition.JobAuthorizationScope);
      this.BindNullableInt("@jobTimeout", definition.JobTimeoutInMinutes, 0);
      this.BindInt("@jobCancelTimeout", definition.JobCancelTimeoutInMinutes);
      this.BindBoolean("@badgeEnabled", definition.BadgeEnabled);
      this.BindString("@comment", definition.Comment, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@requestedBy", requestedBy);
      this.BindGuid("@writerId", this.Author);
      this.BindString("@path", DBHelper.UserToDBPath(definition.Path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@processParameters", JsonUtility.ToString((object) definition.ProcessParameters), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@buildCompletionTriggersString", JsonUtility.ToString<BuildCompletionTrigger>((IList<BuildCompletionTrigger>) definition.BuildCompletionTriggers), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBuildCompletionTriggerTable("@buildCompletionTriggers", (IEnumerable<BuildCompletionTrigger>) definition.BuildCompletionTriggers);
      this.BindAuthorizedResourcesTable("@authorizedResources", authorizedResources ?? new BuildProcessResources());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        BuildDefinition buildDefinition = resultCollection.GetCurrent<BuildDefinition>().FirstOrDefault<BuildDefinition>();
        this.TraceLeave(0, nameof (AddDefinition));
        return buildDefinition;
      }
    }

    public override async Task<BuildProcessResources> GetAuthorizedResourcesAsync(
      Guid projectId,
      int? definitionId,
      ResourceType? resourceType,
      string resourceId)
    {
      Build2Component52 build2Component52 = this;
      if (!definitionId.HasValue)
        return new BuildProcessResources();
      build2Component52.TraceEnter(0, nameof (GetAuthorizedResourcesAsync));
      build2Component52.PrepareStoredProcedure("Build.prc_GetAuthorizedResources");
      build2Component52.BindInt("@dataspaceId", build2Component52.GetDataspaceId(projectId));
      build2Component52.BindInt("@definitionId", definitionId.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component52.ExecuteReaderAsync(), build2Component52.ProcedureName, build2Component52.RequestContext))
      {
        resultCollection.AddBinder<ResourceReference>(build2Component52.GetResourceReferenceBinder());
        BuildProcessResources authorizedResourcesAsync = new BuildProcessResources();
        foreach (ResourceReference resourceReference in resultCollection.GetCurrent<ResourceReference>())
        {
          ResourceType result;
          if (resourceReference != null && (!resourceType.HasValue || ResourceTypeNames.TryParse(resourceReference.Type, out result) && resourceType.Value == result) && (string.IsNullOrEmpty(resourceId) || string.Equals(resourceId, resourceReference.GetId(), StringComparison.OrdinalIgnoreCase)))
            authorizedResourcesAsync.Add(resourceReference);
        }
        build2Component52.TraceLeave(0, nameof (GetAuthorizedResourcesAsync));
        return authorizedResourcesAsync;
      }
    }

    public override async Task<BuildProcessResources> UpdateAuthorizedResourcesAsync(
      Guid projectId,
      int? definitionId,
      Guid authorizedBy,
      BuildProcessResources resources)
    {
      Build2Component52 build2Component52 = this;
      build2Component52.TraceEnter(0, nameof (UpdateAuthorizedResourcesAsync));
      if (!definitionId.HasValue)
        return new BuildProcessResources();
      build2Component52.PrepareStoredProcedure("Build.prc_UpdateAuthorizedResources");
      build2Component52.BindInt("@dataspaceId", build2Component52.GetDataspaceId(projectId));
      build2Component52.BindInt("@definitionId", definitionId.Value);
      build2Component52.BindGuid("@authorizedBy", authorizedBy);
      build2Component52.BindAuthorizedResourcesTable("@resourcesToAuthorize", resources.GetAuthorizedResources());
      build2Component52.BindAuthorizedResourcesTable("@resourcesToUnauthorize", resources.GetUnauthorizedResources());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component52.ExecuteReaderAsync(), build2Component52.ProcedureName, build2Component52.RequestContext))
      {
        resultCollection.AddBinder<ResourceReference>(build2Component52.GetResourceReferenceBinder());
        BuildProcessResources processResources = new BuildProcessResources();
        foreach (ResourceReference resourceReference in resultCollection.GetCurrent<ResourceReference>())
        {
          if (resourceReference != null)
          {
            processResources.Add(resourceReference);
            resourceReference.DefinitionId = new int?(definitionId.Value);
          }
        }
        build2Component52.TraceLeave(0, nameof (UpdateAuthorizedResourcesAsync));
        return processResources;
      }
    }

    protected virtual ObjectBinder<ResourceReference> GetResourceReferenceBinder() => (ObjectBinder<ResourceReference>) new ResourceReferenceBinder(this.RequestContext);

    protected void BindAuthorizedResourcesTable(
      string parameterName,
      BuildProcessResources resources)
    {
      resources = resources ?? new BuildProcessResources();
      IEnumerable<KeyValuePair<int, string>> source = resources.Endpoints.Select<ServiceEndpointReference, KeyValuePair<int, string>>((System.Func<ServiceEndpointReference, KeyValuePair<int, string>>) (e => new KeyValuePair<int, string>(1, e.Id.ToString("D")))).Concat<KeyValuePair<int, string>>(resources.Files.Select<SecureFileReference, KeyValuePair<int, string>>((System.Func<SecureFileReference, KeyValuePair<int, string>>) (f => new KeyValuePair<int, string>(3, f.Id.ToString("D"))))).Concat<KeyValuePair<int, string>>(resources.Queues.Select<AgentPoolQueueReference, KeyValuePair<int, string>>((System.Func<AgentPoolQueueReference, KeyValuePair<int, string>>) (q => new KeyValuePair<int, string>(2, q.Id.ToString())))).Concat<KeyValuePair<int, string>>(resources.VariableGroups.Select<VariableGroupReference, KeyValuePair<int, string>>((System.Func<VariableGroupReference, KeyValuePair<int, string>>) (vg => new KeyValuePair<int, string>(4, vg.Id.ToString()))));
      this.BindTable(parameterName, "Build.typ_AuthorizedResourceTable", source.Select<KeyValuePair<int, string>, SqlDataRecord>(new System.Func<KeyValuePair<int, string>, SqlDataRecord>(rowBinder)));

      static SqlDataRecord rowBinder(KeyValuePair<int, string> resource)
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component52.typ_AuthorizedResourceTable);
        record.SetInt32(0, resource.Key);
        record.SetString(1, resource.Value, BindStringBehavior.Unchanged);
        return record;
      }
    }

    public override BuildData AddBuild(
      BuildData build,
      Guid requestedBy,
      Guid requestedFor,
      bool changesCalculated,
      IEnumerable<ChangeData> changeData)
    {
      this.TraceEnter(0, nameof (AddBuild));
      this.PrepareStoredProcedure("Build.prc_AddBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceId(build.ProjectId));
      this.BindNullableInt32("@queueId", build.QueueId);
      this.BindInt("@definitionId", build.Definition.Id);
      this.BindNullableInt32("@definitionVersion", build.Definition.Revision);
      this.BindString("@sourceBranch", build.SourceBranch, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@sourceVersion", build.SourceVersion, 326, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@priority", (byte) build.Priority);
      this.BindInt("@reason", (int) build.Reason);
      this.BindGuid("@requestedFor", requestedFor);
      this.BindGuid("@requestedBy", requestedBy);
      this.BindString("@parameters", build.Parameters, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@buildStatus", (byte) build.Status.Value);
      this.BindUtcDateTime2("@queueTime", build.QueueTime ?? DateTime.UtcNow);
      this.BindNullableUtcDateTime2("@startTime", build.StartTime);
      this.BindNullableUtcDateTime2("@finishTime", build.FinishTime);
      this.BindByte("@result", (byte) build.Result.Value);
      this.BindString("@buildNumberFormat", DBHelper.ServerPathToDBPath(build.BuildNumber), 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@validationIssues", JsonUtility.ToString<BuildRequestValidationResult>((IList<BuildRequestValidationResult>) build.ValidationResults), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@queueOptions", (byte) build.QueueOptions, (byte) 0);
      this.BindBoolean("@changesCalculated", changesCalculated);
      this.BindChangeDataTable("@changeData", changeData);
      this.BindString("@sourceVersionInfo", this.ToString<SourceVersionInfo>(build.SourceVersionInfo), 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@triggerInfo", this.ToString<string, string>(build.TriggerInfo), 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        BuildData buildData = resultCollection.GetCurrent<BuildData>().FirstOrDefault<BuildData>();
        resultCollection.NextResult();
        foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
        {
          if (buildData.Id == buildTagData.BuildId)
            buildData.Tags.Add(buildTagData.Tag);
        }
        resultCollection.NextResult();
        foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
        {
          int? orchestrationType = orchestrationData.Plan.OrchestrationType;
          int num = 1;
          if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
            buildData.OrchestrationPlan = orchestrationData.Plan;
          buildData.Plans.Add(orchestrationData.Plan);
        }
        this.TraceLeave(0, nameof (AddBuild));
        return buildData;
      }
    }

    public override BuildData QueueBuild(
      BuildData build,
      Guid requestedBy,
      Guid requestedFor,
      bool changesCalculated,
      IEnumerable<ChangeData> changes,
      int maxConcurrentBuildsPerBranch,
      out AgentPoolQueue queue)
    {
      this.TraceEnter(0, nameof (QueueBuild));
      this.PrepareStoredProcedure("Build.prc_QueueBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceId(build.ProjectId));
      this.BindNullableInt32("@queueId", build.QueueId);
      this.BindInt("@definitionId", build.Definition.Id);
      this.BindNullableInt32("@definitionVersion", build.Definition.Revision);
      this.BindString("@sourceBranch", build.SourceBranch, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@sourceVersion", build.SourceVersion, 326, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@priority", (byte) build.Priority);
      this.BindInt("@reason", (int) build.Reason);
      this.BindGuid("@requestedFor", requestedFor);
      this.BindGuid("@requestedBy", requestedBy);
      this.BindString("@parameters", build.Parameters, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@buildNumberFormat", DBHelper.ServerPathToDBPath(build.BuildNumber), 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindUtcDateTime2("@queueTime", build.QueueTime ?? DateTime.UtcNow);
      this.BindByte("@queueOptions", (byte) build.QueueOptions, (byte) 0);
      this.BindBoolean("@changesCalculated", changesCalculated);
      this.BindChangeDataTable("@changeData", changes);
      this.BindInt("@maxConcurrentBuildsPerBranch", maxConcurrentBuildsPerBranch);
      this.BindString("@sourceVersionInfo", this.ToString<SourceVersionInfo>(build.SourceVersionInfo), 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@triggerInfo", this.ToString<string, string>(build.TriggerInfo), 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableInt32("@triggeredByDataspaceId", build.TriggeredByBuild != null ? new int?(this.GetDataspaceId(build.TriggeredByBuild.ProjectId)) : new int?());
      this.BindNullableInt32("@triggeredByDefinitionId", build.TriggeredByBuild != null ? new int?(build.TriggeredByBuild.DefinitionId) : new int?());
      this.BindNullableInt32("@triggeredByDefVersion", build.TriggeredByBuild != null ? build.TriggeredByBuild.DefinitionVersion : new int?());
      this.BindNullableInt32("@triggeredByBuildId", build.TriggeredByBuild != null ? new int?(build.TriggeredByBuild.BuildId) : new int?());
      this.BindString("@validationIssues", JsonUtility.ToString<BuildRequestValidationResult>((IList<BuildRequestValidationResult>) build.ValidationResults.Where<BuildRequestValidationResult>((System.Func<BuildRequestValidationResult, bool>) (vr => vr.Result != 0)).ToList<BuildRequestValidationResult>()), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      queue = (AgentPoolQueue) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        BuildData buildData = resultCollection.GetCurrent<BuildData>().FirstOrDefault<BuildData>();
        resultCollection.NextResult();
        foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
          buildData.Tags.Add(buildTagData.Tag);
        resultCollection.NextResult();
        foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
        {
          int? orchestrationType = orchestrationData.Plan.OrchestrationType;
          int num = 1;
          if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
            buildData.OrchestrationPlan = orchestrationData.Plan;
          buildData.Plans.Add(orchestrationData.Plan);
        }
        this.TraceLeave(0, nameof (QueueBuild));
        return buildData;
      }
    }

    protected override SqlParameter BindBuildUpdateTable(
      string parameterName,
      IEnumerable<BuildData> builds)
    {
      builds = builds ?? Enumerable.Empty<BuildData>();
      System.Func<BuildData, SqlDataRecord> selector = (System.Func<BuildData, SqlDataRecord>) (build =>
      {
        SqlDataRecord record1 = new SqlDataRecord(Build2Component52.typ_BuildUpdateTable5);
        record1.SetInt32(0, build.Id);
        record1.SetString(1, DBHelper.ServerPathToDBPath(build.BuildNumber), BindStringBehavior.Unchanged);
        SqlDataRecord record2 = record1;
        DateTime? nullable1 = build.StartTime;
        DateTime dateTime1 = nullable1 ?? DateTime.MinValue;
        record2.SetNullableDateTime(2, dateTime1);
        SqlDataRecord record3 = record1;
        nullable1 = build.FinishTime;
        DateTime dateTime2 = nullable1 ?? DateTime.MinValue;
        record3.SetNullableDateTime(3, dateTime2);
        record1.SetString(4, build.SourceBranch, BindStringBehavior.Unchanged);
        record1.SetString(5, build.SourceVersion, BindStringBehavior.Unchanged);
        SqlDataRecord record4 = record1;
        BuildStatus? status = build.Status;
        byte? nullable2 = status.HasValue ? new byte?((byte) status.GetValueOrDefault()) : new byte?();
        record4.SetNullableByte(6, nullable2);
        SqlDataRecord record5 = record1;
        BuildResult? result = build.Result;
        byte? nullable3 = result.HasValue ? new byte?((byte) result.GetValueOrDefault()) : new byte?();
        record5.SetNullableByte(7, nullable3);
        bool? nullable4 = build.LegacyInputKeepForever;
        if (nullable4.HasValue)
        {
          SqlDataRecord sqlDataRecord = record1;
          nullable4 = build.LegacyInputKeepForever;
          int num = nullable4.Value ? 1 : 0;
          sqlDataRecord.SetBoolean(8, num != 0);
        }
        nullable4 = build.LegacyInputRetainedByRelease;
        if (nullable4.HasValue)
        {
          SqlDataRecord sqlDataRecord = record1;
          nullable4 = build.LegacyInputRetainedByRelease;
          int num = nullable4.Value ? 1 : 0;
          sqlDataRecord.SetBoolean(9, num != 0);
        }
        if (build.ValidationResults.Count > 0)
          record1.SetString(10, JsonUtility.ToString<BuildRequestValidationResult>((IList<BuildRequestValidationResult>) build.ValidationResults), BindStringBehavior.Unchanged);
        string str = this.ToString<SourceVersionInfo>(build.SourceVersionInfo);
        if (!string.IsNullOrEmpty(str))
          record1.SetString(11, str, BindStringBehavior.Unchanged);
        return record1;
      });
      return this.BindTable(parameterName, "Build.typ_BuildUpdateTable5", builds.Select<BuildData, SqlDataRecord>(selector));
    }

    protected override sealed ObjectBinder<BuildData> GetBuildBinder() => this.GetBuildDataBinder();

    protected override ObjectBinder<BuildData> GetBuildDataBinder() => (ObjectBinder<BuildData>) new BuildDataBinder15(this.RequestContext, (BuildSqlComponentBase) this);
  }
}
