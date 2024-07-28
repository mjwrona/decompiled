// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceEventComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommerceEventComparer : IEqualityComparer<ICommerceEvent>
  {
    public bool Equals(ICommerceEvent x, ICommerceEvent y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null || !(x.EventId == y.EventId) || !(x.EventName == y.EventName) || !(x.OrganizationId == y.OrganizationId) || !(x.OrganizationName == y.OrganizationName) || !(x.CollectionId == y.CollectionId) || !(x.CollectionName == y.CollectionName) || !(x.SubscriptionId == y.SubscriptionId) || !(x.MeterName == y.MeterName) || !(x.GalleryId == y.GalleryId) || x.CommittedQuantity != y.CommittedQuantity || x.CurrentQuantity != y.CurrentQuantity || x.PreviousQuantity != y.PreviousQuantity || x.BilledQuantity != y.BilledQuantity || x.IncludedQuantity != y.IncludedQuantity)
        return false;
      int? nullable = x.PreviousIncludedQuantity;
      int? includedQuantity = y.PreviousIncludedQuantity;
      if (nullable.GetValueOrDefault() == includedQuantity.GetValueOrDefault() & nullable.HasValue == includedQuantity.HasValue && x.MaxQuantity == y.MaxQuantity)
      {
        int? previousMaxQuantity = x.PreviousMaxQuantity;
        nullable = y.PreviousMaxQuantity;
        if (previousMaxQuantity.GetValueOrDefault() == nullable.GetValueOrDefault() & previousMaxQuantity.HasValue == nullable.HasValue && x.RenewalGroup == y.RenewalGroup && x.EventSource == y.EventSource && x.Environment == y.Environment && x.UserIdentity == y.UserIdentity && x.ServiceIdentity == y.ServiceIdentity)
          return x.Version == y.Version;
      }
      return false;
    }

    public int GetHashCode(ICommerceEvent x) => x.EventId.GetHashCode();
  }
}
