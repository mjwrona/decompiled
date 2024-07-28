// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component46
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component46 : Build2Component45
  {
    public override async Task<IList<BuildData>> GetAllBuildsUnderFolderAsync(
      Guid projectId,
      string folderPath,
      TimeFilter? timeFilter,
      int count,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component46 component = this;
      IList<BuildData> underFolderAsync;
      using (component.TraceScope(method: nameof (GetAllBuildsUnderFolderAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetAllBuildsUnderFolder");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindRecursiveFolderPath("@folderPath", folderPath);
        Build2Component46 build2Component46_1 = component;
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
        build2Component46_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component46 build2Component46_2 = component;
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
        build2Component46_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindInt("@count", count);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        List<BuildData> items;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
        }
        underFolderAsync = (IList<BuildData>) items;
      }
      return underFolderAsync;
    }

    public override BuildResult? GetBranchStatus(
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName)
    {
      this.TraceEnter(0, nameof (GetBranchStatus));
      this.PrepareStoredProcedure("Build.prc_GetBranchStatus");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@definitionId", definitionId);
      this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@branchName", branchName, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildResult?>(this.GetBuildResultBinder());
        return resultCollection.GetCurrent<BuildResult?>().SingleOrDefault<BuildResult?>();
      }
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
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        resultCollection.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        resultCollection.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
        resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> items = resultCollection.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
        resultCollection.NextResult();
        foreach (BuildDefinitionMetric definitionMetric in resultCollection.GetCurrent<BuildDefinitionMetric>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionMetric.DefinitionId, out buildDefinition))
            buildDefinition.Metrics.Add(definitionMetric.Metric);
        }
        resultCollection.NextResult();
        foreach (DefinitionTagData definitionTagData in resultCollection.GetCurrent<DefinitionTagData>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionTagData.DefinitionId, out buildDefinition))
            buildDefinition.Tags.Add(definitionTagData.Tag);
        }
        resultCollection.NextResult();
        this.TraceLeave(0, nameof (GetDefinitionsWithTriggers));
        return items;
      }
    }

    public override BuildData GetLatestBuildForBranch(
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName)
    {
      using (this.TraceScope(method: nameof (GetLatestBuildForBranch)))
      {
        this.PrepareStoredProcedure("Build.prc_GetLatestBuildForBranch");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@definitionId", definitionId);
        this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@branchName", branchName, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        BuildData buildData;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          buildData = resultCollection.GetCurrent<BuildData>().SingleOrDefault<BuildData>();
          if (buildData != null)
          {
            resultCollection.NextResult();
            buildData.Tags.AddRange(resultCollection.GetCurrent<BuildTagData>().Where<BuildTagData>((System.Func<BuildTagData, bool>) (bd => bd.BuildId == buildData.Id)).Select<BuildTagData, string>((System.Func<BuildTagData, string>) (bd => bd.Tag)));
            resultCollection.NextResult();
            buildData.Plans.AddRange(resultCollection.GetCurrent<BuildOrchestrationData>().Select<BuildOrchestrationData, TaskOrchestrationPlanReference>((System.Func<BuildOrchestrationData, TaskOrchestrationPlanReference>) (bo => bo.Plan)));
            buildData.OrchestrationPlan = buildData.Plans.SingleOrDefault<TaskOrchestrationPlanReference>((System.Func<TaskOrchestrationPlanReference, bool>) (bo =>
            {
              int? orchestrationType = bo.OrchestrationType;
              int num = 1;
              return orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue;
            }));
          }
        }
        return buildData;
      }
    }
  }
}
