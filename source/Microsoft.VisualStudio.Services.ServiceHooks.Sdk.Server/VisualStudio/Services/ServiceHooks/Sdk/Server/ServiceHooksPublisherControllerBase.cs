// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksPublisherControllerBase
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Web;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [ApplyRequestLanguage]
  public abstract class ServiceHooksPublisherControllerBase : TfsApiController
  {
    private const string c_userScopes = "$Scp";
    private const string c_previewScope = "preview_api_all";
    private static readonly IDictionary<Type, HttpStatusCode> s_baseHttpExceptions = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AuthenticationException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (ConsumerActionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ConsumerNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ConsumerNotAvailableException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (FormatException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidOperationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidPayloadException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (IndexOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MissingMethodException),
        HttpStatusCode.NotFound
      },
      {
        typeof (NoPublisherSpecifiedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NotificationNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (NotImplementedException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PublisherNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ServiceHookException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (ServiceHooksPublisherControllerBase.ServiceHooksFeatureFlagNotEnabledException),
        HttpStatusCode.NotFound
      },
      {
        typeof (SubscriptionInputException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (UriFormatException),
        HttpStatusCode.BadRequest
      }
    };
    private static readonly IDictionary<string, string[]> s_eventTypeScopes;
    private static readonly IDictionary<string, List<string>> s_requiredScopes;
    private static readonly IDictionary<string, List<string>> s_requiredScopesForUpdate;

    static ServiceHooksPublisherControllerBase()
    {
      string[] strArray = new string[39]
      {
        "build.complete",
        "workitem.created",
        "workitem.updated",
        "workitem.commented",
        "tfvc.checkin",
        "git.push",
        "git.clone",
        "git.fetch",
        "ms.vss-code.codereview-created-event",
        "ms.vss-code.codereview-updated-event",
        "ms.vss-code.codereview-iteration-changed-event",
        "ms.vss-code.codereview-reviewers-changed-event",
        "message.posted",
        "git.pullrequest.created",
        "git.pullrequest.updated",
        "git.pullrequest.merged",
        "ms.vss-code.git-pullrequest-comment-event",
        "workitem.deleted",
        "workitem.restored",
        "ms.vss-release.release-created-event",
        "ms.vss-release.release-abandoned-event",
        "ms.vss-release.deployment-approval-pending-event",
        "ms.vss-release.deployment-approval-completed-event",
        "ms.vss-release.deployment-started-event",
        "ms.vss-release.deployment-completed-event",
        "ms.vss-pipelines.stage-state-changed-event",
        "ms.vss-pipelines.job-state-changed-event",
        "ms.vss-pipelines.run-state-changed-event",
        "ms.vss-pipelinechecks-events.approval-pending",
        "ms.vss-pipelinechecks-events.approval-completed",
        "elasticagentpool.resized",
        "git.repo.created",
        "git.repo.forked",
        "git.repo.renamed",
        "git.repo.deleted",
        "git.repo.undeleted",
        "git.repo.statuschanged",
        "git.branch.download",
        "ms.vss-release.deployment-mi-pending-event"
      };
      ServiceHooksPublisherControllerBase.s_eventTypeScopes = (IDictionary<string, string[]>) new Dictionary<string, string[]>()
      {
        {
          "preview_api_all",
          strArray
        },
        {
          "vso.work",
          new string[5]
          {
            "workitem.created",
            "workitem.updated",
            "workitem.commented",
            "workitem.deleted",
            "workitem.restored"
          }
        },
        {
          "vso.work_write",
          new string[5]
          {
            "workitem.created",
            "workitem.updated",
            "workitem.commented",
            "workitem.deleted",
            "workitem.restored"
          }
        },
        {
          "vso.work_full",
          new string[5]
          {
            "workitem.created",
            "workitem.updated",
            "workitem.commented",
            "workitem.deleted",
            "workitem.restored"
          }
        },
        {
          "vso.build",
          new string[4]
          {
            "build.complete",
            "ms.vss-pipelines.stage-state-changed-event",
            "ms.vss-pipelines.job-state-changed-event",
            "ms.vss-pipelines.run-state-changed-event"
          }
        },
        {
          "vso.build_execute",
          new string[4]
          {
            "build.complete",
            "ms.vss-pipelines.stage-state-changed-event",
            "ms.vss-pipelines.job-state-changed-event",
            "ms.vss-pipelines.run-state-changed-event"
          }
        },
        {
          "vso.code",
          new string[19]
          {
            "tfvc.checkin",
            "git.push",
            "git.pullrequest.created",
            "git.pullrequest.updated",
            "git.pullrequest.merged",
            "ms.vss-code.git-pullrequest-comment-event",
            "ms.vss-code.codereview-created-event",
            "ms.vss-code.codereview-updated-event",
            "ms.vss-code.codereview-iteration-changed-event",
            "ms.vss-code.codereview-reviewers-changed-event",
            "git.clone",
            "git.fetch",
            "git.repo.created",
            "git.repo.forked",
            "git.repo.renamed",
            "git.repo.deleted",
            "git.repo.undeleted",
            "git.repo.statuschanged",
            "git.branch.download"
          }
        },
        {
          "vso.code_write",
          new string[19]
          {
            "tfvc.checkin",
            "git.push",
            "git.pullrequest.created",
            "git.pullrequest.updated",
            "git.pullrequest.merged",
            "ms.vss-code.git-pullrequest-comment-event",
            "ms.vss-code.codereview-created-event",
            "ms.vss-code.codereview-updated-event",
            "ms.vss-code.codereview-iteration-changed-event",
            "ms.vss-code.codereview-reviewers-changed-event",
            "git.clone",
            "git.fetch",
            "git.repo.created",
            "git.repo.forked",
            "git.repo.renamed",
            "git.repo.deleted",
            "git.repo.undeleted",
            "git.repo.statuschanged",
            "git.branch.download"
          }
        },
        {
          "vso.code_manage",
          new string[19]
          {
            "tfvc.checkin",
            "git.push",
            "git.pullrequest.created",
            "git.pullrequest.updated",
            "git.pullrequest.merged",
            "ms.vss-code.git-pullrequest-comment-event",
            "ms.vss-code.codereview-created-event",
            "ms.vss-code.codereview-updated-event",
            "ms.vss-code.codereview-iteration-changed-event",
            "ms.vss-code.codereview-reviewers-changed-event",
            "git.clone",
            "git.fetch",
            "git.repo.created",
            "git.repo.forked",
            "git.repo.renamed",
            "git.repo.deleted",
            "git.repo.undeleted",
            "git.repo.statuschanged",
            "git.branch.download"
          }
        },
        {
          "vso.code_full",
          new string[19]
          {
            "tfvc.checkin",
            "git.push",
            "git.pullrequest.created",
            "git.pullrequest.updated",
            "git.pullrequest.merged",
            "ms.vss-code.git-pullrequest-comment-event",
            "ms.vss-code.codereview-created-event",
            "ms.vss-code.codereview-updated-event",
            "ms.vss-code.codereview-iteration-changed-event",
            "ms.vss-code.codereview-reviewers-changed-event",
            "git.clone",
            "git.fetch",
            "git.repo.created",
            "git.repo.forked",
            "git.repo.renamed",
            "git.repo.deleted",
            "git.repo.undeleted",
            "git.repo.statuschanged",
            "git.branch.download"
          }
        },
        {
          "vso.chat_write",
          new string[1]{ "message.posted" }
        },
        {
          "vso.chat_manage",
          new string[1]{ "message.posted" }
        },
        {
          "vso.hooks",
          strArray
        },
        {
          "vso.hooks_write",
          strArray
        }
      };
      ServiceHooksPublisherControllerBase.s_requiredScopesForUpdate = (IDictionary<string, List<string>>) new Dictionary<string, List<string>>()
      {
        {
          "workitem.created",
          new List<string>()
          {
            "preview_api_all",
            "vso.work",
            "vso.work_write",
            "vso.work_full",
            "vso.hooks_write"
          }
        },
        {
          "workitem.updated",
          new List<string>()
          {
            "preview_api_all",
            "vso.work",
            "vso.work_write",
            "vso.work_full",
            "vso.hooks_write"
          }
        },
        {
          "workitem.deleted",
          new List<string>()
          {
            "preview_api_all",
            "vso.work",
            "vso.work_write",
            "vso.work_full",
            "vso.hooks_write"
          }
        },
        {
          "workitem.restored",
          new List<string>()
          {
            "preview_api_all",
            "vso.work",
            "vso.work_write",
            "vso.work_full",
            "vso.hooks_write"
          }
        },
        {
          "workitem.commented",
          new List<string>()
          {
            "preview_api_all",
            "vso.work",
            "vso.work_write",
            "vso.work_full",
            "vso.hooks_write"
          }
        },
        {
          "build.complete",
          new List<string>()
          {
            "preview_api_all",
            "vso.build",
            "vso.build_execute",
            "vso.hooks_write"
          }
        },
        {
          "tfvc.checkin",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.push",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.clone",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.fetch",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.repo.created",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.repo.forked",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.repo.renamed",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.repo.deleted",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.repo.undeleted",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.repo.statuschanged",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.pullrequest.created",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.pullrequest.updated",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.pullrequest.merged",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "ms.vss-code.git-pullrequest-comment-event",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "git.branch.download",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "ms.vss-code.codereview-created-event",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "ms.vss-code.codereview-updated-event",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "ms.vss-code.codereview-iteration-changed-event",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "ms.vss-code.codereview-reviewers-changed-event",
          new List<string>()
          {
            "preview_api_all",
            "vso.code",
            "vso.code_write",
            "vso.code_manage",
            "vso.code_full",
            "vso.hooks_write"
          }
        },
        {
          "message.posted",
          new List<string>()
          {
            "preview_api_all",
            "vso.chat_write",
            "vso.chat_manage",
            "vso.hooks_write"
          }
        },
        {
          "ms.vss-pipelines.stage-state-changed-event",
          new List<string>()
          {
            "preview_api_all",
            "vso.build",
            "vso.build_execute",
            "vso.hooks_write"
          }
        },
        {
          "ms.vss-pipelines.job-state-changed-event",
          new List<string>()
          {
            "preview_api_all",
            "vso.build",
            "vso.build_execute",
            "vso.hooks_write"
          }
        },
        {
          "ms.vss-pipelines.run-state-changed-event",
          new List<string>()
          {
            "preview_api_all",
            "vso.build",
            "vso.build_execute",
            "vso.hooks_write"
          }
        }
      };
      ServiceHooksPublisherControllerBase.s_requiredScopes = (IDictionary<string, List<string>>) new Dictionary<string, List<string>>();
      foreach (string key1 in (IEnumerable<string>) ServiceHooksPublisherControllerBase.s_eventTypeScopes.Keys)
      {
        foreach (string key2 in ServiceHooksPublisherControllerBase.s_eventTypeScopes[key1])
        {
          if (ServiceHooksPublisherControllerBase.s_requiredScopes.ContainsKey(key2))
            ServiceHooksPublisherControllerBase.s_requiredScopes[key2].Add(key1);
          else
            ServiceHooksPublisherControllerBase.s_requiredScopes.Add(key2, new List<string>()
            {
              key1
            });
        }
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => ServiceHooksPublisherControllerBase.s_baseHttpExceptions;

    protected ServiceHooksPublisher FindPublisher(string publisherId)
    {
      ServiceHooksPublisherService service = this.TfsRequestContext.GetService<ServiceHooksPublisherService>();
      if (!string.IsNullOrEmpty(publisherId))
        return service.GetPublisher(this.TfsRequestContext, publisherId);
      IEnumerable<ServiceHooksPublisher> publishers = service.GetPublishers(this.TfsRequestContext);
      return (publishers.Count<ServiceHooksPublisher>() <= 1 ? publishers.FirstOrDefault<ServiceHooksPublisher>() : throw new NoPublisherSpecifiedException()) ?? throw new NoPublishersDefinedException();
    }

    protected Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription GetSubscription(
      IVssRequestContext requestContext,
      Guid hooksId)
    {
      Microsoft.VisualStudio.Services.Notifications.Server.Subscription localSubscription = this.GetLocalSubscription(requestContext, hooksId);
      if (localSubscription == null)
        return this.GetHooksService().GetSubscription(requestContext, hooksId);
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription hooksSubscription = localSubscription.GetHooksSubscription();
      SubscriptionInputValueStrongBoxHelper.AddConfidentialInputValuesToSubscription(requestContext, hooksSubscription, true);
      return hooksSubscription;
    }

    protected Microsoft.VisualStudio.Services.Notifications.Server.Subscription GetLocalSubscription(
      IVssRequestContext requestContext,
      Guid hookId)
    {
      return this.GetLocalSubscriptions(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        hookId
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
    }

    protected IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription> GetLocalSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> uniqueIds)
    {
      if (!requestContext.IsFeatureEnabled("Notifications.Channel.AzureServiceBus"))
        return (IEnumerable<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>) new List<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>();
      NotificationSubscriptionService service = requestContext.GetService<NotificationSubscriptionService>();
      IEnumerable<SubscriptionLookup> subscriptionLookups = uniqueIds.Select<Guid, SubscriptionLookup>((Func<Guid, SubscriptionLookup>) (id => SubscriptionLookup.CreateUniqueIdLookup(id)));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<SubscriptionLookup> subscriptionKeys = subscriptionLookups;
      return service.QuerySubscriptions(requestContext1, subscriptionKeys, SubscriptionQueryFlags.None, true).Where<Microsoft.VisualStudio.Services.Notifications.Server.Subscription>((Func<Microsoft.VisualStudio.Services.Notifications.Server.Subscription, bool>) (s => s.IsLocalServiceHooksDelivery));
    }

    protected bool IsRequestUsingAccessToken() => this.TfsRequestContext.Items.ContainsKey("$Scp");

    protected void CheckScope(string EventType, bool update = false)
    {
      if (!this.IsRequestUsingAccessToken() || !ServiceHooksPublisherControllerBase.s_requiredScopesForUpdate.ContainsKey(EventType))
        return;
      string[] userScopes = this.GetUserScopes();
      if (this.ContainsAppToken(userScopes))
        return;
      List<string> RequiredScopes = !update ? ServiceHooksPublisherControllerBase.s_requiredScopes[EventType] : ServiceHooksPublisherControllerBase.s_requiredScopesForUpdate[EventType];
      if (this.MatchScope(userScopes, RequiredScopes))
        return;
      if (update)
        throw new HttpException(403, "No scope found");
      throw new HttpException(404, "Not found");
    }

    protected List<string> GetUserEventTypes()
    {
      List<string> source = new List<string>();
      if (this.IsRequestUsingAccessToken())
      {
        string[] userScopes = this.GetUserScopes();
        if (!this.ContainsAppToken(userScopes))
        {
          foreach (string key in userScopes)
          {
            string[] collection;
            if (ServiceHooksPublisherControllerBase.s_eventTypeScopes.TryGetValue(key, out collection))
              source.AddRange((IEnumerable<string>) collection);
          }
        }
        else
          source.AddRange((IEnumerable<string>) ServiceHooksPublisherControllerBase.s_eventTypeScopes["preview_api_all"]);
      }
      else
        source.AddRange((IEnumerable<string>) ServiceHooksPublisherControllerBase.s_eventTypeScopes["preview_api_all"]);
      return source.Distinct<string>().ToList<string>();
    }

    private string[] GetUserScopes()
    {
      object obj;
      this.TfsRequestContext.Items.TryGetValue("$Scp", out obj);
      return ((string) obj).Split(' ');
    }

    private bool MatchScope(string[] AcquiredScopes, List<string> RequiredScopes)
    {
      foreach (string requiredScope in RequiredScopes)
      {
        if (((IEnumerable<string>) AcquiredScopes).Contains<string>(requiredScope))
          return true;
      }
      return false;
    }

    private bool ContainsAppToken(string[] scopesList) => scopesList.Length == 1 && scopesList[0] == "app_token";

    protected IEnumerable<ServiceHooksPublisher> FindPublishers(string publisherId)
    {
      ServiceHooksPublisherService service = this.TfsRequestContext.GetService<ServiceHooksPublisherService>();
      if (string.IsNullOrEmpty(publisherId))
        return service.GetPublishers(this.TfsRequestContext);
      return (IEnumerable<ServiceHooksPublisher>) new ServiceHooksPublisher[1]
      {
        service.GetPublisher(this.TfsRequestContext, publisherId)
      };
    }

    protected ServiceHooksClientService GetHooksService() => (this.FindPublishers((string) null).FirstOrDefault<ServiceHooksPublisher>() ?? throw new NoPublishersDefinedException()).GetHooksService(this.TfsRequestContext);

    public override string ActivityLogArea => "Service Hooks";

    protected bool CanUserViewDiagnostics(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      ServiceHooksPublisher publisher)
    {
      if (subscription == null || publisher == null || !publisher.CheckPermission(this.TfsRequestContext, subscription, false, true))
        return false;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (subscription.Id.Equals(Guid.Empty) || userIdentity.Id == Guid.Parse(subscription.CreatedBy.Id) || requestContext.ExecutionEnvironment.IsOnPremisesDeployment && this.IsDeploymentAdmin(requestContext, userIdentity) || this.IsCollectionAdmin(requestContext, userIdentity))
        return true;
      Guid result = Guid.Empty;
      return subscription.PublisherInputs.ContainsKey("projectId") && Guid.TryParse(subscription.PublisherInputs["projectId"], out result) && this.IsProjectdAdmin(requestContext, userIdentity, result) || subscription.PublisherInputs.ContainsKey("subscriberId");
    }

    protected void ScrubNotificationHistory(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification)
    {
      if (notification.Details == null)
        return;
      notification.Details.Event = (Event) null;
      notification.Details.Request = (string) null;
      notification.Details.Response = (string) null;
    }

    protected void ScrubNotificationHistory(
      IVssRequestContext requestContext,
      NotificationsQuery notificationsQuery,
      HashSet<Guid> subscriptionsToScrub)
    {
      if (notificationsQuery == null || subscriptionsToScrub == null)
        return;
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification in notificationsQuery.Results.Where<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>((Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification, bool>) (n => subscriptionsToScrub.Contains(n.SubscriptionId))))
        this.ScrubNotificationHistory(requestContext, notification);
    }

    protected void ScrubNotificationHistory(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> notifications)
    {
      if (notifications == null)
        return;
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification in (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) notifications)
        this.ScrubNotificationHistory(requestContext, notification);
    }

    protected IEnumerable<EventTypeDescriptor> GetSupportedEvents(
      IVssRequestContext requestContext,
      ServiceHooksPublisher publisher)
    {
      return requestContext.ExecutionEnvironment.IsHostedDeployment && publisher is ContributedServiceHooksPublisher serviceHooksPublisher && !string.IsNullOrWhiteSpace(serviceHooksPublisher.ServiceInstanceType) && serviceHooksPublisher.ServiceInstanceType != requestContext.ServiceInstanceType().ToString() ? (IEnumerable<EventTypeDescriptor>) requestContext.GetClient<ServiceHooksPublisherHttpClient>(new Guid(serviceHooksPublisher.ServiceInstanceType)).GetPublisherAsync(publisher.Id).SyncResult<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Publisher>().SupportedEvents : publisher.GetSupportedEvents(requestContext);
    }

    private bool IsDeploymentAdmin(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity user)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment || user == null)
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IdentityService>().IsMember(vssRequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, user.Descriptor);
    }

    private bool IsCollectionAdmin(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity user) => user != null && requestContext.GetService<IdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, user.Descriptor);

    private bool IsProjectdAdmin(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity user, Guid projectId) => user != null && !(projectId == Guid.Empty) && requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).HasPermission(requestContext, projectId.ToString(), TeamProjectPermissions.GenericWrite);

    public class ServiceHooksFeatureFlagNotEnabledException : ServiceHookException
    {
    }
  }
}
