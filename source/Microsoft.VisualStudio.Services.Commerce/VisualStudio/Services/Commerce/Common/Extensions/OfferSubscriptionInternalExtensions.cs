// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Extensions.OfferSubscriptionInternalExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Extensions
{
  internal static class OfferSubscriptionInternalExtensions
  {
    internal static void WriteOfferSubscriptionCIData(
      this OfferSubscriptionInternal offerSubscription,
      IVssRequestContext collectionContext,
      Guid subscriptionId,
      SubscriptionStatus subscriptionStatus)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("SubscriptionId", (object) subscriptionId);
      eventData.Add("SubscriptionStatus", subscriptionStatus.ToString());
      eventData.Add("MeterName", offerSubscription.Meter.Name);
      eventData.Add("CommittedQuantity", (double) offerSubscription.CommittedQuantity);
      eventData.Add("CurrentQuantity", (double) offerSubscription.CurrentQuantity);
      eventData.Add("IncludedQuantity", (double) offerSubscription.IncludedQuantity);
      CustomerIntelligence.PublishEvent(collectionContext, "MeterResetEvent", eventData);
    }

    internal static void WriteCloudLoadTestResetCIData(
      this OfferSubscriptionInternal offerSubscription,
      IVssRequestContext collectionContext)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("MeterName", offerSubscription.Meter.Name);
      eventData.Add("CommittedQuantity", (double) offerSubscription.CommittedQuantity);
      eventData.Add("CurrentQuantity", (double) offerSubscription.CurrentQuantity);
      eventData.Add("IncludedQuantity", (double) offerSubscription.IncludedQuantity);
      CustomerIntelligence.PublishEvent(collectionContext, "CloudLoadTestResetEvent", eventData);
    }
  }
}
