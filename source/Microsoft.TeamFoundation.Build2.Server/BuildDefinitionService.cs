// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Authorization.Server;
using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build2.Server.Helpers;
using Microsoft.TeamFoundation.Build2.Server.ObjectModel;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class BuildDefinitionService : IBuildDefinitionService, IVssFrameworkService
  {
    internal const string BuildDefinitionCache = "$BuildDefinitionCache";
    private const string TraceLayer = "BuildDefinitionService";
    private readonly IBuildSecurityProvider SecurityProvider;
    private const int DefaultGetCount = 10000;
    private static readonly RegistryQuery s_defaultCountQuery = new RegistryQuery("/Configuration/Build/defaultMaxDefinitions");
    private static readonly ReadOnlyCollection<string> fieldsToIgnoreCollection = new ReadOnlyCollection<string>((IList<string>) new List<string>()
    {
      "RepositoryString",
      "ProcessParametersString",
      "BuildOptionsString",
      "Url",
      "Comment",
      "QueueStatus"
    });

    public BuildDefinitionService()
      : this((IBuildSecurityProvider) new BuildSecurityProvider())
    {
    }

    internal BuildDefinitionService(IBuildSecurityProvider securityProvider) => this.SecurityProvider = securityProvider;

    public BuildDefinition AddDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      DefinitionUpdateOptions options = null)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (AddDefinition)))
      {
        BuildProcess process1 = definition.Process;
        if ((process1 != null ? (process1.Type == 1 ? 1 : 0) : 0) != 0 && requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, "ms.vss-build-web.disable-classic-build-pipeline-creation"))
          throw new InvalidOperationException(BuildServerResources.ClassicPipelinesDisabled());
        this.ValidateDefinition(requestContext, definition);
        this.SecurityProvider.CheckFolderPermission(requestContext, definition.ProjectId, definition.Path, Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition);
        foreach (BuildCompletionTrigger completionTrigger in definition.BuildCompletionTriggers)
        {
          IBuildSecurityProvider securityProvider = this.SecurityProvider;
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId = completionTrigger.ProjectId;
          MinimalBuildDefinition definition1 = new MinimalBuildDefinition();
          definition1.Id = completionTrigger.DefinitionId;
          definition1.Path = completionTrigger.Path;
          definition1.ProjectId = completionTrigger.ProjectId;
          int viewBuildDefinition = Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuildDefinition;
          securityProvider.CheckDefinitionPermission(requestContext1, projectId, definition1, viewBuildDefinition);
        }
        if (this.ShouldVerifyQueuePermissions(requestContext, definition))
          requestContext.GetService<IDistributedTaskPoolService>().VerifyQueuePermission(requestContext, definition.ProjectId, definition.Queue);
        IBuildSourceProvider buildSourceProvider = (IBuildSourceProvider) null;
        if (definition.Repository != null)
        {
          buildSourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type);
          buildSourceProvider.CheckEndpointAuthorization(requestContext, definition.ProjectId, definition.Repository);
          buildSourceProvider.SetRepositoryDefaultInfo(requestContext, definition.ProjectId, definition.Repository);
        }
        new BuildProcessValidator().Validate(requestContext, definition, options: options);
        if (definition.Process is YamlCompatProcess process2)
          process2.Resources.Clear();
        BuildProcessResources resources1 = (BuildProcessResources) null;
        BuildProcess process3 = definition.Process;
        if ((process3 != null ? (process3.Type == 2 ? 1 : 0) : 0) != 0)
        {
          try
          {
            resources1 = definition.LoadYamlPipeline(requestContext, options?.SourceBranch, options?.SourceVersion, true).Environment.Resources.ToBuildProcessResources();
          }
          catch (Exception ex)
          {
            requestContext.TraceException(nameof (BuildDefinitionService), ex);
          }
        }
        if (options?.SecretSource != null)
        {
          this.ValidateSecretSourceDefinition(requestContext, definition, options.SecretSource);
          this.ReadMissingSecrets(requestContext.Elevate(), definition, options.SecretSource);
        }
        List<BuildRepository> definitionRepositories;
        this.GetAllRepositoriesForDefinition(requestContext, definition, out definitionRepositories);
        ITeamFoundationEventService service1 = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo(12030003, "Service", "Publishing BuildDefinitonChangingEvent decision point:  adding build definition {0}", (object) definition.Id);
        service1.PublishDecisionPoint(requestContext, (object) new BuildDefinitionChangingEvent(AuditAction.Add, (BuildDefinition) null, definition, (List<BuildRepository>) null, definitionRepositories));
        DefinitionSecrets secrets = this.ExtractSecrets(requestContext, definition);
        this.UpdateScheduleTriggerInfo(definition);
        this.AddPollingTriggerInfo(definition);
        if (definition.Repository != null)
          buildSourceProvider.BeforeSerialize(requestContext, definition.Repository);
        Guid requestedBy = options != null ? options.AuthorId : Guid.Empty;
        if (requestedBy == Guid.Empty)
          requestedBy = requestContext.GetUserId(true);
        definition.ConvertTriggerPathsToProjectId(requestContext);
        definition.ConvertTaskParametersToProjectId(requestContext);
        definition.ConvertVariablesToProjectId(requestContext);
        definition.ConvertVariableGroupsToReferences(requestContext);
        this.FixPhaseRefNamesAndDependencies(requestContext, definition.Process);
        definition.ForceEnablePublicProjectBadges(requestContext);
        if (definition.Repository != null)
          definition.Repository.Id = SourceProviderHelper.NormalizeRepositoryId(requestContext, definition.Repository.Type, definition.Repository.Id, definition.Repository.Name, true);
        if (!requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build2.Service.AllowNumericFolderNames") && !string.IsNullOrEmpty(definition.Path))
        {
          IBuildFolderService service2 = requestContext.GetService<IBuildFolderService>();
          IList<Folder> folders = service2.GetFolders(requestContext, definition.ProjectId, definition.Path, FolderQueryOrder.None);
          if (folders == null || folders.Count == 0)
            service2.AddFolder(requestContext, definition.ProjectId, definition.Path, new Folder()
            {
              Path = definition.Path
            });
        }
        BuildDefinition newDefinition;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          newDefinition = component.AddDefinition(definition, requestedBy);
        if (resources1 != null)
        {
          if (!requestContext.IsFeatureEnabled("Pipelines.Checks.AuthChecksAsyncFlow"))
          {
            try
            {
              foreach (ResourceReference allResource in resources1.AllResources)
                allResource.Authorized = true;
              PipelineProcessResources resources = resources1.ToPipelineProcessResources();
              IPipelineResourceAuthorizationService resourceAuthorizationService = requestContext.GetService<IPipelineResourceAuthorizationService>();
              requestContext.RunSynchronously<PipelineProcessResources>((Func<Task<PipelineProcessResources>>) (() => resourceAuthorizationService.UpdateResourcesAsync(requestContext, newDefinition.ProjectId, resources, newDefinition.Id)));
            }
            catch (Exception ex)
            {
              requestContext.TraceException(nameof (BuildDefinitionService), ex);
            }
          }
        }
        if (definition.ParentDefinition != null && !this.SecurityProvider.HasDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition.ParentDefinition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuildDefinition, true))
          definition.ParentDefinition = (BuildDefinition) null;
        newDefinition.Properties = definition.Properties;
        if (newDefinition.Properties != null && newDefinition.Properties.Count > 0)
          requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, newDefinition.CreateArtifactSpec(requestContext), newDefinition.Properties.Convert());
        if (definition.Triggers.Any<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.GatedCheckIn)))
          requestContext.GetService<GatedBuildDefinitionCache>().Invalidate(requestContext, (IEnumerable<Tuple<Guid, int>>) null);
        if (newDefinition.Repository != null)
          buildSourceProvider.AfterDeserialize(requestContext, newDefinition.Repository);
        this.StoreSecrets(requestContext.Elevate(), newDefinition, secrets);
        Dictionary<BuildDefinition, List<BuildRepository>> dictionary = new Dictionary<BuildDefinition, List<BuildRepository>>();
        BuildProcess process4 = newDefinition.Process;
        if ((process4 != null ? (process4.Type == 2 ? 1 : 0) : 0) != 0)
        {
          if (!definition.Triggers.OfType<ScheduleTrigger>().Any<ScheduleTrigger>((Func<ScheduleTrigger, bool>) (trigger => trigger.Schedules.Count > 0)))
          {
            string str = buildSourceProvider.NormalizeSourceBranch(newDefinition?.Repository?.DefaultBranch, newDefinition);
            CronScheduleHelper.UpdateCronSchedules(requestContext, new List<BuildDefinition>()
            {
              newDefinition
            }, (RepositoryUpdateInfo) null, true, new List<string>()
            {
              str
            });
          }
          dictionary = FilteredBuildTriggerHelper.UpdateResourceRepositories(requestContext, new List<BuildDefinition>()
          {
            newDefinition
          }, (RepositoryUpdateInfo) null, true);
        }
        else
          this.AddScheduleTriggerJobs(requestContext, newDefinition);
        this.AddPollingTriggerJob(requestContext, newDefinition);
        newDefinition.UpdateReferences(requestContext);
        newDefinition.PopulateVariableGroups(requestContext);
        BuildProcess process5 = newDefinition.Process;
        if ((process5 != null ? (process5.Type == 2 ? 1 : 0) : 0) != 0)
        {
          if (requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
          {
            try
            {
              YamlProcess process6 = newDefinition.GetProcess<YamlProcess>();
              PipelineBuilder pipelineBuilder = newDefinition.GetPipelineBuilder(requestContext, authorizeNewResources: true);
              RepositoryResource repositoryResource = newDefinition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, newDefinition.Repository?.DefaultBranch);
              requestContext.GetService<IArtifactYamlTriggerService>().AddTriggers(requestContext, newDefinition.ProjectId, newDefinition.Id, repositoryResource, process6.YamlFilename, pipelineBuilder, options?.SourceVersion, definition.Repository?.Url);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(nameof (BuildDefinitionService), ex);
            }
          }
        }
        List<BuildRepository> resourceRepositories = (List<BuildRepository>) null;
        dictionary?.TryGetValue(newDefinition, out resourceRepositories);
        requestContext.TraceInfo(12030004, "Service", "Publishing BuildDefinitionChangedEvent:  adding definition {0}", (object) newDefinition.Id);
        service1.PublishNotification(requestContext, (object) new BuildDefinitionChangedEvent(AuditAction.Add, newDefinition, resourceRepositories));
        IVssRequestContext requestContext2 = requestContext;
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["PipelineId"] = (object) newDefinition.Id;
        data["PipelineName"] = (object) newDefinition.Name;
        data["PipelineScope"] = (object) newDefinition.Path;
        data["ProjectName"] = (object) newDefinition.ProjectName;
        Guid projectId1 = definition.ProjectId;
        Guid targetHostId = new Guid();
        Guid projectId2 = projectId1;
        requestContext2.LogAuditEvent("Pipelines.PipelineCreated", data, targetHostId, projectId2);
        return newDefinition;
      }
    }

    public void DisableScheduledBuilds(IVssRequestContext requestContext, Guid projectId)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (DisableScheduledBuilds)))
      {
        if (!requestContext.IsSystemContext)
        {
          requestContext.TraceError("Service", "RequestContext is not a system context");
          throw new UnexpectedRequestContextTypeException(RequestContextType.SystemContext);
        }
        IEnumerable<BuildDefinition> source = requestContext.RunSynchronously<IEnumerable<BuildDefinition>>((Func<Task<IEnumerable<BuildDefinition>>>) (() => this.GetDefinitionsWithSchedulesAsync(requestContext, projectId)));
        requestContext.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(requestContext, (IEnumerable<Guid>) source.SelectMany<BuildDefinition, Guid>((Func<BuildDefinition, IEnumerable<Guid>>) (d => d.GetScheduleTriggerJobIds())).ToArray<Guid>());
      }
    }

    public void EnableScheduledBuilds(IVssRequestContext requestContext, Guid projectId)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (EnableScheduledBuilds)))
      {
        if (!requestContext.IsSystemContext)
        {
          requestContext.TraceError("Service", "RequestContext is not a system context");
          throw new UnexpectedRequestContextTypeException(RequestContextType.SystemContext);
        }
        IEnumerable<BuildDefinition> source = requestContext.RunSynchronously<IEnumerable<BuildDefinition>>((Func<Task<IEnumerable<BuildDefinition>>>) (() => this.GetDefinitionsWithSchedulesAsync(requestContext, projectId)));
        IEnumerable<Guid> jobIds = source.SelectMany<BuildDefinition, Guid>((Func<BuildDefinition, IEnumerable<Guid>>) (d => d.GetScheduleTriggerJobIds()));
        ITeamFoundationJobService jobService = requestContext.GetService<ITeamFoundationJobService>();
        HashSet<Guid> check = jobService.QueryJobDefinitions(requestContext, jobIds).Where<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (jd => jd != null)).ToHashSet<TeamFoundationJobDefinition, Guid>((Func<TeamFoundationJobDefinition, Guid>) (jd => jd.JobId));
        jobService.UpdateJobDefinitions(requestContext, Enumerable.Empty<Guid>(), (IEnumerable<TeamFoundationJobDefinition>) source.SelectMany<BuildDefinition, TeamFoundationJobDefinition>((Func<BuildDefinition, IEnumerable<TeamFoundationJobDefinition>>) (d => d.GetScheduleJobDefinitions(requestContext, jobService.IsIgnoreDormancyPermitted).Where<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (jd => !check.Contains(jd.JobId))))).ToArray<TeamFoundationJobDefinition>());
      }
    }

    public async Task<BuildDefinition> GetDefinitionAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int? definitionVersion = null,
      IEnumerable<string> propertyFilters = null,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false)
    {
      BuildDefinitionService definitionService = this;
      requestContext.AssertAsyncExecutionEnabled();
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDefinitionAsync)))
      {
        BuildDefinition definition1;
        BuildDefinitionService.TryGetCachedBuildDefinition(requestContext, projectId, definitionId, definitionVersion, out definition1);
        if (definition1 == null)
        {
          using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
            definition1 = await bc.GetDefinitionAsync(projectId, definitionId, definitionVersion, includeDeleted, minMetricsTime, includeLatestBuilds);
          if (definition1 == null)
            return (BuildDefinition) null;
        }
        BuildDefinition definition2 = definitionService.PostProcessDefinitions(requestContext, (IList<BuildDefinition>) new BuildDefinition[1]
        {
          definition1
        }).FirstOrDefault<BuildDefinition>();
        if (definition2 == null)
          return (BuildDefinition) null;
        BuildDefinitionService.UpdateDefinitionCache(requestContext, projectId, definitionId, definitionVersion, ref definition2);
        if (propertyFilters != null && propertyFilters.Any<string>())
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, definition2.CreateArtifactSpec(requestContext), propertyFilters))
          {
            foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
              definition2.Properties = current.PropertyValues.Convert();
          }
        }
        definitionService.ResolveDefinitionTaskVersions(requestContext, definition2);
        return definition2;
      }
    }

    public IEnumerable<BuildDefinitionSummary> GetDefinitionsSummary(
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
      bool includeDrafts = false)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      this.OverrideDefaultCount(requestContext, ref count);
      IEnumerable<BuildDefinition> definitionsInternal = this.GetDefinitionsInternal(requestContext, projectId, name, triggers, (string) null, repositoryType, queryOrder, count, minLastModifiedTime, maxLastModifiedTime, lastDefinitionName, minMetricsTime, path, builtAfter, notBuiltAfter, options, tagFilters, includeLatestBuilds, taskIdFilter, processType);
      bool checkQueueBuildPermissionOnPipeline = requestContext.IsFeatureEnabled("Build2.CheckQueueBuildPermissionOnPipeline");
      int count1 = count;
      return (IEnumerable<BuildDefinitionSummary>) definitionsInternal.Take<BuildDefinition>(count1).Select<BuildDefinition, BuildDefinitionSummary>((Func<BuildDefinition, BuildDefinitionSummary>) (buildDef => new BuildDefinitionSummary(buildDef, !checkQueueBuildPermissionOnPipeline || this.SecurityProvider.HasDefinitionPermission(requestContext, buildDef.ProjectId, (MinimalBuildDefinition) buildDef, Microsoft.TeamFoundation.Build.Common.BuildPermissions.QueueBuilds, true)))).ToArray<BuildDefinitionSummary>();
    }

    public IEnumerable<BuildDefinition> GetDefinitions(
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
      int? processType = null)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      this.OverrideDefaultCount(requestContext, ref count);
      return this.GetDefinitionsAndProcessInternal(requestContext, projectId, name, triggers, (string) null, repositoryType, queryOrder, count, minLastModifiedTime, maxLastModifiedTime, lastDefinitionName, minMetricsTime, path, builtAfter, notBuiltAfter, options, tagFilters, includeLatestBuilds, taskIdFilter, processType);
    }

    public IEnumerable<BuildDefinition> GetAllDefinitionsForPath(
      IVssRequestContext requestContext,
      Guid projectId,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      int count = 10000,
      string path = null,
      bool includeLatestBuilds = false)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      this.OverrideDefaultCount(requestContext, ref count);
      return this.GetDefinitionsAndProcessInternal(requestContext, projectId, "*", DefinitionTriggerType.All, (string) null, (string) null, queryOrder, count, new DateTime?(), new DateTime?(), (string) null, new DateTime?(), path, includeLatestBuilds: (includeLatestBuilds ? 1 : 0) != 0, exclusions: new ExcludePopulatingDefinitionResources()
      {
        Endpoints = true,
        Queues = true,
        References = true,
        VariableGroups = true
      });
    }

    public IEnumerable<BuildDefinition> GetDefinitionsForRepository(
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
      int? processType = null)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      this.OverrideDefaultCount(requestContext, ref count);
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDefinitionsForRepository)))
      {
        repositoryId = SourceProviderHelper.NormalizeRepositoryId(requestContext, repositoryType, repositoryId, name);
        return this.GetDefinitionsAndProcessInternal(requestContext, projectId, name, triggers, repositoryId, repositoryType, queryOrder, count, minLastModifiedTime, maxLastModifiedTime, lastDefinitionName, minMetricsTime, path, builtAfter, notBuiltAfter, options, tagFilters, includeLatestBuilds, taskIdFilter, processType);
      }
    }

    public IEnumerable<BuildDefinition> GetYamlDefinitionsForRepository(
      IVssRequestContext requestContext,
      List<Guid> projectIds,
      string repositoryId,
      string repositoryType,
      int maxDefinitions = 10000)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetYamlDefinitionsForRepository)))
      {
        this.OverrideDefaultCount(requestContext, ref maxDefinitions);
        repositoryId = SourceProviderHelper.NormalizeRepositoryId(requestContext, repositoryType, repositoryId);
        HashSet<int> dataspaceIds = new HashSet<int>();
        IDataspaceService service = requestContext.GetService<IDataspaceService>();
        foreach (Guid projectId in projectIds)
        {
          Dataspace dataspace = service.QueryDataspace(requestContext, "Build", projectId, false);
          if (dataspace != null)
            dataspaceIds.Add(dataspace.DataspaceId);
        }
        IList<BuildDefinition> definitionsForRepository;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          definitionsForRepository = (IList<BuildDefinition>) component.GetYamlDefinitionsForRepository(dataspaceIds, repositoryId, repositoryType, maxDefinitions);
        HashSet<int> intSet = new HashSet<int>();
        BuildDefinition[] array = this.FilterDefinitionsAndUpdateContinuationData(requestContext, definitionsForRepository, out HashSet<int> _, out bool _).Take<BuildDefinition>(maxDefinitions).ToArray<BuildDefinition>();
        this.PostProcessFilteredDefinitions(requestContext, (IReadOnlyList<BuildDefinition>) array);
        return (IEnumerable<BuildDefinition>) array;
      }
    }

    public IEnumerable<BuildDefinition> GetCIDefinitions(
      IVssRequestContext requestContext,
      List<Guid> projectIds,
      string repositoryType,
      string repositoryId,
      int count = 10000)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetCIDefinitions)))
      {
        this.OverrideDefaultCount(requestContext, ref count);
        repositoryId = SourceProviderHelper.NormalizeRepositoryId(requestContext, repositoryType, repositoryId);
        HashSet<int> dataspaceIds = new HashSet<int>();
        IDataspaceService service = requestContext.GetService<IDataspaceService>();
        foreach (Guid projectId in projectIds)
        {
          Dataspace dataspace = service.QueryDataspace(requestContext, "Build", projectId, false);
          if (dataspace != null)
            dataspaceIds.Add(dataspace.DataspaceId);
        }
        IList<BuildDefinition> ciDefinitions;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          ciDefinitions = (IList<BuildDefinition>) component.GetCIDefinitions(dataspaceIds, repositoryId, repositoryType, count);
        HashSet<int> intSet = new HashSet<int>();
        BuildDefinition[] array = this.FilterDefinitionsAndUpdateContinuationData(requestContext, ciDefinitions, out HashSet<int> _, out bool _).Take<BuildDefinition>(count).ToArray<BuildDefinition>();
        this.PostProcessFilteredDefinitions(requestContext, (IReadOnlyList<BuildDefinition>) array);
        return (IEnumerable<BuildDefinition>) array;
      }
    }

    public IEnumerable<BuildDefinition> GetDefinitionsWithTriggers(
      IVssRequestContext requestContext,
      List<Guid> projectIds,
      string repositoryType,
      string repositoryId,
      DefinitionTriggerType triggerFilter,
      int count = 10000)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDefinitionsWithTriggers)))
      {
        this.OverrideDefaultCount(requestContext, ref count);
        repositoryId = SourceProviderHelper.NormalizeRepositoryId(requestContext, repositoryType, repositoryId);
        HashSet<int> intSet = new HashSet<int>();
        IDataspaceService service = requestContext.GetService<IDataspaceService>();
        foreach (Guid projectId in projectIds)
        {
          Dataspace dataspace = service.QueryDataspace(requestContext, "Build", projectId, false);
          if (dataspace != null)
            intSet.Add(dataspace.DataspaceId);
        }
        IList<BuildDefinition> definitionsWithTriggers;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          definitionsWithTriggers = (IList<BuildDefinition>) component.GetDefinitionsWithTriggers(intSet, repositoryId, repositoryType, (int) triggerFilter, count);
        requestContext.TraceInfo(nameof (BuildDefinitionService), "RepositoryId: {0}; dataspaceIds: {1}; definitions: {2}", (object) repositoryId, (object) string.Join<int>(", ", (IEnumerable<int>) intSet.ToList<int>()), (object) string.Join(", ", (IEnumerable<string>) definitionsWithTriggers.Select<BuildDefinition, string>((Func<BuildDefinition, string>) (x => x.Name)).ToList<string>()));
        HashSet<int> excludedDefinitionIds;
        List<BuildDefinition> source = this.FilterDefinitionsAndUpdateContinuationData(requestContext, definitionsWithTriggers, out excludedDefinitionIds, out bool _);
        requestContext.TraceInfo(nameof (BuildDefinitionService), "Definitions After Filter: {0}; Filtered Out Definitions: {1}", (object) string.Join(", ", (IEnumerable<string>) source.Select<BuildDefinition, string>((Func<BuildDefinition, string>) (x => x.Name)).ToList<string>()), (object) string.Join<int>(", ", (IEnumerable<int>) excludedDefinitionIds.ToList<int>()));
        BuildDefinition[] array = source.Take<BuildDefinition>(count).ToArray<BuildDefinition>();
        this.PostProcessFilteredDefinitions(requestContext, (IReadOnlyList<BuildDefinition>) array);
        requestContext.TraceInfo(nameof (BuildDefinitionService), "Definitions After PostProcessFilteredDefinitions: {0}", (object) string.Join(", ", (IEnumerable<string>) ((IEnumerable<BuildDefinition>) array).Select<BuildDefinition, string>((Func<BuildDefinition, string>) (x => x.Name)).ToList<string>()));
        return (IEnumerable<BuildDefinition>) array;
      }
    }

    public IEnumerable<BuildDefinition> GetDefinitionHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDefinitionHistory)))
      {
        IList<BuildDefinition> definitionHistory;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          definitionHistory = (IList<BuildDefinition>) component.GetDefinitionHistory(projectId, definitionId);
        return (IEnumerable<BuildDefinition>) this.PostProcessDefinitions(requestContext, definitionHistory);
      }
    }

    public void DestroyDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (DestroyDefinition)))
      {
        BuildDefinition definition = (BuildDefinition) null;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          definition = requestContext.RunSynchronously<BuildDefinition>((Func<Task<BuildDefinition>>) (() => bc.GetDefinitionAsync(projectId, definitionId, new int?(), true)));
        if (definition == null)
        {
          requestContext.TraceInfo(0, "Service", "Definition not found {0}", (object) definitionId);
        }
        else
        {
          this.SecurityProvider.CheckDefinitionPermission(requestContext, projectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.DeleteBuildDefinition);
          using (Build2Component component = requestContext.CreateComponent<Build2Component>())
            component.DestroyDefinition(projectId, definitionId);
          try
          {
            requestContext.GetService<IPipelineResourceAuthorizationService>().RemoveDefinitionSpecificAuthorization(requestContext, projectId, definitionId);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(nameof (BuildDefinitionService), ex);
          }
          this.DeleteDefinitionSecrets(requestContext.Elevate(), definition);
        }
      }
    }

    public BuildDefinition UpdateDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      bool authorizeNewResources,
      DefinitionUpdateOptions options = null)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (UpdateDefinition)))
      {
        if (requestContext.IsServicingContext & authorizeNewResources)
          throw new InvalidOperationException(BuildServerResources.UpdateDefinition_ServicingContextCannotAuthorizeResources());
        BuildDefinition originalDefinition = (BuildDefinition) null;
        bool flag1 = false;
        if (requestContext.IsFeatureEnabled("Build2.Service.SkipValidationsForDisableBuildDefinition") && definition.QueueStatus == DefinitionQueueStatus.Disabled)
        {
          originalDefinition = this.GetCurrentDefinition(requestContext, definition);
          flag1 = BuildDefinitionService.IsDefinitionJustBeingDisabled(requestContext, definition, originalDefinition);
        }
        if (!requestContext.IsServicingContext && !flag1)
          this.ValidateDefinition(requestContext, definition, true);
        if (originalDefinition == null)
          originalDefinition = this.GetCurrentDefinition(requestContext, definition);
        this.SecurityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition);
        foreach (BuildCompletionTrigger completionTrigger in definition.BuildCompletionTriggers)
        {
          BuildCompletionTrigger buildCompletedTrigger = completionTrigger;
          if (!originalDefinition.BuildCompletionTriggers.Any<BuildCompletionTrigger>((Func<BuildCompletionTrigger, bool>) (x => x.DefinitionId == buildCompletedTrigger.DefinitionId)))
          {
            IBuildSecurityProvider securityProvider = this.SecurityProvider;
            IVssRequestContext requestContext1 = requestContext;
            Guid projectId = buildCompletedTrigger.ProjectId;
            MinimalBuildDefinition definition1 = new MinimalBuildDefinition();
            definition1.Id = buildCompletedTrigger.DefinitionId;
            definition1.ProjectId = buildCompletedTrigger.ProjectId;
            definition1.Path = buildCompletedTrigger.Path;
            int viewBuildDefinition = Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuildDefinition;
            securityProvider.CheckDefinitionPermission(requestContext1, projectId, definition1, viewBuildDefinition);
          }
        }
        if (this.ShouldVerifyQueuePermissions(requestContext, definition))
          requestContext.GetService<IDistributedTaskPoolService>().VerifyQueuePermission(requestContext, definition.ProjectId, definition.Queue, originalDefinition.Queue);
        IBuildSourceProvider buildSourceProvider = (IBuildSourceProvider) null;
        if (definition.Repository != null)
        {
          buildSourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type);
          buildSourceProvider.CheckEndpointAuthorization(requestContext, definition.ProjectId, definition.Repository, originalDefinition.Repository);
          if (!flag1)
            buildSourceProvider.SetRepositoryDefaultInfo(requestContext, definition.ProjectId, definition.Repository);
        }
        if (!flag1)
          new BuildProcessValidator().Validate(requestContext, definition, originalDefinition);
        List<BuildRepository> definitionRepositories1;
        this.GetAllRepositoriesForDefinition(requestContext, originalDefinition, out definitionRepositories1);
        List<BuildRepository> definitionRepositories2;
        this.GetAllRepositoriesForDefinition(requestContext, definition, out definitionRepositories2);
        ITeamFoundationEventService service1 = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo(12030006, "Service", "Publishing BuildDefinitionChangingEvent decision point: updating definition {0}", (object) definition.Id);
        service1.PublishDecisionPoint(requestContext, (object) new BuildDefinitionChangingEvent(AuditAction.Update, originalDefinition, definition, definitionRepositories1, definitionRepositories2));
        if (options?.SecretSource != null)
        {
          this.ValidateSecretSourceDefinition(requestContext, definition, options.SecretSource);
          this.ReadMissingSecrets(requestContext.Elevate(), definition, options.SecretSource);
        }
        else
          this.ReadMissingSecrets(requestContext.Elevate(), definition, originalDefinition);
        DefinitionSecrets secrets = this.ExtractSecrets(requestContext, definition);
        BuildProcess process1 = definition.Process;
        if ((process1 != null ? (process1.Type == 2 ? 1 : 0) : 0) != 0)
        {
          BuildProcess process2 = originalDefinition.Process;
          if ((process2 != null ? (process2.Type == 2 ? 1 : 0) : 0) != 0)
          {
            YamlProcess process3 = originalDefinition.GetProcess<YamlProcess>();
            YamlProcess process4 = definition.GetProcess<YamlProcess>();
            bool flag2 = definition.Triggers.Where<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.Schedule)).ToList<BuildTrigger>().Any<BuildTrigger>((Func<BuildTrigger, bool>) (x => (x as ScheduleTrigger).Schedules.Count > 0));
            if (process4.YamlFilename != process3.YamlFilename)
            {
              if (!flag2)
              {
                requestContext.TraceAlways(12030006, TraceLevel.Info, "Build2", "Service", string.Format("Attempting to delete the schedules for definition id {0} and name {1} because the definition's YAML  ", (object) definition?.Id, (object) definition?.Name) + "was updated from " + process3.YamlFilename + " to " + process4.YamlFilename);
                this.DeleteCronSchedulesForDefinitions(requestContext, definition.ProjectId, new List<int>()
                {
                  definition.Id
                });
                string str = buildSourceProvider.NormalizeSourceBranch(definition?.Repository?.DefaultBranch, definition);
                CronScheduleHelper.UpdateCronSchedules(requestContext, new List<BuildDefinition>()
                {
                  definition
                }, (RepositoryUpdateInfo) null, true, new List<string>()
                {
                  str
                });
              }
              Dictionary<BuildDefinition, List<BuildRepository>> dictionary = new Dictionary<BuildDefinition, List<BuildRepository>>();
              FilteredBuildTriggerHelper.UpdateResourceRepositories(requestContext, new List<BuildDefinition>()
              {
                definition
              }, (RepositoryUpdateInfo) null, true);
            }
          }
        }
        this.UpdateScheduleTriggerInfo(definition);
        this.UpdatePollingTriggerInfo(requestContext, definition, originalDefinition);
        if (definition.Repository != null)
          buildSourceProvider.BeforeSerialize(requestContext, definition.Repository);
        Guid requestedBy = options != null ? options.AuthorId : Guid.Empty;
        if (requestedBy == Guid.Empty)
          requestedBy = requestContext.GetUserId(true);
        definition.ConvertTriggerPathsToProjectId(requestContext);
        definition.ConvertTaskParametersToProjectId(requestContext);
        definition.ConvertVariablesToProjectId(requestContext);
        definition.ConvertVariableGroupsToReferences(requestContext);
        this.FixPhaseRefNamesAndDependencies(requestContext, definition.Process);
        definition.ForceEnablePublicProjectBadges(requestContext);
        if (definition.Repository != null)
        {
          definition.Repository.Id = SourceProviderHelper.NormalizeRepositoryId(requestContext, definition.Repository.Type, definition.Repository.Id, definition.Repository.Name, true);
          if (definition.Repository.Type != null && definition.Repository.Type != "TfsVersionControl")
            definition.Repository.DefaultBranch = buildSourceProvider.NormalizeSourceBranch(definition.Repository.DefaultBranch, definition);
        }
        BuildProcessResources other = (BuildProcessResources) null;
        BuildProcessResources processResources = (BuildProcessResources) null;
        BuildProcess process5 = definition.Process;
        if ((process5 != null ? (process5.Type == 2 ? 1 : 0) : 0) != 0)
        {
          IBuildResourceAuthorizationServiceInternal resourceAuthorizationService = requestContext.GetService<IBuildResourceAuthorizationServiceInternal>();
          processResources = requestContext.RunSynchronously<BuildProcessResources>((Func<Task<BuildProcessResources>>) (() => resourceAuthorizationService.GetAuthorizedResourcesAsync(requestContext, originalDefinition)));
          try
          {
            other = definition.LoadYamlPipeline(requestContext, options?.SourceBranch, options?.SourceVersion, true, processResources).Environment.Resources.ToBuildProcessResources();
          }
          catch (Exception ex)
          {
            requestContext.TraceException(nameof (BuildDefinitionService), ex);
          }
        }
        IPipelineResourceAuthorizationService pipelineResourceAuthorizationService = requestContext.GetService<IPipelineResourceAuthorizationService>();
        BuildDefinition buildDefinition1 = (BuildDefinition) null;
        if (definition.Process is YamlCompatProcess process6)
        {
          process6.Resources.Clear();
          process6.Resources.MergeWith(processResources);
          if (other != null & authorizeNewResources)
          {
            foreach (ResourceReference allResource in other.AllResources)
              allResource.DefinitionId = new int?(definition.Id);
            process6.Resources.MergeWith(other);
          }
          Guid serviceEndpointId;
          if (!process6.SupportsYamlRepositoryEndpointAuthorization() && definition.Repository.TryGetServiceEndpointId(out serviceEndpointId))
          {
            ISet<ServiceEndpointReference> endpoints = process6.Resources.Endpoints;
            ServiceEndpointReference endpointReference = new ServiceEndpointReference();
            endpointReference.Id = serviceEndpointId;
            endpointReference.DefinitionId = new int?(definition.Id);
            endpointReference.Authorized = true;
            endpoints.Add(endpointReference);
          }
          foreach (ResourceReference allResource in process6.Resources.AllResources)
            allResource.Authorized = true;
          PipelineProcessResources pipelineResources = process6.Resources.Where((Func<ResourceReference, bool>) (r => r.DefinitionId.HasValue)).ToPipelineProcessResources();
          requestContext.RunSynchronously<PipelineProcessResources>((Func<Task<PipelineProcessResources>>) (() => pipelineResourceAuthorizationService.UpdateResourcesAsync(requestContext, definition.ProjectId, pipelineResources, definition.Id)));
          BuildDefinition buildDefinition2 = definition;
          YamlProcess yamlProcess = new YamlProcess();
          yamlProcess.YamlFilename = process6.YamlFilename;
          yamlProcess.Version = 3;
          buildDefinition2.Process = (BuildProcess) yamlProcess;
        }
        else if (authorizeNewResources && definition.Process.Type == 2 && other != null && (!processResources.Endpoints.SetEquals((IEnumerable<ServiceEndpointReference>) other.Endpoints) || !processResources.Files.SetEquals((IEnumerable<SecureFileReference>) other.Files) || !processResources.Queues.SetEquals((IEnumerable<AgentPoolQueueReference>) other.Queues) || !processResources.VariableGroups.SetEquals((IEnumerable<VariableGroupReference>) other.VariableGroups)))
        {
          foreach (ResourceReference allResource in other.AllResources)
            allResource.DefinitionId = new int?(definition.Id);
          processResources.MergeWith(other);
          foreach (ResourceReference allResource in processResources.AllResources)
            allResource.Authorized = true;
          PipelineProcessResources pipelineResources = processResources.Where((Func<ResourceReference, bool>) (r => r.DefinitionId.HasValue)).ToPipelineProcessResources();
          requestContext.RunSynchronously<PipelineProcessResources>((Func<Task<PipelineProcessResources>>) (() => pipelineResourceAuthorizationService.UpdateResourcesAsync(requestContext, definition.ProjectId, pipelineResources, definition.Id)));
        }
        if (!requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build2.Service.AllowNumericFolderNames") && !string.IsNullOrEmpty(definition.Path) && originalDefinition.Path != definition.Path)
        {
          IBuildFolderService service2 = requestContext.GetService<IBuildFolderService>();
          IList<Folder> folders = service2.GetFolders(requestContext, definition.ProjectId, definition.Path, FolderQueryOrder.None);
          if (folders == null || folders.Count == 0)
            service2.AddFolder(requestContext, definition.ProjectId, definition.Path, new Folder()
            {
              Path = definition.Path
            });
        }
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        {
          buildDefinition1 = component.UpdateDefinition(definition, requestedBy, originalDefinition.GetToken(), definition.GetToken());
          if (buildDefinition1 == null)
            requestContext.TraceInfo(12030364, "Service", "The updated definition is empty, AuthorId: {0}, Original definition token {1}, New definition token {2}", (object) requestedBy.ToString(), (object) originalDefinition.GetToken(), (object) definition.GetToken());
        }
        if (definition.ParentDefinition != null && !this.SecurityProvider.HasDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition.ParentDefinition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuildDefinition, true))
          definition.ParentDefinition = (BuildDefinition) null;
        try
        {
          if (buildDefinition1.Repository != null)
            buildSourceProvider.AfterDeserialize(requestContext, buildDefinition1.Repository);
          buildDefinition1.Properties = definition.Properties;
          if (buildDefinition1.Properties != null && buildDefinition1.Properties.Count > 0)
            requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, buildDefinition1.CreateArtifactSpec(requestContext), buildDefinition1.Properties.Convert());
          if (buildDefinition1.Triggers.Any<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.GatedCheckIn)) || originalDefinition.Triggers.Any<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.GatedCheckIn)))
            requestContext.GetService<GatedBuildDefinitionCache>().Invalidate(requestContext, (IEnumerable<Tuple<Guid, int>>) null);
          if (buildDefinition1.QueueStatus == DefinitionQueueStatus.Enabled && originalDefinition.QueueStatus != DefinitionQueueStatus.Enabled)
            this.EvaluatePausedBuildsQueue(requestContext, definition);
          buildDefinition1.PopulateProperties(requestContext);
          buildDefinition1.PopulateVariableGroups(requestContext);
          this.StoreSecrets(requestContext.Elevate(), buildDefinition1, secrets);
          this.UpdateScheduleTriggerJobs(requestContext, originalDefinition, buildDefinition1);
          this.UpdatePollingTriggerJob(requestContext, originalDefinition, buildDefinition1);
          buildDefinition1.UpdateReferences(requestContext);
          if (BuildDefinitionServiceHelper.NeedUpdateTriggers(requestContext, originalDefinition, buildDefinition1))
          {
            try
            {
              YamlProcess process7 = definition.GetProcess<YamlProcess>();
              PipelineBuilder pipelineBuilder = definition.GetPipelineBuilder(requestContext, authorizeNewResources: true);
              RepositoryResource repositoryResource = definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, definition.Repository?.DefaultBranch);
              requestContext.GetService<IArtifactYamlTriggerService>().UpdateTriggers(requestContext, buildDefinition1.ProjectId, buildDefinition1.Id, repositoryResource, process7.YamlFilename, pipelineBuilder, options?.SourceVersion, definition.Repository.Url);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(nameof (BuildDefinitionService), ex);
            }
          }
          requestContext.TraceInfo(12030007, "Service", "Publishing BuildDefinitionChangedEvent: updated definition {0}", (object) buildDefinition1.Id);
          service1.PublishNotification(requestContext, (object) new BuildDefinitionChangedEvent(AuditAction.Update, buildDefinition1));
          IVssRequestContext requestContext2 = requestContext;
          Dictionary<string, object> data = new Dictionary<string, object>();
          data["PipelineId"] = (object) definition.Id;
          data["PipelineName"] = (object) definition.Name;
          data["PipelineScope"] = (object) definition.Path;
          data["PipelineRevision"] = (object) buildDefinition1.Revision;
          data["ProjectName"] = (object) definition.ProjectName;
          Guid projectId1 = definition.ProjectId;
          Guid targetHostId = new Guid();
          Guid projectId2 = projectId1;
          requestContext2.LogAuditEvent("Pipelines.PipelineModified", data, targetHostId, projectId2);
          return buildDefinition1;
        }
        catch (NullReferenceException ex)
        {
          requestContext.TraceException(12030364, TraceLevel.Error, "Build2", "Service", (Exception) ex);
          throw;
        }
      }
    }

    public void DeleteDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> definitionIds,
      Guid? authorId = null)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      if (definitionIds.Count == 0)
        return;
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (DeleteDefinitions)))
      {
        List<BuildDefinition> definitionsByIds = this.GetDefinitionsByIds(requestContext, projectId, (IEnumerable<int>) definitionIds, false, new DateTime?(), false, true, (ExcludePopulatingDefinitionResources) null);
        if (definitionsByIds.Count == 0)
          return;
        foreach (BuildDefinition definition in definitionsByIds)
          this.SecurityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.DeleteBuildDefinition);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        List<BuildRepository> definitionRepositories = new List<BuildRepository>();
        foreach (BuildDefinition buildDefinition in definitionsByIds)
        {
          this.GetAllRepositoriesForDefinition(requestContext, buildDefinition, out definitionRepositories);
          requestContext.TraceInfo(12030008, "Service", "Publishing BuildDefinitionChangingEvent decision point: deleting definition {0}", (object) buildDefinition.Id);
          service.PublishDecisionPoint(requestContext, (object) new BuildDefinitionChangingEvent(AuditAction.Delete, buildDefinition, (BuildDefinition) null, definitionRepositories, (List<BuildRepository>) null));
        }
        if (!authorId.HasValue || authorId.Value == Guid.Empty)
          authorId = new Guid?(requestContext.GetUserId(true));
        List<BuildData> buildsToCancel = (List<BuildData>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          component.DeleteDefinitions(projectId, definitionsByIds.Select<BuildDefinition, int>((Func<BuildDefinition, int>) (d => d.Id)), authorId.Value, out buildsToCancel);
        if (buildsToCancel != null)
          requestContext.GetService<BuildService>().CancelBuildsInternal(requestContext, projectId, (IEnumerable<BuildData>) buildsToCancel, authorId);
        if (definitionsByIds.SelectMany<BuildDefinition, BuildTrigger>((Func<BuildDefinition, IEnumerable<BuildTrigger>>) (definition => (IEnumerable<BuildTrigger>) definition.Triggers)).Select<BuildTrigger, DefinitionTriggerType>((Func<BuildTrigger, DefinitionTriggerType>) (trigger => trigger.TriggerType)).Any<DefinitionTriggerType>((Func<DefinitionTriggerType, bool>) (triggerType => triggerType == DefinitionTriggerType.GatedCheckIn)))
          requestContext.GetService<GatedBuildDefinitionCache>().Invalidate(requestContext, (IEnumerable<Tuple<Guid, int>>) null);
        this.DeleteScheduleTriggerJobs(requestContext, definitionsByIds);
        List<int> list = definitionsByIds.Select<BuildDefinition, int>((Func<BuildDefinition, int>) (d => d.Id)).ToList<int>();
        requestContext.TraceAlways(12030006, TraceLevel.Info, "Build2", "Service", string.Format("Attempting to remove the schedules for {0} definitions as they are being deleted", (object) definitionIds?.Count));
        this.DeleteCronSchedulesForDefinitions(requestContext, projectId, list);
        this.DeletePollingTriggerJob(requestContext, definitionsByIds);
        this.AddDeleteDefinitionJobs(requestContext, (IEnumerable<BuildDefinition>) definitionsByIds);
        if (requestContext.IsFeatureEnabled("DistributedTask.EnablePipelineTriggers"))
          requestContext.GetService<IArtifactYamlTriggerService>().DeleteTriggers(requestContext, projectId, definitionsByIds.Select<BuildDefinition, int>((Func<BuildDefinition, int>) (x => x.Id)));
        foreach (BuildDefinition definition in definitionsByIds)
        {
          requestContext.TraceInfo(12030009, "Service", "Publishing BuildDefinitionChangedEvent: deleted definition {0}", (object) definition.Id);
          service.PublishNotification(requestContext, (object) new BuildDefinitionChangedEvent(AuditAction.Delete, definition, definitionRepositories));
          IVssRequestContext requestContext1 = requestContext;
          Dictionary<string, object> data = new Dictionary<string, object>();
          data["PipelineId"] = (object) definition.Id;
          data["PipelineName"] = (object) definition.Name;
          data["PipelineScope"] = (object) definition.Path;
          data["ProjectName"] = (object) definition.ProjectName;
          Guid projectId1 = definition.ProjectId;
          Guid targetHostId = new Guid();
          Guid projectId2 = projectId1;
          requestContext1.LogAuditEvent("Pipelines.PipelineDeleted", data, targetHostId, projectId2);
        }
      }
    }

    public BuildDefinition RestoreDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      Guid? authorId = null,
      string comment = null)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (RestoreDefinition)))
      {
        BuildDefinition definition = (BuildDefinition) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          definition = component.GetDeletedDefinition(projectId, definitionId);
        if (definition != null)
        {
          this.SecurityProvider.CheckDefinitionPermission(requestContext, projectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition, true);
          if (!authorId.HasValue || authorId.Value == Guid.Empty)
            authorId = new Guid?(requestContext.GetUserId(true));
          using (Build2Component component = requestContext.CreateComponent<Build2Component>())
            definition = component.RestoreDefinition(projectId, definitionId, authorId.Value, comment);
          if (definition != null)
            return this.PostProcessDefinitions(requestContext, (IList<BuildDefinition>) new BuildDefinition[1]
            {
              definition
            }).Single<BuildDefinition>();
        }
        return (BuildDefinition) null;
      }
    }

    public BuildDefinition GetDeletedDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDeletedDefinition)))
      {
        BuildDefinition buildDefinition = (BuildDefinition) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          buildDefinition = component.GetDeletedDefinition(projectId, definitionId);
        if (buildDefinition == null)
          return (BuildDefinition) null;
        return this.PostProcessDefinitions(requestContext, (IList<BuildDefinition>) new BuildDefinition[1]
        {
          buildDefinition
        }).Single<BuildDefinition>();
      }
    }

    public async Task<IEnumerable<BuildDefinition>> GetDeletedDefinitionsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      DefinitionQueryOrder queryOrder = DefinitionQueryOrder.None,
      int count = 10000)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      IEnumerable<BuildDefinition> definitionsAsync1;
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDeletedDefinitionsAsync)))
      {
        IList<BuildDefinition> definitionsAsync2;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          definitionsAsync2 = (IList<BuildDefinition>) await bc.GetDeletedDefinitionsAsync(projectId, queryOrder, count);
        definitionsAsync1 = (IEnumerable<BuildDefinition>) this.PostProcessDefinitions(requestContext, definitionsAsync2);
      }
      return definitionsAsync1;
    }

    public List<BuildDefinition> GetDefinitionsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> definitionIds,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false,
      bool includeDrafts = false,
      ExcludePopulatingDefinitionResources excludePopulatingResources = null)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDefinitionsByIds)))
      {
        List<int> list = definitionIds.Distinct<int>().ToList<int>();
        if (list.Count == 0)
          return new List<BuildDefinition>();
        IList<BuildDefinition> definitions = (IList<BuildDefinition>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          definitions = (IList<BuildDefinition>) component.GetDefinitionsByIds(projectId, list, includeDeleted, minMetricsTime, includeLatestBuilds, includeDrafts);
        return this.PostProcessDefinitions(requestContext, definitions, excludePopulatingResources).ToList<BuildDefinition>();
      }
    }

    public async Task<List<BuildDefinition>> GetDefinitionsByIdsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> definitionIds,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false,
      bool includeDrafts = false)
    {
      requestContext.AssertAsyncExecutionEnabled();
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDefinitionsByIdsAsync)))
      {
        if (definitionIds.Count == 0)
          return new List<BuildDefinition>();
        definitionIds = definitionIds.Distinct<int>().ToList<int>();
        IList<BuildDefinition> definitions = (IList<BuildDefinition>) null;
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          definitions = (IList<BuildDefinition>) await bc.GetDefinitionsByIdsAsync(projectId, definitionIds, includeDeleted, minMetricsTime, includeLatestBuilds, includeDrafts);
        return this.PostProcessDefinitions(requestContext, definitions).ToList<BuildDefinition>();
      }
    }

    public IEnumerable<string> AddTags(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition), "Build2");
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tags, nameof (tags), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (AddTags)))
      {
        this.SecurityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition);
        List<string> collection = (List<string>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          collection = component.AddDefinitionTags(definition.ProjectId, definition.Id, definition.Revision.Value, tags).ToList<string>();
        definition.Tags.Clear();
        definition.Tags.AddRange((IEnumerable<string>) collection);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo(12030010, "Service", "Publishing BuildDefinitionChangedEvent (added tags) for definition {0}", (object) definition.Id);
        service.PublishNotification(requestContext, (object) new BuildDefinitionChangedEvent(AuditAction.Update, definition));
        return (IEnumerable<string>) collection;
      }
    }

    public IEnumerable<string> DeleteTags(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IEnumerable<string> tags)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition), "Build2");
      ArgumentUtility.CheckForNull<IEnumerable<string>>(tags, nameof (tags), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (DeleteTags)))
      {
        this.SecurityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition);
        List<string> collection = (List<string>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          collection = component.DeleteDefinitionTags(definition.ProjectId, definition.Id, definition.Revision.Value, tags).ToList<string>();
        definition.Tags.Clear();
        definition.Tags.AddRange((IEnumerable<string>) collection);
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo(12030077, "Service", "Publishing BuildDefinitionChangedEvent (delete tags) for definition {0}", (object) definition.Id);
        service.PublishNotification(requestContext, (object) new BuildDefinitionChangedEvent(AuditAction.Update, definition));
        return (IEnumerable<string>) collection;
      }
    }

    public IEnumerable<BuildMetric> GetMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      DateTime? minMetricsTime)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      if (this.GetDefinition(requestContext, projectId, definitionId) == null)
        throw new Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definitionId));
      DateTime minMetricsTime1 = minMetricsTime ?? DateTime.UtcNow.AddDays(-7.0);
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetMetrics)))
      {
        List<BuildDefinitionMetric> list;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          list = component.GetDefinitionMetrics(projectId, (IEnumerable<int>) new int[1]
          {
            definitionId
          }, minMetricsTime1).ToList<BuildDefinitionMetric>();
        return list.Select<BuildDefinitionMetric, BuildMetric>((Func<BuildDefinitionMetric, BuildMetric>) (data => data.Metric));
      }
    }

    public IEnumerable<BuildMetric> UpdateMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      IEnumerable<BuildMetric> metrics)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (UpdateMetrics)))
      {
        this.SecurityProvider.CheckDefinitionPermission(requestContext, projectId, (MinimalBuildDefinition) (this.GetDefinition(requestContext, projectId, definitionId) ?? throw new Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definitionId))), Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition);
        List<BuildDefinitionMetric> list;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          list = component.UpdateDefinitionMetrics(projectId, metrics.Select<BuildMetric, BuildDefinitionMetric>((Func<BuildMetric, BuildDefinitionMetric>) (x => new BuildDefinitionMetric(x, definitionId)))).ToList<BuildDefinitionMetric>();
        return list.Select<BuildDefinitionMetric, BuildMetric>((Func<BuildDefinitionMetric, BuildMetric>) (x => x.Metric));
      }
    }

    public void ReadSecretVariables(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IDictionary<string, string> targetSecretVariables,
      IDictionary<string, string> targetRepositorySecrets,
      bool omitVariableGroups = false)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForEmptyGuid(definition.ProjectId, "definition.ProjectId");
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (ReadSecretVariables)))
      {
        if (definition.Id <= 0)
          return;
        this.SecurityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuildDefinition);
        if (!omitVariableGroups)
          this.MergeVariableGroupSecretVariables(requestContext, definition, targetSecretVariables);
        IEnumerable<string> source = definition.Variables.Where<KeyValuePair<string, BuildDefinitionVariable>>((Func<KeyValuePair<string, BuildDefinitionVariable>, bool>) (pair => pair.Value.IsSecret)).Select<KeyValuePair<string, BuildDefinitionVariable>, string>((Func<KeyValuePair<string, BuildDefinitionVariable>, string>) (pair => pair.Key));
        IEnumerable<string> secretPropertyNames = this.GetRepositorySecretPropertyNames(requestContext, definition);
        if ((!source.Any<string>() || targetSecretVariables == null) && (!secretPropertyNames.Any<string>() || targetRepositorySecrets == null))
          return;
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(requestContext.Elevate(), DefinitionSecrets.GetDefinitionDrawerName(definition), false);
        if (!(drawerId != Guid.Empty))
          return;
        if (targetSecretVariables != null)
        {
          foreach (string str1 in source)
          {
            string str2;
            if (!targetSecretVariables.TryGetValue(str1, out str2) || str2 == null)
            {
              string secretVariableKey = DefinitionSecrets.GetDefinitionSecretVariableKey(definition, str1);
              string str3;
              if (service.TryGetStrongBoxValue(requestContext.Elevate(), drawerId, secretVariableKey, out str3))
                targetSecretVariables[str1] = str3;
            }
          }
        }
        if (targetRepositorySecrets == null)
          return;
        foreach (string str4 in secretPropertyNames)
        {
          string str5 = (string) null;
          if (!targetRepositorySecrets.TryGetValue(str4, out str5) || str5 == null)
          {
            string repositorySecretKey = DefinitionSecrets.GetDefinitionRepositorySecretKey(definition, str4);
            string str6;
            if (service.TryGetStrongBoxValue(requestContext.Elevate(), drawerId, repositorySecretKey, out str6))
              targetRepositorySecrets[str4] = str6;
          }
        }
      }
    }

    public BuildRepository GetExpandedRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IDictionary<string, string> variables = null)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetExpandedRepository)))
      {
        BuildRepository expandedRepository = definition.Repository;
        if (definition.Id > 0 && expandedRepository != null)
        {
          if (variables == null)
            variables = (IDictionary<string, string>) definition.Variables.Where<KeyValuePair<string, BuildDefinitionVariable>>((Func<KeyValuePair<string, BuildDefinitionVariable>, bool>) (v => !v.Value.IsSecret || v.Value.Value != null)).ToDictionary<KeyValuePair<string, BuildDefinitionVariable>, string, string>((Func<KeyValuePair<string, BuildDefinitionVariable>, string>) (v => v.Key), (Func<KeyValuePair<string, BuildDefinitionVariable>, string>) (v => v.Value.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          expandedRepository = expandedRepository.Clone();
          IDictionary<string, string> properties = expandedRepository.Properties;
          this.ReadSecretVariables(requestContext, definition, variables, properties, false);
          expandedRepository.Clean = BuildCommonUtil.ExpandEnvironmentVariables(expandedRepository.Clean, variables);
          foreach (string key in expandedRepository.Properties.Keys.ToList<string>())
            expandedRepository.Properties[key] = BuildCommonUtil.ExpandEnvironmentVariables(expandedRepository.Properties[key], variables);
        }
        return expandedRepository;
      }
    }

    public PropertiesCollection UpdateProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      PropertiesCollection properties)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (UpdateProperties)))
      {
        BuildDefinition definition = this.GetDefinition(requestContext, projectId, definitionId, includeDeleted: true);
        if (definition == null)
          throw new Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definitionId));
        this.SecurityProvider.CheckDefinitionPermission(requestContext, projectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition);
        ArtifactSpec artifactSpec = definition.CreateArtifactSpec(requestContext);
        return requestContext.GetService<ITeamFoundationPropertyService>().UpdateProperties(requestContext, artifactSpec, properties);
      }
    }

    public PipelineBuildResult BuildPipeline(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      PipelineResources authorizedResources = null,
      bool authorizeNewResources = false,
      BuildOptions options = null)
    {
      PipelineBuilder pipelineBuilder = definition.GetPipelineBuilder(requestContext, authorizedResources, authorizeNewResources);
      PipelineBuildContext buildContext = pipelineBuilder.CreateBuildContext(options);
      return pipelineBuilder.Build(definition.GetProcess<DesignerProcess>().ToPipelineProcess(requestContext, definition, (IPipelineContext) buildContext).Stages, options);
    }

    public BuildDefinition GetRenamedDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      string name,
      string path)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetRenamedDefinition)))
      {
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        {
          BuildDefinition renamedDefinition = component.GetRenamedDefinition(projectId, name, path);
          return renamedDefinition == null || !this.SecurityProvider.HasDefinitionPermission(requestContext, renamedDefinition.ProjectId, (MinimalBuildDefinition) renamedDefinition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuilds) ? (BuildDefinition) null : renamedDefinition;
        }
      }
    }

    public IEnumerable<BuildDefinitionBranch> UpdateDefinitionBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildDefinition definition,
      IEnumerable<BuildDefinitionBranch> branches,
      int maxConcurrentBuildsPerBranch,
      bool ignoreSourceIdCheck)
    {
      if (!requestContext.IsFeatureEnabled("Build2.MaxConcurrentBuildsPerBranch"))
        maxConcurrentBuildsPerBranch = 1;
      return this.UpdateDefinitionBranches(requestContext, projectId, definition.Id, definition.Revision.Value, branches, maxConcurrentBuildsPerBranch, ignoreSourceIdCheck);
    }

    public IEnumerable<BuildDefinitionBranch> GetBuildableDefinitionBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int maxConcurrentBuildsPerBranch)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetBuildableDefinitionBranches)))
      {
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return (IEnumerable<BuildDefinitionBranch>) component.GetBuildableDefinitionBranches(projectId, definitionId, maxConcurrentBuildsPerBranch);
      }
    }

    internal IEnumerable<BuildDefinitionBranch> UpdateDefinitionBranches(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int definitionVersion,
      IEnumerable<BuildDefinitionBranch> branches,
      int maxConcurrentBuildsPerBranch,
      bool ignoreSourceIdCheck)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (UpdateDefinitionBranches)))
      {
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return component.UpdateDefinitionBranches(projectId, definitionId, definitionVersion, branches, maxConcurrentBuildsPerBranch, ignoreSourceIdCheck);
      }
    }

    private static bool TryGetCachedBuildDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int? definitionVersion,
      out BuildDefinition definition)
    {
      ConcurrentDictionary<Tuple<Guid, int, int?>, BuildDefinition> concurrentDictionary = (ConcurrentDictionary<Tuple<Guid, int, int?>, BuildDefinition>) null;
      if (requestContext.IsFeatureEnabled("Build2.Service.CacheBuildDefinitions"))
      {
        if (requestContext.Items.TryGetValue<ConcurrentDictionary<Tuple<Guid, int, int?>, BuildDefinition>>("$BuildDefinitionCache", out concurrentDictionary))
        {
          Tuple<Guid, int, int?> key = Tuple.Create<Guid, int, int?>(projectId, definitionId, definitionVersion);
          return concurrentDictionary.TryGetValue(key, out definition);
        }
        requestContext.Items["$BuildDefinitionCache"] = (object) new ConcurrentDictionary<Tuple<Guid, int, int?>, BuildDefinition>();
      }
      definition = (BuildDefinition) null;
      return false;
    }

    private static void UpdateDefinitionCache(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int? definitionVersion,
      ref BuildDefinition definition)
    {
      if (!requestContext.IsFeatureEnabled("Build2.Service.CacheBuildDefinitions"))
        return;
      Tuple<Guid, int, int?> key1 = Tuple.Create<Guid, int, int?>(projectId, definitionId, definitionVersion);
      ConcurrentDictionary<Tuple<Guid, int, int?>, BuildDefinition> concurrentDictionary = requestContext.Items["$BuildDefinitionCache"] as ConcurrentDictionary<Tuple<Guid, int, int?>, BuildDefinition>;
      if (definitionVersion.HasValue)
      {
        Tuple<Guid, int, int?> key2 = Tuple.Create<Guid, int, int?>(projectId, definitionId, new int?());
        BuildDefinition buildDefinition;
        if (concurrentDictionary.TryGetValue(key2, out buildDefinition))
        {
          int? revision1 = definition.Revision;
          int? revision2 = buildDefinition.Revision;
          if (revision1.GetValueOrDefault() > revision2.GetValueOrDefault() & revision1.HasValue & revision2.HasValue)
          {
            concurrentDictionary[key2] = definition;
            requestContext.TraceInfo(nameof (BuildDefinitionService), "The build definition from project {0} with definition id {1} has a cached version with revision {2} as the latest and a new version with a higher revision {3} has been requested on the same request context.", (object) projectId, (object) definitionId, (object) buildDefinition.Revision, (object) definition.Revision);
          }
        }
      }
      concurrentDictionary[key1] = definition;
      if (!definitionVersion.HasValue)
        concurrentDictionary[Tuple.Create<Guid, int, int?>(projectId, definitionId, definition.Revision)] = definition;
      definition = definition.Clone();
    }

    private void DeleteDefinitionSecrets(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (DeleteDefinitionSecrets)))
      {
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(requestContext, DefinitionSecrets.GetDefinitionDrawerName(definition), false);
        if (!(drawerId != Guid.Empty))
          return;
        service.DeleteDrawer(requestContext, drawerId);
      }
    }

    private void ValidateSecretSourceDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildDefinition secretSource)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (ValidateSecretSourceDefinition)))
        this.SecurityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) (this.GetDefinition(requestContext, definition.ProjectId, secretSource.Id, secretSource.Revision) ?? throw new Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) secretSource.Id))), Microsoft.TeamFoundation.Build.Common.BuildPermissions.EditBuildDefinition);
    }

    private void ReadMissingSecrets(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildDefinition secretSource)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (ReadMissingSecrets)))
      {
        IEnumerable<KeyValuePair<string, BuildDefinitionVariable>> source1 = definition.Variables.Where<KeyValuePair<string, BuildDefinitionVariable>>((Func<KeyValuePair<string, BuildDefinitionVariable>, bool>) (v => v.Value.IsSecret && v.Value.Value == null));
        IEnumerable<string> source2 = this.GetRepositorySecretPropertyNames(requestContext, definition).Where<string>((Func<string, bool>) (key => !definition.Repository.Properties.ContainsKey(key)));
        if (!source1.Any<KeyValuePair<string, BuildDefinitionVariable>>() && !source2.Any<string>())
          return;
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(requestContext, DefinitionSecrets.GetDefinitionDrawerName(secretSource), false);
        if (!(drawerId != Guid.Empty))
          return;
        foreach (KeyValuePair<string, BuildDefinitionVariable> keyValuePair in source1)
        {
          string secretVariableKey = DefinitionSecrets.GetDefinitionSecretVariableKey(secretSource, keyValuePair.Key);
          string str;
          service.TryGetStrongBoxValue(requestContext, drawerId, secretVariableKey, out str);
          keyValuePair.Value.Value = str ?? string.Empty;
        }
        foreach (string str1 in source2)
        {
          string repositorySecretKey = DefinitionSecrets.GetDefinitionRepositorySecretKey(secretSource, str1);
          string str2;
          service.TryGetStrongBoxValue(requestContext, drawerId, repositorySecretKey, out str2);
          definition.Repository.Properties[str1] = str2 ?? string.Empty;
        }
      }
    }

    public List<BuildDefinition> GetRecentlyBuiltDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      int top,
      bool includeQueuedBuilds = false)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetRecentlyBuiltDefinitions)))
      {
        IList<BuildDefinition> definitions = (IList<BuildDefinition>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          definitions = (IList<BuildDefinition>) component.GetRecentlyBuiltDefinitions(projectId, top, includeQueuedBuilds);
        return this.PostProcessDefinitions(requestContext, definitions, new ExcludePopulatingDefinitionResources()
        {
          References = true,
          Endpoints = true,
          VariableGroups = true
        }).ToList<BuildDefinition>();
      }
    }

    public List<BuildDefinition> GetMyRecentlyBuiltDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      DateTime minFinishTime,
      int top,
      IEnumerable<int> excludeDefinitionIds)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetMyRecentlyBuiltDefinitions)))
      {
        IList<BuildDefinition> definitions = (IList<BuildDefinition>) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          definitions = (IList<BuildDefinition>) component.GetMyRecentlyBuiltDefinitions(projectId, requestContext.GetUserId(), minFinishTime, excludeDefinitionIds, top);
        return this.PostProcessDefinitions(requestContext, definitions, new ExcludePopulatingDefinitionResources()
        {
          References = true,
          Endpoints = true,
          VariableGroups = true
        }).ToList<BuildDefinition>();
      }
    }

    public async Task<IList<RepositoryBranchReferences>> GetBranchesByName(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount,
      string nameSubstring,
      int? definitionId,
      HashSet<int> excludedBranchIds)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetBranchesByName)))
      {
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        {
          int? nullable = definitionId;
          int num = 0;
          return nullable.GetValueOrDefault() > num & nullable.HasValue ? await bc.GetBranchesByNameForDefinition(projectId, maxCount, definitionId.Value, "%" + nameSubstring + "%", excludedBranchIds) : await bc.GetBranchesByName(projectId, maxCount, "%" + nameSubstring + "%", excludedBranchIds);
        }
      }
    }

    public async Task<IList<RepositoryBranchReferences>> GetBranchesById(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> branchIds)
    {
      IList<RepositoryBranchReferences> branchesByIdAsync;
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetBranchesById)))
      {
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          branchesByIdAsync = await bc.GetBranchesByIdAsync(projectId, branchIds);
      }
      return branchesByIdAsync;
    }

    public async Task<IList<RepositoryBranchReferences>> GetRecentlyBuiltRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      int? definitionId,
      int topRepositories,
      int topBranches,
      HashSet<int> excludedRepositoryIds,
      HashSet<int> excludedDefinitionIds)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetRecentlyBuiltRepositories)))
      {
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        {
          int? nullable = definitionId;
          int num = 0;
          if (!(nullable.GetValueOrDefault() > num & nullable.HasValue))
            return await bc.GetRecentlyBuiltRepositories(projectId, topRepositories, topBranches, excludedRepositoryIds, excludedDefinitionIds);
          return excludedDefinitionIds != null && excludedDefinitionIds.Contains(definitionId.Value) ? (IList<RepositoryBranchReferences>) Array.Empty<RepositoryBranchReferences>() : await bc.GetBranchesByNameForDefinition(projectId, topBranches, definitionId.Value, "%", new HashSet<int>());
        }
      }
    }

    public async Task<IList<RepositoryBranchReferences>> GetRecentlyBuiltBranchesForRepositories(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxBranches,
      IEnumerable<string> repositoryIdentifiers,
      HashSet<int> excludedRepositoryIds)
    {
      IList<RepositoryBranchReferences> branchesForRepositories;
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetRecentlyBuiltBranchesForRepositories)))
      {
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          branchesForRepositories = await bc.GetRecentlyBuiltBranchesForRepositories(projectId, maxBranches, repositoryIdentifiers, excludedRepositoryIds);
      }
      return branchesForRepositories;
    }

    public async Task<IList<Guid>> GetRecentlyBuiltRequestedForIdentities(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount,
      HashSet<Guid> excludedIds,
      HashSet<int> excludedDefinitionIds)
    {
      IList<Guid> requestedForIdentities;
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetRecentlyBuiltRequestedForIdentities)))
      {
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          requestedForIdentities = await bc.GetRecentlyBuiltRequestedForIdentities(projectId, maxCount, excludedIds, excludedDefinitionIds);
      }
      return requestedForIdentities;
    }

    public async Task<IList<BuildSchedule>> GetSchedulesByDefinitionId(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      IList<BuildSchedule> schedulesByDefinitionId;
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetSchedulesByDefinitionId)))
      {
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          schedulesByDefinitionId = await bc.GetSchedulesByDefinitionId(projectId, definitionId);
      }
      return schedulesByDefinitionId;
    }

    private static bool IsDefinitionJustBeingDisabled(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildDefinition originalDefinition)
    {
      return definition.QueueStatus == DefinitionQueueStatus.Disabled && ObjectComparerHelper.AreSameObjects((object) definition, (object) originalDefinition, (ICollection<string>) BuildDefinitionService.fieldsToIgnoreCollection, requestContext);
    }

    private BuildDefinition GetCurrentDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      return this.GetDefinition(requestContext, definition.ProjectId, definition.Id, definition.Revision) ?? throw new Microsoft.TeamFoundation.Build.WebApi.DefinitionNotFoundException(BuildServerResources.DefinitionNotFound((object) definition.Id));
    }

    public IList<Guid> GetAllServiceConnectionsForRepoAndProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryIdentifier,
      string repoType,
      int triggerType)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetAllServiceConnectionsForRepoAndProject)))
      {
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return component.GetAllServiceConnectionsForRepoAndProject(projectId, repositoryIdentifier, repoType, triggerType);
      }
    }

    private IEnumerable<string> GetRepositorySecretPropertyNames(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetRepositorySecretPropertyNames)))
      {
        IEnumerable<string> secretPropertyNames = Enumerable.Empty<string>();
        if (definition.Repository != null)
        {
          IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, definition.Repository.Type, false);
          if (sourceProvider != null)
            secretPropertyNames = sourceProvider.GetRepositorySecretPropertyNames();
        }
        return secretPropertyNames;
      }
    }

    private void MergeVariableGroupSecretVariables(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      IDictionary<string, string> targetSecretVariables)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (MergeVariableGroupSecretVariables)))
      {
        if (targetSecretVariables == null)
          return;
        definition.PopulateVariableGroups(requestContext.Elevate());
        foreach (VariableGroup variableGroup in definition.VariableGroups)
        {
          foreach (KeyValuePair<string, BuildDefinitionVariable> keyValuePair in variableGroup.Variables.Where<KeyValuePair<string, BuildDefinitionVariable>>((Func<KeyValuePair<string, BuildDefinitionVariable>, bool>) (x => x.Value != null && x.Value.IsSecret)))
          {
            string str;
            if (!targetSecretVariables.TryGetValue(keyValuePair.Key, out str) || str == null)
              targetSecretVariables[keyValuePair.Key] = keyValuePair.Value.Value;
          }
        }
      }
    }

    private DefinitionSecrets ExtractSecrets(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (ExtractSecrets)))
      {
        DefinitionSecrets secrets = new DefinitionSecrets();
        foreach (KeyValuePair<string, BuildDefinitionVariable> keyValuePair in definition.Variables.Where<KeyValuePair<string, BuildDefinitionVariable>>((Func<KeyValuePair<string, BuildDefinitionVariable>, bool>) (v => v.Value.IsSecret)))
        {
          BuildDefinitionVariable definitionVariable = keyValuePair.Value.Clone();
          keyValuePair.Value.Value = (string) null;
          secrets.SecretVariables.Add(keyValuePair.Key, definitionVariable);
        }
        foreach (string secretPropertyName in this.GetRepositorySecretPropertyNames(requestContext, definition))
        {
          string str;
          if (definition.Repository.Properties.TryGetValue(secretPropertyName, out str))
            definition.Repository.Properties.Remove(secretPropertyName);
          secrets.RepositorySecrets.Add(secretPropertyName, str);
        }
        return secrets;
      }
    }

    private void StoreSecrets(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      DefinitionSecrets secrets)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (StoreSecrets)))
      {
        IEnumerable<KeyValuePair<string, BuildDefinitionVariable>> source1 = secrets.SecretVariables.Where<KeyValuePair<string, BuildDefinitionVariable>>((Func<KeyValuePair<string, BuildDefinitionVariable>, bool>) (p => p.Value != null && p.Value.Value != null));
        IEnumerable<KeyValuePair<string, string>> source2 = secrets.RepositorySecrets.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => p.Value != null));
        if (!source1.Any<KeyValuePair<string, BuildDefinitionVariable>>() && !source2.Any<KeyValuePair<string, string>>())
          return;
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawer = service.GetOrCreateDrawer(requestContext, DefinitionSecrets.GetDefinitionDrawerName(definition));
        foreach (KeyValuePair<string, BuildDefinitionVariable> keyValuePair in source1)
        {
          string secretVariableKey = DefinitionSecrets.GetDefinitionSecretVariableKey(definition, keyValuePair.Key);
          service.AddString(requestContext, drawer, secretVariableKey, keyValuePair.Value.Value);
        }
        foreach (KeyValuePair<string, string> keyValuePair in source2)
        {
          string repositorySecretKey = DefinitionSecrets.GetDefinitionRepositorySecretKey(definition, keyValuePair.Key);
          service.AddString(requestContext, drawer, repositorySecretKey, keyValuePair.Value);
        }
      }
    }

    private void UpdateScheduleTriggerInfo(BuildDefinition definition)
    {
      foreach (BuildTrigger buildTrigger in definition.Triggers.Where<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.Schedule)).ToList<BuildTrigger>())
      {
        foreach (Schedule schedule in (buildTrigger as ScheduleTrigger).Schedules)
          schedule.ScheduleJobId = Guid.NewGuid();
      }
    }

    private void AddPollingTriggerInfo(BuildDefinition definition)
    {
      ContinuousIntegrationTrigger trigger = definition.Triggers.OfType<ContinuousIntegrationTrigger>().FirstOrDefault<ContinuousIntegrationTrigger>();
      if (!trigger.IsPollingEnabled())
        return;
      trigger.PollingJobId = Guid.NewGuid();
    }

    internal void AddScheduleTriggerJobs(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (AddScheduleTriggerJobs)))
      {
        ITeamFoundationJobService service = requestContext.Elevate().GetService<ITeamFoundationJobService>();
        List<TeamFoundationJobDefinition> scheduleJobDefinitions = definition.GetScheduleJobDefinitions(requestContext, service.IsIgnoreDormancyPermitted);
        if (scheduleJobDefinitions.Count <= 0)
          return;
        service.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) scheduleJobDefinitions);
      }
    }

    private void DeletePollingTriggerJob(
      IVssRequestContext requestContext,
      List<BuildDefinition> deletedDefinitions)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (DeletePollingTriggerJob)))
      {
        List<Guid> jobsToDelete = new List<Guid>();
        foreach (BuildDefinition deletedDefinition in deletedDefinitions)
        {
          ContinuousIntegrationTrigger integrationTrigger = deletedDefinition.Triggers.OfType<ContinuousIntegrationTrigger>().Where<ContinuousIntegrationTrigger>((Func<ContinuousIntegrationTrigger, bool>) (x => x.IsPollingEnabled())).FirstOrDefault<ContinuousIntegrationTrigger>();
          if (integrationTrigger != null)
            jobsToDelete.Add(integrationTrigger.PollingJobId);
        }
        if (jobsToDelete.Count <= 0)
          return;
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        vssRequestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(vssRequestContext, (IEnumerable<Guid>) jobsToDelete, (IEnumerable<TeamFoundationJobDefinition>) null);
      }
    }

    private void DeleteScheduleTriggerJobs(
      IVssRequestContext requestContext,
      List<BuildDefinition> deletedDefinitions)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (DeleteScheduleTriggerJobs)))
      {
        List<Guid> list = deletedDefinitions.SelectMany<BuildDefinition, Guid>((Func<BuildDefinition, IEnumerable<Guid>>) (definition => definition.GetScheduleTriggerJobIds())).ToList<Guid>();
        if (list.Count <= 0)
          return;
        requestContext.Elevate().GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) list, (IEnumerable<TeamFoundationJobDefinition>) null);
      }
    }

    private void DeleteCronSchedulesForDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      List<int> definitionIds)
    {
      List<CronSchedule> source;
      using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
        source = requestContext.RunSynchronously<List<CronSchedule>>((Func<Task<List<CronSchedule>>>) (() => bc.DeleteCronSchedulesForDefinitions(projectId, definitionIds)));
      // ISSUE: explicit non-virtual call
      if (source == null || __nonvirtual (source.Count) <= 0)
        return;
      List<Guid> list = source.Select<CronSchedule, Guid>((Func<CronSchedule, Guid>) (s => s.ScheduleJobId)).ToList<Guid>();
      requestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) list, (IEnumerable<TeamFoundationJobDefinition>) null);
      requestContext.TraceAlways(12030006, TraceLevel.Info, "Build2", "Service", string.Format("Removing {0} cron syntax schedules for total {1} definitions with Ids {2}.", (object) source?.Count, (object) definitionIds?.Count, (object) string.Join<int>(",", (IEnumerable<int>) definitionIds)));
    }

    private async Task<IEnumerable<BuildDefinition>> GetDefinitionsWithSchedulesAsync(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      IEnumerable<BuildDefinition> withSchedulesAsync;
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDefinitionsWithSchedulesAsync)))
      {
        using (Build2Component bc = requestContext.CreateComponent<Build2Component>())
          withSchedulesAsync = (IEnumerable<BuildDefinition>) await bc.GetDefinitionsWithSchedulesAsync(projectId);
      }
      return withSchedulesAsync;
    }

    private void OverrideDefaultCount(IVssRequestContext requestContext, ref int count)
    {
      if (count != 10000)
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      count = service.GetValue<int>(requestContext, in BuildDefinitionService.s_defaultCountQuery, 10000);
    }

    private void UpdatePollingTriggerJob(
      IVssRequestContext requestContext,
      BuildDefinition originalDefinition,
      BuildDefinition updatedDefinition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (UpdatePollingTriggerJob)))
      {
        ContinuousIntegrationTrigger trigger1 = originalDefinition.Triggers.OfType<ContinuousIntegrationTrigger>().FirstOrDefault<ContinuousIntegrationTrigger>();
        ContinuousIntegrationTrigger trigger2 = updatedDefinition.Triggers.OfType<ContinuousIntegrationTrigger>().FirstOrDefault<ContinuousIntegrationTrigger>();
        if (!trigger1.IsPollingEnabled() && !trigger2.IsPollingEnabled())
          return;
        List<Guid> jobsToDelete = new List<Guid>();
        List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>();
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ITeamFoundationJobService service = vssRequestContext.GetService<ITeamFoundationJobService>();
        if (!trigger1.IsPollingEnabled() && trigger2.IsPollingEnabled())
        {
          TeamFoundationJobDefinition pollingJobDefinition = updatedDefinition.GetPollingJobDefinition(service.IsIgnoreDormancyPermitted);
          if (pollingJobDefinition != null)
            jobUpdates.Add(pollingJobDefinition);
          else
            requestContext.TraceWarning(12030296, nameof (BuildDefinitionService), "GetPollingJobDefinition() returned null for definition with ProjectId {0}, DefinitionId {1} (site 1)", (object) updatedDefinition.ProjectId, (object) updatedDefinition.Id);
        }
        else if (trigger1.IsPollingEnabled() && !trigger2.IsPollingEnabled())
          jobsToDelete.Add(trigger1.PollingJobId);
        else if (trigger1.IsPollingEnabled() && trigger2.IsPollingEnabled())
        {
          int? pollingInterval1 = trigger1.PollingInterval;
          int? pollingInterval2 = trigger2.PollingInterval;
          if (!(pollingInterval1.GetValueOrDefault() == pollingInterval2.GetValueOrDefault() & pollingInterval1.HasValue == pollingInterval2.HasValue))
          {
            string lastSourceVersion = string.Empty;
            string currentConnectionId = string.Empty;
            string lastFailedBuildDate = string.Empty;
            trigger2.GetLatestValuesFromPollingJob(requestContext, out lastSourceVersion, out currentConnectionId, out lastFailedBuildDate);
            TeamFoundationJobDefinition pollingJobDefinition = updatedDefinition.GetPollingJobDefinition(service.IsIgnoreDormancyPermitted, lastSourceVersion, currentConnectionId, lastFailedBuildDate);
            if (pollingJobDefinition != null)
              jobUpdates.Add(pollingJobDefinition);
            else
              requestContext.TraceWarning(12030296, nameof (BuildDefinitionService), "GetPollingJobDefinition() returned null for definition with ProjectId {0}, DefinitionId {1} (site 2)", (object) updatedDefinition.ProjectId, (object) updatedDefinition.Id);
          }
          else
          {
            IBuildSourceProvider buildSourceProvider = (IBuildSourceProvider) null;
            if (updatedDefinition.Repository != null)
              buildSourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, updatedDefinition.Repository.Type, false);
            if (buildSourceProvider == null)
            {
              requestContext.TraceWarning(0, "Service", "Build definition {0} has no source provider so not updating any polling job information.", (object) updatedDefinition.Id);
              return;
            }
            if (buildSourceProvider is IBuildPollingSourceProvider pollingSourceProvider && pollingSourceProvider.HasTriggerBeenInvalidated(requestContext, originalDefinition, updatedDefinition))
            {
              jobsToDelete.Add(trigger1.PollingJobId);
              TeamFoundationJobDefinition pollingJobDefinition = updatedDefinition.GetPollingJobDefinition(service.IsIgnoreDormancyPermitted);
              if (pollingJobDefinition != null)
                jobUpdates.Add(pollingJobDefinition);
              else
                requestContext.TraceWarning(12030296, nameof (BuildDefinitionService), "GetPollingJobDefinition() returned null for definition with ProjectId {0}, DefinitionId {1} (site 3)", (object) updatedDefinition.ProjectId, (object) updatedDefinition.Id);
            }
          }
        }
        if (jobsToDelete.Count != 1 && jobUpdates.Count != 1)
          return;
        service.UpdateJobDefinitions(vssRequestContext, (IEnumerable<Guid>) jobsToDelete, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      }
    }

    private void UpdatePollingTriggerInfo(
      IVssRequestContext requestContext,
      BuildDefinition incomingDefinition,
      BuildDefinition originalDefinition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (UpdatePollingTriggerInfo)))
      {
        ContinuousIntegrationTrigger trigger1 = incomingDefinition.Triggers.OfType<ContinuousIntegrationTrigger>().FirstOrDefault<ContinuousIntegrationTrigger>();
        ContinuousIntegrationTrigger trigger2 = originalDefinition.Triggers.OfType<ContinuousIntegrationTrigger>().FirstOrDefault<ContinuousIntegrationTrigger>();
        if (trigger1.IsPollingEnabled())
        {
          if (!trigger2.IsPollingEnabled())
          {
            trigger1.PollingJobId = Guid.NewGuid();
          }
          else
          {
            IBuildSourceProvider buildSourceProvider = (IBuildSourceProvider) null;
            if (incomingDefinition.Repository != null)
              buildSourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, incomingDefinition.Repository.Type, false);
            if (buildSourceProvider == null)
            {
              requestContext.TraceWarning(0, "Service", "Build definition {0} has no source provider so not updating any polling information.", (object) incomingDefinition.Id);
            }
            else
            {
              if (!(buildSourceProvider is IBuildPollingSourceProvider pollingSourceProvider) || !pollingSourceProvider.HasTriggerBeenInvalidated(requestContext, originalDefinition, incomingDefinition))
                return;
              trigger1.PollingJobId = Guid.NewGuid();
            }
          }
        }
        else
        {
          if (trigger1 == null || !(trigger1.PollingJobId != Guid.Empty))
            return;
          trigger1.PollingJobId = Guid.Empty;
        }
      }
    }

    private void UpdateScheduleTriggerJobs(
      IVssRequestContext requestContext,
      BuildDefinition originalDefinition,
      BuildDefinition updatedDefinition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (UpdateScheduleTriggerJobs)))
      {
        List<BuildTrigger> list1 = originalDefinition.Triggers.Where<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.Schedule)).ToList<BuildTrigger>();
        List<BuildTrigger> list2 = updatedDefinition.Triggers.Where<BuildTrigger>((Func<BuildTrigger, bool>) (x => x.TriggerType == DefinitionTriggerType.Schedule)).ToList<BuildTrigger>();
        if (list1.Count <= 0 && list2.Count <= 0)
          return;
        ITeamFoundationJobService service = requestContext.Elevate().GetService<ITeamFoundationJobService>();
        List<Guid> jobsToDelete = new List<Guid>();
        List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>();
        if (list1.Count > 0)
        {
          foreach (BuildTrigger buildTrigger in list1)
          {
            foreach (Schedule schedule in (buildTrigger as ScheduleTrigger).Schedules)
              jobsToDelete.Add(schedule.ScheduleJobId);
          }
        }
        if (list2.Count > 0)
          jobUpdates = updatedDefinition.GetScheduleJobDefinitions(requestContext, service.IsIgnoreDormancyPermitted);
        if (jobsToDelete.Count <= 0 && jobUpdates.Count <= 0)
          return;
        service.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) jobsToDelete, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      }
    }

    private void AddPollingTriggerJob(IVssRequestContext requestContext, BuildDefinition definition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (AddPollingTriggerJob)))
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ITeamFoundationJobService service = vssRequestContext.GetService<ITeamFoundationJobService>();
        TeamFoundationJobDefinition pollingJobDefinition = definition.GetPollingJobDefinition(service.IsIgnoreDormancyPermitted);
        if (pollingJobDefinition == null)
          return;
        service.UpdateJobDefinitions(vssRequestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
        {
          pollingJobDefinition
        });
      }
    }

    private List<BuildDefinition> FilterDefinitionsAndUpdateContinuationData(
      IVssRequestContext requestContext,
      IList<BuildDefinition> definitions,
      out HashSet<int> excludedDefinitionIds,
      out bool dataUpdated,
      BuildDefinitionService.DefinitionContinuationData continuationData = null)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (FilterDefinitionsAndUpdateContinuationData)))
      {
        excludedDefinitionIds = new HashSet<int>();
        dataUpdated = false;
        List<BuildDefinition> buildDefinitionList = new List<BuildDefinition>();
        Dictionary<int, bool> permissionChecks = new Dictionary<int, bool>();
        foreach (BuildDefinition definition in (IEnumerable<BuildDefinition>) definitions)
        {
          bool flag;
          if (!permissionChecks.TryGetValue(definition.Id, out flag))
          {
            flag = this.SecurityProvider.HasDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuildDefinition, true);
            permissionChecks.Add(definition.Id, flag);
          }
          if (flag)
          {
            buildDefinitionList.Add(definition);
            if (definition.LatestBuild != null || definition.LatestCompletedBuild != null)
            {
              if (!this.SecurityProvider.HasDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuilds))
              {
                definition.LatestBuild = (BuildData) null;
                definition.LatestCompletedBuild = (BuildData) null;
              }
              else
              {
                if (definition.LatestBuild != null && !this.SecurityProvider.HasBuildPermission(requestContext, definition.ProjectId, (IReadOnlyBuildData) definition.LatestBuild, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuilds))
                  definition.LatestBuild = (BuildData) null;
                if (definition.LatestCompletedBuild != null && !this.SecurityProvider.HasBuildPermission(requestContext, definition.ProjectId, (IReadOnlyBuildData) definition.LatestCompletedBuild, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuilds))
                  definition.LatestCompletedBuild = (BuildData) null;
              }
            }
          }
          else
            excludedDefinitionIds.Add(definition.Id);
          DateTime? nullable;
          if (continuationData != null)
          {
            if (continuationData.QueryOrder == DefinitionQueryOrder.LastModifiedDescending)
            {
              nullable = continuationData.MaxTime;
              if (nullable.HasValue)
              {
                DateTime createdDate = definition.CreatedDate;
                ref DateTime local = ref createdDate;
                nullable = continuationData.MaxTime;
                DateTime dateTime = nullable.Value;
                if (local.CompareTo(dateTime) >= 0)
                  continue;
              }
              continuationData.MaxTime = new DateTime?(definition.CreatedDate);
              dataUpdated = true;
            }
            else if (continuationData.QueryOrder == DefinitionQueryOrder.LastModifiedAscending)
            {
              nullable = continuationData.MinTime;
              if (nullable.HasValue)
              {
                DateTime createdDate = definition.CreatedDate;
                ref DateTime local = ref createdDate;
                nullable = continuationData.MinTime;
                DateTime dateTime = nullable.Value;
                if (local.CompareTo(dateTime) <= 0)
                  continue;
              }
              continuationData.MinTime = new DateTime?(definition.CreatedDate);
              dataUpdated = true;
            }
            else if (continuationData.QueryOrder == DefinitionQueryOrder.DefinitionNameDescending)
            {
              if (string.IsNullOrEmpty(continuationData.LastDefinitionName) || string.Compare(definition.Name, continuationData.LastDefinitionName, StringComparison.InvariantCultureIgnoreCase) < 0)
              {
                continuationData.LastDefinitionName = definition.Name;
                dataUpdated = true;
              }
            }
            else if (continuationData.QueryOrder == DefinitionQueryOrder.DefinitionNameAscending && (string.IsNullOrEmpty(continuationData.LastDefinitionName) || string.Compare(definition.Name, continuationData.LastDefinitionName, StringComparison.InvariantCultureIgnoreCase) > 0))
            {
              continuationData.LastDefinitionName = definition.Name;
              dataUpdated = true;
            }
          }
        }
        foreach (BuildDefinition buildDefinition in buildDefinitionList)
        {
          if (buildDefinition.ParentDefinition != null)
          {
            bool flag;
            if (!permissionChecks.TryGetValue(buildDefinition.ParentDefinition.Id, out flag))
            {
              flag = this.SecurityProvider.HasDefinitionPermission(requestContext, buildDefinition.ProjectId, (MinimalBuildDefinition) buildDefinition.ParentDefinition, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuildDefinition, true);
              permissionChecks.Add(buildDefinition.ParentDefinition.Id, flag);
            }
            if (!flag)
              buildDefinition.ParentDefinition = (BuildDefinition) null;
          }
          buildDefinition.Drafts.RemoveAll((Predicate<BuildDefinition>) (draft =>
          {
            bool flag;
            if (!permissionChecks.TryGetValue(draft.Id, out flag))
            {
              flag = this.SecurityProvider.HasDefinitionPermission(requestContext, draft.ProjectId, (MinimalBuildDefinition) draft, Microsoft.TeamFoundation.Build.Common.BuildPermissions.ViewBuildDefinition, true);
              permissionChecks.Add(draft.Id, flag);
            }
            return !flag;
          }));
        }
        return buildDefinitionList;
      }
    }

    private void PostProcessFilteredDefinitions(
      IVssRequestContext requestContext,
      IReadOnlyList<BuildDefinition> filteredDefinitions,
      ExcludePopulatingDefinitionResources excludePopulatingResources = null)
    {
      if (filteredDefinitions.Count <= 0)
        return;
      using (IDisposableReadOnlyList<IBuildOption> extensions = requestContext.GetExtensions<IBuildOption>())
      {
        List<IBuildOption> list = extensions.OrderBy<IBuildOption, int>((Func<IBuildOption, int>) (option => option.GetOrdinal())).ToList<IBuildOption>();
        if (excludePopulatingResources == null || !excludePopulatingResources.VariableGroups)
          filteredDefinitions.PopulateVariableGroups(requestContext);
        Dictionary<(string, string), (string, Uri)> dictionary = new Dictionary<(string, string), (string, Uri)>();
        bool flag = requestContext.IsFeatureEnabled("Build2.Service.CacheRepositoryNameAndUrl");
        foreach (BuildDefinition filteredDefinition in (IEnumerable<BuildDefinition>) filteredDefinitions)
        {
          try
          {
            if (filteredDefinition.Repository != null)
            {
              (string, Uri) valueTuple1;
              if (((IEnumerable<string>) new string[3]
              {
                "GitHub",
                "GitHubEnterprise",
                "Bitbucket"
              }).Contains<string>(filteredDefinition.Repository.Type) & flag && dictionary.TryGetValue((filteredDefinition.Repository.Type, filteredDefinition.Repository.Id), out valueTuple1))
              {
                BuildRepository repository1 = filteredDefinition.Repository;
                BuildRepository repository2 = filteredDefinition.Repository;
                (string, Uri) valueTuple2 = valueTuple1;
                string str = valueTuple2.Item1;
                repository1.Name = str;
                repository2.Url = valueTuple2.Item2;
              }
              else
              {
                IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, filteredDefinition.Repository.Type, false);
                if (sourceProvider != null)
                {
                  sourceProvider.AfterDeserialize(requestContext, filteredDefinition.Repository);
                  if (!requestContext.IsFeatureEnabled("Build2.DisableBuildDefinitionRepositoryNameAndUrlLookup"))
                  {
                    try
                    {
                      sourceProvider.SetRepositoryNameAndUrl(requestContext, filteredDefinition.ProjectId, filteredDefinition.Repository);
                      if (flag)
                      {
                        if (!string.IsNullOrEmpty(filteredDefinition.Repository.Id))
                          dictionary[(filteredDefinition.Repository.Type, filteredDefinition.Repository.Id)] = (filteredDefinition.Repository.Name, filteredDefinition.Repository.Url);
                      }
                    }
                    catch (Exception ex)
                    {
                      requestContext.TraceException(nameof (BuildDefinitionService), ex);
                    }
                  }
                }
              }
            }
            if (filteredDefinition.Options != null)
            {
              filteredDefinition.Options.RemoveAll((Predicate<BuildOption>) (o => o == null || o.Definition == null));
              filteredDefinition.ModernizeOptions(requestContext, (IList<IBuildOption>) list);
            }
            this.SanitizeDefinitionWithInvalidTaskRefNames(filteredDefinition);
            this.FixPhaseRefNamesAndDependencies(requestContext, filteredDefinition.Process);
            this.ResolveDefinitionTaskVersions(requestContext, filteredDefinition);
            if (excludePopulatingResources != null)
            {
              if (excludePopulatingResources.References)
                continue;
            }
            filteredDefinition.UpdateReferences(requestContext);
          }
          catch (Exception ex)
          {
            requestContext.TraceError(0, nameof (BuildDefinitionService), "Error processing defintion {0}.  Error: {1}", (object) filteredDefinition.Id, (object) ex);
            throw;
          }
        }
      }
    }

    private List<BuildDefinition> PostProcessDefinitions(
      IVssRequestContext requestContext,
      IList<BuildDefinition> definitions,
      ExcludePopulatingDefinitionResources excludePopulatingResources = null)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (PostProcessDefinitions)))
      {
        List<BuildDefinition> filteredDefinitions = this.FilterDefinitionsAndUpdateContinuationData(requestContext, definitions, out HashSet<int> _, out bool _);
        this.PostProcessFilteredDefinitions(requestContext, (IReadOnlyList<BuildDefinition>) filteredDefinitions, excludePopulatingResources);
        return filteredDefinitions;
      }
    }

    private IEnumerable<BuildDefinition> GetDefinitionsAndProcessInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string name,
      DefinitionTriggerType triggers,
      string repositoryId,
      string repositoryType,
      DefinitionQueryOrder queryOrder,
      int count,
      DateTime? minLastModifiedTime,
      DateTime? maxLastModifiedTime,
      string lastDefinitionName,
      DateTime? minMetricsTime,
      string path,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      DefinitionQueryOptions options = DefinitionQueryOptions.All,
      IEnumerable<string> tagFilters = null,
      bool includeLatestBuilds = false,
      Guid? taskIdFilter = null,
      int? processType = null,
      ExcludePopulatingDefinitionResources exclusions = null)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (GetDefinitionsAndProcessInternal)))
      {
        BuildDefinition[] array = this.GetDefinitionsInternal(requestContext, projectId, name, triggers, repositoryId, repositoryType, queryOrder, count, minLastModifiedTime, maxLastModifiedTime, lastDefinitionName, minMetricsTime, path, builtAfter, notBuiltAfter, options, tagFilters, includeLatestBuilds, taskIdFilter, processType, exclusions).Take<BuildDefinition>(count).ToArray<BuildDefinition>();
        this.PostProcessFilteredDefinitions(requestContext, (IReadOnlyList<BuildDefinition>) array, exclusions);
        return (IEnumerable<BuildDefinition>) array;
      }
    }

    private IEnumerable<BuildDefinition> GetDefinitionsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string name,
      DefinitionTriggerType triggers,
      string repositoryId,
      string repositoryType,
      DefinitionQueryOrder queryOrder,
      int count,
      DateTime? minLastModifiedTime,
      DateTime? maxLastModifiedTime,
      string lastDefinitionName,
      DateTime? minMetricsTime,
      string path,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      DefinitionQueryOptions options = DefinitionQueryOptions.All,
      IEnumerable<string> tagFilters = null,
      bool includeLatestBuilds = false,
      Guid? taskIdFilter = null,
      int? processType = null,
      ExcludePopulatingDefinitionResources exclusions = null)
    {
      IList<BuildDefinition> definitions;
      using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        definitions = (IList<BuildDefinition>) component.GetDefinitions(projectId, name, triggers, repositoryId, repositoryType, queryOrder, count, minLastModifiedTime, maxLastModifiedTime, lastDefinitionName, minMetricsTime, path, builtAfter, notBuiltAfter, options, tagFilters, includeLatestBuilds, taskIdFilter, processType);
      BuildDefinitionService.DefinitionContinuationData continuationData = new BuildDefinitionService.DefinitionContinuationData()
      {
        LastDefinitionName = lastDefinitionName,
        MinTime = minLastModifiedTime,
        MaxTime = maxLastModifiedTime,
        QueryOrder = queryOrder
      };
      HashSet<int> collection = new HashSet<int>();
      HashSet<int> excludedDefinitionIds;
      bool dataUpdated;
      List<BuildDefinition> allDefinitions = this.FilterDefinitionsAndUpdateContinuationData(requestContext, definitions, out excludedDefinitionIds, out dataUpdated, continuationData);
      collection.AddRange<int, HashSet<int>>((IEnumerable<int>) excludedDefinitionIds);
      if (collection.Count > 0)
      {
        while (definitions.Count > 0 && allDefinitions.Count < count)
        {
          bool flag = collection.Count < excludedDefinitionIds.Count;
          if (dataUpdated | flag)
          {
            using (Build2Component component = requestContext.CreateComponent<Build2Component>())
              definitions = (IList<BuildDefinition>) component.GetDefinitions(projectId, name, triggers, repositoryId, repositoryType, queryOrder, count, continuationData.MinTime, continuationData.MaxTime, continuationData.LastDefinitionName, minMetricsTime, path, builtAfter, notBuiltAfter, options, tagFilters, includeLatestBuilds, taskIdFilter, processType);
            allDefinitions.AddRange(this.FilterDefinitionsAndUpdateContinuationData(requestContext, definitions, out excludedDefinitionIds, out dataUpdated, continuationData).Where<BuildDefinition>((Func<BuildDefinition, bool>) (d => !allDefinitions.Exists((Predicate<BuildDefinition>) (x => d.Id == x.Id)))));
            collection.AddRange<int, HashSet<int>>((IEnumerable<int>) excludedDefinitionIds);
          }
          else
            break;
        }
      }
      return (IEnumerable<BuildDefinition>) allDefinitions;
    }

    private void AddDeleteDefinitionJobs(
      IVssRequestContext requestContext,
      IEnumerable<BuildDefinition> definitions)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (AddDeleteDefinitionJobs)))
      {
        ITeamFoundationJobService jobService = requestContext.GetService<ITeamFoundationJobService>();
        int deleteDefinitionJobScheduleInDays = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "Pipelines/Retention/DeleteDefinitionJobSchedule", true, BuildServerConstants.DefaultDeleteDefinitionJobSchedule);
        jobService.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) definitions.Select<BuildDefinition, TeamFoundationJobDefinition>((Func<BuildDefinition, TeamFoundationJobDefinition>) (definition => definition.GetDeleteDefinitionJobDefinition(jobService.IsIgnoreDormancyPermitted, deleteDefinitionJobScheduleInDays))).Where<TeamFoundationJobDefinition>((Func<TeamFoundationJobDefinition, bool>) (job => job != null)).ToArray<TeamFoundationJobDefinition>());
      }
    }

    private void EvaluatePausedBuildsQueue(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      using (requestContext.TraceScope(nameof (BuildDefinitionService), nameof (EvaluatePausedBuildsQueue)))
      {
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        try
        {
          service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            JobExtensionIds.PausedBuildsQueueEvaluationJob
          }, false);
        }
        catch (JobDefinitionNotFoundException ex)
        {
          TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition()
          {
            JobId = JobExtensionIds.PausedBuildsQueueEvaluationJob,
            Name = "Build2 Paused Build Queue Evaluation Job",
            PriorityClass = JobPriorityClass.AboveNormal,
            EnabledState = TeamFoundationJobEnabledState.Enabled,
            ExtensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.PausedBuildsQueueEvaluationJob"
          };
          service.UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
          {
            foundationJobDefinition
          });
          service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            JobExtensionIds.PausedBuildsQueueEvaluationJob
          }, false);
        }
      }
    }

    private bool ShouldVerifyQueuePermissions(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      return definition.Process.Type != 2 || definition.Queue?.Pool == null || !definition.Queue.Pool.IsHosted;
    }

    private bool GetAllRepositoriesForDefinition(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      out List<BuildRepository> definitionRepositories)
    {
      if (definition.Process is YamlProcess)
      {
        Dictionary<string, List<BuildDefinition>> fileToDefinitionMap = FilteredBuildTriggerHelper.GetYamlFileToDefinitionMap(requestContext, new List<BuildDefinition>()
        {
          definition
        });
        if (FilteredBuildTriggerHelper.GetDefinitionRepositoryMappings(requestContext, fileToDefinitionMap, (RepositoryUpdateInfo) null, true, false).TryGetValue(definition, out definitionRepositories) && definitionRepositories.Any<BuildRepository>((Func<BuildRepository, bool>) (d => d != null)))
        {
          definitionRepositories = definitionRepositories.Where<BuildRepository>((Func<BuildRepository, bool>) (d => d != null)).Distinct<BuildRepository>((IEqualityComparer<BuildRepository>) new BuildRepositoryEqualityComparer()).ToList<BuildRepository>();
          return true;
        }
        definitionRepositories = (List<BuildRepository>) null;
        return false;
      }
      definitionRepositories = (List<BuildRepository>) null;
      return false;
    }

    internal void SanitizeDefinitionWithInvalidTaskRefNames(BuildDefinition definition)
    {
      if (!(definition.Process is DesignerProcess process))
        return;
      foreach (Phase phase in process.Phases)
      {
        List<string> list = phase.Steps.Where<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (s => !string.IsNullOrEmpty(s.RefName))).Select<BuildDefinitionStep, string>((Func<BuildDefinitionStep, string>) (step => step.RefName)).ToList<string>();
        bool flag = list.Count != list.Distinct<string>().Count<string>();
        if (phase.Steps.Any<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (step => step.RefName != null && !NameValidation.IsValid(step.RefName))) | flag)
        {
          foreach (BuildDefinitionStep step in phase.Steps)
            step.RefName = string.Empty;
        }
      }
    }

    internal void FixPhaseRefNamesAndDependencies(
      IVssRequestContext requestContext,
      BuildProcess process)
    {
      if (!(process is DesignerProcess buildProcess))
        return;
      List<Phase> phases = buildProcess.Phases;
      if (phases == null)
        return;
      HashSet<string> stringSet = new HashSet<string>(phases.Where<Phase>((Func<Phase, bool>) (p => !string.IsNullOrEmpty(p.RefName))).Select<Phase, string>((Func<Phase, string>) (phase => phase.RefName)));
      List<Phase> phaseList = new List<Phase>(phases.Where<Phase>((Func<Phase, bool>) (p => string.IsNullOrEmpty(p.RefName))));
      int num = 0;
      foreach (Phase phase in phaseList)
      {
        string str = string.Format("Phase_{0}", (object) ++num);
        while (stringSet.Contains(str))
          str = string.Format("Phase_{0}", (object) ++num);
        phase.RefName = str;
      }
      if (buildProcess.SupportsPhaseDependencies())
        return;
      for (int index = 0; index < phases.Count; ++index)
      {
        phases[index].Dependencies = new List<Dependency>();
        if (index > 0)
          phases[index].Dependencies.Add(Dependency.PhaseCompleted(phases[index - 1].RefName));
      }
    }

    internal void ResolveDefinitionTaskVersions(
      IVssRequestContext requestContext,
      BuildDefinition definition)
    {
      Dictionary<Guid, string> dev15RtmVersionSpecs = Dev15RtmVersionSpecs.GetDev15RtmVersionSpecs();
      if (!(definition.Process is DesignerProcess process))
        return;
      foreach (Phase phase in process.Phases)
      {
        if (phase.Steps != null)
        {
          foreach (BuildDefinitionStep buildDefinitionStep in phase.Steps.Where<BuildDefinitionStep>((Func<BuildDefinitionStep, bool>) (s => s.TaskDefinition != null && s.TaskDefinition.VersionSpec == "*")))
          {
            string str = (string) null;
            Guid id = buildDefinitionStep.TaskDefinition.Id;
            if (dev15RtmVersionSpecs.TryGetValue(id, out str))
            {
              requestContext.TraceWarning(12030166, "Service", "Task ID: {0} is being moved from Task Version: '*' to Task Version: '{1}'", (object) id, (object) str);
              buildDefinitionStep.TaskDefinition.VersionSpec = str;
            }
          }
        }
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    internal class DefinitionContinuationData
    {
      internal DefinitionQueryOrder QueryOrder { get; set; }

      internal DateTime? MinTime { get; set; }

      internal DateTime? MaxTime { get; set; }

      internal string LastDefinitionName { get; set; }
    }
  }
}
