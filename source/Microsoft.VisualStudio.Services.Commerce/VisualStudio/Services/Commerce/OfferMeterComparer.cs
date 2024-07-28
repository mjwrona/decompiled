// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class OfferMeterComparer : IEqualityComparer<IOfferMeter>
  {
    public bool Equals(IOfferMeter x, IOfferMeter y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.AbsoluteMaximumQuantity == y.AbsoluteMaximumQuantity && x.AssignmentModel == y.AssignmentModel && x.BillingEntity == y.BillingEntity && x.BillingMode == y.BillingMode && x.BillingState == y.BillingState && x.Category == y.Category && x.CommittedQuantity == y.CommittedQuantity && x.CurrentQuantity == y.CurrentQuantity && (x.FixedQuantityPlans != null && y.FixedQuantityPlans != null && x.FixedQuantityPlans.SequenceEqual<AzureOfferPlanDefinition>(y.FixedQuantityPlans) || x.FixedQuantityPlans == null && y.FixedQuantityPlans == null) && x.GalleryId == y.GalleryId && x.IncludedQuantity == y.IncludedQuantity && x.MaximumQuantity == y.MaximumQuantity && x.MeterId == y.MeterId && x.Name == y.Name && x.OfferScope == y.OfferScope && x.PlatformMeterId == y.PlatformMeterId && (int) x.PreviewGraceDays == (int) y.PreviewGraceDays && x.RenewalFrequency == y.RenewalFrequency && x.Status == y.Status && x.TrialCycles == y.TrialCycles && (int) x.TrialDays == (int) y.TrialDays && x.Unit == y.Unit && x.MinimumRequiredAccessLevel == y.MinimumRequiredAccessLevel && x.IncludedInLicenseLevel == y.IncludedInLicenseLevel && x.IsFirstParty == y.IsFirstParty && x.AutoAssignOnAccess == y.AutoAssignOnAccess;
    }

    public int GetHashCode(IOfferMeter x) => x.MeterId.GetHashCode();
  }
}
