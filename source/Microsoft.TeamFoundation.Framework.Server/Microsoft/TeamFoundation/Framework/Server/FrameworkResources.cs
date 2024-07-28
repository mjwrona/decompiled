// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FrameworkResources
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class FrameworkResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (FrameworkResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => FrameworkResources.s_resMgr;

    private static string Get(string resourceName) => FrameworkResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? FrameworkResources.Get(resourceName) : FrameworkResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) FrameworkResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? FrameworkResources.GetInt(resourceName) : (int) FrameworkResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) FrameworkResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? FrameworkResources.GetBool(resourceName) : (bool) FrameworkResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => FrameworkResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = FrameworkResources.Get(resourceName, culture);
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

    public static string AgeThreshold() => FrameworkResources.Get(nameof (AgeThreshold));

    public static string AgeThreshold(CultureInfo culture) => FrameworkResources.Get(nameof (AgeThreshold), culture);

    public static string AgeThresholdException(object arg0, object arg1) => FrameworkResources.Format(nameof (AgeThresholdException), arg0, arg1);

    public static string AgeThresholdException(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (AgeThresholdException), culture, arg0, arg1);

    public static string AnalysisServiceConnectionException() => FrameworkResources.Get(nameof (AnalysisServiceConnectionException));

    public static string AnalysisServiceConnectionException(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisServiceConnectionException), culture);

    public static string AnalysisServicesAssemblyFailedToLoad() => FrameworkResources.Get(nameof (AnalysisServicesAssemblyFailedToLoad));

    public static string AnalysisServicesAssemblyFailedToLoad(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisServicesAssemblyFailedToLoad), culture);

    public static string AnalysisConnectionValidatorInvalidConfiguration() => FrameworkResources.Get(nameof (AnalysisConnectionValidatorInvalidConfiguration));

    public static string AnalysisConnectionValidatorInvalidConfiguration(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisConnectionValidatorInvalidConfiguration), culture);

    public static string AnalysisConnectionValidatorMultipleDatabasesFound() => FrameworkResources.Get(nameof (AnalysisConnectionValidatorMultipleDatabasesFound));

    public static string AnalysisConnectionValidatorMultipleDatabasesFound(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisConnectionValidatorMultipleDatabasesFound), culture);

    public static string AnalysisConnectionValidatorNoDatabaseFound() => FrameworkResources.Get(nameof (AnalysisConnectionValidatorNoDatabaseFound));

    public static string AnalysisConnectionValidatorNoDatabaseFound(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisConnectionValidatorNoDatabaseFound), culture);

    public static string AnalysisConnectionValidatorNoTfsDataSourceFound() => FrameworkResources.Get(nameof (AnalysisConnectionValidatorNoTfsDataSourceFound));

    public static string AnalysisConnectionValidatorNoTfsDataSourceFound(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisConnectionValidatorNoTfsDataSourceFound), culture);

    public static string AnalysisDataSourceConnectionCannotBeUpdated() => FrameworkResources.Get(nameof (AnalysisDataSourceConnectionCannotBeUpdated));

    public static string AnalysisDataSourceConnectionCannotBeUpdated(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisDataSourceConnectionCannotBeUpdated), culture);

    public static string AnalysisConnectionValidatorFix() => FrameworkResources.Get(nameof (AnalysisConnectionValidatorFix));

    public static string AnalysisConnectionValidatorFix(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisConnectionValidatorFix), culture);

    public static string AnalysisConnectionValidatorDataSourceFix() => FrameworkResources.Get(nameof (AnalysisConnectionValidatorDataSourceFix));

    public static string AnalysisConnectionValidatorDataSourceFix(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisConnectionValidatorDataSourceFix), culture);

    public static string ApplicationRequestEndMessage() => FrameworkResources.Get(nameof (ApplicationRequestEndMessage));

    public static string ApplicationRequestEndMessage(CultureInfo culture) => FrameworkResources.Get(nameof (ApplicationRequestEndMessage), culture);

    public static string ApplicationRequestStartMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (ApplicationRequestStartMessage), arg0, arg1);

    public static string ApplicationRequestStartMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ApplicationRequestStartMessage), culture, arg0, arg1);
    }

    public static string ApplicationShutdownOnException() => FrameworkResources.Get(nameof (ApplicationShutdownOnException));

    public static string ApplicationShutdownOnException(CultureInfo culture) => FrameworkResources.Get(nameof (ApplicationShutdownOnException), culture);

    public static string AuthenticationTypeNotSupported(object arg0) => FrameworkResources.Format(nameof (AuthenticationTypeNotSupported), arg0);

    public static string AuthenticationTypeNotSupported(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AuthenticationTypeNotSupported), culture, arg0);

    public static string BasicAuthenticationPasswordInvalid() => FrameworkResources.Get(nameof (BasicAuthenticationPasswordInvalid));

    public static string BasicAuthenticationPasswordInvalid(CultureInfo culture) => FrameworkResources.Get(nameof (BasicAuthenticationPasswordInvalid), culture);

    public static string CommandStopped(object arg0) => FrameworkResources.Format(nameof (CommandStopped), arg0);

    public static string CommandStopped(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CommandStopped), culture, arg0);

    public static string DatabaseConfigurationException() => FrameworkResources.Get(nameof (DatabaseConfigurationException));

    public static string DatabaseConfigurationException(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseConfigurationException), culture);

    public static string DatabaseConnectionException() => FrameworkResources.Get(nameof (DatabaseConnectionException));

    public static string DatabaseConnectionException(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseConnectionException), culture);

    public static string DatabaseFullException() => FrameworkResources.Get(nameof (DatabaseFullException));

    public static string DatabaseFullException(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseFullException), culture);

    public static string DatabaseOperationCanceledException() => FrameworkResources.Get(nameof (DatabaseOperationCanceledException));

    public static string DatabaseOperationCanceledException(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseOperationCanceledException), culture);

    public static string DatabaseOperationTimeoutException() => FrameworkResources.Get(nameof (DatabaseOperationTimeoutException));

    public static string DatabaseOperationTimeoutException(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseOperationTimeoutException), culture);

    public static string DatabaseVersionMismatch(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (DatabaseVersionMismatch), arg0, arg1, arg2);

    public static string DatabaseVersionMismatch(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DatabaseVersionMismatch), culture, arg0, arg1, arg2);
    }

    public static string DatabaseVersionMissing(object arg0) => FrameworkResources.Format(nameof (DatabaseVersionMissing), arg0);

    public static string DatabaseVersionMissing(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseVersionMissing), culture, arg0);

    public static string DateTimeShiftDetected() => FrameworkResources.Get(nameof (DateTimeShiftDetected));

    public static string DateTimeShiftDetected(CultureInfo culture) => FrameworkResources.Get(nameof (DateTimeShiftDetected), culture);

    public static string ErrorWhileProcessingResults() => FrameworkResources.Get(nameof (ErrorWhileProcessingResults));

    public static string ErrorWhileProcessingResults(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorWhileProcessingResults), culture);

    public static string EventConditionUnexpectedEndOfFile() => FrameworkResources.Get(nameof (EventConditionUnexpectedEndOfFile));

    public static string EventConditionUnexpectedEndOfFile(CultureInfo culture) => FrameworkResources.Get(nameof (EventConditionUnexpectedEndOfFile), culture);

    public static string EventConditionUnexpectedEndOfLine() => FrameworkResources.Get(nameof (EventConditionUnexpectedEndOfLine));

    public static string EventConditionUnexpectedEndOfLine(CultureInfo culture) => FrameworkResources.Get(nameof (EventConditionUnexpectedEndOfLine), culture);

    public static string EventSourceMissingException() => FrameworkResources.Get(nameof (EventSourceMissingException));

    public static string EventSourceMissingException(CultureInfo culture) => FrameworkResources.Get(nameof (EventSourceMissingException), culture);

    public static string GenericClientErrorMessage(object arg0) => FrameworkResources.Format(nameof (GenericClientErrorMessage), arg0);

    public static string GenericClientErrorMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (GenericClientErrorMessage), culture, arg0);

    public static string InstalledUICultureUnavailable() => FrameworkResources.Get(nameof (InstalledUICultureUnavailable));

    public static string InstalledUICultureUnavailable(CultureInfo culture) => FrameworkResources.Get(nameof (InstalledUICultureUnavailable), culture);

    public static string InstanceEmptyError() => FrameworkResources.Get(nameof (InstanceEmptyError));

    public static string InstanceEmptyError(CultureInfo culture) => FrameworkResources.Get(nameof (InstanceEmptyError), culture);

    public static string InvalidRequestId(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidRequestId), arg0, arg1);

    public static string InvalidRequestId(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidRequestId), culture, arg0, arg1);

    public static string HostAlreadyExistsException(object arg0) => FrameworkResources.Format(nameof (HostAlreadyExistsException), arg0);

    public static string HostAlreadyExistsException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostAlreadyExistsException), culture, arg0);

    public static string HostDoesNotExistException(object arg0) => FrameworkResources.Format(nameof (HostDoesNotExistException), arg0);

    public static string HostDoesNotExistException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostDoesNotExistException), culture, arg0);

    public static string HostMustBeTopLevelException(object arg0) => FrameworkResources.Format(nameof (HostMustBeTopLevelException), arg0);

    public static string HostMustBeTopLevelException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostMustBeTopLevelException), culture, arg0);

    public static string InstanceMismatchError(object arg0) => FrameworkResources.Format(nameof (InstanceMismatchError), arg0);

    public static string InstanceMismatchError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InstanceMismatchError), culture, arg0);

    public static string InvalidLicenseException() => FrameworkResources.Get(nameof (InvalidLicenseException));

    public static string InvalidLicenseException(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidLicenseException), culture);

    public static string MaxItemsExceeded(object arg0) => FrameworkResources.Format(nameof (MaxItemsExceeded), arg0);

    public static string MaxItemsExceeded(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MaxItemsExceeded), culture, arg0);

    public static string MessageBrief() => FrameworkResources.Get(nameof (MessageBrief));

    public static string MessageBrief(CultureInfo culture) => FrameworkResources.Get(nameof (MessageBrief), culture);

    public static string NoHostAvailableForRequest() => FrameworkResources.Get(nameof (NoHostAvailableForRequest));

    public static string NoHostAvailableForRequest(CultureInfo culture) => FrameworkResources.Get(nameof (NoHostAvailableForRequest), culture);

    public static string NotAvailable() => FrameworkResources.Get(nameof (NotAvailable));

    public static string NotAvailable(CultureInfo culture) => FrameworkResources.Get(nameof (NotAvailable), culture);

    public static string NullStoredProcException() => FrameworkResources.Get(nameof (NullStoredProcException));

    public static string NullStoredProcException(CultureInfo culture) => FrameworkResources.Get(nameof (NullStoredProcException), culture);

    public static string OnlyOneConfigVariable() => FrameworkResources.Get(nameof (OnlyOneConfigVariable));

    public static string OnlyOneConfigVariable(CultureInfo culture) => FrameworkResources.Get(nameof (OnlyOneConfigVariable), culture);

    public static string PerfCounterInitializationException(object arg0, object arg1) => FrameworkResources.Format(nameof (PerfCounterInitializationException), arg0, arg1);

    public static string PerfCounterInitializationException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (PerfCounterInitializationException), culture, arg0, arg1);
    }

    public static string ProjectCollectionMissingId() => FrameworkResources.Get(nameof (ProjectCollectionMissingId));

    public static string ProjectCollectionMissingId(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionMissingId), culture);

    public static string RequestImpersonationNotSupported() => FrameworkResources.Get(nameof (RequestImpersonationNotSupported));

    public static string RequestImpersonationNotSupported(CultureInfo culture) => FrameworkResources.Get(nameof (RequestImpersonationNotSupported), culture);

    public static string RequiredSignatureMissing() => FrameworkResources.Get(nameof (RequiredSignatureMissing));

    public static string RequiredSignatureMissing(CultureInfo culture) => FrameworkResources.Get(nameof (RequiredSignatureMissing), culture);

    public static string SchemaEmptyError(object arg0, object arg1) => FrameworkResources.Format(nameof (SchemaEmptyError), arg0, arg1);

    public static string SchemaEmptyError(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (SchemaEmptyError), culture, arg0, arg1);

    public static string SchemaMismatchError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (SchemaMismatchError), arg0, arg1, arg2);

    public static string SchemaMismatchError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SchemaMismatchError), culture, arg0, arg1, arg2);
    }

    public static string ServiceLevelEmptyError(object arg0, object arg1) => FrameworkResources.Format(nameof (ServiceLevelEmptyError), arg0, arg1);

    public static string ServiceLevelEmptyError(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceLevelEmptyError), culture, arg0, arg1);

    public static string ServiceLevelMismatchError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (ServiceLevelMismatchError), arg0, arg1, arg2);

    public static string ServiceLevelMismatchError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServiceLevelMismatchError), culture, arg0, arg1, arg2);
    }

    public static string ServerFatalInitError() => FrameworkResources.Get(nameof (ServerFatalInitError));

    public static string ServerFatalInitError(CultureInfo culture) => FrameworkResources.Get(nameof (ServerFatalInitError), culture);

    public static string ServiceCannotBeCreatedDuringShutdown(object arg0) => FrameworkResources.Format(nameof (ServiceCannotBeCreatedDuringShutdown), arg0);

    public static string ServiceCannotBeCreatedDuringShutdown(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceCannotBeCreatedDuringShutdown), culture, arg0);

    public static string ServiceRequiresApplicationHost() => FrameworkResources.Get(nameof (ServiceRequiresApplicationHost));

    public static string ServiceRequiresApplicationHost(CultureInfo culture) => FrameworkResources.Get(nameof (ServiceRequiresApplicationHost), culture);

    public static string ServiceRequiresSiteHost() => FrameworkResources.Get(nameof (ServiceRequiresSiteHost));

    public static string ServiceRequiresSiteHost(CultureInfo culture) => FrameworkResources.Get(nameof (ServiceRequiresSiteHost), culture);

    public static string ShutdownReason(object arg0) => FrameworkResources.Format(nameof (ShutdownReason), arg0);

    public static string ShutdownReason(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ShutdownReason), culture, arg0);

    public static string CollectionIsNotProcessingRequests(object arg0) => FrameworkResources.Format(nameof (CollectionIsNotProcessingRequests), arg0);

    public static string CollectionIsNotProcessingRequests(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionIsNotProcessingRequests), culture, arg0);

    public static string HostOfflineForDataImport() => FrameworkResources.Get(nameof (HostOfflineForDataImport));

    public static string HostOfflineForDataImport(CultureInfo culture) => FrameworkResources.Get(nameof (HostOfflineForDataImport), culture);

    public static string HostOfflineForDataImportWithId(object arg0) => FrameworkResources.Format(nameof (HostOfflineForDataImportWithId), arg0);

    public static string HostOfflineForDataImportWithId(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostOfflineForDataImportWithId), culture, arg0);

    public static string HostOfflineForReparentCollection(object arg0) => FrameworkResources.Format(nameof (HostOfflineForReparentCollection), arg0);

    public static string HostOfflineForReparentCollection(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostOfflineForReparentCollection), culture, arg0);

    public static string SqlException(object arg0) => FrameworkResources.Format(nameof (SqlException), arg0);

    public static string SqlException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SqlException), culture, arg0);

    public static string MonitorServiceHostMessageBrief() => FrameworkResources.Get(nameof (MonitorServiceHostMessageBrief));

    public static string MonitorServiceHostMessageBrief(CultureInfo culture) => FrameworkResources.Get(nameof (MonitorServiceHostMessageBrief), culture);

    public static string QueuedRequestThresholdExceeded(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (QueuedRequestThresholdExceeded), arg0, arg1, arg2);

    public static string QueuedRequestThresholdExceeded(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (QueuedRequestThresholdExceeded), culture, arg0, arg1, arg2);
    }

    public static string QuotaExceededMessage(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (QuotaExceededMessage), arg0, arg1, arg2);

    public static string QuotaExceededMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (QuotaExceededMessage), culture, arg0, arg1, arg2);
    }

    public static string QueuedRequestThresholdCleared(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (QueuedRequestThresholdCleared), arg0, arg1, arg2);

    public static string QueuedRequestThresholdCleared(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (QueuedRequestThresholdCleared), culture, arg0, arg1, arg2);
    }

    public static string QueuedRequestElapsedThresholdExceeded(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (QueuedRequestElapsedThresholdExceeded), arg0, arg1, arg2, arg3);
    }

    public static string QueuedRequestElapsedThresholdExceeded(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (QueuedRequestElapsedThresholdExceeded), culture, arg0, arg1, arg2, arg3);
    }

    public static string QueuedRequestElapsedThresholdCleared(object arg0, object arg1) => FrameworkResources.Format(nameof (QueuedRequestElapsedThresholdCleared), arg0, arg1);

    public static string QueuedRequestElapsedThresholdCleared(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (QueuedRequestElapsedThresholdCleared), culture, arg0, arg1);
    }

    public static string TotalExecutionElapsedThresholdExceeded(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (TotalExecutionElapsedThresholdExceeded), arg0, arg1, arg2, arg3);
    }

    public static string TotalExecutionElapsedThresholdExceeded(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (TotalExecutionElapsedThresholdExceeded), culture, arg0, arg1, arg2, arg3);
    }

    public static string TotalExecutionElapsedThresholdCleared(object arg0, object arg1) => FrameworkResources.Format(nameof (TotalExecutionElapsedThresholdCleared), arg0, arg1);

    public static string TotalExecutionElapsedThresholdCleared(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (TotalExecutionElapsedThresholdCleared), culture, arg0, arg1);
    }

    public static string RequestContextDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7)
    {
      return FrameworkResources.Format(nameof (RequestContextDetails), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string RequestContextDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (RequestContextDetails), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string SubscriberDisabledMessage(object arg0) => FrameworkResources.Format(nameof (SubscriberDisabledMessage), arg0);

    public static string SubscriberDisabledMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SubscriberDisabledMessage), culture, arg0);

    public static string SubscriberExceptionMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (SubscriberExceptionMessage), arg0, arg1);

    public static string SubscriberExceptionMessage(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (SubscriberExceptionMessage), culture, arg0, arg1);

    public static string TeamFoundationApplicationRequired() => FrameworkResources.Get(nameof (TeamFoundationApplicationRequired));

    public static string TeamFoundationApplicationRequired(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationApplicationRequired), culture);

    public static string TeamFoundationUnavilable() => FrameworkResources.Get(nameof (TeamFoundationUnavilable));

    public static string TeamFoundationUnavilable(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationUnavilable), culture);

    public static string Error_TeamFoundationUnavailable() => FrameworkResources.Get(nameof (Error_TeamFoundationUnavailable));

    public static string Error_TeamFoundationUnavailable(CultureInfo culture) => FrameworkResources.Get(nameof (Error_TeamFoundationUnavailable), culture);

    public static string Error_TeamFoundationUnavailableWithCorrelation(object arg0) => FrameworkResources.Format(nameof (Error_TeamFoundationUnavailableWithCorrelation), arg0);

    public static string Error_TeamFoundationUnavailableWithCorrelation(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (Error_TeamFoundationUnavailableWithCorrelation), culture, arg0);
    }

    public static string UnauthorizedWorkgroupUser(object arg0) => FrameworkResources.Format(nameof (UnauthorizedWorkgroupUser), arg0);

    public static string UnauthorizedWorkgroupUser(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnauthorizedWorkgroupUser), culture, arg0);

    public static string UnexpectedDatabaseResultException(object arg0) => FrameworkResources.Format(nameof (UnexpectedDatabaseResultException), arg0);

    public static string UnexpectedDatabaseResultException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnexpectedDatabaseResultException), culture, arg0);

    public static string UnhandledExceptionError() => FrameworkResources.Get(nameof (UnhandledExceptionError));

    public static string UnhandledExceptionError(CultureInfo culture) => FrameworkResources.Get(nameof (UnhandledExceptionError), culture);

    public static string UnhandledExceptionErrorWithMessage(object arg0) => FrameworkResources.Format(nameof (UnhandledExceptionErrorWithMessage), arg0);

    public static string UnhandledExceptionErrorWithMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnhandledExceptionErrorWithMessage), culture, arg0);

    public static string WebRequestDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8)
    {
      return FrameworkResources.Format(nameof (WebRequestDetails), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    public static string WebRequestDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (WebRequestDetails), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    public static string TrialExpirationImminent(object arg0) => FrameworkResources.Format(nameof (TrialExpirationImminent), arg0);

    public static string TrialExpirationImminent(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (TrialExpirationImminent), culture, arg0);

    public static string BetaExpirationImminent(object arg0) => FrameworkResources.Format(nameof (BetaExpirationImminent), arg0);

    public static string BetaExpirationImminent(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (BetaExpirationImminent), culture, arg0);

    public static string TrialExpired(object arg0) => FrameworkResources.Format(nameof (TrialExpired), arg0);

    public static string TrialExpired(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (TrialExpired), culture, arg0);

    public static string BetaExpired(object arg0) => FrameworkResources.Format(nameof (BetaExpired), arg0);

    public static string BetaExpired(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (BetaExpired), culture, arg0);

    public static string CouldNotCastParameterToT(object arg0) => FrameworkResources.Format(nameof (CouldNotCastParameterToT), arg0);

    public static string CouldNotCastParameterToT(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CouldNotCastParameterToT), culture, arg0);

    public static string DatabaseMissingErrorMessages(object arg0) => FrameworkResources.Format(nameof (DatabaseMissingErrorMessages), arg0);

    public static string DatabaseMissingErrorMessages(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseMissingErrorMessages), culture, arg0);

    public static string InvalidSecurityServiceModificationFormat(object arg0) => FrameworkResources.Format(nameof (InvalidSecurityServiceModificationFormat), arg0);

    public static string InvalidSecurityServiceModificationFormat(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidSecurityServiceModificationFormat), culture, arg0);

    public static string InformationalEventFormatNoHost(object arg0, object arg1) => FrameworkResources.Format(nameof (InformationalEventFormatNoHost), arg0, arg1);

    public static string InformationalEventFormatNoHost(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InformationalEventFormatNoHost), culture, arg0, arg1);
    }

    public static string InformationalEventFormat(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (InformationalEventFormat), arg0, arg1, arg2);

    public static string InformationalEventFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InformationalEventFormat), culture, arg0, arg1, arg2);
    }

    public static string CollectionRequiresRelativePath(object arg0) => FrameworkResources.Format(nameof (CollectionRequiresRelativePath), arg0);

    public static string CollectionRequiresRelativePath(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionRequiresRelativePath), culture, arg0);

    public static string CollectionRequiresSubDirectory(object arg0) => FrameworkResources.Format(nameof (CollectionRequiresSubDirectory), arg0);

    public static string CollectionRequiresSubDirectory(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionRequiresSubDirectory), culture, arg0);

    public static string CollectionSubDirectoryReserved(object arg0, object arg1) => FrameworkResources.Format(nameof (CollectionSubDirectoryReserved), arg0, arg1);

    public static string CollectionSubDirectoryReserved(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CollectionSubDirectoryReserved), culture, arg0, arg1);
    }

    public static string RegistryBadPatternMatch() => FrameworkResources.Get(nameof (RegistryBadPatternMatch));

    public static string RegistryBadPatternMatch(CultureInfo culture) => FrameworkResources.Get(nameof (RegistryBadPatternMatch), culture);

    public static string RegistryNameReserved(object arg0) => FrameworkResources.Format(nameof (RegistryNameReserved), arg0);

    public static string RegistryNameReserved(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RegistryNameReserved), culture, arg0);

    public static string RegistryPathBadEnding() => FrameworkResources.Get(nameof (RegistryPathBadEnding));

    public static string RegistryPathBadEnding(CultureInfo culture) => FrameworkResources.Get(nameof (RegistryPathBadEnding), culture);

    public static string RegistryPathEmptySegment() => FrameworkResources.Get(nameof (RegistryPathEmptySegment));

    public static string RegistryPathEmptySegment(CultureInfo culture) => FrameworkResources.Get(nameof (RegistryPathEmptySegment), culture);

    public static string RegistryPathInvalidSegment(object arg0) => FrameworkResources.Format(nameof (RegistryPathInvalidSegment), arg0);

    public static string RegistryPathInvalidSegment(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RegistryPathInvalidSegment), culture, arg0);

    public static string RegistryPathNoLiterals() => FrameworkResources.Get(nameof (RegistryPathNoLiterals));

    public static string RegistryPathNoLiterals(CultureInfo culture) => FrameworkResources.Get(nameof (RegistryPathNoLiterals), culture);

    public static string RegistryPathRequired() => FrameworkResources.Get(nameof (RegistryPathRequired));

    public static string RegistryPathRequired(CultureInfo culture) => FrameworkResources.Get(nameof (RegistryPathRequired), culture);

    public static string RegistryPathReserved() => FrameworkResources.Get(nameof (RegistryPathReserved));

    public static string RegistryPathReserved(CultureInfo culture) => FrameworkResources.Get(nameof (RegistryPathReserved), culture);

    public static string ServiceInitFailed(object arg0) => FrameworkResources.Format(nameof (ServiceInitFailed), arg0);

    public static string ServiceInitFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceInitFailed), culture, arg0);

    public static string RegistryUninitializedForPartition(object arg0) => FrameworkResources.Format(nameof (RegistryUninitializedForPartition), arg0);

    public static string RegistryUninitializedForPartition(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RegistryUninitializedForPartition), culture, arg0);

    public static string StopImmediateReceived() => FrameworkResources.Get(nameof (StopImmediateReceived));

    public static string StopImmediateReceived(CultureInfo culture) => FrameworkResources.Get(nameof (StopImmediateReceived), culture);

    public static string StopNonImmediateReceived() => FrameworkResources.Get(nameof (StopNonImmediateReceived));

    public static string StopNonImmediateReceived(CultureInfo culture) => FrameworkResources.Get(nameof (StopNonImmediateReceived), culture);

    public static string InvalidConditionSyntax() => FrameworkResources.Get(nameof (InvalidConditionSyntax));

    public static string InvalidConditionSyntax(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidConditionSyntax), culture);

    public static string UnknownRequestMethod() => FrameworkResources.Get(nameof (UnknownRequestMethod));

    public static string UnknownRequestMethod(CultureInfo culture) => FrameworkResources.Get(nameof (UnknownRequestMethod), culture);

    public static string InvalidServicingStepTypeException(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (InvalidServicingStepTypeException), arg0, arg1, arg2);

    public static string InvalidServicingStepTypeException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidServicingStepTypeException), culture, arg0, arg1, arg2);
    }

    public static string StampNotSetException(object arg0) => FrameworkResources.Format(nameof (StampNotSetException), arg0);

    public static string StampNotSetException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (StampNotSetException), culture, arg0);

    public static string ConnectionStringNotFound(object arg0) => FrameworkResources.Format(nameof (ConnectionStringNotFound), arg0);

    public static string ConnectionStringNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ConnectionStringNotFound), culture, arg0);

    public static string CollectionObjectNotFound(object arg0) => FrameworkResources.Format(nameof (CollectionObjectNotFound), arg0);

    public static string CollectionObjectNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionObjectNotFound), culture, arg0);

    public static string ServicingDeserializationError(object arg0) => FrameworkResources.Format(nameof (ServicingDeserializationError), arg0);

    public static string ServicingDeserializationError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingDeserializationError), culture, arg0);

    public static string ServicingSerializationError(object arg0) => FrameworkResources.Format(nameof (ServicingSerializationError), arg0);

    public static string ServicingSerializationError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingSerializationError), culture, arg0);

    public static string ConnectionStringUpdated(object arg0) => FrameworkResources.Format(nameof (ConnectionStringUpdated), arg0);

    public static string ConnectionStringUpdated(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ConnectionStringUpdated), culture, arg0);

    public static string ServiceDefinitionAlreadyExists(object arg0, object arg1) => FrameworkResources.Format(nameof (ServiceDefinitionAlreadyExists), arg0, arg1);

    public static string ServiceDefinitionAlreadyExists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServiceDefinitionAlreadyExists), culture, arg0, arg1);
    }

    public static string ServiceDefinitionDoesNotExist(object arg0, object arg1) => FrameworkResources.Format(nameof (ServiceDefinitionDoesNotExist), arg0, arg1);

    public static string ServiceDefinitionDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServiceDefinitionDoesNotExist), culture, arg0, arg1);
    }

    public static string InvalidServicingStepData(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (InvalidServicingStepData), arg0, arg1, arg2);

    public static string InvalidServicingStepData(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidServicingStepData), culture, arg0, arg1, arg2);
    }

    public static string ServicingStepValidationFailure(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (ServicingStepValidationFailure), arg0, arg1, arg2, arg3);
    }

    public static string ServicingStepValidationFailure(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServicingStepValidationFailure), culture, arg0, arg1, arg2, arg3);
    }

    public static string ServicingStepExecutionFailure(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (ServicingStepExecutionFailure), arg0, arg1, arg2, arg3);
    }

    public static string ServicingStepExecutionFailure(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServicingStepExecutionFailure), culture, arg0, arg1, arg2, arg3);
    }

    public static string IncompleteUploadException(object arg0) => FrameworkResources.Format(nameof (IncompleteUploadException), arg0);

    public static string IncompleteUploadException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IncompleteUploadException), culture, arg0);

    public static string ParameterFormatException(object arg0) => FrameworkResources.Format(nameof (ParameterFormatException), arg0);

    public static string ParameterFormatException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ParameterFormatException), culture, arg0);

    public static string UnsupportedContentType(object arg0) => FrameworkResources.Format(nameof (UnsupportedContentType), arg0);

    public static string UnsupportedContentType(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnsupportedContentType), culture, arg0);

    public static string BadChecksumException() => FrameworkResources.Get(nameof (BadChecksumException));

    public static string BadChecksumException(CultureInfo culture) => FrameworkResources.Get(nameof (BadChecksumException), culture);

    public static string IncompatibleCompressionFormatException() => FrameworkResources.Get(nameof (IncompatibleCompressionFormatException));

    public static string IncompatibleCompressionFormatException(CultureInfo culture) => FrameworkResources.Get(nameof (IncompatibleCompressionFormatException), culture);

    public static string IncorrectSizeException() => FrameworkResources.Get(nameof (IncorrectSizeException));

    public static string IncorrectSizeException(CultureInfo culture) => FrameworkResources.Get(nameof (IncorrectSizeException), culture);

    public static string ServicingStepPerformerNotFound(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (ServicingStepPerformerNotFound), arg0, arg1, arg2, arg3);
    }

    public static string ServicingStepPerformerNotFound(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServicingStepPerformerNotFound), culture, arg0, arg1, arg2, arg3);
    }

    public static string FileIdNotFoundException(object arg0) => FrameworkResources.Format(nameof (FileIdNotFoundException), arg0);

    public static string FileIdNotFoundException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FileIdNotFoundException), culture, arg0);

    public static string ResourceStreamNotFoundException(object arg0) => FrameworkResources.Format(nameof (ResourceStreamNotFoundException), arg0);

    public static string ResourceStreamNotFoundException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ResourceStreamNotFoundException), culture, arg0);

    public static string CollectionRequestContextNotAvailableException() => FrameworkResources.Get(nameof (CollectionRequestContextNotAvailableException));

    public static string CollectionRequestContextNotAvailableException(CultureInfo culture) => FrameworkResources.Get(nameof (CollectionRequestContextNotAvailableException), culture);

    public static string SecurityNamespaceAlreadyExists(object arg0) => FrameworkResources.Format(nameof (SecurityNamespaceAlreadyExists), arg0);

    public static string SecurityNamespaceAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SecurityNamespaceAlreadyExists), culture, arg0);

    public static string CollectionIdMustBeEmpty() => FrameworkResources.Get(nameof (CollectionIdMustBeEmpty));

    public static string CollectionIdMustBeEmpty(CultureInfo culture) => FrameworkResources.Get(nameof (CollectionIdMustBeEmpty), culture);

    public static string VirtualPathMappingException(object arg0) => FrameworkResources.Format(nameof (VirtualPathMappingException), arg0);

    public static string VirtualPathMappingException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (VirtualPathMappingException), culture, arg0);

    public static string VirtualPathsConflictException(object arg0) => FrameworkResources.Format(nameof (VirtualPathsConflictException), arg0);

    public static string VirtualPathsConflictException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (VirtualPathsConflictException), culture, arg0);

    public static string FrameworkServicingStepGroupNotFirstError() => FrameworkResources.Get(nameof (FrameworkServicingStepGroupNotFirstError));

    public static string FrameworkServicingStepGroupNotFirstError(CultureInfo culture) => FrameworkResources.Get(nameof (FrameworkServicingStepGroupNotFirstError), culture);

    public static string ServicingStepGroupNotFoundError(object arg0) => FrameworkResources.Format(nameof (ServicingStepGroupNotFoundError), arg0);

    public static string ServicingStepGroupNotFoundError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingStepGroupNotFoundError), culture, arg0);

    public static string ADDMEMBERGROUPMISSINGEXCEPTION() => FrameworkResources.Get(nameof (ADDMEMBERGROUPMISSINGEXCEPTION));

    public static string ADDMEMBERGROUPMISSINGEXCEPTION(CultureInfo culture) => FrameworkResources.Get(nameof (ADDMEMBERGROUPMISSINGEXCEPTION), culture);

    public static string IDENTITY_SYNC_ABORT() => FrameworkResources.Get(nameof (IDENTITY_SYNC_ABORT));

    public static string IDENTITY_SYNC_ABORT(CultureInfo culture) => FrameworkResources.Get(nameof (IDENTITY_SYNC_ABORT), culture);

    public static string IDENTITY_SYNC_SKIP() => FrameworkResources.Get(nameof (IDENTITY_SYNC_SKIP));

    public static string IDENTITY_SYNC_SKIP(CultureInfo culture) => FrameworkResources.Get(nameof (IDENTITY_SYNC_SKIP), culture);

    public static string IDENTITY_SYNC_END(object arg0) => FrameworkResources.Format(nameof (IDENTITY_SYNC_END), arg0);

    public static string IDENTITY_SYNC_END(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IDENTITY_SYNC_END), culture, arg0);

    public static string IDENTITY_SYNC_START(object arg0) => FrameworkResources.Format(nameof (IDENTITY_SYNC_START), arg0);

    public static string IDENTITY_SYNC_START(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IDENTITY_SYNC_START), culture, arg0);

    public static string IDENTITY_SYNC_ERROR(object arg0) => FrameworkResources.Format(nameof (IDENTITY_SYNC_ERROR), arg0);

    public static string IDENTITY_SYNC_ERROR(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IDENTITY_SYNC_ERROR), culture, arg0);

    public static string IDENTITY_SYNC_ERRORS(object arg0, object arg1) => FrameworkResources.Format(nameof (IDENTITY_SYNC_ERRORS), arg0, arg1);

    public static string IDENTITY_SYNC_ERRORS(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (IDENTITY_SYNC_ERRORS), culture, arg0, arg1);

    public static string IDENTITY_SYNC_ERRORS_TRUNCATED() => FrameworkResources.Get(nameof (IDENTITY_SYNC_ERRORS_TRUNCATED));

    public static string IDENTITY_SYNC_ERRORS_TRUNCATED(CultureInfo culture) => FrameworkResources.Get(nameof (IDENTITY_SYNC_ERRORS_TRUNCATED), culture);

    public static string NOT_A_SECURITY_GROUP_ADDED(object arg0) => FrameworkResources.Format(nameof (NOT_A_SECURITY_GROUP_ADDED), arg0);

    public static string NOT_A_SECURITY_GROUP_ADDED(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NOT_A_SECURITY_GROUP_ADDED), culture, arg0);

    public static string IdentityNotFoundMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (IdentityNotFoundMessage), arg0, arg1);

    public static string IdentityNotFoundMessage(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (IdentityNotFoundMessage), culture, arg0, arg1);

    public static string IdentityNotFoundSimpleMessage() => FrameworkResources.Get(nameof (IdentityNotFoundSimpleMessage));

    public static string IdentityNotFoundSimpleMessage(CultureInfo culture) => FrameworkResources.Get(nameof (IdentityNotFoundSimpleMessage), culture);

    public static string IdentityNotServiceIdentity() => FrameworkResources.Get(nameof (IdentityNotServiceIdentity));

    public static string IdentityNotServiceIdentity(CultureInfo culture) => FrameworkResources.Get(nameof (IdentityNotServiceIdentity), culture);

    public static string SearchFactorNotFoundMessage(object arg0) => FrameworkResources.Format(nameof (SearchFactorNotFoundMessage), arg0);

    public static string SearchFactorNotFoundMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SearchFactorNotFoundMessage), culture, arg0);

    public static string IdentityProviderNotFoundMessage(object arg0) => FrameworkResources.Format(nameof (IdentityProviderNotFoundMessage), arg0);

    public static string IdentityProviderNotFoundMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IdentityProviderNotFoundMessage), culture, arg0);

    public static string UnknownIdentity(object arg0) => FrameworkResources.Format(nameof (UnknownIdentity), arg0);

    public static string UnknownIdentity(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnknownIdentity), culture, arg0);

    public static string LocationServiceConfigurationException() => FrameworkResources.Get(nameof (LocationServiceConfigurationException));

    public static string LocationServiceConfigurationException(CultureInfo culture) => FrameworkResources.Get(nameof (LocationServiceConfigurationException), culture);

    public static string CannotSetTwoDefaultsMessage() => FrameworkResources.Get(nameof (CannotSetTwoDefaultsMessage));

    public static string CannotSetTwoDefaultsMessage(CultureInfo culture) => FrameworkResources.Get(nameof (CannotSetTwoDefaultsMessage), culture);

    public static string CatalogResourceMustBePassedWithNode() => FrameworkResources.Get(nameof (CatalogResourceMustBePassedWithNode));

    public static string CatalogResourceMustBePassedWithNode(CultureInfo culture) => FrameworkResources.Get(nameof (CatalogResourceMustBePassedWithNode), culture);

    public static string PROJECT_NOT_FOUND() => FrameworkResources.Get(nameof (PROJECT_NOT_FOUND));

    public static string PROJECT_NOT_FOUND(CultureInfo culture) => FrameworkResources.Get(nameof (PROJECT_NOT_FOUND), culture);

    public static string CatalogResourceAlreadyExists(object arg0) => FrameworkResources.Format(nameof (CatalogResourceAlreadyExists), arg0);

    public static string CatalogResourceAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogResourceAlreadyExists), culture, arg0);

    public static string InvalidArtifactId(object arg0) => FrameworkResources.Format(nameof (InvalidArtifactId), arg0);

    public static string InvalidArtifactId(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidArtifactId), culture, arg0);

    public static string InvalidArtifactVersion(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidArtifactVersion), arg0, arg1);

    public static string InvalidArtifactVersion(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidArtifactVersion), culture, arg0, arg1);

    public static string InvalidMonikerVersion(object arg0) => FrameworkResources.Format(nameof (InvalidMonikerVersion), arg0);

    public static string InvalidMonikerVersion(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidMonikerVersion), culture, arg0);

    public static string InvalidCatalogNodeMoveDelete() => FrameworkResources.Get(nameof (InvalidCatalogNodeMoveDelete));

    public static string InvalidCatalogNodeMoveDelete(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidCatalogNodeMoveDelete), culture);

    public static string InvalidDoubleCatalogNodeMove() => FrameworkResources.Get(nameof (InvalidDoubleCatalogNodeMove));

    public static string InvalidDoubleCatalogNodeMove(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidDoubleCatalogNodeMove), culture);

    public static string MustPassMovedAndParentNodeOnMove() => FrameworkResources.Get(nameof (MustPassMovedAndParentNodeOnMove));

    public static string MustPassMovedAndParentNodeOnMove(CultureInfo culture) => FrameworkResources.Get(nameof (MustPassMovedAndParentNodeOnMove), culture);

    public static string MustIncludeDependentNodeOverWebService() => FrameworkResources.Get(nameof (MustIncludeDependentNodeOverWebService));

    public static string MustIncludeDependentNodeOverWebService(CultureInfo culture) => FrameworkResources.Get(nameof (MustIncludeDependentNodeOverWebService), culture);

    public static string DefaultSQLServerInstance() => FrameworkResources.Get(nameof (DefaultSQLServerInstance));

    public static string DefaultSQLServerInstance(CultureInfo culture) => FrameworkResources.Get(nameof (DefaultSQLServerInstance), culture);

    public static string HostShutdownError() => FrameworkResources.Get(nameof (HostShutdownError));

    public static string HostShutdownError(CultureInfo culture) => FrameworkResources.Get(nameof (HostShutdownError), culture);

    public static string IllegalCatalogChildRelationship(object arg0, object arg1) => FrameworkResources.Format(nameof (IllegalCatalogChildRelationship), arg0, arg1);

    public static string IllegalCatalogChildRelationship(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (IllegalCatalogChildRelationship), culture, arg0, arg1);
    }

    public static string IllegalCatalogParentRelationship(object arg0, object arg1) => FrameworkResources.Format(nameof (IllegalCatalogParentRelationship), arg0, arg1);

    public static string IllegalCatalogParentRelationship(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (IllegalCatalogParentRelationship), culture, arg0, arg1);
    }

    public static string CatalogNodeDependencyMissing(object arg0, object arg1) => FrameworkResources.Format(nameof (CatalogNodeDependencyMissing), arg0, arg1);

    public static string CatalogNodeDependencyMissing(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CatalogNodeDependencyMissing), culture, arg0, arg1);
    }

    public static string MissingCatalogResourcePropertyMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (MissingCatalogResourcePropertyMessage), arg0, arg1);

    public static string MissingCatalogResourcePropertyMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MissingCatalogResourcePropertyMessage), culture, arg0, arg1);
    }

    public static string MissingCatalogServiceReferenceMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (MissingCatalogServiceReferenceMessage), arg0, arg1, arg2);
    }

    public static string MissingCatalogServiceReferenceMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MissingCatalogServiceReferenceMessage), culture, arg0, arg1, arg2);
    }

    public static string CatalogApplicationNodeReferenceNotFound(object arg0, object arg1) => FrameworkResources.Format(nameof (CatalogApplicationNodeReferenceNotFound), arg0, arg1);

    public static string CatalogApplicationNodeReferenceNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CatalogApplicationNodeReferenceNotFound), culture, arg0, arg1);
    }

    public static string CatalogApplicationResourceIdNotFound(object arg0) => FrameworkResources.Format(nameof (CatalogApplicationResourceIdNotFound), arg0);

    public static string CatalogApplicationResourceIdNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogApplicationResourceIdNotFound), culture, arg0);

    public static string CatalogApplicationResourceNotFound(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (CatalogApplicationResourceNotFound), arg0, arg1, arg2);

    public static string CatalogApplicationResourceNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CatalogApplicationResourceNotFound), culture, arg0, arg1, arg2);
    }

    public static string CatalogCollectionResourceIdNotFound(object arg0) => FrameworkResources.Format(nameof (CatalogCollectionResourceIdNotFound), arg0);

    public static string CatalogCollectionResourceIdNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogCollectionResourceIdNotFound), culture, arg0);

    public static string CatalogCollectionResourceNotFound(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (CatalogCollectionResourceNotFound), arg0, arg1, arg2);

    public static string CatalogCollectionResourceNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CatalogCollectionResourceNotFound), culture, arg0, arg1, arg2);
    }

    public static string CatalogCollectionNodeReferenceNotFound(object arg0, object arg1) => FrameworkResources.Format(nameof (CatalogCollectionNodeReferenceNotFound), arg0, arg1);

    public static string CatalogCollectionNodeReferenceNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CatalogCollectionNodeReferenceNotFound), culture, arg0, arg1);
    }

    public static string ExclusiveNodeReferenceFailure(object arg0, object arg1) => FrameworkResources.Format(nameof (ExclusiveNodeReferenceFailure), arg0, arg1);

    public static string ExclusiveNodeReferenceFailure(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ExclusiveNodeReferenceFailure), culture, arg0, arg1);
    }

    public static string ExclusiveResourceTypeExistenceFailure(object arg0, object arg1) => FrameworkResources.Format(nameof (ExclusiveResourceTypeExistenceFailure), arg0, arg1);

    public static string ExclusiveResourceTypeExistenceFailure(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ExclusiveResourceTypeExistenceFailure), culture, arg0, arg1);
    }

    public static string RequestCanceledError() => FrameworkResources.Get(nameof (RequestCanceledError));

    public static string RequestCanceledError(CultureInfo culture) => FrameworkResources.Get(nameof (RequestCanceledError), culture);

    public static string RequestCanceledErrorWithReason(object arg0) => FrameworkResources.Format(nameof (RequestCanceledErrorWithReason), arg0);

    public static string RequestCanceledErrorWithReason(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequestCanceledErrorWithReason), culture, arg0);

    public static string CatalogInvalidNonRecursiveDelete(object arg0) => FrameworkResources.Format(nameof (CatalogInvalidNonRecursiveDelete), arg0);

    public static string CatalogInvalidNonRecursiveDelete(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogInvalidNonRecursiveDelete), culture, arg0);

    public static string CatalogInvalidRecursiveDelete(object arg0) => FrameworkResources.Format(nameof (CatalogInvalidRecursiveDelete), arg0);

    public static string CatalogInvalidRecursiveDelete(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogInvalidRecursiveDelete), culture, arg0);

    public static string CatalogInvalidSelfDependency() => FrameworkResources.Get(nameof (CatalogInvalidSelfDependency));

    public static string CatalogInvalidSelfDependency(CultureInfo culture) => FrameworkResources.Get(nameof (CatalogInvalidSelfDependency), culture);

    public static string GenericLink() => FrameworkResources.Get(nameof (GenericLink));

    public static string GenericLink(CultureInfo culture) => FrameworkResources.Get(nameof (GenericLink), culture);

    public static string GenericLinkDescription() => FrameworkResources.Get(nameof (GenericLinkDescription));

    public static string GenericLinkDescription(CultureInfo culture) => FrameworkResources.Get(nameof (GenericLinkDescription), culture);

    public static string InfrastructureRoot() => FrameworkResources.Get(nameof (InfrastructureRoot));

    public static string InfrastructureRoot(CultureInfo culture) => FrameworkResources.Get(nameof (InfrastructureRoot), culture);

    public static string InfrastructureRootDescription() => FrameworkResources.Get(nameof (InfrastructureRootDescription));

    public static string InfrastructureRootDescription(CultureInfo culture) => FrameworkResources.Get(nameof (InfrastructureRootDescription), culture);

    public static string Machine() => FrameworkResources.Get(nameof (Machine));

    public static string Machine(CultureInfo culture) => FrameworkResources.Get(nameof (Machine), culture);

    public static string MachineDescription() => FrameworkResources.Get(nameof (MachineDescription));

    public static string MachineDescription(CultureInfo culture) => FrameworkResources.Get(nameof (MachineDescription), culture);

    public static string OrganizationalRoot() => FrameworkResources.Get(nameof (OrganizationalRoot));

    public static string OrganizationalRoot(CultureInfo culture) => FrameworkResources.Get(nameof (OrganizationalRoot), culture);

    public static string OrganizationalRootDescription() => FrameworkResources.Get(nameof (OrganizationalRootDescription));

    public static string OrganizationalRootDescription(CultureInfo culture) => FrameworkResources.Get(nameof (OrganizationalRootDescription), culture);

    public static string ProjectServerResource() => FrameworkResources.Get(nameof (ProjectServerResource));

    public static string ProjectServerResource(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectServerResource), culture);

    public static string ProjectServerDescription() => FrameworkResources.Get(nameof (ProjectServerDescription));

    public static string ProjectServerDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectServerDescription), culture);

    public static string ResourceFolder() => FrameworkResources.Get(nameof (ResourceFolder));

    public static string ResourceFolder(CultureInfo culture) => FrameworkResources.Get(nameof (ResourceFolder), culture);

    public static string ResourceFolderDescription() => FrameworkResources.Get(nameof (ResourceFolderDescription));

    public static string ResourceFolderDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ResourceFolderDescription), culture);

    public static string SqlDatabaseInstance() => FrameworkResources.Get(nameof (SqlDatabaseInstance));

    public static string SqlDatabaseInstance(CultureInfo culture) => FrameworkResources.Get(nameof (SqlDatabaseInstance), culture);

    public static string SqlDatabaseInstanceDescription() => FrameworkResources.Get(nameof (SqlDatabaseInstanceDescription));

    public static string SqlDatabaseInstanceDescription(CultureInfo culture) => FrameworkResources.Get(nameof (SqlDatabaseInstanceDescription), culture);

    public static string TeamFoundationConfigurationDatabase() => FrameworkResources.Get(nameof (TeamFoundationConfigurationDatabase));

    public static string TeamFoundationConfigurationDatabase(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationConfigurationDatabase), culture);

    public static string TeamFoundationConfigurationDatabaseDescription() => FrameworkResources.Get(nameof (TeamFoundationConfigurationDatabaseDescription));

    public static string TeamFoundationConfigurationDatabaseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationConfigurationDatabaseDescription), culture);

    public static string TeamFoundationProjectCollectionDatabase() => FrameworkResources.Get(nameof (TeamFoundationProjectCollectionDatabase));

    public static string TeamFoundationProjectCollectionDatabase(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationProjectCollectionDatabase), culture);

    public static string TeamFoundationProjectCollectionDatabaseDescription() => FrameworkResources.Get(nameof (TeamFoundationProjectCollectionDatabaseDescription));

    public static string TeamFoundationProjectCollectionDatabaseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationProjectCollectionDatabaseDescription), culture);

    public static string TeamFoundationServerInstance() => FrameworkResources.Get(nameof (TeamFoundationServerInstance));

    public static string TeamFoundationServerInstance(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationServerInstance), culture);

    public static string TeamFoundationServerInstanceDescription() => FrameworkResources.Get(nameof (TeamFoundationServerInstanceDescription));

    public static string TeamFoundationServerInstanceDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationServerInstanceDescription), culture);

    public static string TeamFoundationWebApplication() => FrameworkResources.Get(nameof (TeamFoundationWebApplication));

    public static string TeamFoundationWebApplication(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationWebApplication), culture);

    public static string TeamFoundationWebApplicationDescription() => FrameworkResources.Get(nameof (TeamFoundationWebApplicationDescription));

    public static string TeamFoundationWebApplicationDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationWebApplicationDescription), culture);

    public static string TeamProject() => FrameworkResources.Get(nameof (TeamProject));

    public static string TeamProject(CultureInfo culture) => FrameworkResources.Get(nameof (TeamProject), culture);

    public static string TeamProjectCollection() => FrameworkResources.Get(nameof (TeamProjectCollection));

    public static string TeamProjectCollection(CultureInfo culture) => FrameworkResources.Get(nameof (TeamProjectCollection), culture);

    public static string TeamProjectCollectionDescription() => FrameworkResources.Get(nameof (TeamProjectCollectionDescription));

    public static string TeamProjectCollectionDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TeamProjectCollectionDescription), culture);

    public static string TeamProjectDescription() => FrameworkResources.Get(nameof (TeamProjectDescription));

    public static string TeamProjectDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TeamProjectDescription), culture);

    public static string CatalogUnreferencedResourceCreateMessage(object arg0) => FrameworkResources.Format(nameof (CatalogUnreferencedResourceCreateMessage), arg0);

    public static string CatalogUnreferencedResourceCreateMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogUnreferencedResourceCreateMessage), culture, arg0);

    public static string UnexpectedHostType(object arg0) => FrameworkResources.Format(nameof (UnexpectedHostType), arg0);

    public static string UnexpectedHostType(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnexpectedHostType), culture, arg0);

    public static string ServiceAvailableInHostedTfsOnly() => FrameworkResources.Get(nameof (ServiceAvailableInHostedTfsOnly));

    public static string ServiceAvailableInHostedTfsOnly(CultureInfo culture) => FrameworkResources.Get(nameof (ServiceAvailableInHostedTfsOnly), culture);

    public static string ServiceAvailableInOnPremTfsOnly() => FrameworkResources.Get(nameof (ServiceAvailableInOnPremTfsOnly));

    public static string ServiceAvailableInOnPremTfsOnly(CultureInfo culture) => FrameworkResources.Get(nameof (ServiceAvailableInOnPremTfsOnly), culture);

    public static string MethodAvailableInHostedTfsOnly() => FrameworkResources.Get(nameof (MethodAvailableInHostedTfsOnly));

    public static string MethodAvailableInHostedTfsOnly(CultureInfo culture) => FrameworkResources.Get(nameof (MethodAvailableInHostedTfsOnly), culture);

    public static string CatalogNodeParentDoesNotExist(object arg0) => FrameworkResources.Format(nameof (CatalogNodeParentDoesNotExist), arg0);

    public static string CatalogNodeParentDoesNotExist(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogNodeParentDoesNotExist), culture, arg0);

    public static string IllegalAddToCatalogRoot(object arg0) => FrameworkResources.Format(nameof (IllegalAddToCatalogRoot), arg0);

    public static string IllegalAddToCatalogRoot(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IllegalAddToCatalogRoot), culture, arg0);

    public static string CatalogInvalidDeleteResourceType(object arg0) => FrameworkResources.Format(nameof (CatalogInvalidDeleteResourceType), arg0);

    public static string CatalogInvalidDeleteResourceType(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogInvalidDeleteResourceType), culture, arg0);

    public static string SharePointSiteCreationLocation() => FrameworkResources.Get(nameof (SharePointSiteCreationLocation));

    public static string SharePointSiteCreationLocation(CultureInfo culture) => FrameworkResources.Get(nameof (SharePointSiteCreationLocation), culture);

    public static string SharePointSiteCreationLocationDescription() => FrameworkResources.Get(nameof (SharePointSiteCreationLocationDescription));

    public static string SharePointSiteCreationLocationDescription(CultureInfo culture) => FrameworkResources.Get(nameof (SharePointSiteCreationLocationDescription), culture);

    public static string ProcessGuidanceSite() => FrameworkResources.Get(nameof (ProcessGuidanceSite));

    public static string ProcessGuidanceSite(CultureInfo culture) => FrameworkResources.Get(nameof (ProcessGuidanceSite), culture);

    public static string ProcessGuidanceSiteDescription() => FrameworkResources.Get(nameof (ProcessGuidanceSiteDescription));

    public static string ProcessGuidanceSiteDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProcessGuidanceSiteDescription), culture);

    public static string ProjectPortal() => FrameworkResources.Get(nameof (ProjectPortal));

    public static string ProjectPortal(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectPortal), culture);

    public static string ProjectPortalDescription() => FrameworkResources.Get(nameof (ProjectPortalDescription));

    public static string ProjectPortalDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectPortalDescription), culture);

    public static string DuplicateJobIdError() => FrameworkResources.Get(nameof (DuplicateJobIdError));

    public static string DuplicateJobIdError(CultureInfo culture) => FrameworkResources.Get(nameof (DuplicateJobIdError), culture);

    public static string JobDefinitionUpdatesNotPermitted(object arg0) => FrameworkResources.Format(nameof (JobDefinitionUpdatesNotPermitted), arg0);

    public static string JobDefinitionUpdatesNotPermitted(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobDefinitionUpdatesNotPermitted), culture, arg0);

    public static string SharePointWebApplication() => FrameworkResources.Get(nameof (SharePointWebApplication));

    public static string SharePointWebApplication(CultureInfo culture) => FrameworkResources.Get(nameof (SharePointWebApplication), culture);

    public static string DuplicateJobScheduleError() => FrameworkResources.Get(nameof (DuplicateJobScheduleError));

    public static string DuplicateJobScheduleError(CultureInfo culture) => FrameworkResources.Get(nameof (DuplicateJobScheduleError), culture);

    public static string SharePointWebApplicationDescription() => FrameworkResources.Get(nameof (SharePointWebApplicationDescription));

    public static string SharePointWebApplicationDescription(CultureInfo culture) => FrameworkResources.Get(nameof (SharePointWebApplicationDescription), culture);

    public static string CannotMoveCatalogNodeBelowItself() => FrameworkResources.Get(nameof (CannotMoveCatalogNodeBelowItself));

    public static string CannotMoveCatalogNodeBelowItself(CultureInfo culture) => FrameworkResources.Get(nameof (CannotMoveCatalogNodeBelowItself), culture);

    public static string ReportingServer() => FrameworkResources.Get(nameof (ReportingServer));

    public static string ReportingServer(CultureInfo culture) => FrameworkResources.Get(nameof (ReportingServer), culture);

    public static string JobCannotBeStoppedError(object arg0) => FrameworkResources.Format(nameof (JobCannotBeStoppedError), arg0);

    public static string JobCannotBeStoppedError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobCannotBeStoppedError), culture, arg0);

    public static string ReportingServerDescription() => FrameworkResources.Get(nameof (ReportingServerDescription));

    public static string ReportingServerDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ReportingServerDescription), culture);

    public static string JobCannotBePausedError(object arg0) => FrameworkResources.Format(nameof (JobCannotBePausedError), arg0);

    public static string JobCannotBePausedError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobCannotBePausedError), culture, arg0);

    public static string JobCannotBeResumedError(object arg0) => FrameworkResources.Format(nameof (JobCannotBeResumedError), arg0);

    public static string JobCannotBeResumedError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobCannotBeResumedError), culture, arg0);

    public static string JobDidntPauseError() => FrameworkResources.Get(nameof (JobDidntPauseError));

    public static string JobDidntPauseError(CultureInfo culture) => FrameworkResources.Get(nameof (JobDidntPauseError), culture);

    public static string Catalog_Validation_Exception_DuplicateProperty(object arg0, object arg1) => FrameworkResources.Format(nameof (Catalog_Validation_Exception_DuplicateProperty), arg0, arg1);

    public static string Catalog_Validation_Exception_DuplicateProperty(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (Catalog_Validation_Exception_DuplicateProperty), culture, arg0, arg1);
    }

    public static string FailedToReadTracingConfiguration(object arg0) => FrameworkResources.Format(nameof (FailedToReadTracingConfiguration), arg0);

    public static string FailedToReadTracingConfiguration(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FailedToReadTracingConfiguration), culture, arg0);

    public static string ServicingItemIsNull(object arg0) => FrameworkResources.Format(nameof (ServicingItemIsNull), arg0);

    public static string ServicingItemIsNull(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingItemIsNull), culture, arg0);

    public static string ServicingItemIsWrongType(object arg0, object arg1) => FrameworkResources.Format(nameof (ServicingItemIsWrongType), arg0, arg1);

    public static string ServicingItemIsWrongType(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingItemIsWrongType), culture, arg0, arg1);

    public static string ServicingItemNotDefined(object arg0) => FrameworkResources.Format(nameof (ServicingItemNotDefined), arg0);

    public static string ServicingItemNotDefined(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingItemNotDefined), culture, arg0);

    public static string RequiredServicingTokenIsNull(object arg0) => FrameworkResources.Format(nameof (RequiredServicingTokenIsNull), arg0);

    public static string RequiredServicingTokenIsNull(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequiredServicingTokenIsNull), culture, arg0);

    public static string RequiredServicingTokenNotDefined(object arg0) => FrameworkResources.Format(nameof (RequiredServicingTokenNotDefined), arg0);

    public static string RequiredServicingTokenNotDefined(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequiredServicingTokenNotDefined), culture, arg0);

    public static string CatalogNodeDependencyIncorrectType(object arg0, object arg1) => FrameworkResources.Format(nameof (CatalogNodeDependencyIncorrectType), arg0, arg1);

    public static string CatalogNodeDependencyIncorrectType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CatalogNodeDependencyIncorrectType), culture, arg0, arg1);
    }

    public static string SqlAnalysisInstance() => FrameworkResources.Get(nameof (SqlAnalysisInstance));

    public static string SqlAnalysisInstance(CultureInfo culture) => FrameworkResources.Get(nameof (SqlAnalysisInstance), culture);

    public static string SqlAnalysisInstanceDescription() => FrameworkResources.Get(nameof (SqlAnalysisInstanceDescription));

    public static string SqlAnalysisInstanceDescription(CultureInfo culture) => FrameworkResources.Get(nameof (SqlAnalysisInstanceDescription), culture);

    public static string SqlReportingInstance() => FrameworkResources.Get(nameof (SqlReportingInstance));

    public static string SqlReportingInstance(CultureInfo culture) => FrameworkResources.Get(nameof (SqlReportingInstance), culture);

    public static string SqlReportingInstanceDescription() => FrameworkResources.Get(nameof (SqlReportingInstanceDescription));

    public static string SqlReportingInstanceDescription(CultureInfo culture) => FrameworkResources.Get(nameof (SqlReportingInstanceDescription), culture);

    public static string AnalysisDatabaseDescription() => FrameworkResources.Get(nameof (AnalysisDatabaseDescription));

    public static string AnalysisDatabaseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisDatabaseDescription), culture);

    public static string ReportingConfiguration() => FrameworkResources.Get(nameof (ReportingConfiguration));

    public static string ReportingConfiguration(CultureInfo culture) => FrameworkResources.Get(nameof (ReportingConfiguration), culture);

    public static string ReportingConfigurationDescription() => FrameworkResources.Get(nameof (ReportingConfigurationDescription));

    public static string ReportingConfigurationDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ReportingConfigurationDescription), culture);

    public static string ReportingFolder() => FrameworkResources.Get(nameof (ReportingFolder));

    public static string ReportingFolder(CultureInfo culture) => FrameworkResources.Get(nameof (ReportingFolder), culture);

    public static string ReportingFolderDescription() => FrameworkResources.Get(nameof (ReportingFolderDescription));

    public static string ReportingFolderDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ReportingFolderDescription), culture);

    public static string WarehouseDatabase() => FrameworkResources.Get(nameof (WarehouseDatabase));

    public static string WarehouseDatabase(CultureInfo culture) => FrameworkResources.Get(nameof (WarehouseDatabase), culture);

    public static string AnalysisDatabase() => FrameworkResources.Get(nameof (AnalysisDatabase));

    public static string AnalysisDatabase(CultureInfo culture) => FrameworkResources.Get(nameof (AnalysisDatabase), culture);

    public static string WarehouseDatabaseDescription() => FrameworkResources.Get(nameof (WarehouseDatabaseDescription));

    public static string WarehouseDatabaseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (WarehouseDatabaseDescription), culture);

    public static string CatalogTFSInstanceNodeMissing(object arg0) => FrameworkResources.Format(nameof (CatalogTFSInstanceNodeMissing), arg0);

    public static string CatalogTFSInstanceNodeMissing(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogTFSInstanceNodeMissing), culture, arg0);

    public static string InvalidSecurityTokenElementLength(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidSecurityTokenElementLength), arg0, arg1);

    public static string InvalidSecurityTokenElementLength(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidSecurityTokenElementLength), culture, arg0, arg1);
    }

    public static string ToParentCrossesIsMasterBoundary() => FrameworkResources.Get(nameof (ToParentCrossesIsMasterBoundary));

    public static string ToParentCrossesIsMasterBoundary(CultureInfo culture) => FrameworkResources.Get(nameof (ToParentCrossesIsMasterBoundary), culture);

    public static string RemoteDoesNotMatchIdentityDomain() => FrameworkResources.Get(nameof (RemoteDoesNotMatchIdentityDomain));

    public static string RemoteDoesNotMatchIdentityDomain(CultureInfo culture) => FrameworkResources.Get(nameof (RemoteDoesNotMatchIdentityDomain), culture);

    public static string TeamSystemWebAccess() => FrameworkResources.Get(nameof (TeamSystemWebAccess));

    public static string TeamSystemWebAccess(CultureInfo culture) => FrameworkResources.Get(nameof (TeamSystemWebAccess), culture);

    public static string TeamSystemWebAccessDescription() => FrameworkResources.Get(nameof (TeamSystemWebAccessDescription));

    public static string TeamSystemWebAccessDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TeamSystemWebAccessDescription), culture);

    public static string NoServicingStepGroupsWithoutDependencies(object arg0) => FrameworkResources.Format(nameof (NoServicingStepGroupsWithoutDependencies), arg0);

    public static string NoServicingStepGroupsWithoutDependencies(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NoServicingStepGroupsWithoutDependencies), culture, arg0);

    public static string ProjectCollectionDeleteInProgress() => FrameworkResources.Get(nameof (ProjectCollectionDeleteInProgress));

    public static string ProjectCollectionDeleteInProgress(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionDeleteInProgress), culture);

    public static string HostCannotBeDeletedException(object arg0, object arg1) => FrameworkResources.Format(nameof (HostCannotBeDeletedException), arg0, arg1);

    public static string HostCannotBeDeletedException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (HostCannotBeDeletedException), culture, arg0, arg1);
    }

    public static string ProjectCollectionServicingInProgress() => FrameworkResources.Get(nameof (ProjectCollectionServicingInProgress));

    public static string ProjectCollectionServicingInProgress(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionServicingInProgress), culture);

    public static string ProjectCollectionServicingWaitingForShutdown() => FrameworkResources.Get(nameof (ProjectCollectionServicingWaitingForShutdown));

    public static string ProjectCollectionServicingWaitingForShutdown(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionServicingWaitingForShutdown), culture);

    public static string SecurityNamespaceHasDuplicatedAction(object arg0, object arg1) => FrameworkResources.Format(nameof (SecurityNamespaceHasDuplicatedAction), arg0, arg1);

    public static string SecurityNamespaceHasDuplicatedAction(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SecurityNamespaceHasDuplicatedAction), culture, arg0, arg1);
    }

    public static string SqlScriptResourceComponentGeneralError(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (SqlScriptResourceComponentGeneralError), arg0, arg1, arg2);
    }

    public static string SqlScriptResourceComponentGeneralError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SqlScriptResourceComponentGeneralError), culture, arg0, arg1, arg2);
    }

    public static string SqlScriptResourceComponentSqlError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (SqlScriptResourceComponentSqlError), arg0, arg1, arg2);

    public static string SqlScriptResourceComponentSqlError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SqlScriptResourceComponentSqlError), culture, arg0, arg1, arg2);
    }

    public static string SqlScriptFailed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5)
    {
      return FrameworkResources.Format(nameof (SqlScriptFailed), arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string SqlScriptFailed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SqlScriptFailed), culture, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string TestController() => FrameworkResources.Get(nameof (TestController));

    public static string TestController(CultureInfo culture) => FrameworkResources.Get(nameof (TestController), culture);

    public static string CouldNotLoadSecurityNamespaceExtension(object arg0, object arg1) => FrameworkResources.Format(nameof (CouldNotLoadSecurityNamespaceExtension), arg0, arg1);

    public static string CouldNotLoadSecurityNamespaceExtension(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CouldNotLoadSecurityNamespaceExtension), culture, arg0, arg1);
    }

    public static string SchemaUpgradeInProgressError(object arg0) => FrameworkResources.Format(nameof (SchemaUpgradeInProgressError), arg0);

    public static string SchemaUpgradeInProgressError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SchemaUpgradeInProgressError), culture, arg0);

    public static string TestControllerDescription() => FrameworkResources.Get(nameof (TestControllerDescription));

    public static string TestControllerDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TestControllerDescription), culture);

    public static string FrameworkAccessDeniedMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (FrameworkAccessDeniedMessage), arg0, arg1);

    public static string FrameworkAccessDeniedMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (FrameworkAccessDeniedMessage), culture, arg0, arg1);
    }

    public static string TestEnvironment() => FrameworkResources.Get(nameof (TestEnvironment));

    public static string TestEnvironment(CultureInfo culture) => FrameworkResources.Get(nameof (TestEnvironment), culture);

    public static string PermissionCatalogCreate() => FrameworkResources.Get(nameof (PermissionCatalogCreate));

    public static string PermissionCatalogCreate(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionCatalogCreate), culture);

    public static string TestEnvironmentDescription() => FrameworkResources.Get(nameof (TestEnvironmentDescription));

    public static string TestEnvironmentDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TestEnvironmentDescription), culture);

    public static string PermissionCatalogDelete() => FrameworkResources.Get(nameof (PermissionCatalogDelete));

    public static string PermissionCatalogDelete(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionCatalogDelete), culture);

    public static string PermissionCatalogModify() => FrameworkResources.Get(nameof (PermissionCatalogModify));

    public static string PermissionCatalogModify(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionCatalogModify), culture);

    public static string PermissionCatalogRead() => FrameworkResources.Get(nameof (PermissionCatalogRead));

    public static string PermissionCatalogRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionCatalogRead), culture);

    public static string PermissionCollectionGenericRead() => FrameworkResources.Get(nameof (PermissionCollectionGenericRead));

    public static string PermissionCollectionGenericRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionCollectionGenericRead), culture);

    public static string PermissionCollectionGenericWrite() => FrameworkResources.Get(nameof (PermissionCollectionGenericWrite));

    public static string PermissionCollectionGenericWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionCollectionGenericWrite), culture);

    public static string PermissionCollectionManagementCreate() => FrameworkResources.Get(nameof (PermissionCollectionManagementCreate));

    public static string PermissionCollectionManagementCreate(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionCollectionManagementCreate), culture);

    public static string PermissionCollectionManagementDelete() => FrameworkResources.Get(nameof (PermissionCollectionManagementDelete));

    public static string PermissionCollectionManagementDelete(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionCollectionManagementDelete), culture);

    public static string PermissionFrameworkImpersonate() => FrameworkResources.Get(nameof (PermissionFrameworkImpersonate));

    public static string PermissionFrameworkImpersonate(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionFrameworkImpersonate), culture);

    public static string PermissionIdentityRead() => FrameworkResources.Get(nameof (PermissionIdentityRead));

    public static string PermissionIdentityRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentityRead), culture);

    public static string PermissionTriggerEvent() => FrameworkResources.Get(nameof (PermissionTriggerEvent));

    public static string PermissionTriggerEvent(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionTriggerEvent), culture);

    public static string PermissionIdentityWrite() => FrameworkResources.Get(nameof (PermissionIdentityWrite));

    public static string PermissionIdentityWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentityWrite), culture);

    public static string PermissionIdentityDelete() => FrameworkResources.Get(nameof (PermissionIdentityDelete));

    public static string PermissionIdentityDelete(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentityDelete), culture);

    public static string PermissionInstanceGenericRead() => FrameworkResources.Get(nameof (PermissionInstanceGenericRead));

    public static string PermissionInstanceGenericRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionInstanceGenericRead), culture);

    public static string PermissionInstanceGenericWrite() => FrameworkResources.Get(nameof (PermissionInstanceGenericWrite));

    public static string PermissionInstanceGenericWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionInstanceGenericWrite), culture);

    public static string PermissionJobQueue() => FrameworkResources.Get(nameof (PermissionJobQueue));

    public static string PermissionJobQueue(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionJobQueue), culture);

    public static string PermissionJobRead() => FrameworkResources.Get(nameof (PermissionJobRead));

    public static string PermissionJobRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionJobRead), culture);

    public static string PermissionJobUpdate() => FrameworkResources.Get(nameof (PermissionJobUpdate));

    public static string PermissionJobUpdate(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionJobUpdate), culture);

    public static string PermissionRegistryRead() => FrameworkResources.Get(nameof (PermissionRegistryRead));

    public static string PermissionRegistryRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionRegistryRead), culture);

    public static string PermissionRegistryWrite() => FrameworkResources.Get(nameof (PermissionRegistryWrite));

    public static string PermissionRegistryWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionRegistryWrite), culture);

    public static string ServiceHostUpdateError(object arg0, object arg1) => FrameworkResources.Format(nameof (ServiceHostUpdateError), arg0, arg1);

    public static string ServiceHostUpdateError(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceHostUpdateError), culture, arg0, arg1);

    public static string ApplicationIsNotProcessingRequests() => FrameworkResources.Get(nameof (ApplicationIsNotProcessingRequests));

    public static string ApplicationIsNotProcessingRequests(CultureInfo culture) => FrameworkResources.Get(nameof (ApplicationIsNotProcessingRequests), culture);

    public static string HostOfflineAdministratorReason(object arg0) => FrameworkResources.Format(nameof (HostOfflineAdministratorReason), arg0);

    public static string HostOfflineAdministratorReason(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostOfflineAdministratorReason), culture, arg0);

    public static string HostOfflineWithAdministratorReasonFormatString(object arg0, object arg1) => FrameworkResources.Format(nameof (HostOfflineWithAdministratorReasonFormatString), arg0, arg1);

    public static string HostOfflineWithAdministratorReasonFormatString(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (HostOfflineWithAdministratorReasonFormatString), culture, arg0, arg1);
    }

    public static string StepDataUndefinedTokensWarning(object arg0) => FrameworkResources.Format(nameof (StepDataUndefinedTokensWarning), arg0);

    public static string StepDataUndefinedTokensWarning(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (StepDataUndefinedTokensWarning), culture, arg0);

    public static string StepDataUndefinedResourcesWarning(object arg0) => FrameworkResources.Format(nameof (StepDataUndefinedResourcesWarning), arg0);

    public static string StepDataUndefinedResourcesWarning(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (StepDataUndefinedResourcesWarning), culture, arg0);

    public static string PermissionLabReadWrite() => FrameworkResources.Get(nameof (PermissionLabReadWrite));

    public static string PermissionLabReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionLabReadWrite), culture);

    public static string TimezoneIntervalNotSupportedError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (TimezoneIntervalNotSupportedError), arg0, arg1, arg2);

    public static string TimezoneIntervalNotSupportedError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (TimezoneIntervalNotSupportedError), culture, arg0, arg1, arg2);
    }

    public static string SecurityChangedEventHandlerExceptionOccurred(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (SecurityChangedEventHandlerExceptionOccurred), arg0, arg1, arg2);
    }

    public static string SecurityChangedEventHandlerExceptionOccurred(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SecurityChangedEventHandlerExceptionOccurred), culture, arg0, arg1, arg2);
    }

    public static string BadInstanceStampMessage() => FrameworkResources.Get(nameof (BadInstanceStampMessage));

    public static string BadInstanceStampMessage(CultureInfo culture) => FrameworkResources.Get(nameof (BadInstanceStampMessage), culture);

    public static string CollectionInstanceIdAlreadyExists(object arg0) => FrameworkResources.Format(nameof (CollectionInstanceIdAlreadyExists), arg0);

    public static string CollectionInstanceIdAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionInstanceIdAlreadyExists), culture, arg0);

    public static string UnableToPropagateServiceDefinitionChange(object arg0, object arg1) => FrameworkResources.Format(nameof (UnableToPropagateServiceDefinitionChange), arg0, arg1);

    public static string UnableToPropagateServiceDefinitionChange(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UnableToPropagateServiceDefinitionChange), culture, arg0, arg1);
    }

    public static string InvalidCatalogNodeDependencyLoaded(object arg0) => FrameworkResources.Format(nameof (InvalidCatalogNodeDependencyLoaded), arg0);

    public static string InvalidCatalogNodeDependencyLoaded(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidCatalogNodeDependencyLoaded), culture, arg0);

    public static string SecurityNamespaceRequestContextHostMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (SecurityNamespaceRequestContextHostMessage), arg0, arg1, arg2);
    }

    public static string SecurityNamespaceRequestContextHostMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SecurityNamespaceRequestContextHostMessage), culture, arg0, arg1, arg2);
    }

    public static string SecurityServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (SecurityServiceRequestContextHostMessage), arg0, arg1);

    public static string SecurityServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SecurityServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string CatalogServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (CatalogServiceRequestContextHostMessage), arg0, arg1);

    public static string CatalogServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CatalogServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string LocationServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (LocationServiceRequestContextHostMessage), arg0, arg1);

    public static string LocationServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (LocationServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string LockingServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (LockingServiceRequestContextHostMessage), arg0, arg1);

    public static string LockingServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (LockingServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string ServicingServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (ServicingServiceRequestContextHostMessage), arg0, arg1);

    public static string ServicingServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServicingServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string CacheServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (CacheServiceRequestContextHostMessage), arg0, arg1);

    public static string CacheServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CacheServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string ProcessTemplateServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (ProcessTemplateServiceRequestContextHostMessage), arg0, arg1);

    public static string ProcessTemplateServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ProcessTemplateServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string ComponentServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (ComponentServiceRequestContextHostMessage), arg0, arg1);

    public static string ComponentServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ComponentServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string ServiceRequestContextHostMessage(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (ServiceRequestContextHostMessage), arg0, arg1, arg2);

    public static string ServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServiceRequestContextHostMessage), culture, arg0, arg1, arg2);
    }

    public static string TaggingServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (TaggingServiceRequestContextHostMessage), arg0, arg1);

    public static string TaggingServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (TaggingServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string TagProviderHasBeenRegisteredError(object arg0) => FrameworkResources.Format(nameof (TagProviderHasBeenRegisteredError), arg0);

    public static string TagProviderHasBeenRegisteredError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (TagProviderHasBeenRegisteredError), culture, arg0);

    public static string TagArtifactKindsMustBeSame() => FrameworkResources.Get(nameof (TagArtifactKindsMustBeSame));

    public static string TagArtifactKindsMustBeSame(CultureInfo culture) => FrameworkResources.Get(nameof (TagArtifactKindsMustBeSame), culture);

    public static string RemovingJobQueueEntryWithoutDefinition(object arg0, object arg1) => FrameworkResources.Format(nameof (RemovingJobQueueEntryWithoutDefinition), arg0, arg1);

    public static string RemovingJobQueueEntryWithoutDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (RemovingJobQueueEntryWithoutDefinition), culture, arg0, arg1);
    }

    public static string ServicingOperationAlreadyExists(object arg0) => FrameworkResources.Format(nameof (ServicingOperationAlreadyExists), arg0);

    public static string ServicingOperationAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingOperationAlreadyExists), culture, arg0);

    public static string ServicingStepGroupAlreadyExistsError(object arg0) => FrameworkResources.Format(nameof (ServicingStepGroupAlreadyExistsError), arg0);

    public static string ServicingStepGroupAlreadyExistsError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingStepGroupAlreadyExistsError), culture, arg0);

    public static string ServicingStepGroupInUseError(object arg0, object arg1) => FrameworkResources.Format(nameof (ServicingStepGroupInUseError), arg0, arg1);

    public static string ServicingStepGroupInUseError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServicingStepGroupInUseError), culture, arg0, arg1);
    }

    public static string DuplicateServicingStepGroupError(object arg0) => FrameworkResources.Format(nameof (DuplicateServicingStepGroupError), arg0);

    public static string DuplicateServicingStepGroupError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DuplicateServicingStepGroupError), culture, arg0);

    public static string ServicingOperationConfigDoesNotDefineOperationNameError() => FrameworkResources.Get(nameof (ServicingOperationConfigDoesNotDefineOperationNameError));

    public static string ServicingOperationConfigDoesNotDefineOperationNameError(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingOperationConfigDoesNotDefineOperationNameError), culture);

    public static string ServicingOperationFileDoesNotDefineOperationNameError(object arg0) => FrameworkResources.Format(nameof (ServicingOperationFileDoesNotDefineOperationNameError), arg0);

    public static string ServicingOperationFileDoesNotDefineOperationNameError(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServicingOperationFileDoesNotDefineOperationNameError), culture, arg0);
    }

    public static string FinishStepGroupMustBeCalledAfterStartStepGroupError() => FrameworkResources.Get(nameof (FinishStepGroupMustBeCalledAfterStartStepGroupError));

    public static string FinishStepGroupMustBeCalledAfterStartStepGroupError(CultureInfo culture) => FrameworkResources.Get(nameof (FinishStepGroupMustBeCalledAfterStartStepGroupError), culture);

    public static string ServicingOperationHandlerNotFound(object arg0) => FrameworkResources.Format(nameof (ServicingOperationHandlerNotFound), arg0);

    public static string ServicingOperationHandlerNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingOperationHandlerNotFound), culture, arg0);

    public static string ServicingStepGroupHandlerNotFound(object arg0) => FrameworkResources.Format(nameof (ServicingStepGroupHandlerNotFound), arg0);

    public static string ServicingStepGroupHandlerNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingStepGroupHandlerNotFound), culture, arg0);

    public static string StepPerformerHasBeenRegisteredError(object arg0) => FrameworkResources.Format(nameof (StepPerformerHasBeenRegisteredError), arg0);

    public static string StepPerformerHasBeenRegisteredError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (StepPerformerHasBeenRegisteredError), culture, arg0);

    public static string StepPerformerNotFoundException(object arg0) => FrameworkResources.Format(nameof (StepPerformerNotFoundException), arg0);

    public static string StepPerformerNotFoundException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (StepPerformerNotFoundException), culture, arg0);

    public static string StepPerformerNotFoundWithOperationAndGroup(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (StepPerformerNotFoundWithOperationAndGroup), arg0, arg1, arg2);
    }

    public static string StepPerformerNotFoundWithOperationAndGroup(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (StepPerformerNotFoundWithOperationAndGroup), culture, arg0, arg1, arg2);
    }

    public static string TargetRequestContextNotFound() => FrameworkResources.Get(nameof (TargetRequestContextNotFound));

    public static string TargetRequestContextNotFound(CultureInfo culture) => FrameworkResources.Get(nameof (TargetRequestContextNotFound), culture);

    public static string CatalogMissingParentNode(object arg0) => FrameworkResources.Format(nameof (CatalogMissingParentNode), arg0);

    public static string CatalogMissingParentNode(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogMissingParentNode), culture, arg0);

    public static string CollectionWithIdAlreadyExists(object arg0) => FrameworkResources.Format(nameof (CollectionWithIdAlreadyExists), arg0);

    public static string CollectionWithIdAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionWithIdAlreadyExists), culture, arg0);

    public static string ExecutingUnassignedJobError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8,
      object arg9,
      object arg10)
    {
      return FrameworkResources.Format(nameof (ExecutingUnassignedJobError), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    public static string ExecutingUnassignedJobError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8,
      object arg9,
      object arg10,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ExecutingUnassignedJobError), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    public static string IllegalDeleteSelfReferenceServiceDefinition() => FrameworkResources.Get(nameof (IllegalDeleteSelfReferenceServiceDefinition));

    public static string IllegalDeleteSelfReferenceServiceDefinition(CultureInfo culture) => FrameworkResources.Get(nameof (IllegalDeleteSelfReferenceServiceDefinition), culture);

    public static string WebRequestLoggerError(object arg0) => FrameworkResources.Format(nameof (WebRequestLoggerError), arg0);

    public static string WebRequestLoggerError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (WebRequestLoggerError), culture, arg0);

    public static string SQLInstanceUnreachable(object arg0, object arg1) => FrameworkResources.Format(nameof (SQLInstanceUnreachable), arg0, arg1);

    public static string SQLInstanceUnreachable(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (SQLInstanceUnreachable), culture, arg0, arg1);

    public static string ConnectionReestablished(object arg0, object arg1) => FrameworkResources.Format(nameof (ConnectionReestablished), arg0, arg1);

    public static string ConnectionReestablished(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ConnectionReestablished), culture, arg0, arg1);

    public static string HostShutdownTimeoutExceeded(object arg0, object arg1) => FrameworkResources.Format(nameof (HostShutdownTimeoutExceeded), arg0, arg1);

    public static string HostShutdownTimeoutExceeded(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (HostShutdownTimeoutExceeded), culture, arg0, arg1);

    public static string CannotStartHostDueToServicing(object arg0, object arg1) => FrameworkResources.Format(nameof (CannotStartHostDueToServicing), arg0, arg1);

    public static string CannotStartHostDueToServicing(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CannotStartHostDueToServicing), culture, arg0, arg1);
    }

    public static string DependentNodeDoesNotExist(object arg0) => FrameworkResources.Format(nameof (DependentNodeDoesNotExist), arg0);

    public static string DependentNodeDoesNotExist(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DependentNodeDoesNotExist), culture, arg0);

    public static string ServicingOperationNotFoundError(object arg0) => FrameworkResources.Format(nameof (ServicingOperationNotFoundError), arg0);

    public static string ServicingOperationNotFoundError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingOperationNotFoundError), culture, arg0);

    public static string DatabaseMustHaveSnapshotAttributeToAttach() => FrameworkResources.Get(nameof (DatabaseMustHaveSnapshotAttributeToAttach));

    public static string DatabaseMustHaveSnapshotAttributeToAttach(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseMustHaveSnapshotAttributeToAttach), culture);

    public static string InvalidIdentityMappings() => FrameworkResources.Get(nameof (InvalidIdentityMappings));

    public static string InvalidIdentityMappings(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidIdentityMappings), culture);

    public static string ArtifactKindsMustBeUniform() => FrameworkResources.Get(nameof (ArtifactKindsMustBeUniform));

    public static string ArtifactKindsMustBeUniform(CultureInfo culture) => FrameworkResources.Get(nameof (ArtifactKindsMustBeUniform), culture);

    public static string ArtifactKindRestricted(object arg0) => FrameworkResources.Format(nameof (ArtifactKindRestricted), arg0);

    public static string ArtifactKindRestricted(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ArtifactKindRestricted), culture, arg0);

    public static string GetServiceArgumentError(object arg0) => FrameworkResources.Format(nameof (GetServiceArgumentError), arg0);

    public static string GetServiceArgumentError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (GetServiceArgumentError), culture, arg0);

    public static string SecurityNamespaceHasActionWith0Bit(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (SecurityNamespaceHasActionWith0Bit), arg0, arg1, arg2);

    public static string SecurityNamespaceHasActionWith0Bit(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SecurityNamespaceHasActionWith0Bit), culture, arg0, arg1, arg2);
    }

    public static string DuplicateDatabasesFound(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return FrameworkResources.Format(nameof (DuplicateDatabasesFound), arg0, arg1, arg2, arg3, arg4);
    }

    public static string DuplicateDatabasesFound(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DuplicateDatabasesFound), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string CantEnterMethodTwice(object arg0, object arg1) => FrameworkResources.Format(nameof (CantEnterMethodTwice), arg0, arg1);

    public static string CantEnterMethodTwice(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (CantEnterMethodTwice), culture, arg0, arg1);

    public static string DeleteCollectionJobTitle(object arg0) => FrameworkResources.Format(nameof (DeleteCollectionJobTitle), arg0);

    public static string DeleteCollectionJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DeleteCollectionJobTitle), culture, arg0);

    public static string AttachCollectionJobTitle(object arg0) => FrameworkResources.Format(nameof (AttachCollectionJobTitle), arg0);

    public static string AttachCollectionJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AttachCollectionJobTitle), culture, arg0);

    public static string UpgradeCollectionJobTitle(object arg0) => FrameworkResources.Format(nameof (UpgradeCollectionJobTitle), arg0);

    public static string UpgradeCollectionJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UpgradeCollectionJobTitle), culture, arg0);

    public static string CreateCollectionJobTitle(object arg0) => FrameworkResources.Format(nameof (CreateCollectionJobTitle), arg0);

    public static string CreateCollectionJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CreateCollectionJobTitle), culture, arg0);

    public static string CreateTeamProjectJobTitle(object arg0) => FrameworkResources.Format(nameof (CreateTeamProjectJobTitle), arg0);

    public static string CreateTeamProjectJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CreateTeamProjectJobTitle), culture, arg0);

    public static string DeleteTeamProjectJobTitle(object arg0) => FrameworkResources.Format(nameof (DeleteTeamProjectJobTitle), arg0);

    public static string DeleteTeamProjectJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DeleteTeamProjectJobTitle), culture, arg0);

    public static string DetachCollectionJobTitle(object arg0) => FrameworkResources.Format(nameof (DetachCollectionJobTitle), arg0);

    public static string DetachCollectionJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DetachCollectionJobTitle), culture, arg0);

    public static string ServiceCollectionJobTitle(object arg0) => FrameworkResources.Format(nameof (ServiceCollectionJobTitle), arg0);

    public static string ServiceCollectionJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceCollectionJobTitle), culture, arg0);

    public static string DeletePrivateArtifactsJobTitle(object arg0) => FrameworkResources.Format(nameof (DeletePrivateArtifactsJobTitle), arg0);

    public static string DeletePrivateArtifactsJobTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DeletePrivateArtifactsJobTitle), culture, arg0);

    public static string UnableToFindDatabase(object arg0, object arg1) => FrameworkResources.Format(nameof (UnableToFindDatabase), arg0, arg1);

    public static string UnableToFindDatabase(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (UnableToFindDatabase), culture, arg0, arg1);

    public static string UnableToFindDatabaseWithSchema(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (UnableToFindDatabaseWithSchema), arg0, arg1, arg2);

    public static string UnableToFindDatabaseWithSchema(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UnableToFindDatabaseWithSchema), culture, arg0, arg1, arg2);
    }

    public static string UnableToFindFrameworkConnectionString(object arg0, object arg1) => FrameworkResources.Format(nameof (UnableToFindFrameworkConnectionString), arg0, arg1);

    public static string UnableToFindFrameworkConnectionString(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UnableToFindFrameworkConnectionString), culture, arg0, arg1);
    }

    public static string UnableToFindWarehouseDatabase() => FrameworkResources.Get(nameof (UnableToFindWarehouseDatabase));

    public static string UnableToFindWarehouseDatabase(CultureInfo culture) => FrameworkResources.Get(nameof (UnableToFindWarehouseDatabase), culture);

    public static string FailedToAcquireLock(object arg0) => FrameworkResources.Format(nameof (FailedToAcquireLock), arg0);

    public static string FailedToAcquireLock(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FailedToAcquireLock), culture, arg0);

    public static string ApplicationHostConfigurationRepaired(object arg0) => FrameworkResources.Format(nameof (ApplicationHostConfigurationRepaired), arg0);

    public static string ApplicationHostConfigurationRepaired(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ApplicationHostConfigurationRepaired), culture, arg0);

    public static string InvalidApplicationHostConfiguration(object arg0) => FrameworkResources.Format(nameof (InvalidApplicationHostConfiguration), arg0);

    public static string InvalidApplicationHostConfiguration(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidApplicationHostConfiguration), culture, arg0);

    public static string UsedConfigSqlInstanceForHostConnectionStringUpdateAction(
      object arg0,
      object arg1)
    {
      return FrameworkResources.Format(nameof (UsedConfigSqlInstanceForHostConnectionStringUpdateAction), arg0, arg1);
    }

    public static string UsedConfigSqlInstanceForHostConnectionStringUpdateAction(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UsedConfigSqlInstanceForHostConnectionStringUpdateAction), culture, arg0, arg1);
    }

    public static string InstanceStampMismatch(object arg0, object arg1) => FrameworkResources.Format(nameof (InstanceStampMismatch), arg0, arg1);

    public static string InstanceStampMismatch(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InstanceStampMismatch), culture, arg0, arg1);

    public static string UpdateConnectionStringAction(object arg0) => FrameworkResources.Format(nameof (UpdateConnectionStringAction), arg0);

    public static string UpdateConnectionStringAction(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UpdateConnectionStringAction), culture, arg0);

    public static string UpdateConnectionStringWithSchemasAction(object arg0, object arg1) => FrameworkResources.Format(nameof (UpdateConnectionStringWithSchemasAction), arg0, arg1);

    public static string UpdateConnectionStringWithSchemasAction(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UpdateConnectionStringWithSchemasAction), culture, arg0, arg1);
    }

    public static string ArtifactKindAlreadyExists(object arg0) => FrameworkResources.Format(nameof (ArtifactKindAlreadyExists), arg0);

    public static string ArtifactKindAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ArtifactKindAlreadyExists), culture, arg0);

    public static string WildcardPatternIsInvalidError(object arg0) => FrameworkResources.Format(nameof (WildcardPatternIsInvalidError), arg0);

    public static string WildcardPatternIsInvalidError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (WildcardPatternIsInvalidError), culture, arg0);

    public static string ArgumentPropertyIsInvalid(object arg0, object arg1) => FrameworkResources.Format(nameof (ArgumentPropertyIsInvalid), arg0, arg1);

    public static string ArgumentPropertyIsInvalid(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ArgumentPropertyIsInvalid), culture, arg0, arg1);

    public static string ArgumentPropertyCannotBeNull(object arg0) => FrameworkResources.Format(nameof (ArgumentPropertyCannotBeNull), arg0);

    public static string ArgumentPropertyCannotBeNull(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ArgumentPropertyCannotBeNull), culture, arg0);

    public static string DatabaseCategoryConnectionStringMissing(object arg0) => FrameworkResources.Format(nameof (DatabaseCategoryConnectionStringMissing), arg0);

    public static string DatabaseCategoryConnectionStringMissing(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseCategoryConnectionStringMissing), culture, arg0);

    public static string DatabaseCategoryConnectionStringIsInvalid(object arg0) => FrameworkResources.Format(nameof (DatabaseCategoryConnectionStringIsInvalid), arg0);

    public static string DatabaseCategoryConnectionStringIsInvalid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseCategoryConnectionStringIsInvalid), culture, arg0);

    public static string UpdateLocalMappingsFailed() => FrameworkResources.Get(nameof (UpdateLocalMappingsFailed));

    public static string UpdateLocalMappingsFailed(CultureInfo culture) => FrameworkResources.Get(nameof (UpdateLocalMappingsFailed), culture);

    public static string HostNotReadyCannotConnectToDatabase(object arg0) => FrameworkResources.Format(nameof (HostNotReadyCannotConnectToDatabase), arg0);

    public static string HostNotReadyCannotConnectToDatabase(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostNotReadyCannotConnectToDatabase), culture, arg0);

    public static string HostNotReadyConnectionStringNotValid(object arg0) => FrameworkResources.Format(nameof (HostNotReadyConnectionStringNotValid), arg0);

    public static string HostNotReadyConnectionStringNotValid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostNotReadyConnectionStringNotValid), culture, arg0);

    public static string HostNotReadyServicingFailed(object arg0) => FrameworkResources.Format(nameof (HostNotReadyServicingFailed), arg0);

    public static string HostNotReadyServicingFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostNotReadyServicingFailed), culture, arg0);

    public static string CollectionNotDetachableServicingFailed(object arg0) => FrameworkResources.Format(nameof (CollectionNotDetachableServicingFailed), arg0);

    public static string CollectionNotDetachableServicingFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionNotDetachableServicingFailed), culture, arg0);

    public static string HostNotReadySnapshotFailed(object arg0) => FrameworkResources.Format(nameof (HostNotReadySnapshotFailed), arg0);

    public static string HostNotReadySnapshotFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostNotReadySnapshotFailed), culture, arg0);

    public static string HostNotReadyCollectionDeleteFailed(object arg0) => FrameworkResources.Format(nameof (HostNotReadyCollectionDeleteFailed), arg0);

    public static string HostNotReadyCollectionDeleteFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostNotReadyCollectionDeleteFailed), culture, arg0);

    public static string HostNotReadyCollectionCloneFailed(object arg0) => FrameworkResources.Format(nameof (HostNotReadyCollectionCloneFailed), arg0);

    public static string HostNotReadyCollectionCloneFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostNotReadyCollectionCloneFailed), culture, arg0);

    public static string HostNotReadyCollectionVersionMismatch(object arg0) => FrameworkResources.Format(nameof (HostNotReadyCollectionVersionMismatch), arg0);

    public static string HostNotReadyCollectionVersionMismatch(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostNotReadyCollectionVersionMismatch), culture, arg0);

    public static string HostNotReadyValidatorException(object arg0, object arg1) => FrameworkResources.Format(nameof (HostNotReadyValidatorException), arg0, arg1);

    public static string HostNotReadyValidatorException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (HostNotReadyValidatorException), culture, arg0, arg1);
    }

    public static string HostNotReadyServicingInProgress(object arg0) => FrameworkResources.Format(nameof (HostNotReadyServicingInProgress), arg0);

    public static string HostNotReadyServicingInProgress(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostNotReadyServicingInProgress), culture, arg0);

    public static string ConfigurationHostCannotChangeStatus(object arg0, object arg1) => FrameworkResources.Format(nameof (ConfigurationHostCannotChangeStatus), arg0, arg1);

    public static string ConfigurationHostCannotChangeStatus(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ConfigurationHostCannotChangeStatus), culture, arg0, arg1);
    }

    public static string CollectionHostCannotChangeStatus(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (CollectionHostCannotChangeStatus), arg0, arg1, arg2);

    public static string CollectionHostCannotChangeStatus(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CollectionHostCannotChangeStatus), culture, arg0, arg1, arg2);
    }

    public static string CollectionCannotChangeStatusParent(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (CollectionCannotChangeStatusParent), arg0, arg1, arg2);

    public static string CollectionCannotChangeStatusParent(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CollectionCannotChangeStatusParent), culture, arg0, arg1, arg2);
    }

    public static string HostActionStarted() => FrameworkResources.Get(nameof (HostActionStarted));

    public static string HostActionStarted(CultureInfo culture) => FrameworkResources.Get(nameof (HostActionStarted), culture);

    public static string HostActionStopped() => FrameworkResources.Get(nameof (HostActionStopped));

    public static string HostActionStopped(CultureInfo culture) => FrameworkResources.Get(nameof (HostActionStopped), culture);

    public static string HostActionPaused() => FrameworkResources.Get(nameof (HostActionPaused));

    public static string HostActionPaused(CultureInfo culture) => FrameworkResources.Get(nameof (HostActionPaused), culture);

    public static string HostStateStarting() => FrameworkResources.Get(nameof (HostStateStarting));

    public static string HostStateStarting(CultureInfo culture) => FrameworkResources.Get(nameof (HostStateStarting), culture);

    public static string HostStateStarted() => FrameworkResources.Get(nameof (HostStateStarted));

    public static string HostStateStarted(CultureInfo culture) => FrameworkResources.Get(nameof (HostStateStarted), culture);

    public static string HostStateStopping() => FrameworkResources.Get(nameof (HostStateStopping));

    public static string HostStateStopping(CultureInfo culture) => FrameworkResources.Get(nameof (HostStateStopping), culture);

    public static string HostStateStopped() => FrameworkResources.Get(nameof (HostStateStopped));

    public static string HostStateStopped(CultureInfo culture) => FrameworkResources.Get(nameof (HostStateStopped), culture);

    public static string HostStatePausing() => FrameworkResources.Get(nameof (HostStatePausing));

    public static string HostStatePausing(CultureInfo culture) => FrameworkResources.Get(nameof (HostStatePausing), culture);

    public static string HostStatePaused() => FrameworkResources.Get(nameof (HostStatePaused));

    public static string HostStatePaused(CultureInfo culture) => FrameworkResources.Get(nameof (HostStatePaused), culture);

    public static string UsedConfigSqlInstanceForCategoryConnectionStringUpdateAction(
      object arg0,
      object arg1)
    {
      return FrameworkResources.Format(nameof (UsedConfigSqlInstanceForCategoryConnectionStringUpdateAction), arg0, arg1);
    }

    public static string UsedConfigSqlInstanceForCategoryConnectionStringUpdateAction(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UsedConfigSqlInstanceForCategoryConnectionStringUpdateAction), culture, arg0, arg1);
    }

    public static string SecurityIdentityCleanupCompleted(object arg0, object arg1) => FrameworkResources.Format(nameof (SecurityIdentityCleanupCompleted), arg0, arg1);

    public static string SecurityIdentityCleanupCompleted(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SecurityIdentityCleanupCompleted), culture, arg0, arg1);
    }

    public static string SecurityIdentityCleanupJobSuccessMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (SecurityIdentityCleanupJobSuccessMessage), arg0, arg1, arg2);
    }

    public static string SecurityIdentityCleanupJobSuccessMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SecurityIdentityCleanupJobSuccessMessage), culture, arg0, arg1, arg2);
    }

    public static string SubscriberNotFound(object arg0) => FrameworkResources.Format(nameof (SubscriberNotFound), arg0);

    public static string SubscriberNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SubscriberNotFound), culture, arg0);

    public static string UpdateCollectionPropertiesTitle(object arg0) => FrameworkResources.Format(nameof (UpdateCollectionPropertiesTitle), arg0);

    public static string UpdateCollectionPropertiesTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UpdateCollectionPropertiesTitle), culture, arg0);

    public static string AssignCollectionTitle(object arg0) => FrameworkResources.Format(nameof (AssignCollectionTitle), arg0);

    public static string AssignCollectionTitle(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AssignCollectionTitle), culture, arg0);

    public static string DataCollector() => FrameworkResources.Get(nameof (DataCollector));

    public static string DataCollector(CultureInfo culture) => FrameworkResources.Get(nameof (DataCollector), culture);

    public static string DataCollectorDescription() => FrameworkResources.Get(nameof (DataCollectorDescription));

    public static string DataCollectorDescription(CultureInfo culture) => FrameworkResources.Get(nameof (DataCollectorDescription), culture);

    public static string ReportingInstance() => FrameworkResources.Get(nameof (ReportingInstance));

    public static string ReportingInstance(CultureInfo culture) => FrameworkResources.Get(nameof (ReportingInstance), culture);

    public static string ReportingInstanceDescription() => FrameworkResources.Get(nameof (ReportingInstanceDescription));

    public static string ReportingInstanceDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ReportingInstanceDescription), culture);

    public static string DataDirectoryNotAbsolute(object arg0) => FrameworkResources.Format(nameof (DataDirectoryNotAbsolute), arg0);

    public static string DataDirectoryNotAbsolute(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DataDirectoryNotAbsolute), culture, arg0);

    public static string DataDirectoryConfigurationException(object arg0, object arg1) => FrameworkResources.Format(nameof (DataDirectoryConfigurationException), arg0, arg1);

    public static string DataDirectoryConfigurationException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DataDirectoryConfigurationException), culture, arg0, arg1);
    }

    public static string NoDefaultAccessMapping() => FrameworkResources.Get(nameof (NoDefaultAccessMapping));

    public static string NoDefaultAccessMapping(CultureInfo culture) => FrameworkResources.Get(nameof (NoDefaultAccessMapping), culture);

    public static string InvalidVirtualPathError(object arg0) => FrameworkResources.Format(nameof (InvalidVirtualPathError), arg0);

    public static string InvalidVirtualPathError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidVirtualPathError), culture, arg0);

    public static string JobDataIsInvalid() => FrameworkResources.Get(nameof (JobDataIsInvalid));

    public static string JobDataIsInvalid(CultureInfo culture) => FrameworkResources.Get(nameof (JobDataIsInvalid), culture);

    public static string UnreachableDatabases(object arg0) => FrameworkResources.Format(nameof (UnreachableDatabases), arg0);

    public static string UnreachableDatabases(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnreachableDatabases), culture, arg0);

    public static string UnreachableDatabasesReason() => FrameworkResources.Get(nameof (UnreachableDatabasesReason));

    public static string UnreachableDatabasesReason(CultureInfo culture) => FrameworkResources.Get(nameof (UnreachableDatabasesReason), culture);

    public static string UnreachableSqlInstances(object arg0) => FrameworkResources.Format(nameof (UnreachableSqlInstances), arg0);

    public static string UnreachableSqlInstances(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnreachableSqlInstances), culture, arg0);

    public static string UnreachableSqlInstancesReason() => FrameworkResources.Get(nameof (UnreachableSqlInstancesReason));

    public static string UnreachableSqlInstancesReason(CultureInfo culture) => FrameworkResources.Get(nameof (UnreachableSqlInstancesReason), culture);

    public static string ArtifactKindMonikerDisallowed(object arg0) => FrameworkResources.Format(nameof (ArtifactKindMonikerDisallowed), arg0);

    public static string ArtifactKindMonikerDisallowed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ArtifactKindMonikerDisallowed), culture, arg0);

    public static string AccessPointConflicts(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (AccessPointConflicts), arg0, arg1, arg2);

    public static string AccessPointConflicts(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AccessPointConflicts), culture, arg0, arg1, arg2);
    }

    public static string InvalidRegistryPathInvalidCharactersAndWildcards() => FrameworkResources.Get(nameof (InvalidRegistryPathInvalidCharactersAndWildcards));

    public static string InvalidRegistryPathInvalidCharactersAndWildcards(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidRegistryPathInvalidCharactersAndWildcards), culture);

    public static string NoConnectionStringFound(object arg0) => FrameworkResources.Format(nameof (NoConnectionStringFound), arg0);

    public static string NoConnectionStringFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NoConnectionStringFound), culture, arg0);

    public static string CannotUpgradeFromHigherServiceLevel() => FrameworkResources.Get(nameof (CannotUpgradeFromHigherServiceLevel));

    public static string CannotUpgradeFromHigherServiceLevel(CultureInfo culture) => FrameworkResources.Get(nameof (CannotUpgradeFromHigherServiceLevel), culture);

    public static string DatabaseIsBelowMinimumServiceLevel() => FrameworkResources.Get(nameof (DatabaseIsBelowMinimumServiceLevel));

    public static string DatabaseIsBelowMinimumServiceLevel(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseIsBelowMinimumServiceLevel), culture);

    public static string CannotUpgradeFromNonGoLiveDatabase() => FrameworkResources.Get(nameof (CannotUpgradeFromNonGoLiveDatabase));

    public static string CannotUpgradeFromNonGoLiveDatabase(CultureInfo culture) => FrameworkResources.Get(nameof (CannotUpgradeFromNonGoLiveDatabase), culture);

    public static string CannotUpgradeFromDatabaseVersion() => FrameworkResources.Get(nameof (CannotUpgradeFromDatabaseVersion));

    public static string CannotUpgradeFromDatabaseVersion(CultureInfo culture) => FrameworkResources.Get(nameof (CannotUpgradeFromDatabaseVersion), culture);

    public static string CannotAttachCollection(object arg0) => FrameworkResources.Format(nameof (CannotAttachCollection), arg0);

    public static string CannotAttachCollection(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CannotAttachCollection), culture, arg0);

    public static string AttachHostedCollectionServiceLevelMismatch(object arg0, object arg1) => FrameworkResources.Format(nameof (AttachHostedCollectionServiceLevelMismatch), arg0, arg1);

    public static string AttachHostedCollectionServiceLevelMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionServiceLevelMismatch), culture, arg0, arg1);
    }

    public static string AttachHostedCollectionUnexpectedMappingValidationException(object arg0) => FrameworkResources.Format(nameof (AttachHostedCollectionUnexpectedMappingValidationException), arg0);

    public static string AttachHostedCollectionUnexpectedMappingValidationException(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionUnexpectedMappingValidationException), culture, arg0);
    }

    public static string AttachHostedCollectionIdentityNameContainsInvalidCharacters(
      object arg0,
      object arg1)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionIdentityNameContainsInvalidCharacters), arg0, arg1);
    }

    public static string AttachHostedCollectionIdentityNameContainsInvalidCharacters(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionIdentityNameContainsInvalidCharacters), culture, arg0, arg1);
    }

    public static string AttachHostedCollectionDuplicateMappingFileEntries(object arg0) => FrameworkResources.Format(nameof (AttachHostedCollectionDuplicateMappingFileEntries), arg0);

    public static string AttachHostedCollectionDuplicateMappingFileEntries(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionDuplicateMappingFileEntries), culture, arg0);
    }

    public static string AttachHostedCollectionDuplicateLocalIdentities(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionDuplicateLocalIdentities), arg0, arg1, arg2);
    }

    public static string AttachHostedCollectionDuplicateLocalIdentities(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionDuplicateLocalIdentities), culture, arg0, arg1, arg2);
    }

    public static string AttachHostedCollectionMultipleResolvedLocalMappings(
      object arg0,
      object arg1)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionMultipleResolvedLocalMappings), arg0, arg1);
    }

    public static string AttachHostedCollectionMultipleResolvedLocalMappings(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionMultipleResolvedLocalMappings), culture, arg0, arg1);
    }

    public static string AttachHostedCollectionCannotResolveLocalIdentity(object arg0) => FrameworkResources.Format(nameof (AttachHostedCollectionCannotResolveLocalIdentity), arg0);

    public static string AttachHostedCollectionCannotResolveLocalIdentity(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionCannotResolveLocalIdentity), culture, arg0);
    }

    public static string AttachHostedCollectionMappingFileMissingEntries(object arg0) => FrameworkResources.Format(nameof (AttachHostedCollectionMappingFileMissingEntries), arg0);

    public static string AttachHostedCollectionMappingFileMissingEntries(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionMappingFileMissingEntries), culture, arg0);
    }

    public static string AttachHostedCollectionMappingFileExtraClouldIdentity(object arg0) => FrameworkResources.Format(nameof (AttachHostedCollectionMappingFileExtraClouldIdentity), arg0);

    public static string AttachHostedCollectionMappingFileExtraClouldIdentity(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionMappingFileExtraClouldIdentity), culture, arg0);
    }

    public static string AttachHostedCollectionMappingFileMappingLocalIdentityNotSpecified(
      object arg0)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionMappingFileMappingLocalIdentityNotSpecified), arg0);
    }

    public static string AttachHostedCollectionMappingFileMappingLocalIdentityNotSpecified(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachHostedCollectionMappingFileMappingLocalIdentityNotSpecified), culture, arg0);
    }

    public static string AttachCollectionServiceLevelNewer(object arg0, object arg1) => FrameworkResources.Format(nameof (AttachCollectionServiceLevelNewer), arg0, arg1);

    public static string AttachCollectionServiceLevelNewer(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AttachCollectionServiceLevelNewer), culture, arg0, arg1);
    }

    public static string DetachCollectionServiceLevelDiffer(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (DetachCollectionServiceLevelDiffer), arg0, arg1, arg2);

    public static string DetachCollectionServiceLevelDiffer(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DetachCollectionServiceLevelDiffer), culture, arg0, arg1, arg2);
    }

    public static string UnauthorizedAccessException(object arg0, object arg1) => FrameworkResources.Format(nameof (UnauthorizedAccessException), arg0, arg1);

    public static string UnauthorizedAccessException(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (UnauthorizedAccessException), culture, arg0, arg1);

    public static string CatalogInvalidChangeTypeNode(object arg0) => FrameworkResources.Format(nameof (CatalogInvalidChangeTypeNode), arg0);

    public static string CatalogInvalidChangeTypeNode(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogInvalidChangeTypeNode), culture, arg0);

    public static string CatalogInvalidChangeTypeResource(object arg0) => FrameworkResources.Format(nameof (CatalogInvalidChangeTypeResource), arg0);

    public static string CatalogInvalidChangeTypeResource(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CatalogInvalidChangeTypeResource), culture, arg0);

    public static string ArgumentMustBeWellKnownGroup() => FrameworkResources.Get(nameof (ArgumentMustBeWellKnownGroup));

    public static string ArgumentMustBeWellKnownGroup(CultureInfo culture) => FrameworkResources.Get(nameof (ArgumentMustBeWellKnownGroup), culture);

    public static string CannotAuthenticateAsTFSIdentity() => FrameworkResources.Get(nameof (CannotAuthenticateAsTFSIdentity));

    public static string CannotAuthenticateAsTFSIdentity(CultureInfo culture) => FrameworkResources.Get(nameof (CannotAuthenticateAsTFSIdentity), culture);

    public static string CollectionWithNameAlreadyExists(object arg0) => FrameworkResources.Format(nameof (CollectionWithNameAlreadyExists), arg0);

    public static string CollectionWithNameAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionWithNameAlreadyExists), culture, arg0);

    public static string CollectionWithConnectionStringAlreadyExists() => FrameworkResources.Get(nameof (CollectionWithConnectionStringAlreadyExists));

    public static string CollectionWithConnectionStringAlreadyExists(CultureInfo culture) => FrameworkResources.Get(nameof (CollectionWithConnectionStringAlreadyExists), culture);

    public static string CollectionNameReserved(object arg0) => FrameworkResources.Format(nameof (CollectionNameReserved), arg0);

    public static string CollectionNameReserved(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CollectionNameReserved), culture, arg0);

    public static string InvalidCollectionNameTwoDotError(object arg0) => FrameworkResources.Format(nameof (InvalidCollectionNameTwoDotError), arg0);

    public static string InvalidCollectionNameTwoDotError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidCollectionNameTwoDotError), culture, arg0);

    public static string JobIntervalOutOfRange(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (JobIntervalOutOfRange), arg0, arg1, arg2);

    public static string JobIntervalOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (JobIntervalOutOfRange), culture, arg0, arg1, arg2);
    }

    public static string JobStateUnexpected(object arg0) => FrameworkResources.Format(nameof (JobStateUnexpected), arg0);

    public static string JobStateUnexpected(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobStateUnexpected), culture, arg0);

    public static string ExtensionThrewException(object arg0) => FrameworkResources.Format(nameof (ExtensionThrewException), arg0);

    public static string ExtensionThrewException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ExtensionThrewException), culture, arg0);

    public static string UnexpectedJobAgentError() => FrameworkResources.Get(nameof (UnexpectedJobAgentError));

    public static string UnexpectedJobAgentError(CultureInfo culture) => FrameworkResources.Get(nameof (UnexpectedJobAgentError), culture);

    public static string JobRunnerThreadHasNotTerminatedError(object arg0, object arg1) => FrameworkResources.Format(nameof (JobRunnerThreadHasNotTerminatedError), arg0, arg1);

    public static string JobRunnerThreadHasNotTerminatedError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (JobRunnerThreadHasNotTerminatedError), culture, arg0, arg1);
    }

    public static string ErrorRecordingJobResult(object arg0) => FrameworkResources.Format(nameof (ErrorRecordingJobResult), arg0);

    public static string ErrorRecordingJobResult(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ErrorRecordingJobResult), culture, arg0);

    public static string ErrorAttemptingRetryableOperation() => FrameworkResources.Get(nameof (ErrorAttemptingRetryableOperation));

    public static string ErrorAttemptingRetryableOperation(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorAttemptingRetryableOperation), culture);

    public static string JobAgentStoppingDueToUnhandledException() => FrameworkResources.Get(nameof (JobAgentStoppingDueToUnhandledException));

    public static string JobAgentStoppingDueToUnhandledException(CultureInfo culture) => FrameworkResources.Get(nameof (JobAgentStoppingDueToUnhandledException), culture);

    public static string JobHealthStateChange(object arg0, object arg1) => FrameworkResources.Format(nameof (JobHealthStateChange), arg0, arg1);

    public static string JobHealthStateChange(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (JobHealthStateChange), culture, arg0, arg1);

    public static string JobHealthStateChangeDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return FrameworkResources.Format(nameof (JobHealthStateChangeDetails), arg0, arg1, arg2, arg3, arg4);
    }

    public static string JobHealthStateChangeDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (JobHealthStateChangeDetails), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string JobPartialSuccessStateEnter() => FrameworkResources.Get(nameof (JobPartialSuccessStateEnter));

    public static string JobPartialSuccessStateEnter(CultureInfo culture) => FrameworkResources.Get(nameof (JobPartialSuccessStateEnter), culture);

    public static string JobPartialSuccessStateLeave() => FrameworkResources.Get(nameof (JobPartialSuccessStateLeave));

    public static string JobPartialSuccessStateLeave(CultureInfo culture) => FrameworkResources.Get(nameof (JobPartialSuccessStateLeave), culture);

    public static string JobFailedStateEnter() => FrameworkResources.Get(nameof (JobFailedStateEnter));

    public static string JobFailedStateEnter(CultureInfo culture) => FrameworkResources.Get(nameof (JobFailedStateEnter), culture);

    public static string JobFailedStateLeave() => FrameworkResources.Get(nameof (JobFailedStateLeave));

    public static string JobFailedStateLeave(CultureInfo culture) => FrameworkResources.Get(nameof (JobFailedStateLeave), culture);

    public static string JobDisabledStateEnter() => FrameworkResources.Get(nameof (JobDisabledStateEnter));

    public static string JobDisabledStateEnter(CultureInfo culture) => FrameworkResources.Get(nameof (JobDisabledStateEnter), culture);

    public static string JobDisabledStateLeave() => FrameworkResources.Get(nameof (JobDisabledStateLeave));

    public static string JobDisabledStateLeave(CultureInfo culture) => FrameworkResources.Get(nameof (JobDisabledStateLeave), culture);

    public static string FailedToApplyPendingPatchOperations(object arg0) => FrameworkResources.Format(nameof (FailedToApplyPendingPatchOperations), arg0);

    public static string FailedToApplyPendingPatchOperations(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FailedToApplyPendingPatchOperations), culture, arg0);

    public static string AccessPointMustBeAValidURI(object arg0) => FrameworkResources.Format(nameof (AccessPointMustBeAValidURI), arg0);

    public static string AccessPointMustBeAValidURI(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AccessPointMustBeAValidURI), culture, arg0);

    public static string ErrorAccessingDictionaryKey(object arg0, object arg1) => FrameworkResources.Format(nameof (ErrorAccessingDictionaryKey), arg0, arg1);

    public static string ErrorAccessingDictionaryKey(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ErrorAccessingDictionaryKey), culture, arg0, arg1);

    public static string MaxDelaySecondsOutOfRange(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (MaxDelaySecondsOutOfRange), arg0, arg1, arg2);

    public static string MaxDelaySecondsOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MaxDelaySecondsOutOfRange), culture, arg0, arg1, arg2);
    }

    public static string ScheduledHighPriorityJobsProhibited() => FrameworkResources.Get(nameof (ScheduledHighPriorityJobsProhibited));

    public static string ScheduledHighPriorityJobsProhibited(CultureInfo culture) => FrameworkResources.Get(nameof (ScheduledHighPriorityJobsProhibited), culture);

    public static string IdentityProfileMissing() => FrameworkResources.Get(nameof (IdentityProfileMissing));

    public static string IdentityProfileMissing(CultureInfo culture) => FrameworkResources.Get(nameof (IdentityProfileMissing), culture);

    public static string IdentityProfileNotValidated() => FrameworkResources.Get(nameof (IdentityProfileNotValidated));

    public static string IdentityProfileNotValidated(CultureInfo culture) => FrameworkResources.Get(nameof (IdentityProfileNotValidated), culture);

    public static string JobsStillRunningDuringRecycleError(object arg0) => FrameworkResources.Format(nameof (JobsStillRunningDuringRecycleError), arg0);

    public static string JobsStillRunningDuringRecycleError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobsStillRunningDuringRecycleError), culture, arg0);

    public static string JobsStillRunningDuringTerminationError(object arg0) => FrameworkResources.Format(nameof (JobsStillRunningDuringTerminationError), arg0);

    public static string JobsStillRunningDuringTerminationError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobsStillRunningDuringTerminationError), culture, arg0);

    public static string ProcessStillRunningError() => FrameworkResources.Get(nameof (ProcessStillRunningError));

    public static string ProcessStillRunningError(CultureInfo culture) => FrameworkResources.Get(nameof (ProcessStillRunningError), culture);

    public static string JobAgentTeardownTimedOutError() => FrameworkResources.Get(nameof (JobAgentTeardownTimedOutError));

    public static string JobAgentTeardownTimedOutError(CultureInfo culture) => FrameworkResources.Get(nameof (JobAgentTeardownTimedOutError), culture);

    public static string MessageQueueDequeueTimeout(object arg0, object arg1) => FrameworkResources.Format(nameof (MessageQueueDequeueTimeout), arg0, arg1);

    public static string MessageQueueDequeueTimeout(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (MessageQueueDequeueTimeout), culture, arg0, arg1);

    public static string MessageQueueNotFound(object arg0) => FrameworkResources.Format(nameof (MessageQueueNotFound), arg0);

    public static string MessageQueueNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MessageQueueNotFound), culture, arg0);

    public static string MessageQueueAlreadyExists(object arg0) => FrameworkResources.Format(nameof (MessageQueueAlreadyExists), arg0);

    public static string MessageQueueAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MessageQueueAlreadyExists), culture, arg0);

    public static string InvalidMessageQueueName(object arg0) => FrameworkResources.Format(nameof (InvalidMessageQueueName), arg0);

    public static string InvalidMessageQueueName(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidMessageQueueName), culture, arg0);

    public static string ErrorMappingRequestToCatalogResource() => FrameworkResources.Get(nameof (ErrorMappingRequestToCatalogResource));

    public static string ErrorMappingRequestToCatalogResource(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorMappingRequestToCatalogResource), culture);

    public static string PermissionHostingAccountAdministerAccount() => FrameworkResources.Get(nameof (PermissionHostingAccountAdministerAccount));

    public static string PermissionHostingAccountAdministerAccount(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionHostingAccountAdministerAccount), culture);

    public static string PermissionHostingAccountAuditAccount() => FrameworkResources.Get(nameof (PermissionHostingAccountAuditAccount));

    public static string PermissionHostingAccountAuditAccount(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionHostingAccountAuditAccount), culture);

    public static string TeamFoundationUserProfileNotFoundException() => FrameworkResources.Get(nameof (TeamFoundationUserProfileNotFoundException));

    public static string TeamFoundationUserProfileNotFoundException(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationUserProfileNotFoundException), culture);

    public static string AuthenticationRequiredError() => FrameworkResources.Get(nameof (AuthenticationRequiredError));

    public static string AuthenticationRequiredError(CultureInfo culture) => FrameworkResources.Get(nameof (AuthenticationRequiredError), culture);

    public static string UnauthorizedUserError(object arg0) => FrameworkResources.Format(nameof (UnauthorizedUserError), arg0);

    public static string UnauthorizedUserError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnauthorizedUserError), culture, arg0);

    public static string DeltaTooLargeException() => FrameworkResources.Get(nameof (DeltaTooLargeException));

    public static string DeltaTooLargeException(CultureInfo culture) => FrameworkResources.Get(nameof (DeltaTooLargeException), culture);

    public static string UnableToLoadDll(object arg0) => FrameworkResources.Format(nameof (UnableToLoadDll), arg0);

    public static string UnableToLoadDll(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnableToLoadDll), culture, arg0);

    public static string DeltaComputationError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (DeltaComputationError), arg0, arg1, arg2);

    public static string DeltaComputationError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DeltaComputationError), culture, arg0, arg1, arg2);
    }

    public static string DeltaComputationErrorRetry(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (DeltaComputationErrorRetry), arg0, arg1, arg2, arg3);
    }

    public static string DeltaComputationErrorRetry(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DeltaComputationErrorRetry), culture, arg0, arg1, arg2, arg3);
    }

    public static string ERROR_PATCH_CORRUPT() => FrameworkResources.Get(nameof (ERROR_PATCH_CORRUPT));

    public static string ERROR_PATCH_CORRUPT(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_CORRUPT), culture);

    public static string ERROR_PATCH_DECODE_FAILURE() => FrameworkResources.Get(nameof (ERROR_PATCH_DECODE_FAILURE));

    public static string ERROR_PATCH_DECODE_FAILURE(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_DECODE_FAILURE), culture);

    public static string ERROR_PATCH_ENCODE_FAILURE() => FrameworkResources.Get(nameof (ERROR_PATCH_ENCODE_FAILURE));

    public static string ERROR_PATCH_ENCODE_FAILURE(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_ENCODE_FAILURE), culture);

    public static string ERROR_PATCH_IMAGEHLP_FAILURE() => FrameworkResources.Get(nameof (ERROR_PATCH_IMAGEHLP_FAILURE));

    public static string ERROR_PATCH_IMAGEHLP_FAILURE(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_IMAGEHLP_FAILURE), culture);

    public static string ERROR_PATCH_INVALID_OPTIONS() => FrameworkResources.Get(nameof (ERROR_PATCH_INVALID_OPTIONS));

    public static string ERROR_PATCH_INVALID_OPTIONS(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_INVALID_OPTIONS), culture);

    public static string ERROR_PATCH_NEWER_FORMAT() => FrameworkResources.Get(nameof (ERROR_PATCH_NEWER_FORMAT));

    public static string ERROR_PATCH_NEWER_FORMAT(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_NEWER_FORMAT), culture);

    public static string ERROR_PATCH_NOT_AVAILABLE() => FrameworkResources.Get(nameof (ERROR_PATCH_NOT_AVAILABLE));

    public static string ERROR_PATCH_NOT_AVAILABLE(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_NOT_AVAILABLE), culture);

    public static string ERROR_PATCH_NOT_NECESSARY() => FrameworkResources.Get(nameof (ERROR_PATCH_NOT_NECESSARY));

    public static string ERROR_PATCH_NOT_NECESSARY(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_NOT_NECESSARY), culture);

    public static string ERROR_PATCH_RETAIN_RANGES_DIFFER() => FrameworkResources.Get(nameof (ERROR_PATCH_RETAIN_RANGES_DIFFER));

    public static string ERROR_PATCH_RETAIN_RANGES_DIFFER(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_RETAIN_RANGES_DIFFER), culture);

    public static string ERROR_PATCH_SAME_FILE() => FrameworkResources.Get(nameof (ERROR_PATCH_SAME_FILE));

    public static string ERROR_PATCH_SAME_FILE(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_SAME_FILE), culture);

    public static string ERROR_PATCH_WRONG_FILE() => FrameworkResources.Get(nameof (ERROR_PATCH_WRONG_FILE));

    public static string ERROR_PATCH_WRONG_FILE(CultureInfo culture) => FrameworkResources.Get(nameof (ERROR_PATCH_WRONG_FILE), culture);

    public static string DuplicateFileNameException(object arg0) => FrameworkResources.Format(nameof (DuplicateFileNameException), arg0);

    public static string DuplicateFileNameException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DuplicateFileNameException), culture, arg0);

    public static string FileAlreadyUploadedException(object arg0) => FrameworkResources.Format(nameof (FileAlreadyUploadedException), arg0);

    public static string FileAlreadyUploadedException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FileAlreadyUploadedException), culture, arg0);

    public static string EmailAddressCouldNotBeDeterminedError() => FrameworkResources.Get(nameof (EmailAddressCouldNotBeDeterminedError));

    public static string EmailAddressCouldNotBeDeterminedError(CultureInfo culture) => FrameworkResources.Get(nameof (EmailAddressCouldNotBeDeterminedError), culture);

    public static string ServiceIdentityAlreadyExists(object arg0) => FrameworkResources.Format(nameof (ServiceIdentityAlreadyExists), arg0);

    public static string ServiceIdentityAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceIdentityAlreadyExists), culture, arg0);

    public static string AccessControlServiceMisconfigured() => FrameworkResources.Get(nameof (AccessControlServiceMisconfigured));

    public static string AccessControlServiceMisconfigured(CultureInfo culture) => FrameworkResources.Get(nameof (AccessControlServiceMisconfigured), culture);

    public static string RelyingPartyNotFound() => FrameworkResources.Get(nameof (RelyingPartyNotFound));

    public static string RelyingPartyNotFound(CultureInfo culture) => FrameworkResources.Get(nameof (RelyingPartyNotFound), culture);

    public static string ErrorComputingFolderStatistics(object arg0) => FrameworkResources.Format(nameof (ErrorComputingFolderStatistics), arg0);

    public static string ErrorComputingFolderStatistics(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ErrorComputingFolderStatistics), culture, arg0);

    public static string ConfigFileHasNoServers() => FrameworkResources.Get(nameof (ConfigFileHasNoServers));

    public static string ConfigFileHasNoServers(CultureInfo culture) => FrameworkResources.Get(nameof (ConfigFileHasNoServers), culture);

    public static string ConfigInvalidUriFormat(object arg0) => FrameworkResources.Format(nameof (ConfigInvalidUriFormat), arg0);

    public static string ConfigInvalidUriFormat(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ConfigInvalidUriFormat), culture, arg0);

    public static string ConfigMissingServerUriNode() => FrameworkResources.Get(nameof (ConfigMissingServerUriNode));

    public static string ConfigMissingServerUriNode(CultureInfo culture) => FrameworkResources.Get(nameof (ConfigMissingServerUriNode), culture);

    public static string ConfigurationNumericValueOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (ConfigurationNumericValueOutOfRange), arg0, arg1, arg2, arg3);
    }

    public static string ConfigurationNumericValueOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ConfigurationNumericValueOutOfRange), culture, arg0, arg1, arg2, arg3);
    }

    public static string ConfigValueNotNumeric(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (ConfigValueNotNumeric), arg0, arg1, arg2);

    public static string ConfigValueNotNumeric(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ConfigValueNotNumeric), culture, arg0, arg1, arg2);
    }

    public static string ErrorProcessingProxyConfig() => FrameworkResources.Get(nameof (ErrorProcessingProxyConfig));

    public static string ErrorProcessingProxyConfig(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorProcessingProxyConfig), culture);

    public static string ErrorReadingProxyStats() => FrameworkResources.Get(nameof (ErrorReadingProxyStats));

    public static string ErrorReadingProxyStats(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorReadingProxyStats), culture);

    public static string FileCacheRootNotAbsolute() => FrameworkResources.Get(nameof (FileCacheRootNotAbsolute));

    public static string FileCacheRootNotAbsolute(CultureInfo culture) => FrameworkResources.Get(nameof (FileCacheRootNotAbsolute), culture);

    public static string InvalidCacheRoot() => FrameworkResources.Get(nameof (InvalidCacheRoot));

    public static string InvalidCacheRoot(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidCacheRoot), culture);

    public static string ProxyCacheMissBecameHitException() => FrameworkResources.Get(nameof (ProxyCacheMissBecameHitException));

    public static string ProxyCacheMissBecameHitException(CultureInfo culture) => FrameworkResources.Get(nameof (ProxyCacheMissBecameHitException), culture);

    public static string ProxyClientRedirectException() => FrameworkResources.Get(nameof (ProxyClientRedirectException));

    public static string ProxyClientRedirectException(CultureInfo culture) => FrameworkResources.Get(nameof (ProxyClientRedirectException), culture);

    public static string ProxyStatsMissingNode(object arg0) => FrameworkResources.Format(nameof (ProxyStatsMissingNode), arg0);

    public static string ProxyStatsMissingNode(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ProxyStatsMissingNode), culture, arg0);

    public static string RequestSignatureValidationFailed() => FrameworkResources.Get(nameof (RequestSignatureValidationFailed));

    public static string RequestSignatureValidationFailed(CultureInfo culture) => FrameworkResources.Get(nameof (RequestSignatureValidationFailed), culture);

    public static string RequestTicketTypeNotSupported(object arg0) => FrameworkResources.Format(nameof (RequestTicketTypeNotSupported), arg0);

    public static string RequestTicketTypeNotSupported(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequestTicketTypeNotSupported), culture, arg0);

    public static string StartingCacheCleanup(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (StartingCacheCleanup), arg0, arg1, arg2);

    public static string StartingCacheCleanup(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (StartingCacheCleanup), culture, arg0, arg1, arg2);
    }

    public static string UnknownProxyException() => FrameworkResources.Get(nameof (UnknownProxyException));

    public static string UnknownProxyException(CultureInfo culture) => FrameworkResources.Get(nameof (UnknownProxyException), culture);

    public static string DefaultCacheLimit(object arg0) => FrameworkResources.Format(nameof (DefaultCacheLimit), arg0);

    public static string DefaultCacheLimit(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DefaultCacheLimit), culture, arg0);

    public static string DestroyedContentUnavailableException() => FrameworkResources.Get(nameof (DestroyedContentUnavailableException));

    public static string DestroyedContentUnavailableException(CultureInfo culture) => FrameworkResources.Get(nameof (DestroyedContentUnavailableException), culture);

    public static string DuplicateServerUri(object arg0) => FrameworkResources.Format(nameof (DuplicateServerUri), arg0);

    public static string DuplicateServerUri(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DuplicateServerUri), culture, arg0);

    public static string ErrorCommitingToCache() => FrameworkResources.Get(nameof (ErrorCommitingToCache));

    public static string ErrorCommitingToCache(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorCommitingToCache), culture);

    public static string ErrorDownloadingFromAppTier() => FrameworkResources.Get(nameof (ErrorDownloadingFromAppTier));

    public static string ErrorDownloadingFromAppTier(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorDownloadingFromAppTier), culture);

    public static string ErrorLoadingCollection(object arg0, object arg1) => FrameworkResources.Format(nameof (ErrorLoadingCollection), arg0, arg1);

    public static string ErrorLoadingCollection(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ErrorLoadingCollection), culture, arg0, arg1);

    public static string MetadataFormatWrong(object arg0) => FrameworkResources.Format(nameof (MetadataFormatWrong), arg0);

    public static string MetadataFormatWrong(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MetadataFormatWrong), culture, arg0);

    public static string RequestExpired() => FrameworkResources.Get(nameof (RequestExpired));

    public static string RequestExpired(CultureInfo culture) => FrameworkResources.Get(nameof (RequestExpired), culture);

    public static string RequestFileIdMalformed() => FrameworkResources.Get(nameof (RequestFileIdMalformed));

    public static string RequestFileIdMalformed(CultureInfo culture) => FrameworkResources.Get(nameof (RequestFileIdMalformed), culture);

    public static string RequestFileIdMissing() => FrameworkResources.Get(nameof (RequestFileIdMissing));

    public static string RequestFileIdMissing(CultureInfo culture) => FrameworkResources.Get(nameof (RequestFileIdMissing), culture);

    public static string RequestServerIdMissing() => FrameworkResources.Get(nameof (RequestServerIdMissing));

    public static string RequestServerIdMissing(CultureInfo culture) => FrameworkResources.Get(nameof (RequestServerIdMissing), culture);

    public static string MultipleIdentitiesFoundRow(object arg0, object arg1) => FrameworkResources.Format(nameof (MultipleIdentitiesFoundRow), arg0, arg1);

    public static string MultipleIdentitiesFoundRow(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (MultipleIdentitiesFoundRow), culture, arg0, arg1);

    public static string ServiceIdentityClaimRuleDescription(object arg0) => FrameworkResources.Format(nameof (ServiceIdentityClaimRuleDescription), arg0);

    public static string ServiceIdentityClaimRuleDescription(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceIdentityClaimRuleDescription), culture, arg0);

    public static string QueryExpression_InvalidEscapeSequence() => FrameworkResources.Get(nameof (QueryExpression_InvalidEscapeSequence));

    public static string QueryExpression_InvalidEscapeSequence(CultureInfo culture) => FrameworkResources.Get(nameof (QueryExpression_InvalidEscapeSequence), culture);

    public static string QueryExpression_InvalidToken(object arg0) => FrameworkResources.Format(nameof (QueryExpression_InvalidToken), arg0);

    public static string QueryExpression_InvalidToken(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (QueryExpression_InvalidToken), culture, arg0);

    public static string QueryExpression_Malformed() => FrameworkResources.Get(nameof (QueryExpression_Malformed));

    public static string QueryExpression_Malformed(CultureInfo culture) => FrameworkResources.Get(nameof (QueryExpression_Malformed), culture);

    public static string QueryExpression_TokenMismatch(object arg0, object arg1) => FrameworkResources.Format(nameof (QueryExpression_TokenMismatch), arg0, arg1);

    public static string QueryExpression_TokenMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (QueryExpression_TokenMismatch), culture, arg0, arg1);
    }

    public static string IdentityExpression_TargetIdentityMustBeId() => FrameworkResources.Get(nameof (IdentityExpression_TargetIdentityMustBeId));

    public static string IdentityExpression_TargetIdentityMustBeId(CultureInfo culture) => FrameworkResources.Get(nameof (IdentityExpression_TargetIdentityMustBeId), culture);

    public static string PermissionIdentityManageMembership() => FrameworkResources.Get(nameof (PermissionIdentityManageMembership));

    public static string PermissionIdentityManageMembership(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentityManageMembership), culture);

    public static string CustomDisplayNameError() => FrameworkResources.Get(nameof (CustomDisplayNameError));

    public static string CustomDisplayNameError(CultureInfo culture) => FrameworkResources.Get(nameof (CustomDisplayNameError), culture);

    public static string AccountExpired() => FrameworkResources.Get(nameof (AccountExpired));

    public static string AccountExpired(CultureInfo culture) => FrameworkResources.Get(nameof (AccountExpired), culture);

    public static string AccountBeingDeleted() => FrameworkResources.Get(nameof (AccountBeingDeleted));

    public static string AccountBeingDeleted(CultureInfo culture) => FrameworkResources.Get(nameof (AccountBeingDeleted), culture);

    public static string HostCreationUnavailable() => FrameworkResources.Get(nameof (HostCreationUnavailable));

    public static string HostCreationUnavailable(CultureInfo culture) => FrameworkResources.Get(nameof (HostCreationUnavailable), culture);

    public static string MaxCollectionCountReached() => FrameworkResources.Get(nameof (MaxCollectionCountReached));

    public static string MaxCollectionCountReached(CultureInfo culture) => FrameworkResources.Get(nameof (MaxCollectionCountReached), culture);

    public static string InvalidLobParameter(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidLobParameter), arg0, arg1);

    public static string InvalidLobParameter(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidLobParameter), culture, arg0, arg1);

    public static string IdentityNotFoundWithName(object arg0) => FrameworkResources.Format(nameof (IdentityNotFoundWithName), arg0);

    public static string IdentityNotFoundWithName(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IdentityNotFoundWithName), culture, arg0);

    public static string RequestedPageNotFound(object arg0) => FrameworkResources.Format(nameof (RequestedPageNotFound), arg0);

    public static string RequestedPageNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequestedPageNotFound), culture, arg0);

    public static string InvalidAccessException() => FrameworkResources.Get(nameof (InvalidAccessException));

    public static string InvalidAccessException(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidAccessException), culture);

    public static string LockIsNotExclusive() => FrameworkResources.Get(nameof (LockIsNotExclusive));

    public static string LockIsNotExclusive(CultureInfo culture) => FrameworkResources.Get(nameof (LockIsNotExclusive), culture);

    public static string LockIsNotCollectionServicing() => FrameworkResources.Get(nameof (LockIsNotCollectionServicing));

    public static string LockIsNotCollectionServicing(CultureInfo culture) => FrameworkResources.Get(nameof (LockIsNotCollectionServicing), culture);

    public static string MultipleLocksTaken() => FrameworkResources.Get(nameof (MultipleLocksTaken));

    public static string MultipleLocksTaken(CultureInfo culture) => FrameworkResources.Get(nameof (MultipleLocksTaken), culture);

    public static string DataTierNotRegistered() => FrameworkResources.Get(nameof (DataTierNotRegistered));

    public static string DataTierNotRegistered(CultureInfo culture) => FrameworkResources.Get(nameof (DataTierNotRegistered), culture);

    public static string NotSupportedOnSqlAzure() => FrameworkResources.Get(nameof (NotSupportedOnSqlAzure));

    public static string NotSupportedOnSqlAzure(CultureInfo culture) => FrameworkResources.Get(nameof (NotSupportedOnSqlAzure), culture);

    public static string MethodCanOnlyBeExecutedOnMaster() => FrameworkResources.Get(nameof (MethodCanOnlyBeExecutedOnMaster));

    public static string MethodCanOnlyBeExecutedOnMaster(CultureInfo culture) => FrameworkResources.Get(nameof (MethodCanOnlyBeExecutedOnMaster), culture);

    public static string TeamFoundationAnonymousUsersGroup() => FrameworkResources.Get(nameof (TeamFoundationAnonymousUsersGroup));

    public static string TeamFoundationAnonymousUsersGroup(CultureInfo culture) => FrameworkResources.Get(nameof (TeamFoundationAnonymousUsersGroup), culture);

    public static string ResourceCannotBeFound() => FrameworkResources.Get(nameof (ResourceCannotBeFound));

    public static string ResourceCannotBeFound(CultureInfo culture) => FrameworkResources.Get(nameof (ResourceCannotBeFound), culture);

    public static string HostProcessNotFound(object arg0) => FrameworkResources.Format(nameof (HostProcessNotFound), arg0);

    public static string HostProcessNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostProcessNotFound), culture, arg0);

    public static string StreamingCollectionNotSupportedError() => FrameworkResources.Get(nameof (StreamingCollectionNotSupportedError));

    public static string StreamingCollectionNotSupportedError(CultureInfo culture) => FrameworkResources.Get(nameof (StreamingCollectionNotSupportedError), culture);

    public static string DeploymentOrApplicationHostRequired() => FrameworkResources.Get(nameof (DeploymentOrApplicationHostRequired));

    public static string DeploymentOrApplicationHostRequired(CultureInfo culture) => FrameworkResources.Get(nameof (DeploymentOrApplicationHostRequired), culture);

    public static string ApplicationHostRequired() => FrameworkResources.Get(nameof (ApplicationHostRequired));

    public static string ApplicationHostRequired(CultureInfo culture) => FrameworkResources.Get(nameof (ApplicationHostRequired), culture);

    public static string DeploymentHostRequired() => FrameworkResources.Get(nameof (DeploymentHostRequired));

    public static string DeploymentHostRequired(CultureInfo culture) => FrameworkResources.Get(nameof (DeploymentHostRequired), culture);

    public static string InvalidChildHost(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidChildHost), arg0, arg1);

    public static string InvalidChildHost(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidChildHost), culture, arg0, arg1);

    public static string LicensingPackageValidation_MultipleDefaultLicenseTypes() => FrameworkResources.Get(nameof (LicensingPackageValidation_MultipleDefaultLicenseTypes));

    public static string LicensingPackageValidation_MultipleDefaultLicenseTypes(CultureInfo culture) => FrameworkResources.Get(nameof (LicensingPackageValidation_MultipleDefaultLicenseTypes), culture);

    public static string LicensingPackageValidation_DuplicateFeatureOrLicenseType(object arg0) => FrameworkResources.Format(nameof (LicensingPackageValidation_DuplicateFeatureOrLicenseType), arg0);

    public static string LicensingPackageValidation_DuplicateFeatureOrLicenseType(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (LicensingPackageValidation_DuplicateFeatureOrLicenseType), culture, arg0);
    }

    public static string LicensingPackage_UnknownFeature(object arg0) => FrameworkResources.Format(nameof (LicensingPackage_UnknownFeature), arg0);

    public static string LicensingPackage_UnknownFeature(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (LicensingPackage_UnknownFeature), culture, arg0);

    public static string LicensingPackage_UnknownLicenseType(object arg0) => FrameworkResources.Format(nameof (LicensingPackage_UnknownLicenseType), arg0);

    public static string LicensingPackage_UnknownLicenseType(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (LicensingPackage_UnknownLicenseType), culture, arg0);

    public static string AccessControlCreateRPException(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (AccessControlCreateRPException), arg0, arg1, arg2);

    public static string AccessControlCreateRPException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AccessControlCreateRPException), culture, arg0, arg1, arg2);
    }

    public static string AccessControlCreateSIException(object arg0, object arg1) => FrameworkResources.Format(nameof (AccessControlCreateSIException), arg0, arg1);

    public static string AccessControlCreateSIException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AccessControlCreateSIException), culture, arg0, arg1);
    }

    public static string AccessControlDeleteRPException(object arg0) => FrameworkResources.Format(nameof (AccessControlDeleteRPException), arg0);

    public static string AccessControlDeleteRPException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AccessControlDeleteRPException), culture, arg0);

    public static string AccessControlDeleteSIException(object arg0) => FrameworkResources.Format(nameof (AccessControlDeleteSIException), arg0);

    public static string AccessControlDeleteSIException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AccessControlDeleteSIException), culture, arg0);

    public static string AccessControlExceptionDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7)
    {
      return FrameworkResources.Format(nameof (AccessControlExceptionDetails), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string AccessControlExceptionDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AccessControlExceptionDetails), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string AccessControlGetSIException() => FrameworkResources.Get(nameof (AccessControlGetSIException));

    public static string AccessControlGetSIException(CultureInfo culture) => FrameworkResources.Get(nameof (AccessControlGetSIException), culture);

    public static string InvalidCollation(object arg0) => FrameworkResources.Format(nameof (InvalidCollation), arg0);

    public static string InvalidCollation(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidCollation), culture, arg0);

    public static string PreferenceCannotBeEmpty(object arg0) => FrameworkResources.Format(nameof (PreferenceCannotBeEmpty), arg0);

    public static string PreferenceCannotBeEmpty(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (PreferenceCannotBeEmpty), culture, arg0);

    public static string InvalidSpecificCultureValue(object arg0) => FrameworkResources.Format(nameof (InvalidSpecificCultureValue), arg0);

    public static string InvalidSpecificCultureValue(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidSpecificCultureValue), culture, arg0);

    public static string InvalidSystemTimeZoneValue(object arg0) => FrameworkResources.Format(nameof (InvalidSystemTimeZoneValue), arg0);

    public static string InvalidSystemTimeZoneValue(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidSystemTimeZoneValue), culture, arg0);

    public static string LcidMustBeInstalledLanguageError(object arg0) => FrameworkResources.Format(nameof (LcidMustBeInstalledLanguageError), arg0);

    public static string LcidMustBeInstalledLanguageError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (LcidMustBeInstalledLanguageError), culture, arg0);

    public static string DeadlockWhileExecutingSQL() => FrameworkResources.Get(nameof (DeadlockWhileExecutingSQL));

    public static string DeadlockWhileExecutingSQL(CultureInfo culture) => FrameworkResources.Get(nameof (DeadlockWhileExecutingSQL), culture);

    public static string PropertyDefinitionDoesNotExist(object arg0) => FrameworkResources.Format(nameof (PropertyDefinitionDoesNotExist), arg0);

    public static string PropertyDefinitionDoesNotExist(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (PropertyDefinitionDoesNotExist), culture, arg0);

    public static string UnknownMigrationOwnerException(object arg0) => FrameworkResources.Format(nameof (UnknownMigrationOwnerException), arg0);

    public static string UnknownMigrationOwnerException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnknownMigrationOwnerException), culture, arg0);

    public static string FavoriteItemAlreadyExist() => FrameworkResources.Get(nameof (FavoriteItemAlreadyExist));

    public static string FavoriteItemAlreadyExist(CultureInfo culture) => FrameworkResources.Get(nameof (FavoriteItemAlreadyExist), culture);

    public static string ParentFavoriteItemCouldNotBeFound() => FrameworkResources.Get(nameof (ParentFavoriteItemCouldNotBeFound));

    public static string ParentFavoriteItemCouldNotBeFound(CultureInfo culture) => FrameworkResources.Get(nameof (ParentFavoriteItemCouldNotBeFound), culture);

    public static string ServerExtensionLoadFailure(object arg0, object arg1) => FrameworkResources.Format(nameof (ServerExtensionLoadFailure), arg0, arg1);

    public static string ServerExtensionLoadFailure(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ServerExtensionLoadFailure), culture, arg0, arg1);

    public static string ServerExtensionDuplicateLoad(object arg0, object arg1) => FrameworkResources.Format(nameof (ServerExtensionDuplicateLoad), arg0, arg1);

    public static string ServerExtensionDuplicateLoad(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServerExtensionDuplicateLoad), culture, arg0, arg1);
    }

    public static string ServerAssemblyDuplicateLoadDifferentVersion(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (ServerAssemblyDuplicateLoadDifferentVersion), arg0, arg1, arg2);
    }

    public static string ServerAssemblyDuplicateLoadDifferentVersion(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServerAssemblyDuplicateLoadDifferentVersion), culture, arg0, arg1, arg2);
    }

    public static string ProjectCreateStepDescription() => FrameworkResources.Get(nameof (ProjectCreateStepDescription));

    public static string ProjectCreateStepDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCreateStepDescription), culture);

    public static string ProjectPromoteStepDescription() => FrameworkResources.Get(nameof (ProjectPromoteStepDescription));

    public static string ProjectPromoteStepDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectPromoteStepDescription), culture);

    public static string FailedToDisposeContext() => FrameworkResources.Get(nameof (FailedToDisposeContext));

    public static string FailedToDisposeContext(CultureInfo culture) => FrameworkResources.Get(nameof (FailedToDisposeContext), culture);

    public static string ProjectCollectionServicingFailureMessage() => FrameworkResources.Get(nameof (ProjectCollectionServicingFailureMessage));

    public static string ProjectCollectionServicingFailureMessage(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionServicingFailureMessage), culture);

    public static string ClientDisconnectedCancelReason() => FrameworkResources.Get(nameof (ClientDisconnectedCancelReason));

    public static string ClientDisconnectedCancelReason(CultureInfo culture) => FrameworkResources.Get(nameof (ClientDisconnectedCancelReason), culture);

    public static string InvalidParentServiceHostError(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidParentServiceHostError), arg0, arg1);

    public static string InvalidParentServiceHostError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidParentServiceHostError), culture, arg0, arg1);
    }

    public static string InvalidBindPendingIdentityDescriptorErrorV2(object arg0) => FrameworkResources.Format(nameof (InvalidBindPendingIdentityDescriptorErrorV2), arg0);

    public static string InvalidBindPendingIdentityDescriptorErrorV2(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidBindPendingIdentityDescriptorErrorV2), culture, arg0);
    }

    public static string InvalidBindPendingIdentityDescriptorError(object arg0) => FrameworkResources.Format(nameof (InvalidBindPendingIdentityDescriptorError), arg0);

    public static string InvalidBindPendingIdentityDescriptorError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidBindPendingIdentityDescriptorError), culture, arg0);

    public static string InvalidBindPendingIdentityEmailAddressError(object arg0) => FrameworkResources.Format(nameof (InvalidBindPendingIdentityEmailAddressError), arg0);

    public static string InvalidBindPendingIdentityEmailAddressError(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidBindPendingIdentityEmailAddressError), culture, arg0);
    }

    public static string AlreadyInitialized() => FrameworkResources.Get(nameof (AlreadyInitialized));

    public static string AlreadyInitialized(CultureInfo culture) => FrameworkResources.Get(nameof (AlreadyInitialized), culture);

    public static string ObjectIsNotInitialized() => FrameworkResources.Get(nameof (ObjectIsNotInitialized));

    public static string ObjectIsNotInitialized(CultureInfo culture) => FrameworkResources.Get(nameof (ObjectIsNotInitialized), culture);

    public static string MinServiceVersionGreaterServiceVersion() => FrameworkResources.Get(nameof (MinServiceVersionGreaterServiceVersion));

    public static string MinServiceVersionGreaterServiceVersion(CultureInfo culture) => FrameworkResources.Get(nameof (MinServiceVersionGreaterServiceVersion), culture);

    public static string ServiceNotRegistered(object arg0) => FrameworkResources.Format(nameof (ServiceNotRegistered), arg0);

    public static string ServiceNotRegistered(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceNotRegistered), culture, arg0);

    public static string NoComponentFactoryField(object arg0) => FrameworkResources.Format(nameof (NoComponentFactoryField), arg0);

    public static string NoComponentFactoryField(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NoComponentFactoryField), culture, arg0);

    public static string InvalidComponentFactoryFieldType(object arg0) => FrameworkResources.Format(nameof (InvalidComponentFactoryFieldType), arg0);

    public static string InvalidComponentFactoryFieldType(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidComponentFactoryFieldType), culture, arg0);

    public static string ComponentFactoryFieldIsNull(object arg0) => FrameworkResources.Format(nameof (ComponentFactoryFieldIsNull), arg0);

    public static string ComponentFactoryFieldIsNull(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ComponentFactoryFieldIsNull), culture, arg0);

    public static string ServiceVersionNotSupported(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (ServiceVersionNotSupported), arg0, arg1, arg2);

    public static string ServiceVersionNotSupported(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServiceVersionNotSupported), culture, arg0, arg1, arg2);
    }

    public static string ProjectDefaultTeam(object arg0) => FrameworkResources.Format(nameof (ProjectDefaultTeam), arg0);

    public static string ProjectDefaultTeam(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ProjectDefaultTeam), culture, arg0);

    public static string ProjectDefaultTeamDescription() => FrameworkResources.Get(nameof (ProjectDefaultTeamDescription));

    public static string ProjectDefaultTeamDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectDefaultTeamDescription), culture);

    public static string StandardFeaturesDescription() => FrameworkResources.Get(nameof (StandardFeaturesDescription));

    public static string StandardFeaturesDescription(CultureInfo culture) => FrameworkResources.Get(nameof (StandardFeaturesDescription), culture);

    public static string StandardFeaturesName() => FrameworkResources.Get(nameof (StandardFeaturesName));

    public static string StandardFeaturesName(CultureInfo culture) => FrameworkResources.Get(nameof (StandardFeaturesName), culture);

    public static string StandardLicenseDescription() => FrameworkResources.Get(nameof (StandardLicenseDescription));

    public static string StandardLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (StandardLicenseDescription), culture);

    public static string StandardLicenseName() => FrameworkResources.Get(nameof (StandardLicenseName));

    public static string StandardLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (StandardLicenseName), culture);

    public static string ViewMyWorkItemsFeatureDescription() => FrameworkResources.Get(nameof (ViewMyWorkItemsFeatureDescription));

    public static string ViewMyWorkItemsFeatureDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ViewMyWorkItemsFeatureDescription), culture);

    public static string ViewMyWorkItemsFeatureName() => FrameworkResources.Get(nameof (ViewMyWorkItemsFeatureName));

    public static string ViewMyWorkItemsFeatureName(CultureInfo culture) => FrameworkResources.Get(nameof (ViewMyWorkItemsFeatureName), culture);

    public static string WorkItemOnlyViewLicenseDescription() => FrameworkResources.Get(nameof (WorkItemOnlyViewLicenseDescription));

    public static string WorkItemOnlyViewLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (WorkItemOnlyViewLicenseDescription), culture);

    public static string WorkItemOnlyViewLicenseName() => FrameworkResources.Get(nameof (WorkItemOnlyViewLicenseName));

    public static string WorkItemOnlyViewLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (WorkItemOnlyViewLicenseName), culture);

    public static string LicensingPackageValidation_MissingDefaultLicense() => FrameworkResources.Get(nameof (LicensingPackageValidation_MissingDefaultLicense));

    public static string LicensingPackageValidation_MissingDefaultLicense(CultureInfo culture) => FrameworkResources.Get(nameof (LicensingPackageValidation_MissingDefaultLicense), culture);

    public static string MailServiceDisabledError() => FrameworkResources.Get(nameof (MailServiceDisabledError));

    public static string MailServiceDisabledError(CultureInfo culture) => FrameworkResources.Get(nameof (MailServiceDisabledError), culture);

    public static string DrawerExistsException(object arg0) => FrameworkResources.Format(nameof (DrawerExistsException), arg0);

    public static string DrawerExistsException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DrawerExistsException), culture, arg0);

    public static string DrawerNotFoundException(object arg0) => FrameworkResources.Format(nameof (DrawerNotFoundException), arg0);

    public static string DrawerNotFoundException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DrawerNotFoundException), culture, arg0);

    public static string ItemNotFoundException(object arg0) => FrameworkResources.Format(nameof (ItemNotFoundException), arg0);

    public static string ItemNotFoundException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ItemNotFoundException), culture, arg0);

    public static string UnexpectedItemKindException() => FrameworkResources.Get(nameof (UnexpectedItemKindException));

    public static string UnexpectedItemKindException(CultureInfo culture) => FrameworkResources.Get(nameof (UnexpectedItemKindException), culture);

    public static string PermissionStrongBoxCreateDrawer() => FrameworkResources.Get(nameof (PermissionStrongBoxCreateDrawer));

    public static string PermissionStrongBoxCreateDrawer(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionStrongBoxCreateDrawer), culture);

    public static string PermissionStrongBoxDeleteDrawer() => FrameworkResources.Get(nameof (PermissionStrongBoxDeleteDrawer));

    public static string PermissionStrongBoxDeleteDrawer(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionStrongBoxDeleteDrawer), culture);

    public static string PermissionStrongBoxAdminister() => FrameworkResources.Get(nameof (PermissionStrongBoxAdminister));

    public static string PermissionStrongBoxAdminister(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionStrongBoxAdminister), culture);

    public static string PermissionStrongBoxAddItem() => FrameworkResources.Get(nameof (PermissionStrongBoxAddItem));

    public static string PermissionStrongBoxAddItem(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionStrongBoxAddItem), culture);

    public static string PermissionStrongBoxGetItem() => FrameworkResources.Get(nameof (PermissionStrongBoxGetItem));

    public static string PermissionStrongBoxGetItem(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionStrongBoxGetItem), culture);

    public static string PermissionStrongBoxDeleteItem() => FrameworkResources.Get(nameof (PermissionStrongBoxDeleteItem));

    public static string PermissionStrongBoxDeleteItem(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionStrongBoxDeleteItem), culture);

    public static string PermissionStrongBoxAdministerDrawer() => FrameworkResources.Get(nameof (PermissionStrongBoxAdministerDrawer));

    public static string PermissionStrongBoxAdministerDrawer(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionStrongBoxAdministerDrawer), culture);

    public static string DrawerIdMalformed() => FrameworkResources.Get(nameof (DrawerIdMalformed));

    public static string DrawerIdMalformed(CultureInfo culture) => FrameworkResources.Get(nameof (DrawerIdMalformed), culture);

    public static string DrawerIdStringMissing() => FrameworkResources.Get(nameof (DrawerIdStringMissing));

    public static string DrawerIdStringMissing(CultureInfo culture) => FrameworkResources.Get(nameof (DrawerIdStringMissing), culture);

    public static string FullLicenseDescription() => FrameworkResources.Get(nameof (FullLicenseDescription));

    public static string FullLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (FullLicenseDescription), culture);

    public static string FullLicenseName() => FrameworkResources.Get(nameof (FullLicenseName));

    public static string FullLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (FullLicenseName), culture);

    public static string LimitedLicenseDescription() => FrameworkResources.Get(nameof (LimitedLicenseDescription));

    public static string LimitedLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (LimitedLicenseDescription), culture);

    public static string LimitedLicenseName() => FrameworkResources.Get(nameof (LimitedLicenseName));

    public static string LimitedLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (LimitedLicenseName), culture);

    public static string CanNotDeleteDefaultTeam(object arg0) => FrameworkResources.Format(nameof (CanNotDeleteDefaultTeam), arg0);

    public static string CanNotDeleteDefaultTeam(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CanNotDeleteDefaultTeam), culture, arg0);

    public static string NotATeamException(object arg0) => FrameworkResources.Format(nameof (NotATeamException), arg0);

    public static string NotATeamException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NotATeamException), culture, arg0);

    public static string DataSourceNotInConnectionStringError() => FrameworkResources.Get(nameof (DataSourceNotInConnectionStringError));

    public static string DataSourceNotInConnectionStringError(CultureInfo culture) => FrameworkResources.Get(nameof (DataSourceNotInConnectionStringError), culture);

    public static string InitialCatalogNotInConnectionStringError() => FrameworkResources.Get(nameof (InitialCatalogNotInConnectionStringError));

    public static string InitialCatalogNotInConnectionStringError(CultureInfo culture) => FrameworkResources.Get(nameof (InitialCatalogNotInConnectionStringError), culture);

    public static string EmailNotificationHashVerificationFailed() => FrameworkResources.Get(nameof (EmailNotificationHashVerificationFailed));

    public static string EmailNotificationHashVerificationFailed(CultureInfo culture) => FrameworkResources.Get(nameof (EmailNotificationHashVerificationFailed), culture);

    public static string EmailNotificationPeferredEmailMismatch() => FrameworkResources.Get(nameof (EmailNotificationPeferredEmailMismatch));

    public static string EmailNotificationPeferredEmailMismatch(CultureInfo culture) => FrameworkResources.Get(nameof (EmailNotificationPeferredEmailMismatch), culture);

    public static string EmailNotificationWrongUserSignedIn() => FrameworkResources.Get(nameof (EmailNotificationWrongUserSignedIn));

    public static string EmailNotificationWrongUserSignedIn(CultureInfo culture) => FrameworkResources.Get(nameof (EmailNotificationWrongUserSignedIn), culture);

    public static string InvalidTeamName(object arg0) => FrameworkResources.Format(nameof (InvalidTeamName), arg0);

    public static string InvalidTeamName(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidTeamName), culture, arg0);

    public static string SmtpCertificateNotFound(object arg0) => FrameworkResources.Format(nameof (SmtpCertificateNotFound), arg0);

    public static string SmtpCertificateNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SmtpCertificateNotFound), culture, arg0);

    public static string SmtpPrivateKeyNotInstalled(object arg0) => FrameworkResources.Format(nameof (SmtpPrivateKeyNotInstalled), arg0);

    public static string SmtpPrivateKeyNotInstalled(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SmtpPrivateKeyNotInstalled), culture, arg0);

    public static string InvalidEventData() => FrameworkResources.Get(nameof (InvalidEventData));

    public static string InvalidEventData(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidEventData), culture);

    public static string JobDefinitionNotFoundError(object arg0) => FrameworkResources.Format(nameof (JobDefinitionNotFoundError), arg0);

    public static string JobDefinitionNotFoundError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobDefinitionNotFoundError), culture, arg0);

    public static string FailedToAcquireLockError() => FrameworkResources.Get(nameof (FailedToAcquireLockError));

    public static string FailedToAcquireLockError(CultureInfo culture) => FrameworkResources.Get(nameof (FailedToAcquireLockError), culture);

    public static string InvalidNameNotRecognized(object arg0) => FrameworkResources.Format(nameof (InvalidNameNotRecognized), arg0);

    public static string InvalidNameNotRecognized(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidNameNotRecognized), culture, arg0);

    public static string EnumerationNoneArgumentError(object arg0) => FrameworkResources.Format(nameof (EnumerationNoneArgumentError), arg0);

    public static string EnumerationNoneArgumentError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (EnumerationNoneArgumentError), culture, arg0);

    public static string PermissionDiagnosticRead() => FrameworkResources.Get(nameof (PermissionDiagnosticRead));

    public static string PermissionDiagnosticRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionDiagnosticRead), culture);

    public static string PermissionDiagnosticTroubleshoot() => FrameworkResources.Get(nameof (PermissionDiagnosticTroubleshoot));

    public static string PermissionDiagnosticTroubleshoot(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionDiagnosticTroubleshoot), culture);

    public static string PermissionDiagnosticWrite() => FrameworkResources.Get(nameof (PermissionDiagnosticWrite));

    public static string PermissionDiagnosticWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionDiagnosticWrite), culture);

    public static string OAuthTokenNotFoundError(object arg0) => FrameworkResources.Format(nameof (OAuthTokenNotFoundError), arg0);

    public static string OAuthTokenNotFoundError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (OAuthTokenNotFoundError), culture, arg0);

    public static string OAuthApplicationNotFoundError(object arg0) => FrameworkResources.Format(nameof (OAuthApplicationNotFoundError), arg0);

    public static string OAuthApplicationNotFoundError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (OAuthApplicationNotFoundError), culture, arg0);

    public static string OAuthApplicationExistsError(object arg0) => FrameworkResources.Format(nameof (OAuthApplicationExistsError), arg0);

    public static string OAuthApplicationExistsError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (OAuthApplicationExistsError), culture, arg0);

    public static string OAuthDisabledByServiceHost(object arg0) => FrameworkResources.Format(nameof (OAuthDisabledByServiceHost), arg0);

    public static string OAuthDisabledByServiceHost(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (OAuthDisabledByServiceHost), culture, arg0);

    public static string LockTimeoutError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (LockTimeoutError), arg0, arg1, arg2);

    public static string LockTimeoutError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (LockTimeoutError), culture, arg0, arg1, arg2);
    }

    public static string OAuthFileIdDoesNotMatch() => FrameworkResources.Get(nameof (OAuthFileIdDoesNotMatch));

    public static string OAuthFileIdDoesNotMatch(CultureInfo culture) => FrameworkResources.Get(nameof (OAuthFileIdDoesNotMatch), culture);

    public static string ProcessTemplateTypesDoNotMatch() => FrameworkResources.Get(nameof (ProcessTemplateTypesDoNotMatch));

    public static string ProcessTemplateTypesDoNotMatch(CultureInfo culture) => FrameworkResources.Get(nameof (ProcessTemplateTypesDoNotMatch), culture);

    public static string DatabasePartitionNotFound(object arg0) => FrameworkResources.Format(nameof (DatabasePartitionNotFound), arg0);

    public static string DatabasePartitionNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePartitionNotFound), culture, arg0);

    public static string PermissionWebAccessFullAccess() => FrameworkResources.Get(nameof (PermissionWebAccessFullAccess));

    public static string PermissionWebAccessFullAccess(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionWebAccessFullAccess), culture);

    public static string PermissionWebAccessModify() => FrameworkResources.Get(nameof (PermissionWebAccessModify));

    public static string PermissionWebAccessModify(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionWebAccessModify), culture);

    public static string RemoveSelfFromAdminGroupError(object arg0) => FrameworkResources.Format(nameof (RemoveSelfFromAdminGroupError), arg0);

    public static string RemoveSelfFromAdminGroupError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RemoveSelfFromAdminGroupError), culture, arg0);

    public static string ConnectedServicesServiceNotSupported() => FrameworkResources.Get(nameof (ConnectedServicesServiceNotSupported));

    public static string ConnectedServicesServiceNotSupported(CultureInfo culture) => FrameworkResources.Get(nameof (ConnectedServicesServiceNotSupported), culture);

    public static string SendingOversizedEmailIsRejected(object arg0) => FrameworkResources.Format(nameof (SendingOversizedEmailIsRejected), arg0);

    public static string SendingOversizedEmailIsRejected(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SendingOversizedEmailIsRejected), culture, arg0);

    public static string DatabasePartitionIdNotFound(object arg0) => FrameworkResources.Format(nameof (DatabasePartitionIdNotFound), arg0);

    public static string DatabasePartitionIdNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePartitionIdNotFound), culture, arg0);

    public static string DatabaseAlreadyExists(object arg0, object arg1) => FrameworkResources.Format(nameof (DatabaseAlreadyExists), arg0, arg1);

    public static string DatabaseAlreadyExists(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseAlreadyExists), culture, arg0, arg1);

    public static string NoDatabasePartitionFound(object arg0) => FrameworkResources.Format(nameof (NoDatabasePartitionFound), arg0);

    public static string NoDatabasePartitionFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NoDatabasePartitionFound), culture, arg0);

    public static string MultiplePartitionsNotSupportedError(object arg0) => FrameworkResources.Format(nameof (MultiplePartitionsNotSupportedError), arg0);

    public static string MultiplePartitionsNotSupportedError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MultiplePartitionsNotSupportedError), culture, arg0);

    public static string AcquireDatabasePartitionExceptionMessage(object arg0) => FrameworkResources.Format(nameof (AcquireDatabasePartitionExceptionMessage), arg0);

    public static string AcquireDatabasePartitionExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AcquireDatabasePartitionExceptionMessage), culture, arg0);

    public static string DatabaseNotFoundExceptionMessage(object arg0) => FrameworkResources.Format(nameof (DatabaseNotFoundExceptionMessage), arg0);

    public static string DatabaseNotFoundExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseNotFoundExceptionMessage), culture, arg0);

    public static string DatabasePoolNotFoundExceptionMessage(object arg0) => FrameworkResources.Format(nameof (DatabasePoolNotFoundExceptionMessage), arg0);

    public static string DatabasePoolNotFoundExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePoolNotFoundExceptionMessage), culture, arg0);

    public static string DatabasePoolExistsError(object arg0) => FrameworkResources.Format(nameof (DatabasePoolExistsError), arg0);

    public static string DatabasePoolExistsError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePoolExistsError), culture, arg0);

    public static string DatabaseAlreadyRegisteredError(object arg0) => FrameworkResources.Format(nameof (DatabaseAlreadyRegisteredError), arg0);

    public static string DatabaseAlreadyRegisteredError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseAlreadyRegisteredError), culture, arg0);

    public static string DatabasePoolCannotBeDeletedError(object arg0) => FrameworkResources.Format(nameof (DatabasePoolCannotBeDeletedError), arg0);

    public static string DatabasePoolCannotBeDeletedError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePoolCannotBeDeletedError), culture, arg0);

    public static string DatabaseCopyError(object arg0, object arg1, object arg2, object arg3) => FrameworkResources.Format(nameof (DatabaseCopyError), arg0, arg1, arg2, arg3);

    public static string DatabaseCopyError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DatabaseCopyError), culture, arg0, arg1, arg2, arg3);
    }

    public static string DataTierNotFoundError(object arg0) => FrameworkResources.Format(nameof (DataTierNotFoundError), arg0);

    public static string DataTierNotFoundError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DataTierNotFoundError), culture, arg0);

    public static string DatabaseIsNotBeingCopied(object arg0, object arg1) => FrameworkResources.Format(nameof (DatabaseIsNotBeingCopied), arg0, arg1);

    public static string DatabaseIsNotBeingCopied(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseIsNotBeingCopied), culture, arg0, arg1);

    public static string DatabaseCopyTimeoutError(object arg0, object arg1) => FrameworkResources.Format(nameof (DatabaseCopyTimeoutError), arg0, arg1);

    public static string DatabaseCopyTimeoutError(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseCopyTimeoutError), culture, arg0, arg1);

    public static string SourceDestinationDatabaseNameMustDiffer(object arg0) => FrameworkResources.Format(nameof (SourceDestinationDatabaseNameMustDiffer), arg0);

    public static string SourceDestinationDatabaseNameMustDiffer(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SourceDestinationDatabaseNameMustDiffer), culture, arg0);

    public static string ApplicationStoppingForcefully(object arg0) => FrameworkResources.Format(nameof (ApplicationStoppingForcefully), arg0);

    public static string ApplicationStoppingForcefully(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ApplicationStoppingForcefully), culture, arg0);

    public static string JobAgentEnteringLifecycleStage(object arg0) => FrameworkResources.Format(nameof (JobAgentEnteringLifecycleStage), arg0);

    public static string JobAgentEnteringLifecycleStage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobAgentEnteringLifecycleStage), culture, arg0);

    public static string JobAgentConfigDbSettingInvalid() => FrameworkResources.Get(nameof (JobAgentConfigDbSettingInvalid));

    public static string JobAgentConfigDbSettingInvalid(CultureInfo culture) => FrameworkResources.Get(nameof (JobAgentConfigDbSettingInvalid), culture);

    public static string DatabaseSplitFailed(object arg0) => FrameworkResources.Format(nameof (DatabaseSplitFailed), arg0);

    public static string DatabaseSplitFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseSplitFailed), culture, arg0);

    public static string DatabasePoolFullError(object arg0) => FrameworkResources.Format(nameof (DatabasePoolFullError), arg0);

    public static string DatabasePoolFullError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePoolFullError), culture, arg0);

    public static string RemoteSqlServerNotSupportedWhenNotJoinedToDomain() => FrameworkResources.Get(nameof (RemoteSqlServerNotSupportedWhenNotJoinedToDomain));

    public static string RemoteSqlServerNotSupportedWhenNotJoinedToDomain(CultureInfo culture) => FrameworkResources.Get(nameof (RemoteSqlServerNotSupportedWhenNotJoinedToDomain), culture);

    public static string DatabasePartitionCannotBeDeleted(object arg0) => FrameworkResources.Format(nameof (DatabasePartitionCannotBeDeleted), arg0);

    public static string DatabasePartitionCannotBeDeleted(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePartitionCannotBeDeleted), culture, arg0);

    public static string DatabasePartitionCannotBeCreated(object arg0) => FrameworkResources.Format(nameof (DatabasePartitionCannotBeCreated), arg0);

    public static string DatabasePartitionCannotBeCreated(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePartitionCannotBeCreated), culture, arg0);

    public static string DatabasePartitionInUse(object arg0) => FrameworkResources.Format(nameof (DatabasePartitionInUse), arg0);

    public static string DatabasePartitionInUse(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePartitionInUse), culture, arg0);

    public static string DatabaseDetachedError() => FrameworkResources.Get(nameof (DatabaseDetachedError));

    public static string DatabaseDetachedError(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseDetachedError), culture);

    public static string MajorVersionIsInvalid(object arg0) => FrameworkResources.Format(nameof (MajorVersionIsInvalid), arg0);

    public static string MajorVersionIsInvalid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MajorVersionIsInvalid), culture, arg0);

    public static string MilestoneIsInvalid(object arg0) => FrameworkResources.Format(nameof (MilestoneIsInvalid), arg0);

    public static string MilestoneIsInvalid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MilestoneIsInvalid), culture, arg0);

    public static string ServiceLevelIsInvalid(object arg0) => FrameworkResources.Format(nameof (ServiceLevelIsInvalid), arg0);

    public static string ServiceLevelIsInvalid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceLevelIsInvalid), culture, arg0);

    public static string ServiceLevelListIsInvalid(object arg0) => FrameworkResources.Format(nameof (ServiceLevelListIsInvalid), arg0);

    public static string ServiceLevelListIsInvalid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceLevelListIsInvalid), culture, arg0);

    public static string ServiceVersionAreaNotRegistered(object arg0) => FrameworkResources.Format(nameof (ServiceVersionAreaNotRegistered), arg0);

    public static string ServiceVersionAreaNotRegistered(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceVersionAreaNotRegistered), culture, arg0);

    public static string StepDataNotRequired() => FrameworkResources.Get(nameof (StepDataNotRequired));

    public static string StepDataNotRequired(CultureInfo culture) => FrameworkResources.Get(nameof (StepDataNotRequired), culture);

    public static string DeleteSelfError() => FrameworkResources.Get(nameof (DeleteSelfError));

    public static string DeleteSelfError(CultureInfo culture) => FrameworkResources.Get(nameof (DeleteSelfError), culture);

    public static string UpdatedConnectionStringsAction(object arg0) => FrameworkResources.Format(nameof (UpdatedConnectionStringsAction), arg0);

    public static string UpdatedConnectionStringsAction(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UpdatedConnectionStringsAction), culture, arg0);

    public static string IncorrectRequestContextForJobSourceError(object arg0, object arg1) => FrameworkResources.Format(nameof (IncorrectRequestContextForJobSourceError), arg0, arg1);

    public static string IncorrectRequestContextForJobSourceError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (IncorrectRequestContextForJobSourceError), culture, arg0, arg1);
    }

    public static string InitializingJobRunnerError(object arg0) => FrameworkResources.Format(nameof (InitializingJobRunnerError), arg0);

    public static string InitializingJobRunnerError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InitializingJobRunnerError), culture, arg0);

    public static string ServicingOperationNotDefined(object arg0) => FrameworkResources.Format(nameof (ServicingOperationNotDefined), arg0);

    public static string ServicingOperationNotDefined(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingOperationNotDefined), culture, arg0);

    public static string ServicingConnectionStringNotFound(object arg0) => FrameworkResources.Format(nameof (ServicingConnectionStringNotFound), arg0);

    public static string ServicingConnectionStringNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingConnectionStringNotFound), culture, arg0);

    public static string DeletePrivateArtifactsStepDescription() => FrameworkResources.Get(nameof (DeletePrivateArtifactsStepDescription));

    public static string DeletePrivateArtifactsStepDescription(CultureInfo culture) => FrameworkResources.Get(nameof (DeletePrivateArtifactsStepDescription), culture);

    public static string DatabaseNotFoundByNameMessage(object arg0) => FrameworkResources.Format(nameof (DatabaseNotFoundByNameMessage), arg0);

    public static string DatabaseNotFoundByNameMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseNotFoundByNameMessage), culture, arg0);

    public static string MessageBusNotFoundException(object arg0) => FrameworkResources.Format(nameof (MessageBusNotFoundException), arg0);

    public static string MessageBusNotFoundException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MessageBusNotFoundException), culture, arg0);

    public static string MessageBusProviderNotConfiguredException() => FrameworkResources.Get(nameof (MessageBusProviderNotConfiguredException));

    public static string MessageBusProviderNotConfiguredException(CultureInfo culture) => FrameworkResources.Get(nameof (MessageBusProviderNotConfiguredException), culture);

    public static string IgnoreDormancyProhibitedError(object arg0) => FrameworkResources.Format(nameof (IgnoreDormancyProhibitedError), arg0);

    public static string IgnoreDormancyProhibitedError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IgnoreDormancyProhibitedError), culture, arg0);

    public static string ServiceBusClaimDescription(object arg0, object arg1) => FrameworkResources.Format(nameof (ServiceBusClaimDescription), arg0, arg1);

    public static string ServiceBusClaimDescription(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceBusClaimDescription), culture, arg0, arg1);

    public static string PermissionWarehouseAdminister() => FrameworkResources.Get(nameof (PermissionWarehouseAdminister));

    public static string PermissionWarehouseAdminister(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionWarehouseAdminister), culture);

    public static string CannotDetermineRootDomain() => FrameworkResources.Get(nameof (CannotDetermineRootDomain));

    public static string CannotDetermineRootDomain(CultureInfo culture) => FrameworkResources.Get(nameof (CannotDetermineRootDomain), culture);

    public static string ExtensionNotFoundMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (ExtensionNotFoundMessage), arg0, arg1);

    public static string ExtensionNotFoundMessage(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ExtensionNotFoundMessage), culture, arg0, arg1);

    public static string ConnectedServiceNotFound(object arg0) => FrameworkResources.Format(nameof (ConnectedServiceNotFound), arg0);

    public static string ConnectedServiceNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ConnectedServiceNotFound), culture, arg0);

    public static string MessageBusSubscriberActionNotFoundException(object arg0) => FrameworkResources.Format(nameof (MessageBusSubscriberActionNotFoundException), arg0);

    public static string MessageBusSubscriberActionNotFoundException(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MessageBusSubscriberActionNotFoundException), culture, arg0);
    }

    public static string ExtensibleServiceTypeNotRegistered(object arg0) => FrameworkResources.Format(nameof (ExtensibleServiceTypeNotRegistered), arg0);

    public static string ExtensibleServiceTypeNotRegistered(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ExtensibleServiceTypeNotRegistered), culture, arg0);

    public static string ServicingOperationGroupUnknownError(object arg0, object arg1) => FrameworkResources.Format(nameof (ServicingOperationGroupUnknownError), arg0, arg1);

    public static string ServicingOperationGroupUnknownError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServicingOperationGroupUnknownError), culture, arg0, arg1);
    }

    public static string ContainerItemWriteAccessDenied(object arg0) => FrameworkResources.Format(nameof (ContainerItemWriteAccessDenied), arg0);

    public static string ContainerItemWriteAccessDenied(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ContainerItemWriteAccessDenied), culture, arg0);

    public static string FileContainerItemsControllerPathNotAllowed() => FrameworkResources.Get(nameof (FileContainerItemsControllerPathNotAllowed));

    public static string FileContainerItemsControllerPathNotAllowed(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerItemsControllerPathNotAllowed), culture);

    public static string FileContainerUploadControllerRangeHeaderError() => FrameworkResources.Get(nameof (FileContainerUploadControllerRangeHeaderError));

    public static string FileContainerUploadControllerRangeHeaderError(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerUploadControllerRangeHeaderError), culture);

    public static string FileContainerUploadContentError() => FrameworkResources.Get(nameof (FileContainerUploadContentError));

    public static string FileContainerUploadContentError(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerUploadContentError), culture);

    public static string FileContainerCannotDownloadPartialFile() => FrameworkResources.Get(nameof (FileContainerCannotDownloadPartialFile));

    public static string FileContainerCannotDownloadPartialFile(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerCannotDownloadPartialFile), culture);

    public static string ExtensibleServiceTypeNotValid(object arg0, object arg1) => FrameworkResources.Format(nameof (ExtensibleServiceTypeNotValid), arg0, arg1);

    public static string ExtensibleServiceTypeNotValid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ExtensibleServiceTypeNotValid), culture, arg0, arg1);
    }

    public static string ExtensibleServiceTypeInEnvironmentVariableNotFound(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (ExtensibleServiceTypeInEnvironmentVariableNotFound), arg0, arg1, arg2);
    }

    public static string ExtensibleServiceTypeInEnvironmentVariableNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ExtensibleServiceTypeInEnvironmentVariableNotFound), culture, arg0, arg1, arg2);
    }

    public static string ServiceGroupName() => FrameworkResources.Get(nameof (ServiceGroupName));

    public static string ServiceGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (ServiceGroupName), culture);

    public static string ServiceGroupDescription() => FrameworkResources.Get(nameof (ServiceGroupDescription));

    public static string ServiceGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ServiceGroupDescription), culture);

    public static string AdministratorsGroupName() => FrameworkResources.Get(nameof (AdministratorsGroupName));

    public static string AdministratorsGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (AdministratorsGroupName), culture);

    public static string AdministratorsGroupDescription() => FrameworkResources.Get(nameof (AdministratorsGroupDescription));

    public static string AdministratorsGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (AdministratorsGroupDescription), culture);

    public static string AnonymousPrincipalName() => FrameworkResources.Get(nameof (AnonymousPrincipalName));

    public static string AnonymousPrincipalName(CultureInfo culture) => FrameworkResources.Get(nameof (AnonymousPrincipalName), culture);

    public static string AnonymousPrincipalDescription() => FrameworkResources.Get(nameof (AnonymousPrincipalDescription));

    public static string AnonymousPrincipalDescription(CultureInfo culture) => FrameworkResources.Get(nameof (AnonymousPrincipalDescription), culture);

    public static string ValidUsersGroupName() => FrameworkResources.Get(nameof (ValidUsersGroupName));

    public static string ValidUsersGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (ValidUsersGroupName), culture);

    public static string ValidUsersGroupDescription() => FrameworkResources.Get(nameof (ValidUsersGroupDescription));

    public static string ValidUsersGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ValidUsersGroupDescription), culture);

    public static string ProjectValidUsersGroupName() => FrameworkResources.Get(nameof (ProjectValidUsersGroupName));

    public static string ProjectValidUsersGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectValidUsersGroupName), culture);

    public static string ProjectValidUsersGroupDescription() => FrameworkResources.Get(nameof (ProjectValidUsersGroupDescription));

    public static string ProjectValidUsersGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectValidUsersGroupDescription), culture);

    public static string AlternateCredentialsDisabledSubject() => FrameworkResources.Get(nameof (AlternateCredentialsDisabledSubject));

    public static string AlternateCredentialsDisabledSubject(CultureInfo culture) => FrameworkResources.Get(nameof (AlternateCredentialsDisabledSubject), culture);

    public static string PasswordChangedSubject() => FrameworkResources.Get(nameof (PasswordChangedSubject));

    public static string PasswordChangedSubject(CultureInfo culture) => FrameworkResources.Get(nameof (PasswordChangedSubject), culture);

    public static string AlternateLoginRestriction() => FrameworkResources.Get(nameof (AlternateLoginRestriction));

    public static string AlternateLoginRestriction(CultureInfo culture) => FrameworkResources.Get(nameof (AlternateLoginRestriction), culture);

    public static string BasicAuthPasswordChangeLimitError(object arg0, object arg1) => FrameworkResources.Format(nameof (BasicAuthPasswordChangeLimitError), arg0, arg1);

    public static string BasicAuthPasswordChangeLimitError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (BasicAuthPasswordChangeLimitError), culture, arg0, arg1);
    }

    public static string BasicAuthPasswordInvalid() => FrameworkResources.Get(nameof (BasicAuthPasswordInvalid));

    public static string BasicAuthPasswordInvalid(CultureInfo culture) => FrameworkResources.Get(nameof (BasicAuthPasswordInvalid), culture);

    public static string CreateAccountFailed(object arg0) => FrameworkResources.Format(nameof (CreateAccountFailed), arg0);

    public static string CreateAccountFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CreateAccountFailed), culture, arg0);

    public static string FileContainerContentForFolderError(object arg0) => FrameworkResources.Format(nameof (FileContainerContentForFolderError), arg0);

    public static string FileContainerContentForFolderError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FileContainerContentForFolderError), culture, arg0);

    public static string FileContainerGZipNotSupported() => FrameworkResources.Get(nameof (FileContainerGZipNotSupported));

    public static string FileContainerGZipNotSupported(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerGZipNotSupported), culture);

    public static string FileContainerContentAlreadyComplete() => FrameworkResources.Get(nameof (FileContainerContentAlreadyComplete));

    public static string FileContainerContentAlreadyComplete(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerContentAlreadyComplete), culture);

    public static string FileContainerContentIdNotSeekable() => FrameworkResources.Get(nameof (FileContainerContentIdNotSeekable));

    public static string FileContainerContentIdNotSeekable(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerContentIdNotSeekable), culture);

    public static string FileContainerContentCalculationBlockSize(object arg0) => FrameworkResources.Format(nameof (FileContainerContentCalculationBlockSize), arg0);

    public static string FileContainerContentCalculationBlockSize(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FileContainerContentCalculationBlockSize), culture, arg0);

    public static string FileContainerContentCalculationNoSeed() => FrameworkResources.Get(nameof (FileContainerContentCalculationNoSeed));

    public static string FileContainerContentCalculationNoSeed(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerContentCalculationNoSeed), culture);

    public static string FileContainerUploadItemDoesNotExist() => FrameworkResources.Get(nameof (FileContainerUploadItemDoesNotExist));

    public static string FileContainerUploadItemDoesNotExist(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerUploadItemDoesNotExist), culture);

    public static string FileContainerUploadContentIdDoesNotMatch() => FrameworkResources.Get(nameof (FileContainerUploadContentIdDoesNotMatch));

    public static string FileContainerUploadContentIdDoesNotMatch(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerUploadContentIdDoesNotMatch), culture);

    public static string FileContainerContentIdMalformed() => FrameworkResources.Get(nameof (FileContainerContentIdMalformed));

    public static string FileContainerContentIdMalformed(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerContentIdMalformed), culture);

    public static string FileContainerNoItems() => FrameworkResources.Get(nameof (FileContainerNoItems));

    public static string FileContainerNoItems(CultureInfo culture) => FrameworkResources.Get(nameof (FileContainerNoItems), culture);

    public static string IdentityValidationHandlerError(object arg0) => FrameworkResources.Format(nameof (IdentityValidationHandlerError), arg0);

    public static string IdentityValidationHandlerError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IdentityValidationHandlerError), culture, arg0);

    public static string RequestDisabled() => FrameworkResources.Get(nameof (RequestDisabled));

    public static string RequestDisabled(CultureInfo culture) => FrameworkResources.Get(nameof (RequestDisabled), culture);

    public static string RequestDisabledDetails(object arg0, object arg1) => FrameworkResources.Format(nameof (RequestDisabledDetails), arg0, arg1);

    public static string RequestDisabledDetails(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (RequestDisabledDetails), culture, arg0, arg1);

    public static string RelyingPartyExistsWithConflictingSigningKey(object arg0) => FrameworkResources.Format(nameof (RelyingPartyExistsWithConflictingSigningKey), arg0);

    public static string RelyingPartyExistsWithConflictingSigningKey(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (RelyingPartyExistsWithConflictingSigningKey), culture, arg0);
    }

    public static string KpiExistsException(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (KpiExistsException), arg0, arg1, arg2);

    public static string KpiExistsException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (KpiExistsException), culture, arg0, arg1, arg2);
    }

    public static string KpiNotFoundException(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (KpiNotFoundException), arg0, arg1, arg2);

    public static string KpiNotFoundException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (KpiNotFoundException), culture, arg0, arg1, arg2);
    }

    public static string InvalidKpiStateException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (InvalidKpiStateException), arg0, arg1, arg2, arg3);
    }

    public static string InvalidKpiStateException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidKpiStateException), culture, arg0, arg1, arg2, arg3);
    }

    public static string KpiMetricMessage(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6)
    {
      return FrameworkResources.Format(nameof (KpiMetricMessage), arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public static string KpiMetricMessage(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (KpiMetricMessage), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public static string AccountSuspendedReason() => FrameworkResources.Get(nameof (AccountSuspendedReason));

    public static string AccountSuspendedReason(CultureInfo culture) => FrameworkResources.Get(nameof (AccountSuspendedReason), culture);

    public static string CollectionStoppedReason() => FrameworkResources.Get(nameof (CollectionStoppedReason));

    public static string CollectionStoppedReason(CultureInfo culture) => FrameworkResources.Get(nameof (CollectionStoppedReason), culture);

    public static string FieldReadOnly(object arg0) => FrameworkResources.Format(nameof (FieldReadOnly), arg0);

    public static string FieldReadOnly(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FieldReadOnly), culture, arg0);

    public static string GroupMemberWithNoId(object arg0, object arg1) => FrameworkResources.Format(nameof (GroupMemberWithNoId), arg0, arg1);

    public static string GroupMemberWithNoId(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (GroupMemberWithNoId), culture, arg0, arg1);

    public static string NotificationHeartbeatMessage(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (NotificationHeartbeatMessage), arg0, arg1, arg2);

    public static string NotificationHeartbeatMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (NotificationHeartbeatMessage), culture, arg0, arg1, arg2);
    }

    public static string WcfCannotFindClientCert(object arg0, object arg1) => FrameworkResources.Format(nameof (WcfCannotFindClientCert), arg0, arg1);

    public static string WcfCannotFindClientCert(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (WcfCannotFindClientCert), culture, arg0, arg1);

    public static string WcfInvalidResourceName(object arg0) => FrameworkResources.Format(nameof (WcfInvalidResourceName), arg0);

    public static string WcfInvalidResourceName(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (WcfInvalidResourceName), culture, arg0);

    public static string FeatureAvailabilityAccountAdminUsersGroupName() => FrameworkResources.Get(nameof (FeatureAvailabilityAccountAdminUsersGroupName));

    public static string FeatureAvailabilityAccountAdminUsersGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (FeatureAvailabilityAccountAdminUsersGroupName), culture);

    public static string FeatureAvailabilityAdminUsersGroupName() => FrameworkResources.Get(nameof (FeatureAvailabilityAdminUsersGroupName));

    public static string FeatureAvailabilityAdminUsersGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (FeatureAvailabilityAdminUsersGroupName), culture);

    public static string FeatureAvailabilityReadersUsersGroupName() => FrameworkResources.Get(nameof (FeatureAvailabilityReadersUsersGroupName));

    public static string FeatureAvailabilityReadersUsersGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (FeatureAvailabilityReadersUsersGroupName), culture);

    public static string ViewAllFeatureFlags() => FrameworkResources.Get(nameof (ViewAllFeatureFlags));

    public static string ViewAllFeatureFlags(CultureInfo culture) => FrameworkResources.Get(nameof (ViewAllFeatureFlags), culture);

    public static string ViewFeatureFlagByName() => FrameworkResources.Get(nameof (ViewFeatureFlagByName));

    public static string ViewFeatureFlagByName(CultureInfo culture) => FrameworkResources.Get(nameof (ViewFeatureFlagByName), culture);

    public static string EditFeatureFlags() => FrameworkResources.Get(nameof (EditFeatureFlags));

    public static string EditFeatureFlags(CultureInfo culture) => FrameworkResources.Get(nameof (EditFeatureFlags), culture);

    public static string AccessCheckNoNamespaceFound(object arg0) => FrameworkResources.Format(nameof (AccessCheckNoNamespaceFound), arg0);

    public static string AccessCheckNoNamespaceFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AccessCheckNoNamespaceFound), culture, arg0);

    public static string UnknownXmlAttributeError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (UnknownXmlAttributeError), arg0, arg1, arg2);

    public static string UnknownXmlAttributeError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UnknownXmlAttributeError), culture, arg0, arg1, arg2);
    }

    public static string UnknownXmlElementError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (UnknownXmlElementError), arg0, arg1, arg2);

    public static string UnknownXmlElementError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UnknownXmlElementError), culture, arg0, arg1, arg2);
    }

    public static string ErrorDeserializingStepGroup(object arg0) => FrameworkResources.Format(nameof (ErrorDeserializingStepGroup), arg0);

    public static string ErrorDeserializingStepGroup(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ErrorDeserializingStepGroup), culture, arg0);

    public static string UnexpectedHttpStatusCode(object arg0, object arg1) => FrameworkResources.Format(nameof (UnexpectedHttpStatusCode), arg0, arg1);

    public static string UnexpectedHttpStatusCode(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (UnexpectedHttpStatusCode), culture, arg0, arg1);

    public static string CannotExtractFilenameFromFileContainerPath(object arg0) => FrameworkResources.Format(nameof (CannotExtractFilenameFromFileContainerPath), arg0);

    public static string CannotExtractFilenameFromFileContainerPath(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CannotExtractFilenameFromFileContainerPath), culture, arg0);
    }

    public static string FeatureDisabledError() => FrameworkResources.Get(nameof (FeatureDisabledError));

    public static string FeatureDisabledError(CultureInfo culture) => FrameworkResources.Get(nameof (FeatureDisabledError), culture);

    public static string OmitScopeIndexForKpiError() => FrameworkResources.Get(nameof (OmitScopeIndexForKpiError));

    public static string OmitScopeIndexForKpiError(CultureInfo culture) => FrameworkResources.Get(nameof (OmitScopeIndexForKpiError), culture);

    public static string EventIdNotSuppliedError() => FrameworkResources.Get(nameof (EventIdNotSuppliedError));

    public static string EventIdNotSuppliedError(CultureInfo culture) => FrameworkResources.Get(nameof (EventIdNotSuppliedError), culture);

    public static string EventIdResolutionError(object arg0) => FrameworkResources.Format(nameof (EventIdResolutionError), arg0);

    public static string EventIdResolutionError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (EventIdResolutionError), culture, arg0);

    public static string SDKEventIdRangeError(object arg0, object arg1, object arg2, object arg3) => FrameworkResources.Format(nameof (SDKEventIdRangeError), arg0, arg1, arg2, arg3);

    public static string SDKEventIdRangeError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SDKEventIdRangeError), culture, arg0, arg1, arg2, arg3);
    }

    public static string SDKConsumerEventIdRangeError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (SDKConsumerEventIdRangeError), arg0, arg1, arg2, arg3);
    }

    public static string SDKConsumerEventIdRangeError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SDKConsumerEventIdRangeError), culture, arg0, arg1, arg2, arg3);
    }

    public static string SigningCertificateNotFoundException(object arg0) => FrameworkResources.Format(nameof (SigningCertificateNotFoundException), arg0);

    public static string SigningCertificateNotFoundException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SigningCertificateNotFoundException), culture, arg0);

    public static string TenantIdNotFoundException() => FrameworkResources.Get(nameof (TenantIdNotFoundException));

    public static string TenantIdNotFoundException(CultureInfo culture) => FrameworkResources.Get(nameof (TenantIdNotFoundException), culture);

    public static string TokenSigningAlgorithmInvalidException(object arg0) => FrameworkResources.Format(nameof (TokenSigningAlgorithmInvalidException), arg0);

    public static string TokenSigningAlgorithmInvalidException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (TokenSigningAlgorithmInvalidException), culture, arg0);

    public static string MissingLicenseException(object arg0) => FrameworkResources.Format(nameof (MissingLicenseException), arg0);

    public static string MissingLicenseException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MissingLicenseException), culture, arg0);

    public static string CounterNameNotPopulatedError(object arg0) => FrameworkResources.Format(nameof (CounterNameNotPopulatedError), arg0);

    public static string CounterNameNotPopulatedError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CounterNameNotPopulatedError), culture, arg0);

    public static string NoDataspaceProvidedWhenIsolated() => FrameworkResources.Get(nameof (NoDataspaceProvidedWhenIsolated));

    public static string NoDataspaceProvidedWhenIsolated(CultureInfo culture) => FrameworkResources.Get(nameof (NoDataspaceProvidedWhenIsolated), culture);

    public static string NoncompliantUserError(object arg0, object arg1) => FrameworkResources.Format(nameof (NoncompliantUserError), arg0, arg1);

    public static string NoncompliantUserError(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (NoncompliantUserError), culture, arg0, arg1);

    public static string ErrorRegisteringWebApplication() => FrameworkResources.Get(nameof (ErrorRegisteringWebApplication));

    public static string ErrorRegisteringWebApplication(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorRegisteringWebApplication), culture);

    public static string RegisterErrorMachineNodeNotFound(object arg0) => FrameworkResources.Format(nameof (RegisterErrorMachineNodeNotFound), arg0);

    public static string RegisterErrorMachineNodeNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RegisterErrorMachineNodeNotFound), culture, arg0);

    public static string RegisterErrorInstanceNodeNotFound() => FrameworkResources.Get(nameof (RegisterErrorInstanceNodeNotFound));

    public static string RegisterErrorInstanceNodeNotFound(CultureInfo culture) => FrameworkResources.Get(nameof (RegisterErrorInstanceNodeNotFound), culture);

    public static string ServicingStepLogEntryKindError() => FrameworkResources.Get(nameof (ServicingStepLogEntryKindError));

    public static string ServicingStepLogEntryKindError(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingStepLogEntryKindError), culture);

    public static string ServicingStepLogEntryKindInformational() => FrameworkResources.Get(nameof (ServicingStepLogEntryKindInformational));

    public static string ServicingStepLogEntryKindInformational(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingStepLogEntryKindInformational), culture);

    public static string ServicingStepLogEntryKindWarning() => FrameworkResources.Get(nameof (ServicingStepLogEntryKindWarning));

    public static string ServicingStepLogEntryKindWarning(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingStepLogEntryKindWarning), culture);

    public static string ServicingStepLogEntryKindStepDuration() => FrameworkResources.Get(nameof (ServicingStepLogEntryKindStepDuration));

    public static string ServicingStepLogEntryKindStepDuration(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingStepLogEntryKindStepDuration), culture);

    public static string ServicingStepLogEntryKindGroupDuration() => FrameworkResources.Get(nameof (ServicingStepLogEntryKindGroupDuration));

    public static string ServicingStepLogEntryKindGroupDuration(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingStepLogEntryKindGroupDuration), culture);

    public static string ServicingStepLogEntryKindOperationDuration() => FrameworkResources.Get(nameof (ServicingStepLogEntryKindOperationDuration));

    public static string ServicingStepLogEntryKindOperationDuration(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingStepLogEntryKindOperationDuration), culture);

    public static string ServicingStepLogEntryKindSleepDuration() => FrameworkResources.Get(nameof (ServicingStepLogEntryKindSleepDuration));

    public static string ServicingStepLogEntryKindSleepDuration(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingStepLogEntryKindSleepDuration), culture);

    public static string ServicingStepLogEntryKindUnknown() => FrameworkResources.Get(nameof (ServicingStepLogEntryKindUnknown));

    public static string ServicingStepLogEntryKindUnknown(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingStepLogEntryKindUnknown), culture);

    public static string NoncompliantUserResolutionError(object arg0, object arg1) => FrameworkResources.Format(nameof (NoncompliantUserResolutionError), arg0, arg1);

    public static string NoncompliantUserResolutionError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (NoncompliantUserResolutionError), culture, arg0, arg1);
    }

    public static string AccountNameReserved() => FrameworkResources.Get(nameof (AccountNameReserved));

    public static string AccountNameReserved(CultureInfo culture) => FrameworkResources.Get(nameof (AccountNameReserved), culture);

    public static string JobFailedEventError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6)
    {
      return FrameworkResources.Format(nameof (JobFailedEventError), arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public static string JobFailedEventError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (JobFailedEventError), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public static string AdvancedLicenseDescription() => FrameworkResources.Get(nameof (AdvancedLicenseDescription));

    public static string AdvancedLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (AdvancedLicenseDescription), culture);

    public static string AdvancedLicenseName() => FrameworkResources.Get(nameof (AdvancedLicenseName));

    public static string AdvancedLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (AdvancedLicenseName), culture);

    public static string AdvancedPlusLicenseDescription() => FrameworkResources.Get(nameof (AdvancedPlusLicenseDescription));

    public static string AdvancedPlusLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (AdvancedPlusLicenseDescription), culture);

    public static string AdvancedPlusLicenseName() => FrameworkResources.Get(nameof (AdvancedPlusLicenseName));

    public static string AdvancedPlusLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (AdvancedPlusLicenseName), culture);

    public static string ExpressLicenseDescription() => FrameworkResources.Get(nameof (ExpressLicenseDescription));

    public static string ExpressLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ExpressLicenseDescription), culture);

    public static string ExpressLicenseName() => FrameworkResources.Get(nameof (ExpressLicenseName));

    public static string ExpressLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (ExpressLicenseName), culture);

    public static string SignedInContentForJavascriptNotify() => FrameworkResources.Get(nameof (SignedInContentForJavascriptNotify));

    public static string SignedInContentForJavascriptNotify(CultureInfo culture) => FrameworkResources.Get(nameof (SignedInContentForJavascriptNotify), culture);

    public static string SignedInContentForRealm() => FrameworkResources.Get(nameof (SignedInContentForRealm));

    public static string SignedInContentForRealm(CultureInfo culture) => FrameworkResources.Get(nameof (SignedInContentForRealm), culture);

    public static string ComplianceStateNotSaved() => FrameworkResources.Get(nameof (ComplianceStateNotSaved));

    public static string ComplianceStateNotSaved(CultureInfo culture) => FrameworkResources.Get(nameof (ComplianceStateNotSaved), culture);

    public static string GetClientArgumentError(object arg0) => FrameworkResources.Format(nameof (GetClientArgumentError), arg0);

    public static string GetClientArgumentError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (GetClientArgumentError), culture, arg0);

    public static string GetClientFailed(object arg0) => FrameworkResources.Format(nameof (GetClientFailed), arg0);

    public static string GetClientFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (GetClientFailed), culture, arg0);

    public static string UserLacksAccountRights(object arg0) => FrameworkResources.Format(nameof (UserLacksAccountRights), arg0);

    public static string UserLacksAccountRights(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UserLacksAccountRights), culture, arg0);

    public static string UserMustBecomeCompliant(object arg0) => FrameworkResources.Format(nameof (UserMustBecomeCompliant), arg0);

    public static string UserMustBecomeCompliant(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UserMustBecomeCompliant), culture, arg0);

    public static string UserNotCompliant(object arg0) => FrameworkResources.Format(nameof (UserNotCompliant), arg0);

    public static string UserNotCompliant(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UserNotCompliant), culture, arg0);

    public static string UserNotCompliantFromAuthority(object arg0) => FrameworkResources.Format(nameof (UserNotCompliantFromAuthority), arg0);

    public static string UserNotCompliantFromAuthority(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UserNotCompliantFromAuthority), culture, arg0);

    public static string IdentityMappingFileNotFound(object arg0) => FrameworkResources.Format(nameof (IdentityMappingFileNotFound), arg0);

    public static string IdentityMappingFileNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (IdentityMappingFileNotFound), culture, arg0);

    public static string ErrorParsingCsvFile(object arg0, object arg1) => FrameworkResources.Format(nameof (ErrorParsingCsvFile), arg0, arg1);

    public static string ErrorParsingCsvFile(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ErrorParsingCsvFile), culture, arg0, arg1);

    public static string ErrorAtLeastThreeFields() => FrameworkResources.Get(nameof (ErrorAtLeastThreeFields));

    public static string ErrorAtLeastThreeFields(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorAtLeastThreeFields), culture);

    public static string ErrorMappingFileAccessMode() => FrameworkResources.Get(nameof (ErrorMappingFileAccessMode));

    public static string ErrorMappingFileAccessMode(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorMappingFileAccessMode), culture);

    public static string ErrorMappingFileNotOpenForWrite() => FrameworkResources.Get(nameof (ErrorMappingFileNotOpenForWrite));

    public static string ErrorMappingFileNotOpenForWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorMappingFileNotOpenForWrite), culture);

    public static string ErrorMappingFileNotOpenForRead() => FrameworkResources.Get(nameof (ErrorMappingFileNotOpenForRead));

    public static string ErrorMappingFileNotOpenForRead(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorMappingFileNotOpenForRead), culture);

    public static string UnsupportedHostTypeMessage(object arg0) => FrameworkResources.Format(nameof (UnsupportedHostTypeMessage), arg0);

    public static string UnsupportedHostTypeMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnsupportedHostTypeMessage), culture, arg0);

    public static string InvalidRegionsWeightsMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidRegionsWeightsMessage), arg0, arg1);

    public static string InvalidRegionsWeightsMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidRegionsWeightsMessage), culture, arg0, arg1);
    }

    public static string InvalidWeightMessage(object arg0) => FrameworkResources.Format(nameof (InvalidWeightMessage), arg0);

    public static string InvalidWeightMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidWeightMessage), culture, arg0);

    public static string LicenseeGroupDescription() => FrameworkResources.Get(nameof (LicenseeGroupDescription));

    public static string LicenseeGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (LicenseeGroupDescription), culture);

    public static string LicenseeGroupName() => FrameworkResources.Get(nameof (LicenseeGroupName));

    public static string LicenseeGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (LicenseeGroupName), culture);

    public static string UnexpectedRequestContextType(object arg0) => FrameworkResources.Format(nameof (UnexpectedRequestContextType), arg0);

    public static string UnexpectedRequestContextType(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnexpectedRequestContextType), culture, arg0);

    public static string PasswordButNoUserName() => FrameworkResources.Get(nameof (PasswordButNoUserName));

    public static string PasswordButNoUserName(CultureInfo culture) => FrameworkResources.Get(nameof (PasswordButNoUserName), culture);

    public static string UserNameButNoPassword() => FrameworkResources.Get(nameof (UserNameButNoPassword));

    public static string UserNameButNoPassword(CultureInfo culture) => FrameworkResources.Get(nameof (UserNameButNoPassword), culture);

    public static string NoSqlConnectionFactory() => FrameworkResources.Get(nameof (NoSqlConnectionFactory));

    public static string NoSqlConnectionFactory(CultureInfo culture) => FrameworkResources.Get(nameof (NoSqlConnectionFactory), culture);

    public static string RequestContextRequiredForOperation() => FrameworkResources.Get(nameof (RequestContextRequiredForOperation));

    public static string RequestContextRequiredForOperation(CultureInfo culture) => FrameworkResources.Get(nameof (RequestContextRequiredForOperation), culture);

    public static string DatabaseManagementComponentVersionMismatch() => FrameworkResources.Get(nameof (DatabaseManagementComponentVersionMismatch));

    public static string DatabaseManagementComponentVersionMismatch(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseManagementComponentVersionMismatch), culture);

    public static string DataTierComponentVersionMismatch() => FrameworkResources.Get(nameof (DataTierComponentVersionMismatch));

    public static string DataTierComponentVersionMismatch(CultureInfo culture) => FrameworkResources.Get(nameof (DataTierComponentVersionMismatch), culture);

    public static string DatabasePartitionComponentVersionMismatch() => FrameworkResources.Get(nameof (DatabasePartitionComponentVersionMismatch));

    public static string DatabasePartitionComponentVersionMismatch(CultureInfo culture) => FrameworkResources.Get(nameof (DatabasePartitionComponentVersionMismatch), culture);

    public static string InvalidConnectionString() => FrameworkResources.Get(nameof (InvalidConnectionString));

    public static string InvalidConnectionString(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidConnectionString), culture);

    public static string PasswordChangeAlreadyPending() => FrameworkResources.Get(nameof (PasswordChangeAlreadyPending));

    public static string PasswordChangeAlreadyPending(CultureInfo culture) => FrameworkResources.Get(nameof (PasswordChangeAlreadyPending), culture);

    public static string PasswordChangeNotPending() => FrameworkResources.Get(nameof (PasswordChangeNotPending));

    public static string PasswordChangeNotPending(CultureInfo culture) => FrameworkResources.Get(nameof (PasswordChangeNotPending), culture);

    public static string PasswordResetNotSupported() => FrameworkResources.Get(nameof (PasswordResetNotSupported));

    public static string PasswordResetNotSupported(CultureInfo culture) => FrameworkResources.Get(nameof (PasswordResetNotSupported), culture);

    public static string InvalidDeploymentHostConnectionInfo() => FrameworkResources.Get(nameof (InvalidDeploymentHostConnectionInfo));

    public static string InvalidDeploymentHostConnectionInfo(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidDeploymentHostConnectionInfo), culture);

    public static string DataTooLargeForKeyLenAndAlgorithm(object arg0, object arg1) => FrameworkResources.Format(nameof (DataTooLargeForKeyLenAndAlgorithm), arg0, arg1);

    public static string DataTooLargeForKeyLenAndAlgorithm(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DataTooLargeForKeyLenAndAlgorithm), culture, arg0, arg1);
    }

    public static string IncompatibleKeyLengthAndAlgorithm(object arg0, object arg1) => FrameworkResources.Format(nameof (IncompatibleKeyLengthAndAlgorithm), arg0, arg1);

    public static string IncompatibleKeyLengthAndAlgorithm(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (IncompatibleKeyLengthAndAlgorithm), culture, arg0, arg1);
    }

    public static string NoDataToEncrypt() => FrameworkResources.Get(nameof (NoDataToEncrypt));

    public static string NoDataToEncrypt(CultureInfo culture) => FrameworkResources.Get(nameof (NoDataToEncrypt), culture);

    public static string InvalidSigningAlgorithm() => FrameworkResources.Get(nameof (InvalidSigningAlgorithm));

    public static string InvalidSigningAlgorithm(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidSigningAlgorithm), culture);

    public static string DatabaseThresholdIntervalSettingInvalid(object arg0) => FrameworkResources.Format(nameof (DatabaseThresholdIntervalSettingInvalid), arg0);

    public static string DatabaseThresholdIntervalSettingInvalid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseThresholdIntervalSettingInvalid), culture, arg0);

    public static string RoutingServiceReloadFailed(object arg0) => FrameworkResources.Format(nameof (RoutingServiceReloadFailed), arg0);

    public static string RoutingServiceReloadFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RoutingServiceReloadFailed), culture, arg0);

    public static string DatabaseWrongPool(object arg0) => FrameworkResources.Format(nameof (DatabaseWrongPool), arg0);

    public static string DatabaseWrongPool(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseWrongPool), culture, arg0);

    public static string InvalidNamedLockReentry(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidNamedLockReentry), arg0, arg1);

    public static string InvalidNamedLockReentry(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidNamedLockReentry), culture, arg0, arg1);

    public static string InvalidNamedLockUsage(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (InvalidNamedLockUsage), arg0, arg1, arg2);

    public static string InvalidNamedLockUsage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidNamedLockUsage), culture, arg0, arg1, arg2);
    }

    public static string InvalidOneTimeJobIdError(object arg0) => FrameworkResources.Format(nameof (InvalidOneTimeJobIdError), arg0);

    public static string InvalidOneTimeJobIdError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidOneTimeJobIdError), culture, arg0);

    public static string QueueOneTimeJobNotPermitted() => FrameworkResources.Get(nameof (QueueOneTimeJobNotPermitted));

    public static string QueueOneTimeJobNotPermitted(CultureInfo culture) => FrameworkResources.Get(nameof (QueueOneTimeJobNotPermitted), culture);

    public static string DatabaseCredentialNotFoundExceptionMessage(object arg0) => FrameworkResources.Format(nameof (DatabaseCredentialNotFoundExceptionMessage), arg0);

    public static string DatabaseCredentialNotFoundExceptionMessage(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DatabaseCredentialNotFoundExceptionMessage), culture, arg0);
    }

    public static string StaleDatabaseCredentialExceptionMessage(object arg0) => FrameworkResources.Format(nameof (StaleDatabaseCredentialExceptionMessage), arg0);

    public static string StaleDatabaseCredentialExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (StaleDatabaseCredentialExceptionMessage), culture, arg0);

    public static string DatabaseTypeNotSupported(object arg0) => FrameworkResources.Format(nameof (DatabaseTypeNotSupported), arg0);

    public static string DatabaseTypeNotSupported(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseTypeNotSupported), culture, arg0);

    public static string TagPermissionEnumerate() => FrameworkResources.Get(nameof (TagPermissionEnumerate));

    public static string TagPermissionEnumerate(CultureInfo culture) => FrameworkResources.Get(nameof (TagPermissionEnumerate), culture);

    public static string TagPermissionCreate() => FrameworkResources.Get(nameof (TagPermissionCreate));

    public static string TagPermissionCreate(CultureInfo culture) => FrameworkResources.Get(nameof (TagPermissionCreate), culture);

    public static string TagPermissionUpdate() => FrameworkResources.Get(nameof (TagPermissionUpdate));

    public static string TagPermissionUpdate(CultureInfo culture) => FrameworkResources.Get(nameof (TagPermissionUpdate), culture);

    public static string TagPermissionDelete() => FrameworkResources.Get(nameof (TagPermissionDelete));

    public static string TagPermissionDelete(CultureInfo culture) => FrameworkResources.Get(nameof (TagPermissionDelete), culture);

    public static string UserLacksAccountRightsWithReason(object arg0, object arg1) => FrameworkResources.Format(nameof (UserLacksAccountRightsWithReason), arg0, arg1);

    public static string UserLacksAccountRightsWithReason(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UserLacksAccountRightsWithReason), culture, arg0, arg1);
    }

    public static string SourceDatabaseIdMismatchExceptionMessage() => FrameworkResources.Get(nameof (SourceDatabaseIdMismatchExceptionMessage));

    public static string SourceDatabaseIdMismatchExceptionMessage(CultureInfo culture) => FrameworkResources.Get(nameof (SourceDatabaseIdMismatchExceptionMessage), culture);

    public static string DatabasePropertiesStaleExceptionMessage(object arg0) => FrameworkResources.Format(nameof (DatabasePropertiesStaleExceptionMessage), arg0);

    public static string DatabasePropertiesStaleExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabasePropertiesStaleExceptionMessage), culture, arg0);

    public static string AzureCertNotFound(object arg0) => FrameworkResources.Format(nameof (AzureCertNotFound), arg0);

    public static string AzureCertNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AzureCertNotFound), culture, arg0);

    public static string InvalidIdentityDomain() => FrameworkResources.Get(nameof (InvalidIdentityDomain));

    public static string InvalidIdentityDomain(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidIdentityDomain), culture);

    public static string AccessControlAddSignCertException() => FrameworkResources.Get(nameof (AccessControlAddSignCertException));

    public static string AccessControlAddSignCertException(CultureInfo culture) => FrameworkResources.Get(nameof (AccessControlAddSignCertException), culture);

    public static string PrimaryCertificateSlotNotEmptyException() => FrameworkResources.Get(nameof (PrimaryCertificateSlotNotEmptyException));

    public static string PrimaryCertificateSlotNotEmptyException(CultureInfo culture) => FrameworkResources.Get(nameof (PrimaryCertificateSlotNotEmptyException), culture);

    public static string SecondaryCertificateSlotNotEmptyException(object arg0) => FrameworkResources.Format(nameof (SecondaryCertificateSlotNotEmptyException), arg0);

    public static string SecondaryCertificateSlotNotEmptyException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SecondaryCertificateSlotNotEmptyException), culture, arg0);

    public static string ServiceKeyNotCertificateException() => FrameworkResources.Get(nameof (ServiceKeyNotCertificateException));

    public static string ServiceKeyNotCertificateException(CultureInfo culture) => FrameworkResources.Get(nameof (ServiceKeyNotCertificateException), culture);

    public static string MultipleServiceKeysWithThumbprintException(object arg0) => FrameworkResources.Format(nameof (MultipleServiceKeysWithThumbprintException), arg0);

    public static string MultipleServiceKeysWithThumbprintException(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MultipleServiceKeysWithThumbprintException), culture, arg0);
    }

    public static string ServiceKeyNotFoundException(object arg0) => FrameworkResources.Format(nameof (ServiceKeyNotFoundException), arg0);

    public static string ServiceKeyNotFoundException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceKeyNotFoundException), culture, arg0);

    public static string AccessControlDeleteKeyException() => FrameworkResources.Get(nameof (AccessControlDeleteKeyException));

    public static string AccessControlDeleteKeyException(CultureInfo culture) => FrameworkResources.Get(nameof (AccessControlDeleteKeyException), culture);

    public static string NotPermittedPrimaryCertException() => FrameworkResources.Get(nameof (NotPermittedPrimaryCertException));

    public static string NotPermittedPrimaryCertException(CultureInfo culture) => FrameworkResources.Get(nameof (NotPermittedPrimaryCertException), culture);

    public static string AccessControlGetSigningCertsException() => FrameworkResources.Get(nameof (AccessControlGetSigningCertsException));

    public static string AccessControlGetSigningCertsException(CultureInfo culture) => FrameworkResources.Get(nameof (AccessControlGetSigningCertsException), culture);

    public static string CreateCollectionFailed(object arg0) => FrameworkResources.Format(nameof (CreateCollectionFailed), arg0);

    public static string CreateCollectionFailed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CreateCollectionFailed), culture, arg0);

    public static string DataspaceNotFoundByCategoryIdentifier(object arg0, object arg1) => FrameworkResources.Format(nameof (DataspaceNotFoundByCategoryIdentifier), arg0, arg1);

    public static string DataspaceNotFoundByCategoryIdentifier(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DataspaceNotFoundByCategoryIdentifier), culture, arg0, arg1);
    }

    public static string DataspaceNotFoundById(object arg0) => FrameworkResources.Format(nameof (DataspaceNotFoundById), arg0);

    public static string DataspaceNotFoundById(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DataspaceNotFoundById), culture, arg0);

    public static string LocationServiceOwnersDoNotMatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7)
    {
      return FrameworkResources.Format(nameof (LocationServiceOwnersDoNotMatch), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string LocationServiceOwnersDoNotMatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (LocationServiceOwnersDoNotMatch), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string ErrorMessage_MsaTenantName() => FrameworkResources.Get(nameof (ErrorMessage_MsaTenantName));

    public static string ErrorMessage_MsaTenantName(CultureInfo culture) => FrameworkResources.Get(nameof (ErrorMessage_MsaTenantName), culture);

    public static string InvalidQueryParam(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (InvalidQueryParam), arg0, arg1, arg2);

    public static string InvalidQueryParam(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidQueryParam), culture, arg0, arg1, arg2);
    }

    public static string InvalidQueryParamFileName(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidQueryParamFileName), arg0, arg1);

    public static string InvalidQueryParamFileName(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidQueryParamFileName), culture, arg0, arg1);

    public static string InvalidRegionMessage(object arg0) => FrameworkResources.Format(nameof (InvalidRegionMessage), arg0);

    public static string InvalidRegionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidRegionMessage), culture, arg0);

    public static string StakeholderLicenseDescription() => FrameworkResources.Get(nameof (StakeholderLicenseDescription));

    public static string StakeholderLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (StakeholderLicenseDescription), culture);

    public static string StakeholderLicenseName() => FrameworkResources.Get(nameof (StakeholderLicenseName));

    public static string StakeholderLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (StakeholderLicenseName), culture);

    public static string ServiceOwnerNotFoundMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (ServiceOwnerNotFoundMessage), arg0, arg1);

    public static string ServiceOwnerNotFoundMessage(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceOwnerNotFoundMessage), culture, arg0, arg1);

    public static string InvalidServicingHostType(object arg0) => FrameworkResources.Format(nameof (InvalidServicingHostType), arg0);

    public static string InvalidServicingHostType(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidServicingHostType), culture, arg0);

    public static string InvalidServicingOperationPrefix(object arg0) => FrameworkResources.Format(nameof (InvalidServicingOperationPrefix), arg0);

    public static string InvalidServicingOperationPrefix(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidServicingOperationPrefix), culture, arg0);

    public static string InvalidServicingTarget(object arg0) => FrameworkResources.Format(nameof (InvalidServicingTarget), arg0);

    public static string InvalidServicingTarget(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidServicingTarget), culture, arg0);

    public static string ServicingResourcesLoadException(object arg0) => FrameworkResources.Format(nameof (ServicingResourcesLoadException), arg0);

    public static string ServicingResourcesLoadException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServicingResourcesLoadException), culture, arg0);

    public static string SignerCantEncryptExceptionMessage(object arg0) => FrameworkResources.Format(nameof (SignerCantEncryptExceptionMessage), arg0);

    public static string SignerCantEncryptExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SignerCantEncryptExceptionMessage), culture, arg0);

    public static string MissingDataspaceMapper(object arg0) => FrameworkResources.Format(nameof (MissingDataspaceMapper), arg0);

    public static string MissingDataspaceMapper(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MissingDataspaceMapper), culture, arg0);

    public static string CannotUpdateSecurityTemplateEntryAtInstallTime(object arg0) => FrameworkResources.Format(nameof (CannotUpdateSecurityTemplateEntryAtInstallTime), arg0);

    public static string CannotUpdateSecurityTemplateEntryAtInstallTime(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CannotUpdateSecurityTemplateEntryAtInstallTime), culture, arg0);
    }

    public static string UnknownSSASIntegratedSecurity(object arg0) => FrameworkResources.Format(nameof (UnknownSSASIntegratedSecurity), arg0);

    public static string UnknownSSASIntegratedSecurity(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnknownSSASIntegratedSecurity), culture, arg0);

    public static string ExpectedValueForKeywordToBeAnInteger(object arg0) => FrameworkResources.Format(nameof (ExpectedValueForKeywordToBeAnInteger), arg0);

    public static string ExpectedValueForKeywordToBeAnInteger(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ExpectedValueForKeywordToBeAnInteger), culture, arg0);

    public static string RedisCacheServiceRequestContextHostMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (RedisCacheServiceRequestContextHostMessage), arg0, arg1);

    public static string RedisCacheServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (RedisCacheServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string PermissionLocationRead() => FrameworkResources.Get(nameof (PermissionLocationRead));

    public static string PermissionLocationRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionLocationRead), culture);

    public static string PermissionLocationWrite() => FrameworkResources.Get(nameof (PermissionLocationWrite));

    public static string PermissionLocationWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionLocationWrite), culture);

    public static string PermissionIdentitiesDelete() => FrameworkResources.Get(nameof (PermissionIdentitiesDelete));

    public static string PermissionIdentitiesDelete(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentitiesDelete), culture);

    public static string PermissionIdentitiesImpersonate() => FrameworkResources.Get(nameof (PermissionIdentitiesImpersonate));

    public static string PermissionIdentitiesImpersonate(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentitiesImpersonate), culture);

    public static string PermissionIdentitiesRead() => FrameworkResources.Get(nameof (PermissionIdentitiesRead));

    public static string PermissionIdentitiesRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentitiesRead), culture);

    public static string PermissionIdentitiesWrite() => FrameworkResources.Get(nameof (PermissionIdentitiesWrite));

    public static string PermissionIdentitiesWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentitiesWrite), culture);

    public static string DataspaceMustBeCreatingOrActive() => FrameworkResources.Get(nameof (DataspaceMustBeCreatingOrActive));

    public static string DataspaceMustBeCreatingOrActive(CultureInfo culture) => FrameworkResources.Get(nameof (DataspaceMustBeCreatingOrActive), culture);

    public static string UnsupportedLazyThreadSafetyMode() => FrameworkResources.Get(nameof (UnsupportedLazyThreadSafetyMode));

    public static string UnsupportedLazyThreadSafetyMode(CultureInfo culture) => FrameworkResources.Get(nameof (UnsupportedLazyThreadSafetyMode), culture);

    public static string PermissionIdentityCreateScope() => FrameworkResources.Get(nameof (PermissionIdentityCreateScope));

    public static string PermissionIdentityCreateScope(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentityCreateScope), culture);

    public static string ProxyAlreadyAddedToSiteException() => FrameworkResources.Get(nameof (ProxyAlreadyAddedToSiteException));

    public static string ProxyAlreadyAddedToSiteException(CultureInfo culture) => FrameworkResources.Get(nameof (ProxyAlreadyAddedToSiteException), culture);

    public static string ErrorCaption(object arg0) => FrameworkResources.Format(nameof (ErrorCaption), arg0);

    public static string ErrorCaption(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ErrorCaption), culture, arg0);

    public static string Error_InternalTFSFailure() => FrameworkResources.Get(nameof (Error_InternalTFSFailure));

    public static string Error_InternalTFSFailure(CultureInfo culture) => FrameworkResources.Get(nameof (Error_InternalTFSFailure), culture);

    public static string Error_InternalTFSFailureWithCorrelation(object arg0) => FrameworkResources.Format(nameof (Error_InternalTFSFailureWithCorrelation), arg0);

    public static string Error_InternalTFSFailureWithCorrelation(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (Error_InternalTFSFailureWithCorrelation), culture, arg0);

    public static string Error_UnAuthorizedAccess() => FrameworkResources.Get(nameof (Error_UnAuthorizedAccess));

    public static string Error_UnAuthorizedAccess(CultureInfo culture) => FrameworkResources.Get(nameof (Error_UnAuthorizedAccess), culture);

    public static string CookiesSupportRequired(object arg0) => FrameworkResources.Format(nameof (CookiesSupportRequired), arg0);

    public static string CookiesSupportRequired(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CookiesSupportRequired), culture, arg0);

    public static string InvalidAndPotentiallyDangerousRequest() => FrameworkResources.Get(nameof (InvalidAndPotentiallyDangerousRequest));

    public static string InvalidAndPotentiallyDangerousRequest(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidAndPotentiallyDangerousRequest), culture);

    public static string DuplicateFileIdDataspaceExceptionMessage(object arg0) => FrameworkResources.Format(nameof (DuplicateFileIdDataspaceExceptionMessage), arg0);

    public static string DuplicateFileIdDataspaceExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DuplicateFileIdDataspaceExceptionMessage), culture, arg0);

    public static string MissingRequiredAadGroupMembership() => FrameworkResources.Get(nameof (MissingRequiredAadGroupMembership));

    public static string MissingRequiredAadGroupMembership(CultureInfo culture) => FrameworkResources.Get(nameof (MissingRequiredAadGroupMembership), culture);

    public static string AadGuestUserNotAllowedInAccount() => FrameworkResources.Get(nameof (AadGuestUserNotAllowedInAccount));

    public static string AadGuestUserNotAllowedInAccount(CultureInfo culture) => FrameworkResources.Get(nameof (AadGuestUserNotAllowedInAccount), culture);

    public static string StoredProcedureHeader() => FrameworkResources.Get(nameof (StoredProcedureHeader));

    public static string StoredProcedureHeader(CultureInfo culture) => FrameworkResources.Get(nameof (StoredProcedureHeader), culture);

    public static string FunctionHeader() => FrameworkResources.Get(nameof (FunctionHeader));

    public static string FunctionHeader(CultureInfo culture) => FrameworkResources.Get(nameof (FunctionHeader), culture);

    public static string ViewHeader() => FrameworkResources.Get(nameof (ViewHeader));

    public static string ViewHeader(CultureInfo culture) => FrameworkResources.Get(nameof (ViewHeader), culture);

    public static string KeyTypeNotAllowed(object arg0) => FrameworkResources.Format(nameof (KeyTypeNotAllowed), arg0);

    public static string KeyTypeNotAllowed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (KeyTypeNotAllowed), culture, arg0);

    public static string ClientNotificationProxyTimeout() => FrameworkResources.Get(nameof (ClientNotificationProxyTimeout));

    public static string ClientNotificationProxyTimeout(CultureInfo culture) => FrameworkResources.Get(nameof (ClientNotificationProxyTimeout), culture);

    public static string BadConnectionStringFormat(object arg0) => FrameworkResources.Format(nameof (BadConnectionStringFormat), arg0);

    public static string BadConnectionStringFormat(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (BadConnectionStringFormat), culture, arg0);

    public static string FailedToStopHost() => FrameworkResources.Get(nameof (FailedToStopHost));

    public static string FailedToStopHost(CultureInfo culture) => FrameworkResources.Get(nameof (FailedToStopHost), culture);

    public static string SqlObjectValidationHelpMessage() => FrameworkResources.Get(nameof (SqlObjectValidationHelpMessage));

    public static string SqlObjectValidationHelpMessage(CultureInfo culture) => FrameworkResources.Get(nameof (SqlObjectValidationHelpMessage), culture);

    public static string OperationUpdateInvalid(object arg0) => FrameworkResources.Format(nameof (OperationUpdateInvalid), arg0);

    public static string OperationUpdateInvalid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (OperationUpdateInvalid), culture, arg0);

    public static string RequestTimeoutException() => FrameworkResources.Get(nameof (RequestTimeoutException));

    public static string RequestTimeoutException(CultureInfo culture) => FrameworkResources.Get(nameof (RequestTimeoutException), culture);

    public static string JobTimeoutException() => FrameworkResources.Get(nameof (JobTimeoutException));

    public static string JobTimeoutException(CultureInfo culture) => FrameworkResources.Get(nameof (JobTimeoutException), culture);

    public static string JobStoppedUponServiceShutdownReason() => FrameworkResources.Get(nameof (JobStoppedUponServiceShutdownReason));

    public static string JobStoppedUponServiceShutdownReason(CultureInfo culture) => FrameworkResources.Get(nameof (JobStoppedUponServiceShutdownReason), culture);

    public static string JobStoppedUponUserRequestReason() => FrameworkResources.Get(nameof (JobStoppedUponUserRequestReason));

    public static string JobStoppedUponUserRequestReason(CultureInfo culture) => FrameworkResources.Get(nameof (JobStoppedUponUserRequestReason), culture);

    public static string KeyTypeNotAllowedForHost(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (KeyTypeNotAllowedForHost), arg0, arg1, arg2);

    public static string KeyTypeNotAllowedForHost(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (KeyTypeNotAllowedForHost), culture, arg0, arg1, arg2);
    }

    public static string KeyTypeMustBeRSASecuredOrStored(object arg0) => FrameworkResources.Format(nameof (KeyTypeMustBeRSASecuredOrStored), arg0);

    public static string KeyTypeMustBeRSASecuredOrStored(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (KeyTypeMustBeRSASecuredOrStored), culture, arg0);

    public static string InvalidDatabasePoolForHostType(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidDatabasePoolForHostType), arg0, arg1);

    public static string InvalidDatabasePoolForHostType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InvalidDatabasePoolForHostType), culture, arg0, arg1);
    }

    public static string DatabaseFileGrowthException(object arg0) => FrameworkResources.Format(nameof (DatabaseFileGrowthException), arg0);

    public static string DatabaseFileGrowthException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseFileGrowthException), culture, arg0);

    public static string CounterDataspaceDoesNotExist(object arg0) => FrameworkResources.Format(nameof (CounterDataspaceDoesNotExist), arg0);

    public static string CounterDataspaceDoesNotExist(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CounterDataspaceDoesNotExist), culture, arg0);

    public static string InvalidCharactersInFilename() => FrameworkResources.Get(nameof (InvalidCharactersInFilename));

    public static string InvalidCharactersInFilename(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidCharactersInFilename), culture);

    public static string InvalidCharactersInFilepath() => FrameworkResources.Get(nameof (InvalidCharactersInFilepath));

    public static string InvalidCharactersInFilepath(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidCharactersInFilepath), culture);

    public static string InvalidRootedFilenameOrPath() => FrameworkResources.Get(nameof (InvalidRootedFilenameOrPath));

    public static string InvalidRootedFilenameOrPath(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidRootedFilenameOrPath), culture);

    public static string CollectionHostRequired() => FrameworkResources.Get(nameof (CollectionHostRequired));

    public static string CollectionHostRequired(CultureInfo culture) => FrameworkResources.Get(nameof (CollectionHostRequired), culture);

    public static string ResourceArea() => FrameworkResources.Get(nameof (ResourceArea));

    public static string ResourceArea(CultureInfo culture) => FrameworkResources.Get(nameof (ResourceArea), culture);

    public static string WebApiInitializationPreviouslyFailed() => FrameworkResources.Get(nameof (WebApiInitializationPreviouslyFailed));

    public static string WebApiInitializationPreviouslyFailed(CultureInfo culture) => FrameworkResources.Get(nameof (WebApiInitializationPreviouslyFailed), culture);

    public static string IdentityMruNameTooBig(object arg0, object arg1) => FrameworkResources.Format(nameof (IdentityMruNameTooBig), arg0, arg1);

    public static string IdentityMruNameTooBig(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (IdentityMruNameTooBig), culture, arg0, arg1);

    public static string InvalidIdentityMruId() => FrameworkResources.Get(nameof (InvalidIdentityMruId));

    public static string InvalidIdentityMruId(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidIdentityMruId), culture);

    public static string RequestedResourceAlreadyExists(object arg0) => FrameworkResources.Format(nameof (RequestedResourceAlreadyExists), arg0);

    public static string RequestedResourceAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequestedResourceAlreadyExists), culture, arg0);

    public static string RequestedResourceNotFound(object arg0) => FrameworkResources.Format(nameof (RequestedResourceNotFound), arg0);

    public static string RequestedResourceNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequestedResourceNotFound), culture, arg0);

    public static string UnauthorizedToAccessResource() => FrameworkResources.Get(nameof (UnauthorizedToAccessResource));

    public static string UnauthorizedToAccessResource(CultureInfo culture) => FrameworkResources.Get(nameof (UnauthorizedToAccessResource), culture);

    public static string InvalidIdentityMruValue() => FrameworkResources.Get(nameof (InvalidIdentityMruValue));

    public static string InvalidIdentityMruValue(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidIdentityMruValue), culture);

    public static string MatchingDataTierAlreadyRegistered(object arg0, object arg1) => FrameworkResources.Format(nameof (MatchingDataTierAlreadyRegistered), arg0, arg1);

    public static string MatchingDataTierAlreadyRegistered(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MatchingDataTierAlreadyRegistered), culture, arg0, arg1);
    }

    public static string TriggerHeader() => FrameworkResources.Get(nameof (TriggerHeader));

    public static string TriggerHeader(CultureInfo culture) => FrameworkResources.Get(nameof (TriggerHeader), culture);

    public static string ISleepIfBusyInTransaction() => FrameworkResources.Get(nameof (ISleepIfBusyInTransaction));

    public static string ISleepIfBusyInTransaction(CultureInfo culture) => FrameworkResources.Get(nameof (ISleepIfBusyInTransaction), culture);

    public static string MissingDatabaseTypeAndCategory(object arg0) => FrameworkResources.Format(nameof (MissingDatabaseTypeAndCategory), arg0);

    public static string MissingDatabaseTypeAndCategory(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MissingDatabaseTypeAndCategory), culture, arg0);

    public static string AllItemsMustBeInSameDrawer() => FrameworkResources.Get(nameof (AllItemsMustBeInSameDrawer));

    public static string AllItemsMustBeInSameDrawer(CultureInfo culture) => FrameworkResources.Get(nameof (AllItemsMustBeInSameDrawer), culture);

    public static string LookupKeysMustBeUnique(object arg0) => FrameworkResources.Format(nameof (LookupKeysMustBeUnique), arg0);

    public static string LookupKeysMustBeUnique(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (LookupKeysMustBeUnique), culture, arg0);

    public static string ConfigurationConnectionStringError(object arg0) => FrameworkResources.Format(nameof (ConfigurationConnectionStringError), arg0);

    public static string ConfigurationConnectionStringError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ConfigurationConnectionStringError), culture, arg0);

    public static string ConfigurationGuidError(object arg0) => FrameworkResources.Format(nameof (ConfigurationGuidError), arg0);

    public static string ConfigurationGuidError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ConfigurationGuidError), culture, arg0);

    public static string JobAgentConfigurationDoesntExistError(object arg0) => FrameworkResources.Format(nameof (JobAgentConfigurationDoesntExistError), arg0);

    public static string JobAgentConfigurationDoesntExistError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobAgentConfigurationDoesntExistError), culture, arg0);

    public static string JobAgentConfigurationError(object arg0) => FrameworkResources.Format(nameof (JobAgentConfigurationError), arg0);

    public static string JobAgentConfigurationError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobAgentConfigurationError), culture, arg0);

    public static string JobAgentReadConfigurationError(object arg0) => FrameworkResources.Format(nameof (JobAgentReadConfigurationError), arg0);

    public static string JobAgentReadConfigurationError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JobAgentReadConfigurationError), culture, arg0);

    public static string InvalidArgumentsOnRedirectToIdentityProvider() => FrameworkResources.Get(nameof (InvalidArgumentsOnRedirectToIdentityProvider));

    public static string InvalidArgumentsOnRedirectToIdentityProvider(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidArgumentsOnRedirectToIdentityProvider), culture);

    public static string SqlScriptSqlConnectionLost(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (SqlScriptSqlConnectionLost), arg0, arg1, arg2, arg3);
    }

    public static string SqlScriptSqlConnectionLost(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (SqlScriptSqlConnectionLost), culture, arg0, arg1, arg2, arg3);
    }

    public static string ServicingLockHeldTimeout() => FrameworkResources.Get(nameof (ServicingLockHeldTimeout));

    public static string ServicingLockHeldTimeout(CultureInfo culture) => FrameworkResources.Get(nameof (ServicingLockHeldTimeout), culture);

    public static string ServicingLockHeldTimeoutAdminDetails(object arg0, object arg1) => FrameworkResources.Format(nameof (ServicingLockHeldTimeoutAdminDetails), arg0, arg1);

    public static string ServicingLockHeldTimeoutAdminDetails(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ServicingLockHeldTimeoutAdminDetails), culture, arg0, arg1);
    }

    public static string StepPerformerOverrideWithNoBaseError(object arg0) => FrameworkResources.Format(nameof (StepPerformerOverrideWithNoBaseError), arg0);

    public static string StepPerformerOverrideWithNoBaseError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (StepPerformerOverrideWithNoBaseError), culture, arg0);

    public static string ProfileServiceUnavailableMessage(object arg0) => FrameworkResources.Format(nameof (ProfileServiceUnavailableMessage), arg0);

    public static string ProfileServiceUnavailableMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ProfileServiceUnavailableMessage), culture, arg0);

    public static string MultiplePrimaryResolutionEntriesError(object arg0) => FrameworkResources.Format(nameof (MultiplePrimaryResolutionEntriesError), arg0);

    public static string MultiplePrimaryResolutionEntriesError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MultiplePrimaryResolutionEntriesError), culture, arg0);

    public static string ResolutionEntryAlreadyExistsError(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (ResolutionEntryAlreadyExistsError), arg0, arg1, arg2);

    public static string ResolutionEntryAlreadyExistsError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ResolutionEntryAlreadyExistsError), culture, arg0, arg1, arg2);
    }

    public static string MaximumBrowsingLimitExceeded(object arg0) => FrameworkResources.Format(nameof (MaximumBrowsingLimitExceeded), arg0);

    public static string MaximumBrowsingLimitExceeded(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MaximumBrowsingLimitExceeded), culture, arg0);

    public static string IndexRebuildInProgress() => FrameworkResources.Get(nameof (IndexRebuildInProgress));

    public static string IndexRebuildInProgress(CultureInfo culture) => FrameworkResources.Get(nameof (IndexRebuildInProgress), culture);

    public static string ExtensionNotInstalledOrLicensed(object arg0) => FrameworkResources.Format(nameof (ExtensionNotInstalledOrLicensed), arg0);

    public static string ExtensionNotInstalledOrLicensed(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ExtensionNotInstalledOrLicensed), culture, arg0);

    public static string MessageBusSubscriberNotFoundException(object arg0, object arg1) => FrameworkResources.Format(nameof (MessageBusSubscriberNotFoundException), arg0, arg1);

    public static string MessageBusSubscriberNotFoundException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MessageBusSubscriberNotFoundException), culture, arg0, arg1);
    }

    public static string SigningKeyNamespaceAlreadyExists(object arg0) => FrameworkResources.Format(nameof (SigningKeyNamespaceAlreadyExists), arg0);

    public static string SigningKeyNamespaceAlreadyExists(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SigningKeyNamespaceAlreadyExists), culture, arg0);

    public static string SigningKeyNamespaceNotRegistered(object arg0) => FrameworkResources.Format(nameof (SigningKeyNamespaceNotRegistered), arg0);

    public static string SigningKeyNamespaceNotRegistered(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SigningKeyNamespaceNotRegistered), culture, arg0);

    public static string UnableToResolveObjectSid(object arg0) => FrameworkResources.Format(nameof (UnableToResolveObjectSid), arg0);

    public static string UnableToResolveObjectSid(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnableToResolveObjectSid), culture, arg0);

    public static string UnableToFindManagersEntity(object arg0) => FrameworkResources.Format(nameof (UnableToFindManagersEntity), arg0);

    public static string UnableToFindManagersEntity(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnableToFindManagersEntity), culture, arg0);

    public static string DirectoryOperationNotSupported(object arg0, object arg1) => FrameworkResources.Format(nameof (DirectoryOperationNotSupported), arg0, arg1);

    public static string DirectoryOperationNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DirectoryOperationNotSupported), culture, arg0, arg1);
    }

    public static string MissingRegisteredDirectory(object arg0) => FrameworkResources.Format(nameof (MissingRegisteredDirectory), arg0);

    public static string MissingRegisteredDirectory(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MissingRegisteredDirectory), culture, arg0);

    public static string UnableToResolveDomainName(object arg0) => FrameworkResources.Format(nameof (UnableToResolveDomainName), arg0);

    public static string UnableToResolveDomainName(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnableToResolveDomainName), culture, arg0);

    public static string InvalidDomainNameArgument() => FrameworkResources.Get(nameof (InvalidDomainNameArgument));

    public static string InvalidDomainNameArgument(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidDomainNameArgument), culture);

    public static string UnableToFindGlobalCatalog() => FrameworkResources.Get(nameof (UnableToFindGlobalCatalog));

    public static string UnableToFindGlobalCatalog(CultureInfo culture) => FrameworkResources.Get(nameof (UnableToFindGlobalCatalog), culture);

    public static string UnableToRetrieveDirectoryEntry(object arg0) => FrameworkResources.Format(nameof (UnableToRetrieveDirectoryEntry), arg0);

    public static string UnableToRetrieveDirectoryEntry(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnableToRetrieveDirectoryEntry), culture, arg0);

    public static string UnableToBindToDirectoryEntry(object arg0) => FrameworkResources.Format(nameof (UnableToBindToDirectoryEntry), arg0);

    public static string UnableToBindToDirectoryEntry(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnableToBindToDirectoryEntry), culture, arg0);

    public static string MethodCanNotBeExecutedOnMaster() => FrameworkResources.Get(nameof (MethodCanNotBeExecutedOnMaster));

    public static string MethodCanNotBeExecutedOnMaster(CultureInfo culture) => FrameworkResources.Get(nameof (MethodCanNotBeExecutedOnMaster), culture);

    public static string OnlySupportedOnSqlAzure() => FrameworkResources.Get(nameof (OnlySupportedOnSqlAzure));

    public static string OnlySupportedOnSqlAzure(CultureInfo culture) => FrameworkResources.Get(nameof (OnlySupportedOnSqlAzure), culture);

    public static string MsdnEntitlementSeriveFailed(object arg0, object arg1) => FrameworkResources.Format(nameof (MsdnEntitlementSeriveFailed), arg0, arg1);

    public static string MsdnEntitlementSeriveFailed(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (MsdnEntitlementSeriveFailed), culture, arg0, arg1);

    public static string DatabaseFullException_TempDbAllocation(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (DatabaseFullException_TempDbAllocation), arg0, arg1, arg2);
    }

    public static string DatabaseFullException_TempDbAllocation(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DatabaseFullException_TempDbAllocation), culture, arg0, arg1, arg2);
    }

    public static string DatabaseFullException_TransactionLogFull(
      object arg0,
      object arg1,
      object arg2)
    {
      return FrameworkResources.Format(nameof (DatabaseFullException_TransactionLogFull), arg0, arg1, arg2);
    }

    public static string DatabaseFullException_TransactionLogFull(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DatabaseFullException_TransactionLogFull), culture, arg0, arg1, arg2);
    }

    public static string DatabaseFullExceptionWithDetails(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (DatabaseFullExceptionWithDetails), arg0, arg1, arg2);

    public static string DatabaseFullExceptionWithDetails(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DatabaseFullExceptionWithDetails), culture, arg0, arg1, arg2);
    }

    public static string PermissionDataImportRead() => FrameworkResources.Get(nameof (PermissionDataImportRead));

    public static string PermissionDataImportRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionDataImportRead), culture);

    public static string PermissionDataImportQueue() => FrameworkResources.Get(nameof (PermissionDataImportQueue));

    public static string PermissionDataImportQueue(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionDataImportQueue), culture);

    public static string PermissionDataImportCancel() => FrameworkResources.Get(nameof (PermissionDataImportCancel));

    public static string PermissionDataImportCancel(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionDataImportCancel), culture);

    public static string PermissionServicingOrchestrationRead() => FrameworkResources.Get(nameof (PermissionServicingOrchestrationRead));

    public static string PermissionServicingOrchestrationRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionServicingOrchestrationRead), culture);

    public static string PermissionServicingOrchestrationQueue() => FrameworkResources.Get(nameof (PermissionServicingOrchestrationQueue));

    public static string PermissionServicingOrchestrationQueue(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionServicingOrchestrationQueue), culture);

    public static string PermissionServicingOrchestrationCancel() => FrameworkResources.Get(nameof (PermissionServicingOrchestrationCancel));

    public static string PermissionServicingOrchestrationCancel(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionServicingOrchestrationCancel), culture);

    public static string AccountServiceUnavailableExceptionMessage(object arg0) => FrameworkResources.Format(nameof (AccountServiceUnavailableExceptionMessage), arg0);

    public static string AccountServiceUnavailableExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (AccountServiceUnavailableExceptionMessage), culture, arg0);

    public static string DatabasePartitionForeignKeyExceptionMessage(object arg0) => FrameworkResources.Format(nameof (DatabasePartitionForeignKeyExceptionMessage), arg0);

    public static string DatabasePartitionForeignKeyExceptionMessage(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DatabasePartitionForeignKeyExceptionMessage), culture, arg0);
    }

    public static string PartitionAlreadyExistsExceptionMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (PartitionAlreadyExistsExceptionMessage), arg0, arg1);

    public static string PartitionAlreadyExistsExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (PartitionAlreadyExistsExceptionMessage), culture, arg0, arg1);
    }

    public static string VirtualApplicationHostException() => FrameworkResources.Get(nameof (VirtualApplicationHostException));

    public static string VirtualApplicationHostException(CultureInfo culture) => FrameworkResources.Get(nameof (VirtualApplicationHostException), culture);

    public static string DatabaseUnavailableException(object arg0) => FrameworkResources.Format(nameof (DatabaseUnavailableException), arg0);

    public static string DatabaseUnavailableException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DatabaseUnavailableException), culture, arg0);

    public static string RequestBlockedWithFwlink(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (RequestBlockedWithFwlink), arg0, arg1, arg2);

    public static string RequestBlockedWithFwlink(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (RequestBlockedWithFwlink), culture, arg0, arg1, arg2);
    }

    public static string DelegatedAuthorizationUserIdSpecifiedWithoutSystemContext() => FrameworkResources.Get(nameof (DelegatedAuthorizationUserIdSpecifiedWithoutSystemContext));

    public static string DelegatedAuthorizationUserIdSpecifiedWithoutSystemContext(
      CultureInfo culture)
    {
      return FrameworkResources.Get(nameof (DelegatedAuthorizationUserIdSpecifiedWithoutSystemContext), culture);
    }

    public static string UserIdCannotbeSpecifiedAtDeploymentLevel() => FrameworkResources.Get(nameof (UserIdCannotbeSpecifiedAtDeploymentLevel));

    public static string UserIdCannotbeSpecifiedAtDeploymentLevel(CultureInfo culture) => FrameworkResources.Get(nameof (UserIdCannotbeSpecifiedAtDeploymentLevel), culture);

    public static string SecurityServiceGroupDescription() => FrameworkResources.Get(nameof (SecurityServiceGroupDescription));

    public static string SecurityServiceGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (SecurityServiceGroupDescription), culture);

    public static string SecurityServiceGroupName() => FrameworkResources.Get(nameof (SecurityServiceGroupName));

    public static string SecurityServiceGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (SecurityServiceGroupName), culture);

    public static string ProjectCollectionAdministrators() => FrameworkResources.Get(nameof (ProjectCollectionAdministrators));

    public static string ProjectCollectionAdministrators(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionAdministrators), culture);

    public static string ProjectCollectionAdministratorsDescription() => FrameworkResources.Get(nameof (ProjectCollectionAdministratorsDescription));

    public static string ProjectCollectionAdministratorsDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionAdministratorsDescription), culture);

    public static string ProjectCollectionServiceAccounts() => FrameworkResources.Get(nameof (ProjectCollectionServiceAccounts));

    public static string ProjectCollectionServiceAccounts(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionServiceAccounts), culture);

    public static string ProjectCollectionServiceAccountsDescription() => FrameworkResources.Get(nameof (ProjectCollectionServiceAccountsDescription));

    public static string ProjectCollectionServiceAccountsDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionServiceAccountsDescription), culture);

    public static string ProjectCollectionValidUsers() => FrameworkResources.Get(nameof (ProjectCollectionValidUsers));

    public static string ProjectCollectionValidUsers(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionValidUsers), culture);

    public static string ProjectCollectionValidUsersDescription() => FrameworkResources.Get(nameof (ProjectCollectionValidUsersDescription));

    public static string ProjectCollectionValidUsersDescription(CultureInfo culture) => FrameworkResources.Get(nameof (ProjectCollectionValidUsersDescription), culture);

    public static string HostCannotBeDeletedBecauseItHasChildrenException(object arg0) => FrameworkResources.Format(nameof (HostCannotBeDeletedBecauseItHasChildrenException), arg0);

    public static string HostCannotBeDeletedBecauseItHasChildrenException(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (HostCannotBeDeletedBecauseItHasChildrenException), culture, arg0);
    }

    public static string TimeToFirstPageTimerAlreadyPausedExceptionMessage() => FrameworkResources.Get(nameof (TimeToFirstPageTimerAlreadyPausedExceptionMessage));

    public static string TimeToFirstPageTimerAlreadyPausedExceptionMessage(CultureInfo culture) => FrameworkResources.Get(nameof (TimeToFirstPageTimerAlreadyPausedExceptionMessage), culture);

    public static string ServiceInstanceNotFoundMessage(object arg0) => FrameworkResources.Format(nameof (ServiceInstanceNotFoundMessage), arg0);

    public static string ServiceInstanceNotFoundMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ServiceInstanceNotFoundMessage), culture, arg0);

    public static string AccessControlServiceNotSupported() => FrameworkResources.Get(nameof (AccessControlServiceNotSupported));

    public static string AccessControlServiceNotSupported(CultureInfo culture) => FrameworkResources.Get(nameof (AccessControlServiceNotSupported), culture);

    public static string VsEnterpriseLicenseDescription() => FrameworkResources.Get(nameof (VsEnterpriseLicenseDescription));

    public static string VsEnterpriseLicenseDescription(CultureInfo culture) => FrameworkResources.Get(nameof (VsEnterpriseLicenseDescription), culture);

    public static string VsEnterpriseLicenseName() => FrameworkResources.Get(nameof (VsEnterpriseLicenseName));

    public static string VsEnterpriseLicenseName(CultureInfo culture) => FrameworkResources.Get(nameof (VsEnterpriseLicenseName), culture);

    public static string PermissionTracingRead() => FrameworkResources.Get(nameof (PermissionTracingRead));

    public static string PermissionTracingRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionTracingRead), culture);

    public static string PermissionTracingWrite() => FrameworkResources.Get(nameof (PermissionTracingWrite));

    public static string PermissionTracingWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionTracingWrite), culture);

    public static string SettingsScopePluginNotFound(object arg0) => FrameworkResources.Format(nameof (SettingsScopePluginNotFound), arg0);

    public static string SettingsScopePluginNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SettingsScopePluginNotFound), culture, arg0);

    public static string SettingsScopeInvalidUserContext() => FrameworkResources.Get(nameof (SettingsScopeInvalidUserContext));

    public static string SettingsScopeInvalidUserContext(CultureInfo culture) => FrameworkResources.Get(nameof (SettingsScopeInvalidUserContext), culture);

    public static string SettingsScopeValueNotDetermined(object arg0) => FrameworkResources.Format(nameof (SettingsScopeValueNotDetermined), arg0);

    public static string SettingsScopeValueNotDetermined(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SettingsScopeValueNotDetermined), culture, arg0);

    public static string RegionNotFoundMessage(object arg0) => FrameworkResources.Format(nameof (RegionNotFoundMessage), arg0);

    public static string RegionNotFoundMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RegionNotFoundMessage), culture, arg0);

    public static string InvalidReplyToExceptionMessage(object arg0) => FrameworkResources.Format(nameof (InvalidReplyToExceptionMessage), arg0);

    public static string InvalidReplyToExceptionMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidReplyToExceptionMessage), culture, arg0);

    public static string InvalidReplyTo() => FrameworkResources.Get(nameof (InvalidReplyTo));

    public static string InvalidReplyTo(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidReplyTo), culture);

    public static string LeaseLostException(object arg0, object arg1) => FrameworkResources.Format(nameof (LeaseLostException), arg0, arg1);

    public static string LeaseLostException(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (LeaseLostException), culture, arg0, arg1);

    public static string AcquireLeaseTimedOutException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (AcquireLeaseTimedOutException), arg0, arg1, arg2, arg3);
    }

    public static string AcquireLeaseTimedOutException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AcquireLeaseTimedOutException), culture, arg0, arg1, arg2, arg3);
    }

    public static string LeaseExpiredException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return FrameworkResources.Format(nameof (LeaseExpiredException), arg0, arg1, arg2, arg3, arg4);
    }

    public static string LeaseExpiredException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (LeaseExpiredException), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string LeaseNotHeldException(object arg0) => FrameworkResources.Format(nameof (LeaseNotHeldException), arg0);

    public static string LeaseNotHeldException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (LeaseNotHeldException), culture, arg0);

    public static string NotSupportedJsonPatchOperation(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (NotSupportedJsonPatchOperation), arg0, arg1, arg2);

    public static string NotSupportedJsonPatchOperation(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (NotSupportedJsonPatchOperation), culture, arg0, arg1, arg2);
    }

    public static string NullOperationDetected() => FrameworkResources.Get(nameof (NullOperationDetected));

    public static string NullOperationDetected(CultureInfo culture) => FrameworkResources.Get(nameof (NullOperationDetected), culture);

    public static string NotSupportedPath(object arg0, object arg1) => FrameworkResources.Format(nameof (NotSupportedPath), arg0, arg1);

    public static string NotSupportedPath(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (NotSupportedPath), culture, arg0, arg1);

    public static string WebSigninRequiredError(object arg0) => FrameworkResources.Format(nameof (WebSigninRequiredError), arg0);

    public static string WebSigninRequiredError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (WebSigninRequiredError), culture, arg0);

    public static string MessageBusAlreadySubscribingException(object arg0, object arg1) => FrameworkResources.Format(nameof (MessageBusAlreadySubscribingException), arg0, arg1);

    public static string MessageBusAlreadySubscribingException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MessageBusAlreadySubscribingException), culture, arg0, arg1);
    }

    public static string PolicyDisallowAadGuestUserAccessDescription() => FrameworkResources.Get(nameof (PolicyDisallowAadGuestUserAccessDescription));

    public static string PolicyDisallowAadGuestUserAccessDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowAadGuestUserAccessDescription), culture);

    public static string PolicyDisallowAadGuestUserAccessMoreInfoLink() => FrameworkResources.Get(nameof (PolicyDisallowAadGuestUserAccessMoreInfoLink));

    public static string PolicyDisallowAadGuestUserAccessMoreInfoLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowAadGuestUserAccessMoreInfoLink), culture);

    public static string PolicyDisallowBasicAuthenticationDescription() => FrameworkResources.Get(nameof (PolicyDisallowBasicAuthenticationDescription));

    public static string PolicyDisallowBasicAuthenticationDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowBasicAuthenticationDescription), culture);

    public static string PolicyDisallowBasicAuthenticationMoreInfoLink() => FrameworkResources.Get(nameof (PolicyDisallowBasicAuthenticationMoreInfoLink));

    public static string PolicyDisallowBasicAuthenticationMoreInfoLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowBasicAuthenticationMoreInfoLink), culture);

    public static string PolicyDisallowOAuthAuthenticationDescription() => FrameworkResources.Get(nameof (PolicyDisallowOAuthAuthenticationDescription));

    public static string PolicyDisallowOAuthAuthenticationDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowOAuthAuthenticationDescription), culture);

    public static string PolicyDisallowOAuthAuthenticationMoreInfoLink() => FrameworkResources.Get(nameof (PolicyDisallowOAuthAuthenticationMoreInfoLink));

    public static string PolicyDisallowOAuthAuthenticationMoreInfoLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowOAuthAuthenticationMoreInfoLink), culture);

    public static string PolicyDisallowSecureShellDescription() => FrameworkResources.Get(nameof (PolicyDisallowSecureShellDescription));

    public static string PolicyDisallowSecureShellDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowSecureShellDescription), culture);

    public static string PolicyDisallowSecureShellMoreInfoLink() => FrameworkResources.Get(nameof (PolicyDisallowSecureShellMoreInfoLink));

    public static string PolicyDisallowSecureShellMoreInfoLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowSecureShellMoreInfoLink), culture);

    public static string OrganizationNotFoundByContext(object arg0) => FrameworkResources.Format(nameof (OrganizationNotFoundByContext), arg0);

    public static string OrganizationNotFoundByContext(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (OrganizationNotFoundByContext), culture, arg0);

    public static string CertificateAuthenticationRequestFailed(object arg0, object arg1) => FrameworkResources.Format(nameof (CertificateAuthenticationRequestFailed), arg0, arg1);

    public static string CertificateAuthenticationRequestFailed(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CertificateAuthenticationRequestFailed), culture, arg0, arg1);
    }

    public static string ExternalProviders() => FrameworkResources.Get(nameof (ExternalProviders));

    public static string ExternalProviders(CultureInfo culture) => FrameworkResources.Get(nameof (ExternalProviders), culture);

    public static string SpsExtensionNotAvailable() => FrameworkResources.Get(nameof (SpsExtensionNotAvailable));

    public static string SpsExtensionNotAvailable(CultureInfo culture) => FrameworkResources.Get(nameof (SpsExtensionNotAvailable), culture);

    public static string ConnectionStringIsNotDetached(object arg0) => FrameworkResources.Format(nameof (ConnectionStringIsNotDetached), arg0);

    public static string ConnectionStringIsNotDetached(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ConnectionStringIsNotDetached), culture, arg0);

    public static string ConnectionStringMustUseEncryption() => FrameworkResources.Get(nameof (ConnectionStringMustUseEncryption));

    public static string ConnectionStringMustUseEncryption(CultureInfo culture) => FrameworkResources.Get(nameof (ConnectionStringMustUseEncryption), culture);

    public static string ConnectionStringMustUseSQLAuth() => FrameworkResources.Get(nameof (ConnectionStringMustUseSQLAuth));

    public static string ConnectionStringMustUseSQLAuth(CultureInfo culture) => FrameworkResources.Get(nameof (ConnectionStringMustUseSQLAuth), culture);

    public static string ConnectionStringUserMustBeInRole(object arg0, object arg1) => FrameworkResources.Format(nameof (ConnectionStringUserMustBeInRole), arg0, arg1);

    public static string ConnectionStringUserMustBeInRole(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (ConnectionStringUserMustBeInRole), culture, arg0, arg1);
    }

    public static string InvalidImportConnectionString(object arg0) => FrameworkResources.Format(nameof (InvalidImportConnectionString), arg0);

    public static string InvalidImportConnectionString(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidImportConnectionString), culture, arg0);

    public static string ConnectionStringIsNotTFSCollection(object arg0) => FrameworkResources.Format(nameof (ConnectionStringIsNotTFSCollection), arg0);

    public static string ConnectionStringIsNotTFSCollection(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ConnectionStringIsNotTFSCollection), culture, arg0);

    public static string OrganizationAdministratorsGroupName() => FrameworkResources.Get(nameof (OrganizationAdministratorsGroupName));

    public static string OrganizationAdministratorsGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (OrganizationAdministratorsGroupName), culture);

    public static string OrganizationServiceAccountsGroupName() => FrameworkResources.Get(nameof (OrganizationServiceAccountsGroupName));

    public static string OrganizationServiceAccountsGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (OrganizationServiceAccountsGroupName), culture);

    public static string OrganizationValidUsersGroupName() => FrameworkResources.Get(nameof (OrganizationValidUsersGroupName));

    public static string OrganizationValidUsersGroupName(CultureInfo culture) => FrameworkResources.Get(nameof (OrganizationValidUsersGroupName), culture);

    public static string OrganizationAdministratorsGroupDescription() => FrameworkResources.Get(nameof (OrganizationAdministratorsGroupDescription));

    public static string OrganizationAdministratorsGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (OrganizationAdministratorsGroupDescription), culture);

    public static string OrganizationServiceAccountsGroupDescription() => FrameworkResources.Get(nameof (OrganizationServiceAccountsGroupDescription));

    public static string OrganizationServiceAccountsGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (OrganizationServiceAccountsGroupDescription), culture);

    public static string OrganizationValidUsersGroupDescription() => FrameworkResources.Get(nameof (OrganizationValidUsersGroupDescription));

    public static string OrganizationValidUsersGroupDescription(CultureInfo culture) => FrameworkResources.Get(nameof (OrganizationValidUsersGroupDescription), culture);

    public static string DatabaseReadOnlyException() => FrameworkResources.Get(nameof (DatabaseReadOnlyException));

    public static string DatabaseReadOnlyException(CultureInfo culture) => FrameworkResources.Get(nameof (DatabaseReadOnlyException), culture);

    public static string BasicAuthWrongPassword() => FrameworkResources.Get(nameof (BasicAuthWrongPassword));

    public static string BasicAuthWrongPassword(CultureInfo culture) => FrameworkResources.Get(nameof (BasicAuthWrongPassword), culture);

    public static string UnsupportJsonPatchPath(object arg0, object arg1) => FrameworkResources.Format(nameof (UnsupportJsonPatchPath), arg0, arg1);

    public static string UnsupportJsonPatchPath(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (UnsupportJsonPatchPath), culture, arg0, arg1);

    public static string PolicyNewCollectionsJoinOrganizationDescription() => FrameworkResources.Get(nameof (PolicyNewCollectionsJoinOrganizationDescription));

    public static string PolicyNewCollectionsJoinOrganizationDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyNewCollectionsJoinOrganizationDescription), culture);

    public static string PermissionDataImportUseAdvancedParameter() => FrameworkResources.Get(nameof (PermissionDataImportUseAdvancedParameter));

    public static string PermissionDataImportUseAdvancedParameter(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionDataImportUseAdvancedParameter), culture);

    public static string NameIsReserved(object arg0) => FrameworkResources.Format(nameof (NameIsReserved), arg0);

    public static string NameIsReserved(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NameIsReserved), culture, arg0);

    public static string NameIsForbidden(object arg0) => FrameworkResources.Format(nameof (NameIsForbidden), arg0);

    public static string NameIsForbidden(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NameIsForbidden), culture, arg0);

    public static string AexServiceUnavailableMessage() => FrameworkResources.Get(nameof (AexServiceUnavailableMessage));

    public static string AexServiceUnavailableMessage(CultureInfo culture) => FrameworkResources.Get(nameof (AexServiceUnavailableMessage), culture);

    public static string InvalidGraphDisabledFilter(object arg0) => FrameworkResources.Format(nameof (InvalidGraphDisabledFilter), arg0);

    public static string InvalidGraphDisabledFilter(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidGraphDisabledFilter), culture, arg0);

    public static string PatExpired() => FrameworkResources.Get(nameof (PatExpired));

    public static string PatExpired(CultureInfo culture) => FrameworkResources.Get(nameof (PatExpired), culture);

    public static string InvalidSubjectKinds(object arg0) => FrameworkResources.Format(nameof (InvalidSubjectKinds), arg0);

    public static string InvalidSubjectKinds(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidSubjectKinds), culture, arg0);

    public static string InvalidSubjectTypes(object arg0) => FrameworkResources.Format(nameof (InvalidSubjectTypes), arg0);

    public static string InvalidSubjectTypes(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidSubjectTypes), culture, arg0);

    public static string RequiresContainerIdentity(object arg0) => FrameworkResources.Format(nameof (RequiresContainerIdentity), arg0);

    public static string RequiresContainerIdentity(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequiresContainerIdentity), culture, arg0);

    public static string UnableToCreateIdentityFromToken() => FrameworkResources.Get(nameof (UnableToCreateIdentityFromToken));

    public static string UnableToCreateIdentityFromToken(CultureInfo culture) => FrameworkResources.Get(nameof (UnableToCreateIdentityFromToken), culture);

    public static string SearchIdentitiesRequestContextHostMessage() => FrameworkResources.Get(nameof (SearchIdentitiesRequestContextHostMessage));

    public static string SearchIdentitiesRequestContextHostMessage(CultureInfo culture) => FrameworkResources.Get(nameof (SearchIdentitiesRequestContextHostMessage), culture);

    public static string InvalidPropertiesPatchOperationException() => FrameworkResources.Get(nameof (InvalidPropertiesPatchOperationException));

    public static string InvalidPropertiesPatchOperationException(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidPropertiesPatchOperationException), culture);

    public static string PropertiesRemovePatchPathCannotBeEmpty() => FrameworkResources.Get(nameof (PropertiesRemovePatchPathCannotBeEmpty));

    public static string PropertiesRemovePatchPathCannotBeEmpty(CultureInfo culture) => FrameworkResources.Get(nameof (PropertiesRemovePatchPathCannotBeEmpty), culture);

    public static string PropertiesReplacePatchPathCannotBeEmpty() => FrameworkResources.Get(nameof (PropertiesReplacePatchPathCannotBeEmpty));

    public static string PropertiesReplacePatchPathCannotBeEmpty(CultureInfo culture) => FrameworkResources.Get(nameof (PropertiesReplacePatchPathCannotBeEmpty), culture);

    public static string PolicyNewCollectionsJoinOrganizationMoreInfoLink() => FrameworkResources.Get(nameof (PolicyNewCollectionsJoinOrganizationMoreInfoLink));

    public static string PolicyNewCollectionsJoinOrganizationMoreInfoLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyNewCollectionsJoinOrganizationMoreInfoLink), culture);

    public static string PrThreads() => FrameworkResources.Get(nameof (PrThreads));

    public static string PrThreads(CultureInfo culture) => FrameworkResources.Get(nameof (PrThreads), culture);

    public static string AccessCheckExceptionPrivilegeFormatWithDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return FrameworkResources.Format(nameof (AccessCheckExceptionPrivilegeFormatWithDetails), arg0, arg1, arg2, arg3, arg4);
    }

    public static string AccessCheckExceptionPrivilegeFormatWithDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AccessCheckExceptionPrivilegeFormatWithDetails), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string IdentityNamespaceName() => FrameworkResources.Get(nameof (IdentityNamespaceName));

    public static string IdentityNamespaceName(CultureInfo culture) => FrameworkResources.Get(nameof (IdentityNamespaceName), culture);

    public static string SignedInNavigation() => FrameworkResources.Get(nameof (SignedInNavigation));

    public static string SignedInNavigation(CultureInfo culture) => FrameworkResources.Get(nameof (SignedInNavigation), culture);

    public static string CspIdentityAccessDenied_ServiceDoesNotAllowCspAccess() => FrameworkResources.Get(nameof (CspIdentityAccessDenied_ServiceDoesNotAllowCspAccess));

    public static string CspIdentityAccessDenied_ServiceDoesNotAllowCspAccess(CultureInfo culture) => FrameworkResources.Get(nameof (CspIdentityAccessDenied_ServiceDoesNotAllowCspAccess), culture);

    public static string CspIdentityAccessDenied_TenantsNotMatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return FrameworkResources.Format(nameof (CspIdentityAccessDenied_TenantsNotMatch), arg0, arg1, arg2, arg3);
    }

    public static string CspIdentityAccessDenied_TenantsNotMatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CspIdentityAccessDenied_TenantsNotMatch), culture, arg0, arg1, arg2, arg3);
    }

    public static string ScopeAdvancedSecurityRead() => FrameworkResources.Get(nameof (ScopeAdvancedSecurityRead));

    public static string ScopeAdvancedSecurityRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeAdvancedSecurityRead), culture);

    public static string ScopeAdvancedSecurityReadWrite() => FrameworkResources.Get(nameof (ScopeAdvancedSecurityReadWrite));

    public static string ScopeAdvancedSecurityReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeAdvancedSecurityReadWrite), culture);

    public static string ScopeAdvancedSecurityReadWriteManage() => FrameworkResources.Get(nameof (ScopeAdvancedSecurityReadWriteManage));

    public static string ScopeAdvancedSecurityReadWriteManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeAdvancedSecurityReadWriteManage), culture);

    public static string ScopeAgentPoolsRead() => FrameworkResources.Get(nameof (ScopeAgentPoolsRead));

    public static string ScopeAgentPoolsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeAgentPoolsRead), culture);

    public static string ScopeAgentPoolsReadManage() => FrameworkResources.Get(nameof (ScopeAgentPoolsReadManage));

    public static string ScopeAgentPoolsReadManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeAgentPoolsReadManage), culture);

    public static string ScopeAnalyticsRead() => FrameworkResources.Get(nameof (ScopeAnalyticsRead));

    public static string ScopeAnalyticsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeAnalyticsRead), culture);

    public static string ScopeBuildRead() => FrameworkResources.Get(nameof (ScopeBuildRead));

    public static string ScopeBuildRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeBuildRead), culture);

    public static string ScopeBuildReadExecute() => FrameworkResources.Get(nameof (ScopeBuildReadExecute));

    public static string ScopeBuildReadExecute(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeBuildReadExecute), culture);

    public static string ScopeCodeRead() => FrameworkResources.Get(nameof (ScopeCodeRead));

    public static string ScopeCodeRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeCodeRead), culture);

    public static string ScopeCodeReadWrite() => FrameworkResources.Get(nameof (ScopeCodeReadWrite));

    public static string ScopeCodeReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeCodeReadWrite), culture);

    public static string ScopeCodeReadWriteManage() => FrameworkResources.Get(nameof (ScopeCodeReadWriteManage));

    public static string ScopeCodeReadWriteManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeCodeReadWriteManage), culture);

    public static string ScopeCodeStatus() => FrameworkResources.Get(nameof (ScopeCodeStatus));

    public static string ScopeCodeStatus(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeCodeStatus), culture);

    public static string ScopeConnectedServer() => FrameworkResources.Get(nameof (ScopeConnectedServer));

    public static string ScopeConnectedServer(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeConnectedServer), culture);

    public static string ScopeEntlmntsRead() => FrameworkResources.Get(nameof (ScopeEntlmntsRead));

    public static string ScopeEntlmntsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeEntlmntsRead), culture);

    public static string ScopeExtnDataRead() => FrameworkResources.Get(nameof (ScopeExtnDataRead));

    public static string ScopeExtnDataRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeExtnDataRead), culture);

    public static string ScopeExtnDataReadWrite() => FrameworkResources.Get(nameof (ScopeExtnDataReadWrite));

    public static string ScopeExtnDataReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeExtnDataReadWrite), culture);

    public static string ScopeExtnsRead() => FrameworkResources.Get(nameof (ScopeExtnsRead));

    public static string ScopeExtnsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeExtnsRead), culture);

    public static string ScopeExtnsReadManage() => FrameworkResources.Get(nameof (ScopeExtnsReadManage));

    public static string ScopeExtnsReadManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeExtnsReadManage), culture);

    public static string ScopeGraphRead() => FrameworkResources.Get(nameof (ScopeGraphRead));

    public static string ScopeGraphRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeGraphRead), culture);

    public static string ScopeGraphManage() => FrameworkResources.Get(nameof (ScopeGraphManage));

    public static string ScopeGraphManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeGraphManage), culture);

    public static string ScopeIdentityRead() => FrameworkResources.Get(nameof (ScopeIdentityRead));

    public static string ScopeIdentityRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeIdentityRead), culture);

    public static string ScopeLoadTestRead() => FrameworkResources.Get(nameof (ScopeLoadTestRead));

    public static string ScopeLoadTestRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeLoadTestRead), culture);

    public static string ScopeLoadTestReadWrite() => FrameworkResources.Get(nameof (ScopeLoadTestReadWrite));

    public static string ScopeLoadTestReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeLoadTestReadWrite), culture);

    public static string ScopeMarketplace() => FrameworkResources.Get(nameof (ScopeMarketplace));

    public static string ScopeMarketplace(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeMarketplace), culture);

    public static string ScopeMarketplaceAcquire() => FrameworkResources.Get(nameof (ScopeMarketplaceAcquire));

    public static string ScopeMarketplaceAcquire(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeMarketplaceAcquire), culture);

    public static string ScopeMarketplaceManage() => FrameworkResources.Get(nameof (ScopeMarketplaceManage));

    public static string ScopeMarketplaceManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeMarketplaceManage), culture);

    public static string ScopeMarketplacePublish() => FrameworkResources.Get(nameof (ScopeMarketplacePublish));

    public static string ScopeMarketplacePublish(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeMarketplacePublish), culture);

    public static string ScopeNotificationDiagnostics() => FrameworkResources.Get(nameof (ScopeNotificationDiagnostics));

    public static string ScopeNotificationDiagnostics(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeNotificationDiagnostics), culture);

    public static string ScopeNotificationsManage() => FrameworkResources.Get(nameof (ScopeNotificationsManage));

    public static string ScopeNotificationsManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeNotificationsManage), culture);

    public static string ScopeNotificationsRead() => FrameworkResources.Get(nameof (ScopeNotificationsRead));

    public static string ScopeNotificationsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeNotificationsRead), culture);

    public static string ScopeNotificationsWrite() => FrameworkResources.Get(nameof (ScopeNotificationsWrite));

    public static string ScopeNotificationsWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeNotificationsWrite), culture);

    public static string ScopePackagingRead() => FrameworkResources.Get(nameof (ScopePackagingRead));

    public static string ScopePackagingRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopePackagingRead), culture);

    public static string ScopePackagingReadWrite() => FrameworkResources.Get(nameof (ScopePackagingReadWrite));

    public static string ScopePackagingReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopePackagingReadWrite), culture);

    public static string ScopePackagingReadWriteManage() => FrameworkResources.Get(nameof (ScopePackagingReadWriteManage));

    public static string ScopePackagingReadWriteManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopePackagingReadWriteManage), culture);

    public static string ScopePjTmReadWrite() => FrameworkResources.Get(nameof (ScopePjTmReadWrite));

    public static string ScopePjTmReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopePjTmReadWrite), culture);

    public static string ScopePjTmReadWriteManage() => FrameworkResources.Get(nameof (ScopePjTmReadWriteManage));

    public static string ScopePjTmReadWriteManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopePjTmReadWriteManage), culture);

    public static string ScopeProjectTeamRead() => FrameworkResources.Get(nameof (ScopeProjectTeamRead));

    public static string ScopeProjectTeamRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeProjectTeamRead), culture);

    public static string ScopeReleaseRead() => FrameworkResources.Get(nameof (ScopeReleaseRead));

    public static string ScopeReleaseRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeReleaseRead), culture);

    public static string ScopeReleaseReadWriteExecute() => FrameworkResources.Get(nameof (ScopeReleaseReadWriteExecute));

    public static string ScopeReleaseReadWriteExecute(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeReleaseReadWriteExecute), culture);

    public static string ScopeReleaseReadWriteExecuteManage() => FrameworkResources.Get(nameof (ScopeReleaseReadWriteExecuteManage));

    public static string ScopeReleaseReadWriteExecuteManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeReleaseReadWriteExecuteManage), culture);

    public static string ScopeSecurityManage() => FrameworkResources.Get(nameof (ScopeSecurityManage));

    public static string ScopeSecurityManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeSecurityManage), culture);

    public static string ScopeSvcEndptsRead() => FrameworkResources.Get(nameof (ScopeSvcEndptsRead));

    public static string ScopeSvcEndptsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeSvcEndptsRead), culture);

    public static string ScopeSvcEndptsReadQuery() => FrameworkResources.Get(nameof (ScopeSvcEndptsReadQuery));

    public static string ScopeSvcEndptsReadQuery(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeSvcEndptsReadQuery), culture);

    public static string ScopeSvcEndptsReadQueryManage() => FrameworkResources.Get(nameof (ScopeSvcEndptsReadQueryManage));

    public static string ScopeSvcEndptsReadQueryManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeSvcEndptsReadQueryManage), culture);

    public static string ScopeTeamDshbdsManage() => FrameworkResources.Get(nameof (ScopeTeamDshbdsManage));

    public static string ScopeTeamDshbdsManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTeamDshbdsManage), culture);

    public static string ScopeTeamDshbdsRead() => FrameworkResources.Get(nameof (ScopeTeamDshbdsRead));

    public static string ScopeTeamDshbdsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTeamDshbdsRead), culture);

    public static string ScopeTestMgmtRead() => FrameworkResources.Get(nameof (ScopeTestMgmtRead));

    public static string ScopeTestMgmtRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTestMgmtRead), culture);

    public static string ScopeTestMgmtReadWrite() => FrameworkResources.Get(nameof (ScopeTestMgmtReadWrite));

    public static string ScopeTestMgmtReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTestMgmtReadWrite), culture);

    public static string ScopeTskGrpsRead() => FrameworkResources.Get(nameof (ScopeTskGrpsRead));

    public static string ScopeTskGrpsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTskGrpsRead), culture);

    public static string ScopeTskGrpsReadCreate() => FrameworkResources.Get(nameof (ScopeTskGrpsReadCreate));

    public static string ScopeTskGrpsReadCreate(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTskGrpsReadCreate), culture);

    public static string ScopeTskGrpsReadCreateManage() => FrameworkResources.Get(nameof (ScopeTskGrpsReadCreateManage));

    public static string ScopeTskGrpsReadCreateManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTskGrpsReadCreateManage), culture);

    public static string ScopeUserProfileRead() => FrameworkResources.Get(nameof (ScopeUserProfileRead));

    public static string ScopeUserProfileRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeUserProfileRead), culture);

    public static string ScopeUserProfileWrite() => FrameworkResources.Get(nameof (ScopeUserProfileWrite));

    public static string ScopeUserProfileWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeUserProfileWrite), culture);

    public static string ScopeVarGrpsRead() => FrameworkResources.Get(nameof (ScopeVarGrpsRead));

    public static string ScopeVarGrpsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeVarGrpsRead), culture);

    public static string ScopeVarGrpsReadCreate() => FrameworkResources.Get(nameof (ScopeVarGrpsReadCreate));

    public static string ScopeVarGrpsReadCreate(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeVarGrpsReadCreate), culture);

    public static string ScopeVarGrpsReadCreateManage() => FrameworkResources.Get(nameof (ScopeVarGrpsReadCreateManage));

    public static string ScopeVarGrpsReadCreateManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeVarGrpsReadCreateManage), culture);

    public static string ScopeWorkItemSearchRead() => FrameworkResources.Get(nameof (ScopeWorkItemSearchRead));

    public static string ScopeWorkItemSearchRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeWorkItemSearchRead), culture);

    public static string ScopeWorkItemsRead() => FrameworkResources.Get(nameof (ScopeWorkItemsRead));

    public static string ScopeWorkItemsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeWorkItemsRead), culture);

    public static string ScopeWorkItemsReadWrite() => FrameworkResources.Get(nameof (ScopeWorkItemsReadWrite));

    public static string ScopeWorkItemsReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeWorkItemsReadWrite), culture);

    public static string PermissionSettingEntriesRead() => FrameworkResources.Get(nameof (PermissionSettingEntriesRead));

    public static string PermissionSettingEntriesRead(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionSettingEntriesRead), culture);

    public static string PermissionSettingEntriesWrite() => FrameworkResources.Get(nameof (PermissionSettingEntriesWrite));

    public static string PermissionSettingEntriesWrite(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionSettingEntriesWrite), culture);

    public static string RequiresNonContainerIdentity(object arg0) => FrameworkResources.Format(nameof (RequiresNonContainerIdentity), arg0);

    public static string RequiresNonContainerIdentity(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequiresNonContainerIdentity), culture, arg0);

    public static string RequiresNonServicePrincipalIdentity(object arg0) => FrameworkResources.Format(nameof (RequiresNonServicePrincipalIdentity), arg0);

    public static string RequiresNonServicePrincipalIdentity(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequiresNonServicePrincipalIdentity), culture, arg0);

    public static string InvalidMetaType(object arg0) => FrameworkResources.Format(nameof (InvalidMetaType), arg0);

    public static string InvalidMetaType(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidMetaType), culture, arg0);

    public static string ErrorInitializingCacheStatistics(object arg0) => FrameworkResources.Format(nameof (ErrorInitializingCacheStatistics), arg0);

    public static string ErrorInitializingCacheStatistics(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ErrorInitializingCacheStatistics), culture, arg0);

    public static string InfoFileCacheShutsDownWhileStatisticInitializing(object arg0) => FrameworkResources.Format(nameof (InfoFileCacheShutsDownWhileStatisticInitializing), arg0);

    public static string InfoFileCacheShutsDownWhileStatisticInitializing(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (InfoFileCacheShutsDownWhileStatisticInitializing), culture, arg0);
    }

    public static string ScopeWorkItemsFull() => FrameworkResources.Get(nameof (ScopeWorkItemsFull));

    public static string ScopeWorkItemsFull(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeWorkItemsFull), culture);

    public static string ScopeCodeFull() => FrameworkResources.Get(nameof (ScopeCodeFull));

    public static string ScopeCodeFull(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeCodeFull), culture);

    public static string JsonPatchOperationNotSupported(object arg0) => FrameworkResources.Format(nameof (JsonPatchOperationNotSupported), arg0);

    public static string JsonPatchOperationNotSupported(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (JsonPatchOperationNotSupported), culture, arg0);

    public static string RequestValidationException(object arg0) => FrameworkResources.Format(nameof (RequestValidationException), arg0);

    public static string RequestValidationException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RequestValidationException), culture, arg0);

    public static string UnauthorizedWriteException() => FrameworkResources.Get(nameof (UnauthorizedWriteException));

    public static string UnauthorizedWriteException(CultureInfo culture) => FrameworkResources.Get(nameof (UnauthorizedWriteException), culture);

    public static string ScopeCodeSearchRead() => FrameworkResources.Get(nameof (ScopeCodeSearchRead));

    public static string ScopeCodeSearchRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeCodeSearchRead), culture);

    public static string ScopeIdentityManage() => FrameworkResources.Get(nameof (ScopeIdentityManage));

    public static string ScopeIdentityManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeIdentityManage), culture);

    public static string ScopeWikiRead() => FrameworkResources.Get(nameof (ScopeWikiRead));

    public static string ScopeWikiRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeWikiRead), culture);

    public static string ScopeWikiReadWrite() => FrameworkResources.Get(nameof (ScopeWikiReadWrite));

    public static string ScopeWikiReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeWikiReadWrite), culture);

    public static string ClientRequestThrottledException() => FrameworkResources.Get(nameof (ClientRequestThrottledException));

    public static string ClientRequestThrottledException(CultureInfo culture) => FrameworkResources.Get(nameof (ClientRequestThrottledException), culture);

    public static string ScopeDeploymentGroupReadManage() => FrameworkResources.Get(nameof (ScopeDeploymentGroupReadManage));

    public static string ScopeDeploymentGroupReadManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeDeploymentGroupReadManage), culture);

    public static string ScopesSymbolsRead() => FrameworkResources.Get(nameof (ScopesSymbolsRead));

    public static string ScopesSymbolsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopesSymbolsRead), culture);

    public static string ScopesSymbolsReadWrite() => FrameworkResources.Get(nameof (ScopesSymbolsReadWrite));

    public static string ScopesSymbolsReadWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopesSymbolsReadWrite), culture);

    public static string ScopesSymbolsReadWriteManage() => FrameworkResources.Get(nameof (ScopesSymbolsReadWriteManage));

    public static string ScopesSymbolsReadWriteManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopesSymbolsReadWriteManage), culture);

    public static string SettingsScopeAnonymousNotAllowed() => FrameworkResources.Get(nameof (SettingsScopeAnonymousNotAllowed));

    public static string SettingsScopeAnonymousNotAllowed(CultureInfo culture) => FrameworkResources.Get(nameof (SettingsScopeAnonymousNotAllowed), culture);

    public static string SettingsScopeValueInvalid(object arg0, object arg1) => FrameworkResources.Format(nameof (SettingsScopeValueInvalid), arg0, arg1);

    public static string SettingsScopeValueInvalid(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (SettingsScopeValueInvalid), culture, arg0, arg1);

    public static string FileCacheScanCompleted(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5)
    {
      return FrameworkResources.Format(nameof (FileCacheScanCompleted), arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string FileCacheScanCompleted(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (FileCacheScanCompleted), culture, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string AvatarImageParseError() => FrameworkResources.Get(nameof (AvatarImageParseError));

    public static string AvatarImageParseError(CultureInfo culture) => FrameworkResources.Get(nameof (AvatarImageParseError), culture);

    public static string CannotStartDeletingHostException(object arg0) => FrameworkResources.Format(nameof (CannotStartDeletingHostException), arg0);

    public static string CannotStartDeletingHostException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (CannotStartDeletingHostException), culture, arg0);

    public static string NoParentTokenForFlatSecurityNamespace(object arg0) => FrameworkResources.Format(nameof (NoParentTokenForFlatSecurityNamespace), arg0);

    public static string NoParentTokenForFlatSecurityNamespace(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NoParentTokenForFlatSecurityNamespace), culture, arg0);

    public static string NoParentTokenForRootToken(object arg0) => FrameworkResources.Format(nameof (NoParentTokenForRootToken), arg0);

    public static string NoParentTokenForRootToken(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (NoParentTokenForRootToken), culture, arg0);

    public static string NullDocumentDBComponent() => FrameworkResources.Get(nameof (NullDocumentDBComponent));

    public static string NullDocumentDBComponent(CultureInfo culture) => FrameworkResources.Get(nameof (NullDocumentDBComponent), culture);

    public static string ModifyDataConnectionReadOnly() => FrameworkResources.Get(nameof (ModifyDataConnectionReadOnly));

    public static string ModifyDataConnectionReadOnly(CultureInfo culture) => FrameworkResources.Get(nameof (ModifyDataConnectionReadOnly), culture);

    public static string NullETagOCCException() => FrameworkResources.Get(nameof (NullETagOCCException));

    public static string NullETagOCCException(CultureInfo culture) => FrameworkResources.Get(nameof (NullETagOCCException), culture);

    public static string PolicyAllowAnonymousAccessDescription() => FrameworkResources.Get(nameof (PolicyAllowAnonymousAccessDescription));

    public static string PolicyAllowAnonymousAccessDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyAllowAnonymousAccessDescription), culture);

    public static string PolicyAllowAnonymousAccessMoreInfoLink() => FrameworkResources.Get(nameof (PolicyAllowAnonymousAccessMoreInfoLink));

    public static string PolicyAllowAnonymousAccessMoreInfoLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyAllowAnonymousAccessMoreInfoLink), culture);

    public static string PolicyAllowOrgAccessDescription() => FrameworkResources.Get(nameof (PolicyAllowOrgAccessDescription));

    public static string PolicyAllowOrgAccessDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyAllowOrgAccessDescription), culture);

    public static string PolicyEmptyInfoLink() => FrameworkResources.Get(nameof (PolicyEmptyInfoLink));

    public static string PolicyEmptyInfoLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyEmptyInfoLink), culture);

    public static string InvalidDescriptor(object arg0) => FrameworkResources.Format(nameof (InvalidDescriptor), arg0);

    public static string InvalidDescriptor(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidDescriptor), culture, arg0);

    public static string TokenUserIdSpecifiedWithoutSystemContext() => FrameworkResources.Get(nameof (TokenUserIdSpecifiedWithoutSystemContext));

    public static string TokenUserIdSpecifiedWithoutSystemContext(CultureInfo culture) => FrameworkResources.Get(nameof (TokenUserIdSpecifiedWithoutSystemContext), culture);

    public static string ScopeMemberEntitlementManagementRead() => FrameworkResources.Get(nameof (ScopeMemberEntitlementManagementRead));

    public static string ScopeMemberEntitlementManagementRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeMemberEntitlementManagementRead), culture);

    public static string ScopeMemberEntitlementManagementWrite() => FrameworkResources.Get(nameof (ScopeMemberEntitlementManagementWrite));

    public static string ScopeMemberEntitlementManagementWrite(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeMemberEntitlementManagementWrite), culture);

    public static string MissingRequiredHeaderError(object arg0) => FrameworkResources.Format(nameof (MissingRequiredHeaderError), arg0);

    public static string MissingRequiredHeaderError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MissingRequiredHeaderError), culture, arg0);

    public static string MultipleHeaderValuesError(object arg0) => FrameworkResources.Format(nameof (MultipleHeaderValuesError), arg0);

    public static string MultipleHeaderValuesError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MultipleHeaderValuesError), culture, arg0);

    public static string RequestToStartJobWasCanceled() => FrameworkResources.Get(nameof (RequestToStartJobWasCanceled));

    public static string RequestToStartJobWasCanceled(CultureInfo culture) => FrameworkResources.Get(nameof (RequestToStartJobWasCanceled), culture);

    public static string Minute(object arg0) => FrameworkResources.Format(nameof (Minute), arg0);

    public static string Minute(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (Minute), culture, arg0);

    public static string Minutes(object arg0) => FrameworkResources.Format(nameof (Minutes), arg0);

    public static string Minutes(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (Minutes), culture, arg0);

    public static string CacheCleanupComplete(object arg0, object arg1, object arg2, object arg3) => FrameworkResources.Format(nameof (CacheCleanupComplete), arg0, arg1, arg2, arg3);

    public static string CacheCleanupComplete(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CacheCleanupComplete), culture, arg0, arg1, arg2, arg3);
    }

    public static string CrossDataspaceAccessException(object arg0, object arg1) => FrameworkResources.Format(nameof (CrossDataspaceAccessException), arg0, arg1);

    public static string CrossDataspaceAccessException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (CrossDataspaceAccessException), culture, arg0, arg1);
    }

    public static string PublicDisplayNameForBindPendingIdentity() => FrameworkResources.Get(nameof (PublicDisplayNameForBindPendingIdentity));

    public static string PublicDisplayNameForBindPendingIdentity(CultureInfo culture) => FrameworkResources.Get(nameof (PublicDisplayNameForBindPendingIdentity), culture);

    public static string RegionCodeContainsInvalidCharacter(object arg0) => FrameworkResources.Format(nameof (RegionCodeContainsInvalidCharacter), arg0);

    public static string RegionCodeContainsInvalidCharacter(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RegionCodeContainsInvalidCharacter), culture, arg0);

    public static string RegionDoesNotExist(object arg0) => FrameworkResources.Format(nameof (RegionDoesNotExist), arg0);

    public static string RegionDoesNotExist(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RegionDoesNotExist), culture, arg0);

    public static string RegionCodeTooLong(object arg0, object arg1) => FrameworkResources.Format(nameof (RegionCodeTooLong), arg0, arg1);

    public static string RegionCodeTooLong(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (RegionCodeTooLong), culture, arg0, arg1);

    public static string RegionCodeIsNullOrOnlyContainsWhiteSpacce(object arg0) => FrameworkResources.Format(nameof (RegionCodeIsNullOrOnlyContainsWhiteSpacce), arg0);

    public static string RegionCodeIsNullOrOnlyContainsWhiteSpacce(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (RegionCodeIsNullOrOnlyContainsWhiteSpacce), culture, arg0);

    public static string CannotSetEarlyAdopterStageOnInfrastructureHost() => FrameworkResources.Get(nameof (CannotSetEarlyAdopterStageOnInfrastructureHost));

    public static string CannotSetEarlyAdopterStageOnInfrastructureHost(CultureInfo culture) => FrameworkResources.Get(nameof (CannotSetEarlyAdopterStageOnInfrastructureHost), culture);

    public static string PolicyAllowRemoteWorkItemLinkingMoreInfoLink() => FrameworkResources.Get(nameof (PolicyAllowRemoteWorkItemLinkingMoreInfoLink));

    public static string PolicyAllowRemoteWorkItemLinkingMoreInfoLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyAllowRemoteWorkItemLinkingMoreInfoLink), culture);

    public static string PolicyConditionalAccessPolicyDescription() => FrameworkResources.Get(nameof (PolicyConditionalAccessPolicyDescription));

    public static string PolicyConditionalAccessPolicyDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyConditionalAccessPolicyDescription), culture);

    public static string PolicyConditionalAccessPolicyLink() => FrameworkResources.Get(nameof (PolicyConditionalAccessPolicyLink));

    public static string PolicyConditionalAccessPolicyLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyConditionalAccessPolicyLink), culture);

    public static string ConditionalAccessPolicyForbidsAccess() => FrameworkResources.Get(nameof (ConditionalAccessPolicyForbidsAccess));

    public static string ConditionalAccessPolicyForbidsAccess(CultureInfo culture) => FrameworkResources.Get(nameof (ConditionalAccessPolicyForbidsAccess), culture);

    public static string ResourceIdAlreadyExistsException(object arg0) => FrameworkResources.Format(nameof (ResourceIdAlreadyExistsException), arg0);

    public static string ResourceIdAlreadyExistsException(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ResourceIdAlreadyExistsException), culture, arg0);

    public static string HostHasUndrainedActiveRequests(object arg0) => FrameworkResources.Format(nameof (HostHasUndrainedActiveRequests), arg0);

    public static string HostHasUndrainedActiveRequests(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostHasUndrainedActiveRequests), culture, arg0);

    public static string HostInReadOnlyModeForMaintenance() => FrameworkResources.Get(nameof (HostInReadOnlyModeForMaintenance));

    public static string HostInReadOnlyModeForMaintenance(CultureInfo culture) => FrameworkResources.Get(nameof (HostInReadOnlyModeForMaintenance), culture);

    public static string LogAuditEventNotInTransaction() => FrameworkResources.Get(nameof (LogAuditEventNotInTransaction));

    public static string LogAuditEventNotInTransaction(CultureInfo culture) => FrameworkResources.Get(nameof (LogAuditEventNotInTransaction), culture);

    public static string MultipleParametersProvidedError3(object arg0, object arg1, object arg2) => FrameworkResources.Format(nameof (MultipleParametersProvidedError3), arg0, arg1, arg2);

    public static string MultipleParametersProvidedError3(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (MultipleParametersProvidedError3), culture, arg0, arg1, arg2);
    }

    public static string AuditSecurityAllow() => FrameworkResources.Get(nameof (AuditSecurityAllow));

    public static string AuditSecurityAllow(CultureInfo culture) => FrameworkResources.Get(nameof (AuditSecurityAllow), culture);

    public static string AuditSecurityDeny() => FrameworkResources.Get(nameof (AuditSecurityDeny));

    public static string AuditSecurityDeny(CultureInfo culture) => FrameworkResources.Get(nameof (AuditSecurityDeny), culture);

    public static string AuditSecurityReset() => FrameworkResources.Get(nameof (AuditSecurityReset));

    public static string AuditSecurityReset(CultureInfo culture) => FrameworkResources.Get(nameof (AuditSecurityReset), culture);

    public static string AuditSecurityAnd(object arg0, object arg1) => FrameworkResources.Format(nameof (AuditSecurityAnd), arg0, arg1);

    public static string AuditSecurityAnd(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (AuditSecurityAnd), culture, arg0, arg1);

    public static string AuditSecurityAndOtherIdentities(object arg0, object arg1) => FrameworkResources.Format(nameof (AuditSecurityAndOtherIdentities), arg0, arg1);

    public static string AuditSecurityAndOtherIdentities(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (AuditSecurityAndOtherIdentities), culture, arg0, arg1);
    }

    public static string ScopeTokenAdministration() => FrameworkResources.Get(nameof (ScopeTokenAdministration));

    public static string ScopeTokenAdministration(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTokenAdministration), culture);

    public static string ScopeTokens() => FrameworkResources.Get(nameof (ScopeTokens));

    public static string ScopeTokens(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeTokens), culture);

    public static string MailServiceMissingPlugin(object arg0) => FrameworkResources.Format(nameof (MailServiceMissingPlugin), arg0);

    public static string MailServiceMissingPlugin(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MailServiceMissingPlugin), culture, arg0);

    public static string SpsHostInReadOnlyMode() => FrameworkResources.Get(nameof (SpsHostInReadOnlyMode));

    public static string SpsHostInReadOnlyMode(CultureInfo culture) => FrameworkResources.Get(nameof (SpsHostInReadOnlyMode), culture);

    public static string PermissionIdentityRestoreScope() => FrameworkResources.Get(nameof (PermissionIdentityRestoreScope));

    public static string PermissionIdentityRestoreScope(CultureInfo culture) => FrameworkResources.Get(nameof (PermissionIdentityRestoreScope), culture);

    public static string AuditReadLogScope() => FrameworkResources.Get(nameof (AuditReadLogScope));

    public static string AuditReadLogScope(CultureInfo culture) => FrameworkResources.Get(nameof (AuditReadLogScope), culture);

    public static string AuditManageStreamsScope() => FrameworkResources.Get(nameof (AuditManageStreamsScope));

    public static string AuditManageStreamsScope(CultureInfo culture) => FrameworkResources.Get(nameof (AuditManageStreamsScope), culture);

    public static string AuditDeleteStreamsScope() => FrameworkResources.Get(nameof (AuditDeleteStreamsScope));

    public static string AuditDeleteStreamsScope(CultureInfo culture) => FrameworkResources.Get(nameof (AuditDeleteStreamsScope), culture);

    public static string AuditSecurityResetPermissionNames() => FrameworkResources.Get(nameof (AuditSecurityResetPermissionNames));

    public static string AuditSecurityResetPermissionNames(CultureInfo culture) => FrameworkResources.Get(nameof (AuditSecurityResetPermissionNames), culture);

    public static string DrawerNotFoundExceptionById(object arg0) => FrameworkResources.Format(nameof (DrawerNotFoundExceptionById), arg0);

    public static string DrawerNotFoundExceptionById(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (DrawerNotFoundExceptionById), culture, arg0);

    public static string InvalidPathEmpty(object arg0) => FrameworkResources.Format(nameof (InvalidPathEmpty), arg0);

    public static string InvalidPathEmpty(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidPathEmpty), culture, arg0);

    public static string InvalidPathSegmentTooLong(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidPathSegmentTooLong), arg0, arg1);

    public static string InvalidPathSegmentTooLong(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidPathSegmentTooLong), culture, arg0, arg1);

    public static string InvalidPathSpacePeriodSegment(object arg0) => FrameworkResources.Format(nameof (InvalidPathSpacePeriodSegment), arg0);

    public static string InvalidPathSpacePeriodSegment(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidPathSpacePeriodSegment), culture, arg0);

    public static string InvalidPathNotCanonical(object arg0, object arg1) => FrameworkResources.Format(nameof (InvalidPathNotCanonical), arg0, arg1);

    public static string InvalidPathNotCanonical(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidPathNotCanonical), culture, arg0, arg1);

    public static string CannotChangeOtherUserPicture() => FrameworkResources.Get(nameof (CannotChangeOtherUserPicture));

    public static string CannotChangeOtherUserPicture(CultureInfo culture) => FrameworkResources.Get(nameof (CannotChangeOtherUserPicture), culture);

    public static string CannotChangePicturePermissions() => FrameworkResources.Get(nameof (CannotChangePicturePermissions));

    public static string CannotChangePicturePermissions(CultureInfo culture) => FrameworkResources.Get(nameof (CannotChangePicturePermissions), culture);

    public static string CannotChangeWellKnownPicture() => FrameworkResources.Get(nameof (CannotChangeWellKnownPicture));

    public static string CannotChangeWellKnownPicture(CultureInfo culture) => FrameworkResources.Get(nameof (CannotChangeWellKnownPicture), culture);

    public static string NoDnsAddressForHost(object arg0, object arg1) => FrameworkResources.Format(nameof (NoDnsAddressForHost), arg0, arg1);

    public static string NoDnsAddressForHost(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (NoDnsAddressForHost), culture, arg0, arg1);

    public static string UriMissingHostSegment() => FrameworkResources.Get(nameof (UriMissingHostSegment));

    public static string UriMissingHostSegment(CultureInfo culture) => FrameworkResources.Get(nameof (UriMissingHostSegment), culture);

    public static string SettingsScopeValueInvalidForGlobalScope() => FrameworkResources.Get(nameof (SettingsScopeValueInvalidForGlobalScope));

    public static string SettingsScopeValueInvalidForGlobalScope(CultureInfo culture) => FrameworkResources.Get(nameof (SettingsScopeValueInvalidForGlobalScope), culture);

    public static string SecureFilesRead() => FrameworkResources.Get(nameof (SecureFilesRead));

    public static string SecureFilesRead(CultureInfo culture) => FrameworkResources.Get(nameof (SecureFilesRead), culture);

    public static string SecureFilesReadCreate() => FrameworkResources.Get(nameof (SecureFilesReadCreate));

    public static string SecureFilesReadCreate(CultureInfo culture) => FrameworkResources.Get(nameof (SecureFilesReadCreate), culture);

    public static string SecureFilesReadCreateManage() => FrameworkResources.Get(nameof (SecureFilesReadCreateManage));

    public static string SecureFilesReadCreateManage(CultureInfo culture) => FrameworkResources.Get(nameof (SecureFilesReadCreateManage), culture);

    public static string PolicyGitHubInviteDescription() => FrameworkResources.Get(nameof (PolicyGitHubInviteDescription));

    public static string PolicyGitHubInviteDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyGitHubInviteDescription), culture);

    public static string VSDInvalidPagingToken(object arg0) => FrameworkResources.Format(nameof (VSDInvalidPagingToken), arg0);

    public static string VSDInvalidPagingToken(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (VSDInvalidPagingToken), culture, arg0);

    public static string InvalidParameter(object arg0) => FrameworkResources.Format(nameof (InvalidParameter), arg0);

    public static string InvalidParameter(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (InvalidParameter), culture, arg0);

    public static string MissingParameter(object arg0) => FrameworkResources.Format(nameof (MissingParameter), arg0);

    public static string MissingParameter(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MissingParameter), culture, arg0);

    public static string NotSignedByMe() => FrameworkResources.Get(nameof (NotSignedByMe));

    public static string NotSignedByMe(CultureInfo culture) => FrameworkResources.Get(nameof (NotSignedByMe), culture);

    public static string SignedUriExpired() => FrameworkResources.Get(nameof (SignedUriExpired));

    public static string SignedUriExpired(CultureInfo culture) => FrameworkResources.Get(nameof (SignedUriExpired), culture);

    public static string SignedUrlSignatureNotMatch() => FrameworkResources.Get(nameof (SignedUrlSignatureNotMatch));

    public static string SignedUrlSignatureNotMatch(CultureInfo culture) => FrameworkResources.Get(nameof (SignedUrlSignatureNotMatch), culture);

    public static string DuplicateSecurityNamespaceTemplate(object arg0, object arg1) => FrameworkResources.Format(nameof (DuplicateSecurityNamespaceTemplate), arg0, arg1);

    public static string DuplicateSecurityNamespaceTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (DuplicateSecurityNamespaceTemplate), culture, arg0, arg1);
    }

    public static string MaxWaiterThreadLimitExceeded(object arg0) => FrameworkResources.Format(nameof (MaxWaiterThreadLimitExceeded), arg0);

    public static string MaxWaiterThreadLimitExceeded(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (MaxWaiterThreadLimitExceeded), culture, arg0);

    public static string HostOfflineForMigration(object arg0) => FrameworkResources.Format(nameof (HostOfflineForMigration), arg0);

    public static string HostOfflineForMigration(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (HostOfflineForMigration), culture, arg0);

    public static string TenantPolicyOrganizationCreationRestrictionDefaultErrorMessage() => FrameworkResources.Get(nameof (TenantPolicyOrganizationCreationRestrictionDefaultErrorMessage));

    public static string TenantPolicyOrganizationCreationRestrictionDefaultErrorMessage(
      CultureInfo culture)
    {
      return FrameworkResources.Get(nameof (TenantPolicyOrganizationCreationRestrictionDefaultErrorMessage), culture);
    }

    public static string TenantPolicyOrganizationCreationRestrictionDescription() => FrameworkResources.Get(nameof (TenantPolicyOrganizationCreationRestrictionDescription));

    public static string TenantPolicyOrganizationCreationRestrictionDescription(CultureInfo culture) => FrameworkResources.Get(nameof (TenantPolicyOrganizationCreationRestrictionDescription), culture);

    public static string TenantPolicyNotExist(object arg0) => FrameworkResources.Format(nameof (TenantPolicyNotExist), arg0);

    public static string TenantPolicyNotExist(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (TenantPolicyNotExist), culture, arg0);

    public static string TenantPolicyPermissionValidationFailed(object arg0, object arg1) => FrameworkResources.Format(nameof (TenantPolicyPermissionValidationFailed), arg0, arg1);

    public static string TenantPolicyPermissionValidationFailed(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (TenantPolicyPermissionValidationFailed), culture, arg0, arg1);
    }

    public static string TenantPolicyNameDataDoNotMatch(object arg0, object arg1) => FrameworkResources.Format(nameof (TenantPolicyNameDataDoNotMatch), arg0, arg1);

    public static string TenantPolicyNameDataDoNotMatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (TenantPolicyNameDataDoNotMatch), culture, arg0, arg1);
    }

    public static string ClientRequestThrottledRateLimitReachedException() => FrameworkResources.Get(nameof (ClientRequestThrottledRateLimitReachedException));

    public static string ClientRequestThrottledRateLimitReachedException(CultureInfo culture) => FrameworkResources.Get(nameof (ClientRequestThrottledRateLimitReachedException), culture);

    public static string TenantPolicyInvalidProperty(object arg0) => FrameworkResources.Format(nameof (TenantPolicyInvalidProperty), arg0);

    public static string TenantPolicyInvalidProperty(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (TenantPolicyInvalidProperty), culture, arg0);

    public static string TenantPolicyInvalidTenantId(object arg0) => FrameworkResources.Format(nameof (TenantPolicyInvalidTenantId), arg0);

    public static string TenantPolicyInvalidTenantId(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (TenantPolicyInvalidTenantId), culture, arg0);

    public static string ScopeEnvironmentReadManage() => FrameworkResources.Get(nameof (ScopeEnvironmentReadManage));

    public static string ScopeEnvironmentReadManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeEnvironmentReadManage), culture);

    public static string InvalidScopes() => FrameworkResources.Get(nameof (InvalidScopes));

    public static string InvalidScopes(CultureInfo culture) => FrameworkResources.Get(nameof (InvalidScopes), culture);

    public static string UnauthorizedUserErrorWithMessage(object arg0, object arg1) => FrameworkResources.Format(nameof (UnauthorizedUserErrorWithMessage), arg0, arg1);

    public static string UnauthorizedUserErrorWithMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (UnauthorizedUserErrorWithMessage), culture, arg0, arg1);
    }

    public static string PolicyAllowRequestAccessDescription() => FrameworkResources.Get(nameof (PolicyAllowRequestAccessDescription));

    public static string PolicyAllowRequestAccessDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyAllowRequestAccessDescription), culture);

    public static string PolicyAllowRequestAccessLink() => FrameworkResources.Get(nameof (PolicyAllowRequestAccessLink));

    public static string PolicyAllowRequestAccessLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyAllowRequestAccessLink), culture);

    public static string SigningKeyNotFound(object arg0) => FrameworkResources.Format(nameof (SigningKeyNotFound), arg0);

    public static string SigningKeyNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SigningKeyNotFound), culture, arg0);

    public static string PolicyTeamAdminsInvitationsDescription() => FrameworkResources.Get(nameof (PolicyTeamAdminsInvitationsDescription));

    public static string PolicyTeamAdminsInvitationsDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyTeamAdminsInvitationsDescription), culture);

    public static string PolicyTeamAdminsInvitationsLink() => FrameworkResources.Get(nameof (PolicyTeamAdminsInvitationsLink));

    public static string PolicyTeamAdminsInvitationsLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyTeamAdminsInvitationsLink), culture);

    public static string SigningKeyRotated() => FrameworkResources.Get(nameof (SigningKeyRotated));

    public static string SigningKeyRotated(CultureInfo culture) => FrameworkResources.Get(nameof (SigningKeyRotated), culture);

    public static string UserIsDeletedFromAccount() => FrameworkResources.Get(nameof (UserIsDeletedFromAccount));

    public static string UserIsDeletedFromAccount(CultureInfo culture) => FrameworkResources.Get(nameof (UserIsDeletedFromAccount), culture);

    public static string GroupNameIsReservedBySystemError(object arg0) => FrameworkResources.Format(nameof (GroupNameIsReservedBySystemError), arg0);

    public static string GroupNameIsReservedBySystemError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (GroupNameIsReservedBySystemError), culture, arg0);

    public static string PolicyExternalPackageProtectionDescription() => FrameworkResources.Get(nameof (PolicyExternalPackageProtectionDescription));

    public static string PolicyExternalPackageProtectionDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyExternalPackageProtectionDescription), culture);

    public static string PolicyExternalPackageProtectionLink() => FrameworkResources.Get(nameof (PolicyExternalPackageProtectionLink));

    public static string PolicyExternalPackageProtectionLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyExternalPackageProtectionLink), culture);

    public static string FileContainerContentForFileError(object arg0) => FrameworkResources.Format(nameof (FileContainerContentForFileError), arg0);

    public static string FileContainerContentForFileError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (FileContainerContentForFileError), culture, arg0);

    public static string UnsupportedBlobUpload(object arg0) => FrameworkResources.Format(nameof (UnsupportedBlobUpload), arg0);

    public static string UnsupportedBlobUpload(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnsupportedBlobUpload), culture, arg0);

    public static string IncludePropertiesNotSupported() => FrameworkResources.Get(nameof (IncludePropertiesNotSupported));

    public static string IncludePropertiesNotSupported(CultureInfo culture) => FrameworkResources.Get(nameof (IncludePropertiesNotSupported), culture);

    public static string UploadBuildArtifactsToBlobNotEnabled() => FrameworkResources.Get(nameof (UploadBuildArtifactsToBlobNotEnabled));

    public static string UploadBuildArtifactsToBlobNotEnabled(CultureInfo culture) => FrameworkResources.Get(nameof (UploadBuildArtifactsToBlobNotEnabled), culture);

    public static string TenantPolicyRestrictFullScopePersonalAccessTokenDescription() => FrameworkResources.Get(nameof (TenantPolicyRestrictFullScopePersonalAccessTokenDescription));

    public static string TenantPolicyRestrictFullScopePersonalAccessTokenDescription(
      CultureInfo culture)
    {
      return FrameworkResources.Get(nameof (TenantPolicyRestrictFullScopePersonalAccessTokenDescription), culture);
    }

    public static string TenantPolicyRestrictGlobalPersonalAccessTokenDescription() => FrameworkResources.Get(nameof (TenantPolicyRestrictGlobalPersonalAccessTokenDescription));

    public static string TenantPolicyRestrictGlobalPersonalAccessTokenDescription(
      CultureInfo culture)
    {
      return FrameworkResources.Get(nameof (TenantPolicyRestrictGlobalPersonalAccessTokenDescription), culture);
    }

    public static string TenantPolicyRestrictPersonalAccessTokenLifespanDescription() => FrameworkResources.Get(nameof (TenantPolicyRestrictPersonalAccessTokenLifespanDescription));

    public static string TenantPolicyRestrictPersonalAccessTokenLifespanDescription(
      CultureInfo culture)
    {
      return FrameworkResources.Get(nameof (TenantPolicyRestrictPersonalAccessTokenLifespanDescription), culture);
    }

    public static string TenantPolicyEnableLeakedPersonalAccessTokenAutoRevocationDescription() => FrameworkResources.Get(nameof (TenantPolicyEnableLeakedPersonalAccessTokenAutoRevocationDescription));

    public static string TenantPolicyEnableLeakedPersonalAccessTokenAutoRevocationDescription(
      CultureInfo culture)
    {
      return FrameworkResources.Get(nameof (TenantPolicyEnableLeakedPersonalAccessTokenAutoRevocationDescription), culture);
    }

    public static string TenantPolicyInvalidMaxPatLifespanInDays() => FrameworkResources.Get(nameof (TenantPolicyInvalidMaxPatLifespanInDays));

    public static string TenantPolicyInvalidMaxPatLifespanInDays(CultureInfo culture) => FrameworkResources.Get(nameof (TenantPolicyInvalidMaxPatLifespanInDays), culture);

    public static string PolicyLogAuditEventsDescription() => FrameworkResources.Get(nameof (PolicyLogAuditEventsDescription));

    public static string PolicyLogAuditEventsDescription(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyLogAuditEventsDescription), culture);

    public static string PolicyLogAuditEventsLink() => FrameworkResources.Get(nameof (PolicyLogAuditEventsLink));

    public static string PolicyLogAuditEventsLink(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyLogAuditEventsLink), culture);

    public static string UnauthorizedErrorWithMessage(object arg0) => FrameworkResources.Format(nameof (UnauthorizedErrorWithMessage), arg0);

    public static string UnauthorizedErrorWithMessage(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (UnauthorizedErrorWithMessage), culture, arg0);

    public static string SmartRouterInternalError(object arg0) => FrameworkResources.Format(nameof (SmartRouterInternalError), arg0);

    public static string SmartRouterInternalError(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (SmartRouterInternalError), culture, arg0);

    public static string ScopePipelineResourcesUse() => FrameworkResources.Get(nameof (ScopePipelineResourcesUse));

    public static string ScopePipelineResourcesUse(CultureInfo culture) => FrameworkResources.Get(nameof (ScopePipelineResourcesUse), culture);

    public static string ScopePipelineResourcesUseManage() => FrameworkResources.Get(nameof (ScopePipelineResourcesUseManage));

    public static string ScopePipelineResourcesUseManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopePipelineResourcesUseManage), culture);

    public static string ScopeGitHubConnectionsRead() => FrameworkResources.Get(nameof (ScopeGitHubConnectionsRead));

    public static string ScopeGitHubConnectionsRead(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeGitHubConnectionsRead), culture);

    public static string ScopeGitHubConnectionsReadManage() => FrameworkResources.Get(nameof (ScopeGitHubConnectionsReadManage));

    public static string ScopeGitHubConnectionsReadManage(CultureInfo culture) => FrameworkResources.Get(nameof (ScopeGitHubConnectionsReadManage), culture);

    public static string RequireAadBackedOrg() => FrameworkResources.Get(nameof (RequireAadBackedOrg));

    public static string RequireAadBackedOrg(CultureInfo culture) => FrameworkResources.Get(nameof (RequireAadBackedOrg), culture);

    public static string VirtualMachineHasMultipleUserAssignedIdentities() => FrameworkResources.Get(nameof (VirtualMachineHasMultipleUserAssignedIdentities));

    public static string VirtualMachineHasMultipleUserAssignedIdentities(CultureInfo culture) => FrameworkResources.Get(nameof (VirtualMachineHasMultipleUserAssignedIdentities), culture);

    public static string ManagedIdentityNotFound(object arg0) => FrameworkResources.Format(nameof (ManagedIdentityNotFound), arg0);

    public static string ManagedIdentityNotFound(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (ManagedIdentityNotFound), culture, arg0);

    public static string GeographyCodeContainsInvalidCharacter(object arg0) => FrameworkResources.Format(nameof (GeographyCodeContainsInvalidCharacter), arg0);

    public static string GeographyCodeContainsInvalidCharacter(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (GeographyCodeContainsInvalidCharacter), culture, arg0);

    public static string GeographyCodeIsNullOrOnlyContainsWhiteSpace(object arg0) => FrameworkResources.Format(nameof (GeographyCodeIsNullOrOnlyContainsWhiteSpace), arg0);

    public static string GeographyCodeIsNullOrOnlyContainsWhiteSpace(
      object arg0,
      CultureInfo culture)
    {
      return FrameworkResources.Format(nameof (GeographyCodeIsNullOrOnlyContainsWhiteSpace), culture, arg0);
    }

    public static string GeographyCodeTooLong(object arg0, object arg1) => FrameworkResources.Format(nameof (GeographyCodeTooLong), arg0, arg1);

    public static string GeographyCodeTooLong(object arg0, object arg1, CultureInfo culture) => FrameworkResources.Format(nameof (GeographyCodeTooLong), culture, arg0, arg1);

    public static string GeographyDoesNotExist(object arg0) => FrameworkResources.Format(nameof (GeographyDoesNotExist), arg0);

    public static string GeographyDoesNotExist(object arg0, CultureInfo culture) => FrameworkResources.Format(nameof (GeographyDoesNotExist), culture, arg0);

    public static string PolicyDisallowBasicAuthentication() => FrameworkResources.Get(nameof (PolicyDisallowBasicAuthentication));

    public static string PolicyDisallowBasicAuthentication(CultureInfo culture) => FrameworkResources.Get(nameof (PolicyDisallowBasicAuthentication), culture);
  }
}
