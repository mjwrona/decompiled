// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformOfferSubscriptionService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Commerce;
using Microsoft.VisualStudio.Services.Commerce.Common;
using Microsoft.VisualStudio.Services.Commerce.Common.Extensions;
using Microsoft.VisualStudio.Services.Commerce.Communicators;
using Microsoft.VisualStudio.Services.Commerce.Migration.Utilities;
using Microsoft.VisualStudio.Services.Commerce.Notifications;
using Microsoft.VisualStudio.Services.Commerce.Service.Risk;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PlatformOfferSubscriptionService : IOfferSubscriptionService, IVssFrameworkService
  {
    private IVssDateTimeProvider dateTimeProvider;
    internal Type usageEventStoreProviderType;
    private const string storageProviderRegistryPath = "/Service/Commerce/Metering/UsageEventStorageProvider";
    internal const string azurePortalBaseUriPath = "/Service/Commerce/Subscription/AzurePortalBaseUri";
    private IBillingMessageHelper billingMessageHelper;
    private const string embeddedErrorMessageIndicator = "Error message:";
    private const string scsErrorMessageIndicator = "SCS error:";
    private const string Area = "Commerce";
    private const string Layer = "PlatformOfferSubscriptionService";
    private const string lastUpdatedByTrialExtension = "DC1C6FEE-BBE5-4EAD-B239-FA50DE875CEE";
    private const string TrialReportingEventVersion = "1.0";
    private const string BillableReportingEventVersion = "2.0";
    private const string QuantityChangeReportingEventVersion = "1.0";
    private IUsageEventsStore usageEventStore;

    public PlatformOfferSubscriptionService()
      : this(VssDateTimeProvider.DefaultProvider)
    {
    }

    public PlatformOfferSubscriptionService(IVssDateTimeProvider dateTimeProvider) => this.dateTimeProvider = dateTimeProvider;

    internal virtual IBillingMessageHelper BillingMessageHelper => this.billingMessageHelper ?? (this.billingMessageHelper = (IBillingMessageHelper) new Microsoft.VisualStudio.Services.Commerce.BillingMessageHelper());

    public void ServiceStart(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string typeName = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/Commerce/Metering/UsageEventStorageProvider", string.Empty);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      this.usageEventStoreProviderType = Type.GetType(typeName, true);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      if (this.usageEventStore == null)
        return;
      this.usageEventStore.Cleanup(requestContext);
    }

    public IOfferSubscription GetOfferSubscription(
      IVssRequestContext collectionContext,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      bool nextBillingPeriod = false)
    {
      this.ValidateRequestContext(collectionContext);
      if (CommerceDeploymentHelper.IsCommerceServiceOfferMetersEnabled(collectionContext) && CommerceDeploymentHelper.IsMissingLocalDeployment(collectionContext) && !CommerceDeploymentHelper.IsOfferMeterHandledOffline(resourceName))
        return (IOfferSubscription) null;
      try
      {
        collectionContext.TraceEnter(5104202, "Commerce", nameof (PlatformOfferSubscriptionService), new object[3]
        {
          (object) resourceName,
          (object) renewalGroup,
          (object) nextBillingPeriod
        }, nameof (GetOfferSubscription));
        this.CheckPermissionUserContext(collectionContext, 1, 1);
        IVssRequestContext collectionContext1 = collectionContext.Elevate();
        OfferMeter validMeter = this.GetValidMeter(collectionContext, resourceName);
        IOfferSubscription offerSubscription = this.GetSubscriptionResourceInternal(collectionContext1, nextBillingPeriod, validMeter, renewalGroup).FirstOrDefault<IOfferSubscription>();
        collectionContext.TraceProperties<IOfferSubscription>(5104213, "Commerce", nameof (PlatformOfferSubscriptionService), offerSubscription, (string) null);
        return offerSubscription;
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5104203, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5104204, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (GetOfferSubscription));
      }
    }

    public IOfferSubscription GetOfferSubscription(
      IVssRequestContext collectionContext,
      string resourceName,
      bool nextBillingPeriod = false)
    {
      return this.GetOfferSubscription(collectionContext, resourceName, ResourceRenewalGroup.Monthly, nextBillingPeriod);
    }

    public virtual IEnumerable<IOfferSubscription> GetOfferSubscriptions(
      IVssRequestContext collectionContext,
      bool nextBillingPeriod = false)
    {
      this.ValidateRequestContext(collectionContext);
      try
      {
        collectionContext.TraceEnter(5104205, "Commerce", nameof (PlatformOfferSubscriptionService), new object[1]
        {
          (object) nextBillingPeriod
        }, nameof (GetOfferSubscriptions));
        this.CheckPermissionUserContext(collectionContext, 1, 1);
        IEnumerable<IOfferSubscription> resourceInternal = this.GetSubscriptionResourceInternal(collectionContext.Elevate(), nextBillingPeriod);
        collectionContext.TraceProperties<IEnumerable<IOfferSubscription>>(5104208, "Commerce", nameof (PlatformOfferSubscriptionService), resourceInternal, (string) null);
        return resourceInternal;
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5104206, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5104207, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (GetOfferSubscriptions));
      }
    }

    public void EnableTrialOrPreview(
      IVssRequestContext collectionContext,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup)
    {
      collectionContext.TraceEnter(5107440, "Commerce", nameof (PlatformOfferSubscriptionService), new object[2]
      {
        (object) offerMeterName,
        (object) renewalGroup
      }, nameof (EnableTrialOrPreview));
      try
      {
        this.CheckMigrationStatus(collectionContext);
        collectionContext.CheckHostedDeployment();
        this.ValidateRequestContext(collectionContext);
        if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.PerformCollectionOfferSubscriptionSecurityCheck"))
          this.CheckPermissionCollectionContext(collectionContext, 4, "/Trial");
        IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
        OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName);
        this.PerformValidations(collectionContext, offerMeter, offerMeterName, renewalGroup);
        OfferSubscriptionInternal subscriptionInternal1 = this.GetMeterUsages(collectionContext, new int?(offerMeter.MeterId), new ResourceRenewalGroup?(renewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>();
        if (subscriptionInternal1 != null && subscriptionInternal1.IsTrialOrPreview)
          throw new InvalidOperationException("This account is already in trial or preview.");
        MigrationUtilities.SetStaleOrganization(collectionContext, offerMeter.MeterId);
        this.CreateTrialOrPreview(collectionContext, offerMeter.MeterId, renewalGroup, offerMeter.IncludedQuantity);
        OfferSubscriptionInternal subscriptionInternal2 = this.GetMeterUsages(collectionContext, new int?(offerMeter.MeterId), new ResourceRenewalGroup?(renewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>();
        if (subscriptionInternal2 == null)
          return;
        DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, subscriptionInternal2?.MeterId, subscriptionInternal2?.RenewalGroup);
        if (!subscriptionInternal2.IsPreview)
          this.SaveTrialReportingEvent(collectionContext, CommerceReportingEventType.TrialStart, offerMeter, subscriptionInternal2);
        if (subscriptionInternal2.IsPreview)
          return;
        if (!collectionContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          this.BillingMessageHelper.SendTrialStartMessage(collectionContext, subscriptionInternal2);
          if (CommerceUtil.isValidMeterForTrialNotificationEmail(collectionContext, subscriptionInternal1.Meter.GalleryId))
          {
            if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableCommerceNotificationEmail"))
              new TrialStartNotification().SendTrialNotificationEmail(collectionContext, offerMeter.Name, -1);
            else
              new TrialStartNotificationCommunicator().SendTrialNotificationEmail(collectionContext, offerMeterName, -1);
          }
          this.QueueSendNotificationEmailJob(collectionContext, subscriptionInternal2);
        }
        this.WriteTrialCustomerIntelligenceEvent(collectionContext, "TrialStart", collectionContext.ServiceHost.InstanceId, offerMeterName, subscriptionInternal2.StartDate, subscriptionInternal2.TrialExpiryDate, TrialCustomerIntelligenceSource.MarketplaceStartTrial, inTrial: true);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5107441, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5107442, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (EnableTrialOrPreview));
      }
    }

    private void CreateTrialOrPreview(
      IVssRequestContext collectionContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      int includedQuantity = 0)
    {
      this.ValidateRequestContext(collectionContext);
      Guid lastUpdatedBy = new Guid("E0099FD8-83FA-4C4F-94FA-19F5F7B3C633");
      collectionContext.GetService<IOfferSubscriptionCachedAccessService>().CreateTrialOrPreview(collectionContext, meterId, renewalGroup, lastUpdatedBy, includedQuantity);
    }

    public void ExtendTrialDate(
      IVssRequestContext collectionContext,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      DateTime endDate)
    {
      this.ValidateRequestContext(collectionContext);
      collectionContext.CheckHostedDeployment();
      this.CheckMigrationStatus(collectionContext);
      collectionContext.TraceEnter(5115120, "Commerce", nameof (PlatformOfferSubscriptionService), new object[3]
      {
        (object) offerMeterName,
        (object) renewalGroup,
        (object) endDate
      }, nameof (ExtendTrialDate));
      try
      {
        this.CheckPermissionUserContext(collectionContext, 2, 4);
        IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
        OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName);
        this.PerformValidations(collectionContext, offerMeter, offerMeterName, renewalGroup);
        OfferSubscriptionInternal subscriptionInternal1 = this.GetMeterUsages(collectionContext, new int?(offerMeter.MeterId), new ResourceRenewalGroup?(renewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>();
        if (subscriptionInternal1 != null && subscriptionInternal1.IsPaidBillingEnabled && subscriptionInternal1.CommittedQuantity > 0)
          throw new InvalidOperationException("This " + subscriptionInternal1.Meter.Name + " is already purchased.");
        if (subscriptionInternal1 != null)
        {
          DateTime? trialExpiryDate = subscriptionInternal1.TrialExpiryDate;
          DateTime dateTime = endDate;
          if ((trialExpiryDate.HasValue ? (trialExpiryDate.GetValueOrDefault() > dateTime ? 1 : 0) : 0) != 0)
            throw new InvalidOperationException(string.Format("Trial for collection {0} need not be extended as endDate {1} is less than trial expiry date {2}", (object) collectionContext.ServiceHost.InstanceId, (object) endDate, (object) subscriptionInternal1.TrialExpiryDate));
        }
        MigrationUtilities.SetStaleOrganization(collectionContext, offerMeter.MeterId);
        int trialDays = subscriptionInternal1.CalculateTrialDays(endDate);
        collectionContext.TraceConditionally(5115123, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), (Func<string>) (() => string.Format("Offer Meter Name :{0} RenewalGroup :{1} End Date :{2} Calculated Trial Days :{3}", (object) offerMeterName, (object) renewalGroup, (object) endDate, (object) trialDays)));
        this.ExtendTrialExpiryDate(collectionContext, offerMeter.MeterId, renewalGroup, trialDays);
        OfferSubscriptionInternal subscriptionInternal2 = this.GetMeterUsages(collectionContext, new int?(offerMeter.MeterId), new ResourceRenewalGroup?(renewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>();
        DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, subscriptionInternal2?.MeterId, subscriptionInternal2?.RenewalGroup);
        this.SaveTrialReportingEvent(collectionContext, CommerceReportingEventType.TrialExtend, offerMeter, subscriptionInternal2);
        if (subscriptionInternal2 == null || subscriptionInternal2.IsPreview || !subscriptionInternal2.IsTrialOrPreview)
          return;
        this.BillingMessageHelper.SendTrialExtendedMessage(collectionContext, subscriptionInternal2);
        this.WriteTrialCustomerIntelligenceEvent(collectionContext, "TrialExtended", collectionContext.ServiceHost.InstanceId, offerMeterName, subscriptionInternal2.StartDate, subscriptionInternal2.TrialExpiryDate, TrialCustomerIntelligenceSource.CSSToolTrialExtension, inTrial: true);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5115121, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5115122, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (ExtendTrialDate));
      }
    }

    private void ExtendTrialExpiryDate(
      IVssRequestContext collectionContext,
      int meterId,
      ResourceRenewalGroup renewalGroup,
      int trialDays)
    {
      this.ValidateRequestContext(collectionContext);
      Guid lastUpdatedBy = new Guid("DC1C6FEE-BBE5-4EAD-B239-FA50DE875CEE");
      collectionContext.GetService<IOfferSubscriptionCachedAccessService>().ExtendTrialExpiryDate(collectionContext, meterId, renewalGroup, lastUpdatedBy, trialDays);
    }

    private void ResetCloudLoadTest(
      IVssRequestContext collectionContext,
      ResourceRenewalGroup renewalGroup)
    {
      this.ValidateRequestContext(collectionContext);
      collectionContext.CheckHostedDeployment();
      this.CheckMigrationStatus(collectionContext);
      collectionContext.TraceEnter(5109345, "Commerce", nameof (PlatformOfferSubscriptionService), new object[1]
      {
        (object) renewalGroup
      }, nameof (ResetCloudLoadTest));
      try
      {
        this.CheckPermissionUserContext(collectionContext, 2, 4);
        IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
        OfferMeter offerMeter = (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, "LoadTest");
        collectionContext.TraceAlways(5109348, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), new
        {
          Msg = nameof (ResetCloudLoadTest)
        }.Serialize());
        this.ResetCloudLoadTestUsage(collectionContext, offerMeter.MeterId, renewalGroup);
        OfferSubscriptionInternal offerSubscription = this.GetMeterUsages(collectionContext, new int?(offerMeter.MeterId), new ResourceRenewalGroup?(renewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>();
        if (offerSubscription == null || offerSubscription.CurrentQuantity != 0 || offerSubscription.CommittedQuantity != 0)
          return;
        offerSubscription.WriteCloudLoadTestResetCIData(collectionContext);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5109346, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5109347, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (ResetCloudLoadTest));
      }
    }

    private void ResetCloudLoadTestUsage(
      IVssRequestContext collectionContext,
      int meterId,
      ResourceRenewalGroup renewalGroup)
    {
      Guid lastUpdateByUserGuid = new Guid("75DFD427-6624-4791-AC3C-E61BB343AC72");
      collectionContext.GetService<IOfferSubscriptionCachedAccessService>().ResetCloudLoadTestUsage(collectionContext, meterId, renewalGroup, lastUpdateByUserGuid);
    }

    private void PerformValidations(
      IVssRequestContext requestContext,
      OfferMeter offerMeter,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup)
    {
      if (offerMeter == (OfferMeter) null)
        throw new OfferMeterNotFoundException(offerMeterName);
      if (offerMeter.IncludedInLicenseLevel != MinimumRequiredServiceLevel.None)
        throw new InvalidOperationException(string.Format("Cannot enable or extend trial for extension {0} since it is already included in the license level {1}", (object) offerMeter.Name, (object) offerMeter.IncludedInLicenseLevel));
      if (offerMeter.Category != MeterCategory.Extension)
        throw new ArgumentException(string.Format("Unsupported meter category {0}", (object) offerMeter.Category), nameof (offerMeterName));
      if (requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new ArgumentException("OfferSubscriptions of this scope must be called at the account level.", nameof (requestContext));
      if (renewalGroup != ResourceRenewalGroup.Monthly)
        throw new ArgumentException("For Extensions, renewal group must be monthly.", nameof (renewalGroup));
    }

    public void CreateOfferSubscription(
      IVssRequestContext requestContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      Guid? billingTarget)
    {
      this.CreateOfferSubscription(requestContext, offerMeterName, azureSubscriptionId, renewalGroup, quantity, (string) null, new Guid?(), new Guid?(), billingTarget, false, new Guid?(), false);
    }

    internal static void ValidateResourceResponse(ResourceResponse resourceResponse)
    {
      if (!string.IsNullOrWhiteSpace(resourceResponse?.error?.message))
      {
        string message = resourceResponse.error.message;
        int num = resourceResponse.error.message.IndexOf("Error message:");
        if (num != -1)
        {
          string json = resourceResponse.error.message.Substring(num + "Error message:".Length).Trim();
          if (json.StartsWith("'") && json.EndsWith("'"))
            json = json.Substring(1, json.Length - 2);
          EmbeddedInsightsMessageError insightsMessageError = JsonUtilities.Deserialize<EmbeddedInsightsMessageError>(json);
          if (!string.IsNullOrWhiteSpace(insightsMessageError.ErrorDescription))
          {
            string errorDescription = insightsMessageError.ErrorDescription;
            if (!string.IsNullOrWhiteSpace(errorDescription) && errorDescription.StartsWith("[\"") && errorDescription.EndsWith("\"]"))
              message = errorDescription.Substring(2, errorDescription.Length - 4);
          }
        }
        int length = message.IndexOf("SCS error:");
        message = length != -1 ? message.Substring(0, length).TrimEnd() : throw new AzureResponseException(message, (string) null);
      }
      else if (string.IsNullOrWhiteSpace(resourceResponse?.id))
        throw new AzureResponseException(HostingResources.UnknownArmResponseError(), (string) null);
    }

    public void CreateOfferSubscription(
      IVssRequestContext deploymentContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string offerCode = null,
      Guid? tenantId = null,
      Guid? objectId = null,
      Guid? billingTarget = null,
      bool immediate = false,
      Guid? sessionId = null,
      bool skipSubscriptionValidation = false)
    {
      deploymentContext.CheckDeploymentRequestContext();
      deploymentContext.CheckHostedDeployment();
      double billedQuantity = 0.0;
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity = deploymentContext.GetUserIdentity();
      IVssRequestContext requestContext = deploymentContext.Elevate();
      OfferMeter offerMeter = (OfferMeter) deploymentContext.GetService<IOfferMeterService>().GetOfferMeter(deploymentContext, offerMeterName);
      if (offerMeter == (OfferMeter) null)
        throw new OfferMeterNotFoundException(offerMeterName);
      if (offerMeter.IncludedInLicenseLevel != MinimumRequiredServiceLevel.None)
        throw new InvalidOperationException("Cannot buy extension");
      bool isBundle = offerMeter.Category == MeterCategory.Bundle;
      if (deploymentContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableSkipSubscriptionCheckOfferSubscription") & skipSubscriptionValidation && !isBundle)
        deploymentContext.TraceAlways(5106903, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Skipping Subscription validation in createoffersubscription on azure subscription {0}", (object) azureSubscriptionId));
      else
        this.VerifyCurrentUserIsAzureAdminOrCoAdmin(deploymentContext, azureSubscriptionId, isBundle);
      deploymentContext.Trace(5106903, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Subscription Id:{0} Offer Meter Name:{1} SignedInUser CUID:{2}", (object) azureSubscriptionId, (object) offerMeterName, (object) deploymentContext.GetUserCuid()));
      Guid purchaseHostId = billingTarget.GetValueOrDefault();
      deploymentContext.GetService<IArmAdapterService>().RegisterSubscriptionAgainstResourceProvider(deploymentContext, azureSubscriptionId);
      if (isBundle)
      {
        if (azureSubscriptionId == new Guid())
          throw new ArgumentException(string.Format("Incorrect subscription id {0}", (object) azureSubscriptionId), nameof (azureSubscriptionId));
        this.ValidateBundlePurchaseEligibility(deploymentContext, offerMeterName, azureSubscriptionId);
        if (offerMeter.RenewalFrequency == MeterRenewalFrequecy.Annually && renewalGroup == ResourceRenewalGroup.Monthly)
          renewalGroup = this.GetDefaultRenewalGroup(this.GetUtcNow());
        else if (offerMeter.RenewalFrequency == MeterRenewalFrequecy.Monthly && renewalGroup != ResourceRenewalGroup.Monthly)
          renewalGroup = ResourceRenewalGroup.Monthly;
        InfrastructureHostHelper.WithInfrastructureCollectionContext(requestContext, azureSubscriptionId, requestorIdentity, (Action<IVssRequestContext>) (collectionSystemContext =>
        {
          this.CheckMigrationStatus(collectionSystemContext);
          InfrastructureHostHelper.CopyRequestContextItems(deploymentContext, collectionSystemContext);
          purchaseHostId = collectionSystemContext.ServiceHost.InstanceId;
          OfferSubscriptionInternal subscriptionInternal = this.GetMeterUsages(collectionSystemContext, new int?(offerMeter.MeterId), new ResourceRenewalGroup?(renewalGroup), false).Single<OfferSubscriptionInternal>();
          int changedQuantity = quantity - subscriptionInternal.CurrentQuantity;
          if (changedQuantity > 0)
            this.GetRiskEvaluation(collectionSystemContext, deploymentContext.RemoteIPAddress(), deploymentContext.UserAgent, azureSubscriptionId, requestorIdentity, purchaseHostId, sessionId, tenantId, objectId, offerMeterName, changedQuantity);
          billedQuantity = this.CreateOfferReportUsage(collectionSystemContext, azureSubscriptionId, renewalGroup, quantity, offerMeter, requestorIdentity, immediate);
          PurchaseNotificationBundleCommunicator emailCommunicator = new PurchaseNotificationBundleCommunicator();
          DateTime meterResetDateTime = this.GetEstimatedMeterResetDateTime(collectionSystemContext, (IOfferMeter) offerMeter, new Guid?(azureSubscriptionId));
          this.SendPurchaseConfirmationEmail(collectionSystemContext, renewalGroup, offerMeter.MeterId, billedQuantity, azureSubscriptionId, offerCode, meterResetDateTime, tenantId, objectId, (PurchaseNotificationCommunicator) emailCommunicator, requestorIdentity);
          if (subscriptionInternal.Meter.RenewalFrequency != MeterRenewalFrequecy.Annually)
            return;
          TimeSpan scheduleDelay = this.GetNextMonthQueueDateTime(this.GetUtcNow(), subscriptionInternal.Meter.RenewalFrequency) - this.GetUtcNow();
          this.QueueSendAnnualSubscriptionPurchaseExpiryNotificationEmailJob(collectionSystemContext, scheduleDelay);
        }));
      }
      else
      {
        if (renewalGroup != ResourceRenewalGroup.Monthly)
          throw new ArgumentException("For Extensions, renewal group must be monthly.");
        if (!billingTarget.HasValue)
          throw new ArgumentException("Create offer subscription requires a billing target when called for extension purchases.");
        if (azureSubscriptionId == new Guid())
          throw new ArgumentException(string.Format("Incorrect subscription id {0} to link with {1}", (object) azureSubscriptionId, (object) billingTarget.Value), nameof (azureSubscriptionId));
        Guid collectionId = billingTarget.Value;
        AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(deploymentContext, collectionId);
        if (azureResourceAccount == null)
          deploymentContext.GetService<ISubscriptionService>().LinkCollection(deploymentContext.Elevate(), azureSubscriptionId, AccountProviderNamespace.VisualStudioOnline, collectionId, new Guid?(requestorIdentity.MasterId), true);
        else if (azureResourceAccount.AzureSubscriptionId != azureSubscriptionId)
          throw new ArgumentException(string.Format("Account is already linked to subscription {0}. Cannot purchase for subscription {1}.", (object) azureResourceAccount.AzureSubscriptionId, (object) azureSubscriptionId));
        CollectionHelper.WithCollectionContext(deploymentContext, collectionId, (Action<IVssRequestContext>) (collectionSystemContext =>
        {
          this.CheckMigrationStatus(collectionSystemContext);
          if (collectionSystemContext.IsFeatureEnabled(collectionId, "VisualStudio.Services.Commerce.EnableSkipSubscriptionCheckOfferSubscription") & skipSubscriptionValidation && !CommerceDeploymentHelper.IsProjectCollectionAdmin(collectionSystemContext, requestorIdentity, 5106903, "Commerce", nameof (PlatformOfferSubscriptionService)))
          {
            deploymentContext.TraceAlways(5106903, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format(" A non PCA idenitity {0} tried purchase {1} on azure subscription {2} for collection {3}", (object) requestorIdentity, (object) offerMeterName, (object) azureSubscriptionId, (object) collectionId));
            throw new AccessCheckException(string.Format("{0} is not a PCA to perform this operation", (object) requestorIdentity));
          }
          if (offerMeter.BillingEntity == BillingProvider.AzureStoreManaged)
          {
            if (collectionSystemContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableARMWritesFromCommerceService"))
              return;
            this.CreateAzureStoreManagedResource(deploymentContext, collectionSystemContext, offerMeter, quantity, azureSubscriptionId);
            DateTime meterResetDateTime = this.GetEstimatedMeterResetDateTime(collectionSystemContext, (IOfferMeter) offerMeter, new Guid?(azureSubscriptionId));
            this.SendPurchaseConfirmationEmail(collectionSystemContext, ResourceRenewalGroup.Monthly, offerMeter.MeterId, billedQuantity, azureSubscriptionId, (string) null, meterResetDateTime, tenantId, objectId, (PurchaseNotificationCommunicator) null, requestorIdentity);
          }
          else
          {
            billedQuantity = this.CreateOfferReportUsage(collectionSystemContext, azureSubscriptionId, renewalGroup, quantity, offerMeter, requestorIdentity, immediate);
            if (deploymentContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableCommerceEmailOnPurchase"))
              return;
            PurchaseNotificationExtensionCommunicator emailCommunicator = new PurchaseNotificationExtensionCommunicator();
            DateTime meterResetDateTime = this.GetEstimatedMeterResetDateTime(collectionSystemContext, (IOfferMeter) offerMeter, new Guid?(azureSubscriptionId));
            this.SendPurchaseConfirmationEmail(collectionSystemContext, renewalGroup, offerMeter.MeterId, billedQuantity, azureSubscriptionId, offerCode, meterResetDateTime, tenantId, objectId, (PurchaseNotificationCommunicator) emailCommunicator, requestorIdentity);
          }
        }), new RequestContextType?(RequestContextType.SystemContext), nameof (CreateOfferSubscription));
      }
      this.WriteOfferSubscriptionCustomerEvent(deploymentContext, offerMeter.GalleryId, purchaseHostId, azureSubscriptionId, quantity, "Purchase");
    }

    private void CreateAzureStoreManagedResource(
      IVssRequestContext deploymentContext,
      IVssRequestContext collectionSystemContext,
      OfferMeter offerMeter,
      int quantity,
      Guid azureSubscriptionId,
      Guid? tenantId = null)
    {
      IVssRequestContext vssRequestContext = deploymentContext.Elevate();
      IArmAdapterService service = deploymentContext.GetService<IArmAdapterService>();
      this.SaveAzureStoreManagedCIEvent(collectionSystemContext, offerMeter.Name, quantity, azureSubscriptionId, tenantId);
      AzureOfferPlanDefinition offerPlanDefinition = offerMeter.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, bool>) (x => x.Quantity == quantity));
      if (offerPlanDefinition == (AzureOfferPlanDefinition) null)
        throw new ArgumentException("Invalid quantity specified, no plan corresponds to the provided quantity.");
      Collection collection = collectionSystemContext.GetService<ICollectionService>().GetCollection(collectionSystemContext, (IEnumerable<string>) null);
      string name = collectionSystemContext.ServiceHost.Name;
      string preferredRegion = collection?.PreferredRegion;
      string azureAccountRegion = "West US";
      if (!string.IsNullOrEmpty(preferredRegion))
      {
        IVssRequestContext requestContext = deploymentContext.IsCommerceService() ? vssRequestContext : deploymentContext;
        azureAccountRegion = requestContext.GetExtension<ICommerceRegionHandler>().GetAzureRegion(requestContext, preferredRegion);
      }
      deploymentContext.Trace(5106904, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Signing agreement {0} for {1}", (object) offerPlanDefinition.PlanId, (object) azureSubscriptionId));
      service.SignAgreement(deploymentContext, azureSubscriptionId.ToString(), offerPlanDefinition.Publisher, offerPlanDefinition.OfferId, offerPlanDefinition.PlanName, tenantId);
      string resourceGroupName;
      string resourceName;
      this.GetResourceGroupAndName(deploymentContext, out resourceGroupName, out resourceName, collectionSystemContext.ServiceHost.InstanceId, string.Format("VS-{0}-Group", (object) name), name);
      ResourceGroupResponse getResourceGroup = this.CreateOrGetResourceGroup(deploymentContext, azureSubscriptionId, resourceGroupName, azureAccountRegion, tenantId);
      deploymentContext.Trace(5106908, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Overwrite azure account region {0} for subscription {1} with resource group region {2}", (object) azureAccountRegion, (object) azureSubscriptionId, (object) getResourceGroup.location));
      ResourceResponse accountResource = service.GetAccountResource(deploymentContext, azureSubscriptionId, resourceGroupName, resourceName, tenantId);
      string str = string.IsNullOrEmpty(accountResource?.location) ? getResourceGroup.location : accountResource.location;
      if (accountResource == null)
      {
        deploymentContext.Trace(CommerceTracePoints.PlatformOfferSubscriptionService.CreateOfferSubscription.CreateAccountResource, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Creating account {0} for {1}/{2}", (object) name, (object) azureSubscriptionId, (object) resourceGroupName));
        ResourceRequest request = new ResourceRequest(properties: new Dictionary<string, string>()
        {
          {
            "AccountName",
            name
          },
          {
            "operationType",
            "Link"
          }
        })
        {
          id = string.Format("/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.visualstudio/account/{2}", (object) azureSubscriptionId, (object) resourceGroupName, (object) resourceName),
          name = resourceName,
          type = "microsoft.visualstudio/account",
          location = str
        };
        ResourceResponse resourceResponse = service.PutResource(deploymentContext, azureSubscriptionId, resourceGroupName, resourceName, request, AzureErrorBehavior.SameAsSuccess, tenantId);
        deploymentContext.Trace(CommerceTracePoints.PlatformOfferSubscriptionService.CreateOfferSubscription.CreateAccountResource, TraceLevel.Error, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Creating account {0} for {1}/{2}", (object) name, (object) azureSubscriptionId, (object) resourceGroupName) + " Error Message: " + resourceResponse?.error?.message + " Error Code: " + resourceResponse?.error?.code);
        PlatformOfferSubscriptionService.ValidateResourceResponse(resourceResponse);
      }
      string galleryId1 = offerMeter.GalleryId;
      ResourceResponse extensionResource = this.GetExtensionResource(deploymentContext, azureSubscriptionId, resourceGroupName, resourceName, offerMeter, service, ref galleryId1, tenantId);
      deploymentContext.Trace(5106908, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), "Extension resource name " + galleryId1 + " is being used to buy for account " + name);
      ResourceRequest request1 = new ResourceRequest()
      {
        plan = (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "name",
            offerPlanDefinition.PlanId
          },
          {
            "publisher",
            offerPlanDefinition.Publisher
          },
          {
            "product",
            offerPlanDefinition.OfferId
          }
        }
      };
      if (extensionResource == null)
      {
        string galleryId2 = offerMeter.GalleryId;
        deploymentContext.Trace(CommerceTracePoints.PlatformOfferSubscriptionService.CreateOfferSubscription.CreateExtensionResource, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), "Creating extension for plan " + offerPlanDefinition.PlanId + " for account " + resourceName + " for gallery id " + galleryId2);
        request1.id = string.Format("/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.visualstudio/account/{2}/extension/{3}", (object) azureSubscriptionId, (object) resourceGroupName, (object) resourceName, (object) galleryId2);
        request1.name = galleryId2;
        request1.type = "microsoft.visualstudio/account/extension";
        request1.location = str;
        ResourceResponse resourceResponse = service.PutResource(deploymentContext, azureSubscriptionId, resourceGroupName, resourceName, galleryId2, request1, AzureErrorBehavior.SameAsSuccess, tenantId);
        deploymentContext.Trace(CommerceTracePoints.PlatformOfferSubscriptionService.CreateOfferSubscription.CreateExtensionResource, TraceLevel.Error, "Commerce", nameof (PlatformOfferSubscriptionService), "Creating extension for plan " + offerPlanDefinition.PlanId + " for account " + resourceName + " for gallery id " + galleryId2 + " Error Message: " + resourceResponse?.error?.message + " Error Code: " + resourceResponse?.error?.code);
        PlatformOfferSubscriptionService.ValidateResourceResponse(resourceResponse);
      }
      else
      {
        deploymentContext.Trace(5106906, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), "Patching existing extension plan for account " + name + " with new plan " + offerPlanDefinition.PlanId);
        ResourceResponse resourceResponse = service.PatchResource(deploymentContext, azureSubscriptionId, resourceGroupName, resourceName, galleryId1, request1, AzureErrorBehavior.SameAsSuccess, tenantId);
        deploymentContext.Trace(5106906, TraceLevel.Error, "Commerce", nameof (PlatformOfferSubscriptionService), "Patching existing extension plan for account " + name + " with new plan " + offerPlanDefinition.PlanId + " Error Message: " + resourceResponse?.error?.message + " Error Code: " + resourceResponse?.error?.code);
        PlatformOfferSubscriptionService.ValidateResourceResponse(resourceResponse);
      }
    }

    private void SaveAzureStoreManagedCIEvent(
      IVssRequestContext requestContext,
      string meterName,
      int quantity,
      Guid azureSubscriptionId,
      Guid? tenantId = null)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add(nameof (meterName), meterName);
      eventData.Add(nameof (quantity), (double) quantity);
      eventData.Add(nameof (azureSubscriptionId), (object) azureSubscriptionId);
      eventData.Add(nameof (tenantId), (object) tenantId);
      CustomerIntelligence.PublishEvent(requestContext, "ExtensionResourceRequest", eventData);
    }

    private void GetRiskEvaluation(
      IVssRequestContext requestContext,
      string ipAddress,
      string userAgent,
      Guid azureSubscriptionId,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity,
      Guid purchaseHostId,
      Guid? sessionId,
      Guid? tenantId,
      Guid? objectId,
      string offerMeterName,
      int changedQuantity)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.UseRiskEvaluationService"))
        return;
      RiskEvaluationResult evaluationResult = RiskEvaluationResult.Unknown;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IRiskEvaluationService service = vssRequestContext.GetService<IRiskEvaluationService>();
        using (new ActionPerformanceTracer("Commerce", nameof (PlatformOfferSubscriptionService)).Trace(vssRequestContext, 5109233, nameof (GetRiskEvaluation)))
        {
          evaluationResult = service.GetRiskEvaluation(vssRequestContext, ipAddress, userAgent, azureSubscriptionId, requestorIdentity, purchaseHostId, sessionId, tenantId, objectId, offerMeterName, changedQuantity);
          vssRequestContext.TraceAlways(5109234, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Risk evaluation result: {0}. azureSubscriptionId: {1}, ", (object) evaluationResult, (object) azureSubscriptionId) + string.Format("requestorIdentity: {0}, ", (object) IdentityCuidHelper.ComputeCuid(vssRequestContext, (IReadOnlyVssIdentity) requestorIdentity)) + string.Format("purchaseHostId: {0}, sessionId: {1}, tenantId: {2}, ", (object) purchaseHostId, (object) sessionId, (object) tenantId) + string.Format("objectId: {0}, offerMeterName: {1}, changedQuantity: {2}.", (object) objectId, (object) offerMeterName, (object) changedQuantity));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109235, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
      }
      if (requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.UseRiskEvaluationResult") && evaluationResult == RiskEvaluationResult.Rejected)
      {
        requestContext.TraceAlways(5109236, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Risk Evaluation is Reject and purchase is stopped.");
        throw new RiskEvaluationRejectException();
      }
    }

    internal void ValidateBundlePurchaseEligibility(
      IVssRequestContext requestContext,
      string galleryId,
      Guid subscriptionId)
    {
      if (!CommerceUtil.IsBundleEligibleForPurchase(requestContext, galleryId, subscriptionId))
        throw new InvalidOperationException("Yearly Visual Studio Subscription is not supported for purchase.");
    }

    internal ResourceResponse GetExtensionResource(
      IVssRequestContext deploymentContext,
      Guid azureSubscriptionId,
      string resourceGroupName,
      string resourceName,
      OfferMeter offerMeter,
      IArmAdapterService armAdapter,
      ref string returnExtensionResourceName,
      Guid? tenantId = null)
    {
      string extensionName = offerMeter.GalleryId;
      ResourceResponse resource = armAdapter.GetResource(deploymentContext, azureSubscriptionId, resourceGroupName, resourceName, extensionName, tenantId);
      if (resource == null)
      {
        extensionName = offerMeter.Name;
        resource = armAdapter.GetResource(deploymentContext, azureSubscriptionId, resourceGroupName, resourceName, extensionName, tenantId);
      }
      returnExtensionResourceName = extensionName;
      return resource;
    }

    internal ResourceGroupResponse CreateOrGetResourceGroup(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      string resourceGroupName,
      string azureAccountRegion,
      Guid? tenantId = null)
    {
      IArmAdapterService service = requestContext.GetService<IArmAdapterService>();
      requestContext.Trace(5106907, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Retrieving existing resource group {0} for subscription {1}", (object) resourceGroupName, (object) azureSubscriptionId));
      ResourceGroupResponse resourceGroup = service.GetResourceGroup(requestContext, azureSubscriptionId, resourceGroupName, tenantId);
      if (resourceGroup == null)
      {
        requestContext.Trace(5106905, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Did not find an existing resource group, so creating new resource group {0} for subscription {1} in region {2}", (object) resourceGroupName, (object) azureSubscriptionId, (object) azureAccountRegion));
        resourceGroup = service.CreateResourceGroup(requestContext, azureSubscriptionId, resourceGroupName, azureAccountRegion, tenantId);
        if (resourceGroup == null)
          throw new InvalidOperationException(string.Format("Creating resource group failed for resource group {0} in subscription {1}", (object) resourceGroupName, (object) azureSubscriptionId));
      }
      return resourceGroup;
    }

    public virtual void CancelOfferSubscription(
      IVssRequestContext deploymentContext,
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      string cancelReason,
      Guid? billingTarget,
      bool immediate = false,
      Guid? tenantId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      deploymentContext.CheckDeploymentRequestContext();
      deploymentContext.CheckHostedDeployment();
      deploymentContext.TraceEnter(5107901, "Commerce", nameof (PlatformOfferSubscriptionService), new object[6]
      {
        (object) offerMeterName,
        (object) azureSubscriptionId,
        (object) renewalGroup,
        (object) cancelReason,
        (object) billingTarget,
        (object) immediate
      }, nameof (CancelOfferSubscription));
      Guid valueOrDefault = billingTarget.GetValueOrDefault();
      try
      {
        OfferSubscriptionInternal offerSubscription = (OfferSubscriptionInternal) null;
        OfferMeter offerMeter = (OfferMeter) deploymentContext.GetService<IOfferMeterService>().GetOfferMeter(deploymentContext, offerMeterName);
        if (offerMeter == (OfferMeter) null)
          throw new OfferMeterNotFoundException(offerMeterName);
        if (!billingTarget.HasValue && (offerMeter.BillingEntity == BillingProvider.AzureStoreManaged || offerMeter.Category == MeterCategory.Extension))
          throw new ArgumentException("Billing target must be specified for extensions.", nameof (billingTarget));
        if (billingTarget.GetValueOrDefault() != Guid.Empty)
        {
          CollectionHelper.WithCollectionContext(deploymentContext.Elevate(), billingTarget.Value, (Action<IVssRequestContext>) (collectionContext =>
          {
            this.CheckMigrationStatus(collectionContext);
            Guid collectionId = billingTarget.Value;
            if (offerMeter.BillingEntity == BillingProvider.AzureStoreManaged && !collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableARMWritesFromCommerceService"))
            {
              string name = collectionContext.ServiceHost.Name;
              string resourceGroupName;
              string resourceName;
              this.GetResourceGroupAndName(deploymentContext, out resourceGroupName, out resourceName, collectionId, string.Format("VS-{0}-Group", (object) name), name);
              deploymentContext.Trace(5107904, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), "Deleting resource {0} from resourceGroup {1} and collection {2}", (object) offerMeter.Name, (object) resourceGroupName, (object) name);
              if (ServicePrincipals.IsServicePrincipal(deploymentContext, deploymentContext.UserContext))
              {
                if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                  tenantId = CommerceUtil.GetSubscriptionTenantId(deploymentContext, azureSubscriptionId);
                this.DeleteArmSubscriptionResource(deploymentContext.Elevate(), azureSubscriptionId, resourceGroupName, resourceName, offerMeter, tenantId);
              }
              else
                this.DeleteArmSubscriptionResource(deploymentContext, azureSubscriptionId, resourceGroupName, resourceName, offerMeter, tenantId);
              if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableSetResourceUsageOnExtensionDelete"))
                return;
              collectionContext.GetService<IResourceTaggingService>().DeleteTags(collectionContext, offerMeter.GalleryId);
              collectionContext.Trace(5109127, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Deleted tags for resource " + offerMeter.GalleryId);
              collectionContext.TraceConditionally(5107303, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), (Func<string>) (() => string.Format("Feature flag state for cancellation without write is {0}, meter {1}, collection {2}", (object) collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableThirdPartyOrderCancellationWrite"), (object) offerMeter.GalleryId, (object) collectionId)));
              if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableThirdPartyOrderCancellationWrite"))
                return;
              collectionContext.Items.Add("Commerce.RequestSource", (object) "Marketplace");
              this.CreateOfferReportUsage(collectionContext, azureSubscriptionId, ResourceRenewalGroup.Monthly, 0, offerMeter, CommerceConstants.ReportUsageUserId, false);
              this.WriteThirdPartyOfferSubscriptionCancellationCustomerIntelligenceEvent(deploymentContext, collectionId, offerMeter);
            }
            else
            {
              OfferMeter validMeter = this.GetValidMeter(deploymentContext, offerMeterName);
              int includedQuantity = this.GetMeterUsages(collectionContext, new int?(validMeter.MeterId), new ResourceRenewalGroup?(renewalGroup), false).First<OfferSubscriptionInternal>().IncludedQuantity;
              IVssRequestContext deploymentContext1 = deploymentContext;
              string offerMeterName1 = offerMeterName;
              Guid azureSubscriptionId1 = azureSubscriptionId;
              int renewalGroup1 = (int) renewalGroup;
              int quantity = includedQuantity;
              Guid? nullable = new Guid?(collectionId);
              bool flag = immediate;
              Guid? tenantId1 = new Guid?();
              Guid? objectId = new Guid?();
              Guid? billingTarget1 = nullable;
              int num = flag ? 1 : 0;
              Guid? sessionId = new Guid?();
              this.CreateOfferSubscription(deploymentContext1, offerMeterName1, azureSubscriptionId1, (ResourceRenewalGroup) renewalGroup1, quantity, tenantId: tenantId1, objectId: objectId, billingTarget: billingTarget1, immediate: num != 0, sessionId: sessionId);
              if (offerMeter.Category != MeterCategory.Extension)
                return;
              MigrationUtilities.SetStaleOrganization(collectionContext, offerMeter.MeterId);
              Guid userIdentityId = CommerceIdentityHelper.UserIdentityIdOrCommerceContext(deploymentContext);
              IOfferSubscriptionCachedAccessService service = collectionContext.GetService<IOfferSubscriptionCachedAccessService>();
              service.UpdatePaidBillingMode(collectionContext, validMeter.MeterId, false, userIdentityId);
              offerSubscription = service.GetOfferSubscriptions(collectionContext, new int?(validMeter.MeterId)).FirstOrDefault<OfferSubscriptionInternal>();
              DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, offerSubscription?.MeterId, offerSubscription?.RenewalGroup);
            }
          }), method: nameof (CancelOfferSubscription));
        }
        else
        {
          IVssRequestContext deploymentContext2 = deploymentContext;
          string offerMeterName2 = offerMeterName;
          Guid azureSubscriptionId2 = azureSubscriptionId;
          int renewalGroup2 = (int) renewalGroup;
          Guid? nullable = billingTarget;
          bool flag = immediate;
          Guid? tenantId2 = new Guid?();
          Guid? objectId = new Guid?();
          Guid? billingTarget2 = nullable;
          int num = flag ? 1 : 0;
          Guid? sessionId = new Guid?();
          this.CreateOfferSubscription(deploymentContext2, offerMeterName2, azureSubscriptionId2, (ResourceRenewalGroup) renewalGroup2, 0, tenantId: tenantId2, objectId: objectId, billingTarget: billingTarget2, immediate: num != 0, sessionId: sessionId);
        }
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        eventData.Add(CustomerIntelligenceProperty.AccountId, (object) valueOrDefault);
        eventData.Add(CustomerIntelligenceProperty.ActivityId, (object) deploymentContext.ActivityId);
        eventData.Add(CustomerIntelligenceProperty.PaidBillingState, false);
        eventData.Add(CustomerIntelligenceProperty.ResourceType, offerMeterName);
        eventData.Add(CustomerIntelligenceProperty.CancelPurchaseReason, cancelReason);
        if (offerSubscription != null)
        {
          eventData.Add(CustomerIntelligenceProperty.CommittedQuantity, (double) offerSubscription.CommittedQuantity);
          eventData.Add(CustomerIntelligenceProperty.TrialOrPreview, offerSubscription.IsTrialOrPreview);
        }
        this.CheckForRequestSource(deploymentContext, eventData);
        CustomerIntelligence.PublishEvent(deploymentContext, "CancelPurchase", eventData);
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5107903, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(5107902, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (CancelOfferSubscription));
      }
      this.WriteOfferSubscriptionCustomerEvent(deploymentContext, offerMeterName, valueOrDefault, azureSubscriptionId, 0, "CancelPurchase");
    }

    private void DeleteArmSubscriptionResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      OfferMeter offerMeter,
      Guid? tenantId = null)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IArmAdapterService service = vssRequestContext.GetService<IArmAdapterService>();
      service.DeleteResource(vssRequestContext, subscriptionId, resourceGroupName, resourceName, offerMeter.Name, tenantId);
      service.DeleteResource(vssRequestContext, subscriptionId, resourceGroupName, resourceName, offerMeter.GalleryId, tenantId);
    }

    internal void EnablePaidBillingModeOnLinkingAccount(IVssRequestContext collectionContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(collectionContext, nameof (collectionContext));
      try
      {
        this.CheckMigrationStatus(collectionContext);
        collectionContext.TraceEnter(5106981, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (EnablePaidBillingModeOnLinkingAccount));
        IOfferSubscriptionCachedAccessService service = collectionContext.GetService<IOfferSubscriptionCachedAccessService>();
        IEnumerable<OfferSubscriptionInternal> subscriptionInternals = service.GetOfferSubscriptions(collectionContext, new int?()).Where<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (m => m.Meter.BillingMode == ResourceBillingMode.Committment));
        Guid userIdentityId = CommerceIdentityHelper.UserIdentityIdOrCommerceContext(collectionContext);
        foreach (OfferSubscriptionInternal subscriptionInternal in subscriptionInternals)
        {
          MigrationUtilities.SetStaleOrganization(collectionContext, subscriptionInternal.MeterId);
          service.UpdatePaidBillingMode(collectionContext, subscriptionInternal.MeterId, true, userIdentityId);
          DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, new int?(subscriptionInternal.MeterId));
        }
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5106983, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5106982, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (EnablePaidBillingModeOnLinkingAccount));
      }
    }

    public virtual double CreateOfferReportUsage(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int committedQuantity,
      OfferMeter offerMeter,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity,
      bool immediate)
    {
      return this.CreateOfferReportUsage(requestContext, azureSubscriptionId, renewalGroup, committedQuantity, offerMeter, requestorIdentity.Id, immediate);
    }

    public virtual double CreateOfferReportUsage(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int committedQuantity,
      OfferMeter offerMeter,
      Guid requestorIdentityId,
      bool immediate)
    {
      requestContext.Trace(5106800, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Report usage to {0} total committed quantity for meter {1}, for user {2} with collection {3} on azure subscription {4}", (object) committedQuantity, (object) offerMeter.Name, (object) requestorIdentityId, (object) requestContext.ServiceHost.InstanceId, (object) azureSubscriptionId));
      string eventId = string.Format("{0} {1}", (object) offerMeter.Name, (object) Guid.NewGuid());
      return this.ReportUsage(requestContext, requestorIdentityId, offerMeter.Name, renewalGroup, committedQuantity, eventId, this.GetUtcNow(), immediate);
    }

    public virtual double ReportUsage(
      IVssRequestContext collectionContext,
      Guid eventUserId,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string eventId,
      DateTime billableDate,
      bool immediate = false)
    {
      this.ValidateRequestContext(collectionContext);
      this.CheckMigrationStatus(collectionContext);
      this.ValidateBlobStoreReportingOperation(collectionContext, resourceName);
      ArgumentUtility.CheckStringForNullOrEmpty(eventId, nameof (eventId));
      ArgumentUtility.CheckForEmptyGuid(eventUserId, nameof (eventUserId));
      collectionContext.TraceEnter(5104211, "Commerce", nameof (PlatformOfferSubscriptionService), new object[6]
      {
        (object) eventUserId,
        (object) resourceName,
        (object) renewalGroup,
        (object) quantity,
        (object) eventId,
        (object) billableDate
      }, nameof (ReportUsage));
      Guid lastUpdatedBy = new Guid("D7E4D4E2-6861-4948-9769-24F9DCF2C02F");
      double billedQuantity = 0.0;
      if (billableDate.Kind != DateTimeKind.Utc)
        throw new ArgumentException(nameof (billableDate));
      OfferMeter meterConfig = this.GetValidMeter(collectionContext, resourceName);
      this.TrySetSourceForPayAsYouGoMeters(collectionContext, meterConfig);
      if (meterConfig.BillingMode == ResourceBillingMode.Committment)
        this.CheckPermissionUserContext(collectionContext, 2, 2);
      else
        this.CheckPermissionSystemContext(collectionContext, 2);
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.GetIdentity(collectionContext);
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, "serviceIdentity");
        OfferSubscriptionInternal meterUsage = this.GetMeterUsages(collectionContext, new int?(meterConfig.MeterId), new ResourceRenewalGroup?(renewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (x => x.MeterId == meterConfig.MeterId));
        if (meterUsage.Meter.Category == MeterCategory.Legacy)
          collectionContext.GetService<IResourceQuantityUpdaterService>().SetResetInternalAccountResourceQuantities(collectionContext, meterUsage);
        AzureResourceAccount azureResourceAccount1;
        AzureSubscriptionInternal azureSubscription1;
        this.GetAzureResourceAccountAndSubscription(collectionContext, out azureResourceAccount1, out azureSubscription1);
        IVssRequestContext collectionContext1 = collectionContext;
        List<OfferSubscriptionInternal> meteredResources = new List<OfferSubscriptionInternal>();
        meteredResources.Add(meterUsage);
        AzureResourceAccount azureResourceAccount2 = azureResourceAccount1;
        AzureSubscriptionInternal azureSubscription2 = azureSubscription1;
        IOfferSubscription offerSubscription = this.GetSubscriptionResourceInternal(collectionContext1, (IEnumerable<OfferSubscriptionInternal>) meteredResources, azureResourceAccount2, azureSubscription2, false).FirstOrDefault<IOfferSubscription>();
        ArgumentUtility.CheckForNull<IOfferSubscription>(offerSubscription, "resource");
        if (this.IsValidArtifactsUsage(azureResourceAccount1, meterUsage, azureSubscription1, offerSubscription, quantity, billableDate) || this.IsValidUsage(azureResourceAccount1, meterUsage, offerSubscription, quantity, billableDate))
        {
          int committedQuantity = meterUsage.CommittedQuantity;
          if (azureSubscription1 != null && meterConfig.BillingMode == ResourceBillingMode.Committment && meterUsage.IncludedQuantity < quantity && !meterUsage.IsPreview)
          {
            if (meterUsage.IsTrialOrPreview)
            {
              if (meterUsage.TrialExpiryDate.HasValue)
              {
                DateTime? trialExpiryDate = meterUsage.TrialExpiryDate;
                DateTime dateTime = billableDate;
                if ((trialExpiryDate.HasValue ? (trialExpiryDate.GetValueOrDefault() < dateTime ? 1 : 0) : 0) != 0)
                  goto label_13;
              }
              this.QueueFirstTrialToPurchaseJob(collectionContext, meterUsage);
              this.WriteTrialCustomerIntelligenceEvent(collectionContext, "TrialBuy", collectionContext.ServiceHost.InstanceId, meterUsage.Meter.GalleryId, meterUsage.StartDate, meterUsage.TrialExpiryDate, TrialCustomerIntelligenceSource.MarketplaceBuy, new int?(meterUsage.IncludedQuantity), new int?(meterUsage.CommittedQuantity), true);
              goto label_15;
            }
label_13:
            this.QueueResetJob(collectionContext, offerSubscription.ResetDate, meterUsage, "ReportUsage API");
          }
label_15:
          collectionContext.Trace(5104212, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Complete validation and starting saving to azure table storage");
          UsageEvent usageEvent1 = this.CreateUsageEvent(collectionContext, eventUserId, quantity, eventId, billableDate, meterConfig, identity, azureSubscription1);
          if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableNewBillingPipeline"))
          {
            string str = this.SaveUsageEventInEventStore(collectionContext, usageEvent1);
            usageEvent1.EventId = str;
          }
          IOfferSubscriptionCachedAccessService service = collectionContext.GetService<IOfferSubscriptionCachedAccessService>();
          AggregateUsageEventResult aggregateUsageEventResult = new AggregateUsageEventResult();
          bool flag1 = this.IsArtifactMeter(meterConfig.Name);
          bool flag2 = this.IsInternalBuildMeter(meterConfig.Name);
          MigrationUtilities.SetStaleOrganization(collectionContext, meterConfig.MeterId);
          if (!flag1)
            aggregateUsageEventResult = service.AggregateUsageEvents(collectionContext, meterConfig, renewalGroup, usageEvent1, lastUpdatedBy, this.dateTimeProvider.UtcNow);
          else if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableArtifactsReporting"))
          {
            int quantity1 = quantity - meterUsage.CommittedQuantity;
            UsageEvent usageEvent2 = this.CreateUsageEvent(collectionContext, eventUserId, quantity1, eventId, billableDate, meterConfig, identity, azureSubscription1);
            collectionContext.Trace(5109269, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Saving artifact meter usage into commerce DB. " + string.Format("Old Usage: {0} New quantity: {1} Submitting Quantity: {2}", (object) meterUsage.CommittedQuantity, (object) quantity, (object) quantity1));
            aggregateUsageEventResult = service.AggregateUsageEvents(collectionContext, meterConfig, renewalGroup, usageEvent2, lastUpdatedBy, this.dateTimeProvider.UtcNow);
            aggregateUsageEventResult.BillableQuantity = quantity - meterUsage.IncludedQuantity <= 0 || !meterUsage.IsPaidBillingEnabled ? 0.0 : (double) (quantity - meterUsage.IncludedQuantity);
          }
          if (flag2 && !collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableInternalBuildBilling"))
            aggregateUsageEventResult.BillableQuantity = 0.0;
          if (flag1 && !this.ShouldBillUsage(collectionContext, meterUsage, billableDate))
            aggregateUsageEventResult.BillableQuantity = 0.0;
          if (meterUsage.IsTrialOrPreview)
          {
            if (meterUsage.TrialExpiryDate.HasValue)
            {
              DateTime? trialExpiryDate = meterUsage.TrialExpiryDate;
              DateTime dateTime = billableDate;
              if ((trialExpiryDate.HasValue ? (trialExpiryDate.GetValueOrDefault() < dateTime ? 1 : 0) : 0) == 0)
                goto label_33;
            }
            else
              goto label_33;
          }
          if (!meterUsage.IsPreview)
          {
            BillableEvent billableEvent1 = new BillableEvent();
            BillableEvent billableEvent2;
            if (!flag1)
            {
              billableEvent2 = this.CreateBillableEvent(collectionContext, aggregateUsageEventResult, usageEvent1, meterConfig);
            }
            else
            {
              collectionContext.Trace(5109270, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Creating billable events for Artifacts usage. Billing: {0}", (object) (aggregateUsageEventResult.BillableQuantity / 31.0)));
              billableEvent2 = this.CreateBillableEvent(collectionContext, usageEvent1.SubscriptionId, meterConfig, meterConfig.Name == "Artifacts" ? aggregateUsageEventResult.BillableQuantity / 31.0 : aggregateUsageEventResult.BillableQuantity, usageEvent1.EventTimestamp, usageEvent1.EventUniqueId);
            }
            BillingEventsManipulator.CreateManipulator(BillingEventType.Purchase).UpdateEvents(collectionContext, billableEvent2);
            billedQuantity = this.SaveBillableEvents(collectionContext, offerSubscription, meterConfig, azureSubscription1, billableEvent2, meterUsage, renewalGroup, azureResourceAccount1);
          }
label_33:
          if (quantity < meterUsage.CommittedQuantity & immediate)
            service.UpdateCommittedAndCurrentQuantities(collectionContext, meterConfig.MeterId, (byte) renewalGroup, quantity, quantity, eventUserId);
          this.TrySetOnPremisesPurchaseSource(collectionContext, azureResourceAccount1);
          this.SaveBillableReportingEvent(collectionContext, eventUserId, (double) quantity, (double) meterUsage.CurrentQuantity, billedQuantity, (IOfferMeter) meterConfig, meterUsage, offerSubscription.ResetDate, azureSubscription1);
          OfferSubscriptionInternal subscriptionInternal = service.GetOfferSubscriptions(collectionContext, new int?(meterConfig.MeterId)).FirstOrDefault<OfferSubscriptionInternal>();
          CustomerIntelligenceEventType intelligenceEventType = this.GetCustomerIntelligenceEventType((double) quantity, (double) meterUsage.CurrentQuantity, billedQuantity, meterUsage);
          if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableNewBillingPipeline"))
            this.TraceBIEventsForMeteringService(collectionContext, eventUserId, IdentityCuidHelper.GetCuidByVsid(collectionContext, eventUserId), aggregateUsageEventResult.UpdatedOfferSubscription, quantity, billedQuantity, subscriptionInternal != null ? subscriptionInternal.CommittedQuantity : committedQuantity, committedQuantity, quantity - meterUsage.CurrentQuantity, azureSubscription1 == null ? Guid.Empty : azureSubscription1.AzureSubscriptionId, intelligenceEventType.ToString());
          if (meterUsage.IsTrialOrPreview && !meterUsage.IsPreview && meterUsage.TrialExpiryDate.HasValue)
          {
            DateTime? trialExpiryDate = meterUsage.TrialExpiryDate;
            DateTime dateTime = billableDate;
            if ((trialExpiryDate.HasValue ? (trialExpiryDate.GetValueOrDefault() < dateTime ? 1 : 0) : 0) != 0)
            {
              this.WriteTrialCustomerIntelligenceEvent(collectionContext, "TrialEndedBuy", collectionContext.ServiceHost.InstanceId, meterUsage.Meter.GalleryId, meterUsage.StartDate, meterUsage.TrialExpiryDate, TrialCustomerIntelligenceSource.None);
              service.RemoveTrialForPaidOfferSubscription(collectionContext, meterUsage.Meter.MeterId, renewalGroup, meterUsage.Meter.IncludedQuantity, lastUpdatedBy);
            }
          }
          DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, new int?(meterConfig.MeterId), new ResourceRenewalGroup?(renewalGroup));
          this.QueueAggregateJob(collectionContext);
          this.AddCIEventForCommitmentLicenses(collectionContext, meterConfig, aggregateUsageEventResult.UpdatedOfferSubscription, quantity, azureSubscription1, eventUserId);
          this.SendOfferSubscriptionUpgradeDowngradeMessage(collectionContext, eventUserId, quantity, meterUsage, azureSubscription1?.AzureSubscriptionId, aggregateUsageEventResult, immediate);
        }
        return billedQuantity;
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5104219, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5104220, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (ReportUsage));
      }
    }

    private void SendOfferSubscriptionUpgradeDowngradeMessage(
      IVssRequestContext collectionContext,
      Guid eventUserId,
      int quantity,
      OfferSubscriptionInternal meterUsage,
      Guid? azureSubscriptionId,
      AggregateUsageEventResult aggregateUsageEventResult,
      bool shouldBeImmediate = false,
      bool sendAccountRightsMessage = false)
    {
      this.ValidateRequestContext(collectionContext);
      if (meterUsage.BillingEntity == BillingProvider.AzureStoreManaged)
        shouldBeImmediate = true;
      try
      {
        if (quantity > meterUsage.CommittedQuantity)
        {
          Guid valueOrDefault = azureSubscriptionId.GetValueOrDefault();
          this.BillingMessageHelper.CreateOfferSubscriptionUpgradeMessage(collectionContext, eventUserId, aggregateUsageEventResult?.UpdatedOfferSubscription ?? meterUsage, valueOrDefault, collectionContext.ServiceHost.InstanceId);
        }
        else
        {
          if (!shouldBeImmediate || quantity > meterUsage.CommittedQuantity)
            return;
          List<DowngradedResource> downgradedResourceList = new List<DowngradedResource>()
          {
            new DowngradedResource()
            {
              RenewalGroup = meterUsage.RenewalGroup,
              MeterId = meterUsage.MeterId,
              NewQuantity = quantity
            }
          };
          Guid valueOrDefault = azureSubscriptionId.GetValueOrDefault();
          this.BillingMessageHelper.CreateOfferSubscriptionDowngradeMessage(collectionContext, (IEnumerable<DowngradedResource>) downgradedResourceList, valueOrDefault, collectionContext.ServiceHost.InstanceId);
          if (!sendAccountRightsMessage)
            return;
          IOfferMeterService service = collectionContext.GetService<IOfferMeterService>();
          this.SendAccountRightsNotifications(collectionContext, service.GetOfferMeters(collectionContext), new MeterResetEvents()
          {
            DowngradedResources = (IEnumerable<DowngradedResource>) downgradedResourceList
          });
        }
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5104222, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
      }
    }

    private void QueueFirstTrialToPurchaseJob(
      IVssRequestContext collectionContext,
      OfferSubscriptionInternal meterUsage)
    {
      this.ValidateRequestContext(collectionContext);
      int num = meterUsage.IncludedQuantity == -1 ? meterUsage.Meter.IncludedQuantity : meterUsage.IncludedQuantity;
      if (meterUsage.CommittedQuantity > num)
        return;
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new List<KeyValue<string, int>>()
      {
        new KeyValue<string, int>("MeterId", meterUsage.MeterId),
        new KeyValue<string, int>("RenewalGroup", (int) meterUsage.RenewalGroup)
      });
      TimeSpan startOffset = meterUsage.TrialExpiryDate.Value.AddDays(1.0) - this.GetUtcNow();
      collectionContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(collectionContext, "Commerce Delayed Billing For Trial Job", "Microsoft.VisualStudio.Services.Commerce.FirstBillingEventForTrialOfferJobExtension", xml, startOffset);
    }

    private void QueueSendNotificationEmailJob(
      IVssRequestContext collectionContext,
      OfferSubscriptionInternal meterUsage)
    {
      this.ValidateRequestContext(collectionContext);
      if (meterUsage.CommittedQuantity > meterUsage.IncludedQuantity)
        return;
      DateTime? trialExpiryDate = meterUsage.TrialExpiryDate;
      if (!trialExpiryDate.HasValue)
        return;
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new List<KeyValue<string, int>>()
      {
        new KeyValue<string, int>("MeterId", meterUsage.Meter.MeterId),
        new KeyValue<string, int>("RenewalGroup", (int) meterUsage.RenewalGroup)
      });
      trialExpiryDate = meterUsage.TrialExpiryDate;
      TimeSpan startOffset = trialExpiryDate.Value.AddDays(-7.0) - this.GetUtcNow();
      collectionContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(collectionContext, "Send Emails On or Before Trial expiry", "Microsoft.VisualStudio.Services.Commerce.SendNotificationEmailOnTrialExpiryJobExtension", xml, startOffset);
    }

    internal virtual void QueueSendAnnualSubscriptionPurchaseExpiryNotificationEmailJob(
      IVssRequestContext collectionContext,
      TimeSpan scheduleDelay)
    {
      this.ValidateRequestContext(collectionContext);
      if (scheduleDelay <= TimeSpan.Zero)
      {
        collectionContext.Trace(5115110, TraceLevel.Error, "Commerce", nameof (PlatformOfferSubscriptionService), "Received zero/negative delay to schedule the job");
      }
      else
      {
        Guid jobGuid = new Guid("8DF56497-4267-496D-ABB1-4AF0668973EC");
        if (!JobHelper.DoesJobDefinitionExist(collectionContext, jobGuid))
          JobHelper.CreateJobDefinition(collectionContext, jobGuid, "Commerce Subscription Renewal Email Monthly Job", "Microsoft.VisualStudio.Services.Commerce.SendAnnualSubscriptionPurchaseExpiryNotificationEmailJobExtension");
        if (JobHelper.IsJobScheduled(collectionContext, jobGuid))
          return;
        JobHelper.QueueDelayedJob(collectionContext, jobGuid, scheduleDelay);
      }
    }

    public void ManualBillingEvent(
      IVssRequestContext collectionContext,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      double quantity)
    {
      ArgumentUtility.CheckForOutOfRange<double>(quantity, nameof (quantity), 0.0);
      collectionContext.CheckProjectCollectionRequestContext();
      OfferMeter meterConfig = this.GetValidMeter(collectionContext, resourceName);
      this.CheckPermissionSystemContext(collectionContext, 2);
      try
      {
        collectionContext.TraceEnter(5104260, "Commerce", nameof (PlatformOfferSubscriptionService), new object[3]
        {
          (object) resourceName,
          (object) renewalGroup,
          (object) quantity
        }, nameof (ManualBillingEvent));
        OfferSubscriptionInternal subscriptionInternal = this.GetMeterUsages(collectionContext, new int?(meterConfig.MeterId), new ResourceRenewalGroup?(renewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (x => x.MeterId == meterConfig.MeterId));
        Guid instanceId = collectionContext.ServiceHost.InstanceId;
        Guid empty = Guid.Empty;
        IVssRequestContext deploymentContext = collectionContext.To(TeamFoundationHostType.Deployment);
        AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(deploymentContext, instanceId);
        Guid subscriptionId = azureResourceAccount != null ? azureResourceAccount.AzureSubscriptionId : throw new AzureResourceAccountMissingException(instanceId);
        collectionContext.TraceProperties<AzureResourceAccount>(5104264, "Commerce", nameof (PlatformOfferSubscriptionService), azureResourceAccount, (string) null);
        collectionContext.Trace(5104261, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Starting to create billable event");
        BillableEvent billableEvent1 = this.CreateBillableEvent(collectionContext, subscriptionId, meterConfig, quantity, this.GetUtcNow(), Guid.NewGuid());
        collectionContext.Trace(5104262, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Created billable event and saving it in usageStore");
        if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableNewBillingPipeline"))
          return;
        IUsageEventsStore eventsStoreInstance = this.GetUsageEventsStoreInstance(deploymentContext);
        string location = string.IsNullOrEmpty(azureResourceAccount.AzureGeoRegion) ? this.GetAzureRegion(collectionContext) : azureResourceAccount.AzureGeoRegion;
        IVssRequestContext requestContext = deploymentContext;
        AzureBillableEvent2 billableEvent2 = new AzureBillableEvent2(billableEvent1, location, azureResourceAccount.GetArmResourceUri(), CsmUtilities.GetAccountTags(collectionContext, azureResourceAccount));
        eventsStoreInstance.SaveBillableEvent<AzureBillableEvent2>(requestContext, billableEvent2);
        collectionContext.TraceAlways(5104263, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Saved the event it in usageStore");
        Guid userCUID;
        Guid eventUserId = CommerceIdentityHelper.UserIdentityIdOrCommerceContext(collectionContext, out userCUID);
        DateTime utcNow = this.GetUtcNow();
        TeamFoundationTracingService.TraceCommerceMeteredResource(instanceId, collectionContext.ServiceHost.ParentServiceHost.InstanceId, collectionContext.ServiceHost.HostType, eventUserId, userCUID, subscriptionInternal.Meter.Name, subscriptionInternal.Meter.PlatformMeterId, subscriptionInternal.RenewalGroup.ToString(), subscriptionInternal.IncludedQuantity, subscriptionInternal.CommittedQuantity, new int?(subscriptionInternal.CommittedQuantity), subscriptionInternal.CurrentQuantity, subscriptionInternal.MaximumQuantity, 0, subscriptionInternal.IsPaidBillingEnabled, string.Format("{0}{1}", (object) utcNow.Year, (object) utcNow.Month.ToString("00")), quantity, subscriptionInternal.CommittedQuantity, azureResourceAccount == null || azureResourceAccount.ProviderNamespaceId != AccountProviderNamespace.OnPremise ? "ManualBilling" : "TeamFoundationServerManualBilling", subscriptionId, subscriptionInternal.Meter.Category.ToString(), utcNow, "Billing");
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5104265, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5104266, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (ManualBillingEvent));
      }
    }

    public void ManualAzureStoreManagedBillingEvent(
      IVssRequestContext collectionContext,
      string resourceName,
      int quantity,
      Guid? tenantId = null)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      OfferMeter validMeter = this.GetValidMeter(collectionContext, resourceName);
      this.CheckPermissionSystemContext(collectionContext, 2);
      try
      {
        Guid instanceId = collectionContext.ServiceHost.InstanceId;
        IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
        Guid azureSubscriptionId = (this.GetAzureResourceAccount(vssRequestContext, instanceId) ?? throw new AzureResourceAccountMissingException(instanceId)).AzureSubscriptionId;
        if (!tenantId.HasValue)
          tenantId = CommerceUtil.GetSubscriptionTenantId(vssRequestContext, azureSubscriptionId);
        if (!tenantId.HasValue)
          throw new ArgumentException(string.Format("Invalid AzureSubscription {0}, can't get AAD tenant", (object) azureSubscriptionId));
        this.CreateAzureStoreManagedResource(collectionContext.To(TeamFoundationHostType.Deployment), collectionContext, validMeter, quantity, azureSubscriptionId, tenantId);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5104265, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
    }

    public virtual DateTime GetEstimatedMeterResetDateTime(
      IVssRequestContext requestContext,
      IOfferMeter offerMeter,
      Guid? subscriptionId)
    {
      DateTime utcNow = this.GetUtcNow();
      ResourceRenewalGroup resourceRenewalGroup = ResourceRenewalGroup.Monthly;
      if (offerMeter.RenewalFrequency == MeterRenewalFrequecy.Annually)
      {
        resourceRenewalGroup = this.GetDefaultRenewalGroup(utcNow);
        OfferSubscriptionInternal subscriptionInternal = this.GetMeterUsagesForSubscription(requestContext, new int?(offerMeter.MeterId), subscriptionId.GetValueOrDefault(), new ResourceRenewalGroup?(resourceRenewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>();
        int? year1 = subscriptionInternal?.LastResetDate.Year;
        int year2 = utcNow.Year;
        if (!(year1.GetValueOrDefault() == year2 & year1.HasValue) && subscriptionInternal != null && subscriptionInternal.RenewalGroup == resourceRenewalGroup && subscriptionInternal != null && subscriptionInternal.CurrentQuantity > 0)
          return new DateTime(utcNow.Year, (int) resourceRenewalGroup, subscriptionInternal.LastResetDate.Day);
      }
      return this.GetNextCalendarDayResetTime(requestContext, (AzureResourceAccount) null, resourceRenewalGroup, new DateTime?());
    }

    public virtual IEnumerable<IUsageEventAggregate> GetUsage(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan,
      ResourceName resource)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, resource.ToString()).BillingMode != ResourceBillingMode.PayAsYouGo)
        return Enumerable.Empty<IUsageEventAggregate>();
      AggregationInterval interval = AggregationInterval.Hourly;
      if (timeSpan.TotalDays == (double) (int) timeSpan.TotalDays)
      {
        interval = AggregationInterval.Daily;
      }
      else
      {
        if (timeSpan.TotalHours != (double) (int) timeSpan.TotalHours)
          throw new ArgumentException("Argument timeSpan currently only supports multiple of an hour or a day.");
        interval = AggregationInterval.Hourly;
      }
      this.usageEventStore = this.GetUsageEventsStoreInstance(vssRequestContext);
      AggregationTotal[] array = this.usageEventStore.GetAggregationTotals(requestContext, resource.ToString(), startTime, endTime, interval).OrderBy<AggregationTotal, string>((Func<AggregationTotal, string>) (x => x.RowKey)).ToArray<AggregationTotal>();
      List<IUsageEventAggregate> usage = new List<IUsageEventAggregate>();
      DateTime sliceStartTime = startTime;
      DateTime sliceEndTime = startTime + timeSpan;
      while (sliceStartTime < endTime)
      {
        IEnumerable<AggregationTotal> source = ((IEnumerable<AggregationTotal>) array).Where<AggregationTotal>((Func<AggregationTotal, bool>) (x =>
        {
          string format = interval == AggregationInterval.Hourly ? "yyyy-MM-dd-HH" : "yyyy-MM-dd";
          DateTime exact = DateTime.ParseExact(x.RowKey, format, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
          return exact >= sliceStartTime && exact < sliceEndTime;
        }));
        UsageEventAggregate usageEventAggregate = new UsageEventAggregate()
        {
          StartTime = sliceStartTime,
          EndTime = sliceEndTime,
          Resource = resource,
          Value = source.Aggregate<AggregationTotal, int>(0, (Func<int, AggregationTotal, int>) ((x, y) => x + y.Value))
        };
        usage.Add((IUsageEventAggregate) usageEventAggregate);
        sliceStartTime = sliceEndTime;
        sliceEndTime = sliceStartTime + timeSpan;
      }
      return (IEnumerable<IUsageEventAggregate>) usage;
    }

    public void TogglePaidBilling(
      IVssRequestContext collectionContext,
      string resourceName,
      bool paidBillingState)
    {
      collectionContext.TraceEnter(5104231, "Commerce", nameof (PlatformOfferSubscriptionService), new object[2]
      {
        (object) resourceName,
        (object) paidBillingState
      }, nameof (TogglePaidBilling));
      try
      {
        this.ValidateRequestContext(collectionContext);
        this.CheckMigrationStatus(collectionContext);
        OfferMeter validMeter = this.GetValidMeter(collectionContext, resourceName);
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        if (validMeter.BillingMode == ResourceBillingMode.Committment)
          throw new InvalidResourceException(HostingResources.ResourceIsUnusable((object) resourceName, (object) HostingResources.ResourceNotPayAsYouGo()));
        this.CheckPermissionUserContext(collectionContext, 4, 2);
        eventData.Add(CustomerIntelligenceProperty.AccountId, (object) collectionContext.ServiceHost.InstanceId);
        eventData.Add(CustomerIntelligenceProperty.ActivityId, (object) collectionContext.ActivityId);
        eventData.Add(CustomerIntelligenceProperty.PaidBillingState, paidBillingState);
        eventData.Add(CustomerIntelligenceProperty.ResourceType, resourceName);
        this.CheckForRequestSource(collectionContext, eventData);
        Guid instanceId = collectionContext.ServiceHost.InstanceId;
        IVssRequestContext context = collectionContext.To(TeamFoundationHostType.Deployment);
        if (context.GetService<PlatformSubscriptionService>().GetAzureResourceAccountByCollectionId(context.Elevate(), instanceId, true) == null)
          throw new InvalidResourceException(HostingResources.ResourceIsUnusable((object) resourceName, (object) HostingResources.NoAccountRelationshipExists()));
        MigrationUtilities.SetStaleOrganization(collectionContext, validMeter.MeterId);
        Guid userIdentityId = CommerceIdentityHelper.UserIdentityIdOrCommerceContext(collectionContext);
        collectionContext.GetService<IOfferSubscriptionCachedAccessService>().UpdatePaidBillingMode(collectionContext, validMeter.MeterId, paidBillingState, userIdentityId);
        DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, new int?(validMeter.MeterId));
        CustomerIntelligence.PublishEvent(collectionContext, "ToggleBillingState", eventData);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5104239, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        collectionContext.TraceLeave(5104240, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (TogglePaidBilling));
      }
    }

    public void SetAccountQuantity(
      IVssRequestContext collectionContext,
      string resourceName,
      ResourceRenewalGroup renewalGroup,
      int? includedQuantity,
      int? maximumQuantity)
    {
      this.ValidateRequestContext(collectionContext);
      this.CheckMigrationStatus(collectionContext);
      collectionContext.TraceEnter(5104221, "Commerce", nameof (PlatformOfferSubscriptionService), new object[3]
      {
        (object) resourceName,
        (object) includedQuantity,
        (object) maximumQuantity
      }, nameof (SetAccountQuantity));
      OfferMeter validMeter = this.GetValidMeter(collectionContext, resourceName);
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add(CustomerIntelligenceProperty.ResourceType, resourceName);
      eventData.Add(CustomerIntelligenceProperty.ActivityId, (object) collectionContext.ActivityId);
      eventData.Add(CustomerIntelligenceProperty.AccountId, (object) collectionContext.ServiceHost.InstanceId);
      eventData.Add(CustomerIntelligenceProperty.ResourceIncludedQuantity, (object) includedQuantity);
      eventData.Add(CustomerIntelligenceProperty.ResourceMaximumQuantity, (object) maximumQuantity);
      this.CheckForRequestSource(collectionContext, eventData);
      int? nullable1 = includedQuantity;
      int num1 = 0;
      if (!(nullable1.GetValueOrDefault() < num1 & nullable1.HasValue))
      {
        int? nullable2 = maximumQuantity;
        int num2 = 0;
        if (!(nullable2.GetValueOrDefault() < num2 & nullable2.HasValue))
        {
          try
          {
            this.CheckPermissionUserContext(collectionContext, 4, 2);
            Guid instanceId = collectionContext.ServiceHost.InstanceId;
            Guid userCUID;
            Guid guid = CommerceIdentityHelper.UserIdentityIdOrCommerceContext(collectionContext, out userCUID);
            OfferSubscriptionInternal subscriptionInternal1 = this.GetMeterUsages(collectionContext, new int?(validMeter.MeterId), new ResourceRenewalGroup?(ResourceRenewalGroup.Monthly), false).FirstOrDefault<OfferSubscriptionInternal>();
            MigrationUtilities.SetStaleOrganization(collectionContext, validMeter.MeterId);
            collectionContext.GetService<IOfferSubscriptionCachedAccessService>().UpdateAccountQuantities(collectionContext, validMeter.MeterId, renewalGroup, includedQuantity, maximumQuantity, validMeter.IncludedQuantity, validMeter.MaximumQuantity, validMeter.AbsoluteMaximumQuantity, validMeter.BillingMode, guid);
            CustomerIntelligence.PublishEvent(collectionContext, "UpdateMeter", eventData);
            OfferSubscriptionInternal subscriptionInternal2 = this.GetMeterUsages(collectionContext, new int?(validMeter.MeterId), new ResourceRenewalGroup?(ResourceRenewalGroup.Monthly), false).FirstOrDefault<OfferSubscriptionInternal>();
            DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, new int?(validMeter.MeterId), new ResourceRenewalGroup?(renewalGroup));
            AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(collectionContext.To(TeamFoundationHostType.Deployment), instanceId);
            this.TrySetOnPremisesPurchaseSource(collectionContext, azureResourceAccount);
            this.SendSetAccountQuantityBIEvent(collectionContext, guid, userCUID, subscriptionInternal1, subscriptionInternal2, subscriptionInternal2.CommittedQuantity, subscriptionInternal1.CommittedQuantity, Guid.Empty);
            if (resourceName == "LoadTest")
              CommerceAuditUtilities.LogCLTLimitUpdate(collectionContext, subscriptionInternal1.MaximumQuantity, subscriptionInternal2.MaximumQuantity, subscriptionInternal1.IsPaidBillingEnabled, subscriptionInternal1.IncludedQuantity);
            if (subscriptionInternal1.IncludedQuantity > subscriptionInternal2.IncludedQuantity && validMeter.BillingMode == ResourceBillingMode.Committment)
              this.QueueResetJob(collectionContext, this.GetEstimatedMeterResetDateTime(collectionContext, (IOfferMeter) validMeter, new Guid?()), subscriptionInternal1, nameof (SetAccountQuantity));
            this.SaveQuantityChangeReportingEvent(collectionContext, validMeter, subscriptionInternal2, subscriptionInternal1);
            this.SendOfferSubscriptionUpgradeDowngradeMessage(collectionContext, collectionContext.GetUserIdentity().Id, subscriptionInternal2.CurrentQuantity, subscriptionInternal2, new Guid?(), (AggregateUsageEventResult) null, true, true);
            this.WriteOfferSubscriptionCustomerEvent(collectionContext, subscriptionInternal2.Meter.GalleryId, instanceId, azureResourceAccount != null ? azureResourceAccount.AzureSubscriptionId : Guid.Empty, subscriptionInternal2.CommittedQuantity, "Update");
            return;
          }
          catch (Exception ex)
          {
            collectionContext.TraceException(5104229, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
            if (ex is CommerceInvalidQuantitySqlException)
              throw new AccountQuantityException(HostingResources.InvalidAccountQuantities((object) resourceName), (ex as CommerceInvalidQuantitySqlException).ErrorCode);
            throw;
          }
          finally
          {
            collectionContext.TraceLeave(5104230, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (SetAccountQuantity));
          }
        }
      }
      throw new ArgumentOutOfRangeException(nameof (includedQuantity));
    }

    public void SetAccountQuantity(
      IVssRequestContext collectionContext,
      string resourceName,
      int? includedQuantity,
      int? maximumQuantity)
    {
      this.SetAccountQuantity(collectionContext, resourceName, ResourceRenewalGroup.Monthly, includedQuantity, maximumQuantity);
    }

    public void DecreaseResourceQuantity(
      IVssRequestContext requestContext,
      Guid? azureSubscriptionId,
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      bool shouldBeImmediate)
    {
      requestContext.CheckHostedDeployment();
      requestContext.TraceEnter(5109001, "Commerce", nameof (PlatformOfferSubscriptionService), new object[5]
      {
        (object) azureSubscriptionId,
        (object) offerMeterName,
        (object) renewalGroup,
        (object) quantity,
        (object) shouldBeImmediate
      }, nameof (DecreaseResourceQuantity));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(offerMeterName, nameof (offerMeterName));
        OfferMeter meterConfig = this.GetValidMeter(requestContext, offerMeterName);
        if (quantity < 0)
          throw new ArgumentOutOfRangeException(nameof (quantity));
        if (quantity < meterConfig.IncludedQuantity)
          throw new ArgumentException(string.Format("Cannot decrease quantity to {0} since it's below the included quantity: {1}", (object) quantity, (object) meterConfig.IncludedQuantity), nameof (quantity));
        bool flag = true;
        if (requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        {
          ArgumentUtility.CheckForEmptyGuid(azureSubscriptionId, nameof (azureSubscriptionId));
          if (meterConfig.Category != MeterCategory.Bundle)
            throw new ArgumentException(string.Format("At deployment level expected {0} category for {1}, but found {2}.", (object) MeterCategory.Bundle, (object) offerMeterName, (object) meterConfig.Category));
          flag = false;
        }
        else
        {
          requestContext.CheckProjectCollectionRequestContext();
          if (meterConfig.Category == MeterCategory.Bundle)
            throw new ArgumentException(string.Format("At collection level expected a category other than {0} for {1}.", (object) meterConfig.Category, (object) offerMeterName));
        }
        requestContext.Trace(5109019, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("HostType: {0}, OfferMeterName: {1}, RenewalGroup: {2}, ", (object) requestContext.ServiceHost.HostType, (object) offerMeterName, (object) renewalGroup) + string.Format("AzureSubscriptionId: {0}, MeterCategory: {1}, Quantity: {2}", (object) azureSubscriptionId, (object) meterConfig.Category, (object) quantity));
        try
        {
          if (flag)
            this.DecreaseResourceQuantityInternal(requestContext, azureSubscriptionId, meterConfig, renewalGroup, quantity, shouldBeImmediate);
          else
            InfrastructureHostHelper.WithInfrastructureCollectionContext(requestContext.Elevate(), azureSubscriptionId.Value, requestContext.GetUserIdentity(), (Action<IVssRequestContext>) (collectionSystemContext =>
            {
              InfrastructureHostHelper.CopyRequestContextItems(requestContext, collectionSystemContext);
              this.DecreaseResourceQuantityInternal(collectionSystemContext, azureSubscriptionId, meterConfig, renewalGroup, quantity, shouldBeImmediate);
            }));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5109002, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
          throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(5109003, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (DecreaseResourceQuantity));
      }
    }

    internal void DecreaseResourceQuantityInternal(
      IVssRequestContext collectionContext,
      Guid? azureSubscriptionId,
      OfferMeter meterConfig,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      bool shouldBeImmediate)
    {
      this.CheckPermissionCollectionContext(collectionContext, 4, "/OfferSubscriptionCommittedQuantity");
      this.CheckMigrationStatus(collectionContext);
      OfferSubscriptionInternal meterUsage = this.GetMeterUsages(collectionContext, new int?(meterConfig.MeterId), new ResourceRenewalGroup?(renewalGroup), false).FirstOrDefault<OfferSubscriptionInternal>();
      if (meterUsage.IsDefaultEmptyEntry)
        throw new InvalidResourceException(string.Format("No resource usage found for MeterId: {0}, RenewalGroup: {1}", (object) meterConfig.MeterId, (object) renewalGroup));
      int committedQuantity = shouldBeImmediate ? quantity : meterUsage.CommittedQuantity;
      if (committedQuantity > meterUsage.CommittedQuantity)
        throw new AccountQuantityException("Cannot increase committed (immediate) quantity " + string.Format("from {0} to {1}.", (object) meterUsage.CommittedQuantity, (object) quantity));
      if (quantity > meterUsage.CurrentQuantity)
        throw new AccountQuantityException("Cannot increase current (non-immediate) quantity " + string.Format("from {0} to {1}.", (object) meterUsage.CurrentQuantity, (object) quantity));
      collectionContext.Trace(5109020, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("OfferMeterName: {0}, RenewalGroup: {1}, Quantity: {2}, NewCommittedQuantity {3}, ", (object) meterConfig.Name, (object) renewalGroup, (object) quantity, (object) committedQuantity) + string.Format("MeteredResourceCommmittedQuantity: {0}, MeteredResourceCurrentQuantity: {1}", (object) meterUsage.CommittedQuantity, (object) meterUsage.CurrentQuantity));
      Guid instanceId = collectionContext.ServiceHost.InstanceId;
      Guid userCUID;
      Guid guid = CommerceIdentityHelper.UserIdentityIdOrCommerceContext(collectionContext, out userCUID);
      MigrationUtilities.SetStaleOrganization(collectionContext, meterConfig.MeterId);
      int num = collectionContext.GetService<IOfferSubscriptionCachedAccessService>().UpdateCommittedAndCurrentQuantities(collectionContext, meterConfig.MeterId, (byte) renewalGroup, committedQuantity, quantity, guid);
      if (num != 1)
        throw new InvalidResourceException(string.Format("Invalid number of records updated: {0} (MeterId: {1}, RenewalGroup: {2})", (object) num, (object) meterConfig.MeterId, (object) renewalGroup));
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add(CustomerIntelligenceProperty.AzureSubscriptionId, (object) (azureSubscriptionId ?? Guid.Empty));
      eventData.Add("OfferMeterName", meterConfig.Name);
      eventData.Add("RenewalGroup", renewalGroup.ToString());
      eventData.Add("NewQuantity", (double) quantity);
      eventData.Add("ImmediateChange", shouldBeImmediate);
      eventData.Add("CommittedQuantity", (double) meterUsage.CommittedQuantity);
      eventData.Add("CurrentQuantity", (double) meterUsage.CurrentQuantity);
      eventData.Add("UserIdentityId", (object) guid);
      eventData.Add("UserIdentityCUID", (object) userCUID);
      eventData.Add(CustomerIntelligenceProperty.ActivityId, (object) collectionContext.ActivityId);
      eventData.Add("CollectionId", (object) instanceId);
      this.CheckForRequestSource(collectionContext, eventData);
      CustomerIntelligence.PublishEvent(collectionContext, "ResourceQuantityDecreased", eventData);
      AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(collectionContext.To(TeamFoundationHostType.Deployment), instanceId);
      this.TrySetOnPremisesPurchaseSource(collectionContext, azureResourceAccount);
      this.TraceBIEventsForMeteringService(collectionContext, guid, userCUID, meterUsage, 0, 0.0, committedQuantity, meterUsage.CommittedQuantity, quantity - meterUsage.CurrentQuantity, Guid.Empty, "ReducePaidQuantity");
      OfferSubscriptionInternal subscriptionInternal = this.GetMeterUsages(collectionContext, new int?(meterConfig.MeterId), new ResourceRenewalGroup?(renewalGroup), false).FirstOrDefault<OfferSubscriptionInternal>();
      DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, new int?(meterConfig.MeterId), new ResourceRenewalGroup?(renewalGroup));
      this.SendOfferSubscriptionUpgradeDowngradeMessage(collectionContext, collectionContext.GetUserIdentity().Id, subscriptionInternal.CommittedQuantity, meterUsage, azureSubscriptionId, (AggregateUsageEventResult) null, shouldBeImmediate, shouldBeImmediate);
      this.WriteOfferSubscriptionCustomerEvent(collectionContext, subscriptionInternal.Meter.GalleryId, instanceId, azureResourceAccount != null ? azureResourceAccount.AzureSubscriptionId : Guid.Empty, subscriptionInternal.CommittedQuantity, "Update");
    }

    public IEnumerable<IOfferSubscription> GetOfferSubscriptionsForAllAzureSubscription(
      IVssRequestContext requestContext,
      bool validateAzuresubscription = false,
      bool nextBillingPeriod = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5106761, "Commerce", nameof (PlatformOfferSubscriptionService), new object[2]
      {
        (object) validateAzuresubscription,
        (object) nextBillingPeriod
      }, nameof (GetOfferSubscriptionsForAllAzureSubscription));
      try
      {
        if (!requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
          return (IEnumerable<IOfferSubscription>) null;
        IEnumerable<IOfferSubscription> azureSubscription = this.GetOfferSubscriptionsForAllAssociatedAzureSubscription(requestContext, validateAzuresubscription, nextBillingPeriod);
        requestContext.TraceAlways(5106764, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), new
        {
          Msg = nameof (GetOfferSubscriptionsForAllAzureSubscription),
          nextBillingPeriod = nextBillingPeriod,
          offerSubscriptions = azureSubscription.Select(x => new
          {
            AzureSubscriptionId = x.AzureSubscriptionId,
            Name = x.OfferMeter.Name,
            RenewalGroup = x.RenewalGroup,
            CommittedQuantity = x.CommittedQuantity
          })
        }.Serialize());
        return azureSubscription;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106763, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106762, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (GetOfferSubscriptionsForAllAzureSubscription));
      }
    }

    public AccountLicenseType GetDefaultLicenseLevel(
      IVssRequestContext collectionContext,
      Guid organizationId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(collectionContext, nameof (collectionContext));
      if (collectionContext.ServiceHost.InstanceId != organizationId)
        throw new ArgumentException(string.Format("Invalid organizationId: {0}.", (object) organizationId), nameof (organizationId));
      collectionContext.TraceEnter(5109285, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (GetDefaultLicenseLevel));
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      AccountLicenseType defaultLicenseLevel = !vssRequestContext.GetService<PlatformSubscriptionService>().IsAssignmentBillingEnabled(vssRequestContext, organizationId) ? (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableStakeholderLicenceLevelByDefault") ? AccountLicenseType.Stakeholder : AccountLicenseType.Express) : (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableBasicLicenceLevelByDefault") ? AccountLicenseType.Express : AccountLicenseType.Stakeholder);
      collectionContext.TraceProperties<AccountLicenseType>(5106764, "Commerce", nameof (PlatformOfferSubscriptionService), defaultLicenseLevel, organizationId.ToString());
      return defaultLicenseLevel;
    }

    public IEnumerable<IOfferSubscription> GetOfferSubscriptionsForGalleryItem(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      string galleryItemId,
      bool nextBillingPeriod = false)
    {
      requestContext.TraceEnter(5106771, "Commerce", nameof (PlatformOfferSubscriptionService), new object[3]
      {
        (object) azureSubscriptionId,
        (object) galleryItemId,
        (object) nextBillingPeriod
      }, nameof (GetOfferSubscriptionsForGalleryItem));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckStringForNullOrEmpty(galleryItemId, nameof (galleryItemId));
        ArgumentUtility.CheckForEmptyGuid(azureSubscriptionId, nameof (azureSubscriptionId));
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        ISubscriptionAccount subscriptionAccount = this.GetSubscriptionAccountBySubscriptionId(requestContext1, azureSubscriptionId, AccountProviderNamespace.Marketplace).SingleOrDefault<ISubscriptionAccount>();
        if (subscriptionAccount == null)
          return (IEnumerable<IOfferSubscription>) null;
        IOfferMeterService service = requestContext.GetService<IOfferMeterService>();
        List<IOfferSubscription> list = this.GetOfferSubscriptionsFromAccountId(requestContext1, subscriptionAccount.AccountId, (OfferMeter) service.GetOfferMeter(requestContext1, galleryItemId), nextBillingPeriod).Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (x => x.OfferMeter.GalleryId == galleryItemId)).ToList<IOfferSubscription>();
        requestContext.TraceProperties<List<IOfferSubscription>>(5106774, "Commerce", nameof (PlatformOfferSubscriptionService), list, (string) null);
        return (IEnumerable<IOfferSubscription>) list;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106773, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106772, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (GetOfferSubscriptionsForGalleryItem));
      }
    }

    public void CreatePurchaseRequest(
      IVssRequestContext requestContext,
      PurchaseRequest purchaseRequest)
    {
      requestContext.CheckProjectCollectionRequestContext();
      requestContext.CheckHostedDeployment();
      ArgumentUtility.CheckForNull<PurchaseRequest>(purchaseRequest, nameof (purchaseRequest));
      ArgumentUtility.CheckStringForNullOrEmpty(purchaseRequest.OfferMeterName, "OfferMeterName");
      ArgumentUtility.CheckGreaterThanZero((float) purchaseRequest.Quantity, "Quantity");
      requestContext.TraceEnter(5109070, "Commerce", nameof (PlatformOfferSubscriptionService), new object[1]
      {
        (object) purchaseRequest
      }, nameof (CreatePurchaseRequest));
      try
      {
        this.CheckPermissionCollectionContext(requestContext, 1, "/Meter");
        IOfferMeter offerMeter = requestContext.GetService<IOfferMeterService>().GetOfferMeter(requestContext, purchaseRequest.OfferMeterName);
        ArgumentUtility.CheckForNull<IOfferMeter>(offerMeter, "OfferMeterName");
        ArgumentUtility.CheckForOutOfRange(purchaseRequest.Quantity, "Quantity", 1, offerMeter.MaximumQuantity);
        ArgumentUtility.CheckStringForNullOrEmpty(offerMeter.GalleryId, "GalleryId");
        IOfferSubscription offerSubscription = this.GetSubscriptionResourceInternal(requestContext.Elevate(), false, (OfferMeter) offerMeter).FirstOrDefault<IOfferSubscription>();
        if (offerSubscription == null)
          throw new InvalidOperationException(HostingResources.OfferSubscriptionNotFound());
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableCommerceNotificationEmail"))
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          if (!requestContext.IsOrganizationAadBacked())
            identity = requestContext.GetService<IdentityService>().GetPrimaryMsaIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity);
          new PurchaseRequestNotification().SendPurchaseRequestEmail(requestContext, offerMeter.Name, offerSubscription.AzureSubscriptionId, offerSubscription.RenewalGroup, purchaseRequest.Quantity, string.Empty, offerSubscription.ResetDate, new Guid?(), new Guid?(), identity ?? userIdentity, purchaseRequest.Reason);
        }
        else
          this.InvokeCommunicator((PurchaseNotificationCommunicator) new PurchaseRequestEmailCommunicator(purchaseRequest, new Guid?(offerSubscription.AzureSubscriptionId)), requestContext, offerMeter.Name, offerMeter.GalleryId, offerSubscription.AzureSubscriptionId, offerSubscription.RenewalGroup, purchaseRequest.Quantity, string.Empty, offerSubscription.ResetDate, new Guid?(), new Guid?(), requestContext.GetUserIdentity());
        Dictionary<string, object> additionalFields = new Dictionary<string, object>()
        {
          {
            CustomerIntelligenceProperty.SubscriptionId,
            (object) offerSubscription.AzureSubscriptionId
          }
        };
        try
        {
          PurchaseRequest purchaseRequest1 = JsonUtilities.Deserialize<PurchaseRequest>(purchaseRequest.Serialize<PurchaseRequest>());
          purchaseRequest1.Reason = (string) null;
          this.WritePurchaseRequestCustomerIntelligenceEvent(requestContext, nameof (CreatePurchaseRequest), purchaseRequest1, (IDictionary<string, object>) additionalFields);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5109079, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109079, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5109080, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (CreatePurchaseRequest));
      }
    }

    public void UpdatePurchaseRequest(
      IVssRequestContext requestContext,
      PurchaseRequest purchaseRequest)
    {
      requestContext.CheckProjectCollectionRequestContext();
      requestContext.CheckHostedDeployment();
      ArgumentUtility.CheckForNull<PurchaseRequest>(purchaseRequest, nameof (purchaseRequest));
      ArgumentUtility.CheckStringForNullOrEmpty(purchaseRequest.OfferMeterName, "OfferMeterName");
      ArgumentUtility.CheckGreaterThanZero((float) purchaseRequest.Quantity, "Quantity");
      requestContext.TraceEnter(5109081, "Commerce", nameof (PlatformOfferSubscriptionService), new object[1]
      {
        (object) purchaseRequest
      }, nameof (UpdatePurchaseRequest));
      try
      {
        this.CheckPermissionCollectionContext(requestContext, 4, "/Meter");
        IOfferMeter offerMeter = requestContext.GetService<IOfferMeterService>().GetOfferMeter(requestContext, purchaseRequest.OfferMeterName);
        ArgumentUtility.CheckForNull<IOfferMeter>(offerMeter, "OfferMeterName");
        ArgumentUtility.CheckForOutOfRange(purchaseRequest.Quantity, "Quantity", 1, offerMeter.MaximumQuantity);
        ArgumentUtility.CheckStringForNullOrEmpty(offerMeter.GalleryId, "GalleryId");
        requestContext.Trace(5109082, TraceLevel.Verbose, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Purchase request for host {0}, meter {1}, quantity {2} and response is {3}", (object) requestContext.ServiceHost.InstanceId, (object) purchaseRequest.OfferMeterName, (object) purchaseRequest.Quantity, (object) purchaseRequest.Response));
        this.WritePurchaseRequestCustomerIntelligenceEvent(requestContext, nameof (UpdatePurchaseRequest), purchaseRequest);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109089, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5109090, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (UpdatePurchaseRequest));
      }
    }

    private BillableEvent CreateBillableEvent(
      IVssRequestContext collectionContext,
      AggregateUsageEventResult aggregateUsageEventResult,
      UsageEvent usageEvent,
      OfferMeter offerMeter)
    {
      double quantity = aggregateUsageEventResult?.BillableEvent?.UsageResourceQuantity ?? aggregateUsageEventResult.BillableQuantity;
      return this.CreateBillableEvent(collectionContext, usageEvent.SubscriptionId, offerMeter, quantity, usageEvent.EventTimestamp, usageEvent.EventUniqueId);
    }

    private BillableEvent CreateBillableEvent(
      IVssRequestContext collectionContext,
      Guid subscriptionId,
      OfferMeter offerMeter,
      double quantity,
      DateTime eventTime,
      Guid eventId)
    {
      BillableEvent billableEvent = new BillableEvent()
      {
        AccountId = collectionContext.ServiceHost.InstanceId,
        AccountName = CollectionHelper.GetCollectionName(collectionContext),
        Quantity = quantity,
        SubscriptionId = subscriptionId,
        MeterPlatformGuid = offerMeter.PlatformMeterId,
        EventUtcTime = eventTime,
        EventUniqueId = eventId
      };
      collectionContext.TraceProperties<BillableEvent>(5108904, "Commerce", nameof (PlatformOfferSubscriptionService), billableEvent, (string) null);
      return billableEvent;
    }

    private void QueueAggregateJob(IVssRequestContext collectionContext)
    {
      this.ValidateRequestContext(collectionContext);
      Guid jobGuid = new Guid("8B78365F-452C-4AB9-86D7-DF769010DA6E");
      if (!JobHelper.DoesJobDefinitionExist(collectionContext, jobGuid))
        JobHelper.CreateJobDefinition(collectionContext, jobGuid, "UsageAggregationJobExtension", "Microsoft.VisualStudio.Services.Commerce.UsageAggregationJobExtension");
      this.QueueAggregateJobDirect(collectionContext);
    }

    private void QueueAggregateJobDirect(IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      List<TeamFoundationJobQueueEntry> source = service.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        new Guid("8B78365F-452C-4AB9-86D7-DF769010DA6E")
      });
      TeamFoundationJobQueueEntry foundationJobQueueEntry = source != null ? source.FirstOrDefault<TeamFoundationJobQueueEntry>() : (TeamFoundationJobQueueEntry) null;
      if (foundationJobQueueEntry != null && foundationJobQueueEntry.QueueTime <= this.GetUtcNow())
      {
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) foundationJobQueueEntry.QueueTime);
        service.QueueOneTimeJob(requestContext, "UsageAggregationJobExtension", "Microsoft.VisualStudio.Services.Commerce.UsageAggregationJobExtension", xml, false);
      }
      DateTime utcNow = this.GetUtcNow();
      DateTime dateTime = utcNow.AddHours(1.0);
      int maxDelaySeconds = (int) Math.Ceiling((new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 1) - utcNow).TotalSeconds);
      service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        new Guid("8B78365F-452C-4AB9-86D7-DF769010DA6E")
      }, maxDelaySeconds);
    }

    private UsageEvent CreateUsageEvent(
      IVssRequestContext collectionContext,
      Guid eventUserId,
      int quantity,
      string eventId,
      DateTime billableDate,
      OfferMeter meterConfig,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity,
      AzureSubscriptionInternal subscription)
    {
      UsageEvent usageEvent = new UsageEvent()
      {
        PartitionId = collectionContext.ServiceHost.PartitionId,
        AssociatedUser = eventUserId,
        MeterName = meterConfig.Name,
        Quantity = quantity,
        EventId = eventId,
        BillableDate = billableDate,
        EventTimestamp = this.GetUtcNow(),
        EventUniqueId = Guid.NewGuid(),
        AccountName = CollectionHelper.GetCollectionName(collectionContext),
        ServiceIdentity = serviceIdentity.MasterId,
        ResourceBillingMode = meterConfig.BillingMode,
        SubscriptionId = subscription != null ? subscription.AzureSubscriptionId : Guid.Empty,
        SubscriptionAnniversaryDay = 1,
        AccountId = collectionContext.ServiceHost.InstanceId
      };
      collectionContext.TraceProperties<UsageEvent>(5108905, "Commerce", nameof (PlatformOfferSubscriptionService), usageEvent, (string) null);
      return usageEvent;
    }

    private CommerceReportingEventType GetBillableReportingEventType(
      double newQuantity,
      double currentQuantity)
    {
      CommerceReportingEventType reportingEventType = CommerceReportingEventType.Unknown;
      if (currentQuantity == 0.0 && newQuantity > 0.0)
        reportingEventType = CommerceReportingEventType.NewPurchase;
      else if (newQuantity > currentQuantity)
        reportingEventType = CommerceReportingEventType.UpgradeQuantity;
      else if (newQuantity == currentQuantity)
        reportingEventType = CommerceReportingEventType.RenewPurchase;
      else if (newQuantity > 0.0 && newQuantity < currentQuantity)
        reportingEventType = CommerceReportingEventType.DowngradeQuantity;
      else if (newQuantity == 0.0)
        reportingEventType = CommerceReportingEventType.CancelPurchase;
      return reportingEventType;
    }

    private CustomerIntelligenceEventType GetCustomerIntelligenceEventType(
      double newQuantity,
      double currentQuantity,
      double billedQuantity,
      OfferSubscriptionInternal meterUsage)
    {
      if ((meterUsage.Meter.Name.Equals((object) ResourceName.Build) || meterUsage.Meter.Name.Equals((object) ResourceName.LoadTest) || meterUsage.Meter.Name.Equals((object) ResourceName.Artifacts)) && billedQuantity == 0.0)
        return CustomerIntelligenceEventType.FreeConsumption;
      switch (this.GetBillableReportingEventType(newQuantity, currentQuantity))
      {
        case CommerceReportingEventType.NewPurchase:
          return CustomerIntelligenceEventType.NewPurchase;
        case CommerceReportingEventType.UpgradeQuantity:
          return CustomerIntelligenceEventType.IncreasePaidQuantity;
        case CommerceReportingEventType.DowngradeQuantity:
          return CustomerIntelligenceEventType.ReducePaidQuantity;
        case CommerceReportingEventType.CancelPurchase:
          return CustomerIntelligenceEventType.CancelAll;
        case CommerceReportingEventType.RenewPurchase:
          return CustomerIntelligenceEventType.RenewPurchase;
        default:
          return CustomerIntelligenceEventType.Unknown;
      }
    }

    private string GenerateEventId(DateTime eventTime) => string.Format("{0}_{1}", (object) eventTime.Ticks.ToString("D19"), (object) Guid.NewGuid());

    private bool IsOnPrem(IVssRequestContext collectionContext) => ServiceHostTags.FromString(collectionContext.ServiceHost.ParentServiceHost.Description).HasTag(CommerceWellKnownServiceHostTags.AssociatedWithOnPremisesCollection);

    private string GetEnvironment(IVssRequestContext collectionContext) => !this.IsOnPrem(collectionContext) ? "Hosted" : "OnPremises";

    private ReportingEvent CreateBillableReportingEvent(
      IVssRequestContext collectionContext,
      Guid userId,
      double newQuantity,
      double prevQuantity,
      double billedQuantity,
      IOfferMeter meterConfig,
      OfferSubscriptionInternal meterUsage,
      DateTime resetDate,
      AzureSubscriptionInternal subscription)
    {
      DateTime utcNow = this.GetUtcNow();
      DateTime dateTime = utcNow;
      if (meterUsage.IsTrialOrPreview && meterUsage.TrialExpiryDate.HasValue)
        dateTime = meterUsage.TrialExpiryDate.Value.AddDays(1.0);
      else if (newQuantity < (double) meterUsage.CommittedQuantity)
        dateTime = resetDate;
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Application);
      ReportingEvent billableReportingEvent = new ReportingEvent()
      {
        EventId = this.GenerateEventId(utcNow),
        EventTime = utcNow,
        EventName = this.GetBillableReportingEventType(newQuantity, (double) meterUsage.CurrentQuantity).ToString(),
        OrganizationId = vssRequestContext.ServiceHost.InstanceId,
        OrganizationName = vssRequestContext.ServiceHost.Name,
        CollectionId = collectionContext.ServiceHost.InstanceId,
        CollectionName = CollectionHelper.GetCollectionName(collectionContext),
        Environment = this.GetEnvironment(collectionContext),
        UserIdentity = userId,
        ServiceIdentity = this.GetIdentity(collectionContext).MasterId,
        Version = "2.0"
      };
      billableReportingEvent.Properties.Add("EventSource", CommerceUtil.CheckForRequestSource(collectionContext));
      billableReportingEvent.Properties.Add("SubscriptionId", subscription?.AzureSubscriptionId.ToString() ?? Guid.Empty.ToString());
      billableReportingEvent.Properties.Add("MeterName", meterConfig.Name);
      billableReportingEvent.Properties.Add("GalleryId", meterConfig.GalleryId);
      billableReportingEvent.Properties.Add("CommittedQuantity", (newQuantity > (double) meterUsage.CommittedQuantity ? newQuantity : (double) meterUsage.CommittedQuantity).ToString());
      billableReportingEvent.Properties.Add("CurrentQuantity", newQuantity.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      billableReportingEvent.Properties.Add("PreviousQuantity", prevQuantity.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      billableReportingEvent.Properties.Add("BilledQuantity", billedQuantity.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      SerializableDictionary<string, string> properties1 = billableReportingEvent.Properties;
      int num = meterUsage.IncludedQuantity;
      string str1 = num.ToString();
      properties1.Add("IncludedQuantity", str1);
      SerializableDictionary<string, string> properties2 = billableReportingEvent.Properties;
      num = meterUsage.MaximumQuantity;
      string str2 = num.ToString();
      properties2.Add("MaxQuantity", str2);
      billableReportingEvent.Properties.Add("RenewalGroup", meterUsage.RenewalGroup.ToString());
      billableReportingEvent.Properties.Add("EffectiveDate", dateTime.ToString("u"));
      collectionContext.TraceProperties<ReportingEvent>(5108914, "Commerce", nameof (PlatformOfferSubscriptionService), billableReportingEvent, (string) null);
      return billableReportingEvent;
    }

    private ReportingEvent CreateTrialReportingEvent(
      IVssRequestContext collectionContext,
      CommerceReportingEventType eventType,
      OfferMeter meterConfig,
      OfferSubscriptionInternal meterUsage)
    {
      DateTime utcNow = this.GetUtcNow();
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Application);
      ReportingEvent trialReportingEvent = new ReportingEvent()
      {
        EventId = this.GenerateEventId(utcNow),
        EventTime = utcNow,
        EventName = eventType.ToString(),
        OrganizationId = vssRequestContext.ServiceHost.InstanceId,
        OrganizationName = vssRequestContext.ServiceHost.Name,
        CollectionId = collectionContext.ServiceHost.InstanceId,
        CollectionName = CollectionHelper.GetCollectionName(collectionContext.Elevate()),
        Environment = this.GetEnvironment(collectionContext),
        UserIdentity = collectionContext.GetUserIdentity().Id,
        ServiceIdentity = this.GetIdentity(collectionContext).MasterId,
        Version = "1.0"
      };
      trialReportingEvent.Properties.Add("EventSource", CommerceUtil.CheckForRequestSource(collectionContext));
      trialReportingEvent.Properties.Add("MeterName", meterConfig.Name);
      trialReportingEvent.Properties.Add("GalleryId", meterConfig.GalleryId);
      trialReportingEvent.Properties.Add("IncludedQuantity", meterUsage.IncludedQuantity.ToString());
      trialReportingEvent.Properties.Add("MaxQuantity", meterUsage.MaximumQuantity.ToString());
      trialReportingEvent.Properties.Add("RenewalGroup", meterUsage.RenewalGroup.ToString());
      DateTime? nullable;
      if (meterUsage.StartDate.HasValue)
      {
        SerializableDictionary<string, string> properties = trialReportingEvent.Properties;
        nullable = meterUsage.StartDate;
        string str = nullable.Value.ToString("u");
        properties.Add("TrialStartDate", str);
      }
      nullable = meterUsage.TrialExpiryDate;
      if (nullable.HasValue)
      {
        SerializableDictionary<string, string> properties = trialReportingEvent.Properties;
        nullable = meterUsage.TrialExpiryDate;
        string str = nullable.Value.ToString("u");
        properties.Add("TrialEndDate", str);
      }
      collectionContext.TraceProperties<ReportingEvent>(5108914, "Commerce", nameof (PlatformOfferSubscriptionService), trialReportingEvent, (string) null);
      return trialReportingEvent;
    }

    private ReportingEvent CreateQuantityChangeReportingEvent(
      IVssRequestContext collectionContext,
      OfferMeter meterConfig,
      OfferSubscriptionInternal meterUsage,
      OfferSubscriptionInternal prevMeterUsage)
    {
      DateTime utcNow = this.GetUtcNow();
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Application);
      ReportingEvent changeReportingEvent = new ReportingEvent()
      {
        EventId = this.GenerateEventId(utcNow),
        EventTime = utcNow,
        EventName = "QuantityChange",
        OrganizationId = vssRequestContext.ServiceHost.InstanceId,
        OrganizationName = vssRequestContext.ServiceHost.Name,
        CollectionId = collectionContext.ServiceHost.InstanceId,
        CollectionName = CollectionHelper.GetCollectionName(collectionContext.Elevate()),
        Environment = this.GetEnvironment(collectionContext),
        UserIdentity = collectionContext.GetUserIdentity().Id,
        ServiceIdentity = this.GetIdentity(collectionContext).MasterId,
        Version = "1.0"
      };
      changeReportingEvent.Properties.Add("EventSource", CommerceUtil.CheckForRequestSource(collectionContext));
      changeReportingEvent.Properties.Add("MeterName", meterConfig.Name);
      changeReportingEvent.Properties.Add("GalleryId", meterConfig.GalleryId);
      changeReportingEvent.Properties.Add("IncludedQuantity", meterUsage.IncludedQuantity.ToString());
      changeReportingEvent.Properties.Add("MaxQuantity", meterUsage.MaximumQuantity.ToString());
      changeReportingEvent.Properties.Add("PreviousIncludedQuantity", prevMeterUsage.IncludedQuantity.ToString());
      changeReportingEvent.Properties.Add("PreviousMaxQuantity", prevMeterUsage.MaximumQuantity.ToString());
      changeReportingEvent.Properties.Add("RenewalGroup", meterUsage.RenewalGroup.ToString());
      collectionContext.TraceProperties<ReportingEvent>(5108914, "Commerce", nameof (PlatformOfferSubscriptionService), changeReportingEvent, (string) null);
      return changeReportingEvent;
    }

    internal virtual void SaveBillableReportingEvent(
      IVssRequestContext collectionContext,
      Guid userId,
      double newQuantity,
      double prevQuantity,
      double billedQuantity,
      IOfferMeter meterConfig,
      OfferSubscriptionInternal meterUsage,
      DateTime resetDate,
      AzureSubscriptionInternal subscription)
    {
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableSaveReportingEvents"))
        return;
      ReportingEvent billableReportingEvent = this.CreateBillableReportingEvent(collectionContext, userId, newQuantity, prevQuantity, billedQuantity, meterConfig, meterUsage, resetDate, subscription);
      this.SaveReportingEvent(collectionContext, billableReportingEvent);
    }

    internal virtual void SaveTrialReportingEvent(
      IVssRequestContext collectionContext,
      CommerceReportingEventType trialEventType,
      OfferMeter meterConfig,
      OfferSubscriptionInternal meterUsage)
    {
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableSaveReportingEvents"))
        return;
      ReportingEvent trialReportingEvent = this.CreateTrialReportingEvent(collectionContext, trialEventType, meterConfig, meterUsage);
      this.SaveReportingEvent(collectionContext, trialReportingEvent);
    }

    internal virtual void SaveQuantityChangeReportingEvent(
      IVssRequestContext collectionContext,
      OfferMeter meterConfig,
      OfferSubscriptionInternal meterUsage,
      OfferSubscriptionInternal prevMeterUsage)
    {
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableSaveReportingEvents"))
        return;
      ReportingEvent changeReportingEvent = this.CreateQuantityChangeReportingEvent(collectionContext, meterConfig, meterUsage, prevMeterUsage);
      this.SaveReportingEvent(collectionContext, changeReportingEvent);
    }

    private void SaveReportingEvent(
      IVssRequestContext collectionContext,
      ReportingEvent reportingEvent)
    {
      if (CommerceUtil.IsRunningOnCommerceServiceAsBackup(collectionContext))
        return;
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IReportingService>().SaveReportingEvent(vssRequestContext, reportingEvent);
    }

    internal virtual double SaveBillableEvents(
      IVssRequestContext collectionContext,
      IOfferSubscription resource,
      OfferMeter meterConfig,
      AzureSubscriptionInternal subscription,
      BillableEvent billableEvent,
      OfferSubscriptionInternal meterUsage,
      ResourceRenewalGroup renewalGroup,
      AzureResourceAccount account)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      DateTime utcNow = this.GetUtcNow();
      if (billableEvent != null && utcNow < resource.ResetDate)
      {
        if (billableEvent.Quantity <= 0.0)
        {
          collectionContext.Trace(5104234, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Not raising the billing event because the prorated quantity is {0}", (object) billableEvent.Quantity));
          return 0.0;
        }
        if (subscription != null && subscription.AzureSubscriptionSource == SubscriptionSource.FreeTier)
        {
          collectionContext.Trace(5104232, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "The given subscription is free tier. Hence not reporting free tier");
          return 0.0;
        }
        if (billableEvent.MeterPlatformGuid == Guid.Empty)
        {
          collectionContext.Trace(5104235, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Not raising the billing event because the meter guid is empty for " + resource.OfferMeter.Name);
          return 0.0;
        }
        if (meterConfig.RenewalFrequency == MeterRenewalFrequecy.Annually && meterUsage.LastResetDate.Year != utcNow.Year && renewalGroup == resource.RenewalGroup && meterUsage.LastResetDate != new DateTime())
        {
          collectionContext.TraceAlways(5104236, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Not raising the billing event for changed quantity because reset date is {0}. AccountId: {1}, ", (object) resource.ResetDate, (object) collectionContext.ServiceHost.InstanceId) + string.Format("SubscriptionId: {0}, ResourceName: {1}, RenewalGroup: {2}.", (object) subscription.AzureSubscriptionId, (object) meterUsage.Meter.Name, (object) renewalGroup));
          return 0.0;
        }
        if (meterConfig.RenewalFrequency != MeterRenewalFrequecy.Annually && meterConfig.BillingMode != ResourceBillingMode.PayAsYouGo && utcNow.Day == 1 && meterUsage.LastResetDate != new DateTime() && meterUsage.LastResetDate.Month < utcNow.Month && meterUsage.LastResetDate < utcNow)
        {
          collectionContext.TraceAlways(5104233, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Not sending the billing event to azure because reset job has not run yet. Last Reset date is {0} and calculated reset date is {1}.", (object) meterUsage.LastResetDate, (object) resource.ResetDate) + string.Format("AccountId: {0}, SubscriptionId: {1}, ResourceName: {2}, RenewalGroup: {3}.", (object) collectionContext.ServiceHost.InstanceId, (object) subscription.AzureSubscriptionId, (object) meterUsage.Meter.Name, (object) renewalGroup));
          return 0.0;
        }
        if (billableEvent.SubscriptionId != Guid.Empty)
        {
          if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableNewBillingPipeline"))
          {
            IVssRequestContext deploymentContext = collectionContext.To(TeamFoundationHostType.Deployment);
            IUsageEventsStore eventsStoreInstance = this.GetUsageEventsStoreInstance(deploymentContext);
            string location = string.IsNullOrEmpty(account.AzureGeoRegion) ? this.GetAzureRegion(collectionContext) : account.AzureGeoRegion;
            IVssRequestContext requestContext = deploymentContext;
            AzureBillableEvent2 billableEvent1 = new AzureBillableEvent2(billableEvent, location, account.GetArmResourceUri(), CsmUtilities.GetAccountTags(collectionContext, account));
            eventsStoreInstance.SaveBillableEvent<AzureBillableEvent2>(requestContext, billableEvent1);
            collectionContext.Trace(5104215, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "UsageAggregationJobExtension saved 1 Azure billable events in to table storage ");
          }
          return billableEvent.Quantity;
        }
      }
      return 0.0;
    }

    internal virtual string GetAzureRegion(IVssRequestContext collectionContext) => collectionContext.GetExtension<ICommerceRegionHandler>().GetRegionFromCollectionContext(collectionContext);

    internal virtual void WritePurchaseRequestCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      string feature,
      PurchaseRequest purchaseRequest,
      IDictionary<string, object> additionalFields = null)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "OfferMeterName",
          (object) purchaseRequest.OfferMeterName
        },
        {
          "Quantity",
          (object) purchaseRequest.Quantity
        },
        {
          "Reason",
          (object) purchaseRequest.Reason
        },
        {
          "Response",
          (object) purchaseRequest.Response
        },
        {
          "vsid",
          (object) requestContext.GetUserId()
        }
      };
      if (additionalFields != null)
        dictionary.AddRange<KeyValuePair<string, object>, Dictionary<string, object>>((IEnumerable<KeyValuePair<string, object>>) additionalFields);
      CustomerIntelligence.PublishEvent(requestContext, feature, new CustomerIntelligenceData((IDictionary<string, object>) dictionary));
    }

    internal virtual void InvokeCommunicator(
      PurchaseNotificationCommunicator communicator,
      IVssRequestContext collectionContext,
      string offerMeterName,
      string galleryId,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string offerCode,
      DateTime renewalDate,
      Guid? tenantId,
      Guid? objectId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      communicator.SendPurchaseNotificationEmail(collectionContext, offerMeterName, galleryId, azureSubscriptionId, renewalGroup, quantity, offerCode, renewalDate, tenantId, objectId, identity);
    }

    private void ValidateRequestContext(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    internal virtual IEnumerable<IOfferSubscription> GetOfferSubscriptionsFromAccountId(
      IVssRequestContext requestContext,
      Guid accountId,
      OfferMeter meterConfig = null,
      bool nextBillingPeriod = false)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IEnumerable<IOfferSubscription> offerSubscriptions = (IEnumerable<IOfferSubscription>) null;
      Guid hostId = accountId;
      Action<IVssRequestContext> action = (Action<IVssRequestContext>) (newRequestContext => offerSubscriptions = this.GetSubscriptionResourceInternal(newRequestContext, nextBillingPeriod));
      RequestContextType? requestContextType = new RequestContextType?(RequestContextType.SystemContext);
      CollectionHelper.WithCollectionContext(requestContext1, hostId, action, requestContextType, nameof (GetOfferSubscriptionsFromAccountId));
      return offerSubscriptions;
    }

    internal virtual IEnumerable<IOfferSubscription> GetOfferSubscriptionsForAllAssociatedAzureSubscription(
      IVssRequestContext requestContext,
      bool validateAzuresubscription,
      bool nextBillingPeriod)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IEnumerable<AzureSubscriptionInfo> azureSubscriptions = vssRequestContext.GetService<IAzureResourceHelper>().GetAzureSubscriptions(vssRequestContext, validateAzuresubscription, new Guid?());
      List<IOfferSubscription> azureSubscription1 = new List<IOfferSubscription>();
      foreach (AzureSubscriptionInfo subscriptionInfo in azureSubscriptions)
      {
        AzureSubscriptionInfo azureSubscription = subscriptionInfo;
        IEnumerable<ISubscriptionAccount> bySubscriptionId = this.GetSubscriptionAccountBySubscriptionId(requestContext, azureSubscription.SubscriptionId, AccountProviderNamespace.Marketplace);
        if (bySubscriptionId != null)
          azureSubscription1.AddRange(bySubscriptionId.SelectMany<ISubscriptionAccount, IOfferSubscription>((Func<ISubscriptionAccount, IEnumerable<IOfferSubscription>>) (subscriptionAccount => this.GetOfferSubscriptionsFromAccountId(requestContext, subscriptionAccount.AccountId, nextBillingPeriod: nextBillingPeriod).Select<IOfferSubscription, IOfferSubscription>((Func<IOfferSubscription, IOfferSubscription>) (offerSubscription => PlatformOfferSubscriptionService.BuildOfferSubscription(offerSubscription, azureSubscription))))));
      }
      return (IEnumerable<IOfferSubscription>) azureSubscription1;
    }

    private static IOfferSubscription BuildOfferSubscription(
      IOfferSubscription offerSubscription,
      AzureSubscriptionInfo subscription)
    {
      offerSubscription.AzureSubscriptionId = subscription.SubscriptionId;
      offerSubscription.AzureSubscriptionName = subscription.DisplayName;
      offerSubscription.AzureSubscriptionState = subscription.Status;
      ((OfferSubscription) offerSubscription).IsUseable = offerSubscription.AzureSubscriptionState == SubscriptionStatus.Active && subscription.IsAdministrator;
      return offerSubscription;
    }

    internal virtual IEnumerable<ISubscriptionAccount> GetSubscriptionAccountBySubscriptionId(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      AccountProviderNamespace providerNamespace)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IEnumerable<ISubscriptionAccount> accounts = context.GetService<PlatformSubscriptionService>().GetAccounts(context.Elevate(), azureSubscriptionId, providerNamespace);
      requestContext.TraceAlways(5109364, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), new
      {
        Msg = nameof (GetSubscriptionAccountBySubscriptionId),
        azureSubscriptionId = azureSubscriptionId,
        providerNamespace = providerNamespace,
        subscriptionAccount_count = accounts.Count<ISubscriptionAccount>(),
        subscriptionAccounts = accounts.Select(x => new
        {
          AccountId = x.AccountId,
          AccountName = x.AccountName
        })
      }.Serialize());
      return accounts;
    }

    [ExcludeFromCodeCoverage]
    internal virtual IUsageEventsStore GetUsageEventsStoreInstance(
      IVssRequestContext deploymentContext)
    {
      if (this.usageEventStore == null)
        this.usageEventStore = Activator.CreateInstance(this.usageEventStoreProviderType, (object) deploymentContext) as IUsageEventsStore;
      return this.usageEventStore;
    }

    internal virtual MeterResetEvents ResetResourceUsage(
      IVssRequestContext collectionContext,
      bool monthlyReset,
      bool isCurrentQuantitiesResetOnly = false)
    {
      this.ValidateRequestContext(collectionContext);
      this.CheckMigrationStatus(collectionContext);
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      AzureSubscriptionInternal subscription = (AzureSubscriptionInternal) null;
      AzureResourceAccount azureResourceAccount = (AzureResourceAccount) null;
      if (monthlyReset)
      {
        PlatformSubscriptionService service = vssRequestContext.GetService<PlatformSubscriptionService>();
        azureResourceAccount = service.GetAzureResourceAccountByCollectionId(vssRequestContext, collectionContext.ServiceHost.InstanceId, true);
        if (azureResourceAccount != null)
          subscription = service.GetAzureSubscription(vssRequestContext, azureResourceAccount.AzureSubscriptionId);
      }
      IEnumerable<IOfferMeter> offerMeters = collectionContext.GetService<IOfferMeterService>().GetOfferMeters(collectionContext);
      IEnumerable<OfferSubscriptionInternal> meterUsages = this.GetMeterUsages(collectionContext, new int?(), new ResourceRenewalGroup?(), false);
      List<KeyValuePair<int, int>> list1 = meterUsages.Where<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (m => m.IncludedQuantity > 0)).Select<OfferSubscriptionInternal, KeyValuePair<int, int>>((Func<OfferSubscriptionInternal, KeyValuePair<int, int>>) (x => new KeyValuePair<int, int>(x.MeterId, x.IncludedQuantity))).ToList<KeyValuePair<int, int>>();
      List<KeyValuePair<int, string>> list2 = offerMeters.Where<IOfferMeter>((Func<IOfferMeter, bool>) (m => m.BillingMode == ResourceBillingMode.PayAsYouGo)).Select<IOfferMeter, KeyValuePair<int, string>>((Func<IOfferMeter, KeyValuePair<int, string>>) (x => new KeyValuePair<int, string>(x.MeterId, x.BillingMode.ToString().Substring(0, 1)))).ToList<KeyValuePair<int, string>>();
      MigrationUtilities.SetStaleOrganization(collectionContext, meterUsages.Select<OfferSubscriptionInternal, int>((Func<OfferSubscriptionInternal, int>) (x => x.MeterId)), true);
      MeterResetEvents accountReset = collectionContext.GetService<IOfferSubscriptionCachedAccessService>().ResetResourceUsage(collectionContext, monthlyReset, subscription != null ? subscription.AzureSubscriptionId : Guid.Empty, (IEnumerable<KeyValuePair<int, int>>) list1, (IEnumerable<KeyValuePair<int, string>>) list2, isCurrentQuantitiesResetOnly, this.GetUtcNow());
      DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext);
      if (subscription != null && subscription.AzureSubscriptionStatusId != SubscriptionStatus.Active)
      {
        accountReset.BillableEvents = (IEnumerable<BillableEvent>) null;
        foreach (DowngradedResource downgradedResource in accountReset.DowngradedResources)
          downgradedResource.NewQuantity = 0;
      }
      try
      {
        if (accountReset.RenewedOfferSubscriptions != null)
        {
          Guid id = collectionContext.GetUserIdentity().Id;
          foreach (OfferSubscriptionInternal offerSubscription in accountReset.RenewedOfferSubscriptions)
          {
            OfferSubscriptionInternal meterUsage = offerSubscription;
            IOfferMeter meterConfig = offerMeters.SingleOrDefault<IOfferMeter>((Func<IOfferMeter, bool>) (m => m.MeterId == meterUsage.MeterId));
            OfferSubscriptionInternal subscriptionInternal = meterUsages.SingleOrDefault<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (x => x.MeterId == meterUsage.MeterId && x.RenewalGroup == meterUsage.RenewalGroup));
            int committedQuantity = subscriptionInternal != null ? subscriptionInternal.CommittedQuantity : 0;
            this.SaveBillableReportingEvent(collectionContext, id, (double) meterUsage.CurrentQuantity, (double) committedQuantity, (double) (meterUsage.CurrentQuantity - meterUsage.IncludedQuantity), meterConfig, meterUsage, this.GetUtcNow(), subscription);
          }
        }
        if (isCurrentQuantitiesResetOnly)
        {
          Guid id = collectionContext.GetUserIdentity().Id;
          foreach (OfferSubscriptionInternal subscriptionInternal in meterUsages.Where<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (m => m.CommittedQuantity > m.IncludedQuantity)))
          {
            OfferSubscriptionInternal meterUsage = subscriptionInternal;
            IOfferMeter meterConfig = offerMeters.SingleOrDefault<IOfferMeter>((Func<IOfferMeter, bool>) (m => m.MeterId == meterUsage.MeterId));
            this.SaveBillableReportingEvent(collectionContext, id, 0.0, (double) meterUsage.CommittedQuantity, 0.0, meterConfig, meterUsage, this.GetUtcNow(), subscription);
          }
        }
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5105780, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
      }
      if (!accountReset.DowngradedResources.IsNullOrEmpty<DowngradedResource>())
      {
        collectionContext.Trace(5105706, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Started publishing notifications");
        this.SendAccountRightsNotifications(collectionContext, offerMeters, accountReset);
        if (monthlyReset)
          this.BillingMessageHelper.CreateOfferSubscriptionDowngradeMessage(collectionContext, accountReset.DowngradedResources, azureResourceAccount == null ? new Guid() : azureResourceAccount.AzureSubscriptionId, collectionContext.ServiceHost.InstanceId);
        collectionContext.Trace(5105706, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Completed publishing notifications");
      }
      return accountReset;
    }

    internal void SendAccountRightsNotifications(
      IVssRequestContext collectionContext,
      IEnumerable<IOfferMeter> meterConfig,
      MeterResetEvents accountReset)
    {
      IVssRequestContext deploymentContext = collectionContext.To(TeamFoundationHostType.Deployment);
      Guid organizationId = collectionContext.ServiceHost.ParentServiceHost.InstanceId;
      PlatformSubscriptionService subscriptionService = deploymentContext.GetService<PlatformSubscriptionService>();
      meterConfig.Join(accountReset.DowngradedResources, (Func<IOfferMeter, int>) (m => m.MeterId), (Func<DowngradedResource, int>) (d => d.MeterId), (m, d) => new
      {
        NewQuantity = d.NewQuantity,
        Category = m.Category,
        GalleryId = m.GalleryId
      }).ToList().ForEach(item =>
      {
        if (item.Category == MeterCategory.Legacy)
        {
          subscriptionService.PublishAccountRightsNotification(deploymentContext, 5105705, new AccountRightsChangedEvent(organizationId, this.GetUtcNow())
          {
            ChangeType = AccountRightsChangeType.Account
          }, new Guid?(organizationId));
        }
        else
        {
          if (item.Category != MeterCategory.Extension)
            return;
          subscriptionService.PublishAccountRightsNotification(deploymentContext, 5105705, new AccountRightsChangedEvent(organizationId, this.GetUtcNow())
          {
            ChangeType = AccountRightsChangeType.Extension,
            ExtensionId = item.GalleryId,
            NewQuantity = item.NewQuantity
          }, new Guid?(organizationId));
        }
      });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal virtual IEnumerable<IOfferSubscription> GetSubscriptionResourceInternal(
      IVssRequestContext collectionContext,
      bool nextBillingPeriod,
      OfferMeter meterConfig,
      ResourceRenewalGroup renewalGroup)
    {
      IEnumerable<OfferSubscriptionInternal> meterUsages = this.GetMeterUsages(collectionContext, new int?(meterConfig.MeterId), new ResourceRenewalGroup?(renewalGroup), false);
      return this.GetSubscriptionResourceInternal(collectionContext, nextBillingPeriod, meterUsages);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal virtual IEnumerable<IOfferSubscription> GetSubscriptionResourceInternal(
      IVssRequestContext collectionContext,
      bool nextBillingPeriod,
      OfferMeter meterConfig)
    {
      IEnumerable<OfferSubscriptionInternal> meterUsages = this.GetMeterUsages(collectionContext, new int?(meterConfig.MeterId), new ResourceRenewalGroup?(), false);
      return this.GetSubscriptionResourceInternal(collectionContext, nextBillingPeriod, meterUsages);
    }

    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal virtual IEnumerable<IOfferSubscription> GetSubscriptionResourceInternal(
      IVssRequestContext collectionContext,
      bool nextBillingPeriod)
    {
      IEnumerable<OfferSubscriptionInternal> meterUsages = this.GetMeterUsages(collectionContext, new int?(), new ResourceRenewalGroup?(), false);
      return this.GetSubscriptionResourceInternal(collectionContext, nextBillingPeriod, meterUsages);
    }

    internal virtual IEnumerable<IOfferSubscription> GetSubscriptionResourceInternal(
      IVssRequestContext collectionContext,
      bool nextBillingPeriod,
      IEnumerable<OfferSubscriptionInternal> meteredResources)
    {
      if (!collectionContext.ExecutionEnvironment.IsHostedDeployment)
        return (IEnumerable<IOfferSubscription>) meteredResources.Select<OfferSubscriptionInternal, OfferSubscription>((Func<OfferSubscriptionInternal, OfferSubscription>) (m => m.ToOfferSubscription(nextBillingPeriod))).ToList<OfferSubscription>();
      AzureResourceAccount azureResourceAccount;
      AzureSubscriptionInternal azureSubscription;
      this.GetAzureResourceAccountAndSubscription(collectionContext, out azureResourceAccount, out azureSubscription);
      return this.GetSubscriptionResourceInternal(collectionContext, meteredResources, azureResourceAccount, azureSubscription, nextBillingPeriod);
    }

    internal virtual IEnumerable<IOfferSubscription> GetSubscriptionResourceInternal(
      IVssRequestContext collectionContext,
      IEnumerable<OfferSubscriptionInternal> meteredResources,
      AzureResourceAccount azureResourceAccount,
      AzureSubscriptionInternal azureSubscription,
      bool nextBillingPeriod)
    {
      this.ValidateRequestContext(collectionContext);
      collectionContext.Trace(5108697, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Applying business rules on MeteredResources");
      return (IEnumerable<IOfferSubscription>) meteredResources.Select<OfferSubscriptionInternal, OfferSubscription>((Func<OfferSubscriptionInternal, OfferSubscription>) (resource =>
      {
        DateTime calendarDayResetTime = this.GetNextCalendarDayResetTime(collectionContext, azureResourceAccount, resource.RenewalGroup, new DateTime?(resource.LastResetDate));
        return this.CreateSubscriptionResource(collectionContext, resource, nextBillingPeriod, azureResourceAccount, azureSubscription, calendarDayResetTime);
      })).ToList<OfferSubscription>();
    }

    internal virtual IEnumerable<OfferSubscriptionInternal> GetMeterUsages(
      IVssRequestContext collectionContext,
      int? meterId,
      ResourceRenewalGroup? renewalGroup,
      bool readDirectlyFromDatabase)
    {
      this.ValidateRequestContext(collectionContext);
      IEnumerable<OfferSubscriptionInternal> meterUsages = collectionContext.GetService<IOfferSubscriptionCachedAccessService>().GetOfferSubscriptions(collectionContext, meterId, !readDirectlyFromDatabase).Where<OfferSubscriptionInternal>((Func<OfferSubscriptionInternal, bool>) (m =>
      {
        if (!renewalGroup.HasValue)
          return true;
        int renewalGroup1 = (int) m.RenewalGroup;
        ResourceRenewalGroup? nullable = renewalGroup;
        int valueOrDefault = (int) nullable.GetValueOrDefault();
        return renewalGroup1 == valueOrDefault & nullable.HasValue;
      }));
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      IOfferMeterService service = vssRequestContext.GetService<IOfferMeterService>();
      if (!meterUsages.Any<OfferSubscriptionInternal>())
      {
        if (meterId.HasValue)
        {
          OfferSubscriptionInternal subscriptionInternal = service.GetOfferMeters(vssRequestContext).Where<IOfferMeter>((Func<IOfferMeter, bool>) (m => m.MeterId == meterId.Value)).Select<IOfferMeter, OfferSubscriptionInternal>((Func<IOfferMeter, OfferSubscriptionInternal>) (m => m.ToDefaultMeterUsage())).FirstOrDefault<OfferSubscriptionInternal>();
          if (subscriptionInternal != null && renewalGroup.HasValue)
            subscriptionInternal.RenewalGroup = renewalGroup.Value;
          return (IEnumerable<OfferSubscriptionInternal>) new OfferSubscriptionInternal[1]
          {
            subscriptionInternal
          };
        }
        meterUsages = (IEnumerable<OfferSubscriptionInternal>) service.GetOfferMeters(vssRequestContext).Where<IOfferMeter>((Func<IOfferMeter, bool>) (m => m.Category == MeterCategory.Legacy || m.IncludedQuantity > 0)).Select<IOfferMeter, OfferSubscriptionInternal>((Func<IOfferMeter, OfferSubscriptionInternal>) (m => m.ToDefaultMeterUsage())).ToArray<OfferSubscriptionInternal>();
      }
      else if (!meterId.HasValue)
      {
        IEnumerable<OfferSubscriptionInternal> source = service.GetOfferMeters(vssRequestContext).Where<IOfferMeter>((Func<IOfferMeter, bool>) (m => m.Category == MeterCategory.Legacy || m.IncludedQuantity > 0)).Select<IOfferMeter, OfferSubscriptionInternal>((Func<IOfferMeter, OfferSubscriptionInternal>) (m => m.ToDefaultMeterUsage()));
        HashSet<int> alreadyReadUsageIds = new HashSet<int>(meterUsages.Select<OfferSubscriptionInternal, int>((Func<OfferSubscriptionInternal, int>) (m => m.MeterId)));
        Func<OfferSubscriptionInternal, bool> predicate = (Func<OfferSubscriptionInternal, bool>) (m => !alreadyReadUsageIds.Contains(m.MeterId));
        OfferSubscriptionInternal[] array = source.Where<OfferSubscriptionInternal>(predicate).ToArray<OfferSubscriptionInternal>();
        meterUsages = meterUsages.Concat<OfferSubscriptionInternal>((IEnumerable<OfferSubscriptionInternal>) array);
      }
      return meterUsages;
    }

    internal virtual IEnumerable<OfferSubscriptionInternal> GetMeterUsagesForSubscription(
      IVssRequestContext requestContext,
      int? meterId,
      Guid azureSubscriptionId,
      ResourceRenewalGroup? renewalGroup,
      bool readDirectlyFromDatabase)
    {
      ArgumentUtility.CheckForEmptyGuid(azureSubscriptionId, nameof (azureSubscriptionId));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      AzureResourceAccount azureResourceAccount = vssRequestContext.GetService<PlatformSubscriptionService>().GetAzureResourceAccountsBySubscriptionIdFromDatabase(vssRequestContext, azureSubscriptionId, AccountProviderNamespace.Marketplace).FirstOrDefault<AzureResourceAccount>();
      IEnumerable<OfferSubscriptionInternal> meterUsages = Enumerable.Empty<OfferSubscriptionInternal>();
      if (azureResourceAccount != null)
        CollectionHelper.WithCollectionContext(requestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext => meterUsages = this.GetMeterUsages(collectionContext, meterId, renewalGroup, readDirectlyFromDatabase)), method: nameof (GetMeterUsagesForSubscription));
      return meterUsages;
    }

    internal virtual OfferSubscription CreateSubscriptionResource(
      IVssRequestContext collectionContext,
      OfferSubscriptionInternal meteredResource,
      bool nextBillingPeriod,
      AzureResourceAccount azureResourceAccount,
      AzureSubscriptionInternal azureSubscription,
      DateTime resetDate)
    {
      this.ValidateRequestContext(collectionContext);
      if (meteredResource.IncludedQuantity == -1)
        meteredResource.IncludedQuantity = meteredResource.Meter.IncludedQuantity;
      if (meteredResource.MaximumQuantity == -1)
        meteredResource.MaximumQuantity = meteredResource.Meter.MaximumQuantity;
      if (meteredResource.Meter.BillingMode == ResourceBillingMode.PayAsYouGo && this.IsPayAsYouGoMonthlyMeterResetRequired(meteredResource.LastResetDate))
      {
        meteredResource.CurrentQuantity = 0;
        meteredResource.CommittedQuantity = 0;
      }
      collectionContext.GetService<IResourceQuantityUpdaterService>().GetInternalAccountResourceQuantities(collectionContext, meteredResource);
      OfferSubscription offerSubscription = meteredResource.ToOfferSubscription(nextBillingPeriod);
      offerSubscription.ResetDate = resetDate;
      new OfferSubscriptionManipulator().ManipulateOfferSubscription(collectionContext, offerSubscription, azureSubscription);
      if (azureSubscription != null && azureSubscription.AzureSubscriptionStatusId == SubscriptionStatus.Active)
      {
        offerSubscription.AzureSubscriptionId = azureSubscription.AzureSubscriptionId;
        offerSubscription.AzureSubscriptionState = azureSubscription.AzureSubscriptionStatusId;
        offerSubscription.IsUseable = true;
      }
      else
      {
        if (meteredResource.CommittedQuantity > 0 && meteredResource.Meter.BillingMode == ResourceBillingMode.Committment || meteredResource.Meter.BillingMode == ResourceBillingMode.PayAsYouGo)
          offerSubscription.IsUseable = true;
        offerSubscription.IsPaidBillingEnabled = false;
      }
      if (this.IsInternalBuildMeter(meteredResource.Meter.Name) && azureSubscription != null && this.IsAccountInternal(collectionContext))
      {
        meteredResource.IsPaidBillingEnabled = true;
        offerSubscription.IsPaidBillingEnabled = true;
      }
      if (this.IsArtifactMeter(meteredResource.Meter.Name) && azureSubscription != null && this.IsAccountInternal(collectionContext))
      {
        meteredResource.IsPaidBillingEnabled = true;
        offerSubscription.IsPaidBillingEnabled = true;
      }
      if (offerSubscription.IsUseable && meteredResource.CurrentQuantity >= meteredResource.IncludedQuantity && meteredResource.CurrentQuantity < meteredResource.MaximumQuantity && meteredResource.Meter.BillingMode == ResourceBillingMode.PayAsYouGo)
      {
        if (azureSubscription == null)
        {
          offerSubscription.IsUseable = false;
          offerSubscription.DisabledReason = ResourceStatusReason.NoAzureSubscription | ResourceStatusReason.NoIncludedQuantityLeft;
          offerSubscription.DisabledResourceActionLink = new Uri(HostingResources.AzureSubscriptionSignupLink((object) "0x409"));
        }
        else if (azureSubscription.AzureSubscriptionStatusId == SubscriptionStatus.Active && !meteredResource.IsPaidBillingEnabled && meteredResource.Meter.BillingMode == ResourceBillingMode.PayAsYouGo)
        {
          offerSubscription.IsUseable = false;
          offerSubscription.DisabledReason = ResourceStatusReason.NoIncludedQuantityLeft | ResourceStatusReason.PaidBillingDisabled;
          offerSubscription.DisabledResourceActionLink = new Uri(HostingResources.AzureManageDevServicesLink((object) "0x409"));
        }
      }
      else if (offerSubscription.IsUseable && meteredResource.Meter.BillingMode == ResourceBillingMode.PayAsYouGo && meteredResource.CurrentQuantity >= meteredResource.MaximumQuantity)
      {
        offerSubscription.IsUseable = false;
        offerSubscription.DisabledReason = ResourceStatusReason.MaximumQuantityReached;
        offerSubscription.DisabledResourceActionLink = new Uri(HostingResources.AzureManageDevServicesLink((object) "0x409"));
      }
      if (azureSubscription != null && azureSubscription.AzureSubscriptionStatusId != SubscriptionStatus.Active && (offerSubscription.OfferMeter.Category != MeterCategory.Bundle || offerSubscription.OfferMeter.RenewalFrequency != MeterRenewalFrequecy.Annually || offerSubscription.CommittedQuantity <= 0))
      {
        offerSubscription.IsUseable = false;
        offerSubscription.DisabledReason |= ResourceStatusReason.SubscriptionDisabled;
        offerSubscription.DisabledResourceActionLink = new Uri(HostingResources.AzureSubscriptionsPageLink((object) "0x409"));
        if (offerSubscription.OfferMeter.BillingMode == ResourceBillingMode.PayAsYouGo)
          offerSubscription.IsPaidBillingEnabled = false;
        if (meteredResource.Meter.BillingMode == ResourceBillingMode.Committment)
          offerSubscription.CommittedQuantity = meteredResource.IncludedQuantity;
        else if (meteredResource.Meter.BillingMode == ResourceBillingMode.PayAsYouGo && meteredResource.CurrentQuantity < meteredResource.IncludedQuantity)
        {
          offerSubscription.IsUseable = true;
          offerSubscription.DisabledReason = ResourceStatusReason.None;
          offerSubscription.DisabledResourceActionLink = (Uri) null;
        }
      }
      if (azureSubscription != null && azureResourceAccount != null && !meteredResource.IsTrialOrPreview && meteredResource.Meter.BillingMode == ResourceBillingMode.Committment && meteredResource.CommittedQuantity > meteredResource.IncludedQuantity)
        this.QueueResetJob(collectionContext, resetDate, meteredResource, "GetResourceStatus API");
      string linkUriFromRegistry = this.GetAzureDeepLinkUriFromRegistry(collectionContext, azureSubscription, azureResourceAccount);
      if (!string.IsNullOrWhiteSpace(linkUriFromRegistry) && meteredResource.Meter.BillingMode == ResourceBillingMode.Committment)
        offerSubscription.DisabledResourceActionLink = new Uri(linkUriFromRegistry);
      if (this.IsArtifactMeter(meteredResource.Meter.Name) && this.IsExistingArtifactsOrg(collectionContext) && !this.IsAccountInternal(collectionContext))
        offerSubscription.IsUseable = true;
      if (this.IsInternalBuildMeter(meteredResource.Meter.Name) && !collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.BlockInternalBuild"))
        offerSubscription.IsUseable = true;
      if (this.IsCLTMeter(meteredResource.Meter.Name) && collectionContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableMeterResetJobQueueAndCLTChanges") && this.IsPayAsYouGoMonthlyMeterResetRequired(meteredResource.LastResetDate) && !meteredResource.IsDefaultEmptyEntry)
      {
        collectionContext.TraceAlways(5109354, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), new
        {
          Msg = "Meter resource details before CLT reset",
          resource = meteredResource
        }.Serialize());
        this.ResetCloudLoadTest(collectionContext, meteredResource.RenewalGroup);
      }
      collectionContext.TraceProperties<OfferSubscription>(5108906, "Commerce", nameof (PlatformOfferSubscriptionService), offerSubscription, (string) null);
      return offerSubscription;
    }

    internal virtual void GetAzureResourceAccountAndSubscription(
      IVssRequestContext requestContext,
      out AzureResourceAccount azureResourceAccount,
      out AzureSubscriptionInternal azureSubscription)
    {
      requestContext.TraceEnter(5108695, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (GetAzureResourceAccountAndSubscription));
      try
      {
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        azureSubscription = (AzureSubscriptionInternal) null;
        IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
        azureResourceAccount = this.GetAzureResourceAccount(deploymentContext, instanceId);
        if (azureResourceAccount == null)
          return;
        azureSubscription = this.GetAzureSubscription(deploymentContext, azureResourceAccount.AzureSubscriptionId);
      }
      finally
      {
        requestContext.TraceLeave(5108907, "Commerce", nameof (PlatformOfferSubscriptionService), nameof (GetAzureResourceAccountAndSubscription));
      }
    }

    private string GetAzureDeepLinkUriFromRegistry(
      IVssRequestContext requestContext,
      AzureSubscriptionInternal azureSubscription,
      AzureResourceAccount azureResourceAccount)
    {
      return azureSubscription == null || azureResourceAccount == null ? string.Empty : HostingResources.AzureExtensionLink((object) "0x409");
    }

    internal virtual DateTime GetNextMonthQueueDateTime(
      DateTime utcNow,
      MeterRenewalFrequecy renewalFrequency)
    {
      if (renewalFrequency != MeterRenewalFrequecy.Annually)
        throw new InvalidOperationException("Cannot invoke this method if offer meter is not a bundle purchase");
      return utcNow.Month == 12 ? new DateTime(utcNow.AddYears(1).Year, 1, 15, utcNow.Hour, utcNow.Minute, utcNow.Second) : new DateTime(utcNow.Year, utcNow.AddMonths(1).Month, 15, utcNow.Hour, utcNow.Minute, utcNow.Second);
    }

    internal ResourceRenewalGroup GetDefaultRenewalGroup(DateTime currentDate)
    {
      DateTime date = currentDate.Date;
      if (date.Day == 1)
        return (ResourceRenewalGroup) date.Month;
      return date.Month != 12 ? (ResourceRenewalGroup) (date.Month + 1) : ResourceRenewalGroup.Jan;
    }

    internal virtual DateTime GetNextCalendarDayResetTime(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount,
      ResourceRenewalGroup renewalGroup,
      DateTime? lastResetDate)
    {
      DateTime calendarDayResetTime = renewalGroup != ResourceRenewalGroup.Monthly ? this.GetCurrentCalendarMonthMeterResetTime(renewalGroup, lastResetDate).AddYears(1) : this.GetCurrentCalendarMonthMeterResetTime(renewalGroup, lastResetDate).AddMonths(1);
      if (azureResourceAccount != null)
      {
        ref DateTime local = ref calendarDayResetTime;
        int year = calendarDayResetTime.Year;
        int month = calendarDayResetTime.Month;
        int day = calendarDayResetTime.Day;
        DateTime created = azureResourceAccount.Created;
        int hour = created.Hour;
        created = azureResourceAccount.Created;
        int minute = created.Minute;
        created = azureResourceAccount.Created;
        int second = created.Second;
        local = new DateTime(year, month, day, hour, minute, second, 0, DateTimeKind.Utc);
      }
      if (calendarDayResetTime < this.GetUtcNow())
      {
        requestContext.Trace(5104268, TraceLevel.Error, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Calculated invalid reset date {0} for subscription with renewal group {1} on account {2}.", (object) calendarDayResetTime, (object) renewalGroup, (object) azureResourceAccount?.AccountId) + "This may indicate that the previous reset job was skipped or did not run successfully. Setting reset data to next first of the month.");
        Func<DateTime, DateTime, int> func = (Func<DateTime, DateTime, int>) ((date1, date2) => 12 * (date1.Year - date2.Year) + (date1.Month - date2.Month));
        calendarDayResetTime = calendarDayResetTime.AddMonths(func(this.GetUtcNow(), calendarDayResetTime) + 1);
      }
      return calendarDayResetTime;
    }

    internal virtual bool IsPayAsYouGoMonthlyMeterResetRequired(DateTime lastResetDate) => lastResetDate < this.GetCurrentCalendarMonthMeterResetTime(ResourceRenewalGroup.Monthly, new DateTime?());

    internal virtual DateTime GetCurrentCalendarMonthMeterResetTime(
      ResourceRenewalGroup renewalGroup,
      DateTime? lastResetDate)
    {
      DateTime utcNow = this.GetUtcNow();
      DateTime monthMeterResetTime;
      if (renewalGroup != ResourceRenewalGroup.Monthly && lastResetDate.HasValue)
      {
        DateTime date1 = lastResetDate.Value.Date;
        DateTime minValue = DateTime.MinValue;
        DateTime date2 = minValue.Date;
        if (!(date1 == date2))
        {
          int num = (int) renewalGroup;
          minValue = lastResetDate.Value;
          int month1 = minValue.Month;
          if (num != month1)
          {
            ref DateTime? local = ref lastResetDate;
            minValue = lastResetDate.Value;
            DateTime dateTime = minValue.AddMonths(1);
            local = new DateTime?(dateTime);
          }
          ref DateTime local1 = ref monthMeterResetTime;
          minValue = lastResetDate.Value;
          int year = minValue.Year;
          int month2 = (int) renewalGroup;
          local1 = new DateTime(year, month2, 1, 0, 0, 0, DateTimeKind.Utc);
          goto label_6;
        }
      }
      int month = renewalGroup != ResourceRenewalGroup.Monthly ? (int) renewalGroup : utcNow.Month;
      monthMeterResetTime = new DateTime(utcNow.Year, month, 1, 0, 0, 0, DateTimeKind.Utc);
label_6:
      return monthMeterResetTime;
    }

    internal virtual AzureResourceAccount GetAzureResourceAccount(
      IVssRequestContext deploymentContext,
      Guid collectionId)
    {
      return deploymentContext.GetService<PlatformSubscriptionService>().GetAzureResourceAccountByCollectionId(deploymentContext.Elevate(), collectionId);
    }

    internal virtual AzureSubscriptionInternal GetAzureSubscription(
      IVssRequestContext deploymentContext,
      Guid azureSubscriptionId)
    {
      return deploymentContext.GetService<PlatformSubscriptionService>().GetAzureSubscription(deploymentContext.Elevate(), azureSubscriptionId);
    }

    internal virtual TeamFoundationJobQueueEntry QueueResetJob(
      IVssRequestContext collectionContext,
      DateTime resetDate,
      OfferSubscriptionInternal meterUsage,
      string source = "Job Extension")
    {
      this.ValidateRequestContext(collectionContext);
      try
      {
        if (collectionContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableMeterResetJobQueueAndCLTChanges") && !collectionContext.IsInfrastructureHost())
        {
          collectionContext.TraceAlways(5109344, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Reset job was not queued as this is not a infrastructure host.");
          return (TeamFoundationJobQueueEntry) null;
        }
        Guid jobGuid = new Guid("43EB5A2D-760C-4538-B8AF-26C05279E5C6");
        if (!JobHelper.DoesJobDefinitionExist(collectionContext, jobGuid))
          JobHelper.CreateJobDefinition(collectionContext, jobGuid, "MeterResetJobExtension", "Microsoft.VisualStudio.Services.Commerce.MeterResetJobExtension");
        return this.QueueResetJobInCollectionContext(collectionContext, resetDate, meterUsage, source);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5104251, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        return (TeamFoundationJobQueueEntry) null;
      }
    }

    private TeamFoundationJobQueueEntry QueueResetJobInCollectionContext(
      IVssRequestContext collectionContext,
      DateTime resetDate,
      OfferSubscriptionInternal meterUsage,
      string source)
    {
      Guid jobGuid = new Guid("43EB5A2D-760C-4538-B8AF-26C05279E5C6");
      List<TeamFoundationJobQueueEntry> source1 = collectionContext.GetService<ITeamFoundationJobService>().QueryJobQueue(collectionContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobGuid
      });
      TeamFoundationJobQueueEntry foundationJobQueueEntry = source1 != null ? source1.FirstOrDefault<TeamFoundationJobQueueEntry>() : (TeamFoundationJobQueueEntry) null;
      return foundationJobQueueEntry == null || foundationJobQueueEntry.State == TeamFoundationJobState.QueuedScheduled && foundationJobQueueEntry.QueueTime.Date > resetDate.Date ? this.QueueResetJobInContext(collectionContext, jobGuid, resetDate, meterUsage, source) : foundationJobQueueEntry;
    }

    private TeamFoundationJobQueueEntry QueueResetJobInContext(
      IVssRequestContext requestContext,
      Guid jobGuid,
      DateTime resetDate,
      OfferSubscriptionInternal meterUsage,
      string source)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      requestContext.Trace(5104250, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Reset Job is being queued by {0}. Current Time: {1} and Queue Time: {2}. Source offer meter: {3}", (object) source, (object) this.GetUtcNow(), (object) resetDate, (object) meterUsage.Meter?.GalleryId));
      service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobGuid
      }, Convert.ToInt32(resetDate.Subtract(DateTime.UtcNow).TotalSeconds));
      CommerceKpi.PlatformMeteringQueueResetJobHit.IncrementByOne(requestContext);
      return service.QueryJobQueue(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobGuid
      }).FirstOrDefault<TeamFoundationJobQueueEntry>();
    }

    internal virtual bool IsValidUsage(
      AzureResourceAccount azureAccount,
      OfferSubscriptionInternal meterUsage,
      IOfferSubscription resource,
      int reportedQuantity,
      DateTime billableDate)
    {
      ArgumentUtility.CheckForOutOfRange(reportedQuantity, nameof (reportedQuantity), 0);
      if (resource.IsTrialOrPreview && !resource.TrialExpiryDate.HasValue)
        throw new InvalidOperationException(string.Format("Can not purchase when a trial date is not set. Account {0}", (object) azureAccount.AccountId));
      if (reportedQuantity <= 0 && meterUsage.Meter.BillingMode == ResourceBillingMode.PayAsYouGo)
        throw new InvalidResourceException(HostingResources.InvalidCommitmentResourceQuantity((object) resource.OfferMeter.Name));
      if ((resource.IncludedQuantity != -1 && reportedQuantity < resource.IncludedQuantity || resource.MaximumQuantity != -1 && reportedQuantity > resource.MaximumQuantity) && meterUsage.Meter.BillingMode == ResourceBillingMode.Committment)
        throw new AccountQuantityException(HostingResources.InvalidCommitmentResourceQuantity((object) resource.OfferMeter.Name));
      if (!resource.IsUseable)
      {
        if (!meterUsage.IsPaidBillingEnabled && billableDate < meterUsage.PaidBillingUpdatedDate)
          return true;
        throw new InvalidResourceException(HostingResources.ResourceIsUnusable((object) resource.OfferMeter.Name, (object) resource.DisabledReason.ToString()));
      }
      return (azureAccount != null || meterUsage.Meter.BillingMode != ResourceBillingMode.Committment) && (meterUsage.Meter.BillingMode != ResourceBillingMode.Committment || reportedQuantity != meterUsage.CommittedQuantity || meterUsage.CurrentQuantity != meterUsage.CommittedQuantity || meterUsage.Meter.AssignmentModel == OfferMeterAssignmentModel.Implicit && reportedQuantity == 0);
    }

    internal virtual bool IsValidArtifactsUsage(
      AzureResourceAccount azureAccount,
      OfferSubscriptionInternal meterUsage,
      AzureSubscriptionInternal subscription,
      IOfferSubscription resource,
      int reportedQuantity,
      DateTime billableDate)
    {
      if (!string.Equals(meterUsage.Meter.Name, "Artifacts", StringComparison.OrdinalIgnoreCase))
        return false;
      if (reportedQuantity < 0)
        throw new InvalidResourceException(HostingResources.InvalidCommitmentResourceQuantity((object) resource.OfferMeter.Name));
      return true;
    }

    private string SaveUsageEventInEventStore(
      IVssRequestContext requestContext,
      UsageEvent usageEvent)
    {
      string str = this.GetUsageEventsStoreInstance(requestContext.To(TeamFoundationHostType.Deployment)).SaveUsageEvent(requestContext, usageEvent);
      CommerceKpi.ReportUsageEventStore.IncrementByOne(requestContext);
      return str;
    }

    [ExcludeFromCodeCoverage]
    internal virtual CommerceMeteringComponent GetSqlComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<CommerceMeteringComponent>();

    internal virtual void CheckPermissionSystemContext(
      IVssRequestContext requestContext,
      int permissions)
    {
      requestContext.GetService<IPermissionCheckerService>().CheckPermission(requestContext.To(TeamFoundationHostType.Deployment), permissions, CommerceSecurity.MeteringSecurityNamespaceId);
    }

    [ExcludeFromCodeCoverage]
    internal virtual void CheckPermissionCollectionContext(
      IVssRequestContext collectionContext,
      int permissions,
      string token)
    {
      collectionContext.GetService<IPermissionCheckerService>().CheckPermission(collectionContext, permissions, CollectionBasedPermission.NamespaceId, token);
    }

    internal virtual void CheckPermissionUserContext(
      IVssRequestContext requestContext,
      int deploymentPermission,
      int collectionPermission)
    {
      bool flag = false;
      IPermissionCheckerService service = requestContext.GetService<IPermissionCheckerService>();
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        flag = service.CheckPermission(requestContext.To(TeamFoundationHostType.Deployment), deploymentPermission, CommerceSecurity.MeteringSecurityNamespaceId, throwAccessDenied: false);
      if (!flag && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        flag = service.CheckPermission(requestContext, collectionPermission, CollectionBasedPermission.NamespaceId, "/Meter", false);
      if (flag)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity == null)
        throw new AccessCheckException(string.Format("{0} has no permissions to perform this operation", (object) requestContext.UserContext));
      throw new AccessCheckException(userIdentity.Descriptor, userIdentity.DisplayName, "/Meter", collectionPermission, CollectionBasedPermission.NamespaceId, "User Not Authorized MeteringService");
    }

    internal virtual void WriteTrialCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      string feature,
      Guid collectionId,
      string galleryId,
      DateTime? trialStartDate,
      DateTime? trialEndDate,
      string source,
      int? includedQuantity = null,
      int? committedQuantity = null,
      bool inTrial = false)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add("CollectionId", (object) collectionId);
      eventData.Add("GalleryId", galleryId);
      eventData.Add("InTrial", inTrial);
      eventData.Add("TrialStartDate", (object) trialStartDate);
      eventData.Add("TrialEndDate", (object) trialEndDate);
      eventData.Add("Source", source);
      eventData.Add("IncludedQuantity", (object) includedQuantity);
      eventData.Add("CommittedQuantity", (object) committedQuantity);
      CustomerIntelligence.PublishEvent(requestContext, feature, eventData);
    }

    internal virtual void PublishMeterResetJobCompletedEventNotification(
      IVssRequestContext collectionContext)
    {
      try
      {
        this.ValidateRequestContext(collectionContext);
        Guid instanceId = collectionContext.ServiceHost.InstanceId;
        MeterResetJobCompletedEvent jobCompletedEvent = new MeterResetJobCompletedEvent(this.GetUtcNow());
        EventServiceNotificationPublisher notificationPublisher = new EventServiceNotificationPublisher();
        collectionContext.Trace(5105781, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("Publishing notification MeterResetJobCompletedEvent for Collection {0}", (object) instanceId));
        IVssRequestContext requestContext = collectionContext;
        MeterResetJobCompletedEvent notification = jobCompletedEvent;
        notificationPublisher.Publish(requestContext, (object) notification);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5105782, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
      }
    }

    private void WriteOfferSubscriptionCustomerEvent(
      IVssRequestContext requestContext,
      string galleryId,
      Guid hostId,
      Guid subscriptionId,
      int committedQuantity,
      string purchaseType)
    {
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        string property = userIdentity.GetProperty<string>("Domain", (string) null);
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        eventData.Add("Identity", (object) userIdentity.Id);
        eventData.Add("IdentityTenant", property);
        eventData.Add("GalleryId", galleryId);
        eventData.Add("AccountId", (object) hostId);
        eventData.Add("SubscriptionId", (object) subscriptionId);
        eventData.Add("PurchaseType", purchaseType);
        eventData.Add("CommittedQuantity", (double) committedQuantity);
        CustomerIntelligence.PublishEvent(requestContext, "OfferSubscription", eventData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106969, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
      }
    }

    internal virtual Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext)
    {
      return CommerceIdentityHelper.GetIdentity(requestContext);
    }

    internal virtual void AddCIEventForCommitmentLicenses(
      IVssRequestContext requestContext,
      OfferMeter meterConfig,
      OfferSubscriptionInternal meterUsage,
      int quantity,
      AzureSubscriptionInternal subscription,
      Guid eventUserId)
    {
      if (meterConfig.BillingMode != ResourceBillingMode.Committment)
        return;
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add(CustomerIntelligenceProperty.ActivityId, (object) requestContext.ActivityId);
      eventData.Add(CustomerIntelligenceProperty.AccountId, (object) requestContext.ServiceHost.InstanceId);
      this.CheckForRequestSource(requestContext, eventData);
      eventData.Add(CustomerIntelligenceProperty.LicenseCommittedQuantity, (double) quantity);
      if (meterUsage != null)
      {
        eventData.Add(CustomerIntelligenceProperty.LicenseType, meterUsage.Meter.Name);
        eventData.Add(CustomerIntelligenceProperty.LicenseCurrentQuantity, (double) meterUsage.CurrentQuantity);
      }
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
        {
          eventUserId
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        eventData.Add("UserType", identity == null || !identity.IsCspPartnerUser ? string.Empty : "CSP");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109034, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
      }
      if (subscription != null)
        eventData.Add(CustomerIntelligenceProperty.AzureSubscriptionId, (object) subscription.AzureSubscriptionId);
      CustomerIntelligence.PublishEvent(requestContext, "PurchaseLicenses", eventData);
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

    internal virtual void TraceBIEventsForMeteringService(
      IVssRequestContext requestContext,
      Guid eventUserId,
      Guid eventUserCUID,
      OfferSubscriptionInternal meterUsage,
      int usedQuantity,
      double billedQuantity,
      int committedQuantity,
      int priorCommittedQuantity,
      int changedPurchaseQuantity,
      Guid subscriptionId,
      string eventType)
    {
      if (meterUsage == null)
        return;
      DateTime utcNow = this.GetUtcNow();
      TeamFoundationTracingService.TraceCommerceMeteredResource(requestContext.ServiceHost.InstanceId, requestContext.ServiceHost.ParentServiceHost.InstanceId, requestContext.ServiceHost.HostType, eventUserId, eventUserCUID, meterUsage.Meter.Name, meterUsage.Meter.PlatformMeterId, meterUsage.RenewalGroup.ToString(), meterUsage.IncludedQuantity, committedQuantity, new int?(priorCommittedQuantity), meterUsage.CurrentQuantity, meterUsage.MaximumQuantity, changedPurchaseQuantity, meterUsage.IsPaidBillingEnabled, string.Format("{0}{1}", (object) utcNow.Year, (object) utcNow.Month.ToString("00")), billedQuantity, usedQuantity, CommerceUtil.CheckForRequestSource(requestContext), subscriptionId, meterUsage.Meter.Category.ToString(), utcNow, eventType);
    }

    internal virtual void SendSetAccountQuantityBIEvent(
      IVssRequestContext requestContext,
      Guid eventUserId,
      Guid eventUserCUID,
      OfferSubscriptionInternal meterUsage,
      OfferSubscriptionInternal updatedMeterUsage,
      int committedQuantity,
      int priorCommittedQuantity,
      Guid subscriptionId)
    {
      if (meterUsage.IncludedQuantity != updatedMeterUsage.IncludedQuantity)
        this.TraceBIEventsForMeteringService(requestContext, eventUserId, eventUserCUID, updatedMeterUsage, 0, 0.0, committedQuantity, priorCommittedQuantity, 0, Guid.Empty, updatedMeterUsage.IncludedQuantity < meterUsage.IncludedQuantity ? "ReduceIncludedQuantity" : "IncreaseIncludedQuantity");
      if (meterUsage.MaximumQuantity == updatedMeterUsage.MaximumQuantity)
        return;
      this.TraceBIEventsForMeteringService(requestContext, eventUserId, eventUserCUID, updatedMeterUsage, 0, 0.0, committedQuantity, priorCommittedQuantity, 0, Guid.Empty, updatedMeterUsage.MaximumQuantity < meterUsage.MaximumQuantity ? "ReduceMaxQuantity" : "IncreaseMaxQuantity");
    }

    internal void GetResourceGroupAndName(
      IVssRequestContext requestContext,
      out string resourceGroupName,
      out string resourceName,
      Guid collectionId,
      string defaultResourceGroupName,
      string defaultResourceName)
    {
      resourceGroupName = defaultResourceGroupName;
      resourceName = defaultResourceName;
      AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(requestContext.To(TeamFoundationHostType.Deployment), collectionId);
      if (azureResourceAccount == null)
        return;
      resourceGroupName = azureResourceAccount.AzureCloudServiceName;
      resourceName = azureResourceAccount.AzureResourceName;
    }

    internal virtual OfferMeter GetValidMeter(
      IVssRequestContext requestContext,
      string resourceName)
    {
      OfferMeter offerMeter = (OfferMeter) requestContext.GetService<IOfferMeterService>().GetOfferMeter(requestContext, resourceName);
      return !(offerMeter == (OfferMeter) null) ? offerMeter : throw new ArgumentException("Invalid resource name: " + resourceName + ".", nameof (resourceName));
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.dateTimeProvider.UtcNow;

    internal virtual void CheckPermission(IVssRequestContext requestContext, int permissions)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.IsSystemContext)
        return;
      requestContext.GetService<IPermissionCheckerService>().CheckPermission(requestContext, permissions, CommerceSecurity.CommerceSecurityNamespaceId);
    }

    internal virtual bool VerifyCurrentUserIsAzureAdminOrCoAdmin(
      IVssRequestContext requestContext,
      Guid azureSubscriptionId,
      bool isBundle,
      bool shouldThrowOnFail = true)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IAzureResourceHelper service = vssRequestContext.GetService<IAzureResourceHelper>();
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.BypassConnectedServerSubscriptionCheck") || ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) || service.GetSubscriptionAuthorization(vssRequestContext, azureSubscriptionId, isBundle) != SubscriptionAuthorizationSource.Unauthorized)
        return true;
      if (shouldThrowOnFail)
        throw new UserIsNotSubscriptionAdminException();
      return false;
    }

    internal virtual void SendPurchaseConfirmationEmail(
      IVssRequestContext collectionContext,
      ResourceRenewalGroup renewalGroup,
      int meterId,
      double billedQuantity,
      Guid azureSubscriptionId,
      string offerCode,
      DateTime renewalDate,
      Guid? tenantId,
      Guid? objectId,
      PurchaseNotificationCommunicator emailCommunicator,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        OfferSubscriptionInternal subscriptionInternal = this.GetMeterUsages(collectionContext, new int?(meterId), new ResourceRenewalGroup?(renewalGroup), false).SingleOrDefault<OfferSubscriptionInternal>();
        string name = subscriptionInternal.Meter.Name;
        string galleryId = subscriptionInternal.Meter.GalleryId;
        if (emailCommunicator == null)
          emailCommunicator = (PurchaseNotificationCommunicator) new PurchaseNotificationExtensionCommunicator();
        if (renewalGroup == ResourceRenewalGroup.Monthly)
        {
          int quantity = subscriptionInternal.CurrentQuantity - subscriptionInternal.IncludedQuantity;
          if (quantity <= 0)
            return;
          if (!(emailCommunicator is PurchaseNotificationBundleCommunicator) && collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableCommerceNotificationEmail"))
          {
            Microsoft.VisualStudio.Services.Identity.Identity msaIdentityHelper = CommerceCommons.GetPrimaryMsaIdentityHelper(collectionContext, identity);
            new PurchaseExtensionNotification().SendPurchaseNotificationEmail(collectionContext, name, azureSubscriptionId, quantity, offerCode, tenantId, objectId, msaIdentityHelper);
          }
          else
            emailCommunicator.SendPurchaseNotificationEmail(collectionContext, name, galleryId, azureSubscriptionId, renewalGroup, quantity, offerCode, renewalDate, tenantId, objectId, identity);
        }
        else
        {
          if (billedQuantity <= 0.0)
            return;
          emailCommunicator.SendPurchaseNotificationEmail(collectionContext, name, galleryId, azureSubscriptionId, renewalGroup, Convert.ToInt32(billedQuantity), offerCode, renewalDate, tenantId, objectId, identity);
        }
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5109212, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
      }
    }

    internal virtual void TrySetOnPremisesPurchaseSource(
      IVssRequestContext collectionContext,
      AzureResourceAccount azureAccount)
    {
      this.ValidateRequestContext(collectionContext);
      if (azureAccount == null || azureAccount.ProviderNamespaceId != AccountProviderNamespace.OnPremise)
        return;
      if (!collectionContext.Items.ContainsKey("Commerce.RequestSource"))
        collectionContext.Items.Add("Commerce.RequestSource", (object) "TeamFoundationServer");
      else
        collectionContext.Items["Commerce.RequestSource"] = (object) "TeamFoundationServer";
    }

    internal virtual void TrySetSourceForPayAsYouGoMeters(
      IVssRequestContext collectionContext,
      OfferMeter offerMeter)
    {
      this.ValidateRequestContext(collectionContext);
      if (offerMeter.BillingMode != ResourceBillingMode.PayAsYouGo)
        return;
      if (!collectionContext.Items.ContainsKey("Commerce.RequestSource"))
        collectionContext.Items.Add("Commerce.RequestSource", (object) "Metering");
      else
        collectionContext.Items["Commerce.RequestSource"] = (object) "Metering";
    }

    internal virtual void WriteThirdPartyOfferSubscriptionCancellationCustomerIntelligenceEvent(
      IVssRequestContext deploymentContext,
      Guid collectionId,
      OfferMeter offerMeter)
    {
      AzureResourceAccount azureResourceAccount = this.GetAzureResourceAccount(deploymentContext, collectionId);
      if (azureResourceAccount == null)
        return;
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add(CustomerIntelligenceProperty.PartNumber, "Not present");
      eventData.Add(CustomerIntelligenceProperty.OrderNumber, "Not present");
      eventData.Add(CustomerIntelligenceProperty.SubscriptionId, (object) azureResourceAccount.AzureSubscriptionId);
      eventData.Add(CustomerIntelligenceProperty.ResourceGroup, azureResourceAccount.AzureCloudServiceName);
      eventData.Add(CustomerIntelligenceProperty.AccountResourceName, azureResourceAccount.AzureResourceName);
      eventData.Add(CustomerIntelligenceProperty.ExtensionResourceName, offerMeter.GalleryId);
      eventData.Add(CustomerIntelligenceProperty.ExtensionResourceHttpMethod, "DELETE");
      if (!offerMeter.FixedQuantityPlans.IsNullOrEmpty<AzureOfferPlanDefinition>())
      {
        AzureOfferPlanDefinition offerPlanDefinition = offerMeter.FixedQuantityPlans.FirstOrDefault<AzureOfferPlanDefinition>();
        eventData.Add("Plan", offerPlanDefinition?.PlanName);
        eventData.Add("Product", offerPlanDefinition?.OfferId);
        eventData.Add("Publisher", offerPlanDefinition?.Publisher);
      }
      CustomerIntelligence.PublishEvent(deploymentContext, "ExtensionResourcePurchase", eventData);
    }

    public void SetDateTimeProvider(IVssDateTimeProvider dateTimeProvider) => this.dateTimeProvider = dateTimeProvider;

    internal static double GetProratedQuantity(Decimal quantity, DateTime billDate)
    {
      Decimal num1 = (Decimal) DateTime.DaysInMonth(billDate.Year, billDate.Month);
      Decimal num2 = num1 - (Decimal) billDate.Day;
      return SqlDecimal.ConvertToPrecScale((SqlDecimal) (quantity * (num2 / num1)), 20, 8).ToDouble();
    }

    private void CheckMigrationStatus(IVssRequestContext collectionContext)
    {
      if (collectionContext.IsSpsService() && collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress") || collectionContext.IsCommerceService() && collectionContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.ResourceMigrationInProgress"))
        throw new AccountMigrationInProgressException("Account migration is in progress. Please try again after sometime.");
    }

    private void ValidateBlobStoreReportingOperation(
      IVssRequestContext collectionContext,
      string resourceName)
    {
      if (CommerceDeploymentHelper.IsCallerBlobstoreServicePrincipal(collectionContext) && !string.Equals(resourceName, "Artifacts", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Blobstore Service Principle can't be used to report usage for meter " + resourceName);
    }

    private bool IsExistingArtifactsOrg(IVssRequestContext collectionContext)
    {
      if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.IgnoreBillingExistingArtifactsOrgs"))
      {
        try
        {
          IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
          DateTime dateTime = vssRequestContext.GetService<IVssRegistryService>().GetValue<DateTime>(vssRequestContext, new RegistryQuery("/Service/Commerce/Artifacts/ArtifactsExistingCustomersCutoffDate"), new DateTime(2019, 5, 6, 0, 0, 0, DateTimeKind.Utc));
          DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
          Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
          if (collection != null)
            return collection.DateCreated < dateTime;
          collectionContext.TraceAlways(5109291, TraceLevel.Warning, "Commerce", nameof (PlatformOfferSubscriptionService), string.Format("ICollectionService.GetCollection returns null for HostId {0}", (object) collectionContext.ServiceHost.InstanceId));
        }
        catch (Exception ex)
        {
          collectionContext.TraceException(5109278, "Commerce", nameof (PlatformOfferSubscriptionService), ex);
        }
      }
      return true;
    }

    private bool IsArtifactMeter(string meterName) => string.Equals(meterName, "Artifacts", StringComparison.OrdinalIgnoreCase);

    private bool IsInternalBuildMeter(string meterName) => string.Equals(meterName, "MSHostedCICDforMacOS", StringComparison.OrdinalIgnoreCase) || string.Equals(meterName, "MsHostedCICDforWindowsLinux", StringComparison.OrdinalIgnoreCase);

    private bool IsStandardLicense(string meterName) => string.Equals(meterName, "StandardLicense", StringComparison.OrdinalIgnoreCase);

    private bool IsCLTMeter(string meterName) => string.Equals(meterName, "LoadTest", StringComparison.OrdinalIgnoreCase);

    private bool ShouldBillUsage(
      IVssRequestContext collectionContext,
      OfferSubscriptionInternal meterUsage,
      DateTime billableDate)
    {
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableArtifactsReporting") || !collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableArtifactsBilling"))
      {
        collectionContext.TraceAlways(5109271, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Skipping billing for Artifacts. ReportingFF: " + string.Format("{0} BillingFF: {1}", (object) collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableArtifactsReporting"), (object) collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableArtifactsBilling")));
        return false;
      }
      if (meterUsage.LastUpdated.Day == billableDate.Day)
      {
        collectionContext.TraceAlways(5109272, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Skipping billing for Artifacts. There has already been usage reported today." + string.Format(" Last updated: {0}", (object) meterUsage.LastUpdated));
        return false;
      }
      if (!this.IsExistingArtifactsOrg(collectionContext) || this.IsAccountInternal(collectionContext))
        return true;
      collectionContext.TraceAlways(5109277, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Skipping billing for Artifacts. The external org was created before release date.");
      return false;
    }

    internal virtual bool IsAccountInternal(IVssRequestContext collectionContext)
    {
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.IsInternalOrganizationCheck"))
        return false;
      int num = collectionContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(collectionContext, "Policy.IsInternal", false).EffectiveValue ? 1 : 0;
      if (num == 0)
        return num != 0;
      collectionContext.TraceAlways(5109279, TraceLevel.Info, "Commerce", nameof (PlatformOfferSubscriptionService), "Internal Organization");
      return num != 0;
    }
  }
}
