// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.FeatureFlags
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public static class FeatureFlags
  {
    internal const string EnableJailingNoisyUserSubscriptions = "NotificationJob.EnableJailingNoisyUserSubscriptions";
    internal const string EnableJailingNoisyServiceHooksSubscriptions = "NotificationJob.EnableJailingNoisyServiceHooksSubscriptions";
    internal const string SkipSubscriptionAdapters = "Notifications.SkipSubscriptionAdapters";
    internal const string SerializeFilterModel = "Notifications.SerializeFilterModel";
    public const string EnableFormattedXmlSerialization = "Notifications.EnableFormattedXmlSerialization";
    internal const string DisablePlainTextEmail = "NotificationJob.DisablePlainTextEmail";
    internal const string UseRegexForMatch = "NotificationJob.UseRegexForMatch";
    internal const string DontPersistResultDetailsOnSuccess = "Notifications.LocalDelivery.DontPersistResultDetailsOnSuccess";
    internal const string AzureServiceBusChannel = "Notifications.Channel.AzureServiceBus";
    public const string AllowUserRegexInMatchConditionFeatureFlag = "VisualStudio.Services.Notifications.AllowUserRegexInMatchCondition";
    public const string EnableSubscriptionEvaluationFlag = "VisualStudio.Services.Notifications.EnableSubscriptionEvaluationFlag";
    public const string EnableNewFilterOut = "Notifications.EnableNewFilterOut";
    public const string ThirdPartyEventPublishing = "Notifications.ThirdPartyEventPublishing";
    internal const string UseEmailTemplatesV2 = "Notifications.UseEmailTemplatesV2";
    public const string EnableEventPublishStats = "Notifications.EnableEventPublishStats";
    public const string EnableFallbackIsMemberCheckForInactiveIdentities = "Notifications.EnableFallbackIsMemberCheckForInactiveIdentities";
    public const string EnableAllowSubscriptionForInactiveIdentities = "Notifications.EnableAllowSubscriptionForInactiveIdentities";
    public const string DisableNotificationsForLogicallyDeletedAccounts = "Notifications.DisableNotificationsForLogicallyDeletedAccounts";
    public const string EnableAdditionalTracingForServiceHooksDeliveryJob = "Notifications.EnableAdditionalTracingForServiceHooksDeliveryJob";
    public const string EnableCachingIdentityIsTeamProperties = "Notifications.EnableCachingIdentityIsTeamProperties";
    public const string EnableServiceHooksDeliveryJobParallelism = "Notifications.EnableServiceHooksDeliveryJobParallelism";
  }
}
