// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommercePackageComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommercePackageComparer : IEqualityComparer<ICommercePackage>
  {
    public bool Equals(ICommercePackage x, ICommercePackage y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.Configuration.SequenceEqual<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) y.Configuration) && CompareUtils.CompareSequence<IOfferMeter>((IEnumerable<IOfferMeter>) x.OfferMeters.OrderBy<OfferMeter, int>((Func<OfferMeter, int>) (m => m.MeterId)).ToList<OfferMeter>(), (IEnumerable<IOfferMeter>) y.OfferMeters.OrderBy<OfferMeter, int>((Func<OfferMeter, int>) (m => m.MeterId)).ToList<OfferMeter>(), (IEqualityComparer<IOfferMeter>) new OfferMeterComparer()) && CompareUtils.CompareSequence<IOfferSubscription>((IEnumerable<IOfferSubscription>) x.OfferSubscriptions.OrderBy<OfferSubscription, int>((Func<OfferSubscription, int>) (o => o.OfferMeter.MeterId)).ToList<OfferSubscription>(), (IEnumerable<IOfferSubscription>) y.OfferSubscriptions.OrderBy<OfferSubscription, int>((Func<OfferSubscription, int>) (o => o.OfferMeter.MeterId)).ToList<OfferSubscription>(), (IEqualityComparer<IOfferSubscription>) new OfferSubscriptionComparer());
    }

    public int GetHashCode(ICommercePackage x)
    {
      int hashCode = 0;
      foreach (OfferMeter offerMeter in x.OfferMeters)
        hashCode ^= offerMeter.MeterId;
      foreach (OfferSubscription offerSubscription in x.OfferSubscriptions)
        hashCode ^= offerSubscription.OfferMeter.MeterId ^ offerSubscription.CommittedQuantity;
      return hashCode;
    }
  }
}
