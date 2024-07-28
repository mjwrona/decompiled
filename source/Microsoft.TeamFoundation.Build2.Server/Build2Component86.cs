// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component86
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component86 : Build2Component85
  {
    public override async Task<IList<BuildData>> GetAllBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component86 component = this;
      IList<BuildData> allBuildsAsync;
      using (component.TraceScope(method: nameof (GetAllBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetAllBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) component.CollapseToOldQueryOrderValues(queryOrder));
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          allBuildsAsync = (IList<BuildData>) items;
        }
      }
      return allBuildsAsync;
    }

    public override async Task<IList<BuildData>> GetAllBuildsUnderFolderAsync(
      Guid projectId,
      string folderPath,
      TimeFilter? timeFilter,
      int count,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component86 component = this;
      IList<BuildData> underFolderAsync;
      using (component.TraceScope(method: nameof (GetAllBuildsUnderFolderAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetAllBuildsUnderFolder");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindRecursiveFolderPath("@folderPath", folderPath);
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindInt("@count", count);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          underFolderAsync = (IList<BuildData>) items;
        }
      }
      return underFolderAsync;
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
      using (this.TraceScope(method: nameof (GetBuilds)))
      {
        this.PrepareStoredProcedure("Build.prc_GetBuilds2");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindUniqueInt32Table("@definitionIdTable", definitionIds);
        this.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        this.BindUniqueInt32Table("@queueIdTable", queueIds);
        this.BindString("@buildNumber", DBHelper.ServerPathToDBPath(buildNumber), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUtcDateTime2("@minTime", minFinishTime);
        this.BindNullableUtcDateTime2("@maxTime", maxFinishTime);
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
          resultCollection.AddBinder<RetentionLease>(this.GetRetentionLeaseBinder());
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
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          return (IEnumerable<BuildData>) items;
        }
      }
    }

    public override async Task<IList<BuildData>> GetBuildsByDefinitionsAsync(
      Guid projectId,
      int count,
      HashSet<int> definitionIds,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component86 component = this;
      IList<BuildData> definitionsAsync;
      using (component.TraceScope(method: nameof (GetBuildsByDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetBuildsByDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        component.BindInt("@queryOrder", (int) component.CollapseToOldQueryOrderValues(queryOrder));
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          definitionsAsync = (IList<BuildData>) items;
        }
      }
      return definitionsAsync;
    }

    public override IEnumerable<BuildData> GetBuildsByIds(
      IEnumerable<int> buildIds,
      bool includeDeleted)
    {
      using (this.TraceScope(method: nameof (GetBuildsByIds)))
      {
        this.PrepareStoredProcedure("Build.prc_GetBuildsByIds");
        this.BindInt32Table("@buildIdTable", buildIds.Distinct<int>());
        this.BindBoolean("@includeDeleted", includeDeleted);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(this.GetRetentionLeaseBinder());
          Dictionary<int, BuildData> dictionary = resultCollection.GetCurrent<BuildData>().Items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
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
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          return (IEnumerable<BuildData>) dictionary.Values;
        }
      }
    }

    public override async Task<IEnumerable<BuildData>> GetBuildsByIdsAsync(
      IEnumerable<int> buildIds,
      bool includeDeleted)
    {
      Build2Component86 component = this;
      IEnumerable<BuildData> values;
      using (component.TraceScope(method: nameof (GetBuildsByIdsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetBuildsByIds");
        component.BindInt32Table("@buildIdTable", buildIds.Distinct<int>());
        component.BindBoolean("@includeDeleted", includeDeleted);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          Dictionary<int, BuildData> dictionary = resultCollection.GetCurrent<BuildData>().Items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
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
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          values = (IEnumerable<BuildData>) dictionary.Values;
        }
      }
      return values;
    }

    public override async Task<IList<BuildData>> GetCompletedBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component86 component = this;
      IList<BuildData> completedBuildsAsync;
      using (component.TraceScope(method: nameof (GetCompletedBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetCompletedBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) component.CollapseToOldQueryOrderValues(queryOrder));
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          completedBuildsAsync = (IList<BuildData>) items;
        }
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
      Build2Component86 component = this;
      IList<BuildData> definitionsAsync;
      using (component.TraceScope(method: nameof (GetCompletedBuildsByDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetCompletedBuildsByDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) component.CollapseToOldQueryOrderValues(queryOrder));
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          definitionsAsync = (IList<BuildData>) items;
        }
      }
      return definitionsAsync;
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
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(this.GetRetentionLeaseBinder());
          BuildData buildData = resultCollection.GetCurrent<BuildData>().SingleOrDefault<BuildData>();
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
            resultCollection.NextResult();
            buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) resultCollection.GetCurrent<RetentionLease>());
          }
          return buildData;
        }
      }
    }

    public override BuildData GetLatestSuccessfulBuildForBranch(
      Guid projectId,
      int definitionId,
      string repositoryId,
      string repositoryType,
      string branchName,
      DateTime? maxFinishTime)
    {
      using (this.TraceScope(method: nameof (GetLatestSuccessfulBuildForBranch)))
      {
        this.PrepareStoredProcedure("Build.prc_GetLatestSuccessfulBuildForBranch");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@definitionId", definitionId);
        this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@branchName", branchName, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindUtcDateTime2("@maxFinishTime", maxFinishTime ?? DateTime.UtcNow);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(this.GetRetentionLeaseBinder());
          BuildData successfulBuildForBranch = resultCollection.GetCurrent<BuildData>().FirstOrDefault<BuildData>();
          resultCollection.NextResult();
          foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
          {
            if (successfulBuildForBranch.Id == buildTagData.BuildId)
              successfulBuildForBranch.Tags.Add(buildTagData.Tag);
          }
          resultCollection.NextResult();
          foreach (BuildOrchestrationData orchestrationData in resultCollection.GetCurrent<BuildOrchestrationData>())
          {
            int? orchestrationType = orchestrationData.Plan.OrchestrationType;
            int num = 1;
            if (orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue)
              successfulBuildForBranch.OrchestrationPlan = orchestrationData.Plan;
            successfulBuildForBranch.Plans.Add(orchestrationData.Plan);
          }
          resultCollection.NextResult();
          if (resultCollection.GetCurrent<RetentionLease>().Any<RetentionLease>())
            successfulBuildForBranch.RetentionLeases.AddRange((IEnumerable<RetentionLease>) resultCollection.GetCurrent<RetentionLease>());
          return successfulBuildForBranch;
        }
      }
    }

    public override IList<BuildData> GetRunnableGatedBuilds()
    {
      using (this.TraceScope(method: nameof (GetRunnableGatedBuilds)))
      {
        this.PrepareStoredProcedure("Build.prc_GetRunnableGatedBuilds");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(this.GetRetentionLeaseBinder());
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
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          return (IList<BuildData>) items;
        }
      }
    }

    public override IList<BuildData> GetRunnablePausedBuilds(int maxSelected)
    {
      using (this.TraceScope(method: nameof (GetRunnablePausedBuilds)))
      {
        this.PrepareStoredProcedure("Build.prc_GetRunnablePausedBuilds");
        this.BindInt("@maxSelected", maxSelected);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(this.GetRetentionLeaseBinder());
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
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          return (IList<BuildData>) items;
        }
      }
    }

    public override async Task<IList<BuildData>> GetRunningBuildsAsync(
      Guid projectId,
      int count,
      BuildQueryOrder queryOrder,
      TimeFilter? timeFilter,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component86 component = this;
      IList<BuildData> runningBuildsAsync;
      using (component.TraceScope(method: nameof (GetRunningBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRunningBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) component.CollapseToOldQueryOrderValues(queryOrder));
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          runningBuildsAsync = (IList<BuildData>) items;
        }
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
      Build2Component86 component = this;
      IList<BuildData> definitionsAsync;
      using (component.TraceScope(method: nameof (GetRunningBuildsByDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRunningBuildsByDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) component.CollapseToOldQueryOrderValues(queryOrder));
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          definitionsAsync = (IList<BuildData>) items;
        }
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
      Build2Component86 component = this;
      IList<BuildData> queuedBuildsAsync;
      using (component.TraceScope(method: nameof (GetQueuedBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetQueuedBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) component.CollapseToOldQueryOrderValues(queryOrder));
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          queuedBuildsAsync = (IList<BuildData>) items;
        }
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
      Build2Component86 component = this;
      IList<BuildData> definitionsAsync;
      using (component.TraceScope(method: nameof (GetQueuedBuildsByDefinitionsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetQueuedBuildsByDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@count", count);
        component.BindInt("@queryOrder", (int) component.CollapseToOldQueryOrderValues(queryOrder));
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        Build2Component86 build2Component86_1 = component;
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
        build2Component86_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component86 build2Component86_2 = component;
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
        build2Component86_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildTagData> current1 = resultCollection.GetCurrent<BuildTagData>();
          component.BindTagData(dictionary, current1);
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current2 = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current2);
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          definitionsAsync = (IList<BuildData>) items;
        }
      }
      return definitionsAsync;
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
        this.BindAppendCommitMessageToRunName(build.AppendCommitMessageToRunName);
        this.BindString("@validationIssues", JsonUtility.ToString<BuildRequestValidationResult>((IList<BuildRequestValidationResult>) build.ValidationResults.Where<BuildRequestValidationResult>((System.Func<BuildRequestValidationResult, bool>) (vr => vr.Result != 0)).ToList<BuildRequestValidationResult>()), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        if (build.OrchestrationPlan == null || build.OrchestrationPlan.PlanId == Guid.Empty)
          this.BindNullableGuid("@orchestrationId", new Guid?());
        else
          this.BindNullableGuid("@orchestrationId", build.OrchestrationPlan.PlanId);
        queue = (AgentPoolQueue) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(this.GetRetentionLeaseBinder());
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
          resultCollection.NextResult();
          if (resultCollection.GetCurrent<RetentionLease>().Any<RetentionLease>())
            buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) resultCollection.GetCurrent<RetentionLease>());
          return buildData;
        }
      }
    }

    public override List<BuildData> UpdateBuilds(
      Guid projectId,
      IEnumerable<BuildData> builds,
      Guid changedBy,
      out IList<BuildData> oldBuilds,
      out IDictionary<int, BuildDefinition> definitionsById)
    {
      using (this.TraceScope(method: nameof (UpdateBuilds)))
      {
        this.PrepareStoredProcedure("Build.prc_UpdateBuilds");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindBuildUpdateTable("@buildUpdateTable", builds);
        this.BindGuid("@requestedBy", changedBy);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildData>(this.GetBuildDataBinder());
          resultCollection.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
          resultCollection.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(this.GetRetentionLeaseBinder());
          oldBuilds = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
          resultCollection.NextResult();
          List<BuildData> items1 = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items1.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
          resultCollection.NextResult();
          List<BuildDefinition> items2 = resultCollection.GetCurrent<BuildDefinition>().Items;
          definitionsById = (IDictionary<int, BuildDefinition>) items2.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (d => d.Id));
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
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          return items1;
        }
      }
    }

    public override async Task<UpdateBuildsResult> UpdateBuildsAsync(
      Guid projectId,
      IEnumerable<BuildData> builds,
      Guid changedBy)
    {
      Build2Component86 build2Component86 = this;
      UpdateBuildsResult updateBuildsResult;
      using (build2Component86.TraceScope(method: nameof (UpdateBuildsAsync)))
      {
        build2Component86.PrepareStoredProcedure("Build.prc_UpdateBuilds");
        build2Component86.BindInt("@dataspaceId", build2Component86.GetDataspaceId(projectId));
        build2Component86.BindBuildUpdateTable("@buildUpdateTable", builds);
        build2Component86.BindGuid("@requestedBy", changedBy);
        UpdateBuildsResult result = new UpdateBuildsResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component86.ExecuteReaderAsync(), build2Component86.ProcedureName, build2Component86.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(build2Component86.GetBuildDataBinder());
          resultCollection.AddBinder<BuildData>(build2Component86.GetBuildDataBinder());
          resultCollection.AddBinder<BuildDefinition>(build2Component86.GetBuildDefinitionBinder());
          resultCollection.AddBinder<BuildTagData>(build2Component86.GetBuildTagDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(build2Component86.GetBuildOrchestrationDataBinder());
          resultCollection.AddBinder<RetentionLease>(build2Component86.GetRetentionLeaseBinder());
          result.OldBuilds = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
          resultCollection.NextResult();
          result.NewBuilds = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = result.NewBuilds.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (b => b.Id));
          resultCollection.NextResult();
          result.DefinitionsById = (IDictionary<int, BuildDefinition>) resultCollection.GetCurrent<BuildDefinition>().Items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (d => d.Id));
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
          resultCollection.NextResult();
          foreach (IGrouping<int, RetentionLease> collection in resultCollection.GetCurrent<RetentionLease>().GroupBy<RetentionLease, int>((System.Func<RetentionLease, int>) (l => l.RunId)))
          {
            BuildData buildData;
            if (dictionary.TryGetValue(collection.Key, out buildData))
              buildData.RetentionLeases.AddRange((IEnumerable<RetentionLease>) collection);
          }
          updateBuildsResult = result;
        }
      }
      return updateBuildsResult;
    }

    protected virtual BuildQueryOrder CollapseToOldQueryOrderValues(BuildQueryOrder queryOrder)
    {
      switch (queryOrder)
      {
        case BuildQueryOrder.QueueTimeDescending:
        case BuildQueryOrder.StartTimeDescending:
        case BuildQueryOrder.FinishTimeDescending:
          return BuildQueryOrder.Descending;
        case BuildQueryOrder.QueueTimeAscending:
        case BuildQueryOrder.StartTimeAscending:
        case BuildQueryOrder.FinishTimeAscending:
          return BuildQueryOrder.Ascending;
        default:
          return queryOrder;
      }
    }

    protected override BuildObjectBinder<ArtifactCleanupRecord> GetArtifactCleanupRecordBinder() => (BuildObjectBinder<ArtifactCleanupRecord>) new ArtifactCleanupRecordBinder2(this.RequestContext, (BuildSqlComponentBase) this);

    protected override ObjectBinder<BuildData> GetBuildDataBinder() => (ObjectBinder<BuildData>) new BuildDataBinder17(this.RequestContext, (BuildSqlComponentBase) this);
  }
}
