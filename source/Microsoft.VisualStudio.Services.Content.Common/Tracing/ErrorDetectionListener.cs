// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.ErrorDetectionListener
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  internal class ErrorDetectionListener : TraceListener
  {
    internal bool HasError { get; private set; }

    internal void Reset() => this.HasError = false;

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string message)
    {
      switch (eventType)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          this.HasError = true;
          break;
      }
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string format,
      params object[] args)
    {
      switch (eventType)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          this.HasError = true;
          break;
      }
    }

    public override void Write(string message)
    {
    }

    public override void WriteLine(string message)
    {
    }
  }
}
