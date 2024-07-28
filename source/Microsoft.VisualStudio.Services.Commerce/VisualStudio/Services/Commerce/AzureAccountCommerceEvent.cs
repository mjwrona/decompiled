// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureAccountCommerceEvent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureAccountCommerceEvent : TableEntity, ICommerceEvent
  {
    public AzureAccountCommerceEvent()
    {
    }

    public AzureAccountCommerceEvent(Guid collectionId, string eventId)
    {
      this.PartitionKey = collectionId.ToString();
      this.RowKey = this.EventId = eventId;
      this.CollectionId = collectionId;
    }

    public string EventId { get; set; }

    public DateTime EventTime { get; set; }

    public string EventName { get; set; }

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; }

    public Guid CollectionId { get; set; }

    public string CollectionName { get; set; }

    public Guid SubscriptionId { get; set; }

    public string SubscriptionName { get; set; }

    public string MeterName { get; set; }

    public string GalleryId { get; set; }

    public int CommittedQuantity { get; set; }

    public int CurrentQuantity { get; set; }

    public int PreviousQuantity { get; set; }

    public double BilledQuantity { get; set; }

    public int IncludedQuantity { get; set; }

    public int? PreviousIncludedQuantity { get; set; }

    public int MaxQuantity { get; set; }

    public int? PreviousMaxQuantity { get; set; }

    public string RenewalGroup { get; set; }

    public string EventSource { get; set; }

    public string Environment { get; set; }

    public Guid UserIdentity { get; set; }

    public Guid ServiceIdentity { get; set; }

    public DateTime? TrialStartDate { get; set; }

    public DateTime? TrialEndDate { get; set; }

    public DateTime? EffectiveDate { get; set; }

    public string Version { get; set; }
  }
}
