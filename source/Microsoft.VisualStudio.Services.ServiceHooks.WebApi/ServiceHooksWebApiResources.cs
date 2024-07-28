// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksWebApiResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  internal static class ServiceHooksWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServiceHooksWebApiResources), typeof (ServiceHooksWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServiceHooksWebApiResources.s_resMgr;

    private static string Get(string resourceName) => ServiceHooksWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServiceHooksWebApiResources.Get(resourceName) : ServiceHooksWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServiceHooksWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServiceHooksWebApiResources.GetInt(resourceName) : (int) ServiceHooksWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServiceHooksWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServiceHooksWebApiResources.GetBool(resourceName) : (bool) ServiceHooksWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServiceHooksWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServiceHooksWebApiResources.Get(resourceName, culture);
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

    public static string Error_ConsumerActionNotFoundByIdFormat(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (Error_ConsumerActionNotFoundByIdFormat), arg0, arg1);

    public static string Error_ConsumerActionNotFoundByIdFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_ConsumerActionNotFoundByIdFormat), culture, arg0, arg1);
    }

    public static string Error_ConsumerNotAvailableByIdFormat(object arg0) => ServiceHooksWebApiResources.Format(nameof (Error_ConsumerNotAvailableByIdFormat), arg0);

    public static string Error_ConsumerNotAvailableByIdFormat(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (Error_ConsumerNotAvailableByIdFormat), culture, arg0);

    public static string Error_ConsumerNotFoundByIdFormat(object arg0) => ServiceHooksWebApiResources.Format(nameof (Error_ConsumerNotFoundByIdFormat), arg0);

    public static string Error_ConsumerNotFoundByIdFormat(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (Error_ConsumerNotFoundByIdFormat), culture, arg0);

    public static string Error_MissingRequiredSubscriptionInputFormat(object arg0) => ServiceHooksWebApiResources.Format(nameof (Error_MissingRequiredSubscriptionInputFormat), arg0);

    public static string Error_MissingRequiredSubscriptionInputFormat(
      object arg0,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_MissingRequiredSubscriptionInputFormat), culture, arg0);
    }

    public static string Error_NoPublishers() => ServiceHooksWebApiResources.Get(nameof (Error_NoPublishers));

    public static string Error_NoPublishers(CultureInfo culture) => ServiceHooksWebApiResources.Get(nameof (Error_NoPublishers), culture);

    public static string Error_NoSubscriptionWithId(object arg0) => ServiceHooksWebApiResources.Format(nameof (Error_NoSubscriptionWithId), arg0);

    public static string Error_NoSubscriptionWithId(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (Error_NoSubscriptionWithId), culture, arg0);

    public static string Error_NotificationNotFoundByIdFormat(object arg0) => ServiceHooksWebApiResources.Format(nameof (Error_NotificationNotFoundByIdFormat), arg0);

    public static string Error_NotificationNotFoundByIdFormat(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (Error_NotificationNotFoundByIdFormat), culture, arg0);

    public static string Error_PublisherEventTypeNotFoundByIdFormat(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (Error_PublisherEventTypeNotFoundByIdFormat), arg0, arg1);

    public static string Error_PublisherEventTypeNotFoundByIdFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_PublisherEventTypeNotFoundByIdFormat), culture, arg0, arg1);
    }

    public static string Error_PublisherIdNotSpecified() => ServiceHooksWebApiResources.Get(nameof (Error_PublisherIdNotSpecified));

    public static string Error_PublisherIdNotSpecified(CultureInfo culture) => ServiceHooksWebApiResources.Get(nameof (Error_PublisherIdNotSpecified), culture);

    public static string Error_PublisherNotFoundByIdFormat(object arg0) => ServiceHooksWebApiResources.Format(nameof (Error_PublisherNotFoundByIdFormat), arg0);

    public static string Error_PublisherNotFoundByIdFormat(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (Error_PublisherNotFoundByIdFormat), culture, arg0);

    public static string Error_SubscriptionInputDataTypeMismatchFormat(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputDataTypeMismatchFormat), arg0, arg1);

    public static string Error_SubscriptionInputDataTypeMismatchFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputDataTypeMismatchFormat), culture, arg0, arg1);
    }

    public static string Error_SubscriptionInputInvalidLengthFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthFormat), arg0, arg1, arg2, arg3);
    }

    public static string Error_SubscriptionInputInvalidLengthFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthFormat), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_SubscriptionInputInvalidLengthTooBigFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthTooBigFormat), arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputInvalidLengthTooBigFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthTooBigFormat), culture, arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputInvalidLengthTooSmallFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthTooSmallFormat), arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputInvalidLengthTooSmallFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputInvalidLengthTooSmallFormat), culture, arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputLoopbackUrl() => ServiceHooksWebApiResources.Get(nameof (Error_SubscriptionInputLoopbackUrl));

    public static string Error_SubscriptionInputLoopbackUrl(CultureInfo culture) => ServiceHooksWebApiResources.Get(nameof (Error_SubscriptionInputLoopbackUrl), culture);

    public static string Error_SubscriptionInputOutOfRangeFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputOutOfRangeFormat), arg0, arg1, arg2, arg3);
    }

    public static string Error_SubscriptionInputOutOfRangeFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputOutOfRangeFormat), culture, arg0, arg1, arg2, arg3);
    }

    public static string Error_SubscriptionInputPatternMismatchFormat(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputPatternMismatchFormat), arg0, arg1);

    public static string Error_SubscriptionInputPatternMismatchFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputPatternMismatchFormat), culture, arg0, arg1);
    }

    public static string Error_SubscriptionInputScopeUnsupportedFormat(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputScopeUnsupportedFormat), arg0, arg1);

    public static string Error_SubscriptionInputScopeUnsupportedFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputScopeUnsupportedFormat), culture, arg0, arg1);
    }

    public static string Error_SubscriptionInputValueTooBigFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputValueTooBigFormat), arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputValueTooBigFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputValueTooBigFormat), culture, arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputValueTooSmallFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputValueTooSmallFormat), arg0, arg1, arg2);
    }

    public static string Error_SubscriptionInputValueTooSmallFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionInputValueTooSmallFormat), culture, arg0, arg1, arg2);
    }

    public static string Error_SubscriptionScopeChangeNotAllowed(object arg0) => ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionScopeChangeNotAllowed), arg0);

    public static string Error_SubscriptionScopeChangeNotAllowed(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (Error_SubscriptionScopeChangeNotAllowed), culture, arg0);

    public static string HttpActionTask_ContentTemplate(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_ContentTemplate), arg0, arg1);

    public static string HttpActionTask_ContentTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (HttpActionTask_ContentTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_HeaderKeyValueTemplate(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServiceHooksWebApiResources.Format(nameof (HttpActionTask_HeaderKeyValueTemplate), arg0, arg1, arg2);
    }

    public static string HttpActionTask_HeaderKeyValueTemplate(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (HttpActionTask_HeaderKeyValueTemplate), culture, arg0, arg1, arg2);
    }

    public static string HttpActionTask_HeadersEndTemplate(object arg0) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_HeadersEndTemplate), arg0);

    public static string HttpActionTask_HeadersEndTemplate(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_HeadersEndTemplate), culture, arg0);

    public static string HttpActionTask_HeadersStartTemplate(object arg0) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_HeadersStartTemplate), arg0);

    public static string HttpActionTask_HeadersStartTemplate(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_HeadersStartTemplate), culture, arg0);

    public static string HttpActionTask_HttpVersionTemplate(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_HttpVersionTemplate), arg0, arg1);

    public static string HttpActionTask_HttpVersionTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (HttpActionTask_HttpVersionTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_MethodTemplate(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_MethodTemplate), arg0, arg1);

    public static string HttpActionTask_MethodTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (HttpActionTask_MethodTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_ReasonPhraseTemplate(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_ReasonPhraseTemplate), arg0, arg1);

    public static string HttpActionTask_ReasonPhraseTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (HttpActionTask_ReasonPhraseTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_StatusCodeTemplate(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_StatusCodeTemplate), arg0, arg1);

    public static string HttpActionTask_StatusCodeTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (HttpActionTask_StatusCodeTemplate), culture, arg0, arg1);
    }

    public static string HttpActionTask_UriTemplate(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_UriTemplate), arg0, arg1);

    public static string HttpActionTask_UriTemplate(object arg0, object arg1, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (HttpActionTask_UriTemplate), culture, arg0, arg1);

    public static string IncorrectParameterTypeExceptionFormat(object arg0, object arg1) => ServiceHooksWebApiResources.Format(nameof (IncorrectParameterTypeExceptionFormat), arg0, arg1);

    public static string IncorrectParameterTypeExceptionFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServiceHooksWebApiResources.Format(nameof (IncorrectParameterTypeExceptionFormat), culture, arg0, arg1);
    }

    public static string Response_Error() => ServiceHooksWebApiResources.Get(nameof (Response_Error));

    public static string Response_Error(CultureInfo culture) => ServiceHooksWebApiResources.Get(nameof (Response_Error), culture);

    public static string Response_ErrorNoResponse(object arg0) => ServiceHooksWebApiResources.Format(nameof (Response_ErrorNoResponse), arg0);

    public static string Response_ErrorNoResponse(object arg0, CultureInfo culture) => ServiceHooksWebApiResources.Format(nameof (Response_ErrorNoResponse), culture, arg0);

    public static string Response_OK() => ServiceHooksWebApiResources.Get(nameof (Response_OK));

    public static string Response_OK(CultureInfo culture) => ServiceHooksWebApiResources.Get(nameof (Response_OK), culture);

    public static string Response_OnPremFirewall() => ServiceHooksWebApiResources.Get(nameof (Response_OnPremFirewall));

    public static string Response_OnPremFirewall(CultureInfo culture) => ServiceHooksWebApiResources.Get(nameof (Response_OnPremFirewall), culture);
  }
}
