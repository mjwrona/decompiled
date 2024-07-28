// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.PipelineChecksResources
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal static class PipelineChecksResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (PipelineChecksResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PipelineChecksResources.s_resMgr;

    private static string Get(string resourceName) => PipelineChecksResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PipelineChecksResources.Get(resourceName) : PipelineChecksResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PipelineChecksResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PipelineChecksResources.GetInt(resourceName) : (int) PipelineChecksResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PipelineChecksResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PipelineChecksResources.GetBool(resourceName) : (bool) PipelineChecksResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PipelineChecksResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PipelineChecksResources.Get(resourceName, culture);
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

    public static string AccessDeniedException() => PipelineChecksResources.Get(nameof (AccessDeniedException));

    public static string AccessDeniedException(CultureInfo culture) => PipelineChecksResources.Get(nameof (AccessDeniedException), culture);

    public static string CheckAbortedMessage() => PipelineChecksResources.Get(nameof (CheckAbortedMessage));

    public static string CheckAbortedMessage(CultureInfo culture) => PipelineChecksResources.Get(nameof (CheckAbortedMessage), culture);

    public static string ChecksConfigurationNotFound(object arg0) => PipelineChecksResources.Format(nameof (ChecksConfigurationNotFound), arg0);

    public static string ChecksConfigurationNotFound(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (ChecksConfigurationNotFound), culture, arg0);

    public static string ChecksRunExits(object arg0) => PipelineChecksResources.Format(nameof (ChecksRunExits), arg0);

    public static string ChecksRunExits(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (ChecksRunExits), culture, arg0);

    public static string ChecksRunNotFound(object arg0) => PipelineChecksResources.Format(nameof (ChecksRunNotFound), arg0);

    public static string ChecksRunNotFound(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (ChecksRunNotFound), culture, arg0);

    public static string ChecksSuiteIdExits(object arg0) => PipelineChecksResources.Format(nameof (ChecksSuiteIdExits), arg0);

    public static string ChecksSuiteIdExits(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (ChecksSuiteIdExits), culture, arg0);

    public static string ChecksSuiteIdNotFound(object arg0) => PipelineChecksResources.Format(nameof (ChecksSuiteIdNotFound), arg0);

    public static string ChecksSuiteIdNotFound(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (ChecksSuiteIdNotFound), culture, arg0);

    public static string CheckTypeIdNotFound(object arg0) => PipelineChecksResources.Format(nameof (CheckTypeIdNotFound), arg0);

    public static string CheckTypeIdNotFound(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (CheckTypeIdNotFound), culture, arg0);

    public static string CheckTypeNameNotFound(object arg0) => PipelineChecksResources.Format(nameof (CheckTypeNameNotFound), arg0);

    public static string CheckTypeNameNotFound(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (CheckTypeNameNotFound), culture, arg0);

    public static string GenericDatabaseUpdateError() => PipelineChecksResources.Get(nameof (GenericDatabaseUpdateError));

    public static string GenericDatabaseUpdateError(CultureInfo culture) => PipelineChecksResources.Get(nameof (GenericDatabaseUpdateError), culture);

    public static string InvalidCheckTypeException() => PipelineChecksResources.Get(nameof (InvalidCheckTypeException));

    public static string InvalidCheckTypeException(CultureInfo culture) => PipelineChecksResources.Get(nameof (InvalidCheckTypeException), culture);

    public static string FailedJobStatus(object arg0) => PipelineChecksResources.Format(nameof (FailedJobStatus), arg0);

    public static string FailedJobStatus(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (FailedJobStatus), culture, arg0);

    public static string SuccessfulJobStatus(object arg0) => PipelineChecksResources.Format(nameof (SuccessfulJobStatus), arg0);

    public static string SuccessfulJobStatus(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (SuccessfulJobStatus), culture, arg0);

    public static string ResourceAccessDenied() => PipelineChecksResources.Get(nameof (ResourceAccessDenied));

    public static string ResourceAccessDenied(CultureInfo culture) => PipelineChecksResources.Get(nameof (ResourceAccessDenied), culture);

    public static string InvalidResourceId(object arg0) => PipelineChecksResources.Format(nameof (InvalidResourceId), arg0);

    public static string InvalidResourceId(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (InvalidResourceId), culture, arg0);

    public static string InvalidResourceType(object arg0) => PipelineChecksResources.Format(nameof (InvalidResourceType), arg0);

    public static string InvalidResourceType(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (InvalidResourceType), culture, arg0);

    public static string InValidCheckConfiguration(object arg0, object arg1) => PipelineChecksResources.Format(nameof (InValidCheckConfiguration), arg0, arg1);

    public static string InValidCheckConfiguration(object arg0, object arg1, CultureInfo culture) => PipelineChecksResources.Format(nameof (InValidCheckConfiguration), culture, arg0, arg1);

    public static string CheckSuiteIsAlreadyCompleted(object arg0) => PipelineChecksResources.Format(nameof (CheckSuiteIsAlreadyCompleted), arg0);

    public static string CheckSuiteIsAlreadyCompleted(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (CheckSuiteIsAlreadyCompleted), culture, arg0);

    public static string ChecksConfigurationUpdateRequired() => PipelineChecksResources.Get(nameof (ChecksConfigurationUpdateRequired));

    public static string ChecksConfigurationUpdateRequired(CultureInfo culture) => PipelineChecksResources.Get(nameof (ChecksConfigurationUpdateRequired), culture);

    public static string NonCompliantCheckConfigurations(object arg0) => PipelineChecksResources.Format(nameof (NonCompliantCheckConfigurations), arg0);

    public static string NonCompliantCheckConfigurations(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (NonCompliantCheckConfigurations), culture, arg0);

    public static string CheckCannotBeRerun(object arg0) => PipelineChecksResources.Format(nameof (CheckCannotBeRerun), arg0);

    public static string CheckCannotBeRerun(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (CheckCannotBeRerun), culture, arg0);

    public static string CheckRunAlreadyBypassed(object arg0, object arg1) => PipelineChecksResources.Format(nameof (CheckRunAlreadyBypassed), arg0, arg1);

    public static string CheckRunAlreadyBypassed(object arg0, object arg1, CultureInfo culture) => PipelineChecksResources.Format(nameof (CheckRunAlreadyBypassed), culture, arg0, arg1);

    public static string CheckRunAlreadyBypassedBy(object arg0, object arg1, object arg2) => PipelineChecksResources.Format(nameof (CheckRunAlreadyBypassedBy), arg0, arg1, arg2);

    public static string CheckRunAlreadyBypassedBy(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineChecksResources.Format(nameof (CheckRunAlreadyBypassedBy), culture, arg0, arg1, arg2);
    }

    public static string CheckRunBypassGenericError() => PipelineChecksResources.Get(nameof (CheckRunBypassGenericError));

    public static string CheckRunBypassGenericError(CultureInfo culture) => PipelineChecksResources.Get(nameof (CheckRunBypassGenericError), culture);

    public static string CheckTypeAlreadyExists(object arg0) => PipelineChecksResources.Format(nameof (CheckTypeAlreadyExists), arg0);

    public static string CheckTypeAlreadyExists(object arg0, CultureInfo culture) => PipelineChecksResources.Format(nameof (CheckTypeAlreadyExists), culture, arg0);
  }
}
