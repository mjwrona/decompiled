// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ResourceStrings
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal static class ResourceStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ResourceStrings), typeof (ResourceStrings).GetTypeInfo().Assembly);
    public const string ErrorBisMiddleTierNotRegistered = "ErrorBisMiddleTierNotRegistered";
    public const string InvalidRequestId = "InvalidRequestId";
    public const string StreamDoesNotSupportLength = "StreamDoesNotSupportLength";
    public const string HTTPStatusCode = "HTTPStatusCode";
    public const string HTTPStatusCodeAndDescription = "HTTPStatusCodeAndDescription";
    public const string ParameterNotNullOrEmpty = "ParameterNotNullOrEmpty";
    public const string UnexpectedColumnType = "UnexpectedColumnType";
    public const string WriteXmlNotImplemented = "WriteXmlNotImplemented";
    public const string BadQuery = "BadQuery";

    public static ResourceManager Manager => ResourceStrings.s_resMgr;

    public static string Get(string resourceName) => ResourceStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.Get(resourceName) : ResourceStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetInt(resourceName) : (int) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetBool(resourceName) : (bool) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => ResourceStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ResourceStrings.Get(resourceName, culture);
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
  }
}
