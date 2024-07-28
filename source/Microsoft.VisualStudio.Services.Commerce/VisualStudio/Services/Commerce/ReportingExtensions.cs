// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ReportingExtensions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class ReportingExtensions
  {
    public static AzurePublisherCommerceEvent ToAzurePublisherCommerceEvent(
      this AzureReportingEvent reportingEvent)
    {
      Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(reportingEvent.Properties);
      AzurePublisherCommerceEvent publisherCommerceEvent = new AzurePublisherCommerceEvent(dictionary["GalleryId"], reportingEvent.EventId)
      {
        EventTime = reportingEvent.EventTime,
        EventName = reportingEvent.EventName,
        OrganizationId = reportingEvent.OrganizationId,
        OrganizationName = reportingEvent.OrganizationName,
        CollectionId = reportingEvent.CollectionId,
        CollectionName = reportingEvent.CollectionName,
        Environment = reportingEvent.Environment,
        UserIdentity = reportingEvent.UserIdentity,
        ServiceIdentity = reportingEvent.ServiceIdentity,
        Version = reportingEvent.Version
      };
      string str;
      if (dictionary.TryGetValue("SubscriptionId", out str))
        publisherCommerceEvent.SubscriptionId = Guid.Parse(str);
      if (dictionary.TryGetValue("MeterName", out str))
        publisherCommerceEvent.MeterName = str;
      if (dictionary.TryGetValue("CommittedQuantity", out str))
        publisherCommerceEvent.CommittedQuantity = int.Parse(str);
      if (dictionary.TryGetValue("CurrentQuantity", out str))
        publisherCommerceEvent.CurrentQuantity = int.Parse(str);
      if (dictionary.TryGetValue("PreviousQuantity", out str))
        publisherCommerceEvent.PreviousQuantity = int.Parse(str);
      if (dictionary.TryGetValue("BilledQuantity", out str))
        publisherCommerceEvent.BilledQuantity = double.Parse(str);
      if (dictionary.TryGetValue("IncludedQuantity", out str))
        publisherCommerceEvent.IncludedQuantity = int.Parse(str);
      if (dictionary.TryGetValue("PreviousIncludedQuantity", out str))
        publisherCommerceEvent.PreviousIncludedQuantity = new int?(int.Parse(str));
      if (dictionary.TryGetValue("MaxQuantity", out str))
        publisherCommerceEvent.MaxQuantity = int.Parse(str);
      if (dictionary.TryGetValue("PreviousMaxQuantity", out str))
        publisherCommerceEvent.PreviousMaxQuantity = new int?(int.Parse(str));
      if (dictionary.TryGetValue("RenewalGroup", out str))
        publisherCommerceEvent.RenewalGroup = str;
      if (dictionary.TryGetValue("EventSource", out str))
        publisherCommerceEvent.EventSource = str;
      if (dictionary.TryGetValue("TrialStartDate", out str))
        publisherCommerceEvent.TrialStartDate = new DateTime?(DateTime.Parse(str));
      if (dictionary.TryGetValue("TrialEndDate", out str))
        publisherCommerceEvent.TrialEndDate = new DateTime?(DateTime.Parse(str));
      if (dictionary.TryGetValue("EffectiveDate", out str))
        publisherCommerceEvent.EffectiveDate = new DateTime?(DateTime.Parse(str));
      return publisherCommerceEvent;
    }

    public static AzureAccountCommerceEvent ToAzureAccountCommerceEvent(
      this AzureReportingEvent reportingEvent)
    {
      Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(reportingEvent.Properties);
      AzureAccountCommerceEvent accountCommerceEvent = new AzureAccountCommerceEvent(reportingEvent.CollectionId, reportingEvent.EventId)
      {
        EventTime = reportingEvent.EventTime,
        EventName = reportingEvent.EventName,
        OrganizationId = reportingEvent.OrganizationId,
        OrganizationName = reportingEvent.OrganizationName,
        CollectionName = reportingEvent.CollectionName,
        Environment = reportingEvent.Environment,
        UserIdentity = reportingEvent.UserIdentity,
        ServiceIdentity = reportingEvent.ServiceIdentity,
        Version = reportingEvent.Version
      };
      string str;
      if (dictionary.TryGetValue("SubscriptionId", out str))
        accountCommerceEvent.SubscriptionId = Guid.Parse(str);
      if (dictionary.TryGetValue("MeterName", out str))
        accountCommerceEvent.MeterName = str;
      if (dictionary.TryGetValue("GalleryId", out str))
        accountCommerceEvent.GalleryId = str;
      if (dictionary.TryGetValue("CommittedQuantity", out str))
        accountCommerceEvent.CommittedQuantity = int.Parse(str);
      if (dictionary.TryGetValue("CurrentQuantity", out str))
        accountCommerceEvent.CurrentQuantity = int.Parse(str);
      if (dictionary.TryGetValue("PreviousQuantity", out str))
        accountCommerceEvent.PreviousQuantity = int.Parse(str);
      if (dictionary.TryGetValue("BilledQuantity", out str))
        accountCommerceEvent.BilledQuantity = double.Parse(str);
      if (dictionary.TryGetValue("IncludedQuantity", out str))
        accountCommerceEvent.IncludedQuantity = int.Parse(str);
      if (dictionary.TryGetValue("PreviousIncludedQuantity", out str))
        accountCommerceEvent.PreviousIncludedQuantity = new int?(int.Parse(str));
      if (dictionary.TryGetValue("MaxQuantity", out str))
        accountCommerceEvent.MaxQuantity = int.Parse(str);
      if (dictionary.TryGetValue("PreviousMaxQuantity", out str))
        accountCommerceEvent.PreviousMaxQuantity = new int?(int.Parse(str));
      if (dictionary.TryGetValue("RenewalGroup", out str))
        accountCommerceEvent.RenewalGroup = str;
      if (dictionary.TryGetValue("EventSource", out str))
        accountCommerceEvent.EventSource = str;
      if (dictionary.TryGetValue("TrialStartDate", out str))
        accountCommerceEvent.TrialStartDate = new DateTime?(DateTime.Parse(str));
      if (dictionary.TryGetValue("TrialEndDate", out str))
        accountCommerceEvent.TrialEndDate = new DateTime?(DateTime.Parse(str));
      if (dictionary.TryGetValue("EffectiveDate", out str))
        accountCommerceEvent.EffectiveDate = new DateTime?(DateTime.Parse(str));
      return accountCommerceEvent;
    }

    public static CommerceEvent ToCommerceEvent(this AzurePublisherCommerceEvent reportingEvent) => new CommerceEvent()
    {
      EventId = reportingEvent.EventId,
      EventTime = reportingEvent.EventTime,
      EventName = reportingEvent.EventName,
      OrganizationId = reportingEvent.OrganizationId,
      OrganizationName = reportingEvent.OrganizationName,
      CollectionId = reportingEvent.CollectionId,
      CollectionName = reportingEvent.CollectionName,
      SubscriptionId = reportingEvent.SubscriptionId,
      MeterName = reportingEvent.MeterName,
      GalleryId = reportingEvent.GalleryId,
      CommittedQuantity = reportingEvent.CommittedQuantity,
      CurrentQuantity = reportingEvent.CurrentQuantity,
      PreviousQuantity = reportingEvent.PreviousQuantity,
      BilledQuantity = reportingEvent.BilledQuantity,
      IncludedQuantity = reportingEvent.IncludedQuantity,
      MaxQuantity = reportingEvent.MaxQuantity,
      PreviousIncludedQuantity = reportingEvent.PreviousIncludedQuantity,
      PreviousMaxQuantity = reportingEvent.PreviousMaxQuantity,
      RenewalGroup = reportingEvent.RenewalGroup,
      EventSource = reportingEvent.EventSource,
      Environment = reportingEvent.Environment,
      UserIdentity = reportingEvent.UserIdentity,
      ServiceIdentity = reportingEvent.ServiceIdentity,
      TrialStartDate = reportingEvent.TrialStartDate,
      TrialEndDate = reportingEvent.TrialEndDate,
      EffectiveDate = reportingEvent.EffectiveDate,
      Version = reportingEvent.Version
    };

    public static CommerceEvent ToCommerceEvent(this AzureAccountCommerceEvent reportingEvent) => new CommerceEvent()
    {
      EventId = reportingEvent.EventId,
      EventTime = reportingEvent.EventTime,
      EventName = reportingEvent.EventName,
      OrganizationId = reportingEvent.OrganizationId,
      OrganizationName = reportingEvent.OrganizationName,
      CollectionId = reportingEvent.CollectionId,
      CollectionName = reportingEvent.CollectionName,
      SubscriptionId = reportingEvent.SubscriptionId,
      MeterName = reportingEvent.MeterName,
      GalleryId = reportingEvent.GalleryId,
      CommittedQuantity = reportingEvent.CommittedQuantity,
      CurrentQuantity = reportingEvent.CurrentQuantity,
      PreviousQuantity = reportingEvent.PreviousQuantity,
      BilledQuantity = reportingEvent.BilledQuantity,
      IncludedQuantity = reportingEvent.IncludedQuantity,
      MaxQuantity = reportingEvent.MaxQuantity,
      PreviousIncludedQuantity = reportingEvent.PreviousIncludedQuantity,
      PreviousMaxQuantity = reportingEvent.PreviousMaxQuantity,
      RenewalGroup = reportingEvent.RenewalGroup,
      EventSource = reportingEvent.EventSource,
      Environment = reportingEvent.Environment,
      UserIdentity = reportingEvent.UserIdentity,
      ServiceIdentity = reportingEvent.ServiceIdentity,
      TrialStartDate = reportingEvent.TrialStartDate,
      TrialEndDate = reportingEvent.TrialEndDate,
      EffectiveDate = reportingEvent.EffectiveDate,
      Version = reportingEvent.Version
    };

    public static AzureUsageEvent ToAzureUsageEvent(this UsageEvent usageEvent, bool isProcessed) => new AzureUsageEvent(usageEvent.EventTimestamp, usageEvent.EventUniqueId)
    {
      PartitionId = usageEvent.PartitionId,
      AccountName = usageEvent.AccountName,
      BillEventDateTime = usageEvent.BillableDate,
      EventId = usageEvent.EventId,
      ReportedQuantity = usageEvent.Quantity,
      ResourceType = usageEvent.MeterName,
      ServiceIdentity = usageEvent.ServiceIdentity,
      UserIdentity = usageEvent.AssociatedUser,
      ResourceBillingMode = usageEvent.ResourceBillingMode.ToString(),
      SubscriptionId = usageEvent.SubscriptionId,
      SubscriptionAnniversaryDay = usageEvent.SubscriptionAnniversaryDay,
      AccountId = usageEvent.AccountId
    };

    public static AzureReportingEvent ToAzureReportingEvent(this ReportingEvent reportingEvent) => new AzureReportingEvent(reportingEvent.EventTime, reportingEvent.EventId)
    {
      EventName = reportingEvent.EventName,
      OrganizationId = reportingEvent.OrganizationId,
      OrganizationName = reportingEvent.OrganizationName,
      CollectionId = reportingEvent.CollectionId,
      CollectionName = reportingEvent.CollectionName,
      Environment = reportingEvent.Environment,
      UserIdentity = reportingEvent.UserIdentity,
      ServiceIdentity = reportingEvent.ServiceIdentity,
      Version = reportingEvent.Version,
      Properties = JsonConvert.SerializeObject((object) reportingEvent.Properties)
    };

    public static UsageEvent ToUsageEvent(this AzureUsageEvent azureUsageEvent) => new UsageEvent()
    {
      PartitionId = azureUsageEvent.PartitionId,
      AccountName = azureUsageEvent.AccountName,
      BillableDate = azureUsageEvent.BillEventDateTime,
      EventTimestamp = new DateTime(azureUsageEvent.EventTicks, DateTimeKind.Utc),
      AssociatedUser = azureUsageEvent.UserIdentity,
      EventId = azureUsageEvent.RowKey,
      Quantity = azureUsageEvent.ReportedQuantity,
      MeterName = azureUsageEvent.ResourceType,
      ServiceIdentity = azureUsageEvent.ServiceIdentity,
      ResourceBillingMode = (ResourceBillingMode) Enum.Parse(typeof (ResourceBillingMode), azureUsageEvent.ResourceBillingMode),
      SubscriptionId = azureUsageEvent.SubscriptionId,
      SubscriptionAnniversaryDay = azureUsageEvent.SubscriptionAnniversaryDay,
      AccountId = azureUsageEvent.AccountId,
      EventUniqueId = azureUsageEvent.UniqueIdentifier
    };
  }
}
