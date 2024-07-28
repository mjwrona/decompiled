// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Remote.Billing.BillingClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Remote.Billing
{
  internal class BillingClient : IBillingClient
  {
    private readonly BillingHttpClient _billingHttpClient;
    private readonly GalleryKPIHelpers _kpiHelpers;

    public BillingClient(BillingHttpClient billingClient, GalleryKPIHelpers kpiHelpers)
    {
      this._billingHttpClient = billingClient;
      this._kpiHelpers = kpiHelpers;
    }

    public void CreateOfferSubscriptionSync(
      IVssRequestContext requestContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      Guid? billingTarget)
    {
      this.ExecuteAction(requestContext, (Action) (() => this._billingHttpClient.CreateOfferSubscription(offerMeterName, azureSubscriptionId, renewalGroup, quantity, billingTarget).SyncResult()));
    }

    public void CancelOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      Guid? billingTarget)
    {
      this.ExecuteAction(requestContext, (Action) (() => this._billingHttpClient.CancelOfferSubscription(offerMeterName, azureSubscriptionId, renewalGroup, "Canceled by selecting 0 quantity in Visual Studio Marketplace", billingTarget).SyncResult()));
    }

    public void EnableTrialOrPreview(IVssRequestContext requestContext, string offerMeterName) => this.ExecuteAction(requestContext, (Action) (() => this._billingHttpClient.EnableTrialOrPreviewOfferSubscription(offerMeterName, ResourceRenewalGroup.Monthly).SyncResult()));

    public IOfferSubscription GetOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      bool nextBillingPeriod = false)
    {
      return this.ExecuteAction<IOfferSubscription>(requestContext, (Func<IOfferSubscription>) (() => this._billingHttpClient.GetResourceUsage(offerMeterName, nextBillingPeriod).SyncResult<IOfferSubscription>()));
    }

    private void ExecuteAction(IVssRequestContext requestContext, Action action)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        this.OnS2SException(requestContext, ex);
        throw;
      }
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessBilling");
    }

    private T ExecuteAction<T>(IVssRequestContext requestContext, Func<T> action)
    {
      T obj1 = default (T);
      T obj2;
      try
      {
        obj2 = action();
      }
      catch (Exception ex)
      {
        this.OnS2SException(requestContext, ex);
        throw;
      }
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessBilling");
      return obj2;
    }

    private void OnS2SException(IVssRequestContext requestContext, Exception e)
    {
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SFailureBilling");
      requestContext.TraceException(12060031, "GalleryKPIHelpers", "Service", e);
    }
  }
}
