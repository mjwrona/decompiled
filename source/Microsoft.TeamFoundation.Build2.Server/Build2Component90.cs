// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component90
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
  internal class Build2Component90 : Build2Component89
  {
    public override async Task<PurgedBuildsResults> PurgeBuildsAsync(
      Guid projectId,
      int daysOld,
      int batchSize,
      string branchPrefix)
    {
      Build2Component90 build2Component90 = this;
      PurgedBuildsResults purgedBuildsResults;
      using (build2Component90.TraceScope(method: nameof (PurgeBuildsAsync)))
      {
        build2Component90.PrepareStoredProcedure("Build.prc_PurgeBuilds");
        build2Component90.BindInt("@dataspaceId", build2Component90.GetDataspaceId(projectId));
        build2Component90.BindInt("@daysOld", daysOld);
        build2Component90.BindInt("@batchSize", batchSize);
        build2Component90.BindString("@branchPrefix", branchPrefix, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection rc = new ResultCollection((IDataReader) await build2Component90.ExecuteReaderAsync(), build2Component90.ProcedureName, build2Component90.RequestContext))
        {
          rc.AddBinder<BuildArtifact>(build2Component90.GetBuildArtifactBinder());
          rc.AddBinder<BuildData>(build2Component90.GetBuildDataBinder());
          rc.AddBinder<BuildOrchestrationData>(build2Component90.GetBuildOrchestrationDataBinder());
          purgedBuildsResults = new PurgedBuildsResults(rc);
        }
      }
      return purgedBuildsResults;
    }

    public override async Task<PurgedBuildsResults> PurgeArtifactsAsync(
      Guid projectId,
      int daysOld,
      int batchSize)
    {
      Build2Component90 build2Component90 = this;
      PurgedBuildsResults purgedBuildsResults;
      using (build2Component90.TraceScope(method: nameof (PurgeArtifactsAsync)))
      {
        build2Component90.PrepareStoredProcedure("Build.prc_PurgeArtifacts");
        build2Component90.BindInt("@dataspaceId", build2Component90.GetDataspaceId(projectId));
        build2Component90.BindInt("@daysOld", daysOld);
        build2Component90.BindInt("@batchSize", batchSize);
        using (ResultCollection rc = new ResultCollection((IDataReader) await build2Component90.ExecuteReaderAsync(), build2Component90.ProcedureName, build2Component90.RequestContext))
        {
          rc.AddBinder<BuildArtifact>(build2Component90.GetBuildArtifactBinder());
          rc.AddBinder<BuildData>(build2Component90.GetBuildDataBinder());
          purgedBuildsResults = new PurgedBuildsResults(rc);
        }
      }
      return purgedBuildsResults;
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
      this.BindDesignerScheduleTable("@scheduleTable", (IEnumerable<DesignerSchedule>) this.ExtractDesignerSchedulesFromDefinition(definition));
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
      this.BindDesignerScheduleTable("@scheduleTable", (IEnumerable<DesignerSchedule>) this.ExtractDesignerSchedulesFromDefinition(definition));
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
  }
}
