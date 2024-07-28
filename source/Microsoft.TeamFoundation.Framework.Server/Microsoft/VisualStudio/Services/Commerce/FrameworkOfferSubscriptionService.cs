// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.FrameworkOfferSubscriptionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkOfferSubscriptionService : IOfferSubscriptionService, IVssFrameworkService
  {
    private const string s_area = "Commerce";
    private const string s_layer = "FrameworkOfferSubscriptionService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    internal virtual BillingHttpClient GetOfferSubscriptionHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<BillingHttpClient>();
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.Client.MeteringHttpClient GetMeteringHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.Client.MeteringHttpClient>();
    }

    internal virtual OfferSubscriptionHttpClient GetCommerceOfferSubscriptionHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<OfferSubscriptionHttpClient>();
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.WebApi.MeteringHttpClient GetCommerceMeteringHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.MeteringHttpClient>();
    }

    internal virtual PurchaseRequestHttpClient GetCommercePurchaseRequestHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<PurchaseRequestHttpClient>();
    }

    public IOfferSubscription GetOfferSubscription(
      IVssRequestContext requestContext,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      bool nextBillingPeriod = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5104005, "Commerce", nameof (FrameworkOfferSubscriptionService), "GetResourceStatus");
      try
      {
        requestContext.Trace(5104006, TraceLevel.Info, "Commerce", nameof (FrameworkOfferSubscriptionService), string.Format("Getting resource status for {0} and {1} {2}", (object) requestContext.ServiceHost.InstanceId, (object) resourceName, (object) renewalGroup));
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IOfferSubscription>(requestContext, (Func<IOfferSubscription>) (() => (IOfferSubscription) this.GetCommerceOfferSubscriptionHttpClient(requestContext).GetOfferSubscriptionForRenewalGroupAsync(resourceName, renewalGroup, new bool?(nextBillingPeriod)).SyncResult<OfferSubscription>()), (Func<IOfferSubscription>) (() => this.GetOfferSubscriptionHttpClient(requestContext).GetResourceUsage(resourceName, renewalGroup, nextBillingPeriod).SyncResult<IOfferSubscription>()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104006, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104007, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        switch (ex)
        {
          case VssServiceResponseException _:
          case HttpRequestException _:
          case WebException _:
          case TaskCanceledException _:
            requestContext.Trace(5104008, TraceLevel.Info, "Commerce", nameof (FrameworkOfferSubscriptionService), "Returning default value because of unhandled exception");
            return (IOfferSubscription) new OfferSubscription()
            {
              IsUseable = true
            };
          default:
            throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(5104009, "Commerce", nameof (FrameworkOfferSubscriptionService), "GetResourceStatus");
      }
    }

    public IOfferSubscription GetOfferSubscription(
      IVssRequestContext requestContext,
      string resourceName,
      bool nextBillingPeriod = false)
    {
      return this.GetOfferSubscription(requestContext, resourceName, ResourceRenewalGroup.Monthly, nextBillingPeriod);
    }

    public IEnumerable<IOfferSubscription> GetOfferSubscriptions(
      IVssRequestContext requestContext,
      bool nextBillingPeriod = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5104001, "Commerce", nameof (FrameworkOfferSubscriptionService), "GetResourceStatus");
      try
      {
        requestContext.Trace(5104002, TraceLevel.Info, "Commerce", nameof (FrameworkOfferSubscriptionService), "Getting all resources status for {0}", (object) requestContext.ServiceHost.InstanceId);
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<IOfferSubscription>>(requestContext, (Func<IEnumerable<IOfferSubscription>>) (() => (IEnumerable<IOfferSubscription>) this.GetCommerceOfferSubscriptionHttpClient(requestContext).GetOfferSubscriptionsAsync(new bool?(nextBillingPeriod)).SyncResult<List<OfferSubscription>>()), (Func<IEnumerable<IOfferSubscription>>) (() => this.GetOfferSubscriptionHttpClient(requestContext).GetResourceUsage(nextBillingPeriod).SyncResult<IEnumerable<IOfferSubscription>>()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104002, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104027, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        switch (ex)
        {
          case VssServiceResponseException _:
          case HttpRequestException _:
          case WebException _:
          case TaskCanceledException _:
            requestContext.Trace(5104003, TraceLevel.Info, "Commerce", nameof (FrameworkOfferSubscriptionService), "Returning default value because of unhandled exception");
            return (IEnumerable<IOfferSubscription>) new OfferSubscription[1]
            {
              new OfferSubscription() { IsUseable = true }
            };
          default:
            throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(5104004, "Commerce", nameof (FrameworkOfferSubscriptionService), "GetResourceStatus");
      }
    }

    public IEnumerable<IOfferSubscription> GetOfferSubscriptionsForAllAzureSubscription(
      IVssRequestContext requestContext,
      bool validateAzuresubscription = false,
      bool nextBillingPeriod = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (CommerceDeploymentHelper.IsHostTypeCheckEnabled(requestContext) && requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5104023, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (GetOfferSubscriptionsForAllAzureSubscription));
      try
      {
        requestContext.Trace(5104024, TraceLevel.Info, "Commerce", nameof (FrameworkOfferSubscriptionService), "Getting offer subscriptions for user ", (object) true);
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<IOfferSubscription>>(requestContext, (Func<IEnumerable<IOfferSubscription>>) (() => (IEnumerable<IOfferSubscription>) this.GetCommerceOfferSubscriptionHttpClient(requestContext).GetAllOfferSubscriptionsForUserAsync(validateAzuresubscription, nextBillingPeriod).SyncResult<List<OfferSubscription>>()), (Func<IEnumerable<IOfferSubscription>>) (() => this.GetOfferSubscriptionHttpClient(requestContext).GetAllOfferSubscriptionsForUser(validateAzuresubscription, nextBillingPeriod).SyncResult<IEnumerable<IOfferSubscription>>()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104024, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104025, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104026, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (GetOfferSubscriptionsForAllAzureSubscription));
      }
    }

    public AccountLicenseType GetDefaultLicenseLevel(
      IVssRequestContext requestContext,
      Guid organizationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
      requestContext.TraceEnter(5104037, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (GetDefaultLicenseLevel));
      try
      {
        requestContext.Trace(5104038, TraceLevel.Info, "Commerce", nameof (FrameworkOfferSubscriptionService), "Getting default license level ", (object) true);
        return this.GetCommerceOfferSubscriptionHttpClient(requestContext).GetDefaultLicenseLevelAsync(organizationId).SyncResult<AccountLicenseType>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104039, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104040, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (GetDefaultLicenseLevel));
      }
    }

    public IEnumerable<IOfferSubscription> GetOfferSubscriptionsForGalleryItem(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      string galleryId = null,
      bool nextBillingPeriod = false)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceEnter(5104027, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (GetOfferSubscriptionsForGalleryItem));
      try
      {
        requestContext.Trace(5104028, TraceLevel.Info, "Commerce", nameof (FrameworkOfferSubscriptionService), "Getting offer subscription for {0} and {1}", (object) azureSubscriptionId.ToString(), (object) galleryId, (object) true);
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<IOfferSubscription>>(requestContext, (Func<IEnumerable<IOfferSubscription>>) (() => (IEnumerable<IOfferSubscription>) this.GetCommerceOfferSubscriptionHttpClient(requestContext).GetOfferSubscriptionsForGalleryItemAsync(galleryId, azureSubscriptionId, new bool?(nextBillingPeriod)).SyncResult<List<OfferSubscription>>()), (Func<IEnumerable<IOfferSubscription>>) (() => this.GetOfferSubscriptionHttpClient(requestContext).GetOfferSubscriptionsForGalleryItem(azureSubscriptionId, galleryId, nextBillingPeriod).SyncResult<IEnumerable<IOfferSubscription>>()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104028, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104029, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104030, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (GetOfferSubscriptionsForGalleryItem));
      }
    }

    public void CreateOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      Guid? billingTarget)
    {
      ArgumentUtility.CheckForOutOfRange(quantity, nameof (quantity), 1);
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
      requestContext.TraceEnter(5104010, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (CreateOfferSubscription));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() =>
        {
          OfferSubscription offerSubscription1 = new OfferSubscription()
          {
            OfferMeter = new OfferMeter()
            {
              GalleryId = offerMeterName
            },
            AzureSubscriptionId = azureSubscriptionId,
            RenewalGroup = renewalGroup,
            CommittedQuantity = quantity
          };
          OfferSubscriptionHttpClient subscriptionHttpClient = this.GetCommerceOfferSubscriptionHttpClient(requestContext);
          OfferSubscription offerSubscription2 = offerSubscription1;
          Guid? nullable = billingTarget;
          Guid? tenantId = new Guid?();
          Guid? objectId = new Guid?();
          Guid? billingTarget1 = nullable;
          bool? skipSubscriptionValidation = new bool?();
          CancellationToken cancellationToken = new CancellationToken();
          subscriptionHttpClient.CreateOfferSubscriptionAsync(offerSubscription2, tenantId: tenantId, objectId: objectId, billingTarget: billingTarget1, skipSubscriptionValidation: skipSubscriptionValidation, cancellationToken: cancellationToken).Wait();
        }), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).CreateOfferSubscription(offerMeterName, azureSubscriptionId, renewalGroup, quantity, billingTarget).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104010, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, billingTarget.GetValueOrDefault(), new Guid?(azureSubscriptionId), nameof (FrameworkOfferSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104011, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104012, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (CreateOfferSubscription));
      }
    }

    public double ReportUsage(
      IVssRequestContext requestContext,
      Guid eventUserId,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string eventId,
      DateTime billingEventDateTime,
      bool immediate = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(quantity, nameof (quantity), 1);
      ArgumentUtility.CheckStringForNullOrEmpty(eventId, nameof (eventId));
      ArgumentUtility.CheckForEmptyGuid(eventUserId, nameof (eventUserId));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5104040, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (ReportUsage));
      ResourceName knownResourceName;
      if (!System.Enum.TryParse<ResourceName>(resourceName, out knownResourceName))
        throw new ArgumentException(nameof (resourceName));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => Microsoft.VisualStudio.Services.WebApi.TaskExtensions.SyncResult(this.GetCommerceMeteringHttpClient(requestContext).ReportUsage(eventUserId, knownResourceName, quantity, eventId, billingEventDateTime))), (Action) (() => this.GetMeteringHttpClient(requestContext).ReportUsage(eventUserId, knownResourceName, quantity, eventId, billingEventDateTime).SyncResult()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104040, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
        return 1.0;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104041, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104042, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (ReportUsage));
      }
    }

    public IEnumerable<IUsageEventAggregate> GetUsage(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan,
      ResourceName resource)
    {
      throw new NotImplementedException();
    }

    public void CancelOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      string cancelReason,
      Guid? billingTarget,
      bool immediate = false,
      Guid? tenantId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
      requestContext.TraceEnter(5104034, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (CancelOfferSubscription));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() =>
        {
          OfferSubscription offerSubscription = new OfferSubscription()
          {
            OfferMeter = new OfferMeter()
            {
              GalleryId = offerMeterName
            },
            AzureSubscriptionId = azureSubscriptionId,
            RenewalGroup = renewalGroup
          };
          this.GetCommerceOfferSubscriptionHttpClient(requestContext).CancelOfferSubscriptionAsync(offerSubscription, cancelReason, billingTarget.GetValueOrDefault(), new bool?(immediate)).Wait();
        }), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).CancelOfferSubscription(offerMeterName, azureSubscriptionId, renewalGroup, cancelReason, billingTarget, immediate).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104034, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, billingTarget.GetValueOrDefault(), new Guid?(azureSubscriptionId), nameof (FrameworkOfferSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104035, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104036, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (CancelOfferSubscription));
      }
    }

    public void TogglePaidBilling(
      IVssRequestContext requestContext,
      string resourceName,
      bool paidBillingState)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5104013, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (TogglePaidBilling));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() =>
        {
          OfferSubscriptionHttpClient subscriptionHttpClient = this.GetCommerceOfferSubscriptionHttpClient(requestContext);
          OfferSubscription offerSubscription = subscriptionHttpClient.GetOfferSubscriptionAsync(resourceName, new bool?(true)).SyncResult<OfferSubscription>();
          offerSubscription.IsPaidBillingEnabled = paidBillingState;
          requestContext.TraceAlways(5104013, TraceLevel.Info, "Commerce", nameof (FrameworkOfferSubscriptionService), "Updating the paid billing mode in commerce service for the meter " + resourceName + "..");
          subscriptionHttpClient.UpdateOfferSubscriptionAsync(offerSubscription).Wait();
        }), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).TogglePaidBilling(resourceName, paidBillingState).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104013, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104014, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104015, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (TogglePaidBilling));
      }
    }

    public void SetAccountQuantity(
      IVssRequestContext requestContext,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      int? includedQuantity,
      int? maximumQuantity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5104016, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (SetAccountQuantity));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceOfferSubscriptionHttpClient(requestContext).SetAccountQuantityAsync(resourceName, renewalGroup, includedQuantity.GetValueOrDefault(), maximumQuantity.GetValueOrDefault()).Wait()), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).SetAccountQuantity(resourceName, renewalGroup, includedQuantity, maximumQuantity).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104016, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104017, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104018, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (SetAccountQuantity));
      }
    }

    public void SetAccountQuantity(
      IVssRequestContext requestContext,
      string resourceName,
      int? includedQuantity,
      int? maximumQuantity)
    {
      this.SetAccountQuantity(requestContext, resourceName, ResourceRenewalGroup.Monthly, includedQuantity, maximumQuantity);
    }

    public void DecreaseResourceQuantity(
      IVssRequestContext requestContext,
      Guid? azureSubscriptionId,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      bool shouldBeImmediate)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(offerMeterName, nameof (offerMeterName));
      if (quantity < 0)
        throw new ArgumentOutOfRangeException(nameof (quantity));
      requestContext.CheckHostedDeployment();
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        ArgumentUtility.CheckForEmptyGuid(azureSubscriptionId, nameof (azureSubscriptionId));
      else
        requestContext.CheckProjectCollectionRequestContext();
      requestContext.TraceEnter(5104104, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (DecreaseResourceQuantity));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceOfferSubscriptionHttpClient(requestContext).DecreaseResourceQuantityAsync(offerMeterName, renewalGroup, quantity, shouldBeImmediate, azureSubscriptionId.GetValueOrDefault()).Wait()), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).DecreaseResourceQuantity(offerMeterName, renewalGroup, quantity, shouldBeImmediate, azureSubscriptionId).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104104, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, requestContext.ServiceHost.InstanceId, azureSubscriptionId, nameof (FrameworkOfferSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104105, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104106, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (DecreaseResourceQuantity));
      }
    }

    public void EnableTrialOrPreview(
      IVssRequestContext requestContext,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(offerMeterName, nameof (offerMeterName));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5104031, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (EnableTrialOrPreview));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceOfferSubscriptionHttpClient(requestContext).EnableTrialOrPreviewOfferSubscriptionAsync(offerMeterName, renewalGroup).SyncResult()), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).EnableTrialOrPreviewOfferSubscription(offerMeterName, renewalGroup).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104031, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104032, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        if (ex is AggregateException)
          throw (ex as AggregateException).Flatten().InnerException;
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104033, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (EnableTrialOrPreview));
      }
    }

    public void ExtendTrialDate(
      IVssRequestContext requestContext,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      DateTime endDate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(offerMeterName, nameof (offerMeterName));
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5104100, "Commerce", nameof (FrameworkOfferSubscriptionService), "EnableExtensionOfTrial");
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceOfferSubscriptionHttpClient(requestContext).EnableTrialOfferSubscriptionExtensionAsync(offerMeterName, renewalGroup, endDate).Wait()), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).EnableTrialOfferSubscriptionExtension(offerMeterName, renewalGroup, endDate).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104100, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (AggregateException ex)
      {
        requestContext.TraceException(5104101, "Commerce", nameof (FrameworkOfferSubscriptionService), (Exception) ex);
        throw ex.Flatten().InnerException;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104102, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104103, "Commerce", nameof (FrameworkOfferSubscriptionService), "EnableExtensionOfTrial");
      }
    }

    public void ManualBillingEvent(
      IVssRequestContext requestContext,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      double quantity)
    {
      throw new NotImplementedException();
    }

    public void ManualAzureStoreManagedBillingEvent(
      IVssRequestContext collectionContext,
      string resourceName,
      int quantity,
      Guid? tenantId = null)
    {
      throw new NotImplementedException();
    }

    public void CreatePurchaseRequest(
      IVssRequestContext requestContext,
      PurchaseRequest purchaseRequest)
    {
      requestContext.CheckProjectCollectionRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(purchaseRequest.OfferMeterName, "OfferMeterName");
      ArgumentUtility.CheckGreaterThanZero((float) purchaseRequest.Quantity, "Quantity");
      requestContext.TraceEnter(5104400, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (CreatePurchaseRequest));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommercePurchaseRequestHttpClient(requestContext).CreatePurchaseRequestAsync(purchaseRequest).Wait()), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).CreatePurchaseRequest(purchaseRequest).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104400, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (AggregateException ex)
      {
        requestContext.TraceException(5104401, "Commerce", nameof (FrameworkOfferSubscriptionService), (Exception) ex);
        throw ex.Flatten().InnerException;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104401, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104410, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (CreatePurchaseRequest));
      }
    }

    public void UpdatePurchaseRequest(
      IVssRequestContext requestContext,
      PurchaseRequest purchaseRequest)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(purchaseRequest.OfferMeterName, "OfferMeterName");
      ArgumentUtility.CheckGreaterThanZero((float) purchaseRequest.Quantity, "Quantity");
      requestContext.CheckProjectCollectionRequestContext();
      requestContext.TraceEnter(5104411, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (UpdatePurchaseRequest));
      try
      {
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommercePurchaseRequestHttpClient(requestContext).UpdatePurchaseRequestAsync(purchaseRequest).Wait()), (Action) (() => this.GetOfferSubscriptionHttpClient(requestContext).UpdatePurchaseRequest(purchaseRequest).Wait()), "Commerce", nameof (FrameworkOfferSubscriptionService), 5104411, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext));
      }
      catch (AggregateException ex)
      {
        requestContext.TraceException(5104412, "Commerce", nameof (FrameworkOfferSubscriptionService), (Exception) ex);
        throw ex.Flatten().InnerException;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104412, "Commerce", nameof (FrameworkOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5104420, "Commerce", nameof (FrameworkOfferSubscriptionService), nameof (UpdatePurchaseRequest));
      }
    }
  }
}
