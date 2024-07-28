// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.EmailNotificationResources
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  public static class EmailNotificationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (EmailNotificationResources), typeof (EmailNotificationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => EmailNotificationResources.s_resMgr;

    private static string Get(string resourceName) => EmailNotificationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? EmailNotificationResources.Get(resourceName) : EmailNotificationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) EmailNotificationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? EmailNotificationResources.GetInt(resourceName) : (int) EmailNotificationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) EmailNotificationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? EmailNotificationResources.GetBool(resourceName) : (bool) EmailNotificationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => EmailNotificationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = EmailNotificationResources.Get(resourceName, culture);
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

    public static string InvalidTemplate() => EmailNotificationResources.Get(nameof (InvalidTemplate));

    public static string InvalidTemplate(CultureInfo culture) => EmailNotificationResources.Get(nameof (InvalidTemplate), culture);

    public static string InvalidToken() => EmailNotificationResources.Get(nameof (InvalidToken));

    public static string InvalidToken(CultureInfo culture) => EmailNotificationResources.Get(nameof (InvalidToken), culture);

    public static string MarketingFooterTemplate() => EmailNotificationResources.Get(nameof (MarketingFooterTemplate));

    public static string MarketingFooterTemplate(CultureInfo culture) => EmailNotificationResources.Get(nameof (MarketingFooterTemplate), culture);

    public static string MarketingHeaderTemplate() => EmailNotificationResources.Get(nameof (MarketingHeaderTemplate));

    public static string MarketingHeaderTemplate(CultureInfo culture) => EmailNotificationResources.Get(nameof (MarketingHeaderTemplate), culture);

    public static string NotAbleToLoadRegistrySetting(object arg0, object arg1) => EmailNotificationResources.Format(nameof (NotAbleToLoadRegistrySetting), arg0, arg1);

    public static string NotAbleToLoadRegistrySetting(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return EmailNotificationResources.Format(nameof (NotAbleToLoadRegistrySetting), culture, arg0, arg1);
    }

    public static string NoValueFoundForToken(object arg0) => EmailNotificationResources.Format(nameof (NoValueFoundForToken), arg0);

    public static string NoValueFoundForToken(object arg0, CultureInfo culture) => EmailNotificationResources.Format(nameof (NoValueFoundForToken), culture, arg0);

    public static string ResourceManagerCannotBeNull() => EmailNotificationResources.Get(nameof (ResourceManagerCannotBeNull));

    public static string ResourceManagerCannotBeNull(CultureInfo culture) => EmailNotificationResources.Get(nameof (ResourceManagerCannotBeNull), culture);

    public static string ServiceNotificationFooterTemplate() => EmailNotificationResources.Get(nameof (ServiceNotificationFooterTemplate));

    public static string ServiceNotificationFooterTemplate(CultureInfo culture) => EmailNotificationResources.Get(nameof (ServiceNotificationFooterTemplate), culture);

    public static string ServiceNotificationHeaderTemplate() => EmailNotificationResources.Get(nameof (ServiceNotificationHeaderTemplate));

    public static string ServiceNotificationHeaderTemplate(CultureInfo culture) => EmailNotificationResources.Get(nameof (ServiceNotificationHeaderTemplate), culture);

    public static string TemplateNotFound(object arg0) => EmailNotificationResources.Format(nameof (TemplateNotFound), arg0);

    public static string TemplateNotFound(object arg0, CultureInfo culture) => EmailNotificationResources.Format(nameof (TemplateNotFound), culture, arg0);

    public static string TokenAlreadyExists(object arg0) => EmailNotificationResources.Format(nameof (TokenAlreadyExists), arg0);

    public static string TokenAlreadyExists(object arg0, CultureInfo culture) => EmailNotificationResources.Format(nameof (TokenAlreadyExists), culture, arg0);

    public static string EmailBodyCannotBeNullOrWhitespace() => EmailNotificationResources.Get(nameof (EmailBodyCannotBeNullOrWhitespace));

    public static string EmailBodyCannotBeNullOrWhitespace(CultureInfo culture) => EmailNotificationResources.Get(nameof (EmailBodyCannotBeNullOrWhitespace), culture);

    public static string TokenDictionaryValues(object arg0, object arg1) => EmailNotificationResources.Format(nameof (TokenDictionaryValues), arg0, arg1);

    public static string TokenDictionaryValues(object arg0, object arg1, CultureInfo culture) => EmailNotificationResources.Format(nameof (TokenDictionaryValues), culture, arg0, arg1);
  }
}
