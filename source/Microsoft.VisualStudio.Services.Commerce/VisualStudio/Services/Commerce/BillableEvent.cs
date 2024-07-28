// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.BillableEvent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class BillableEvent
  {
    public BillableEvent() => this.DataCenterCode = "CH";

    public Guid EventUniqueId { get; set; }

    public DateTime EventUtcTime { get; set; }

    public string AccountName { get; set; }

    public Guid SubscriptionId { get; set; }

    public double Quantity { get; set; }

    public Guid MeterPlatformGuid { get; set; }

    public string DataCenterCode { get; set; }

    public Guid AccountId { get; set; }

    public int AccountPartitionId { get; set; }
  }
}
