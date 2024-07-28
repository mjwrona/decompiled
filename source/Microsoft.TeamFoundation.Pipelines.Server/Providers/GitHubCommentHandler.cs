// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.GitHubCommentHandler
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Commands;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  internal class GitHubCommentHandler : IPipelineEventHandler
  {
    private const string c_layer = "GitHubCommentHandler";
    private const string c_gitHubIssueCommentEventType = "issue_comment";
    private static readonly string[] s_supportedCommandNames = new string[4]
    {
      CommandNames.Help,
      CommandNames.List,
      CommandNames.Run,
      CommandNames.Where
    };

    public bool HandleEvent(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      string jsonPayload,
      IDictionary<string, string> headers)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubCommentHandler), "HandleEvent - github");
      ArgumentUtility.CheckForNull<string>(jsonPayload, nameof (jsonPayload));
      string headerValue = headers.GetHeaderValue("X-GitHub-Event");
      JObject jobject = JObject.Parse(jsonPayload);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubCommentHandler), "Ignoring {0} {1} event at the deployment level.", (object) "GitHub", (object) headerValue);
        return false;
      }
      if (string.Equals("issue_comment", headerValue, StringComparison.OrdinalIgnoreCase))
      {
        ExternalCommentEventCommand commentAndValidate = PublishingUtils.ParseGitHubCommentAndValidate(jobject, (IReadOnlyList<string>) GitHubCommentHandler.s_supportedCommandNames);
        if (commentAndValidate != null)
        {
          try
          {
            ExternalPullRequestCommentEvent externalIssueComment = PublishingUtils.GitHubIssueCommentEventToExternalIssueComment(jobject, commentAndValidate, PipelineEventLogger.GetPipelineEventId(requestContext));
            JObject providerAuthentication = GitHubHelper.GetProviderAuthentication(requestContext, jobject);
            if (CommentCommandProvider.IsValidCommandName(externalIssueComment.Command.CommandKeyword, GitHubCommentHandler.s_supportedCommandNames))
              CommandJobHelper.ExectueCommentCommandInJob(requestContext, providerAuthentication, provider, externalIssueComment);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubCommentHandler), ex);
          }
          return true;
        }
      }
      return false;
    }
  }
}
