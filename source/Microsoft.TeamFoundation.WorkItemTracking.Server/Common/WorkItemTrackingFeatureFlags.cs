// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTrackingFeatureFlags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class WorkItemTrackingFeatureFlags
  {
    public const string WorkItemTrackingEnableMetadataFileCache = "WorkItemTracking.Server.EnableMetadataFileCache";
    public const string AllowNonClusteredColumnstoreIndex = "WorkItemTracking.Server.Queries.AllowNonClusteredColumnstoreIndex";
    public const string MetadataNewDummyRowsDisabled = "WorkItemTracking.Server.MetadataNewDummyRowsDisabled";
    public const string ProjectChangeProcessDisabled = "WorkItemTracking.Server.ProjectChangeProcessDisabled";
    public const string SetValueAndReadOnlyRuleDisabled = "WorkItemTracking.Server.SetValueAndReadOnlyRuleDisabled";
    public const string ForNotReadOnlyRuleDisabled = "WorkItemTracking.Server.ForNotReadOnlyRuleDisabled";
    public const string EnableQueryAutoOptimization = "WorkItemTracking.Server.Queries.AutoOptimization";
    public const string SaveQueryExecutionInformationSprocV2Enabled = "WorkItemTracking.Server.Queries.SaveQueryExecutionInformationSprocV2Enabled";
    public const string QueryExecutionInformationSprocDisabled = "WorkItemTracking.Server.Queries.QueryExecutionInformationSprocDisabled";
    public const string DisableQueryAutoOptimizationFuzzMatch = "WorkItemTracking.Server.Queries.AutoOptimization.DisableFuzzMatch";
    public const string OverrideOptimizationOrder = "WorkItemTracking.Server.Queries.AutoOptimization.OverrideOptimizationOrder";
    public const string ResetNonOptimizableQueries = "WorkItemTracking.Server.Queries.AutoOptimization.ResetNonOptimizableQueries";
    public const string FilterDuplicateQueryExecutionHistories = "WorkItemTracking.Server.Queries.AutoOptimization.FilterDuplicateQueryExecutionHistories";
    public const string EnableQueryIdentityConstIdOptimization = "WorkItemTracking.Server.Queries.EnableQueryIdentityConstIdOptimization";
    public const string DisableQueryHashTag = "WorkItemTracking.Server.Queries.M174.DisableQueryHashTag";
    public const string ReadOnlyFields = "WorkItemTracking.Server.ReadOnlyFields";
    public const string ProcessAdminServiceFieldPrefixLegacyBehaviorEnabled = "WorkItemTracking.Server.ProcessAdminServiceFieldPrefixLegacyBehaviorEnabled";
    public const string WorkItemTrackingMoveFeature = "WorkItemTracking.Server.MoveWorkItems";
    public const string WorkItemTrackingChangeWITFeature = "WorkItemTracking.Server.ChangeWorkItemType";
    public const string DeleteAttachmentsWithNoCurrentReference = "WorkItemTracking.Server.DeleteNoCurrentReferenceAttachments";
    public const string FireWorkItemsChangedWithExtensionsBatchEvent = "WorkItemTracking.Server.FireWorkItemsChangedWithExtensionsBatchEvent";
    public const string FireWorkItemsChangedWithAllCustomFields = "WorkItemTracking.Server.FireWorkItemsChangedWithAllCustomFields";
    public const string AllowAllOperatorsOnCustomFields = "WorkItemTracking.Server.AllowAllOperatorsOnCustomFields";
    public const string AllowTagsInAlerts = "WorkItemTracking.Server.AllowTagsInAlerts";
    public const string AllowTagsInAlertsUI = "WorkItemTracking.Server.AllowTagsInAlertsUI";
    public const string ReclassifyWorkItemsRaceConditionFixDisabled = "WorkItemTracking.Server.ReclassifyWorkItemsRaceConditionFixDisabled";
    public const string TruncateRichTextValuesForNotifications = "WorkItemTracking.Server.TruncateRichTextValuesForNotifications";
    public const string WorkItemProjectFieldModelFactoryPerfFixDisabled = "WorkItemTracking.Server.WorkItemProjectFieldModelFactoryPerfFixDisabled";
    public const string RemoveContainsOperatorOnRichTextFields = "WorkItemTracking.Server.RemoveContainsOperatorOnRichTextFields";
    public const string AddContainsOperatorOnRichTextFields = "WorkItemTracking.Server.AddContainsOperatorOnRichTextFields";
    public const string EnableWorkItemChangedEventJSONNotification = "WorkItemTracking.Server.EnableWorkItemChangedEventJSONNotification";
    public const string EnableNewTestStepsEmail = "WorkItemTracking.Server.EnableNewTestStepsEmail";
    public const string WorkItemMove = "WorkItemTracking.Server.MoveWorkItems";
    public const string WorkItemTrackingNotificationFilter = "WorkItemTracking.Server.NotificationFilter";
    public const string WorkItemTrackingFollowsFilter = "WorkItemTracking.Server.FollowsFilter";
    public const string WorkItemTrackingFireEventsToServiceBus = "WorkItemTracking.Server.EnableFireEventsToServiceBus";
    public const string ClassificationNodeChangeServiceBusNotification = "WorkItemTracking.Server.CssChangeServiceBusNotification";
    public const string ProjectCreationStampDbDisabled = "WorkItemTracking.Server.ProjectCreationStampDbDisabled";
    public const string WorkItemURLsWithProjectContext = "WorkItemTracking.Server.WorkItemURLsWithProjectContext";
    public const string DisableBackfillingCommentCount = "WorkItemTracking.Server.DisableBackfillingCommentCount";
    public const string EnableDebugValidUserGroup = "WorkItemTracking.Server.DebugValidUserGroup";
    public const string DisableDeletionOfDanglingLinks = "WorkItemTracking.Server.DisableDeletionOfDanglingLinks";
    public const string EnableFieldsExistInWITCheck = "WorkItemTracking.Server.EnableFieldsExistInWITCheck";
    public const string EnableInheritedProcessValidationOnProjectCreate = "WorkItemTracking.Server.EnableInheritedProcessValidationOnProjectCreate";
    public const string EnableDeletePicklistWhenNotReferencedByAnyField = "WorkItemTracking.Server.EnableDeletePicklistWhenNotReferencedByAnyField";
    public const string EnableChangeTestCaseWit = "WorkItemTracking.Server.EnableChangeTestCaseWit";
    public const string EnablePicklistValueChangeAuditLog = "WorkItemTracking.Server.EnablePicklistValueChangeAuditLog";
    public const string WiqlDatabaseUpgradeTimeout = "WorkItemTracking.Server.WiqlDatabaseUpgradeTimeout";
    public const string OobPicklistFixDisabled = "WorkItemTracking.Server.OobPicklistFixDisabled";
    public const string ExecuteExtensionRulesFixDisabled = "WorkItemTracking.Server.ExecuteExtensionRulesFixDisabled";
    public const string ChangedFieldsInputValuesFixDisabled = "WorkItemTracking.Server.ChangedFieldsInputValuesFixDisabled";
    public const string ValidateUniqueDisplayNameFixDisabled = "WorkItemTracking.Server.ValidateUniqueDisplayNameFixDisabled";
    public const string ValidatePresenceAllowedValuesOfExistingFieldsDisabled = "WorkItemTracking.Server.ValidatePresenceAllowedValuesOfExistingFieldsDisabled";
    public const string ConstantsCaseIgnoringForXmlUpdateDisabled = "WorkItemTracking.Server.ConstantsCaseIgnoringForXmlUpdateDisabled";
    public const string SetClosedDateIfWITStateIsCompletedDisabled = "WorkItemTracking.Server.SetClosedDateIfWITStateIsCompletedDisabled";
    public const string EnableLinksOfDeletedAttachments = "WebAccess.WorkItemTracking.EnableLinksOfDeletedAttachments";
    public const string WorkItemsBulkDeleteDisabled = "WebAccess.WorkItemTracking.WorkItemsBulkDeleteDisabled";
    public const string ProjectLimitAreaPaths = "WorkItemTracking.Server.ProjectLimitAreaPaths";
    public const string ProjectLimitIterationPaths = "WorkItemTracking.Server.ProjectLimitIterationPaths";
    public const string ServicePrincipalsAllowed = "WebAccess.WorkItemTracking.ServicePrincipalsAllowed";
    public const string FixForAsOfFullTextQueriesEnabled = "WorkItemTracking.Server.FixForAsOfFullTextQueriesEnabled";
    public const string UsingRowNumberForSortingFixEnabled = "WorkItemTracking.Server.UsingRowNumberForSortingFixEnabled";
    public const string AadIdentityHelperFixEnabled = "WorkItemTracking.Server.AadIdentityHelperFixEnabled";
    public const string ClassificationNodeDataProviderExcludeUrlEnabled = "WorkItemTracking.Server.ClassificationNodeDataProviderExcludeUrlEnabled";
    public const string WorkItemTrackingEnableSecretsScanningAsync = "WorkItemTracking.Server.EnableSecretsScanningAsync";
    public const string WorkItemTrackingEnableSecretsScanning = "WorkItemTracking.Server.EnableSecretsScanning";
    public const string WorkItemTrackingEnableSecretsBlocking = "WorkItemTracking.Server.EnableSecretsBlocking";
    public const string WorkItemTrackingDisableSecretsPrescriptiveBlocking = "WorkItemTracking.Server.DisableSecretsPrescriptiveBlocking";
    public const string WorkItemTrackingBypassSecretsScanning = "WorkItemTracking.Server.BypassSecretsScanning";
    public const string WorkItemTrackingReadFromReadReplica = "WorkItemTracking.Server.ReadFromReadReplica";
    public const string CommandsForcedReadFromReadReplica = "WorkItemTracking.Server.Commands.ForcedReadFromReadReplica";
    public const string ServiceHooksLimitPaloadRelations = "ServiceHooks.Payload.LimitWorkItemRelations";
    public const string QueryItemServiceNewAPI = "WorkItemTracking.Server.QueryItemService.NewAPI";
    public const string ProcessHierarchy = "WebAccess.Process.Hierarchy";
    public const string ProcessFieldAssociationEnabled = "WorkItemTracking.Server.ProcessFieldAssociationEnabled";
    public const string ProcessUpload = "WebAccess.Process.ProcessUpload";
    public const string XmlTemplateProcess = "WebAccess.Process.XmlTemplateProcess";
    public const string DisableForceSyncAdObjects = "WorkItemTracking.Server.DisableForceSyncAdObjects";
    public const string SkipFieldCacheResetOnImport = "WorkItemTracking.Server.SkipFieldCacheResetOnImport";
    public const string ReadingFromEleadTablesEnabled = "WorkItemTracking.Server.ReadingFromEleadTablesEnabled";
    public const string WritingToEleadTablesEnabled = "WorkItemTracking.Server.WritingToEleadTablesEnabled";
    public const string CleaningACEsOnDeletingNodesDisabled = "WorkItemTracking.Server.CleaningACEsOnDeletingNodesDisabled";
    public const string SupportNoHistoryEnabledFields = "WorkItemTracking.Server.SupportNoHistoryEnabledFields";
    public const string TrackRecentActivity = "WorkItemTracking.RecentActivity.TrackRecentActivity";
    public const string TrackRecentProjectActivity = "WorkItemTracking.RecentActivity.TrackRecentProjectActivity";
    public const string PublishRecentActivityEventToServiceBus = "WorkItemTracking.Server.RecentActivity.PublishEventToServiceBus";
    public const string DisableProjectionLevelThreeForAll = "WorkItemTracking.Server.DisableProjectionLevelThreeForAll";
    public const string EnableProjectionLevelThreeForReportingAPIs = "WorkItemTracking.Server.EnableProjectionLevelThreeForReportingAPIs";
    public const string PriorityZeroEnabled = "WorkItemTracking.Server.Process.PriorityZeroEnabled";
    public const string RichClassificationNodeEvents = "WebAccess.WorkItemTracking.RichClassificationNodeEvents";
    public const string AllowInvalidLinkArtifacts = "WorkItemTracking.Server.WorkItem.AllowInvalidLinkArtifacts";
    public const string RecentMentionsMacro = "WorkItemTracking.Server.RecentMentionsMacro";
    public const string MyRecentActivityMacro = "WorkItemTracking.Server.MyRecentActivityMacro";
    public const string RecentProjectActivityMacro = "WorkItemTracking.Server.RecentProjectActivityMacro";
    public const string QueryMyWorkByExcludingDoneStates = "WorkItemTracking.Server.QueryMyWorkByExcludingDoneStates";
    public const string DoneColumnSortNullsLast = "WebAccess.Agile.DoneColumnSortNullsLast";
    public const string ProcessSettingsValidatorAllowCustomTeamField = "Agile.ProcessSettingsValidator.AllowCustomTeamField";
    public const string EnforceBypassRulesPermissionInClientOM = "WorkItemTracking.Server.EnforceBypassRulesPermissionInClientOM";
    public const string AnonymousAccess = "VisualStudio.Services.Identity.AnonymousAccess";
    public const string AllOobLinkTypesProvisioned = "WorkItemTracking.Server.AllOobLinkTypesProvisioned";
    public const string FakeWorkItemTypeIdForAllProjectsEnabled = "WorkItemTracking.Server.FakeWorkItemTypeIdForAllProjectsEnabled";
    public const string DisableReturningSuggestedFields = "WorkItemTracking.Server.DisableReturningSuggestedFields";
    public const string ProjectSupportsMarkdown = "WorkItemTracking.Server.ProjectSupportsMarkdown";
    public const string EnableHtmlFieldsMentions = "WebAccess.WorkItemTracking.EnableHtmlFieldsMentions";
    public const string DisableWorkItemStoreDataProviders = "WorkItemTracking.Server.DisableWorkItemStoreDataProviders";
    public const string DisableNewWorkItemZeroStateCleanUpBehavior = "WorkItemTracking.Server.DisableNewWorkItemZeroStateCleanUpBehavior";
    public const string UpdateDateIsSetInWorkitemUpdate = "WorkItemTracking.Server.CommentService.UpdateDateIsSetInWorkitemUpdate";
    public const string CommentServiceDualWriteEnabledF1 = "WorkItemTracking.Server.CommentService.DualWriteEnabled.F1";
    public const string CommentServiceDisableMigrationF2 = "WorkItemTracking.Server.CommentService.DisableMigration.F2";
    public const string CommentServiceEnableReadsFromNewStorageF3 = "WorkItemTracking.Server.CommentService.EnableReadsFromNewStorage.F3";
    public const string CommentServiceEnableEditAndDeleteF4 = "WorkItemTracking.Server.CommentService.EnableEditAndDelete.F4";
    public const string CommentServiceDisableLegacyWritesF5 = "WorkItemTracking.Server.CommentService.DisableLegacyWrites.F5";
    public const string EnableCommentReactions = "WorkItemTracking.Server.CommentService.EnableCommentReactions";
    public const string CommentServiceDisableFixRevisionMismatch = "WorkItemTracking.Server.CommentService.DisableFixRevisionMismatch";
    public const string MarkdownDiscussionEnabled = "WebAccess.WorkItemTracking.Form.MarkdownDiscussionEnabled";
    public const string EnableNewCommentsStorage = "WorkItemTracking.Server.CommentService.LegacyCommentsApiOnNewStorage";
    public const string CommentFactoryMentionPerfFixDisabled = "WorkItemTracking.Server.CommentFactoryMentionPerfFixDisabled";
    public const string DataImportOverride_PermitForNotRules = "PermitForNotRules";
    public const string DisableResetSequenceId = "WorkItemTracking.ReparentCollection.DisableResetSequenceId";
    public const string OnPremisesProcessInheritance = "WorkItemTracking.Server.OnPremisesProcessInheritance";
    public const string AzureBoardsBotWILinkingFeedback = "WorkItemTracking.Server.AzureBoardsBotWILinkingFeedback";
    public const string AzureBoardsBotIsBotFixDisabled = "WorkItemTracking.Server.AzureBoardsBotIsBotFixDisabled";
    public const string TakeFreshGithubPRInfoAlwaysEnabled = "WorkItemTracking.Server.TakeFreshGithubPRInfoAlwaysEnabled";
    public const string AdvancedGitHubPrExperienceEnabled = "WebPlatform.WorkItemTracking.AdvancedGitHubPrExperienceEnabled";
    public const string GitHubPrCheckRunInfoEnabled = "WorkItemTracking.Server.GitHubPrCheckRunInfoEnabled";
    public const string DisableSendMailDataProvider = "WorkItemTracking.Server.DisableSendMailDataProvider";
    public const string DisableWorkItemTrackingStateColors = "WebAccess.WorkItemTracking.Safeguard.DisableWorkItemStateColors";
    public const string TeamServiceSafeguardBlockCreateTeamAndAdminOperations = "TeamService.Safeguard.BlockCreateTeamAndAdminOperations";
    public const string TeamConfigurationServiceLimitTeamAreaPathsAndIterations = "TeamConfigurationService.SaveTeamFields.LimitAllowedTeamAreaPathsAndIterations";
    public const string WorkItemTrackingVisualizeFollows = "WebAccess.WorkItemTracking.VisualizeFollows";
    public const string WebAccessWorkItemTrackingDisableCommentCount = "WebAccess.WorkItemTracking.WorkItemsHub.DisableCommentCount";
    public const string RestrictMaxRevisionsSupportedByGetWorkItemHistoryDisabled = "WorkItemTracking.Server.RestrictMaxRevisionsSupportedByGetWorkItemHistoryDisabled";
    public const string DisableInjectExcludeGroupRule = "WorkItemTracking.Server.DisableInjectExcludeGroupForCustomWorkItemType";
    public const string DisableInlineAttachmentException = "WorkItemTracking.Server.DisableInlineAttachmentException";
    public const string DisableDetailsLocalizationFix = "WorkItemTracking.Server.DisableDetailsLocalizationFix";
    public const string AlwaysCheckNonEmptyInFullTextTempTable = "WorkItemTracking.Server.Queries.M197.AlwaysCheckNonEmptyInFullTextTempTable";
    public const string DisableLinkTypeDuplicatesIgnore = "WorkItemTracking.Server.DisableLinkTypeDuplicatesIgnore";
    public const string UseNewWorkItemUpdateIdGenerationEnabled = "WorkItemTracking.Server.UseNewWorkItemUpdateIdGenerationEnabled";
    public const string TeamAutomationRules = "WebAccess.WorkItemTracking.Backlog.TeamAutomationRulesEnabled";
    public const string HistoryLinksOptimizationsEnabled = "WebAccess.WorkItemTracking.HistoryLinksOptimizationsEnabled";
    public const string TopWorkItemQueryingJobDisabled = "WorkItemTracking.Server.TopWorkItemQueryingJobDisabled";

    public static bool IsDebugValidUserGroupEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.DebugValidUserGroup");

    public static bool IsResetSequenceIdDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.ReparentCollection.DisableResetSequenceId");

    public static bool IsQueryMyWorkByExcludingDoneStatesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.QueryMyWorkByExcludingDoneStates");

    public static bool IsAllowNonClusteredColumnstoreIndexEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.AllowNonClusteredColumnstoreIndex");

    public static bool IsProjectChangeProcessEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.ProjectChangeProcessDisabled");

    public static bool IsSetValueAndReadOnlyRuleEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.SetValueAndReadOnlyRuleDisabled");

    public static bool IsForNotReadOnlyRuleDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.ForNotReadOnlyRuleDisabled");

    public static bool IsQueryAutoOptimizationEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.AutoOptimization");

    public static bool IsQueryAutoOptimizationFuzzMatchEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.AutoOptimization.DisableFuzzMatch");

    public static bool IsOverrideQueryAutoOptimizationOrderEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.AutoOptimization.OverrideOptimizationOrder");

    public static bool IsFilterDuplicateQueryExecutionHistoriesEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.AutoOptimization.FilterDuplicateQueryExecutionHistories");
    }

    public static bool IsResetNonOptimizableQueriesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.AutoOptimization.ResetNonOptimizableQueries");

    public static bool IsOobPicklistFixEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.OobPicklistFixDisabled");

    public static bool IsChangedFieldsInputValuesFixEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.ChangedFieldsInputValuesFixDisabled");

    public static bool IsExecuteExtensionRulesFixEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.ExecuteExtensionRulesFixDisabled");

    public static bool IsValidateUniqueDisplayNameFixEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.ValidateUniqueDisplayNameFixDisabled");

    public static bool IsValidateAllowedValuesOfExistingFieldsPresenceEnabled(
      IVssRequestContext requestContext)
    {
      return !requestContext.IsFeatureEnabled("WorkItemTracking.Server.ValidatePresenceAllowedValuesOfExistingFieldsDisabled");
    }

    public static bool IsHostedXMLProject(IVssRequestContext requestContext, Guid projectId)
    {
      ProcessDescriptor processDescriptor;
      return requestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(requestContext, projectId, out processDescriptor) && processDescriptor.IsCustom;
    }

    public static bool IsProcessAdminServiceFieldPrefixLegacyBehaviorEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.ProcessAdminServiceFieldPrefixLegacyBehaviorEnabled");
    }

    public static bool IsTeamProjectMoveEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") || requestContext.IsFeatureEnabled("WorkItemTracking.Server.MoveWorkItems");

    public static bool IsChangeWorkItemTypeEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") || requestContext.IsFeatureEnabled("WorkItemTracking.Server.ChangeWorkItemType");

    public static bool IsProcessCustomizationEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy");

    public static bool IsProcessFieldAssociationEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.ProcessFieldAssociationEnabled");

    public static bool IsXmlTemplateProcessEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Process.XmlTemplateProcess");

    public static bool IsRecentMentionsMacroEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.RecentMentionsMacro");

    public static bool IsMyRecentActivityMacroEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.MyRecentActivityMacro");

    public static bool IsRecentProjectActivityMacroEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.RecentProjectActivityMacro");

    public static bool IsSharedProcessEnabled(IVssRequestContext requestContext) => WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) || WorkItemTrackingFeatureFlags.AreXMLProcessesEnabled(requestContext);

    public static bool IsProjectionLevelThreeDisabledForAll(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableProjectionLevelThreeForAll");

    public static bool IsProjectionLevelThreeForReportingAPIsEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableProjectionLevelThreeForReportingAPIs");
    }

    public static bool IsRicherClassificationNodeChangeEventEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.RichClassificationNodeEvents");
    }

    public static bool IsUsingQueryItemServiceNewAPI(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.QueryItemService.NewAPI");
    }

    public static bool IsNoHistoryEnabledFieldsSupported(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.SupportNoHistoryEnabledFields");

    public static bool ShouldTrackRecentActivity(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.RecentActivity.TrackRecentActivity");

    public static bool ShouldTrackRecentProjectActivity(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.RecentActivity.TrackRecentProjectActivity");

    public static bool ShouldPublishRecentActivityEventToServiceBus(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.RecentActivity.PublishEventToServiceBus");
    }

    public static void CheckProcessCustomizationEnabled(IVssRequestContext requestContext)
    {
      if (!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
        throw new FeatureDisabledException(FrameworkResources.FeatureDisabledError());
    }

    public static bool AreXMLProcessesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload");

    public static bool IsInheritedProcessCustomizationOnlyAccount(IVssRequestContext requestContext) => !WorkItemTrackingFeatureFlags.AreXMLProcessesEnabled(requestContext) && WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);

    internal static bool IsFullRuleGenerationEnabled(IVssRequestContext requestContext) => WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && WorkItemTrackingFeatureFlags.IsInheritedProcessCustomizationOnlyAccount(requestContext) && WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);

    public static bool IsPartialRuleGenerationEnabled(IVssRequestContext requestContext) => WorkItemTrackingFeatureFlags.AreXMLProcessesEnabled(requestContext) && WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);

    internal static bool IsFullOrPartialRuleGenerationEnabled(IVssRequestContext requestContext) => WorkItemTrackingFeatureFlags.IsFullRuleGenerationEnabled(requestContext) || WorkItemTrackingFeatureFlags.IsPartialRuleGenerationEnabled(requestContext);

    public static bool IsVisualStudio(IVssRequestContext requestContext)
    {
      string userAgent = requestContext.UserAgent;
      return !string.IsNullOrEmpty(userAgent) && userAgent.IndexOf("devenv.exe", StringComparison.InvariantCultureIgnoreCase) >= 0;
    }

    internal static bool IsPriorityZeroEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.Process.PriorityZeroEnabled");

    internal static void CheckLegacyProcessUpdateInCustomizationModeEnabled(
      IVssRequestContext requestContext)
    {
      if (WorkItemTrackingFeatureFlags.IsSharedProcessEnabled(requestContext))
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("MethodName", requestContext.Method?.Name);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WitAdmin", "WitAdminBlock", properties);
        throw new FeatureDisabledException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.LegacyProcessUpdateBlocked());
      }
    }

    public static bool IsDoneColumnSortNullsLastEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Agile.DoneColumnSortNullsLast");

    public static bool IsReclassifyWorkItemsRaceConditionFixEnabled(
      IVssRequestContext requestContext)
    {
      return !requestContext.IsFeatureEnabled("WorkItemTracking.Server.ReclassifyWorkItemsRaceConditionFixDisabled");
    }

    public static bool IsWorkItemProjectFieldModelFactoryPerfFixEnabled(
      IVssRequestContext requestContext)
    {
      return !requestContext.IsFeatureEnabled("WorkItemTracking.Server.WorkItemProjectFieldModelFactoryPerfFixDisabled");
    }

    public static bool IsProjectCreationStampDbEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.ProjectCreationStampDbDisabled");

    public static bool IsReadingFromEleadTablesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.ReadingFromEleadTablesEnabled");

    public static bool IsWritingToEleadTablesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.WritingToEleadTablesEnabled");

    public static bool IsCleaningACEsOnDeletingNodesEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.CleaningACEsOnDeletingNodesDisabled");

    public static bool GenerateWorkItemURLsWithProjectContext(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.WorkItemURLsWithProjectContext");

    public static bool IsEnforceBypassRulesPermissionInClientOM(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnforceBypassRulesPermissionInClientOM");

    public static bool IsAllOobLinkTypesProvisioned(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.AllOobLinkTypesProvisioned");

    public static bool IsBackfillingCommentCountEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableBackfillingCommentCount");

    public static bool IsCommentServiceDualWriteEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentService.DualWriteEnabled.F1");

    public static bool IsCommentServiceMigrationEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentService.DisableMigration.F2");

    public static bool IsCommentServiceReadsFromNewStorageEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentService.EnableReadsFromNewStorage.F3");

    public static bool IsCommentServiceEditAndDeleteEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentService.EnableEditAndDelete.F4");

    public static bool IsUpdateDateInWorkitemUpdateEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentService.UpdateDateIsSetInWorkitemUpdate");

    public static bool IsProjectSupportsMarkdownEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.ProjectSupportsMarkdown");

    public static bool IsHtmlFieldsMentionsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.EnableHtmlFieldsMentions");

    public static bool IsCommentServiceLegacyWritesEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentService.DisableLegacyWrites.F5");

    public static bool IsCommentServiceFixRevisionMismatchEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentService.DisableFixRevisionMismatch");

    public static bool IsOnPremisesProcessInheritanceEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.OnPremisesProcessInheritance");

    public static bool IsLegacyCommentsApiNewStorageEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentService.LegacyCommentsApiOnNewStorage");

    public static bool IsInjectExcludeGroupRuleEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableInjectExcludeGroupForCustomWorkItemType");

    public static bool IsInlineAttachmentExceptionEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableInlineAttachmentException");

    public static bool IsRestrictMaxRevisionsSupportedByGetWorkItemHistoryEnabled(
      IVssRequestContext requestContext)
    {
      return !requestContext.IsFeatureEnabled("WorkItemTracking.Server.RestrictMaxRevisionsSupportedByGetWorkItemHistoryDisabled");
    }

    public static bool IsDanglingLinkDeletionDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableDeletionOfDanglingLinks");

    public static bool IsFieldsExistInWITCheckEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableFieldsExistInWITCheck");

    public static bool IsChangeTestCaseWitEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableChangeTestCaseWit");

    public static bool IsPicklistValueChangeAuditLogEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnablePicklistValueChangeAuditLog");

    public static bool IsDeletePicklistWhenNotReferencedByAnyFieldEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableDeletePicklistWhenNotReferencedByAnyField");
    }

    public static bool IsInheritedProcessValidationOnProjectCreateDisabled(
      IVssRequestContext requestContext)
    {
      return !requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableInheritedProcessValidationOnProjectCreate");
    }

    public static bool IsWorkItemTrackingAsyncSecretsScanningEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableSecretsScanningAsync") && requestContext.ExecutionEnvironment.IsHostedDeployment;
    }

    public static bool IsWorkItemTrackingSecretsScanningEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableSecretsScanning") && requestContext.ExecutionEnvironment.IsHostedDeployment;

    public static bool IsWorkItemTrackingSecretsBlockingEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.EnableSecretsBlocking");

    public static bool IsWorkItemTrackingSecretsPrescriptiveBlockingDisabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableSecretsPrescriptiveBlocking");
    }

    public static bool IsWorkItemTrackingSecretsBypassScanningEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.BypassSecretsScanning") && requestContext.ExecutionEnvironment.IsHostedDeployment;
    }

    public static bool IsDetailsLocalizationFixEnabled(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableDetailsLocalizationFix");

    public static bool IsDisableSendMailDataProviderEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableSendMailDataProvider");

    public static bool IsAlwaysCheckNonEmptyInFullTextTempTableEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.M197.AlwaysCheckNonEmptyInFullTextTempTable");
    }

    public static bool IsConstantsCaseIgnoredForXmlUpdate(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.ConstantsCaseIgnoringForXmlUpdateDisabled");

    public static bool IsSetClosedDateIfWITStateIsCompletedEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.SetClosedDateIfWITStateIsCompletedDisabled");

    public static bool IsDisabledLinksOfDeletedAttachments(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.EnableLinksOfDeletedAttachments");

    public static bool IsWorkItemsBulkDeleteDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.WorkItemsBulkDeleteDisabled");

    public static bool IsMarkdownDiscussionEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.Form.MarkdownDiscussionEnabled");

    public static bool MetadataNewDummyRowsEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.MetadataNewDummyRowsDisabled");

    public static bool IsLimitAreaPathsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.ProjectLimitAreaPaths");

    public static bool IsLimitIterationPathsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.ProjectLimitIterationPaths");

    public static bool AreServicePrincipalsAllowed(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.ServicePrincipalsAllowed");

    public static bool IsTeamAutomationRulesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.Backlog.TeamAutomationRulesEnabled");

    public static bool IsHistoryLinksOptimizationsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.HistoryLinksOptimizationsEnabled");

    public static bool IsAzureBoardsBotWILinkingFeedbackEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.AzureBoardsBotWILinkingFeedback");

    public static bool IsTopWorkItemQueryingJobDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.TopWorkItemQueryingJobDisabled");

    public static bool IsCommentFactoryMentionPerfFixEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.CommentFactoryMentionPerfFixDisabled");

    public static bool IsSaveQueryExecutionInformationSprocV2Enabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.SaveQueryExecutionInformationSprocV2Enabled");
    }

    public static bool IsFixForAsOfFullTextQueriesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.FixForAsOfFullTextQueriesEnabled");

    public static bool IsIsBotFixEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("WorkItemTracking.Server.AzureBoardsBotIsBotFixDisabled");

    public static bool IsTakeFreshGithubPRInfoAlwaysEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.TakeFreshGithubPRInfoAlwaysEnabled");

    public static bool IsAdvancedGitHubPrExperienceEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebPlatform.WorkItemTracking.AdvancedGitHubPrExperienceEnabled");

    public static bool IsGitHubPrCheckRunInfoEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.GitHubPrCheckRunInfoEnabled");

    public static bool IsUsingRowNumberForSortingFixEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.UsingRowNumberForSortingFixEnabled");

    public static bool IsAadIdentityHelperFixEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.AadIdentityHelperFixEnabled");

    public static bool IsQueryExecutionInformationSprocDisabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.Queries.QueryExecutionInformationSprocDisabled");

    public static bool IsNewWorkItemZeroStateCleanUpBehaviorDisabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableNewWorkItemZeroStateCleanUpBehavior");
    }

    public static bool IsNewWorkItemUpdateIdGenerationEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.UseNewWorkItemUpdateIdGenerationEnabled");
  }
}
