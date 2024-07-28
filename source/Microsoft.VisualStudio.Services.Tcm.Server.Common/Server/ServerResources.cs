// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ServerResources
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ServerResources.resourceMan == null)
          ServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.TestManagement.Server.ServerResources", typeof (ServerResources).Assembly);
        return ServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ServerResources.resourceCulture;
      set => ServerResources.resourceCulture = value;
    }

    internal static string AccessDeniedExceptionMessage => ServerResources.ResourceManager.GetString(nameof (AccessDeniedExceptionMessage), ServerResources.resourceCulture);

    internal static string ActionResultNotFound => ServerResources.ResourceManager.GetString(nameof (ActionResultNotFound), ServerResources.resourceCulture);

    internal static string ActiveTestConfigurationsNotFound => ServerResources.ResourceManager.GetString(nameof (ActiveTestConfigurationsNotFound), ServerResources.resourceCulture);

    internal static string AmbiguousDate => ServerResources.ResourceManager.GetString(nameof (AmbiguousDate), ServerResources.resourceCulture);

    internal static string ApiDisallowMultiplePointIdsMessage => ServerResources.ResourceManager.GetString(nameof (ApiDisallowMultiplePointIdsMessage), ServerResources.resourceCulture);

    internal static string ApiDisallowMultipleTestCaseIdsMessage => ServerResources.ResourceManager.GetString(nameof (ApiDisallowMultipleTestCaseIdsMessage), ServerResources.resourceCulture);

    internal static string ArtifactTitle => ServerResources.ResourceManager.GetString(nameof (ArtifactTitle), ServerResources.resourceCulture);

    internal static string AutomatedTestNameNotSpecifiedError => ServerResources.ResourceManager.GetString(nameof (AutomatedTestNameNotSpecifiedError), ServerResources.resourceCulture);

    internal static string AutomatedTestParameterSpecifiedForManualRunError => ServerResources.ResourceManager.GetString(nameof (AutomatedTestParameterSpecifiedForManualRunError), ServerResources.resourceCulture);

    internal static string AutomationStatusMismatch => ServerResources.ResourceManager.GetString(nameof (AutomationStatusMismatch), ServerResources.resourceCulture);

    internal static string BothSuiteAndPointSpecifiedInQuery => ServerResources.ResourceManager.GetString(nameof (BothSuiteAndPointSpecifiedInQuery), ServerResources.resourceCulture);

    internal static string BuildIdAndPipelineIdAreNotEqual => ServerResources.ResourceManager.GetString(nameof (BuildIdAndPipelineIdAreNotEqual), ServerResources.resourceCulture);

    internal static string BuildIdDefinitionAmbigious => ServerResources.ResourceManager.GetString(nameof (BuildIdDefinitionAmbigious), ServerResources.resourceCulture);

    internal static string BuildIdNotSpecifiedWithPlatformFlavor => ServerResources.ResourceManager.GetString(nameof (BuildIdNotSpecifiedWithPlatformFlavor), ServerResources.resourceCulture);

    internal static string BuildNotFound => ServerResources.ResourceManager.GetString(nameof (BuildNotFound), ServerResources.resourceCulture);

    internal static string BuildNotFoundError => ServerResources.ResourceManager.GetString(nameof (BuildNotFoundError), ServerResources.resourceCulture);

    internal static string BuildsNotFound => ServerResources.ResourceManager.GetString(nameof (BuildsNotFound), ServerResources.resourceCulture);

    internal static string BulkMarkRunTitle => ServerResources.ResourceManager.GetString(nameof (BulkMarkRunTitle), ServerResources.resourceCulture);

    internal static string BulkUpdateOfTestPointFailed => ServerResources.ResourceManager.GetString(nameof (BulkUpdateOfTestPointFailed), ServerResources.resourceCulture);

    internal static string BulkUpdateResultApiMaxLimitError => ServerResources.ResourceManager.GetString(nameof (BulkUpdateResultApiMaxLimitError), ServerResources.resourceCulture);

    internal static string CannotAccessTfsSubsystem => ServerResources.ResourceManager.GetString(nameof (CannotAccessTfsSubsystem), ServerResources.resourceCulture);

    internal static string CannotAddDuplicateTestCaseToSuite => ServerResources.ResourceManager.GetString(nameof (CannotAddDuplicateTestCaseToSuite), ServerResources.resourceCulture);

    internal static string CannotCancelAlreadyCompletedTestRun => ServerResources.ResourceManager.GetString(nameof (CannotCancelAlreadyCompletedTestRun), ServerResources.resourceCulture);

    internal static string CannotCreateTeamProject => ServerResources.ResourceManager.GetString(nameof (CannotCreateTeamProject), ServerResources.resourceCulture);

    internal static string CannotCreateWorkItemInInvalidState => ServerResources.ResourceManager.GetString(nameof (CannotCreateWorkItemInInvalidState), ServerResources.resourceCulture);

    internal static string CannotDeleteTeamProject => ServerResources.ResourceManager.GetString(nameof (CannotDeleteTeamProject), ServerResources.resourceCulture);

    internal static string CannotDeleteTestResults => ServerResources.ResourceManager.GetString(nameof (CannotDeleteTestResults), ServerResources.resourceCulture);

    internal static string CannotEditWorkItems => ServerResources.ResourceManager.GetString(nameof (CannotEditWorkItems), ServerResources.resourceCulture);

    internal static string CannotManageTestConfigurations => ServerResources.ResourceManager.GetString(nameof (CannotManageTestConfigurations), ServerResources.resourceCulture);

    internal static string CannotManageTestControllers => ServerResources.ResourceManager.GetString(nameof (CannotManageTestControllers), ServerResources.resourceCulture);

    internal static string CannotManageTestEnvironments => ServerResources.ResourceManager.GetString(nameof (CannotManageTestEnvironments), ServerResources.resourceCulture);

    internal static string CannotManageTestPlans => ServerResources.ResourceManager.GetString(nameof (CannotManageTestPlans), ServerResources.resourceCulture);

    internal static string CannotManageTestSuites => ServerResources.ResourceManager.GetString(nameof (CannotManageTestSuites), ServerResources.resourceCulture);

    internal static string CannotMoveChildSuiteBelow => ServerResources.ResourceManager.GetString(nameof (CannotMoveChildSuiteBelow), ServerResources.resourceCulture);

    internal static string CannotMoveSuiteEntriesAcrossPlans => ServerResources.ResourceManager.GetString(nameof (CannotMoveSuiteEntriesAcrossPlans), ServerResources.resourceCulture);

    internal static string CannotPublishTestResults => ServerResources.ResourceManager.GetString(nameof (CannotPublishTestResults), ServerResources.resourceCulture);

    internal static string CannotReadProject => ServerResources.ResourceManager.GetString(nameof (CannotReadProject), ServerResources.resourceCulture);

    internal static string CannotViewTestResults => ServerResources.ResourceManager.GetString(nameof (CannotViewTestResults), ServerResources.resourceCulture);

    internal static string CannotViewWorkItems => ServerResources.ResourceManager.GetString(nameof (CannotViewWorkItems), ServerResources.resourceCulture);

    internal static string CannotWriteProject => ServerResources.ResourceManager.GetString(nameof (CannotWriteProject), ServerResources.resourceCulture);

    internal static string ChartDimensionConfiguration => ServerResources.ResourceManager.GetString(nameof (ChartDimensionConfiguration), ServerResources.resourceCulture);

    internal static string ChartDimensionFailureType => ServerResources.ResourceManager.GetString(nameof (ChartDimensionFailureType), ServerResources.resourceCulture);

    internal static string ChartDimensionOutcome => ServerResources.ResourceManager.GetString(nameof (ChartDimensionOutcome), ServerResources.resourceCulture);

    internal static string ChartDimensionPriority => ServerResources.ResourceManager.GetString(nameof (ChartDimensionPriority), ServerResources.resourceCulture);

    internal static string ChartDimensionResolution => ServerResources.ResourceManager.GetString(nameof (ChartDimensionResolution), ServerResources.resourceCulture);

    internal static string ChartDimensionRunBy => ServerResources.ResourceManager.GetString(nameof (ChartDimensionRunBy), ServerResources.resourceCulture);

    internal static string ChartDimensionRunType => ServerResources.ResourceManager.GetString(nameof (ChartDimensionRunType), ServerResources.resourceCulture);

    internal static string ChartDimensionRunTypeAutomated => ServerResources.ResourceManager.GetString(nameof (ChartDimensionRunTypeAutomated), ServerResources.resourceCulture);

    internal static string ChartDimensionRunTypeManual => ServerResources.ResourceManager.GetString(nameof (ChartDimensionRunTypeManual), ServerResources.resourceCulture);

    internal static string ChartDimensionSuite => ServerResources.ResourceManager.GetString(nameof (ChartDimensionSuite), ServerResources.resourceCulture);

    internal static string ChartDimensionTester => ServerResources.ResourceManager.GetString(nameof (ChartDimensionTester), ServerResources.resourceCulture);

    internal static string ChartDimensionValueNone => ServerResources.ResourceManager.GetString(nameof (ChartDimensionValueNone), ServerResources.resourceCulture);

    internal static string ChartDimensionValueNotAvailable => ServerResources.ResourceManager.GetString(nameof (ChartDimensionValueNotAvailable), ServerResources.resourceCulture);

    internal static string ChartDimensionValueNotRun => ServerResources.ResourceManager.GetString(nameof (ChartDimensionValueNotRun), ServerResources.resourceCulture);

    internal static string ChartMeasureTestCases => ServerResources.ResourceManager.GetString(nameof (ChartMeasureTestCases), ServerResources.resourceCulture);

    internal static string ChartMeasureTests => ServerResources.ResourceManager.GetString(nameof (ChartMeasureTests), ServerResources.resourceCulture);

    internal static string CloneAcrossProjectsNotSupported => ServerResources.ResourceManager.GetString(nameof (CloneAcrossProjectsNotSupported), ServerResources.resourceCulture);

    internal static string CloneOperationJobName => ServerResources.ResourceManager.GetString(nameof (CloneOperationJobName), ServerResources.resourceCulture);

    internal static string CloneOperationNotFoundForPlan => ServerResources.ResourceManager.GetString(nameof (CloneOperationNotFoundForPlan), ServerResources.resourceCulture);

    internal static string CloneOperationNotFoundForSuite => ServerResources.ResourceManager.GetString(nameof (CloneOperationNotFoundForSuite), ServerResources.resourceCulture);

    internal static string CloneOperationNotFoundForTestCase => ServerResources.ResourceManager.GetString(nameof (CloneOperationNotFoundForTestCase), ServerResources.resourceCulture);

    internal static string CloneRequirementsNotSupported => ServerResources.ResourceManager.GetString(nameof (CloneRequirementsNotSupported), ServerResources.resourceCulture);

    internal static string ControllerNotSetForAutomatedRun => ServerResources.ResourceManager.GetString(nameof (ControllerNotSetForAutomatedRun), ServerResources.resourceCulture);

    internal static string ControllerSetForNonAutomatedRun => ServerResources.ResourceManager.GetString(nameof (ControllerSetForNonAutomatedRun), ServerResources.resourceCulture);

    internal static string CopySuiteEntries_NameTemplate1 => ServerResources.ResourceManager.GetString(nameof (CopySuiteEntries_NameTemplate1), ServerResources.resourceCulture);

    internal static string CopySuiteEntries_NameTemplate2 => ServerResources.ResourceManager.GetString(nameof (CopySuiteEntries_NameTemplate2), ServerResources.resourceCulture);

    internal static string CouldNotFindAValidPlanStateMapping => ServerResources.ResourceManager.GetString(nameof (CouldNotFindAValidPlanStateMapping), ServerResources.resourceCulture);

    internal static string CouldNotFindAValidSuiteStateMapping => ServerResources.ResourceManager.GetString(nameof (CouldNotFindAValidSuiteStateMapping), ServerResources.resourceCulture);

    internal static string CouldNotFindProperStateMappingInProcessConfig => ServerResources.ResourceManager.GetString(nameof (CouldNotFindProperStateMappingInProcessConfig), ServerResources.resourceCulture);

    internal static string CouldNotFindStateMappingsInProjectProcessConfig => ServerResources.ResourceManager.GetString(nameof (CouldNotFindStateMappingsInProjectProcessConfig), ServerResources.resourceCulture);

    internal static string CouldNotFindStatesForWorkItemType => ServerResources.ResourceManager.GetString(nameof (CouldNotFindStatesForWorkItemType), ServerResources.resourceCulture);

    internal static string CouldNotFindTransitionsForWorkItemType => ServerResources.ResourceManager.GetString(nameof (CouldNotFindTransitionsForWorkItemType), ServerResources.resourceCulture);

    internal static string CouldNotFindValidStartStateForWorkItemType => ServerResources.ResourceManager.GetString(nameof (CouldNotFindValidStartStateForWorkItemType), ServerResources.resourceCulture);

    internal static string CreateSuiteInsideNonStaticSuite => ServerResources.ResourceManager.GetString(nameof (CreateSuiteInsideNonStaticSuite), ServerResources.resourceCulture);

    internal static string CreateTestResults_InvalidData => ServerResources.ResourceManager.GetString(nameof (CreateTestResults_InvalidData), ServerResources.resourceCulture);

    internal static string CreateTestResults_InvalidData_WithArgument => ServerResources.ResourceManager.GetString(nameof (CreateTestResults_InvalidData_WithArgument), ServerResources.resourceCulture);

    internal static string CreateTestRunStateInvalidMessage => ServerResources.ResourceManager.GetString(nameof (CreateTestRunStateInvalidMessage), ServerResources.resourceCulture);

    internal static string DatabaseOperationNotSupportedWhileUpgrade => ServerResources.ResourceManager.GetString(nameof (DatabaseOperationNotSupportedWhileUpgrade), ServerResources.resourceCulture);

    internal static string DatabaseVersionOnlySupportedForServicing => ServerResources.ResourceManager.GetString(nameof (DatabaseVersionOnlySupportedForServicing), ServerResources.resourceCulture);

    internal static string DataCollectorAlreadyExists => ServerResources.ResourceManager.GetString(nameof (DataCollectorAlreadyExists), ServerResources.resourceCulture);

    internal static string DataCollectorNotFound => ServerResources.ResourceManager.GetString(nameof (DataCollectorNotFound), ServerResources.resourceCulture);

    internal static string DateRangeInValid => ServerResources.ResourceManager.GetString(nameof (DateRangeInValid), ServerResources.resourceCulture);

    internal static string DeepCopy_TargetProjectHasNoRequirementCategory => ServerResources.ResourceManager.GetString(nameof (DeepCopy_TargetProjectHasNoRequirementCategory), ServerResources.resourceCulture);

    internal static string DeepCopy_TargetProjectHasNoTestCaseCategory => ServerResources.ResourceManager.GetString(nameof (DeepCopy_TargetProjectHasNoTestCaseCategory), ServerResources.resourceCulture);

    internal static string DeepCopy_TreePathMustForCloneAcrossProjects => ServerResources.ResourceManager.GetString(nameof (DeepCopy_TreePathMustForCloneAcrossProjects), ServerResources.resourceCulture);

    internal static string DeepCopy_TreePathNotInProject => ServerResources.ResourceManager.GetString(nameof (DeepCopy_TreePathNotInProject), ServerResources.resourceCulture);

    internal static string DeepCopyFailedForRequirement => ServerResources.ResourceManager.GetString(nameof (DeepCopyFailedForRequirement), ServerResources.resourceCulture);

    internal static string DeepCopyFailedForTestCase => ServerResources.ResourceManager.GetString(nameof (DeepCopyFailedForTestCase), ServerResources.resourceCulture);

    internal static string DeepCopyFailedWithWITError => ServerResources.ResourceManager.GetString(nameof (DeepCopyFailedWithWITError), ServerResources.resourceCulture);

    internal static string DeepCopyFieldNotFound => ServerResources.ResourceManager.GetString(nameof (DeepCopyFieldNotFound), ServerResources.resourceCulture);

    internal static string DeepCopyFieldOverriddenFieldNameEmpty => ServerResources.ResourceManager.GetString(nameof (DeepCopyFieldOverriddenFieldNameEmpty), ServerResources.resourceCulture);

    internal static string DeepCopyFieldOverriddenMultipleTimes => ServerResources.ResourceManager.GetString(nameof (DeepCopyFieldOverriddenMultipleTimes), ServerResources.resourceCulture);

    internal static string DeepCopyFieldValueInvalidDataType => ServerResources.ResourceManager.GetString(nameof (DeepCopyFieldValueInvalidDataType), ServerResources.resourceCulture);

    internal static string DeepCopyFieldValueMissing => ServerResources.ResourceManager.GetString(nameof (DeepCopyFieldValueMissing), ServerResources.resourceCulture);

    internal static string DeepCopyFieldValueTooLong => ServerResources.ResourceManager.GetString(nameof (DeepCopyFieldValueTooLong), ServerResources.resourceCulture);

    internal static string DeepCopyInvalidOperationId => ServerResources.ResourceManager.GetString(nameof (DeepCopyInvalidOperationId), ServerResources.resourceCulture);

    internal static string DeepCopyInvalidPath => ServerResources.ResourceManager.GetString(nameof (DeepCopyInvalidPath), ServerResources.resourceCulture);

    internal static string DeepCopyLinkDefaultComment => ServerResources.ResourceManager.GetString(nameof (DeepCopyLinkDefaultComment), ServerResources.resourceCulture);

    internal static string DeepCopyNonEditableField => ServerResources.ResourceManager.GetString(nameof (DeepCopyNonEditableField), ServerResources.resourceCulture);

    internal static string DeepCopyPermissionError => ServerResources.ResourceManager.GetString(nameof (DeepCopyPermissionError), ServerResources.resourceCulture);

    internal static string DeepCopyRelatedLinkTooLong => ServerResources.ResourceManager.GetString(nameof (DeepCopyRelatedLinkTooLong), ServerResources.resourceCulture);

    internal static string DeepCopySharedStepXmlParseError => ServerResources.ResourceManager.GetString(nameof (DeepCopySharedStepXmlParseError), ServerResources.resourceCulture);

    internal static string DeepCopySourceAndDestinationCannotBeInSamePlan => ServerResources.ResourceManager.GetString(nameof (DeepCopySourceAndDestinationCannotBeInSamePlan), ServerResources.resourceCulture);

    internal static string DefaultConfigurationCreated => ServerResources.ResourceManager.GetString(nameof (DefaultConfigurationCreated), ServerResources.resourceCulture);

    internal static string DefaultFailureType_KnownIssue => ServerResources.ResourceManager.GetString(nameof (DefaultFailureType_KnownIssue), ServerResources.resourceCulture);

    internal static string DefaultFailureType_NewIssue => ServerResources.ResourceManager.GetString(nameof (DefaultFailureType_NewIssue), ServerResources.resourceCulture);

    internal static string DefaultFailureType_None => ServerResources.ResourceManager.GetString(nameof (DefaultFailureType_None), ServerResources.resourceCulture);

    internal static string DefaultFailureType_NullValue => ServerResources.ResourceManager.GetString(nameof (DefaultFailureType_NullValue), ServerResources.resourceCulture);

    internal static string DefaultFailureType_Regression => ServerResources.ResourceManager.GetString(nameof (DefaultFailureType_Regression), ServerResources.resourceCulture);

    internal static string DefaultFailureType_Unknown => ServerResources.ResourceManager.GetString(nameof (DefaultFailureType_Unknown), ServerResources.resourceCulture);

    internal static string DefaultFailureTypeCreationFailed => ServerResources.ResourceManager.GetString(nameof (DefaultFailureTypeCreationFailed), ServerResources.resourceCulture);

    internal static string DefaultStateNotMappedToInProgressInProjectProcessConfig => ServerResources.ResourceManager.GetString(nameof (DefaultStateNotMappedToInProgressInProjectProcessConfig), ServerResources.resourceCulture);

    internal static string DefaultTestConfigurationNotFound => ServerResources.ResourceManager.GetString(nameof (DefaultTestConfigurationNotFound), ServerResources.resourceCulture);

    internal static string DeletedAttachmentArtifact => ServerResources.ResourceManager.GetString(nameof (DeletedAttachmentArtifact), ServerResources.resourceCulture);

    internal static string DeletedResultArtifact => ServerResources.ResourceManager.GetString(nameof (DeletedResultArtifact), ServerResources.resourceCulture);

    internal static string DeletedSessionArtifactTitle => ServerResources.ResourceManager.GetString(nameof (DeletedSessionArtifactTitle), ServerResources.resourceCulture);

    internal static string DeleteInProgressResultSetForNotCompleted => ServerResources.ResourceManager.GetString(nameof (DeleteInProgressResultSetForNotCompleted), ServerResources.resourceCulture);

    internal static string DeleteTestPlanNotSupported => ServerResources.ResourceManager.GetString(nameof (DeleteTestPlanNotSupported), ServerResources.resourceCulture);

    internal static string DestinationPlanNotEmptyForPlanCloning => ServerResources.ResourceManager.GetString(nameof (DestinationPlanNotEmptyForPlanCloning), ServerResources.resourceCulture);

    internal static string DestinationWorkItemTypeNotFound => ServerResources.ResourceManager.GetString(nameof (DestinationWorkItemTypeNotFound), ServerResources.resourceCulture);

    internal static string DuplicateBranchFlakyStateUpdate => ServerResources.ResourceManager.GetString(nameof (DuplicateBranchFlakyStateUpdate), ServerResources.resourceCulture);

    internal static string DuplicateIdsExceptionMessage => ServerResources.ResourceManager.GetString(nameof (DuplicateIdsExceptionMessage), ServerResources.resourceCulture);

    internal static string DuplicateLinkErrorMessage => ServerResources.ResourceManager.GetString(nameof (DuplicateLinkErrorMessage), ServerResources.resourceCulture);

    internal static string DuplicateNameError => ServerResources.ResourceManager.GetString(nameof (DuplicateNameError), ServerResources.resourceCulture);

    internal static string DuplicateSuiteName => ServerResources.ResourceManager.GetString(nameof (DuplicateSuiteName), ServerResources.resourceCulture);

    internal static string DuplicateTestCaseId => ServerResources.ResourceManager.GetString(nameof (DuplicateTestCaseId), ServerResources.resourceCulture);

    internal static string DuplicateTestCaseInSuite => ServerResources.ResourceManager.GetString(nameof (DuplicateTestCaseInSuite), ServerResources.resourceCulture);

    internal static string DuplicateTestCasesInSuite => ServerResources.ResourceManager.GetString(nameof (DuplicateTestCasesInSuite), ServerResources.resourceCulture);

    internal static string DuplicateTestVariable => ServerResources.ResourceManager.GetString(nameof (DuplicateTestVariable), ServerResources.resourceCulture);

    internal static string DurationInMsError => ServerResources.ResourceManager.GetString(nameof (DurationInMsError), ServerResources.resourceCulture);

    internal static string ErrorQueuingJob => ServerResources.ResourceManager.GetString(nameof (ErrorQueuingJob), ServerResources.resourceCulture);

    internal static string ExtensionFieldsNotFoundError => ServerResources.ResourceManager.GetString(nameof (ExtensionFieldsNotFoundError), ServerResources.resourceCulture);

    internal static string FailedToCreateTestResultsForTestRun => ServerResources.ResourceManager.GetString(nameof (FailedToCreateTestResultsForTestRun), ServerResources.resourceCulture);

    internal static string FailureTypeKnownIssue => ServerResources.ResourceManager.GetString(nameof (FailureTypeKnownIssue), ServerResources.resourceCulture);

    internal static string FailureTypeNewIssue => ServerResources.ResourceManager.GetString(nameof (FailureTypeNewIssue), ServerResources.resourceCulture);

    internal static string FailureTypeNone => ServerResources.ResourceManager.GetString(nameof (FailureTypeNone), ServerResources.resourceCulture);

    internal static string FailureTypeRegression => ServerResources.ResourceManager.GetString(nameof (FailureTypeRegression), ServerResources.resourceCulture);

    internal static string FailureTypesAreInUse => ServerResources.ResourceManager.GetString(nameof (FailureTypesAreInUse), ServerResources.resourceCulture);

    internal static string FailureTypesCannotBeEmpty => ServerResources.ResourceManager.GetString(nameof (FailureTypesCannotBeEmpty), ServerResources.resourceCulture);

    internal static string FailureTypeUnknown => ServerResources.ResourceManager.GetString(nameof (FailureTypeUnknown), ServerResources.resourceCulture);

    internal static string FieldNotFound => ServerResources.ResourceManager.GetString(nameof (FieldNotFound), ServerResources.resourceCulture);

    internal static string FieldNotSupportedInQuery => ServerResources.ResourceManager.GetString(nameof (FieldNotSupportedInQuery), ServerResources.resourceCulture);

    internal static string FieldValueInvalidDataType => ServerResources.ResourceManager.GetString(nameof (FieldValueInvalidDataType), ServerResources.resourceCulture);

    internal static string FieldValueMissing => ServerResources.ResourceManager.GetString(nameof (FieldValueMissing), ServerResources.resourceCulture);

    internal static string FileDownloadExceedMaxSize => ServerResources.ResourceManager.GetString(nameof (FileDownloadExceedMaxSize), ServerResources.resourceCulture);

    internal static string FileUploadExceedMaxSize => ServerResources.ResourceManager.GetString(nameof (FileUploadExceedMaxSize), ServerResources.resourceCulture);

    internal static string GenericPermission => ServerResources.ResourceManager.GetString(nameof (GenericPermission), ServerResources.resourceCulture);

    internal static string GetTestResultsMetaDataApiMaxLimitError => ServerResources.ResourceManager.GetString(nameof (GetTestResultsMetaDataApiMaxLimitError), ServerResources.resourceCulture);

    internal static string HierarchicalResultNotSupported => ServerResources.ResourceManager.GetString(nameof (HierarchicalResultNotSupported), ServerResources.resourceCulture);

    internal static string HistoryCommentForClonedObject => ServerResources.ResourceManager.GetString(nameof (HistoryCommentForClonedObject), ServerResources.resourceCulture);

    internal static string Id => ServerResources.ResourceManager.GetString(nameof (Id), ServerResources.resourceCulture);

    internal static string IdMustBeSpecifiedError => ServerResources.ResourceManager.GetString(nameof (IdMustBeSpecifiedError), ServerResources.resourceCulture);

    internal static string IllegalResultStateTransition => ServerResources.ResourceManager.GetString(nameof (IllegalResultStateTransition), ServerResources.resourceCulture);

    internal static string IllegalRunStateTransition => ServerResources.ResourceManager.GetString(nameof (IllegalRunStateTransition), ServerResources.resourceCulture);

    internal static string InactivePlanCannotBeMarkedAsDefault => ServerResources.ResourceManager.GetString(nameof (InactivePlanCannotBeMarkedAsDefault), ServerResources.resourceCulture);

    internal static string IncompleteAttachmentTitle => ServerResources.ResourceManager.GetString(nameof (IncompleteAttachmentTitle), ServerResources.resourceCulture);

    internal static string IncorrectCopyHierarchyValue => ServerResources.ResourceManager.GetString(nameof (IncorrectCopyHierarchyValue), ServerResources.resourceCulture);

    internal static string InSufficientProjectMigratePermissions => ServerResources.ResourceManager.GetString(nameof (InSufficientProjectMigratePermissions), ServerResources.resourceCulture);

    internal static string InsufficientRetentionSettingsPermission => ServerResources.ResourceManager.GetString(nameof (InsufficientRetentionSettingsPermission), ServerResources.resourceCulture);

    internal static string InvalidArgument => ServerResources.ResourceManager.GetString(nameof (InvalidArgument), ServerResources.resourceCulture);

    internal static string InvalidAttachmentContent => ServerResources.ResourceManager.GetString(nameof (InvalidAttachmentContent), ServerResources.resourceCulture);

    internal static string InvalidAttachmentFile => ServerResources.ResourceManager.GetString(nameof (InvalidAttachmentFile), ServerResources.resourceCulture);

    internal static string InvalidAttachmentForUpdate => ServerResources.ResourceManager.GetString(nameof (InvalidAttachmentForUpdate), ServerResources.resourceCulture);

    internal static string InvalidAttachmentType => ServerResources.ResourceManager.GetString(nameof (InvalidAttachmentType), ServerResources.resourceCulture);

    internal static string InvalidBuildIdSpecified => ServerResources.ResourceManager.GetString(nameof (InvalidBuildIdSpecified), ServerResources.resourceCulture);

    internal static string InvalidChartDimension => ServerResources.ResourceManager.GetString(nameof (InvalidChartDimension), ServerResources.resourceCulture);

    internal static string InvalidChartTransformInstruction => ServerResources.ResourceManager.GetString(nameof (InvalidChartTransformInstruction), ServerResources.resourceCulture);

    internal static string InvalidConditionForTcmField => ServerResources.ResourceManager.GetString(nameof (InvalidConditionForTcmField), ServerResources.resourceCulture);

    internal static string InvalidContentType => ServerResources.ResourceManager.GetString(nameof (InvalidContentType), ServerResources.resourceCulture);

    internal static string InvalidDestinationPlanForPlanCloning => ServerResources.ResourceManager.GetString(nameof (InvalidDestinationPlanForPlanCloning), ServerResources.resourceCulture);

    internal static string InvalidDimensionInChartTransform => ServerResources.ResourceManager.GetString(nameof (InvalidDimensionInChartTransform), ServerResources.resourceCulture);

    internal static string InvalidFieldValue => ServerResources.ResourceManager.GetString(nameof (InvalidFieldValue), ServerResources.resourceCulture);

    internal static string InvalidFileNameSpecified => ServerResources.ResourceManager.GetString(nameof (InvalidFileNameSpecified), ServerResources.resourceCulture);

    internal static string InvalidGuidFormat => ServerResources.ResourceManager.GetString(nameof (InvalidGuidFormat), ServerResources.resourceCulture);

    internal static string InvalidIdSpecified => ServerResources.ResourceManager.GetString(nameof (InvalidIdSpecified), ServerResources.resourceCulture);

    internal static string InValidListOfIds => ServerResources.ResourceManager.GetString(nameof (InValidListOfIds), ServerResources.resourceCulture);

    internal static string InvalidParameterForUpdate => ServerResources.ResourceManager.GetString(nameof (InvalidParameterForUpdate), ServerResources.resourceCulture);

    internal static string InvalidPropertyMessage => ServerResources.ResourceManager.GetString(nameof (InvalidPropertyMessage), ServerResources.resourceCulture);

    internal static string InvalidPropertyUpdateForArtifact => ServerResources.ResourceManager.GetString(nameof (InvalidPropertyUpdateForArtifact), ServerResources.resourceCulture);

    internal static string InvalidReleaseSpecified => ServerResources.ResourceManager.GetString(nameof (InvalidReleaseSpecified), ServerResources.resourceCulture);

    internal static string InvalidSQLArgument => ServerResources.ResourceManager.GetString(nameof (InvalidSQLArgument), ServerResources.resourceCulture);

    internal static string InvalidStructurePath => ServerResources.ResourceManager.GetString(nameof (InvalidStructurePath), ServerResources.resourceCulture);

    internal static string InvalidSuiteType => ServerResources.ResourceManager.GetString(nameof (InvalidSuiteType), ServerResources.resourceCulture);

    internal static string InvalidTcmState => ServerResources.ResourceManager.GetString(nameof (InvalidTcmState), ServerResources.resourceCulture);

    internal static string InvalidTestActionResultForUpdate => ServerResources.ResourceManager.GetString(nameof (InvalidTestActionResultForUpdate), ServerResources.resourceCulture);

    internal static string InvalidTestCaseResultForUpdate => ServerResources.ResourceManager.GetString(nameof (InvalidTestCaseResultForUpdate), ServerResources.resourceCulture);

    internal static string InvalidTestResultIdsSpecified => ServerResources.ResourceManager.GetString(nameof (InvalidTestResultIdsSpecified), ServerResources.resourceCulture);

    internal static string InvalidTestSuiteEntry => ServerResources.ResourceManager.GetString(nameof (InvalidTestSuiteEntry), ServerResources.resourceCulture);

    internal static string InvalidValueSpecified => ServerResources.ResourceManager.GetString(nameof (InvalidValueSpecified), ServerResources.resourceCulture);

    internal static string InvalidWitCategory => ServerResources.ResourceManager.GetString(nameof (InvalidWitCategory), ServerResources.resourceCulture);

    internal static string InvalidWorkItemPassed => ServerResources.ResourceManager.GetString(nameof (InvalidWorkItemPassed), ServerResources.resourceCulture);

    internal static string IterationResultNotFound => ServerResources.ResourceManager.GetString(nameof (IterationResultNotFound), ServerResources.resourceCulture);

    internal static string KeyValueSeparator => ServerResources.ResourceManager.GetString(nameof (KeyValueSeparator), ServerResources.resourceCulture);

    internal static string LinkAlreadyExistsErrorMessage => ServerResources.ResourceManager.GetString(nameof (LinkAlreadyExistsErrorMessage), ServerResources.resourceCulture);

    internal static string ListNullError => ServerResources.ResourceManager.GetString(nameof (ListNullError), ServerResources.resourceCulture);

    internal static string LogStoreContainerDeleted => ServerResources.ResourceManager.GetString(nameof (LogStoreContainerDeleted), ServerResources.resourceCulture);

    internal static string LogStoreContainerDeletedMessage => ServerResources.ResourceManager.GetString(nameof (LogStoreContainerDeletedMessage), ServerResources.resourceCulture);

    internal static string LogStoreStorageAccountNotFound => ServerResources.ResourceManager.GetString(nameof (LogStoreStorageAccountNotFound), ServerResources.resourceCulture);

    internal static string MaxHierarchicalResultsLimitCrossedError => ServerResources.ResourceManager.GetString(nameof (MaxHierarchicalResultsLimitCrossedError), ServerResources.resourceCulture);

    internal static string MaxIdsError => ServerResources.ResourceManager.GetString(nameof (MaxIdsError), ServerResources.resourceCulture);

    internal static string MaximumSuitesExceeded => ServerResources.ResourceManager.GetString(nameof (MaximumSuitesExceeded), ServerResources.resourceCulture);

    internal static string MaximumTestCaseIdsExceeded => ServerResources.ResourceManager.GetString(nameof (MaximumTestCaseIdsExceeded), ServerResources.resourceCulture);

    internal static string MaxLimitForLinkedTestsExeceeded => ServerResources.ResourceManager.GetString(nameof (MaxLimitForLinkedTestsExeceeded), ServerResources.resourceCulture);

    internal static string MaxLimitForWorkItemsExeceeded => ServerResources.ResourceManager.GetString(nameof (MaxLimitForWorkItemsExeceeded), ServerResources.resourceCulture);

    internal static string MaxTestCaseIdsError => ServerResources.ResourceManager.GetString(nameof (MaxTestCaseIdsError), ServerResources.resourceCulture);

    internal static string MaxTestFlakinessBranchLimitExceeded => ServerResources.ResourceManager.GetString(nameof (MaxTestFlakinessBranchLimitExceeded), ServerResources.resourceCulture);

    internal static string MaxTestResultsLimitCrossedError => ServerResources.ResourceManager.GetString(nameof (MaxTestResultsLimitCrossedError), ServerResources.resourceCulture);

    internal static string MetricsCalculationException => ServerResources.ResourceManager.GetString(nameof (MetricsCalculationException), ServerResources.resourceCulture);

    internal static string MigrateQBSWarning => ServerResources.ResourceManager.GetString(nameof (MigrateQBSWarning), ServerResources.resourceCulture);

    internal static string MoveSuiteInsideNonStaticSuite => ServerResources.ResourceManager.GetString(nameof (MoveSuiteInsideNonStaticSuite), ServerResources.resourceCulture);

    internal static string MultipleTestPlanWitStatesMappedToSameMetaStateException => ServerResources.ResourceManager.GetString(nameof (MultipleTestPlanWitStatesMappedToSameMetaStateException), ServerResources.resourceCulture);

    internal static string MultipleTestSuiteWitStatesMappedToSameMetaStateException => ServerResources.ResourceManager.GetString(nameof (MultipleTestSuiteWitStatesMappedToSameMetaStateException), ServerResources.resourceCulture);

    internal static string NegativeAttemptNumber => ServerResources.ResourceManager.GetString(nameof (NegativeAttemptNumber), ServerResources.resourceCulture);

    internal static string NewTestOnlySetToTrueWithoutFF => ServerResources.ResourceManager.GetString(nameof (NewTestOnlySetToTrueWithoutFF), ServerResources.resourceCulture);

    internal static string NoBuildDefinitionWithName => ServerResources.ResourceManager.GetString(nameof (NoBuildDefinitionWithName), ServerResources.resourceCulture);

    internal static string NonConfigNotSupported => ServerResources.ResourceManager.GetString(nameof (NonConfigNotSupported), ServerResources.resourceCulture);

    internal static string NoPlansToMigrate => ServerResources.ResourceManager.GetString(nameof (NoPlansToMigrate), ServerResources.resourceCulture);

    internal static string NotAuthorizedToAccessApi => ServerResources.ResourceManager.GetString(nameof (NotAuthorizedToAccessApi), ServerResources.resourceCulture);

    internal static string NoTestResultFoundForUpdatingFlakySettings => ServerResources.ResourceManager.GetString(nameof (NoTestResultFoundForUpdatingFlakySettings), ServerResources.resourceCulture);

    internal static string NotificationError => ServerResources.ResourceManager.GetString(nameof (NotificationError), ServerResources.resourceCulture);

    internal static string NotSupportedArgumentValue => ServerResources.ResourceManager.GetString(nameof (NotSupportedArgumentValue), ServerResources.resourceCulture);

    internal static string NoValidStateTransitionExistsForPlan => ServerResources.ResourceManager.GetString(nameof (NoValidStateTransitionExistsForPlan), ServerResources.resourceCulture);

    internal static string NoValidStateTransitionExistsForSuite => ServerResources.ResourceManager.GetString(nameof (NoValidStateTransitionExistsForSuite), ServerResources.resourceCulture);

    internal static string NullArgument => ServerResources.ResourceManager.GetString(nameof (NullArgument), ServerResources.resourceCulture);

    internal static string NullValueForRequiredParameter => ServerResources.ResourceManager.GetString(nameof (NullValueForRequiredParameter), ServerResources.resourceCulture);

    internal static string OnlyEnumTypesSupportedError => ServerResources.ResourceManager.GetString(nameof (OnlyEnumTypesSupportedError), ServerResources.resourceCulture);

    internal static string OperationNotSupportedDuringUpradeError => ServerResources.ResourceManager.GetString(nameof (OperationNotSupportedDuringUpradeError), ServerResources.resourceCulture);

    internal static string OtherOutcomesNotSupportedError => ServerResources.ResourceManager.GetString(nameof (OtherOutcomesNotSupportedError), ServerResources.resourceCulture);

    internal static string ParallelArraySizeMismatch => ServerResources.ResourceManager.GetString(nameof (ParallelArraySizeMismatch), ServerResources.resourceCulture);

    internal static string ParameterResultNotFound => ServerResources.ResourceManager.GetString(nameof (ParameterResultNotFound), ServerResources.resourceCulture);

    internal static string ParentSuiteNotFound => ServerResources.ResourceManager.GetString(nameof (ParentSuiteNotFound), ServerResources.resourceCulture);

    internal static string ParentSuiteUpdated => ServerResources.ResourceManager.GetString(nameof (ParentSuiteUpdated), ServerResources.resourceCulture);

    internal static string PipelineIdDoNotExist => ServerResources.ResourceManager.GetString(nameof (PipelineIdDoNotExist), ServerResources.resourceCulture);

    internal static string PlanAreaPathNotFound => ServerResources.ResourceManager.GetString(nameof (PlanAreaPathNotFound), ServerResources.resourceCulture);

    internal static string PlanCategoryValidationError => ServerResources.ResourceManager.GetString(nameof (PlanCategoryValidationError), ServerResources.resourceCulture);

    internal static string PlannedTestResultPropertiesNotSpecifiedError => ServerResources.ResourceManager.GetString(nameof (PlannedTestResultPropertiesNotSpecifiedError), ServerResources.resourceCulture);

    internal static string PlanUnderClone => ServerResources.ResourceManager.GetString(nameof (PlanUnderClone), ServerResources.resourceCulture);

    internal static string PointAssignmentsContainDuplicateConfigurations => ServerResources.ResourceManager.GetString(nameof (PointAssignmentsContainDuplicateConfigurations), ServerResources.resourceCulture);

    internal static string PointOrSuiteIdMissingInLinkedWorkItemQueryParameters => ServerResources.ResourceManager.GetString(nameof (PointOrSuiteIdMissingInLinkedWorkItemQueryParameters), ServerResources.resourceCulture);

    internal static string PositiveArgument => ServerResources.ResourceManager.GetString(nameof (PositiveArgument), ServerResources.resourceCulture);

    internal static string ProjectCollectionCatalogNodeNotFound => ServerResources.ResourceManager.GetString(nameof (ProjectCollectionCatalogNodeNotFound), ServerResources.resourceCulture);

    internal static string PropertyCannotBeEmpty => ServerResources.ResourceManager.GetString(nameof (PropertyCannotBeEmpty), ServerResources.resourceCulture);

    internal static string PropertyCanNotBeNonNull => ServerResources.ResourceManager.GetString(nameof (PropertyCanNotBeNonNull), ServerResources.resourceCulture);

    internal static string PropertyCannotBeNullOrEmpty => ServerResources.ResourceManager.GetString(nameof (PropertyCannotBeNullOrEmpty), ServerResources.resourceCulture);

    internal static string PropertyTooLong => ServerResources.ResourceManager.GetString(nameof (PropertyTooLong), ServerResources.resourceCulture);

    internal static string PublicProjectReachedLogStoreCapacityThreshold => ServerResources.ResourceManager.GetString(nameof (PublicProjectReachedLogStoreCapacityThreshold), ServerResources.resourceCulture);

    internal static string QueryNeedsCategoryCondition => ServerResources.ResourceManager.GetString(nameof (QueryNeedsCategoryCondition), ServerResources.resourceCulture);

    internal static string QueryParameterOutOfRange => ServerResources.ResourceManager.GetString(nameof (QueryParameterOutOfRange), ServerResources.resourceCulture);

    internal static string RelatedLinkComment => ServerResources.ResourceManager.GetString(nameof (RelatedLinkComment), ServerResources.resourceCulture);

    internal static string ReleaseEnvironmentIdDefinitionAmbigious => ServerResources.ResourceManager.GetString(nameof (ReleaseEnvironmentIdDefinitionAmbigious), ServerResources.resourceCulture);

    internal static string ReleaseEnvironmentNotFound => ServerResources.ResourceManager.GetString(nameof (ReleaseEnvironmentNotFound), ServerResources.resourceCulture);

    internal static string ReleaseIdDefinitionAmbigious => ServerResources.ResourceManager.GetString(nameof (ReleaseIdDefinitionAmbigious), ServerResources.resourceCulture);

    internal static string ReleaseIdOrEnvironmentIdNotSpecified => ServerResources.ResourceManager.GetString(nameof (ReleaseIdOrEnvironmentIdNotSpecified), ServerResources.resourceCulture);

    internal static string RequiredFieldValueAbsent => ServerResources.ResourceManager.GetString(nameof (RequiredFieldValueAbsent), ServerResources.resourceCulture);

    internal static string RequiredLinkedWorkItemQueryParametersMissing => ServerResources.ResourceManager.GetString(nameof (RequiredLinkedWorkItemQueryParametersMissing), ServerResources.resourceCulture);

    internal static string RequirementSuiteNotFoundForId => ServerResources.ResourceManager.GetString(nameof (RequirementSuiteNotFoundForId), ServerResources.resourceCulture);

    internal static string ResolutionStatesAreInUse => ServerResources.ResourceManager.GetString(nameof (ResolutionStatesAreInUse), ServerResources.resourceCulture);

    internal static string ResolutionStatesCannotBeEmpty => ServerResources.ResourceManager.GetString(nameof (ResolutionStatesCannotBeEmpty), ServerResources.resourceCulture);

    internal static string ResourceUrlCreationFailed => ServerResources.ResourceManager.GetString(nameof (ResourceUrlCreationFailed), ServerResources.resourceCulture);

    internal static string ResultQueryInvalid => ServerResources.ResourceManager.GetString(nameof (ResultQueryInvalid), ServerResources.resourceCulture);

    internal static string ResultRetentionDurationValidationError => ServerResources.ResourceManager.GetString(nameof (ResultRetentionDurationValidationError), ServerResources.resourceCulture);

    internal static string ResultRetentionSettingsAlreadyExistsExceptionText => ServerResources.ResourceManager.GetString(nameof (ResultRetentionSettingsAlreadyExistsExceptionText), ServerResources.resourceCulture);

    internal static string RunSummaryDuplicateEntry => ServerResources.ResourceManager.GetString(nameof (RunSummaryDuplicateEntry), ServerResources.resourceCulture);

    internal static string RunSummaryNegativeValueError => ServerResources.ResourceManager.GetString(nameof (RunSummaryNegativeValueError), ServerResources.resourceCulture);

    internal static string RunSummaryNotComputedException => ServerResources.ResourceManager.GetString(nameof (RunSummaryNotComputedException), ServerResources.resourceCulture);

    internal static string RunSummaryWhenTestRunIsNotCompletedException => ServerResources.ResourceManager.GetString(nameof (RunSummaryWhenTestRunIsNotCompletedException), ServerResources.resourceCulture);

    internal static string RunSummaryWithRunTypeErrorMsg => ServerResources.ResourceManager.GetString(nameof (RunSummaryWithRunTypeErrorMsg), ServerResources.resourceCulture);

    internal static string ServiceAccountRequired => ServerResources.ResourceManager.GetString(nameof (ServiceAccountRequired), ServerResources.resourceCulture);

    internal static string SessionAlreadyExists => ServerResources.ResourceManager.GetString(nameof (SessionAlreadyExists), ServerResources.resourceCulture);

    internal static string SessionArtifactTitle => ServerResources.ResourceManager.GetString(nameof (SessionArtifactTitle), ServerResources.resourceCulture);

    internal static string SessionNotFound => ServerResources.ResourceManager.GetString(nameof (SessionNotFound), ServerResources.resourceCulture);

    internal static string SharedStepsNotFound => ServerResources.ResourceManager.GetString(nameof (SharedStepsNotFound), ServerResources.resourceCulture);

    internal static string SomeTestResultLinkedWorkItemsCannotBeObtained => ServerResources.ResourceManager.GetString(nameof (SomeTestResultLinkedWorkItemsCannotBeObtained), ServerResources.resourceCulture);

    internal static string SpecifiedInvalidStepIdentifier => ServerResources.ResourceManager.GetString(nameof (SpecifiedInvalidStepIdentifier), ServerResources.resourceCulture);

    internal static string StartDateAfterEndDateError => ServerResources.ResourceManager.GetString(nameof (StartDateAfterEndDateError), ServerResources.resourceCulture);

    internal static string StreamNotObtained => ServerResources.ResourceManager.GetString(nameof (StreamNotObtained), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoAddedChildSuite => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoAddedChildSuite), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoAddedSuites => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoAddedSuites), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoAddedTestCases => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoAddedTestCases), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoAddedTestCasesAndSuites => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoAddedTestCasesAndSuites), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoAssignedConfig => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoAssignedConfig), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoAssignedTesters => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoAssignedTesters), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoClone => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoClone), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoDeletedSuites => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoDeletedSuites), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoDeletedTestCases => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoDeletedTestCases), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoInhertiConfig => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoInhertiConfig), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoPlanNameChanged => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoPlanNameChanged), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoRemovedTestCases => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoRemovedTestCases), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoRemovedTestCasesAndSuites => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoRemovedTestCasesAndSuites), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoRemovedTestSuites => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoRemovedTestSuites), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoSetEntryConfigurations => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoSetEntryConfigurations), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoTestCasesOrdering => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoTestCasesOrdering), ServerResources.resourceCulture);

    internal static string SuiteAuditInfoTestPlanDeleted => ServerResources.ResourceManager.GetString(nameof (SuiteAuditInfoTestPlanDeleted), ServerResources.resourceCulture);

    internal static string SuiteDepthOverLimit => ServerResources.ResourceManager.GetString(nameof (SuiteDepthOverLimit), ServerResources.resourceCulture);

    internal static string SyncTestPointsForSuitesFailed => ServerResources.ResourceManager.GetString(nameof (SyncTestPointsForSuitesFailed), ServerResources.resourceCulture);

    internal static string SystemFieldNotFound => ServerResources.ResourceManager.GetString(nameof (SystemFieldNotFound), ServerResources.resourceCulture);

    internal static string TCMChartingInProgressError => ServerResources.ResourceManager.GetString(nameof (TCMChartingInProgressError), ServerResources.resourceCulture);

    internal static string TCMDataMigrationInProgressError => ServerResources.ResourceManager.GetString(nameof (TCMDataMigrationInProgressError), ServerResources.resourceCulture);

    internal static string TeamProjectCatalogNodeNotFound => ServerResources.ResourceManager.GetString(nameof (TeamProjectCatalogNodeNotFound), ServerResources.resourceCulture);

    internal static string TeamProjectNotFound => ServerResources.ResourceManager.GetString(nameof (TeamProjectNotFound), ServerResources.resourceCulture);

    internal static string TestActionResultCompletedDateGreaterThanStartDate => ServerResources.ResourceManager.GetString(nameof (TestActionResultCompletedDateGreaterThanStartDate), ServerResources.resourceCulture);

    internal static string TestAttachmentIdNotObtained => ServerResources.ResourceManager.GetString(nameof (TestAttachmentIdNotObtained), ServerResources.resourceCulture);

    internal static string TestAttachmentNotFound => ServerResources.ResourceManager.GetString(nameof (TestAttachmentNotFound), ServerResources.resourceCulture);

    internal static string TestCaseAddedToDynamicSuite => ServerResources.ResourceManager.GetString(nameof (TestCaseAddedToDynamicSuite), ServerResources.resourceCulture);

    internal static string TestCaseAddedToRequirementSuite => ServerResources.ResourceManager.GetString(nameof (TestCaseAddedToRequirementSuite), ServerResources.resourceCulture);

    internal static string TestCaseAddtoSuiteFailed => ServerResources.ResourceManager.GetString(nameof (TestCaseAddtoSuiteFailed), ServerResources.resourceCulture);

    internal static string TestCaseClonerNotInitialized => ServerResources.ResourceManager.GetString(nameof (TestCaseClonerNotInitialized), ServerResources.resourceCulture);

    internal static string TestCaseIdSpecifiedWhenTestPointIdSpecifiedError => ServerResources.ResourceManager.GetString(nameof (TestCaseIdSpecifiedWhenTestPointIdSpecifiedError), ServerResources.resourceCulture);

    internal static string TestCaseNotFound => ServerResources.ResourceManager.GetString(nameof (TestCaseNotFound), ServerResources.resourceCulture);

    internal static string TestCaseNotFoundError => ServerResources.ResourceManager.GetString(nameof (TestCaseNotFoundError), ServerResources.resourceCulture);

    internal static string TestCaseNotFoundWithMoreDetails => ServerResources.ResourceManager.GetString(nameof (TestCaseNotFoundWithMoreDetails), ServerResources.resourceCulture);

    internal static string TestCasePropertiesSpecifiedError => ServerResources.ResourceManager.GetString(nameof (TestCasePropertiesSpecifiedError), ServerResources.resourceCulture);

    internal static string TestCaseReferenceError => ServerResources.ResourceManager.GetString(nameof (TestCaseReferenceError), ServerResources.resourceCulture);

    internal static string TestCaseReferenceNotFoundError => ServerResources.ResourceManager.GetString(nameof (TestCaseReferenceNotFoundError), ServerResources.resourceCulture);

    internal static string TestCaseRemovedFromDynamicSuite => ServerResources.ResourceManager.GetString(nameof (TestCaseRemovedFromDynamicSuite), ServerResources.resourceCulture);

    internal static string TestCaseResultNotFoundError => ServerResources.ResourceManager.GetString(nameof (TestCaseResultNotFoundError), ServerResources.resourceCulture);

    internal static string TestCasesNotFoundForAdd => ServerResources.ResourceManager.GetString(nameof (TestCasesNotFoundForAdd), ServerResources.resourceCulture);

    internal static string TestCasesNotFoundForRemove => ServerResources.ResourceManager.GetString(nameof (TestCasesNotFoundForRemove), ServerResources.resourceCulture);

    internal static string TestCaseTitleNotSpecifiedError => ServerResources.ResourceManager.GetString(nameof (TestCaseTitleNotSpecifiedError), ServerResources.resourceCulture);

    internal static string TestConfigurationAlreadyExists => ServerResources.ResourceManager.GetString(nameof (TestConfigurationAlreadyExists), ServerResources.resourceCulture);

    internal static string TestConfigurationCannotBeFoundError => ServerResources.ResourceManager.GetString(nameof (TestConfigurationCannotBeFoundError), ServerResources.resourceCulture);

    internal static string TestConfigurationInactive => ServerResources.ResourceManager.GetString(nameof (TestConfigurationInactive), ServerResources.resourceCulture);

    internal static string TestConfigurationInUse => ServerResources.ResourceManager.GetString(nameof (TestConfigurationInUse), ServerResources.resourceCulture);

    internal static string TestConfigurationNotFound => ServerResources.ResourceManager.GetString(nameof (TestConfigurationNotFound), ServerResources.resourceCulture);

    internal static string TestConfigurationSpecifiedError => ServerResources.ResourceManager.GetString(nameof (TestConfigurationSpecifiedError), ServerResources.resourceCulture);

    internal static string TestControllerAlreadyExists => ServerResources.ResourceManager.GetString(nameof (TestControllerAlreadyExists), ServerResources.resourceCulture);

    internal static string TestControllerNotFound => ServerResources.ResourceManager.GetString(nameof (TestControllerNotFound), ServerResources.resourceCulture);

    internal static string TestControllerNotSupported => ServerResources.ResourceManager.GetString(nameof (TestControllerNotSupported), ServerResources.resourceCulture);

    internal static string TestEnvironmentAlreadyExists => ServerResources.ResourceManager.GetString(nameof (TestEnvironmentAlreadyExists), ServerResources.resourceCulture);

    internal static string TestEnvironmentAlreadyExistsOnTestController => ServerResources.ResourceManager.GetString(nameof (TestEnvironmentAlreadyExistsOnTestController), ServerResources.resourceCulture);

    internal static string TestEnvironmentNotFound => ServerResources.ResourceManager.GetString(nameof (TestEnvironmentNotFound), ServerResources.resourceCulture);

    internal static string TestFailureTypeAlreadyExists => ServerResources.ResourceManager.GetString(nameof (TestFailureTypeAlreadyExists), ServerResources.resourceCulture);

    internal static string TestFailureTypeDeprecationMessage => ServerResources.ResourceManager.GetString(nameof (TestFailureTypeDeprecationMessage), ServerResources.resourceCulture);

    internal static string TestFailureTypeInUse => ServerResources.ResourceManager.GetString(nameof (TestFailureTypeInUse), ServerResources.resourceCulture);

    internal static string TestFailureTypeNotFound => ServerResources.ResourceManager.GetString(nameof (TestFailureTypeNotFound), ServerResources.resourceCulture);

    internal static string TestIterationNotFound => ServerResources.ResourceManager.GetString(nameof (TestIterationNotFound), ServerResources.resourceCulture);

    internal static string TestIterationResultCompletedDateGreaterThanStartDate => ServerResources.ResourceManager.GetString(nameof (TestIterationResultCompletedDateGreaterThanStartDate), ServerResources.resourceCulture);

    internal static string TestManagementAPINotSupportedError => ServerResources.ResourceManager.GetString(nameof (TestManagementAPINotSupportedError), ServerResources.resourceCulture);

    internal static string TestManagementServiveMaintenanceInProgress => ServerResources.ResourceManager.GetString(nameof (TestManagementServiveMaintenanceInProgress), ServerResources.resourceCulture);

    internal static string TestManagementSoapAccessBlocked => ServerResources.ResourceManager.GetString(nameof (TestManagementSoapAccessBlocked), ServerResources.resourceCulture);

    internal static string TestObjectUpdatedError => ServerResources.ResourceManager.GetString(nameof (TestObjectUpdatedError), ServerResources.resourceCulture);

    internal static string TestOutcome_Aborted => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Aborted), ServerResources.resourceCulture);

    internal static string TestOutcome_Blocked => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Blocked), ServerResources.resourceCulture);

    internal static string TestOutcome_Error => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Error), ServerResources.resourceCulture);

    internal static string TestOutcome_Failed => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Failed), ServerResources.resourceCulture);

    internal static string TestOutcome_Inconclusive => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Inconclusive), ServerResources.resourceCulture);

    internal static string TestOutcome_InProgress => ServerResources.ResourceManager.GetString(nameof (TestOutcome_InProgress), ServerResources.resourceCulture);

    internal static string TestOutcome_None => ServerResources.ResourceManager.GetString(nameof (TestOutcome_None), ServerResources.resourceCulture);

    internal static string TestOutcome_NotApplicable => ServerResources.ResourceManager.GetString(nameof (TestOutcome_NotApplicable), ServerResources.resourceCulture);

    internal static string TestOutcome_NotExecuted => ServerResources.ResourceManager.GetString(nameof (TestOutcome_NotExecuted), ServerResources.resourceCulture);

    internal static string TestOutcome_NotImpacted => ServerResources.ResourceManager.GetString(nameof (TestOutcome_NotImpacted), ServerResources.resourceCulture);

    internal static string TestOutcome_Passed => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Passed), ServerResources.resourceCulture);

    internal static string TestOutcome_Paused => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Paused), ServerResources.resourceCulture);

    internal static string TestOutcome_Timeout => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Timeout), ServerResources.resourceCulture);

    internal static string TestOutcome_Unspecified => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Unspecified), ServerResources.resourceCulture);

    internal static string TestOutcome_Warning => ServerResources.ResourceManager.GetString(nameof (TestOutcome_Warning), ServerResources.resourceCulture);

    internal static string TestPlanActiveWorkItemState => ServerResources.ResourceManager.GetString(nameof (TestPlanActiveWorkItemState), ServerResources.resourceCulture);

    internal static string TestPlanAlreadyCreatedWithWorkItemError => ServerResources.ResourceManager.GetString(nameof (TestPlanAlreadyCreatedWithWorkItemError), ServerResources.resourceCulture);

    internal static string TestPlanCategoryNotFound => ServerResources.ResourceManager.GetString(nameof (TestPlanCategoryNotFound), ServerResources.resourceCulture);

    internal static string TestPlanCloneNotFound => ServerResources.ResourceManager.GetString(nameof (TestPlanCloneNotFound), ServerResources.resourceCulture);

    internal static string TestPlanGetFilterOwnerInsufficientParams => ServerResources.ResourceManager.GetString(nameof (TestPlanGetFilterOwnerInsufficientParams), ServerResources.resourceCulture);

    internal static string TestPlanInactiveWorkItemState => ServerResources.ResourceManager.GetString(nameof (TestPlanInactiveWorkItemState), ServerResources.resourceCulture);

    internal static string TestPlanInUse => ServerResources.ResourceManager.GetString(nameof (TestPlanInUse), ServerResources.resourceCulture);

    internal static string TestPlanMigrationAlreadyCompleted => ServerResources.ResourceManager.GetString(nameof (TestPlanMigrationAlreadyCompleted), ServerResources.resourceCulture);

    internal static string TestPlanMigrationAlreadyInProgress => ServerResources.ResourceManager.GetString(nameof (TestPlanMigrationAlreadyInProgress), ServerResources.resourceCulture);

    internal static string TestPlanNotFound => ServerResources.ResourceManager.GetString(nameof (TestPlanNotFound), ServerResources.resourceCulture);

    internal static string TestPlanStartStateReason => ServerResources.ResourceManager.GetString(nameof (TestPlanStartStateReason), ServerResources.resourceCulture);

    internal static string TestPlanViewTestResultPermission => ServerResources.ResourceManager.GetString(nameof (TestPlanViewTestResultPermission), ServerResources.resourceCulture);

    internal static string TestPointAlreadyExists => ServerResources.ResourceManager.GetString(nameof (TestPointAlreadyExists), ServerResources.resourceCulture);

    internal static string TestPointNotFound => ServerResources.ResourceManager.GetString(nameof (TestPointNotFound), ServerResources.resourceCulture);

    internal static string TestPointNotFoundError => ServerResources.ResourceManager.GetString(nameof (TestPointNotFoundError), ServerResources.resourceCulture);

    internal static string TestPointsNotFound => ServerResources.ResourceManager.GetString(nameof (TestPointsNotFound), ServerResources.resourceCulture);

    internal static string TestResolutionStateDeprecationMessage => ServerResources.ResourceManager.GetString(nameof (TestResolutionStateDeprecationMessage), ServerResources.resourceCulture);

    internal static string TestResStateAlreadyExists => ServerResources.ResourceManager.GetString(nameof (TestResStateAlreadyExists), ServerResources.resourceCulture);

    internal static string TestResStateInUse => ServerResources.ResourceManager.GetString(nameof (TestResStateInUse), ServerResources.resourceCulture);

    internal static string TestResStateNotFound => ServerResources.ResourceManager.GetString(nameof (TestResStateNotFound), ServerResources.resourceCulture);

    internal static string TestResultAttachmentNotFound => ServerResources.ResourceManager.GetString(nameof (TestResultAttachmentNotFound), ServerResources.resourceCulture);

    internal static string TestResultCompletedDateGreaterThanStartDate => ServerResources.ResourceManager.GetString(nameof (TestResultCompletedDateGreaterThanStartDate), ServerResources.resourceCulture);

    internal static string TestResultIdCannotBeObtainedError => ServerResources.ResourceManager.GetString(nameof (TestResultIdCannotBeObtainedError), ServerResources.resourceCulture);

    internal static string TestResultIdNotObtainedError => ServerResources.ResourceManager.GetString(nameof (TestResultIdNotObtainedError), ServerResources.resourceCulture);

    internal static string TestResultIdSpecifiedInBulkUpdateTestResults => ServerResources.ResourceManager.GetString(nameof (TestResultIdSpecifiedInBulkUpdateTestResults), ServerResources.resourceCulture);

    internal static string TestResultNotFound => ServerResources.ResourceManager.GetString(nameof (TestResultNotFound), ServerResources.resourceCulture);

    internal static string TestResultNotUpdatedError => ServerResources.ResourceManager.GetString(nameof (TestResultNotUpdatedError), ServerResources.resourceCulture);

    internal static string TestResultsCreateErrorDueToRunState => ServerResources.ResourceManager.GetString(nameof (TestResultsCreateErrorDueToRunState), ServerResources.resourceCulture);

    internal static string TestResultsSettingsSecurityErrorMessage => ServerResources.ResourceManager.GetString(nameof (TestResultsSettingsSecurityErrorMessage), ServerResources.resourceCulture);

    internal static string TestResultUpdateFailureCompatiblityIssue => ServerResources.ResourceManager.GetString(nameof (TestResultUpdateFailureCompatiblityIssue), ServerResources.resourceCulture);

    internal static string TestRunAlreadyCanceled => ServerResources.ResourceManager.GetString(nameof (TestRunAlreadyCanceled), ServerResources.resourceCulture);

    internal static string TestRunAlreadyUpdated => ServerResources.ResourceManager.GetString(nameof (TestRunAlreadyUpdated), ServerResources.resourceCulture);

    internal static string TestRunCompletedTitle => ServerResources.ResourceManager.GetString(nameof (TestRunCompletedTitle), ServerResources.resourceCulture);

    internal static string TestRunNoResultsError => ServerResources.ResourceManager.GetString(nameof (TestRunNoResultsError), ServerResources.resourceCulture);

    internal static string TestRunNotFound => ServerResources.ResourceManager.GetString(nameof (TestRunNotFound), ServerResources.resourceCulture);

    internal static string TestRunNotFoundError => ServerResources.ResourceManager.GetString(nameof (TestRunNotFoundError), ServerResources.resourceCulture);

    internal static string TestRunStartedTitle => ServerResources.ResourceManager.GetString(nameof (TestRunStartedTitle), ServerResources.resourceCulture);

    internal static string TestRunTypeNotSupported => ServerResources.ResourceManager.GetString(nameof (TestRunTypeNotSupported), ServerResources.resourceCulture);

    internal static string TestSettingsAlreadyExists => ServerResources.ResourceManager.GetString(nameof (TestSettingsAlreadyExists), ServerResources.resourceCulture);

    internal static string TestSettingsInUse => ServerResources.ResourceManager.GetString(nameof (TestSettingsInUse), ServerResources.resourceCulture);

    internal static string TestSettingsNotFound => ServerResources.ResourceManager.GetString(nameof (TestSettingsNotFound), ServerResources.resourceCulture);

    internal static string TestSubResultCountMaxedOut => ServerResources.ResourceManager.GetString(nameof (TestSubResultCountMaxedOut), ServerResources.resourceCulture);

    internal static string TestSubResultInvalidMaxHierarchy => ServerResources.ResourceManager.GetString(nameof (TestSubResultInvalidMaxHierarchy), ServerResources.resourceCulture);

    internal static string TestSuiteCannotBeUpdated => ServerResources.ResourceManager.GetString(nameof (TestSuiteCannotBeUpdated), ServerResources.resourceCulture);

    internal static string TestSuiteCloneNotFound => ServerResources.ResourceManager.GetString(nameof (TestSuiteCloneNotFound), ServerResources.resourceCulture);

    internal static string TestSuiteCompletedWorkItemState => ServerResources.ResourceManager.GetString(nameof (TestSuiteCompletedWorkItemState), ServerResources.resourceCulture);

    internal static string TestSuiteDoesNotExistInPlan => ServerResources.ResourceManager.GetString(nameof (TestSuiteDoesNotExistInPlan), ServerResources.resourceCulture);

    internal static string TestSuiteEntryNotFound => ServerResources.ResourceManager.GetString(nameof (TestSuiteEntryNotFound), ServerResources.resourceCulture);

    internal static string TestSuiteInPlanningWorkItemState => ServerResources.ResourceManager.GetString(nameof (TestSuiteInPlanningWorkItemState), ServerResources.resourceCulture);

    internal static string TestSuiteInProgressWorkItemState => ServerResources.ResourceManager.GetString(nameof (TestSuiteInProgressWorkItemState), ServerResources.resourceCulture);

    internal static string TestSuiteNotBelongToRequirementBasedCategory => ServerResources.ResourceManager.GetString(nameof (TestSuiteNotBelongToRequirementBasedCategory), ServerResources.resourceCulture);

    internal static string TestSuiteNotFound => ServerResources.ResourceManager.GetString(nameof (TestSuiteNotFound), ServerResources.resourceCulture);

    internal static string TestSuiteStartStateReason => ServerResources.ResourceManager.GetString(nameof (TestSuiteStartStateReason), ServerResources.resourceCulture);

    internal static string TestSuiteViewTestResultPermission => ServerResources.ResourceManager.GetString(nameof (TestSuiteViewTestResultPermission), ServerResources.resourceCulture);

    internal static string TestTagFormatError => ServerResources.ResourceManager.GetString(nameof (TestTagFormatError), ServerResources.resourceCulture);

    internal static string TestTagForRunNotSupported => ServerResources.ResourceManager.GetString(nameof (TestTagForRunNotSupported), ServerResources.resourceCulture);

    internal static string TestTagLimitReached => ServerResources.ResourceManager.GetString(nameof (TestTagLimitReached), ServerResources.resourceCulture);

    internal static string TestTagNotSupportedForManual => ServerResources.ResourceManager.GetString(nameof (TestTagNotSupportedForManual), ServerResources.resourceCulture);

    internal static string TestVariableAlreadyExists => ServerResources.ResourceManager.GetString(nameof (TestVariableAlreadyExists), ServerResources.resourceCulture);

    internal static string TestVariableAlreadyExistsInConfiguration => ServerResources.ResourceManager.GetString(nameof (TestVariableAlreadyExistsInConfiguration), ServerResources.resourceCulture);

    internal static string TestVariableInUse => ServerResources.ResourceManager.GetString(nameof (TestVariableInUse), ServerResources.resourceCulture);

    internal static string TestVariableNotFound => ServerResources.ResourceManager.GetString(nameof (TestVariableNotFound), ServerResources.resourceCulture);

    internal static string TestVariableValueInUse => ServerResources.ResourceManager.GetString(nameof (TestVariableValueInUse), ServerResources.resourceCulture);

    internal static string TfsIdentityNotFound => ServerResources.ResourceManager.GetString(nameof (TfsIdentityNotFound), ServerResources.resourceCulture);

    internal static string TFSUsingHelperlayer => ServerResources.ResourceManager.GetString(nameof (TFSUsingHelperlayer), ServerResources.resourceCulture);

    internal static string UnsupportedGroupByFieldsError => ServerResources.ResourceManager.GetString(nameof (UnsupportedGroupByFieldsError), ServerResources.resourceCulture);

    internal static string UpdateCssCacheFailedError => ServerResources.ResourceManager.GetString(nameof (UpdateCssCacheFailedError), ServerResources.resourceCulture);

    internal static string UpdateTestPlanSettingsWithCorrectBuild => ServerResources.ResourceManager.GetString(nameof (UpdateTestPlanSettingsWithCorrectBuild), ServerResources.resourceCulture);

    internal static string UsersCannotAddSystemFieldsError => ServerResources.ResourceManager.GetString(nameof (UsersCannotAddSystemFieldsError), ServerResources.resourceCulture);

    internal static string ValueAlreadyExistsInVariable => ServerResources.ResourceManager.GetString(nameof (ValueAlreadyExistsInVariable), ServerResources.resourceCulture);

    internal static string VariableSeparator => ServerResources.ResourceManager.GetString(nameof (VariableSeparator), ServerResources.resourceCulture);

    internal static string ViewTestPointsPermission => ServerResources.ResourceManager.GetString(nameof (ViewTestPointsPermission), ServerResources.resourceCulture);

    internal static string WIQLNoFilterError => ServerResources.ResourceManager.GetString(nameof (WIQLNoFilterError), ServerResources.resourceCulture);

    internal static string WorkItemDeletePermissionError => ServerResources.ResourceManager.GetString(nameof (WorkItemDeletePermissionError), ServerResources.resourceCulture);

    internal static string WorkItemNotBelongToRequirementCategory => ServerResources.ResourceManager.GetString(nameof (WorkItemNotBelongToRequirementCategory), ServerResources.resourceCulture);

    internal static string WorkItemNotFoundForRequirement => ServerResources.ResourceManager.GetString(nameof (WorkItemNotFoundForRequirement), ServerResources.resourceCulture);

    internal static string WorkItemNotOfTypeRBSSupportedCategory => ServerResources.ResourceManager.GetString(nameof (WorkItemNotOfTypeRBSSupportedCategory), ServerResources.resourceCulture);

    internal static string WorkItemNotOfTypeRequirementCategory => ServerResources.ResourceManager.GetString(nameof (WorkItemNotOfTypeRequirementCategory), ServerResources.resourceCulture);
  }
}
