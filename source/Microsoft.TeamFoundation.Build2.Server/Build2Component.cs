// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server.ObjectModel;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component : BuildSqlComponentBase
  {
    private const string c_layer = "Build2Component";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[65]
    {
      (IComponentCreator) new ComponentCreator<Build2Component>(30),
      (IComponentCreator) new ComponentCreator<Build2Component31>(31),
      (IComponentCreator) new ComponentCreator<Build2Component32>(32),
      (IComponentCreator) new ComponentCreator<Build2Component33>(33),
      (IComponentCreator) new ComponentCreator<Build2Component34>(34),
      (IComponentCreator) new ComponentCreator<Build2Component35>(35),
      (IComponentCreator) new ComponentCreator<Build2Component36>(36),
      (IComponentCreator) new ComponentCreator<Build2Component37>(37),
      (IComponentCreator) new ComponentCreator<Build2Component38>(38),
      (IComponentCreator) new ComponentCreator<Build2Component39>(39),
      (IComponentCreator) new ComponentCreator<Build2Component40>(40),
      (IComponentCreator) new ComponentCreator<Build2Component41>(41),
      (IComponentCreator) new ComponentCreator<Build2Component42>(42),
      (IComponentCreator) new ComponentCreator<Build2Component43>(43),
      (IComponentCreator) new ComponentCreator<Build2Component44>(44),
      (IComponentCreator) new ComponentCreator<Build2Component45>(45),
      (IComponentCreator) new ComponentCreator<Build2Component46>(46),
      (IComponentCreator) new ComponentCreator<Build2Component47>(47),
      (IComponentCreator) new ComponentCreator<Build2Component48>(48),
      (IComponentCreator) new ComponentCreator<Build2Component49>(49),
      (IComponentCreator) new ComponentCreator<Build2Component50>(50),
      (IComponentCreator) new ComponentCreator<Build2Component51>(51),
      (IComponentCreator) new ComponentCreator<Build2Component52>(52),
      (IComponentCreator) new ComponentCreator<Build2Component53>(53),
      (IComponentCreator) new ComponentCreator<Build2Component54>(54),
      (IComponentCreator) new ComponentCreator<Build2Component55>(55),
      (IComponentCreator) new ComponentCreator<Build2Component56>(56),
      (IComponentCreator) new ComponentCreator<Build2Component57>(57),
      (IComponentCreator) new ComponentCreator<Build2Component58>(58),
      (IComponentCreator) new ComponentCreator<Build2Component59>(59),
      (IComponentCreator) new ComponentCreator<Build2Component60>(60),
      (IComponentCreator) new ComponentCreator<Build2Component61>(61),
      (IComponentCreator) new ComponentCreator<Build2Component62>(62),
      (IComponentCreator) new ComponentCreator<Build2Component63>(63),
      (IComponentCreator) new ComponentCreator<Build2Component64>(64),
      (IComponentCreator) new ComponentCreator<Build2Component65>(65),
      (IComponentCreator) new ComponentCreator<Build2Component66>(66),
      (IComponentCreator) new ComponentCreator<Build2Component67>(67),
      (IComponentCreator) new ComponentCreator<Build2Component68>(68),
      (IComponentCreator) new ComponentCreator<Build2Component69>(69),
      (IComponentCreator) new ComponentCreator<Build2Component70>(70),
      (IComponentCreator) new ComponentCreator<Build2Component71>(71),
      (IComponentCreator) new ComponentCreator<Build2Component72>(72),
      (IComponentCreator) new ComponentCreator<Build2Component73>(73),
      (IComponentCreator) new ComponentCreator<Build2Component74>(74),
      (IComponentCreator) new ComponentCreator<Build2Component75>(75),
      (IComponentCreator) new ComponentCreator<Build2Component76>(76),
      (IComponentCreator) new ComponentCreator<Build2Component77>(77),
      (IComponentCreator) new ComponentCreator<Build2Component78>(78),
      (IComponentCreator) new ComponentCreator<Build2Component79>(79),
      (IComponentCreator) new ComponentCreator<Build2Component80>(80),
      (IComponentCreator) new ComponentCreator<Build2Component81>(81),
      (IComponentCreator) new ComponentCreator<Build2Component82>(82),
      (IComponentCreator) new ComponentCreator<Build2Component83>(83),
      (IComponentCreator) new ComponentCreator<Build2Component84>(84),
      (IComponentCreator) new ComponentCreator<Build2Component85>(85),
      (IComponentCreator) new ComponentCreator<Build2Component86>(86),
      (IComponentCreator) new ComponentCreator<Build2Component87>(87),
      (IComponentCreator) new ComponentCreator<Build2Component88>(88),
      (IComponentCreator) new ComponentCreator<Build2Component89>(89),
      (IComponentCreator) new ComponentCreator<Build2Component90>(90),
      (IComponentCreator) new ComponentCreator<Build2Component91>(91),
      (IComponentCreator) new ComponentCreator<Build2Component92>(92),
      (IComponentCreator) new ComponentCreator<Build2Component93>(93),
      (IComponentCreator) new ComponentCreator<Build2Component94>(94)
    }, "Build2", "Build");
    protected static readonly DateTime BuildMetricUnscopedDate = new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Utc);
    protected static readonly SqlMetaData[] typ_BuildUpdateTable3 = new SqlMetaData[10]
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
      new SqlMetaData("RetainedByRelease", SqlDbType.Bit)
    };
    protected static readonly SqlMetaData[] typ_BuildUpdateTable4 = new SqlMetaData[11]
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
      new SqlMetaData("ValidationIssues", SqlDbType.NVarChar, -1L)
    };
    protected static readonly SqlMetaData[] typ_ChangeDataTable = new SqlMetaData[2]
    {
      new SqlMetaData("Descriptor", SqlDbType.NVarChar, 326L),
      new SqlMetaData("ExternalData", SqlDbType.NVarChar, 2048L)
    };
    protected static readonly SqlMetaData[] typ_ChangeDataTable2 = new SqlMetaData[3]
    {
      new SqlMetaData("SourceChangeOnly", SqlDbType.Bit),
      new SqlMetaData("Descriptor", SqlDbType.NVarChar, 326L),
      new SqlMetaData("ExternalData", SqlDbType.NVarChar, 2048L)
    };
    protected static readonly SqlMetaData[] typ_DefinitionBranchTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("SourceId", SqlDbType.BigInt),
      new SqlMetaData("SourceOwner", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SourceVersion", SqlDbType.NVarChar, 326L)
    };
    protected static readonly SqlMetaData[] typ_DefinitionMetricUpdateTable = new SqlMetaData[4]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, 400L),
      new SqlMetaData("ScopedDate", SqlDbType.DateTime),
      new SqlMetaData("IntIncValue", SqlDbType.Int)
    };
    protected static readonly SqlMetaData[] typ_DefinitionRepositoryMapTable = new SqlMetaData[5]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("RepoType", SqlDbType.NVarChar, 40L),
      new SqlMetaData("RepoIdentifier", SqlDbType.NVarChar, 400L),
      new SqlMetaData("DefaultBranch", SqlDbType.NVarChar, 400L)
    };

    public IList<BuildArtifact> GetUniqueArtifacts(IList<BuildArtifact> artifacts) => (IList<BuildArtifact>) artifacts.GetUniqueArtifacts();

    public virtual BuildArtifact AddArtifact(Guid projectId, int buildId, BuildArtifact artifact)
    {
      this.TraceEnter(0, nameof (AddArtifact));
      this.PrepareStoredProcedure("Build.prc_AddArtifact");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindString("@artifactName", artifact.Name, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@artifactType", artifact.Resource.Type, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@data", artifact.Resource.Data, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      string parameterValue = "";
      if (artifact.Resource.Properties != null)
      {
        try
        {
          parameterValue = JsonUtility.ToString((object) artifact.Resource.Properties);
        }
        catch (JsonSerializationException ex)
        {
          parameterValue = "";
        }
      }
      this.BindString("@metadata", parameterValue, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildArtifact>(this.GetBuildArtifactBinder());
        BuildArtifact buildArtifact = resultCollection.GetCurrent<BuildArtifact>().FirstOrDefault<BuildArtifact>();
        this.TraceLeave(0, nameof (AddArtifact));
        return buildArtifact;
      }
    }

    public virtual BuildData AddBuild(
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

    public virtual IEnumerable<string> AddBuildTags(
      Guid projectId,
      int buildId,
      IEnumerable<string> tags)
    {
      this.TraceEnter(0, nameof (AddBuildTags));
      this.PrepareStoredProcedure("Build.prc_AddBuildTags");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        List<string> list = resultCollection.GetCurrent<BuildTagData>().Where<BuildTagData>((System.Func<BuildTagData, bool>) (t => t.BuildId == buildId)).Select<BuildTagData, string>((System.Func<BuildTagData, string>) (t => t.Tag)).ToList<string>();
        this.TraceLeave(0, nameof (AddBuildTags));
        return (IEnumerable<string>) list;
      }
    }

    public virtual BuildDefinition AddDefinition(
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
      this.BindString("@options", this.ToString<BuildOption>((IList<BuildOption>) definition.Options), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repository", JsonUtility.ToString((object) definition.Repository), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@triggers", this.ToString<BuildTrigger>((IList<BuildTrigger>) definition.Triggers), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@steps", this.ProcessToSteps(definition.Process), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@variables", this.ToString<string, BuildDefinitionVariable>((IDictionary<string, BuildDefinitionVariable>) definition.Variables), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@demands", this.ToString<Demand>((IList<Demand>) definition.Demands), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@retentionPolicy", (string) null, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@description", definition.Description, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@buildNumberFormat", definition.BuildNumberFormat, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@jobAuthorizationScope", (byte) definition.JobAuthorizationScope);
      this.BindNullableInt("@jobTimeout", definition.JobTimeoutInMinutes, 0);
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

    public virtual IEnumerable<string> AddDefinitionTags(
      Guid projectId,
      int definitionId,
      int definitionVersion,
      IEnumerable<string> tags)
    {
      this.TraceEnter(0, nameof (AddDefinitionTags));
      this.PrepareStoredProcedure("Build.prc_AddDefinitionTags");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@definitionVersion", definitionVersion);
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        List<string> list = resultCollection.GetCurrent<DefinitionTagData>().Select<DefinitionTagData, string>((System.Func<DefinitionTagData, string>) (t => t.Tag)).ToList<string>();
        this.TraceLeave(0, nameof (AddDefinitionTags));
        return (IEnumerable<string>) list;
      }
    }

    public virtual Folder AddFolder(Guid projectId, string path, Folder folder, Guid requestedBy)
    {
      this.TraceEnter(0, nameof (AddFolder));
      this.PrepareStoredProcedure("Build.prc_AddFolder");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@path", DBHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", folder.Description, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@requestedBy", requestedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Folder>(this.GetFolderBinder());
        Folder folder1 = resultCollection.GetCurrent<Folder>().FirstOrDefault<Folder>();
        this.TraceLeave(0, nameof (AddFolder));
        return folder1;
      }
    }

    public virtual BuildOrchestrationData AddOrchestration(
      Guid projectId,
      int buildId,
      int orchestrationType)
    {
      this.TraceEnter(0, nameof (AddOrchestration));
      this.PrepareStoredProcedure("Build.prc_AddBuildOrchestration");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindInt("@orchestrationType", orchestrationType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        BuildOrchestrationData orchestrationData = resultCollection.GetCurrent<BuildOrchestrationData>().FirstOrDefault<BuildOrchestrationData>();
        this.TraceLeave(0, nameof (AddOrchestration));
        return orchestrationData;
      }
    }

    public virtual AgentPoolQueue AddQueue(AgentPoolQueue queue)
    {
      this.TraceEnter(0, nameof (AddQueue));
      this.PrepareStoredProcedure("Build.prc_AddQueue");
      this.BindString("@queueName", DBHelper.ServerPathToDBPath(queue.Name), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@poolId", queue.Pool.Id);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AgentPoolQueue>(this.GetAgentPoolQueueBinder());
        queue = resultCollection.GetCurrent<AgentPoolQueue>().FirstOrDefault<AgentPoolQueue>();
      }
      this.TraceLeave(0, nameof (AddQueue));
      return queue;
    }

    public virtual Task<RetentionLease> AddRetentionLease(
      Guid projectId,
      string owner,
      int buildId,
      int definitionId,
      DateTime validUntil,
      bool highPriority,
      int maxLeases)
    {
      return Task.FromResult<RetentionLease>((RetentionLease) null);
    }

    public virtual Task<IReadOnlyList<RetentionLease>> AddRetentionLeases(
      Guid projectId,
      IEnumerable<RetentionLease> leases)
    {
      return Task.FromResult<IReadOnlyList<RetentionLease>>((IReadOnlyList<RetentionLease>) Array.Empty<RetentionLease>());
    }

    public virtual Task<IList<BuildSchedule>> GetSchedulesByDefinitionId(
      Guid projectId,
      int definitionId)
    {
      return Task.FromResult<IList<BuildSchedule>>((IList<BuildSchedule>) Array.Empty<BuildSchedule>());
    }

    public virtual Task<BuildProcessResources> UpdateAuthorizedResourcesAsync(
      Guid projectId,
      int? definitionId,
      Guid authorizedBy,
      BuildProcessResources resources)
    {
      return Task.FromResult<BuildProcessResources>(new BuildProcessResources());
    }

    public virtual Task<BuildProcessResources> GetAuthorizedResourcesAsync(
      Guid projectId,
      int? definitionId,
      ResourceType? resourceType,
      string resourceId)
    {
      return Task.FromResult<BuildProcessResources>(new BuildProcessResources());
    }

    public virtual bool SupportsResourceAuthorizationForAllDefinitions() => false;

    protected virtual SqlParameter BindChangeDataTable(
      string parameterName,
      IEnumerable<ChangeData> changes)
    {
      changes = changes ?? Enumerable.Empty<ChangeData>();
      changes = changes.Where<ChangeData>((System.Func<ChangeData, bool>) (c => !c.SourceChangeOnly));
      return this.BindTable(parameterName, "Build.typ_ChangeDataTable", changes.Select<ChangeData, SqlDataRecord>(new System.Func<ChangeData, SqlDataRecord>(rowBinder)));

      static SqlDataRecord rowBinder(ChangeData changeData)
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component.typ_ChangeDataTable);
        record.SetString(0, changeData.Descriptor, BindStringBehavior.Unchanged);
        record.SetString(1, changeData.ExternalData, BindStringBehavior.Unchanged);
        return record;
      }
    }

    protected SqlParameter BindBuildDefinitionBranchTable2(
      string parameterName,
      IEnumerable<BuildDefinitionBranch> branches)
    {
      branches = branches ?? Enumerable.Empty<BuildDefinitionBranch>();
      System.Func<BuildDefinitionBranch, SqlDataRecord> selector = (System.Func<BuildDefinitionBranch, SqlDataRecord>) (branch =>
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component.typ_DefinitionBranchTable2);
        record.SetString(0, branch.BranchName, BindStringBehavior.Unchanged);
        record.SetInt64(1, branch.SourceId);
        record.SetGuid(2, branch.PendingSourceOwner);
        record.SetString(3, branch.PendingSourceVersion, BindStringBehavior.Unchanged);
        this.Trace(12030299, TraceLevel.Info, "Build2", (object) nameof (Build2Component), (object) ("Adding the following branch record to Build.typ_DefinitionBranchTable2 table: " + JsonConvert.SerializeObject((object) new Dictionary<string, object>()
        {
          ["branchName"] = (object) branch.BranchName,
          ["sourceId"] = (object) branch.SourceId,
          ["pendingSourceOwner"] = (object) branch.PendingSourceOwner,
          ["pendingSourceVersion"] = (object) branch.PendingSourceVersion
        })));
        return record;
      });
      return this.BindTable(parameterName, "Build.typ_DefinitionBranchTable2", branches.Select<BuildDefinitionBranch, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindBuildDefinitionMetricUpdateTable(
      string parameterName,
      IEnumerable<BuildDefinitionMetric> buildDefinitionMetrics)
    {
      buildDefinitionMetrics = buildDefinitionMetrics ?? Enumerable.Empty<BuildDefinitionMetric>();
      System.Func<BuildDefinitionMetric, SqlDataRecord> selector = (System.Func<BuildDefinitionMetric, SqlDataRecord>) (buildDefinitionMetric =>
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component.typ_DefinitionMetricUpdateTable);
        record.SetInt32(0, buildDefinitionMetric.DefinitionId);
        record.SetString(1, buildDefinitionMetric.Metric.Name, BindStringBehavior.Unchanged);
        record.SetDateTime(2, buildDefinitionMetric.Metric.Date ?? Build2Component.BuildMetricUnscopedDate);
        record.SetInt32(3, buildDefinitionMetric.Metric.IntValue);
        return record;
      });
      return this.BindTable(parameterName, "Build.typ_DefinitionMetricUpdateTable2", buildDefinitionMetrics.Select<BuildDefinitionMetric, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindBuildUpdateTable(
      string parameterName,
      IEnumerable<BuildData> builds)
    {
      builds = builds ?? Enumerable.Empty<BuildData>();
      System.Func<BuildData, SqlDataRecord> selector = (System.Func<BuildData, SqlDataRecord>) (build =>
      {
        SqlDataRecord record1 = new SqlDataRecord(Build2Component.typ_BuildUpdateTable3);
        record1.SetInt32(0, build.Id);
        record1.SetString(1, build.BuildNumber, BindStringBehavior.Unchanged);
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

    protected void BindLatestBuilds(ResultCollection rc, BuildDefinition definition) => this.BindLatestBuilds(rc, new Dictionary<int, BuildDefinition>()
    {
      {
        definition.Id,
        definition
      }
    });

    protected virtual void BindLatestBuilds(
      ResultCollection rc,
      Dictionary<int, BuildDefinition> definitions)
    {
      rc.NextResult();
      List<BuildData> items = rc.GetCurrent<BuildData>().Items;
      Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
      rc.NextResult();
      foreach (BuildTagData buildTagData in rc.GetCurrent<BuildTagData>())
      {
        BuildData buildData;
        if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
          buildData.Tags.Add(buildTagData.Tag);
      }
      rc.NextResult();
      foreach (BuildOrchestrationData orchestrationData in rc.GetCurrent<BuildOrchestrationData>())
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
      foreach (BuildData buildData in items)
      {
        BuildDefinition buildDefinition;
        if (definitions.TryGetValue(buildData.Definition.Id, out buildDefinition))
        {
          BuildStatus? status = buildData.Status;
          BuildStatus buildStatus = BuildStatus.Completed;
          DateTime? nullable;
          if (status.GetValueOrDefault() == buildStatus & status.HasValue)
          {
            if (buildDefinition.LatestCompletedBuild != null)
            {
              nullable = buildDefinition.LatestCompletedBuild.FinishTime;
              DateTime dateTime1 = nullable.Value;
              ref DateTime local = ref dateTime1;
              nullable = buildData.FinishTime;
              DateTime dateTime2 = nullable.Value;
              if (local.CompareTo(dateTime2) >= 0)
                goto label_25;
            }
            buildDefinition.LatestCompletedBuild = buildData;
          }
label_25:
          if (buildDefinition.LatestBuild != null)
          {
            nullable = buildDefinition.LatestBuild.QueueTime;
            DateTime dateTime3 = nullable.Value;
            ref DateTime local = ref dateTime3;
            nullable = buildData.QueueTime;
            DateTime dateTime4 = nullable.Value;
            if (local.CompareTo(dateTime4) >= 0)
              continue;
          }
          buildDefinition.LatestBuild = buildData;
        }
      }
    }

    public virtual void CreateTeamProject(Guid projectId)
    {
      this.TraceEnter(0, nameof (CreateTeamProject));
      this.PrepareStoredProcedure("Build.prc_CreateTeamProject");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (CreateTeamProject));
    }

    public virtual DeleteBuildsResult DeleteBuilds(
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

    public virtual async Task<DeleteBuildsResult> DeleteBuildsAsync(
      Guid projectId,
      IEnumerable<int> buildIds,
      Guid requestedBy,
      bool setBuildRecordAsDeleted = true,
      string deletedReason = null)
    {
      Build2Component component = this;
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

    public virtual IEnumerable<string> DeleteBuildTags(
      Guid projectId,
      int buildId,
      IEnumerable<string> tags)
    {
      this.TraceEnter(0, nameof (DeleteBuildTags));
      this.PrepareStoredProcedure("Build.prc_DeleteBuildTags");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        List<string> list = resultCollection.GetCurrent<BuildTagData>().Where<BuildTagData>((System.Func<BuildTagData, bool>) (t => t.BuildId == buildId)).Select<BuildTagData, string>((System.Func<BuildTagData, string>) (t => t.Tag)).ToList<string>();
        this.TraceLeave(0, nameof (DeleteBuildTags));
        return (IEnumerable<string>) list;
      }
    }

    public virtual void DeleteDefinitions(
      Guid projectId,
      IEnumerable<int> definitionIds,
      Guid requestedBy)
    {
      this.TraceEnter(0, nameof (DeleteDefinitions));
      this.PrepareStoredProcedure("Build.prc_DeleteDefinitions");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt32Table("@definitionIdTable", definitionIds);
      this.BindGuid("@requestedBy", requestedBy);
      this.BindGuid("@writerId", this.Author);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteDefinitions));
    }

    public virtual void DeleteDefinitions(
      Guid projectId,
      IEnumerable<int> definitionIds,
      Guid requestedBy,
      out List<BuildData> buildsToCancel)
    {
      buildsToCancel = (List<BuildData>) null;
      this.DeleteDefinitions(projectId, definitionIds, requestedBy);
    }

    public virtual IEnumerable<string> DeleteDefinitionTags(
      Guid projectId,
      int definitionId,
      int definitionVersion,
      IEnumerable<string> tags)
    {
      this.TraceEnter(0, nameof (DeleteDefinitionTags));
      this.PrepareStoredProcedure("Build.prc_DeleteDefinitionTags");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@definitionVersion", definitionVersion);
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        List<string> list = resultCollection.GetCurrent<DefinitionTagData>().Select<DefinitionTagData, string>((System.Func<DefinitionTagData, string>) (t => t.Tag)).ToList<string>();
        this.TraceLeave(0, nameof (DeleteDefinitionTags));
        return (IEnumerable<string>) list;
      }
    }

    public virtual Task<RetentionLease> DeleteRetentionLease(
      Guid projectId,
      string owner,
      int runId,
      int definitionId)
    {
      return Task.FromResult<RetentionLease>((RetentionLease) null);
    }

    public virtual Task<IReadOnlyList<RetentionLease>> DeleteRetentionLeases(
      Guid projectId,
      HashSet<int> leaseIds)
    {
      return Task.FromResult<IReadOnlyList<RetentionLease>>((IReadOnlyList<RetentionLease>) Array.Empty<RetentionLease>());
    }

    public virtual Task<IReadOnlyList<RetentionLease>> DeleteRetentionLeasesByOwnerPrefix(
      Guid projectId,
      string ownerPrefix)
    {
      return Task.FromResult<IReadOnlyList<RetentionLease>>((IReadOnlyList<RetentionLease>) Array.Empty<RetentionLease>());
    }

    public virtual IEnumerable<string> DeleteTags(Guid projectId, IEnumerable<string> tags)
    {
      this.TraceEnter(0, nameof (DeleteTags));
      this.PrepareStoredProcedure("Build.prc_DeleteTags");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindStringTable("@tags", tags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TagStringData>(this.GetTagStringBinder());
        List<string> list = resultCollection.GetCurrent<TagStringData>().Select<TagStringData, string>((System.Func<TagStringData, string>) (t => t.StringValue)).ToList<string>();
        this.TraceLeave(0, nameof (DeleteTags));
        return (IEnumerable<string>) list;
      }
    }

    public virtual void DeleteDefinitionTemplates(Guid projectId, List<string> templateIds)
    {
      this.TraceEnter(0, nameof (DeleteDefinitionTemplates));
      this.PrepareStoredProcedure("Build.prc_DeleteDefinitionTemplates");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindStringTable("@templateIds", (IEnumerable<string>) templateIds);
      this.BindGuid("@writerId", this.Author);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteDefinitionTemplates));
    }

    public virtual void DeleteFolder(Guid projectId, string path, Guid requestedBy)
    {
      this.TraceEnter(0, nameof (DeleteFolder));
      this.PrepareStoredProcedure("Build.prc_DeleteFolder");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@path", DBHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@requestedBy", requestedBy);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteFolder));
    }

    public virtual void DeleteQueues(IEnumerable<int> queueIds)
    {
      this.TraceEnter(0, nameof (DeleteQueues));
      this.PrepareStoredProcedure("Build.prc_DeleteQueues");
      this.BindInt32Table("@queueIds", queueIds);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteQueues));
    }

    public virtual void DeleteTeamProject(Guid projectId)
    {
      this.TraceEnter(0, nameof (DeleteTeamProject));
      this.PrepareStoredProcedure("Build.prc_DeleteTeamProject");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteTeamProject));
    }

    public virtual IEnumerable<BuildData> DestroyBuilds(
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

    public virtual void DestroyDefinition(Guid projectId, int definitionId)
    {
      this.TraceEnter(0, nameof (DestroyDefinition));
      this.PrepareStoredProcedure("Build.prc_DestroyDefinition");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DestroyDefinition));
    }

    public virtual void DestroyDefinitionMetrics(Guid projectId, int days)
    {
      this.TraceEnter(0, nameof (DestroyDefinitionMetrics));
      this.PrepareStoredProcedure("Build.prc_DestroyDefinitionMetrics");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@days", days);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DestroyDefinitionMetrics));
    }

    public virtual void DestroyProjectMetrics(
      Guid projectId,
      DateTime minTimeToRetain,
      DateTime startTimeToAggregate,
      DateTime endTimeToAggregate)
    {
      this.TraceEnter(0, nameof (DestroyProjectMetrics));
      this.PrepareStoredProcedure("Build.prc_DestroyProjectMetrics");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindDateTime("@minTimeToRetain", minTimeToRetain, true);
      this.BindDateTime("@startTimeToAggregate", startTimeToAggregate, true);
      this.BindDateTime("@endTimeToAggregate", endTimeToAggregate, true);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DestroyProjectMetrics));
    }

    public virtual bool FilePathArtifactCleanupSupported() => true;

    public virtual Task<IList<BuildData>> FilterBuildsAsync(
      Guid projectId,
      int? definitionId,
      string folderPath,
      HashSet<int> repositoryIds,
      HashSet<int> branchIds,
      HashSet<string> keywordFilter,
      HashSet<Guid> requestedForFilter,
      BuildResult? resultFilter,
      BuildStatus? statusFilter,
      HashSet<string> tagFilter,
      TimeFilter? timeFilter,
      int maxBuilds,
      HashSet<int> excludedDefinitionIds)
    {
      return Task.FromResult<IList<BuildData>>((IList<BuildData>) Array.Empty<BuildData>());
    }

    public virtual Task<IList<BuildData>> FilterBuildsByBranchAsync(
      Guid projectId,
      int? definitionId,
      string folderPath,
      HashSet<int> repositoryIds,
      HashSet<int> branchIds,
      TimeFilter? timeFilter,
      int maxBuilds,
      HashSet<int> excludedDefinitionIds)
    {
      return Task.FromResult<IList<BuildData>>((IList<BuildData>) Array.Empty<BuildData>());
    }

    public virtual Task<IList<BuildData>> FilterBuildsByTagsAsync(
      Guid projectId,
      int? definitionId,
      string folderPath,
      HashSet<string> tagFilter,
      TimeFilter? timeFilter,
      int maxBuilds,
      HashSet<int> excludedDefinitionIds)
    {
      return Task.FromResult<IList<BuildData>>((IList<BuildData>) Array.Empty<BuildData>());
    }

    public virtual Task<IList<RepositoryBranchReferences>> GetBranchesByName(
      Guid projectId,
      int maxCount,
      string nameLike,
      HashSet<int> excludedBranchIds)
    {
      return Task.FromResult<IList<RepositoryBranchReferences>>((IList<RepositoryBranchReferences>) Array.Empty<RepositoryBranchReferences>());
    }

    public virtual Task<IList<RepositoryBranchReferences>> GetBranchesByNameForDefinition(
      Guid projectId,
      int maxCount,
      int definitionId,
      string nameLike,
      HashSet<int> excludedBranchIds)
    {
      return Task.FromResult<IList<RepositoryBranchReferences>>((IList<RepositoryBranchReferences>) Array.Empty<RepositoryBranchReferences>());
    }

    public virtual Task<IList<BuildArtifact>> GetArtifactsBySourceAsync(
      Guid projectId,
      int buildId,
      string source)
    {
      return Task.FromResult<IList<BuildArtifact>>((IList<BuildArtifact>) Array.Empty<BuildArtifact>());
    }

    public virtual IEnumerable<BuildArtifact> GetArtifacts(
      Guid projectId,
      int buildId,
      string artifactName)
    {
      this.TraceEnter(0, nameof (GetArtifacts));
      this.PrepareStoredProcedure("Build.prc_GetArtifacts");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindString("@artifactName", artifactName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildArtifact>(this.GetBuildArtifactBinder());
        IList<BuildArtifact> uniqueArtifacts = this.GetUniqueArtifacts((IList<BuildArtifact>) resultCollection.GetCurrent<BuildArtifact>().Items);
        this.TraceLeave(0, nameof (GetArtifacts));
        return (IEnumerable<BuildArtifact>) uniqueArtifacts;
      }
    }

    public virtual List<BuildDefinitionBranch> GetBuildableDefinitionBranches(
      Guid projectId,
      int definitionId,
      int maxConcurrentBuildsPerBranch)
    {
      this.TraceEnter(0, nameof (GetBuildableDefinitionBranches));
      this.PrepareStoredProcedure("Build.prc_GetBuildableDefinitionBranches");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@maxConcurrentBuildsPerBranch", maxConcurrentBuildsPerBranch);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinitionBranch>(this.GetBuildDefinitionBranchBinder());
        List<BuildDefinitionBranch> items = resultCollection.GetCurrent<BuildDefinitionBranch>().Items;
        this.TraceLeave(0, nameof (GetBuildableDefinitionBranches));
        return items;
      }
    }

    public virtual void SampleRetentionData(int retentionDays)
    {
    }

    public virtual IEnumerable<BuildRetentionSample> GetRetentionHistory(int lookbackDays) => (IEnumerable<BuildRetentionSample>) new List<BuildRetentionSample>();

    protected virtual ObjectBinder<BuildRetentionSample> GetBuildRetentionSampleBinder() => (ObjectBinder<BuildRetentionSample>) new BuildRetentionSampleBinder(this.RequestContext);

    protected virtual CheckEventBinder GetCheckEventBinder(bool securityFixEnabled = false) => new CheckEventBinder((BuildSqlComponentBase) this, securityFixEnabled);

    public virtual IEnumerable<BuildData> GetBuilds(
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

    public virtual async Task<IList<BuildData>> GetAllBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      BuildQueryOrder buildQueryOrder = queryOrder;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? minFinishTime;
      if (!local1.HasValue)
      {
        minFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        minFinishTime = valueOrDefault.MinTime;
      }
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? maxFinishTime;
      if (!local2.HasValue)
      {
        maxFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        maxFinishTime = valueOrDefault.MaxTime;
      }
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable1 = new int?();
      BuildResult? nullable2 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?();
      BuildResult? resultFilter = nullable2;
      int maxBuilds = num;
      int queryOrder1 = (int) buildQueryOrder;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable1;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) null, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, (BuildQueryOrder) queryOrder1, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual async Task<IList<BuildData>> GetBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      BuildQueryOrder buildQueryOrder = queryOrder;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? nullable1;
      if (!local1.HasValue)
      {
        nullable1 = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        nullable1 = valueOrDefault.MinTime;
      }
      DateTime? nullable2 = nullable1;
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? nullable3;
      if (!local2.HasValue)
      {
        nullable3 = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        nullable3 = valueOrDefault.MaxTime;
      }
      DateTime? nullable4 = nullable3;
      HashSet<int> definitionIds1 = definitionIds;
      DateTime? minFinishTime = nullable2;
      DateTime? maxFinishTime = nullable4;
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable5 = new int?();
      BuildResult? nullable6 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?();
      BuildResult? resultFilter = nullable6;
      int maxBuilds = num;
      int queryOrder1 = (int) buildQueryOrder;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable5;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) definitionIds1, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, (BuildQueryOrder) queryOrder1, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual async Task<IList<BuildData>> GetCompletedBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      BuildQueryOrder buildQueryOrder = queryOrder;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? minFinishTime;
      if (!local1.HasValue)
      {
        minFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        minFinishTime = valueOrDefault.MinTime;
      }
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? maxFinishTime;
      if (!local2.HasValue)
      {
        maxFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        maxFinishTime = valueOrDefault.MaxTime;
      }
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable1 = new int?();
      BuildResult? nullable2 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?(BuildStatus.Completed);
      BuildResult? resultFilter = nullable2;
      int maxBuilds = num;
      int queryOrder1 = (int) buildQueryOrder;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable1;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) null, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, (BuildQueryOrder) queryOrder1, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual async Task<IList<BuildData>> GetCompletedBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      BuildQueryOrder buildQueryOrder = queryOrder;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? nullable1;
      if (!local1.HasValue)
      {
        nullable1 = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        nullable1 = valueOrDefault.MinTime;
      }
      DateTime? nullable2 = nullable1;
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? nullable3;
      if (!local2.HasValue)
      {
        nullable3 = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        nullable3 = valueOrDefault.MaxTime;
      }
      DateTime? nullable4 = nullable3;
      HashSet<int> definitionIds1 = definitionIds;
      DateTime? minFinishTime = nullable2;
      DateTime? maxFinishTime = nullable4;
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable5 = new int?();
      BuildResult? nullable6 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?(BuildStatus.Completed);
      BuildResult? resultFilter = nullable6;
      int maxBuilds = num;
      int queryOrder1 = (int) buildQueryOrder;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable5;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) definitionIds1, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, (BuildQueryOrder) queryOrder1, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual async Task<IList<BuildData>> GetQueuedBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      BuildQueryOrder buildQueryOrder = queryOrder;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? minFinishTime;
      if (!local1.HasValue)
      {
        minFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        minFinishTime = valueOrDefault.MinTime;
      }
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? maxFinishTime;
      if (!local2.HasValue)
      {
        maxFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        maxFinishTime = valueOrDefault.MaxTime;
      }
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable1 = new int?();
      BuildResult? nullable2 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?(BuildStatus.NotStarted);
      BuildResult? resultFilter = nullable2;
      int maxBuilds = num;
      int queryOrder1 = (int) buildQueryOrder;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable1;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) null, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, (BuildQueryOrder) queryOrder1, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual async Task<IList<BuildData>> GetQueuedBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      BuildQueryOrder buildQueryOrder = queryOrder;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? nullable1;
      if (!local1.HasValue)
      {
        nullable1 = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        nullable1 = valueOrDefault.MinTime;
      }
      DateTime? nullable2 = nullable1;
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? nullable3;
      if (!local2.HasValue)
      {
        nullable3 = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        nullable3 = valueOrDefault.MaxTime;
      }
      DateTime? nullable4 = nullable3;
      HashSet<int> definitionIds1 = definitionIds;
      DateTime? minFinishTime = nullable2;
      DateTime? maxFinishTime = nullable4;
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable5 = new int?();
      BuildResult? nullable6 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?(BuildStatus.NotStarted);
      BuildResult? resultFilter = nullable6;
      int maxBuilds = num;
      int queryOrder1 = (int) buildQueryOrder;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable5;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) definitionIds1, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, (BuildQueryOrder) queryOrder1, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual async Task<IList<BuildData>> GetRunningBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      BuildQueryOrder buildQueryOrder = queryOrder;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? minFinishTime;
      if (!local1.HasValue)
      {
        minFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        minFinishTime = valueOrDefault.MinTime;
      }
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? maxFinishTime;
      if (!local2.HasValue)
      {
        maxFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        maxFinishTime = valueOrDefault.MaxTime;
      }
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable1 = new int?();
      BuildResult? nullable2 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?(BuildStatus.InProgress);
      BuildResult? resultFilter = nullable2;
      int maxBuilds = num;
      int queryOrder1 = (int) buildQueryOrder;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable1;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) null, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, (BuildQueryOrder) queryOrder1, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual async Task<IList<BuildData>> GetRunningBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      BuildQueryOrder buildQueryOrder = queryOrder;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? nullable1;
      if (!local1.HasValue)
      {
        nullable1 = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        nullable1 = valueOrDefault.MinTime;
      }
      DateTime? nullable2 = nullable1;
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? nullable3;
      if (!local2.HasValue)
      {
        nullable3 = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        nullable3 = valueOrDefault.MaxTime;
      }
      DateTime? nullable4 = nullable3;
      HashSet<int> definitionIds1 = definitionIds;
      DateTime? minFinishTime = nullable2;
      DateTime? maxFinishTime = nullable4;
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable5 = new int?();
      BuildResult? nullable6 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?(BuildStatus.InProgress);
      BuildResult? resultFilter = nullable6;
      int maxBuilds = num;
      int queryOrder1 = (int) buildQueryOrder;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable5;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) definitionIds1, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, (BuildQueryOrder) queryOrder1, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual IEnumerable<BuildData> GetBuildsByIds(
      IEnumerable<int> buildIds,
      bool includeDeleted)
    {
      this.TraceEnter(0, nameof (GetBuildsByIds));
      this.PrepareStoredProcedure("Build.prc_GetBuildsByIds");
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

    public virtual async Task<IEnumerable<BuildData>> GetBuildsByIdsAsync(
      IEnumerable<int> buildIds,
      bool includeDeleted)
    {
      Build2Component component = this;
      component.TraceEnter(0, nameof (GetBuildsByIdsAsync));
      component.PrepareStoredProcedure("Build.prc_GetBuildsByIds");
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

    public virtual List<BuildForRetention> GetBuildsForRetention(
      Guid projectId,
      int definitionId,
      DateTime minFinishTime,
      DateTime maxFinishTime,
      int maxBuilds)
    {
      this.TraceEnter(0, nameof (GetBuildsForRetention));
      this.PrepareStoredProcedure("Build.prc_GetBuildsForRetention");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.BindDateTime2("@minFinishTime", minFinishTime);
      this.BindDateTime2("@maxFinishTime", maxFinishTime);
      this.BindInt("@maxBuilds", maxBuilds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildForRetention>(this.GetBuildForRetentionBinder());
        List<BuildForRetention> items = resultCollection.GetCurrent<BuildForRetention>().Items;
        this.TraceLeave(0, nameof (GetBuildsForRetention));
        return items;
      }
    }

    public virtual List<ChangeData> GetChanges(
      Guid projectId,
      int buildId,
      int startId,
      bool includeSourceChangeOnly,
      int maxChanges)
    {
      this.TraceEnter(0, nameof (GetChanges));
      this.PrepareStoredProcedure("Build.prc_GetChanges");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindInt("@startId", startId);
      this.BindInt("@maxChanges", maxChanges);
      List<ChangeData> changes = (List<ChangeData>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ChangeData>(this.GetChangeDataBinder());
        changes = resultCollection.GetCurrent<ChangeData>().Items;
      }
      this.TraceLeave(0, nameof (GetChanges));
      return changes;
    }

    public virtual async Task<BuildDefinition> GetDefinitionAsync(
      Guid projectId,
      int definitionId,
      int? definitionVersion,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false)
    {
      Build2Component build2Component = this;
      build2Component.TraceEnter(0, nameof (GetDefinitionAsync));
      build2Component.PrepareStoredProcedure("Build.prc_GetDefinition");
      build2Component.BindInt("@dataspaceId", build2Component.GetDataspaceId(projectId));
      build2Component.BindInt("@definitionId", definitionId);
      build2Component.BindNullableInt32("@definitionVersion", definitionVersion);
      build2Component.BindBoolean("@includeDeleted", includeDeleted);
      build2Component.BindNullableUtcDateTime2("@minMetricsTime", minMetricsTime);
      build2Component.BindBoolean("@includeLatestBuilds", includeLatestBuilds);
      BuildDefinition definitionAsync;
      using (ResultCollection rc = new ResultCollection((IDataReader) await build2Component.ExecuteReaderAsync(), build2Component.ProcedureName, build2Component.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(build2Component.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(build2Component.GetBuildDefinitionMetricBinder());
        rc.AddBinder<BuildData>(build2Component.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(build2Component.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(build2Component.GetBuildOrchestrationDataBinder());
        BuildDefinition definition = rc.GetCurrent<BuildDefinition>().FirstOrDefault<BuildDefinition>();
        rc.NextResult();
        foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
          definition.Metrics.Add(definitionMetric.Metric);
        if (includeLatestBuilds)
          build2Component.BindLatestBuilds(rc, definition);
        build2Component.TraceLeave(0, nameof (GetDefinitionAsync));
        definitionAsync = definition;
      }
      return definitionAsync;
    }

    public virtual List<BuildDefinition> GetDefinitionHistory(Guid projectId, int definitionId)
    {
      this.TraceEnter(0, nameof (GetDefinitionHistory));
      this.PrepareStoredProcedure("Build.prc_GetDefinitionHistory");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        resultCollection.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        List<BuildDefinition> items = resultCollection.GetCurrent<BuildDefinition>().Items;
        Dictionary<int?, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int?>((System.Func<BuildDefinition, int?>) (x => x.Revision));
        resultCollection.NextResult();
        foreach (DefinitionTagData definitionTagData in resultCollection.GetCurrent<DefinitionTagData>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(new int?(definitionTagData.DefinitionVersion), out buildDefinition))
            buildDefinition.Tags.Add(definitionTagData.Tag);
        }
        this.TraceLeave(0, nameof (GetDefinitionHistory));
        return items;
      }
    }

    public virtual IEnumerable<BuildDefinitionMetric> GetDefinitionMetrics(
      Guid projectId,
      IEnumerable<int> definitionIds,
      DateTime minMetricsTime)
    {
      this.TraceEnter(0, nameof (GetDefinitionMetrics));
      this.PrepareStoredProcedure("Build.prc_GetDefinitionMetrics");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindUniqueInt32Table("@definitionIdTable", definitionIds);
      this.BindUtcDateTime2("@minMetricsTime", minMetricsTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        List<BuildDefinitionMetric> items = resultCollection.GetCurrent<BuildDefinitionMetric>().Items;
        this.TraceLeave(0, nameof (GetDefinitionMetrics));
        return (IEnumerable<BuildDefinitionMetric>) items;
      }
    }

    public virtual List<BuildDefinition> GetDefinitions(
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
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
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
        if (includeLatestBuilds)
          this.BindLatestBuilds(rc, dictionary);
        this.TraceLeave(0, nameof (GetDefinitions));
        return items;
      }
    }

    public virtual List<BuildDefinition> GetCIDefinitions(
      HashSet<int> dataspaceIds,
      string repositoryId,
      string repositoryType,
      int maxDefinitions)
    {
      return new List<BuildDefinition>();
    }

    public virtual List<BuildDefinition> GetYamlDefinitionsForRepository(
      HashSet<int> dataspaceIds,
      string repositoryId,
      string repositoryType,
      int maxDefinitions)
    {
      return new List<BuildDefinition>();
    }

    public virtual List<BuildDefinition> GetDefinitionsByIds(
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
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        rc.AddBinder<BuildData>(this.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = rc.GetCurrent<BuildDefinition>().Items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
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
        this.TraceLeave(0, nameof (GetDefinitionsByIds));
        return items;
      }
    }

    public virtual async Task<List<BuildDefinition>> GetDefinitionsByIdsAsync(
      Guid projectId,
      List<int> definitionIds,
      bool includeDeleted,
      DateTime? minMetricsTime,
      bool includeLatestBuilds,
      bool includeDrafts = false)
    {
      Build2Component component = this;
      component.TraceEnter(0, nameof (GetDefinitionsByIdsAsync));
      component.PrepareStoredProcedure("Build.prc_GetDefinitionsByIds");
      component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
      component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
      component.BindBoolean("@includeDeleted", includeDeleted);
      component.BindNullableUtcDateTime2("@minMetricsTime", minMetricsTime);
      component.BindBoolean("@includeLatestBuilds", includeLatestBuilds);
      List<BuildDefinition> definitionsByIdsAsync;
      using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(component.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(component.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(component.GetDefinitionTagDataBinder());
        rc.AddBinder<BuildData>(component.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(component.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = rc.GetCurrent<BuildDefinition>().Items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
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
          component.BindLatestBuilds(rc, dictionary);
        component.TraceLeave(0, nameof (GetDefinitionsByIdsAsync));
        definitionsByIdsAsync = items;
      }
      return definitionsByIdsAsync;
    }

    public virtual Task<List<BuildDefinition>> GetDefinitionsWithSchedulesAsync(Guid projectId) => Task.FromResult<List<BuildDefinition>>(new List<BuildDefinition>());

    public virtual List<BuildDefinition> GetDefinitionsWithTriggers(
      HashSet<int> dataspaceIds,
      string repositoryId,
      string repositoryType,
      int triggerFilter,
      int maxDefinitions)
    {
      return new List<BuildDefinition>();
    }

    public virtual List<BuildDefinitionTemplate> GetDefinitionTemplates(
      Guid projectId,
      string templateId)
    {
      this.TraceEnter(0, nameof (GetDefinitionTemplates));
      this.PrepareStoredProcedure("Build.prc_GetDefinitionTemplates");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@templateId", templateId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinitionTemplate>(this.GetBuildDefinitionTemplateBinder());
        List<BuildDefinitionTemplate> items = resultCollection.GetCurrent<BuildDefinitionTemplate>().Items;
        this.TraceLeave(0, nameof (GetDefinitionTemplates));
        return items;
      }
    }

    public virtual Task<IList<BuildData>> GetDeletedBuilds(
      Guid projectId,
      int? definitionId,
      string folderPath,
      DateTime maxQueueTime,
      int maxBuilds,
      HashSet<int> excludedDefinitionIds)
    {
      return Task.FromResult<IList<BuildData>>((IList<BuildData>) Array.Empty<BuildData>());
    }

    public virtual BuildDefinition GetDeletedDefinition(Guid projectId, int definitionId) => (BuildDefinition) null;

    public virtual Task<List<BuildDefinition>> GetDeletedDefinitionsAsync(
      Guid projectId,
      DefinitionQueryOrder queryOrder,
      int maxDefinitions)
    {
      return Task.FromResult<List<BuildDefinition>>(new List<BuildDefinition>());
    }

    public virtual IList<Folder> GetFolders(
      Guid projectId,
      string path,
      FolderQueryOrder queryOrder)
    {
      this.TraceEnter(0, nameof (GetFolders));
      this.PrepareStoredProcedure("Build.prc_GetFolders");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@path", DBHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@queryOrder", (int) queryOrder);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Folder>(this.GetFolderBinder());
        List<Folder> items = resultCollection.GetCurrent<Folder>().Items;
        this.TraceLeave(0, nameof (GetFolders));
        return (IList<Folder>) items;
      }
    }

    public virtual IEnumerable<BuildMetric> GetProjectMetrics(
      Guid projectId,
      DateTime minMetricsTime)
    {
      this.TraceEnter(0, nameof (GetProjectMetrics));
      this.PrepareStoredProcedure("Build.prc_GetProjectMetrics");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindUtcDateTime2("@minMetricsTime", minMetricsTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildMetric>(this.GetBuildMetricBinder());
        List<BuildMetric> items = resultCollection.GetCurrent<BuildMetric>().Items;
        this.TraceLeave(0, nameof (GetProjectMetrics));
        return (IEnumerable<BuildMetric>) items;
      }
    }

    public virtual List<AgentPoolQueue> GetQueues(IEnumerable<int> queueIds)
    {
      this.TraceEnter(0, nameof (GetQueues));
      this.PrepareStoredProcedure("Build.prc_GetQueuesById");
      this.BindInt32Table("@queueIds", queueIds);
      List<AgentPoolQueue> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AgentPoolQueue>(this.GetAgentPoolQueueBinder());
        items = resultCollection.GetCurrent<AgentPoolQueue>().Items;
      }
      this.TraceLeave(0, nameof (GetQueues));
      return items;
    }

    public virtual List<AgentPoolQueue> GetQueues(string name)
    {
      this.TraceEnter(0, nameof (GetQueues));
      this.PrepareStoredProcedure("Build.prc_GetQueues");
      this.BindString("@queueName", DBHelper.ServerPathToDBPath(name), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      List<AgentPoolQueue> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AgentPoolQueue>(this.GetAgentPoolQueueBinder());
        items = resultCollection.GetCurrent<AgentPoolQueue>().Items;
      }
      this.TraceLeave(0, nameof (GetQueues));
      return items;
    }

    public virtual IEnumerable<AgentPoolQueue> GetQueuesByPoolId(IEnumerable<int> poolIds)
    {
      this.TraceEnter(0, nameof (GetQueuesByPoolId));
      this.PrepareStoredProcedure("Build.prc_GetQueuesByPoolId");
      this.BindInt32Table("@poolIds", poolIds);
      List<AgentPoolQueue> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AgentPoolQueue>(this.GetAgentPoolQueueBinder());
        items = resultCollection.GetCurrent<AgentPoolQueue>().Items;
      }
      this.TraceLeave(0, nameof (GetQueuesByPoolId));
      return (IEnumerable<AgentPoolQueue>) items;
    }

    public virtual List<BuildDefinition> GetRecentlyBuiltDefinitions(
      Guid projectId,
      int top,
      bool includeQueuedBuilds)
    {
      return new List<BuildDefinition>();
    }

    public virtual Task<IList<RepositoryBranchReferences>> GetRecentlyBuiltRepositories(
      Guid projectId,
      int topRepositories,
      int topBranches,
      HashSet<int> excludedRepositoryIds,
      HashSet<int> excludedDefinitionIds)
    {
      return Task.FromResult<IList<RepositoryBranchReferences>>((IList<RepositoryBranchReferences>) Array.Empty<RepositoryBranchReferences>());
    }

    public virtual Task<IList<RepositoryBranchReferences>> GetRecentlyBuiltBranchesForRepositories(
      Guid projectId,
      int maxBranches,
      IEnumerable<string> repositoryIdentifiers,
      HashSet<int> excludedRepositoryIds)
    {
      return Task.FromResult<IList<RepositoryBranchReferences>>((IList<RepositoryBranchReferences>) Array.Empty<RepositoryBranchReferences>());
    }

    public virtual Task<IList<Guid>> GetRecentlyBuiltRequestedForIdentities(
      Guid projectId,
      int maxCount,
      HashSet<Guid> excludedIds,
      HashSet<int> excludedDefinitionIds)
    {
      return Task.FromResult<IList<Guid>>((IList<Guid>) Array.Empty<Guid>());
    }

    public virtual BuildDefinition GetRenamedDefinition(Guid projectId, string name, string path) => (BuildDefinition) null;

    public virtual Task<RetentionLease> GetRetentionLeaseById(Guid projectId, int leaseId) => Task.FromResult<RetentionLease>((RetentionLease) null);

    public virtual Task<IReadOnlyList<RetentionLease>> GetRetentionLeasesByIds(
      Guid projectId,
      HashSet<int> leaseId)
    {
      return Task.FromResult<IReadOnlyList<RetentionLease>>((IReadOnlyList<RetentionLease>) Array.Empty<RetentionLease>());
    }

    public virtual Task<IReadOnlyList<RetentionLease>> GetRetentionLeases(
      Guid projectId,
      IEnumerable<MinimalRetentionLease> leases,
      bool useOptimizedSelect = false)
    {
      return Task.FromResult<IReadOnlyList<RetentionLease>>((IReadOnlyList<RetentionLease>) Array.Empty<RetentionLease>());
    }

    public virtual Task<IReadOnlyList<RetentionLease>> GetRetentionLeasesForRuns(
      Guid projectId,
      HashSet<int> runIds)
    {
      return Task.FromResult<IReadOnlyList<RetentionLease>>((IReadOnlyList<RetentionLease>) Array.Empty<RetentionLease>());
    }

    public virtual List<BuildDefinition> GetMyRecentlyBuiltDefinitions(
      Guid projectId,
      Guid requestedFor,
      DateTime minFinishTime,
      IEnumerable<int> excludedDefinitionIds,
      int top)
    {
      return new List<BuildDefinition>();
    }

    public virtual IList<BuildData> GetRunnableGatedBuilds()
    {
      this.TraceEnter(0, nameof (GetRunnableGatedBuilds));
      this.PrepareStoredProcedure("Build.prc_GetRunnableGatedBuilds");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
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
        this.TraceLeave(0, nameof (GetRunnableGatedBuilds));
        return (IList<BuildData>) items;
      }
    }

    public virtual IList<BuildData> GetRunnablePausedBuilds(int maxSelected) => (IList<BuildData>) Array.Empty<BuildData>();

    public virtual IEnumerable<string> GetTags(Guid projectId)
    {
      this.TraceEnter(0, nameof (GetTags));
      this.PrepareStoredProcedure("Build.prc_GetTags");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new StringBinder());
        List<string> items = resultCollection.GetCurrent<string>().Items;
        this.TraceLeave(0, nameof (GetTags));
        return (IEnumerable<string>) items;
      }
    }

    public virtual IEnumerable<BuildTagFilter> GetFilterBuildTags(Guid projectId, int? definitionId) => Enumerable.Empty<BuildTagFilter>();

    public virtual IEnumerable<int> GetVeryOldDeletedDefinitionIds(Guid projectId) => Enumerable.Empty<int>();

    public virtual Task<PurgedBuildsResults> PurgeArtifactsAsync(
      Guid projectId,
      int daysOld,
      int batchSize)
    {
      return Task.FromResult<PurgedBuildsResults>(new PurgedBuildsResults());
    }

    public virtual Task<PurgedBuildsResults> PurgeBuildsAsync(
      Guid projectId,
      int daysOld,
      int batchSize,
      string branchPrefix)
    {
      return Task.FromResult<PurgedBuildsResults>(new PurgedBuildsResults());
    }

    public virtual void UpdateCronSchedules(
      BuildDefinition definition,
      string branchName,
      List<PipelineSchedule> schedules,
      out List<CronSchedule> deletedSchedules,
      out List<CronSchedule> addedSchedules)
    {
      deletedSchedules = new List<CronSchedule>();
      addedSchedules = new List<CronSchedule>();
    }

    public virtual List<CronSchedule> GetCronSchedules(Guid projectId, Guid scheduleJobId = default (Guid)) => new List<CronSchedule>();

    protected string ProcessToSteps(BuildProcess process)
    {
      string str = (string) null;
      return process is DesignerProcess designerProcess && designerProcess.Phases.Count > 0 ? this.ToString<BuildDefinitionStep>((IList<BuildDefinitionStep>) designerProcess.Phases[0].Steps) : str;
    }

    public virtual BuildData QueueBuild(
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

    protected byte? RepositoryTypeStringToByte(string repositoryTypeString)
    {
      if (string.Equals(repositoryTypeString, "TfsVersionControl", StringComparison.OrdinalIgnoreCase))
        return new byte?((byte) 0);
      if (string.Equals(repositoryTypeString, "TfsGit", StringComparison.OrdinalIgnoreCase))
        return new byte?((byte) 1);
      return string.Equals(repositoryTypeString, "Git", StringComparison.OrdinalIgnoreCase) ? new byte?((byte) 2) : new byte?();
    }

    public virtual void ResetDefinitionCounter(Guid projectId, int definitionId, int counterId)
    {
    }

    public virtual BuildDefinition RestoreDefinition(
      Guid projectId,
      int definitionId,
      Guid authorId,
      string comment = null)
    {
      return (BuildDefinition) null;
    }

    public virtual BuildDefinitionTemplate SaveDefinitionTemplate(
      Guid projectId,
      BuildDefinitionTemplate template)
    {
      this.TraceEnter(0, nameof (SaveDefinitionTemplate));
      this.PrepareStoredProcedure("Build.prc_SaveDefinitionTemplate");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@templateId", template.Id, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@name", template.Name, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", template.Description, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@template", JsonUtility.ToString((object) template.Template), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinitionTemplate>(this.GetBuildDefinitionTemplateBinder());
        BuildDefinitionTemplate definitionTemplate = resultCollection.GetCurrent<BuildDefinitionTemplate>().FirstOrDefault<BuildDefinitionTemplate>();
        this.TraceLeave(0, nameof (SaveDefinitionTemplate));
        return definitionTemplate;
      }
    }

    public virtual List<ChangeData> StoreChanges(
      Guid projectId,
      int buildId,
      IEnumerable<ChangeData> changes)
    {
      this.TraceEnter(0, nameof (StoreChanges));
      this.PrepareStoredProcedure("Build.prc_StoreChanges");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindStringTable("@changeDescriptors", changes != null ? changes.Select<ChangeData, string>((System.Func<ChangeData, string>) (c => c.Descriptor)) : (IEnumerable<string>) null);
      List<ChangeData> changeDataList = (List<ChangeData>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ChangeData>(this.GetChangeDataBinder());
        changeDataList = resultCollection.GetCurrent<ChangeData>().Items;
      }
      this.TraceLeave(0, nameof (StoreChanges));
      return changeDataList;
    }

    public virtual async Task<IList<BuildData>> GetAllBuildsUnderFolderAsync(
      Guid projectId,
      string folderPath,
      TimeFilter? timeFilter,
      int count,
      HashSet<int> excludedDefinitionIds)
    {
      Guid projectId1 = projectId;
      ref TimeFilter? local1 = ref timeFilter;
      TimeFilter valueOrDefault;
      DateTime? minFinishTime;
      if (!local1.HasValue)
      {
        minFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        minFinishTime = valueOrDefault.MinTime;
      }
      ref TimeFilter? local2 = ref timeFilter;
      DateTime? maxFinishTime;
      if (!local2.HasValue)
      {
        maxFinishTime = new DateTime?();
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        maxFinishTime = valueOrDefault.MaxTime;
      }
      BuildReason? reasonFilter = new BuildReason?();
      int num = count;
      IList<int> list = (IList<int>) excludedDefinitionIds.ToList<int>();
      int? nullable1 = new int?();
      BuildResult? nullable2 = new BuildResult?();
      BuildStatus? statusFilter = new BuildStatus?();
      BuildResult? resultFilter = nullable2;
      int maxBuilds = num;
      IList<int> excludedDefinitionIds1 = list;
      int? maxBuildsPerDefinition = nullable1;
      return await Task.FromResult<IList<BuildData>>((IList<BuildData>) this.GetBuilds(projectId1, (IEnumerable<int>) null, (IEnumerable<int>) null, "*", minFinishTime, maxFinishTime, (IEnumerable<Guid>) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, maxBuilds, QueryDeletedOption.ExcludeDeleted, BuildQueryOrder.Descending, excludedDefinitionIds1, (string) null, (string) null, (string) null, maxBuildsPerDefinition).ToList<BuildData>());
    }

    public virtual Task<ILookup<Guid, long>> GetOrphanedBuildContainersAsync(
      DateTime minDate,
      int maxContainers)
    {
      return Task.FromResult<ILookup<Guid, long>>(Enumerable.Empty<long>().ToLookup<long, Guid>((System.Func<long, Guid>) (_ => new Guid())));
    }

    public virtual Task<UpdateBuildsResult> ResetBuildStateAsync(
      Guid projectId,
      int buildId,
      Guid requestedBy)
    {
      return Task.FromResult<UpdateBuildsResult>(new UpdateBuildsResult());
    }

    public virtual bool SupportsExcludedDefinitions() => true;

    public virtual void UpdateDefinitionRepositoryMappings(
      Dictionary<BuildDefinition, List<BuildRepository>> definitionRepositoryMaps,
      out List<BuildDefinitionRepositoryMap> deletedDefRepoPairs,
      out List<BuildDefinitionRepositoryMap> addedDefRepoPairs)
    {
      deletedDefRepoPairs = new List<BuildDefinitionRepositoryMap>();
      addedDefRepoPairs = new List<BuildDefinitionRepositoryMap>();
    }

    public virtual BuildData UpdateBuild(
      BuildData build,
      Guid changedBy,
      out BuildData oldBuild,
      out BuildDefinition buildDefinition)
    {
      this.TraceEnter(0, nameof (UpdateBuild));
      IList<BuildData> oldBuilds;
      IDictionary<int, BuildDefinition> definitionsById;
      BuildData buildData = this.UpdateBuilds(build.ProjectId, (IEnumerable<BuildData>) new BuildData[1]
      {
        build
      }, changedBy, out oldBuilds, out definitionsById).FirstOrDefault<BuildData>();
      oldBuild = oldBuilds.FirstOrDefault<BuildData>();
      buildDefinition = definitionsById.Values.FirstOrDefault<BuildDefinition>();
      this.TraceLeave(0, nameof (UpdateBuild));
      return buildData;
    }

    public virtual Task<UpdateBuildsResult> UpdateBuildAsync(BuildData build, Guid changedBy) => this.UpdateBuildsAsync(build.ProjectId, (IEnumerable<BuildData>) new BuildData[1]
    {
      build
    }, changedBy);

    public virtual List<BuildData> UpdateBuilds(
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

    public virtual async Task<UpdateBuildsResult> UpdateBuildsAsync(
      Guid projectId,
      IEnumerable<BuildData> builds,
      Guid changedBy)
    {
      Build2Component build2Component = this;
      build2Component.TraceEnter(0, nameof (UpdateBuildsAsync));
      build2Component.PrepareStoredProcedure("Build.prc_UpdateBuilds");
      build2Component.BindInt("@dataspaceId", build2Component.GetDataspaceId(projectId));
      build2Component.BindBuildUpdateTable("@buildUpdateTable", builds);
      build2Component.BindGuid("@requestedBy", changedBy);
      UpdateBuildsResult result = new UpdateBuildsResult();
      UpdateBuildsResult updateBuildsResult;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component.ExecuteReaderAsync(), build2Component.ProcedureName, build2Component.RequestContext))
      {
        resultCollection.AddBinder<BuildData>(build2Component.GetBuildDataBinder());
        resultCollection.AddBinder<BuildData>(build2Component.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(build2Component.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(build2Component.GetBuildOrchestrationDataBinder());
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
        build2Component.TraceLeave(0, nameof (UpdateBuildsAsync));
        updateBuildsResult = result;
      }
      result = new UpdateBuildsResult();
      return updateBuildsResult;
    }

    public virtual BuildDefinition UpdateDefinition(
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
      this.BindString("@options", this.ToString<BuildOption>((IList<BuildOption>) definition.Options), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repository", JsonUtility.ToString((object) definition.Repository), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@triggers", this.ToString<BuildTrigger>((IList<BuildTrigger>) definition.Triggers), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@steps", this.ProcessToSteps(definition.Process), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@variables", this.ToString<string, BuildDefinitionVariable>((IDictionary<string, BuildDefinitionVariable>) definition.Variables), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@demands", this.ToString<Demand>((IList<Demand>) definition.Demands), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@retentionPolicy", (string) null, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@description", definition.Description, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@buildNumberFormat", definition.BuildNumberFormat, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@jobAuthorizationScope", (byte) definition.JobAuthorizationScope);
      this.BindNullableInt("@jobTimeout", definition.JobTimeoutInMinutes, 0);
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

    public virtual IEnumerable<BuildDefinitionBranch> UpdateDefinitionBranches(
      Guid projectId,
      int definitionId,
      int definitionVersion,
      IEnumerable<BuildDefinitionBranch> branches,
      int maxConcurrentBuildsPerBranch,
      bool ignoreSourceIdCheck)
    {
      this.TraceEnter(0, nameof (UpdateDefinitionBranches));
      this.PrepareStoredProcedure("Build.prc_UpdateDefinitionBranches");
      int dataspaceId = this.GetDataspaceId(projectId);
      this.BindInt("@dataspaceId", dataspaceId);
      this.BindInt("@definitionId", definitionId);
      this.BindInt("@definitionVersion", definitionVersion);
      this.BindBuildDefinitionBranchTable2("@branches", branches);
      this.BindInt("@maxConcurrentBuildsPerBranch", maxConcurrentBuildsPerBranch);
      this.Trace(12030298, TraceLevel.Info, "Build2", (object) nameof (Build2Component), (object) ("calling prc_UpdateDefinitionBranches with the following params: " + JsonConvert.SerializeObject((object) new Dictionary<string, object>()
      {
        ["dataspaceId"] = (object) dataspaceId,
        [nameof (definitionId)] = (object) definitionId,
        [nameof (definitionVersion)] = (object) definitionVersion,
        [nameof (maxConcurrentBuildsPerBranch)] = (object) maxConcurrentBuildsPerBranch
      })));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinitionBranch>(this.GetBuildDefinitionBranchBinder());
        List<BuildDefinitionBranch> items = resultCollection.GetCurrent<BuildDefinitionBranch>().Items;
        this.TraceLeave(0, nameof (UpdateDefinitionBranches));
        return (IEnumerable<BuildDefinitionBranch>) items;
      }
    }

    public virtual Task UpdateDefinitionBuildOptionsAsync(BuildDefinition definition) => throw new NotImplementedException("This feature is not available for the versions less than and including 63.");

    public virtual void UpdateDefinitionCounterSeed(
      Guid projectId,
      int definitionId,
      int counterId,
      long newSeed,
      bool resetValue)
    {
    }

    public virtual IEnumerable<BuildDefinitionMetric> UpdateDefinitionMetrics(
      Guid projectId,
      IEnumerable<BuildDefinitionMetric> definitionMetrics)
    {
      this.TraceEnter(0, nameof (UpdateDefinitionMetrics));
      this.PrepareStoredProcedure("Build.prc_UpdateDefinitionMetrics");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindBuildDefinitionMetricUpdateTable("@metricUpdateTable", definitionMetrics);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        List<BuildDefinitionMetric> items = resultCollection.GetCurrent<BuildDefinitionMetric>().Items;
        this.TraceLeave(0, nameof (UpdateDefinitionMetrics));
        return (IEnumerable<BuildDefinitionMetric>) items;
      }
    }

    public virtual Folder UpdateFolder(
      string path,
      Folder folder,
      Guid requestedBy,
      string originalSecurityToken,
      string newSecurityToken)
    {
      this.TraceEnter(0, nameof (UpdateFolder));
      this.PrepareStoredProcedure("Build.prc_UpdateFolder");
      this.BindInt("@dataspaceId", this.GetDataspaceId(folder.ProjectId));
      this.BindString("@oldPath", DBHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@newPath", DBHelper.UserToDBPath(folder.Path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", folder.Description, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@requestedBy", requestedBy);
      this.BindString("@originalSecurityToken", originalSecurityToken, 435, false, SqlDbType.NVarChar);
      this.BindString("@newSecurityToken", newSecurityToken, 435, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Folder>(this.GetFolderBinder());
        Folder folder1 = resultCollection.GetCurrent<Folder>().FirstOrDefault<Folder>();
        this.TraceLeave(0, nameof (UpdateFolder));
        return folder1;
      }
    }

    public virtual void UpdateGatedBuildQueue(IEnumerable<BuildData> queuedBuilds)
    {
      this.TraceEnter(0, nameof (UpdateGatedBuildQueue));
      this.PrepareStoredProcedure("Build.prc_UpdateGatedBuildQueue");
      this.BindKeyValuePairInt32Int32Table("@queuedBuilds", (IEnumerable<KeyValuePair<int, int>>) queuedBuilds.Select<BuildData, KeyValuePair<int, int>>((System.Func<BuildData, KeyValuePair<int, int>>) (x => new KeyValuePair<int, int>(this.GetDataspaceId(x.ProjectId), x.Id))).ToList<KeyValuePair<int, int>>());
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (UpdateGatedBuildQueue));
    }

    public virtual void UpdatePausedBuildQueue(IEnumerable<BuildData> queuedBuilds)
    {
    }

    public virtual IEnumerable<BuildCompletionTriggerCandidate> GetBuildCompletionTriggerCandidates(
      TriggeredByBuild buildCompletionTrigger,
      BuildResult result)
    {
      return (IEnumerable<BuildCompletionTriggerCandidate>) null;
    }

    public virtual BuildResult? GetBranchStatus(
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName)
    {
      return this.GetBuilds(projectId, (IEnumerable<int>) new int[1]
      {
        definitionId
      }, (IEnumerable<int>) new HashSet<int>(), "*", new DateTime?(), new DateTime?(), (IEnumerable<Guid>) new HashSet<Guid>(), new BuildReason?(), new BuildStatus?(BuildStatus.Completed), new BuildResult?(), (IEnumerable<string>) null, 1, QueryDeletedOption.ExcludeDeleted, BuildQueryOrder.Descending, (IList<int>) null, repositoryId, repositoryType, branchName, new int?()).SingleOrDefault<BuildData>()?.Result;
    }

    public virtual BuildData GetLatestBuildForBranch(
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName)
    {
      return this.GetBuilds(projectId, (IEnumerable<int>) new int[1]
      {
        definitionId
      }, (IEnumerable<int>) new HashSet<int>(), "*", new DateTime?(), new DateTime?(), (IEnumerable<Guid>) new HashSet<Guid>(), new BuildReason?(), new BuildStatus?(BuildStatus.Completed), new BuildResult?(), (IEnumerable<string>) null, 1, QueryDeletedOption.ExcludeDeleted, BuildQueryOrder.Descending, (IList<int>) null, repositoryId, repositoryType, branchName, new int?()).SingleOrDefault<BuildData>();
    }

    public virtual Task<BuildData> GetLatestCompletedBuildAsync(
      Guid projectId,
      string repositoryIdentifier,
      string repositoryType,
      string branchName)
    {
      return Task.FromResult<BuildData>(this.GetBuilds(projectId, (IEnumerable<int>) null, (IEnumerable<int>) new HashSet<int>(), "*", new DateTime?(), new DateTime?(), (IEnumerable<Guid>) null, new BuildReason?(), new BuildStatus?(BuildStatus.Completed), new BuildResult?(), (IEnumerable<string>) null, 1, QueryDeletedOption.ExcludeDeleted, BuildQueryOrder.Descending, (IList<int>) null, repositoryIdentifier, repositoryType, branchName, new int?()).FirstOrDefault<BuildData>());
    }

    public virtual BuildData GetLatestSuccessfulBuildForBranch(
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName,
      DateTime? maxFinishTime)
    {
      return this.GetBuilds(projectId, (IEnumerable<int>) new int[1]
      {
        definitionId
      }, (IEnumerable<int>) new HashSet<int>(), "*", new DateTime?(), maxFinishTime, (IEnumerable<Guid>) new HashSet<Guid>(), new BuildReason?(), new BuildStatus?(BuildStatus.Completed), new BuildResult?(BuildResult.Succeeded), (IEnumerable<string>) null, 1, QueryDeletedOption.IncludeDeleted, BuildQueryOrder.Descending, (IList<int>) null, repositoryId, repositoryType, branchName, new int?()).FirstOrDefault<BuildData>();
    }

    public virtual Task<List<BuildAnalyticsData>> GetBuildsByDateAsync(
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      return Task.FromResult<List<BuildAnalyticsData>>(new List<BuildAnalyticsData>());
    }

    public virtual Task<List<ShallowBuildAnalyticsData>> GetShallowBuildAnaltyticsDataByDateAsync(
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      throw new NotImplementedException("This feature is not available for the versions below 62(M152)");
    }

    public virtual Task<List<BuildDefinitionAnalyticsData>> GetBuildDefinitionsByDateAsync(
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      return Task.FromResult<List<BuildDefinitionAnalyticsData>>(new List<BuildDefinitionAnalyticsData>());
    }

    public virtual Task<IList<BranchesViewItem>> GetBranchesViewAsync(
      Guid projectId,
      int definitionId,
      int repositoryId,
      string defaultBranch,
      DateTime minQueueTime,
      int maxBranches,
      int buildsPerBranch)
    {
      return Task.FromResult<IList<BranchesViewItem>>((IList<BranchesViewItem>) new List<BranchesViewItem>());
    }

    public virtual int GetScheduledBuildsCount(
      Guid projectId,
      IEnumerable<int> definitionIds,
      IList<int> excludedDefinitionIds,
      bool includeDeleted,
      DateTime minQueueTime,
      DateTime? maxQueueTime,
      BuildStatus? statusFilter,
      BuildResult? resultFilter)
    {
      return 0;
    }

    public virtual Task<int> DeleteDefinitionCronSchedules(BuildDefinition definition) => Task.FromResult<int>(0);

    public virtual Task<List<CronSchedule>> DeleteCronSchedulesForDefinitions(
      Guid projectId,
      List<int> definitionIds)
    {
      return Task.FromResult<List<CronSchedule>>(new List<CronSchedule>());
    }

    public virtual Task<IList<RepositoryBranchReferences>> GetBranchesByIdAsync(
      Guid projectId,
      List<int> branchIds)
    {
      return Task.FromResult<IList<RepositoryBranchReferences>>((IList<RepositoryBranchReferences>) Array.Empty<RepositoryBranchReferences>());
    }

    public virtual BuildCheckEvent AddCheckEvent(
      BuildCheckEvent checkEvent,
      bool securityFixEnabled = false)
    {
      return (BuildCheckEvent) null;
    }

    public virtual List<BuildCheckEvent> AddCheckEvents(
      List<BuildCheckEvent> checkEvents,
      bool securityFixEnabled = false)
    {
      return checkEvents.Select<BuildCheckEvent, BuildCheckEvent>((System.Func<BuildCheckEvent, BuildCheckEvent>) (x => this.AddCheckEvent(x, securityFixEnabled))).Where<BuildCheckEvent>((System.Func<BuildCheckEvent, bool>) (x => x != null)).ToList<BuildCheckEvent>();
    }

    public virtual CheckEventResults GetCheckEvents(int? maxEvents, bool securityFixEnabled = false) => (CheckEventResults) null;

    public virtual List<BuildCheckEvent> UpdateCheckEvents(
      IEnumerable<BuildCheckEvent> events,
      bool securityFixEnabled = false)
    {
      return (List<BuildCheckEvent>) null;
    }

    public virtual void DeleteCheckEvents(
      CheckEventStatus status,
      DateTime minCreatedTime,
      int? batchSize)
    {
    }

    public virtual IList<Guid> GetAllServiceConnectionsForRepoAndProject(
      Guid projectId,
      string repoId,
      string repoType,
      int triggerFilter)
    {
      return (IList<Guid>) new List<Guid>();
    }

    public virtual Task<RetentionLease> UpdateRetentionLease(
      Guid projectId,
      int leaseId,
      DateTime? validUntil,
      bool? highPriority)
    {
      return Task.FromResult<RetentionLease>((RetentionLease) null);
    }

    public virtual Task<IList<ArtifactCleanupRecord>> GetArtifactsToCleanUp() => Task.FromResult<IList<ArtifactCleanupRecord>>((IList<ArtifactCleanupRecord>) Array.Empty<ArtifactCleanupRecord>());

    public virtual Task DeleteArtifactCleanupRecords(IList<ArtifactCleanupRecordKey> artifactRecords) => Task.CompletedTask;

    public virtual IList<PoisonedBuild> GetPoisonedBuilds(int batchSize) => (IList<PoisonedBuild>) Array.Empty<PoisonedBuild>();

    protected virtual ObjectBinder<BuildArtifact> GetBuildArtifactBinder() => (ObjectBinder<BuildArtifact>) new BuildArtifactBinder4(this.RequestContext);

    protected virtual ObjectBinder<BuildData> GetBuildDataBinder() => (ObjectBinder<BuildData>) new BuildDataBinder12(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual ObjectBinder<BuildResult?> GetBuildResultBinder() => (ObjectBinder<BuildResult?>) new BuildResultBinder(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual ObjectBinder<BuildTagData> GetBuildTagDataBinder() => (ObjectBinder<BuildTagData>) new BuildTagBinder();

    protected virtual ObjectBinder<BuildTagFilter> GetBuildTagFilterBinder() => (ObjectBinder<BuildTagFilter>) new BuildTagFilterBinder((BuildSqlComponentBase) this);

    protected virtual ObjectBinder<BuildOrchestrationData> GetBuildOrchestrationDataBinder() => (ObjectBinder<BuildOrchestrationData>) new BuildOrchestrationDataBinder();

    protected virtual ObjectBinder<BuildDefinition> GetBuildDefinitionBinder() => (ObjectBinder<BuildDefinition>) new BuildDefinitionBinder10(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual ObjectBinder<DefinitionTagData> GetDefinitionTagDataBinder() => (ObjectBinder<DefinitionTagData>) new DefinitionTagBinder();

    protected virtual ObjectBinder<TagStringData> GetTagStringBinder() => (ObjectBinder<TagStringData>) new TagStringBinder();

    protected virtual ObjectBinder<Folder> GetFolderBinder() => (ObjectBinder<Folder>) new FolderBinder((BuildSqlComponentBase) this);

    protected virtual ObjectBinder<AgentPoolQueue> GetAgentPoolQueueBinder() => (ObjectBinder<AgentPoolQueue>) new AgentPoolQueueBinder();

    protected virtual ObjectBinder<BuildDefinitionBranch> GetBuildDefinitionBranchBinder() => (ObjectBinder<BuildDefinitionBranch>) new BuildDefinitionBranchBinder2(this.RequestContext);

    protected virtual ObjectBinder<BuildForRetention> GetBuildForRetentionBinder() => (ObjectBinder<BuildForRetention>) new BuildForRetentionBinder3(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual ObjectBinder<ChangeData> GetChangeDataBinder() => (ObjectBinder<ChangeData>) new ChangeDataBinder();

    protected virtual ObjectBinder<BuildDefinitionMetric> GetBuildDefinitionMetricBinder() => (ObjectBinder<BuildDefinitionMetric>) new BuildDefinitionMetricBinder2();

    protected virtual ObjectBinder<BuildDefinitionForRetention> GetBuildDefinitionForRetentionBinder() => (ObjectBinder<BuildDefinitionForRetention>) new BuildDefinitionForRetentionBinder3(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual ObjectBinder<BuildDefinitionTemplate> GetBuildDefinitionTemplateBinder() => (ObjectBinder<BuildDefinitionTemplate>) new BuildDefinitionTemplateBinder(this.RequestContext);

    protected virtual ObjectBinder<BuildMetric> GetBuildMetricBinder() => (ObjectBinder<BuildMetric>) new BuildMetricBinder();

    protected virtual ObjectBinder<OrphanedBuildContainer> GetOrphanedBuildContainerBinder() => (ObjectBinder<OrphanedBuildContainer>) new OrphanedBuildContainerBinder();

    protected string ToString<SourceVersionInfo>(SourceVersionInfo info)
    {
      string a = JsonUtility.ToString((object) info);
      return string.Equals(a, "{}", StringComparison.Ordinal) ? (string) null : a;
    }

    protected string SerializeTemplateParameters(BuildData build)
    {
      if (build.TemplateParameters.Count == 0)
        return (string) null;
      return JsonUtility.ToString((object) new Dictionary<string, Dictionary<string, object>>()
      {
        {
          "TemplateParameters",
          build.TemplateParameters
        }
      });
    }

    protected virtual void BindAppendCommitMessageToRunName(bool appendCommitMessageToRunName)
    {
    }

    public virtual int UpdatePermissionsToEditQueueVars(int batchSize) => 0;
  }
}
