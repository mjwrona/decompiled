// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.NoopAppTraceSource
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public class NoopAppTraceSource : IAppTraceSource
  {
    public static NoopAppTraceSource Instance = new NoopAppTraceSource();

    private NoopAppTraceSource()
    {
    }

    public TraceListenerCollection Listeners => (TraceListenerCollection) null;

    public bool HasError => false;

    public SourceLevels SwitchLevel => SourceLevels.Off;

    public void AddConsoleTraceListener()
    {
    }

    public void AddFileTraceListener(string fullFileName)
    {
    }

    public void Critical(string format, params object[] args)
    {
    }

    public void Critical(int id, string format, params object[] args)
    {
    }

    public void Critical(Exception ex)
    {
    }

    public void Critical(int id, Exception ex)
    {
    }

    public void Critical(Exception ex, string format, params object[] args)
    {
    }

    public void Critical(int id, Exception ex, string format, params object[] args)
    {
    }

    public void Error(string format, params object[] args)
    {
    }

    public void Error(int id, string format, params object[] args)
    {
    }

    public void Error(Exception ex)
    {
    }

    public void Error(int id, Exception ex)
    {
    }

    public void Error(Exception ex, string format, params object[] args)
    {
    }

    public void Error(int id, Exception ex, string format, params object[] args)
    {
    }

    public void Info(string format, params object[] args)
    {
    }

    public void Info(int id, string format, params object[] args)
    {
    }

    public void TraceEvent(TraceEventType eventType, int id)
    {
    }

    public void TraceEvent(TraceEventType eventType, int id, string message)
    {
    }

    public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
    {
    }

    public void Verbose(string format, params object[] args)
    {
    }

    public void Verbose(int id, string format, params object[] args)
    {
    }

    public void Verbose(Exception ex)
    {
    }

    public void Verbose(int id, Exception ex)
    {
    }

    public void Verbose(Exception ex, string format, params object[] args)
    {
    }

    public void Verbose(int id, Exception ex, string format, params object[] args)
    {
    }

    public void Warn(string format, params object[] args)
    {
    }

    public void Warn(int id, string format, params object[] args)
    {
    }

    public void Warn(Exception ex)
    {
    }

    public void Warn(int id, Exception ex)
    {
    }

    public void Warn(Exception ex, string format, params object[] args)
    {
    }

    public void Warn(int id, Exception ex, string format, params object[] args)
    {
    }

    public void ResetErrorDetection()
    {
    }
  }
}
