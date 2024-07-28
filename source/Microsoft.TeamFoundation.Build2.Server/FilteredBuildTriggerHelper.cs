// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.FilteredBuildTriggerHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.ServiceHooks.Sdk.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class FilteredBuildTriggerHelper
  {
    private const string c_neutral = "neutral";
    private const int c_maxCommitMessageLength = 200;
    private const string c_cisourcebranch = "cisourcebranch";
    private const string c_cisourcesha = "cisourcesha";
    private const int c_slownessTreshold = 120000;
    private const string c_area = "Build2";
    private const string c_layer = "FilteredBuildTriggerHelper";
    private const string c_repoUnreachable = "RepoNotReachable";

    public static List<TriggerInstance> GetCIBranchUpdates(
      IVssRequestContext requestContext,
      List<BuildDefinition> definitions,
      RepositoryUpdateInfo repositoryUpdateInfo,
      IdentityRef sourceOwner,
      List<TriggerLoadError> triggerLoadErrors,
      IExternalGitEvent gitEvent)
    {
      return FilteredBuildTriggerHelper.GetBranchUpdates(requestContext, definitions, repositoryUpdateInfo, sourceOwner, triggerLoadErrors, DefinitionTriggerType.ContinuousIntegration, gitEvent);
    }

    public static List<TriggerInstance> GetPullRequestUpdates(
      IVssRequestContext requestContext,
      List<BuildDefinition> definitions,
      RepositoryUpdateInfo repositoryUpdateInfo,
      IdentityRef sourceOwner,
      List<TriggerLoadError> triggerLoadErrors,
      ExternalGitPullRequest pullRequest)
    {
      return FilteredBuildTriggerHelper.GetBranchUpdates(requestContext, definitions, repositoryUpdateInfo, sourceOwner, triggerLoadErrors, DefinitionTriggerType.PullRequest, (IExternalGitEvent) pullRequest);
    }

    public static Dictionary<BuildDefinition, List<BuildRepository>> UpdateResourceRepositories(
      IVssRequestContext requestContext,
      List<BuildDefinition> yamlDefinitions,
      RepositoryUpdateInfo repositoryUpdateInfo,
      bool manualRun = false)
    {
      if (!yamlDefinitions.Any<BuildDefinition>() || !manualRun && repositoryUpdateInfo.RefUpdates.Count == 0)
      {
        requestContext.TraceAlways(12030269, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), "Tried to update resource repositories for YAML, but either there were not real updates or no YAML definitions.");
        return (Dictionary<BuildDefinition, List<BuildRepository>>) null;
      }
      List<BuildDefinition> yamlDefinitions1 = yamlDefinitions;
      if (!manualRun)
      {
        ArgumentUtility.CheckForNull<RepositoryUpdateInfo>(repositoryUpdateInfo, nameof (repositoryUpdateInfo));
        yamlDefinitions1 = yamlDefinitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (def => def.RepositoryMatchesUpdateInfo(requestContext, repositoryUpdateInfo))).ToList<BuildDefinition>();
      }
      Dictionary<string, List<BuildDefinition>> fileToDefinitionMap = FilteredBuildTriggerHelper.GetYamlFileToDefinitionMap(requestContext, yamlDefinitions1);
      Dictionary<BuildDefinition, List<BuildRepository>> repositoryMappings = FilteredBuildTriggerHelper.GetDefinitionRepositoryMappings(requestContext, fileToDefinitionMap, repositoryUpdateInfo, manualRun, false);
      if (repositoryMappings != null)
      {
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        {
          List<BuildDefinitionRepositoryMap> deletedDefRepoPairs = new List<BuildDefinitionRepositoryMap>();
          List<BuildDefinitionRepositoryMap> addedDefRepoPairs = new List<BuildDefinitionRepositoryMap>();
          component.UpdateDefinitionRepositoryMappings(repositoryMappings, out deletedDefRepoPairs, out addedDefRepoPairs);
          if (deletedDefRepoPairs.Count <= 0)
          {
            if (addedDefRepoPairs.Count <= 0)
              goto label_12;
          }
          requestContext.TraceAlways(12030269, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("Deleted '{0}' and added '{1}' definition-repository mappings in tbl_DefinitionRepositoryMapping as part of YAML update.", (object) deletedDefRepoPairs.Count, (object) addedDefRepoPairs.Count));
        }
      }
