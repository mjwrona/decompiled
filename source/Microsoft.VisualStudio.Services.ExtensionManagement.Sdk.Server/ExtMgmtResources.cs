// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtMgmtResources
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public static class ExtMgmtResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("ExtensionFrameworkResources", typeof (ExtMgmtResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ExtMgmtResources.s_resMgr;

    private static string Get(string resourceName) => ExtMgmtResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ExtMgmtResources.Get(resourceName) : ExtMgmtResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ExtMgmtResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ExtMgmtResources.GetInt(resourceName) : (int) ExtMgmtResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ExtMgmtResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ExtMgmtResources.GetBool(resourceName) : (bool) ExtMgmtResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ExtMgmtResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ExtMgmtResources.Get(resourceName, culture);
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

    public static string InvalidExtensionName(object arg0) => ExtMgmtResources.Format(nameof (InvalidExtensionName), arg0);

    public static string InvalidExtensionName(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (InvalidExtensionName), culture, arg0);

    public static string InvalidFullyQualifiedExtensionName(object arg0) => ExtMgmtResources.Format(nameof (InvalidFullyQualifiedExtensionName), arg0);

    public static string InvalidFullyQualifiedExtensionName(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (InvalidFullyQualifiedExtensionName), culture, arg0);

    public static string DocumentAlreadyExistsException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExtMgmtResources.Format(nameof (DocumentAlreadyExistsException), arg0, arg1, arg2, arg3);
    }

    public static string DocumentAlreadyExistsException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (DocumentAlreadyExistsException), culture, arg0, arg1, arg2, arg3);
    }

    public static string DocumentDoesNotExistException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExtMgmtResources.Format(nameof (DocumentDoesNotExistException), arg0, arg1, arg2, arg3);
    }

    public static string DocumentDoesNotExistException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (DocumentDoesNotExistException), culture, arg0, arg1, arg2, arg3);
    }

    public static string ExtensionAlreadyInstalledException(object arg0) => ExtMgmtResources.Format(nameof (ExtensionAlreadyInstalledException), arg0);

    public static string ExtensionAlreadyInstalledException(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (ExtensionAlreadyInstalledException), culture, arg0);

    public static string ExtensionUnauthorizedException(object arg0, object arg1) => ExtMgmtResources.Format(nameof (ExtensionUnauthorizedException), arg0, arg1);

    public static string ExtensionUnauthorizedException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (ExtensionUnauthorizedException), culture, arg0, arg1);
    }

    public static string ExtensionRequestAlreadyExistsException() => ExtMgmtResources.Get(nameof (ExtensionRequestAlreadyExistsException));

    public static string ExtensionRequestAlreadyExistsException(CultureInfo culture) => ExtMgmtResources.Get(nameof (ExtensionRequestAlreadyExistsException), culture);

    public static string InstalledAppNotFoundForAppIdException(object arg0) => ExtMgmtResources.Format(nameof (InstalledAppNotFoundForAppIdException), arg0);

    public static string InstalledAppNotFoundForAppIdException(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (InstalledAppNotFoundForAppIdException), culture, arg0);

    public static string InvalidDocumentVersionException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExtMgmtResources.Format(nameof (InvalidDocumentVersionException), arg0, arg1, arg2, arg3);
    }

    public static string InvalidDocumentVersionException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (InvalidDocumentVersionException), culture, arg0, arg1, arg2, arg3);
    }

    public static string InvalidInstallationTargetException(object arg0, object arg1) => ExtMgmtResources.Format(nameof (InvalidInstallationTargetException), arg0, arg1);

    public static string InvalidInstallationTargetException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (InvalidInstallationTargetException), culture, arg0, arg1);
    }

    public static string InvalidPublisherName(object arg0) => ExtMgmtResources.Format(nameof (InvalidPublisherName), arg0);

    public static string InvalidPublisherName(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (InvalidPublisherName), culture, arg0);

    public static string TokenMissingHostAuthorizationClaimException() => ExtMgmtResources.Get(nameof (TokenMissingHostAuthorizationClaimException));

    public static string TokenMissingHostAuthorizationClaimException(CultureInfo culture) => ExtMgmtResources.Get(nameof (TokenMissingHostAuthorizationClaimException), culture);

    public static string MustacheTemplateInvalidContext() => ExtMgmtResources.Get(nameof (MustacheTemplateInvalidContext));

    public static string MustacheTemplateInvalidContext(CultureInfo culture) => ExtMgmtResources.Get(nameof (MustacheTemplateInvalidContext), culture);

    public static string DemandDefaultErrorFormat(object arg0) => ExtMgmtResources.Format(nameof (DemandDefaultErrorFormat), arg0);

    public static string DemandDefaultErrorFormat(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (DemandDefaultErrorFormat), culture, arg0);

    public static string DemandInvalidDemandResolverErrorFormat(object arg0, object arg1) => ExtMgmtResources.Format(nameof (DemandInvalidDemandResolverErrorFormat), arg0, arg1);

    public static string DemandInvalidDemandResolverErrorFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (DemandInvalidDemandResolverErrorFormat), culture, arg0, arg1);
    }

    public static string DemandResolutionErrorFormat(object arg0, object arg1) => ExtMgmtResources.Format(nameof (DemandResolutionErrorFormat), arg0, arg1);

    public static string DemandResolutionErrorFormat(object arg0, object arg1, CultureInfo culture) => ExtMgmtResources.Format(nameof (DemandResolutionErrorFormat), culture, arg0, arg1);

    public static string ContributedFeatureInvalidScopeMessage(object arg0, object arg1) => ExtMgmtResources.Format(nameof (ContributedFeatureInvalidScopeMessage), arg0, arg1);

    public static string ContributedFeatureInvalidScopeMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (ContributedFeatureInvalidScopeMessage), culture, arg0, arg1);
    }

    public static string ContributedFeatureNotFoundMessage(object arg0) => ExtMgmtResources.Format(nameof (ContributedFeatureNotFoundMessage), arg0);

    public static string ContributedFeatureNotFoundMessage(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (ContributedFeatureNotFoundMessage), culture, arg0);

    public static string ErrorFetchingExtensions() => ExtMgmtResources.Get(nameof (ErrorFetchingExtensions));

    public static string ErrorFetchingExtensions(CultureInfo culture) => ExtMgmtResources.Get(nameof (ErrorFetchingExtensions), culture);

    public static string ExtensionNotInstalled(object arg0) => ExtMgmtResources.Format(nameof (ExtensionNotInstalled), arg0);

    public static string ExtensionNotInstalled(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (ExtensionNotInstalled), culture, arg0);

    public static string ExtensionNotLicensed(object arg0) => ExtMgmtResources.Format(nameof (ExtensionNotLicensed), arg0);

    public static string ExtensionNotLicensed(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (ExtensionNotLicensed), culture, arg0);

    public static string AssetNotCachedException(object arg0, object arg1, object arg2) => ExtMgmtResources.Format(nameof (AssetNotCachedException), arg0, arg1, arg2);

    public static string AssetNotCachedException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (AssetNotCachedException), culture, arg0, arg1, arg2);
    }

    public static string AssetNotFound(object arg0, object arg1) => ExtMgmtResources.Format(nameof (AssetNotFound), arg0, arg1);

    public static string AssetNotFound(object arg0, object arg1, CultureInfo culture) => ExtMgmtResources.Format(nameof (AssetNotFound), culture, arg0, arg1);

    public static string AssetProviderNotFound(object arg0, object arg1) => ExtMgmtResources.Format(nameof (AssetProviderNotFound), arg0, arg1);

    public static string AssetProviderNotFound(object arg0, object arg1, CultureInfo culture) => ExtMgmtResources.Format(nameof (AssetProviderNotFound), culture, arg0, arg1);

    public static string AssetVersionNotFound(object arg0, object arg1) => ExtMgmtResources.Format(nameof (AssetVersionNotFound), arg0, arg1);

    public static string AssetVersionNotFound(object arg0, object arg1, CultureInfo culture) => ExtMgmtResources.Format(nameof (AssetVersionNotFound), culture, arg0, arg1);

    public static string ContributionProviderNotFound(object arg0) => ExtMgmtResources.Format(nameof (ContributionProviderNotFound), arg0);

    public static string ContributionProviderNotFound(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (ContributionProviderNotFound), culture, arg0);

    public static string DeploymentInstanceNotFoundException(object arg0, object arg1) => ExtMgmtResources.Format(nameof (DeploymentInstanceNotFoundException), arg0, arg1);

    public static string DeploymentInstanceNotFoundException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (DeploymentInstanceNotFoundException), culture, arg0, arg1);
    }

    public static string ResourceAreaNotFoundException(object arg0, object arg1) => ExtMgmtResources.Format(nameof (ResourceAreaNotFoundException), arg0, arg1);

    public static string ResourceAreaNotFoundException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtMgmtResources.Format(nameof (ResourceAreaNotFoundException), culture, arg0, arg1);
    }

    public static string ResourceAreaNotSet() => ExtMgmtResources.Get(nameof (ResourceAreaNotSet));

    public static string ResourceAreaNotSet(CultureInfo culture) => ExtMgmtResources.Get(nameof (ResourceAreaNotSet), culture);

    public static string ExtensionIsDisabled(object arg0) => ExtMgmtResources.Format(nameof (ExtensionIsDisabled), arg0);

    public static string ExtensionIsDisabled(object arg0, CultureInfo culture) => ExtMgmtResources.Format(nameof (ExtensionIsDisabled), culture, arg0);
  }
}
