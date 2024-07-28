// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ExtensionResourceGroupControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [CaptureExternalOperationIdHeader("x-ms-correlation-request-id")]
  [SetCsmV2ResponseHeaders]
  public abstract class ExtensionResourceGroupControllerBase : 
    ExtensionResourceControllerInternalBase
  {
    public ExtensionResourceGroupControllerBase()
    {
    }

    internal override string Layer => nameof (ExtensionResourceGroupControllerBase);

    internal ExtensionResourceGroupControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    [HttpGet]
    [TraceDetailsFilter(5107330, 5107339)]
    [CsmControllerExceptionHandler(5107338)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The response contains all extension resource details for the specified account.", false)]
    public virtual ExtensionResourceListResult Extensions_ListByAccount(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName)
    {
      AzureResourceAccount azureResourceAccount = this.TfsRequestContext.GetService<PlatformSubscriptionService>().GetAzureResourceAccount(this.TfsRequestContext, subscriptionId, AccountProviderNamespace.VisualStudioOnline, accountResourceName);
      if (azureResourceAccount == null)
        throw new AzureResourceAccountDoesNotExistException(accountResourceName);
      List<ExtensionResource> returnCollection = new List<ExtensionResource>();
      CollectionHelper.WithCollectionContext(this.TfsRequestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
      {
        IEnumerable<IOfferSubscription> offerSubscriptions = collectionContext.GetService<PlatformOfferSubscriptionService>().GetOfferSubscriptions(collectionContext, true);
        IResourceTaggingService taggingService = collectionContext.GetService<IResourceTaggingService>();
        returnCollection.AddRange(offerSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (offerSubscription =>
        {
          if (offerSubscription != null)
          {
            BillingProvider? billingEntity = offerSubscription.OfferMeter?.BillingEntity;
            BillingProvider billingProvider = BillingProvider.AzureStoreManaged;
            if (billingEntity.GetValueOrDefault() == billingProvider & billingEntity.HasValue)
              return offerSubscription.CommittedQuantity != 0;
          }
          return false;
        })).Select<IOfferSubscription, ExtensionResource>((Func<IOfferSubscription, ExtensionResource>) (subscription => this.CreateGetResponseBody(subscription.OfferMeter.GalleryId, azureResourceAccount, this.GetPlanFromOfferSubscription(subscription), taggingService.GetTags(collectionContext, subscription.OfferMeter.GalleryId)))));
      }), method: nameof (Extensions_ListByAccount));
      return new ExtensionResourceListResult()
      {
        Value = returnCollection.ToArray()
      };
    }
  }
}
