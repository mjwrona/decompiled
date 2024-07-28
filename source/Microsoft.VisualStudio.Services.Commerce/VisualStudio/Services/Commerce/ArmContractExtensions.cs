// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ArmContractExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class ArmContractExtensions
  {
    public static AzureSubscriptionInfo ToAzureSubscriptionInfo(
      this ArmAzureSubscriptionInfo armAzureSubscriptionInfo)
    {
      AzureSubscriptionInfo subscriptionInfo = new AzureSubscriptionInfo()
      {
        Id = armAzureSubscriptionInfo.Id,
        SubscriptionId = armAzureSubscriptionInfo.SubscriptionId,
        DisplayName = armAzureSubscriptionInfo.DisplayName,
        Status = ArmContractExtensions.MapSubscriptionStatus(armAzureSubscriptionInfo.State)
      };
      if (armAzureSubscriptionInfo.SubscriptionPolicies != null)
      {
        subscriptionInfo.LocationPlacementId = armAzureSubscriptionInfo.SubscriptionPolicies.LocationPlacementId;
        subscriptionInfo.QuotaId = armAzureSubscriptionInfo.SubscriptionPolicies.QuotaId;
        AzureSpendingLimit result;
        Enum.TryParse<AzureSpendingLimit>(armAzureSubscriptionInfo.SubscriptionPolicies.SpendingLimit, out result);
        subscriptionInfo.SpendingLimit = result;
      }
      subscriptionInfo.TenantId = armAzureSubscriptionInfo.TenantId;
      return subscriptionInfo;
    }

    private static SubscriptionStatus MapSubscriptionStatus(string status)
    {
      switch (status?.ToLower())
      {
        case "active":
        case "enabled":
          return SubscriptionStatus.Active;
        case "deleted":
          return SubscriptionStatus.Deleted;
        case "other":
          return SubscriptionStatus.Unknown;
        case "suspended":
        case "disabled":
          return SubscriptionStatus.Disabled;
        default:
          return SubscriptionStatus.Unknown;
      }
    }
  }
}
