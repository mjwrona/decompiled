// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component37
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
  internal class Build2Component37 : Build2Component36
  {
    public override IEnumerable<BuildData> GetBuildsByIds(
      IEnumerable<int> buildIds,
      bool includeDeleted)
    {
      using (this.TraceScope(method: nameof (GetBuildsByIds)))
      {
        this.PrepareStoredProcedure("Build.prc_GetBuildsByIds");
        this.BindInt32Table("@buildIdTable", buildIds);
        this.BindBoolean("@includeDeleted", includeDeleted);
        Dictionary<int, BuildData> dictionary;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          dictionary = resultCollection.GetCurrent<BuildData>().Items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
          resultCollection.NextResult();
          foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
          {
            BuildData buildData = (BuildData) null;
            if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
              buildData.Tags.Add(buildTagData.Tag);
          }
          resultCollection.NextResult();
          foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
          {
            BuildData buildData;
            if (dictionary.TryGetValue(orchestrationData.BuildId, out buildData))
            {
              int? orchestrationType = orchestrationData.Plan.OrchestrationType;
              int num = 1;
              if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
                buildData.OrchestrationPlan = orchestrationData.Plan;
              buildData.Plans.Add(orchestrationData.Plan);
            }
          }
        }
        return (IEnumerable<BuildData>) dictionary.Values;
      }
    }

    public override async Task<IEnumerable<BuildData>> GetBuildsByIdsAsync(
      IEnumerable<int> buildIds,
      bool includeDeleted)
    {
      Build2Component37 component = this;
      IEnumerable<BuildData> values;
      using (component.TraceScope(method: nameof (GetBuildsByIdsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetBuildsByIds");
        component.BindInt32Table("@buildIdTable", buildIds);
        component.BindBoolean("@includeDeleted", includeDeleted);
        Dictionary<int, BuildData> dictionary;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          dictionary = resultCollection.GetCurrent<BuildData>().Items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
          resultCollection.NextResult();
          foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
          {
            BuildData buildData;
            if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
              buildData.Tags.Add(buildTagData.Tag);
          }
          resultCollection.NextResult();
          foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
          {
            BuildData buildData;
            if (dictionary.TryGetValue(orchestrationData.BuildId, out buildData))
            {
              int? orchestrationType = orchestrationData.Plan.OrchestrationType;
              int num = 1;
              if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
                buildData.OrchestrationPlan = orchestrationData.Plan;
              buildData.Plans.Add(orchestrationData.Plan);
            }
          }
        }
        values = (IEnumerable<BuildData>) dictionary.Values;
      }
      return values;
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
      Build2Component37 component = this;
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
        if (includeLatestBuilds)
          component.BindLatestBuilds(rc, dictionary);
        component.TraceLeave(0, nameof (GetDefinitionsByIdsAsync));
        definitionsByIdsAsync = list;
      }
      return definitionsByIdsAsync;
    }

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
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        rc.AddBinder<BuildData>(this.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> source = rc.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = source.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
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
        if (includeLatestBuilds)
          this.BindLatestBuilds(rc, dictionary);
        if (processType.HasValue)
          source = source.Where<BuildDefinition>((System.Func<BuildDefinition, bool>) (x => x.Process.Type == processType.Value)).ToList<BuildDefinition>();
        this.TraceLeave(0, nameof (GetDefinitions));
        return source;
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
      this.BindStringTable("@changeDescriptors", changeData != null ? changeData.Select<ChangeData, string>((System.Func<ChangeData, string>) (c => c.Descriptor)) : (IEnumerable<string>) null);
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
      this.BindStringTable("@changeDescriptors", changes != null ? changes.Select<ChangeData, string>((System.Func<ChangeData, string>) (c => c.Descriptor)) : (IEnumerable<string>) null);
      this.BindInt("@maxConcurrentBuildsPerBranch", maxConcurrentBuildsPerBranch);
      this.BindString("@triggerInfo", this.ToString<string, string>(build.TriggerInfo), 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
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

    public override IEnumerable<BuildData> GetBuilds(
      Guid projectId,
      IEnumerable<int> definitionIds,
      IEnumerable<int> queueIds,
      string buildNumber,
      DateTime? minFinishTime,
      DateTime? maxFinishTime,
      IEnumerable<Guid> requestedForIds,
      BuildReason? reasonFilter,
      BuildStatus? statusFilter,
      BuildResult? resultFilter,
      IEnumerable<string> tagFilters,
      int maxBuilds,
      QueryDeletedOption queryDeletedOption,
      BuildQueryOrder queryOrder,
      IList<int> excludedDefinitionIds,
      string repositoryId,
      string repositoryType,
      string branchName,
      int? maxBuildsPerDefinition)
    {
      this.TraceEnter(0, nameof (GetBuilds));
      this.PrepareStoredProcedure("Build.prc_GetBuilds");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindUniqueInt32Table("@definitionIdTable", definitionIds);
      this.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
      this.BindUniqueInt32Table("@queueIdTable", queueIds);
      this.BindString("@buildNumber", DBHelper.ServerPathToDBPath(buildNumber), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindNullableUtcDateTime2("@minFinishTime", minFinishTime);
      this.BindNullableUtcDateTime2("@maxFinishTime", maxFinishTime);
      this.BindGuidTable("@requestedForIdTable", requestedForIds);
      BuildReason? nullable1 = reasonFilter;
      this.BindNullableInt32("@reasonFilter", nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?());
      BuildStatus? nullable2 = statusFilter;
      this.BindNullableByte("@statusFilter", nullable2.HasValue ? new byte?((byte) nullable2.GetValueOrDefault()) : new byte?());
      BuildResult? nullable3 = resultFilter;
      this.BindNullableByte("@resultFilter", nullable3.HasValue ? new byte?((byte) nullable3.GetValueOrDefault()) : new byte?());
      this.BindNullableInt32("@maxBuilds", new int?(maxBuilds));
      this.BindInt("@queryOrder", (int) queryOrder);
      this.BindInt("@queryDeletedOption", (int) queryDeletedOption);
      this.BindStringTable("@tagFilters", tagFilters);
      this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@branchName", branchName, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableInt32("@maxBuildsPerDefinition", maxBuildsPerDefinition);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
        Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
        resultCollection.NextResult();
        foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
        {
          BuildData buildData;
          if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
            buildData.Tags.Add(buildTagData.Tag);
        }
        resultCollection.NextResult();
        foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
        {
          BuildData buildData;
          if (dictionary.TryGetValue(orchestrationData.BuildId, out buildData))
          {
            int? orchestrationType = orchestrationData.Plan.OrchestrationType;
            int num = 1;
            if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
              buildData.OrchestrationPlan = orchestrationData.Plan;
            buildData.Plans.Add(orchestrationData.Plan);
          }
        }
        this.TraceLeave(0, nameof (GetBuilds));
        return (IEnumerable<BuildData>) items;
      }
    }

    protected override SqlParameter BindBuildUpdateTable(
      string parameterName,
      IEnumerable<BuildData> builds)
    {
      builds = builds ?? Enumerable.Empty<BuildData>();
      System.Func<BuildData, SqlDataRecord> selector = (System.Func<BuildData, SqlDataRecord>) (build =>
      {
        SqlDataRecord record1 = new SqlDataRecord(Build2Component.typ_BuildUpdateTable3);
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
        return record1;
      });
      return this.BindTable(parameterName, "Build.typ_BuildUpdateTable3", builds.Select<BuildData, SqlDataRecord>(selector));
    }
  }
}
