// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component50
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component50 : Build2Component49
  {
    public override IEnumerable<int> GetVeryOldDeletedDefinitionIds(Guid projectId)
    {
      this.TraceEnter(0, nameof (GetVeryOldDeletedDefinitionIds));
      this.PrepareStoredProcedure("Build.prc_GetVeryOldDeletedDefinitionIds");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
        List<int> items = resultCollection.GetCurrent<int>().Items;
        this.TraceLeave(0, nameof (GetVeryOldDeletedDefinitionIds));
        return (IEnumerable<int>) items;
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

    public override List<ChangeData> StoreChanges(
      Guid projectId,
      int buildId,
      IEnumerable<ChangeData> changes)
    {
      this.TraceEnter(0, nameof (StoreChanges));
      this.PrepareStoredProcedure("Build.prc_StoreChanges");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindChangeDataTable("@changes", changes);
      List<ChangeData> changeDataList = (List<ChangeData>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ChangeData>(this.GetChangeDataBinder());
        changeDataList = resultCollection.GetCurrent<ChangeData>().Items;
      }
      this.TraceLeave(0, nameof (StoreChanges));
      return changeDataList;
    }

    protected override ObjectBinder<ChangeData> GetChangeDataBinder() => (ObjectBinder<ChangeData>) new ChangeDataBinder2();
  }
}
