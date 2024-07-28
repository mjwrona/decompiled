// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureBillableEvent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureBillableEvent : TableEntity, IAzureBillableEvent, ITableEntity
  {
    public AzureBillableEvent()
    {
    }

    public AzureBillableEvent(DateTime eventTimeStamp, string rowKey)
    {
      this.N = new Random().Next(0, 100).ToString().PadLeft(19, '0');
      this.PartitionKey = this.GeneratePartitionKey(eventTimeStamp);
      this.RowKey = rowKey;
      this.EventTicks = rowKey.Split('_')[0];
      this.UniqueIdentifier = rowKey.Substring(this.EventTicks.Length + 1);
    }

    internal AzureBillableEvent(BillableEvent billableEvent)
    {
      this.N = new Random().Next(0, 100).ToString().PadLeft(19, '0');
      this.PartitionKey = this.GeneratePartitionKey(billableEvent.EventUtcTime);
      this.RowKey = string.Format("{0}_{1}", (object) billableEvent.EventUtcTime.Ticks.ToString(), (object) billableEvent.EventUniqueId);
      this.EventTicks = billableEvent.EventUtcTime.Ticks.ToString();
      this.UniqueIdentifier = billableEvent.EventUniqueId.ToString();
      this.AccountName = billableEvent.AccountName;
      this.SubscriptionId = billableEvent.SubscriptionId.ToString();
      this.UsageResourceQuantity = billableEvent.Quantity;
      this.ResourceGuid = billableEvent.MeterPlatformGuid.ToString();
      this.DataCenterCode = billableEvent.DataCenterCode;
      this.AccountId = billableEvent.AccountId;
    }

    public string GetTableName() => "CommerceBillableEventsTable";

    public string N { get; set; }

    public string AccountName { get; set; }

    public string SubscriptionId { get; set; }

    public double UsageResourceQuantity { get; set; }

    public string ResourceGuid { get; set; }

    public string DataCenterCode { get; set; }

    public string UniqueIdentifier { get; set; }

    public string EventTicks { get; set; }

    public Guid AccountId { get; set; }

    public bool Validate(out IList<string> failedProperties)
    {
      failedProperties = (IList<string>) new List<string>();
      if (string.IsNullOrEmpty(this.N))
        failedProperties.Add("N");
      if (string.IsNullOrEmpty(this.PartitionKey))
        failedProperties.Add("PartitionKey");
      if (string.IsNullOrEmpty(this.RowKey))
        failedProperties.Add("RowKey");
      if (string.IsNullOrEmpty(this.AccountName))
        failedProperties.Add("AccountName");
      if (string.IsNullOrEmpty(this.SubscriptionId))
        failedProperties.Add("SubscriptionId");
      if (string.IsNullOrEmpty(this.ResourceGuid))
        failedProperties.Add("ResourceGuid");
      if (string.IsNullOrEmpty(this.DataCenterCode))
        failedProperties.Add("DataCenterCode");
      if (string.IsNullOrEmpty(this.UniqueIdentifier))
        failedProperties.Add("UniqueIdentifier");
      if (string.IsNullOrEmpty(this.EventTicks))
        failedProperties.Add("EventTicks");
      if (Guid.Empty.Equals(this.AccountId))
        failedProperties.Add("AccountId");
      return !failedProperties.Any<string>();
    }

    public AzureBillableEvent UpdatePartitionKey(DateTime eventTimeStamp)
    {
      this.PartitionKey = this.GeneratePartitionKey(eventTimeStamp);
      return this;
    }

    private string GeneratePartitionKey(DateTime eventTimeStamp) => string.Join("___", new string[2]
    {
      this.N,
      eventTimeStamp.Ticks.ToString().PadLeft(19, '0')
    });
  }
}
