// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureReportingEvent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureReportingEvent : TableEntity
  {
    public AzureReportingEvent()
    {
    }

    public AzureReportingEvent(DateTime eventTime, string eventId)
    {
      this.PartitionKey = eventTime.ToString("yyyy-MM-dd-HH", (IFormatProvider) CultureInfo.InvariantCulture);
      this.EventTime = eventTime;
      this.RowKey = this.EventId = eventId;
    }

    public string EventId { get; set; }

    public DateTime EventTime { get; set; }

    public string EventName { get; set; }

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; }

    public Guid CollectionId { get; set; }

    public string CollectionName { get; set; }

    public string Environment { get; set; }

    public Guid UserIdentity { get; set; }

    public Guid ServiceIdentity { get; set; }

    public string Version { get; set; }

    public string Properties { get; set; }
  }
}