label_12:
      return repositoryMappings;
    }

    public static List<BuildData> QueueBuildsForRepositoryUpdate(
      IVssRequestContext requestContext,
      List<TriggerInstance> triggerInstances,
      List<TriggerLoadError> triggerLoadErrors,
      Action<BuildData> additionalBuildSettings = null,
      List<Exception> queueExceptions = null,
      HashSet<string> internalRuntimeVariables = null)
    {
      using (requestContext.TraceSlowCall(nameof (FilteredBuildTriggerHelper), 120000, new Lazy<string>((Func<string>) (() => string.Format("triggerInstances.Count={0}, triggerLoadErrors.Count={1}", (object) triggerInstances?.Count, (object) triggerLoadErrors?.Count))), nameof (QueueBuildsForRepositoryUpdate)))
      {
        StringBuilder stringBuilder = new StringBuilder();
        List<BuildData> buildDataList = new List<BuildData>();
        if (triggerLoadErrors.Count > 0)
        {
          requestContext.TraceAlways(12030305, TraceLevel.Warning, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("QueueBuildsForRepositoryUpdate called with {0} trigger load errors!", (object) triggerLoadErrors.Count));
          IBuildService service = requestContext.GetService<IBuildService>();
          foreach (TriggerLoadError triggerLoadError in triggerLoadErrors)
          {
            BuildData build1 = new BuildData()
            {
              ProjectId = triggerLoadError.Definition.ProjectId,
              Definition = (MinimalBuildDefinition) triggerLoadError.Definition,
              Priority = QueuePriority.Normal,
              QueueId = triggerLoadError.Definition.Queue?.Id,
              Reason = triggerLoadError.TriggerType == DefinitionTriggerType.PullRequest ? BuildReason.PullRequest : BuildReason.IndividualCI,
              RequestedFor = triggerLoadError.Branch.PendingSourceOwner,
              SourceBranch = triggerLoadError.Branch.BranchName,
              SourceVersion = triggerLoadError.Branch.PendingSourceVersion
            };
            List<PipelineTriggerIssues> triggers = new List<PipelineTriggerIssues>(triggerLoadError.Errors.Count);
            bool flag = requestContext.IsFeatureEnabled("Build2.ReportUnreachableExternalRepo");
            foreach (PipelineValidationError error in triggerLoadError.Errors)
            {
              build1.ValidationResults.Add(new BuildRequestValidationResult()
              {
                Result = ValidationResult.Error,
                Message = error.Message
              });
              if (flag)
                triggers.Add(new PipelineTriggerIssues()
                {
                  PipelineDefinitionId = triggerLoadError.Definition.Id,
                  isError = true,
                  RepositoryUrl = triggerLoadError.Definition.Repository?.Url?.ToString(),
                  ErrorMessage = error.Message
                });
            }
            if (additionalBuildSettings != null)
              additionalBuildSettings(build1);
            BuildData build2 = (BuildData) null;
            string str1 = "";
            try
            {
              BuildRequestValidationFlags validationFlags = BuildRequestValidationFlags.QueueFailedBuild | BuildRequestValidationFlags.SkipSourceVersionValidation;
              if (flag)
              {
                PipelineValidationError pipelineValidationError = triggerLoadError.Errors.Where<PipelineValidationError>((Func<PipelineValidationError, bool>) (err => err.Code == "RepoNotReachable")).FirstOrDefault<PipelineValidationError>();
                if (pipelineValidationError != null)
                  build1.TriggerInfo["ci.message"] = FilteredBuildTriggerHelper.TruncateMessageToFirstLine(pipelineValidationError.Message);
                if (pipelineValidationError != null)
                  validationFlags |= BuildRequestValidationFlags.QueueInconclusiveBuild;
              }
              build2 = service.QueueBuild(requestContext, build1, (IEnumerable<IBuildRequestValidator>) Array.Empty<IBuildRequestValidator>(), validationFlags, callingMethod: nameof (QueueBuildsForRepositoryUpdate), callingFile: "D:\\a\\_work\\1\\s\\Tfs\\Service\\Build2\\Server\\Helpers\\FilteredBuildTriggerHelper.cs");
              str1 = string.Format("Queued failed YAML build request '{0}' for definition id {1}, definition name {2} using branch {3} version {4}.", (object) build2?.Id, (object) triggerLoadError.Definition.Id, (object) triggerLoadError.Definition.Name, (object) triggerLoadError.Branch.BranchName, (object) triggerLoadError.Branch.PendingSourceVersion);
              if (flag)
              {
                if (requestContext.IsFeatureEnabled("DistributedTask.EnableYamlPipelineTriggerIssues"))
                {
                  foreach (PipelineTriggerIssues pipelineTriggerIssues in triggers)
                    pipelineTriggerIssues.BuildNumber = build2.BuildNumber;
                  if (triggers.Count > 0)
                    requestContext.GetService<IPipelineTriggerIssuesService>().CreatePipelineTriggerIssues(requestContext, triggerLoadError.Definition.ProjectId, triggerLoadError.Definition.Id, (IList<PipelineTriggerIssues>) triggers);
                }
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12030306, nameof (FilteredBuildTriggerHelper), ex);
              queueExceptions?.Add(ex);
            }
            CustomerIntelligenceData ciData;
            if (build2 != null)
            {
              string message = string.Format("Queued failed YAML build request '{0}' for definition id {1}, definition name {2} using branch {3} version {4}", (object) build2.Id, (object) triggerLoadError.Definition.Id, (object) triggerLoadError.Definition.Name, (object) triggerLoadError.Branch.BranchName, (object) triggerLoadError.Branch.PendingSourceVersion);
              requestContext.TraceAlways(12030307, TraceLevel.Info, "Build2", "QueueFailedBuild", message + " Stack Trace: " + Environment.StackTrace);
              ciData = new CustomerIntelligenceData((IDictionary<string, object>) FilteredBuildTriggerHelper.GetBuildCIData(build2, message));
              buildDataList.Add(build2);
            }
            else
            {
              string str2 = string.Format("QueueBuild call for a Yaml error in definition {0} using branch {1} version {2} did not actually queue a build", (object) triggerLoadError.Definition.Id, (object) triggerLoadError.Branch.BranchName, (object) triggerLoadError.Branch.PendingSourceVersion);
              requestContext.TraceInfo(12030149, nameof (FilteredBuildTriggerHelper), str2);
              ciData = new CustomerIntelligenceData((IDictionary<string, object>) FilteredBuildTriggerHelper.GetBuildCIData(build1, str2));
              build1.Result = new BuildResult?(BuildResult.Failed);
              buildDataList.Add(build1);
            }
            FilteredBuildTriggerHelper.PublishCI(requestContext, ciData);
          }
        }
        if (triggerInstances.Count > 0)
        {
          IBuildDefinitionService service1 = requestContext.GetService<IBuildDefinitionService>();
          IBuildService service2 = requestContext.GetService<IBuildService>();
          IBuildSourceProviderService sourceProviderService = requestContext.GetService<IBuildSourceProviderService>();
          foreach (IGrouping<\u003C\u003Ef__AnonymousType1<int, DefinitionTriggerType, bool, bool?>, TriggerInstance> source in triggerInstances.GroupBy(trigger => new
          {
            DefinitionId = trigger.Definition.Id,
            TriggerType = trigger.Trigger.TriggerType,
            BatchChanges = trigger.Trigger is ContinuousIntegrationTrigger trigger1 && trigger1.BatchChanges,
            AutoCancel = trigger.Trigger is PullRequestTrigger trigger2 ? trigger2.AutoCancel : new bool?(true)
          }).ToList<IGrouping<\u003C\u003Ef__AnonymousType1<int, DefinitionTriggerType, bool, bool?>, TriggerInstance>>())
          {
            int branchCount = 0;
            IEnumerable<BuildDefinitionBranch> definitionBranches = (IEnumerable<BuildDefinitionBranch>) source.Select<TriggerInstance, BuildDefinitionBranch>((Func<TriggerInstance, BuildDefinitionBranch>) (trigger =>
            {
              ++branchCount;
              bool flag = trigger.TriggerRepository == null || trigger.Definition.Repository.Id == trigger.TriggerRepository.Id;
              string str = (string) null;
              if (!flag)
              {
                RepositoryResourceParameters resourceParameters;
                if (trigger.ResourcesParameters.Repositories.TryGetValue(PipelineConstants.SelfAlias, out resourceParameters))
                  str = resourceParameters.RefName;
                if (string.IsNullOrEmpty(str))
                {
                  Guid serviceEndpointId = Guid.Empty;
                  trigger.Definition.Repository.TryGetServiceEndpointId(out serviceEndpointId);
                  try
                  {
                    if (sourceProviderService.GetSourceProvider(requestContext, trigger.Definition.Repository.Type, false).GetUserRepository(requestContext, trigger.Definition.ProjectId, new Guid?(serviceEndpointId), trigger.Definition.Repository.Id) == null)
                      throw new Exception("Failed to get the user respository for repository Id '" + trigger.Definition.Repository.Id + "' and repository name '" + trigger.Definition.Repository.Name + "'.");
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(12030308, nameof (FilteredBuildTriggerHelper), ex);
                    queueExceptions?.Add(ex);
                  }
                }
              }
              BuildDefinitionBranch definitionBranch = new BuildDefinitionBranch()
              {
                BranchName = flag ? (string.IsNullOrEmpty(trigger.RefUpdate.MergeRef) ? trigger.RefUpdate.RefName : trigger.RefUpdate.MergeRef) : str,
                SourceId = trigger.UpdateId,
                PendingSourceOwner = Guid.Parse(trigger.SourceOwner.Id),
                PendingSourceVersion = flag ? trigger.SourceVersion : (string) null
              };
              requestContext.TraceInfo(12030309, nameof (FilteredBuildTriggerHelper), string.Format("Added BuildDefinitionBranch with the following properties. BranchName: {0}, SourceId: {1}, PendingSourceOwner: {2}, PendingSourceVersion: {3}", (object) definitionBranch?.BranchName, (object) definitionBranch?.SourceId, (object) definitionBranch?.PendingSourceOwner, (object) definitionBranch?.PendingSourceVersion));
              definitionBranch.Properties["cisourcebranch"] = trigger.RefUpdate.RefName;
              definitionBranch.Properties["cisourcesha"] = trigger.SourceVersion;
              return definitionBranch;
            })).ToList<BuildDefinitionBranch>();
            BuildDefinition definition = source.First<TriggerInstance>().Definition;
            BuildReason buildReason;
            if (source.Key.BatchChanges)
            {
              buildReason = BuildReason.BatchedCI;
              int concurrentBuildsPerBranch = (source.First<TriggerInstance>().Trigger as ContinuousIntegrationTrigger).MaxConcurrentBuildsPerBranch;
              bool ignoreSourceIdCheck = false;
              if (requestContext.IsFeatureEnabled("Build2.IgnoreSourceIdCheckForBatchTriggers"))
                ignoreSourceIdCheck = string.Equals(definition.Repository.Type, "GitHub", StringComparison.OrdinalIgnoreCase);
              requestContext.TraceInfo(12030147, nameof (FilteredBuildTriggerHelper), string.Format("Calling  UpdateDefinitionBranches for definition name: {0}, branch count: {1}, MaxConcurrentBuildsPerBranch: {2}", (object) definition.Name, (object) (definitionBranches != null ? new int?(definitionBranches.Count<BuildDefinitionBranch>()) : new int?()), (object) concurrentBuildsPerBranch));
              definitionBranches = service1.UpdateDefinitionBranches(requestContext, definition.ProjectId, definition, definitionBranches, concurrentBuildsPerBranch, ignoreSourceIdCheck);
              string format = string.Format("Updated {0} branches for definition id {1}, definition name {2} in project {3}", (object) (definitionBranches != null ? new int?(definitionBranches.Count<BuildDefinitionBranch>()) : new int?()), (object) definition.Id, (object) definition.Name, (object) definition.ProjectId);
              requestContext.TraceInfo(12030147, nameof (FilteredBuildTriggerHelper), format);
            }
            else
              buildReason = source.Key.TriggerType != DefinitionTriggerType.PullRequest ? BuildReason.IndividualCI : BuildReason.PullRequest;
            foreach (BuildDefinitionBranch definitionBranch in definitionBranches)
            {
              BuildData build = new BuildData()
              {
                ProjectId = definition.ProjectId,
                Definition = (MinimalBuildDefinition) definition,
                Priority = QueuePriority.Normal,
                QueueId = definition.Queue?.Id,
                Reason = buildReason,
                RequestedFor = definitionBranch.PendingSourceOwner,
                SourceBranch = definitionBranch.BranchName,
                SourceVersion = definitionBranch.PendingSourceVersion,
                Resources = source.First<TriggerInstance>().ResourcesParameters
              };
              IEnumerable<IBuildRequestValidator> validators = BuildRequestValidatorProvider.GetValidators(new BuildRequestValidationOptions()
              {
                InternalRuntimeVariables = internalRuntimeVariables
              });
              if (additionalBuildSettings != null)
                additionalBuildSettings(build);
              if (source.First<TriggerInstance>().Trigger is PullRequestTrigger trigger && trigger.AutoCancel.HasValue)
                build.TriggerInfo["pr.autoCancel"] = trigger.AutoCancel.ToString().ToLowerInvariant();
              else if (buildReason == BuildReason.IndividualCI)
              {
                BuildRepository triggerRepository = source.First<TriggerInstance>().TriggerRepository;
                if (triggerRepository != null)
                {
                  triggerRepository.FixBuildRepositoryType();
                  string str3;
                  if (definitionBranch.Properties != null && definitionBranch.Properties.TryGetValue("cisourcebranch", out str3))
                    build.TriggerInfo["ci.sourceBranch"] = str3;
                  string str4;
                  if (definitionBranch.Properties != null && definitionBranch.Properties.TryGetValue("cisourcesha", out str4))
                    build.TriggerInfo["ci.sourceSha"] = str4;
                  IBuildSourceProvider sourceProvider = sourceProviderService.GetSourceProvider(requestContext, triggerRepository.Type);
                  string sourceVersion;
                  if (build.TriggerInfo.TryGetValue("ci.sourceSha", out sourceVersion))
                  {
                    string sourceVersionMessage = sourceProvider.GetSourceVersionMessage(requestContext, definition.ProjectId, triggerRepository, sourceVersion);
                    build.TriggerInfo["ci.message"] = FilteredBuildTriggerHelper.TruncateMessageToFirstLine(sourceVersionMessage);
                  }
                  build.TriggerInfo["ci.triggerRepository"] = triggerRepository.Id;
                }
              }
              BuildData buildData = (BuildData) null;
              try
              {
                buildData = service2.QueueBuild(requestContext, build, validators, BuildRequestValidationFlags.QueueFailedBuild | BuildRequestValidationFlags.SkipSourceVersionValidation, callingMethod: nameof (QueueBuildsForRepositoryUpdate), callingFile: "D:\\a\\_work\\1\\s\\Tfs\\Service\\Build2\\Server\\Helpers\\FilteredBuildTriggerHelper.cs");
              }
              catch (Exception ex)
              {
                requestContext.TraceException(12030310, nameof (FilteredBuildTriggerHelper), ex);
                queueExceptions?.Add(ex);
              }
              CustomerIntelligenceData ciData;
              if (buildData != null)
              {
                string str = string.Format("Queued build {0} for definition {1} using branch {2} version {3}", (object) buildData.Id, (object) buildData.Definition?.Id, (object) buildData.SourceBranch, (object) buildData.SourceVersion);
                requestContext.TraceInfo(12030311, nameof (FilteredBuildTriggerHelper), str);
                ciData = new CustomerIntelligenceData((IDictionary<string, object>) FilteredBuildTriggerHelper.GetBuildCIData(buildData, str));
                if (buildData.Reason == BuildReason.BatchedCI)
                  requestContext.TraceInfo(12030311, "TriggeredBuilds", "FilteredBuildTriggerHelper: Received a Batched Continuous Integration trigger for build {0} from definition {1} in project {2}, which has been queued. The most recent commit associated with this build is {3} and the most recent commit associated with the previous successful CI build for this project and branch is {4}", (object) buildData.Id, (object) buildData.Definition?.Id, (object) buildData.ProjectId, (object) buildData.SourceVersion, (object) FilteredBuildTriggerHelper.LastBatchedCIBuildSourceVersion(requestContext, buildData));
                else
                  requestContext.TraceInfo(12030311, "TriggeredBuilds", "FilteredBuildTriggerHelper: Received an Individual Continuous Integration trigger for build {0} from definition {1} in project {2}, which has been queued. The most recent commit associated with this build is {3}", (object) buildData.Id, (object) buildData.Definition?.Id, (object) buildData.ProjectId, (object) buildData.SourceVersion);
                buildDataList.Add(buildData);
              }
              else
              {
                requestContext.TraceInfo(12030311, "TriggeredBuilds", "Received a Continuous Integration trigger for build {0} from definition {1} in project {2}, but the build was unable to be queued at this point.", (object) build.Id, (object) build.Definition?.Id, (object) build.ProjectId);
                string str = string.Format("QueueBuild call for definition {0} using branch {1} version {2} did not actually queue a build", (object) definition.Id, (object) build.SourceBranch, (object) build.SourceVersion);
                requestContext.TraceInfo(12030149, nameof (FilteredBuildTriggerHelper), str);
                ciData = new CustomerIntelligenceData((IDictionary<string, object>) FilteredBuildTriggerHelper.GetBuildCIData(build, str));
                build.Result = new BuildResult?(BuildResult.Failed);
                buildDataList.Add(build);
              }
              FilteredBuildTriggerHelper.PublishCI(requestContext, ciData);
            }
          }
        }
        if (buildDataList.Count > 1)
          requestContext.TraceAlways(12030312, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("QueueBuildsForRepositoryUpdate queued {0} builds.", (object) buildDataList.Count));
        return buildDataList;
      }
    }

    public static bool TryGetFilesChanged(
      IVssRequestContext requestContext,
      IEnumerable<BuildDefinition> buildDefinitions,
      RepositoryUpdateInfo repositoryUpdateInfo,
      RefUpdateInfo refUpdate,
      out List<string> filesChanged)
    {
      buildDefinitions = (IEnumerable<BuildDefinition>) buildDefinitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (d => d != null)).ToArray<BuildDefinition>();
      if (buildDefinitions.Any<BuildDefinition>())
      {
        try
        {
          foreach (BuildDefinition buildDefinition in buildDefinitions)
          {
            if (FilteredBuildTriggerHelper.TryGetFilesChanged(requestContext, buildDefinition, repositoryUpdateInfo, refUpdate, (ExternalGitPullRequest) null, out filesChanged))
            {
              IVssRequestContext requestContext1 = requestContext;
              List<string> stringList = filesChanged;
              // ISSUE: explicit non-virtual call
              string format = string.Format("Retrieved '{0}' changes for refUpdate on branch '{1}' using definition {2}.", (object) (stringList != null ? __nonvirtual (stringList.Count) : 0), (object) refUpdate.RefName, (object) buildDefinition.Name);
              object[] objArray = Array.Empty<object>();
              requestContext1.TraceInfo(12030313, nameof (FilteredBuildTriggerHelper), format, objArray);
              return true;
            }
            requestContext.TraceAlways(12030314, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), "Failed to retrieve changes for refUpdate on branch '" + refUpdate.RefName + "' using definition " + buildDefinition.Name + ".");
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12030315, nameof (FilteredBuildTriggerHelper), ex);
        }
      }
      else
        requestContext.TraceAlways(12030316, TraceLevel.Warning, "Build2", nameof (FilteredBuildTriggerHelper), "Unable to find a valid definition to retrieve changed files for refUpdate on branch '{0}'.", (object) refUpdate.RefName);
      filesChanged = (List<string>) null;
      return false;
    }

    public static bool TryGetFilesChanged(
      IVssRequestContext requestContext,
      BuildDefinition buildDefinition,
      RepositoryUpdateInfo repositoryUpdateInfo,
      RefUpdateInfo refUpdate,
      ExternalGitPullRequest pullRequest,
      out List<string> filesChanged)
    {
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repositoryUpdateInfo.RepositoryType);
      if (requestContext.IsFeatureEnabled("Build2.GetFilesChangedByPullRequest") && pullRequest != null)
        return sourceProvider.TryGetChangedPrFiles(requestContext, buildDefinition, repositoryUpdateInfo.RepositoryId, pullRequest.Number, pullRequest.MergeCommitSha, out filesChanged);
      if (refUpdate.CompleteFilesChangedList != null)
      {
        filesChanged = refUpdate.CompleteFilesChangedList;
        return true;
      }
      string oldObjectId;
      if (Sha1Id.IsNullOrEmpty(refUpdate.OldObjectId))
      {
        IEnumerable<Change> source = !(repositoryUpdateInfo.RepositoryType == "TfsGit") ? sourceProvider.GetChangeHistory(requestContext, buildDefinition, refUpdate.NewObjectId, repositoryUpdateInfo.IncludedChanges.Count + 1) : sourceProvider.GetChangeHistory(requestContext, repositoryUpdateInfo.RepositoryId, refUpdate.NewObjectId, repositoryUpdateInfo.IncludedChanges.Count + 1);
        if (repositoryUpdateInfo.IncludedChanges.Count == 0 || source.Count<Change>() < repositoryUpdateInfo.IncludedChanges.Count + 1)
        {
          filesChanged = new List<string>();
          return true;
        }
        oldObjectId = source.Last<Change>().Id;
      }
      else
        oldObjectId = refUpdate.OldObjectId;
      string newObjectId = Sha1Id.IsNullOrEmpty(refUpdate.MergeObjectId) ? refUpdate.NewObjectId : refUpdate.MergeObjectId;
      Stopwatch stopwatch = Stopwatch.StartNew();
      bool filesChanged1 = sourceProvider.TryGetFilesChanged(requestContext, buildDefinition, repositoryUpdateInfo.RepositoryId, oldObjectId, newObjectId, out filesChanged);
      long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      if (elapsedMilliseconds > 120000L)
        requestContext.TraceAlways(12030265, TraceLevel.Warning, nameof (FilteredBuildTriggerHelper), nameof (TryGetFilesChanged), string.Format("Get files changed for repository '{0}' with shartSha '{1}' and endSha '{2}', getFilesChangedtimeTaken: {3}", (object) repositoryUpdateInfo.RepositoryId, (object) oldObjectId, (object) newObjectId, (object) elapsedMilliseconds));
      if (filesChanged1)
        refUpdate.CompleteFilesChangedList = filesChanged;
      return filesChanged1;
    }

    public static Dictionary<string, List<BuildDefinition>> GetYamlFileToDefinitionMap(
      IVssRequestContext requestContext,
      List<BuildDefinition> yamlDefinitions)
    {
      Dictionary<string, List<BuildDefinition>> fileToDefinitionMap = new Dictionary<string, List<BuildDefinition>>();
      if (yamlDefinitions == null)
      {
        requestContext.TraceAlways(12030270, TraceLevel.Error, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("Request with Activity Id: {0} is passing null for yamlDefinitions.", (object) requestContext.ActivityId));
      }
      else
      {
        foreach (BuildDefinition definition in yamlDefinitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (definition => definition != null)))
        {
          YamlProcess process = definition.GetProcess<YamlProcess>();
          if (process == null)
          {
            requestContext.TraceAlways(12030270, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("Definition '{0}' (Id: {1}) provided is not YAML based", (object) definition.Name, (object) definition.Id));
          }
          else
          {
            string key = process.YamlFilename;
            if (key == null)
            {
              requestContext.TraceAlways(12030270, TraceLevel.Error, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("Definition '{0}:' (Id: {1}) provided does not have a valid YAML file name", (object) definition.Name, (object) definition.Id));
            }
            else
            {
              if (key.StartsWith("./"))
                key = key.Substring(1);
              else if (!key.StartsWith("/"))
                key = "/" + key;
              List<BuildDefinition> buildDefinitionList = (List<BuildDefinition>) null;
              if (fileToDefinitionMap.TryGetValue(key, out buildDefinitionList))
                buildDefinitionList.Add(definition);
              else
                fileToDefinitionMap.Add(key, new List<BuildDefinition>()
                {
                  definition
                });
            }
          }
        }
      }
      requestContext.TraceInfo(12030270, nameof (FilteredBuildTriggerHelper), string.Format("Discovered '{0}' YAML files to check for updates in the supplied refUpdates for '{1}' definitions.", (object) fileToDefinitionMap.Count, (object) yamlDefinitions?.Count));
      return fileToDefinitionMap;
    }

    public static Dictionary<BuildDefinition, List<BuildRepository>> GetDefinitionRepositoryMappings(
      IVssRequestContext requestContext,
      Dictionary<string, List<BuildDefinition>> yamlFileToDefinitionsMap,
      RepositoryUpdateInfo repositoryUpdateInfo,
      bool manualRun = false,
      bool throwOnError = true)
    {
      using (requestContext.TraceSlowCall(nameof (FilteredBuildTriggerHelper), 120000, new Lazy<string>((Func<string>) (() => string.Format("RepoId={0}, RefUpdates.Count={1}", (object) repositoryUpdateInfo?.RepositoryId, (object) repositoryUpdateInfo?.RefUpdates?.Count))), nameof (GetDefinitionRepositoryMappings)))
      {
        Dictionary<BuildDefinition, List<BuildRepository>> definitionsToRepo = new Dictionary<BuildDefinition, List<BuildRepository>>();
        if (!manualRun)
        {
          ArgumentUtility.CheckForNull<RepositoryUpdateInfo>(repositoryUpdateInfo, nameof (repositoryUpdateInfo));
          BuildRepository buildRepository1 = new BuildRepository();
          buildRepository1.Id = repositoryUpdateInfo.RepositoryId;
          buildRepository1.Type = repositoryUpdateInfo.RepositoryType;
          BuildRepository buildRepository2 = buildRepository1;
          buildRepository2.FixBuildRepositoryType();
          IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, buildRepository2.Type);
          string a = (string) null;
          List<BuildDefinition> list = yamlFileToDefinitionsMap.SelectMany<KeyValuePair<string, List<BuildDefinition>>, BuildDefinition>((Func<KeyValuePair<string, List<BuildDefinition>>, IEnumerable<BuildDefinition>>) (pair => (IEnumerable<BuildDefinition>) pair.Value)).Where<BuildDefinition>((Func<BuildDefinition, bool>) (d => d?.Repository != null)).ToList<BuildDefinition>();
          for (int index = 0; index < list.Count; ++index)
          {
            try
            {
              BuildDefinition definition = list[index];
              Guid serviceEndpointId = Guid.Empty;
              if (definition.Repository.Type == "TfsGit" || definition.Repository.TryGetServiceEndpointId(out serviceEndpointId))
              {
                SourceRepository userRepository = sourceProvider.GetUserRepository(requestContext, definition.ProjectId, new Guid?(serviceEndpointId), definition.Repository.Id);
                if (userRepository == null)
                {
                  requestContext.TraceAlways(12030308, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("Failed to retrieve source repository. Project id: '{0}' Repository id: '{1}'.", (object) definition.ProjectId, (object) definition.Repository.Id));
                  break;
                }
                a = sourceProvider.NormalizeSourceBranch(userRepository.DefaultBranch, definition);
                if (a == null)
                {
                  requestContext.TraceAlways(12030317, TraceLevel.Warning, "Build2", nameof (FilteredBuildTriggerHelper), "Failed to retrieve defaultBranch for repository '" + userRepository.Name + "' using definition " + definition.Name + ".");
                }
                else
                {
                  requestContext.TraceInfo(12030318, nameof (FilteredBuildTriggerHelper), "Retrieved defaultBranch for repository '" + userRepository.Name + "' using definition " + definition.Name + ".");
                  break;
                }
              }
              else
                requestContext.TraceAlways(12030319, TraceLevel.Error, "Build2", nameof (FilteredBuildTriggerHelper), "Failed to get Service Endpoint ID for repository " + definition.Repository.Name + " of build definition " + definition.Name + ".");
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12030320, nameof (FilteredBuildTriggerHelper), ex);
            }
          }
          foreach (RefUpdateInfo refUpdate in repositoryUpdateInfo.RefUpdates)
          {
            string refName = refUpdate.RefName;
            if (string.IsNullOrEmpty(refName))
              requestContext.TraceAlways(12030321, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), "refUpdate branch name is null or empty. RepoId=" + repositoryUpdateInfo.RepositoryId + ".");
            else if (!refName.StartsWith("refs/heads/", StringComparison.Ordinal))
            {
              requestContext.TraceAlways(12030322, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), "RefUpdate branch name " + refName + " does not start with refs/heads. Repo=" + repositoryUpdateInfo.RepositoryId);
            }
            else
            {
              List<string> filesChanged = (List<string>) null;
              if (string.Equals(a, refName, StringComparison.Ordinal))
              {
                if (FilteredBuildTriggerHelper.TryGetFilesChanged(requestContext, yamlFileToDefinitionsMap.SelectMany<KeyValuePair<string, List<BuildDefinition>>, BuildDefinition>((Func<KeyValuePair<string, List<BuildDefinition>>, IEnumerable<BuildDefinition>>) (pair => (IEnumerable<BuildDefinition>) pair.Value)), repositoryUpdateInfo, refUpdate, out filesChanged) && filesChanged.Count > 0)
                  FilteredBuildTriggerHelper.PopulateDefinitionsToRepo(requestContext, yamlFileToDefinitionsMap, definitionsToRepo, refName, buildRepository2, filesChanged);
              }
              else
                requestContext.TraceInfo(12030323, nameof (FilteredBuildTriggerHelper), "RefUpdate branch name " + refName + " is not the default branch for repository " + repositoryUpdateInfo.RepositoryId + ". Skipping PopulateDefinitionsToRepo invocation.");
            }
          }
        }
        else
        {
          BuildDefinition buildDefinition = yamlFileToDefinitionsMap.SelectMany<KeyValuePair<string, List<BuildDefinition>>, BuildDefinition>((Func<KeyValuePair<string, List<BuildDefinition>>, IEnumerable<BuildDefinition>>) (pair => (IEnumerable<BuildDefinition>) pair.Value)).FirstOrDefault<BuildDefinition>((Func<BuildDefinition, bool>) (d => d?.Repository?.DefaultBranch != null));
          if (buildDefinition != null)
            FilteredBuildTriggerHelper.PopulateDefinitionsToRepo(requestContext, yamlFileToDefinitionsMap, definitionsToRepo, buildDefinition.Repository.DefaultBranch, (BuildRepository) null, (List<string>) null, manualRun, throwOnError);
        }
        return definitionsToRepo;
      }
    }

    internal static void PopulateDefinitionsToRepo(
      IVssRequestContext requestContext,
      Dictionary<string, List<BuildDefinition>> yamlFileToDefinitionsMap,
      Dictionary<BuildDefinition, List<BuildRepository>> definitionsToRepo,
      string branchName,
      BuildRepository updatedBuildRepository,
      List<string> filesChanged,
      bool manual = false,
      bool throwOnError = true)
    {
      using (requestContext.TraceSlowCall(nameof (FilteredBuildTriggerHelper), 120000, new Lazy<string>((Func<string>) (() => string.Format("branchName={0}, yamlFileToDefnitionMap.Count:{1}", (object) branchName, (object) yamlFileToDefinitionsMap?.Count))), nameof (PopulateDefinitionsToRepo)))
      {
        if (yamlFileToDefinitionsMap != null)
        {
          foreach (KeyValuePair<string, List<BuildDefinition>> fileToDefinitions in yamlFileToDefinitionsMap)
          {
            string key = fileToDefinitions.Key;
            if (manual || filesChanged != null && filesChanged.Contains(key))
            {
              foreach (BuildDefinition buildDefinition in fileToDefinitions.Value.Where<BuildDefinition>((Func<BuildDefinition, bool>) (buildDef => buildDef != null)))
              {
                if (!definitionsToRepo.ContainsKey(buildDefinition))
                {
                  YamlProcess process = buildDefinition.GetProcess<YamlProcess>();
                  if (process != null)
                  {
                    List<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource> repositoriesFromYaml = FilteredBuildTriggerHelper.ParseRepositoriesFromYaml(requestContext, buildDefinition, branchName, process, key);
                    List<BuildRepository> source = new List<BuildRepository>();
                    foreach (Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource repositoryResource in repositoriesFromYaml)
                    {
                      BuildRepository buildRepository = new BuildRepository();
                      buildRepository.Id = repositoryResource.Id;
                      buildRepository.Type = repositoryResource.Type;
                      buildRepository.Name = repositoryResource.Name;
                      BuildRepository repository = buildRepository;
                      if (repositoryResource.Endpoint != null && repositoryResource.Endpoint.Id != Guid.Empty)
                        repository.Properties.Add("connectedServiceId", repositoryResource.Endpoint.Id.ToString());
                      if (repositoryResource.Trigger != null)
                        repository.Properties.Add("hasCITrigger", "true");
                      if (repositoryResource.PR != null)
                        repository.Properties.Add("hasPRTrigger", "true");
                      repository.FixBuildRepositoryType();
                      if (!requestContext.IsFeatureEnabled("Build2.SkipRemoteAzureReposInPopulateDefinitionsToRepo") || repositoryResource.Endpoint == null || !string.Equals(repository.Type, "TfsGit", StringComparison.OrdinalIgnoreCase))
                      {
                        source.Add(repository);
                        IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repository.Type);
                        try
                        {
                          SourceRepository userRepository = sourceProvider.GetUserRepository(requestContext, buildDefinition.ProjectId, repositoryResource.Endpoint?.Id, repositoryResource.Id);
                          string str;
                          if (userRepository != null && userRepository.Properties.TryGetValue("apiUrl", out str))
                            repository.Properties.Add("apiUrl", str);
                          sourceProvider.SetRepositoryDefaultInfo(requestContext, buildDefinition.ProjectId, repository);
                        }
                        catch (Exception ex)
                        {
                          requestContext.TraceException(12030324, TraceLevel.Error, "Build2", nameof (FilteredBuildTriggerHelper), ex, "Exception in PopulateDefinitionsToRepo for definition id '{0}' and repository name '{1}'\r\n{2}", (object) buildDefinition.Id, (object) repositoryResource.Name, (object) ex);
                          if (throwOnError)
                            throw;
                        }
                      }
                    }
                    if (source.Contains(updatedBuildRepository))
                      source.Add(updatedBuildRepository);
                    if (source.Any<BuildRepository>())
                      definitionsToRepo.Add(buildDefinition, source);
                    requestContext.TraceInfo(12030325, nameof (FilteredBuildTriggerHelper), "'{0}' repositories have been found to be added for definition '{1}' in branch '{2}'.", (object) repositoriesFromYaml.Count, (object) buildDefinition.Name, (object) branchName);
                  }
                }
                else
                  requestContext.TraceInfo(12030326, nameof (FilteredBuildTriggerHelper), "An update to the YAML based repositories for definition '" + buildDefinition.Name + "' on branch '" + branchName + "' has already been encountered. Since we read from current version, ignore this commit and continue looking through the list.");
              }
            }
          }
        }
        else
          requestContext.TraceInfo(12030327, nameof (FilteredBuildTriggerHelper), "yamlFileToDefinitionMap is null on branch '" + branchName + "'.");
      }
    }

    internal static List<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource> ParseRepositoriesFromYaml(
      IVssRequestContext requestContext,
      BuildDefinition yamlDefinition,
      string branchName,
      YamlProcess yamlProcess,
      string yamlFile)
    {
      using (requestContext.TraceSlowCall(nameof (FilteredBuildTriggerHelper), 120000, new Lazy<string>((Func<string>) (() => yamlFile)), nameof (ParseRepositoriesFromYaml)))
      {
        List<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource> repositoriesFromYaml = new List<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>();
        requestContext.GetService<IYamlPipelineLoaderService>();
        YamlPipelineLoadResult pipelineLoadResult;
        try
        {
          pipelineLoadResult = yamlDefinition.LoadYamlPipeline(requestContext, false);
          requestContext.TraceInfo(1030271, nameof (FilteredBuildTriggerHelper), "Attempting to load resources from YAML file '" + yamlFile + "' for Definition '" + yamlDefinition.Name + "' on branch '" + branchName + "'.");
        }
        catch (YamlFileNotFoundException ex)
        {
          requestContext.TraceAlways(1030271, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), "Yaml file '" + yamlFile + "' does not exist for project '" + yamlDefinition.ProjectName + "', repo '" + yamlDefinition.Repository.Name + "', ref '" + branchName + "'. It must have been deleted. Sending along data to remove repositories for definition '" + yamlDefinition.Name + "' on branch '" + branchName + "'.");
          return repositoriesFromYaml;
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(1030271, TraceLevel.Error, "Build2", nameof (FilteredBuildTriggerHelper), "Exception '" + ex.ToString() + "' encountered when trying to Load Yaml file '" + yamlFile + "' for project '" + yamlDefinition.ProjectName + "', repo '" + yamlDefinition.Repository.Name + "', on branch '" + branchName + "' and definition '" + yamlDefinition.Name + "'");
          return repositoriesFromYaml;
        }
        if (pipelineLoadResult == null || pipelineLoadResult.Template.Errors.Count > 0)
        {
          requestContext.TraceAlways(1030271, TraceLevel.Error, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("'{0}' Load errors were encountered when trying to Load Yaml file '{1}' for project '{2}', repo '{3}', on branch '{4}' and definition '{5}'", (object) pipelineLoadResult.Template.Errors.Count, (object) yamlFile, (object) yamlDefinition.ProjectName, (object) yamlDefinition.Repository.Name, (object) branchName, (object) yamlDefinition.Name));
          foreach (PipelineValidationError error in (IEnumerable<PipelineValidationError>) pipelineLoadResult.Template.Errors)
            requestContext.TraceError(1030271, nameof (FilteredBuildTriggerHelper), "Error while trying to Load Yaml file: " + error.Message);
          repositoriesFromYaml.AddRange((IEnumerable<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>) pipelineLoadResult.Template.Resources.Repositories);
          return repositoriesFromYaml;
        }
        requestContext.TraceInfo(1030271, nameof (FilteredBuildTriggerHelper), "YAML file '" + yamlFile + "' for definition '" + yamlDefinition.Name + "' on branch '" + branchName + "' successfully parsed, filtering results and adding them to the list to use for update.");
        repositoriesFromYaml.AddRange((IEnumerable<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource>) pipelineLoadResult.Template.Resources.Repositories);
        return repositoriesFromYaml;
      }
    }

    private static List<TriggerInstance> GetBranchUpdates(
      IVssRequestContext requestContext,
      List<BuildDefinition> definitions,
      RepositoryUpdateInfo repositoryUpdateInfo,
      IdentityRef sourceOwner,
      List<TriggerLoadError> triggerLoadErrors,
      DefinitionTriggerType triggerType,
      IExternalGitEvent gitEvent)
    {
      using (requestContext.TraceSlowCall(nameof (FilteredBuildTriggerHelper), 120000, new Lazy<string>((Func<string>) (() => string.Format("TriggerType={0};PipelineEventId={1}", (object) triggerType, (object) gitEvent?.PipelineEventId))), nameof (GetBranchUpdates)))
      {
        if (repositoryUpdateInfo.RefUpdates.Count == 0)
        {
          PipelineEventLogger.IgnoreEvent(requestContext, gitEvent, "No ref updates are present in the git notification");
          return new List<TriggerInstance>(0);
        }
        if (!definitions.Any<BuildDefinition>())
        {
          PipelineEventLogger.IgnoreEvent(requestContext, gitEvent, "No definitions are available for the repository for which the git event was created");
          return new List<TriggerInstance>(0);
        }
        List<BuildDefinition> definitions1 = new List<BuildDefinition>();
        definitions1.AddRange((IEnumerable<BuildDefinition>) definitions);
        bool filterOutDefinitionsReferencingBitBucketRepos = requestContext.IsFeatureEnabled("Build2.MultiRepoPushTriggers.FilterOutDefinitionsReferencingBitBucketRepos");
        bool filterOutDefinitionsReferencingGithubRepos = requestContext.IsFeatureEnabled("Build2.MultiRepoPushTriggers.FilterOutDefinitionsReferencingGithubRepos");
        definitions1.RemoveAll((Predicate<BuildDefinition>) (def =>
        {
          BuildProcess process = def.Process;
          bool flag = process == null || process.Type != 2;
          if (filterOutDefinitionsReferencingBitBucketRepos && repositoryUpdateInfo.IsBitBucketRepository())
            flag = true;
          if (filterOutDefinitionsReferencingGithubRepos && repositoryUpdateInfo.IsGithubRepository())
            flag = true;
          return flag && !def.RepositoryMatchesUpdateInfo(requestContext, repositoryUpdateInfo);
        }));
        requestContext.TraceAlways(12030346, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), new
        {
          Msg = string.Format("Filtered out {0} of total {1} candidate definitions", (object) (definitions.Count - definitions1.Count), (object) definitions.Count),
          RemainingDefinitions = definitions.Select(def => new
          {
            RepoName = def?.Repository?.Name,
            DefinitionName = def?.Name,
            DefinitionId = def?.Id
          }).Serialize<IEnumerable<\u003C\u003Ef__AnonymousType3<string, string, int?>>>()
        }.Serialize());
        List<TriggerInstance> source = new List<TriggerInstance>();
        foreach (RefUpdateInfo refUpdate in repositoryUpdateInfo.RefUpdates)
        {
          if (Sha1Id.IsNullOrEmpty(refUpdate.NewObjectId))
          {
            requestContext.TraceInfo(12030065, nameof (FilteredBuildTriggerHelper), "Ref {0} is being unpublished. Ignoring update.", (object) refUpdate.RefName);
          }
          else
          {
            requestContext.TraceInfo(12030065, nameof (FilteredBuildTriggerHelper), "Processing push id {0}, ref {1}, from {2} to {3}", (object) repositoryUpdateInfo.UpdateId, (object) refUpdate.RefName, (object) refUpdate.OldObjectId, (object) refUpdate.NewObjectId);
            string sourceVersion;
            List<TriggerInstance> triggerInstanceList1 = FilteredBuildTriggerHelper.FilterBranches(requestContext, triggerType, definitions1, repositoryUpdateInfo, sourceOwner, refUpdate, triggerLoadErrors, gitEvent, out sourceVersion);
            IVssRequestContext requestContext1 = requestContext;
            int? nullable1;
            int? nullable2;
            if (triggerInstanceList1 == null)
            {
              nullable1 = new int?();
              nullable2 = nullable1;
            }
            else
            {
              // ISSUE: explicit non-virtual call
              nullable2 = new int?(__nonvirtual (triggerInstanceList1.Count));
            }
            string format1 = string.Format("Retrieved {0} matching triggers for the ref update on the branch filters", (object) nullable2);
            object[] objArray1 = Array.Empty<object>();
            requestContext1.TraceInfo(12030065, nameof (FilteredBuildTriggerHelper), format1, objArray1);
            // ISSUE: explicit non-virtual call
            if (triggerInstanceList1 != null && __nonvirtual (triggerInstanceList1.Count) > 0)
            {
              List<TriggerInstance> triggerInstanceList2 = FilteredBuildTriggerHelper.FilterPaths(requestContext, repositoryUpdateInfo, refUpdate, triggerLoadErrors, triggerInstanceList1, gitEvent as ExternalGitPullRequest);
              IVssRequestContext requestContext2 = requestContext;
              int? nullable3;
              if (triggerInstanceList2 == null)
              {
                nullable1 = new int?();
                nullable3 = nullable1;
              }
              else
              {
                // ISSUE: explicit non-virtual call
                nullable3 = new int?(__nonvirtual (triggerInstanceList2.Count));
              }
              string format2 = string.Format("Retrieved {0} matching triggers for the ref update on the path filters", (object) nullable3);
              object[] objArray2 = Array.Empty<object>();
              requestContext2.TraceInfo(12030065, nameof (FilteredBuildTriggerHelper), format2, objArray2);
              // ISSUE: explicit non-virtual call
              if (triggerInstanceList2 != null && __nonvirtual (triggerInstanceList2.Count) > 0)
                source.AddRange((IEnumerable<TriggerInstance>) triggerInstanceList2);
              if (triggerType == DefinitionTriggerType.PullRequest && triggerInstanceList1 != null && triggerInstanceList2 != null)
              {
                nullable1 = triggerInstanceList1?.Count;
                int? count = triggerInstanceList2?.Count;
                if (!(nullable1.GetValueOrDefault() == count.GetValueOrDefault() & nullable1.HasValue == count.HasValue))
                  FilteredBuildTriggerHelper.PostNeutralStatusesForSkippedChange(requestContext, triggerInstanceList1.Except<TriggerInstance>((IEnumerable<TriggerInstance>) triggerInstanceList2).Select<TriggerInstance, BuildDefinition>((Func<TriggerInstance, BuildDefinition>) (x => x.Definition)), BuildServerResources.GitHubCheckSkippedNoMatchingPathFilter(), Sha1Id.ValidateSha(refUpdate.NewObjectId, sourceVersion), gitEvent);
              }
            }
          }
        }
        requestContext.TraceAlways(12030347, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), new
        {
          Msg = string.Format("GetBranchUpdates found total {0} matching triggers. There were {1} load errors.", (object) source.Count, (object) triggerLoadErrors.Count),
          Triggers = source.Select(trigger => new
          {
            DefinitionId = trigger?.Definition?.Id,
            DefinitionName = trigger?.Definition?.Name,
            RepoName = trigger?.TriggerRepository?.Name,
            RefName = trigger?.RefUpdate?.RefName,
            BranchFilters = trigger?.Trigger?.BranchFilters != null ? trigger?.Trigger?.BranchFilters.Serialize<List<string>>() : "",
            PathFilters = trigger?.Trigger?.PathFilters != null ? trigger?.Trigger?.PathFilters.Serialize<List<string>>() : ""
          }).Serialize<IEnumerable<\u003C\u003Ef__AnonymousType5<int?, string, string, string, string, string>>>()
        }.Serialize());
        return source;
      }
    }

    private static List<TriggerInstance> FilterBranches(
      IVssRequestContext requestContext,
      DefinitionTriggerType triggerType,
      List<BuildDefinition> definitions,
      RepositoryUpdateInfo repositoryUpdateInfo,
      IdentityRef sourceOwner,
      RefUpdateInfo refUpdate,
      List<TriggerLoadError> triggerLoadErrors,
      IExternalGitEvent gitEvent,
      out string sourceVersion)
    {
      using (requestContext.TraceSlowCall(nameof (FilteredBuildTriggerHelper), 120000, new Lazy<string>((Func<string>) (() => string.Format("definitions.Count={0}", (object) definitions.Count))), nameof (FilterBranches)))
      {
        List<TriggerInstance> matchingTriggers = new List<TriggerInstance>();
        List<BuildDefinition> definitions1 = new List<BuildDefinition>();
        sourceVersion = (string) null;
        List<BuildDefinition> definitions2 = new List<BuildDefinition>();
        List<BuildDefinition> definitions3 = new List<BuildDefinition>();
        foreach (BuildDefinition definition in definitions)
        {
          bool flag1 = false;
          bool flag2 = false;
          foreach (FilteredBuildTrigger trigger in definition.Triggers.OfType<FilteredBuildTrigger>())
          {
            if (trigger.TriggerType == triggerType)
            {
              if (trigger is PullRequestTrigger prTrigger && !FilteredBuildTriggerHelper.IsValidPullRequestTrigger(requestContext, prTrigger, gitEvent as ExternalGitPullRequest, definition))
              {
                flag2 = true;
                flag1 = true;
              }
              else if (trigger.SettingsSourceType == 1)
              {
                if (string.Equals(definition.Repository.Id, repositoryUpdateInfo.RepositoryId, StringComparison.OrdinalIgnoreCase) && string.Equals(definition.Repository.Type, repositoryUpdateInfo.RepositoryType, StringComparison.OrdinalIgnoreCase) && FilteredBuildTriggerHelper.IsMatchingBranch(requestContext, refUpdate, definition, trigger))
                {
                  if (sourceVersion == null)
                  {
                    try
                    {
                      sourceVersion = FilteredBuildTriggerHelper.GetSourceVersion(requestContext, triggerType, definition, repositoryUpdateInfo, refUpdate);
                    }
                    catch (Exception ex)
                    {
                      PipelineEventLogger.LogException(requestContext, gitEvent, ex);
                      requestContext.TraceAlways(12030328, TraceLevel.Error, "Build2", "QueueFailedBuild", string.Format("Queuing a failed build for definition id {0}, definition name {1} due to the following error: {2}", (object) definition?.Id, (object) definition?.Name, (object) ex));
                      triggerLoadErrors.Add(new TriggerLoadError(definition, repositoryUpdateInfo.UpdateId, refUpdate, sourceOwner, trigger.TriggerType, ex));
                      continue;
                    }
                  }
                  matchingTriggers.Add(new TriggerInstance(definition, trigger, repositoryUpdateInfo.UpdateId, refUpdate, sourceOwner, sourceVersion));
                  flag1 = true;
                  flag2 = true;
                }
              }
              else if (trigger.SettingsSourceType == 2)
              {
                BuildProcess process = definition.Process;
                if ((process != null ? (process.Type == 2 ? 1 : 0) : 0) != 0)
                {
                  if (sourceVersion == null)
                  {
                    try
                    {
                      sourceVersion = FilteredBuildTriggerHelper.GetSourceVersion(requestContext, triggerType, definition, repositoryUpdateInfo, refUpdate);
                    }
                    catch (Exception ex)
                    {
                      PipelineEventLogger.LogException(requestContext, gitEvent, ex);
                      requestContext.TraceAlways(12030328, TraceLevel.Error, "Build2", "QueueFailedBuild", string.Format("Queuing a failed build for definition id {0}, definition name {1} due to the following error: {2}", (object) definition?.Id, (object) definition?.Name, (object) ex));
                      triggerLoadErrors.Add(new TriggerLoadError(definition, repositoryUpdateInfo.UpdateId, refUpdate, sourceOwner, triggerType, ex));
                      continue;
                    }
                  }
                  definitions1.Add(definition);
                  flag2 = true;
                  flag1 = true;
                }
              }
            }
          }
          if (triggerType == DefinitionTriggerType.PullRequest)
          {
            if (!flag2)
              definitions3.Add(definition);
            else if (!flag1)
              definitions2.Add(definition);
          }
        }
        FilteredBuildTriggerHelper.PostNeutralStatusesForSkippedChange(requestContext, (IEnumerable<BuildDefinition>) definitions3, BuildServerResources.GitHubCheckSkippedNoMatchingBranchFilter(), refUpdate.NewObjectId, gitEvent);
        FilteredBuildTriggerHelper.PostNeutralStatusesForSkippedChange(requestContext, (IEnumerable<BuildDefinition>) definitions2, BuildServerResources.GitHubCheckSkippedNoMatchingTriggers(), refUpdate.NewObjectId, gitEvent);
        if (sourceVersion == null)
          sourceVersion = Sha1Id.IsNullOrEmpty(refUpdate.MergeObjectId) ? refUpdate.NewObjectId : refUpdate.MergeObjectId;
        if (matchingTriggers.Count > 0 || definitions1.Count > 0)
        {
          IBuildSourceProviderService service = requestContext.GetService<IBuildSourceProviderService>();
          string str = matchingTriggers.Count > 0 ? matchingTriggers[0].Definition.Repository.Type : definitions1[0].Repository.Type;
          IVssRequestContext requestContext1 = requestContext;
          string repositoryType = str;
          IBuildSourceProvider sourceProvider = service.GetSourceProvider(requestContext1, repositoryType);
          bool hasNoCICommits;
          sourceProvider.GetNoCICommits(requestContext, repositoryUpdateInfo.RepositoryId, (IEnumerable<Change>) repositoryUpdateInfo.IncludedChanges, sourceVersion, new List<Change>(), definitions1, out hasNoCICommits);
          if (hasNoCICommits)
          {
            requestContext.TraceAlways(12030329, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), "hasNoCICommits is true in FilterBranches. Returning null. refUpdate.RefName=" + refUpdate?.RefName);
            FilteredBuildTriggerHelper.PostNeutralStatusesForSkippedChange(requestContext, (IEnumerable<BuildDefinition>) definitions1, BuildServerResources.GitHubCheckSkippedCommentHasSkipCIKeyword(), Sha1Id.ValidateSha(refUpdate.NewObjectId, sourceVersion), gitEvent);
            return (List<TriggerInstance>) null;
          }
          if (definitions1.Count > 0)
            YamlTriggerHelper.LoadYamlTriggers(requestContext, triggerType, sourceProvider, repositoryUpdateInfo, refUpdate, sourceOwner, sourceVersion, gitEvent, definitions1, matchingTriggers, triggerLoadErrors);
        }
        return matchingTriggers;
      }
    }

    internal static void PostNeutralStatusesForSkippedChange(
      IVssRequestContext requestContext,
      IEnumerable<BuildDefinition> definitions,
      string reasonSkipped,
      string sourceVersion,
      IExternalGitEvent gitEvent)
    {
      try
      {
        if (definitions == null || !definitions.Any<BuildDefinition>())
          return;
        definitions.Where<BuildDefinition>((Func<BuildDefinition, bool>) (d => d.Repository?.Type != null)).GroupBy<BuildDefinition, string, BuildDefinition>((Func<BuildDefinition, string>) (k => k.Repository?.Type), (Func<BuildDefinition, BuildDefinition>) (v => v)).ForEach<IGrouping<string, BuildDefinition>>((Action<IGrouping<string, BuildDefinition>>) (x =>
        {
          requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, x.Key).PostMultipleStatusesForSkippedChanges(requestContext, x.ToList<BuildDefinition>(), reasonSkipped, "neutral", sourceVersion);
          x.ForEach<BuildDefinition>((Action<BuildDefinition>) (d => PipelineEventLogger.Log(requestContext, PipelineEventType.BuildStatus, gitEvent, (IDictionary<string, string>) new Dictionary<string, string>()
          {
            [PipelineEventProperties.BuildResult] = PipelineEventConstants.NoBuild,
            [PipelineEventProperties.Conclusion] = reasonSkipped,
            [PipelineEventProperties.DefinitionName] = d.Name
          })));
        }));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030334, nameof (FilteredBuildTriggerHelper), ex);
      }
    }

    private static bool IsValidPullRequestTrigger(
      IVssRequestContext requestContext,
      PullRequestTrigger prTrigger,
      ExternalGitPullRequest pullRequest,
      BuildDefinition definition)
    {
      if (pullRequest == null)
      {
        requestContext.TraceInfo(nameof (FilteredBuildTriggerHelper), "No pull request information was found for build definition {0} for repository {1} so no build was triggered.", (object) definition.Name, (object) definition.Repository.Name);
        return false;
      }
      ProjectPipelineGeneralSettingsHelper settingsHelper = new ProjectPipelineGeneralSettingsHelper(requestContext, definition.ProjectId, true);
      bool isGitHubRepository = string.Equals(definition.Repository.Type, "GitHub", StringComparison.OrdinalIgnoreCase);
      PipelineTriggerSettings pullRequestTrigger = settingsHelper.GetEffectiveSettingsOfPullRequestTrigger(requestContext, prTrigger, isGitHubRepository);
      if (pullRequest.IsFork && !pullRequestTrigger.BuildsEnabledForForks)
      {
        requestContext.TraceInfo(nameof (FilteredBuildTriggerHelper), (!prTrigger.Forks.Enabled ? "Build definition {0} settings" : "Organization {2} or project {3} settings") + " does not support building forks for repository {1}", (object) definition.Name, (object) definition.Repository.Name, (object) requestContext.ServiceHost.Name, (object) definition.ProjectName);
        return false;
      }
      if (!pullRequest.IsFromComment && pullRequestTrigger.IsCommentRequiredForPullRequest)
      {
        if ((pullRequestTrigger.RequireCommentsForNonTeamMembersOnly ? 0 : (!pullRequestTrigger.RequireCommentsForNonTeamMemberAndNonContributors ? 1 : 0)) != 0)
        {
          requestContext.TraceInfo(nameof (FilteredBuildTriggerHelper), "Build definition {0} needs to be triggered by a PR comment for all users for repository {1} as it is set to required in " + (prTrigger.RequireCommentsForNonTeamMembersOnly || prTrigger.RequireCommentsForNonTeamMemberAndNonContributors ? "organization {2} or project {3} settings." : "build definition settings."), (object) definition.Name, (object) definition.Repository.Name, (object) requestContext.ServiceHost.Name, (object) definition.ProjectName);
          return false;
        }
        if (pullRequestTrigger.RequireCommentsForNonTeamMembersOnly)
        {
          requestContext.TraceInfo(nameof (FilteredBuildTriggerHelper), "Build definition {0} needs to be triggered by a PR comment for non team members for repository {1} as it is set to required in " + (prTrigger.RequireCommentsForNonTeamMembersOnly ? "build definition settings" : "organization {2} or project {3} settings.") + "PR author permissions are: {4}.", (object) definition.Name, (object) definition.Repository.Name, (object) requestContext.ServiceHost.Name, (object) definition.ProjectName, (object) pullRequest.DoesAuthorHaveWriteAccess);
          return pullRequest.DoesAuthorHaveWriteAccess;
        }
        if (!requestContext.IsFeatureEnabled("Build2.GitHubRequireCommentsForNonTeamAndNonContributors") || !pullRequestTrigger.RequireCommentsForNonTeamMemberAndNonContributors)
          return pullRequest.DoesAuthorHaveWriteAccess;
        requestContext.TraceInfo(nameof (FilteredBuildTriggerHelper), "Build definition {0} needs to be triggered by a PR comment for non team members and non contributors for repository {1} as it is set to required in " + (prTrigger.RequireCommentsForNonTeamMemberAndNonContributors ? "build definition settings" : "organization {2} or project {3} settings.") + "PR author association are: {4}. PR author permissions are: {5}", (object) definition.Name, (object) definition.Repository.Name, (object) requestContext.ServiceHost.Name, (object) definition.ProjectName, (object) pullRequest.AuthorAssociation, (object) pullRequest.DoesAuthorHaveWriteAccess);
        return pullRequest.DoesAuthorHaveWriteAccess || pullRequest.AuthorIsRepoContributor;
      }
      if (requestContext.IsTracing(12030375, TraceLevel.Verbose, "Build2", nameof (FilteredBuildTriggerHelper)))
      {
        string str;
        definition.Repository.Properties.TryGetValue("isFork", out str);
        if (str == "True")
        {
          ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, definition.ProjectId);
          TracepointUtils.Tracepoint(requestContext, 12030375, "Build2", nameof (FilteredBuildTriggerHelper), (Func<object>) (() => (object) new
          {
            PrId = pullRequest.Id,
            RepoName = pullRequest.Repo.Name,
            SourceBranch = pullRequest.SourceRef,
            TargetBranch = pullRequest.TargetRef,
            AuthorAssociation = pullRequest.AuthorAssociation,
            IsFromComment = pullRequest.IsFromComment,
            IsForksEnabled = prTrigger.Forks.Enabled,
            IsFullAccessToken = prTrigger.Forks.AllowFullAccessToken,
            AllowSecrets = prTrigger.Forks.AllowSecrets,
            IsCommentRequiredForPR = prTrigger.IsCommentRequiredForPullRequest,
            RequireCommentsForNonTeamMemberAndNonContributors = prTrigger.RequireCommentsForNonTeamMemberAndNonContributors,
            RequireCommentsForNonTeamMembersOnly = prTrigger.RequireCommentsForNonTeamMembersOnly,
            ProjectId = project.Id,
            ProjectVisibility = project.Visibility,
            EnforceJobAuthScope = settingsHelper.EnforceJobAuthScope,
            EnforceReferencedRepoScopedToken = settingsHelper.EnforceReferencedRepoScopedToken,
            DefinitionId = definition.Id
          }), TraceLevel.Info, caller: nameof (IsValidPullRequestTrigger));
        }
      }
      return true;
    }

    private static List<TriggerInstance> FilterPaths(
      IVssRequestContext requestContext,
      RepositoryUpdateInfo repositoryUpdateInfo,
      RefUpdateInfo refUpdate,
      List<TriggerLoadError> triggerLoadErrors,
      List<TriggerInstance> triggers,
      ExternalGitPullRequest pullRequest)
    {
      using (requestContext.TraceSlowCall(nameof (FilteredBuildTriggerHelper), 120000, new Lazy<string>((Func<string>) (() => string.Format("repositoryUpdateInfo.RepositoryId={0}; refUpdate.RefName={1}; triggers.Count={2}", (object) repositoryUpdateInfo?.RepositoryId, (object) refUpdate?.RefName, (object) triggers?.Count))), nameof (FilterPaths)))
      {
        List<TriggerInstance> triggerInstanceList1 = new List<TriggerInstance>();
        List<TriggerInstance> triggerInstanceList2 = new List<TriggerInstance>();
        foreach (TriggerInstance trigger in triggers)
        {
          string str = refUpdate.RefName ?? string.Empty;
          if (trigger.Trigger.PathFilters.Count == 0 || str.StartsWith("refs/tags/"))
          {
            requestContext.TraceInfo(12030099, nameof (FilteredBuildTriggerHelper), "No path filters found or a tag is being updated, ignoring path filters for {0}", (object) str);
            triggerInstanceList1.Add(trigger);
          }
          else
            triggerInstanceList2.Add(trigger);
        }
        if (triggerInstanceList2.Count > 0)
        {
          SparseTree<List<PathMapping>> tree = new SparseTree<List<PathMapping>>('/', StringComparison.Ordinal);
          List<string> filesChanged = (List<string>) null;
          foreach (TriggerInstance trigger in triggerInstanceList2)
          {
            try
            {
              if (filesChanged == null)
              {
                if (!FilteredBuildTriggerHelper.TryGetFilesChanged(requestContext, trigger.Definition, repositoryUpdateInfo, refUpdate, pullRequest, out filesChanged))
                {
                  filesChanged = (List<string>) null;
                  if (!requestContext.IsFeatureEnabled("Build2.DontTriggerBuildsWithoutFilesChanged"))
                  {
                    triggerInstanceList1.Add(trigger);
                    continue;
                  }
                  requestContext.TraceAlways(12030260, TraceLevel.Info, nameof (FilterPaths), nameof (FilteredBuildTriggerHelper), "There were issues finding path filters for {0} so not triggering build", (object) trigger.Definition.RepositoryString);
                  continue;
                }
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException(12030330, nameof (FilteredBuildTriggerHelper), ex);
              if (ex is RateLimitExceededException)
              {
                PipelineEventLogger.LogException(requestContext, (IExternalGitEvent) pullRequest, ex);
                requestContext.TraceAlways(12030262, TraceLevel.Info, nameof (FilterPaths), "QueueFailedBuild", "Added the trigger to queue a failed build because external source provider throttled the API request to GET the PR changes for repo {0}", (object) trigger.Definition.RepositoryString);
                triggerLoadErrors.Add(new TriggerLoadError(trigger.Definition, repositoryUpdateInfo.UpdateId, refUpdate, trigger.SourceOwner, trigger.Trigger.TriggerType, new Exception("Build failed due to API rate limiting from " + repositoryUpdateInfo.RepositoryType)));
              }
              filesChanged = (List<string>) null;
              continue;
            }
            bool flag = true;
            foreach (string pathFilter in trigger.Trigger.PathFilters)
            {
              bool excludeBranch;
              string branch;
              BuildSourceProviders.GitProperties.ParseBranchSpec(pathFilter, out excludeBranch, out branch, out bool _, true);
              if (!branch.StartsWith("/"))
                branch = "/" + branch;
              if (!excludeBranch || branch == "/")
                flag = false;
              FilteredBuildTriggerHelper.AddPathMapping(tree, branch, !excludeBranch, trigger);
            }
            if (flag)
              FilteredBuildTriggerHelper.AddPathMapping(tree, "/", true, trigger);
          }
          if (filesChanged != null)
          {
            HashSet<TriggerInstance> triggerInstanceSet = new HashSet<TriggerInstance>();
            foreach (TriggerInstance triggerInstance in triggerInstanceList2)
            {
              foreach (string filePath in filesChanged)
              {
                if (BuildSourceProviders.GitProperties.IsFilePathIncluded((IEnumerable<string>) triggerInstance.Trigger.PathFilters, filePath))
                {
                  triggerInstanceSet.Add(triggerInstance);
                  break;
                }
              }
            }
            foreach (TriggerInstance triggerInstance in triggerInstanceSet)
              triggerInstanceList1.Add(triggerInstance);
          }
        }
        requestContext.TraceAlways(12030331, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("FilterPaths is returning {0} trigger instances. There were {1} triggers with path filters.", (object) triggerInstanceList1.Count, (object) triggerInstanceList2.Count));
        return triggerInstanceList1;
      }
    }

    private static void AddPathMapping(
      SparseTree<List<PathMapping>> tree,
      string path,
      bool include,
      TriggerInstance trigger)
    {
      List<PathMapping> referencedObject = (List<PathMapping>) null;
      if (!tree.TryGetValue(path, out referencedObject))
      {
        referencedObject = new List<PathMapping>();
        tree.Add(path, referencedObject);
      }
      referencedObject.Add(new PathMapping()
      {
        Include = include,
        TriggerInfo = trigger
      });
    }

    internal static bool IsMatchingBranch(
      IVssRequestContext requestContext,
      RefUpdateInfo refUpdate,
      BuildDefinition definition,
      FilteredBuildTrigger trigger)
    {
      if (trigger.BranchFilters.Count == 0)
      {
        requestContext.TraceInfo(12030030, nameof (FilteredBuildTriggerHelper), "Build definition " + definition.Name + " does not contain branch specifications for repository " + definition.Repository.Name + ", including all branches.");
        trigger.BranchFilters.Add("+refs/heads/*");
      }
      if (BuildSourceProviders.GitProperties.IsRepositoryBranchIncluded((IEnumerable<string>) trigger.BranchFilters, refUpdate.RefName))
        return true;
      requestContext.TraceInfo(12030032, nameof (FilteredBuildTriggerHelper), "Ref " + refUpdate.RefName + " does not match any filters specified by " + string.Join(", ", trigger.BranchFilters.ToArray()) + ".");
      return false;
    }

    private static string GetSourceVersion(
      IVssRequestContext requestContext,
      DefinitionTriggerType triggerType,
      BuildDefinition definition,
      RepositoryUpdateInfo repositoryUpdateInfo,
      RefUpdateInfo refUpdate)
    {
      if (triggerType == DefinitionTriggerType.ContinuousIntegration)
      {
        string str = refUpdate.RefName ?? string.Empty;
        if (!str.StartsWith("refs/tags/", StringComparison.Ordinal))
          return refUpdate.NewObjectId;
        BuildRepository buildRepository = new BuildRepository();
        buildRepository.Id = repositoryUpdateInfo.RepositoryId;
        buildRepository.Type = repositoryUpdateInfo.RepositoryType;
        buildRepository.Properties = definition.Repository?.Properties;
        BuildRepository repository = buildRepository;
        repository.FixBuildRepositoryType();
        string commit = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repository.Type).ResolveToCommit(requestContext, definition.ProjectId, repository, refUpdate.NewObjectId);
        if (!string.IsNullOrEmpty(commit) && !string.Equals(commit, refUpdate.NewObjectId, StringComparison.OrdinalIgnoreCase))
        {
          requestContext.TraceInfo(12030173, nameof (FilteredBuildTriggerHelper), "Peeled tag to a commit for ref '{0}', object ID '{1}', repository ID '{2}', commit ID '{3}'", (object) str, (object) refUpdate.NewObjectId, (object) definition.Repository.Id, (object) commit);
          return commit;
        }
        requestContext.TraceInfo(12030172, nameof (FilteredBuildTriggerHelper), "Unable to peel tag to a commit for ref '{0}', object ID '{1}', repository ID '{2}'", (object) str, (object) refUpdate.NewObjectId, (object) definition.Repository.Id);
        return refUpdate.NewObjectId;
      }
      return Sha1Id.IsNullOrEmpty(refUpdate.MergeObjectId) ? refUpdate.NewObjectId : refUpdate.MergeObjectId;
    }

    private static Dictionary<string, object> GetBuildCIData(BuildData build, string message)
    {
      MinimalBuildDefinition definition = build.Definition;
      int num;
      if (definition == null)
      {
        num = 1;
      }
      else
      {
        // ISSUE: explicit non-virtual call
        int id = __nonvirtual (definition.Id);
        num = 0;
      }
      if (num != 0)
        return new Dictionary<string, object>();
      return new Dictionary<string, object>()
      {
        {
          "BuildReason",
          (object) build.Reason
        },
        {
          "ProjectId",
          (object) build.ProjectId
        },
        {
          "DefinitionId",
          (object) build.Definition.Id
        },
        {
          "DefinitionName",
          (object) build.Definition.Name
        },
        {
          "SourceBranch",
          (object) build.SourceBranch
        },
        {
          "SourceVersion",
          (object) build.SourceVersion
        },
        {
          "YamlErrors",
          (object) JsonConvert.SerializeObject((object) build.ValidationResults.Select<BuildRequestValidationResult, string>((Func<BuildRequestValidationResult, string>) (r => r.Message)))
        },
        {
          "Message",
          (object) message
        }
      };
    }

    private static void PublishCI(
      IVssRequestContext requestContext,
      CustomerIntelligenceData ciData)
    {
      try
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, requestContext.ServiceHost.InstanceId, "Build", "BuildGitEvent", ciData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030332, nameof (FilteredBuildTriggerHelper), ex);
      }
    }

    public static string TruncateMessageToFirstLine(string message) => StringUtil.TruncateToFirstLine(message, 200, out bool _);

    internal static bool RepositoryMatchesUpdateInfo(
      this BuildDefinition definition,
      IVssRequestContext requestContext,
      RepositoryUpdateInfo repositoryUpdateInfo)
    {
      if (definition != null && repositoryUpdateInfo != null && definition.Repository?.Type != null && string.Equals(definition.Repository.Type, repositoryUpdateInfo.RepositoryType, StringComparison.OrdinalIgnoreCase) && definition.Repository.Id != null && string.Equals(definition.Repository.Id, repositoryUpdateInfo.RepositoryId, StringComparison.OrdinalIgnoreCase))
        return true;
      requestContext.TraceAlways(12030333, TraceLevel.Info, "Build2", nameof (FilteredBuildTriggerHelper), string.Format("The updated repository {0} does not mach the repo from the definition {1} {2} ({3})", (object) repositoryUpdateInfo?.RepositoryId, (object) definition?.Id, (object) definition?.Name, (object) definition.Repository.Id));
      return false;
    }

    internal static string LastBatchedCIBuildSourceVersion(
      IVssRequestContext requestContext,
      BuildData currentBuild)
    {
      if (requestContext == null || currentBuild == null)
        return "[No Prior CI build found]";
      BuildData buildData = (BuildData) null;
      using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        buildData = component.GetBuilds(currentBuild.ProjectId, (IEnumerable<int>) new int[1]
        {
          currentBuild.Definition.Id
        }, (IEnumerable<int>) null, (string) null, new DateTime?(), new DateTime?(), (IEnumerable<Guid>) null, new BuildReason?(BuildReason.BatchedCI), new BuildStatus?(BuildStatus.Completed), new BuildResult?(BuildResult.Succeeded), (IEnumerable<string>) null, 1, QueryDeletedOption.ExcludeDeleted, BuildQueryOrder.Descending, (IList<int>) null, currentBuild.Repository.Id, (string) null, currentBuild.SourceBranch, new int?()).ToList<BuildData>().SingleOrDefault<BuildData>();
      if (buildData == null)
        return "[No Prior CI build found]";
      return buildData.SourceVersion;
    }
  }
}
