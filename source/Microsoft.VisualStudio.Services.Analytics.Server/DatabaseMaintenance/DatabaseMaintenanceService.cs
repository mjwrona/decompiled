// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.DatabaseMaintenanceService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Telemetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  public class DatabaseMaintenanceService : IDatabaseMaintenanceService, IVssFrameworkService
  {
    public const string Area = "Analytics";
    public const string Layer = "DatabaseMaintenance";
    internal const int MaxRetryAttempt = 10;
    internal const int TraceRowBatchSize = 20;
    private int m_openRowGroupThreshold;
    private long m_openRowGroupSizeInBytesThreshold;
    private int m_fragmentationThreshold;
    private int m_timeoutInSeconds;
    private int m_statsTimeoutInSeconds;
    private int m_trimReasonDictionarySizeThreshold;
    private int m_overlapsThreshold;
    private string m_restrictTableForSORT;
    private int m_minPartitionRowCount;
    private const string c_openRowGroupThresholdRegistryKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/OpenRowGroupThresholdInPercentage";
    private const string c_openRowGroupSizeInBytesThresholdRegistryKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/OpenRowGroupSizeInBytesThreshold";
    private const string c_fragmentationThresholdRegistryKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/FragmentationThresholdInPercentage";
    private const string c_timeoutRegistryKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/TimeoutInSeconds";
    private const string c_statsTimeoutRegistryKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/StatsTimeoutInSeconds";
    private const string c_overlapsThresholdRegistryKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/OverlapsThresholdInPercentage";
    private const string c_trimReasonDictionarySizeThresholdRegistryKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/TrimReasonDictionarySizeThresholdInPercentage";
    private const string c_restrictSortTableRegistryKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/RestrictTableForSORT";
    private const string c_minPartitionRowCountKey = "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/MinPartitionRowCount";
    private const int c_defaultOpenRowGroupThresholdInPercentage = 30;
    private const long c_defaultOpenRowGroupSizeInBytesThreshold = 104857600;
    private const int c_defaultFragmentationInPercentage = 10;
    private const int c_defaultTimeoutInSeconds = 10800;
    private const int c_defaultTrimReasonDictionarySizeThreshold = 70;
    private const int c_defaultOverlapsInPercentage = 30;
    private const int c_defaultStatsTimeoutInSeconds = 150;
    private const int c_defaultOperationTimeThresholdInHours = 0;
    private const string c_defaultBlankTableName = "  ";
    private const int c_defaultMinPartitionRowCount = 102400;

    internal bool InSort { get; set; }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      this.m_openRowGroupThreshold = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/OpenRowGroupThresholdInPercentage", 30);
      this.m_openRowGroupSizeInBytesThreshold = service.GetValue<long>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/OpenRowGroupSizeInBytesThreshold", 104857600L);
      this.m_fragmentationThreshold = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/FragmentationThresholdInPercentage", 10);
      this.m_timeoutInSeconds = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/TimeoutInSeconds", 10800);
      this.m_statsTimeoutInSeconds = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/StatsTimeoutInSeconds", 150);
      this.m_trimReasonDictionarySizeThreshold = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/TrimReasonDictionarySizeThresholdInPercentage", 70);
      this.m_overlapsThreshold = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/OverlapsThresholdInPercentage", 30);
      this.m_restrictTableForSORT = service.GetValue<string>(systemRequestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/RestrictTableForSORT", "  ");
    }

    public void ProcessPartitionDB(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database)
    {
      List<Exception> exceptionList = new List<Exception>();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_minPartitionRowCount = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Analytics/Jobs/AnalyticsDBMaintenanceJob/MinPartitionRowCount", 102400);
      using (DatabaseMaintenanceComponent component = this.GetComponent(database))
      {
        IList<Partition> columnStoreIndexes = component.GetPartitionsWithColumnStoreIndexes();
        if (columnStoreIndexes != null && columnStoreIndexes.Any<Partition>())
        {
          IList<PartitionToSplit> partitionsToSplitMetric = component.GetPartitionsToSplitMetric();
          DatabaseMaintenanceService.WriteResultsToTrace<PartitionToSplit>(requestContext, partitionsToSplitMetric, 20, database.DatabaseId);
          using (List<Partition>.Enumerator enumerator = columnStoreIndexes.OrderByDescending<Partition, bool>((Func<Partition, bool>) (p => p.IsActive)).ThenBy<Partition, int>((Func<Partition, int>) (p => p.PartitionId)).ThenBy<Partition, string>((Func<Partition, string>) (p => p.ColumnStoreIndexName)).ToList<Partition>().GetEnumerator())
          {
label_24:
            while (enumerator.MoveNext())
            {
              Partition current = enumerator.Current;
              bool flag1 = false;
              int num = 1;
              while (true)
              {
                if (num <= 10 && num == 1 | flag1)
                {
                  flag1 = false;
                  this.InSort = false;
                  if (!requestContext.IsCanceled)
                  {
                    int? partitionNumber = new int?(current.PartitionId);
                    try
                    {
                      this.Trace(requestContext, string.Format("Processing partitionid {0} for {1} index", (object) current.PartitionId, (object) current.ColumnStoreIndexName), database.DatabaseName, attempt: new int?(num));
                      this.MaintainColumnStoreIndex(requestContext, component, current.ColumnStoreIndexName, partitionNumber, current.TableName, database.DatabaseId);
                    }
                    catch (Exception ex1) when (ex1.GetType().Name != "AssertFailedException")
                    {
                      bool flag2 = num == 10;
                      flag1 = this.ShouldRetry(ex1);
                      if (!flag1)
                        flag2 = true;
                      this.Trace(requestContext, string.Format("Exception {0} occurred in partitionid: {1} for {2} index", (object) ex1, (object) current.PartitionId, (object) current.ColumnStoreIndexName), database.DatabaseName, flag2 ? TraceLevel.Error : TraceLevel.Warning, new int?(num));
                      if (flag2)
                        exceptionList.Add(ex1);
                      if (flag2)
                      {
                        if (this.InSort)
                        {
                          if (ex1.GetType() == typeof (NumbersOfRecordsDontMatchForTableException))
                          {
                            try
                            {
                              if ((!requestContext.IsFeatureEnabled("Analytics.Jobs.ClearFailedDatabaseMaintenanceOperation") ? 0 : (!service.GetValue<bool>(requestContext, (RegistryQuery) ("/Service/Analytics/Settings/DatabaseMaintenance/OperationCleanupDisabledDatabase/" + database.SqlConnectionInfo.InitialCatalog), false) ? 1 : 0)) != 0)
                              {
                                this.Trace(requestContext, string.Format("Clearing stuck operation for partition_number {0} for {1} index", (object) current.PartitionId, (object) current.ColumnStoreIndexName), database.DatabaseName, TraceLevel.Warning, new int?(num));
                                int fromPartitionNumber = component.GetPartitionIdFromPartitionNumber(partitionNumber, DatabaseMaintenanceService.PartitionTableDataMap.GetPartitionSchemeName(current.TableName));
                                component.ClearFailedDatabaseMaintenanceOperations(fromPartitionNumber, 0, current.TableName.Replace("AnalyticsModel.", ""));
                              }
                            }
                            catch (Exception ex2)
                            {
                              exceptionList.Add(ex2);
                              this.Trace(requestContext, string.Format("Exception while Clearing stuck operation for partition_number {0} for {1} index {2} ", (object) current.PartitionId, (object) current.ColumnStoreIndexName, (object) ex2.ToString()), database.DatabaseName, TraceLevel.Error, new int?(num));
                            }
                          }
                          throw new AggregateException(AnalyticsResources.AGGREGATE_EXCEPTIONS((object) exceptionList.Count, (object) "AnalyticsDatabaseMaintenanceJob", (object) database.DatabaseName, (object) 12015001), (IEnumerable<Exception>) exceptionList);
                        }
                      }
                    }
                    ++num;
                  }
                  else
                    break;
                }
                else
                  goto label_24;
              }
              requestContext.TraceAlways(12015001, TraceLevel.Warning, "Analytics", "DatabaseMaintenance", (string[]) null, "Exiting loop: User requested to stop the job.");
              return;
            }
          }
        }
        else
          this.Trace(requestContext, "Did not find column store index. Skip.", database.DatabaseName);
      }
      if (exceptionList.Any<Exception>())
        throw new AggregateException(AnalyticsResources.AGGREGATE_EXCEPTIONS((object) exceptionList.Count, (object) "AnalyticsDatabaseMaintenanceJob", (object) database.DatabaseName, (object) 12015001), (IEnumerable<Exception>) exceptionList);
    }

    internal virtual DatabaseMaintenanceComponent GetComponent(
      ITeamFoundationDatabaseProperties database)
    {
      return TeamFoundationResourceManagementService.CreateComponentRaw<DatabaseMaintenanceComponent>(database.SqlConnectionInfo);
    }

    internal virtual bool ShouldRetry(Exception ex)
    {
      switch (ex)
      {
        case DatabaseOperationTimeoutException _:
        case DatabaseOperationCanceledException _:
        case DBExecutingDeadlockException _:
          return true;
        default:
          return this.InSort;
      }
    }

    internal virtual void MaintainColumnStoreIndex(
      IVssRequestContext requestContext,
      DatabaseMaintenanceComponent component,
      string columnStoreIndexName,
      int? partitionNumber,
      string tableName,
      int databaseId)
    {
      DateTime today = DateTime.Today;
      IList<ColumnStoreOverlapStatistics> storeOverlapsMetric1 = component.GetClusteredColumnStoreOverlapsMetric(partitionNumber, tableName, DatabaseMaintenanceService.PartitionTableDataMap.GetPartitionColumnName(tableName), this.m_statsTimeoutInSeconds, this.m_minPartitionRowCount);
      DatabaseMaintenanceService.WriteResultsToTrace<ColumnStoreOverlapStatistics>(requestContext, storeOverlapsMetric1, 20, databaseId);
      IList<ColumnStoreFragmentationStatisticRow> fragmentationStatisticRowList = component.GetColumnStoreIndexStats(partitionNumber, columnStoreIndexName, "BeforeReorganize", this.m_statsTimeoutInSeconds);
      DatabaseMaintenanceService.WriteResultsToTrace<ColumnStoreFragmentationStatisticRow>(requestContext, fragmentationStatisticRowList, 20, databaseId);
      double overlapPercent;
      if (this.HasOverlapsAboveThreshold(storeOverlapsMetric1, out overlapPercent))
      {
        string[] source = Array.Empty<string>();
        if (!string.IsNullOrWhiteSpace(this.m_restrictTableForSORT))
          source = this.m_restrictTableForSORT.Split(',');
        if (!((IEnumerable<string>) source).Contains<string>(tableName) || today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == (DayOfWeek.Wednesday | DayOfWeek.Thursday))
        {
          this.Trace(requestContext, string.Format("Attempting to sort for partition {0} for index {1} on table {2} due to overlaps({3}%) exceeding threshold", (object) partitionNumber, (object) columnStoreIndexName, (object) tableName, (object) overlapPercent), component.Database);
          IList<ColumnStoreFragmentationStatisticRow> result = this.InvokeSortOnTable(component, partitionNumber, tableName, columnStoreIndexName, "AfterSortForOverlaps");
          DatabaseMaintenanceService.WriteResultsToTrace<ColumnStoreFragmentationStatisticRow>(requestContext, result, 20, databaseId);
          IList<ColumnStoreOverlapStatistics> storeOverlapsMetric2 = component.GetClusteredColumnStoreOverlapsMetric(partitionNumber, tableName, DatabaseMaintenanceService.PartitionTableDataMap.GetPartitionColumnName(tableName), this.m_statsTimeoutInSeconds, this.m_minPartitionRowCount);
          DatabaseMaintenanceService.WriteResultsToTrace<ColumnStoreOverlapStatistics>(requestContext, storeOverlapsMetric2, 20, databaseId, true);
          return;
        }
        this.Trace(requestContext, "Skipping " + tableName + " table which has been blocked to run in weekday via registry", component.Database);
      }
      if (fragmentationStatisticRowList.Any<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.Fragmentation >= this.m_fragmentationThreshold)))
      {
        double percent;
        if (this.HasTrimReasonDictionarySizeAboveThreshold(fragmentationStatisticRowList, out percent))
        {
          if (!this.WillCreateOpenRowGroups(fragmentationStatisticRowList) && (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == (DayOfWeek.Wednesday | DayOfWeek.Thursday)))
          {
            this.Trace(requestContext, string.Format("Attempting to sort for partition {0} for index {1} on table {2} due to fragmenation exceeding threshold and dictionary size({3}%)", (object) partitionNumber, (object) columnStoreIndexName, (object) tableName, (object) percent), component.Database);
            IList<ColumnStoreFragmentationStatisticRow> result = this.InvokeSortOnTable(component, partitionNumber, tableName, columnStoreIndexName, "AfterSortForFragmentation");
            DatabaseMaintenanceService.WriteResultsToTrace<ColumnStoreFragmentationStatisticRow>(requestContext, result, 20, databaseId);
            return;
          }
        }
        else
        {
          this.Trace(requestContext, string.Format("Attempting to reorganize for partition {0} for index {1} on table {2}", (object) partitionNumber, (object) columnStoreIndexName, (object) tableName), component.Database);
          fragmentationStatisticRowList = this.InvokeReorganizeOnTable(component, partitionNumber, tableName, columnStoreIndexName, "AfterReorganize");
          DatabaseMaintenanceService.WriteResultsToTrace<ColumnStoreFragmentationStatisticRow>(requestContext, fragmentationStatisticRowList, 20, databaseId);
        }
      }
      List<ColumnStoreFragmentationStatisticRow> list = fragmentationStatisticRowList.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.State.Equals("OPEN"))).ToList<ColumnStoreFragmentationStatisticRow>();
      int num1 = this.HasOpenRowGroupCountsAboveThreshold((IEnumerable<ColumnStoreFragmentationStatisticRow>) list, fragmentationStatisticRowList.Count) ? 1 : 0;
      long num2 = list.Any<ColumnStoreFragmentationStatisticRow>() ? list.Select<ColumnStoreFragmentationStatisticRow, long>((Func<ColumnStoreFragmentationStatisticRow, long>) (r => r.SizeInBytes)).Max() : 0L;
      if (num1 != 0 || num2 > this.m_openRowGroupSizeInBytesThreshold)
      {
        this.Trace(requestContext, string.Format("Attempting to compress delta store for partition {0} for index {1} on table {2}", (object) partitionNumber, (object) columnStoreIndexName, (object) tableName), component.Database);
        fragmentationStatisticRowList = this.InvokeReorganizeOnTable(component, partitionNumber, tableName, columnStoreIndexName, "AfterCompressReorganize", true);
        DatabaseMaintenanceService.WriteResultsToTrace<ColumnStoreFragmentationStatisticRow>(requestContext, fragmentationStatisticRowList, 20, databaseId);
      }
      if (num1 == 0 && !fragmentationStatisticRowList.Any<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.Fragmentation >= this.m_fragmentationThreshold)))
        return;
      this.Trace(requestContext, string.Format("Attempting to reorganize for partition {0} for index {1} on table {2}", (object) partitionNumber, (object) columnStoreIndexName, (object) tableName), component.Database);
      IList<ColumnStoreFragmentationStatisticRow> result1 = this.InvokeReorganizeOnTable(component, partitionNumber, tableName, columnStoreIndexName, "AfterCompressReorganize");
      DatabaseMaintenanceService.WriteResultsToTrace<ColumnStoreFragmentationStatisticRow>(requestContext, result1, 20, databaseId);
    }

    private bool WillCreateOpenRowGroups(
      IList<ColumnStoreFragmentationStatisticRow> resultList)
    {
      return resultList.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.State.Equals("COMPRESSED"))).Sum<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, long>) (x => x.TotalRows)) - resultList.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.State.Equals("COMPRESSED"))).Sum<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, long>) (x => x.DeletedRows)) < 102400L;
    }

    private bool HasOverlapsAboveThreshold(
      IList<ColumnStoreOverlapStatistics> clusteredColumnStoreOverlapsMetric,
      out double overlapPercent)
    {
      ColumnStoreOverlapStatistics overlapStatistics = clusteredColumnStoreOverlapsMetric.FirstOrDefault<ColumnStoreOverlapStatistics>();
      overlapPercent = overlapStatistics == null ? 0.0 : (double) overlapStatistics.Overlaps / (double) overlapStatistics.SegmentsInPartition * 100.0;
      return overlapStatistics != null && overlapStatistics.SegmentsInPartition > 3L && overlapPercent > (double) this.m_overlapsThreshold;
    }

    private bool HasTrimReasonDictionarySizeAboveThreshold(
      IList<ColumnStoreFragmentationStatisticRow> resultList,
      out double percent)
    {
      List<ColumnStoreFragmentationStatisticRow> list = resultList.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.State.Equals("COMPRESSED"))).ToList<ColumnStoreFragmentationStatisticRow>();
      percent = (double) list.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.TrimReason.Equals("DICTIONARY_SIZE"))).Count<ColumnStoreFragmentationStatisticRow>() / (double) list.Count<ColumnStoreFragmentationStatisticRow>() * 100.0;
      return percent > (double) this.m_trimReasonDictionarySizeThreshold;
    }

    private bool HasOpenRowGroupCountsAboveThreshold(
      IEnumerable<ColumnStoreFragmentationStatisticRow> openRowGroupsList,
      int totalCount)
    {
      int num1;
      double num2 = (double) (num1 = openRowGroupsList.Count<ColumnStoreFragmentationStatisticRow>()) / (double) totalCount * 100.0;
      long num3 = openRowGroupsList.Any<ColumnStoreFragmentationStatisticRow>() ? openRowGroupsList.Select<ColumnStoreFragmentationStatisticRow, long>((Func<ColumnStoreFragmentationStatisticRow, long>) (r => r.TotalRows)).Max() : 0L;
      return num1 > 1 && num2 >= (double) this.m_openRowGroupThreshold || num3 > 1048576L;
    }

    internal static void WriteResultsToTrace<T>(
      IVssRequestContext requestContext,
      IList<T> result,
      int batchSize,
      int databaseId,
      bool sortedForOverlaps = false)
    {
      if (result == null || !result.Any<T>())
        return;
      DatabaseMaintenanceService.WriteAggregatedDataOnPrem<T>(requestContext, result, databaseId, sortedForOverlaps);
      IEnumerable<T> source = result.AsEnumerable<T>();
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      };
      while (source.Any<T>())
      {
        List<T> list = source.Take<T>(batchSize).ToList<T>();
        source = source.Skip<T>(list.Count).AsEnumerable<T>();
        if (typeof (T) == typeof (ColumnStoreFragmentationStatisticRow))
          requestContext.TraceAlways(12015001, TraceLevel.Info, "Analytics", "DatabaseMaintenance", (string[]) null, JsonConvert.SerializeObject((object) list, settings));
        else if (typeof (T) == typeof (ColumnStoreOverlapStatistics))
          requestContext.TraceAlways(12015002, TraceLevel.Info, "Analytics", "DatabaseMaintenance", (string[]) null, JsonConvert.SerializeObject((object) list, settings));
        else if (typeof (T) == typeof (PartitionToSplit))
          requestContext.TraceAlways(12015003, TraceLevel.Info, "Analytics", "DatabaseMaintenance", (string[]) null, JsonConvert.SerializeObject((object) list, settings));
      }
    }

    private static void WriteAggregatedDataOnPrem<T>(
      IVssRequestContext requestContext,
      IList<T> result,
      int databaseId,
      bool sortedForOverlaps = false)
    {
      IAnalyticsOnPremTelemetryService service = requestContext.GetService<IAnalyticsOnPremTelemetryService>();
      if (typeof (T) == typeof (ColumnStoreFragmentationStatisticRow))
      {
        IList<ColumnStoreFragmentationStatisticRow> source = (IList<ColumnStoreFragmentationStatisticRow>) result;
        ColumnStoreFragmentationStatistics columnStoreIndexStatRowAggregated = new ColumnStoreFragmentationStatistics()
        {
          Action = source.FirstOrDefault<ColumnStoreFragmentationStatisticRow>().Action,
          IndexName = source.FirstOrDefault<ColumnStoreFragmentationStatisticRow>().Name,
          DictonarySizeRowGroupCount = (long) source.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.State.Equals("COMPRESSED") && r.TrimReason.Equals("DICTIONARY_SIZE"))).Count<ColumnStoreFragmentationStatisticRow>(),
          TotalCount = (long) source.Count,
          FragmentedRowGroupCount = (long) source.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.Fragmentation >= 10)).Count<ColumnStoreFragmentationStatisticRow>(),
          MaxFragmentation = source.Max<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, int>) (r => r.Fragmentation)),
          OpenRowGroupsCount = source.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.State.Equals("OPEN"))).Count<ColumnStoreFragmentationStatisticRow>(),
          OpenRowGroupMaxSizeInBytes = source.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.State.Equals("OPEN"))).Select<ColumnStoreFragmentationStatisticRow, long>((Func<ColumnStoreFragmentationStatisticRow, long>) (r => r.SizeInBytes)).DefaultIfEmpty<long>(0L).Max(),
          OpenRowGroupMaxRowCount = source.Where<ColumnStoreFragmentationStatisticRow>((Func<ColumnStoreFragmentationStatisticRow, bool>) (r => r.State.Equals("OPEN"))).Select<ColumnStoreFragmentationStatisticRow, long>((Func<ColumnStoreFragmentationStatisticRow, long>) (r => r.TotalRows)).DefaultIfEmpty<long>(0L).Max()
        };
        service.SetDatabaseSegmentFragmentationResult(requestContext, databaseId, columnStoreIndexStatRowAggregated);
      }
      else
      {
        if (!(typeof (T) == typeof (ColumnStoreOverlapStatistics)) || sortedForOverlaps)
          return;
        IList<ColumnStoreOverlapStatistics> source = (IList<ColumnStoreOverlapStatistics>) result;
        service.SetDatabaseSegmentOverlapsResult(requestContext, databaseId, source.First<ColumnStoreOverlapStatistics>().TableName, source.First<ColumnStoreOverlapStatistics>().Overlaps, source.First<ColumnStoreOverlapStatistics>().SegmentsInPartition);
      }
    }

    private void Trace(
      IVssRequestContext requestContext,
      string message,
      string dbName = null,
      TraceLevel traceLevel = TraceLevel.Info,
      int? attempt = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(dbName))
        stringBuilder.Append("DB:" + dbName + " | ");
      if (attempt.HasValue)
        stringBuilder.Append(string.Format("Attempt {0} of {1} | ", (object) attempt, (object) 10));
      stringBuilder.Append(message);
      requestContext.TraceLongTextAlways(12015001, traceLevel, "Analytics", "DatabaseMaintenance", stringBuilder.ToString());
    }

    private IList<ColumnStoreFragmentationStatisticRow> InvokeSortOnTable(
      DatabaseMaintenanceComponent component,
      int? partitionNumber,
      string tableName,
      string columnStoreIndexName,
      string action)
    {
      string partitionSchemeName = DatabaseMaintenanceService.PartitionTableDataMap.GetPartitionSchemeName(tableName);
      int fromPartitionNumber = component.GetPartitionIdFromPartitionNumber(partitionNumber, DatabaseMaintenanceService.PartitionTableDataMap.GetPartitionSchemeName(tableName));
      this.InSort = true;
      if (fromPartitionNumber != 0)
        component.ExecuteColumnStoreMaintenanceOperation("SORT", fromPartitionNumber, partitionSchemeName, tableName.Replace("AnalyticsModel.", ""));
      this.InSort = false;
      return component.GetColumnStoreIndexStats(partitionNumber, columnStoreIndexName, action, this.m_statsTimeoutInSeconds);
    }

    private IList<ColumnStoreFragmentationStatisticRow> InvokeReorganizeOnTable(
      DatabaseMaintenanceComponent component,
      int? partitionNumber,
      string tableName,
      string columnStoreIndexName,
      string action,
      bool compress = false)
    {
      component.ReorganizePartition(tableName, columnStoreIndexName, partitionNumber, this.m_timeoutInSeconds, compress);
      return component.GetColumnStoreIndexStats(partitionNumber, columnStoreIndexName, action, this.m_statsTimeoutInSeconds);
    }

    public ColumnStoreMaintenanceResult ExecuteColumnStoreMaintenaceOperation(
      IVssRequestContext requestContext,
      string operation,
      int partitionId,
      string schemeName,
      ITeamFoundationDatabaseProperties database = null,
      string tableName = null)
    {
      using (DatabaseMaintenanceComponent maintenanceComponent = database != null ? TeamFoundationResourceManagementService.CreateComponentRaw<DatabaseMaintenanceComponent>(database) : requestContext.CreateComponent<DatabaseMaintenanceComponent>())
        return maintenanceComponent.ExecuteColumnStoreMaintenanceOperation(operation, partitionId, schemeName, tableName);
    }

    private static class PartitionTableDataMap
    {
      private static Dictionary<string, string> TableColumnMap = new Dictionary<string, string>();
      private static Dictionary<string, string> TableSchemeMap = new Dictionary<string, string>();

      static PartitionTableDataMap()
      {
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap = DatabaseMaintenanceService.PartitionTableDataMap.GetCustomTableColumnsMap();
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_WorkItemHistory", "WorkItemRevisionSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_WorkItemLinkHistory", "WorkItemRevisionSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_WorkItem", "WorkItemRevisionSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_TestRun", "TestRunSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_Test", "TestSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_TestResult", "TestResultSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_TestResultDaily", "TestResultDailySK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_Build", "BuildSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_BuildTaskResult", "BuildTaskResultSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_TaskAgentRequest", "TaskAgentRequestSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_TestPoint", "TestPointSK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.Add("AnalyticsModel.tbl_TestPointHistory", "TestPointHistorySK");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap = DatabaseMaintenanceService.PartitionTableDataMap.GetCustomTableSchemeMap();
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_WorkItemHistory", "scheme_AnalyticsWorkItem");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_WorkItemLinkHistory", "scheme_AnalyticsWorkItem");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_WorkItem", "scheme_AnalyticsWorkItem");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_TestRun", "scheme_AnalyticsTest");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_Test", "scheme_AnalyticsTest");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_TestResult", "scheme_AnalyticsTest");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_TestResultDaily", "scheme_AnalyticsTest");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_Build", "scheme_AnalyticsBuild");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_BuildTaskResult", "scheme_AnalyticsBuild");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_TaskAgentRequest", "scheme_AnalyticsBuild");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_TestPoint", "scheme_AnalyticsTest");
        DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.Add("AnalyticsModel.tbl_TestPointHistory", "scheme_AnalyticsTest");
      }

      private static Dictionary<string, string> GetCustomTableColumnsMap() => Enumerable.Range(1, 20).Select(i => new
      {
        Key = "AnalyticsModel.tbl_WorkItemRevisionCustom" + i.ToString("00"),
        Value = "WorkItemRevisionSK"
      }).ToDictionary(x => x.Key, x => x.Value);

      internal static string GetPartitionColumnName(string tableName)
      {
        if (tableName == null)
          return (string) null;
        string str;
        return DatabaseMaintenanceService.PartitionTableDataMap.TableColumnMap.TryGetValue(tableName, out str) ? str : (string) null;
      }

      private static Dictionary<string, string> GetCustomTableSchemeMap() => Enumerable.Range(1, 20).Select(i => new
      {
        Key = "AnalyticsModel.tbl_WorkItemRevisionCustom" + i.ToString("00"),
        Value = "scheme_AnalyticsWorkItem"
      }).ToDictionary(x => x.Key, x => x.Value);

      internal static string GetPartitionSchemeName(string tableName)
      {
        if (tableName == null)
          return (string) null;
        string str;
        return DatabaseMaintenanceService.PartitionTableDataMap.TableSchemeMap.TryGetValue(tableName, out str) ? str : (string) null;
      }
    }
  }
}
