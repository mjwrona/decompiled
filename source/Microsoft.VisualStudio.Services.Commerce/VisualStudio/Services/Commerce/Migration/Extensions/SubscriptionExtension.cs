// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Migration.Extensions.SubscriptionExtension
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce.Migration.Extensions
{
  internal static class SubscriptionExtension
  {
    public static Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureSubscriptionInternal ToMigrationSubscriptionInternal(
      this Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInternal subscriptionInternal)
    {
      Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureSubscriptionInternal subscriptionInternal1 = (Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureSubscriptionInternal) null;
      if (subscriptionInternal != null)
        subscriptionInternal1 = new Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureSubscriptionInternal()
        {
          AzureOfferCode = subscriptionInternal.AzureOfferCode,
          AzureSubscriptionId = subscriptionInternal.AzureSubscriptionId,
          AzureSubscriptionTenantId = subscriptionInternal.AzureSubscriptionTenantId,
          AzureSubscriptionSource = (Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionSource) subscriptionInternal.AzureSubscriptionSource,
          AzureSubscriptionStatusId = (Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionStatus) subscriptionInternal.AzureSubscriptionStatusId,
          Created = subscriptionInternal.Created,
          LastUpdated = subscriptionInternal.LastUpdated
        };
      return subscriptionInternal1;
    }
  }
}
