// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(resourceName, culture);
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

    public static string InvalidEndpointAuthorizer(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidEndpointAuthorizer), arg0);

    public static string InvalidEndpointAuthorizer(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidEndpointAuthorizer), culture, arg0);

    public static string ResourceUrlNotSupported(object arg0, object arg1) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (ResourceUrlNotSupported), arg0, arg1);

    public static string ResourceUrlNotSupported(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (ResourceUrlNotSupported), culture, arg0, arg1);

    public static string InvalidEndpointVariable(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidEndpointVariable), arg0);

    public static string InvalidEndpointVariable(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidEndpointVariable), culture, arg0);

    public static string InvalidSelectorType(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidSelectorType), arg0);

    public static string InvalidSelectorType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidSelectorType), culture, arg0);

    public static string KeyValueCountMismatch() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (KeyValueCountMismatch));

    public static string KeyValueCountMismatch(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (KeyValueCountMismatch), culture);

    public static string SelectorParseError() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (SelectorParseError));

    public static string SelectorParseError(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (SelectorParseError), culture);

    public static string ShouldStartWithEndpointUrlError() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (ShouldStartWithEndpointUrlError));

    public static string ShouldStartWithEndpointUrlError(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (ShouldStartWithEndpointUrlError), culture);

    public static string InvalidBrokerTokenArgument(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidBrokerTokenArgument), arg0);

    public static string InvalidBrokerTokenArgument(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidBrokerTokenArgument), culture, arg0);

    public static string InvalidBodyArgument(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidBodyArgument), arg0);

    public static string InvalidBodyArgument(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidBodyArgument), culture, arg0);

    public static string InvalidResponseSize(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidResponseSize), arg0);

    public static string InvalidResponseSize(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidResponseSize), culture, arg0);

    public static string InvalidContentTypeArgument(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidContentTypeArgument), arg0);

    public static string InvalidContentTypeArgument(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidContentTypeArgument), culture, arg0);

    public static string InvalidMethod(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidMethod), arg0);

    public static string InvalidMethod(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidMethod), culture, arg0);

    public static string ResultTemplateNotSupported() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (ResultTemplateNotSupported));

    public static string ResultTemplateNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (ResultTemplateNotSupported), culture);

    public static string InvaludMaxRepeatCount() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (InvaludMaxRepeatCount));

    public static string InvaludMaxRepeatCount(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (InvaludMaxRepeatCount), culture);

    public static string InvalidFormatSpecifierInRecursiveFormat() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (InvalidFormatSpecifierInRecursiveFormat));

    public static string InvalidFormatSpecifierInRecursiveFormat(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (InvalidFormatSpecifierInRecursiveFormat), culture);

    public static string MaxResponseParsingDepthReached(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (MaxResponseParsingDepthReached), arg0);

    public static string MaxResponseParsingDepthReached(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (MaxResponseParsingDepthReached), culture, arg0);

    public static string MaxStringLengthReached(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (MaxStringLengthReached), arg0);

    public static string MaxStringLengthReached(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (MaxStringLengthReached), culture, arg0);

    public static string MaxTokenLimitReached(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (MaxTokenLimitReached), arg0);

    public static string MaxTokenLimitReached(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (MaxTokenLimitReached), culture, arg0);

    public static string InvalidMustacheExpressionParameter(object arg0, object arg1) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidMustacheExpressionParameter), arg0, arg1);

    public static string InvalidMustacheExpressionParameter(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidMustacheExpressionParameter), culture, arg0, arg1);
    }

    public static string ResponseSizeExceeded() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (ResponseSizeExceeded));

    public static string ResponseSizeExceeded(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (ResponseSizeExceeded), culture);

    public static string InvalidCertificate() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (InvalidCertificate));

    public static string InvalidCertificate(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (InvalidCertificate), culture);

    public static string CannotSortBeyondMaxObjectCount(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (CannotSortBeyondMaxObjectCount), arg0);

    public static string CannotSortBeyondMaxObjectCount(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (CannotSortBeyondMaxObjectCount), culture, arg0);

    public static string NoCertificate() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (NoCertificate));

    public static string NoCertificate(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (NoCertificate), culture);

    public static string RegexMatchTimeExceeded(object arg0, object arg1) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (RegexMatchTimeExceeded), arg0, arg1);

    public static string RegexMatchTimeExceeded(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (RegexMatchTimeExceeded), culture, arg0, arg1);

    public static string InvalidUrl(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidUrl), arg0);

    public static string InvalidUrl(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidUrl), culture, arg0);

    public static string HttpTimeoutException(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (HttpTimeoutException), arg0);

    public static string HttpTimeoutException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (HttpTimeoutException), culture, arg0);

    public static string TemplateEvaluationTimeExceeded(object arg0, object arg1) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (TemplateEvaluationTimeExceeded), arg0, arg1);

    public static string TemplateEvaluationTimeExceeded(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (TemplateEvaluationTimeExceeded), culture, arg0, arg1);
    }

    public static string MaxArrayLimitReached(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (MaxArrayLimitReached), arg0);

    public static string MaxArrayLimitReached(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (MaxArrayLimitReached), culture, arg0);

    public static string ExecuteServiceEndpointFailed() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (ExecuteServiceEndpointFailed));

    public static string ExecuteServiceEndpointFailed(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (ExecuteServiceEndpointFailed), culture);

    public static string UrlCannotBeEmpty() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (UrlCannotBeEmpty));

    public static string UrlCannotBeEmpty(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (UrlCannotBeEmpty), culture);

    public static string AuthTokenCannotBeEmpty() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (AuthTokenCannotBeEmpty));

    public static string AuthTokenCannotBeEmpty(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (AuthTokenCannotBeEmpty), culture);

    public static string UrlIsNotWhiteListed(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (UrlIsNotWhiteListed), arg0);

    public static string UrlIsNotWhiteListed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (UrlIsNotWhiteListed), culture, arg0);

    public static string FileContentResponseSizeExceeded() => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (FileContentResponseSizeExceeded));

    public static string FileContentResponseSizeExceeded(CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Get(nameof (FileContentResponseSizeExceeded), culture);

    public static string InvalidJsonStringArgument(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidJsonStringArgument), arg0);

    public static string InvalidJsonStringArgument(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidJsonStringArgument), culture, arg0);

    public static string InvalidDateTimeFormatException(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidDateTimeFormatException), arg0);

    public static string InvalidDateTimeFormatException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidDateTimeFormatException), culture, arg0);

    public static string InvalidDateTimeStringRepresentationException(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidDateTimeStringRepresentationException), arg0);

    public static string InvalidDateTimeStringRepresentationException(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidDateTimeStringRepresentationException), culture, arg0);
    }

    public static string OutofRangeDateTimeException(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (OutofRangeDateTimeException), arg0);

    public static string OutofRangeDateTimeException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (OutofRangeDateTimeException), culture, arg0);

    public static string InvalidGuidFormatException(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidGuidFormatException), arg0);

    public static string InvalidGuidFormatException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidGuidFormatException), culture, arg0);

    public static string InvalidJsonResponse(object arg0) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidJsonResponse), arg0);

    public static string InvalidJsonResponse(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ServiceEndpoints.Common.Resources.Format(nameof (InvalidJsonResponse), culture, arg0);
  }
}
