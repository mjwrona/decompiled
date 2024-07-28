// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.WebFrameworkResources
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public static class WebFrameworkResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WebFrameworkResources), typeof (WebFrameworkResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WebFrameworkResources.s_resMgr;

    private static string Get(string resourceName) => WebFrameworkResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WebFrameworkResources.Get(resourceName) : WebFrameworkResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WebFrameworkResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WebFrameworkResources.GetInt(resourceName) : (int) WebFrameworkResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WebFrameworkResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WebFrameworkResources.GetBool(resourceName) : (bool) WebFrameworkResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WebFrameworkResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WebFrameworkResources.Get(resourceName, culture);
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

    public static string ContentViolationServiceNotEnabled() => WebFrameworkResources.Get(nameof (ContentViolationServiceNotEnabled));

    public static string ContentViolationServiceNotEnabled(CultureInfo culture) => WebFrameworkResources.Get(nameof (ContentViolationServiceNotEnabled), culture);

    public static string InvalidTemplateTypeFormat(object arg0, object arg1, object arg2) => WebFrameworkResources.Format(nameof (InvalidTemplateTypeFormat), arg0, arg1, arg2);

    public static string InvalidTemplateTypeFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WebFrameworkResources.Format(nameof (InvalidTemplateTypeFormat), culture, arg0, arg1, arg2);
    }

    public static string NoScriptHeader() => WebFrameworkResources.Get(nameof (NoScriptHeader));

    public static string NoScriptHeader(CultureInfo culture) => WebFrameworkResources.Get(nameof (NoScriptHeader), culture);

    public static string NoScriptMessage() => WebFrameworkResources.Get(nameof (NoScriptMessage));

    public static string NoScriptMessage(CultureInfo culture) => WebFrameworkResources.Get(nameof (NoScriptMessage), culture);

    public static string PageNotFound() => WebFrameworkResources.Get(nameof (PageNotFound));

    public static string PageNotFound(CultureInfo culture) => WebFrameworkResources.Get(nameof (PageNotFound), culture);

    public static string RouteValueFilterMissingProperty(object arg0) => WebFrameworkResources.Format(nameof (RouteValueFilterMissingProperty), arg0);

    public static string RouteValueFilterMissingProperty(object arg0, CultureInfo culture) => WebFrameworkResources.Format(nameof (RouteValueFilterMissingProperty), culture, arg0);

    public static string TemplateAssetInvalidFormat(object arg0, object arg1) => WebFrameworkResources.Format(nameof (TemplateAssetInvalidFormat), arg0, arg1);

    public static string TemplateAssetInvalidFormat(object arg0, object arg1, CultureInfo culture) => WebFrameworkResources.Format(nameof (TemplateAssetInvalidFormat), culture, arg0, arg1);

    public static string NotSupportedBrowser() => WebFrameworkResources.Get(nameof (NotSupportedBrowser));

    public static string NotSupportedBrowser(CultureInfo culture) => WebFrameworkResources.Get(nameof (NotSupportedBrowser), culture);

    public static string UnauthorizedMessage(object arg0) => WebFrameworkResources.Format(nameof (UnauthorizedMessage), arg0);

    public static string UnauthorizedMessage(object arg0, CultureInfo culture) => WebFrameworkResources.Format(nameof (UnauthorizedMessage), culture, arg0);

    public static string AuthorizationException_Title() => WebFrameworkResources.Get(nameof (AuthorizationException_Title));

    public static string AuthorizationException_Title(CultureInfo culture) => WebFrameworkResources.Get(nameof (AuthorizationException_Title), culture);

    public static string ForbiddenException_Title() => WebFrameworkResources.Get(nameof (ForbiddenException_Title));

    public static string ForbiddenException_Title(CultureInfo culture) => WebFrameworkResources.Get(nameof (ForbiddenException_Title), culture);

    public static string AuthorizationException_NoEmail() => WebFrameworkResources.Get(nameof (AuthorizationException_NoEmail));

    public static string AuthorizationException_NoEmail(CultureInfo culture) => WebFrameworkResources.Get(nameof (AuthorizationException_NoEmail), culture);

    public static string AuthorizationException_SecondaryActionText() => WebFrameworkResources.Get(nameof (AuthorizationException_SecondaryActionText));

    public static string AuthorizationException_SecondaryActionText(CultureInfo culture) => WebFrameworkResources.Get(nameof (AuthorizationException_SecondaryActionText), culture);

    public static string TooManyRequestsException_Title() => WebFrameworkResources.Get(nameof (TooManyRequestsException_Title));

    public static string TooManyRequestsException_Title(CultureInfo culture) => WebFrameworkResources.Get(nameof (TooManyRequestsException_Title), culture);

    public static string TooManyRequestsExceptionWithUsage_Desc() => WebFrameworkResources.Get(nameof (TooManyRequestsExceptionWithUsage_Desc));

    public static string TooManyRequestsExceptionWithUsage_Desc(CultureInfo culture) => WebFrameworkResources.Get(nameof (TooManyRequestsExceptionWithUsage_Desc), culture);

    public static string TooManyRequestsException_Desc() => WebFrameworkResources.Get(nameof (TooManyRequestsException_Desc));

    public static string TooManyRequestsException_Desc(CultureInfo culture) => WebFrameworkResources.Get(nameof (TooManyRequestsException_Desc), culture);

    public static string ContactSupport_ActionText() => WebFrameworkResources.Get(nameof (ContactSupport_ActionText));

    public static string ContactSupport_ActionText(CultureInfo culture) => WebFrameworkResources.Get(nameof (ContactSupport_ActionText), culture);

    public static string ViewMyUsage_ActionText() => WebFrameworkResources.Get(nameof (ViewMyUsage_ActionText));

    public static string ViewMyUsage_ActionText(CultureInfo culture) => WebFrameworkResources.Get(nameof (ViewMyUsage_ActionText), culture);

    public static string ViewStatus_ActionText() => WebFrameworkResources.Get(nameof (ViewStatus_ActionText));

    public static string ViewStatus_ActionText(CultureInfo culture) => WebFrameworkResources.Get(nameof (ViewStatus_ActionText), culture);

    public static string BadReqeust_Title() => WebFrameworkResources.Get(nameof (BadReqeust_Title));

    public static string BadReqeust_Title(CultureInfo culture) => WebFrameworkResources.Get(nameof (BadReqeust_Title), culture);

    public static string InternalException_Title() => WebFrameworkResources.Get(nameof (InternalException_Title));

    public static string InternalException_Title(CultureInfo culture) => WebFrameworkResources.Get(nameof (InternalException_Title), culture);

    public static string GenericError_Title() => WebFrameworkResources.Get(nameof (GenericError_Title));

    public static string GenericError_Title(CultureInfo culture) => WebFrameworkResources.Get(nameof (GenericError_Title), culture);

    public static string GenericError_Desc() => WebFrameworkResources.Get(nameof (GenericError_Desc));

    public static string GenericError_Desc(CultureInfo culture) => WebFrameworkResources.Get(nameof (GenericError_Desc), culture);

    public static string ErrorName_ServiceUnavailableTitle() => WebFrameworkResources.Get(nameof (ErrorName_ServiceUnavailableTitle));

    public static string ErrorName_ServiceUnavailableTitle(CultureInfo culture) => WebFrameworkResources.Get(nameof (ErrorName_ServiceUnavailableTitle), culture);

    public static string ErrorName_ServiceUnavailableMessage(object arg0) => WebFrameworkResources.Format(nameof (ErrorName_ServiceUnavailableMessage), arg0);

    public static string ErrorName_ServiceUnavailableMessage(object arg0, CultureInfo culture) => WebFrameworkResources.Format(nameof (ErrorName_ServiceUnavailableMessage), culture, arg0);

    public static string NotFoundException_Title() => WebFrameworkResources.Get(nameof (NotFoundException_Title));

    public static string NotFoundException_Title(CultureInfo culture) => WebFrameworkResources.Get(nameof (NotFoundException_Title), culture);

    public static string NotFoundException_Message() => WebFrameworkResources.Get(nameof (NotFoundException_Message));

    public static string NotFoundException_Message(CultureInfo culture) => WebFrameworkResources.Get(nameof (NotFoundException_Message), culture);

    public static string GoBackHome_ActionText() => WebFrameworkResources.Get(nameof (GoBackHome_ActionText));

    public static string GoBackHome_ActionText(CultureInfo culture) => WebFrameworkResources.Get(nameof (GoBackHome_ActionText), culture);

    public static string ReparentingTitle() => WebFrameworkResources.Get(nameof (ReparentingTitle));

    public static string ReparentingTitle(CultureInfo culture) => WebFrameworkResources.Get(nameof (ReparentingTitle), culture);

    public static string ReparentingMessage() => WebFrameworkResources.Get(nameof (ReparentingMessage));

    public static string ReparentingMessage(CultureInfo culture) => WebFrameworkResources.Get(nameof (ReparentingMessage), culture);

    public static string DataImportTitle() => WebFrameworkResources.Get(nameof (DataImportTitle));

    public static string DataImportTitle(CultureInfo culture) => WebFrameworkResources.Get(nameof (DataImportTitle), culture);

    public static string DataImportMessage() => WebFrameworkResources.Get(nameof (DataImportMessage));

    public static string DataImportMessage(CultureInfo culture) => WebFrameworkResources.Get(nameof (DataImportMessage), culture);
  }
}
