// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.IAppTraceSource
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public interface IAppTraceSource
  {
    TraceListenerCollection Listeners { get; }

    SourceLevels SwitchLevel { get; }

    bool HasError { get; }

    void AddConsoleTraceListener();

    void AddFileTraceListener(string fullFileName);

    void ResetErrorDetection();

    void Info(string format, params object[] args);

    void Info(int id, string format, params object[] args);

    void Warn(string format, params object[] args);

    void Warn(int id, string format, params object[] args);

    void Warn(Exception ex);

    void Warn(int id, Exception ex);

    void Warn(Exception ex, string format, params object[] args);

    void Warn(int id, Exception ex, string format, params object[] args);

    void Error(string format, params object[] args);

    void Error(int id, string format, params object[] args);

    void Error(Exception ex);

    void Error(int id, Exception ex);

    void Error(Exception ex, string format, params object[] args);

    void Error(int id, Exception ex, string format, params object[] args);

    void Critical(string format, params object[] args);

    void Critical(int id, string format, params object[] args);

    void Critical(Exception ex);

    void Critical(int id, Exception ex);

    void Critical(Exception ex, string format, params object[] args);

    void Critical(int id, Exception ex, string format, params object[] args);

    void Verbose(string format, params object[] args);

    void Verbose(int id, string format, params object[] args);

    void Verbose(Exception ex);

    void Verbose(int id, Exception ex);

    void Verbose(Exception ex, string format, params object[] args);

    void Verbose(int id, Exception ex, string format, params object[] args);

    void TraceEvent(TraceEventType eventType, int id);

    void TraceEvent(TraceEventType eventType, int id, string message);

    void TraceEvent(TraceEventType eventType, int id, string format, params object[] args);
  }
}
