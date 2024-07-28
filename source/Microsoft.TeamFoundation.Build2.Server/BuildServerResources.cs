// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildServerResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class BuildServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (BuildServerResources), typeof (BuildServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => BuildServerResources.s_resMgr;

    private static string Get(string resourceName) => BuildServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? BuildServerResources.Get(resourceName) : BuildServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) BuildServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? BuildServerResources.GetInt(resourceName) : (int) BuildServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) BuildServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? BuildServerResources.GetBool(resourceName) : (bool) BuildServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => BuildServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = BuildServerResources.Get(resourceName, culture);
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

    public static string AccessDeniedForDefinition(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (AccessDeniedForDefinition), arg0, arg1, arg2, arg3);
    }

    public static string AccessDeniedForDefinition(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (AccessDeniedForDefinition), culture, arg0, arg1, arg2, arg3);
    }

    public static string AccessDeniedForPrivilege(object arg0, object arg1) => BuildServerResources.Format(nameof (AccessDeniedForPrivilege), arg0, arg1);

    public static string AccessDeniedForPrivilege(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (AccessDeniedForPrivilege), culture, arg0, arg1);

    public static string AccessDeniedForProject(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (AccessDeniedForProject), arg0, arg1, arg2);

    public static string AccessDeniedForProject(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (AccessDeniedForProject), culture, arg0, arg1, arg2);
    }

    public static string AgentsNotRegistered() => BuildServerResources.Get(nameof (AgentsNotRegistered));

    public static string AgentsNotRegistered(CultureInfo culture) => BuildServerResources.Get(nameof (AgentsNotRegistered), culture);

    public static string ArtifactExists(object arg0, object arg1) => BuildServerResources.Format(nameof (ArtifactExists), arg0, arg1);

    public static string ArtifactExists(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (ArtifactExists), culture, arg0, arg1);

    public static string BuildCompleted_Canceled(object arg0, object arg1) => BuildServerResources.Format(nameof (BuildCompleted_Canceled), arg0, arg1);

    public static string BuildCompleted_Canceled(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (BuildCompleted_Canceled), culture, arg0, arg1);

    public static string BuildCompleted_Failed(object arg0, object arg1) => BuildServerResources.Format(nameof (BuildCompleted_Failed), arg0, arg1);

    public static string BuildCompleted_Failed(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (BuildCompleted_Failed), culture, arg0, arg1);

    public static string BuildCompleted_PartiallySuccessful(object arg0, object arg1) => BuildServerResources.Format(nameof (BuildCompleted_PartiallySuccessful), arg0, arg1);

    public static string BuildCompleted_PartiallySuccessful(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (BuildCompleted_PartiallySuccessful), culture, arg0, arg1);
    }

    public static string BuildCompleted_Successful(object arg0, object arg1) => BuildServerResources.Format(nameof (BuildCompleted_Successful), arg0, arg1);

    public static string BuildCompleted_Successful(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (BuildCompleted_Successful), culture, arg0, arg1);

    public static string BuildCompleted_Unknown(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (BuildCompleted_Unknown), arg0, arg1, arg2);

    public static string BuildCompleted_Unknown(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (BuildCompleted_Unknown), culture, arg0, arg1, arg2);
    }

    public static string BuildCompletionEmailTitle(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (BuildCompletionEmailTitle), arg0, arg1, arg2);

    public static string BuildCompletionEmailTitle(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (BuildCompletionEmailTitle), culture, arg0, arg1, arg2);
    }

    public static string BuildExists(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (BuildExists), arg0, arg1, arg2);

    public static string BuildExists(object arg0, object arg1, object arg2, CultureInfo culture) => BuildServerResources.Format(nameof (BuildExists), culture, arg0, arg1, arg2);

    public static string BuildJobName() => BuildServerResources.Get(nameof (BuildJobName));

    public static string BuildJobName(CultureInfo culture) => BuildServerResources.Get(nameof (BuildJobName), culture);

    public static string BuildNotFound(object arg0) => BuildServerResources.Format(nameof (BuildNotFound), arg0);

    public static string BuildNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildNotFound), culture, arg0);

    public static string BuildOrchestrationExists(object arg0, object arg1) => BuildServerResources.Format(nameof (BuildOrchestrationExists), arg0, arg1);

    public static string BuildOrchestrationExists(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (BuildOrchestrationExists), culture, arg0, arg1);

    public static string BuildRepositoryTypeNotSupported(object arg0) => BuildServerResources.Format(nameof (BuildRepositoryTypeNotSupported), arg0);

    public static string BuildRepositoryTypeNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildRepositoryTypeNotSupported), culture, arg0);

    public static string BuildRequestValidationFailed() => BuildServerResources.Get(nameof (BuildRequestValidationFailed));

    public static string BuildRequestValidationFailed(CultureInfo culture) => BuildServerResources.Get(nameof (BuildRequestValidationFailed), culture);

    public static string BuildStatusInvalidChange(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (BuildStatusInvalidChange), arg0, arg1, arg2);

    public static string BuildStatusInvalidChange(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (BuildStatusInvalidChange), culture, arg0, arg1, arg2);
    }

    public static string CannotDeleteRetainedBuild(object arg0) => BuildServerResources.Format(nameof (CannotDeleteRetainedBuild), arg0);

    public static string CannotDeleteRetainedBuild(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (CannotDeleteRetainedBuild), culture, arg0);

    public static string ContainerNotFoundException(object arg0) => BuildServerResources.Format(nameof (ContainerNotFoundException), arg0);

    public static string ContainerNotFoundException(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ContainerNotFoundException), culture, arg0);

    public static string DefinitionBadgeCanceled() => BuildServerResources.Get(nameof (DefinitionBadgeCanceled));

    public static string DefinitionBadgeCanceled(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionBadgeCanceled), culture);

    public static string DefinitionBadgeFailed() => BuildServerResources.Get(nameof (DefinitionBadgeFailed));

    public static string DefinitionBadgeFailed(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionBadgeFailed), culture);

    public static string DefinitionBadgeLeftText() => BuildServerResources.Get(nameof (DefinitionBadgeLeftText));

    public static string DefinitionBadgeLeftText(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionBadgeLeftText), culture);

    public static string DefinitionBadgeNoBuilds() => BuildServerResources.Get(nameof (DefinitionBadgeNoBuilds));

    public static string DefinitionBadgeNoBuilds(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionBadgeNoBuilds), culture);

    public static string DefinitionBadgeNone() => BuildServerResources.Get(nameof (DefinitionBadgeNone));

    public static string DefinitionBadgeNone(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionBadgeNone), culture);

    public static string DefinitionBadgePartiallySucceeded() => BuildServerResources.Get(nameof (DefinitionBadgePartiallySucceeded));

    public static string DefinitionBadgePartiallySucceeded(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionBadgePartiallySucceeded), culture);

    public static string DefinitionBadgeSucceeded() => BuildServerResources.Get(nameof (DefinitionBadgeSucceeded));

    public static string DefinitionBadgeSucceeded(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionBadgeSucceeded), culture);

    public static string DefinitionDisabled(object arg0, object arg1) => BuildServerResources.Format(nameof (DefinitionDisabled), arg0, arg1);

    public static string DefinitionDisabled(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (DefinitionDisabled), culture, arg0, arg1);

    public static string DefinitionExists(object arg0, object arg1) => BuildServerResources.Format(nameof (DefinitionExists), arg0, arg1);

    public static string DefinitionExists(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (DefinitionExists), culture, arg0, arg1);

    public static string DefinitionNotFound(object arg0) => BuildServerResources.Format(nameof (DefinitionNotFound), arg0);

    public static string DefinitionNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DefinitionNotFound), culture, arg0);

    public static string DefinitionTemplateCannotBeModified(object arg0) => BuildServerResources.Format(nameof (DefinitionTemplateCannotBeModified), arg0);

    public static string DefinitionTemplateCannotBeModified(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DefinitionTemplateCannotBeModified), culture, arg0);

    public static string DemandsNotSatisfied(object arg0) => BuildServerResources.Format(nameof (DemandsNotSatisfied), arg0);

    public static string DemandsNotSatisfied(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DemandsNotSatisfied), culture, arg0);

    public static string DryRunYamlOverrideNotPermitted(object arg0, object arg1) => BuildServerResources.Format(nameof (DryRunYamlOverrideNotPermitted), arg0, arg1);

    public static string DryRunYamlOverrideNotPermitted(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (DryRunYamlOverrideNotPermitted), culture, arg0, arg1);
    }

    public static string InvalidYamlOverride() => BuildServerResources.Get(nameof (InvalidYamlOverride));

    public static string InvalidYamlOverride(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidYamlOverride), culture);

    public static string FailedToProcessCheckin(object arg0) => BuildServerResources.Format(nameof (FailedToProcessCheckin), arg0);

    public static string FailedToProcessCheckin(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (FailedToProcessCheckin), culture, arg0);

    public static string FailedToQueueABuild(object arg0, object arg1) => BuildServerResources.Format(nameof (FailedToQueueABuild), arg0, arg1);

    public static string FailedToQueueABuild(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (FailedToQueueABuild), culture, arg0, arg1);

    public static string InvalidBuildStep() => BuildServerResources.Get(nameof (InvalidBuildStep));

    public static string InvalidBuildStep(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidBuildStep), culture);

    public static string InvalidCommitId() => BuildServerResources.Get(nameof (InvalidCommitId));

    public static string InvalidCommitId(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidCommitId), culture);

    public static string NoQueueSpecified() => BuildServerResources.Get(nameof (NoQueueSpecified));

    public static string NoQueueSpecified(CultureInfo culture) => BuildServerResources.Get(nameof (NoQueueSpecified), culture);

    public static string NotSupportedOnXamlBuildDefinition() => BuildServerResources.Get(nameof (NotSupportedOnXamlBuildDefinition));

    public static string NotSupportedOnXamlBuildDefinition(CultureInfo culture) => BuildServerResources.Get(nameof (NotSupportedOnXamlBuildDefinition), culture);

    public static string QueueExists(object arg0) => BuildServerResources.Format(nameof (QueueExists), arg0);

    public static string QueueExists(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (QueueExists), culture, arg0);

    public static string QueueNotFound(object arg0) => BuildServerResources.Format(nameof (QueueNotFound), arg0);

    public static string QueueNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (QueueNotFound), culture, arg0);

    public static string DraftBuildNumberSuffix() => BuildServerResources.Get(nameof (DraftBuildNumberSuffix));

    public static string DraftBuildNumberSuffix(CultureInfo culture) => BuildServerResources.Get(nameof (DraftBuildNumberSuffix), culture);

    public static string ServiceIdentityName() => BuildServerResources.Get(nameof (ServiceIdentityName));

    public static string ServiceIdentityName(CultureInfo culture) => BuildServerResources.Get(nameof (ServiceIdentityName), culture);

    public static string UnexpectedError() => BuildServerResources.Get(nameof (UnexpectedError));

    public static string UnexpectedError(CultureInfo culture) => BuildServerResources.Get(nameof (UnexpectedError), culture);

    public static string WrongProjectSpecifiedForDefinition(object arg0, object arg1) => BuildServerResources.Format(nameof (WrongProjectSpecifiedForDefinition), arg0, arg1);

    public static string WrongProjectSpecifiedForDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (WrongProjectSpecifiedForDefinition), culture, arg0, arg1);
    }

    public static string ScheduleJobName() => BuildServerResources.Get(nameof (ScheduleJobName));

    public static string ScheduleJobName(CultureInfo culture) => BuildServerResources.Get(nameof (ScheduleJobName), culture);

    public static string WrongProjectSpecifiedForBuild(object arg0, object arg1) => BuildServerResources.Format(nameof (WrongProjectSpecifiedForBuild), arg0, arg1);

    public static string WrongProjectSpecifiedForBuild(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (WrongProjectSpecifiedForBuild), culture, arg0, arg1);
    }

    public static string ProjectRequired() => BuildServerResources.Get(nameof (ProjectRequired));

    public static string ProjectRequired(CultureInfo culture) => BuildServerResources.Get(nameof (ProjectRequired), culture);

    public static string BuildSpecifiedMultipleTimes(object arg0) => BuildServerResources.Format(nameof (BuildSpecifiedMultipleTimes), arg0);

    public static string BuildSpecifiedMultipleTimes(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildSpecifiedMultipleTimes), culture, arg0);

    public static string UpdateBuildsMultipleProjectsNotSupported() => BuildServerResources.Get(nameof (UpdateBuildsMultipleProjectsNotSupported));

    public static string UpdateBuildsMultipleProjectsNotSupported(CultureInfo culture) => BuildServerResources.Get(nameof (UpdateBuildsMultipleProjectsNotSupported), culture);

    public static string RequestPropertyInvalid(object arg0, object arg1) => BuildServerResources.Format(nameof (RequestPropertyInvalid), arg0, arg1);

    public static string RequestPropertyInvalid(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (RequestPropertyInvalid), culture, arg0, arg1);

    public static string RestApiInvalidLogLocation() => BuildServerResources.Get(nameof (RestApiInvalidLogLocation));

    public static string RestApiInvalidLogLocation(CultureInfo culture) => BuildServerResources.Get(nameof (RestApiInvalidLogLocation), culture);

    public static string BuildCanceledReason(object arg0) => BuildServerResources.Format(nameof (BuildCanceledReason), arg0);

    public static string BuildCanceledReason(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildCanceledReason), culture, arg0);

    public static string DaysToBuildInvalid() => BuildServerResources.Get(nameof (DaysToBuildInvalid));

    public static string DaysToBuildInvalid(CultureInfo culture) => BuildServerResources.Get(nameof (DaysToBuildInvalid), culture);

    public static string WrongIdSpecifiedForBuild(object arg0, object arg1) => BuildServerResources.Format(nameof (WrongIdSpecifiedForBuild), arg0, arg1);

    public static string WrongIdSpecifiedForBuild(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (WrongIdSpecifiedForBuild), culture, arg0, arg1);

    public static string ProjectBuildService(object arg0, object arg1) => BuildServerResources.Format(nameof (ProjectBuildService), arg0, arg1);

    public static string ProjectBuildService(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (ProjectBuildService), culture, arg0, arg1);

    public static string ProjectCollectionBuildService(object arg0) => BuildServerResources.Format(nameof (ProjectCollectionBuildService), arg0);

    public static string ProjectCollectionBuildService(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ProjectCollectionBuildService), culture, arg0);

    public static string BuildNumberFormatInvalidCharacters(object arg0) => BuildServerResources.Format(nameof (BuildNumberFormatInvalidCharacters), arg0);

    public static string BuildNumberFormatInvalidCharacters(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildNumberFormatInvalidCharacters), culture, arg0);

    public static string BuildNumberFormatInvalidOutput(object arg0, object arg1) => BuildServerResources.Format(nameof (BuildNumberFormatInvalidOutput), arg0, arg1);

    public static string BuildNumberFormatInvalidOutput(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (BuildNumberFormatInvalidOutput), culture, arg0, arg1);
    }

    public static string NoCapableAgentsOnline() => BuildServerResources.Get(nameof (NoCapableAgentsOnline));

    public static string NoCapableAgentsOnline(CultureInfo culture) => BuildServerResources.Get(nameof (NoCapableAgentsOnline), culture);

    public static string BuildCanceledBeforeStarted() => BuildServerResources.Get(nameof (BuildCanceledBeforeStarted));

    public static string BuildCanceledBeforeStarted(CultureInfo culture) => BuildServerResources.Get(nameof (BuildCanceledBeforeStarted), culture);

    public static string BuildInvalidState(object arg0) => BuildServerResources.Format(nameof (BuildInvalidState), arg0);

    public static string BuildInvalidState(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildInvalidState), culture, arg0);

    public static string InvalidMaxBuildsPerDefinition() => BuildServerResources.Get(nameof (InvalidMaxBuildsPerDefinition));

    public static string InvalidMaxBuildsPerDefinition(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidMaxBuildsPerDefinition), culture);

    public static string VariableNameIsReserved(object arg0) => BuildServerResources.Format(nameof (VariableNameIsReserved), arg0);

    public static string VariableNameIsReserved(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (VariableNameIsReserved), culture, arg0);

    public static string PollingJobName() => BuildServerResources.Get(nameof (PollingJobName));

    public static string PollingJobName(CultureInfo culture) => BuildServerResources.Get(nameof (PollingJobName), culture);

    public static string DuplicateDeleteBuildSpecs(object arg0) => BuildServerResources.Format(nameof (DuplicateDeleteBuildSpecs), arg0);

    public static string DuplicateDeleteBuildSpecs(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DuplicateDeleteBuildSpecs), culture, arg0);

    public static string ArtifactTypeNotSupported(object arg0) => BuildServerResources.Format(nameof (ArtifactTypeNotSupported), arg0);

    public static string ArtifactTypeNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ArtifactTypeNotSupported), culture, arg0);

    public static string ArtifactNotFoundForBuild(object arg0, object arg1) => BuildServerResources.Format(nameof (ArtifactNotFoundForBuild), arg0, arg1);

    public static string ArtifactNotFoundForBuild(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (ArtifactNotFoundForBuild), culture, arg0, arg1);

    public static string BuildControllerMustNotBeSpecifiedForBuild() => BuildServerResources.Get(nameof (BuildControllerMustNotBeSpecifiedForBuild));

    public static string BuildControllerMustNotBeSpecifiedForBuild(CultureInfo culture) => BuildServerResources.Get(nameof (BuildControllerMustNotBeSpecifiedForBuild), culture);

    public static string BuildControllerNotFound(object arg0) => BuildServerResources.Format(nameof (BuildControllerNotFound), arg0);

    public static string BuildControllerNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildControllerNotFound), culture, arg0);

    public static string BuildLogNotFound(object arg0) => BuildServerResources.Format(nameof (BuildLogNotFound), arg0);

    public static string BuildLogNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildLogNotFound), culture, arg0);

    public static string BuildQueueMustNotBeSpecifiedForXamlBuild() => BuildServerResources.Get(nameof (BuildQueueMustNotBeSpecifiedForXamlBuild));

    public static string BuildQueueMustNotBeSpecifiedForXamlBuild(CultureInfo culture) => BuildServerResources.Get(nameof (BuildQueueMustNotBeSpecifiedForXamlBuild), culture);

    public static string DefinitionTemplateNotFound(object arg0) => BuildServerResources.Format(nameof (DefinitionTemplateNotFound), arg0);

    public static string DefinitionTemplateNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DefinitionTemplateNotFound), culture, arg0);

    public static string DefinitionTypeNotSupported(object arg0) => BuildServerResources.Format(nameof (DefinitionTypeNotSupported), arg0);

    public static string DefinitionTypeNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DefinitionTypeNotSupported), culture, arg0);

    public static string TimelineNotFound(object arg0) => BuildServerResources.Format(nameof (TimelineNotFound), arg0);

    public static string TimelineNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (TimelineNotFound), culture, arg0);

    public static string WrongIdSpecifiedForDefinition(object arg0, object arg1) => BuildServerResources.Format(nameof (WrongIdSpecifiedForDefinition), arg0, arg1);

    public static string WrongIdSpecifiedForDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (WrongIdSpecifiedForDefinition), culture, arg0, arg1);
    }

    public static string PollingExceptionOccurred(object arg0) => BuildServerResources.Format(nameof (PollingExceptionOccurred), arg0);

    public static string PollingExceptionOccurred(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (PollingExceptionOccurred), culture, arg0);

    public static string DefinitionNotMatched(object arg0, object arg1) => BuildServerResources.Format(nameof (DefinitionNotMatched), arg0, arg1);

    public static string DefinitionNotMatched(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (DefinitionNotMatched), culture, arg0, arg1);

    public static string InvalidInputForGetChangesAndWorkItemsBetweenBuilds() => BuildServerResources.Get(nameof (InvalidInputForGetChangesAndWorkItemsBetweenBuilds));

    public static string InvalidInputForGetChangesAndWorkItemsBetweenBuilds(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidInputForGetChangesAndWorkItemsBetweenBuilds), culture);

    public static string SourceOrTargetBuildNotFound(object arg0, object arg1) => BuildServerResources.Format(nameof (SourceOrTargetBuildNotFound), arg0, arg1);

    public static string SourceOrTargetBuildNotFound(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (SourceOrTargetBuildNotFound), culture, arg0, arg1);

    public static string RepositoryBranchMissing() => BuildServerResources.Get(nameof (RepositoryBranchMissing));

    public static string RepositoryBranchMissing(CultureInfo culture) => BuildServerResources.Get(nameof (RepositoryBranchMissing), culture);

    public static string RepositoryTypeMissing() => BuildServerResources.Get(nameof (RepositoryTypeMissing));

    public static string RepositoryTypeMissing(CultureInfo culture) => BuildServerResources.Get(nameof (RepositoryTypeMissing), culture);

    public static string DefinitionBadgeNoDefinition() => BuildServerResources.Get(nameof (DefinitionBadgeNoDefinition));

    public static string DefinitionBadgeNoDefinition(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionBadgeNoDefinition), culture);

    public static string InvalidContinuationToken() => BuildServerResources.Get(nameof (InvalidContinuationToken));

    public static string InvalidContinuationToken(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidContinuationToken), culture);

    public static string InvalidRepositoryParameters() => BuildServerResources.Get(nameof (InvalidRepositoryParameters));

    public static string InvalidRepositoryParameters(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidRepositoryParameters), culture);

    public static string EmailTitleSuffixForTestResults(object arg0) => BuildServerResources.Format(nameof (EmailTitleSuffixForTestResults), arg0);

    public static string EmailTitleSuffixForTestResults(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (EmailTitleSuffixForTestResults), culture, arg0);

    public static string InvalidIntegerValue(object arg0) => BuildServerResources.Format(nameof (InvalidIntegerValue), arg0);

    public static string InvalidIntegerValue(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidIntegerValue), culture, arg0);

    public static string MultipleTriggersNotAllowed(object arg0) => BuildServerResources.Format(nameof (MultipleTriggersNotAllowed), arg0);

    public static string MultipleTriggersNotAllowed(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (MultipleTriggersNotAllowed), culture, arg0);

    public static string TriggerNotSupportedForRepository(object arg0, object arg1) => BuildServerResources.Format(nameof (TriggerNotSupportedForRepository), arg0, arg1);

    public static string TriggerNotSupportedForRepository(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (TriggerNotSupportedForRepository), culture, arg0, arg1);
    }

    public static string ReportFormatNotSupported(object arg0) => BuildServerResources.Format(nameof (ReportFormatNotSupported), arg0);

    public static string ReportFormatNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ReportFormatNotSupported), culture, arg0);

    public static string BuildDurationTextInProgress(object arg0, object arg1) => BuildServerResources.Format(nameof (BuildDurationTextInProgress), arg0, arg1);

    public static string BuildDurationTextInProgress(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (BuildDurationTextInProgress), culture, arg0, arg1);

    public static string BuildResultTextCanceled() => BuildServerResources.Get(nameof (BuildResultTextCanceled));

    public static string BuildResultTextCanceled(CultureInfo culture) => BuildServerResources.Get(nameof (BuildResultTextCanceled), culture);

    public static string BuildResultTextFailed() => BuildServerResources.Get(nameof (BuildResultTextFailed));

    public static string BuildResultTextFailed(CultureInfo culture) => BuildServerResources.Get(nameof (BuildResultTextFailed), culture);

    public static string BuildResultTextPartiallySucceeded() => BuildServerResources.Get(nameof (BuildResultTextPartiallySucceeded));

    public static string BuildResultTextPartiallySucceeded(CultureInfo culture) => BuildServerResources.Get(nameof (BuildResultTextPartiallySucceeded), culture);

    public static string BuildResultTextSucceeded() => BuildServerResources.Get(nameof (BuildResultTextSucceeded));

    public static string BuildResultTextSucceeded(CultureInfo culture) => BuildServerResources.Get(nameof (BuildResultTextSucceeded), culture);

    public static string BuildResultTextUnknown() => BuildServerResources.Get(nameof (BuildResultTextUnknown));

    public static string BuildResultTextUnknown(CultureInfo culture) => BuildServerResources.Get(nameof (BuildResultTextUnknown), culture);

    public static string BuildStatusTextCancelling() => BuildServerResources.Get(nameof (BuildStatusTextCancelling));

    public static string BuildStatusTextCancelling(CultureInfo culture) => BuildServerResources.Get(nameof (BuildStatusTextCancelling), culture);

    public static string BuildStatusTextCompleted(object arg0) => BuildServerResources.Format(nameof (BuildStatusTextCompleted), arg0);

    public static string BuildStatusTextCompleted(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildStatusTextCompleted), culture, arg0);

    public static string BuildStatusTextInProgress() => BuildServerResources.Get(nameof (BuildStatusTextInProgress));

    public static string BuildStatusTextInProgress(CultureInfo culture) => BuildServerResources.Get(nameof (BuildStatusTextInProgress), culture);

    public static string BuildStatusTextNotStarted() => BuildServerResources.Get(nameof (BuildStatusTextNotStarted));

    public static string BuildStatusTextNotStarted(CultureInfo culture) => BuildServerResources.Get(nameof (BuildStatusTextNotStarted), culture);

    public static string BuildStatusTextPostPoned() => BuildServerResources.Get(nameof (BuildStatusTextPostPoned));

    public static string BuildStatusTextPostPoned(CultureInfo culture) => BuildServerResources.Get(nameof (BuildStatusTextPostPoned), culture);

    public static string DurationDays(object arg0) => BuildServerResources.Format(nameof (DurationDays), arg0);

    public static string DurationDays(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DurationDays), culture, arg0);

    public static string DurationHours(object arg0) => BuildServerResources.Format(nameof (DurationHours), arg0);

    public static string DurationHours(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DurationHours), culture, arg0);

    public static string DurationMinutes(object arg0) => BuildServerResources.Format(nameof (DurationMinutes), arg0);

    public static string DurationMinutes(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DurationMinutes), culture, arg0);

    public static string DurationSeconds(object arg0) => BuildServerResources.Format(nameof (DurationSeconds), arg0);

    public static string DurationSeconds(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DurationSeconds), culture, arg0);

    public static string BuildDurationTextCompleted(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (BuildDurationTextCompleted), arg0, arg1, arg2);

    public static string BuildDurationTextCompleted(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (BuildDurationTextCompleted), culture, arg0, arg1, arg2);
    }

    public static string ReportStreamNotSupported(object arg0) => BuildServerResources.Format(nameof (ReportStreamNotSupported), arg0);

    public static string ReportStreamNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ReportStreamNotSupported), culture, arg0);

    public static string InvalidRequestContent() => BuildServerResources.Get(nameof (InvalidRequestContent));

    public static string InvalidRequestContent(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidRequestContent), culture);

    public static string MissingRequestContent() => BuildServerResources.Get(nameof (MissingRequestContent));

    public static string MissingRequestContent(CultureInfo culture) => BuildServerResources.Get(nameof (MissingRequestContent), culture);

    public static string DefinitionContinuationTokenNoQueryOrder() => BuildServerResources.Get(nameof (DefinitionContinuationTokenNoQueryOrder));

    public static string DefinitionContinuationTokenNoQueryOrder(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionContinuationTokenNoQueryOrder), culture);

    public static string DefinitionContinuationTokenNotSupportedForXaml() => BuildServerResources.Get(nameof (DefinitionContinuationTokenNotSupportedForXaml));

    public static string DefinitionContinuationTokenNotSupportedForXaml(CultureInfo culture) => BuildServerResources.Get(nameof (DefinitionContinuationTokenNotSupportedForXaml), culture);

    public static string DeleteDefinitionJobName() => BuildServerResources.Get(nameof (DeleteDefinitionJobName));

    public static string DeleteDefinitionJobName(CultureInfo culture) => BuildServerResources.Get(nameof (DeleteDefinitionJobName), culture);

    public static string MetricQueryDuplicateNames() => BuildServerResources.Get(nameof (MetricQueryDuplicateNames));

    public static string MetricQueryDuplicateNames(CultureInfo culture) => BuildServerResources.Get(nameof (MetricQueryDuplicateNames), culture);

    public static string OrchestrationTypeNotSupported(object arg0) => BuildServerResources.Format(nameof (OrchestrationTypeNotSupported), arg0);

    public static string OrchestrationTypeNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (OrchestrationTypeNotSupported), culture, arg0);

    public static string MissingTasksForDefinition(object arg0, object arg1) => BuildServerResources.Format(nameof (MissingTasksForDefinition), arg0, arg1);

    public static string MissingTasksForDefinition(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (MissingTasksForDefinition), culture, arg0, arg1);

    public static string NoCapableAgentsExist(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (NoCapableAgentsExist), arg0, arg1, arg2);

    public static string NoCapableAgentsExist(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (NoCapableAgentsExist), culture, arg0, arg1, arg2);
    }

    public static string CleanupJobName() => BuildServerResources.Get(nameof (CleanupJobName));

    public static string CleanupJobName(CultureInfo culture) => BuildServerResources.Get(nameof (CleanupJobName), culture);

    public static string InvalidStatusFilterFinishTimeCombination(object arg0) => BuildServerResources.Format(nameof (InvalidStatusFilterFinishTimeCombination), arg0);

    public static string InvalidStatusFilterFinishTimeCombination(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidStatusFilterFinishTimeCombination), culture, arg0);

    public static string ContinuationTokenNotSupportedForDefinitionIds() => BuildServerResources.Get(nameof (ContinuationTokenNotSupportedForDefinitionIds));

    public static string ContinuationTokenNotSupportedForDefinitionIds(CultureInfo culture) => BuildServerResources.Get(nameof (ContinuationTokenNotSupportedForDefinitionIds), culture);

    public static string RepositoryInfoNotSupportedForDefinitionIds() => BuildServerResources.Get(nameof (RepositoryInfoNotSupportedForDefinitionIds));

    public static string RepositoryInfoNotSupportedForDefinitionIds(CultureInfo culture) => BuildServerResources.Get(nameof (RepositoryInfoNotSupportedForDefinitionIds), culture);

    public static string InvalidStatusResultFilterCombination(object arg0, object arg1) => BuildServerResources.Format(nameof (InvalidStatusResultFilterCombination), arg0, arg1);

    public static string InvalidStatusResultFilterCombination(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (InvalidStatusResultFilterCombination), culture, arg0, arg1);
    }

    public static string RetentionPolicyFinishTimeDeletedReason(object arg0) => BuildServerResources.Format(nameof (RetentionPolicyFinishTimeDeletedReason), arg0);

    public static string RetentionPolicyFinishTimeDeletedReason(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (RetentionPolicyFinishTimeDeletedReason), culture, arg0);

    public static string DeleteTeamProjectsBuildDeletedReason(object arg0) => BuildServerResources.Format(nameof (DeleteTeamProjectsBuildDeletedReason), arg0);

    public static string DeleteTeamProjectsBuildDeletedReason(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DeleteTeamProjectsBuildDeletedReason), culture, arg0);

    public static string BuildDeletedReasonManual() => BuildServerResources.Get(nameof (BuildDeletedReasonManual));

    public static string BuildDeletedReasonManual(CultureInfo culture) => BuildServerResources.Get(nameof (BuildDeletedReasonManual), culture);

    public static string FolderExists(object arg0, object arg1) => BuildServerResources.Format(nameof (FolderExists), arg0, arg1);

    public static string FolderExists(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (FolderExists), culture, arg0, arg1);

    public static string FolderNotFound(object arg0) => BuildServerResources.Format(nameof (FolderNotFound), arg0);

    public static string FolderNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (FolderNotFound), culture, arg0);

    public static string WrongIdSpecifiedForDefinitionFolder(object arg0, object arg1) => BuildServerResources.Format(nameof (WrongIdSpecifiedForDefinitionFolder), arg0, arg1);

    public static string WrongIdSpecifiedForDefinitionFolder(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (WrongIdSpecifiedForDefinitionFolder), culture, arg0, arg1);
    }

    public static string BuildReasonNotSupported(object arg0) => BuildServerResources.Format(nameof (BuildReasonNotSupported), arg0);

    public static string BuildReasonNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildReasonNotSupported), culture, arg0);

    public static string InvalidFolder(object arg0) => BuildServerResources.Format(nameof (InvalidFolder), arg0);

    public static string InvalidFolder(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidFolder), culture, arg0);

    public static string FolderPathNotSupported(object arg0) => BuildServerResources.Format(nameof (FolderPathNotSupported), arg0);

    public static string FolderPathNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (FolderPathNotSupported), culture, arg0);

    public static string WrongProjectSpecifiedForFolder(object arg0, object arg1) => BuildServerResources.Format(nameof (WrongProjectSpecifiedForFolder), arg0, arg1);

    public static string WrongProjectSpecifiedForFolder(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (WrongProjectSpecifiedForFolder), culture, arg0, arg1);
    }

    public static string EndpointsAccessDenied(object arg0) => BuildServerResources.Format(nameof (EndpointsAccessDenied), arg0);

    public static string EndpointsAccessDenied(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (EndpointsAccessDenied), culture, arg0);

    public static string AmbigiousBuiltAfterFilter() => BuildServerResources.Get(nameof (AmbigiousBuiltAfterFilter));

    public static string AmbigiousBuiltAfterFilter(CultureInfo culture) => BuildServerResources.Get(nameof (AmbigiousBuiltAfterFilter), culture);

    public static string BuiltAfterFilterNotSupportedForDefinitionIds() => BuildServerResources.Get(nameof (BuiltAfterFilterNotSupportedForDefinitionIds));

    public static string BuiltAfterFilterNotSupportedForDefinitionIds(CultureInfo culture) => BuildServerResources.Get(nameof (BuiltAfterFilterNotSupportedForDefinitionIds), culture);

    public static string MetaTaskDefinitionsNotFound(object arg0) => BuildServerResources.Format(nameof (MetaTaskDefinitionsNotFound), arg0);

    public static string MetaTaskDefinitionsNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (MetaTaskDefinitionsNotFound), culture, arg0);

    public static string InvalidBuildIdsQuery() => BuildServerResources.Get(nameof (InvalidBuildIdsQuery));

    public static string InvalidBuildIdsQuery(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidBuildIdsQuery), culture);

    public static string Canceled() => BuildServerResources.Get(nameof (Canceled));

    public static string Canceled(CultureInfo culture) => BuildServerResources.Get(nameof (Canceled), culture);

    public static string Failed() => BuildServerResources.Get(nameof (Failed));

    public static string Failed(CultureInfo culture) => BuildServerResources.Get(nameof (Failed), culture);

    public static string None() => BuildServerResources.Get(nameof (None));

    public static string None(CultureInfo culture) => BuildServerResources.Get(nameof (None), culture);

    public static string PartiallySucceeded() => BuildServerResources.Get(nameof (PartiallySucceeded));

    public static string PartiallySucceeded(CultureInfo culture) => BuildServerResources.Get(nameof (PartiallySucceeded), culture);

    public static string Succeeded() => BuildServerResources.Get(nameof (Succeeded));

    public static string Succeeded(CultureInfo culture) => BuildServerResources.Get(nameof (Succeeded), culture);

    public static string UseDTQueueAPIs() => BuildServerResources.Get(nameof (UseDTQueueAPIs));

    public static string UseDTQueueAPIs(CultureInfo culture) => BuildServerResources.Get(nameof (UseDTQueueAPIs), culture);

    public static string AccessDeniedForFolder(object arg0, object arg1, object arg2, object arg3) => BuildServerResources.Format(nameof (AccessDeniedForFolder), arg0, arg1, arg2, arg3);

    public static string AccessDeniedForFolder(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (AccessDeniedForFolder), culture, arg0, arg1, arg2, arg3);
    }

    public static string BuildHasNoRepository(object arg0) => BuildServerResources.Format(nameof (BuildHasNoRepository), arg0);

    public static string BuildHasNoRepository(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildHasNoRepository), culture, arg0);

    public static string CouldNotRetrieveSourceVersionDisplayUrl(object arg0, object arg1) => BuildServerResources.Format(nameof (CouldNotRetrieveSourceVersionDisplayUrl), arg0, arg1);

    public static string CouldNotRetrieveSourceVersionDisplayUrl(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (CouldNotRetrieveSourceVersionDisplayUrl), culture, arg0, arg1);
    }

    public static string MetricAggregationTypeNotSupported(object arg0) => BuildServerResources.Format(nameof (MetricAggregationTypeNotSupported), arg0);

    public static string MetricAggregationTypeNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (MetricAggregationTypeNotSupported), culture, arg0);

    public static string DuplicateBuildOption(object arg0) => BuildServerResources.Format(nameof (DuplicateBuildOption), arg0);

    public static string DuplicateBuildOption(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DuplicateBuildOption), culture, arg0);

    public static string ProjectIsNotWellFormed() => BuildServerResources.Get(nameof (ProjectIsNotWellFormed));

    public static string ProjectIsNotWellFormed(CultureInfo culture) => BuildServerResources.Get(nameof (ProjectIsNotWellFormed), culture);

    public static string BuildDefinitionTemplateSerializationError(object arg0) => BuildServerResources.Format(nameof (BuildDefinitionTemplateSerializationError), arg0);

    public static string BuildDefinitionTemplateSerializationError(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (BuildDefinitionTemplateSerializationError), culture, arg0);

    public static string ContributionDoesntTargetTemplate(object arg0, object arg1) => BuildServerResources.Format(nameof (ContributionDoesntTargetTemplate), arg0, arg1);

    public static string ContributionDoesntTargetTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (ContributionDoesntTargetTemplate), culture, arg0, arg1);
    }

    public static string AmbiguousTaskName(object arg0) => BuildServerResources.Format(nameof (AmbiguousTaskName), arg0);

    public static string AmbiguousTaskName(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (AmbiguousTaskName), culture, arg0);

    public static string ErrorResolvingVersionFromBranch(object arg0, object arg1) => BuildServerResources.Format(nameof (ErrorResolvingVersionFromBranch), arg0, arg1);

    public static string ErrorResolvingVersionFromBranch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (ErrorResolvingVersionFromBranch), culture, arg0, arg1);
    }

    public static string FileExceedsMaxLength(object arg0, object arg1) => BuildServerResources.Format(nameof (FileExceedsMaxLength), arg0, arg1);

    public static string FileExceedsMaxLength(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (FileExceedsMaxLength), culture, arg0, arg1);

    public static string FileNotFoundInRepo(object arg0, object arg1, object arg2, object arg3) => BuildServerResources.Format(nameof (FileNotFoundInRepo), arg0, arg1, arg2, arg3);

    public static string FileNotFoundInRepo(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (FileNotFoundInRepo), culture, arg0, arg1, arg2, arg3);
    }

    public static string GitHubUpdateFileContentError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (GitHubUpdateFileContentError), arg0, arg1, arg2, arg3);
    }

    public static string GitHubUpdateFileContentError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubUpdateFileContentError), culture, arg0, arg1, arg2, arg3);
    }

    public static string GitHubFileContentError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return BuildServerResources.Format(nameof (GitHubFileContentError), arg0, arg1, arg2, arg3, arg4);
    }

    public static string GitHubFileContentError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubFileContentError), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string GitHubFileContentErrorAtCommitOrBranch(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (GitHubFileContentErrorAtCommitOrBranch), arg0, arg1, arg2, arg3);
    }

    public static string GitHubFileContentErrorAtCommitOrBranch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubFileContentErrorAtCommitOrBranch), culture, arg0, arg1, arg2, arg3);
    }

    public static string InvalidVersionFormat(object arg0, object arg1) => BuildServerResources.Format(nameof (InvalidVersionFormat), arg0, arg1);

    public static string InvalidVersionFormat(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidVersionFormat), culture, arg0, arg1);

    public static string TaskNotFound(object arg0) => BuildServerResources.Format(nameof (TaskNotFound), arg0);

    public static string TaskNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (TaskNotFound), culture, arg0);

    public static string UnableToDetermineRepoAccessToken(object arg0) => BuildServerResources.Format(nameof (UnableToDetermineRepoAccessToken), arg0);

    public static string UnableToDetermineRepoAccessToken(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (UnableToDetermineRepoAccessToken), culture, arg0);

    public static string UnableToDetermineRepository() => BuildServerResources.Get(nameof (UnableToDetermineRepository));

    public static string UnableToDetermineRepository(CultureInfo culture) => BuildServerResources.Get(nameof (UnableToDetermineRepository), culture);

    public static string YamlLoadError(object arg0) => BuildServerResources.Format(nameof (YamlLoadError), arg0);

    public static string YamlLoadError(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (YamlLoadError), culture, arg0);

    public static string YamlRequiresOneJob(object arg0) => BuildServerResources.Format(nameof (YamlRequiresOneJob), arg0);

    public static string YamlRequiresOneJob(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (YamlRequiresOneJob), culture, arg0);

    public static string YamlRequiresOnePhase() => BuildServerResources.Get(nameof (YamlRequiresOnePhase));

    public static string YamlRequiresOnePhase(CultureInfo culture) => BuildServerResources.Get(nameof (YamlRequiresOnePhase), culture);

    public static string VariableGroupsAccessDenied(object arg0) => BuildServerResources.Format(nameof (VariableGroupsAccessDenied), arg0);

    public static string VariableGroupsAccessDenied(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (VariableGroupsAccessDenied), culture, arg0);

    public static string BuildDefinitionInputInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (BuildDefinitionInputInvalid), arg0, arg1, arg2, arg3);
    }

    public static string BuildDefinitionInputInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (BuildDefinitionInputInvalid), culture, arg0, arg1, arg2, arg3);
    }

    public static string InvalidDraftDefinition() => BuildServerResources.Get(nameof (InvalidDraftDefinition));

    public static string InvalidDraftDefinition(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidDraftDefinition), culture);

    public static string InvalidDefinitionProperty(object arg0) => BuildServerResources.Format(nameof (InvalidDefinitionProperty), arg0);

    public static string InvalidDefinitionProperty(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidDefinitionProperty), culture, arg0);

    public static string MultipliedBuildJobFormat(object arg0, object arg1) => BuildServerResources.Format(nameof (MultipliedBuildJobFormat), arg0, arg1);

    public static string MultipliedBuildJobFormat(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (MultipliedBuildJobFormat), culture, arg0, arg1);

    public static string MultipliedBuildJobValueSeparator() => BuildServerResources.Get(nameof (MultipliedBuildJobValueSeparator));

    public static string MultipliedBuildJobValueSeparator(CultureInfo culture) => BuildServerResources.Get(nameof (MultipliedBuildJobValueSeparator), culture);

    public static string CouldNotRecreateSubscription(object arg0) => BuildServerResources.Format(nameof (CouldNotRecreateSubscription), arg0);

    public static string CouldNotRecreateSubscription(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (CouldNotRecreateSubscription), culture, arg0);

    public static string ErrorWhileCreatingSubscription(object arg0) => BuildServerResources.Format(nameof (ErrorWhileCreatingSubscription), arg0);

    public static string ErrorWhileCreatingSubscription(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ErrorWhileCreatingSubscription), culture, arg0);

    public static string ErrorWhileDeletingSubscription(object arg0) => BuildServerResources.Format(nameof (ErrorWhileDeletingSubscription), arg0);

    public static string ErrorWhileDeletingSubscription(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ErrorWhileDeletingSubscription), culture, arg0);

    public static string ErrorWhileFindingSubscription(object arg0) => BuildServerResources.Format(nameof (ErrorWhileFindingSubscription), arg0);

    public static string ErrorWhileFindingSubscription(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ErrorWhileFindingSubscription), culture, arg0);

    public static string RepositoryDoesNotProvideApiUrl() => BuildServerResources.Get(nameof (RepositoryDoesNotProvideApiUrl));

    public static string RepositoryDoesNotProvideApiUrl(CultureInfo culture) => BuildServerResources.Get(nameof (RepositoryDoesNotProvideApiUrl), culture);

    public static string RepositoryDoesNotReferenceServiceEndpoint() => BuildServerResources.Get(nameof (RepositoryDoesNotReferenceServiceEndpoint));

    public static string RepositoryDoesNotReferenceServiceEndpoint(CultureInfo culture) => BuildServerResources.Get(nameof (RepositoryDoesNotReferenceServiceEndpoint), culture);

    public static string PipelineForkBuildBlocked() => BuildServerResources.Get(nameof (PipelineForkBuildBlocked));

    public static string PipelineForkBuildBlocked(CultureInfo culture) => BuildServerResources.Get(nameof (PipelineForkBuildBlocked), culture);

    public static string SecureFilesAccessDenied(object arg0) => BuildServerResources.Format(nameof (SecureFilesAccessDenied), arg0);

    public static string SecureFilesAccessDenied(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (SecureFilesAccessDenied), culture, arg0);

    public static string RepositoryDoesNotExist(object arg0) => BuildServerResources.Format(nameof (RepositoryDoesNotExist), arg0);

    public static string RepositoryDoesNotExist(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (RepositoryDoesNotExist), culture, arg0);

    public static string FailedToDeserializeBuildParameters() => BuildServerResources.Get(nameof (FailedToDeserializeBuildParameters));

    public static string FailedToDeserializeBuildParameters(CultureInfo culture) => BuildServerResources.Get(nameof (FailedToDeserializeBuildParameters), culture);

    public static string CycleDetectedInProvidedBuildCompletionTriggers() => BuildServerResources.Get(nameof (CycleDetectedInProvidedBuildCompletionTriggers));

    public static string CycleDetectedInProvidedBuildCompletionTriggers(CultureInfo culture) => BuildServerResources.Get(nameof (CycleDetectedInProvidedBuildCompletionTriggers), culture);

    public static string DefinitionTriggerAlreadyExistsException(object arg0) => BuildServerResources.Format(nameof (DefinitionTriggerAlreadyExistsException), arg0);

    public static string DefinitionTriggerAlreadyExistsException(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DefinitionTriggerAlreadyExistsException), culture, arg0);

    public static string ForkBuildBlockedDueToYamlProcess() => BuildServerResources.Get(nameof (ForkBuildBlockedDueToYamlProcess));

    public static string ForkBuildBlockedDueToYamlProcess(CultureInfo culture) => BuildServerResources.Get(nameof (ForkBuildBlockedDueToYamlProcess), culture);

    public static string InvalidAuthScopeErrorMessage() => BuildServerResources.Get(nameof (InvalidAuthScopeErrorMessage));

    public static string InvalidAuthScopeErrorMessage(CultureInfo culture) => BuildServerResources.Get(nameof (InvalidAuthScopeErrorMessage), culture);

    public static string UnsupportedBuildCompletionTriggerChain(object arg0) => BuildServerResources.Format(nameof (UnsupportedBuildCompletionTriggerChain), arg0);

    public static string UnsupportedBuildCompletionTriggerChain(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (UnsupportedBuildCompletionTriggerChain), culture, arg0);

    public static string InvalidDefinitionTriggerSource(object arg0) => BuildServerResources.Format(nameof (InvalidDefinitionTriggerSource), arg0);

    public static string InvalidDefinitionTriggerSource(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidDefinitionTriggerSource), culture, arg0);

    public static string MultipleBuildStagesNotAllowedInYaml() => BuildServerResources.Get(nameof (MultipleBuildStagesNotAllowedInYaml));

    public static string MultipleBuildStagesNotAllowedInYaml(CultureInfo culture) => BuildServerResources.Get(nameof (MultipleBuildStagesNotAllowedInYaml), culture);

    public static string YamlRequiresOneStage() => BuildServerResources.Get(nameof (YamlRequiresOneStage));

    public static string YamlRequiresOneStage(CultureInfo culture) => BuildServerResources.Get(nameof (YamlRequiresOneStage), culture);

    public static string DirectoryNotFoundInRepoAtCommitOrBranch(
      object arg0,
      object arg1,
      object arg2)
    {
      return BuildServerResources.Format(nameof (DirectoryNotFoundInRepoAtCommitOrBranch), arg0, arg1, arg2);
    }

    public static string DirectoryNotFoundInRepoAtCommitOrBranch(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (DirectoryNotFoundInRepoAtCommitOrBranch), culture, arg0, arg1, arg2);
    }

    public static string GitHubPathContentErrorAtCommitOrBranch(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (GitHubPathContentErrorAtCommitOrBranch), arg0, arg1, arg2, arg3);
    }

    public static string GitHubPathContentErrorAtCommitOrBranch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubPathContentErrorAtCommitOrBranch), culture, arg0, arg1, arg2, arg3);
    }

    public static string CannotDeleteDefinitionWithRetainedBuilds() => BuildServerResources.Get(nameof (CannotDeleteDefinitionWithRetainedBuilds));

    public static string CannotDeleteDefinitionWithRetainedBuilds(CultureInfo culture) => BuildServerResources.Get(nameof (CannotDeleteDefinitionWithRetainedBuilds), culture);

    public static string GitHubUnknownRepository(object arg0, object arg1) => BuildServerResources.Format(nameof (GitHubUnknownRepository), arg0, arg1);

    public static string GitHubUnknownRepository(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (GitHubUnknownRepository), culture, arg0, arg1);

    public static string GitHubRepositoryNoCommitFound(object arg0, object arg1) => BuildServerResources.Format(nameof (GitHubRepositoryNoCommitFound), arg0, arg1);

    public static string GitHubRepositoryNoCommitFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubRepositoryNoCommitFound), culture, arg0, arg1);
    }

    public static string GitHubRepositoryNoTreeFound(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (GitHubRepositoryNoTreeFound), arg0, arg1, arg2);

    public static string GitHubRepositoryNoTreeFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubRepositoryNoTreeFound), culture, arg0, arg1, arg2);
    }

    public static string InvalidSettingsSourceType(object arg0) => BuildServerResources.Format(nameof (InvalidSettingsSourceType), arg0);

    public static string InvalidSettingsSourceType(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidSettingsSourceType), culture, arg0);

    public static string InvalidTriggerSettingsSourceType(object arg0, object arg1) => BuildServerResources.Format(nameof (InvalidTriggerSettingsSourceType), arg0, arg1);

    public static string InvalidTriggerSettingsSourceType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (InvalidTriggerSettingsSourceType), culture, arg0, arg1);
    }

    public static string GatedCheckinPhaseDisplayName() => BuildServerResources.Get(nameof (GatedCheckinPhaseDisplayName));

    public static string GatedCheckinPhaseDisplayName(CultureInfo culture) => BuildServerResources.Get(nameof (GatedCheckinPhaseDisplayName), culture);

    public static string GatedCheckinRequiresSinglePhase(object arg0) => BuildServerResources.Format(nameof (GatedCheckinRequiresSinglePhase), arg0);

    public static string GatedCheckinRequiresSinglePhase(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (GatedCheckinRequiresSinglePhase), culture, arg0);

    public static string GatedCheckinRequiresAgentPhase(object arg0) => BuildServerResources.Format(nameof (GatedCheckinRequiresAgentPhase), arg0);

    public static string GatedCheckinRequiresAgentPhase(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (GatedCheckinRequiresAgentPhase), culture, arg0);

    public static string MultiAgentNotSupportedForGatedCheckin(object arg0) => BuildServerResources.Format(nameof (MultiAgentNotSupportedForGatedCheckin), arg0);

    public static string MultiAgentNotSupportedForGatedCheckin(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (MultiAgentNotSupportedForGatedCheckin), culture, arg0);

    public static string YamlRequiresBranchSpecification() => BuildServerResources.Get(nameof (YamlRequiresBranchSpecification));

    public static string YamlRequiresBranchSpecification(CultureInfo culture) => BuildServerResources.Get(nameof (YamlRequiresBranchSpecification), culture);

    public static string CannotUpdateTriggeredByBuildException() => BuildServerResources.Get(nameof (CannotUpdateTriggeredByBuildException));

    public static string CannotUpdateTriggeredByBuildException(CultureInfo culture) => BuildServerResources.Get(nameof (CannotUpdateTriggeredByBuildException), culture);

    public static string CannotRestoreDeletedDraftWithoutRestoringParent() => BuildServerResources.Get(nameof (CannotRestoreDeletedDraftWithoutRestoringParent));

    public static string CannotRestoreDeletedDraftWithoutRestoringParent(CultureInfo culture) => BuildServerResources.Get(nameof (CannotRestoreDeletedDraftWithoutRestoringParent), culture);

    public static string UnableToResolveVersionFromRef(object arg0) => BuildServerResources.Format(nameof (UnableToResolveVersionFromRef), arg0);

    public static string UnableToResolveVersionFromRef(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (UnableToResolveVersionFromRef), culture, arg0);

    public static string DropArtifactRootIdNotFound() => BuildServerResources.Get(nameof (DropArtifactRootIdNotFound));

    public static string DropArtifactRootIdNotFound(CultureInfo culture) => BuildServerResources.Get(nameof (DropArtifactRootIdNotFound), culture);

    public static string DropArtifactManifestNotFound(object arg0) => BuildServerResources.Format(nameof (DropArtifactManifestNotFound), arg0);

    public static string DropArtifactManifestNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (DropArtifactManifestNotFound), culture, arg0);

    public static string BuildEventNotFound(object arg0, object arg1) => BuildServerResources.Format(nameof (BuildEventNotFound), arg0, arg1);

    public static string BuildEventNotFound(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (BuildEventNotFound), culture, arg0, arg1);

    public static string InvalidBuildEventStatusUpdate(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (InvalidBuildEventStatusUpdate), arg0, arg1, arg2);

    public static string InvalidBuildEventStatusUpdate(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (InvalidBuildEventStatusUpdate), culture, arg0, arg1, arg2);
    }

    public static string BuildEventPermissionError() => BuildServerResources.Get(nameof (BuildEventPermissionError));

    public static string BuildEventPermissionError(CultureInfo culture) => BuildServerResources.Get(nameof (BuildEventPermissionError), culture);

    public static string GitHubUnknownPullRequestId(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (GitHubUnknownPullRequestId), arg0, arg1, arg2);

    public static string GitHubUnknownPullRequestId(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubUnknownPullRequestId), culture, arg0, arg1, arg2);
    }

    public static string AmbiguousDefinitionName(object arg0) => BuildServerResources.Format(nameof (AmbiguousDefinitionName), arg0);

    public static string AmbiguousDefinitionName(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (AmbiguousDefinitionName), culture, arg0);

    public static string AbusiveUserRequestError() => BuildServerResources.Get(nameof (AbusiveUserRequestError));

    public static string AbusiveUserRequestError(CultureInfo culture) => BuildServerResources.Get(nameof (AbusiveUserRequestError), culture);

    public static string GitHubCreateFileAtBranchError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (GitHubCreateFileAtBranchError), arg0, arg1, arg2, arg3);
    }

    public static string GitHubCreateFileAtBranchError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubCreateFileAtBranchError), culture, arg0, arg1, arg2, arg3);
    }

    public static string GitHubCreateBranchError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (GitHubCreateBranchError), arg0, arg1, arg2, arg3);
    }

    public static string GitHubCreateBranchError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubCreateBranchError), culture, arg0, arg1, arg2, arg3);
    }

    public static string GitHubCreatePullRequestError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (GitHubCreatePullRequestError), arg0, arg1, arg2, arg3);
    }

    public static string GitHubCreatePullRequestError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubCreatePullRequestError), culture, arg0, arg1, arg2, arg3);
    }

    public static string GitHubBranchNotFound(object arg0, object arg1) => BuildServerResources.Format(nameof (GitHubBranchNotFound), arg0, arg1);

    public static string GitHubBranchNotFound(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (GitHubBranchNotFound), culture, arg0, arg1);

    public static string GitHubFileAlreadyExists(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (GitHubFileAlreadyExists), arg0, arg1, arg2);

    public static string GitHubFileAlreadyExists(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubFileAlreadyExists), culture, arg0, arg1, arg2);
    }

    public static string GitHubBranchAlreadyExists(object arg0, object arg1) => BuildServerResources.Format(nameof (GitHubBranchAlreadyExists), arg0, arg1);

    public static string GitHubBranchAlreadyExists(object arg0, object arg1, CultureInfo culture) => BuildServerResources.Format(nameof (GitHubBranchAlreadyExists), culture, arg0, arg1);

    public static string RetryNotSupportedForPausedDefinition(object arg0, object arg1) => BuildServerResources.Format(nameof (RetryNotSupportedForPausedDefinition), arg0, arg1);

    public static string RetryNotSupportedForPausedDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (RetryNotSupportedForPausedDefinition), culture, arg0, arg1);
    }

    public static string RetryNotSupportedForReason(object arg0) => BuildServerResources.Format(nameof (RetryNotSupportedForReason), arg0);

    public static string RetryNotSupportedForReason(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (RetryNotSupportedForReason), culture, arg0);

    public static string DockerProcess_BuildStep() => BuildServerResources.Get(nameof (DockerProcess_BuildStep));

    public static string DockerProcess_BuildStep(CultureInfo culture) => BuildServerResources.Get(nameof (DockerProcess_BuildStep), culture);

    public static string ProcessTypeNotSupported(object arg0) => BuildServerResources.Format(nameof (ProcessTypeNotSupported), arg0);

    public static string ProcessTypeNotSupported(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (ProcessTypeNotSupported), culture, arg0);

    public static string GitHubRepoIdIsInvalid() => BuildServerResources.Get(nameof (GitHubRepoIdIsInvalid));

    public static string GitHubRepoIdIsInvalid(CultureInfo culture) => BuildServerResources.Get(nameof (GitHubRepoIdIsInvalid), culture);

    public static string AccessDeniedForBuild(object arg0, object arg1, object arg2, object arg3) => BuildServerResources.Format(nameof (AccessDeniedForBuild), arg0, arg1, arg2, arg3);

    public static string AccessDeniedForBuild(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (AccessDeniedForBuild), culture, arg0, arg1, arg2, arg3);
    }

    public static string DefinitionRestoredBecauseRetainedByRelease(object arg0) => BuildServerResources.Format(nameof (DefinitionRestoredBecauseRetainedByRelease), arg0);

    public static string DefinitionRestoredBecauseRetainedByRelease(
      object arg0,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (DefinitionRestoredBecauseRetainedByRelease), culture, arg0);
    }

    public static string ResourcesQueryRequiresType() => BuildServerResources.Get(nameof (ResourcesQueryRequiresType));

    public static string ResourcesQueryRequiresType(CultureInfo culture) => BuildServerResources.Get(nameof (ResourcesQueryRequiresType), culture);

    public static string GatedCheckInValidatorNotFound() => BuildServerResources.Get(nameof (GatedCheckInValidatorNotFound));

    public static string GatedCheckInValidatorNotFound(CultureInfo culture) => BuildServerResources.Get(nameof (GatedCheckInValidatorNotFound), culture);

    public static string UpdateDefinition_ServicingContextCannotAuthorizeResources() => BuildServerResources.Get(nameof (UpdateDefinition_ServicingContextCannotAuthorizeResources));

    public static string UpdateDefinition_ServicingContextCannotAuthorizeResources(
      CultureInfo culture)
    {
      return BuildServerResources.Get(nameof (UpdateDefinition_ServicingContextCannotAuthorizeResources), culture);
    }

    public static string PipelineRetentionWorkerJobName() => BuildServerResources.Get(nameof (PipelineRetentionWorkerJobName));

    public static string PipelineRetentionWorkerJobName(CultureInfo culture) => BuildServerResources.Get(nameof (PipelineRetentionWorkerJobName), culture);

    public static string GitHubCheckSkippedCommentHasSkipCIKeyword() => BuildServerResources.Get(nameof (GitHubCheckSkippedCommentHasSkipCIKeyword));

    public static string GitHubCheckSkippedCommentHasSkipCIKeyword(CultureInfo culture) => BuildServerResources.Get(nameof (GitHubCheckSkippedCommentHasSkipCIKeyword), culture);

    public static string GitHubCheckSkippedNoMatchingBranchFilter() => BuildServerResources.Get(nameof (GitHubCheckSkippedNoMatchingBranchFilter));

    public static string GitHubCheckSkippedNoMatchingBranchFilter(CultureInfo culture) => BuildServerResources.Get(nameof (GitHubCheckSkippedNoMatchingBranchFilter), culture);

    public static string GitHubCheckSkippedNoMatchingPathFilter() => BuildServerResources.Get(nameof (GitHubCheckSkippedNoMatchingPathFilter));

    public static string GitHubCheckSkippedNoMatchingPathFilter(CultureInfo culture) => BuildServerResources.Get(nameof (GitHubCheckSkippedNoMatchingPathFilter), culture);

    public static string GitHubCheckSkippedNoMatchingTriggers() => BuildServerResources.Get(nameof (GitHubCheckSkippedNoMatchingTriggers));

    public static string GitHubCheckSkippedNoMatchingTriggers(CultureInfo culture) => BuildServerResources.Get(nameof (GitHubCheckSkippedNoMatchingTriggers), culture);

    public static string NumericFolderNameNotAllowed() => BuildServerResources.Get(nameof (NumericFolderNameNotAllowed));

    public static string NumericFolderNameNotAllowed(CultureInfo culture) => BuildServerResources.Get(nameof (NumericFolderNameNotAllowed), culture);

    public static string YamlConfigurationPipelineRequired() => BuildServerResources.Get(nameof (YamlConfigurationPipelineRequired));

    public static string YamlConfigurationPipelineRequired(CultureInfo culture) => BuildServerResources.Get(nameof (YamlConfigurationPipelineRequired), culture);

    public static string EnforceSettableVarErrorMessage(object arg0) => BuildServerResources.Format(nameof (EnforceSettableVarErrorMessage), arg0);

    public static string EnforceSettableVarErrorMessage(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (EnforceSettableVarErrorMessage), culture, arg0);

    public static string EnforceSettableVarWarningMessage(object arg0) => BuildServerResources.Format(nameof (EnforceSettableVarWarningMessage), arg0);

    public static string EnforceSettableVarWarningMessage(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (EnforceSettableVarWarningMessage), culture, arg0);

    public static string InvalidTimelineRecordStateChange(object arg0) => BuildServerResources.Format(nameof (InvalidTimelineRecordStateChange), arg0);

    public static string InvalidTimelineRecordStateChange(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidTimelineRecordStateChange), culture, arg0);

    public static string InvalidGuidValue(object arg0) => BuildServerResources.Format(nameof (InvalidGuidValue), arg0);

    public static string InvalidGuidValue(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidGuidValue), culture, arg0);

    public static string CannotDeleteFolderDueToHiddenDefinitions(object arg0) => BuildServerResources.Format(nameof (CannotDeleteFolderDueToHiddenDefinitions), arg0);

    public static string CannotDeleteFolderDueToHiddenDefinitions(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (CannotDeleteFolderDueToHiddenDefinitions), culture, arg0);

    public static string RemoveLeaseAccessDeniedException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return BuildServerResources.Format(nameof (RemoveLeaseAccessDeniedException), arg0, arg1, arg2, arg3);
    }

    public static string RemoveLeaseAccessDeniedException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (RemoveLeaseAccessDeniedException), culture, arg0, arg1, arg2, arg3);
    }

    public static string MustIncludeDefinitionIdWithRunId() => BuildServerResources.Get(nameof (MustIncludeDefinitionIdWithRunId));

    public static string MustIncludeDefinitionIdWithRunId(CultureInfo culture) => BuildServerResources.Get(nameof (MustIncludeDefinitionIdWithRunId), culture);

    public static string MustSpecifyAtLeastOneRetentionLeaseProperty() => BuildServerResources.Get(nameof (MustSpecifyAtLeastOneRetentionLeaseProperty));

    public static string MustSpecifyAtLeastOneRetentionLeaseProperty(CultureInfo culture) => BuildServerResources.Get(nameof (MustSpecifyAtLeastOneRetentionLeaseProperty), culture);

    public static string InvalidStageToSkip(object arg0) => BuildServerResources.Format(nameof (InvalidStageToSkip), arg0);

    public static string InvalidStageToSkip(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidStageToSkip), culture, arg0);

    public static string SkippingAllStages() => BuildServerResources.Get(nameof (SkippingAllStages));

    public static string SkippingAllStages(CultureInfo culture) => BuildServerResources.Get(nameof (SkippingAllStages), culture);

    public static string GitHubReadRepositoryRefFailure(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (GitHubReadRepositoryRefFailure), arg0, arg1, arg2);

    public static string GitHubReadRepositoryRefFailure(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (GitHubReadRepositoryRefFailure), culture, arg0, arg1, arg2);
    }

    public static string MissingLeasesToFetchParameter() => BuildServerResources.Get(nameof (MissingLeasesToFetchParameter));

    public static string MissingLeasesToFetchParameter(CultureInfo culture) => BuildServerResources.Get(nameof (MissingLeasesToFetchParameter), culture);

    public static string WrongStageCount(object arg0) => BuildServerResources.Format(nameof (WrongStageCount), arg0);

    public static string WrongStageCount(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (WrongStageCount), culture, arg0);

    public static string UnrecognizedPhaseTarget(object arg0) => BuildServerResources.Format(nameof (UnrecognizedPhaseTarget), arg0);

    public static string UnrecognizedPhaseTarget(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (UnrecognizedPhaseTarget), culture, arg0);

    public static string RetentionLeaseNotFound(object arg0) => BuildServerResources.Format(nameof (RetentionLeaseNotFound), arg0);

    public static string RetentionLeaseNotFound(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (RetentionLeaseNotFound), culture, arg0);

    public static string RetentionLeaseUpdateIsNull() => BuildServerResources.Get(nameof (RetentionLeaseUpdateIsNull));

    public static string RetentionLeaseUpdateIsNull(CultureInfo culture) => BuildServerResources.Get(nameof (RetentionLeaseUpdateIsNull), culture);

    public static string MustSpecifyAtLeastOneRetentionProperty() => BuildServerResources.Get(nameof (MustSpecifyAtLeastOneRetentionProperty));

    public static string MustSpecifyAtLeastOneRetentionProperty(CultureInfo culture) => BuildServerResources.Get(nameof (MustSpecifyAtLeastOneRetentionProperty), culture);

    public static string RepositoryNotFound(object arg0, object arg1, object arg2) => BuildServerResources.Format(nameof (RepositoryNotFound), arg0, arg1, arg2);

    public static string RepositoryNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return BuildServerResources.Format(nameof (RepositoryNotFound), culture, arg0, arg1, arg2);
    }

    public static string YamlIsBroken() => BuildServerResources.Get(nameof (YamlIsBroken));

    public static string YamlIsBroken(CultureInfo culture) => BuildServerResources.Get(nameof (YamlIsBroken), culture);

    public static string CannotUpdateFolderDueToLackOfPermissions(object arg0) => BuildServerResources.Format(nameof (CannotUpdateFolderDueToLackOfPermissions), arg0);

    public static string CannotUpdateFolderDueToLackOfPermissions(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (CannotUpdateFolderDueToLackOfPermissions), culture, arg0);

    public static string ClassicPipelinesDisabled() => BuildServerResources.Get(nameof (ClassicPipelinesDisabled));

    public static string ClassicPipelinesDisabled(CultureInfo culture) => BuildServerResources.Get(nameof (ClassicPipelinesDisabled), culture);

    public static string InvalidBuildParametersJson(object arg0) => BuildServerResources.Format(nameof (InvalidBuildParametersJson), arg0);

    public static string InvalidBuildParametersJson(object arg0, CultureInfo culture) => BuildServerResources.Format(nameof (InvalidBuildParametersJson), culture, arg0);

    public static string ImmutableSourceBranch() => BuildServerResources.Get(nameof (ImmutableSourceBranch));

    public static string ImmutableSourceBranch(CultureInfo culture) => BuildServerResources.Get(nameof (ImmutableSourceBranch), culture);

    public static string EditPipelineQueueConfigurationPermission() => BuildServerResources.Get(nameof (EditPipelineQueueConfigurationPermission));

    public static string EditPipelineQueueConfigurationPermission(CultureInfo culture) => BuildServerResources.Get(nameof (EditPipelineQueueConfigurationPermission), culture);
  }
}
