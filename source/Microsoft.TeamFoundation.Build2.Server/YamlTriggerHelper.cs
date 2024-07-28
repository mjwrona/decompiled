// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.YamlTriggerHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class YamlTriggerHelper
  {
    private const string c_area = "Build2";
    private const string c_layer = "YamlTriggerHelper";

    internal static void LoadYamlTriggers(
      IVssRequestContext requestContext,
      DefinitionTriggerType triggerType,
      IBuildSourceProvider sourceProvider,
      RepositoryUpdateInfo repositoryUpdateInfo,
      RefUpdateInfo refUpdate,
      IdentityRef sourceOwner,
      string sourceVersion,
      IExternalGitEvent gitEvent,
      List<BuildDefinition> definitions,
      List<TriggerInstance> matchingTriggers,
      List<TriggerLoadError> errors)
    {
      using (new ExcessiveMemoryAllocationTraceScope(requestContext, 12030349, "Build2", nameof (YamlTriggerHelper), new Lazy<string>((Func<string>) (() => new
      {
        DefinitionCount = definitions?.Count,
        PipelineEventId = gitEvent?.PipelineEventId,
        TriggerType = triggerType,
        RepositoryId = repositoryUpdateInfo?.RepositoryId,
        BranchName = refUpdate?.RefName
      }.Serialize())), 500, caller: nameof (LoadYamlTriggers)))
      {
        try
        {
          if (repositoryUpdateInfo == null)
          {
            requestContext.TraceAlways(12030334, TraceLevel.Warning, "Build2", nameof (YamlTriggerHelper), "Returning from LoadYamlTriggers. repositoryUpdateInfo is null.");
          }
          else
          {
            long updateId = repositoryUpdateInfo.UpdateId;
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            RetrieveOptions retrieveOptions1;
            if (triggerType == DefinitionTriggerType.ContinuousIntegration)
            {
              string str = BuildSourceProviders.GitProperties.BranchToRefName(refUpdate.RefName) ?? string.Empty;
              if (!str.StartsWith("refs/heads/", StringComparison.Ordinal) && !str.StartsWith("refs/tags/", StringComparison.Ordinal))
              {
                requestContext.TraceInfo(12030144, nameof (YamlTriggerHelper), "CI with YAML-defined triggers is not allowed for branch '{0}', repository ID '{1}', project ID '{2}'", (object) str, (object) definitions[0]?.Repository?.Id, (object) definitions[0]?.ProjectId);
                return;
              }
              retrieveOptions1 = RetrieveOptions.ContinuousIntegrationTrigger;
            }
            else
            {
              if (triggerType != DefinitionTriggerType.PullRequest)
                return;
              retrieveOptions1 = RetrieveOptions.PullRequestTrigger;
            }
            long num1 = 0;
            long num2 = 0;
            long num3 = 0;
            List<BuildDefinition> definitions1 = new List<BuildDefinition>();
            foreach (BuildDefinition buildDefinition in definitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (d => d?.Repository != null)))
            {
              BuildDefinition definition = buildDefinition;
              YamlProcess process = definition.GetProcess<YamlProcess>();
              IYamlPipelineLoaderService service1 = requestContext.GetService<IYamlPipelineLoaderService>();
              string sourceBranch;
              string sourceVersion1;
              if (string.Equals(definition.Repository.Id, repositoryUpdateInfo.RepositoryId, StringComparison.OrdinalIgnoreCase))
              {
                sourceBranch = refUpdate.RefName;
                sourceVersion1 = sourceVersion;
              }
              else
              {
                Guid serviceEndpointId = Guid.Empty;
                definition.Repository.TryGetServiceEndpointId(out serviceEndpointId);
                try
                {
                  SourceRepository userRepository = sourceProvider.GetUserRepository(requestContext, definition.ProjectId, new Guid?(serviceEndpointId), definition.Repository.Id);
                  if (userRepository == null)
                  {
                    requestContext.TraceAlways(12030335, TraceLevel.Info, "Build2", "QueueFailedBuild", string.Format("Queuing a failed build for definition id {0}, definition name {1} because GetUserRepository API returned null for repository name {2}", (object) definition?.Id, (object) definition?.Name, (object) definition?.Repository?.Name));
                    errors.Add(new TriggerLoadError(definition, updateId, refUpdate, sourceOwner, triggerType, new Exception("Failed to get the user respository for repository Id '" + definition.Repository.Id + "'.")));
                    continue;
                  }
                  sourceBranch = sourceProvider.NormalizeSourceBranch(userRepository.DefaultBranch, definition);
                  sourceVersion1 = (string) null;
                  requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "We will use latest version of YAML file from default branch '" + sourceBranch + "' as the refUpdate repository '" + repositoryUpdateInfo.RepositoryId + "' doesn't match with pipeline's repository '" + definition.Repository.Id + "'.");
                }
                catch (Exception ex)
                {
                  PipelineEventLogger.LogException(requestContext, gitEvent, ex);
                  requestContext.TraceException(12030336, nameof (YamlTriggerHelper), ex);
                  continue;
                }
              }
              Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repositoryResource1 = (Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource) null;
              YamlPipelineLoadResult pipelineLoadResult;
              try
              {
                IBuildResourceAuthorizationService service2 = requestContext.GetService<IBuildResourceAuthorizationService>();
                Stopwatch stopwatch2 = Stopwatch.StartNew();
                IVssRequestContext requestContext1 = requestContext;
                Guid projectId1 = definition.ProjectId;
                int id = definition.Id;
                ResourceType? resourceType = new ResourceType?();
                PipelineResources authorizedResources = service2.GetAuthorizedResources(requestContext1, projectId1, id, resourceType).ToPipelineResources() ?? new PipelineResources();
                num1 += stopwatch2.ElapsedMilliseconds;
                IVssRequestContext vssRequestContext = requestContext;
                repositoryResource1 = definition.Repository.ToRepositoryResource(requestContext, PipelineConstants.SelfAlias, sourceBranch, sourceVersion1);
                if (repositoryResource1.Endpoint != null && !process.SupportsYamlRepositoryEndpointAuthorization())
                  authorizedResources.Endpoints.Add(repositoryResource1.Endpoint);
                if (sourceProvider.HasLimitedRights(requestContext, definition))
                  vssRequestContext = requestContext.Elevate();
                stopwatch2.Restart();
                PipelineBuilder pipelineBuilder = definition.GetPipelineBuilder(requestContext, authorizedResources);
                num2 += stopwatch2.ElapsedMilliseconds;
                stopwatch2.Restart();
                IYamlPipelineLoaderService pipelineLoaderService = service1;
                IVssRequestContext requestContext2 = vssRequestContext;
                Guid projectId2 = definition.ProjectId;
                Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repository = repositoryResource1;
                string yamlFilename = process.YamlFilename;
                PipelineBuilder builder = pipelineBuilder;
                RetrieveOptions retrieveOptions2 = retrieveOptions1;
                int? defaultQueueId = new int?();
                int retrieveOptions3 = (int) retrieveOptions2;
                pipelineLoadResult = pipelineLoaderService.Load(requestContext2, projectId2, repository, yamlFilename, builder, defaultQueueId, retrieveOptions: (RetrieveOptions) retrieveOptions3);
                num3 += stopwatch2.ElapsedMilliseconds;
              }
              catch (YamlFileNotFoundException ex)
              {
                PipelineEventLogger.LogException(requestContext, gitEvent, (Exception) ex);
                requestContext.TraceAlways(12030130, TraceLevel.Warning, "Build2", nameof (YamlTriggerHelper), "Yaml file '{0}' does not exist for project '{1}', repo '{2}', ref '{3}', commit '{4}'", (object) process.YamlFilename, (object) definition.ProjectName, (object) definition.Repository.Name, (object) refUpdate.RefName, (object) sourceVersion);
                continue;
              }
              catch (Exception ex)
              {
                PipelineEventLogger.LogException(requestContext, gitEvent, ex);
                requestContext.TraceAlways(12030337, TraceLevel.Warning, "Build2", "QueueFailedBuild", string.Format("Queuing a failed build for definition id {0}, definition name {1}, repository name {2} due to the following error: {3}", (object) definition?.Id, (object) definition?.Name, (object) repositoryResource1?.Name, (object) ex));
                errors.Add(new TriggerLoadError(definition, updateId, refUpdate, sourceOwner, triggerType, ex));
                continue;
              }
              bool flag1 = pipelineLoadResult.Template.Resources.Repositories.Any<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource, bool>) (r => r?.Id == definition.Repository.Id && r?.Trigger != null));
              bool flag2 = pipelineLoadResult.Template.Resources.Repositories.Any<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource, bool>) (r => r?.Id == definition.Repository.Id && r?.PR != null));
              if (pipelineLoadResult.Template.Errors.Count > 0)
              {
                requestContext.TraceAlways(12030338, TraceLevel.Warning, "Build2", "QueueFailedBuild", string.Format("Queuing a failed build for definition id {0}, definition name {1}, repository name {2} because of the YAML template loading errors", (object) definition?.Id, (object) definition?.Name, (object) repositoryResource1?.Name));
                errors.Add(new TriggerLoadError(definition, updateId, refUpdate, sourceOwner, triggerType, (IEnumerable<PipelineValidationError>) pipelineLoadResult.Template.Errors));
              }
              else if (pipelineLoadResult.Template.Triggers.Count > 0)
              {
                foreach (PipelineTrigger pipelineTrigger1 in pipelineLoadResult.Template.Triggers.Where<PipelineTrigger>((Func<PipelineTrigger, bool>) (t => t != null)))
                {
                  if (!pipelineTrigger1.Enabled)
                  {
                    requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "trigger type '{0}' for definition id '{1}', repository id '{2}', project ID '{3}' is not enabled", (object) pipelineTrigger1.TriggerType, (object) definition.Id, (object) definition.Repository.Id, (object) definition.ProjectId);
                  }
                  else
                  {
                    FilteredBuildTrigger trigger = (FilteredBuildTrigger) null;
                    PipelineTrigger pipelineTrigger2 = pipelineTrigger1;
                    if (!(pipelineTrigger2 is Microsoft.TeamFoundation.DistributedTask.Pipelines.ContinuousIntegrationTrigger integrationTrigger))
                    {
                      if (pipelineTrigger2 is Microsoft.TeamFoundation.DistributedTask.Pipelines.PullRequestTrigger pullRequestTrigger && triggerType == DefinitionTriggerType.PullRequest)
                      {
                        if (!pullRequestTrigger.Drafts && gitEvent is ExternalGitPullRequest externalGitPullRequest && externalGitPullRequest.Draft)
                        {
                          PipelineEventLogger.IgnoreEvent(requestContext, gitEvent, "draft pull request");
                        }
                        else
                        {
                          trigger = (FilteredBuildTrigger) new PullRequestTrigger()
                          {
                            AutoCancel = new bool?(pullRequestTrigger.AutoCancel)
                          };
                          trigger.BranchFilters.AddRange((IEnumerable<string>) pullRequestTrigger.BranchFilters);
                          trigger.PathFilters.AddRange((IEnumerable<string>) pullRequestTrigger.PathFilters);
                          requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "PR trigger created for definition id '{0}' for repository id '{1}'. GitHub PR draft is: '{2}'. YAML PR trigger 'drafts' set to: '{3}'", (object) definition.Id, (object) definition.Repository.Id, (object) ((ExternalGitPullRequest) gitEvent).Draft, (object) pullRequestTrigger.Drafts);
                        }
                      }
                    }
                    else if (triggerType == DefinitionTriggerType.ContinuousIntegration)
                    {
                      trigger = (FilteredBuildTrigger) new ContinuousIntegrationTrigger()
                      {
                        BatchChanges = integrationTrigger.BatchChanges
                      };
                      trigger.BranchFilters.AddRange((IEnumerable<string>) integrationTrigger.BranchFilters);
                      trigger.PathFilters.AddRange((IEnumerable<string>) integrationTrigger.PathFilters);
                    }
                    if (trigger != null && string.Equals(definition.Repository.Id, repositoryUpdateInfo.RepositoryId, StringComparison.OrdinalIgnoreCase) && string.Equals(definition.Repository.Type, repositoryUpdateInfo.RepositoryType, StringComparison.OrdinalIgnoreCase))
                    {
                      if (FilteredBuildTriggerHelper.IsMatchingBranch(requestContext, refUpdate, definition, trigger))
                      {
                        requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "build trigger type {0} matched for RefName '{1}', definition id '{2}', repository id '{3}', project ID '{4}, definition name '{5}'", (object) trigger.TriggerType, (object) refUpdate.RefName, (object) definition.Id, (object) definition.Repository.Id, (object) definition.ProjectId, (object) definition.Name);
                        matchingTriggers.Add(new TriggerInstance(definition, trigger, updateId, refUpdate, sourceOwner, sourceVersion));
                      }
                      else if (triggerType == DefinitionTriggerType.PullRequest)
                        definitions1.Add(definition);
                    }
                  }
                }
              }
              else if (triggerType == DefinitionTriggerType.ContinuousIntegration && string.Equals(definition.Repository.Id, repositoryUpdateInfo.RepositoryId, StringComparison.OrdinalIgnoreCase) && !flag1)
              {
                ProjectPipelineGeneralSettingsHelper generalSettingsHelper = new ProjectPipelineGeneralSettingsHelper(requestContext, definition.ProjectId, true);
                if (requestContext.IsFeatureEnabled("Build2.DisableImpliedYAMLCiTrigger") && generalSettingsHelper.GetEffectiveSettings().DisableImpliedYAMLCiTrigger)
                {
                  requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "CI triggers are not configured explicitly for definition id '{0}', repository id '{1}', project ID '{2}, definition name {3} and DisableImpliedYAMLCiTrigger setting is set to true", (object) definition.Id, (object) definition.Repository.Id, (object) definition.ProjectId, (object) definition.Name);
                }
                else
                {
                  requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "CI triggers are not configured explicitly for definition id '{0}', repository id '{1}', project ID '{2}, definition name {3}", (object) definition.Id, (object) definition.Repository.Id, (object) definition.ProjectId, (object) definition.Name);
                  ContinuousIntegrationTrigger integrationTrigger = new ContinuousIntegrationTrigger();
                  integrationTrigger.BranchFilters.Add("+refs/heads/*");
                  ContinuousIntegrationTrigger trigger = integrationTrigger;
                  if (FilteredBuildTriggerHelper.IsMatchingBranch(requestContext, refUpdate, definition, (FilteredBuildTrigger) trigger))
                  {
                    requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "Default CI trigger matched for RefName '{0}', definition id '{1}', repository id '{2}', project ID '{3}, definition name '{4}'", (object) refUpdate.RefName, (object) definition.Id, (object) definition.Repository.Id, (object) definition.ProjectId, (object) definition.Name);
                    matchingTriggers.Add(new TriggerInstance(definition, (FilteredBuildTrigger) trigger, updateId, refUpdate, sourceOwner, sourceVersion));
                  }
                }
              }
              else if (triggerType == DefinitionTriggerType.PullRequest && string.Equals(definition.Repository.Id, repositoryUpdateInfo.RepositoryId, StringComparison.OrdinalIgnoreCase) && !flag2)
              {
                requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "PR triggers are not configured explicitly for definition id '{0}', repository id '{1}', project ID '{2}", (object) definition.Id, (object) definition.Repository.Id, (object) definition.ProjectId);
                PullRequestTrigger pullRequestTrigger = new PullRequestTrigger();
                pullRequestTrigger.BranchFilters.Add("+refs/heads/*");
                PullRequestTrigger trigger = pullRequestTrigger;
                if (FilteredBuildTriggerHelper.IsMatchingBranch(requestContext, refUpdate, definition, (FilteredBuildTrigger) trigger))
                {
                  requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "Default PR trigger matched for RefName '{0}', definition id '{1}', repository id '{2}', project ID '{3}", (object) refUpdate.RefName, (object) definition.Id, (object) definition.Repository.Id, (object) definition.ProjectId);
                  matchingTriggers.Add(new TriggerInstance(definition, (FilteredBuildTrigger) trigger, updateId, refUpdate, sourceOwner, sourceVersion));
                }
              }
              if (pipelineLoadResult.Template.Resources.Repositories.Any<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource, bool>) (r => r?.Trigger != null)))
              {
                List<TriggerInstance> triggerInstanceList = new List<TriggerInstance>();
                Dictionary<string, RepositoryResourceParameters> values = new Dictionary<string, RepositoryResourceParameters>();
                IBuildSourceProviderService service3 = requestContext.GetService<IBuildSourceProviderService>();
                foreach (Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repositoryResource2 in pipelineLoadResult.Template.Resources.Repositories.Where<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource, bool>) (r => r != null)))
                {
                  requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "Loading repository triggers for RefName '{0}', definition id '{1}', repository id '{2}', project ID '{3}", (object) refUpdate.RefName, (object) definition.Id, (object) repositoryResource2.Id, (object) definition.ProjectId);
                  BuildRepository buildRepository1 = new BuildRepository();
                  buildRepository1.Type = repositoryResource2.Type;
                  buildRepository1.Id = repositoryResource2.Id;
                  buildRepository1.Name = repositoryResource2.Name;
                  BuildRepository buildRepository2 = buildRepository1;
                  try
                  {
                    sourceProvider.SetRepositoryDefaultInfo(requestContext, definition.ProjectId, buildRepository2);
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(12030339, nameof (YamlTriggerHelper), ex);
                    PipelineEventLogger.LogException(requestContext, gitEvent, ex);
                    continue;
                  }
                  buildRepository2.FixBuildRepositoryType();
                  RepositoryResourceParameters resourceParameters = new RepositoryResourceParameters(string.IsNullOrEmpty(repositoryResource2.Ref) ? buildRepository2.DefaultBranch : repositoryResource2.Ref, (string) null, repositoryResource2.Endpoint);
                  if (buildRepository2.Id == repositoryUpdateInfo.RepositoryId && (triggerType != DefinitionTriggerType.PullRequest && repositoryResource2.Trigger != null && repositoryResource2.Trigger.Enabled || triggerType == DefinitionTriggerType.PullRequest && repositoryResource2.PR != null))
                  {
                    FilteredBuildTrigger trigger;
                    if (triggerType == DefinitionTriggerType.PullRequest)
                    {
                      trigger = (FilteredBuildTrigger) new PullRequestTrigger();
                      trigger.BranchFilters.AddRange((IEnumerable<string>) repositoryResource2.PR.BranchFilters);
                      trigger.PathFilters.AddRange((IEnumerable<string>) repositoryResource2.PR.PathFilters);
                    }
                    else
                    {
                      trigger = (FilteredBuildTrigger) new ContinuousIntegrationTrigger()
                      {
                        BatchChanges = repositoryResource2.Trigger.BatchChanges
                      };
                      trigger.BranchFilters.AddRange((IEnumerable<string>) repositoryResource2.Trigger.BranchFilters);
                      trigger.PathFilters.AddRange((IEnumerable<string>) repositoryResource2.Trigger.PathFilters);
                    }
                    if (FilteredBuildTriggerHelper.IsMatchingBranch(requestContext, refUpdate, definition, trigger))
                    {
                      Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference endpoint = repositoryResource2.Endpoint;
                      int num4;
                      if (endpoint == null)
                      {
                        num4 = 0;
                      }
                      else
                      {
                        Guid id = endpoint.Id;
                        num4 = 1;
                      }
                      if (num4 != 0 && repositoryResource2.Endpoint.Id != Guid.Empty)
                        buildRepository2.Properties.Add("connectedServiceId", repositoryResource2.Endpoint.Id.ToString());
                      try
                      {
                        string str;
                        if (service3.GetSourceProvider(requestContext, buildRepository2.Type).GetUserRepository(requestContext, definition.ProjectId, repositoryResource2.Endpoint?.Id, repositoryResource2.Id).Properties.TryGetValue("apiUrl", out str))
                          buildRepository2.Properties.Add("apiUrl", str);
                      }
                      catch (Exception ex)
                      {
                        requestContext.TraceAlways(12030340, TraceLevel.Warning, "Build2", nameof (YamlTriggerHelper), "Exception while fetching user repository details for repository '" + repositoryResource2.Name + "' while processing resource repository triggers - " + ex.Message + ".");
                        continue;
                      }
                      requestContext.TraceInfo(12030244, nameof (YamlTriggerHelper), "Repository resource trigger type {0} matched for RefName '{1}', definition id '{2}', repository id '{3}', project ID '{4}", (object) trigger.TriggerType, (object) refUpdate.RefName, (object) definition.Id, (object) repositoryResource2.Id, (object) definition.ProjectId);
                      triggerInstanceList.Add(new TriggerInstance(definition, trigger, updateId, refUpdate, sourceOwner, sourceVersion, buildRepository2));
                      resourceParameters = new RepositoryResourceParameters(refUpdate.RefName, sourceVersion, repositoryResource2.Endpoint);
                    }
                  }
                  if (!values.ContainsKey(repositoryResource2.Alias))
                    values.Add(repositoryResource2.Alias, resourceParameters);
                }
                foreach (TriggerInstance triggerInstance in triggerInstanceList)
                {
                  triggerInstance.ResourcesParameters.Repositories.Clear();
                  triggerInstance.ResourcesParameters.Repositories.AddRange<KeyValuePair<string, RepositoryResourceParameters>, Dictionary<string, RepositoryResourceParameters>>((IEnumerable<KeyValuePair<string, RepositoryResourceParameters>>) values);
                  matchingTriggers.Add(triggerInstance);
                }
              }
            }
            FilteredBuildTriggerHelper.PostNeutralStatusesForSkippedChange(requestContext, (IEnumerable<BuildDefinition>) definitions1, BuildServerResources.GitHubCheckSkippedNoMatchingBranchFilter(), Sha1Id.ValidateSha(refUpdate.NewObjectId, sourceVersion), gitEvent);
            long elapsedMilliseconds = stopwatch1.ElapsedMilliseconds;
            if (elapsedMilliseconds <= 60000L)
              return;
            requestContext.TraceAlways(12030341, TraceLevel.Warning, "Build2", nameof (YamlTriggerHelper), "LoadYaml for pipeline event {0},getAuthorizedResourcestimeTaken: {1}, getPipelineBuildertimeTaken: {2}, loadTimeTaken: {3}, overallTimer: {4}", (object) gitEvent, (object) num1, (object) num2, (object) num3, (object) elapsedMilliseconds);
          }
        }
        catch (Exception ex)
        {
          PipelineEventLogger.LogException(requestContext, gitEvent, ex);
          requestContext.TraceException(12030342, nameof (YamlTriggerHelper), ex);
        }
      }
    }
  }
}
