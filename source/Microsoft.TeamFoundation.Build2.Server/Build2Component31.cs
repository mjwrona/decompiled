// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component31
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component31 : Build2Component
  {
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
      this.BindString("@buildNumberFormat", build.BuildNumber, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@validationIssues", JsonUtility.ToString<BuildRequestValidationResult>((IList<BuildRequestValidationResult>) build.ValidationResults), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@queueOptions", (byte) build.QueueOptions, (byte) 0);
      this.BindBoolean("@changesCalculated", changesCalculated);
      this.BindStringTable("@changeDescriptors", changeData != null ? changeData.Select<ChangeData, string>((System.Func<ChangeData, string>) (c => c.Descriptor)) : (IEnumerable<string>) null);
      this.BindString("@triggerInfo", this.ToString<string, string>(build.TriggerInfo), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
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

    public override DeleteBuildsResult DeleteBuilds(
      Guid projectId,
      IEnumerable<int> buildIds,
      Guid requestedBy,
      bool setBuildRecordAsDeleted = true,
      string deletedReason = null)
    {
      this.TraceEnter(0, nameof (DeleteBuilds));
      this.PrepareStoredProcedure("Build.prc_DeleteBuilds", 3600);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindBoolean("@setDeleted", setBuildRecordAsDeleted);
      this.BindInt32Table("@buildIdTable", buildIds);
      this.BindGuid("@requestedBy", requestedBy);
      this.BindString("@deletedReason", deletedReason, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      DeleteBuildsResult deleteBuildsResult = new DeleteBuildsResult();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildArtifact>(this.GetBuildArtifactBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        Dictionary<int, BuildData> dictionary = resultCollection.GetCurrent<BuildData>().Items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
        resultCollection.NextResult();
        foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
        {
          BuildData buildData = (BuildData) null;
          if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
            buildData.Tags.Add(buildTagData.Tag);
        }
        deleteBuildsResult.DeletedBuilds = (IList<BuildData>) dictionary.Values.ToList<BuildData>();
        resultCollection.NextResult();
        deleteBuildsResult.DeletedArtifacts = this.GetUniqueArtifacts((IList<BuildArtifact>) resultCollection.GetCurrent<BuildArtifact>().Items);
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
      this.TraceLeave(0, nameof (DeleteBuilds));
      return deleteBuildsResult;
    }

    public override async Task<DeleteBuildsResult> DeleteBuildsAsync(
      Guid projectId,
      IEnumerable<int> buildIds,
      Guid requestedBy,
      bool setBuildRecordAsDeleted = true,
      string deletedReason = null)
    {
      Build2Component31 component = this;
      component.TraceEnter(0, nameof (DeleteBuildsAsync));
      component.PrepareStoredProcedure("Build.prc_DeleteBuilds", 3600);
      component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
      component.BindBoolean("@setDeleted", setBuildRecordAsDeleted);
      component.BindInt32Table("@buildIdTable", buildIds);
      component.BindGuid("@requestedBy", requestedBy);
      component.BindString("@deletedReason", deletedReason, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      DeleteBuildsResult result = new DeleteBuildsResult();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(component.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildArtifact>(component.GetBuildArtifactBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
        Dictionary<int, BuildData> dictionary = resultCollection.GetCurrent<BuildData>().Items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
        resultCollection.NextResult();
        foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
        {
          BuildData buildData = (BuildData) null;
          if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
            buildData.Tags.Add(buildTagData.Tag);
        }
        result.DeletedBuilds = (IList<BuildData>) dictionary.Values.ToList<BuildData>();
        resultCollection.NextResult();
        result.DeletedArtifacts = component.GetUniqueArtifacts((IList<BuildArtifact>) resultCollection.GetCurrent<BuildArtifact>().Items);
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
      component.TraceLeave(0, nameof (DeleteBuildsAsync));
      DeleteBuildsResult deleteBuildsResult = result;
      result = (DeleteBuildsResult) null;
      return deleteBuildsResult;
    }

    public override IEnumerable<BuildData> DestroyBuilds(
      Guid projectId,
      int definitionId,
      DateTime maxDeletedTime,
      int maxBuilds)
    {
      this.TraceEnter(0, nameof (DestroyBuilds));
      this.PrepareStoredProcedure("Build.prc_DestroyBuilds", 3600);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.BindNullableUtcDateTime2("@maxDeletedTime", new DateTime?(maxDeletedTime));
      this.BindInt("@maxBuilds", maxBuilds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        List<BuildData> list = resultCollection.GetCurrent<BuildData>().ToList<BuildData>();
        this.TraceLeave(0, nameof (DestroyBuilds));
        return (IEnumerable<BuildData>) list;
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
      this.BindString("@buildNumber", DBHelper.DBPathToServerPath(buildNumber), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
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

    public override IEnumerable<BuildData> GetBuildsByIds(
      IEnumerable<int> buildIds,
      bool includeDeleted)
    {
      this.TraceEnter(0, nameof (GetBuildsByIds));
      this.PrepareStoredProcedure("Build.prc_GetBuildsByIds");
      this.BindInt("@dataspaceId", 0);
      this.BindInt32Table("@buildIdTable", buildIds);
      this.BindBoolean("@includeDeleted", includeDeleted);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        Dictionary<int, BuildData> dictionary = resultCollection.GetCurrent<BuildData>().Items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
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
        this.TraceLeave(0, nameof (GetBuildsByIds));
        return (IEnumerable<BuildData>) dictionary.Values;
      }
    }

    public override async Task<IEnumerable<BuildData>> GetBuildsByIdsAsync(
      IEnumerable<int> buildIds,
      bool includeDeleted)
    {
      Build2Component31 component = this;
      component.TraceEnter(0, nameof (GetBuildsByIdsAsync));
      component.PrepareStoredProcedure("Build.prc_GetBuildsByIds");
      component.BindInt("@dataspaceId", 0);
      component.BindInt32Table("@buildIdTable", buildIds);
      component.BindBoolean("@includeDeleted", includeDeleted);
      IEnumerable<BuildData> values;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(component.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
        Dictionary<int, BuildData> dictionary = resultCollection.GetCurrent<BuildData>().Items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
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
        component.TraceLeave(0, nameof (GetBuildsByIdsAsync));
        values = (IEnumerable<BuildData>) dictionary.Values;
      }
      return values;
    }

    public override IList<BuildData> GetRunnableGatedBuilds()
    {
      using (this.TraceScope(method: nameof (GetRunnableGatedBuilds)))
      {
        this.PrepareStoredProcedure("Build.prc_GetRunnableGatedBuilds");
        List<BuildData> items;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
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
        return (IList<BuildData>) items;
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
      this.BindString("@buildNumberFormat", build.BuildNumber, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindUtcDateTime2("@queueTime", build.QueueTime ?? DateTime.UtcNow);
      this.BindByte("@queueOptions", (byte) build.QueueOptions, (byte) 0);
      this.BindBoolean("@changesCalculated", changesCalculated);
      this.BindStringTable("@changeDescriptors", changes != null ? changes.Select<ChangeData, string>((System.Func<ChangeData, string>) (c => c.Descriptor)) : (IEnumerable<string>) null);
      this.BindInt("@maxConcurrentBuildsPerBranch", maxConcurrentBuildsPerBranch);
      this.BindString("@triggerInfo", this.ToString<string, string>(build.TriggerInfo), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
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

    public override List<BuildData> UpdateBuilds(
      Guid projectId,
      IEnumerable<BuildData> builds,
      Guid changedBy,
      out IList<BuildData> oldBuilds,
      out IDictionary<int, BuildDefinition> definitionsById)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("Build.prc_UpdateBuilds");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindBuildUpdateTable("@buildUpdateTable", builds);
      this.BindGuid("@requestedBy", changedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        oldBuilds = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
        resultCollection.NextResult();
        List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
        Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
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
        definitionsById = (IDictionary<int, BuildDefinition>) new Dictionary<int, BuildDefinition>();
        this.TraceLeave(0, nameof (UpdateBuilds));
        return items;
      }
    }

    public override async Task<UpdateBuildsResult> UpdateBuildsAsync(
      Guid projectId,
      IEnumerable<BuildData> builds,
      Guid changedBy)
    {
      Build2Component31 build2Component31 = this;
      build2Component31.TraceEnter(0, nameof (UpdateBuildsAsync));
      build2Component31.PrepareStoredProcedure("Build.prc_UpdateBuilds");
      build2Component31.BindInt("@dataspaceId", build2Component31.GetDataspaceId(projectId));
      build2Component31.BindBuildUpdateTable("@buildUpdateTable", builds);
      build2Component31.BindGuid("@requestedBy", changedBy);
      UpdateBuildsResult result = new UpdateBuildsResult();
      UpdateBuildsResult updateBuildsResult;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component31.ExecuteReaderAsync(), build2Component31.ProcedureName, build2Component31.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(build2Component31.GetBuildDataBinder());
        resultCollection.AddBinder<BuildData>(build2Component31.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(build2Component31.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(build2Component31.GetBuildOrchestrationDataBinder());
        result.OldBuilds = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
        resultCollection.NextResult();
        result.NewBuilds = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
        Dictionary<int, BuildData> dictionary = result.NewBuilds.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
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
        result.DefinitionsById = (IDictionary<int, BuildDefinition>) new Dictionary<int, BuildDefinition>();
        build2Component31.TraceLeave(0, nameof (UpdateBuildsAsync));
        updateBuildsResult = result;
      }
      result = new UpdateBuildsResult();
      return updateBuildsResult;
    }

    protected override ObjectBinder<BuildData> GetBuildDataBinder() => (ObjectBinder<BuildData>) new BuildDataBinder13(this.RequestContext, (BuildSqlComponentBase) this);
  }
}
