// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Utils.WebhookUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Events;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Utils
{
  public static class WebhookUtil
  {
    public static void PublishWebHook(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      ClientTraceData eventData,
      HashSet<Sha1Id> wants)
    {
      if (requestContext.IsFeatureEnabled("SourceControl.Git.DisableRepositoryFetchEvent"))
        return;
      IDictionary<string, object> data = eventData.GetData();
      if (!data.ContainsKey("Action"))
        return;
      switch (data["Action"].ToString())
      {
        case "Clone":
          GitCloneEvent payload1 = new GitCloneEvent()
          {
            ProjectId = repository.Key.ProjectId,
            RepositoryId = repository.Key.RepoId,
            UserId = requestContext.GetUserId(),
            CloneDate = DateTime.UtcNow
          };
          VssNotificationEvent repoNotification1 = WebhookUtil.CreateRepoNotification(repository, "ms.vss-code.git-clone-event", (object) payload1);
          WebhookUtil.PublishNotification(requestContext, repoNotification1);
          break;
        case "Fetch":
          GitFetchEvent payload2 = new GitFetchEvent()
          {
            ProjectId = repository.Key.ProjectId,
            RepositoryId = repository.Key.RepoId,
            UserId = requestContext.GetUserId(),
            FetchDate = DateTime.UtcNow,
            Wants = wants
          };
          VssNotificationEvent repoNotification2 = WebhookUtil.CreateRepoNotification(repository, "ms.vss-code.git-fetch-event", (object) payload2);
          WebhookUtil.PublishNotification(requestContext, repoNotification2);
          break;
      }
    }

    private static VssNotificationEvent CreateRepoNotification(
      ITfsGitRepository repo,
      string eventType,
      object payload)
    {
      VssNotificationEvent repoNotification = new VssNotificationEvent(payload);
      repoNotification.EventType = eventType;
      repoNotification.AddScope(VssNotificationEvent.ScopeNames.Project, repo.Key.ProjectId);
      repoNotification.AddScope(VssNotificationEvent.ScopeNames.Repository, repo.Key.RepoId);
      return repoNotification;
    }

    private static void PublishNotification(
      IVssRequestContext requestContext,
      VssNotificationEvent notificationEvent)
    {
      requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, notificationEvent);
    }
  }
}
