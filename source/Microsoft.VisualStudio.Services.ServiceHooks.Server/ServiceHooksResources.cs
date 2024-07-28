// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ServiceHooksResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ServiceHooksResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ServiceHooksResources.resourceMan == null)
          ServiceHooksResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksResources", typeof (ServiceHooksResources).Assembly);
        return ServiceHooksResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ServiceHooksResources.resourceCulture;
      set => ServiceHooksResources.resourceCulture = value;
    }

    internal static string AccountMigrationInProgress => ServiceHooksResources.ResourceManager.GetString(nameof (AccountMigrationInProgress), ServiceHooksResources.resourceCulture);

    internal static string Error_CannotResetAndIncrementProbationRetries => ServiceHooksResources.ResourceManager.GetString(nameof (Error_CannotResetAndIncrementProbationRetries), ServiceHooksResources.resourceCulture);

    internal static string Error_ConfidentialInputValuesRequired => ServiceHooksResources.ResourceManager.GetString(nameof (Error_ConfidentialInputValuesRequired), ServiceHooksResources.resourceCulture);

    internal static string Error_CouldNotSendNotification => ServiceHooksResources.ResourceManager.GetString(nameof (Error_CouldNotSendNotification), ServiceHooksResources.resourceCulture);

    internal static string Error_DetailActivityIdFormat => ServiceHooksResources.ResourceManager.GetString(nameof (Error_DetailActivityIdFormat), ServiceHooksResources.resourceCulture);

    internal static string Error_DetailExPlusActivityIdFormat => ServiceHooksResources.ResourceManager.GetString(nameof (Error_DetailExPlusActivityIdFormat), ServiceHooksResources.resourceCulture);

    internal static string Error_InvalidConsumerActionIdForConsumerId => ServiceHooksResources.ResourceManager.GetString(nameof (Error_InvalidConsumerActionIdForConsumerId), ServiceHooksResources.resourceCulture);

    internal static string Error_InvalidConsumerId => ServiceHooksResources.ResourceManager.GetString(nameof (Error_InvalidConsumerId), ServiceHooksResources.resourceCulture);

    internal static string Error_InvalidSubscriptionIdOnSubscriptionInputTemplate => ServiceHooksResources.ResourceManager.GetString(nameof (Error_InvalidSubscriptionIdOnSubscriptionInputTemplate), ServiceHooksResources.resourceCulture);

    internal static string Error_MaskedConsumerInputValueFormat => ServiceHooksResources.ResourceManager.GetString(nameof (Error_MaskedConsumerInputValueFormat), ServiceHooksResources.resourceCulture);

    internal static string Error_MissingConfigurationSettingTemplate => ServiceHooksResources.ResourceManager.GetString(nameof (Error_MissingConfigurationSettingTemplate), ServiceHooksResources.resourceCulture);

    internal static string Error_MissingRequiredConsumerInputFormat => ServiceHooksResources.ResourceManager.GetString(nameof (Error_MissingRequiredConsumerInputFormat), ServiceHooksResources.resourceCulture);

    internal static string Error_MissingSubscriptionBySpecifiedIdTemplate => ServiceHooksResources.ResourceManager.GetString(nameof (Error_MissingSubscriptionBySpecifiedIdTemplate), ServiceHooksResources.resourceCulture);

    internal static string Error_MissingSubscriptionInputElementTemplate => ServiceHooksResources.ResourceManager.GetString(nameof (Error_MissingSubscriptionInputElementTemplate), ServiceHooksResources.resourceCulture);

    internal static string Error_MissingSubscriptionQueryCriteria => ServiceHooksResources.ResourceManager.GetString(nameof (Error_MissingSubscriptionQueryCriteria), ServiceHooksResources.resourceCulture);

    internal static string Error_NoActionsForConsumerId => ServiceHooksResources.ResourceManager.GetString(nameof (Error_NoActionsForConsumerId), ServiceHooksResources.resourceCulture);

    internal static string Error_PublisherInputsNotQueryable => ServiceHooksResources.ResourceManager.GetString(nameof (Error_PublisherInputsNotQueryable), ServiceHooksResources.resourceCulture);

    internal static string Error_PublisherInputValueCannotBeNull => ServiceHooksResources.ResourceManager.GetString(nameof (Error_PublisherInputValueCannotBeNull), ServiceHooksResources.resourceCulture);

    internal static string Error_UnexpectedResourceVersionFormat => ServiceHooksResources.ResourceManager.GetString(nameof (Error_UnexpectedResourceVersionFormat), ServiceHooksResources.resourceCulture);

    internal static string Error_UnknownConsumerInputFormat => ServiceHooksResources.ResourceManager.GetString(nameof (Error_UnknownConsumerInputFormat), ServiceHooksResources.resourceCulture);

    internal static string Error_UnknownKpiMetricNameTemplate => ServiceHooksResources.ResourceManager.GetString(nameof (Error_UnknownKpiMetricNameTemplate), ServiceHooksResources.resourceCulture);

    internal static string Error_UnsupportedResourceVersionFormat => ServiceHooksResources.ResourceManager.GetString(nameof (Error_UnsupportedResourceVersionFormat), ServiceHooksResources.resourceCulture);

    internal static string InputIdentifierUnknown => ServiceHooksResources.ResourceManager.GetString(nameof (InputIdentifierUnknown), ServiceHooksResources.resourceCulture);

    internal static string InputValueMissing => ServiceHooksResources.ResourceManager.GetString(nameof (InputValueMissing), ServiceHooksResources.resourceCulture);

    internal static string NoCustomEventDescription => ServiceHooksResources.ResourceManager.GetString(nameof (NoCustomEventDescription), ServiceHooksResources.resourceCulture);

    internal static string ProjectIdOrSubscriberIdRequired => ServiceHooksResources.ResourceManager.GetString(nameof (ProjectIdOrSubscriberIdRequired), ServiceHooksResources.resourceCulture);

    internal static string Truncated => ServiceHooksResources.ResourceManager.GetString(nameof (Truncated), ServiceHooksResources.resourceCulture);
  }
}
