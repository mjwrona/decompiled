// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class SubscriptionResourceComparer : IEqualityComparer<ISubscriptionResource>
  {
    public bool Equals(ISubscriptionResource x, ISubscriptionResource y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.Name == y.Name && x.CommittedQuantity == y.CommittedQuantity && x.DisabledResourceActionLink == y.DisabledResourceActionLink && x.DisabledReason == y.DisabledReason && x.IncludedQuantity == y.IncludedQuantity && x.IsUseable == y.IsUseable && x.IsPaidBillingEnabled == y.IsPaidBillingEnabled && x.MaximumQuantity == y.MaximumQuantity;
    }

    public int GetHashCode(ISubscriptionResource x)
    {
      ResourceName name = x.Name;
      int hashCode1 = name.GetHashCode();
      name = x.Name;
      int hashCode2 = name.GetHashCode();
      return hashCode1 ^ hashCode2;
    }
  }
}
