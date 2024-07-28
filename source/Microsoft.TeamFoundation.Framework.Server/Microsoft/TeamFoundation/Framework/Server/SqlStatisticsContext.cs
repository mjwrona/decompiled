// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlStatisticsContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class SqlStatisticsContext
  {
    [ThreadStatic]
    public static bool CollectingStatistics;
    [ThreadStatic]
    public static int SqlCalls;
    [ThreadStatic]
    public static TimeSpan SqlConnectTime;
    [ThreadStatic]
    public static TimeSpan SqlExecuteTime;
    [ThreadStatic]
    public static List<MethodTime> WorstMethods;
    [ThreadStatic]
    public static List<MethodTime> WorstSqlCalls;
    [ThreadStatic]
    private static TfsStackFrame[] s_frames;
    [ThreadStatic]
    private static int s_stackIndex;
    [ThreadStatic]
    public static List<string> CollectionErrors;
    [ThreadStatic]
    internal static Exception Exception;
    private static TimeSpan s_minTime = TimeSpan.FromMilliseconds(100.0);
    private static TimeSpan s_minSqlTime = TimeSpan.FromMilliseconds(25.0);
    private const int c_maxStackSize = 512;
    private const int c_maxCollectionErrors = 4;
    private static readonly string s_area = "Profiling";
    private static readonly string s_layer = "SqlStatistics";

    public static void ResetStatistics()
    {
      try
      {
        SqlStatisticsContext.SqlCalls = 0;
        SqlStatisticsContext.SqlConnectTime = TimeSpan.Zero;
        SqlStatisticsContext.SqlExecuteTime = TimeSpan.Zero;
        SqlStatisticsContext.WorstMethods = new List<MethodTime>();
        SqlStatisticsContext.WorstSqlCalls = new List<MethodTime>();
        SqlStatisticsContext.CollectionErrors = new List<string>();
        SqlStatisticsContext.s_stackIndex = -1;
        SqlStatisticsContext.Exception = (Exception) null;
      }
      catch (Exception ex)
      {
        SqlStatisticsContext.HandleException(ex);
      }
    }

    internal static void InitStack()
    {
      try
      {
        SqlStatisticsContext.s_frames = new TfsStackFrame[512];
        for (int index = 0; index < 512; ++index)
          SqlStatisticsContext.s_frames[index] = new TfsStackFrame()
          {
            Watch = new Stopwatch()
          };
        SqlStatisticsContext.s_stackIndex = -1;
      }
      catch (Exception ex)
      {
        SqlStatisticsContext.HandleException(ex);
      }
    }

    public static void Enter(string methodName)
    {
      try
      {
        if (SqlStatisticsContext.s_frames == null)
          SqlStatisticsContext.InitStack();
        ++SqlStatisticsContext.s_stackIndex;
        if (SqlStatisticsContext.s_stackIndex < 512)
        {
          if (SqlStatisticsContext.s_stackIndex < 0)
            return;
          SqlStatisticsContext.s_frames[SqlStatisticsContext.s_stackIndex].Name = methodName;
          SqlStatisticsContext.s_frames[SqlStatisticsContext.s_stackIndex].Watch.Restart();
        }
        else
        {
          if (SqlStatisticsContext.CollectionErrors.Count >= 4)
            return;
          SqlStatisticsContext.CollectionErrors.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionError: stack overflow. stackIndex: {0}. methodName: {1}", (object) SqlStatisticsContext.s_stackIndex, (object) methodName));
        }
      }
      catch (Exception ex)
      {
        SqlStatisticsContext.HandleException(ex);
      }
    }

    public static void Leave(string methodName)
    {
      try
      {
        if (SqlStatisticsContext.s_frames == null)
          return;
        if (SqlStatisticsContext.s_stackIndex >= 0)
        {
          if (SqlStatisticsContext.s_stackIndex < 512)
          {
            if (SqlStatisticsContext.s_frames[SqlStatisticsContext.s_stackIndex].Name.StartsWith(methodName, StringComparison.OrdinalIgnoreCase))
            {
              SqlStatisticsContext.s_frames[SqlStatisticsContext.s_stackIndex].Watch.Stop();
              TimeSpan elapsed = SqlStatisticsContext.s_frames[SqlStatisticsContext.s_stackIndex].Watch.Elapsed;
              if (elapsed > SqlStatisticsContext.s_minTime)
              {
                MethodTime methodTime = new MethodTime()
                {
                  Name = SqlStatisticsContext.s_frames[SqlStatisticsContext.s_stackIndex].Name,
                  Time = elapsed
                };
                SqlStatisticsContext.WorstMethods.Add(methodTime);
              }
              --SqlStatisticsContext.s_stackIndex;
            }
            else
            {
              if (SqlStatisticsContext.CollectionErrors.Count >= 4)
                return;
              SqlStatisticsContext.CollectionErrors.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " CollectionError: {0} != {1}.", (object) SqlStatisticsContext.s_frames[SqlStatisticsContext.s_stackIndex].Name, (object) methodName));
            }
          }
          else
            --SqlStatisticsContext.s_stackIndex;
        }
        else
        {
          if (SqlStatisticsContext.CollectionErrors.Count >= 4)
            return;
          SqlStatisticsContext.CollectionErrors.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CollectionError: Stack index went negative. stackIndex: {0}. methodName: {1}", (object) SqlStatisticsContext.s_stackIndex, (object) methodName));
        }
      }
      catch (Exception ex)
      {
        SqlStatisticsContext.HandleException(ex);
      }
    }

    public static void AddSqlExecute(string name, TimeSpan time)
    {
      try
      {
        ++SqlStatisticsContext.SqlCalls;
        SqlStatisticsContext.SqlExecuteTime += time;
        if (!(time > SqlStatisticsContext.s_minSqlTime))
          return;
        if (name.Length > 140)
          name = name.Substring(0, 140);
        MethodTime methodTime = new MethodTime()
        {
          Name = name,
          Time = time
        };
        SqlStatisticsContext.WorstSqlCalls.Add(methodTime);
      }
      catch (Exception ex)
      {
        SqlStatisticsContext.HandleException(ex);
      }
    }

    private static void HandleException(Exception exception)
    {
      TeamFoundationTracingService.TraceExceptionRaw(0, SqlStatisticsContext.s_area, SqlStatisticsContext.s_layer, exception);
      SqlStatisticsContext.Exception = exception;
      SqlStatisticsContext.CollectingStatistics = false;
    }
  }
}
