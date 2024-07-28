// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ExtensionResourceControllerInternalBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public abstract class ExtensionResourceControllerInternalBase : CsmControllerBase
  {
    protected const string PlanDataPropertyKey = "plan";
    protected const string RootResourceType = "account";
    protected const string ResourceType = "extension";

    internal override string Layer => nameof (ExtensionResourceControllerInternalBase);

    internal ExtensionResourceControllerInternalBase()
    {
    }

    internal ExtensionResourceControllerInternalBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    protected internal override bool ExemptFromGlobalExceptionFormatting => true;

    internal AzureResourceAccount GetAzureResourceAccount(
      IVssRequestContext requestContext,
      ExtensionResourceRequestInternal request)
    {
      PlatformSubscriptionService service = requestContext.GetService<PlatformSubscriptionService>();
      return (service.GetAzureResourceAccount(requestContext, request.SubscriptionId, AccountProviderNamespace.VisualStudioOnline, request.RootResourceName) ?? service.GetAzureResourceAccount(requestContext, request.SubscriptionId, AccountProviderNamespace.OnPremise, request.RootResourceName)) ?? throw new AzureResourceAccountDoesNotExistException(request.RootResourceName);
    }

    internal ExtensionResource SetResourceUsage(
      IVssRequestContext requestContext,
      ExtensionResourceRequestInternal request,
      bool isCancel = false)
    {
      this.TfsRequestContext.Trace(5107301, TraceLevel.Verbose, this.Area, this.Layer, string.Format("Set Resource Usage: {0}, User CUID: {1}", (object) request.GetResourcePath(), (object) this.TfsRequestContext.GetUserCuid()));
      AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(requestContext, request);
      string resourceName = request.ResourceName;
      OfferMeter offerMeter = (OfferMeter) this.TfsRequestContext.GetService<IOfferMeterService>().GetOfferMeter(this.TfsRequestContext, resourceName);
      if (offerMeter == (OfferMeter) null)
        throw new OfferMeterNotFound(resourceName);
      int quantity = 0;
      if (!isCancel)
      {
        if (request.Plan == null)
          throw new ArgumentException("Plan data can not be null");
        AzureOfferPlanDefinition offerPlanDefinition = offerMeter.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (x => x.Publisher == request.Plan.publisher && x.OfferId == request.Plan.product && x.PlanId == request.Plan.name));
        quantity = !(offerPlanDefinition == (AzureOfferPlanDefinition) null) ? offerPlanDefinition.Quantity : throw new ArgumentException("No registered plan could be found which matches the passed in plan details.");
      }
      CollectionHelper.WithCollectionContext(requestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
      {
        IResourceTaggingService service1 = collectionContext.GetService<IResourceTaggingService>();
        if (isCancel)
          service1.DeleteTags(collectionContext, request.ResourceName);
        else
          service1.SaveTags(collectionContext, request.ResourceName, request.Tags);
        this.TfsRequestContext.TraceConditionally(5107303, TraceLevel.Verbose, this.Area, this.Layer, (Func<string>) (() => string.Format("Feature flag state for cancellation without write is {0}, meter {1}, collection {2}", (object) collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableThirdPartyOrderCancellationWrite"), (object) offerMeter.GalleryId, (object) azureResourceAccount.AccountId)));
        if (isCancel && collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableThirdPartyOrderCancellationWrite") || collectionContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DisableThirdPartyOrderWrite"))
          return;
        PlatformOfferSubscriptionService service2 = collectionContext.GetService<PlatformOfferSubscriptionService>();
        this.TfsRequestContext.Trace(5107302, TraceLevel.Verbose, this.Area, this.Layer, string.Format("Create offer report usage for account {0}, meter {1}, quantity {2}", (object) azureResourceAccount.AccountId, (object) offerMeter.GalleryId, (object) quantity));
        collectionContext.Items.Add("Commerce.RequestSource", (object) "Marketplace");
        IVssRequestContext requestContext1 = collectionContext;
        Guid azureSubscriptionId = azureResourceAccount.AzureSubscriptionId;
        int committedQuantity = quantity;
        OfferMeter offerMeter1 = offerMeter;
        Guid reportUsageUserId = CommerceConstants.ReportUsageUserId;
        service2.CreateOfferReportUsage(requestContext1, azureSubscriptionId, ResourceRenewalGroup.Monthly, committedQuantity, offerMeter1, reportUsageUserId, false);
      }), method: nameof (SetResourceUsage));
      this.EmitExtensionPurchaseCustomerIntelligenceEvent(request);
      return this.CreateGetResponseBody(request.ResourceName, azureResourceAccount, request.Plan, request.Tags);
    }

    internal void CheckParameters(
      Guid subscriptionId,
      string resourceGroupName,
      string rootResourceType,
      string rootResourceName,
      string resourceType,
      string resourceName)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceGroupName, nameof (resourceGroupName));
      ArgumentUtility.CheckStringForNullOrEmpty(rootResourceType, nameof (rootResourceType));
      ArgumentUtility.CheckStringForNullOrEmpty(rootResourceName, nameof (rootResourceName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceType, nameof (resourceType));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
    }

    internal ExtensionResource CreateGetResponseBody(
      string galleryId,
      AzureResourceAccount azureResourceAccount,
      ExtensionResourcePlan planData,
      Dictionary<string, string> tags)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string id = string.Format("/subscriptions/{0}/resourceGroups/{1}/providers/{2}/account/{3}/extension/{4}", (object) azureResourceAccount.AzureSubscriptionId, (object) azureResourceAccount.AzureCloudServiceName, (object) "Microsoft.VisualStudio", (object) azureResourceAccount.AzureResourceName, (object) galleryId);
      string str = "Microsoft.VisualStudio/account/extension";
      string name = galleryId;
      string type = str;
      ExtensionResource getResponseBody = new ExtensionResource(id, name, type);
      getResponseBody.location = azureResourceAccount.AzureGeoRegion;
      getResponseBody.plan = planData;
      getResponseBody.tags = tags;
      getResponseBody.properties = dictionary;
      return getResponseBody;
    }

    internal ExtensionResourcePlan GetPlanFromOfferSubscription(IOfferSubscription offerSubscription)
    {
      if (offerSubscription == null)
        return (ExtensionResourcePlan) null;
      int quantity = offerSubscription.CommittedQuantity;
      OfferMeter offerMeter = offerSubscription.OfferMeter;
      AzureOfferPlanDefinition offerPlanDefinition1;
      if ((object) offerMeter == null)
      {
        offerPlanDefinition1 = (AzureOfferPlanDefinition) null;
      }
      else
      {
        // ISSUE: explicit non-virtual call
        IEnumerable<AzureOfferPlanDefinition> fixedQuantityPlans = __nonvirtual (offerMeter.FixedQuantityPlans);
        offerPlanDefinition1 = fixedQuantityPlans != null ? fixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (x => x.Quantity == quantity)) : (AzureOfferPlanDefinition) null;
      }
      AzureOfferPlanDefinition offerPlanDefinition2 = offerPlanDefinition1;
      if (offerPlanDefinition2 == (AzureOfferPlanDefinition) null)
        return (ExtensionResourcePlan) null;
      return new ExtensionResourcePlan()
      {
        publisher = offerPlanDefinition2.Publisher,
        product = offerPlanDefinition2.OfferId,
        name = offerPlanDefinition2.PlanId,
        version = offerPlanDefinition2.PlanVersion
      };
    }

    internal ExtensionResourcePlan GetPlanFromOfferMeter(
      IVssRequestContext collectionContext,
      Guid accountId,
      string offerMeterName)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      return this.GetPlanFromOfferSubscription(collectionContext.GetService<IOfferSubscriptionService>().GetOfferSubscription(collectionContext, offerMeterName, true));
    }

    internal void EmitExtensionPurchaseCustomerIntelligenceEvent(
      ExtensionResourceRequestInternal request)
    {
      try
      {
        this.TfsRequestContext.TraceEnter(5108726, this.Area, this.Layer, nameof (EmitExtensionPurchaseCustomerIntelligenceEvent));
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        IEnumerable<string> values1;
        if (this.Request.Headers.TryGetValues("x-ms-marketplace-part", out values1))
        {
          eventData.Add(CustomerIntelligenceProperty.PartNumber, values1.FirstOrDefault<string>());
        }
        else
        {
          eventData.Add(CustomerIntelligenceProperty.PartNumber, "Not present");
          this.TfsRequestContext.TraceConditionally(5108744, TraceLevel.Warning, this.Area, this.Layer, (Func<string>) (() => "Part number header (x-ms-marketplace-part) not present. Available headers: " + string.Join(", ", this.Request.Headers.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (x => x.Key)))));
        }
        IEnumerable<string> values2;
        if (this.Request.Headers.TryGetValues("x-ms-marketplace-order", out values2))
        {
          eventData.Add(CustomerIntelligenceProperty.OrderNumber, values2.FirstOrDefault<string>());
        }
        else
        {
          eventData.Add(CustomerIntelligenceProperty.OrderNumber, "Not present");
          this.TfsRequestContext.TraceConditionally(5108743, TraceLevel.Warning, this.Area, this.Layer, (Func<string>) (() => "Order number header (x-ms-marketplace-part) not present. Available headers: " + string.Join(", ", this.Request.Headers.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (x => x.Key)))));
        }
        eventData.Add(CustomerIntelligenceProperty.SubscriptionId, (object) request.SubscriptionId);
        eventData.Add(CustomerIntelligenceProperty.ResourceGroup, request.ResourceGroupName);
        eventData.Add(CustomerIntelligenceProperty.AccountResourceName, request.RootResourceName);
        eventData.Add(CustomerIntelligenceProperty.ExtensionResourceName, request.ResourceName);
        eventData.Add(CustomerIntelligenceProperty.ExtensionResourceHttpMethod, this.Request.Method.Method);
        if (request.Plan != null)
        {
          eventData.Add("Plan", request.Plan.name);
          eventData.Add("Product", request.Plan.product);
          eventData.Add("Publisher", request.Plan.publisher);
        }
        CustomerIntelligence.PublishEvent(this.TfsRequestContext, "ExtensionResourcePurchase", eventData);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(5108728, this.Area, this.Layer, ex);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(5108729, this.Area, this.Layer, nameof (EmitExtensionPurchaseCustomerIntelligenceEvent));
      }
    }
  }
}
