// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventTypeMapper
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class EventTypeMapper
  {
    public static readonly Dictionary<string, string> CollectionLegacyToContributedEventTypes = new Dictionary<string, string>()
    {
      {
        "WorkItemChangedEvent",
        "ms.vss-work.workitem-changed-event"
      },
      {
        "CheckinEvent",
        "ms.vss-code.checkin-event"
      },
      {
        "CodeReviewChangedEvent",
        "ms.vss-codereview.codereview-changed-event"
      },
      {
        "GitPushEvent",
        "ms.vss-code.git-push-event"
      },
      {
        "GitPullRequestEvent",
        "ms.vss-code.git-pullrequest-event"
      },
      {
        "GitPullRequestMergeEvent",
        "ms.vss-code.git-pullrequest-merge-event"
      },
      {
        "GitRepositoryCreatedEvent",
        "ms.vss-code.git-repository-created-event"
      },
      {
        "GitRepositoryRenamedEvent",
        "ms.vss-code.git-repository-renamed-event"
      },
      {
        "GitRepositoryDeletedEvent",
        "ms.vss-code.git-repository-deleted-event"
      },
      {
        "GitRepositoryDisabledEvent",
        "ms.vss-code.git-repository-disabled-event"
      },
      {
        "BuildCompletedEvent",
        "ms.vss-build.build-completed-event"
      },
      {
        "BuildCompletionEvent",
        "ms.vss-build.build-completion-legacy-event"
      },
      {
        "BuildCompletionEvent2",
        "ms.vss-build.build-completion-legacy-event2"
      },
      {
        "BuildDefinitionChangedEvent",
        "ms.vss-build.build-definition-changed-event"
      },
      {
        "BuildResourceChangedEvent",
        "ms.vss-build.build-resource-changed-event"
      },
      {
        "BuildStatusChangeEvent",
        "ms.vss-build.build-status-change-event"
      },
      {
        "BuildDefinitionUpgradeCompletionEvent",
        "ms.vss-build.build-definition-upgrade-completion-event"
      },
      {
        "ProjectCreatedEvent",
        "ms.vss-tfs.project-created-event"
      },
      {
        "ProjectDeletedEvent",
        "ms.vss-tfs.project-deleted-event"
      },
      {
        "ProjectUpdatedEvent",
        "ms.vss-tfs.project-updated-event"
      },
      {
        "ShelvesetEvent",
        "ms.vss-code.shelveset-event"
      },
      {
        "ReleaseAbandonedEvent",
        "ms.vss-release.release-abandoned-event"
      },
      {
        "ReleaseCreatedEvent",
        "ms.vss-release.release-created-event"
      },
      {
        "DeploymentApprovalCompletedEvent",
        "ms.vss-release.deployment-approval-completed-event"
      },
      {
        "DeploymentApprovalPendingEvent",
        "ms.vss-release.deployment-approval-pending-event"
      },
      {
        "DeploymentCompletedEvent",
        "ms.vss-release.deployment-completed-event"
      },
      {
        "DeploymentStartedEvent",
        "ms.vss-release.deployment-started-event"
      },
      {
        "DeploymentManualInterventionPendingEvent",
        "ms.vss-release.deployment-mi-pending-event"
      },
      {
        "ApprovalPendingEvent",
        "ms.vss-pipelinechecks-events.approval-pending"
      },
      {
        "ApprovalCompletedEvent",
        "ms.vss-pipelinechecks-events.approval-completed"
      },
      {
        "ManualValidationPendingEvent",
        "ms.vss-pipelinechecks-events.manual-validation-pending-event"
      },
      {
        "ManualValidationCompletedEvent",
        "ms.vss-pipelinechecks-events.manual-validation-completed-event"
      }
    };
    public static readonly Dictionary<string, string> CollectionContributedToLegacyEventTypes = new Dictionary<string, string>()
    {
      {
        "ms.vss-work.workitem-changed-event",
        "WorkItemChangedEvent"
      },
      {
        "ms.vss-code.checkin-event",
        "CheckinEvent"
      },
      {
        "ms.vss-codereview.codereview-changed-event",
        "CodeReviewChangedEvent"
      },
      {
        "ms.vss-code.git-push-event",
        "GitPushEvent"
      },
      {
        "ms.vss-code.git-pullrequest-event",
        "GitPullRequestEvent"
      },
      {
        "ms.vss-code.git-pullrequest-merge-event",
        "GitPullRequestMergeEvent"
      },
      {
        "ms.vss-code.git-repository-created-event",
        "GitRepositoryCreatedEvent"
      },
      {
        "ms.vss-code.git-repository-renamed-event",
        "GitRepositoryRenamedEvent"
      },
      {
        "ms.vss-code.git-repository-deleted-event",
        "GitRepositoryDeletedEvent"
      },
      {
        "ms.vss-code.git-repository-disabled-event",
        "GitRepositoryDisabledEvent"
      },
      {
        "ms.vss-build.build-completed-event",
        "BuildCompletedEvent"
      },
      {
        "ms.vss-build.build-completion-legacy-event",
        "BuildCompletionEvent"
      },
      {
        "ms.vss-build.build-completion-legacy-event2",
        "BuildCompletionEvent2"
      },
      {
        "ms.vss-build.build-definition-changed-event",
        "BuildDefinitionChangedEvent"
      },
      {
        "ms.vss-build.build-resource-changed-event",
        "BuildResourceChangedEvent"
      },
      {
        "ms.vss-build.build-status-change-event",
        "BuildStatusChangeEvent"
      },
      {
        "ms.vss-build.build-definition-upgrade-completion-event",
        "BuildDefinitionUpgradeCompletionEvent"
      },
      {
        "ms.vss-tfs.project-created-event",
        "ProjectCreatedEvent"
      },
      {
        "ms.vss-tfs.project-deleted-event",
        "ProjectDeletedEvent"
      },
      {
        "ms.vss-tfs.project-updated-event",
        "ProjectUpdatedEvent"
      },
      {
        "ms.vss-code.shelveset-event",
        "ShelvesetEvent"
      },
      {
        "ms.vss-release.release-abandoned-event",
        "ReleaseAbandonedEvent"
      },
      {
        "ms.vss-release.release-created-event",
        "ReleaseCreatedEvent"
      },
      {
        "ms.vss-release.deployment-approval-completed-event",
        "DeploymentApprovalCompletedEvent"
      },
      {
        "ms.vss-release.deployment-approval-pending-event",
        "DeploymentApprovalPendingEvent"
      },
      {
        "ms.vss-release.deployment-completed-event",
        "DeploymentCompletedEvent"
      },
      {
        "ms.vss-release.deployment-started-event",
        "DeploymentStartedEvent"
      },
      {
        "ms.vss-release.deployment-mi-pending-event",
        "DeploymentManualInterventionPendingEvent"
      },
      {
        "ms.vss-pipelinechecks-events.approval-pending",
        "ApprovalPendingEvent"
      },
      {
        "ms.vss-pipelinechecks-events.approval-completed",
        "ApprovalCompletedEvent"
      },
      {
        "ms.vss-pipelinechecks-events.manual-validation-pending-event",
        "ManualValidationPendingEvent"
      },
      {
        "ms.vss-pipelinechecks-events.manual-validation-completed-event",
        "ManualValidationCompletedEvent"
      }
    };
    public static readonly Dictionary<string, string> DeploymentLegacyToContributedEventTypes = new Dictionary<string, string>()
    {
      {
        "DatabaseBackupCompletionEvent",
        "ms.vss-tfs.database-backup-completion-event"
      }
    };
    public static readonly Dictionary<string, string> DeploymentContributedToLegacyEventTypes = new Dictionary<string, string>()
    {
      {
        "ms.vss-tfs.database-backup-completion-event",
        "DatabaseBackupCompletionEvent"
      }
    };

    private static string MapIt(
      IVssRequestContext requestContext,
      string eventType,
      Dictionary<string, string> collectionMap,
      Dictionary<string, string> deploymentMap)
    {
      if (!string.IsNullOrEmpty(eventType))
      {
        if (requestContext != null)
        {
          string str;
          if ((requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? collectionMap : deploymentMap).TryGetValue(eventType, out str))
            eventType = str;
        }
        else
        {
          string str;
          if (collectionMap.TryGetValue(eventType, out str) || deploymentMap.TryGetValue(eventType, out str))
            eventType = str;
        }
      }
      return eventType;
    }

    public static string ToLegacy(IVssRequestContext requestContext, string eventType) => EventTypeMapper.MapIt(requestContext, eventType, EventTypeMapper.CollectionContributedToLegacyEventTypes, EventTypeMapper.DeploymentContributedToLegacyEventTypes);

    public static string ToContributed(IVssRequestContext requestContext, string eventType) => EventTypeMapper.MapIt(requestContext, eventType, EventTypeMapper.CollectionLegacyToContributedEventTypes, EventTypeMapper.DeploymentLegacyToContributedEventTypes);

    private static bool IsA(
      IVssRequestContext requestContext,
      string eventType,
      Dictionary<string, string> collectionMap,
      Dictionary<string, string> deploymentMap)
    {
      return !string.IsNullOrEmpty(eventType) && (requestContext == null ? collectionMap.ContainsKey(eventType) || deploymentMap.ContainsKey(eventType) : (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? collectionMap : deploymentMap).ContainsKey(eventType));
    }

    public static bool IsLegacy(IVssRequestContext requestContext, string eventType) => EventTypeMapper.IsA(requestContext, eventType, EventTypeMapper.CollectionLegacyToContributedEventTypes, EventTypeMapper.DeploymentLegacyToContributedEventTypes);

    public static bool IsContributed(IVssRequestContext requestContext, string eventType) => EventTypeMapper.IsA(requestContext, eventType, EventTypeMapper.CollectionContributedToLegacyEventTypes, EventTypeMapper.DeploymentContributedToLegacyEventTypes);

    public static bool IsKnown1stParty(IVssRequestContext requestContext, string eventType) => EventTypeMapper.IsContributed(requestContext, eventType);

    public static void ThrowIfContributedEventName(
      IVssRequestContext requestContext,
      string eventType)
    {
      if (EventTypeMapper.IsContributed(requestContext, eventType))
        throw new ContributedEventNameNotSupportedException(eventType);
    }

    public static void ThrowIfLegacyEventName(IVssRequestContext requestContext, string eventType)
    {
      if (EventTypeMapper.IsLegacy(requestContext, eventType))
        throw new LegacyEventNameNotSupportedException(eventType);
    }
  }
}
