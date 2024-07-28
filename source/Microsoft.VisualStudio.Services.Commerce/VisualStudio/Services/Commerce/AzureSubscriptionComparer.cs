// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureSubscriptionComparer : IEqualityComparer<IAzureSubscription>
  {
    public bool Equals(IAzureSubscription x, IAzureSubscription y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null || !(x.Id == y.Id) || x.Status != y.Status || x.Namespace != y.Namespace)
        return false;
      AzureOfferType? offerType1 = x.OfferType;
      AzureOfferType? offerType2 = y.OfferType;
      return offerType1.GetValueOrDefault() == offerType2.GetValueOrDefault() & offerType1.HasValue == offerType2.HasValue && x.Source == y.Source && x.AnniversaryDay == y.AnniversaryDay;
    }

    public int GetHashCode(IAzureSubscription x) => x.Id.GetHashCode();
  }
}
