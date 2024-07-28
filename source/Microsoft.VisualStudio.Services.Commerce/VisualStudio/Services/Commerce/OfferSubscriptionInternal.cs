// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionInternal
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DebuggerDisplay("{Meter?.Name} | {IncludedQuantity} | {CommittedQuantity} | {CurrentQuantity}")]
  internal class OfferSubscriptionInternal
  {
    private bool? autoAssignOnAccess;
    private ReadOnlyOfferMeter configuration;

    private DateTime EpochDateTime { get; } = new DateTime(1900, 1, 1, 0, 0, 0);

    public OfferSubscriptionInternal()
    {
    }

    public OfferSubscriptionInternal(OfferSubscription offerSubscription)
    {
      this.MeterId = offerSubscription.OfferMeter.MeterId;
      this.RenewalGroup = offerSubscription.RenewalGroup;
      this.CommittedQuantity = offerSubscription.CommittedQuantity;
      this.CurrentQuantity = offerSubscription.CommittedQuantity;
      this.IncludedQuantity = offerSubscription.IncludedQuantity;
      this.MaximumQuantity = offerSubscription.MaximumQuantity;
      this.IsPaidBillingEnabled = offerSubscription.IsPaidBillingEnabled;
      DateTime? nullable = offerSubscription.StartDate;
      this.PaidBillingUpdatedDate = nullable ?? DateTime.MinValue;
      this.LastResetDate = offerSubscription.ResetDate.AddMonths(-1);
      this.IsTrialOrPreview = offerSubscription.IsTrialOrPreview;
      this.StartDate = offerSubscription.StartDate;
      this.IsDefaultEmptyEntry = false;
      this.BillingEntity = offerSubscription.OfferMeter.BillingEntity;
      this.Meter = new ReadOnlyOfferMeter((IOfferMeter) offerSubscription.OfferMeter);
      this.SetAutoAssignOnAccess(new bool?(offerSubscription.AutoAssignOnAccess));
      nullable = this.StartDate;
      if (nullable.HasValue)
      {
        nullable = offerSubscription.TrialExpiryDate;
        this.TrialDays = this.CalculateTrialDays(nullable.Value);
      }
      else
        this.TrialDays = -1;
    }

    public int MeterId { get; set; }

    public ResourceRenewalGroup RenewalGroup { get; set; }

    public int CommittedQuantity { get; set; }

    public int CurrentQuantity { get; set; }

    public int IncludedQuantity { get; set; }

    public int MaximumQuantity { get; set; }

    public bool IsPaidBillingEnabled { get; set; }

    public DateTime PaidBillingUpdatedDate { get; set; }

    public DateTime LastResetDate { get; set; }

    public bool IsTrialOrPreview { get; set; }

    public DateTime? StartDate { get; set; }

    public bool IsDefaultEmptyEntry { get; set; }

    public BillingProvider BillingEntity { get; set; }

    public DateTime LastUpdated { get; set; }

    public int TrialDays { get; set; }

    public bool AutoAssignOnAccess => this.autoAssignOnAccess.HasValue ? this.autoAssignOnAccess.Value : throw new InvalidOperationException("AutoAssignOnAccess was accessed without correctly initializing it.");

    public DateTime? TrialExpiryDate
    {
      get
      {
        if (!this.StartDate.HasValue || !this.Meter.BillingStartDate.HasValue)
          return new DateTime?();
        int num = 0;
        DateTime? nullable = this.StartDate;
        DateTime? billingStartDate = this.Meter.BillingStartDate;
        DateTime? startDate = this.StartDate;
        if ((billingStartDate.HasValue & startDate.HasValue ? (billingStartDate.GetValueOrDefault() > startDate.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          nullable = this.Meter.BillingStartDate;
          num = (int) this.Meter.PreviewGraceDays;
        }
        return new DateTime?(nullable.Value.AddDays((double) (this.TrialDays + num)));
      }
    }

    public bool IsPreview
    {
      get
      {
        if (!this.Meter.BillingStartDate.HasValue)
          return true;
        DateTime utcNow = DateTime.UtcNow;
        DateTime? billingStartDate = this.Meter.BillingStartDate;
        return billingStartDate.HasValue && utcNow < billingStartDate.GetValueOrDefault();
      }
    }

    public bool IsPurchaseCanceled => !this.IsPaidBillingEnabled && this.CommittedQuantity >= this.IncludedQuantity && this.CurrentQuantity == 0 && this.PaidBillingUpdatedDate > this.EpochDateTime;

    public bool IsPurchasedDuringTrial => this.IsTrialOrPreview && !this.IsPreview && this.CommittedQuantity > this.IncludedQuantity;

    public ReadOnlyOfferMeter Meter
    {
      get => this.configuration;
      set
      {
        this.configuration = value;
        if (this.IncludedQuantity < 0)
          this.IncludedQuantity = this.configuration.IncludedQuantity;
        if (this.MaximumQuantity < 0)
          this.MaximumQuantity = this.configuration.MaximumQuantity;
        if (this.TrialDays < 0)
          this.TrialDays = (int) this.configuration.TrialDays;
        if (this.autoAssignOnAccess.HasValue)
          return;
        this.SetAutoAssignOnAccess(new bool?(this.configuration.AutoAssignOnAccess));
      }
    }

    public OfferSubscriptionInternal Clone() => new OfferSubscriptionInternal()
    {
      MeterId = this.MeterId,
      CommittedQuantity = this.CommittedQuantity,
      CurrentQuantity = this.CurrentQuantity,
      IncludedQuantity = this.IncludedQuantity,
      MaximumQuantity = this.MaximumQuantity,
      IsPaidBillingEnabled = this.IsPaidBillingEnabled,
      LastResetDate = this.LastResetDate,
      RenewalGroup = this.RenewalGroup,
      Meter = this.Meter,
      BillingEntity = this.BillingEntity,
      IsTrialOrPreview = this.IsTrialOrPreview,
      LastUpdated = this.LastUpdated,
      TrialDays = this.TrialDays,
      autoAssignOnAccess = new bool?(this.AutoAssignOnAccess)
    };

    public OfferSubscription ToOfferSubscription(bool nextBillingPeriod)
    {
      OfferSubscription offerSubscription = new OfferSubscription()
      {
        CommittedQuantity = this.CommittedQuantity,
        IncludedQuantity = this.IncludedQuantity,
        MaximumQuantity = this.MaximumQuantity,
        IsPaidBillingEnabled = this.IsPaidBillingEnabled,
        OfferMeter = this.Meter.ToOfferMeter(),
        IsTrialOrPreview = this.IsTrialOrPreview,
        IsPurchaseCanceled = this.IsPurchaseCanceled,
        IsPurchasedDuringTrial = this.IsPurchasedDuringTrial,
        TrialExpiryDate = this.TrialExpiryDate,
        IsPreview = this.IsPreview,
        StartDate = this.StartDate,
        RenewalGroup = this.RenewalGroup,
        AutoAssignOnAccess = this.AutoAssignOnAccess
      };
      if (nextBillingPeriod)
        offerSubscription.CommittedQuantity = this.Meter.BillingMode != ResourceBillingMode.Committment ? 0 : this.CurrentQuantity;
      return offerSubscription;
    }

    public BillableEvent ToBillableEvent(IVssRequestContext collectionContext, Guid subscriptionId) => new BillableEvent()
    {
      SubscriptionId = subscriptionId,
      Quantity = (double) (this.CommittedQuantity - this.IncludedQuantity),
      MeterPlatformGuid = this.Meter.PlatformMeterId,
      AccountId = collectionContext.ServiceHost.InstanceId,
      AccountName = CollectionHelper.GetCollectionName(collectionContext),
      EventUtcTime = DateTime.UtcNow,
      EventUniqueId = Guid.NewGuid()
    };

    public void SetAutoAssignOnAccess(bool? state)
    {
      if (!state.HasValue)
        return;
      this.autoAssignOnAccess = state;
    }

    public int CalculateTrialDays(DateTime endDate)
    {
      DateTime? billingStartDate = this.Meter.BillingStartDate;
      DateTime? startDate = this.StartDate;
      return (billingStartDate.HasValue & startDate.HasValue ? (billingStartDate.GetValueOrDefault() > startDate.GetValueOrDefault() ? 1 : 0) : 0) == 0 ? Convert.ToInt32((endDate - this.StartDate.Value).TotalDays) : Convert.ToInt32((endDate - this.Meter.BillingStartDate.Value.AddDays((double) this.Meter.PreviewGraceDays)).TotalDays);
    }
  }
}
