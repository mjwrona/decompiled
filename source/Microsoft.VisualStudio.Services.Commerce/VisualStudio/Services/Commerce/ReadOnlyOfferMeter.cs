// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ReadOnlyOfferMeter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ReadOnlyOfferMeter
  {
    public ReadOnlyOfferMeter()
    {
    }

    internal ReadOnlyOfferMeter(IOfferMeter meter)
    {
      this.MeterId = meter.MeterId;
      this.PlatformMeterId = meter.PlatformMeterId;
      this.GalleryId = meter.GalleryId;
      this.Name = meter.Name;
      this.RenewalFrequency = meter.RenewalFrequency;
      this.Category = meter.Category;
      this.BillingMode = meter.BillingMode;
      this.OfferScope = meter.OfferScope;
      this.BillingState = meter.BillingState;
      this.Status = meter.Status;
      this.Unit = meter.Unit;
      this.AssignmentModel = meter.AssignmentModel;
      this.BillingStartDate = meter.BillingStartDate;
      this.TrialDays = meter.TrialDays;
      this.PreviewGraceDays = meter.PreviewGraceDays;
      this.IncludedQuantity = meter.IncludedQuantity;
      this.CurrentQuantity = meter.CurrentQuantity;
      this.CommittedQuantity = meter.CommittedQuantity;
      this.MaximumQuantity = meter.MaximumQuantity;
      this.AbsoluteMaximumQuantity = meter.AbsoluteMaximumQuantity;
      this.TrialCycles = meter.TrialCycles;
      this.BillingEntity = meter.BillingEntity;
      this.IncludedInLicenseLevel = meter.IncludedInLicenseLevel;
      this.MinimumRequiredAccessLevel = meter.MinimumRequiredAccessLevel;
      this.AutoAssignOnAccess = meter.AutoAssignOnAccess;
      IEnumerable<AzureOfferPlanDefinition> fixedQuantityPlans = meter.FixedQuantityPlans;
      List<AzureOfferPlanDefinition> offerPlanDefinitionList;
      if (fixedQuantityPlans == null)
      {
        offerPlanDefinitionList = (List<AzureOfferPlanDefinition>) null;
      }
      else
      {
        IEnumerable<AzureOfferPlanDefinition> source = fixedQuantityPlans.Select<AzureOfferPlanDefinition, AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, AzureOfferPlanDefinition>) (p => p.Clone()));
        offerPlanDefinitionList = source != null ? source.ToList<AzureOfferPlanDefinition>() : (List<AzureOfferPlanDefinition>) null;
      }
      this.FixedQuantityPlans = (IEnumerable<AzureOfferPlanDefinition>) offerPlanDefinitionList;
    }

    internal OfferMeter ToOfferMeter()
    {
      OfferMeter offerMeter = new OfferMeter();
      offerMeter.MeterId = this.MeterId;
      offerMeter.PlatformMeterId = this.PlatformMeterId;
      offerMeter.GalleryId = this.GalleryId;
      offerMeter.Name = this.Name;
      offerMeter.RenewalFrequency = this.RenewalFrequency;
      offerMeter.Category = this.Category;
      offerMeter.BillingMode = this.BillingMode;
      offerMeter.OfferScope = this.OfferScope;
      offerMeter.BillingState = this.BillingState;
      offerMeter.Status = this.Status;
      offerMeter.Unit = this.Unit;
      offerMeter.AssignmentModel = this.AssignmentModel;
      offerMeter.BillingStartDate = this.BillingStartDate;
      offerMeter.TrialDays = this.TrialDays;
      offerMeter.PreviewGraceDays = this.PreviewGraceDays;
      offerMeter.IncludedQuantity = this.IncludedQuantity;
      offerMeter.CurrentQuantity = this.CurrentQuantity;
      offerMeter.CommittedQuantity = this.CommittedQuantity;
      offerMeter.MaximumQuantity = this.MaximumQuantity;
      offerMeter.AbsoluteMaximumQuantity = this.AbsoluteMaximumQuantity;
      offerMeter.TrialCycles = this.TrialCycles;
      offerMeter.BillingEntity = this.BillingEntity;
      IEnumerable<AzureOfferPlanDefinition> fixedQuantityPlans = this.FixedQuantityPlans;
      List<AzureOfferPlanDefinition> offerPlanDefinitionList;
      if (fixedQuantityPlans == null)
      {
        offerPlanDefinitionList = (List<AzureOfferPlanDefinition>) null;
      }
      else
      {
        IEnumerable<AzureOfferPlanDefinition> source = fixedQuantityPlans.Select<AzureOfferPlanDefinition, AzureOfferPlanDefinition>((Func<AzureOfferPlanDefinition, AzureOfferPlanDefinition>) (p => p.Clone()));
        offerPlanDefinitionList = source != null ? source.ToList<AzureOfferPlanDefinition>() : (List<AzureOfferPlanDefinition>) null;
      }
      offerMeter.FixedQuantityPlans = (IEnumerable<AzureOfferPlanDefinition>) offerPlanDefinitionList;
      offerMeter.IncludedInLicenseLevel = this.IncludedInLicenseLevel;
      offerMeter.MinimumRequiredAccessLevel = this.MinimumRequiredAccessLevel;
      offerMeter.AutoAssignOnAccess = this.AutoAssignOnAccess;
      return offerMeter;
    }

    public int MeterId { get; private set; }

    public Guid PlatformMeterId { get; private set; }

    public string GalleryId { get; private set; }

    public string Name { get; private set; }

    public MeterRenewalFrequecy RenewalFrequency { get; private set; }

    public ResourceBillingMode BillingMode { get; private set; }

    public MeterCategory Category { get; private set; }

    public OfferScope OfferScope { get; private set; }

    public MeterBillingState BillingState { get; private set; }

    public MeterState Status { get; private set; }

    public string Unit { get; private set; }

    public OfferMeterAssignmentModel AssignmentModel { get; private set; }

    public DateTime? BillingStartDate { get; private set; }

    public byte TrialDays { get; private set; }

    public byte PreviewGraceDays { get; private set; }

    public int IncludedQuantity { get; private set; }

    public int CurrentQuantity { get; private set; }

    public int CommittedQuantity { get; private set; }

    public int MaximumQuantity { get; private set; }

    public int AbsoluteMaximumQuantity { get; private set; }

    public int TrialCycles { get; private set; }

    public BillingProvider BillingEntity { get; private set; }

    public IEnumerable<AzureOfferPlanDefinition> FixedQuantityPlans { get; private set; }

    public MinimumRequiredServiceLevel MinimumRequiredAccessLevel { get; private set; }

    public MinimumRequiredServiceLevel IncludedInLicenseLevel { get; private set; }

    public bool AutoAssignOnAccess { get; set; }

    public override string ToString() => this.Name ?? string.Empty + "|" + this.GalleryId;
  }
}
