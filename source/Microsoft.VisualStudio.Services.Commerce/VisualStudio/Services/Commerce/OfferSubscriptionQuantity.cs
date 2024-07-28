// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferSubscriptionQuantity
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class OfferSubscriptionQuantity : ICloneable
  {
    public Guid AzureSubscriptionId { get; set; }

    public int MeterId { get; set; }

    public int IncludedQuantity { get; set; }

    public DateTime LastUpdated { get; set; }

    public Guid LastUpdatedBy { get; set; }

    public object Clone() => (object) new OfferSubscriptionQuantity()
    {
      AzureSubscriptionId = this.AzureSubscriptionId,
      MeterId = this.MeterId,
      IncludedQuantity = this.IncludedQuantity,
      LastUpdated = this.LastUpdated,
      LastUpdatedBy = this.LastUpdatedBy
    };
  }
}
