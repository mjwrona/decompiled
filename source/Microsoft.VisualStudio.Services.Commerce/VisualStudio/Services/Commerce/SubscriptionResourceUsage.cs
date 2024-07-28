// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class SubscriptionResourceUsage
  {
    public int ResourceId { get; set; }

    public byte ResourceSeq { get; set; }

    public int CurrentQuantity { get; set; }

    public int CommittedQuantity { get; set; }

    public int IncludedQuantity { get; set; }

    public int MaxQuantity { get; set; }

    public bool IsPaidBillingEnabled { get; set; }

    public DateTime PaidBillingUpdated { get; set; }

    public DateTime LastResetDate { get; set; }

    public DateTime LastUpdated { get; set; }

    public Guid LastUpdatedBy { get; set; }

    public bool IsTrialOrPreview { get; set; }

    public DateTime StartDate { get; set; }

    public bool AutoAssignOnAccess { get; set; }

    public int TrialDays { get; set; }
  }
}
