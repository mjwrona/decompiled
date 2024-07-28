// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Commands.RunCommand
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.TeamFoundation.ServiceHooks.Sdk.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server.Commands
{
  internal class RunCommand : ICommentCommand
  {
    private string c_layer => nameof (RunCommand);

    public string CommandKeyword => CommandNames.Run;

    public string ShortDescription => "Run all pipelines or specific pipelines for this repository using a comment. Use this command by itself to trigger all related pipelines, or specify specific pipelines to run.";

    public string ExampleUsage => "\"run\" or \"run pipeline_name, pipeline_name, pipeline_name\"";

    public bool IsValid(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage)
    {
      if (!requestContext.IsFeatureEnabled("Pipelines.DoNotCheckIsCommitAfterComment") && !CommandJobHelper.IsCommentAfterCommit(requestContext, commentEvent.PullRequest, commentEvent.UpdatedAt))
      {
        responseMessage = "No pull request updatedAt time could be found or updateAt is after the comment time for PR " + commentEvent.PullRequest.Number + " in repo " + commentEvent.PullRequest.Repo.Id;
        return false;
      }
      if (!requestContext.GetService<IPipelineSourceProviderService>().GetProvider(providerId).PullRequestProvider.DoesUserHaveWritePermissions(requestContext, authentication, commentEvent.AuthorAssociation, commentEvent.Repo.Id, commentEvent.CommentedBy.Name))
      {
        responseMessage = CommentResponseBuilder.Build("Commenter does not have sufficient privileges for PR " + commentEvent.PullRequest.Number + " in repo " + commentEvent.PullRequest.Repo.Id);
        return false;
      }
      responseMessage = string.Empty;
      return true;
    }

    public bool TryExecute(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage,
      out List<Exception> queueExceptions)
    {
      IPipelineSourceProvider provider = requestContext.GetService<IPipelineSourceProviderService>().GetProvider(providerId);
      queueExceptions = new List<Exception>();
      switch (mergeJobStatus)
      {
        case MergeJobStatus.NotReady:
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.CommentHandlerJob, this.c_layer, "GitHub merge job is still working; retrying job for PR {0}", (object) commentEvent?.PullRequest?.Url);
          responseMessage = (string) null;
          return false;
        case MergeJobStatus.Conflicts:
          responseMessage = CommentResponseBuilder.Build("Pull request contains merge conflicts.");
          return true;
        case MergeJobStatus.PullRequestClosed:
          responseMessage = CommentResponseBuilder.Build("Pull request is closed");
          return true;
        default:
          List<BuildDefinition> list1 = BuildServiceHelper.GetDefinitionsForPullRequest(requestContext, provider, commentEvent.PullRequest).ToList<BuildDefinition>();
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.CommentHandlerJob, this.c_layer, "Retrieved {0} definition candidates for pull request {1}", (object) list1.Count, (object) commentEvent?.PullRequest?.Url);
          List<string> definitionsToBuild = CommandJobHelper.GetCommandArguments(commentEvent.Command.RemainingParameters);
          if (definitionsToBuild.Any<string>())
          {
            list1.RemoveAll((Predicate<BuildDefinition>) (d => !definitionsToBuild.Contains<string>(d.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
            requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.CommentHandlerJob, this.c_layer, "Filtered definitions to just {0} candidates for pull request {1}", (object) list1.Count, (object) commentEvent?.PullRequest?.Url);
          }
          int num1 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Build/Settings/MaxNumberPipelinesForCommentTrigger", true, 10);
          if (!list1.Any<BuildDefinition>())
          {
            PipelineEventLogger.Log(requestContext, PipelineEventType.NoMatchingPipelines, (IExternalGitEvent) commentEvent?.PullRequest);
            responseMessage = CommentResponseBuilder.Build("No pipelines are associated with this pull request.");
          }
          else if (list1.Count > num1)
          {
            requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.CommentHandlerJob, this.c_layer, "Filtered definition count of {0} exceeds configured max count of {1}.", (object) list1.Count, (object) num1);
            responseMessage = CommentResponseBuilder.Build(string.Format("You have several pipelines (over {0}) configured to build pull requests in this repository. Specify which pipelines you would like to run by using /azp run [pipelines] command. You can specify multiple pipelines using a comma separated list.", (object) num1));
            PipelineEventLogger.Log(requestContext, PipelineEventType.DefinitionCountExceeded, (IExternalGitEvent) commentEvent?.PullRequest);
          }
          else
          {
            List<BuildData> source = this.QueueBuildsForPullRequest(requestContext, commentEvent.PullRequest, provider, list1, queueExceptions);
            if (source == null)
              responseMessage = string.Empty;
            else if (!source.Any<BuildData>())
            {
              responseMessage = CommentResponseBuilder.Build("Azure Pipelines could not run because the pipeline triggers exclude this branch/path.");
            }
            else
            {
              List<BuildData> list2 = source.Where<BuildData>((Func<BuildData, bool>) (b =>
              {
                BuildResult? result = b.Result;
                BuildResult buildResult = BuildResult.Failed;
                return result.GetValueOrDefault() == buildResult & result.HasValue;
              })).ToList<BuildData>();
              if (list2.Any<BuildData>())
              {
                int num2 = source.Count - list2.Count;
                responseMessage = num2 <= 0 ? CommentResponseBuilder.Build(string.Format("Azure Pipelines failed to run {0} pipeline(s).", (object) list2.Count)) : CommentResponseBuilder.Build(string.Format("Azure Pipelines successfully started running {0} pipeline(s), but failed to run {1} pipeline(s).", (object) num2, (object) list2.Count));
              }
              else
                responseMessage = CommentResponseBuilder.Build(string.Format("Azure Pipelines successfully started running {0} pipeline(s).", (object) source.Count));
            }
          }
          return true;
      }
    }

    protected List<BuildData> QueueBuildsForPullRequest(
      IVssRequestContext requestContext,
      ExternalGitPullRequest pullRequest,
      IPipelineSourceProvider provider,
      List<BuildDefinition> definitions,
      List<Exception> queueExceptions)
    {
      if (!definitions.Any<BuildDefinition>())
        return (List<BuildData>) null;
      List<IGrouping<Guid, BuildDefinition>> list1 = definitions.GroupBy<BuildDefinition, Guid>((Func<BuildDefinition, Guid>) (d => d.ProjectId)).ToList<IGrouping<Guid, BuildDefinition>>();
      string buildParameters = pullRequest.GetBuildParameters();
      HashSet<string> internalRuntimeVariables = new HashSet<string>((IEnumerable<string>) pullRequest.GetBuildParameterMap().Keys);
      RepositoryUpdateInfo repositoryUpdateInfo = pullRequest.GetRepositoryUpdateInfo(provider.ConnectionCreator.RepositoryType);
      bool providerDefinitionsFound = false;
      List<BuildData> totalBuilds = new List<BuildData>();
      foreach (IGrouping<Guid, BuildDefinition> grouping in list1)
      {
        IGrouping<Guid, BuildDefinition> definitionsOfProject = grouping;
        Guid projectId = definitionsOfProject.Key;
        requestContext.RunAsUser(requestContext.ServiceHost.InstanceId, projectId, provider.ConnectionCreator.IdentityRole, (Action<IVssRequestContext>) (userContext =>
        {
          List<BuildDefinition> list2 = definitionsOfProject.Where<BuildDefinition>((Func<BuildDefinition, bool>) (d => provider.ConnectionCreator.IsProviderDefinition(userContext, d))).ToList<BuildDefinition>();
          if (list2.Any<BuildDefinition>())
          {
            providerDefinitionsFound = true;
            userContext.TraceInfo(this.c_layer, "{0} provider definitions for repo {1} were found in project {2}", (object) list2.Count, (object) pullRequest.Repo?.Id, (object) projectId);
            bool isExternalUser;
            IdentityRef identityRefFromEmail = ExternalBuildHelper.GetIdentityRefFromEmail(userContext, pullRequest.Sender?.Email, out isExternalUser);
            List<TriggerLoadError> triggerLoadErrors = new List<TriggerLoadError>();
            List<TriggerInstance> pullRequestUpdates = FilteredBuildTriggerHelper.GetPullRequestUpdates(userContext, list2, repositoryUpdateInfo, identityRefFromEmail, triggerLoadErrors, pullRequest);
            userContext.TryDeleteOldBuilds(projectId, list2, pullRequest.MergeRef);
            Action<BuildData> additionalBuildSettings = (Action<BuildData>) (buildData =>
            {
              buildData.Parameters = buildParameters;
              buildData.TriggerInfo = (IDictionary<string, string>) pullRequest.GetPullRequestTriggerInfo(provider.ProviderId, isExternalUser);
              buildData.Properties[BuildProperties.PipelinesProvider] = (object) provider.ProviderId;
              buildData.Properties[BuildProperties.PipelineEventId] = (object) pullRequest?.PipelineEventId;
            });
            totalBuilds.AddRange((IEnumerable<BuildData>) FilteredBuildTriggerHelper.QueueBuildsForRepositoryUpdate(userContext, pullRequestUpdates, triggerLoadErrors, additionalBuildSettings, queueExceptions, internalRuntimeVariables));
          }
          else
            userContext.TraceInfo(this.c_layer, "No provider definitions for repo {0} were found in project {1}", (object) pullRequest.Repo?.Id, (object) projectId);
        }));
      }
      PipelineEventLogger.PullRequestBuildsQueued(requestContext, pullRequest, (IEnumerable<BuildData>) totalBuilds);
      return !providerDefinitionsFound ? (List<BuildData>) null : totalBuilds;
    }
  }
}
