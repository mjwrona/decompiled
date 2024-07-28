// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildDefinitionService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildDefinitionService))]
  public interface IBuildDefinitionService : IVssFrameworkService
  {
    BuildDefinition AddDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      DefinitionUpdateOptions options = null);

    void DisableScheduledBuilds(IVssRequestContext requestContext, Guid projectId);

    void EnableScheduledBuilds(IVssRequestContext requestContext, Guid projectId);

    IEnumerable<BuildDefinitionBranch> GetBuildableDefinitionBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int maxConcurrentBuildsPerBranch);

    Task<BuildDefinition> GetDefinitionAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int? definitionVersion = null,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false);

    IEnumerable<BuildDefinitionSummary> GetDefinitionsSummary(
      IVssRequestContext requestContext,
      Guid projectId,
      string name = "*",
      DefinitionTriggerType triggers = DefinitionTriggerType.All,
      string repositoryType = null,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      int count = 10000,
      DateTime? minLastModifiedTime = null,
      DateTime? maxLastModifiedTime = null,
      string lastDefinitionName = null,
      DateTime? minMetricsTime = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      DefinitionQueryOptions options = DefinitionQueryOptions.All,
      IEnumerable<string> tagFilters = null,
      bool includeLatestBuilds = false,
      Guid? taskIdFilter = null,
      int? processType = null,
      bool includeDrafts = false);

    IEnumerable<BuildDefinition> GetDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      string name = "*",
      DefinitionTriggerType triggers = DefinitionTriggerType.All,
      string repositoryType = null,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      int count = 10000,
      DateTime? minLastModifiedTime = null,
      DateTime? maxLastModifiedTime = null,
      string lastDefinitionName = null,
      DateTime? minMetricsTime = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      DefinitionQueryOptions options = DefinitionQueryOptions.All,
      IEnumerable<string> tagFilters = null,
      bool includeLatestBuilds = false,
      Guid? taskIdFilter = null,
      int? processType = null);

    IEnumerable<BuildDefinition> GetAllDefinitionsForPath(
      IVssRequestContext requestContext,
      Guid projectId,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      int count = 10000,
      string path = null,
      bool includeLatestBuilds = false);

    List<BuildDefinition> GetDefinitionsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> definitionIds,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false,
      bool includeDrafts = false,
      ExcludePopulatingDefinitionResources excludePopulatingResources = null);

    Task<List<BuildDefinition>> GetDefinitionsByIdsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> definitionIds,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false,
      bool includeDrafts = false);

    IEnumerable<BuildDefinition> GetDefinitionsForRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repositoryId,
      string name = "*",
      DefinitionTriggerType triggers = DefinitionTriggerType.All,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      int count = 10000,
      DateTime? minLastModifiedTime = null,
      DateTime? maxLastModifiedTime = null,
      string lastDefinitionName = null,
      DateTime? minMetricsTime = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      DefinitionQueryOptions options = DefinitionQueryOptions.All,
      IEnumerable<string> tagFilters = null,
      bool includeLatestBuilds = false,
      Guid? taskIdFilter = null,
      int? processType = null);

    IEnumerable<BuildDefinition> GetYamlDefinitionsForRepository(
      IVssRequestContext requestContext,
      List<Guid> projectIds,
      string repositoryId,
      string repositoryType,
      int maxDefinitions = 10000);

    IEnumerable<BuildDefinition> GetDefinitionsWithTriggers(
      IVssRequestContext requestContext,
      List<Guid> projectIds,
      string repositoryType,
      string repositoryId,
      DefinitionTriggerType triggerFilter,
      int count = 10000);

    IEnumerable<BuildDefinition> GetCIDefinitions(
      IVssRequestContext requestContext,
      List<Guid> projectIds,
      string repositoryType,
      string repositoryId,
      int count = 10000);

    IEnumerable<BuildDefinition> GetDefinitionHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId);

    void DestroyDefinition(IVssRequestContext requestContext, Guid projectId, int definitionId);

    BuildDefinition UpdateDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      bool authorizeNewResources = false,
      DefinitionUpdateOptions options = null);

    IEnumerable<BuildDefinitionBranch> UpdateDefinitionBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildDefinition definition,
      IEnumerable<BuildDefinitionBranch> branches,
      int maxConcurrentBuildsPerBranch,
      bool ignoreSourceIdCheck);

    void DeleteDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> definitionIds,
      Guid? authorId = null);

    BuildDefinition RestoreDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      Guid? authorId = null,
      string comment = null);

    BuildDefinition GetDeletedDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId);

    Task<IEnumerable<BuildDefinition>> GetDeletedDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      int count = 10000);

    IEnumerable<string> AddTags(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IEnumerable<string> tags);

    IEnumerable<string> DeleteTags(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IEnumerable<string> tags);

    IEnumerable<BuildMetric> GetMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      DateTime? minMetricsTime);

    IEnumerable<BuildMetric> UpdateMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      IEnumerable<BuildMetric> metrics);

    void ReadSecretVariables(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IDictionary<string, string> targetSecretVariables,
      IDictionary<string, string> targetRepositorySecrets,
      bool omitVariableGroups = false);

    BuildRepository GetExpandedRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IDictionary<string, string> variables = null);

    PropertiesCollection UpdateProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      PropertiesCollection properties);

    PipelineBuildResult BuildPipeline(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      PipelineResources authorizedResources = null,
      bool authorizeNewResources = false,
      BuildOptions options = null);

    List<BuildDefinition> GetRecentlyBuiltDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      int top,
      bool includeQueuedBuilds = false);

    List<BuildDefinition> GetMyRecentlyBuiltDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      DateTime minFinishTime,
      int top,
      IEnumerable<int> excludeDefinitionIds);

    Task<IList<RepositoryBranchReferences>> GetBranchesByName(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount,
      string nameSubstring,
      int? definitionId,
      HashSet<int> excludedBranchIds);

    Task<IList<RepositoryBranchReferences>> GetRecentlyBuiltRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      int? definitionId,
      int topRepositories,
      int topBranches,
      HashSet<int> excludedRepositoryIds,
      HashSet<int> excludedDefinitionIds);

    Task<IList<RepositoryBranchReferences>> GetRecentlyBuiltBranchesForRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxBranches,
      IEnumerable<string> repositoryIdentifiers,
      HashSet<int> excludedRepositoryIds);

    Task<IList<Guid>> GetRecentlyBuiltRequestedForIdentities(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount,
      HashSet<Guid> excludedIds,
      HashSet<int> excludedDefinitionIds);

    BuildDefinition GetRenamedDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      string name,
      string path);

    Task<IList<BuildSchedule>> GetSchedulesByDefinitionId(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId);

    Task<IList<RepositoryBranchReferences>> GetBranchesById(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> branchIds);

    IList<Guid> GetAllServiceConnectionsForRepoAndProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string repoIdentifier,
      string repoType,
      int triggerType);
  }
}
