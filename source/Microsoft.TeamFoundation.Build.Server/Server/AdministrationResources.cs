// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.AdministrationResources
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class AdministrationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AdministrationResources), typeof (AdministrationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AdministrationResources.s_resMgr;

    private static string Get(string resourceName) => AdministrationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AdministrationResources.Get(resourceName) : AdministrationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AdministrationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AdministrationResources.GetInt(resourceName) : (int) AdministrationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AdministrationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AdministrationResources.GetBool(resourceName) : (bool) AdministrationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AdministrationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AdministrationResources.Get(resourceName, culture);
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

    public static string BuildAgentAlreadyExistsForController(object arg0, object arg1) => AdministrationResources.Format(nameof (BuildAgentAlreadyExistsForController), arg0, arg1);

    public static string BuildAgentAlreadyExistsForController(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentAlreadyExistsForController), culture, arg0, arg1);
    }

    public static string BuildAgentAlreadyExistsForServiceHost(object arg0, object arg1) => AdministrationResources.Format(nameof (BuildAgentAlreadyExistsForServiceHost), arg0, arg1);

    public static string BuildAgentAlreadyExistsForServiceHost(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentAlreadyExistsForServiceHost), culture, arg0, arg1);
    }

    public static string BuildAgentDoesNotExist(object arg0) => AdministrationResources.Format(nameof (BuildAgentDoesNotExist), arg0);

    public static string BuildAgentDoesNotExist(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildAgentDoesNotExist), culture, arg0);

    public static string BuildAgentCannotBeMovedWhileReserved(object arg0, object arg1) => AdministrationResources.Format(nameof (BuildAgentCannotBeMovedWhileReserved), arg0, arg1);

    public static string BuildAgentCannotBeMovedWhileReserved(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentCannotBeMovedWhileReserved), culture, arg0, arg1);
    }

    public static string BuildControllerAlreadyExists(object arg0) => AdministrationResources.Format(nameof (BuildControllerAlreadyExists), arg0);

    public static string BuildControllerAlreadyExists(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildControllerAlreadyExists), culture, arg0);

    public static string BuildControllerAlreadyExistsForServiceHost(object arg0) => AdministrationResources.Format(nameof (BuildControllerAlreadyExistsForServiceHost), arg0);

    public static string BuildControllerAlreadyExistsForServiceHost(
      object arg0,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildControllerAlreadyExistsForServiceHost), culture, arg0);
    }

    public static string BuildControllerCannotBeDeletedAgentsAssociated(object arg0) => AdministrationResources.Format(nameof (BuildControllerCannotBeDeletedAgentsAssociated), arg0);

    public static string BuildControllerCannotBeDeletedAgentsAssociated(
      object arg0,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildControllerCannotBeDeletedAgentsAssociated), culture, arg0);
    }

    public static string BuildControllerCannotBeDeletedBuildsInProgress(object arg0) => AdministrationResources.Format(nameof (BuildControllerCannotBeDeletedBuildsInProgress), arg0);

    public static string BuildControllerCannotBeDeletedBuildsInProgress(
      object arg0,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildControllerCannotBeDeletedBuildsInProgress), culture, arg0);
    }

    public static string BuildControllerDoesNotExist(object arg0) => AdministrationResources.Format(nameof (BuildControllerDoesNotExist), arg0);

    public static string BuildControllerDoesNotExist(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildControllerDoesNotExist), culture, arg0);

    public static string BuildControllerDoesNotExistForComputer(object arg0) => AdministrationResources.Format(nameof (BuildControllerDoesNotExistForComputer), arg0);

    public static string BuildControllerDoesNotExistForComputer(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildControllerDoesNotExistForComputer), culture, arg0);

    public static string BuildServiceHostAlreadyExists(object arg0) => AdministrationResources.Format(nameof (BuildServiceHostAlreadyExists), arg0);

    public static string BuildServiceHostAlreadyExists(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildServiceHostAlreadyExists), culture, arg0);

    public static string BuildServiceHostDoesNotExist(object arg0) => AdministrationResources.Format(nameof (BuildServiceHostDoesNotExist), arg0);

    public static string BuildServiceHostDoesNotExist(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildServiceHostDoesNotExist), culture, arg0);

    public static string BuildControllerReenabled() => AdministrationResources.Get(nameof (BuildControllerReenabled));

    public static string BuildControllerReenabled(CultureInfo culture) => AdministrationResources.Get(nameof (BuildControllerReenabled), culture);

    public static string BuildAgentReenabled() => AdministrationResources.Get(nameof (BuildAgentReenabled));

    public static string BuildAgentReenabled(CultureInfo culture) => AdministrationResources.Get(nameof (BuildAgentReenabled), culture);

    public static string BuildControllerStatusAutomaticallyChanged(object arg0, object arg1) => AdministrationResources.Format(nameof (BuildControllerStatusAutomaticallyChanged), arg0, arg1);

    public static string BuildControllerStatusAutomaticallyChanged(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildControllerStatusAutomaticallyChanged), culture, arg0, arg1);
    }

    public static string BuildAgentStatusAutomaticallyChanged(object arg0, object arg1) => AdministrationResources.Format(nameof (BuildAgentStatusAutomaticallyChanged), arg0, arg1);

    public static string BuildAgentStatusAutomaticallyChanged(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentStatusAutomaticallyChanged), culture, arg0, arg1);
    }

    public static string BuildControllerStatusChangedByUser(object arg0) => AdministrationResources.Format(nameof (BuildControllerStatusChangedByUser), arg0);

    public static string BuildControllerStatusChangedByUser(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildControllerStatusChangedByUser), culture, arg0);

    public static string BuildAgentStatusChangedByUser(object arg0) => AdministrationResources.Format(nameof (BuildAgentStatusChangedByUser), arg0);

    public static string BuildAgentStatusChangedByUser(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildAgentStatusChangedByUser), culture, arg0);

    public static string BuildAgentExpired() => AdministrationResources.Get(nameof (BuildAgentExpired));

    public static string BuildAgentExpired(CultureInfo culture) => AdministrationResources.Get(nameof (BuildAgentExpired), culture);

    public static string BuildControllerExpired() => AdministrationResources.Get(nameof (BuildControllerExpired));

    public static string BuildControllerExpired(CultureInfo culture) => AdministrationResources.Get(nameof (BuildControllerExpired), culture);

    public static string CannotAddDuplicateProcessTemplate(object arg0, object arg1) => AdministrationResources.Format(nameof (CannotAddDuplicateProcessTemplate), arg0, arg1);

    public static string CannotAddDuplicateProcessTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (CannotAddDuplicateProcessTemplate), culture, arg0, arg1);
    }

    public static string CannotCancelQueuedBuild(object arg0, object arg1, object arg2) => AdministrationResources.Format(nameof (CannotCancelQueuedBuild), arg0, arg1, arg2);

    public static string CannotCancelQueuedBuild(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (CannotCancelQueuedBuild), culture, arg0, arg1, arg2);
    }

    public static string CannotDestroyBuildNotDeleted(object arg0) => AdministrationResources.Format(nameof (CannotDestroyBuildNotDeleted), arg0);

    public static string CannotDestroyBuildNotDeleted(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (CannotDestroyBuildNotDeleted), culture, arg0);

    public static string CannotSatisfyQueuePosition(object arg0, object arg1, object arg2) => AdministrationResources.Format(nameof (CannotSatisfyQueuePosition), arg0, arg1, arg2);

    public static string CannotSatisfyQueuePosition(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (CannotSatisfyQueuePosition), culture, arg0, arg1, arg2);
    }

    public static string CannotUpdateQueuedBuild(object arg0, object arg1, object arg2) => AdministrationResources.Format(nameof (CannotUpdateQueuedBuild), arg0, arg1, arg2);

    public static string CannotUpdateQueuedBuild(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (CannotUpdateQueuedBuild), culture, arg0, arg1, arg2);
    }

    public static string BuildAgentReservationInvalidBuildStatus(object arg0) => AdministrationResources.Format(nameof (BuildAgentReservationInvalidBuildStatus), arg0);

    public static string BuildAgentReservationInvalidBuildStatus(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildAgentReservationInvalidBuildStatus), culture, arg0);

    public static string BuildAgentReservationCannotBeSatisfied(object arg0, object arg1) => AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfied), arg0, arg1);

    public static string BuildAgentReservationCannotBeSatisfied(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfied), culture, arg0, arg1);
    }

    public static string BuildAgentReservationCannotBeSatisfiedMatchExactly(
      object arg0,
      object arg1)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfiedMatchExactly), arg0, arg1);
    }

    public static string BuildAgentReservationCannotBeSatisfiedMatchExactly(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfiedMatchExactly), culture, arg0, arg1);
    }

    public static string BuildAgentReservationCannotBeSatisfiedWithTags(
      object arg0,
      object arg1,
      object arg2)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfiedWithTags), arg0, arg1, arg2);
    }

    public static string BuildAgentReservationCannotBeSatisfiedWithTags(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfiedWithTags), culture, arg0, arg1, arg2);
    }

    public static string BuildAgentReservationCannotBeSatisfiedWithTagsMatchExactly(
      object arg0,
      object arg1,
      object arg2)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfiedWithTagsMatchExactly), arg0, arg1, arg2);
    }

    public static string BuildAgentReservationCannotBeSatisfiedWithTagsMatchExactly(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfiedWithTagsMatchExactly), culture, arg0, arg1, arg2);
    }

    public static string BuildAgentReservationCannotBeSatisfiedVersionMismatch(
      object arg0,
      object arg1)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfiedVersionMismatch), arg0, arg1);
    }

    public static string BuildAgentReservationCannotBeSatisfiedVersionMismatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildAgentReservationCannotBeSatisfiedVersionMismatch), culture, arg0, arg1);
    }

    public static string ListSeparatorFormat(object arg0) => AdministrationResources.Format(nameof (ListSeparatorFormat), arg0);

    public static string ListSeparatorFormat(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (ListSeparatorFormat), culture, arg0);

    public static string InvalidBuildAgentTag(object arg0) => AdministrationResources.Format(nameof (InvalidBuildAgentTag), arg0);

    public static string InvalidBuildAgentTag(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (InvalidBuildAgentTag), culture, arg0);

    public static string MultipleDefaultProcessTemplates(object arg0) => AdministrationResources.Format(nameof (MultipleDefaultProcessTemplates), arg0);

    public static string MultipleDefaultProcessTemplates(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (MultipleDefaultProcessTemplates), culture, arg0);

    public static string MultipleUpgradeProcessTemplates(object arg0) => AdministrationResources.Format(nameof (MultipleUpgradeProcessTemplates), arg0);

    public static string MultipleUpgradeProcessTemplates(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (MultipleUpgradeProcessTemplates), culture, arg0);

    public static string ProcessTemplateDoesNotExist(object arg0) => AdministrationResources.Format(nameof (ProcessTemplateDoesNotExist), arg0);

    public static string ProcessTemplateDoesNotExist(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (ProcessTemplateDoesNotExist), culture, arg0);

    public static string QueuedBuildDoesNotExist(object arg0) => AdministrationResources.Format(nameof (QueuedBuildDoesNotExist), arg0);

    public static string QueuedBuildDoesNotExist(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (QueuedBuildDoesNotExist), culture, arg0);

    public static string QueuedBuildDoesNotExistForUri(object arg0) => AdministrationResources.Format(nameof (QueuedBuildDoesNotExistForUri), arg0);

    public static string QueuedBuildDoesNotExistForUri(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (QueuedBuildDoesNotExistForUri), culture, arg0);

    public static string SharedResourceAlreadyRequested(object arg0, object arg1) => AdministrationResources.Format(nameof (SharedResourceAlreadyRequested), arg0, arg1);

    public static string SharedResourceAlreadyRequested(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (SharedResourceAlreadyRequested), culture, arg0, arg1);
    }

    public static string SharedResourceAlreadyAcquired(object arg0, object arg1) => AdministrationResources.Format(nameof (SharedResourceAlreadyAcquired), arg0, arg1);

    public static string SharedResourceAlreadyAcquired(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (SharedResourceAlreadyAcquired), culture, arg0, arg1);
    }

    public static string SharedResourceRequesterCannotBeContacted(object arg0) => AdministrationResources.Format(nameof (SharedResourceRequesterCannotBeContacted), arg0);

    public static string SharedResourceRequesterCannotBeContacted(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (SharedResourceRequesterCannotBeContacted), culture, arg0);

    public static string AdministrativeActionDenied(object arg0, object arg1) => AdministrationResources.Format(nameof (AdministrativeActionDenied), arg0, arg1);

    public static string AdministrativeActionDenied(object arg0, object arg1, CultureInfo culture) => AdministrationResources.Format(nameof (AdministrativeActionDenied), culture, arg0, arg1);

    public static string CannotRetryQueuedBuild(object arg0, object arg1, object arg2) => AdministrationResources.Format(nameof (CannotRetryQueuedBuild), arg0, arg1, arg2);

    public static string CannotRetryQueuedBuild(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (CannotRetryQueuedBuild), culture, arg0, arg1, arg2);
    }

    public static string BuildServiceHostOwnershipException(object arg0) => AdministrationResources.Format(nameof (BuildServiceHostOwnershipException), arg0);

    public static string BuildServiceHostOwnershipException(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (BuildServiceHostOwnershipException), culture, arg0);

    public static string ElasticBuildServiceIdentityName() => AdministrationResources.Get(nameof (ElasticBuildServiceIdentityName));

    public static string ElasticBuildServiceIdentityName(CultureInfo culture) => AdministrationResources.Get(nameof (ElasticBuildServiceIdentityName), culture);

    public static string BuildServiceHostIsVirtualReserved() => AdministrationResources.Get(nameof (BuildServiceHostIsVirtualReserved));

    public static string BuildServiceHostIsVirtualReserved(CultureInfo culture) => AdministrationResources.Get(nameof (BuildServiceHostIsVirtualReserved), culture);

    public static string BuildStoppedMachineConnectionLost() => AdministrationResources.Get(nameof (BuildStoppedMachineConnectionLost));

    public static string BuildStoppedMachineConnectionLost(CultureInfo culture) => AdministrationResources.Get(nameof (BuildStoppedMachineConnectionLost), culture);

    public static string ElasticBuildServiceHostCannotBeDeleted(object arg0) => AdministrationResources.Format(nameof (ElasticBuildServiceHostCannotBeDeleted), arg0);

    public static string ElasticBuildServiceHostCannotBeDeleted(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (ElasticBuildServiceHostCannotBeDeleted), culture, arg0);

    public static string ElasticBuildControllerCannotBeDeleted(object arg0) => AdministrationResources.Format(nameof (ElasticBuildControllerCannotBeDeleted), arg0);

    public static string ElasticBuildControllerCannotBeDeleted(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (ElasticBuildControllerCannotBeDeleted), culture, arg0);

    public static string ElasticBuildAgentCannotBeDeleted(object arg0) => AdministrationResources.Format(nameof (ElasticBuildAgentCannotBeDeleted), arg0);

    public static string ElasticBuildAgentCannotBeDeleted(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (ElasticBuildAgentCannotBeDeleted), culture, arg0);

    public static string ElasticBuildAgentCannotBeAdded(object arg0, object arg1) => AdministrationResources.Format(nameof (ElasticBuildAgentCannotBeAdded), arg0, arg1);

    public static string ElasticBuildAgentCannotBeAdded(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (ElasticBuildAgentCannotBeAdded), culture, arg0, arg1);
    }

    public static string BuildStoppedMachineNonResponsive() => AdministrationResources.Get(nameof (BuildStoppedMachineNonResponsive));

    public static string BuildStoppedMachineNonResponsive(CultureInfo culture) => AdministrationResources.Get(nameof (BuildStoppedMachineNonResponsive), culture);

    public static string BuildStoppedMachineOffline() => AdministrationResources.Get(nameof (BuildStoppedMachineOffline));

    public static string BuildStoppedMachineOffline(CultureInfo culture) => AdministrationResources.Get(nameof (BuildStoppedMachineOffline), culture);

    public static string ElasticBuildAgentCannotBeUpdated(object arg0, object arg1) => AdministrationResources.Format(nameof (ElasticBuildAgentCannotBeUpdated), arg0, arg1);

    public static string ElasticBuildAgentCannotBeUpdated(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (ElasticBuildAgentCannotBeUpdated), culture, arg0, arg1);
    }

    public static string ElasticBuildControllerCannotBeUpdated(object arg0, object arg1) => AdministrationResources.Format(nameof (ElasticBuildControllerCannotBeUpdated), arg0, arg1);

    public static string ElasticBuildControllerCannotBeUpdated(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (ElasticBuildControllerCannotBeUpdated), culture, arg0, arg1);
    }

    public static string ElasticBuildServiceHostCannotBeUpdated(object arg0, object arg1) => AdministrationResources.Format(nameof (ElasticBuildServiceHostCannotBeUpdated), arg0, arg1);

    public static string ElasticBuildServiceHostCannotBeUpdated(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (ElasticBuildServiceHostCannotBeUpdated), culture, arg0, arg1);
    }

    public static string BuildServiceHostOwnershipExceptionWithOwner(object arg0, object arg1) => AdministrationResources.Format(nameof (BuildServiceHostOwnershipExceptionWithOwner), arg0, arg1);

    public static string BuildServiceHostOwnershipExceptionWithOwner(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AdministrationResources.Format(nameof (BuildServiceHostOwnershipExceptionWithOwner), culture, arg0, arg1);
    }

    public static string SharedResourceInvalidBuildStatus(object arg0) => AdministrationResources.Format(nameof (SharedResourceInvalidBuildStatus), arg0);

    public static string SharedResourceInvalidBuildStatus(object arg0, CultureInfo culture) => AdministrationResources.Format(nameof (SharedResourceInvalidBuildStatus), culture, arg0);

    public static string BuildStoppedMachineConnectionLostNoRetry() => AdministrationResources.Get(nameof (BuildStoppedMachineConnectionLostNoRetry));

    public static string BuildStoppedMachineConnectionLostNoRetry(CultureInfo culture) => AdministrationResources.Get(nameof (BuildStoppedMachineConnectionLostNoRetry), culture);

    public static string BuildAgentVersionMismatch(object arg0, object arg1) => AdministrationResources.Format(nameof (BuildAgentVersionMismatch), arg0, arg1);

    public static string BuildAgentVersionMismatch(object arg0, object arg1, CultureInfo culture) => AdministrationResources.Format(nameof (BuildAgentVersionMismatch), culture, arg0, arg1);
  }
}
