// Decompiled with JetBrains decompiler
// Type: System.Diagnostics.TraceSourceExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

namespace System.Diagnostics
{
  public static class TraceSourceExtensions
  {
    public static void TraceVerbose(this TraceSource traceSource, string msg) => TraceSourceExtensions.Trace(traceSource, TraceEventType.Verbose, msg);

    public static void TraceVerbose(
      this TraceSource traceSource,
      string format,
      params object[] args)
    {
      TraceSourceExtensions.Trace(traceSource, TraceEventType.Verbose, format, args);
    }

    public static void TraceWarning(this TraceSource traceSource, string msg) => TraceSourceExtensions.Trace(traceSource, TraceEventType.Warning, msg);

    public static void TraceWarning(
      this TraceSource traceSource,
      string format,
      params object[] args)
    {
      TraceSourceExtensions.Trace(traceSource, TraceEventType.Warning, format, args);
    }

    public static void TraceError(this TraceSource traceSource, string msg) => TraceSourceExtensions.Trace(traceSource, TraceEventType.Error, msg);

    public static void TraceError(
      this TraceSource traceSource,
      string format,
      params object[] args)
    {
      TraceSourceExtensions.Trace(traceSource, TraceEventType.Error, format, args);
    }

    private static void Trace(TraceSource traceSource, TraceEventType eventType, string msg) => traceSource.TraceEvent(eventType, 0, msg);

    private static void Trace(
      TraceSource traceSource,
      TraceEventType eventType,
      string format,
      params object[] args)
    {
      traceSource.TraceEvent(eventType, 0, format, args);
    }
  }
}
