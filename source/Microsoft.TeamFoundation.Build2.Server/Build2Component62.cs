// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component62
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
  internal class Build2Component62 : Build2Component61
  {
    public override async Task<IList<BuildData>> FilterBuildsByTagsAsync(
      Guid projectId,
      int? definitionId,
      string folderPath,
      HashSet<string> tagFilter,
      TimeFilter? timeFilter,
      int maxBuilds,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component62 component = this;
      IList<BuildData> buildDataList;
      using (component.TraceScope(method: nameof (FilterBuildsByTagsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_FilterBuildsByTags");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindNullableInt("@definitionId", definitionId);
        component.BindRecursiveFolderPath("@folderPath", folderPath);
        component.BindStringTable("@tagFilters", (IEnumerable<string>) tagFilter);
        Build2Component62 build2Component62_1 = component;
        ref TimeFilter? local1 = ref timeFilter;
        TimeFilter valueOrDefault;
        DateTime? parameterValue1;
        if (!local1.HasValue)
        {
          parameterValue1 = new DateTime?();
        }
        else
        {
          valueOrDefault = local1.GetValueOrDefault();
          parameterValue1 = valueOrDefault.MinTime;
        }
        build2Component62_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component62 build2Component62_2 = component;
        ref TimeFilter? local2 = ref timeFilter;
        DateTime? parameterValue2;
        if (!local2.HasValue)
        {
          parameterValue2 = new DateTime?();
        }
        else
        {
          valueOrDefault = local2.GetValueOrDefault();
          parameterValue2 = valueOrDefault.MaxTime;
        }
        build2Component62_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindInt("@maxBuilds", maxBuilds);
        component.BindUniqueInt32Table("@excludedDefinitionIds", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
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
          ObjectBinder<BuildOrchestrationData> current = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current);
          buildDataList = (IList<BuildData>) items;
        }
      }
      return buildDataList;
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
  }
}
