// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.TraceLevelHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Diagnostics;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal class TraceLevelHelper
  {
    private static TraceEventType[] EtwLevelToTraceEventType = new TraceEventType[6]
    {
      TraceEventType.Critical,
      TraceEventType.Critical,
      TraceEventType.Error,
      TraceEventType.Warning,
      TraceEventType.Information,
      TraceEventType.Verbose
    };

    private static TraceEventType EtwOpcodeToTraceEventType(TraceEventOpcode opcode)
    {
      switch (opcode)
      {
        case TraceEventOpcode.Start:
          return TraceEventType.Start;
        case TraceEventOpcode.Stop:
          return TraceEventType.Stop;
        case TraceEventOpcode.Resume:
          return TraceEventType.Resume;
        case TraceEventOpcode.Suspend:
          return TraceEventType.Suspend;
        default:
          return TraceEventType.Information;
      }
    }

    internal static TraceEventType GetTraceEventType(byte level, byte opcode) => opcode == (byte) 0 ? TraceLevelHelper.EtwLevelToTraceEventType[(int) level] : TraceLevelHelper.EtwOpcodeToTraceEventType((TraceEventOpcode) opcode);

    internal static TraceEventType GetTraceEventType(TraceEventLevel level) => TraceLevelHelper.EtwLevelToTraceEventType[(int) level];

    internal static TraceEventType GetTraceEventType(byte level) => TraceLevelHelper.EtwLevelToTraceEventType[(int) level];

    internal static string LookupSeverity(TraceEventLevel level, TraceEventOpcode opcode)
    {
      string str;
      switch (opcode)
      {
        case TraceEventOpcode.Info:
          switch (level)
          {
            case TraceEventLevel.Critical:
              str = "Critical";
              break;
            case TraceEventLevel.Error:
              str = "Error";
              break;
            case TraceEventLevel.Warning:
              str = "Warning";
              break;
            case TraceEventLevel.Informational:
              str = "Information";
              break;
            case TraceEventLevel.Verbose:
              str = "Verbose";
              break;
            default:
              str = level.ToString();
              break;
          }
          break;
        case TraceEventOpcode.Start:
          str = "Start";
          break;
        case TraceEventOpcode.Stop:
          str = "Stop";
          break;
        case TraceEventOpcode.Resume:
          str = "Resume";
          break;
        case TraceEventOpcode.Suspend:
          str = "Suspend";
          break;
        default:
          str = opcode.ToString();
          break;
      }
      return str;
    }
  }
}
