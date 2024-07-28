// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationTrace
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation
{
  public sealed class TeamFoundationTrace
  {
    private static TeamFoundationTraceSettings s_traceSettings;
    private static object s_lock = new object();
    private static LocalDataStoreSlot s_dataStoreSlot;

    public static bool IsTracingEnabled => TeamFoundationTrace.TraceSettings.IsTracingEnabled;

    public static bool IsTracing(string keyword) => TeamFoundationTrace.IsTracing(new string[1]
    {
      keyword
    });

    public static bool IsTracing(string[] keywords) => TeamFoundationTrace.IsTracing(keywords, TraceLevel.Error);

    public static bool IsTracing(string keyword, TraceLevel traceLevel) => TeamFoundationTrace.IsTracing(new string[1]
    {
      keyword
    }, traceLevel);

    public static bool IsTracing(string[] keywords, TraceLevel traceLevel)
    {
      if (!TeamFoundationTrace.TraceSettings.IsTracingEnabled)
        return false;
      if (TeamFoundationTrace.TraceSettings.IsOverrideEnabled(traceLevel))
        return true;
      foreach (string keyword in keywords)
      {
        if (TeamFoundationTrace.TraceSettings[keyword] >= traceLevel)
          return true;
      }
      return false;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsLogging => TeamFoundationTrace.TraceSettings.IsLogging;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string LogFileName => TeamFoundationTrace.TraceSettings.LogFileName;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void EnableLogging(string path) => TeamFoundationTrace.TraceSettings.EnableLogging(path);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void DisableLogging() => TeamFoundationTrace.TraceSettings.DisableLogging();

    public static void TraceAndDebugFailException(Exception exception) => TeamFoundationTrace.TraceAndDebugFailException(TraceKeywordSets.General, exception);

    public static void TraceAndDebugFailException(string[] keywords, Exception exception) => TeamFoundationTrace.TraceException(keywords, exception);

    public static void TraceException(Exception exception) => TeamFoundationTrace.Error(TraceKeywordSets.General, (string) null, (string) null, exception);

    public static void TraceException(string[] keywords, Exception exception) => TeamFoundationTrace.Error(keywords, (string) null, (string) null, exception);

    public static void TraceException(string message, string method, Exception exception) => TeamFoundationTrace.Error(TraceKeywordSets.General, message, method, exception);

    public static void TraceException(string[] keywords, string method, Exception exception) => TeamFoundationTrace.Error(keywords, (string) null, method, exception);

    public static void TraceCallStack(string[] keywords)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Verbose))
        return;
      StackTrace stackTrace = new StackTrace(true);
      for (int index = 0; index < stackTrace.FrameCount - 1; ++index)
      {
        StackFrame frame = stackTrace.GetFrame(index);
        TeamFoundationTrace.Info("{0} {1} L{2}", (object) frame.GetMethod(), (object) frame.GetFileName(), (object) frame.GetFileLineNumber());
      }
    }

    public static void ErrorIf(bool condition, string info)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Error(info);
    }

    public static void ErrorIf(bool condition, string format, params object[] args)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Error(TraceKeywordSets.General, format, args);
    }

    public static void ErrorIf(
      bool condition,
      string[] keywords,
      string format,
      params object[] args)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Error(keywords, format, args);
    }

    public static void Error(string info) => TeamFoundationTrace.Error(TraceKeywordSets.General, info);

    public static void Error(string[] keywords, string info)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Error))
        return;
      TeamFoundationTrace.WriteLine(keywords, TraceLevel.Error, info);
    }

    public static void Error(string format, params object[] args) => TeamFoundationTrace.Error(TraceKeywordSets.General, format, args);

    public static void Error(string[] keywords, string format, params object[] args)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Error))
        return;
      TeamFoundationTrace.WriteLine(keywords, TraceLevel.Error, format, args);
    }

    public static void Error(string info, Exception exception) => TeamFoundationTrace.Error(TraceKeywordSets.General, info, (string) null, exception);

    public static void Error(string[] keywords, Exception exception) => TeamFoundationTrace.Error(keywords, (string) null, (string) null, exception);

    public static void Error(string[] keywords, string info, string method, Exception exception)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Error))
        return;
      TeamFoundationTrace.ReportErrorCondition(keywords, info, method, exception);
    }

    public static void WarningIf(bool condition, string info)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Warning(TraceKeywordSets.General, info);
    }

    public static void Warning(string info) => TeamFoundationTrace.Warning(TraceKeywordSets.General, info);

    public static void Warning(string format, params object[] args) => TeamFoundationTrace.Warning(TraceKeywordSets.General, format, args);

    public static void WarningIf(bool condition, string format, params object[] args)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Warning(TraceKeywordSets.General, format, args);
    }

    public static void WarningIf(
      bool condition,
      string[] keywords,
      string format,
      params object[] args)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Warning(keywords, format, args);
    }

    public static void Warning(string[] keywords, string format, params object[] args)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Warning))
        return;
      TeamFoundationTrace.WriteLine(keywords, TraceLevel.Warning, format, args);
    }

    public static void Warning(string[] keywords, string info)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Warning))
        return;
      TeamFoundationTrace.WriteLine(keywords, TraceLevel.Warning, info);
    }

    public static void InfoIf(bool condition, string info)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Info(info);
    }

    public static void Info(string format, params object[] args) => TeamFoundationTrace.Info(TraceKeywordSets.General, format, args);

    public static void InfoIf(bool condition, string format, params object[] args)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Info(TraceKeywordSets.General, format, args);
    }

    public static void InfoIf(
      bool condition,
      string[] keywords,
      string format,
      params object[] args)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Info(keywords, format, args);
    }

    public static void Info(string information) => TeamFoundationTrace.Info(TraceKeywordSets.General, information);

    public static void Info(string[] keywords, string format, params object[] args)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Info))
        return;
      TeamFoundationTrace.WriteLine(keywords, TraceLevel.Info, format, args);
    }

    public static void Info(string[] keywords, string information)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Info))
        return;
      TeamFoundationTrace.WriteLine(keywords, TraceLevel.Info, information);
    }

    public static void Verbose(string info) => TeamFoundationTrace.Verbose(TraceKeywordSets.General, info);

    public static void VerboseIf(bool condition, string info)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Verbose(TraceKeywordSets.General, info);
    }

    public static void VerboseIf(bool condition, string format, params object[] args)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Verbose(TraceKeywordSets.General, format, args);
    }

    public static void VerboseIf(
      bool condition,
      string[] keywords,
      string format,
      params object[] args)
    {
      if (!condition)
        return;
      TeamFoundationTrace.Verbose(keywords, format, args);
    }

    public static void Verbose(string format, params object[] args) => TeamFoundationTrace.Verbose(TraceKeywordSets.General, format, args);

    public static void Verbose(string[] keywords, string format, params object[] args)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Verbose))
        return;
      TeamFoundationTrace.WriteLine(keywords, TraceLevel.Verbose, format, args);
    }

    public static void Verbose(string[] keywords, string info)
    {
      if (!TeamFoundationTrace.IsTracing(keywords, TraceLevel.Verbose))
        return;
      TeamFoundationTrace.WriteLine(keywords, TraceLevel.Verbose, info);
    }

    private static void WriteLine(
      string[] keywords,
      TraceLevel level,
      string format,
      params object[] args)
    {
      if (string.IsNullOrEmpty(format))
        return;
      object[] objArray1 = args;
      List<KeyValuePair<string, ICollection>> keyValuePairList = (List<KeyValuePair<string, ICollection>>) null;
      if (objArray1 != null)
      {
        for (int index = 0; index < objArray1.Length; ++index)
        {
          if (objArray1[index] is ICollection collection)
          {
            if (keyValuePairList == null)
            {
              keyValuePairList = new List<KeyValuePair<string, ICollection>>();
              objArray1 = new object[args.Length];
              args.CopyTo((Array) objArray1, 0);
            }
            KeyValuePair<string, ICollection> keyValuePair = new KeyValuePair<string, ICollection>("[Collection" + keyValuePairList.Count.ToString() + "]", collection);
            keyValuePairList.Add(keyValuePair);
            objArray1[index] = (object) keyValuePair.Key;
          }
        }
      }
      StringBuilder stringBuilder1 = TeamFoundationTrace.GetStringBuilder(keywords, level);
      int length = stringBuilder1.Length;
      StringBuilder stringBuilder2 = stringBuilder1;
      TeamFoundationTrace.TeamFoundationTraceFormatter provider = TeamFoundationTrace.TeamFoundationTraceFormatter.Create(level);
      string format1 = format;
      object[] objArray2 = objArray1;
      if (objArray2 == null)
        objArray2 = new object[1]{ (object) "null" };
      stringBuilder2.AppendFormat((IFormatProvider) provider, format1, objArray2);
      TeamFoundationTrace.WriteLine(stringBuilder1, level);
      if (keyValuePairList == null)
        return;
      foreach (KeyValuePair<string, ICollection> keyValuePair in keyValuePairList)
      {
        stringBuilder1.Length = length;
        stringBuilder1.Append(keyValuePair.Key);
        stringBuilder1.Append(" Count=");
        stringBuilder1.Append(keyValuePair.Value.Count);
        TeamFoundationTrace.WriteLine(stringBuilder1, level);
        foreach (object obj in (IEnumerable) keyValuePair.Value)
        {
          stringBuilder1.Length = length;
          stringBuilder1.Append(obj != null ? obj.ToString() : "null");
          TeamFoundationTrace.WriteLine(stringBuilder1, level);
        }
      }
    }

    private static void WriteLine(string[] keywords, TraceLevel level, string info)
    {
      if (string.IsNullOrEmpty(info))
        return;
      StringBuilder stringBuilder = TeamFoundationTrace.GetStringBuilder(keywords, level);
      stringBuilder.Append(info);
      TeamFoundationTrace.WriteLine(stringBuilder, level);
    }

    private static void WriteLine(StringBuilder formattedInfo, TraceLevel level)
    {
      try
      {
        string eventMessage = formattedInfo.ToString();
        TeamFoundationTrace.TraceSettings.EventProvider?.WriteMessageEvent(eventMessage, TeamFoundationTraceSettings.ConvertLevelToETW(level), 0L);
        switch (level)
        {
          case TraceLevel.Error:
            Trace.TraceError(eventMessage.ToString());
            break;
          case TraceLevel.Warning:
            Trace.TraceWarning(eventMessage.ToString());
            break;
          case TraceLevel.Info:
            Trace.TraceInformation(eventMessage.ToString());
            break;
          case TraceLevel.Verbose:
            Trace.WriteLine(eventMessage.ToString());
            break;
        }
      }
      catch (Exception ex)
      {
      }
    }

    private static StringBuilder GetStringBuilder(string[] keywords, TraceLevel level)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "[{0}, PID {1}, TID {2}, {3:yyyy}/{3:MM}/{3:dd} {3:HH}:{3:mm}:{3:ss}.{3:fff}, {4} ms, ", (object) level.ToString(), (object) TFCommonUtil.CurrentProcess.Id, (object) TeamFoundationTrace.GetCurrentThreadId(), (object) DateTime.UtcNow, (object) TeamFoundationTrace.TraceSettings.MillisecondsElapsed);
      if (keywords != null && keywords.Length != 0)
      {
        stringBuilder.Append(keywords[0]);
        for (int index = 1; index < keywords.Length; ++index)
        {
          stringBuilder.Append(',');
          stringBuilder.Append(keywords[index]);
        }
      }
      stringBuilder.Append("] ");
      return stringBuilder;
    }

    public static void Enter(string info) => TeamFoundationTrace.Enter(TraceKeywordSets.General, info);

    public static void Enter(string[] keywords, string info) => TeamFoundationTrace.Enter(keywords, TraceLevel.Verbose, info);

    public static void Enter(string[] keywords, TraceLevel level, string info) => TeamFoundationTrace.SafeEnter(keywords, level, info);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int SafeEnter(string[] keywords, TraceLevel level, string info)
    {
      if (!string.IsNullOrEmpty(info))
      {
        if (TeamFoundationTrace.IsTracing(keywords, level))
        {
          try
          {
            TeamFoundationTrace.WriteLine(keywords, level, info + " {");
            TeamFoundationTrace.s_traceStack.Push(HighResTimer.TimeStamp);
            return TeamFoundationTrace.s_traceStack.Count;
          }
          catch
          {
          }
        }
      }
      return -1;
    }

    public static void Exit(string info) => TeamFoundationTrace.Exit(TraceKeywordSets.General, info);

    public static void Exit(string[] keywords, string info) => TeamFoundationTrace.Exit(keywords, TraceLevel.Verbose, info);

    public static void Exit(string[] keywords, TraceLevel level, string info) => TeamFoundationTrace.SafeExit(-1, keywords, level, info);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SafeExit(int traceToken, string[] keywords, TraceLevel level, string info)
    {
      if (string.IsNullOrEmpty(info))
        return;
      if (!TeamFoundationTrace.IsTracing(keywords, level))
        return;
      try
      {
        long timeStamp = HighResTimer.TimeStamp;
        long start;
        if (TeamFoundationTrace.s_traceStack.Count > 0)
        {
          if (traceToken < 1)
            start = TeamFoundationTrace.s_traceStack.Pop();
          else if (TeamFoundationTrace.s_traceStack.Count == traceToken)
            start = TeamFoundationTrace.s_traceStack.Pop();
          else if (TeamFoundationTrace.s_traceStack.Count > traceToken)
          {
            TeamFoundationTrace.WriteLine(keywords, level, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TeamFoundationTrace.SafeExit() called with mismatched trace token. Stack depth = {0}, token = {1}", (object) TeamFoundationTrace.s_traceStack.Count, (object) traceToken));
            while (TeamFoundationTrace.s_traceStack.Count > traceToken)
              TeamFoundationTrace.s_traceStack.Pop();
            start = TeamFoundationTrace.s_traceStack.Pop();
          }
          else
          {
            TeamFoundationTrace.WriteLine(keywords, level, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TeamFoundationTrace.SafeExit() called with mismatched trace token. Stack depth = {0}, token = {1}", (object) TeamFoundationTrace.s_traceStack.Count, (object) traceToken));
            start = timeStamp;
          }
        }
        else
          start = timeStamp;
        string info1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} }} /* ({1} usecs) */", (object) info, (object) HighResTimer.ElapsedTime(timeStamp, start));
        TeamFoundationTrace.WriteLine(keywords, level, info1);
      }
      catch
      {
      }
    }

    [Conditional("DEBUG")]
    public static void MethodEntry(params object[] parameters)
    {
    }

    [Conditional("DEBUG")]
    public static void MethodEntry(int depth, params object[] parameters)
    {
      if (!TeamFoundationTrace.IsTracing(TraceKeywordSets.API, TraceLevel.Verbose))
        return;
      if (depth < int.MaxValue)
        ++depth;
      MethodBase method = new StackFrame(depth).GetMethod();
      TeamFoundationTrace.WriteLine(TraceKeywordSets.API, TraceLevel.Verbose, TeamFoundationTrace.GetMethodSignature(nameof (MethodEntry), method));
      if (parameters == null || parameters.Length == 0)
        return;
      StringBuilder stringBuilder = new StringBuilder("Input parameters are: ");
      bool flag = true;
      foreach (object parameter in parameters)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(", ");
        stringBuilder.Append(parameter);
      }
      TeamFoundationTrace.WriteLine(TraceKeywordSets.API, TraceLevel.Verbose, stringBuilder.ToString());
    }

    [Conditional("DEBUG")]
    public static void MethodExit()
    {
    }

    [Conditional("DEBUG")]
    public static void MethodExit(int depth)
    {
      if (!TeamFoundationTrace.IsTracing(TraceKeywordSets.API, TraceLevel.Verbose))
        return;
      if (depth < int.MaxValue)
        ++depth;
      MethodBase method = new StackFrame(depth).GetMethod();
      TeamFoundationTrace.WriteLine(TraceKeywordSets.API, TraceLevel.Verbose, TeamFoundationTrace.GetMethodSignature(nameof (MethodExit), method));
    }

    private static string GetMethodSignature(string prefix, MethodBase method) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} method of type {2}", (object) prefix, (object) method.ToString(), (object) method.DeclaringType.FullName);

    private static Stack<long> s_traceStack
    {
      get
      {
        if (TeamFoundationTrace.s_dataStoreSlot == null)
        {
          lock (TeamFoundationTrace.s_lock)
          {
            if (TeamFoundationTrace.s_dataStoreSlot == null)
            {
              LocalDataStoreSlot localDataStoreSlot = Thread.AllocateDataSlot();
              Thread.MemoryBarrier();
              TeamFoundationTrace.s_dataStoreSlot = localDataStoreSlot;
            }
          }
        }
        Stack<long> data = (Stack<long>) Thread.GetData(TeamFoundationTrace.s_dataStoreSlot);
        if (data == null)
        {
          data = new Stack<long>();
          Thread.SetData(TeamFoundationTrace.s_dataStoreSlot, (object) data);
        }
        return data;
      }
    }

    private static void ReportErrorCondition(
      string[] keywords,
      string info,
      string method,
      Exception e)
    {
      StringBuilder stringBuilder = TeamFoundationTrace.GetStringBuilder(keywords, TraceLevel.Error);
      int length = stringBuilder.Length;
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("{");
      bool flag = false;
      if (!string.IsNullOrEmpty(info))
      {
        stringBuilder.AppendLine(info);
        flag = true;
      }
      if (!string.IsNullOrEmpty(method))
      {
        if (flag)
          stringBuilder.AppendLine();
        stringBuilder.AppendLine(TFCommonResources.Method((object) method));
      }
      stringBuilder.Append(TFCommonResources.Exception());
      stringBuilder.Append(" {");
      stringBuilder.Append(TeamFoundationExceptionFormatter.FormatException(e, false));
      stringBuilder.AppendLine(" }");
      stringBuilder.Append("}");
      TeamFoundationTrace.WriteLine(stringBuilder, TraceLevel.Error);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TeamFoundationTraceSettings TraceSettings
    {
      get
      {
        if (TeamFoundationTrace.s_traceSettings == null)
        {
          TeamFoundationTrace.s_traceSettings = new TeamFoundationTraceSettings();
          Trace.AutoFlush = true;
        }
        return TeamFoundationTrace.s_traceSettings;
      }
    }

    [DllImport("kernel32")]
    private static extern int GetCurrentThreadId();

    private class TeamFoundationTraceFormatter : IFormatProvider, ICustomFormatter
    {
      private static TeamFoundationTrace.TeamFoundationTraceFormatter[] s_formatters;

      public static TeamFoundationTrace.TeamFoundationTraceFormatter Create(TraceLevel traceLevel)
      {
        if (TeamFoundationTrace.TeamFoundationTraceFormatter.s_formatters == null)
          TeamFoundationTrace.TeamFoundationTraceFormatter.s_formatters = TeamFoundationTrace.TeamFoundationTraceFormatter.CreateFormatters();
        return TeamFoundationTrace.TeamFoundationTraceFormatter.s_formatters[(int) traceLevel];
      }

      private static TeamFoundationTrace.TeamFoundationTraceFormatter[] CreateFormatters()
      {
        int length = Enum.GetNames(typeof (TraceLevel)).Length;
        TeamFoundationTrace.TeamFoundationTraceFormatter[] formatters = new TeamFoundationTrace.TeamFoundationTraceFormatter[length];
        for (int index = 0; index < length; ++index)
          formatters[index] = new TeamFoundationTrace.TeamFoundationTraceFormatter((TraceLevel) index);
        return formatters;
      }

      private TeamFoundationTraceFormatter(TraceLevel traceLevel) => this.TraceLevel = traceLevel;

      public TraceLevel TraceLevel { get; private set; }

      public object GetFormat(Type formatType) => formatType == typeof (ICustomFormatter) ? (object) this : (object) null;

      public string Format(string format, object arg, IFormatProvider formatProvider)
      {
        switch (arg)
        {
          case null:
            return string.Empty;
          case ITeamFoundationTraceable foundationTraceable:
            return foundationTraceable.GetTraceString(this.TraceLevel);
          case IFormattable formattable:
            return formattable.ToString(format, (IFormatProvider) CultureInfo.InvariantCulture);
          default:
            return arg.ToString();
        }
      }
    }
  }
}
