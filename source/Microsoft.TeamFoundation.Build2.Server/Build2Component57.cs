// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component57
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
  internal class Build2Component57 : Build2Component56
  {
    public override async Task<IList<BranchesViewItem>> GetBranchesViewAsync(
      Guid projectId,
      int definitionId,
      int repositoryId,
      string defaultBranch,
      DateTime minQueueTime,
      int maxBranches,
      int buildsPerBranch)
    {
      Build2Component57 build2Component57 = this;
      IList<BranchesViewItem> branchesViewAsync;
      using (build2Component57.TraceScope(method: nameof (GetBranchesViewAsync)))
      {
        build2Component57.PrepareStoredProcedure("Build.prc_GetBranchesView");
        build2Component57.BindInt("@dataspaceId", build2Component57.GetDataspaceId(projectId));
        build2Component57.BindInt("@definitionId", definitionId);
        build2Component57.BindString("@defaultBranch", defaultBranch, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        build2Component57.BindDateTime2("@minQueueTime", minQueueTime);
        build2Component57.BindInt("@maxBranches", maxBranches);
        build2Component57.BindInt("@buildsPerBranch", buildsPerBranch);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component57.ExecuteReaderAsync(), build2Component57.ProcedureName, build2Component57.RequestContext))
        {
          resultCollection.AddBinder<BranchesViewItem>((ObjectBinder<BranchesViewItem>) new BranchesViewItemBinder());
          resultCollection.AddBinder<BranchesViewItem>((ObjectBinder<BranchesViewItem>) new BranchesViewItemBinder());
          List<BranchesViewItem> branchesViewItemList = new List<BranchesViewItem>();
          branchesViewItemList.AddRange((IEnumerable<BranchesViewItem>) resultCollection.GetCurrent<BranchesViewItem>().Items);
          branchesViewItemList.AddRange((IEnumerable<BranchesViewItem>) resultCollection.GetCurrent<BranchesViewItem>().Items);
          branchesViewAsync = (IList<BranchesViewItem>) branchesViewItemList;
        }
      }
      return branchesViewAsync;
    }

    public override async Task<IList<BuildData>> FilterBuildsAsync(
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
      Build2Component57 component = this;
      IList<BuildData> buildDataList;
      using (component.TraceScope(method: nameof (FilterBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_FilterBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindNullableInt("@definitionId", definitionId);
        component.BindRecursiveFolderPath("@folderPath", folderPath);
        component.BindUniqueInt32Table("@repositoryIds", (IEnumerable<int>) repositoryIds);
        component.BindUniqueInt32Table("@branchIds", (IEnumerable<int>) branchIds);
        component.BindStringTable("@keywordFilter", (IEnumerable<string>) keywordFilter);
        component.BindGuidTable("@requestedForFilter", (IEnumerable<Guid>) requestedForFilter);
        Build2Component57 build2Component57_1 = component;
        BuildResult? nullable1 = resultFilter;
        byte? parameterValue1 = nullable1.HasValue ? new byte?((byte) nullable1.GetValueOrDefault()) : new byte?();
        build2Component57_1.BindNullableByte("@resultFilter", parameterValue1);
        Build2Component57 build2Component57_2 = component;
        BuildStatus? nullable2 = statusFilter;
        byte? parameterValue2 = nullable2.HasValue ? new byte?((byte) nullable2.GetValueOrDefault()) : new byte?();
        build2Component57_2.BindNullableByte("@statusFilter", parameterValue2);
        component.BindStringTable("@tagFilters", (IEnumerable<string>) tagFilter);
        Build2Component57 build2Component57_3 = component;
        ref TimeFilter? local1 = ref timeFilter;
        TimeFilter valueOrDefault;
        DateTime? parameterValue3;
        if (!local1.HasValue)
        {
          parameterValue3 = new DateTime?();
        }
        else
        {
          valueOrDefault = local1.GetValueOrDefault();
          parameterValue3 = valueOrDefault.MinTime;
        }
        build2Component57_3.BindNullableUtcDateTime2("@minQueueTime", parameterValue3);
        Build2Component57 build2Component57_4 = component;
        ref TimeFilter? local2 = ref timeFilter;
        DateTime? parameterValue4;
        if (!local2.HasValue)
        {
          parameterValue4 = new DateTime?();
        }
        else
        {
          valueOrDefault = local2.GetValueOrDefault();
          parameterValue4 = valueOrDefault.MaxTime;
        }
        build2Component57_4.BindNullableUtcDateTime2("@maxQueueTime", parameterValue4);
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

    public override async Task<IList<BuildData>> FilterBuildsByBranchAsync(
      Guid projectId,
      int? definitionId,
      string folderPath,
      HashSet<int> repositoryIds,
      HashSet<int> branchIds,
      TimeFilter? timeFilter,
      int maxBuilds,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component57 component = this;
      IList<BuildData> buildDataList;
      using (component.TraceScope(method: nameof (FilterBuildsByBranchAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_FilterBuildsByBranch");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindNullableInt("@definitionId", definitionId);
        component.BindRecursiveFolderPath("@folderPath", folderPath);
        component.BindUniqueInt32Table("@repositoryIds", (IEnumerable<int>) repositoryIds);
        component.BindUniqueInt32Table("@branchIds", (IEnumerable<int>) branchIds);
        Build2Component57 build2Component57_1 = component;
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
        build2Component57_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component57 build2Component57_2 = component;
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
        build2Component57_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindInt("@maxBuilds", maxBuilds);
        component.BindUniqueInt32Table("@excludedDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
          resultCollection.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          ObjectBinder<BuildOrchestrationData> current = resultCollection.GetCurrent<BuildOrchestrationData>();
          component.BindOrchestrationData(dictionary, current);
          buildDataList = (IList<BuildData>) items;
        }
      }
      return buildDataList;
    }
  }
}
