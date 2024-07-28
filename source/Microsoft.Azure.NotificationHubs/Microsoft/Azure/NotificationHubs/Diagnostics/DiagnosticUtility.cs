// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.DiagnosticUtility
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal static class DiagnosticUtility
  {
    private const string TraceSourceName = "System.ServiceModel";
    internal const string EventSourceName = "System.ServiceModel 3.0.0.0";
    private static DiagnosticTrace diagnosticTrace = DiagnosticUtility.InitializeTracing();
    private static ExceptionUtility exceptionUtility;
    private static Utility utility;
    private static object lockObject = new object();

    public static void DebugAssert(bool condition, string debugText)
    {
    }

    public static void DebugAssert(string debugText)
    {
    }

    private static DiagnosticTrace InitializeTracing()
    {
      DiagnosticUtility.InitDiagnosticTraceImpl(TraceSourceKind.DiagnosticTraceSource, "System.ServiceModel");
      if (!DiagnosticUtility.diagnosticTrace.HaveListeners)
        DiagnosticUtility.diagnosticTrace = (DiagnosticTrace) null;
      return DiagnosticUtility.diagnosticTrace;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void InitDiagnosticTraceImpl(TraceSourceKind sourceType, string traceSourceName) => DiagnosticUtility.diagnosticTrace = new DiagnosticTrace(sourceType, traceSourceName, "System.ServiceModel 3.0.0.0");

    internal static bool ShouldTrace(TraceEventType type)
    {
      bool flag = false;
      if (DiagnosticUtility.TracingEnabled)
      {
        switch (type)
        {
          case TraceEventType.Critical:
            flag = DiagnosticUtility.ShouldTraceCritical;
            break;
          case TraceEventType.Error:
            flag = DiagnosticUtility.ShouldTraceError;
            break;
          case TraceEventType.Warning:
            flag = DiagnosticUtility.ShouldTraceWarning;
            break;
          case TraceEventType.Information:
            flag = DiagnosticUtility.ShouldTraceInformation;
            break;
          case TraceEventType.Verbose:
            flag = DiagnosticUtility.ShouldTraceVerbose;
            break;
        }
      }
      return flag;
    }

    internal static DiagnosticTrace DiagnosticTrace => DiagnosticUtility.diagnosticTrace;

    internal static ExceptionUtility ExceptionUtility => DiagnosticUtility.exceptionUtility ?? DiagnosticUtility.GetExceptionUtility();

    private static ExceptionUtility GetExceptionUtility()
    {
      lock (DiagnosticUtility.lockObject)
      {
        if (DiagnosticUtility.exceptionUtility == null)
          DiagnosticUtility.exceptionUtility = new ExceptionUtility((object) DiagnosticUtility.diagnosticTrace);
      }
      return DiagnosticUtility.exceptionUtility;
    }

    internal static bool ShouldUseActivity => DiagnosticUtility.DiagnosticTrace != null && DiagnosticUtility.DiagnosticTrace.ShouldTrace(TraceEventType.Start);

    internal static bool ShouldTraceError => DiagnosticUtility.DiagnosticTrace != null && DiagnosticUtility.DiagnosticTrace.ShouldTrace(TraceEventType.Error);

    internal static bool ShouldTraceInformation => DiagnosticUtility.DiagnosticTrace != null && DiagnosticUtility.DiagnosticTrace.ShouldTrace(TraceEventType.Information);

    internal static bool ShouldTraceVerbose => DiagnosticUtility.DiagnosticTrace != null && DiagnosticUtility.DiagnosticTrace.ShouldTrace(TraceEventType.Verbose);

    internal static bool ShouldTraceWarning => DiagnosticUtility.DiagnosticTrace != null && DiagnosticUtility.DiagnosticTrace.ShouldTrace(TraceEventType.Warning);

    internal static bool ShouldTraceCritical => DiagnosticUtility.DiagnosticTrace != null && DiagnosticUtility.DiagnosticTrace.ShouldTrace(TraceEventType.Critical);

    internal static AsyncCallback ThunkAsyncCallback(AsyncCallback callback) => callback;

    internal static bool TracingEnabled => DiagnosticUtility.DiagnosticTrace != null && DiagnosticUtility.DiagnosticTrace.TracingEnabled;

    internal static Utility Utility => DiagnosticUtility.utility ?? DiagnosticUtility.GetUtility();

    private static Utility GetUtility()
    {
      lock (DiagnosticUtility.lockObject)
      {
        if (DiagnosticUtility.utility == null)
          DiagnosticUtility.utility = new Utility(DiagnosticUtility.ExceptionUtility);
      }
      return DiagnosticUtility.utility;
    }
  }
}
