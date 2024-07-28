// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureBillableEvent2
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureBillableEvent2 : TableEntity, IAzureBillableEvent, ITableEntity
  {
    public AzureBillableEvent2()
    {
    }

    public AzureBillableEvent2(AzureBillableEventContract billableEvent)
    {
      DateTime eventDateTime = billableEvent.EventDateTime;
      this.PartitionKey = this.GeneratePartitionKey(eventDateTime);
      this.RowKey = string.Format("{0}_{1}", (object) eventDateTime.Ticks.ToString(), (object) billableEvent.EventId);
      this.SubscriptionId = billableEvent.SubscriptionId.ToString();
      this.EventId = billableEvent.EventId;
      this.EventDateTime = billableEvent.EventDateTime;
      this.Quantity = billableEvent.Quantity;
      this.MeterId = billableEvent.MeterId;
      this.ResourceUri = billableEvent.ResourceUri;
      string str = billableEvent.Location;
      if (string.IsNullOrEmpty(str))
        str = "northcentralus";
      this.Location = str;
      this.Tags = billableEvent.Tags;
      this.PartNumber = billableEvent.PartNumber;
      this.AdditionalInfo = billableEvent.AdditionalInfo;
      this.OrderNumber = billableEvent.OrderNumber;
    }

    internal AzureBillableEvent2(
      BillableEvent billableEvent,
      string location,
      string resourceUri,
      Dictionary<string, string> tags = null)
    {
      this.PartitionKey = this.GeneratePartitionKey(billableEvent.EventUtcTime);
      this.RowKey = string.Format("{0}_{1}", (object) billableEvent.EventUtcTime.Ticks.ToString(), (object) billableEvent.EventUniqueId);
      this.SubscriptionId = billableEvent.SubscriptionId.ToString();
      this.EventId = billableEvent.EventUniqueId;
      this.EventDateTime = billableEvent.EventUtcTime;
      this.Quantity = billableEvent.Quantity;
      this.MeterId = billableEvent.MeterPlatformGuid.ToString();
      this.ResourceUri = resourceUri;
      if (string.IsNullOrEmpty(location))
        location = "northcentralus";
      this.Location = location;
      if (tags == null || tags.Count <= 0)
        return;
      this.Tags = tags.Serialize<Dictionary<string, string>>();
    }

    public string GetTableName() => "CommerceBillableEventsTable2";

    internal string ToStringValue()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Partition Key:");
      stringBuilder.Append(this.PartitionKey);
      stringBuilder.Append(" Row Key:");
      stringBuilder.Append(this.RowKey);
      stringBuilder.Append(" Subscription ID:");
      stringBuilder.Append(this.SubscriptionId);
      stringBuilder.Append(" Event ID:");
      stringBuilder.Append((object) this.EventId);
      stringBuilder.Append(" EventDateTime:");
      stringBuilder.Append((object) this.EventDateTime);
      stringBuilder.Append(" Quantity:");
      stringBuilder.Append(this.Quantity);
      stringBuilder.Append(" Meter ID:");
      stringBuilder.Append(this.MeterId);
      stringBuilder.Append(" ResourceUri:");
      stringBuilder.Append(this.ResourceUri);
      stringBuilder.Append(" Location:");
      stringBuilder.Append(this.Location);
      stringBuilder.Append(" Tags:");
      stringBuilder.Append(this.Tags?.ToString());
      stringBuilder.Append(" Part#:");
      stringBuilder.Append(this.PartNumber);
      stringBuilder.Append(" Order#:");
      stringBuilder.Append(this.OrderNumber);
      stringBuilder.Append(" Additional Info:");
      stringBuilder.Append(this.AdditionalInfo?.ToString());
      return stringBuilder.ToString();
    }

    internal string ToJsonStringValue() => string.Format("{{\r\n                                    \"partitionKey\" : \"{0}\",\r\n                                    \"rowKey\" : \"{1}\",\r\n\t\t                            \"subscriptionId\" : \"{2}\",\r\n\t\t                            \"eventId\" : \"{3}\",\r\n\t\t                            \"eventDateTime\" : \"{4}\",\r\n\t\t                            \"quantity\" : \"{5}\",\r\n\t\t                            \"meterId\" : \"{6}\",\r\n\t\t                            \"resourceUri\" : \"{7}\",\r\n\t\t                            \"location\" : \"{8}\",\r\n\t\t                            \"tags\" : \"{9}\",\r\n\t\t                            \"partNumber\" : \"{10}\",\r\n\t\t                            \"orderNumber\" : \"{11}\",\r\n\t\t                            \"additionalInfo\" : \"{12}\",\r\n                                   }}", (object) this.PartitionKey, (object) this.RowKey, (object) this.SubscriptionId, (object) this.EventId, (object) this.EventDateTime, (object) this.Quantity, (object) this.MeterId, (object) this.ResourceUri, (object) this.Location, (object) this.Tags?.ToString(), (object) this.PartNumber, (object) this.OrderNumber, (object) this.AdditionalInfo?.ToString());

    public bool Validate(out IList<string> failedProperties)
    {
      failedProperties = (IList<string>) new List<string>();
      if (string.IsNullOrEmpty(this.PartitionKey))
        failedProperties.Add("PartitionKey");
      if (string.IsNullOrEmpty(this.RowKey))
        failedProperties.Add("RowKey");
      if (string.IsNullOrEmpty(this.SubscriptionId))
        failedProperties.Add("SubscriptionId");
      if (string.IsNullOrEmpty(this.MeterId))
        failedProperties.Add("MeterId");
      if (string.IsNullOrEmpty(this.ResourceUri))
        failedProperties.Add("ResourceUri");
      if (string.IsNullOrEmpty(this.Location))
        failedProperties.Add("Location");
      return !failedProperties.Any<string>();
    }

    public AzureBillableEvent2 UpdatePartitionKey(string eventTimeStamp)
    {
      this.PartitionKey = eventTimeStamp;
      return this;
    }

    public string SubscriptionId { set; get; }

    public Guid EventId { set; get; }

    public DateTime EventDateTime { set; get; }

    public double Quantity { set; get; }

    public string MeterId { set; get; }

    public string ResourceUri { set; get; }

    public string Tags { set; get; }

    public string Location { set; get; }

    [DataMember]
    public string PartNumber { set; get; }

    public string OrderNumber { set; get; }

    public object AdditionalInfo { set; get; }

    internal string GeneratePartitionKey(DateTime eventTimeStamp) => eventTimeStamp.Ticks.ToString();
  }
}
