// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component39
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
  internal class Build2Component39 : Build2Component38
  {
    public override async Task<IList<BuildData>> GetAllBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component39 component = this;
      IList<BuildData> allBuildsAsync;
      using (component.TraceScope(method: nameof (GetAllBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetAllBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) queryOrder);
        Build2Component39 build2Component39_1 = component;
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
        build2Component39_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component39 build2Component39_2 = component;
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
        build2Component39_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
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
        allBuildsAsync = (IList<BuildData>) items;
      }
      return allBuildsAsync;
    }

    public override async Task<IList<BuildData>> GetBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component39 component = this;
      IList<BuildData> definitionsAsync;
      using (component.TraceScope(method: nameof (GetBuildsByDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetBuildsByDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        component.BindInt("@queryOrder", (int) queryOrder);
        Build2Component39 build2Component39_1 = component;
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
        build2Component39_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component39 build2Component39_2 = component;
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
        build2Component39_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
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
        definitionsAsync = (IList<BuildData>) items;
      }
      return definitionsAsync;
    }

    public override async Task<IList<BuildData>> GetCompletedBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component39 component = this;
      IList<BuildData> completedBuildsAsync;
      using (component.TraceScope(method: nameof (GetCompletedBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetCompletedBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) queryOrder);
        Build2Component39 build2Component39_1 = component;
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
        build2Component39_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component39 build2Component39_2 = component;
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
        build2Component39_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
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
        completedBuildsAsync = (IList<BuildData>) items;
      }
      return completedBuildsAsync;
    }

    public override async Task<IList<BuildData>> GetCompletedBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component39 component = this;
      IList<BuildData> definitionsAsync;
      using (component.TraceScope(method: nameof (GetCompletedBuildsByDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetCompletedBuildsByDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) queryOrder);
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        Build2Component39 build2Component39_1 = component;
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
        build2Component39_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component39 build2Component39_2 = component;
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
        build2Component39_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
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
        definitionsAsync = (IList<BuildData>) items;
      }
      return definitionsAsync;
    }

    public override async Task<IList<BuildData>> GetQueuedBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component39 component = this;
      IList<BuildData> queuedBuildsAsync;
      using (component.TraceScope(method: nameof (GetQueuedBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetQueuedBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) queryOrder);
        Build2Component39 build2Component39_1 = component;
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
        build2Component39_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component39 build2Component39_2 = component;
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
        build2Component39_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
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
        queuedBuildsAsync = (IList<BuildData>) items;
      }
      return queuedBuildsAsync;
    }

    public override async Task<IList<BuildData>> GetQueuedBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component39 component = this;
      IList<BuildData> definitionsAsync;
      using (component.TraceScope(method: nameof (GetQueuedBuildsByDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetQueuedBuildsByDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) queryOrder);
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        Build2Component39 build2Component39_1 = component;
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
        build2Component39_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component39 build2Component39_2 = component;
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
        build2Component39_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
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
        definitionsAsync = (IList<BuildData>) items;
      }
      return definitionsAsync;
    }

    public override async Task<IList<BuildData>> GetRunningBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component39 component = this;
      IList<BuildData> runningBuildsAsync;
      using (component.TraceScope(method: nameof (GetRunningBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRunningBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) queryOrder);
        Build2Component39 build2Component39_1 = component;
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
        build2Component39_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component39 build2Component39_2 = component;
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
        build2Component39_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
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
        runningBuildsAsync = (IList<BuildData>) items;
      }
      return runningBuildsAsync;
    }

    public override async Task<IList<BuildData>> GetRunningBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component39 component = this;
      IList<BuildData> definitionsAsync;
      using (component.TraceScope(method: nameof (GetRunningBuildsByDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRunningBuildsByDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) queryOrder);
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        Build2Component39 build2Component39_1 = component;
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
        build2Component39_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component39 build2Component39_2 = component;
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
        build2Component39_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
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
        definitionsAsync = (IList<BuildData>) items;
      }
      return definitionsAsync;
    }

    protected virtual ObjectBinder<BuildData> GetBuildBinder() => (ObjectBinder<BuildData>) new BuildDataBinder13(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual ObjectBinder<BuildTagData> GetBuildTagBinder() => (ObjectBinder<BuildTagData>) new BuildTagBinder();

    protected void BindOrchestrationData(
      Dictionary<int, BuildData> buildsDictionary,
      ObjectBinder<BuildOrchestrationData> buildOrchestrationData)
    {
      foreach (BuildOrchestrationData orchestrationData in buildOrchestrationData)
      {
        BuildData buildData;
        if (buildsDictionary.TryGetValue(orchestrationData.BuildId, out buildData))
        {
          int? orchestrationType = orchestrationData.Plan.OrchestrationType;
          int num = 1;
          if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
            buildData.OrchestrationPlan = orchestrationData.Plan;
          buildData.Plans.Add(orchestrationData.Plan);
        }
      }
    }

    protected void BindTagData(
      Dictionary<int, BuildData> buildsDictionary,
      ObjectBinder<BuildTagData> buildTagData)
    {
      foreach (BuildTagData buildTagData1 in buildTagData)
      {
        BuildData buildData;
        if (buildsDictionary.TryGetValue(buildTagData1.BuildId, out buildData))
          buildData.Tags.Add(buildTagData1.Tag);
      }
    }
  }
}
