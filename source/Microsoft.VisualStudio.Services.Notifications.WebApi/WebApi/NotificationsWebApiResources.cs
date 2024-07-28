// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationsWebApiResources
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  internal static class NotificationsWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (NotificationsWebApiResources), typeof (NotificationsWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => NotificationsWebApiResources.s_resMgr;

    private static string Get(string resourceName) => NotificationsWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? NotificationsWebApiResources.Get(resourceName) : NotificationsWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) NotificationsWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? NotificationsWebApiResources.GetInt(resourceName) : (int) NotificationsWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) NotificationsWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? NotificationsWebApiResources.GetBool(resourceName) : (bool) NotificationsWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => NotificationsWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = NotificationsWebApiResources.Get(resourceName, culture);
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

    public static string Error_NoSubscriptionWithId(object arg0) => NotificationsWebApiResources.Format(nameof (Error_NoSubscriptionWithId), arg0);

    public static string Error_NoSubscriptionWithId(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (Error_NoSubscriptionWithId), culture, arg0);

    public static string Error_NotificationNotFoundByIdFormat(object arg0) => NotificationsWebApiResources.Format(nameof (Error_NotificationNotFoundByIdFormat), arg0);

    public static string Error_NotificationNotFoundByIdFormat(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (Error_NotificationNotFoundByIdFormat), culture, arg0);

    public static string Error_PublisherEventTypeNotFoundByIdFormat(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (Error_PublisherEventTypeNotFoundByIdFormat), arg0, arg1);

    public static string Error_PublisherEventTypeNotFoundByIdFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_PublisherEventTypeNotFoundByIdFormat), culture, arg0, arg1);
    }

    public static string Error_PublisherIdNotSpecified() => NotificationsWebApiResources.Get(nameof (Error_PublisherIdNotSpecified));

    public static string Error_PublisherIdNotSpecified(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (Error_PublisherIdNotSpecified), culture);

    public static string Error_PublisherNotFoundByIdFormat(object arg0) => NotificationsWebApiResources.Format(nameof (Error_PublisherNotFoundByIdFormat), arg0);

    public static string Error_PublisherNotFoundByIdFormat(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (Error_PublisherNotFoundByIdFormat), culture, arg0);

    public static string Error_SubscriptionInputDataTypeMismatchFormat(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputDataTypeMismatchFormat), arg0, arg1);

    public static string Error_SubscriptionInputDataTypeMismatchFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputDataTypeMismatchFormat), culture, arg0, arg1);
    }

    public static string Error_SubscriptionInputInvalidLengthFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthFormat), arg0, arg1, arg2, arg3);
    }

    public static string Error_SubscriptionInputInvalidLengthFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthFormat), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_SubscriptionInputInvalidLengthTooBigFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthTooBigFormat), arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputInvalidLengthTooBigFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthTooBigFormat), culture, arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputInvalidLengthTooSmallFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthTooSmallFormat), arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputInvalidLengthTooSmallFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthTooSmallFormat), culture, arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputOutOfRangeFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputOutOfRangeFormat), arg0, arg1, arg2, arg3);
    }

    public static string Error_SubscriptionInputOutOfRangeFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputOutOfRangeFormat), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_SubscriptionInputPatternMismatchFormat(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputPatternMismatchFormat), arg0, arg1);

    public static string Error_SubscriptionInputPatternMismatchFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputPatternMismatchFormat), culture, arg0, arg1);
    }

    public static string Error_SubscriptionInputValueTooBigFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputValueTooBigFormat), arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputValueTooBigFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputValueTooBigFormat), culture, arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputValueTooSmallFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputValueTooSmallFormat), arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputValueTooSmallFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (Error_SubscriptionInputValueTooSmallFormat), culture, arg0, arg1, arg2);
    }

    public static string HttpActionTask_ContentTemplate(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (HttpActionTask_ContentTemplate), arg0, arg1);

    public static string HttpActionTask_ContentTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (HttpActionTask_ContentTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_HeaderKeyValueTemplate(
      object arg0,
      object arg1,
      object arg2)
    {
      return NotificationsWebApiResources.Format(nameof (HttpActionTask_HeaderKeyValueTemplate), arg0, arg1, arg2);
    }

    public static string HttpActionTask_HeaderKeyValueTemplate(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (HttpActionTask_HeaderKeyValueTemplate), culture, arg0, arg1, arg2);
    }

    public static string HttpActionTask_HeadersEndTemplate(object arg0) => NotificationsWebApiResources.Format(nameof (HttpActionTask_HeadersEndTemplate), arg0);

    public static string HttpActionTask_HeadersEndTemplate(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (HttpActionTask_HeadersEndTemplate), culture, arg0);

    public static string HttpActionTask_HeadersStartTemplate(object arg0) => NotificationsWebApiResources.Format(nameof (HttpActionTask_HeadersStartTemplate), arg0);

    public static string HttpActionTask_HeadersStartTemplate(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (HttpActionTask_HeadersStartTemplate), culture, arg0);

    public static string HttpActionTask_HttpVersionTemplate(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (HttpActionTask_HttpVersionTemplate), arg0, arg1);

    public static string HttpActionTask_HttpVersionTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (HttpActionTask_HttpVersionTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_MethodTemplate(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (HttpActionTask_MethodTemplate), arg0, arg1);

    public static string HttpActionTask_MethodTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (HttpActionTask_MethodTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_ReasonPhraseTemplate(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (HttpActionTask_ReasonPhraseTemplate), arg0, arg1);

    public static string HttpActionTask_ReasonPhraseTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (HttpActionTask_ReasonPhraseTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_StatusCodeTemplate(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (HttpActionTask_StatusCodeTemplate), arg0, arg1);

    public static string HttpActionTask_StatusCodeTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (HttpActionTask_StatusCodeTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_UriTemplate(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (HttpActionTask_UriTemplate), arg0, arg1);

    public static string HttpActionTask_UriTemplate(object arg0, object arg1, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (HttpActionTask_UriTemplate), culture, arg0, arg1);

    public static string IncorrectParameterTypeExceptionFormat(object arg0, object arg1) => NotificationsWebApiResources.Format(nameof (IncorrectParameterTypeExceptionFormat), arg0, arg1);

    public static string IncorrectParameterTypeExceptionFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return NotificationsWebApiResources.Format(nameof (IncorrectParameterTypeExceptionFormat), culture, arg0, arg1);
    }

    public static string OperatorChanges() => NotificationsWebApiResources.Get(nameof (OperatorChanges));

    public static string OperatorChanges(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorChanges), culture);

    public static string OperatorChangesFrom() => NotificationsWebApiResources.Get(nameof (OperatorChangesFrom));

    public static string OperatorChangesFrom(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorChangesFrom), culture);

    public static string OperatorChangesTo() => NotificationsWebApiResources.Get(nameof (OperatorChangesTo));

    public static string OperatorChangesTo(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorChangesTo), culture);

    public static string OperatorDynamic() => NotificationsWebApiResources.Get(nameof (OperatorDynamic));

    public static string OperatorDynamic(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorDynamic), culture);

    public static string OperatorEqualTo() => NotificationsWebApiResources.Get(nameof (OperatorEqualTo));

    public static string OperatorEqualTo(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorEqualTo), culture);

    public static string OperatorGT() => NotificationsWebApiResources.Get(nameof (OperatorGT));

    public static string OperatorGT(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorGT), culture);

    public static string OperatorGTE() => NotificationsWebApiResources.Get(nameof (OperatorGTE));

    public static string OperatorGTE(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorGTE), culture);

    public static string OperatorLike() => NotificationsWebApiResources.Get(nameof (OperatorLike));

    public static string OperatorLike(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorLike), culture);

    public static string OperatorLT() => NotificationsWebApiResources.Get(nameof (OperatorLT));

    public static string OperatorLT(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorLT), culture);

    public static string OperatorLTE() => NotificationsWebApiResources.Get(nameof (OperatorLTE));

    public static string OperatorLTE(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorLTE), culture);

    public static string OperatorMatch() => NotificationsWebApiResources.Get(nameof (OperatorMatch));

    public static string OperatorMatch(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorMatch), culture);

    public static string OperatorNotContains() => NotificationsWebApiResources.Get(nameof (OperatorNotContains));

    public static string OperatorNotContains(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorNotContains), culture);

    public static string OperatorNotEqualTo() => NotificationsWebApiResources.Get(nameof (OperatorNotEqualTo));

    public static string OperatorNotEqualTo(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorNotEqualTo), culture);

    public static string OperatorNotUnder() => NotificationsWebApiResources.Get(nameof (OperatorNotUnder));

    public static string OperatorNotUnder(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorNotUnder), culture);

    public static string OperatorUnder() => NotificationsWebApiResources.Get(nameof (OperatorUnder));

    public static string OperatorUnder(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorUnder), culture);

    public static string Response_Error() => NotificationsWebApiResources.Get(nameof (Response_Error));

    public static string Response_Error(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (Response_Error), culture);

    public static string Response_OK() => NotificationsWebApiResources.Get(nameof (Response_OK));

    public static string Response_OK(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (Response_OK), culture);

    public static string LogicalOperatorAnd() => NotificationsWebApiResources.Get(nameof (LogicalOperatorAnd));

    public static string LogicalOperatorAnd(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (LogicalOperatorAnd), culture);

    public static string LogicalOperatorOr() => NotificationsWebApiResources.Get(nameof (LogicalOperatorOr));

    public static string LogicalOperatorOr(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (LogicalOperatorOr), culture);

    public static string Error_CannotSearchWithCriteria() => NotificationsWebApiResources.Get(nameof (Error_CannotSearchWithCriteria));

    public static string Error_CannotSearchWithCriteria(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (Error_CannotSearchWithCriteria), culture);

    public static string Error_MustSpecifyArtifactUri() => NotificationsWebApiResources.Get(nameof (Error_MustSpecifyArtifactUri));

    public static string Error_MustSpecifyArtifactUri(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (Error_MustSpecifyArtifactUri), culture);

    public static string Error_MustSpecifyConditions() => NotificationsWebApiResources.Get(nameof (Error_MustSpecifyConditions));

    public static string Error_MustSpecifyConditions(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (Error_MustSpecifyConditions), culture);

    public static string UnsupportedChannel(object arg0) => NotificationsWebApiResources.Format(nameof (UnsupportedChannel), arg0);

    public static string UnsupportedChannel(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (UnsupportedChannel), culture, arg0);

    public static string Error_SubscriptionFilterInvalid(object arg0) => NotificationsWebApiResources.Format(nameof (Error_SubscriptionFilterInvalid), arg0);

    public static string Error_SubscriptionFilterInvalid(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (Error_SubscriptionFilterInvalid), culture, arg0);

    public static string InvalidEventTypeForCustomSubscriptions(object arg0) => NotificationsWebApiResources.Format(nameof (InvalidEventTypeForCustomSubscriptions), arg0);

    public static string InvalidEventTypeForCustomSubscriptions(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (InvalidEventTypeForCustomSubscriptions), culture, arg0);

    public static string InvalidFilterTypeUpdate() => NotificationsWebApiResources.Get(nameof (InvalidFilterTypeUpdate));

    public static string InvalidFilterTypeUpdate(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (InvalidFilterTypeUpdate), culture);

    public static string OperatorUnderPath() => NotificationsWebApiResources.Get(nameof (OperatorUnderPath));

    public static string OperatorUnderPath(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorUnderPath), culture);

    public static string InvalidScopeForCustomSubscriptions(object arg0) => NotificationsWebApiResources.Format(nameof (InvalidScopeForCustomSubscriptions), arg0);

    public static string InvalidScopeForCustomSubscriptions(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (InvalidScopeForCustomSubscriptions), culture, arg0);

    public static string OperatorUnknown(object arg0) => NotificationsWebApiResources.Format(nameof (OperatorUnknown), arg0);

    public static string OperatorUnknown(object arg0, CultureInfo culture) => NotificationsWebApiResources.Format(nameof (OperatorUnknown), culture, arg0);

    public static string OperatorContainsValue() => NotificationsWebApiResources.Get(nameof (OperatorContainsValue));

    public static string OperatorContainsValue(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorContainsValue), culture);

    public static string OperatorDoesNotContainValue() => NotificationsWebApiResources.Get(nameof (OperatorDoesNotContainValue));

    public static string OperatorDoesNotContainValue(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorDoesNotContainValue), culture);

    public static string OperatorMemberOf() => NotificationsWebApiResources.Get(nameof (OperatorMemberOf));

    public static string OperatorMemberOf(CultureInfo culture) => NotificationsWebApiResources.Get(nameof (OperatorMemberOf), culture);
  }
}
