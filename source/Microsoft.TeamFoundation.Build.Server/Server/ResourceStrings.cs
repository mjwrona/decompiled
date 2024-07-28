// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ResourceStrings
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class ResourceStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ResourceStrings), typeof (ResourceStrings).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ResourceStrings.s_resMgr;

    private static string Get(string resourceName) => ResourceStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.Get(resourceName) : ResourceStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetInt(resourceName) : (int) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetBool(resourceName) : (bool) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ResourceStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ResourceStrings.Get(resourceName, culture);
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

    public static string AccessDeniedAdministratorRequired(object arg0) => ResourceStrings.Format(nameof (AccessDeniedAdministratorRequired), arg0);

    public static string AccessDeniedAdministratorRequired(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (AccessDeniedAdministratorRequired), culture, arg0);

    public static string AccessDeniedForTeamProject(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (AccessDeniedForTeamProject), arg0, arg1, arg2);

    public static string AccessDeniedForTeamProject(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (AccessDeniedForTeamProject), culture, arg0, arg1, arg2);
    }

    public static string AddingAgentsDeprecated() => ResourceStrings.Get(nameof (AddingAgentsDeprecated));

    public static string AddingAgentsDeprecated(CultureInfo culture) => ResourceStrings.Get(nameof (AddingAgentsDeprecated), culture);

    public static string AddingDefinitionsDeprecated() => ResourceStrings.Get(nameof (AddingDefinitionsDeprecated));

    public static string AddingDefinitionsDeprecated(CultureInfo culture) => ResourceStrings.Get(nameof (AddingDefinitionsDeprecated), culture);

    public static string ApplicationStartKeyError(object arg0) => ResourceStrings.Format(nameof (ApplicationStartKeyError), arg0);

    public static string ApplicationStartKeyError(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ApplicationStartKeyError), culture, arg0);

    public static string ArtifactUrlHelp() => ResourceStrings.Get(nameof (ArtifactUrlHelp));

    public static string ArtifactUrlHelp(CultureInfo culture) => ResourceStrings.Get(nameof (ArtifactUrlHelp), culture);

    public static string BuildArtifactTitle(object arg0) => ResourceStrings.Format(nameof (BuildArtifactTitle), arg0);

    public static string BuildArtifactTitle(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildArtifactTitle), culture, arg0);

    public static string BuildCompleted_Canceled(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildCompleted_Canceled), arg0, arg1);

    public static string BuildCompleted_Canceled(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildCompleted_Canceled), culture, arg0, arg1);

    public static string BuildCompleted_Failed(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildCompleted_Failed), arg0, arg1);

    public static string BuildCompleted_Failed(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildCompleted_Failed), culture, arg0, arg1);

    public static string BuildCompleted_PartiallySuccessful(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildCompleted_PartiallySuccessful), arg0, arg1);

    public static string BuildCompleted_PartiallySuccessful(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildCompleted_PartiallySuccessful), culture, arg0, arg1);
    }

    public static string BuildCompleted_Successful(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildCompleted_Successful), arg0, arg1);

    public static string BuildCompleted_Successful(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildCompleted_Successful), culture, arg0, arg1);

    public static string BuildCompleted_Unknown(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (BuildCompleted_Unknown), arg0, arg1, arg2);

    public static string BuildCompleted_Unknown(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildCompleted_Unknown), culture, arg0, arg1, arg2);
    }

    public static string BuildCompletionEmailTitle(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (BuildCompletionEmailTitle), arg0, arg1, arg2);

    public static string BuildCompletionEmailTitle(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildCompletionEmailTitle), culture, arg0, arg1, arg2);
    }

    public static string BuildDefinitionAlreadyExists(object arg0) => ResourceStrings.Format(nameof (BuildDefinitionAlreadyExists), arg0);

    public static string BuildDefinitionAlreadyExists(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildDefinitionAlreadyExists), culture, arg0);

    public static string BuildDefinitionNameInvalid(object arg0) => ResourceStrings.Format(nameof (BuildDefinitionNameInvalid), arg0);

    public static string BuildDefinitionNameInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildDefinitionNameInvalid), culture, arg0);

    public static string BuildDefinitionCacheUpdateError() => ResourceStrings.Get(nameof (BuildDefinitionCacheUpdateError));

    public static string BuildDefinitionCacheUpdateError(CultureInfo culture) => ResourceStrings.Get(nameof (BuildDefinitionCacheUpdateError), culture);

    public static string BuildDefinitionDisabled(object arg0) => ResourceStrings.Format(nameof (BuildDefinitionDisabled), arg0);

    public static string BuildDefinitionDisabled(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildDefinitionDisabled), culture, arg0);

    public static string BuildDefinitionDoesNotExist(object arg0) => ResourceStrings.Format(nameof (BuildDefinitionDoesNotExist), arg0);

    public static string BuildDefinitionDoesNotExist(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildDefinitionDoesNotExist), culture, arg0);

    public static string BuildDoesNotExistForSpec(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildDoesNotExistForSpec), arg0, arg1);

    public static string BuildDoesNotExistForSpec(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildDoesNotExistForSpec), culture, arg0, arg1);

    public static string BuildGroupAlreadyExists(object arg0) => ResourceStrings.Format(nameof (BuildGroupAlreadyExists), arg0);

    public static string BuildGroupAlreadyExists(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildGroupAlreadyExists), culture, arg0);

    public static string BuildGroupDoesNotExist(object arg0) => ResourceStrings.Format(nameof (BuildGroupDoesNotExist), arg0);

    public static string BuildGroupDoesNotExist(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildGroupDoesNotExist), culture, arg0);

    public static string BuildGroupItemCannotMoveAcrossTeamProjects(object arg0) => ResourceStrings.Format(nameof (BuildGroupItemCannotMoveAcrossTeamProjects), arg0);

    public static string BuildGroupItemCannotMoveAcrossTeamProjects(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildGroupItemCannotMoveAcrossTeamProjects), culture, arg0);
    }

    public static string BuildGroupItemInvalidGroup(object arg0) => ResourceStrings.Format(nameof (BuildGroupItemInvalidGroup), arg0);

    public static string BuildGroupItemInvalidGroup(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildGroupItemInvalidGroup), culture, arg0);

    public static string BuildGroupItemNameRequired(object arg0) => ResourceStrings.Format(nameof (BuildGroupItemNameRequired), arg0);

    public static string BuildGroupItemNameRequired(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildGroupItemNameRequired), culture, arg0);

    public static string BuildNumberAlreadyExists(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildNumberAlreadyExists), arg0, arg1);

    public static string BuildNumberAlreadyExists(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildNumberAlreadyExists), culture, arg0, arg1);

    public static string BuildRequestCustomGetVersionIgnored(object arg0) => ResourceStrings.Format(nameof (BuildRequestCustomGetVersionIgnored), arg0);

    public static string BuildRequestCustomGetVersionIgnored(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildRequestCustomGetVersionIgnored), culture, arg0);

    public static string BuildRequestCustomGetVersionRequired(object arg0) => ResourceStrings.Format(nameof (BuildRequestCustomGetVersionRequired), arg0);

    public static string BuildRequestCustomGetVersionRequired(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildRequestCustomGetVersionRequired), culture, arg0);

    public static string BuildRequestGetOptionInvalid(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildRequestGetOptionInvalid), arg0, arg1);

    public static string BuildRequestGetOptionInvalid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildRequestGetOptionInvalid), culture, arg0, arg1);
    }

    public static string BuildRequestReasonInvalid(object arg0) => ResourceStrings.Format(nameof (BuildRequestReasonInvalid), arg0);

    public static string BuildRequestReasonInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildRequestReasonInvalid), culture, arg0);

    public static string BuildRequestReasonReserved(object arg0) => ResourceStrings.Format(nameof (BuildRequestReasonReserved), arg0);

    public static string BuildRequestReasonReserved(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildRequestReasonReserved), culture, arg0);

    public static string BuildRequestShelvesetIgnored(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildRequestShelvesetIgnored), arg0, arg1);

    public static string BuildRequestShelvesetIgnored(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildRequestShelvesetIgnored), culture, arg0, arg1);
    }

    public static string BuildResourceAddedEmailTitle(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildResourceAddedEmailTitle), arg0, arg1);

    public static string BuildResourceAddedEmailTitle(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildResourceAddedEmailTitle), culture, arg0, arg1);
    }

    public static string BuildResourceDeletedEmailTitle(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildResourceDeletedEmailTitle), arg0, arg1);

    public static string BuildResourceDeletedEmailTitle(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildResourceDeletedEmailTitle), culture, arg0, arg1);
    }

    public static string BuildResourceChangedEmailTitle(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildResourceChangedEmailTitle), arg0, arg1);

    public static string BuildResourceChangedEmailTitle(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildResourceChangedEmailTitle), culture, arg0, arg1);
    }

    public static string BuildDefinitionChangedEmailTitle(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildDefinitionChangedEmailTitle), arg0, arg1);

    public static string BuildDefinitionChangedEmailTitle(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildDefinitionChangedEmailTitle), culture, arg0, arg1);
    }

    public static string BuildDefinitionAddedEmailTitle(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildDefinitionAddedEmailTitle), arg0, arg1);

    public static string BuildDefinitionAddedEmailTitle(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildDefinitionAddedEmailTitle), culture, arg0, arg1);
    }

    public static string BuildDefinitionDeletedEmailTitle(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildDefinitionDeletedEmailTitle), arg0, arg1);

    public static string BuildDefinitionDeletedEmailTitle(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildDefinitionDeletedEmailTitle), culture, arg0, arg1);
    }

    public static string BuildStatusChangeEmailTitle(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (BuildStatusChangeEmailTitle), arg0, arg1, arg2);

    public static string BuildStatusChangeEmailTitle(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildStatusChangeEmailTitle), culture, arg0, arg1, arg2);
    }

    public static string BuildStepProjectCompilationMessage(object arg0) => ResourceStrings.Format(nameof (BuildStepProjectCompilationMessage), arg0);

    public static string BuildStepProjectCompilationMessage(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildStepProjectCompilationMessage), culture, arg0);

    public static string BuildStepTopLevelCompilationMessage(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildStepTopLevelCompilationMessage), arg0, arg1);

    public static string BuildStepTopLevelCompilationMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildStepTopLevelCompilationMessage), culture, arg0, arg1);
    }

    public static string BuildStepTopLevelTestMessage(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildStepTopLevelTestMessage), arg0, arg1);

    public static string BuildStepTopLevelTestMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildStepTopLevelTestMessage), culture, arg0, arg1);
    }

    public static string BuildDefinitionUpgradeCompletionEmailTitle(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildDefinitionUpgradeCompletionEmailTitle), arg0, arg1);

    public static string BuildDefinitionUpgradeCompletionEmailTitle(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildDefinitionUpgradeCompletionEmailTitle), culture, arg0, arg1);
    }

    public static string BuildQualityDoesNotExist(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildQualityDoesNotExist), arg0, arg1);

    public static string BuildQualityDoesNotExist(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildQualityDoesNotExist), culture, arg0, arg1);

    public static string BuildStatusMustBeSingleValue(object arg0) => ResourceStrings.Format(nameof (BuildStatusMustBeSingleValue), arg0);

    public static string BuildStatusMustBeSingleValue(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildStatusMustBeSingleValue), culture, arg0);

    public static string CannotDeleteDefinitionBuildExists(object arg0) => ResourceStrings.Format(nameof (CannotDeleteDefinitionBuildExists), arg0);

    public static string CannotDeleteDefinitionBuildExists(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotDeleteDefinitionBuildExists), culture, arg0);

    public static string CannotDeleteDefinitionQueuedBuildExists(object arg0, object arg1) => ResourceStrings.Format(nameof (CannotDeleteDefinitionQueuedBuildExists), arg0, arg1);

    public static string CannotDeleteDefinitionQueuedBuildExists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (CannotDeleteDefinitionQueuedBuildExists), culture, arg0, arg1);
    }

    public static string CannotDeleteInProgressBuild(object arg0) => ResourceStrings.Format(nameof (CannotDeleteInProgressBuild), arg0);

    public static string CannotDeleteInProgressBuild(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotDeleteInProgressBuild), culture, arg0);

    public static string CannotSetBatchSizeInvalidType(object arg0) => ResourceStrings.Format(nameof (CannotSetBatchSizeInvalidType), arg0);

    public static string CannotSetBatchSizeInvalidType(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotSetBatchSizeInvalidType), culture, arg0);

    public static string CannotSetQuietPeriodInvalidType(object arg0) => ResourceStrings.Format(nameof (CannotSetQuietPeriodInvalidType), arg0);

    public static string CannotSetQuietPeriodInvalidType(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotSetQuietPeriodInvalidType), culture, arg0);

    public static string CannotStopBuildNotStartedOnAgent(object arg0, object arg1) => ResourceStrings.Format(nameof (CannotStopBuildNotStartedOnAgent), arg0, arg1);

    public static string CannotStopBuildNotStartedOnAgent(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (CannotStopBuildNotStartedOnAgent), culture, arg0, arg1);
    }

    public static string CannotUpdateBatchedDefinitionFromOldClient(object arg0) => ResourceStrings.Format(nameof (CannotUpdateBatchedDefinitionFromOldClient), arg0);

    public static string CannotUpdateBatchedDefinitionFromOldClient(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (CannotUpdateBatchedDefinitionFromOldClient), culture, arg0);
    }

    public static string CoverageServiceRestartFailureDuringDelete() => ResourceStrings.Get(nameof (CoverageServiceRestartFailureDuringDelete));

    public static string CoverageServiceRestartFailureDuringDelete(CultureInfo culture) => ResourceStrings.Get(nameof (CoverageServiceRestartFailureDuringDelete), culture);

    public static string DateTimeOutOfRange(object arg0) => ResourceStrings.Format(nameof (DateTimeOutOfRange), arg0);

    public static string DateTimeOutOfRange(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (DateTimeOutOfRange), culture, arg0);

    public static string DeletingAgentsDeprecated() => ResourceStrings.Get(nameof (DeletingAgentsDeprecated));

    public static string DeletingAgentsDeprecated(CultureInfo culture) => ResourceStrings.Get(nameof (DeletingAgentsDeprecated), culture);

    public static string DuplicateBuildUpdateRequestException(object arg0) => ResourceStrings.Format(nameof (DuplicateBuildUpdateRequestException), arg0);

    public static string DuplicateBuildUpdateRequestException(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (DuplicateBuildUpdateRequestException), culture, arg0);

    public static string DuplicateRetentionPolicyDefinition(object arg0, object arg1) => ResourceStrings.Format(nameof (DuplicateRetentionPolicyDefinition), arg0, arg1);

    public static string DuplicateRetentionPolicyDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (DuplicateRetentionPolicyDefinition), culture, arg0, arg1);
    }

    public static string DuplicateWorkspaceMappingDefinition(object arg0) => ResourceStrings.Format(nameof (DuplicateWorkspaceMappingDefinition), arg0);

    public static string DuplicateWorkspaceMappingDefinition(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (DuplicateWorkspaceMappingDefinition), culture, arg0);

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_InitialTestPassed() => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_InitialTestPassed));

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_InitialTestPassed(CultureInfo culture) => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_InitialTestPassed), culture);

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_LabTestPassed() => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_LabTestPassed));

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_LabTestPassed(CultureInfo culture) => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_LabTestPassed), culture);

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForDeployment() => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForDeployment));

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForDeployment(CultureInfo culture) => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForDeployment), culture);

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForInitialTest() => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForInitialTest));

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForInitialTest(
      CultureInfo culture)
    {
      return ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForInitialTest), culture);
    }

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_Rejected() => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_Rejected));

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_Rejected(CultureInfo culture) => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_Rejected), culture);

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_Released() => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_Released));

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_Released(CultureInfo culture) => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_Released), culture);

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_UATPassed() => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_UATPassed));

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_UATPassed(CultureInfo culture) => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_UATPassed), culture);

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_UnderInvestigation() => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_UnderInvestigation));

    public static string EDA8CA26_B64A_44c1_A1D5_42451DD97486_UnderInvestigation(CultureInfo culture) => ResourceStrings.Get(nameof (EDA8CA26_B64A_44c1_A1D5_42451DD97486_UnderInvestigation), culture);

    public static string FailedToProcessChangeset(object arg0) => ResourceStrings.Format(nameof (FailedToProcessChangeset), arg0);

    public static string FailedToProcessChangeset(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FailedToProcessChangeset), culture, arg0);

    public static string FailedToUpgradeBuildType(object arg0) => ResourceStrings.Format(nameof (FailedToUpgradeBuildType), arg0);

    public static string FailedToUpgradeBuildType(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FailedToUpgradeBuildType), culture, arg0);

    public static string FailedToUpgradeBuildTypes(object arg0) => ResourceStrings.Format(nameof (FailedToUpgradeBuildTypes), arg0);

    public static string FailedToUpgradeBuildTypes(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FailedToUpgradeBuildTypes), culture, arg0);

    public static string FailedToUpdateProcessParameters(object arg0) => ResourceStrings.Format(nameof (FailedToUpdateProcessParameters), arg0);

    public static string FailedToUpdateProcessParameters(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FailedToUpdateProcessParameters), culture, arg0);

    public static string GatedCheckInTicketValidationError() => ResourceStrings.Get(nameof (GatedCheckInTicketValidationError));

    public static string GatedCheckInTicketValidationError(CultureInfo culture) => ResourceStrings.Get(nameof (GatedCheckInTicketValidationError), culture);

    public static string GatedCheckInTicketExpired() => ResourceStrings.Get(nameof (GatedCheckInTicketExpired));

    public static string GatedCheckInTicketExpired(CultureInfo culture) => ResourceStrings.Get(nameof (GatedCheckInTicketExpired), culture);

    public static string GatedCheckInTicketInvalidDefinition() => ResourceStrings.Get(nameof (GatedCheckInTicketInvalidDefinition));

    public static string GatedCheckInTicketInvalidDefinition(CultureInfo culture) => ResourceStrings.Get(nameof (GatedCheckInTicketInvalidDefinition), culture);

    public static string GatedCheckInTicketInvalidOwner(object arg0) => ResourceStrings.Format(nameof (GatedCheckInTicketInvalidOwner), arg0);

    public static string GatedCheckInTicketInvalidOwner(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (GatedCheckInTicketInvalidOwner), culture, arg0);

    public static string GatedCheckInTicketInvalidShelveset() => ResourceStrings.Get(nameof (GatedCheckInTicketInvalidShelveset));

    public static string GatedCheckInTicketInvalidShelveset(CultureInfo culture) => ResourceStrings.Get(nameof (GatedCheckInTicketInvalidShelveset), culture);

    public static string GatedCheckInTicketShelvesetModified() => ResourceStrings.Get(nameof (GatedCheckInTicketShelvesetModified));

    public static string GatedCheckInTicketShelvesetModified(CultureInfo culture) => ResourceStrings.Get(nameof (GatedCheckInTicketShelvesetModified), culture);

    public static string InformationAddRequestCycleDetected(object arg0, object arg1) => ResourceStrings.Format(nameof (InformationAddRequestCycleDetected), arg0, arg1);

    public static string InformationAddRequestCycleDetected(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (InformationAddRequestCycleDetected), culture, arg0, arg1);
    }

    public static string InformationChangeDuplicateRequest(object arg0, object arg1) => ResourceStrings.Format(nameof (InformationChangeDuplicateRequest), arg0, arg1);

    public static string InformationChangeDuplicateRequest(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (InformationChangeDuplicateRequest), culture, arg0, arg1);
    }

    public static string InformationNodeDoesNotExist(object arg0, object arg1) => ResourceStrings.Format(nameof (InformationNodeDoesNotExist), arg0, arg1);

    public static string InformationNodeDoesNotExist(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (InformationNodeDoesNotExist), culture, arg0, arg1);

    public static string InformationParentNodeDoesNotExist(object arg0, object arg1) => ResourceStrings.Format(nameof (InformationParentNodeDoesNotExist), arg0, arg1);

    public static string InformationParentNodeDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (InformationParentNodeDoesNotExist), culture, arg0, arg1);
    }

    public static string InvalidBuildRequest(object arg0) => ResourceStrings.Format(nameof (InvalidBuildRequest), arg0);

    public static string InvalidBuildRequest(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidBuildRequest), culture, arg0);

    public static string InvalidBuildUri(object arg0) => ResourceStrings.Format(nameof (InvalidBuildUri), arg0);

    public static string InvalidBuildUri(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidBuildUri), culture, arg0);

    public static string InvalidContinuousIntegrationType(object arg0) => ResourceStrings.Format(nameof (InvalidContinuousIntegrationType), arg0);

    public static string InvalidContinuousIntegrationType(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidContinuousIntegrationType), culture, arg0);

    public static string InvalidCustomAssemblyPath() => ResourceStrings.Get(nameof (InvalidCustomAssemblyPath));

    public static string InvalidCustomAssemblyPath(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidCustomAssemblyPath), culture);

    public static string InvalidDeleteOptions(object arg0) => ResourceStrings.Format(nameof (InvalidDeleteOptions), arg0);

    public static string InvalidDeleteOptions(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidDeleteOptions), culture, arg0);

    public static string InvalidDescription() => ResourceStrings.Get(nameof (InvalidDescription));

    public static string InvalidDescription(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidDescription), culture);

    public static string InvalidIdentityName(object arg0) => ResourceStrings.Format(nameof (InvalidIdentityName), arg0);

    public static string InvalidIdentityName(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidIdentityName), culture, arg0);

    public static string InvalidLocalItem(object arg0, object arg1) => ResourceStrings.Format(nameof (InvalidLocalItem), arg0, arg1);

    public static string InvalidLocalItem(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidLocalItem), culture, arg0, arg1);

    public static string InvalidPlatformFlavorSpecified(object arg0, object arg1) => ResourceStrings.Format(nameof (InvalidPlatformFlavorSpecified), arg0, arg1);

    public static string InvalidPlatformFlavorSpecified(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (InvalidPlatformFlavorSpecified), culture, arg0, arg1);
    }

    public static string InvalidQuality() => ResourceStrings.Get(nameof (InvalidQuality));

    public static string InvalidQuality(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidQuality), culture);

    public static string InvalidTeamProject() => ResourceStrings.Get(nameof (InvalidTeamProject));

    public static string InvalidTeamProject(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidTeamProject), culture);

    public static string InvalidWorkspaceMappingCannotContainWildcards(object arg0) => ResourceStrings.Format(nameof (InvalidWorkspaceMappingCannotContainWildcards), arg0);

    public static string InvalidWorkspaceMappingCannotContainWildcards(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (InvalidWorkspaceMappingCannotContainWildcards), culture, arg0);
    }

    public static string InvalidWorkspaceMappingDepth(object arg0) => ResourceStrings.Format(nameof (InvalidWorkspaceMappingDepth), arg0);

    public static string InvalidWorkspaceMappingDepth(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidWorkspaceMappingDepth), culture, arg0);

    public static string LabelNameInvalid(object arg0) => ResourceStrings.Format(nameof (LabelNameInvalid), arg0);

    public static string LabelNameInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (LabelNameInvalid), culture, arg0);

    public static string LabelRequiresScope(object arg0) => ResourceStrings.Format(nameof (LabelRequiresScope), arg0);

    public static string LabelRequiresScope(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (LabelRequiresScope), culture, arg0);

    public static string MissingController() => ResourceStrings.Get(nameof (MissingController));

    public static string MissingController(CultureInfo culture) => ResourceStrings.Get(nameof (MissingController), culture);

    public static string MissingDefaultDropLocation() => ResourceStrings.Get(nameof (MissingDefaultDropLocation));

    public static string MissingDefaultDropLocation(CultureInfo culture) => ResourceStrings.Get(nameof (MissingDefaultDropLocation), culture);

    public static string MissingDefinition() => ResourceStrings.Get(nameof (MissingDefinition));

    public static string MissingDefinition(CultureInfo culture) => ResourceStrings.Get(nameof (MissingDefinition), culture);

    public static string MissingDescription() => ResourceStrings.Get(nameof (MissingDescription));

    public static string MissingDescription(CultureInfo culture) => ResourceStrings.Get(nameof (MissingDescription), culture);

    public static string MissingQuality() => ResourceStrings.Get(nameof (MissingQuality));

    public static string MissingQuality(CultureInfo culture) => ResourceStrings.Get(nameof (MissingQuality), culture);

    public static string MissingUri() => ResourceStrings.Get(nameof (MissingUri));

    public static string MissingUri(CultureInfo culture) => ResourceStrings.Get(nameof (MissingUri), culture);

    public static string ProcessingChangesetDeprecated() => ResourceStrings.Get(nameof (ProcessingChangesetDeprecated));

    public static string ProcessingChangesetDeprecated(CultureInfo culture) => ResourceStrings.Get(nameof (ProcessingChangesetDeprecated), culture);

    public static string RetentionPolicyErrors(object arg0, object arg1) => ResourceStrings.Format(nameof (RetentionPolicyErrors), arg0, arg1);

    public static string RetentionPolicyErrors(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (RetentionPolicyErrors), culture, arg0, arg1);

    public static string RetentionPolicyInvalidReason(object arg0) => ResourceStrings.Format(nameof (RetentionPolicyInvalidReason), arg0);

    public static string RetentionPolicyInvalidReason(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (RetentionPolicyInvalidReason), culture, arg0);

    public static string RetentionPolicyInvalidStatus(object arg0) => ResourceStrings.Format(nameof (RetentionPolicyInvalidStatus), arg0);

    public static string RetentionPolicyInvalidStatus(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (RetentionPolicyInvalidStatus), culture, arg0);

    public static string ScheduleJobName() => ResourceStrings.Get(nameof (ScheduleJobName));

    public static string ScheduleJobName(CultureInfo culture) => ResourceStrings.Get(nameof (ScheduleJobName), culture);

    public static string ScheduleLimitExceeded() => ResourceStrings.Get(nameof (ScheduleLimitExceeded));

    public static string ScheduleLimitExceeded(CultureInfo culture) => ResourceStrings.Get(nameof (ScheduleLimitExceeded), culture);

    public static string SourceProviderLimitExceeded() => ResourceStrings.Get(nameof (SourceProviderLimitExceeded));

    public static string SourceProviderLimitExceeded(CultureInfo culture) => ResourceStrings.Get(nameof (SourceProviderLimitExceeded), culture);

    public static string StopBuildTimeout(object arg0, object arg1) => ResourceStrings.Format(nameof (StopBuildTimeout), arg0, arg1);

    public static string StopBuildTimeout(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (StopBuildTimeout), culture, arg0, arg1);

    public static string TfsBuildProjTargetImportUpdateComment(object arg0) => ResourceStrings.Format(nameof (TfsBuildProjTargetImportUpdateComment), arg0);

    public static string TfsBuildProjTargetImportUpdateComment(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (TfsBuildProjTargetImportUpdateComment), culture, arg0);

    public static string UnexpectedError() => ResourceStrings.Get(nameof (UnexpectedError));

    public static string UnexpectedError(CultureInfo culture) => ResourceStrings.Get(nameof (UnexpectedError), culture);

    public static string UnknownPlaceHolder() => ResourceStrings.Get(nameof (UnknownPlaceHolder));

    public static string UnknownPlaceHolder(CultureInfo culture) => ResourceStrings.Get(nameof (UnknownPlaceHolder), culture);

    public static string UpdatingAgentsDeprecated() => ResourceStrings.Get(nameof (UpdatingAgentsDeprecated));

    public static string UpdatingAgentsDeprecated(CultureInfo culture) => ResourceStrings.Get(nameof (UpdatingAgentsDeprecated), culture);

    public static string UpdatingDefinitionsDeprecated() => ResourceStrings.Get(nameof (UpdatingDefinitionsDeprecated));

    public static string UpdatingDefinitionsDeprecated(CultureInfo culture) => ResourceStrings.Get(nameof (UpdatingDefinitionsDeprecated), culture);

    public static string UpdatingBuildInformationDeprecated() => ResourceStrings.Get(nameof (UpdatingBuildInformationDeprecated));

    public static string UpdatingBuildInformationDeprecated(CultureInfo culture) => ResourceStrings.Get(nameof (UpdatingBuildInformationDeprecated), culture);

    public static string UpgradeNoControllers(object arg0) => ResourceStrings.Format(nameof (UpgradeNoControllers), arg0);

    public static string UpgradeNoControllers(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (UpgradeNoControllers), culture, arg0);

    public static string UpgradeWorkspaceComment() => ResourceStrings.Get(nameof (UpgradeWorkspaceComment));

    public static string UpgradeWorkspaceComment(CultureInfo culture) => ResourceStrings.Get(nameof (UpgradeWorkspaceComment), culture);

    public static string VersionControlFileNotFoundException(object arg0) => ResourceStrings.Format(nameof (VersionControlFileNotFoundException), arg0);

    public static string VersionControlFileNotFoundException(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (VersionControlFileNotFoundException), culture, arg0);

    public static string WorkspaceTemplateMustDefineMapping() => ResourceStrings.Get(nameof (WorkspaceTemplateMustDefineMapping));

    public static string WorkspaceTemplateMustDefineMapping(CultureInfo culture) => ResourceStrings.Get(nameof (WorkspaceTemplateMustDefineMapping), culture);

    public static string AccessDeniedForBuildDefinition(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ResourceStrings.Format(nameof (AccessDeniedForBuildDefinition), arg0, arg1, arg2, arg3);
    }

    public static string AccessDeniedForBuildDefinition(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (AccessDeniedForBuildDefinition), culture, arg0, arg1, arg2, arg3);
    }

    public static string FailedToUpdateBuildControllers(object arg0) => ResourceStrings.Format(nameof (FailedToUpdateBuildControllers), arg0);

    public static string FailedToUpdateBuildControllers(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FailedToUpdateBuildControllers), culture, arg0);

    public static string MessageQueueDescription(object arg0) => ResourceStrings.Format(nameof (MessageQueueDescription), arg0);

    public static string MessageQueueDescription(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (MessageQueueDescription), culture, arg0);

    public static string BuildDefinitionCanOnlyBeSpecifiedOnce(object arg0) => ResourceStrings.Format(nameof (BuildDefinitionCanOnlyBeSpecifiedOnce), arg0);

    public static string BuildDefinitionCanOnlyBeSpecifiedOnce(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildDefinitionCanOnlyBeSpecifiedOnce), culture, arg0);

    public static string InvalidSharedResourceRequestedBy(object arg0) => ResourceStrings.Format(nameof (InvalidSharedResourceRequestedBy), arg0);

    public static string InvalidSharedResourceRequestedBy(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidSharedResourceRequestedBy), culture, arg0);

    public static string InvalidIdentityNotFound(object arg0) => ResourceStrings.Format(nameof (InvalidIdentityNotFound), arg0);

    public static string InvalidIdentityNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidIdentityNotFound), culture, arg0);

    public static string UpgradeTemplateDoesNotSupportBatching(object arg0, object arg1) => ResourceStrings.Format(nameof (UpgradeTemplateDoesNotSupportBatching), arg0, arg1);

    public static string UpgradeTemplateDoesNotSupportBatching(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (UpgradeTemplateDoesNotSupportBatching), culture, arg0, arg1);
    }

    public static string BuildElasticAgentName() => ResourceStrings.Get(nameof (BuildElasticAgentName));

    public static string BuildElasticAgentName(CultureInfo culture) => ResourceStrings.Get(nameof (BuildElasticAgentName), culture);

    public static string BuildElasticControllerName() => ResourceStrings.Get(nameof (BuildElasticControllerName));

    public static string BuildElasticControllerName(CultureInfo culture) => ResourceStrings.Get(nameof (BuildElasticControllerName), culture);

    public static string BuildElasticServiceHostName() => ResourceStrings.Get(nameof (BuildElasticServiceHostName));

    public static string BuildElasticServiceHostName(CultureInfo culture) => ResourceStrings.Get(nameof (BuildElasticServiceHostName), culture);

    public static string InvalidVersionControlDropLocation(object arg0, object arg1) => ResourceStrings.Format(nameof (InvalidVersionControlDropLocation), arg0, arg1);

    public static string InvalidVersionControlDropLocation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (InvalidVersionControlDropLocation), culture, arg0, arg1);
    }

    public static string RequestLogs_BuildNotInProgress(object arg0) => ResourceStrings.Format(nameof (RequestLogs_BuildNotInProgress), arg0);

    public static string RequestLogs_BuildNotInProgress(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (RequestLogs_BuildNotInProgress), culture, arg0);

    public static string RequestLogs_DropLocationNotSpecified(object arg0) => ResourceStrings.Format(nameof (RequestLogs_DropLocationNotSpecified), arg0);

    public static string RequestLogs_DropLocationNotSpecified(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (RequestLogs_DropLocationNotSpecified), culture, arg0);

    public static string MissingRequiredDeploymentEnvironmentProperty() => ResourceStrings.Get(nameof (MissingRequiredDeploymentEnvironmentProperty));

    public static string MissingRequiredDeploymentEnvironmentProperty(CultureInfo culture) => ResourceStrings.Get(nameof (MissingRequiredDeploymentEnvironmentProperty), culture);

    public static string InvalidMachinePoolName() => ResourceStrings.Get(nameof (InvalidMachinePoolName));

    public static string InvalidMachinePoolName(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidMachinePoolName), culture);

    public static string InvalidVersionControlDefaultDropLocation(
      object arg0,
      object arg1,
      object arg2)
    {
      return ResourceStrings.Format(nameof (InvalidVersionControlDefaultDropLocation), arg0, arg1, arg2);
    }

    public static string InvalidVersionControlDefaultDropLocation(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (InvalidVersionControlDefaultDropLocation), culture, arg0, arg1, arg2);
    }

    public static string DeploymentEnvironmentNotFound(object arg0) => ResourceStrings.Format(nameof (DeploymentEnvironmentNotFound), arg0);

    public static string DeploymentEnvironmentNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (DeploymentEnvironmentNotFound), culture, arg0);

    public static string BuildDeploymentServiceNotSupported() => ResourceStrings.Get(nameof (BuildDeploymentServiceNotSupported));

    public static string BuildDeploymentServiceNotSupported(CultureInfo culture) => ResourceStrings.Get(nameof (BuildDeploymentServiceNotSupported), culture);

    public static string CannotRedeployBuildNotSucceeded() => ResourceStrings.Get(nameof (CannotRedeployBuildNotSucceeded));

    public static string CannotRedeployBuildNotSucceeded(CultureInfo culture) => ResourceStrings.Get(nameof (CannotRedeployBuildNotSucceeded), culture);

    public static string CannotRedeployBuildReason() => ResourceStrings.Get(nameof (CannotRedeployBuildReason));

    public static string CannotRedeployBuildReason(CultureInfo culture) => ResourceStrings.Get(nameof (CannotRedeployBuildReason), culture);

    public static string CannotRedeployNondeploymentBuild() => ResourceStrings.Get(nameof (CannotRedeployNondeploymentBuild));

    public static string CannotRedeployNondeploymentBuild(CultureInfo culture) => ResourceStrings.Get(nameof (CannotRedeployNondeploymentBuild), culture);

    public static string BuildMachineRequestDoesNotExistException(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildMachineRequestDoesNotExistException), arg0, arg1);

    public static string BuildMachineRequestDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildMachineRequestDoesNotExistException), culture, arg0, arg1);
    }

    public static string AddingServiceHostRequiresCurrentClient() => ResourceStrings.Get(nameof (AddingServiceHostRequiresCurrentClient));

    public static string AddingServiceHostRequiresCurrentClient(CultureInfo culture) => ResourceStrings.Get(nameof (AddingServiceHostRequiresCurrentClient), culture);

    public static string CannotAddPropertyToNondeploymentBuild() => ResourceStrings.Get(nameof (CannotAddPropertyToNondeploymentBuild));

    public static string CannotAddPropertyToNondeploymentBuild(CultureInfo culture) => ResourceStrings.Get(nameof (CannotAddPropertyToNondeploymentBuild), culture);

    public static string FailedToExtractArtifactSpecFromUri() => ResourceStrings.Get(nameof (FailedToExtractArtifactSpecFromUri));

    public static string FailedToExtractArtifactSpecFromUri(CultureInfo culture) => ResourceStrings.Get(nameof (FailedToExtractArtifactSpecFromUri), culture);

    public static string CannotRedeployBuildNotFound() => ResourceStrings.Get(nameof (CannotRedeployBuildNotFound));

    public static string CannotRedeployBuildNotFound(CultureInfo culture) => ResourceStrings.Get(nameof (CannotRedeployBuildNotFound), culture);

    public static string AzureContinuousDeploymentTemplateNotFound(object arg0) => ResourceStrings.Format(nameof (AzureContinuousDeploymentTemplateNotFound), arg0);

    public static string AzureContinuousDeploymentTemplateNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (AzureContinuousDeploymentTemplateNotFound), culture, arg0);

    public static string AgentHostShutdownForcefullyError(object arg0) => ResourceStrings.Format(nameof (AgentHostShutdownForcefullyError), arg0);

    public static string AgentHostShutdownForcefullyError(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (AgentHostShutdownForcefullyError), culture, arg0);

    public static string CannotCreateCDBuildDefWithoutHostedController() => ResourceStrings.Get(nameof (CannotCreateCDBuildDefWithoutHostedController));

    public static string CannotCreateCDBuildDefWithoutHostedController(CultureInfo culture) => ResourceStrings.Get(nameof (CannotCreateCDBuildDefWithoutHostedController), culture);

    public static string InvalidBuildContainerDropLocation(object arg0, object arg1) => ResourceStrings.Format(nameof (InvalidBuildContainerDropLocation), arg0, arg1);

    public static string InvalidBuildContainerDropLocation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (InvalidBuildContainerDropLocation), culture, arg0, arg1);
    }

    public static string ServerDropMustDeleteDrop() => ResourceStrings.Get(nameof (ServerDropMustDeleteDrop));

    public static string ServerDropMustDeleteDrop(CultureInfo culture) => ResourceStrings.Get(nameof (ServerDropMustDeleteDrop), culture);

    public static string ElasticMessageQueueDescription(object arg0, object arg1) => ResourceStrings.Format(nameof (ElasticMessageQueueDescription), arg0, arg1);

    public static string ElasticMessageQueueDescription(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (ElasticMessageQueueDescription), culture, arg0, arg1);
    }

    public static string BuildMachineRequestInProgressException(
      object arg0,
      object arg1,
      object arg2)
    {
      return ResourceStrings.Format(nameof (BuildMachineRequestInProgressException), arg0, arg1, arg2);
    }

    public static string BuildMachineRequestInProgressException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildMachineRequestInProgressException), culture, arg0, arg1, arg2);
    }

    public static string BuildContainerDropMustUseVirtual(object arg0) => ResourceStrings.Format(nameof (BuildContainerDropMustUseVirtual), arg0);

    public static string BuildContainerDropMustUseVirtual(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildContainerDropMustUseVirtual), culture, arg0);

    public static string BuildContainerDropNotSupported(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildContainerDropNotSupported), arg0, arg1);

    public static string BuildContainerDropNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildContainerDropNotSupported), culture, arg0, arg1);
    }

    public static string BuildMachineRequestAlreadyFinishedException(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildMachineRequestAlreadyFinishedException), arg0, arg1);

    public static string BuildMachineRequestAlreadyFinishedException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildMachineRequestAlreadyFinishedException), culture, arg0, arg1);
    }

    public static string BuildMachineRequestAlreadyStartedException(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildMachineRequestAlreadyStartedException), arg0, arg1);

    public static string BuildMachineRequestAlreadyStartedException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildMachineRequestAlreadyStartedException), culture, arg0, arg1);
    }

    public static string BuildFailedAlert(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildFailedAlert), arg0, arg1);

    public static string BuildFailedAlert(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildFailedAlert), culture, arg0, arg1);

    public static string BuildMachineSlowReimage(object arg0) => ResourceStrings.Format(nameof (BuildMachineSlowReimage), arg0);

    public static string BuildMachineSlowReimage(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildMachineSlowReimage), culture, arg0);

    public static string BuildOkNumberWaitingInQueue(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildOkNumberWaitingInQueue), arg0, arg1);

    public static string BuildOkNumberWaitingInQueue(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildOkNumberWaitingInQueue), culture, arg0, arg1);

    public static string BuildTooManyWaitingInQueue(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildTooManyWaitingInQueue), arg0, arg1);

    public static string BuildTooManyWaitingInQueue(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildTooManyWaitingInQueue), culture, arg0, arg1);

    public static string BuildWaitedInQueueTooLong(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildWaitedInQueueTooLong), arg0, arg1);

    public static string BuildWaitedInQueueTooLong(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildWaitedInQueueTooLong), culture, arg0, arg1);

    public static string MachineCountGreen(object arg0, object arg1) => ResourceStrings.Format(nameof (MachineCountGreen), arg0, arg1);

    public static string MachineCountGreen(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (MachineCountGreen), culture, arg0, arg1);

    public static string MachineCountRed(object arg0, object arg1) => ResourceStrings.Format(nameof (MachineCountRed), arg0, arg1);

    public static string MachineCountRed(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (MachineCountRed), culture, arg0, arg1);

    public static string MachineCountYellow(object arg0, object arg1) => ResourceStrings.Format(nameof (MachineCountYellow), arg0, arg1);

    public static string MachineCountYellow(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (MachineCountYellow), culture, arg0, arg1);

    public static string numberOfAlertsGenerated(object arg0) => ResourceStrings.Format(nameof (numberOfAlertsGenerated), arg0);

    public static string numberOfAlertsGenerated(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (numberOfAlertsGenerated), culture, arg0);

    public static string BuildTimeoutAlert(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildTimeoutAlert), arg0, arg1);

    public static string BuildTimeoutAlert(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildTimeoutAlert), culture, arg0, arg1);

    public static string GitTemplateDescription() => ResourceStrings.Get(nameof (GitTemplateDescription));

    public static string GitTemplateDescription(CultureInfo culture) => ResourceStrings.Get(nameof (GitTemplateDescription), culture);

    public static string GitTemplate12Description() => ResourceStrings.Get(nameof (GitTemplate12Description));

    public static string GitTemplate12Description(CultureInfo culture) => ResourceStrings.Get(nameof (GitTemplate12Description), culture);

    public static string TemplateMissingFromFileContainer(object arg0, object arg1) => ResourceStrings.Format(nameof (TemplateMissingFromFileContainer), arg0, arg1);

    public static string TemplateMissingFromFileContainer(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (TemplateMissingFromFileContainer), culture, arg0, arg1);
    }

    public static string FailedToProcessDeletedFiles(object arg0) => ResourceStrings.Format(nameof (FailedToProcessDeletedFiles), arg0);

    public static string FailedToProcessDeletedFiles(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FailedToProcessDeletedFiles), culture, arg0);

    public static string FailedToProcessUploadedFile(object arg0, object arg1) => ResourceStrings.Format(nameof (FailedToProcessUploadedFile), arg0, arg1);

    public static string FailedToProcessUploadedFile(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (FailedToProcessUploadedFile), culture, arg0, arg1);

    public static string IntermediateLogsNotSupported(object arg0) => ResourceStrings.Format(nameof (IntermediateLogsNotSupported), arg0);

    public static string IntermediateLogsNotSupported(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (IntermediateLogsNotSupported), culture, arg0);

    public static string CannotBatchWith2010(object arg0) => ResourceStrings.Format(nameof (CannotBatchWith2010), arg0);

    public static string CannotBatchWith2010(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotBatchWith2010), culture, arg0);

    public static string IncompatibleController(object arg0) => ResourceStrings.Format(nameof (IncompatibleController), arg0);

    public static string IncompatibleController(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (IncompatibleController), culture, arg0);

    public static string InvalidRepoUri(object arg0) => ResourceStrings.Format(nameof (InvalidRepoUri), arg0);

    public static string InvalidRepoUri(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidRepoUri), culture, arg0);

    public static string FailedToStartBuild(object arg0) => ResourceStrings.Format(nameof (FailedToStartBuild), arg0);

    public static string FailedToStartBuild(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FailedToStartBuild), culture, arg0);

    public static string TooLongSinceLastCheck(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (TooLongSinceLastCheck), arg0, arg1, arg2);

    public static string TooLongSinceLastCheck(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (TooLongSinceLastCheck), culture, arg0, arg1, arg2);
    }

    public static string BuildProcessTemplateNotFound(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildProcessTemplateNotFound), arg0, arg1);

    public static string BuildProcessTemplateNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildProcessTemplateNotFound), culture, arg0, arg1);
    }

    public static string FatalErrorDeployingBuildTemplate(object arg0, object arg1) => ResourceStrings.Format(nameof (FatalErrorDeployingBuildTemplate), arg0, arg1);

    public static string FatalErrorDeployingBuildTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (FatalErrorDeployingBuildTemplate), culture, arg0, arg1);
    }

    public static string ServiceCallFailedAuthentication() => ResourceStrings.Get(nameof (ServiceCallFailedAuthentication));

    public static string ServiceCallFailedAuthentication(CultureInfo culture) => ResourceStrings.Get(nameof (ServiceCallFailedAuthentication), culture);

    public static string ServiceAccountMissingPermissions(object arg0) => ResourceStrings.Format(nameof (ServiceAccountMissingPermissions), arg0);

    public static string ServiceAccountMissingPermissions(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ServiceAccountMissingPermissions), culture, arg0);

    public static string TfvcTemplate12Description() => ResourceStrings.Get(nameof (TfvcTemplate12Description));

    public static string TfvcTemplate12Description(CultureInfo culture) => ResourceStrings.Get(nameof (TfvcTemplate12Description), culture);

    public static string UpgradeTemplateDescription() => ResourceStrings.Get(nameof (UpgradeTemplateDescription));

    public static string UpgradeTemplateDescription(CultureInfo culture) => ResourceStrings.Get(nameof (UpgradeTemplateDescription), culture);

    public static string AzureTemplateDescription() => ResourceStrings.Get(nameof (AzureTemplateDescription));

    public static string AzureTemplateDescription(CultureInfo culture) => ResourceStrings.Get(nameof (AzureTemplateDescription), culture);

    public static string InvalidGitUri(object arg0) => ResourceStrings.Format(nameof (InvalidGitUri), arg0);

    public static string InvalidGitUri(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidGitUri), culture, arg0);

    public static string UnknownTemplate(object arg0) => ResourceStrings.Format(nameof (UnknownTemplate), arg0);

    public static string UnknownTemplate(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (UnknownTemplate), culture, arg0);

    public static string UnableToFindControllersServiceHost(object arg0, object arg1) => ResourceStrings.Format(nameof (UnableToFindControllersServiceHost), arg0, arg1);

    public static string UnableToFindControllersServiceHost(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (UnableToFindControllersServiceHost), culture, arg0, arg1);
    }

    public static string MachineRequestFailedAlert(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ResourceStrings.Format(nameof (MachineRequestFailedAlert), arg0, arg1, arg2, arg3);
    }

    public static string MachineRequestFailedAlert(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (MachineRequestFailedAlert), culture, arg0, arg1, arg2, arg3);
    }

    public static string BuildNotFound(object arg0) => ResourceStrings.Format(nameof (BuildNotFound), arg0);

    public static string BuildNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildNotFound), culture, arg0);

    public static string BuildRequestNotFound(object arg0) => ResourceStrings.Format(nameof (BuildRequestNotFound), arg0);

    public static string BuildRequestNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildRequestNotFound), culture, arg0);

    public static string BuildDefinitionIdInvalid(object arg0) => ResourceStrings.Format(nameof (BuildDefinitionIdInvalid), arg0);

    public static string BuildDefinitionIdInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildDefinitionIdInvalid), culture, arg0);

    public static string BuildRequestPropertyInvalid(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildRequestPropertyInvalid), arg0, arg1);

    public static string BuildRequestPropertyInvalid(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (BuildRequestPropertyInvalid), culture, arg0, arg1);

    public static string BuildControllerNotFound(object arg0) => ResourceStrings.Format(nameof (BuildControllerNotFound), arg0);

    public static string BuildControllerNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildControllerNotFound), culture, arg0);

    public static string BuildAgentNotFound(object arg0) => ResourceStrings.Format(nameof (BuildAgentNotFound), arg0);

    public static string BuildAgentNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildAgentNotFound), culture, arg0);

    public static string BuildServerNotFound(object arg0) => ResourceStrings.Format(nameof (BuildServerNotFound), arg0);

    public static string BuildServerNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildServerNotFound), culture, arg0);

    public static string BuildProcessTemplateNotFoundForWebApi(object arg0) => ResourceStrings.Format(nameof (BuildProcessTemplateNotFoundForWebApi), arg0);

    public static string BuildProcessTemplateNotFoundForWebApi(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildProcessTemplateNotFoundForWebApi), culture, arg0);

    public static string DefinitionNotFound(object arg0) => ResourceStrings.Format(nameof (DefinitionNotFound), arg0);

    public static string DefinitionNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (DefinitionNotFound), culture, arg0);

    public static string ProjectDoesNotExistWithName(object arg0) => ResourceStrings.Format(nameof (ProjectDoesNotExistWithName), arg0);

    public static string ProjectDoesNotExistWithName(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ProjectDoesNotExistWithName), culture, arg0);

    public static string BuildContainerDropDisabled() => ResourceStrings.Get(nameof (BuildContainerDropDisabled));

    public static string BuildContainerDropDisabled(CultureInfo culture) => ResourceStrings.Get(nameof (BuildContainerDropDisabled), culture);

    public static string BuildAccountDoesNotHavePermission(object arg0) => ResourceStrings.Format(nameof (BuildAccountDoesNotHavePermission), arg0);

    public static string BuildAccountDoesNotHavePermission(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildAccountDoesNotHavePermission), culture, arg0);

    public static string BuildAccountDoesNotHaveUsageLeft(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildAccountDoesNotHaveUsageLeft), arg0, arg1);

    public static string BuildAccountDoesNotHaveUsageLeft(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildAccountDoesNotHaveUsageLeft), culture, arg0, arg1);
    }

    public static string GitCustomTemplateNotSupported(object arg0, object arg1) => ResourceStrings.Format(nameof (GitCustomTemplateNotSupported), arg0, arg1);

    public static string GitCustomTemplateNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (GitCustomTemplateNotSupported), culture, arg0, arg1);
    }

    public static string ConflictingBuildQueryParameters() => ResourceStrings.Get(nameof (ConflictingBuildQueryParameters));

    public static string ConflictingBuildQueryParameters(CultureInfo culture) => ResourceStrings.Get(nameof (ConflictingBuildQueryParameters), culture);

    public static string IllFormattedId(object arg0) => ResourceStrings.Format(nameof (IllFormattedId), arg0);

    public static string IllFormattedId(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (IllFormattedId), culture, arg0);

    public static string GitContinuousDeploymentTemplate12Description() => ResourceStrings.Get(nameof (GitContinuousDeploymentTemplate12Description));

    public static string GitContinuousDeploymentTemplate12Description(CultureInfo culture) => ResourceStrings.Get(nameof (GitContinuousDeploymentTemplate12Description), culture);

    public static string NoValidRepoFound(object arg0) => ResourceStrings.Format(nameof (NoValidRepoFound), arg0);

    public static string NoValidRepoFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (NoValidRepoFound), culture, arg0);

    public static string SubscriptionIdNotAGuid(object arg0) => ResourceStrings.Format(nameof (SubscriptionIdNotAGuid), arg0);

    public static string SubscriptionIdNotAGuid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (SubscriptionIdNotAGuid), culture, arg0);

    public static string AzureConnectedServiceFriendlyName(object arg0) => ResourceStrings.Format(nameof (AzureConnectedServiceFriendlyName), arg0);

    public static string AzureConnectedServiceFriendlyName(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (AzureConnectedServiceFriendlyName), culture, arg0);

    public static string AzureConnectedServiceDescription() => ResourceStrings.Get(nameof (AzureConnectedServiceDescription));

    public static string AzureConnectedServiceDescription(CultureInfo culture) => ResourceStrings.Get(nameof (AzureConnectedServiceDescription), culture);

    public static string TfvcContinuousDeploymentTemplate12Description() => ResourceStrings.Get(nameof (TfvcContinuousDeploymentTemplate12Description));

    public static string TfvcContinuousDeploymentTemplate12Description(CultureInfo culture) => ResourceStrings.Get(nameof (TfvcContinuousDeploymentTemplate12Description), culture);

    public static string CannotDeleteRetainedBuild(object arg0) => ResourceStrings.Format(nameof (CannotDeleteRetainedBuild), arg0);

    public static string CannotDeleteRetainedBuild(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotDeleteRetainedBuild), culture, arg0);

    public static string OnlyHostedServiceOrWebsiteAllowed() => ResourceStrings.Get(nameof (OnlyHostedServiceOrWebsiteAllowed));

    public static string OnlyHostedServiceOrWebsiteAllowed(CultureInfo culture) => ResourceStrings.Get(nameof (OnlyHostedServiceOrWebsiteAllowed), culture);

    public static string GitUpgradeTemplateDescription() => ResourceStrings.Get(nameof (GitUpgradeTemplateDescription));

    public static string GitUpgradeTemplateDescription(CultureInfo culture) => ResourceStrings.Get(nameof (GitUpgradeTemplateDescription), culture);

    public static string BuildControllerDoesNotExist(object arg0) => ResourceStrings.Format(nameof (BuildControllerDoesNotExist), arg0);

    public static string BuildControllerDoesNotExist(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (BuildControllerDoesNotExist), culture, arg0);

    public static string ResourceUrlCreationFailed() => ResourceStrings.Get(nameof (ResourceUrlCreationFailed));

    public static string ResourceUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (ResourceUrlCreationFailed), culture);

    public static string BuildDeploymentAlreadyExists(object arg0, object arg1) => ResourceStrings.Format(nameof (BuildDeploymentAlreadyExists), arg0, arg1);

    public static string BuildDeploymentAlreadyExists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (BuildDeploymentAlreadyExists), culture, arg0, arg1);
    }

    public static string ServiceHostStartupTimedOut() => ResourceStrings.Get(nameof (ServiceHostStartupTimedOut));

    public static string ServiceHostStartupTimedOut(CultureInfo culture) => ResourceStrings.Get(nameof (ServiceHostStartupTimedOut), culture);

    public static string ConnectedServiceNotFound(object arg0) => ResourceStrings.Format(nameof (ConnectedServiceNotFound), arg0);

    public static string ConnectedServiceNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ConnectedServiceNotFound), culture, arg0);

    public static string CannotUseProjectRootAsCustomAssemblyPath(object arg0) => ResourceStrings.Format(nameof (CannotUseProjectRootAsCustomAssemblyPath), arg0);

    public static string CannotUseProjectRootAsCustomAssemblyPath(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotUseProjectRootAsCustomAssemblyPath), culture, arg0);

    public static string ScheduleJobInvalidArtifactUri(object arg0, object arg1) => ResourceStrings.Format(nameof (ScheduleJobInvalidArtifactUri), arg0, arg1);

    public static string ScheduleJobInvalidArtifactUri(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (ScheduleJobInvalidArtifactUri), culture, arg0, arg1);
    }

    public static string ScheduleJobInvalidDataNoProjectIdManyDefinitions(object arg0, object arg1) => ResourceStrings.Format(nameof (ScheduleJobInvalidDataNoProjectIdManyDefinitions), arg0, arg1);

    public static string ScheduleJobInvalidDataNoProjectIdManyDefinitions(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (ScheduleJobInvalidDataNoProjectIdManyDefinitions), culture, arg0, arg1);
    }

    public static string ScheduleJobInvalidDataNoProjectIdZeroDefinitions(object arg0) => ResourceStrings.Format(nameof (ScheduleJobInvalidDataNoProjectIdZeroDefinitions), arg0);

    public static string ScheduleJobInvalidDataNoProjectIdZeroDefinitions(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (ScheduleJobInvalidDataNoProjectIdZeroDefinitions), culture, arg0);
    }

    public static string ScheduleJobInvalidDefinitionUri(object arg0) => ResourceStrings.Format(nameof (ScheduleJobInvalidDefinitionUri), arg0);

    public static string ScheduleJobInvalidDefinitionUri(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ScheduleJobInvalidDefinitionUri), culture, arg0);

    public static string HostedXamlBeingRetired() => ResourceStrings.Get(nameof (HostedXamlBeingRetired));

    public static string HostedXamlBeingRetired(CultureInfo culture) => ResourceStrings.Get(nameof (HostedXamlBeingRetired), culture);

    public static string HostedXamlRetired() => ResourceStrings.Get(nameof (HostedXamlRetired));

    public static string HostedXamlRetired(CultureInfo culture) => ResourceStrings.Get(nameof (HostedXamlRetired), culture);

    public static string XamlBuildDisabledMessage() => ResourceStrings.Get(nameof (XamlBuildDisabledMessage));

    public static string XamlBuildDisabledMessage(CultureInfo culture) => ResourceStrings.Get(nameof (XamlBuildDisabledMessage), culture);
  }
}
