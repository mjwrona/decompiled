// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AggregationTotal
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class AggregationTotal : TableEntity
  {
    public const string HourlyRowKeyFormat = "yyyy-MM-dd-HH";
    public const string DailyRowKeyFormat = "yyyy-MM-dd";

    public int Value { get; set; }

    public AggregationTotal(
      DateTime eventTime,
      string resourceName,
      Guid accountId,
      AggregationInterval interval)
    {
      this.PartitionKey = AggregationTotal.FormatPartitionKey(accountId, resourceName);
      this.RowKey = AggregationTotal.FormatRowKey(eventTime, interval);
    }

    public AggregationTotal()
    {
    }

    public static string FormatPartitionKey(Guid AccountId, string resourceName) => string.Format("{0}_{1}", (object) AccountId, (object) resourceName);

    public static string FormatRowKey(DateTime eventTime, AggregationInterval interval)
    {
      string format = (string) null;
      switch (interval)
      {
        case AggregationInterval.Hourly:
          format = "yyyy-MM-dd-HH";
          break;
        case AggregationInterval.Daily:
          format = "yyyy-MM-dd";
          break;
      }
      return eventTime.ToString(format);
    }
  }
}
