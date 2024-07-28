// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics.CustomerIntelligenceHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "These namespacesd are required to capture all telemetry")]
  public static class CustomerIntelligenceHelper
  {
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the intended design")]
    public static void PublishEventsOnEnvironmentCompletion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      EnvironmentStatus status,
      Guid projectId,
      string message,
      Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int, Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>>> getChangesForEnvironment)
    {
      if (currentRelease == null)
        throw new ArgumentNullException(nameof (currentRelease));
      if (releaseEnvironment == null)
        throw new ArgumentNullException(nameof (releaseEnvironment));
      if (getChangesForEnvironment == null)
        throw new ArgumentNullException(nameof (getChangesForEnvironment));
      IList<WorkflowTask> environmentTasks1 = releaseEnvironment.GetEnabledReleaseEnvironmentTasks();
      IEnumerable<string> endpointTypesInUse = environmentTasks1.GetUniqueEndpointTypesInUse(requestContext, releaseEnvironment.ProcessParameters);
      string subscriptionsInUse = environmentTasks1.GetUniqueAzureSubscriptionsInUse(requestContext, releaseEnvironment.ProcessParameters, projectId);
      IList<WorkflowTask> environmentTasks2 = releaseEnvironment.GetEnabledReleaseEnvironmentTasks();
      bool isAzure = Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.CustomerIntelligenceHelper.DoesContainEndpointTargetingAzure(endpointTypesInUse) || CustomerIntelligenceHelper.DoesTaskNameContainsAzure((IEnumerable<WorkflowTask>) environmentTasks2);
      bool isProjectPublic = CustomerIntelligenceHelper.IsProjectPublic(requestContext, projectId);
      CustomerIntelligenceHelper.PublishEnvironmentCompletionEvent(requestContext, currentRelease, releaseEnvironment, environmentTasks2, status, projectId, isAzure, isProjectPublic, subscriptionsInUse, message);
      CustomerIntelligenceHelper.PublishArtifactsUsedInDeploymentEvent(requestContext, currentRelease, releaseEnvironment, projectId, isAzure);
      CustomerIntelligenceHelper.PublishReposUsedInDeploymentEvent(requestContext, currentRelease, releaseEnvironment, projectId, isAzure);
      CustomerIntelligenceHelper.PublishCommittersForEnvironmentCompletedEvent(requestContext, currentRelease, releaseEnvironment, projectId, getChangesForEnvironment, isAzure);
    }

    public static void PublishReleaseGetByUser(
      IVssRequestContext requestContext,
      int releaseId,
      Guid userId,
      Guid userCuid)
    {
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("UserId", (object) userId);
        intelligenceData.Add("UserCUID", (object) userCuid);
        intelligenceData.Add("ReleaseId", (double) releaseId);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmReleaseGetByUser", intelligenceData);
      }), 1972113);
    }

    public static void PublishUpdateRetainBuild(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId,
      string jobResult)
    {
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("ReleaseDefinitionId", (double) releaseDefinitionId);
        intelligenceData.Add("Result", jobResult);
        intelligenceData.AddDataspaceInformationForServerEvents(requestContext, projectId);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmUpdateRetainBuild", intelligenceData);
      }), 1971048);
    }

    public static void PublishReleaseCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Guid projectId)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData releaseCreationEvent = CustomerIntelligenceHelper.CreateReleaseCreationEvent(requestContext, release, projectId, requestContext.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(requestContext, projectId, release.ReleaseDefinitionId));
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, release.CreatedBy, IdentityCuidHelper.GetCuidByVsid(requestContext, release.CreatedBy), "RmReleaseCreated", releaseCreationEvent);
      }), 1972101);
    }

    public static void PublishDefinitionCreated(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData definitionCreationEvent = CustomerIntelligenceHelper.CreateDefinitionCreationEvent(requestContext, releaseDefinition);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmDefinitionCreated", definitionCreationEvent);
      }), 1972103);
    }

    public static void PublishDefinitionDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId)
    {
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData definitionDeletedEvent = CustomerIntelligenceHelper.CreateDefinitionDeletedEvent(requestContext, projectId, requestorId, releaseDefinitionId);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmDefinitionDeleted", definitionDeletedEvent);
      }), 1972105);
    }

    public static void PublishDefinitionMarkedAsDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId)
    {
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData definitionDeletedEvent = CustomerIntelligenceHelper.CreateDefinitionDeletedEvent(requestContext, projectId, requestorId, releaseDefinitionId);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmDefinitionMarkedAsDeleted", definitionDeletedEvent);
      }), 1972106);
    }

    public static void PublishReleaseCompleted(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData releaseCompletionEvent = CustomerIntelligenceHelper.CreateReleaseCompletionEvent(requestContext, release);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmReleaseCompleted", releaseCompletionEvent);
      }), 1972104);
    }

    public static void PublishReleaseWatched(IVssRequestContext requestContext, int releaseId) => CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
    {
      CustomerIntelligenceData releaseWatchedEvent = CustomerIntelligenceHelper.CreateReleaseWatchedEvent(releaseId);
      CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmReleaseWatchedEvent", releaseWatchedEvent);
    }), 1973132);

    public static void PublishDefinitionReleasesWatched(
      IVssRequestContext requestContext,
      int definitionId)
    {
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData definitionWatchedEvent = CustomerIntelligenceHelper.CreateDefinitionWatchedEvent(definitionId);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmDefinitionReleasesWatchedEvent", definitionWatchedEvent);
      }), 1973140);
    }

    public static void PublishRunPlanCompleted(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      int releaseId,
      Guid projectId,
      TimelineRecord jobTimelineRecord,
      bool timelineRecordsPassed)
    {
      if (plan == null)
        throw new ArgumentNullException(nameof (plan));
      if (jobTimelineRecord == null)
        return;
      System.Action publishAction = (System.Action) (() =>
      {
        CustomerIntelligenceData planCompletedEvent = CustomerIntelligenceHelper.CreateRunPlanCompletedEvent(requestContext, plan, releaseId, projectId, jobTimelineRecord, timelineRecordsPassed);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, requestContext.UserContext.Identifier, Guid.Empty, Guid.Empty, "RmRunPlanCompleted", planCompletedEvent);
      });
      CustomerIntelligenceHelper.SafeExecute(requestContext, publishAction, 1973129);
    }

    public static void PublishWorkflowFailedEvent(
      IVssRequestContext requestContext,
      int releaseId,
      Guid projectId,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      string message)
    {
      if (releaseEnvironmentStep == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentStep));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData workflowFailedEvent = CustomerIntelligenceHelper.CreateWorkflowFailedEvent(requestContext, releaseId, projectId, releaseEnvironmentStep, message);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmPublishWorkflowFailedEvent", workflowFailedEvent);
      }), 1973131);
    }

    public static void PublishPlanGroupsStartedEvent(
      IVssRequestContext requestContext,
      IEnumerable<TaskOrchestrationPlanGroupReference> planGroupReferences)
    {
      if (planGroupReferences == null)
        throw new ArgumentNullException(nameof (planGroupReferences));
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        CustomerIntelligenceData groupsStartedEvent = CustomerIntelligenceHelper.CreatePlanGroupsStartedEvent(planGroupReferences);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "ReleasePlanGroupsStartedEvent", groupsStartedEvent);
      }), 1976399);
    }

    public static void PublishManualApprovalCompletedEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> approvals)
    {
      if (requestContext == null || approvals == null || !approvals.Any<ReleaseEnvironmentStep>())
        return;
      foreach (ReleaseEnvironmentStep approval1 in approvals)
      {
        ReleaseEnvironmentStep approval = approval1;
        CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
        {
          CustomerIntelligenceData approvalCompletedEvent = CustomerIntelligenceHelper.CreateManualApprovalCompletedEvent(projectId, approval, currentRelease.GetEnvironment(approval.ReleaseEnvironmentId));
          CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmManualApprovalCompleted", approvalCompletedEvent);
        }), 1980010);
      }
    }

    public static void PublishRevalidatedApprovalEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Guid projectId,
      int environmentId,
      DeploymentAuthorizationInfo approverInfo)
    {
      if (requestContext == null)
        return;
      System.Action publishAction = (System.Action) (() =>
      {
        CustomerIntelligenceData revalidateApprovalEvent = CustomerIntelligenceHelper.CreateRevalidateApprovalEvent(requestContext, projectId, currentRelease.GetEnvironment(environmentId), currentRelease.Id, approverInfo.AuthorizationHeaderFor);
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmRevalidateApprovalEvent", revalidateApprovalEvent);
      });
      CustomerIntelligenceHelper.SafeExecute(requestContext, publishAction, 1980011);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Analytics api shuld not throw")]
    private static void SafeExecute(
      IVssRequestContext requestContext,
      System.Action publishAction,
      int tracepoint)
    {
      try
      {
        publishAction();
      }
      catch (Exception ex)
      {
        requestContext.Trace(tracepoint, TraceLevel.Error, "ReleaseManagementService", "Analytics", "Failed to publish customer intellligence data. Exception {0}", (object) ex);
      }
    }

    private static void PublishCustomerIntelligence(
      IVssRequestContext requestContext,
      string userDisplayName,
      Guid userIdentityId,
      Guid identityConsistentVSID,
      string feature,
      CustomerIntelligenceData intelligenceData)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, requestContext.ServiceHost.InstanceId, userDisplayName, userIdentityId, identityConsistentVSID, DateTime.UtcNow, "ReleaseManagementService", feature, intelligenceData);
    }

    private static void PublishEnvironmentCompletionEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      IList<WorkflowTask> tasks,
      EnvironmentStatus status,
      Guid projectId,
      bool isAzure,
      bool isProjectPublic,
      string azureSubscriptionsUsedInEnvironment,
      string message)
    {
      CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
      {
        Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> dictionary = VariableGroupsMerger.MergeVariableGroups(currentRelease.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
        {
          Value = p.Value.Value,
          IsSecret = p.Value.IsSecret
        }));
        StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
        IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>[] dictionaryArray = new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>[2]
        {
          currentRelease.Variables,
          (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) dictionary
        };
        string azureAgentIds;
        string sharedDeploymentPoolIds;
        string deploymentTargetsAzureSubscriptionId;
        CustomerIntelligenceHelper.GetDeploymentGroupPhaseDetails(requestContext, projectId, releaseEnvironment, DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((IEqualityComparer<string>) ordinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) dictionaryArray), out azureAgentIds, out sharedDeploymentPoolIds, out deploymentTargetsAzureSubscriptionId);
        bool isMachineGroupAzure = azureAgentIds.Length > 0;
        bool isWebAppVariableSubstitutionUsed = CustomerIntelligenceHelper.IsWebAppVariableSubstitutionUsed((IEnumerable<WorkflowTask>) tasks);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition = new DataAccessLayer(requestContext, projectId).GetReleaseDefinition(currentRelease.ReleaseDefinitionId);
        List<string> propertiesFilter = new List<string>();
        propertiesFilter.Add("DefinitionCreationSource");
        propertiesFilter.Add("ReleaseCreationSource");
        currentRelease.PopulateProperties(requestContext, projectId, (IEnumerable<string>) propertiesFilter);
        releaseDefinition.PopulateProperties(requestContext, projectId, (IEnumerable<string>) propertiesFilter);
        CustomerIntelligenceData environmentCompletionEvent = CustomerIntelligenceHelper.CreateEnvironmentCompletionEvent(requestContext, releaseDefinition, currentRelease, releaseEnvironment, tasks, status, projectId, isAzure, isMachineGroupAzure, azureAgentIds, isWebAppVariableSubstitutionUsed, sharedDeploymentPoolIds, isProjectPublic, azureSubscriptionsUsedInEnvironment, deploymentTargetsAzureSubscriptionId, message);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deploymentByAttempt = releaseEnvironment.GetDeploymentByAttempt(releaseEnvironment.GetLatestTrialNumber());
        Guid guid = deploymentByAttempt == null ? currentRelease.CreatedBy : deploymentByAttempt.RequestedFor;
        CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, guid, IdentityCuidHelper.GetCuidByVsid(requestContext, guid), "RmEnvironmentCompleted", environmentCompletionEvent);
      }), 1972102);
    }

    private static void PublishCommittersForEnvironmentCompletedEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Guid projectId,
      Func<IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, int, Dictionary<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>>> getChangesForEnvironment,
      bool isAzure)
    {
      foreach (KeyValuePair<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>> keyValuePair in getChangesForEnvironment(requestContext, projectId, currentRelease, releaseEnvironment, 500))
      {
        KeyValuePair<ArtifactSource, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>> artifactChanges = keyValuePair;
        if (artifactChanges.Value.Count<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change>() > 0)
        {
          foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change change in artifactChanges.Value)
          {
            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Change artifactChange = change;
            string input = string.Empty;
            if (artifactChange.PushedBy?.Id != null)
              input = artifactChange.PushedBy.Id;
            else if (artifactChange.Author?.Id != null)
              input = artifactChange.Author.Id;
            string committer = string.Empty;
            bool isGitHubChange = string.Equals(artifactChange.ChangeType, "GitHub", StringComparison.OrdinalIgnoreCase);
            Guid pusherId;
            if (!Guid.TryParse(input, out pusherId))
              committer = string.IsNullOrEmpty(artifactChange.Author?.UniqueName) ? string.Empty : artifactChange.Author.UniqueName;
            string commitId = artifactChange.Id;
            System.Action publishAction = (System.Action) (() =>
            {
              CustomerIntelligenceData environmentEvent = CustomerIntelligenceHelper.CreateCommittersCompletedEnvironmentEvent(requestContext, commitId, committer, artifactChange.ChangeType, currentRelease, releaseEnvironment, artifactChanges.Key, projectId, isAzure, isGitHubChange);
              CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, pusherId, IdentityCuidHelper.GetCuidByVsid(requestContext, pusherId), "CommittersForEnvironmentCompleted", environmentEvent);
            });
            CustomerIntelligenceHelper.SafeExecute(requestContext, publishAction, 1972115);
          }
        }
      }
    }

    private static void PublishArtifactsUsedInDeploymentEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Guid projectId,
      bool isAzure)
    {
      foreach (ArtifactSource linkedArtifact1 in (IEnumerable<ArtifactSource>) currentRelease.LinkedArtifacts)
      {
        ArtifactSource linkedArtifact = linkedArtifact1;
        CustomerIntelligenceHelper.SafeExecute(requestContext, (System.Action) (() =>
        {
          CustomerIntelligenceData inDeploymentEvent = CustomerIntelligenceHelper.CreateRmArtifactsUsedInDeploymentEvent(requestContext, currentRelease, releaseEnvironment, linkedArtifact, projectId, isAzure, CustomerIntelligenceHelper.IsArtifactPublic(requestContext, linkedArtifact));
          CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmArtifactsUsedInDeployment", inDeploymentEvent);
        }), 1972114);
      }
    }

    private static void PublishReposUsedInDeploymentEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Guid projectId,
      bool isAzure)
    {
      foreach (ArtifactSource linkedArtifact1 in (IEnumerable<ArtifactSource>) currentRelease.LinkedArtifacts)
      {
        ArtifactSource linkedArtifact = linkedArtifact1;
        ArtifactSource artifactSource1 = linkedArtifact;
        if ((artifactSource1 != null ? (artifactSource1.IsBuildArtifact ? 1 : 0) : 0) == 0)
        {
          ArtifactSource artifactSource2 = linkedArtifact;
          if ((artifactSource2 != null ? (artifactSource2.IsGitHubArtifact ? 1 : 0) : 0) == 0)
            continue;
        }
        System.Action publishAction = (System.Action) (() =>
        {
          CustomerIntelligenceData inDeploymentEvent = CustomerIntelligenceHelper.CreateRmReposUsedInDeploymentEvent(requestContext, linkedArtifact, currentRelease, releaseEnvironment, projectId, isAzure);
          CustomerIntelligenceHelper.PublishCustomerIntelligence(requestContext, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), "RmRepositoriesUsedInDeployment", inDeploymentEvent);
        });
        CustomerIntelligenceHelper.SafeExecute(requestContext, publishAction, 1972128);
      }
    }

    private static bool GetRepoIdFromArtifact(ArtifactSource artifact, out string repoId)
    {
      repoId = string.Empty;
      InputValue inputValue;
      if (artifact != null && artifact.IsBuildArtifact && artifact.SourceData.TryGetValue("repository", out inputValue))
        repoId = inputValue?.Value;
      else if (artifact != null && artifact.IsGitHubArtifact)
        repoId = artifact.DefinitionsData?.Value;
      return !string.IsNullOrWhiteSpace(repoId);
    }

    private static bool GetDeploymentChangeType(ArtifactSource artifact, out string changeType)
    {
      changeType = string.Empty;
      InputValue inputValue;
      if (artifact != null && artifact.IsBuildArtifact && artifact.SourceData.TryGetValue("repository.provider", out inputValue))
        changeType = inputValue?.Value;
      else if (artifact != null && artifact.IsGitHubArtifact)
        changeType = "GitHub";
      return !string.IsNullOrWhiteSpace(changeType);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502: Avoid excessive complexity", Justification = "Required to capture all telemetry")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "These namespacesd are required to capture all telemetry")]
    private static CustomerIntelligenceData CreateEnvironmentCompletionEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      IList<WorkflowTask> tasks,
      EnvironmentStatus status,
      Guid projectId,
      bool isAzure,
      bool isMachineGroupAzure,
      string agentIdsOfAzureMachinesInMachineGroup,
      bool isWebAppVariableSubstitutionUsed,
      string sharedDeploymentPoolIds,
      bool isProjectPublic,
      string azureSubscriptionsUsedInEnvironment,
      string deploymentTargetsAzureSubscriptionId,
      string message)
    {
      int attempt = releaseEnvironment != null ? releaseEnvironment.GetLatestTrialNumber() : throw new ArgumentNullException(nameof (releaseEnvironment));
      int id = releaseEnvironment.Id;
      Guid guid = Guid.Empty;
      if (id > 0)
        guid = currentRelease.GetRunPlanId(id, attempt);
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      bool environmentIsAuto1 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentIsAuto(releaseEnvironment, EnvironmentStepType.PreDeploy);
      int stepApprovalInfo1 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentStepApprovalInfo(releaseEnvironment, ReleaseEnvironmentStepStatus.Done, EnvironmentStepType.PreDeploy);
      int stepApprovalInfo2 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentStepApprovalInfo(releaseEnvironment, ReleaseEnvironmentStepStatus.Rejected, EnvironmentStepType.PreDeploy);
      int stepApprovalInfo3 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentStepApprovalInfo(releaseEnvironment, ReleaseEnvironmentStepStatus.Reassigned, EnvironmentStepType.PreDeploy);
      bool environmentIsAuto2 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentIsAuto(releaseEnvironment, EnvironmentStepType.PostDeploy);
      int stepApprovalInfo4 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentStepApprovalInfo(releaseEnvironment, ReleaseEnvironmentStepStatus.Done, EnvironmentStepType.PostDeploy);
      int stepApprovalInfo5 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentStepApprovalInfo(releaseEnvironment, ReleaseEnvironmentStepStatus.Rejected, EnvironmentStepType.PostDeploy);
      int stepApprovalInfo6 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentStepApprovalInfo(releaseEnvironment, ReleaseEnvironmentStepStatus.Reassigned, EnvironmentStepType.PostDeploy);
      List<DeployPhaseSnapshot> list1 = releaseEnvironment.GetDesignerDeployPhaseSnapshots(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.RunOnServer).ToList<DeployPhaseSnapshot>();
      List<WorkflowTask> workflowTaskList1 = list1.Aggregate<DeployPhaseSnapshot, List<WorkflowTask>>(new List<WorkflowTask>(), (Func<List<WorkflowTask>, DeployPhaseSnapshot, List<WorkflowTask>>) ((current, serverDeployPhase) => current.Concat<WorkflowTask>(serverDeployPhase.Workflow.Where<WorkflowTask>((Func<WorkflowTask, bool>) (x => x.Enabled))).ToList<WorkflowTask>()));
      List<DeployPhaseSnapshot> list2 = releaseEnvironment.GetDesignerDeployPhaseSnapshots(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.DeploymentGates).ToList<DeployPhaseSnapshot>();
      List<WorkflowTask> workflowTaskList2 = list2.Aggregate<DeployPhaseSnapshot, List<WorkflowTask>>(new List<WorkflowTask>(), (Func<List<WorkflowTask>, DeployPhaseSnapshot, List<WorkflowTask>>) ((current, gatesDeployPhase) => current.Concat<WorkflowTask>(gatesDeployPhase.Workflow.Where<WorkflowTask>((Func<WorkflowTask, bool>) (x => x.Enabled))).ToList<WorkflowTask>()));
      List<DeployPhaseSnapshot> list3 = releaseEnvironment.GetDesignerDeployPhaseSnapshots(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.AgentBasedDeployment).ToList<DeployPhaseSnapshot>();
      List<WorkflowTask> workflowTaskList3 = list3.Aggregate<DeployPhaseSnapshot, List<WorkflowTask>>(new List<WorkflowTask>(), (Func<List<WorkflowTask>, DeployPhaseSnapshot, List<WorkflowTask>>) ((current, agentDeployPhase) => current.Concat<WorkflowTask>(agentDeployPhase.Workflow.Where<WorkflowTask>((Func<WorkflowTask, bool>) (x => x.Enabled))).ToList<WorkflowTask>()));
      List<DeployPhaseSnapshot> list4 = releaseEnvironment.GetDesignerDeployPhaseSnapshots(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment).ToList<DeployPhaseSnapshot>();
      List<WorkflowTask> workflowTaskList4 = list4.Aggregate<DeployPhaseSnapshot, List<WorkflowTask>>(new List<WorkflowTask>(), (Func<List<WorkflowTask>, DeployPhaseSnapshot, List<WorkflowTask>>) ((current, machineGroupDeployPhase) => current.Concat<WorkflowTask>(machineGroupDeployPhase.Workflow.Where<WorkflowTask>((Func<WorkflowTask, bool>) (x => x.Enabled))).ToList<WorkflowTask>()));
      List<WorkflowTask> list5 = ReleaseEnvironmentExtensions.GetReleaseEnvironmentMetaTasks(releaseDefinition, releaseEnvironment).ToList<WorkflowTask>();
      eventData.Add("ReleaseDefinitionId", (double) currentRelease.ReleaseDefinitionId);
      eventData.Add("ReleaseId", (double) releaseEnvironment.ReleaseId);
      eventData.Add("ReleaseReason", (object) currentRelease.Reason);
      eventData.Add("ReleaseEnvironmentTrialNumber", (double) attempt);
      eventData.Add("IsProjectPublic", isProjectPublic);
      eventData.Add("RunPlanId", (object) guid);
      eventData.Add("IsAzure", isAzure);
      eventData.Add("IsMachineGroupAzure", isMachineGroupAzure);
      eventData.Add("AgentIdsOfAzureMachinesInMachineGroup", agentIdsOfAzureMachinesInMachineGroup);
      eventData.Add("IsWebAppVariableSubstitutionUsed", isWebAppVariableSubstitutionUsed);
      eventData.Add("SharedDeploymentPoolIds", sharedDeploymentPoolIds);
      eventData.Add("AzureSubscriptionsUsedInEnvironment", ((IEnumerable<string>) azureSubscriptionsUsedInEnvironment.Split(',')).ToList<string>());
      eventData.Add("AzureSubscriptionsUsedInDeploymentTargets", ((IEnumerable<string>) deploymentTargetsAzureSubscriptionId.Split(',')).ToList<string>());
      eventData.Add("DefinitionEnvironmentId", (double) releaseEnvironment.DefinitionEnvironmentId);
      eventData.Add("ReleaseEnvironmentId", (double) releaseEnvironment.Id);
      eventData.Add("ReleaseEnvironmentRank", (double) releaseEnvironment.Rank);
      eventData.Add("ReleaseEnvironmentStatus", status.ToString("G"));
      eventData.Add("ReleaseEnvironmentCompletedMessage", message?.Substring(0, Math.Min(200, message.Length)) ?? string.Empty);
      eventData.Add("ReleaseEnvironmentDemandCount", (double) releaseEnvironment.GetCompatDemands().Count<char>());
      eventData.Add("ReleaseEnvironmentStepCount", (double) releaseEnvironment.GetStepsCount());
      eventData.Add("ReleaseEnvironmentVariableCount", (double) releaseEnvironment.Variables.Count);
      eventData.Add("ReleaseEnvironmentRunOptionEmailNotificationType", releaseEnvironment.EnvironmentOptions.EmailNotificationType);
      eventData.Add("ReleaseEnvironmentServerTaskCount", (double) workflowTaskList1.Count);
      eventData.Add("ReleaseEnvironmentGatesPhaseTaskCount", (double) workflowTaskList2.Count);
      eventData.Add("ReleaseEnvironmentAgentTaskCount", (double) workflowTaskList3.Count);
      eventData.Add("ReleaseEnvironmentMachineGroupTaskCount", (double) workflowTaskList4.Count);
      eventData.Add("ReleaseEnvironmentTaskGroupCount", (double) list5.Count);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment latestDeployment = releaseEnvironment.GetLatestDeployment();
      if (latestDeployment != null)
        eventData.Add("ReleaseEnvironmentDeploymentReason", latestDeployment.Reason.ToString("G"));
      eventData.Add("ReleaseEnvironmentServerPhasesCount", (double) list1.Count);
      eventData.Add("ReleaseEnvironmentGatesPhasesCount", (double) list2.Count);
      eventData.Add("ReleaseEnvironmentAgentPhasesCount", (double) list3.Count);
      eventData.Add("ReleaseEnvironmentMachineGroupPhasesCount", (double) list4.Count);
      eventData.Add("ReleaseEnvironmentUniqueTaskIds", tasks.Select<WorkflowTask, string>((Func<WorkflowTask, string>) (t => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) t.TaskId, (object) t.Version))).Distinct<string>().ToList<string>());
      eventData.Add("ReleaseEnvironmentPreDeploymentApprovalIsAutomated", environmentIsAuto1);
      eventData.Add("ReleaseEnvironmentPostDeploymentApprovalIsAutomated", environmentIsAuto2);
      eventData.Add("ReleaseEnvironmentPreDeploymentStepStatusCount_Done", (double) stepApprovalInfo1);
      eventData.Add("ReleaseEnvironmentPreDeploymentStepStatusCount_Rejected", (double) stepApprovalInfo2);
      eventData.Add("ReleaseEnvironmentPreDeploymentStepStatusCount_Reassigned", (double) stepApprovalInfo3);
      eventData.Add("ReleaseEnvironmentPostDeploymentStepStatusCount_Done", (double) stepApprovalInfo4);
      eventData.Add("ReleaseEnvironmentPostDeploymentStepStatusCount_Rejected", (double) stepApprovalInfo5);
      eventData.Add("ReleaseEnvironmentPostDeploymentStepStatusCount_Reassigned", (double) stepApprovalInfo6);
      eventData.Add("ReleaseIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) currentRelease.Id));
      eventData.Add("ReleaseDefinitionIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) currentRelease.ReleaseDefinitionId));
      eventData.Add("DeploymentIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) releaseEnvironment.GetLatestDeploymentId()));
      int count = currentRelease.VariableGroups != null ? currentRelease.VariableGroups.Count : 0;
      eventData.Add("ReleaseVariableGroupCount", (double) count);
      int num1 = currentRelease.VariableGroups != null ? currentRelease.VariableGroups.Count<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup, bool>) (v => v.Type.Equals("AzureKeyVault", StringComparison.OrdinalIgnoreCase))) : 0;
      eventData.Add("ReleaseAzureKeyVaultVariableGroupCount", (double) num1);
      int num2 = list3.Where<DeployPhaseSnapshot>((Func<DeployPhaseSnapshot, bool>) (x => x.DeploymentInput != null && CustomerIntelligenceHelper.IsAgentPhaseParallelExecution(x.DeploymentInput.ToObject<AgentDeploymentInput>()))).Count<DeployPhaseSnapshot>();
      eventData.Add("NumberOfPhasesWithParallelExecution", (double) num2);
      eventData.Add("ReleaseDefinitionSource", (object) releaseDefinition.Source);
      PropertyValue propertyValue1 = releaseDefinition.Properties.Where<PropertyValue>((Func<PropertyValue, bool>) (x => x.PropertyName.Equals("DefinitionCreationSource"))).FirstOrDefault<PropertyValue>();
      object obj1 = propertyValue1 != null ? propertyValue1.Value : (object) string.Empty;
      eventData.Add("DefinitionCreationSource", obj1);
      string str = string.Empty;
      if (currentRelease.LinkedArtifacts.Count > 0)
        str = string.Join(",", currentRelease.LinkedArtifacts.Select<ArtifactSource, string>((Func<ArtifactSource, string>) (artifact => artifact.ArtifactTypeId)));
      eventData.Add("ReleaseCommaSeparatedArtifactTypes", str);
      CustomerIntelligenceHelper.AddReleaseEnvironmentGateData(releaseEnvironment, EnvironmentStepType.PreGate, eventData);
      CustomerIntelligenceHelper.AddReleaseEnvironmentGateData(releaseEnvironment, EnvironmentStepType.PostGate, eventData);
      CustomerIntelligenceHelper.AddAutoRedeployTriggerData(releaseDefinition, releaseEnvironment, eventData);
      CustomerIntelligenceHelper.AddVariableGroupsInServiceEndpointsInformation(requestContext, tasks, currentRelease, releaseEnvironment, eventData);
      PropertyValue propertyValue2 = currentRelease.Properties.Where<PropertyValue>((Func<PropertyValue, bool>) (x => x.PropertyName.Equals("ReleaseCreationSource"))).FirstOrDefault<PropertyValue>();
      object obj2 = propertyValue2 != null ? propertyValue2.Value : (object) string.Empty;
      eventData.Add("ReleaseCreationSource", obj2);
      eventData.AddDataspaceInformationForServerEvents(requestContext, projectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreateReleaseCreationEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("ReleaseId", (double) release.Id);
      eventData.Add("ReleaseCreatedById", (object) release.CreatedBy);
      eventData.Add("ReleaseDefinitionId", (double) release.ReleaseDefinitionId);
      eventData.Add("ReleaseEnvironmentCount", (double) release.Environments.Count);
      eventData.Add("ReleaseVariableCount", (double) release.Variables.Count);
      eventData.Add("ReleaseReason", release.Reason.ToString("G"));
      string str = string.Empty;
      if (release.LinkedArtifacts.Count > 0)
        str = string.Join(",", release.LinkedArtifacts.Select<ArtifactSource, string>((Func<ArtifactSource, string>) (artifact => artifact.ArtifactTypeId)));
      eventData.Add("ReleaseCommaSeparatedArtifactTypes", str);
      eventData.Add("ReleaseArtifactCount", (double) release.LinkedArtifacts.Count);
      eventData.Add("ReleaseIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) release.Id));
      eventData.Add("ReleaseDefinitionIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) release.ReleaseDefinitionId));
      eventData.Add("ReleaseDefinitionSource", (object) releaseDefinition.Source);
      eventData.AddDataspaceInformationForServerEvents(requestContext, projectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreateReleaseCompletionEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("ReleaseId", (double) release.Id);
      eventData.Add("ReleaseReason", (object) release.Reason);
      eventData.Add("ReleaseStatus", release.Status.ToString());
      eventData.Add("ReleaseCurrentEnvironmentRank", (double) release.GetLastModifiedEnvironment().Rank);
      eventData.Add("ReleaseDuration", release.GetRunDuration().TotalSeconds);
      eventData.AddDataspaceInformationForServerEvents(requestContext, release.ProjectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreateReleaseWatchedEvent(int releaseId)
    {
      CustomerIntelligenceData releaseWatchedEvent = new CustomerIntelligenceData();
      releaseWatchedEvent.Add("ReleaseId", (double) releaseId);
      return releaseWatchedEvent;
    }

    private static CustomerIntelligenceData CreateDefinitionWatchedEvent(int definitionId)
    {
      CustomerIntelligenceData definitionWatchedEvent = new CustomerIntelligenceData();
      definitionWatchedEvent.Add("ReleaseDefinitionId", (double) definitionId);
      return definitionWatchedEvent;
    }

    private static CustomerIntelligenceData CreateDefinitionCreationEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition)
    {
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      string str = string.Empty;
      if (releaseDefinition.LinkedArtifacts.Any<ArtifactSource>() && releaseDefinition.PullRequestTriggers.Count > 0)
      {
        List<string> pullRequestTriggerArtifactAliases = releaseDefinition.PullRequestTriggers.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger, string>) (x => x.ArtifactAlias)).ToList<string>();
        str = string.Join(",", releaseDefinition.LinkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => pullRequestTriggerArtifactAliases.Contains(x.Alias))).Select<ArtifactSource, string>((Func<ArtifactSource, string>) (x => x.ArtifactTypeId)));
      }
      int num = 0;
      if (releaseDefinition.Environments.Any<DefinitionEnvironment>())
        num = releaseDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (environment => environment.EnvironmentOptions != null && environment.EnvironmentOptions.PullRequestDeploymentEnabled)).ToList<DefinitionEnvironment>().Count<DefinitionEnvironment>();
      eventData.Add("DefinitionId", (double) releaseDefinition.Id);
      eventData.Add("ReleaseDefinitionIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) releaseDefinition.ProjectId, (object) releaseDefinition.Id));
      eventData.Add("DefinitionEnvironmentCount", (double) releaseDefinition.Environments.Count);
      eventData.Add("DefinitionArtifactCount", (double) releaseDefinition.LinkedArtifacts.Count);
      eventData.Add("DefinitionVariableCount", (double) releaseDefinition.Variables.Count);
      eventData.Add("DefinitionTriggerCount", (double) releaseDefinition.Triggers.Count);
      eventData.Add("DefinitionPullRequestTriggerCount", (double) releaseDefinition.PullRequestTriggers.Count);
      eventData.Add("DefinitionPullRequestEnabledEnvironmentCount", (double) num);
      eventData.Add("DefinitionCommaSeparatedPullRequestTriggerArtifactTypes", str);
      eventData.Add("DefinitionRevisionCount", (double) releaseDefinition.Revision);
      eventData.Add("DefinitionReleaseNameIsDefaultFormat", releaseDefinition.ReleaseNameFormat.Equals("Release-$(Rev:r)"));
      eventData.Add("ReleaseDefinitionSource", (object) releaseDefinition.Source);
      PropertyValue propertyValue = releaseDefinition.Properties.Where<PropertyValue>((Func<PropertyValue, bool>) (x => x.PropertyName.Equals("DefinitionCreationSource"))).FirstOrDefault<PropertyValue>();
      eventData.Add("DefinitionCreationSource", propertyValue != null ? propertyValue.Value : (object) string.Empty);
      eventData.AddDataspaceInformationForServerEvents(requestContext, releaseDefinition.ProjectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreateDefinitionDeletedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid requestorId,
      int releaseDefinitionId)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("DefinitionReleaseDeletionRequestorId", (object) requestorId);
      eventData.Add("DefinitionReleaseDeletedId", (double) releaseDefinitionId);
      eventData.AddDataspaceInformationForServerEvents(requestContext, projectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreateCommittersCompletedEnvironmentEvent(
      IVssRequestContext requestContext,
      string commitId,
      string committer,
      string changeType,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      ArtifactSource artifact,
      Guid projectId,
      bool isAzure,
      bool truncateCommitterHash)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      int latestTrialNumber = releaseEnvironment.GetLatestTrialNumber();
      eventData.Add("ReleaseId", (double) release.Id);
      eventData.Add("ReleaseDefinitionId", (double) release.ReleaseDefinitionId);
      eventData.Add("ReleaseEnvironmentTrialNumber", (double) latestTrialNumber);
      eventData.Add("DefinitionEnvironmentId", (double) releaseEnvironment.DefinitionEnvironmentId);
      eventData.Add("ReleaseEnvironmentId", (double) releaseEnvironment.Id);
      eventData.Add("IsAzure", isAzure);
      eventData.Add("ArtifactId", (double) artifact.Id);
      eventData.Add("ArtifactTypeId", artifact.ArtifactTypeId);
      eventData.Add("ArtifactSourceId", artifact.SourceId);
      eventData.Add("CommitId", commitId);
      if (!string.IsNullOrEmpty(committer))
        eventData.Add("ReleaseEnvironmentCommitterHash", CustomerIntelligenceHelper.GetHash(committer, truncateCommitterHash));
      eventData.Add("ReleaseEnvironmentChangeType", changeType);
      eventData.Add("ReleaseIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) release.Id));
      eventData.Add("ReleaseDefinitionIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) release.ReleaseDefinitionId));
      eventData.Add("DeploymentIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) releaseEnvironment.GetLatestDeploymentId()));
      string repoId;
      if (CustomerIntelligenceHelper.GetRepoIdFromArtifact(artifact, out repoId))
        eventData.Add("RepositoryId", repoId);
      eventData.AddDataspaceInformationForServerEvents(requestContext, projectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreateEventWithReleaseEnvData(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Guid projectId,
      bool isAzure)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("ReleaseId", (double) release.Id);
      eventData.Add("ReleaseDefinitionId", (double) release.ReleaseDefinitionId);
      eventData.Add("DefinitionEnvironmentId", (double) releaseEnvironment.DefinitionEnvironmentId);
      eventData.Add("ReleaseEnvironmentId", (double) releaseEnvironment.Id);
      eventData.Add("IsAzure", isAzure);
      eventData.Add("ReleaseIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) release.Id));
      eventData.Add("ReleaseDefinitionIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) release.ReleaseDefinitionId));
      eventData.Add("DeploymentIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) releaseEnvironment.GetLatestDeploymentId()));
      eventData.AddDataspaceInformationForServerEvents(requestContext, projectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreateRmArtifactsUsedInDeploymentEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      ArtifactSource artifact,
      Guid projectId,
      bool isAzure,
      bool? isPublic)
    {
      CustomerIntelligenceData withReleaseEnvData = CustomerIntelligenceHelper.CreateEventWithReleaseEnvData(requestContext, release, releaseEnvironment, projectId, isAzure);
      withReleaseEnvData.Add("ArtifactTypeId", artifact.ArtifactTypeId);
      withReleaseEnvData.Add("ArtifactSourceId", artifact.SourceId);
      if (artifact.IsMultiDefinitionType)
        withReleaseEnvData.Add("IsMultiDefinitionType", true);
      if (isPublic.HasValue)
        withReleaseEnvData.Add("IsArtifactPublic", (object) isPublic);
      if (artifact is PipelineArtifactSource pipelineArtifactSource && pipelineArtifactSource.VersionId != 0)
        withReleaseEnvData.Add("ArtifactVersionId", (double) pipelineArtifactSource.VersionId);
      return withReleaseEnvData;
    }

    private static CustomerIntelligenceData CreateRmReposUsedInDeploymentEvent(
      IVssRequestContext requestContext,
      ArtifactSource artifact,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      Guid projectId,
      bool isAzure)
    {
      CustomerIntelligenceData withReleaseEnvData = CustomerIntelligenceHelper.CreateEventWithReleaseEnvData(requestContext, release, releaseEnvironment, projectId, isAzure);
      string repoId;
      if (CustomerIntelligenceHelper.GetRepoIdFromArtifact(artifact, out repoId))
        withReleaseEnvData.Add("RepositoryId", repoId);
      withReleaseEnvData.Add("ArtifactId", (double) artifact.Id);
      withReleaseEnvData.Add("ArtifactTypeId", artifact.ArtifactTypeId);
      withReleaseEnvData.Add("ArtifactSourceId", artifact.SourceId);
      string changeType;
      if (CustomerIntelligenceHelper.GetDeploymentChangeType(artifact, out changeType))
        withReleaseEnvData.Add("DeploymentChangeType", changeType);
      return withReleaseEnvData;
    }

    private static CustomerIntelligenceData CreateRunPlanCompletedEvent(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      int releaseId,
      Guid projectId,
      TimelineRecord jobTimelineRecord,
      bool timelineRecordsPassed)
    {
      if (plan == null)
        throw new ArgumentNullException(nameof (plan));
      if (jobTimelineRecord == null)
        throw new ArgumentNullException(nameof (jobTimelineRecord));
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      DateTime? finishTime = jobTimelineRecord.FinishTime;
      DateTime? startTime = jobTimelineRecord.StartTime;
      TimeSpan valueOrDefault = (finishTime.HasValue & startTime.HasValue ? new TimeSpan?(finishTime.GetValueOrDefault() - startTime.GetValueOrDefault()) : new TimeSpan?()).GetValueOrDefault();
      eventData.Add("ReleaseId", (double) releaseId);
      eventData.Add("RunPlanId", (object) plan.PlanId);
      eventData.Add("RunPlanResult", plan.Result.ToString());
      eventData.Add("RunPlanResultCode", plan.ResultCode);
      eventData.Add("RunPlanRequestedById", (object) plan.RequestedById);
      eventData.Add("RunPlanRequestedForId", (object) plan.RequestedForId);
      eventData.Add("JobDuration", valueOrDefault.TotalSeconds);
      eventData.Add("TimelinePassed", timelineRecordsPassed.ToString());
      eventData.AddDataspaceInformationForServerEvents(requestContext, projectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreateWorkflowFailedEvent(
      IVssRequestContext requestContext,
      int releaseId,
      Guid projectId,
      ReleaseEnvironmentStep releaseEnvironmentStep,
      string exceptionMessage)
    {
      if (releaseEnvironmentStep == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentStep));
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("ReleaseId", (double) releaseId);
      eventData.Add("ReleaseEnvironmentStepId", (double) releaseEnvironmentStep.Id);
      eventData.Add("DefinitionId", (double) releaseEnvironmentStep.ReleaseDefinitionId);
      eventData.Add("ReleaseEnvironmentStepStatus", (object) releaseEnvironmentStep.Status);
      eventData.Add("DefinitionEnvironmentId", (double) releaseEnvironmentStep.DefinitionEnvironmentId);
      eventData.Add("ReleaseEnvironmentId", (double) releaseEnvironmentStep.ReleaseEnvironmentId);
      eventData.Add("ReleaseEnvironmentStepFailureMessage", exceptionMessage);
      eventData.AddDataspaceInformationForServerEvents(requestContext, projectId);
      return eventData;
    }

    private static CustomerIntelligenceData CreatePlanGroupsStartedEvent(
      IEnumerable<TaskOrchestrationPlanGroupReference> planGroupReferences)
    {
      CustomerIntelligenceData groupsStartedEvent = new CustomerIntelligenceData();
      groupsStartedEvent.Add("PlanGroupsStartedCount", (double) planGroupReferences.Count<TaskOrchestrationPlanGroupReference>());
      return groupsStartedEvent;
    }

    private static CustomerIntelligenceData CreateManualApprovalCompletedEvent(
      Guid projectId,
      ReleaseEnvironmentStep approval,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      CustomerIntelligenceData approvalCompletedEvent = new CustomerIntelligenceData();
      approvalCompletedEvent.Add("ReleaseIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) approval.ReleaseId));
      approvalCompletedEvent.Add("DeploymentIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) releaseEnvironment.GetLatestDeploymentId()));
      approvalCompletedEvent.Add("ApprovalType", (object) approval.StepType.ToWebApiStepType());
      return approvalCompletedEvent;
    }

    private static CustomerIntelligenceData CreateRevalidateApprovalEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      int releaseId,
      AuthorizationHeaderFor authorizationHeaderFor)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("ReleaseIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) releaseId));
      eventData.Add("ReleaseEnvironmentId", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) releaseEnvironment.Id));
      eventData.Add("DeploymentIdentifier", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) projectId, (object) releaseEnvironment.GetLatestDeploymentId()));
      eventData.Add("AuthorizationHeaderFor", (object) authorizationHeaderFor);
      eventData.AddDataspaceInformationForServerEvents(requestContext, projectId);
      return eventData;
    }

    private static string GetHash(string input, bool truncateHash)
    {
      using (SHA512CryptoServiceProvider cryptoServiceProvider = new SHA512CryptoServiceProvider())
        return HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(input)), 0, truncateHash ? 62 : 64);
    }

    private static bool DoesTaskNameContainsAzure(IEnumerable<WorkflowTask> tasks) => tasks != null && tasks.Any<WorkflowTask>((Func<WorkflowTask, bool>) (task => !string.IsNullOrEmpty(task.Name) && task.Name.ToUpperInvariant().Contains("AZURE")));

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "It requires to be in lower case")]
    private static void GetDeploymentGroupPhaseDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> releaseVariables,
      out string azureAgentIds,
      out string sharedDeploymentPoolIds,
      out string deploymentTargetsAzureSubscriptionId)
    {
      IList<DeployPhaseSnapshot> deployPhaseSnapshots = releaseEnvironment.GetDesignerDeployPhaseSnapshots(Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseTypes.MachineGroupBasedDeployment);
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables = DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) new IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>[3]
      {
        (IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) releaseEnvironment.ProcessParameters.GetProcessParametersAsDataModelVariables(),
        releaseEnvironment.Variables,
        releaseVariables
      });
      HashSet<int> azureAgentIdsList = new HashSet<int>();
      List<int> poolIds = new List<int>();
      HashSet<string> source = new HashSet<string>();
      foreach (DeployPhaseSnapshot deployPhaseSnapshot in (IEnumerable<DeployPhaseSnapshot>) deployPhaseSnapshots)
      {
        if (deployPhaseSnapshot.GetDeploymentInput(variables) is MachineGroupDeploymentInput deploymentInput)
        {
          int queueId = deploymentInput.QueueId;
          IEnumerable<DeploymentMachine> tagsAndProperties = CustomerIntelligenceHelper.GetMachinesWithTagsAndProperties(requestContext, projectId, queueId, deploymentInput.Tags);
          foreach (DeploymentMachine deploymentMachine in tagsAndProperties)
          {
            string str;
            if (deploymentMachine.Properties.TryGetValue<string>("AzureSubscriptionId", out str))
              source.Add(str.ToLowerInvariant());
          }
          int poolIdFromMgId = CustomerIntelligenceHelper.GetPoolIdFromMgId(requestContext, projectId, queueId);
          poolIds.Add(poolIdFromMgId);
          CustomerIntelligenceHelper.GetAgentsWithAzureCapabililty(requestContext, poolIdFromMgId).Intersect<int>(tagsAndProperties.Select<DeploymentMachine, int>((Func<DeploymentMachine, int>) (m => m.Agent.Id))).ToList<int>().ForEach((Action<int>) (x => azureAgentIdsList.Add(x)));
        }
      }
      IList<int> ids = CustomerIntelligenceHelper.FilterSharedDeploymentPoolIds(requestContext, poolIds);
      sharedDeploymentPoolIds = CustomerIntelligenceHelper.GetCommaSeparatedIdsString(ids);
      azureAgentIds = CustomerIntelligenceHelper.GetCommaSeparatedIdsString((IList<int>) azureAgentIdsList.ToList<int>());
      deploymentTargetsAzureSubscriptionId = CustomerIntelligenceHelper.GetCommaSeparatedIdsString((IList<string>) source.ToList<string>());
    }

    private static IList<int> FilterSharedDeploymentPoolIds(
      IVssRequestContext requestContext,
      List<int> poolIds)
    {
      return poolIds.Count == 0 || !requestContext.IsFeatureEnabled("WebAccess.ReleaseManagement.EnableDeploymentPoolSharingTelemetry") ? (IList<int>) new List<int>() : (IList<int>) requestContext.GetService<IDistributedTaskPoolService>().GetDeploymentPoolsSummary(requestContext, includeDeploymentGroupReferences: true, poolIds: (IList<int>) poolIds).Where<DeploymentPoolSummary>((Func<DeploymentPoolSummary, bool>) (x => x.DeploymentGroups.Count > 1)).Select<DeploymentPoolSummary, int>((Func<DeploymentPoolSummary, int>) (x => x.Pool.Id)).ToList<int>();
    }

    private static string GetCommaSeparatedIdsString(IList<int> ids)
    {
      string idsString = string.Empty;
      ids.ToList<int>().ForEach((Action<int>) (x => idsString = idsString + x.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ","));
      idsString = idsString.TrimEnd(',');
      return idsString;
    }

    private static string GetCommaSeparatedIdsString(IList<string> azureSubscriptionIds)
    {
      string idsString = string.Empty;
      azureSubscriptionIds.ToList<string>().ForEach((Action<string>) (x => idsString = idsString + x.ToString() + ","));
      idsString = idsString.TrimEnd(',');
      return idsString;
    }

    private static IEnumerable<DeploymentMachine> GetMachinesWithTagsAndProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<string> tags)
    {
      return (IEnumerable<DeploymentMachine>) requestContext.GetService<IDistributedTaskPoolService>().GetDeploymentMachines(requestContext, projectId, machineGroupId, tags, enabled: new bool?(true), propertyFilters: (IList<string>) new List<string>()
      {
        "AzureSubscriptionId"
      });
    }

    private static IEnumerable<int> GetAgentsWithAzureCapabililty(
      IVssRequestContext requestContext,
      int poolId)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand;
      if (!Microsoft.TeamFoundation.DistributedTask.WebApi.Demand.TryParse("AzureGuestAgent", out demand))
        return (IEnumerable<int>) new List<int>();
      return requestContext.GetService<IDistributedTaskPoolService>().GetAgents(requestContext, poolId, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) new Microsoft.TeamFoundation.DistributedTask.WebApi.Demand[1]
      {
        demand
      }).Select<TaskAgent, int>((Func<TaskAgent, int>) (a => a.Id));
    }

    private static int GetPoolIdFromMgId(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId)
    {
      return requestContext.GetService<IDistributedTaskPoolService>().GetDeploymentGroup(requestContext, projectId, machineGroupId).Pool.Id;
    }

    private static bool IsWebAppVariableSubstitutionUsed(IEnumerable<WorkflowTask> tasks) => tasks.Where<WorkflowTask>((Func<WorkflowTask, bool>) (t =>
    {
      if (!t.TaskId.Equals(new Guid("497d490f-eea7-4f2b-ab94-48d9c1acdcb1")) && !t.TaskId.Equals(new Guid("1B467810-6725-4B6D-ACCD-886174C09BBA")))
        return false;
      if (t.Inputs.ContainsKey("XmlTransformation") && t.Inputs["XmlTransformation"].Equals("true", StringComparison.OrdinalIgnoreCase) || t.Inputs.ContainsKey("XmlVariableSubstitution") && t.Inputs["XmlVariableSubstitution"].Equals("true", StringComparison.OrdinalIgnoreCase))
        return true;
      return t.Inputs.ContainsKey("JSONFiles") && !string.IsNullOrWhiteSpace(t.Inputs["JSONFiles"]);
    })).Any<WorkflowTask>();

    private static bool IsAgentPhaseParallelExecution(AgentDeploymentInput agentDeploymentInput)
    {
      if (agentDeploymentInput == null || agentDeploymentInput.ParallelExecution.ParallelExecutionType == ParallelExecutionTypes.None)
        return false;
      if (agentDeploymentInput.ParallelExecution.ParallelExecutionType == ParallelExecutionTypes.MultiMachine)
      {
        if ((agentDeploymentInput.ParallelExecution as ParallelExecutionInputBase).MaxNumberOfAgents > 1)
          return true;
      }
      else if (agentDeploymentInput.ParallelExecution.ParallelExecutionType == ParallelExecutionTypes.MultiConfiguration && !string.IsNullOrEmpty((agentDeploymentInput.ParallelExecution as MultiConfigInput).Multipliers))
        return true;
      return false;
    }

    private static void AddAutoRedeployTriggerData(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment,
      CustomerIntelligenceData eventData)
    {
      DefinitionEnvironment definitionEnvironment = releaseDefinition.Environments.FirstOrDefault<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (i => i.Id == environment.DefinitionEnvironmentId));
      if (definitionEnvironment == null || definitionEnvironment.EnvironmentTriggers.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger>())
        return;
      IEnumerable<byte> source = definitionEnvironment.EnvironmentTriggers.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger, byte>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.EnvironmentTrigger, byte>) (t => t.TriggerType));
      if (!source.Any<byte>())
        return;
      string str = string.Empty;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.EnvironmentTriggerType environmentTriggerType in source)
        str = environmentTriggerType.ToString() + ",";
      eventData.Add("DefinitionEnvironmentAutoRedeployTriggers", str);
    }

    private static void AddReleaseEnvironmentGateData(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment,
      EnvironmentStepType gateType,
      CustomerIntelligenceData eventData)
    {
      ReleaseDefinitionGatesOptions definitionGatesOptions = gateType == EnvironmentStepType.PreGate ? environment.PreDeploymentGates.GatesOptions : environment.PostDeploymentGates.GatesOptions;
      if (definitionGatesOptions == null)
        return;
      string name1 = gateType == EnvironmentStepType.PreGate ? "PreDeploymentGatesEnabled" : "PostDeploymentGatesEnabled";
      eventData.Add(name1, definitionGatesOptions.IsEnabled);
      if (!definitionGatesOptions.IsEnabled)
        return;
      string name2 = gateType == EnvironmentStepType.PreGate ? "PreDeploymentGates_SamplingTime" : "PostDeploymentGates_SamplingTime";
      eventData.Add(name2, (double) definitionGatesOptions.SamplingInterval);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment latestDeployment = environment.GetLatestDeployment();
      if (latestDeployment?.DeploymentGates == null)
        return;
      DeploymentGate deploymentGate = latestDeployment?.DeploymentGates.FirstOrDefault<DeploymentGate>((Func<DeploymentGate, bool>) (g => g.GateType == gateType));
      if (deploymentGate == null)
        return;
      string str1 = gateType == EnvironmentStepType.PreGate ? "PreDeploymentGates_StartTime" : "PostDeploymentGates_StartTime";
      CustomerIntelligenceData intelligenceData1 = eventData;
      string name3 = str1;
      DateTime? startedOn = deploymentGate.StartedOn;
      ref DateTime? local1 = ref startedOn;
      string str2 = (local1.HasValue ? local1.GetValueOrDefault().ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) : (string) null) ?? string.Empty;
      intelligenceData1.Add(name3, str2);
      string str3 = gateType == EnvironmentStepType.PreGate ? "PreDeploymentGates_StabilizationCompleteTime" : "PostDeploymentGates_StabilizationCompleteTime";
      CustomerIntelligenceData intelligenceData2 = eventData;
      string name4 = str3;
      DateTime? stabilizationCompletedOn = deploymentGate.StabilizationCompletedOn;
      ref DateTime? local2 = ref stabilizationCompletedOn;
      string str4 = (local2.HasValue ? local2.GetValueOrDefault().ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) : (string) null) ?? string.Empty;
      intelligenceData2.Add(name4, str4);
      string name5 = gateType == EnvironmentStepType.PreGate ? "PreDeploymentGates_EndTime" : "PostDeploymentGates_EndTime";
      string str5 = string.Empty;
      if (deploymentGate.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.Succeeded || deploymentGate.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.Failed || deploymentGate.Status == Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.GateStatus.Canceled)
      {
        DateTime? lastModifiedOn = deploymentGate.LastModifiedOn;
        ref DateTime? local3 = ref lastModifiedOn;
        str5 = (local3.HasValue ? local3.GetValueOrDefault().ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) : (string) null) ?? string.Empty;
      }
      eventData.Add(name5, str5);
      string name6 = gateType == EnvironmentStepType.PreGate ? "PreDeploymentGates_Result" : "PostDeploymentGates_Result";
      eventData.Add(name6, (object) deploymentGate.Status);
    }

    private static bool IsProjectPublic(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<IProjectService>().GetProject(requestContext, projectId).Visibility == ProjectVisibility.Public;

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This should not fail if the getRepo call fails.")]
    private static bool? IsGitHubRepoPublic(IVssRequestContext requestContext, string gitHubRepoUrl)
    {
      try
      {
        GitHubHttpClient gitHubHttpClient = GitHubHttpClientFactory.Create(requestContext);
        GitHubAuthentication authentication = new GitHubAuthentication(GitHubAuthScheme.None, string.Empty);
        authentication.AcceptUntrustedCertificates = true;
        string repoUrl = gitHubRepoUrl;
        GitHubResult<GitHubData.V3.Repository> repo = gitHubHttpClient.GetRepo(authentication, repoUrl);
        if (repo.IsSuccessful)
        {
          GitHubData.V3.Repository result = repo.Result;
          return new bool?(result != null && !result.Private);
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1971046, TraceLevel.Error, "ReleaseManagementService", "Analytics", ex.ToString());
        return new bool?();
      }
      return new bool?();
    }

    private static bool? IsArtifactPublic(
      IVssRequestContext requestContext,
      ArtifactSource artifact)
    {
      switch (artifact.ArtifactTypeId)
      {
        case "Build":
        case "Git":
        case "TFVC":
          Guid result;
          return Guid.TryParse(artifact.ProjectData.Value, out result) ? new bool?(CustomerIntelligenceHelper.IsProjectPublic(requestContext, result)) : new bool?();
        case "GitHub":
          if (artifact.SourceData == null)
            return new bool?();
          string enumerable = artifact.SourceData["definition"]?.Value;
          if (enumerable.IsNullOrEmpty<char>())
            return new bool?();
          string gitHubRepoUrl = StringUtil.Format("https://api.github.com/repos/{0}", (object) enumerable);
          return CustomerIntelligenceHelper.IsGitHubRepoPublic(requestContext, gitHubRepoUrl);
        default:
          return new bool?(false);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to continue on error")]
    private static void AddDataspaceInformationForServerEvents(
      this CustomerIntelligenceData eventData,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      ProjectVisibility projectVisibility = ProjectVisibility.Private;
      try
      {
        projectVisibility = requestContext.GetService<IProjectService>().GetProject(requestContext1, projectId).Visibility;
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(1976457, "ReleaseManagementService", "Service", ex);
      }
      eventData.AddDataspaceInformation(CustomerIntelligenceDataspaceType.Project, projectId.ToString(), ((int) projectVisibility).ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static void AddVariableGroupsInServiceEndpointsInformation(
      IVssRequestContext requestContext,
      IList<WorkflowTask> tasks,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release currentRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment,
      CustomerIntelligenceData eventData)
    {
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) DictionaryMerger.MergeDictionaries<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) new Dictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>[2]
      {
        VariableGroupsMerger.MergeVariableGroups(currentRelease.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$({0})", (object) p.Key)), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
        {
          Value = p.Value.Value,
          IsSecret = p.Value.IsSecret
        })),
        VariableGroupsMerger.MergeVariableGroups(releaseEnvironment.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$({0})", (object) p.Key)), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>) (p => new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue()
        {
          Value = p.Value.Value,
          IsSecret = p.Value.IsSecret
        }))
      }).Keys);
      IList<ServiceEndpointTasksReference> serviceEndpointsInUse = tasks.GetServiceEndpointsInUse(requestContext, (IDictionary<string, string>) releaseEnvironment.ProcessParameters.GetProcessParametersInputs(), (IList<TaskDefinition>) null);
      HashSet<string> source = new HashSet<string>();
      foreach (ServiceEndpointTasksReference endpointTasksReference in (IEnumerable<ServiceEndpointTasksReference>) serviceEndpointsInUse)
      {
        if (endpointTasksReference.EndpointReference != null && stringSet.Contains(endpointTasksReference.EndpointReference))
          source.Add(endpointTasksReference.EndpointType);
      }
      bool flag = source.Count != 0;
      eventData.Add("IsGroupVariableUsedInServiceEndpoint", flag);
      eventData.Add("ServiceEndpointWithGroupVariablesTypes", source.ToList<string>());
    }
  }
}
