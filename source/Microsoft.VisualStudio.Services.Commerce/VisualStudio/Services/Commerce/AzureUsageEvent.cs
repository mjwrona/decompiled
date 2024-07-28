// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureUsageEvent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureUsageEvent : TableEntity
  {
    public AzureUsageEvent()
    {
    }

    public AzureUsageEvent(DateTime eventTimeStamp, Guid uniqueId)
    {
      this.UniqueIdentifier = uniqueId;
      this.EventTicks = eventTimeStamp.Ticks;
      this.PartitionKey = eventTimeStamp.ToString("yyyy-MM-dd-HH");
      this.RowKey = string.Format("{0}_{1}", (object) eventTimeStamp.Ticks, (object) this.UniqueIdentifier);
    }

    public string EventId { get; set; }

    public int PartitionId { get; set; }

    public string AccountName { get; set; }

    public int ReportedQuantity { get; set; }

    public string ResourceType { get; set; }

    public Guid ServiceIdentity { get; set; }

    public Guid UserIdentity { get; set; }

    public long EventTicks { get; set; }

    public DateTime BillEventDateTime { get; set; }

    public string ResourceBillingMode { get; set; }

    public Guid UniqueIdentifier { get; set; }

    public Guid SubscriptionId { get; set; }

    public int SubscriptionAnniversaryDay { get; set; }

    public Guid AccountId { get; set; }
  }
}
