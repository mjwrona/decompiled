// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceQuantityUpdaterService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Migration.Utilities;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ResourceQuantityUpdaterService : 
    IResourceQuantityUpdaterService,
    IVssFrameworkService
  {
    private const string TraceArea = "Commerce";
    private const string TraceLayer = "ResourceQuantityUpdaterService";

    public virtual void UpdateOfferSubscription(
      IVssRequestContext requestContext,
      int? maximumQuantity,
      int? includedQuantity,
      bool? isPaidBillingEnabled,
      OfferMeter meterConfig,
      ResourceRenewalGroup renewalGroup)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5106069, "Commerce", nameof (ResourceQuantityUpdaterService), new object[4]
      {
        (object) maximumQuantity,
        (object) includedQuantity,
        (object) isPaidBillingEnabled,
        (object) meterConfig
      }, nameof (UpdateOfferSubscription));
      try
      {
        IOfferSubscriptionService service1 = requestContext.GetService<IOfferSubscriptionService>();
        IPermissionCheckerService service2 = requestContext.GetService<IPermissionCheckerService>();
        IOfferSubscription offerSubscription = service1.GetOfferSubscription(requestContext, meterConfig.Name, renewalGroup, true);
        if (isPaidBillingEnabled.HasValue)
        {
          int num1 = offerSubscription.IsPaidBillingEnabled ? 1 : 0;
          bool? nullable = isPaidBillingEnabled;
          int num2 = nullable.GetValueOrDefault() ? 1 : 0;
          if (!(num1 == num2 & nullable.HasValue))
          {
            requestContext.Trace(5106069, TraceLevel.Info, "Commerce", nameof (ResourceQuantityUpdaterService), string.Format("Updating toggle paid billing for {0} to value {1} with billing enabled: {2}", (object) requestContext.ServiceHost.InstanceId, (object) meterConfig, (object) isPaidBillingEnabled));
            service1.TogglePaidBilling(requestContext, meterConfig.Name, isPaidBillingEnabled.Value);
          }
        }
        if (!maximumQuantity.HasValue && !includedQuantity.HasValue)
          return;
        int? nullable1;
        if (maximumQuantity.HasValue)
        {
          nullable1 = maximumQuantity;
          int num = 0;
          if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
            maximumQuantity = new int?(meterConfig.MaximumQuantity);
        }
        if (includedQuantity.HasValue)
        {
          nullable1 = includedQuantity;
          int num = 0;
          if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
            includedQuantity = new int?(meterConfig.IncludedQuantity);
        }
        int maximumQuantity1 = offerSubscription.MaximumQuantity;
        nullable1 = maximumQuantity;
        int valueOrDefault1 = nullable1.GetValueOrDefault();
        if (maximumQuantity1 == valueOrDefault1 & nullable1.HasValue)
        {
          int includedQuantity1 = offerSubscription.IncludedQuantity;
          nullable1 = includedQuantity;
          int valueOrDefault2 = nullable1.GetValueOrDefault();
          if (includedQuantity1 == valueOrDefault2 & nullable1.HasValue)
            return;
        }
        if (service2.CheckPermission(requestContext.To(TeamFoundationHostType.Deployment), 4, CommerceSecurity.MeteringSecurityNamespaceId, throwAccessDenied: false))
        {
          requestContext.Trace(5106069, TraceLevel.Info, "Commerce", nameof (ResourceQuantityUpdaterService), string.Format("Updating included and maximum quantities for {0} and {1} to values {2} and {3}", (object) requestContext.ServiceHost.InstanceId, (object) meterConfig, includedQuantity.HasValue ? (object) includedQuantity.ToString() : (object) "existing-value", maximumQuantity.HasValue ? (object) maximumQuantity.ToString() : (object) "existing-value"));
          service1.SetAccountQuantity(requestContext, meterConfig.Name, renewalGroup, includedQuantity, maximumQuantity);
        }
        else
        {
          if (!maximumQuantity.HasValue)
            return;
          requestContext.Trace(5106069, TraceLevel.Info, "Commerce", nameof (ResourceQuantityUpdaterService), string.Format("Updating maximum quantity for {0} and {1} to values {2}", (object) requestContext.ServiceHost.InstanceId, (object) meterConfig, (object) maximumQuantity));
          service1.SetAccountQuantity(requestContext, meterConfig.Name, renewalGroup, new int?(offerSubscription.IncludedQuantity), maximumQuantity);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106069, "Commerce", nameof (ResourceQuantityUpdaterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106069, "Commerce", nameof (ResourceQuantityUpdaterService), nameof (UpdateOfferSubscription));
      }
    }

    public virtual void UpdateOfferSubscription(
      IVssRequestContext requestContext,
      int? maximumQuantity,
      int? includedQuantity,
      bool? isPaidBillingEnabled,
      OfferMeter meterConfig)
    {
      this.UpdateOfferSubscription(requestContext, maximumQuantity, includedQuantity, isPaidBillingEnabled, meterConfig, ResourceRenewalGroup.Monthly);
    }

    public virtual void SetResetInternalAccountResourceQuantities(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal meterUsage)
    {
      requestContext.TraceEnter(5106061, "Commerce", nameof (ResourceQuantityUpdaterService), nameof (SetResetInternalAccountResourceQuantities));
      try
      {
        this.SetAccountResourcesInternal(requestContext, meterUsage, true);
      }
      finally
      {
        requestContext.TraceLeave(5106063, "Commerce", nameof (ResourceQuantityUpdaterService), nameof (SetResetInternalAccountResourceQuantities));
      }
    }

    public virtual void GetInternalAccountResourceQuantities(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal meterUsage)
    {
      requestContext.TraceEnter(5106070, "Commerce", nameof (ResourceQuantityUpdaterService), nameof (GetInternalAccountResourceQuantities));
      try
      {
        this.SetAccountResourcesInternal(requestContext, meterUsage, false);
      }
      finally
      {
        requestContext.TraceLeave(5106074, "Commerce", nameof (ResourceQuantityUpdaterService), nameof (GetInternalAccountResourceQuantities));
      }
    }

    private void SetAccountResourcesInternal(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal meterUsage,
      bool updateDb)
    {
      if (meterUsage.Meter.BillingMode != ResourceBillingMode.PayAsYouGo && meterUsage.Meter.MeterId != 7)
        return;
      DateTime utcNow = this.GetUtcNow();
      bool isInternalAccount = this.IsAccountInternal(requestContext);
      OfferSubscriptionInternal expectedResource = meterUsage.Clone();
      bool resourceQuantity = this.GetResourceQuantity(requestContext, expectedResource, utcNow, isInternalAccount);
      if (meterUsage.IncludedQuantity == expectedResource.IncludedQuantity)
        return;
      if (updateDb)
        this.SetAccountQuantity(requestContext, meterUsage.MeterId, new int?(expectedResource.IncludedQuantity), new int?(expectedResource.MaximumQuantity), resourceQuantity, meterUsage.Meter);
      IVssRequestContext requestContext1 = requestContext;
      string name = meterUsage.Meter.Name;
      IVssServiceHost serviceHost = requestContext.ServiceHost;
      // ISSUE: variable of a boxed type
      __Boxed<Guid> local = (ValueType) (serviceHost != null ? serviceHost.InstanceId : Guid.NewGuid());
      string message = string.Format("{0} Resource of account {1} was updated: Included quantity from ", (object) name, (object) local) + string.Format("{0} to {1}; Maximum quantity from {2} to ", (object) meterUsage.IncludedQuantity, (object) expectedResource.IncludedQuantity, (object) meterUsage.MaximumQuantity) + string.Format("{0}. Reset required: {1}. DB updated: {2}", (object) expectedResource.MaximumQuantity, (object) resourceQuantity, (object) updateDb);
      requestContext1.Trace(5106062, TraceLevel.Info, "Commerce", nameof (ResourceQuantityUpdaterService), message);
      meterUsage.IncludedQuantity = expectedResource.IncludedQuantity;
      if (expectedResource.Meter.BillingMode == ResourceBillingMode.Committment && meterUsage.CommittedQuantity < meterUsage.IncludedQuantity)
      {
        meterUsage.CommittedQuantity = meterUsage.IncludedQuantity;
        meterUsage.CurrentQuantity = meterUsage.IncludedQuantity;
      }
      meterUsage.MaximumQuantity = expectedResource.MaximumQuantity;
      if (!resourceQuantity)
        return;
      meterUsage.CurrentQuantity = 0;
      meterUsage.CommittedQuantity = 0;
    }

    internal bool GetResourceQuantity(
      IVssRequestContext requestContext,
      OfferSubscriptionInternal expectedResource,
      DateTime currentDate,
      bool isInternalAccount)
    {
      if (isInternalAccount)
      {
        int num = 2000000;
        if (expectedResource.MeterId == 4 && expectedResource.IncludedQuantity != expectedResource.Meter.MaximumQuantity)
        {
          expectedResource.IncludedQuantity = expectedResource.Meter.MaximumQuantity;
          expectedResource.MaximumQuantity = expectedResource.Meter.MaximumQuantity;
          return false;
        }
        if (expectedResource.MeterId == 5 && expectedResource.IncludedQuantity <= num)
        {
          expectedResource.IncludedQuantity = num;
          expectedResource.MaximumQuantity = expectedResource.Meter.MaximumQuantity;
          return false;
        }
        if (expectedResource.MeterId == 7 && expectedResource.IncludedQuantity <= 10000)
        {
          expectedResource.IncludedQuantity = 10000;
          expectedResource.MaximumQuantity = 10000;
          return false;
        }
      }
      else if (expectedResource.IncludedQuantity < expectedResource.Meter.IncludedQuantity)
      {
        expectedResource.IncludedQuantity = expectedResource.Meter.IncludedQuantity;
        return true;
      }
      return false;
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.Trace(5106064, TraceLevel.Info, "Commerce", nameof (ResourceQuantityUpdaterService), "ResourceQuantityUpdaterService Starting");
      if (requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.Trace(5106065, TraceLevel.Info, "Commerce", nameof (ResourceQuantityUpdaterService), "ResourceQuantityUpdaterService ending");

    internal virtual bool IsAccountInternal(IVssRequestContext requestContext) => requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext.Elevate(), "Policy.IsInternal", false).EffectiveValue;

    internal virtual void SetAccountQuantity(
      IVssRequestContext collectionContext,
      int meterId,
      int? includedQuantity,
      int? maximumQuantity,
      bool resetUsedQuantity,
      ReadOnlyOfferMeter meterConfiguration)
    {
      try
      {
        collectionContext.TraceEnter(5106066, "Commerce", nameof (ResourceQuantityUpdaterService), nameof (SetAccountQuantity));
        Guid userCUID;
        Guid guid = CommerceIdentityHelper.UserIdentityIdOrCommerceContext(collectionContext, out userCUID);
        MigrationUtilities.SetStaleOrganization(collectionContext, meterId);
        IOfferSubscriptionCachedAccessService service = collectionContext.GetService<IOfferSubscriptionCachedAccessService>();
        OfferSubscriptionInternal priorMeterUsage = service.GetOfferSubscriptions(collectionContext, new int?(meterId)).FirstOrDefault<OfferSubscriptionInternal>();
        service.UpdateAccountQuantities(collectionContext, meterId, ResourceRenewalGroup.Monthly, includedQuantity, maximumQuantity, meterConfiguration.IncludedQuantity, meterConfiguration.MaximumQuantity, meterConfiguration.AbsoluteMaximumQuantity, meterConfiguration.BillingMode, guid, resetUsedQuantity);
        OfferSubscriptionInternal newMeterUsage = collectionContext.GetService<IOfferSubscriptionCachedAccessService>().GetOfferSubscriptions(collectionContext, new int?(meterId)).FirstOrDefault<OfferSubscriptionInternal>();
        DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, newMeterUsage?.MeterId, newMeterUsage?.RenewalGroup);
        this.SendSetAccountQuantityBIEvent(collectionContext, guid, userCUID, newMeterUsage, priorMeterUsage, Guid.Empty);
      }
      catch (CommerceInvalidQuantitySqlException ex)
      {
        collectionContext.TraceException(5106067, "Commerce", "Commerce", (Exception) ex);
        throw new AccountQuantityException(HostingResources.InvalidAccountQuantities((object) meterConfiguration.Name), ex.ErrorCode);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5106067, "Commerce", "Commerce", ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5106068, "Commerce", "Commerce", nameof (SetAccountQuantity));
      }
    }

    internal virtual DateTime GetUtcNow() => DateTime.UtcNow;

    internal virtual CommerceSqlComponent GetSqlComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<CommerceSqlComponent>();

    internal void SendSetAccountQuantityBIEvent(
      IVssRequestContext requestContext,
      Guid eventUserId,
      Guid eventUserCUID,
      OfferSubscriptionInternal newMeterUsage,
      OfferSubscriptionInternal priorMeterUsage,
      Guid subscriptionId)
    {
      if (newMeterUsage == null || priorMeterUsage == null)
        return;
      if (priorMeterUsage.IncludedQuantity != newMeterUsage.IncludedQuantity)
      {
        CustomerIntelligenceEventType intelligenceEventType = newMeterUsage.IncludedQuantity < priorMeterUsage.IncludedQuantity ? CustomerIntelligenceEventType.ReduceIncludedQuantity : CustomerIntelligenceEventType.IncreaseIncludedQuantity;
        this.TraceBIEventsForQuantityUpdaterService(requestContext, eventUserId, eventUserCUID, newMeterUsage, priorMeterUsage, Guid.Empty, intelligenceEventType.ToString());
      }
      if (priorMeterUsage.MaximumQuantity == newMeterUsage.MaximumQuantity)
        return;
      CustomerIntelligenceEventType intelligenceEventType1 = newMeterUsage.MaximumQuantity < priorMeterUsage.MaximumQuantity ? CustomerIntelligenceEventType.ReduceMaxQuantity : CustomerIntelligenceEventType.IncreaseMaxQuantity;
      this.TraceBIEventsForQuantityUpdaterService(requestContext, eventUserId, eventUserCUID, newMeterUsage, priorMeterUsage, Guid.Empty, intelligenceEventType1.ToString());
    }

    internal virtual void TraceBIEventsForQuantityUpdaterService(
      IVssRequestContext requestContext,
      Guid eventUserId,
      Guid eventUserCUID,
      OfferSubscriptionInternal newMeterUsage,
      OfferSubscriptionInternal priorMeterUsage,
      Guid subscriptionId,
      string eventType)
    {
      Guid instanceId1 = requestContext.ServiceHost.InstanceId;
      Guid instanceId2 = requestContext.ServiceHost.ParentServiceHost.InstanceId;
      int hostType = (int) requestContext.ServiceHost.HostType;
      Guid eventUserId1 = eventUserId;
      Guid eventUserCUID1 = eventUserCUID;
      string name = newMeterUsage.Meter.Name;
      Guid platformMeterId = newMeterUsage.Meter.PlatformMeterId;
      string renewalGroup = newMeterUsage.RenewalGroup.ToString();
      int includedQuantity = newMeterUsage.IncludedQuantity;
      int committedQuantity1 = newMeterUsage.CommittedQuantity;
      int? committedQuantity2 = priorMeterUsage?.CommittedQuantity;
      int currentQuantity = newMeterUsage.CurrentQuantity;
      int maximumQuantity = newMeterUsage.MaximumQuantity;
      int num = newMeterUsage.IsPaidBillingEnabled ? 1 : 0;
      DateTime utcNow1 = DateTime.UtcNow;
      // ISSUE: variable of a boxed type
      __Boxed<int> year = (ValueType) utcNow1.Year;
      utcNow1 = DateTime.UtcNow;
      string str = utcNow1.Month.ToString("00");
      string billingPeriod = string.Format("{0}{1}", (object) year, (object) str);
      string source = CommerceUtil.CheckForRequestSource(requestContext);
      Guid subscriptionId1 = subscriptionId;
      string meterCategory = newMeterUsage.Meter.Category.ToString();
      DateTime utcNow2 = DateTime.UtcNow;
      string eventType1 = eventType;
      TeamFoundationTracingService.TraceCommerceMeteredResource(instanceId1, instanceId2, (TeamFoundationHostType) hostType, eventUserId1, eventUserCUID1, name, platformMeterId, renewalGroup, includedQuantity, committedQuantity1, committedQuantity2, currentQuantity, maximumQuantity, 0, num != 0, billingPeriod, 0.0, 0, source, subscriptionId1, meterCategory, utcNow2, eventType1);
    }
  }
}
