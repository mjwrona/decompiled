// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.ClientResources
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Client
{
  internal static class ClientResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (ClientResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ClientResources.s_resMgr;

    private static string Get(string resourceName) => ClientResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ClientResources.Get(resourceName) : ClientResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ClientResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ClientResources.GetInt(resourceName) : (int) ClientResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ClientResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ClientResources.GetBool(resourceName) : (bool) ClientResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ClientResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ClientResources.Get(resourceName, culture);
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

    public static string UnexpectedNavigation() => ClientResources.Get(nameof (UnexpectedNavigation));

    public static string UnexpectedNavigation(CultureInfo culture) => ClientResources.Get(nameof (UnexpectedNavigation), culture);

    public static string UnexpectedNavigationToUrl() => ClientResources.Get(nameof (UnexpectedNavigationToUrl));

    public static string UnexpectedNavigationToUrl(CultureInfo culture) => ClientResources.Get(nameof (UnexpectedNavigationToUrl), culture);

    public static string STAThreadRequired() => ClientResources.Get(nameof (STAThreadRequired));

    public static string STAThreadRequired(CultureInfo culture) => ClientResources.Get(nameof (STAThreadRequired), culture);

    public static string BrowserNavigationFailed() => ClientResources.Get(nameof (BrowserNavigationFailed));

    public static string BrowserNavigationFailed(CultureInfo culture) => ClientResources.Get(nameof (BrowserNavigationFailed), culture);

    public static string BrowserNavigationToUrlFailed() => ClientResources.Get(nameof (BrowserNavigationToUrlFailed));

    public static string BrowserNavigationToUrlFailed(CultureInfo culture) => ClientResources.Get(nameof (BrowserNavigationToUrlFailed), culture);

    public static string None() => ClientResources.Get(nameof (None));

    public static string None(CultureInfo culture) => ClientResources.Get(nameof (None), culture);

    public static string BrowserScriptDisabled() => ClientResources.Get(nameof (BrowserScriptDisabled));

    public static string BrowserScriptDisabled(CultureInfo culture) => ClientResources.Get(nameof (BrowserScriptDisabled), culture);

    public static string ErrorDeserializeFailed() => ClientResources.Get(nameof (ErrorDeserializeFailed));

    public static string ErrorDeserializeFailed(CultureInfo culture) => ClientResources.Get(nameof (ErrorDeserializeFailed), culture);

    public static string SignInCancelled() => ClientResources.Get(nameof (SignInCancelled));

    public static string SignInCancelled(CultureInfo culture) => ClientResources.Get(nameof (SignInCancelled), culture);

    public static string TokenDeserializeFailed() => ClientResources.Get(nameof (TokenDeserializeFailed));

    public static string TokenDeserializeFailed(CultureInfo culture) => ClientResources.Get(nameof (TokenDeserializeFailed), culture);

    public static string ExplicitTrustRequired() => ClientResources.Get(nameof (ExplicitTrustRequired));

    public static string ExplicitTrustRequired(CultureInfo culture) => ClientResources.Get(nameof (ExplicitTrustRequired), culture);

    public static string NavigationBadGateway() => ClientResources.Get(nameof (NavigationBadGateway));

    public static string NavigationBadGateway(CultureInfo culture) => ClientResources.Get(nameof (NavigationBadGateway), culture);

    public static string NavigationBadRequest() => ClientResources.Get(nameof (NavigationBadRequest));

    public static string NavigationBadRequest(CultureInfo culture) => ClientResources.Get(nameof (NavigationBadRequest), culture);

    public static string NavigationForbidden() => ClientResources.Get(nameof (NavigationForbidden));

    public static string NavigationForbidden(CultureInfo culture) => ClientResources.Get(nameof (NavigationForbidden), culture);

    public static string NavigationGatewayTimeout() => ClientResources.Get(nameof (NavigationGatewayTimeout));

    public static string NavigationGatewayTimeout(CultureInfo culture) => ClientResources.Get(nameof (NavigationGatewayTimeout), culture);

    public static string NavigationInternalServerError() => ClientResources.Get(nameof (NavigationInternalServerError));

    public static string NavigationInternalServerError(CultureInfo culture) => ClientResources.Get(nameof (NavigationInternalServerError), culture);

    public static string NavigationNotFound() => ClientResources.Get(nameof (NavigationNotFound));

    public static string NavigationNotFound(CultureInfo culture) => ClientResources.Get(nameof (NavigationNotFound), culture);

    public static string NavigationServiceUnavailable() => ClientResources.Get(nameof (NavigationServiceUnavailable));

    public static string NavigationServiceUnavailable(CultureInfo culture) => ClientResources.Get(nameof (NavigationServiceUnavailable), culture);

    public static string NavigationUnauthorized() => ClientResources.Get(nameof (NavigationUnauthorized));

    public static string NavigationUnauthorized(CultureInfo culture) => ClientResources.Get(nameof (NavigationUnauthorized), culture);

    public static string ServerBadRequest() => ClientResources.Get(nameof (ServerBadRequest));

    public static string ServerBadRequest(CultureInfo culture) => ClientResources.Get(nameof (ServerBadRequest), culture);

    public static string ServerForbidden() => ClientResources.Get(nameof (ServerForbidden));

    public static string ServerForbidden(CultureInfo culture) => ClientResources.Get(nameof (ServerForbidden), culture);

    public static string ServerInternalServerError() => ClientResources.Get(nameof (ServerInternalServerError));

    public static string ServerInternalServerError(CultureInfo culture) => ClientResources.Get(nameof (ServerInternalServerError), culture);

    public static string ServerNotFound() => ClientResources.Get(nameof (ServerNotFound));

    public static string ServerNotFound(CultureInfo culture) => ClientResources.Get(nameof (ServerNotFound), culture);

    public static string ServerServiceUnavailable() => ClientResources.Get(nameof (ServerServiceUnavailable));

    public static string ServerServiceUnavailable(CultureInfo culture) => ClientResources.Get(nameof (ServerServiceUnavailable), culture);

    public static string ServerUnauthorized() => ClientResources.Get(nameof (ServerUnauthorized));

    public static string ServerUnauthorized(CultureInfo culture) => ClientResources.Get(nameof (ServerUnauthorized), culture);

    public static string ServerUnknownError() => ClientResources.Get(nameof (ServerUnknownError));

    public static string ServerUnknownError(CultureInfo culture) => ClientResources.Get(nameof (ServerUnknownError), culture);

    public static string UnknownClientError() => ClientResources.Get(nameof (UnknownClientError));

    public static string UnknownClientError(CultureInfo culture) => ClientResources.Get(nameof (UnknownClientError), culture);

    public static string UnknownError() => ClientResources.Get(nameof (UnknownError));

    public static string UnknownError(CultureInfo culture) => ClientResources.Get(nameof (UnknownError), culture);

    public static string AccountManagerProblemExecutingInGlobalMutex(object arg0) => ClientResources.Format(nameof (AccountManagerProblemExecutingInGlobalMutex), arg0);

    public static string AccountManagerProblemExecutingInGlobalMutex(
      object arg0,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (AccountManagerProblemExecutingInGlobalMutex), culture, arg0);
    }

    public static string RegistryAccountStoreCannotCreateSubKey(object arg0, object arg1) => ClientResources.Format(nameof (RegistryAccountStoreCannotCreateSubKey), arg0, arg1);

    public static string RegistryAccountStoreCannotCreateSubKey(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (RegistryAccountStoreCannotCreateSubKey), culture, arg0, arg1);
    }

    public static string RegistryAccountStoreMemberNotValid(object arg0, object arg1) => ClientResources.Format(nameof (RegistryAccountStoreMemberNotValid), arg0, arg1);

    public static string RegistryAccountStoreMemberNotValid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (RegistryAccountStoreMemberNotValid), culture, arg0, arg1);
    }

    public static string BlueBadgeAccountImage192192() => ClientResources.Get(nameof (BlueBadgeAccountImage192192));

    public static string BlueBadgeAccountImage192192(CultureInfo culture) => ClientResources.Get(nameof (BlueBadgeAccountImage192192), culture);

    public static string VsAccountProviderName() => ClientResources.Get(nameof (VsAccountProviderName));

    public static string VsAccountProviderName(CultureInfo culture) => ClientResources.Get(nameof (VsAccountProviderName), culture);

    public static string MicrosoftLogo_128xLG() => ClientResources.Get(nameof (MicrosoftLogo_128xLG));

    public static string MicrosoftLogo_128xLG(CultureInfo culture) => ClientResources.Get(nameof (MicrosoftLogo_128xLG), culture);

    public static string VsAccountProviderWorkOrSchool() => ClientResources.Get(nameof (VsAccountProviderWorkOrSchool));

    public static string VsAccountProviderWorkOrSchool(CultureInfo culture) => ClientResources.Get(nameof (VsAccountProviderWorkOrSchool), culture);

    public static string VsAccountProviderUserNameCannotBeEmpty() => ClientResources.Get(nameof (VsAccountProviderUserNameCannotBeEmpty));

    public static string VsAccountProviderUserNameCannotBeEmpty(CultureInfo culture) => ClientResources.Get(nameof (VsAccountProviderUserNameCannotBeEmpty), culture);

    public static string VsAccountProviderUnsupportedAccount() => ClientResources.Get(nameof (VsAccountProviderUnsupportedAccount));

    public static string VsAccountProviderUnsupportedAccount(CultureInfo culture) => ClientResources.Get(nameof (VsAccountProviderUnsupportedAccount), culture);

    public static string VsAccountProviderSetCacheInvalidOperation() => ClientResources.Get(nameof (VsAccountProviderSetCacheInvalidOperation));

    public static string VsAccountProviderSetCacheInvalidOperation(CultureInfo culture) => ClientResources.Get(nameof (VsAccountProviderSetCacheInvalidOperation), culture);

    public static string VsAccountProviderAccountNotFoundFromKey() => ClientResources.Get(nameof (VsAccountProviderAccountNotFoundFromKey));

    public static string VsAccountProviderAccountNotFoundFromKey(CultureInfo culture) => ClientResources.Get(nameof (VsAccountProviderAccountNotFoundFromKey), culture);

    public static string UICredProvider_MessageText() => ClientResources.Get(nameof (UICredProvider_MessageText));

    public static string UICredProvider_MessageText(CultureInfo culture) => ClientResources.Get(nameof (UICredProvider_MessageText), culture);

    public static string UICredProvider_TitleText(object arg0) => ClientResources.Format(nameof (UICredProvider_TitleText), arg0);

    public static string UICredProvider_TitleText(object arg0, CultureInfo culture) => ClientResources.Format(nameof (UICredProvider_TitleText), culture, arg0);
  }
}
