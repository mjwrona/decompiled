// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Core.Trace.DefaultTrace
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Core.Trace
{
  internal static class DefaultTrace
  {
    public static readonly Guid ProviderId = new Guid("{B30ABF1C-6A50-4F2B-85C4-61823ED6CF24}");
    private static readonly TraceSource TraceSourceInternal;
    private static bool IsListenerAdded;

    static DefaultTrace()
    {
      System.Diagnostics.Trace.UseGlobalLock = false;
      DefaultTrace.TraceSourceInternal = new TraceSource("DocDBTrace");
      DefaultTrace.RemoveDefaultTraceListener();
    }

    public static TraceSource TraceSource => DefaultTrace.TraceSourceInternal;

    public static void InitEventListener()
    {
      if (DefaultTrace.IsListenerAdded)
        return;
      DefaultTrace.IsListenerAdded = true;
      SourceSwitch sourceSwitch = new SourceSwitch("ClientSwitch", "Information");
      DefaultTrace.TraceSourceInternal.Switch = sourceSwitch;
    }

    public static void Flush() => DefaultTrace.TraceSource.Flush();

    public static void TraceVerbose(string message) => DefaultTrace.TraceSource.TraceEvent(TraceEventType.Verbose, 0, message);

    public static void TraceVerbose(string format, params object[] args) => DefaultTrace.TraceSource.TraceEvent(TraceEventType.Verbose, 0, format, args);

    public static void TraceInformation(string message) => DefaultTrace.TraceSource.TraceInformation(message);

    public static void TraceInformation(string format, params object[] args) => DefaultTrace.TraceSource.TraceInformation(format, args);

    public static void TraceWarning(string message) => DefaultTrace.TraceSource.TraceEvent(TraceEventType.Warning, 0, message);

    public static void TraceWarning(string format, params object[] args) => DefaultTrace.TraceSource.TraceEvent(TraceEventType.Warning, 0, format, args);

    public static void TraceError(string message) => DefaultTrace.TraceSource.TraceEvent(TraceEventType.Error, 0, message);

    public static void TraceError(string format, params object[] args) => DefaultTrace.TraceSource.TraceEvent(TraceEventType.Error, 0, format, args);

    public static void TraceCritical(string message) => DefaultTrace.TraceSource.TraceEvent(TraceEventType.Critical, 0, message);

    public static void TraceCritical(string format, params object[] args) => DefaultTrace.TraceSource.TraceEvent(TraceEventType.Critical, 0, format, args);

    public static void RemoveDefaultTraceListener()
    {
      if (Debugger.IsAttached || DefaultTrace.TraceSource.Listeners.Count <= 0)
        return;
      List<DefaultTraceListener> defaultTraceListenerList = new List<DefaultTraceListener>();
      foreach (object listener in DefaultTrace.TraceSource.Listeners)
      {
        if (listener is DefaultTraceListener defaultTraceListener)
          defaultTraceListenerList.Add(defaultTraceListener);
      }
      foreach (TraceListener listener in defaultTraceListenerList)
        DefaultTrace.TraceSource.Listeners.Remove(listener);
    }

    internal static void TraceMetrics(string name, params object[] values) => DefaultTrace.TraceInformation(string.Join<object>("|", ((IEnumerable<object>) new object[2]
    {
      (object) nameof (TraceMetrics),
      (object) name
    }).Concat<object>((IEnumerable<object>) values)));
  }
}
