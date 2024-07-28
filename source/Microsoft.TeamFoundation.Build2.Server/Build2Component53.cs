// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component53
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
  internal class Build2Component53 : Build2Component52
  {
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
      Build2Component53 component = this;
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
        Build2Component53 build2Component53_1 = component;
        BuildResult? nullable1 = resultFilter;
        byte? parameterValue1 = nullable1.HasValue ? new byte?((byte) nullable1.GetValueOrDefault()) : new byte?();
        build2Component53_1.BindNullableByte("@resultFilter", parameterValue1);
        Build2Component53 build2Component53_2 = component;
        BuildStatus? nullable2 = statusFilter;
        byte? parameterValue2 = nullable2.HasValue ? new byte?((byte) nullable2.GetValueOrDefault()) : new byte?();
        build2Component53_2.BindNullableByte("@statusFilter", parameterValue2);
        component.BindStringTable("@tagFilters", (IEnumerable<string>) tagFilter);
        Build2Component53 build2Component53_3 = component;
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
        build2Component53_3.BindNullableUtcDateTime2("@minQueueTime", parameterValue3);
        Build2Component53 build2Component53_4 = component;
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
        build2Component53_4.BindNullableUtcDateTime2("@maxQueueTime", parameterValue4);
        component.BindInt("@maxBuilds", maxBuilds);
        component.BindUniqueInt32Table("@excludedDefinitionIds", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
          resultCollection.AddBinder<BuildTagData>(component.GetBuildTagDataBinder());
          List<BuildData> items = resultCollection.GetCurrent<BuildData>().Items;
          Dictionary<int, BuildData> dictionary = items.ToDictionary<BuildData, int>((System.Func<BuildData, int>) (x => x.Id));
          resultCollection.NextResult();
          foreach (BuildTagData buildTagData in resultCollection.GetCurrent<BuildTagData>())
          {
            BuildData buildData;
            if (dictionary.TryGetValue(buildTagData.BuildId, out buildData))
              buildData.Tags.Add(buildTagData.Tag);
          }
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
      Build2Component53 component = this;
      IList<BuildData> items;
      using (component.TraceScope(method: nameof (FilterBuildsByBranchAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_FilterBuildsByBranch");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindNullableInt("@definitionId", definitionId);
        component.BindRecursiveFolderPath("@folderPath", folderPath);
        component.BindUniqueInt32Table("@repositoryIds", (IEnumerable<int>) repositoryIds);
        component.BindUniqueInt32Table("@branchIds", (IEnumerable<int>) branchIds);
        Build2Component53 build2Component53_1 = component;
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
        build2Component53_1.BindNullableUtcDateTime2("@minTime", parameterValue1);
        Build2Component53 build2Component53_2 = component;
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
        build2Component53_2.BindNullableUtcDateTime2("@maxTime", parameterValue2);
        component.BindInt("@maxBuilds", maxBuilds);
        component.BindUniqueInt32Table("@excludedDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
          items = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
        }
      }
      return items;
    }

    public override async Task<IList<RepositoryBranchReferences>> GetBranchesByName(
      Guid projectId,
      int maxCount,
      string nameLike,
      HashSet<int> excludedBranchIds)
    {
      Build2Component53 component = this;
      IList<RepositoryBranchReferences> branchReferences;
      using (component.TraceScope(method: nameof (GetBranchesByName)))
      {
        component.PrepareStoredProcedure("Build.prc_GetBranchesByName");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@maxCount", maxCount);
        component.BindString("@nameLike", nameLike, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindUniqueInt32Table("@excludedBranchIds", (IEnumerable<int>) excludedBranchIds);
        using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          rc.AddBinder<RepositoryReference>(component.GetRepositoryReferenceBinder());
          rc.AddBinder<BranchReference>(component.GetBranchReferenceBinder());
          branchReferences = (IList<RepositoryBranchReferences>) component.GetRepositoryBranchReferences(rc);
        }
      }
      return branchReferences;
    }

    public override async Task<IList<RepositoryBranchReferences>> GetRecentlyBuiltRepositories(
      Guid projectId,
      int topRepositories,
      int topBranches,
      HashSet<int> excludedRepositoryIds,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component53 component = this;
      IList<RepositoryBranchReferences> branchReferences;
      using (component.TraceScope(method: nameof (GetRecentlyBuiltRepositories)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRecentlyBuiltRepositories");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@topRepositories", topRepositories);
        component.BindInt("@topBranches", topBranches);
        component.BindUniqueInt32Table("@excludedRepositoryIds", (IEnumerable<int>) excludedRepositoryIds);
        component.BindUniqueInt32Table("@excludedDefinitionIds", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          rc.AddBinder<RepositoryReference>(component.GetRepositoryReferenceBinder());
          rc.AddBinder<BranchReference>(component.GetBranchReferenceBinder());
          branchReferences = (IList<RepositoryBranchReferences>) component.GetRepositoryBranchReferences(rc);
        }
      }
      return branchReferences;
    }

    public override async Task<IList<Guid>> GetRecentlyBuiltRequestedForIdentities(
      Guid projectId,
      int maxCount,
      HashSet<Guid> excludedIds,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component53 component = this;
      IList<Guid> items;
      using (component.TraceScope(method: nameof (GetRecentlyBuiltRequestedForIdentities)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRecentlyBuiltRequestedForIdentities");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@maxCount", maxCount);
        component.BindGuidTable("@excludedIds", (IEnumerable<Guid>) excludedIds);
        component.BindUniqueInt32Table("@excludedDefinitionIds", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new GuidBinder());
          items = (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
        }
      }
      return items;
    }

    protected virtual RepositoryBranchReferences[] GetRepositoryBranchReferences(ResultCollection rc)
    {
      List<RepositoryReference> items = rc.GetCurrent<RepositoryReference>().Items;
      rc.NextResult();
      ILookup<int, BranchReference> branches = rc.GetCurrent<BranchReference>().Items.ToLookup<BranchReference, int>((System.Func<BranchReference, int>) (br => br.RepositoryId));
      System.Func<RepositoryReference, RepositoryBranchReferences> selector = (System.Func<RepositoryReference, RepositoryBranchReferences>) (repo => new RepositoryBranchReferences()
      {
        Id = repo.Id,
        Identifier = repo.Identifier,
        RepositoryString = repo.RepositoryString,
        Type = repo.Type,
        Branches = branches[repo.Id].ToArray<BranchReference>()
      });
      return items.Select<RepositoryReference, RepositoryBranchReferences>(selector).ToArray<RepositoryBranchReferences>();
    }

    protected override ObjectBinder<BuildDefinitionForRetention> GetBuildDefinitionForRetentionBinder() => (ObjectBinder<BuildDefinitionForRetention>) new BuildDefinitionForRetentionBinder4(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual ObjectBinder<BranchReference> GetBranchReferenceBinder() => (ObjectBinder<BranchReference>) new BranchReferenceBinder(this.RequestContext, (BuildSqlComponentBase) this);

    protected virtual ObjectBinder<RepositoryReference> GetRepositoryReferenceBinder() => (ObjectBinder<RepositoryReference>) new RepositoryReferenceBinder(this.RequestContext, (BuildSqlComponentBase) this);
  }
}
