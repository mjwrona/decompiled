// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.OfferSubscriptionManipulator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce.Common
{
  internal class OfferSubscriptionManipulator
  {
    public void ManipulateOfferSubscription(
      IVssRequestContext collectionContext,
      OfferSubscription subscriptionResource,
      AzureSubscriptionInternal azureSubscription)
    {
      collectionContext.TraceAlways(5109315, TraceLevel.Info, "Commerce", nameof (OfferSubscriptionManipulator), new
      {
        Msg = "Manipulate offersubscription start",
        CommittedQuantity = subscriptionResource?.CommittedQuantity,
        IncludedQuantity = subscriptionResource?.IncludedQuantity,
        MaximumQuantity = subscriptionResource?.MaximumQuantity,
        AzureSubscriptionId = azureSubscription?.AzureSubscriptionId,
        AzureSubscriptionStatusId = azureSubscription?.AzureSubscriptionStatusId
      }.Serialize());
      if (this.IsManipulationRequired(collectionContext, subscriptionResource, azureSubscription))
      {
        subscriptionResource.CommittedQuantity = 100000;
        subscriptionResource.MaximumQuantity = 100000;
      }
      collectionContext.TraceAlways(5109316, TraceLevel.Info, "Commerce", nameof (OfferSubscriptionManipulator), new
      {
        Msg = "Manipulate offersubscription end",
        CommittedQuantity = subscriptionResource?.CommittedQuantity,
        IncludedQuantity = subscriptionResource?.IncludedQuantity,
        MaximumQuantity = subscriptionResource?.MaximumQuantity,
        AzureSubscriptionId = azureSubscription?.AzureSubscriptionId,
        AzureSubscriptionStatusId = azureSubscription?.AzureSubscriptionStatusId
      }.Serialize());
    }

    private bool IsManipulationRequired(
      IVssRequestContext collectionContext,
      OfferSubscription subscriptionResource,
      AzureSubscriptionInternal azureSubscription)
    {
      if (collectionContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableAssignmentBasedBilling") && azureSubscription != null && azureSubscription.AzureSubscriptionStatusId == SubscriptionStatus.Active)
      {
        Guid? azureSubscriptionId = azureSubscription?.AzureSubscriptionId;
        Guid empty = Guid.Empty;
        if ((azureSubscriptionId.HasValue ? (azureSubscriptionId.HasValue ? (azureSubscriptionId.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
        {
          int? meterId = subscriptionResource?.OfferMeter.MeterId;
          int basicMeterId = OfferSubscriptionManipulator.MetersResources.BasicMeterId;
          if (meterId.GetValueOrDefault() == basicMeterId & meterId.HasValue)
            return true;
          meterId = subscriptionResource?.OfferMeter.MeterId;
          int testManagerMeterId = OfferSubscriptionManipulator.MetersResources.TestManagerMeterId;
          return meterId.GetValueOrDefault() == testManagerMeterId & meterId.HasValue;
        }
      }
      return false;
    }

    private static class MetersResources
    {
      public static int BasicMeterId = 1;
      public static int TestManagerMeterId = 9;
    }
  }
}
