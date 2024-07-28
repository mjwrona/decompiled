// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Tracing.AppTraceListener
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common.Tracing
{
  public abstract class AppTraceListener : DefaultTraceListener
  {
    protected const string TimeFormat = "o";

    public virtual bool DetailedMessageFormat { get; set; }

    public string AppName { get; set; }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string message)
    {
      base.TraceEvent(eventCache, source, eventType, id, this.FormatMessage(eventType, id, message));
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string format,
      params object[] args)
    {
      base.TraceEvent(eventCache, source, eventType, id, this.FormatMessage(eventType, id, format, args));
    }

    protected virtual string FormatMessage(
      TraceEventType eventType,
      int id,
      string message,
      params object[] args)
    {
      string str1 = DateTime.Now.ToString("o");
      string str2 = SafeStringFormat.FormatSafe(message, args);
      if (this.DetailedMessageFormat)
      {
        string str3;
        switch (eventType)
        {
          case TraceEventType.Critical:
            str3 = "[CRIT]   ";
            break;
          case TraceEventType.Error:
            str3 = "[ERROR]  ";
            break;
          case TraceEventType.Warning:
            str3 = "[WARNING]";
            break;
          case TraceEventType.Information:
            str3 = "[INFO]   ";
            break;
          case TraceEventType.Verbose:
            str3 = "[VERBOSE]";
            break;
          default:
            str3 = "[" + eventType.ToString().ToUpper() + "]";
            break;
        }
        if (id > 0)
          str2 = SafeStringFormat.FormatSafe(string.Format("[{0}][{1}]{2} {3}  {4}", (object) id, (object) this.AppName, (object) str3, (object) str1, (object) str2));
        else
          str2 = SafeStringFormat.FormatSafe("[" + this.AppName + "]" + str3 + " " + str1 + "  " + str2);
      }
      return str2;
    }
  }
}
