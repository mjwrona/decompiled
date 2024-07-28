// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildTypeResource
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class BuildTypeResource
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (BuildTypeResource), typeof (BuildTypeResource).GetTypeInfo().Assembly);

    public static ResourceManager Manager => BuildTypeResource.s_resMgr;

    private static string Get(string resourceName) => BuildTypeResource.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? BuildTypeResource.Get(resourceName) : BuildTypeResource.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) BuildTypeResource.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? BuildTypeResource.GetInt(resourceName) : (int) BuildTypeResource.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) BuildTypeResource.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? BuildTypeResource.GetBool(resourceName) : (bool) BuildTypeResource.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => BuildTypeResource.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = BuildTypeResource.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string ADMINISTER_BUILD() => BuildTypeResource.Get(nameof (ADMINISTER_BUILD));

    public static string ADMINISTER_BUILD(CultureInfo culture) => BuildTypeResource.Get(nameof (ADMINISTER_BUILD), culture);

    public static string ArtifactTypeMisMatch(object arg0, object arg1) => BuildTypeResource.Format(nameof (ArtifactTypeMisMatch), arg0, arg1);

    public static string ArtifactTypeMisMatch(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (ArtifactTypeMisMatch), culture, arg0, arg1);

    public static string BuildDeletionResultDropLocationFailure(
      object arg0,
      object arg1,
      object arg2)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultDropLocationFailure), arg0, arg1, arg2);
    }

    public static string BuildDeletionResultDropLocationFailure(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultDropLocationFailure), culture, arg0, arg1, arg2);
    }

    public static string BuildDeletionResultDropLocationFailureWithAccount(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultDropLocationFailureWithAccount), arg0, arg1, arg2, arg3);
    }

    public static string BuildDeletionResultDropLocationFailureWithAccount(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultDropLocationFailureWithAccount), culture, arg0, arg1, arg2, arg3);
    }

    public static string BuildDeletionResultLabelFailure(object arg0, object arg1, object arg2) => BuildTypeResource.Format(nameof (BuildDeletionResultLabelFailure), arg0, arg1, arg2);

    public static string BuildDeletionResultLabelFailure(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultLabelFailure), culture, arg0, arg1, arg2);
    }

    public static string BuildDeletionResultLabelFailureWithAccount(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultLabelFailureWithAccount), arg0, arg1, arg2, arg3);
    }

    public static string BuildDeletionResultLabelFailureWithAccount(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultLabelFailureWithAccount), culture, arg0, arg1, arg2, arg3);
    }

    public static string BuildDeletionResultSymbolsFailure(object arg0, object arg1) => BuildTypeResource.Format(nameof (BuildDeletionResultSymbolsFailure), arg0, arg1);

    public static string BuildDeletionResultSymbolsFailure(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultSymbolsFailure), culture, arg0, arg1);
    }

    public static string BuildDeletionResultTestFailure(object arg0, object arg1) => BuildTypeResource.Format(nameof (BuildDeletionResultTestFailure), arg0, arg1);

    public static string BuildDeletionResultTestFailure(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (BuildDeletionResultTestFailure), culture, arg0, arg1);
    }

    public static string BuildQualityUnexamined() => BuildTypeResource.Get(nameof (BuildQualityUnexamined));

    public static string BuildQualityUnexamined(CultureInfo culture) => BuildTypeResource.Get(nameof (BuildQualityUnexamined), culture);

    public static string BuildStoppedBy(object arg0) => BuildTypeResource.Format(nameof (BuildStoppedBy), arg0);

    public static string BuildStoppedBy(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildStoppedBy), culture, arg0);

    public static string BuildInitializingState() => BuildTypeResource.Get(nameof (BuildInitializingState));

    public static string BuildInitializingState(CultureInfo culture) => BuildTypeResource.Get(nameof (BuildInitializingState), culture);

    public static string BuildInProgress(object arg0) => BuildTypeResource.Format(nameof (BuildInProgress), arg0);

    public static string BuildInProgress(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildInProgress), culture, arg0);

    public static string BuildNumberInvalidChars(object arg0) => BuildTypeResource.Format(nameof (BuildNumberInvalidChars), arg0);

    public static string BuildNumberInvalidChars(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildNumberInvalidChars), culture, arg0);

    public static string BuildNumberInvalidTermination(object arg0) => BuildTypeResource.Format(nameof (BuildNumberInvalidTermination), arg0);

    public static string BuildNumberInvalidTermination(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildNumberInvalidTermination), culture, arg0);

    public static string BuildExeNotFound(object arg0, object arg1, object arg2) => BuildTypeResource.Format(nameof (BuildExeNotFound), arg0, arg1, arg2);

    public static string BuildExeNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (BuildExeNotFound), culture, arg0, arg1, arg2);
    }

    public static string BuildReasonNotSupported(object arg0, object arg1, object arg2) => BuildTypeResource.Format(nameof (BuildReasonNotSupported), arg0, arg1, arg2);

    public static string BuildReasonNotSupported(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (BuildReasonNotSupported), culture, arg0, arg1, arg2);
    }

    public static string TemplateVersionNotSupported(object arg0, object arg1) => BuildTypeResource.Format(nameof (TemplateVersionNotSupported), arg0, arg1);

    public static string TemplateVersionNotSupported(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (TemplateVersionNotSupported), culture, arg0, arg1);

    public static string BuildRequestFromInvalidMachine(object arg0) => BuildTypeResource.Format(nameof (BuildRequestFromInvalidMachine), arg0);

    public static string BuildRequestFromInvalidMachine(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildRequestFromInvalidMachine), culture, arg0);

    public static string BuildStepNotFoundException(object arg0, object arg1) => BuildTypeResource.Format(nameof (BuildStepNotFoundException), arg0, arg1);

    public static string BuildStepNotFoundException(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildStepNotFoundException), culture, arg0, arg1);

    public static string BuildTagGroup_Agent() => BuildTypeResource.Get(nameof (BuildTagGroup_Agent));

    public static string BuildTagGroup_Agent(CultureInfo culture) => BuildTypeResource.Get(nameof (BuildTagGroup_Agent), culture);

    public static string BuildTagGroup_Build() => BuildTypeResource.Get(nameof (BuildTagGroup_Build));

    public static string BuildTagGroup_Build(CultureInfo culture) => BuildTypeResource.Get(nameof (BuildTagGroup_Build), culture);

    public static string BuildTagGroup_Definition() => BuildTypeResource.Get(nameof (BuildTagGroup_Definition));

    public static string BuildTagGroup_Definition(CultureInfo culture) => BuildTypeResource.Get(nameof (BuildTagGroup_Definition), culture);

    public static string BuildTypeNotFound(object arg0, object arg1) => BuildTypeResource.Format(nameof (BuildTypeNotFound), arg0, arg1);

    public static string BuildTypeNotFound(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildTypeNotFound), culture, arg0, arg1);

    public static string CannotCreateDropLocation(object arg0, object arg1) => BuildTypeResource.Format(nameof (CannotCreateDropLocation), arg0, arg1);

    public static string CannotCreateDropLocation(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (CannotCreateDropLocation), culture, arg0, arg1);

    public static string CannotStartBuildForActiveDefinition(object arg0, object arg1) => BuildTypeResource.Format(nameof (CannotStartBuildForActiveDefinition), arg0, arg1);

    public static string CannotStartBuildForActiveDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (CannotStartBuildForActiveDefinition), culture, arg0, arg1);
    }

    public static string CannotStartBuildForUnavailableController(object arg0, object arg1) => BuildTypeResource.Format(nameof (CannotStartBuildForUnavailableController), arg0, arg1);

    public static string CannotStartBuildForUnavailableController(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (CannotStartBuildForUnavailableController), culture, arg0, arg1);
    }

    public static string CannotStartBuildControllerOverloaded(object arg0, object arg1) => BuildTypeResource.Format(nameof (CannotStartBuildControllerOverloaded), arg0, arg1);

    public static string CannotStartBuildControllerOverloaded(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (CannotStartBuildControllerOverloaded), culture, arg0, arg1);
    }

    public static string CheckinComment() => BuildTypeResource.Get(nameof (CheckinComment));

    public static string CheckinComment(CultureInfo culture) => BuildTypeResource.Get(nameof (CheckinComment), culture);

    public static string CheckinPermissionDenied(object arg0, object arg1) => BuildTypeResource.Format(nameof (CheckinPermissionDenied), arg0, arg1);

    public static string CheckinPermissionDenied(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (CheckinPermissionDenied), culture, arg0, arg1);

    public static string CheckinPluginInitializationException(object arg0, object arg1) => BuildTypeResource.Format(nameof (CheckinPluginInitializationException), arg0, arg1);

    public static string CheckinPluginInitializationException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (CheckinPluginInitializationException), culture, arg0, arg1);
    }

    public static string CheckinPluginSubscriptionException(object arg0, object arg1) => BuildTypeResource.Format(nameof (CheckinPluginSubscriptionException), arg0, arg1);

    public static string CheckinPluginSubscriptionException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (CheckinPluginSubscriptionException), culture, arg0, arg1);
    }

    public static string ConnectionLostError() => BuildTypeResource.Get(nameof (ConnectionLostError));

    public static string ConnectionLostError(CultureInfo culture) => BuildTypeResource.Get(nameof (ConnectionLostError), culture);

    public static string DefinitionQueueStatus_Disabled() => BuildTypeResource.Get(nameof (DefinitionQueueStatus_Disabled));

    public static string DefinitionQueueStatus_Disabled(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionQueueStatus_Disabled), culture);

    public static string DefinitionQueueStatus_Enabled() => BuildTypeResource.Get(nameof (DefinitionQueueStatus_Enabled));

    public static string DefinitionQueueStatus_Enabled(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionQueueStatus_Enabled), culture);

    public static string DefinitionQueueStatus_Paused() => BuildTypeResource.Get(nameof (DefinitionQueueStatus_Paused));

    public static string DefinitionQueueStatus_Paused(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionQueueStatus_Paused), culture);

    public static string DefinitionTrigger_All() => BuildTypeResource.Get(nameof (DefinitionTrigger_All));

    public static string DefinitionTrigger_All(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionTrigger_All), culture);

    public static string DefinitionTrigger_None() => BuildTypeResource.Get(nameof (DefinitionTrigger_None));

    public static string DefinitionTrigger_None(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionTrigger_None), culture);

    public static string DefinitionTrigger_ContinuousIntegration() => BuildTypeResource.Get(nameof (DefinitionTrigger_ContinuousIntegration));

    public static string DefinitionTrigger_ContinuousIntegration(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionTrigger_ContinuousIntegration), culture);

    public static string DefinitionTrigger_BatchedContinuousIntegration() => BuildTypeResource.Get(nameof (DefinitionTrigger_BatchedContinuousIntegration));

    public static string DefinitionTrigger_BatchedContinuousIntegration(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionTrigger_BatchedContinuousIntegration), culture);

    public static string DefinitionTrigger_Schedule() => BuildTypeResource.Get(nameof (DefinitionTrigger_Schedule));

    public static string DefinitionTrigger_Schedule(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionTrigger_Schedule), culture);

    public static string DefinitionTrigger_ScheduleForced() => BuildTypeResource.Get(nameof (DefinitionTrigger_ScheduleForced));

    public static string DefinitionTrigger_ScheduleForced(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionTrigger_ScheduleForced), culture);

    public static string DefinitionTrigger_GatedCheckIn() => BuildTypeResource.Get(nameof (DefinitionTrigger_GatedCheckIn));

    public static string DefinitionTrigger_GatedCheckIn(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionTrigger_GatedCheckIn), culture);

    public static string DefinitionTrigger_BatchedGatedCheckIn() => BuildTypeResource.Get(nameof (DefinitionTrigger_BatchedGatedCheckIn));

    public static string DefinitionTrigger_BatchedGatedCheckIn(CultureInfo culture) => BuildTypeResource.Get(nameof (DefinitionTrigger_BatchedGatedCheckIn), culture);

    public static string DeleteOptions_All() => BuildTypeResource.Get(nameof (DeleteOptions_All));

    public static string DeleteOptions_All(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteOptions_All), culture);

    public static string DeleteOptions_Details() => BuildTypeResource.Get(nameof (DeleteOptions_Details));

    public static string DeleteOptions_Details(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteOptions_Details), culture);

    public static string DeleteOptions_DropLocation() => BuildTypeResource.Get(nameof (DeleteOptions_DropLocation));

    public static string DeleteOptions_DropLocation(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteOptions_DropLocation), culture);

    public static string DeleteOptions_Label() => BuildTypeResource.Get(nameof (DeleteOptions_Label));

    public static string DeleteOptions_Label(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteOptions_Label), culture);

    public static string DeleteOptions_None() => BuildTypeResource.Get(nameof (DeleteOptions_None));

    public static string DeleteOptions_None(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteOptions_None), culture);

    public static string DeleteOptions_Symbols() => BuildTypeResource.Get(nameof (DeleteOptions_Symbols));

    public static string DeleteOptions_Symbols(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteOptions_Symbols), culture);

    public static string DeleteOptions_TestResults() => BuildTypeResource.Get(nameof (DeleteOptions_TestResults));

    public static string DeleteOptions_TestResults(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteOptions_TestResults), culture);

    public static string DropLocationAlreadyExists(object arg0) => BuildTypeResource.Format(nameof (DropLocationAlreadyExists), arg0);

    public static string DropLocationAlreadyExists(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (DropLocationAlreadyExists), culture, arg0);

    public static string DuplicateBuildType(object arg0) => BuildTypeResource.Format(nameof (DuplicateBuildType), arg0);

    public static string DuplicateBuildType(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (DuplicateBuildType), culture, arg0);

    public static string EDIT_BUILD_STATUS() => BuildTypeResource.Get(nameof (EDIT_BUILD_STATUS));

    public static string EDIT_BUILD_STATUS(CultureInfo culture) => BuildTypeResource.Get(nameof (EDIT_BUILD_STATUS), culture);

    public static string EnlistmentEmptyTag(object arg0) => BuildTypeResource.Format(nameof (EnlistmentEmptyTag), arg0);

    public static string EnlistmentEmptyTag(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (EnlistmentEmptyTag), culture, arg0);

    public static string Error_InvalidOperationFailure() => BuildTypeResource.Get(nameof (Error_InvalidOperationFailure));

    public static string Error_InvalidOperationFailure(CultureInfo culture) => BuildTypeResource.Get(nameof (Error_InvalidOperationFailure), culture);

    public static string Error_NetFailure(object arg0) => BuildTypeResource.Format(nameof (Error_NetFailure), arg0);

    public static string Error_NetFailure(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (Error_NetFailure), culture, arg0);

    public static string Error_TFSFailure() => BuildTypeResource.Get(nameof (Error_TFSFailure));

    public static string Error_TFSFailure(CultureInfo culture) => BuildTypeResource.Get(nameof (Error_TFSFailure), culture);

    public static string Error_UnauthorizedAccess() => BuildTypeResource.Get(nameof (Error_UnauthorizedAccess));

    public static string Error_UnauthorizedAccess(CultureInfo culture) => BuildTypeResource.Get(nameof (Error_UnauthorizedAccess), culture);

    public static string ErrorInCheckInTag() => BuildTypeResource.Get(nameof (ErrorInCheckInTag));

    public static string ErrorInCheckInTag(CultureInfo culture) => BuildTypeResource.Get(nameof (ErrorInCheckInTag), culture);

    public static string ErrorInGettingSolutionNamesTag(object arg0) => BuildTypeResource.Format(nameof (ErrorInGettingSolutionNamesTag), arg0);

    public static string ErrorInGettingSolutionNamesTag(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ErrorInGettingSolutionNamesTag), culture, arg0);

    public static string ErrorInGettingVsmdiFilesTag(object arg0) => BuildTypeResource.Format(nameof (ErrorInGettingVsmdiFilesTag), arg0);

    public static string ErrorInGettingVsmdiFilesTag(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ErrorInGettingVsmdiFilesTag), culture, arg0);

    public static string ErrorInSyncingConfigFilesTag() => BuildTypeResource.Get(nameof (ErrorInSyncingConfigFilesTag));

    public static string ErrorInSyncingConfigFilesTag(CultureInfo culture) => BuildTypeResource.Get(nameof (ErrorInSyncingConfigFilesTag), culture);

    public static string ErrorOpeningWizardTag(object arg0) => BuildTypeResource.Format(nameof (ErrorOpeningWizardTag), arg0);

    public static string ErrorOpeningWizardTag(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ErrorOpeningWizardTag), culture, arg0);

    public static string ErrorWorkspaceAlreadyExistsTag(object arg0) => BuildTypeResource.Format(nameof (ErrorWorkspaceAlreadyExistsTag), arg0);

    public static string ErrorWorkspaceAlreadyExistsTag(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ErrorWorkspaceAlreadyExistsTag), culture, arg0);

    public static string ExceedMaxLength(object arg0) => BuildTypeResource.Format(nameof (ExceedMaxLength), arg0);

    public static string ExceedMaxLength(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ExceedMaxLength), culture, arg0);

    public static string FailedToStartBuild() => BuildTypeResource.Get(nameof (FailedToStartBuild));

    public static string FailedToStartBuild(CultureInfo culture) => BuildTypeResource.Get(nameof (FailedToStartBuild), culture);

    public static string FileSystemCannotSetAcls() => BuildTypeResource.Get(nameof (FileSystemCannotSetAcls));

    public static string FileSystemCannotSetAcls(CultureInfo culture) => BuildTypeResource.Get(nameof (FileSystemCannotSetAcls), culture);

    public static string FinishedCompilationState() => BuildTypeResource.Get(nameof (FinishedCompilationState));

    public static string FinishedCompilationState(CultureInfo culture) => BuildTypeResource.Get(nameof (FinishedCompilationState), culture);

    public static string FixFrameworkInstall() => BuildTypeResource.Get(nameof (FixFrameworkInstall));

    public static string FixFrameworkInstall(CultureInfo culture) => BuildTypeResource.Get(nameof (FixFrameworkInstall), culture);

    public static string FolderOpenError(object arg0, object arg1) => BuildTypeResource.Format(nameof (FolderOpenError), arg0, arg1);

    public static string FolderOpenError(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (FolderOpenError), culture, arg0, arg1);

    public static string FolderOpenErrorMissingPath() => BuildTypeResource.Get(nameof (FolderOpenErrorMissingPath));

    public static string FolderOpenErrorMissingPath(CultureInfo culture) => BuildTypeResource.Get(nameof (FolderOpenErrorMissingPath), culture);

    public static string GatedCheckinSingleDefinition(object arg0, object arg1) => BuildTypeResource.Format(nameof (GatedCheckinSingleDefinition), arg0, arg1);

    public static string GatedCheckinSingleDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (GatedCheckinSingleDefinition), culture, arg0, arg1);
    }

    public static string GatedCheckinSingleDefinitionFromShelveset(object arg0) => BuildTypeResource.Format(nameof (GatedCheckinSingleDefinitionFromShelveset), arg0);

    public static string GatedCheckinSingleDefinitionFromShelveset(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (GatedCheckinSingleDefinitionFromShelveset), culture, arg0);

    public static string GatedCheckinMultipleDefinition(object arg0) => BuildTypeResource.Format(nameof (GatedCheckinMultipleDefinition), arg0);

    public static string GatedCheckinMultipleDefinition(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (GatedCheckinMultipleDefinition), culture, arg0);

    public static string GatedCheckinMultipleDefinitionWithShelveset(object arg0, object arg1) => BuildTypeResource.Format(nameof (GatedCheckinMultipleDefinitionWithShelveset), arg0, arg1);

    public static string GatedCheckinMultipleDefinitionWithShelveset(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (GatedCheckinMultipleDefinitionWithShelveset), culture, arg0, arg1);
    }

    public static string GatedCheckinOverrideDenied(object arg0, object arg1, object arg2) => BuildTypeResource.Format(nameof (GatedCheckinOverrideDenied), arg0, arg1, arg2);

    public static string GatedCheckinOverrideDenied(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (GatedCheckinOverrideDenied), culture, arg0, arg1, arg2);
    }

    public static string GENERIC_READ() => BuildTypeResource.Get(nameof (GENERIC_READ));

    public static string GENERIC_READ(CultureInfo culture) => BuildTypeResource.Get(nameof (GENERIC_READ), culture);

    public static string InvalidAgentNameInvalidCharacters(object arg0) => BuildTypeResource.Format(nameof (InvalidAgentNameInvalidCharacters), arg0);

    public static string InvalidAgentNameInvalidCharacters(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidAgentNameInvalidCharacters), culture, arg0);

    public static string InvalidAgentNameTooLong(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidAgentNameTooLong), arg0, arg1);

    public static string InvalidAgentNameTooLong(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidAgentNameTooLong), culture, arg0, arg1);

    public static string InvalidAgentNameReservedName(object arg0) => BuildTypeResource.Format(nameof (InvalidAgentNameReservedName), arg0);

    public static string InvalidAgentNameReservedName(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidAgentNameReservedName), culture, arg0);

    public static string InvalidAgentNameInvalidTermination(object arg0) => BuildTypeResource.Format(nameof (InvalidAgentNameInvalidTermination), arg0);

    public static string InvalidAgentNameInvalidTermination(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidAgentNameInvalidTermination), culture, arg0);

    public static string InvalidControllerNameInvalidCharacters(object arg0) => BuildTypeResource.Format(nameof (InvalidControllerNameInvalidCharacters), arg0);

    public static string InvalidControllerNameInvalidCharacters(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidControllerNameInvalidCharacters), culture, arg0);

    public static string InvalidControllerNameTooLong(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidControllerNameTooLong), arg0, arg1);

    public static string InvalidControllerNameTooLong(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (InvalidControllerNameTooLong), culture, arg0, arg1);
    }

    public static string InvalidControllerNameReservedName(object arg0) => BuildTypeResource.Format(nameof (InvalidControllerNameReservedName), arg0);

    public static string InvalidControllerNameReservedName(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidControllerNameReservedName), culture, arg0);

    public static string InvalidControllerNameInvalidTermination(object arg0) => BuildTypeResource.Format(nameof (InvalidControllerNameInvalidTermination), arg0);

    public static string InvalidControllerNameInvalidTermination(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidControllerNameInvalidTermination), culture, arg0);

    public static string InvalidBuildDir() => BuildTypeResource.Get(nameof (InvalidBuildDir));

    public static string InvalidBuildDir(CultureInfo culture) => BuildTypeResource.Get(nameof (InvalidBuildDir), culture);

    public static string InvalidBuildRequest(object arg0) => BuildTypeResource.Format(nameof (InvalidBuildRequest), arg0);

    public static string InvalidBuildRequest(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidBuildRequest), culture, arg0);

    public static string InvalidBuildDirectoryForDrive(object arg0) => BuildTypeResource.Format(nameof (InvalidBuildDirectoryForDrive), arg0);

    public static string InvalidBuildDirectoryForDrive(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidBuildDirectoryForDrive), culture, arg0);

    public static string InvalidLogLocation(object arg0) => BuildTypeResource.Format(nameof (InvalidLogLocation), arg0);

    public static string InvalidLogLocation(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidLogLocation), culture, arg0);

    public static string InvalidDropLocation(object arg0) => BuildTypeResource.Format(nameof (InvalidDropLocation), arg0);

    public static string InvalidDropLocation(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidDropLocation), culture, arg0);

    public static string NonCanonicalBuildDirectory(object arg0) => BuildTypeResource.Format(nameof (NonCanonicalBuildDirectory), arg0);

    public static string NonCanonicalBuildDirectory(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (NonCanonicalBuildDirectory), culture, arg0);

    public static string RootBuildDirectory(object arg0) => BuildTypeResource.Format(nameof (RootBuildDirectory), arg0);

    public static string RootBuildDirectory(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (RootBuildDirectory), culture, arg0);

    public static string InvalidDefinitionNameInvalidCharacters(object arg0) => BuildTypeResource.Format(nameof (InvalidDefinitionNameInvalidCharacters), arg0);

    public static string InvalidDefinitionNameInvalidCharacters(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidDefinitionNameInvalidCharacters), culture, arg0);

    public static string InvalidDefinitionNameTooLong(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidDefinitionNameTooLong), arg0, arg1);

    public static string InvalidDefinitionNameTooLong(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (InvalidDefinitionNameTooLong), culture, arg0, arg1);
    }

    public static string InvalidDefinitionNameReservedName(object arg0) => BuildTypeResource.Format(nameof (InvalidDefinitionNameReservedName), arg0);

    public static string InvalidDefinitionNameReservedName(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidDefinitionNameReservedName), culture, arg0);

    public static string InvalidDefinitionNameInvalidTermination(object arg0) => BuildTypeResource.Format(nameof (InvalidDefinitionNameInvalidTermination), arg0);

    public static string InvalidDefinitionNameInvalidTermination(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidDefinitionNameInvalidTermination), culture, arg0);

    public static string InvalidDropDir(object arg0) => BuildTypeResource.Format(nameof (InvalidDropDir), arg0);

    public static string InvalidDropDir(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidDropDir), culture, arg0);

    public static string InvalidPathContainsWildcards(object arg0) => BuildTypeResource.Format(nameof (InvalidPathContainsWildcards), arg0);

    public static string InvalidPathContainsWildcards(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidPathContainsWildcards), culture, arg0);

    public static string InvalidPathMustContainRoot(object arg0) => BuildTypeResource.Format(nameof (InvalidPathMustContainRoot), arg0);

    public static string InvalidPathMustContainRoot(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidPathMustContainRoot), culture, arg0);

    public static string InvalidPathTeamProjectRequired(object arg0) => BuildTypeResource.Format(nameof (InvalidPathTeamProjectRequired), arg0);

    public static string InvalidPathTeamProjectRequired(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidPathTeamProjectRequired), culture, arg0);

    public static string InvalidPathVolumeRequired(object arg0) => BuildTypeResource.Format(nameof (InvalidPathVolumeRequired), arg0);

    public static string InvalidPathVolumeRequired(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidPathVolumeRequired), culture, arg0);

    public static string InvalidPlatformFlavorInvalidCharacters(object arg0) => BuildTypeResource.Format(nameof (InvalidPlatformFlavorInvalidCharacters), arg0);

    public static string InvalidPlatformFlavorInvalidCharacters(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidPlatformFlavorInvalidCharacters), culture, arg0);

    public static string InvalidPathReservedName(object arg0) => BuildTypeResource.Format(nameof (InvalidPathReservedName), arg0);

    public static string InvalidPathReservedName(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidPathReservedName), culture, arg0);

    public static string InvalidServiceHostNameInvalidCharacters(object arg0) => BuildTypeResource.Format(nameof (InvalidServiceHostNameInvalidCharacters), arg0);

    public static string InvalidServiceHostNameInvalidCharacters(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidServiceHostNameInvalidCharacters), culture, arg0);

    public static string InvalidServiceHostNameTooLong(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidServiceHostNameTooLong), arg0, arg1);

    public static string InvalidServiceHostNameTooLong(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (InvalidServiceHostNameTooLong), culture, arg0, arg1);
    }

    public static string InvalidServiceHostNameReservedName(object arg0) => BuildTypeResource.Format(nameof (InvalidServiceHostNameReservedName), arg0);

    public static string InvalidServiceHostNameReservedName(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidServiceHostNameReservedName), culture, arg0);

    public static string InvalidServiceHostNameInvalidTermination(object arg0) => BuildTypeResource.Format(nameof (InvalidServiceHostNameInvalidTermination), arg0);

    public static string InvalidServiceHostNameInvalidTermination(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidServiceHostNameInvalidTermination), culture, arg0);

    public static string InvalidSharedResourceName(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidSharedResourceName), arg0, arg1);

    public static string InvalidSharedResourceName(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidSharedResourceName), culture, arg0, arg1);

    public static string InvalidShelvesetName(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidShelvesetName), arg0, arg1);

    public static string InvalidShelvesetName(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidShelvesetName), culture, arg0, arg1);

    public static string InvalidShelvesetOwner(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidShelvesetOwner), arg0, arg1);

    public static string InvalidShelvesetOwner(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidShelvesetOwner), culture, arg0, arg1);

    public static string MissingShelvesetOwner(object arg0) => BuildTypeResource.Format(nameof (MissingShelvesetOwner), arg0);

    public static string MissingShelvesetOwner(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (MissingShelvesetOwner), culture, arg0);

    public static string InvalidUncPath(object arg0) => BuildTypeResource.Format(nameof (InvalidUncPath), arg0);

    public static string InvalidUncPath(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidUncPath), culture, arg0);

    public static string ServerPathNotSupported(object arg0) => BuildTypeResource.Format(nameof (ServerPathNotSupported), arg0);

    public static string ServerPathNotSupported(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ServerPathNotSupported), culture, arg0);

    public static string InvalidUriNotAbsolute(object arg0) => BuildTypeResource.Format(nameof (InvalidUriNotAbsolute), arg0);

    public static string InvalidUriNotAbsolute(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidUriNotAbsolute), culture, arg0);

    public static string InvalidBuildTypeNoDownload(object arg0) => BuildTypeResource.Format(nameof (InvalidBuildTypeNoDownload), arg0);

    public static string InvalidBuildTypeNoDownload(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidBuildTypeNoDownload), culture, arg0);

    public static string InvalidBuildType(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidBuildType), arg0, arg1);

    public static string InvalidBuildType(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidBuildType), culture, arg0, arg1);

    public static string InvalidBuildTypeFile(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidBuildTypeFile), arg0, arg1);

    public static string InvalidBuildTypeFile(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidBuildTypeFile), culture, arg0, arg1);

    public static string InvalidBuildTypeFileFormat(object arg0) => BuildTypeResource.Format(nameof (InvalidBuildTypeFileFormat), arg0);

    public static string InvalidBuildTypeFileFormat(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidBuildTypeFileFormat), culture, arg0);

    public static string InvalidInput(object arg0) => BuildTypeResource.Format(nameof (InvalidInput), arg0);

    public static string InvalidInput(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidInput), culture, arg0);

    public static string InvalidInputAtIndex(object arg0, object arg1, object arg2) => BuildTypeResource.Format(nameof (InvalidInputAtIndex), arg0, arg1, arg2);

    public static string InvalidInputAtIndex(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (InvalidInputAtIndex), culture, arg0, arg1, arg2);
    }

    public static string InvalidInputParameterOutOfRange(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidInputParameterOutOfRange), arg0, arg1);

    public static string InvalidInputParameterOutOfRange(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (InvalidInputParameterOutOfRange), culture, arg0, arg1);
    }

    public static string InvalidInputNull() => BuildTypeResource.Get(nameof (InvalidInputNull));

    public static string InvalidInputNull(CultureInfo culture) => BuildTypeResource.Get(nameof (InvalidInputNull), culture);

    public static string InvalidInputParameterNull(object arg0) => BuildTypeResource.Format(nameof (InvalidInputParameterNull), arg0);

    public static string InvalidInputParameterNull(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidInputParameterNull), culture, arg0);

    public static string MergeConflictTag() => BuildTypeResource.Get(nameof (MergeConflictTag));

    public static string MergeConflictTag(CultureInfo culture) => BuildTypeResource.Get(nameof (MergeConflictTag), culture);

    public static string MissingAgentName() => BuildTypeResource.Get(nameof (MissingAgentName));

    public static string MissingAgentName(CultureInfo culture) => BuildTypeResource.Get(nameof (MissingAgentName), culture);

    public static string MissingControllerName() => BuildTypeResource.Get(nameof (MissingControllerName));

    public static string MissingControllerName(CultureInfo culture) => BuildTypeResource.Get(nameof (MissingControllerName), culture);

    public static string MissingDefinitionName() => BuildTypeResource.Get(nameof (MissingDefinitionName));

    public static string MissingDefinitionName(CultureInfo culture) => BuildTypeResource.Get(nameof (MissingDefinitionName), culture);

    public static string MissingBuildDirectory() => BuildTypeResource.Get(nameof (MissingBuildDirectory));

    public static string MissingBuildDirectory(CultureInfo culture) => BuildTypeResource.Get(nameof (MissingBuildDirectory), culture);

    public static string MissingBuildMachine() => BuildTypeResource.Get(nameof (MissingBuildMachine));

    public static string MissingBuildMachine(CultureInfo culture) => BuildTypeResource.Get(nameof (MissingBuildMachine), culture);

    public static string MissingBuildNumber() => BuildTypeResource.Get(nameof (MissingBuildNumber));

    public static string MissingBuildNumber(CultureInfo culture) => BuildTypeResource.Get(nameof (MissingBuildNumber), culture);

    public static string MissingBuildType() => BuildTypeResource.Get(nameof (MissingBuildType));

    public static string MissingBuildType(CultureInfo culture) => BuildTypeResource.Get(nameof (MissingBuildType), culture);

    public static string MissingServiceHostName() => BuildTypeResource.Get(nameof (MissingServiceHostName));

    public static string MissingServiceHostName(CultureInfo culture) => BuildTypeResource.Get(nameof (MissingServiceHostName), culture);

    public static string PermissionDenied(object arg0) => BuildTypeResource.Format(nameof (PermissionDenied), arg0);

    public static string PermissionDenied(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (PermissionDenied), culture, arg0);

    public static string PlatformFlavorFormatText(object arg0, object arg1) => BuildTypeResource.Format(nameof (PlatformFlavorFormatText), arg0, arg1);

    public static string PlatformFlavorFormatText(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (PlatformFlavorFormatText), culture, arg0, arg1);

    public static string PortfolioProjectDoesNotExistTag(object arg0) => BuildTypeResource.Format(nameof (PortfolioProjectDoesNotExistTag), arg0);

    public static string PortfolioProjectDoesNotExistTag(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (PortfolioProjectDoesNotExistTag), culture, arg0);

    public static string ProcessTemplateDeleted() => BuildTypeResource.Get(nameof (ProcessTemplateDeleted));

    public static string ProcessTemplateDeleted(CultureInfo culture) => BuildTypeResource.Get(nameof (ProcessTemplateDeleted), culture);

    public static string ProcessTemplateDeletedWithServerPath(object arg0) => BuildTypeResource.Format(nameof (ProcessTemplateDeletedWithServerPath), arg0);

    public static string ProcessTemplateDeletedWithServerPath(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ProcessTemplateDeletedWithServerPath), culture, arg0);

    public static string ColumnRetentionPolicyHeaderText() => BuildTypeResource.Get(nameof (ColumnRetentionPolicyHeaderText));

    public static string ColumnRetentionPolicyHeaderText(CultureInfo culture) => BuildTypeResource.Get(nameof (ColumnRetentionPolicyHeaderText), culture);

    public static string ColumnDeleteOptionsHeaderText() => BuildTypeResource.Get(nameof (ColumnDeleteOptionsHeaderText));

    public static string ColumnDeleteOptionsHeaderText(CultureInfo culture) => BuildTypeResource.Get(nameof (ColumnDeleteOptionsHeaderText), culture);

    public static string QueuePriority_High() => BuildTypeResource.Get(nameof (QueuePriority_High));

    public static string QueuePriority_High(CultureInfo culture) => BuildTypeResource.Get(nameof (QueuePriority_High), culture);

    public static string QueuePriority_AboveNormal() => BuildTypeResource.Get(nameof (QueuePriority_AboveNormal));

    public static string QueuePriority_AboveNormal(CultureInfo culture) => BuildTypeResource.Get(nameof (QueuePriority_AboveNormal), culture);

    public static string QueuePriority_Normal() => BuildTypeResource.Get(nameof (QueuePriority_Normal));

    public static string QueuePriority_Normal(CultureInfo culture) => BuildTypeResource.Get(nameof (QueuePriority_Normal), culture);

    public static string QueuePriority_BelowNormal() => BuildTypeResource.Get(nameof (QueuePriority_BelowNormal));

    public static string QueuePriority_BelowNormal(CultureInfo culture) => BuildTypeResource.Get(nameof (QueuePriority_BelowNormal), culture);

    public static string QueuePriority_Low() => BuildTypeResource.Get(nameof (QueuePriority_Low));

    public static string QueuePriority_Low(CultureInfo culture) => BuildTypeResource.Get(nameof (QueuePriority_Low), culture);

    public static string Reason_All() => BuildTypeResource.Get(nameof (Reason_All));

    public static string Reason_All(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_All), culture);

    public static string Reason_None() => BuildTypeResource.Get(nameof (Reason_None));

    public static string Reason_None(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_None), culture);

    public static string Reason_Manual() => BuildTypeResource.Get(nameof (Reason_Manual));

    public static string Reason_Manual(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_Manual), culture);

    public static string Reason_IndividualCI() => BuildTypeResource.Get(nameof (Reason_IndividualCI));

    public static string Reason_IndividualCI(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_IndividualCI), culture);

    public static string Reason_BatchedCI() => BuildTypeResource.Get(nameof (Reason_BatchedCI));

    public static string Reason_BatchedCI(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_BatchedCI), culture);

    public static string Reason_Schedule() => BuildTypeResource.Get(nameof (Reason_Schedule));

    public static string Reason_Schedule(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_Schedule), culture);

    public static string Reason_ScheduleForced() => BuildTypeResource.Get(nameof (Reason_ScheduleForced));

    public static string Reason_ScheduleForced(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_ScheduleForced), culture);

    public static string Reason_UserCreated() => BuildTypeResource.Get(nameof (Reason_UserCreated));

    public static string Reason_UserCreated(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_UserCreated), culture);

    public static string Reason_ValidateShelveset() => BuildTypeResource.Get(nameof (Reason_ValidateShelveset));

    public static string Reason_ValidateShelveset(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_ValidateShelveset), culture);

    public static string Reason_CheckinShelveset() => BuildTypeResource.Get(nameof (Reason_CheckinShelveset));

    public static string Reason_CheckinShelveset(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_CheckinShelveset), culture);

    public static string Reason_Triggered() => BuildTypeResource.Get(nameof (Reason_Triggered));

    public static string Reason_Triggered(CultureInfo culture) => BuildTypeResource.Get(nameof (Reason_Triggered), culture);

    public static string RetentionPolicy_KeepAll() => BuildTypeResource.Get(nameof (RetentionPolicy_KeepAll));

    public static string RetentionPolicy_KeepAll(CultureInfo culture) => BuildTypeResource.Get(nameof (RetentionPolicy_KeepAll), culture);

    public static string RetentionPolicy_KeepNone() => BuildTypeResource.Get(nameof (RetentionPolicy_KeepNone));

    public static string RetentionPolicy_KeepNone(CultureInfo culture) => BuildTypeResource.Get(nameof (RetentionPolicy_KeepNone), culture);

    public static string RetentionPolicy_KeepLatest() => BuildTypeResource.Get(nameof (RetentionPolicy_KeepLatest));

    public static string RetentionPolicy_KeepLatest(CultureInfo culture) => BuildTypeResource.Get(nameof (RetentionPolicy_KeepLatest), culture);

    public static string RetentionPolicy_KeepLatestN_Format(object arg0) => BuildTypeResource.Format(nameof (RetentionPolicy_KeepLatestN_Format), arg0);

    public static string RetentionPolicy_KeepLatestN_Format(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (RetentionPolicy_KeepLatestN_Format), culture, arg0);

    public static string ServiceAccountUnauthorized(object arg0, object arg1) => BuildTypeResource.Format(nameof (ServiceAccountUnauthorized), arg0, arg1);

    public static string ServiceAccountUnauthorized(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (ServiceAccountUnauthorized), culture, arg0, arg1);

    public static string SyncState() => BuildTypeResource.Get(nameof (SyncState));

    public static string SyncState(CultureInfo culture) => BuildTypeResource.Get(nameof (SyncState), culture);

    public static string SyncCompletedState() => BuildTypeResource.Get(nameof (SyncCompletedState));

    public static string SyncCompletedState(CultureInfo culture) => BuildTypeResource.Get(nameof (SyncCompletedState), culture);

    public static string StartedCompilationState() => BuildTypeResource.Get(nameof (StartedCompilationState));

    public static string StartedCompilationState(CultureInfo culture) => BuildTypeResource.Get(nameof (StartedCompilationState), culture);

    public static string StartedTestingState() => BuildTypeResource.Get(nameof (StartedTestingState));

    public static string StartedTestingState(CultureInfo culture) => BuildTypeResource.Get(nameof (StartedTestingState), culture);

    public static string SourceControlDoesNotExist() => BuildTypeResource.Get(nameof (SourceControlDoesNotExist));

    public static string SourceControlDoesNotExist(CultureInfo culture) => BuildTypeResource.Get(nameof (SourceControlDoesNotExist), culture);

    public static string START_BUILD() => BuildTypeResource.Get(nameof (START_BUILD));

    public static string START_BUILD(CultureInfo culture) => BuildTypeResource.Get(nameof (START_BUILD), culture);

    public static string Status_All() => BuildTypeResource.Get(nameof (Status_All));

    public static string Status_All(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_All), culture);

    public static string Status_Available() => BuildTypeResource.Get(nameof (Status_Available));

    public static string Status_Available(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Available), culture);

    public static string Status_Canceled() => BuildTypeResource.Get(nameof (Status_Canceled));

    public static string Status_Canceled(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Canceled), culture);

    public static string Status_Completed() => BuildTypeResource.Get(nameof (Status_Completed));

    public static string Status_Completed(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Completed), culture);

    public static string Status_Disabled() => BuildTypeResource.Get(nameof (Status_Disabled));

    public static string Status_Disabled(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Disabled), culture);

    public static string Status_Enabled() => BuildTypeResource.Get(nameof (Status_Enabled));

    public static string Status_Enabled(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Enabled), culture);

    public static string Status_Failed() => BuildTypeResource.Get(nameof (Status_Failed));

    public static string Status_Failed(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Failed), culture);

    public static string Status_Initializing() => BuildTypeResource.Get(nameof (Status_Initializing));

    public static string Status_Initializing(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Initializing), culture);

    public static string Status_InProgress() => BuildTypeResource.Get(nameof (Status_InProgress));

    public static string Status_InProgress(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_InProgress), culture);

    public static string Status_None() => BuildTypeResource.Get(nameof (Status_None));

    public static string Status_None(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_None), culture);

    public static string Status_NotStarted() => BuildTypeResource.Get(nameof (Status_NotStarted));

    public static string Status_NotStarted(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_NotStarted), culture);

    public static string Status_Offline() => BuildTypeResource.Get(nameof (Status_Offline));

    public static string Status_Offline(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Offline), culture);

    public static string Status_PartiallySucceeded() => BuildTypeResource.Get(nameof (Status_PartiallySucceeded));

    public static string Status_PartiallySucceeded(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_PartiallySucceeded), culture);

    public static string Status_Postponed() => BuildTypeResource.Get(nameof (Status_Postponed));

    public static string Status_Postponed(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Postponed), culture);

    public static string Status_Queued() => BuildTypeResource.Get(nameof (Status_Queued));

    public static string Status_Queued(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Queued), culture);

    public static string Status_Retry() => BuildTypeResource.Get(nameof (Status_Retry));

    public static string Status_Retry(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Retry), culture);

    public static string Status_Stopped() => BuildTypeResource.Get(nameof (Status_Stopped));

    public static string Status_Stopped(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Stopped), culture);

    public static string Status_Succeeded() => BuildTypeResource.Get(nameof (Status_Succeeded));

    public static string Status_Succeeded(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Succeeded), culture);

    public static string Status_V1_Succeeded() => BuildTypeResource.Get(nameof (Status_V1_Succeeded));

    public static string Status_V1_Succeeded(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_V1_Succeeded), culture);

    public static string Status_Unavailable() => BuildTypeResource.Get(nameof (Status_Unavailable));

    public static string Status_Unavailable(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Unavailable), culture);

    public static string Status_Unknown() => BuildTypeResource.Get(nameof (Status_Unknown));

    public static string Status_Unknown(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Unknown), culture);

    public static string Status_Unreachable() => BuildTypeResource.Get(nameof (Status_Unreachable));

    public static string Status_Unreachable(CultureInfo culture) => BuildTypeResource.Get(nameof (Status_Unreachable), culture);

    public static string TaskCannotAddBuildStep(object arg0) => BuildTypeResource.Format(nameof (TaskCannotAddBuildStep), arg0);

    public static string TaskCannotAddBuildStep(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (TaskCannotAddBuildStep), culture, arg0);

    public static string TaskBuildStepException(object arg0, object arg1) => BuildTypeResource.Format(nameof (TaskBuildStepException), arg0, arg1);

    public static string TaskBuildStepException(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (TaskBuildStepException), culture, arg0, arg1);

    public static string InvalidPropertyValueException(object arg0, object arg1, object arg2) => BuildTypeResource.Format(nameof (InvalidPropertyValueException), arg0, arg1, arg2);

    public static string InvalidPropertyValueException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (InvalidPropertyValueException), culture, arg0, arg1, arg2);
    }

    public static string TestCompletedState() => BuildTypeResource.Get(nameof (TestCompletedState));

    public static string TestCompletedState(CultureInfo culture) => BuildTypeResource.Get(nameof (TestCompletedState), culture);

    public static string ThreadPoolFailure() => BuildTypeResource.Get(nameof (ThreadPoolFailure));

    public static string ThreadPoolFailure(CultureInfo culture) => BuildTypeResource.Get(nameof (ThreadPoolFailure), culture);

    public static string UnableToSetAcl(object arg0, object arg1) => BuildTypeResource.Format(nameof (UnableToSetAcl), arg0, arg1);

    public static string UnableToSetAcl(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (UnableToSetAcl), culture, arg0, arg1);

    public static string UnableToSetPermissionsOnDropDir(object arg0, object arg1) => BuildTypeResource.Format(nameof (UnableToSetPermissionsOnDropDir), arg0, arg1);

    public static string UnableToSetPermissionsOnDropDir(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (UnableToSetPermissionsOnDropDir), culture, arg0, arg1);
    }

    public static string UnexpectedTFServerException(object arg0, object arg1) => BuildTypeResource.Format(nameof (UnexpectedTFServerException), arg0, arg1);

    public static string UnexpectedTFServerException(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (UnexpectedTFServerException), culture, arg0, arg1);

    public static string UPDATE_BUILD() => BuildTypeResource.Get(nameof (UPDATE_BUILD));

    public static string UPDATE_BUILD(CultureInfo culture) => BuildTypeResource.Get(nameof (UPDATE_BUILD), culture);

    public static string VSNotFound() => BuildTypeResource.Get(nameof (VSNotFound));

    public static string VSNotFound(CultureInfo culture) => BuildTypeResource.Get(nameof (VSNotFound), culture);

    public static string ResponseFileReadException(object arg0) => BuildTypeResource.Format(nameof (ResponseFileReadException), arg0);

    public static string ResponseFileReadException(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ResponseFileReadException), culture, arg0);

    public static string ResponseFileWriteException(object arg0) => BuildTypeResource.Format(nameof (ResponseFileWriteException), arg0);

    public static string ResponseFileWriteException(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (ResponseFileWriteException), culture, arg0);

    public static string InvalidAgentStatus(object arg0) => BuildTypeResource.Format(nameof (InvalidAgentStatus), arg0);

    public static string InvalidAgentStatus(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidAgentStatus), culture, arg0);

    public static string Unknown() => BuildTypeResource.Get(nameof (Unknown));

    public static string Unknown(CultureInfo culture) => BuildTypeResource.Get(nameof (Unknown), culture);

    public static string DeleteBuildDefinitionPermission() => BuildTypeResource.Get(nameof (DeleteBuildDefinitionPermission));

    public static string DeleteBuildDefinitionPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteBuildDefinitionPermission), culture);

    public static string DeleteBuildsPermission() => BuildTypeResource.Get(nameof (DeleteBuildsPermission));

    public static string DeleteBuildsPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (DeleteBuildsPermission), culture);

    public static string DestroyBuildsPermission() => BuildTypeResource.Get(nameof (DestroyBuildsPermission));

    public static string DestroyBuildsPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (DestroyBuildsPermission), culture);

    public static string EditBuildDefinitionPermission() => BuildTypeResource.Get(nameof (EditBuildDefinitionPermission));

    public static string EditBuildDefinitionPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (EditBuildDefinitionPermission), culture);

    public static string EditBuildQualityPermission() => BuildTypeResource.Get(nameof (EditBuildQualityPermission));

    public static string EditBuildQualityPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (EditBuildQualityPermission), culture);

    public static string ManageBuildQualitiesPermission() => BuildTypeResource.Get(nameof (ManageBuildQualitiesPermission));

    public static string ManageBuildQualitiesPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (ManageBuildQualitiesPermission), culture);

    public static string ManageBuildQueuePermission() => BuildTypeResource.Get(nameof (ManageBuildQueuePermission));

    public static string ManageBuildQueuePermission(CultureInfo culture) => BuildTypeResource.Get(nameof (ManageBuildQueuePermission), culture);

    public static string ManageBuildResourcesPermission() => BuildTypeResource.Get(nameof (ManageBuildResourcesPermission));

    public static string ManageBuildResourcesPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (ManageBuildResourcesPermission), culture);

    public static string QueueBuildsPermission() => BuildTypeResource.Get(nameof (QueueBuildsPermission));

    public static string QueueBuildsPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (QueueBuildsPermission), culture);

    public static string RetainIndefinitelyPermission() => BuildTypeResource.Get(nameof (RetainIndefinitelyPermission));

    public static string RetainIndefinitelyPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (RetainIndefinitelyPermission), culture);

    public static string StopBuildsPermission() => BuildTypeResource.Get(nameof (StopBuildsPermission));

    public static string StopBuildsPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (StopBuildsPermission), culture);

    public static string UpdateBuildInformationPermission() => BuildTypeResource.Get(nameof (UpdateBuildInformationPermission));

    public static string UpdateBuildInformationPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (UpdateBuildInformationPermission), culture);

    public static string UseBuildResourcesPermission() => BuildTypeResource.Get(nameof (UseBuildResourcesPermission));

    public static string UseBuildResourcesPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (UseBuildResourcesPermission), culture);

    public static string ViewBuildDefinitionPermission() => BuildTypeResource.Get(nameof (ViewBuildDefinitionPermission));

    public static string ViewBuildDefinitionPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (ViewBuildDefinitionPermission), culture);

    public static string ViewBuildResourcesPermission() => BuildTypeResource.Get(nameof (ViewBuildResourcesPermission));

    public static string ViewBuildResourcesPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (ViewBuildResourcesPermission), culture);

    public static string ViewBuildsPermission() => BuildTypeResource.Get(nameof (ViewBuildsPermission));

    public static string ViewBuildsPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (ViewBuildsPermission), culture);

    public static string OverrideBuildCheckInPermission() => BuildTypeResource.Get(nameof (OverrideBuildCheckInPermission));

    public static string OverrideBuildCheckInPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (OverrideBuildCheckInPermission), culture);

    public static string BuildServicesGroupDescription() => BuildTypeResource.Get(nameof (BuildServicesGroupDescription));

    public static string BuildServicesGroupDescription(CultureInfo culture) => BuildTypeResource.Get(nameof (BuildServicesGroupDescription), culture);

    public static string CollectionBuildServicesGroupName() => BuildTypeResource.Get(nameof (CollectionBuildServicesGroupName));

    public static string CollectionBuildServicesGroupName(CultureInfo culture) => BuildTypeResource.Get(nameof (CollectionBuildServicesGroupName), culture);

    public static string InvalidControllerStatus(object arg0) => BuildTypeResource.Format(nameof (InvalidControllerStatus), arg0);

    public static string InvalidControllerStatus(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidControllerStatus), culture, arg0);

    public static string InvalidAccount(object arg0) => BuildTypeResource.Format(nameof (InvalidAccount), arg0);

    public static string InvalidAccount(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidAccount), culture, arg0);

    public static string InvalidContractVersion(object arg0) => BuildTypeResource.Format(nameof (InvalidContractVersion), arg0);

    public static string InvalidContractVersion(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidContractVersion), culture, arg0);

    public static string BuildAdministratorsGroupDescription() => BuildTypeResource.Get(nameof (BuildAdministratorsGroupDescription));

    public static string BuildAdministratorsGroupDescription(CultureInfo culture) => BuildTypeResource.Get(nameof (BuildAdministratorsGroupDescription), culture);

    public static string CollectionBuildAdministratorsGroupName() => BuildTypeResource.Get(nameof (CollectionBuildAdministratorsGroupName));

    public static string CollectionBuildAdministratorsGroupName(CultureInfo culture) => BuildTypeResource.Get(nameof (CollectionBuildAdministratorsGroupName), culture);

    public static string CannotStartBuildGatedInProgress(object arg0, object arg1) => BuildTypeResource.Format(nameof (CannotStartBuildGatedInProgress), arg0, arg1);

    public static string CannotStartBuildGatedInProgress(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildTypeResource.Format(nameof (CannotStartBuildGatedInProgress), culture, arg0, arg1);
    }

    public static string AdministerBuildPermissionsPermission() => BuildTypeResource.Get(nameof (AdministerBuildPermissionsPermission));

    public static string AdministerBuildPermissionsPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (AdministerBuildPermissionsPermission), culture);

    public static string AdministerBuildResourcePermissionsPermission() => BuildTypeResource.Get(nameof (AdministerBuildResourcePermissionsPermission));

    public static string AdministerBuildResourcePermissionsPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (AdministerBuildResourcePermissionsPermission), culture);

    public static string EditPipelineQueueConfigurationPermission() => BuildTypeResource.Get(nameof (EditPipelineQueueConfigurationPermission));

    public static string EditPipelineQueueConfigurationPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (EditPipelineQueueConfigurationPermission), culture);

    public static string InvalidBuildFileContainerId(object arg0) => BuildTypeResource.Format(nameof (InvalidBuildFileContainerId), arg0);

    public static string InvalidBuildFileContainerId(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidBuildFileContainerId), culture, arg0);

    public static string InvalidBuildFileContainerRoot(object arg0) => BuildTypeResource.Format(nameof (InvalidBuildFileContainerRoot), arg0);

    public static string InvalidBuildFileContainerRoot(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidBuildFileContainerRoot), culture, arg0);

    public static string BuildContainerPathNotSupported(object arg0) => BuildTypeResource.Format(nameof (BuildContainerPathNotSupported), arg0);

    public static string BuildContainerPathNotSupported(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildContainerPathNotSupported), culture, arg0);

    public static string InvalidUriToolId(object arg0, object arg1) => BuildTypeResource.Format(nameof (InvalidUriToolId), arg0, arg1);

    public static string InvalidUriToolId(object arg0, object arg1, CultureInfo culture) => BuildTypeResource.Format(nameof (InvalidUriToolId), culture, arg0, arg1);

    public static string BuildParameterDeserializationException(object arg0) => BuildTypeResource.Format(nameof (BuildParameterDeserializationException), arg0);

    public static string BuildParameterDeserializationException(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildParameterDeserializationException), culture, arg0);

    public static string BuildParameterElementNotFoundException(object arg0) => BuildTypeResource.Format(nameof (BuildParameterElementNotFoundException), arg0);

    public static string BuildParameterElementNotFoundException(object arg0, CultureInfo culture) => BuildTypeResource.Format(nameof (BuildParameterElementNotFoundException), culture, arg0);

    public static string ManagePipelinePoliciesPermission() => BuildTypeResource.Get(nameof (ManagePipelinePoliciesPermission));

    public static string ManagePipelinePoliciesPermission(CultureInfo culture) => BuildTypeResource.Get(nameof (ManagePipelinePoliciesPermission), culture);
  }
}
