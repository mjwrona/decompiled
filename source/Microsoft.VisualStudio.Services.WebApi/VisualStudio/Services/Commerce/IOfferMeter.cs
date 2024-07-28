// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IOfferMeter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public interface IOfferMeter
  {
    int MeterId { get; set; }

    string Name { get; set; }

    ResourceBillingMode BillingMode { get; set; }

    MeterBillingState BillingState { get; set; }

    MeterCategory Category { get; set; }

    int CommittedQuantity { get; set; }

    int CurrentQuantity { get; set; }

    string GalleryId { get; set; }

    int IncludedQuantity { get; set; }

    int MaximumQuantity { get; set; }

    int AbsoluteMaximumQuantity { get; set; }

    OfferScope OfferScope { get; set; }

    Guid PlatformMeterId { get; set; }

    MeterRenewalFrequecy RenewalFrequency { get; set; }

    MeterState Status { get; set; }

    int TrialCycles { get; set; }

    string Unit { get; set; }

    OfferMeterAssignmentModel AssignmentModel { get; set; }

    DateTime? BillingStartDate { get; set; }

    byte TrialDays { get; set; }

    byte PreviewGraceDays { get; set; }

    BillingProvider BillingEntity { get; set; }

    MinimumRequiredServiceLevel MinimumRequiredAccessLevel { get; set; }

    MinimumRequiredServiceLevel IncludedInLicenseLevel { get; set; }

    IEnumerable<AzureOfferPlanDefinition> FixedQuantityPlans { get; set; }

    bool IsFirstParty { get; }

    bool AutoAssignOnAccess { get; set; }
  }
}
