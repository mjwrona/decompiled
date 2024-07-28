// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DeploymentResources
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.Deployment
{
  internal static class DeploymentResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (DeploymentResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DeploymentResources.s_resMgr;

    private static string Get(string resourceName) => DeploymentResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DeploymentResources.Get(resourceName) : DeploymentResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DeploymentResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DeploymentResources.GetInt(resourceName) : (int) DeploymentResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DeploymentResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DeploymentResources.GetBool(resourceName) : (bool) DeploymentResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DeploymentResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DeploymentResources.Get(resourceName, culture);
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

    public static string AmbigousServiceEndpointUsed(object arg0) => DeploymentResources.Format(nameof (AmbigousServiceEndpointUsed), arg0);

    public static string AmbigousServiceEndpointUsed(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (AmbigousServiceEndpointUsed), culture, arg0);

    public static string CannotCreateUniqueIdentifierForArtifactType(object arg0, object arg1) => DeploymentResources.Format(nameof (CannotCreateUniqueIdentifierForArtifactType), arg0, arg1);

    public static string CannotCreateUniqueIdentifierForArtifactType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DeploymentResources.Format(nameof (CannotCreateUniqueIdentifierForArtifactType), culture, arg0, arg1);
    }

    public static string CannotCreateUniqueIdentifierInvalidInput() => DeploymentResources.Get(nameof (CannotCreateUniqueIdentifierInvalidInput));

    public static string CannotCreateUniqueIdentifierInvalidInput(CultureInfo culture) => DeploymentResources.Get(nameof (CannotCreateUniqueIdentifierInvalidInput), culture);

    public static string CannotFindWebHookPublisher(object arg0) => DeploymentResources.Format(nameof (CannotFindWebHookPublisher), arg0);

    public static string CannotFindWebHookPublisher(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (CannotFindWebHookPublisher), culture, arg0);

    public static string CannotGetWebHookPublisher() => DeploymentResources.Get(nameof (CannotGetWebHookPublisher));

    public static string CannotGetWebHookPublisher(CultureInfo culture) => DeploymentResources.Get(nameof (CannotGetWebHookPublisher), culture);

    public static string CannotGetWebHookPublisherRequiredProperty(object arg0) => DeploymentResources.Format(nameof (CannotGetWebHookPublisherRequiredProperty), arg0);

    public static string CannotGetWebHookPublisherRequiredProperty(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (CannotGetWebHookPublisherRequiredProperty), culture, arg0);

    public static string CannotMeetFilterCriteriaForPipelineTriggers() => DeploymentResources.Get(nameof (CannotMeetFilterCriteriaForPipelineTriggers));

    public static string CannotMeetFilterCriteriaForPipelineTriggers(CultureInfo culture) => DeploymentResources.Get(nameof (CannotMeetFilterCriteriaForPipelineTriggers), culture);

    public static string ContainerJobTraceabilityError(object arg0, object arg1) => DeploymentResources.Format(nameof (ContainerJobTraceabilityError), arg0, arg1);

    public static string ContainerJobTraceabilityError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DeploymentResources.Format(nameof (ContainerJobTraceabilityError), culture, arg0, arg1);
    }

    public static string CouldNotFetchUser() => DeploymentResources.Get(nameof (CouldNotFetchUser));

    public static string CouldNotFetchUser(CultureInfo culture) => DeploymentResources.Get(nameof (CouldNotFetchUser), culture);

    public static string EndpointOfTypeNotFound(object arg0, object arg1) => DeploymentResources.Format(nameof (EndpointOfTypeNotFound), arg0, arg1);

    public static string EndpointOfTypeNotFound(object arg0, object arg1, CultureInfo culture) => DeploymentResources.Format(nameof (EndpointOfTypeNotFound), culture, arg0, arg1);

    public static string IncorrectPackageFormat() => DeploymentResources.Get(nameof (IncorrectPackageFormat));

    public static string IncorrectPackageFormat(CultureInfo culture) => DeploymentResources.Get(nameof (IncorrectPackageFormat), culture);

    public static string ErrorTriggerConfiguration() => DeploymentResources.Get(nameof (ErrorTriggerConfiguration));

    public static string ErrorTriggerConfiguration(CultureInfo culture) => DeploymentResources.Get(nameof (ErrorTriggerConfiguration), culture);

    public static string InvalidJobIdColumnType(object arg0) => DeploymentResources.Format(nameof (InvalidJobIdColumnType), arg0);

    public static string InvalidJobIdColumnType(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (InvalidJobIdColumnType), culture, arg0);

    public static string InvalidParameterFound(object arg0) => DeploymentResources.Format(nameof (InvalidParameterFound), arg0);

    public static string InvalidParameterFound(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (InvalidParameterFound), culture, arg0);

    public static string MissingContainerResourceField(object arg0, object arg1) => DeploymentResources.Format(nameof (MissingContainerResourceField), arg0, arg1);

    public static string MissingContainerResourceField(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DeploymentResources.Format(nameof (MissingContainerResourceField), culture, arg0, arg1);
    }

    public static string NoteExists(object arg0) => DeploymentResources.Format(nameof (NoteExists), arg0);

    public static string NoteExists(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (NoteExists), culture, arg0);

    public static string NoteNotFound(object arg0) => DeploymentResources.Format(nameof (NoteNotFound), arg0);

    public static string NoteNotFound(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (NoteNotFound), culture, arg0);

    public static string OccurrenceNotFound(object arg0) => DeploymentResources.Format(nameof (OccurrenceNotFound), arg0);

    public static string OccurrenceNotFound(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (OccurrenceNotFound), culture, arg0);

    public static string OccurrenceTagExists(object arg0) => DeploymentResources.Format(nameof (OccurrenceTagExists), arg0);

    public static string OccurrenceTagExists(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (OccurrenceTagExists), culture, arg0);

    public static string PlanNotFound(object arg0) => DeploymentResources.Format(nameof (PlanNotFound), arg0);

    public static string PlanNotFound(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (PlanNotFound), culture, arg0);

    public static string CannotTriggerPipelineForTriggerEvent(object arg0) => DeploymentResources.Format(nameof (CannotTriggerPipelineForTriggerEvent), arg0);

    public static string CannotTriggerPipelineForTriggerEvent(object arg0, CultureInfo culture) => DeploymentResources.Format(nameof (CannotTriggerPipelineForTriggerEvent), culture, arg0);
  }
}
