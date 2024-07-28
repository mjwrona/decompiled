// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceMan == null)
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources", typeof (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources).Assembly);
        return Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture;
      set => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture = value;
    }

    public static string AccessDeniedMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AccessDeniedMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string AgentBasedDeploymentDefaultName => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AgentBasedDeploymentDefaultName), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ApprovalNotFoundException => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ApprovalNotFoundException), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ArtifactDefinitionAlreadyExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ArtifactDefinitionAlreadyExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ArtifactDefinitionDoesNotExist => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ArtifactDefinitionDoesNotExist), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ArtifactWithAliasNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ArtifactWithAliasNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string AutoTriggerArtifactNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AutoTriggerArtifactNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string AutoTriggerIssueMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AutoTriggerIssueMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string AutoTriggerIssuesBranchConditionIsExcludedUserIssue => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AutoTriggerIssuesBranchConditionIsExcludedUserIssue), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string AutoTriggerIssuesBranchConditionsDontMatchUserIssue => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AutoTriggerIssuesBranchConditionsDontMatchUserIssue), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string AutoTriggerIssuesBuildEventMissingPullRequestData => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AutoTriggerIssuesBuildEventMissingPullRequestData), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string AutoTriggerIssuesTagsDontMatchUserIssue => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AutoTriggerIssuesTagsDontMatchUserIssue), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string AutoTriggerIssuesTriggeringBuildMatchesLastReleaseBuildIssue => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (AutoTriggerIssuesTriggeringBuildMatchesLastReleaseBuildIssue), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string BranchFilterNotAllowedWhenBuildDefinitionBranchIsUsed => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (BranchFilterNotAllowedWhenBuildDefinitionBranchIsUsed), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string BranchNotAllowedForLatestWithBuildDefinitionBranchAndTagsType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (BranchNotAllowedForLatestWithBuildDefinitionBranchAndTagsType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string BuildVersionNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (BuildVersionNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string CancelManualInterventionMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (CancelManualInterventionMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string CannotGetArtifactVersionForSelectDuringReleaseCreationDefaultVersionType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (CannotGetArtifactVersionForSelectDuringReleaseCreationDefaultVersionType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string CannotGetPipelineGeneralSettings => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (CannotGetPipelineGeneralSettings), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string CannotOverrideEnvironmentToManual => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (CannotOverrideEnvironmentToManual), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string CannotUpdateManualIntervention => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (CannotUpdateManualIntervention), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string CDConfiguredSuccessfully => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (CDConfiguredSuccessfully), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ClassicPipelinesDisabled => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ClassicPipelinesDisabled), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ContainerImageTagDoesNotMatchRegex => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ContainerImageTagDoesNotMatchRegex), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ContainerImageTagMatchesRegex => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ContainerImageTagMatchesRegex), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ContainerImageTriggerTagsLimitExceeded => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ContainerImageTriggerTagsLimitExceeded), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string CreatedNewSlot => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (CreatedNewSlot), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string CreatedNewTestWebApp => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (CreatedNewTestWebApp), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefaultVersionTypeNotAllowedForArtifactType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefaultVersionTypeNotAllowedForArtifactType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefaultVersionTypeNotAllowedForSelectedArtifact => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefaultVersionTypeNotAllowedForSelectedArtifact), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefinitionAlreadyUpdated => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefinitionAlreadyUpdated), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefinitionEnvironmentHealthJobFailureMessageFormat => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefinitionEnvironmentHealthJobFailureMessageFormat), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefinitionEnvironmentNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefinitionEnvironmentNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefinitionEnvironmentTemplateAlreadyExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefinitionEnvironmentTemplateAlreadyExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefinitionEnvironmentTemplateNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefinitionEnvironmentTemplateNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefinitionFolderPathsNotFoundMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefinitionFolderPathsNotFoundMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DefinitionReferenceNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DefinitionReferenceNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeletedDefinitionEnvironmentTemplateNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeletedDefinitionEnvironmentTemplateNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeletedReleaseDefinitionNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeletedReleaseDefinitionNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DemandsNotMet => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DemandsNotMet), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentApprovalTimeoutMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentApprovalTimeoutMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentGateAlreadyIgnored => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentGateAlreadyIgnored), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentGatesPhaseName => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentGatesPhaseName), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentHealthNotMet => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentHealthNotMet), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentNotStartedTaskNotFoundMessageFormat => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentNotStartedTaskNotFoundMessageFormat), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentOperationStatusAlreadyUpdated => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentOperationStatusAlreadyUpdated), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentResourceAlreadyExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentResourceAlreadyExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentResourceNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentResourceNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentSkippedOnTarget => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentSkippedOnTarget), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentStatusAlreadyUpdated => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentStatusAlreadyUpdated), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeploymentUpdateNotAllowedException => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeploymentUpdateNotAllowedException), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DeployPhaseWorkflowCannotBeEmpty => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DeployPhaseWorkflowCannotBeEmpty), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DownloadOfCustomArtifactsNotSupported => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DownloadOfCustomArtifactsNotSupported), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DraftReleaseCannotBeStarted => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DraftReleaseCannotBeStarted), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DuplicateGateNames => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DuplicateGateNames), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DuplicateKeyInVariableGroup => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DuplicateKeyInVariableGroup), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DuplicatePhaseRefNameUsedInStage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DuplicatePhaseRefNameUsedInStage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DuplicateRefNameUsedInPhase => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DuplicateRefNameUsedInPhase), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string DuplicateStepsInsertion => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (DuplicateStepsInsertion), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string EnvironmentCancelledByQueuingPolicyComment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (EnvironmentCancelledByQueuingPolicyComment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string EnvironmentResetByAbandonReleaseComment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (EnvironmentResetByAbandonReleaseComment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string EnvironmentResetByQueuingPolicyComment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (EnvironmentResetByQueuingPolicyComment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string EnvironmentResetByReleaseDeletionComment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (EnvironmentResetByReleaseDeletionComment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string EnvironmentResetBySchedulesDeletionComment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (EnvironmentResetBySchedulesDeletionComment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string EnvironmentStatusCannotBeConvertedError => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (EnvironmentStatusCannotBeConvertedError), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ExecutionPolicyConcurrencyCountErrorWithValidValuesSuggestionString => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ExecutionPolicyConcurrencyCountErrorWithValidValuesSuggestionString), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ExecutionPolicyErrorWithValidValuesSuggestionString => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ExecutionPolicyErrorWithValidValuesSuggestionString), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string FolderExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (FolderExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string FolderNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (FolderNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string FolderParentNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (FolderParentNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string GatesAreNotInProgress => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (GatesAreNotInProgress), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string GateWorkflowCannotBeEmpty => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (GateWorkflowCannotBeEmpty), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string GenericDatabaseUpdateError => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (GenericDatabaseUpdateError), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string GitHubInstallationAccessTokenMissingError => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (GitHubInstallationAccessTokenMissingError), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string IgnoreGateUpdateFailedExceptionMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (IgnoreGateUpdateFailedExceptionMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string IgnoreGateUpdateFailedMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (IgnoreGateUpdateFailedMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string IncompatibleTasksInDeployPhase => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (IncompatibleTasksInDeployPhase), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidApprovalUpdate => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidApprovalUpdate), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidAreaRequested => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidAreaRequested), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidArtifactAliasInDeploymentInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidArtifactAliasInDeploymentInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidArtifactDownloadModeInDeploymentInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidArtifactDownloadModeInDeploymentInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidArtifactsDownloadForNonTaskifiedArtifacts => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidArtifactsDownloadForNonTaskifiedArtifacts), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidArtifactTypeInDeploymentInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidArtifactTypeInDeploymentInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidData => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidData), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidDeployPhaseRefName => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidDeployPhaseRefName), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidDeployPhaseType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidDeployPhaseType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidDeployStepUpdate => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidDeployStepUpdate), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidGateSamplingIntervalTime => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidGateSamplingIntervalTime), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidGatesMinimumSuccessWindow => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidGatesMinimumSuccessWindow), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidGateStabilizationTime => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidGateStabilizationTime), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidGateStepType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidGateStepType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidGateTaskTimeout => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidGateTaskTimeout), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidGateTimeout => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidGateTimeout), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidIntegerValue => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidIntegerValue), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidNewEnvironmentId => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidNewEnvironmentId), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidOverrideInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidOverrideInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidOverrideInputsKeyValuePair => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidOverrideInputsKeyValuePair), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidPatchReleaseRequest => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidPatchReleaseRequest), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidRegexInTagFilter => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidRegexInTagFilter), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidReleaseEnvironmentStatus => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidReleaseEnvironmentStatus), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidReleaseEnvironmentStatusUpdate => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidReleaseEnvironmentStatusUpdate), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidReleaseStatusUpdate => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidReleaseStatusUpdate), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidReleaseTriggerType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidReleaseTriggerType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidSelectiveArtifactsInDeploymentInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidSelectiveArtifactsInDeploymentInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidSkipArtifactsDownloadInDeploymentInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidSkipArtifactsDownloadInDeploymentInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidTaskCondition => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidTaskCondition), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidTaskOverrideInputEnvironmentVariable => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidTaskOverrideInputEnvironmentVariable), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidTaskOverrideInputs => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidTaskOverrideInputs), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidTaskOverrideInputValue => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidTaskOverrideInputValue), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidTasksInDeployPhase => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidTasksInDeployPhase), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidTaskTimeout => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidTaskTimeout), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidTriggerType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidTriggerType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidValuesInDeploymentInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidValuesInDeploymentInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidValuesInEnvironmentOptions => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidValuesInEnvironmentOptions), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string InvalidValuesInExecutionPolicy => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (InvalidValuesInExecutionPolicy), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string JobScopeCacheKeyFormat => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (JobScopeCacheKeyFormat), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string LatestArtifactVersionNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (LatestArtifactVersionNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string LatestArtifactVersionUnavailable => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (LatestArtifactVersionUnavailable), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string LatestFromBranchType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (LatestFromBranchType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string LatestType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (LatestType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string LinkedArtifactNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (LinkedArtifactNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string MachineExcluded => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (MachineExcluded), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string MachineGroupNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (MachineGroupNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string MachineOffline => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (MachineOffline), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ManualInterventionNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ManualInterventionNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ManualInterventionRejected => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ManualInterventionRejected), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ManualInterventionResumed => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ManualInterventionResumed), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ManualInterventionTimeoutMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ManualInterventionTimeoutMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string MaxPropertyLengthExceeded => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (MaxPropertyLengthExceeded), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string MissingRequiredArtifactSourceDataInArtifact => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (MissingRequiredArtifactSourceDataInArtifact), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string MoreThanOneTaskInServerPhase => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (MoreThanOneTaskInServerPhase), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string NewDeploymentAlreadyStarted => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (NewDeploymentAlreadyStarted), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string NoLinkedArtifactForReleaseDefinition => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (NoLinkedArtifactForReleaseDefinition), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string NoMachineFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (NoMachineFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string NoMachineFoundWithGivenTags => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (NoMachineFoundWithGivenTags), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string NoPrimaryArtifactInReleaseDefinition => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (NoPrimaryArtifactInReleaseDefinition), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string NoTriggerForReleaseDefinition => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (NoTriggerForReleaseDefinition), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string NumberOfGatesMoreThanLimit => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (NumberOfGatesMoreThanLimit), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ObjectDoesNotExist => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ObjectDoesNotExist), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string OnlyAllowedToAcceptOrRejectManualIntervention => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (OnlyAllowedToAcceptOrRejectManualIntervention), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string OrphanScheduleJobDetected => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (OrphanScheduleJobDetected), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string OverrideInputPropertyTypeNotMatching => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (OverrideInputPropertyTypeNotMatching), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string PhaseConditionInvalid => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (PhaseConditionInvalid), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string PlanIdAlreadyExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (PlanIdAlreadyExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string PoolNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (PoolNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ProcessParametersNotDefiniedForOverrideInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ProcessParametersNotDefiniedForOverrideInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ProcessParametersNotDefinitedForOverrideInputs => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ProcessParametersNotDefinitedForOverrideInputs), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ProjectCollectionReleaseService => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ProjectCollectionReleaseService), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ProjectReleaseService => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ProjectReleaseService), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ProjectScopedServiceIdentityNotFoundInPublicProject => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ProjectScopedServiceIdentityNotFoundInPublicProject), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string PropertyNotAllowedToOverride => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (PropertyNotAllowedToOverride), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string QueueAlreadyExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (QueueAlreadyExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string QueueNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (QueueNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string QueueReleaseNotAllowed => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (QueueReleaseNotAllowed), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string RegexMatchingErrorForContainerImageTag => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (RegexMatchingErrorForContainerImageTag), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string RegexTimeoutForContainerImageTag => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (RegexTimeoutForContainerImageTag), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseApprovalHistoryApprovedPostApproval => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseApprovalHistoryApprovedPostApproval), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseApprovalHistoryApprovedPreApproval => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseApprovalHistoryApprovedPreApproval), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseApprovalHistoryReassignedPostApproval => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseApprovalHistoryReassignedPostApproval), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseApprovalHistoryReassignedPreApproval => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseApprovalHistoryReassignedPreApproval), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseApprovalHistoryRejectedPostApproval => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseApprovalHistoryRejectedPostApproval), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseApprovalHistoryRejectedPreApproval => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseApprovalHistoryRejectedPreApproval), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDefinitionAlreadyExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDefinitionAlreadyExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDefinitionDeletionNotAllowed => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDefinitionDeletionNotAllowed), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDefinitionNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDefinitionNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDefinitionDisabled => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDefinitionDisabled), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDefinitionRevisionAlreadyExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDefinitionRevisionAlreadyExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDefinitionSnapshotFileIdUpdateFailed => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDefinitionSnapshotFileIdUpdateFailed), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDefinitionSnapshotRevisionNotMatched => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDefinitionSnapshotRevisionNotMatched), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDeletionNotAllowedAsKeepForeverIsSet => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDeletionNotAllowedAsKeepForeverIsSet), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDeletionNotAllowedDueToCurrentlyActiveOnEnvironments => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDeletionNotAllowedDueToCurrentlyActiveOnEnvironments), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDeletionNotAllowedDueToPendingOnEnvironments => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDeletionNotAllowedDueToPendingOnEnvironments), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDeletionNotAllowedForRetainedReleases => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDeletionNotAllowedForRetainedReleases), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDeployPhasesSnapshotDeploymentInputCannotBeModified => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDeployPhasesSnapshotDeploymentInputCannotBeModified), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDescriptionFormatForBuildTagsTriggeredRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDescriptionFormatForBuildTagsTriggeredRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDescriptionFormatForBuildTriggeredRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDescriptionFormatForBuildTriggeredRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDescriptionFormatForGitTriggeredRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDescriptionFormatForGitTriggeredRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseDescriptionFormatForScheduleTriggeredRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseDescriptionFormatForScheduleTriggeredRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentAutoRecoveryJobText => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentAutoRecoveryJobText), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentHistoryCancelledDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentHistoryCancelledDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentHistoryPartiallySucceededDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentHistoryPartiallySucceededDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentHistoryQueuedDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentHistoryQueuedDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentHistoryRejectedDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentHistoryRejectedDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentHistoryResetDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentHistoryResetDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentHistoryScheduledDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentHistoryScheduledDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentHistorySucceededDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentHistorySucceededDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentHistoryTriggeredDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentHistoryTriggeredDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseEnvironmentNotInActiveState => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseEnvironmentNotInActiveState), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryAbandonedRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryAbandonedRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryChangeDetailsUnknownApprovalStatus => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryChangeDetailsUnknownApprovalStatus), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryChangeDetailsUnknownMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryChangeDetailsUnknownMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryChangeDetailsUnknownReleaseChangeType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryChangeDetailsUnknownReleaseChangeType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryChangeDetailsUnknownReleaseEnvironmentStatus => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryChangeDetailsUnknownReleaseEnvironmentStatus), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryChangeDetailsUnknownReleaseStatus => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryChangeDetailsUnknownReleaseStatus), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryCreatedRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryCreatedRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryDeletedRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryDeletedRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryPostDeploymentGateUpdate => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryPostDeploymentGateUpdate), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryPostDeploymentGateUpdateInprogress => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryPostDeploymentGateUpdateInprogress), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryPreDeploymentGateUpdate => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryPreDeploymentGateUpdate), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryPreDeploymentGateUpdateInprogress => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryPreDeploymentGateUpdateInprogress), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryStartedRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryStartedRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryUndeleteRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryUndeleteRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseHistoryUpdatedRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseHistoryUpdatedRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseIssuesErrorText => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseIssuesErrorText), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseIssuesWarningText => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseIssuesWarningText), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseNameFormatForBuildTriggeredRelease => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseNameFormatForBuildTriggeredRelease), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseNotInActiveState => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseNotInActiveState), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseRevisionFileIdUpdateFailureTraceMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseRevisionFileIdUpdateFailureTraceMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleasesNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleasesNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseStepByDefinitionEnvironmentIdNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseStepByDefinitionEnvironmentIdNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseStepNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseStepNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ReleaseWithGivenNameAlreadyExists => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ReleaseWithGivenNameAlreadyExists), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string RetrieveFileForRDAndCheckForSecretFailureMessage => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (RetrieveFileForRDAndCheckForSecretFailureMessage), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string RunOnAgentPhaseName => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (RunOnAgentPhaseName), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string RunOnMachineGroupPhaseName => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (RunOnMachineGroupPhaseName), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string RunOnServerPhaseName => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (RunOnServerPhaseName), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ScheduledReleaseCreationFailed => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ScheduledReleaseCreationFailed), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ScheduledReleaseForDefinitionChangeNotInitiated => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ScheduledReleaseForDefinitionChangeNotInitiated), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ScheduleEnvironmentJobExtentionFailureMessageFormat => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ScheduleEnvironmentJobExtentionFailureMessageFormat), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ScheduleReleaseJobExtentionFailureMessageFormat => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ScheduleReleaseJobExtentionFailureMessageFormat), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ScheduleTriggerNotFoundInDefinition => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ScheduleTriggerNotFoundInDefinition), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ScheduleTriggerServiceAccountNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ScheduleTriggerServiceAccountNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string SelectDuringReleaseCreationType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (SelectDuringReleaseCreationType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string SelectiveArtifactsCannotBeEmptyInDeploymentInput => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (SelectiveArtifactsCannotBeEmptyInDeploymentInput), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string SelectiveArtifactsNotSupportedForXamlBuild => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (SelectiveArtifactsNotSupportedForXamlBuild), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ServerJobCreationError => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ServerJobCreationError), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ServiceConnectionIsNotValid => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ServiceConnectionIsNotValid), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ServiceIdentityNotFound => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ServiceIdentityNotFound), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string ShareOutputVariablesNotSupportedError => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (ShareOutputVariablesNotSupportedError), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string SlotCreationFailed => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (SlotCreationFailed), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string StartEnvironmentJobFailureMessageFormat => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (StartEnvironmentJobFailureMessageFormat), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string StringifiedReleaseScheduleFormat => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (StringifiedReleaseScheduleFormat), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string TestWebAppCreationFailed => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (TestWebAppCreationFailed), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string TransactionRequiredError => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (TransactionRequiredError), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string TriggerContentExceededLimit => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (TriggerContentExceededLimit), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string TriggerReasonOnSuccesfulDeployment => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (TriggerReasonOnSuccesfulDeployment), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string TriggerReleaseCreationJobFailureMessageFormat => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (TriggerReleaseCreationJobFailureMessageFormat), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string UnsupportedVariableGroupType => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (UnsupportedVariableGroupType), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);

    public static string VariableNameLengthExceeded => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.ResourceManager.GetString(nameof (VariableNameLengthExceeded), Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.resourceCulture);
  }
}
