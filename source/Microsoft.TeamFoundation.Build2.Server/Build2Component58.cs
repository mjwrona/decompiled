// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component58
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component58 : Build2Component57
  {
    protected static readonly SqlMetaData[] typ_ScheduleAddTable = new SqlMetaData[7]
    {
      new SqlMetaData("ScheduleName", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ScheduleType", SqlDbType.Int),
      new SqlMetaData("BranchDetails", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ScheduleDetails", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ScheduleJobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("scheduleOnlyWithChanges", SqlDbType.Bit),
      new SqlMetaData("Batch", SqlDbType.Bit)
    };
    private const int branchDetails_MaxLength = 2048;
    private const int scheduleDetails_MaxLength = 2048;

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
      this.BindScheduleAddTable("@scheduleTable", (IEnumerable<Schedule2>) this.ExtractDesignerSchedules(definition));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        BuildDefinition buildDefinition = resultCollection.GetCurrent<BuildDefinition>().FirstOrDefault<BuildDefinition>();
        this.TraceLeave(0, nameof (AddDefinition));
        return buildDefinition;
      }
    }

    public override BuildDefinition UpdateDefinition(
      BuildDefinition definition,
      Guid requestedBy,
      string originalSecurityToken,
      string newSecurityToken)
    {
      this.TraceEnter(0, nameof (UpdateDefinition));
      this.PrepareStoredProcedure("Build.prc_UpdateDefinition");
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
      this.BindInt("@definitionId", definition.Id);
      this.BindInt("@definitionVersion", definition.Revision.Value);
      this.BindString("@definitionName", DBHelper.ServerPathToDBPath(definition.Name), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindNullableInt32("@queueId", definition.Queue != null ? new int?(definition.Queue.Id) : new int?());
      this.BindByte("@queueStatus", (byte) definition.QueueStatus);
      this.BindString("@repositoryType", parameterValue1, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repositoryIdentifier", parameterValue2, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@defaultBranch", parameterValue3, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@triggerTypes", (int) definition.Triggers.Aggregate<BuildTrigger, DefinitionTriggerType>(DefinitionTriggerType.None, (Func<DefinitionTriggerType, BuildTrigger, DefinitionTriggerType>) ((x, y) => x | y.TriggerType)));
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
      this.BindString("@originalSecurityToken", originalSecurityToken, 435, false, SqlDbType.NVarChar);
      this.BindString("@newSecurityToken", newSecurityToken, 435, false, SqlDbType.NVarChar);
      this.BindString("@processParameters", JsonUtility.ToString((object) definition.ProcessParameters), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@buildCompletionTriggersString", JsonUtility.ToString<BuildCompletionTrigger>((IList<BuildCompletionTrigger>) definition.BuildCompletionTriggers), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBuildCompletionTriggerTable("@buildCompletionTriggers", (IEnumerable<BuildCompletionTrigger>) definition.BuildCompletionTriggers);
      this.BindScheduleAddTable("@scheduleTable", (IEnumerable<Schedule2>) this.ExtractDesignerSchedules(definition));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        resultCollection.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        BuildDefinition buildDefinition = resultCollection.GetCurrent<BuildDefinition>().FirstOrDefault<BuildDefinition>();
        resultCollection.NextResult();
        foreach (DefinitionTagData definitionTagData in resultCollection.GetCurrent<DefinitionTagData>())
          buildDefinition.Tags.Add(definitionTagData.Tag);
        this.TraceLeave(0, nameof (UpdateDefinition));
        return buildDefinition;
      }
    }

    public override BuildDefinition RestoreDefinition(
      Guid projectId,
      int definitionId,
      Guid authorId,
      string comment = null)
    {
      this.TraceEnter(0, nameof (RestoreDefinition));
      this.PrepareStoredProcedure("Build.prc_RestoreDefinition");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.BindGuid("@requestedBy", authorId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        resultCollection.AddBinder<Schedule2>(this.GetBuildSchedule2Binder());
        BuildDefinition buildDefinition = resultCollection.GetCurrent<BuildDefinition>().SingleOrDefault<BuildDefinition>();
        if (buildDefinition != null)
        {
          resultCollection.NextResult();
          buildDefinition.Triggers.Remove(buildDefinition.Triggers.FirstOrDefault<BuildTrigger>((System.Func<BuildTrigger, bool>) (t => t.TriggerType == DefinitionTriggerType.Schedule)));
          List<Schedule> scheduleList = new List<Schedule>();
          foreach (Schedule2 schedule2 in resultCollection.GetCurrent<Schedule2>())
            scheduleList.Add(new Schedule()
            {
              BranchFilters = schedule2.BranchDetails,
              TimeZoneId = schedule2.ScheduleDetails.TimeZoneId,
              StartHours = schedule2.ScheduleDetails.StartHours,
              StartMinutes = schedule2.ScheduleDetails.StartMinutes,
              DaysToBuild = schedule2.ScheduleDetails.DaysToBuild,
              ScheduleJobId = schedule2.ScheduleJobId,
              ScheduleOnlyWithChanges = schedule2.ScheduleOnlyWithChanges
            });
          if (scheduleList.Count > 0)
            buildDefinition.Triggers.Add((BuildTrigger) new ScheduleTrigger()
            {
              Schedules = scheduleList
            });
        }
        this.TraceLeave(0, nameof (RestoreDefinition));
        return buildDefinition;
      }
    }

    public override async Task<BuildDefinition> GetDefinitionAsync(
      Guid projectId,
      int definitionId,
      int? definitionVersion,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false)
    {
      Build2Component58 build2Component58 = this;
      build2Component58.TraceEnter(0, nameof (GetDefinitionAsync));
      build2Component58.PrepareStoredProcedure("Build.prc_GetDefinition");
      build2Component58.BindInt("@dataspaceId", build2Component58.GetDataspaceId(projectId));
      build2Component58.BindInt("@definitionId", definitionId);
      build2Component58.BindNullableInt32("@definitionVersion", definitionVersion);
      build2Component58.BindBoolean("@includeDeleted", includeDeleted);
      build2Component58.BindNullableUtcDateTime2("@minMetricsTime", minMetricsTime);
      build2Component58.BindBoolean("@includeLatestBuilds", includeLatestBuilds);
      BuildDefinition definitionAsync;
      using (ResultCollection rc = new ResultCollection((IDataReader) await build2Component58.ExecuteReaderAsync(), build2Component58.ProcedureName, build2Component58.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(build2Component58.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(build2Component58.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(build2Component58.GetDefinitionTagDataBinder());
        if (!definitionVersion.HasValue)
          rc.AddBinder<Schedule2>(build2Component58.GetBuildSchedule2Binder());
        rc.AddBinder<BuildData>(build2Component58.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(build2Component58.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(build2Component58.GetBuildOrchestrationDataBinder());
        BuildDefinition definition = rc.GetCurrent<BuildDefinition>().SingleOrDefault<BuildDefinition>();
        if (definition != null)
        {
          rc.NextResult();
          foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
            definition.Metrics.Add(definitionMetric.Metric);
          rc.NextResult();
          foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>())
            definition.Tags.Add(definitionTagData.Tag);
          if (!definitionVersion.HasValue)
            build2Component58.UpdateScheduleTrigger(rc, definition);
          if (includeLatestBuilds)
            build2Component58.BindLatestBuilds(rc, definition);
        }
        build2Component58.TraceLeave(0, nameof (GetDefinitionAsync));
        definitionAsync = definition;
      }
      return definitionAsync;
    }

    public override List<BuildDefinition> GetDefinitionsByIds(
      Guid projectId,
      List<int> definitionIds,
      bool includeDeleted,
      DateTime? minMetricsTime,
      bool includeLatestBuilds,
      bool includeDrafts = false)
    {
      this.TraceEnter(0, nameof (GetDefinitionsByIds));
      this.PrepareStoredProcedure("Build.prc_GetDefinitionsByIds");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindNullableUtcDateTime2("@minMetricsTime", minMetricsTime);
      this.BindBoolean("@includeLatestBuilds", includeLatestBuilds);
      this.BindBoolean("@includeDrafts", includeDrafts);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        rc.AddBinder<Schedule2>(this.GetBuildSchedule2Binder());
        rc.AddBinder<BuildData>(this.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> list = rc.GetCurrent<BuildDefinition>().Items.Distinct<BuildDefinition>((IEqualityComparer<BuildDefinition>) BuildDefinitionEqualityComparer.Default).ToList<BuildDefinition>();
        Dictionary<int, BuildDefinition> dictionary = list.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
        if (includeDrafts)
        {
          foreach (BuildDefinition buildDefinition in list.Where<BuildDefinition>((System.Func<BuildDefinition, bool>) (bd => bd.ParentDefinition != null)))
          {
            if (dictionary.ContainsKey(buildDefinition.ParentDefinition.Id))
              dictionary[buildDefinition.ParentDefinition.Id].Drafts.Add(buildDefinition);
          }
        }
        rc.NextResult();
        foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionMetric.DefinitionId, out buildDefinition))
            buildDefinition.Metrics.Add(definitionMetric.Metric);
        }
        rc.NextResult();
        foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>().Items.Distinct<DefinitionTagData>((IEqualityComparer<DefinitionTagData>) DefinitionTagDataEqualityComparer.Default).ToList<DefinitionTagData>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionTagData.DefinitionId, out buildDefinition))
            buildDefinition.Tags.Add(definitionTagData.Tag);
        }
        this.UpdateScheduleTriggers(rc, dictionary);
        if (includeLatestBuilds)
          this.BindLatestBuilds(rc, dictionary);
        this.TraceLeave(0, nameof (GetDefinitionsByIds));
        return list;
      }
    }

    public override async Task<List<BuildDefinition>> GetDefinitionsByIdsAsync(
      Guid projectId,
      List<int> definitionIds,
      bool includeDeleted,
      DateTime? minMetricsTime,
      bool includeLatestBuilds,
      bool includeDrafts = false)
    {
      Build2Component58 component = this;
      component.TraceEnter(0, nameof (GetDefinitionsByIdsAsync));
      component.PrepareStoredProcedure("Build.prc_GetDefinitionsByIds");
      component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
      component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
      component.BindBoolean("@includeDeleted", includeDeleted);
      component.BindNullableUtcDateTime2("@minMetricsTime", minMetricsTime);
      component.BindBoolean("@includeLatestBuilds", includeLatestBuilds);
      component.BindBoolean("@includeDrafts", includeDrafts);
      List<BuildDefinition> definitionsByIdsAsync;
      using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(component.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(component.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(component.GetDefinitionTagDataBinder());
        rc.AddBinder<Schedule2>(component.GetBuildSchedule2Binder());
        rc.AddBinder<BuildData>(component.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(component.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> list = rc.GetCurrent<BuildDefinition>().Items.Distinct<BuildDefinition>((IEqualityComparer<BuildDefinition>) BuildDefinitionEqualityComparer.Default).ToList<BuildDefinition>();
        Dictionary<int, BuildDefinition> dictionary = list.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
        if (includeDrafts)
        {
          foreach (BuildDefinition buildDefinition in list.Where<BuildDefinition>((System.Func<BuildDefinition, bool>) (bd => bd.ParentDefinition != null)))
          {
            if (dictionary.ContainsKey(buildDefinition.ParentDefinition.Id))
              dictionary[buildDefinition.ParentDefinition.Id].Drafts.Add(buildDefinition);
          }
        }
        rc.NextResult();
        foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionMetric.DefinitionId, out buildDefinition))
            buildDefinition.Metrics.Add(definitionMetric.Metric);
        }
        rc.NextResult();
        foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>().Items.Distinct<DefinitionTagData>((IEqualityComparer<DefinitionTagData>) DefinitionTagDataEqualityComparer.Default).ToList<DefinitionTagData>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionTagData.DefinitionId, out buildDefinition))
            buildDefinition.Tags.Add(definitionTagData.Tag);
        }
        component.UpdateScheduleTriggers(rc, dictionary);
        if (includeLatestBuilds)
          component.BindLatestBuilds(rc, dictionary);
        component.TraceLeave(0, nameof (GetDefinitionsByIdsAsync));
        definitionsByIdsAsync = list;
      }
      return definitionsByIdsAsync;
    }

    public override List<BuildDefinition> GetDefinitionsWithTriggers(
      HashSet<int> dataspaceIds,
      string repositoryId,
      string repositoryType,
      int triggerFilter,
      int maxDefinitions)
    {
      this.TraceEnter(0, nameof (GetDefinitionsWithTriggers));
      this.PrepareStoredProcedure("Build.prc_GetDefinitionsWithTriggers");
      this.BindUniqueInt32Table("@dataspaceIdTable", (IEnumerable<int>) dataspaceIds);
      this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@triggerFilter", triggerFilter);
      this.BindInt("@maxDefinitions", maxDefinitions);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        rc.AddBinder<Schedule2>(this.GetBuildSchedule2Binder());
        rc.AddBinder<BuildData>(this.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
        rc.NextResult();
        foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionMetric.DefinitionId, out buildDefinition))
            buildDefinition.Metrics.Add(definitionMetric.Metric);
        }
        rc.NextResult();
        foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionTagData.DefinitionId, out buildDefinition))
            buildDefinition.Tags.Add(definitionTagData.Tag);
        }
        this.UpdateScheduleTriggers(rc, dictionary);
        rc.NextResult();
        this.TraceLeave(0, nameof (GetDefinitionsWithTriggers));
        return items;
      }
    }

    public override List<BuildDefinition> GetDefinitions(
      Guid projectId,
      string name,
      DefinitionTriggerType triggerFilter,
      string repositoryId,
      string repositoryType,
      DefinitionQueryOrder queryOrder,
      int maxDefinitions,
      DateTime? minLastModifiedTime,
      DateTime? maxLastModifiedTime,
      string lastDefinitionName,
      DateTime? minMetricsTime,
      string path,
      DateTime? builtAfter,
      DateTime? notBuiltAfter,
      DefinitionQueryOptions options,
      IEnumerable<string> tagFilters,
      bool includeLatestBuilds,
      Guid? taskIdFilter = null,
      int? processType = null)
    {
      this.TraceEnter(0, nameof (GetDefinitions));
      this.PrepareStoredProcedure("Build.prc_GetDefinitions");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@definitionName", DBHelper.ServerPathToDBPath(name), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@triggerFilter", (int) triggerFilter);
      this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@queryOrder", (int) queryOrder);
      this.BindInt("@maxDefinitions", maxDefinitions);
      this.BindNullableUtcDateTime2("@minLastModifiedTime", minLastModifiedTime);
      this.BindNullableUtcDateTime2("@maxLastModifiedTime", maxLastModifiedTime);
      this.BindString("@lastDefinitionName", DBHelper.ServerPathToDBPath(lastDefinitionName), 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableUtcDateTime2("@minMetricsTime", minMetricsTime);
      this.BindString("@path", DBHelper.UserToDBPath(path), 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableUtcDateTime2("@builtAfter", builtAfter);
      this.BindNullableUtcDateTime2("@notBuiltAfter", notBuiltAfter);
      this.BindInt("@queryOptions", (int) options);
      this.BindStringTable("@tagFilters", tagFilters);
      this.BindBoolean("@includeLatestBuilds", includeLatestBuilds);
      this.BindNullableGuid("@taskIdFilter", taskIdFilter);
      this.BindNullableInt32("@processType", processType);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        rc.AddBinder<Schedule2>(this.GetBuildSchedule2Binder());
        rc.AddBinder<BuildData>(this.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
        rc.NextResult();
        foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionMetric.DefinitionId, out buildDefinition))
            buildDefinition.Metrics.Add(definitionMetric.Metric);
        }
        rc.NextResult();
        foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionTagData.DefinitionId, out buildDefinition))
            buildDefinition.Tags.Add(definitionTagData.Tag);
        }
        this.UpdateScheduleTriggers(rc, dictionary);
        if (includeLatestBuilds)
          this.BindLatestBuilds(rc, dictionary);
        this.TraceLeave(0, nameof (GetDefinitions));
        return items;
      }
    }

    public override List<BuildDefinition> GetCIDefinitions(
      HashSet<int> dataspaceIds,
      string repositoryId,
      string repositoryType,
      int maxDefinitions)
    {
      this.TraceEnter(0, nameof (GetCIDefinitions));
      this.PrepareStoredProcedure("Build.prc_GetCIDefinitions");
      this.BindUniqueInt32Table("@dataspaceIdTable", (IEnumerable<int>) dataspaceIds);
      this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@maxDefinitions", maxDefinitions);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        rc.AddBinder<Schedule2>(this.GetBuildSchedule2Binder());
        rc.AddBinder<BuildData>(this.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
        rc.NextResult();
        foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionMetric.DefinitionId, out buildDefinition))
            buildDefinition.Metrics.Add(definitionMetric.Metric);
        }
        rc.NextResult();
        foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionTagData.DefinitionId, out buildDefinition))
            buildDefinition.Tags.Add(definitionTagData.Tag);
        }
        this.UpdateScheduleTriggers(rc, dictionary);
        rc.NextResult();
        this.TraceLeave(0, nameof (GetCIDefinitions));
        return items;
      }
    }

    protected List<Schedule2> ExtractDesignerSchedules(BuildDefinition definition)
    {
      List<Schedule2> designerSchedules = new List<Schedule2>();
      if (definition.Triggers.FirstOrDefault<BuildTrigger>((System.Func<BuildTrigger, bool>) (t => t.TriggerType == DefinitionTriggerType.Schedule)) is ScheduleTrigger scheduleTrigger)
      {
        foreach (Schedule schedule in scheduleTrigger.Schedules)
        {
          Schedule2 schedule2 = new Schedule2(0, (string) null, ScheduleType.Designer, schedule.BranchFilters, new DesignerScheduleDetails(schedule.TimeZoneId, schedule.StartHours, schedule.StartMinutes, schedule.DaysToBuild), schedule.ScheduleJobId, schedule.ScheduleOnlyWithChanges, false, 0);
          designerSchedules.Add(schedule2);
        }
      }
      return designerSchedules;
    }

    protected virtual void UpdateScheduleTrigger(ResultCollection rc, BuildDefinition definition)
    {
      rc.NextResult();
      definition.Triggers.Remove(definition.Triggers.FirstOrDefault<BuildTrigger>((System.Func<BuildTrigger, bool>) (t => t.TriggerType == DefinitionTriggerType.Schedule)));
      List<Schedule> scheduleList = new List<Schedule>();
      foreach (Schedule2 schedule2 in rc.GetCurrent<Schedule2>())
        scheduleList.Add(new Schedule()
        {
          BranchFilters = schedule2.BranchDetails,
          TimeZoneId = schedule2.ScheduleDetails.TimeZoneId,
          StartHours = schedule2.ScheduleDetails.StartHours,
          StartMinutes = schedule2.ScheduleDetails.StartMinutes,
          DaysToBuild = schedule2.ScheduleDetails.DaysToBuild,
          ScheduleJobId = schedule2.ScheduleJobId,
          ScheduleOnlyWithChanges = schedule2.ScheduleOnlyWithChanges
        });
      if (scheduleList.Count <= 0)
        return;
      definition.Triggers.Add((BuildTrigger) new ScheduleTrigger()
      {
        Schedules = scheduleList
      });
    }

    protected virtual void UpdateScheduleTriggers(
      ResultCollection rc,
      Dictionary<int, BuildDefinition> definitions)
    {
      rc.NextResult();
      List<Schedule2> list = rc.GetCurrent<Schedule2>().Items.Distinct<Schedule2>().ToList<Schedule2>();
      List<Schedule> scheduleList = new List<Schedule>();
      foreach (KeyValuePair<int, BuildDefinition> keyValuePair in definitions.ToArray<KeyValuePair<int, BuildDefinition>>())
      {
        KeyValuePair<int, BuildDefinition> buildDefinition = keyValuePair;
        if (buildDefinition.Value.Triggers.Exists((Predicate<BuildTrigger>) (t => t.TriggerType == DefinitionTriggerType.Schedule)))
        {
          BuildTrigger buildTrigger = buildDefinition.Value.Triggers.Find((Predicate<BuildTrigger>) (t => t.TriggerType == DefinitionTriggerType.Schedule));
          buildDefinition.Value.Triggers.Remove(buildTrigger);
        }
        scheduleList.Clear();
        foreach (Schedule2 schedule2 in list.Where<Schedule2>((System.Func<Schedule2, bool>) (ns => ns.DefinitionId == buildDefinition.Value.Id)))
          scheduleList.Add(new Schedule()
          {
            BranchFilters = schedule2.BranchDetails,
            TimeZoneId = schedule2.ScheduleDetails.TimeZoneId,
            StartHours = schedule2.ScheduleDetails.StartHours,
            StartMinutes = schedule2.ScheduleDetails.StartMinutes,
            DaysToBuild = schedule2.ScheduleDetails.DaysToBuild,
            ScheduleJobId = schedule2.ScheduleJobId,
            ScheduleOnlyWithChanges = schedule2.ScheduleOnlyWithChanges
          });
        if (scheduleList.Count > 0)
          buildDefinition.Value.Triggers.Add((BuildTrigger) new ScheduleTrigger()
          {
            Schedules = scheduleList
          });
      }
    }

    protected virtual ObjectBinder<Schedule2> GetBuildSchedule2Binder() => (ObjectBinder<Schedule2>) new BuildSchedule2Binder(this.RequestContext, (BuildSqlComponentBase) this);

    protected void BindScheduleAddTable(string parameterName, IEnumerable<Schedule2> schedules)
    {
      schedules = schedules ?? Enumerable.Empty<Schedule2>();
      System.Func<Schedule2, SqlDataRecord> selector = (System.Func<Schedule2, SqlDataRecord>) (sch2 =>
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component58.typ_ScheduleAddTable);
        string str1 = JsonUtility.ToString<string>((IList<string>) sch2.BranchDetails);
        string str2 = JsonUtility.ToString((object) sch2.ScheduleDetails);
        this.ValidateMaxLengthForScheduleColumn(str1.Length, 2048, "BranchDetails");
        this.ValidateMaxLengthForScheduleColumn(str2.Length, 2048, "ScheduleDetails");
        record.SetString(0, sch2.ScheduleName, BindStringBehavior.Unchanged);
        record.SetInt32(1, (int) sch2.ScheduleType);
        record.SetString(2, str1, BindStringBehavior.Unchanged);
        record.SetString(3, str2, BindStringBehavior.Unchanged);
        record.SetGuid(4, sch2.ScheduleJobId);
        record.SetBoolean(5, sch2.ScheduleOnlyWithChanges);
        record.SetBoolean(6, sch2.Batch);
        return record;
      });
      this.BindTable(parameterName, "Build.typ_ScheduleAddTable", schedules.Select<Schedule2, SqlDataRecord>(selector));
    }

    private void ValidateMaxLengthForScheduleColumn(
      int actualLength,
      int expectedLength,
      string fieldName)
    {
      if (actualLength > expectedLength)
        throw new ArgumentException("'" + fieldName + "' for schedule exceeds maximum length and will be truncated. Consider removing filters/details that are no longer needed.");
    }
  }
}
