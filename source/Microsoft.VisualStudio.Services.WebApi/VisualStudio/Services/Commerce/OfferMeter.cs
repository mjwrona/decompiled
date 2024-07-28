// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DebuggerDisplay("{MeterId} | {Name} | {GalleryId}")]
  public class OfferMeter : IOfferMeter, IEquatable<OfferMeter>
  {
    public int MeterId { get; set; }

    public Guid PlatformMeterId { get; set; }

    public string GalleryId { get; set; }

    public string Name { get; set; }

    public MeterRenewalFrequecy RenewalFrequency { get; set; }

    public ResourceBillingMode BillingMode { get; set; }

    public MeterCategory Category { get; set; }

    public OfferScope OfferScope { get; set; }

    public MeterBillingState BillingState { get; set; }

    public MeterState Status { get; set; }

    public string Unit { get; set; }

    public OfferMeterAssignmentModel AssignmentModel { get; set; }

    public DateTime? BillingStartDate { get; set; }

    public byte TrialDays { get; set; }

    public byte PreviewGraceDays { get; set; }

    public int IncludedQuantity { get; set; }

    public int CurrentQuantity { get; set; }

    public int CommittedQuantity { get; set; }

    public int MaximumQuantity { get; set; }

    public int AbsoluteMaximumQuantity { get; set; }

    public int TrialCycles { get; set; }

    public bool AutoAssignOnAccess { get; set; }

    public BillingProvider BillingEntity { get; set; }

    public MinimumRequiredServiceLevel MinimumRequiredAccessLevel { get; set; }

    public MinimumRequiredServiceLevel IncludedInLicenseLevel { get; set; }

    public IEnumerable<AzureOfferPlanDefinition> FixedQuantityPlans { get; set; }

    public bool IsFirstParty => this.BillingEntity == BillingProvider.SelfManaged || this.GalleryId.ToLower().Contains("ms.");

    public bool Equals(OfferMeter meter)
    {
      if (!(meter != (OfferMeter) null) || this.AbsoluteMaximumQuantity != meter.AbsoluteMaximumQuantity || this.AssignmentModel != meter.AssignmentModel || this.BillingEntity != meter.BillingEntity || this.BillingMode != meter.BillingMode)
        return false;
      DateTime? billingStartDate1 = this.BillingStartDate;
      DateTime? billingStartDate2 = meter.BillingStartDate;
      return (billingStartDate1.HasValue == billingStartDate2.HasValue ? (billingStartDate1.HasValue ? (billingStartDate1.GetValueOrDefault() == billingStartDate2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.BillingState == meter.BillingState && this.Category == meter.Category && this.CommittedQuantity == meter.CommittedQuantity && this.CurrentQuantity == meter.CurrentQuantity && (this.FixedQuantityPlans != null && meter.FixedQuantityPlans != null && this.FixedQuantityPlans.SequenceEqual<AzureOfferPlanDefinition>(meter.FixedQuantityPlans) || this.FixedQuantityPlans == null && meter.FixedQuantityPlans == null) && this.GalleryId == meter.GalleryId && this.IncludedQuantity == meter.IncludedQuantity && this.MaximumQuantity == meter.MaximumQuantity && this.MeterId == meter.MeterId && this.Name == meter.Name && this.OfferScope == meter.OfferScope && this.PlatformMeterId == meter.PlatformMeterId && (int) this.PreviewGraceDays == (int) meter.PreviewGraceDays && this.RenewalFrequency == meter.RenewalFrequency && this.Status == meter.Status && this.TrialCycles == meter.TrialCycles && (int) this.TrialDays == (int) meter.TrialDays && this.Unit == meter.Unit && this.MinimumRequiredAccessLevel == meter.MinimumRequiredAccessLevel && this.IncludedInLicenseLevel == meter.IncludedInLicenseLevel && this.IsFirstParty == meter.IsFirstParty && this.AutoAssignOnAccess == meter.AutoAssignOnAccess;
    }

    public OfferMeter Clone()
    {
      OfferMeter offerMeter = new OfferMeter()
      {
        AbsoluteMaximumQuantity = this.AbsoluteMaximumQuantity,
        AssignmentModel = this.AssignmentModel,
        BillingEntity = this.BillingEntity,
        BillingMode = this.BillingMode,
        BillingStartDate = this.BillingStartDate,
        BillingState = this.BillingState,
        Category = this.Category,
        CommittedQuantity = this.CommittedQuantity,
        CurrentQuantity = this.CurrentQuantity,
        GalleryId = this.GalleryId,
        IncludedInLicenseLevel = this.IncludedInLicenseLevel,
        IncludedQuantity = this.IncludedQuantity,
        MaximumQuantity = this.MaximumQuantity,
        MeterId = this.MeterId,
        Name = this.Name,
        OfferScope = this.OfferScope,
        PlatformMeterId = this.PlatformMeterId,
        PreviewGraceDays = this.PreviewGraceDays,
        RenewalFrequency = this.RenewalFrequency,
        Status = this.Status,
        TrialCycles = this.TrialCycles,
        TrialDays = this.TrialDays,
        Unit = this.Unit,
        MinimumRequiredAccessLevel = this.MinimumRequiredAccessLevel,
        AutoAssignOnAccess = this.AutoAssignOnAccess
      };
      if (this.FixedQuantityPlans != null)
      {
        List<AzureOfferPlanDefinition> offerPlanDefinitionList = new List<AzureOfferPlanDefinition>();
        offerMeter.FixedQuantityPlans = (IEnumerable<AzureOfferPlanDefinition>) offerPlanDefinitionList;
        foreach (AzureOfferPlanDefinition fixedQuantityPlan in this.FixedQuantityPlans)
          offerPlanDefinitionList.Add(fixedQuantityPlan.Clone());
      }
      return offerMeter;
    }

    public override bool Equals(object obj) => this.Equals(obj as OfferMeter);

    public override int GetHashCode() => this.MeterId.GetHashCode();

    public static bool operator ==(OfferMeter left, OfferMeter right) => (object) left == null || (object) right == null ? object.Equals((object) left, (object) right) : left.Equals(right);

    public static bool operator !=(OfferMeter left, OfferMeter right) => !(left == right);
  }
}
