// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Telemetry.IAnalyticsOnPremTelemetryService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance;

namespace Microsoft.VisualStudio.Services.Analytics.Telemetry
{
  [DefaultServiceImplementation(typeof (AnalyticsOnPremTelemetryService))]
  public interface IAnalyticsOnPremTelemetryService : IVssFrameworkService
  {
    void IncrementCounter(
      IVssRequestContext requestContext,
      string counterArea,
      string counterName,
      int value);

    void SetDataQualityResult(IVssRequestContext requestContext, DataQualityResult result);

    void SetDatabaseSegmentFragmentationResult(
      IVssRequestContext requestContext,
      int databaseId,
      ColumnStoreFragmentationStatistics columnStoreIndexStatRowAggregated);

    void SetDatabaseSegmentOverlapsResult(
      IVssRequestContext requestContext,
      int databaseId,
      string tableName,
      int overlaps,
      long segmentsInPartition);
  }
}
