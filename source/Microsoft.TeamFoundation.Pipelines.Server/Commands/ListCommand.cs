// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Commands.ListCommand
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.TeamFoundation.ServiceHooks.Sdk.Server;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server.Commands
{
  internal class ListCommand : ICommentCommand
  {
    private string c_layer => nameof (ListCommand);

    public string CommandKeyword => CommandNames.List;

    public string ShortDescription => "List all pipelines for this repository using a comment.";

    public string ExampleUsage => "\"list\"";

    public bool IsValid(
      IVssRequestContext requestContext,
      string providerId,
      JObject authentication,
      ExternalPullRequestCommentEvent commentEvent,
      MergeJobStatus mergeJobStatus,
      out string responseMessage)
    {
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
      out List<Exception> exceptions)
    {
      exceptions = new List<Exception>();
      IPipelineSourceProvider provider = requestContext.GetService<IPipelineSourceProviderService>().GetProvider(providerId);
      List<BuildDefinition> list = BuildServiceHelper.GetDefinitionsForPullRequest(requestContext, provider, commentEvent.PullRequest).ToList<BuildDefinition>();
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.CommentHandlerJob, this.c_layer, "Retrieved {0} definition candidates for pull request {1}", (object) list.Count, (object) commentEvent?.PullRequest?.Url);
      if (!string.IsNullOrEmpty(commentEvent.Command.RemainingParameters))
      {
        list.RemoveAll((Predicate<BuildDefinition>) (d => !this.MatchesFilter(d.Name, commentEvent.Command.RemainingParameters)));
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.CommentHandlerJob, this.c_layer, "Filtered definitions to just {0} candidates for pull request {1}", (object) list.Count, (object) commentEvent?.PullRequest?.Url);
      }
      if (!list.Any<BuildDefinition>())
      {
        PipelineEventLogger.Log(requestContext, PipelineEventType.NoMatchingPipelines, (IExternalGitEvent) commentEvent?.PullRequest);
        responseMessage = CommentResponseBuilder.Build("No pipelines found for this repository.");
      }
      else
        responseMessage = this.GetListAsString(requestContext, list);
      return true;
    }

    private bool MatchesFilter(string name, string filter) => name != null && name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;

    private string GetListAsString(
      IVssRequestContext requestContext,
      List<BuildDefinition> definitions)
    {
      CommentResponseBuilder commentResponseBuilder = new CommentResponseBuilder();
      commentResponseBuilder.AppendLine("CI/CD Pipelines for this repository:").StartList();
      foreach (BuildDefinition definition in definitions)
        commentResponseBuilder.StartListItem().AppendLink(definition.GetWebUrl(requestContext), definition.Name).EndListItem();
      return commentResponseBuilder.EndList().ToString();
    }
  }
}
