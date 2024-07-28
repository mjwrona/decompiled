// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionResourceUsage
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts
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
