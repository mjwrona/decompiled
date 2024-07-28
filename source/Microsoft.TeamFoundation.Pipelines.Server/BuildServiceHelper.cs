// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.BuildServiceHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class BuildServiceHelper
  {
    private const string c_layer = "BuildServiceHelper";

    public static bool IsPipelineDefinition(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition,
      bool autoFetchProperties = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(definition, nameof (definition));
      if (autoFetchProperties)
        definition.PopulateProperties(requestContext, (IEnumerable<string>) new string[1]
        {
          "*"
        });
      PropertiesCollection properties = definition.Properties;
      return properties != null && properties.ContainsKey(PipelineConstants.DefinitionPropertyProviderKey);
    }

    public static void MarkBuildAsPipelineBuild(
      IVssRequestContext requestContext,
      BuildData build,
      string providerId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<BuildData>(build, nameof (build));
      ArgumentUtility.CheckStringForNullOrEmpty(providerId, nameof (providerId));
      BuildServiceHelper.AddPropertyToBuild(requestContext, build, BuildProperties.PipelinesProvider, (object) providerId);
    }

    public static bool IsPipelineBuild(IVssRequestContext requestContext, IReadOnlyBuildData build)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(build, nameof (build));
      PropertiesCollection properties = build.Properties;
      return properties != null && properties.ContainsKey(BuildProperties.PipelinesProvider);
    }

    public static void AddPropertyToBuild(
      IVssRequestContext requestContext,
      BuildData build,
      string propertyName,
      object propertyValue)
    {
      BuildServiceHelper.AddPropertiesToBuild(requestContext, (IReadOnlyBuildData) build, new Dictionary<string, object>()
      {
        {
          propertyName,
          propertyValue
        }
      });
    }

    public static void AddPropertiesToBuild(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      Dictionary<string, object> additionalProperties)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(build, nameof (build));
      ArgumentUtility.CheckForNull<Dictionary<string, object>>(additionalProperties, nameof (additionalProperties));
      IBuildService service = requestContext.GetService<IBuildService>();
      PropertiesCollection properties = build.Properties;
      foreach (KeyValuePair<string, object> additionalProperty in additionalProperties)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(additionalProperty.Key, "Key");
        ArgumentUtility.CheckForNull<object>(additionalProperty.Value, "Value");
        properties[additionalProperty.Key] = additionalProperty.Value;
      }
      service.UpdateProperties(requestContext, build.ProjectId, build.Id, properties);
    }

    public static bool IsReportBuildStatusEnabled(this Microsoft.TeamFoundation.Build2.Server.BuildRepository repository)
    {
      string str;
      bool result;
      return repository != null && ((!repository.Properties.TryGetValue("reportBuildStatus", out str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0;
    }

    public static bool IsTriggerableDefinition(Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition)
    {
      if (definition == null || !BuildServiceHelper.HasEventTriggers(definition) || definition.QueueStatus == Microsoft.TeamFoundation.Build2.Server.DefinitionQueueStatus.Disabled)
        return false;
      Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? definitionQuality1 = definition.DefinitionQuality;
      Microsoft.TeamFoundation.Build2.Server.DefinitionQuality definitionQuality2 = Microsoft.TeamFoundation.Build2.Server.DefinitionQuality.Definition;
      return definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue;
    }

    public static bool IsPullRequestTriggerableDefinition(
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition,
      bool isForkPullRequest)
    {
      if (definition == null || !BuildServiceHelper.HasPullRequestEventTrigger(definition, isForkPullRequest) || definition.QueueStatus == Microsoft.TeamFoundation.Build2.Server.DefinitionQueueStatus.Disabled)
        return false;
      Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? definitionQuality1 = definition.DefinitionQuality;
      Microsoft.TeamFoundation.Build2.Server.DefinitionQuality definitionQuality2 = Microsoft.TeamFoundation.Build2.Server.DefinitionQuality.Definition;
      return definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue;
    }

    public static bool HasEventTriggers(Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition) => definition.Triggers.Any<Microsoft.TeamFoundation.Build2.Server.BuildTrigger>((Func<Microsoft.TeamFoundation.Build2.Server.BuildTrigger, bool>) (trigger => trigger.TriggerType.HasFlag((Enum) Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.ContinuousIntegration))) | definition.Triggers.Any<Microsoft.TeamFoundation.Build2.Server.BuildTrigger>((Func<Microsoft.TeamFoundation.Build2.Server.BuildTrigger, bool>) (trigger => trigger.TriggerType.HasFlag((Enum) Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest)));

    public static bool HasPullRequestEventTrigger(
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition,
      bool isForkPullRequest)
    {
      Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger pullRequestTrigger = definition.Triggers.OfType<Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger>().FirstOrDefault<Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger>((Func<Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger, bool>) (trigger => trigger.TriggerType.HasFlag((Enum) Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest)));
      if (pullRequestTrigger == null)
        return false;
      return pullRequestTrigger.Forks.Enabled || !isForkPullRequest;
    }

    public static bool TryGetPreviousDefinitionRevision(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition,
      out Microsoft.TeamFoundation.Build2.Server.BuildDefinition previousDefinitionRevision)
    {
      if (definition != null)
      {
        // ISSUE: explicit non-virtual call
        int? revision1 = __nonvirtual (definition.Revision);
        int num = 1;
        if (revision1.GetValueOrDefault() > num & revision1.HasValue)
        {
          IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
          ref Microsoft.TeamFoundation.Build2.Server.BuildDefinition local = ref previousDefinitionRevision;
          IBuildDefinitionService definitionService = service;
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId = definition.ProjectId;
          int id = definition.Id;
          int? revision2 = definition.Revision;
          int? definitionVersion = revision2.HasValue ? new int?(revision2.GetValueOrDefault() - 1) : new int?();
          DateTime? minMetricsTime = new DateTime?();
          Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition1 = definitionService.GetDefinition(requestContext1, projectId, id, definitionVersion, minMetricsTime: minMetricsTime);
          local = definition1;
          return true;
        }
      }
      previousDefinitionRevision = (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
      return false;
    }

    public static bool RepositoryChanged(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition previousDefinitionRevision)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(previousDefinitionRevision, nameof (previousDefinitionRevision));
      bool flag = !string.Equals(previousDefinitionRevision.Repository?.Type, definition.Repository?.Type) || !string.Equals(previousDefinitionRevision.Repository?.Id, definition.Repository?.Id);
      requestContext.TraceInfo(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, nameof (BuildServiceHelper), "Repository changed: {0}", (object) flag);
      return flag;
    }

    public static bool ServiceEndpointChanged(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition previousDefinitionRevision)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(previousDefinitionRevision, nameof (previousDefinitionRevision));
      Guid serviceEndpointId1;
      Guid serviceEndpointId2;
      bool flag = definition.Repository.TryGetServiceEndpointId(out serviceEndpointId1) != previousDefinitionRevision.Repository.TryGetServiceEndpointId(out serviceEndpointId2) || serviceEndpointId1 != serviceEndpointId2;
      requestContext.TraceInfo(TracePoints.Events.BuildDefinitionChangedEventListenerHandleEvent, nameof (BuildServiceHelper), "Service endpoint changed: {0}", (object) flag);
      return flag;
    }

    public static IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> GetTriggerableRepositoryDefinitionsForCollection(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      string repository)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      List<Guid> list = vssRequestContext.GetService<IProjectService>().GetProjects(vssRequestContext, ProjectState.WellFormed).Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (projectInfo => projectInfo.Id)).ToList<Guid>();
      requestContext.GetService<IBuildDefinitionService>();
      return BuildServiceHelper.GetTriggerableRepositoryDefinitions(vssRequestContext, provider, list, repository);
    }

    public static IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> GetTriggerableRepositoryDefinitionsForCollection(
      IVssRequestContext requestContext,
      string repositoryType,
      string repository)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      List<Guid> list = vssRequestContext.GetService<IProjectService>().GetProjects(vssRequestContext, ProjectState.WellFormed).Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (projectInfo => projectInfo.Id)).ToList<Guid>();
      requestContext.GetService<IBuildDefinitionService>();
      return BuildServiceHelper.GetTriggerableRepositoryDefinitions(vssRequestContext, repositoryType, list, repository);
    }

    public static IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> GetDefinitionsForPullRequest(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      ExternalGitPullRequest pullRequest)
    {
      List<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> buildDefinitionList = new List<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>();
      IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
      List<Guid> list = requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed).Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (p => p.Id)).ToList<Guid>();
      IVssRequestContext requestContext1 = requestContext;
      List<Guid> projectIds = list;
      string repositoryType = provider.ConnectionCreator.RepositoryType;
      string id = pullRequest.Repo.Id;
      return service.GetDefinitionsWithTriggers(requestContext1, projectIds, repositoryType, id, Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest).Where<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, bool>) (d => BuildServiceHelper.IsPullRequestTriggerableDefinition(d, pullRequest.IsFork) && !d.IsTooNewForTriggers(requestContext)));
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinition GetDefinitionForPullRequest(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      Guid projectId,
      int definitionId,
      ExternalGitPullRequest pullRequest)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckGreaterThanZero((float) definitionId, nameof (definitionId));
      ArgumentUtility.CheckForNull<ExternalGitPullRequest>(pullRequest, nameof (pullRequest));
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = requestContext.GetService<IBuildDefinitionService>().GetDefinition(requestContext, projectId, definitionId);
      if (definition != null && BuildServiceHelper.IsPullRequestTriggerableDefinition(definition, pullRequest.IsFork))
        return definition;
      requestContext.TraceError(TracePoints.Events.AbstractJobExtension, nameof (BuildServiceHelper), string.Format("The definition {0} in the project {1} cannot be triggered for the pull request {2}.", (object) definitionId, (object) projectId, (object) pullRequest.Url));
      return (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
    }

    public static IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> GetTriggerableRepositoryDefinitions(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      List<Guid> projectIds,
      string repository)
    {
      return requestContext.GetService<IBuildDefinitionService>().GetDefinitionsWithTriggers(requestContext, projectIds, provider.ConnectionCreator.RepositoryType, repository, Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.ContinuousIntegration | Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest).Where<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, bool>) (d => BuildServiceHelper.IsTriggerableDefinition(d)));
    }

    public static IEnumerable<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> GetTriggerableRepositoryDefinitions(
      IVssRequestContext requestContext,
      string repositoryType,
      List<Guid> projectIds,
      string repository)
    {
      return requestContext.GetService<IBuildDefinitionService>().GetDefinitionsWithTriggers(requestContext, projectIds, repositoryType, repository, Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.ContinuousIntegration | Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest).Where<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, bool>) (d => BuildServiceHelper.IsTriggerableDefinition(d)));
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildRepository CreateBuildRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repositoryId,
      string repositoryName,
      Guid? serviceEndpointId,
      bool reportStatus = true)
    {
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repositoryType);
      Microsoft.TeamFoundation.Build2.Server.SourceRepository sourceRepository;
      try
      {
        sourceRepository = sourceProvider.GetUserRepository(requestContext, projectId, serviceEndpointId, repositoryId);
      }
      catch (GitRepositoryNotFoundException ex)
      {
        sourceRepository = (Microsoft.TeamFoundation.Build2.Server.SourceRepository) null;
      }
      string uriString = sourceRepository != null ? sourceRepository.Properties["cloneUrl"] : throw new PipelineConnectionException(PipelinesResources.GitHubConnectionExceptionCannotCreateBuildRepository((object) repositoryName));
      requestContext.TraceInfo(TracePoints.Provider.CreateConnection, nameof (BuildServiceHelper), "CreateBuildRepository - " + repositoryType + " url: " + uriString);
      string name = sourceProvider.GetAttributes(requestContext).Name;
      Microsoft.TeamFoundation.Build2.Server.BuildRepository buildRepository = new Microsoft.TeamFoundation.Build2.Server.BuildRepository();
      buildRepository.Id = sourceRepository.Id;
      buildRepository.Type = name;
      buildRepository.Url = new Uri(uriString);
      buildRepository.Name = sourceRepository.FullName;
      buildRepository.DefaultBranch = sourceRepository.DefaultBranch;
      buildRepository.Properties = (IDictionary<string, string>) new Dictionary<string, string>(sourceRepository.Properties);
      buildRepository.Properties["reportBuildStatus"] = reportStatus ? "true" : "false";
      return buildRepository;
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinition CreateJustInTimeBuildDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      string name,
      string targetBranch,
      Microsoft.TeamFoundation.Build2.Server.BuildRepository buildRepository,
      string queueName = "default",
      string comment = null,
      IDictionary<string, object> properties = null,
      IDictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> variables = null,
      IReadOnlyList<string> tags = null,
      Guid authorId = default (Guid),
      string sourceVersion = null,
      string path = null)
    {
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definitionInternal = BuildServiceHelper.CreateDefinitionInternal(requestContext, projectId, targetBranch, buildRepository, (Microsoft.TeamFoundation.Build2.Server.BuildProcess) new Microsoft.TeamFoundation.Build2.Server.JustInTimeProcess(), false, queueName, properties, variables);
      return BuildServiceHelper.AddDefinition(requestContext, name, path, comment, tags, authorId, sourceVersion, definitionInternal);
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinition CreateYamlBuildDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      string name,
      string targetBranch,
      string configurationFilePath,
      Microsoft.TeamFoundation.Build2.Server.BuildRepository buildRepository,
      string queueName = "default",
      string comment = null,
      IDictionary<string, object> properties = null,
      IDictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> variables = null,
      IReadOnlyList<string> tags = null,
      Guid authorId = default (Guid),
      string sourceVersion = null,
      string path = null)
    {
      if (string.IsNullOrEmpty(configurationFilePath))
        configurationFilePath = requestContext.GetService<IRepositoryAnalysisService>().PrimaryConfigurationPath.TrimStart('/');
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId1 = projectId;
      string targetBranch1 = targetBranch;
      Microsoft.TeamFoundation.Build2.Server.BuildRepository buildRepository1 = buildRepository;
      Microsoft.TeamFoundation.Build2.Server.YamlProcess process = new Microsoft.TeamFoundation.Build2.Server.YamlProcess();
      process.YamlFilename = configurationFilePath;
      string queueName1 = queueName;
      IDictionary<string, object> properties1 = properties;
      IDictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> variables1 = variables;
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definitionInternal = BuildServiceHelper.CreateDefinitionInternal(requestContext1, projectId1, targetBranch1, buildRepository1, (Microsoft.TeamFoundation.Build2.Server.BuildProcess) process, true, queueName1, properties1, variables1);
      return BuildServiceHelper.AddDefinition(requestContext, name, path, comment, tags, authorId, sourceVersion, definitionInternal);
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinition CreateDockerDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      string name,
      string targetBranch,
      Microsoft.TeamFoundation.Build2.Server.BuildRepository buildRepository,
      string queueName,
      string comment = null,
      IDictionary<string, object> properties = null,
      IDictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> variables = null,
      IReadOnlyList<string> tags = null,
      Guid authorId = default (Guid),
      string sourceVersion = null,
      string path = null)
    {
      string str = "echo Build and tag\ndocker build -t ${Pipelines.Docker.Tag} .\necho Upload the image to the server\ndocker login -u token -p ${Pipelines.Docker.Token} ${Pipelines.Docker.Server}\ndocker push ${Pipelines.Docker.Tag}";
      Microsoft.TeamFoundation.Build2.Server.DesignerProcess process = new Microsoft.TeamFoundation.Build2.Server.DesignerProcess()
      {
        Phases = {
          new Microsoft.TeamFoundation.Build2.Server.Phase()
          {
            RefName = "Phase_1",
            Name = "Agent job 1",
            JobAuthorizationScope = Microsoft.TeamFoundation.Build2.Server.BuildAuthorizationScope.Project,
            JobTimeoutInMinutes = 60,
            Steps = {
              new Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep()
              {
                DisplayName = "Build and upload docker image",
                Enabled = true,
                TaskDefinition = new Microsoft.TeamFoundation.Build2.Server.TaskDefinitionReference()
                {
                  Id = new Guid("d9bafed4-0b18-4f58-968d-86655b4d2ce9"),
                  VersionSpec = "2.*"
                },
                Inputs = (IDictionary<string, string>) new Dictionary<string, string>()
                {
                  {
                    "script",
                    str
                  }
                }
              }
            },
            Target = (Microsoft.TeamFoundation.Build2.Server.PhaseTarget) new Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget()
            {
              AllowScriptsAuthAccessOption = false
            }
          }
        }
      };
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definitionInternal = BuildServiceHelper.CreateDefinitionInternal(requestContext, projectId, targetBranch, buildRepository, (Microsoft.TeamFoundation.Build2.Server.BuildProcess) process, true, queueName, properties, variables);
      return BuildServiceHelper.AddDefinition(requestContext, name, path, comment, tags, authorId, sourceVersion, definitionInternal);
    }

    private static Microsoft.TeamFoundation.Build2.Server.BuildDefinition CreateDefinitionInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string targetBranch,
      Microsoft.TeamFoundation.Build2.Server.BuildRepository buildRepository,
      Microsoft.TeamFoundation.Build2.Server.BuildProcess process,
      bool addTriggers,
      string queueName,
      IDictionary<string, object> properties,
      IDictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> variables)
    {
      if (string.IsNullOrEmpty(targetBranch))
      {
        targetBranch = buildRepository?.DefaultBranch;
        if (string.IsNullOrEmpty(targetBranch))
          targetBranch = "master";
      }
      List<Microsoft.TeamFoundation.Build2.Server.BuildTrigger> buildTriggerList1 = new List<Microsoft.TeamFoundation.Build2.Server.BuildTrigger>();
      if (addTriggers)
      {
        Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType supportedTriggerTypes = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, buildRepository.Type).GetAttributes(requestContext).SupportedTriggerTypes;
        List<string> stringList = new List<string>()
        {
          "+" + targetBranch
        };
        bool flag = BuildServiceHelper.IsPrivateRepo(buildRepository);
        if (supportedTriggerTypes.HasFlag((Enum) Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.ContinuousIntegration))
        {
          int num = process is Microsoft.TeamFoundation.Build2.Server.YamlProcess ? 2 : 1;
          List<Microsoft.TeamFoundation.Build2.Server.BuildTrigger> buildTriggerList2 = buildTriggerList1;
          Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger integrationTrigger = new Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger();
          integrationTrigger.SettingsSourceType = num;
          integrationTrigger.BranchFilters = stringList;
          buildTriggerList2.Add((Microsoft.TeamFoundation.Build2.Server.BuildTrigger) integrationTrigger);
        }
        if (supportedTriggerTypes.HasFlag((Enum) Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest))
        {
          List<Microsoft.TeamFoundation.Build2.Server.BuildTrigger> buildTriggerList3 = buildTriggerList1;
          Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger pullRequestTrigger = new Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger();
          pullRequestTrigger.SettingsSourceType = 2;
          pullRequestTrigger.Forks = new Microsoft.TeamFoundation.Build2.Server.Forks()
          {
            Enabled = true,
            AllowSecrets = flag
          };
          pullRequestTrigger.BranchFilters = stringList;
          buildTriggerList3.Add((Microsoft.TeamFoundation.Build2.Server.BuildTrigger) pullRequestTrigger);
        }
      }
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition = new Microsoft.TeamFoundation.Build2.Server.BuildDefinition();
      buildDefinition.JobCancelTimeoutInMinutes = 5;
      buildDefinition.JobTimeoutInMinutes = 60;
      buildDefinition.Path = "\\";
      buildDefinition.ProjectId = projectId;
      buildDefinition.Repository = buildRepository;
      buildDefinition.Process = process;
      buildDefinition.Triggers = buildTriggerList1;
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definitionInternal = buildDefinition;
      TaskAgentQueue defaultQueue = BuildServiceHelper.GetDefaultQueue(requestContext, projectId, queueName, "default");
      definitionInternal.Queue = new Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue()
      {
        Id = defaultQueue.Id,
        Name = defaultQueue.Name,
        Pool = new Microsoft.TeamFoundation.Build2.Server.TaskAgentPoolReference()
        {
          Id = defaultQueue.Pool.Id,
          IsHosted = defaultQueue.Pool.IsHosted,
          Name = defaultQueue.Pool.Name
        }
      };
      if (properties != null)
        definitionInternal.Properties = new PropertiesCollection(properties);
      if (variables != null)
        definitionInternal.Variables = new Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>(variables);
      return definitionInternal;
    }

    private static Microsoft.TeamFoundation.Build2.Server.BuildDefinition AddDefinition(
      IVssRequestContext requestContext,
      string name,
      string path,
      string comment,
      IReadOnlyList<string> tags,
      Guid authorId,
      string sourceVersion,
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition)
    {
      IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
      int num = 1;
      definition.Name = name;
      if (!string.IsNullOrEmpty(path))
        definition.Path = path;
      if (!string.IsNullOrEmpty(comment))
        definition.Comment = comment;
      DefinitionUpdateOptions options = new DefinitionUpdateOptions()
      {
        AuthorId = authorId,
        SourceVersion = sourceVersion
      };
      Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> dictionary = definition.Variables.ToDictionary<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>, string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>((Func<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>, string>) (x => x.Key), (Func<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>) (x => x.Value.Clone()));
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition1 = (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
      while (definition1 == null)
      {
        try
        {
          definition1 = service.AddDefinition(requestContext, definition, options);
        }
        catch (DefinitionExistsException ex)
        {
          requestContext.TraceWarning(TracePoints.Connections.CreateDefinition, nameof (BuildServiceHelper), definition.Name + " already exists, increment counter.");
          definition.Name = name + " (" + num++.ToString() + ")";
          definition.Variables = dictionary.ToDictionary<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>, string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>((Func<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>, string>) (x => x.Key), (Func<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>) (x => x.Value.Clone()));
          definition1 = (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
        }
      }
      if (tags != null)
        service.AddTags(requestContext, definition1, (IEnumerable<string>) tags);
      return definition1;
    }

    public static bool IsPrivateRepo(Microsoft.TeamFoundation.Build2.Server.BuildRepository buildRepository)
    {
      string str;
      bool result;
      return buildRepository == null || !buildRepository.Properties.TryGetValue("isPrivate", out str) || !bool.TryParse(str, out result) || result;
    }

    private static TaskAgentQueue GetDefaultQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName,
      string alternateQueueName)
    {
      requestContext.TraceInfo(TracePoints.Connections.CreateDefinition, nameof (BuildServiceHelper), string.Format("{0}({1},{2}", (object) nameof (GetDefaultQueue), (object) projectId, (object) queueName));
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      TaskAgentQueue defaultQueue = service.GetAgentQueues(requestContext, projectId, queueName).SingleOrDefault<TaskAgentQueue>();
      if (defaultQueue == null)
      {
        requestContext.TraceInfo(TracePoints.Connections.CreateDefinition, nameof (BuildServiceHelper), "Unable to find agent queue matching: " + queueName + ". Using alternate: " + alternateQueueName);
        defaultQueue = service.GetAgentQueues(requestContext, projectId, alternateQueueName, new TaskAgentQueueActionFilter?(TaskAgentQueueActionFilter.Use)).SingleOrDefault<TaskAgentQueue>();
      }
      if (defaultQueue == null)
      {
        defaultQueue = service.GetAgentQueues(requestContext, projectId, actionFilter: new TaskAgentQueueActionFilter?(TaskAgentQueueActionFilter.Use)).FirstOrDefault<TaskAgentQueue>();
        if (defaultQueue != null)
          requestContext.TraceInfo(TracePoints.Connections.CreateDefinition, nameof (BuildServiceHelper), "Unable to find agent queue matching: " + queueName + " or " + alternateQueueName + ". Selecting first available queue: " + defaultQueue.Name);
      }
      if (defaultQueue == null)
      {
        requestContext.TraceError(TracePoints.Connections.CreateDefinition, nameof (BuildServiceHelper), "Unable to find any available agent queues");
        throw new PipelineConnectionException(PipelinesResources.ExceptionAgentNoAvailableQueueFound());
      }
      return defaultQueue;
    }
  }
}
