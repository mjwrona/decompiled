// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyResources
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal static class PolicyResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (PolicyResources), typeof (PolicyResources).GetTypeInfo().Assembly);
    public const string InvalidArtifactId = "InvalidArtifactId";
    public const string PolicyConfigurationNotFound = "PolicyConfigurationNotFound";
    public const string PolicyConfigurationNotFoundWithVersion = "PolicyConfigurationNotFoundWithVersion";
    public const string PolicyInvalidGuid = "PolicyInvalidGuid";
    public const string PolicyOperationFailed = "PolicyOperationFailed";
    public const string PolicySettingsFormatException = "PolicySettingsFormatException";
    public const string PolicySettingsFormatExceptionNoError = "PolicySettingsFormatExceptionNoError";
    public const string PolicySettingsTooLargeException = "PolicySettingsTooLargeException";
    public const string EnterpriseDisabled = "EnterpriseDisabled";
    public const string PolicyTypeNotFound = "PolicyTypeNotFound";
    public const string PolicyImplementationException = "PolicyImplementationException";
    public const string PolicyImplementationBadParseSettings = "PolicyImplementationBadParseSettings";
    public const string ConfigurationIdNotSupportedOnPost = "ConfigurationIdNotSupportedOnPost";
    public const string PolicyThrewAnException = "PolicyThrewAnException";
    public const string PolicyImplementationExceptionNoId = "PolicyImplementationExceptionNoId";
    public const string PolicyTypeCannotBeChanged = "PolicyTypeCannotBeChanged";
    public const string ConfigurationIdMismatch = "ConfigurationIdMismatch";
    public const string PolicyFailedToInitialize = "PolicyFailedToInitialize";
    public const string PolicyConfigurationUpdateRejectedByPolicy = "PolicyConfigurationUpdateRejectedByPolicy";
    public const string PolicyImplementationNoAnswerForCheck = "PolicyImplementationNoAnswerForCheck";
    public const string PolicyIsDeleteCannotBeUsedForDeletion = "PolicyIsDeleteCannotBeUsedForDeletion";
    public const string PolicyCustomErrorException = "PolicyCustomErrorException";
    public const string PolicyEvaluationNotFound = "PolicyEvaluationNotFound";
    public const string PolicyEvaluationForArtifactCannotBeRequeued = "PolicyEvaluationForArtifactCannotBeRequeued";
    public const string PolicyArtifactCouldNotBeDecoded = "PolicyArtifactCouldNotBeDecoded";
    public const string ArtifactIdNotFound = "ArtifactIdNotFound";
    public const string PolicyConfigurationUpdatedByAnotherRequest = "PolicyConfigurationUpdatedByAnotherRequest";
    public const string CannotCreateNewDeprecatedScope = "CannotCreateNewDeprecatedScope";
    public const string InvalidContinuationToken = "InvalidContinuationToken";
    public const string PaginationIsNotSupported = "PaginationIsNotSupported";
    public const string CannotCreateUnknownScope = "CannotCreateUnknownScope";

    public static ResourceManager Manager => PolicyResources.s_resMgr;

    public static string Get(string resourceName) => PolicyResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? PolicyResources.Get(resourceName) : PolicyResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PolicyResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PolicyResources.GetInt(resourceName) : (int) PolicyResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PolicyResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PolicyResources.GetBool(resourceName) : (bool) PolicyResources.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => PolicyResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PolicyResources.Get(resourceName, culture);
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
