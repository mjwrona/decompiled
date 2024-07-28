// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.Resources
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Symbol.WebApi.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Get(resourceName, culture);
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

    public static string ClientNotFoundExceptionMessage(object arg0) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (ClientNotFoundExceptionMessage), arg0);

    public static string ClientNotFoundExceptionMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (ClientNotFoundExceptionMessage), culture, arg0);

    public static string ClientTypeInvalidExceptionMessage(object arg0) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (ClientTypeInvalidExceptionMessage), arg0);

    public static string ClientTypeInvalidExceptionMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (ClientTypeInvalidExceptionMessage), culture, arg0);

    public static string CollectionNotFoundExceptionMessage(object arg0) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (CollectionNotFoundExceptionMessage), arg0);

    public static string CollectionNotFoundExceptionMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (CollectionNotFoundExceptionMessage), culture, arg0);

    public static string DebugEntryExistsExceptionMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryExistsExceptionMessage), arg0, arg1);

    public static string DebugEntryExistsExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryExistsExceptionMessage), culture, arg0, arg1);
    }

    public static string DebugEntryNotChunkedExceptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryNotChunkedExceptionMessage), arg0, arg1, arg2);
    }

    public static string DebugEntryNotChunkedExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryNotChunkedExceptionMessage), culture, arg0, arg1, arg2);
    }

    public static string DebugEntryNotCreatedExceptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryNotCreatedExceptionMessage), arg0, arg1, arg2);
    }

    public static string DebugEntryNotCreatedExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryNotCreatedExceptionMessage), culture, arg0, arg1, arg2);
    }

    public static string DebugEntryNotDeletedExceptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryNotDeletedExceptionMessage), arg0, arg1, arg2);
    }

    public static string DebugEntryNotDeletedExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryNotDeletedExceptionMessage), culture, arg0, arg1, arg2);
    }

    public static string DebugEntryNotFoundExceptionMessage(object arg0) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryNotFoundExceptionMessage), arg0);

    public static string DebugEntryNotFoundExceptionMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (DebugEntryNotFoundExceptionMessage), culture, arg0);

    public static string InvalidEndpointParameterExceptionMessage(object arg0) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (InvalidEndpointParameterExceptionMessage), arg0);

    public static string InvalidEndpointParameterExceptionMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (InvalidEndpointParameterExceptionMessage), culture, arg0);

    public static string NotAuthorizedExceptionMessage() => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Get(nameof (NotAuthorizedExceptionMessage));

    public static string NotAuthorizedExceptionMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Get(nameof (NotAuthorizedExceptionMessage), culture);

    public static string QueryParameterIsNotNumericExceptionMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (QueryParameterIsNotNumericExceptionMessage), arg0, arg1);

    public static string QueryParameterIsNotNumericExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (QueryParameterIsNotNumericExceptionMessage), culture, arg0, arg1);
    }

    public static string QueryParameterMustNotBeNegativeExceptionMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (QueryParameterMustNotBeNegativeExceptionMessage), arg0, arg1);

    public static string QueryParameterMustNotBeNegativeExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (QueryParameterMustNotBeNegativeExceptionMessage), culture, arg0, arg1);
    }

    public static string RequestExistsExceptionMessage(object arg0) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestExistsExceptionMessage), arg0);

    public static string RequestExistsExceptionMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestExistsExceptionMessage), culture, arg0);

    public static string RequestNameRequiredExceptionMessage() => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Get(nameof (RequestNameRequiredExceptionMessage));

    public static string RequestNameRequiredExceptionMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Get(nameof (RequestNameRequiredExceptionMessage), culture);

    public static string RequestNotDeleteExceptionMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestNotDeleteExceptionMessage), arg0, arg1);

    public static string RequestNotDeleteExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestNotDeleteExceptionMessage), culture, arg0, arg1);
    }

    public static string RequestNotFoundExceptionMessage(object arg0) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestNotFoundExceptionMessage), arg0);

    public static string RequestNotFoundExceptionMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestNotFoundExceptionMessage), culture, arg0);

    public static string RequestSealedExceptionMessage(object arg0) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestSealedExceptionMessage), arg0);

    public static string RequestSealedExceptionMessage(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestSealedExceptionMessage), culture, arg0);

    public static string RequestUpdateConflictExceptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestUpdateConflictExceptionMessage), arg0, arg1, arg2);
    }

    public static string RequestUpdateConflictExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestUpdateConflictExceptionMessage), culture, arg0, arg1, arg2);
    }

    public static string RequestUpdateInvalidExceptionMessage(object arg0, object arg1) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestUpdateInvalidExceptionMessage), arg0, arg1);

    public static string RequestUpdateInvalidExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestUpdateInvalidExceptionMessage), culture, arg0, arg1);
    }

    public static string RequestUpdateInvalidStatusExceptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestUpdateInvalidStatusExceptionMessage), arg0, arg1, arg2);
    }

    public static string RequestUpdateInvalidStatusExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Format(nameof (RequestUpdateInvalidStatusExceptionMessage), culture, arg0, arg1, arg2);
    }

    public static string PackageExtensionNotInstalledExceptionMessage() => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Get(nameof (PackageExtensionNotInstalledExceptionMessage));

    public static string PackageExtensionNotInstalledExceptionMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.Symbol.WebApi.Resources.Get(nameof (PackageExtensionNotInstalledExceptionMessage), culture);
  }
}
