// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostingResources
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal static class HostingResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (HostingResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => HostingResources.s_resMgr;

    private static string Get(string resourceName) => HostingResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? HostingResources.Get(resourceName) : HostingResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) HostingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? HostingResources.GetInt(resourceName) : (int) HostingResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) HostingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? HostingResources.GetBool(resourceName) : (bool) HostingResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => HostingResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = HostingResources.Get(resourceName, culture);
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

    public static string DeletionBatchAlreadySucceededError(object arg0) => HostingResources.Format(nameof (DeletionBatchAlreadySucceededError), arg0);

    public static string DeletionBatchAlreadySucceededError(object arg0, CultureInfo culture) => HostingResources.Format(nameof (DeletionBatchAlreadySucceededError), culture, arg0);

    public static string DeletionBatchCanNoLongerBeModifiedError(object arg0) => HostingResources.Format(nameof (DeletionBatchCanNoLongerBeModifiedError), arg0);

    public static string DeletionBatchCanNoLongerBeModifiedError(object arg0, CultureInfo culture) => HostingResources.Format(nameof (DeletionBatchCanNoLongerBeModifiedError), culture, arg0);

    public static string DeletionBatchInvalidTransitionError(object arg0) => HostingResources.Format(nameof (DeletionBatchInvalidTransitionError), arg0);

    public static string DeletionBatchInvalidTransitionError(object arg0, CultureInfo culture) => HostingResources.Format(nameof (DeletionBatchInvalidTransitionError), culture, arg0);

    public static string DeletionBatchJobAlreadyInProgressError(object arg0) => HostingResources.Format(nameof (DeletionBatchJobAlreadyInProgressError), arg0);

    public static string DeletionBatchJobAlreadyInProgressError(object arg0, CultureInfo culture) => HostingResources.Format(nameof (DeletionBatchJobAlreadyInProgressError), culture, arg0);

    public static string DeletionBatchNotFoundError(object arg0) => HostingResources.Format(nameof (DeletionBatchNotFoundError), arg0);

    public static string DeletionBatchNotFoundError(object arg0, CultureInfo culture) => HostingResources.Format(nameof (DeletionBatchNotFoundError), culture, arg0);

    public static string InvalidPath() => HostingResources.Get(nameof (InvalidPath));

    public static string InvalidPath(CultureInfo culture) => HostingResources.Get(nameof (InvalidPath), culture);

    public static string InvalidPathTwoDotError(object arg0) => HostingResources.Format(nameof (InvalidPathTwoDotError), arg0);

    public static string InvalidPathTwoDotError(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidPathTwoDotError), culture, arg0);

    public static string ServiceBusDefaultRuleGroupNotFound(object arg0) => HostingResources.Format(nameof (ServiceBusDefaultRuleGroupNotFound), arg0);

    public static string ServiceBusDefaultRuleGroupNotFound(object arg0, CultureInfo culture) => HostingResources.Format(nameof (ServiceBusDefaultRuleGroupNotFound), culture, arg0);

    public static string ServiceBusManagementCredentialsNotFound() => HostingResources.Get(nameof (ServiceBusManagementCredentialsNotFound));

    public static string ServiceBusManagementCredentialsNotFound(CultureInfo culture) => HostingResources.Get(nameof (ServiceBusManagementCredentialsNotFound), culture);

    public static string EventIdNotSuppliedError() => HostingResources.Get(nameof (EventIdNotSuppliedError));

    public static string EventIdNotSuppliedError(CultureInfo culture) => HostingResources.Get(nameof (EventIdNotSuppliedError), culture);

    public static string EventIdResolutionError(object arg0) => HostingResources.Format(nameof (EventIdResolutionError), arg0);

    public static string EventIdResolutionError(object arg0, CultureInfo culture) => HostingResources.Format(nameof (EventIdResolutionError), culture, arg0);

    public static string InvalidFieldValue(object arg0, object arg1) => HostingResources.Format(nameof (InvalidFieldValue), arg0, arg1);

    public static string InvalidFieldValue(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (InvalidFieldValue), culture, arg0, arg1);

    public static string InvalidQueryParameter(object arg0) => HostingResources.Format(nameof (InvalidQueryParameter), arg0);

    public static string InvalidQueryParameter(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidQueryParameter), culture, arg0);

    public static string InvalidSortColumn(object arg0) => HostingResources.Format(nameof (InvalidSortColumn), arg0);

    public static string InvalidSortColumn(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidSortColumn), culture, arg0);

    public static string InvalidSortColumnExpression(object arg0) => HostingResources.Format(nameof (InvalidSortColumnExpression), arg0);

    public static string InvalidSortColumnExpression(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidSortColumnExpression), culture, arg0);

    public static string DataMigrationAlreadyExistsException(object arg0, object arg1) => HostingResources.Format(nameof (DataMigrationAlreadyExistsException), arg0, arg1);

    public static string DataMigrationAlreadyExistsException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (DataMigrationAlreadyExistsException), culture, arg0, arg1);
    }

    public static string HostMoveRequestAlreadyExistsException(object arg0) => HostingResources.Format(nameof (HostMoveRequestAlreadyExistsException), arg0);

    public static string HostMoveRequestAlreadyExistsException(object arg0, CultureInfo culture) => HostingResources.Format(nameof (HostMoveRequestAlreadyExistsException), culture, arg0);

    public static string InvalidSubscriptionNameEmptyName() => HostingResources.Get(nameof (InvalidSubscriptionNameEmptyName));

    public static string InvalidSubscriptionNameEmptyName(CultureInfo culture) => HostingResources.Get(nameof (InvalidSubscriptionNameEmptyName), culture);

    public static string InvalidSubscriptionNameInvalidCharacters() => HostingResources.Get(nameof (InvalidSubscriptionNameInvalidCharacters));

    public static string InvalidSubscriptionNameInvalidCharacters(CultureInfo culture) => HostingResources.Get(nameof (InvalidSubscriptionNameInvalidCharacters), culture);

    public static string MessageBusSubscriberAlert(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return HostingResources.Format(nameof (MessageBusSubscriberAlert), arg0, arg1, arg2, arg3);
    }

    public static string MessageBusSubscriberAlert(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (MessageBusSubscriberAlert), culture, arg0, arg1, arg2, arg3);
    }

    public static string SubscriptionNameCannotBeGenerated() => HostingResources.Get(nameof (SubscriptionNameCannotBeGenerated));

    public static string SubscriptionNameCannotBeGenerated(CultureInfo culture) => HostingResources.Get(nameof (SubscriptionNameCannotBeGenerated), culture);

    public static string DataMigrationCannotDeleteHostException(object arg0) => HostingResources.Format(nameof (DataMigrationCannotDeleteHostException), arg0);

    public static string DataMigrationCannotDeleteHostException(object arg0, CultureInfo culture) => HostingResources.Format(nameof (DataMigrationCannotDeleteHostException), culture, arg0);

    public static string DataMigrationDisabledException() => HostingResources.Get(nameof (DataMigrationDisabledException));

    public static string DataMigrationDisabledException(CultureInfo culture) => HostingResources.Get(nameof (DataMigrationDisabledException), culture);

    public static string OnlineBlobCopyDisabledException() => HostingResources.Get(nameof (OnlineBlobCopyDisabledException));

    public static string OnlineBlobCopyDisabledException(CultureInfo culture) => HostingResources.Get(nameof (OnlineBlobCopyDisabledException), culture);

    public static string ServiceBusNamespaceNotRegistered(object arg0) => HostingResources.Format(nameof (ServiceBusNamespaceNotRegistered), arg0);

    public static string ServiceBusNamespaceNotRegistered(object arg0, CultureInfo culture) => HostingResources.Format(nameof (ServiceBusNamespaceNotRegistered), culture, arg0);

    public static string ServiceBusPublisherAlreadyExists(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (ServiceBusPublisherAlreadyExists), arg0, arg1, arg2);

    public static string ServiceBusPublisherAlreadyExists(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ServiceBusPublisherAlreadyExists), culture, arg0, arg1, arg2);
    }

    public static string MessageBusSubscriberAlertHeader(object arg0) => HostingResources.Format(nameof (MessageBusSubscriberAlertHeader), arg0);

    public static string MessageBusSubscriberAlertHeader(object arg0, CultureInfo culture) => HostingResources.Format(nameof (MessageBusSubscriberAlertHeader), culture, arg0);

    public static string ATHealthRequestFilterException() => HostingResources.Get(nameof (ATHealthRequestFilterException));

    public static string ATHealthRequestFilterException(CultureInfo culture) => HostingResources.Get(nameof (ATHealthRequestFilterException), culture);

    public static string ATHealthTraceMessage(object arg0, object arg1) => HostingResources.Format(nameof (ATHealthTraceMessage), arg0, arg1);

    public static string ATHealthTraceMessage(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (ATHealthTraceMessage), culture, arg0, arg1);

    public static string DataImportAlreadyExistsException(object arg0) => HostingResources.Format(nameof (DataImportAlreadyExistsException), arg0);

    public static string DataImportAlreadyExistsException(object arg0, CultureInfo culture) => HostingResources.Format(nameof (DataImportAlreadyExistsException), culture, arg0);

    public static string DataImportDoesNotExistException(object arg0) => HostingResources.Format(nameof (DataImportDoesNotExistException), arg0);

    public static string DataImportDoesNotExistException(object arg0, CultureInfo culture) => HostingResources.Format(nameof (DataImportDoesNotExistException), culture, arg0);

    public static string ServiceBusForwardedTopicFullException(object arg0) => HostingResources.Format(nameof (ServiceBusForwardedTopicFullException), arg0);

    public static string ServiceBusForwardedTopicFullException(object arg0, CultureInfo culture) => HostingResources.Format(nameof (ServiceBusForwardedTopicFullException), culture, arg0);

    public static string YouAreBeingThrottledFormat(object arg0, object arg1) => HostingResources.Format(nameof (YouAreBeingThrottledFormat), arg0, arg1);

    public static string YouAreBeingThrottledFormat(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (YouAreBeingThrottledFormat), culture, arg0, arg1);

    public static string YouAreBeingThrottledUsageLinkText() => HostingResources.Get(nameof (YouAreBeingThrottledUsageLinkText));

    public static string YouAreBeingThrottledUsageLinkText(CultureInfo culture) => HostingResources.Get(nameof (YouAreBeingThrottledUsageLinkText), culture);

    public static string YouAreBeingThrottledDocumentationLinkText() => HostingResources.Get(nameof (YouAreBeingThrottledDocumentationLinkText));

    public static string YouAreBeingThrottledDocumentationLinkText(CultureInfo culture) => HostingResources.Get(nameof (YouAreBeingThrottledDocumentationLinkText), culture);

    public static string ThrottleNotificationEmailSubjectLineTarpit(object arg0) => HostingResources.Format(nameof (ThrottleNotificationEmailSubjectLineTarpit), arg0);

    public static string ThrottleNotificationEmailSubjectLineTarpit(
      object arg0,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailSubjectLineTarpit), culture, arg0);
    }

    public static string ThrottleNotificationEmailSubjectLineTarpitForAdmin() => HostingResources.Get(nameof (ThrottleNotificationEmailSubjectLineTarpitForAdmin));

    public static string ThrottleNotificationEmailSubjectLineTarpitForAdmin(CultureInfo culture) => HostingResources.Get(nameof (ThrottleNotificationEmailSubjectLineTarpitForAdmin), culture);

    public static string ThrottleNotificationEmailSubjectLineBlock(object arg0) => HostingResources.Format(nameof (ThrottleNotificationEmailSubjectLineBlock), arg0);

    public static string ThrottleNotificationEmailSubjectLineBlock(object arg0, CultureInfo culture) => HostingResources.Format(nameof (ThrottleNotificationEmailSubjectLineBlock), culture, arg0);

    public static string ThrottleNotificationEmailSubjectLineBlockForAdmin() => HostingResources.Get(nameof (ThrottleNotificationEmailSubjectLineBlockForAdmin));

    public static string ThrottleNotificationEmailSubjectLineBlockForAdmin(CultureInfo culture) => HostingResources.Get(nameof (ThrottleNotificationEmailSubjectLineBlockForAdmin), culture);

    public static string ThrottleNotificationEmailBodyUsagePage() => HostingResources.Get(nameof (ThrottleNotificationEmailBodyUsagePage));

    public static string ThrottleNotificationEmailBodyUsagePage(CultureInfo culture) => HostingResources.Get(nameof (ThrottleNotificationEmailBodyUsagePage), culture);

    public static string ThrottleNotificationEmailBodyUsagePageForAdmin(object arg0) => HostingResources.Format(nameof (ThrottleNotificationEmailBodyUsagePageForAdmin), arg0);

    public static string ThrottleNotificationEmailBodyUsagePageForAdmin(
      object arg0,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyUsagePageForAdmin), culture, arg0);
    }

    public static string ThrottleNotificationEmailBodyStatusBlock() => HostingResources.Get(nameof (ThrottleNotificationEmailBodyStatusBlock));

    public static string ThrottleNotificationEmailBodyStatusBlock(CultureInfo culture) => HostingResources.Get(nameof (ThrottleNotificationEmailBodyStatusBlock), culture);

    public static string ThrottleNotificationEmailBodyStatusBlockForAdmin(object arg0) => HostingResources.Format(nameof (ThrottleNotificationEmailBodyStatusBlockForAdmin), arg0);

    public static string ThrottleNotificationEmailBodyStatusBlockForAdmin(
      object arg0,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyStatusBlockForAdmin), culture, arg0);
    }

    public static string ThrottleNotificationEmailBodyStatusTarpit() => HostingResources.Get(nameof (ThrottleNotificationEmailBodyStatusTarpit));

    public static string ThrottleNotificationEmailBodyStatusTarpit(CultureInfo culture) => HostingResources.Get(nameof (ThrottleNotificationEmailBodyStatusTarpit), culture);

    public static string ThrottleNotificationEmailBodyStatusTarpitForAdmin(object arg0) => HostingResources.Format(nameof (ThrottleNotificationEmailBodyStatusTarpitForAdmin), arg0);

    public static string ThrottleNotificationEmailBodyStatusTarpitForAdmin(
      object arg0,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyStatusTarpitForAdmin), culture, arg0);
    }

    public static string ThrottleNotificationEmailBodyDetailsTarpit(
      object arg0,
      object arg1,
      object arg2)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsTarpit), arg0, arg1, arg2);
    }

    public static string ThrottleNotificationEmailBodyDetailsTarpit(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsTarpit), culture, arg0, arg1, arg2);
    }

    public static string ThrottleNotificationEmailBodyDetailsTarpitForAdmin(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsTarpitForAdmin), arg0, arg1, arg2, arg3);
    }

    public static string ThrottleNotificationEmailBodyDetailsTarpitForAdmin(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsTarpitForAdmin), culture, arg0, arg1, arg2, arg3);
    }

    public static string ThrottleNotificationEmailBodyDetailsBlockedConsumption(
      object arg0,
      object arg1,
      object arg2)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsBlockedConsumption), arg0, arg1, arg2);
    }

    public static string ThrottleNotificationEmailBodyDetailsBlockedConsumption(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsBlockedConsumption), culture, arg0, arg1, arg2);
    }

    public static string ThrottleNotificationEmailBodyDetailsBlockedConsumptionForAdmin(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsBlockedConsumptionForAdmin), arg0, arg1, arg2, arg3);
    }

    public static string ThrottleNotificationEmailBodyDetailsBlockedConsumptionForAdmin(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsBlockedConsumptionForAdmin), culture, arg0, arg1, arg2, arg3);
    }

    public static string ThrottleNotificationEmailBodyDetailsBlockedConcurrent(
      object arg0,
      object arg1,
      object arg2)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsBlockedConcurrent), arg0, arg1, arg2);
    }

    public static string ThrottleNotificationEmailBodyDetailsBlockedConcurrent(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsBlockedConcurrent), culture, arg0, arg1, arg2);
    }

    public static string ThrottleNotificationEmailBodyDetailsBlockedConcurrentForAdmin(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsBlockedConcurrentForAdmin), arg0, arg1, arg2, arg3);
    }

    public static string ThrottleNotificationEmailBodyDetailsBlockedConcurrentForAdmin(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (ThrottleNotificationEmailBodyDetailsBlockedConcurrentForAdmin), culture, arg0, arg1, arg2, arg3);
    }

    public static string InvalidAadUserState(object arg0) => HostingResources.Format(nameof (InvalidAadUserState), arg0);

    public static string InvalidAadUserState(object arg0, CultureInfo culture) => HostingResources.Format(nameof (InvalidAadUserState), culture, arg0);

    public static string KustoConfigurationExceptionMessage() => HostingResources.Get(nameof (KustoConfigurationExceptionMessage));

    public static string KustoConfigurationExceptionMessage(CultureInfo culture) => HostingResources.Get(nameof (KustoConfigurationExceptionMessage), culture);

    public static string KustoQueryFailedExceptionMessage() => HostingResources.Get(nameof (KustoQueryFailedExceptionMessage));

    public static string KustoQueryFailedExceptionMessage(CultureInfo culture) => HostingResources.Get(nameof (KustoQueryFailedExceptionMessage), culture);

    public static string EmptyDefaultValues() => HostingResources.Get(nameof (EmptyDefaultValues));

    public static string EmptyDefaultValues(CultureInfo culture) => HostingResources.Get(nameof (EmptyDefaultValues), culture);

    public static string EmptyPolicyNames() => HostingResources.Get(nameof (EmptyPolicyNames));

    public static string EmptyPolicyNames(CultureInfo culture) => HostingResources.Get(nameof (EmptyPolicyNames), culture);

    public static string PolicyNamesAndDeafultValuesCountMismatch(object arg0, object arg1) => HostingResources.Format(nameof (PolicyNamesAndDeafultValuesCountMismatch), arg0, arg1);

    public static string PolicyNamesAndDeafultValuesCountMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (PolicyNamesAndDeafultValuesCountMismatch), culture, arg0, arg1);
    }

    public static string DataImportBlobsCopiedExceededThreshold(object arg0, object arg1) => HostingResources.Format(nameof (DataImportBlobsCopiedExceededThreshold), arg0, arg1);

    public static string DataImportBlobsCopiedExceededThreshold(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (DataImportBlobsCopiedExceededThreshold), culture, arg0, arg1);
    }

    public static string DuplicateResourceExtensionNamespaceIds(object arg0, object arg1) => HostingResources.Format(nameof (DuplicateResourceExtensionNamespaceIds), arg0, arg1);

    public static string DuplicateResourceExtensionNamespaceIds(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (DuplicateResourceExtensionNamespaceIds), culture, arg0, arg1);
    }

    public static string ResourceExtensionDoesNotExist(object arg0) => HostingResources.Format(nameof (ResourceExtensionDoesNotExist), arg0);

    public static string ResourceExtensionDoesNotExist(object arg0, CultureInfo culture) => HostingResources.Format(nameof (ResourceExtensionDoesNotExist), culture, arg0);

    public static string DCRegionBM() => HostingResources.Get(nameof (DCRegionBM));

    public static string DCRegionBM(CultureInfo culture) => HostingResources.Get(nameof (DCRegionBM), culture);

    public static string DCRegionCC() => HostingResources.Get(nameof (DCRegionCC));

    public static string DCRegionCC(CultureInfo culture) => HostingResources.Get(nameof (DCRegionCC), culture);

    public static string DCRegionCE() => HostingResources.Get(nameof (DCRegionCE));

    public static string DCRegionCE(CultureInfo culture) => HostingResources.Get(nameof (DCRegionCE), culture);

    public static string DCRegionCUS() => HostingResources.Get(nameof (DCRegionCUS));

    public static string DCRegionCUS(CultureInfo culture) => HostingResources.Get(nameof (DCRegionCUS), culture);

    public static string DCRegionEA() => HostingResources.Get(nameof (DCRegionEA));

    public static string DCRegionEA(CultureInfo culture) => HostingResources.Get(nameof (DCRegionEA), culture);

    public static string DCRegionEAU() => HostingResources.Get(nameof (DCRegionEAU));

    public static string DCRegionEAU(CultureInfo culture) => HostingResources.Get(nameof (DCRegionEAU), culture);

    public static string DCRegionEJP() => HostingResources.Get(nameof (DCRegionEJP));

    public static string DCRegionEJP(CultureInfo culture) => HostingResources.Get(nameof (DCRegionEJP), culture);

    public static string DCRegionEUS() => HostingResources.Get(nameof (DCRegionEUS));

    public static string DCRegionEUS(CultureInfo culture) => HostingResources.Get(nameof (DCRegionEUS), culture);

    public static string DCRegionEUS2() => HostingResources.Get(nameof (DCRegionEUS2));

    public static string DCRegionEUS2(CultureInfo culture) => HostingResources.Get(nameof (DCRegionEUS2), culture);

    public static string DCRegionMA() => HostingResources.Get(nameof (DCRegionMA));

    public static string DCRegionMA(CultureInfo culture) => HostingResources.Get(nameof (DCRegionMA), culture);

    public static string DCRegionNCUS() => HostingResources.Get(nameof (DCRegionNCUS));

    public static string DCRegionNCUS(CultureInfo culture) => HostingResources.Get(nameof (DCRegionNCUS), culture);

    public static string DCRegionNEU() => HostingResources.Get(nameof (DCRegionNEU));

    public static string DCRegionNEU(CultureInfo culture) => HostingResources.Get(nameof (DCRegionNEU), culture);

    public static string DCRegionPN() => HostingResources.Get(nameof (DCRegionPN));

    public static string DCRegionPN(CultureInfo culture) => HostingResources.Get(nameof (DCRegionPN), culture);

    public static string DCRegionSBR() => HostingResources.Get(nameof (DCRegionSBR));

    public static string DCRegionSBR(CultureInfo culture) => HostingResources.Get(nameof (DCRegionSBR), culture);

    public static string DCRegionSCUS() => HostingResources.Get(nameof (DCRegionSCUS));

    public static string DCRegionSCUS(CultureInfo culture) => HostingResources.Get(nameof (DCRegionSCUS), culture);

    public static string DCRegionSEA() => HostingResources.Get(nameof (DCRegionSEA));

    public static string DCRegionSEA(CultureInfo culture) => HostingResources.Get(nameof (DCRegionSEA), culture);

    public static string DCRegionSEAU() => HostingResources.Get(nameof (DCRegionSEAU));

    public static string DCRegionSEAU(CultureInfo culture) => HostingResources.Get(nameof (DCRegionSEAU), culture);

    public static string DCRegionUKS() => HostingResources.Get(nameof (DCRegionUKS));

    public static string DCRegionUKS(CultureInfo culture) => HostingResources.Get(nameof (DCRegionUKS), culture);

    public static string DCRegionUKW() => HostingResources.Get(nameof (DCRegionUKW));

    public static string DCRegionUKW(CultureInfo culture) => HostingResources.Get(nameof (DCRegionUKW), culture);

    public static string DCRegionWCUS() => HostingResources.Get(nameof (DCRegionWCUS));

    public static string DCRegionWCUS(CultureInfo culture) => HostingResources.Get(nameof (DCRegionWCUS), culture);

    public static string DCRegionWEU() => HostingResources.Get(nameof (DCRegionWEU));

    public static string DCRegionWEU(CultureInfo culture) => HostingResources.Get(nameof (DCRegionWEU), culture);

    public static string DCRegionWJP() => HostingResources.Get(nameof (DCRegionWJP));

    public static string DCRegionWJP(CultureInfo culture) => HostingResources.Get(nameof (DCRegionWJP), culture);

    public static string DCRegionWUS() => HostingResources.Get(nameof (DCRegionWUS));

    public static string DCRegionWUS(CultureInfo culture) => HostingResources.Get(nameof (DCRegionWUS), culture);

    public static string DCRegionWUS2() => HostingResources.Get(nameof (DCRegionWUS2));

    public static string DCRegionWUS2(CultureInfo culture) => HostingResources.Get(nameof (DCRegionWUS2), culture);

    public static string InvalidDataImportConnectionString(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (InvalidDataImportConnectionString), arg0, arg1, arg2);

    public static string InvalidDataImportConnectionString(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (InvalidDataImportConnectionString), culture, arg0, arg1, arg2);
    }

    public static string TimeOutDuringDataImportConnection(object arg0, object arg1) => HostingResources.Format(nameof (TimeOutDuringDataImportConnection), arg0, arg1);

    public static string TimeOutDuringDataImportConnection(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (TimeOutDuringDataImportConnection), culture, arg0, arg1);
    }

    public static string AadGroupTooBigToTraverseDescendants(object arg0, object arg1) => HostingResources.Format(nameof (AadGroupTooBigToTraverseDescendants), arg0, arg1);

    public static string AadGroupTooBigToTraverseDescendants(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (AadGroupTooBigToTraverseDescendants), culture, arg0, arg1);
    }

    public static string NoRegistryValue(object arg0) => HostingResources.Format(nameof (NoRegistryValue), arg0);

    public static string NoRegistryValue(object arg0, CultureInfo culture) => HostingResources.Format(nameof (NoRegistryValue), culture, arg0);

    public static string TenantAccessDeniedError(object arg0, object arg1) => HostingResources.Format(nameof (TenantAccessDeniedError), arg0, arg1);

    public static string TenantAccessDeniedError(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (TenantAccessDeniedError), culture, arg0, arg1);

    public static string AadGuestInvitationFailed() => HostingResources.Get(nameof (AadGuestInvitationFailed));

    public static string AadGuestInvitationFailed(CultureInfo culture) => HostingResources.Get(nameof (AadGuestInvitationFailed), culture);

    public static string HostMigrateRequestAlreadyExistsException(
      object arg0,
      object arg1,
      object arg2)
    {
      return HostingResources.Format(nameof (HostMigrateRequestAlreadyExistsException), arg0, arg1, arg2);
    }

    public static string HostMigrateRequestAlreadyExistsException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (HostMigrateRequestAlreadyExistsException), culture, arg0, arg1, arg2);
    }

    public static string InvalidSASTokenRequest_InvalidUriFormat() => HostingResources.Get(nameof (InvalidSASTokenRequest_InvalidUriFormat));

    public static string InvalidSASTokenRequest_InvalidUriFormat(CultureInfo culture) => HostingResources.Get(nameof (InvalidSASTokenRequest_InvalidUriFormat), culture);

    public static string InvalidSASTokenRequest_InvalidExpiration() => HostingResources.Get(nameof (InvalidSASTokenRequest_InvalidExpiration));

    public static string InvalidSASTokenRequest_InvalidExpiration(CultureInfo culture) => HostingResources.Get(nameof (InvalidSASTokenRequest_InvalidExpiration), culture);

    public static string AadGloballyBeenThrottled(object arg0, object arg1) => HostingResources.Format(nameof (AadGloballyBeenThrottled), arg0, arg1);

    public static string AadGloballyBeenThrottled(object arg0, object arg1, CultureInfo culture) => HostingResources.Format(nameof (AadGloballyBeenThrottled), culture, arg0, arg1);

    public static string AadSpecialTenantBeenThrottled(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (AadSpecialTenantBeenThrottled), arg0, arg1, arg2);

    public static string AadSpecialTenantBeenThrottled(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (AadSpecialTenantBeenThrottled), culture, arg0, arg1, arg2);
    }

    public static string ConcurrentResource() => HostingResources.Get(nameof (ConcurrentResource));

    public static string ConcurrentResource(CultureInfo culture) => HostingResources.Get(nameof (ConcurrentResource), culture);

    public static string ConcurrencyTimeout() => HostingResources.Get(nameof (ConcurrencyTimeout));

    public static string ConcurrencyTimeout(CultureInfo culture) => HostingResources.Get(nameof (ConcurrencyTimeout), culture);

    public static string ShortResourceWindow() => HostingResources.Get(nameof (ShortResourceWindow));

    public static string ShortResourceWindow(CultureInfo culture) => HostingResources.Get(nameof (ShortResourceWindow), culture);

    public static string LongResourceWindow() => HostingResources.Get(nameof (LongResourceWindow));

    public static string LongResourceWindow(CultureInfo culture) => HostingResources.Get(nameof (LongResourceWindow), culture);

    public static string YouAreBeingThrottledDocumentationLink() => HostingResources.Get(nameof (YouAreBeingThrottledDocumentationLink));

    public static string YouAreBeingThrottledDocumentationLink(CultureInfo culture) => HostingResources.Get(nameof (YouAreBeingThrottledDocumentationLink), culture);

    public static string AnonymousRequestRejectedNoUserAgent() => HostingResources.Get(nameof (AnonymousRequestRejectedNoUserAgent));

    public static string AnonymousRequestRejectedNoUserAgent(CultureInfo culture) => HostingResources.Get(nameof (AnonymousRequestRejectedNoUserAgent), culture);

    public static string YouAreBeingThrottledAnonymousFormat(object arg0, object arg1) => HostingResources.Format(nameof (YouAreBeingThrottledAnonymousFormat), arg0, arg1);

    public static string YouAreBeingThrottledAnonymousFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (YouAreBeingThrottledAnonymousFormat), culture, arg0, arg1);
    }

    public static string YouAreBeingThrottledLearnMoreText() => HostingResources.Get(nameof (YouAreBeingThrottledLearnMoreText));

    public static string YouAreBeingThrottledLearnMoreText(CultureInfo culture) => HostingResources.Get(nameof (YouAreBeingThrottledLearnMoreText), culture);

    public static string YouAreBeingThrottledPublicUserFormat(object arg0) => HostingResources.Format(nameof (YouAreBeingThrottledPublicUserFormat), arg0);

    public static string YouAreBeingThrottledPublicUserFormat(object arg0, CultureInfo culture) => HostingResources.Format(nameof (YouAreBeingThrottledPublicUserFormat), culture, arg0);

    public static string RequestBlockedAnonymous(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (RequestBlockedAnonymous), arg0, arg1, arg2);

    public static string RequestBlockedAnonymous(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (RequestBlockedAnonymous), culture, arg0, arg1, arg2);
    }

    public static string RequestBlockedAnonymousHtml(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (RequestBlockedAnonymousHtml), arg0, arg1, arg2);

    public static string RequestBlockedAnonymousHtml(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (RequestBlockedAnonymousHtml), culture, arg0, arg1, arg2);
    }

    public static string RequestBlockedPublicUser(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (RequestBlockedPublicUser), arg0, arg1, arg2);

    public static string RequestBlockedPublicUser(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (RequestBlockedPublicUser), culture, arg0, arg1, arg2);
    }

    public static string RequestBlockedPublicUserHtml(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (RequestBlockedPublicUserHtml), arg0, arg1, arg2);

    public static string RequestBlockedPublicUserHtml(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (RequestBlockedPublicUserHtml), culture, arg0, arg1, arg2);
    }

    public static string ContentViolationServiceNotEnabled() => HostingResources.Get(nameof (ContentViolationServiceNotEnabled));

    public static string ContentViolationServiceNotEnabled(CultureInfo culture) => HostingResources.Get(nameof (ContentViolationServiceNotEnabled), culture);

    public static string DocDbInvalidId() => HostingResources.Get(nameof (DocDbInvalidId));

    public static string DocDbInvalidId(CultureInfo culture) => HostingResources.Get(nameof (DocDbInvalidId), culture);

    public static string KustoQueryExecutionTimeoutExceptionMessage() => HostingResources.Get(nameof (KustoQueryExecutionTimeoutExceptionMessage));

    public static string KustoQueryExecutionTimeoutExceptionMessage(CultureInfo culture) => HostingResources.Get(nameof (KustoQueryExecutionTimeoutExceptionMessage), culture);

    public static string DocDBSecretsServiceRequestContextHostMessage(object arg0, object arg1) => HostingResources.Format(nameof (DocDBSecretsServiceRequestContextHostMessage), arg0, arg1);

    public static string DocDBSecretsServiceRequestContextHostMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (DocDBSecretsServiceRequestContextHostMessage), culture, arg0, arg1);
    }

    public static string GitHubDirectoryGetUserAvatarFailed(object arg0, object arg1) => HostingResources.Format(nameof (GitHubDirectoryGetUserAvatarFailed), arg0, arg1);

    public static string GitHubDirectoryGetUserAvatarFailed(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (GitHubDirectoryGetUserAvatarFailed), culture, arg0, arg1);
    }

    public static string GitHubDirectoryGetUserFailed(object arg0, object arg1, object arg2) => HostingResources.Format(nameof (GitHubDirectoryGetUserFailed), arg0, arg1, arg2);

    public static string GitHubDirectoryGetUserFailed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (GitHubDirectoryGetUserFailed), culture, arg0, arg1, arg2);
    }

    public static string GitHubDirectorySearchUsersFailed(object arg0, object arg1) => HostingResources.Format(nameof (GitHubDirectorySearchUsersFailed), arg0, arg1);

    public static string GitHubDirectorySearchUsersFailed(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (GitHubDirectorySearchUsersFailed), culture, arg0, arg1);
    }

    public static string GitHubUserAuthenticationRetrievalFailed(object arg0) => HostingResources.Format(nameof (GitHubUserAuthenticationRetrievalFailed), arg0);

    public static string GitHubUserAuthenticationRetrievalFailed(object arg0, CultureInfo culture) => HostingResources.Format(nameof (GitHubUserAuthenticationRetrievalFailed), culture, arg0);

    public static string DataMigrationTargetBlobNotMatchSourceBlobError(object arg0, object arg1) => HostingResources.Format(nameof (DataMigrationTargetBlobNotMatchSourceBlobError), arg0, arg1);

    public static string DataMigrationTargetBlobNotMatchSourceBlobError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (DataMigrationTargetBlobNotMatchSourceBlobError), culture, arg0, arg1);
    }

    public static string DataMigrationTargetExtraBlobError(object arg0, object arg1) => HostingResources.Format(nameof (DataMigrationTargetExtraBlobError), arg0, arg1);

    public static string DataMigrationTargetExtraBlobError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return HostingResources.Format(nameof (DataMigrationTargetExtraBlobError), culture, arg0, arg1);
    }

    public static string GitHubRequestIsForbidden(object arg0) => HostingResources.Format(nameof (GitHubRequestIsForbidden), arg0);

    public static string GitHubRequestIsForbidden(object arg0, CultureInfo culture) => HostingResources.Format(nameof (GitHubRequestIsForbidden), culture, arg0);

    public static string TenantAdminRolesNotFound(object arg0) => HostingResources.Format(nameof (TenantAdminRolesNotFound), arg0);

    public static string TenantAdminRolesNotFound(object arg0, CultureInfo culture) => HostingResources.Format(nameof (TenantAdminRolesNotFound), culture, arg0);

    public static string FailureRetrieveDirectoryRoles() => HostingResources.Get(nameof (FailureRetrieveDirectoryRoles));

    public static string FailureRetrieveDirectoryRoles(CultureInfo culture) => HostingResources.Get(nameof (FailureRetrieveDirectoryRoles), culture);

    public static string FailureRetrieveRoleMembers(object arg0) => HostingResources.Format(nameof (FailureRetrieveRoleMembers), arg0);

    public static string FailureRetrieveRoleMembers(object arg0, CultureInfo culture) => HostingResources.Format(nameof (FailureRetrieveRoleMembers), culture, arg0);

    public static string DCGeographyAP() => HostingResources.Get(nameof (DCGeographyAP));

    public static string DCGeographyAP(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyAP), culture);

    public static string DCGeographyAU() => HostingResources.Get(nameof (DCGeographyAU));

    public static string DCGeographyAU(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyAU), culture);

    public static string DCGeographyBR() => HostingResources.Get(nameof (DCGeographyBR));

    public static string DCGeographyBR(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyBR), culture);

    public static string DCGeographyCA() => HostingResources.Get(nameof (DCGeographyCA));

    public static string DCGeographyCA(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyCA), culture);

    public static string DCGeographyEU() => HostingResources.Get(nameof (DCGeographyEU));

    public static string DCGeographyEU(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyEU), culture);

    public static string DCGeographyFR() => HostingResources.Get(nameof (DCGeographyFR));

    public static string DCGeographyFR(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyFR), culture);

    public static string DCGeographyIN() => HostingResources.Get(nameof (DCGeographyIN));

    public static string DCGeographyIN(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyIN), culture);

    public static string DCGeographyUK() => HostingResources.Get(nameof (DCGeographyUK));

    public static string DCGeographyUK(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyUK), culture);

    public static string DCGeographyUS() => HostingResources.Get(nameof (DCGeographyUS));

    public static string DCGeographyUS(CultureInfo culture) => HostingResources.Get(nameof (DCGeographyUS), culture);
  }
}
