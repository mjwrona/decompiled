// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.EventsHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.ServerEvents;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  public static class EventsHelper
  {
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    public static void FireEnvironmentExecutionStartEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      if (environment == null)
        throw new ArgumentNullException(nameof (environment));
      ReleaseEnvironmentStartedServerEvent notificationEvent1 = new ReleaseEnvironmentStartedServerEvent()
      {
        Release = release,
        CurrentEnvironmentId = environment.Id,
        Status = environment.Status,
        ProjectId = projectId
      };
      EventsHelper.FireEvent(requestContext, (object) notificationEvent1);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release contract1 = release.ToContract(requestContext, projectId, false);
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment contract2 = environment.ToContract(release, requestContext, projectId);
      if (contract2 == null)
        return;
      string projectName = ProjectHelper.GetProjectName(requestContext, projectId);
      DeploymentStartedEvent deploymentStartedEvent = new DeploymentStartedEvent();
      deploymentStartedEvent.Environment = contract2;
      deploymentStartedEvent.Release = contract1;
      deploymentStartedEvent.Project = new ProjectReference()
      {
        Id = projectId,
        Name = projectName
      };
      deploymentStartedEvent.Id = contract1.Id;
      deploymentStartedEvent.StageName = contract2.Name;
      deploymentStartedEvent.Url = WebAccessUrlBuilder.GetReleaseWebAccessUri(requestContext, projectName, contract1.Id);
      List<DeploymentAttempt> deploySteps1 = contract2.DeploySteps;
      int? nullable1 = deploySteps1 != null ? deploySteps1.OrderByDescending<DeploymentAttempt, int?>((Func<DeploymentAttempt, int?>) (step => step?.Attempt)).FirstOrDefault<DeploymentAttempt>()?.Attempt : new int?();
      deploymentStartedEvent.AttemptId = nullable1.GetValueOrDefault();
      DeploymentStartedEvent notificationEvent2 = deploymentStartedEvent;
      if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.RemoveExcessEventData"))
      {
        List<DeploymentAttempt> deploySteps2 = contract2.DeploySteps;
        int? nullable2;
        if (deploySteps2 == null)
        {
          nullable1 = new int?();
          nullable2 = nullable1;
        }
        else
        {
          DeploymentAttempt deploymentAttempt = deploySteps2.OrderByDescending<DeploymentAttempt, int>((Func<DeploymentAttempt, int>) (d => d.Attempt)).FirstOrDefault<DeploymentAttempt>();
          if (deploymentAttempt == null)
          {
            nullable1 = new int?();
            nullable2 = nullable1;
          }
          else
            nullable2 = new int?(deploymentAttempt.Attempt);
        }
        nullable1 = nullable2;
        int valueOrDefault = nullable1.GetValueOrDefault();
        EventsHelper.TrimDataFromEnvironmentForAttempt(notificationEvent2.Environment, valueOrDefault);
        notificationEvent2.Release.Environments = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>();
      }
      EventsHelper.FireEvent(requestContext, (object) notificationEvent2);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireReleaseEnvironmentCompletionEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      int currentEnvironmentId,
      EnvironmentStatus environmentStatus,
      string comment)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      try
      {
        ReleaseEnvironmentCompletedServerEvent notificationEvent = new ReleaseEnvironmentCompletedServerEvent()
        {
          Release = release,
          CurrentEnvironmentId = currentEnvironmentId,
          ProjectId = projectId,
          Status = environmentStatus,
          Comment = comment ?? string.Empty
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972001, TraceLevel.Info, "ReleaseManagementService", "Events", "Fired ReleaseEnvironmentCompletionServerEvent, ReleaseId: {0}, EnvironmentId: {1}, EnvironmentStatus: {2}", (object) release.Id, (object) currentEnvironmentId, (object) environmentStatus);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972001, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire ReleaseEnvironmentCompletionServerEvent, ReleaseId: {0}, EnvironmentId: {1}, EnvironmentStatus: {2}. Exception: {3}", (object) release.Id, (object) currentEnvironmentId, (object) environmentStatus, (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireReleaseCreatedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      try
      {
        ReleaseCreatedServerEvent notificationEvent = new ReleaseCreatedServerEvent()
        {
          Release = release,
          ProjectId = projectId
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972004, TraceLevel.Info, "ReleaseManagementService", "Events", "Fired ReleaseCreatedEvent, ReleaseId: {0}, ReleaseName: {1}, ReleaseDefinitionName: {2}", (object) release.Id, (object) release.Name, (object) release.ReleaseDefinitionName);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972004, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire ReleaseCreatedEvent, ReleaseId: {0}, ReleaseName: {1}, ReleaseDefinitionName: {2}. Exception: {3}", (object) release.Id, (object) release.Name, (object) release.ReleaseDefinitionName, (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireReleaseUpdatedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      try
      {
        ReleaseUpdatedServerEvent notificationEvent = new ReleaseUpdatedServerEvent()
        {
          Release = release,
          ProjectId = projectId
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972088, TraceLevel.Info, "ReleaseManagementService", "Events", "Fired ReleaseUpdatedServerEvent, ReleaseId: {0}, ReleaseName: {1}, ReleaseDefinitionName: {2}", (object) release.Id, (object) release.Name, (object) release.ReleaseDefinitionName);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972088, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire ReleaseUpdatedServerEvent, ReleaseId: {0}, ReleaseName: {1}, ReleaseDefinitionName: {2}. Exception: {3}", (object) release.Id, (object) release.Name, (object) release.ReleaseDefinitionName, (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireReleaseCompletedEvent(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      try
      {
        ReleaseCompletedServerEvent notificationEvent = new ReleaseCompletedServerEvent()
        {
          Release = release
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972006, TraceLevel.Info, "ReleaseManagementService", "Events", "Fired ReleaseCompletedEvent, ReleaseId: {0}, ReleaseName: {1}, ReleaseDefinitionName: {2}", (object) release.Id, (object) release.Name, (object) release.ReleaseDefinitionName);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972006, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire ReleaseCompletedEvent, ReleaseId: {0}, ReleaseName: {1}, ReleaseDefinitionName: {2}. Exception: {3}", (object) release.Id, (object) release.Name, (object) release.ReleaseDefinitionName, (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireReleasesDeletedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> hardDeletedReleaseIds)
    {
      if (hardDeletedReleaseIds == null)
        throw new ArgumentNullException(nameof (hardDeletedReleaseIds));
      if (!hardDeletedReleaseIds.Any<int>())
        return;
      try
      {
        ReleasesDeletedServerEvent notificationEvent = new ReleasesDeletedServerEvent()
        {
          ReleaseIds = hardDeletedReleaseIds.Distinct<int>().ToList<int>(),
          ProjectId = projectId
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972036, TraceLevel.Info, "ReleaseManagementService", "Events", "Fired releases deleted evenet in ReleasesDeletedServerEvent, ProjectId: {0}, ReleaseIds: {1}", (object) projectId, (object) string.Join<int>("; ", hardDeletedReleaseIds));
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972036, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire releases deleted event in ReleasesDeletedServerEvent, ProjectId: {0}, ReleaseIds: {1}. Exception: {2}", (object) projectId, (object) string.Join<int>("; ", hardDeletedReleaseIds), (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireReleaseDefinitionsDeletedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> hardDeletedReleaseDefinitionIds)
    {
      if (hardDeletedReleaseDefinitionIds == null)
        throw new ArgumentNullException(nameof (hardDeletedReleaseDefinitionIds));
      if (!hardDeletedReleaseDefinitionIds.Any<int>())
        return;
      try
      {
        ReleaseDefinitionsDeletedServerEvent notificationEvent = new ReleaseDefinitionsDeletedServerEvent()
        {
          ReleaseDefinitionIds = hardDeletedReleaseDefinitionIds.Distinct<int>().ToList<int>(),
          ProjectId = projectId
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972038, TraceLevel.Info, "ReleaseManagementService", "Events", "Fired releasedefinitions deleted evenet in ReleaseDefinitionsDeletedServerEvent, ProjectId: {0}, DefinitionIds: {1}", (object) projectId, (object) string.Join<int>("; ", hardDeletedReleaseDefinitionIds));
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972038, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire releasedefinitions deleted event in ReleaseDefinitionsDeletedServerEvent, ProjectId: {0}, DefinitionIds: {1}. Exception: {2}", (object) projectId, (object) string.Join<int>("; ", hardDeletedReleaseDefinitionIds), (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireReleaseDefinitionChangedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition previousDefinition)
    {
      int num = 0;
      if (releaseDefinition != null)
        num = releaseDefinition.Id;
      else if (previousDefinition != null)
        num = previousDefinition.Id;
      try
      {
        ReleaseDefinitionChangedServerEvent notificationEvent = new ReleaseDefinitionChangedServerEvent()
        {
          ReleaseDefinition = releaseDefinition,
          PreviousReleaseDefinition = previousDefinition,
          ProjectId = projectId
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972040, TraceLevel.Info, "ReleaseManagementService", "Events", "Fire ReleaseDefinitionChangedServerEvent, ReleaseDefinitionId: {0}", (object) num);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972040, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire ReleaseDefinitionChangedServerEvent, ReleaseDefinitionId: {0}. Exception {1}", (object) num, (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireDefinitionCreatedEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      try
      {
        DefinitionCreatedServerEvent notificationEvent = new DefinitionCreatedServerEvent()
        {
          ReleaseDefinition = releaseDefinition
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972005, TraceLevel.Info, "ReleaseManagementService", "Events", "Fire DefinitionCreatedEvent, ReleaseDefinitionId: {0}", (object) releaseDefinition.Id);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972005, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire DefinitionCreatedEvent, ReleaseDefinitionId: {0}. Exception {1}", (object) releaseDefinition.Id, (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Called from pipeline, should not fail if event does not fire")]
    public static void FireApprovalPendingEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep approvalStep,
      int releaseId)
    {
      if (approvalStep == null || approvalStep.IsAutomated)
        return;
      if (approvalStep.StepType == EnvironmentStepType.Deploy)
        return;
      try
      {
        ApprovalPendingServerEvent notificationEvent = new ApprovalPendingServerEvent()
        {
          Release = release,
          ApprovalStep = approvalStep,
          ProjectId = projectId,
          ReleaseId = releaseId
        };
        EventsHelper.FireEvent(requestContext, (object) notificationEvent);
        requestContext.Trace(1972003, TraceLevel.Info, "ReleaseManagementService", "Events", "Fired ApprovalPendingServerEvent, step Id : {0}", (object) approvalStep.Id);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972001, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to fire ApprovalPendingServerEvent, step Id : {0}. Exception {0}", (object) approvalStep.Id, (object) ex);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    public static void FireApprovalCompletedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      ReleaseEnvironmentStep approvalStep)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (approvalStep == null || approvalStep.IsAutomated || approvalStep.StepType == EnvironmentStepType.Deploy)
        return;
      string projectName = ProjectHelper.GetProjectName(requestContext, projectId);
      ReleaseApproval approval = approvalStep.ToApproval(requestContext, projectId, release);
      DeploymentApprovalCompletedEvent approvalCompletedEvent = new DeploymentApprovalCompletedEvent();
      approvalCompletedEvent.Approval = approval;
      approvalCompletedEvent.Release = release.ConvertModelToContract(requestContext, projectId);
      approvalCompletedEvent.Project = new ProjectReference()
      {
        Id = projectId,
        Name = projectName
      };
      approvalCompletedEvent.Id = release.Id;
      approvalCompletedEvent.StageName = approval.ReleaseEnvironmentReference.Name;
      approvalCompletedEvent.Url = WebAccessUrlBuilder.GetReleaseWebAccessUri(requestContext, projectName, release.Id);
      approvalCompletedEvent.AttemptId = approval.Attempt;
      DeploymentApprovalCompletedEvent notificationEvent = approvalCompletedEvent;
      if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.RemoveExcessEventData"))
      {
        int attempt = notificationEvent.Approval.Attempt;
        string currentEnvironmentName = notificationEvent.Approval.ReleaseEnvironmentReference.Name;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment = notificationEvent.Release.Environments.First<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment, bool>) (e => e.Name.Equals(currentEnvironmentName, StringComparison.OrdinalIgnoreCase)));
        EventsHelper.TrimDataFromEnvironmentForAttempt(environment, attempt);
        notificationEvent.Release.Environments = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>()
        {
          environment
        };
      }
      EventsHelper.FireEvent(requestContext, (object) notificationEvent);
    }

    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Does not need to return event")]
    public static void FireReleaseAbandonedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release contract = release.ConvertModelToContract(requestContext, projectId);
      contract.ModifiedBy.DisplayName = IdentityExtensions.GetIdentityDisplayName(requestContext, contract.ModifiedBy.Id);
      contract.CreatedBy.DisplayName = IdentityExtensions.GetIdentityDisplayName(requestContext, contract.CreatedBy.Id);
      string projectName = ProjectHelper.GetProjectName(requestContext, projectId);
      ReleaseAbandonedEvent releaseAbandonedEvent = new ReleaseAbandonedEvent();
      releaseAbandonedEvent.Release = contract;
      releaseAbandonedEvent.Project = new ProjectReference()
      {
        Id = projectId,
        Name = projectName
      };
      releaseAbandonedEvent.Id = contract.Id;
      releaseAbandonedEvent.Url = WebAccessUrlBuilder.GetReleaseWebAccessUri(requestContext, projectName, release.Id);
      ReleaseAbandonedEvent notificationEvent = releaseAbandonedEvent;
      if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.RemoveExcessEventData"))
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) notificationEvent.Release.Environments)
        {
          DeploymentAttempt deploymentAttempt = environment.DeploySteps.OrderByDescending<DeploymentAttempt, int>((Func<DeploymentAttempt, int>) (ds => ds.Attempt)).FirstOrDefault<DeploymentAttempt>();
          EventsHelper.TrimDataFromEnvironmentForAttempt(environment, deploymentAttempt != null ? deploymentAttempt.Attempt : 0);
        }
      }
      EventsHelper.FireEvent(requestContext, (object) notificationEvent);
    }

    private static void TrimDataFromEnvironmentForAttempt(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      int attempt)
    {
      List<DeploymentAttempt> list1 = environment.DeploySteps.Where<DeploymentAttempt>((Func<DeploymentAttempt, bool>) (x => x.Attempt == attempt)).ToList<DeploymentAttempt>();
      environment.DeploySteps = list1;
      List<ReleaseApproval> list2 = environment.PreDeployApprovals.Where<ReleaseApproval>((Func<ReleaseApproval, bool>) (x => x.Attempt == attempt)).ToList<ReleaseApproval>();
      environment.PreDeployApprovals = list2;
      List<ReleaseApproval> list3 = environment.PostDeployApprovals.Where<ReleaseApproval>((Func<ReleaseApproval, bool>) (x => x.Attempt == attempt)).ToList<ReleaseApproval>();
      environment.PostDeployApprovals = list3;
    }

    private static void FireEvent(IVssRequestContext requestContext, object notificationEvent) => requestContext.GetService<IReleaseManagementEventService>().PublishNotification(requestContext, notificationEvent);
  }
}
