// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Migration.Extensions.ResourceUsageExtension
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce.Migration.Extensions
{
  internal static class ResourceUsageExtension
  {
    public static IEnumerable<Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionResourceUsage> ToResourceUsages(
      this IEnumerable<Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage> resourceUsages)
    {
      IList<Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionResourceUsage> resourceUsages1 = (IList<Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionResourceUsage>) new List<Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionResourceUsage>();
      if (!resourceUsages.IsNullOrEmpty<Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage>())
      {
        foreach (Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage resourceUsage in resourceUsages)
        {
          Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionResourceUsage subscriptionResourceUsage = new Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionResourceUsage()
          {
            AutoAssignOnAccess = resourceUsage.AutoAssignOnAccess,
            CommittedQuantity = resourceUsage.CommittedQuantity,
            CurrentQuantity = resourceUsage.CurrentQuantity,
            IncludedQuantity = resourceUsage.IncludedQuantity,
            IsPaidBillingEnabled = resourceUsage.IsPaidBillingEnabled,
            IsTrialOrPreview = resourceUsage.IsTrialOrPreview,
            LastResetDate = resourceUsage.LastResetDate,
            LastUpdated = resourceUsage.LastUpdated,
            LastUpdatedBy = resourceUsage.LastUpdatedBy,
            MaxQuantity = resourceUsage.MaxQuantity,
            PaidBillingUpdated = resourceUsage.PaidBillingUpdated,
            ResourceId = resourceUsage.ResourceId,
            ResourceSeq = resourceUsage.ResourceSeq,
            StartDate = resourceUsage.StartDate,
            TrialDays = resourceUsage.TrialDays
          };
          resourceUsages1.Add(subscriptionResourceUsage);
        }
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionResourceUsage>) resourceUsages1;
    }
  }
}
