// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Extensions.OfferSubscriptionExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Extensions
{
  internal static class OfferSubscriptionExtensions
  {
    internal static bool IsRenewingNextMonth(
      this IOfferSubscription offerSubscription,
      DateTime queueTime)
    {
      DateTime resetDate = offerSubscription.ResetDate;
      return queueTime.Month == 12 ? resetDate.Month == 1 && queueTime.AddYears(1).Year == resetDate.Year : queueTime.AddMonths(1).Month == resetDate.Month && queueTime.Year == resetDate.Year;
    }
  }
}
