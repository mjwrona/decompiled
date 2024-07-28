// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.UsageEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ExcludeFromCodeCoverage]
  public class UsageEvent
  {
    public string MeterName { get; set; }

    public string EventId { get; set; }

    public string AccountName { get; set; }

    public Guid AssociatedUser { get; set; }

    public int Quantity { get; set; }

    public DateTime BillableDate { get; set; }

    public DateTime EventTimestamp { get; set; }

    public Guid EventUniqueId { get; set; }

    public Guid ServiceIdentity { get; set; }

    public ResourceBillingMode ResourceBillingMode { get; set; }

    public Guid SubscriptionId { get; set; }

    public int SubscriptionAnniversaryDay { get; set; }

    public int PartitionId { get; set; }

    public Guid AccountId { get; set; }
  }
}
