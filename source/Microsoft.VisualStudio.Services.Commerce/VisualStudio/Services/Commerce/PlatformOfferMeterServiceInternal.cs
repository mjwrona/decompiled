// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformOfferMeterServiceInternal
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class PlatformOfferMeterServiceInternal : IOfferMeterService, IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "PlatformOfferMeterServiceInternal";

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public virtual IOfferMeter GetOfferMeter(IVssRequestContext requestContext, string galleryId)
    {
      try
      {
        requestContext.TraceEnter(5108709, "Commerce", nameof (PlatformOfferMeterServiceInternal), nameof (GetOfferMeter));
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return (IOfferMeter) vssRequestContext.GetService<IOfferMeterCachedAccessService>().GetOfferMeter(vssRequestContext, galleryId);
      }
      finally
      {
        requestContext.TraceLeave(5108710, "Commerce", nameof (PlatformOfferMeterServiceInternal), nameof (GetOfferMeter));
      }
    }

    public virtual PurchasableOfferMeter GetPurchasableOfferMeter(
      IVssRequestContext requestContext,
      string resourceName,
      string resourceNameResolveMethod,
      Guid? subscriptionId,
      bool includeMeterPricing,
      Guid? tenantId = null,
      Guid? objectId = null)
    {
      try
      {
        requestContext.CheckDeploymentRequestContext();
        requestContext.CheckHostedDeployment();
        requestContext.TraceEnter(5108901, "Commerce", nameof (PlatformOfferMeterServiceInternal), new object[6]
        {
          (object) resourceName,
          (object) resourceNameResolveMethod,
          (object) subscriptionId,
          (object) includeMeterPricing,
          (object) tenantId,
          (object) objectId
        }, nameof (GetPurchasableOfferMeter));
        if (string.IsNullOrEmpty(resourceName))
          throw new ArgumentNullException(nameof (resourceName));
        if (string.IsNullOrEmpty(resourceNameResolveMethod) || !resourceNameResolveMethod.Equals("GalleryId", StringComparison.OrdinalIgnoreCase) && !resourceNameResolveMethod.Equals("MeterName", StringComparison.OrdinalIgnoreCase))
          throw new ArgumentOutOfRangeException(nameof (resourceNameResolveMethod));
        OfferMeter offerMeter = (OfferMeter) requestContext.GetService<IOfferMeterService>().GetOfferMeter(requestContext, resourceName);
        PurchasableOfferMeter purchasableOfferMeter1 = new PurchasableOfferMeter()
        {
          OfferMeterDefinition = offerMeter
        };
        requestContext.Trace(5108902, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferMeterServiceInternal), "Retrieved configuration for offer meter " + resourceName + " for subscription " + subscriptionId?.ToString());
        if (includeMeterPricing)
        {
          if (subscriptionId.HasValue)
          {
            IAzureBillingService service1 = requestContext.GetService<IAzureBillingService>();
            if (!tenantId.HasValue || !objectId.HasValue)
            {
              CommerceBillingContextInfo contextForSubscription = service1.GetBillingContextForSubscription(requestContext, subscriptionId.Value);
              tenantId = (Guid?) contextForSubscription?.TenantId;
              objectId = (Guid?) contextForSubscription?.ObjectId;
            }
            if (!tenantId.HasValue || !objectId.HasValue)
              requestContext.Trace(5106756, TraceLevel.Warning, "Commerce", nameof (PlatformOfferMeterServiceInternal), string.Format("{0} returned tenantId:{1}, objectId:{2}", (object) "GetBillingContextForSubscription", (object) tenantId, (object) objectId));
            else if (offerMeter.BillingEntity == BillingProvider.SelfManaged)
            {
              IAzureResourceHelper service2 = requestContext.GetService<IAzureResourceHelper>();
              string currencyCode;
              string locale;
              purchasableOfferMeter1.MeterPricing = (IEnumerable<KeyValuePair<double, double>>) service2.GetMeterPricing(requestContext, subscriptionId.Value, offerMeter.PlatformMeterId, out currencyCode, out locale, tenantId.Value, objectId.Value);
              purchasableOfferMeter1.CurrencyCode = currencyCode;
              purchasableOfferMeter1.LocaleCode = locale;
              requestContext.Trace(5108903, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferMeterServiceInternal), "Pricing retrieved for locale " + locale + " and currency " + currencyCode);
            }
            else if (offerMeter.BillingEntity == BillingProvider.AzureStoreManaged)
            {
              CommerceBillingContextInfo billingContext = new CommerceBillingContextInfo()
              {
                ObjectId = new Guid?(objectId.Value),
                TenantId = new Guid?(tenantId.Value)
              };
              CommerceBillingAccountInfo billingAccountInfo = service1.GetBillingAccountInfo(requestContext, billingContext);
              string currencyCode;
              List<KeyValuePair<double, double>> meterPricing = requestContext.GetService<IAzureStoreService>().GetMeterPricing(requestContext, subscriptionId.Value, offerMeter, billingAccountInfo, out currencyCode);
              purchasableOfferMeter1.MeterPricing = (IEnumerable<KeyValuePair<double, double>>) meterPricing;
              purchasableOfferMeter1.CurrencyCode = string.IsNullOrEmpty(currencyCode) ? billingAccountInfo.CurrencyCode : currencyCode;
              purchasableOfferMeter1.LocaleCode = billingAccountInfo.CommunicationCulture;
              requestContext.Trace(5108903, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferMeterServiceInternal), "Pricing retrieved for locale " + billingAccountInfo.CommunicationCulture + " and currency " + billingAccountInfo.CurrencyCode);
            }
          }
          else
          {
            IList<OfferMeterPrice> offerMeterPriceList = (IList<OfferMeterPrice>) new List<OfferMeterPrice>();
            IList<OfferMeterPrice> offerMeterPrice = (IList<OfferMeterPrice>) requestContext.GetService<IOfferMeterPriceCachedAccessService>().GetOfferMeterPrice(requestContext, resourceName);
            requestContext.Trace(5108905, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferMeterServiceInternal), "Returning from Cache");
            if (!offerMeterPrice.IsNullOrEmpty<OfferMeterPrice>())
            {
              string region = "US";
              IEnumerable<OfferMeterPrice> offerMeterPrices = offerMeterPrice.Where<OfferMeterPrice>((Func<OfferMeterPrice, bool>) (x => string.Equals(x.Region, region)));
              if (!offerMeterPrices.IsNullOrEmpty<OfferMeterPrice>())
                purchasableOfferMeter1.MeterPricing = offerMeterPrices.Select<OfferMeterPrice, KeyValuePair<double, double>>((Func<OfferMeterPrice, KeyValuePair<double, double>>) (x => new KeyValuePair<double, double>(x.Quantity, x.Price)));
            }
            else
            {
              IDictionary<double, double> pricingForMeter = OfferPriceList.GetPricingForMeter(offerMeter.PlatformMeterId);
              if (pricingForMeter != null)
                purchasableOfferMeter1.MeterPricing = (IEnumerable<KeyValuePair<double, double>>) pricingForMeter;
              requestContext.Trace(5108904, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferMeterServiceInternal), "Returning default USD pricing");
            }
            purchasableOfferMeter1.LocaleCode = "en-us";
            purchasableOfferMeter1.CurrencyCode = "USD";
          }
          PurchasableOfferMeter purchasableOfferMeter2 = purchasableOfferMeter1;
          IEnumerable<KeyValuePair<double, double>> meterPricing1 = purchasableOfferMeter1.MeterPricing;
          List<KeyValuePair<double, double>> list = meterPricing1 != null ? meterPricing1.ToList<KeyValuePair<double, double>>() : (List<KeyValuePair<double, double>>) null;
          purchasableOfferMeter2.MeterPricing = (IEnumerable<KeyValuePair<double, double>>) list;
        }
        if (purchasableOfferMeter1.LocaleCode.IsNullOrEmpty<char>())
          purchasableOfferMeter1.LocaleCode = "en-us";
        if (purchasableOfferMeter1.CurrencyCode.IsNullOrEmpty<char>())
          purchasableOfferMeter1.CurrencyCode = "USD";
        PlatformOfferSubscriptionService service = requestContext.GetService<PlatformOfferSubscriptionService>();
        purchasableOfferMeter1.EstimatedRenewalDate = service.GetEstimatedMeterResetDateTime(requestContext, (IOfferMeter) offerMeter, subscriptionId);
        requestContext.TraceProperties<PurchasableOfferMeter>(5108915, "Commerce", nameof (PlatformOfferMeterServiceInternal), purchasableOfferMeter1, (string) null);
        return purchasableOfferMeter1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108909, "Commerce", nameof (PlatformOfferMeterServiceInternal), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5108910, "Commerce", nameof (PlatformOfferMeterServiceInternal), nameof (GetPurchasableOfferMeter));
      }
    }

    public IEnumerable<IOfferMeterPrice> GetOfferMeterPrice(
      IVssRequestContext requestContext,
      string galleryId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.TraceEnter(5108910, "Commerce", nameof (PlatformOfferMeterServiceInternal), new object[1]
      {
        (object) galleryId
      }, nameof (GetOfferMeterPrice));
      IEnumerable<OfferMeterPrice> enumerable = (IEnumerable<OfferMeterPrice>) null;
      try
      {
        enumerable = (IEnumerable<OfferMeterPrice>) vssRequestContext.GetService<IOfferMeterPriceCachedAccessService>().GetOfferMeterPrice(vssRequestContext, galleryId);
        if (enumerable.IsNullOrEmpty<OfferMeterPrice>())
        {
          IOfferMeter offerMeter = vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, galleryId);
          if (offerMeter.PlatformMeterId != Guid.Empty)
            enumerable = OfferPriceList.GetPricingForMeter(offerMeter.PlatformMeterId).ToOfferMeterPrice(galleryId);
        }
        return (IEnumerable<IOfferMeterPrice>) enumerable;
      }
      catch (Exception ex)
      {
        vssRequestContext.TraceException(5108911, "Commerce", nameof (PlatformOfferMeterServiceInternal), ex);
        throw;
      }
      finally
      {
        requestContext.TraceProperties<IEnumerable<OfferMeterPrice>>(5108913, "Commerce", nameof (PlatformOfferMeterServiceInternal), enumerable, (string) null);
        vssRequestContext.TraceLeave(5108912, "Commerce", nameof (PlatformOfferMeterServiceInternal), nameof (GetOfferMeterPrice));
      }
    }

    public void CreateOfferMeterDefinition(
      IVssRequestContext requestContext,
      IOfferMeter meterConfig)
    {
      requestContext.TraceEnter(5107270, "Commerce", nameof (PlatformOfferMeterServiceInternal), new object[1]
      {
        (object) meterConfig
      }, nameof (CreateOfferMeterDefinition));
      try
      {
        ArgumentUtility.CheckForNull<IOfferMeter>(meterConfig, nameof (meterConfig));
        this.CheckPermission(requestContext, 2);
        if (meterConfig.IncludedQuantity > meterConfig.MaximumQuantity)
          throw new AccountQuantityException(string.Format("Invalid Quantities: Included:{0}, MaxQuantity:{1}", (object) meterConfig.IncludedQuantity, (object) meterConfig.MaximumQuantity));
        this.SetupOfferMeterDefaults(meterConfig);
        IEnumerable<AzureOfferPlanDefinition> fixedQuantityPlans1 = meterConfig.FixedQuantityPlans;
        if ((fixedQuantityPlans1 != null ? (fixedQuantityPlans1.Any<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (plan => plan.Quantity <= 0)) ? 1 : 0) : 0) != 0)
          throw new AccountQuantityException("Number of plan users cannot be less than 1");
        if (meterConfig.MinimumRequiredAccessLevel == MinimumRequiredServiceLevel.None)
          meterConfig.MinimumRequiredAccessLevel = MinimumRequiredServiceLevel.Express;
        IOfferMeterCachedAccessService service = requestContext.GetService<IOfferMeterCachedAccessService>();
        service.UpdateOfferMeterDefinitionName(requestContext, meterConfig);
        if (meterConfig.MeterId != 0)
        {
          IEnumerable<AzureOfferPlanDefinition> fixedQuantityPlans2 = meterConfig.FixedQuantityPlans;
          if ((fixedQuantityPlans2 != null ? fixedQuantityPlans2.FirstOrDefault<AzureOfferPlanDefinition>()?.IsPublic : new bool?()).GetValueOrDefault())
          {
            IOfferMeter offerMeter = this.GetOfferMeter(requestContext, meterConfig.Name);
            if (offerMeter == null || offerMeter.FixedQuantityPlans.IsNullOrEmpty<AzureOfferPlanDefinition>())
              throw new InvalidOperationException("Attempted production plans update for meter (" + meterConfig.Name + ") with no existing plans.");
            List<AzureOfferPlanDefinition> list1 = offerMeter.FixedQuantityPlans.Where<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (plan => !plan.IsPublic)).ToList<AzureOfferPlanDefinition>();
            if (list1.IsNullOrEmpty<AzureOfferPlanDefinition>())
              throw new InvalidOperationException("Attempted production plans update for meter (" + meterConfig.Name + ") with no existing staged plans.");
            List<AzureOfferPlanDefinition> list2 = meterConfig.FixedQuantityPlans.ToList<AzureOfferPlanDefinition>();
            if (!list1.OrderBy<AzureOfferPlanDefinition, string>((Func<AzureOfferPlanDefinition, string>) (p => p.PlanId)).SequenceEqual<AzureOfferPlanDefinition>((IEnumerable<AzureOfferPlanDefinition>) list2.OrderBy<AzureOfferPlanDefinition, string>((Func<AzureOfferPlanDefinition, string>) (p => p.PlanId)), (IEqualityComparer<AzureOfferPlanDefinition>) new AzureOfferPlanDefinitionUpdateComparer()))
            {
              string str1 = string.Join(", ", list1.Select<AzureOfferPlanDefinition, string>((Func<AzureOfferPlanDefinition, string>) (p => p.PlanId)));
              string str2 = string.Join(", ", list2.Select<AzureOfferPlanDefinition, string>((Func<AzureOfferPlanDefinition, string>) (p => p.PlanId)));
              throw new InvalidOperationException("Attempted production plans update for meter (" + meterConfig.Name + "), but the list of plans doesn't exactly match the list of existing staged plans. Existing staged plan IDs: " + str1 + ". New production plan IDs: " + str2);
            }
          }
        }
        service.CreateOfferMeterDefinition(requestContext, meterConfig);
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        this.QueueOfferMeterPriceJob(requestContext, meterConfig);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5107271, "Commerce", nameof (PlatformOfferMeterServiceInternal), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5107289, "Commerce", nameof (PlatformOfferMeterServiceInternal), nameof (CreateOfferMeterDefinition));
      }
    }

    public IEnumerable<IOfferMeter> GetOfferMeters(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return (IEnumerable<IOfferMeter>) vssRequestContext.GetService<IOfferMeterCachedAccessService>().GetOfferMeters(vssRequestContext);
    }

    private void QueueOfferMeterPriceJob(
      IVssRequestContext deploymentRequestContext,
      IOfferMeter meterName)
    {
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new PopulateOfferMeterPriceJobData()
      {
        MeterName = meterName.GalleryId,
        RetryCount = 0
      });
      deploymentRequestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(deploymentRequestContext, "Populate Offer Meter Price Job", "Microsoft.VisualStudio.Services.Commerce.PopulateOfferMeterPriceJobExtension", xml, new TimeSpan(0, 0, 0));
    }

    private void SetupOfferMeterDefaults(IOfferMeter offerConfig)
    {
      if (offerConfig.RenewalFrequency == MeterRenewalFrequecy.None)
        offerConfig.RenewalFrequency = MeterRenewalFrequecy.Monthly;
      if (offerConfig.Unit == null)
        offerConfig.Unit = "Seat";
      offerConfig.BillingState = MeterBillingState.Paid;
      if (offerConfig.TrialDays != (byte) 0)
        return;
      offerConfig.TrialDays = (byte) 30;
    }

    internal virtual Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext)
    {
      return CommerceIdentityHelper.GetIdentity(requestContext);
    }

    internal virtual void CheckForRequestSource(
      IVssRequestContext requestContext,
      CustomerIntelligenceData eventData)
    {
      string str = CommerceUtil.CheckForRequestSource(requestContext);
      if (string.IsNullOrEmpty(str))
        return;
      eventData.Add(CustomerIntelligenceProperty.PurchaseLicensesSource, str);
    }

    internal virtual OfferMeter GetValidMeter(
      IVssRequestContext requestContext,
      string resourceName)
    {
      OfferMeter offerMeter = (OfferMeter) this.GetOfferMeter(requestContext, resourceName);
      return !(offerMeter == (OfferMeter) null) ? offerMeter : throw new ArgumentException("Invalid resource name", nameof (resourceName));
    }

    internal virtual void CheckPermission(IVssRequestContext requestContext, int permissions)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.IsSystemContext)
        return;
      requestContext.GetService<IPermissionCheckerService>().CheckPermission(requestContext, permissions, CommerceSecurity.CommerceSecurityNamespaceId);
    }

    internal static double GetProratedQuantity(Decimal quantity, DateTime billDate)
    {
      Decimal num1 = (Decimal) DateTime.DaysInMonth(billDate.Year, billDate.Month);
      Decimal num2 = num1 - (Decimal) billDate.Day;
      return SqlDecimal.ConvertToPrecScale((SqlDecimal) (quantity * (num2 / num1)), 20, 8).ToDouble();
    }
  }
}
