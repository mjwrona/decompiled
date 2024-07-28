// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Resources
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.Build2.WebServer.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.Build2.WebServer.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.Build2.WebServer.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(resourceName) : Microsoft.TeamFoundation.Build2.WebServer.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.Build2.WebServer.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Build2.WebServer.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.Build2.WebServer.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.Build2.WebServer.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Build2.WebServer.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.Build2.WebServer.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(resourceName, culture);
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

    public static string AmbigiousBuiltAfterFilter() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (AmbigiousBuiltAfterFilter));

    public static string AmbigiousBuiltAfterFilter(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (AmbigiousBuiltAfterFilter), culture);

    public static string ArtifactInvalidFileId(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (ArtifactInvalidFileId), arg0, arg1);

    public static string ArtifactInvalidFileId(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (ArtifactInvalidFileId), culture, arg0, arg1);

    public static string ArtifactNotFoundForBuild(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (ArtifactNotFoundForBuild), arg0, arg1);

    public static string ArtifactNotFoundForBuild(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (ArtifactNotFoundForBuild), culture, arg0, arg1);

    public static string ArtifactTypeNotSupported(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (ArtifactTypeNotSupported), arg0);

    public static string ArtifactTypeNotSupported(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (ArtifactTypeNotSupported), culture, arg0);

    public static string BodyMustBeEmptyForRebuild() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BodyMustBeEmptyForRebuild));

    public static string BodyMustBeEmptyForRebuild(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BodyMustBeEmptyForRebuild), culture);

    public static string BuildControllerMustNotBeSpecifiedForBuild() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildControllerMustNotBeSpecifiedForBuild));

    public static string BuildControllerMustNotBeSpecifiedForBuild(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildControllerMustNotBeSpecifiedForBuild), culture);

    public static string BuildControllerNotFound(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildControllerNotFound), arg0);

    public static string BuildControllerNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildControllerNotFound), culture, arg0);

    public static string BuildDurationTextCompleted(object arg0, object arg1, object arg2) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildDurationTextCompleted), arg0, arg1, arg2);

    public static string BuildDurationTextCompleted(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildDurationTextCompleted), culture, arg0, arg1, arg2);
    }

    public static string BuildDurationTextInProgress(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildDurationTextInProgress), arg0, arg1);

    public static string BuildDurationTextInProgress(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildDurationTextInProgress), culture, arg0, arg1);

    public static string BuildHasNoRepository(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildHasNoRepository), arg0);

    public static string BuildHasNoRepository(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildHasNoRepository), culture, arg0);

    public static string BuildLogNotFound(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildLogNotFound), arg0);

    public static string BuildLogNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildLogNotFound), culture, arg0);

    public static string BuildNotFound(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildNotFound), arg0);

    public static string BuildNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildNotFound), culture, arg0);

    public static string BuildQueryOrderOnlyFinishTimeIsSupported(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildQueryOrderOnlyFinishTimeIsSupported), arg0);

    public static string BuildQueryOrderOnlyFinishTimeIsSupported(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildQueryOrderOnlyFinishTimeIsSupported), culture, arg0);

    public static string BuildQueueMustNotBeSpecifiedForXamlBuild() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildQueueMustNotBeSpecifiedForXamlBuild));

    public static string BuildQueueMustNotBeSpecifiedForXamlBuild(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildQueueMustNotBeSpecifiedForXamlBuild), culture);

    public static string BuildRepositoryTypeNotSupported(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildRepositoryTypeNotSupported), arg0);

    public static string BuildRepositoryTypeNotSupported(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildRepositoryTypeNotSupported), culture, arg0);

    public static string BuildRequestValidationFailed() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildRequestValidationFailed));

    public static string BuildRequestValidationFailed(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildRequestValidationFailed), culture);

    public static string BuildResultTextCanceled() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextCanceled));

    public static string BuildResultTextCanceled(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextCanceled), culture);

    public static string BuildResultTextFailed() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextFailed));

    public static string BuildResultTextFailed(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextFailed), culture);

    public static string BuildResultTextPartiallySucceeded() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextPartiallySucceeded));

    public static string BuildResultTextPartiallySucceeded(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextPartiallySucceeded), culture);

    public static string BuildResultTextSucceeded() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextSucceeded));

    public static string BuildResultTextSucceeded(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextSucceeded), culture);

    public static string BuildResultTextUnknown() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextUnknown));

    public static string BuildResultTextUnknown(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildResultTextUnknown), culture);

    public static string BuildStatusTextCancelling() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildStatusTextCancelling));

    public static string BuildStatusTextCancelling(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildStatusTextCancelling), culture);

    public static string BuildStatusTextCompleted(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildStatusTextCompleted), arg0);

    public static string BuildStatusTextCompleted(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BuildStatusTextCompleted), culture, arg0);

    public static string BuildStatusTextInProgress() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildStatusTextInProgress));

    public static string BuildStatusTextInProgress(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildStatusTextInProgress), culture);

    public static string BuildStatusTextNotStarted() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildStatusTextNotStarted));

    public static string BuildStatusTextNotStarted(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildStatusTextNotStarted), culture);

    public static string BuildStatusTextPostPoned() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildStatusTextPostPoned));

    public static string BuildStatusTextPostPoned(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuildStatusTextPostPoned), culture);

    public static string BuiltAfterFilterNotSupportedForDefinitionIds() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuiltAfterFilterNotSupportedForDefinitionIds));

    public static string BuiltAfterFilterNotSupportedForDefinitionIds(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BuiltAfterFilterNotSupportedForDefinitionIds), culture);

    public static string ContinuationTokenNotSupportedForDefinitionIds() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (ContinuationTokenNotSupportedForDefinitionIds));

    public static string ContinuationTokenNotSupportedForDefinitionIds(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (ContinuationTokenNotSupportedForDefinitionIds), culture);

    public static string CouldNotRetrieveSourceVersionDisplayUrl(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (CouldNotRetrieveSourceVersionDisplayUrl), arg0, arg1);

    public static string CouldNotRetrieveSourceVersionDisplayUrl(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (CouldNotRetrieveSourceVersionDisplayUrl), culture, arg0, arg1);
    }

    public static string DefinitionContinuationTokenNoQueryOrder() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (DefinitionContinuationTokenNoQueryOrder));

    public static string DefinitionContinuationTokenNoQueryOrder(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (DefinitionContinuationTokenNoQueryOrder), culture);

    public static string DefinitionDisabled(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionDisabled), arg0, arg1);

    public static string DefinitionDisabled(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionDisabled), culture, arg0, arg1);

    public static string DefinitionMissingRepositoryInfo(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionMissingRepositoryInfo), arg0);

    public static string DefinitionMissingRepositoryInfo(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionMissingRepositoryInfo), culture, arg0);

    public static string DefinitionNotFound(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionNotFound), arg0);

    public static string DefinitionNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionNotFound), culture, arg0);

    public static string DefinitionTemplateNotFound(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionTemplateNotFound), arg0);

    public static string DefinitionTemplateNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionTemplateNotFound), culture, arg0);

    public static string DefinitionTemplateNotFound1(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionTemplateNotFound1), arg0);

    public static string DefinitionTemplateNotFound1(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionTemplateNotFound1), culture, arg0);

    public static string DefinitionTypeNotSupported(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionTypeNotSupported), arg0);

    public static string DefinitionTypeNotSupported(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionTypeNotSupported), culture, arg0);

    public static string InvalidBuildIdsQuery() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (InvalidBuildIdsQuery));

    public static string InvalidBuildIdsQuery(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (InvalidBuildIdsQuery), culture);

    public static string InvalidContinuationToken() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (InvalidContinuationToken));

    public static string InvalidContinuationToken(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (InvalidContinuationToken), culture);

    public static string InvalidInputForGetChangesAndWorkItemsBetweenBuilds() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (InvalidInputForGetChangesAndWorkItemsBetweenBuilds));

    public static string InvalidInputForGetChangesAndWorkItemsBetweenBuilds(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (InvalidInputForGetChangesAndWorkItemsBetweenBuilds), culture);

    public static string InvalidRequestContent() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (InvalidRequestContent));

    public static string InvalidRequestContent(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (InvalidRequestContent), culture);

    public static string InvalidStatusFilterFinishTimeCombination(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (InvalidStatusFilterFinishTimeCombination), arg0);

    public static string InvalidStatusFilterFinishTimeCombination(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (InvalidStatusFilterFinishTimeCombination), culture, arg0);

    public static string InvalidStatusQueryOrderFilterCombination(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (InvalidStatusQueryOrderFilterCombination), arg0, arg1);

    public static string InvalidStatusQueryOrderFilterCombination(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (InvalidStatusQueryOrderFilterCombination), culture, arg0, arg1);
    }

    public static string InvalidStatusResultFilterCombination(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (InvalidStatusResultFilterCombination), arg0, arg1);

    public static string InvalidStatusResultFilterCombination(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (InvalidStatusResultFilterCombination), culture, arg0, arg1);
    }

    public static string KeyVaultVariableGroupsNotSupported(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (KeyVaultVariableGroupsNotSupported), arg0);

    public static string KeyVaultVariableGroupsNotSupported(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (KeyVaultVariableGroupsNotSupported), culture, arg0);

    public static string MissingRequestContent() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (MissingRequestContent));

    public static string MissingRequestContent(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (MissingRequestContent), culture);

    public static string NotSupportedOnXamlBuildDefinition() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (NotSupportedOnXamlBuildDefinition));

    public static string NotSupportedOnXamlBuildDefinition(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (NotSupportedOnXamlBuildDefinition), culture);

    public static string ProjectIsNotWellFormed() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (ProjectIsNotWellFormed));

    public static string ProjectIsNotWellFormed(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (ProjectIsNotWellFormed), culture);

    public static string ProjectRequired() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (ProjectRequired));

    public static string ProjectRequired(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (ProjectRequired), culture);

    public static string QueueNotFound(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (QueueNotFound), arg0);

    public static string QueueNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (QueueNotFound), culture, arg0);

    public static string RebuildMustBePRBuild() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RebuildMustBePRBuild));

    public static string RebuildMustBePRBuild(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RebuildMustBePRBuild), culture);

    public static string ReportStreamNotSupported(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (ReportStreamNotSupported), arg0);

    public static string ReportStreamNotSupported(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (ReportStreamNotSupported), culture, arg0);

    public static string RepositoryInfoNotSupportedForDefinitionIds() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RepositoryInfoNotSupportedForDefinitionIds));

    public static string RepositoryInfoNotSupportedForDefinitionIds(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RepositoryInfoNotSupportedForDefinitionIds), culture);

    public static string RepositoryTypeMissing() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RepositoryTypeMissing));

    public static string RepositoryTypeMissing(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RepositoryTypeMissing), culture);

    public static string RequestPropertyInvalid(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (RequestPropertyInvalid), arg0, arg1);

    public static string RequestPropertyInvalid(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (RequestPropertyInvalid), culture, arg0, arg1);

    public static string RestApiInvalidLogLocation() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RestApiInvalidLogLocation));

    public static string RestApiInvalidLogLocation(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RestApiInvalidLogLocation), culture);

    public static string InvalidResourceTypeName(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (InvalidResourceTypeName), arg0);

    public static string InvalidResourceTypeName(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (InvalidResourceTypeName), culture, arg0);

    public static string TimelineNotFound(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (TimelineNotFound), arg0);

    public static string TimelineNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (TimelineNotFound), culture, arg0);

    public static string UseDeleteVerbToDeleteDefinition() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (UseDeleteVerbToDeleteDefinition));

    public static string UseDeleteVerbToDeleteDefinition(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (UseDeleteVerbToDeleteDefinition), culture);

    public static string UseDTQueueAPIs() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (UseDTQueueAPIs));

    public static string UseDTQueueAPIs(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (UseDTQueueAPIs), culture);

    public static string WrongIdSpecifiedForBuild(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongIdSpecifiedForBuild), arg0, arg1);

    public static string WrongIdSpecifiedForBuild(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongIdSpecifiedForBuild), culture, arg0, arg1);

    public static string WrongIdSpecifiedForDefinition(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongIdSpecifiedForDefinition), arg0, arg1);

    public static string WrongIdSpecifiedForDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongIdSpecifiedForDefinition), culture, arg0, arg1);
    }

    public static string WrongProjectSpecifiedForBuild(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongProjectSpecifiedForBuild), arg0, arg1);

    public static string WrongProjectSpecifiedForBuild(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongProjectSpecifiedForBuild), culture, arg0, arg1);
    }

    public static string WrongProjectSpecifiedForDefinition(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongProjectSpecifiedForDefinition), arg0, arg1);

    public static string WrongProjectSpecifiedForDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongProjectSpecifiedForDefinition), culture, arg0, arg1);
    }

    public static string WrongProjectSpecifiedForFolder(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongProjectSpecifiedForFolder), arg0, arg1);

    public static string WrongProjectSpecifiedForFolder(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (WrongProjectSpecifiedForFolder), culture, arg0, arg1);
    }

    public static string YamlFilenameRequiresYamlProcessType() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (YamlFilenameRequiresYamlProcessType));

    public static string YamlFilenameRequiresYamlProcessType(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (YamlFilenameRequiresYamlProcessType), culture);

    public static string YamlFilenameRequiresIncludeAllPropertiese() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (YamlFilenameRequiresIncludeAllPropertiese));

    public static string YamlFilenameRequiresIncludeAllPropertiese(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (YamlFilenameRequiresIncludeAllPropertiese), culture);

    public static string CompletedBuildNotFound(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (CompletedBuildNotFound), arg0);

    public static string CompletedBuildNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (CompletedBuildNotFound), culture, arg0);

    public static string RangeHeaderNotAllowed() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RangeHeaderNotAllowed));

    public static string RangeHeaderNotAllowed(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RangeHeaderNotAllowed), culture);

    public static string RetryNotSupported(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (RetryNotSupported), arg0, arg1);

    public static string RetryNotSupported(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (RetryNotSupported), culture, arg0, arg1);

    public static string BodyMustBeEmptyForRetry() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BodyMustBeEmptyForRetry));

    public static string BodyMustBeEmptyForRetry(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (BodyMustBeEmptyForRetry), culture);

    public static string BranchNotFound(object arg0, object arg1) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BranchNotFound), arg0, arg1);

    public static string BranchNotFound(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (BranchNotFound), culture, arg0, arg1);

    public static string DefinitionNotDesigner(object arg0) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionNotDesigner), arg0);

    public static string DefinitionNotDesigner(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Format(nameof (DefinitionNotDesigner), culture, arg0);

    public static string KeepForeverObsolete() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (KeepForeverObsolete));

    public static string KeepForeverObsolete(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (KeepForeverObsolete), culture);

    public static string RestApiToBeDeprecated() => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RestApiToBeDeprecated));

    public static string RestApiToBeDeprecated(CultureInfo culture) => Microsoft.TeamFoundation.Build2.WebServer.Resources.Get(nameof (RestApiToBeDeprecated), culture);
  }
}
