// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.UsageStoreEntityNames
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class UsageStoreEntityNames
  {
    internal const string MasterTableName = "CommerceMasterEventsTable";
    internal const string PublisherTableName = "CommercePublisherEventsTable";
    internal const string UsageTableName = "CommerceUsageEventsTable";
    internal const string BillingTableName = "CommerceBillableEventsTable";
    internal const string HourlyAggregationTableName = "CommerceUsageEventsHourlyAggregationTable";
    internal const string DailyAggregationTableName = "CommerceUsageEventsDailyAggregationTable";
    internal const string BillingTable2Name = "CommerceBillableEventsTable2";
    internal const string BillingQueueName = "commerce-usage-trigger";
    internal const string PushAgentBillingTableName = "CommerceUsageRecordsTable";
    internal const string ErrorReportingQueueName = "commerce-usage-error-trigger";
    internal const string ErrorReportingTableName = "CommerceUsageProcessingErrors";
  }
}
