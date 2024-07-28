// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Core.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 519ECFE1-FB7F-41E8-9E4A-A2FC032ED5AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Core.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Core.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(resourceName) : Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(resourceName, culture);
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

    public static string TaskDefinitionInvalidDependentInputs(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidDependentInputs), arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidDependentInputs(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidDependentInputs), culture, arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidDependentInputsOrder(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidDependentInputsOrder), arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidDependentInputsOrder(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidDependentInputsOrder), culture, arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidTaskInput(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidTaskInput), arg0, arg1);

    public static string TaskDefinitionInvalidTaskInput(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidTaskInput), culture, arg0, arg1);
    }

    public static string TaskDefinitionInvalidVisibleRuleFormat(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidVisibleRuleFormat), arg0, arg1);

    public static string TaskDefinitionInvalidVisibleRuleFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidVisibleRuleFormat), culture, arg0, arg1);
    }

    public static string TaskDefinitionInvalidVisibleRuleRelation(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidVisibleRuleRelation), arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidVisibleRuleRelation(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidVisibleRuleRelation), culture, arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidCategory(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidCategory), arg0);

    public static string TaskDefinitionInvalidCategory(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidCategory), culture, arg0);

    public static string TaskDefinitionInvalidVisibleRuleCondition(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidVisibleRuleCondition), arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidVisibleRuleCondition(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidVisibleRuleCondition), culture, arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidVisibleRuleOperators(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidVisibleRuleOperators), arg0);

    public static string TaskDefinitionInvalidVisibleRuleOperators(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidVisibleRuleOperators), culture, arg0);

    public static string DataSourceBindingInvalid(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (DataSourceBindingInvalid), arg0);

    public static string DataSourceBindingInvalid(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (DataSourceBindingInvalid), culture, arg0);

    public static string EndpointIdMissingOrEmpty(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (EndpointIdMissingOrEmpty), arg0);

    public static string EndpointIdMissingOrEmpty(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (EndpointIdMissingOrEmpty), culture, arg0);

    public static string TargetMissingOrEmpty(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TargetMissingOrEmpty), arg0);

    public static string TargetMissingOrEmpty(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TargetMissingOrEmpty), culture, arg0);

    public static string EndpointNameInvalidMessage() => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (EndpointNameInvalidMessage));

    public static string EndpointNameInvalidMessage(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (EndpointNameInvalidMessage), culture);

    public static string EndpointTypeInvalidMessage() => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (EndpointTypeInvalidMessage));

    public static string EndpointTypeInvalidMessage(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (EndpointTypeInvalidMessage), culture);

    public static string EndpointDisplayNameInvalidMessage() => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (EndpointDisplayNameInvalidMessage));

    public static string EndpointDisplayNameInvalidMessage(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (EndpointDisplayNameInvalidMessage), culture);

    public static string EndpointTypeMissingAuthScheme() => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (EndpointTypeMissingAuthScheme));

    public static string EndpointTypeMissingAuthScheme(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (EndpointTypeMissingAuthScheme), culture);

    public static string EndpointTypeInvalidAuthSchemes(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (EndpointTypeInvalidAuthSchemes), arg0);

    public static string EndpointTypeInvalidAuthSchemes(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (EndpointTypeInvalidAuthSchemes), culture, arg0);

    public static string ContributionIdDoesNotMatchTaskId(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionIdDoesNotMatchTaskId), arg0, arg1, arg2, arg3);
    }

    public static string ContributionIdDoesNotMatchTaskId(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionIdDoesNotMatchTaskId), culture, arg0, arg1, arg2, arg3);
    }

    public static string MissingContribution(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (MissingContribution), arg0);

    public static string MissingContribution(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (MissingContribution), culture, arg0);

    public static string ContributionUsesExistingTaskID(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionUsesExistingTaskID), arg0, arg1);

    public static string ContributionUsesExistingTaskID(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionUsesExistingTaskID), culture, arg0, arg1);
    }

    public static string ContributionReusesTaskId(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionReusesTaskId), arg0);

    public static string ContributionReusesTaskId(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionReusesTaskId), culture, arg0);

    public static string ContributionsWithSameTaskId(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionsWithSameTaskId), arg0, arg1);

    public static string ContributionsWithSameTaskId(object arg0, object arg1, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionsWithSameTaskId), culture, arg0, arg1);

    public static string TaskDefinitionInvalidGroupVisibleRuleDependent(
      object arg0,
      object arg1,
      object arg2)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidGroupVisibleRuleDependent), arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidGroupVisibleRuleDependent(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidGroupVisibleRuleDependent), culture, arg0, arg1, arg2);
    }

    public static string TaskDefinitionInvalidAliasDuplicate(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidAliasDuplicate), arg0);

    public static string TaskDefinitionInvalidAliasDuplicate(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidAliasDuplicate), culture, arg0);

    public static string TaskDefinitionInvalidAliasMatchingName(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidAliasMatchingName), arg0);

    public static string TaskDefinitionInvalidAliasMatchingName(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (TaskDefinitionInvalidAliasMatchingName), culture, arg0);

    public static string InvalidUrlForEndpoint(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (InvalidUrlForEndpoint), arg0);

    public static string InvalidUrlForEndpoint(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (InvalidUrlForEndpoint), culture, arg0);

    public static string ParsingFailedForEndpointType(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ParsingFailedForEndpointType), arg0);

    public static string ParsingFailedForEndpointType(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ParsingFailedForEndpointType), culture, arg0);

    public static string TaskDefinitionInvalidIdRequired() => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (TaskDefinitionInvalidIdRequired));

    public static string TaskDefinitionInvalidIdRequired(CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Get(nameof (TaskDefinitionInvalidIdRequired), culture);

    public static string ContributionUsesExistingServiceEndpointName(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionUsesExistingServiceEndpointName), arg0, arg1);

    public static string ContributionUsesExistingServiceEndpointName(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionUsesExistingServiceEndpointName), culture, arg0, arg1);
    }

    public static string ContributionsWithSameServiceEndpointName(object arg0, object arg1) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionsWithSameServiceEndpointName), arg0, arg1);

    public static string ContributionsWithSameServiceEndpointName(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Resources.Format(nameof (ContributionsWithSameServiceEndpointName), culture, arg0, arg1);
    }
  }
}
