// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.INotificationTrace
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public interface INotificationTrace
  {
    bool IsTracing(int tracepoint, TraceLevel level, string[] tags = null);

    void LogMessage(string message);

    void LogMessage(string message, params object[] args);

    void TraceMessage(int tracepoint, string message, params object[] args);

    void TraceAlways(int tracepoint, string message);

    void LogWarning(string message);

    void TraceWarning(int tracepoint, string message);

    void LogError(string message);

    void TraceError(int tracepoint, string message);

    void TraceException(int tracepoint, Exception exception, string message = null);

    void TraceEnter(int tracepoint, [CallerMemberName] string methodName = null);

    void TraceLeave(int tracepoint, [CallerMemberName] string methodName = null);
  }
}
