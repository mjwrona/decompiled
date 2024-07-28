// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.MeteredResource
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class MeteredResource : ICloneable
  {
    public int CommittedQuantity { get; set; }

    public int CurrentQuantity { get; set; }

    public int IncludedQuantity { get; set; }

    public bool IsPaidBillingEnabled { get; set; }

    public DateTime PaidBillingUpdatedDate { get; set; }

    public ResourceName ResourceName { get; set; }

    public int MaximumQuantity { get; set; }

    public ResourceBillingMode BillingMode { get; set; }

    public int AbsoluteMaximumQuantity { get; set; }

    public Guid CommercePlatformMeterId { get; set; }

    public AccountProviderNamespace ProviderNamespaceId { get; set; }

    public string Unit { get; set; }

    public DateTime LastResetDate { get; set; }

    public MeterGroupType MeterGroup { get; set; }

    object ICloneable.Clone() => this.MemberwiseClone();

    public MeteredResource Clone() => (MeteredResource) this.MemberwiseClone();
  }
}
