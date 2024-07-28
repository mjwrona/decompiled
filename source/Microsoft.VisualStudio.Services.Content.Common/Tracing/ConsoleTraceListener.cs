// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.ConsoleTraceListener
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public class ConsoleTraceListener : AppTraceListener
  {
    private TraceEventType CurrentTraceEventType { get; set; }

    public override void TraceData(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      object data)
    {
      this.CurrentTraceEventType = eventType;
      base.TraceData(eventCache, source, eventType, id, data);
    }

    public override void TraceData(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      params object[] data)
    {
      this.CurrentTraceEventType = eventType;
      base.TraceData(eventCache, source, eventType, id, data);
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id)
    {
      this.CurrentTraceEventType = eventType;
      base.TraceEvent(eventCache, source, eventType, id);
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string message)
    {
      this.CurrentTraceEventType = eventType;
      base.TraceEvent(eventCache, source, eventType, id, message);
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string format,
      params object[] args)
    {
      this.CurrentTraceEventType = eventType;
      base.TraceEvent(eventCache, source, eventType, id, format, args);
    }

    public override void WriteLine(string message)
    {
      if (this.CurrentTraceEventType == TraceEventType.Error || this.CurrentTraceEventType == TraceEventType.Critical)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.WriteColorMessage(message, ConsoleColor.Red, ConsoleTraceListener.\u003C\u003EO.\u003C0\u003E__WriteLine ?? (ConsoleTraceListener.\u003C\u003EO.\u003C0\u003E__WriteLine = new Action<string>(Console.WriteLine)));
        this.CurrentTraceEventType = TraceEventType.Information;
      }
      else if (this.CurrentTraceEventType == TraceEventType.Warning)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.WriteColorMessage(message, ConsoleColor.Yellow, ConsoleTraceListener.\u003C\u003EO.\u003C0\u003E__WriteLine ?? (ConsoleTraceListener.\u003C\u003EO.\u003C0\u003E__WriteLine = new Action<string>(Console.WriteLine)));
        this.CurrentTraceEventType = TraceEventType.Information;
      }
      else
        Console.WriteLine(message);
    }

    private void WriteColorMessage(string message, ConsoleColor color, Action<string> write)
    {
      ConsoleColor foregroundColor = Console.ForegroundColor;
      try
      {
        Console.ForegroundColor = color;
        write(message);
      }
      finally
      {
        Console.ForegroundColor = foregroundColor;
      }
    }
  }
}
