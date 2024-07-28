// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionMigrationExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class OfferSubscriptionMigrationExtensions
  {
    internal static SubscriptionResourceUsage ToSubscriptionResourceUsage(
      this OfferSubscriptionInternal offerSubscription,
      Guid updatedByIdentifier)
    {
      SubscriptionResourceUsage subscriptionResourceUsage = new SubscriptionResourceUsage()
      {
        CommittedQuantity = offerSubscription.CommittedQuantity,
        CurrentQuantity = offerSubscription.CurrentQuantity,
        IncludedQuantity = offerSubscription.IncludedQuantity,
        IsPaidBillingEnabled = offerSubscription.IsPaidBillingEnabled,
        IsTrialOrPreview = offerSubscription.IsTrialOrPreview,
        LastResetDate = offerSubscription.LastResetDate,
        MaxQuantity = offerSubscription.MaximumQuantity,
        ResourceId = offerSubscription.MeterId,
        ResourceSeq = (byte) offerSubscription.RenewalGroup,
        StartDate = offerSubscription.StartDate ?? DateTime.MinValue,
        PaidBillingUpdated = offerSubscription.PaidBillingUpdatedDate,
        LastUpdated = DateTime.UtcNow,
        LastUpdatedBy = updatedByIdentifier,
        TrialDays = offerSubscription.TrialDays
      };
      try
      {
        subscriptionResourceUsage.AutoAssignOnAccess = offerSubscription.AutoAssignOnAccess;
      }
      catch (InvalidOperationException ex)
      {
      }
      return subscriptionResourceUsage;
    }
  }
}
