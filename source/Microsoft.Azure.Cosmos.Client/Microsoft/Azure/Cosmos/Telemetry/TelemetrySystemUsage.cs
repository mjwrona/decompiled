// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.TelemetrySystemUsage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal static class TelemetrySystemUsage
  {
    public static SystemInfo GetCpuInfo(
      IReadOnlyCollection<SystemUsageLoad> systemUsageCollection)
    {
      LongConcurrentHistogram histogram = new LongConcurrentHistogram(1L, 99999L, 2);
      SystemInfo cpuInfo = new SystemInfo("CPU", "Percentage");
      foreach (SystemUsageLoad systemUsage in (IEnumerable<SystemUsageLoad>) systemUsageCollection)
      {
        if (!float.IsNaN(systemUsage.CpuUsage.Value))
        {
          float? cpuUsage = systemUsage.CpuUsage;
          long? nullable1 = cpuUsage.HasValue ? new long?((long) cpuUsage.GetValueOrDefault()) : new long?();
          long num = 100;
          long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() * num) : new long?();
          if (nullable2.HasValue)
            histogram.RecordValue(nullable2.Value);
        }
      }
      if (histogram.TotalCount > 0L)
        cpuInfo.SetAggregators(histogram, 100.0);
      return cpuInfo;
    }

    public static SystemInfo GetMemoryRemainingInfo(
      IReadOnlyCollection<SystemUsageLoad> systemUsageCollection)
    {
      LongConcurrentHistogram histogram = new LongConcurrentHistogram(1L, long.MaxValue, 2);
      SystemInfo memoryRemainingInfo = new SystemInfo("MemoryRemaining", "MB");
      foreach (SystemUsageLoad systemUsage in (IEnumerable<SystemUsageLoad>) systemUsageCollection)
      {
        long? memoryAvailable = systemUsage.MemoryAvailable;
        if (memoryAvailable.HasValue)
          histogram.RecordValue(memoryAvailable.Value);
      }
      if (histogram.TotalCount > 0L)
        memoryRemainingInfo.SetAggregators(histogram, 1024.0);
      return memoryRemainingInfo;
    }

    public static SystemInfo GetAvailableThreadsInfo(
      IReadOnlyCollection<SystemUsageLoad> systemUsageCollection)
    {
      LongConcurrentHistogram histogram = new LongConcurrentHistogram(1L, long.MaxValue, 2);
      SystemInfo availableThreadsInfo = new SystemInfo("SystemPool_AvailableThreads", "ThreadCount");
      foreach (SystemUsageLoad systemUsage in (IEnumerable<SystemUsageLoad>) systemUsageCollection)
      {
        int? availableThreads = (int?) systemUsage.ThreadInfo?.AvailableThreads;
        long? nullable = availableThreads.HasValue ? new long?((long) availableThreads.GetValueOrDefault()) : new long?();
        if (nullable.HasValue)
          histogram.RecordValue(nullable.Value);
      }
      if (histogram.TotalCount > 0L)
        availableThreadsInfo.SetAggregators(histogram);
      return availableThreadsInfo;
    }

    public static SystemInfo GetThreadStarvationSignalCount(
      IReadOnlyCollection<SystemUsageLoad> systemUsageCollection)
    {
      int count = 0;
      foreach (SystemUsageLoad systemUsage in (IEnumerable<SystemUsageLoad>) systemUsageCollection)
      {
        bool? isThreadStarving = (bool?) systemUsage.ThreadInfo?.IsThreadStarving;
        if (isThreadStarving.HasValue && isThreadStarving.Value)
          ++count;
      }
      return new SystemInfo("SystemPool_IsThreadStarving_True", "Count", count);
    }

    public static SystemInfo GetThreadWaitIntervalInMs(
      IReadOnlyCollection<SystemUsageLoad> systemUsageCollection)
    {
      LongConcurrentHistogram histogram = new LongConcurrentHistogram(1L, 10000000L, 2);
      SystemInfo waitIntervalInMs1 = new SystemInfo("SystemPool_ThreadWaitInterval", "MilliSecond");
      foreach (SystemUsageLoad systemUsage in (IEnumerable<SystemUsageLoad>) systemUsageCollection)
      {
        double? waitIntervalInMs2 = (double?) systemUsage.ThreadInfo?.ThreadWaitIntervalInMs;
        if (waitIntervalInMs2.HasValue)
          histogram.RecordValue(TimeSpan.FromMilliseconds(waitIntervalInMs2.Value).Ticks);
      }
      if (histogram.TotalCount > 0L)
        waitIntervalInMs1.SetAggregators(histogram, 10000.0);
      return waitIntervalInMs1;
    }

    public static SystemInfo GetTcpConnectionCount(
      IReadOnlyCollection<SystemUsageLoad> systemUsageCollection)
    {
      LongConcurrentHistogram histogram = new LongConcurrentHistogram(1L, 70000L, 2);
      SystemInfo tcpConnectionCount = new SystemInfo("RntbdOpenConnections", "Count");
      foreach (SystemUsageLoad systemUsage in (IEnumerable<SystemUsageLoad>) systemUsageCollection)
      {
        int? nullable = systemUsage.NumberOfOpenTcpConnections;
        if (nullable.HasValue && nullable.Value >= 70000)
          nullable = new int?(69999);
        if (nullable.HasValue)
          histogram.RecordValue((long) nullable.Value);
      }
      if (histogram.TotalCount > 0L)
        tcpConnectionCount.SetAggregators(histogram);
      return tcpConnectionCount;
    }
  }
}
