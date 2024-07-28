// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionManagementResources
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  internal static class ExtensionManagementResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ExtensionManagementResources), typeof (ExtensionManagementResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ExtensionManagementResources.s_resMgr;

    private static string Get(string resourceName) => ExtensionManagementResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ExtensionManagementResources.Get(resourceName) : ExtensionManagementResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ExtensionManagementResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ExtensionManagementResources.GetInt(resourceName) : (int) ExtensionManagementResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ExtensionManagementResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ExtensionManagementResources.GetBool(resourceName) : (bool) ExtensionManagementResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ExtensionManagementResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ExtensionManagementResources.Get(resourceName, culture);
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

    public static string AppNotFoundForAppIdException(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (AppNotFoundForAppIdException), arg0, arg1);

    public static string AppNotFoundForAppIdException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (AppNotFoundForAppIdException), culture, arg0, arg1);
    }

    public static string InstalledAppNotFoundForAppIdException(object arg0) => ExtensionManagementResources.Format(nameof (InstalledAppNotFoundForAppIdException), arg0);

    public static string InstalledAppNotFoundForAppIdException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (InstalledAppNotFoundForAppIdException), culture, arg0);

    public static string CollectionDoesNotExistException(object arg0, object arg1, object arg2) => ExtensionManagementResources.Format(nameof (CollectionDoesNotExistException), arg0, arg1, arg2);

    public static string CollectionDoesNotExistException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (CollectionDoesNotExistException), culture, arg0, arg1, arg2);
    }

    public static string DocumentAlreadyExistsException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExtensionManagementResources.Format(nameof (DocumentAlreadyExistsException), arg0, arg1, arg2, arg3);
    }

    public static string DocumentAlreadyExistsException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (DocumentAlreadyExistsException), culture, arg0, arg1, arg2, arg3);
    }

    public static string DocumentDoesNotExistException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExtensionManagementResources.Format(nameof (DocumentDoesNotExistException), arg0, arg1, arg2, arg3);
    }

    public static string DocumentDoesNotExistException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (DocumentDoesNotExistException), culture, arg0, arg1, arg2, arg3);
    }

    public static string ExtensionAlreadyInstalledException(object arg0) => ExtensionManagementResources.Format(nameof (ExtensionAlreadyInstalledException), arg0);

    public static string ExtensionAlreadyInstalledException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (ExtensionAlreadyInstalledException), culture, arg0);

    public static string InvalidDocumentVersionException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExtensionManagementResources.Format(nameof (InvalidDocumentVersionException), arg0, arg1, arg2, arg3);
    }

    public static string InvalidDocumentVersionException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (InvalidDocumentVersionException), culture, arg0, arg1, arg2, arg3);
    }

    public static string InvalidInstallationTargetException(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (InvalidInstallationTargetException), arg0, arg1);

    public static string InvalidInstallationTargetException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (InvalidInstallationTargetException), culture, arg0, arg1);
    }

    public static string AppInstallAccessException(object arg0) => ExtensionManagementResources.Format(nameof (AppInstallAccessException), arg0);

    public static string AppInstallAccessException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (AppInstallAccessException), culture, arg0);

    public static string AppManifestValidationException(object arg0) => ExtensionManagementResources.Format(nameof (AppManifestValidationException), arg0);

    public static string AppManifestValidationException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (AppManifestValidationException), culture, arg0);

    public static string AppNamespaceExistsException(object arg0) => ExtensionManagementResources.Format(nameof (AppNamespaceExistsException), arg0);

    public static string AppNamespaceExistsException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (AppNamespaceExistsException), culture, arg0);

    public static string AppNotFoundForNamespaceException(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (AppNotFoundForNamespaceException), arg0, arg1);

    public static string AppNotFoundForNamespaceException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (AppNotFoundForNamespaceException), culture, arg0, arg1);
    }

    public static string AppNotFoundForStoreException(object arg0, object arg1, object arg2) => ExtensionManagementResources.Format(nameof (AppNotFoundForStoreException), arg0, arg1, arg2);

    public static string AppNotFoundForStoreException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (AppNotFoundForStoreException), culture, arg0, arg1, arg2);
    }

    public static string AppPublishAccessException(object arg0) => ExtensionManagementResources.Format(nameof (AppPublishAccessException), arg0);

    public static string AppPublishAccessException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (AppPublishAccessException), culture, arg0);

    public static string ContributionPointNotFoundError(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionPointNotFoundError), arg0, arg1);

    public static string ContributionPointNotFoundError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionPointNotFoundError), culture, arg0, arg1);
    }

    public static string ContributionPointOwnerNotFoundError(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionPointOwnerNotFoundError), arg0, arg1);

    public static string ContributionPointOwnerNotFoundError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionPointOwnerNotFoundError), culture, arg0, arg1);
    }

    public static string ContributionTypeNotFoundError(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionTypeNotFoundError), arg0, arg1);

    public static string ContributionTypeNotFoundError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionTypeNotFoundError), culture, arg0, arg1);
    }

    public static string ContributionTypeOwnerNotFoundError(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionTypeOwnerNotFoundError), arg0, arg1);

    public static string ContributionTypeOwnerNotFoundError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionTypeOwnerNotFoundError), culture, arg0, arg1);
    }

    public static string ContributionTypeSchemaValidationError(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionTypeSchemaValidationError), arg0, arg1);

    public static string ContributionTypeSchemaValidationError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionTypeSchemaValidationError), culture, arg0, arg1);
    }

    public static string ContributionTypeUndefinedError(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionTypeUndefinedError), arg0, arg1);

    public static string ContributionTypeUndefinedError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionTypeUndefinedError), culture, arg0, arg1);
    }

    public static string ContributionValidationError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ExtensionManagementResources.Format(nameof (ContributionValidationError), arg0, arg1, arg2, arg3);
    }

    public static string ContributionValidationError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionValidationError), culture, arg0, arg1, arg2, arg3);
    }

    public static string ExtensionRequestAlreadyExistsException() => ExtensionManagementResources.Get(nameof (ExtensionRequestAlreadyExistsException));

    public static string ExtensionRequestAlreadyExistsException(CultureInfo culture) => ExtensionManagementResources.Get(nameof (ExtensionRequestAlreadyExistsException), culture);

    public static string ExtensionUnauthorizedException(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ExtensionUnauthorizedException), arg0, arg1);

    public static string ExtensionUnauthorizedException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ExtensionUnauthorizedException), culture, arg0, arg1);
    }

    public static string TokenMissingHostAuthorizationClaimException() => ExtensionManagementResources.Get(nameof (TokenMissingHostAuthorizationClaimException));

    public static string TokenMissingHostAuthorizationClaimException(CultureInfo culture) => ExtensionManagementResources.Get(nameof (TokenMissingHostAuthorizationClaimException), culture);

    public static string ExtensionNotLoaded(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ExtensionNotLoaded), arg0, arg1);

    public static string ExtensionNotLoaded(object arg0, object arg1, CultureInfo culture) => ExtensionManagementResources.Format(nameof (ExtensionNotLoaded), culture, arg0, arg1);

    public static string DocumentCollectionDoesNotExistException(
      object arg0,
      object arg1,
      object arg2)
    {
      return ExtensionManagementResources.Format(nameof (DocumentCollectionDoesNotExistException), arg0, arg1, arg2);
    }

    public static string DocumentCollectionDoesNotExistException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (DocumentCollectionDoesNotExistException), culture, arg0, arg1, arg2);
    }

    public static string ContributionEmbeddedException(object arg0) => ExtensionManagementResources.Format(nameof (ContributionEmbeddedException), arg0);

    public static string ContributionEmbeddedException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (ContributionEmbeddedException), culture, arg0);

    public static string ContributionIdInvalidInElementException(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionIdInvalidInElementException), arg0, arg1);

    public static string ContributionIdInvalidInElementException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionIdInvalidInElementException), culture, arg0, arg1);
    }

    public static string ContributionIdRequiredInElementException(object arg0) => ExtensionManagementResources.Format(nameof (ContributionIdRequiredInElementException), arg0);

    public static string ContributionIdRequiredInElementException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (ContributionIdRequiredInElementException), culture, arg0);

    public static string ContributionRendererNotDefinedException(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionRendererNotDefinedException), arg0, arg1);

    public static string ContributionRendererNotDefinedException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionRendererNotDefinedException), culture, arg0, arg1);
    }

    public static string ContributionElementTypeException(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (ContributionElementTypeException), arg0, arg1);

    public static string ContributionElementTypeException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (ContributionElementTypeException), culture, arg0, arg1);
    }

    public static string CharacterLimitExceeded(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (CharacterLimitExceeded), arg0, arg1);

    public static string CharacterLimitExceeded(object arg0, object arg1, CultureInfo culture) => ExtensionManagementResources.Format(nameof (CharacterLimitExceeded), culture, arg0, arg1);

    public static string ExtensionDemandsNotSupportedException(object arg0) => ExtensionManagementResources.Format(nameof (ExtensionDemandsNotSupportedException), arg0);

    public static string ExtensionDemandsNotSupportedException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (ExtensionDemandsNotSupportedException), culture, arg0);

    public static string ExtensionDemandErrorFormat(object arg0) => ExtensionManagementResources.Format(nameof (ExtensionDemandErrorFormat), arg0);

    public static string ExtensionDemandErrorFormat(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (ExtensionDemandErrorFormat), culture, arg0);

    public static string ExtensionDemandsNotSupportedWithoutReasonException() => ExtensionManagementResources.Get(nameof (ExtensionDemandsNotSupportedWithoutReasonException));

    public static string ExtensionDemandsNotSupportedWithoutReasonException(CultureInfo culture) => ExtensionManagementResources.Get(nameof (ExtensionDemandsNotSupportedWithoutReasonException), culture);

    public static string ValueCannotBeNullException(object arg0) => ExtensionManagementResources.Format(nameof (ValueCannotBeNullException), arg0);

    public static string ValueCannotBeNullException(object arg0, CultureInfo culture) => ExtensionManagementResources.Format(nameof (ValueCannotBeNullException), culture, arg0);

    public static string MaximumDocumentSizeException(object arg0, object arg1) => ExtensionManagementResources.Format(nameof (MaximumDocumentSizeException), arg0, arg1);

    public static string MaximumDocumentSizeException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ExtensionManagementResources.Format(nameof (MaximumDocumentSizeException), culture, arg0, arg1);
    }
  }
}
