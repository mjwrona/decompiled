// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.Resources
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.Azure.Boards.ProcessTemplates.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.Azure.Boards.ProcessTemplates.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.Azure.Boards.ProcessTemplates.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(resourceName) : Microsoft.Azure.Boards.ProcessTemplates.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.Azure.Boards.ProcessTemplates.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Boards.ProcessTemplates.Resources.GetInt(resourceName) : (int) Microsoft.Azure.Boards.ProcessTemplates.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.Azure.Boards.ProcessTemplates.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Boards.ProcessTemplates.Resources.GetBool(resourceName) : (bool) Microsoft.Azure.Boards.ProcessTemplates.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(resourceName, culture);
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

    public static string CannotCreateProjectsWithDisableProcess(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (CannotCreateProjectsWithDisableProcess), arg0);

    public static string CannotCreateProjectsWithDisableProcess(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (CannotCreateProjectsWithDisableProcess), culture, arg0);

    public static string DuplicateFile(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (DuplicateFile), arg0);

    public static string DuplicateFile(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (DuplicateFile), culture, arg0);

    public static string InvalidProcessTemplatePackage() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (InvalidProcessTemplatePackage));

    public static string InvalidProcessTemplatePackage(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (InvalidProcessTemplatePackage), culture);

    public static string PermissionDeniedOnProcess() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (PermissionDeniedOnProcess));

    public static string PermissionDeniedOnProcess(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (PermissionDeniedOnProcess), culture);

    public static string ProcessCannotBeDeleted() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessCannotBeDeleted));

    public static string ProcessCannotBeDeleted(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessCannotBeDeleted), culture);

    public static string ProcessCannotDeleteDefault() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessCannotDeleteDefault));

    public static string ProcessCannotDeleteDefault(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessCannotDeleteDefault), culture);

    public static string ProcessCannotDeleteWithActiveProjects() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessCannotDeleteWithActiveProjects));

    public static string ProcessCannotDeleteWithActiveProjects(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessCannotDeleteWithActiveProjects), culture);

    public static string ProcessDisableException() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessDisableException));

    public static string ProcessDisableException(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessDisableException), culture);

    public static string ProcessInvalidCultureException(object arg0, object arg1) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessInvalidCultureException), arg0, arg1);

    public static string ProcessInvalidCultureException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessInvalidCultureException), culture, arg0, arg1);
    }

    public static string ProcessInvalidInheritedProcessUpdateInput() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessInvalidInheritedProcessUpdateInput));

    public static string ProcessInvalidInheritedProcessUpdateInput(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessInvalidInheritedProcessUpdateInput), culture);

    public static string ProcessLimitExceeded() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessLimitExceeded));

    public static string ProcessLimitExceeded(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessLimitExceeded), culture);

    public static string ProcessNameInvalid(object arg0, object arg1) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessNameInvalid), arg0, arg1);

    public static string ProcessNameInvalid(object arg0, object arg1, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessNameInvalid), culture, arg0, arg1);

    public static string ProcessNameNotSupported(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessNameNotSupported), arg0);

    public static string ProcessNameNotSupported(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessNameNotSupported), culture, arg0);

    public static string ProcessNameNotUnique() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessNameNotUnique));

    public static string ProcessNameNotUnique(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessNameNotUnique), culture);

    public static string ProcessProjectWithInvalidProcess(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessProjectWithInvalidProcess), arg0);

    public static string ProcessProjectWithInvalidProcess(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessProjectWithInvalidProcess), culture, arg0);

    public static string ProcessReferenceNameInvalid(object arg0, object arg1) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessReferenceNameInvalid), arg0, arg1);

    public static string ProcessReferenceNameInvalid(object arg0, object arg1, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessReferenceNameInvalid), culture, arg0, arg1);

    public static string ProcessRefNameNotUnique(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessRefNameNotUnique), arg0);

    public static string ProcessRefNameNotUnique(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessRefNameNotUnique), culture, arg0);

    public static string ProcessTemplateBadlyFormattedMetadata() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateBadlyFormattedMetadata));

    public static string ProcessTemplateBadlyFormattedMetadata(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateBadlyFormattedMetadata), culture);

    public static string ProcessTemplateDescriptionTooLarge(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateDescriptionTooLarge), arg0);

    public static string ProcessTemplateDescriptionTooLarge(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateDescriptionTooLarge), culture, arg0);

    public static string ProcessTemplateEmptyName() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateEmptyName));

    public static string ProcessTemplateEmptyName(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateEmptyName), culture);

    public static string ProcessTemplateFileNotFound(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateFileNotFound), arg0);

    public static string ProcessTemplateFileNotFound(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateFileNotFound), culture, arg0);

    public static string ProcessTemplateInformedStateTransition(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateInformedStateTransition), arg0, arg1, arg2);
    }

    public static string ProcessTemplateInformedStateTransition(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateInformedStateTransition), culture, arg0, arg1, arg2);
    }

    public static string ProcessTemplateInvalidDownloadOnInherited() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateInvalidDownloadOnInherited));

    public static string ProcessTemplateInvalidDownloadOnInherited(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateInvalidDownloadOnInherited), culture);

    public static string ProcessTemplateInvalidOverride(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateInvalidOverride), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string ProcessTemplateInvalidOverride(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateInvalidOverride), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string ProcessTemplateInvalidStateTransition() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateInvalidStateTransition));

    public static string ProcessTemplateInvalidStateTransition(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateInvalidStateTransition), culture);

    public static string ProcessTemplateInvalidVersion() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateInvalidVersion));

    public static string ProcessTemplateInvalidVersion(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateInvalidVersion), culture);

    public static string ProcessTemplateInvalidXmlFile(object arg0, object arg1) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateInvalidXmlFile), arg0, arg1);

    public static string ProcessTemplateInvalidXmlFile(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateInvalidXmlFile), culture, arg0, arg1);
    }

    public static string ProcessTemplateNameConflict(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNameConflict), arg0);

    public static string ProcessTemplateNameConflict(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNameConflict), culture, arg0);

    public static string ProcessTemplateNameDisambugiated(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNameDisambugiated), arg0);

    public static string ProcessTemplateNameDisambugiated(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNameDisambugiated), culture, arg0);

    public static string ProcessTemplateNotFound() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateNotFound));

    public static string ProcessTemplateNotFound(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateNotFound), culture);

    public static string ProcessTemplateNotFoundById(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNotFoundById), arg0);

    public static string ProcessTemplateNotFoundById(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNotFoundById), culture, arg0);

    public static string ProcessTemplateNotFoundByIntegerId(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNotFoundByIntegerId), arg0);

    public static string ProcessTemplateNotFoundByIntegerId(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNotFoundByIntegerId), culture, arg0);

    public static string ProcessTemplateNotFoundByVersion(object arg0, object arg1, object arg2) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNotFoundByVersion), arg0, arg1, arg2);

    public static string ProcessTemplateNotFoundByVersion(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateNotFoundByVersion), culture, arg0, arg1, arg2);
    }

    public static string ProcessTemplatePluginNotFound(object arg0, object arg1) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplatePluginNotFound), arg0, arg1);

    public static string ProcessTemplatePluginNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplatePluginNotFound), culture, arg0, arg1);
    }

    public static string ProcessTemplateSchemaValidation(object arg0, object arg1) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateSchemaValidation), arg0, arg1);

    public static string ProcessTemplateSchemaValidation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateSchemaValidation), culture, arg0, arg1);
    }

    public static string ProcessTemplatesProvisioningPermissionDenied() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplatesProvisioningPermissionDenied));

    public static string ProcessTemplatesProvisioningPermissionDenied(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplatesProvisioningPermissionDenied), culture);

    public static string ProcessTemplateTypeNotFound(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateTypeNotFound), arg0);

    public static string ProcessTemplateTypeNotFound(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateTypeNotFound), culture, arg0);

    public static string ProcessTemplateUploadTooLarge(object arg0, object arg1) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateUploadTooLarge), arg0, arg1);

    public static string ProcessTemplateUploadTooLarge(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateUploadTooLarge), culture, arg0, arg1);
    }

    public static string ProcessTemplateValidationEmbeddedImage() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateValidationEmbeddedImage));

    public static string ProcessTemplateValidationEmbeddedImage(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateValidationEmbeddedImage), culture);

    public static string ProcessTemplateValidatorInvalidPlugin(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateValidatorInvalidPlugin), arg0);

    public static string ProcessTemplateValidatorInvalidPlugin(object arg0, CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateValidatorInvalidPlugin), culture, arg0);

    public static string ProcessTemplateValidatorMultipleTaskList(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateValidatorMultipleTaskList), arg0, arg1, arg2);
    }

    public static string ProcessTemplateValidatorMultipleTaskList(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateValidatorMultipleTaskList), culture, arg0, arg1, arg2);
    }

    public static string ProcessTemplateValidatorRequiredPluginMissing(object arg0) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateValidatorRequiredPluginMissing), arg0);

    public static string ProcessTemplateValidatorRequiredPluginMissing(
      object arg0,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProcessTemplateValidatorRequiredPluginMissing), culture, arg0);
    }

    public static string ProcessTemplateValidatorTypeInvalid() => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateValidatorTypeInvalid));

    public static string ProcessTemplateValidatorTypeInvalid(CultureInfo culture) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Get(nameof (ProcessTemplateValidatorTypeInvalid), culture);

    public static string ProjectsPerXmlProcessLimitExceeded(object arg0, object arg1, object arg2) => Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProjectsPerXmlProcessLimitExceeded), arg0, arg1, arg2);

    public static string ProjectsPerXmlProcessLimitExceeded(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (ProjectsPerXmlProcessLimitExceeded), culture, arg0, arg1, arg2);
    }

    public static string UnableToFindMetadataResourceForProcess(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (UnableToFindMetadataResourceForProcess), arg0, arg1, arg2);
    }

    public static string UnableToFindMetadataResourceForProcess(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.ProcessTemplates.Resources.Format(nameof (UnableToFindMetadataResourceForProcess), culture, arg0, arg1, arg2);
    }
  }
}
