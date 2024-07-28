// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlStatistics
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SqlStatistics : IDisposable
  {
    private Stopwatch m_watch;
    private ProcessThread m_thread;
    private TimeSpan m_cpuTimeBefore;
    private Exception m_exception;
    private static readonly string s_area = "Profiling";
    private static readonly string s_layer = nameof (SqlStatistics);

    public SqlStatistics()
    {
      try
      {
        if (SqlStatisticsContext.CollectingStatistics)
          throw new ApplicationException("Already collecting SQL statistics.");
        SqlStatisticsContext.ResetStatistics();
        SqlStatisticsContext.CollectingStatistics = true;
        Process currentProcess = Process.GetCurrentProcess();
        this.m_thread = (ProcessThread) null;
        foreach (ProcessThread thread in (ReadOnlyCollectionBase) currentProcess.Threads)
        {
          if (thread.Id == AppDomain.GetCurrentThreadId())
          {
            this.m_thread = thread;
            break;
          }
        }
        if (this.m_thread != null)
          this.m_cpuTimeBefore = this.m_thread.TotalProcessorTime;
        this.m_watch = Stopwatch.StartNew();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, SqlStatistics.s_area, SqlStatistics.s_layer, ex);
        SqlStatisticsContext.CollectingStatistics = false;
        this.m_exception = ex;
      }
    }

    public override string ToString()
    {
      try
      {
        if (this.m_exception != null)
          return this.m_exception.ToReadableStackTrace();
        if (SqlStatisticsContext.Exception != null)
          return SqlStatisticsContext.Exception.ToReadableStackTrace();
        TimeSpan elapsed = this.m_watch.Elapsed;
        TimeSpan timeSpan1 = TimeSpan.Zero;
        double num1 = 0.0;
        if (this.m_thread != null)
        {
          timeSpan1 = this.m_thread.TotalProcessorTime - this.m_cpuTimeBefore;
          num1 = 100.0 * (double) timeSpan1.Ticks / (double) elapsed.Ticks;
        }
        TimeSpan timeSpan2 = elapsed - SqlStatisticsContext.SqlConnectTime - SqlStatisticsContext.SqlExecuteTime;
        string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "elapsed time: {0}, sql calls: {1}, sql connect time: {2}, sql execute time: {3}, non-sql time: {4} ({5,2:0}%), cpu time: {6} ({7,4:0.0}%), avg connect time: {8,4:0.00} ms, avg execute time: {9,4:0.0} ms.", (object) elapsed, (object) SqlStatisticsContext.SqlCalls, (object) SqlStatisticsContext.SqlConnectTime, (object) SqlStatisticsContext.SqlExecuteTime, (object) timeSpan2, (object) (100.0 * timeSpan2.TotalMilliseconds / elapsed.TotalMilliseconds), this.m_thread != null ? (object) timeSpan1 : (object) "N/A", (object) num1, (object) (SqlStatisticsContext.SqlConnectTime.TotalMilliseconds / (double) SqlStatisticsContext.SqlCalls), (object) (SqlStatisticsContext.SqlExecuteTime.TotalMilliseconds / (double) SqlStatisticsContext.SqlCalls));
        int num2 = Math.Min(15, SqlStatisticsContext.WorstMethods.Count);
        string str2 = num2 != 0 ? str1 + string.Format(" {0,2} slowest methods:", (object) num2) : str1 + " All methods quick.";
        SqlStatisticsContext.WorstMethods.Sort((IComparer<MethodTime>) new MethodTimeComparer());
        for (int index = 0; index < num2; ++index)
          str2 += string.Format((IFormatProvider) CultureInfo.InvariantCulture, " {0}({1})", (object) SqlStatisticsContext.WorstMethods[index].Name, (object) SqlStatisticsContext.WorstMethods[index].Time);
        int num3 = Math.Min(15, SqlStatisticsContext.WorstSqlCalls.Count);
        string str3 = num3 != 0 ? str2 + string.Format(" {0,2} slowest sql calls:", (object) num2) : str2 + " All sql calls quick.";
        SqlStatisticsContext.WorstSqlCalls.Sort((IComparer<MethodTime>) new MethodTimeComparer());
        for (int index = 0; index < num3; ++index)
          str3 += string.Format((IFormatProvider) CultureInfo.InvariantCulture, " {0}({1} ms)", (object) SqlStatisticsContext.WorstSqlCalls[index].Name, (object) SqlStatisticsContext.WorstSqlCalls[index].Time.TotalMilliseconds);
        if (SqlStatisticsContext.CollectionErrors.Count > 0)
        {
          foreach (string collectionError in SqlStatisticsContext.CollectionErrors)
            str3 += collectionError;
        }
        return str3;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, SqlStatistics.s_area, SqlStatistics.s_layer, ex);
        SqlStatisticsContext.CollectingStatistics = false;
        return ex.ToReadableStackTrace();
      }
    }

    public void Dispose()
    {
      try
      {
        SqlStatisticsContext.CollectingStatistics = false;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, SqlStatistics.s_area, SqlStatistics.s_layer, ex);
      }
    }
  }
}
