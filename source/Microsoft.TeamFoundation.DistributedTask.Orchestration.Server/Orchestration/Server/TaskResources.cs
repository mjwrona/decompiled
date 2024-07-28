// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class TaskResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (TaskResources), typeof (TaskResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => TaskResources.s_resMgr;

    private static string Get(string resourceName) => TaskResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? TaskResources.Get(resourceName) : TaskResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) TaskResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? TaskResources.GetInt(resourceName) : (int) TaskResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) TaskResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? TaskResources.GetBool(resourceName) : (bool) TaskResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => TaskResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = TaskResources.Get(resourceName, culture);
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

    public static string PlanNotFound(object arg0) => TaskResources.Format(nameof (PlanNotFound), arg0);

    public static string PlanNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PlanNotFound), culture, arg0);

    public static string PlanSecurityDeleteError(object arg0, object arg1) => TaskResources.Format(nameof (PlanSecurityDeleteError), arg0, arg1);

    public static string PlanSecurityDeleteError(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (PlanSecurityDeleteError), culture, arg0, arg1);

    public static string PlanSecurityWriteError(object arg0, object arg1) => TaskResources.Format(nameof (PlanSecurityWriteError), arg0, arg1);

    public static string PlanSecurityWriteError(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (PlanSecurityWriteError), culture, arg0, arg1);

    public static string HubExtensionNotFound(object arg0) => TaskResources.Format(nameof (HubExtensionNotFound), arg0);

    public static string HubExtensionNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (HubExtensionNotFound), culture, arg0);

    public static string SecurityTokenNotFound(object arg0, object arg1) => TaskResources.Format(nameof (SecurityTokenNotFound), arg0, arg1);

    public static string SecurityTokenNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (SecurityTokenNotFound), culture, arg0, arg1);

    public static string TimelineNotFound(object arg0, object arg1) => TaskResources.Format(nameof (TimelineNotFound), arg0, arg1);

    public static string TimelineNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (TimelineNotFound), culture, arg0, arg1);

    public static string LogWithNoContentError() => TaskResources.Get(nameof (LogWithNoContentError));

    public static string LogWithNoContentError(CultureInfo culture) => TaskResources.Get(nameof (LogWithNoContentError), culture);

    public static string LogWithNoContentLengthError() => TaskResources.Get(nameof (LogWithNoContentLengthError));

    public static string LogWithNoContentLengthError(CultureInfo culture) => TaskResources.Get(nameof (LogWithNoContentLengthError), culture);

    public static string UnsupportedRollbackContainers() => TaskResources.Get(nameof (UnsupportedRollbackContainers));

    public static string UnsupportedRollbackContainers(CultureInfo culture) => TaskResources.Get(nameof (UnsupportedRollbackContainers), culture);

    public static string HubNotFound(object arg0) => TaskResources.Format(nameof (HubNotFound), arg0);

    public static string HubNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (HubNotFound), culture, arg0);

    public static string MultipleHubResolversNotSupported(object arg0) => TaskResources.Format(nameof (MultipleHubResolversNotSupported), arg0);

    public static string MultipleHubResolversNotSupported(object arg0, CultureInfo culture) => TaskResources.Format(nameof (MultipleHubResolversNotSupported), culture, arg0);

    public static string HubExists(object arg0) => TaskResources.Format(nameof (HubExists), arg0);

    public static string HubExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (HubExists), culture, arg0);

    public static string TimelineRecordInvalid(object arg0) => TaskResources.Format(nameof (TimelineRecordInvalid), arg0);

    public static string TimelineRecordInvalid(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TimelineRecordInvalid), culture, arg0);

    public static string TimelineRecordNotFound(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (TimelineRecordNotFound), arg0, arg1, arg2);

    public static string TimelineRecordNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TimelineRecordNotFound), culture, arg0, arg1, arg2);
    }

    public static string FailedToObtainJobAuthorization(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (FailedToObtainJobAuthorization), arg0, arg1, arg2);

    public static string FailedToObtainJobAuthorization(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (FailedToObtainJobAuthorization), culture, arg0, arg1, arg2);
    }

    public static string TaskInputRequired(object arg0, object arg1) => TaskResources.Format(nameof (TaskInputRequired), arg0, arg1);

    public static string TaskInputRequired(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (TaskInputRequired), culture, arg0, arg1);

    public static string PlanOrchestrationTerminated(object arg0) => TaskResources.Format(nameof (PlanOrchestrationTerminated), arg0);

    public static string PlanOrchestrationTerminated(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PlanOrchestrationTerminated), culture, arg0);

    public static string PlanAlreadyStarted(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (PlanAlreadyStarted), arg0, arg1, arg2);

    public static string PlanAlreadyStarted(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (PlanAlreadyStarted), culture, arg0, arg1, arg2);
    }

    public static string TimelineExists(object arg0, object arg1) => TaskResources.Format(nameof (TimelineExists), arg0, arg1);

    public static string TimelineExists(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (TimelineExists), culture, arg0, arg1);

    public static string InvalidContainer(object arg0) => TaskResources.Format(nameof (InvalidContainer), arg0);

    public static string InvalidContainer(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidContainer), culture, arg0);

    public static string EndpointNotFound(object arg0) => TaskResources.Format(nameof (EndpointNotFound), arg0);

    public static string EndpointNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EndpointNotFound), culture, arg0);

    public static string ShouldBeASubdomainOfEndpointUrl() => TaskResources.Get(nameof (ShouldBeASubdomainOfEndpointUrl));

    public static string ShouldBeASubdomainOfEndpointUrl(CultureInfo culture) => TaskResources.Get(nameof (ShouldBeASubdomainOfEndpointUrl), culture);

    public static string TaskExecutionDefinitionInvalid(object arg0) => TaskResources.Format(nameof (TaskExecutionDefinitionInvalid), arg0);

    public static string TaskExecutionDefinitionInvalid(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskExecutionDefinitionInvalid), culture, arg0);

    public static string ServerExecutionFailure(object arg0) => TaskResources.Format(nameof (ServerExecutionFailure), arg0);

    public static string ServerExecutionFailure(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ServerExecutionFailure), culture, arg0);

    public static string UnsupportedTaskCountForServerJob() => TaskResources.Get(nameof (UnsupportedTaskCountForServerJob));

    public static string UnsupportedTaskCountForServerJob(CultureInfo culture) => TaskResources.Get(nameof (UnsupportedTaskCountForServerJob), culture);

    public static string TaskServiceBusPublishFailed(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (TaskServiceBusPublishFailed), arg0, arg1, arg2);

    public static string TaskServiceBusPublishFailed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskServiceBusPublishFailed), culture, arg0, arg1, arg2);
    }

    public static string TaskServiceBusExecutionFailure(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (TaskServiceBusExecutionFailure), arg0, arg1, arg2, arg3);
    }

    public static string TaskServiceBusExecutionFailure(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TaskServiceBusExecutionFailure), culture, arg0, arg1, arg2, arg3);
    }

    public static string TimeoutFormatNotValid(object arg0, object arg1, object arg2) => TaskResources.Format(nameof (TimeoutFormatNotValid), arg0, arg1, arg2);

    public static string TimeoutFormatNotValid(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TimeoutFormatNotValid), culture, arg0, arg1, arg2);
    }

    public static string JobNotFound(object arg0, object arg1) => TaskResources.Format(nameof (JobNotFound), arg0, arg1);

    public static string JobNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (JobNotFound), culture, arg0, arg1);

    public static string PlanGroupNotFound(object arg0) => TaskResources.Format(nameof (PlanGroupNotFound), arg0);

    public static string PlanGroupNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PlanGroupNotFound), culture, arg0);

    public static string PlanSecurityReadError(object arg0, object arg1) => TaskResources.Format(nameof (PlanSecurityReadError), arg0, arg1);

    public static string PlanSecurityReadError(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (PlanSecurityReadError), culture, arg0, arg1);

    public static string SaveJobOutputVariablesError(object arg0, object arg1) => TaskResources.Format(nameof (SaveJobOutputVariablesError), arg0, arg1);

    public static string SaveJobOutputVariablesError(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (SaveJobOutputVariablesError), culture, arg0, arg1);

    public static string InvalidScopeId(object arg0) => TaskResources.Format(nameof (InvalidScopeId), arg0);

    public static string InvalidScopeId(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidScopeId), culture, arg0);

    public static string InvalidOrchestrationId(object arg0) => TaskResources.Format(nameof (InvalidOrchestrationId), arg0);

    public static string InvalidOrchestrationId(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidOrchestrationId), culture, arg0);

    public static string ServiceEndPointNotFound(object arg0) => TaskResources.Format(nameof (ServiceEndPointNotFound), arg0);

    public static string ServiceEndPointNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ServiceEndPointNotFound), culture, arg0);

    public static string InvalidLicenseHub(object arg0) => TaskResources.Format(nameof (InvalidLicenseHub), arg0);

    public static string InvalidLicenseHub(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidLicenseHub), culture, arg0);

    public static string HttpMethodNotRecognized(object arg0) => TaskResources.Format(nameof (HttpMethodNotRecognized), arg0);

    public static string HttpMethodNotRecognized(object arg0, CultureInfo culture) => TaskResources.Format(nameof (HttpMethodNotRecognized), culture, arg0);

    public static string UrlCannotBeEmpty() => TaskResources.Get(nameof (UrlCannotBeEmpty));

    public static string UrlCannotBeEmpty(CultureInfo culture) => TaskResources.Get(nameof (UrlCannotBeEmpty), culture);

    public static string UrlIsNotCorrect(object arg0) => TaskResources.Format(nameof (UrlIsNotCorrect), arg0);

    public static string UrlIsNotCorrect(object arg0, CultureInfo culture) => TaskResources.Format(nameof (UrlIsNotCorrect), culture, arg0);

    public static string WaitForCompletionInvalid(object arg0) => TaskResources.Format(nameof (WaitForCompletionInvalid), arg0);

    public static string WaitForCompletionInvalid(object arg0, CultureInfo culture) => TaskResources.Format(nameof (WaitForCompletionInvalid), culture, arg0);

    public static string HttpRequestTimeoutError(object arg0) => TaskResources.Format(nameof (HttpRequestTimeoutError), arg0);

    public static string HttpRequestTimeoutError(object arg0, CultureInfo culture) => TaskResources.Format(nameof (HttpRequestTimeoutError), culture, arg0);

    public static string UnableToAcquireLease(object arg0) => TaskResources.Format(nameof (UnableToAcquireLease), arg0);

    public static string UnableToAcquireLease(object arg0, CultureInfo culture) => TaskResources.Format(nameof (UnableToAcquireLease), culture, arg0);

    public static string UnableToCompleteOperationSecurely() => TaskResources.Get(nameof (UnableToCompleteOperationSecurely));

    public static string UnableToCompleteOperationSecurely(CultureInfo culture) => TaskResources.Get(nameof (UnableToCompleteOperationSecurely), culture);

    public static string CancellingHttpRequestException(object arg0) => TaskResources.Format(nameof (CancellingHttpRequestException), arg0);

    public static string CancellingHttpRequestException(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CancellingHttpRequestException), culture, arg0);

    public static string EncryptionKeyNotFound(object arg0, object arg1) => TaskResources.Format(nameof (EncryptionKeyNotFound), arg0, arg1);

    public static string EncryptionKeyNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (EncryptionKeyNotFound), culture, arg0, arg1);

    public static string ProcessingHttpRequestException(object arg0) => TaskResources.Format(nameof (ProcessingHttpRequestException), arg0);

    public static string ProcessingHttpRequestException(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ProcessingHttpRequestException), culture, arg0);

    public static string AzureKeyVaultTaskName(object arg0) => TaskResources.Format(nameof (AzureKeyVaultTaskName), arg0);

    public static string AzureKeyVaultTaskName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AzureKeyVaultTaskName), culture, arg0);

    public static string AzureKeyVaultServiceEndpointIdMustBeValidGuid() => TaskResources.Get(nameof (AzureKeyVaultServiceEndpointIdMustBeValidGuid));

    public static string AzureKeyVaultServiceEndpointIdMustBeValidGuid(CultureInfo culture) => TaskResources.Get(nameof (AzureKeyVaultServiceEndpointIdMustBeValidGuid), culture);

    public static string AzureKeyVaultKeyVaultNameMustBeValid() => TaskResources.Get(nameof (AzureKeyVaultKeyVaultNameMustBeValid));

    public static string AzureKeyVaultKeyVaultNameMustBeValid(CultureInfo culture) => TaskResources.Get(nameof (AzureKeyVaultKeyVaultNameMustBeValid), culture);

    public static string AzureKeyVaultLastRefreshedOnMustBeValid() => TaskResources.Get(nameof (AzureKeyVaultLastRefreshedOnMustBeValid));

    public static string AzureKeyVaultLastRefreshedOnMustBeValid(CultureInfo culture) => TaskResources.Get(nameof (AzureKeyVaultLastRefreshedOnMustBeValid), culture);

    public static string InvalidAzureKeyVaultVariableGroupProviderData() => TaskResources.Get(nameof (InvalidAzureKeyVaultVariableGroupProviderData));

    public static string InvalidAzureKeyVaultVariableGroupProviderData(CultureInfo culture) => TaskResources.Get(nameof (InvalidAzureKeyVaultVariableGroupProviderData), culture);

    public static string VariableGroupTypeNotSupported(object arg0) => TaskResources.Format(nameof (VariableGroupTypeNotSupported), arg0);

    public static string VariableGroupTypeNotSupported(object arg0, CultureInfo culture) => TaskResources.Format(nameof (VariableGroupTypeNotSupported), culture, arg0);

    public static string TaskRequestMessageTypeNotSupported(object arg0) => TaskResources.Format(nameof (TaskRequestMessageTypeNotSupported), arg0);

    public static string TaskRequestMessageTypeNotSupported(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskRequestMessageTypeNotSupported), culture, arg0);

    public static string HttpHandlerUnableToProcessError(object arg0, object arg1) => TaskResources.Format(nameof (HttpHandlerUnableToProcessError), arg0, arg1);

    public static string HttpHandlerUnableToProcessError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (HttpHandlerUnableToProcessError), culture, arg0, arg1);
    }

    public static string YamlFrontMatterNotClosed(object arg0) => TaskResources.Format(nameof (YamlFrontMatterNotClosed), arg0);

    public static string YamlFrontMatterNotClosed(object arg0, CultureInfo culture) => TaskResources.Format(nameof (YamlFrontMatterNotClosed), culture, arg0);

    public static string YamlFrontMatterNotValid(object arg0, object arg1) => TaskResources.Format(nameof (YamlFrontMatterNotValid), arg0, arg1);

    public static string YamlFrontMatterNotValid(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (YamlFrontMatterNotValid), culture, arg0, arg1);

    public static string YamlFileCount(object arg0) => TaskResources.Format(nameof (YamlFileCount), arg0);

    public static string YamlFileCount(object arg0, CultureInfo culture) => TaskResources.Format(nameof (YamlFileCount), culture, arg0);

    public static string UnableToPopulateAzureStackData() => TaskResources.Get(nameof (UnableToPopulateAzureStackData));

    public static string UnableToPopulateAzureStackData(CultureInfo culture) => TaskResources.Get(nameof (UnableToPopulateAzureStackData), culture);

    public static string EvaluationOfExpressionFailed(object arg0) => TaskResources.Format(nameof (EvaluationOfExpressionFailed), arg0);

    public static string EvaluationOfExpressionFailed(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EvaluationOfExpressionFailed), culture, arg0);

    public static string CouldNotParseApiResponse() => TaskResources.Get(nameof (CouldNotParseApiResponse));

    public static string CouldNotParseApiResponse(CultureInfo culture) => TaskResources.Get(nameof (CouldNotParseApiResponse), culture);

    public static string TaskInputValidationReason(object arg0) => TaskResources.Format(nameof (TaskInputValidationReason), arg0);

    public static string TaskInputValidationReason(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskInputValidationReason), culture, arg0);

    public static string TaskInputInvalidRegExOptions(object arg0) => TaskResources.Format(nameof (TaskInputInvalidRegExOptions), arg0);

    public static string TaskInputInvalidRegExOptions(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskInputInvalidRegExOptions), culture, arg0);

    public static string PipelineNotSupported() => TaskResources.Get(nameof (PipelineNotSupported));

    public static string PipelineNotSupported(CultureInfo culture) => TaskResources.Get(nameof (PipelineNotSupported), culture);

    public static string QueueNotFound(object arg0) => TaskResources.Format(nameof (QueueNotFound), arg0);

    public static string QueueNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (QueueNotFound), culture, arg0);

    public static string PhaseNotFound(object arg0, object arg1) => TaskResources.Format(nameof (PhaseNotFound), arg0, arg1);

    public static string PhaseNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (PhaseNotFound), culture, arg0, arg1);

    public static string TaskDefinitionNotFound(object arg0, object arg1) => TaskResources.Format(nameof (TaskDefinitionNotFound), arg0, arg1);

    public static string TaskDefinitionNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (TaskDefinitionNotFound), culture, arg0, arg1);

    public static string HttpRequestResponseSizeExceeded() => TaskResources.Get(nameof (HttpRequestResponseSizeExceeded));

    public static string HttpRequestResponseSizeExceeded(CultureInfo culture) => TaskResources.Get(nameof (HttpRequestResponseSizeExceeded), culture);

    public static string ResponseParsingIsCurrentlyDisabled() => TaskResources.Get(nameof (ResponseParsingIsCurrentlyDisabled));

    public static string ResponseParsingIsCurrentlyDisabled(CultureInfo culture) => TaskResources.Get(nameof (ResponseParsingIsCurrentlyDisabled), culture);

    public static string PlanLogNotFound(object arg0, object arg1) => TaskResources.Format(nameof (PlanLogNotFound), arg0, arg1);

    public static string PlanLogNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (PlanLogNotFound), culture, arg0, arg1);

    public static string TaskEndpointInDirtyState(object arg0) => TaskResources.Format(nameof (TaskEndpointInDirtyState), arg0);

    public static string TaskEndpointInDirtyState(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskEndpointInDirtyState), culture, arg0);

    public static string EndpointUsedInTaskNotFound(object arg0) => TaskResources.Format(nameof (EndpointUsedInTaskNotFound), arg0);

    public static string EndpointUsedInTaskNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EndpointUsedInTaskNotFound), culture, arg0);

    public static string ServiceEndpointLinkedToTask(object arg0, object arg1) => TaskResources.Format(nameof (ServiceEndpointLinkedToTask), arg0, arg1);

    public static string ServiceEndpointLinkedToTask(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (ServiceEndpointLinkedToTask), culture, arg0, arg1);

    public static string TimelineRecordAttemptInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TaskResources.Format(nameof (TimelineRecordAttemptInvalid), arg0, arg1, arg2, arg3);
    }

    public static string TimelineRecordAttemptInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (TimelineRecordAttemptInvalid), culture, arg0, arg1, arg2, arg3);
    }

    public static string InvalidPipelineRetryDidNotFail(object arg0) => TaskResources.Format(nameof (InvalidPipelineRetryDidNotFail), arg0);

    public static string InvalidPipelineRetryDidNotFail(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidPipelineRetryDidNotFail), culture, arg0);

    public static string InvalidPipelineRetryNotCompleted(object arg0) => TaskResources.Format(nameof (InvalidPipelineRetryNotCompleted), arg0);

    public static string InvalidPipelineRetryNotCompleted(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidPipelineRetryNotCompleted), culture, arg0);

    public static string InvalidTypeForIsNullOrEmptyFunction(object arg0) => TaskResources.Format(nameof (InvalidTypeForIsNullOrEmptyFunction), arg0);

    public static string InvalidTypeForIsNullOrEmptyFunction(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidTypeForIsNullOrEmptyFunction), culture, arg0);

    public static string VisibilityRuleEvaluationFailure(object arg0) => TaskResources.Format(nameof (VisibilityRuleEvaluationFailure), arg0);

    public static string VisibilityRuleEvaluationFailure(object arg0, CultureInfo culture) => TaskResources.Format(nameof (VisibilityRuleEvaluationFailure), culture, arg0);

    public static string ArtifactTypeNotFound(object arg0) => TaskResources.Format(nameof (ArtifactTypeNotFound), arg0);

    public static string ArtifactTypeNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ArtifactTypeNotFound), culture, arg0);

    public static string CannotLoadArtifactTypeDefinition(object arg0) => TaskResources.Format(nameof (CannotLoadArtifactTypeDefinition), arg0);

    public static string CannotLoadArtifactTypeDefinition(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotLoadArtifactTypeDefinition), culture, arg0);

    public static string DownloadArtifact(object arg0) => TaskResources.Format(nameof (DownloadArtifact), arg0);

    public static string DownloadArtifact(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DownloadArtifact), culture, arg0);

    public static string UnableToResolveLatestPipelineVersion(object arg0) => TaskResources.Format(nameof (UnableToResolveLatestPipelineVersion), arg0);

    public static string UnableToResolveLatestPipelineVersion(object arg0, CultureInfo culture) => TaskResources.Format(nameof (UnableToResolveLatestPipelineVersion), culture, arg0);

    public static string CannotFindCurrentPipelineId() => TaskResources.Get(nameof (CannotFindCurrentPipelineId));

    public static string CannotFindCurrentPipelineId(CultureInfo culture) => TaskResources.Get(nameof (CannotFindCurrentPipelineId), culture);

    public static string PipelineResourceProjectInputRequired() => TaskResources.Get(nameof (PipelineResourceProjectInputRequired));

    public static string PipelineResourceProjectInputRequired(CultureInfo culture) => TaskResources.Get(nameof (PipelineResourceProjectInputRequired), culture);

    public static string PipelineResourceSourceInputRequired() => TaskResources.Get(nameof (PipelineResourceSourceInputRequired));

    public static string PipelineResourceSourceInputRequired(CultureInfo culture) => TaskResources.Get(nameof (PipelineResourceSourceInputRequired), culture);

    public static string PipelineResourceTagMustBeNonEmpty() => TaskResources.Get(nameof (PipelineResourceTagMustBeNonEmpty));

    public static string PipelineResourceTagMustBeNonEmpty(CultureInfo culture) => TaskResources.Get(nameof (PipelineResourceTagMustBeNonEmpty), culture);

    public static string ArtifactTypeDuplicateIdentifier(object arg0) => TaskResources.Format(nameof (ArtifactTypeDuplicateIdentifier), arg0);

    public static string ArtifactTypeDuplicateIdentifier(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ArtifactTypeDuplicateIdentifier), culture, arg0);

    public static string UnableToResolvePipelineVersion(object arg0, object arg1) => TaskResources.Format(nameof (UnableToResolvePipelineVersion), arg0, arg1);

    public static string UnableToResolvePipelineVersion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (UnableToResolvePipelineVersion), culture, arg0, arg1);
    }

    public static string DataSourceBindingMissing(object arg0) => TaskResources.Format(nameof (DataSourceBindingMissing), arg0);

    public static string DataSourceBindingMissing(object arg0, CultureInfo culture) => TaskResources.Format(nameof (DataSourceBindingMissing), culture, arg0);

    public static string MissingProperty(object arg0) => TaskResources.Format(nameof (MissingProperty), arg0);

    public static string MissingProperty(object arg0, CultureInfo culture) => TaskResources.Format(nameof (MissingProperty), culture, arg0);

    public static string InvalidServiceEndpointId(object arg0) => TaskResources.Format(nameof (InvalidServiceEndpointId), arg0);

    public static string InvalidServiceEndpointId(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidServiceEndpointId), culture, arg0);

    public static string NoArtifactVersionsAvailable() => TaskResources.Get(nameof (NoArtifactVersionsAvailable));

    public static string NoArtifactVersionsAvailable(CultureInfo culture) => TaskResources.Get(nameof (NoArtifactVersionsAvailable), culture);

    public static string NoArtifactVersionsForBranchAndTagsAvailable(object arg0, object arg1) => TaskResources.Format(nameof (NoArtifactVersionsForBranchAndTagsAvailable), arg0, arg1);

    public static string NoArtifactVersionsForBranchAndTagsAvailable(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (NoArtifactVersionsForBranchAndTagsAvailable), culture, arg0, arg1);
    }

    public static string NoArtifactVersionsForBranchAvailable(object arg0) => TaskResources.Format(nameof (NoArtifactVersionsForBranchAvailable), arg0);

    public static string NoArtifactVersionsForBranchAvailable(object arg0, CultureInfo culture) => TaskResources.Format(nameof (NoArtifactVersionsForBranchAvailable), culture, arg0);

    public static string NoArtifactVersionsForTagsAvailable(object arg0) => TaskResources.Format(nameof (NoArtifactVersionsForTagsAvailable), arg0);

    public static string NoArtifactVersionsForTagsAvailable(object arg0, CultureInfo culture) => TaskResources.Format(nameof (NoArtifactVersionsForTagsAvailable), culture, arg0);

    public static string NoSpecificVersionValueAvailableForSpecificVersionType() => TaskResources.Get(nameof (NoSpecificVersionValueAvailableForSpecificVersionType));

    public static string NoSpecificVersionValueAvailableForSpecificVersionType(CultureInfo culture) => TaskResources.Get(nameof (NoSpecificVersionValueAvailableForSpecificVersionType), culture);

    public static string SpecificArtifactVersionNotAvailable(object arg0) => TaskResources.Format(nameof (SpecificArtifactVersionNotAvailable), arg0);

    public static string SpecificArtifactVersionNotAvailable(object arg0, CultureInfo culture) => TaskResources.Format(nameof (SpecificArtifactVersionNotAvailable), culture, arg0);

    public static string ArtifactDataSourceBindingNotFound(object arg0, object arg1) => TaskResources.Format(nameof (ArtifactDataSourceBindingNotFound), arg0, arg1);

    public static string ArtifactDataSourceBindingNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (ArtifactDataSourceBindingNotFound), culture, arg0, arg1);
    }

    public static string ArtifactTypeNotBrowsableError() => TaskResources.Get(nameof (ArtifactTypeNotBrowsableError));

    public static string ArtifactTypeNotBrowsableError(CultureInfo culture) => TaskResources.Get(nameof (ArtifactTypeNotBrowsableError), culture);

    public static string NoArtifactTypeError() => TaskResources.Get(nameof (NoArtifactTypeError));

    public static string NoArtifactTypeError(CultureInfo culture) => TaskResources.Get(nameof (NoArtifactTypeError), culture);

    public static string NoEndpointAuthorizationPresent() => TaskResources.Get(nameof (NoEndpointAuthorizationPresent));

    public static string NoEndpointAuthorizationPresent(CultureInfo culture) => TaskResources.Get(nameof (NoEndpointAuthorizationPresent), culture);

    public static string NoEndpointPresent() => TaskResources.Get(nameof (NoEndpointPresent));

    public static string NoEndpointPresent(CultureInfo culture) => TaskResources.Get(nameof (NoEndpointPresent), culture);

    public static string NoVersionFound() => TaskResources.Get(nameof (NoVersionFound));

    public static string NoVersionFound(CultureInfo culture) => TaskResources.Get(nameof (NoVersionFound), culture);

    public static string UnableToDetermineTargetPool(object arg0) => TaskResources.Format(nameof (UnableToDetermineTargetPool), arg0);

    public static string UnableToDetermineTargetPool(object arg0, CultureInfo culture) => TaskResources.Format(nameof (UnableToDetermineTargetPool), culture, arg0);

    public static string BuildResourceConnectionInputRequired() => TaskResources.Get(nameof (BuildResourceConnectionInputRequired));

    public static string BuildResourceConnectionInputRequired(CultureInfo culture) => TaskResources.Get(nameof (BuildResourceConnectionInputRequired), culture);

    public static string BuildResourceSourceInputRequired() => TaskResources.Get(nameof (BuildResourceSourceInputRequired));

    public static string BuildResourceSourceInputRequired(CultureInfo culture) => TaskResources.Get(nameof (BuildResourceSourceInputRequired), culture);

    public static string BuildResourceTypeRequired() => TaskResources.Get(nameof (BuildResourceTypeRequired));

    public static string BuildResourceTypeRequired(CultureInfo culture) => TaskResources.Get(nameof (BuildResourceTypeRequired), culture);

    public static string CannotFindBuildResourceExtension(object arg0) => TaskResources.Format(nameof (CannotFindBuildResourceExtension), arg0);

    public static string CannotFindBuildResourceExtension(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotFindBuildResourceExtension), culture, arg0);

    public static string CannotUseNonBuildTypeArtifact(object arg0) => TaskResources.Format(nameof (CannotUseNonBuildTypeArtifact), arg0);

    public static string CannotUseNonBuildTypeArtifact(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotUseNonBuildTypeArtifact), culture, arg0);

    public static string SystemPipelineDecorators() => TaskResources.Get(nameof (SystemPipelineDecorators));

    public static string SystemPipelineDecorators(CultureInfo culture) => TaskResources.Get(nameof (SystemPipelineDecorators), culture);

    public static string BuildResourceValidConnectionInputRequired(object arg0) => TaskResources.Format(nameof (BuildResourceValidConnectionInputRequired), arg0);

    public static string BuildResourceValidConnectionInputRequired(object arg0, CultureInfo culture) => TaskResources.Format(nameof (BuildResourceValidConnectionInputRequired), culture, arg0);

    public static string BuildResourceContainsInvalidInput(object arg0, object arg1) => TaskResources.Format(nameof (BuildResourceContainsInvalidInput), arg0, arg1);

    public static string BuildResourceContainsInvalidInput(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (BuildResourceContainsInvalidInput), culture, arg0, arg1);
    }

    public static string CannotFindBuildResource(object arg0) => TaskResources.Format(nameof (CannotFindBuildResource), arg0);

    public static string CannotFindBuildResource(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotFindBuildResource), culture, arg0);

    public static string CannotFindTaskReference(object arg0) => TaskResources.Format(nameof (CannotFindTaskReference), arg0);

    public static string CannotFindTaskReference(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotFindTaskReference), culture, arg0);

    public static string AmbiguousBuildDefinitionsFound(object arg0, object arg1) => TaskResources.Format(nameof (AmbiguousBuildDefinitionsFound), arg0, arg1);

    public static string AmbiguousBuildDefinitionsFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (AmbiguousBuildDefinitionsFound), culture, arg0, arg1);
    }

    public static string BuildDefinitionNotFound(object arg0, object arg1) => TaskResources.Format(nameof (BuildDefinitionNotFound), arg0, arg1);

    public static string BuildDefinitionNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (BuildDefinitionNotFound), culture, arg0, arg1);

    public static string HttpRequestExpressionNotValid(object arg0, object arg1) => TaskResources.Format(nameof (HttpRequestExpressionNotValid), arg0, arg1);

    public static string HttpRequestExpressionNotValid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (HttpRequestExpressionNotValid), culture, arg0, arg1);
    }

    public static string PipelineResourceInvalidConnectionInput(object arg0) => TaskResources.Format(nameof (PipelineResourceInvalidConnectionInput), arg0);

    public static string PipelineResourceInvalidConnectionInput(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PipelineResourceInvalidConnectionInput), culture, arg0);

    public static string InvalidTypeOnlyStringTypeSupported(object arg0) => TaskResources.Format(nameof (InvalidTypeOnlyStringTypeSupported), arg0);

    public static string InvalidTypeOnlyStringTypeSupported(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidTypeOnlyStringTypeSupported), culture, arg0);

    public static string CannotFindWebHookExtension() => TaskResources.Get(nameof (CannotFindWebHookExtension));

    public static string CannotFindWebHookExtension(CultureInfo culture) => TaskResources.Get(nameof (CannotFindWebHookExtension), culture);

    public static string FailedToGetACRLocation(object arg0, object arg1) => TaskResources.Format(nameof (FailedToGetACRLocation), arg0, arg1);

    public static string FailedToGetACRLocation(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (FailedToGetACRLocation), culture, arg0, arg1);

    public static string KeyNotFoundForACR(object arg0, object arg1) => TaskResources.Format(nameof (KeyNotFoundForACR), arg0, arg1);

    public static string KeyNotFoundForACR(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (KeyNotFoundForACR), culture, arg0, arg1);

    public static string AmbigousServiceEndpointUsed(object arg0) => TaskResources.Format(nameof (AmbigousServiceEndpointUsed), arg0);

    public static string AmbigousServiceEndpointUsed(object arg0, CultureInfo culture) => TaskResources.Format(nameof (AmbigousServiceEndpointUsed), culture, arg0);

    public static string EndpointAccessDeniedForUseOperation() => TaskResources.Get(nameof (EndpointAccessDeniedForUseOperation));

    public static string EndpointAccessDeniedForUseOperation(CultureInfo culture) => TaskResources.Get(nameof (EndpointAccessDeniedForUseOperation), culture);

    public static string FailedToGetDockerTagAndDigest(object arg0, object arg1) => TaskResources.Format(nameof (FailedToGetDockerTagAndDigest), arg0, arg1);

    public static string FailedToGetDockerTagAndDigest(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (FailedToGetDockerTagAndDigest), culture, arg0, arg1);
    }

    public static string KeyNotFoundForDocker(object arg0, object arg1) => TaskResources.Format(nameof (KeyNotFoundForDocker), arg0, arg1);

    public static string KeyNotFoundForDocker(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (KeyNotFoundForDocker), culture, arg0, arg1);

    public static string ServiceExtensionNotFound(object arg0) => TaskResources.Format(nameof (ServiceExtensionNotFound), arg0);

    public static string ServiceExtensionNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ServiceExtensionNotFound), culture, arg0);

    public static string StageFilterIsNotSupported(object arg0) => TaskResources.Format(nameof (StageFilterIsNotSupported), arg0);

    public static string StageFilterIsNotSupported(object arg0, CultureInfo culture) => TaskResources.Format(nameof (StageFilterIsNotSupported), culture, arg0);

    public static string TagFilterIsNotSupported(object arg0) => TaskResources.Format(nameof (TagFilterIsNotSupported), arg0);

    public static string TagFilterIsNotSupported(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TagFilterIsNotSupported), culture, arg0);

    public static string CannotFetchArtifactTypeDefinition(object arg0) => TaskResources.Format(nameof (CannotFetchArtifactTypeDefinition), arg0);

    public static string CannotFetchArtifactTypeDefinition(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotFetchArtifactTypeDefinition), culture, arg0);

    public static string CustomArtifactTriggerFeatureNotEnabled() => TaskResources.Get(nameof (CustomArtifactTriggerFeatureNotEnabled));

    public static string CustomArtifactTriggerFeatureNotEnabled(CultureInfo culture) => TaskResources.Get(nameof (CustomArtifactTriggerFeatureNotEnabled), culture);

    public static string InvalidWebHookId(object arg0) => TaskResources.Format(nameof (InvalidWebHookId), arg0);

    public static string InvalidWebHookId(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidWebHookId), culture, arg0);

    public static string WebHookIdNotFound(object arg0) => TaskResources.Format(nameof (WebHookIdNotFound), arg0);

    public static string WebHookIdNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (WebHookIdNotFound), culture, arg0);

    public static string WebHookPayloadHashDoesNotMatch(object arg0) => TaskResources.Format(nameof (WebHookPayloadHashDoesNotMatch), arg0);

    public static string WebHookPayloadHashDoesNotMatch(object arg0, CultureInfo culture) => TaskResources.Format(nameof (WebHookPayloadHashDoesNotMatch), culture, arg0);

    public static string EndpointOfTypeNotFound(object arg0, object arg1) => TaskResources.Format(nameof (EndpointOfTypeNotFound), arg0, arg1);

    public static string EndpointOfTypeNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (EndpointOfTypeNotFound), culture, arg0, arg1);

    public static string DockerHubEndpointNotFound(object arg0, object arg1) => TaskResources.Format(nameof (DockerHubEndpointNotFound), arg0, arg1);

    public static string DockerHubEndpointNotFound(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (DockerHubEndpointNotFound), culture, arg0, arg1);

    public static string FailedToGetACRTagAndDigest(object arg0, object arg1) => TaskResources.Format(nameof (FailedToGetACRTagAndDigest), arg0, arg1);

    public static string FailedToGetACRTagAndDigest(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (FailedToGetACRTagAndDigest), culture, arg0, arg1);

    public static string PhaseProviderExtensionNotFound(object arg0) => TaskResources.Format(nameof (PhaseProviderExtensionNotFound), arg0);

    public static string PhaseProviderExtensionNotFound(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PhaseProviderExtensionNotFound), culture, arg0);

    public static string EndpointMissingInfo(object arg0) => TaskResources.Format(nameof (EndpointMissingInfo), arg0);

    public static string EndpointMissingInfo(object arg0, CultureInfo culture) => TaskResources.Format(nameof (EndpointMissingInfo), culture, arg0);

    public static string PipelineResourceMustBeValid(object arg0) => TaskResources.Format(nameof (PipelineResourceMustBeValid), arg0);

    public static string PipelineResourceMustBeValid(object arg0, CultureInfo culture) => TaskResources.Format(nameof (PipelineResourceMustBeValid), culture, arg0);

    public static string PackageResourceProjectInputRequired() => TaskResources.Get(nameof (PackageResourceProjectInputRequired));

    public static string PackageResourceProjectInputRequired(CultureInfo culture) => TaskResources.Get(nameof (PackageResourceProjectInputRequired), culture);

    public static string PackageResourceTypeInputRequired() => TaskResources.Get(nameof (PackageResourceTypeInputRequired));

    public static string PackageResourceTypeInputRequired(CultureInfo culture) => TaskResources.Get(nameof (PackageResourceTypeInputRequired), culture);

    public static string PackageResourceNameInputRequired() => TaskResources.Get(nameof (PackageResourceNameInputRequired));

    public static string PackageResourceNameInputRequired(CultureInfo culture) => TaskResources.Get(nameof (PackageResourceNameInputRequired), culture);

    public static string PackageResourceConnectionInputRequired() => TaskResources.Get(nameof (PackageResourceConnectionInputRequired));

    public static string PackageResourceConnectionInputRequired(CultureInfo culture) => TaskResources.Get(nameof (PackageResourceConnectionInputRequired), culture);

    public static string GitHubPATSupportedForPackageResources() => TaskResources.Get(nameof (GitHubPATSupportedForPackageResources));

    public static string GitHubPATSupportedForPackageResources(CultureInfo culture) => TaskResources.Get(nameof (GitHubPATSupportedForPackageResources), culture);

    public static string CannotFindPackageResource(object arg0) => TaskResources.Format(nameof (CannotFindPackageResource), arg0);

    public static string CannotFindPackageResource(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotFindPackageResource), culture, arg0);

    public static string OverScopedRepositoryLimit(object arg0) => TaskResources.Format(nameof (OverScopedRepositoryLimit), arg0);

    public static string OverScopedRepositoryLimit(object arg0, CultureInfo culture) => TaskResources.Format(nameof (OverScopedRepositoryLimit), culture, arg0);

    public static string WebHookResourceConnectionInputRequired() => TaskResources.Get(nameof (WebHookResourceConnectionInputRequired));

    public static string WebHookResourceConnectionInputRequired(CultureInfo culture) => TaskResources.Get(nameof (WebHookResourceConnectionInputRequired), culture);

    public static string WebHookResourceValidConnectionInputRequired(object arg0) => TaskResources.Format(nameof (WebHookResourceValidConnectionInputRequired), arg0);

    public static string WebHookResourceValidConnectionInputRequired(
      object arg0,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (WebHookResourceValidConnectionInputRequired), culture, arg0);
    }

    public static string CannotCalculateWebHookSignature() => TaskResources.Get(nameof (CannotCalculateWebHookSignature));

    public static string CannotCalculateWebHookSignature(CultureInfo culture) => TaskResources.Get(nameof (CannotCalculateWebHookSignature), culture);

    public static string CannotFindPackageResourceExtension(object arg0) => TaskResources.Format(nameof (CannotFindPackageResourceExtension), arg0);

    public static string CannotFindPackageResourceExtension(object arg0, CultureInfo culture) => TaskResources.Format(nameof (CannotFindPackageResourceExtension), culture, arg0);

    public static string ConflictingSelfRepositoryCITriggers() => TaskResources.Get(nameof (ConflictingSelfRepositoryCITriggers));

    public static string ConflictingSelfRepositoryCITriggers(CultureInfo culture) => TaskResources.Get(nameof (ConflictingSelfRepositoryCITriggers), culture);

    public static string ConflictingSelfRepositoryPRTriggers() => TaskResources.Get(nameof (ConflictingSelfRepositoryPRTriggers));

    public static string ConflictingSelfRepositoryPRTriggers(CultureInfo culture) => TaskResources.Get(nameof (ConflictingSelfRepositoryPRTriggers), culture);

    public static string PipelineResourceStageMustBeNonEmpty() => TaskResources.Get(nameof (PipelineResourceStageMustBeNonEmpty));

    public static string PipelineResourceStageMustBeNonEmpty(CultureInfo culture) => TaskResources.Get(nameof (PipelineResourceStageMustBeNonEmpty), culture);

    public static string AgentLastWordsMessage() => TaskResources.Get(nameof (AgentLastWordsMessage));

    public static string AgentLastWordsMessage(CultureInfo culture) => TaskResources.Get(nameof (AgentLastWordsMessage), culture);

    public static string SpecifyRepoAlias() => TaskResources.Get(nameof (SpecifyRepoAlias));

    public static string SpecifyRepoAlias(CultureInfo culture) => TaskResources.Get(nameof (SpecifyRepoAlias), culture);

    public static string RepoAlreadyExists(object arg0) => TaskResources.Format(nameof (RepoAlreadyExists), arg0);

    public static string RepoAlreadyExists(object arg0, CultureInfo culture) => TaskResources.Format(nameof (RepoAlreadyExists), culture, arg0);

    public static string NoRepoFoundByName(object arg0) => TaskResources.Format(nameof (NoRepoFoundByName), arg0);

    public static string NoRepoFoundByName(object arg0, CultureInfo culture) => TaskResources.Format(nameof (NoRepoFoundByName), culture, arg0);

    public static string EndpointNotFoundForRepo(object arg0, object arg1) => TaskResources.Format(nameof (EndpointNotFoundForRepo), arg0, arg1);

    public static string EndpointNotFoundForRepo(object arg0, object arg1, CultureInfo culture) => TaskResources.Format(nameof (EndpointNotFoundForRepo), culture, arg0, arg1);

    public static string NoYamlSupport(object arg0) => TaskResources.Format(nameof (NoYamlSupport), arg0);

    public static string NoYamlSupport(object arg0, CultureInfo culture) => TaskResources.Format(nameof (NoYamlSupport), culture, arg0);

    public static string Canceled() => TaskResources.Get(nameof (Canceled));

    public static string Canceled(CultureInfo culture) => TaskResources.Get(nameof (Canceled), culture);

    public static string InvalidTaskOverrideFormatWarning(object arg0) => TaskResources.Format(nameof (InvalidTaskOverrideFormatWarning), arg0);

    public static string InvalidTaskOverrideFormatWarning(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InvalidTaskOverrideFormatWarning), culture, arg0);

    public static string TaskOverrideInfo(object arg0) => TaskResources.Format(nameof (TaskOverrideInfo), arg0);

    public static string TaskOverrideInfo(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskOverrideInfo), culture, arg0);

    public static string TaskBuildConfigExceptionInfo(object arg0) => TaskResources.Format(nameof (TaskBuildConfigExceptionInfo), arg0);

    public static string TaskBuildConfigExceptionInfo(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskBuildConfigExceptionInfo), culture, arg0);

    public static string InvalidTaskBuildConfigExceptionFormatWarning(object arg0) => TaskResources.Format(nameof (InvalidTaskBuildConfigExceptionFormatWarning), arg0);

    public static string InvalidTaskBuildConfigExceptionFormatWarning(
      object arg0,
      CultureInfo culture)
    {
      return TaskResources.Format(nameof (InvalidTaskBuildConfigExceptionFormatWarning), culture, arg0);
    }

    public static string ExternalCustomerThrottling() => TaskResources.Get(nameof (ExternalCustomerThrottling));

    public static string ExternalCustomerThrottling(CultureInfo culture) => TaskResources.Get(nameof (ExternalCustomerThrottling), culture);

    public static string InternalCustomerThrottling(object arg0) => TaskResources.Format(nameof (InternalCustomerThrottling), arg0);

    public static string InternalCustomerThrottling(object arg0, CultureInfo culture) => TaskResources.Format(nameof (InternalCustomerThrottling), culture, arg0);

    public static string CreateNewPlansForbidden() => TaskResources.Get(nameof (CreateNewPlansForbidden));

    public static string CreateNewPlansForbidden(CultureInfo culture) => TaskResources.Get(nameof (CreateNewPlansForbidden), culture);

    public static string TaskEndpointInDisabledState(object arg0) => TaskResources.Format(nameof (TaskEndpointInDisabledState), arg0);

    public static string TaskEndpointInDisabledState(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TaskEndpointInDisabledState), culture, arg0);

    public static string ServiceEndpointDisabled(object arg0) => TaskResources.Format(nameof (ServiceEndpointDisabled), arg0);

    public static string ServiceEndpointDisabled(object arg0, CultureInfo culture) => TaskResources.Format(nameof (ServiceEndpointDisabled), culture, arg0);

    public static string TimelineUpdateFailure(object arg0) => TaskResources.Format(nameof (TimelineUpdateFailure), arg0);

    public static string TimelineUpdateFailure(object arg0, CultureInfo culture) => TaskResources.Format(nameof (TimelineUpdateFailure), culture, arg0);
  }
}
