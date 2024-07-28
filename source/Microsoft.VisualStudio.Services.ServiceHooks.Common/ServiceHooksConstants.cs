// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ServiceHooksConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  [GenerateAllConstants(null)]
  public static class ServiceHooksConstants
  {
    public const string ServiceName = "ServiceHooks";
    public const string DataspaceCategoryName = "ServiceHooks";
    public const string ContributionTargetForConsumers = "ms.vss-servicehooks.consumers";
    public const string ContributionTypeNameConsumer = "ms.vss-servicehooks.consumer";
    public const string ContributionTypeNameConsumerPlugin = "ms.vss-servicehooks.consumer-plugin";

    public static class FeatureFlags
    {
      public const string ValidatePublisherInputs = "ServiceHooks.Subscription.ValidatePublisherInputs";
      public const string SkipOverrideServiceIdentitiesStepForQuerySubscriptions = "ServiceHooks.Subscriptions.SkipOverrideServiceIdentitiesStepForQuerySubscriptions";
      public const string DoNotUseMultipleServiceConnections = "ServiceHooks.Subscriptions.DoNotUseMultipleServiceConnections";
      public const string UseApplyIPAddressAllowedRangeOnHttpRequest = "ServiceHooks.Http.UseApplyIPAddressAllowedRangeOnHttpRequest";
      public const string IPAddressOnHttpRequestPreferIPv4 = "ServiceHooks.Http.IPAddressOnHttpRequestPreferIPv4";
      public const string DisableRedirects = "ServiceHooks.Notification.DisableRedirects";
    }
  }
}
