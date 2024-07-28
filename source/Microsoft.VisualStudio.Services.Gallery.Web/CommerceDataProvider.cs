// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.CommerceDataProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Remote;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class CommerceDataProvider : ICommerceDataProvider
  {
    private static readonly string s_area = "Gallery";
    private static readonly string s_layer = nameof (CommerceDataProvider);

    public IOfferMeter GetOfferDetails(
      IVssRequestContext requestContext,
      string fullyQualifiedExtensionName)
    {
      try
      {
        return requestContext.GetService<IOfferMeterService>().GetOfferMeter(requestContext, fullyQualifiedExtensionName);
      }
      catch (ArgumentException ex)
      {
        return (IOfferMeter) null;
      }
    }

    public IEnumerable<IOfferMeterPrice> GetOfferMeterPrice(
      IVssRequestContext requestContext,
      string fullyQualifiedExtensionName)
    {
      try
      {
        return requestContext.GetService<IOfferMeterService>().GetOfferMeterPrice(requestContext, fullyQualifiedExtensionName);
      }
      catch (ArgumentException ex)
      {
        requestContext.TraceException(12061128, CommerceDataProvider.s_area, CommerceDataProvider.s_layer, (Exception) ex);
        return (IEnumerable<IOfferMeterPrice>) null;
      }
      catch (VssServiceResponseException ex)
      {
        requestContext.TraceException(12061128, CommerceDataProvider.s_area, CommerceDataProvider.s_layer, (Exception) ex);
        return (IEnumerable<IOfferMeterPrice>) null;
      }
    }

    public ISubscriptionAccount GetSubscriptionAccount(
      IVssRequestContext requestContext,
      Guid accountId,
      AccountProviderNamespace providerNamespace = AccountProviderNamespace.VisualStudioOnline)
    {
      ISubscriptionService service = requestContext.GetService<ISubscriptionService>();
      ISubscriptionAccount subscriptionAccount = (ISubscriptionAccount) null;
      try
      {
        subscriptionAccount = service.GetSubscriptionAccount(requestContext, providerNamespace, accountId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061056, CommerceDataProvider.s_area, CommerceDataProvider.s_layer, ex);
        if (ex.InnerException != null)
        {
          if (ex.InnerException is ArgumentException)
            goto label_5;
        }
        throw;
      }
label_5:
      return subscriptionAccount;
    }

    public IOfferSubscription GetOfferSubscriptionDetails(
      IVssRequestContext requestContext,
      string fullyQualifiedExtensionName,
      Guid accountId,
      IRemoteServiceClientFactory clientFactory,
      bool nextBillingPeriod = false)
    {
      IOfferSubscription offerSubscription = (IOfferSubscription) null;
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => offerSubscription = (IOfferSubscription) clientFactory.GetNewNonDeploymentLevelCommerceClient(requestContext, accountId).GetOfferSubscriptionAsync(fullyQualifiedExtensionName, new bool?(nextBillingPeriod)).SyncResult<OfferSubscription>()), (Action) (() => offerSubscription = clientFactory.GetNewNonDeploymentLevelBillingClient(requestContext, accountId).GetOfferSubscription(requestContext, fullyQualifiedExtensionName, nextBillingPeriod)), CommerceDataProvider.s_area, CommerceDataProvider.s_layer, 12061057, requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Commerce.CommerceServiceRoutingEnabled"));
      }
      catch (VssServiceException ex)
      {
        requestContext.TraceException(12061057, CommerceDataProvider.s_area, CommerceDataProvider.s_layer, (Exception) ex);
        if (ex.InnerException != null)
        {
          if (ex.InnerException is ArgumentException)
            goto label_5;
        }
        throw;
      }
label_5:
      return offerSubscription;
    }
  }
}
