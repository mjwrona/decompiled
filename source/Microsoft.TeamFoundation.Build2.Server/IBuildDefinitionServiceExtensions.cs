// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildDefinitionServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IBuildDefinitionServiceExtensions
  {
    private const string TraceLayer = "IBuildDefinitionServiceExtensions";
    private static readonly char[] c_PathSeparators = new char[2]
    {
      '\\',
      '/'
    };

    public static BuildDefinition AddDefinition(
      this IBuildDefinitionService definitionService,
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildDefinition secretSource = null,
      Guid? authorId = null)
    {
      DefinitionUpdateOptions options = (DefinitionUpdateOptions) null;
      if (authorId.HasValue || secretSource != null)
        options = new DefinitionUpdateOptions()
        {
          AuthorId = authorId ?? Guid.Empty,
          SecretSource = secretSource
        };
      return definitionService.AddDefinition(requestContext, definition, options);
    }

    public static IEnumerable<BuildDefinition> GetDefinitions(
      this IBuildDefinitionService service,
      IVssRequestContext requestContext,
      string project,
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
      Guid? taskIdFilter = null)
    {
      Guid projectId;
      if (!Guid.TryParse(project, out projectId))
        requestContext.GetService<IProjectService>().TryGetProjectId(requestContext, project, out projectId);
      return service.GetDefinitions(requestContext, projectId, name, triggers, repositoryType, queryOrder, count, minLastModifiedTime, maxLastModifiedTime, lastDefinitionName, minMetricsTime, path, builtAfter, notBuiltAfter, options, tagFilters, includeLatestBuilds, taskIdFilter);
    }

    public static BuildDefinition GetDefinition(
      this IBuildDefinitionService definitionService,
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int? definitionVersion = null,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false)
    {
      return requestContext.RunSynchronously<BuildDefinition>((Func<Task<BuildDefinition>>) (() => definitionService.GetDefinitionAsync(requestContext, projectId, definitionId, definitionVersion, propertyFilters, includeDeleted, minMetricsTime, includeLatestBuilds)));
    }

    public static BuildDefinition GetDefinition(
      this IBuildDefinitionService service,
      IVssRequestContext requestContext,
      Guid projectId,
      string pathOrId)
    {
      BuildDefinition definition = (BuildDefinition) null;
      int result;
      if (int.TryParse(pathOrId, out result))
        definition = service.GetDefinition(requestContext, projectId, result);
      if (definition == null)
      {
        pathOrId = pathOrId.Replace('/', '\\');
        int num = pathOrId.LastIndexOfAny(IBuildDefinitionServiceExtensions.c_PathSeparators);
        string name1 = num < 0 ? pathOrId : pathOrId.Substring(num + 1);
        string path1 = num > 0 ? pathOrId.Substring(0, num + 1) : (string) null;
        if (!string.IsNullOrWhiteSpace(path1) && !path1.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
          path1 = "\\" + path1;
        IBuildDefinitionService definitionService = service;
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        string name2 = name1;
        string str = path1;
        DateTime? minLastModifiedTime = new DateTime?();
        DateTime? maxLastModifiedTime = new DateTime?();
        DateTime? minMetricsTime = new DateTime?();
        string path2 = str;
        DateTime? builtAfter = new DateTime?();
        DateTime? notBuiltAfter = new DateTime?();
        Guid? taskIdFilter = new Guid?();
        int? processType = new int?();
        IEnumerable<BuildDefinition> definitions = definitionService.GetDefinitions(requestContext1, projectId1, name2, minLastModifiedTime: minLastModifiedTime, maxLastModifiedTime: maxLastModifiedTime, minMetricsTime: minMetricsTime, path: path2, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, taskIdFilter: taskIdFilter, processType: processType);
        if (definitions.Count<BuildDefinition>() > 1)
        {
          if (string.IsNullOrWhiteSpace(path1))
            definition = definitions.FirstOrDefault<BuildDefinition>((Func<BuildDefinition, bool>) (d => d.Path == "\\"));
          if (definition == null)
            throw new AmbiguousDefinitionNameException(BuildServerResources.AmbiguousDefinitionName((object) name1));
        }
        else
          definition = definitions.SingleOrDefault<BuildDefinition>() ?? service.GetRenamedDefinition(requestContext, projectId, name1, path1);
      }
      return definition;
    }

    public static IEnumerable<BuildDefinition> GetDeletedDefinitions(
      this IBuildDefinitionService service,
      IVssRequestContext requestContext,
      Guid projectId,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      int count = 10000)
    {
      return requestContext.RunSynchronously<IEnumerable<BuildDefinition>>((Func<Task<IEnumerable<BuildDefinition>>>) (() => service.GetDeletedDefinitionsAsync(requestContext, projectId, queryOrder, count)));
    }

    public static BuildDefinition UpdateDefinition(
      this IBuildDefinitionService definitionService,
      IVssRequestContext requestContext,
      BuildDefinition definition,
      bool authorizeNewResources = false,
      BuildDefinition secretSource = null,
      Guid? authorId = null)
    {
      DefinitionUpdateOptions options = (DefinitionUpdateOptions) null;
      if (authorId.HasValue || secretSource != null)
        options = new DefinitionUpdateOptions()
        {
          AuthorId = authorId ?? Guid.Empty,
          SecretSource = secretSource
        };
      return definitionService.UpdateDefinition(requestContext, definition, authorizeNewResources, options);
    }

    public static void DeleteDefinition(
      this IBuildDefinitionService definitionService,
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      Guid? authorId = null)
    {
      IBuildDefinitionService definitionService1 = definitionService;
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      List<int> definitionIds = new List<int>();
      definitionIds.Add(definitionId);
      Guid? authorId1 = authorId;
      definitionService1.DeleteDefinitions(requestContext1, projectId1, definitionIds, authorId1);
    }

    public static void ValidateDefinition(
      this IBuildDefinitionService definitionService,
      IVssRequestContext requestContext,
      BuildDefinition definition,
      bool update = false)
    {
      new BuildDefinitionValidator().Validate(requestContext, definition, update);
    }

    public static IEnumerable<string> AddTags(
      this IBuildDefinitionService definitionService,
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckForNonPositiveInt(definitionId, nameof (definitionId), "Build2");
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tags, nameof (tags), "Build2");
      using (requestContext.TraceScope(nameof (IBuildDefinitionServiceExtensions), nameof (AddTags)))
        return definitionService.AddTags(requestContext, definitionService.GetDefinition(requestContext, projectId, definitionId) ?? throw new DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definitionId)), tags);
    }

    public static IEnumerable<string> DeleteTags(
      this IBuildDefinitionService definitionService,
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      ArgumentUtility.CheckForNonPositiveInt(definitionId, nameof (definitionId), "Build2");
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tags, nameof (tags), "Build2");
      using (requestContext.TraceScope(nameof (IBuildDefinitionServiceExtensions), nameof (DeleteTags)))
        return definitionService.DeleteTags(requestContext, definitionService.GetDefinition(requestContext, projectId, definitionId) ?? throw new DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definitionId)), tags);
    }

    public static void QueryResources(
      this IBuildDefinitionService definitionService,
      IVssRequestContext requestContext,
      Guid projectId,
      List<Guid> endpointIds,
      List<int> queueIds,
      List<int> variableGroupIds,
      ExcludePopulatingDefinitionResources excludePopulatingResources,
      out Dictionary<Guid, string> endpointsLookup,
      out Dictionary<int, string> queuesLookup,
      out Dictionary<int, string> variableGroupsLookup)
    {
      endpointsLookup = new Dictionary<Guid, string>();
      queuesLookup = new Dictionary<int, string>();
      variableGroupsLookup = new Dictionary<int, string>();
      using (requestContext.TraceScope(nameof (IBuildDefinitionServiceExtensions), nameof (QueryResources)))
      {
        if (endpointIds.Count > 0 && (excludePopulatingResources == null || !excludePopulatingResources.Endpoints))
        {
          try
          {
            IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> source = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) endpointIds, false, false, ServiceEndpointActionFilter.None);
            endpointsLookup = source.Distinct<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>().ToDictionary<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Guid, string>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Guid>) (x => x.Id), (Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, string>) (x => x.Name));
          }
          catch (Exception ex)
          {
            requestContext.TraceError("BuildDefinitionExtensions", "Failed to populate resources {0}", (object) ex);
          }
        }
        if (queueIds.Count > 0 && (excludePopulatingResources == null || !excludePopulatingResources.Queues))
        {
          try
          {
            IList<TaskAgentQueue> agentQueues = requestContext.GetService<IDistributedTaskPoolService>().GetAgentQueues(requestContext, projectId, (IEnumerable<int>) queueIds);
            queuesLookup = agentQueues.Distinct<TaskAgentQueue>().ToDictionary<TaskAgentQueue, int, string>((Func<TaskAgentQueue, int>) (x => x.Id), (Func<TaskAgentQueue, string>) (x => x.Name));
          }
          catch (Exception ex)
          {
            requestContext.TraceError("BuildDefinitionExtensions", "Failed to populate resources {0}", (object) ex);
          }
        }
        if (variableGroupIds.Count <= 0 || excludePopulatingResources != null && excludePopulatingResources.VariableGroups)
          return;
        try
        {
          IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = requestContext.GetService<IVariableGroupService>().GetVariableGroups(requestContext, projectId, (IList<int>) variableGroupIds.ToList<int>());
          variableGroupsLookup = variableGroups.Distinct<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>().ToDictionary<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, int, string>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, int>) (x => x.Id), (Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, string>) (x => x.Name));
        }
        catch (Exception ex)
        {
          requestContext.TraceError("BuildDefinitionExtensions", "Failed to populate resources {0}", (object) ex);
        }
      }
    }
  }
}
