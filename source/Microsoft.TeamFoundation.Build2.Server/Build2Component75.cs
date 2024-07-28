// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component75
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component75 : Build2Component74
  {
    protected static readonly SqlMetaData[] typ_ResourceRepositoryBranchTable = new SqlMetaData[3]
    {
      new SqlMetaData("RepositoryIdentifier", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RepositoryType", SqlDbType.NVarChar, 40L),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L)
    };

    public override BuildData AddBuild(
      BuildData build,
      Guid requestedBy,
      Guid requestedFor,
      bool changesCalculated,
      IEnumerable<ChangeData> changeData)
    {
      using (this.TraceScope(method: nameof (AddBuild)))
      {
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
        this.BindString("@templateParameters", this.SerializeTemplateParameters(build), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
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
        this.BindResourceRepositoryBranchTable("@resourceRepositoryBranches", (IEnumerable<RepositoryResource>) build.RepositoryResources);
        this.BindAppendCommitMessageToRunName(build.AppendCommitMessageToRunName);
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
          return buildData;
        }
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
      using (this.TraceScope(method: nameof (QueueBuild)))
      {
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
        this.BindString("@templateParameters", this.SerializeTemplateParameters(build), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
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
        this.BindResourceRepositoryBranchTable("@resourceRepositoryBranches", (IEnumerable<RepositoryResource>) build.RepositoryResources);
        this.BindString("@validationIssues", JsonUtility.ToString<BuildRequestValidationResult>((IList<BuildRequestValidationResult>) build.ValidationResults.Where<BuildRequestValidationResult>((System.Func<BuildRequestValidationResult, bool>) (vr => vr.Result != 0)).ToList<BuildRequestValidationResult>()), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (build.OrchestrationPlan == null || build.OrchestrationPlan.PlanId == Guid.Empty)
          this.BindNullableGuid("@orchestrationId", new Guid?());
        else
          this.BindNullableGuid("@orchestrationId", build.OrchestrationPlan.PlanId);
        queue = (AgentPoolQueue) null;
        BuildData buildData;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          buildData = resultCollection.GetCurrent<BuildData>().FirstOrDefault<BuildData>();
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
        }
        return buildData;
      }
    }

    public virtual void BindResourceRepositoryBranchTable(
      string parameterName,
      IEnumerable<RepositoryResource> resourceRepositories)
    {
      IEnumerable<RepositoryResource> source = Enumerable.Empty<RepositoryResource>();
      if (resourceRepositories != null && resourceRepositories.Count<RepositoryResource>() > 0)
        source = resourceRepositories.Where<RepositoryResource>((System.Func<RepositoryResource, bool>) (r => r.Id != null && r.Type != null));
      System.Func<RepositoryResource, SqlDataRecord> selector = (System.Func<RepositoryResource, SqlDataRecord>) (repoResource =>
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component75.typ_ResourceRepositoryBranchTable);
        record.SetString(0, repoResource.Id, BindStringBehavior.Unchanged);
        record.SetString(1, this.TranslateRepositoryType(repoResource.Type), BindStringBehavior.Unchanged);
        record.SetString(2, repoResource.Ref, BindStringBehavior.Unchanged);
        return record;
      });
      this.BindTable(parameterName, "Build.typ_ResourceRepositoryBranchTable", source.Select<RepositoryResource, SqlDataRecord>(selector));
    }

    internal string TranslateRepositoryType(string currentRepositoryType)
    {
      if (string.Equals(currentRepositoryType, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.ExternalGit, StringComparison.OrdinalIgnoreCase))
        return "Git";
      if (string.Equals(currentRepositoryType, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Git, StringComparison.OrdinalIgnoreCase))
        return "TfsGit";
      return string.Equals(currentRepositoryType, Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryTypes.Tfvc, StringComparison.OrdinalIgnoreCase) ? "TfsVersionControl" : currentRepositoryType;
    }
  }
}
